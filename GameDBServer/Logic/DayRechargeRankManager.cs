using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	public class DayRechargeRankManager
	{
		public List<InputKingPaiHangData> GetRankByDay(DBManager dbMgr, int day)
		{
			List<InputKingPaiHangData> list = null;
			int offsetDay = Global.GetOffsetDay(DateTime.Now);
			List<InputKingPaiHangData> result;
			if (day > offsetDay)
			{
				result = null;
			}
			else
			{
				if (day < offsetDay)
				{
					lock (this.RechargeRankDict)
					{
						if (this.RechargeRankDict.ContainsKey(day))
						{
							list = this.RechargeRankDict[day];
							return list;
						}
					}
				}
				List<int> list2 = new List<int>();
				for (int i = 0; i < 4; i++)
				{
					list2.Add(1);
				}
				DateTime realDate = Global.GetRealDate(day);
				string fromDate = new DateTime(realDate.Year, realDate.Month, realDate.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
				string toDate = new DateTime(realDate.Year, realDate.Month, realDate.Day, 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
				list = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, list2, 4);
				if (null == list)
				{
					result = null;
				}
				else
				{
					foreach (InputKingPaiHangData inputKingPaiHangData in list)
					{
						Global.GetUserMaxLevelRole(dbMgr, inputKingPaiHangData.UserID, out inputKingPaiHangData.MaxLevelRoleName, out inputKingPaiHangData.MaxLevelRoleZoneID);
					}
					lock (this.RechargeRankDict)
					{
						this.RechargeRankDict[day] = list;
					}
					result = list;
				}
			}
			return result;
		}

		public int GetRoleRankByDay(DBManager dbMgr, string userid, int day)
		{
			List<InputKingPaiHangData> rankByDay = this.GetRankByDay(dbMgr, day);
			int result;
			if (null == rankByDay)
			{
				result = 0;
			}
			else
			{
				int num = 0;
				foreach (InputKingPaiHangData inputKingPaiHangData in rankByDay)
				{
					if (string.Compare(userid, inputKingPaiHangData.UserID) == 0)
					{
						return num + 1;
					}
					num++;
				}
				result = 0;
			}
			return result;
		}

		private const int HeFuRankCount = 4;

		private Dictionary<int, List<InputKingPaiHangData>> RechargeRankDict = new Dictionary<int, List<InputKingPaiHangData>>();
	}
}
