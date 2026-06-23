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
using Tmsk.Contract.KuaFuData;

namespace KF.Client
{
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class TianTiClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		public static TianTiClient getInstance()
		{
			return TianTiClient.instance;
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
			this.ClientInfo.GameType = 2;
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
				string runtimeVariable = this.CoreInterface.GetRuntimeVariable("TianTiUri", null);
				if (this.RemoteServiceUri != runtimeVariable)
				{
					this.RemoteServiceUri = runtimeVariable;
				}
				ITianTiService kuaFuService = this.GetKuaFuService(false);
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
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("TianTiUri", null);
			lock (this.Mutex)
			{
				this.KuaFuService = null;
				this.RomoteServiceConnect = false;
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

		public bool IsKuaFuServerOK()
		{
			return this.RomoteServiceConnect;
		}

		private ITianTiService GetKuaFuService(bool noWait = false)
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
					ITianTiService tianTiService;
					if (this.KuaFuService == null)
					{
						tianTiService = (ITianTiService)Activator.GetObject(typeof(ITianTiService), this.RemoteServiceUri);
						if (null == tianTiService)
						{
							return null;
						}
					}
					else
					{
						tianTiService = this.KuaFuService;
					}
					int num = this.ClientInfo.ClientId;
					long num2 = TimeUtil.NOW();
					if (num <= 0 || Math.Abs(num2 - this.ClientInfo.LastInitClientTicks) > 12000L)
					{
						this.ClientInfo.LastInitClientTicks = num2;
						num = tianTiService.InitializeClient(this.ClientInfo);
					}
					if (!this.RomoteServiceConnect)
					{
						this.RomoteServiceConnect = true;
						LogManager.WriteLog(1000, "KuaFu5v5.InitializeClient Connected", null, false);
					}
					if (tianTiService != null && (num != this.ClientInfo.ClientId || this.KuaFuService != tianTiService))
					{
						lock (this.Mutex)
						{
							if (num > 0)
							{
								this.KuaFuService = tianTiService;
							}
							else
							{
								this.KuaFuService = null;
							}
							this.ClientInfo.ClientId = num;
							return tianTiService;
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
			ITianTiService kuaFuService = this.GetKuaFuService(false);
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
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
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
				if (num <= 38)
				{
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
						if (args.Length == 2)
						{
							int num2 = (int)args[0];
							int num3 = (int)args[1];
							this.CoreInterface.GetEventSourceInterface().fireEvent(new KuaFuFuBenRoleCountEvent(num2, num3), this.SceneType);
						}
						break;
					case 3:
						if (args.Length == 1)
						{
							KuaFuRoleData kuaFuRoleData = args[0] as KuaFuRoleData;
							if (null != kuaFuRoleData)
							{
								this.UpdateRoleData(kuaFuRoleData, kuaFuRoleData.RoleId);
								TianTiFuBenData kuaFuFuBenData = this.GetKuaFuFuBenData(kuaFuRoleData.GameId);
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
									this.CoreInterface.GetEventSourceInterface().fireEvent(new KuaFuNotifyEnterGameEvent(kuaFuServerLoginData), this.SceneType);
								}
							}
						}
						break;
					case 4:
						if (args.Length == 1)
						{
							int gameId = (int)args[0];
							TianTiManager.getInstance().CancleTianTiScene(gameId);
						}
						break;
					default:
						if (num != 20)
						{
							switch (num)
							{
							case 34:
								CompManager.getInstance().OnNoticeListData(args[0] as byte[]);
								break;
							case 35:
								CompManager.getInstance().OnChatListData(args[0] as byte[]);
								break;
							case 36:
								CompManager.getInstance().OnRefreshAllCompNpc((int)args[0]);
								break;
							case 37:
								CompBattleManager.getInstance().OnCompBattleReset();
								break;
							case 38:
								CompMineManager.getInstance().OnCompMineReset();
								break;
							}
						}
						else
						{
							ZhengDuoSyncData zhengDuoSyncData = args[0] as ZhengDuoSyncData;
							if (null != zhengDuoSyncData)
							{
								this._ZhengDuoSyncData = zhengDuoSyncData;
							}
						}
						break;
					}
				}
				else if (num <= 10028)
				{
					switch (num)
					{
					case 10009:
					{
						ZhengBaSupportLogData zhengBaSupportLogData = args[0] as ZhengBaSupportLogData;
						if (zhengBaSupportLogData != null && zhengBaSupportLogData.FromServerId != this.ClientInfo.ServerId)
						{
							this.CoreInterface.GetEventSourceInterface().fireEvent(new KFZhengBaSupportEvent(zhengBaSupportLogData), 36);
						}
						break;
					}
					case 10010:
						if (args.Length == 1)
						{
							ZhengBaPkLogData zhengBaPkLogData = args[0] as ZhengBaPkLogData;
							if (zhengBaPkLogData != null)
							{
								this.CoreInterface.GetEventSourceInterface().fireEvent(new KFZhengBaPkLogEvent(zhengBaPkLogData), 36);
							}
						}
						break;
					case 10011:
					{
						ZhengBaNtfEnterData zhengBaNtfEnterData = args[0] as ZhengBaNtfEnterData;
						KuaFuServerInfo kuaFuServerInfo;
						if (KuaFuManager.getInstance().TryGetValue(zhengBaNtfEnterData.ToServerId, out kuaFuServerInfo))
						{
							zhengBaNtfEnterData.ToServerIp = kuaFuServerInfo.Ip;
							zhengBaNtfEnterData.ToServerPort = kuaFuServerInfo.Port;
						}
						else
						{
							LogManager.WriteLog(2, string.Format("KuaFuEventTypes.ZhengBaNtfEnter not find kfserver={0}", zhengBaNtfEnterData.ToServerId), null, true);
						}
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFZhengBaNtfEnterEvent(zhengBaNtfEnterData), 36);
						break;
					}
					case 10012:
					{
						ZhengBaMirrorFightData zhengBaMirrorFightData = args[0] as ZhengBaMirrorFightData;
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFZhengBaMirrorFightEvent(zhengBaMirrorFightData), 36);
						break;
					}
					case 10013:
					{
						ZhengBaBulletinJoinData zhengBaBulletinJoinData = args[0] as ZhengBaBulletinJoinData;
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFZhengBaBulletinJoinEvent(zhengBaBulletinJoinData), 36);
						break;
					}
					case 10014:
						this.CoreInterface.GetEventSourceInterface().fireEvent(new CoupleArenaCanEnterEvent(args[0] as CoupleArenaCanEnterData), 38);
						break;
					default:
						if (num == 10028)
						{
							BHMatchNtfEnterData bhmatchNtfEnterData = args[0] as BHMatchNtfEnterData;
							this.CoreInterface.GetEventSourceInterface().fireEvent(new KFBHMatchNtfEnterData(bhmatchNtfEnterData), 45);
						}
						break;
					}
				}
				else if (num != 10033)
				{
					if (num == 10036)
					{
						KuaFu5v5FuBenData kuaFu5v5FuBenData = args[0] as KuaFu5v5FuBenData;
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFZorkBattleNtfEnterData(kuaFu5v5FuBenData), 57);
					}
				}
				else if (args.Length >= 1)
				{
					KuaFu5v5FuBenData kuaFu5v5FuBenData2 = args[0] as KuaFu5v5FuBenData;
					if (kuaFu5v5FuBenData2 != null && kuaFu5v5FuBenData2.State == 2)
					{
						foreach (KuaFuFuBenRoleData kuaFuFuBenRoleData in kuaFu5v5FuBenData2.RoleDict.Values)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(kuaFuFuBenRoleData.RoleId);
							if (null != gameClient)
							{
								KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(gameClient);
								if (null != clientKuaFuServerLoginData)
								{
									clientKuaFuServerLoginData.RoleId = kuaFuFuBenRoleData.RoleId;
									clientKuaFuServerLoginData.GameId = (long)kuaFu5v5FuBenData2.GameId;
									clientKuaFuServerLoginData.GameType = kuaFu5v5FuBenData2.GameType;
									clientKuaFuServerLoginData.EndTicks = kuaFu5v5FuBenData2.EndTime.Ticks;
									clientKuaFuServerLoginData.ServerId = this.ClientInfo.ServerId;
									clientKuaFuServerLoginData.ServerIp = kuaFu5v5FuBenData2.LoginInfo.KuaFuIP;
									clientKuaFuServerLoginData.ServerPort = kuaFu5v5FuBenData2.LoginInfo.KuaFuPort;
									clientKuaFuServerLoginData.ips = kuaFu5v5FuBenData2.LoginInfo.LocalIPs;
									clientKuaFuServerLoginData.ports = kuaFu5v5FuBenData2.LoginInfo.LocalPorts;
									KuaFuManager.getInstance().KuaFuSwitchServer(gameClient);
								}
							}
						}
					}
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

		public int TianTiSignUp(string userId, int roleId, int zoneId, int gameType, int groupIndex, int zhanDouLi)
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
						ITianTiService kuaFuService = this.GetKuaFuService(false);
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
			ITianTiService kuaFuService = this.GetKuaFuService(noWait);
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

		private TianTiFuBenData GetKuaFuFuBenData(int gameId)
		{
			TianTiFuBenData tianTiFuBenData = null;
			if (tianTiFuBenData == null)
			{
				ITianTiService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						tianTiFuBenData = kuaFuService.GetFuBenData(gameId);
					}
					catch (Exception ex)
					{
						tianTiFuBenData = null;
					}
				}
			}
			return tianTiFuBenData;
		}

		public int GetRoleKuaFuFuBenRoleCount(int roleId)
		{
			int result = 0;
			try
			{
				ITianTiService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					object obj = kuaFuService.GetRoleExtendData(this.ClientInfo.ServerId, roleId, 0);
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
				ITianTiService kuaFuService = this.GetKuaFuService(false);
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
			ITianTiService kuaFuService = this.GetKuaFuService(false);
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
			TianTiFuBenData kuaFuFuBenData = this.GetKuaFuFuBenData((int)kuaFuServerLoginData.GameId);
			if (kuaFuFuBenData != null && kuaFuFuBenData.State < 3)
			{
				if (kuaFuFuBenData.ServerId == GameManager.ServerId)
				{
					if (kuaFuFuBenData.RoleDict.ContainsKey(kuaFuServerLoginData.RoleId))
					{
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

		public int GetRoleBattleWhichSide(int gameId, int roleId)
		{
			TianTiFuBenData kuaFuFuBenData = this.GetKuaFuFuBenData(gameId);
			if (kuaFuFuBenData != null && kuaFuFuBenData.State < 3)
			{
				if (kuaFuFuBenData.ServerId == this.ClientInfo.ServerId)
				{
					KuaFuFuBenRoleData kuaFuFuBenRoleData;
					if (kuaFuFuBenData.RoleDict.TryGetValue(roleId, out kuaFuFuBenRoleData))
					{
						return kuaFuFuBenRoleData.Side;
					}
				}
			}
			return 0;
		}

		public TianTiRankData GetRankingData()
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					DateTime modifyTime;
					lock (this.Mutex)
					{
						modifyTime = this.RankData.ModifyTime;
					}
					TianTiRankData tianTiRankData = kuaFuService.GetRankingData(modifyTime);
					lock (this.Mutex)
					{
						if (tianTiRankData != null && tianTiRankData.ModifyTime > this.RankData.ModifyTime)
						{
							this.RankData = tianTiRankData;
						}
						tianTiRankData = new TianTiRankData();
						tianTiRankData.ModifyTime = this.RankData.ModifyTime;
						tianTiRankData.MaxPaiMingRank = this.RankData.MaxPaiMingRank;
						if (this.RankData.TianTiRoleInfoDataList != null && this.RankData.TianTiRoleInfoDataList.Count > 0)
						{
							tianTiRankData.TianTiRoleInfoDataList = new List<TianTiRoleInfoData>(this.RankData.TianTiRoleInfoDataList);
						}
						if (this.RankData.TianTiMonthRoleInfoDataList != null && this.RankData.TianTiMonthRoleInfoDataList.Count > 0)
						{
							tianTiRankData.TianTiMonthRoleInfoDataList = new List<TianTiRoleInfoData>(this.RankData.TianTiMonthRoleInfoDataList);
						}
						return tianTiRankData;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public void UpdateRoleInfoData(TianTiRoleInfoData data)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.UpdateRoleInfoData(data);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		public ZhengBaSyncData GetZhengBaRankData(ZhengBaSyncData lastSyncData)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.SyncZhengBaData(lastSyncData);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public int ZhengBaSupport(ZhengBaSupportLogData data)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhengBaSupport(data);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11003;
		}

		public int ZhengBaRequestEnter(int roleId, int gameId, EZhengBaEnterType enter)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhengBaRequestEnter(roleId, gameId, enter);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11003;
		}

		public int ZhengBaKuaFuLogin(int roleId, int gameId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhengBaKuaFuLogin(roleId, gameId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11003;
		}

		public List<ZhengBaNtfPkResultData> ZhengBaPkResult(int gameId, int winner, int FirstLeaveRoleId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhengBaPkResult(gameId, winner, FirstLeaveRoleId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public int CoupleArenaJoin(int roleId1, int roleId2, int serverId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleArenaJoin(roleId1, roleId2, serverId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return -11003;
				}
			}
			return -11001;
		}

		public int CoupleArenaQuit(int roleId1, int roleId2)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleArenaQuit(roleId1, roleId2);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return -11003;
				}
			}
			return -11001;
		}

		public CoupleArenaSyncData CoupleArenaSync(DateTime lastSyncTime)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleArenaSync(lastSyncTime);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return null;
				}
			}
			return null;
		}

		public int CoupleArenaPreDivorce(int roleId1, int roleId2)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(true);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleArenaPreDivorce(roleId1, roleId2);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return -11003;
				}
			}
			return -11001;
		}

		public CoupleArenaFuBenData GetFuBenData(long gameId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetCoupleFuBenData(gameId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return null;
				}
			}
			return null;
		}

		public CoupleArenaPkResultRsp CoupleArenaPkResult(CoupleArenaPkResultReq req)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleArenaPkResult(req);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return null;
				}
			}
			return null;
		}

		public int CoupleWishWishRole(CoupleWishWishRoleReq req)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleWishWishRole(req);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return -11003;
				}
			}
			return -11001;
		}

		public List<CoupleWishWishRecordData> CoupleWishGetWishRecord(int roleId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleWishGetWishRecord(roleId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return null;
				}
			}
			return null;
		}

		public CoupleWishSyncData CoupleWishSyncCenterData(DateTime oldThisWeek, DateTime oldLastWeek, DateTime oldStatue)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleWishSyncCenterData(oldThisWeek, oldLastWeek, oldStatue);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return null;
				}
			}
			return null;
		}

		public int CoupleWishPreDivorce(int man, int wife)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(true);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleWishPreDivorce(man, wife);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return -11003;
				}
			}
			return -11001;
		}

		public void CoupleWishReportCoupleStatue(CoupleWishReportStatueData req)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.CoupleWishReportCoupleStatue(req);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		public int CoupleWishAdmire(int fromRole, int fromZone, int admireType, int toCoupleId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(true);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleWishAdmire(fromRole, fromZone, admireType, toCoupleId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return -11003;
				}
			}
			return -11001;
		}

		public int CoupleWishJoinParty(int fromRole, int fromZone, int toCoupleId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CoupleWishJoinParty(fromRole, fromZone, toCoupleId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					return -11003;
				}
			}
			return -11001;
		}

		public ZhengDuoSyncData ZhengDuoSync(long age)
		{
			if (Math.Abs(TimeUtil.NOW() - age) > 20000L)
			{
				ITianTiService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						ZhengDuoSyncData zhengDuoSyncData = kuaFuService.ZhengDuoSync(this.ClientInfo.ServerId, age);
						if (null != zhengDuoSyncData)
						{
							this._ZhengDuoSyncData = zhengDuoSyncData;
						}
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			return this._ZhengDuoSyncData;
		}

		public int ZhengDuoSign(int bhid, int usedTime, int zoneId, string bhName, int bhLevel, long bhZhanLi)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhengDuoSign(this.ClientInfo.ServerId, bhid, usedTime, zoneId, bhName, bhLevel, bhZhanLi);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		public int ZhengDuoResult(int bhidSuccess, int[] bhids)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhengDuoResult(bhidSuccess, bhids);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		public int GmCommand(string[] args, byte[] data)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GmCommand(args, data);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		public ZhengDuoFuBenData GetZhengDuoFuBenDataByBhid(int bhid)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetZhengDuoFuBenDataByBhid(bhid);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public ZhengDuoFuBenData GetZhengDuoFuBenData(long gameId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetZhengDuoFuBenData(gameId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public BHMatchBHData GetBHDataByBhid_BHMatch(int type, int bhid)
		{
			try
			{
				ITianTiService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KuaFuData<BHMatchBHData> kuaFuData = null;
						if (type == 1)
						{
							if (!this.BHMatchBHDataDict_Gold.TryGetValue(bhid, out kuaFuData))
							{
								kuaFuData = new KuaFuData<BHMatchBHData>();
								this.BHMatchBHDataDict_Gold[bhid] = kuaFuData;
							}
						}
						else if (!this.BHMatchBHDataDict_Rookie.TryGetValue(bhid, out kuaFuData))
						{
							kuaFuData = new KuaFuData<BHMatchBHData>();
							this.BHMatchBHDataDict_Rookie[bhid] = kuaFuData;
						}
						KuaFuCmdData bhdataByBhid_BHMatch = kuaFuService.GetBHDataByBhid_BHMatch(type, bhid, kuaFuData.Age);
						if (bhdataByBhid_BHMatch == null || bhdataByBhid_BHMatch.Age < 0L)
						{
							return null;
						}
						if (bhdataByBhid_BHMatch != null && bhdataByBhid_BHMatch.Age > kuaFuData.Age)
						{
							kuaFuData.Age = bhdataByBhid_BHMatch.Age;
							if (null != bhdataByBhid_BHMatch.Bytes0)
							{
								kuaFuData.V = DataHelper2.BytesToObject<BHMatchBHData>(bhdataByBhid_BHMatch.Bytes0, 0, bhdataByBhid_BHMatch.Bytes0.Length);
							}
							if (null != kuaFuData.V)
							{
								if (type == 1)
								{
									this.BHMatchBHDataDict_Gold[bhid] = kuaFuData;
								}
								else
								{
									this.BHMatchBHDataDict_Rookie[bhid] = kuaFuData;
								}
							}
						}
						return kuaFuData.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		public BHMatchSyncData SyncData_BHMatch(long ageRank, long agePKInfo, long ageChampion)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.SyncData_BHMatch(ageRank, agePKInfo, ageChampion);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public string GetKuaFuGameState_BHMatch(int bhid)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetKuaFuGameState_BHMatch(bhid);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public bool CheckRookieJoinLast_BHMatch(int bhid)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CheckRookieJoinLast_BHMatch(bhid);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return false;
		}

		public int RookieSignUp_BHMatch(int bhid, int zoneid_bh, string bhname, int rid, string rname, int zoneid_r)
		{
			int result = -11;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.RookieSignUp_BHMatch(bhid, zoneid_bh, bhname, rid, rname, zoneid_r);
				}
				catch (Exception ex)
				{
					result = -11003;
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public BHMatchFuBenData GetFuBenDataByBhid_BHMatch(int bhid)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetFuBenDataByBhid_BHMatch(bhid);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public BHMatchFuBenData GetFuBenDataByGameId_BHMatch(int GameId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetFuBenDataByGameId_BHMatch(GameId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public int GameFuBenComplete_BHMatch(BangHuiMatchStatisticalData data)
		{
			int result = -11;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.GameFuBenComplete_BHMatch(data);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public int RemoveBangHui_BHMatch(int bhid)
		{
			int result = -11;
			int serverId = this.ClientInfo.ServerId;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.RemoveBangHui_BHMatch(bhid);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public void SwitchLastGoldBH_GM()
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				kuaFuService.SwitchLastGoldBH_GM();
			}
		}

		public CompSyncData Comp_SyncData(long ageComp, long ageRankJX, long ageRankJXL, long ageRankBD, long ageRankBJF, long ageRankMJF)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.Comp_SyncData(ageComp, ageRankJX, ageRankJXL, ageRankBD, ageRankBJF, ageRankMJF);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public KFCompRoleData Comp_GetCompRoleData(int roleId)
		{
			try
			{
				ITianTiService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KuaFuData<KFCompRoleData> kuaFuData = null;
						if (!this.CompRoleDataDict.TryGetValue(roleId, out kuaFuData))
						{
							kuaFuData = new KuaFuData<KFCompRoleData>();
							this.CompRoleDataDict[roleId] = kuaFuData;
						}
						KuaFuCmdData kuaFuCmdData = kuaFuService.Comp_GetCompRoleData(roleId, kuaFuData.Age);
						if (kuaFuCmdData == null || kuaFuCmdData.Age < 0L)
						{
							return null;
						}
						if (kuaFuCmdData != null && kuaFuCmdData.Age > kuaFuData.Age)
						{
							kuaFuData.Age = kuaFuCmdData.Age;
							if (null != kuaFuCmdData.Bytes0)
							{
								kuaFuData.V = DataHelper2.BytesToObject<KFCompRoleData>(kuaFuCmdData.Bytes0, 0, kuaFuCmdData.Bytes0.Length);
							}
							if (null != kuaFuData.V)
							{
								this.CompRoleDataDict[roleId] = kuaFuData;
							}
						}
						return kuaFuData.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		public int Comp_ChangeName(int roleId, string roleName)
		{
			int result = 0;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Comp_ChangeName(roleId, roleName);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public int Comp_JoinComp_Repair(int roleId, int zoneId, string roleName, int compType, int battleJiFen)
		{
			int result = -11;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.Comp_JoinComp_Repair(roleId, zoneId, roleName, compType, battleJiFen);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public int Comp_JoinComp(int roleId, int zoneId, string roleName, int compType)
		{
			int result = -11;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.Comp_JoinComp(roleId, zoneId, roleName, compType);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public int Comp_CompOpt(int compType, int optType, int param1, int param2)
		{
			int result = 0;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Comp_CompOpt(compType, optType, param1, param2);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public int Comp_SetBulletin(int compType, string bulletin)
		{
			int result = 0;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Comp_SetBulletin(compType, bulletin);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public int Comp_BroadCastCompNotice(List<KFCompNotice> noticeList)
		{
			int result = 0;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					byte[] array = DataHelper.ObjectToBytes<List<KFCompNotice>>(noticeList);
					kuaFuService.Comp_BroadCastCompNotice(this.ClientInfo.ServerId, array);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public int Comp_CompChat(List<KFCompChat> chatList)
		{
			int result = 0;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					byte[] array = DataHelper.ObjectToBytes<List<KFCompChat>>(chatList);
					kuaFuService.Comp_CompChat(this.ClientInfo.ServerId, array);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public int Comp_SetRoleData4Selector(int roleId, byte[] bytes)
		{
			int result = 0;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Comp_SetRoleData4Selector(roleId, bytes);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public int Comp_UpdateMapRoleNum(int mapCode, int roleNum)
		{
			int result = 0;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Comp_UpdateMapRoleNum(mapCode, roleNum);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public void Comp_UpdateKuaFuMapClientCount(int gameType, CompFuBenData fubenItem)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Comp_UpdateFuBenMapRoleNum(gameType, fubenItem);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		public void Comp_UpdateStrongholdData(int cityID, List<CompStrongholdData> shDataList)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Comp_UpdateStrongholdData(cityID, shDataList);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		public int Comp_GameFuBenRoleChangeState(int gameType, int serverId, int cityID, int roleId, int zhiwu, int state)
		{
			int result = -11;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.Comp_GameFuBenRoleChangeState(gameType, serverId, cityID, roleId, zhiwu, state);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public CompFuBenData Comp_GetKuaFuFuBenData(int gameType, int cityId)
		{
			try
			{
				ITianTiService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KeyValuePair<int, int> key = new KeyValuePair<int, int>(gameType, cityId);
						KuaFuData<CompFuBenData> kuaFuData = null;
						if (!this.CompFuBenDataDict.TryGetValue(key, out kuaFuData))
						{
							kuaFuData = new KuaFuData<CompFuBenData>();
							this.CompFuBenDataDict[key] = kuaFuData;
						}
						KuaFuCmdData kuaFuCmdData = kuaFuService.Comp_GetKuaFuFuBenData(gameType, cityId, kuaFuData.Age);
						if (kuaFuCmdData == null || kuaFuCmdData.Age < 0L)
						{
							return null;
						}
						if (kuaFuCmdData != null && kuaFuCmdData.Age > kuaFuData.Age)
						{
							kuaFuData.Age = kuaFuCmdData.Age;
							if (null != kuaFuCmdData.Bytes0)
							{
								kuaFuData.V = DataHelper2.BytesToObject<CompFuBenData>(kuaFuCmdData.Bytes0, 0, kuaFuCmdData.Bytes0.Length);
							}
							if (null == kuaFuData.V)
							{
								kuaFuData.V = new CompFuBenData();
							}
						}
						return kuaFuData.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		private void ClearOverTimeFuBen(DateTime now)
		{
			lock (this.Mutex)
			{
				List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
				foreach (KeyValuePair<KeyValuePair<int, int>, KuaFuData<CompFuBenData>> keyValuePair in this.CompFuBenDataDict)
				{
					if (keyValuePair.Value.V.EndTime < now)
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (KeyValuePair<int, int> key in list)
				{
					this.CompFuBenDataDict.Remove(key);
				}
			}
		}

		public int ChangeRoleSex(int serverID, int rid, int newSex, string data)
		{
			int result = -11000;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11003;
				}
			}
			return result;
		}

		public int ChangeRoleOcc(int serverID, int rid, int newOcc, string data)
		{
			int result = -11000;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11003;
				}
			}
			return result;
		}

		public int CreateZhanDui(TianTi5v5ZhanDuiData pData)
		{
			int result = -11000;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.CreateZhanDui(this.ClientInfo.ServerId, pData);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11003;
				}
			}
			return result;
		}

		public int UpdateZhanDuiXuanYan(long teamID, string xuanYan)
		{
			int result = -11000;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.UpdateZhanDuiXuanYan(teamID, xuanYan);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11003;
				}
			}
			return result;
		}

		public int ExcuteGMCmd(int serverID, int rid, string[] cmd)
		{
			int result = -11000;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.ExcuteGMCmd(serverID, rid, cmd);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11003;
				}
			}
			return result;
		}

		public int DeleteZhanDui(int serverID, int roleid, int teamID)
		{
			int result = -11000;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.DeleteZhanDui(serverID, roleid, teamID);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11003;
				}
			}
			return result;
		}

		public int ZhanDuiRoleSignUp(int serverId, int gameType, int teamID, long zhanLi, int groupIndex)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhanDuiRoleSignUp(serverId, gameType, teamID, zhanLi, groupIndex);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		public int ZhanDuiRoleChangeState(int serverId, int zhanDuiID, int roleId, int state, int gameID = 0)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhanDuiRoleChangeState(serverId, zhanDuiID, roleId, state, gameID);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		public KuaFu5v5FuBenData ZhanDuiGetFuBenData(int gameId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhanDuiGetFuBenData(gameId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public TianTi5v5RankData ZhanDuiGetRankingData(DateTime modifyTime)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.ZhanDuiGetRankingData(modifyTime);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public int UpdateZhanDuiData(TianTi5v5ZhanDuiData data, ZhanDuiDataModeTypes modeType)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.UpdateZhanDuiData(data, modeType);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		public ZorkBattleSyncData SyncData_ZorkBattle(long gsTicks, long ageRank)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.SyncData_ZorkBattle(gsTicks, ageRank);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public string GetKuaFuGameState_ZorkBattle(int zhanduiID)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetKuaFuGameState_ZorkBattle(zhanduiID);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public int SignUp_ZorkBattle(int zhanduiID, int serverID)
		{
			int result = -11;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.SignUp_ZorkBattle(zhanduiID, serverID);
				}
				catch (Exception ex)
				{
					result = -11003;
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public int GameFuBenComplete_ZorkBattle(ZorkBattleStatisticalData data)
		{
			int result = -11;
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.GameFuBenComplete_ZorkBattle(data);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public KuaFu5v5FuBenData GetFuBenDataByGameId_ZorkBattle(int GameId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetFuBenDataByGameId_ZorkBattle(GameId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public KuaFu5v5FuBenData GetFuBenDataByZhanDuiId_ZorkBattle(int ZhanDuiId)
		{
			ITianTiService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetFuBenDataByZhanDuiId_ZorkBattle(ZhanDuiId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		private static TianTiClient instance = new TianTiClient();

		private object Mutex = new object();

		private object RemotingMutex = new object();

		private ICoreInterface CoreInterface = null;

		private ITianTiService KuaFuService = null;

		private bool ClientInitialized = false;

		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		public int SceneType = 26;

		private int CurrentRequestCount = 0;

		private int MaxRequestCount = 50;

		private Dictionary<int, KuaFuRoleData> RoleId2RoleDataDict = new Dictionary<int, KuaFuRoleData>();

		private Dictionary<int, int> RoleId2KuaFuStateDict = new Dictionary<int, int>();

		private TianTiRankData RankData = new TianTiRankData();

		private string RemoteServiceUri = null;

		private DateTime NextClearFuBenTime;

		public Dictionary<int, KuaFuData<BHMatchBHData>> BHMatchBHDataDict_Gold = new Dictionary<int, KuaFuData<BHMatchBHData>>();

		public Dictionary<int, KuaFuData<BHMatchBHData>> BHMatchBHDataDict_Rookie = new Dictionary<int, KuaFuData<BHMatchBHData>>();

		public Dictionary<int, KuaFuData<BHMatchRoleData>> BHMatchRoleDataDict_Gold = new Dictionary<int, KuaFuData<BHMatchRoleData>>();

		public Dictionary<int, KuaFuData<BHMatchRoleData>> BHMatchRoleDataDict_Rookie = new Dictionary<int, KuaFuData<BHMatchRoleData>>();

		public Dictionary<int, KuaFuData<KFCompRoleData>> CompRoleDataDict = new Dictionary<int, KuaFuData<KFCompRoleData>>();

		public Dictionary<KeyValuePair<int, int>, KuaFuData<CompFuBenData>> CompFuBenDataDict = new Dictionary<KeyValuePair<int, int>, KuaFuData<CompFuBenData>>();

		private bool RomoteServiceConnect = true;

		private DuplexChannelFactory<ITianTiService> channelFactory;

		private ZhengDuoSyncData _ZhengDuoSyncData;
	}
}
