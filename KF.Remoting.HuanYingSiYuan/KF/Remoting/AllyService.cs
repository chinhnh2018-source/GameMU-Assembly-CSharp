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
using KF.Remoting.Data;
using Server.Tools;
using Tmsk.Contract.Interface;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
	public class AllyService : MarshalByRefObject, IAllyService
	{
		public override object InitializeLifetimeService()
		{
			AllyService.Instance = this;
			ILease lease = (ILease)base.InitializeLifetimeService();
			if (lease.CurrentState == LeaseState.Initial)
			{
				lease.InitialLeaseTime = TimeSpan.FromDays(2000.0);
			}
			return lease;
		}

		public AllyService()
		{
			AllyService.Instance = this;
			this._BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this._BackgroundThread.IsBackground = true;
			this._BackgroundThread.Start();
		}

		~AllyService()
		{
			this._BackgroundThread.Abort();
		}

		public void ThreadProc(object state)
		{
			do
			{
				Thread.Sleep(1000);
			}
			while (!this._Persistence.Initialized);
			for (;;)
			{
				try
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					Global.UpdateNowTime(dateTime);
					if (dateTime > this._clearTimeRequest)
					{
						this._clearTimeRequest = dateTime.AddSeconds(30.0);
						this.ClearAcceptData();
						this.ClearRequestData();
					}
					RankService.getInstance().Update();
					int num = (int)(TimeUtil.NowDateTime() - dateTime).TotalMilliseconds;
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

		private void ClearAcceptData()
		{
			lock (this._Mutex)
			{
				if (this._acceptDic != null && this._acceptDic.Count > 0)
				{
					foreach (KeyValuePair<int, List<KFAllyData>> keyValuePair in this._acceptDic)
					{
						int key = keyValuePair.Key;
						List<KFAllyData> value = keyValuePair.Value;
						IEnumerable<KFAllyData> source = from info in value
						where info.LogTime <= TimeUtil.NowDateTime().AddSeconds((double)(-(double)Consts.AllyRequestClearSecond))
						select info;
						if (source.Any<KFAllyData>())
						{
							KFAllyData unionData = this.GetUnionData(key);
							List<KFAllyData> list = source.ToList<KFAllyData>();
							foreach (KFAllyData kfallyData in list)
							{
								int num = this.AllyOperate(unionData.ServerID, unionData.UnionID, kfallyData.UnionID, 2, false);
							}
						}
					}
				}
			}
		}

		private void ClearRequestData()
		{
			lock (this._Mutex)
			{
				if (this._requestDic != null && this._requestDic.Count > 0)
				{
					foreach (KeyValuePair<int, List<KFAllyData>> keyValuePair in this._requestDic)
					{
						int key = keyValuePair.Key;
						List<KFAllyData> value = keyValuePair.Value;
						IEnumerable<KFAllyData> source = from info in value
						where info.LogTime <= TimeUtil.NowDateTime().AddSeconds((double)(-(double)Consts.AllyRequestClearSecond))
						select info;
						if (source.Any<KFAllyData>())
						{
							KFAllyData unionData = this.GetUnionData(key);
							List<KFAllyData> list = source.ToList<KFAllyData>();
							foreach (KFAllyData kfallyData in list)
							{
								int num = this.AllyOperate(kfallyData.ServerID, kfallyData.UnionID, unionData.UnionID, 2, false);
							}
						}
					}
				}
			}
		}

		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.GameType == this._gameType && clientInfo.ServerId != 0)
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

		public AsyncDataItem[] GetClientCacheItems(int serverID)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverID, this._gameType);
		}

		public long UnionAllyVersion(int serverID)
		{
			long result;
			if (!this.IsAgent(serverID))
			{
				result = 0L;
			}
			else
			{
				result = this._Persistence.DataVersion;
			}
			return result;
		}

		public int UnionAllyInit(int serverID, int unionID, bool isKF)
		{
			int result;
			if (!this.IsAgent(serverID))
			{
				result = -16;
			}
			else
			{
				lock (this._Mutex)
				{
					KFAllyData kfallyData;
					if (!this._unionDic.TryGetValue(unionID, out kfallyData))
					{
						kfallyData = AllyPersistence.Instance.DBUnionDataGet(unionID);
						if (kfallyData == null)
						{
							return -18;
						}
						this._unionDic.TryAdd(unionID, kfallyData);
					}
					if (!isKF && kfallyData.ServerID != serverID)
					{
						kfallyData.ServerID = serverID;
						if (!AllyPersistence.Instance.DBUnionDataUpdate(kfallyData))
						{
							return -17;
						}
					}
					if (!isKF)
					{
						this.CheckAllyLog(serverID, unionID);
					}
					kfallyData.UpdateLogtime();
					result = 50;
				}
			}
			return result;
		}

		private void CheckAllyLog(int serverID, int unionID)
		{
			List<AllyLogData> list = AllyPersistence.Instance.DBAllyLogList(unionID);
			if (list != null && list.Count > 0)
			{
				ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(10016, new object[]
				{
					unionID,
					list
				}));
			}
		}

		public int UnionDataChange(int serverID, AllyData newData)
		{
			int result;
			if (!this.IsAgent(serverID))
			{
				result = -16;
			}
			else
			{
				lock (this._Mutex)
				{
					KFAllyData kfallyData = new KFAllyData();
					kfallyData.Copy(newData);
					kfallyData.LogTime = TimeUtil.NowDateTime();
					kfallyData.ServerID = serverID;
					kfallyData.UpdateLogtime();
					if (!AllyPersistence.Instance.DBUnionDataUpdate(kfallyData))
					{
						result = -17;
					}
					else
					{
						KFAllyData kfallyData2;
						if (this._unionDic.TryGetValue(kfallyData.UnionID, out kfallyData2))
						{
							this._unionDic[kfallyData.UnionID] = kfallyData;
						}
						else
						{
							this._unionDic.TryAdd(kfallyData.UnionID, kfallyData);
						}
						List<int> list;
						if (this._allyDic.TryGetValue(kfallyData.UnionID, out list))
						{
							AllyData allyData = new AllyData();
							allyData.Copy(kfallyData);
							allyData.LogState = 12;
							foreach (int unionID in list)
							{
								KFAllyData kfallyData3 = this.GetUnionData(unionID);
								if (this.IsAgent(kfallyData3.ServerID))
								{
									ClientAgentManager.Instance().PostAsyncEvent(kfallyData3.ServerID, this._gameType, new AsyncDataItem(10023, new object[]
									{
										kfallyData3.UnionID,
										allyData
									}));
								}
							}
						}
						AllyData allyData2 = new AllyData();
						allyData2.Copy(kfallyData);
						allyData2.LogState = 1;
						List<KFAllyData> list2 = null;
						if (this._requestDic.TryGetValue(kfallyData.UnionID, out list2))
						{
							foreach (KFAllyData kfallyData3 in list2)
							{
								List<KFAllyData> list3 = null;
								if (this.IsAgent(kfallyData3.ServerID) && this._acceptDic.TryGetValue(kfallyData3.UnionID, out list3))
								{
									KFAllyData acceptData = this.GetAcceptData(kfallyData3.UnionID, kfallyData.UnionID);
									if (acceptData != null)
									{
										list3.Remove(acceptData);
										list3.Add(kfallyData);
										ClientAgentManager.Instance().PostAsyncEvent(kfallyData3.ServerID, this._gameType, new AsyncDataItem(10023, new object[]
										{
											kfallyData3.UnionID,
											allyData2
										}));
									}
								}
							}
						}
						List<KFAllyData> list4 = null;
						if (this._acceptDic.TryGetValue(kfallyData.UnionID, out list4))
						{
							foreach (KFAllyData kfallyData3 in list4)
							{
								List<KFAllyData> list5 = null;
								if (this.IsAgent(kfallyData3.ServerID) && this._requestDic.TryGetValue(kfallyData3.UnionID, out list5))
								{
									KFAllyData requestData = this.GetRequestData(kfallyData3.UnionID, kfallyData.UnionID);
									if (requestData != null)
									{
										list5.Remove(requestData);
										list5.Add(kfallyData);
										ClientAgentManager.Instance().PostAsyncEvent(kfallyData3.ServerID, this._gameType, new AsyncDataItem(10023, new object[]
										{
											kfallyData3.UnionID,
											allyData2
										}));
									}
								}
							}
						}
						result = 50;
					}
				}
			}
			return result;
		}

		public int UnionDel(int serverID, int unionID)
		{
			int result;
			if (!this.IsAgent(serverID))
			{
				result = -16;
			}
			else
			{
				lock (this._Mutex)
				{
					KFAllyData kfallyData;
					if (!this._unionDic.TryGetValue(unionID, out kfallyData))
					{
						result = -16;
					}
					else
					{
						List<int> list;
						if (this._allyDic.TryGetValue(unionID, out list))
						{
							foreach (int targetID in list)
							{
								int num = this.AllyOperate(serverID, unionID, targetID, 3, true);
							}
							this._allyDic.TryRemove(unionID, out list);
						}
						List<KFAllyData> list2 = null;
						if (this._requestDic.TryGetValue(unionID, out list2))
						{
							foreach (KFAllyData kfallyData2 in list2)
							{
								int num = this.AllyOperate(serverID, unionID, kfallyData2.UnionID, 4, true);
							}
							this._requestDic.TryRemove(unionID, out list2);
						}
						List<KFAllyData> list3 = null;
						if (this._acceptDic.TryGetValue(unionID, out list3))
						{
							foreach (KFAllyData kfallyData2 in list3)
							{
								int num = this.AllyOperate(serverID, unionID, kfallyData2.UnionID, 2, true);
							}
							this._acceptDic.TryRemove(unionID, out list3);
						}
						this._unionDic.TryRemove(unionID, out kfallyData);
						result = (AllyPersistence.Instance.DBUnionDataDel(unionID) ? 50 : -17);
					}
				}
			}
			return result;
		}

		public List<AllyData> AllyDataList(int serverID, int unionID, int type)
		{
			List<AllyData> list = new List<AllyData>();
			List<AllyData> result;
			if (!this.IsAgent(serverID))
			{
				result = list;
			}
			else
			{
				switch (type)
				{
				case 1:
					list = this.AllyList(unionID);
					break;
				case 2:
					list = this.AllyAcceptList(unionID);
					break;
				case 3:
					list = this.AllyRequestList(unionID);
					break;
				}
				result = list;
			}
			return result;
		}

		public List<int> InitAllyIDList(int unionID)
		{
			List<int> result;
			lock (this._Mutex)
			{
				List<int> list = null;
				if (this._allyDic.TryGetValue(unionID, out list))
				{
					result = list;
				}
				else
				{
					list = AllyPersistence.Instance.DBAllyIDList(unionID);
					this._allyDic.TryAdd(unionID, list);
					result = list;
				}
			}
			return result;
		}

		public List<AllyData> AllyList(int unionID)
		{
			List<AllyData> result;
			lock (this._Mutex)
			{
				List<int> list = this.InitAllyIDList(unionID);
				List<AllyData> list2 = new List<AllyData>();
				foreach (int unionID2 in list)
				{
					KFAllyData unionData = this.GetUnionData(unionID2);
					if (unionData != null)
					{
						list2.Add(new AllyData
						{
							UnionID = unionData.UnionID,
							UnionZoneID = unionData.UnionZoneID,
							UnionName = unionData.UnionName,
							UnionLevel = unionData.UnionLevel,
							UnionNum = unionData.UnionNum,
							LeaderID = unionData.LeaderID,
							LeaderZoneID = unionData.LeaderZoneID,
							LeaderName = unionData.LeaderName,
							LogState = 12
						});
					}
				}
				result = list2;
			}
			return result;
		}

		private List<KFAllyData> InitAllyRequestList(int unionID)
		{
			List<KFAllyData> result;
			lock (this._Mutex)
			{
				List<KFAllyData> list = null;
				if (this._requestDic.TryGetValue(unionID, out list))
				{
					result = list;
				}
				else
				{
					list = AllyPersistence.Instance.DBAllyRequestList(unionID);
					this._requestDic.TryAdd(unionID, list);
					List<KFAllyData> list2 = new List<KFAllyData>();
					foreach (KFAllyData kfallyData in list)
					{
						KFAllyData unionData = this.GetUnionData(kfallyData.UnionID);
						if (unionData != null)
						{
							kfallyData.UnionZoneID = unionData.UnionZoneID;
							kfallyData.UnionName = unionData.UnionName;
							kfallyData.UnionLevel = unionData.UnionLevel;
							kfallyData.UnionNum = unionData.UnionNum;
							kfallyData.LeaderID = unionData.LeaderID;
							kfallyData.LeaderZoneID = unionData.LeaderZoneID;
							kfallyData.LeaderName = unionData.LeaderName;
							kfallyData.ServerID = unionData.ServerID;
						}
					}
					result = list;
				}
			}
			return result;
		}

		public List<AllyData> AllyRequestList(int unionID)
		{
			List<AllyData> result;
			lock (this._Mutex)
			{
				List<KFAllyData> list = this.InitAllyRequestList(unionID);
				List<AllyData> list2 = new List<AllyData>();
				foreach (KFAllyData kfallyData in list)
				{
					AllyData allyData = new AllyData();
					allyData.Copy(kfallyData);
					list2.Add(allyData);
				}
				result = list2;
			}
			return result;
		}

		public void AllyRequestAdd(int unionID, KFAllyData item)
		{
			lock (this._Mutex)
			{
				List<KFAllyData> list = this.InitAllyRequestList(unionID);
				list.Add(item);
			}
		}

		public List<KFAllyData> InitAllyAcceptList(int unionID)
		{
			List<KFAllyData> result;
			lock (this._Mutex)
			{
				List<KFAllyData> list = null;
				if (this._acceptDic.TryGetValue(unionID, out list))
				{
					result = list;
				}
				else
				{
					list = AllyPersistence.Instance.DBAllyAcceptList(unionID);
					this._acceptDic.TryAdd(unionID, list);
					foreach (KFAllyData kfallyData in list)
					{
						KFAllyData unionData = this.GetUnionData(kfallyData.UnionID);
						if (unionData != null)
						{
							kfallyData.UnionZoneID = unionData.UnionZoneID;
							kfallyData.UnionName = unionData.UnionName;
							kfallyData.UnionLevel = unionData.UnionLevel;
							kfallyData.UnionNum = unionData.UnionNum;
							kfallyData.LeaderID = unionData.LeaderID;
							kfallyData.LeaderZoneID = unionData.LeaderZoneID;
							kfallyData.LeaderName = unionData.LeaderName;
							kfallyData.ServerID = unionData.ServerID;
						}
					}
					result = list;
				}
			}
			return result;
		}

		public List<AllyData> AllyAcceptList(int unionID)
		{
			List<AllyData> result;
			lock (this._Mutex)
			{
				List<KFAllyData> list = this.InitAllyAcceptList(unionID);
				List<AllyData> list2 = new List<AllyData>();
				foreach (KFAllyData kfallyData in list)
				{
					AllyData allyData = new AllyData();
					allyData.Copy(kfallyData);
					list2.Add(allyData);
				}
				result = list2;
			}
			return result;
		}

		public void AllyAcceptAdd(int unionID, KFAllyData item)
		{
			lock (this._Mutex)
			{
				List<KFAllyData> list = this.InitAllyAcceptList(unionID);
				list.Add(item);
			}
		}

		public AllyData AllyRequest(int serverID, int unionID, int zoneID, string unionName)
		{
			AllyData result;
			lock (this._Mutex)
			{
				AllyData allyData = new AllyData();
				if (!this.IsAgent(serverID))
				{
					allyData.LogState = -16;
					result = allyData;
				}
				else
				{
					KFAllyData unionData = this.GetUnionData(zoneID, unionName);
					if (unionData == null)
					{
						allyData.LogState = -3;
						result = allyData;
					}
					else
					{
						this.InitAllyIDList(unionData.UnionID);
						this.InitAllyRequestList(unionData.UnionID);
						this.InitAllyAcceptList(unionData.UnionID);
						if (this.UnionIsAlly(unionID, unionData.UnionID))
						{
							allyData.LogState = -9;
							result = allyData;
						}
						else if (this.UnionIsRequest(unionID, unionData.UnionID) || this.UnionIsAccept(unionData.UnionID, unionID))
						{
							allyData.LogState = -10;
							result = allyData;
						}
						else
						{
							int num = this._allyDic[unionID].Count + this._requestDic[unionID].Count;
							if (num >= Consts.AllyNumMax)
							{
								allyData.LogState = -11;
								result = allyData;
							}
							else
							{
								DateTime logTime = TimeUtil.NowDateTime();
								int logState = 1;
								if (!AllyPersistence.Instance.DBAllyRequestAdd(unionID, unionData.UnionID, logTime, logState))
								{
									allyData.LogState = -1;
									result = allyData;
								}
								else
								{
									KFAllyData unionData2 = this.GetUnionData(unionID);
									AllyLogData allyLogData = new AllyLogData();
									allyLogData.UnionID = unionData.UnionID;
									allyLogData.UnionZoneID = unionData.UnionZoneID;
									allyLogData.UnionName = unionData.UnionName;
									allyLogData.MyUnionID = unionID;
									allyLogData.LogTime = logTime;
									allyLogData.LogState = logState;
									ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(10016, new object[]
									{
										unionID,
										new List<AllyLogData>
										{
											allyLogData
										}
									}));
									KFAllyData kfallyData = new KFAllyData();
									kfallyData.Copy(unionData);
									kfallyData.LogState = logState;
									kfallyData.LogTime = logTime;
									kfallyData.UpdateLogtime();
									this.AllyRequestAdd(unionID, kfallyData);
									allyData.Copy(kfallyData);
									KFAllyData kfallyData2 = new KFAllyData();
									kfallyData2.Copy(unionData2);
									kfallyData2.LogState = logState;
									kfallyData2.LogTime = logTime;
									this.AllyAcceptAdd(unionData.UnionID, kfallyData2);
									if (this.IsAgent(unionData.ServerID))
									{
										AllyData allyData2 = new AllyData();
										allyData2.Copy(kfallyData2);
										ClientAgentManager.Instance().PostAsyncEvent(unionData.ServerID, this._gameType, new AsyncDataItem(10019, new object[]
										{
											unionData.UnionID,
											allyData2
										}));
									}
									result = allyData;
								}
							}
						}
					}
				}
			}
			return result;
		}

		public int AllyOperate(int serverID, int unionID, int targetID, int operateType, bool isDel = false)
		{
			int result;
			lock (this._Mutex)
			{
				int num = -17;
				KFAllyData unionData = this.GetUnionData(targetID);
				if (unionData == null)
				{
					result = -13;
				}
				else
				{
					this.InitAllyIDList(unionData.UnionID);
					this.InitAllyRequestList(unionData.UnionID);
					this.InitAllyAcceptList(unionData.UnionID);
					switch (operateType)
					{
					case 1:
						num = this.OperateAgree(serverID, unionID, targetID);
						break;
					case 2:
						num = this.OperateRefuse(serverID, unionID, targetID, isDel);
						break;
					case 3:
						num = this.OperateRemove(serverID, unionID, targetID, isDel);
						break;
					case 4:
						num = this.OperateCancel(serverID, unionID, targetID, isDel);
						break;
					}
					result = num;
				}
			}
			return result;
		}

		public int OperateRemove(int serverID, int unionID, int targetID, bool isDel = false)
		{
			int result;
			lock (this._Mutex)
			{
				KFAllyData unionData = this.GetUnionData(targetID);
				if (unionData == null)
				{
					result = -13;
				}
				else if (!this.UnionIsAlly(unionID, unionData.UnionID))
				{
					result = -17;
				}
				else if (!this.UnionIsAlly(targetID, unionID))
				{
					result = -17;
				}
				else if (!AllyPersistence.Instance.DBAllyDel(unionID, targetID))
				{
					result = -17;
				}
				else
				{
					DateTime logTime = TimeUtil.NowDateTime();
					int logState = 41;
					if (!isDel)
					{
						this._allyDic[unionID].Remove(targetID);
					}
					AllyLogData allyLogData = new AllyLogData();
					allyLogData.UnionID = unionData.UnionID;
					allyLogData.UnionZoneID = unionData.UnionZoneID;
					allyLogData.UnionName = unionData.UnionName;
					allyLogData.MyUnionID = unionID;
					allyLogData.LogTime = logTime;
					allyLogData.LogState = logState;
					ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(10016, new object[]
					{
						unionID,
						new List<AllyLogData>
						{
							allyLogData
						}
					}));
					if (this._allyDic.ContainsKey(unionData.UnionID))
					{
						this._allyDic[unionData.UnionID].Remove(unionID);
					}
					KFAllyData unionData2 = this.GetUnionData(unionID);
					allyLogData = new AllyLogData();
					allyLogData.UnionID = unionData2.UnionID;
					allyLogData.UnionZoneID = unionData2.UnionZoneID;
					allyLogData.UnionName = unionData2.UnionName;
					allyLogData.MyUnionID = targetID;
					allyLogData.LogTime = logTime;
					allyLogData.LogState = 42;
					if (this.IsAgent(unionData.ServerID))
					{
						ClientAgentManager.Instance().PostAsyncEvent(unionData.ServerID, this._gameType, new AsyncDataItem(10018, new object[]
						{
							targetID,
							unionID
						}));
						ClientAgentManager.Instance().PostAsyncEvent(unionData.ServerID, this._gameType, new AsyncDataItem(10016, new object[]
						{
							targetID,
							new List<AllyLogData>
							{
								allyLogData
							}
						}));
					}
					else
					{
						AllyPersistence.Instance.DBAllyLogAdd(allyLogData);
					}
					ClientAgentManager.Instance().KFBroadCastAsyncEvent(this._gameType, new AsyncDataItem(10025, new object[]
					{
						targetID,
						unionID
					}));
					result = 41;
				}
			}
			return result;
		}

		public int OperateCancel(int serverID, int unionID, int targetID, bool isDel = false)
		{
			int result;
			lock (this._Mutex)
			{
				KFAllyData unionData = this.GetUnionData(targetID);
				if (unionData == null)
				{
					result = -13;
				}
				else if (!this.UnionIsRequest(unionID, targetID))
				{
					result = -17;
				}
				else if (!this.UnionIsAccept(targetID, unionID))
				{
					result = -17;
				}
				else if (!AllyPersistence.Instance.DBAllyRequestDel(unionID, targetID))
				{
					result = -17;
				}
				else
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					KFAllyData requestData = this.GetRequestData(unionID, targetID);
					if (!isDel)
					{
						this._requestDic[unionID].Remove(requestData);
					}
					ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(10022, new object[]
					{
						unionID,
						targetID
					}));
					if (this._acceptDic.ContainsKey(targetID))
					{
						KFAllyData acceptData = this.GetAcceptData(targetID, unionID);
						this._acceptDic[targetID].Remove(acceptData);
					}
					if (this.IsAgent(unionData.ServerID))
					{
						ClientAgentManager.Instance().PostAsyncEvent(unionData.ServerID, this._gameType, new AsyncDataItem(10020, new object[]
						{
							targetID,
							unionID
						}));
					}
					result = 31;
				}
			}
			return result;
		}

		public int OperateAgree(int serverID, int unionID, int targetID)
		{
			int result;
			lock (this._Mutex)
			{
				KFAllyData unionData = this.GetUnionData(targetID);
				if (unionData == null)
				{
					result = -13;
				}
				else if (!this.UnionIsAccept(unionID, targetID))
				{
					result = -17;
				}
				else if (!this.UnionIsRequest(targetID, unionID))
				{
					result = -17;
				}
				else
				{
					int num = this._allyDic[unionID].Count + this._requestDic[unionID].Count;
					if (num >= Consts.AllyNumMax)
					{
						result = -11;
					}
					else
					{
						DateTime logTime = TimeUtil.NowDateTime();
						int logState = 12;
						if (!AllyPersistence.Instance.DBAllyRequestDel(targetID, unionID))
						{
							result = -17;
						}
						else if (!AllyPersistence.Instance.DBAllyAdd(unionID, targetID, logTime))
						{
							result = -17;
						}
						else
						{
							KFAllyData acceptData = this.GetAcceptData(unionID, targetID);
							this._acceptDic[unionID].Remove(acceptData);
							ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(10020, new object[]
							{
								unionID,
								targetID
							}));
							this._allyDic[unionID].Add(targetID);
							acceptData.LogTime = logTime;
							acceptData.LogState = logState;
							AllyData allyData = new AllyData();
							allyData.Copy(acceptData);
							ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(10017, new object[]
							{
								unionID,
								allyData,
								false
							}));
							if (this._requestDic.ContainsKey(targetID))
							{
								KFAllyData requestData = this.GetRequestData(targetID, unionID);
								this._requestDic[targetID].Remove(requestData);
							}
							if (this._allyDic.ContainsKey(targetID))
							{
								this._allyDic[targetID].Add(unionID);
							}
							KFAllyData unionData2 = this.GetUnionData(unionID);
							unionData2.LogTime = logTime;
							unionData2.LogState = logState;
							AllyLogData allyLogData = new AllyLogData();
							allyLogData.UnionID = unionData2.UnionID;
							allyLogData.UnionZoneID = unionData2.UnionZoneID;
							allyLogData.UnionName = unionData2.UnionName;
							allyLogData.MyUnionID = targetID;
							allyLogData.LogTime = logTime;
							allyLogData.LogState = 21;
							AllyData allyData2 = new AllyData();
							allyData2.Copy(unionData2);
							if (this.IsAgent(unionData.ServerID))
							{
								ClientAgentManager.Instance().PostAsyncEvent(unionData.ServerID, this._gameType, new AsyncDataItem(10017, new object[]
								{
									targetID,
									allyData2,
									true
								}));
								ClientAgentManager.Instance().PostAsyncEvent(unionData.ServerID, this._gameType, new AsyncDataItem(10022, new object[]
								{
									targetID,
									unionID
								}));
								ClientAgentManager.Instance().PostAsyncEvent(unionData.ServerID, this._gameType, new AsyncDataItem(10016, new object[]
								{
									targetID,
									new List<AllyLogData>
									{
										allyLogData
									}
								}));
							}
							else
							{
								AllyPersistence.Instance.DBAllyLogAdd(allyLogData);
							}
							ClientAgentManager.Instance().KFBroadCastAsyncEvent(this._gameType, new AsyncDataItem(10024, new object[]
							{
								allyData,
								allyData2
							}));
							result = 12;
						}
					}
				}
			}
			return result;
		}

		public int OperateRefuse(int serverID, int unionID, int targetID, bool isDel = false)
		{
			int result;
			lock (this._Mutex)
			{
				KFAllyData unionData = this.GetUnionData(targetID);
				if (unionData == null)
				{
					result = -13;
				}
				else if (!this.UnionIsAccept(unionID, targetID))
				{
					result = -17;
				}
				else if (!this.UnionIsRequest(targetID, unionID))
				{
					result = -17;
				}
				else if (!AllyPersistence.Instance.DBAllyRequestDel(targetID, unionID))
				{
					result = -17;
				}
				else
				{
					DateTime logTime = TimeUtil.NowDateTime();
					KFAllyData acceptData = this.GetAcceptData(unionID, targetID);
					if (!isDel)
					{
						this._acceptDic[unionID].Remove(acceptData);
					}
					ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(10020, new object[]
					{
						unionID,
						targetID
					}));
					if (this._requestDic.ContainsKey(targetID))
					{
						KFAllyData requestData = this.GetRequestData(targetID, unionID);
						this._requestDic[targetID].Remove(requestData);
					}
					KFAllyData unionData2 = this.GetUnionData(unionID);
					AllyLogData allyLogData = new AllyLogData();
					allyLogData.UnionID = unionData2.UnionID;
					allyLogData.UnionZoneID = unionData2.UnionZoneID;
					allyLogData.UnionName = unionData2.UnionName;
					allyLogData.MyUnionID = targetID;
					allyLogData.LogTime = logTime;
					allyLogData.LogState = 20;
					if (this.IsAgent(unionData.ServerID))
					{
						ClientAgentManager.Instance().PostAsyncEvent(unionData.ServerID, this._gameType, new AsyncDataItem(10022, new object[]
						{
							targetID,
							unionID
						}));
						ClientAgentManager.Instance().PostAsyncEvent(unionData.ServerID, this._gameType, new AsyncDataItem(10016, new object[]
						{
							targetID,
							new List<AllyLogData>
							{
								allyLogData
							}
						}));
					}
					else
					{
						AllyPersistence.Instance.DBAllyLogAdd(allyLogData);
					}
					result = 11;
				}
			}
			return result;
		}

		private KFAllyData GetUnionData(int unionID)
		{
			KFAllyData result;
			lock (this._Mutex)
			{
				KFAllyData kfallyData = null;
				if (!this._unionDic.TryGetValue(unionID, out kfallyData))
				{
					kfallyData = AllyPersistence.Instance.DBUnionDataGet(unionID);
					if (kfallyData != null)
					{
						this._unionDic.TryAdd(unionID, kfallyData);
					}
				}
				result = kfallyData;
			}
			return result;
		}

		private KFAllyData GetUnionData(int unionZoneID, string unionName)
		{
			KFAllyData result;
			lock (this._Mutex)
			{
				IEnumerable<KFAllyData> source = from item in this._unionDic.Values
				where item.UnionZoneID == unionZoneID && item.UnionName == unionName
				select item;
				KFAllyData kfallyData;
				if (source.Any<KFAllyData>())
				{
					kfallyData = source.First<KFAllyData>();
				}
				else
				{
					kfallyData = AllyPersistence.Instance.DBUnionDataGet(unionZoneID, unionName);
					if (kfallyData != null)
					{
						this._unionDic.TryAdd(kfallyData.UnionID, kfallyData);
					}
				}
				result = kfallyData;
			}
			return result;
		}

		private bool UnionIsAlly(int unionID, int targetID)
		{
			bool result;
			lock (this._Mutex)
			{
				List<AllyData> list = this.AllyList(unionID);
				if (list != null && list.Count > 0)
				{
					AllyData allyData = list.Find((AllyData data) => data.UnionID == targetID);
					if (allyData != null)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		private bool UnionIsRequest(int unionID, int targetID)
		{
			bool result;
			lock (this._Mutex)
			{
				KFAllyData requestData = this.GetRequestData(unionID, targetID);
				if (requestData != null)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private KFAllyData GetRequestData(int unionID, int targetID)
		{
			KFAllyData result;
			lock (this._Mutex)
			{
				List<KFAllyData> list = this.InitAllyRequestList(unionID);
				if (list != null && list.Count > 0)
				{
					result = list.Find((KFAllyData data) => data.UnionID == targetID);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private bool UnionIsAccept(int unionID, int targetID)
		{
			bool result;
			lock (this._Mutex)
			{
				KFAllyData acceptData = this.GetAcceptData(unionID, targetID);
				if (acceptData != null)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private KFAllyData GetAcceptData(int unionID, int targetID)
		{
			KFAllyData result;
			lock (this._Mutex)
			{
				List<KFAllyData> list = this.InitAllyAcceptList(unionID);
				if (list != null && list.Count > 0)
				{
					result = list.Find((KFAllyData data) => data.UnionID == targetID);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public bool IsAgent(int serverID)
		{
			bool flag = ClientAgentManager.Instance().ExistAgent(serverID);
			if (!flag)
			{
				LogManager.WriteLog(2, string.Format("UnionAlly时ServerId错误.ServerId:{0}", serverID), null, true);
			}
			return flag;
		}

		public long RankVersion(int serverID)
		{
			return RankService.getInstance().RankVersion(serverID);
		}

		public int RankGradeUpdate(int serverID, KFRankData rankData)
		{
			return RankService.getInstance().RankGradeUpdate(serverID, rankData);
		}

		public List<KFRankData> RankTopList(int serverID, int rankType)
		{
			return RankService.getInstance().RankTopList(serverID, rankType);
		}

		public KFRankData RankRole(int serverID, int rankType, int roleID)
		{
			return RankService.getInstance().RankRole(serverID, rankType, roleID);
		}

		private const double REQUEST_SECOND_CLEAR_SPAN = 30.0;

		private object _Mutex = new object();

		public readonly GameTypes _gameType = 14;

		public readonly GameTypes EvItemGameType = 2;

		private ConcurrentDictionary<int, KFAllyData> _unionDic = new ConcurrentDictionary<int, KFAllyData>();

		private ConcurrentDictionary<int, List<int>> _allyDic = new ConcurrentDictionary<int, List<int>>();

		private ConcurrentDictionary<int, List<KFAllyData>> _requestDic = new ConcurrentDictionary<int, List<KFAllyData>>();

		private ConcurrentDictionary<int, List<KFAllyData>> _acceptDic = new ConcurrentDictionary<int, List<KFAllyData>>();

		public static AllyService Instance = null;

		public AllyPersistence _Persistence = AllyPersistence.Instance;

		public Thread _BackgroundThread;

		private DateTime _clearTimeRequest = DateTime.MinValue;
	}
}
