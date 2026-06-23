using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using System.Threading;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using KF.Remoting.Data;
using KF.Remoting.IPStatistics;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	public class IPStatisticsService : MarshalByRefObject, IIPStatisticsService
	{
		public static IPStatisticsService getInstance()
		{
			return IPStatisticsService._Instance;
		}

		public override object InitializeLifetimeService()
		{
			IPStatisticsService._Instance = this;
			ILease lease = (ILease)base.InitializeLifetimeService();
			if (lease.CurrentState == LeaseState.Initial)
			{
				lease.InitialLeaseTime = TimeSpan.FromDays(2000.0);
			}
			return lease;
		}

		private IPStatisticsService()
		{
			IPStatisticsService._Instance = this;
			this.BackgroundThread = new Thread(new ParameterizedThreadStart(this.ThreadProc));
			this.BackgroundThread.IsBackground = true;
			this.BackgroundThread.Start();
		}

		~IPStatisticsService()
		{
			this.BackgroundThread.Abort();
		}

		public void ThreadProc(object state)
		{
			for (;;)
			{
				Thread.Sleep(1000);
				int offsetMiniteNow = Global.GetOffsetMiniteNow();
				if (offsetMiniteNow != IPStatisticsService.lastMinite)
				{
					try
					{
						this.IPStatisticsProc();
					}
					catch (Exception ex)
					{
						LogManager.WriteExceptionUseCache(ex.ToString());
					}
					LogManager.WriteLog(13, string.Format("minite change  {0} {1}", IPStatisticsService.lastMinite, offsetMiniteNow), null, true);
					IPStatisticsService.lastMinite = offsetMiniteNow;
				}
			}
		}

		private void IPStatisticsProc()
		{
			Dictionary<int, List<IPStatisticsData>> dictionary = new Dictionary<int, List<IPStatisticsData>>();
			lock (IPStatisticsService.dictCurrData)
			{
				foreach (KeyValuePair<int, List<IPStatisticsData>> keyValuePair in IPStatisticsService.dictCurrData)
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
				IPStatisticsService.dictCurrData.Clear();
			}
			Dictionary<long, HashSet<int>> dictionary2 = new Dictionary<long, HashSet<int>>();
			Dictionary<long, IPStatisticsData> dictionary3 = new Dictionary<long, IPStatisticsData>();
			Dictionary<int, List<IPOperaData>> dictionary4 = new Dictionary<int, List<IPOperaData>>();
			foreach (KeyValuePair<int, List<IPStatisticsData>> keyValuePair2 in dictionary)
			{
				foreach (IPStatisticsData ipstatisticsData in keyValuePair2.Value)
				{
					try
					{
						IPOperaData ipoperaData = this.checkIP(ipstatisticsData, true);
						if (null != ipoperaData)
						{
							List<IPOperaData> list;
							if (!dictionary4.TryGetValue(keyValuePair2.Key, out list))
							{
								list = new List<IPOperaData>();
								dictionary4[keyValuePair2.Key] = list;
							}
							list.Add(ipoperaData);
						}
						if (dictionary2.ContainsKey(ipstatisticsData.ipAsInt))
						{
							dictionary2[ipstatisticsData.ipAsInt].Add(keyValuePair2.Key);
						}
						else
						{
							HashSet<int> hashSet = new HashSet<int>();
							hashSet.Add(keyValuePair2.Key);
							dictionary2.Add(ipstatisticsData.ipAsInt, hashSet);
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(2, "IPStatisticsProc", ex, false);
					}
					IPStatisticsData ipstatisticsData2 = null;
					if (dictionary3.TryGetValue(ipstatisticsData.ipAsInt, out ipstatisticsData2))
					{
						ipstatisticsData2 += ipstatisticsData;
					}
					else
					{
						dictionary3.Add(ipstatisticsData.ipAsInt, ipstatisticsData);
					}
				}
			}
			List<IPOperaData> list2 = new List<IPOperaData>();
			foreach (KeyValuePair<long, IPStatisticsData> keyValuePair3 in dictionary3)
			{
				IPOperaData ipoperaData = this.checkIP(keyValuePair3.Value, false);
				if (null != ipoperaData)
				{
					list2.Add(ipoperaData);
				}
			}
			this.AddOpList(dictionary4, list2, dictionary2);
			lock (IPStatisticsService.dictIPOperaData)
			{
				IPStatisticsService.dictIPOperaData = dictionary4;
			}
		}

		private void AddOpList(Dictionary<int, List<IPOperaData>> IPOperaDataDict, List<IPOperaData> resultList, Dictionary<long, HashSet<int>> ip2serveridDict)
		{
			foreach (IPOperaData ipoperaData in resultList)
			{
				HashSet<int> hashSet = null;
				if (ip2serveridDict.TryGetValue(ipoperaData.ipAsInt, out hashSet))
				{
					foreach (int key in hashSet)
					{
						List<IPOperaData> list = null;
						if (!IPOperaDataDict.TryGetValue(key, out list))
						{
							list = new List<IPOperaData>();
							IPOperaDataDict.Add(key, list);
						}
						list.Add(ipoperaData);
					}
				}
			}
		}

		private IPOperaData checkIP(IPStatisticsData ipData, bool local)
		{
			IPOperaData ipoperaData = null;
			foreach (StatisticsControl statisticsControl in IPStatisticsPersistence.Instance._IPControlList)
			{
				if (statisticsControl.Local == local && !this.checkIP(ipData, statisticsControl))
				{
					if (!IPStatisticsPersistence.Instance.isCanPassIP(ipData.ipAsInt))
					{
						if (null == ipoperaData)
						{
							ipoperaData = new IPOperaData();
						}
						ipoperaData.ipAsInt = ipData.ipAsInt;
						if (statisticsControl.OperaType >= 0)
						{
							if (statisticsControl.OperaParam > ipoperaData.OperaTime[statisticsControl.OperaType])
							{
								ipoperaData.OperaTime[statisticsControl.OperaType] = statisticsControl.OperaParam;
							}
							if (ipData.IPInfoParams[statisticsControl.ParamType] > ipoperaData.OperaParam[statisticsControl.OperaType])
							{
								ipoperaData.OperaParam[statisticsControl.OperaType] = ipData.IPInfoParams[statisticsControl.ParamType];
							}
						}
						string text = string.Format("cant pass ip={0}:{1} ruleid={2} paramValue={3}", new object[]
						{
							ipoperaData.ipAsInt,
							IpHelper.IntToIp(ipoperaData.ipAsInt),
							statisticsControl.ID,
							ipData.IPInfoParams[statisticsControl.ParamType]
						});
						if (statisticsControl.ComParamType >= 0)
						{
							text += string.Format(" comParamValue={0}", ipData.IPInfoParams[statisticsControl.ComParamType]);
						}
						LogManager.WriteLog(13, text, null, true);
					}
				}
			}
			return ipoperaData;
		}

		private bool checkIP(IPStatisticsData IPData, StatisticsControl config)
		{
			bool flag = true;
			if (config.ParamLimit > 0)
			{
				if (IPData.IPInfoParams[config.ParamType] >= config.ParamLimit)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				if (config.ComParamType >= 0)
				{
					double num = double.MaxValue;
					if (IPData.IPInfoParams[config.ComParamType] > 0)
					{
						num = (double)IPData.IPInfoParams[config.ParamType] * 1.0 / (double)IPData.IPInfoParams[config.ComParamType];
					}
					flag = ((config.ComParamLimit > 0.0) ? (num > config.ComParamLimit) : (num < -config.ComParamLimit));
				}
			}
			return flag;
		}

		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			int result;
			try
			{
				if (clientInfo.ServerId != 0)
				{
					bool flag = false;
					int num = ClientAgentManager.Instance().InitializeClient(clientInfo, out flag);
					if (num > 0)
					{
					}
					result = num;
				}
				else
				{
					LogManager.WriteLog(2, string.Format("InitializeClient时GameType错误,禁止连接.ServerId:{0},GameType:{1}", clientInfo.ServerId, clientInfo.GameType), null, true);
					result = -4003;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(string.Format("InitializeClient服务器ID重复,禁止连接.ServerId:{0},ClientId:{1},info:{2}", clientInfo.ServerId, clientInfo.ClientId, clientInfo.Token));
				result = -11003;
			}
			return result;
		}

		public int RequestMinite()
		{
			return IPStatisticsService.lastMinite;
		}

		public int IPStatisticsDataReport(int serverId, int lastMinite, List<IPStatisticsData> list)
		{
			int result;
			if (!ClientAgentManager.Instance().ExistAgent(serverId))
			{
				LogManager.WriteLog(2, string.Format("IPStatisticsDataReport时ServerId错误.ServerId:{0},roleId:{1}", serverId), null, true);
				result = -500;
			}
			else
			{
				lock (IPStatisticsService.dictCurrData)
				{
					IPStatisticsService.dictCurrData.Add(serverId, list);
				}
				LogManager.WriteLog(13, string.Format("recv ip report serverid={0} minite={1} count={2}", serverId, lastMinite, list.Count), null, true);
				result = 0;
			}
			return result;
		}

		public List<IPOperaData> GetIPStatisticsResult(int serverId)
		{
			List<IPOperaData> result;
			if (!ClientAgentManager.Instance().ExistAgent(serverId))
			{
				LogManager.WriteLog(2, string.Format("GetIPStatisticsResult时ServerId错误.ServerId:{0},roleId:{1}", serverId), null, true);
				result = null;
			}
			else
			{
				List<IPOperaData> list = null;
				lock (IPStatisticsService.dictIPOperaData)
				{
					if (IPStatisticsService.dictIPOperaData.TryGetValue(serverId, out list))
					{
						return list;
					}
				}
				result = list;
			}
			return result;
		}

		private static IPStatisticsService _Instance = null;

		public readonly GameTypes IPGameType = 18;

		private static int lastMinite = Global.GetOffsetMiniteNow();

		public Thread BackgroundThread;

		private static Dictionary<int, List<IPStatisticsData>> dictCurrData = new Dictionary<int, List<IPStatisticsData>>();

		private static Dictionary<int, List<IPOperaData>> dictIPOperaData = new Dictionary<int, List<IPOperaData>>();
	}
}
