using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using KF.Client;
using KF.Contract.Data;
using Server.TCP;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic.KuaFuIPStatistics
{
	public class IPStatisticsManager : IManager, IEventListener
	{
		public static IPStatisticsManager getInstance()
		{
			return IPStatisticsManager.instance;
		}

		public bool initialize()
		{
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.PlayerCreateRoleBeBan, 3);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.LoginFailByDataCheck, 4);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.LoginFailByUserBan, 5);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.LoginFailByTimeout, 6);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.LoginSuccess, 7);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.PlayerInitGameAsync, 8);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.PlayerInitGameBeBan, 9);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.PlayerCreateRoleLimit, 10);
			IPStatisticsManager.event2IPTypeDict.Add(EventTypes.SocketConnect, 11);
			IPStatisticsManager.event2UserTypeDict.Add(EventTypes.LoginFailByDataCheck, UserParamType.Begin);
			IPStatisticsManager.event2UserTypeDict.Add(EventTypes.LoginFailByUserBan, UserParamType.LoginFailByBanCnt);
			IPStatisticsManager.event2UserTypeDict.Add(EventTypes.LoginFailByTimeout, UserParamType.LoginFailByLoginTimeOutCnt);
			IPStatisticsManager.event2UserTypeDict.Add(EventTypes.PlayerInitGameBeBan, UserParamType.InitGameFailByBanCnt);
			IPStatisticsManager.event2UserTypeDict.Add(EventTypes.PlayerCreateRoleLimit, UserParamType.createRoleFailByCnt);
			IPStatisticsManager.event2UserTypeDict.Add(EventTypes.PlayerCreateRoleBeBan, UserParamType.createRoleFailByBan);
			GlobalEventSource.getInstance().registerListener(43, IPStatisticsManager.getInstance());
			GlobalEventSource.getInstance().registerListener(46, IPStatisticsManager.getInstance());
			GlobalEventSource.getInstance().registerListener(47, IPStatisticsManager.getInstance());
			GlobalEventSource.getInstance().registerListener(48, IPStatisticsManager.getInstance());
			GlobalEventSource.getInstance().registerListener(45, IPStatisticsManager.getInstance());
			GlobalEventSource.getInstance().registerListener(15, IPStatisticsManager.getInstance());
			GlobalEventSource.getInstance().registerListener(42, IPStatisticsManager.getInstance());
			GlobalEventSource.getInstance().registerListener(44, IPStatisticsManager.getInstance());
			this.LoadConfig();
			return true;
		}

		public void LoadConfig()
		{
			try
			{
				XElement xelement = ConfigHelper.Load(Global.GameResPath("Config/IPPassList.xml"));
				if (null != xelement)
				{
					List<IPPassList> list = new List<IPPassList>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						IPPassList ippassList = new IPPassList();
						ippassList.ID = Convert.ToInt32(xelement2.Attribute("ID").Value.ToString());
						string text = xelement2.Attribute("MinIP").Value.ToString();
						ippassList.MinIP = IpHelper.IpToInt(text);
						string text2 = xelement2.Attribute("MaxIP").Value.ToString();
						ippassList.MaxIP = IpHelper.IpToInt(text2);
						list.Add(ippassList);
					}
					List<StatisticsControl> list2 = new List<StatisticsControl>();
					xelement = XElement.Load(Global.GameResPath("Config/UserIDStaristicsConfig.xml"));
					if (null != xelement)
					{
						foreach (XElement xelement2 in xelement.Elements())
						{
							list2.Add(new StatisticsControl
							{
								ID = Convert.ToInt32(xelement2.Attribute("ID").Value.ToString()),
								ParamType = Convert.ToInt32(xelement2.Attribute("ParamType").Value.ToString()),
								ParamLimit = Convert.ToInt32(xelement2.Attribute("ParamLimit").Value.ToString()),
								ComParamType = Convert.ToInt32(xelement2.Attribute("ComParamType").Value.ToString()),
								ComParamLimit = Convert.ToDouble(xelement2.Attribute("ComParamLimit").Value.ToString()),
								OperaType = Convert.ToInt32(xelement2.Attribute("OperaType").Value.ToString()),
								OperaParam = Convert.ToInt32(xelement2.Attribute("OperaParam").Value.ToString()),
								Local = (xelement2.Attribute("OperaParam").Value == "1")
							});
						}
						this._IPPassList = list;
						IPStatisticsManager._UserIDControlList = list2;
						IPStatisticsManager.bBeReload = true;
						HashSet<string> hashSet = new HashSet<string>();
						xelement = XElement.Load(Global.GameResPath("Config/UserIDPassList.xml"));
						if (null != xelement)
						{
							foreach (XElement xelement2 in xelement.Elements())
							{
								string text3 = xelement2.Attribute("UserID").Value.ToString();
								string[] array = text3.Split(new char[]
								{
									','
								});
								foreach (string item in array)
								{
									try
									{
										hashSet.Add(item);
									}
									catch
									{
									}
								}
							}
							IPStatisticsManager._UserIDPass = hashSet;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString() + "xmlFileName=IPPassList.xml");
			}
		}

		private bool isCanPassIP(long ipAsInt)
		{
			bool result;
			lock (this._IPPassList)
			{
				if (this._IPPassList == null || this._IPPassList.Count == 0)
				{
					result = false;
				}
				else
				{
					foreach (IPPassList ippassList2 in this._IPPassList)
					{
						if (ippassList2.MinIP <= ipAsInt && ippassList2.MaxIP >= ipAsInt)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		private bool isCanPassUserID(string userID)
		{
			return IPStatisticsManager._UserIDPass.Contains(userID);
		}

		private bool checkUserID(int[] Count, StatisticsControl config)
		{
			bool flag = true;
			if (config.ParamLimit > 0)
			{
				if (Count[config.ParamType] >= config.ParamLimit)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				if (config.ComParamType >= 0)
				{
					double num = double.MaxValue;
					if (Count[config.ComParamType] > 0)
					{
						num = (double)Count[config.ParamType] * 1.0 / (double)Count[config.ComParamType];
					}
					flag = ((config.ComParamLimit > 0.0) ? (num > config.ComParamLimit) : (num < -config.ComParamLimit));
				}
			}
			return flag;
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

		public void processEvent(EventObject eventObject)
		{
			if (!GameManager.IsKuaFuServer)
			{
				IpEventBase ipEventBase = eventObject as IpEventBase;
				IPStatisticsData ipstatisticsData = null;
				if (eventObject.getEventType() == 15)
				{
					PlayerInitGameAsyncEventObject playerInitGameAsyncEventObject = eventObject as PlayerInitGameAsyncEventObject;
					if (null != playerInitGameAsyncEventObject)
					{
						GameClient player = playerInitGameAsyncEventObject.getPlayer();
						this.SetUserIdValue(player.strUserID, (int)player.ClientData.VipExp, Global.GetUnionLevel(player, false));
					}
				}
				lock (IPStatisticsManager.dictIPStatisticsData)
				{
					if (ipEventBase.getIpAsInt() > 0L)
					{
						if (!IPStatisticsManager.dictIPStatisticsData.TryGetValue(ipEventBase.getIpAsInt(), out ipstatisticsData))
						{
							ipstatisticsData = new IPStatisticsData
							{
								ipAsInt = ipEventBase.getIpAsInt()
							};
							IPStatisticsManager.dictIPStatisticsData.Add(ipEventBase.getIpAsInt(), ipstatisticsData);
						}
						if (null == ipstatisticsData)
						{
							return;
						}
						IPInfoType ipinfoType = 0;
						if (IPStatisticsManager.event2IPTypeDict.TryGetValue((EventTypes)eventObject.getEventType(), out ipinfoType))
						{
							ipstatisticsData.IPInfoParams[ipinfoType]++;
						}
					}
				}
				UserParamType userParamType = UserParamType.Begin;
				if (IPStatisticsManager.event2UserTypeDict.TryGetValue((EventTypes)eventObject.getEventType(), out userParamType))
				{
					if (!string.IsNullOrEmpty(ipEventBase.getUserID()))
					{
						UserIDState userIDState = null;
						lock (this.dictUserStateData)
						{
							if (!this.dictUserStateData.TryGetValue(ipEventBase.getUserID(), out userIDState))
							{
								userIDState = new UserIDState();
								this.dictUserStateData.Add(ipEventBase.getUserID(), userIDState);
							}
							if (null != userIDState)
							{
								userIDState.Count[(int)userParamType]++;
							}
						}
					}
				}
			}
		}

		public List<IPStatisticsData> getCurrDataList()
		{
			List<IPStatisticsData> result;
			lock (IPStatisticsManager.dictIPStatisticsData)
			{
				Dictionary<long, Tuple<int, int, int>> socketCnt = Global._TCPManager.MySocketListener.GetSocketCnt();
				foreach (KeyValuePair<long, Tuple<int, int, int>> keyValuePair in socketCnt)
				{
					IPStatisticsData ipstatisticsData = null;
					if (!IPStatisticsManager.dictIPStatisticsData.TryGetValue(keyValuePair.Key, out ipstatisticsData))
					{
						ipstatisticsData = new IPStatisticsData
						{
							ipAsInt = keyValuePair.Key
						};
						IPStatisticsManager.dictIPStatisticsData[keyValuePair.Key] = ipstatisticsData;
					}
					ipstatisticsData.IPInfoParams[0] = keyValuePair.Value.Item1;
					ipstatisticsData.IPInfoParams[1] = keyValuePair.Value.Item2;
					ipstatisticsData.IPInfoParams[2] = keyValuePair.Value.Item3;
				}
				List<IPStatisticsData> list = IPStatisticsManager.dictIPStatisticsData.Values.ToList<IPStatisticsData>();
				IPStatisticsManager.dictIPStatisticsData.Clear();
				result = list;
			}
			return result;
		}

		public void TimerProcForIP()
		{
			if (!GameManager.IsKuaFuServer)
			{
				long num = TimeUtil.NOW();
				if (num - IPStatisticsManager.updateTicks >= 30000L)
				{
					IPStatisticsManager.updateTicks = num;
					int num2 = IPStatisticsClient.getInstance().RequestMinite();
					if (num2 != IPStatisticsManager.lastMinite)
					{
						IPStatisticsManager.lastMinite = num2;
						this.RequestResult();
						this.ReportProc();
					}
				}
			}
		}

		public void TimerProcForUserID()
		{
			int offsetMiniteNow = Global.GetOffsetMiniteNow();
			if (offsetMiniteNow != IPStatisticsManager.lastUserIDMinite)
			{
				IPStatisticsManager.lastUserIDMinite = offsetMiniteNow;
				long num = TimeUtil.NOW();
				List<UserIDState> list = null;
				lock (this.dictUserStateData)
				{
					list = this.dictUserStateData.Values.ToList<UserIDState>();
					this.dictUserStateData.Clear();
				}
				if (IPStatisticsManager.bBeReload)
				{
					IPStatisticsManager.bBeReload = false;
					lock (this.dictOperaUserID)
					{
						foreach (KeyValuePair<string, UserOperaData> keyValuePair in this.dictOperaUserID)
						{
							for (int i = 0; i < 4; i++)
							{
								keyValuePair.Value.OperaTime[i] = 0;
								keyValuePair.Value.OperaCount[i] = 0;
							}
							foreach (StatisticsControl statisticsControl in IPStatisticsManager._UserIDControlList)
							{
								if (!this.checkUserID(keyValuePair.Value.AllCount, statisticsControl))
								{
									if (statisticsControl.OperaType >= 0)
									{
										keyValuePair.Value.OperaTime[statisticsControl.OperaType] = statisticsControl.OperaParam;
										keyValuePair.Value.OperaCount[statisticsControl.OperaType] = keyValuePair.Value.AllCount[statisticsControl.ParamType];
									}
								}
							}
						}
					}
				}
				foreach (UserIDState userIDState in list)
				{
					UserOperaData userOperaData = null;
					foreach (StatisticsControl statisticsControl in IPStatisticsManager._UserIDControlList)
					{
						if (!this.checkUserID(userIDState.Count, statisticsControl))
						{
							lock (this.dictOperaUserID)
							{
								if (!this.dictOperaUserID.TryGetValue(userIDState.UserID, out userOperaData))
								{
									userOperaData = new UserOperaData();
									userOperaData.UserID = userIDState.UserID;
									this.dictOperaUserID.Add(userIDState.UserID, userOperaData);
								}
							}
							userOperaData.IPAsInt = userIDState.IPAsInt;
							if (statisticsControl.OperaType >= 0)
							{
								if ((long)statisticsControl.OperaParam + num > userOperaData.createTicks + (long)userOperaData.OperaTime[statisticsControl.OperaType])
								{
									userOperaData.createTicks = num;
									userOperaData.OperaTime[statisticsControl.OperaType] = statisticsControl.OperaParam;
								}
								if (userIDState.Count[statisticsControl.ParamType] > userOperaData.OperaCount[statisticsControl.OperaType])
								{
									userOperaData.OperaCount[statisticsControl.OperaType] = userIDState.Count[statisticsControl.ParamType];
								}
								for (int i = 0; i < 6; i++)
								{
									userOperaData.AllCount[i] = userIDState.Count[i];
								}
							}
							string text = string.Format("cant pass userid={0}:{1} ruleid={2} paramValue={3}", new object[]
							{
								userOperaData.IPAsInt,
								IpHelper.IntToIp(userOperaData.IPAsInt),
								statisticsControl.ID,
								userIDState.Count[statisticsControl.ParamType]
							});
							if (statisticsControl.ComParamType >= 0)
							{
								text += string.Format(" comParamValue={0}", userIDState.Count[statisticsControl.ComParamType]);
							}
							LogManager.WriteLog(14, text, null, true);
						}
					}
				}
				List<string> list2 = new List<string>();
				lock (this.dictOperaUserID)
				{
					foreach (KeyValuePair<string, UserOperaData> keyValuePair2 in this.dictOperaUserID)
					{
						bool flag5 = true;
						if (this.isCanPassUserID(keyValuePair2.Key))
						{
							flag5 = true;
						}
						else
						{
							for (int i = 0; i < 4; i++)
							{
								if (keyValuePair2.Value.createTicks + (long)(keyValuePair2.Value.OperaTime[i] * 1000) > num)
								{
									flag5 = false;
									break;
								}
							}
						}
						if (flag5)
						{
							list2.Add(keyValuePair2.Key);
						}
					}
					foreach (string key in list2)
					{
						this.dictOperaUserID.Remove(key);
					}
				}
			}
		}

		public void RequestResult()
		{
			long num = TimeUtil.NOW();
			List<long> list = new List<long>();
			lock (IPStatisticsManager.dictOperaMothod)
			{
				foreach (KeyValuePair<long, IPOperaData> keyValuePair in IPStatisticsManager.dictOperaMothod)
				{
					bool flag2 = true;
					if (this.isCanPassIP(keyValuePair.Key))
					{
						flag2 = true;
					}
					else
					{
						for (int i = 0; i < 4; i++)
						{
							if (keyValuePair.Value.recvTicks + (long)(keyValuePair.Value.OperaTime[i] * 1000) > num)
							{
								flag2 = false;
								break;
							}
						}
					}
					if (flag2)
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (long key in list)
				{
					IPStatisticsManager.dictOperaMothod.Remove(key);
				}
			}
			List<IPOperaData> ipstatisticsResult = IPStatisticsClient.getInstance().GetIPStatisticsResult();
			LogManager.WriteLog(13, string.Format("request ip data minite={0} count={1}", IPStatisticsManager.lastMinite, (ipstatisticsResult == null) ? 0 : ipstatisticsResult.Count), null, true);
			if (ipstatisticsResult != null && ipstatisticsResult.Count > 0)
			{
				lock (IPStatisticsManager.dictOperaMothod)
				{
					foreach (IPOperaData ipoperaData in ipstatisticsResult)
					{
						if (!this.isCanPassIP(ipoperaData.ipAsInt))
						{
							IPOperaData ipoperaData2 = null;
							if (IPStatisticsManager.dictOperaMothod.TryGetValue(ipoperaData.ipAsInt, out ipoperaData2))
							{
								for (int i = 0; i < 4; i++)
								{
									if (num + (long)(ipoperaData.OperaTime[i] * 1000) > ipoperaData2.recvTicks + (long)(ipoperaData2.OperaTime[i] * 1000))
									{
										ipoperaData2.recvTicks = num;
										ipoperaData2.OperaTime[i] = ipoperaData.OperaTime[i];
									}
								}
							}
							else
							{
								ipoperaData.recvTicks = num;
								IPStatisticsManager.dictOperaMothod.Add(ipoperaData.ipAsInt, ipoperaData);
							}
							LogManager.WriteLog(13, string.Format("recv need ip minite={0} ip={1}:{2} ", IPStatisticsManager.lastMinite, ipoperaData.ipAsInt, IpHelper.IntToIp(ipoperaData.ipAsInt)), null, true);
						}
					}
				}
			}
		}

		public void ReportProc()
		{
			List<IPStatisticsData> currDataList = this.getCurrDataList();
			if (currDataList != null && currDataList.Count > 0)
			{
				int num = IPStatisticsClient.getInstance().IPStatisticsDataReport(IPStatisticsManager.lastMinite, currDataList);
				LogManager.WriteLog(13, string.Format("report ip data minite={0} count={1} result={2}", IPStatisticsManager.lastMinite, currDataList.Count, num), null, true);
			}
		}

		public bool GetIPInBeOperation(TMSKSocket socket, IPOperaType type)
		{
			if (type == 3)
			{
				this.processEvent(new IpEventBase(49, socket.AcceptIpAsInt, socket.UserID));
			}
			long ipAsIntSafe = Global.GetIpAsIntSafe(socket);
			lock (IPStatisticsManager.dictOperaMothod)
			{
				IPOperaData ipoperaData = null;
				if (!IPStatisticsManager.dictOperaMothod.TryGetValue(ipAsIntSafe, out ipoperaData))
				{
					return false;
				}
				if (ipoperaData.recvTicks + (long)(ipoperaData.OperaTime[type] * 1000) > TimeUtil.NOW())
				{
					if (type != 3)
					{
						string userID = socket.UserID;
						if (!string.IsNullOrEmpty(userID))
						{
							if (this.CheckUserIdValue(userID, "ThisIPPass"))
							{
								return false;
							}
						}
					}
					LogManager.WriteLog(13, string.Format("Operation {0} ip={1}", type.ToString(), Global.GetIPAddress(socket)), null, true);
					return true;
				}
			}
			return false;
		}

		public bool GetUserIDInBeOperation(string userid, IPOperaType type)
		{
			lock (this.dictOperaUserID)
			{
				UserOperaData userOperaData = null;
				if (!this.dictOperaUserID.TryGetValue(userid, out userOperaData))
				{
					return false;
				}
				if (userOperaData.createTicks + (long)(userOperaData.OperaTime[type] * 1000) > TimeUtil.NOW())
				{
					if (this.CheckUserIdValue(userid, "ThisUserIDPass"))
					{
						return false;
					}
					LogManager.WriteLog(14, string.Format("Operation {0} ip={1}", type.ToString(), userid), null, true);
					return true;
				}
			}
			return false;
		}

		public void SetUserIdValue(string userID, int vipExp, int unionLevel)
		{
			lock (IPStatisticsManager._UserIdValueDict)
			{
				int[] array;
				if (IPStatisticsManager._UserIdValueDict.TryGetValue(userID, out array) && null != array)
				{
					if (vipExp > array[0])
					{
						array[0] = vipExp;
					}
					if (unionLevel > array[1])
					{
						array[1] = unionLevel;
					}
				}
				else
				{
					IPStatisticsManager._UserIdValueDict[userID] = new int[]
					{
						vipExp,
						unionLevel
					};
				}
			}
		}

		public int[] GetUserIdValue(string userID)
		{
			try
			{
				int[] array;
				lock (IPStatisticsManager._UserIdValueDict)
				{
					if (IPStatisticsManager._UserIdValueDict.TryGetValue(userID, out array))
					{
						return array;
					}
				}
				array = GameManager.ClientMgr.QueryUserIdValue(userID, 0);
				lock (IPStatisticsManager._UserIdValueDict)
				{
					int[] result;
					if (IPStatisticsManager._UserIdValueDict.TryGetValue(userID, out result))
					{
						return result;
					}
					IPStatisticsManager._UserIdValueDict[userID] = array;
				}
				return array;
			}
			catch (Exception ex)
			{
			}
			return null;
		}

		public bool CheckUserIdValue(string userID, string configName)
		{
			if (!string.IsNullOrEmpty(userID))
			{
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName(configName, ',');
				if (paramValueIntArrayByName != null && paramValueIntArrayByName.Length == 3)
				{
					int[] userIdValue = this.GetUserIdValue(userID);
					if (null != userIdValue)
					{
						if (userIdValue[0] >= paramValueIntArrayByName[0])
						{
							return true;
						}
						if (userIdValue[1] >= Global.GetUnionLevel2(paramValueIntArrayByName[1], paramValueIntArrayByName[2]))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private static IPStatisticsManager instance = new IPStatisticsManager();

		private static Dictionary<long, IPStatisticsData> dictIPStatisticsData = new Dictionary<long, IPStatisticsData>();

		private static int lastMinite = Global.GetOffsetMiniteNow();

		private static long updateTicks = TimeUtil.NOW();

		private static Dictionary<EventTypes, IPInfoType> event2IPTypeDict = new Dictionary<EventTypes, IPInfoType>();

		private static Dictionary<long, IPOperaData> dictOperaMothod = new Dictionary<long, IPOperaData>();

		private List<IPPassList> _IPPassList = new List<IPPassList>();

		private static Dictionary<EventTypes, UserParamType> event2UserTypeDict = new Dictionary<EventTypes, UserParamType>();

		private Dictionary<string, UserIDState> dictUserStateData = new Dictionary<string, UserIDState>();

		private Dictionary<string, UserOperaData> dictOperaUserID = new Dictionary<string, UserOperaData>();

		private static int lastUserIDMinite = Global.GetOffsetMiniteNow();

		private static List<StatisticsControl> _UserIDControlList = new List<StatisticsControl>();

		private static HashSet<string> _UserIDPass = new HashSet<string>();

		private static Dictionary<string, int[]> _UserIdValueDict = new Dictionary<string, int[]>();

		private static bool bBeReload = false;
	}
}
