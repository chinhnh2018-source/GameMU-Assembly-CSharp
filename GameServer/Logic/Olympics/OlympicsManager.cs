using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace GameServer.Logic.Olympics
{
	public class OlympicsManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListenerEx
	{
		public static OlympicsManager getInstance()
		{
			return OlympicsManager.instance;
		}

		public bool initialize()
		{
			this.InitOlympics();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1050, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1051, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1059, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1052, 2, 2, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1053, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1054, 2, 2, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1055, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1056, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1057, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1058, 2, 2, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1061, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1060, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return true;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 1050:
				return this.ProcessOlympicsGradeCmd(client, nID, bytes, cmdParams);
			case 1051:
				return this.ProcessOlympicsGameCountCmd(client, nID, bytes, cmdParams);
			case 1052:
				return this.ProcessOlympicsGameOperateCmd(client, nID, bytes, cmdParams);
			case 1054:
				return this.ProcessOlympicsGuessSubCmd(client, nID, bytes, cmdParams);
			case 1055:
				return this.ProcessOlympicsGuessListCmd(client, nID, bytes, cmdParams);
			case 1056:
				return this.ProcessOlympicsRankCmd(client, nID, bytes, cmdParams);
			case 1057:
				return this.ProcessOlympicsShopListCmd(client, nID, bytes, cmdParams);
			case 1058:
				return this.ProcessOlympicsShopBuyCmd(client, nID, bytes, cmdParams);
			case 1059:
				return this.ProcessOlympicsGameBeginCmd(client, nID, bytes, cmdParams);
			case 1060:
				return this.ProcessOlympicsAwardStateCmd(client, nID, bytes, cmdParams);
			case 1061:
				return this.ProcessOlympicsAwardCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		public void processEvent(EventObjectEx eventObject)
		{
			throw new NotImplementedException();
		}

		private bool ProcessOlympicsGradeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				if (!this._olympicsIsOpen)
				{
					client.sendCmd(nID, "0:0", false);
					return true;
				}
				string cmdData = this.OlympicsGradeGet(client);
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessOlympicsGameCountCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				if (!this._olympicsIsOpen)
				{
					client.sendCmd(nID, "0:0", false);
					return true;
				}
				lock (this._mutex)
				{
					this.JudgeClearOlympicsActivityData(client);
					int olympicsDay = this.GetOlympicsDay();
					int[] array = this.OlympicsGameCountGet(client, EGameType.Shoot);
					if (array[0] != olympicsDay)
					{
						int[] array2 = new int[5];
						array2[0] = olympicsDay;
						array = array2;
						this.OlympicsGameCountSet(client, EGameType.Shoot, string.Join<int>(",", array));
					}
					int[] array3 = this.OlympicsGameCountGet(client, EGameType.Football);
					if (array3[0] != olympicsDay)
					{
						int[] array2 = new int[5];
						array2[0] = olympicsDay;
						array3 = array2;
						this.OlympicsGameCountSet(client, EGameType.Football, string.Join<int>(",", array3));
					}
					int num = array[1];
					if (array[4] > 0)
					{
						num--;
					}
					int num2 = array3[1];
					if (array3[4] > 0)
					{
						num2--;
					}
					client.sendCmd(1051, string.Format("{0}:{1}", num, num2), false);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessOlympicsGameBeginCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				lock (this._mutex)
				{
					int gameType = int.Parse(cmdParams[0]);
					string cmdData = this.OlympicsGameBegin(client, (EGameType)gameType);
					client.sendCmd(nID, cmdData, false);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private string OlympicsGameBegin(GameClient client, EGameType gameType)
		{
			string format = "{0}:{1}:{2}:{3}:{4}";
			string result;
			if (!this._olympicsIsOpen)
			{
				result = string.Format(format, new object[]
				{
					-1,
					(int)gameType,
					0,
					0,
					0
				});
			}
			else
			{
				int[] array = this.OlympicsGameCountGet(client, gameType);
				OlympicsGameInfo olympicsGameInfo = null;
				this._gameDic.TryGetValue((int)gameType, out olympicsGameInfo);
				if (olympicsGameInfo == null)
				{
					result = string.Format(format, new object[]
					{
						-6,
						(int)gameType,
						0,
						0,
						0
					});
				}
				else if (array[4] > 0 && array[2] >= 0 && array[2] < olympicsGameInfo.CountGame)
				{
					result = string.Format(format, new object[]
					{
						1,
						(int)gameType,
						array[1],
						array[2],
						array[3]
					});
				}
				else if (array[1] >= olympicsGameInfo.CountFree + olympicsGameInfo.CountDiamond)
				{
					result = string.Format(format, new object[]
					{
						-7,
						(int)gameType,
						0,
						0,
						0
					});
				}
				else
				{
					if (array[1] >= olympicsGameInfo.CountFree)
					{
						int subMoney = olympicsGameInfo.DiamondList[array[1] - olympicsGameInfo.CountFree];
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, subMoney, "奥运次数", true, false, false, DaiBiSySType.None))
						{
							return string.Format(format, new object[]
							{
								-8,
								(int)gameType,
								0,
								0,
								0
							});
						}
					}
					array[1]++;
					array[2] = 0;
					array[3] = 0;
					array[4] = 1;
					this.OlympicsGameCountSet(client, gameType, string.Join<int>(",", array));
					result = string.Format(format, new object[]
					{
						1,
						(int)gameType,
						array[1],
						array[2],
						array[3]
					});
				}
			}
			return result;
		}

		private bool ProcessOlympicsGameOperateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			string format = "{0}:{1}:{2}";
			try
			{
				lock (this._mutex)
				{
					if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 2))
					{
						return false;
					}
					int num = int.Parse(cmdParams[0]);
					int num2 = int.Parse(cmdParams[1]);
					string text = this.OlympicsGameBegin(client, (EGameType)num);
					int[] array = StringUtil.StringToIntArr(text, ':');
					if (array[0] != 1)
					{
						client.sendCmd(nID, string.Format(format, array[0], num, 0), false);
						return true;
					}
					int[] array2 = this.OlympicsGameCountGet(client, (EGameType)num);
					if (array2[4] == 0)
					{
						client.sendCmd(nID, string.Format(format, 0, num, 0), false);
						return true;
					}
					int olympicsDay = this.GetOlympicsDay();
					if (olympicsDay != array2[0])
					{
						array2[0] = olympicsDay;
						array2[1] = 1;
					}
					array2[2]++;
					int num3;
					if (num == 1)
					{
						if (num2 < 0 || num2 > 10)
						{
							client.sendCmd(nID, string.Format(format, 0, num, 0), false);
							return true;
						}
						array2[3] += num2;
						num3 = array2[3];
					}
					else
					{
						int num4 = 0;
						int randomNumber = RandomHelper.GetRandomNumber(0, 100);
						int num5 = (int)(this.GetFootballRate() * 100.0);
						if (randomNumber < num5)
						{
							num4 = 1;
						}
						array2[3] += num4;
						num3 = num4;
					}
					OlympicsGameInfo olympicsGameInfo = this._gameDic[num];
					if (array2[2] >= olympicsGameInfo.CountGame)
					{
						int num6 = (array2[3] >= olympicsGameInfo.CountWin) ? 1 : 0;
						int num7 = array2[3];
						int num8 = (num6 > 0) ? olympicsGameInfo.GradeWin : olympicsGameInfo.GradeLost;
						array2[2] = 0;
						array2[3] = 0;
						array2[4] = 0;
						this.OlympicsGradeAdd(client, num8);
						client.sendCmd(1053, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							num6,
							num7,
							num8
						}), false);
						this.CheckTip(client);
					}
					this.OlympicsGameCountSet(client, (EGameType)num, string.Join<int>(",", array2));
					int[] array3 = this.OlympicsGameCountGet(client, EGameType.Shoot);
					int[] array4 = this.OlympicsGameCountGet(client, EGameType.Football);
					int num9 = array3[1];
					if (array3[4] > 0)
					{
						num9--;
					}
					int num10 = array4[1];
					if (array4[4] > 0)
					{
						num10--;
					}
					client.sendCmd(1051, string.Format("{0}:{1}", num9, num10), false);
					client.sendCmd(nID, string.Format(format, 1, num, num3), false);
					return true;
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessOlympicsGuessSubCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 2))
				{
					return false;
				}
				if (!this._olympicsIsOpen)
				{
					client.sendCmd<int>(nID, -1, false);
					return true;
				}
				string text = cmdParams[0];
				int num = int.Parse(cmdParams[1]);
				int[] array = StringUtil.StringToIntArr(text, ',');
				if (array.Length != 3)
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				if (num != this.GetOlympicsDayFromBegin())
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				int olympicsDay = this.GetOlympicsDay();
				OlympicsGuessDataDB olympicsGuessDataDB = this.DBOlympicsGuess(client, olympicsDay);
				if (olympicsGuessDataDB.A1 != -1 || olympicsGuessDataDB.A2 != -1 || olympicsGuessDataDB.A3 != -1)
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				olympicsGuessDataDB.A1 = array[0];
				olympicsGuessDataDB.A2 = array[1];
				olympicsGuessDataDB.A3 = array[2];
				if (!this.DBOlympicsGuessUpdate(client, olympicsGuessDataDB))
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				this.CheckTip(client);
				client.sendCmd<int>(nID, 1, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessOlympicsGuessListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				if (!this._olympicsIsOpen)
				{
					client.sendCmd<OlympicsGuessDataResult>(nID, new OlympicsGuessDataResult(), false);
					return true;
				}
				int num = int.Parse(cmdParams[0]);
				int num2 = this.GetOlympicsDay();
				if (num == 2)
				{
					num2--;
				}
				if (num2 <= 0)
				{
					client.sendCmd<OlympicsGuessDataResult>(nID, null, false);
					return true;
				}
				OlympicsGuessDataDB answerData = this.DBOlympicsGuess(client, num2);
				List<OlympicsGuessData> olympicsGuessList = this.GetOlympicsGuessList(answerData);
				client.sendCmd<OlympicsGuessDataResult>(nID, new OlympicsGuessDataResult
				{
					Type = num,
					List = olympicsGuessList,
					DayID = this.TransformOffsetDayToFromBegin(num2)
				}, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessOlympicsRankCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				if (!this._olympicsIsOpen)
				{
					client.sendCmd<List<KFRankData>>(nID, new List<KFRankData>(), false);
					return true;
				}
				List<KFRankData> list = null;
				KFRankData kfrankData = null;
				List<KFRankData> list2 = AllyClient.getInstance().HRankTopList(1);
				if (list2 != null && list2.Count > 0)
				{
					kfrankData = list2.Find((KFRankData _x) => _x != null && _x.RoleID == client.ClientData.RoleID);
				}
				if (kfrankData == null)
				{
					kfrankData = AllyClient.getInstance().HRankData(1, client.ClientData.RoleID);
					if (kfrankData != null)
					{
						list = new List<KFRankData>(list2);
						list.Add(kfrankData);
					}
				}
				if (null != list)
				{
					client.sendCmd<List<KFRankData>>(nID, list, false);
					return true;
				}
				client.sendCmd<List<KFRankData>>(nID, list2, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessOlympicsShopListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				if (!this._olympicsIsOpen)
				{
					client.sendCmd<List<OlympicsShopData>>(nID, new List<OlympicsShopData>(), false);
					return true;
				}
				int olympicsDay = this.GetOlympicsDay();
				Dictionary<int, int> myCountDic = this.OlympicsShopCountGet(client, olympicsDay);
				List<OlympicsShopData> olympicsShopList = this.GetOlympicsShopList(olympicsDay, myCountDic);
				client.sendCmd<List<OlympicsShopData>>(nID, olympicsShopList, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessOlympicsShopBuyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			lock (this._mutex)
			{
				string format = "{0}:{1}:{2}:{3}";
				try
				{
					if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 2))
					{
						return false;
					}
					int num = int.Parse(cmdParams[0]);
					int num2 = int.Parse(cmdParams[1]);
					if (!this._olympicsIsOpen)
					{
						client.sendCmd(nID, string.Format(format, new object[]
						{
							0,
							num,
							0,
							0
						}), false);
						return true;
					}
					OlympicsShopData olympicsShopData = this.GetOlympicsShopData(num);
					if (olympicsShopData == null)
					{
						client.sendCmd(nID, string.Format(format, new object[]
						{
							0,
							num,
							0,
							0
						}), false);
						return true;
					}
					int olympicsDay = this.GetOlympicsDay();
					if (olympicsShopData.DayID != olympicsDay)
					{
						client.sendCmd(nID, string.Format(format, new object[]
						{
							0,
							num,
							0,
							0
						}), false);
						return true;
					}
					int num3 = 0;
					Dictionary<int, int> dictionary = this.OlympicsShopCountGet(client, olympicsDay);
					dictionary.TryGetValue(num, out num3);
					if (num3 + num2 > olympicsShopData.NumSingle)
					{
						client.sendCmd(nID, string.Format(format, new object[]
						{
							-2,
							num,
							0,
							0
						}), false);
						return true;
					}
					int num4 = 0;
					this._shopCountDic.TryGetValue(num, out num4);
					if (num4 + num2 > olympicsShopData.NumFull)
					{
						client.sendCmd(nID, string.Format(format, new object[]
						{
							-3,
							num,
							0,
							0
						}), false);
						return true;
					}
					string text = this.OlympicsGradeGet(client);
					int[] array = StringUtil.StringToIntArr(text, ':');
					int num5 = olympicsShopData.Price * num2;
					if (array[1] < num5)
					{
						client.sendCmd(nID, string.Format(format, new object[]
						{
							-4,
							num,
							0,
							0
						}), false);
						return true;
					}
					if (!Global.CanAddGoodsNum(client, num2))
					{
						client.sendCmd(nID, string.Format(format, new object[]
						{
							-5,
							num,
							0,
							0
						}), false);
						return true;
					}
					if (!this._shopCountDic.ContainsKey(num))
					{
						this._shopCountDic.Add(num, num2);
					}
					else
					{
						Dictionary<int, int> dictionary2;
						int key;
						(dictionary2 = this._shopCountDic)[key = num] = dictionary2[key] + num2;
					}
					if (!this.DBOlympicsShopUpdate(olympicsDay, num, this._shopCountDic[num]))
					{
						client.sendCmd(nID, string.Format(format, new object[]
						{
							0,
							num,
							0,
							0
						}), false);
						return true;
					}
					if (!dictionary.ContainsKey(num))
					{
						dictionary.Add(num, num2);
					}
					else
					{
						Dictionary<int, int> dictionary2;
						int key;
						(dictionary2 = dictionary)[key = num] = dictionary2[key] + num2;
					}
					this.OlympicsShopCountSet(client, olympicsDay, dictionary);
					this.OlympicsGradeSet(client, array[0], array[1] - num5);
					Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, olympicsShopData.Goods.GoodsID, olympicsShopData.Goods.GCount, olympicsShopData.Goods.Quality, "", olympicsShopData.Goods.Forge_level, olympicsShopData.Goods.Binding, 0, "", true, num2, "奥运积分商店", "1900-01-01 12:00:00", olympicsShopData.Goods.AddPropIndex, olympicsShopData.Goods.BornIndex, olympicsShopData.Goods.Lucky, olympicsShopData.Goods.Strong, olympicsShopData.Goods.ExcellenceInfo, olympicsShopData.Goods.AppendPropLev, 0, null, null, 0, true);
					client.sendCmd(nID, string.Format(format, new object[]
					{
						1,
						num,
						dictionary[num],
						this._shopCountDic[num]
					}), false);
					return true;
				}
				catch (Exception e)
				{
					DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
				result = false;
			}
			return result;
		}

		private bool ProcessOlympicsAwardStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string format = "{0}:{1}";
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				int num = 1;
				int num2 = Convert.ToInt32(cmdParams[0]);
				if (!this._olympicsIsOpen)
				{
					client.sendCmd(nID, string.Format(format, num2, num), false);
					return true;
				}
				switch (num2)
				{
				case 1:
				{
					int olympicsDay = this.GetOlympicsDay();
					List<OlympicsGuessDataDB> list = this.DBOlympicsGuessList(client);
					if (list != null && list.Count > 0)
					{
						foreach (OlympicsGuessDataDB olympicsGuessDataDB in list)
						{
							if (olympicsGuessDataDB.DayID < olympicsDay && (olympicsGuessDataDB.Award1 == 0 || olympicsGuessDataDB.Award2 == 0 || olympicsGuessDataDB.Award3 == 0))
							{
								num = 0;
								break;
							}
						}
					}
					break;
				}
				case 2:
				{
					int[] array = StringUtil.StringToIntArr(this.OlympicsGradeGet(client), ':');
					int num3 = array[0];
					if (num3 <= 0)
					{
						num = -4;
					}
					else if (!this.IsRankAwardTime())
					{
						num = -10;
					}
					else
					{
						num = Global.GetRoleParamsInt32FromDB(client, "20015");
					}
					break;
				}
				}
				client.sendCmd(nID, string.Format(format, num2, num), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessOlympicsAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				string text = "{0}:{1}:{2}:{3}";
				int num = Convert.ToInt32(cmdParams[0]);
				EOperateType eoperateType = EOperateType.Fail;
				int num2 = 0;
				int num3 = 0;
				if (!this._olympicsIsOpen)
				{
					client.sendCmd(nID, string.Format(text, new object[]
					{
						num,
						eoperateType,
						num2,
						num3
					}), false);
					return true;
				}
				switch (num)
				{
				case 1:
					eoperateType = this.OlympicAwardGuess(client, out num2);
					break;
				case 2:
					eoperateType = this.OlympicAwardRank(client, out num2, out num3);
					break;
				}
				text = string.Format(text, new object[]
				{
					num,
					(int)eoperateType,
					num2,
					num3
				});
				client.sendCmd(nID, text, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private List<OlympicsGuessData> GetOlympicsGuessList(OlympicsGuessDataDB answerData)
		{
			IOrderedEnumerable<OlympicsGuessData> orderedEnumerable = from info in this._guessDic.Values
			where info.DayID == answerData.DayID
			orderby info.ID
			select info;
			List<OlympicsGuessData> result;
			if (!orderedEnumerable.Any<OlympicsGuessData>())
			{
				result = null;
			}
			else
			{
				int num = 0;
				int[] array = new int[]
				{
					answerData.A1,
					answerData.A2,
					answerData.A3
				};
				List<OlympicsGuessData> list = new List<OlympicsGuessData>();
				foreach (OlympicsGuessData data in orderedEnumerable)
				{
					OlympicsGuessData olympicsGuessData = new OlympicsGuessData();
					olympicsGuessData.Clone(data);
					olympicsGuessData.DayID = this.TransformOffsetDayToFromBegin(olympicsGuessData.DayID);
					olympicsGuessData.Select = array[num++];
					list.Add(olympicsGuessData);
				}
				result = list;
			}
			return result;
		}

		private List<OlympicsShopData> GetOlympicsShopList(int day, Dictionary<int, int> myCountDic)
		{
			IOrderedEnumerable<OlympicsShopData> orderedEnumerable = from info in this._shopDic.Values
			where info.DayID == day
			orderby info.ID
			select info;
			List<OlympicsShopData> result;
			if (!orderedEnumerable.Any<OlympicsShopData>())
			{
				result = null;
			}
			else
			{
				List<OlympicsShopData> list = new List<OlympicsShopData>();
				foreach (OlympicsShopData olympicsShopData in orderedEnumerable)
				{
					OlympicsShopData olympicsShopData2 = new OlympicsShopData();
					olympicsShopData2.Clone(olympicsShopData);
					olympicsShopData2.DayID = this.TransformOffsetDayToFromBegin(olympicsShopData2.DayID);
					this._shopCountDic.TryGetValue(olympicsShopData.ID, out olympicsShopData2.NumFullBuy);
					myCountDic.TryGetValue(olympicsShopData.ID, out olympicsShopData2.NumSingleBuy);
					list.Add(olympicsShopData2);
				}
				result = list;
			}
			return result;
		}

		private EOperateType OlympicAwardGuess(GameClient client, out int awardID)
		{
			awardID = 0;
			List<OlympicsGuessDataDB> list = new List<OlympicsGuessDataDB>();
			int olympicsDay = this.GetOlympicsDay();
			List<OlympicsGuessDataDB> list2 = this.DBOlympicsGuessList(client);
			foreach (OlympicsGuessDataDB olympicsGuessDataDB in list2)
			{
				if (olympicsGuessDataDB.DayID < olympicsDay && (olympicsGuessDataDB.Award1 == 0 || olympicsGuessDataDB.Award2 == 0 || olympicsGuessDataDB.Award3 == 0))
				{
					List<OlympicsGuessData> olympicsGuessList = this.GetOlympicsGuessList(olympicsGuessDataDB);
					if (olympicsGuessList != null && olympicsGuessList.Count > 0)
					{
						if (olympicsGuessDataDB.Award1 == 0 && olympicsGuessList[0].Select > 0 && olympicsGuessList[0].Answer > 0)
						{
							olympicsGuessDataDB.Award1 = 1;
							if (olympicsGuessList[0].Select == olympicsGuessList[0].Answer)
							{
								awardID += olympicsGuessList[0].Grade;
							}
						}
						if (olympicsGuessDataDB.Award2 == 0 && olympicsGuessList[1].Select > 0 && olympicsGuessList[1].Answer > 0)
						{
							olympicsGuessDataDB.Award2 = 1;
							if (olympicsGuessList[1].Select == olympicsGuessList[1].Answer)
							{
								awardID += olympicsGuessList[1].Grade;
							}
						}
						if (olympicsGuessDataDB.Award3 == 0 && olympicsGuessList[2].Select > 0 && olympicsGuessList[2].Answer > 0)
						{
							olympicsGuessDataDB.Award3 = 1;
							if (olympicsGuessList[2].Select == olympicsGuessList[2].Answer)
							{
								awardID += olympicsGuessList[2].Grade;
							}
						}
						list.Add(olympicsGuessDataDB);
					}
				}
			}
			foreach (OlympicsGuessDataDB data in list)
			{
				this.DBOlympicsGuessUpdate(client, data);
			}
			if (awardID > 0)
			{
				this.OlympicsGradeAdd(client, awardID);
			}
			return EOperateType.Succ;
		}

		private EOperateType OlympicAwardRank(GameClient client, out int awardID, out int rankOut)
		{
			awardID = 0;
			rankOut = 0;
			int rank = 50001;
			EOperateType result;
			if (!this.IsRankAwardTime())
			{
				result = EOperateType.EAwardTime;
			}
			else
			{
				int[] array = StringUtil.StringToIntArr(this.OlympicsGradeGet(client), ':');
				int num = array[0];
				if (num <= 0)
				{
					result = EOperateType.ENoGrade;
				}
				else
				{
					KFRankData kfrankData = AllyClient.getInstance().HRankData(1, client.ClientData.RoleID);
					if (kfrankData != null)
					{
						rank = kfrankData.Rank;
					}
					OlympicsRankAwardInfo olympicsRankAwardInfo = this._rankAwardList.Find((OlympicsRankAwardInfo _x) => rank >= _x.RankBegin && (_x.RankEnd == -1 || rank <= _x.RankEnd));
					if (olympicsRankAwardInfo == null)
					{
						result = EOperateType.Fail;
					}
					else
					{
						Global.SaveRoleParamsInt32ValueToDB(client, "20015", 1, true);
						List<GoodsData> defaultGoodsList = olympicsRankAwardInfo.DefaultGoodsList;
						if (Global.CanAddGoodsDataList(client, olympicsRankAwardInfo.DefaultGoodsList))
						{
							for (int i = 0; i < defaultGoodsList.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, defaultGoodsList[i].GoodsID, defaultGoodsList[i].GCount, defaultGoodsList[i].Quality, "", defaultGoodsList[i].Forge_level, defaultGoodsList[i].Binding, 0, "", true, 1, "奥运奖励", "1900-01-01 12:00:00", defaultGoodsList[i].AddPropIndex, defaultGoodsList[i].BornIndex, defaultGoodsList[i].Lucky, defaultGoodsList[i].Strong, defaultGoodsList[i].ExcellenceInfo, defaultGoodsList[i].AppendPropLev, 0, null, null, 0, true);
							}
						}
						else
						{
							int num2 = Global.UseMailGivePlayerAward2(client, defaultGoodsList, GLang.GetLang(505, new object[0]), GLang.GetLang(505, new object[0]), 0, 0, 0);
						}
						awardID = olympicsRankAwardInfo.ID;
						rankOut = ((rank >= 50001) ? -1 : rank);
						result = EOperateType.Succ;
					}
				}
			}
			return result;
		}

		private void JudgeClearOlympicsActivityData(GameClient client)
		{
			if (this._olympicsIsOpen)
			{
				string text = string.Format("{0}", this.timeBegin.ToString("yyyy-MM-dd HH:mm:ss")).Replace(':', '$');
				string text2 = Global.GetRoleParamByName(client, "20014");
				if (string.IsNullOrEmpty(text2))
				{
					text2 = text;
					Global.SaveRoleParamsStringToDB(client, "20014", text2, true);
				}
				else if (string.Compare(text, text2) != 0)
				{
					text2 = text;
					this.OlympicsGradeSet(client, 0, 0);
					Global.SaveRoleParamsInt32ValueToDB(client, "20015", 0, true);
					Global.SaveRoleParamsStringToDB(client, "20014", text2, true);
				}
			}
		}

		private string OlympicsGradeGet(GameClient client)
		{
			string text = Global.GetRoleParamByName(client, "20010");
			string result;
			if (string.IsNullOrEmpty(text))
			{
				result = "0:0";
			}
			else
			{
				text = text.Replace(",", ":");
				result = text;
			}
			return result;
		}

		private void OlympicsGradeSet(GameClient client, int gradeAll, int gradeLeft)
		{
			string valueString = string.Format("{0},{1}", gradeAll, gradeLeft);
			Global.SaveRoleParamsStringToDB(client, "20010", valueString, true);
			client.sendCmd(1050, string.Format("{0}:{1}", gradeAll, gradeLeft), false);
		}

		public void OlympicsGradeAdd(GameClient client, int addGrade)
		{
			if (this._olympicsIsOpen && !this.IsRankAwardTime())
			{
				string text = this.OlympicsGradeGet(client);
				int[] array = StringUtil.StringToIntArr(text, ':');
				int gradeAll = array[0] + addGrade;
				int gradeLeft = array[1] + addGrade;
				this.OlympicsGradeSet(client, gradeAll, gradeLeft);
				RoleData4Selector roleData4Selector = Global.RoleDataEx2RoleData4Selector(client.ClientData.GetRoleDataEx());
				byte[] roleData = DataHelper.ObjectToBytes<RoleData4Selector>(roleData4Selector);
				AllyClient.getInstance().HRankUpdate(1, array[0] + addGrade, client.ClientData.RoleID, client.ClientData.ZoneID, client.ClientData.RoleName, roleData);
			}
		}

		private int[] OlympicsGameCountGet(GameClient client, EGameType gameType)
		{
			string text = "";
			switch (gameType)
			{
			case EGameType.Shoot:
				text = Global.GetRoleParamByName(client, "20011");
				break;
			case EGameType.Football:
				text = Global.GetRoleParamByName(client, "20012");
				break;
			}
			int olympicsDay = this.GetOlympicsDay();
			int[] result;
			if (string.IsNullOrEmpty(text))
			{
				int[] array = new int[5];
				array[0] = olympicsDay;
				result = array;
			}
			else
			{
				int[] array2 = StringUtil.StringToIntArr(text, ',');
				result = array2;
			}
			return result;
		}

		private void OlympicsGameCountSet(GameClient client, EGameType gameType, string value)
		{
			switch (gameType)
			{
			case EGameType.Shoot:
				Global.SaveRoleParamsStringToDB(client, "20011", value, true);
				break;
			case EGameType.Football:
				Global.SaveRoleParamsStringToDB(client, "20012", value, true);
				break;
			}
		}

		private Dictionary<int, int> OlympicsShopCountGet(GameClient client, int dayID)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			string roleParamByName = Global.GetRoleParamByName(client, "20013");
			Dictionary<int, int> result;
			if (string.IsNullOrEmpty(roleParamByName))
			{
				result = dictionary;
			}
			else
			{
				string[] array = roleParamByName.Split(new char[]
				{
					'*'
				});
				int num = Global.SafeConvertToInt32(array[0]);
				if (num != dayID)
				{
					this.OlympicsShopCountSet(client, dayID, null);
					result = dictionary;
				}
				else
				{
					if (array.Length > 1 && !string.IsNullOrEmpty(array[1]))
					{
						int[] array2 = StringUtil.StringToIntArr(array[1], ',');
						for (int i = 0; i < array2.Length; i++)
						{
							dictionary.Add(array2[i++], array2[i]);
						}
					}
					result = dictionary;
				}
			}
			return result;
		}

		private void OlympicsShopCountSet(GameClient client, int dayID, Dictionary<int, int> dic)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (dic != null)
			{
				foreach (KeyValuePair<int, int> keyValuePair in dic)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(string.Format("{0},{1}", keyValuePair.Key, keyValuePair.Value));
				}
			}
			Global.SaveRoleParamsStringToDB(client, "20013", string.Format("{0}*{1}", dayID, stringBuilder.ToString()), true);
		}

		private Dictionary<int, int> DBOlympicsShopList(int dayID)
		{
			return Global.sendToDB<Dictionary<int, int>, int>(13124, dayID, GameManager.ServerId);
		}

		private bool DBOlympicsShopUpdate(int dayID, int shopID, int count)
		{
			string cmd = string.Format("{0}:{1}:{2}", dayID, shopID, count);
			return Global.sendToDB<bool, string>(13125, cmd, GameManager.ServerId);
		}

		private OlympicsGuessDataDB DBOlympicsGuess(GameClient client, int dayID)
		{
			string cmd = string.Format("{0}:{1}", client.ClientData.RoleID, dayID);
			return Global.sendToDB<OlympicsGuessDataDB, string>(13126, cmd, GameManager.ServerId);
		}

		private List<OlympicsGuessDataDB> DBOlympicsGuessList(GameClient client)
		{
			return Global.sendToDB<List<OlympicsGuessDataDB>, int>(13127, client.ClientData.RoleID, GameManager.ServerId);
		}

		private bool DBOlympicsGuessUpdate(GameClient client, OlympicsGuessDataDB data)
		{
			return Global.sendToDB<bool, OlympicsGuessDataDB>(13128, data, GameManager.ServerId);
		}

		public void CheckOlympicsOpenState(long ticks, bool changeDay = false)
		{
			if (changeDay || ticks - this._lastTicks >= 10000L)
			{
				this._lastTicks = ticks;
				DateTime t = TimeUtil.NowDateTime();
				if (!this._olympicsIsOpen && t >= this.timeBegin && t < this.timeAwardEnd)
				{
					this._olympicsIsOpen = true;
					this._olympicsOpenTime = this.timeBegin;
					int olympicsDay = this.GetOlympicsDay();
					this._shopCountDic = this.DBOlympicsShopList(olympicsDay);
					ActivityData activityData = new ActivityData();
					activityData.ActivityType = 4;
					activityData.ActivityIsOpen = true;
					activityData.TimeBegin = this.timeBegin;
					activityData.TimeEnd = this.timeEnd;
					activityData.TimeAwardBegin = this.timeAwardBegin;
					activityData.TimeAwardEnd = this.timeAwardEnd;
					SingletonTemplate<ActivityManager>.Instance().ActivityAdd(activityData);
					AllyClient.getInstance().HRankTopList(1);
				}
				if (this._olympicsIsOpen && (t > this.timeAwardEnd || t < this.timeBegin))
				{
					this._olympicsIsOpen = false;
					this._olympicsOpenTime = DateTime.MinValue;
					this._shopCountDic.Clear();
					SingletonTemplate<ActivityManager>.Instance().ActivityDel(4);
				}
			}
		}

		private int TransformOffsetDayToFromBegin(int OffsetDay)
		{
			return OffsetDay - Global.GetOffsetDay(this.timeBegin);
		}

		private int GetOlympicsDay()
		{
			int result = 0;
			if (this._olympicsIsOpen && this._olympicsOpenTime > DateTime.MinValue)
			{
				result = Global.GetOffsetDay(TimeUtil.NowDateTime()) + 1;
			}
			return result;
		}

		private int GetOlympicsDayFromBegin()
		{
			int result = 0;
			if (this._olympicsIsOpen && this._olympicsOpenTime > DateTime.MinValue)
			{
				result = Global.GetOffsetDay(TimeUtil.NowDateTime()) - Global.GetOffsetDay(this._olympicsOpenTime) + 1;
			}
			return result;
		}

		public void InitOlympics()
		{
			lock (this._mutex)
			{
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("AoYunTime");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array = paramValueByName.Split(new char[]
					{
						','
					});
					if (array != null && array.Length == 2)
					{
						DateTime.TryParse(array[0], out this.timeBegin);
						DateTime.TryParse(array[1], out this.timeEnd);
					}
				}
				paramValueByName = GameManager.systemParamsList.GetParamValueByName("AoYunAwardTime");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array = paramValueByName.Split(new char[]
					{
						','
					});
					DateTime.TryParse(array[0], out this.timeAwardBegin);
					DateTime.TryParse(array[1], out this.timeAwardEnd);
				}
				ActivityData activityData = new ActivityData();
				activityData.ActivityType = 4;
				activityData.ActivityIsOpen = this._olympicsIsOpen;
				activityData.TimeBegin = this.timeBegin;
				activityData.TimeEnd = this.timeEnd;
				activityData.TimeAwardBegin = this.timeAwardBegin;
				activityData.TimeAwardEnd = this.timeAwardEnd;
				SingletonTemplate<ActivityManager>.Instance().UpdateActivityData(activityData);
				this.InitOlympicsGame();
				this.InitOlympicsGuess();
				this.InitOlympicsRankAward();
				this.InitOlympicsShop();
			}
		}

		private bool IsRankAwardTime()
		{
			return TimeUtil.NowDateTime() >= this.timeAwardBegin && TimeUtil.NowDateTime() < this.timeAwardEnd;
		}

		private double GetFootballRate()
		{
			return GameManager.systemParamsList.GetParamValueDoubleByName("AoYunGoalOdds", 0.0);
		}

		private void InitOlympicsGame()
		{
			lock (this._mutex)
			{
				string text = Global.GameResPath("Config/AoYunMatch.xml");
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					try
					{
						this._gameDic.Clear();
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xelement2 in enumerable)
						{
							if (xelement2 != null)
							{
								OlympicsGameInfo olympicsGameInfo = new OlympicsGameInfo();
								olympicsGameInfo.GameID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
								olympicsGameInfo.CountFree = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "FreeNum", "0"));
								string defAttributeStr = Global.GetDefAttributeStr(xelement2, "NeedZhuanShi", "0");
								string[] array = defAttributeStr.Split(new char[]
								{
									','
								});
								List<int> list = new List<int>();
								foreach (string s in array)
								{
									list.Add(int.Parse(s));
								}
								olympicsGameInfo.DiamondList = list;
								olympicsGameInfo.CountDiamond = list.Count;
								olympicsGameInfo.CountGame = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "GameNum", "0"));
								olympicsGameInfo.CountWin = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "WinNum", "0"));
								olympicsGameInfo.GradeWin = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "WinJiFen", "0"));
								olympicsGameInfo.GradeLost = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "LoseJiFen", "0"));
								this._gameDic.Add(olympicsGameInfo.GameID, olympicsGameInfo);
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", text), null, true);
					}
				}
			}
		}

		private void InitOlympicsGuess()
		{
			lock (this._mutex)
			{
				string text = Global.GameResPath("Config/AoYunQuestion.xml");
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					try
					{
						this._guessDic.Clear();
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xelement2 in enumerable)
						{
							if (xelement2 != null)
							{
								OlympicsGuessData olympicsGuessData = new OlympicsGuessData();
								olympicsGuessData.ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
								olympicsGuessData.DayID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Day", "0"));
								olympicsGuessData.DayID += Global.GetOffsetDay(this.timeBegin);
								olympicsGuessData.Content = Global.GetDefAttributeStr(xelement2, "Question", "");
								olympicsGuessData.A = Global.GetDefAttributeStr(xelement2, "A", "0");
								olympicsGuessData.B = Global.GetDefAttributeStr(xelement2, "B", "0");
								olympicsGuessData.C = Global.GetDefAttributeStr(xelement2, "C", "0");
								olympicsGuessData.D = Global.GetDefAttributeStr(xelement2, "D", "0");
								olympicsGuessData.Answer = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Answer", "0"));
								olympicsGuessData.Grade = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "JiFen", "0"));
								this._guessDic.Add(olympicsGuessData.ID, olympicsGuessData);
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", text), null, true);
					}
				}
			}
		}

		private void InitOlympicsRankAward()
		{
			lock (this._mutex)
			{
				string text = Global.GameResPath("Config/AoYunAward.xml");
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					try
					{
						this._rankAwardList.Clear();
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xelement2 in enumerable)
						{
							if (xelement2 != null)
							{
								OlympicsRankAwardInfo olympicsRankAwardInfo = new OlympicsRankAwardInfo();
								olympicsRankAwardInfo.ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
								olympicsRankAwardInfo.Content = Global.GetDefAttributeStr(xelement2, "Name", "");
								olympicsRankAwardInfo.RankBegin = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "BeginNum", "0"));
								olympicsRankAwardInfo.RankEnd = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "EngNum", "0"));
								if (olympicsRankAwardInfo.RankEnd == -1)
								{
									olympicsRankAwardInfo.RankEnd = int.MaxValue;
								}
								string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsOne");
								if (!string.IsNullOrEmpty(safeAttributeStr))
								{
									string[] array = safeAttributeStr.Split(new char[]
									{
										'|'
									});
									if (array.Length > 0)
									{
										olympicsRankAwardInfo.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(array, text);
									}
								}
								this._rankAwardList.Add(olympicsRankAwardInfo);
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", text), null, true);
					}
				}
			}
		}

		private void InitOlympicsShop()
		{
			lock (this._mutex)
			{
				string text = Global.GameResPath("Config/AoYunQiangGou.xml");
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					try
					{
						this._shopDic.Clear();
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xelement2 in enumerable)
						{
							if (xelement2 != null)
							{
								OlympicsShopData olympicsShopData = new OlympicsShopData();
								olympicsShopData.ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
								olympicsShopData.DayID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Day", "0"));
								olympicsShopData.DayID += Global.GetOffsetDay(this.timeBegin);
								string defAttributeStr = Global.GetDefAttributeStr(xelement2, "GoodsOne", "");
								if (!string.IsNullOrEmpty(defAttributeStr))
								{
									olympicsShopData.Goods = GoodsHelper.ParseGoodsData(defAttributeStr, text);
								}
								olympicsShopData.Price = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "JiFen", "0"));
								olympicsShopData.NumSingle = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "SinglePurchase", "0"));
								olympicsShopData.NumFull = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "FullPurchase", "0"));
								this._shopDic.Add(olympicsShopData.ID, olympicsShopData);
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", text), null, true);
					}
				}
			}
		}

		private OlympicsShopData GetOlympicsShopData(int id)
		{
			OlympicsShopData result;
			if (this._shopDic.ContainsKey(id))
			{
				result = this._shopDic[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void CheckTip(GameClient client)
		{
			if (!this._olympicsIsOpen || this.IsRankAwardTime())
			{
				client._IconStateMgr.AddFlushIconState(20001, false);
				client._IconStateMgr.AddFlushIconState(20002, false);
				client._IconStateMgr.AddFlushIconState(20000, false);
				client._IconStateMgr.SendIconStateToClient(client);
			}
			else
			{
				bool flag = false;
				bool flag2 = false;
				int olympicsDay = this.GetOlympicsDay();
				foreach (OlympicsGameInfo olympicsGameInfo in this._gameDic.Values)
				{
					int[] array = this.OlympicsGameCountGet(client, (EGameType)olympicsGameInfo.GameID);
					if (array[1] < olympicsGameInfo.CountFree || array[0] != olympicsDay)
					{
						flag = true;
					}
				}
				OlympicsGuessDataDB olympicsGuessDataDB = this.DBOlympicsGuess(client, olympicsDay);
				List<OlympicsGuessData> olympicsGuessList = this.GetOlympicsGuessList(olympicsGuessDataDB);
				if (olympicsGuessList != null && (olympicsGuessDataDB.A1 == -1 || olympicsGuessDataDB.A2 == -1 || olympicsGuessDataDB.A3 == -1))
				{
					flag2 = true;
				}
				bool flag3 = false;
				flag3 |= client._IconStateMgr.AddFlushIconState(20001, flag);
				flag3 |= client._IconStateMgr.AddFlushIconState(20002, flag2);
				flag3 |= client._IconStateMgr.AddFlushIconState(20000, flag || flag2);
				if (flag3)
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		private const int OLYMPICS_GUESS_COUNT = 3;

		private const int OLYMPICS_RANK_MAX = 50001;

		public const int _sceneType = 10006;

		public object _mutex = new object();

		private DateTime timeBegin = DateTime.MinValue;

		private DateTime timeEnd = DateTime.MinValue;

		private DateTime timeAwardBegin = DateTime.MinValue;

		private DateTime timeAwardEnd = DateTime.MinValue;

		private static OlympicsManager instance = new OlympicsManager();

		private long _lastTicks = 0L;

		private bool _olympicsIsOpen = false;

		private DateTime _olympicsOpenTime = DateTime.MinValue;

		private Dictionary<int, int> _shopCountDic = new Dictionary<int, int>();

		private Dictionary<int, OlympicsGameInfo> _gameDic = new Dictionary<int, OlympicsGameInfo>();

		private Dictionary<int, OlympicsGuessData> _guessDic = new Dictionary<int, OlympicsGuessData>();

		private List<OlympicsRankAwardInfo> _rankAwardList = new List<OlympicsRankAwardInfo>();

		private Dictionary<int, OlympicsShopData> _shopDic = new Dictionary<int, OlympicsShopData>();
	}
}
