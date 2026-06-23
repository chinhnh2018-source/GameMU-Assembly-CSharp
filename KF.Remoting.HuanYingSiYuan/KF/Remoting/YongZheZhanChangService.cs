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
using Server.Tools;

namespace KF.Remoting
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
	public class YongZheZhanChangService : MarshalByRefObject, IYongZheZhanChangService, IExecCommand
	{
		public override object InitializeLifetimeService()
		{
			YongZheZhanChangService.Instance = this;
			ILease lease = (ILease)base.InitializeLifetimeService();
			if (lease.CurrentState == LeaseState.Initial)
			{
				lease.InitialLeaseTime = TimeSpan.FromDays(2000.0);
			}
			return lease;
		}

		public YongZheZhanChangService()
		{
			YongZheZhanChangService.Instance = this;
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
		}

		~YongZheZhanChangService()
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
					this.RunLangHunLingYuTimerProc();
					if (dateTime2 > this.CheckRoleTimerProcTime)
					{
						this.CheckRoleTimerProcTime = dateTime2.AddSeconds(1.428);
						int signUpCount;
						int startCount;
						lock (this.Mutex)
						{
							this.CheckRoleTimerProc(dateTime2, out signUpCount, out startCount);
						}
						ClientAgentManager.Instance().SetGameTypeLoad(this.RunTimeGameType, signUpCount, startCount);
					}
					if (dateTime2 > this.CheckGameFuBenTime)
					{
						this.CheckGameFuBenTime = dateTime2.AddSeconds(1000.0);
						this.CheckGameFuBenTimerProc(dateTime2);
						this.CheckOverTimeLangHunLingYuGameFuBen(dateTime2);
					}
					this.Persistence.WriteRoleInfoDataProc();
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
			int gameState;
			lock (this.Mutex)
			{
				gameState = this.GameState;
			}
			if (gameState == 2)
			{
				LogManager.WriteLog(0, "清除上场遗留的活动副本信息,开始统计报名玩家列表", null, true);
				this.FuBenDataDict.Clear();
				this.PreAssignGameFuBenDataDict.Clear();
			}
			DateTime stateEndTime = now.AddSeconds((double)this.EnterGameSecs);
			DateTime dateTime = now.AddSeconds((double)(-(double)this.EnterGameSecs));
			foreach (KuaFuRoleData kuaFuRoleData in this.RoleIdKuaFuRoleDataDict.Values)
			{
				if (kuaFuRoleData.StateEndTicks < dateTime.Ticks)
				{
					KuaFuRoleData kuaFuRoleData2;
					this.RoleIdKuaFuRoleDataDict.TryRemove(KuaFuRoleKey.Get(kuaFuRoleData.ServerId, kuaFuRoleData.RoleId), out kuaFuRoleData2);
				}
				else if (kuaFuRoleData.State >= 1 && kuaFuRoleData.State < 5)
				{
					signUpCount++;
					if (kuaFuRoleData.State == 1)
					{
						if (gameState == 2)
						{
							this.AssignGameFubenStep1(kuaFuRoleData, stateEndTime.Ticks);
						}
					}
				}
				else if (kuaFuRoleData.State == 5)
				{
					startCount++;
				}
			}
			if (gameState == 2)
			{
				LogManager.WriteLog(0, string.Format("对玩家进行场次分组:SignUpRoleCount={0},StartGameRoleCount={1}", signUpCount, startCount), null, true);
				this.AssignGameFubenStep2();
				this.AssginGameFuBenComplete = false;
				lock (this.Mutex)
				{
					this.GameState = 3;
				}
			}
			else if (gameState == 3)
			{
				if (!this.AssginGameFuBenComplete)
				{
					LogManager.WriteLog(0, string.Format("尝试为场次创建活动副本", new object[0]), null, true);
					this.AssginGameFuBenComplete = this.AssignGameFubenStep3(stateEndTime);
				}
				else
				{
					lock (this.Mutex)
					{
						this.GameState = 1;
						GameLogItem gameLogItem = new GameLogItem();
						gameLogItem.SignUpCount = signUpCount;
						gameLogItem.EnterCount = startCount;
						gameLogItem.GameType = this.RunTimeGameType;
						this.Persistence.UpdateRoleInfoData(gameLogItem);
					}
				}
			}
		}

		private void CheckGameFuBenTimerProc(DateTime now)
		{
			if (this.FuBenDataDict.Count > 0)
			{
				foreach (YongZheZhanChangFuBenData yongZheZhanChangFuBenData in this.FuBenDataDict.Values)
				{
					lock (yongZheZhanChangFuBenData)
					{
						if (yongZheZhanChangFuBenData.CanRemove() || yongZheZhanChangFuBenData.EndTime < now)
						{
							this.RemoveGameFuBen(yongZheZhanChangFuBenData);
						}
					}
				}
			}
		}

		public AsyncDataItem[] GetClientCacheItems(int serverId)
		{
			bool flag = false;
			lock (this.Mutex)
			{
				if (this.Persistence.LangHunLingYuInitialized)
				{
					ClientAgent currentClientAgent = ClientAgentManager.Instance().GetCurrentClientAgent(serverId);
					if (currentClientAgent != null && currentClientAgent.ClientInfo.ClientId > 0)
					{
						int num;
						if (!this.Persistence.LangHunLingYuBroadcastServerIdHashSet.TryGetValue(serverId, out num) || num != currentClientAgent.ClientInfo.ClientId)
						{
							this.Persistence.LangHunLingYuBroadcastServerIdHashSet[serverId] = currentClientAgent.ClientInfo.ClientId;
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				this.PostAllLangHunLingYuData(serverId);
			}
			return ClientAgentManager.Instance().PickAsyncEvent(serverId, this.YzzcGameType);
		}

		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.ServerId != 0)
				{
					bool flag = false;
					int num = ClientAgentManager.Instance().InitializeClient(clientInfo, out flag);
					if (num > 0)
					{
						if (clientInfo.MapClientCountDict != null && clientInfo.MapClientCountDict.Count > 0)
						{
							KuaFuServerManager.UpdateKuaFuLineData(clientInfo.ServerId, clientInfo.MapClientCountDict);
							ClientAgentManager.Instance().SetMainlinePayload(clientInfo.ServerId, clientInfo.MapClientCountDict.Values.ToList<int>().Sum());
						}
					}
					result = num;
				}
				else
				{
					LogManager.WriteLog(2, string.Format("InitializeClient时GameType错误,禁止连接.ServerId:{0},GameType:{1}", clientInfo.ServerId, clientInfo.GameType), null, true);
					result = -4003;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(string.Format("InitializeClient服务器ID重复,禁止连接.ServerId:{0},ClientId:{1},info:{2}", clientInfo.ServerId, clientInfo.ClientId, clientInfo.Token));
				result = -11003;
			}
			return result;
		}

		public void PostAllLangHunLingYuData(int serverId)
		{
			lock (this.Mutex)
			{
				foreach (LangHunLingYuBangHuiDataEx langHunLingYuBangHuiDataEx in this.LangHunLingYuBangHuiDataExDict.Values)
				{
					AsyncDataItem evItem = new AsyncDataItem(6, new object[]
					{
						langHunLingYuBangHuiDataEx
					});
					ClientAgentManager.Instance().PostAsyncEvent(serverId, this.YzzcGameType, evItem);
				}
				foreach (LangHunLingYuCityDataEx langHunLingYuCityDataEx in this.LangHunLingYuCityDataExDict)
				{
					if (null != langHunLingYuCityDataEx)
					{
						AsyncDataItem evItem = new AsyncDataItem(7, new object[]
						{
							langHunLingYuCityDataEx
						});
						ClientAgentManager.Instance().PostAsyncEvent(serverId, this.YzzcGameType, evItem);
					}
				}
				ClientAgentManager.Instance().PostAsyncEvent(serverId, this.YzzcGameType, new AsyncDataItem(8, new object[]
				{
					new Dictionary<int, List<int>>(this.OtherCityList)
				}));
				ClientAgentManager.Instance().PostAsyncEvent(serverId, this.YzzcGameType, new AsyncDataItem(9, new object[]
				{
					this.GetLangHunLingYuCityOwnerHist()
				}));
			}
		}

		public void UpdateKuaFuMapClientCount(int serverId, Dictionary<int, int> mapClientCountDict)
		{
			if (mapClientCountDict != null && mapClientCountDict.Count > 0)
			{
				ClientAgent currentClientAgent = ClientAgentManager.Instance().GetCurrentClientAgent(serverId);
				if (null != currentClientAgent)
				{
					KuaFuServerManager.UpdateKuaFuLineData(currentClientAgent.ClientInfo.ServerId, mapClientCountDict);
					ClientAgentManager.Instance().SetMainlinePayload(currentClientAgent.ClientInfo.ServerId, mapClientCountDict.Values.ToList<int>().Sum());
				}
			}
		}

		public int ExecuteCommand(string cmd)
		{
			int result;
			if (string.IsNullOrEmpty(cmd))
			{
				result = -18;
			}
			else
			{
				string[] args = cmd.Split(YongZheZhanChangService.WriteSpaceChars, StringSplitOptions.RemoveEmptyEntries);
				result = this.ExecCommand(args);
			}
			return result;
		}

		public void UpdateStatisticalData(AsyncDataItem data)
		{
			this.Persistence.UpdateRoleInfoData(data.Args[0]);
		}

		public int RoleSignUp(int serverId, string userId, int zoneId, int roleId, int gameType, int groupIndex, IGameData gameData)
		{
			int num = 1;
			int result;
			if (this.GameState != 1)
			{
				result = -2001;
			}
			else if (!ClientAgentManager.Instance().ExistAgent(serverId))
			{
				LogManager.WriteLog(2, string.Format("RoleSignUp时ServerId错误.ServerId:{0},roleId:{1}", serverId, roleId), null, true);
				result = -500;
			}
			else
			{
				Lazy<KuaFuRoleData> lazy = new Lazy<KuaFuRoleData>(() => new KuaFuRoleData
				{
					RoleId = roleId,
					UserId = userId,
					GameType = gameType
				});
				KuaFuRoleKey key = KuaFuRoleKey.Get(serverId, roleId);
				KuaFuRoleData orAdd = this.RoleIdKuaFuRoleDataDict.GetOrAdd(key, (KuaFuRoleKey x) => lazy.Value);
				lock (orAdd)
				{
					orAdd.Age++;
					orAdd.State = 1;
					orAdd.ServerId = serverId;
					orAdd.ZoneId = zoneId;
					orAdd.GameData = gameData;
					orAdd.GroupIndex = groupIndex;
					orAdd.StateEndTicks = Global.NowTime.Ticks;
				}
				LogManager.WriteLog(1002, string.Format("YongZheZhanChang.RoleSignUp,{0},{1},{2},{3},{4},{5},{6}", new object[]
				{
					serverId,
					userId,
					zoneId,
					roleId,
					gameType,
					groupIndex,
					gameData.ZhanDouLi
				}), null, true);
				result = num;
			}
			return result;
		}

		public int RoleChangeState(int serverId, int roleId, int state)
		{
			int result;
			if (!ClientAgentManager.Instance().ExistAgent(serverId))
			{
				result = -11003;
			}
			else
			{
				KuaFuRoleKey key = KuaFuRoleKey.Get(serverId, roleId);
				KuaFuRoleData kuaFuRoleData;
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
			}
			return result;
		}

		public int GameFuBenRoleChangeState(int serverId, int roleId, int gameId, int state)
		{
			YongZheZhanChangFuBenData yongZheZhanChangFuBenData;
			if (this.FuBenDataDict.TryGetValue(gameId, out yongZheZhanChangFuBenData))
			{
				lock (yongZheZhanChangFuBenData)
				{
					KuaFuFuBenRoleData kuaFuFuBenRoleData;
					if (yongZheZhanChangFuBenData.RoleDict.TryGetValue(roleId, out kuaFuFuBenRoleData))
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
			else if (dataType == 2)
			{
				result = kuaFuRoleData.State;
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

		public YongZheZhanChangFuBenData GetFuBenData(int gameId)
		{
			YongZheZhanChangFuBenData yongZheZhanChangFuBenData = null;
			YongZheZhanChangFuBenData result;
			if (this.FuBenDataDict.TryGetValue(gameId, out yongZheZhanChangFuBenData) && yongZheZhanChangFuBenData.State < 3)
			{
				result = yongZheZhanChangFuBenData;
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
			YongZheZhanChangFuBenData yongZheZhanChangFuBenData = null;
			int result;
			if (this.FuBenDataDict.TryGetValue(gameId, out yongZheZhanChangFuBenData))
			{
				lock (yongZheZhanChangFuBenData)
				{
					yongZheZhanChangFuBenData.State = state;
					if (state == 3)
					{
						this.RemoveGameFuBen(yongZheZhanChangFuBenData);
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

		public AsyncDataItem GetKuaFuLineDataList(int mapCode)
		{
			return new AsyncDataItem(9998, new object[]
			{
				KuaFuServerManager.GetKuaFuLineDataList(mapCode)
			});
		}

		public int EnterKuaFuMap(int serverId, int roleId, int mapCode, int kuaFuLine)
		{
			int num = KuaFuServerManager.EnterKuaFuMapLine(kuaFuLine, mapCode);
			int result;
			if (num > 0)
			{
				KuaFuMapRoleData kuaFuMapRoleData = new KuaFuMapRoleData();
				kuaFuMapRoleData = this.RoleId2KuaFuMapIdDict.GetOrAdd(roleId, kuaFuMapRoleData);
				kuaFuMapRoleData.ServerId = serverId;
				kuaFuMapRoleData.RoleId = roleId;
				kuaFuMapRoleData.KuaFuMapCode = mapCode;
				kuaFuMapRoleData.KuaFuServerId = num;
				result = num;
			}
			else
			{
				result = -100;
			}
			return result;
		}

		public KuaFuMapRoleData GetKuaFuMapRoleData(int roleId)
		{
			KuaFuMapRoleData result;
			this.RoleId2KuaFuMapIdDict.TryGetValue(roleId, out result);
			return result;
		}

		private void RunLangHunLingYuTimerProc()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.Mutex)
			{
				if (!this.Persistence.LangHunLingYuInitialized)
				{
					List<LangHunLingYuBangHuiDataEx> list = new List<LangHunLingYuBangHuiDataEx>();
					List<LangHunLingYuCityDataEx> list2 = new List<LangHunLingYuCityDataEx>();
					List<LangHunLingYuKingHist> list3 = new List<LangHunLingYuKingHist>();
					if (this.Persistence.LoadBangHuiDataExList(list) && this.Persistence.LoadCityDataExList(list2) && this.Persistence.LoadCityOwnerHistory(list3))
					{
						HashSet<long> hashSet = new HashSet<long>();
						foreach (LangHunLingYuCityDataEx langHunLingYuCityDataEx in list2)
						{
							foreach (long item in langHunLingYuCityDataEx.Site)
							{
								hashSet.Add(item);
							}
							if (this.LangHunLingYuCityDataExDict[langHunLingYuCityDataEx.CityId] == null)
							{
								this.LangHunLingYuCityDataExDict[langHunLingYuCityDataEx.CityId] = langHunLingYuCityDataEx;
							}
							else
							{
								this.LangHunLingYuCityDataExDict[langHunLingYuCityDataEx.CityId].CityId = langHunLingYuCityDataEx.CityId;
								this.LangHunLingYuCityDataExDict[langHunLingYuCityDataEx.CityId].CityLevel = langHunLingYuCityDataEx.CityLevel;
								Array.Copy(langHunLingYuCityDataEx.Site, this.LangHunLingYuCityDataExDict[langHunLingYuCityDataEx.CityId].Site, this.LangHunLingYuCityDataExDict[langHunLingYuCityDataEx.CityId].Site.Length);
							}
						}
						foreach (LangHunLingYuKingHist langHunLingYuKingHist in list3)
						{
							int num = 0;
							if (!this.LangHunLingYuAdmireDict.TryGetValue(langHunLingYuKingHist.rid, out num))
							{
								this.LangHunLingYuAdmireDict[langHunLingYuKingHist.rid] = langHunLingYuKingHist.AdmireCount;
							}
						}
						foreach (LangHunLingYuBangHuiDataEx langHunLingYuBangHuiDataEx in list)
						{
							LangHunLingYuBangHuiDataEx langHunLingYuBangHuiDataEx2;
							if (!this.LangHunLingYuBangHuiDataExDict.TryGetValue((long)langHunLingYuBangHuiDataEx.Bhid, out langHunLingYuBangHuiDataEx2))
							{
								if (hashSet.Contains((long)langHunLingYuBangHuiDataEx.Bhid))
								{
									this.LangHunLingYuBangHuiDataExDict[(long)langHunLingYuBangHuiDataEx.Bhid] = langHunLingYuBangHuiDataEx;
								}
							}
							else
							{
								langHunLingYuBangHuiDataEx2.Bhid = langHunLingYuBangHuiDataEx.Bhid;
								langHunLingYuBangHuiDataEx2.BhName = langHunLingYuBangHuiDataEx.BhName;
								langHunLingYuBangHuiDataEx2.ZoneId = langHunLingYuBangHuiDataEx.ZoneId;
							}
						}
						this.LangHunLingYuCityHistList = list3;
						this.CalcBangHuiCityLevel(null, false);
						this.Persistence.LangHunLingYuInitialized = true;
					}
				}
				double num2 = (dateTime.TimeOfDay - this.Persistence.LangHunLingYuResetCityTime).TotalSeconds;
				if (num2 >= 0.0 && num2 < 300.0 && (dateTime - this.Persistence.LastLangHunLingYuResetCityTime).TotalHours >= 1.0)
				{
					this.Persistence.LastLangHunLingYuResetCityTime = dateTime;
					LangHunLingYuCityDataEx[] langHunLingYuCityDataExDict = this.LangHunLingYuCityDataExDict;
					int i = 0;
					while (i < langHunLingYuCityDataExDict.Length)
					{
						LangHunLingYuCityDataEx langHunLingYuCityDataEx = langHunLingYuCityDataExDict[i];
						bool flag2 = false;
						if (langHunLingYuCityDataEx != null)
						{
							bool flag3 = false;
							lock (this.Mutex)
							{
								CityLevelInfo cityLevelInfo;
								if (this.Persistence.CityLevelInfoDict.TryGetValue(langHunLingYuCityDataEx.CityLevel, out cityLevelInfo))
								{
									foreach (int num3 in cityLevelInfo.AttackWeekDay)
									{
										if (num3 == (int)dateTime.DayOfWeek)
										{
											flag3 = true;
											break;
										}
									}
								}
							}
							if (flag3)
							{
								langHunLingYuCityDataEx.GameId = 0;
								for (int k = 1; k < 4; k++)
								{
									if (langHunLingYuCityDataEx.Site[k] > 0L)
									{
										langHunLingYuCityDataEx.Site[k] = 0L;
										flag2 = true;
									}
								}
								if (flag2)
								{
									LogManager.WriteLog(0, string.Format("清空城池{0}进攻者状态", langHunLingYuCityDataEx.CityId), null, true);
									this.Persistence.SaveCityData(langHunLingYuCityDataEx);
									this.NotifyUpdateCityDataEx(langHunLingYuCityDataEx);
								}
							}
						}
						IL_4AA:
						i++;
						continue;
						goto IL_4AA;
					}
					this.FilterOwnerHistListData();
				}
				num2 %= 3600.0;
				num2 = (num2 + 3600.0) % 3600.0;
				if (num2 >= 600.0 && num2 <= 660.0 && (dateTime - this.Persistence.LastLangHunLingYuBroadcastTime).TotalHours >= 2.0)
				{
					this.Persistence.LastLangHunLingYuBroadcastTime = dateTime;
					this.Persistence.LangHunLingYuBroadcastServerIdHashSet.Clear();
				}
			}
		}

		private int GetCityLevelById(int cityId)
		{
			return 10 - (int)Math.Log((double)cityId, 2.0);
		}

		private void NotifyUpdatBangHuiDataEx(LangHunLingYuBangHuiDataEx bangHuiDataEx)
		{
			this.Broadcast2GsAgent(new AsyncDataItem(6, new object[]
			{
				bangHuiDataEx
			}));
		}

		private void NotifyUpdateCityDataEx(LangHunLingYuCityDataEx cityDataEx)
		{
			this.Broadcast2GsAgent(new AsyncDataItem(7, new object[]
			{
				cityDataEx
			}));
		}

		private void NotifyUpdateOtherCityList(Dictionary<int, List<int>> list)
		{
			this.Broadcast2GsAgent(new AsyncDataItem(8, new object[]
			{
				list
			}));
		}

		private void NotifyUpdateCityOwnerHist(List<LangHunLingYuKingHist> list)
		{
			this.Broadcast2GsAgent(new AsyncDataItem(9, new object[]
			{
				list
			}));
		}

		private void NotifyUpdateCityOwnerAdmire(int rid, int admirecount)
		{
			this.Broadcast2GsAgent(new AsyncDataItem(10, new object[]
			{
				rid,
				admirecount
			}));
		}

		public int CalcNeedNextLevelCityCount(int cityLevel, int maxAttackerPerCity)
		{
			int result;
			if (cityLevel <= 0)
			{
				result = 1023;
			}
			else
			{
				int num = 0;
				int num2 = 1 << 10 - cityLevel;
				int num3 = num2 * 2;
				for (int i = num2; i < num3; i++)
				{
					if (this.LangHunLingYuCityDataExDict[i] != null)
					{
						if (this.LangHunLingYuCityDataExDict[i].Site[0] > 0L)
						{
							num++;
						}
					}
				}
				result = (int)Math.Ceiling((double)num / (double)maxAttackerPerCity);
			}
			return result;
		}

		public List<int> GetRandomCityListByLevel(int cityLevel, int reserveCount)
		{
			List<int> list = new List<int>();
			int num = 0;
			int num2 = 1 << 10 - cityLevel;
			int num3 = num2 * 2;
			HashSet<int> hashSet = new HashSet<int>();
			for (int i = num2; i < num3; i++)
			{
				if (this.LangHunLingYuCityDataExDict[i] != null)
				{
					if (this.LangHunLingYuCityDataExDict[i].Site.Any((long x) => x > 0L))
					{
						hashSet.Add(i);
						num++;
					}
				}
			}
			int[] array = hashSet.ToArray<int>();
			foreach (int item in array)
			{
				int randomNumber = Global.GetRandomNumber(0, list.Count + 1);
				list.Insert(randomNumber, item);
			}
			if (num < reserveCount)
			{
				for (int i = num2; i < num3; i++)
				{
					if (!hashSet.Contains(i))
					{
						list.Add(i);
						num++;
						if (num >= reserveCount)
						{
							break;
						}
					}
				}
			}
			return list;
		}

		private void FilterOwnerHistListData()
		{
			lock (this.Mutex)
			{
				if (this.LangHunLingYuCityHistList != null && this.LangHunLingYuCityHistList.Count != 0)
				{
					CityLevelInfo cityLevelInfo = null;
					if (this.Persistence.CityLevelInfoDict.TryGetValue(10, out cityLevelInfo))
					{
						bool flag2 = false;
						DateTime completeTime = TimeUtil.NowDateTime();
						for (int i = 0; i < cityLevelInfo.AttackWeekDay.Length; i++)
						{
							if (completeTime.DayOfWeek == (DayOfWeek)cityLevelInfo.AttackWeekDay[i])
							{
								flag2 = true;
								break;
							}
						}
						if (flag2)
						{
							LangHunLingYuKingHist langHunLingYuKingHist = this.LangHunLingYuCityHistList[this.LangHunLingYuCityHistList.Count - 1];
							DateTime t = new DateTime(completeTime.Year, completeTime.Month, completeTime.Day);
							DateTime t2 = new DateTime(langHunLingYuKingHist.CompleteTime.Year, langHunLingYuKingHist.CompleteTime.Month, langHunLingYuKingHist.CompleteTime.Day);
							if (DateTime.Compare(t, t2) != 0)
							{
								langHunLingYuKingHist.CompleteTime = completeTime;
								this.LangHunLingYuCityHistList.Add(langHunLingYuKingHist);
								this.Persistence.InsertCityOwnerHistory(langHunLingYuKingHist);
								this.NotifyUpdateCityOwnerHist(this.GetLangHunLingYuCityOwnerHist());
							}
						}
					}
				}
			}
		}

		private List<LangHunLingYuKingHist> GetLangHunLingYuCityOwnerHist()
		{
			List<LangHunLingYuKingHist> result = null;
			lock (this.Mutex)
			{
				if (this.LangHunLingYuCityHistList.Count != 0 && this.LangHunLingYuCityHistList.Count > 10)
				{
					int index = this.LangHunLingYuCityHistList.Count - 10;
					result = this.LangHunLingYuCityHistList.GetRange(index, 10);
				}
				else
				{
					result = new List<LangHunLingYuKingHist>(this.LangHunLingYuCityHistList);
				}
			}
			return result;
		}

		public bool LangHunLingYuAdmaire(int rid)
		{
			lock (this.Mutex)
			{
				if (!this.Persistence.LangHunLingYuInitialized)
				{
					return false;
				}
				int num = 0;
				if (!this.LangHunLingYuAdmireDict.TryGetValue(rid, out num))
				{
					return false;
				}
				num = (this.LangHunLingYuAdmireDict[rid] = num + 1);
				foreach (LangHunLingYuKingHist langHunLingYuKingHist in this.LangHunLingYuCityHistList)
				{
					if (langHunLingYuKingHist.rid == rid)
					{
						langHunLingYuKingHist.AdmireCount++;
					}
				}
				this.Persistence.AdmireCityOwner(rid);
				this.NotifyUpdateCityOwnerAdmire(rid, num);
			}
			return true;
		}

		public int LangHunLingYuSignUp(string bhName, int bhid, int zoneId, int gameType, int groupIndex, int zhanDouLi)
		{
			int num = 0;
			try
			{
				lock (this.Mutex)
				{
					if (!this.Persistence.LangHunLingYuInitialized)
					{
						num = -11000;
						return num;
					}
					LangHunLingYuBangHuiDataEx langHunLingYuBangHuiDataEx;
					if (!this.LangHunLingYuBangHuiDataExDict.TryGetValue((long)bhid, out langHunLingYuBangHuiDataEx))
					{
						langHunLingYuBangHuiDataEx = new LangHunLingYuBangHuiDataEx
						{
							Bhid = bhid,
							BhName = bhName,
							ZoneId = zoneId
						};
						this.LangHunLingYuBangHuiDataExDict[(long)bhid] = langHunLingYuBangHuiDataEx;
						this.NotifyUpdatBangHuiDataEx(langHunLingYuBangHuiDataEx);
						this.Persistence.SaveBangHuiData(langHunLingYuBangHuiDataEx);
					}
					else if (langHunLingYuBangHuiDataEx.BhName != bhName)
					{
						langHunLingYuBangHuiDataEx.BhName = bhName;
						this.NotifyUpdatBangHuiDataEx(langHunLingYuBangHuiDataEx);
						this.Persistence.SaveBangHuiData(langHunLingYuBangHuiDataEx);
					}
					int num2 = langHunLingYuBangHuiDataEx.Level + 1;
					if (num2 > 10)
					{
						num = -4004;
						return num;
					}
					int num3 = 3;
					CityLevelInfo cityLevelInfo;
					if (this.Persistence.CityLevelInfoDict.TryGetValue(num2, out cityLevelInfo))
					{
						num3 = cityLevelInfo.MaxNum;
					}
					num = -2001;
					DateTime dateTime = TimeUtil.NowDateTime();
					for (int i = 0; i < cityLevelInfo.BaoMingTime.Count - 1; i += 2)
					{
						TimeSpan t = dateTime.TimeOfDay.Add(TimeSpan.FromDays((double)dateTime.DayOfWeek));
						if (t >= cityLevelInfo.BaoMingTime[i] && t <= cityLevelInfo.BaoMingTime[i + 1])
						{
							num = 1;
							break;
						}
					}
					if (num < 0)
					{
						return num;
					}
					int num4 = -1;
					int num5 = 0;
					int reserveCount = this.CalcNeedNextLevelCityCount(num2 - 1, num3);
					List<int> randomCityListByLevel = this.GetRandomCityListByLevel(num2, reserveCount);
					foreach (int num6 in randomCityListByLevel)
					{
						if (this.LangHunLingYuCityDataExDict[num6] == null)
						{
							if (num4 < 0)
							{
								num4 = num6;
								num5 = 1;
							}
						}
						else
						{
							for (int j = 1; j <= num3; j++)
							{
								if (this.LangHunLingYuCityDataExDict[num6].Site[j] == (long)bhid)
								{
									num = -4005;
									return num;
								}
								if (num4 < 0)
								{
									if (this.LangHunLingYuCityDataExDict[num6].Site[j] == 0L)
									{
										num4 = num6;
										num5 = j;
									}
								}
							}
						}
					}
					if (num4 >= 0)
					{
						if (this.LangHunLingYuCityDataExDict[num4] == null)
						{
							this.LangHunLingYuCityDataExDict[num4] = new LangHunLingYuCityDataEx
							{
								CityId = num4,
								CityLevel = num2
							};
						}
						this.LangHunLingYuCityDataExDict[num4].Site[num5] = (long)bhid;
						LangHunLingYuCityDataEx cityDataEx = this.LangHunLingYuCityDataExDict[num4].Clone() as LangHunLingYuCityDataEx;
						this.Persistence.SaveCityData(cityDataEx);
						this.NotifyUpdateCityDataEx(cityDataEx);
					}
					else
					{
						num = -22;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				num = -11003;
			}
			return num;
		}

		private void CalcBangHuiCityLevel(HashSet<long> reCalcBangHuiLevelHashSet = null, bool broadcast = false)
		{
			lock (this.Mutex)
			{
				Dictionary<long, int> dictionary = new Dictionary<long, int>();
				if (null != reCalcBangHuiLevelHashSet)
				{
					foreach (long num in reCalcBangHuiLevelHashSet)
					{
						dictionary[num] = 0;
					}
				}
				else
				{
					foreach (long num in this.LangHunLingYuBangHuiDataExDict.Keys)
					{
						dictionary[num] = 0;
					}
				}
				foreach (LangHunLingYuCityDataEx langHunLingYuCityDataEx in this.LangHunLingYuCityDataExDict)
				{
					if (null != langHunLingYuCityDataEx)
					{
						long num = langHunLingYuCityDataEx.Site[0];
						int num2;
						if (num > 0L && dictionary.TryGetValue(num, out num2))
						{
							if (langHunLingYuCityDataEx.CityLevel > num2)
							{
								dictionary[num] = langHunLingYuCityDataEx.CityLevel;
							}
						}
					}
				}
				foreach (KeyValuePair<long, int> keyValuePair in dictionary)
				{
					LangHunLingYuBangHuiDataEx langHunLingYuBangHuiDataEx;
					if (this.LangHunLingYuBangHuiDataExDict.TryGetValue(keyValuePair.Key, out langHunLingYuBangHuiDataEx))
					{
						if (langHunLingYuBangHuiDataEx.Level != keyValuePair.Value)
						{
							langHunLingYuBangHuiDataEx.Level = keyValuePair.Value;
							this.Persistence.SaveBangHuiData(langHunLingYuBangHuiDataEx);
							if (broadcast)
							{
								this.Broadcast2GsAgent(new AsyncDataItem(6, new object[]
								{
									langHunLingYuBangHuiDataEx.Clone() as LangHunLingYuBangHuiDataEx
								}));
							}
						}
					}
				}
			}
		}

		private void UpdateOtherCityList(bool broadcast = false)
		{
			lock (this.Mutex)
			{
				Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
				foreach (LangHunLingYuCityDataEx langHunLingYuCityDataEx in this.LangHunLingYuCityDataExDict)
				{
					if (langHunLingYuCityDataEx != null && this.OtherCityLevelList[langHunLingYuCityDataEx.CityLevel] >= 0)
					{
						int num = langHunLingYuCityDataEx.Site.Count((long x) => x > 0L);
						if (num > 0)
						{
							List<int> list;
							if (!dictionary.TryGetValue(langHunLingYuCityDataEx.CityLevel, out list))
							{
								list = new List<int>();
								dictionary[langHunLingYuCityDataEx.CityLevel] = list;
							}
							list.Add(langHunLingYuCityDataEx.CityId);
						}
					}
				}
				string text = "";
				foreach (KeyValuePair<int, List<int>> keyValuePair in dictionary)
				{
					int num2 = this.OtherCityLevelList[keyValuePair.Key];
					int randomNumber = Global.GetRandomNumber(0, keyValuePair.Value.Count);
					int item = keyValuePair.Value[randomNumber];
					List<int> list;
					if (!this.OtherCityList.TryGetValue(keyValuePair.Key, out list))
					{
						list = new List<int>();
						this.OtherCityList[keyValuePair.Key] = list;
					}
					list.Clear();
					list.Add(item);
					if (keyValuePair.Value.Count >= 2)
					{
						if (randomNumber > 0)
						{
							list.Add(keyValuePair.Value[randomNumber - 1]);
						}
						else
						{
							list.Add(keyValuePair.Value[randomNumber + 1]);
						}
					}
					text += string.Format("Level={0}:{1},{2};", keyValuePair.Key, (keyValuePair.Value.Count >= 1) ? keyValuePair.Value[0] : 0, (keyValuePair.Value.Count >= 2) ? keyValuePair.Value[1] : 0);
				}
				LogManager.WriteLog(0, string.Format("重新计算他人城池展示列表{0}", text), null, true);
				if (broadcast)
				{
					this.NotifyUpdateOtherCityList(new Dictionary<int, List<int>>(this.OtherCityList));
				}
			}
		}

		public int GameFuBenComplete(LangHunLingYuStatisticalData data)
		{
			int result = 0;
			try
			{
				int cityId = data.CityId;
				int[] siteBhids = data.SiteBhids;
				lock (this.Mutex)
				{
					if (!this.Persistence.LangHunLingYuInitialized)
					{
						result = -11000;
						return result;
					}
					int num = (int)(TimeUtil.NowDateTime() - data.CompliteTime).TotalHours;
					bool flag2 = true;
					if (data.GameId > 0 && Math.Abs(num) >= 20)
					{
						flag2 = false;
						LogManager.WriteLog(2, string.Format("更新城池占领者,CityID={0},占领者={1},但时间已超过预期时间{2}小时,将不重置进攻者", data.CityId, data.SiteBhids[0], num), null, true);
					}
					HashSet<long> hashSet = new HashSet<long>();
					LangHunLingYuCityDataEx langHunLingYuCityDataEx = this.LangHunLingYuCityDataExDict[cityId];
					if (null == langHunLingYuCityDataEx)
					{
						langHunLingYuCityDataEx = (this.LangHunLingYuCityDataExDict[cityId] = new LangHunLingYuCityDataEx
						{
							CityId = cityId,
							CityLevel = this.GetCityLevelById(cityId)
						});
					}
					for (int i = 0; i < langHunLingYuCityDataEx.Site.Length; i++)
					{
						if (flag2 || i == 0)
						{
							LogManager.WriteLog(0, string.Format("更新城池信息,CityID={0},Site={1},old bhid={2},new bhid={3}", new object[]
							{
								langHunLingYuCityDataEx.CityId,
								i,
								langHunLingYuCityDataEx.Site[i],
								siteBhids[i]
							}), null, true);
							if (langHunLingYuCityDataEx.Site[i] > 0L && !hashSet.Contains(langHunLingYuCityDataEx.Site[i]))
							{
								hashSet.Add(langHunLingYuCityDataEx.Site[i]);
							}
							langHunLingYuCityDataEx.Site[i] = (long)siteBhids[i];
							if (langHunLingYuCityDataEx.Site[i] > 0L && !hashSet.Contains(langHunLingYuCityDataEx.Site[i]))
							{
								hashSet.Add(langHunLingYuCityDataEx.Site[i]);
							}
						}
					}
					this.CalcBangHuiCityLevel(hashSet, true);
					langHunLingYuCityDataEx = (langHunLingYuCityDataEx.Clone() as LangHunLingYuCityDataEx);
					this.Persistence.SaveCityData(langHunLingYuCityDataEx);
					this.NotifyUpdateCityDataEx(langHunLingYuCityDataEx);
					if (langHunLingYuCityDataEx.CityLevel == 10 && data.CityOwnerRoleData != null)
					{
						int num2 = 0;
						if (!this.LangHunLingYuAdmireDict.TryGetValue(data.rid, out num2))
						{
							this.LangHunLingYuAdmireDict[data.rid] = num2;
						}
						LangHunLingYuKingHist langHunLingYuKingHist = new LangHunLingYuKingHist
						{
							rid = data.rid,
							AdmireCount = num2,
							CompleteTime = data.CompliteTime,
							CityOwnerRoleData = data.CityOwnerRoleData
						};
						this.LangHunLingYuCityHistList.Add(langHunLingYuKingHist);
						this.Persistence.InsertCityOwnerHistory(langHunLingYuKingHist);
						this.NotifyUpdateCityOwnerHist(this.GetLangHunLingYuCityOwnerHist());
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = -11003;
			}
			return result;
		}

		private bool CreateLangHunLingYuGameFuBen(LangHunLingYuFuBenData fuBenData, DateTime stateEndTime)
		{
			try
			{
				int roleNum = 10;
				int nextGameId = this.Persistence.GetNextGameId();
				int num = 0;
				bool flag = ClientAgentManager.Instance().AssginKfFuben(this.LhlyGameType, (long)nextGameId, roleNum, out num);
				if (flag)
				{
					fuBenData.ServerId = num;
					fuBenData.GameId = nextGameId;
					fuBenData.EndTime = Global.NowTime.AddMinutes(65.0);
					this.AddLangHunLingYuGameFuBen(fuBenData);
					LogManager.WriteLog(0, string.Format("创建圣域争霸副本GameID={0},CityId={1},ServerID={2}", fuBenData.GameId, fuBenData.CityId, fuBenData.ServerId), null, true);
					this.Persistence.LogCreateYongZheFuBen(num, nextGameId, 0, roleNum);
					return true;
				}
				LogManager.WriteLog(2, string.Format("暂时没有可用的服务器可以给活动副本分配,稍后重试", new object[0]), null, true);
				return false;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		public LangHunLingYuFuBenData GetLangHunLingYuGameFuBenDataByCityId(int cityId)
		{
			LangHunLingYuFuBenData langHunLingYuFuBenData = null;
			try
			{
				int cityLevelById = this.GetCityLevelById(cityId);
				lock (this.Mutex)
				{
					LangHunLingYuCityDataEx langHunLingYuCityDataEx = this.LangHunLingYuCityDataExDict[cityId];
					if (null == langHunLingYuCityDataEx)
					{
						return null;
					}
					if (!this.LangHunLingYuFuBenDataDict.TryGetValue(langHunLingYuCityDataEx.GameId, out langHunLingYuFuBenData))
					{
						langHunLingYuFuBenData = new LangHunLingYuFuBenData();
						langHunLingYuFuBenData.CityId = cityId;
						if (!this.CreateLangHunLingYuGameFuBen(langHunLingYuFuBenData, Global.NowTime.AddHours(1.0)))
						{
							return null;
						}
						langHunLingYuCityDataEx.GameId = langHunLingYuFuBenData.GameId;
						this.Persistence.SaveCityData(langHunLingYuCityDataEx);
						this.NotifyUpdateCityDataEx(langHunLingYuCityDataEx);
					}
					langHunLingYuFuBenData.CityDataEx = (langHunLingYuCityDataEx.Clone() as LangHunLingYuCityDataEx);
					return langHunLingYuFuBenData;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				langHunLingYuFuBenData = null;
			}
			return langHunLingYuFuBenData;
		}

		public LangHunLingYuFuBenData GetLangHunLingYuGameFuBenData(int gameId)
		{
			LangHunLingYuFuBenData result = null;
			try
			{
				lock (this.Mutex)
				{
					if (!this.LangHunLingYuFuBenDataDict.TryGetValue(gameId, out result))
					{
						return null;
					}
					return result;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = null;
			}
			return result;
		}

		private void AddLangHunLingYuGameFuBen(LangHunLingYuFuBenData fuBenData)
		{
			lock (this.Mutex)
			{
				this.LangHunLingYuFuBenDataDict[fuBenData.GameId] = fuBenData;
			}
		}

		private void CheckOverTimeLangHunLingYuGameFuBen(DateTime now)
		{
			lock (this.Mutex)
			{
				List<LangHunLingYuFuBenData> list = new List<LangHunLingYuFuBenData>();
				foreach (LangHunLingYuFuBenData langHunLingYuFuBenData in this.LangHunLingYuFuBenDataDict.Values)
				{
					if (now > langHunLingYuFuBenData.EndTime)
					{
						list.Add(langHunLingYuFuBenData);
					}
				}
				foreach (LangHunLingYuFuBenData langHunLingYuFuBenData2 in list)
				{
					int gameId = langHunLingYuFuBenData2.GameId;
					if (this.LangHunLingYuFuBenDataDict.Remove(gameId))
					{
						langHunLingYuFuBenData2.State = 3;
						ClientAgentManager.Instance().RemoveKfFuben(this.LhlyGameType, langHunLingYuFuBenData2.ServerId, (long)langHunLingYuFuBenData2.GameId);
					}
				}
			}
		}

		private void Broadcast2GsAgent(AsyncDataItem item)
		{
			ClientAgentManager.Instance().BroadCastAsyncEvent(this.YzzcGameType, item, 0);
		}

		private void ChangeRoleState(KuaFuRoleData kuaFuRoleData, KuaFuRoleStates state)
		{
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
					ClientAgentManager.Instance().PostAsyncEvent(kuaFuRoleData.ServerId, this.YzzcGameType, evItem);
				}
			}
			catch (Exception ex)
			{
			}
		}

		private void NotifyFuBenRoleCount(YongZheZhanChangFuBenData fuBenData)
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
							ClientAgentManager.Instance().PostAsyncEvent(kuaFuRoleData.ServerId, this.YzzcGameType, evItem);
						}
					}
				}
			}
			catch
			{
			}
		}

		private void NotifyFuBenRoleEnterGame(YongZheZhanChangFuBenData fuBenData)
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
							ClientAgentManager.Instance().PostAsyncEvent(kuaFuRoleData.ServerId, this.YzzcGameType, evItem);
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		private void AssignGameFubenStep1(KuaFuRoleData kuaFuRoleData, long endStateTicks)
		{
			KuaFuFuBenRoleData kuaFuFuBenRoleData = new KuaFuFuBenRoleData
			{
				ServerId = kuaFuRoleData.ServerId,
				RoleId = kuaFuRoleData.RoleId,
				ZhanDouLi = kuaFuRoleData.ZhanDouLi
			};
			YongZheZhanChangGameFuBenPreAssignData yongZheZhanChangGameFuBenPreAssignData;
			if (!this.PreAssignGameFuBenDataDict.TryGetValue(kuaFuRoleData.GroupIndex, out yongZheZhanChangGameFuBenPreAssignData))
			{
				yongZheZhanChangGameFuBenPreAssignData = new YongZheZhanChangGameFuBenPreAssignData();
				this.PreAssignGameFuBenDataDict.Add(kuaFuRoleData.GroupIndex, yongZheZhanChangGameFuBenPreAssignData);
			}
			if (null == yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData)
			{
				yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData = new YongZheZhanChangFuBenData
				{
					GroupIndex = kuaFuRoleData.GroupIndex
				};
			}
			int num = yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData, false);
			if (num >= this.Persistence.MinEnterCount)
			{
				yongZheZhanChangGameFuBenPreAssignData.FullFuBenDataList.Add(yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData);
				yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData = null;
			}
			kuaFuRoleData.UpdateStateTime(0, 2, endStateTicks);
		}

		private void AssignGameFubenStep2()
		{
			List<int> list = this.PreAssignGameFuBenDataDict.Keys.Reverse<int>().ToList<int>();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				YongZheZhanChangGameFuBenPreAssignData yongZheZhanChangGameFuBenPreAssignData = null;
				YongZheZhanChangGameFuBenPreAssignData yongZheZhanChangGameFuBenPreAssignData2 = null;
				yongZheZhanChangGameFuBenPreAssignData2 = this.PreAssignGameFuBenDataDict[list[i]];
				if (i > 0)
				{
					yongZheZhanChangGameFuBenPreAssignData = this.PreAssignGameFuBenDataDict[list[i - 1]];
				}
				if (yongZheZhanChangGameFuBenPreAssignData != null && yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData != null)
				{
					int count2 = yongZheZhanChangGameFuBenPreAssignData2.FullFuBenDataList.Count;
					int count3 = yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData.RoleDict.Count;
					List<int> list2 = yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData.RoleDict.Keys.ToList<int>();
					if (count2 > 0)
					{
						for (int j = 0; j < count2; j++)
						{
							YongZheZhanChangFuBenData yongZheZhanChangFuBenData = yongZheZhanChangGameFuBenPreAssignData2.FullFuBenDataList[j];
							for (int k = j; k < count3; k += count2)
							{
								yongZheZhanChangFuBenData.AddKuaFuFuBenRoleData(yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData.RoleDict[list2[k]], true);
							}
						}
					}
					else if (null != yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData)
					{
						foreach (KuaFuFuBenRoleData kuaFuFuBenRoleData in yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData.RoleDict.Values)
						{
							if (yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData, false) >= this.Persistence.MinEnterCount)
							{
								YongZheZhanChangFuBenData remainFuBenData = new YongZheZhanChangFuBenData
								{
									GroupIndex = yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData.GroupIndex
								};
								yongZheZhanChangGameFuBenPreAssignData2.FullFuBenDataList.Add(yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData);
								yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData = remainFuBenData;
							}
						}
					}
					else
					{
						yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData = yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData;
						yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData.GroupIndex = list[i];
					}
					yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData = null;
				}
				if (yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData != null && i == count - 1 && yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData.GetFuBenRoleCount() > 0)
				{
					if (yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData.GetFuBenRoleCount() >= this.Persistence.MinEnterCount)
					{
						yongZheZhanChangGameFuBenPreAssignData2.FullFuBenDataList.Add(yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData);
					}
					else if (yongZheZhanChangGameFuBenPreAssignData2.FullFuBenDataList.Count <= 0)
					{
						yongZheZhanChangGameFuBenPreAssignData2.FullFuBenDataList.Add(yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData);
					}
					else
					{
						int count2 = yongZheZhanChangGameFuBenPreAssignData2.FullFuBenDataList.Count;
						int count3 = yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData.RoleDict.Count;
						List<int> list2 = yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData.RoleDict.Keys.ToList<int>();
						for (int j = 0; j < count2; j++)
						{
							YongZheZhanChangFuBenData yongZheZhanChangFuBenData = yongZheZhanChangGameFuBenPreAssignData2.FullFuBenDataList[j];
							for (int k = j; k < count3; k += count2)
							{
								yongZheZhanChangFuBenData.AddKuaFuFuBenRoleData(yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData.RoleDict[list2[k]], true);
							}
						}
					}
					yongZheZhanChangGameFuBenPreAssignData2.RemainFuBenData = null;
				}
			}
		}

		private bool AssignGameFubenStep3(DateTime stateEndTime)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			int num = 0;
			foreach (YongZheZhanChangGameFuBenPreAssignData yongZheZhanChangGameFuBenPreAssignData in this.PreAssignGameFuBenDataDict.Values)
			{
				foreach (YongZheZhanChangFuBenData yongZheZhanChangFuBenData in yongZheZhanChangGameFuBenPreAssignData.FullFuBenDataList)
				{
					if (yongZheZhanChangFuBenData.State < 2)
					{
						if (yongZheZhanChangFuBenData.GetFuBenRoleCount() > 0)
						{
							if (!this.CreateGameFuBenOnServer(yongZheZhanChangFuBenData, stateEndTime))
							{
								return false;
							}
							num++;
							if (num >= 15)
							{
								num = 0;
								DateTime dateTime2 = TimeUtil.NowDateTime();
								if (dateTime2 < dateTime)
								{
									Thread.Sleep((int)(dateTime - dateTime2).TotalMilliseconds);
								}
								else
								{
									dateTime = dateTime2.AddSeconds(1.0);
								}
							}
						}
					}
				}
				yongZheZhanChangGameFuBenPreAssignData.FullFuBenDataList.Clear();
				if (yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData != null && yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData.GetFuBenRoleCount() > 0)
				{
					if (yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData.State < 2)
					{
						if (!this.CreateGameFuBenOnServer(yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData, stateEndTime))
						{
							return false;
						}
					}
					yongZheZhanChangGameFuBenPreAssignData.RemainFuBenData = null;
				}
			}
			return true;
		}

		private bool CreateGameFuBenOnServer(YongZheZhanChangFuBenData fuBenData, DateTime stateEndTime)
		{
			try
			{
				int nextGameId = this.Persistence.GetNextGameId();
				int num = 0;
				bool flag = ClientAgentManager.Instance().AssginKfFuben(this.YzzcGameType, (long)nextGameId, fuBenData.GetFuBenRoleCount(), out num);
				if (flag)
				{
					fuBenData.ServerId = num;
					fuBenData.GameId = nextGameId;
					fuBenData.EndTime = Global.NowTime.AddMinutes(65.0);
					this.AddGameFuBen(fuBenData);
					fuBenData.SortFuBenRoleList();
					foreach (KuaFuFuBenRoleData kuaFuFuBenRoleData in fuBenData.RoleDict.Values)
					{
						KuaFuRoleKey key = KuaFuRoleKey.Get(kuaFuFuBenRoleData.ServerId, kuaFuFuBenRoleData.RoleId);
						KuaFuRoleData kuaFuRoleData;
						if (this.RoleIdKuaFuRoleDataDict.TryGetValue(key, out kuaFuRoleData))
						{
							LogManager.WriteLog(0, string.Format("通知活动副本GameID={0}的角色{1}准备进入ServerID={2}开始游戏", fuBenData.GameId, kuaFuRoleData.RoleId, fuBenData.ServerId), null, true);
							kuaFuRoleData.UpdateStateTime(fuBenData.GameId, 3, stateEndTime.Ticks);
						}
					}
					fuBenData.State = 2;
					this.NotifyFuBenRoleEnterGame(fuBenData);
					this.Persistence.LogCreateYongZheFuBen(num, nextGameId, 0, fuBenData.GetFuBenRoleCount());
					return true;
				}
				LogManager.WriteLog(2, string.Format("暂时没有可用的服务器可以给活动副本分配,稍后重试", new object[0]), null, true);
				return false;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		private void AddGameFuBen(YongZheZhanChangFuBenData fubenData)
		{
			this.FuBenDataDict[fubenData.GameId] = fubenData;
		}

		private void RemoveGameFuBen(YongZheZhanChangFuBenData fuBenData)
		{
			int gameId = fuBenData.GameId;
			YongZheZhanChangFuBenData yongZheZhanChangFuBenData;
			if (this.FuBenDataDict.TryRemove(gameId, out yongZheZhanChangFuBenData))
			{
				yongZheZhanChangFuBenData.State = 3;
			}
			ClientAgentManager.Instance().RemoveKfFuben(this.YzzcGameType, yongZheZhanChangFuBenData.ServerId, (long)yongZheZhanChangFuBenData.GameId);
			lock (fuBenData)
			{
				foreach (KuaFuFuBenRoleData kuaFuFuBenRoleData in fuBenData.RoleDict.Values)
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
				YongZheZhanChangFuBenData yongZheZhanChangFuBenData;
				if (this.FuBenDataDict.TryGetValue(gameId, out yongZheZhanChangFuBenData))
				{
					lock (yongZheZhanChangFuBenData)
					{
						yongZheZhanChangFuBenData.RemoveKuaFuFuBenRoleData(roleId);
						if (yongZheZhanChangFuBenData.CanRemove())
						{
							this.RemoveGameFuBen(yongZheZhanChangFuBenData);
						}
					}
				}
			}
		}

		public int ExecCommand(string[] args)
		{
			int result = 0;
			try
			{
				if (0 == string.Compare(args[0], "GameState", true))
				{
					if (args.Length >= 2)
					{
						int num = 5;
						int num2 = int.Parse(args[1]);
						if (args.Length >= 3)
						{
							int.TryParse(args[2], out num);
						}
						DateTime dateTime = TimeUtil.NowDateTime();
						lock (this.Mutex)
						{
							if (num2 == 2 && (dateTime - this.PrepareStartGameTime).TotalHours >= 1.0)
							{
								this.PrepareStartGameTime = dateTime;
								if (this.GameState > num2)
								{
									LogManager.WriteLog(0, "ExecCommand Set GameState to ignore" + num2, null, true);
									return -4;
								}
								this.GameState = num2;
								this.RunTimeGameType = num;
								int num3 = num;
								switch (num3)
								{
								case 5:
									this.Persistence.MinEnterCount = this.Persistence.YongZheZhanChangRoleCount;
									this.Persistence.ServerCapacityRate = 1;
									break;
								case 6:
									this.Persistence.MinEnterCount = this.Persistence.KuaFuBossRoleCount;
									this.Persistence.ServerCapacityRate = 2;
									break;
								default:
									if (num3 == 15)
									{
										this.Persistence.MinEnterCount = this.Persistence.KingOfBattleRoleCount;
										this.Persistence.ServerCapacityRate = 2;
									}
									break;
								}
								LogManager.WriteLog(0, "ExecCommand Set GameState to " + num2, null, true);
							}
						}
					}
				}
				else if (0 == string.Compare(args[0], "-settime", true))
				{
					if (KuaFuServerManager.EnableGMSetAllServerTime && args.Length >= 3)
					{
						string text = args[2];
						if (args.Length > 3)
						{
							text = text + " " + args[3];
						}
						ThreadPool.QueueUserWorkItem(delegate(object x)
						{
							Thread.Sleep(10000);
							string text2 = x as string;
							if (!string.IsNullOrEmpty(text2))
							{
								TimeUtil.SetTime(text2);
								LogManager.WriteLog(2, string.Format("GM命令修改时间#from={0},to={1}", TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"), text2), null, true);
							}
						}, text);
						this.Broadcast2GsAgent(new AsyncDataItem(9997, args));
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		private const double CheckGameFuBenInterval = 1000.0;

		private const double CheckRoleTimerProcInterval = 1.428;

		public static YongZheZhanChangService Instance = null;

		private object Mutex = new object();

		public readonly GameTypes YzzcGameType = 5;

		public readonly GameTypes LhlyGameType = 10;

		private DateTime CheckGameFuBenTime;

		private DateTime CheckRoleTimerProcTime;

		public YongZheZhanChangPersistence Persistence = YongZheZhanChangPersistence.Instance;

		public int[] OtherCityLevelList = new int[]
		{
			-1,
			0,
			-1,
			1,
			-1,
			2,
			3,
			-1,
			-1,
			-1,
			-1
		};

		public Dictionary<int, List<int>> OtherCityList = new Dictionary<int, List<int>>();

		public int ExistKuaFuFuBenCount = 0;

		private int EnterGameSecs = 3600;

		private int GameState = 1;

		private bool AssginGameFuBenComplete = false;

		private int RunTimeGameType;

		private DateTime PrepareStartGameTime = DateTime.MinValue;

		public ConcurrentDictionary<int, YongZheZhanChangFuBenData> FuBenDataDict = new ConcurrentDictionary<int, YongZheZhanChangFuBenData>(1, 4096);

		private SortedDictionary<int, YongZheZhanChangGameFuBenPreAssignData> PreAssignGameFuBenDataDict = new SortedDictionary<int, YongZheZhanChangGameFuBenPreAssignData>();

		private ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData> RoleIdKuaFuRoleDataDict = new ConcurrentDictionary<KuaFuRoleKey, KuaFuRoleData>();

		public ConcurrentDictionary<string, int> UserId2RoleIdActiveDict = new ConcurrentDictionary<string, int>(1, 16384);

		private ConcurrentDictionary<int, KuaFuMapRoleData> RoleId2KuaFuMapIdDict = new ConcurrentDictionary<int, KuaFuMapRoleData>();

		public Thread BackgroundThread;

		public static char[] WriteSpaceChars = new char[]
		{
			' '
		};

		public Dictionary<long, LangHunLingYuBangHuiDataEx> LangHunLingYuBangHuiDataExDict = new Dictionary<long, LangHunLingYuBangHuiDataEx>();

		public LangHunLingYuCityDataEx[] LangHunLingYuCityDataExDict = new LangHunLingYuCityDataEx[1024];

		public List<LangHunLingYuKingHist> LangHunLingYuCityHistList = new List<LangHunLingYuKingHist>();

		public Dictionary<int, int> LangHunLingYuAdmireDict = new Dictionary<int, int>();

		public Dictionary<int, LangHunLingYuFuBenData> LangHunLingYuFuBenDataDict = new Dictionary<int, LangHunLingYuFuBenData>();
	}
}
