using System;
using System.Collections.Generic;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
	public class HuangDiTeQuanMgr
	{
		private static void AddToHuangDiToOtherRoleDict(int cmdID, int otherRoleID)
		{
			string key = string.Format("{0}_{1}", cmdID, otherRoleID);
			HuangDiTeQuanMgr.HuangDiToOtherRoleDict[key] = DateTime.Now.DayOfYear;
		}

		public static bool FindHuangDiToOtherRoleDict(int cmdID, int otherRoleID)
		{
			string key = string.Format("{0}_{1}", cmdID, otherRoleID);
			bool result;
			lock (HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
			{
				int num = -1;
				if (!HuangDiTeQuanMgr.HuangDiToOtherRoleDict.TryGetValue(key, out num))
				{
					result = false;
				}
				else
				{
					result = (num == DateTime.Now.DayOfYear);
				}
			}
			return result;
		}

		public static void LoadHuangDiTeQuan(DBManager dbMgr)
		{
			HuangDiTeQuanMgr.MyHuangDiTeQuanItem = DBQuery.LoadHuangDiTeQuan(dbMgr);
			if (null == HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
			{
				HuangDiTeQuanMgr.MyHuangDiTeQuanItem = new HuangDiTeQuanItem
				{
					ID = 1,
					ToLaoFangDayID = 0,
					ToLaoFangNum = 0,
					OffLaoFangDayID = 0,
					OffLaoFangNum = 0,
					BanCatDayID = 0,
					BanCatNum = 0,
					LastBanChatTicks = 0L,
					LastSendToLaoFangTicks = 0L,
					LastTakeOffLaoFangTicks = 0L
				};
			}
		}

		public static HuangDiTeQuanItem GetHuangDiTeQuanItem()
		{
			return HuangDiTeQuanMgr.MyHuangDiTeQuanItem;
		}

		public static void ClearHuangDiTeQuan()
		{
			HuangDiTeQuanMgr.MyHuangDiTeQuanItem = new HuangDiTeQuanItem
			{
				ID = 1,
				ToLaoFangDayID = 0,
				ToLaoFangNum = 0,
				OffLaoFangDayID = 0,
				OffLaoFangNum = 0,
				BanCatDayID = 0,
				BanCatNum = 0,
				LastBanChatTicks = 0L,
				LastSendToLaoFangTicks = 0L,
				LastTakeOffLaoFangTicks = 0L
			};
		}

		public static bool CanExecuteHuangDiTeQuanNow(int cmdID)
		{
			long ticks = DateTime.Now.Ticks;
			if (344 == cmdID)
			{
				lock (HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
				{
					if (ticks - HuangDiTeQuanMgr.MyHuangDiTeQuanItem.LastSendToLaoFangTicks > 18000000000L)
					{
						return true;
					}
				}
			}
			else if (345 == cmdID)
			{
				lock (HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
				{
					if (ticks - HuangDiTeQuanMgr.MyHuangDiTeQuanItem.LastTakeOffLaoFangTicks > 18000000000L)
					{
						return true;
					}
				}
			}
			else if (346 == cmdID)
			{
				lock (HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
				{
					if (ticks - HuangDiTeQuanMgr.MyHuangDiTeQuanItem.LastBanChatTicks > 18000000000L)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool AddHuanDiTeQuan(int cmdID, int otherRoleID)
		{
			long ticks = DateTime.Now.Ticks;
			int dayOfYear = DateTime.Now.DayOfYear;
			if (344 == cmdID)
			{
				lock (HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
				{
					if (HuangDiTeQuanMgr.MyHuangDiTeQuanItem.ToLaoFangDayID == dayOfYear)
					{
						if (HuangDiTeQuanMgr.MyHuangDiTeQuanItem.ToLaoFangNum >= 3)
						{
							return false;
						}
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.ToLaoFangNum++;
					}
					else
					{
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.ToLaoFangDayID = dayOfYear;
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.ToLaoFangNum = 1;
					}
					HuangDiTeQuanMgr.MyHuangDiTeQuanItem.LastSendToLaoFangTicks = ticks;
					HuangDiTeQuanMgr.AddToHuangDiToOtherRoleDict(cmdID, otherRoleID);
					return true;
				}
			}
			if (345 == cmdID)
			{
				lock (HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
				{
					if (HuangDiTeQuanMgr.MyHuangDiTeQuanItem.OffLaoFangDayID == dayOfYear)
					{
						if (HuangDiTeQuanMgr.MyHuangDiTeQuanItem.OffLaoFangNum >= 3)
						{
							return false;
						}
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.OffLaoFangNum++;
					}
					else
					{
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.OffLaoFangDayID = dayOfYear;
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.OffLaoFangNum = 1;
					}
					HuangDiTeQuanMgr.MyHuangDiTeQuanItem.LastTakeOffLaoFangTicks = ticks;
					HuangDiTeQuanMgr.AddToHuangDiToOtherRoleDict(cmdID, otherRoleID);
					return true;
				}
			}
			if (346 == cmdID)
			{
				lock (HuangDiTeQuanMgr.MyHuangDiTeQuanItem)
				{
					if (HuangDiTeQuanMgr.MyHuangDiTeQuanItem.BanCatDayID == dayOfYear)
					{
						if (HuangDiTeQuanMgr.MyHuangDiTeQuanItem.BanCatNum >= 3)
						{
							return false;
						}
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.BanCatNum++;
					}
					else
					{
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.BanCatDayID = dayOfYear;
						HuangDiTeQuanMgr.MyHuangDiTeQuanItem.BanCatNum = 1;
					}
					HuangDiTeQuanMgr.MyHuangDiTeQuanItem.LastBanChatTicks = ticks;
					HuangDiTeQuanMgr.AddToHuangDiToOtherRoleDict(cmdID, otherRoleID);
					return true;
				}
			}
			return false;
		}

		private static HuangDiTeQuanItem MyHuangDiTeQuanItem = null;

		private static Dictionary<string, int> HuangDiToOtherRoleDict = new Dictionary<string, int>();
	}
}
