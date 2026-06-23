using System;
using System.Collections.Generic;

namespace GameDBServer.Logic
{
	public class RoleOnlineManager
	{
		public static long GetRoleOnlineTicks(int roleID)
		{
			long result = 0L;
			lock (RoleOnlineManager._RoleOlineTicksDict)
			{
				if (!RoleOnlineManager._RoleOlineTicksDict.TryGetValue(roleID, out result))
				{
					result = 0L;
				}
			}
			return result;
		}

		public static void UpdateRoleOnlineTicks(int roleID)
		{
			long value = DateTime.Now.Ticks / 10000L;
			lock (RoleOnlineManager._RoleOlineTicksDict)
			{
				if (RoleOnlineManager._RoleOlineTicksDict.ContainsKey(roleID))
				{
					RoleOnlineManager._RoleOlineTicksDict[roleID] = value;
				}
				else
				{
					RoleOnlineManager._RoleOlineTicksDict.Add(roleID, value);
				}
			}
		}

		public static void RemoveRoleOnlineTicks(int roleID)
		{
			lock (RoleOnlineManager._RoleOlineTicksDict)
			{
				RoleOnlineManager._RoleOlineTicksDict.Remove(roleID);
			}
		}

		private static Dictionary<int, long> _RoleOlineTicksDict = new Dictionary<int, long>();
	}
}
