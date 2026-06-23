using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Ornament;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class YongZheZhanChangManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		public static YongZheZhanChangManager getInstance()
		{
			return YongZheZhanChangManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("YongZheZhanChangManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1100, 1, 1, YongZheZhanChangManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1101, 1, 1, YongZheZhanChangManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1103, 1, 1, YongZheZhanChangManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1108, 1, 1, YongZheZhanChangManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1102, 1, 1, YongZheZhanChangManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10001, 27, YongZheZhanChangManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10002, 27, YongZheZhanChangManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, YongZheZhanChangManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10001, 27, YongZheZhanChangManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10002, 27, YongZheZhanChangManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, YongZheZhanChangManager.getInstance());
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
			case 1100:
				return this.ProcessYongZheZhanChangJoinCmd(client, nID, bytes, cmdParams);
			case 1101:
				return this.ProcessYongZheZhanChangEnterCmd(client, nID, bytes, cmdParams);
			case 1102:
				return this.ProcessGetYongZheZhanChangAwardInfoCmd(client, nID, bytes, cmdParams);
			case 1103:
				return this.ProcessGetYongZheZhanChangStateCmd(client, nID, bytes, cmdParams);
			case 1108:
				return this.ProcessGetYongZheZhanChangAwardCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 10)
			{
				PlayerDeadEventObject playerDeadEventObject = eventObject as PlayerDeadEventObject;
				if (null != playerDeadEventObject)
				{
					if (playerDeadEventObject.Type == PlayerDeadEventTypes.ByRole)
					{
						this.OnKillRole(playerDeadEventObject.getAttackerRole(), playerDeadEventObject.getPlayer());
					}
				}
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 10001:
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
							LogManager.WriteLog(2, string.Format("通知角色ID={0}拥有进入勇者战场资格,跨服GameID={1}", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
						}
					}
					eventObject.Handled = true;
				}
				break;
			}
			case 10002:
			{
				CaiJiEventObject caiJiEventObject = eventObject as CaiJiEventObject;
				if (null != caiJiEventObject)
				{
					GameClient client = caiJiEventObject.Source as GameClient;
					Monster monster = caiJiEventObject.Target as Monster;
					this.OnCaiJiFinish(client, monster);
					eventObject.Handled = true;
					eventObject.Result = true;
				}
				break;
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
					this.RuntimeData.BattleCrystalMonsterDict.Clear();
					text = "Config/BattleCrystalMonster.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						BattleCrystalMonsterItem battleCrystalMonsterItem = new BattleCrystalMonsterItem();
						battleCrystalMonsterItem.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
						battleCrystalMonsterItem.MonsterID = (int)Global.GetSafeAttributeLong(xml, "MonsterID");
						battleCrystalMonsterItem.GatherTime = (int)Global.GetSafeAttributeLong(xml, "GatherTime");
						battleCrystalMonsterItem.BattleJiFen = (int)Global.GetSafeAttributeLong(xml, "BattleJiFen");
						battleCrystalMonsterItem.PosX = (int)Global.GetSafeAttributeLong(xml, "X");
						battleCrystalMonsterItem.PosY = (int)Global.GetSafeAttributeLong(xml, "Y");
						battleCrystalMonsterItem.FuHuoTime = (int)Global.GetSafeAttributeLong(xml, "FuHuoTime") * 1000;
						this.RuntimeData.BattleCrystalMonsterDict[battleCrystalMonsterItem.Id] = battleCrystalMonsterItem;
					}
					this.RuntimeData.MapBirthPointDict.Clear();
					text = "Config/ThroughServiceRebirth.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						YongZheZhanChangBirthPoint yongZheZhanChangBirthPoint = new YongZheZhanChangBirthPoint();
						yongZheZhanChangBirthPoint.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						yongZheZhanChangBirthPoint.PosX = (int)Global.GetSafeAttributeLong(xml, "PosX");
						yongZheZhanChangBirthPoint.PosY = (int)Global.GetSafeAttributeLong(xml, "PosY");
						yongZheZhanChangBirthPoint.BirthRadius = (int)Global.GetSafeAttributeLong(xml, "BirthRadius");
						this.RuntimeData.MapBirthPointDict[yongZheZhanChangBirthPoint.ID] = yongZheZhanChangBirthPoint;
					}
					this.RuntimeData.SceneDataDict.Clear();
					this.RuntimeData.LevelRangeSceneIdDict.Clear();
					text = "Config/ThroughServiceBattle.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						YongZheZhanChangSceneInfo yongZheZhanChangSceneInfo = new YongZheZhanChangSceneInfo();
						int num = (int)Global.GetSafeAttributeLong(xml, "Group");
						int num2 = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						yongZheZhanChangSceneInfo.Id = num;
						yongZheZhanChangSceneInfo.MapCode = num2;
						yongZheZhanChangSceneInfo.MinLevel = (int)Global.GetSafeAttributeLong(xml, "MinLevel");
						yongZheZhanChangSceneInfo.MaxLevel = (int)Global.GetSafeAttributeLong(xml, "MaxLevel");
						yongZheZhanChangSceneInfo.MinZhuanSheng = (int)Global.GetSafeAttributeLong(xml, "MinZhuanSheng");
						yongZheZhanChangSceneInfo.MaxZhuanSheng = (int)Global.GetSafeAttributeLong(xml, "MaxZhuanSheng");
						yongZheZhanChangSceneInfo.PrepareSecs = (int)Global.GetSafeAttributeLong(xml, "PrepareSecs");
						yongZheZhanChangSceneInfo.WaitingEnterSecs = (int)Global.GetSafeAttributeLong(xml, "WaitingEnterSecs");
						yongZheZhanChangSceneInfo.FightingSecs = (int)Global.GetSafeAttributeLong(xml, "FightingSecs");
						yongZheZhanChangSceneInfo.ClearRolesSecs = (int)Global.GetSafeAttributeLong(xml, "ClearRolesSecs");
						ConfigParser.ParseStrInt2(Global.GetSafeAttributeStr(xml, "ApplyTime"), ref yongZheZhanChangSceneInfo.SignUpStartSecs, ref yongZheZhanChangSceneInfo.SignUpEndSecs, ',');
						yongZheZhanChangSceneInfo.SignUpStartSecs += yongZheZhanChangSceneInfo.SignUpEndSecs;
						if (!ConfigParser.ParserTimeRangeListWithDay(yongZheZhanChangSceneInfo.TimePoints, Global.GetSafeAttributeStr(xml, "TimePoints"), true, '|', '-', ','))
						{
							result = false;
							LogManager.WriteLog(1000, string.Format("读取{0}时间配置(TimePoints)出错", text), null, true);
						}
						for (int i = 0; i < yongZheZhanChangSceneInfo.TimePoints.Count; i++)
						{
							TimeSpan timeSpan = new TimeSpan(yongZheZhanChangSceneInfo.TimePoints[i].Hours, yongZheZhanChangSceneInfo.TimePoints[i].Minutes, yongZheZhanChangSceneInfo.TimePoints[i].Seconds);
							yongZheZhanChangSceneInfo.SecondsOfDay.Add(timeSpan.TotalSeconds);
						}
						GameMap gameMap = null;
						if (!GameManager.MapMgr.DictMaps.TryGetValue(num2, out gameMap))
						{
							result = false;
							LogManager.WriteLog(1000, string.Format("地图配置中缺少{0}所需的地图:{1}", text, num2), null, true);
						}
						RangeKey key = new RangeKey(Global.GetUnionLevel(yongZheZhanChangSceneInfo.MinZhuanSheng, yongZheZhanChangSceneInfo.MinLevel, false), Global.GetUnionLevel(yongZheZhanChangSceneInfo.MaxZhuanSheng, yongZheZhanChangSceneInfo.MaxLevel, false), null);
						this.RuntimeData.LevelRangeSceneIdDict[key] = yongZheZhanChangSceneInfo;
						this.RuntimeData.SceneDataDict[num] = yongZheZhanChangSceneInfo;
					}
					text = "Config/ThroughServiceBattleAward.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						int num = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						YongZheZhanChangSceneInfo yongZheZhanChangSceneInfo;
						if (this.RuntimeData.SceneDataDict.TryGetValue(num, out yongZheZhanChangSceneInfo))
						{
							yongZheZhanChangSceneInfo.Exp = Global.GetSafeAttributeLong(xml, "Exp");
							yongZheZhanChangSceneInfo.BandJinBi = (int)Global.GetSafeAttributeLong(xml, "BandJinBi");
							ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "WinGoods"), ref yongZheZhanChangSceneInfo.WinAwardsItemList, '|', ',');
							ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "LoseGoods"), ref yongZheZhanChangSceneInfo.LoseAwardsItemList, '|', ',');
						}
					}
					this.RuntimeData.SceneDynMonsterDict.Clear();
					text = "Config/BattleMonster.xml";
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
						List<BattleDynamicMonsterItem> list = null;
						if (!this.RuntimeData.SceneDynMonsterDict.TryGetValue(battleDynamicMonsterItem.MapCode, out list))
						{
							list = new List<BattleDynamicMonsterItem>();
							this.RuntimeData.SceneDynMonsterDict[battleDynamicMonsterItem.MapCode] = list;
						}
						list.Add(battleDynamicMonsterItem);
					}
					this.RuntimeData.WarriorBattleBOssLastAttack = (int)GameManager.systemParamsList.GetParamValueIntByName("WarriorBattleBOssLastAttack", -1);
					this.RuntimeData.WarriorBattleLowestJiFen = (int)GameManager.systemParamsList.GetParamValueIntByName("WarriorBattleLowestJiFen", -1);
					double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WarriorBattleBossAttack", ',');
					if (paramValueDoubleArrayByName.Length == 2)
					{
						this.RuntimeData.WarriorBattleBossAttackPercent = paramValueDoubleArrayByName[0];
						this.RuntimeData.WarriorBattleBossAttackScore = (int)paramValueDoubleArrayByName[1];
					}
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("WarriorBattleUltraKill", ',');
					if (paramValueDoubleArrayByName.Length == 2)
					{
						this.RuntimeData.WarriorBattleUltraKillParam1 = paramValueIntArrayByName[0];
						this.RuntimeData.WarriorBattleUltraKillParam2 = paramValueIntArrayByName[1];
						this.RuntimeData.WarriorBattleUltraKillParam3 = paramValueIntArrayByName[2];
						this.RuntimeData.WarriorBattleUltraKillParam4 = paramValueIntArrayByName[3];
					}
					paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("WarriorBattleShutDown", ',');
					if (paramValueDoubleArrayByName.Length == 2)
					{
						this.RuntimeData.WarriorBattleShutDownParam1 = paramValueIntArrayByName[0];
						this.RuntimeData.WarriorBattleShutDownParam2 = paramValueIntArrayByName[1];
						this.RuntimeData.WarriorBattleShutDownParam3 = paramValueIntArrayByName[2];
						this.RuntimeData.WarriorBattleShutDownParam4 = paramValueIntArrayByName[3];
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
				YongZheZhanChangSceneInfo yongZheZhanChangSceneInfo = this.RuntimeData.SceneDataDict.Values.FirstOrDefault<YongZheZhanChangSceneInfo>();
				for (int i = 0; i < yongZheZhanChangSceneInfo.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)yongZheZhanChangSceneInfo.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= yongZheZhanChangSceneInfo.SecondsOfDay[i] - (double)yongZheZhanChangSceneInfo.SignUpStartSecs && dateTime.TimeOfDay.TotalSeconds <= yongZheZhanChangSceneInfo.SecondsOfDay[i + 1])
					{
						double num = yongZheZhanChangSceneInfo.SecondsOfDay[i] - dateTime.TimeOfDay.TotalSeconds;
						flag4 = true;
						if (!this.RuntimeData.PrepareGame)
						{
							if (num > 0.0 && num < (double)(yongZheZhanChangSceneInfo.SignUpEndSecs / 2))
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
				string cmd = string.Format("{0} {1} {2}", "GameState", 2, 5);
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
						gameClient.sendCmd<int>(1101, 1, false);
					}
				}
			}
		}

		public bool ProcessYongZheZhanChangJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				if (!GameFuncControlManager.IsGameFuncDisabled(6))
				{
					YongZheZhanChangSceneInfo yongZheZhanChangSceneInfo = null;
					YongZheZhanChangGameStates yongZheZhanChangGameStates = YongZheZhanChangGameStates.None;
					if (!this.CheckMap(client))
					{
						num = -21;
					}
					else
					{
						num = this.CheckCondition(client, ref yongZheZhanChangSceneInfo, ref yongZheZhanChangGameStates);
					}
					if (yongZheZhanChangGameStates != YongZheZhanChangGameStates.SignUp)
					{
						num = -2001;
					}
					else if (this.RuntimeData.RoleId2JoinGroup.ContainsKey(client.ClientData.RoleID))
					{
						num = -12;
					}
					if (num >= 0)
					{
						int id = yongZheZhanChangSceneInfo.Id;
						num = YongZheZhanChangClient.getInstance().YongZheZhanChangSignUp(client.strUserID, client.ClientData.RoleID, client.ClientData.ZoneID, 5, id, client.ClientData.CombatForce);
						if (num > 0)
						{
							this.RuntimeData.RoleId2JoinGroup[client.ClientData.RoleID] = id;
							client.ClientData.SignUpGameType = 5;
						}
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

		private int CheckCondition(GameClient client, ref YongZheZhanChangSceneInfo sceneItem, ref YongZheZhanChangGameStates state)
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
								state = YongZheZhanChangGameStates.None;
								result = -2001;
							}
							else if (dateTime.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpEndSecs)
							{
								state = YongZheZhanChangGameStates.SignUp;
								result = 1;
							}
							else if (dateTime.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i])
							{
								state = YongZheZhanChangGameStates.Wait;
								result = 1;
							}
							else if (dateTime.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1])
							{
								state = YongZheZhanChangGameStates.Start;
								result = 1;
							}
							else
							{
								state = YongZheZhanChangGameStates.None;
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
			YongZheZhanChangSceneInfo yongZheZhanChangSceneInfo = null;
			TimeSpan timeSpan = TimeSpan.MinValue;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.SceneDataDict.TryGetValue(sceneId, out yongZheZhanChangSceneInfo))
				{
					goto IL_153;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < yongZheZhanChangSceneInfo.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)yongZheZhanChangSceneInfo.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= yongZheZhanChangSceneInfo.SecondsOfDay[i] - (double)yongZheZhanChangSceneInfo.SignUpStartSecs && dateTime.TimeOfDay.TotalSeconds <= yongZheZhanChangSceneInfo.SecondsOfDay[i + 1])
					{
						timeSpan = TimeSpan.FromSeconds(yongZheZhanChangSceneInfo.SecondsOfDay[i]);
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

		public bool ProcessGetYongZheZhanChangAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 1;
				if (GameFuncControlManager.IsGameFuncDisabled(6))
				{
					return false;
				}
				string roleParamByName = Global.GetRoleParamByName(client, "YongZheZhanChangAwards");
				if (!string.IsNullOrEmpty(roleParamByName))
				{
					int num2 = 0;
					int score = 0;
					int success = 0;
					string text = Global.GetRoleParamByName(client, "44");
					int num3 = 0;
					List<string> list = Global.StringToList(text, '&');
					if (list.Count != 4 && !string.IsNullOrEmpty(text))
					{
						byte[] bytes2 = Convert.FromBase64String(text);
						text = new UTF8Encoding().GetString(bytes2);
						list = Global.StringToList(text, '&');
					}
					ConfigParser.ParseStrInt3(roleParamByName, ref num2, ref success, ref score, ',');
					List<int> list2 = Global.StringToIntList(roleParamByName, ',');
					num2 = list2[0];
					bool flag = true;
					if (list2.Count >= 5 && num2 > 0)
					{
						success = list2[1];
						score = list2[2];
						int num4 = list2[3];
						int num5 = list2[4];
						if (list.Count >= 4)
						{
							num3 = Global.SafeConvertToInt32(list[0]);
							string text2 = list[1];
							int num6 = Global.SafeConvertToInt32(list[2]);
							int num7 = Global.SafeConvertToInt32(list[3]);
						}
						YongZheZhanChangSceneInfo sceneInfo = null;
						if (this.RuntimeData.SceneDataDict.TryGetValue(num2, out sceneInfo))
						{
							num = this.GiveRoleAwards(client, success, score, sceneInfo);
							if (num < 0)
							{
								flag = false;
							}
							if (client.ClientData.RoleID == num3)
							{
								GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_YongZheZhanChangMVP, new int[0]));
							}
						}
					}
					if (flag)
					{
						Global.SaveRoleParamsStringToDB(client, "YongZheZhanChangAwards", this.RuntimeData.RoleParamsAwardsDefaultString, true);
						Global.SaveRoleParamsStringToDB(client, "44", "", true);
					}
					client.sendCmd<int>(nID, num, false);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		public bool ProcessGetYongZheZhanChangAwardInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(6))
				{
					return false;
				}
				string roleParamByName = Global.GetRoleParamByName(client, "YongZheZhanChangAwards");
				if (!string.IsNullOrEmpty(roleParamByName))
				{
					int num = 0;
					int num2 = 0;
					int success = 0;
					string text = Global.GetRoleParamByName(client, "44");
					int mvprid = 0;
					string mvpname = "";
					int mvpocc = 0;
					int mvpsex = 0;
					List<string> list = Global.StringToList(text, '&');
					if (list.Count != 4 && !string.IsNullOrEmpty(text))
					{
						byte[] bytes2 = Convert.FromBase64String(text);
						text = new UTF8Encoding().GetString(bytes2);
						list = Global.StringToList(text, '&');
					}
					ConfigParser.ParseStrInt3(roleParamByName, ref num, ref success, ref num2, ',');
					List<int> list2 = Global.StringToIntList(roleParamByName, ',');
					num = list2[0];
					bool flag = true;
					if (list2.Count >= 5 && num > 0)
					{
						success = list2[1];
						num2 = list2[2];
						int sideScore = list2[3];
						int sideScore2 = list2[4];
						if (list.Count >= 4)
						{
							mvprid = Global.SafeConvertToInt32(list[0]);
							mvpname = list[1];
							mvpocc = Global.SafeConvertToInt32(list[2]);
							mvpsex = Global.SafeConvertToInt32(list[3]);
						}
						YongZheZhanChangSceneInfo sceneInfo = null;
						if (this.RuntimeData.SceneDataDict.TryGetValue(num, out sceneInfo))
						{
							if (num2 >= this.RuntimeData.WarriorBattleLowestJiFen)
							{
								flag = false;
							}
							this.NtfCanGetAward(client, success, num2, sceneInfo, sideScore, sideScore2, mvprid, mvpname, mvpocc, mvpsex);
						}
					}
					if (flag)
					{
						Global.SaveRoleParamsStringToDB(client, "YongZheZhanChangAwards", this.RuntimeData.RoleParamsAwardsDefaultString, true);
						Global.SaveRoleParamsStringToDB(client, "44", "", true);
					}
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		public bool ProcessGetYongZheZhanChangStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string roleParamByName = Global.GetRoleParamByName(client, "YongZheZhanChangAwards");
				if (!string.IsNullOrEmpty(roleParamByName))
				{
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					ConfigParser.ParseStrInt3(roleParamByName, ref num, ref num3, ref num2, ',');
					if (num > 0)
					{
						YongZheZhanChangSceneInfo yongZheZhanChangSceneInfo = null;
						if (this.RuntimeData.SceneDataDict.TryGetValue(num, out yongZheZhanChangSceneInfo))
						{
							client.sendCmd<int>(nID, 4, false);
							return true;
						}
					}
				}
				YongZheZhanChangSceneInfo yongZheZhanChangSceneInfo2 = null;
				YongZheZhanChangGameStates yongZheZhanChangGameStates = YongZheZhanChangGameStates.None;
				int cmdData = 0;
				int num4 = 0;
				this.RuntimeData.RoleId2JoinGroup.TryGetValue(client.ClientData.RoleID, out num4);
				this.CheckCondition(client, ref yongZheZhanChangSceneInfo2, ref yongZheZhanChangGameStates);
				if (num4 > 0)
				{
					if (yongZheZhanChangGameStates >= YongZheZhanChangGameStates.SignUp && yongZheZhanChangGameStates <= YongZheZhanChangGameStates.Wait)
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
					else if (yongZheZhanChangGameStates == YongZheZhanChangGameStates.Start)
					{
						if (this.RuntimeData.RoleIdKuaFuLoginDataDict.ContainsKey(client.ClientData.RoleID))
						{
							cmdData = 3;
						}
					}
				}
				else if (yongZheZhanChangGameStates == YongZheZhanChangGameStates.SignUp)
				{
					cmdData = 1;
				}
				else if (yongZheZhanChangGameStates == YongZheZhanChangGameStates.Wait || yongZheZhanChangGameStates == YongZheZhanChangGameStates.Start)
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

		public bool ProcessYongZheZhanChangEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				if (GameFuncControlManager.IsGameFuncDisabled(6))
				{
					client.sendCmd<int>(nID, num, false);
					return true;
				}
				YongZheZhanChangSceneInfo yongZheZhanChangSceneInfo = null;
				YongZheZhanChangGameStates yongZheZhanChangGameStates = YongZheZhanChangGameStates.None;
				if (!this.CheckMap(client))
				{
					num = -21;
				}
				else
				{
					num = this.CheckCondition(client, ref yongZheZhanChangSceneInfo, ref yongZheZhanChangGameStates);
				}
				if (yongZheZhanChangGameStates == YongZheZhanChangGameStates.Start)
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
			int battleWhichSide = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				YongZheZhanChangBirthPoint yongZheZhanChangBirthPoint = null;
				if (this.RuntimeData.MapBirthPointDict.TryGetValue(battleWhichSide, out yongZheZhanChangBirthPoint))
				{
					posX = yongZheZhanChangBirthPoint.PosX;
					posY = yongZheZhanChangBirthPoint.PosY;
					return battleWhichSide;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
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
				client.ClientData.BattleWhichSide = kuaFuFuBenRoleData.Side;
				int posX;
				int posY;
				int birthPoint = this.GetBirthPoint(client, out posX, out posY);
				if (birthPoint <= 0)
				{
					LogManager.WriteLog(2, "无法获取有效的阵营和出生点,进入跨服失败,side=" + birthPoint, null, true);
					result = false;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						clientKuaFuServerLoginData.FuBenSeqId = yongZheZhanChangFuBenData.SequenceId;
						YongZheZhanChangSceneInfo yongZheZhanChangSceneInfo;
						if (!this.RuntimeData.SceneDataDict.TryGetValue(yongZheZhanChangFuBenData.GroupIndex, out yongZheZhanChangSceneInfo))
						{
							return false;
						}
						client.ClientData.MapCode = yongZheZhanChangSceneInfo.MapCode;
					}
					client.ClientData.PosX = posX;
					client.ClientData.PosY = posY;
					client.ClientData.FuBenSeqID = clientKuaFuServerLoginData.FuBenSeqId;
					result = true;
				}
			}
			return result;
		}

		public bool ClientRelive(GameClient client)
		{
			int posX;
			int posY;
			int birthPoint = this.GetBirthPoint(client, out posX, out posY);
			bool result;
			if (birthPoint <= 0)
			{
				result = false;
			}
			else
			{
				client.ClientData.CurrentLifeV = client.ClientData.LifeV;
				client.ClientData.CurrentMagicV = client.ClientData.MagicV;
				client.ClientData.MoveAndActionNum = 0;
				GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, posX, posY, -1);
				Global.ClientRealive(client, posX, posY, -1);
				result = true;
			}
			return result;
		}

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(6) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("YongZheZhanChang") && GlobalNew.IsGongNengOpened(client, 66, hint);
		}

		public int GetCaiJiMonsterTime(GameClient client, Monster monster)
		{
			BattleCrystalMonsterItem battleCrystalMonsterItem = (monster != null) ? (monster.Tag as BattleCrystalMonsterItem) : null;
			int result;
			if (battleCrystalMonsterItem == null)
			{
				result = -200;
			}
			else
			{
				result = battleCrystalMonsterItem.GatherTime;
			}
			return result;
		}

		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 27)
			{
				GameMap gameMap = null;
				if (!GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap))
				{
					result = false;
				}
				else
				{
					int fuBenSeqID = copyMap.FuBenSeqID;
					int mapCode = copyMap.MapCode;
					int roleID = client.ClientData.RoleID;
					int num = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
					DateTime dateTime = TimeUtil.NowDateTime();
					lock (this.RuntimeData.Mutex)
					{
						YongZheZhanChangScene yongZheZhanChangScene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqID, out yongZheZhanChangScene))
						{
							YongZheZhanChangSceneInfo yongZheZhanChangSceneInfo = null;
							YongZheZhanChangFuBenData yongZheZhanChangFuBenData;
							if (!this.RuntimeData.FuBenItemData.TryGetValue(num, out yongZheZhanChangFuBenData))
							{
								LogManager.WriteLog(2, "勇者战场没有为副本找到对应的跨服副本数据,GameID:" + num, null, true);
							}
							if (!this.RuntimeData.SceneDataDict.TryGetValue(yongZheZhanChangFuBenData.GroupIndex, out yongZheZhanChangSceneInfo))
							{
								LogManager.WriteLog(2, "勇者战场没有为副本找到对应的档位数据,ID:" + yongZheZhanChangFuBenData.GroupIndex, null, true);
							}
							yongZheZhanChangScene = new YongZheZhanChangScene();
							yongZheZhanChangScene.CopyMap = copyMap;
							yongZheZhanChangScene.CleanAllInfo();
							yongZheZhanChangScene.GameId = num;
							yongZheZhanChangScene.m_nMapCode = mapCode;
							yongZheZhanChangScene.CopyMapId = copyMap.CopyMapID;
							yongZheZhanChangScene.FuBenSeqId = fuBenSeqID;
							yongZheZhanChangScene.m_nPlarerCount = 1;
							yongZheZhanChangScene.SceneInfo = yongZheZhanChangSceneInfo;
							yongZheZhanChangScene.MapGridWidth = gameMap.MapGridWidth;
							yongZheZhanChangScene.MapGridHeight = gameMap.MapGridHeight;
							DateTime dateTime2 = dateTime.Date.Add(this.GetStartTime(yongZheZhanChangSceneInfo.Id));
							yongZheZhanChangScene.StartTimeTicks = dateTime2.Ticks / 10000L;
							yongZheZhanChangScene.GameStatisticalData.GameId = num;
							this.SceneDict[fuBenSeqID] = yongZheZhanChangScene;
						}
						else
						{
							yongZheZhanChangScene.m_nPlarerCount++;
						}
						YongZheZhanChangClientContextData yongZheZhanChangClientContextData;
						if (!yongZheZhanChangScene.ClientContextDataDict.TryGetValue(roleID, out yongZheZhanChangClientContextData))
						{
							yongZheZhanChangClientContextData = new YongZheZhanChangClientContextData
							{
								RoleId = roleID,
								ServerId = client.ServerId,
								BattleWhichSide = client.ClientData.BattleWhichSide,
								RoleName = client.ClientData.RoleName,
								Occupation = client.ClientData.Occupation,
								RoleSex = client.ClientData.RoleSex,
								ZoneID = client.ClientData.ZoneID
							};
							yongZheZhanChangScene.ClientContextDataDict[roleID] = yongZheZhanChangClientContextData;
						}
						else
						{
							yongZheZhanChangClientContextData.KillNum = 0;
						}
						client.SceneContextData2 = yongZheZhanChangClientContextData;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(yongZheZhanChangScene.SceneInfo.TotalSecs * 1000));
					}
					YongZheZhanChangClient.getInstance().GameFuBenRoleChangeState(roleID, 5, 0, 0);
					result = true;
				}
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
			if (sceneType == 27)
			{
				lock (this.RuntimeData.Mutex)
				{
					YongZheZhanChangScene yongZheZhanChangScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out yongZheZhanChangScene);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void OnCaiJiFinish(GameClient client, Monster monster)
		{
			int num = 0;
			YongZheZhanChangScene yongZheZhanChangScene;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out yongZheZhanChangScene))
				{
					return;
				}
				if (yongZheZhanChangScene.m_eStatus != 2)
				{
					return;
				}
				BattleCrystalMonsterItem battleCrystalMonsterItem = monster.Tag as BattleCrystalMonsterItem;
				if (battleCrystalMonsterItem == null)
				{
					return;
				}
				YongZheZhanChangClientContextData yongZheZhanChangClientContextData = client.SceneContextData2 as YongZheZhanChangClientContextData;
				if (null != yongZheZhanChangClientContextData)
				{
					num = battleCrystalMonsterItem.BattleJiFen;
					yongZheZhanChangClientContextData.TotalScore += num;
					yongZheZhanChangScene.GameStatisticalData.CaiJiScore += num;
					if (client.ClientData.BattleWhichSide == 1)
					{
						yongZheZhanChangScene.ScoreData.Score1 += num;
					}
					else if (client.ClientData.BattleWhichSide == 2)
					{
						yongZheZhanChangScene.ScoreData.Score2 += num;
					}
				}
				this.AddDelayCreateMonster(yongZheZhanChangScene, TimeUtil.NOW() + (long)battleCrystalMonsterItem.FuHuoTime, battleCrystalMonsterItem);
			}
			if (num > 0)
			{
				GameManager.ClientMgr.BroadSpecialCopyMapMessage<YongZheZhanChangScoreData>(1104, yongZheZhanChangScene.ScoreData, yongZheZhanChangScene.CopyMap);
				this.NotifyTimeStateInfoAndScoreInfo(client, false, false, true);
			}
		}

		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			if (monster.MonsterType == 401)
			{
				YongZheZhanChangClientContextData yongZheZhanChangClientContextData = client.SceneContextData2 as YongZheZhanChangClientContextData;
				if (null != yongZheZhanChangClientContextData)
				{
					YongZheZhanChangScene yongZheZhanChangScene = null;
					int num = 0;
					if (monster.HandledDead && monster.WhoKillMeID == client.ClientData.RoleID)
					{
						num += this.RuntimeData.WarriorBattleBOssLastAttack;
					}
					double num2 = this.RuntimeData.WarriorBattleBossAttackPercent * monster.MonsterInfo.VLifeMax;
					yongZheZhanChangClientContextData.InjureBossDelta += (double)injure;
					if (yongZheZhanChangClientContextData.InjureBossDelta >= num2 && num2 > 0.0)
					{
						int num3 = (int)(yongZheZhanChangClientContextData.InjureBossDelta / num2);
						yongZheZhanChangClientContextData.InjureBossDelta -= num2 * (double)num3;
						num += this.RuntimeData.WarriorBattleBossAttackScore * num3;
					}
					lock (this.RuntimeData.Mutex)
					{
						yongZheZhanChangClientContextData.TotalScore += num;
						if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out yongZheZhanChangScene))
						{
							if (yongZheZhanChangScene.m_eStatus != 2)
							{
								return;
							}
							if (client.ClientData.BattleWhichSide == 1)
							{
								yongZheZhanChangScene.ScoreData.Score1 += num;
							}
							else if (client.ClientData.BattleWhichSide == 2)
							{
								yongZheZhanChangScene.ScoreData.Score2 += num;
							}
							yongZheZhanChangScene.GameStatisticalData.BossScore += num;
						}
					}
					if (num > 0 && yongZheZhanChangScene != null)
					{
						GameManager.ClientMgr.BroadSpecialCopyMapMessage<YongZheZhanChangScoreData>(1104, yongZheZhanChangScene.ScoreData, yongZheZhanChangScene.CopyMap);
						this.NotifyTimeStateInfoAndScoreInfo(client, false, false, true);
					}
				}
			}
		}

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= YongZheZhanChangManager.NextHeartBeatTicks)
			{
				YongZheZhanChangManager.NextHeartBeatTicks = num + 1020L;
				foreach (YongZheZhanChangScene yongZheZhanChangScene in this.SceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int fuBenSeqId = yongZheZhanChangScene.FuBenSeqId;
						int copyMapId = yongZheZhanChangScene.CopyMapId;
						int nMapCode = yongZheZhanChangScene.m_nMapCode;
						if (fuBenSeqId >= 0 && copyMapId >= 0 && nMapCode >= 0)
						{
							CopyMap copyMap = yongZheZhanChangScene.CopyMap;
							DateTime time = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							if (yongZheZhanChangScene.m_eStatus == 0)
							{
								if (num2 >= yongZheZhanChangScene.StartTimeTicks)
								{
									yongZheZhanChangScene.m_lPrepareTime = yongZheZhanChangScene.StartTimeTicks;
									yongZheZhanChangScene.m_lBeginTime = yongZheZhanChangScene.m_lPrepareTime + (long)(yongZheZhanChangScene.SceneInfo.PrepareSecs * 1000);
									yongZheZhanChangScene.m_eStatus = 1;
									yongZheZhanChangScene.StateTimeData.GameType = 5;
									yongZheZhanChangScene.StateTimeData.State = yongZheZhanChangScene.m_eStatus;
									yongZheZhanChangScene.StateTimeData.EndTicks = yongZheZhanChangScene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, yongZheZhanChangScene.StateTimeData, yongZheZhanChangScene.CopyMap);
								}
							}
							else if (yongZheZhanChangScene.m_eStatus == 1)
							{
								if (num2 >= yongZheZhanChangScene.m_lBeginTime)
								{
									yongZheZhanChangScene.m_eStatus = 2;
									yongZheZhanChangScene.m_lEndTime = yongZheZhanChangScene.m_lBeginTime + (long)(yongZheZhanChangScene.SceneInfo.FightingSecs * 1000);
									yongZheZhanChangScene.StateTimeData.GameType = 5;
									yongZheZhanChangScene.StateTimeData.State = yongZheZhanChangScene.m_eStatus;
									yongZheZhanChangScene.StateTimeData.EndTicks = yongZheZhanChangScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, yongZheZhanChangScene.StateTimeData, yongZheZhanChangScene.CopyMap);
									this.InitCreateDynamicMonster(yongZheZhanChangScene);
									copyMap.AddGuangMuEvent(1, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 1, 0);
									copyMap.AddGuangMuEvent(2, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 2, 0);
									copyMap.AddGuangMuEvent(3, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 3, 0);
									copyMap.AddGuangMuEvent(4, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 4, 0);
								}
							}
							else if (yongZheZhanChangScene.m_eStatus == 2)
							{
								if (num2 >= yongZheZhanChangScene.m_lEndTime)
								{
									int successSide = 0;
									if (yongZheZhanChangScene.ScoreData.Score1 > yongZheZhanChangScene.ScoreData.Score2)
									{
										successSide = 1;
									}
									else if (yongZheZhanChangScene.ScoreData.Score2 > yongZheZhanChangScene.ScoreData.Score1)
									{
										successSide = 2;
									}
									this.CompleteScene(yongZheZhanChangScene, successSide);
									yongZheZhanChangScene.m_eStatus = 3;
									yongZheZhanChangScene.m_lLeaveTime = yongZheZhanChangScene.m_lEndTime + (long)(yongZheZhanChangScene.SceneInfo.ClearRolesSecs * 1000);
									yongZheZhanChangScene.StateTimeData.GameType = 5;
									yongZheZhanChangScene.StateTimeData.State = 5;
									yongZheZhanChangScene.StateTimeData.EndTicks = yongZheZhanChangScene.m_lLeaveTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, yongZheZhanChangScene.StateTimeData, yongZheZhanChangScene.CopyMap);
								}
								else
								{
									this.CheckCreateDynamicMonster(yongZheZhanChangScene, num2);
								}
							}
							else if (yongZheZhanChangScene.m_eStatus == 3)
							{
								GameManager.CopyMapMgr.KillAllMonster(yongZheZhanChangScene.CopyMap);
								yongZheZhanChangScene.m_eStatus = 4;
								YongZheZhanChangClient.getInstance().GameFuBenChangeState(yongZheZhanChangScene.GameId, 3, time);
								this.GiveAwards(yongZheZhanChangScene);
								YongZheZhanChangFuBenData yongZheZhanChangFuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue(yongZheZhanChangScene.GameId, out yongZheZhanChangFuBenData))
								{
									yongZheZhanChangFuBenData.State = 3;
									LogManager.WriteLog(2, string.Format("勇者战场跨服副本GameID={0},战斗结束", yongZheZhanChangFuBenData.GameId), null, true);
								}
							}
							else if (yongZheZhanChangScene.m_eStatus == 4)
							{
								if (num2 >= yongZheZhanChangScene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(yongZheZhanChangScene.m_lLeaveTime);
									yongZheZhanChangScene.m_eStatus = 5;
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
										DataHelper.WriteExceptionLogEx(ex, "勇者战场系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		private void CreateCrystalMonster(YongZheZhanChangScene scene, BattleCrystalMonsterItem crystal)
		{
			GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, crystal.MonsterID, scene.CopyMapId, 1, crystal.PosX / scene.MapGridWidth, crystal.PosY / scene.MapGridHeight, 0, 0, 27, crystal, null);
		}

		private void AddDelayCreateMonster(YongZheZhanChangScene scene, long ticks, object monster)
		{
			lock (this.RuntimeData.Mutex)
			{
				List<object> list = null;
				if (!scene.CreateMonsterQueue.TryGetValue(ticks, out list))
				{
					list = new List<object>();
					scene.CreateMonsterQueue.Add(ticks, list);
				}
				list.Add(monster);
			}
		}

		private void InitCreateDynamicMonster(YongZheZhanChangScene scene)
		{
			lock (this.RuntimeData.Mutex)
			{
				foreach (BattleCrystalMonsterItem monster in this.RuntimeData.BattleCrystalMonsterDict.Values)
				{
					this.AddDelayCreateMonster(scene, scene.m_lBeginTime, monster);
				}
				List<BattleDynamicMonsterItem> list = null;
				if (this.RuntimeData.SceneDynMonsterDict.TryGetValue(scene.m_nMapCode, out list))
				{
					foreach (BattleDynamicMonsterItem battleDynamicMonsterItem in list)
					{
						this.AddDelayCreateMonster(scene, scene.m_lBeginTime + (long)battleDynamicMonsterItem.DelayBirthMs, battleDynamicMonsterItem);
					}
				}
			}
		}

		public void CheckCreateDynamicMonster(YongZheZhanChangScene scene, long nowMs)
		{
			lock (this.RuntimeData.Mutex)
			{
				while (scene.CreateMonsterQueue.Count > 0)
				{
					KeyValuePair<long, List<object>> keyValuePair = scene.CreateMonsterQueue.First<KeyValuePair<long, List<object>>>();
					if (nowMs < keyValuePair.Key)
					{
						break;
					}
					try
					{
						foreach (object obj in keyValuePair.Value)
						{
							if (obj is BattleDynamicMonsterItem)
							{
								BattleDynamicMonsterItem battleDynamicMonsterItem = obj as BattleDynamicMonsterItem;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, battleDynamicMonsterItem.MonsterID, scene.CopyMapId, 1, battleDynamicMonsterItem.PosX / scene.MapGridWidth, battleDynamicMonsterItem.PosY / scene.MapGridHeight, 0, 0, 27, battleDynamicMonsterItem, null);
							}
							else if (obj is BattleCrystalMonsterItem)
							{
								BattleCrystalMonsterItem battleCrystalMonsterItem = obj as BattleCrystalMonsterItem;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, battleCrystalMonsterItem.MonsterID, scene.CopyMap.CopyMapID, 1, battleCrystalMonsterItem.PosX / scene.MapGridWidth, battleCrystalMonsterItem.PosY / scene.MapGridHeight, 0, 0, 27, battleCrystalMonsterItem, null);
							}
						}
					}
					finally
					{
						scene.CreateMonsterQueue.RemoveAt(0);
					}
				}
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true, bool selfScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				YongZheZhanChangScene yongZheZhanChangScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out yongZheZhanChangScene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, yongZheZhanChangScene.StateTimeData, false);
					}
					if (sideScore)
					{
						client.sendCmd<YongZheZhanChangScoreData>(1104, yongZheZhanChangScene.ScoreData, false);
					}
					if (selfScore)
					{
						YongZheZhanChangClientContextData yongZheZhanChangClientContextData = client.SceneContextData2 as YongZheZhanChangClientContextData;
						if (null != yongZheZhanChangClientContextData)
						{
							client.sendCmd<int>(1105, yongZheZhanChangClientContextData.TotalScore, false);
						}
					}
				}
			}
		}

		public void CompleteScene(YongZheZhanChangScene scene, int successSide)
		{
			scene.SuccessSide = successSide;
			if (successSide != 0)
			{
				List<YongZheZhanChangClientContextData> list = new List<YongZheZhanChangClientContextData>();
				foreach (YongZheZhanChangClientContextData yongZheZhanChangClientContextData in scene.ClientContextDataDict.Values)
				{
					if (yongZheZhanChangClientContextData.BattleWhichSide == successSide)
					{
						list.Add(yongZheZhanChangClientContextData);
					}
				}
				list.Sort(delegate(YongZheZhanChangClientContextData left, YongZheZhanChangClientContextData right)
				{
					int result;
					if (left.TotalScore > right.TotalScore)
					{
						result = -1;
					}
					else if (left.TotalScore < right.TotalScore)
					{
						result = 1;
					}
					else
					{
						result = 0;
					}
					return result;
				});
				if (list.Count != 0)
				{
					scene.ClientContextMVP = list[0];
				}
			}
		}

		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				YongZheZhanChangScene yongZheZhanChangScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out yongZheZhanChangScene))
				{
					if (yongZheZhanChangScene.m_eStatus == 2)
					{
						int num = 0;
						int warriorBattleDie = this.RuntimeData.WarriorBattleDie;
						YongZheZhanChangClientContextData yongZheZhanChangClientContextData = client.SceneContextData2 as YongZheZhanChangClientContextData;
						YongZheZhanChangClientContextData yongZheZhanChangClientContextData2 = other.SceneContextData2 as YongZheZhanChangClientContextData;
						HuanYingSiYuanLianSha huanYingSiYuanLianSha = null;
						HuanYingSiYuanLianshaOver huanYingSiYuanLianshaOver = null;
						HuanYingSiYuanAddScore huanYingSiYuanAddScore = new HuanYingSiYuanAddScore();
						huanYingSiYuanAddScore.Name = Global.FormatRoleName4(client);
						huanYingSiYuanAddScore.ZoneID = client.ClientData.ZoneID;
						huanYingSiYuanAddScore.Side = client.ClientData.BattleWhichSide;
						huanYingSiYuanAddScore.ByLianShaNum = 1;
						huanYingSiYuanAddScore.RoleId = client.ClientData.RoleID;
						huanYingSiYuanAddScore.Occupation = client.ClientData.Occupation;
						yongZheZhanChangScene.GameStatisticalData.KillScore += this.RuntimeData.WarriorBattleUltraKillParam1;
						if (null != yongZheZhanChangClientContextData)
						{
							yongZheZhanChangClientContextData.KillNum++;
							int num2 = this.RuntimeData.WarriorBattleUltraKillParam1 + yongZheZhanChangClientContextData.KillNum * this.RuntimeData.WarriorBattleUltraKillParam2;
							num2 = Math.Min(this.RuntimeData.WarriorBattleUltraKillParam4, Math.Max(this.RuntimeData.WarriorBattleUltraKillParam3, num2));
							huanYingSiYuanAddScore.ByLianShaNum = 1;
							huanYingSiYuanLianSha = new HuanYingSiYuanLianSha();
							huanYingSiYuanLianSha.Name = huanYingSiYuanAddScore.Name;
							huanYingSiYuanLianSha.ZoneID = huanYingSiYuanAddScore.ZoneID;
							huanYingSiYuanLianSha.Occupation = huanYingSiYuanAddScore.Occupation;
							huanYingSiYuanLianSha.LianShaType = Math.Min(yongZheZhanChangClientContextData.KillNum, 30) / 5;
							huanYingSiYuanLianSha.ExtScore = num2;
							huanYingSiYuanLianSha.Side = huanYingSiYuanAddScore.Side;
							num += num2;
							yongZheZhanChangScene.GameStatisticalData.LianShaScore += num2;
							if (yongZheZhanChangClientContextData.KillNum % 5 != 0)
							{
								huanYingSiYuanLianSha = null;
							}
						}
						if (null != yongZheZhanChangClientContextData2)
						{
							int num3 = this.RuntimeData.WarriorBattleShutDownParam1 + yongZheZhanChangClientContextData2.KillNum * this.RuntimeData.WarriorBattleShutDownParam2;
							num3 = Math.Min(this.RuntimeData.WarriorBattleShutDownParam4, Math.Max(this.RuntimeData.WarriorBattleShutDownParam3, num3));
							num += num3;
							yongZheZhanChangScene.GameStatisticalData.ZhongJieScore += num3;
							if (yongZheZhanChangClientContextData2.KillNum >= 10)
							{
								huanYingSiYuanLianshaOver = new HuanYingSiYuanLianshaOver();
								huanYingSiYuanLianshaOver.KillerName = huanYingSiYuanAddScore.Name;
								huanYingSiYuanLianshaOver.KillerZoneID = huanYingSiYuanAddScore.ZoneID;
								huanYingSiYuanLianshaOver.KillerOccupation = client.ClientData.Occupation;
								huanYingSiYuanLianshaOver.KillerSide = huanYingSiYuanAddScore.Side;
								huanYingSiYuanLianshaOver.KilledName = Global.FormatRoleName4(other);
								huanYingSiYuanLianshaOver.KilledZoneID = other.ClientData.ZoneID;
								huanYingSiYuanLianshaOver.KilledOccupation = other.ClientData.Occupation;
								huanYingSiYuanLianshaOver.KilledSide = other.ClientData.BattleWhichSide;
								huanYingSiYuanLianshaOver.ExtScore = num3;
							}
							yongZheZhanChangClientContextData2.KillNum = 0;
							yongZheZhanChangClientContextData2.TotalScore += warriorBattleDie;
							yongZheZhanChangScene.GameStatisticalData.KillScore += warriorBattleDie;
						}
						huanYingSiYuanAddScore.Score = num;
						if (client.ClientData.BattleWhichSide == 1)
						{
							yongZheZhanChangScene.ScoreData.Score1 += num;
							yongZheZhanChangScene.ScoreData.Score2 += warriorBattleDie;
						}
						else
						{
							yongZheZhanChangScene.ScoreData.Score2 += num;
							yongZheZhanChangScene.ScoreData.Score1 += warriorBattleDie;
						}
						if (null != yongZheZhanChangClientContextData)
						{
							yongZheZhanChangClientContextData.TotalScore += num;
						}
						GameManager.ClientMgr.BroadSpecialCopyMapMessage<YongZheZhanChangScoreData>(1104, yongZheZhanChangScene.ScoreData, yongZheZhanChangScene.CopyMap);
						if (null != huanYingSiYuanLianSha)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianSha>(1106, huanYingSiYuanLianSha, yongZheZhanChangScene.CopyMap);
						}
						if (null != huanYingSiYuanLianshaOver)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianshaOver>(1107, huanYingSiYuanLianshaOver, yongZheZhanChangScene.CopyMap);
						}
						this.NotifyTimeStateInfoAndScoreInfo(client, false, false, true);
						this.NotifyTimeStateInfoAndScoreInfo(other, false, false, true);
					}
				}
			}
		}

		public void GiveAwards(YongZheZhanChangScene scene)
		{
			try
			{
				Dictionary<int, int[]> dictionary = new Dictionary<int, int[]>();
				YongZheZhanChangStatisticalData gameStatisticalData = scene.GameStatisticalData;
				foreach (YongZheZhanChangClientContextData yongZheZhanChangClientContextData in scene.ClientContextDataDict.Values)
				{
					gameStatisticalData.AllRoleCount++;
					int num;
					if (yongZheZhanChangClientContextData.BattleWhichSide == scene.SuccessSide)
					{
						num = 1;
						gameStatisticalData.WinRoleCount++;
					}
					else
					{
						num = 0;
						gameStatisticalData.LoseRoleCount++;
					}
					GameClient gameClient = GameManager.ClientMgr.FindClient(yongZheZhanChangClientContextData.RoleId);
					string text = string.Format("{0},{1},{2},{3},{4}", new object[]
					{
						scene.SceneInfo.Id,
						num,
						yongZheZhanChangClientContextData.TotalScore,
						scene.ScoreData.Score1,
						scene.ScoreData.Score2
					});
					string text2 = string.Format("{0}&{1}&{2}&{3}", new object[]
					{
						scene.ClientContextMVP.RoleId,
						scene.ClientContextMVP.RoleName,
						scene.ClientContextMVP.Occupation,
						scene.ClientContextMVP.RoleSex
					});
					byte[] bytes = new UTF8Encoding().GetBytes(text2);
					text2 = Convert.ToBase64String(bytes);
					if (gameClient != null)
					{
						int faction = gameClient.ClientData.Faction;
						int junTuanId = gameClient.ClientData.JunTuanId;
						if (faction > 0 && junTuanId > 0)
						{
							int[] array;
							if (!dictionary.TryGetValue(faction, out array))
							{
								int[] array2 = new int[2];
								array2[0] = junTuanId;
								array = array2;
								dictionary[faction] = array;
							}
							array[1]++;
						}
						int totalScore = yongZheZhanChangClientContextData.TotalScore;
						yongZheZhanChangClientContextData.TotalScore = 0;
						if (totalScore >= this.RuntimeData.WarriorBattleLowestJiFen)
						{
							Global.SaveRoleParamsStringToDB(gameClient, "YongZheZhanChangAwards", text, true);
							Global.SaveRoleParamsStringToDB(gameClient, "44", text2, true);
						}
						else
						{
							Global.SaveRoleParamsStringToDB(gameClient, "YongZheZhanChangAwards", this.RuntimeData.RoleParamsAwardsDefaultString, true);
							Global.SaveRoleParamsStringToDB(gameClient, "44", "", true);
						}
						this.NtfCanGetAward(gameClient, num, totalScore, scene.SceneInfo, scene.ScoreData.Score1, scene.ScoreData.Score2, scene.ClientContextMVP.RoleId, scene.ClientContextMVP.RoleName, scene.ClientContextMVP.Occupation, scene.ClientContextMVP.RoleSex);
					}
					else if (yongZheZhanChangClientContextData.TotalScore >= this.RuntimeData.WarriorBattleLowestJiFen)
					{
						Global.UpdateRoleParamByNameOffline(yongZheZhanChangClientContextData.RoleId, "YongZheZhanChangAwards", text, yongZheZhanChangClientContextData.ServerId);
						Global.UpdateRoleParamByNameOffline(yongZheZhanChangClientContextData.RoleId, "44", text2, yongZheZhanChangClientContextData.ServerId);
					}
					else
					{
						Global.UpdateRoleParamByNameOffline(yongZheZhanChangClientContextData.RoleId, "YongZheZhanChangAwards", this.RuntimeData.RoleParamsAwardsDefaultString, yongZheZhanChangClientContextData.ServerId);
						Global.UpdateRoleParamByNameOffline(yongZheZhanChangClientContextData.RoleId, "44", "", yongZheZhanChangClientContextData.ServerId);
					}
				}
				foreach (KeyValuePair<int, int[]> keyValuePair in dictionary)
				{
					JunTuanManager.getInstance().AddJunTuanTaskValue(keyValuePair.Key, keyValuePair.Value[0], Global.GetMapSceneType(scene.m_nMapCode), keyValuePair.Value[1]);
				}
				YongZheZhanChangClient.getInstance().PushGameResultData(gameStatisticalData);
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "天梯系统清场调度异常");
			}
		}

		private void NtfCanGetAward(GameClient client, int success, int score, YongZheZhanChangSceneInfo sceneInfo, int sideScore1, int sideScore2, int mvprid, string mvpname, int mvpocc, int mvpsex)
		{
			long num = 0L;
			int num2 = 0;
			List<AwardsItemData> awardsItemDataList = null;
			if (score >= this.RuntimeData.WarriorBattleLowestJiFen)
			{
				num = (long)((double)sceneInfo.Exp * (0.2 + Math.Min(0.8, Math.Pow((double)score, 0.5) / 100.0)));
				num2 = (int)((double)sceneInfo.BandJinBi * Math.Min(100.0, Math.Pow((double)score, 0.5)));
				if (success > 0)
				{
					awardsItemDataList = sceneInfo.WinAwardsItemList.Items;
				}
				else
				{
					num = (long)((double)num * 0.8);
					num2 = (int)((double)num2 * 0.8);
					awardsItemDataList = sceneInfo.LoseAwardsItemList.Items;
				}
				num -= num % 10000L;
				num2 -= num2 % 10000;
			}
			client.sendCmd<YongZheZhanChangAwardsData>(1102, new YongZheZhanChangAwardsData
			{
				Exp = num,
				BindJinBi = num2,
				Success = success,
				AwardsItemDataList = awardsItemDataList,
				SideScore1 = sideScore1,
				SideScore2 = sideScore2,
				SelfScore = score,
				MvpRoleName = mvpname,
				MvpOccupation = mvpocc,
				MvpRoleSex = mvpsex
			}, false);
		}

		private int GiveRoleAwards(GameClient client, int success, int score, YongZheZhanChangSceneInfo sceneInfo)
		{
			long num = 0L;
			int num2 = 0;
			List<AwardsItemData> list = null;
			if (score >= this.RuntimeData.WarriorBattleLowestJiFen)
			{
				num = (long)((double)sceneInfo.Exp * (0.2 + Math.Min(0.8, Math.Pow((double)score, 0.5) / 100.0)));
				num2 = (int)((double)sceneInfo.BandJinBi * Math.Min(100.0, Math.Pow((double)score, 0.5)));
				if (success > 0)
				{
					list = sceneInfo.WinAwardsItemList.Items;
				}
				else
				{
					num = (long)((double)num * 0.8);
					num2 = (int)((double)num2 * 0.8);
					list = sceneInfo.LoseAwardsItemList.Items;
				}
				num -= num % 10000L;
				num2 -= num2 % 10000;
			}
			int result;
			if (list != null && !Global.CanAddGoodsNum(client, list.Count))
			{
				result = -100;
			}
			else
			{
				if (num > 0L)
				{
					GameManager.ClientMgr.ProcessRoleExperience(client, num, true, true, false, "none");
				}
				if (num2 > 0)
				{
					GameManager.ClientMgr.AddMoney1(client, num2, "勇者战场奖励", true);
				}
				if (list != null)
				{
					foreach (AwardsItemData awardsItemData in list)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, "勇者战场奖励", "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
					}
				}
				if (score >= this.RuntimeData.WarriorBattleLowestJiFen && success > 0)
				{
					GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_YongZheZhanChang, new int[0]));
				}
				result = 1;
			}
			return result;
		}

		public void LeaveFuBen(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				YongZheZhanChangScene yongZheZhanChangScene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out yongZheZhanChangScene))
				{
					yongZheZhanChangScene.m_nPlarerCount--;
				}
			}
		}

		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		public const SceneUIClasses ManagerType = 27;

		private static YongZheZhanChangManager instance = new YongZheZhanChangManager();

		public YongZheZhanChangData RuntimeData = new YongZheZhanChangData();

		public ConcurrentDictionary<int, YongZheZhanChangScene> SceneDict = new ConcurrentDictionary<int, YongZheZhanChangScene>();

		private static long NextHeartBeatTicks = 0L;
	}
}
