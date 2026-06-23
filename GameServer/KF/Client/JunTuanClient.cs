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
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class JunTuanClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		public static JunTuanClient getInstance()
		{
			return JunTuanClient.instance;
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
			this.ClientInfo.GameType = 21;
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

		public void TimerProc(object sender, EventArgs e)
		{
			try
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				if (this.NextClearFuBenTime < dateTime)
				{
					this.NextClearFuBenTime = dateTime.AddHours(1.0);
					this.ClearOverTimeFuBen(dateTime);
					this.ClearOverTimePrisonData(dateTime);
				}
				string runtimeVariable = this.CoreInterface.GetRuntimeVariable("JunTuanUri", null);
				if (this.RemoteServiceUri != runtimeVariable)
				{
					this.RemoteServiceUri = runtimeVariable;
				}
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					if (this.ClientInfo.ClientId > 0)
					{
						AsyncDataItem[] clientCacheItems = kuaFuService.GetClientCacheItems(this.ClientInfo.ServerId, this.AsyncDataItemAge);
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
			this.RemoteServiceUri = this.CoreInterface.GetRuntimeVariable("JunTuanUri", null);
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

		private IJunTuanService GetKuaFuService(bool noWait = false)
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
					IJunTuanService junTuanService;
					if (this.KuaFuService == null)
					{
						junTuanService = (IJunTuanService)Activator.GetObject(typeof(IJunTuanService), this.RemoteServiceUri);
						if (null == junTuanService)
						{
							return null;
						}
					}
					else
					{
						junTuanService = this.KuaFuService;
					}
					int num = this.ClientInfo.ClientId;
					long num2 = TimeUtil.NOW();
					if (num <= 0 || Math.Abs(num2 - this.ClientInfo.LastInitClientTicks) > 12000L)
					{
						this.ClientInfo.LastInitClientTicks = num2;
						num = junTuanService.InitializeClient(this.ClientInfo);
					}
					if (junTuanService != null && (num != this.ClientInfo.ClientId || this.KuaFuService != junTuanService))
					{
						lock (this.Mutex)
						{
							if (num > 0)
							{
								this.KuaFuService = junTuanService;
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

		public void EventCallBackHandler(AsyncDataItem item)
		{
			try
			{
				int eventType = item.EventType;
				object[] args = item.Args;
				int num = eventType;
				switch (num)
				{
				case 21:
				{
					KuaFuData<JunTuanData> kuaFuData = args[0] as KuaFuData<JunTuanData>;
					if (null != kuaFuData)
					{
						JunTuanManager.getInstance().UpdateJunTuanData(kuaFuData);
					}
					break;
				}
				case 22:
				{
					JunTuanBangHuiMiniData junTuanBangHuiMiniData = args[0] as JunTuanBangHuiMiniData;
					if (null != junTuanBangHuiMiniData)
					{
						JunTuanManager.getInstance().UpdateBhJunTuan(junTuanBangHuiMiniData);
					}
					break;
				}
				case 23:
				case 24:
					break;
				case 25:
				{
					int bhid = (int)args[0];
					JunTuanManager.getInstance().DelayUpdateJunTuanRoleList(bhid);
					break;
				}
				case 26:
					JunTuanManager.getInstance().OnChatListData(args[0] as byte[]);
					break;
				case 27:
				{
					int num2 = (int)args[0];
					bool icon = (int)args[1] > 0;
					JunTuanManager.getInstance().NotifyJunTuanRequest(num2, icon);
					break;
				}
				case 28:
					if (args.Length == 1)
					{
						LingDiCaiJiManager.getInstance().NotifyJunTuanRequest(args[0] as LingDiData, 28);
					}
					break;
				case 29:
					if (args.Length == 1)
					{
						LingDiCaiJiManager.getInstance().NotifyJunTuanRequest(args[0] as LingDiData, 29);
					}
					break;
				case 30:
					if (args.Length == 1)
					{
						HongBaoManager.getInstance().UpdateChongZhiHongBaoDataList(args[0] as KuaFuCmdData);
					}
					break;
				case 31:
					if (args.Length == 1)
					{
						int roleID = (int)args[0];
						YaoSaiJianYuManager.getInstance().UpdateYaoSaiLogData(roleID);
					}
					break;
				case 32:
					if (args.Length == 1)
					{
						int num2 = (int)args[0];
						this.GetJunTuanEraData(num2, false);
						EraManager.getInstance().CheckAllJunTuanEraIcon(num2);
					}
					break;
				case 33:
					if (args.Length == 1)
					{
						this.JunTuanEraID = (int)args[0];
						EraManager.getInstance().OnChangeEraID(this.JunTuanEraID);
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
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public int CreateJunTuan(JunTuanRequestData data)
		{
			int result = 0;
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return -11001;
				}
				try
				{
					result = kuaFuService.CreateJunTuan(DataHelper.ObjectToBytes<JunTuanRequestData>(data));
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11000;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		public int ChangeJunTuanBulltin(int bhid, int junTuanId, string bulltin)
		{
			int result = 0;
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return -11001;
				}
				try
				{
					result = kuaFuService.ChangeJunTuanBulltin(bhid, junTuanId, bulltin);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11000;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		public int ChangeJunTuanGVoicePrioritys(int bhid, string prioritys)
		{
			int result = 0;
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return -11001;
				}
				try
				{
					result = kuaFuService.ChangeJunTuanGVoicePrioritys(bhid, prioritys);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11000;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		public void BroadcastGMCmdData(GMCmdData data, int serverFlag)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
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

		public string GetJunTuanGVoicePrioritys(int bhid)
		{
			string result = null;
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						result = kuaFuService.GetJunTuanGVoicePrioritys(bhid);
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
			return result;
		}

		public int UpdateJunTuanLingDi(int junTuanId, int lingdi)
		{
			int result = 0;
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return -11001;
				}
				try
				{
					result = kuaFuService.UpdateJunTuanLingDi(junTuanId, lingdi);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11000;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		public int QuitJunTuan(int bhid, int junTuanId, int otherBhId)
		{
			int result = 0;
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return -11001;
				}
				try
				{
					result = kuaFuService.QuitJunTuan(bhid, junTuanId, otherBhId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11000;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		public int DestroyJunTuan(int bhid, int junTuanId)
		{
			int result = 0;
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return -11001;
				}
				try
				{
					result = kuaFuService.DestroyJunTuan(bhid, junTuanId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					result = -11000;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return result;
		}

		public void JunTuanChat(List<KFChat> chatList)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						byte[] array = DataHelper.ObjectToBytes<List<KFChat>>(chatList);
						kuaFuService.JunTuanChat(this.ClientInfo.ServerId, array);
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

		public int JoinJunTuan(JunTuanRequestData data)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return -11000;
				}
				try
				{
					return kuaFuService.JoinJunTuan(DataHelper.ObjectToBytes<JunTuanRequestData>(data));
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
			return -11000;
		}

		public List<JunTuanMiniData> GetJunTuanList(int bhid)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						KuaFuCmdData junTuanList = kuaFuService.GetJunTuanList(this.ClientInfo.ServerId, this.JunTuanList.Age);
						lock (this.Mutex)
						{
							if (null != junTuanList)
							{
								if (junTuanList == null || junTuanList.Age < 0L)
								{
									return null;
								}
								if (junTuanList.Age == this.JunTuanList.Age)
								{
									return this.JunTuanList.V;
								}
								this.JunTuanList.Age = junTuanList.Age;
								if (junTuanList.Bytes0 != null)
								{
									this.JunTuanList.V = DataHelper2.BytesToObject<List<JunTuanMiniData>>(junTuanList.Bytes0, 0, junTuanList.Bytes0.Length);
								}
								if (this.JunTuanList.V == null)
								{
									this.JunTuanList.V = new List<JunTuanMiniData>();
								}
							}
							return this.JunTuanList.V;
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
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		public JunTuanData GetJunTuanData(int bhid, int junTuanId = 0, bool wait = true)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					long num = 0L;
					JunTuanDetailData junTuanDetailData = this.GetJunTuanDetailData(bhid, junTuanId);
					if (null != junTuanDetailData)
					{
						num = junTuanDetailData.JunTuanData.Age;
					}
					if (!wait)
					{
						return (junTuanDetailData == null) ? null : junTuanDetailData.JunTuanData.V;
					}
					KuaFuCmdData junTuanData = kuaFuService.GetJunTuanData(bhid, num);
					lock (this.Mutex)
					{
						if (junTuanData == null || junTuanData.Age < 0L)
						{
							return null;
						}
						if (junTuanData.Age == num)
						{
							return junTuanDetailData.JunTuanData.V;
						}
						JunTuanData junTuanData2 = null;
						if (null != junTuanData.Bytes0)
						{
							junTuanData2 = DataHelper.BytesToObject<JunTuanData>(junTuanData.Bytes0, 0, junTuanData.Bytes0.Length);
						}
						junTuanDetailData = this.AddJunTuanDetailData(bhid, junTuanId);
						if (junTuanData2 != null && null != junTuanDetailData)
						{
							return junTuanDetailData.UpdateJunTuanData(junTuanData2, junTuanData.Age);
						}
						return null;
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

		public int GameFuBenRoleChangeState(int serverId, int rid, int gameId, int side, int state)
		{
			int result = -11;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.GameFuBenRoleChangeState(serverId, rid, gameId, side, state);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public void UpdateKuaFuMapClientCount(int gameId, List<int> mapClientCountList)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.UpdateKuaFuMapClientCount(this.ClientInfo.ServerId, gameId, mapClientCountList);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		private void ClearOverTimePrisonData(DateTime now)
		{
			lock (this.Mutex)
			{
				List<int> list = new List<int>();
				foreach (KeyValuePair<int, KFPrisonRoleAllData> keyValuePair in this.YaoSaiPrisonRoleDataDict)
				{
					if (keyValuePair.Value.RoleDataEndTime < now)
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (int key in list)
				{
					this.YaoSaiPrisonRoleDataDict.Remove(key);
				}
				list.Clear();
				foreach (KeyValuePair<int, KFPrisonFuLuAllData> keyValuePair2 in this.YaoSaiOwnerVsFuLuDict)
				{
					if (keyValuePair2.Value.DataEndTime < now)
					{
						list.Add(keyValuePair2.Key);
					}
				}
				foreach (int key in list)
				{
					this.YaoSaiOwnerVsFuLuDict.Remove(key);
				}
				list.Clear();
				foreach (KeyValuePair<int, KFPrisonJingJiAllData> keyValuePair3 in this.YaoSaiPrisonJingJiDataDict)
				{
					if (keyValuePair3.Value.JingJiDataEndTime < now)
					{
						list.Add(keyValuePair3.Key);
					}
				}
				foreach (int key in list)
				{
					this.YaoSaiPrisonJingJiDataDict.Remove(key);
				}
				list.Clear();
				foreach (KeyValuePair<int, KFPrisonLogAllData> keyValuePair4 in this.YaoSaiPrisonLogDataDict)
				{
					if (keyValuePair4.Value.LogDataEndTime < now)
					{
						list.Add(keyValuePair4.Key);
					}
				}
				foreach (int key in list)
				{
					this.YaoSaiPrisonLogDataDict.Remove(key);
				}
			}
		}

		private void ClearOverTimeFuBen(DateTime now)
		{
			lock (this.Mutex)
			{
				List<int> list = new List<int>();
				foreach (KeyValuePair<int, KuaFuData<KarenFuBenData>> keyValuePair in this.KarenFuBenDataDict)
				{
					if (keyValuePair.Value.V.EndTime < now)
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (int key in list)
				{
					this.KarenFuBenDataDict.Remove(key);
				}
			}
		}

		public KarenFuBenRoleData GetKarenFuBenRoleData(int gameid, int roleId)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetKarenFuBenRoleData(gameid, roleId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public KarenFuBenData GetKarenKuaFuFuBenData(int mapCode)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KuaFuData<KarenFuBenData> kuaFuData = null;
						if (!this.KarenFuBenDataDict.TryGetValue(mapCode, out kuaFuData))
						{
							kuaFuData = new KuaFuData<KarenFuBenData>();
							this.KarenFuBenDataDict[mapCode] = kuaFuData;
						}
						SceneUIClasses mapSceneType = Global.GetMapSceneType(mapCode);
						int num;
						if (mapSceneType == 41)
						{
							num = 19;
						}
						else
						{
							num = 20;
						}
						KuaFuCmdData karenKuaFuFuBenData = kuaFuService.GetKarenKuaFuFuBenData(num, mapCode, kuaFuData.Age);
						if (karenKuaFuFuBenData == null || karenKuaFuFuBenData.Age < 0L)
						{
							return null;
						}
						if (karenKuaFuFuBenData != null && karenKuaFuFuBenData.Age > kuaFuData.Age)
						{
							kuaFuData.Age = karenKuaFuFuBenData.Age;
							if (null != karenKuaFuFuBenData.Bytes0)
							{
								kuaFuData.V = DataHelper2.BytesToObject<KarenFuBenData>(karenKuaFuFuBenData.Bytes0, 0, karenKuaFuFuBenData.Bytes0.Length);
							}
							if (null == kuaFuData.V)
							{
								kuaFuData.V = new KarenFuBenData();
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

		public List<JunTuanBaseData> GetJunTuanBaseDataList(bool wait = true)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					KuaFuCmdData junTuanBaseDataList = kuaFuService.GetJunTuanBaseDataList(this.JunTuanBaseDataList.Age);
					lock (this.Mutex)
					{
						if (junTuanBaseDataList == null || junTuanBaseDataList.Age < 0L)
						{
							return null;
						}
						if (junTuanBaseDataList.Age > this.JunTuanBaseDataList.Age)
						{
							this.JunTuanBaseDataList.Age = junTuanBaseDataList.Age;
							if (null != junTuanBaseDataList.Bytes0)
							{
								this.JunTuanBaseDataList.V = DataHelper.BytesToObject<List<JunTuanBaseData>>(junTuanBaseDataList.Bytes0, 0, junTuanBaseDataList.Bytes0.Length);
							}
							if (null == this.JunTuanBaseDataList.V)
							{
								this.JunTuanBaseDataList.V = new List<JunTuanBaseData>();
							}
						}
						return this.JunTuanBaseDataList.V;
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

		public int GetJunTuanTaskAllData(int bhid, int junTuanId, out JunTuanTaskAllData taskAllData)
		{
			taskAllData = null;
			try
			{
				KuaFuCmdData kuaFuCmdData = null;
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					long num = 0L;
					JunTuanDetailData junTuanDetailData = this.GetJunTuanDetailData(bhid, junTuanId);
					try
					{
						if (null != junTuanDetailData)
						{
							num = junTuanDetailData.JunTuanTaskAllData.Age;
						}
						kuaFuCmdData = kuaFuService.GetJunTuanTaskAllData(bhid, junTuanId, num);
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
						return -11000;
					}
					lock (this.Mutex)
					{
						if (kuaFuCmdData == null || kuaFuCmdData.Age < 0L)
						{
							return -11003;
						}
						if (kuaFuCmdData.Age == num)
						{
							taskAllData = junTuanDetailData.JunTuanTaskAllData.V;
							return 0;
						}
						JunTuanTaskAllData junTuanTaskAllData = null;
						if (kuaFuCmdData.Bytes0 != null)
						{
							junTuanTaskAllData = DataHelper.BytesToObject<JunTuanTaskAllData>(kuaFuCmdData.Bytes0, 0, kuaFuCmdData.Bytes0.Length);
							if (junTuanTaskAllData != null)
							{
								junTuanDetailData = this.AddJunTuanDetailData(bhid, junTuanId);
								if (junTuanDetailData.JunTuanTaskAllData.Age < kuaFuCmdData.Age)
								{
									junTuanDetailData.JunTuanTaskAllData.Age = kuaFuCmdData.Age;
									junTuanDetailData.JunTuanTaskAllData.V = junTuanTaskAllData;
								}
							}
						}
						taskAllData = junTuanTaskAllData;
						return (taskAllData != null) ? 0 : -11000;
					}
				}
				return -11000;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return -11003;
		}

		public bool IsTaskComplete(int bhid, int junTuanId, int taskId)
		{
			try
			{
				JunTuanTaskAllData junTuanTaskAllData;
				if (this.GetJunTuanTaskAllData(bhid, junTuanId, out junTuanTaskAllData) < 0 || junTuanTaskAllData.TaskList == null)
				{
					return false;
				}
				JunTuanTaskData junTuanTaskData = junTuanTaskAllData.TaskList.Find((JunTuanTaskData x) => x.TaskId == taskId);
				if (junTuanTaskData != null && junTuanTaskData.TaskState == 1L)
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return false;
		}

		public int JoinJunTuanResponse(int bhid, int junTuanId, int otherBhid, bool accept)
		{
			int result = -11;
			int serverId = this.ClientInfo.ServerId;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.JoinJunTuanResponse(bhid, junTuanId, otherBhid, accept);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public int RemoveBangHui(int bhid)
		{
			int result = -11;
			int serverId = this.ClientInfo.ServerId;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.RemoveBangHui(bhid);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public int ChangeBangHuiName(int bhid, string bhName)
		{
			int result = -11;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.ChangeBangHuiName(bhid, bhName);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public void PushGameResultData(object data)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
		}

		public int ExecuteCommand(string cmd)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11003;
		}

		public List<JunTuanRequestData> GetJunTuanRequestList(int bhid)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null != kuaFuService)
				{
					try
					{
						long num = 0L;
						KuaFuData<List<JunTuanRequestData>> kuaFuData;
						lock (this.Mutex)
						{
							if (this.JunTuanRequestListDict.TryGetValue(bhid, out kuaFuData))
							{
								num = kuaFuData.Age;
							}
						}
						KuaFuCmdData junTuanRequestList = kuaFuService.GetJunTuanRequestList(bhid, num);
						lock (this.Mutex)
						{
							if (junTuanRequestList == null || junTuanRequestList.Age < 0L)
							{
								return null;
							}
							if (num == junTuanRequestList.Age)
							{
								return kuaFuData.V;
							}
							if (!this.JunTuanRequestListDict.TryGetValue(bhid, out kuaFuData))
							{
								kuaFuData = new KuaFuData<List<JunTuanRequestData>>();
								this.JunTuanRequestListDict[bhid] = kuaFuData;
							}
							if (null != junTuanRequestList)
							{
								kuaFuData.Age = junTuanRequestList.Age;
								if (junTuanRequestList.Bytes0 != null)
								{
									kuaFuData.V = DataHelper2.BytesToObject<List<JunTuanRequestData>>(junTuanRequestList.Bytes0, 0, junTuanRequestList.Bytes0.Length);
								}
								else
								{
									kuaFuData.V = new List<JunTuanRequestData>();
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
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		private JunTuanDetailData GetJunTuanDetailData(int bhid, int junTuanId)
		{
			JunTuanDetailData junTuanDetailData = null;
			lock (this.Mutex)
			{
				int num;
				if (!this.BangHuiJunTuanIdDict.TryGetValue(bhid, out num) || num == 0 || num != junTuanId)
				{
					return null;
				}
				if (!this.JunTuanDetailDataDict.TryGetValue(junTuanId, out junTuanDetailData) || junTuanDetailData.JunTuanBaseData.V.BhList == null || !junTuanDetailData.JunTuanBaseData.V.BhList.Contains(bhid))
				{
					return null;
				}
			}
			return junTuanDetailData;
		}

		private JunTuanDetailData AddJunTuanDetailData(int bhid, int junTuanId)
		{
			JunTuanDetailData junTuanDetailData = null;
			JunTuanDetailData result;
			lock (this.Mutex)
			{
				this.BangHuiJunTuanIdDict[bhid] = junTuanId;
				if (!this.JunTuanDetailDataDict.TryGetValue(junTuanId, out junTuanDetailData))
				{
					junTuanDetailData = new JunTuanDetailData
					{
						JunTuanId = junTuanId
					};
					this.JunTuanDetailDataDict[junTuanId] = junTuanDetailData;
				}
				result = junTuanDetailData;
			}
			return result;
		}

		public List<JunTuanBangHuiData> GetJunTuanBangHuiList(int bhid, int junTuanId)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					long num = 0L;
					JunTuanDetailData junTuanDetailData = this.GetJunTuanDetailData(bhid, junTuanId);
					if (null != junTuanDetailData)
					{
						num = junTuanDetailData.JunTuanBangHuiList.Age;
					}
					KuaFuCmdData junTuanBangHuiList = kuaFuService.GetJunTuanBangHuiList(bhid, junTuanId, num);
					lock (this.Mutex)
					{
						if (junTuanBangHuiList == null || junTuanBangHuiList.Age < 0L)
						{
							return null;
						}
						if (junTuanBangHuiList.Age == num)
						{
							return junTuanDetailData.JunTuanBangHuiList.V;
						}
						if (junTuanBangHuiList.Bytes0 != null)
						{
							junTuanDetailData = this.AddJunTuanDetailData(bhid, junTuanId);
							if (junTuanDetailData.JunTuanBangHuiList.Age < junTuanBangHuiList.Age)
							{
								junTuanDetailData.JunTuanBangHuiList.Age = junTuanBangHuiList.Age;
								if (null != junTuanBangHuiList.Bytes0)
								{
									junTuanDetailData.JunTuanBangHuiList.V = DataHelper2.BytesToObject<List<JunTuanBangHuiData>>(junTuanBangHuiList.Bytes0, 0, junTuanBangHuiList.Bytes0.Length);
								}
								else
								{
									junTuanDetailData.JunTuanBangHuiList.V = new List<JunTuanBangHuiData>();
								}
							}
							return junTuanDetailData.JunTuanBangHuiList.V;
						}
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public List<JunTuanBangHuiData> GetJunTuanBangHuiList(int junTuanId)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					KuaFuCmdData junTuanBangHuiList = kuaFuService.GetJunTuanBangHuiList(0, junTuanId, 0L);
					if (junTuanBangHuiList == null || junTuanBangHuiList.Age < 0L)
					{
						return null;
					}
					if (junTuanBangHuiList.Bytes0 != null)
					{
						return DataHelper2.BytesToObject<List<JunTuanBangHuiData>>(junTuanBangHuiList.Bytes0, 0, junTuanBangHuiList.Bytes0.Length);
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public int JunTuanChangeBangHuiZhiWu(int bhid, int junTuanId, int otherBhid, int zhiWu)
		{
			int result = -11000;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.JunTuanChangeBangHuiZhiWu(bhid, junTuanId, otherBhid, zhiWu);
				}
				catch (Exception ex)
				{
					result = -11003;
				}
			}
			return result;
		}

		public List<JunTuanRoleData> GetJunTuanRoleList(int bhid, int junTuanId)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					long num = 0L;
					JunTuanDetailData junTuanDetailData = this.GetJunTuanDetailData(bhid, junTuanId);
					if (null != junTuanDetailData)
					{
						num = junTuanDetailData.JunTuanRoleDataList.Age;
					}
					KuaFuCmdData junTuanRoleList = kuaFuService.GetJunTuanRoleList(bhid, num);
					lock (this.Mutex)
					{
						if (junTuanRoleList == null || junTuanRoleList.Age < 0L)
						{
							return null;
						}
						if (junTuanRoleList.Age == num)
						{
							return junTuanDetailData.JunTuanRoleDataList.V;
						}
						if (junTuanRoleList != null)
						{
							junTuanDetailData = this.AddJunTuanDetailData(bhid, junTuanId);
							if (junTuanDetailData.JunTuanRoleDataList.Age < junTuanRoleList.Age)
							{
								junTuanDetailData.JunTuanRoleDataList.Age = junTuanRoleList.Age;
								if (null != junTuanRoleList.Bytes0)
								{
									junTuanDetailData.JunTuanRoleDataList.V = DataHelper2.BytesToObject<List<JunTuanRoleData>>(junTuanRoleList.Bytes0, 0, junTuanRoleList.Bytes0.Length);
								}
								else
								{
									junTuanDetailData.JunTuanRoleDataList.V = new List<JunTuanRoleData>();
								}
							}
							return junTuanDetailData.JunTuanRoleDataList.V;
						}
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public int UpdateRoleDataList(int bhid, List<JunTuanRoleData> list)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.UpdateRoleDataList(bhid, new KuaFuCmdData
					{
						Bytes0 = DataHelper.ObjectToBytes<List<JunTuanRoleData>>(list)
					});
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		public int JunTuanChangeTaskValue(int bhid, int junTuanId, int taskId, int addValue, long ticks)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.JunTuanChangeTaskValue(bhid, junTuanId, taskId, addValue, ticks);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		public List<JunTuanRankData> GetJunTuanRankingData()
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					long age = this.JunTuanRankDataList.Age;
					KuaFuCmdData junTuanRankingData = kuaFuService.GetJunTuanRankingData(age);
					lock (this.Mutex)
					{
						if (junTuanRankingData == null || junTuanRankingData.Age < 0L)
						{
							return null;
						}
						if (junTuanRankingData.Age == age)
						{
							return this.JunTuanRankDataList.V;
						}
						if (junTuanRankingData != null)
						{
							this.JunTuanRankDataList.Age = junTuanRankingData.Age;
							if (null != junTuanRankingData.Bytes0)
							{
								this.JunTuanRankDataList.V = DataHelper2.BytesToObject<List<JunTuanRankData>>(junTuanRankingData.Bytes0, 0, junTuanRankingData.Bytes0.Length);
							}
							else
							{
								this.JunTuanRankDataList.V = new List<JunTuanRankData>();
							}
						}
						return this.JunTuanRankDataList.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public List<JunTuanEventLog> GetJunTuanLogList(int bhid)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					KuaFuCmdData junTuanLogList = kuaFuService.GetJunTuanLogList(bhid, this.JunTuanEventLogList.Age);
					lock (this.Mutex)
					{
						if (junTuanLogList == null || junTuanLogList.Age < 0L)
						{
							return null;
						}
						if (junTuanLogList.Age == this.JunTuanEventLogList.Age)
						{
							return this.JunTuanEventLogList.V;
						}
						if (junTuanLogList != null)
						{
							this.JunTuanEventLogList.Age = junTuanLogList.Age;
							if (null != junTuanLogList.Bytes0)
							{
								this.JunTuanEventLogList.V = DataHelper2.BytesToObject<List<JunTuanEventLog>>(junTuanLogList.Bytes0, 0, junTuanLogList.Bytes0.Length);
							}
							else
							{
								this.JunTuanEventLogList.V = new List<JunTuanEventLog>();
							}
						}
						return this.JunTuanEventLogList.V;
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public int GetJunTuanPoint(int bhid, int junTuanId)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetJunTuanPoint(bhid, junTuanId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		public int SetDoubleOpenTime(int roleId, int linDiType, DateTime openTime, int openSeconds)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.SetDoubleOpenTime(roleId, linDiType, openTime, openSeconds);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -1;
		}

		public int SetShouWeiTime(int roleId, int bhid, int linDiType, DateTime openTime, int index, int junTuanPointCost)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.SetShouWeiTime(roleId, bhid, linDiType, openTime, index, junTuanPointCost);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -1;
		}

		public int CanEnterKuaFuMap(int roleId, int lingDiType)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					int num = kuaFuService.CanEnterKuaFuMap(roleId, lingDiType);
					if (num == 0)
					{
						return -1;
					}
					return num;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -1;
		}

		public int UpdateMapRoleNum(int lingDiType, int roleNum, int serverId)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.UpdateMapRoleNum(lingDiType, roleNum, serverId);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -1;
		}

		public List<LingDiData> GetLingDiData()
		{
			List<LingDiData> result = new List<LingDiData>();
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.GetLingDiData();
					return result;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public int SetLingZhu(int roleId, int lingDiType, int junTuanId, string junTuanName, int zhiWu, byte[] roledata)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			int result = 0;
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.SetLingZhu(roleId, lingDiType, junTuanId, junTuanName, zhiWu, roledata);
					return result;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public int SetShouWei(int lingDiType, List<LingDiShouWei> shouWeiList)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			int result = 0;
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.SetShouWei(lingDiType, shouWeiList);
					return result;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public int GetLingDiRoleNum(int lingDiType)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			int result = 0;
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.GetLingDiRoleNum(lingDiType);
					return result;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public bool GetClientCacheItems(int serverId)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			bool result = false;
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.GetClientCacheItems(serverId);
					return result;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public AsyncDataItem GetHongBaoDataList(long dataAge)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.GetHongBaoDataList(dataAge);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return null;
		}

		public int OpenHongBao(int hongBaoId, int rid, int zoneid, string userid, string rname)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.OpenHongBao(hongBaoId, rid, zoneid, userid, rname);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return -11000;
		}

		public bool AddChargeValue(string keyStr, long addCharge)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					kuaFuService.AddServerTotalCharge(keyStr, addCharge);
					return true;
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return false;
		}

		public void ClearYaoSaiPrisonData(int roleID)
		{
			lock (this.Mutex)
			{
			}
		}

		public List<KFPrisonLogData> GetYaoSaiPrisonLogData(int roleID)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KFPrisonLogAllData kfprisonLogAllData = null;
						if (!this.YaoSaiPrisonLogDataDict.TryGetValue(roleID, out kfprisonLogAllData))
						{
							kfprisonLogAllData = new KFPrisonLogAllData();
							this.YaoSaiPrisonLogDataDict[roleID] = kfprisonLogAllData;
						}
						KuaFuCmdData yaoSaiPrisonLogData = kuaFuService.GetYaoSaiPrisonLogData(roleID, kfprisonLogAllData.LogListData.Age);
						if (yaoSaiPrisonLogData == null || yaoSaiPrisonLogData.Age < 0L)
						{
							return null;
						}
						if (yaoSaiPrisonLogData != null && yaoSaiPrisonLogData.Age > kfprisonLogAllData.LogListData.Age)
						{
							kfprisonLogAllData.LogListData.Age = yaoSaiPrisonLogData.Age;
							if (null != yaoSaiPrisonLogData.Bytes0)
							{
								kfprisonLogAllData.LogListData.V = DataHelper2.BytesToObject<List<KFPrisonLogData>>(yaoSaiPrisonLogData.Bytes0, 0, yaoSaiPrisonLogData.Bytes0.Length);
							}
							if (null == kfprisonLogAllData.LogListData.V)
							{
								kfprisonLogAllData.LogListData.V = new List<KFPrisonLogData>();
							}
						}
						kfprisonLogAllData.LogDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						return kfprisonLogAllData.LogListData.V;
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

		public List<KFPrisonRoleData> GetYaoSaiPrisonFuLuData(int roleID)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KFPrisonFuLuAllData kfprisonFuLuAllData = null;
						if (!this.YaoSaiOwnerVsFuLuDict.TryGetValue(roleID, out kfprisonFuLuAllData))
						{
							kfprisonFuLuAllData = new KFPrisonFuLuAllData();
							this.YaoSaiOwnerVsFuLuDict[roleID] = kfprisonFuLuAllData;
						}
						KuaFuCmdData yaoSaiFuLuListData = kuaFuService.GetYaoSaiFuLuListData(roleID, kfprisonFuLuAllData.fuluData.Age);
						if (yaoSaiFuLuListData == null || yaoSaiFuLuListData.Age < 0L)
						{
							return null;
						}
						if (yaoSaiFuLuListData != null && yaoSaiFuLuListData.Age > kfprisonFuLuAllData.fuluData.Age)
						{
							kfprisonFuLuAllData.fuluData.Age = yaoSaiFuLuListData.Age;
							if (null != yaoSaiFuLuListData.Bytes0)
							{
								kfprisonFuLuAllData.fuluData.V = DataHelper2.BytesToObject<List<KFPrisonRoleData>>(yaoSaiFuLuListData.Bytes0, 0, yaoSaiFuLuListData.Bytes0.Length);
							}
							if (null == kfprisonFuLuAllData.fuluData.V)
							{
								kfprisonFuLuAllData.fuluData.V = new List<KFPrisonRoleData>();
							}
						}
						kfprisonFuLuAllData.DataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						return kfprisonFuLuAllData.fuluData.V;
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

		public KFPrisonRoleData GetYaoSaiPrisonRoleData(int roleID)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KFPrisonRoleAllData kfprisonRoleAllData = null;
						if (!this.YaoSaiPrisonRoleDataDict.TryGetValue(roleID, out kfprisonRoleAllData))
						{
							kfprisonRoleAllData = new KFPrisonRoleAllData();
							this.YaoSaiPrisonRoleDataDict[roleID] = kfprisonRoleAllData;
						}
						KuaFuCmdData yaoSaiPrisonRoleData = kuaFuService.GetYaoSaiPrisonRoleData(roleID, kfprisonRoleAllData.kfRoleData.Age);
						if (yaoSaiPrisonRoleData == null || yaoSaiPrisonRoleData.Age < 0L)
						{
							return null;
						}
						if (yaoSaiPrisonRoleData != null && yaoSaiPrisonRoleData.Age > kfprisonRoleAllData.kfRoleData.Age)
						{
							kfprisonRoleAllData.kfRoleData.Age = yaoSaiPrisonRoleData.Age;
							if (null != yaoSaiPrisonRoleData.Bytes0)
							{
								kfprisonRoleAllData.kfRoleData.V = DataHelper2.BytesToObject<KFPrisonRoleData>(yaoSaiPrisonRoleData.Bytes0, 0, yaoSaiPrisonRoleData.Bytes0.Length);
							}
							if (null == kfprisonRoleAllData.kfRoleData.V)
							{
								kfprisonRoleAllData.kfRoleData.V = new KFPrisonRoleData();
							}
						}
						kfprisonRoleAllData.RoleDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						return kfprisonRoleAllData.kfRoleData.V;
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

		public KFPrisonJingJiData GetYaoSaiPrisonJingJiData(int roleID)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KFPrisonJingJiAllData kfprisonJingJiAllData = null;
						if (!this.YaoSaiPrisonJingJiDataDict.TryGetValue(roleID, out kfprisonJingJiAllData))
						{
							kfprisonJingJiAllData = new KFPrisonJingJiAllData();
							this.YaoSaiPrisonJingJiDataDict[roleID] = kfprisonJingJiAllData;
						}
						KuaFuCmdData yaoSaiPrisonJingJiData = kuaFuService.GetYaoSaiPrisonJingJiData(roleID, kfprisonJingJiAllData.JingJiData.Age);
						if (yaoSaiPrisonJingJiData == null || yaoSaiPrisonJingJiData.Age < 0L)
						{
							return null;
						}
						if (yaoSaiPrisonJingJiData != null && yaoSaiPrisonJingJiData.Age > kfprisonJingJiAllData.JingJiData.Age)
						{
							kfprisonJingJiAllData.JingJiData.Age = yaoSaiPrisonJingJiData.Age;
							if (null != yaoSaiPrisonJingJiData.Bytes0)
							{
								kfprisonJingJiAllData.JingJiData.V = DataHelper2.BytesToObject<KFPrisonJingJiData>(yaoSaiPrisonJingJiData.Bytes0, 0, yaoSaiPrisonJingJiData.Bytes0.Length);
							}
							if (null == kfprisonJingJiAllData.JingJiData.V)
							{
								kfprisonJingJiAllData.JingJiData.V = new KFPrisonJingJiData();
							}
						}
						kfprisonJingJiAllData.JingJiDataEndTime = TimeUtil.NowDateTime().AddHours(1.0);
						return kfprisonJingJiAllData.JingJiData.V;
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

		public KFPrisonRoleData SearchYaoSaiFuLu(int rid, int unionlev, int faction, HashSet<int> frindSet)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KuaFuCmdData kuaFuCmdData = kuaFuService.SearchYaoSaiFuLu(rid, unionlev, faction, frindSet);
						if (kuaFuCmdData == null || kuaFuCmdData.Age < 0L)
						{
							return null;
						}
						if (null != kuaFuCmdData)
						{
							KFPrisonRoleAllData kfprisonRoleAllData = new KFPrisonRoleAllData();
							if (null != kuaFuCmdData.Bytes0)
							{
								kfprisonRoleAllData.kfRoleData.V = DataHelper2.BytesToObject<KFPrisonRoleData>(kuaFuCmdData.Bytes0, 0, kuaFuCmdData.Bytes0.Length);
							}
							if (null != kfprisonRoleAllData.kfRoleData.V)
							{
								this.YaoSaiPrisonRoleDataDict[kfprisonRoleAllData.kfRoleData.V.RoleID] = kfprisonRoleAllData;
							}
							return kfprisonRoleAllData.kfRoleData.V;
						}
						return null;
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

		public int YaoSaiPrisonOpt(int srcrid, int otherrid, int type, bool success)
		{
			int result = -11;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.YaoSaiPrisonOpt(srcrid, otherrid, type, success);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public int YaoSaiPrisonHuDong(int ownerid, int fuluid, int type, int param0, int param1, int param2)
		{
			int result = -11;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.YaoSaiPrisonHuDong(ownerid, fuluid, type, param0, param1, param2);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public int UpdateYaoSaiPrisonRoleData(KFUpdatePrisonRole data)
		{
			int result = -11;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.UpdateYaoSaiPrisonRoleData(data);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public int UpdateYaoSaiPrisonLogData(int rid, long id, int state)
		{
			int result = -11;
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					result = kuaFuService.UpdateYaoSaiPrisonLogData(rid, id, state);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return result;
		}

		public bool EraDonate(int juntuanid, int taskid, int var1, int var2, int var3)
		{
			IJunTuanService kuaFuService = this.GetKuaFuService(false);
			if (null != kuaFuService)
			{
				try
				{
					return kuaFuService.EraDonate(juntuanid, taskid, var1, var2, var3);
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return false;
		}

		public int GetCurrentEraID()
		{
			int junTuanEraID;
			if (-1 != this.JunTuanEraID)
			{
				junTuanEraID = this.JunTuanEraID;
			}
			else
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					junTuanEraID = this.JunTuanEraID;
				}
				else
				{
					try
					{
						lock (this.Mutex)
						{
							KuaFuCmdData eraData = kuaFuService.GetEraData(0, 0L);
							if (eraData == null || eraData.Age < 0L)
							{
								return this.JunTuanEraID;
							}
							if (null != eraData.Bytes0)
							{
								KFEraData kferaData = DataHelper2.BytesToObject<KFEraData>(eraData.Bytes0, 0, eraData.Bytes0.Length);
								return this.JunTuanEraID = kferaData.EraID;
							}
							return this.JunTuanEraID;
						}
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
						LogManager.WriteLog(2, "GetCurrentEraID Error{0}", ex, true);
					}
					junTuanEraID = this.JunTuanEraID;
				}
			}
			return junTuanEraID;
		}

		public KFEraData GetJunTuanEraData(int JunTuanID, bool noWait = false)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(noWait);
				if (null == kuaFuService)
				{
					return null;
				}
				try
				{
					lock (this.Mutex)
					{
						KuaFuData<KFEraData> kuaFuData = null;
						if (!this.EraDataDict.TryGetValue(JunTuanID, out kuaFuData))
						{
							kuaFuData = new KuaFuData<KFEraData>();
							this.EraDataDict[JunTuanID] = kuaFuData;
						}
						else if (noWait)
						{
							return kuaFuData.V;
						}
						KuaFuCmdData eraData = kuaFuService.GetEraData(JunTuanID, kuaFuData.Age);
						if (eraData == null || eraData.Age < 0L)
						{
							return null;
						}
						if (eraData != null && eraData.Age > kuaFuData.Age)
						{
							kuaFuData.Age = eraData.Age;
							if (null != eraData.Bytes0)
							{
								kuaFuData.V = DataHelper2.BytesToObject<KFEraData>(eraData.Bytes0, 0, eraData.Bytes0.Length);
							}
							if (null == kuaFuData.V)
							{
								kuaFuData.V = new KFEraData();
							}
						}
						if (kuaFuData.V.EraID != this.JunTuanEraID)
						{
							this.JunTuanEraID = kuaFuData.V.EraID;
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

		public List<KFEraRankData> GetJunTuanEraRankData(bool noWait = false)
		{
			try
			{
				IJunTuanService kuaFuService = this.GetKuaFuService(noWait);
				if (null != kuaFuService)
				{
					try
					{
						lock (this.Mutex)
						{
							if (noWait)
							{
								return this.EraRankList.V;
							}
							KuaFuCmdData eraRankData = kuaFuService.GetEraRankData(this.EraRankList.Age);
							if (eraRankData == null || eraRankData.Age < 0L)
							{
								return null;
							}
							if (eraRankData != null && eraRankData.Age > this.EraRankList.Age)
							{
								this.EraRankList.Age = eraRankData.Age;
								if (null != eraRankData.Bytes0)
								{
									this.EraRankList.V = DataHelper2.BytesToObject<List<KFEraRankData>>(eraRankData.Bytes0, 0, eraRankData.Bytes0.Length);
								}
								if (null == this.EraRankList.V)
								{
									this.EraRankList.V = new List<KFEraRankData>();
								}
							}
							return this.EraRankList.V;
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
				LogManager.WriteException(ex.ToString());
			}
			return null;
		}

		private static JunTuanClient instance = new JunTuanClient();

		public static SafeLock LockRoot = new SafeLock(null, 0, false);

		private SafeLock Mutex = new SafeLock(JunTuanClient.LockRoot, 2, false);

		private SafeLock RemotingMutex = new SafeLock(JunTuanClient.LockRoot, 1, false);

		private ICoreInterface CoreInterface = null;

		private IJunTuanService KuaFuService = null;

		private bool ClientInitialized = false;

		private KuaFuClientContext ClientInfo = new KuaFuClientContext();

		private int CurrentRequestCount = 0;

		private int MaxRequestCount = 50;

		private Dictionary<int, JunTuanDetailData> JunTuanDetailDataDict = new Dictionary<int, JunTuanDetailData>();

		private Dictionary<int, JunTuanBaseData> JunTuanBaseDataDict = new Dictionary<int, JunTuanBaseData>();

		private KuaFuData<List<JunTuanBaseData>> JunTuanBaseDataList = new KuaFuData<List<JunTuanBaseData>>();

		private KuaFuData<List<JunTuanRankData>> JunTuanRankDataList = new KuaFuData<List<JunTuanRankData>>();

		private KuaFuData<List<JunTuanEventLog>> JunTuanEventLogList = new KuaFuData<List<JunTuanEventLog>>();

		private Dictionary<int, int> BangHuiJunTuanIdDict = new Dictionary<int, int>();

		private Dictionary<int, KuaFuData<KarenFuBenData>> KarenFuBenDataDict = new Dictionary<int, KuaFuData<KarenFuBenData>>();

		private Dictionary<int, KFPrisonRoleAllData> YaoSaiPrisonRoleDataDict = new Dictionary<int, KFPrisonRoleAllData>();

		private Dictionary<int, KFPrisonFuLuAllData> YaoSaiOwnerVsFuLuDict = new Dictionary<int, KFPrisonFuLuAllData>();

		private Dictionary<int, KFPrisonJingJiAllData> YaoSaiPrisonJingJiDataDict = new Dictionary<int, KFPrisonJingJiAllData>();

		private Dictionary<int, KFPrisonLogAllData> YaoSaiPrisonLogDataDict = new Dictionary<int, KFPrisonLogAllData>();

		private KuaFuData<List<KFEraRankData>> EraRankList = new KuaFuData<List<KFEraRankData>>();

		private Dictionary<int, KuaFuData<KFEraData>> EraDataDict = new Dictionary<int, KuaFuData<KFEraData>>();

		private volatile int JunTuanEraID = -1;

		private DateTime NextClearFuBenTime;

		private string RemoteServiceUri = null;

		private long AsyncDataItemAge;

		private DuplexChannelFactory<IJunTuanService> channelFactory;

		private KuaFuData<List<JunTuanMiniData>> JunTuanList = new KuaFuData<List<JunTuanMiniData>>();

		private Dictionary<int, KuaFuData<JunTuanTaskAllData>> JunTuanTaskAllDataDict = new Dictionary<int, KuaFuData<JunTuanTaskAllData>>();

		private Dictionary<int, KuaFuData<List<JunTuanRequestData>>> JunTuanRequestListDict = new Dictionary<int, KuaFuData<List<JunTuanRequestData>>>();
	}
}
