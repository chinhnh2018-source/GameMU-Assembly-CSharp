using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Server.Tools;
using Tmsk.Contract;

namespace KF.Remoting
{
	internal class ZhengBaManagerK
	{
		private ZhengBaManagerK()
		{
			this.StateMachine.Install(new ZhengBaStateMachine.StateHandler(ZhengBaStateMachine.StateType.Idle, null, new Action<DateTime>(this.MS_Idle_Update), null));
			this.StateMachine.Install(new ZhengBaStateMachine.StateHandler(ZhengBaStateMachine.StateType.TodayPkStart, new Action<DateTime>(this.MS_TodayPkStart_Enter), null, null));
			this.StateMachine.Install(new ZhengBaStateMachine.StateHandler(ZhengBaStateMachine.StateType.InitPkLoop, new Action<DateTime>(this.MS_InitPkLoop_Enter), null, null));
			this.StateMachine.Install(new ZhengBaStateMachine.StateHandler(ZhengBaStateMachine.StateType.NotifyEnter, null, new Action<DateTime>(this.MS_NotifyEnter_Update), null));
			this.StateMachine.Install(new ZhengBaStateMachine.StateHandler(ZhengBaStateMachine.StateType.PkLoopStart, null, new Action<DateTime>(this.MS_PkLoopStart_Update), null));
			this.StateMachine.Install(new ZhengBaStateMachine.StateHandler(ZhengBaStateMachine.StateType.PkLoopEnd, null, new Action<DateTime>(this.MS_PkLoopEnd_Update), null));
			this.StateMachine.Install(new ZhengBaStateMachine.StateHandler(ZhengBaStateMachine.StateType.TodayPkEnd, new Action<DateTime>(this.MS_TodayPkEnd_Enter), null, null));
			this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.Idle, TimeUtil.NowDateTime());
			this.Persistence.MonthRankFirstCreate = delegate(int selectRoleIfNewCreate)
			{
				lock (this.Mutex)
				{
					this.AsyncEvQ.Enqueue(new AsyncDataItem(10013, new object[]
					{
						new ZhengBaBulletinJoinData
						{
							NtfType = 1,
							Args1 = selectRoleIfNewCreate
						}
					}));
					this.AsyncEvQ.Enqueue(new AsyncDataItem(10013, new object[]
					{
						new ZhengBaBulletinJoinData
						{
							NtfType = 2
						}
					}));
				}
			};
		}

		public static ZhengBaManagerK Instance()
		{
			return ZhengBaManagerK._instance;
		}

		public AsyncDataItem[] Update()
		{
			DateTime now = TimeUtil.NowDateTime();
			if (now.Month != this.lastUpdateTime.Month)
			{
				this.ReloadSyncData(now);
			}
			else if (now.Day != this.lastUpdateTime.Day)
			{
				this.FixSyncData(now);
			}
			else
			{
				lock (this.Mutex)
				{
					this.StateMachine.Tick(now);
				}
			}
			AsyncDataItem[] result = null;
			lock (this.Mutex)
			{
				result = this.AsyncEvQ.ToArray();
				this.AsyncEvQ.Clear();
			}
			this.lastUpdateTime = now;
			return result;
		}

		private void MS_Idle_Update(DateTime now)
		{
			if (this.SyncData.RealActDay >= 1 && this.SyncData.RealActDay <= 7)
			{
				ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
				if (this.lastUpdateTime.TimeOfDay.Ticks < zhengBaMatchConfig.DayBeginTick && now.TimeOfDay.Ticks >= zhengBaMatchConfig.DayBeginTick)
				{
					this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.TodayPkStart, now);
				}
				else if (this.lastUpdateTime.TimeOfDay.Ticks < zhengBaMatchConfig.DayEndTick && now.TimeOfDay.Ticks >= zhengBaMatchConfig.DayEndTick)
				{
					this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.TodayPkEnd, now);
				}
			}
		}

		private void MS_TodayPkStart_Enter(DateTime now)
		{
			this.SyncData.TodayIsPking = true;
			this.TodayJoinRoleDatas.Clear();
			this.CurrLoopIndex = 0;
			this.HadUpGradeRoleNum = 0;
			foreach (ZhengBaRoleInfoData zhengBaRoleInfoData in this.SyncData.RoleList)
			{
				if (zhengBaRoleInfoData.State == 0 || zhengBaRoleInfoData.State == 1)
				{
					this.TodayJoinRoleDatas.Add(new ZhengBaManagerK.JoinRolePkData
					{
						RoleID = zhengBaRoleInfoData.RoleId,
						ZoneId = zhengBaRoleInfoData.ZoneId,
						RoleName = zhengBaRoleInfoData.RoleName,
						Group = zhengBaRoleInfoData.Group
					});
				}
			}
			if (this.SyncData.RealActDay == 3)
			{
				this.RandomGroup.Clear();
				this.RandomGroup.AddRange(Enumerable.Range(1, 16));
				Random random = new Random((int)now.Ticks);
				int num = 0;
				while (this.TodayJoinRoleDatas.Count >= 16 && num < 50)
				{
					int index = random.Next(0, this.RandomGroup.Count);
					int index2 = random.Next(0, this.RandomGroup.Count);
					int value = this.RandomGroup[index];
					this.RandomGroup[index] = this.RandomGroup[index2];
					this.RandomGroup[index2] = value;
					num++;
				}
			}
			this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.InitPkLoop, now);
		}

		private void MS_InitPkLoop_Enter(DateTime now)
		{
			this.ThisLoopPkLogs.Clear();
			this.CurrLoopIndex++;
			ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
			if (this.HadUpGradeRoleNum >= zhengBaMatchConfig.MaxUpGradeNum || this.TodayJoinRoleDatas.Count <= 1)
			{
				this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.TodayPkEnd, now);
			}
			else
			{
				if (zhengBaMatchConfig.Mathching == 1)
				{
					Random random = new Random((int)now.Ticks);
					int i = 0;
					while (this.TodayJoinRoleDatas.Count > 0 && i < this.TodayJoinRoleDatas.Count * 2)
					{
						int index = random.Next(0, this.TodayJoinRoleDatas.Count);
						int index2 = random.Next(0, this.TodayJoinRoleDatas.Count);
						ZhengBaManagerK.JoinRolePkData value = this.TodayJoinRoleDatas[index];
						this.TodayJoinRoleDatas[index] = this.TodayJoinRoleDatas[index2];
						this.TodayJoinRoleDatas[index2] = value;
						i++;
					}
				}
				else if (zhengBaMatchConfig.Mathching == 2)
				{
					List<ZhengBaManagerK.JoinRolePkData> list = new List<ZhengBaManagerK.JoinRolePkData>();
					using (List<RangeKey>.Enumerator enumerator = ZhengBaUtils.GetDayPkGroupRange(this.SyncData.RealActDay).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							RangeKey range = enumerator.Current;
							List<ZhengBaManagerK.JoinRolePkData> list2 = this.TodayJoinRoleDatas.FindAll((ZhengBaManagerK.JoinRolePkData _r) => _r.Group >= range.Left && _r.Group <= range.Right);
							if (list2 != null && list2.Count == 2)
							{
								list.AddRange(list2);
							}
						}
					}
					this.TodayJoinRoleDatas.Clear();
					this.TodayJoinRoleDatas.AddRange(list);
				}
				else
				{
					Debug.Assert(false, "unknown pk match type");
				}
				int j = 0;
				for (int i = 0; i < this.TodayJoinRoleDatas.Count / 2; i++)
				{
					ZhengBaManagerK.JoinRolePkData joinRolePkData = this.TodayJoinRoleDatas[j++];
					ZhengBaManagerK.JoinRolePkData joinRolePkData2 = this.TodayJoinRoleDatas[j++];
					int num = 0;
					int nextGameId = TianTiPersistence.Instance.GetNextGameId();
					if (ClientAgentManager.Instance().AssginKfFuben(TianTiService.Instance.GameType, (long)nextGameId, 2, out num))
					{
						joinRolePkData.ToServerID = (joinRolePkData2.ToServerID = num);
						joinRolePkData.CurrGameID = (joinRolePkData2.CurrGameID = nextGameId);
						joinRolePkData.WaitReqEnter = (joinRolePkData2.WaitReqEnter = true);
						joinRolePkData.WaitKuaFuLogin = (joinRolePkData2.WaitKuaFuLogin = false);
						ZhengBaNtfEnterData zhengBaNtfEnterData = new ZhengBaNtfEnterData();
						zhengBaNtfEnterData.RoleId1 = joinRolePkData.RoleID;
						zhengBaNtfEnterData.RoleId2 = joinRolePkData2.RoleID;
						zhengBaNtfEnterData.ToServerId = num;
						zhengBaNtfEnterData.GameId = nextGameId;
						zhengBaNtfEnterData.Day = this.SyncData.RealActDay;
						zhengBaNtfEnterData.Loop = this.CurrLoopIndex;
						this.AsyncEvQ.Enqueue(new AsyncDataItem(10011, new object[]
						{
							zhengBaNtfEnterData
						}));
						ZhengBaPkLogData zhengBaPkLogData = new ZhengBaPkLogData();
						zhengBaPkLogData.Month = this.SyncData.Month;
						zhengBaPkLogData.Day = this.SyncData.RealActDay;
						zhengBaPkLogData.RoleID1 = joinRolePkData.RoleID;
						zhengBaPkLogData.ZoneID1 = joinRolePkData.ZoneId;
						zhengBaPkLogData.RoleName1 = joinRolePkData.RoleName;
						zhengBaPkLogData.RoleID2 = joinRolePkData2.RoleID;
						zhengBaPkLogData.ZoneID2 = joinRolePkData2.ZoneId;
						zhengBaPkLogData.RoleName2 = joinRolePkData2.RoleName;
						zhengBaPkLogData.StartTime = now;
						this.ThisLoopPkLogs[nextGameId] = zhengBaPkLogData;
					}
					else
					{
						LogManager.WriteLog(2, string.Format("众神争霸第{0}天第{1}轮分配游戏服务器失败，role1={2},role2={3}", new object[]
						{
							this.SyncData.RealActDay,
							this.CurrLoopIndex,
							joinRolePkData.RoleID,
							joinRolePkData2.RoleID
						}), null, true);
					}
				}
				while (j < this.TodayJoinRoleDatas.Count)
				{
					ZhengBaManagerK.JoinRolePkData joinRolePkData3 = this.TodayJoinRoleDatas[j++];
					joinRolePkData3.ToServerID = 0;
					joinRolePkData3.CurrGameID = 0;
					joinRolePkData3.WaitReqEnter = false;
					joinRolePkData3.WaitKuaFuLogin = false;
				}
				this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.NotifyEnter, now);
			}
		}

		private void MS_NotifyEnter_Update(DateTime now)
		{
			ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
			if (this.StateMachine.ContinueTicks(now) >= (long)zhengBaMatchConfig.WaitSeconds * 10000000L)
			{
				int i = 0;
				int num = 0;
				while (i < this.TodayJoinRoleDatas.Count / 2)
				{
					ZhengBaManagerK.JoinRolePkData joinRolePkData = this.TodayJoinRoleDatas[num++];
					if (joinRolePkData.WaitReqEnter)
					{
						joinRolePkData.WaitReqEnter = false;
						joinRolePkData.WaitKuaFuLogin = false;
						ZhengBaPkLogData zhengBaPkLogData = null;
						if (this.ThisLoopPkLogs.TryGetValue(joinRolePkData.CurrGameID, out zhengBaPkLogData))
						{
							zhengBaPkLogData.IsMirror1 = true;
							this.AsyncEvQ.Enqueue(new AsyncDataItem(10012, new object[]
							{
								new ZhengBaMirrorFightData
								{
									GameId = joinRolePkData.CurrGameID,
									RoleId = joinRolePkData.RoleID,
									ToServerId = joinRolePkData.ToServerID
								}
							}));
						}
					}
					ZhengBaManagerK.JoinRolePkData joinRolePkData2 = this.TodayJoinRoleDatas[num++];
					if (joinRolePkData2.WaitReqEnter)
					{
						joinRolePkData2.WaitReqEnter = false;
						joinRolePkData2.WaitKuaFuLogin = false;
						ZhengBaPkLogData zhengBaPkLogData = null;
						if (this.ThisLoopPkLogs.TryGetValue(joinRolePkData2.CurrGameID, out zhengBaPkLogData))
						{
							zhengBaPkLogData.IsMirror2 = true;
							this.AsyncEvQ.Enqueue(new AsyncDataItem(10012, new object[]
							{
								new ZhengBaMirrorFightData
								{
									GameId = joinRolePkData2.CurrGameID,
									RoleId = joinRolePkData2.RoleID,
									ToServerId = joinRolePkData2.ToServerID
								}
							}));
						}
					}
					i++;
				}
				this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.PkLoopStart, now);
			}
		}

		private void MS_PkLoopStart_Update(DateTime now)
		{
			ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
			if (this.StateMachine.ContinueTicks(now) >= (long)(zhengBaMatchConfig.FightSeconds + zhengBaMatchConfig.ClearSeconds) * 10000000L)
			{
				this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.PkLoopEnd, now);
			}
		}

		private void MS_PkLoopEnd_Update(DateTime now)
		{
			ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
			if (this.StateMachine.ContinueTicks(now) >= (long)zhengBaMatchConfig.IntervalSeconds * 10000000L)
			{
				foreach (KeyValuePair<int, ZhengBaPkLogData> keyValuePair in this.ThisLoopPkLogs)
				{
					keyValuePair.Value.PkResult = 0;
					keyValuePair.Value.UpGrade = false;
					keyValuePair.Value.EndTime = now;
					this.Persistence.SavePkLog(keyValuePair.Value);
					this.AsyncEvQ.Enqueue(new AsyncDataItem(10010, new object[]
					{
						keyValuePair.Value
					}));
				}
				this.ThisLoopPkLogs.Clear();
				foreach (ZhengBaManagerK.JoinRolePkData joinRolePkData in this.TodayJoinRoleDatas)
				{
					if (joinRolePkData.CurrGameID > 0 || joinRolePkData.ToServerID > 0)
					{
						ClientAgentManager.Instance().RemoveKfFuben(TianTiService.Instance.GameType, joinRolePkData.ToServerID, (long)joinRolePkData.CurrGameID);
					}
					joinRolePkData.ToServerID = 0;
					joinRolePkData.CurrGameID = 0;
				}
				if (now.TimeOfDay.Ticks >= zhengBaMatchConfig.DayEndTick)
				{
					this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.TodayPkEnd, now);
				}
				else
				{
					this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.InitPkLoop, now);
				}
			}
		}

		private void MS_TodayPkEnd_Enter(DateTime now)
		{
			this.SyncData.TodayIsPking = false;
			this.FixSyncData(now);
			this.StateMachine.SetCurrState(ZhengBaStateMachine.StateType.Idle, now);
			this.AsyncEvQ.Enqueue(new AsyncDataItem(10013, new object[]
			{
				new ZhengBaBulletinJoinData
				{
					NtfType = 4,
					Args1 = this.SyncData.RealActDay
				}
			}));
		}

		public void InitConfig()
		{
			if (!this._Config.Load(KuaFuServerManager.GetResourcePath("Config\\Match.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\Sustain.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\MatchBirthPoint.xml", KuaFuServerManager.ResourcePathTypes.GameRes)))
			{
				LogManager.WriteLog(2, "ZhengBaManagerK.InitConfig failed!", null, true);
			}
		}

		public void ReloadSyncData(DateTime now)
		{
			int selectRoleIfNewCreate = 100;
			ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1);
			if (zhengBaMatchConfig.MinRank > 0)
			{
				selectRoleIfNewCreate = zhengBaMatchConfig.MinRank;
			}
			long dayBeginTick = zhengBaMatchConfig.DayBeginTick;
			ZhengBaSyncData syncData = this.Persistence.LoadZhengBaSyncData(now, selectRoleIfNewCreate, dayBeginTick);
			lock (this.Mutex)
			{
				this.SyncData = syncData;
				this.FixSyncData(now);
			}
		}

		private bool FixSyncData_State(DateTime now)
		{
			bool result = false;
			int num = now.Day - ZhengBaConsts.StartMonthDay + 1;
			bool flag = false;
			try
			{
				object mutex;
				Monitor.Enter(mutex = this.Mutex, ref flag);
				int rankOfDay;
				for (rankOfDay = 7; rankOfDay >= 1; rankOfDay--)
				{
					EZhengBaGrade willUpGrade = ZhengBaUtils.GetDayUpGrade(rankOfDay);
					ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == rankOfDay);
					List<ZhengBaRoleInfoData> list = this.SyncData.RoleList.FindAll((ZhengBaRoleInfoData _r) => _r.Grade == willUpGrade);
					if (list.Count > 0)
					{
						int num2 = zhengBaMatchConfig.MaxUpGradeNum - list.Count;
						if (num2 > 0)
						{
							List<ZhengBaRoleInfoData> list2 = new List<ZhengBaRoleInfoData>();
							if (rankOfDay <= 3)
							{
								List<ZhengBaRoleInfoData> list3 = this.SyncData.RoleList.FindAll((ZhengBaRoleInfoData _r) => _r.Grade > willUpGrade);
								list3.Sort(delegate(ZhengBaRoleInfoData _l, ZhengBaRoleInfoData _r)
								{
									int result2;
									if (_l.Grade < _r.Grade)
									{
										result2 = -1;
									}
									else if (_l.Grade > _r.Grade)
									{
										result2 = 1;
									}
									else
									{
										result2 = _l.DuanWeiRank - _r.DuanWeiRank;
									}
									return result2;
								});
								foreach (ZhengBaRoleInfoData zhengBaRoleInfoData in list3.GetRange(0, Math.Min(num2, list3.Count)))
								{
									list2.Add(zhengBaRoleInfoData);
									LogManager.WriteLog(2, string.Format("晋级补位 [s{0}.{1}] {2}->{3}", new object[]
									{
										zhengBaRoleInfoData.ZoneId,
										zhengBaRoleInfoData.RoleId,
										zhengBaRoleInfoData.Grade,
										willUpGrade
									}), null, true);
									zhengBaRoleInfoData.Grade = willUpGrade;
									result = true;
								}
							}
							else
							{
								using (List<RangeKey>.Enumerator enumerator2 = ZhengBaUtils.GetDayPkGroupRange(rankOfDay).GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										RangeKey range = enumerator2.Current;
										List<ZhengBaRoleInfoData> list4 = this.SyncData.RoleList.FindAll((ZhengBaRoleInfoData _r) => _r.Group >= range.Left && _r.Group <= range.Right);
										if (!list4.Exists((ZhengBaRoleInfoData _r) => _r.Grade <= ZhengBaUtils.GetDayUpGrade(rankOfDay)))
										{
											list4.RemoveAll((ZhengBaRoleInfoData _r) => _r.Grade != ZhengBaUtils.GetDayUpGrade(rankOfDay - 1));
											if (list4.Count > 0)
											{
												list4.Sort((ZhengBaRoleInfoData _l, ZhengBaRoleInfoData _r) => _l.DuanWeiRank - _r.DuanWeiRank);
												ZhengBaRoleInfoData zhengBaRoleInfoData2 = list4[0];
												LogManager.WriteLog(2, string.Format("晋级补位 [s{0}.{1}] {2}->{3}", new object[]
												{
													zhengBaRoleInfoData2.ZoneId,
													zhengBaRoleInfoData2.RoleId,
													zhengBaRoleInfoData2.Grade,
													willUpGrade
												}), null, true);
												zhengBaRoleInfoData2.Grade = ZhengBaUtils.GetDayUpGrade(rankOfDay);
												result = true;
												list2.Add(zhengBaRoleInfoData2);
											}
										}
									}
								}
							}
							foreach (ZhengBaRoleInfoData zhengBaRoleInfoData in list2)
							{
								this.AsyncEvQ.Enqueue(new AsyncDataItem(10013, new object[]
								{
									new ZhengBaBulletinJoinData
									{
										NtfType = 3,
										Args1 = zhengBaRoleInfoData.RoleId
									}
								}));
							}
						}
						break;
					}
				}
				this.SyncData.RankResultOfDay = rankOfDay;
				this.SyncData.RealActDay = rankOfDay;
				foreach (ZhengBaRoleInfoData zhengBaRoleInfoData3 in this.SyncData.RoleList)
				{
					if (rankOfDay <= 0)
					{
						if (zhengBaRoleInfoData3.Grade != 100 || zhengBaRoleInfoData3.State != 0 || zhengBaRoleInfoData3.Group != 0)
						{
							zhengBaRoleInfoData3.Grade = 100;
							zhengBaRoleInfoData3.State = 0;
							zhengBaRoleInfoData3.Group = 0;
							result = true;
						}
					}
					else
					{
						EZhengBaGrade willUpGrade2 = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == rankOfDay).WillUpGrade;
						if (zhengBaRoleInfoData3.Grade <= willUpGrade2 && zhengBaRoleInfoData3.State != 1)
						{
							zhengBaRoleInfoData3.State = 1;
							result = true;
						}
						if (zhengBaRoleInfoData3.Grade > willUpGrade2 && zhengBaRoleInfoData3.State != 2)
						{
							zhengBaRoleInfoData3.State = 2;
							result = true;
						}
					}
					if (zhengBaRoleInfoData3.Grade == 1 && this.SyncData.LastKingModTime != this.SyncData.Month)
					{
						this.SyncData.LastKingData = zhengBaRoleInfoData3;
						this.SyncData.LastKingModTime = this.SyncData.Month;
					}
				}
				if (num > 0 && this.SyncData.RealActDay < num)
				{
					if (this.SyncData.RealActDay < 7)
					{
						ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay + 1);
						if (now.TimeOfDay.Ticks < zhengBaMatchConfig.DayBeginTick)
						{
							this.SyncData.RealActDay++;
						}
					}
					else
					{
						this.SyncData.RealActDay = 8;
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

		private bool FixSyncData_Group(DateTime now)
		{
			bool result = false;
			lock (this.Mutex)
			{
				if (this.SyncData.RealActDay < 3)
				{
					return false;
				}
				List<int> list = Enumerable.Range(1, 16).ToList<int>();
				List<ZhengBaRoleInfoData> list2 = new List<ZhengBaRoleInfoData>();
				foreach (ZhengBaRoleInfoData zhengBaRoleInfoData in this.SyncData.RoleList)
				{
					if (zhengBaRoleInfoData.Grade <= 16)
					{
						if (zhengBaRoleInfoData.Group >= 1 && zhengBaRoleInfoData.Group <= 16)
						{
							list.Remove(zhengBaRoleInfoData.Group);
						}
						else
						{
							zhengBaRoleInfoData.Group = 0;
							list2.Add(zhengBaRoleInfoData);
						}
					}
				}
				if (list2.Count <= list.Count && list2.Count > 0)
				{
					list2.Sort((ZhengBaRoleInfoData _l, ZhengBaRoleInfoData _r) => _l.ZoneId * _l.DuanWeiRank * now.TimeOfDay.Minutes % _r.RoleId - _r.ZoneId * _r.DuanWeiRank * now.TimeOfDay.Minutes % _l.RoleId);
					foreach (ZhengBaRoleInfoData zhengBaRoleInfoData in list2)
					{
						zhengBaRoleInfoData.Group = list.Last<int>();
						list.RemoveAt(list.Count<int>() - 1);
						LogManager.WriteLog(2, string.Format("晋级补分组 [s{0}.{1}] Group:{2}", zhengBaRoleInfoData.ZoneId, zhengBaRoleInfoData.RoleId, zhengBaRoleInfoData.Group), null, true);
						result = true;
					}
				}
				else if (list2.Count > 0)
				{
					LogManager.WriteLog(1000, string.Format("晋级补分组发生异常，待补人数={0}，可用分组数={1}", list2.Count, list.Count), null, false);
				}
			}
			return result;
		}

		private void FixSyncData(DateTime now)
		{
			lock (this.Mutex)
			{
				bool flag2 = false;
				flag2 |= this.FixSyncData_State(now);
				flag2 |= this.FixSyncData_Group(now);
				if (flag2)
				{
					foreach (ZhengBaRoleInfoData zhengBaRoleInfoData in this.SyncData.RoleList)
					{
						this.Persistence.UpdateRole(this.SyncData.Month, zhengBaRoleInfoData.RoleId, zhengBaRoleInfoData.Grade, zhengBaRoleInfoData.State, zhengBaRoleInfoData.Group);
					}
				}
				this.SyncData.RoleModTime = now;
				this.SyncData.SupportModTime = now;
			}
		}

		public ZhengBaSyncData SyncZhengBaData(ZhengBaSyncData lastSyncData)
		{
			ZhengBaSyncData zhengBaSyncData = new ZhengBaSyncData();
			lock (this.Mutex)
			{
				zhengBaSyncData.Month = this.SyncData.Month;
				zhengBaSyncData.RealActDay = this.SyncData.RealActDay;
				zhengBaSyncData.RoleModTime = this.SyncData.RoleModTime;
				zhengBaSyncData.SupportModTime = this.SyncData.SupportModTime;
				zhengBaSyncData.LastKingModTime = this.SyncData.LastKingModTime;
				zhengBaSyncData.TodayIsPking = this.SyncData.TodayIsPking;
				zhengBaSyncData.IsThisMonthInActivity = this.SyncData.IsThisMonthInActivity;
				zhengBaSyncData.RankResultOfDay = this.SyncData.RankResultOfDay;
				zhengBaSyncData.CenterTime = TimeUtil.NowDateTime();
				if (zhengBaSyncData.RoleModTime > lastSyncData.RoleModTime && this.SyncData.RoleList != null)
				{
					zhengBaSyncData.RoleList = new List<ZhengBaRoleInfoData>(this.SyncData.RoleList);
				}
				if (zhengBaSyncData.SupportModTime > lastSyncData.SupportModTime && this.SyncData.SupportList != null)
				{
					zhengBaSyncData.SupportList = new List<ZhengBaSupportAnalysisData>(this.SyncData.SupportList);
				}
				if (zhengBaSyncData.LastKingModTime > lastSyncData.LastKingModTime)
				{
					zhengBaSyncData.LastKingData = this.SyncData.LastKingData;
				}
			}
			return zhengBaSyncData;
		}

		public int ZhengBaSupport(ZhengBaSupportLogData data)
		{
			int result;
			if (data == null || !this.Persistence.SaveSupportLog(data))
			{
				result = -15;
			}
			else
			{
				lock (this.Mutex)
				{
					ZhengBaSupportAnalysisData zhengBaSupportAnalysisData;
					if ((zhengBaSupportAnalysisData = this.SyncData.SupportList.Find((ZhengBaSupportAnalysisData _s) => _s.UnionGroup == data.ToUnionGroup && _s.Group == data.ToGroup)) == null)
					{
						zhengBaSupportAnalysisData = new ZhengBaSupportAnalysisData
						{
							UnionGroup = data.ToUnionGroup,
							Group = data.ToGroup,
							RankOfDay = data.RankOfDay
						};
						this.SyncData.SupportList.Add(zhengBaSupportAnalysisData);
					}
					if (data.SupportType == 1)
					{
						zhengBaSupportAnalysisData.TotalSupport++;
					}
					else if (data.SupportType == 2)
					{
						zhengBaSupportAnalysisData.TotalOppose++;
					}
					else if (data.SupportType == 3)
					{
						zhengBaSupportAnalysisData.TotalYaZhu++;
					}
					this.SyncData.SupportModTime = TimeUtil.NowDateTime();
					this.AsyncEvQ.Enqueue(new AsyncDataItem(10009, new object[]
					{
						data
					}));
				}
				result = 1;
			}
			return result;
		}

		public int ZhengBaRequestEnter(int roleId, int gameId, EZhengBaEnterType enter)
		{
			lock (this.Mutex)
			{
				if (this.StateMachine.GetCurrState() != ZhengBaStateMachine.StateType.NotifyEnter)
				{
					return -2001;
				}
				ZhengBaManagerK.JoinRolePkData joinRolePkData = this.TodayJoinRoleDatas.Find((ZhengBaManagerK.JoinRolePkData _r) => _r.RoleID == roleId && _r.CurrGameID == gameId);
				ZhengBaPkLogData zhengBaPkLogData = null;
				this.ThisLoopPkLogs.TryGetValue(gameId, out zhengBaPkLogData);
				if (joinRolePkData == null || zhengBaPkLogData == null)
				{
					return -12;
				}
				if (!joinRolePkData.WaitReqEnter)
				{
					return -12;
				}
				joinRolePkData.WaitReqEnter = false;
				joinRolePkData.WaitKuaFuLogin = true;
				if (enter == 2)
				{
					if (zhengBaPkLogData.RoleID1 == roleId)
					{
						zhengBaPkLogData.IsMirror1 = true;
					}
					else if (zhengBaPkLogData.RoleID2 == roleId)
					{
						zhengBaPkLogData.IsMirror2 = true;
					}
					this.AsyncEvQ.Enqueue(new AsyncDataItem(10012, new object[]
					{
						new ZhengBaMirrorFightData
						{
							RoleId = roleId,
							GameId = gameId,
							ToServerId = joinRolePkData.ToServerID
						}
					}));
				}
			}
			return 0;
		}

		public int ZhengBaKuaFuLogin(int roleid, int gameId)
		{
			lock (this.Mutex)
			{
				ZhengBaManagerK.JoinRolePkData joinRolePkData = this.TodayJoinRoleDatas.Find((ZhengBaManagerK.JoinRolePkData _r) => _r.RoleID == roleid && _r.CurrGameID == gameId);
				ZhengBaPkLogData zhengBaPkLogData = null;
				this.ThisLoopPkLogs.TryGetValue(gameId, out zhengBaPkLogData);
				if (joinRolePkData == null || zhengBaPkLogData == null)
				{
					return -12;
				}
				if (!joinRolePkData.WaitKuaFuLogin)
				{
					return -12;
				}
			}
			return 0;
		}

		public List<ZhengBaNtfPkResultData> ZhengBaPkResult(int gameId, int winner1, int FirstLeaveRoleId)
		{
			bool flag = false;
			List<ZhengBaNtfPkResultData> result;
			try
			{
				object mutex;
				Monitor.Enter(mutex = this.Mutex, ref flag);
				ZhengBaPkLogData log = null;
				if (!this.ThisLoopPkLogs.TryGetValue(gameId, out log))
				{
					result = null;
				}
				else
				{
					if (FirstLeaveRoleId == log.RoleID1)
					{
						winner1 = log.RoleID2;
					}
					else if (FirstLeaveRoleId == log.RoleID2)
					{
						winner1 = log.RoleID1;
					}
					if (winner1 != log.RoleID1 && winner1 != log.RoleID2)
					{
						result = null;
					}
					else
					{
						ZhengBaManagerK.JoinRolePkData joinRolePkData = this.TodayJoinRoleDatas.Find((ZhengBaManagerK.JoinRolePkData _r) => _r.RoleID == log.RoleID1 && _r.CurrGameID == gameId);
						ZhengBaManagerK.JoinRolePkData joinRolePkData2 = this.TodayJoinRoleDatas.Find((ZhengBaManagerK.JoinRolePkData _r) => _r.RoleID == log.RoleID2 && _r.CurrGameID == gameId);
						if (joinRolePkData == null || joinRolePkData2 == null)
						{
							result = null;
						}
						else
						{
							ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
							ZhengBaNtfPkResultData zhengBaNtfPkResultData = new ZhengBaNtfPkResultData
							{
								RoleID = joinRolePkData.RoleID
							};
							ZhengBaNtfPkResultData zhengBaNtfPkResultData2 = new ZhengBaNtfPkResultData
							{
								RoleID = joinRolePkData2.RoleID
							};
							ZhengBaManagerK.JoinRolePkData winJoinRole = null;
							ZhengBaNtfPkResultData zhengBaNtfPkResultData3 = null;
							if (winner1 > 0 && winner1 == joinRolePkData.RoleID)
							{
								winJoinRole = joinRolePkData;
								zhengBaNtfPkResultData3 = zhengBaNtfPkResultData;
							}
							else if (winner1 > 0 && winner1 == joinRolePkData2.RoleID)
							{
								winJoinRole = joinRolePkData2;
								zhengBaNtfPkResultData3 = zhengBaNtfPkResultData2;
							}
							if (winJoinRole != null && zhengBaNtfPkResultData3 != null)
							{
								zhengBaNtfPkResultData3.IsWin = true;
								winJoinRole.WinTimes++;
								if (winJoinRole.WinTimes >= zhengBaMatchConfig.NeedWinTimes && this.HadUpGradeRoleNum < zhengBaMatchConfig.MaxUpGradeNum)
								{
									int num = (this.RandomGroup.Count > 0) ? this.RandomGroup.Last<int>() : 0;
									bool flag2 = false;
									ZhengBaRoleInfoData zhengBaRoleInfoData = this.SyncData.RoleList.Find((ZhengBaRoleInfoData _r) => _r.RoleId == winJoinRole.RoleID);
									if (zhengBaRoleInfoData != null)
									{
										int willUpGrade = zhengBaMatchConfig.WillUpGrade;
										int state = 1;
										int group = (num != 0) ? num : zhengBaRoleInfoData.Group;
										if (this.Persistence.UpdateRole(this.SyncData.Month, zhengBaRoleInfoData.RoleId, willUpGrade, state, group))
										{
											zhengBaRoleInfoData.Grade = willUpGrade;
											zhengBaRoleInfoData.State = state;
											zhengBaRoleInfoData.Group = group;
											this.SyncData.RoleModTime = TimeUtil.NowDateTime();
											flag2 = true;
											if (willUpGrade != 1)
											{
												this.AsyncEvQ.Enqueue(new AsyncDataItem(10013, new object[]
												{
													new ZhengBaBulletinJoinData
													{
														NtfType = 3,
														Args1 = zhengBaRoleInfoData.RoleId
													}
												}));
											}
										}
									}
									if (flag2)
									{
										zhengBaNtfPkResultData3.RandGroup = num;
										zhengBaNtfPkResultData3.IsUpGrade = true;
										zhengBaNtfPkResultData3.NewGrade = zhengBaRoleInfoData.Grade;
										log.UpGrade = true;
										this.HadUpGradeRoleNum++;
										this.RandomGroup.Remove(num);
										this.TodayJoinRoleDatas.RemoveAll((ZhengBaManagerK.JoinRolePkData _r) => _r.RoleID == winJoinRole.RoleID);
									}
								}
							}
							log.EndTime = TimeUtil.NowDateTime();
							if (winner1 > 0 && winner1 == log.RoleID1)
							{
								log.PkResult = 1;
							}
							else if (winner1 > 0 && winner1 == log.RoleID2)
							{
								log.PkResult = 2;
							}
							else
							{
								log.PkResult = 0;
							}
							zhengBaNtfPkResultData.StillNeedWin = Math.Max(0, zhengBaMatchConfig.NeedWinTimes - joinRolePkData.WinTimes);
							zhengBaNtfPkResultData.LeftUpGradeNum = zhengBaMatchConfig.MaxUpGradeNum - this.HadUpGradeRoleNum;
							zhengBaNtfPkResultData2.StillNeedWin = Math.Max(0, zhengBaMatchConfig.NeedWinTimes - joinRolePkData2.WinTimes);
							zhengBaNtfPkResultData2.LeftUpGradeNum = zhengBaMatchConfig.MaxUpGradeNum - this.HadUpGradeRoleNum;
							this.Persistence.SavePkLog(log);
							this.ThisLoopPkLogs.Remove(gameId);
							this.AsyncEvQ.Enqueue(new AsyncDataItem(10010, new object[]
							{
								log
							}));
							result = new List<ZhengBaNtfPkResultData>
							{
								zhengBaNtfPkResultData,
								zhengBaNtfPkResultData2
							};
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

		private static ZhengBaManagerK _instance = new ZhengBaManagerK();

		private ZhengBaSyncData SyncData = new ZhengBaSyncData
		{
			Month = ZhengBaUtils.MakeMonth(TimeUtil.NowDateTime())
		};

		private DateTime lastUpdateTime = TimeUtil.NowDateTime();

		private Queue<AsyncDataItem> AsyncEvQ = new Queue<AsyncDataItem>();

		private int HadUpGradeRoleNum = 0;

		private List<int> RandomGroup = new List<int>();

		private ZhengBaConfig _Config = new ZhengBaConfig();

		private List<ZhengBaManagerK.JoinRolePkData> TodayJoinRoleDatas = new List<ZhengBaManagerK.JoinRolePkData>();

		private Dictionary<int, ZhengBaPkLogData> ThisLoopPkLogs = new Dictionary<int, ZhengBaPkLogData>();

		private int CurrLoopIndex = 0;

		private ZhengBaStateMachine StateMachine = new ZhengBaStateMachine();

		private object Mutex = new object();

		private ZhengBaPersistence Persistence = ZhengBaPersistence.Instance;

		private class JoinRolePkData
		{
			public int RoleID;

			public int ZoneId;

			public string RoleName;

			public int Group;

			public int ToServerID;

			public int CurrGameID;

			public bool WaitReqEnter;

			public bool WaitKuaFuLogin;

			public int WinTimes;
		}
	}
}
