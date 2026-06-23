using System;
using System.Collections.Generic;
using GameDBServer.Data;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
	internal class RebornStampManager
	{
		public static void InitRebornYinJi(DBManager dbMgr)
		{
			RebornStampManager.UserRebornData = DBQuery.GetRebornYinJiCached(dbMgr);
		}

		public static List<int> UnMakeYinJiUpdateInfo(string UpdateInfo)
		{
			List<int> list = new List<int>();
			List<int> result;
			if (UpdateInfo == "" || UpdateInfo == null)
			{
				result = list;
			}
			else
			{
				string[] array = UpdateInfo.Split(new char[]
				{
					'|'
				});
				string[] array2 = array[0].Split(new char[]
				{
					'_'
				});
				foreach (string value in array2)
				{
					list.Add(Convert.ToInt32(value));
				}
				array2 = array[1].Split(new char[]
				{
					'_'
				});
				foreach (string value in array2)
				{
					list.Add(Convert.ToInt32(value));
				}
				result = list;
			}
			return result;
		}

		public static RebornStampData GetUserRebornInfoFromCached(int RoleID)
		{
			RebornStampData result;
			if (!RebornStampManager.UserRebornData.ContainsKey(RoleID))
			{
				result = null;
			}
			else
			{
				result = RebornStampManager.UserRebornData[RoleID];
			}
			return result;
		}

		public static bool UpdateUserRebornInfo(int RoleID, string StampInfo, int ResetNum, int UsePoint)
		{
			bool result;
			if (RebornStampManager.UserRebornData.ContainsKey(RoleID))
			{
				List<int> stampInfo = RebornStampManager.UnMakeYinJiUpdateInfo(StampInfo);
				RebornStampManager.UserRebornData[RoleID].ResetNum = ResetNum;
				RebornStampManager.UserRebornData[RoleID].UsePoint = UsePoint;
				RebornStampManager.UserRebornData[RoleID].StampInfo = stampInfo;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static bool InsertUserRebornInfo(int RoleID, string StampInfo, int ResetNum, int UsePoint)
		{
			bool result;
			if (RebornStampManager.UserRebornData.ContainsKey(RoleID))
			{
				result = false;
			}
			else
			{
				RebornStampData rebornStampData = new RebornStampData();
				rebornStampData.RoleID = RoleID;
				rebornStampData.ResetNum = ResetNum;
				rebornStampData.UsePoint = UsePoint;
				rebornStampData.StampInfo = RebornStampManager.UnMakeYinJiUpdateInfo(StampInfo);
				RebornStampManager.UserRebornData.Add(RoleID, rebornStampData);
				result = true;
			}
			return result;
		}

		public static Dictionary<int, RebornStampData> UserRebornData = new Dictionary<int, RebornStampData>();
	}
}
