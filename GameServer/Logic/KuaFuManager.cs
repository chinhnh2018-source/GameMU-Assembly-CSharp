using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Logic.Copy;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.MoRi;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using KF.TcpCall;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class KuaFuManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		static KuaFuManager()
		{
			AsyncDataItem.InitKnownTypes();
		}

		public static KuaFuManager getInstance()
		{
			return KuaFuManager.instance;
		}

		public bool initialize()
		{
			return true;
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			try
			{
				this.CoreInterface = coreInterface;
				if (!this.InitConfig())
				{
					return false;
				}
				RemotingConfiguration.Configure(Process.GetCurrentProcess().MainModule.FileName + ".config", false);
				if (!HuanYingSiYuanClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!TianTiClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!YongZheZhanChangClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!KFCopyRpcClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!SpreadClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!AllyClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!IPStatisticsClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!JunTuanClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				if (!KuaFuWorldClient.getInstance().initialize(coreInterface))
				{
					return false;
				}
				GlobalEventSource.getInstance().registerListener(12, KuaFuManager.getInstance());
			}
			catch (Exception ex)
			{
				return false;
			}
			return true;
		}

		public bool startup()
		{
			try
			{
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("HuanYingSiYuanClient.TimerProc", new EventHandler(HuanYingSiYuanClient.getInstance().TimerProc)), 2000, 2857);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("TianTiClient.TimerProc", new EventHandler(TianTiClient.getInstance().TimerProc)), 2000, 2857);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("YongZheZhanChangClient.TimerProc", new EventHandler(YongZheZhanChangClient.getInstance().TimerProc)), 2000, 3389);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("KFCopyRpcClient.TimerProc", new EventHandler(KFCopyRpcClient.getInstance().TimerProc)), 2000, 2732);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("SpreadClient.TimerProc", new EventHandler(SpreadClient.getInstance().TimerProc)), 2000, 4285);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("AllyClient.TimerProc", new EventHandler(AllyClient.getInstance().TimerProc)), 2000, 5714);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("IPStatisticsClient.TimerProc", new EventHandler(IPStatisticsClient.getInstance().TimerProc)), 2000, 5000);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("JunTuanClient.TimerProc", new EventHandler(JunTuanClient.getInstance().TimerProc)), 2000, 2500);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("KuaFuWorldClient.TimerProc", new EventHandler(KuaFuWorldClient.getInstance().TimerProc)), 2000, 3389);
				lock (this.RuntimeData.Mutex)
				{
					if (null == this.RuntimeData.BackGroundThread)
					{
						this.RuntimeData.BackGroundThread = new Thread(new ThreadStart(this.BackGroudThreadProc));
						this.RuntimeData.BackGroundThread.IsBackground = true;
						this.RuntimeData.BackGroundThread.Start();
					}
				}
			}
			catch (Exception ex)
			{
				return false;
			}
			return true;
		}

		public bool showdown()
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.BackGroundThread.Abort();
					this.RuntimeData.BackGroundThread = null;
				}
				GlobalEventSource.getInstance().removeListener(12, KuaFuManager.getInstance());
				if (!HuanYingSiYuanClient.getInstance().showdown())
				{
					return false;
				}
				if (!SpreadClient.getInstance().showdown())
				{
					return false;
				}
				if (!AllyClient.getInstance().showdown())
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				return false;
			}
			return true;
		}

		public bool destroy()
		{
			try
			{
				if (!HuanYingSiYuanClient.getInstance().destroy())
				{
					return false;
				}
				if (!SpreadClient.getInstance().destroy())
				{
					return false;
				}
				if (!AllyClient.getInstance().destroy())
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				return false;
			}
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			int num = eventType;
			if (num != 12)
			{
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
		}

		public bool InitConfig()
		{
			bool result = true;
			try
			{
				ConfigurationManager.RefreshSection("appSettings");
				KuaFuManager.KuaFuWorldKuaFuGameServer = (ConfigurationManager.AppSettings.Get("KuaFuWorldKuaFuGameServer") == "1");
				foreach (string text in RuntimeVariableNames.KuaFuUriNames)
				{
					string text2 = ConfigurationManager.AppSettings.Get(text);
					this.CoreInterface.SetRuntimeVariable(text, text2);
				}
				KFCallManager.Start();
			}
			catch (Exception ex)
			{
				result = false;
				LogManager.WriteLog(1000, "加载跨服中心服务地址配置失败。", ex, true);
			}
			return result;
		}

		public bool OnUserLogin2(TMSKSocket socket, int verSign, string userID, string userName, string lastTime, string isadult, string signCode)
		{
			WebLoginToken webLoginToken = new WebLoginToken
			{
				VerSign = verSign,
				UserID = userID,
				UserName = userName,
				LastTime = lastTime,
				Isadult = isadult,
				SignCode = signCode
			};
			socket.ClientKuaFuServerLoginData.WebLoginToken = webLoginToken;
			return true;
		}

		public bool OnUserLogin(TMSKSocket socket, int verSign, string userID, string userName, string lastTime, string userToken, string isadult, string signCode, int serverId = 0, string ip = null, int port = 0, int roleId = 0, int gameType = 0, long gameId = 0L)
		{
			KuaFuServerLoginData clientKuaFuServerLoginData = socket.ClientKuaFuServerLoginData;
			if (serverId > 0 && ip != null)
			{
				clientKuaFuServerLoginData.ServerId = serverId;
				clientKuaFuServerLoginData.ServerIp = ip;
				clientKuaFuServerLoginData.ServerPort = port;
				clientKuaFuServerLoginData.RoleId = roleId;
				clientKuaFuServerLoginData.GameType = gameType;
				clientKuaFuServerLoginData.GameId = gameId;
			}
			if (clientKuaFuServerLoginData.WebLoginToken == null)
			{
				clientKuaFuServerLoginData.WebLoginToken = new WebLoginToken
				{
					VerSign = verSign,
					UserID = userID,
					UserName = userName,
					LastTime = lastTime,
					Isadult = isadult,
					SignCode = signCode
				};
			}
			if (roleId > 0 && serverId > 0 && gameType > 0)
			{
				if (GameManager.ServerLineID != GameManager.ServerId)
				{
					LogManager.WriteLog(2, "GameManager.ServerLineID未配置,禁止跨服登录", null, true);
					return false;
				}
				if (string.IsNullOrEmpty(ip) || port <= 0 || gameType <= 0 || gameId <= 0L)
				{
					LogManager.WriteLog(2, string.Format("角色{0}未能在服务器列表中找本服务器，作为跨服服务器", clientKuaFuServerLoginData.RoleId), null, true);
					return false;
				}
				if (!KuaFuManager.getInstance().CanKuaFuLogin())
				{
					return false;
				}
				socket.ServerId = serverId;
				switch (gameType)
				{
				case 1:
					socket.IsKuaFuLogin = HuanYingSiYuanClient.getInstance().KuaFuLogin(clientKuaFuServerLoginData);
					goto IL_399;
				case 2:
					socket.IsKuaFuLogin = TianTiClient.getInstance().KuaFuLogin(clientKuaFuServerLoginData);
					goto IL_399;
				case 5:
				case 6:
				case 15:
					socket.IsKuaFuLogin = YongZheZhanChangClient.getInstance().KuaFuLogin(clientKuaFuServerLoginData);
					goto IL_399;
				case 7:
					socket.IsKuaFuLogin = YongZheZhanChangClient.getInstance().CanEnterKuaFuMap(clientKuaFuServerLoginData);
					goto IL_399;
				case 8:
					socket.IsKuaFuLogin = SingletonTemplate<CopyTeamManager>.Instance().HandleKuaFuLogin(clientKuaFuServerLoginData);
					goto IL_399;
				case 10:
					socket.IsKuaFuLogin = LangHunLingYuManager.getInstance().CanEnterKuaFuMap(clientKuaFuServerLoginData);
					goto IL_399;
				case 12:
					socket.IsKuaFuLogin = SingletonTemplate<ZhengBaManager>.Instance().CanKuaFuLogin(clientKuaFuServerLoginData);
					goto IL_399;
				case 13:
					socket.IsKuaFuLogin = SingletonTemplate<CoupleArenaManager>.Instance().CanKuaFuLogin(clientKuaFuServerLoginData);
					goto IL_399;
				case 17:
					socket.IsKuaFuLogin = true;
					goto IL_399;
				case 19:
				case 20:
					socket.IsKuaFuLogin = KarenBattleManager.getInstance().KuaFuLogin(clientKuaFuServerLoginData);
					goto IL_399;
				case 22:
					socket.IsKuaFuLogin = true;
					goto IL_399;
				case 24:
					socket.IsKuaFuLogin = BangHuiMatchManager.getInstance().KuaFuLogin(clientKuaFuServerLoginData);
					goto IL_399;
				case 25:
					socket.IsKuaFuLogin = KuaFuLueDuoManager.getInstance().KuaFuLogin(clientKuaFuServerLoginData);
					goto IL_399;
				case 27:
				case 28:
				case 29:
					socket.IsKuaFuLogin = true;
					goto IL_399;
				case 30:
					socket.IsKuaFuLogin = CompBattleManager.getInstance().KuaFuLogin(clientKuaFuServerLoginData);
					goto IL_399;
				case 31:
					socket.IsKuaFuLogin = CompMineManager.getInstance().KuaFuLogin(clientKuaFuServerLoginData);
					goto IL_399;
				case 32:
					socket.IsKuaFuLogin = true;
					goto IL_399;
				case 34:
					socket.IsKuaFuLogin = TianTi5v5Manager.getInstance().CanKuaFuLogin(clientKuaFuServerLoginData);
					goto IL_399;
				case 36:
					socket.IsKuaFuLogin = ZorkBattleManager.getInstance().KuaFuLogin(clientKuaFuServerLoginData);
					goto IL_399;
				}
				EventObjectEx_I1 eventObjectEx_I = new EventObjectEx_I1(clientKuaFuServerLoginData, 61, gameType);
				if (GlobalEventSource4Scene.getInstance().fireEvent(eventObjectEx_I, 10007))
				{
					socket.IsKuaFuLogin = eventObjectEx_I.Result;
				}
				IL_399:
				string ip2 = "";
				int port2 = 0;
				string logIp = "";
				int logPort = 0;
				if (clientKuaFuServerLoginData.ips != null && clientKuaFuServerLoginData.ports != null)
				{
					ip2 = clientKuaFuServerLoginData.ips[0];
					logIp = clientKuaFuServerLoginData.ips[1];
					port2 = clientKuaFuServerLoginData.ports[0];
					logPort = clientKuaFuServerLoginData.ports[1];
				}
				else if (!KuaFuManager.getInstance().GetKuaFuDbServerInfo(serverId, out ip2, out port2, out logIp, out logPort))
				{
					LogManager.WriteLog(2, string.Format("GameType{0}未能找到角色{1}的原服务器的服务器IP和端口", gameType, clientKuaFuServerLoginData.RoleId), null, true);
					return false;
				}
				if (socket.IsKuaFuLogin && serverId != 0)
				{
					if (serverId != 0)
					{
						if (!this.InitGameDbConnection(serverId, ip2, port2, logIp, logPort))
						{
							LogManager.WriteLog(2, string.Format("连接角色{0}的原服务器的GameDBServer和LogDBServer失败", clientKuaFuServerLoginData.RoleId), null, true);
							return false;
						}
					}
					return socket.IsKuaFuLogin;
				}
			}
			else
			{
				if (GameManager.IsKuaFuServer)
				{
					LogManager.WriteLog(2, "跨服服务器禁止非跨服登录,请检查是否将LineID配置,原服应当为1", null, true);
					return false;
				}
				if (KuaFuManager.getInstance().LocalLogin(userID))
				{
					clientKuaFuServerLoginData.RoleId = 0;
					clientKuaFuServerLoginData.GameId = 0L;
					clientKuaFuServerLoginData.GameType = 0;
					clientKuaFuServerLoginData.ServerId = 0;
					socket.ServerId = 0;
					socket.IsKuaFuLogin = false;
					return true;
				}
			}
			LogManager.WriteLog(2, string.Format("未能找到角色{0}的跨服活动或副本信息", clientKuaFuServerLoginData.RoleId), null, true);
			return false;
		}

		public bool OnInitGame(GameClient client)
		{
			int gameType = Global.GetClientKuaFuServerLoginData(client).GameType;
			bool result;
			if (KuaFuManager.KuaFuWorldKuaFuGameServer && !KuaFuManager.KuaFuWorldGameTypes.Contains(gameType))
			{
				result = true;
			}
			else
			{
				switch (gameType)
				{
				case 1:
					return HuanYingSiYuanManager.getInstance().OnInitGame(client);
				case 2:
					return TianTiManager.getInstance().OnInitGame(client);
				case 3:
					return SingletonTemplate<MoRiJudgeManager>.Instance().OnInitGame(client);
				case 4:
					return ElementWarManager.getInstance().OnInitGame(client);
				case 5:
					return YongZheZhanChangManager.getInstance().OnInitGame(client);
				case 6:
					return KuaFuBossManager.getInstance().OnInitGame(client);
				case 7:
				case 32:
					return KuaFuMapManager.getInstance().OnInitGame(client);
				case 8:
					return SingletonTemplate<CopyTeamManager>.Instance().HandleKuaFuInitGame(client);
				case 10:
					return LangHunLingYuManager.getInstance().OnInitGameKuaFu(client);
				case 12:
					return SingletonTemplate<ZhengBaManager>.Instance().KuaFuInitGame(client);
				case 13:
					return SingletonTemplate<CoupleArenaManager>.Instance().KuaFuInitGame(client);
				case 15:
					return KingOfBattleManager.getInstance().OnInitGame(client);
				case 17:
					return ZhengDuoManager.getInstance().OnInitGame(client);
				case 19:
					return KarenBattleManager_MapWest.getInstance().OnInitGame(client);
				case 20:
					return KarenBattleManager_MapEast.getInstance().OnInitGame(client);
				case 22:
					return LingDiCaiJiManager.getInstance().KuaFuInitGame(client);
				case 24:
					return BangHuiMatchManager.getInstance().OnInitGameKuaFu(client);
				case 25:
					return KuaFuLueDuoManager.getInstance().OnInitGameKuaFu(client);
				case 26:
					return WanMoXiaGuManager.getInstance().OnInitGame(client);
				case 27:
				case 28:
				case 29:
					return CompManager.getInstance().OnInitGameKuaFu(client);
				case 30:
					return CompBattleManager.getInstance().OnInitGameKuaFu(client);
				case 31:
					return CompMineManager.getInstance().OnInitGameKuaFu(client);
				case 34:
					return TianTi5v5Manager.getInstance().OnInitGame(client);
				case 36:
					return ZorkBattleManager.getInstance().OnInitGameKuaFu(client);
				}
				EventObjectEx_I1 eventObjectEx_I = new EventObjectEx_I1(client, 62, gameType);
				result = (GlobalEventSource4Scene.getInstance().fireEvent(eventObjectEx_I, 10007) && eventObjectEx_I.Result);
			}
			return result;
		}

		public void OnStartPlayGame(GameClient client)
		{
			int gameType = Global.GetClientKuaFuServerLoginData(client).GameType;
			int num = gameType;
			if (num != 7)
			{
				switch (num)
				{
				case 27:
				case 28:
				case 29:
					CompManager.getInstance().OnStartPlayGame(client);
					return;
				case 30:
				case 31:
					return;
				case 32:
					break;
				default:
					return;
				}
			}
			KuaFuMapManager.getInstance().OnStartPlayGame(client);
		}

		public void OnLeaveScene(GameClient client, SceneUIClasses sceneType, bool logout = false)
		{
			if (client.ClientSocket.IsKuaFuLogin)
			{
				switch (sceneType)
				{
				case 25:
					HuanYingSiYuanManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case 26:
					TianTiManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case 27:
					YongZheZhanChangManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case 28:
					ElementWarManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case 29:
					SingletonTemplate<MoRiJudgeManager>.Instance().OnLogOut(client);
					goto IL_1E7;
				case 34:
					CopyWolfManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case 35:
					LangHunLingYuManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case 36:
					SingletonTemplate<ZhengBaManager>.Instance().OnLogout(client);
					goto IL_1E7;
				case 38:
					SingletonTemplate<CoupleArenaManager>.Instance().OnLeaveFuBen(client);
					goto IL_1E7;
				case 39:
					KingOfBattleManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case 41:
					KarenBattleManager_MapWest.getInstance().OnLogout(client);
					goto IL_1E7;
				case 42:
					KarenBattleManager_MapEast.getInstance().OnLogout(client);
					goto IL_1E7;
				case 43:
					LingDiCaiJiManager.getInstance().OnLeaveFuBen(client);
					goto IL_1E7;
				case 45:
					BangHuiMatchManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case 47:
					KuaFuLueDuoManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case 49:
					WanMoXiaGuManager.getInstance().OnLogout(client);
					goto IL_1E7;
				case 57:
					ZorkBattleManager.getInstance().OnLogout(client);
					goto IL_1E7;
				}
				if (SingletonTemplate<CopyTeamManager>.Instance().IsKuaFuCopy(client.ClientData.FuBenID))
				{
					SingletonTemplate<CopyTeamManager>.Instance().OnLeaveFuBen(client, sceneType);
				}
				IL_1E7:
				if (!logout)
				{
					this.GotoLastMap(client);
				}
			}
		}

		public void OnLogout(GameClient client)
		{
			switch (client.ClientData.SignUpGameType)
			{
			case 1:
				HuanYingSiYuanClient.getInstance().ChangeRoleState(client.ClientData.RoleID, 0, true);
				break;
			case 2:
				TianTiClient.getInstance().ChangeRoleState(client.ClientData.RoleID, 0, true);
				break;
			}
		}

		public void GotoLastMap(GameClient client)
		{
			if (!client.CheckCheatData.DisableAutoKuaFu)
			{
				client.sendCmd<KuaFuServerLoginData>(14000, new KuaFuServerLoginData
				{
					RoleId = 0
				}, false);
			}
		}

		public void ClearCopyMapClients(CopyMap copyMap)
		{
			try
			{
				List<GameClient> clientsList = copyMap.GetClientsList();
				if (clientsList != null && clientsList.Count > 0)
				{
					for (int i = 0; i < clientsList.Count; i++)
					{
						GameClient gameClient = clientsList[i];
						if (gameClient != null)
						{
							KuaFuManager.getInstance().GotoLastMap(gameClient);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "跨服组队竞技清场调度异常");
			}
		}

		public int GetServerInfoAsyncAge()
		{
			int serverInfoAsyncAge;
			lock (this.RuntimeData.Mutex)
			{
				serverInfoAsyncAge = this.RuntimeData.ServerInfoAsyncAge;
			}
			return serverInfoAsyncAge;
		}

		public bool LocalLogin(string userId)
		{
			return this.LocalServerFlags == 0 || (2 & this.LocalServerFlags) == 0 || true;
		}

		public bool CanKuaFuLogin()
		{
			return GameManager.ServerId >= 9000;
		}

		public void UpdateServerInfoList(List<KuaFuServerInfo> list)
		{
			if (list != null && list.Count > 0)
			{
				lock (this.RuntimeData.Mutex)
				{
					int age = list[0].Age;
					if (age > this.RuntimeData.ServerInfoAsyncAge || this.RuntimeData.ServerInfoAsyncAge - age > 120000)
					{
						this.RuntimeData.ServerInfoAsyncAge = age;
						this.RuntimeData.ServerIdServerInfoDict.Clear();
						foreach (KuaFuServerInfo kuaFuServerInfo in list)
						{
							this.RuntimeData.ServerIdServerInfoDict[kuaFuServerInfo.ServerId] = kuaFuServerInfo;
							if (GameManager.ServerId == kuaFuServerInfo.ServerId || GameManager.KuaFuServerId == kuaFuServerInfo.ServerId)
							{
								this.LocalServerFlags = kuaFuServerInfo.Flags;
							}
						}
					}
				}
			}
		}

		public bool TryGetValue(int serverId, out KuaFuServerInfo kuaFuServerInfo)
		{
			bool result;
			lock (this.RuntimeData.Mutex)
			{
				result = this.RuntimeData.ServerIdServerInfoDict.TryGetValue(serverId, out kuaFuServerInfo);
			}
			return result;
		}

		public bool GetKuaFuDbServerInfo(int serverId, out string dbIp, out int dbPort, out string logIp, out int logPort)
		{
			KuaFuServerInfo kuaFuServerInfo;
			bool result;
			if (KuaFuManager.getInstance().TryGetValue(serverId, out kuaFuServerInfo))
			{
				dbIp = kuaFuServerInfo.DbIp;
				dbPort = kuaFuServerInfo.DbPort;
				logIp = kuaFuServerInfo.DbIp;
				logPort = kuaFuServerInfo.LogDbPort;
				result = true;
			}
			else
			{
				dbIp = null;
				dbPort = 0;
				logIp = null;
				logPort = 0;
				result = false;
			}
			return result;
		}

		public bool GetKuaFuDbServerInfo(int serverId, out string dbIp, out int dbPort, out string logIp, out int logPort, out string gsIp, out int gsPort)
		{
			KuaFuServerInfo kuaFuServerInfo;
			bool result;
			if (KuaFuManager.getInstance().TryGetValue(serverId, out kuaFuServerInfo))
			{
				dbIp = kuaFuServerInfo.DbIp;
				dbPort = kuaFuServerInfo.DbPort;
				logIp = kuaFuServerInfo.DbIp;
				logPort = kuaFuServerInfo.LogDbPort;
				gsIp = kuaFuServerInfo.Ip;
				gsPort = kuaFuServerInfo.Port;
				result = true;
			}
			else
			{
				dbIp = null;
				dbPort = 0;
				logIp = null;
				logPort = 0;
				gsIp = null;
				gsPort = 0;
				result = false;
			}
			return result;
		}

		public void KuaFuSwitchServer(GameClient client)
		{
			long num = TimeUtil.NOW();
			if (num >= client.KuaFuSwitchServerTicks + 5000L)
			{
				client.KuaFuSwitchServerTicks = num;
				GlobalNew.RecordSwitchKuaFuServerLog(client);
				client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
			}
		}

		private void BackGroudThreadProc()
		{
			for (;;)
			{
				try
				{
					this.HandleTransferChatMsg();
				}
				catch
				{
				}
				Thread.Sleep(1800);
			}
		}

		public bool InitGameDbConnection(int serverId, string ip, int port, string logIp, int logPort)
		{
			bool flag = false;
			KuaFuDbConnection kuaFuDbConnection;
			lock (this.DbMutex)
			{
				if (!this.GameDbConnectPoolDict.TryGetValue(serverId, out kuaFuDbConnection))
				{
					kuaFuDbConnection = new KuaFuDbConnection(serverId);
					kuaFuDbConnection.LastHeartTicks = TimeUtil.NOW();
					this.GameDbConnectPoolDict[serverId] = kuaFuDbConnection;
					flag = true;
				}
				else
				{
					kuaFuDbConnection.Pool[0].ChangeIpPort(ip, port);
					kuaFuDbConnection.Pool[1].ChangeIpPort(logIp, logPort);
				}
			}
			bool result;
			if (flag)
			{
				result = (kuaFuDbConnection.Pool[0].Init(3, ip, port, string.Format("server_db_{0}", serverId)) && kuaFuDbConnection.Pool[1].Init(3, logIp, logPort, string.Format("server_log_{0}", serverId)));
			}
			else
			{
				result = (kuaFuDbConnection.Pool[0].Supply() && kuaFuDbConnection.Pool[1].Supply());
			}
			return result;
		}

		public TCPClient PopGameDbClient(int serverId, int poolId)
		{
			try
			{
				KuaFuDbConnection kuaFuDbConnection;
				lock (this.DbMutex)
				{
					if (!this.GameDbConnectPoolDict.TryGetValue(serverId, out kuaFuDbConnection))
					{
						return null;
					}
				}
				return kuaFuDbConnection.Pool[poolId].Pop();
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return null;
		}

		public void PushGameDbClient(int serverId, TCPClient tcpClient, int poolId)
		{
			try
			{
				KuaFuDbConnection kuaFuDbConnection;
				lock (this.DbMutex)
				{
					if (!this.GameDbConnectPoolDict.TryGetValue(serverId, out kuaFuDbConnection))
					{
						return;
					}
					if (tcpClient.LastCmdID == 10025)
					{
						kuaFuDbConnection.LastHeartTicks = TimeUtil.NOW();
					}
				}
				kuaFuDbConnection.Pool[poolId].Push(tcpClient);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		private void HandleTransferChatMsg()
		{
			long num = TimeUtil.NOW();
			if (num - this.LastTransferTicks >= 1000L)
			{
				this.LastTransferTicks = num;
				string cmd = "";
				cmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					GameManager.ServerLineID,
					0,
					this.SendServerHeartCount,
					""
				});
				this.SendServerHeartCount++;
				this.ActiveServerIdList.Clear();
				lock (this.DbMutex)
				{
					foreach (KuaFuDbConnection kuaFuDbConnection in this.GameDbConnectPoolDict.Values)
					{
						if (kuaFuDbConnection.ServerId % 3 == this.SendServerHeartCount % 3)
						{
							this.ActiveServerIdList.Add(kuaFuDbConnection);
						}
					}
				}
				List<int> list = new List<int>();
				foreach (KuaFuDbConnection kuaFuDbConnection2 in this.ActiveServerIdList)
				{
					if (num - kuaFuDbConnection2.LastHeartTicks > 300000L)
					{
						lock (this.DbMutex)
						{
							this.GameDbConnectPoolDict.Remove(kuaFuDbConnection2.ServerId);
							kuaFuDbConnection2.Dispose();
						}
					}
					else if (kuaFuDbConnection2.Pool[0].ErrCount == 0)
					{
						list.Add(kuaFuDbConnection2.ServerId);
					}
				}
				foreach (int num2 in list)
				{
					try
					{
						List<string> list2 = Global.sendToDB<List<string>, string>(10018, cmd, num2);
						if (list2 != null && list2.Count > 0)
						{
							for (int i = 0; i < list2.Count; i++)
							{
								GameManager.ClientMgr.TransferChatMsg(list2[i]);
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteExceptionUseCache(ex.ToString());
						lock (this.DbMutex)
						{
							this.GameDbConnectPoolDict.Remove(num2);
						}
					}
				}
			}
		}

		public bool IsKuaFuMap(int mapCode)
		{
			switch (Global.GetMapSceneType(mapCode))
			{
			case 25:
			case 26:
			case 27:
			case 28:
			case 29:
			case 31:
			case 32:
			case 34:
			case 35:
			case 36:
			case 37:
			case 38:
			case 39:
			case 40:
			case 41:
			case 42:
			case 43:
			case 45:
			case 47:
			case 48:
			case 49:
			case 52:
			case 53:
			case 54:
			case 55:
			case 56:
			case 57:
			case 59:
				return true;
			}
			return false;
		}

		public int SingUpMaxSeconds { get; private set; }

		public int AutoCancelMaxSeconds { get; private set; }

		public int CannotJoinCopyMaxSeconds { get; private set; }

		public void InitCopyTime()
		{
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("KuaFuFuBenTime", ',');
			if (paramValueIntArrayByName != null && paramValueIntArrayByName.Length >= 3)
			{
				this.SingUpMaxSeconds = paramValueIntArrayByName[0];
				this.AutoCancelMaxSeconds = paramValueIntArrayByName[1];
				this.CannotJoinCopyMaxSeconds = paramValueIntArrayByName[2];
			}
		}

		public void SetCannotJoinKuaFu_UseAutoEndTicks(GameClient client)
		{
			if (this.CannotJoinCopyMaxSeconds > 0)
			{
				this.SetCannotJoinKuaFuCopyEndTicks(client, TimeUtil.NowDateTime().AddSeconds((double)this.CannotJoinCopyMaxSeconds).Ticks);
			}
		}

		public void SetCannotJoinKuaFuCopyEndTicks(GameClient client, long endTicks)
		{
			if (client != null)
			{
				Global.SaveRoleParamsInt64ValueToDB(client, "CannotJoinKFCopyEndTicks", endTicks, true);
			}
		}

		public bool IsInCannotJoinKuaFuCopyTime(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = true;
			}
			else
			{
				long roleParamsInt64FromDB = Global.GetRoleParamsInt64FromDB(client, "CannotJoinKFCopyEndTicks");
				result = (TimeUtil.NowDateTime().Ticks < roleParamsInt64FromDB);
			}
			return result;
		}

		public void NotifyClientCannotJoinKuaFuCopyEndTicks(GameClient client)
		{
			long roleParamsInt64FromDB = Global.GetRoleParamsInt64FromDB(client, "CannotJoinKFCopyEndTicks");
		}

		public string GetZoneName(int serverID)
		{
			string strServerName;
			lock (this.ZoneID2ZoneNameDict)
			{
				if (this.ZoneID2ZoneNameDict.TryGetValue(serverID, out strServerName))
				{
					return strServerName;
				}
			}
			KuaFuServerInfo kuaFuServerInfo;
			if (KuaFuManager.getInstance().TryGetValue(serverID, out kuaFuServerInfo))
			{
				strServerName = kuaFuServerInfo.strServerName;
				lock (this.ZoneID2ZoneNameDict)
				{
					this.ZoneID2ZoneNameDict[serverID] = strServerName;
				}
			}
			return strServerName;
		}

		public bool ClientCmdCheckFaild(int cmdID, GameClient client, ref int roleID)
		{
			bool result;
			if (null == client)
			{
				LogManager.WriteLog(2, string.Format("ClientCmdCheckFaild,cmd={0},client=null", cmdID), null, true);
				result = true;
			}
			else if (client.ClientSocket.IsKuaFuLogin)
			{
				if (client.ClientSocket.ClientKuaFuServerLoginData.GameType == 32)
				{
					if (!Data.KuaFuWorldCmdEnabled(cmdID))
					{
						LogManager.WriteLog(1000, "KuaFuWorldCmd " + (TCPGameServerCmds)cmdID, null, true);
						return true;
					}
				}
				if (client.ClientData.RoleID != roleID)
				{
					LogManager.WriteLog(2, string.Format("RoleIDCheckFaild,cmd={0},NeedRoleID={1},CmdRoleID={2}", (TCPGameServerCmds)cmdID, client.ClientData.RoleID, roleID), null, true);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			else if (client.ClientData.RoleID != roleID)
			{
				LogManager.WriteLog(2, string.Format("RoleIDCheckFaild,cmd={0},NeedRoleID={1},CmdRoleID={2}", (TCPGameServerCmds)cmdID, client.ClientData.RoleID, roleID), null, true);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private ICoreInterface CoreInterface = null;

		private static KuaFuManager instance = new KuaFuManager();

		public static bool KuaFuWorldKuaFuGameServer = false;

		public KuaFuDataData RuntimeData = new KuaFuDataData();

		private static int[] KuaFuWorldGameTypes = new int[]
		{
			32
		};

		private int LocalServerFlags = 0;

		private object DbMutex = new object();

		private Dictionary<int, KuaFuDbConnection> GameDbConnectPoolDict = new Dictionary<int, KuaFuDbConnection>();

		public long LastTransferTicks = 0L;

		public int SendServerHeartCount = 0;

		private List<KuaFuDbConnection> ActiveServerIdList = new List<KuaFuDbConnection>();

		private Dictionary<int, string> ZoneID2ZoneNameDict = new Dictionary<int, string>();
	}
}
