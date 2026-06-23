using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class BattleManager
	{
		public int GetBattlingState()
		{
			return (int)this.BattlingState;
		}

		public int GetBattlingLeftSecs()
		{
			long num = TimeUtil.NOW();
			int num2 = 0;
			if (this.BattlingState == BattleStates.PublishMsg)
			{
				num2 = this.WaitingEnterSecs;
			}
			else if (this.BattlingState == BattleStates.WaitingFight)
			{
				num2 = this.PrepareSecs;
			}
			else if (this.BattlingState == BattleStates.StartFight)
			{
				num2 = this.FightingSecs;
			}
			else if (this.BattlingState == BattleStates.EndFight)
			{
				num2 = this.ClearRolesSecs;
			}
			else if (this.BattlingState == BattleStates.ClearBattle)
			{
				num2 = this.ClearRolesSecs;
			}
			return (int)(((long)(num2 * 1000) - (num - this.StateStartTicks)) / 1000L);
		}

		public void LoadParams()
		{
			SystemXmlItem systemXmlItem = null;
			if (GameManager.SystemBattle.SystemXmlItemDict.TryGetValue(1, out systemXmlItem))
			{
				List<string> list = new List<string>();
				string stringValue = systemXmlItem.GetStringValue("TimePoints");
				if (stringValue != null && stringValue != "")
				{
					string[] array = stringValue.Split(new char[]
					{
						','
					});
					for (int i = 0; i < array.Length; i++)
					{
						list.Add(array[i].Trim());
					}
				}
				this.TimePointsList = list;
				this.MapCode = systemXmlItem.GetIntValue("MapCode", -1);
				this.MinLevel = systemXmlItem.GetIntValue("MinLevel", -1);
				this.MinRequestNum = systemXmlItem.GetIntValue("MinRequestNum", -1);
				this.MaxEnterNum = systemXmlItem.GetIntValue("MaxEnterNum", -1);
				this.FallGiftNum = systemXmlItem.GetIntValue("FallGiftNum", -1);
				this.FallID = systemXmlItem.GetIntValue("FallID", -1);
				this.DisableGoodsIDs = systemXmlItem.GetStringValue("DisableGoodsIDs");
				this.AddExpSecs = systemXmlItem.GetIntValue("AddExpSecs", -1);
				this.NotifyBattleKilledNumSecs = Global.GMax(5, Global.GMin(100, systemXmlItem.GetIntValue("NotifyBattleKilledNumSecs", -1)));
				this.WaitingEnterSecs = systemXmlItem.GetIntValue("WaitingEnterSecs", -1);
				this.PrepareSecs = systemXmlItem.GetIntValue("PrepareSecs", -1);
				this.FightingSecs = systemXmlItem.GetIntValue("FightingSecs", -1);
				this.ClearRolesSecs = systemXmlItem.GetIntValue("ClearRolesSecs", -1);
				this.m_NeedMinChangeLev = systemXmlItem.GetIntValue("MinZhuanSheng", -1);
				this.BattleLineID = Global.GMax(1, systemXmlItem.GetIntValue("LineID", -1));
				this.ReloadGiveAwardsGoodsDataList(systemXmlItem);
				Global.QueryDayActivityTotalPointInfoToDB(SpecialActivityTypes.CampBattle);
				BattleManager.PushMsgDayID = Global.SafeConvertToInt32(GameManager.GameConfigMgr.GetGameConifgItem("BattlePushMsgDayID"));
			}
		}

		public void ReloadGiveAwardsGoodsDataList(SystemXmlItem systemBattle = null)
		{
			if (null == systemBattle)
			{
				if (!GameManager.SystemBattle.SystemXmlItemDict.TryGetValue(1, out systemBattle))
				{
					return;
				}
			}
			List<GoodsData> list = new List<GoodsData>();
			string text = systemBattle.GetStringValue("GiveGoodsIDs").Trim();
			string[] array = text.Split(new char[]
			{
				','
			});
			if (array != null && array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrEmpty(array[i].Trim()))
					{
						int num = Convert.ToInt32(array[i].Trim());
						SystemXmlItem systemXmlItem = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num, out systemXmlItem))
						{
							LogManager.WriteLog(2, string.Format("角斗场配置文件中，配置的固定物品奖励中的物品不存在, GoodsID={0}", num), null, true);
						}
						else
						{
							GoodsData item = new GoodsData
							{
								Id = -1,
								GoodsID = num,
								Using = 0,
								Forge_level = 0,
								Starttime = "1900-01-01 12:00:00",
								Endtime = "1900-01-01 12:00:00",
								Site = 0,
								Quality = 0,
								Props = "",
								GCount = 1,
								Binding = 0,
								Jewellist = "",
								BagIndex = 0,
								AddPropIndex = 0,
								BornIndex = 0,
								Lucky = 0,
								Strong = 0,
								ExcellenceInfo = 0,
								AppendPropLev = 0,
								ChangeLifeLevForEquip = 0
							};
							list.Add(item);
						}
					}
				}
			}
			this.GiveAwardsGoodsDataList = list;
		}

		public void Init()
		{
			this.LoadParams();
			this.AllowAttack = false;
			this.StartRoleNum = 0;
			this.TotalClientCount = 0;
			this.AllKilledRoleNum = 0;
		}

		public void Process()
		{
			if (this.BattlingState > BattleStates.NoBattle)
			{
				this.ProcessBattling();
			}
			else
			{
				this.ProcessNoBattle();
			}
		}

		private void ProcessBattling()
		{
			if (this.BattlingState == BattleStates.PublishMsg)
			{
				int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
				if (BattleManager.PushMsgDayID != dayOfYear)
				{
					Global.UpdateDBGameConfigg("BattlePushMsgDayID", dayOfYear.ToString());
					BattleManager.PushMsgDayID = dayOfYear;
				}
				long num = TimeUtil.NOW();
				if (num >= this.StateStartTicks + (long)(this.WaitingEnterSecs * 1000))
				{
					GameManager.ClientMgr.NotifyBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode, 2, 2, this.PrepareSecs);
					this.BattlingState = BattleStates.WaitingFight;
					this.StateStartTicks = TimeUtil.NOW();
				}
			}
			else if (this.BattlingState == BattleStates.WaitingFight)
			{
				long num = TimeUtil.NOW();
				if (num >= this.StateStartTicks + (long)(this.PrepareSecs * 1000))
				{
					this.AllowAttack = true;
					GameManager.ClientMgr.NotifyBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode, 2, 3, this.FightingSecs);
					this.StartRoleNum = GameManager.ClientMgr.GetMapClientsCount(this.MapCode);
					this.BattlingState = BattleStates.StartFight;
					this.StateStartTicks = TimeUtil.NOW();
					this.LastAddBattleExpTicks = this.StateStartTicks;
					this.LastNotifyBattleKilledNumTicks = this.StateStartTicks;
					this._SuiLastKillEmemyTime = TimeUtil.NOW() * 10000L;
					this._TangLastKillEmemyTime = this._SuiLastKillEmemyTime;
				}
			}
			else if (this.BattlingState == BattleStates.StartFight)
			{
				long num = TimeUtil.NOW();
				if (num >= this.StateStartTicks + (long)(this.FightingSecs * 1000))
				{
					this.AllowAttack = false;
					this.BattlingState = BattleStates.EndFight;
					this.StateStartTicks = TimeUtil.NOW();
				}
				else
				{
					this.ProcessTimeAddRoleExp();
					this.ProcessTimeNotifyBattleKilledNum();
				}
			}
			else if (this.BattlingState == BattleStates.EndFight)
			{
				this.BattlingState = BattleStates.ClearBattle;
				this.StateStartTicks = TimeUtil.NOW();
				GameManager.ClientMgr.NotifyBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode, 2, 5, this.ClearRolesSecs);
				this.ProcessBattleResultAwards2();
			}
			else if (this.BattlingState == BattleStates.ClearBattle)
			{
				long num = TimeUtil.NOW();
				if (num >= this.StateStartTicks + (long)(this.ClearRolesSecs * 1000))
				{
					GameManager.ClientMgr.NotifyBattleLeaveMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode);
					this.BattlingState = BattleStates.NoBattle;
					this.StateStartTicks = 0L;
					this.ClearAllRoleLeaveInfo();
					this.ClearAllRolePointInfo();
				}
			}
		}

		private void ProcessNoBattle()
		{
			if (this.JugeStartBattle())
			{
				this.StartRoleNum = 0;
				this.TotalClientCount = 0;
				this.AllKilledRoleNum = 0;
				this.SuiClientCount = 0;
				this.TangClientCount = 0;
				this.SuiKilledNum = 0;
				this.TangKilledNum = 0;
				this.BattlingState = BattleStates.PublishMsg;
				GameManager.ClientMgr.NotifyAllBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MinLevel, 1, 1, this.WaitingEnterSecs);
				this.StateStartTicks = TimeUtil.NOW();
			}
		}

		private bool JugeStartBattle()
		{
			string b = TimeUtil.NowDateTime().ToString("HH:mm");
			List<string> timePointsList = this.TimePointsList;
			bool result;
			if (null == timePointsList)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < timePointsList.Count; i++)
				{
					if (timePointsList[i] == b)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public int LeftEnterCount()
		{
			int num = 0;
			string text = TimeUtil.NowDateTime().ToString("HH:mm");
			List<string> timePointsList = this.TimePointsList;
			int result;
			if (null == timePointsList)
			{
				result = 0;
			}
			else
			{
				try
				{
					for (int i = 0; i < timePointsList.Count; i++)
					{
						DateTime t = DateTime.Parse(timePointsList[i]);
						t.AddMinutes((double)this.ClearRolesSecs);
						if (t >= TimeUtil.NowDateTime())
						{
							num++;
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				result = num;
			}
			return result;
		}

		public object ExternalMutex
		{
			get
			{
				return this.mutex;
			}
		}

		public int BattleMapCode
		{
			get
			{
				return this.MapCode;
			}
		}

		public int BattleServerLineID
		{
			get
			{
				return this.BattleLineID;
			}
		}

		public bool AllowEnterMap
		{
			get
			{
				return this.BattlingState >= BattleStates.PublishMsg && this.BattlingState < BattleStates.EndFight;
			}
		}

		public bool AllowAttack
		{
			get
			{
				bool allowAttack;
				lock (this.mutex)
				{
					allowAttack = this._AllowAttack;
				}
				return allowAttack;
			}
			set
			{
				lock (this.mutex)
				{
					this._AllowAttack = value;
				}
			}
		}

		public int AllowMinLevel
		{
			get
			{
				return this.MinLevel;
			}
		}

		public int TotalClientCount
		{
			get
			{
				int totalClientCount;
				lock (this.mutex)
				{
					totalClientCount = this._TotalClientCount;
				}
				return totalClientCount;
			}
			set
			{
				lock (this.mutex)
				{
					this._TotalClientCount = value;
				}
			}
		}

		public int NeedMinChangeLev
		{
			get
			{
				return this.m_NeedMinChangeLev;
			}
		}

		public static int BattleMaxPoint
		{
			get
			{
				return BattleManager.m_BattleMaxPoint;
			}
			set
			{
				BattleManager.m_BattleMaxPoint = value;
			}
		}

		public static string BattleMaxPointName
		{
			get
			{
				return BattleManager.m_BattleMaxPointName;
			}
			set
			{
				BattleManager.m_BattleMaxPointName = value;
			}
		}

		public static int BattleMaxPointNow
		{
			get
			{
				return BattleManager.m_BattleMaxPointNow;
			}
			set
			{
				BattleManager.m_BattleMaxPointNow = value;
			}
		}

		public static int PushMsgDayID
		{
			get
			{
				return BattleManager.m_nPushMsgDayID;
			}
			set
			{
				BattleManager.m_nPushMsgDayID = value;
			}
		}

		public static void SetTotalPointInfo(string sName, int nValue)
		{
			BattleManager.BattleMaxPointName = sName;
			BattleManager.BattleMaxPoint = nValue;
		}

		private void ClearAllRolePointInfo()
		{
			lock (this.RolePointMutex)
			{
				this.RolePointDict.Clear();
				for (int i = 0; i < this.TopPointList.Length; i++)
				{
					this.TopPointList[i] = null;
				}
			}
		}

		public void UpdateRolePointInfo(GameClient client)
		{
			int roleID = client.ClientData.RoleID;
			int battleKilledNum = client.ClientData.BattleKilledNum;
			List<RoleDamage> list = null;
			BattlePointInfo battlePointInfo = null;
			bool flag = false;
			lock (this.RolePointMutex)
			{
				if (this.RolePointDict.TryGetValue(roleID, out battlePointInfo))
				{
					flag = (battlePointInfo.m_DamagePoint == battleKilledNum);
					battlePointInfo.m_DamagePoint = battleKilledNum;
				}
				else
				{
					battlePointInfo = new BattlePointInfo();
					battlePointInfo.m_RoleID = roleID;
					battlePointInfo.m_RoleName = Global.FormatRoleName4(client);
					battlePointInfo.m_DamagePoint = battleKilledNum;
					this.RolePointDict[roleID] = battlePointInfo;
				}
				if (flag || battlePointInfo.CompareTo(this.TopPointList[4]) < 0)
				{
					if (battlePointInfo.Ranking < 0)
					{
						this.TopPointList[5] = battlePointInfo;
					}
					Array.Sort<BattlePointInfo>(this.TopPointList, new Comparison<BattlePointInfo>(battlePointInfo.Compare));
					if (null != this.TopPointList[5])
					{
						this.TopPointList[5].Ranking = -1;
					}
					flag = true;
				}
				if (battlePointInfo.Side != client.ClientData.BattleWhichSide)
				{
					battlePointInfo.Side = client.ClientData.BattleWhichSide;
					flag = true;
				}
				if (flag)
				{
					list = new List<RoleDamage>(5);
					int num = 0;
					while (this.TopPointList[num] != null && num < 5)
					{
						this.TopPointList[num].Ranking = num;
						list.Add(new RoleDamage(this.TopPointList[num].m_RoleID, (long)this.TopPointList[num].m_DamagePoint, this.TopPointList[num].m_RoleName, new int[]
						{
							this.TopPointList[num].Side
						}));
						num++;
					}
				}
			}
			if (flag)
			{
				List<GameClient> mapGameClients = GameManager.ClientMgr.GetMapGameClients(this.MapCode);
				foreach (GameClient gameClient in mapGameClients)
				{
					gameClient.sendCmd<List<RoleDamage>>(647, list, false);
				}
			}
		}

		public void SendScoreInfoListToClient(GameClient client)
		{
			int roleID = client.ClientData.RoleID;
			List<RoleDamage> list = new List<RoleDamage>(5);
			lock (this.RolePointMutex)
			{
				int num = 0;
				while (this.TopPointList[num] != null && num < 5)
				{
					list.Add(new RoleDamage(this.TopPointList[num].m_RoleID, (long)this.TopPointList[num].m_DamagePoint, this.TopPointList[num].m_RoleName, new int[]
					{
						this.TopPointList[num].Side
					}));
					num++;
				}
			}
			if (null != list)
			{
				client.sendCmd<List<RoleDamage>>(647, list, false);
			}
		}

		public bool ClientEnter()
		{
			bool result = false;
			lock (this.mutex)
			{
				if (this._TotalClientCount < this.MaxEnterNum)
				{
					this._TotalClientCount++;
					result = true;
				}
			}
			return result;
		}

		public void ClientLeave()
		{
			lock (this.mutex)
			{
				this._TotalClientCount--;
			}
		}

		public string BattleDisableGoodsIDs
		{
			get
			{
				return this.DisableGoodsIDs;
			}
		}

		public int StartRoleNum
		{
			get
			{
				int startRoleNum;
				lock (this.mutex)
				{
					startRoleNum = this._StartRoleNum;
				}
				return startRoleNum;
			}
			set
			{
				lock (this.mutex)
				{
					this._StartRoleNum = value;
				}
			}
		}

		public int AllKilledRoleNum
		{
			get
			{
				int allKilledRoleNum;
				lock (this.mutex)
				{
					allKilledRoleNum = this._AllKilledRoleNum;
				}
				return allKilledRoleNum;
			}
			set
			{
				lock (this.mutex)
				{
					this._AllKilledRoleNum = value;
				}
			}
		}

		public int SuiKilledNum
		{
			get
			{
				int suiKilledNum;
				lock (this.mutex)
				{
					suiKilledNum = this._SuiKilledNum;
				}
				return suiKilledNum;
			}
			set
			{
				lock (this.mutex)
				{
					this._SuiLastKillEmemyTime = TimeUtil.NOW() * 10000L;
					this._SuiKilledNum = value;
				}
			}
		}

		public int TangKilledNum
		{
			get
			{
				int tangKilledNum;
				lock (this.mutex)
				{
					tangKilledNum = this._TangKilledNum;
				}
				return tangKilledNum;
			}
			set
			{
				lock (this.mutex)
				{
					this._TangLastKillEmemyTime = TimeUtil.NOW() * 10000L;
					this._TangKilledNum = value;
				}
			}
		}

		public int SuiClientCount
		{
			get
			{
				int suiClientCount;
				lock (this.mutex)
				{
					suiClientCount = this._SuiClientCount;
				}
				return suiClientCount;
			}
			set
			{
				lock (this.mutex)
				{
					this._SuiClientCount = value;
				}
			}
		}

		public int TangClientCount
		{
			get
			{
				int tangClientCount;
				lock (this.mutex)
				{
					tangClientCount = this._TangClientCount;
				}
				return tangClientCount;
			}
			set
			{
				lock (this.mutex)
				{
					this._TangClientCount = value;
				}
			}
		}

		public void ClearBattleExpByLevels()
		{
			this.BattleExpByLevels = null;
		}

		private long GetBattleExpByLevel(GameClient client, int level)
		{
			long[] array = this.BattleExpByLevels;
			if (null == array)
			{
				SystemXmlItem systemXmlItem = null;
				array = new long[Data.LevelUpExperienceList.Length - 1];
				for (int i = 0; i < array.Length; i++)
				{
					if (GameManager.systemBattleExpMgr.SystemXmlItemDict.TryGetValue(i + 1, out systemXmlItem))
					{
						array[i] = (long)Global.GMax(0, systemXmlItem.GetIntValue("Experience", -1));
					}
				}
				this.BattleExpByLevels = array;
			}
			int num = level - 1;
			long result;
			if (num < 0 || num >= this.BattleExpByLevels.Length)
			{
				result = 0L;
			}
			else
			{
				int changeLifeCount = client.ClientData.ChangeLifeCount;
				double num2;
				if (changeLifeCount == 0)
				{
					num2 = 1.0;
				}
				else
				{
					num2 = Data.ChangeLifeEverydayExpRate[changeLifeCount];
				}
				result = (long)((int)((double)array[num] * num2));
			}
			return result;
		}

		private List<BattleManager.Award> BattleAwardByScore
		{
			get
			{
				List<BattleManager.Award> list = this._BattleAwardByScore;
				if (null == list)
				{
					list = new List<BattleManager.Award>();
					foreach (SystemXmlItem systemXmlItem in GameManager.systemBattleAwardMgr.SystemXmlItemDict.Values)
					{
						BattleManager.Award award = new BattleManager.Award
						{
							MinJiFen = Math.Max(0, systemXmlItem.GetIntValue("MinJiFen", -1)),
							MaxJiFen = Math.Max(0, systemXmlItem.GetIntValue("MaxJiFen", -1)),
							ExpXiShu = Math.Max(0, systemXmlItem.GetIntValue("ExpXiShu", -1)),
							MoJingXiShu = Math.Max(0.0, systemXmlItem.GetDoubleValue("MoJingXiShu")),
							ChengJiuXiShu = Math.Max(0.0, systemXmlItem.GetDoubleValue("ChengJiuXiShu")),
							MinExp = Math.Max(0, systemXmlItem.GetIntValue("MinExp", -1)),
							MaxExp = Math.Max(0, systemXmlItem.GetIntValue("MaxExp", -1)),
							MinMoJing = Math.Max(0, systemXmlItem.GetIntValue("MinMoJing", -1)),
							MaxMoJing = Math.Max(0, systemXmlItem.GetIntValue("MaxMoJing", -1)),
							MinChengJiu = Math.Max(0, systemXmlItem.GetIntValue("MinChengJiu", -1)),
							MaxChengJiu = Math.Max(0, systemXmlItem.GetIntValue("MaxChengJiu", -1))
						};
						if (award.MinJiFen > award.MaxJiFen)
						{
							award.MaxJiFen = 268435455;
						}
						list.Add(award);
					}
					this._BattleAwardByScore = list;
				}
				return list;
			}
			set
			{
				this._BattleAwardByScore = value;
			}
		}

		public void ClearBattleAwardByScore()
		{
			this._BattleAwardByScore = null;
		}

		private int GetSuccessSide()
		{
			int result = -1;
			if (this.TangKilledNum > this.SuiKilledNum)
			{
				result = 2;
			}
			else if (this.TangKilledNum < this.SuiKilledNum)
			{
				result = 1;
			}
			else if (this._SuiLastKillEmemyTime < this._TangLastKillEmemyTime)
			{
				result = 1;
			}
			else if (this._SuiLastKillEmemyTime > this._TangLastKillEmemyTime)
			{
				result = 2;
			}
			return result;
		}

		private bool IsSuccessClient(GameClient client)
		{
			int num;
			if (this.TangKilledNum > this.SuiKilledNum)
			{
				num = 2;
			}
			else if (this.TangKilledNum < this.SuiKilledNum)
			{
				num = 1;
			}
			else if (this._SuiLastKillEmemyTime < this._TangLastKillEmemyTime)
			{
				num = 1;
			}
			else
			{
				if (this._SuiLastKillEmemyTime <= this._TangLastKillEmemyTime)
				{
					return false;
				}
				num = 2;
			}
			return num == client.ClientData.BattleWhichSide;
		}

		private void ProcessRoleBattleExpAndFlagAward(GameClient client, int successSide, int paiMing)
		{
			ProcessTask.ProcessAddTaskVal(client, TaskTypes.Battle, -1, 1, new object[0]);
			List<BattleManager.Award> battleAwardByScore = this.BattleAwardByScore;
			if (null == battleAwardByScore)
			{
				LogManager.WriteLog(2, string.Format("处理大乱斗结束奖励时, 奖励列表项未空", new object[0]), null, true);
			}
			else
			{
				double num = 0.0;
				double num2 = 0.0;
				double num3 = 0.0;
				AwardsItemList awardsItemList = new AwardsItemList();
				bool flag = successSide == client.ClientData.BattleWhichSide;
				double num4 = 0.0;
				HeFuAwardTimesActivity heFuAwardTimesActivity = HuodongCachingMgr.GetHeFuAwardTimesActivity();
				if (heFuAwardTimesActivity != null && heFuAwardTimesActivity.InActivityTime())
				{
					num4 += (double)heFuAwardTimesActivity.activityTimes;
				}
				JieRiMultAwardActivity jieRiMultAwardActivity = HuodongCachingMgr.GetJieRiMultAwardActivity();
				if (null != jieRiMultAwardActivity)
				{
					JieRiMultConfig config = jieRiMultAwardActivity.GetConfig(2);
					if (null != config)
					{
						num4 += config.GetMult();
					}
				}
				SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
				if (null != specPriorityActivity)
				{
					num4 += specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_Battle);
				}
				num4 = Math.Max(1.0, num4);
				foreach (BattleManager.Award award in battleAwardByScore)
				{
					if (client.ClientData.BattleKilledNum >= award.MinJiFen && client.ClientData.BattleKilledNum < award.MaxJiFen)
					{
						num = (double)(client.ClientData.BattleKilledNum * award.ExpXiShu);
						if (award.MoJingXiShu > 0.0)
						{
							num2 = (double)((int)((double)client.ClientData.BattleKilledNum * award.MoJingXiShu));
						}
						if (award.ChengJiuXiShu > 0.0)
						{
							num3 = (double)((int)((double)client.ClientData.BattleKilledNum * award.ChengJiuXiShu));
						}
						if (!flag)
						{
							if (num > 0.0)
							{
								num *= 0.8;
							}
							if (num2 > 0.0)
							{
								num2 *= 0.8;
							}
							if (num3 > 0.0)
							{
								num3 *= 0.8;
							}
						}
						num = (double)((long)(num * Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount]));
						num = Math.Max(num, (double)award.MinExp * Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount]);
						num = Math.Min(num, (double)award.MaxExp * Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount]);
						num2 = Math.Max(num2, (double)award.MinMoJing);
						num2 = Math.Min(num2, (double)award.MaxMoJing);
						num3 = Math.Max(num3, (double)award.MinChengJiu);
						num3 = Math.Min(num3, (double)award.MaxChengJiu);
						if (num > 0.0)
						{
							num = (double)((int)(num * num4));
						}
						if (num2 > 0.0)
						{
							num2 = (double)((int)(num2 * num4));
						}
						if (num3 > 0.0)
						{
							num3 = (double)((int)(num3 * num4));
						}
						break;
					}
				}
				foreach (SystemXmlItem systemXmlItem in GameManager.SystemBattlePaiMingAwards.SystemXmlItemDict.Values)
				{
					if (null != systemXmlItem)
					{
						int num5 = systemXmlItem.GetIntValue("MinPaiMing", -1) - 1;
						int num6 = systemXmlItem.GetIntValue("MaxPaiMing", -1) - 1;
						if (paiMing >= num5 && paiMing <= num6)
						{
							awardsItemList.AddNoRepeat(systemXmlItem.GetStringValue("Goods"));
						}
					}
				}
				if (num > 0.0)
				{
					GameManager.ClientMgr.ProcessRoleExperience(client, (long)num, true, false, false, "none");
				}
				if (num2 > 0.0)
				{
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, (int)num2, "阵营战", false, true, false);
				}
				if (num3 > 0.0)
				{
					GameManager.ClientMgr.ModifyChengJiuPointsValue(client, (int)num3, "阵营战", false, true);
				}
				List<GoodsData> list = Global.ConvertToGoodsDataList(awardsItemList.Items, -1);
				if (!Global.CanAddGoodsDataList(client, list))
				{
					GameManager.ClientMgr.SendMailWhenPacketFull(client, list, GLang.GetLang(13, new object[0]), string.Format(GLang.GetLang(14, new object[0]), paiMing + 1));
				}
				else
				{
					for (int i = 0; i < list.Count; i++)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, list[i].GoodsID, list[i].GCount, list[i].Quality, "", list[i].Forge_level, list[i].Binding, 0, "", true, 1, "阵营战排名奖励", "1900-01-01 12:00:00", 0, list[i].BornIndex, list[i].Lucky, 0, list[i].ExcellenceInfo, list[i].AppendPropLev, 0, null, null, 0, true);
					}
				}
				GameManager.ClientMgr.NotifySelfSuiTangBattleAward(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, this.TangKilledNum, this.SuiKilledNum, (long)num, (int)num2, (int)num3, flag, paiMing, awardsItemList.ToString());
			}
		}

		private void ProcessBattleResultAwards()
		{
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(this.MapCode);
			if (null != mapClients)
			{
				int successSide = this.GetSuccessSide();
				for (int i = 0; i < mapClients.Count; i++)
				{
					GameClient gameClient = mapClients[i] as GameClient;
					if (gameClient != null)
					{
						this.ProcessRoleBattleExpAndFlagAward(gameClient, successSide, i);
					}
				}
				GameManager.GoodsPackMgr.ProcessBattle(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, mapClients, null, 0, 0);
			}
		}

		private void ProcessBattleResultAwards2()
		{
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(this.MapCode);
			if (null != mapClients)
			{
				int successSide = this.GetSuccessSide();
				List<GameClient> list = new List<GameClient>();
				for (int i = 0; i < mapClients.Count; i++)
				{
					GameClient gameClient = mapClients[i] as GameClient;
					if (gameClient != null)
					{
						list.Add(gameClient);
					}
				}
				list.Sort((GameClient x, GameClient y) => y.ClientData.BattleKilledNum - x.ClientData.BattleKilledNum);
				for (int i = 0; i < list.Count; i++)
				{
					this.ProcessRoleBattleExpAndFlagAward(list[i], successSide, i);
				}
				GameManager.GoodsPackMgr.ProcessBattle(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, mapClients, null, 0, 0);
			}
		}

		public void ClearRoleLeaveInfo(int roleID)
		{
			lock (this._RoleLeaveJiFenDict)
			{
				this._RoleLeaveJiFenDict.Remove(roleID);
			}
			lock (this._RoleLeaveTicksDict)
			{
				this._RoleLeaveTicksDict.Remove(roleID);
			}
			lock (this._RoleLeaveSideDict)
			{
				this._RoleLeaveSideDict.Remove(roleID);
			}
		}

		public void ClearAllRoleLeaveInfo()
		{
			lock (this._RoleLeaveJiFenDict)
			{
				this._RoleLeaveJiFenDict.Clear();
			}
			lock (this._RoleLeaveTicksDict)
			{
				this._RoleLeaveTicksDict.Clear();
			}
			lock (this._RoleLeaveSideDict)
			{
				this._RoleLeaveSideDict.Clear();
			}
			BattleManager.m_BattleMaxPointNow = 0;
		}

		public long GetRoleLeaveTicks(int roleID)
		{
			long result = 0L;
			lock (this._RoleLeaveTicksDict)
			{
				this._RoleLeaveTicksDict.TryGetValue(roleID, out result);
			}
			return result;
		}

		public int GetRoleLeaveJiFen(int roleID)
		{
			int result = 0;
			lock (this._RoleLeaveJiFenDict)
			{
				this._RoleLeaveJiFenDict.TryGetValue(roleID, out result);
			}
			return result;
		}

		public int GetRoleLeaveSideID(int roleID)
		{
			int result = 0;
			lock (this._RoleLeaveSideDict)
			{
				this._RoleLeaveSideDict.TryGetValue(roleID, out result);
			}
			return result;
		}

		public void LeaveBattleMap(GameClient client, bool regLastInfo)
		{
			if (client.ClientData.MapCode == GameManager.BattleMgr.MapCode)
			{
				GameManager.BattleMgr.ClientLeave();
				if (1 == client.ClientData.BattleWhichSide)
				{
					GameManager.BattleMgr.SuiClientCount--;
				}
				else
				{
					GameManager.BattleMgr.TangClientCount--;
				}
				if (!regLastInfo)
				{
					GameManager.BattleMgr.ClearRoleLeaveInfo(client.ClientData.RoleID);
				}
				else if (GameManager.BattleMgr.GetBattlingState() < 1 || GameManager.BattleMgr.GetBattlingState() >= 4)
				{
					GameManager.BattleMgr.ClearRoleLeaveInfo(client.ClientData.RoleID);
				}
				else
				{
					int roleID = client.ClientData.RoleID;
					int battleKilledNum = client.ClientData.BattleKilledNum;
					int battleWhichSide = client.ClientData.BattleWhichSide;
					lock (this._RoleLeaveJiFenDict)
					{
						this._RoleLeaveJiFenDict[roleID] = battleKilledNum;
					}
					long value = TimeUtil.NOW();
					lock (this._RoleLeaveTicksDict)
					{
						this._RoleLeaveTicksDict[roleID] = value;
					}
					lock (this._RoleLeaveSideDict)
					{
						this._RoleLeaveSideDict[roleID] = battleWhichSide;
					}
					client.ClientData.BattleWhichSide = 0;
				}
			}
		}

		private void ProcessAddRoleExperience(GameClient client)
		{
			long num = this.GetBattleExpByLevel(client, client.ClientData.Level);
			if (num > 0L)
			{
				double num2 = 0.0;
				JieRiMultAwardActivity jieRiMultAwardActivity = HuodongCachingMgr.GetJieRiMultAwardActivity();
				if (null != jieRiMultAwardActivity)
				{
					JieRiMultConfig config = jieRiMultAwardActivity.GetConfig(2);
					if (null != config)
					{
						num2 += config.GetMult();
					}
				}
				SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
				if (null != specPriorityActivity)
				{
					num2 += specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_Battle);
				}
				num2 = Math.Max(1.0, num2);
				num = (long)((double)num * num2);
				GameManager.ClientMgr.ProcessRoleExperience(client, num, true, false, false, "none");
			}
		}

		private void ProcessTimeAddRoleExp()
		{
			if (this.BattlingState == BattleStates.StartFight)
			{
				long num = TimeUtil.NOW();
				if (num - this.LastAddBattleExpTicks >= (long)(this.AddExpSecs * 1000))
				{
					this.LastAddBattleExpTicks = num;
					List<object> mapClients = GameManager.ClientMgr.GetMapClients(this.MapCode);
					if (null != mapClients)
					{
						for (int i = 0; i < mapClients.Count; i++)
						{
							GameClient gameClient = mapClients[i] as GameClient;
							if (gameClient != null)
							{
								this.ProcessAddRoleExperience(gameClient);
							}
						}
					}
				}
			}
		}

		private void ProcessTimeNotifyBattleKilledNum()
		{
			if (this.BattlingState == BattleStates.StartFight)
			{
				long num = TimeUtil.NOW();
				if (num - this.LastNotifyBattleKilledNumTicks >= (long)(this.NotifyBattleKilledNumSecs * 1000))
				{
					this.LastNotifyBattleKilledNumTicks = num;
					if (this.LastSuiKilledNum != this.SuiKilledNum || this.LastTangKilledNum != this.TangKilledNum)
					{
						GameManager.ClientMgr.NotifyBattleKilledNumCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.SuiKilledNum, this.TangKilledNum);
						this.LastSuiKilledNum = this.SuiKilledNum;
						this.LastTangKilledNum = this.TangKilledNum;
					}
				}
			}
		}

		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				if (!string.IsNullOrEmpty(BattleManager.m_BattleMaxPointName) && BattleManager.m_BattleMaxPointName == oldName)
				{
					BattleManager.m_BattleMaxPointName = newName;
				}
			}
		}

		public const int ConstTopPointNumber = 5;

		public const int ConstJiFenByKillRole = 5;

		public const int ConstJiFenByKilled = 1;

		private List<string> TimePointsList = new List<string>();

		private int MapCode = -1;

		private int MinLevel = 20;

		private int MinRequestNum = 100;

		private int MaxEnterNum = 30;

		private int FallGiftNum = 5;

		private int FallID = -1;

		private string DisableGoodsIDs = "";

		private List<GoodsData> GiveAwardsGoodsDataList = null;

		private int AddExpSecs = 60;

		private int NotifyBattleKilledNumSecs = 30;

		private int WaitingEnterSecs = 30;

		private int PrepareSecs = 30;

		private int FightingSecs = 300;

		private int ClearRolesSecs = 30;

		private int m_NeedMinChangeLev = 0;

		private static int m_BattleMaxPoint = 0;

		private static string m_BattleMaxPointName = "";

		private static int m_BattleMaxPointNow = 0;

		private static int m_nPushMsgDayID = -1;

		private int BattleLineID = 1;

		public static SystemXmlItems systemBattleAwardMgr = null;

		private BattleStates BattlingState = BattleStates.NoBattle;

		private long StateStartTicks = 0L;

		private object RolePointMutex = new object();

		private BattlePointInfo[] TopPointList = new BattlePointInfo[6];

		private Dictionary<int, BattlePointInfo> RolePointDict = new Dictionary<int, BattlePointInfo>();

		private long LastAddBattleExpTicks = 0L;

		private long LastNotifyBattleKilledNumTicks = 0L;

		private object mutex = new object();

		private bool _AllowAttack = false;

		private int _TotalClientCount = 0;

		private int _StartRoleNum = 0;

		private int _AllKilledRoleNum = 0;

		private long _SuiLastKillEmemyTime = 0L;

		private int _SuiKilledNum = 0;

		private long _TangLastKillEmemyTime = 0L;

		private int _TangKilledNum = 0;

		private int _SuiClientCount = 0;

		private int _TangClientCount = 0;

		private long[] BattleExpByLevels = null;

		private List<BattleManager.Award> _BattleAwardByScore = null;

		private Dictionary<int, int> _RoleLeaveJiFenDict = new Dictionary<int, int>();

		private Dictionary<int, int> _RoleLeaveSideDict = new Dictionary<int, int>();

		private Dictionary<int, long> _RoleLeaveTicksDict = new Dictionary<int, long>();

		private int LastSuiKilledNum = -1;

		private int LastTangKilledNum = -1;

		protected class Award
		{
			public int MinJiFen = 0;

			public int MaxJiFen = 200;

			public int ExpXiShu = 5000;

			public double MoJingXiShu = 4.0;

			public double ChengJiuXiShu = 1.0;

			public int MinExp = 3000;

			public int MaxExp = 600000;

			public int MinMoJing = 10;

			public int MaxMoJing = 20;

			public int MinChengJiu = 100;

			public int MaxChengJiu = 200;
		}
	}
}
