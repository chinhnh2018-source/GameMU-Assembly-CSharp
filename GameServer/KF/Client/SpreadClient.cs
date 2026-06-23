using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
	public class SpreadClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		public static SpreadClient getInstance()
		{
			return SpreadClient.instance;
		}

		public bool initialize()
		{
			return true;
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			this._CoreInterface = coreInterface;
			this._ClientInfo.PTID = GameManager.PTID;
			this._ClientInfo.ServerId = GameManager.ServerId;
			this._ClientInfo.GameType = 9;
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

		public void TimerProc(object sender, EventArgs e)
		{
			try
			{
				string runtimeVariable = this._CoreInterface.GetRuntimeVariable("SpreadUri", null);
				if (this._RemoteServiceUri != runtimeVariable)
				{
					this._RemoteServiceUri = runtimeVariable;
				}
				ISpreadService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					if (this._ClientInfo.ClientId > 0)
					{
						AsyncDataItem[] clientCacheItems = kuaFuService.GetClientCacheItems(this._ClientInfo.ServerId);
						if (clientCacheItems != null && clientCacheItems.Length > 0)
						{
							this.ExecuteEventCallBackAsync(clientCacheItems);
						}
					}
				}
				this.CheckSpreadData();
			}
			catch (Exception ex)
			{
				this.ResetKuaFuService();
			}
		}

		private void CheckSpreadData()
		{
			if (this._RoleId2KFSpreadDataDict != null && this._RoleId2KFSpreadDataDict.Count > 0)
			{
				if (!(TimeUtil.NowDateTime() < this._checkSpreadDataTime))
				{
					this._checkSpreadDataTime = TimeUtil.NowDateTime().AddSeconds(43200.0);
					lock (this._RoleId2KFSpreadDataDict)
					{
						List<int> list = (from info in this._RoleId2KFSpreadDataDict.Values
						where info.LogTime <= TimeUtil.NowDateTime().AddSeconds(-43200.0)
						select info.RoleID).ToList<int>();
						foreach (int key in list)
						{
							KFSpreadData kfspreadData;
							this._RoleId2KFSpreadDataDict.TryRemove(key, out kfspreadData);
						}
					}
				}
			}
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

		private void CloseConnection()
		{
			this._ClientInfo.ClientId = 0;
			this._RemoteServiceUri = this._CoreInterface.GetRuntimeVariable("SpreadUri", null);
			lock (this._Mutex)
			{
				this._KuaFuService = null;
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

		private ISpreadService GetKuaFuService(bool noWait = false)
		{
			try
			{
				if (KuaFuManager.KuaFuWorldKuaFuGameServer)
				{
					return null;
				}
				lock (this._Mutex)
				{
					if (string.IsNullOrEmpty(this._RemoteServiceUri))
					{
						return null;
					}
					if (this._KuaFuService == null && noWait)
					{
						return null;
					}
				}
				lock (this._RemotingMutex)
				{
					ISpreadService spreadService;
					if (this._KuaFuService == null)
					{
						spreadService = (ISpreadService)Activator.GetObject(typeof(ISpreadService), this._RemoteServiceUri);
						if (null == spreadService)
						{
							return null;
						}
					}
					else
					{
						spreadService = this._KuaFuService;
					}
					int num = this._ClientInfo.ClientId;
					long num2 = TimeUtil.NOW();
					if (num <= 0 || Math.Abs(num2 - this._ClientInfo.LastInitClientTicks) > 12000L)
					{
						this._ClientInfo.LastInitClientTicks = num2;
						num = spreadService.InitializeClient(this._ClientInfo);
					}
					if (spreadService != null && (num != this._ClientInfo.ClientId || this._KuaFuService != spreadService))
					{
						lock (this._Mutex)
						{
							if (num > 0)
							{
								this._KuaFuService = spreadService;
							}
							else
							{
								this._KuaFuService = null;
							}
							this._ClientInfo.ClientId = num;
							return spreadService;
						}
					}
					return this._KuaFuService;
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
				if (num == 10008)
				{
					if (args.Length == 5)
					{
						int num2 = (int)args[0];
						int num3 = (int)args[1];
						int num4 = (int)args[2];
						int num5 = (int)args[3];
						int num6 = (int)args[4];
						KFSpreadData kfspreadData;
						if (this._RoleId2KFSpreadDataDict.TryGetValue(num3, out kfspreadData))
						{
							lock (kfspreadData)
							{
								kfspreadData.CountRole = num4;
								kfspreadData.CountVip = num5;
								kfspreadData.CountLevel = num6;
								kfspreadData.UpdateLogtime();
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifySpreadCountGameEvent(num2, num3, num4, num5, num6), 10002);
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

		public object GetDataFromClientServer(int dataType, params object[] args)
		{
			return null;
		}

		public int GetNewFuBenSeqId()
		{
			return 0;
		}

		public int UpdateRoleData(KuaFuRoleData kuaFuRoleData, int roleId = 0)
		{
			return 0;
		}

		public int OnRoleChangeState(int roleId, int state, int age)
		{
			return 0;
		}

		public int SpreadSign(int zoneID, int roleID)
		{
			try
			{
				lock (this._Mutex)
				{
					KFSpreadData kfspreadData;
					if (this._RoleId2KFSpreadDataDict.TryGetValue(roleID, out kfspreadData))
					{
						return 0;
					}
					ISpreadService kuaFuService = this.GetKuaFuService(false);
					if (null == kuaFuService)
					{
						return -11001;
					}
					try
					{
						int num = kuaFuService.SpreadSign(this._ClientInfo.ServerId, zoneID, roleID);
						if (num > 0)
						{
							KFSpreadData kfspreadData2 = new KFSpreadData();
							kfspreadData2.ServerID = this._ClientInfo.ServerId;
							kfspreadData2.ZoneID = zoneID;
							kfspreadData2.RoleID = roleID;
							this._RoleId2KFSpreadDataDict.TryAdd(roleID, kfspreadData2);
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
			return 1;
		}

		public int[] SpreadCount(int zoneID, int roleID)
		{
			int[] array = new int[3];
			int[] array2 = array;
			lock (this._Mutex)
			{
				KFSpreadData kfspreadData;
				if (this._RoleId2KFSpreadDataDict.TryGetValue(roleID, out kfspreadData))
				{
					kfspreadData.UpdateLogtime();
					return new int[]
					{
						kfspreadData.CountRole,
						kfspreadData.CountVip,
						kfspreadData.CountLevel
					};
				}
				ISpreadService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return array2;
				}
				try
				{
					array2 = kuaFuService.SpreadCount(this._ClientInfo.ServerId, zoneID, roleID);
					if (array2 != null && array2.Length == 3)
					{
						KFSpreadData kfspreadData2 = new KFSpreadData();
						kfspreadData2.ServerID = this._ClientInfo.ServerId;
						kfspreadData2.ZoneID = zoneID;
						kfspreadData2.RoleID = roleID;
						kfspreadData2.CountRole = array2[0];
						kfspreadData2.CountVip = array2[1];
						kfspreadData2.CountLevel = array2[2];
						this._RoleId2KFSpreadDataDict.TryAdd(roleID, kfspreadData2);
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return array2;
		}

		public int CheckVerifyCode(string cuserID, int czoneID, int croleID, int pzoneID, int proleID, int isVip, int isLevel)
		{
			int result = -1;
			try
			{
				ISpreadService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.CheckVerifyCode(this._ClientInfo.ServerId, cuserID, czoneID, croleID, pzoneID, proleID, isVip, isLevel);
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
			return result;
		}

		public int TelCodeGet(int czoneID, int croleID, string tel)
		{
			int result = -1;
			try
			{
				ISpreadService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.TelCodeGet(this._ClientInfo.ServerId, czoneID, croleID, tel);
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
			return result;
		}

		public int TelCodeVerify(int czoneID, int croleID, int telCode)
		{
			int result = -1;
			try
			{
				ISpreadService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.TelCodeVerify(this._ClientInfo.ServerId, czoneID, croleID, telCode);
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
			return result;
		}

		public bool SpreadLevel(int pzoneID, int proleID, int czoneID, int croleID)
		{
			bool result = false;
			try
			{
				ISpreadService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.SpreadLevel(pzoneID, proleID, czoneID, croleID);
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
			return result;
		}

		public bool SpreadVip(int pzoneID, int proleID, int czoneID, int croleID)
		{
			bool result = false;
			try
			{
				ISpreadService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.SpreadVip(pzoneID, proleID, czoneID, croleID);
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
			return result;
		}

		public const int _gameType = 9;

		public const int _sceneType = 10002;

		private const int CHECK_SPREAD_DATA_SECOND = 43200;

		private static SpreadClient instance = new SpreadClient();

		private object _Mutex = new object();

		private object _RemotingMutex = new object();

		private ICoreInterface _CoreInterface = null;

		private ISpreadService _KuaFuService = null;

		private bool _ClientInitialized = false;

		private KuaFuClientContext _ClientInfo = new KuaFuClientContext();

		private ConcurrentDictionary<int, KFSpreadData> _RoleId2KFSpreadDataDict = new ConcurrentDictionary<int, KFSpreadData>();

		private string _RemoteServiceUri = null;

		private DateTime _checkSpreadDataTime = DateTime.MinValue;

		private DuplexChannelFactory<ISpreadService> channelFactory;
	}
}
