using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	internal class FreshPlayerCopySceneManager
	{
		public static void AddFreshPlayerListCopyMap(int nID, CopyMap mapInfo)
		{
			lock (FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps)
			{
				FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps.Add(nID, mapInfo);
			}
		}

		public static void RemoveFreshPlayerListCopyMap(int nID, CopyMap mapInfo)
		{
			lock (FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps)
			{
				FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps.Remove(nID);
			}
		}

		public static void HeartBeatFreshPlayerCopyMap()
		{
			long num = TimeUtil.NOW();
			if (num - FreshPlayerCopySceneManager.LastHeartBeatTicks >= 1000L)
			{
				FreshPlayerCopySceneManager.LastHeartBeatTicks = num;
				List<CopyMap> list = new List<CopyMap>();
				lock (FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps)
				{
					CopyMap copyMap = null;
					foreach (CopyMap copyMap2 in FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps.Values)
					{
						copyMap = copyMap2;
						if (copyMap2.FreshPlayerCreateGateFlag == 0)
						{
							FreshPlayerCopySceneManager.CreateGateMonster(copyMap2);
						}
						List<GameClient> clientsList = copyMap2.GetClientsList();
						if (clientsList != null && clientsList.Count <= 0)
						{
							list.Add(copyMap2);
						}
					}
					if (null != copyMap)
					{
						GameManager.MonsterZoneMgr.ReloadCopyMapMonsters(copyMap.MapCode, -1);
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					GameManager.CopyMapMgr.ProcessRemoveCopyMap(list[i]);
				}
			}
		}

		public static void KillMonsterInFreshPlayerScene(GameClient client, Monster monster)
		{
			CopyMap copyMap;
			lock (FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps)
			{
				if (!FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps.TryGetValue(client.ClientData.FuBenSeqID, out copyMap) || copyMap == null)
				{
					return;
				}
			}
			if (monster.MonsterInfo.VLevel >= Data.FreshPlayerSceneInfo.NeedKillMonster1Level)
			{
				copyMap.FreshPlayerKillMonsterACount++;
				if (copyMap.FreshPlayerKillMonsterACount >= Data.FreshPlayerSceneInfo.NeedKillMonster1Num)
				{
					string strCmd = string.Format("{0}", client.ClientData.RoleID);
					GameManager.ClientMgr.SendToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strCmd, 525);
				}
			}
			if (monster.MonsterInfo.ExtensionID == Data.FreshPlayerSceneInfo.NeedKillMonster2ID)
			{
				copyMap.FreshPlayerKillMonsterBCount++;
				if (copyMap.FreshPlayerKillMonsterBCount >= Data.FreshPlayerSceneInfo.NeedKillMonster2Num)
				{
					bool flag2 = false;
					TaskData taskData = Global.GetTaskData(client, 105);
					if (null != taskData)
					{
						flag2 = true;
					}
					if (flag2)
					{
						copyMap.HaveBirthShuiJingGuan = true;
						int monsterID = Data.FreshPlayerSceneInfo.CrystalID;
						string[] array = Data.FreshPlayerSceneInfo.CrystalPos.Split(new char[]
						{
							','
						});
						int value = Global.SafeConvertToInt32(array[0]);
						int value2 = Global.SafeConvertToInt32(array[1]);
						GameMap gameMap = null;
						if (!GameManager.MapMgr.DictMaps.TryGetValue(copyMap.MapCode, out gameMap))
						{
							return;
						}
						int gridX = gameMap.CorrectWidthPointToGridPoint(value) / gameMap.MapGridWidth;
						int gridY = gameMap.CorrectHeightPointToGridPoint(value2) / gameMap.MapGridHeight;
						GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMap.MapCode, monsterID, copyMap.CopyMapID, 1, gridX, gridY, 0, 0, 0, null, null);
					}
				}
			}
			if (monster.MonsterInfo.ExtensionID == Data.FreshPlayerSceneInfo.GateID)
			{
				FreshPlayerCopySceneManager.CreateMonsterBFreshPlayerScene(copyMap);
			}
			if (monster.MonsterInfo.ExtensionID == Data.FreshPlayerSceneInfo.CrystalID)
			{
				int monsterID = Data.FreshPlayerSceneInfo.DiaoXiangID;
				string[] array = Data.FreshPlayerSceneInfo.DiaoXiangPos.Split(new char[]
				{
					','
				});
				int value = Global.SafeConvertToInt32(array[0]);
				int value2 = Global.SafeConvertToInt32(array[1]);
				GameMap gameMap = null;
				if (GameManager.MapMgr.DictMaps.TryGetValue(copyMap.MapCode, out gameMap))
				{
					int gridX = gameMap.CorrectWidthPointToGridPoint(value) / gameMap.MapGridWidth;
					int gridY = gameMap.CorrectHeightPointToGridPoint(value2) / gameMap.MapGridHeight;
					GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMap.MapCode, monsterID, copyMap.CopyMapID, 1, gridX, gridY, 0, 0, 0, null, null);
				}
			}
		}

		public static void AddShuiJingGuanCaiMonsters(GameClient client)
		{
			CopyMap copyMap = null;
			lock (FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps)
			{
				if (!FreshPlayerCopySceneManager.m_FreshPlayerListCopyMaps.TryGetValue(client.ClientData.FuBenSeqID, out copyMap) || copyMap == null)
				{
					return;
				}
			}
			if (!copyMap.HaveBirthShuiJingGuan)
			{
				if (copyMap.FreshPlayerKillMonsterBCount >= Data.FreshPlayerSceneInfo.NeedKillMonster2Num)
				{
					bool flag2 = true;
					if (flag2)
					{
						copyMap.HaveBirthShuiJingGuan = true;
						int crystalID = Data.FreshPlayerSceneInfo.CrystalID;
						string[] array = Data.FreshPlayerSceneInfo.CrystalPos.Split(new char[]
						{
							','
						});
						int value = Global.SafeConvertToInt32(array[0]);
						int value2 = Global.SafeConvertToInt32(array[1]);
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(copyMap.MapCode, out gameMap))
						{
							int gridX = gameMap.CorrectWidthPointToGridPoint(value) / gameMap.MapGridWidth;
							int gridY = gameMap.CorrectHeightPointToGridPoint(value2) / gameMap.MapGridHeight;
							GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMap.MapCode, crystalID, copyMap.CopyMapID, 1, gridX, gridY, 0, 0, 0, null, null);
						}
					}
				}
			}
		}

		public static void CreateMonsterBFreshPlayerScene(CopyMap copyMapInfo)
		{
			int needKillMonster2ID = Data.FreshPlayerSceneInfo.NeedKillMonster2ID;
			string[] array = Data.FreshPlayerSceneInfo.NeedCreateMonster2Pos.Split(new char[]
			{
				','
			});
			int value = Global.SafeConvertToInt32(array[0]);
			int value2 = Global.SafeConvertToInt32(array[1]);
			GameMap gameMap = null;
			if (GameManager.MapMgr.DictMaps.TryGetValue(copyMapInfo.MapCode, out gameMap))
			{
				int gridX = gameMap.CorrectWidthPointToGridPoint(value) / gameMap.MapGridWidth;
				int gridY = gameMap.CorrectHeightPointToGridPoint(value2) / gameMap.MapGridHeight;
				int radius = gameMap.CorrectPointToGrid(Data.FreshPlayerSceneInfo.NeedCreateMonster2Radius);
				for (int i = 0; i < Data.FreshPlayerSceneInfo.NeedCreateMonster2Num; i++)
				{
					GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMapInfo.MapCode, needKillMonster2ID, copyMapInfo.CopyMapID, 1, gridX, gridY, radius, Data.FreshPlayerSceneInfo.NeedCreateMonster2PursuitRadius, 0, null, null);
				}
			}
		}

		public static void CreateGateMonster(CopyMap copyMapInfo)
		{
			int gateID = Data.FreshPlayerSceneInfo.GateID;
			string[] array = Data.FreshPlayerSceneInfo.GatePos.Split(new char[]
			{
				','
			});
			int value = Global.SafeConvertToInt32(array[0]);
			int value2 = Global.SafeConvertToInt32(array[1]);
			GameMap gameMap = null;
			if (GameManager.MapMgr.DictMaps.TryGetValue(copyMapInfo.MapCode, out gameMap))
			{
				int gridX = gameMap.CorrectWidthPointToGridPoint(value) / gameMap.MapGridWidth;
				int gridY = gameMap.CorrectHeightPointToGridPoint(value2) / gameMap.MapGridHeight;
				GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMapInfo.MapCode, gateID, copyMapInfo.CopyMapID, 1, gridX, gridY, 0, 0, 0, null, null);
				copyMapInfo.FreshPlayerCreateGateFlag = 1;
			}
		}

		public static Dictionary<int, CopyMap> m_FreshPlayerListCopyMaps = new Dictionary<int, CopyMap>();

		private static long LastHeartBeatTicks = 0L;
	}
}
