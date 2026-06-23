using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	public class HongBaoManager_K
	{
		public static HongBaoManager_K getInstance()
		{
			return HongBaoManager_K._Instance;
		}

		private KuaFuCmdData JunTuanBaseDataListCmdData
		{
			get
			{
				return this.Persistence.JunTuanBaseDataListCmdData;
			}
			set
			{
				this.Persistence.JunTuanBaseDataListCmdData = value;
			}
		}

		public void ThreadProc(object state)
		{
			if (this.Initialiazed)
			{
				try
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					long nowTicks = TimeUtil.NOW();
					bool flag = false;
					if (dateTime > this.CheckTime20)
					{
						this.CheckTime20 = dateTime.AddSeconds(20.0);
						flag = true;
					}
					this.SendHongBaoProc(dateTime);
					this.CheckHongBaoState(nowTicks, flag);
					if (flag)
					{
						this.Persistence.UpdateHongBaoHuoDongData(this.HuoDongStartTicks, this.NextSendID, this.LeftCharge, this.TotalCharge);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
			}
		}

		public bool LoadConfig()
		{
			try
			{
				this.Initialiazed = false;
				XElement xelement = ConfigHelper.Load(KuaFuServerManager.GetResourcePath("Config/JieRiGifts/JieRiChongZhiHongBao.xml", KuaFuServerManager.ResourcePathTypes.GameRes));
				if (null == xelement)
				{
					return false;
				}
				lock (this.Mutex)
				{
					this.ConfigDict.Clear();
					XElement xelement2 = xelement.Element("Activities");
					if (null != xelement2)
					{
						this.FromDate = ConfigHelper.GetElementAttributeValue(xelement2, "FromDate", "");
						this.ToDate = ConfigHelper.GetElementAttributeValue(xelement2, "ToDate", "");
						this.ActivityType = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ActivityType", 0L);
						this.AwardStartDate = ConfigHelper.GetElementAttributeValue(xelement2, "AwardStartDate", "");
						this.AwardEndDate = ConfigHelper.GetElementAttributeValue(xelement2, "AwardEndDate", "");
						this.StartTime = DateTime.Parse(this.FromDate);
						this.EndTime = DateTime.Parse(this.ToDate);
						this.ActivityKeyStr = string.Format("{0}_{1}", this.FromDate, this.ToDate).Replace(':', '$');
					}
					else
					{
						this.ActivityKeyStr = "";
						this.StartTime = (this.EndTime = DateTime.MinValue);
					}
					this.HuoDongStartTicks = this.StartTime.Ticks / 10000000L * 1000L;
					xelement2 = xelement.Element("GiftList");
					if (null != xelement2)
					{
						IEnumerable<XElement> enumerable = xelement2.Elements();
						foreach (XElement xelement3 in enumerable)
						{
							if (null != xelement3)
							{
								JieRiChongZhiHongBaoInfo jieRiChongZhiHongBaoInfo = new JieRiChongZhiHongBaoInfo();
								jieRiChongZhiHongBaoInfo.ID = (int)ConfigHelper.GetElementAttributeValueLong(xelement3, "ID", 0L);
								jieRiChongZhiHongBaoInfo.RechargeDifference = (int)ConfigHelper.GetElementAttributeValueLong(xelement3, "RechargeDifference", 0L);
								jieRiChongZhiHongBaoInfo.PlatformID = ConfigHelper.GetElementAttributeValue(xelement3, "PlatformID", "");
								jieRiChongZhiHongBaoInfo.RedPacketSize = (int)ConfigHelper.GetElementAttributeValueLong(xelement3, "RedPacketSize", 0L);
								jieRiChongZhiHongBaoInfo.Interval = ConfigHelper.String2IntArray(ConfigHelper.GetElementAttributeValue(xelement3, "Interval", ""), ',');
								jieRiChongZhiHongBaoInfo.DurationTime = (int)ConfigHelper.GetElementAttributeValueLong(xelement3, "DurationTime", 0L);
								if (string.Compare(jieRiChongZhiHongBaoInfo.PlatformID, KuaFuServerManager.platformType.ToString(), true) == 0)
								{
									this.ConfigDict.Add(jieRiChongZhiHongBaoInfo.ID, jieRiChongZhiHongBaoInfo);
								}
							}
						}
					}
				}
				this.Initialiazed = this.InitData();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/JieRiGifts/JieRiChongZhiHongBao.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool InitData()
		{
			lock (this.Mutex)
			{
				long num = 0L;
				int nextSendID = 0;
				long leftCharge = 0L;
				long totalCharge = 0L;
				if (this.Persistence.GetHongBaoHuoDongData(ref num, ref nextSendID, ref leftCharge, ref totalCharge))
				{
					if (num == this.HuoDongStartTicks)
					{
						this.HuoDongStartTicks = num;
						this.NextSendID = nextSendID;
						this.LeftCharge = leftCharge;
						this.TotalCharge = totalCharge;
						this.Persistence.LoadHongBaoDataList(this.ActivityKeyStr, this.HongBaoDataDict, this.HongBaoRecvDict);
						foreach (KeyValuePair<int, SystemHongBaoData> keyValuePair in this.HongBaoDataDict)
						{
							if (this.NextSendID < keyValuePair.Value.ID)
							{
								this.NextSendID = keyValuePair.Value.ID;
							}
						}
					}
					else
					{
						this.NextSendID = 0;
						this.LeftCharge = 0L;
						this.TotalCharge = 0L;
						this.HongBaoDataDict.Clear();
						this.HongBaoRecvDict.Clear();
					}
				}
			}
			return true;
		}

		public List<SystemHongBaoData> SendHongBaoProc(DateTime now)
		{
			List<SystemHongBaoData> result = new List<SystemHongBaoData>();
			lock (this.Mutex)
			{
				JieRiChongZhiHongBaoInfo jieRiChongZhiHongBaoInfo = null;
				JieRiChongZhiHongBaoInfo jieRiChongZhiHongBaoInfo2 = null;
				for (int i = 0; i < this.ConfigDict.Values.Count; i++)
				{
					jieRiChongZhiHongBaoInfo = this.ConfigDict.Values[i];
					if (i < this.ConfigDict.Values.Count - 1)
					{
						jieRiChongZhiHongBaoInfo2 = this.ConfigDict.Values[i + 1];
					}
					if (jieRiChongZhiHongBaoInfo.ID >= this.NextSendID)
					{
						break;
					}
				}
				if (this.LeftCharge >= (long)jieRiChongZhiHongBaoInfo.RechargeDifference)
				{
					this.NextSendID = jieRiChongZhiHongBaoInfo2.ID;
					this.LeftCharge -= (long)jieRiChongZhiHongBaoInfo.RechargeDifference;
					SystemHongBaoData systemHongBaoData = new SystemHongBaoData();
					systemHongBaoData.ID = jieRiChongZhiHongBaoInfo.ID;
					systemHongBaoData.LeftZuanShi = jieRiChongZhiHongBaoInfo.RedPacketSize;
					systemHongBaoData.StartTime = TimeUtil.NOW() + 15000L;
					systemHongBaoData.DurationTime = jieRiChongZhiHongBaoInfo.DurationTime * 1000;
					DateTime startTime = new DateTime(systemHongBaoData.StartTime * 10000L);
					DateTime endTime = new DateTime((systemHongBaoData.StartTime + (long)jieRiChongZhiHongBaoInfo.DurationTime * 1000L) * 10000L);
					int num = (int)this.Persistence.CreateHongBao(this.ActivityKeyStr, systemHongBaoData.ID, startTime, endTime, systemHongBaoData.LeftZuanShi, 0);
					if (num > 0)
					{
						systemHongBaoData.HongBaoId = num;
						this.HongBaoDataDict[num] = systemHongBaoData;
					}
				}
			}
			return result;
		}

		public void CheckHongBaoState(long nowTicks, bool writedb)
		{
			lock (this.Mutex)
			{
				List<SystemHongBaoData> list = new List<SystemHongBaoData>();
				foreach (SystemHongBaoData systemHongBaoData in this.HongBaoDataDict.Values)
				{
					if (systemHongBaoData.LeftZuanShi <= 0)
					{
						systemHongBaoData.State = 3;
						list.Add(systemHongBaoData);
					}
					else if (nowTicks > systemHongBaoData.StartTime + (long)systemHongBaoData.DurationTime)
					{
						systemHongBaoData.State = 2;
						list.Add(systemHongBaoData);
					}
					else if (writedb)
					{
						this.Persistence.UpdateHongBao(systemHongBaoData.HongBaoId, systemHongBaoData.LeftZuanShi, systemHongBaoData.State);
					}
				}
				foreach (SystemHongBaoData systemHongBaoData in list)
				{
					this.HongBaoDataDict.Remove(systemHongBaoData.HongBaoId);
					this.Persistence.UpdateHongBao(systemHongBaoData.HongBaoId, systemHongBaoData.LeftZuanShi, systemHongBaoData.State);
				}
			}
		}

		public AsyncDataItem GetHongBaoDataList(long dataAge)
		{
			try
			{
				if (this.TotalCharge > 0L)
				{
					DateTime t = TimeUtil.NowDateTime();
					long age = TimeUtil.NOW();
					if (t >= this.StartTime && t < this.EndTime)
					{
						lock (this.Mutex)
						{
							byte[] bytes = DataHelper2.ObjectToBytes<Dictionary<int, SystemHongBaoData>>(this.HongBaoDataDict);
							KuaFuCmdData kuaFuCmdData = new KuaFuCmdData
							{
								Age = age,
								Bytes0 = bytes
							};
							return new AsyncDataItem(30, new object[]
							{
								kuaFuCmdData
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
			return null;
		}

		public int OpenHongBao(int hongBaoId, int rid, int zoneid, string userid, string rname)
		{
			try
			{
				lock (this.Mutex)
				{
					SystemHongBaoData systemHongBaoData;
					if (!this.HongBaoDataDict.TryGetValue(hongBaoId, out systemHongBaoData))
					{
						return -20;
					}
					if (systemHongBaoData.LeftZuanShi <= 0)
					{
						return -42;
					}
					long key = ((long)hongBaoId << 36) + (long)rid;
					if (this.HongBaoRecvDict.ContainsKey(key))
					{
						return -200;
					}
					JieRiChongZhiHongBaoInfo jieRiChongZhiHongBaoInfo;
					if (!this.ConfigDict.TryGetValue(systemHongBaoData.ID, out jieRiChongZhiHongBaoInfo))
					{
						return -42;
					}
					int maxV = Math.Min(systemHongBaoData.LeftZuanShi, jieRiChongZhiHongBaoInfo.Interval[1]);
					int minV = Math.Min(systemHongBaoData.LeftZuanShi, jieRiChongZhiHongBaoInfo.Interval[0]);
					int randomNumber = Global.GetRandomNumber(minV, maxV);
					systemHongBaoData.LeftZuanShi -= randomNumber;
					this.HongBaoRecvDict.Add(key, randomNumber);
					this.Persistence.WriteHongBaoRecv(this.ActivityKeyStr, systemHongBaoData.HongBaoId, rid, zoneid, userid, rname, randomNumber);
					return randomNumber;
				}
			}
			catch (Exception ex)
			{
			}
			return -11000;
		}

		public void AddServerTotalCharge(string keyStr, long addCharge)
		{
			try
			{
				if (!(keyStr != this.ActivityKeyStr))
				{
					DateTime t = TimeUtil.NowDateTime();
					if (!(t < this.StartTime) && !(t > this.EndTime))
					{
						lock (this.Mutex)
						{
							this.LeftCharge += addCharge;
							this.TotalCharge += addCharge;
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		private const double SaveServerStateProcInterval = 30.0;

		private const long DelaySendTicks = 15000L;

		private const string CfgFile = "Config/JieRiGifts/JieRiChongZhiHongBao.xml";

		private static HongBaoManager_K _Instance = new HongBaoManager_K();

		private object Mutex = new object();

		public string FromDate = "";

		public string ToDate = "";

		public string AwardStartDate = "";

		public string AwardEndDate = "";

		public int ActivityType = -1;

		protected int CodeForParamsValidate = 0;

		public string ActivityKeyStr;

		public DateTime StartTime;

		public DateTime EndTime;

		public readonly GameTypes GameType = 21;

		private DateTime CheckTime20;

		private bool Initialiazed = false;

		private DateTime SaveServerStateProcTime;

		private int LastUpdateRankHour = -1;

		public JunTuanPersistence Persistence = JunTuanPersistence.Instance;

		private long HuoDongStartTicks;

		private int NextSendID;

		private long LeftCharge;

		private long TotalCharge;

		private SortedList<int, JieRiChongZhiHongBaoInfo> ConfigDict = new SortedList<int, JieRiChongZhiHongBaoInfo>();

		private Dictionary<int, SystemHongBaoData> HongBaoDataDict = new Dictionary<int, SystemHongBaoData>();

		private Dictionary<long, int> HongBaoRecvDict = new Dictionary<long, int>();
	}
}
