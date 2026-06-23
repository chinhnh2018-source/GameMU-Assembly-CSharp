using System;
using System.Collections.Generic;
using System.ServiceModel;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.Data;
using Tmsk.Contract.KuaFuData;

namespace KF.Client
{
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class KFCopyRpcClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		public static KFCopyRpcClient getInstance()
		{
			return KFCopyRpcClient.instance;
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			this.CoreInterface = coreInterface;
			this.ClientInfo.PTID = GameManager.PTID;
			this.ClientInfo.ServerId = GameManager.ServerId;
			this.ClientInfo.GameType = 8;
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

		public void EventCallBackHandler(AsyncDataItem item)
		{
			try
			{
				int eventType = item.EventType;
				object[] args = item.Args;
				switch (eventType)
				{
				case 10000:
					if (args != null && args.Length == 2 && (int)args[0] != this.ClientInfo.ServerId)
					{
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFCopyRoomCreateEvent((CopyTeamCreateData)args[1]), 10001);
					}
					break;
				case 10001:
					if (args != null && args.Length == 2 && (int)args[0] != this.ClientInfo.ServerId)
					{
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFCopyRoomJoinEvent((CopyTeamJoinData)args[1]), 10001);
					}
					break;
				case 10002:
					if (args != null && args.Length == 2 && (int)args[0] != this.ClientInfo.ServerId)
					{
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFCopyRoomKickoutEvent((CopyTeamKickoutData)args[1]), 10001);
					}
					break;
				case 10003:
					if (args != null && args.Length == 2 && (int)args[0] != this.ClientInfo.ServerId)
					{
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFCopyRoomLeaveEvent((CopyTeamLeaveData)args[1]), 10001);
					}
					break;
				case 10004:
					if (args != null && args.Length == 2 && (int)args[0] != this.ClientInfo.ServerId)
					{
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFCopyRoomReadyEvent((CopyTeamReadyData)args[1]), 10001);
					}
					break;
				case 10005:
					if (args != null && args.Length == 2 && (int)args[0] != this.ClientInfo.ServerId)
					{
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFCopyRoomStartEvent((CopyTeamStartData)args[1]), 10001);
					}
					break;
				case 10006:
					if (args != null && args.Length == 1)
					{
						this.CoreInterface.GetEventSourceInterface().fireEvent(new KFCopyTeamDestroyEvent((CopyTeamDestroyData)args[0]), 10001);
					}
					break;
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

		public void RemoveRoleData(int roleId)
		{
			lock (this.Mutex)
			{
				this.RoleId2RoleDataDict.Remove(roleId);
				this.RoleId2KuaFuStateDict.Remove(roleId);
			}
		}

		public void TimerProc(object sender, EventArgs e)
		{
			try
			{
				string runtimeVariable = this.CoreInterface.GetRuntimeVariable("KuaFuCopyUri", null);
				if (this.RemoteServiceUri != runtimeVariable)
				{
					this.RemoteServiceUri = runtimeVariable;
				}
				IKuaFuCopyService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					if (this.ClientInfo.ClientId > 0)
					{
						AsyncDataItem[] clientCacheItems = kuaFuService.GetClientCacheItems(this.ClientInfo.ServerId);
						if (clientCacheItems != null && clientCacheItems.Length > 0)
						{
							foreach (AsyncDataItem item in clientCacheItems)
							{
								this.EventCallBackHandler(item);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "KFCopyRpcClient.TimerProc调度异常", ex, true);
				this.ResetKuaFuService();
			}
		}

		private void CloseConnection()
		{
			this.ClientInfo.ClientId = 0;
			this.ServerInfoAsyncAge = 0;
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("KuaFuCopyUri", null);
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

		private IKuaFuCopyService GetKuaFuService(bool noWait = false)
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
					IKuaFuCopyService kuaFuCopyService;
					if (this.KuaFuService == null)
					{
						kuaFuCopyService = (IKuaFuCopyService)Activator.GetObject(typeof(IKuaFuCopyService), this.RemoteServiceUri);
						if (null == kuaFuCopyService)
						{
							return null;
						}
					}
					else
					{
						kuaFuCopyService = this.KuaFuService;
					}
					int num = this.ClientInfo.ClientId;
					long num2 = TimeUtil.NOW();
					if (num <= 0 || Math.Abs(num2 - this.ClientInfo.LastInitClientTicks) > 12000L)
					{
						this.ClientInfo.LastInitClientTicks = num2;
						num = kuaFuCopyService.InitializeClient(this.ClientInfo);
					}
					if (kuaFuCopyService != null && (num != this.ClientInfo.ClientId || this.KuaFuService != kuaFuCopyService))
					{
						lock (this.Mutex)
						{
							if (num > 0)
							{
								this.KuaFuService = kuaFuCopyService;
							}
							else
							{
								this.KuaFuService = null;
							}
							this.ClientInfo.ClientId = num;
							return kuaFuCopyService;
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

		public bool GetKuaFuGSInfo(int serverId, out string gsIp, out int gsPort)
		{
			gsIp = string.Empty;
			gsPort = 0;
			KuaFuServerInfo kuaFuServerInfo = null;
			bool result;
			if (!KuaFuManager.getInstance().TryGetValue(serverId, out kuaFuServerInfo))
			{
				result = false;
			}
			else
			{
				gsIp = kuaFuServerInfo.Ip;
				gsPort = kuaFuServerInfo.Port;
				result = true;
			}
			return result;
		}

		public KFCopyTeamCreateRsp CreateTeam(KFCopyTeamCreateReq req)
		{
			IKuaFuCopyService kuaFuService = this.GetKuaFuService(true);
			KFCopyTeamCreateRsp result;
			if (kuaFuService == null)
			{
				result = null;
			}
			else
			{
				result = kuaFuService.CreateTeam(req);
			}
			return result;
		}

		public KFCopyTeamJoinRsp JoinTeam(KFCopyTeamJoinReq req)
		{
			IKuaFuCopyService kuaFuService = this.GetKuaFuService(true);
			KFCopyTeamJoinRsp result;
			if (kuaFuService == null)
			{
				result = null;
			}
			else
			{
				result = kuaFuService.JoinTeam(req);
			}
			return result;
		}

		public KFCopyTeamStartRsp StartGame(KFCopyTeamStartReq req)
		{
			IKuaFuCopyService kuaFuService = this.GetKuaFuService(true);
			KFCopyTeamStartRsp result;
			if (kuaFuService == null)
			{
				result = null;
			}
			else
			{
				result = kuaFuService.StartGame(req);
			}
			return result;
		}

		public KFCopyTeamKickoutRsp KickoutTeam(KFCopyTeamKickoutReq req)
		{
			IKuaFuCopyService kuaFuService = this.GetKuaFuService(true);
			KFCopyTeamKickoutRsp result;
			if (kuaFuService == null)
			{
				result = null;
			}
			else
			{
				result = kuaFuService.KickoutTeam(req);
			}
			return result;
		}

		public KFCopyTeamLeaveRsp LeaveTeam(KFCopyTeamLeaveReq req)
		{
			IKuaFuCopyService kuaFuService = this.GetKuaFuService(true);
			KFCopyTeamLeaveRsp result;
			if (kuaFuService == null)
			{
				result = null;
			}
			else
			{
				result = kuaFuService.LeaveTeam(req);
			}
			return result;
		}

		public KFCopyTeamSetReadyRsp SetReady(KFCopyTeamSetReadyReq req)
		{
			IKuaFuCopyService kuaFuService = this.GetKuaFuService(true);
			KFCopyTeamSetReadyRsp result;
			if (kuaFuService == null)
			{
				result = null;
			}
			else
			{
				result = kuaFuService.TeamSetReady(req);
			}
			return result;
		}

		public KFCopyTeamSetFlagRsp SetFlag(KFCopyTeamSetFlagReq req)
		{
			IKuaFuCopyService kuaFuService = this.GetKuaFuService(true);
			KFCopyTeamSetFlagRsp result;
			if (kuaFuService == null)
			{
				result = null;
			}
			else
			{
				result = kuaFuService.TeamSetFlag(req);
			}
			return result;
		}

		public CopyTeamData GetTeamData(long teamid)
		{
			IKuaFuCopyService kuaFuService = this.GetKuaFuService(false);
			CopyTeamData result;
			if (kuaFuService == null)
			{
				result = null;
			}
			else
			{
				result = kuaFuService.GetTeamData(teamid);
			}
			return result;
		}

		public void KFCopyTeamRemove(long teamId)
		{
			IKuaFuCopyService kuaFuService = this.GetKuaFuService(false);
			if (kuaFuService != null)
			{
				kuaFuService.RemoveTeam(teamId);
			}
		}

		public InputKingPaiHangDataEx GetPlatChargeKing()
		{
			IKuaFuCopyService kuaFuService = this.GetKuaFuService(true);
			InputKingPaiHangDataEx result;
			if (kuaFuService == null)
			{
				result = null;
			}
			else
			{
				object platChargeKing = kuaFuService.GetPlatChargeKing();
				result = ((platChargeKing != null) ? (platChargeKing as InputKingPaiHangDataEx) : null);
			}
			return result;
		}

		public List<InputKingPaiHangDataEx> GetPlatChargeKingEveryDay(DateTime FromDate, DateTime ToDate)
		{
			IKuaFuCopyService kuaFuService = this.GetKuaFuService(true);
			if (kuaFuService != null)
			{
				try
				{
					object platChargeKingEveryDay = kuaFuService.GetPlatChargeKingEveryDay(FromDate, ToDate);
					return (platChargeKingEveryDay != null) ? (platChargeKingEveryDay as List<InputKingPaiHangDataEx>) : null;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public long QueryHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid)
		{
			long result = -11000L;
			IKuaFuCopyService kuaFuService = this.GetKuaFuService(true);
			if (kuaFuService != null)
			{
				try
				{
					result = kuaFuService.QueryHuodongAwardUserHist(actType, huoDongKeyStr, userid);
				}
				catch (Exception ex)
				{
					result = -11003L;
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public int UpdateHuodongAwardUserHist(int actType, string huoDongKeyStr, string userid, int extTag)
		{
			int result = -11000;
			IKuaFuCopyService kuaFuService = this.GetKuaFuService(true);
			if (kuaFuService != null)
			{
				try
				{
					result = kuaFuService.UpdateHuodongAwardUserHist(actType, huoDongKeyStr, userid, extTag);
				}
				catch (Exception ex)
				{
					result = -11003;
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public int SpecPriority_ModifyActivityConditionNum(int key, int add)
		{
			int result = -11000;
			IKuaFuCopyService kuaFuService = this.GetKuaFuService(true);
			if (kuaFuService != null)
			{
				try
				{
					result = kuaFuService.SpecPriority_ModifyActivityConditionNum(key, add);
				}
				catch (Exception ex)
				{
					result = -11003;
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public SpecPrioritySyncData SpecPriority_GetActivityConditionInfo()
		{
			IKuaFuCopyService kuaFuService = this.GetKuaFuService(true);
			if (kuaFuService != null)
			{
				try
				{
					object obj = kuaFuService.SpecPriority_GetActivityConditionInfo();
					return (obj != null) ? (obj as SpecPrioritySyncData) : null;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		private const int MaxRequestCount = 50;

		public const int SceneType = 10001;

		private static readonly KFCopyRpcClient instance = new KFCopyRpcClient();

		private object Mutex = new object();

		private object RemotingMutex = new object();

		private int CurrentRequestCount = 0;

		private Dictionary<int, KuaFuRoleData> RoleId2RoleDataDict = new Dictionary<int, KuaFuRoleData>();

		private Dictionary<int, int> RoleId2KuaFuStateDict = new Dictionary<int, int>();

		private int ServerInfoAsyncAge = 0;

		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		private string RemoteServiceUri = null;

		private IKuaFuCopyService KuaFuService = null;

		private ICoreInterface CoreInterface = null;

		private DuplexChannelFactory<IKuaFuCopyService> channelFactory;
	}
}
