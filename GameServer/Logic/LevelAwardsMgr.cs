using System;

namespace GameServer.Logic
{
	public class LevelAwardsMgr
	{
		private void ClearAwardsByLevels()
		{
			this.ExpByLevels = null;
			this.RongYuByLevels = null;
		}

		public void LoadFromXMlFile(string fullFileName, string rootName, string keyName, int resType = 0)
		{
			this.ClearAwardsByLevels();
			this.systemLevelAwardsXml.LoadFromXMlFile(fullFileName, rootName, keyName, resType);
		}

		private long GetExpByLevel(GameClient client, int level)
		{
			long[] array = this.ExpByLevels;
			if (null == array)
			{
				SystemXmlItem systemXmlItem = null;
				array = new long[Data.LevelUpExperienceList.Length];
				for (int i = 1; i < array.Length; i++)
				{
					if (this.systemLevelAwardsXml.SystemXmlItemDict.TryGetValue(i, out systemXmlItem))
					{
						array[i] = (long)Global.GMax(0, systemXmlItem.GetIntValue("Experience", -1));
					}
				}
				this.ExpByLevels = array;
			}
			long result;
			if (level <= 0 || level >= this.ExpByLevels.Length)
			{
				result = 0L;
			}
			else
			{
				long exp = array[level];
				result = Global.GetExpMultiByZhuanShengExpXiShu(client, exp);
			}
			return result;
		}

		private int GetRongYuByLevel(int level)
		{
			return 0;
		}

		private void ProcessAddRoleExperience(GameClient client)
		{
			long expByLevel = this.GetExpByLevel(client, client.ClientData.Level);
			if (expByLevel > 0L)
			{
				GameManager.ClientMgr.ProcessRoleExperience(client, expByLevel, true, false, true, "none");
			}
		}

		private void ProcessAddRoleRongYu(GameClient client)
		{
			int rongYuByLevel = this.GetRongYuByLevel(client.ClientData.Level);
			if (rongYuByLevel > 0)
			{
				GameManager.ClientMgr.ModifyRongYuValue(client, rongYuByLevel, true, true);
			}
		}

		public void ProcessBangZhanAwards(GameClient client)
		{
			if (client.ClientData.GuanZhanGM <= 0)
			{
				this.ProcessAddRoleExperience(client);
			}
		}

		public SystemXmlItems systemLevelAwardsXml = new SystemXmlItems();

		private long[] ExpByLevels = null;

		private int[] RongYuByLevels = null;
	}
}
