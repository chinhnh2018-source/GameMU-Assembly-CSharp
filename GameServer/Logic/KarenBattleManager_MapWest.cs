using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class KarenBattleManager_MapWest : IManager, IEventListener, IEventListenerEx, IManager2
	{
		public static KarenBattleManager_MapWest getInstance()
		{
			return KarenBattleManager_MapWest.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		public bool startup()
		{
			GlobalEventSource4Scene.getInstance().registerListener(33, 41, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(27, 41, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 41, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource.getInstance().registerListener(10, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource.getInstance().registerListener(11, KarenBattleManager_MapWest.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(33, 41, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(27, 41, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 41, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource.getInstance().removeListener(10, KarenBattleManager_MapWest.getInstance());
			GlobalEventSource.getInstance().removeListener(11, KarenBattleManager_MapWest.getInstance());
			return true;
		}

		public bool destroy()
		{
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
				}
			}
			if (eventType == 11)
			{
				MonsterDeadEventObject monsterDeadEventObject = eventObject as MonsterDeadEventObject;
				this.OnProcessMonsterDead(monsterDeadEventObject.getAttacker(), monsterDeadEventObject.getMonster());
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num != 27)
			{
				if (num != 30)
				{
					if (num == 33)
					{
						PreMonsterInjureEventObject preMonsterInjureEventObject = eventObject as PreMonsterInjureEventObject;
						if (preMonsterInjureEventObject != null && preMonsterInjureEventObject.SceneType == 41)
						{
							Monster monster = preMonsterInjureEventObject.Monster;
							if (monster != null)
							{
								if (this.IsQiZhiExtensionID(monster.MonsterInfo.ExtensionID))
								{
									this.RuntimeData.KarenBattleDamage.TryGetValue(monster.MonsterInfo.ExtensionID, out preMonsterInjureEventObject.Injure);
									eventObject.Handled = true;
									eventObject.Result = true;
								}
							}
						}
					}
				}
				else
				{
					OnCreateMonsterEventObject onCreateMonsterEventObject = eventObject as OnCreateMonsterEventObject;
					if (null != onCreateMonsterEventObject)
					{
						KarenBattleQiZhiConfig_West karenBattleQiZhiConfig_West = onCreateMonsterEventObject.Monster.Tag as KarenBattleQiZhiConfig_West;
						if (null != karenBattleQiZhiConfig_West)
						{
							onCreateMonsterEventObject.Monster.Camp = karenBattleQiZhiConfig_West.BattleWhichSide;
							onCreateMonsterEventObject.Result = true;
							onCreateMonsterEventObject.Handled = true;
						}
					}
				}
			}
			else
			{
				ProcessClickOnNpcEventObject processClickOnNpcEventObject = eventObject as ProcessClickOnNpcEventObject;
				if (null != processClickOnNpcEventObject)
				{
					if (null != processClickOnNpcEventObject.Npc)
					{
						int npcID = processClickOnNpcEventObject.Npc.NpcID;
					}
					if (this.OnSpriteClickOnNpc(processClickOnNpcEventObject.Client, processClickOnNpcEventObject.NpcId, processClickOnNpcEventObject.ExtensionID))
					{
						processClickOnNpcEventObject.Result = false;
						processClickOnNpcEventObject.Handled = true;
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
					this.RuntimeData.MapBirthPointDict.Clear();
					text = "Config/LegionsWestBirthPoint.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						KarenBattleBirthPoint karenBattleBirthPoint = new KarenBattleBirthPoint();
						karenBattleBirthPoint.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						karenBattleBirthPoint.PosX = (int)Global.GetSafeAttributeLong(xml, "PosX");
						karenBattleBirthPoint.PosY = (int)Global.GetSafeAttributeLong(xml, "PosY");
						karenBattleBirthPoint.BirthRadius = (int)Global.GetSafeAttributeLong(xml, "BirthRadius");
						this.RuntimeData.MapBirthPointDict[karenBattleBirthPoint.ID] = karenBattleBirthPoint;
					}
					this.RuntimeData.KarenBattleDamage.Clear();
					List<string> paramValueStringListByName = GameManager.systemParamsList.GetParamValueStringListByName("LegionsWest", '|');
					if (null != paramValueStringListByName)
					{
						foreach (string text2 in paramValueStringListByName)
						{
							string[] array = text2.Split(new char[]
							{
								','
							});
							int key = Global.SafeConvertToInt32(array[0]);
							int value = Global.SafeConvertToInt32(array[1]);
							this.RuntimeData.KarenBattleDamage[key] = value;
						}
					}
					this.RuntimeData.NPCID2QiZhiConfigDict.Clear();
					text = "Config/LegionsWest.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						KarenBattleQiZhiConfig_West karenBattleQiZhiConfig_West = new KarenBattleQiZhiConfig_West();
						karenBattleQiZhiConfig_West.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						karenBattleQiZhiConfig_West.QiZhiID = (int)Global.GetSafeAttributeLong(xml, "QiZhiID");
						karenBattleQiZhiConfig_West.QiZuoID = (int)Global.GetSafeAttributeLong(xml, "QiZuoID");
						string safeAttributeStr = Global.GetSafeAttributeStr(xml, "QiZuoSite");
						string[] array2 = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (2 == array2.Length)
						{
							karenBattleQiZhiConfig_West.PosX = Global.SafeConvertToInt32(array2[0]);
							karenBattleQiZhiConfig_West.PosY = Global.SafeConvertToInt32(array2[1]);
						}
						safeAttributeStr = Global.GetSafeAttributeStr(xml, "RebirthSite");
						array2 = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (2 == array2.Length)
						{
							karenBattleQiZhiConfig_West.BirthX = Global.SafeConvertToInt32(array2[0]);
							karenBattleQiZhiConfig_West.BirthY = Global.SafeConvertToInt32(array2[1]);
						}
						karenBattleQiZhiConfig_West.BirthRadius = (int)Global.GetSafeAttributeLong(xml, "RebirthRadius");
						karenBattleQiZhiConfig_West.ProduceTime = (int)Global.GetSafeAttributeLong(xml, "ProduceTime");
						karenBattleQiZhiConfig_West.ProduceNum = (int)Global.GetSafeAttributeLong(xml, "ProduceNum");
						this.RuntimeData.NPCID2QiZhiConfigDict[karenBattleQiZhiConfig_West.QiZuoID] = karenBattleQiZhiConfig_West;
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

		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		private void LeaveFuBen(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				KarenBattleScene karenBattleScene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out karenBattleScene))
				{
					karenBattleScene.m_nPlarerCount--;
				}
				JunTuanClient.getInstance().GameFuBenRoleChangeState(client.ServerId, client.ClientData.RoleID, karenBattleScene.GameId, client.ClientData.BattleWhichSide, 7);
			}
		}

		private void InitScene(KarenBattleScene scene, GameClient client)
		{
			foreach (KarenBattleQiZhiConfig_West karenBattleQiZhiConfig_West in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
			{
				scene.NPCID2QiZhiConfigDict.Add(karenBattleQiZhiConfig_West.QiZuoID, karenBattleQiZhiConfig_West.Clone() as KarenBattleQiZhiConfig_West);
			}
			scene.ScoreData.Clear();
			for (int i = 1; i <= scene.SceneInfo.MaxLegions; i++)
			{
				JunTuanRankData junTuanRankDataBySide = KarenBattleManager.getInstance().GetJunTuanRankDataBySide(i);
				KarenBattleScoreData karenBattleScoreData = new KarenBattleScoreData();
				if (null != junTuanRankDataBySide)
				{
					karenBattleScoreData.LegionID = junTuanRankDataBySide.JunTuanId;
					karenBattleScoreData.Name = junTuanRankDataBySide.JunTuanName;
				}
				scene.ScoreData.Add(karenBattleScoreData);
			}
		}

		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 41)
			{
				lock (this.RuntimeData.Mutex)
				{
					KarenBattleScene karenBattleScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out karenBattleScene);
					this.RuntimeData.FuBenItemData.Remove(karenBattleScene.GameId);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 41)
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
						KarenBattleScene karenBattleScene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqID, out karenBattleScene))
						{
							KarenFuBenData karenFuBenData;
							if (!this.RuntimeData.FuBenItemData.TryGetValue(num, out karenFuBenData))
							{
								LogManager.WriteLog(2, "阿卡伦战场没有为副本找到对应的跨服副本数据,GameID:" + num, null, true);
							}
							KarenBattleSceneInfo karenBattleSceneInfo;
							if (null == (karenBattleSceneInfo = KarenBattleManager.getInstance().TryGetKarenBattleSceneInfo(mapCode)))
							{
								LogManager.WriteLog(2, "阿卡伦战场没有为副本找到对应的档位数据,ID:" + mapCode, null, true);
							}
							karenBattleScene = new KarenBattleScene();
							karenBattleScene.CopyMap = copyMap;
							karenBattleScene.CleanAllInfo();
							karenBattleScene.GameId = num;
							karenBattleScene.m_nMapCode = mapCode;
							karenBattleScene.CopyMapId = copyMap.CopyMapID;
							karenBattleScene.FuBenSeqId = fuBenSeqID;
							karenBattleScene.m_nPlarerCount = 1;
							karenBattleScene.SceneInfo = karenBattleSceneInfo;
							karenBattleScene.MapGridWidth = gameMap.MapGridWidth;
							karenBattleScene.MapGridHeight = gameMap.MapGridHeight;
							DateTime dateTime2 = dateTime.Date.Add(KarenBattleManager.getInstance().GetStartTime(karenBattleSceneInfo.MapCode));
							karenBattleScene.StartTimeTicks = dateTime2.Ticks / 10000L;
							this.InitScene(karenBattleScene, client);
							this.SceneDict[fuBenSeqID] = karenBattleScene;
						}
						else
						{
							karenBattleScene.m_nPlarerCount++;
						}
						KarenBattleClientContextData karenBattleClientContextData;
						if (!karenBattleScene.ClientContextDataDict.TryGetValue(roleID, out karenBattleClientContextData))
						{
							karenBattleClientContextData = new KarenBattleClientContextData
							{
								RoleId = roleID,
								ServerId = client.ServerId,
								BattleWhichSide = client.ClientData.BattleWhichSide
							};
							karenBattleScene.ClientContextDataDict[roleID] = karenBattleClientContextData;
						}
						client.SceneObject = karenBattleScene;
						client.SceneGameId = (long)karenBattleScene.GameId;
						client.SceneContextData2 = karenBattleClientContextData;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(karenBattleScene.SceneInfo.TotalSecs * 1000));
					}
					JunTuanClient.getInstance().GameFuBenRoleChangeState(client.ServerId, roleID, num, client.ClientData.BattleWhichSide, 5);
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void BroadcastSceneScoreInfo(KarenBattleScene scene)
		{
			List<GameClient> clientsList = scene.CopyMap.GetClientsList();
			if (clientsList != null && clientsList.Count > 0)
			{
				for (int i = 0; i < clientsList.Count; i++)
				{
					GameClient gameClient = clientsList[i];
					if (gameClient != null && gameClient.ClientData.CopyMapID == scene.CopyMap.CopyMapID)
					{
						this.NotifyTimeStateInfoAndScoreInfo(gameClient, false, true);
					}
				}
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				KarenBattleScene karenBattleScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out karenBattleScene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, karenBattleScene.StateTimeData, false);
					}
					if (sideScore)
					{
						client.sendCmd<List<KarenBattleScoreData>>(1213, karenBattleScene.ScoreData, false);
					}
				}
			}
		}

		public void CompleteScene(KarenBattleScene scene, int successSide)
		{
			scene.SuccessSide = successSide;
		}

		private void ProcessEnd(KarenBattleScene scene, int successSide, long nowTicks)
		{
			this.CompleteScene(scene, successSide);
			scene.m_eStatus = 3;
			scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
			scene.StateTimeData.GameType = 19;
			scene.StateTimeData.State = 5;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
		}

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= KarenBattleManager_MapWest.NextHeartBeatTicks)
			{
				KarenBattleManager_MapWest.NextHeartBeatTicks = num + 510L;
				foreach (KarenBattleScene karenBattleScene in this.SceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int fuBenSeqId = karenBattleScene.FuBenSeqId;
						int copyMapId = karenBattleScene.CopyMapId;
						int nMapCode = karenBattleScene.m_nMapCode;
						if (fuBenSeqId >= 0 && copyMapId >= 0 && nMapCode >= 0)
						{
							CopyMap copyMap = karenBattleScene.CopyMap;
							DateTime dateTime = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							if (karenBattleScene.m_eStatus == 1 || karenBattleScene.m_eStatus == 2)
							{
							}
							if (karenBattleScene.m_eStatus == 0)
							{
								if (num2 >= karenBattleScene.StartTimeTicks)
								{
									karenBattleScene.m_lPrepareTime = karenBattleScene.StartTimeTicks;
									karenBattleScene.m_lBeginTime = karenBattleScene.m_lPrepareTime + (long)(karenBattleScene.SceneInfo.PrepareSecs * 1000);
									karenBattleScene.m_eStatus = 1;
									karenBattleScene.StateTimeData.GameType = 19;
									karenBattleScene.StateTimeData.State = karenBattleScene.m_eStatus;
									karenBattleScene.StateTimeData.EndTicks = karenBattleScene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, karenBattleScene.StateTimeData, karenBattleScene.CopyMap);
								}
							}
							else if (karenBattleScene.m_eStatus == 1)
							{
								if (num2 >= karenBattleScene.m_lBeginTime)
								{
									karenBattleScene.m_eStatus = 2;
									karenBattleScene.m_lEndTime = karenBattleScene.m_lBeginTime + (long)(karenBattleScene.SceneInfo.FightingSecs * 1000);
									karenBattleScene.StateTimeData.GameType = 19;
									karenBattleScene.StateTimeData.State = karenBattleScene.m_eStatus;
									karenBattleScene.StateTimeData.EndTicks = karenBattleScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, karenBattleScene.StateTimeData, karenBattleScene.CopyMap);
									for (int i = 1; i <= 4; i++)
									{
										GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, i, 0);
									}
								}
							}
							else if (karenBattleScene.m_eStatus == 2)
							{
								if (num2 >= karenBattleScene.m_lEndTime)
								{
									long num3 = long.MaxValue;
									int num4 = 0;
									int successSide = 0;
									for (int j = 0; j < karenBattleScene.ScoreData.Count; j++)
									{
										KarenBattleScoreData karenBattleScoreData = karenBattleScene.ScoreData[j];
										if (karenBattleScoreData.Score > num4)
										{
											successSide = j + 1;
											num3 = karenBattleScoreData.ticks;
											num4 = karenBattleScoreData.Score;
										}
										else if (karenBattleScoreData.Score == num4 && karenBattleScoreData.ticks < num3)
										{
											successSide = j + 1;
											num3 = karenBattleScoreData.ticks;
										}
									}
									this.ProcessEnd(karenBattleScene, successSide, num2);
								}
								else
								{
									this.CheckSceneScoreTime(karenBattleScene, num2);
								}
							}
							else if (karenBattleScene.m_eStatus == 3)
							{
								GameManager.CopyMapMgr.KillAllMonster(karenBattleScene.CopyMap);
								karenBattleScene.m_eStatus = 4;
								JunTuanClient.getInstance().GameFuBenRoleChangeState(-1, -1, karenBattleScene.GameId, -1, 6);
								KarenBattleManager.getInstance().GiveAwards(karenBattleScene);
								KarenFuBenData karenFuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue(karenBattleScene.GameId, out karenFuBenData))
								{
									karenFuBenData.State = 3;
									LogManager.WriteLog(2, string.Format("阿卡伦西战场跨服GameID={0},战斗结束", karenFuBenData.GameId), null, true);
								}
							}
							else if (karenBattleScene.m_eStatus == 4)
							{
								if (num2 >= karenBattleScene.m_lLeaveTime)
								{
									karenBattleScene.m_eStatus = 5;
									try
									{
										List<GameClient> clientsList = copyMap.GetClientsList();
										if (clientsList != null && clientsList.Count > 0)
										{
											for (int k = 0; k < clientsList.Count; k++)
											{
												GameClient gameClient = clientsList[k];
												if (gameClient != null)
												{
													KuaFuManager.getInstance().GotoLastMap(gameClient);
												}
											}
										}
									}
									catch (Exception ex)
									{
										DataHelper.WriteExceptionLogEx(ex, "阿卡伦西战场系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			if (client != null && this.IsQiZhiExtensionID(monster.MonsterInfo.ExtensionID))
			{
				KarenBattleScene karenBattleScene = client.SceneObject as KarenBattleScene;
				KarenBattleQiZhiConfig_West karenBattleQiZhiConfig_West = monster.Tag as KarenBattleQiZhiConfig_West;
				if (karenBattleScene != null && null != karenBattleQiZhiConfig_West)
				{
					lock (this.RuntimeData.Mutex)
					{
						karenBattleScene.ScoreData[karenBattleQiZhiConfig_West.BattleWhichSide - 1].ResourceList.Remove(karenBattleQiZhiConfig_West.ID);
						karenBattleQiZhiConfig_West.DeadTicks = TimeUtil.NOW();
						karenBattleQiZhiConfig_West.Alive = false;
						karenBattleQiZhiConfig_West.BattleWhichSide = client.ClientData.BattleWhichSide;
						karenBattleQiZhiConfig_West.OwnTicks = 0L;
						karenBattleQiZhiConfig_West.OwnTicksDelta = 0L;
						this.BroadcastSceneScoreInfo(karenBattleScene);
					}
				}
			}
		}

		private void CheckSceneScoreTime(KarenBattleScene karenBattleScene, long nowTicks)
		{
			lock (this.RuntimeData.Mutex)
			{
				bool flag2 = false;
				foreach (KeyValuePair<int, KarenBattleQiZhiConfig_West> keyValuePair in karenBattleScene.NPCID2QiZhiConfigDict)
				{
					KarenBattleQiZhiConfig_West value = keyValuePair.Value;
					if (value.BattleWhichSide != 0 && value.Alive)
					{
						value.OwnTicksDelta += nowTicks - value.OwnTicks;
						value.OwnTicks = nowTicks;
						if (value.OwnTicksDelta >= (long)(value.ProduceTime * 1000) && value.ProduceTime > 0)
						{
							int num = (int)(value.OwnTicksDelta / (long)(value.ProduceTime * 1000));
							value.OwnTicksDelta -= (long)(num * value.ProduceTime * 1000);
							karenBattleScene.ScoreData[value.BattleWhichSide - 1].Score += num * value.ProduceNum;
							karenBattleScene.ScoreData[value.BattleWhichSide - 1].ticks = nowTicks;
							flag2 = true;
						}
					}
				}
				if (flag2)
				{
					List<GameClient> clientsList = karenBattleScene.CopyMap.GetClientsList();
					if (clientsList != null && clientsList.Count > 0)
					{
						for (int i = 0; i < clientsList.Count; i++)
						{
							GameClient gameClient = clientsList[i];
							if (gameClient != null && gameClient.ClientData.CopyMapID == karenBattleScene.CopyMap.CopyMapID)
							{
								this.NotifyTimeStateInfoAndScoreInfo(gameClient, false, true);
							}
						}
					}
				}
			}
		}

		public bool OnSpriteClickOnNpc(GameClient client, int npcID, int npcExtentionID)
		{
			KarenBattleQiZhiConfig_West karenBattleQiZhiConfig_West = null;
			bool flag = false;
			bool flag2 = false;
			KarenBattleScene karenBattleScene = client.SceneObject as KarenBattleScene;
			bool result;
			if (null == karenBattleScene)
			{
				result = flag;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (karenBattleScene.NPCID2QiZhiConfigDict.TryGetValue(npcExtentionID, out karenBattleQiZhiConfig_West))
					{
						flag = true;
						if (karenBattleQiZhiConfig_West.Alive)
						{
							return flag;
						}
						if (client.ClientData.BattleWhichSide == karenBattleQiZhiConfig_West.BattleWhichSide || Math.Abs(TimeUtil.NOW() - karenBattleQiZhiConfig_West.DeadTicks) >= 5000L)
						{
							if (karenBattleScene.m_eStatus == 2)
							{
								if (Math.Abs(client.ClientData.PosX - karenBattleQiZhiConfig_West.PosX) <= 1000 && Math.Abs(client.ClientData.PosY - karenBattleQiZhiConfig_West.PosY) <= 1000)
								{
									flag2 = true;
								}
							}
						}
					}
					if (flag2)
					{
						this.InstallJunQi(karenBattleScene, client, karenBattleQiZhiConfig_West);
					}
				}
				result = flag;
			}
			return result;
		}

		public bool IsQiZhiExtensionID(int QiZhiID)
		{
			lock (this.RuntimeData.Mutex)
			{
				foreach (KarenBattleQiZhiConfig_West karenBattleQiZhiConfig_West in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
				{
					if (karenBattleQiZhiConfig_West.QiZhiID == QiZhiID)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void InstallJunQi(KarenBattleScene scene, GameClient client, KarenBattleQiZhiConfig_West item)
		{
			CopyMap copyMap = scene.CopyMap;
			GameMap gameMap = GameManager.MapMgr.GetGameMap(scene.m_nMapCode);
			if (copyMap != null && null != gameMap)
			{
				item.Alive = true;
				item.BattleWhichSide = client.ClientData.BattleWhichSide;
				item.OwnTicks = TimeUtil.NOW();
				scene.ScoreData[item.BattleWhichSide - 1].ResourceList.Add(item.ID);
				GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMap.MapCode, item.QiZhiID, copyMap.CopyMapID, 1, item.PosX / gameMap.MapGridWidth, item.PosY / gameMap.MapGridHeight, 0, 0, 41, item, null);
				SystemXmlItem systemXmlItem = null;
				if (GameManager.SystemNPCsMgr.SystemXmlItemDict.TryGetValue(item.QiZuoID, out systemXmlItem))
				{
					string stringValue = systemXmlItem.GetStringValue("SName");
					string junTuanName = client.ClientData.JunTuanName;
					KarenBattleManager.getInstance().NtfKarenNotifyMsg(scene, KarenNotifyMsgType.Own, client.ClientData.JunTuanId, stringValue, junTuanName);
				}
				this.BroadcastSceneScoreInfo(scene);
			}
		}

		public bool OnInitGame(GameClient client)
		{
			KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			KarenFuBenData karenFuBenData;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.FuBenItemData.TryGetValue((int)clientKuaFuServerLoginData.GameId, out karenFuBenData))
				{
					karenFuBenData = null;
				}
				else if (karenFuBenData.State >= 3)
				{
					return false;
				}
			}
			if (null == karenFuBenData)
			{
				if (KarenBattleManager.getInstance().GMTest)
				{
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.FuBenItemData.TryGetValue((int)clientKuaFuServerLoginData.GameId, out karenFuBenData))
						{
							karenFuBenData = new KarenFuBenData();
							karenFuBenData.GameId = (int)clientKuaFuServerLoginData.GameId;
							karenFuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
							this.RuntimeData.FuBenItemData[karenFuBenData.GameId] = karenFuBenData;
						}
					}
				}
				else
				{
					KarenFuBenData karenKuaFuFuBenData = JunTuanClient.getInstance().GetKarenKuaFuFuBenData((int)clientKuaFuServerLoginData.GameId);
					if (karenKuaFuFuBenData == null || karenKuaFuFuBenData.State == 3)
					{
						LogManager.WriteLog(2, ("获取不到有效的副本数据," + karenKuaFuFuBenData == null) ? "fuBenData == null" : "fuBenData.State == GameFuBenState.End", null, true);
						return false;
					}
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.FuBenItemData.TryGetValue((int)clientKuaFuServerLoginData.GameId, out karenFuBenData))
						{
							karenFuBenData = karenKuaFuFuBenData;
							karenFuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
							this.RuntimeData.FuBenItemData[karenFuBenData.GameId] = karenFuBenData;
						}
					}
				}
			}
			if (KarenBattleManager.getInstance().GMTest)
			{
				client.ClientData.BattleWhichSide = Global.GetRandomNumber(1, 5);
			}
			else
			{
				KarenFuBenRoleData karenFuBenRoleData = JunTuanClient.getInstance().GetKarenFuBenRoleData((int)clientKuaFuServerLoginData.GameId, client.ClientData.RoleID);
				if (null == karenFuBenRoleData)
				{
					return false;
				}
				client.ClientData.BattleWhichSide = karenFuBenRoleData.Side;
			}
			int posX;
			int posY;
			int birthPoint = this.GetBirthPoint(client, out posX, out posY, true);
			bool result;
			if (birthPoint <= 0)
			{
				LogManager.WriteLog(2, "无法获取有效的阵营和出生点,进入跨服失败,side=" + birthPoint, null, true);
				result = false;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					clientKuaFuServerLoginData.FuBenSeqId = karenFuBenData.SequenceId;
					KarenBattleSceneInfo karenBattleSceneInfo;
					if (null == (karenBattleSceneInfo = KarenBattleManager.getInstance().TryGetKarenBattleSceneInfo((int)clientKuaFuServerLoginData.GameId)))
					{
						return false;
					}
					client.ClientData.MapCode = karenBattleSceneInfo.MapCode;
				}
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				client.ClientData.FuBenSeqID = clientKuaFuServerLoginData.FuBenSeqId;
				KarenBattleManager.getInstance().OnInitGame(41, client);
				result = true;
			}
			return result;
		}

		public bool ClientRelive(GameClient client)
		{
			int posX;
			int posY;
			int birthPoint = this.GetBirthPoint(client, out posX, out posY, false);
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

		public int GetBirthPoint(GameClient client, out int posX, out int posY, bool isLogin = false)
		{
			posX = 0;
			posY = 0;
			double num = 0.0;
			int battleWhichSide = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				KarenBattleBirthPoint karenBattleBirthPoint = null;
				if (!this.RuntimeData.MapBirthPointDict.TryGetValue(battleWhichSide, out karenBattleBirthPoint))
				{
					return -1;
				}
				posX = karenBattleBirthPoint.PosX;
				posY = karenBattleBirthPoint.PosY;
				num = Global.GetTwoPointDistance(new Point((double)posX, (double)posY), new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY));
			}
			int result;
			if (isLogin)
			{
				result = battleWhichSide;
			}
			else
			{
				KarenBattleScene karenBattleScene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out karenBattleScene))
				{
					foreach (KeyValuePair<int, KarenBattleQiZhiConfig_West> keyValuePair in karenBattleScene.NPCID2QiZhiConfigDict)
					{
						if (keyValuePair.Value.BattleWhichSide == battleWhichSide)
						{
							double twoPointDistance = Global.GetTwoPointDistance(new Point((double)keyValuePair.Value.BirthX, (double)keyValuePair.Value.BirthY), new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY));
							if (twoPointDistance < num)
							{
								num = twoPointDistance;
								Point mapPointByGridXY = Global.GetMapPointByGridXY(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, keyValuePair.Value.BirthX / karenBattleScene.MapGridWidth, keyValuePair.Value.BirthY / karenBattleScene.MapGridHeight, keyValuePair.Value.BirthRadius / karenBattleScene.MapGridWidth, 0, false);
								posX = (int)mapPointByGridXY.X;
								posY = (int)mapPointByGridXY.Y;
							}
						}
					}
				}
				result = battleWhichSide;
			}
			return result;
		}

		public const SceneUIClasses ManagerType = 41;

		public ConcurrentDictionary<int, KarenBattleScene> SceneDict = new ConcurrentDictionary<int, KarenBattleScene>();

		private static long NextHeartBeatTicks = 0L;

		private static KarenBattleManager_MapWest instance = new KarenBattleManager_MapWest();

		public KarenBattleDataWest RuntimeData = new KarenBattleDataWest();
	}
}
