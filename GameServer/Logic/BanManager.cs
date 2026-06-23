using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class BanManager
	{
		public static void BanRoleName(string roleName, int banMinutes, int reason = 1)
		{
			lock (BanManager._RoleNameDict)
			{
				BanManager._RoleNameDict[roleName] = reason;
			}
			lock (BanManager._RoleNameTicksDict)
			{
				if (banMinutes > 0)
				{
					BanManager._RoleNameTicksDict[roleName] = TimeUtil.NOW() + (long)(banMinutes * 60 * 1000);
				}
				else
				{
					BanManager._RoleNameTicksDict[roleName] = 0L;
				}
			}
		}

		public static int IsBanRoleName(string roleName, out int leftSecs)
		{
			leftSecs = 0;
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
						long num3 = TimeUtil.NOW();
						if (num3 >= num2)
						{
							num = 0;
						}
						else
						{
							leftSecs = (int)((num2 - num3) / 1000L);
						}
					}
				}
			}
			return num;
		}

		public static void BanUserID2Memory(string userID)
		{
			int offsetHour = Global.GetOffsetHour(TimeUtil.NowDateTime());
			lock (BanManager.m_HourBanDictMutex)
			{
				if (BanManager.m_HourBanDict.ContainsKey(offsetHour))
				{
					if (!BanManager.m_HourBanDict[offsetHour].ContainsKey(userID))
					{
						BanManager.m_HourBanDict[offsetHour][userID] = 1;
					}
				}
				else
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>();
					dictionary[userID] = 1;
					BanManager.m_HourBanDict[offsetHour] = dictionary;
				}
			}
		}

		public static void ClearBanMemory(int nHour)
		{
			int offsetHour = Global.GetOffsetHour(TimeUtil.NowDateTime());
			int num = offsetHour - nHour;
			lock (BanManager.m_HourBanDictMutex)
			{
				List<int> list = new List<int>();
				foreach (KeyValuePair<int, Dictionary<string, int>> keyValuePair in BanManager.m_HourBanDict)
				{
					if (keyValuePair.Key <= offsetHour && keyValuePair.Key >= num)
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (int key in list)
				{
					BanManager.m_HourBanDict.Remove(key);
				}
			}
		}

		public static void CheckBanMemory()
		{
			int offsetHour = Global.GetOffsetHour(TimeUtil.NowDateTime());
			if (BanManager.m_UpdateHour != offsetHour)
			{
				int gameConfigItemInt = GameManager.PlatConfigMgr.GetGameConfigItemInt("fileban_hour", 24);
				int num = offsetHour - gameConfigItemInt;
				lock (BanManager.m_HourBanDictMutex)
				{
					List<int> list = new List<int>();
					foreach (KeyValuePair<int, Dictionary<string, int>> keyValuePair in BanManager.m_HourBanDict)
					{
						if (keyValuePair.Key < num)
						{
							list.Add(keyValuePair.Key);
						}
					}
					foreach (int key in list)
					{
						BanManager.m_HourBanDict.Remove(key);
					}
				}
				BanManager.m_UpdateHour = offsetHour;
			}
		}

		public static bool IsBanInMemory(string userID)
		{
			lock (BanManager.m_HourBanDictMutex)
			{
				foreach (KeyValuePair<int, Dictionary<string, int>> keyValuePair in BanManager.m_HourBanDict)
				{
					if (keyValuePair.Value.ContainsKey(userID))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool UnBanUserID2Memory(string userID)
		{
			lock (BanManager.m_HourBanDictMutex)
			{
				foreach (KeyValuePair<int, Dictionary<string, int>> keyValuePair in BanManager.m_HourBanDict)
				{
					keyValuePair.Value.Remove(userID);
				}
			}
			return false;
		}

		private static Dictionary<string, int> _RoleNameDict = new Dictionary<string, int>();

		private static Dictionary<string, long> _RoleNameTicksDict = new Dictionary<string, long>();

		private static object m_HourBanDictMutex = new object();

		private static Dictionary<int, Dictionary<string, int>> m_HourBanDict = new Dictionary<int, Dictionary<string, int>>();

		private static int m_UpdateHour = Global.GetOffsetHour(TimeUtil.NowDateTime());

		public enum BanReason
		{
			UseSpeedSoftware = 1,
			RobotTask,
			TradeException
		}
	}
}
