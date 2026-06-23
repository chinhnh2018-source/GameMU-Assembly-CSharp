using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Reborn;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	public class ZorkBattleManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		public static ZorkBattleManager getInstance()
		{
			return ZorkBattleManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("ZorkBattleManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 2000);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2100, 1, 1, ZorkBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2101, 1, 1, ZorkBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2103, 1, 1, ZorkBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2105, 1, 1, ZorkBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2108, 2, 2, ZorkBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2109, 1, 1, ZorkBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10033, 57, ZorkBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 57, ZorkBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(63, 10000, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(64, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, ZorkBattleManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10033, 57, ZorkBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 57, ZorkBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(63, 10000, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(64, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(28, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, ZorkBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, ZorkBattleManager.getInstance());
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
			if (nID != 2103)
			{
				if (!this.IsGongNengOpened(client, false))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, "", GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return true;
				}
			}
			switch (nID)
			{
			case 2100:
				return this.ProcessGetZorkBattleBaseDataCmd(client, nID, bytes, cmdParams);
			case 2101:
				return this.ProcessZorkBattleEnterCmd(client, nID, bytes, cmdParams);
			case 2103:
				return this.ProcessGetZorkBattleStateCmd(client, nID, bytes, cmdParams);
			case 2105:
				return this.ProcessGetZorkBattleRankInfoCmd(client, nID, bytes, cmdParams);
			case 2108:
				return this.ProcessGetZorkBattleAwardCmd(client, nID, bytes, cmdParams);
			case 2109:
				return this.ProcessZorkBattleJoinCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 28)
			{
				OnStartPlayGameEventObject onStartPlayGameEventObject = eventObject as OnStartPlayGameEventObject;
				this.OnStartPlayGame(onStartPlayGameEventObject.Client);
			}
			else if (eventType == 10)
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
			else if (eventType == 11)
			{
				MonsterDeadEventObject monsterDeadEventObject = eventObject as MonsterDeadEventObject;
				this.OnProcessMonsterDead(monsterDeadEventObject.getAttacker(), monsterDeadEventObject.getMonster());
			}
			else if (eventType == 64)
			{
				this.UpdateChengHaoBuffer(eventObject.Params[0] as GameClient);
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num != 30)
			{
				if (num != 63)
				{
					if (num == 10033)
					{
						this.HandleNtfEnterEvent((eventObject as KFZorkBattleNtfEnterData).Data);
						eventObject.Handled = true;
					}
				}
				else
				{
					PreZhanDuiChangeMemberEventObject preZhanDuiChangeMemberEventObject = (PreZhanDuiChangeMemberEventObject)eventObject;
					preZhanDuiChangeMemberEventObject.Handled = this.OnPreZhanDuiChangeMember(preZhanDuiChangeMemberEventObject);
				}
			}
			else
			{
				OnCreateMonsterEventObject onCreateMonsterEventObject = eventObject as OnCreateMonsterEventObject;
				if (null != onCreateMonsterEventObject)
				{
					ZorkBattleMonsterCreateTag zorkBattleMonsterCreateTag = onCreateMonsterEventObject.Monster.Tag as ZorkBattleMonsterCreateTag;
					if (zorkBattleMonsterCreateTag != null && zorkBattleMonsterCreateTag.monsterTag.ArmyType == ZorkBattleArmyType.Boss)
					{
						int num2 = zorkBattleMonsterCreateTag.monsterTag.RandomBuffID();
						CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(onCreateMonsterEventObject.Monster.CopyMapID);
						ZorkBattleScene zorkBattleScene = null;
						if (copyMap != null && this.SceneDict.TryGetValue(copyMap.FuBenSeqID, out zorkBattleScene))
						{
							this.UpdateBuff4GameScene(zorkBattleScene, onCreateMonsterEventObject.Monster, num2, zorkBattleMonsterCreateTag, true);
							zorkBattleScene.ScoreData.BossBuffID = num2;
							this.BroadScoreInfo(copyMap, -1);
						}
					}
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
					this.RuntimeData.MapBirthPointDict = new Dictionary<int, ZorkBattleBirthPoint>();
					text = "Config/ZorkPlayPoint.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						ZorkBattleBirthPoint zorkBattleBirthPoint = new ZorkBattleBirthPoint();
						zorkBattleBirthPoint.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						string[] array = Global.GetSafeAttributeStr(xml, "MapTeamPoint").Split(new char[]
						{
							','
						});
						if (array.Length == 2)
						{
							zorkBattleBirthPoint.PosX = Global.SafeConvertToInt32(array[0]);
							zorkBattleBirthPoint.PosY = Global.SafeConvertToInt32(array[1]);
						}
						zorkBattleBirthPoint.BirthRadius = (int)Global.GetSafeAttributeLong(xml, "MapTeamRange");
						this.RuntimeData.MapBirthPointDict[zorkBattleBirthPoint.ID] = zorkBattleBirthPoint;
					}
					this.RuntimeData.SceneDataDict = new Dictionary<int, ZorkBattleSceneInfo>();
					text = "Config/ZorkActivityRules.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						ZorkBattleSceneInfo zorkBattleSceneInfo = new ZorkBattleSceneInfo();
						int num = (int)Global.GetSafeAttributeLong(xml, "ID");
						int num2 = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						zorkBattleSceneInfo.Id = num;
						zorkBattleSceneInfo.MapCode = num2;
						zorkBattleSceneInfo.MaxEnterNum = (int)Global.GetSafeAttributeLong(xml, "MaxEnterNum");
						zorkBattleSceneInfo.PrepareSecs = (int)Global.GetSafeAttributeLong(xml, "PrepareSecs");
						zorkBattleSceneInfo.FightingSecs = (int)Global.GetSafeAttributeLong(xml, "FightingSecs");
						zorkBattleSceneInfo.ClearRolesSecs = (int)Global.GetSafeAttributeLong(xml, "ClearRolesSecs");
						zorkBattleSceneInfo.BattleSignSecs = (int)Global.GetSafeAttributeLong(xml, "BattleSignSecs");
						zorkBattleSceneInfo.SignCondition = Global.GetSafeAttributeIntArray(xml, "SignCondition", -1, '|');
						zorkBattleSceneInfo.SeasonFightRound = (int)Global.GetSafeAttributeLong(xml, "SeasonFightDay");
						if (!ConfigParser.ParserTimeRangeListWithDay(zorkBattleSceneInfo.TimePoints, Global.GetSafeAttributeStr(xml, "TimePoints"), true, '|', '-', ','))
						{
							result = false;
							LogManager.WriteLog(1000, string.Format("读取{0}时间配置(TimePoints)出错", text), null, true);
						}
						for (int i = 0; i < zorkBattleSceneInfo.TimePoints.Count; i++)
						{
							TimeSpan timeSpan = new TimeSpan(zorkBattleSceneInfo.TimePoints[i].Hours, zorkBattleSceneInfo.TimePoints[i].Minutes, zorkBattleSceneInfo.TimePoints[i].Seconds);
							zorkBattleSceneInfo.SecondsOfDay.Add(timeSpan.TotalSeconds);
						}
						GameMap gameMap = null;
						if (!GameManager.MapMgr.DictMaps.TryGetValue(num2, out gameMap))
						{
							result = false;
							LogManager.WriteLog(1000, string.Format("地图配置中缺少{0}所需的地图:{1}", text, num2), null, true);
						}
						this.RuntimeData.SceneDataDict[num2] = zorkBattleSceneInfo;
					}
					this.RuntimeData.ZorkBattleArmyList = new List<ZorkBattleArmyConfig>();
					text = "Config/ZorkScene.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						ZorkBattleArmyConfig zorkBattleArmyConfig = new ZorkBattleArmyConfig();
						zorkBattleArmyConfig.ID = (int)Global.GetSafeAttributeLong(xml, "BuffAreID");
						string[] array = Global.GetSafeAttributeStr(xml, "BuffArePlace").Split(new char[]
						{
							'|'
						});
						if (array.Length == 2)
						{
							string[] array2 = array[0].Split(new char[]
							{
								','
							});
							if (array2.Length == 2)
							{
								zorkBattleArmyConfig.PosX = Global.SafeConvertToInt32(array2[0]);
								zorkBattleArmyConfig.PosY = Global.SafeConvertToInt32(array2[1]);
							}
							zorkBattleArmyConfig.PursuitRadius = Global.SafeConvertToInt32(array[1]);
						}
						zorkBattleArmyConfig.Range = (int)Global.GetSafeAttributeLong(xml, "ArmyRefreshRange");
						zorkBattleArmyConfig.ArmyType = (ZorkBattleArmyType)Global.GetSafeAttributeLong(xml, "ArmyType");
						zorkBattleArmyConfig.ArmyGroupRound = Global.GetSafeAttributeIntArray(xml, "ArmyGroupRound", -1, '|');
						zorkBattleArmyConfig.GuardGroupID = Global.GetSafeAttributeIntArray(xml, "GuardGroupID", -1, '|');
						zorkBattleArmyConfig.FirstArmyTime = (int)Global.GetSafeAttributeLong(xml, "FirstArmyTime");
						zorkBattleArmyConfig.NextArmyRefresTime = (int)Global.GetSafeAttributeLong(xml, "NextArmyRefresTime");
						this.RuntimeData.ZorkBattleArmyList.Add(zorkBattleArmyConfig);
					}
					this.RuntimeData.ZorkBattleMonsterDict = new Dictionary<int, List<ZorkBattleMonsterConfig>>();
					text = "Config/ZorkMonster.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						ZorkBattleMonsterConfig zorkBattleMonsterConfig = new ZorkBattleMonsterConfig();
						zorkBattleMonsterConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						zorkBattleMonsterConfig.GroupID = (int)Global.GetSafeAttributeLong(xml, "GroupID");
						zorkBattleMonsterConfig.ArmyType = (ZorkBattleArmyType)Global.GetSafeAttributeLong(xml, "MonsterType");
						zorkBattleMonsterConfig.MonsterId = (int)Global.GetSafeAttributeLong(xml, "MonsterId");
						zorkBattleMonsterConfig.MonsterNum = (int)Global.GetSafeAttributeLong(xml, "MonsterNum");
						zorkBattleMonsterConfig.MonsterDropBuffId = (int)Global.GetSafeAttributeLong(xml, "MonsterDropBuffId");
						zorkBattleMonsterConfig.BuffEffictTime = (int)Global.GetSafeAttributeLong(xml, "BuffEffictTime");
						zorkBattleMonsterConfig.RewardIntegral = (int)Global.GetSafeAttributeLong(xml, "RewardIntegral");
						zorkBattleMonsterConfig.BossBlood = Global.GetSafeAttributeDouble(xml, "BossBlood");
						zorkBattleMonsterConfig.BuffRefreshTime = (int)Global.GetSafeAttributeDouble(xml, "BuffRefreshTime");
						zorkBattleMonsterConfig.BossBuffGroup = Global.GetSafeAttributeIntArray(xml, "BossBuffGroup", -1, '|');
						zorkBattleMonsterConfig.BossBuffRound = Global.GetSafeAttributeIntArray(xml, "BossBuffRound", -1, '|');
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "BossLostSkill"), ref zorkBattleMonsterConfig.BossKillAwardsItemList, '|', ',');
						List<ZorkBattleMonsterConfig> list = null;
						if (!this.RuntimeData.ZorkBattleMonsterDict.TryGetValue(zorkBattleMonsterConfig.GroupID, out list))
						{
							list = new List<ZorkBattleMonsterConfig>();
							this.RuntimeData.ZorkBattleMonsterDict[zorkBattleMonsterConfig.GroupID] = list;
						}
						list.Add(zorkBattleMonsterConfig);
					}
					this.RuntimeData.ZorkAchievementDict = new Dictionary<int, ZorkAchievementConfig>();
					text = "Config/ZorkAchievement.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						ZorkAchievementConfig zorkAchievementConfig = new ZorkAchievementConfig();
						zorkAchievementConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						zorkAchievementConfig.ATarType = (int)Global.GetSafeAttributeLong(xml, "AchievementTarget");
						zorkAchievementConfig.TargetNum = (int)Global.GetSafeAttributeLong(xml, "TargetNum");
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "AchievementReward"), ref zorkAchievementConfig.AAwardsItemList, '|', ',');
						this.RuntimeData.ZorkAchievementDict[zorkAchievementConfig.ID] = zorkAchievementConfig;
					}
					this.RuntimeData.ZorkLevelRangeList = new List<ZorkBattleAwardConfig>();
					text = "Config/ZorkDanAward.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						ZorkBattleAwardConfig zorkBattleAwardConfig = new ZorkBattleAwardConfig();
						zorkBattleAwardConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						zorkBattleAwardConfig.RankValue = (int)Global.GetSafeAttributeLong(xml, "RankValue");
						zorkBattleAwardConfig.WinRankValue = (int)Global.GetSafeAttributeLong(xml, "WinRankValue");
						zorkBattleAwardConfig.LoseRankValue = (int)Global.GetSafeAttributeLong(xml, "LoseRankValue");
						zorkBattleAwardConfig.RankLevel = Global.GetSafeAttributeStr(xml, "RankLevel");
						AwardsItemList firstWinAwardsItemList = new AwardsItemList();
						zorkBattleAwardConfig.FirstWinAwardsItemList = firstWinAwardsItemList;
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "FirstBattleReward"), ref firstWinAwardsItemList, '|', ',');
						AwardsItemList seasonAwardsItemList = new AwardsItemList();
						zorkBattleAwardConfig.SeasonAwardsItemList = seasonAwardsItemList;
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "SeasonReward"), ref seasonAwardsItemList, '|', ',');
						AwardsItemList winAwardsItemList = new AwardsItemList();
						zorkBattleAwardConfig.WinAwardsItemList = winAwardsItemList;
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "WinRankReward"), ref winAwardsItemList, '|', ',');
						AwardsItemList loseAwardsItemList = new AwardsItemList();
						zorkBattleAwardConfig.LoseAwardsItemList = loseAwardsItemList;
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "LoseRankReward"), ref loseAwardsItemList, '|', ',');
						this.RuntimeData.ZorkLevelRangeList.Add(zorkBattleAwardConfig);
					}
					this.RuntimeData.BossHurtCleanTime = (int)GameManager.systemParamsList.GetParamValueIntByName("BossHurtCleanTime", -1);
					this.RuntimeData.ZorkWarEnterMapSet.Clear();
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("ZorkWarEnterMap", ',');
					if (null != paramValueIntArrayByName)
					{
						foreach (int num in paramValueIntArrayByName)
						{
							this.RuntimeData.ZorkWarEnterMapSet.Add(num);
						}
					}
					int[] paramValueIntArrayByName2 = GameManager.systemParamsList.GetParamValueIntArrayByName("ZorkBattleUltraKill", ',');
					if (paramValueIntArrayByName2.Length == 4)
					{
						this.RuntimeData.ZorkBattleUltraKillParam1 = paramValueIntArrayByName2[0];
						this.RuntimeData.ZorkBattleUltraKillParam2 = paramValueIntArrayByName2[1];
						this.RuntimeData.ZorkBattleUltraKillParam3 = paramValueIntArrayByName2[2];
						this.RuntimeData.ZorkBattleUltraKillParam4 = paramValueIntArrayByName2[3];
					}
					paramValueIntArrayByName2 = GameManager.systemParamsList.GetParamValueIntArrayByName("ZorkBattleShutDown", ',');
					if (paramValueIntArrayByName2.Length == 4)
					{
						this.RuntimeData.ZorkBattleShutDownParam1 = paramValueIntArrayByName2[0];
						this.RuntimeData.ZorkBattleShutDownParam2 = paramValueIntArrayByName2[1];
						this.RuntimeData.ZorkBattleShutDownParam3 = paramValueIntArrayByName2[2];
						this.RuntimeData.ZorkBattleShutDownParam4 = paramValueIntArrayByName2[3];
					}
					this.RuntimeData.ZorkEnterPlayNumMin = (int)GameManager.systemParamsList.GetParamValueIntByName("ZorkEnterPlayNum", 4);
					DateTime.TryParse(GameManager.systemParamsList.GetParamValueByName("ZorkStartTime"), out this.RuntimeData.ZorkStartTime);
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
			if (this.IsGongNengOpened(null, false))
			{
				ZorkBattleSyncData zorkBattleSyncData = TianTiClient.getInstance().SyncData_ZorkBattle(TimeUtil.NOW(), this.ZorkBattleSyncDataCache.ZorkBattleRankInfoDict.Age);
				if (null != zorkBattleSyncData)
				{
					lock (this.RuntimeData.Mutex)
					{
						if (this.ZorkBattleSyncDataCache.CurSeasonID != zorkBattleSyncData.CurSeasonID)
						{
							this.ZorkBattleSyncDataCache.CurSeasonID = zorkBattleSyncData.CurSeasonID;
						}
						this.ZorkBattleSyncDataCache.CurRound = zorkBattleSyncData.CurRound;
						if (this.ZorkBattleSyncDataCache.TopZhanDui != zorkBattleSyncData.TopZhanDui)
						{
							this.ZorkBattleSyncDataCache.TopZhanDui = zorkBattleSyncData.TopZhanDui;
							int maxClientCount = GameManager.ClientMgr.GetMaxClientCount();
							for (int i = 0; i < maxClientCount; i++)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClientByNid(i);
								if (null != gameClient)
								{
									this.UpdateChengHaoBuffer(gameClient);
								}
							}
						}
						this.ZorkBattleSyncDataCache.TopZhanDuiName = zorkBattleSyncData.TopZhanDuiName;
						if (this.ZorkBattleSyncDataCache.TopKiller != zorkBattleSyncData.TopKiller)
						{
							this.ZorkBattleSyncDataCache.TopKiller = zorkBattleSyncData.TopKiller;
							int maxClientCount = GameManager.ClientMgr.GetMaxClientCount();
							for (int i = 0; i < maxClientCount; i++)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClientByNid(i);
								if (null != gameClient)
								{
									this.UpdateChengHaoBuffer(gameClient);
								}
							}
						}
						this.ZorkBattleSyncDataCache.DiffKFCenterSeconds = zorkBattleSyncData.DiffKFCenterSeconds;
						if (this.ZorkBattleSyncDataCache.ZorkBattleRankInfoDict.Age != zorkBattleSyncData.ZorkBattleRankInfoDict.Age)
						{
							this.ZorkBattleSyncDataCache.ZorkBattleRankInfoDict = zorkBattleSyncData.ZorkBattleRankInfoDict;
						}
					}
					if (!GameManager.IsKuaFuServer)
					{
						DateTime seasonDateTm = ZorkBattleUtils.GetSeasonDateTm(this.ZorkBattleSyncDataCache.CurSeasonID);
						ZorkBattleSceneInfo zorkBattleSceneInfo = this.RuntimeData.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
						int num = GameManager.GameConfigMgr.GetGameConfigItemInt("ZorkAwardSeasonID", 0);
						if (zorkBattleSceneInfo != null && num != this.ZorkBattleSyncDataCache.CurSeasonID && this.ZorkBattleSyncDataCache.CurRound > zorkBattleSceneInfo.SeasonFightRound)
						{
							num = this.ZorkBattleSyncDataCache.CurSeasonID;
							lock (TianTi5v5Manager.getInstance().RuntimeData.Mutex)
							{
								List<TianTi5v5ZhanDuiMiniData> zhanDuiMiniDataList = TianTi5v5Manager.getInstance().GetZhanDuiMiniDataList(int.MaxValue, GameManager.ServerId);
								if (null != zhanDuiMiniDataList)
								{
									foreach (TianTi5v5ZhanDuiMiniData tianTi5v5ZhanDuiMiniData in zhanDuiMiniDataList)
									{
										TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(tianTi5v5ZhanDuiMiniData.ZhanDuiID, GameManager.ServerId);
										if (zhanDuiData == null || zhanDuiData.ZorkLastFightTime < seasonDateTm)
										{
											LogManager.WriteLog(2, string.Format("魔域夺宝{0}赛季奖励 战队ID:{1} 获取zhanduiData失败", this.ZorkBattleSyncDataCache.CurSeasonID, tianTi5v5ZhanDuiMiniData.ZhanDuiID), null, true);
										}
										else
										{
											LogManager.WriteLog(2, string.Format("魔域夺宝{0}赛季奖励 战队ID:{1} 积分:{2} 成功！", this.ZorkBattleSyncDataCache.CurSeasonID, zhanDuiData.ZhanDuiID, zhanDuiData.ZorkJiFen), null, true);
											ZorkBattleAwardConfig zorkBattleAwardConfigByJiFen = this.GetZorkBattleAwardConfigByJiFen(zhanDuiData.ZorkJiFen);
											if (null != zorkBattleAwardConfigByJiFen)
											{
												AwardsItemList awardsItemList = zorkBattleAwardConfigByJiFen.SeasonAwardsItemList as AwardsItemList;
												if (null != awardsItemList)
												{
													foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in zhanDuiData.teamerList)
													{
														List<GoodsData> goodsData = Global.ConvertToGoodsDataList(awardsItemList.Items, -1);
														string sContent = string.Format(GLang.GetLang(8006, new object[0]), zorkBattleAwardConfigByJiFen.RankLevel);
														Global.UseMailGivePlayerAward3(tianTi5v5ZhanDuiRoleData.RoleID, goodsData, GLang.GetLang(8003, new object[0]), sContent, 0, 0, 0);
													}
												}
											}
										}
									}
									GameManager.GameConfigMgr.SetGameConfigItem("ZorkAwardSeasonID", num.ToString());
									Global.UpdateDBGameConfigg("ZorkAwardSeasonID", num.ToString());
								}
							}
						}
					}
				}
			}
		}

		public List<int> BuildZorkBattleAnalysisData(GameClient client)
		{
			List<int> list = new List<int>(new int[7]);
			List<int> zorkBattleRoleAnalysisData = this.GetZorkBattleRoleAnalysisData(client);
			if (null != zorkBattleRoleAnalysisData)
			{
				list[0] = zorkBattleRoleAnalysisData[2];
				list[1] = zorkBattleRoleAnalysisData[3];
				list[2] = zorkBattleRoleAnalysisData[4];
				list[3] = zorkBattleRoleAnalysisData[5];
				list[4] = zorkBattleRoleAnalysisData[8];
				list[5] = zorkBattleRoleAnalysisData[9];
				list[6] = zorkBattleRoleAnalysisData[10];
			}
			return list;
		}

		public bool ProcessGetZorkBattleBaseDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				ZorkBattleBaseData zorkBattleBaseData = new ZorkBattleBaseData();
				zorkBattleBaseData.listAnalysisData = this.BuildZorkBattleAnalysisData(client);
				Dictionary<int, ZorkAchievementConfig> dictionary = null;
				lock (this.RuntimeData.Mutex)
				{
					dictionary = this.RuntimeData.ZorkAchievementDict;
				}
				if (null != dictionary)
				{
					foreach (KeyValuePair<int, ZorkAchievementConfig> keyValuePair in dictionary)
					{
						zorkBattleBaseData.ArchievementAwardDict[keyValuePair.Key] = 0;
					}
				}
				List<int> zorkBattleAchievementAwardData = this.GetZorkBattleAchievementAwardData(client);
				if (null != zorkBattleAchievementAwardData)
				{
					for (int i = 2; i < zorkBattleAchievementAwardData.Count; i++)
					{
						zorkBattleBaseData.ArchievementAwardDict[zorkBattleAchievementAwardData[i]] = 1;
					}
				}
				TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
				if (null != zhanDuiData)
				{
					ZorkBattleAwardConfig zorkBattleAwardConfigByJiFen = this.GetZorkBattleAwardConfigByJiFen(zhanDuiData.ZorkJiFen);
					if (null != zorkBattleAwardConfigByJiFen)
					{
						zorkBattleBaseData.TeamDuanWei = zorkBattleAwardConfigByJiFen.ID;
					}
				}
				client.sendCmd<ZorkBattleBaseData>(nID, zorkBattleBaseData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessZorkBattleEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData;
				if (!this.IsGongNengOpened(client, true))
				{
					cmdData = -13;
				}
				else
				{
					string kuaFuGameState_ZorkBattle = TianTiClient.getInstance().GetKuaFuGameState_ZorkBattle(client.ClientData.ZhanDuiID);
					if (string.IsNullOrEmpty(kuaFuGameState_ZorkBattle))
					{
						cmdData = -11003;
					}
					else
					{
						int num = Global.SafeConvertToInt32(kuaFuGameState_ZorkBattle);
						if (num == -4036)
						{
							cmdData = -4036;
						}
						else if (num == -4006)
						{
							cmdData = -4006;
						}
						else if (num == -4035)
						{
							cmdData = -4035;
						}
						else
						{
							if (num == -4034)
							{
							}
							ZorkBattleSceneInfo zorkBattleSceneInfo = null;
							ZorkBattleGameStates zorkBattleGameStates = ZorkBattleGameStates.None;
							if (!this.CheckMap(client))
							{
								cmdData = -21;
							}
							else
							{
								cmdData = this.CheckCondition(client, ref zorkBattleSceneInfo, ref zorkBattleGameStates);
								if (zorkBattleGameStates != ZorkBattleGameStates.Start)
								{
									cmdData = -2001;
								}
								else
								{
									KuaFuServerInfo kuaFuServerInfo = null;
									KuaFu5v5FuBenData fuBenDataByZhanDuiId_ZorkBattle = TianTiClient.getInstance().GetFuBenDataByZhanDuiId_ZorkBattle(client.ClientData.ZhanDuiID);
									if (fuBenDataByZhanDuiId_ZorkBattle == null || !KuaFuManager.getInstance().TryGetValue(fuBenDataByZhanDuiId_ZorkBattle.ServerId, out kuaFuServerInfo))
									{
										cmdData = -11000;
									}
									else
									{
										KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
										if (null != clientKuaFuServerLoginData)
										{
											clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
											clientKuaFuServerLoginData.GameId = (long)fuBenDataByZhanDuiId_ZorkBattle.GameId;
											clientKuaFuServerLoginData.GameType = 36;
											clientKuaFuServerLoginData.EndTicks = 0L;
											clientKuaFuServerLoginData.ServerId = client.ServerId;
											clientKuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
											clientKuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
										}
										GlobalNew.RecordSwitchKuaFuServerLog(client);
										client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
									}
								}
							}
						}
					}
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

		public bool ProcessGetZorkBattleStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.CheckOpenState(TimeUtil.NowDateTime()))
				{
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						4,
						this.ZorkBattleSyncDataCache.CurRound,
						this.ZorkBattleSyncDataCache.DiffKFCenterSeconds,
						this.ZorkBattleSyncDataCache.TopZhanDuiName
					}), false);
					return true;
				}
				string kuaFuGameState_ZorkBattle = TianTiClient.getInstance().GetKuaFuGameState_ZorkBattle(client.ClientData.ZhanDuiID);
				if (string.IsNullOrEmpty(kuaFuGameState_ZorkBattle))
				{
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						0,
						this.ZorkBattleSyncDataCache.CurRound,
						this.ZorkBattleSyncDataCache.DiffKFCenterSeconds,
						this.ZorkBattleSyncDataCache.TopZhanDuiName
					}), false);
					return true;
				}
				int num = Global.SafeConvertToInt32(kuaFuGameState_ZorkBattle);
				ZorkBattleSceneInfo zorkBattleSceneInfo = null;
				ZorkBattleGameStates zorkBattleGameStates = ZorkBattleGameStates.None;
				this.CheckCondition(client, ref zorkBattleSceneInfo, ref zorkBattleGameStates);
				if (num == -4036)
				{
					zorkBattleGameStates = ZorkBattleGameStates.Bye;
				}
				else if (num == -4006)
				{
					zorkBattleGameStates = ZorkBattleGameStates.End;
				}
				else if (num == -4034)
				{
					if (zorkBattleGameStates == ZorkBattleGameStates.SignUp || zorkBattleGameStates == ZorkBattleGameStates.Wait)
					{
						zorkBattleGameStates = ZorkBattleGameStates.Wait;
					}
				}
				else if (zorkBattleGameStates == ZorkBattleGameStates.Wait || zorkBattleGameStates == ZorkBattleGameStates.Start)
				{
					zorkBattleGameStates = ZorkBattleGameStates.NotJoin;
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					(int)zorkBattleGameStates,
					this.ZorkBattleSyncDataCache.CurRound,
					this.ZorkBattleSyncDataCache.DiffKFCenterSeconds,
					this.ZorkBattleSyncDataCache.TopZhanDuiName
				}), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetZorkBattleRankInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				ZorkBattleRankInfo zorkBattleRankInfo = new ZorkBattleRankInfo();
				lock (this.RuntimeData.Mutex)
				{
					foreach (KeyValuePair<int, List<KFZorkRankInfo>> keyValuePair in this.ZorkBattleSyncDataCache.ZorkBattleRankInfoDict.V)
					{
						zorkBattleRankInfo.rankInfo2Client[keyValuePair.Key] = new List<KFZorkRankInfo>(keyValuePair.Value);
					}
				}
				client.sendCmd<ZorkBattleRankInfo>(nID, zorkBattleRankInfo, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetZorkBattleAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				int num3 = Global.SafeConvertToInt32(cmdParams[1]);
				ZorkAchievementConfig zorkAchievementConfig;
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.ZorkAchievementDict.TryGetValue(num3, out zorkAchievementConfig))
					{
						num = -3;
						goto IL_21C;
					}
				}
				HashSet<int> hashSet = new HashSet<int>();
				List<int> zorkBattleAchievementAwardData = this.GetZorkBattleAchievementAwardData(client);
				if (null != zorkBattleAchievementAwardData)
				{
					for (int i = 2; i < zorkBattleAchievementAwardData.Count; i++)
					{
						hashSet.Add(zorkBattleAchievementAwardData[i]);
					}
				}
				if (hashSet.Contains(num3))
				{
					num = -200;
				}
				else
				{
					List<int> list = this.BuildZorkBattleAnalysisData(client);
					if (null == list)
					{
						num = -15;
					}
					else if (list[zorkAchievementConfig.ATarType - 1] < zorkAchievementConfig.TargetNum)
					{
						num = -12;
					}
					else
					{
						List<AwardsItemData> items = zorkAchievementConfig.AAwardsItemList.Items;
						if (null != items)
						{
							int num4;
							if (!RebornEquip.MoreIsCanIntoRebornOrBaseBagAward(client, items, out num4))
							{
								if (num4 == 1)
								{
									num = -101;
									goto IL_21C;
								}
								num = -100;
								goto IL_21C;
							}
							else
							{
								foreach (AwardsItemData awardsItemData in items)
								{
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, "魔域夺宝成就奖励", "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
								}
							}
						}
						zorkBattleAchievementAwardData.Add(num3);
						this.SaveZorkBattleAchievementAwardData(client, zorkBattleAchievementAwardData);
					}
				}
				IL_21C:
				client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessZorkBattleJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				if (this.IsGongNengOpened(client, false))
				{
					int zhanDuiID = client.ClientData.ZhanDuiID;
					if (zhanDuiID <= 0)
					{
						num = -4013;
					}
					else if (client.ClientData.ZhanDuiZhiWu != 1)
					{
						num = -4016;
					}
					else
					{
						ZorkBattleSceneInfo zorkBattleSceneInfo = null;
						ZorkBattleGameStates zorkBattleGameStates = ZorkBattleGameStates.None;
						if (!this.CheckMap(client))
						{
							num = -21;
						}
						else
						{
							num = this.CheckCondition(client, ref zorkBattleSceneInfo, ref zorkBattleGameStates);
						}
						TianTi5v5ZhanDuiData zhanDuiData = TianTi5v5Manager.getInstance().GetZhanDuiData(zhanDuiID, GameManager.ServerId);
						if (null == zhanDuiData)
						{
							num = -4013;
						}
						else if (zhanDuiData.teamerList.Count < this.RuntimeData.ZorkEnterPlayNumMin)
						{
							num = -4026;
						}
						else
						{
							int num2 = 0;
							foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in zhanDuiData.teamerList)
							{
								if (client.ClientData.RoleID != tianTi5v5ZhanDuiRoleData.RoleID && tianTi5v5ZhanDuiRoleData.RebornLevel >= zorkBattleSceneInfo.SignCondition[1])
								{
									num2++;
								}
							}
							if (num2 < zorkBattleSceneInfo.SignCondition[0] || client.ClientData.RebornLevel < zorkBattleSceneInfo.SignCondition[1])
							{
								num = -19;
							}
							else
							{
								string kuaFuGameState_ZorkBattle = TianTiClient.getInstance().GetKuaFuGameState_ZorkBattle(zhanDuiID);
								if (string.IsNullOrEmpty(kuaFuGameState_ZorkBattle))
								{
									num = -11003;
								}
								else
								{
									int num3 = Global.SafeConvertToInt32(kuaFuGameState_ZorkBattle);
									if (zorkBattleGameStates != ZorkBattleGameStates.SignUp)
									{
										num = -2001;
									}
									else if (num3 == -4034)
									{
										num = -12;
									}
									if (num >= 0)
									{
										num = TianTiClient.getInstance().SignUp_ZorkBattle(zhanDuiID, GameManager.ServerId);
										if (num >= 0)
										{
											client.ClientData.SignUpGameType = 36;
										}
									}
								}
							}
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

		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			int battleWhichSide = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				ZorkBattleBirthPoint zorkBattleBirthPoint = null;
				if (this.RuntimeData.MapBirthPointDict.TryGetValue(battleWhichSide, out zorkBattleBirthPoint))
				{
					posX = zorkBattleBirthPoint.PosX;
					posY = zorkBattleBirthPoint.PosY;
					return battleWhichSide;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, 114, hint);
		}

		public bool KuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			KuaFu5v5FuBenData fuBenDataByGameId_ZorkBattle = TianTiClient.getInstance().GetFuBenDataByGameId_ZorkBattle((int)kuaFuServerLoginData.GameId);
			bool result;
			if (fuBenDataByGameId_ZorkBattle == null || fuBenDataByGameId_ZorkBattle.ServerId != GameManager.ServerId)
			{
				LogManager.WriteLog(2, string.Format("{0}不具有进入跨服地图{1}的资格", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public bool OnInitGameKuaFu(GameClient client)
		{
			KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			KuaFu5v5FuBenData kuaFu5v5FuBenData;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.FuBenItemData.TryGetValue((int)clientKuaFuServerLoginData.GameId, out kuaFu5v5FuBenData))
				{
					kuaFu5v5FuBenData = null;
				}
				else if (kuaFu5v5FuBenData.State >= 3)
				{
					return false;
				}
			}
			if (null == kuaFu5v5FuBenData)
			{
				KuaFu5v5FuBenData fuBenDataByGameId_ZorkBattle = TianTiClient.getInstance().GetFuBenDataByGameId_ZorkBattle((int)clientKuaFuServerLoginData.GameId);
				if (fuBenDataByGameId_ZorkBattle == null || fuBenDataByGameId_ZorkBattle.State == 3)
				{
					LogManager.WriteLog(2, ("获取不到有效的副本数据," + fuBenDataByGameId_ZorkBattle == null) ? "fuBenData == null" : "fuBenData.State == GameFuBenState.End", null, true);
					return false;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.FuBenItemData.TryGetValue((int)clientKuaFuServerLoginData.GameId, out kuaFu5v5FuBenData))
					{
						kuaFu5v5FuBenData = fuBenDataByGameId_ZorkBattle;
						kuaFu5v5FuBenData.FuBenSeqID = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						this.RuntimeData.FuBenItemData[kuaFu5v5FuBenData.GameId] = kuaFu5v5FuBenData;
					}
				}
			}
			bool result;
			if (kuaFu5v5FuBenData == null || kuaFu5v5FuBenData.State >= 3)
			{
				result = false;
			}
			else
			{
				KuaFuFuBenRoleData kuaFuFuBenRoleData;
				if (kuaFu5v5FuBenData.RoleDict.TryGetValue(client.ClientData.RoleID, out kuaFuFuBenRoleData))
				{
					client.ClientData.BattleWhichSide = kuaFuFuBenRoleData.Side;
				}
				int posX;
				int posY;
				int birthPoint = this.GetBirthPoint(client, out posX, out posY);
				if (birthPoint <= 0)
				{
					result = false;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						clientKuaFuServerLoginData.FuBenSeqId = kuaFu5v5FuBenData.FuBenSeqID;
						ZorkBattleSceneInfo zorkBattleSceneInfo = this.RuntimeData.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
						if (null == zorkBattleSceneInfo)
						{
							return false;
						}
						client.ClientData.MapCode = zorkBattleSceneInfo.MapCode;
					}
					client.ClientData.PosX = posX;
					client.ClientData.PosY = posY;
					client.ClientData.FuBenSeqID = clientKuaFuServerLoginData.FuBenSeqId;
					result = true;
				}
			}
			return result;
		}

		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 57)
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
					int roleId = client.ClientData.RoleID;
					long gameId = Global.GetClientKuaFuServerLoginData(client).GameId;
					DateTime dateTime = TimeUtil.NowDateTime();
					lock (this.RuntimeData.Mutex)
					{
						ZorkBattleScene zorkBattleScene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqID, out zorkBattleScene))
						{
							ZorkBattleSceneInfo zorkBattleSceneInfo = null;
							KuaFu5v5FuBenData kuaFu5v5FuBenData;
							if (!this.RuntimeData.FuBenItemData.TryGetValue((int)gameId, out kuaFu5v5FuBenData))
							{
								LogManager.WriteLog(2, "魔域夺宝没有为副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
							}
							if (!this.RuntimeData.SceneDataDict.TryGetValue(client.ClientData.MapCode, out zorkBattleSceneInfo))
							{
								LogManager.WriteLog(2, "魔域夺宝没有为副本找到对应的场景数据,MapCodeID:" + client.ClientData.MapCode, null, true);
							}
							zorkBattleScene = new ZorkBattleScene();
							zorkBattleScene.CleanAllInfo();
							zorkBattleScene.GameId = (int)gameId;
							zorkBattleScene.CopyMap = copyMap;
							zorkBattleScene.m_nMapCode = mapCode;
							zorkBattleScene.CopyMapId = copyMap.CopyMapID;
							zorkBattleScene.FuBenSeqId = fuBenSeqID;
							zorkBattleScene.SceneInfo = zorkBattleSceneInfo;
							zorkBattleScene.MapGridWidth = gameMap.MapGridWidth;
							zorkBattleScene.MapGridHeight = gameMap.MapGridHeight;
							DateTime dateTime2 = dateTime.Date.Add(this.GetStartTime(zorkBattleSceneInfo.MapCode));
							zorkBattleScene.StartTimeTicks = dateTime2.Ticks / 10000L;
							this.InitScene(zorkBattleScene, client);
							zorkBattleScene.GameStatisticalData.GameId = (int)gameId;
							this.SceneDict[fuBenSeqID] = zorkBattleScene;
							foreach (KeyValuePair<int, string> keyValuePair in kuaFu5v5FuBenData.ZhanDuiNameDict)
							{
								ZorkBattleTeamInfo item = new ZorkBattleTeamInfo
								{
									TeamID = keyValuePair.Key,
									TeamName = keyValuePair.Value
								};
								zorkBattleScene.ScoreData.ZorkBattleTeamList.Add(item);
							}
						}
						if (!zorkBattleScene.GameStatisticalData.ZhanDuiDict.Keys.Contains(client.ClientData.ZhanDuiID))
						{
							TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
							if (null != zhanDuiData)
							{
								zorkBattleScene.GameStatisticalData.ZhanDuiDict[client.ClientData.ZhanDuiID] = zhanDuiData;
								zorkBattleScene.GameStatisticalData.ZhanDuiIDVsServerIDDict[client.ClientData.ZhanDuiID] = client.ServerId;
							}
						}
						List<ZorkBattleRoleInfo> list;
						if (!zorkBattleScene.ClientContextDataDict.TryGetValue(client.ClientData.ZhanDuiID, out list))
						{
							list = new List<ZorkBattleRoleInfo>();
							zorkBattleScene.ClientContextDataDict[client.ClientData.ZhanDuiID] = list;
						}
						ZorkBattleRoleInfo zorkBattleRoleInfo = list.Find((ZorkBattleRoleInfo x) => x.RoleID == roleId);
						if (null == zorkBattleRoleInfo)
						{
							zorkBattleRoleInfo = new ZorkBattleRoleInfo
							{
								RoleID = roleId,
								Name = client.ClientData.RoleName,
								RebornLevel = client.ClientData.RebornLevel,
								RebornCount = client.ClientData.RebornCount,
								ZoneID = client.ClientData.ZoneID,
								Occupation = client.ClientData.Occupation,
								RoleSex = client.ClientData.RoleSex,
								LifeV = client.ClientData.CurrentLifeV,
								MaxLifeV = client.ClientData.LifeV,
								ZhanDuiID = client.ClientData.ZhanDuiID,
								OnLine = true
							};
							list.Add(zorkBattleRoleInfo);
						}
						else
						{
							zorkBattleRoleInfo.Occupation = client.ClientData.Occupation;
							zorkBattleRoleInfo.RoleSex = client.ClientData.RoleSex;
							zorkBattleRoleInfo.LifeV = client.ClientData.CurrentLifeV;
							zorkBattleRoleInfo.MaxLifeV = client.ClientData.LifeV;
							zorkBattleRoleInfo.OnLine = true;
						}
						client.SceneObject = zorkBattleScene;
						client.SceneGameId = (long)zorkBattleScene.GameId;
						client.SceneContextData2 = zorkBattleRoleInfo;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(zorkBattleScene.SceneInfo.TotalSecs * 1000));
					}
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		private void HandleNtfEnterEvent(KuaFu5v5FuBenData data)
		{
			foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
			{
				if (this.IsGongNengOpened(gameClient, false) && this.CheckMap(gameClient))
				{
					if (gameClient != null && data.ZhanDuiDict.Keys.Contains(gameClient.ClientData.ZhanDuiID))
					{
						gameClient.sendCmd<int>(2101, 1, false);
					}
				}
			}
			string arg = string.Join<int>("|", data.ZhanDuiDict.Keys.ToArray<int>());
			LogManager.WriteLog(2, string.Format("通知战队ID={0} 拥有进入魔域夺宝资格", arg), null, true);
		}

		private bool CheckMap(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.ZorkWarEnterMapSet.Contains(client.ClientData.MapCode))
				{
					return false;
				}
			}
			return true;
		}

		public ZorkBattleAwardConfig GetZorkBattleAwardConfigByJiFen(int jifen)
		{
			ZorkBattleAwardConfig zorkBattleAwardConfig = null;
			lock (this.RuntimeData.Mutex)
			{
				foreach (ZorkBattleAwardConfig zorkBattleAwardConfig2 in this.RuntimeData.ZorkLevelRangeList)
				{
					if ((zorkBattleAwardConfig2.RankValue < 0 || jifen >= zorkBattleAwardConfig2.RankValue) && (zorkBattleAwardConfig == null || zorkBattleAwardConfig2.ID > zorkBattleAwardConfig.ID))
					{
						zorkBattleAwardConfig = zorkBattleAwardConfig2;
					}
				}
			}
			return zorkBattleAwardConfig;
		}

		public List<int> GetZorkBattleRoleAnalysisData(int rid, int serverid)
		{
			List<int> result;
			if (0 == this.ZorkBattleSyncDataCache.CurSeasonID)
			{
				result = null;
			}
			else
			{
				List<int> roleParamsIntListFromDBOffline = Global.GetRoleParamsIntListFromDBOffline(rid, "154", serverid);
				this.FilterZorkBattleAnalysisData(roleParamsIntListFromDBOffline);
				result = roleParamsIntListFromDBOffline;
			}
			return result;
		}

		public List<int> GetZorkBattleRoleAnalysisData(GameClient client)
		{
			List<int> result;
			if (0 == this.ZorkBattleSyncDataCache.CurSeasonID)
			{
				result = null;
			}
			else
			{
				List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "154");
				this.FilterZorkBattleAnalysisData(roleParamsIntListFromDB);
				result = roleParamsIntListFromDB;
			}
			return result;
		}

		private void SaveZorkBattleAchievementAwardData(GameClient client, List<int> countList)
		{
			Global.SaveRoleParamsIntListToDB(client, countList, "155", true);
		}

		public List<int> GetZorkBattleAchievementAwardData(GameClient client)
		{
			List<int> result;
			if (0 == this.ZorkBattleSyncDataCache.CurSeasonID)
			{
				result = null;
			}
			else
			{
				List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "155");
				this.FilterZorkBattleAchievementAwardData(roleParamsIntListFromDB);
				result = roleParamsIntListFromDB;
			}
			return result;
		}

		public void FilterZorkBattleAchievementAwardData(List<int> countList)
		{
			if (countList.Count != 3)
			{
				for (int i = countList.Count; i < 3; i++)
				{
					countList.Add(0);
				}
			}
			int offsetDay = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
			if (this.ZorkBattleSyncDataCache.CurSeasonID != countList[0])
			{
				countList[0] = this.ZorkBattleSyncDataCache.CurSeasonID;
				for (int j = 2; j < countList.Count; j++)
				{
					countList[j] = 0;
				}
			}
			if (offsetDay != countList[1])
			{
				countList[1] = offsetDay;
			}
			countList.RemoveAll((int x) => x == 0);
		}

		private void FilterZorkBattleAnalysisData(List<int> countList)
		{
			if (countList.Count != 11)
			{
				for (int i = countList.Count; i < 11; i++)
				{
					countList.Add(0);
				}
			}
			int offsetDay = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
			if (this.ZorkBattleSyncDataCache.CurSeasonID != countList[0])
			{
				countList[0] = this.ZorkBattleSyncDataCache.CurSeasonID;
				countList[2] = 0;
				countList[3] = 0;
				countList[4] = 0;
				countList[5] = 0;
				countList[7] = 0;
				countList[8] = 0;
				countList[9] = 0;
				countList[10] = 0;
			}
			if (offsetDay != countList[1])
			{
				countList[1] = offsetDay;
				countList[6] = 0;
			}
		}

		private void SaveZorkBattleRoleAnalysisDataOffline(int rid, List<int> countList, int serverid)
		{
			Global.SaveRoleParamsIntListToDBOffline(rid, countList, "154", serverid);
		}

		private void SaveZorkBattleRoleAnalysisData(GameClient client, List<int> countList)
		{
			Global.SaveRoleParamsIntListToDB(client, countList, "154", true);
		}

		private void UpdateChengHaoBuffer(GameClient client)
		{
			if (this.ZorkBattleSyncDataCache.TopZhanDui > 0 && client.ClientData.ZhanDuiID == this.ZorkBattleSyncDataCache.TopZhanDui)
			{
				double[] actionParams = new double[]
				{
					1.0
				};
				Global.UpdateBufferData(client, BufferItemTypes.ZorkTopTeam_Title, actionParams, 1, true);
			}
			else
			{
				double[] array = new double[1];
				double[] actionParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.ZorkTopTeam_Title, actionParams, 1, true);
			}
			if (this.ZorkBattleSyncDataCache.TopKiller > 0 && client.ClientData.RoleID == this.ZorkBattleSyncDataCache.TopKiller)
			{
				double[] actionParams = new double[]
				{
					1.0
				};
				Global.UpdateBufferData(client, BufferItemTypes.ZorkTopKiller_Title, actionParams, 1, true);
			}
			else
			{
				double[] array = new double[1];
				double[] actionParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.ZorkTopKiller_Title, actionParams, 1, true);
			}
		}

		public void InitZorkBattleZhanDuiData(TianTi5v5ZhanDuiData zhanduiData)
		{
			if (zhanduiData != null && 0 != this.ZorkBattleSyncDataCache.CurSeasonID)
			{
				DateTime seasonDateTm = ZorkBattleUtils.GetSeasonDateTm(this.ZorkBattleSyncDataCache.CurSeasonID);
				if (zhanduiData.ZorkLastFightTime < seasonDateTm)
				{
					zhanduiData.ZorkJiFen = 0;
					zhanduiData.ZorkBossInjure = 0;
					zhanduiData.ZorkWin = 0;
					zhanduiData.ZorkWinStreak = 0;
				}
			}
		}

		public TianTi5v5ZhanDuiData GetZhanDuiData(int zhanDuiID, int serverID)
		{
			TianTi5v5ZhanDuiData zhanDuiData = TianTi5v5Manager.getInstance().GetZhanDuiData(zhanDuiID, serverID);
			if (null == zhanDuiData)
			{
				LogManager.WriteLog(2, string.Format("获取战队数据失败 ZhanDuiID={0} ServerID={1}", zhanDuiID, serverID), null, true);
			}
			this.InitZorkBattleZhanDuiData(zhanDuiData);
			return zhanDuiData;
		}

		public bool OnPreZhanDuiChangeMember(PreZhanDuiChangeMemberEventObject e)
		{
			lock (this.RuntimeData.Mutex)
			{
				ZorkBattleSceneInfo zorkBattleSceneInfo = null;
				ZorkBattleGameStates zorkBattleGameStates = ZorkBattleGameStates.None;
				this.CheckCondition(null, ref zorkBattleSceneInfo, ref zorkBattleGameStates);
				if (ZorkBattleGameStates.None == zorkBattleGameStates)
				{
					return false;
				}
				string kuaFuGameState_ZorkBattle = TianTiClient.getInstance().GetKuaFuGameState_ZorkBattle(e.ZhanDuiID);
				if (string.IsNullOrEmpty(kuaFuGameState_ZorkBattle))
				{
					return false;
				}
				int num = Global.SafeConvertToInt32(kuaFuGameState_ZorkBattle);
				if (num != -4034)
				{
					return false;
				}
				e.Result = false;
			}
			bool result;
			if (!e.Result)
			{
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(8001, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool CheckOpenState(DateTime now)
		{
			bool result;
			lock (this.RuntimeData.Mutex)
			{
				if (now < this.RuntimeData.ZorkStartTime)
				{
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private void InitScene(ZorkBattleScene scene, GameClient client)
		{
			foreach (ZorkBattleArmyConfig zorkBattleArmyConfig in this.RuntimeData.ZorkBattleArmyList)
			{
				scene.ZorkBattleArmyList.Add(zorkBattleArmyConfig.Clone());
			}
			foreach (KeyValuePair<int, List<ZorkBattleMonsterConfig>> keyValuePair in this.RuntimeData.ZorkBattleMonsterDict)
			{
				List<ZorkBattleMonsterConfig> list = new List<ZorkBattleMonsterConfig>();
				foreach (ZorkBattleMonsterConfig zorkBattleMonsterConfig in keyValuePair.Value)
				{
					list.Add(zorkBattleMonsterConfig.Clone());
				}
				scene.ZorkBattleMonsterDict.Add(keyValuePair.Key, list);
			}
		}

		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 57)
			{
				lock (this.RuntimeData.Mutex)
				{
					ZorkBattleScene zorkBattleScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out zorkBattleScene);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private int CheckCondition(GameClient client, ref ZorkBattleSceneInfo sceneItem, ref ZorkBattleGameStates state)
		{
			int result = 0;
			sceneItem = null;
			lock (this.RuntimeData.Mutex)
			{
				sceneItem = this.RuntimeData.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
				if (null == sceneItem)
				{
					return -12;
				}
			}
			state = ZorkBattleGameStates.None;
			result = 0;
			DateTime dateTime = TimeUtil.NowDateTime();
			double num = dateTime.TimeOfDay.TotalSeconds + (double)this.ZorkBattleSyncDataCache.DiffKFCenterSeconds;
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && num >= sceneItem.SecondsOfDay[i] && num <= sceneItem.SecondsOfDay[i + 1])
					{
						int num2 = sceneItem.BattleSignSecs + sceneItem.PrepareSecs + sceneItem.FightingSecs + sceneItem.ClearRolesSecs;
						int num3 = (int)(sceneItem.SecondsOfDay[i + 1] - sceneItem.SecondsOfDay[i]) / num2;
						for (int j = 0; j < num3; j++)
						{
							int num4 = (int)sceneItem.SecondsOfDay[i] + num2 * j;
							int num5 = num4 + sceneItem.BattleSignSecs;
							int num6 = num5 + num2 - sceneItem.BattleSignSecs;
							if (num >= (double)num4 && num < (double)num5)
							{
								state = ZorkBattleGameStates.SignUp;
							}
							else if (num >= (double)num5 && num < (double)num6)
							{
								state = ZorkBattleGameStates.Start;
							}
						}
						break;
					}
				}
			}
			return result;
		}

		private TimeSpan GetStartTime(int MapCodeID)
		{
			ZorkBattleSceneInfo zorkBattleSceneInfo = null;
			TimeSpan timeSpan = TimeSpan.MinValue;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				zorkBattleSceneInfo = this.RuntimeData.SceneDataDict.Values.FirstOrDefault<ZorkBattleSceneInfo>();
				if (null == zorkBattleSceneInfo)
				{
					goto IL_212;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < zorkBattleSceneInfo.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)zorkBattleSceneInfo.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= zorkBattleSceneInfo.SecondsOfDay[i] && dateTime.TimeOfDay.TotalSeconds <= zorkBattleSceneInfo.SecondsOfDay[i + 1])
					{
						int num = zorkBattleSceneInfo.BattleSignSecs + zorkBattleSceneInfo.PrepareSecs + zorkBattleSceneInfo.FightingSecs + zorkBattleSceneInfo.ClearRolesSecs;
						int num2 = (int)(zorkBattleSceneInfo.SecondsOfDay[i + 1] - zorkBattleSceneInfo.SecondsOfDay[i]) / num;
						for (int j = 0; j < num2; j++)
						{
							int num3 = (int)zorkBattleSceneInfo.SecondsOfDay[i] + num * j;
							int num4 = num3 + zorkBattleSceneInfo.BattleSignSecs;
							int num5 = num4 + num - zorkBattleSceneInfo.BattleSignSecs;
							if (dateTime.TimeOfDay.TotalSeconds >= (double)num3 && dateTime.TimeOfDay.TotalSeconds < (double)num5)
							{
								timeSpan = TimeSpan.FromSeconds((double)num4);
							}
						}
						break;
					}
				}
			}
			IL_212:
			if (timeSpan < TimeSpan.Zero)
			{
				timeSpan = dateTime.TimeOfDay;
			}
			return timeSpan;
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
				ZorkBattleScene zorkBattleScene = client.SceneObject as ZorkBattleScene;
				this.BroadScoreInfo(zorkBattleScene.CopyMap, client.ClientData.ZhanDuiID);
				result = true;
			}
			return result;
		}

		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			ZorkBattleRoleInfo zorkBattleRoleInfo = client.SceneContextData2 as ZorkBattleRoleInfo;
			ZorkBattleScene zorkBattleScene = client.SceneObject as ZorkBattleScene;
			ZorkBattleMonsterCreateTag zorkBattleMonsterCreateTag = monster.Tag as ZorkBattleMonsterCreateTag;
			if (zorkBattleScene != null && zorkBattleMonsterCreateTag != null && null != zorkBattleRoleInfo)
			{
				ZorkBattleTeamInfo zorkBattleTeamInfo = zorkBattleScene.ScoreData.ZorkBattleTeamList.Find((ZorkBattleTeamInfo x) => x.TeamID == client.ClientData.ZhanDuiID);
				if (null != zorkBattleTeamInfo)
				{
					int num = 0;
					lock (this.RuntimeData.Mutex)
					{
						if (zorkBattleScene.m_eStatus != 2)
						{
							return;
						}
						if (zorkBattleMonsterCreateTag.monsterTag.ArmyType == ZorkBattleArmyType.Guard)
						{
							zorkBattleRoleInfo.KillGuardNum++;
							num += zorkBattleMonsterCreateTag.monsterTag.RewardIntegral;
							zorkBattleMonsterCreateTag.armyTag.MonsterDeadNum++;
							if (zorkBattleMonsterCreateTag.armyTag.MonsterDeadNum >= zorkBattleMonsterCreateTag.monsterTag.MonsterNum)
							{
								zorkBattleMonsterCreateTag.armyTag.MonsterDeadNum = 0;
								int key = zorkBattleMonsterCreateTag.armyTag.RandomGroupID();
								List<ZorkBattleMonsterConfig> list = null;
								if (zorkBattleScene.ZorkBattleMonsterDict.TryGetValue(key, out list))
								{
									foreach (ZorkBattleMonsterConfig monsterTag in list)
									{
										ZorkBattleMonsterCreateTag monster2 = new ZorkBattleMonsterCreateTag
										{
											armyTag = zorkBattleMonsterCreateTag.armyTag,
											monsterTag = monsterTag
										};
										long num2 = TimeUtil.NOW() + (long)(zorkBattleMonsterCreateTag.armyTag.NextArmyRefresTime * 1000);
										DateTime dateTime = new DateTime(num2 * 10000L);
										zorkBattleScene.ScoreData.MosterNextTimeDict[zorkBattleMonsterCreateTag.armyTag.ID] = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
										this.AddDelayCreateMonster(zorkBattleScene, num2, monster2);
									}
								}
							}
						}
						else if (zorkBattleMonsterCreateTag.monsterTag.ArmyType == ZorkBattleArmyType.Monster)
						{
							num += zorkBattleMonsterCreateTag.monsterTag.RewardIntegral;
							zorkBattleMonsterCreateTag.armyTag.MonsterDeadNum++;
							if (zorkBattleMonsterCreateTag.armyTag.MonsterDeadNum >= zorkBattleMonsterCreateTag.monsterTag.MonsterNum)
							{
								zorkBattleMonsterCreateTag.armyTag.MonsterDeadNum = 0;
								int key = zorkBattleMonsterCreateTag.armyTag.RandomGroupID();
								List<ZorkBattleMonsterConfig> list = null;
								if (zorkBattleScene.ZorkBattleMonsterDict.TryGetValue(key, out list))
								{
									foreach (ZorkBattleMonsterConfig monsterTag in list)
									{
										ZorkBattleMonsterCreateTag monster2 = new ZorkBattleMonsterCreateTag
										{
											armyTag = zorkBattleMonsterCreateTag.armyTag,
											monsterTag = monsterTag
										};
										long num2 = TimeUtil.NOW() + (long)(zorkBattleMonsterCreateTag.armyTag.NextArmyRefresTime * 1000);
										this.AddDelayCreateMonster(zorkBattleScene, num2, monster2);
									}
								}
							}
						}
						else if (zorkBattleMonsterCreateTag.monsterTag.ArmyType == ZorkBattleArmyType.Boss)
						{
							zorkBattleRoleInfo.BossKillAwardsItemList = zorkBattleMonsterCreateTag.monsterTag.BossKillAwardsItemList;
							zorkBattleRoleInfo.KillBossNum++;
							this.ProcessEnd(zorkBattleScene, true, TimeUtil.NOW());
						}
						zorkBattleRoleInfo.TotalScore += num;
						zorkBattleTeamInfo.JiFen += num;
						if (zorkBattleMonsterCreateTag.monsterTag.MonsterDropBuffId > 0)
						{
							List<ZorkBattleRoleInfo> bufferOwner;
							if (zorkBattleScene.ClientContextDataDict.TryGetValue(client.ClientData.ZhanDuiID, out bufferOwner))
							{
								this.UpdateBuff4GameScene(zorkBattleScene, bufferOwner, zorkBattleMonsterCreateTag.monsterTag.MonsterDropBuffId, zorkBattleMonsterCreateTag, true);
							}
						}
					}
					if (num > 0 && zorkBattleScene != null)
					{
						string msgText = string.Format(GLang.GetLang(8007, new object[0]), num);
						GameManager.ClientMgr.NotifyImportantMsg(client, msgText, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						this.ResortZhanDuiRankByJiFen(zorkBattleScene);
						this.BroadScoreInfo(zorkBattleScene.CopyMap, -1);
					}
				}
			}
		}

		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			if (401 == monster.MonsterType)
			{
				ZorkBattleRoleInfo zorkBattleRoleInfo = client.SceneContextData2 as ZorkBattleRoleInfo;
				ZorkBattleMonsterCreateTag zorkBattleMonsterCreateTag = monster.Tag as ZorkBattleMonsterCreateTag;
				if (zorkBattleRoleInfo != null && zorkBattleMonsterCreateTag != null && zorkBattleMonsterCreateTag.monsterTag.ArmyType == ZorkBattleArmyType.Boss)
				{
					ZorkBattleScene zorkBattleScene = client.SceneObject as ZorkBattleScene;
					if (zorkBattleScene != null && zorkBattleScene.m_eStatus == 2)
					{
						ZorkBattleTeamInfo zorkBattleTeamInfo = zorkBattleScene.ScoreData.ZorkBattleTeamList.Find((ZorkBattleTeamInfo x) => x.TeamID == client.ClientData.ZhanDuiID);
						if (null != zorkBattleTeamInfo)
						{
							int num = 0;
							bool flag = false;
							double num2 = zorkBattleMonsterCreateTag.monsterTag.BossBlood * monster.MonsterInfo.VLifeMax;
							lock (this.RuntimeData.Mutex)
							{
								double num3 = 0.0;
								zorkBattleRoleInfo.InjureBossDeltaDict.TryGetValue(monster.MonsterInfo.ExtensionID, out num3);
								num3 += (double)injure;
								if (num3 >= num2 && num2 > 0.0)
								{
									int num4 = (int)(num3 / num2);
									num3 -= num2 * (double)num4;
									num += zorkBattleMonsterCreateTag.monsterTag.RewardIntegral * num4;
								}
								zorkBattleRoleInfo.InjureBossDeltaDict[monster.MonsterInfo.ExtensionID] = num3;
								zorkBattleRoleInfo.BossInjure += injure;
								zorkBattleRoleInfo.TotalScore += num;
								zorkBattleTeamInfo.JiFen += num;
								zorkBattleTeamInfo.BossInjureTicks = TimeUtil.NOW();
								zorkBattleTeamInfo.BossInjure += injure;
								int bossInjurePct = zorkBattleTeamInfo.BossInjurePct;
								zorkBattleTeamInfo.BossInjurePct = (int)((double)zorkBattleTeamInfo.BossInjure / monster.MonsterInfo.VLifeMax * 100.0);
								if (bossInjurePct != zorkBattleTeamInfo.BossInjurePct)
								{
									flag = true;
								}
							}
							if (num > 0 && zorkBattleScene != null)
							{
								flag = true;
								string msgText = string.Format(GLang.GetLang(8007, new object[0]), num);
								GameManager.ClientMgr.NotifyImportantMsg(client, msgText, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								this.ResortZhanDuiRankByJiFen(zorkBattleScene);
							}
							if (flag)
							{
								this.BroadScoreInfo(zorkBattleScene.CopyMap, -1);
							}
						}
					}
				}
			}
		}

		private void ProcessEnd(ZorkBattleScene scene, bool bossKill, long nowTicks)
		{
			if (scene.m_eStatus < 3)
			{
				if (bossKill)
				{
					scene.ScoreData.ZorkBattleTeamList.Sort(delegate(ZorkBattleTeamInfo left, ZorkBattleTeamInfo right)
					{
						int result;
						if (left.BossInjure > right.BossInjure)
						{
							result = -1;
						}
						else if (left.BossInjure < right.BossInjure)
						{
							result = 1;
						}
						else if (left.BossInjureTicks > right.BossInjureTicks)
						{
							result = 1;
						}
						else if (left.BossInjureTicks < right.BossInjureTicks)
						{
							result = -1;
						}
						else if (left.TeamID > right.TeamID)
						{
							result = -1;
						}
						else if (left.TeamID < right.TeamID)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					scene.GameStatisticalData.ZhanDuiIDWin = scene.ScoreData.ZorkBattleTeamList[0].TeamID;
				}
				else
				{
					this.ResortZhanDuiRankByJiFen(scene);
					scene.GameStatisticalData.ZhanDuiIDWin = scene.ScoreData.ZorkBattleTeamList[0].TeamID;
				}
				foreach (KeyValuePair<int, List<ZorkBattleRoleInfo>> keyValuePair in scene.ClientContextDataDict)
				{
					TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData;
					if (scene.GameStatisticalData.ZhanDuiDict.TryGetValue(keyValuePair.Key, out tianTi5v5ZhanDuiData))
					{
						int num;
						if (scene.GameStatisticalData.ZhanDuiIDVsServerIDDict.TryGetValue(keyValuePair.Key, out num))
						{
							foreach (ZorkBattleRoleInfo zorkBattleRoleInfo in keyValuePair.Value)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(zorkBattleRoleInfo.RoleID);
								List<int> zorkBattleRoleAnalysisData;
								if (gameClient != null)
								{
									zorkBattleRoleAnalysisData = this.GetZorkBattleRoleAnalysisData(gameClient);
								}
								else
								{
									zorkBattleRoleAnalysisData = this.GetZorkBattleRoleAnalysisData(zorkBattleRoleInfo.RoleID, num);
								}
								if (null != zorkBattleRoleAnalysisData)
								{
									List<int> list;
									(list = zorkBattleRoleAnalysisData)[2] = list[2] + zorkBattleRoleInfo.KillRoleNum;
									(list = zorkBattleRoleAnalysisData)[3] = list[3] + 1;
									(list = zorkBattleRoleAnalysisData)[4] = list[4] + zorkBattleRoleInfo.KillGuardNum;
									(list = zorkBattleRoleAnalysisData)[5] = list[5] + zorkBattleRoleInfo.KillBossNum;
									if (zorkBattleRoleInfo.ZhanDuiID == scene.GameStatisticalData.ZhanDuiIDWin)
									{
										(list = zorkBattleRoleAnalysisData)[6] = list[6] + 1;
										(list = zorkBattleRoleAnalysisData)[7] = list[7] + 1;
										if (zorkBattleRoleAnalysisData[7] > zorkBattleRoleAnalysisData[8])
										{
											zorkBattleRoleAnalysisData[8] = zorkBattleRoleAnalysisData[7];
										}
										(list = zorkBattleRoleAnalysisData)[9] = list[9] + 1;
									}
									else
									{
										zorkBattleRoleAnalysisData[7] = 0;
									}
									(list = zorkBattleRoleAnalysisData)[10] = list[10] + (int)(zorkBattleRoleInfo.BossInjure / 10000L);
								}
								if (gameClient != null)
								{
									this.SaveZorkBattleRoleAnalysisData(gameClient, zorkBattleRoleAnalysisData);
								}
								else
								{
									this.SaveZorkBattleRoleAnalysisDataOffline(zorkBattleRoleInfo.RoleID, zorkBattleRoleAnalysisData, num);
								}
								if (zorkBattleRoleInfo.KillRoleNum > 0)
								{
									scene.GameStatisticalData.ClientContextDataList.Add(zorkBattleRoleInfo);
								}
							}
						}
					}
				}
				foreach (TianTi5v5ZhanDuiData zhanduiData2 in scene.GameStatisticalData.ZhanDuiDict.Values)
				{
					TianTi5v5ZhanDuiData zhanduiData = zhanduiData2;
					int num;
					if (scene.GameStatisticalData.ZhanDuiIDVsServerIDDict.TryGetValue(zhanduiData.ZhanDuiID, out num))
					{
						ZorkBattleTeamInfo zorkBattleTeamInfo = scene.ScoreData.ZorkBattleTeamList.Find((ZorkBattleTeamInfo x) => x.TeamID == zhanduiData.ZhanDuiID);
						if (null != zorkBattleTeamInfo)
						{
							ZorkBattleAwardConfig zorkBattleAwardConfigByJiFen = this.GetZorkBattleAwardConfigByJiFen(zhanduiData.ZorkJiFen);
							if (null != zorkBattleAwardConfigByJiFen)
							{
								if (zhanduiData.ZhanDuiID == scene.GameStatisticalData.ZhanDuiIDWin)
								{
									zhanduiData.ZorkWin++;
									zhanduiData.ZorkWinStreak++;
									zhanduiData.ZorkJiFen += zorkBattleAwardConfigByJiFen.WinRankValue;
								}
								else
								{
									zhanduiData.ZorkWinStreak = 0;
									zhanduiData.ZorkJiFen -= zorkBattleAwardConfigByJiFen.LoseRankValue;
								}
								zhanduiData.ZorkBossInjure += (int)(zorkBattleTeamInfo.BossInjure / 10000L);
								zhanduiData.ZorkJiFen = Math.Max(zhanduiData.ZorkJiFen, 0);
								zhanduiData.ZorkLastFightTime = TimeUtil.NowDateTime();
								TianTi5v5Manager.getInstance().UpdateZorkZhanDuiData2DB(zhanduiData, num);
							}
						}
					}
				}
				scene.m_eStatus = 3;
				scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
				scene.StateTimeData.GameType = 36;
				scene.StateTimeData.State = 5;
				scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
				GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
			}
		}

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= ZorkBattleManager.NextHeartBeatTicks)
			{
				ZorkBattleManager.NextHeartBeatTicks = num + 1020L;
				foreach (ZorkBattleScene zorkBattleScene in this.SceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int fuBenSeqId = zorkBattleScene.FuBenSeqId;
						int copyMapId = zorkBattleScene.CopyMapId;
						int nMapCode = zorkBattleScene.m_nMapCode;
						if (fuBenSeqId >= 0 && copyMapId >= 0 && nMapCode >= 0)
						{
							CopyMap copyMap = zorkBattleScene.CopyMap;
							DateTime dateTime = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							if (zorkBattleScene.m_eStatus == 1 || zorkBattleScene.m_eStatus == 2)
							{
								this.CheckCreateDynamicMonster(zorkBattleScene, num2);
							}
							if (zorkBattleScene.m_eStatus == 0)
							{
								if (num2 >= zorkBattleScene.StartTimeTicks)
								{
									zorkBattleScene.m_lPrepareTime = zorkBattleScene.StartTimeTicks;
									zorkBattleScene.m_lBeginTime = zorkBattleScene.m_lPrepareTime + (long)(zorkBattleScene.SceneInfo.PrepareSecs * 1000);
									zorkBattleScene.m_eStatus = 1;
									zorkBattleScene.StateTimeData.GameType = 36;
									zorkBattleScene.StateTimeData.State = zorkBattleScene.m_eStatus;
									zorkBattleScene.StateTimeData.EndTicks = zorkBattleScene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, zorkBattleScene.StateTimeData, zorkBattleScene.CopyMap);
									this.InitCreateDynamicMonster(zorkBattleScene, num2);
								}
							}
							else if (zorkBattleScene.m_eStatus == 1)
							{
								if (num2 >= zorkBattleScene.m_lBeginTime)
								{
									zorkBattleScene.m_eStatus = 2;
									zorkBattleScene.m_lEndTime = zorkBattleScene.m_lBeginTime + (long)(zorkBattleScene.SceneInfo.FightingSecs * 1000);
									zorkBattleScene.StateTimeData.GameType = 36;
									zorkBattleScene.StateTimeData.State = zorkBattleScene.m_eStatus;
									zorkBattleScene.StateTimeData.EndTicks = zorkBattleScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, zorkBattleScene.StateTimeData, zorkBattleScene.CopyMap);
									for (int i = 1; i <= 4; i++)
									{
										GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, i, 0);
									}
								}
							}
							else if (zorkBattleScene.m_eStatus == 2)
							{
								if (num2 >= zorkBattleScene.m_lEndTime)
								{
									this.ProcessEnd(zorkBattleScene, false, num2);
								}
								else
								{
									this.CheckSceneScoreData(zorkBattleScene, num2);
									this.CheckSceneBufferTime(zorkBattleScene, num2);
								}
							}
							else if (zorkBattleScene.m_eStatus == 3)
							{
								zorkBattleScene.m_eStatus = 4;
								this.GiveAwards(zorkBattleScene);
								GameManager.CopyMapMgr.KillAllMonster(zorkBattleScene.CopyMap);
								KuaFu5v5FuBenData kuaFu5v5FuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue(zorkBattleScene.GameId, out kuaFu5v5FuBenData))
								{
									LogManager.WriteLog(2, string.Format("魔域夺宝跨服副本GameID={0},战斗结束", kuaFu5v5FuBenData.GameId), null, true);
									this.RuntimeData.FuBenItemData.Remove(zorkBattleScene.GameId);
								}
							}
							else if (zorkBattleScene.m_eStatus == 4)
							{
								if (num2 >= zorkBattleScene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(zorkBattleScene.m_lLeaveTime);
									zorkBattleScene.m_eStatus = 5;
									try
									{
										List<GameClient> clientsList = copyMap.GetClientsList();
										if (clientsList != null && clientsList.Count > 0)
										{
											for (int j = 0; j < clientsList.Count; j++)
											{
												GameClient gameClient = clientsList[j];
												if (gameClient != null)
												{
													KuaFuManager.getInstance().GotoLastMap(gameClient);
												}
											}
										}
									}
									catch (Exception ex)
									{
										DataHelper.WriteExceptionLogEx(ex, "魔域夺宝系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		private void InitCreateDynamicMonster(ZorkBattleScene scene, long nowMs)
		{
			foreach (ZorkBattleArmyConfig zorkBattleArmyConfig in scene.ZorkBattleArmyList)
			{
				int key = zorkBattleArmyConfig.RandomGroupID();
				List<ZorkBattleMonsterConfig> list = null;
				if (scene.ZorkBattleMonsterDict.TryGetValue(key, out list))
				{
					foreach (ZorkBattleMonsterConfig monsterTag in list)
					{
						ZorkBattleMonsterCreateTag monster = new ZorkBattleMonsterCreateTag
						{
							armyTag = zorkBattleArmyConfig,
							monsterTag = monsterTag
						};
						long num = scene.m_lPrepareTime + (long)(zorkBattleArmyConfig.FirstArmyTime * 1000);
						if (zorkBattleArmyConfig.ArmyType == ZorkBattleArmyType.Boss || zorkBattleArmyConfig.ArmyType == ZorkBattleArmyType.Guard)
						{
							DateTime dateTime = new DateTime(num * 10000L);
							scene.ScoreData.MosterNextTimeDict[zorkBattleArmyConfig.ID] = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
						}
						this.AddDelayCreateMonster(scene, num, monster);
					}
				}
			}
		}

		private void AddDelayCreateMonster(ZorkBattleScene scene, long ticks, object monster)
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

		public void CheckCreateDynamicMonster(ZorkBattleScene scene, long nowMs)
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
							if (obj is ZorkBattleMonsterCreateTag)
							{
								ZorkBattleMonsterCreateTag zorkBattleMonsterCreateTag = obj as ZorkBattleMonsterCreateTag;
								ZorkBattleArmyConfig armyTag = zorkBattleMonsterCreateTag.armyTag;
								ZorkBattleMonsterConfig monsterTag = zorkBattleMonsterCreateTag.monsterTag;
								if (null != armyTag)
								{
									GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, monsterTag.MonsterId, scene.CopyMapId, monsterTag.MonsterNum, armyTag.PosX / scene.MapGridWidth, armyTag.PosY / scene.MapGridHeight, armyTag.Range, armyTag.PursuitRadius, 57, zorkBattleMonsterCreateTag, null);
									if (armyTag.ArmyType == ZorkBattleArmyType.Boss || armyTag.ArmyType == ZorkBattleArmyType.Guard)
									{
										scene.ScoreData.MosterNextTimeDict[armyTag.ID] = "";
									}
								}
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

		public void BroadScoreInfo(CopyMap copyMap, int specZhanDui = -1)
		{
			List<GameClient> clientsList = copyMap.GetClientsList();
			if (clientsList != null && clientsList.Count > 0)
			{
				for (int i = 0; i < clientsList.Count; i++)
				{
					GameClient gameClient = clientsList[i];
					if (specZhanDui <= 0 || gameClient.ClientData.ZhanDuiID == specZhanDui)
					{
						if (gameClient != null && gameClient.ClientData.CopyMapID == copyMap.CopyMapID)
						{
							this.NotifyTimeStateInfoAndScoreInfo(gameClient, false, true);
						}
					}
				}
			}
		}

		public void NotifySpriteInjured(GameClient client)
		{
			ZorkBattleScene zorkBattleScene = client.SceneObject as ZorkBattleScene;
			if (null != zorkBattleScene)
			{
				this.BroadScoreInfo(zorkBattleScene.CopyMap, client.ClientData.ZhanDuiID);
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				ZorkBattleScene zorkBattleScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out zorkBattleScene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, zorkBattleScene.StateTimeData, false);
					}
					if (sideScore)
					{
						ZorkBattleSideScore zorkBattleSideScore = zorkBattleScene.ScoreData.Clone();
						List<ZorkBattleRoleInfo> list;
						if (zorkBattleScene.ClientContextDataDict.TryGetValue(client.ClientData.ZhanDuiID, out list))
						{
							zorkBattleSideScore.ZorkBattleRoleList = list.FindAll((ZorkBattleRoleInfo x) => x.OnLine && x.RoleID != client.ClientData.RoleID);
							using (List<ZorkBattleRoleInfo>.Enumerator enumerator = zorkBattleSideScore.ZorkBattleRoleList.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									ZorkBattleRoleInfo role = enumerator.Current;
									List<GameClient> clientsList = zorkBattleScene.CopyMap.GetClientsList();
									if (clientsList != null && clientsList.Count >= 0)
									{
										GameClient gameClient = clientsList.Find((GameClient x) => x.ClientData.RoleID == role.RoleID);
										if (null != gameClient)
										{
											role.LifeV = gameClient.ClientData.CurrentLifeV;
											role.MaxLifeV = gameClient.ClientData.LifeV;
										}
									}
								}
							}
						}
						client.sendCmd<ZorkBattleSideScore>(2104, zorkBattleSideScore, false);
					}
				}
			}
		}

		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				ZorkBattleScene zorkBattleScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out zorkBattleScene))
				{
					if (zorkBattleScene.m_eStatus == 2)
					{
						int num = 0;
						ZorkBattleRoleInfo zorkBattleRoleInfo = client.SceneContextData2 as ZorkBattleRoleInfo;
						ZorkBattleRoleInfo zorkBattleRoleInfo2 = other.SceneContextData2 as ZorkBattleRoleInfo;
						HuanYingSiYuanLianSha huanYingSiYuanLianSha = null;
						HuanYingSiYuanLianshaOver huanYingSiYuanLianshaOver = null;
						HuanYingSiYuanAddScore huanYingSiYuanAddScore = new HuanYingSiYuanAddScore();
						huanYingSiYuanAddScore.Name = Global.FormatRoleName4(client);
						huanYingSiYuanAddScore.ZoneID = client.ClientData.ZoneID;
						huanYingSiYuanAddScore.Side = client.ClientData.BattleWhichSide;
						huanYingSiYuanAddScore.ByLianShaNum = 1;
						huanYingSiYuanAddScore.RoleId = client.ClientData.RoleID;
						huanYingSiYuanAddScore.Occupation = client.ClientData.Occupation;
						if (null != zorkBattleRoleInfo)
						{
							zorkBattleRoleInfo.KillNum++;
							zorkBattleRoleInfo.KillRoleNum++;
							int num2 = this.RuntimeData.ZorkBattleUltraKillParam1 + zorkBattleRoleInfo.KillNum * this.RuntimeData.ZorkBattleUltraKillParam2;
							num2 = Math.Min(this.RuntimeData.ZorkBattleUltraKillParam4, Math.Max(this.RuntimeData.ZorkBattleUltraKillParam3, num2));
							huanYingSiYuanAddScore.ByLianShaNum = 1;
							huanYingSiYuanLianSha = new HuanYingSiYuanLianSha();
							huanYingSiYuanLianSha.Name = huanYingSiYuanAddScore.Name;
							huanYingSiYuanLianSha.ZoneID = huanYingSiYuanAddScore.ZoneID;
							huanYingSiYuanLianSha.Occupation = huanYingSiYuanAddScore.Occupation;
							huanYingSiYuanLianSha.LianShaType = Math.Min(zorkBattleRoleInfo.KillNum, 30) / 5;
							huanYingSiYuanLianSha.ExtScore = num2;
							huanYingSiYuanLianSha.Side = huanYingSiYuanAddScore.Side;
							num += num2;
							if (zorkBattleRoleInfo.KillNum % 5 != 0)
							{
								huanYingSiYuanLianSha = null;
							}
						}
						if (null != zorkBattleRoleInfo2)
						{
							int num3 = this.RuntimeData.ZorkBattleShutDownParam1 + zorkBattleRoleInfo2.KillNum * this.RuntimeData.ZorkBattleShutDownParam2;
							num3 = Math.Min(this.RuntimeData.ZorkBattleShutDownParam4, Math.Max(this.RuntimeData.ZorkBattleShutDownParam3, num3));
							num += num3;
							if (zorkBattleRoleInfo2.KillNum >= 10)
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
							zorkBattleRoleInfo2.KillNum = 0;
						}
						huanYingSiYuanAddScore.Score = num;
						ZorkBattleTeamInfo zorkBattleTeamInfo = zorkBattleScene.ScoreData.ZorkBattleTeamList.Find((ZorkBattleTeamInfo x) => x.TeamID == client.ClientData.ZhanDuiID);
						if (null != zorkBattleTeamInfo)
						{
							zorkBattleTeamInfo.JiFen += num;
						}
						if (null != zorkBattleRoleInfo)
						{
							zorkBattleRoleInfo.TotalScore += num;
							string msgText = string.Format(GLang.GetLang(8007, new object[0]), num);
							GameManager.ClientMgr.NotifyImportantMsg(client, msgText, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						if (null != huanYingSiYuanLianSha)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianSha>(2106, huanYingSiYuanLianSha, zorkBattleScene.CopyMap);
						}
						if (null != huanYingSiYuanLianshaOver)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianshaOver>(2107, huanYingSiYuanLianshaOver, zorkBattleScene.CopyMap);
						}
						this.ResortZhanDuiRankByJiFen(zorkBattleScene);
						this.BroadScoreInfo(zorkBattleScene.CopyMap, -1);
					}
				}
			}
		}

		public void ResortZhanDuiRankByJiFen(ZorkBattleScene scene)
		{
			bool flag = false;
			try
			{
				object mutex;
				Monitor.Enter(mutex = this.RuntimeData.Mutex, ref flag);
				List<ZorkBattleTeamInfo> ZorkBattleTeamListOld = new List<ZorkBattleTeamInfo>(scene.ScoreData.ZorkBattleTeamList);
				List<ZorkBattleRoleInfo> clientContextDataList;
				scene.ScoreData.ZorkBattleTeamList.Sort(delegate(ZorkBattleTeamInfo left, ZorkBattleTeamInfo right)
				{
					int result;
					if (left.JiFen > right.JiFen)
					{
						result = -1;
					}
					else if (left.JiFen < right.JiFen)
					{
						result = 1;
					}
					else
					{
						int num = ZorkBattleTeamListOld.FindIndex((ZorkBattleTeamInfo x) => x.TeamID == left.TeamID);
						int num2 = ZorkBattleTeamListOld.FindIndex((ZorkBattleTeamInfo x) => x.TeamID == right.TeamID);
						if (num < num2)
						{
							result = -1;
						}
						else if (num > num2)
						{
							result = 1;
						}
						else
						{
							int num3 = 0;
							int num4 = 0;
							if (scene.ClientContextDataDict.TryGetValue(left.TeamID, out clientContextDataList))
							{
								num3 = clientContextDataList.Count;
							}
							if (scene.ClientContextDataDict.TryGetValue(right.TeamID, out clientContextDataList))
							{
								num4 = clientContextDataList.Count;
							}
							if (num3 > num4)
							{
								result = -1;
							}
							else if (num3 < num4)
							{
								result = 1;
							}
							else if (left.TeamID > right.TeamID)
							{
								result = -1;
							}
							else if (left.TeamID < right.TeamID)
							{
								result = 1;
							}
							else
							{
								result = 0;
							}
						}
					}
					return result;
				});
			}
			finally
			{
				if (flag)
				{
					object mutex;
					Monitor.Exit(mutex);
				}
			}
		}

		public void GiveAwards(ZorkBattleScene scene)
		{
			try
			{
				using (Dictionary<int, List<ZorkBattleRoleInfo>>.Enumerator enumerator = scene.ClientContextDataDict.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, List<ZorkBattleRoleInfo>> kvp = enumerator.Current;
						Dictionary<int, TianTi5v5ZhanDuiData> zhanDuiDict = scene.GameStatisticalData.ZhanDuiDict;
						KeyValuePair<int, List<ZorkBattleRoleInfo>> kvp3 = kvp;
						TianTi5v5ZhanDuiData zhanduiData;
						if (zhanDuiDict.TryGetValue(kvp3.Key, out zhanduiData))
						{
							ZorkBattleTeamInfo zorkBattleTeamInfo = scene.ScoreData.ZorkBattleTeamList.Find(delegate(ZorkBattleTeamInfo x)
							{
								int teamID = x.TeamID;
								KeyValuePair<int, List<ZorkBattleRoleInfo>> kvp2 = kvp;
								return teamID == kvp2.Key;
							});
							if (null != zorkBattleTeamInfo)
							{
								int ranknum = scene.ScoreData.ZorkBattleTeamList.FindIndex(delegate(ZorkBattleTeamInfo x)
								{
									int teamID = x.TeamID;
									KeyValuePair<int, List<ZorkBattleRoleInfo>> kvp2 = kvp;
									return teamID == kvp2.Key;
								}) + 1;
								kvp3 = kvp;
								foreach (ZorkBattleRoleInfo zorkBattleRoleInfo in kvp3.Value)
								{
									int success;
									if (zorkBattleRoleInfo.ZhanDuiID == scene.GameStatisticalData.ZhanDuiIDWin)
									{
										success = 1;
									}
									else
									{
										success = 0;
									}
									GameClient gameClient = GameManager.ClientMgr.FindClient(zorkBattleRoleInfo.RoleID);
									if (gameClient != null && gameClient.ClientData.MapCode == scene.m_nMapCode)
									{
										this.NtfCanGetAward(gameClient, success, ranknum, zhanduiData, zorkBattleTeamInfo);
										this.GiveRoleAwards(gameClient, success, zhanduiData, zorkBattleTeamInfo);
									}
								}
							}
						}
					}
				}
				this.PushGameResultData(scene);
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "魔域夺宝系统清场调度异常");
			}
		}

		public void PushGameResultData(ZorkBattleScene scene)
		{
			TianTiClient.getInstance().GameFuBenComplete_ZorkBattle(scene.GameStatisticalData);
		}

		private void NtfCanGetAward(GameClient client, int success, int ranknum, TianTi5v5ZhanDuiData zhanduiData, ZorkBattleTeamInfo teamInfo)
		{
			ZorkBattleRoleInfo zorkBattleRoleInfo = client.SceneContextData2 as ZorkBattleRoleInfo;
			ZorkBattleAwardConfig zorkBattleAwardConfigByJiFen = this.GetZorkBattleAwardConfigByJiFen(zhanduiData.ZorkJiFen);
			if (zorkBattleAwardConfigByJiFen != null && null != zorkBattleRoleInfo)
			{
				ZorkBattleAwardsData zorkBattleAwardsData = new ZorkBattleAwardsData();
				zorkBattleAwardsData.Success = success;
				zorkBattleAwardsData.RankNum = ranknum;
				zorkBattleAwardsData.AwardID = zorkBattleAwardConfigByJiFen.ID;
				zorkBattleAwardsData.SelfJiFen = zorkBattleRoleInfo.TotalScore;
				zorkBattleAwardsData.JiFen = teamInfo.JiFen;
				List<int> zorkBattleRoleAnalysisData = this.GetZorkBattleRoleAnalysisData(client);
				if (null != zorkBattleRoleAnalysisData)
				{
					zorkBattleAwardsData.WinToDay = zorkBattleRoleAnalysisData[6];
				}
				if (null != zorkBattleRoleInfo.BossKillAwardsItemList)
				{
					zorkBattleAwardsData.BossAwardGoodsDataList = (zorkBattleRoleInfo.BossKillAwardsItemList as AwardsItemList).Items;
				}
				client.sendCmd<ZorkBattleAwardsData>(2102, zorkBattleAwardsData, false);
			}
		}

		private int GiveRoleAwards(GameClient client, int success, TianTi5v5ZhanDuiData zhanduiData, ZorkBattleTeamInfo teamInfo)
		{
			ZorkBattleRoleInfo zorkBattleRoleInfo = client.SceneContextData2 as ZorkBattleRoleInfo;
			ZorkBattleAwardConfig zorkBattleAwardConfigByJiFen = this.GetZorkBattleAwardConfigByJiFen(zhanduiData.ZorkJiFen);
			int result;
			if (zorkBattleAwardConfigByJiFen == null || null == zorkBattleRoleInfo)
			{
				result = -5;
			}
			else
			{
				List<AwardsItemData> items;
				string lang;
				if (success > 0)
				{
					items = (zorkBattleAwardConfigByJiFen.WinAwardsItemList as AwardsItemList).Items;
					lang = GLang.GetLang(8004, new object[0]);
				}
				else
				{
					items = (zorkBattleAwardConfigByJiFen.LoseAwardsItemList as AwardsItemList).Items;
					lang = GLang.GetLang(8005, new object[0]);
				}
				string goodsFromWhere = "魔域夺宝奖励";
				int num = 0;
				List<int> zorkBattleRoleAnalysisData = this.GetZorkBattleRoleAnalysisData(client);
				if (null != zorkBattleRoleAnalysisData)
				{
					num = zorkBattleRoleAnalysisData[6];
				}
				List<AwardsItemData> list = new List<AwardsItemData>();
				if (zorkBattleAwardConfigByJiFen.FirstWinAwardsItemList != null && success > 0 && 1 == num)
				{
					list.AddRange((zorkBattleAwardConfigByJiFen.FirstWinAwardsItemList as AwardsItemList).Items);
				}
				else
				{
					list.AddRange(items);
				}
				if (null != zorkBattleRoleInfo.BossKillAwardsItemList)
				{
					list.AddRange((zorkBattleRoleInfo.BossKillAwardsItemList as AwardsItemList).Items);
				}
				int num2;
				if (list != null && !RebornEquip.MoreIsCanIntoRebornOrBaseBagAward(client, list, out num2))
				{
					Global.UseMailGivePlayerAward2(client, list, GLang.GetLang(8002, new object[0]), lang, 0, 0, 0);
				}
				else if (list != null)
				{
					foreach (AwardsItemData awardsItemData in list)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, goodsFromWhere, "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
					}
				}
				result = 1;
			}
			return result;
		}

		public void LeaveFuBen(GameClient client)
		{
			ZorkBattleScene zorkBattleScene = client.SceneObject as ZorkBattleScene;
			if (null != zorkBattleScene)
			{
				ZorkBattleRoleInfo zorkBattleRoleInfo = client.SceneContextData2 as ZorkBattleRoleInfo;
				zorkBattleRoleInfo.OnLine = false;
				this.BroadScoreInfo(zorkBattleScene.CopyMap, -1);
			}
		}

		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		private void CheckSceneScoreData(ZorkBattleScene zorkBattleScene, long nowTicks)
		{
			bool flag = false;
			foreach (ZorkBattleTeamInfo zorkBattleTeamInfo in zorkBattleScene.ScoreData.ZorkBattleTeamList)
			{
				if (nowTicks - zorkBattleTeamInfo.BossInjureTicks > (long)(this.RuntimeData.BossHurtCleanTime * 1000))
				{
					zorkBattleTeamInfo.BossInjure = 0L;
					zorkBattleTeamInfo.BossInjurePct = 0;
					flag = true;
				}
			}
			if (flag)
			{
				this.BroadScoreInfo(zorkBattleScene.CopyMap, -1);
			}
		}

		private void CheckSceneBufferTime(ZorkBattleScene zorkBattleScene, long nowTicks)
		{
			List<ZorkBattleSceneBuff> list = new List<ZorkBattleSceneBuff>();
			lock (this.RuntimeData.Mutex)
			{
				if (zorkBattleScene.m_eStatus == 2)
				{
					if (zorkBattleScene.SceneBuffDict.Count != 0)
					{
						foreach (ZorkBattleSceneBuff zorkBattleSceneBuff in zorkBattleScene.SceneBuffDict.Values)
						{
							if (zorkBattleSceneBuff.EndTicks < nowTicks)
							{
								list.Add(zorkBattleSceneBuff);
							}
						}
						if (list.Count != 0)
						{
							foreach (ZorkBattleSceneBuff zorkBattleSceneBuff in list)
							{
								if (zorkBattleSceneBuff.RoleID != 0)
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(zorkBattleSceneBuff.RoleID);
									if (null != gameClient)
									{
										this.UpdateBuff4GameScene(zorkBattleScene, gameClient, zorkBattleSceneBuff.BuffID, zorkBattleSceneBuff.tagInfo, false);
									}
								}
								else if (zorkBattleSceneBuff.ZhanDuiID != 0)
								{
									List<ZorkBattleRoleInfo> bufferOwner;
									if (zorkBattleScene.ClientContextDataDict.TryGetValue(zorkBattleSceneBuff.ZhanDuiID, out bufferOwner))
									{
										this.UpdateBuff4GameScene(zorkBattleScene, bufferOwner, zorkBattleSceneBuff.BuffID, zorkBattleSceneBuff.tagInfo, false);
									}
								}
								else if (zorkBattleSceneBuff.MonsterID != 0)
								{
									Monster monster = GameManager.MonsterMgr.FindMonster(zorkBattleScene.m_nMapCode, zorkBattleSceneBuff.MonsterID);
									if (null != monster)
									{
										this.UpdateBuff4GameScene(zorkBattleScene, monster, zorkBattleSceneBuff.BuffID, zorkBattleSceneBuff.tagInfo, false);
										ZorkBattleMonsterCreateTag zorkBattleMonsterCreateTag = monster.Tag as ZorkBattleMonsterCreateTag;
										if (null != zorkBattleMonsterCreateTag)
										{
											int num = zorkBattleMonsterCreateTag.monsterTag.RandomBuffID();
											this.UpdateBuff4GameScene(zorkBattleScene, monster, num, zorkBattleMonsterCreateTag, true);
											zorkBattleScene.ScoreData.BossBuffID = num;
											this.BroadScoreInfo(zorkBattleScene.CopyMap, -1);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void OnStartPlayGame(GameClient client)
		{
			ZorkBattleScene zorkBattleScene = client.SceneObject as ZorkBattleScene;
			if (null != zorkBattleScene)
			{
				lock (this.RuntimeData.Mutex)
				{
					long num = TimeUtil.NOW();
					foreach (ZorkBattleSceneBuff zorkBattleSceneBuff in zorkBattleScene.SceneBuffDict.Values)
					{
						if (zorkBattleSceneBuff.ZhanDuiID == client.ClientData.ZhanDuiID)
						{
							if (num < zorkBattleSceneBuff.EndTicks)
							{
								EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(zorkBattleSceneBuff.BuffID);
								if (null != equipPropItem)
								{
									int num2 = (int)((zorkBattleSceneBuff.EndTicks - num) / 1000L);
									if (num2 > 0)
									{
										double[] actionParams = new double[]
										{
											(double)num2,
											(double)zorkBattleSceneBuff.BuffID
										};
										Global.UpdateBufferData(client, (BufferItemTypes)zorkBattleSceneBuff.BuffID, actionParams, 1, true);
										client.ClientData.PropsCacheManager.SetExtProps(new object[]
										{
											PropsSystemTypes.BufferByGoodsProps,
											zorkBattleSceneBuff.BuffID,
											equipPropItem.ExtProps
										});
									}
								}
							}
						}
					}
				}
				this.BroadScoreInfo(zorkBattleScene.CopyMap, -1);
			}
			this.UpdateChengHaoBuffer(client);
		}

		private string BuildSceneBuffKey(object bufferOwner, int bufferGoodsID)
		{
			GameClient gameClient = bufferOwner as GameClient;
			Monster monster = bufferOwner as Monster;
			List<ZorkBattleRoleInfo> list = bufferOwner as List<ZorkBattleRoleInfo>;
			string result = "";
			if (null != list)
			{
				result = string.Format("Team_{0}_{1}", list[0].ZhanDuiID, bufferGoodsID);
			}
			else if (null != gameClient)
			{
				result = string.Format("Role_{0}_{1}", gameClient.ClientData.RoleID, bufferGoodsID);
			}
			else if (null != monster)
			{
				result = string.Format("Monster_{0}_{1}", monster.RoleID, bufferGoodsID);
			}
			return result;
		}

		private void UpdateBuff4GameScene(ZorkBattleScene scene, object bufferOwner, int bufferGoodsID, object tagInfo, bool add)
		{
			try
			{
				GameClient gameClient = bufferOwner as GameClient;
				Monster monster = bufferOwner as Monster;
				List<ZorkBattleRoleInfo> list = bufferOwner as List<ZorkBattleRoleInfo>;
				if (gameClient == null && monster == null && null == list)
				{
					LogManager.WriteLog(2, string.Format("魔域夺宝 BuffGoodsID:{0} 获取Buff拥有者失败", bufferGoodsID), null, true);
				}
				else
				{
					ZorkBattleMonsterCreateTag zorkBattleMonsterCreateTag = tagInfo as ZorkBattleMonsterCreateTag;
					if (null == zorkBattleMonsterCreateTag)
					{
						LogManager.WriteLog(2, string.Format("魔域夺宝 BuffGoodsID:{0} 获取Monster附件信息失败", bufferGoodsID), null, true);
					}
					else
					{
						int num = 0;
						if (null != zorkBattleMonsterCreateTag)
						{
							if (zorkBattleMonsterCreateTag.monsterTag.ArmyType == ZorkBattleArmyType.Boss)
							{
								num = zorkBattleMonsterCreateTag.monsterTag.BuffRefreshTime;
							}
							else
							{
								num = zorkBattleMonsterCreateTag.monsterTag.BuffEffictTime;
							}
						}
						int zhanDuiID = 0;
						int roleID = 0;
						int monsterID = 0;
						if (null != list)
						{
							zhanDuiID = list[0].ZhanDuiID;
						}
						else if (null != gameClient)
						{
							roleID = gameClient.ClientData.RoleID;
						}
						else
						{
							monsterID = monster.RoleID;
						}
						EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(bufferGoodsID);
						if (null == equipPropItem)
						{
							LogManager.WriteLog(2, string.Format("魔域夺宝 BuffGoodsID:{0} 获取属性信息失败", bufferGoodsID), null, true);
						}
						else
						{
							if (add)
							{
								if (null != list)
								{
									foreach (ZorkBattleRoleInfo zorkBattleRoleInfo in list)
									{
										GameClient gameClient2 = GameManager.ClientMgr.FindClient(zorkBattleRoleInfo.RoleID);
										if (gameClient2 != null && gameClient2.ClientData.MapCode == scene.m_nMapCode)
										{
											double[] actionParams = new double[]
											{
												(double)num,
												(double)bufferGoodsID
											};
											Global.UpdateBufferData(gameClient2, (BufferItemTypes)bufferGoodsID, actionParams, 1, true);
											gameClient2.ClientData.PropsCacheManager.SetExtProps(new object[]
											{
												PropsSystemTypes.BufferByGoodsProps,
												bufferGoodsID,
												equipPropItem.ExtProps
											});
										}
									}
								}
								else if (null != gameClient)
								{
									double[] actionParams = new double[]
									{
										(double)num,
										(double)bufferGoodsID
									};
									Global.UpdateBufferData(gameClient, (BufferItemTypes)bufferGoodsID, actionParams, 1, true);
									gameClient.ClientData.PropsCacheManager.SetExtProps(new object[]
									{
										PropsSystemTypes.BufferByGoodsProps,
										bufferGoodsID,
										equipPropItem.ExtProps
									});
								}
								else
								{
									for (int i = 0; i < equipPropItem.ExtProps.Length; i++)
									{
										monster.DynamicData.ExtProps[i] += equipPropItem.ExtProps[i];
									}
								}
								string text = this.BuildSceneBuffKey(bufferOwner, bufferGoodsID);
								scene.SceneBuffDict[text] = new ZorkBattleSceneBuff
								{
									ZhanDuiID = zhanDuiID,
									RoleID = roleID,
									MonsterID = monsterID,
									BuffID = bufferGoodsID,
									EndTicks = TimeUtil.NOW() + (long)(num * 1000),
									tagInfo = tagInfo
								};
								LogManager.WriteLog(2, string.Format("魔域夺宝 BuffKey:{0} Add:{1}", text, add), null, true);
							}
							else
							{
								if (null != list)
								{
									foreach (ZorkBattleRoleInfo zorkBattleRoleInfo in list)
									{
										GameClient gameClient2 = GameManager.ClientMgr.FindClient(zorkBattleRoleInfo.RoleID);
										if (gameClient2 != null && gameClient2.ClientData.MapCode == scene.m_nMapCode)
										{
											Global.RemoveBufferData(gameClient2, bufferGoodsID);
											gameClient2.ClientData.PropsCacheManager.SetExtProps(new object[]
											{
												PropsSystemTypes.BufferByGoodsProps,
												bufferGoodsID,
												PropsCacheManager.ConstExtProps
											});
										}
									}
								}
								else if (null != gameClient)
								{
									Global.RemoveBufferData(gameClient, bufferGoodsID);
									gameClient.ClientData.PropsCacheManager.SetExtProps(new object[]
									{
										PropsSystemTypes.BufferByGoodsProps,
										bufferGoodsID,
										PropsCacheManager.ConstExtProps
									});
								}
								else
								{
									for (int i = 0; i < equipPropItem.ExtProps.Length; i++)
									{
										monster.DynamicData.ExtProps[i] -= equipPropItem.ExtProps[i];
									}
								}
								string text = this.BuildSceneBuffKey(bufferOwner, bufferGoodsID);
								scene.SceneBuffDict.Remove(text);
								LogManager.WriteLog(2, string.Format("魔域夺宝 BuffKey:{0} Add:{1}", text, add), null, true);
							}
							if (null != list)
							{
								foreach (ZorkBattleRoleInfo zorkBattleRoleInfo in list)
								{
									GameClient gameClient2 = GameManager.ClientMgr.FindClient(zorkBattleRoleInfo.RoleID);
									if (gameClient2 != null && gameClient2.ClientData.MapCode == scene.m_nMapCode)
									{
										GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient2);
									}
								}
							}
							else if (null != gameClient)
							{
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public const SceneUIClasses ManagerType = 57;

		private static ZorkBattleManager instance = new ZorkBattleManager();

		public ZorkBattleData RuntimeData = new ZorkBattleData();

		public ZorkBattleSyncData ZorkBattleSyncDataCache = new ZorkBattleSyncData();

		public ConcurrentDictionary<int, ZorkBattleScene> SceneDict = new ConcurrentDictionary<int, ZorkBattleScene>();

		private static long NextHeartBeatTicks = 0L;
	}
}
