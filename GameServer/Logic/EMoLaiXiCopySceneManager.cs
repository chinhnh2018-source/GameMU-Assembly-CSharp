using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	internal class EMoLaiXiCopySceneManager
	{
		public static void LoadEMoLaiXiCopySceneInfo()
		{
			try
			{
				int num = 0;
				EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.FaildEscapeMonsterNum = (int)GameManager.systemParamsList.GetParamValueIntByName("BaoWeiZhan", -1);
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/EMoLaiXiLuXian.xml", new object[0]));
				foreach (XElement xml in gameResXml.Elements("Path"))
				{
					int num2 = (int)Global.GetSafeAttributeLong(xml, "ID");
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "Path");
					List<int[]> list = new List<int[]>();
					if (string.IsNullOrEmpty(safeAttributeStr))
					{
						LogManager.WriteLog(1, string.Format("金币副本怪路径为空", new object[0]), null, true);
					}
					else
					{
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (array.Length <= 0)
						{
							LogManager.WriteLog(1, string.Format("金币副本怪路径为空", new object[0]), null, true);
						}
						else
						{
							for (int i = 0; i < array.Length; i++)
							{
								string[] array2 = array[i].Split(new char[]
								{
									','
								});
								if (array2.Length != 2)
								{
									LogManager.WriteLog(1, string.Format("解析{0}文件中的奖励项时失败,坐标有误", "Config/EMoLaiXiLuXian.xml"), null, true);
								}
								else
								{
									int[] item = Global.StringArray2IntArray(array2);
									list.Add(item);
								}
							}
						}
					}
					EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.m_MonsterPatorlPathLists.Add(num2, list);
				}
				gameResXml = Global.GetGameResXml(string.Format("Config/EMoLaiXi.xml", new object[0]));
				IEnumerable<XElement> enumerable = gameResXml.Elements("FuBen");
				foreach (XElement xelement in enumerable)
				{
					if (null != xelement)
					{
						EMoLaiXiCopySenceMonster emoLaiXiCopySenceMonster = new EMoLaiXiCopySenceMonster();
						emoLaiXiCopySenceMonster.m_MonsterID = new List<int>();
						int num2 = (int)Global.GetSafeAttributeLong(xelement, "ID");
						emoLaiXiCopySenceMonster.m_ID = num2;
						emoLaiXiCopySenceMonster.m_Wave = (int)Global.GetSafeAttributeLong(xelement, "WaveID");
						emoLaiXiCopySenceMonster.m_Delay1 = (int)Global.GetSafeAttributeLong(xelement, "Delay1");
						emoLaiXiCopySenceMonster.m_Delay2 = (int)Global.GetSafeAttributeLong(xelement, "Delay2");
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement, "PathID");
						emoLaiXiCopySenceMonster.PathIDArray = Global.String2IntArray(safeAttributeStr2, ',');
						string safeAttributeStr3 = Global.GetSafeAttributeStr(xelement, "MonsterList");
						if (string.IsNullOrEmpty(safeAttributeStr3))
						{
							LogManager.WriteLog(1, string.Format("金币副本怪ID为空", new object[0]), null, true);
						}
						else
						{
							string[] array = safeAttributeStr3.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("金币副本怪ID为空", new object[0]), null, true);
							}
							else
							{
								for (int i = 0; i < array.Length; i++)
								{
									int[] array3 = Global.String2IntArray(array[i], ',');
									if (array3 != null && array3.Length == 2 && array3[0] > 0 && array3[1] > 0)
									{
										for (int j = 0; j < array3[1]; j++)
										{
											emoLaiXiCopySenceMonster.m_MonsterID.Add(array3[0]);
											emoLaiXiCopySenceMonster.m_Num++;
										}
									}
								}
							}
						}
						EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.EMoLaiXiCopySenceMonsterData.Add(emoLaiXiCopySenceMonster);
						num = Global.GMax(num, emoLaiXiCopySenceMonster.m_Wave);
					}
				}
				EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.TotalWave = num;
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/JinBiFuBen.xml", new object[0])));
			}
		}

		public static void AddEMoLaiXiCopySceneList(int nID, CopyMap mapInfo)
		{
			bool flag = false;
			lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists)
			{
				CopyMap copyMap = null;
				if (!EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists.TryGetValue(nID, out copyMap))
				{
					EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists.Add(nID, mapInfo);
					flag = true;
				}
				else if (copyMap == null)
				{
					EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists[nID] = mapInfo;
					flag = true;
				}
				lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo)
				{
					if (flag)
					{
						EMoLaiXiCopySence emoLaiXiCopySence = null;
						if (!EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo.TryGetValue(nID, out emoLaiXiCopySence))
						{
							emoLaiXiCopySence = new EMoLaiXiCopySence();
							emoLaiXiCopySence.InitInfo(mapInfo.MapCode, mapInfo.CopyMapID, nID);
							emoLaiXiCopySence.m_StartTimer = TimeUtil.NOW();
							EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo.Add(nID, emoLaiXiCopySence);
						}
					}
				}
			}
		}

		public static void RemoveEMoLaiXiCopySceneList(int nID, int copyMapID)
		{
			lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists)
			{
				EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists.Remove(nID);
			}
			lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo)
			{
				EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo.Remove(nID);
			}
			lock (EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict)
			{
				EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict.Remove(copyMapID);
			}
		}

		public static void HeartBeatEMoLaiXiCopyScene()
		{
			long num = TimeUtil.NOW();
			if (num - EMoLaiXiCopySceneManager.LastHeartBeatTicks >= 1000L)
			{
				EMoLaiXiCopySceneManager.LastHeartBeatTicks = ((EMoLaiXiCopySceneManager.LastHeartBeatTicks < 86400000L) ? num : (EMoLaiXiCopySceneManager.LastHeartBeatTicks + 1000L));
				lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists)
				{
					foreach (CopyMap copyMap in EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists.Values)
					{
						EMoLaiXiCopySence emoLaiXiCopySence = null;
						lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo)
						{
							if (!EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo.TryGetValue(copyMap.FuBenSeqID, out emoLaiXiCopySence))
							{
								continue;
							}
						}
						if (emoLaiXiCopySence != null)
						{
							List<GameClient> clientsList = copyMap.GetClientsList();
							lock (emoLaiXiCopySence)
							{
								if (emoLaiXiCopySence.m_TimeNotifyFlag == 0)
								{
									if (num <= emoLaiXiCopySence.m_StartTimer + (long)EMoLaiXiCopySceneManager.m_PrepareTime - 3000L)
									{
										continue;
									}
									emoLaiXiCopySence.m_TimeNotifyFlag = 1;
									string strcmd = string.Format("{0}:{1}${2}${3}", new object[]
									{
										2,
										3,
										1,
										""
									});
									GameManager.ClientMgr.BroadSpecialCopyMapMessage(419, strcmd, clientsList, true);
								}
								if (num >= emoLaiXiCopySence.m_StartTimer + (long)EMoLaiXiCopySceneManager.m_PrepareTime)
								{
									if (emoLaiXiCopySence.m_Delay2 > 0L)
									{
										EMoLaiXiCopySceneManager.OnSceneTimer(emoLaiXiCopySence, clientsList, copyMap, num);
									}
									else
									{
										EMoLaiXiCopySceneManager.InitNextWaveMonsterList(emoLaiXiCopySence);
									}
								}
							}
						}
					}
				}
			}
		}

		public static void OnSceneTimer(EMoLaiXiCopySence scene, List<GameClient> clientList, CopyMap copyMap, long nowTicks)
		{
			int createMonsterWave = scene.m_CreateMonsterWave;
			int totalWave = EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.TotalWave;
			bool flag = false;
			bool flag2 = false;
			int escapeCount = EMoLaiXiCopySceneManager.GetEscapeCount(scene.m_CopyMapID);
			if (escapeCount > 0)
			{
				scene.m_EscapedMonsterNum += escapeCount;
				flag = true;
			}
			if (scene.m_LoginEnterFlag == 1)
			{
				if (nowTicks - scene.m_LoginEnterTimer > (long)EMoLaiXiCopySceneManager.m_DelayTime)
				{
					scene.m_LoginEnterFlag = 0;
					flag = true;
				}
			}
			if (scene.m_EscapedMonsterNum >= EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.FaildEscapeMonsterNum)
			{
				if (!scene.m_bFinished)
				{
					GameManager.CopyMapMgr.CopyMapFaildForAll(clientList, copyMap);
					scene.m_bFinished = true;
				}
				GameManager.CopyMapMgr.KillAllMonster(copyMap);
				flag = true;
			}
			else if (scene.m_bAllMonsterCreated)
			{
				if (!scene.m_bFinished)
				{
					if (copyMap.KilledDynamicMonsterNum + scene.m_EscapedMonsterNum >= scene.m_TotalMonsterCountAllWave)
					{
						if (clientList != null && clientList.Count > 0)
						{
							flag = true;
							GameManager.CopyMapMgr.CopyMapPassAwardForAll(clientList[0], copyMap, true);
							scene.m_bFinished = true;
							if (copyMap.KilledDynamicMonsterNum > copyMap.TotalDynamicMonsterNum)
							{
								try
								{
									string text = string.Format("恶魔来袭已成功,但杀怪计数异常,结束时间{0},KilledDynamicMonsterNum:{1},m_EscapedMonsterNum:{2},m_TotalMonsterCountAllWave:{3}", new object[]
									{
										new DateTime(nowTicks * 10000L),
										copyMap.KilledDynamicMonsterNum,
										scene.m_EscapedMonsterNum,
										scene.m_TotalMonsterCountAllWave
									});
									LogManager.WriteLog(2, text, null, true);
								}
								catch
								{
								}
							}
						}
					}
				}
			}
			else if (nowTicks - scene.m_CreateMonsterTick2 > scene.m_Delay2 * 1000L)
			{
				if (scene.m_CreateMonsterWaveNotify == 0)
				{
					scene.m_CreateMonsterWaveNotify = 1;
					flag = true;
				}
				for (int i = 0; i < scene.m_CreateWaveMonsterList.Count; i++)
				{
					EMoLaiXiCopySenceMonster emoLaiXiCopySenceMonster = scene.m_CreateWaveMonsterList[i];
					if (emoLaiXiCopySenceMonster.m_CreateMonsterCount < emoLaiXiCopySenceMonster.m_Num)
					{
						if (nowTicks - emoLaiXiCopySenceMonster.m_CreateMonsterTick1 > (long)(emoLaiXiCopySenceMonster.m_Delay1 * 1000))
						{
							int createMonsterCount = emoLaiXiCopySenceMonster.m_CreateMonsterCount;
							int[] array = emoLaiXiCopySenceMonster.PatrolPath[0];
							GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_MapCodeID, emoLaiXiCopySenceMonster.m_MonsterID[createMonsterCount], scene.m_CopyMapID, 1, array[0], array[1], 0, 0, 11, emoLaiXiCopySenceMonster.PatrolPath, null);
							emoLaiXiCopySenceMonster.m_CreateMonsterCount++;
							scene.m_CreateMonsterCount++;
							emoLaiXiCopySenceMonster.m_CreateMonsterTick1 = nowTicks;
						}
					}
				}
				if (scene.m_CreateMonsterCount >= scene.m_TotalMonsterCount)
				{
					scene.m_CreateMonsterTick2 = nowTicks;
					scene.m_CreateMonsterWave++;
					scene.m_CreateMonsterCount = 0;
					scene.m_CreateMonsterWaveNotify = 0;
					scene.m_Delay2 = 0L;
					flag = true;
					copyMap.TotalDynamicMonsterNum = scene.m_TotalMonsterCountAllWave;
					if (scene.m_CreateMonsterWave >= EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.TotalWave)
					{
						scene.m_Delay2 = 2147483647L;
						scene.m_bAllMonsterCreated = true;
						flag2 = true;
					}
				}
			}
			if (flag)
			{
				EMoLaiXiCopySceneManager.SendMsgToClientForEMoLaiXiCopySceneMonsterWave(clientList, scene.m_EscapedMonsterNum, scene.m_CreateMonsterWave, EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.TotalWave, EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.FaildEscapeMonsterNum);
			}
			if (flag2 && null != clientList)
			{
				foreach (GameClient client in clientList)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(108, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				}
			}
		}

		public static void InitNextWaveMonsterList(EMoLaiXiCopySence scene)
		{
			if (scene.m_CreateMonsterWave >= 0 && scene.m_CreateMonsterWave < EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.TotalWave)
			{
				int num = 1;
				int num2 = 0;
				scene.m_CreateWaveMonsterList.Clear();
				foreach (EMoLaiXiCopySenceMonster emoLaiXiCopySenceMonster in EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.EMoLaiXiCopySenceMonsterData)
				{
					if (emoLaiXiCopySenceMonster.m_Wave == scene.m_CreateMonsterWave + 1)
					{
						EMoLaiXiCopySenceMonster emoLaiXiCopySenceMonster2 = emoLaiXiCopySenceMonster.CloneMini();
						scene.m_CreateWaveMonsterList.Add(emoLaiXiCopySenceMonster2);
						int randomNumber = Global.GetRandomNumber(0, emoLaiXiCopySenceMonster2.PathIDArray.Length);
						int key = emoLaiXiCopySenceMonster2.PathIDArray[randomNumber];
						emoLaiXiCopySenceMonster2.PatrolPath = EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.m_MonsterPatorlPathLists[key];
						num = Global.GMax(num, emoLaiXiCopySenceMonster2.m_Delay2);
						num2 += emoLaiXiCopySenceMonster2.m_Num;
					}
				}
				scene.m_Delay2 = (long)num;
				scene.m_TotalMonsterCount = num2;
				scene.m_TotalMonsterCountAllWave += num2;
			}
		}

		public static void IncreaceEscapeCount(int copyMapID)
		{
			lock (EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict)
			{
				int num;
				if (EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict.TryGetValue(copyMapID, out num))
				{
					EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict[copyMapID] = num + 1;
				}
				else
				{
					EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict[copyMapID] = 1;
				}
			}
		}

		public static int GetEscapeCount(int copyMapID)
		{
			int result;
			lock (EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict)
			{
				if (EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict.TryGetValue(copyMapID, out result))
				{
					EMoLaiXiCopySceneManager.m_EMoLaiXiEscapeMonsterNumDict[copyMapID] = 0;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		public static void MonsterMoveStepEMoLaiXiCopySenceCopyMap(Monster monster)
		{
			long num = TimeUtil.NOW();
			if (num - monster.MoveTime >= 500L)
			{
				int step = monster.Step;
				int num2 = monster.PatrolPath.Count<int[]>() - 1;
				int num3 = step + 1;
				if (num3 >= num2)
				{
					EMoLaiXiCopySceneManager.IncreaceEscapeCount(monster.CopyMapID);
					GameManager.MonsterMgr.DeadMonsterImmediately(monster);
				}
				else
				{
					int currentMapCode = monster.CurrentMapCode;
					MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[currentMapCode];
					int num4 = monster.PatrolPath[num3][0];
					int num5 = monster.PatrolPath[num3][1];
					int num6 = num4 / mapGrid.MapGridWidth;
					int num7 = num5 / mapGrid.MapGridHeight;
					Point start = new Point((double)num6, (double)num7);
					Point currentGrid = monster.CurrentGrid;
					int currentX = (int)currentGrid.X;
					int currentY = (int)currentGrid.Y;
					double directionByAspect = Global.GetDirectionByAspect(num6, num7, currentX, currentY);
					if (ChuanQiUtils.WalkTo(monster, (Dircetions)directionByAspect) || ChuanQiUtils.WalkTo(monster, (Dircetions)((directionByAspect + 7.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((directionByAspect + 9.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((directionByAspect + 6.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((directionByAspect + 10.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((directionByAspect + 5.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((directionByAspect + 11.0) % 8.0)))
					{
						monster.MoveTime = num;
					}
					if (Global.GetTwoPointDistance(start, currentGrid) < 2.0)
					{
						monster.Step = step + 1;
					}
				}
			}
		}

		public static void SendMsgToClientForEMoLaiXiCopySceneMonsterWave(List<GameClient> clientList, int escapeNum, int nWave, int totalWave, int faildEscapeNum)
		{
			if (clientList != null && clientList.Count > 0)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					escapeNum,
					nWave,
					EMoLaiXiCopySceneManager.EMoLaiXiCopySencedata.TotalWave,
					faildEscapeNum
				});
				GameManager.ClientMgr.BroadSpecialCopyMapMessage(679, strcmd, clientList, false);
			}
		}

		public static bool EnterEMoLaiXiCopySenceWhenLogin(GameClient client, bool bContinue = true)
		{
			bool result;
			if (client != null)
			{
				CopyMap copyMap = null;
				EMoLaiXiCopySence emoLaiXiCopySence = null;
				lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists)
				{
					if (!EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneLists.TryGetValue(client.ClientData.FuBenSeqID, out copyMap) || copyMap == null)
					{
						return false;
					}
				}
				lock (EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo)
				{
					if (!EMoLaiXiCopySceneManager.m_EMoLaiXiCopySceneInfo.TryGetValue(client.ClientData.FuBenSeqID, out emoLaiXiCopySence) || emoLaiXiCopySence == null)
					{
						return false;
					}
				}
				if (emoLaiXiCopySence.m_bFinished)
				{
					result = false;
				}
				else
				{
					long loginEnterTimer = TimeUtil.NOW();
					emoLaiXiCopySence.m_LoginEnterTimer = loginEnterTimer;
					emoLaiXiCopySence.m_LoginEnterFlag = 1;
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static EMoLaiXiCopySenceData EMoLaiXiCopySencedata = new EMoLaiXiCopySenceData();

		public static int m_PrepareTime = 9000;

		public static int m_DelayTime = 5000;

		public static Dictionary<int, CopyMap> m_EMoLaiXiCopySceneLists = new Dictionary<int, CopyMap>();

		public static Dictionary<int, EMoLaiXiCopySence> m_EMoLaiXiCopySceneInfo = new Dictionary<int, EMoLaiXiCopySence>();

		public static Dictionary<int, int> m_EMoLaiXiEscapeMonsterNumDict = new Dictionary<int, int>();

		public static int EMoLaiXiCopySceneMapCode = 4100;

		private static long LastHeartBeatTicks = 0L;
	}
}
