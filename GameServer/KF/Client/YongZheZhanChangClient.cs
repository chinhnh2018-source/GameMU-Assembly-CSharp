using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using Server.Tools;
using Tmsk.Contract;

namespace KF.Client
{
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class YongZheZhanChangClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		public static YongZheZhanChangClient getInstance()
		{
			return YongZheZhanChangClient.instance;
		}

		public bool initialize()
		{
			return true;
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			this.CoreInterface = coreInterface;
			this.ClientInfo.PTID = GameManager.PTID;
			this.ClientInfo.ServerId = GameManager.ServerId;
			this.ClientInfo.GameType = 5;
			this.ClientInfo.Token = this.CoreInterface.GetLocalAddressIPs();
			return true;
		}

		public bool startup()
		{
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

		public void ExecuteEventCallBackAsync(object state)
		{
			AsyncDataItem[] array = state as AsyncDataItem[];
			if (array != null && array.Length > 0)
			{
				foreach (AsyncDataItem item in array)
				{
					this.EventCallBackHandler(item);
				}
			}
		}

		public void UpdateKuaFuMapClientCount(Dictionary<int, int> dict)
		{
			lock (this.Mutex)
			{
				this.ClientInfo.MapClientCountDict = dict;
			}
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.UpdateKuaFuMapClientCount(this.ClientInfo.ServerId, dict);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		public void TimerProc(object sender, EventArgs e)
		{
			try
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				if (this.NextClearFuBenTime < dateTime)
				{
					this.NextClearFuBenTime = dateTime.AddHours(1.0);
					this.ClearOverTimeFuBen(dateTime);
				}
				string runtimeVariable = this.CoreInterface.GetRuntimeVariable("YongZheZhanChangUri", null);
				if (this.RemoteServiceUri != runtimeVariable)
				{
					this.RemoteServiceUri = runtimeVariable;
				}
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					if (this.ClientInfo.ClientId > 0)
					{
						AsyncDataItem[] clientCacheItems = kuaFuService.GetClientCacheItems(this.ClientInfo.ServerId);
						if (clientCacheItems != null && clientCacheItems.Length > 0)
						{
							this.ExecuteEventCallBackAsync(clientCacheItems);
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.ResetKuaFuService();
			}
		}

		private void CloseConnection()
		{
			this.ClientInfo.ClientId = 0;
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("YongZheZhanChangUri", null);
			lock (this.Mutex)
			{
				this.KuaFuService = null;
			}
		}

		private void OnConnectionClose(object sender, EventArgs e)
		{
			this.CloseConnection();
		}

		private void ResetKuaFuService()
		{
			this.CloseConnection();
		}

		private IYongZheZhanChangService GetKuaFuService(bool noWait = false)
		{
			try
			{
				if (KuaFuManager.KuaFuWorldKuaFuGameServer)
				{
					return null;
				}
				lock (this.Mutex)
				{
					if (string.IsNullOrEmpty(this.RemoteServiceUri))
					{
						return null;
					}
					if (this.KuaFuService == null && noWait)
					{
						return null;
					}
				}
				lock (this.RemotingMutex)
				{
					IYongZheZhanChangService yongZheZhanChangService;
					if (this.KuaFuService == null)
					{
						yongZheZhanChangService = (IYongZheZhanChangService)Activator.GetObject(typeof(IYongZheZhanChangService), this.RemoteServiceUri);
						if (null == yongZheZhanChangService)
						{
							return null;
						}
					}
					else
					{
						yongZheZhanChangService = this.KuaFuService;
					}
					int num = this.ClientInfo.ClientId;
					long num2 = TimeUtil.NOW();
					if (num <= 0 || Math.Abs(num2 - this.ClientInfo.LastInitClientTicks) > 12000L)
					{
						this.ClientInfo.LastInitClientTicks = num2;
						num = yongZheZhanChangService.InitializeClient(this.ClientInfo);
					}
					if (yongZheZhanChangService != null && (num != this.ClientInfo.ClientId || this.KuaFuService != yongZheZhanChangService))
					{
						lock (this.Mutex)
						{
							if (num > 0)
							{
								this.KuaFuService = yongZheZhanChangService;
							}
							else
							{
								this.KuaFuService = null;
							}
							this.ClientInfo.ClientId = num;
							return this.KuaFuService;
						}
					}
					return this.KuaFuService;
				}
			}
			catch (Exception ex)
			{
				this.ResetKuaFuService();
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		public int UpdateRoleData(KuaFuRoleData kuaFuRoleData, int roleId = 0)
		{
			int num = 0;
			int result;
			if (kuaFuRoleData == null)
			{
				result = num;
			}
			else
			{
				roleId = kuaFuRoleData.RoleId;
				lock (this.Mutex)
				{
					if (kuaFuRoleData.State == 0)
					{
						this.RemoveRoleData(kuaFuRoleData.RoleId);
						return 0;
					}
					this.RoleId2RoleDataDict[roleId] = kuaFuRoleData;
					this.RoleId2KuaFuStateDict[roleId] = kuaFuRoleData.State;
				}
				result = num;
			}
			return result;
		}

		public int RoleChangeState(int serverId, int rid, int state)
		{
			int result = -11;
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.RoleChangeState(serverId, rid, state);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public int GameFuBenChangeState(int gameId, GameFuBenState state, DateTime time)
		{
			int result = -11000;
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					lock (this.Mutex)
					{
						YongZheZhanChangFuBenData yongZheZhanChangFuBenData;
						if (this.FuBenDataDict.TryGetValue(gameId, out yongZheZhanChangFuBenData))
						{
							yongZheZhanChangFuBenData.State = state;
						}
					}
					result = kuaFuService.GameFuBenChangeState(gameId, state, time);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11003;
				}
			}
			return result;
		}

		public int GetNewFuBenSeqId()
		{
			int result;
			if (null != this.CoreInterface)
			{
				result = this.CoreInterface.GetNewFuBenSeqId();
			}
			else
			{
				result = -11;
			}
			return result;
		}

		public object GetDataFromClientServer(int dataType, params object[] args)
		{
			return null;
		}

		public void EventCallBackHandler(AsyncDataItem item)
		{
			try
			{
				int eventType = item.EventType;
				object[] args = item.Args;
				int num = eventType;
				switch (num)
				{
				case 0:
				case 1:
					if (args.Length == 1)
					{
						KuaFuRoleData kuaFuRoleData = args[0] as KuaFuRoleData;
						if (null != kuaFuRoleData)
						{
							this.UpdateRoleData(kuaFuRoleData, kuaFuRoleData.RoleId);
						}
					}
					break;
				case 2:
				case 4:
				case 5:
					break;
				case 3:
					if (args.Length == 1)
					{
						KuaFuRoleData kuaFuRoleData = args[0] as KuaFuRoleData;
						if (null != kuaFuRoleData)
						{
							this.UpdateRoleData(kuaFuRoleData, kuaFuRoleData.RoleId);
							YongZheZhanChangFuBenData kuaFuFuBenData = this.GetKuaFuFuBenData(kuaFuRoleData.GameId);
							if (kuaFuFuBenData != null && kuaFuFuBenData.State == 2)
							{
								KuaFuServerLoginData kuaFuServerLoginData = new KuaFuServerLoginData
								{
									RoleId = kuaFuRoleData.RoleId,
									GameType = kuaFuRoleData.GameType,
									GameId = (long)kuaFuRoleData.GameId,
									EndTicks = kuaFuRoleData.StateEndTicks
								};
								kuaFuServerLoginData.ServerId = this.ClientInfo.ServerId;
								KuaFuServerInfo kuaFuServerInfo;
								if (KuaFuManager.getInstance().TryGetValue(kuaFuFuBenData.ServerId, out kuaFuServerInfo))
								{
									kuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
									kuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
								}
								GameTypes gameType = kuaFuRoleData.GameType;
								switch (gameType)
								{
								case 5:
									this.CoreInterface.GetEventSourceInterface().fireEvent(new KuaFuNotifyEnterGameEvent(kuaFuServerLoginData), 27);
									break;
								case 6:
									this.CoreInterface.GetEventSourceInterface().fireEvent(new KuaFuNotifyEnterGameEvent(kuaFuServerLoginData), 31);
									break;
								default:
									if (gameType == 15)
									{
										this.CoreInterface.GetEventSourceInterface().fireEvent(new KuaFuNotifyEnterGameEvent(kuaFuServerLoginData), 39);
									}
									break;
								}
							}
						}
					}
					break;
				case 6:
					if (args.Length == 1)
					{
						LangHunLingYuBangHuiDataEx langHunLingYuBangHuiDataEx = args[0] as LangHunLingYuBangHuiDataEx;
						this.CoreInterface.GetEventSourceInterface().fireEvent(new NotifyLhlyBangHuiDataGameEvent(langHunLingYuBangHuiDataEx), 35);
					}
					break;
				case 7:
					if (args.Length == 1)
					{
						LangHunLingYuCityDataEx langHunLingYuCityDataEx = args[0] as LangHunLingYuCityDataEx;
						this.CoreInterface.GetEventSourceInterface().fireEvent(new NotifyLhlyCityDataGameEvent(langHunLingYuCityDataEx), 35);
					}
					break;
				case 8:
					if (args.Length == 1)
					{
						Dictionary<int, List<int>> dictionary = args[0] as Dictionary<int, List<int>>;
						this.CoreInterface.GetEventSourceInterface().fireEvent(new NotifyLhlyOtherCityListGameEvent(dictionary), 35);
					}
					break;
				case 9:
					if (args.Length == 1)
					{
						List<LangHunLingYuKingHist> list = args[0] as List<LangHunLingYuKingHist>;
						this.CoreInterface.GetEventSourceInterface().fireEvent(new NotifyLhlyCityOwnerHistGameEvent(list), 35);
					}
					break;
				case 10:
					if (args.Length == 2)
					{
						int num2 = (int)args[0];
						int num3 = (int)args[1];
						this.CoreInterface.GetEventSourceInterface().fireEvent(new NotifyLhlyCityOwnerAdmireGameEvent(num2, num3), 35);
					}
					break;
				default:
					if (num == 9997)
					{
						if (GMCommands.EnableGMSetAllServerTime && item.Args.Length == 4)
						{
							string[] array = new string[item.Args.Length];
							for (int i = 0; i < array.Length; i++)
							{
								array[i] = (item.Args[i] as string);
								if (string.IsNullOrEmpty(array[i]))
								{
									return;
								}
							}
							if (array[0] == "-settime")
							{
								GMCommands.GMSetTime(null, array, false);
							}
						}
					}
					break;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public int OnRoleChangeState(int roleId, int state, int age)
		{
			lock (this.Mutex)
			{
				KuaFuRoleData kuaFuRoleData;
				if (!this.RoleId2RoleDataDict.TryGetValue(roleId, out kuaFuRoleData))
				{
					return -1;
				}
				if (age > kuaFuRoleData.Age)
				{
					kuaFuRoleData.State = state;
				}
			}
			return 0;
		}

		public int YongZheZhanChangSignUp(string userId, int roleId, int zoneId, int gameType, int groupIndex, int zhanDouLi)
		{
			int result;
			if (string.IsNullOrEmpty(userId) || roleId <= 0)
			{
				result = -20;
			}
			else
			{
				userId = userId.ToUpper();
				int num = Interlocked.Increment(ref this.CurrentRequestCount);
				try
				{
					if (num < this.MaxRequestCount)
					{
						lock (this.Mutex)
						{
							KuaFuRoleData kuaFuRoleData;
							if (this.RoleId2RoleDataDict.TryGetValue(roleId, out kuaFuRoleData))
							{
								if (kuaFuRoleData.ServerId != this.ClientInfo.ServerId)
								{
									return -11;
								}
							}
						}
						IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
						if (null == kuaFuService)
						{
							return -11001;
						}
						try
						{
							IGameData gameData = new IGameData
							{
								ZhanDouLi = zhanDouLi
							};
							int num2 = kuaFuService.RoleSignUp(this.ClientInfo.ServerId, userId, zoneId, roleId, gameType, groupIndex, gameData);
						}
						catch (Exception ex)
						{
							this.ResetKuaFuService();
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
				finally
				{
					Interlocked.Decrement(ref this.CurrentRequestCount);
				}
				result = 1;
			}
			return result;
		}

		public int ChangeRoleState(int roleId, KuaFuRoleStates state, bool noWait = false)
		{
			int num = -11;
			KuaFuRoleData kuaFuRoleData = null;
			int serverId = this.ClientInfo.ServerId;
			lock (this.Mutex)
			{
				if (this.RoleId2RoleDataDict.TryGetValue(roleId, out kuaFuRoleData))
				{
					serverId = kuaFuRoleData.ServerId;
				}
			}
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(noWait);
			if (null != kuaFuService)
			{
				try
				{
					num = kuaFuService.RoleChangeState(serverId, roleId, state);
					if (num >= 0)
					{
						lock (this.Mutex)
						{
							if (this.RoleId2RoleDataDict.TryGetValue(roleId, out kuaFuRoleData))
							{
								kuaFuRoleData.State = num;
							}
						}
						if (null != kuaFuRoleData)
						{
							this.UpdateRoleData(kuaFuRoleData, 0);
						}
					}
				}
				catch (Exception ex)
				{
					num = -11003;
				}
			}
			return num;
		}

		public YongZheZhanChangFuBenData GetKuaFuFuBenData(int gameId)
		{
			YongZheZhanChangFuBenData yongZheZhanChangFuBenData = null;
			lock (this.Mutex)
			{
				if (this.FuBenDataDict.TryGetValue(gameId, out yongZheZhanChangFuBenData))
				{
					return yongZheZhanChangFuBenData;
				}
			}
			if (yongZheZhanChangFuBenData == null)
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						yongZheZhanChangFuBenData = kuaFuService.GetFuBenData(gameId);
						if (null != yongZheZhanChangFuBenData)
						{
							lock (this.Mutex)
							{
								this.FuBenDataDict[gameId] = yongZheZhanChangFuBenData;
							}
						}
					}
					catch (Exception ex)
					{
						yongZheZhanChangFuBenData = null;
					}
				}
			}
			return yongZheZhanChangFuBenData;
		}

		private void ClearOverTimeFuBen(DateTime now)
		{
			lock (this.Mutex)
			{
				List<int> list = new List<int>();
				foreach (KeyValuePair<int, YongZheZhanChangFuBenData> keyValuePair in this.FuBenDataDict)
				{
					if (keyValuePair.Value.EndTime < now)
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (int key in list)
				{
					this.FuBenDataDict.Remove(key);
				}
			}
		}

		public int GetKuaFuRoleState(int roleId)
		{
			int result = 0;
			try
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					object obj = kuaFuService.GetRoleExtendData(this.ClientInfo.ServerId, roleId, 2);
					if (null != obj)
					{
						result = (int)obj;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		public int GameFuBenRoleChangeState(int roleId, int state, int serverId = 0, int gameId = 0)
		{
			try
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					if (serverId <= 0 || gameId <= 0)
					{
						KuaFuRoleData kuaFuRoleData;
						if (!this.RoleId2RoleDataDict.TryGetValue(roleId, out kuaFuRoleData))
						{
							return 0;
						}
						serverId = kuaFuRoleData.ServerId;
						gameId = kuaFuRoleData.GameId;
					}
					return this.KuaFuService.GameFuBenRoleChangeState(serverId, roleId, gameId, state);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return 0;
		}

		public void RemoveRoleData(int roleId)
		{
			lock (this.Mutex)
			{
				this.RoleId2RoleDataDict.Remove(roleId);
				this.RoleId2KuaFuStateDict.Remove(roleId);
			}
		}

		public KuaFuRoleData GetKuaFuRoleDataFromServer(int serverId, int roleId)
		{
			KuaFuRoleData kuaFuRoleData = null;
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuRoleData = kuaFuService.GetKuaFuRoleData(serverId, roleId);
					this.UpdateRoleData(kuaFuRoleData, 0);
				}
				catch (Exception ex)
				{
					kuaFuRoleData = null;
				}
			}
			return kuaFuRoleData;
		}

		public bool KuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			YongZheZhanChangFuBenData kuaFuFuBenData = this.GetKuaFuFuBenData((int)kuaFuServerLoginData.GameId);
			if (kuaFuFuBenData != null && kuaFuFuBenData.State < 3)
			{
				if (kuaFuFuBenData.ServerId == GameManager.ServerId)
				{
					if (kuaFuFuBenData.RoleDict.ContainsKey(kuaFuServerLoginData.RoleId))
					{
						kuaFuServerLoginData.FuBenSeqId = kuaFuFuBenData.SequenceId;
						KuaFuRoleData kuaFuRoleDataFromServer = this.GetKuaFuRoleDataFromServer(kuaFuServerLoginData.ServerId, kuaFuServerLoginData.RoleId);
						if (kuaFuRoleDataFromServer.GameId == kuaFuFuBenData.GameId)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public void PushGameResultData(object data)
		{
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.UpdateStatisticalData(new AsyncDataItem(9999, new object[]
					{
						data
					}));
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		public int ExecuteCommand(string cmd)
		{
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ExecuteCommand(cmd);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11003;
		}

		public object GetKuaFuLineDataList(int mapCode)
		{
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					AsyncDataItem kuaFuLineDataList = kuaFuService.GetKuaFuLineDataList(mapCode);
					if (kuaFuLineDataList != null && kuaFuLineDataList.Args != null && kuaFuLineDataList.Args.Length > 0)
					{
						return kuaFuLineDataList.Args[0];
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public int EnterKuaFuMap(int roleId, int mapCode, int kuaFuLine, int roleSourceServerId, KuaFuServerLoginData kuaFuServerLoginData)
		{
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					int num = kuaFuService.EnterKuaFuMap(this.ClientInfo.ServerId, roleId, mapCode, kuaFuLine);
					if (num > 0)
					{
						kuaFuServerLoginData.RoleId = roleId;
						kuaFuServerLoginData.ServerId = roleSourceServerId;
						kuaFuServerLoginData.GameType = 7;
						kuaFuServerLoginData.GameId = (long)mapCode;
						KuaFuServerInfo kuaFuServerInfo;
						if (KuaFuManager.getInstance().TryGetValue(num, out kuaFuServerInfo))
						{
							kuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
							kuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
							return num;
						}
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11003;
		}

		public bool CanEnterKuaFuMap(KuaFuServerLoginData kuaFuServerLoginData)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType((int)kuaFuServerLoginData.GameId);
			bool result;
			if (54 == mapSceneType)
			{
				result = true;
			}
			else
			{
				KuaFuMapRoleData kuaFuMapRoleData = YongZheZhanChangClient.getInstance().GetKuaFuMapRoleData(kuaFuServerLoginData.RoleId);
				if (kuaFuMapRoleData == null || kuaFuMapRoleData.KuaFuServerId != GameManager.ServerId || (long)kuaFuMapRoleData.KuaFuMapCode != kuaFuServerLoginData.GameId)
				{
					LogManager.WriteLog(2, string.Format("{0}不具有进入跨服地图{1}的资格", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		public KuaFuMapRoleData GetKuaFuMapRoleData(int roleId)
		{
			IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetKuaFuMapRoleData(roleId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public int LangHunLingYuSignUp(string bhName, int bhid, int zoneId, int gameType, int groupIndex, int zhanDouLi)
		{
			int result = -11000;
			try
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.LangHunLingYuSignUp(bhName, bhid, zoneId, gameType, groupIndex, zhanDouLi);
					}
					catch (Exception ex)
					{
						result = -11000;
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				result = -11000;
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		public int GameFuBenComplete(LangHunLingYuStatisticalData data)
		{
			int result = -11000;
			try
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.GameFuBenComplete(data);
					}
					catch (Exception ex)
					{
						result = -11000;
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				result = -11000;
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		public bool LangHunLingYunAdmire(int rid)
		{
			try
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						return kuaFuService.LangHunLingYuAdmaire(rid);
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		public LangHunLingYuFuBenData GetLangHunLingYuGameFuBenData(int gameId)
		{
			try
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						return kuaFuService.GetLangHunLingYuGameFuBenData(gameId);
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		public bool LangHunLingYuKuaFuLoginData(int roleId, int cityId, int gameId, KuaFuServerLoginData kuaFuServerLoginData)
		{
			try
			{
				IYongZheZhanChangService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						LangHunLingYuFuBenData langHunLingYuFuBenData;
						lock (this.Mutex)
						{
							if (!this.LangHunLingYuFuBenDataDict.TryGetValue(gameId, out langHunLingYuFuBenData))
							{
								langHunLingYuFuBenData = null;
							}
						}
						if (null == langHunLingYuFuBenData)
						{
							langHunLingYuFuBenData = kuaFuService.GetLangHunLingYuGameFuBenDataByCityId(cityId);
						}
						if (null != langHunLingYuFuBenData)
						{
							kuaFuServerLoginData.RoleId = roleId;
							kuaFuServerLoginData.GameId = (long)langHunLingYuFuBenData.GameId;
							kuaFuServerLoginData.GameType = 10;
							kuaFuServerLoginData.EndTicks = langHunLingYuFuBenData.EndTime.Ticks;
							kuaFuServerLoginData.ServerId = this.ClientInfo.ServerId;
							KuaFuServerInfo kuaFuServerInfo;
							if (KuaFuManager.getInstance().TryGetValue(langHunLingYuFuBenData.ServerId, out kuaFuServerInfo))
							{
								kuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
								kuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
								return true;
							}
						}
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		private static YongZheZhanChangClient instance = new YongZheZhanChangClient();

		private object Mutex = new object();

		private object RemotingMutex = new object();

		private ICoreInterface CoreInterface = null;

		private IYongZheZhanChangService KuaFuService = null;

		private bool ClientInitialized = false;

		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		public int SceneType = 27;

		private int CurrentRequestCount = 0;

		private int MaxRequestCount = 50;

		private Dictionary<int, KuaFuRoleData> RoleId2RoleDataDict = new Dictionary<int, KuaFuRoleData>();

		private Dictionary<int, int> RoleId2KuaFuStateDict = new Dictionary<int, int>();

		private Dictionary<int, YongZheZhanChangFuBenData> FuBenDataDict = new Dictionary<int, YongZheZhanChangFuBenData>();

		private Dictionary<int, LangHunLingYuFuBenData> LangHunLingYuFuBenDataDict = new Dictionary<int, LangHunLingYuFuBenData>();

		private DateTime NextClearFuBenTime;

		private string RemoteServiceUri = null;

		private DuplexChannelFactory<IYongZheZhanChangService> channelFactory;
	}
}
