using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract.Data;
using KF.Remoting.Data;
using KF.Remoting.IPStatistics;
using KF.Remoting.KFBoCai;
using KF.TcpCall;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Remoting;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	public static class KuaFuServerManager
	{
		public static bool CheckConfig()
		{
			KuaFuServerManager.ResourcePath = ConfigurationManager.AppSettings.Get("ResourcePath");
			KuaFuServerManager.ConfigPathStructType = 2;
			string resourcePath = KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.GameRes);
			if (!Directory.Exists(KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.GameRes)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("1", KuaFuServerManager.ResourcePathTypes.Map)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("1", KuaFuServerManager.ResourcePathTypes.MapConfig)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.Isolate)))
			{
				KuaFuServerManager.ConfigPathStructType = 1;
				resourcePath = KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.GameRes);
				if (!Directory.Exists(KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.GameRes)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("1", KuaFuServerManager.ResourcePathTypes.Map)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("1", KuaFuServerManager.ResourcePathTypes.MapConfig)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.Isolate)))
				{
					KuaFuServerManager.ConfigPathStructType = 0;
					if (!Directory.Exists(KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.GameRes)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("1", KuaFuServerManager.ResourcePathTypes.Map)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("1", KuaFuServerManager.ResourcePathTypes.MapConfig)) || !Directory.Exists(KuaFuServerManager.GetResourcePath("Config", KuaFuServerManager.ResourcePathTypes.Isolate)))
					{
						LogManager.WriteLog(1000, "配置文件目录结构不正确", null, true);
					}
				}
			}
			string resourcePath2 = KuaFuServerManager.GetResourcePath("Config/Settings.xml", KuaFuServerManager.ResourcePathTypes.GameRes);
			bool result;
			if (!File.Exists(resourcePath2))
			{
				Console.WriteLine("错误:文件未找到.{0}", resourcePath2);
				result = false;
			}
			else
			{
				resourcePath2 = KuaFuServerManager.GetResourcePath("Config/SystemTasks.xml", KuaFuServerManager.ResourcePathTypes.Isolate);
				if (!File.Exists(resourcePath2))
				{
					Console.WriteLine("错误:文件未找到.{0}", resourcePath2);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		public static bool LoadConfig()
		{
			lock (KuaFuServerManager.Mutex)
			{
				try
				{
					KuaFuServerManager.LoadConfigSuccess = true;
					ConfigurationManager.RefreshSection("appSettings");
					KuaFuServerManager.ServerListUrl = ConfigurationManager.AppSettings.Get("ServerListUrl");
					KuaFuServerManager.KuaFuServerListUrl = ConfigurationManager.AppSettings.Get("KuaFuServerListUrl");
					string text = ConfigurationManager.AppSettings.Get("PlatChargeKingUrl");
					if (!string.IsNullOrEmpty(text))
					{
						KuaFuServerManager.GetPlatChargeKingUrl = text.Split(new char[]
						{
							','
						});
					}
					string text2 = ConfigurationManager.AppSettings.Get("PlatChargeKingUrl_EveryDay");
					if (!string.IsNullOrEmpty(text2))
					{
						KuaFuServerManager.GetPlatChargeKingUrl_EveryDay = text2.Split(new char[]
						{
							','
						});
					}
					ConstData.HTTP_MD5_KEY = ConfigurationManager.AppSettings.Get("MD5Key");
					KuaFuServerManager.ResourcePath = ConfigurationManager.AppSettings.Get("ResourcePath");
					KuaFuServerManager.LimitIP = !string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("LimitIP"));
					KuaFuServerManager.UseLanIp = (ConfigurationManager.AppSettings.Get("UseLanIp") == "1");
					KuaFuServerManager.PingTaiKuaFu = (ConfigurationManager.AppSettings.Get("PingTaiKuaFu") == "1");
					KuaFuServerManager.PingTaiKuaFuTestMode = (ConfigurationManager.AppSettings.Get("PingTaiKuaFuTestMode") == "1");
					string strA = ConfigurationManager.AppSettings.Get("Platform");
					for (PlatformTypes platformTypes = 0; platformTypes < 8; platformTypes++)
					{
						if (0 == string.Compare(strA, platformTypes.ToString(), true))
						{
							KuaFuServerManager.platformType = platformTypes;
							break;
						}
					}
					if (KuaFuServerManager.platformType == 8)
					{
						LogManager.WriteLog(1000, "必须在配置文件中设置有效的平台类型: Platform", null, true);
						KuaFuServerManager.LoadConfigSuccess = false;
						return false;
					}
					string text3 = ConfigurationManager.AppSettings.Get("PTID");
					if (string.IsNullOrEmpty(text3) || !int.TryParse(text3, out KuaFuServerManager.PTID) || KuaFuServerManager.PTID <= 0)
					{
						KuaFuServerManager.PTID = KuaFuServerManager.platformType;
					}
					int num = 0;
					if (KuaFuServerManager.PingTaiKuaFu)
					{
						num = ConstData.ConvertToKuaFuServerID(0, KuaFuServerManager.PTID);
					}
					string text4 = ConfigurationManager.AppSettings.Get("ServicePort");
					if (string.IsNullOrEmpty(text4) || !int.TryParse(text4, out KuaFuServerManager.ServicePort))
					{
						KuaFuServerManager.ServicePort = 0;
					}
					string text5 = KuaFuServerManager.UseLanIp ? "1" : "0";
					List<Tuple<int, string, string, string>> list = new List<Tuple<int, string, string, string>>();
					string text6 = ConfigurationManager.AppSettings.Get("PlatfromAll");
					if (!string.IsNullOrEmpty(text6))
					{
						foreach (string text7 in text6.Split(KuaFuServerManager.SpliteChars, StringSplitOptions.RemoveEmptyEntries))
						{
							PlatformTypes item;
							if (!Enum.TryParse<PlatformTypes>(text7, true, out item))
							{
								LogManager.WriteLog(1000, ".config配置错误,无法解析出程序已知的平台类型,key=PlatfromAll,value=" + text6, null, true);
							}
							else
							{
								string text8 = ConfigurationManager.AppSettings.Get("ServerListUrl_" + text7);
								string text9 = ConfigurationManager.AppSettings.Get("KuaFuServerListUrl_" + text7);
								string text10 = ConfigurationManager.AppSettings.Get("UseLanIp_" + text7);
								if (!string.IsNullOrEmpty(text8) || !string.IsNullOrEmpty(text9))
								{
									list.Add(new Tuple<int, string, string, string>(item, text8, text9, text10 ?? text5));
								}
							}
						}
					}
					if (!list.Exists((Tuple<int, string, string, string> x) => x.Item1 == KuaFuServerManager.PTID))
					{
						list.Add(new Tuple<int, string, string, string>(KuaFuServerManager.PTID, KuaFuServerManager.ServerListUrl, KuaFuServerManager.KuaFuServerListUrl, text5));
					}
					Interlocked.Exchange<List<Tuple<int, string, string, string>>>(ref KuaFuServerManager.KuaFuWorldPlatformServerListUrls, list);
					string text11 = ConfigurationManager.AppSettings.Get("KuaFuMapLine");
					if (!string.IsNullOrEmpty(text11))
					{
						KuaFuServerManager.KuaFuMapLineDict.Clear();
						string[] array2 = text11.Split(new char[]
						{
							'|'
						});
						foreach (string text12 in array2)
						{
							string[] array3 = text12.Split(new char[]
							{
								','
							});
							int key;
							int num2;
							if (array3.Length == 2 && int.TryParse(array3[0], out key) && int.TryParse(array3[1], out num2))
							{
								KuaFuServerManager.KuaFuMapLineDict[key] = num2 + num;
							}
						}
					}
					string text13 = ConfigurationManager.AppSettings.Get("SpecialLine");
					if (!string.IsNullOrEmpty(text13))
					{
						KuaFuServerManager.SpecialLineDict.Clear();
						string[] array2 = text13.Split(new char[]
						{
							'|'
						});
						foreach (string text12 in array2)
						{
							string[] array3 = text12.Split(new char[]
							{
								','
							});
							int key;
							int num2;
							if (array3.Length == 2 && int.TryParse(array3[0], out key) && int.TryParse(array3[1], out num2))
							{
								KuaFuServerManager.SpecialLineDict[key] = num2 + num;
							}
						}
					}
					if (KuaFuServerManager.PingTaiKuaFu)
					{
						KuaFuServerManager.KuaFuMapLineDict.Clear();
						KuaFuServerManager.SpecialLineDict.Clear();
						string text14 = ConfigurationManager.AppSettings.Get("PingTaiKuaFuServerLine");
						if (!string.IsNullOrEmpty(text14))
						{
							KuaFuServerManager.PingTaiKuaFuServerLineDict.Clear();
							string[] array2 = text14.Split(new char[]
							{
								'|'
							});
							foreach (string text12 in array2)
							{
								string[] array3 = text12.Split(new char[]
								{
									','
								});
								int key;
								int num2;
								if (array3.Length == 2 && int.TryParse(array3[0], out key) && int.TryParse(array3[1], out num2))
								{
									KuaFuServerManager.PingTaiKuaFuServerLineDict[key] = num2 + num;
								}
							}
						}
					}
					KuaFuServerManager.systemParamsList.LoadParamsList();
					string resourcePath = KuaFuServerManager.GetResourcePath("Config/VersionSystemOpen.xml", KuaFuServerManager.ResourcePathTypes.GameRes);
					if (!KuaFuServerManager.VersionSystemOpenMgr.LoadVersionSystemOpenData(resourcePath))
					{
						KuaFuServerManager.LoadConfigSuccess = false;
					}
					resourcePath = KuaFuServerManager.GetResourcePath("Config/GameFuncControl.xml", KuaFuServerManager.ResourcePathTypes.GameRes);
					if (!GameFuncControlManager.LoadConfig(resourcePath))
					{
						KuaFuServerManager.LoadConfigSuccess = false;
					}
					resourcePath = KuaFuServerManager.GetResourcePath("Config/ThemeActivityOpen.xml", KuaFuServerManager.ResourcePathTypes.GameRes);
					XElement xelement = ConfigHelper.Load(resourcePath);
					if (null != xelement)
					{
						XElement xelement2 = xelement.Element("ThemeActivityOpen");
						if (null != xelement2)
						{
							KuaFuServerManager.ThemeActivityState = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "Open", 0L);
						}
					}
					string resourcePath2 = KuaFuServerManager.GetResourcePath("Config/MapLine.xml", KuaFuServerManager.ResourcePathTypes.GameRes);
					XElement xelement3 = ConfigHelper.Load(resourcePath2);
					KuaFuServerManager.LineMap2KuaFuLineDataDict.Clear();
					KuaFuServerManager.ServerMap2KuaFuLineDataDict.Clear();
					KuaFuServerManager.KuaFuMapServerIdDict.Clear();
					KuaFuServerManager.MapCode2KuaFuLineDataDict.Clear();
					IEnumerable<XElement> xelements = ConfigHelper.GetXElements(xelement3, "MapLine");
					string elementAttributeValue;
					foreach (XElement xelement4 in xelements)
					{
						KuaFuServerManager.MapMaxOnlineCount = (int)ConfigHelper.GetElementAttributeValueLong(xelement4, "MaxNum", 500L);
						int num3 = (int)ConfigHelper.GetElementAttributeValueLong(xelement4, "Type", 0L);
						elementAttributeValue = ConfigHelper.GetElementAttributeValue(xelement4, "Line", "");
						if (!string.IsNullOrEmpty(elementAttributeValue))
						{
							string[] array2 = elementAttributeValue.Split(new char[]
							{
								'|'
							});
							foreach (string text12 in array2)
							{
								KuaFuLineData kuaFuLineData = new KuaFuLineData();
								string[] array3 = text12.Split(new char[]
								{
									','
								});
								kuaFuLineData.Line = int.Parse(array3[0]);
								kuaFuLineData.MapCode = int.Parse(array3[1]);
								if (num3 == 1)
								{
									KuaFuServerManager.PingTaiKuaFuServerLineDict.TryGetValue(kuaFuLineData.Line, out kuaFuLineData.ServerId);
								}
								else if (num3 == 0)
								{
									KuaFuServerManager.KuaFuMapLineDict.TryGetValue(kuaFuLineData.Line, out kuaFuLineData.ServerId);
								}
								kuaFuLineData.MapType = num3;
								kuaFuLineData.MaxOnlineCount = KuaFuServerManager.MapMaxOnlineCount;
								KuaFuServerManager.LineMap2KuaFuLineDataDict.TryAdd(new IntPairKey(kuaFuLineData.Line, kuaFuLineData.MapCode), kuaFuLineData);
								if (kuaFuLineData.ServerId > 0)
								{
									if (KuaFuServerManager.ServerMap2KuaFuLineDataDict.TryAdd(new IntPairKey(kuaFuLineData.ServerId, kuaFuLineData.MapCode), kuaFuLineData))
									{
										List<KuaFuLineData> list2 = null;
										if (!KuaFuServerManager.KuaFuMapServerIdDict.TryGetValue(kuaFuLineData.ServerId, out list2))
										{
											list2 = new List<KuaFuLineData>();
											KuaFuServerManager.KuaFuMapServerIdDict.TryAdd(kuaFuLineData.ServerId, list2);
										}
										list2.Add(kuaFuLineData);
										if (!KuaFuServerManager.MapCode2KuaFuLineDataDict.TryGetValue(kuaFuLineData.MapCode, out list2))
										{
											list2 = new List<KuaFuLineData>();
											KuaFuServerManager.MapCode2KuaFuLineDataDict.TryAdd(kuaFuLineData.MapCode, list2);
										}
										list2.Add(kuaFuLineData);
									}
								}
							}
						}
					}
					xelement3 = ConfigHelper.Load("config.xml");
					KuaFuServerManager.WritePerformanceLogMs = (int)ConfigHelper.GetElementAttributeValueLong(xelement3, "add", "key", "WritePerformanceLogMs", "value", 10L);
					elementAttributeValue = ConfigHelper.GetElementAttributeValue(xelement3, "add", "key", "LoadConfigFromServer", "value", "true");
					if (!bool.TryParse(elementAttributeValue, out KuaFuServerManager.LoadConfigFromServer))
					{
						KuaFuServerManager.LoadConfigFromServer = false;
					}
					try
					{
						DbHelperMySQL.ExecuteSql("select 1");
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(2, "数据库连接失败", null, true);
						LogManager.WriteException(ex.ToString());
						return false;
					}
					HuanYingSiYuanPersistence.Instance.InitConfig();
					TianTiPersistence.Instance.InitConfig();
					YongZheZhanChangPersistence.Instance.InitConfig();
					KuaFuCopyDbMgr.Instance.InitConfig();
					SpreadPersistence.Instance.InitConfig();
					AllyPersistence.Instance.InitConfig();
					ZhengBaManagerK.Instance().InitConfig();
					RankPersistence.Instance.InitConfig();
					zhengDuoService.Instance().InitConfig();
					LingDiCaiJiService.Instance().InitConfig();
					BangHuiMatchService.Instance().InitConfig();
					KuaFuLueDuoService.Instance().InitConfig();
					KuaFuServerManager.LoadConfigSuccess &= HongBaoManager_K.getInstance().LoadConfig();
					JunTuanEraService.Instance().InitConfig();
					CompService.Instance().InitConfig();
					TSingleton<KuaFuWorldManager>.getInstance().LoadConfig(false);
					RebornService.Instance().InitConfig();
					SpecPriorityActivityMgr.Instance().InitConfig();
					TianTi5v5Service.InitConfig();
					KuaFuServerManager.LoadConfigSuccess &= ZhanDuiZhengBa_K.InitConfig();
					KuaFuServerManager.LoadConfigSuccess &= EscapeBattle_K.InitConfig();
					KFBoCaiConfigManager.LoadConfig(false);
					Zork5v5Service.Instance().InitConfig();
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
					return false;
				}
			}
			return KuaFuServerManager.LoadConfigSuccess;
		}

		public static void InitServer()
		{
			DateTime now = TimeUtil.NowDateTime();
			TianTiPersistence.Instance.LoadTianTiRankData(now);
			ZhengBaManagerK.Instance().ReloadSyncData(now);
			CoupleArenaService.getInstance().StartUp();
			CoupleWishService.getInstance().StartUp();
			RankService.getInstance().StartUp();
			IPStatisticsPersistence.Instance.LoadConfig();
			YaoSaiService.Instance().InitConfig();
			JunTuanPersistence.Instance.InitConfig();
			JunTuanPersistence.Instance.LoadDatabase();
			BangHuiMatchService.Instance().LoadDatabase(now);
			CompService.Instance().LoadDatabase(now);
			RebornService.Instance().LoadDatabase(now);
			SpecPriorityActivityMgr.Instance().LoadDatabase(now);
			ZhanDuiZhengBa_K.LoadSyncData(now, false);
			EscapeBattle_K.LoadSyncData(now, false);
			KFBoCaiManager.GetInstance().StartUp();
			Zork5v5Service.Instance().LoadDatabase(now);
		}

		public static void StartServerConfigThread()
		{
			if (KuaFuServerManager.UpdateServerConfigThread == null)
			{
				KuaFuServerManager.UpdateServerConfigThread = new Thread(delegate()
				{
					for (;;)
					{
						try
						{
							KuaFuServerManager.AsyncFromDataBase();
							KuaFuServerManager.UpdateDataFromServer();
							Thread.Sleep(20000);
						}
						catch (Exception ex)
						{
							LogManager.WriteExceptionUseCache(ex.ToString());
						}
					}
				});
				KuaFuServerManager.UpdateServerConfigThread.IsBackground = true;
				KuaFuServerManager.UpdateServerConfigThread.Start();
			}
			if (KuaFuServerManager.CheckServerLoadThread == null)
			{
				KuaFuServerManager.CheckServerLoadThread = new Thread(delegate()
				{
					for (;;)
					{
						try
						{
							KuaFuServerManager.UpdateServerLoad();
							Thread.Sleep(8000);
						}
						catch (Exception ex)
						{
							LogManager.WriteExceptionUseCache(ex.ToString());
						}
					}
				});
				KuaFuServerManager.CheckServerLoadThread.IsBackground = true;
				KuaFuServerManager.CheckServerLoadThread.Start();
			}
			if (KuaFuServerManager.WorkThread == null)
			{
				KuaFuServerManager.WorkThread = new Thread(delegate()
				{
					for (;;)
					{
						long num = TimeUtil.NOW();
						try
						{
							HongBaoManager_K.getInstance().ThreadProc(null);
							ZhanDuiZhengBa_K.Update();
							EscapeBattle_K.Update();
						}
						catch (Exception ex)
						{
							LogManager.WriteExceptionUseCache(ex.ToString());
						}
						long num2 = TimeUtil.NOW();
						long num3 = Math.Min(1000L + num - num2, 1000L);
						if (num3 > 0L)
						{
							Thread.Sleep((int)num3);
							long num4 = TimeUtil.NOW();
							if (num4 - num2 > 2000L)
							{
								LogManager.WriteLog(2, string.Format("休眠时间异常,Thread.Sleep({0})休眠了{1}ms", num3, num4 - num2), null, true);
							}
						}
					}
				});
				KuaFuServerManager.WorkThread.IsBackground = true;
				KuaFuServerManager.WorkThread.Start();
			}
			if (KuaFuServerManager.FastWorkThread == null)
			{
				KuaFuServerManager.FastWorkThread = new Thread(delegate()
				{
					for (;;)
					{
						long num = TimeUtil.NOW();
						try
						{
							KFServiceBase.TimerProc();
						}
						catch (Exception ex)
						{
							LogManager.WriteExceptionUseCache(ex.ToString());
						}
						long num2 = TimeUtil.NOW();
						long num3 = Math.Min(1000L + num - num2, 200L);
						if (num3 > 0L)
						{
							Thread.Sleep((int)num3);
							long num4 = TimeUtil.NOW();
							if (num4 - num2 > 1000L)
							{
								LogManager.WriteLog(2, string.Format("休眠时间异常,Thread.Sleep({0})休眠了{1}ms", num3, num4 - num2), null, true);
							}
						}
					}
				});
				KuaFuServerManager.FastWorkThread.IsBackground = true;
				KuaFuServerManager.FastWorkThread.Start();
			}
		}

		private static void UpdateServerLoad()
		{
			foreach (KuaFuServerInfo kuaFuServerInfo in KuaFuServerManager._ServerIdServerInfoDict.Values)
			{
				int num = 0;
				int num2 = 0;
				ClientAgentManager.Instance().GetServerState(kuaFuServerInfo.ServerId, out num2, out num);
				if (num != kuaFuServerInfo.Load || num2 != kuaFuServerInfo.State)
				{
					try
					{
						DbHelperMySQL.ExecuteSql(string.Format("update ignore t_server_info set `load`={0},`state`={1} where `serverid`={2}", num, num2, kuaFuServerInfo.ServerId));
						kuaFuServerInfo.Load = num;
						kuaFuServerInfo.State = num2;
					}
					catch (Exception ex)
					{
						LogManager.WriteExceptionUseCache(ex.ToString());
					}
				}
			}
			Dictionary<int, GameTypeStaticsData> gameTypeStatics = ClientAgentManager.Instance().GetGameTypeStatics();
			StringBuilder stringBuilder = new StringBuilder();
			if (gameTypeStatics != null)
			{
				foreach (KeyValuePair<int, GameTypeStaticsData> keyValuePair in gameTypeStatics)
				{
					stringBuilder.AppendFormat("REPLACE INTO t_server_load(gametype, server_alived, fuben_alived, role_signup_count, role_start_game_count,tip) VALUES({0}, {1}, {2}, {3}, {4},'{5}');", new object[]
					{
						keyValuePair.Key,
						keyValuePair.Value.ServerAlived,
						keyValuePair.Value.FuBenAlived,
						keyValuePair.Value.SingUpRoleCount,
						keyValuePair.Value.StartGameRoleCount,
						keyValuePair.Key.ToString()
					});
					stringBuilder.AppendLine();
				}
			}
			if (stringBuilder.Length > 0)
			{
				try
				{
					DbHelperMySQL.ExecuteSql(stringBuilder.ToString());
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
			}
		}

		private static void AsyncFromDataBase()
		{
			try
			{
				object single = DbHelperMySQL.GetSingle("select value from t_async where id = 1");
				if (null != single)
				{
					int num = (int)single;
					if (num > KuaFuServerManager._ServerListAge)
					{
						HashSet<int> hashSet = new HashSet<int>();
						HashSet<int> hashSet2 = new HashSet<int>();
						MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader("select * from t_server_info", false);
						while (mySqlDataReader.Read())
						{
							try
							{
								KuaFuServerInfo kuaFuServerInfo = new KuaFuServerInfo
								{
									ServerId = Convert.ToInt32(mySqlDataReader["serverid"]),
									Ip = mySqlDataReader["ip"].ToString(),
									Port = Convert.ToInt32(mySqlDataReader["port"]),
									DbIp = mySqlDataReader["dbip"].ToString(),
									DbPort = Convert.ToInt32(mySqlDataReader["dbport"]),
									LogDbIp = mySqlDataReader["logdbip"].ToString(),
									LogDbPort = Convert.ToInt32(mySqlDataReader["logdbport"]),
									State = Convert.ToInt32(mySqlDataReader["state"]),
									Flags = Convert.ToInt32(mySqlDataReader["flags"]),
									strServerName = mySqlDataReader["strservername"].ToString(),
									PTID = Convert.ToInt32(mySqlDataReader["ptid"]),
									Age = num
								};
								kuaFuServerInfo.LanIp = kuaFuServerInfo.LogDbIp;
								KuaFuServerManager._ServerIdServerInfoDict[kuaFuServerInfo.ServerId] = kuaFuServerInfo;
								hashSet.Add(kuaFuServerInfo.ServerId);
								if ((kuaFuServerInfo.Flags & 1) != 0)
								{
									hashSet2.Add(kuaFuServerInfo.ServerId);
								}
							}
							catch (Exception ex)
							{
								LogManager.WriteExceptionUseCache(ex.ToString());
							}
						}
						mySqlDataReader.Close();
						int num2 = DataHelper2.UnixSecondsNow();
						lock (KuaFuServerManager.MutexServerList)
						{
							foreach (int num3 in KuaFuServerManager._ServerIdServerInfoDict.Keys.ToList<int>())
							{
								if (!hashSet.Contains(num3))
								{
									KuaFuServerInfo kuaFuServerInfo2;
									KuaFuServerManager._ServerIdServerInfoDict.TryRemove(num3, out kuaFuServerInfo2);
								}
							}
							KuaFuServerManager._ServerListAge = num2;
						}
						DbHelperMySQL.ExecuteSql(string.Format("update t_async set value={0} where id = 1", num2));
						ClientAgentManager.Instance().SetAllKfServerId(hashSet2);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public static string GetResourcePath(string fileName, KuaFuServerManager.ResourcePathTypes resType)
		{
			if (KuaFuServerManager.ConfigPathStructType == 1)
			{
				switch (resType)
				{
				case KuaFuServerManager.ResourcePathTypes.Application:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, DataHelper2.CurrentDirectory, fileName);
				case KuaFuServerManager.ResourcePathTypes.GameRes:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/GameRes", fileName);
				case KuaFuServerManager.ResourcePathTypes.Isolate:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/ServerRes/1/IsolateRes", fileName);
				case KuaFuServerManager.ResourcePathTypes.Map:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/Map", fileName);
				case KuaFuServerManager.ResourcePathTypes.MapConfig:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/MapConfig", fileName);
				}
			}
			else if (KuaFuServerManager.ConfigPathStructType == 2)
			{
				switch (resType)
				{
				case KuaFuServerManager.ResourcePathTypes.Application:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, DataHelper2.CurrentDirectory, fileName);
				case KuaFuServerManager.ResourcePathTypes.GameRes:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/GameRes", fileName);
				case KuaFuServerManager.ResourcePathTypes.Isolate:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/ServerRes/1/IsolateRes", fileName);
				case KuaFuServerManager.ResourcePathTypes.Map:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/Map", fileName);
				case KuaFuServerManager.ResourcePathTypes.MapConfig:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/../MapConfig", fileName);
				}
			}
			else
			{
				switch (resType)
				{
				case KuaFuServerManager.ResourcePathTypes.Application:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, DataHelper2.CurrentDirectory, fileName);
				case KuaFuServerManager.ResourcePathTypes.GameRes:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/GameRes", fileName);
				case KuaFuServerManager.ResourcePathTypes.Isolate:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/IsolateRes", fileName);
				case KuaFuServerManager.ResourcePathTypes.Map:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/GameRes/Map", fileName);
				case KuaFuServerManager.ResourcePathTypes.MapConfig:
					return string.Format("{0}{1}/{2}", KuaFuServerManager.ResourcePath, "/GameRes/MapConfig", fileName);
				}
			}
			return KuaFuServerManager.ResourcePath + "\\" + fileName;
		}

		public static int GetSpecialLineId(GameTypes gameType)
		{
			lock (KuaFuServerManager.Mutex)
			{
				int result;
				if (KuaFuServerManager.SpecialLineDict.TryGetValue(gameType, out result))
				{
					return result;
				}
			}
			return 0;
		}

		public static bool IsGongNengOpened(int gongNengID)
		{
			int num = gongNengID;
			if (num >= 100000 && num < 120000)
			{
				num -= 100000;
			}
			lock (KuaFuServerManager.Mutex)
			{
				if (!KuaFuServerManager.VersionSystemOpenMgr.IsVersionSystemOpen(num))
				{
					return false;
				}
			}
			return true;
		}

		public static List<KuaFuServerInfo> GetKuaFuServerInfoData(int age)
		{
			List<KuaFuServerInfo> list = null;
			lock (KuaFuServerManager.MutexServerList)
			{
				if (age < KuaFuServerManager._ServerListAge)
				{
					if (!KuaFuServerManager.OptimizationServerList)
					{
						return KuaFuServerManager._ServerIdServerInfoDict.Values.ToList<KuaFuServerInfo>();
					}
					list = new List<KuaFuServerInfo>(500);
					foreach (KuaFuServerInfo kuaFuServerInfo in KuaFuServerManager._ServerIdServerInfoDict.Values)
					{
						if (kuaFuServerInfo.ServerId % 10000 == kuaFuServerInfo.Port % 10000)
						{
							list.Add(kuaFuServerInfo);
						}
					}
				}
			}
			return list;
		}

		private static BuffServerListData RequestServerListData(string serverListUrl)
		{
			BuffServerListData result;
			try
			{
				ClientServerListData clientServerListData = new ClientServerListData();
				clientServerListData.lTime = TimeUtil.NOW();
				clientServerListData.strMD5 = MD5Helper.get_md5_string(ConstData.HTTP_MD5_KEY + clientServerListData.lTime.ToString());
				byte[] array = DataHelper2.ObjectToBytes<ClientServerListData>(clientServerListData);
				byte[] array2 = WebHelper.RequestByPost(serverListUrl, array, 10000, 30000);
				if (array2 == null)
				{
					result = null;
				}
				else
				{
					BuffServerListData buffServerListData = DataHelper2.BytesToObject<BuffServerListData>(array2, 0, array2.Length);
					if (buffServerListData == null || buffServerListData.listServerData == null || buffServerListData.listServerData.Count == 0)
					{
						result = null;
					}
					else
					{
						result = buffServerListData;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				result = null;
			}
			return result;
		}

		public static bool UpdateDataFromServer()
		{
			int num = DataHelper2.UnixSecondsNow();
			int num2 = DataHelper2.UnixSecondsNow();
			bool result;
			if (Math.Abs(num - KuaFuServerManager._ServerListAge) < 3)
			{
				result = false;
			}
			else if (!KuaFuServerManager.LoadConfigFromServer)
			{
				result = false;
			}
			else
			{
				if (Monitor.TryEnter(KuaFuServerManager.MutexServerList))
				{
					bool flag = false;
					HashSet<int> hashSet = new HashSet<int>();
					HashSet<int> hashSet2 = new HashSet<int>();
					List<BuffServerListData> list = new List<BuffServerListData>();
					try
					{
						List<Tuple<int, string, string, string>> kuaFuWorldPlatformServerListUrls = KuaFuServerManager.KuaFuWorldPlatformServerListUrls;
						foreach (Tuple<int, string, string, string> tuple in kuaFuWorldPlatformServerListUrls)
						{
							int item = tuple.Item1;
							string item2 = tuple.Item2;
							string item3 = tuple.Item3;
							bool useLanIP = tuple.Item4 == "1";
							if (item == KuaFuServerManager.PTID || KuaFuServerManager.PingTaiKuaFu)
							{
								if (!string.IsNullOrEmpty(item2))
								{
									BuffServerListData buffServerListData = KuaFuServerManager.RequestServerListData(item2);
									list.Add(buffServerListData);
									if (null != buffServerListData)
									{
										buffServerListData.PTID = item;
										buffServerListData.UseLanIP = useLanIP;
									}
								}
								if (!string.IsNullOrEmpty(item3))
								{
									BuffServerListData buffServerListData2 = KuaFuServerManager.RequestServerListData(KuaFuServerManager.KuaFuServerListUrl);
									list.Add(buffServerListData2);
									if (null != buffServerListData2)
									{
										buffServerListData2.PTID = item;
									}
									if (null != buffServerListData2)
									{
										buffServerListData2.PTID = item;
										buffServerListData2.UseLanIP = useLanIP;
									}
								}
							}
						}
						if (list.Exists((BuffServerListData x) => x == null))
						{
							return false;
						}
						foreach (BuffServerListData buffServerListData3 in list)
						{
							int num3 = 0;
							if (KuaFuServerManager.PingTaiKuaFu)
							{
								num3 = ConstData.ConvertToKuaFuServerID(0, buffServerListData3.PTID);
							}
							foreach (BuffServerInfo buffServerInfo in buffServerListData3.listServerData)
							{
								int num4 = buffServerInfo.nServerID % 10000;
								buffServerInfo.nServerID = num4 + num3;
								hashSet.Add(buffServerInfo.nServerID);
								int serverFlags;
								if (num4 >= 9000)
								{
									serverFlags = 1;
								}
								else
								{
									serverFlags = 2;
								}
								KuaFuServerInfo kuaFuServerInfo;
								if (KuaFuServerManager.UpdateServerInfo(buffServerInfo, KuaFuServerManager._ServerListAge, serverFlags, out kuaFuServerInfo, KuaFuServerManager._ServerIdServerInfoDict, buffServerListData3))
								{
									flag = true;
								}
							}
						}
						foreach (int num5 in KuaFuServerManager._ServerIdServerInfoDict.Keys)
						{
							if (!hashSet.Contains(num5))
							{
								KuaFuServerManager.RemoveServerInfo(num5, KuaFuServerManager._ServerIdServerInfoDict);
								flag = true;
							}
						}
						if (flag)
						{
							KuaFuServerManager._ServerListAge = num;
						}
						foreach (KuaFuServerInfo kuaFuServerInfo2 in KuaFuServerManager._ServerIdServerInfoDict.Values)
						{
							if ((kuaFuServerInfo2.Flags & 1) > 0)
							{
								hashSet2.Add(kuaFuServerInfo2.ServerId);
							}
							kuaFuServerInfo2.Age = KuaFuServerManager._ServerListAge;
						}
						ClientAgentManager.Instance().SetAllKfServerId(hashSet2);
					}
					catch (Exception ex)
					{
						LogManager.WriteExceptionUseCache(ex.ToString());
						return false;
					}
					finally
					{
						Monitor.Exit(KuaFuServerManager.MutexServerList);
					}
				}
				result = true;
			}
			return result;
		}

		public static void UpdateServerListAge()
		{
			lock (KuaFuServerManager.MutexServerList)
			{
				KuaFuServerManager._ServerListAge = TimeUtil.AgeByUnixTime(KuaFuServerManager._ServerListAge);
				long num = (long)DataHelper2.UnixSecondsNow();
				long num2 = TimeUtil.NOW();
				long num3 = TimeUtil.Before1970Ticks + num * 1000L;
				long num4 = (long)KuaFuServerManager._ServerListAge - num;
				long num5 = num3 - num2;
			}
		}

		public static bool UpdateServerInfo(BuffServerInfo item, int ServerListAge, int serverFlags, out KuaFuServerInfo data, ConcurrentDictionary<int, KuaFuServerInfo> ServerIdServerInfoDict, BuffServerListData list)
		{
			bool flag = false;
			if (!ServerIdServerInfoDict.TryGetValue(item.nServerID, out data))
			{
				data = new KuaFuServerInfo
				{
					ServerId = item.nServerID,
					Age = ServerListAge,
					Flags = serverFlags,
					strServerName = item.strServerName,
					PTID = list.PTID
				};
				ServerIdServerInfoDict[item.nServerID] = data;
				flag = true;
			}
			string text = item.strURL;
			if (list.UseLanIP)
			{
				text = item.strLanIp;
			}
			if (data.Ip != item.strURL || data.Port != item.nServerPort || data.LanIp != item.strLanIp || data.strServerName != item.strServerName || data.PTID != list.PTID || data.DbIp != text)
			{
				data.Ip = item.strURL;
				data.Port = item.nServerPort;
				data.DbPort = item.nServerPort + 10000;
				data.LogDbPort = item.nServerPort + 20000;
				data.LanIp = item.strLanIp;
				data.strServerName = item.strServerName;
				data.Flags = serverFlags;
				data.Age = ServerListAge;
				data.PTID = list.PTID;
				data.DbIp = text;
				data.LogDbIp = text;
				flag = true;
			}
			if (flag)
			{
				try
				{
					DbHelperMySQL.ExecuteSql(string.Format("INSERT INTO t_server_info(serverid,ip,port,dbip,dbport,logdbip,logdbport,state,age,flags,strservername,ptid) VALUES({0},'{1}',{2},'{9}',{3},'{6}',{4},0,0,{5},'{7}','{8}') ON DUPLICATE KEY UPDATE `ip`='{1}',port={2},dbip='{9}',dbport={3},logdbip='{6}',logdbport={4},flags={5},strservername='{7}',ptid='{8}'", new object[]
					{
						data.ServerId,
						data.Ip,
						data.Port,
						data.DbPort,
						data.LogDbPort,
						data.Flags,
						data.LanIp,
						data.strServerName,
						data.PTID,
						data.DbIp
					}));
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.Message);
				}
			}
			return flag;
		}

		public static void RemoveServerInfo(int serverId, ConcurrentDictionary<int, KuaFuServerInfo> ServerIdServerInfoDict)
		{
			KuaFuServerInfo kuaFuServerInfo;
			ServerIdServerInfoDict.TryRemove(serverId, out kuaFuServerInfo);
			try
			{
				DbHelperMySQL.ExecuteSql(string.Format("delete from t_server_info where `serverid`={0}", serverId));
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		public static KuaFuServerInfo GetKuaFuServerInfo(int serverId)
		{
			lock (KuaFuServerManager.MutexServerList)
			{
				KuaFuServerInfo result;
				if (KuaFuServerManager._ServerIdServerInfoDict.TryGetValue(serverId, out result))
				{
					return result;
				}
			}
			return null;
		}

		public static KuaFuLoginInfo GetKuaFuLoginInfo(int serverId, int kfServerID)
		{
			KuaFuLoginInfo kuaFuLoginInfo = new KuaFuLoginInfo
			{
				ServerId = serverId,
				KuaFuServerId = kfServerID
			};
			lock (KuaFuServerManager.MutexServerList)
			{
				KuaFuServerInfo kuaFuServerInfo;
				if (KuaFuServerManager._ServerIdServerInfoDict.TryGetValue(serverId, out kuaFuServerInfo))
				{
					kuaFuLoginInfo.LocalIPs = new string[]
					{
						kuaFuServerInfo.DbIp,
						kuaFuServerInfo.DbIp
					};
					kuaFuLoginInfo.LocalPorts = new int[]
					{
						kuaFuServerInfo.DbPort,
						kuaFuServerInfo.LogDbPort
					};
				}
				if (KuaFuServerManager._ServerIdServerInfoDict.TryGetValue(kfServerID, out kuaFuServerInfo))
				{
					kuaFuLoginInfo.KuaFuIP = kuaFuServerInfo.Ip;
					kuaFuLoginInfo.KuaFuPort = kuaFuServerInfo.Port;
				}
			}
			return kuaFuLoginInfo;
		}

		public static bool UpdateServerGameConfig(int serverId, int gameType, int capacity, ConcurrentDictionary<int, KuaFuServerGameConfig> KuaFuServerIdGameConfigDict)
		{
			bool result = false;
			KuaFuServerGameConfig kuaFuServerGameConfig;
			if (!KuaFuServerIdGameConfigDict.TryGetValue(serverId, out kuaFuServerGameConfig))
			{
				kuaFuServerGameConfig = new KuaFuServerGameConfig
				{
					ServerId = serverId,
					GameType = gameType
				};
				KuaFuServerIdGameConfigDict[serverId] = kuaFuServerGameConfig;
				result = true;
			}
			else if (kuaFuServerGameConfig.Capacity != capacity)
			{
				kuaFuServerGameConfig.Capacity = capacity;
				result = true;
			}
			return result;
		}

		public static int GetUniqueClientId()
		{
			int num = DataHelper2.UnixSecondsNow();
			lock (KuaFuServerManager.UniqueClientIdMutex)
			{
				KuaFuServerManager.UniqueClientId++;
				if (KuaFuServerManager.UniqueClientId < num)
				{
					KuaFuServerManager.UniqueClientId = num;
				}
				num = KuaFuServerManager.UniqueClientId;
			}
			return num;
		}

		public static int GetServerIDFromZoneID(int zoneID)
		{
			KuaFuServerInfo kuaFuServerInfo;
			if (KuaFuServerManager._ServerIdServerInfoDict.TryGetValue(zoneID, out kuaFuServerInfo))
			{
				int num = kuaFuServerInfo.ServerId / 10000 * 10000 + kuaFuServerInfo.Port % 10000;
				if (KuaFuServerManager._ServerIdServerInfoDict.ContainsKey(num))
				{
					return num;
				}
			}
			return zoneID;
		}

		public static string FormatName(int serverID, string name)
		{
			return KuaFuServerManager.FormatName(name, serverID);
		}

		public static string FormatName(string name, int serverID)
		{
			string result;
			if (KuaFuServerManager.ThemeActivityState == 0)
			{
				result = string.Format("S{0}·{1}", ConstData.ConvertToNormalServerID(serverID), name);
			}
			else
			{
				string strServerName;
				KuaFuServerInfo kuaFuServerInfo;
				for (;;)
				{
					lock (KuaFuServerManager.ZoneID2ZoneNameDict)
					{
						if (KuaFuServerManager.ZoneID2ZoneNameDict.TryGetValue(serverID, out strServerName))
						{
							goto IL_E7;
						}
					}
					kuaFuServerInfo = KuaFuServerManager.GetKuaFuServerInfo(serverID);
					if (kuaFuServerInfo != null && !string.IsNullOrEmpty(kuaFuServerInfo.strServerName))
					{
						break;
					}
					Thread.Sleep(100);
				}
				strServerName = kuaFuServerInfo.strServerName;
				lock (KuaFuServerManager.ZoneID2ZoneNameDict)
				{
					KuaFuServerManager.ZoneID2ZoneNameDict[serverID] = strServerName;
				}
				IL_E7:
				result = string.Format("[{0}]{1}", strServerName, name);
			}
			return result;
		}

		public static int EnterKuaFuMapLine(int line, int mapCode)
		{
			lock (KuaFuServerManager.Mutex)
			{
				KuaFuLineData kuaFuLineData;
				if (KuaFuServerManager.LineMap2KuaFuLineDataDict.TryGetValue(new IntPairKey(line, mapCode), out kuaFuLineData))
				{
					if (kuaFuLineData.OnlineCount < kuaFuLineData.MaxOnlineCount)
					{
						kuaFuLineData.OnlineCount++;
						return kuaFuLineData.ServerId;
					}
				}
			}
			return 0;
		}

		public static void UpdateKuaFuLineData(int serverId, Dictionary<int, int> mapClientCountDict)
		{
			if (null != mapClientCountDict)
			{
				lock (KuaFuServerManager.Mutex)
				{
					foreach (KeyValuePair<int, int> keyValuePair in mapClientCountDict)
					{
						KuaFuLineData kuaFuLineData;
						if (KuaFuServerManager.ServerMap2KuaFuLineDataDict.TryGetValue(new IntPairKey(serverId, keyValuePair.Key), out kuaFuLineData))
						{
							kuaFuLineData.OnlineCount = keyValuePair.Value;
							kuaFuLineData.State = 1;
						}
					}
				}
			}
		}

		public static void UpdateKuaFuMapLineState(int serverId, int state)
		{
			List<KuaFuLineData> list = null;
			lock (KuaFuServerManager.Mutex)
			{
				if (KuaFuServerManager.KuaFuMapServerIdDict.TryGetValue(serverId, out list))
				{
					foreach (KuaFuLineData kuaFuLineData in list)
					{
						kuaFuLineData.State = state;
					}
				}
			}
		}

		public static List<KuaFuLineData> GetKuaFuLineDataList(int mapCode)
		{
			lock (KuaFuServerManager.Mutex)
			{
				List<KuaFuLineData> result;
				if (KuaFuServerManager.MapCode2KuaFuLineDataDict.TryGetValue(mapCode, out result))
				{
					return result;
				}
			}
			return null;
		}

		public static bool WaitStop(int millisecondsTimeout = 0)
		{
			return KuaFuServerManager.StopEvent.Wait(millisecondsTimeout);
		}

		public static void OnStartServer()
		{
			try
			{
				KuaFuServerManager.FileWatcher.Path = Environment.CurrentDirectory;
				KuaFuServerManager.FileWatcher.Filter = "Server*.txt";
				KuaFuServerManager.FileWatcher.Changed += KuaFuServerManager.OnFileChanged;
				KuaFuServerManager.FileWatcher.NotifyFilter = NotifyFilters.LastWrite;
				KuaFuServerManager.FileWatcher.EnableRaisingEvents = true;
				KuaFuServerManager.FileWatcher.IncludeSubdirectories = false;
				KuaFuServerManager.WorkerThread.Start();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		public static void OnStopServer()
		{
			try
			{
				bool flag = false;
				lock (KuaFuServerManager.Mutex)
				{
					if (!KuaFuServerManager.ServerStop)
					{
						KuaFuServerManager.ServerStop = true;
						flag = true;
					}
				}
				if (flag)
				{
					CoupleArenaService.getInstance().OnStopServer();
					CoupleWishService.getInstance().OnStopServer();
					KuaFuLueDuoService.Instance().OnStopServer();
					CompService.Instance().OnStopServer();
					RebornService.Instance().OnStopServer();
					TianTiPersistence.Instance.OnStopServer();
					KuaFuServerManager.StopEvent.Signal();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		private static void OnFileChanged(object sender, FileSystemEventArgs e)
		{
			lock (KuaFuServerManager.Mutex)
			{
				if (KuaFuServerManager.ServerStop)
				{
					return;
				}
			}
			if (e.ChangeType == WatcherChangeTypes.Changed)
			{
				if (e.Name.EndsWith("ServerStop.txt"))
				{
					string text = File.ReadAllText(e.Name);
					if (text.Trim().StartsWith(Process.GetCurrentProcess().Id.ToString()))
					{
						KuaFuServerManager.OnStopServer();
					}
				}
			}
		}

		public static void TimerThreadProc()
		{
			for (;;)
			{
				try
				{
					if (KuaFuServerManager.ServerStop)
					{
						break;
					}
					DateTime now = TimeUtil.NowDateTime();
					KuaFuLueDuoService.Instance().Update(now);
					Thread.Sleep(500);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
		}

		private const int ThreadInterval = 500;

		public static object Mutex = new object();

		private static object MutexServerList = new object();

		public static bool LoadConfigFromServer = false;

		public static int WritePerformanceLogMs = 1;

		public static int MapMaxOnlineCount = 500;

		public static int MaxGetAsyncItemDataCount = 10000;

		private static string ServerListUrl;

		private static string KuaFuServerListUrl;

		public static string[] GetPlatChargeKingUrl;

		public static string[] GetPlatChargeKingUrl_EveryDay;

		private static int _ServerListAge;

		private static Thread UpdateServerConfigThread;

		private static Thread CheckServerLoadThread;

		private static Thread WorkThread;

		private static Thread FastWorkThread;

		private static object UniqueClientIdMutex = new object();

		private static int UniqueClientId = 0;

		public static string ResourcePath;

		public static Dictionary<int, int> KuaFuMapLineDict = new Dictionary<int, int>();

		public static Dictionary<int, int> SpecialLineDict = new Dictionary<int, int>();

		public static Dictionary<int, int> PingTaiKuaFuServerLineDict = new Dictionary<int, int>();

		public static PlatformTypes platformType = 8;

		public static bool LoadConfigSuccess = true;

		public static bool LimitIP = true;

		public static bool UseLanIp = false;

		public static bool EnableGMSetAllServerTime;

		public static int ThemeActivityState;

		public static bool PingTaiKuaFu = false;

		public static bool PingTaiKuaFuTestMode = false;

		public static int PTID;

		public static int ServicePort;

		public static List<Tuple<int, string, string, string>> KuaFuWorldPlatformServerListUrls = new List<Tuple<int, string, string, string>>();

		public static ConcurrentDictionary<IntPairKey, KuaFuLineData> LineMap2KuaFuLineDataDict = new ConcurrentDictionary<IntPairKey, KuaFuLineData>();

		public static ConcurrentDictionary<IntPairKey, KuaFuLineData> ServerMap2KuaFuLineDataDict = new ConcurrentDictionary<IntPairKey, KuaFuLineData>();

		public static ConcurrentDictionary<int, List<KuaFuLineData>> KuaFuMapServerIdDict = new ConcurrentDictionary<int, List<KuaFuLineData>>();

		public static ConcurrentDictionary<int, List<KuaFuLineData>> MapCode2KuaFuLineDataDict = new ConcurrentDictionary<int, List<KuaFuLineData>>();

		public static VersionSystemOpenManager VersionSystemOpenMgr = new VersionSystemOpenManager();

		public static SystemParamsListKF systemParamsList = new SystemParamsListKF();

		private static int ConfigPathStructType = 1;

		public static char[] SpliteChars = new char[]
		{
			'-',
			',',
			'|',
			';'
		};

		private static GetKuaFuServerListRequestData KuaFuServerListRequestData = new GetKuaFuServerListRequestData();

		private static ConcurrentDictionary<int, KuaFuServerInfo> _ServerIdServerInfoDict = new ConcurrentDictionary<int, KuaFuServerInfo>();

		public static bool OptimizationServerList = false;

		private static Dictionary<int, string> ZoneID2ZoneNameDict = new Dictionary<int, string>();

		private static FileSystemWatcher FileWatcher = new FileSystemWatcher();

		private static bool ServerStop;

		private static CountdownEvent StopEvent = new CountdownEvent(1);

		private static Thread WorkerThread = new Thread(new ThreadStart(KuaFuServerManager.TimerThreadProc));

		public enum ResourcePathTypes
		{
			Application,
			GameRes,
			Isolate,
			Map,
			MapConfig
		}
	}
}
