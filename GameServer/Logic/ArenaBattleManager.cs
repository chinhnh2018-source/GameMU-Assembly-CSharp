using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.ActivityNew;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class ArenaBattleManager
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
			if (GameManager.SystemArenaBattle.SystemXmlItemDict.TryGetValue(1, out systemXmlItem))
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
				this.MinChangeLifeLev = systemXmlItem.GetIntValue("MinZhuanSheng", -1);
				this.MinLevel = systemXmlItem.GetIntValue("MinLevel", -1);
				this.MinRequestNum = systemXmlItem.GetIntValue("MinRequestNum", -1);
				this.MaxEnterNum = systemXmlItem.GetIntValue("MaxEnterNum", -1);
				this.FallGiftNum = systemXmlItem.GetIntValue("FallGiftNum", -1);
				this.FallID = systemXmlItem.GetIntValue("FallID", -1);
				this.DisableGoodsIDs = systemXmlItem.GetStringValue("DisableGoodsIDs");
				this.AddExpSecs = systemXmlItem.GetIntValue("AddExpSecs", -1);
				this.ForceNotifyBattleScoreSec = Global.GMax(20, Global.GMin(100, systemXmlItem.GetIntValue("NotifyBattleKilledNumSecs", -1)));
				this.WaitingEnterSecs = systemXmlItem.GetIntValue("WaitingEnterSecs", -1);
				this.PrepareSecs = systemXmlItem.GetIntValue("PrepareSecs", -1);
				this.FightingSecs = systemXmlItem.GetIntValue("FightingSecs", -1);
				this.ClearRolesSecs = systemXmlItem.GetIntValue("ClearRolesSecs", -1);
				this.BattleLineID = Global.GMax(1, systemXmlItem.GetIntValue("LineID", -1));
				this.ReloadGiveAwardsGoodsDataList(systemXmlItem);
				ArenaBattleManager.m_nPushMsgDayID = Global.SafeConvertToInt32(GameManager.GameConfigMgr.GetGameConifgItem("PKKingPushMsgDayID"));
			}
		}

		public void ReloadGiveAwardsGoodsDataList(SystemXmlItem systemBattle = null)
		{
			if (null == systemBattle)
			{
				if (!GameManager.SystemArenaBattle.SystemXmlItemDict.TryGetValue(1, out systemBattle))
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
							LogManager.WriteLog(2, string.Format("PK之王配置文件中，配置的固定物品奖励中的物品不存在, GoodsID={0}", num), null, true);
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
			this._AllKilledRoleNum = 0;
			Global.QueryDayActivityTotalPointInfoToDB(SpecialActivityTypes.TheKingOfPK);
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
			this._HandleChangeNameEv();
		}

		private void ProcessBattling()
		{
			if (this.BattlingState == BattleStates.PublishMsg)
			{
				int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
				if (ArenaBattleManager.m_nPushMsgDayID != dayOfYear)
				{
					Global.UpdateDBGameConfigg("PKKingPushMsgDayID", dayOfYear.ToString());
					ArenaBattleManager.m_nPushMsgDayID = dayOfYear;
				}
				long num = TimeUtil.NOW();
				if (num >= this.StateStartTicks + (long)(this.WaitingEnterSecs * 1000))
				{
					GameManager.ClientMgr.NotifyArenaBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode, 2, 2, this.PrepareSecs);
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
					GameManager.ClientMgr.NotifyArenaBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode, 2, 3, this.FightingSecs);
					this.StartRoleNum = GameManager.ClientMgr.GetMapClientsCount(this.MapCode);
					this.EnterBattleClientCount = this.StartRoleNum;
					this.AllKilledRoleNum = 0;
					this.BattlingState = BattleStates.StartFight;
					this.StateStartTicks = TimeUtil.NOW();
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
				else if (num >= this.StateStartTicks + 30000L)
				{
					this.ChampionClient = null;
					int num2 = 0;
					List<GameClient> mapAliveClients = GameManager.ClientMgr.GetMapAliveClients(this.MapCode);
					if (null != mapAliveClients)
					{
						HashSet<int> hashSet = new HashSet<int>();
						lock (this.DeadRoleSets)
						{
							foreach (GameClient gameClient in mapAliveClients)
							{
								if (!hashSet.Add(gameClient.ClientData.RoleID))
								{
									LogManager.WriteLog(2, string.Format("ArenaBattleManager::PK之王活动中角色{0}({1})对象重复", gameClient.ClientData.RoleID, gameClient.ClientData.RoleName), null, true);
								}
								else if (this.DeadRoleSets.Contains(gameClient.ClientData.RoleID))
								{
									LogManager.WriteLog(2, string.Format("ArenaBattleManager::PK之王活动中角色{0}({1})已经死过", gameClient.ClientData.RoleID, gameClient.ClientData.RoleName), null, true);
								}
								else if (Global.InOnlyObsByXY(ObjectTypes.OT_CLIENT, this.MapCode, gameClient.ClientData.PosX, gameClient.ClientData.PosY))
								{
									LogManager.WriteLog(2, string.Format("ArenaBattleManager::PK之王活动中角色{0}({1})卡障碍,重置位置", gameClient.ClientData.RoleID, gameClient.ClientData.RoleName), null, true);
									GameManager.ClientMgr.NotifyOthersGoBack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, -1, -1, -1);
									num2++;
								}
								else
								{
									this.ChampionClient = gameClient;
									num2++;
								}
							}
						}
					}
					if (num2 == 0 || (num2 == 1 && this.ChampionClient != null))
					{
						this.AllowAttack = false;
						this.BattlingState = BattleStates.EndFight;
						this.StateStartTicks = TimeUtil.NOW();
					}
					else
					{
						this.ChampionClient = null;
					}
				}
			}
			else if (this.BattlingState == BattleStates.EndFight)
			{
				this.BattlingState = BattleStates.ClearBattle;
				this.StateStartTicks = TimeUtil.NOW();
				GameManager.ClientMgr.NotifyArenaBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode, 2, 5, this.ClearRolesSecs);
				this.ProcessBattleResultAwards();
			}
			else if (this.BattlingState == BattleStates.ClearBattle)
			{
				long num = TimeUtil.NOW();
				if (num >= this.StateStartTicks + (long)(this.ClearRolesSecs * 1000))
				{
					GameManager.ClientMgr.NotifyBattleLeaveMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode);
					this.BattlingState = BattleStates.NoBattle;
					this.StateStartTicks = 0L;
					this.TheKingOfPKGetawardFlag.Clear();
				}
			}
			this.ProcessTimeNotifyBattleKilledNum();
		}

		private void ProcessNoBattle()
		{
			if (this.JugeStartBattle())
			{
				this.StartRoleNum = 0;
				this.TotalClientCount = 0;
				this._AllKilledRoleNum = 0;
				this._LastNotifyClientCount = 0;
				this._bRoleEnterOrLeave = false;
				this.LastNotifyBattleScoreTicks = 0L;
				lock (this.DeadRoleSets)
				{
					this.DeadRoleSets.Clear();
				}
				GameManager.ClientMgr.BattleBeginForceLeaveg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MapCode);
				this.BattlingState = BattleStates.PublishMsg;
				GameManager.ClientMgr.NotifyAllArenaBattleInviteMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.MinLevel, 1, 1, this.WaitingEnterSecs);
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

		public int TheKingOfPKTopPoint
		{
			get
			{
				return this.TopPoint;
			}
			set
			{
				this.TopPoint = value;
			}
		}

		public string TheKingOfPKTopRoleName
		{
			get
			{
				return this.TopRoleName;
			}
			set
			{
				this.TopRoleName = value;
			}
		}

		public Dictionary<int, int> TheKingOfPKGetawardFlag
		{
			get
			{
				return this.GetawardFlag;
			}
			set
			{
				this.GetawardFlag = value;
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
				return this.BattlingState >= BattleStates.PublishMsg && this.BattlingState < BattleStates.StartFight;
			}
		}

		public bool IsFighting
		{
			get
			{
				return this.BattlingState >= BattleStates.StartFight && this.BattlingState < BattleStates.ClearBattle;
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

		public int AllowMinChangeLifeLev
		{
			get
			{
				return this.MinChangeLifeLev;
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

		public int EnterBattleClientCount
		{
			get
			{
				int enterBattleClientCount;
				lock (this.mutex)
				{
					enterBattleClientCount = this._EnterBattleClientCount;
				}
				return enterBattleClientCount;
			}
			set
			{
				lock (this.mutex)
				{
					this._EnterBattleClientCount = value;
				}
			}
		}

		public bool ClientEnter(GameClient client)
		{
			bool flag = false;
			lock (this.mutex)
			{
				if (this.TheKingOfPKGetawardFlag.ContainsKey(client.ClientData.RoleID))
				{
					return true;
				}
				if (this._TotalClientCount < this.MaxEnterNum)
				{
					this._TotalClientCount++;
					this.TheKingOfPKGetawardFlag.Add(client.ClientData.RoleID, 0);
					flag = true;
				}
			}
			if (flag)
			{
				this._bRoleEnterOrLeave = true;
			}
			return flag;
		}

		protected void ClientLeave(GameClient client)
		{
			lock (this.mutex)
			{
				this._TotalClientCount--;
				this.TheKingOfPKGetawardFlag.Remove(client.ClientData.RoleID);
			}
			this._bRoleEnterOrLeave = true;
		}

		public void LeaveArenaBattleMap(GameClient client)
		{
			if (client.ClientData.MapCode == this.MapCode)
			{
				this.ProcessAward(client);
				this.ClientLeave(client);
			}
		}

		public bool EnterArenaBattleMap(GameClient client)
		{
			return client.ClientData.MapCode == this.MapCode && this.ClientEnter(client);
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

		public void SetTotalPointInfo(string sName, int nValue)
		{
			this.TheKingOfPKTopRoleName = sName;
			this.TheKingOfPKTopPoint = nValue;
		}

		public bool IsInPkScene(int nMap)
		{
			return nMap == 10000;
		}

		public bool ProcessRoleDead(GameClient other)
		{
			int roleID = other.ClientData.RoleID;
			bool flag = false;
			lock (this.DeadRoleSets)
			{
				if (!this.DeadRoleSets.Contains(roleID))
				{
					this.DeadRoleSets.Add(roleID);
					this._AllKilledRoleNum++;
					flag = true;
				}
			}
			if (flag)
			{
			}
			return flag;
		}

		private void ProcessBattleResultAwards()
		{
			GameClient championClient = this.ChampionClient;
			if (null == championClient)
			{
				Global.BroadcastArenaChampionMsg(false, null);
				this.RestorePKingNpc(this.GetPKKingRoleID());
				Global.UpdateDBGameConfigg("PKKingRole", "0");
			}
			else
			{
				this.ProcessAward(championClient);
				Global.BroadcastArenaChampionMsg(true, championClient);
				this.ClearDbKingNpc();
				this.AddBattleBufferAndFlags(championClient);
				this.SetPKKingRoleID(championClient.ClientData.RoleID);
				this.ReplacePKKingNpc(championClient.ClientData.RoleID);
			}
		}

		private void AddBattleBufferAndFlags(GameClient client)
		{
			double[] array = new double[]
			{
				85200.0,
				2000800.0
			};
			client.ClientData.BattleNameStart = TimeUtil.NOW();
			client.ClientData.BattleNameIndex = 1;
			Global.RemoveBufferData(client, 24);
			Global.RemoveBufferData(client, 26);
			Global.RemoveBufferData(client, 25);
			Global.UpdateBufferData(client, BufferItemTypes.PKKingBuffer, array, 0, true);
			GameManager.DBCmdMgr.AddDBCmd(10059, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.BattleNameStart, client.ClientData.BattleNameIndex), null, client.ServerId);
			GameManager.ClientMgr.NotifyRoleBattleNameInfo(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.UpdateBattleNum(client, 1, false);
			HuodongCachingMgr.UpdateHeFuPKKingRoleID(client.ClientData.RoleID);
			EventLogManager.AddTitleEvent(client, 1, (int)array[0], "pkKing");
		}

		public int GetPKKingRoleID()
		{
			return GameManager.GameConfigMgr.GetGameConfigItemInt("PKKingRole", 0);
		}

		public void SetPKKingRoleID(int roleID)
		{
			Global.UpdateDBGameConfigg("PKKingRole", roleID.ToString());
			GameManager.GameConfigMgr.SetGameConfigItem("PKKingRole", roleID.ToString());
		}

		private void ProcessTimeNotifyBattleKilledNum()
		{
			bool flag = false;
			long num = TimeUtil.NOW();
			int mapClientsCount = GameManager.ClientMgr.GetMapClientsCount(this.MapCode);
			if (this._bRoleEnterOrLeave)
			{
				this._bRoleEnterOrLeave = false;
				flag = true;
			}
			else if (num - this.LastNotifyBattleScoreTicks >= (long)(this.ForceNotifyBattleScoreSec * 1000))
			{
				flag = true;
			}
			else if (this._LastNotifyClientCount != mapClientsCount)
			{
				flag = true;
			}
			if (flag)
			{
				this.LastNotifyBattleScoreTicks = num;
				this._LastNotifyClientCount = mapClientsCount;
				GameManager.ClientMgr.NotifyArenaBattleKilledNumCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.AllKilledRoleNum, this.StartRoleNum, this.TotalClientCount);
			}
		}

		private void ProcessTimeAddRoleExp()
		{
			if (this.BattlingState == BattleStates.StartFight)
			{
				long num = TimeUtil.NOW();
				if (num - this.LastAddBangZhanAwardsTicks >= 10000L)
				{
					this.LastAddBangZhanAwardsTicks = num;
					List<object> mapClients = GameManager.ClientMgr.GetMapClients(this.MapCode);
					if (null != mapClients)
					{
						for (int i = 0; i < mapClients.Count; i++)
						{
							GameClient gameClient = mapClients[i] as GameClient;
							if (gameClient != null)
							{
								BangZhanAwardsMgr.ProcessBangZhanAwards(gameClient);
							}
						}
					}
				}
			}
		}

		private void ProcessAward(GameClient client)
		{
			if (this.BattlingState >= BattleStates.StartFight)
			{
				if (this.BattlingState == BattleStates.StartFight)
				{
					long num = TimeUtil.NOW();
					if (num < this.StateStartTicks + 1000L)
					{
						return;
					}
				}
				lock (this.mutex)
				{
					int num2;
					if (!this.TheKingOfPKGetawardFlag.TryGetValue(client.ClientData.RoleID, out num2))
					{
						return;
					}
					if (num2 > 0)
					{
						return;
					}
					this.TheKingOfPKGetawardFlag[client.ClientData.RoleID] = 1;
				}
				if (client.ClientData.KingOfPkCurrentPoint > this.TheKingOfPKTopPoint)
				{
					this.SetTotalPointInfo(client.ClientData.RoleName, client.ClientData.KingOfPkCurrentPoint);
				}
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("PkAward");
				string[] array = null;
				string[] array2 = null;
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array3 = paramValueByName.Split(new char[]
					{
						'|'
					});
					string text = array3[0];
					array = text.Split(new char[]
					{
						','
					});
					text = array3[1];
					array2 = text.Split(new char[]
					{
						','
					});
				}
				HeFuAwardTimesActivity heFuAwardTimesActivity = HuodongCachingMgr.GetHeFuAwardTimesActivity();
				JieRiMultAwardActivity jieRiMultAwardActivity = HuodongCachingMgr.GetJieRiMultAwardActivity();
				SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
				double num3 = 0.0;
				if (heFuAwardTimesActivity != null && heFuAwardTimesActivity.InActivityTime() && (double)heFuAwardTimesActivity.activityTimes > 0.0)
				{
					num3 += (double)heFuAwardTimesActivity.activityTimes;
				}
				if (null != jieRiMultAwardActivity)
				{
					JieRiMultConfig config = jieRiMultAwardActivity.GetConfig(3);
					if (null != config)
					{
						num3 += config.GetMult();
					}
				}
				if (null != specPriorityActivity)
				{
					num3 += specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_PKKing);
				}
				num3 = Math.Max(1.0, num3);
				int num4 = Global.SafeConvertToInt32(array[0]) + Global.GMin(Global.SafeConvertToInt32(array[1]), client.ClientData.KingOfPkCurrentPoint) * Global.SafeConvertToInt32(array[2]);
				num4 *= (int)num3;
				if (num4 > 0)
				{
					ChengJiuManager.AddChengJiuPoints(client, "角斗赛", num4, true, true);
				}
				double num5 = Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount];
				long num6 = (long)((int)((double)Global.SafeConvertToInt32(array2[0]) * num5 + (double)(Global.GMin(Global.SafeConvertToInt32(array2[1]), client.ClientData.KingOfPkCurrentPoint) * Global.SafeConvertToInt32(array2[2])) * num5));
				double num7 = 1.0;
				num7 += num3;
				num6 = (long)((int)((double)num6 * num7));
				if (num6 > 0L)
				{
					GameManager.ClientMgr.ProcessRoleExperience(client, num6, true, true, false, "none");
				}
				string strCmd = string.Format("{0}:{1}:{2}", client.ClientData.KingOfPkCurrentPoint, num4, num6);
				client.ClientData.KingOfPkCurrentPoint = 0;
				GameManager.ClientMgr.SendToClient(client, strCmd, 569);
				ProcessTask.ProcessAddTaskVal(client, TaskTypes.PKKing, -1, 1, new object[0]);
			}
		}

		public RoleDataEx KingRoleData
		{
			get
			{
				RoleDataEx kingRoleData;
				lock (this.kingRoleDataMutex)
				{
					kingRoleData = this._kingRoleData;
				}
				return kingRoleData;
			}
			private set
			{
				lock (this.kingRoleDataMutex)
				{
					this._kingRoleData = value;
				}
			}
		}

		public void ReShowPKKing()
		{
			int pkkingRoleID = this.GetPKKingRoleID();
			if (pkkingRoleID > 0)
			{
				this.ReplacePKKingNpc(pkkingRoleID);
			}
		}

		public void ClearDbKingNpc()
		{
			this.KingRoleData = null;
			Global.sendToDB<bool, string>(13232, string.Format("{0}", 1), 0);
		}

		public void ReplacePKKingNpc(int roleId)
		{
			RoleDataEx roleDataEx = this.KingRoleData;
			this.KingRoleData = null;
			if (roleDataEx == null || roleDataEx.RoleID != roleId)
			{
				roleDataEx = Global.sendToDB<RoleDataEx, KingRoleGetData>(13230, new KingRoleGetData
				{
					KingType = 1
				}, 0);
				if (roleDataEx == null || roleDataEx.RoleID != roleId)
				{
					RoleDataEx roleDataEx2 = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, roleId), 0);
					if (roleDataEx2 == null || roleDataEx2.RoleID <= 0)
					{
						return;
					}
					roleDataEx = roleDataEx2;
					if (!Global.sendToDB<bool, KingRolePutData>(13231, new KingRolePutData
					{
						KingType = 1,
						RoleDataEx = roleDataEx
					}, 0))
					{
					}
				}
			}
			if (roleDataEx != null && roleDataEx.RoleID > 0)
			{
				this.KingRoleData = roleDataEx;
				NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, FakeRoleNpcId.PkKing);
				if (null != npc)
				{
					npc.ShowNpc = false;
					GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
					FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.DiaoXiang, false);
					FakeRoleManager.ProcessNewFakeRole(new SafeClientData
					{
						RoleData = roleDataEx
					}, npc.MapCode, FakeRoleTypes.DiaoXiang, 4, (int)npc.CurrentPos.X, (int)npc.CurrentPos.Y, FakeRoleNpcId.PkKing);
				}
			}
		}

		public void RestorePKingNpc(int pkKingRoleID)
		{
			NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, FakeRoleNpcId.PkKing);
			if (null != npc)
			{
				npc.ShowNpc = true;
				GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
				FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.DiaoXiang, false);
			}
		}

		public bool IsInArenaBattle(GameClient client)
		{
			return client.ClientData.MapCode == GameManager.ArenaBattleMgr.BattleMapCode;
		}

		public void AddArenaBattleKilledNum(GameClient client, object victim)
		{
			if (client.ClientData.MapCode == this.BattleMapCode)
			{
				GameClient gameClient = victim as GameClient;
				if (victim != null && null != gameClient)
				{
					if (this.ProcessRoleDead(gameClient))
					{
						client.ClientData.ArenaBattleKilledNum++;
						client.ClientData.KingOfPkCurrentPoint += client.ClientData.ArenaBattleKilledNum * 5;
						if (client.ClientData.KingOfPkCurrentPoint > client.ClientData.KingOfPkTopPoint)
						{
							client.ClientData.KingOfPkTopPoint = client.ClientData.KingOfPkCurrentPoint;
						}
					}
				}
			}
		}

		public void ClientEnterArenaBattle(GameClient client)
		{
			if (this.BattleMapCode < 0)
			{
				GameManager.ClientMgr.NotifyArenaBattleCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1, 4, this.GetBattlingState(), this.GetBattlingLeftSecs());
			}
			else if (this.BattleServerLineID != GameManager.ServerLineID)
			{
				GameManager.ClientMgr.NotifyArenaBattleCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1000 - this.BattleServerLineID, 4, this.GetBattlingState(), this.GetBattlingLeftSecs());
			}
			else if (client.ClientData.ChangeLifeCount < this.AllowMinChangeLifeLev)
			{
				GameManager.ClientMgr.NotifyArenaBattleCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -11, 4, this.GetBattlingState(), this.GetBattlingLeftSecs());
			}
			else
			{
				if (client.ClientData.ChangeLifeCount == this.AllowMinChangeLifeLev)
				{
					if (client.ClientData.Level < this.AllowMinLevel)
					{
						GameManager.ClientMgr.NotifyArenaBattleCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -10, 4, this.GetBattlingState(), this.GetBattlingLeftSecs());
						return;
					}
				}
				if (!this.AllowEnterMap)
				{
					int status = -2;
					if (this.IsFighting)
					{
						status = -22;
					}
					GameManager.ClientMgr.NotifyArenaBattleCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, status, 4, this.GetBattlingState(), this.GetBattlingLeftSecs());
				}
				else if (!this.ClientEnter(client))
				{
					GameManager.ClientMgr.NotifyArenaBattleCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -3, 4, this.GetBattlingState(), this.GetBattlingLeftSecs());
				}
				else
				{
					client.ClientData.ArenaBattleKilledNum = 0;
					client.ClientData.KingOfPkCurrentPoint = 0;
					int battleMapCode = this.BattleMapCode;
					GameMap gameMap = null;
					if (GameManager.MapMgr.DictMaps.TryGetValue(battleMapCode, out gameMap))
					{
						GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, battleMapCode, -1, -1, -1, 0);
						GameManager.ClientMgr.NotifyArenaBattleCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 0, 4, Global.GMax(2, this.GetBattlingState()), this.GetBattlingLeftSecs());
						Global.BroadcastClientEnterArenaBattle(client);
						Global.UpdateDayActivityEnterCountToDB(client, client.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, 4, 1);
					}
				}
			}
		}

		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				Tuple<int, string, string> item = new Tuple<int, string, string>(roleId, oldName, newName);
				lock (this._ChangeNameEvQ)
				{
					this._ChangeNameEvQ.Enqueue(item);
				}
			}
		}

		private void _HandleChangeNameEv()
		{
			List<Tuple<int, string, string>> list = null;
			lock (this._ChangeNameEvQ)
			{
				list = this._ChangeNameEvQ.ToList<Tuple<int, string, string>>();
				this._ChangeNameEvQ.Clear();
			}
			if (list != null && list.Count != 0)
			{
				foreach (Tuple<int, string, string> tuple in list)
				{
					int item = tuple.Item1;
					string item2 = tuple.Item2;
					string item3 = tuple.Item3;
					RoleDataEx kingRoleData = this.KingRoleData;
					if (kingRoleData != null && kingRoleData.RoleID == item)
					{
						kingRoleData.RoleName = item3;
						if (!Global.sendToDB<bool, KingRolePutData>(13231, new KingRolePutData
						{
							KingType = 1,
							RoleDataEx = kingRoleData
						}, 0))
						{
						}
						this.KingRoleData = null;
						this.ReShowPKKing();
					}
					if (!string.IsNullOrEmpty(this.TheKingOfPKTopRoleName) && this.TheKingOfPKTopRoleName == item2)
					{
						this.TheKingOfPKTopRoleName = item3;
					}
				}
			}
		}

		private int TopPoint = -1;

		private string TopRoleName = "";

		private Dictionary<int, int> GetawardFlag = new Dictionary<int, int>();

		private List<string> TimePointsList = new List<string>();

		private int MapCode = -1;

		private int MinChangeLifeLev = 0;

		private int MinLevel = 20;

		private int MinRequestNum = 100;

		private int MaxEnterNum = 300;

		private int FallGiftNum = 5;

		private int FallID = -1;

		private string DisableGoodsIDs = "";

		private List<GoodsData> GiveAwardsGoodsDataList = null;

		private int AddExpSecs = 60;

		private int ForceNotifyBattleScoreSec = 10;

		private int WaitingEnterSecs = 30;

		private int PrepareSecs = 30;

		private int FightingSecs = 300;

		private int ClearRolesSecs = 30;

		private int BattleLineID = 1;

		public static int m_nPushMsgDayID = -1;

		private Queue<Tuple<int, string, string>> _ChangeNameEvQ = new Queue<Tuple<int, string, string>>();

		private BattleStates BattlingState = BattleStates.NoBattle;

		private long StateStartTicks = 0L;

		private long KeepSingleTicks;

		private long LastNotifyBattleScoreTicks = 0L;

		private HashSet<int> DeadRoleSets = new HashSet<int>();

		private GameClient ChampionClient = null;

		private object mutex = new object();

		private bool _AllowAttack = false;

		private int _TotalClientCount = 0;

		private int _LastNotifyClientCount = 0;

		private bool _bRoleEnterOrLeave = false;

		private int _EnterBattleClientCount = 0;

		private int _StartRoleNum = 0;

		private int _AllKilledRoleNum = 0;

		private long LastAddBangZhanAwardsTicks = 0L;

		private object kingRoleDataMutex = new object();

		private RoleDataEx _kingRoleData = null;
	}
}
