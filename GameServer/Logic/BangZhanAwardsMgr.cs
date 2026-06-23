using System;

namespace GameServer.Logic
{
	public class BangZhanAwardsMgr
	{
		public static void ClearAwardsByLevels()
		{
			BangZhanAwardsMgr.ExpByLevels = null;
			BangZhanAwardsMgr.RongYuByLevels = null;
		}

		private static long GetExpByLevel(int level)
		{
			long[] array = BangZhanAwardsMgr.ExpByLevels;
			if (null == array)
			{
				SystemXmlItem systemXmlItem = null;
				array = new long[Data.LevelUpExperienceList.Length - 1];
				for (int i = 1; i < array.Length; i++)
				{
					if (GameManager.systemBangZhanAwardsMgr.SystemXmlItemDict.TryGetValue(i, out systemXmlItem))
					{
						array[i] = (long)Global.GMax(0, systemXmlItem.GetIntValue("Experience", -1));
					}
				}
				BangZhanAwardsMgr.ExpByLevels = array;
			}
			long result;
			if (level <= 0 || level >= BangZhanAwardsMgr.ExpByLevels.Length)
			{
				result = 0L;
			}
			else
			{
				result = array[level];
			}
			return result;
		}

		private static int GetRongYuByLevel(int level)
		{
			int[] array = BangZhanAwardsMgr.RongYuByLevels;
			if (null == array)
			{
				SystemXmlItem systemXmlItem = null;
				array = new int[Data.LevelUpExperienceList.Length - 1];
				for (int i = 1; i < array.Length; i++)
				{
					if (GameManager.systemBangZhanAwardsMgr.SystemXmlItemDict.TryGetValue(i, out systemXmlItem))
					{
						array[i] = Global.GMax(0, systemXmlItem.GetIntValue("RongYu", -1));
					}
				}
				BangZhanAwardsMgr.RongYuByLevels = array;
			}
			int result;
			if (level <= 0 || level >= BangZhanAwardsMgr.RongYuByLevels.Length)
			{
				result = 0;
			}
			else
			{
				result = array[level];
			}
			return result;
		}

		private static void ProcessAddRoleExperience(GameClient client)
		{
			long expByLevel = BangZhanAwardsMgr.GetExpByLevel(client.ClientData.Level);
			if (expByLevel > 0L)
			{
				GameManager.ClientMgr.ProcessRoleExperience(client, expByLevel, true, false, false, "none");
			}
		}

		private static void ProcessAddRoleRongYu(GameClient client)
		{
			int rongYuByLevel = BangZhanAwardsMgr.GetRongYuByLevel(client.ClientData.Level);
			if (rongYuByLevel > 0)
			{
				GameManager.ClientMgr.ModifyRongYuValue(client, rongYuByLevel, true, true);
			}
		}

		public static void ProcessBangZhanAwards(GameClient client)
		{
		}

		private static long[] ExpByLevels = null;

		private static int[] RongYuByLevels = null;
	}
}
