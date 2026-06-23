using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	public class MonsterBossManager
	{
		public static void AddBoss(Monster monster)
		{
			MonsterBossManager.BossList.Add(monster);
		}

		public static Dictionary<int, BossData> GetBossDictData()
		{
			Dictionary<int, BossData> dictionary = new Dictionary<int, BossData>();
			for (int i = 0; i < MonsterBossManager.BossList.Count; i++)
			{
				Monster monster = MonsterBossManager.BossList[i];
				string nextTime = "";
				if (!monster.Alive)
				{
					nextTime = monster.MonsterZoneNode.GetNextBirthTimePoint();
				}
				BossData bossData = new BossData
				{
					MonsterID = monster.RoleID,
					ExtensionID = monster.MonsterInfo.ExtensionID,
					KillMonsterName = monster.WhoKillMeName,
					KillerOnline = ((GameManager.ClientMgr.FindClient(monster.WhoKillMeID) != null) ? 1 : 0),
					NextTime = nextTime
				};
				dictionary[bossData.ExtensionID] = bossData;
			}
			return dictionary;
		}

		public static void OnChangeName(int roleId, string oldName, string newName)
		{
			for (int i = 0; i < MonsterBossManager.BossList.Count; i++)
			{
				Monster monster = MonsterBossManager.BossList[i];
				if (monster.WhoKillMeID == roleId)
				{
					monster.WhoKillMeName = newName;
				}
			}
		}

		private static List<Monster> BossList = new List<Monster>();
	}
}
