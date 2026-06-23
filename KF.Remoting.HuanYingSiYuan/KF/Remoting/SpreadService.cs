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
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
	public class SpreadService : MarshalByRefObject, ISpreadService
	{
		public override object InitializeLifetimeService()
		{
			SpreadService.Instance = this;
			ILease lease = (ILease)base.InitializeLifetimeService();
			if (lease.CurrentState == LeaseState.Initial)
			{
				lease.InitialLeaseTime = TimeSpan.FromDays(2000.0);
			}
			return lease;
		}

		public SpreadService()
		{
			SpreadService.Instance = this;
			this._BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this._BackgroundThread.IsBackground = true;
			this._BackgroundThread.Start();
		}

		~SpreadService()
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
					if (dateTime > this._clearTimeSpread)
					{
						this._clearTimeSpread = dateTime.AddSeconds(86400.0);
						this.ClearSpreadData();
					}
					if (dateTime > this._clearTimeVerify)
					{
						this._clearTimeVerify = dateTime.AddSeconds(3600.0);
						this.ClearVerifyData();
						this.ClearTelData();
						this.ClearRoleData();
					}
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

		private void ClearSpreadData()
		{
			if (this._spreadDataDic != null && this._spreadDataDic.Count > 0)
			{
				List<KFSpreadKey> list = (from info in this._spreadDataDic.Values
				where info.LogTime <= TimeUtil.NowDateTime().AddSeconds(-86400.0)
				select KFSpreadKey.Get(info.ZoneID, info.RoleID)).ToList<KFSpreadKey>();
				foreach (KFSpreadKey key in list)
				{
					KFSpreadData kfspreadData;
					this._spreadDataDic.TryRemove(key, out kfspreadData);
				}
			}
		}

		private void ClearVerifyData()
		{
			if (this._spreadVerifyDataDic != null && this._spreadVerifyDataDic.Count > 0)
			{
				List<KFSpreadKey> list = (from info in this._spreadVerifyDataDic.Values
				where info.LogTime <= TimeUtil.NowDateTime().AddHours(-3600.0)
				select KFSpreadKey.Get(info.CZoneID, info.CRoleID)).ToList<KFSpreadKey>();
				foreach (KFSpreadKey key in list)
				{
					KFSpreadVerifyData kfspreadVerifyData;
					this._spreadVerifyDataDic.TryRemove(key, out kfspreadVerifyData);
				}
			}
		}

		private void ClearTelData()
		{
			if (this._telTotalDic != null && this._telTotalDic.Count > 0)
			{
				List<string> list = (from info in this._telTotalDic.Values
				where info.LogTime <= TimeUtil.NowDateTime().AddHours(-3600.0) && !info.IsStop
				select info.Tel).ToList<string>();
				foreach (string key in list)
				{
					KFSpreadTelTotal kfspreadTelTotal;
					this._telTotalDic.TryRemove(key, out kfspreadTelTotal);
				}
			}
		}

		private void ClearRoleData()
		{
			if (this._roleTotalDic != null && this._roleTotalDic.Count > 0)
			{
				List<KFSpreadKey> list = (from info in this._roleTotalDic.Values
				where info.LogTime <= TimeUtil.NowDateTime().AddHours(-3600.0) && !info.IsStop
				select KFSpreadKey.Get(info.CZoneID, info.CRoleID)).ToList<KFSpreadKey>();
				foreach (KFSpreadKey key in list)
				{
					KFSpreadRoleTotal kfspreadRoleTotal;
					this._roleTotalDic.TryRemove(key, out kfspreadRoleTotal);
				}
			}
		}

		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.GameType == 9 && clientInfo.ServerId != 0)
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

		public int SpreadSign(int serverID, int zoneID, int roleID)
		{
			int result;
			if (!this.IsAgent(serverID))
			{
				result = -5;
			}
			else
			{
				KFSpreadKey key = KFSpreadKey.Get(zoneID, roleID);
				KFSpreadData kfspreadData;
				if (this._spreadDataDic.TryGetValue(key, out kfspreadData))
				{
					kfspreadData.UpdateLogtime();
					result = -21;
				}
				else if (!SpreadPersistence.Instance.DBSpreadSign(zoneID, roleID))
				{
					result = -1;
				}
				else
				{
					kfspreadData = new KFSpreadData
					{
						ServerID = serverID,
						ZoneID = zoneID,
						RoleID = roleID
					};
					this._spreadDataDic.TryAdd(key, kfspreadData);
					result = 1;
				}
			}
			return result;
		}

		public int[] SpreadCount(int serverID, int zoneID, int roleID)
		{
			int[] array = new int[3];
			int[] array2 = array;
			int[] result;
			if (!this.IsAgent(serverID))
			{
				result = array2;
			}
			else
			{
				KFSpreadKey key = KFSpreadKey.Get(zoneID, roleID);
				KFSpreadData kfspreadData;
				if (this._spreadDataDic.TryGetValue(key, out kfspreadData))
				{
					array2[0] = kfspreadData.CountRole;
					array2[1] = kfspreadData.CountVip;
					array2[2] = kfspreadData.CountLevel;
					kfspreadData.UpdateLogtime();
					result = array2;
				}
				else
				{
					array2[0] = SpreadPersistence.Instance.DBSpreadCountAll(zoneID, roleID);
					array2[1] = SpreadPersistence.Instance.DBSpreadCountVip(zoneID, roleID);
					array2[2] = SpreadPersistence.Instance.DBSpreadCountLevel(zoneID, roleID);
					kfspreadData = new KFSpreadData
					{
						ServerID = serverID,
						ZoneID = zoneID,
						RoleID = roleID,
						CountRole = array2[0],
						CountVip = array2[1],
						CountLevel = array2[2]
					};
					this._spreadDataDic.TryAdd(key, kfspreadData);
					result = array2;
				}
			}
			return result;
		}

		public int CheckVerifyCode(int cserverID, string cuserID, int czoneID, int croleID, int pzoneID, int proleID, int isVip, int isLevel)
		{
			int result;
			if (!this.IsAgent(cserverID))
			{
				result = -5;
			}
			else
			{
				KFSpreadData spreadData = this.GetSpreadData(pzoneID, proleID);
				if (spreadData == null)
				{
					result = -14;
				}
				else
				{
					bool flag = SpreadPersistence.Instance.DBSpreadVeruftCheck(czoneID, croleID, cuserID);
					if (flag)
					{
						result = -12;
					}
					else
					{
						KFSpreadRoleTotal roleTotalData = this.GetRoleTotalData(cserverID, czoneID, croleID, false);
						if (roleTotalData.IsStop)
						{
							result = -16;
						}
						else
						{
							KFSpreadKey key = KFSpreadKey.Get(czoneID, croleID);
							KFSpreadVerifyData value = null;
							this._spreadVerifyDataDic.TryRemove(key, out value);
							value = new KFSpreadVerifyData
							{
								CUserID = cuserID,
								CServerID = cserverID,
								CZoneID = czoneID,
								CRoleID = croleID,
								PZoneID = pzoneID,
								PRoleID = proleID,
								IsVip = isVip,
								IsLevel = isLevel
							};
							this._spreadVerifyDataDic.TryAdd(key, value);
							result = 1;
						}
					}
				}
			}
			return result;
		}

		public int TelCodeGet(int cserverID, int czoneID, int croleID, string tel)
		{
			int result;
			if (!this.IsAgent(cserverID))
			{
				result = -5;
			}
			else
			{
				KFSpreadKey key = KFSpreadKey.Get(czoneID, croleID);
				KFSpreadVerifyData kfspreadVerifyData = null;
				if (!this._spreadVerifyDataDic.TryGetValue(key, out kfspreadVerifyData))
				{
					result = -10;
				}
				else
				{
					bool flag = SpreadPersistence.Instance.DBSpreadTelBind(tel);
					if (flag)
					{
						result = -32;
					}
					else
					{
						KFSpreadTelTotal telTotalData = this.GetTelTotalData(tel, true);
						if (telTotalData.IsStop)
						{
							result = -36;
						}
						else
						{
							KFSpreadRoleTotal roleTotalData = this.GetRoleTotalData(cserverID, czoneID, croleID, true);
							if (roleTotalData.IsStop)
							{
								result = -16;
							}
							else
							{
								kfspreadVerifyData.Tel = tel;
								kfspreadVerifyData.TelCode = this.GetTelCodeRandom();
								kfspreadVerifyData.TelTime = TimeUtil.NowDateTime();
								if (!SpreadPersistence.Instance.DBSpreadTelCodeAdd(kfspreadVerifyData.PZoneID, kfspreadVerifyData.PRoleID, czoneID, croleID, tel, kfspreadVerifyData.TelCode))
								{
									result = -33;
								}
								else
								{
									result = 1;
								}
							}
						}
					}
				}
			}
			return result;
		}

		public int TelCodeVerify(int serverID, int czoneID, int croleID, int telCode)
		{
			int result;
			if (!this.IsAgent(serverID))
			{
				result = -5;
			}
			else
			{
				KFSpreadKey key = KFSpreadKey.Get(czoneID, croleID);
				KFSpreadVerifyData kfspreadVerifyData = null;
				if (!this._spreadVerifyDataDic.TryGetValue(key, out kfspreadVerifyData))
				{
					result = -10;
				}
				else
				{
					KFSpreadData spreadData = this.GetSpreadData(kfspreadVerifyData.PZoneID, kfspreadVerifyData.PRoleID);
					if (spreadData == null)
					{
						result = -14;
					}
					else
					{
						spreadData.UpdateLogtime();
						if (kfspreadVerifyData.TelCode != telCode)
						{
							result = -34;
						}
						else if (TimeUtil.NowDateTime().AddSeconds(-90.0) > kfspreadVerifyData.TelTime)
						{
							result = -35;
						}
						else if (!SpreadPersistence.Instance.DBSpreadRoleAdd(kfspreadVerifyData.PZoneID, kfspreadVerifyData.PRoleID, kfspreadVerifyData.CUserID, kfspreadVerifyData.CZoneID, kfspreadVerifyData.CRoleID, kfspreadVerifyData.Tel, kfspreadVerifyData.IsVip, kfspreadVerifyData.IsLevel))
						{
							result = -1;
						}
						else
						{
							lock (spreadData)
							{
								spreadData.CountLevel += kfspreadVerifyData.IsLevel;
								spreadData.CountVip += kfspreadVerifyData.IsVip;
								spreadData.CountRole++;
								if (spreadData.ServerID > 0)
								{
									this.NotifySpreadData(spreadData);
								}
							}
							this._spreadVerifyDataDic.TryRemove(key, out kfspreadVerifyData);
							result = 1;
						}
					}
				}
			}
			return result;
		}

		public bool SpreadLevel(int pzoneID, int proleID, int czoneID, int croleID)
		{
			KFSpreadData spreadData = this.GetSpreadData(pzoneID, proleID);
			bool result;
			if (spreadData == null)
			{
				result = false;
			}
			else
			{
				spreadData.UpdateLogtime();
				lock (spreadData)
				{
					bool flag2 = SpreadPersistence.Instance.DBSpreadIsLevel(pzoneID, proleID, czoneID, croleID);
					if (flag2)
					{
						spreadData.CountLevel++;
						if (spreadData.ServerID > 0)
						{
							this.NotifySpreadData(spreadData);
						}
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public bool SpreadVip(int pzoneID, int proleID, int czoneID, int croleID)
		{
			KFSpreadData spreadData = this.GetSpreadData(pzoneID, proleID);
			bool result;
			if (spreadData == null)
			{
				result = false;
			}
			else
			{
				spreadData.UpdateLogtime();
				lock (spreadData)
				{
					bool flag2 = SpreadPersistence.Instance.DBSpreadIsVip(pzoneID, proleID, czoneID, croleID);
					if (flag2)
					{
						spreadData.CountVip++;
						if (spreadData.ServerID > 0)
						{
							this.NotifySpreadData(spreadData);
						}
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		private void NotifySpreadData(KFSpreadData data)
		{
			ClientAgentManager.Instance().PostAsyncEvent(data.ServerID, this.GameType, new AsyncDataItem(10008, new object[]
			{
				data.ZoneID,
				data.RoleID,
				data.CountRole,
				data.CountVip,
				data.CountLevel
			}));
		}

		private int GetTelCodeRandom()
		{
			int result;
			if (Consts.IsTest > 0)
			{
				result = 123456;
			}
			else
			{
				result = RandomHelper.GetRandomNumber(100000, 999999);
			}
			return result;
		}

		private KFSpreadData GetSpreadData(int pzoneID, int proleID)
		{
			KFSpreadKey key = KFSpreadKey.Get(pzoneID, proleID);
			KFSpreadData kfspreadData = null;
			if (!this._spreadDataDic.TryGetValue(key, out kfspreadData))
			{
				if (!SpreadPersistence.Instance.DBSpreadSignCheck(pzoneID, proleID))
				{
					return null;
				}
				kfspreadData = new KFSpreadData
				{
					ServerID = 0,
					ZoneID = pzoneID,
					RoleID = proleID,
					CountRole = SpreadPersistence.Instance.DBSpreadCountAll(pzoneID, proleID),
					CountVip = SpreadPersistence.Instance.DBSpreadCountVip(pzoneID, proleID),
					CountLevel = SpreadPersistence.Instance.DBSpreadCountLevel(pzoneID, proleID)
				};
				this._spreadDataDic.TryAdd(key, kfspreadData);
			}
			if (kfspreadData != null)
			{
				kfspreadData.UpdateLogtime();
			}
			return kfspreadData;
		}

		private KFSpreadRoleTotal GetRoleTotalData(int cserverID, int czoneId, int croleID, bool isAddCount = false)
		{
			KFSpreadKey key = KFSpreadKey.Get(czoneId, croleID);
			KFSpreadRoleTotal kfspreadRoleTotal = null;
			if (!this._roleTotalDic.TryGetValue(key, out kfspreadRoleTotal))
			{
				kfspreadRoleTotal = new KFSpreadRoleTotal
				{
					CServerID = cserverID,
					CZoneID = czoneId,
					CRoleID = croleID
				};
				this._roleTotalDic.TryAdd(key, kfspreadRoleTotal);
			}
			int num = this.TimeSpanSecond(kfspreadRoleTotal.LogTime, TimeUtil.NowDateTime());
			KFSpreadRoleTotal result;
			if (kfspreadRoleTotal.IsStop)
			{
				if (num > Consts.VerifyRoleTimeStop)
				{
					kfspreadRoleTotal.LogTime = TimeUtil.NowDateTime();
					kfspreadRoleTotal.Count = 0;
					kfspreadRoleTotal.IsStop = false;
				}
				result = kfspreadRoleTotal;
			}
			else
			{
				if (num > Consts.VerifyRoleTimeLimit)
				{
					kfspreadRoleTotal.LogTime = TimeUtil.NowDateTime();
					kfspreadRoleTotal.Count = 0;
				}
				if (isAddCount)
				{
					kfspreadRoleTotal.AddCount();
				}
				if (kfspreadRoleTotal.Count > Consts.VerifyRoleMaxCount)
				{
					kfspreadRoleTotal.IsStop = true;
				}
				result = kfspreadRoleTotal;
			}
			return result;
		}

		private KFSpreadTelTotal GetTelTotalData(string tel, bool isAddCount = false)
		{
			KFSpreadTelTotal kfspreadTelTotal = null;
			if (!this._telTotalDic.TryGetValue(tel, out kfspreadTelTotal))
			{
				kfspreadTelTotal = new KFSpreadTelTotal
				{
					Tel = tel
				};
				this._telTotalDic.TryAdd(tel, kfspreadTelTotal);
			}
			int num = this.TimeSpanSecond(kfspreadTelTotal.LogTime, TimeUtil.NowDateTime());
			KFSpreadTelTotal result;
			if (kfspreadTelTotal.IsStop)
			{
				if (num > Consts.TelTimeStop)
				{
					kfspreadTelTotal.LogTime = TimeUtil.NowDateTime();
					kfspreadTelTotal.Count = 0;
					kfspreadTelTotal.IsStop = false;
				}
				result = kfspreadTelTotal;
			}
			else
			{
				if (num > Consts.TelTimeLimit)
				{
					kfspreadTelTotal.LogTime = TimeUtil.NowDateTime();
					kfspreadTelTotal.Count = 0;
				}
				if (isAddCount)
				{
					kfspreadTelTotal.AddCount();
				}
				if (kfspreadTelTotal.Count > Consts.TelMaxCount)
				{
					kfspreadTelTotal.IsStop = true;
				}
				result = kfspreadTelTotal;
			}
			return result;
		}

		private int TimeSpanSecond(DateTime begin, DateTime end)
		{
			TimeSpan ts = new TimeSpan(begin.Ticks);
			TimeSpan timeSpan = new TimeSpan(end.Ticks);
			return timeSpan.Subtract(ts).Duration().Seconds;
		}

		public bool IsAgent(int serverID)
		{
			bool flag = ClientAgentManager.Instance().ExistAgent(serverID);
			if (!flag)
			{
				LogManager.WriteLog(2, string.Format("SpreadSign时ServerId错误.ServerId:{0}", serverID), null, true);
			}
			return flag;
		}

		public AsyncDataItem[] GetClientCacheItems(int serverID)
		{
			return ClientAgentManager.Instance().PickAsyncEvent(serverID, this.GameType);
		}

		private const double CLEAR_INTERVAL_SPREAD = 86400.0;

		private const double CLEAR_INTERVAL_VERIFY = 3600.0;

		private const int TEL_CODE_OUT_TIME = 90;

		public static SpreadService Instance = null;

		public SpreadPersistence _Persistence = SpreadPersistence.Instance;

		private object _Mutex = new object();

		public readonly GameTypes GameType = 9;

		private DateTime _clearTimeSpread = DateTime.MinValue;

		private DateTime _clearTimeVerify = DateTime.MinValue;

		private ConcurrentDictionary<KFSpreadKey, KFSpreadData> _spreadDataDic = new ConcurrentDictionary<KFSpreadKey, KFSpreadData>();

		private ConcurrentDictionary<KFSpreadKey, KFSpreadVerifyData> _spreadVerifyDataDic = new ConcurrentDictionary<KFSpreadKey, KFSpreadVerifyData>();

		private ConcurrentDictionary<string, KFSpreadTelTotal> _telTotalDic = new ConcurrentDictionary<string, KFSpreadTelTotal>();

		private ConcurrentDictionary<KFSpreadKey, KFSpreadRoleTotal> _roleTotalDic = new ConcurrentDictionary<KFSpreadKey, KFSpreadRoleTotal>();

		public Thread _BackgroundThread;
	}
}
