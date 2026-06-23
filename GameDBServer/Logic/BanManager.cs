using System;
using System.Collections.Generic;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
	public class BanManager
	{
		public static void BanRoleName(string roleName, int state)
		{
			lock (BanManager._RoleNameDict)
			{
				BanManager._RoleNameDict[roleName] = state;
			}
			lock (BanManager._RoleNameTicksDict)
			{
				if (state > 0)
				{
					BanManager._RoleNameTicksDict[roleName] = DateTime.Now.Ticks / 10000L;
				}
				else
				{
					BanManager._RoleNameTicksDict[roleName] = 0L;
				}
			}
		}

		public static int IsBanRoleName(string roleName)
		{
			int num = 0;
			lock (BanManager._RoleNameDict)
			{
				if (!BanManager._RoleNameDict.TryGetValue(roleName, out num))
				{
					num = 0;
				}
			}
			if (num > 0)
			{
				lock (BanManager._RoleNameTicksDict)
				{
					long num2 = 0L;
					if (!BanManager._RoleNameTicksDict.TryGetValue(roleName, out num2))
					{
						num = 0;
					}
					else
					{
						long num3 = DateTime.Now.Ticks / 10000L;
						if (num3 - num2 >= (long)(num * 60 * 1000))
						{
							num = 0;
						}
					}
				}
			}
			return num;
		}

		public static int GmBanCheckAdd(DBManager dbMgr, int roleID, string banIDs)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string arg = "t_ban_check";
				string sql = string.Format("INSERT INTO {0} (roleID, banIDs, logTime) VALUES({1}, '{2}', now())", arg, roleID, banIDs);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int GmBanCheckClear(DBManager dbMgr)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string arg = "t_ban_check";
				string sql = string.Format("DELETE FROM {0}", arg);
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public static int GmBanLogAdd(DBManager dbMgr, int zoneID, string userID, int roleID, int banType, string banID, int banCount, string deviceID)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string text = "t_ban_log";
				string sql = string.Format("INSERT INTO {0} (zoneID, userID, roleID, banType, banID, banCount, logTime, deviceID) VALUES({1}, '{2}', {3}, {4}, '{5}',{6}, now(), '{7}')", new object[]
				{
					text,
					zoneID,
					userID,
					roleID,
					banType,
					banID,
					banCount,
					deviceID
				});
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		private static Dictionary<string, int> _RoleNameDict = new Dictionary<string, int>();

		private static Dictionary<string, long> _RoleNameTicksDict = new Dictionary<string, long>();
	}
}
