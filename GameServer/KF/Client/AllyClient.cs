using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.Interface;
using Tmsk.Contract.KuaFuData;

namespace KF.Client
{
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, IgnoreExtensionDataObject = true, UseSynchronizationContext = false)]
	public class AllyClient : MarshalByRefObject, IKuaFuClient, IManager2
	{
		public static AllyClient getInstance()
		{
			return AllyClient.instance;
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			this._CoreInterface = coreInterface;
			this._ClientInfo.PTID = GameManager.PTID;
			this._ClientInfo.ServerId = GameManager.ServerId;
			this._ClientInfo.GameType = 14;
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
				lock (this._Mutex)
				{
					List<AllyData> list = null;
					switch (eventType)
					{
					case 10016:
						if (args.Length == 2)
						{
							List<AllyLogData> list2 = (List<AllyLogData>)args[1];
							this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyLogGameEvent(list2), 10004);
						}
						break;
					case 10017:
						if (args.Length == 3)
						{
							int num = (int)args[0];
							AllyData allyData = (AllyData)args[1];
							bool flag2 = (bool)args[2];
							if (!this._allyDic.TryGetValue(num, out list))
							{
								list = new List<AllyData>();
								this._allyDic.TryAdd(num, list);
							}
							AllyData allyData2 = this.GetAllyData(num, allyData.UnionID);
							if (allyData2 == null)
							{
								list.Add(allyData);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyGameEvent(num), 10004);
								if (flag2)
								{
									this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyTipGameEvent(num, 14113), 10004);
								}
							}
						}
						break;
					case 10018:
						if (args.Length == 2)
						{
							int num = (int)args[0];
							int num2 = (int)args[1];
							AllyData allyData3 = this.GetAllyData(num, num2);
							if (allyData3 != null && this._allyDic.TryGetValue(num, out list))
							{
								list.Remove(allyData3);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyTipGameEvent(num, 14113), 10004);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyGameEvent(num), 10004);
							}
						}
						break;
					case 10019:
						if (args.Length == 2)
						{
							int num = (int)args[0];
							AllyData allyData = (AllyData)args[1];
							if (!this._acceptDic.TryGetValue(num, out list))
							{
								list = new List<AllyData>();
								this._acceptDic.TryAdd(num, list);
							}
							AllyData allyData2 = this.GetAcceptData(num, allyData.UnionID);
							if (allyData2 == null)
							{
								list.Add(allyData);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyTipGameEvent(num, 14112), 10004);
							}
						}
						break;
					case 10020:
						if (args.Length == 2)
						{
							int num = (int)args[0];
							int num2 = (int)args[1];
							AllyData allyData3 = this.GetAcceptData(num, num2);
							if (allyData3 != null && this._acceptDic.TryGetValue(num, out list))
							{
								list.Remove(allyData3);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyTipGameEvent(num, 14112), 10004);
							}
						}
						break;
					case 10021:
						if (args.Length == 2)
						{
							int num = (int)args[0];
							AllyData allyData = (AllyData)args[1];
							if (!this._requestDic.TryGetValue(num, out list))
							{
								list = new List<AllyData>();
								this._requestDic.TryAdd(num, list);
							}
							AllyData allyData2 = this.GetRequestData(num, allyData.UnionID);
							if (allyData2 == null)
							{
								list.Add(allyData);
							}
						}
						break;
					case 10022:
						if (args.Length == 2)
						{
							int num = (int)args[0];
							int num2 = (int)args[1];
							AllyData allyData3 = this.GetRequestData(num, num2);
							if (allyData3 != null && this._requestDic.TryGetValue(num, out list))
							{
								list.Remove(allyData3);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyTipGameEvent(num, 14113), 10004);
							}
						}
						break;
					case 10023:
						if (args.Length == 2)
						{
							int num = (int)args[0];
							AllyData allyData4 = (AllyData)args[1];
							AllyData allyData3 = this.GetAllyData(num, allyData4.UnionID);
							if (allyData3 != null && this._allyDic.TryGetValue(num, out list))
							{
								list.Remove(allyData3);
								list.Add(allyData4);
							}
							allyData3 = this.GetRequestData(num, allyData4.UnionID);
							if (allyData3 != null && this._requestDic.TryGetValue(num, out list))
							{
								list.Remove(allyData3);
								list.Add(allyData3);
							}
							allyData3 = this.GetAcceptData(num, allyData4.UnionID);
							if (allyData3 != null && this._acceptDic.TryGetValue(num, out list))
							{
								list.Remove(allyData3);
								list.Add(allyData3);
							}
						}
						break;
					case 10024:
						if (args.Length == 2)
						{
							AllyData allyData5 = (AllyData)args[0];
							AllyData allyData4 = (AllyData)args[1];
							if (!this._allyDic.TryGetValue(allyData5.UnionID, out list))
							{
								list = new List<AllyData>();
								this._allyDic.TryAdd(allyData5.UnionID, list);
							}
							AllyData allyData2 = this.GetAllyData(allyData5.UnionID, allyData4.UnionID);
							if (allyData2 == null)
							{
								list.Add(allyData4);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyGameEvent(allyData5.UnionID), 10004);
							}
							List<AllyData> list3 = null;
							if (!this._allyDic.TryGetValue(allyData4.UnionID, out list3))
							{
								list3 = new List<AllyData>();
								this._allyDic.TryAdd(allyData4.UnionID, list3);
							}
							allyData2 = this.GetAllyData(allyData4.UnionID, allyData5.UnionID);
							if (allyData2 == null)
							{
								list3.Add(allyData5);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyGameEvent(allyData4.UnionID), 10004);
							}
						}
						break;
					case 10025:
						if (args.Length == 2)
						{
							int num = (int)args[0];
							int num2 = (int)args[1];
							AllyData allyData3 = this.GetAllyData(num, num2);
							if (allyData3 != null && this._allyDic.TryGetValue(num, out list))
							{
								list.Remove(allyData3);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyGameEvent(num), 10004);
							}
							AllyData allyData6 = this.GetAllyData(num2, num);
							List<AllyData> list3 = null;
							if (allyData6 != null && this._allyDic.TryGetValue(num2, out list3))
							{
								list3.Remove(allyData6);
								this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyGameEvent(num2), 10004);
							}
						}
						break;
					case 10026:
						if (args.Length == 1)
						{
							KFRankData kfrankData = (KFRankData)args[0];
							lock (this._lockRank)
							{
								if (!this._rankDic.ContainsKey(kfrankData.RankType))
								{
									this._rankDic.Add(kfrankData.RankType, new Dictionary<int, KFRankData>());
								}
								Dictionary<int, KFRankData> dictionary = this._rankDic[kfrankData.RankType];
								if (dictionary.ContainsKey(kfrankData.RoleID))
								{
									dictionary[kfrankData.RoleID] = kfrankData;
								}
								else
								{
									dictionary.Add(kfrankData.RoleID, kfrankData);
								}
							}
						}
						break;
					case 10027:
						if (args.Length == 3)
						{
							int key = (int)args[0];
							int num3 = (int)args[1];
							lock (this._lockRank)
							{
								List<KFRankData> value = (List<KFRankData>)args[2];
								if (!this._rankTopDic.ContainsKey(key))
								{
									this._rankTopDic.Add(key, new List<KFRankData>());
								}
								this._rankTopDic[key] = value;
								if (1 == num3 && this._rankDic.ContainsKey(key))
								{
									this._rankDic[key].Clear();
								}
							}
						}
						break;
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

		public void TimerProc(object sender, EventArgs e)
		{
			try
			{
				string runtimeVariable = this._CoreInterface.GetRuntimeVariable("AllyUri", null);
				if (this._RemoteServiceUri != runtimeVariable)
				{
					this._RemoteServiceUri = runtimeVariable;
				}
				IAllyService kuaFuService = this.GetKuaFuService(false);
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
				DateTime t = TimeUtil.NowDateTime();
				if (t > this._versionTime)
				{
					this._versionTime = t.AddSeconds(30.0);
					if (!this.VersionIsEqual())
					{
						this._unionDic.Clear();
						this._allyDic.Clear();
						this._requestDic.Clear();
						this._acceptDic.Clear();
						this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyStartGameEvent(), 10004);
					}
				}
			}
			catch (Exception ex)
			{
				this.ResetKuaFuService();
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

		private IAllyService GetKuaFuService(bool noWait = false)
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
				lock (this._Mutex)
				{
					IAllyService allyService;
					if (this._KuaFuService == null)
					{
						allyService = (IAllyService)Activator.GetObject(typeof(IAllyService), this._RemoteServiceUri);
						if (null == allyService)
						{
							return null;
						}
					}
					else
					{
						allyService = this._KuaFuService;
					}
					int num = this._ClientInfo.ClientId;
					long num2 = TimeUtil.NOW();
					if (this._ClientInfo.ClientId <= 0 || Math.Abs(num2 - this._ClientInfo.LastInitClientTicks) > 12000L)
					{
						this._ClientInfo.LastInitClientTicks = num2;
						num = allyService.InitializeClient(this._ClientInfo);
					}
					if (allyService != null && (num != this._ClientInfo.ClientId || this._KuaFuService != allyService))
					{
						lock (this._Mutex)
						{
							if (num > 0)
							{
								this._KuaFuService = allyService;
							}
							else
							{
								this._KuaFuService = null;
							}
							this._ClientInfo.ClientId = num;
							return allyService;
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

		private void CloseConnection()
		{
			this._ClientInfo.ClientId = 0;
			this._RemoteServiceUri = this._CoreInterface.GetRuntimeVariable("AllyUri", null);
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

		private bool VersionIsEqual()
		{
			bool result;
			lock (this._Mutex)
			{
				IAllyService kuaFuService = this.GetKuaFuService(true);
				if (null == kuaFuService)
				{
					result = false;
				}
				else
				{
					long unionAllyVersion = this._unionAllyVersion;
					this._unionAllyVersion = kuaFuService.UnionAllyVersion(this._ClientInfo.ServerId);
					result = (this._unionAllyVersion == unionAllyVersion && this._unionAllyVersion > 0L);
				}
			}
			return result;
		}

		public EAlly HUnionAllyInit(int unionID, bool isKF)
		{
			EAlly eally = -17;
			try
			{
				lock (this._Mutex)
				{
					IAllyService kuaFuService = this.GetKuaFuService(true);
					if (null == kuaFuService)
					{
						return eally;
					}
					DateTime dateTime;
					if (this._unionDic.TryGetValue(unionID, out dateTime))
					{
						this._unionDic[unionID] = TimeUtil.NowDateTime();
						return 50;
					}
					try
					{
						eally = kuaFuService.UnionAllyInit(this._ClientInfo.ServerId, unionID, isKF);
						if (eally == 50)
						{
							this._unionDic.TryAdd(unionID, TimeUtil.NowDateTime());
							this.HAllyDataList(unionID, 1);
							this.HAllyDataList(unionID, 3);
							this.HAllyDataList(unionID, 2);
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
			return eally;
		}

		public EAlly HUnionDel(int unionID)
		{
			EAlly eally = -17;
			try
			{
				lock (this._Mutex)
				{
					IAllyService kuaFuService = this.GetKuaFuService(false);
					if (null == kuaFuService)
					{
						return eally;
					}
					try
					{
						eally = kuaFuService.UnionDel(this._ClientInfo.ServerId, unionID);
						if (eally == 50)
						{
							DateTime dateTime;
							this._unionDic.TryRemove(unionID, out dateTime);
							List<AllyData> list;
							this._allyDic.TryRemove(unionID, out list);
							this._requestDic.TryRemove(unionID, out list);
							this._acceptDic.TryRemove(unionID, out list);
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
			return eally;
		}

		public EAlly HUnionDataChange(AllyData unionData)
		{
			EAlly eally = -17;
			try
			{
				lock (this._Mutex)
				{
					IAllyService kuaFuService = this.GetKuaFuService(false);
					if (null == kuaFuService)
					{
						return eally;
					}
					try
					{
						eally = kuaFuService.UnionDataChange(this._ClientInfo.ServerId, unionData);
						if (eally == 50)
						{
							int unionID = unionData.UnionID;
							DateTime dateTime;
							if (this._unionDic.TryGetValue(unionID, out dateTime))
							{
								this._unionDic[unionID] = TimeUtil.NowDateTime();
								return 50;
							}
							this._unionDic.TryAdd(unionID, TimeUtil.NowDateTime());
							this.HAllyDataList(unionID, 1);
							this.HAllyDataList(unionID, 3);
							this.HAllyDataList(unionID, 2);
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
			return eally;
		}

		public List<AllyData> HAllyDataList(int unionID, EAllyDataType type)
		{
			List<AllyData> list = new List<AllyData>();
			lock (this._Mutex)
			{
				ConcurrentDictionary<int, List<AllyData>> concurrentDictionary = null;
				switch (type)
				{
				case 1:
					concurrentDictionary = this._allyDic;
					break;
				case 2:
					concurrentDictionary = this._acceptDic;
					break;
				case 3:
					concurrentDictionary = this._requestDic;
					break;
				}
				if (concurrentDictionary.TryGetValue(unionID, out list))
				{
					return list;
				}
				IAllyService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return list;
				}
				try
				{
					list = kuaFuService.AllyDataList(this._ClientInfo.ServerId, unionID, type);
					if (list != null)
					{
						concurrentDictionary.TryAdd(unionID, list);
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
				}
			}
			return list;
		}

		public EAlly HAllyOperate(int unionID, int targetID, EAllyOperate operateType)
		{
			EAlly eally = -17;
			try
			{
				lock (this._Mutex)
				{
					if (!this.VersionIsEqual())
					{
						this._unionDic.Clear();
						this._allyDic.Clear();
						this._requestDic.Clear();
						this._acceptDic.Clear();
						this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyStartGameEvent(), 10004);
						return eally;
					}
					ConcurrentDictionary<int, List<AllyData>> concurrentDictionary = null;
					switch (operateType)
					{
					case 1:
					case 2:
						concurrentDictionary = this._acceptDic;
						break;
					case 3:
						concurrentDictionary = this._allyDic;
						break;
					case 4:
						concurrentDictionary = this._requestDic;
						break;
					}
					List<AllyData> list = null;
					if (!concurrentDictionary.TryGetValue(unionID, out list))
					{
						return -13;
					}
					AllyData allyData = concurrentDictionary[unionID].Find((AllyData data) => data.UnionID == targetID);
					if (allyData == null)
					{
						return -13;
					}
					IAllyService kuaFuService = this.GetKuaFuService(false);
					if (null == kuaFuService)
					{
						return -16;
					}
					try
					{
						eally = kuaFuService.AllyOperate(this._ClientInfo.ServerId, unionID, targetID, operateType, false);
						if (eally == 12 || eally == 11 || eally == 31 || eally == 41)
						{
							concurrentDictionary[unionID].Remove(allyData);
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
			return eally;
		}

		public EAlly HAllyRequest(int unionID, int zoneID, string unionName)
		{
			try
			{
				lock (this._Mutex)
				{
					if (!this.VersionIsEqual())
					{
						this._unionDic.Clear();
						this._allyDic.Clear();
						this._requestDic.Clear();
						this._acceptDic.Clear();
						this._CoreInterface.GetEventSourceInterface().fireEvent(new KFNotifyAllyStartGameEvent(), 10004);
						return -17;
					}
					IAllyService kuaFuService = this.GetKuaFuService(false);
					if (null == kuaFuService)
					{
						return -16;
					}
					try
					{
						AllyData allyData = kuaFuService.AllyRequest(this._ClientInfo.ServerId, unionID, zoneID, unionName);
						if (allyData.LogState == 1)
						{
							List<AllyData> list = null;
							if (!this._requestDic.TryGetValue(unionID, out list))
							{
								list = new List<AllyData>();
								this._requestDic.TryAdd(unionID, list);
							}
							list.Add(allyData);
							return allyData.LogState;
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
			return -1;
		}

		public bool UnionIsAlly(int unionID, int zoneID, string unionName)
		{
			bool result;
			lock (this._Mutex)
			{
				List<AllyData> list = null;
				if (this._allyDic.TryGetValue(unionID, out list))
				{
					AllyData allyData = list.Find((AllyData data) => data.UnionZoneID == zoneID && data.UnionName == unionName);
					if (allyData != null)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public bool UnionIsRequest(int unionID, int zoneID, string unionName)
		{
			bool result;
			lock (this._Mutex)
			{
				List<AllyData> list = null;
				if (this._requestDic.TryGetValue(unionID, out list))
				{
					AllyData allyData = list.Find((AllyData data) => data.UnionZoneID == zoneID && data.UnionName == unionName);
					if (allyData != null)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public bool UnionIsAccept(int unionID, int zoneID, string unionName)
		{
			bool result;
			lock (this._Mutex)
			{
				List<AllyData> list = null;
				if (this._acceptDic.TryGetValue(unionID, out list))
				{
					AllyData allyData = list.Find((AllyData data) => data.UnionZoneID == zoneID && data.UnionName == unionName);
					if (allyData != null)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public int AllyCount(int unionID)
		{
			int result;
			lock (this._Mutex)
			{
				if (this._allyDic.ContainsKey(unionID))
				{
					result = this._allyDic[unionID].Count;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		public int AllyRequestCount(int unionID)
		{
			int result;
			lock (this._Mutex)
			{
				if (this._requestDic.ContainsKey(unionID))
				{
					result = this._requestDic[unionID].Count;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		private AllyData GetAllyData(int unionID, int targetID)
		{
			AllyData result;
			lock (this._Mutex)
			{
				List<AllyData> list = null;
				if (this._allyDic.TryGetValue(unionID, out list))
				{
					result = list.Find((AllyData data) => data.UnionID == targetID);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private AllyData GetRequestData(int unionID, int targetID)
		{
			AllyData result;
			lock (this._Mutex)
			{
				List<AllyData> list = null;
				if (this._requestDic.TryGetValue(unionID, out list))
				{
					result = list.Find((AllyData data) => data.UnionID == targetID);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private AllyData GetAcceptData(int unionID, int targetID)
		{
			AllyData result;
			lock (this._Mutex)
			{
				List<AllyData> list = null;
				if (this._acceptDic.TryGetValue(unionID, out list))
				{
					result = list.Find((AllyData data) => data.UnionID == targetID);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private object _lockRank
		{
			get
			{
				return this._Mutex;
			}
		}

		public void HRankClear()
		{
			this._rankTopDic = new Dictionary<int, List<KFRankData>>();
			this._rankDic = new Dictionary<int, Dictionary<int, KFRankData>>();
		}

		public KFRankData HRankData(int rankType, int roleID)
		{
			KFRankData result;
			lock (this._lockRank)
			{
				if (this._rankTopDic.ContainsKey(rankType))
				{
					List<KFRankData> list = this._rankTopDic[rankType];
					KFRankData kfrankData = list.Find((KFRankData _x) => _x != null && _x.RoleID == roleID);
					if (kfrankData != null)
					{
						return kfrankData;
					}
				}
				if (this._rankDic.ContainsKey(rankType))
				{
					Dictionary<int, KFRankData> dictionary = this._rankDic[rankType];
					if (dictionary.ContainsKey(roleID))
					{
						return dictionary[roleID];
					}
				}
				else
				{
					this._rankDic.Add(rankType, new Dictionary<int, KFRankData>());
				}
				IAllyService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					result = null;
				}
				else
				{
					try
					{
						KFRankData kfrankData = kuaFuService.RankRole(this._ClientInfo.ServerId, rankType, roleID);
						if (kfrankData != null)
						{
							this._rankDic[rankType].Add(roleID, kfrankData);
						}
						return kfrankData;
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
					result = null;
				}
			}
			return result;
		}

		public List<KFRankData> HRankTopList(int rankType)
		{
			List<KFRankData> list = new List<KFRankData>();
			lock (this._lockRank)
			{
				if (this._rankTopDic.TryGetValue(rankType, out list))
				{
					return list;
				}
				IAllyService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					return list;
				}
				try
				{
					list = kuaFuService.RankTopList(this._ClientInfo.ServerId, rankType);
					if (list != null)
					{
						this._rankTopDic.Add(rankType, list);
					}
				}
				catch (Exception ex)
				{
					this.ResetKuaFuService();
					LogManager.WriteLog(2, "HRankList Error{0}", ex, true);
				}
			}
			return list;
		}

		public KFRankData HRankUpdate(int rankType, int grade, int roleID, int zoneID, string roleName, byte[] roleData)
		{
			KFRankData result;
			lock (this._lockRank)
			{
				KFRankData kfrankData = new KFRankData();
				kfrankData.RankType = rankType;
				kfrankData.Rank = -1;
				kfrankData.ZoneID = zoneID;
				kfrankData.RoleID = roleID;
				kfrankData.RoleName = roleName;
				kfrankData.Grade = grade;
				kfrankData.RoleData = roleData;
				kfrankData.RankTime = TimeUtil.NowDateTime();
				kfrankData.ServerID = this._ClientInfo.ServerId;
				IAllyService kuaFuService = this.GetKuaFuService(false);
				if (null == kuaFuService)
				{
					result = null;
				}
				else
				{
					try
					{
						kuaFuService.RankGradeUpdate(this._ClientInfo.ServerId, kfrankData);
					}
					catch (Exception ex)
					{
						this.ResetKuaFuService();
					}
					result = null;
				}
			}
			return result;
		}

		public const int _gameType = 14;

		public const int _sceneType = 10004;

		private const int ALLY_VERSION_SPAN_SECOND = 30;

		private static AllyClient instance = new AllyClient();

		public object _Mutex = new object();

		private ICoreInterface _CoreInterface = null;

		private IAllyService _KuaFuService = null;

		private KuaFuClientContext _ClientInfo = new KuaFuClientContext();

		private string _RemoteServiceUri = null;

		private DateTime _versionTime = DateTime.MinValue;

		private long _unionAllyVersion = 0L;

		private ConcurrentDictionary<int, DateTime> _unionDic = new ConcurrentDictionary<int, DateTime>();

		private ConcurrentDictionary<int, List<AllyData>> _allyDic = new ConcurrentDictionary<int, List<AllyData>>();

		private ConcurrentDictionary<int, List<AllyData>> _requestDic = new ConcurrentDictionary<int, List<AllyData>>();

		private ConcurrentDictionary<int, List<AllyData>> _acceptDic = new ConcurrentDictionary<int, List<AllyData>>();

		private DuplexChannelFactory<IAllyService> channelFactory;

		public Dictionary<int, List<KFRankData>> _rankTopDic = new Dictionary<int, List<KFRankData>>();

		public Dictionary<int, Dictionary<int, KFRankData>> _rankDic = new Dictionary<int, Dictionary<int, KFRankData>>();
	}
}
