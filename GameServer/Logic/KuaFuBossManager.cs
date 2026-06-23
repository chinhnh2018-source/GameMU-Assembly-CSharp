using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class KuaFuBossManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		public static KuaFuBossManager getInstance()
		{
			return KuaFuBossManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("KuaFuBossManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1120, 1, 1, KuaFuBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1121, 1, 1, KuaFuBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1123, 1, 1, KuaFuBossManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10001, 31, KuaFuBossManager.getInstance());
			GlobalEventSource.getInstance().registerListener(56, KuaFuBossManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10001, 31, KuaFuBossManager.getInstance());
			GlobalEventSource.getInstance().removeListener(56, KuaFuBossManager.getInstance());
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 1120:
				return this.ProcessKuaFuBossJoinCmd(client, nID, bytes, cmdParams);
			case 1121:
				return this.ProcessKuaFuBossEnterCmd(client, nID, bytes, cmdParams);
			case 1123:
				return this.ProcessGetKuaFuBossStateCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 56)
			{
				KillMonsterEventObject killMonsterEventObject = eventObject as KillMonsterEventObject;
				if (null != killMonsterEventObject)
				{
					this.OnKillMonster(killMonsterEventObject.getAttacker(), killMonsterEventObject.getMonster());
				}
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num == 10001)
			{
				KuaFuNotifyEnterGameEvent kuaFuNotifyEnterGameEvent = eventObject as KuaFuNotifyEnterGameEvent;
				if (null != kuaFuNotifyEnterGameEvent)
				{
					KuaFuServerLoginData kuaFuServerLoginData = kuaFuNotifyEnterGameEvent.Arg as KuaFuServerLoginData;
					if (null != kuaFuServerLoginData)
					{
						lock (this.RuntimeData.Mutex)
						{
							this.RuntimeData.RoleIdKuaFuLoginDataDict[kuaFuServerLoginData.RoleId] = kuaFuServerLoginData;
							LogManager.WriteLog(2, string.Format("通知角色ID={0}拥有进入跨服Boss资格,跨服GameID={1}", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
						}
					}
					eventObject.Handled = true;
				}
			}
		}

		public bool InitConfig()
		{
			bool result = true;
			string text = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.MapBirthPointDict.Clear();
					text = "Config/ThroughServiceBossRebirth.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						KuaFuBossBirthPoint kuaFuBossBirthPoint = new KuaFuBossBirthPoint();
						kuaFuBossBirthPoint.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						kuaFuBossBirthPoint.PosX = (int)Global.GetSafeAttributeLong(xml, "PosX");
						kuaFuBossBirthPoint.PosY = (int)Global.GetSafeAttributeLong(xml, "PosY");
						kuaFuBossBirthPoint.BirthRadius = (int)Global.GetSafeAttributeLong(xml, "BirthRadius");
						this.RuntimeData.MapBirthPointDict[kuaFuBossBirthPoint.ID] = kuaFuBossBirthPoint;
					}
					this.RuntimeData.SceneDataDict.Clear();
					this.RuntimeData.LevelRangeSceneIdDict.Clear();
					this.RuntimeData.SceneDynMonsterDict.Clear();
					text = "Config/ThroughServiceBoss.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						KuaFuBossSceneInfo kuaFuBossSceneInfo = new KuaFuBossSceneInfo();
						int num = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						int num2 = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						kuaFuBossSceneInfo.Id = num;
						kuaFuBossSceneInfo.MapCode = num2;
						kuaFuBossSceneInfo.MinLevel = (int)Global.GetSafeAttributeLong(xml, "MinLevel");
						kuaFuBossSceneInfo.MaxLevel = (int)Global.GetSafeAttributeLong(xml, "MaxLevel");
						kuaFuBossSceneInfo.MinZhuanSheng = (int)Global.GetSafeAttributeLong(xml, "MinZhuanSheng");
						kuaFuBossSceneInfo.MaxZhuanSheng = (int)Global.GetSafeAttributeLong(xml, "MaxZhuanSheng");
						kuaFuBossSceneInfo.PrepareSecs = (int)Global.GetSafeAttributeLong(xml, "PrepareSecs");
						kuaFuBossSceneInfo.WaitingEnterSecs = (int)Global.GetSafeAttributeLong(xml, "WaitingEnterSecs");
						kuaFuBossSceneInfo.FightingSecs = (int)Global.GetSafeAttributeLong(xml, "FightingSecs");
						kuaFuBossSceneInfo.ClearRolesSecs = (int)Global.GetSafeAttributeLong(xml, "ClearRolesSecs");
						ConfigParser.ParseStrInt2(Global.GetSafeAttributeStr(xml, "ApplyTime"), ref kuaFuBossSceneInfo.SignUpStartSecs, ref kuaFuBossSceneInfo.SignUpEndSecs, ',');
						kuaFuBossSceneInfo.SignUpStartSecs += kuaFuBossSceneInfo.SignUpEndSecs;
						if (!ConfigParser.ParserTimeRangeListWithDay(kuaFuBossSceneInfo.TimePoints, Global.GetSafeAttributeStr(xml, "TimePoints"), true, '|', '-', ','))
						{
							result = false;
							LogManager.WriteLog(1000, string.Format("读取{0}时间配置(TimePoints)出错", text), null, true);
						}
						for (int i = 0; i < kuaFuBossSceneInfo.TimePoints.Count; i++)
						{
							TimeSpan timeSpan = new TimeSpan(kuaFuBossSceneInfo.TimePoints[i].Hours, kuaFuBossSceneInfo.TimePoints[i].Minutes, kuaFuBossSceneInfo.TimePoints[i].Seconds);
							kuaFuBossSceneInfo.SecondsOfDay.Add(timeSpan.TotalSeconds);
						}
						GameMap gameMap = null;
						if (!GameManager.MapMgr.DictMaps.TryGetValue(num2, out gameMap))
						{
							result = false;
							LogManager.WriteLog(1000, string.Format("地图配置中缺少{0}所需的地图:{1}", text, num2), null, true);
						}
						RangeKey key = new RangeKey(Global.GetUnionLevel(kuaFuBossSceneInfo.MinZhuanSheng, kuaFuBossSceneInfo.MinLevel, false), Global.GetUnionLevel(kuaFuBossSceneInfo.MaxZhuanSheng, kuaFuBossSceneInfo.MaxLevel, false), null);
						this.RuntimeData.LevelRangeSceneIdDict[key] = kuaFuBossSceneInfo;
						this.RuntimeData.SceneDataDict[num] = kuaFuBossSceneInfo;
					}
					text = "Config/ThroughServiceBossMonster.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						BattleDynamicMonsterItem battleDynamicMonsterItem = new BattleDynamicMonsterItem();
						battleDynamicMonsterItem.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
						battleDynamicMonsterItem.MapCode = (int)Global.GetSafeAttributeLong(xml, "CodeID");
						battleDynamicMonsterItem.MonsterID = (int)Global.GetSafeAttributeLong(xml, "MonsterID");
						battleDynamicMonsterItem.PosX = (int)Global.GetSafeAttributeLong(xml, "X");
						battleDynamicMonsterItem.PosY = (int)Global.GetSafeAttributeLong(xml, "Y");
						battleDynamicMonsterItem.DelayBirthMs = (int)Global.GetSafeAttributeLong(xml, "Time");
						battleDynamicMonsterItem.PursuitRadius = (int)Global.GetSafeAttributeLong(xml, "PursuitRadius");
						battleDynamicMonsterItem.Num = (int)Global.GetSafeAttributeLong(xml, "Num");
						battleDynamicMonsterItem.Radius = (int)Global.GetSafeAttributeLong(xml, "Radius");
						List<BattleDynamicMonsterItem> list = null;
						if (!this.RuntimeData.SceneDynMonsterDict.TryGetValue(battleDynamicMonsterItem.MapCode, out list))
						{
							list = new List<BattleDynamicMonsterItem>();
							this.RuntimeData.SceneDynMonsterDict[battleDynamicMonsterItem.MapCode] = list;
						}
						list.Add(battleDynamicMonsterItem);
					}
				}
				catch (Exception ex)
				{
					result = false;
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
				}
			}
			return result;
		}

		private void TimerProc(object sender, EventArgs e)
		{
			bool flag = false;
			bool flag2 = false;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				bool flag4 = false;
				KuaFuBossSceneInfo kuaFuBossSceneInfo = this.RuntimeData.SceneDataDict.Values.FirstOrDefault<KuaFuBossSceneInfo>();
				for (int i = 0; i < kuaFuBossSceneInfo.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)kuaFuBossSceneInfo.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= kuaFuBossSceneInfo.SecondsOfDay[i] - (double)kuaFuBossSceneInfo.SignUpStartSecs && dateTime.TimeOfDay.TotalSeconds <= kuaFuBossSceneInfo.SecondsOfDay[i + 1])
					{
						double num = kuaFuBossSceneInfo.SecondsOfDay[i] - dateTime.TimeOfDay.TotalSeconds;
						flag4 = true;
						if (!this.RuntimeData.PrepareGame)
						{
							if (num > 0.0 && num < (double)(kuaFuBossSceneInfo.SignUpEndSecs / 2))
							{
								LogManager.WriteLog(2, "报名截止5分钟时间过半,通知跨服中心开始分配所有报名玩家的活动场次", null, true);
								this.RuntimeData.PrepareGame = true;
								flag = true;
								break;
							}
						}
						else if (num < 0.0)
						{
							LogManager.WriteLog(2, "报名截止状态结束,可以通知已分配到场次的玩家进入游戏了", null, true);
							flag2 = true;
							this.RuntimeData.PrepareGame = false;
							break;
						}
					}
				}
				if (!flag4)
				{
					if (this.RuntimeData.RoleIdKuaFuLoginDataDict.Count > 0)
					{
						this.RuntimeData.RoleIdKuaFuLoginDataDict.Clear();
					}
					if (this.RuntimeData.RoleId2JoinGroup.Count > 0)
					{
						this.RuntimeData.RoleId2JoinGroup.Clear();
					}
				}
			}
			if (flag)
			{
				LogManager.WriteLog(2, "通知跨服中心开始分配所有报名玩家的活动场次", null, true);
				string cmd = string.Format("{0} {1} {2}", "GameState", 2, 6);
				YongZheZhanChangClient.getInstance().ExecuteCommand(cmd);
			}
			if (flag2)
			{
				lock (this.RuntimeData.Mutex)
				{
					foreach (KuaFuServerLoginData kuaFuServerLoginData in this.RuntimeData.RoleIdKuaFuLoginDataDict.Values)
					{
						this.RuntimeData.NotifyRoleEnterDict.Add(kuaFuServerLoginData.RoleId, kuaFuServerLoginData);
					}
				}
			}
			List<KuaFuServerLoginData> list = null;
			lock (this.RuntimeData.Mutex)
			{
				int count = this.RuntimeData.NotifyRoleEnterDict.Count;
				if (count > 0)
				{
					list = new List<KuaFuServerLoginData>();
					KuaFuServerLoginData kuaFuServerLoginData = this.RuntimeData.NotifyRoleEnterDict.First<KeyValuePair<int, KuaFuServerLoginData>>().Value;
					foreach (KeyValuePair<int, KuaFuServerLoginData> keyValuePair in this.RuntimeData.NotifyRoleEnterDict)
					{
						if (keyValuePair.Key % 15 == kuaFuServerLoginData.RoleId % 15)
						{
							list.Add(keyValuePair.Value);
						}
					}
					foreach (KuaFuServerLoginData kuaFuServerLoginData2 in list)
					{
						this.RuntimeData.NotifyRoleEnterDict.Remove(kuaFuServerLoginData2.RoleId);
					}
				}
			}
			if (null != list)
			{
				foreach (KuaFuServerLoginData kuaFuServerLoginData in list)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(kuaFuServerLoginData.RoleId);
					if (null != gameClient)
					{
						gameClient.sendCmd<int>(1121, 1, false);
					}
				}
			}
		}

		public bool ProcessKuaFuBossJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KuaFuBossSceneInfo kuaFuBossSceneInfo = null;
				KuaFuBossGameStates kuaFuBossGameStates = KuaFuBossGameStates.None;
				int num;
				if (!this.CheckMap(client))
				{
					num = -21;
				}
				else
				{
					num = this.CheckCondition(client, ref kuaFuBossSceneInfo, ref kuaFuBossGameStates);
				}
				if (kuaFuBossGameStates != KuaFuBossGameStates.SignUp)
				{
					num = -2001;
				}
				else if (this.RuntimeData.RoleId2JoinGroup.ContainsKey(client.ClientData.RoleID))
				{
					num = -12;
				}
				if (num >= 0)
				{
					int id = kuaFuBossSceneInfo.Id;
					num = YongZheZhanChangClient.getInstance().YongZheZhanChangSignUp(client.strUserID, client.ClientData.RoleID, client.ClientData.ZoneID, 6, id, client.ClientData.CombatForce);
					if (num > 0)
					{
						this.RuntimeData.RoleId2JoinGroup[client.ClientData.RoleID] = id;
						client.ClientData.SignUpGameType = 6;
					}
				}
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool CheckMap(GameClient client)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return mapSceneType == 0;
		}

		private int CheckCondition(GameClient client, ref KuaFuBossSceneInfo sceneItem, ref KuaFuBossGameStates state)
		{
			int result = 0;
			sceneItem = null;
			if (!this.IsGongNengOpened(client, true))
			{
				result = -13;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.LevelRangeSceneIdDict.TryGetValue(new RangeKey(Global.GetUnionLevel(client, false)), out sceneItem))
					{
						return -12;
					}
				}
				result = -2001;
				DateTime dateTime = TimeUtil.NowDateTime();
				lock (this.RuntimeData.Mutex)
				{
					for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
					{
						if (dateTime.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpStartSecs && dateTime.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
						{
							if (dateTime.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpStartSecs)
							{
								state = KuaFuBossGameStates.None;
								result = -2001;
							}
							else if (dateTime.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpEndSecs)
							{
								state = KuaFuBossGameStates.SignUp;
								result = 1;
							}
							else if (dateTime.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i])
							{
								state = KuaFuBossGameStates.Wait;
								result = 1;
							}
							else if (dateTime.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1])
							{
								state = KuaFuBossGameStates.Start;
								result = 1;
							}
							else
							{
								state = KuaFuBossGameStates.None;
								result = -2001;
							}
							break;
						}
					}
				}
			}
			return result;
		}

		private TimeSpan GetStartTime(int sceneId)
		{
			KuaFuBossSceneInfo kuaFuBossSceneInfo = null;
			TimeSpan timeSpan = TimeSpan.MinValue;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.SceneDataDict.TryGetValue(sceneId, out kuaFuBossSceneInfo))
				{
					goto IL_153;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < kuaFuBossSceneInfo.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)kuaFuBossSceneInfo.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= kuaFuBossSceneInfo.SecondsOfDay[i] - (double)kuaFuBossSceneInfo.SignUpStartSecs && dateTime.TimeOfDay.TotalSeconds <= kuaFuBossSceneInfo.SecondsOfDay[i + 1])
					{
						timeSpan = TimeSpan.FromSeconds(kuaFuBossSceneInfo.SecondsOfDay[i]);
						break;
					}
				}
			}
			IL_153:
			if (timeSpan < TimeSpan.Zero)
			{
				timeSpan = dateTime.TimeOfDay;
			}
			return timeSpan;
		}

		public bool ProcessGetKuaFuBossStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KuaFuBossSceneInfo kuaFuBossSceneInfo = null;
				KuaFuBossGameStates kuaFuBossGameStates = KuaFuBossGameStates.None;
				int cmdData = 0;
				int num = 0;
				this.RuntimeData.RoleId2JoinGroup.TryGetValue(client.ClientData.RoleID, out num);
				this.CheckCondition(client, ref kuaFuBossSceneInfo, ref kuaFuBossGameStates);
				if (num > 0)
				{
					if (kuaFuBossGameStates >= KuaFuBossGameStates.SignUp && kuaFuBossGameStates <= KuaFuBossGameStates.Wait)
					{
						int kuaFuRoleState = YongZheZhanChangClient.getInstance().GetKuaFuRoleState(client.ClientData.RoleID);
						if (kuaFuRoleState >= 1)
						{
							cmdData = 2;
						}
						else
						{
							cmdData = 5;
						}
					}
					else if (kuaFuBossGameStates == KuaFuBossGameStates.Start)
					{
						if (this.RuntimeData.RoleIdKuaFuLoginDataDict.ContainsKey(client.ClientData.RoleID))
						{
							cmdData = 3;
						}
					}
				}
				else if (kuaFuBossGameStates == KuaFuBossGameStates.SignUp)
				{
					cmdData = 1;
				}
				else if (kuaFuBossGameStates == KuaFuBossGameStates.Wait || kuaFuBossGameStates == KuaFuBossGameStates.Start)
				{
					cmdData = 5;
				}
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessKuaFuBossEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KuaFuBossSceneInfo kuaFuBossSceneInfo = null;
				KuaFuBossGameStates kuaFuBossGameStates = KuaFuBossGameStates.None;
				int num = 0;
				if (!this.CheckMap(client))
				{
					num = -21;
				}
				else
				{
					num = this.CheckCondition(client, ref kuaFuBossSceneInfo, ref kuaFuBossGameStates);
				}
				if (kuaFuBossGameStates == KuaFuBossGameStates.Start)
				{
					KuaFuServerLoginData kuaFuServerLoginData = null;
					lock (this.RuntimeData.Mutex)
					{
						if (this.RuntimeData.RoleIdKuaFuLoginDataDict.TryGetValue(client.ClientData.RoleID, out kuaFuServerLoginData))
						{
							KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
							if (null != clientKuaFuServerLoginData)
							{
								clientKuaFuServerLoginData.RoleId = kuaFuServerLoginData.RoleId;
								clientKuaFuServerLoginData.GameId = kuaFuServerLoginData.GameId;
								clientKuaFuServerLoginData.GameType = kuaFuServerLoginData.GameType;
								clientKuaFuServerLoginData.EndTicks = kuaFuServerLoginData.EndTicks;
								clientKuaFuServerLoginData.ServerId = kuaFuServerLoginData.ServerId;
								clientKuaFuServerLoginData.ServerIp = kuaFuServerLoginData.ServerIp;
								clientKuaFuServerLoginData.ServerPort = kuaFuServerLoginData.ServerPort;
								clientKuaFuServerLoginData.FuBenSeqId = kuaFuServerLoginData.FuBenSeqId;
							}
						}
						else
						{
							num = -11000;
						}
					}
					if (num >= 0)
					{
						num = YongZheZhanChangClient.getInstance().ChangeRoleState(client.ClientData.RoleID, 4, false);
						if (num >= 0)
						{
							GlobalNew.RecordSwitchKuaFuServerLog(client);
							client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
						}
						else
						{
							Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
						}
					}
				}
				else
				{
					num = -2001;
				}
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			int result;
			lock (this.RuntimeData.Mutex)
			{
				KuaFuBossBirthPoint kuaFuBossBirthPoint = null;
				int randomNumber = Global.GetRandomNumber(1, this.RuntimeData.MapBirthPointDict.Count);
				if (!this.RuntimeData.MapBirthPointDict.TryGetValue(randomNumber, out kuaFuBossBirthPoint))
				{
					kuaFuBossBirthPoint = this.RuntimeData.MapBirthPointDict.First<KeyValuePair<int, KuaFuBossBirthPoint>>().Value;
				}
				posX = kuaFuBossBirthPoint.PosX;
				posY = kuaFuBossBirthPoint.PosY;
				result = randomNumber;
			}
			return result;
		}

		public bool OnInitGame(GameClient client)
		{
			KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			YongZheZhanChangFuBenData yongZheZhanChangFuBenData;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.FuBenItemData.TryGetValue((int)clientKuaFuServerLoginData.GameId, out yongZheZhanChangFuBenData))
				{
					yongZheZhanChangFuBenData = null;
				}
				else if (yongZheZhanChangFuBenData.State >= 3)
				{
					return false;
				}
			}
			if (null == yongZheZhanChangFuBenData)
			{
				YongZheZhanChangFuBenData kuaFuFuBenData = YongZheZhanChangClient.getInstance().GetKuaFuFuBenData((int)clientKuaFuServerLoginData.GameId);
				if (kuaFuFuBenData == null || kuaFuFuBenData.State == 3)
				{
					LogManager.WriteLog(2, ("获取不到有效的副本数据," + kuaFuFuBenData == null) ? "fuBenData == null" : "fuBenData.State == GameFuBenState.End", null, true);
					return false;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.FuBenItemData.TryGetValue((int)clientKuaFuServerLoginData.GameId, out yongZheZhanChangFuBenData))
					{
						yongZheZhanChangFuBenData = kuaFuFuBenData;
						yongZheZhanChangFuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						this.RuntimeData.FuBenItemData[yongZheZhanChangFuBenData.GameId] = yongZheZhanChangFuBenData;
					}
				}
			}
			KuaFuFuBenRoleData kuaFuFuBenRoleData;
			bool result;
			if (!yongZheZhanChangFuBenData.RoleDict.TryGetValue(client.ClientData.RoleID, out kuaFuFuBenRoleData))
			{
				result = false;
			}
			else
			{
				int posX;
				int posY;
				this.GetBirthPoint(client, out posX, out posY);
				lock (this.RuntimeData.Mutex)
				{
					clientKuaFuServerLoginData.FuBenSeqId = yongZheZhanChangFuBenData.SequenceId;
					KuaFuBossSceneInfo kuaFuBossSceneInfo;
					if (!this.RuntimeData.SceneDataDict.TryGetValue(yongZheZhanChangFuBenData.GroupIndex, out kuaFuBossSceneInfo))
					{
						return false;
					}
					client.ClientData.MapCode = kuaFuBossSceneInfo.MapCode;
				}
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				client.ClientData.FuBenSeqID = clientKuaFuServerLoginData.FuBenSeqId;
				result = true;
			}
			return result;
		}

		public bool ClientRelive(GameClient client)
		{
			int posX;
			int posY;
			this.GetBirthPoint(client, out posX, out posY);
			client.ClientData.CurrentLifeV = client.ClientData.LifeV;
			client.ClientData.CurrentMagicV = client.ClientData.MagicV;
			client.ClientData.MoveAndActionNum = 0;
			GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, posX, posY, -1);
			Global.ClientRealive(client, posX, posY, -1);
			return true;
		}

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(7) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("KuaFuBoss") && GlobalNew.IsGongNengOpened(client, 65, hint);
		}

		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 31)
			{
				int fuBenSeqID = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				int roleID = client.ClientData.RoleID;
				int num = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
				DateTime dateTime = TimeUtil.NowDateTime();
				KuaFuBossScene kuaFuBossScene = null;
				lock (this.RuntimeData.Mutex)
				{
					if (!this.SceneDict.TryGetValue(fuBenSeqID, out kuaFuBossScene))
					{
						KuaFuBossSceneInfo kuaFuBossSceneInfo = null;
						YongZheZhanChangFuBenData yongZheZhanChangFuBenData;
						if (!this.RuntimeData.FuBenItemData.TryGetValue(num, out yongZheZhanChangFuBenData))
						{
							LogManager.WriteLog(2, "跨服Boss没有为副本找到对应的跨服副本数据,GameID:" + num, null, true);
						}
						if (!this.RuntimeData.SceneDataDict.TryGetValue(yongZheZhanChangFuBenData.GroupIndex, out kuaFuBossSceneInfo))
						{
							LogManager.WriteLog(2, "跨服Boss没有为副本找到对应的档位数据,ID:" + yongZheZhanChangFuBenData.GroupIndex, null, true);
						}
						kuaFuBossScene = new KuaFuBossScene();
						kuaFuBossScene.CopyMap = copyMap;
						kuaFuBossScene.CleanAllInfo();
						kuaFuBossScene.GameId = num;
						kuaFuBossScene.m_nMapCode = mapCode;
						kuaFuBossScene.CopyMapId = copyMap.CopyMapID;
						kuaFuBossScene.FuBenSeqId = fuBenSeqID;
						kuaFuBossScene.m_nPlarerCount = 1;
						kuaFuBossScene.SceneInfo = kuaFuBossSceneInfo;
						DateTime dateTime2 = dateTime.Date.Add(this.GetStartTime(kuaFuBossSceneInfo.Id));
						kuaFuBossScene.StartTimeTicks = dateTime2.Ticks / 10000L;
						kuaFuBossScene.GameStatisticalData.GameId = num;
						this.SceneDict[fuBenSeqID] = kuaFuBossScene;
						List<BattleDynamicMonsterItem> dynMonsterList;
						if (this.RuntimeData.SceneDynMonsterDict.TryGetValue(mapCode, out dynMonsterList))
						{
							kuaFuBossScene.DynMonsterList = dynMonsterList;
						}
					}
					else
					{
						kuaFuBossScene.m_nPlarerCount++;
					}
					copyMap.IsKuaFuCopy = true;
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(kuaFuBossScene.SceneInfo.TotalSecs * 1000));
				}
				GameMap gameMap = null;
				if (GameManager.MapMgr.DictMaps.TryGetValue(copyMap.MapCode, out gameMap))
				{
					kuaFuBossScene.MapGridWidth = gameMap.MapGridWidth;
					kuaFuBossScene.MapGridHeight = gameMap.MapGridHeight;
				}
				YongZheZhanChangClient.getInstance().GameFuBenRoleChangeState(roleID, 5, 0, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 31)
			{
				lock (this.RuntimeData.Mutex)
				{
					KuaFuBossScene kuaFuBossScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out kuaFuBossScene);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private void NotifySceneData(KuaFuBossScene scene)
		{
			if (null != scene)
			{
				GameManager.ClientMgr.BroadSpecialCopyMapMessage<KuaFuBossSceneStateData>(1122, scene.SceneStateData, scene.CopyMap);
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				KuaFuBossScene kuaFuBossScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kuaFuBossScene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, kuaFuBossScene.StateTimeData, false);
					}
					if (sideScore)
					{
						client.sendCmd<KuaFuBossSceneStateData>(1122, kuaFuBossScene.SceneStateData, false);
					}
				}
			}
		}

		public void CheckCreateDynamicMonster(KuaFuBossScene scene, long nowMs)
		{
			lock (this.RuntimeData.Mutex)
			{
				List<BattleDynamicMonsterItem> dynMonsterList = scene.DynMonsterList;
				if (dynMonsterList != null)
				{
					foreach (BattleDynamicMonsterItem battleDynamicMonsterItem in dynMonsterList)
					{
						if (!scene.DynMonsterSet.Contains(battleDynamicMonsterItem.Id))
						{
							if (nowMs - scene.m_lBeginTime >= (long)battleDynamicMonsterItem.DelayBirthMs)
							{
								Monster monster = GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, battleDynamicMonsterItem.MonsterID, scene.CopyMap.CopyMapID, battleDynamicMonsterItem.Num, battleDynamicMonsterItem.PosX / scene.MapGridWidth, battleDynamicMonsterItem.PosY / scene.MapGridHeight, battleDynamicMonsterItem.Radius, battleDynamicMonsterItem.PursuitRadius, 31, null, null);
								scene.DynMonsterSet.Add(battleDynamicMonsterItem.Id);
								if (null != monster)
								{
									if (monster.MonsterType == 401)
									{
										scene.SceneStateData.TotalBossNum += battleDynamicMonsterItem.Num;
										scene.SceneStateData.BossNum += battleDynamicMonsterItem.Num;
									}
									else
									{
										scene.SceneStateData.TotalNormalNum += battleDynamicMonsterItem.Num;
										scene.SceneStateData.MonsterNum += battleDynamicMonsterItem.Num;
									}
								}
							}
						}
					}
				}
			}
		}

		public void OnKillMonster(GameClient client, Monster monster)
		{
			if (monster.ManagerType == 31)
			{
				KuaFuBossScene kuaFuBossScene;
				lock (this.RuntimeData.Mutex)
				{
					if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kuaFuBossScene))
					{
						if (kuaFuBossScene.m_eStatus != 2)
						{
							return;
						}
						kuaFuBossScene.GameStatisticalData.MonsterDieTimeList.Add(monster.MonsterInfo.ExtensionID);
						kuaFuBossScene.GameStatisticalData.MonsterDieTimeList.Add(kuaFuBossScene.ElapsedSeconds);
						if (monster.MonsterType == 401)
						{
							kuaFuBossScene.SceneStateData.BossNum = Math.Max(0, kuaFuBossScene.SceneStateData.BossNum - 1);
							if (null != client)
							{
								string msg = string.Format(GLang.GetLang(401, new object[0]), Global.FormatRoleName4(client), monster.MonsterInfo.VSName);
								GameManager.ClientMgr.BroadSpecialCopyMapMsg(kuaFuBossScene.CopyMap, msg, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
							}
						}
						else
						{
							kuaFuBossScene.SceneStateData.MonsterNum = Math.Max(0, kuaFuBossScene.SceneStateData.MonsterNum - 1);
						}
					}
				}
				this.NotifySceneData(kuaFuBossScene);
			}
		}

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= KuaFuBossManager.NextHeartBeatTicks)
			{
				KuaFuBossManager.NextHeartBeatTicks = num + 1020L;
				foreach (KuaFuBossScene kuaFuBossScene in this.SceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int fuBenSeqId = kuaFuBossScene.FuBenSeqId;
						int copyMapId = kuaFuBossScene.CopyMapId;
						int nMapCode = kuaFuBossScene.m_nMapCode;
						if (fuBenSeqId >= 0 && copyMapId >= 0 && nMapCode >= 0)
						{
							CopyMap copyMap = kuaFuBossScene.CopyMap;
							DateTime time = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							if (kuaFuBossScene.m_eStatus == 0)
							{
								if (num2 >= kuaFuBossScene.StartTimeTicks)
								{
									kuaFuBossScene.m_lPrepareTime = kuaFuBossScene.StartTimeTicks;
									kuaFuBossScene.m_lBeginTime = kuaFuBossScene.m_lPrepareTime + (long)(kuaFuBossScene.SceneInfo.PrepareSecs * 1000);
									kuaFuBossScene.m_eStatus = 1;
									kuaFuBossScene.StateTimeData.GameType = 6;
									kuaFuBossScene.StateTimeData.State = kuaFuBossScene.m_eStatus;
									kuaFuBossScene.StateTimeData.EndTicks = kuaFuBossScene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, kuaFuBossScene.StateTimeData, kuaFuBossScene.CopyMap);
								}
							}
							else if (kuaFuBossScene.m_eStatus == 1)
							{
								if (num2 >= kuaFuBossScene.m_lBeginTime)
								{
									kuaFuBossScene.m_eStatus = 2;
									kuaFuBossScene.m_lEndTime = kuaFuBossScene.m_lBeginTime + (long)(kuaFuBossScene.SceneInfo.FightingSecs * 1000);
									kuaFuBossScene.StateTimeData.GameType = 6;
									kuaFuBossScene.StateTimeData.State = kuaFuBossScene.m_eStatus;
									kuaFuBossScene.StateTimeData.EndTicks = kuaFuBossScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, kuaFuBossScene.StateTimeData, kuaFuBossScene.CopyMap);
								}
							}
							else if (kuaFuBossScene.m_eStatus == 2)
							{
								if (num2 >= kuaFuBossScene.m_lEndTime)
								{
									kuaFuBossScene.m_eStatus = 3;
									kuaFuBossScene.m_lLeaveTime = kuaFuBossScene.m_lEndTime + (long)(kuaFuBossScene.SceneInfo.ClearRolesSecs * 1000);
									kuaFuBossScene.StateTimeData.GameType = 6;
									kuaFuBossScene.StateTimeData.State = 5;
									kuaFuBossScene.StateTimeData.EndTicks = kuaFuBossScene.m_lLeaveTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, kuaFuBossScene.StateTimeData, kuaFuBossScene.CopyMap);
									this.NotifySceneData(kuaFuBossScene);
								}
								else
								{
									kuaFuBossScene.ElapsedSeconds = (int)Math.Min((num - kuaFuBossScene.m_lBeginTime) / 1000L, (long)kuaFuBossScene.SceneInfo.TotalSecs);
									this.CheckCreateDynamicMonster(kuaFuBossScene, num2);
									if (num > kuaFuBossScene.NextNotifySceneStateDataTicks)
									{
										kuaFuBossScene.NextNotifySceneStateDataTicks = num + 3000L;
										this.NotifySceneData(kuaFuBossScene);
									}
								}
							}
							else if (kuaFuBossScene.m_eStatus == 3)
							{
								GameManager.CopyMapMgr.KillAllMonster(kuaFuBossScene.CopyMap);
								kuaFuBossScene.m_eStatus = 4;
								YongZheZhanChangClient.getInstance().PushGameResultData(kuaFuBossScene.GameStatisticalData);
								YongZheZhanChangClient.getInstance().GameFuBenChangeState(kuaFuBossScene.GameId, 3, time);
								YongZheZhanChangFuBenData yongZheZhanChangFuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue(kuaFuBossScene.GameId, out yongZheZhanChangFuBenData))
								{
									yongZheZhanChangFuBenData.State = 3;
									LogManager.WriteLog(2, string.Format("跨服Boss跨服副本GameID={0},战斗结束", yongZheZhanChangFuBenData.GameId), null, true);
								}
							}
							else if (kuaFuBossScene.m_eStatus == 4)
							{
								if (num2 >= kuaFuBossScene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(kuaFuBossScene.m_lLeaveTime);
									kuaFuBossScene.m_eStatus = 5;
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
										DataHelper.WriteExceptionLogEx(ex, "跨服Boss系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		public void LeaveFuBen(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				KuaFuBossScene kuaFuBossScene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kuaFuBossScene))
				{
					kuaFuBossScene.m_nPlarerCount--;
				}
			}
		}

		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		public const SceneUIClasses ManagerType = 31;

		private static KuaFuBossManager instance = new KuaFuBossManager();

		public KuaFuBossData RuntimeData = new KuaFuBossData();

		public ConcurrentDictionary<int, KuaFuBossScene> SceneDict = new ConcurrentDictionary<int, KuaFuBossScene>();

		private static long NextHeartBeatTicks = 0L;
	}
}
