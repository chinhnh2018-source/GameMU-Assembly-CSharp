using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GameServer.Core.Executor;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	internal class GlodCopySceneManager
	{
		public static void AddGlodCopySceneList(int nID, CopyMap mapInfo)
		{
			bool flag = false;
			lock (GlodCopySceneManager.m_GlodCopySceneLists)
			{
				CopyMap copyMap = null;
				if (!GlodCopySceneManager.m_GlodCopySceneLists.TryGetValue(nID, out copyMap))
				{
					GlodCopySceneManager.m_GlodCopySceneLists.Add(nID, mapInfo);
					flag = true;
				}
				else if (copyMap == null)
				{
					GlodCopySceneManager.m_GlodCopySceneLists[nID] = mapInfo;
					flag = true;
				}
				lock (GlodCopySceneManager.m_GlodCopySceneInfo)
				{
					if (flag)
					{
						GoldCopyScene goldCopyScene = null;
						if (!GlodCopySceneManager.m_GlodCopySceneInfo.TryGetValue(nID, out goldCopyScene))
						{
							goldCopyScene = new GoldCopyScene();
							goldCopyScene.InitInfo(mapInfo.MapCode, mapInfo.CopyMapID, nID);
							goldCopyScene.m_StartTimer = TimeUtil.NOW();
							GlodCopySceneManager.m_GlodCopySceneInfo.Add(nID, goldCopyScene);
						}
					}
				}
			}
		}

		public static void RemoveGlodCopySceneList(int nID)
		{
			lock (GlodCopySceneManager.m_GlodCopySceneLists)
			{
				GlodCopySceneManager.m_GlodCopySceneLists.Remove(nID);
			}
			lock (GlodCopySceneManager.m_GlodCopySceneInfo)
			{
				GlodCopySceneManager.m_GlodCopySceneInfo.Remove(nID);
			}
		}

		public static void HeartBeatGlodCopyScene()
		{
			long num = TimeUtil.NOW();
			if (num - GlodCopySceneManager.LastHeartBeatTicks >= 1000L)
			{
				GlodCopySceneManager.LastHeartBeatTicks = ((GlodCopySceneManager.LastHeartBeatTicks < 86400000L) ? num : (GlodCopySceneManager.LastHeartBeatTicks + 1000L));
				lock (GlodCopySceneManager.m_GlodCopySceneLists)
				{
					foreach (CopyMap copyMap in GlodCopySceneManager.m_GlodCopySceneLists.Values)
					{
						List<GameClient> clientsList = copyMap.GetClientsList();
						GoldCopyScene goldCopyScene = null;
						lock (GlodCopySceneManager.m_GlodCopySceneInfo)
						{
							if (!GlodCopySceneManager.m_GlodCopySceneInfo.TryGetValue(copyMap.FuBenSeqID, out goldCopyScene))
							{
								continue;
							}
						}
						if (goldCopyScene != null)
						{
							lock (goldCopyScene)
							{
								if (goldCopyScene.m_TimeNotifyFlag == 0)
								{
									goldCopyScene.m_TimeNotifyFlag = 1;
									if (clientsList.Count<GameClient>() != 0 && clientsList[0] != null)
									{
										GlodCopySceneManager.SendMsgToClientForGlodCopyScenePrepare(clientsList[0], goldCopyScene);
									}
								}
								if (num >= goldCopyScene.m_StartTimer + (long)GlodCopySceneManager.m_PrepareTime)
								{
									int createMonsterWave = goldCopyScene.m_CreateMonsterWave;
									int num2 = Data.Goldcopyscenedata.GoldCopySceneMonsterData.Count<KeyValuePair<int, GoldCopySceneMonster>>();
									if (createMonsterWave <= num2)
									{
										if (createMonsterWave == 0 && goldCopyScene.m_CreateMonsterFirstWaveFlag == 0)
										{
											goldCopyScene.m_CreateMonsterFirstWaveFlag = 1;
											goldCopyScene.m_CreateMonsterWave = 1;
										}
										GoldCopySceneMonster goldCopySceneMonster = null;
										if (Data.Goldcopyscenedata.GoldCopySceneMonsterData.TryGetValue(createMonsterWave, out goldCopySceneMonster))
										{
											if (goldCopySceneMonster != null)
											{
												if (num - goldCopyScene.m_CreateMonsterTick2 > (long)(goldCopySceneMonster.m_Delay2 * 1000))
												{
													if (goldCopyScene.m_CreateMonsterWaveNotify == 0)
													{
														goldCopyScene.m_CreateMonsterWaveNotify = 1;
														if (clientsList.Count<GameClient>() != 0 && clientsList[0] != null)
														{
															GlodCopySceneManager.SendMsgToClientForGlodCopySceneMonsterWave(clientsList[0], goldCopyScene.m_CreateMonsterWave);
														}
													}
													if (num - goldCopyScene.m_CreateMonsterTick1 > (long)(goldCopySceneMonster.m_Delay1 * 1000))
													{
														if (goldCopyScene.m_LoginEnterFlag == 1)
														{
															if (clientsList.Count<GameClient>() != 0 && clientsList[0] != null && num - goldCopyScene.m_LoginEnterTimer > (long)GlodCopySceneManager.m_DelayTime)
															{
																goldCopyScene.m_LoginEnterFlag = 0;
																GlodCopySceneManager.SendMsgToClientForGlodCopySceneMonsterWave(clientsList[0], goldCopyScene.m_CreateMonsterWave);
															}
														}
														goldCopyScene.m_CreateMonsterTick1 = ((goldCopyScene.m_CreateMonsterTick1 < 86400000L) ? num : (goldCopyScene.m_CreateMonsterTick1 + (long)(goldCopySceneMonster.m_Delay1 * 1000)));
														if (clientsList.Count<GameClient>() != 0 && clientsList[0] != null)
														{
															GlodCopySceneManager.CreateMonsterForGoldCopyScene(clientsList[0], goldCopyScene, goldCopyScene.m_CreateMonsterWave);
														}
														else
														{
															GlodCopySceneManager.CreateMonsterForGoldCopyScene(null, goldCopyScene, goldCopyScene.m_CreateMonsterWave);
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public static void CreateMonsterForGoldCopyScene(GameClient client, GoldCopyScene goldcopyscene, int nWave)
		{
			GoldCopySceneMonster goldCopySceneMonster = Data.Goldcopyscenedata.GoldCopySceneMonsterData[nWave];
			long createMonsterTick = TimeUtil.NOW();
			int randomNumber = Global.GetRandomNumber(0, 10);
			int[] array = Data.Goldcopyscenedata.m_MonsterPatorlPathList[0];
			Point point = new Point((double)array[0], (double)array[1]);
			GameManager.MonsterZoneMgr.AddDynamicMonsters(goldcopyscene.m_MapCodeID, goldCopySceneMonster.m_MonsterID[randomNumber], goldcopyscene.m_CopyMapID, 1, (int)point.X, (int)point.Y, 1, 0, 0, null, null);
			goldcopyscene.m_CreateMonsterCount++;
			if (goldcopyscene.m_CreateMonsterCount == goldCopySceneMonster.m_Num)
			{
				goldcopyscene.m_CreateMonsterTick2 = createMonsterTick;
				goldcopyscene.m_CreateMonsterWave = nWave + 1;
				goldcopyscene.m_CreateMonsterCount = 0;
				goldcopyscene.m_CreateMonsterWaveNotify = 0;
				if (goldcopyscene.m_CreateMonsterWave > Data.Goldcopyscenedata.GoldCopySceneMonsterData.Count<KeyValuePair<int, GoldCopySceneMonster>>() && client != null)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(376, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				}
			}
		}

		public static void MonsterMoveStepGoldCopySceneCopyMap(Monster monster)
		{
			long num = TimeUtil.NOW();
			if (num - monster.MoveTime >= 500L)
			{
				int step = monster.Step;
				int num2 = monster.PatrolPath.Count<int[]>() - 1;
				int num3 = step + 1;
				if (num3 >= num2)
				{
					GameManager.MonsterMgr.AddDelayDeadMonster(monster);
				}
				else
				{
					int key = 5100;
					MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[key];
					int num4 = monster.PatrolPath[num3][0];
					int num5 = monster.PatrolPath[num3][1];
					int num6 = num4 / mapGrid.MapGridWidth;
					int num7 = num5 / mapGrid.MapGridHeight;
					Point start = new Point((double)num6, (double)num7);
					Point currentGrid = monster.CurrentGrid;
					int currentX = (int)currentGrid.X;
					int currentY = (int)currentGrid.Y;
					double directionByAspect = Global.GetDirectionByAspect(num6, num7, currentX, currentY);
					ChuanQiUtils.WalkTo(monster, (Dircetions)directionByAspect);
					monster.MoveTime = num;
					if (Global.GetTwoPointDistance(start, currentGrid) < 2.0)
					{
						monster.Step = step + 1;
					}
				}
			}
		}

		public static void SendMsgToClientForGlodCopyScenePrepare(GameClient client, GoldCopyScene goldcopyscene)
		{
			if (client != null)
			{
				int num = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
				if (num > 0)
				{
					long num2 = TimeUtil.NOW();
					int num3 = (int)(((long)GlodCopySceneManager.m_PrepareTime - (num2 - goldcopyscene.m_StartTimer)) / 1000L);
					string strCmd = string.Format("{0}", num3);
					GameManager.ClientMgr.SendToClient(client, strCmd, 627);
				}
			}
		}

		public static void SendMsgToClientForGlodCopySceneMonsterWave(GameClient client, int nWave)
		{
			if (client != null)
			{
				int num = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
				if (num > 0)
				{
					string data = string.Format("{0}:{1}", nWave, Data.Goldcopyscenedata.GoldCopySceneMonsterData.Count<KeyValuePair<int, GoldCopySceneMonster>>());
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 628);
					Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
				}
			}
		}

		public static bool EnterGoldCopySceneWhenLogin(GameClient client, bool bContinue = true)
		{
			bool result;
			if (client != null)
			{
				CopyMap copyMap = null;
				GoldCopyScene goldCopyScene = null;
				lock (GlodCopySceneManager.m_GlodCopySceneLists)
				{
					if (!GlodCopySceneManager.m_GlodCopySceneLists.TryGetValue(client.ClientData.FuBenSeqID, out copyMap) || copyMap == null)
					{
						return false;
					}
				}
				lock (GlodCopySceneManager.m_GlodCopySceneInfo)
				{
					if (!GlodCopySceneManager.m_GlodCopySceneInfo.TryGetValue(client.ClientData.FuBenSeqID, out goldCopyScene) || goldCopyScene == null)
					{
						return false;
					}
				}
				long loginEnterTimer = TimeUtil.NOW();
				goldCopyScene.m_LoginEnterTimer = loginEnterTimer;
				goldCopyScene.m_LoginEnterFlag = 1;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static int m_PrepareTime = 10000;

		public static int m_DelayTime = 2000;

		public static Dictionary<int, CopyMap> m_GlodCopySceneLists = new Dictionary<int, CopyMap>();

		public static Dictionary<int, GoldCopyScene> m_GlodCopySceneInfo = new Dictionary<int, GoldCopyScene>();

		private static long LastHeartBeatTicks = 0L;
	}
}
