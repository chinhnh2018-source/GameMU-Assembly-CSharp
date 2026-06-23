using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using System.ServiceModel;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using KF.Remoting.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace KF.Remoting
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class HuanYingSiYuanService : MarshalByRefObject, IKuaFuService
	{
		public override object InitializeLifetimeService()
		{
			HuanYingSiYuanService.Instance = this;
			ILease lease = (ILease)base.InitializeLifetimeService();
			if (lease.CurrentState == LeaseState.Initial)
			{
				lease.InitialLeaseTime = TimeSpan.FromDays(2000.0);
			}
			return lease;
		}

		public HuanYingSiYuanService()
		{
			HuanYingSiYuanService.Instance = this;
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
		}

		~HuanYingSiYuanService()
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
			DateTime dateTime = TimeUtil.NowDateTime();
			for (;;)
			{
				try
				{
					DateTime dateTime2 = TimeUtil.NowDateTime();
					Global.UpdateNowTime(dateTime2);
					if (dateTime2 > this.CheckRoleTimerProcTime)
					{
						this.CheckRoleTimerProcTime = dateTime2.AddSeconds(1.428);
						int signUpCount;
						int startCount;
						this.CheckRoleTimerProc(dateTime2, out signUpCount, out startCount);
						ClientAgentManager.Instance().SetGameTypeLoad(this.GameType, signUpCount, startCount);
					}
					if (dateTime2 > this.CheckGameFuBenTime)
					{
						this.CheckGameFuBenTime = dateTime2.AddSeconds(1000.0);
						this.CheckGameFuBenTimerProc(dateTime2);
					}
					int num = (int)(TimeUtil.NowDateTime() - dateTime2).TotalMilliseconds;
					this.Persistence.SaveCostTime(num);
					num = 1600 - num;
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

		private void CheckRoleTimerProc(DateTime now, out int signUpCount, out int startCount)
		{
			signUpCount = 0;
			startCount = 0;
			bool flag = true;
			long ticks = now.AddHours(-2.0).Ticks;
			long ticks2 = now.AddSeconds((double)(-(double)this.Persistence.SignUpWaitSecs1)).Ticks;
			long ticks3 = now.AddSeconds((double)(-(double)this.Persistence.SignUpWaitSecs2)).Ticks;
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
				}
				if (kuaFuRoleData.State == 1)
				{
					signUpCount++;
					if (flag)
					{
						flag = this.AssignGameFuben(kuaFuRoleData, ticks2, ticks3, now);
					}
				}
				else if (kuaFuRoleData.State == 2)
				{
					signUpCount++;
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
			if (this.HuanYingSiYuanFuBenDataDict.Count > 0)
			{
				DateTime t = now.AddMinutes(-15.0);
				foreach (HuanYingSiYuanFuBenData huanYingSiYuanFuBenData in this.HuanYingSiYuanFuBenDataDict.Values)
				{
					lock (huanYingSiYuanFuBenData)
					{
						if (huanYingSiYuanFuBenData.CanRemove())
						{
							this.RemoveGameFuBen(huanYingSiYuanFuBenData);
						}
						else if (huanYingSiYuanFuBenData.EndTime < t)
						{
							this.RemoveGameFuBen(huanYingSiYuanFuBenData);
						}
					}
				}
			}
		}

		public AsyncDataItem[] GetClientCacheItems(int serverId)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType);
		}

		public AsyncData GetClientCacheItems2(int serverId, long requestTicks)
		{
			return new AsyncData
			{
				RequestTicks = requestTicks,
				ServerTicks = TimeUtil.NOW(),
				ItemList = ClientAgentManager.Instance().PickAsyncEvent(serverId, this.GameType)
			};
		}

		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.GameType == 1 && clientInfo.ServerId != 0)
				{
					result = ClientAgentManager.Instance().InitializeClient(clientInfo);
				}
				else
				{
					LogManager.WriteLog(1, string.Format("InitializeClient时GameType错误,禁止连接.ServerId:{0},GameType:{1}", clientInfo.ServerId, clientInfo.GameType), null, true);
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
			HuanYingSiYuanFuBenData huanYingSiYuanFuBenData;
			if (this.HuanYingSiYuanFuBenDataDict.TryGetValue(gameId, out huanYingSiYuanFuBenData))
			{
				lock (huanYingSiYuanFuBenData)
				{
					KuaFuFuBenRoleData kuaFuFuBenRoleData;
					if (huanYingSiYuanFuBenData.RoleDict.TryGetValue(roleId, out kuaFuFuBenRoleData))
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
					HuanYingSiYuanFuBenData huanYingSiYuanFuBenData;
					if (this.HuanYingSiYuanFuBenDataDict.TryGetValue(kuaFuRoleData.GameId, out huanYingSiYuanFuBenData))
					{
						if (huanYingSiYuanFuBenData.State < 2)
						{
							num = huanYingSiYuanFuBenData.GetFuBenRoleCount();
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

		public HuanYingSiYuanFuBenData GetFuBenData(int gameId)
		{
			HuanYingSiYuanFuBenData huanYingSiYuanFuBenData = null;
			HuanYingSiYuanFuBenData result;
			if (this.HuanYingSiYuanFuBenDataDict.TryGetValue(gameId, out huanYingSiYuanFuBenData) && huanYingSiYuanFuBenData.State < 3)
			{
				result = huanYingSiYuanFuBenData;
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
			HuanYingSiYuanFuBenData huanYingSiYuanFuBenData = null;
			int result;
			if (this.HuanYingSiYuanFuBenDataDict.TryGetValue(gameId, out huanYingSiYuanFuBenData))
			{
				lock (huanYingSiYuanFuBenData)
				{
					huanYingSiYuanFuBenData.State = state;
					if (state == 3)
					{
						this.RemoveGameFuBen(huanYingSiYuanFuBenData);
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

		private void NotifyFuBenRoleCount(HuanYingSiYuanFuBenData fuBenData)
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

		private void NotifyFuBenRoleEnterGame(HuanYingSiYuanFuBenData fuBenData)
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

		private bool AssignGameFuben(KuaFuRoleData kuaFuRoleData, long waitSecs1, long waitSecs2, DateTime now)
		{
			int num = 0;
			DateTime dateTime = now.AddSeconds((double)this.EnterGameSecs);
			HuanYingSiYuanFuBenData huanYingSiYuanFuBenData = null;
			KuaFuFuBenRoleData kuaFuFuBenRoleData = new KuaFuFuBenRoleData
			{
				ServerId = kuaFuRoleData.ServerId,
				RoleId = kuaFuRoleData.RoleId,
				ZhanDouLi = kuaFuRoleData.GameData.ZhanDouLi
			};
			try
			{
				foreach (HuanYingSiYuanFuBenData huanYingSiYuanFuBenData2 in this.ShotOfRolesFuBenDataDict.Values)
				{
					if (huanYingSiYuanFuBenData2.CanRemove())
					{
						this.RemoveGameFuBen(huanYingSiYuanFuBenData2);
					}
					else if (huanYingSiYuanFuBenData2.CanEnter(kuaFuRoleData.GroupIndex, waitSecs1, waitSecs2))
					{
						if (ClientAgentManager.Instance().IsAgentAlive(huanYingSiYuanFuBenData2.ServerId))
						{
							num = huanYingSiYuanFuBenData2.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData, new GameFuBenRoleCountChanged(this.GameFuBenRoleCountChangedHandler));
							if (num > 0)
							{
								huanYingSiYuanFuBenData = huanYingSiYuanFuBenData2;
								break;
							}
						}
					}
				}
				if (null == huanYingSiYuanFuBenData)
				{
					int nextGameId = this.Persistence.GetNextGameId();
					int num2 = 0;
					if (ClientAgentManager.Instance().AssginKfFuben(this.GameType, (long)nextGameId, 1, out num2))
					{
						huanYingSiYuanFuBenData = new HuanYingSiYuanFuBenData();
						huanYingSiYuanFuBenData.ServerId = num2;
						huanYingSiYuanFuBenData.GameId = nextGameId;
						huanYingSiYuanFuBenData.GroupIndex = kuaFuRoleData.GroupIndex;
						huanYingSiYuanFuBenData.EndTime = Global.NowTime.AddMinutes(15.0);
						this.AddGameFuBen(huanYingSiYuanFuBenData);
						num = huanYingSiYuanFuBenData.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData, new GameFuBenRoleCountChanged(this.GameFuBenRoleCountChangedHandler));
						this.Persistence.LogCreateHysyFuben(nextGameId, num2, 0, 1);
					}
				}
				if (huanYingSiYuanFuBenData != null && num > 0)
				{
					if (num == 1)
					{
						huanYingSiYuanFuBenData.EndTime = now;
					}
					if (huanYingSiYuanFuBenData.State == 0)
					{
						if (num == Consts.HuanYingSiYuanRoleCountTotal)
						{
							List<KuaFuFuBenRoleData> list = huanYingSiYuanFuBenData.SortFuBenRoleList();
							foreach (KuaFuFuBenRoleData kuaFuFuBenRoleData2 in list)
							{
								KuaFuRoleKey key = KuaFuRoleKey.Get(kuaFuFuBenRoleData2.ServerId, kuaFuFuBenRoleData2.RoleId);
								KuaFuRoleData kuaFuRoleData2;
								if (this.RoleIdKuaFuRoleDataDict.TryGetValue(key, out kuaFuRoleData2))
								{
									kuaFuRoleData2.UpdateStateTime(huanYingSiYuanFuBenData.GameId, 3, dateTime.Ticks);
								}
							}
							huanYingSiYuanFuBenData.State = 2;
							this.NotifyFuBenRoleEnterGame(huanYingSiYuanFuBenData);
						}
						else
						{
							kuaFuRoleData.UpdateStateTime(huanYingSiYuanFuBenData.GameId, 2, kuaFuRoleData.StateEndTicks);
							this.NotifyFuBenRoleCount(huanYingSiYuanFuBenData);
						}
					}
					else if (huanYingSiYuanFuBenData.State == 2)
					{
						kuaFuRoleData.UpdateStateTime(huanYingSiYuanFuBenData.GameId, 3, dateTime.Ticks);
						this.NotifyFuBenRoleEnterGame(huanYingSiYuanFuBenData);
					}
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		private void GameFuBenRoleCountChangedHandler(HuanYingSiYuanFuBenData huanYingSiYuanFuBenData, int roleCount)
		{
			if (roleCount == Consts.HuanYingSiYuanRoleCountTotal)
			{
				HuanYingSiYuanFuBenData huanYingSiYuanFuBenData2;
				this.ShotOfRolesFuBenDataDict.TryRemove(huanYingSiYuanFuBenData.GameId, out huanYingSiYuanFuBenData2);
			}
			else if (!huanYingSiYuanFuBenData.CanRemove())
			{
				this.ShotOfRolesFuBenDataDict[huanYingSiYuanFuBenData.GameId] = huanYingSiYuanFuBenData;
			}
		}

		private void AddGameFuBen(HuanYingSiYuanFuBenData huanYingSiYuanFuBenData)
		{
			this.HuanYingSiYuanFuBenDataDict[huanYingSiYuanFuBenData.GameId] = huanYingSiYuanFuBenData;
		}

		private void RemoveGameFuBen(HuanYingSiYuanFuBenData fubenData)
		{
			int gameId = fubenData.GameId;
			HuanYingSiYuanFuBenData huanYingSiYuanFuBenData;
			this.ShotOfRolesFuBenDataDict.TryRemove(gameId, out huanYingSiYuanFuBenData);
			if (this.HuanYingSiYuanFuBenDataDict.TryRemove(gameId, out huanYingSiYuanFuBenData))
			{
				huanYingSiYuanFuBenData.State = 3;
			}
			ClientAgentManager.Instance().RemoveKfFuben(this.GameType, huanYingSiYuanFuBenData.ServerId, (long)huanYingSiYuanFuBenData.GameId);
			lock (fubenData)
			{
				foreach (KuaFuFuBenRoleData kuaFuFuBenRoleData in fubenData.RoleDict.Values)
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
				HuanYingSiYuanFuBenData huanYingSiYuanFuBenData;
				if (this.HuanYingSiYuanFuBenDataDict.TryGetValue(gameId, out huanYingSiYuanFuBenData))
				{
					lock (huanYingSiYuanFuBenData)
					{
						int num = huanYingSiYuanFuBenData.RemoveKuaFuFuBenRoleData(roleId, new GameFuBenRoleCountChanged(this.GameFuBenRoleCountChangedHandler));
						if (huanYingSiYuanFuBenData.CanRemove())
						{
							this.RemoveGameFuBen(huanYingSiYuanFuBenData);
						}
						else if (huanYingSiYuanFuBenData.State < 2)
						{
							this.NotifyFuBenRoleCount(huanYingSiYuanFuBenData);
						}
					}
				}
			}
		}

		public int UseGiftCode(string ptid, string uid, string rid, string channel, string codeno, string appid, int zoneid, ref string giftid)
		{
			MySqlDataReader mySqlDataReader = null;
			int result;
			try
			{
				string sqlstring = string.Format("select count(codeno) from  t_uselog  where codeno='{0}'; ", codeno);
				long num = (long)DbHelperMySQL.GetSingle(sqlstring);
				if (num != 0L)
				{
					result = -14;
				}
				else
				{
					string strSQL = string.Format("select * from t_giftcode where codeno ='{0}'", codeno, appid);
					mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
					if (mySqlDataReader == null || !mySqlDataReader.HasRows || !mySqlDataReader.Read())
					{
						result = -11;
					}
					else
					{
						MySqlDataReader mySqlDataReader2 = mySqlDataReader;
						if (!string.IsNullOrEmpty(mySqlDataReader2["usetime"].ToString()))
						{
							result = -14;
						}
						else
						{
							giftid = mySqlDataReader2["giftid"].ToString();
							string text = mySqlDataReader2["ptid"].ToString();
							string text2 = mySqlDataReader2["channel"].ToString();
							List<string> list = new List<string>();
							string[] array = text2.Split(new char[]
							{
								'|'
							}, StringSplitOptions.RemoveEmptyEntries);
							foreach (string text3 in array)
							{
								list.Add(text3.ToLower());
							}
							string text4 = mySqlDataReader2["zoneid"].ToString();
							DateTime t = Convert.ToDateTime(mySqlDataReader2["startdate"]);
							DateTime t2 = Convert.ToDateTime(mySqlDataReader2["enddate"].ToString());
							int num2 = Convert.ToInt32(mySqlDataReader2["times"]);
							int num3 = Convert.ToInt32(mySqlDataReader2["character"]);
							List<string> list2 = new List<string>();
							list2.AddRange(text4.Split(new char[]
							{
								'|'
							}, StringSplitOptions.RemoveEmptyEntries));
							if (channel == "HMN")
							{
								channel = "HM";
							}
							if (text2 != "ALL" && !list.Contains(channel.ToLower()) && !list.Contains("ALL"))
							{
								result = -16;
							}
							else if (text4 != "0" && !list2.Contains(zoneid.ToString()))
							{
								result = -18;
							}
							else if (t > DateTime.Now.Date || t2 < DateTime.Now.Date)
							{
								result = -17;
							}
							else
							{
								string text5 = string.Format("select count(giftid) from t_uselog where giftid ='{0}'", giftid);
								if (num3 == 0)
								{
									text5 += string.Format(" and rid={0}", rid);
								}
								else
								{
									text5 += string.Format(" and userid='{0}'", uid);
								}
								int num4 = Convert.ToInt32(DbHelperMySQL.GetSingle(text5));
								if (num4 >= num2)
								{
									result = -12;
								}
								else
								{
									string sqlstring2 = string.Format("update t_giftcode set usetime=now() where codeno ='{0}';", codeno);
									DbHelperMySQL.GetSingle(sqlstring2);
									sqlstring2 = string.Format("insert into t_uselog(userid,rid,giftid,codeno,usetime,serverid,ptid,channel,status) values('{0}','{1}','{2}','{3}',now(),'{4}','{5}','{6}','{7}');", new object[]
									{
										uid,
										rid,
										giftid,
										codeno,
										zoneid,
										ptid,
										channel,
										1
									});
									DbHelperMySQL.GetSingle(sqlstring2);
									result = 0;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = -11003;
			}
			finally
			{
				if (null != mySqlDataReader)
				{
					mySqlDataReader.Close();
				}
			}
			return result;
		}

		public void BroadcastGMCmdData(GMCmdData data, int serverFlag)
		{
			lock (this.Mutex)
			{
				if (serverFlag == 1)
				{
					ClientAgentManager.Instance().KFBroadCastAsyncEvent(1, new AsyncDataItem(9996, new object[]
					{
						data
					}));
				}
				else
				{
					ClientAgentManager.Instance().BroadCastAsyncEvent(1, new AsyncDataItem(9996, new object[]
					{
						data
					}), 0);
				}
			}
		}

		public KuaFuLueDuoJingJiaResult JingJia_KuaFuLueDuo(int bhid, int zoneid_bh, string bhname, int ziJin, int serverId, int oldZiJin)
		{
			return KuaFuLueDuoService.Instance().JingJia_KuaFuLueDuo(bhid, zoneid_bh, bhname, ziJin, serverId, oldZiJin);
		}

		public KuaFuLueDuoFuBenData GetFuBenDataByServerId_KuaFuLueDuo(int serverId)
		{
			return KuaFuLueDuoService.Instance().GetFuBenDataByServerId_KuaFuLueDuo(serverId);
		}

		public KuaFuLueDuoFuBenData GetFuBenDataByGameId_KuaFuLueDuo(long gameId)
		{
			return KuaFuLueDuoService.Instance().GetFuBenDataByGameId_KuaFuLueDuo(gameId);
		}

		public byte[] GetRoleData_KuaFuLueDuo(long rid)
		{
			return KuaFuLueDuoService.Instance().GetRoleData_KuaFuLueDuo(rid);
		}

		public byte[] SyncData_KuaFuLueDuo(byte[] bytes)
		{
			return DataHelper2.ObjectToBytes<KuaFuLueDuoSyncData>(KuaFuLueDuoService.Instance().SyncData_KuaFuLueDuo(bytes));
		}

		public KuaFuCmdData GetBHDataByBhid_KuaFuLueDuo(int bhid, long age)
		{
			return KuaFuLueDuoService.Instance().GetBHDataByBhid_KuaFuLueDuo(bhid, age);
		}

		public int GameFuBenComplete_KuaFuLueDuo(KuaFuLueDuoStatisticalData data)
		{
			return KuaFuLueDuoService.Instance().GameFuBenComplete_KuaFuLueDuo(data);
		}

		private const double CheckGameFuBenInterval = 1000.0;

		private const double CheckRoleTimerProcInterval = 1.428;

		public static HuanYingSiYuanService Instance = null;

		private object Mutex = new object();

		public readonly GameTypes GameType = 1;

		private DateTime CheckGameFuBenTime;

		private DateTime CheckRoleTimerProcTime;

		public HuanYingSiYuanPersistence Persistence = HuanYingSiYuanPersistence.Instance;

		public int ExistKuaFuFuBenCount = 0;

		private int EnterGameSecs = 20;

		public int DisconnectionProtectSecs = 15;

		public int GameEndProtectSecs = 15;

		private int MaxServerLoad = 400;

		private int MaxHuanYingSiYuanDayCount = 1000;

		private int HuanYingSiYuanDayCount = 0;

		private int HuanYingSiYuanDayId = 0;

		public ConcurrentDictionary<int, HuanYingSiYuanFuBenData> HuanYingSiYuanFuBenDataDict = new ConcurrentDictionary<int, HuanYingSiYuanFuBenData>(1, 4096);

		private ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData> RoleIdKuaFuRoleDataDict = new ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData>();

		public ConcurrentDictionary<string, int> UserId2RoleIdActiveDict = new ConcurrentDictionary<string, int>(1, 16384);

		private ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData> WaitingKuaFuRoleDataDict = new ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData>();

		public ConcurrentDictionary<int, HuanYingSiYuanFuBenData> ShotOfRolesFuBenDataDict = new ConcurrentDictionary<int, HuanYingSiYuanFuBenData>(1, 4096);

		public Thread BackgroundThread;
	}
}
