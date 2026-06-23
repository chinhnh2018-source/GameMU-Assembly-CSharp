using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class ShengXiaoGuessManager
	{
		public int GuessMapCode
		{
			get
			{
				return this.MapCode;
			}
		}

		public void Init()
		{
			this.InitLegalGuessKeys();
			this.ReloadConfig(true);
			this.Reset();
		}

		public void ReloadConfig(bool throwAble = false)
		{
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("ShengXiaoGuessParams", ',');
			if (paramValueIntArrayByName.Length != 6)
			{
				if (throwAble)
				{
					throw new Exception("SystemParmas.xml中生肖竞猜参数ShengXiaoGuessParams配置个数不对");
				}
			}
			else
			{
				this.MapCode = paramValueIntArrayByName[0];
				this.WaitingEnterSecs = paramValueIntArrayByName[1];
				this.NeedGoodsID = paramValueIntArrayByName[2];
				this.SingleGoodsToYuanBaoNum = paramValueIntArrayByName[3];
				this.MaxMortgageForOnce = paramValueIntArrayByName[4];
				this.GateGoldForBroadcast = paramValueIntArrayByName[5];
				this.LegalServerLines.Clear();
				int[] paramValueIntArrayByName2 = GameManager.systemParamsList.GetParamValueIntArrayByName("ShengXiaoGuessLines", ',');
				foreach (int item in paramValueIntArrayByName2)
				{
					this.LegalServerLines.Add(item);
				}
			}
		}

		protected void InitLegalGuessKeys()
		{
			this.LegalGuessKeyList.Clear();
			for (int i = 0; i < 12; i++)
			{
				this.LegalGuessKeyList.Add(1 << i);
			}
			for (int i = 0; i < 6; i++)
			{
				this.LegalGuessKeyList.Add(3 << 2 * i);
			}
			for (int i = 0; i < 6; i++)
			{
				if (i < 5)
				{
					this.LegalGuessKeyList.Add(15 << 2 * i);
				}
				else
				{
					this.LegalGuessKeyList.Add(3075);
				}
			}
			for (int i = 0; i < 2; i++)
			{
				this.LegalGuessKeyList.Add(63 << 6 * i);
			}
		}

		protected void Reset()
		{
			this.GuessStates = ShengXiaoGuessStates.NoMortgage;
			lock (this.GuessItemListDict)
			{
				this.GuessItemListDict.Clear();
			}
			this.StateStartTicks = TimeUtil.NOW();
			this.IsBossKilled = false;
			this.ThisTimeCountDownSecs = 0L;
		}

		public void Process()
		{
			if (this.GuessStates > ShengXiaoGuessStates.NoMortgage)
			{
				this.ProcessGuessing();
			}
			else
			{
				this.ProcessNoGuess();
			}
		}

		protected void ProcessGuessing()
		{
			if (this.GuessStates == ShengXiaoGuessStates.MortgageCountDown)
			{
				long num = TimeUtil.NOW();
				if (num >= this.StateStartTicks + (long)(this.WaitingEnterSecs * 1000))
				{
					this.GuessStates = ShengXiaoGuessStates.BossCountDown;
					GameManager.MonsterZoneMgr.ReloadNormalMapMonsters(this.MapCode, 1);
					this.StateStartTicks = TimeUtil.NOW();
					this.ThisTimeCountDownSecs = (long)this.WaitingKillBossSecs;
					GameManager.ClientMgr.NotifyAllShengXiaoGuessStateMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, (int)this.GuessStates, (int)this.ThisTimeCountDownSecs, 0, this.GetPreGuessResult());
				}
			}
			else if (this.GuessStates == ShengXiaoGuessStates.BossCountDown)
			{
				if (this.WaitingKillBossSecs > 0)
				{
				}
				if (this.IsBossKilled)
				{
					this.GuessStates = ShengXiaoGuessStates.EndKillBoss;
				}
			}
			else if (this.GuessStates == ShengXiaoGuessStates.EndKillBoss)
			{
				int num2 = this.GenerateRandomShengXiao();
				this.AddGuessResultHistory(num2);
				GameManager.ClientMgr.NotifyAllShengXiaoGuessStateMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, (int)this.GuessStates, num2, 0, this.GetPreGuessResult());
				this.ProcessAwards(num2);
				this.GuessStates = ShengXiaoGuessStates.NoMortgage;
				GameManager.ClientMgr.NotifyAllShengXiaoGuessStateMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, (int)this.GuessStates, 0, 0, this.GetPreGuessResult());
				this.Reset();
			}
		}

		protected void ProcessNoGuess()
		{
			if (this.GuessItemListDict.Count > 0)
			{
				this.GuessStates = ShengXiaoGuessStates.MortgageCountDown;
				this.StateStartTicks = TimeUtil.NOW();
				this.ThisTimeCountDownSecs = (long)this.WaitingEnterSecs;
				GameManager.ClientMgr.NotifyAllShengXiaoGuessStateMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, (int)this.GuessStates, this.WaitingEnterSecs, 0, this.GetPreGuessResult());
			}
		}

		public int SubNeedGoods(GameClient client, int totalMortgageNum, bool allowAutoBuy = false)
		{
			int num = totalMortgageNum;
			int num2 = 0;
			if (Global.GetTotalGoodsCountByID(client, this.NeedGoodsID) < totalMortgageNum)
			{
				if (!allowAutoBuy)
				{
					return -3998;
				}
				num2 = Global.GMax(0, totalMortgageNum - Global.GetTotalGoodsCountByID(client, this.NeedGoodsID));
				num = totalMortgageNum - num2;
			}
			if (num > 0)
			{
				bool flag = false;
				bool flag2 = false;
				if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, this.NeedGoodsID, num, false, out flag, out flag2, false))
				{
					return -4010;
				}
			}
			if (num2 > 0)
			{
				int num3 = Global.SubUserMoneyForGoods(client, this.NeedGoodsID, num2, "生肖运程竞猜物品");
				if (num3 <= 0)
				{
					return num3;
				}
			}
			return 1;
		}

		public int IsMortgageLegal(int guessKey, int mortgageNum)
		{
			int result;
			if (this.GuessStates > ShengXiaoGuessStates.MortgageCountDown)
			{
				result = -3700;
			}
			else if (guessKey <= 0 || guessKey > 4095 || mortgageNum <= 0)
			{
				result = -3800;
			}
			else if (!this.LegalGuessKeyList.Contains(guessKey))
			{
				result = -3990;
			}
			else if (mortgageNum > this.MaxMortgageForOnce)
			{
				result = -3996;
			}
			else
			{
				result = 1;
			}
			return result;
		}

		public int AddGuess(GameClient client, int guessKey, int mortgageNum, bool allowAutoBuy = false)
		{
			int result = 1;
			int num = 0;
			Dictionary<int, int> dictionary = null;
			lock (this.GuessItemListDict)
			{
				if (this.GuessItemListDict.TryGetValue(client.ClientData.RoleID, out dictionary) && null != dictionary)
				{
					if (dictionary.TryGetValue(guessKey, out num))
					{
						dictionary[guessKey] = num + mortgageNum;
					}
					else
					{
						dictionary.Add(guessKey, mortgageNum);
					}
				}
				else
				{
					dictionary = new Dictionary<int, int>();
					dictionary.Add(guessKey, mortgageNum);
					this.GuessItemListDict.Add(client.ClientData.RoleID, dictionary);
				}
			}
			GameManager.SystemServerEvents.AddEvent(string.Format("扣除角色竞猜注码金币, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
			{
				client.ClientData.RoleID,
				client.ClientData.RoleName,
				client.ClientData.Gold,
				mortgageNum
			}), EventLevels.Record);
			return result;
		}

		public void OnBossKilled()
		{
			if (this.GuessStates == ShengXiaoGuessStates.BossCountDown)
			{
				this.IsBossKilled = true;
			}
		}

		public bool ClientEnter(GameClient gameClient)
		{
			bool result;
			if (this.LegalServerLines.IndexOf(GameManager.ServerLineID) < 0)
			{
				result = false;
			}
			else
			{
				bool flag = false;
				if (this.GuessStates < ShengXiaoGuessStates.EndKillBoss)
				{
					long num = TimeUtil.NOW();
					long num2 = num - this.StateStartTicks;
					if (this.ThisTimeCountDownSecs > 0L)
					{
						num2 = this.ThisTimeCountDownSecs * 1000L - num2;
					}
					if (num2 >= 1200L || this.ThisTimeCountDownSecs <= 0L)
					{
						GameManager.ClientMgr.NotifyClientShengXiaoGuessStateMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, (int)this.GuessStates, (int)(num2 / 1000L), 0, this.GetPreGuessResult());
					}
				}
				result = flag;
			}
			return result;
		}

		public void ClientLeave()
		{
		}

		public List<int> GetLegalGuessServerLines()
		{
			return this.LegalServerLines;
		}

		public Dictionary<int, int> GetRoleGuessDictionay(int roleID)
		{
			Dictionary<int, int> dictionary = null;
			lock (this.GuessItemListDict)
			{
				if (this.GuessItemListDict.TryGetValue(roleID, out dictionary) && null != dictionary)
				{
					return dictionary;
				}
			}
			return new Dictionary<int, int>();
		}

		public void ProcessAwards(int resultShengXiaoMask)
		{
			string text = "";
			string text2 = "";
			List<int> list = this.GuessItemListDict.Keys.ToList<int>();
			try
			{
				foreach (int num in list)
				{
					text = "";
					GameClient gameClient = GameManager.ClientMgr.FindClient(num);
					Dictionary<int, int> dictionary = null;
					if (this.GuessItemListDict.TryGetValue(num, out dictionary) && null != dictionary)
					{
						foreach (KeyValuePair<int, int> keyValuePair in dictionary)
						{
							if (text.Length > 0)
							{
								text += "|";
							}
							if ((keyValuePair.Key & resultShengXiaoMask) <= 0)
							{
								text += string.Format("{0}_{1}_{2}_{3}_{4}", new object[]
								{
									keyValuePair.Key,
									keyValuePair.Value,
									resultShengXiaoMask,
									0,
									(gameClient != null) ? gameClient.ClientData.Gold : -1
								});
								Global.AddShengXiaoGuessHistoryToStaticsDB(gameClient, num, keyValuePair.Key, keyValuePair.Value, resultShengXiaoMask, 0, (gameClient != null) ? gameClient.ClientData.Gold : -1);
							}
							else
							{
								int multipleByGuessKey = this.GetMultipleByGuessKey(keyValuePair.Key);
								if (multipleByGuessKey > 0)
								{
									int num2 = multipleByGuessKey * keyValuePair.Value;
									int num3 = num2 * this.SingleGoodsToYuanBaoNum;
									if (num3 > 0)
									{
										GameManager.ClientMgr.AddUserGoldOffLine(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, num, num3, "角色竞猜", string.Concat(num));
										if (null != gameClient)
										{
											GameManager.SystemServerEvents.AddEvent(string.Format("角色竞猜获取金币, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
											{
												gameClient.ClientData.RoleID,
												gameClient.ClientData.RoleName,
												gameClient.ClientData.Gold,
												num3
											}), EventLevels.Record);
											if (num3 >= this.GateGoldForBroadcast)
											{
												Global.BroadcastShengXiaoGuessWinHint(gameClient, multipleByGuessKey, Global.GetShengXiaoNameByCode(resultShengXiaoMask), num3);
											}
										}
										else
										{
											GameManager.SystemServerEvents.AddEvent(string.Format("角色竞猜获取金币, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
											{
												num,
												"离线角色",
												"未知",
												num3
											}), EventLevels.Record);
										}
										text += string.Format("{0}_{1}_{2}_{3}_{4}", new object[]
										{
											keyValuePair.Key,
											keyValuePair.Value,
											resultShengXiaoMask,
											num3,
											(gameClient != null) ? gameClient.ClientData.Gold : -1
										});
										Global.AddShengXiaoGuessHistoryToStaticsDB(gameClient, num, keyValuePair.Key, keyValuePair.Value, resultShengXiaoMask, num3, (gameClient != null) ? gameClient.ClientData.Gold : -1);
									}
								}
							}
						}
						if (null != gameClient)
						{
							GameManager.ClientMgr.NotifyShengXiaoGuessResultMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, text);
						}
						if (text.Length > 0)
						{
							if (text2.Length > 0)
							{
								text2 += ";";
							}
							text2 += string.Format("{0},{1}", num, text);
						}
						if (text2.Length > 1200)
						{
							GameManager.DBCmdMgr.AddDBCmd(10094, string.Format("{0}", text2), null, 0);
							text2 = "";
						}
					}
				}
				if (text2.Length > 0)
				{
					GameManager.DBCmdMgr.AddDBCmd(10094, string.Format("{0}", text2), null, 0);
				}
			}
			catch
			{
			}
			lock (this.GuessItemListDict)
			{
				this.GuessItemListDict.Clear();
			}
		}

		public int GenerateRandomShengXiao()
		{
			int randomNumber = Global.GetRandomNumber(0, 12);
			return 1 << randomNumber;
		}

		protected int GetMultipleByGuessKey(int guessKey)
		{
			int num = 0;
			int result;
			if (this.AwardMultipleDict.TryGetValue(guessKey, out num))
			{
				result = num;
			}
			else if (guessKey <= 0)
			{
				result = -3000;
			}
			else
			{
				int num2 = 0;
				for (int i = 0; i < 12; i++)
				{
					num2 += (guessKey >> i & 1);
				}
				if (num2 <= 0 || num2 > 12)
				{
					result = -3001;
				}
				else
				{
					int num3 = 12 / num2;
					if (this.AwardMultipleDict.Count > 50)
					{
						this.AwardMultipleDict.Clear();
					}
					this.AwardMultipleDict.Add(guessKey, num3);
					result = num3;
				}
			}
			return result;
		}

		protected void AddGuessResultHistory(int result)
		{
			lock (this.ShengXiaoGuessResultHistory)
			{
				if (this.ShengXiaoGuessResultHistory.Count > 10)
				{
					this.ShengXiaoGuessResultHistory.RemoveAt(0);
				}
				this.ShengXiaoGuessResultHistory.Add(result);
			}
		}

		public string GetGuessResultHistory()
		{
			string text = "";
			lock (this.ShengXiaoGuessResultHistory)
			{
				foreach (int num in this.ShengXiaoGuessResultHistory)
				{
					if (text.Length > 0)
					{
						text += "|";
					}
					text += string.Format("{0}", num);
				}
			}
			return text;
		}

		private int GetPreGuessResult()
		{
			lock (this.ShengXiaoGuessResultHistory)
			{
				if (this.ShengXiaoGuessResultHistory.Count > 0)
				{
					return this.ShengXiaoGuessResultHistory[this.ShengXiaoGuessResultHistory.Count - 1];
				}
			}
			return 0;
		}

		private int MapCode = -1;

		private int WaitingEnterSecs = 120;

		private int WaitingKillBossSecs = 0;

		private long ThisTimeCountDownSecs = 0L;

		private int MaxMortgageForOnce = 100000;

		private int NeedGoodsID = -1;

		private int SingleGoodsToYuanBaoNum = 100;

		private int GateGoldForBroadcast = 10000;

		private List<int> LegalServerLines = new List<int>();

		private ShengXiaoGuessStates GuessStates = ShengXiaoGuessStates.NoMortgage;

		private Dictionary<int, int> AwardMultipleDict = new Dictionary<int, int>();

		private List<int> LegalGuessKeyList = new List<int>();

		private long StateStartTicks = 0L;

		private bool IsBossKilled = false;

		private List<int> ShengXiaoGuessResultHistory = new List<int>();

		private Dictionary<int, Dictionary<int, int>> GuessItemListDict = new Dictionary<int, Dictionary<int, int>>();
	}
}
