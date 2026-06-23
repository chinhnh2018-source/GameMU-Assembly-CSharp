using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
	public class KarenBattleManager_MapEast : IManager, IEventListener, IEventListenerEx, IManager2
	{
		public static KarenBattleManager_MapEast getInstance()
		{
			return KarenBattleManager_MapEast.instance;
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
			GlobalEventSource4Scene.getInstance().registerListener(10002, 42, KarenBattleManager_MapEast.getInstance());
			GlobalEventSource.getInstance().registerListener(10, KarenBattleManager_MapEast.getInstance());
			GlobalEventSource.getInstance().registerListener(31, KarenBattleManager_MapEast.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10002, 42, KarenBattleManager_MapEast.getInstance());
			GlobalEventSource.getInstance().removeListener(10, KarenBattleManager_MapEast.getInstance());
			GlobalEventSource.getInstance().removeListener(31, KarenBattleManager_MapEast.getInstance());
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 31)
			{
				ClientRegionEventObject clientRegionEventObject = eventObject as ClientRegionEventObject;
				if (null != clientRegionEventObject)
				{
					if (clientRegionEventObject.EventType == 1 && clientRegionEventObject.Flag == 1)
					{
						this.SubmitCrystalBuff(clientRegionEventObject.Client, clientRegionEventObject.AreaLuaID);
					}
				}
			}
			if (eventType == 10)
			{
				PlayerDeadEventObject playerDeadEventObject = eventObject as PlayerDeadEventObject;
				if (null != playerDeadEventObject)
				{
					GameClient player = playerDeadEventObject.getPlayer();
					if (null != player)
					{
						KarenBattleScene scene;
						if (this.SceneDict.TryGetValue(player.ClientData.FuBenSeqID, out scene))
						{
							this.RemoveBattleSceneBuffForRole(scene, player);
						}
					}
				}
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num == 10002)
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
					text = "Config/LegionsEastBirthPoint.xml";
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
					this.RuntimeData.KarenCenterConfigDict.Clear();
					text = "Config/LegionsEast.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						KarenCenterConfig karenCenterConfig = new KarenCenterConfig();
						karenCenterConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						karenCenterConfig.NPCID = (int)Global.GetSafeAttributeLong(xml, "NPCID");
						string safeAttributeStr = Global.GetSafeAttributeStr(xml, "NPCSite");
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (3 == array.Length)
						{
							karenCenterConfig.PosX = Global.SafeConvertToInt32(array[0]);
							karenCenterConfig.PosY = Global.SafeConvertToInt32(array[1]);
							karenCenterConfig.Radius = Global.SafeConvertToInt32(array[2]);
						}
						karenCenterConfig.ProduceTime = (int)Global.GetSafeAttributeLong(xml, "ProduceTime");
						karenCenterConfig.ProduceNum = (int)Global.GetSafeAttributeLong(xml, "ProduceNum");
						karenCenterConfig.OccupyTime = (int)Global.GetSafeAttributeLong(xml, "OccupyTime");
						this.RuntimeData.KarenCenterConfigDict[karenCenterConfig.ID] = karenCenterConfig;
					}
					this.RuntimeData.NPCID2QiZhiConfigDict.Clear();
					text = "Config/LegionsEastFlag.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						KarenBattleQiZhiConfig_East karenBattleQiZhiConfig_East = new KarenBattleQiZhiConfig_East();
						karenBattleQiZhiConfig_East.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						karenBattleQiZhiConfig_East.MonsterID = (int)Global.GetSafeAttributeLong(xml, "MonsterID");
						string safeAttributeStr = Global.GetSafeAttributeStr(xml, "Site");
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (2 == array.Length)
						{
							karenBattleQiZhiConfig_East.PosX = Global.SafeConvertToInt32(array[0]);
							karenBattleQiZhiConfig_East.PosY = Global.SafeConvertToInt32(array[1]);
						}
						karenBattleQiZhiConfig_East.BeginTime = (int)Global.GetSafeAttributeLong(xml, "BeginTime");
						karenBattleQiZhiConfig_East.RefreshCD = (int)Global.GetSafeAttributeLong(xml, "RefreshCD");
						karenBattleQiZhiConfig_East.CollectTime = (int)Global.GetSafeAttributeLong(xml, "CollectTime");
						karenBattleQiZhiConfig_East.HandInNum = (int)Global.GetSafeAttributeLong(xml, "HandInNum");
						karenBattleQiZhiConfig_East.HoldTme = (int)Global.GetSafeAttributeLong(xml, "HoldTme");
						karenBattleQiZhiConfig_East.BuffGoodsID = (int)Global.GetSafeAttributeLong(xml, "Icon");
						this.RuntimeData.NPCID2QiZhiConfigDict[karenBattleQiZhiConfig_East.MonsterID] = karenBattleQiZhiConfig_East;
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
				this.RemoveBattleSceneBuffForRole(karenBattleScene, client);
				JunTuanClient.getInstance().GameFuBenRoleChangeState(client.ServerId, client.ClientData.RoleID, karenBattleScene.GameId, client.ClientData.BattleWhichSide, 7);
			}
		}

		private void InitScene(KarenBattleScene scene, GameClient client)
		{
			foreach (KarenCenterConfig karenCenterConfig in this.RuntimeData.KarenCenterConfigDict.Values)
			{
				scene.KarenCenterConfigDict.Add(karenCenterConfig.ID, karenCenterConfig.Clone() as KarenCenterConfig);
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
			if (sceneType == 42)
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
			if (sceneType == 42)
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
			scene.StateTimeData.GameType = 20;
			scene.StateTimeData.State = 5;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
		}

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= KarenBattleManager_MapEast.NextHeartBeatTicks)
			{
				KarenBattleManager_MapEast.NextHeartBeatTicks = num + 510L;
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
								this.CheckCreateDynamicMonster(karenBattleScene, num2);
							}
							if (karenBattleScene.m_eStatus == 0)
							{
								if (num2 >= karenBattleScene.StartTimeTicks)
								{
									karenBattleScene.m_lPrepareTime = karenBattleScene.StartTimeTicks;
									karenBattleScene.m_lBeginTime = karenBattleScene.m_lPrepareTime + (long)(karenBattleScene.SceneInfo.PrepareSecs * 1000);
									karenBattleScene.m_eStatus = 1;
									karenBattleScene.StateTimeData.GameType = 20;
									karenBattleScene.StateTimeData.State = karenBattleScene.m_eStatus;
									karenBattleScene.StateTimeData.EndTicks = karenBattleScene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, karenBattleScene.StateTimeData, karenBattleScene.CopyMap);
									this.InitCreateDynamicMonster(karenBattleScene);
								}
							}
							else if (karenBattleScene.m_eStatus == 1)
							{
								if (num2 >= karenBattleScene.m_lBeginTime)
								{
									karenBattleScene.m_eStatus = 2;
									karenBattleScene.m_lEndTime = karenBattleScene.m_lBeginTime + (long)(karenBattleScene.SceneInfo.FightingSecs * 1000);
									karenBattleScene.StateTimeData.GameType = 20;
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
									this.CheckSceneGameClients(karenBattleScene, num2);
									this.CheckSceneBufferTime(karenBattleScene, num2);
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
									LogManager.WriteLog(2, string.Format("阿卡伦东战场跨服GameID={0},战斗结束", karenFuBenData.GameId), null, true);
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
										DataHelper.WriteExceptionLogEx(ex, "阿卡伦东战场系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		public void AddDelayCreateMonster(KarenBattleScene scene, long ticks, object monster)
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

		public void CheckCreateDynamicMonster(KarenBattleScene scene, long nowMs)
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
							if (obj is KarenBattleQiZhiConfig_East)
							{
								KarenBattleQiZhiConfig_East karenBattleQiZhiConfig_East = obj as KarenBattleQiZhiConfig_East;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, karenBattleQiZhiConfig_East.MonsterID, scene.CopyMapId, 1, karenBattleQiZhiConfig_East.PosX / scene.MapGridWidth, karenBattleQiZhiConfig_East.PosY / scene.MapGridHeight, 0, 0, 42, karenBattleQiZhiConfig_East, null);
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

		private bool CheckSceneCenterState(KarenBattleScene karenBattleScene, KarenCenterConfig center, long nowTicks)
		{
			List<object> list = new List<object>();
			GameManager.ClientMgr.LookupEnemiesInCircle(karenBattleScene.m_nMapCode, karenBattleScene.CopyMapId, center.PosX, center.PosY, center.Radius, list);
			Dictionary<int, GameClient> dictionary = new Dictionary<int, GameClient>();
			int num = 0;
			foreach (object obj in list)
			{
				GameClient gameClient = obj as GameClient;
				if (gameClient.ClientData.CurrentLifeV > 0)
				{
					dictionary[gameClient.ClientData.BattleWhichSide] = gameClient;
				}
			}
			if (dictionary.Count == 1)
			{
				num = dictionary.Keys.FirstOrDefault<int>();
			}
			bool result;
			if (num == 0 || num == center.BattleWhichSide)
			{
				center.OwnCalculateSide = 0L;
				center.OwnCalculateTicks = 0L;
				result = false;
			}
			else if (center.OwnCalculateSide != (long)num)
			{
				center.OwnCalculateSide = (long)num;
				center.OwnCalculateTicks = nowTicks;
				result = false;
			}
			else if (nowTicks - center.OwnCalculateTicks >= (long)(center.OccupyTime * 1000))
			{
				if (center.BattleWhichSide != 0)
				{
					karenBattleScene.ScoreData[center.BattleWhichSide - 1].ResourceList.Remove(center.ID);
				}
				center.OwnTicksDelta = 0L;
				center.OwnTicks = nowTicks;
				center.BattleWhichSide = num;
				karenBattleScene.ScoreData[center.BattleWhichSide - 1].ResourceList.Add(center.ID);
				GameClient gameClient = dictionary.Values.FirstOrDefault<GameClient>();
				SystemXmlItem systemXmlItem = null;
				if (GameManager.SystemNPCsMgr.SystemXmlItemDict.TryGetValue(center.NPCID, out systemXmlItem))
				{
					string stringValue = systemXmlItem.GetStringValue("SName");
					string junTuanName = gameClient.ClientData.JunTuanName;
					KarenBattleManager.getInstance().NtfKarenNotifyMsg(karenBattleScene, KarenNotifyMsgType.Own, gameClient.ClientData.JunTuanId, stringValue, junTuanName);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private void CheckSceneGameClients(KarenBattleScene karenBattleScene, long nowTicks)
		{
			List<GameClient> clientsList = karenBattleScene.CopyMap.GetClientsList();
			if (clientsList != null && clientsList.Count != 0)
			{
				GameMap gameMap = null;
				if (GameManager.MapMgr.DictMaps.TryGetValue(karenBattleScene.m_nMapCode, out gameMap))
				{
					for (int i = 0; i < clientsList.Count; i++)
					{
						GameClient gameClient = clientsList[i];
						if (gameClient != null)
						{
							if (gameMap.InSafeRegionList(gameClient.CurrentGrid))
							{
								if (gameClient.SceneContextData != null)
								{
									this.RemoveBattleSceneBuffForRole(karenBattleScene, gameClient);
								}
							}
						}
					}
				}
			}
		}

		private void CheckSceneScoreTime(KarenBattleScene karenBattleScene, long nowTicks)
		{
			lock (this.RuntimeData.Mutex)
			{
				bool flag2 = false;
				foreach (KeyValuePair<int, KarenCenterConfig> keyValuePair in karenBattleScene.KarenCenterConfigDict)
				{
					KarenCenterConfig value = keyValuePair.Value;
					flag2 |= this.CheckSceneCenterState(karenBattleScene, value, nowTicks);
					if (value.BattleWhichSide != 0)
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
					this.BroadcastSceneScoreInfo(karenBattleScene);
				}
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
			int birthPoint = this.GetBirthPoint(client, out posX, out posY);
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
				KarenBattleManager.getInstance().OnInitGame(42, client);
				result = true;
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

		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			posX = 0;
			posY = 0;
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
				double twoPointDistance = Global.GetTwoPointDistance(new Point((double)posX, (double)posY), new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY));
			}
			return battleWhichSide;
		}

		public int GetCaiJiMonsterTime(GameClient client, Monster monster)
		{
			KarenBattleQiZhiConfig_East karenBattleQiZhiConfig_East = (monster != null) ? (monster.Tag as KarenBattleQiZhiConfig_East) : null;
			int result;
			if (karenBattleQiZhiConfig_East == null)
			{
				result = -200;
			}
			else
			{
				result = karenBattleQiZhiConfig_East.CollectTime;
			}
			return result;
		}

		public void RemoveBattleSceneBuffForRole(KarenBattleScene scene, GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (scene.SceneBuffDict.Count != 0)
				{
					List<KarenBattleSceneBuff> list = new List<KarenBattleSceneBuff>();
					foreach (KarenBattleSceneBuff karenBattleSceneBuff in scene.SceneBuffDict.Values)
					{
						if (karenBattleSceneBuff.RoleID == client.ClientData.RoleID)
						{
							list.Add(karenBattleSceneBuff);
						}
					}
					if (list.Count != 0)
					{
						foreach (KarenBattleSceneBuff karenBattleSceneBuff in list)
						{
							if (karenBattleSceneBuff.RoleID != 0)
							{
								this.UpdateBuff4GameClient(client, karenBattleSceneBuff.BuffID, karenBattleSceneBuff.tagInfo, false);
							}
							KarenBattleQiZhiConfig_East karenBattleQiZhiConfig_East = karenBattleSceneBuff.tagInfo as KarenBattleQiZhiConfig_East;
							if (null != karenBattleQiZhiConfig_East)
							{
								this.AddDelayCreateMonster(scene, TimeUtil.NOW() + (long)(karenBattleQiZhiConfig_East.RefreshCD * 1000), karenBattleSceneBuff.tagInfo);
							}
						}
					}
				}
			}
		}

		private string BuildSceneBuffKey(GameClient client, int bufferGoodsID)
		{
			return string.Format("{0}_{1}", client.ClientData.RoleID, bufferGoodsID);
		}

		private void UpdateBuff4GameClient(GameClient client, int bufferGoodsID, object tagInfo, bool add)
		{
			try
			{
				KarenBattleQiZhiConfig_East karenBattleQiZhiConfig_East = tagInfo as KarenBattleQiZhiConfig_East;
				if (null != karenBattleQiZhiConfig_East)
				{
					int num = 0;
					BufferItemTypes bufferItemTypes = BufferItemTypes.None;
					if (null != karenBattleQiZhiConfig_East)
					{
						num = karenBattleQiZhiConfig_East.HoldTme;
						bufferItemTypes = BufferItemTypes.KarenEastCrystal;
					}
					KarenBattleScene karenBattleScene;
					if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out karenBattleScene))
					{
						EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(bufferGoodsID);
						if (null != equipPropItem)
						{
							if (add)
							{
								double[] actionParams = new double[]
								{
									(double)num,
									(double)bufferGoodsID
								};
								Global.UpdateBufferData(client, bufferItemTypes, actionParams, 1, true);
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.BufferByGoodsProps,
									bufferGoodsID,
									equipPropItem.ExtProps
								});
								string key = this.BuildSceneBuffKey(client, bufferGoodsID);
								karenBattleScene.SceneBuffDict[key] = new KarenBattleSceneBuff
								{
									RoleID = client.ClientData.RoleID,
									BuffID = bufferGoodsID,
									EndTicks = TimeUtil.NOW() + (long)(num * 1000),
									tagInfo = tagInfo
								};
								if (bufferItemTypes == BufferItemTypes.KarenEastCrystal)
								{
									client.SceneContextData = tagInfo;
								}
							}
							else
							{
								double[] array = new double[2];
								double[] actionParams = array;
								Global.UpdateBufferData(client, bufferItemTypes, actionParams, 1, true);
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.BufferByGoodsProps,
									bufferGoodsID,
									PropsCacheManager.ConstExtProps
								});
								Global.RemoveBufferData(client, (int)bufferItemTypes);
								string key = this.BuildSceneBuffKey(client, bufferGoodsID);
								karenBattleScene.SceneBuffDict.Remove(key);
								if (bufferItemTypes == BufferItemTypes.KarenEastCrystal)
								{
									client.SceneContextData = null;
								}
							}
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void OnCaiJiFinish(GameClient client, Monster monster)
		{
			lock (this.RuntimeData.Mutex)
			{
				KarenBattleScene karenBattleScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out karenBattleScene))
				{
					if (karenBattleScene.m_eStatus == 2)
					{
						KarenBattleQiZhiConfig_East karenBattleQiZhiConfig_East = monster.Tag as KarenBattleQiZhiConfig_East;
						if (karenBattleQiZhiConfig_East != null)
						{
							KarenBattleQiZhiConfig_East karenBattleQiZhiConfig_East2 = client.SceneContextData as KarenBattleQiZhiConfig_East;
							if (null != karenBattleQiZhiConfig_East2)
							{
								this.AddDelayCreateMonster(karenBattleScene, TimeUtil.NOW() + (long)(karenBattleQiZhiConfig_East2.RefreshCD * 1000), karenBattleQiZhiConfig_East2);
							}
							this.UpdateBuff4GameClient(client, karenBattleQiZhiConfig_East.BuffGoodsID, karenBattleQiZhiConfig_East, true);
						}
					}
				}
			}
		}

		public void SubmitCrystalBuff(GameClient client, int areaLuaID)
		{
			KarenBattleQiZhiConfig_East karenBattleQiZhiConfig_East = client.SceneContextData as KarenBattleQiZhiConfig_East;
			if (null != karenBattleQiZhiConfig_East)
			{
				lock (this.RuntimeData.Mutex)
				{
					KarenBattleScene karenBattleScene;
					if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out karenBattleScene))
					{
						KarenCenterConfig karenCenterConfig = null;
						if (karenBattleScene.KarenCenterConfigDict.TryGetValue(areaLuaID, out karenCenterConfig))
						{
							if (karenCenterConfig.BattleWhichSide == client.ClientData.BattleWhichSide)
							{
								KarenBattleClientContextData karenBattleClientContextData = client.SceneContextData2 as KarenBattleClientContextData;
								if (karenBattleClientContextData != null && karenBattleScene.m_eStatus == 2)
								{
									int handInNum = karenBattleQiZhiConfig_East.HandInNum;
									karenBattleScene.ScoreData[client.ClientData.BattleWhichSide - 1].Score += handInNum;
									karenBattleScene.ScoreData[client.ClientData.BattleWhichSide - 1].ticks = TimeUtil.NOW();
									if (handInNum > 0)
									{
										this.NotifyTimeStateInfoAndScoreInfo(client, false, true);
									}
								}
								SystemXmlItem systemXmlItem = null;
								if (GameManager.SystemNPCsMgr.SystemXmlItemDict.TryGetValue(karenCenterConfig.NPCID, out systemXmlItem))
								{
									string junTuanName = client.ClientData.JunTuanName;
									string stringValue = systemXmlItem.GetStringValue("SName");
									KarenBattleManager.getInstance().NtfKarenNotifyMsg(karenBattleScene, KarenNotifyMsgType.Submit, client.ClientData.JunTuanId, junTuanName, stringValue);
								}
								this.UpdateBuff4GameClient(client, karenBattleQiZhiConfig_East.BuffGoodsID, karenBattleQiZhiConfig_East, false);
								this.AddDelayCreateMonster(karenBattleScene, TimeUtil.NOW() + (long)(karenBattleQiZhiConfig_East.RefreshCD * 1000), karenBattleQiZhiConfig_East);
							}
						}
					}
				}
			}
		}

		private void InitCreateDynamicMonster(KarenBattleScene scene)
		{
			foreach (KarenBattleQiZhiConfig_East karenBattleQiZhiConfig_East in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
			{
				this.AddDelayCreateMonster(scene, scene.m_lPrepareTime + (long)(karenBattleQiZhiConfig_East.BeginTime * 1000), karenBattleQiZhiConfig_East);
			}
		}

		private void CheckSceneBufferTime(KarenBattleScene karenBattleScene, long nowTicks)
		{
			List<KarenBattleSceneBuff> list = new List<KarenBattleSceneBuff>();
			lock (this.RuntimeData.Mutex)
			{
				if (karenBattleScene.m_eStatus == 2)
				{
					if (karenBattleScene.SceneBuffDict.Count != 0)
					{
						foreach (KarenBattleSceneBuff karenBattleSceneBuff in karenBattleScene.SceneBuffDict.Values)
						{
							if (karenBattleSceneBuff.EndTicks < nowTicks)
							{
								list.Add(karenBattleSceneBuff);
							}
						}
						if (list.Count != 0)
						{
							foreach (KarenBattleSceneBuff karenBattleSceneBuff in list)
							{
								if (karenBattleSceneBuff.RoleID != 0)
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(karenBattleSceneBuff.RoleID);
									if (null != gameClient)
									{
										this.UpdateBuff4GameClient(gameClient, karenBattleSceneBuff.BuffID, karenBattleSceneBuff.tagInfo, false);
									}
								}
								KarenBattleQiZhiConfig_East karenBattleQiZhiConfig_East = karenBattleSceneBuff.tagInfo as KarenBattleQiZhiConfig_East;
								if (null != karenBattleQiZhiConfig_East)
								{
									this.AddDelayCreateMonster(karenBattleScene, TimeUtil.NOW() + (long)(karenBattleQiZhiConfig_East.RefreshCD * 1000), karenBattleSceneBuff.tagInfo);
								}
							}
						}
					}
				}
			}
		}

		public const SceneUIClasses ManagerType = 42;

		public ConcurrentDictionary<int, KarenBattleScene> SceneDict = new ConcurrentDictionary<int, KarenBattleScene>();

		private static long NextHeartBeatTicks = 0L;

		private static KarenBattleManager_MapEast instance = new KarenBattleManager_MapEast();

		public KarenBattleDataEast RuntimeData = new KarenBattleDataEast();
	}
}
