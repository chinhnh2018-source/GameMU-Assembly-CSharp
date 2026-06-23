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
	public class HuanYingSiYuanClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		public static HuanYingSiYuanClient getInstance()
		{
			return HuanYingSiYuanClient.instance;
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
			this.ClientInfo.GameType = 1;
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
				string runtimeVariable = this.CoreInterface.GetRuntimeVariable("HuanYingSiYuanUri", null);
				if (this.RemoteServiceUri != runtimeVariable)
				{
					this.RemoteServiceUri = runtimeVariable;
				}
				IKuaFuService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					if (this.ClientInfo.ClientId > 0)
					{
						List<KuaFuServerInfo> kuaFuServerInfoData = kuaFuService.GetKuaFuServerInfoData(KuaFuManager.getInstance().GetServerInfoAsyncAge());
						KuaFuManager.getInstance().UpdateServerInfoList(kuaFuServerInfoData);
						AsyncData clientCacheItems = kuaFuService.GetClientCacheItems2(this.ClientInfo.ServerId, TimeUtil.NOW());
						long num = TimeUtil.NOW();
						long num2 = num - clientCacheItems.RequestTicks;
						if (num2 < 200L)
						{
							if (TimeUtil.AsyncNetTicks(clientCacheItems.RequestTicks, clientCacheItems.ServerTicks))
							{
								LogManager.WriteLog(-1, string.Format("时间漂移#local={0},server={1}", clientCacheItems.RequestTicks, clientCacheItems.ServerTicks), null, true);
							}
						}
						AsyncDataItem[] itemList = clientCacheItems.ItemList;
						if (itemList != null && itemList.Length > 0)
						{
							this.ExecuteEventCallBackAsync(itemList);
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
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("HuanYingSiYuanUri", null);
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

		private IKuaFuService GetKuaFuService(bool noWait = false)
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
					IKuaFuService kuaFuService;
					if (this.KuaFuService == null)
					{
						kuaFuService = (IKuaFuService)Activator.GetObject(typeof(IKuaFuService), this.RemoteServiceUri);
						if (null == kuaFuService)
						{
							return null;
						}
					}
					else
					{
						kuaFuService = this.KuaFuService;
					}
					int num = this.ClientInfo.ClientId;
					long num2 = TimeUtil.NOW();
					if (num <= 0 || Math.Abs(num2 - this.ClientInfo.LastInitClientTicks) > 12000L)
					{
						this.ClientInfo.LastInitClientTicks = num2;
						num = kuaFuService.InitializeClient(this.ClientInfo);
					}
					if (kuaFuService != null && (num != this.ClientInfo.ClientId || this.KuaFuService != kuaFuService))
					{
						lock (this.Mutex)
						{
							if (num > 0)
							{
								this.KuaFuService = kuaFuService;
							}
							else
							{
								this.KuaFuService = null;
							}
							this.ClientInfo.ClientId = num;
							return kuaFuService;
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
				int num2 = 0;
				int serverId = 0;
				lock (this.Mutex)
				{
					if (kuaFuRoleData.State == 0)
					{
						this.RemoveRoleData(kuaFuRoleData.RoleId);
						return 0;
					}
					string userId = kuaFuRoleData.UserId;
					KuaFuRoleData kuaFuRoleData2;
					if (this.UserId2RoleDataDict.TryGetValue(userId, out kuaFuRoleData2) && kuaFuRoleData2.RoleId != roleId)
					{
						num2 = kuaFuRoleData2.RoleId;
						int gameType = kuaFuRoleData2.GameType;
						int groupIndex = kuaFuRoleData2.GroupIndex;
						serverId = kuaFuRoleData2.ServerId;
					}
					if (kuaFuRoleData2 != kuaFuRoleData)
					{
						this.UserId2RoleDataDict[userId] = kuaFuRoleData;
						this.RoleId2RoleDataDict[roleId] = kuaFuRoleData;
						this.RoleId2KuaFuStateDict[roleId] = kuaFuRoleData.State;
					}
				}
				if (num2 > 0)
				{
					this.RoleChangeState(serverId, num2, 0);
					this.RemoveRoleData(num2);
				}
				result = num;
			}
			return result;
		}

		public int RoleChangeState(int serverId, int rid, int state)
		{
			int result = -11;
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
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
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
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
				if (num <= 9996)
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
								HuanYingSiYuanFuBenData kuaFuFuBenData = this.GetKuaFuFuBenData(kuaFuRoleData.GameId);
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
									else
									{
										LogManager.WriteLog(2, string.Format("服务器列表中无法找到serverid={0}的IP和端口信息", kuaFuFuBenData.ServerId), null, true);
									}
									this.CoreInterface.GetEventSourceInterface().fireEvent(new KuaFuNotifyEnterGameEvent(kuaFuServerLoginData), this.SceneType);
								}
							}
						}
						break;
					default:
						if (num == 9996)
						{
							if (args.Length == 1)
							{
								GMCmdData cmdData = args[0] as GMCmdData;
								GVoiceManager.getInstance().UpdateGVoicePriority(cmdData, true);
							}
						}
						break;
					}
				}
				else if (num != 10015)
				{
					if (num == 10029)
					{
						KuaFuLueDuoNtfEnterData data = args[0] as KuaFuLueDuoNtfEnterData;
						KuaFuLueDuoManager.getInstance().HandleNtfEnterEvent(data);
					}
				}
				else if (args != null && args.Length == 2)
				{
					KuaFuManager.getInstance().UpdateServerInfoList(args[1] as List<KuaFuServerInfo>);
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

		public int HuanYingSiYuanSignUp(string userId, int roleId, int zoneId, int gameType, int groupIndex, int zhanDouLi)
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
						IKuaFuService kuaFuService = this.GetKuaFuService(false);
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
			IKuaFuService kuaFuService = this.GetKuaFuService(noWait);
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

		private HuanYingSiYuanFuBenData GetKuaFuFuBenData(int gameId)
		{
			HuanYingSiYuanFuBenData huanYingSiYuanFuBenData = null;
			if (huanYingSiYuanFuBenData == null)
			{
				IKuaFuService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						huanYingSiYuanFuBenData = kuaFuService.GetFuBenData(gameId);
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
						huanYingSiYuanFuBenData = null;
					}
				}
			}
			return huanYingSiYuanFuBenData;
		}

		public int GetRoleKuaFuFuBenRoleCount(int roleId)
		{
			int result = 0;
			try
			{
				IKuaFuService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					int roleExtendData = kuaFuService.GetRoleExtendData(this.ClientInfo.ServerId, roleId, 0);
					bool flag = 1 == 0;
					result = roleExtendData;
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
				IKuaFuService kuaFuService = this.GetKuaFuService(false);
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
				KuaFuRoleData kuaFuRoleData;
				if (this.RoleId2RoleDataDict.TryGetValue(roleId, out kuaFuRoleData))
				{
					this.UserId2RoleDataDict.Remove(kuaFuRoleData.UserId);
				}
				this.RoleId2RoleDataDict.Remove(roleId);
				this.RoleId2KuaFuStateDict.Remove(roleId);
			}
		}

		public KuaFuRoleData GetKuaFuRoleDataFromServer(int serverId, int roleId)
		{
			KuaFuRoleData kuaFuRoleData = null;
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
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
			HuanYingSiYuanFuBenData kuaFuFuBenData = this.GetKuaFuFuBenData((int)kuaFuServerLoginData.GameId);
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
			HuanYingSiYuanFuBenData kuaFuFuBenData = this.GetKuaFuFuBenData(gameId);
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

		public int UseGiftCode(string ptid, string uid, string rid, string channel, string codeno, string appid, int zoneid, ref string giftid)
		{
			try
			{
				IKuaFuService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						return kuaFuService.UseGiftCode(ptid, uid, rid, channel, codeno, appid, zoneid, ref giftid);
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
			return -11000;
		}

		public void BroadcastGMCmdData(GMCmdData data, int serverFlag)
		{
			try
			{
				IKuaFuService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						kuaFuService.BroadcastGMCmdData(data, serverFlag);
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public KuaFuLueDuoJingJiaResult JingJia_KuaFuLueDuo(int bhid, int zoneid_bh, string bhname, int ziJin, int serverId, int oldZiJin)
		{
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.JingJia_KuaFuLueDuo(bhid, zoneid_bh, bhname, ziJin, serverId, oldZiJin);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return -11000;
		}

		public KuaFuLueDuoFuBenData GetFuBenDataByServerId_KuaFuLueDuo(int serverId)
		{
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetFuBenDataByServerId_KuaFuLueDuo(serverId);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return null;
		}

		public KuaFuLueDuoFuBenData GetFuBenDataByGameId_KuaFuLueDuo(long gameId)
		{
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetFuBenDataByGameId_KuaFuLueDuo(gameId);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return null;
		}

		public byte[] GetRoleData_KuaFuLueDuo(long rid)
		{
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetRoleData_KuaFuLueDuo(rid);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return null;
		}

		public KuaFuLueDuoSyncData SyncData_KuaFuLueDuo(KuaFuLueDuoSyncData SyncData)
		{
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					byte[] array = kuaFuService.SyncData_KuaFuLueDuo(DataHelper.ObjectToBytes<KuaFuLueDuoSyncData>(SyncData));
					if (null != array)
					{
						return DataHelper.BytesToObject<KuaFuLueDuoSyncData>(array, 0, array.Length);
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public KuaFuLueDuoBHData GetBHDataByBhid_KuaFuLueDuo(int bhid)
		{
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					lock (this.Mutex)
					{
						KuaFuData<KuaFuLueDuoBHData> kuaFuData = null;
						if (!this.KuaFuLueDuoBHDataDict.TryGetValue(bhid, out kuaFuData))
						{
							kuaFuData = new KuaFuData<KuaFuLueDuoBHData>();
							this.KuaFuLueDuoBHDataDict[bhid] = kuaFuData;
						}
						KuaFuCmdData bhdataByBhid_KuaFuLueDuo = kuaFuService.GetBHDataByBhid_KuaFuLueDuo(bhid, kuaFuData.Age);
						if (bhdataByBhid_KuaFuLueDuo == null || bhdataByBhid_KuaFuLueDuo.Age < 0L)
						{
							return null;
						}
						if (bhdataByBhid_KuaFuLueDuo != null && bhdataByBhid_KuaFuLueDuo.Age > kuaFuData.Age)
						{
							kuaFuData.Age = bhdataByBhid_KuaFuLueDuo.Age;
							if (null != bhdataByBhid_KuaFuLueDuo.Bytes0)
							{
								kuaFuData.V = DataHelper2.BytesToObject<KuaFuLueDuoBHData>(bhdataByBhid_KuaFuLueDuo.Bytes0, 0, bhdataByBhid_KuaFuLueDuo.Bytes0.Length);
							}
							if (null != kuaFuData.V)
							{
								this.KuaFuLueDuoBHDataDict[bhid] = kuaFuData;
							}
						}
						return kuaFuData.V;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return null;
		}

		public int GameFuBenComplete_KuaFuLueDuo(KuaFuLueDuoStatisticalData data)
		{
			IKuaFuService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GameFuBenComplete_KuaFuLueDuo(data);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		private static HuanYingSiYuanClient instance = new HuanYingSiYuanClient();

		private object Mutex = new object();

		private object RemotingMutex = new object();

		private ICoreInterface CoreInterface = null;

		private IKuaFuService KuaFuService = null;

		private bool ClientInitialized = false;

		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		public int SceneType = 25;

		public GameTypes GameType = 1;

		private int CurrentRequestCount = 0;

		private int MaxRequestCount = 50;

		private Dictionary<int, KuaFuRoleData> RoleId2RoleDataDict = new Dictionary<int, KuaFuRoleData>();

		private Dictionary<string, KuaFuRoleData> UserId2RoleDataDict = new Dictionary<string, KuaFuRoleData>();

		private Dictionary<int, int> RoleId2KuaFuStateDict = new Dictionary<int, int>();

		public Dictionary<int, KuaFuData<KuaFuLueDuoBHData>> KuaFuLueDuoBHDataDict = new Dictionary<int, KuaFuData<KuaFuLueDuoBHData>>();

		private string RemoteServiceUri = null;

		private DuplexChannelFactory<IKuaFuService> channelFactory;
	}
}
