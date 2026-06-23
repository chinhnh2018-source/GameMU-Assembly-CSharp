using System;
using System.Collections.Generic;
using System.ServiceModel;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace KF.Client
{
	public class KuaFuWorldClient : IManager2
	{
		public static KuaFuWorldClient getInstance()
		{
			return KuaFuWorldClient.instance;
		}

		public bool initialize()
		{
			return true;
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			this.CoreInterface = coreInterface;
			this.ClientInfo.PTID = GameManager.PTID;
			this.ClientInfo.ServerId = GameManager.KuaFuServerId;
			this.ClientInfo.GameType = 32;
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
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
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
				string runtimeVariable = this.CoreInterface.GetRuntimeVariable("KuaFuWorldUri", null);
				if (this.RemoteServiceUri != runtimeVariable)
				{
					this.RemoteServiceUri = runtimeVariable;
				}
				IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					if (this.ClientInfo.ClientId > 0)
					{
						if (KuaFuManager.KuaFuWorldKuaFuGameServer)
						{
							List<KuaFuServerInfo> kuaFuServerInfoData = kuaFuService.GetKuaFuServerInfoData(KuaFuManager.getInstance().GetServerInfoAsyncAge());
							KuaFuManager.getInstance().UpdateServerInfoList(kuaFuServerInfoData);
						}
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
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("KuaFuWorldUri", null);
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

		private IKuaFuWorld GetKuaFuService(bool noWait = false)
		{
			try
			{
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
					IKuaFuWorld kuaFuWorld;
					if (this.KuaFuService == null)
					{
						kuaFuWorld = (IKuaFuWorld)Activator.GetObject(typeof(IKuaFuWorld), this.RemoteServiceUri);
						if (null == kuaFuWorld)
						{
							return null;
						}
					}
					else
					{
						kuaFuWorld = this.KuaFuService;
					}
					int num = this.ClientInfo.ClientId;
					long num2 = TimeUtil.NOW();
					if (num <= 0 || Math.Abs(num2 - this.ClientInfo.LastInitClientTicks) > 12000L)
					{
						this.ClientInfo.LastInitClientTicks = num2;
						num = kuaFuWorld.InitializeClient(this.ClientInfo);
					}
					if (kuaFuWorld != null && (num != this.ClientInfo.ClientId || this.KuaFuService != kuaFuWorld))
					{
						lock (this.Mutex)
						{
							if (num > 0)
							{
								this.KuaFuService = kuaFuWorld;
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

		public void EventCallBackHandler(AsyncDataItem item)
		{
			try
			{
				int eventType = item.EventType;
				object[] args = item.Args;
				int num = eventType;
				if (num != 39)
				{
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
				}
				else
				{
					RebornManager.getInstance().OnChatListData(args[0] as byte[]);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public int ExecuteCommand(string cmd)
		{
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
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
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
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

		public int RegPTKuaFuRoleData(ref KuaFuWorldRoleData data)
		{
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.RegPTKuaFuRoleData(ref data);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		public int EnterPTKuaFuMap(int roleSourceServerId, int roleId, int ptid, int mapCode, int kuaFuLine, KuaFuServerLoginData kuaFuServerLoginData, out string signToken, out string signKey)
		{
			signToken = null;
			signKey = null;
			int num = -11000;
			int num2 = 0;
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					string[] array;
					int[] array2;
					num = kuaFuService.EnterPTKuaFuMap(roleSourceServerId, roleId, ptid, mapCode, kuaFuLine, ref signToken, ref signKey, ref num2, ref array, ref array2);
					if (num <= 0)
					{
						return num;
					}
					kuaFuServerLoginData.RoleId = roleId;
					kuaFuServerLoginData.ServerId = (KuaFuManager.KuaFuWorldKuaFuGameServer ? roleSourceServerId : GameManager.KuaFuServerId);
					kuaFuServerLoginData.GameType = 32;
					kuaFuServerLoginData.GameId = (long)mapCode;
					kuaFuServerLoginData.EndTicks = TimeUtil.UTCTicks();
					kuaFuServerLoginData.TargetServerID = num2;
					KuaFuServerInfo kuaFuServerInfo;
					if (array != null && array2 != null)
					{
						kuaFuServerLoginData.ServerIp = array[0];
						kuaFuServerLoginData.ServerPort = array2[0];
					}
					else if (KuaFuManager.getInstance().TryGetValue(num2, out kuaFuServerInfo))
					{
						kuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
						kuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return num;
		}

		public int CheckEnterWorldKuaFuSign(string worldRoleID, string token, out string signKey, out string[] ips, out int[] ports)
		{
			signKey = null;
			ips = null;
			ports = null;
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.CheckEnterWorldKuaFuSign(worldRoleID, token, ref signKey, ref ips, ref ports);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		public int Reborn_SetRoleData4Selector(int ptId, int roleId, byte[] bytes)
		{
			int result = 0;
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Reborn_SetRoleData4Selector(ptId, roleId, bytes);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public int Reborn_RoleReborn(int ptId, int roleId, string roleName, int level)
		{
			int result = -11;
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.Reborn_RoleReborn(ptId, roleId, roleName, level);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public RebornSyncData Reborn_SyncData(long ageRank, long ageBoss)
		{
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.Reborn_SyncData(ageRank, ageBoss);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public int Reborn_RebornOpt(int ptid, int rid, int optType, int param1, int param2, string param3)
		{
			int result = 0;
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Reborn_RebornOpt(ptid, rid, optType, param1, param2, param3);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public KFRebornRoleData Reborn_GetRebornRoleData(int ptId, int roleId)
		{
			try
			{
				IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KeyValuePair<int, int> key = new KeyValuePair<int, int>(ptId, roleId);
						KuaFuData<KFRebornRoleData> kuaFuData = null;
						if (!this.RebornRoleDataDict.TryGetValue(key, out kuaFuData))
						{
							kuaFuData = new KuaFuData<KFRebornRoleData>();
							this.RebornRoleDataDict[key] = kuaFuData;
						}
						KuaFuCmdData kuaFuCmdData = kuaFuService.Reborn_GetRebornRoleData(ptId, roleId, kuaFuData.Age);
						if (kuaFuCmdData == null || kuaFuCmdData.Age < 0L)
						{
							return null;
						}
						if (kuaFuCmdData != null && kuaFuCmdData.Age > kuaFuData.Age)
						{
							kuaFuData.Age = kuaFuCmdData.Age;
							if (null != kuaFuCmdData.Bytes0)
							{
								kuaFuData.V = DataHelper2.BytesToObject<KFRebornRoleData>(kuaFuCmdData.Bytes0, 0, kuaFuCmdData.Bytes0.Length);
							}
							if (null != kuaFuData.V)
							{
								this.RebornRoleDataDict[key] = kuaFuData;
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

		public int Reborn_ChangeName(int ptId, int roleId, string roleName)
		{
			int result = 0;
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.Reborn_ChangeName(ptId, roleId, roleName);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public int Reborn_PlatFormChat(List<KFPlatFormChat> chatList)
		{
			int result = 0;
			IKuaFuWorld kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					byte[] array = DataHelper.ObjectToBytes<List<KFPlatFormChat>>(chatList);
					kuaFuService.Reborn_PlatFormChat(this.ClientInfo.ServerId, array);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		private static KuaFuWorldClient instance = new KuaFuWorldClient();

		private object Mutex = new object();

		private object RemotingMutex = new object();

		private ICoreInterface CoreInterface = null;

		private IKuaFuWorld KuaFuService = null;

		private bool ClientInitialized = false;

		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		private string RemoteServiceUri = null;

		public Dictionary<KeyValuePair<int, int>, KuaFuData<KFRebornRoleData>> RebornRoleDataDict = new Dictionary<KeyValuePair<int, int>, KuaFuData<KFRebornRoleData>>();

		private DuplexChannelFactory<IKuaFuWorld> channelFactory;
	}
}
