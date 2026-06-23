using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	internal class GuildManager
	{
		public static void AddGuildApply(int nID, int nRole)
		{
			lock (GuildManager.m_GuildApplyDic)
			{
				List<int> list = GuildManager.m_GuildApplyDic[nID];
				if (list == null)
				{
					list = new List<int>();
				}
				list.Add(nRole);
				GuildManager.m_GuildApplyDic[nID] = list;
			}
		}

		public static void RemoveGuildApply(int nID, int nRole)
		{
			lock (GuildManager.m_GuildApplyDic)
			{
				List<int> list = GuildManager.m_GuildApplyDic[nID];
				if (list != null)
				{
					list.Remove(nRole);
					GuildManager.m_GuildApplyDic[nID] = list;
				}
			}
		}

		public static Dictionary<int, List<int>> m_GuildApplyDic = new Dictionary<int, List<int>>();
	}
}
