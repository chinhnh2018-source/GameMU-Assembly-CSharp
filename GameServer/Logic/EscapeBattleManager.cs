using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using AutoCSer.Net.TcpServer;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Ornament;
using GameServer.Logic.Reborn;
using GameServer.Server;
using KF.Contract.Data;
using KF.Remoting;
using KF.TcpCall;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class EscapeBattleManager : ICopySceneManager, IManager, ICmdProcessorEx, ICmdProcessor, IManager2, IEventListener, IEventListenerEx
	{
		private void InitScene(EscapeBattleScene scene, GameClient client)
		{
			foreach (EscapeBattleCollection escapeBattleCollection in this.RuntimeData.CollectionConfigList)
			{
				scene.CollectionConfigList.Add(escapeBattleCollection.Clone());
			}
			EscapeMapSafeArea escapeMapSafeArea;
			lock (this.RuntimeData.Mutex)
			{
				escapeMapSafeArea = this.RuntimeData.EscapeMapSafeAreaList[0];
			}
			scene.ScoreData.safeArea.AreaID = escapeMapSafeArea.ID;
			scene.ScoreData.safeArea.PosX = escapeMapSafeArea.StartSafePoint[0];
			scene.ScoreData.safeArea.PosY = escapeMapSafeArea.StartSafePoint[1];
			scene.ScoreData.targetSafeArea.AreaID = escapeMapSafeArea.ID;
			scene.ScoreData.targetSafeArea.PosX = escapeMapSafeArea.StartSafePoint[0];
			scene.ScoreData.targetSafeArea.PosY = escapeMapSafeArea.StartSafePoint[1];
			scene.ScoreData.AreaChangeTm = DateTime.MinValue;
			for (int i = 0; i < scene.TopClientCalExtProps.Length; i++)
			{
				scene.TopClientCalExtProps[i] = new double[177];
			}
		}

		bool ICopySceneManager.AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 59)
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
					DateTime dateTime = TimeUtil.NowDateTime().Date.Add(this.RuntimeData.DiffTimeSpan);
					lock (this.RuntimeData.Mutex)
					{
						EscapeBattleScene escapeBattleScene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqID, out escapeBattleScene))
						{
							EscapeBattleFuBenData fuBenData;
							if (!this.RuntimeData.KuaFuCopyDataDict.TryGetValue((long)((int)gameId), out fuBenData))
							{
								LogManager.WriteLog(2, "魔界大逃杀没有为副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
							}
							EscapeBattleMatchConfig escapeBattleMatchConfig = this.RuntimeData.Config.MatchConfigList.Find((EscapeBattleMatchConfig x) => x.MapCode == client.ClientData.MapCode);
							if (null == escapeBattleMatchConfig)
							{
								LogManager.WriteLog(2, "魔界大逃杀没有为副本找到对应的场景数据,MapCodeID:" + client.ClientData.MapCode, null, true);
							}
							escapeBattleScene = new EscapeBattleScene();
							escapeBattleScene.CleanAllInfo();
							escapeBattleScene.GameId = (int)gameId;
							escapeBattleScene.CopyMap = copyMap;
							escapeBattleScene.m_nMapCode = mapCode;
							escapeBattleScene.CopyMapId = copyMap.CopyMapID;
							escapeBattleScene.FuBenSeqId = fuBenSeqID;
							escapeBattleScene.SceneInfo = escapeBattleMatchConfig;
							escapeBattleScene.MapGridWidth = gameMap.MapGridWidth;
							escapeBattleScene.MapGridHeight = gameMap.MapGridHeight;
							escapeBattleScene.FuBenData = fuBenData;
							DateTime dateTime2 = dateTime.Add(this.GetStartTime(escapeBattleMatchConfig.MapCode));
							escapeBattleScene.StartTimeTicks = dateTime2.Ticks / 10000L;
							this.InitScene(escapeBattleScene, client);
							escapeBattleScene.GameStatisticalData.GameId = (int)gameId;
							this.SceneDict[fuBenSeqID] = escapeBattleScene;
						}
						escapeBattleScene.ClientDict[client.ClientData.RoleID] = client;
						EscapeBattleTeamInfo escapeBattleTeamInfo = escapeBattleScene.ScoreData.BattleTeamList.Find((EscapeBattleTeamInfo x) => x.TeamID == client.ClientData.ZhanDuiID);
						if (null == escapeBattleTeamInfo)
						{
							TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(client.ClientData.ZhanDuiID, client.ServerId);
							if (null != zhanDuiData)
							{
								escapeBattleTeamInfo = new EscapeBattleTeamInfo
								{
									TeamID = zhanDuiData.ZhanDuiID,
									TeamName = zhanDuiData.ZhanDuiName
								};
								escapeBattleScene.ScoreData.BattleTeamList.Add(escapeBattleTeamInfo);
								escapeBattleScene.GameStatisticalData.ZhanDuiDict[client.ClientData.ZhanDuiID] = zhanDuiData;
								escapeBattleScene.GameStatisticalData.ZhanDuiIDVsServerIDDict[client.ClientData.ZhanDuiID] = client.ServerId;
							}
						}
						List<EscapeBattleRoleInfo> list;
						if (!escapeBattleScene.ClientContextDataDict.TryGetValue(client.ClientData.ZhanDuiID, out list))
						{
							list = new List<EscapeBattleRoleInfo>();
							escapeBattleScene.ClientContextDataDict[client.ClientData.ZhanDuiID] = list;
						}
						EscapeBattleRoleInfo escapeBattleRoleInfo = list.Find((EscapeBattleRoleInfo x) => x.RoleID == roleId);
						if (null == escapeBattleRoleInfo)
						{
							escapeBattleRoleInfo = new EscapeBattleRoleInfo
							{
								RoleID = roleId,
								Name = client.ClientData.RoleName,
								Level = client.ClientData.Level,
								ChangeLevel = client.ClientData.ChangeLifeCount,
								ZoneID = client.ClientData.ZoneID,
								Occupation = client.ClientData.Occupation,
								RoleSex = client.ClientData.RoleSex,
								LifeV = client.ClientData.CurrentLifeV,
								MaxLifeV = client.ClientData.LifeV,
								ZhanDuiID = client.ClientData.ZhanDuiID,
								OnLine = true
							};
							list.Add(escapeBattleRoleInfo);
						}
						else
						{
							escapeBattleRoleInfo.Occupation = client.ClientData.Occupation;
							escapeBattleRoleInfo.RoleSex = client.ClientData.RoleSex;
							escapeBattleRoleInfo.LifeV = client.ClientData.CurrentLifeV;
							escapeBattleRoleInfo.MaxLifeV = client.ClientData.LifeV;
							escapeBattleRoleInfo.OnLine = true;
						}
						client.SceneObject = escapeBattleScene;
						client.SceneGameId = (long)escapeBattleScene.GameId;
						client.SceneContextData2 = escapeBattleRoleInfo;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(escapeBattleScene.SceneInfo.TotalSecs * 1000));
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

		bool ICopySceneManager.RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 59)
			{
				lock (this.RuntimeData.Mutex)
				{
					EscapeBattleScene escapeBattleScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out escapeBattleScene);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		void ICopySceneManager.TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= EscapeBattleManager.NextHeartBeatTicks)
			{
				EscapeBattleManager.NextHeartBeatTicks = num + 1020L;
				foreach (EscapeBattleScene escapeBattleScene in this.SceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int fuBenSeqId = escapeBattleScene.FuBenSeqId;
						int copyMapId = escapeBattleScene.CopyMapId;
						int nMapCode = escapeBattleScene.m_nMapCode;
						if (fuBenSeqId >= 0 && copyMapId >= 0 && nMapCode >= 0)
						{
							CopyMap copyMap = escapeBattleScene.CopyMap;
							DateTime dateTime = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							if (escapeBattleScene.m_eStatus == 1 || escapeBattleScene.m_eStatus == 2 || escapeBattleScene.m_eStatus == 3 || escapeBattleScene.m_eStatus == 4)
							{
								this.CheckCreateDynamicMonster(escapeBattleScene, num2);
								this.CheckDeleteDynamicMonster(escapeBattleScene, false, num2);
							}
							if (escapeBattleScene.m_eStatus == 0)
							{
								if (num2 >= escapeBattleScene.StartTimeTicks)
								{
									escapeBattleScene.m_lPrepareTime = escapeBattleScene.StartTimeTicks;
									escapeBattleScene.m_lBeginTime = escapeBattleScene.m_lPrepareTime + (long)(escapeBattleScene.SceneInfo.WaitSeconds * 1000);
									escapeBattleScene.m_eStatus = 1;
									escapeBattleScene.StateTimeData.GameType = 37;
									escapeBattleScene.StateTimeData.State = escapeBattleScene.m_eStatus;
									escapeBattleScene.StateTimeData.EndTicks = escapeBattleScene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, escapeBattleScene.StateTimeData, escapeBattleScene.CopyMap);
									this.InitCreateDynamicMonster(escapeBattleScene, num2);
								}
							}
							else if (escapeBattleScene.m_eStatus == 1)
							{
								if (num2 >= escapeBattleScene.m_lBeginTime)
								{
									escapeBattleScene.m_eStatus = 2;
									escapeBattleScene.m_lFightTime = escapeBattleScene.m_lBeginTime + (long)(escapeBattleScene.SceneInfo.SafeSecs * 1000);
									escapeBattleScene.StateTimeData.GameType = 37;
									escapeBattleScene.StateTimeData.State = escapeBattleScene.m_eStatus;
									escapeBattleScene.StateTimeData.EndTicks = escapeBattleScene.m_lFightTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, escapeBattleScene.StateTimeData, escapeBattleScene.CopyMap);
									for (int i = 1; i <= 3; i++)
									{
										GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, i, 0);
									}
									this.InitCreateDynamicMonster(escapeBattleScene, num2);
									this.BroadStateInfoAndScoreInfo(escapeBattleScene.CopyMap, -1, false, true);
								}
							}
							else if (escapeBattleScene.m_eStatus == 2)
							{
								if (num2 >= escapeBattleScene.m_lFightTime)
								{
									escapeBattleScene.m_eStatus = 3;
									escapeBattleScene.m_lEndTime = escapeBattleScene.m_lFightTime + (long)(escapeBattleScene.SceneInfo.BattleEndTime * 1000);
									escapeBattleScene.StateTimeData.GameType = 37;
									escapeBattleScene.StateTimeData.State = escapeBattleScene.m_eStatus;
									escapeBattleScene.StateTimeData.EndTicks = escapeBattleScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, escapeBattleScene.StateTimeData, escapeBattleScene.CopyMap);
									this.InitCreateDynamicMonster(escapeBattleScene, num2);
									this.CheckDeleteDynamicMonster(escapeBattleScene, true, num2);
									this.InitSceneBuffOnFightAss(escapeBattleScene, num2);
									this.CheckUpdateSafeArea(escapeBattleScene, num2);
									this.CheckZhanDuiWinLoseState(escapeBattleScene);
								}
							}
							else if (escapeBattleScene.m_eStatus == 3 || escapeBattleScene.m_eStatus == 4)
							{
								this.CheckUpdateSafeArea(escapeBattleScene, num2);
								this.CheckSceneAreaDamage(escapeBattleScene, num2);
								this.CheckSceneToAssState(escapeBattleScene, num2);
								if (num2 >= escapeBattleScene.m_lEndTime)
								{
									this.ProcessEnd(escapeBattleScene, true, num2);
								}
							}
							else if (escapeBattleScene.m_eStatus == 5)
							{
								escapeBattleScene.m_eStatus = 6;
								this.GiveAwards(escapeBattleScene, escapeBattleScene.GameStatisticalData.ZhanDuiIDWin);
								GameManager.CopyMapMgr.KillAllMonster(escapeBattleScene.CopyMap);
								EscapeBattleFuBenData escapeBattleFuBenData;
								if (this.RuntimeData.KuaFuCopyDataDict.TryGetValue((long)escapeBattleScene.GameId, out escapeBattleFuBenData))
								{
									LogManager.WriteLog(2, string.Format("魔界大逃杀跨服副本GameID={0},战斗结束", escapeBattleFuBenData.GameID), null, true);
								}
							}
							else if (escapeBattleScene.m_eStatus == 6)
							{
								if (num2 >= escapeBattleScene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(escapeBattleScene.m_lLeaveTime);
									escapeBattleScene.m_eStatus = 7;
									this.RuntimeData.GameStateQueue.Enqueue(new KeyValuePair<int, int>(escapeBattleScene.GameId, escapeBattleScene.m_eStatus));
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
										DataHelper.WriteExceptionLogEx(ex, "魔界大逃杀系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		public void CheckUpdateSafeArea(EscapeBattleScene scene, long nowMs)
		{
			if (scene.ScoreData.AreaChangeTm.Ticks / 10000L <= nowMs)
			{
				EscapeMapSafeArea escapeMapSafeArea = null;
				EscapeMapSafeArea escapeMapSafeArea2 = null;
				lock (this.RuntimeData.Mutex)
				{
					if (scene.SafeAreaRefreshState >= this.RuntimeData.EscapeMapSafeAreaList.Count)
					{
						return;
					}
					scene.SafeAreaRefreshState++;
					int num = Math.Min(scene.SafeAreaRefreshState, this.RuntimeData.EscapeMapSafeAreaList.Count - 1);
					escapeMapSafeArea = this.RuntimeData.EscapeMapSafeAreaList[num];
					int num2 = Math.Min(scene.SafeAreaRefreshState + 1, this.RuntimeData.EscapeMapSafeAreaList.Count - 1);
					escapeMapSafeArea2 = this.RuntimeData.EscapeMapSafeAreaList[num2];
					if (num == num2 && scene.ScoreData.safeArea.AreaID == scene.ScoreData.targetSafeArea.AreaID)
					{
						return;
					}
				}
				scene.ScoreData.safeArea.AreaID = scene.ScoreData.targetSafeArea.AreaID;
				scene.ScoreData.safeArea.PosX = scene.ScoreData.targetSafeArea.PosX;
				scene.ScoreData.safeArea.PosY = scene.ScoreData.targetSafeArea.PosY;
				int posX = scene.ScoreData.safeArea.PosX;
				int posY = scene.ScoreData.safeArea.PosY;
				this.RandomOnePointInCircle(posX, posY, 0, escapeMapSafeArea.SafeRadius - escapeMapSafeArea2.SafeRadius, ref posX, ref posY);
				scene.ScoreData.targetSafeArea.AreaID = escapeMapSafeArea2.ID;
				scene.ScoreData.targetSafeArea.PosX = posX;
				scene.ScoreData.targetSafeArea.PosY = posY;
				scene.ScoreData.AreaChangeTm = new DateTime(nowMs * 10000L + (long)escapeMapSafeArea.TimeStage * 10000000L);
				this.BroadStateInfoAndScoreInfo(scene.CopyMap, -1, false, true);
			}
		}

		private void CheckSceneToAssState(EscapeBattleScene scene, long nowMs)
		{
			if (scene.m_eStatus == 3)
			{
				int num = 0;
				List<GameClient> clientsList = scene.CopyMap.GetClientsList();
				if (clientsList == null || clientsList.Count <= 0)
				{
					this.ProcessEnd(scene, true, nowMs);
				}
				else
				{
					foreach (GameClient gameClient in clientsList)
					{
						if (gameClient.ClientData.HideGM <= 0)
						{
							EscapeBattleRoleInfo escapeBattleRoleInfo = gameClient.SceneContextData2 as EscapeBattleRoleInfo;
							if (gameClient.ClientData.CurrentLifeV > 0 || escapeBattleRoleInfo.ReliveCount > 0)
							{
								num++;
							}
						}
					}
					bool flag = false;
					lock (this.RuntimeData.Mutex)
					{
						if (num < scene.SceneInfo.FanaticismStartNum)
						{
							flag = true;
						}
					}
					if (flag)
					{
						scene.m_eStatus = 4;
						scene.StateTimeData.State = scene.m_eStatus;
						this.InitCreateDynamicMonster(scene, nowMs);
						this.BroadStateInfoAndScoreInfo(scene.CopyMap, -1, true, true);
					}
				}
			}
		}

		private void InitSceneBuffOnFightAss(EscapeBattleScene scene, long nowMs)
		{
			if (scene.m_eStatus == 3)
			{
				GameClient gameClient = null;
				int num = 0;
				foreach (KeyValuePair<int, GameClient> keyValuePair in scene.ClientDict)
				{
					GameClient gameClient2 = keyValuePair.Value;
					if (gameClient2 != null && gameClient2.ClientData.HideGM == 0 && gameClient2.ClientData.CombatForce > num)
					{
						num = gameClient2.ClientData.CombatForce;
						gameClient = gameClient2;
					}
				}
				if (null != gameClient)
				{
					double extProp = RoleAlgorithm.GetExtProp(gameClient, 13);
					double extProp2 = RoleAlgorithm.GetExtProp(gameClient, 8);
					double extProp3 = RoleAlgorithm.GetExtProp(gameClient, 7);
					double extProp4 = RoleAlgorithm.GetExtProp(gameClient, 10);
					double extProp5 = RoleAlgorithm.GetExtProp(gameClient, 9);
					double num2 = Math.Max(extProp2, extProp3);
					num2 = Math.Max(num2, extProp4);
					num2 = Math.Max(num2, extProp5);
					double extProp6 = RoleAlgorithm.GetExtProp(gameClient, 4);
					double extProp7 = RoleAlgorithm.GetExtProp(gameClient, 3);
					double extProp8 = RoleAlgorithm.GetExtProp(gameClient, 6);
					double extProp9 = RoleAlgorithm.GetExtProp(gameClient, 5);
					double num3 = Math.Max(extProp6, extProp7);
					num3 = Math.Max(num3, extProp8);
					num3 = Math.Max(num3, extProp9);
					double extProp10 = RoleAlgorithm.GetExtProp(gameClient, 18);
					double extProp11 = RoleAlgorithm.GetExtProp(gameClient, 19);
					for (int i = 0; i < scene.TopClientCalExtProps.Length; i++)
					{
						scene.TopClientCalExtProps[i][13] = extProp * this.RuntimeData.BuffAttributeProportion[i];
						scene.TopClientCalExtProps[i][8] = num2 * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
						scene.TopClientCalExtProps[i][7] = num2 * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
						scene.TopClientCalExtProps[i][10] = num2 * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
						scene.TopClientCalExtProps[i][9] = num2 * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
						scene.TopClientCalExtProps[i][4] = num3 * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
						scene.TopClientCalExtProps[i][3] = num3 * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
						scene.TopClientCalExtProps[i][6] = num3 * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
						scene.TopClientCalExtProps[i][5] = num3 * 0.9 * this.RuntimeData.BuffAttributeProportion[i];
						scene.TopClientCalExtProps[i][18] = extProp10 * this.RuntimeData.BuffAttributeProportion[i];
						scene.TopClientCalExtProps[i][19] = extProp11 * this.RuntimeData.BuffAttributeProportion[i];
					}
				}
			}
			if (scene.m_eStatus == 3)
			{
				List<GameClient> clientsList = scene.CopyMap.GetClientsList();
				if (clientsList != null && clientsList.Count > 0)
				{
					for (int j = 0; j < clientsList.Count; j++)
					{
						GameClient gameClient2 = clientsList[j];
						if (gameClient2 != null)
						{
							lock (this.RuntimeData.Mutex)
							{
								this.UpdateBuff4GameClient(gameClient2, BufferItemTypes.EscapeBattleDevil, this.RuntimeData.DevilLossNum);
							}
						}
					}
				}
			}
		}

		private void InitCreateDynamicMonster(EscapeBattleScene scene, long nowMs)
		{
			List<EscapeBattleCollection> list = scene.CollectionConfigList.FindAll((EscapeBattleCollection x) => x.eState == scene.m_eStatus);
			if (null != list)
			{
				foreach (EscapeBattleCollection escapeBattleCollection in list)
				{
					long ticks = nowMs + (long)(escapeBattleCollection.RefreshTime * 1000);
					this.AddDelayCreateMonster(scene, ticks, escapeBattleCollection);
				}
			}
		}

		private void AddDelayCreateMonster(EscapeBattleScene scene, long ticks, object monster)
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

		public void CheckDeleteDynamicMonster(EscapeBattleScene scene, bool chgState, long nowMs)
		{
			CopyMap copyMap = scene.CopyMap;
			List<object> list = GameManager.MonsterMgr.GetCopyMapIDMonsterList(copyMap.CopyMapID);
			list = Global.ConvertObjsList(copyMap.MapCode, copyMap.CopyMapID, list, false);
			if (null != list)
			{
				int i = 0;
				while (i < list.Count)
				{
					Monster monster = list[i] as Monster;
					if (null != monster)
					{
						EscapeBattleCollection escapeBattleCollection = monster.Tag as EscapeBattleCollection;
						if (monster.MonsterType != 1001 && null != escapeBattleCollection)
						{
							bool flag = false;
							if (chgState && scene.m_eStatus == 3 && escapeBattleCollection.cType == EscapeBCollectType.EBCT_LifeSeed)
							{
								flag = true;
							}
							if (escapeBattleCollection.CollectLiveTime > 0)
							{
								long num = monster.GetMonsterBirthTick() / 10000L;
								long num2 = TimeUtil.NOW();
								if ((num2 - num) / 1000L > (long)escapeBattleCollection.CollectLiveTime)
								{
									flag = true;
								}
							}
							if (flag)
							{
								Global.SystemKillMonster(monster);
							}
						}
					}
					IL_11A:
					i++;
					continue;
					goto IL_11A;
				}
			}
		}

		public void CheckCreateDynamicMonster(EscapeBattleScene scene, long nowMs)
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
							if (obj is EscapeBattleCollection)
							{
								EscapeBattleCollection escapeBattleCollection = obj as EscapeBattleCollection;
								int num = 0;
								int num2 = 0;
								if (this.RandomOnePointInArea(scene, escapeBattleCollection.RefreshRegion, ref num, ref num2, ObjectTypes.OT_MONSTER))
								{
									GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, escapeBattleCollection.RefreshMonsterId, scene.CopyMapId, escapeBattleCollection.RefreshMonsterNum, num / scene.MapGridWidth, num2 / scene.MapGridHeight, 3, 0, 59, escapeBattleCollection, null);
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

		public void RandomOnePointInCircle(int centerX, int centerY, int radiusMin, int radiusMax, ref int PosX, ref int PosY)
		{
			PosX = centerX;
			PosY = centerY;
			int randomNumber = Global.GetRandomNumber(radiusMin, radiusMax);
			double num = 6.2831853071795862 * Global.GetRandom();
			PosX += (int)(Math.Cos(num) * (double)randomNumber);
			PosY += (int)(Math.Sin(num) * (double)randomNumber);
		}

		public bool RandomOnePointInArea(EscapeBattleScene scene, int RefreshRegion, ref int PosX, ref int PosY, ObjectTypes objType = ObjectTypes.OT_MONSTER)
		{
			EscapeMapSafeArea escapeMapSafeArea;
			lock (this.RuntimeData.Mutex)
			{
				if (1 == RefreshRegion)
				{
					escapeMapSafeArea = this.RuntimeData.EscapeMapSafeAreaList[0];
				}
				else
				{
					escapeMapSafeArea = this.RuntimeData.EscapeMapSafeAreaList.Find((EscapeMapSafeArea x) => x.ID == scene.ScoreData.safeArea.AreaID);
				}
			}
			bool result;
			if (null == escapeMapSafeArea)
			{
				result = false;
			}
			else
			{
				int num = 0;
				for (;;)
				{
					if (1 == RefreshRegion)
					{
						PosX = escapeMapSafeArea.StartSafePoint[0];
						PosY = escapeMapSafeArea.StartSafePoint[1];
					}
					else
					{
						PosX = scene.ScoreData.safeArea.PosX;
						PosY = scene.ScoreData.safeArea.PosY;
					}
					int radiusMin = 0;
					int radiusMax;
					if (1 == RefreshRegion || 2 == RefreshRegion)
					{
						radiusMax = escapeMapSafeArea.SafeRadius;
					}
					else
					{
						radiusMin = escapeMapSafeArea.SafeRadius;
						radiusMax = (int)((double)escapeMapSafeArea.SafeRadius * 1.5);
					}
					this.RandomOnePointInCircle(PosX, PosY, radiusMin, radiusMax, ref PosX, ref PosY);
					if (!Global.InObsByGridXY(objType, scene.m_nMapCode, PosX / scene.MapGridWidth, PosY / scene.MapGridHeight, 0, 0))
					{
						break;
					}
					num++;
					if (num > 100)
					{
						goto Block_7;
					}
				}
				return true;
				Block_7:
				result = false;
			}
			return result;
		}

		private void ProcessEnd(EscapeBattleScene scene, bool overTime, long nowTicks)
		{
			if (scene.m_eStatus < 5)
			{
				if (!overTime)
				{
					EscapeBattleTeamInfo escapeBattleTeamInfo = scene.ScoreData.BattleTeamList.Find((EscapeBattleTeamInfo x) => x.RankNum == 0 || x.RankNum == 1);
					if (null != escapeBattleTeamInfo)
					{
						escapeBattleTeamInfo.RankNum = 1;
						scene.GameStatisticalData.ZhanDuiIDWin = escapeBattleTeamInfo.TeamID;
					}
					List<GameClient> list = scene.CopyMap.GetClientsList();
					if (list != null && list.Count > 0)
					{
						list = list.FindAll((GameClient x) => x.ClientData.ZhanDuiID == scene.GameStatisticalData.ZhanDuiIDWin);
						foreach (GameClient gameClient in list)
						{
							if (gameClient.ClientData.HideGM <= 0)
							{
								EscapeBattleRoleInfo escapeBattleRoleInfo = gameClient.SceneContextData2 as EscapeBattleRoleInfo;
								if (gameClient.ClientData.CurrentLifeV > 0 || escapeBattleRoleInfo.ReliveCount > 0)
								{
									scene.GameStatisticalData.WinZhanDuiAliveCount++;
								}
							}
						}
					}
				}
				scene.m_eStatus = 5;
				scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearSeconds * 1000);
				scene.StateTimeData.GameType = 37;
				scene.StateTimeData.State = 7;
				scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
				GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
			}
		}

		public void BroadStateInfoAndScoreInfo(CopyMap copyMap, int specZhanDui = -1, bool timeState = true, bool sideScore = true)
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
							this.NotifyTimeStateInfoAndScoreInfo(gameClient, timeState, sideScore);
						}
					}
				}
			}
		}

		public void NotifySpriteInjured(GameClient client)
		{
			EscapeBattleScene escapeBattleScene = client.SceneObject as EscapeBattleScene;
			if (null != escapeBattleScene)
			{
				this.BroadStateInfoAndScoreInfo(escapeBattleScene.CopyMap, client.ClientData.ZhanDuiID, false, true);
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				EscapeBattleScene escapeBattleScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out escapeBattleScene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, escapeBattleScene.StateTimeData, false);
					}
					if (sideScore)
					{
						EscapeBattleRoleInfo escapeBattleRoleInfo = client.SceneContextData2 as EscapeBattleRoleInfo;
						EscapeBattleSideScore escapeBattleSideScore = escapeBattleScene.ScoreData.Clone();
						escapeBattleSideScore.eStatus = escapeBattleScene.StateTimeData.State;
						escapeBattleSideScore.ReliveCount = escapeBattleRoleInfo.ReliveCount;
						List<EscapeBattleRoleInfo> list;
						if (escapeBattleScene.ClientContextDataDict.TryGetValue(client.ClientData.ZhanDuiID, out list))
						{
							escapeBattleSideScore.BattleRoleList = list.FindAll((EscapeBattleRoleInfo x) => x.OnLine && x.RoleID != client.ClientData.RoleID);
							using (List<EscapeBattleRoleInfo>.Enumerator enumerator = escapeBattleSideScore.BattleRoleList.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									EscapeBattleRoleInfo role = enumerator.Current;
									List<GameClient> clientsList = escapeBattleScene.CopyMap.GetClientsList();
									if (clientsList != null && clientsList.Count >= 0)
									{
										GameClient gameClient = clientsList.Find((GameClient x) => x.ClientData.RoleID == role.RoleID);
										if (null != gameClient)
										{
											role.LifeV = gameClient.ClientData.CurrentLifeV;
											role.MaxLifeV = gameClient.ClientData.LifeV;
											if (gameClient.ClientData.HideGM > 0)
											{
												role.LifeV = 0;
											}
										}
									}
								}
							}
						}
						client.sendCmd<EscapeBattleSideScore>(2113, escapeBattleSideScore, false);
					}
				}
			}
		}

		public void PushGameResultData(EscapeBattleScene scene, int zhanduiId)
		{
			EscapeBattleManager.<>c__DisplayClass30 CS$<>8__locals1 = new EscapeBattleManager.<>c__DisplayClass30();
			CS$<>8__locals1.scene = scene;
			List<EscapeBattleRoleInfo> list;
			if (CS$<>8__locals1.scene.ClientContextDataDict.TryGetValue(zhanduiId, out list))
			{
				foreach (EscapeBattleRoleInfo escapeBattleRoleInfo in list)
				{
					GameClient gameClient = null;
					if (CS$<>8__locals1.scene.ClientDict.TryGetValue(escapeBattleRoleInfo.RoleID, out gameClient))
					{
						List<int> escapeBattleRoleAnalysisData = this.GetEscapeBattleRoleAnalysisData(gameClient);
						if (null != escapeBattleRoleAnalysisData)
						{
							List<int> list2;
							(list2 = escapeBattleRoleAnalysisData)[2] = list2[2] + escapeBattleRoleInfo.KillRoleNum;
							if (escapeBattleRoleInfo.ZhanDuiID == CS$<>8__locals1.scene.GameStatisticalData.ZhanDuiIDWin)
							{
								(list2 = escapeBattleRoleAnalysisData)[3] = list2[3] + 1;
							}
						}
						this.SaveEscapeBattleRoleAnalysisData(gameClient, escapeBattleRoleAnalysisData);
						gameClient = GameManager.ClientMgr.FindClient(escapeBattleRoleInfo.RoleID);
						if (null != gameClient)
						{
							GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(gameClient, OrnamentGoalType.OGT_EscapeRoleKill, new int[0]));
						}
					}
				}
			}
			List<int> list3 = new List<int>();
			TianTi5v5ZhanDuiData zhanduiData;
			if (CS$<>8__locals1.scene.GameStatisticalData.ZhanDuiDict.TryGetValue(zhanduiId, out zhanduiData))
			{
				int serverID;
				if (CS$<>8__locals1.scene.GameStatisticalData.ZhanDuiIDVsServerIDDict.TryGetValue(zhanduiId, out serverID))
				{
					EscapeBattleTeamInfo escapeBattleTeamInfo = CS$<>8__locals1.scene.ScoreData.BattleTeamList.Find((EscapeBattleTeamInfo x) => x.TeamID == zhanduiData.ZhanDuiID);
					if (null != escapeBattleTeamInfo)
					{
						EscapeBDuanAwardsConfig escapeBattleAwardConfigByJiFen = this.GetEscapeBattleAwardConfigByJiFen(zhanduiData.EscapeJiFen);
						if (null != escapeBattleAwardConfigByJiFen)
						{
							int escapeJiFen = zhanduiData.EscapeJiFen;
							if (zhanduiData.ZhanDuiID == CS$<>8__locals1.scene.GameStatisticalData.ZhanDuiIDWin)
							{
								List<int> list4 = escapeBattleAwardConfigByJiFen.WinRankValue.Find((List<int> x) => x[0] == CS$<>8__locals1.scene.GameStatisticalData.WinZhanDuiAliveCount);
								if (null != list4)
								{
									zhanduiData.EscapeJiFen += list4[1];
								}
							}
							else
							{
								int leaveNum = CS$<>8__locals1.scene.ScoreData.BattleTeamList.Count - escapeBattleTeamInfo.RankNum + 1;
								List<int> list5;
								if (escapeBattleTeamInfo.RankNum == 0)
								{
									list5 = escapeBattleAwardConfigByJiFen.LoseRankValue[escapeBattleAwardConfigByJiFen.LoseRankValue.Count - 1];
								}
								else
								{
									list5 = escapeBattleAwardConfigByJiFen.LoseRankValue.Find((List<int> x) => x[0] == leaveNum);
								}
								if (null != list5)
								{
									zhanduiData.EscapeJiFen -= list5[1];
								}
							}
							zhanduiData.EscapeJiFen = Math.Max(zhanduiData.EscapeJiFen, 0);
							zhanduiData.EscapeLastFightTime = TimeUtil.NowDateTime();
							list3.Add(zhanduiId);
							list3.Add(zhanduiData.EscapeJiFen);
							TianTi5v5Manager.getInstance().UpdateEscapeZhanDuiData2DB(zhanduiData, serverID);
						}
					}
				}
			}
			KeyValuePair<int, List<int>> item = new KeyValuePair<int, List<int>>(CS$<>8__locals1.scene.GameId, list3);
			this.RuntimeData.PKResultQueue.Enqueue(item);
		}

		public void GiveAwards(EscapeBattleScene scene, int zhanduiId)
		{
			try
			{
				using (Dictionary<int, List<EscapeBattleRoleInfo>>.Enumerator enumerator = scene.ClientContextDataDict.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, List<EscapeBattleRoleInfo>> kvp = enumerator.Current;
						bool flag;
						if (zhanduiId > 0)
						{
							KeyValuePair<int, List<EscapeBattleRoleInfo>> kvp3 = kvp;
							flag = (kvp3.Key == zhanduiId);
						}
						else
						{
							flag = true;
						}
						if (flag)
						{
							EscapeBattleTeamInfo escapeBattleTeamInfo = scene.ScoreData.BattleTeamList.Find(delegate(EscapeBattleTeamInfo x)
							{
								int teamID = x.TeamID;
								KeyValuePair<int, List<EscapeBattleRoleInfo>> kvp2 = kvp;
								return teamID == kvp2.Key;
							});
							if (null != escapeBattleTeamInfo)
							{
								if (zhanduiId > 0 || escapeBattleTeamInfo.RankNum == 0)
								{
									Dictionary<int, TianTi5v5ZhanDuiData> zhanDuiDict = scene.GameStatisticalData.ZhanDuiDict;
									KeyValuePair<int, List<EscapeBattleRoleInfo>> kvp3 = kvp;
									TianTi5v5ZhanDuiData zhanduiData;
									if (zhanDuiDict.TryGetValue(kvp3.Key, out zhanduiData))
									{
										kvp3 = kvp;
										this.PushGameResultData(scene, kvp3.Key);
										kvp3 = kvp;
										foreach (EscapeBattleRoleInfo escapeBattleRoleInfo in kvp3.Value)
										{
											int success;
											if (escapeBattleRoleInfo.ZhanDuiID == scene.GameStatisticalData.ZhanDuiIDWin)
											{
												success = 1;
											}
											else
											{
												success = 0;
											}
											GameClient gameClient = GameManager.ClientMgr.FindClient(escapeBattleRoleInfo.RoleID);
											if (gameClient != null && gameClient.ClientData.MapCode == scene.m_nMapCode)
											{
												this.NtfCanGetAward(gameClient, success, scene, zhanduiData, escapeBattleTeamInfo);
												this.GiveRoleAwards(gameClient, success, scene, zhanduiData, escapeBattleTeamInfo, false);
											}
											else
											{
												scene.ClientDict.TryGetValue(escapeBattleRoleInfo.RoleID, out gameClient);
												this.GiveRoleAwards(gameClient, success, scene, zhanduiData, escapeBattleTeamInfo, true);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "魔界大逃杀系统清场调度异常");
			}
		}

		private void NtfCanGetAward(GameClient client, int success, EscapeBattleScene scene, TianTi5v5ZhanDuiData zhanduiData, EscapeBattleTeamInfo teamInfo)
		{
			EscapeBattleRoleInfo escapeBattleRoleInfo = client.SceneContextData2 as EscapeBattleRoleInfo;
			EscapeBDuanAwardsConfig escapeBattleAwardConfigByJiFen = this.GetEscapeBattleAwardConfigByJiFen(zhanduiData.EscapeJiFen);
			if (escapeBattleAwardConfigByJiFen != null && null != escapeBattleRoleInfo)
			{
				EscapeBattleAwardsData escapeBattleAwardsData = new EscapeBattleAwardsData();
				escapeBattleAwardsData.Success = success;
				escapeBattleAwardsData.RankNum = teamInfo.RankNum;
				escapeBattleAwardsData.AwardID = escapeBattleAwardConfigByJiFen.ID;
				escapeBattleAwardsData.ZhanDuiKillNum = teamInfo.ZhanDuiKillNum;
				List<int> escapeBattleRoleAnalysisData = this.GetEscapeBattleRoleAnalysisData(client);
				if (null != escapeBattleRoleAnalysisData)
				{
					escapeBattleAwardsData.WinToDay = escapeBattleRoleAnalysisData[3];
				}
				if (success > 0)
				{
					List<int> list = escapeBattleAwardConfigByJiFen.WinRankValue.Find((List<int> x) => x[0] == scene.GameStatisticalData.WinZhanDuiAliveCount);
					if (null != list)
					{
						escapeBattleAwardsData.ModJiFen = list[1];
					}
				}
				else
				{
					int leaveNum = scene.ScoreData.BattleTeamList.Count - teamInfo.RankNum + 1;
					List<int> list2;
					if (teamInfo.RankNum == 0)
					{
						list2 = escapeBattleAwardConfigByJiFen.LoseRankValue[escapeBattleAwardConfigByJiFen.LoseRankValue.Count - 1];
					}
					else
					{
						list2 = escapeBattleAwardConfigByJiFen.LoseRankValue.Find((List<int> x) => x[0] == leaveNum);
					}
					if (null != list2)
					{
						escapeBattleAwardsData.ModJiFen = -list2[1];
					}
				}
				client.sendCmd<EscapeBattleAwardsData>(2111, escapeBattleAwardsData, false);
			}
		}

		private int GiveRoleAwards(GameClient client, int success, EscapeBattleScene scene, TianTi5v5ZhanDuiData zhanduiData, EscapeBattleTeamInfo teamInfo, bool froceMail)
		{
			EscapeBattleRoleInfo escapeBattleRoleInfo = client.SceneContextData2 as EscapeBattleRoleInfo;
			EscapeBDuanAwardsConfig escapeBattleAwardConfigByJiFen = this.GetEscapeBattleAwardConfigByJiFen(zhanduiData.EscapeJiFen);
			int result;
			if (escapeBattleAwardConfigByJiFen == null || null == escapeBattleRoleInfo)
			{
				result = -5;
			}
			else
			{
				List<AwardsItemData> items;
				string lang;
				if (success > 0)
				{
					items = (escapeBattleAwardConfigByJiFen.WinAwardsItemList as AwardsItemList).Items;
					lang = GLang.GetLang(8009, new object[0]);
				}
				else
				{
					items = (escapeBattleAwardConfigByJiFen.LoseAwardsItemList as AwardsItemList).Items;
					lang = GLang.GetLang(8010, new object[0]);
				}
				string goodsFromWhere = "魔界大逃杀奖励";
				List<AwardsItemData> list = new List<AwardsItemData>();
				int num = 0;
				List<int> escapeBattleRoleAnalysisData = this.GetEscapeBattleRoleAnalysisData(client);
				if (null != escapeBattleRoleAnalysisData)
				{
					num = escapeBattleRoleAnalysisData[3];
				}
				if (escapeBattleAwardConfigByJiFen.FirstWinAwardsItemList != null && success > 0 && 1 == num)
				{
					list.AddRange((escapeBattleAwardConfigByJiFen.FirstWinAwardsItemList as AwardsItemList).Items);
				}
				else
				{
					list.AddRange(items);
				}
				int num2;
				if (list != null && (froceMail || !RebornEquip.MoreIsCanIntoRebornOrBaseBagAward(client, list, out num2)))
				{
					Global.UseMailGivePlayerAward2(client, list, GLang.GetLang(8008, new object[0]), lang, 0, 0, 0);
				}
				else if (list != null)
				{
					foreach (AwardsItemData awardsItemData in list)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, goodsFromWhere, "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
					}
				}
				this.SetSeasonRankDataFlags(client, true, false);
				result = 1;
			}
			return result;
		}

		public bool ClientRelive(GameClient client)
		{
			EscapeBattleScene escapeBattleScene = client.SceneObject as EscapeBattleScene;
			bool result;
			if (escapeBattleScene == null || client.ClientData.HideGM > 0 || client.ClientData.CurrentLifeV > 0)
			{
				result = false;
			}
			else
			{
				EscapeBattleRoleInfo escapeBattleRoleInfo = client.SceneContextData2 as EscapeBattleRoleInfo;
				bool flag = false;
				if (escapeBattleRoleInfo.ReliveCount > 0)
				{
					flag = true;
				}
				int posX = 0;
				int posY = 0;
				if (flag)
				{
					if (!this.RandomOnePointInArea(escapeBattleScene, 2, ref posX, ref posY, ObjectTypes.OT_CLIENT))
					{
						return true;
					}
				}
				else
				{
					posX = client.ClientData.PosX;
					posY = client.ClientData.PosY;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (flag)
					{
						escapeBattleRoleInfo.ReliveCount--;
						this.NotifyTimeStateInfoAndScoreInfo(client, true, true);
						this.UpdateBuff4GameClient(client, BufferItemTypes.EscapeBattleDevil, this.RuntimeData.DevilLossNum);
					}
					else
					{
						lock (VideoLogic.getInstance().Mutex)
						{
							client.ClientData.GuanZhanGM = 1;
							client.ClientData.HideGM = 1;
							List<int> list = new List<int>(client.ClientData.TrackingRoleIDList);
							foreach (int roleID in list)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
								if (null != gameClient)
								{
									VideoLogic.getInstance().CancleTracking(gameClient, true);
									VideoLogic.getInstance().TryTrackingOther(gameClient, client);
								}
							}
						}
					}
					client.ClientData.CurrentLifeV = client.ClientData.LifeV;
					client.ClientData.CurrentMagicV = client.ClientData.MagicV;
					client.ClientData.MoveAndActionNum = 0;
					GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, posX, posY, -1);
					Global.ClientRealive(client, posX, posY, -1);
					if (!flag)
					{
						List<object> all9Clients = Global.GetAll9Clients(client);
						GameManager.ClientMgr.NotifyOthersLeave(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, all9Clients);
					}
				}
				this.BroadStateInfoAndScoreInfo(escapeBattleScene.CopyMap, client.ClientData.ZhanDuiID, false, true);
				result = true;
			}
			return result;
		}

		private bool GetBirthPoint(int mapCode, int side, out int toPosX, out int toPosY)
		{
			toPosX = -1;
			toPosY = -1;
			GameMap gameMap = null;
			bool result;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
			{
				result = false;
			}
			else
			{
				int posX = this.RuntimeData.MapBirthPointDict[1 + side % this.RuntimeData.MapBirthPointDict.Count].PosX;
				int posY = this.RuntimeData.MapBirthPointDict[1 + side % this.RuntimeData.MapBirthPointDict.Count].PosY;
				int birthRadius = this.RuntimeData.MapBirthPointDict[1 + side % this.RuntimeData.MapBirthPointDict.Count].BirthRadius;
				Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, mapCode, posX, posY, birthRadius);
				toPosX = (int)mapPoint.X;
				toPosY = (int)mapPoint.Y;
				result = true;
			}
			return result;
		}

		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				EscapeBattleScene escapeBattleScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out escapeBattleScene))
				{
					if (escapeBattleScene.m_eStatus == 3 || escapeBattleScene.m_eStatus == 4)
					{
						EscapeBattleRoleInfo escapeBattleRoleInfo = client.SceneContextData2 as EscapeBattleRoleInfo;
						escapeBattleRoleInfo.KillRoleNum++;
						EscapeBattleTeamInfo escapeBattleTeamInfo = escapeBattleScene.ScoreData.BattleTeamList.Find((EscapeBattleTeamInfo x) => x.TeamID == client.ClientData.ZhanDuiID);
						if (null != escapeBattleTeamInfo)
						{
							escapeBattleTeamInfo.ZhanDuiKillNum++;
						}
						long num = 0L;
						BufferData bufferDataByID = Global.GetBufferDataByID(other, 2090002);
						if (null != bufferDataByID)
						{
							num = bufferDataByID.BufferVal;
						}
						this.UpdateBuff4GameClient(other, BufferItemTypes.EscapeBattleDevil, -(int)num);
						this.UpdateBuff4GameClient(client, BufferItemTypes.EscapeBattleDevil, (int)num);
						double lifeV = (double)this.RuntimeData.KillReplyHp;
						GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, lifeV, "魔界大逃杀击杀");
						this.BroadStateInfoAndScoreInfo(escapeBattleScene.CopyMap, -1, false, true);
						this.CheckZhanDuiWinLoseState(escapeBattleScene);
					}
				}
			}
		}

		public void CheckZhanDuiWinLoseState(EscapeBattleScene scene)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (scene.m_eStatus == 3 || scene.m_eStatus == 4)
				{
					long nowTicks = TimeUtil.NOW();
					if (scene.ScoreData.BattleTeamList.Count == 1)
					{
						this.ProcessEnd(scene, false, nowTicks);
					}
					else
					{
						foreach (EscapeBattleTeamInfo escapeBattleTeamInfo in scene.ScoreData.BattleTeamList)
						{
							if (escapeBattleTeamInfo.RankNum <= 0)
							{
								bool flag2 = false;
								List<EscapeBattleRoleInfo> list;
								if (scene.ClientContextDataDict.TryGetValue(escapeBattleTeamInfo.TeamID, out list))
								{
									using (List<EscapeBattleRoleInfo>.Enumerator enumerator2 = list.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											EscapeBattleRoleInfo role = enumerator2.Current;
											if (role.OnLine)
											{
												List<GameClient> clientsList = scene.CopyMap.GetClientsList();
												if (clientsList != null && clientsList.Count > 0)
												{
													GameClient gameClient = clientsList.Find((GameClient x) => x.ClientData.RoleID == role.RoleID);
													if (gameClient != null && gameClient.ClientData.HideGM <= 0)
													{
														if (gameClient.ClientData.CurrentLifeV > 0 || role.ReliveCount > 0)
														{
															flag2 = true;
														}
													}
												}
											}
										}
									}
								}
								if (!flag2)
								{
									escapeBattleTeamInfo.RankNum = scene.ScoreData.BattleTeamList.FindAll((EscapeBattleTeamInfo x) => x.RankNum == 0).Count;
									if (escapeBattleTeamInfo.RankNum <= 2)
									{
										this.ProcessEnd(scene, false, nowTicks);
									}
									if (escapeBattleTeamInfo.RankNum > 1)
									{
										this.GiveAwards(scene, escapeBattleTeamInfo.TeamID);
									}
								}
							}
						}
					}
				}
			}
		}

		public void RoleLeaveFuBen(GameClient client)
		{
			EscapeBattleScene escapeBattleScene = client.SceneObject as EscapeBattleScene;
			if (null != escapeBattleScene)
			{
				EscapeBattleRoleInfo escapeBattleRoleInfo = client.SceneContextData2 as EscapeBattleRoleInfo;
				escapeBattleRoleInfo.OnLine = false;
				this.CheckZhanDuiWinLoseState(escapeBattleScene);
			}
		}

		public void OnCaiJiFinish(GameClient client, Monster monster)
		{
			lock (this.RuntimeData.Mutex)
			{
				EscapeBattleScene escapeBattleScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out escapeBattleScene))
				{
					if (escapeBattleScene.m_eStatus != 1)
					{
						EscapeBattleCollection escapeBattleCollection = monster.Tag as EscapeBattleCollection;
						if (escapeBattleCollection != null)
						{
							if (escapeBattleCollection.IsDeath >= 0)
							{
								this.AddDelayCreateMonster(escapeBattleScene, TimeUtil.NOW(), escapeBattleCollection);
							}
							if (EscapeBCollectType.EBCT_God == escapeBattleCollection.cType)
							{
								this.UpdateBuff4GameClient(client, BufferItemTypes.EscapeBattleGod, escapeBattleCollection.CollectGodNum);
							}
							else if (EscapeBCollectType.EBCT_LifeSeed == escapeBattleCollection.cType)
							{
								EscapeBattleTeamInfo escapeBattleTeamInfo = escapeBattleScene.ScoreData.BattleTeamList.Find((EscapeBattleTeamInfo x) => x.TeamID == client.ClientData.ZhanDuiID);
								if (null != escapeBattleTeamInfo)
								{
									escapeBattleTeamInfo.LifeSeed++;
									this.RefreshTeamMemberReliveCount(escapeBattleScene, escapeBattleTeamInfo);
									this.BroadStateInfoAndScoreInfo(escapeBattleScene.CopyMap, -1, false, true);
								}
							}
						}
					}
				}
			}
		}

		public void RefreshTeamMemberReliveCount(EscapeBattleScene scene, EscapeBattleTeamInfo TeamInfo)
		{
			List<int[]> list;
			lock (this.RuntimeData.Mutex)
			{
				list = this.RuntimeData.LifeSeedNum.FindAll((int[] x) => x[0] <= TeamInfo.LifeSeed);
			}
			if (null != list)
			{
				List<EscapeBattleRoleInfo> list2;
				if (scene.ClientContextDataDict.TryGetValue(TeamInfo.TeamID, out list2))
				{
					foreach (EscapeBattleRoleInfo escapeBattleRoleInfo in list2)
					{
						escapeBattleRoleInfo.ReliveCount = list.Sum((int[] x) => x[1]);
						escapeBattleRoleInfo.ReliveCount = Math.Min(this.RuntimeData.MaxLifeNum, escapeBattleRoleInfo.ReliveCount);
					}
				}
			}
		}

		public int GetCaiJiMonsterTime(GameClient client, Monster monster)
		{
			EscapeBattleCollection escapeBattleCollection = (monster != null) ? (monster.Tag as EscapeBattleCollection) : null;
			int result;
			if (escapeBattleCollection == null)
			{
				result = -200;
			}
			else
			{
				result = escapeBattleCollection.CollectTime;
			}
			return result;
		}

		private void CheckSceneAreaDamage(EscapeBattleScene scene, long nowTicks)
		{
			if (scene.AreaDamageTicks <= nowTicks)
			{
				EscapeMapSafeArea escapeMapSafeArea = this.RuntimeData.EscapeMapSafeAreaList.Find((EscapeMapSafeArea x) => x.ID == scene.ScoreData.safeArea.AreaID);
				if (null != escapeMapSafeArea)
				{
					scene.AreaDamageTicks = nowTicks + (long)(escapeMapSafeArea.GodFireHitTime * 1000);
					List<GameClient> list = scene.CopyMap.GetClientsList();
					list = Global.GetMapAliveClientsEx(list, scene.m_nMapCode, false, 0L);
					for (int i = 0; i < list.Count; i++)
					{
						GameClient gameClient = list[i];
						if (gameClient != null && gameClient.ClientData.HideGM <= 0)
						{
							Point end = new Point
							{
								X = (double)scene.ScoreData.safeArea.PosX,
								Y = (double)scene.ScoreData.safeArea.PosY
							};
							if (Global.GetTwoPointDistance(gameClient.CurrentPos, end) > (double)escapeMapSafeArea.SafeRadius)
							{
								double godFireHitPercent = escapeMapSafeArea.GodFireHitPercent;
								double num = godFireHitPercent * (1.0 - this.CalClientDehurtValue(scene, gameClient));
								double num2 = (double)gameClient.ClientData.LifeV * num + (double)escapeMapSafeArea.GodFireHitHp;
								int currentLifeV = gameClient.ClientData.CurrentLifeV;
								gameClient.ClientData.CurrentLifeV -= (int)num2;
								num2 = (double)(currentLifeV - gameClient.ClientData.CurrentLifeV);
								if (num2 <= 0.0)
								{
									break;
								}
								GameManager.ClientMgr.SubSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, num2);
								GameManager.ClientMgr.NotifySpriteInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, gameClient.ClientData.MapCode, gameClient.ClientData.RoleID, gameClient.ClientData.RoleID, 0, (int)num2, (double)gameClient.ClientData.CurrentLifeV, gameClient.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
								bool flag = gameClient.ClientData.CurrentLifeV <= 0;
								if (flag)
								{
									this.CheckZhanDuiWinLoseState(scene);
									long num3 = 0L;
									BufferData bufferDataByID = Global.GetBufferDataByID(gameClient, 2090002);
									if (null != bufferDataByID)
									{
										num3 = bufferDataByID.BufferVal;
									}
									this.UpdateBuff4GameClient(gameClient, BufferItemTypes.EscapeBattleDevil, -(int)num3);
								}
							}
						}
					}
				}
			}
		}

		private double CalClientDehurtValue(EscapeBattleScene scene, GameClient client)
		{
			return 0.0;
		}

		private void UpdateBuff4GameClient(GameClient client, BufferItemTypes bufferItem, int modNum)
		{
			if (modNum != 0)
			{
				long num = 0L;
				BufferData bufferDataByID = Global.GetBufferDataByID(client, (int)bufferItem);
				if (null != bufferDataByID)
				{
					num = bufferDataByID.BufferVal;
				}
				num = Math.Max(0L, num + (long)modNum);
				lock (this.RuntimeData.Mutex)
				{
					if (BufferItemTypes.EscapeBattleGod == bufferItem)
					{
						num = Math.Min(num, (long)this.RuntimeData.BuffMaxLayerNum[0]);
					}
					if (BufferItemTypes.EscapeBattleDevil == bufferItem)
					{
						num = Math.Min(num, (long)this.RuntimeData.BuffMaxLayerNum[1]);
					}
				}
				double[] actionParams = new double[]
				{
					(double)num
				};
				Global.UpdateBufferData(client, bufferItem, actionParams, 1, false);
				this.RefreshBuffGameClientProps(client, bufferItem, num);
			}
		}

		private void RefreshBuffGameClientProps(GameClient client, BufferItemTypes bufferItem, long BufferVal)
		{
			EscapeBattleScene escapeBattleScene = client.SceneObject as EscapeBattleScene;
			if (null != escapeBattleScene)
			{
				EscapeBattlePropNotify escapeBattlePropNotify = new EscapeBattlePropNotify();
				double[] array = new double[177];
				escapeBattlePropNotify.MergeProp = new Dictionary<int, double[]>();
				for (int i = 0; i < 177; i++)
				{
					if (BufferItemTypes.EscapeBattleGod == bufferItem)
					{
						array[i] = escapeBattleScene.TopClientCalExtProps[0][i] * (double)BufferVal;
					}
					if (BufferItemTypes.EscapeBattleDevil == bufferItem)
					{
						array[i] = escapeBattleScene.TopClientCalExtProps[1][i] * (double)BufferVal;
					}
				}
				if (BufferItemTypes.EscapeBattleGod == bufferItem)
				{
					escapeBattlePropNotify.Type = 0;
					if (!escapeBattlePropNotify.MergeProp.ContainsKey(escapeBattlePropNotify.Type))
					{
						escapeBattlePropNotify.MergeProp.Add(escapeBattlePropNotify.Type, array);
					}
					else
					{
						escapeBattlePropNotify.MergeProp[escapeBattlePropNotify.Type] = array;
					}
				}
				else
				{
					escapeBattlePropNotify.Type = 1;
					if (!escapeBattlePropNotify.MergeProp.ContainsKey(escapeBattlePropNotify.Type))
					{
						escapeBattlePropNotify.MergeProp.Add(escapeBattlePropNotify.Type, array);
					}
					else
					{
						escapeBattlePropNotify.MergeProp[escapeBattlePropNotify.Type] = array;
					}
				}
				client.ClientData.PurePropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.BufferPropsManager,
					(int)bufferItem,
					array
				});
				GameManager.ClientMgr.NotifyUpdateEscapeBattleProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, escapeBattlePropNotify);
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					default(DelayExecProcIds),
					2
				});
			}
		}

		public static EscapeBattleManager getInstance()
		{
			return EscapeBattleManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("EscapeBattleManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 2000);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2112, 1, 2, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2116, 1, 2, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2117, 1, 2, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2110, 1, 2, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2114, 1, 2, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2111, 1, 1, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2115, 1, 1, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2119, 1, 1, EscapeBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(10, EscapeBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(13, EscapeBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, EscapeBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(12, EscapeBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(21, 58, EscapeBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(61, 10007, EscapeBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(62, 10007, EscapeBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(60, 59, EscapeBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10002, 59, EscapeBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(63, 10000, EscapeBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(65, 10000, EscapeBattleManager.getInstance());
			this.NotifyEnterHandler = new EventSourceEx<KFCallMsg>.HandlerData
			{
				ID = 59,
				EventType = 10037,
				Handler = new Func<KFCallMsg, bool>(this.KFCallMsgFunc)
			};
			this.NotifyGameStateHandler = new EventSourceEx<KFCallMsg>.HandlerData
			{
				ID = 59,
				EventType = 10038,
				Handler = new Func<KFCallMsg, bool>(this.KFCallMsgFunc)
			};
			KFCallManager.MsgSource.registerListener(10037, this.NotifyEnterHandler);
			KFCallManager.MsgSource.registerListener(10038, this.NotifyGameStateHandler);
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(10, EscapeBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(13, EscapeBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, EscapeBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(21, 58, EscapeBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(61, 10007, EscapeBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(62, 10007, EscapeBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(60, 59, EscapeBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10002, 59, EscapeBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(63, 10000, EscapeBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(65, 10000, EscapeBattleManager.getInstance());
			KFCallManager.MsgSource.removeListener(10037, this.NotifyEnterHandler);
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
			case 2110:
				return this.ProcessEscapeEnterCmd(client, nID, bytes, cmdParams);
			case 2112:
				return this.ProcessGetMainInfoListCmd(client, nID, bytes, cmdParams);
			case 2114:
				return this.ProcessEscapeRankInfoCmd(client, nID, bytes, cmdParams);
			case 2115:
				return this.ProcessGetPaiHangAwardsCmd(client, nID, bytes, cmdParams);
			case 2116:
				return this.ProcessEscapeJoinCmd(client, nID, bytes, cmdParams);
			case 2117:
				return this.ProcessEscapeInviteCmd(client, nID, bytes, cmdParams);
			case 2119:
				return this.ProcessEscapeDevilBuyCmd(client, nID, bytes, cmdParams);
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
			else if (eventType == 28)
			{
				OnStartPlayGameEventObject onStartPlayGameEventObject = eventObject as OnStartPlayGameEventObject;
				if (null != onStartPlayGameEventObject)
				{
					this.OnStartPlayGame(onStartPlayGameEventObject.Client);
				}
			}
			else if (eventObject.getEventType() == 13)
			{
				PlayerLeaveFuBenEventObject playerLeaveFuBenEventObject = (PlayerLeaveFuBenEventObject)eventObject;
				this.RoleLeaveFuBen(playerLeaveFuBenEventObject.getPlayer());
			}
			else if (eventObject.getEventType() == 12)
			{
				PlayerLogoutEventObject playerLogoutEventObject = (PlayerLogoutEventObject)eventObject;
				if (playerLogoutEventObject.getPlayer().ClientData.SceneType == 58)
				{
					this.CancleJoinState(playerLogoutEventObject.getPlayer());
				}
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			if (eventType != 21)
			{
				switch (eventType)
				{
				case 60:
					this.NotifyTimeStateInfoAndScoreInfo(eventObject.Sender as GameClient, true, true);
					break;
				case 61:
				{
					EventObjectEx_I1 eventObjectEx_I = eventObject as EventObjectEx_I1;
					if (eventObjectEx_I != null && eventObjectEx_I.Param1 == 37)
					{
						eventObject.Handled = true;
						if (this.OnKuaFuLogin(eventObject.Sender as KuaFuServerLoginData))
						{
							eventObject.Result = true;
						}
					}
					break;
				}
				case 62:
				{
					EventObjectEx_I1 eventObjectEx_I = eventObject as EventObjectEx_I1;
					if (eventObjectEx_I != null && eventObjectEx_I.Param1 == 37)
					{
						eventObject.Handled = true;
						if (this.OnKuaFuInitGame(eventObject.Sender as GameClient))
						{
							eventObject.Handled = true;
							eventObject.Result = true;
						}
					}
					break;
				}
				case 63:
				{
					PreZhanDuiChangeMemberEventObject preZhanDuiChangeMemberEventObject = (PreZhanDuiChangeMemberEventObject)eventObject;
					preZhanDuiChangeMemberEventObject.Handled = this.OnPreZhanDuiChangeMember(preZhanDuiChangeMemberEventObject);
					break;
				}
				case 64:
					break;
				case 65:
					if (null != eventObject)
					{
						if ((int)eventObject.Args[1] == this.RuntimeData.ReadyMapCode && (int)eventObject.Args[2] != this.RuntimeData.ReadyMapCode)
						{
							this.CancleJoinState(eventObject.Args[0] as GameClient);
						}
					}
					break;
				default:
					if (eventType == 10002)
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
					break;
				}
			}
			else
			{
				PreGotoLastMapEventObject preGotoLastMapEventObject = eventObject as PreGotoLastMapEventObject;
				if (preGotoLastMapEventObject != null && preGotoLastMapEventObject.SceneType == 58)
				{
					this.CancleJoinState(preGotoLastMapEventObject.Player);
				}
			}
		}

		public bool KFCallMsgFunc(KFCallMsg msg)
		{
			switch (msg.KuaFuEventType)
			{
			case 10037:
				if (!GameManager.IsKuaFuServer)
				{
					EscapeBattleNtfEnterData escapeBattleNtfEnterData = msg.Get<EscapeBattleNtfEnterData>();
					if (null != escapeBattleNtfEnterData)
					{
						string arg = string.Join<int>("|", escapeBattleNtfEnterData.ZhanDuiIDList.ToArray());
						LogManager.WriteLog(2, string.Format("通知战队:{0} 拥有进入魔界大逃杀资格", arg), null, true);
						DateTime fightTime = TimeUtil.NowDateTime().Date.Add(this.GetStartTime(0)).AddSeconds((double)(this.RuntimeData.Config.MatchConfigList[0].WaitSeconds + this.RuntimeData.Config.MatchConfigList[0].SafeSecs));
						DateTime endTime = fightTime.AddSeconds((double)this.RuntimeData.Config.MatchConfigList[0].BattleEndTime);
						foreach (int num in escapeBattleNtfEnterData.ZhanDuiIDList)
						{
							TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(num, GameManager.ServerId);
							if (null != zhanDuiData)
							{
								lock (this.RuntimeData.Mutex)
								{
									EscapeBattlePiPeiState escapeBattlePiPeiState;
									if (this.RuntimeData.ConfirmBattleDict.TryGetValue(num, out escapeBattlePiPeiState))
									{
										escapeBattlePiPeiState.GameID = escapeBattleNtfEnterData.GameId;
										escapeBattlePiPeiState.State = 3;
										escapeBattlePiPeiState.FightTime = fightTime;
										escapeBattlePiPeiState.EndTime = endTime;
									}
								}
								foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in zhanDuiData.teamerList)
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(tianTi5v5ZhanDuiRoleData.RoleID);
									if (gameClient != null && gameClient.ClientData.SceneMapCode == this.RuntimeData.ReadyMapCode)
									{
										this.ProcessEscapeEnterCmd(gameClient, 2110, null, null);
									}
								}
							}
						}
					}
				}
				break;
			case 10038:
				lock (this.RuntimeData.Mutex)
				{
					int[] array = msg.Get<int[]>();
					if (array != null && array.Length >= 3)
					{
						lock (this.RuntimeData.Mutex)
						{
							EscapeBattlePiPeiState escapeBattlePiPeiState;
							if (this.RuntimeData.ConfirmBattleDict.TryGetValue(array[0], out escapeBattlePiPeiState) && escapeBattlePiPeiState.GameID == array[1])
							{
								if (escapeBattlePiPeiState.State >= 3)
								{
									escapeBattlePiPeiState.State = array[2];
								}
							}
						}
					}
				}
				break;
			}
			return true;
		}

		public bool InitConfig()
		{
			bool result = true;
			string text = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.TeamBattleMap = GameManager.systemParamsList.GetParamValueIntArrayByName("EscapeTeamApply", ',');
					Dictionary<int, MapSettingItem> value = Data.SettingsDict.Value;
					if (null != value)
					{
						foreach (MapSettingItem mapSettingItem in value.Values)
						{
							if (mapSettingItem.MapType == 58)
							{
								this.RuntimeData.ReadyMapCode = mapSettingItem.Code;
								break;
							}
						}
					}
					if (!this.RuntimeData.Config.Load(Global.GameResPath("Config\\EscapeActivityRules.xml")))
					{
						return false;
					}
					EscapeBattleMatchConfig escapeBattleMatchConfig = this.RuntimeData.Config.MatchConfigList[0];
					EscapeBattleConsts.MinZhanDuiNumPerGame = escapeBattleMatchConfig.MatchTeamNum;
					EscapeBattleConsts.MinRoleNumPerGame = escapeBattleMatchConfig.EnterBattleNum;
					EscapeBattleConsts.BattleSignSecs = escapeBattleMatchConfig.BattleSignSecs;
					this.RuntimeData.AwardsConfig = new List<EscapeBattleAwardsConfig>();
					text = "Config/EscapeRankAward.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						EscapeBattleAwardsConfig escapeBattleAwardsConfig = new EscapeBattleAwardsConfig();
						escapeBattleAwardsConfig.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						escapeBattleAwardsConfig.MinRank = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "StarRank", 0L);
						escapeBattleAwardsConfig.MaxRank = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "EndRank", 0L);
						if (escapeBattleAwardsConfig.MinRank == -1)
						{
							escapeBattleAwardsConfig.MinRank = 1;
						}
						if (escapeBattleAwardsConfig.MaxRank == -1)
						{
							escapeBattleAwardsConfig.MaxRank = int.MaxValue;
						}
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "Award");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							ConfigParser.ParseAwardsItemList(safeAttributeStr, ref escapeBattleAwardsConfig.Award, '|', ',');
						}
						this.RuntimeData.AwardsConfig.Add(escapeBattleAwardsConfig);
					}
					this.RuntimeData.DuanAwardsConfig = new List<EscapeBDuanAwardsConfig>();
					text = "Config/EscapeDanList.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						EscapeBDuanAwardsConfig escapeBDuanAwardsConfig = new EscapeBDuanAwardsConfig();
						escapeBDuanAwardsConfig.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						escapeBDuanAwardsConfig.RankValue = (int)Global.GetSafeAttributeLong(xelement2, "RankValue");
						escapeBDuanAwardsConfig.WinRankValue = ConfigHelper.ParserIntArrayList(Global.GetSafeAttributeStr(xelement2, "WinRankValue"), true, '|', ',');
						escapeBDuanAwardsConfig.LoseRankValue = ConfigHelper.ParserIntArrayList(Global.GetSafeAttributeStr(xelement2, "LoseRankValue"), true, '|', ',');
						escapeBDuanAwardsConfig.LoseRankValue.Sort(delegate(List<int> left, List<int> right)
						{
							int result2;
							if (left[0] < right[0])
							{
								result2 = -1;
							}
							else if (left[0] > right[0])
							{
								result2 = 1;
							}
							else
							{
								result2 = 0;
							}
							return result2;
						});
						escapeBDuanAwardsConfig.RankLevelName = Global.GetSafeAttributeStr(xelement2, "RankLevelName");
						AwardsItemList firstWinAwardsItemList = new AwardsItemList();
						escapeBDuanAwardsConfig.FirstWinAwardsItemList = firstWinAwardsItemList;
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xelement2, "FirstWinRankReward"), ref firstWinAwardsItemList, '|', ',');
						AwardsItemList winAwardsItemList = new AwardsItemList();
						escapeBDuanAwardsConfig.WinAwardsItemList = winAwardsItemList;
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xelement2, "WinRankReward"), ref winAwardsItemList, '|', ',');
						AwardsItemList loseAwardsItemList = new AwardsItemList();
						escapeBDuanAwardsConfig.LoseAwardsItemList = loseAwardsItemList;
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xelement2, "LoseRankReward"), ref loseAwardsItemList, '|', ',');
						this.RuntimeData.DuanAwardsConfig.Add(escapeBDuanAwardsConfig);
					}
					this.RuntimeData.MapBirthPointDict = new Dictionary<int, EscapeBattleBirthPoint>();
					text = "Config/EscapePlayPoint.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						EscapeBattleBirthPoint escapeBattleBirthPoint = new EscapeBattleBirthPoint();
						escapeBattleBirthPoint.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						escapeBattleBirthPoint.MapCodeID = (int)Global.GetSafeAttributeLong(xelement2, "MapId");
						string[] array = Global.GetSafeAttributeStr(xelement2, "MapTeamPoint").Split(new char[]
						{
							','
						});
						if (array.Length == 2)
						{
							escapeBattleBirthPoint.PosX = Global.SafeConvertToInt32(array[0]);
							escapeBattleBirthPoint.PosY = Global.SafeConvertToInt32(array[1]);
						}
						escapeBattleBirthPoint.BirthRadius = (int)Global.GetSafeAttributeLong(xelement2, "MapTeamRange");
						this.RuntimeData.MapBirthPointDict[escapeBattleBirthPoint.ID] = escapeBattleBirthPoint;
					}
					this.RuntimeData.CollectionConfigList = new List<EscapeBattleCollection>();
					text = "Config/EscapeMapCollection.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						EscapeBattleCollection escapeBattleCollection = new EscapeBattleCollection();
						escapeBattleCollection.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						escapeBattleCollection.MapCodeID = (int)Global.GetSafeAttributeLong(xelement2, "MapId");
						escapeBattleCollection.cType = (EscapeBCollectType)Global.GetSafeAttributeLong(xelement2, "CollectType");
						escapeBattleCollection.eState = (int)Global.GetSafeAttributeLong(xelement2, "CollectRefreshStage");
						escapeBattleCollection.RefreshRegion = (int)Global.GetSafeAttributeLong(xelement2, "RefreshRegion");
						escapeBattleCollection.RefreshTime = (int)Global.GetSafeAttributeLong(xelement2, "RefreshTime");
						escapeBattleCollection.RefreshMonsterId = (int)Global.GetSafeAttributeLong(xelement2, "RefreshMonsterId");
						escapeBattleCollection.RefreshMonsterNum = (int)Global.GetSafeAttributeLong(xelement2, "RefreshMonsterNum");
						escapeBattleCollection.CollectTime = (int)Global.GetSafeAttributeLong(xelement2, "CollectTime");
						escapeBattleCollection.CollectGodNum = (int)Global.GetSafeAttributeLong(xelement2, "CollectGodNum");
						escapeBattleCollection.CollectLiveTime = (int)Global.GetSafeAttributeLong(xelement2, "CollectLiveTime");
						escapeBattleCollection.IsDeath = (int)Global.GetSafeAttributeLong(xelement2, "IsDeath");
						this.RuntimeData.CollectionConfigList.Add(escapeBattleCollection);
					}
					this.RuntimeData.EscapeMapSafeAreaList = new List<EscapeMapSafeArea>();
					text = "Config/EscapeMapSafeArea.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						EscapeMapSafeArea escapeMapSafeArea = new EscapeMapSafeArea();
						escapeMapSafeArea.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						escapeMapSafeArea.eState = (int)Global.GetSafeAttributeLong(xelement2, "RefreshStage");
						escapeMapSafeArea.TimeStage = (int)Global.GetSafeAttributeLong(xelement2, "TimeStage");
						escapeMapSafeArea.StartSafePoint = Global.GetSafeAttributeIntArray(xelement2, "StartSafePoint", -1, '|');
						escapeMapSafeArea.SafeRadius = (int)Global.GetSafeAttributeLong(xelement2, "SafeRadius");
						escapeMapSafeArea.GodFireHitTime = (int)Global.GetSafeAttributeLong(xelement2, "GodFireHitTime");
						escapeMapSafeArea.GodFireHitPercent = Global.GetSafeAttributeDouble(xelement2, "GodFireHitPercent");
						escapeMapSafeArea.GodFireHitHp = (int)Global.GetSafeAttributeLong(xelement2, "GodFireHitHp");
						this.RuntimeData.EscapeMapSafeAreaList.Add(escapeMapSafeArea);
					}
					this.RuntimeData.EscapeMapSafeAreaList.Sort(delegate(EscapeMapSafeArea left, EscapeMapSafeArea right)
					{
						int result2;
						if (left.SafeRadius > right.SafeRadius)
						{
							result2 = -1;
						}
						else if (left.SafeRadius < right.SafeRadius)
						{
							result2 = 1;
						}
						else
						{
							result2 = 0;
						}
						return result2;
					});
					this.RuntimeData.BuyDevilLossDiamonds = (int)GameManager.systemParamsList.GetParamValueIntByName("BuyDevilLossDiamonds", -1);
					this.RuntimeData.KillReplyHp = (int)GameManager.systemParamsList.GetParamValueIntByName("KillReplyHp", -1);
					this.RuntimeData.BuffMaxLayerNum = GameManager.systemParamsList.GetParamValueIntArrayByName("BuffMaxLayerNum", '|');
					this.RuntimeData.BuffAttributeProportion = GameManager.systemParamsList.GetParamValueDoubleArrayByName("BuffAttributeProportion", '|');
					this.RuntimeData.BuffAttributeType = GameManager.systemParamsList.GetParamValueIntArrayByName("BuffAttributeType", ',');
					this.RuntimeData.LifeSeedNum = new List<int[]>();
					List<string> paramValueStringListByName = GameManager.systemParamsList.GetParamValueStringListByName("LifeSeedNum", '|');
					foreach (string text2 in paramValueStringListByName)
					{
						this.RuntimeData.LifeSeedNum.Add(Global.StringArray2IntArray(text2.Split(new char[]
						{
							','
						})));
					}
					this.RuntimeData.MaxLifeNum = (int)GameManager.systemParamsList.GetParamValueIntByName("MaxLifeNum", -1);
					this.RuntimeData.DevilLossNum = (int)GameManager.systemParamsList.GetParamValueIntByName("DevilLossNum", -1);
					this.RuntimeData.ReadyMapCode = (int)GameManager.systemParamsList.GetParamValueIntByName("TeamEnterMap", -1);
					DateTime.TryParse(GameManager.systemParamsList.GetParamValueByName("EscapeStartTime"), out this.RuntimeData.EscapeStartTime);
					List<string> paramValueStringListByName2 = GameManager.systemParamsList.GetParamValueStringListByName("EscapeAttribute", '|');
					foreach (string text2 in paramValueStringListByName2)
					{
						double[] array2 = Global.String2DoubleArray(text2, ',');
						this.RuntimeData.DebuffCalExtProps[(int)array2[0]] = array2[1];
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

		public bool ProcessGetMainInfoListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = 0;
				int zhanDuiID = client.ClientData.ZhanDuiID;
				int num3 = 0;
				if (!this.CheckOpenState(TimeUtil.NowDateTime()))
				{
					num = 6;
				}
				else
				{
					EscapeBattleAwardsConfig escapeBattleAwardsConfig = null;
					num3 = this.CanGetMonthRankAwards(client, out escapeBattleAwardsConfig);
					DateTime dateTime = TimeUtil.NowDateTime().Add(this.RuntimeData.DiffTimeSpan);
					TimeSpan timeSpan = TimeUtil.TimeOfWeek(dateTime);
					lock (this.RuntimeData.Mutex)
					{
						if (Consts.TestMode)
						{
							num2 = (int)(this.RuntimeData.Config.MatchConfigList[0].TimePoints[0] - timeSpan).TotalSeconds % 604800;
							num = 1;
						}
						else
						{
							num2 = (int)this.RuntimeData.DiffTimeSpan.TotalSeconds % 7 * 86400;
							foreach (EscapeBattleMatchConfig escapeBattleMatchConfig in this.RuntimeData.Config.MatchConfigList)
							{
								for (int i = 0; i < escapeBattleMatchConfig.TimePoints.Count - 1; i += 2)
								{
									if (escapeBattleMatchConfig.TimePoints[i] <= timeSpan && timeSpan < escapeBattleMatchConfig.TimePoints[i + 1])
									{
										num = 1;
										break;
									}
								}
							}
						}
						EscapeBattlePiPeiState escapeBattlePiPeiState;
						if (this.RuntimeData.ConfirmBattleDict.TryGetValue(zhanDuiID, out escapeBattlePiPeiState))
						{
							if (escapeBattlePiPeiState.State == 2)
							{
								num = escapeBattlePiPeiState.State;
							}
							else if (escapeBattlePiPeiState.State == 3)
							{
								ReturnValue<int> zhanDuiState = TcpCall.EscapeBattle_K.GetZhanDuiState(client.ClientData.ZhanDuiID);
								if (zhanDuiState.IsReturn)
								{
									escapeBattlePiPeiState.State = zhanDuiState.Value;
									if (zhanDuiState.Value != 3)
									{
										escapeBattlePiPeiState.GameID = 0;
									}
									if (escapeBattlePiPeiState.State != 0)
									{
										num = zhanDuiState.Value;
									}
								}
								else
								{
									num = 0;
								}
							}
						}
					}
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", num2, num, num3), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessEscapeJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = 0;
				if (this.IsGongNengOpened(client, false))
				{
					int zhanDuiID = client.ClientData.ZhanDuiID;
					if (zhanDuiID <= 0)
					{
						cmdData = -4013;
					}
					else if (client.ClientData.ZhanDuiZhiWu != 1)
					{
						cmdData = -4016;
					}
					else if (!this.CheckMap(client))
					{
						cmdData = -21;
					}
					else if (!this.CheckTime())
					{
						cmdData = -2001;
					}
					else
					{
						EscapeBattlePiPeiState escapeBattlePiPeiState = null;
						bool flag = false;
						try
						{
							object mutex;
							Monitor.Enter(mutex = this.RuntimeData.Mutex, ref flag);
							if (!this.RuntimeData.TeamBattleMap.Contains(client.ClientData.MapCode))
							{
								cmdData = -21;
								goto IL_316;
							}
							EscapeBattleMatchConfig config = this.RuntimeData.Config.MatchConfigList[0];
							if (config.SignCondition == null)
							{
								cmdData = -3;
								goto IL_316;
							}
							if (!this.RuntimeData.ConfirmBattleDict.TryGetValue(zhanDuiID, out escapeBattlePiPeiState))
							{
								escapeBattlePiPeiState = new EscapeBattlePiPeiState();
								this.RuntimeData.ConfirmBattleDict[zhanDuiID] = escapeBattlePiPeiState;
							}
							if (escapeBattlePiPeiState.State >= 2)
							{
								cmdData = -5;
								goto IL_316;
							}
							TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(zhanDuiID, client.ServerId);
							if (!Consts.TestMode && zhanDuiData.teamerList.Count < config.EnterBattleNum)
							{
								cmdData = -4026;
								goto IL_316;
							}
							if (!Consts.TestMode && zhanDuiData.teamerList.Any(delegate(TianTi5v5ZhanDuiRoleData x)
							{
								int unionLevel = Global.GetUnionLevel(x.ZhuanSheng, x.Level, false);
								int unionLevel2 = Global.GetUnionLevel(config.SignCondition[0], config.SignCondition[1], false);
								bool result;
								if (unionLevel < unionLevel2)
								{
									result = true;
								}
								else
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(x.RoleID);
									result = (gameClient != null && Global.GetUnionLevel(gameClient, false) < unionLevel2);
								}
								return result;
							}))
							{
								cmdData = -19;
								goto IL_316;
							}
							escapeBattlePiPeiState.RoleList = new List<EscapeBattleJoinRoleInfo>();
							foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in zhanDuiData.teamerList)
							{
								EscapeBattleJoinRoleInfo escapeBattleJoinRoleInfo = new EscapeBattleJoinRoleInfo
								{
									RoleID = tianTi5v5ZhanDuiRoleData.RoleID,
									RoleName = tianTi5v5ZhanDuiRoleData.RoleName,
									Level = tianTi5v5ZhanDuiRoleData.Level,
									ChangeLevel = tianTi5v5ZhanDuiRoleData.ZhuanSheng,
									CombatForce = tianTi5v5ZhanDuiRoleData.ZhanLi,
									IsLeader = (tianTi5v5ZhanDuiRoleData.RoleID == zhanDuiData.LeaderRoleID)
								};
								escapeBattlePiPeiState.RoleList.Add(escapeBattleJoinRoleInfo);
								if (escapeBattleJoinRoleInfo.IsLeader)
								{
									escapeBattleJoinRoleInfo.Join = true;
								}
							}
							escapeBattlePiPeiState.EscapeJiFen = zhanDuiData.EscapeJiFen;
							escapeBattlePiPeiState.State = 2;
						}
						finally
						{
							if (flag)
							{
								object mutex;
								Monitor.Exit(mutex);
							}
						}
						GameManager.ClientMgr.BroadZhanDuiMessage<List<EscapeBattleJoinRoleInfo>>(2118, escapeBattlePiPeiState.RoleList, zhanDuiID);
						Global.GotoMap(client, this.RuntimeData.ReadyMapCode);
					}
				}
				IL_316:
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessEscapeInviteCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = 0;
				if (this.IsGongNengOpened(client, false))
				{
					int zhanDuiID = client.ClientData.ZhanDuiID;
					if (zhanDuiID <= 0)
					{
						cmdData = -4013;
					}
					else if (client.ClientData.ZhanDuiZhiWu != 1)
					{
						cmdData = -4016;
					}
					else if (!this.CheckMap(client))
					{
						cmdData = -21;
					}
					else if (!this.CheckTime())
					{
						cmdData = -2001;
					}
					else
					{
						EscapeBattlePiPeiState escapeBattlePiPeiState = null;
						lock (this.RuntimeData.Mutex)
						{
							if (!this.RuntimeData.ConfirmBattleDict.TryGetValue(zhanDuiID, out escapeBattlePiPeiState))
							{
								cmdData = -4035;
								goto IL_12F;
							}
							if (escapeBattlePiPeiState.State < 2)
							{
								cmdData = -4035;
								goto IL_12F;
							}
							if (escapeBattlePiPeiState.State >= 3)
							{
								cmdData = -4037;
								goto IL_12F;
							}
						}
						GameManager.ClientMgr.BroadZhanDuiMessage<List<EscapeBattleJoinRoleInfo>>(2118, escapeBattlePiPeiState.RoleList, zhanDuiID);
					}
				}
				IL_12F:
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessEscapeRankInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				EscapeBattleRankInfo escapeBattleRankInfo = new EscapeBattleRankInfo();
				int zhanDuiID = client.ClientData.ZhanDuiID;
				if (zhanDuiID > 0)
				{
					if (!GameManager.IsKuaFuServer)
					{
						TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(zhanDuiID, client.ServerId);
						lock (this.RuntimeData.Mutex)
						{
							escapeBattleRankInfo.myZhanDuiRankInfo.Key = zhanDuiID;
							escapeBattleRankInfo.myZhanDuiRankInfo.Param1 = zhanDuiData.ZhanDouLi;
							escapeBattleRankInfo.myZhanDuiRankInfo.StrParam1 = zhanDuiData.ZhanDuiName;
							escapeBattleRankInfo.myZhanDuiRankInfo.ZoneID = zhanDuiData.ZoneID;
							escapeBattleRankInfo.myZhanDuiRankInfo.Value = zhanDuiData.EscapeJiFen;
							if (this.RuntimeData.SyncData.RankList != null && this.RuntimeData.SyncData.RankList.Count > 0)
							{
								escapeBattleRankInfo.SelfRank = this.RuntimeData.SyncData.RankList.FindIndex((KFEscapeRankInfo x) => x.Key == zhanDuiID) + 1;
								int count = Math.Min(this.RuntimeData.SyncData.RankList.Count, EscapeBattleConsts.MaxRankNum);
								escapeBattleRankInfo.rankInfo2Client = this.RuntimeData.SyncData.RankList.GetRange(0, count);
							}
						}
					}
				}
				client.sendCmd<EscapeBattleRankInfo>(nID, escapeBattleRankInfo, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessEscapeEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = 0;
				if (!this.CheckMap(client))
				{
					cmdData = -12;
				}
				else
				{
					int zhanDuiID = client.ClientData.ZhanDuiID;
					DateTime t = TimeUtil.NowDateTime().Add(this.RuntimeData.DiffTimeSpan);
					lock (this.RuntimeData.Mutex)
					{
						EscapeBattlePiPeiState escapeBattlePiPeiState;
						if (!this.RuntimeData.ConfirmBattleDict.TryGetValue(zhanDuiID, out escapeBattlePiPeiState) || escapeBattlePiPeiState.State == 0)
						{
							cmdData = -4038;
							goto IL_233;
						}
						if (escapeBattlePiPeiState.State == 2)
						{
							if (client.ClientData.MapCode != this.RuntimeData.ReadyMapCode)
							{
								Global.GotoMap(client, this.RuntimeData.ReadyMapCode);
								goto IL_233;
							}
						}
						else if (escapeBattlePiPeiState.State == 3)
						{
							if (t > escapeBattlePiPeiState.FightTime)
							{
								cmdData = -2008;
								goto IL_233;
							}
						}
						else if (escapeBattlePiPeiState.State == 4)
						{
							cmdData = -4006;
							goto IL_233;
						}
					}
					int num = 0;
					int targetServerID;
					string[] array;
					int[] array2;
					ReturnValue<int> returnValue = TcpCall.EscapeBattle_K.ZhengBaRequestEnter(client.ClientData.ZhanDuiID, out num, out targetServerID, out array, out array2);
					if (returnValue.Type != 7 || returnValue.Value < 0)
					{
						cmdData = returnValue.Value;
					}
					else
					{
						KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
						clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
						clientKuaFuServerLoginData.ServerId = client.ServerId;
						clientKuaFuServerLoginData.GameType = 37;
						clientKuaFuServerLoginData.GameId = (long)num;
						clientKuaFuServerLoginData.EndTicks = TimeUtil.UTCTicks();
						clientKuaFuServerLoginData.TargetServerID = targetServerID;
						clientKuaFuServerLoginData.ServerIp = array[0];
						clientKuaFuServerLoginData.ServerPort = array2[0];
						clientKuaFuServerLoginData.Param1 = client.ClientData.ZhanDuiID;
						GlobalNew.RecordSwitchKuaFuServerLog(client);
						client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
					}
				}
				IL_233:
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetPaiHangAwardsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = -20;
				EscapeBattleAwardsConfig escapeBattleAwardsConfig = null;
				int num = this.CanGetMonthRankAwards(client, out escapeBattleAwardsConfig);
				if (num > 0)
				{
					List<GoodsData> list = Global.ConvertToGoodsDataList(escapeBattleAwardsConfig.Award.Items, -1);
					if (!Global.CanAddGoodsDataList(client, list))
					{
						cmdData = -100;
					}
					else
					{
						this.SetSeasonRankDataFlags(client, false, true);
						cmdData = 0;
						for (int i = 0; i < list.Count; i++)
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, list[i].GoodsID, list[i].GCount, list[i].Quality, "", list[i].Forge_level, list[i].Binding, 0, "", true, 1, "天梯月段位排名奖励", "1900-01-01 12:00:00", 0, list[i].BornIndex, list[i].Lucky, 0, list[i].ExcellenceInfo, list[i].AppendPropLev, 0, null, null, 0, true);
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

		public bool ProcessEscapeDevilBuyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = 0;
				EscapeBattleScene escapeBattleScene = client.SceneObject as EscapeBattleScene;
				if (escapeBattleScene == null || escapeBattleScene.m_eStatus != 4)
				{
					cmdData = -5;
					client.sendCmd<int>(nID, cmdData, false);
					return true;
				}
				long num = 0L;
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 2090002);
				if (null != bufferDataByID)
				{
					num = bufferDataByID.BufferVal;
				}
				int num2 = 0;
				lock (this.RuntimeData.Mutex)
				{
					num2 = (int)((long)this.RuntimeData.BuffMaxLayerNum[1] - num) * this.RuntimeData.BuyDevilLossDiamonds;
				}
				if (num2 <= 0)
				{
					cmdData = -5;
					client.sendCmd<int>(nID, cmdData, false);
					return true;
				}
				if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num2, "魔界大逃杀购买魔神契约", true, true, false, DaiBiSySType.None))
				{
					cmdData = -10;
					client.sendCmd<int>(nID, cmdData, false);
					return true;
				}
				lock (this.RuntimeData.Mutex)
				{
					this.UpdateBuff4GameClient(client, BufferItemTypes.EscapeBattleDevil, this.RuntimeData.BuffMaxLayerNum[1]);
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

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, 115, false);
		}

		private bool CheckMap(GameClient client)
		{
			bool result;
			if (client.ClientData.MapCode == this.RuntimeData.ReadyMapCode)
			{
				result = true;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					result = this.RuntimeData.TeamBattleMap.Contains(client.ClientData.MapCode);
				}
			}
			return result;
		}

		public void OnStartPlayGame(GameClient client)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			DateTime dateTime = TimeUtil.NowDateTime();
			DateTime dateTime2 = TimeUtil.NowDateTime().Add(this.RuntimeData.DiffTimeSpan);
			lock (this.RuntimeData.Mutex)
			{
				if (client.ClientData.MapCode == this.RuntimeData.ReadyMapCode)
				{
					int num = (int)dateTime2.TimeOfDay.TotalSeconds % EscapeBattleConsts.BattleSignSecs;
					GameSceneStateTimeData gameSceneStateTimeData = new GameSceneStateTimeData();
					gameSceneStateTimeData.GameType = 37;
					gameSceneStateTimeData.State = 1;
					gameSceneStateTimeData.EndTicks = dateTime.AddSeconds((double)(EscapeBattleConsts.BattleSignSecs - num)).Ticks / 10000L;
					client.sendCmd<GameSceneStateTimeData>(827, gameSceneStateTimeData, false);
				}
				else if (59 == mapSceneType)
				{
					client.ClientData.PctPropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.BufferPropsManager,
						9050,
						this.RuntimeData.DebuffCalExtProps
					});
					client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
					{
						default(DelayExecProcIds),
						2
					});
					EscapeBattleScene escapeBattleScene = client.SceneObject as EscapeBattleScene;
					if (null != escapeBattleScene)
					{
						EscapeBattleTeamInfo escapeBattleTeamInfo = escapeBattleScene.ScoreData.BattleTeamList.Find((EscapeBattleTeamInfo x) => x.TeamID == client.ClientData.ZhanDuiID);
						if (null != escapeBattleTeamInfo)
						{
							this.RefreshTeamMemberReliveCount(escapeBattleScene, escapeBattleTeamInfo);
						}
					}
				}
			}
		}

		private bool CheckTime()
		{
			bool result;
			if (Consts.TestMode)
			{
				result = true;
			}
			else
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				lock (this.RuntimeData.Mutex)
				{
					for (int i = 0; i < this.RuntimeData.Config.MatchConfigList.Count; i++)
					{
						EscapeBattleMatchConfig escapeBattleMatchConfig = this.RuntimeData.Config.MatchConfigList[i];
						for (int j = 0; j < escapeBattleMatchConfig.TimePoints.Count; j += 2)
						{
							if (dateTime.DayOfWeek == (DayOfWeek)escapeBattleMatchConfig.TimePoints[j].Days && dateTime.TimeOfDay.TotalSeconds >= escapeBattleMatchConfig.SecondsOfDay[j] && dateTime.TimeOfDay.TotalSeconds <= escapeBattleMatchConfig.SecondsOfDay[j + 1])
							{
								return true;
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		private TimeSpan GetStartTime(int MapCodeID)
		{
			EscapeBattleMatchConfig escapeBattleMatchConfig = null;
			TimeSpan timeSpan = TimeSpan.MinValue;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				escapeBattleMatchConfig = this.RuntimeData.Config.MatchConfigList.FirstOrDefault<EscapeBattleMatchConfig>();
				if (null == escapeBattleMatchConfig)
				{
					goto IL_189;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < escapeBattleMatchConfig.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)escapeBattleMatchConfig.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= escapeBattleMatchConfig.SecondsOfDay[i] && dateTime.TimeOfDay.TotalSeconds <= escapeBattleMatchConfig.SecondsOfDay[i + 1])
					{
						double num = dateTime.TimeOfDay.TotalSeconds + (double)(escapeBattleMatchConfig.BattleSignSecs / 4);
						int num2 = (int)(num - escapeBattleMatchConfig.SecondsOfDay[i]);
						int num3 = (int)num - num2 % escapeBattleMatchConfig.BattleSignSecs;
						timeSpan = TimeSpan.FromSeconds((double)num3);
						break;
					}
				}
			}
			IL_189:
			if (timeSpan < TimeSpan.Zero)
			{
				timeSpan = dateTime.TimeOfDay;
			}
			return timeSpan;
		}

		private bool OnKuaFuLogin(KuaFuServerLoginData data)
		{
			EscapeBattleFuBenData escapeBattleFuBenData = null;
			int param = data.Param1;
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.KuaFuCopyDataDict.TryGetValue(data.GameId, out escapeBattleFuBenData);
			}
			if (null == escapeBattleFuBenData)
			{
				ReturnValue<int> returnValue = TcpCall.EscapeBattle_K.ZhengBaKuaFuLogin(param, (int)data.GameId, data.ServerId, out escapeBattleFuBenData);
				if (!returnValue.IsReturn || returnValue.Value < 0)
				{
					return false;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.KuaFuCopyDataDict.ContainsKey(data.GameId))
					{
						this.RuntimeData.KuaFuCopyDataDict[data.GameId] = escapeBattleFuBenData;
					}
				}
			}
			bool result;
			if (escapeBattleFuBenData != null && GameManager.ServerId == escapeBattleFuBenData.ServerID && escapeBattleFuBenData.SideDict.ContainsKey((long)param))
			{
				data.ips = escapeBattleFuBenData.IPs;
				data.ports = escapeBattleFuBenData.Ports;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool OnKuaFuInitGame(GameClient client)
		{
			int zhanDuiID = client.ClientData.ZhanDuiID;
			int num = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
			bool result;
			if (num <= 0 || zhanDuiID <= 0)
			{
				result = false;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					EscapeBattleMatchConfig escapeBattleMatchConfig = this.RuntimeData.Config.MatchConfigList.First<EscapeBattleMatchConfig>();
					EscapeBattleFuBenData escapeBattleFuBenData = null;
					if (!this.RuntimeData.KuaFuCopyDataDict.TryGetValue((long)num, out escapeBattleFuBenData))
					{
						LogManager.WriteLog(2, string.Format("未找到活动KuaFuCopyData数据,roleid={0},gameid={1},mapcode={2]", client.ClientData.RoleID, num, escapeBattleMatchConfig.MapCode), null, true);
						result = false;
					}
					else
					{
						if (escapeBattleFuBenData.FuBenSeqID == 0)
						{
							escapeBattleFuBenData.FuBenSeqID = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						}
						int posX = 0;
						int posY = 0;
						int num2 = 0;
						EscapeBattleScene escapeBattleScene;
						if (!escapeBattleFuBenData.SideDict.TryGetValue((long)client.ClientData.ZhanDuiID, out num2))
						{
							LogManager.WriteLog(2, string.Format("未找到活动阵营数据KuaFuCopyData,roleid={0},gameid={1},mapcode={2]", client.ClientData.RoleID, num, escapeBattleMatchConfig.MapCode), null, true);
							result = false;
						}
						else if (this.SceneDict.TryGetValue(escapeBattleFuBenData.FuBenSeqID, out escapeBattleScene) && escapeBattleScene.m_eStatus >= 3)
						{
							result = false;
						}
						else if (!this.GetBirthPoint(escapeBattleMatchConfig.MapCode, num2, out posX, out posY))
						{
							LogManager.WriteLog(2, string.Format("roleid={0},mapcode={1},side={2} 未找到出生点", client.ClientData.RoleID, escapeBattleMatchConfig.MapCode, num2), null, true);
							result = false;
						}
						else
						{
							Global.GetClientKuaFuServerLoginData(client).FuBenSeqId = escapeBattleFuBenData.FuBenSeqID;
							client.ClientData.MapCode = escapeBattleMatchConfig.MapCode;
							client.ClientData.PosX = posX;
							client.ClientData.PosY = posY;
							client.ClientData.FuBenSeqID = escapeBattleFuBenData.FuBenSeqID;
							client.ClientData.BattleWhichSide = num2;
							result = true;
						}
					}
				}
			}
			return result;
		}

		public void TimerProc(object sender, EventArgs e)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			DateTime dateTime2 = TimeUtil.NowDateTime().Add(this.RuntimeData.DiffTimeSpan);
			long num = TimeUtil.NOW();
			List<GameClient> mapGameClients = GameManager.ClientMgr.GetMapGameClients(this.RuntimeData.ReadyMapCode);
			if (null != mapGameClients)
			{
				lock (this.RuntimeData.Mutex)
				{
					int num2 = (int)dateTime2.TimeOfDay.TotalSeconds / EscapeBattleConsts.BattleSignSecs;
					if (num2 != this.RuntimeData.LastStartSecs)
					{
						this.RuntimeData.LastStartSecs = num2;
						int num3 = (int)dateTime2.TimeOfDay.TotalSeconds % EscapeBattleConsts.BattleSignSecs;
						GameSceneStateTimeData gameSceneStateTimeData = new GameSceneStateTimeData();
						gameSceneStateTimeData.GameType = 37;
						gameSceneStateTimeData.State = 1;
						gameSceneStateTimeData.EndTicks = dateTime.AddSeconds((double)EscapeBattleConsts.BattleSignSecs).Ticks / 10000L;
						foreach (GameClient gameClient in mapGameClients)
						{
							gameClient.sendCmd<GameSceneStateTimeData>(827, gameSceneStateTimeData, false);
						}
					}
					List<int> list = new List<int>();
					foreach (KeyValuePair<int, EscapeBattlePiPeiState> keyValuePair in this.RuntimeData.ConfirmBattleDict)
					{
						bool flag2 = false;
						int num4 = 0;
						int key = keyValuePair.Key;
						EscapeBattlePiPeiState value = keyValuePair.Value;
						if (value.State <= 2)
						{
							List<GameClient> list2 = new List<GameClient>();
							using (List<EscapeBattleJoinRoleInfo>.Enumerator enumerator3 = value.RoleList.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									EscapeBattleJoinRoleInfo role = enumerator3.Current;
									GameClient gameClient2 = mapGameClients.Find((GameClient x) => x.ClientData.RoleID == role.RoleID);
									if (gameClient2 != null && gameClient2.ClientData.SceneMapCode == this.RuntimeData.ReadyMapCode)
									{
										list2.Add(gameClient2);
										num4++;
										role.Join = true;
										if (role.IsLeader)
										{
											flag2 = true;
										}
									}
									else if (!role.IsLeader)
									{
										role.Join = false;
									}
								}
							}
							num4 = (flag2 ? num4 : 0);
							if (num4 != value.ReadyNum || value.State == 2 != flag2)
							{
								ReturnValue<int> returnValue = TcpCall.EscapeBattle_K.ZhanDuiJoin(key, value.EscapeJiFen, num4);
								if (returnValue.IsReturn && returnValue.Value >= 0)
								{
									value.ReadyNum = num4;
								}
								if (value.State == 2)
								{
									foreach (GameClient gameClient2 in list2)
									{
										gameClient2.sendCmd<List<EscapeBattleJoinRoleInfo>>(2120, value.RoleList, true);
									}
								}
							}
							if (value.State != 2 && list2.Count > 0)
							{
								foreach (GameClient gameClient2 in list2)
								{
									Global.GotoLastMap(gameClient2, 1);
								}
							}
							TianTi5v5ZhanDuiData zhanDuiData = TianTi5v5Manager.getInstance().GetZhanDuiData(keyValuePair.Key, GameManager.ServerId);
							if (null == zhanDuiData)
							{
								list.Add(keyValuePair.Key);
							}
						}
					}
					foreach (int key2 in list)
					{
						this.RuntimeData.ConfirmBattleDict.Remove(key2);
					}
				}
			}
			while (this.RuntimeData.SyncDataByTime.RunByInterval(num))
			{
				EscapeBattleSyncData syncDataRequest = this.RuntimeData.SyncDataRequest;
				ReturnValue<EscapeBattleSyncData> returnValue2 = TcpCall.EscapeBattle_K.SyncZhengBaData(syncDataRequest);
				if (!returnValue2.IsReturn)
				{
					break;
				}
				EscapeBattleSyncData value2 = returnValue2.Value;
				if (value2 == null)
				{
					break;
				}
				long num5 = TimeUtil.NOW();
				if (num5 - num < 1000L)
				{
					long num6 = num5 - num;
					this.RuntimeData.SyncData.CenterTime = value2.CenterTime.AddMilliseconds((double)num6);
					this.RuntimeData.DiffTimeSpan = this.RuntimeData.SyncData.CenterTime - TimeUtil.NowDateTime();
				}
				lock (this.RuntimeData.Mutex)
				{
					if (syncDataRequest != this.RuntimeData.SyncDataRequest)
					{
						break;
					}
					this.RuntimeData.SyncData.Season = value2.Season;
					this.RuntimeData.SyncData.State = value2.State;
					if (value2.RankModTime != syncDataRequest.RankModTime)
					{
						this.RuntimeData.SyncDataRequest.RankModTime = value2.RankModTime;
						this.RuntimeData.SyncData.RankList = value2.RankList;
						this.RuntimeData.SyncData.SeasonRankList = value2.SeasonRankList;
					}
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < this.RuntimeData.PKResultQueue.Count; i++)
				{
					KeyValuePair<int, List<int>> keyValuePair2 = this.RuntimeData.PKResultQueue.Peek();
					if (TcpCall.EscapeBattle_K.GameResult(keyValuePair2.Key, keyValuePair2.Value).Type != 7)
					{
						break;
					}
					this.RuntimeData.PKResultQueue.Dequeue();
				}
				for (int i = 0; i < this.RuntimeData.GameStateQueue.Count; i++)
				{
					KeyValuePair<int, int> keyValuePair3 = this.RuntimeData.GameStateQueue.Peek();
					if (TcpCall.EscapeBattle_K.GameState(keyValuePair3.Key, keyValuePair3.Value).Type != 7)
					{
						break;
					}
					this.RuntimeData.GameStateQueue.Dequeue();
				}
			}
		}

		public void InitEscapeBattleZhanDuiData(TianTi5v5ZhanDuiData zhanduiData)
		{
			if (zhanduiData != null && 0 != this.RuntimeData.SyncData.Season)
			{
				DateTime realDate = TimeUtil.GetRealDate(this.RuntimeData.SyncData.Season);
				if (zhanduiData.EscapeLastFightTime < realDate)
				{
					zhanduiData.EscapeJiFen = 0;
				}
			}
		}

		public TianTi5v5ZhanDuiData GetZhanDuiData(int zhanDuiID, int serverID)
		{
			TianTi5v5ZhanDuiData zhanDuiData = TianTi5v5Manager.getInstance().GetZhanDuiData(zhanDuiID, serverID);
			TianTi5v5ZhanDuiData result;
			if (null == zhanDuiData)
			{
				LogManager.WriteLog(2, string.Format("获取战队数据失败 ZhanDuiID={0} ServerID={1}", zhanDuiID, serverID), null, true);
				result = null;
			}
			else
			{
				this.InitEscapeBattleZhanDuiData(zhanDuiData);
				result = zhanDuiData;
			}
			return result;
		}

		public EscapeBDuanAwardsConfig GetEscapeBattleAwardConfigByJiFen(int jifen)
		{
			EscapeBDuanAwardsConfig escapeBDuanAwardsConfig = null;
			lock (this.RuntimeData.Mutex)
			{
				foreach (EscapeBDuanAwardsConfig escapeBDuanAwardsConfig2 in this.RuntimeData.DuanAwardsConfig)
				{
					if ((escapeBDuanAwardsConfig2.RankValue < 0 || jifen >= escapeBDuanAwardsConfig2.RankValue) && (escapeBDuanAwardsConfig == null || escapeBDuanAwardsConfig2.ID > escapeBDuanAwardsConfig.ID))
					{
						escapeBDuanAwardsConfig = escapeBDuanAwardsConfig2;
					}
				}
			}
			return escapeBDuanAwardsConfig;
		}

		protected void SetSeasonRankDataFlags(GameClient client, bool bFight, bool bHasGet)
		{
			long num = Global.GetRoleParamsInt64FromDB(client, "10256");
			uint[] array = LongUnion.DecodeDec(num, EscapeBattleConsts.SeasonAwardDataBitInfo);
			if (null != array)
			{
				uint num2 = 0U;
				uint num3 = 0U;
				uint num4 = 0U;
				uint num5 = 0U;
				int num6 = this.RuntimeData.SyncData.Season - EscapeBattleConsts.DaysPerSeason;
				if ((ulong)array[0] == (ulong)((long)this.RuntimeData.SyncData.Season))
				{
					num2 = array[1];
					num3 = array[2];
					num4 = array[3];
				}
				else if ((ulong)array[0] == (ulong)((long)num6))
				{
					num3 = array[1];
				}
				if (bFight || num2 > 0U)
				{
					num5 = (uint)(client.ClientData.ZhanDuiID % 10000);
					num2 = 1U;
				}
				if (bHasGet || num4 > 0U)
				{
					num4 = 1U;
				}
				num = LongUnion.CreateDec(EscapeBattleConsts.SeasonAwardDataBitInfo, new uint[]
				{
					(uint)this.RuntimeData.SyncData.Season,
					num2,
					num3,
					num4,
					num5
				});
				Global.SaveRoleParamsInt64ValueToDB(client, "10256", num, true);
			}
		}

		public int CanGetMonthRankAwards(GameClient client, out EscapeBattleAwardsConfig duanWeiRankAward)
		{
			duanWeiRankAward = null;
			int offsetDayNow = TimeUtil.GetOffsetDayNow();
			int result;
			if (offsetDayNow - this.RuntimeData.SyncData.Season > EscapeBattleConsts.DaysPerSeason)
			{
				result = 0;
			}
			else if (this.RuntimeData.SyncData.SeasonRankList == null)
			{
				result = 0;
			}
			else
			{
				int zhanDuiId = client.ClientData.ZhanDuiID;
				int num = this.RuntimeData.SyncData.Season - EscapeBattleConsts.DaysPerSeason;
				DateTime realDate = TimeUtil.GetRealDate(num);
				TianTi5v5ZhanDuiData zhanDuiData = this.GetZhanDuiData(zhanDuiId, client.ServerId);
				lock (this.RuntimeData.Mutex)
				{
					if (zhanDuiData != null && zhanDuiData.EscapeLastFightTime > realDate)
					{
						int rank = this.RuntimeData.SyncData.SeasonRankList.FindIndex((KFEscapeRankInfo x) => x.Key == zhanDuiId) + 1;
						if (rank <= 0)
						{
							return 0;
						}
						duanWeiRankAward = this.RuntimeData.AwardsConfig.Find((EscapeBattleAwardsConfig x) => x.MinRank <= rank && x.MaxRank >= rank);
						if (null != duanWeiRankAward)
						{
							uint num2 = (uint)(client.ClientData.ZhanDuiID % 10000);
							long roleParamsInt64FromDB = Global.GetRoleParamsInt64FromDB(client, "10256");
							uint[] array = LongUnion.DecodeDec(roleParamsInt64FromDB, EscapeBattleConsts.SeasonAwardDataBitInfo);
							if (array == null || array[4] != num2)
							{
								return 0;
							}
							if ((ulong)array[0] == (ulong)((long)num))
							{
								if (array[1] > 0U && array[3] == 0U)
								{
									return rank;
								}
							}
						}
					}
				}
				result = 0;
			}
			return result;
		}

		public List<int> GetEscapeBattleRoleAnalysisData(GameClient client)
		{
			List<int> result;
			if (0 == this.RuntimeData.SyncData.Season)
			{
				result = null;
			}
			else
			{
				List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "156");
				this.FilterEscapeBattleAnalysisData(roleParamsIntListFromDB);
				result = roleParamsIntListFromDB;
			}
			return result;
		}

		private void FilterEscapeBattleAnalysisData(List<int> countList)
		{
			if (countList.Count != 4)
			{
				for (int i = countList.Count; i < 4; i++)
				{
					countList.Add(0);
				}
			}
			int offsetDay = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
			if (this.RuntimeData.SyncData.Season != countList[0])
			{
				countList[0] = this.RuntimeData.SyncData.Season;
			}
			if (offsetDay != countList[1])
			{
				countList[1] = offsetDay;
				countList[3] = 0;
			}
		}

		private void SaveEscapeBattleRoleAnalysisData(GameClient client, List<int> countList)
		{
			Global.SaveRoleParamsIntListToDB(client, countList, "156", true);
		}

		public bool CanUseMaigc(GameClient client)
		{
			EscapeBattleScene escapeBattleScene = client.SceneObject as EscapeBattleScene;
			return null == escapeBattleScene || escapeBattleScene.m_eStatus >= 3;
		}

		public void FillGuanZhanData(GameClient client, GuanZhanData gzData)
		{
			lock (this.RuntimeData.Mutex)
			{
				EscapeBattleScene escapeBattleScene = client.SceneObject as EscapeBattleScene;
				if (null != escapeBattleScene)
				{
					List<EscapeBattleRoleInfo> list;
					if (escapeBattleScene.ClientContextDataDict.TryGetValue(client.ClientData.ZhanDuiID, out list))
					{
						foreach (EscapeBattleRoleInfo escapeBattleRoleInfo in list)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(escapeBattleRoleInfo.RoleID);
							if (gameClient != null && gameClient.ClientData.HideGM <= 0)
							{
								SceneUIClasses mapSceneType = Global.GetMapSceneType(gameClient.ClientData.MapCode);
								if (59 == mapSceneType)
								{
									List<GuanZhanRoleMiniData> list2 = null;
									if (!gzData.RoleMiniDataDict.TryGetValue(client.ClientData.BattleWhichSide, out list2))
									{
										list2 = new List<GuanZhanRoleMiniData>();
										gzData.RoleMiniDataDict[client.ClientData.BattleWhichSide] = list2;
									}
									GuanZhanRoleMiniData guanZhanRoleMiniData = new GuanZhanRoleMiniData();
									guanZhanRoleMiniData.RoleID = gameClient.ClientData.RoleID;
									guanZhanRoleMiniData.Name = Global.FormatRoleNameWithZoneId2(gameClient);
									guanZhanRoleMiniData.Level = gameClient.ClientData.Level;
									guanZhanRoleMiniData.ChangeLevel = gameClient.ClientData.ChangeLifeCount;
									guanZhanRoleMiniData.Occupation = gameClient.ClientData.Occupation;
									guanZhanRoleMiniData.RoleSex = gameClient.ClientData.RoleSex;
									guanZhanRoleMiniData.BHZhiWu = gameClient.ClientData.BHZhiWu;
									guanZhanRoleMiniData.Param1 = escapeBattleRoleInfo.KillRoleNum;
									guanZhanRoleMiniData.Param2 = ((client.ClientData.CurrentLifeV > 0 || escapeBattleRoleInfo.ReliveCount > 0) ? 1 : 0);
									list2.Add(guanZhanRoleMiniData);
								}
							}
						}
						foreach (List<GuanZhanRoleMiniData> list3 in gzData.RoleMiniDataDict.Values)
						{
							list3.Sort(delegate(GuanZhanRoleMiniData left, GuanZhanRoleMiniData right)
							{
								int result;
								if (left.Param2 > right.Param2)
								{
									result = -1;
								}
								else if (left.Param2 < right.Param2)
								{
									result = 1;
								}
								else if (left.Param1 > right.Param1)
								{
									result = -1;
								}
								else if (left.Param1 < right.Param1)
								{
									result = 1;
								}
								else
								{
									result = 0;
								}
								return result;
							});
						}
					}
				}
			}
		}

		public bool CheckOpenState(DateTime now)
		{
			bool result;
			lock (this.RuntimeData.Mutex)
			{
				if (now < this.RuntimeData.EscapeStartTime)
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

		public void CancleJoinState(GameClient client)
		{
			if (client.ClientData.ZhanDuiZhiWu == 1)
			{
				int zhanDuiID = client.ClientData.ZhanDuiID;
				lock (this.RuntimeData.Mutex)
				{
					EscapeBattlePiPeiState escapeBattlePiPeiState;
					if (zhanDuiID > 0 && this.RuntimeData.ConfirmBattleDict.TryGetValue(zhanDuiID, out escapeBattlePiPeiState))
					{
						if (escapeBattlePiPeiState.State == 2)
						{
							escapeBattlePiPeiState.State = 0;
						}
					}
				}
			}
		}

		public bool OnPreZhanDuiChangeMember(PreZhanDuiChangeMemberEventObject e)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			long ticks = dateTime.Ticks;
			TimeSpan t = new TimeSpan((int)dateTime.DayOfWeek, dateTime.Hour, dateTime.Minute, dateTime.Second);
			lock (this.RuntimeData.Mutex)
			{
				List<TimeSpan> timePoints = this.RuntimeData.Config.MatchConfigList[0].TimePoints;
				for (int i = 0; i < timePoints.Count - 1; i += 2)
				{
					if (t >= timePoints[i] && t < timePoints[i])
					{
						EscapeBattlePiPeiState escapeBattlePiPeiState;
						if (!this.RuntimeData.ConfirmBattleDict.TryGetValue(e.ZhanDuiID, out escapeBattlePiPeiState) || escapeBattlePiPeiState.State >= 2)
						{
							return false;
						}
						e.Result = false;
					}
				}
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

		public const SceneUIClasses ManagerType = 59;

		public const GameTypes GameType = 37;

		public ConcurrentDictionary<int, EscapeBattleScene> SceneDict = new ConcurrentDictionary<int, EscapeBattleScene>();

		private static long NextHeartBeatTicks = 0L;

		private static EscapeBattleManager instance = new EscapeBattleManager();

		private EventSourceEx<KFCallMsg>.HandlerData NotifyEnterHandler = null;

		private EventSourceEx<KFCallMsg>.HandlerData NotifyGameStateHandler = null;

		public EscapeBattleData RuntimeData = new EscapeBattleData();
	}
}
