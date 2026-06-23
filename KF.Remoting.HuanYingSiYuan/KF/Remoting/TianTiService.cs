using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.ServiceModel;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using KF.Remoting.Data;
using Remoting;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true, UseSynchronizationContext = true)]
	public class TianTiService : MarshalByRefObject, ITianTiService, IExecCommand
	{
		public override object InitializeLifetimeService()
		{
			TianTiService.Instance = this;
			ILease lease = (ILease)base.InitializeLifetimeService();
			if (lease.CurrentState == LeaseState.Initial)
			{
				lease.InitialLeaseTime = TimeSpan.FromDays(2000.0);
			}
			return lease;
		}

		public TianTiService()
		{
			TianTiService.Instance = this;
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
		}

		~TianTiService()
		{
			this.BackgroundThread.Abort();
		}

		public void ThreadProc(object state)
		{
			do
			{
				Thread.Sleep(1000);
			}
			while (!this.Persistence.Initialized);
			for (;;)
			{
				try
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					Global.UpdateNowTime(dateTime);
					if (dateTime > this.CheckRoleTimerProcTime)
					{
						this.CheckRoleTimerProcTime = dateTime.AddSeconds(1.428);
						int signUpCount;
						int startCount;
						this.CheckRoleTimerProc(dateTime, out signUpCount, out startCount);
						ClientAgentManager.Instance().SetGameTypeLoad(this.GameType, signUpCount, startCount);
					}
					if (dateTime > this.SaveServerStateProcTime)
					{
						this.SaveServerStateProcTime = dateTime.AddSeconds(30.0);
						if (dateTime.Hour >= 3 && dateTime.Hour < 4)
						{
							this.ClearRolePairFightCount();
							this.Persistence.UpdateTianTiRankData(dateTime, false, false);
						}
					}
					if (dateTime > this.CheckGameFuBenTime)
					{
						this.CheckGameFuBenTime = dateTime.AddSeconds(1000.0);
						this.CheckGameFuBenTimerProc(dateTime);
					}
					AsyncDataItem[] evItems = ZhengBaManagerK.Instance().Update();
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.GameType, evItems);
					this.Persistence.WriteRoleInfoDataProc();
					CoupleArenaService.getInstance().Update();
					CoupleWishService.getInstance().Update();
					zhengDuoService.Instance().Update(dateTime);
					BangHuiMatchService.Instance().Update(dateTime);
					CompService.Instance().Update(dateTime);
					TianTi5v5Service.ThreadProc(null);
					Zork5v5Service.Instance().Update(dateTime);
					int num = (int)(TimeUtil.NowDateTime() - dateTime).TotalMilliseconds;
					this.Persistence.SaveCostTime(num);
					num = 1000 - num;
					if (num < 50)
					{
						num = 50;
					}
					Thread.Sleep(num);
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
			}
		}

		public RangeKey GetAssignRange(int baseValue, long startTicks, long waitTicks1, long waitTicks3, long waitTicksAll)
		{
			int num;
			if (startTicks > waitTicks3)
			{
				if (startTicks > waitTicks1)
				{
					num = 0;
				}
				else
				{
					num = 1;
				}
			}
			else if (startTicks > waitTicksAll)
			{
				num = 2;
			}
			else
			{
				num = 3;
			}
			int num2 = this.AssignRangeArray[num];
			return new RangeKey(baseValue - num2, baseValue + num2, null);
		}

		private void CheckRoleTimerProc(DateTime now, out int signUpCnt, out int startCount)
		{
			signUpCnt = 0;
			startCount = 0;
			bool flag = true;
			long ticks = now.AddHours(-2.0).Ticks;
			long ticks2 = now.AddSeconds((double)(-(double)this.Persistence.SignUpWaitSecs1)).Ticks;
			long ticks3 = now.AddSeconds((double)(-(double)this.Persistence.SignUpWaitSecs3)).Ticks;
			long ticks4 = now.AddSeconds((double)(-(double)this.Persistence.SignUpWaitSecsAll)).Ticks;
			long ticks5 = now.AddSeconds((double)(-(double)this.Persistence.WaitForJoinMaxSecs)).Ticks;
			this.ProcessTianTiFuBenDataDict.Clear();
			foreach (KuaFuRoleData kuaFuRoleData in this.RoleIdKuaFuRoleDataDict.Values)
			{
				int num = 0;
				lock (kuaFuRoleData)
				{
					if (kuaFuRoleData.State == null || kuaFuRoleData.State > 5)
					{
						if (kuaFuRoleData.StateEndTicks < ticks)
						{
							KuaFuRoleData kuaFuRoleData2;
							this.RoleIdKuaFuRoleDataDict.TryRemove(KuaFuRoleKey.Get(kuaFuRoleData.ServerId, kuaFuRoleData.RoleId), out kuaFuRoleData2);
							continue;
						}
					}
					else if (kuaFuRoleData.State == 3 || kuaFuRoleData.State == 4)
					{
						if (kuaFuRoleData.StateEndTicks < now.Ticks)
						{
							kuaFuRoleData.Age++;
							kuaFuRoleData.State = 0;
							num = kuaFuRoleData.GameId;
						}
					}
					else if (kuaFuRoleData.State == 1)
					{
						if (kuaFuRoleData.StateEndTicks < ticks5)
						{
							kuaFuRoleData.Age++;
							kuaFuRoleData.State = 0;
						}
					}
				}
				if (kuaFuRoleData.State == 1)
				{
					signUpCnt++;
					if (flag)
					{
						RangeKey assignRange = this.GetAssignRange(kuaFuRoleData.GroupIndex, kuaFuRoleData.StateEndTicks, ticks2, ticks3, ticks4);
						flag = this.AssignGameFuben(kuaFuRoleData, assignRange, now);
					}
				}
				else if (kuaFuRoleData.State == 2)
				{
					signUpCnt++;
				}
				else if (kuaFuRoleData.State == 5)
				{
					startCount++;
				}
				if (num > 0)
				{
					this.RemoveRoleFromFuBen(num, kuaFuRoleData.RoleId);
					AsyncDataItem evItem = new AsyncDataItem(1, new object[]
					{
						kuaFuRoleData
					});
					ClientAgentManager.Instance().PostAsyncEvent(kuaFuRoleData.ServerId, this.GameType, evItem);
				}
			}
		}

		private void CheckGameFuBenTimerProc(DateTime now)
		{
			if (this.TianTiFuBenDataDict.Count > 0)
			{
				DateTime t = now.AddMinutes(-8.0);
				foreach (TianTiFuBenData tianTiFuBenData in this.TianTiFuBenDataDict.Values)
				{
					lock (tianTiFuBenData)
					{
						if (tianTiFuBenData.CanRemove())
						{
							this.RemoveGameFuBen(tianTiFuBenData);
						}
						else if (tianTiFuBenData.EndTime < t)
						{
							this.RemoveGameFuBen(tianTiFuBenData);
						}
					}
				}
			}
		}

		public AsyncDataItem[] GetClientCacheItems(int serverId)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType);
		}

		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.GameType == 2 && clientInfo.ServerId != 0)
				{
					result = ClientAgentManager.Instance().InitializeClient(clientInfo);
				}
				else
				{
					LogManager.WriteLog(2, string.Format("InitializeClient时GameType错误,禁止连接.ServerId:{0},GameType:{1}", clientInfo.ServerId, clientInfo.GameType), null, true);
					result = -4003;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(string.Format("InitializeClient服务器ID重复,禁止连接.ServerId:{0},ClientId:{1}", clientInfo.ServerId, clientInfo.ClientId));
				result = -11003;
			}
			return result;
		}

		public int RoleSignUp(int serverId, string userId, int zoneId, int roleId, int gameType, int groupIndex, IGameData gameData)
		{
			int result = 1;
			int num;
			KuaFuRoleData orAdd;
			if (!this.UserId2RoleIdActiveDict.TryGetValue(userId, out num))
			{
				this.UserId2RoleIdActiveDict[userId] = roleId;
			}
			else if (num > 0 && num != roleId)
			{
				if (this.RoleIdKuaFuRoleDataDict.TryGetValue(KuaFuRoleKey.Get(serverId, num), out orAdd))
				{
					if (this.ChangeRoleState(orAdd, 0) < 0)
					{
					}
				}
				this.UserId2RoleIdActiveDict[userId] = roleId;
			}
			Lazy<KuaFuRoleData> lazy = new Lazy<KuaFuRoleData>(() => new KuaFuRoleData
			{
				RoleId = roleId,
				UserId = userId,
				GameType = gameType
			});
			KuaFuRoleKey key = KuaFuRoleKey.Get(serverId, roleId);
			orAdd = this.RoleIdKuaFuRoleDataDict.GetOrAdd(key, (KuaFuRoleKey x) => lazy.Value);
			int num2 = 0;
			lock (orAdd)
			{
				num2 = orAdd.GameId;
				orAdd.GameId = 0;
				orAdd.Age++;
				orAdd.State = 1;
				orAdd.ServerId = serverId;
				orAdd.ZoneId = zoneId;
				orAdd.GameData = gameData;
				orAdd.GroupIndex = groupIndex;
				orAdd.StateEndTicks = Global.NowTime.Ticks;
			}
			if (num2 > 0)
			{
				this.RemoveRoleFromFuBen(num2, roleId);
			}
			return result;
		}

		public int RoleChangeState(int serverId, int roleId, int state)
		{
			KuaFuRoleKey key = KuaFuRoleKey.Get(serverId, roleId);
			KuaFuRoleData kuaFuRoleData;
			int result;
			if (!this.RoleIdKuaFuRoleDataDict.TryGetValue(key, out kuaFuRoleData))
			{
				result = -11003;
			}
			else
			{
				int num = 0;
				lock (kuaFuRoleData)
				{
					if (state == 0)
					{
						if (kuaFuRoleData.GameId > 0)
						{
							TianTiFuBenData tianTiFuBenData;
							if (this.TianTiFuBenDataDict.TryGetValue(kuaFuRoleData.GameId, out tianTiFuBenData))
							{
								AsyncDataItem evItem = new AsyncDataItem(4, new object[]
								{
									kuaFuRoleData.GameId
								});
								ClientAgentManager.Instance().PostAsyncEvent(tianTiFuBenData.ServerId, this.GameType, evItem);
							}
						}
						num = kuaFuRoleData.GameId;
						kuaFuRoleData.GameId = 0;
					}
					kuaFuRoleData.Age++;
					kuaFuRoleData.State = state;
				}
				if (num > 0)
				{
					this.RemoveRoleFromFuBen(num, roleId);
				}
				result = state;
			}
			return result;
		}

		public int GameFuBenRoleChangeState(int serverId, int roleId, int gameId, int state)
		{
			TianTiFuBenData tianTiFuBenData;
			if (this.TianTiFuBenDataDict.TryGetValue(gameId, out tianTiFuBenData))
			{
				lock (tianTiFuBenData)
				{
					KuaFuFuBenRoleData kuaFuFuBenRoleData;
					if (tianTiFuBenData.RoleDict.TryGetValue(roleId, out kuaFuFuBenRoleData))
					{
						if (state == 7 || state == 0)
						{
							this.RemoveRoleFromFuBen(gameId, roleId);
						}
					}
				}
			}
			KuaFuRoleData kuaFuRoleData;
			int result;
			if (!this.RoleIdKuaFuRoleDataDict.TryGetValue(KuaFuRoleKey.Get(serverId, roleId), out kuaFuRoleData))
			{
				result = -20;
			}
			else
			{
				if (kuaFuRoleData.GameId == gameId)
				{
					this.ChangeRoleState(kuaFuRoleData, state);
				}
				result = state;
			}
			return result;
		}

		public KuaFuRoleData GetKuaFuRoleData(int serverId, int roleId)
		{
			KuaFuRoleData kuaFuRoleData = null;
			KuaFuRoleData result;
			if (this.RoleIdKuaFuRoleDataDict.TryGetValue(KuaFuRoleKey.Get(serverId, roleId), out kuaFuRoleData) && kuaFuRoleData.State != 0)
			{
				result = kuaFuRoleData;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public int GetRoleExtendData(int serverId, int roleId, int dataType)
		{
			KuaFuRoleData kuaFuRoleData = null;
			int result;
			if (!this.RoleIdKuaFuRoleDataDict.TryGetValue(KuaFuRoleKey.Get(serverId, roleId), out kuaFuRoleData))
			{
				result = 0;
			}
			else if (dataType == 0)
			{
				int num = 0;
				if (kuaFuRoleData.State == 1)
				{
					num = 1;
				}
				if (kuaFuRoleData.GameId > 0)
				{
					TianTiFuBenData tianTiFuBenData;
					if (this.TianTiFuBenDataDict.TryGetValue(kuaFuRoleData.GameId, out tianTiFuBenData))
					{
						if (tianTiFuBenData.State < 2)
						{
							num = tianTiFuBenData.GetFuBenRoleCount();
						}
						else
						{
							this.RemoveRoleFromFuBen(kuaFuRoleData.GameId, roleId);
							this.RoleChangeState(serverId, roleId, 0);
							num = 0;
						}
					}
				}
				result = num;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public List<KuaFuServerInfo> GetKuaFuServerInfoData(int age)
		{
			return KuaFuServerManager.GetKuaFuServerInfoData(age);
		}

		public TianTiFuBenData GetFuBenData(int gameId)
		{
			TianTiFuBenData tianTiFuBenData = null;
			TianTiFuBenData result;
			if (this.TianTiFuBenDataDict.TryGetValue(gameId, out tianTiFuBenData) && tianTiFuBenData.State < 3)
			{
				result = tianTiFuBenData;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public int GameFuBenChangeState(int gameId, GameFuBenState state, DateTime time)
		{
			int num = -11;
			TianTiFuBenData tianTiFuBenData = null;
			int result;
			if (this.TianTiFuBenDataDict.TryGetValue(gameId, out tianTiFuBenData))
			{
				this.AddRolePairFightCount(tianTiFuBenData);
				lock (tianTiFuBenData)
				{
					tianTiFuBenData.State = state;
					if (state == 3)
					{
						this.RemoveGameFuBen(tianTiFuBenData);
					}
				}
				result = num;
			}
			else
			{
				result = -20;
			}
			return result;
		}

		public TianTiRankData GetRankingData(DateTime modifyTime)
		{
			return this.Persistence.GetTianTiRankData(modifyTime);
		}

		public void UpdateRoleInfoData(TianTiRoleInfoData data)
		{
			this.Persistence.UpdateRoleInfoData(data);
		}

		public ZhengBaSyncData SyncZhengBaData(ZhengBaSyncData lastSyncData)
		{
			return ZhengBaManagerK.Instance().SyncZhengBaData(lastSyncData);
		}

		public int ZhengBaSupport(ZhengBaSupportLogData data)
		{
			return ZhengBaManagerK.Instance().ZhengBaSupport(data);
		}

		public int ZhengBaRequestEnter(int roleId, int gameId, EZhengBaEnterType enter)
		{
			return ZhengBaManagerK.Instance().ZhengBaRequestEnter(roleId, gameId, enter);
		}

		public int ZhengBaKuaFuLogin(int roleid, int gameId)
		{
			return ZhengBaManagerK.Instance().ZhengBaKuaFuLogin(roleid, gameId);
		}

		public List<ZhengBaNtfPkResultData> ZhengBaPkResult(int gameId, int winner, int FirstLeaveRoleId)
		{
			return ZhengBaManagerK.Instance().ZhengBaPkResult(gameId, winner, FirstLeaveRoleId);
		}

		private int ChangeRoleState(KuaFuRoleData kuaFuRoleData, KuaFuRoleStates state)
		{
			int result = -1;
			try
			{
				KuaFuRoleData kuaFuRoleData2 = null;
				int num = 0;
				lock (kuaFuRoleData)
				{
					kuaFuRoleData.Age++;
					kuaFuRoleData.State = state;
					if (state == null && kuaFuRoleData.GameId > 0)
					{
						num = kuaFuRoleData.GameId;
					}
					kuaFuRoleData2 = kuaFuRoleData;
				}
				if (num > 0)
				{
					this.RemoveRoleFromFuBen(num, kuaFuRoleData.RoleId);
				}
				if (null != kuaFuRoleData2)
				{
					AsyncDataItem evItem = new AsyncDataItem(1, new object[]
					{
						kuaFuRoleData
					});
					ClientAgentManager.Instance().PostAsyncEvent(kuaFuRoleData.ServerId, this.GameType, evItem);
				}
			}
			catch (Exception ex)
			{
				return -1;
			}
			return result;
		}

		private void NotifyFuBenRoleCount(TianTiFuBenData fuBenData)
		{
			try
			{
				lock (fuBenData)
				{
					int count = fuBenData.RoleDict.Count;
					foreach (KuaFuFuBenRoleData kuaFuFuBenRoleData in fuBenData.RoleDict.Values)
					{
						KuaFuRoleData kuaFuRoleData;
						if (this.RoleIdKuaFuRoleDataDict.TryGetValue(KuaFuRoleKey.Get(kuaFuFuBenRoleData.ServerId, kuaFuFuBenRoleData.RoleId), out kuaFuRoleData))
						{
							AsyncDataItem evItem = new AsyncDataItem(2, new object[]
							{
								kuaFuRoleData.RoleId,
								count
							});
							ClientAgentManager.Instance().PostAsyncEvent(kuaFuRoleData.ServerId, this.GameType, evItem);
						}
					}
				}
			}
			catch
			{
			}
		}

		private void NotifyFuBenRoleEnterGame(TianTiFuBenData fuBenData)
		{
			try
			{
				lock (fuBenData)
				{
					foreach (KuaFuFuBenRoleData kuaFuFuBenRoleData in fuBenData.RoleDict.Values)
					{
						KuaFuRoleData kuaFuRoleData;
						if (this.RoleIdKuaFuRoleDataDict.TryGetValue(KuaFuRoleKey.Get(kuaFuFuBenRoleData.ServerId, kuaFuFuBenRoleData.RoleId), out kuaFuRoleData) && kuaFuRoleData.State == 3)
						{
							AsyncDataItem evItem = new AsyncDataItem(3, new object[]
							{
								kuaFuRoleData
							});
							ClientAgentManager.Instance().PostAsyncEvent(kuaFuRoleData.ServerId, this.GameType, evItem);
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		private long MakeRolePairKey(int roleId1, int roleId2)
		{
			long result;
			if (roleId1 < roleId2)
			{
				result = ((long)roleId1 << 32) + (long)roleId2;
			}
			else
			{
				result = ((long)roleId2 << 32) + (long)roleId1;
			}
			return result;
		}

		private void ClearRolePairFightCount()
		{
			lock (this.RolePairFightCountDict)
			{
				this.RolePairFightCountDict.Clear();
			}
		}

		private void AddRolePairFightCount(TianTiFuBenData tianTiFuBenData)
		{
			int num = 0;
			int roleId = 0;
			if (tianTiFuBenData.RoleDict.Count >= 2)
			{
				foreach (KuaFuFuBenRoleData kuaFuFuBenRoleData in tianTiFuBenData.RoleDict.Values)
				{
					if (num == 0)
					{
						num = kuaFuFuBenRoleData.RoleId;
					}
					else
					{
						roleId = kuaFuFuBenRoleData.RoleId;
					}
				}
				long key = this.MakeRolePairKey(num, roleId);
				lock (this.RolePairFightCountDict)
				{
					int num2;
					if (!this.RolePairFightCountDict.TryGetValue(key, out num2))
					{
						this.RolePairFightCountDict[key] = 1;
					}
					else
					{
						this.RolePairFightCountDict[key] = num2 + 1;
					}
				}
			}
		}

		private bool CanAddFuBenRole(TianTiFuBenData tianTiFuBenData, KuaFuRoleData kuaFuRoleData)
		{
			bool result;
			if (tianTiFuBenData.RoleDict.Count == 0)
			{
				result = true;
			}
			else
			{
				KuaFuFuBenRoleData kuaFuFuBenRoleData = tianTiFuBenData.RoleDict.Values.FirstOrDefault<KuaFuFuBenRoleData>();
				long key = this.MakeRolePairKey(kuaFuRoleData.RoleId, kuaFuFuBenRoleData.RoleId);
				lock (this.RolePairFightCountDict)
				{
					int num;
					if (!this.RolePairFightCountDict.TryGetValue(key, out num) || num < this.Persistence.MaxRolePairFightCount)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		private bool AssignGameFuben(KuaFuRoleData kuaFuRoleData, RangeKey range, DateTime now)
		{
			DateTime dateTime = now.AddSeconds((double)this.EnterGameSecs);
			TianTiFuBenData tianTiFuBenData = null;
			KuaFuFuBenRoleData kuaFuFuBenRoleData = new KuaFuFuBenRoleData
			{
				ServerId = kuaFuRoleData.ServerId,
				RoleId = kuaFuRoleData.RoleId
			};
			List<KuaFuRoleData> list = new List<KuaFuRoleData>();
			if (!this.ProcessTianTiFuBenDataDict.TryGetValue(range, out tianTiFuBenData))
			{
				tianTiFuBenData = new TianTiFuBenData();
				this.ProcessTianTiFuBenDataDict.Add(range, tianTiFuBenData);
			}
			else if (!this.CanAddFuBenRole(tianTiFuBenData, kuaFuRoleData))
			{
				return true;
			}
			int num = tianTiFuBenData.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData);
			bool result;
			if (num < Consts.TianTiRoleCountTotal)
			{
				result = true;
			}
			else
			{
				try
				{
					int serverId = 0;
					int nextGameId = this.Persistence.GetNextGameId();
					bool flag = ClientAgentManager.Instance().AssginKfFuben(this.GameType, (long)nextGameId, num, out serverId);
					if (flag)
					{
						tianTiFuBenData.ServerId = serverId;
						tianTiFuBenData.GameId = nextGameId;
						tianTiFuBenData.EndTime = Global.NowTime.AddMinutes(8.0);
						this.AddGameFuBen(tianTiFuBenData);
						this.Persistence.LogCreateTianTiFuBen(tianTiFuBenData.GameId, tianTiFuBenData.ServerId, 0, num);
						foreach (KuaFuFuBenRoleData kuaFuFuBenRoleData2 in tianTiFuBenData.RoleDict.Values)
						{
							KuaFuRoleKey key = KuaFuRoleKey.Get(kuaFuFuBenRoleData2.ServerId, kuaFuFuBenRoleData2.RoleId);
							KuaFuRoleData kuaFuRoleData2;
							if (this.RoleIdKuaFuRoleDataDict.TryGetValue(key, out kuaFuRoleData2))
							{
								kuaFuRoleData2.UpdateStateTime(tianTiFuBenData.GameId, 3, dateTime.Ticks);
							}
						}
						tianTiFuBenData.State = 2;
						this.NotifyFuBenRoleEnterGame(tianTiFuBenData);
						this.ProcessTianTiFuBenDataDict.Remove(range);
						return true;
					}
					return false;
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
				result = false;
			}
			return result;
		}

		private void AddGameFuBen(TianTiFuBenData tianTiFuBenData)
		{
			this.TianTiFuBenDataDict[tianTiFuBenData.GameId] = tianTiFuBenData;
		}

		private void RemoveGameFuBen(TianTiFuBenData tianTiFuBenData)
		{
			int gameId = tianTiFuBenData.GameId;
			TianTiFuBenData tianTiFuBenData2;
			if (this.TianTiFuBenDataDict.TryRemove(gameId, out tianTiFuBenData2))
			{
				tianTiFuBenData2.State = 3;
			}
			ClientAgentManager.Instance().RemoveKfFuben(this.GameType, tianTiFuBenData.ServerId, (long)tianTiFuBenData.GameId);
			lock (tianTiFuBenData)
			{
				foreach (KuaFuFuBenRoleData kuaFuFuBenRoleData in tianTiFuBenData.RoleDict.Values)
				{
					KuaFuRoleData kuaFuRoleData;
					if (this.RoleIdKuaFuRoleDataDict.TryGetValue(KuaFuRoleKey.Get(kuaFuFuBenRoleData.ServerId, kuaFuFuBenRoleData.RoleId), out kuaFuRoleData))
					{
						if (kuaFuRoleData.GameId == gameId)
						{
							kuaFuRoleData.State = 0;
						}
					}
				}
			}
		}

		private void RemoveRoleFromFuBen(int gameId, int roleId)
		{
			if (gameId > 0)
			{
				TianTiFuBenData tianTiFuBenData;
				if (this.TianTiFuBenDataDict.TryGetValue(gameId, out tianTiFuBenData))
				{
					lock (tianTiFuBenData)
					{
						int num = tianTiFuBenData.RemoveKuaFuFuBenRoleData(roleId);
						if (tianTiFuBenData.CanRemove())
						{
							this.RemoveGameFuBen(tianTiFuBenData);
						}
						else if (tianTiFuBenData.State < 2)
						{
							this.NotifyFuBenRoleCount(tianTiFuBenData);
						}
					}
				}
			}
		}

		public int ExecCommand(string[] args)
		{
			int result = -1;
			try
			{
				if (string.Compare(args[0], "reload") == 0 && 0 == string.Compare(args[1], "paihang"))
				{
					bool monthRank = false;
					if (args.Length >= 3)
					{
						monthRank = true;
					}
					this.Persistence.UpdateTianTiRankData(TimeUtil.NowDateTime(), monthRank, true);
				}
				else if (0 == string.Compare(args[0], "load"))
				{
					this.Persistence.LoadTianTiRankData(TimeUtil.NowDateTime());
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		public int CoupleArenaJoin(int roleId1, int roleId2, int serverId)
		{
			return CoupleArenaService.getInstance().CoupleArenaJoin(roleId1, roleId2, serverId);
		}

		public int CoupleArenaQuit(int roleId1, int roleId2)
		{
			return CoupleArenaService.getInstance().CoupleArenaQuit(roleId1, roleId2);
		}

		public CoupleArenaSyncData CoupleArenaSync(DateTime lastSyncTime)
		{
			return CoupleArenaService.getInstance().CoupleArenaSync(lastSyncTime);
		}

		public int CoupleArenaPreDivorce(int roleId1, int roleId2)
		{
			return CoupleArenaService.getInstance().CoupleArenaPreDivorce(roleId1, roleId2);
		}

		public CoupleArenaFuBenData GetCoupleFuBenData(long gameId)
		{
			return CoupleArenaService.getInstance().GetCoupleFuBenData(gameId);
		}

		public CoupleArenaPkResultRsp CoupleArenaPkResult(CoupleArenaPkResultReq req)
		{
			return CoupleArenaService.getInstance().CoupleArenaPkResult(req);
		}

		public int CoupleWishWishRole(CoupleWishWishRoleReq req)
		{
			return CoupleWishService.getInstance().CoupleWishWishRole(req);
		}

		public List<CoupleWishWishRecordData> CoupleWishGetWishRecord(int roleId)
		{
			return CoupleWishService.getInstance().CoupleWishGetWishRecord(roleId);
		}

		public CoupleWishSyncData CoupleWishSyncCenterData(DateTime oldThisWeek, DateTime oldLastWeek, DateTime oldStatue)
		{
			return CoupleWishService.getInstance().CoupleWishSyncCenterData(oldThisWeek, oldLastWeek, oldStatue);
		}

		public int CoupleWishPreDivorce(int man, int wife)
		{
			return CoupleWishService.getInstance().CoupleWishPreDivorce(man, wife);
		}

		public void CoupleWishReportCoupleStatue(CoupleWishReportStatueData req)
		{
			CoupleWishService.getInstance().CoupleWishReportCoupleStatue(req);
		}

		public int CoupleWishAdmire(int fromRole, int fromZone, int admireType, int toCoupleId)
		{
			return CoupleWishService.getInstance().CoupleWishAdmire(fromRole, fromZone, admireType, toCoupleId);
		}

		public int CoupleWishJoinParty(int fromRole, int fromZone, int toCoupleId)
		{
			return CoupleWishService.getInstance().CoupleWishJoinParty(fromRole, fromZone, toCoupleId);
		}

		public ZhengDuoSyncData ZhengDuoSync(int serverID, long version)
		{
			return zhengDuoService.Instance().ZhengDuoSync(serverID, version);
		}

		public int ZhengDuoSign(int serverID, int bhid, int usedTime, int zoneId, string bhName, int bhLevel, long bhZhanLi)
		{
			return zhengDuoService.Instance().ZhengDuoSign(serverID, bhid, usedTime, zoneId, bhName, bhLevel, bhZhanLi);
		}

		public int ZhengDuoResult(int bhidSuccess, int[] bhids)
		{
			return zhengDuoService.Instance().ZhengDuoResult(bhidSuccess, bhids);
		}

		public int GmCommand(string[] args, byte[] data)
		{
			return zhengDuoService.Instance().GmCommand(args, data);
		}

		public ZhengDuoFuBenData GetZhengDuoFuBenDataByBhid(int bhid)
		{
			return zhengDuoService.Instance().GetZhengDuoFuBenDataByBhid(bhid);
		}

		public ZhengDuoFuBenData GetZhengDuoFuBenData(long gameId)
		{
			return zhengDuoService.Instance().GetZhengDuoFuBenData(gameId);
		}

		public KuaFuCmdData GetBHDataByBhid_BHMatch(int type, int bhid, long age)
		{
			return BangHuiMatchService.Instance().GetBHDataByBhid_BHMatch(type, bhid, age);
		}

		public BHMatchSyncData SyncData_BHMatch(long ageRank, long agePKInfo, long ageChampion)
		{
			return BangHuiMatchService.Instance().SyncData_BHMatch(ageRank, agePKInfo, ageChampion);
		}

		public string GetKuaFuGameState_BHMatch(int bhid)
		{
			return BangHuiMatchService.Instance().GetKuaFuGameState_BHMatch(bhid);
		}

		public bool CheckRookieJoinLast_BHMatch(int bhid)
		{
			return BangHuiMatchService.Instance().CheckRookieJoinLast_BHMatch(bhid);
		}

		public int RookieSignUp_BHMatch(int bhid, int zoneid_bh, string bhname, int rid, string rname, int zoneid_r)
		{
			return BangHuiMatchService.Instance().RookieSignUp_BHMatch(bhid, zoneid_bh, bhname, rid, rname, zoneid_r);
		}

		public BHMatchFuBenData GetFuBenDataByBhid_BHMatch(int bhid)
		{
			return BangHuiMatchService.Instance().GetFuBenDataByBhid_BHMatch(bhid);
		}

		public BHMatchFuBenData GetFuBenDataByGameId_BHMatch(int gameid)
		{
			return BangHuiMatchService.Instance().GetFuBenDataByGameId_BHMatch(gameid);
		}

		public int GameFuBenComplete_BHMatch(BangHuiMatchStatisticalData data)
		{
			return BangHuiMatchService.Instance().GameFuBenComplete_BHMatch(data);
		}

		public int RemoveBangHui_BHMatch(int bhid)
		{
			return BangHuiMatchService.Instance().RemoveBangHui_BHMatch(bhid);
		}

		public void SwitchLastGoldBH_GM()
		{
			BangHuiMatchService.Instance().SwitchLastGoldBH_GM();
		}

		public CompSyncData Comp_SyncData(long ageComp, long ageRankJX, long ageRankJXL, long ageRankBD, long ageRankBJF, long ageRankMJF)
		{
			return CompService.Instance().Comp_SyncData(ageComp, ageRankJX, ageRankJXL, ageRankBD, ageRankBJF, ageRankMJF);
		}

		public KuaFuCmdData Comp_GetCompRoleData(int roleId, long dataAge)
		{
			return CompService.Instance().GetCompRoleData(roleId, dataAge);
		}

		public void Comp_ChangeName(int roleId, string roleName)
		{
			CompService.Instance().ChangeName(roleId, roleName);
		}

		public int Comp_JoinComp(int roleId, int zoneId, string roleName, int compType)
		{
			return CompService.Instance().JoinComp(roleId, zoneId, roleName, compType);
		}

		public int Comp_JoinComp_Repair(int roleId, int zoneId, string roleName, int compType, int battleJiFen)
		{
			return CompService.Instance().Comp_JoinComp_Repair(roleId, zoneId, roleName, compType, battleJiFen);
		}

		public void Comp_CompOpt(int compType, int optType, int param1, int param2)
		{
			CompService.Instance().CompOpt(compType, optType, param1, param2);
		}

		public void Comp_SetBulletin(int compType, string bulletin)
		{
			CompService.Instance().SetBulletin(compType, bulletin);
		}

		public void Comp_BroadCastCompNotice(int serverId, byte[] bytes)
		{
			CompService.Instance().BroadCastCompNotice(serverId, bytes);
		}

		public void Comp_CompChat(int serverId, byte[] bytes)
		{
			CompService.Instance().CompChat(serverId, bytes);
		}

		public void Comp_SetRoleData4Selector(int roleId, byte[] bytes)
		{
			CompService.Instance().SetRoleData4Selector(roleId, bytes);
		}

		public void Comp_UpdateMapRoleNum(int mapCode, int roleNum)
		{
			CompService.Instance().UpdateMapRoleNum(mapCode, roleNum);
		}

		public void Comp_UpdateFuBenMapRoleNum(int gameType, CompFuBenData fubenItem)
		{
			CompService.Instance().UpdateFuBenMapRoleNum(gameType, fubenItem);
		}

		public void Comp_UpdateStrongholdData(int cityID, List<CompStrongholdData> shDataList)
		{
			CompService.Instance().UpdateStrongholdData(cityID, shDataList);
		}

		public int Comp_GameFuBenRoleChangeState(int gameType, int serverId, int cityID, int roleId, int zhiwu, int state)
		{
			return CompService.Instance().GameFuBenRoleChangeState(gameType, serverId, cityID, roleId, zhiwu, state);
		}

		public KuaFuCmdData Comp_GetKuaFuFuBenData(int gameType, int cityID, long dataAge)
		{
			return CompService.Instance().GetKuaFuFuBenData(gameType, cityID, dataAge);
		}

		public int CreateZhanDui(int serverID, TianTi5v5ZhanDuiData pData)
		{
			return TianTi5v5Service.CreateZhanDui(serverID, pData);
		}

		public int UpdateZhanDuiXuanYan(long teamID, string xuanYan)
		{
			return TianTi5v5Service.UpdateZhanDuiXuanYan(teamID, xuanYan);
		}

		public int DeleteZhanDui(int serverID, int roleid, int teamID)
		{
			return TianTi5v5Service.DeleteZhanDui(serverID, roleid, teamID);
		}

		public int UpdateZhanDuiData(TianTi5v5ZhanDuiData data, ZhanDuiDataModeTypes modeType)
		{
			return TianTi5v5Service.UpdateZhanDuiData(data, modeType);
		}

		public int ZhanDuiRoleSignUp(int serverId, int gameType, int teamID, long zhanLi, int groupIndex)
		{
			return TianTi5v5Service.ZhanDuiRoleSignUp(serverId, gameType, teamID, zhanLi, groupIndex);
		}

		public int ZhanDuiRoleChangeState(int serverId, int zhanDuiID, int roleId, int state, int gameID)
		{
			return TianTi5v5Service.ZhanDuiRoleChangeState(serverId, zhanDuiID, roleId, state, gameID);
		}

		public KuaFu5v5FuBenData ZhanDuiGetFuBenData(int gameId)
		{
			return TianTi5v5Service.ZhanDuiGetFuBenData(gameId);
		}

		public int ZhanDuiGameFuBenChangeState(int gameId, GameFuBenState state, DateTime time)
		{
			int num = -11;
			TianTiFuBenData tianTiFuBenData = null;
			int result;
			if (this.TianTiFuBenDataDict.TryGetValue(gameId, out tianTiFuBenData))
			{
				this.AddRolePairFightCount(tianTiFuBenData);
				lock (tianTiFuBenData)
				{
					tianTiFuBenData.State = state;
					if (state == 3)
					{
						this.RemoveGameFuBen(tianTiFuBenData);
					}
				}
				result = num;
			}
			else
			{
				result = -20;
			}
			return result;
		}

		public TianTi5v5RankData ZhanDuiGetRankingData(DateTime modifyTime)
		{
			return TianTi5v5Service.ZhanDuiGetRankingData(modifyTime);
		}

		public KuaFu5v5FuBenData GetFuBenDataByGameId_ZorkBattle(int gameid)
		{
			return Zork5v5Service.Instance().GetFuBenDataByGameId_ZorkBattle(gameid);
		}

		public KuaFu5v5FuBenData GetFuBenDataByZhanDuiId_ZorkBattle(int ZhanDuiId)
		{
			return Zork5v5Service.Instance().GetFuBenDataByZhanDuiId_ZorkBattle(ZhanDuiId);
		}

		public string GetKuaFuGameState_ZorkBattle(int zhanduiID)
		{
			return Zork5v5Service.Instance().GetKuaFuGameState_ZorkBattle(zhanduiID);
		}

		public ZorkBattleSyncData SyncData_ZorkBattle(long gsTicks, long ageRank)
		{
			return Zork5v5Service.Instance().SyncData_ZorkBattle(gsTicks, ageRank);
		}

		public int SignUp_ZorkBattle(int zhanduiID, int serverID)
		{
			return Zork5v5Service.Instance().SignUp_ZorkBattle(zhanduiID, serverID);
		}

		public int GameFuBenComplete_ZorkBattle(ZorkBattleStatisticalData data)
		{
			return Zork5v5Service.Instance().GameFuBenComplete_ZorkBattle(data);
		}

		public int ExcuteGMCmd(int serverID, int rid, string[] cmd)
		{
			return 0;
		}

		private const double CheckGameFuBenInterval = 1000.0;

		private const double CheckRoleTimerProcInterval = 1.428;

		private const double SaveServerStateProcInterval = 30.0;

		public static TianTiService Instance = null;

		private object Mutex = new object();

		public readonly GameTypes GameType = 2;

		private DateTime CheckGameFuBenTime;

		private DateTime CheckRoleTimerProcTime;

		private DateTime SaveServerStateProcTime;

		public TianTiPersistence Persistence = TianTiPersistence.Instance;

		public int ExistKuaFuFuBenCount = 0;

		private int EnterGameSecs = 20;

		public int DisconnectionProtectSecs = 15;

		public int GameEndProtectSecs = 15;

		private int MaxServerLoad = 400;

		private int MaxTianTiDayCount = 1000;

		private int TianTiDayCount = 0;

		private int TianTiDayId = 0;

		public int[] AssignRangeArray = new int[]
		{
			0,
			1,
			2,
			100
		};

		public ConcurrentDictionary<int, TianTiFuBenData> TianTiFuBenDataDict = new ConcurrentDictionary<int, TianTiFuBenData>(1, 4096);

		private SortedDictionary<RangeKey, TianTiFuBenData> ProcessTianTiFuBenDataDict = new SortedDictionary<RangeKey, TianTiFuBenData>(RangeKey.Comparer);

		private SortedList<long, int> RolePairFightCountDict = new SortedList<long, int>();

		private ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData> RoleIdKuaFuRoleDataDict = new ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData>();

		public ConcurrentDictionary<string, int> UserId2RoleIdActiveDict = new ConcurrentDictionary<string, int>(1, 16384);

		public Thread BackgroundThread;
	}
}
