using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	internal class ExperienceCopySceneManager
	{
		public static void AddExperienceListCopyMap(int nID, CopyMap mapInfo)
		{
			bool flag = false;
			lock (ExperienceCopySceneManager.m_ExperienceListCopyMaps)
			{
				CopyMap copyMap = null;
				if (!ExperienceCopySceneManager.m_ExperienceListCopyMaps.TryGetValue(nID, out copyMap))
				{
					ExperienceCopySceneManager.m_ExperienceListCopyMaps.Add(nID, mapInfo);
					flag = true;
				}
				else if (copyMap == null)
				{
					ExperienceCopySceneManager.m_ExperienceListCopyMaps[nID] = mapInfo;
					flag = true;
				}
			}
			lock (ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo)
			{
				if (flag)
				{
					ExperienceCopyScene experienceCopyScene = null;
					if (!ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo.TryGetValue(nID, out experienceCopyScene))
					{
						experienceCopyScene = new ExperienceCopyScene();
						experienceCopyScene.InitInfo(mapInfo.MapCode, mapInfo.CopyMapID, nID);
						experienceCopyScene.m_StartTimer = TimeUtil.NOW();
						ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo.Add(nID, experienceCopyScene);
					}
				}
			}
		}

		public static void RemoveExperienceListCopyMap(int nID)
		{
			bool flag = false;
			lock (ExperienceCopySceneManager.m_ExperienceListCopyMaps)
			{
				CopyMap copyMap = null;
				if (ExperienceCopySceneManager.m_ExperienceListCopyMaps.TryGetValue(nID, out copyMap))
				{
					ExperienceCopySceneManager.m_ExperienceListCopyMaps.Remove(nID);
					flag = true;
				}
			}
			lock (ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo)
			{
				if (flag)
				{
					ExperienceCopyScene experienceCopyScene = null;
					if (ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo.TryGetValue(nID, out experienceCopyScene))
					{
						ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo.Remove(nID);
					}
				}
			}
		}

		public static void HeartBeatExperienceCopyMap()
		{
			long num = TimeUtil.NOW();
			if (num - ExperienceCopySceneManager.LastHeartBeatTicks >= 1000L)
			{
				ExperienceCopySceneManager.LastHeartBeatTicks = ((ExperienceCopySceneManager.LastHeartBeatTicks < 86400000L) ? num : (ExperienceCopySceneManager.LastHeartBeatTicks + 1000L));
				List<CopyMap> list = new List<CopyMap>();
				lock (ExperienceCopySceneManager.m_ExperienceListCopyMaps)
				{
					foreach (CopyMap copyMap in ExperienceCopySceneManager.m_ExperienceListCopyMaps.Values)
					{
						List<GameClient> clientsList = copyMap.GetClientsList();
						ExperienceCopyMapDataInfo experienceCopyMapDataInfo = null;
						experienceCopyMapDataInfo = Data.ExperienceCopyMapDataInfoList[copyMap.MapCode];
						if (experienceCopyMapDataInfo != null)
						{
							ExperienceCopyScene experienceCopyScene = null;
							lock (ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo)
							{
								if (!ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo.TryGetValue(copyMap.FuBenSeqID, out experienceCopyScene))
								{
									continue;
								}
							}
							if (experienceCopyScene != null)
							{
								int experienceCopyMapCreateMonsterWave = experienceCopyScene.m_ExperienceCopyMapCreateMonsterWave;
								int count = experienceCopyMapDataInfo.MonsterIDList.Count;
								if (experienceCopyMapCreateMonsterWave < count)
								{
									if (experienceCopyScene.m_ExperienceCopyMapCreateMonsterFlag == 0)
									{
										if (clientsList.Count<GameClient>() != 0 && clientsList[0] != null)
										{
											ExperienceCopySceneManager.ExperienceCopyMapCreateMonster(clientsList[0], experienceCopyScene, experienceCopyMapDataInfo, experienceCopyMapCreateMonsterWave);
										}
										else
										{
											ExperienceCopySceneManager.ExperienceCopyMapCreateMonster(null, experienceCopyScene, experienceCopyMapDataInfo, experienceCopyMapCreateMonsterWave);
										}
									}
								}
							}
						}
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					GameManager.CopyMapMgr.ProcessRemoveCopyMap(list[i]);
				}
			}
		}

		public static void ExperienceCopyMapCreateMonster(GameClient client, ExperienceCopyScene ExperienceMapInfo, ExperienceCopyMapDataInfo exMap, int nWave)
		{
			ExperienceMapInfo.m_ExperienceCopyMapCreateMonsterFlag = 1;
			ExperienceMapInfo.m_ExperienceCopyMapCreateMonsterWave++;
			GameMap gameMap = null;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(ExperienceMapInfo.m_MapCodeID, out gameMap))
			{
				LogManager.WriteLog(2, string.Format("经验副本 地图配置 ID = {0}", ExperienceMapInfo.m_MapCodeID), null, true);
			}
			else
			{
				int gridX = gameMap.CorrectWidthPointToGridPoint(exMap.posX) / gameMap.MapGridWidth;
				int gridY = gameMap.CorrectHeightPointToGridPoint(exMap.posZ) / gameMap.MapGridHeight;
				int radius = gameMap.CorrectWidthPointToGridPoint(exMap.Radius);
				int num = 0;
				List<int> list = exMap.MonsterIDList[nWave];
				List<int> list2 = exMap.MonsterNumList[nWave];
				for (int i = 0; i < list.Count; i++)
				{
					int monsterID = list[i];
					int num2 = list2[i];
					GameManager.MonsterZoneMgr.AddDynamicMonsters(ExperienceMapInfo.m_MapCodeID, monsterID, ExperienceMapInfo.m_CopyMapID, num2, gridX, gridY, radius, 0, 0, null, null);
					num += num2;
					ExperienceMapInfo.m_ExperienceCopyMapCreateMonsterNum += num2;
					ExperienceMapInfo.m_ExperienceCopyMapRemainMonsterNum += num2;
				}
				ExperienceMapInfo.m_ExperienceCopyMapNeedKillMonsterNum = ExperienceMapInfo.m_ExperienceCopyMapCreateMonsterNum * exMap.CreateNextWaveMonsterCondition[nWave] / 100;
				if (client != null)
				{
					ExperienceCopySceneManager.SendMsgToClientForExperienceCopyMapInfo(client, ExperienceMapInfo.m_ExperienceCopyMapCreateMonsterWave);
				}
			}
		}

		public static void ExperienceCopyMapKillMonster(GameClient client, Monster monster)
		{
			ExperienceCopyMapDataInfo experienceCopyMapDataInfo = null;
			if (Data.ExperienceCopyMapDataInfoList.TryGetValue(client.ClientData.MapCode, out experienceCopyMapDataInfo))
			{
				ExperienceCopyScene experienceCopyScene = null;
				lock (ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo)
				{
					if (!ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo.TryGetValue(client.ClientData.FuBenSeqID, out experienceCopyScene))
					{
						return;
					}
				}
				if (experienceCopyScene != null)
				{
					CopyMap copyMap = null;
					if (ExperienceCopySceneManager.m_ExperienceListCopyMaps.TryGetValue(client.ClientData.FuBenSeqID, out copyMap))
					{
						if (copyMap != null)
						{
							experienceCopyScene.m_ExperienceCopyMapKillMonsterNum++;
							experienceCopyScene.m_ExperienceCopyMapKillMonsterTotalNum++;
							experienceCopyScene.m_ExperienceCopyMapRemainMonsterNum--;
							if (experienceCopyScene.m_ExperienceCopyMapCreateMonsterFlag == 1 && experienceCopyScene.m_ExperienceCopyMapKillMonsterNum == experienceCopyScene.m_ExperienceCopyMapNeedKillMonsterNum)
							{
								experienceCopyScene.m_ExperienceCopyMapCreateMonsterFlag = 0;
								experienceCopyScene.m_ExperienceCopyMapKillMonsterNum = 0;
								experienceCopyScene.m_ExperienceCopyMapCreateMonsterNum = 0;
							}
							if (experienceCopyScene.m_ExperienceCopyMapCreateMonsterWave == experienceCopyMapDataInfo.MonsterIDList.Count && experienceCopyScene.m_ExperienceCopyMapKillMonsterTotalNum == experienceCopyMapDataInfo.MonsterSum)
							{
								ExperienceCopySceneManager.SendMsgToClientForExperienceCopyMapAward(client);
							}
							int num = experienceCopyScene.m_ExperienceCopyMapCreateMonsterWave;
							if (experienceCopyScene.m_ExperienceCopyMapKillMonsterTotalNum == experienceCopyMapDataInfo.MonsterSum || experienceCopyScene.m_ExperienceCopyMapRemainMonsterNum == 0)
							{
								num++;
							}
							ExperienceCopySceneManager.SendMsgToClientForExperienceCopyMapInfo(client, num);
						}
					}
				}
			}
		}

		public static void SendMsgToClientForExperienceCopyMapInfo(GameClient client, int nWave)
		{
			ExperienceCopyScene experienceCopyScene = null;
			lock (ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo)
			{
				ExperienceCopySceneManager.m_ExperienceListCopyMapsInfo.TryGetValue(client.ClientData.FuBenSeqID, out experienceCopyScene);
			}
			if (experienceCopyScene != null)
			{
				int num = nWave;
				int count = Data.ExperienceCopyMapDataInfoList[client.ClientData.MapCode].MonsterIDList.Count;
				if (num > count)
				{
					num = count;
				}
				string data = string.Format("{0}:{1}:{2}", num, count, experienceCopyScene.m_ExperienceCopyMapRemainMonsterNum);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 565);
				Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
			}
		}

		public static void SendMsgToClientForExperienceCopyMapAward(GameClient client)
		{
			CopyMap copyMap = ExperienceCopySceneManager.m_ExperienceListCopyMaps[client.ClientData.FuBenSeqID];
			if (copyMap != null)
			{
				int num = FuBenManager.FindFuBenSeqIDByRoleID(client.ClientData.RoleID);
				FuBenTongGuanData fuBenTongGuanData = null;
				if (num > 0)
				{
					FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(num);
					if (null != fuBenInfoItem)
					{
						fuBenInfoItem.EndTicks = TimeUtil.NOW();
						int addNum = 1;
						if (fuBenInfoItem.nDayOfYear != TimeUtil.NowDateTime().DayOfYear)
						{
							addNum = 0;
						}
						int num2 = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
						if (num2 > 0)
						{
							int num3 = (int)((fuBenInfoItem.EndTicks - fuBenInfoItem.StartTicks) / 1000L);
							Global.UpdateFuBenDataForQuickPassTimer(client, num2, num3, addNum);
							FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(num2, client.ClientData.MapCode);
							if (fuBenMapItem.Experience > 0 && fuBenMapItem.Money1 > 0)
							{
								int nMaxTime = fuBenMapItem.MaxTime * 60;
								long startTicks = fuBenInfoItem.StartTicks;
								long endTicks = fuBenInfoItem.EndTicks;
								int nFinishTimer = (int)(endTicks - startTicks) / 1000;
								int killedNormalNum = 0;
								int nDieCount = fuBenInfoItem.nDieCount;
								fuBenTongGuanData = Global.GiveCopyMapGiftForScore(client, num2, client.ClientData.MapCode, nMaxTime, nFinishTimer, killedNormalNum, nDieCount, (int)((double)fuBenMapItem.Experience * fuBenInfoItem.AwardRate), (int)((double)fuBenMapItem.Money1 * fuBenInfoItem.AwardRate), fuBenMapItem, null);
							}
							GameManager.DBCmdMgr.AddDBCmd(10053, string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								client.ClientData.RoleID,
								Global.FormatRoleName(client, client.ClientData.RoleName),
								num2,
								num3
							}), null, client.ServerId);
							int nLev = -1;
							SystemXmlItem systemXmlItem = null;
							if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(num2, out systemXmlItem))
							{
								nLev = systemXmlItem.GetIntValue("FuBenLevel", -1);
							}
							GameManager.ClientMgr.UpdateRoleDailyData_FuBenNum(client, 1, nLev, false);
						}
					}
				}
				if (fuBenTongGuanData != null)
				{
					TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<FuBenTongGuanData>(fuBenTongGuanData, Global._TCPManager.TcpOutPacketPool, 521);
					if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
				}
			}
		}

		public static Dictionary<int, CopyMap> m_ExperienceListCopyMaps = new Dictionary<int, CopyMap>();

		public static Dictionary<int, ExperienceCopyScene> m_ExperienceListCopyMapsInfo = new Dictionary<int, ExperienceCopyScene>();

		private static long LastHeartBeatTicks = 0L;
	}
}
