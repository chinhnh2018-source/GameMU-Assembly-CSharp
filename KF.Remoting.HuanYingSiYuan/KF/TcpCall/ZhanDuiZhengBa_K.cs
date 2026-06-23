using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using AutoCSer.Net.TcpInternalServer;
using AutoCSer.Net.TcpStaticServer;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting;
using KF.Remoting.Data;
using Remoting;
using Server.Tools;
using Tmsk.Contract;

namespace KF.TcpCall
{
	[Server(Name = "KfCall", IsServer = true, IsAttribute = true, IsClientAwaiter = false, MemberFilters = 240, IsSegmentation = true, ClientSegmentationCopyPath = "GameServer\\Remoting\\")]
	public static class ZhanDuiZhengBa_K
	{
		static ZhanDuiZhengBa_K()
		{
			ZhanDuiZhengBa_K.StateMachine.Install(new StateMachineSimple.StateHandler(1, null, new Action<DateTime>(ZhanDuiZhengBa_K.MS_Idle_Update), null));
			ZhanDuiZhengBa_K.StateMachine.Install(new StateMachineSimple.StateHandler(2, new Action<DateTime>(ZhanDuiZhengBa_K.MS_Init_Enter), null, null));
			ZhanDuiZhengBa_K.StateMachine.Install(new StateMachineSimple.StateHandler(4, null, new Action<DateTime>(ZhanDuiZhengBa_K.MS_InitPkLoop_Update), null));
			ZhanDuiZhengBa_K.StateMachine.Install(new StateMachineSimple.StateHandler(5, null, new Action<DateTime>(ZhanDuiZhengBa_K.MS_NotifyEnter_Update), null));
			ZhanDuiZhengBa_K.StateMachine.Install(new StateMachineSimple.StateHandler(6, null, new Action<DateTime>(ZhanDuiZhengBa_K.MS_PkLoopStart_Update), null));
			ZhanDuiZhengBa_K.StateMachine.Install(new StateMachineSimple.StateHandler(7, null, new Action<DateTime>(ZhanDuiZhengBa_K.MS_PkLoopEnd_Update), null));
			ZhanDuiZhengBa_K.StateMachine.Install(new StateMachineSimple.StateHandler(3, new Action<DateTime>(ZhanDuiZhengBa_K.MS_TodayPkEnd_Enter), new Action<DateTime>(ZhanDuiZhengBa_K.MS_TodayPkEnd_Update), null));
			ZhanDuiZhengBa_K.StateMachine.SetCurrState(1, TimeUtil.NowDateTime());
		}

		public static bool InitConfig()
		{
			lock (ZhanDuiZhengBa_K.Mutex)
			{
				ZhanDuiZhengBa_K.Initialize = false;
				bool flag2 = ZhanDuiZhengBa_K._Config.Load(KuaFuServerManager.GetResourcePath("Config\\TeamMatch.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\TeamMatchBirthPoint.xml", KuaFuServerManager.ResourcePathTypes.GameRes));
				ZhanDuiZhengBaConsts.MinZhanDuiNum = (Global.TestMode ? 0 : 4);
				if (!flag2)
				{
					LogManager.WriteLog(2, "ZhanDuiZhengBa_K.InitConfig failed!", null, true);
				}
				ZhanDuiZhengBa_K.Initialize = flag2;
			}
			return true;
		}

		public static void Update()
		{
			if (ZhanDuiZhengBa_K.Initialize)
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				if (dateTime.Day != ZhanDuiZhengBa_K.lastUpdateTime.Day)
				{
					ZhanDuiZhengBa_K.FixSyncData(dateTime);
				}
				else
				{
					lock (ZhanDuiZhengBa_K.Mutex)
					{
						ZhanDuiZhengBa_K.StateMachine.Tick(dateTime);
					}
				}
				KFCallMsg[] array = null;
				lock (ZhanDuiZhengBa_K.Mutex)
				{
					array = ZhanDuiZhengBa_K.AsyncEvQ.ToArray();
					ZhanDuiZhengBa_K.AsyncEvQ.Clear();
				}
				foreach (KFCallMsg msg in array)
				{
					ClientAgentManager.Instance().BroadCastMsg(msg, 0);
				}
				ZhanDuiZhengBa_K.lastUpdateTime = dateTime;
			}
		}

		private static void MS_Idle_Update(DateTime now)
		{
			if (now.Day == ZhanDuiZhengBaConfig.StartDay)
			{
				int num = TimeUtil.MakeYearMonth(now);
				lock (ZhanDuiZhengBa_K.Mutex)
				{
					if (num == ZhanDuiZhengBa_K.SyncData.Month)
					{
						for (int i = 0; i < ZhanDuiZhengBa_K._Config.MatchConfigList.Count; i++)
						{
							ZhanDuiZhengBaMatchConfig zhanDuiZhengBaMatchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList[i];
							if (i == 0 && now.TimeOfDay.Ticks < zhanDuiZhengBaMatchConfig.DayBeginTick)
							{
								return;
							}
							if (now.TimeOfDay.Ticks >= zhanDuiZhengBaMatchConfig.DayBeginTick)
							{
								if (!Consts.TestMode)
								{
									if (now.TimeOfDay.Ticks >= zhanDuiZhengBaMatchConfig.DayBeginTick && now.TimeOfDay.Ticks < zhanDuiZhengBaMatchConfig.DayEndTick)
									{
										if (ZhanDuiZhengBa_K.SyncData.RealActID != zhanDuiZhengBaMatchConfig.ID)
										{
											ZhanDuiZhengBa_K.SyncData.RealActID = zhanDuiZhengBaMatchConfig.ID;
											ZhanDuiZhengBa_K.StateMachine.SetCurrState(2, now);
										}
										return;
									}
									if (i == ZhanDuiZhengBa_K._Config.MatchConfigList.Count - 1 && now.TimeOfDay.Ticks >= zhanDuiZhengBaMatchConfig.DayEndTick)
									{
										if (ZhanDuiZhengBa_K.SyncData.RealActID != zhanDuiZhengBaMatchConfig.ID)
										{
											ZhanDuiZhengBa_K.SyncData.RealActID = zhanDuiZhengBaMatchConfig.ID;
										}
										return;
									}
								}
								else if (now.TimeOfDay.Ticks >= zhanDuiZhengBaMatchConfig.DayBeginTick && now.TimeOfDay.Ticks < zhanDuiZhengBaMatchConfig.DayEndTick)
								{
									if (ZhanDuiZhengBa_K.SyncData.RealActID < zhanDuiZhengBaMatchConfig.ID || zhanDuiZhengBaMatchConfig.ID == 1)
									{
										ZhanDuiZhengBa_K.SyncData.RealActID = zhanDuiZhengBaMatchConfig.ID;
										ZhanDuiZhengBa_K.StateMachine.SetCurrState(2, now);
										return;
									}
								}
								else if (i == ZhanDuiZhengBa_K._Config.MatchConfigList.Count - 1 || now.TimeOfDay.Ticks < ZhanDuiZhengBa_K._Config.MatchConfigList[i + 1].DayBeginTick)
								{
									if (ZhanDuiZhengBa_K.SyncData.RealActID != zhanDuiZhengBaMatchConfig.ID)
									{
										ZhanDuiZhengBa_K.SyncData.RealActID = zhanDuiZhengBaMatchConfig.ID;
										ZhanDuiZhengBa_K.StateMachine.SetCurrState(2, now);
									}
									return;
								}
							}
						}
						for (int i = 0; i < ZhanDuiZhengBa_K._Config.MatchConfigList.Count; i++)
						{
							ZhanDuiZhengBaMatchConfig zhanDuiZhengBaMatchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList[i];
							if (now.TimeOfDay.Ticks >= zhanDuiZhengBaMatchConfig.DayBeginTick)
							{
								if (i == ZhanDuiZhengBa_K._Config.MatchConfigList.Count - 1 || now.TimeOfDay.Ticks < ZhanDuiZhengBa_K._Config.MatchConfigList[i + 1].DayBeginTick)
								{
									ZhanDuiZhengBa_K.SyncData.RealActID = zhanDuiZhengBaMatchConfig.ID;
									ZhanDuiZhengBa_K.StateMachine.SetCurrState(7, now);
									break;
								}
							}
						}
					}
				}
			}
			else
			{
				ZhanDuiZhengBa_K.SyncData.RealActID = 0;
			}
		}

		private static void MS_Init_Enter(DateTime now)
		{
			ZhanDuiZhengBa_K.TodayJoinRoleDatas.Clear();
			foreach (ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData in ZhanDuiZhengBa_K.SyncData.ZhanDuiList)
			{
				if (zhanDuiZhengBaZhanDuiData.State == 0 || zhanDuiZhengBaZhanDuiData.State == 1)
				{
					ZhanDuiZhengBa_K.TodayJoinRoleDatas.Add(new ZhanDuiZhengBa_K.JoinRolePkData
					{
						ZhanDuiID = zhanDuiZhengBaZhanDuiData.ZhanDuiID,
						ZoneId = zhanDuiZhengBaZhanDuiData.ZoneId,
						RoleName = zhanDuiZhengBaZhanDuiData.ZhanDuiName,
						Group = zhanDuiZhengBaZhanDuiData.Group,
						Rank = zhanDuiZhengBaZhanDuiData.DuanWeiRank
					});
				}
			}
			ZhanDuiZhengBa_K.ThisLoopPkLogs.Clear();
			ZhanDuiZhengBa_K.StateMachine.SetCurrState(4, now);
		}

		private static bool CreateGameFuBen(DateTime now, ZhanDuiZhengBaMatchConfig matchConfig, ZhanDuiZhengBa_K.JoinRolePkData joinRole1, ZhanDuiZhengBa_K.JoinRolePkData joinRole2)
		{
			bool result;
			if (joinRole1.CurrGameID > 0)
			{
				result = true;
			}
			else
			{
				int betterZhanDuiID = (joinRole1.Rank < joinRole2.Rank) ? joinRole1.ZhanDuiID : joinRole2.ZhanDuiID;
				int num = 0;
				int nextGameId = TianTiPersistence.Instance.GetNextGameId();
				if (ClientAgentManager.Instance().AssginKfFuben(ZhanDuiZhengBa_K.GameType, (long)nextGameId, 10, out num))
				{
					ZhanDuiZhengBaFuBenData zhanDuiZhengBaFuBenData = new ZhanDuiZhengBaFuBenData();
					zhanDuiZhengBaFuBenData.GameID = (long)nextGameId;
					zhanDuiZhengBaFuBenData.SideDict[(long)joinRole1.ZhanDuiID] = 1;
					zhanDuiZhengBaFuBenData.SideDict[(long)joinRole2.ZhanDuiID] = 2;
					zhanDuiZhengBaFuBenData.BetterZhanDuiID = betterZhanDuiID;
					zhanDuiZhengBaFuBenData.ConfigID = matchConfig.ID;
					zhanDuiZhengBaFuBenData.JoinGrade = matchConfig.JoinGrade;
					zhanDuiZhengBaFuBenData.NewGrade = matchConfig.WillUpGrade;
					zhanDuiZhengBaFuBenData.ServerID = num;
					zhanDuiZhengBaFuBenData.RoleDict.AddRange(TianTi5v5Service.GetZhanDuiMemberIDs(joinRole1.ZhanDuiID));
					if (joinRole1.ZhanDuiID != joinRole2.ZhanDuiID)
					{
						zhanDuiZhengBaFuBenData.RoleDict.AddRange(TianTi5v5Service.GetZhanDuiMemberIDs(joinRole2.ZhanDuiID));
					}
					joinRole1.ToServerID = (joinRole2.ToServerID = num);
					joinRole1.CurrGameID = (joinRole2.CurrGameID = nextGameId);
					joinRole1.CopyData = (joinRole2.CopyData = zhanDuiZhengBaFuBenData);
					joinRole1.WaitReqEnter = (joinRole2.WaitReqEnter = true);
					ZhanDuiZhengBaNtfEnterData zhanDuiZhengBaNtfEnterData = new ZhanDuiZhengBaNtfEnterData();
					zhanDuiZhengBaNtfEnterData.ZhanDuiID1 = joinRole1.ZhanDuiID;
					zhanDuiZhengBaNtfEnterData.ZhanDuiID2 = joinRole2.ZhanDuiID;
					zhanDuiZhengBaNtfEnterData.ToServerId = num;
					zhanDuiZhengBaNtfEnterData.GameId = nextGameId;
					zhanDuiZhengBaNtfEnterData.Day = ZhanDuiZhengBa_K.SyncData.RealActID;
					ZhanDuiZhengBa_K.AsyncEvQ.Enqueue(KFCallMsg.New<ZhanDuiZhengBaNtfEnterData>(10034, zhanDuiZhengBaNtfEnterData));
					LogManager.WriteLog(0, string.Format("战队争霸第{0}轮战队成员通知入场，zhanduiID1={1},zhanduiID2={2}", ZhanDuiZhengBa_K.SyncData.RealActID, joinRole1.ZhanDuiID, joinRole2.ZhanDuiID), null, true);
					ZhanDuiZhengBaPkLogData zhanDuiZhengBaPkLogData = new ZhanDuiZhengBaPkLogData();
					zhanDuiZhengBaPkLogData.Month = ZhanDuiZhengBa_K.SyncData.Month;
					zhanDuiZhengBaPkLogData.ID = ZhanDuiZhengBa_K.SyncData.RealActID;
					zhanDuiZhengBaPkLogData.ZhanDuiID1 = joinRole1.ZhanDuiID;
					zhanDuiZhengBaPkLogData.ZoneID1 = joinRole1.ZoneId;
					zhanDuiZhengBaPkLogData.ZhanDuiName1 = joinRole1.RoleName;
					zhanDuiZhengBaPkLogData.ZhanDuiID2 = joinRole2.ZhanDuiID;
					zhanDuiZhengBaPkLogData.ZoneID2 = joinRole2.ZoneId;
					zhanDuiZhengBaPkLogData.ZhanDuiName2 = joinRole2.RoleName;
					zhanDuiZhengBaPkLogData.StartTime = now;
					zhanDuiZhengBaPkLogData.BetterZhanDuiID = betterZhanDuiID;
					zhanDuiZhengBaPkLogData.GameID = (long)nextGameId;
					ZhanDuiZhengBa_K.ThisLoopPkLogs[nextGameId] = zhanDuiZhengBaPkLogData;
					result = true;
				}
				else
				{
					LogManager.WriteLog(2, string.Format("战队争霸第{0}轮分配游戏服务器失败，zhanduiID1={1},zhanduiID2={2}", ZhanDuiZhengBa_K.SyncData.RealActID, joinRole1.ZhanDuiID, joinRole2.ZhanDuiID), null, true);
					result = false;
				}
			}
			return result;
		}

		private static void MS_InitPkLoop_Update(DateTime now)
		{
			ZhanDuiZhengBaMatchConfig matchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == ZhanDuiZhengBa_K.SyncData.RealActID);
			List<ZhanDuiZhengBa_K.JoinRolePkData> list = new List<ZhanDuiZhengBa_K.JoinRolePkData>();
			using (List<RangeKey>.Enumerator enumerator = ZhanDuiZhengBaUtils.GetDayPkGroupRange(ZhanDuiZhengBa_K.SyncData.RealActID).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					RangeKey range = enumerator.Current;
					List<ZhanDuiZhengBa_K.JoinRolePkData> list2 = ZhanDuiZhengBa_K.TodayJoinRoleDatas.FindAll((ZhanDuiZhengBa_K.JoinRolePkData _r) => _r.Group >= range.Left && _r.Group <= range.Right);
					if (list2 != null && list2.Count == 2)
					{
						ZhanDuiZhengBa_K.JoinRolePkData joinRolePkData = list2[0];
						ZhanDuiZhengBa_K.JoinRolePkData joinRole = list2[1];
						if (joinRolePkData.CurrGameID <= 0)
						{
							if (!ZhanDuiZhengBa_K.CreateGameFuBen(now, matchConfig, joinRolePkData, joinRole))
							{
								return;
							}
						}
					}
					else if (list2 != null && list2.Count == 1)
					{
						if (!Consts.TestMode)
						{
							ZhanDuiZhengBa_K.JoinRolePkData joinRolePkData2 = list2[0];
							joinRolePkData2.ToServerID = 0;
							joinRolePkData2.CurrGameID = 0;
							joinRolePkData2.WaitReqEnter = false;
						}
						else
						{
							ZhanDuiZhengBa_K.JoinRolePkData joinRolePkData = list2[0];
							ZhanDuiZhengBa_K.JoinRolePkData joinRole = list2[0];
							if (joinRolePkData.CurrGameID <= 0)
							{
								if (!ZhanDuiZhengBa_K.CreateGameFuBen(now, matchConfig, joinRolePkData, joinRole))
								{
									return;
								}
							}
						}
					}
				}
			}
			ZhanDuiZhengBa_K.StateMachine.SetCurrState(5, now);
		}

		private static void MS_NotifyEnter_Update(DateTime now)
		{
			ZhanDuiZhengBaMatchConfig zhanDuiZhengBaMatchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == ZhanDuiZhengBa_K.SyncData.RealActID);
			if (now.TimeOfDay.Ticks >= zhanDuiZhengBaMatchConfig.DayBeginTick + 20000000L * (long)zhanDuiZhengBaMatchConfig.WaitSeconds)
			{
				ZhanDuiZhengBa_K.StateMachine.SetCurrState(6, now);
			}
		}

		private static void MS_PkLoopStart_Update(DateTime now)
		{
			ZhanDuiZhengBaMatchConfig zhanDuiZhengBaMatchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == ZhanDuiZhengBa_K.SyncData.RealActID);
			if (now.TimeOfDay.Ticks >= zhanDuiZhengBaMatchConfig.DayBeginTick + 10000000L * (long)(zhanDuiZhengBaMatchConfig.WaitSeconds + zhanDuiZhengBaMatchConfig.FightSeconds + zhanDuiZhengBaMatchConfig.ClearSeconds))
			{
				ZhanDuiZhengBa_K.StateMachine.SetCurrState(7, now);
			}
		}

		private static void MS_PkLoopEnd_Update(DateTime now)
		{
			ZhanDuiZhengBaMatchConfig zhanDuiZhengBaMatchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == ZhanDuiZhengBa_K.SyncData.RealActID);
			if (now.TimeOfDay.Ticks >= zhanDuiZhengBaMatchConfig.DayBeginTick + 10000000L * (long)(zhanDuiZhengBaMatchConfig.WaitSeconds + zhanDuiZhengBaMatchConfig.FightSeconds + zhanDuiZhengBaMatchConfig.ClearSeconds))
			{
				foreach (ZhanDuiZhengBaPkLogData zhanDuiZhengBaPkLogData in ZhanDuiZhengBa_K.ThisLoopPkLogs.Values.ToList<ZhanDuiZhengBaPkLogData>())
				{
					ZhanDuiZhengBa_K.ZhengBaPkResult((int)zhanDuiZhengBaPkLogData.GameID, zhanDuiZhengBaPkLogData.BetterZhanDuiID);
				}
				ZhanDuiZhengBa_K.ThisLoopPkLogs.Clear();
				foreach (ZhanDuiZhengBa_K.JoinRolePkData joinRolePkData in ZhanDuiZhengBa_K.TodayJoinRoleDatas)
				{
					if (joinRolePkData.CurrGameID > 0 || joinRolePkData.ToServerID > 0)
					{
						ClientAgentManager.Instance().RemoveKfFuben(ZhanDuiZhengBa_K.GameType, joinRolePkData.ToServerID, (long)joinRolePkData.CurrGameID);
					}
					joinRolePkData.ToServerID = 0;
					joinRolePkData.CurrGameID = 0;
				}
				ZhanDuiZhengBa_K.StateMachine.SetCurrState(3, now);
			}
		}

		private static void MS_TodayPkEnd_Enter(DateTime now)
		{
			ZhanDuiZhengBa_K.FixSyncData(now);
		}

		private static void MS_TodayPkEnd_Update(DateTime now)
		{
			ZhanDuiZhengBaMatchConfig zhanDuiZhengBaMatchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == ZhanDuiZhengBa_K.SyncData.RealActID);
			if (now.TimeOfDay.Ticks >= zhanDuiZhengBaMatchConfig.DayEndTick)
			{
				ZhanDuiZhengBa_K.StateMachine.SetCurrState(1, now);
			}
		}

		public static void LoadSyncData(DateTime now, bool reload = false)
		{
			int selectRoleIfNewCreate = 64;
			lock (ZhanDuiZhengBa_K.Mutex)
			{
				ZhanDuiZhengBaSyncData syncData = ZhanDuiZhengBa_K.Persistence.LoadZhengBaSyncData(now, selectRoleIfNewCreate);
				ZhanDuiZhengBa_K.SyncData = syncData;
				ZhanDuiZhengBa_K.FixSyncData(now);
			}
		}

		private static bool FixSyncData_State(DateTime now)
		{
			bool result = false;
			int endID = 0;
			bool flag = false;
			try
			{
				object mutex;
				Monitor.Enter(mutex = ZhanDuiZhengBa_K.Mutex, ref flag);
				if (now.Day > ZhanDuiZhengBaConsts.StartMonthDay)
				{
					endID = ZhanDuiZhengBa_K._Config.MatchConfigList[ZhanDuiZhengBa_K._Config.MatchConfigList.Count - 1].ID;
				}
				else if (now.Day == ZhanDuiZhengBaConsts.StartMonthDay)
				{
					for (int i = 0; i < ZhanDuiZhengBa_K._Config.MatchConfigList.Count; i++)
					{
						ZhanDuiZhengBaMatchConfig zhanDuiZhengBaMatchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList[i];
						if (now.TimeOfDay.Ticks < zhanDuiZhengBaMatchConfig.DayBeginTick)
						{
							break;
						}
						if (now.TimeOfDay.Ticks >= zhanDuiZhengBaMatchConfig.ResultTick)
						{
							endID = zhanDuiZhengBaMatchConfig.ID;
						}
					}
				}
				if (endID == ZhanDuiZhengBaUtils.WhichDayResultByGrade(1))
				{
					ZhanDuiZhengBa_K.SyncData.HasSeasonEnd = true;
					ZhanDuiZhengBa_K.SyncData.TopZhanDui = ZhanDuiZhengBa_K.Persistence.GetLastTopZhanDui(ZhanDuiZhengBa_K.SyncData.Month);
				}
				else
				{
					ZhanDuiZhengBa_K.SyncData.HasSeasonEnd = false;
					ZhanDuiZhengBa_K.SyncData.TopZhanDui = ZhanDuiZhengBa_K.Persistence.GetLastTopZhanDui(ZhengBaUtils.MakeMonth(now.AddMonths(-1)));
				}
				int id;
				for (id = 1; id <= endID; id++)
				{
					ZhanDuiZhengBaMatchConfig zhanDuiZhengBaMatchConfig2 = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == id);
					EZhengBaGrade preGrade = zhanDuiZhengBaMatchConfig2.JoinGrade;
					EZhengBaGrade willUpGrade = zhanDuiZhengBaMatchConfig2.WillUpGrade;
					List<ZhanDuiZhengBaZhanDuiData> list = ZhanDuiZhengBa_K.SyncData.ZhanDuiList.FindAll((ZhanDuiZhengBaZhanDuiData _r) => _r.Grade > willUpGrade);
					if (list.Count > 0)
					{
						List<ZhanDuiZhengBaZhanDuiData> list2 = new List<ZhanDuiZhengBaZhanDuiData>();
						using (List<RangeKey>.Enumerator enumerator = ZhanDuiZhengBaUtils.GetDayPkGroupRange(id).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								RangeKey range = enumerator.Current;
								List<ZhanDuiZhengBaZhanDuiData> list3 = ZhanDuiZhengBa_K.SyncData.ZhanDuiList.FindAll((ZhanDuiZhengBaZhanDuiData _r) => _r.Group >= range.Left && _r.Group <= range.Right && _r.Grade <= preGrade);
								if (list3.Count != 0)
								{
									if (!list3.Exists((ZhanDuiZhengBaZhanDuiData _r) => _r.Grade <= willUpGrade))
									{
										list3.Sort(delegate(ZhanDuiZhengBaZhanDuiData _l, ZhanDuiZhengBaZhanDuiData _r)
										{
											int result2;
											if (_l.Grade != _r.Grade)
											{
												result2 = _l.Grade - _r.Grade;
											}
											else
											{
												result2 = _l.DuanWeiRank - _r.DuanWeiRank;
											}
											return result2;
										});
										ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData = list3[0];
										LogManager.WriteLog(2, string.Format("战队争霸::晋级补位 [s{0}.{1}] {2}->{3}", new object[]
										{
											zhanDuiZhengBaZhanDuiData.ZoneId,
											zhanDuiZhengBaZhanDuiData.ZhanDuiID,
											zhanDuiZhengBaZhanDuiData.Grade,
											willUpGrade
										}), null, true);
										zhanDuiZhengBaZhanDuiData.Grade = willUpGrade;
										result = true;
										list2.Add(zhanDuiZhengBaZhanDuiData);
										if (list3.Count >= 2)
										{
											ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData2 = list3[1];
											zhanDuiZhengBaZhanDuiData2.Grade = preGrade;
										}
									}
								}
							}
						}
					}
				}
				ZhanDuiZhengBa_K.SyncData.RealActID = endID;
				foreach (ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData3 in ZhanDuiZhengBa_K.SyncData.ZhanDuiList)
				{
					if (endID <= 0)
					{
						if (zhanDuiZhengBaZhanDuiData3.Grade != 64 || zhanDuiZhengBaZhanDuiData3.State != 0)
						{
							zhanDuiZhengBaZhanDuiData3.Grade = 64;
							zhanDuiZhengBaZhanDuiData3.State = 0;
							result = true;
						}
					}
					else
					{
						EZhengBaGrade willUpGrade2 = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == endID).WillUpGrade;
						if (zhanDuiZhengBaZhanDuiData3.Grade <= willUpGrade2 && zhanDuiZhengBaZhanDuiData3.State != 1)
						{
							zhanDuiZhengBaZhanDuiData3.State = 1;
							result = true;
						}
						if (zhanDuiZhengBaZhanDuiData3.Grade > willUpGrade2 && zhanDuiZhengBaZhanDuiData3.State != 2)
						{
							zhanDuiZhengBaZhanDuiData3.State = 2;
							result = true;
						}
						if (zhanDuiZhengBaZhanDuiData3.Grade == 1)
						{
							ZhanDuiZhengBa_K.SyncData.TopZhanDui = zhanDuiZhengBaZhanDuiData3.ZhanDuiID;
						}
					}
				}
			}
			finally
			{
				if (flag)
				{
					object mutex;
					Monitor.Exit(mutex);
				}
			}
			return result;
		}

		private static void FixSyncData(DateTime now)
		{
			lock (ZhanDuiZhengBa_K.Mutex)
			{
				bool flag2 = false;
				flag2 |= ZhanDuiZhengBa_K.FixSyncData_State(now);
				if (flag2)
				{
					foreach (ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData in ZhanDuiZhengBa_K.SyncData.ZhanDuiList)
					{
						ZhanDuiZhengBa_K.Persistence.UpdateRole(ZhanDuiZhengBa_K.SyncData.Month, zhanDuiZhengBaZhanDuiData.ZhanDuiID, zhanDuiZhengBaZhanDuiData.Grade, zhanDuiZhengBaZhanDuiData.State);
					}
				}
				ZhanDuiZhengBa_K.SyncData.RoleModTime = now;
			}
		}

		[Method]
		public static ZhanDuiZhengBaSyncData SyncZhengBaData(ZhanDuiZhengBaSyncData lastSyncData)
		{
			ZhanDuiZhengBaSyncData zhanDuiZhengBaSyncData = new ZhanDuiZhengBaSyncData();
			lock (ZhanDuiZhengBa_K.Mutex)
			{
				zhanDuiZhengBaSyncData.Month = ZhanDuiZhengBa_K.SyncData.Month;
				zhanDuiZhengBaSyncData.RealActID = ZhanDuiZhengBa_K.SyncData.RealActID;
				zhanDuiZhengBaSyncData.RoleModTime = ZhanDuiZhengBa_K.SyncData.RoleModTime;
				zhanDuiZhengBaSyncData.HasSeasonEnd = ZhanDuiZhengBa_K.SyncData.HasSeasonEnd;
				zhanDuiZhengBaSyncData.IsThisMonthInActivity = ZhanDuiZhengBa_K.SyncData.IsThisMonthInActivity;
				zhanDuiZhengBaSyncData.CenterTime = TimeUtil.NowDateTime();
				zhanDuiZhengBaSyncData.TopZhanDui = ZhanDuiZhengBa_K.SyncData.TopZhanDui;
				if (zhanDuiZhengBaSyncData.RoleModTime > lastSyncData.RoleModTime && ZhanDuiZhengBa_K.SyncData.ZhanDuiList != null)
				{
					zhanDuiZhengBaSyncData.ZhanDuiList = new List<ZhanDuiZhengBaZhanDuiData>(ZhanDuiZhengBa_K.SyncData.ZhanDuiList);
				}
				zhanDuiZhengBaSyncData.PKLogModTime = ZhanDuiZhengBa_K.SyncData.PKLogModTime;
				if (zhanDuiZhengBaSyncData.PKLogModTime > lastSyncData.PKLogModTime && ZhanDuiZhengBa_K.SyncData.PKLogList != null)
				{
					zhanDuiZhengBaSyncData.PKLogList = new List<ZhanDuiZhengBaPkLogData>(ZhanDuiZhengBa_K.SyncData.PKLogList);
				}
			}
			return zhanDuiZhengBaSyncData;
		}

		[Method]
		public static int ZhengBaKuaFuLogin(ServerSocketSender socket, int zhanDuiID, int gameId, int srcServerID, out ZhanDuiZhengBaFuBenData copyData)
		{
			copyData = null;
			lock (ZhanDuiZhengBa_K.Mutex)
			{
				ZhanDuiZhengBa_K.JoinRolePkData joinRolePkData = ZhanDuiZhengBa_K.TodayJoinRoleDatas.Find((ZhanDuiZhengBa_K.JoinRolePkData _r) => _r.ZhanDuiID == zhanDuiID && _r.CurrGameID == gameId);
				ZhanDuiZhengBaPkLogData zhanDuiZhengBaPkLogData = null;
				ZhanDuiZhengBa_K.ThisLoopPkLogs.TryGetValue(gameId, out zhanDuiZhengBaPkLogData);
				if (joinRolePkData == null || zhanDuiZhengBaPkLogData == null)
				{
					return -12;
				}
				if (!joinRolePkData.WaitReqEnter)
				{
					return -12;
				}
				copyData = joinRolePkData.CopyData;
			}
			KuaFuServerInfo kuaFuServerInfo = KuaFuServerManager.GetKuaFuServerInfo(srcServerID);
			int result;
			if (null != kuaFuServerInfo)
			{
				copyData.IPs = new string[]
				{
					kuaFuServerInfo.DbIp,
					kuaFuServerInfo.DbIp
				};
				copyData.Ports = new int[]
				{
					kuaFuServerInfo.DbPort,
					kuaFuServerInfo.LogDbPort
				};
				result = 0;
			}
			else
			{
				result = -11000;
			}
			return result;
		}

		[Method]
		public static int ZhengBaRequestEnter(int zhanDuiID, out int gameId, out int kuaFuServerID, out string[] ips, out int[] ports)
		{
			gameId = 0;
			kuaFuServerID = 0;
			ips = null;
			ports = null;
			lock (ZhanDuiZhengBa_K.Mutex)
			{
				if (ZhanDuiZhengBa_K.StateMachine.GetCurrState() != 5)
				{
					if (!Consts.TestMode || ZhanDuiZhengBa_K.StateMachine.GetCurrState() != 6)
					{
						return -2001;
					}
				}
				ZhanDuiZhengBa_K.JoinRolePkData joinRolePkData = ZhanDuiZhengBa_K.TodayJoinRoleDatas.Find((ZhanDuiZhengBa_K.JoinRolePkData _r) => _r.ZhanDuiID == zhanDuiID);
				if (joinRolePkData == null || joinRolePkData.CurrGameID == 0)
				{
					return -4006;
				}
				ZhanDuiZhengBaPkLogData zhanDuiZhengBaPkLogData = null;
				ZhanDuiZhengBa_K.ThisLoopPkLogs.TryGetValue(joinRolePkData.CurrGameID, out zhanDuiZhengBaPkLogData);
				if (joinRolePkData == null || zhanDuiZhengBaPkLogData == null)
				{
					return -4006;
				}
				if (!joinRolePkData.WaitReqEnter)
				{
					return -2001;
				}
				gameId = joinRolePkData.CurrGameID;
				kuaFuServerID = joinRolePkData.ToServerID;
			}
			KuaFuServerInfo kuaFuServerInfo = KuaFuServerManager.GetKuaFuServerInfo(kuaFuServerID);
			int result;
			if (null != kuaFuServerInfo)
			{
				ips = new string[]
				{
					kuaFuServerInfo.Ip
				};
				ports = new int[]
				{
					kuaFuServerInfo.Port
				};
				result = 0;
			}
			else
			{
				result = -11001;
			}
			return result;
		}

		[Method]
		public static List<ZhanDuiZhengBaNtfPkResultData> ZhengBaPkResult(int gameId, int winner1)
		{
			bool flag = false;
			List<ZhanDuiZhengBaNtfPkResultData> result;
			try
			{
				object mutex;
				Monitor.Enter(mutex = ZhanDuiZhengBa_K.Mutex, ref flag);
				LogManager.WriteLog(1002, string.Format("ZhanDuiZhengBa::ZhengBaPkResult,gameid={0},winner1={1}", gameId, winner1), null, true);
				ZhanDuiZhengBaPkLogData log = null;
				if (!ZhanDuiZhengBa_K.ThisLoopPkLogs.TryGetValue(gameId, out log))
				{
					result = null;
				}
				else if (winner1 != log.ZhanDuiID1 && winner1 != log.ZhanDuiID2)
				{
					result = null;
				}
				else
				{
					ZhanDuiZhengBaMatchConfig zhanDuiZhengBaMatchConfig = ZhanDuiZhengBa_K._Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig _m) => _m.ID == log.ID);
					ZhanDuiZhengBa_K.JoinRolePkData winJoinRole = ZhanDuiZhengBa_K.TodayJoinRoleDatas.Find((ZhanDuiZhengBa_K.JoinRolePkData _r) => _r.ZhanDuiID == winner1 && _r.CurrGameID == gameId);
					ZhanDuiZhengBa_K.JoinRolePkData faildJoinRole = ZhanDuiZhengBa_K.TodayJoinRoleDatas.Find((ZhanDuiZhengBa_K.JoinRolePkData _r) => _r.ZhanDuiID != winner1 && _r.CurrGameID == gameId);
					if (faildJoinRole != null && winJoinRole != null && faildJoinRole.ZhanDuiID != winJoinRole.ZhanDuiID)
					{
						ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData = ZhanDuiZhengBa_K.SyncData.ZhanDuiList.Find((ZhanDuiZhengBaZhanDuiData _r) => _r.ZhanDuiID == faildJoinRole.ZhanDuiID);
						if (zhanDuiZhengBaZhanDuiData != null)
						{
							int num = 2;
							LogManager.WriteLog(1002, string.Format("ZhanDuiZhengBa::ZhengBaPkResult,gameid={0},zhanduiid={1},newstate={2}", gameId, zhanDuiZhengBaZhanDuiData.ZhanDuiID, num), null, true);
							if (ZhanDuiZhengBa_K.Persistence.UpdateRole(ZhanDuiZhengBa_K.SyncData.Month, zhanDuiZhengBaZhanDuiData.ZhanDuiID, zhanDuiZhengBaZhanDuiData.Grade, num))
							{
								zhanDuiZhengBaZhanDuiData.State = num;
								ZhanDuiZhengBa_K.SyncData.RoleModTime = TimeUtil.NowDateTime();
							}
						}
						ZhanDuiZhengBa_K.TodayJoinRoleDatas.RemoveAll((ZhanDuiZhengBa_K.JoinRolePkData _r) => _r.ZhanDuiID == faildJoinRole.ZhanDuiID);
					}
					if (winJoinRole != null)
					{
						bool flag2 = false;
						ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData = ZhanDuiZhengBa_K.SyncData.ZhanDuiList.Find((ZhanDuiZhengBaZhanDuiData _r) => _r.ZhanDuiID == winJoinRole.ZhanDuiID);
						if (zhanDuiZhengBaZhanDuiData != null)
						{
							int willUpGrade = zhanDuiZhengBaMatchConfig.WillUpGrade;
							int num = 1;
							LogManager.WriteLog(1002, string.Format("ZhanDuiZhengBa::ZhengBaPkResult,gameid={0},zhanduiid={1},newstate={2}", gameId, zhanDuiZhengBaZhanDuiData.ZhanDuiID, num), null, true);
							if (ZhanDuiZhengBa_K.Persistence.UpdateRole(ZhanDuiZhengBa_K.SyncData.Month, zhanDuiZhengBaZhanDuiData.ZhanDuiID, willUpGrade, num))
							{
								zhanDuiZhengBaZhanDuiData.Grade = willUpGrade;
								zhanDuiZhengBaZhanDuiData.State = num;
								ZhanDuiZhengBa_K.SyncData.RoleModTime = TimeUtil.NowDateTime();
								flag2 = true;
							}
						}
						if (flag2)
						{
							log.UpGrade = true;
							ZhanDuiZhengBa_K.TodayJoinRoleDatas.RemoveAll((ZhanDuiZhengBa_K.JoinRolePkData _r) => _r.ZhanDuiID == winJoinRole.ZhanDuiID);
						}
					}
					log.EndTime = TimeUtil.NowDateTime();
					if (winner1 > 0 && winner1 == log.ZhanDuiID1)
					{
						log.PkResult = 1;
					}
					else if (winner1 > 0 && winner1 == log.ZhanDuiID2)
					{
						log.PkResult = 2;
					}
					else
					{
						log.PkResult = 0;
					}
					ZhanDuiZhengBa_K.SyncData.PKLogList.Add(log);
					TimeUtil.AgeByDateTime(ref ZhanDuiZhengBa_K.SyncData.PKLogModTime);
					ZhanDuiZhengBa_K.Persistence.SavePkLog(log);
					ZhanDuiZhengBa_K.ThisLoopPkLogs.Remove(gameId);
					result = null;
				}
			}
			finally
			{
				if (flag)
				{
					object mutex;
					Monitor.Exit(mutex);
				}
			}
			return result;
		}

		private static object Mutex = new object();

		private static bool Initialize = false;

		private static GameTypes GameType = 35;

		private static StateMachineSimple StateMachine = new StateMachineSimple(8);

		private static ZhanDuiZhengBaSyncData SyncData = new ZhanDuiZhengBaSyncData
		{
			Month = ZhanDuiZhengBaUtils.MakeMonth(TimeUtil.NowDateTime())
		};

		private static DateTime lastUpdateTime = TimeUtil.NowDateTime();

		private static Queue<KFCallMsg> AsyncEvQ = new Queue<KFCallMsg>();

		private static ZhanDuiZhengBaConfig _Config = new ZhanDuiZhengBaConfig();

		private static List<ZhanDuiZhengBa_K.JoinRolePkData> TodayJoinRoleDatas = new List<ZhanDuiZhengBa_K.JoinRolePkData>();

		private static Dictionary<int, ZhanDuiZhengBaPkLogData> ThisLoopPkLogs = new Dictionary<int, ZhanDuiZhengBaPkLogData>();

		private static ZhanDuiZhengBaPersistence Persistence = ZhanDuiZhengBaPersistence.Instance;

		private class JoinRolePkData
		{
			public int ZhanDuiID;

			public int ZoneId;

			public string RoleName;

			public int Group;

			public int Rank;

			public int ToServerID;

			public int CurrGameID;

			public bool WaitReqEnter;

			public ZhanDuiZhengBaFuBenData CopyData;
		}

		public class StateType
		{
			public const int None = 0;

			public const int Idle = 1;

			public const int Init = 2;

			public const int TodayPkEnd = 3;

			public const int InitPkLoop = 4;

			public const int NotifyEnter = 5;

			public const int PkLoopStart = 6;

			public const int PkLoopEnd = 7;

			public const int Max = 8;
		}

		internal static class TcpStaticServer
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static ZhanDuiZhengBaSyncData _M16(ZhanDuiZhengBaSyncData lastSyncData)
			{
				return ZhanDuiZhengBa_K.SyncZhengBaData(lastSyncData);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M17(ServerSocketSender _sender_, int zhanDuiID, int gameId, int srcServerID, out ZhanDuiZhengBaFuBenData copyData)
			{
				return ZhanDuiZhengBa_K.ZhengBaKuaFuLogin(_sender_, zhanDuiID, gameId, srcServerID, out copyData);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static List<ZhanDuiZhengBaNtfPkResultData> _M18(int gameId, int winner1)
			{
				return ZhanDuiZhengBa_K.ZhengBaPkResult(gameId, winner1);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M19(int zhanDuiID, out int gameId, out int kuaFuServerID, out string[] ips, out int[] ports)
			{
				return ZhanDuiZhengBa_K.ZhengBaRequestEnter(zhanDuiID, out gameId, out kuaFuServerID, out ips, out ports);
			}
		}
	}
}
