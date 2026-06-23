using System;
using System.Collections.Generic;
using System.Globalization;
using GameDBServer.DB;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class BangHuiLingDiManager
	{
		public void LoadBangHuiLingDiItemsDictFromDB(DBManager dbMgr)
		{
			DBQuery.QueryBHLingDiInfoDict(dbMgr, this._BangHuiLingDiItemsDict);
			for (int i = 1; i < 7; i++)
			{
				if (!this._BangHuiLingDiItemsDict.ContainsKey(i))
				{
					BangHuiLingDiInfoData bangHuiLingDiInfoData = new BangHuiLingDiInfoData
					{
						LingDiID = i,
						ZoneID = 0,
						BHName = "",
						LingDiTax = 0,
						TakeDayID = 0,
						TakeDayNum = 0,
						YestodayTax = 0,
						TaxDayID = 0,
						TodayTax = 0,
						TotalTax = 0
					};
					this._BangHuiLingDiItemsDict[bangHuiLingDiInfoData.LingDiID] = bangHuiLingDiInfoData;
				}
			}
		}

		public BangHuiLingDiInfoData FindBangHuiLingDiByID(int lingDiID)
		{
			BangHuiLingDiInfoData result = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out result))
				{
					return null;
				}
			}
			return result;
		}

		public void ClearBangHuiLingDi(int bhid)
		{
			lock (this._BangHuiLingDiItemsDict)
			{
				foreach (BangHuiLingDiInfoData bangHuiLingDiInfoData in this._BangHuiLingDiItemsDict.Values)
				{
					if (bangHuiLingDiInfoData.BHID == bhid)
					{
						bangHuiLingDiInfoData.BHID = 0;
						bangHuiLingDiInfoData.ZoneID = 0;
						bangHuiLingDiInfoData.BHName = "";
						bangHuiLingDiInfoData.LingDiTax = 0;
						bangHuiLingDiInfoData.TakeDayID = 0;
						bangHuiLingDiInfoData.TakeDayNum = 0;
						bangHuiLingDiInfoData.YestodayTax = 0;
						bangHuiLingDiInfoData.TaxDayID = 0;
						bangHuiLingDiInfoData.TodayTax = 0;
						bangHuiLingDiInfoData.TotalTax = 0;
					}
				}
			}
		}

		public void OnChangeBangHuiName(int bhid, string oldName, string newName)
		{
			lock (this._BangHuiLingDiItemsDict)
			{
				foreach (BangHuiLingDiInfoData bangHuiLingDiInfoData in this._BangHuiLingDiItemsDict.Values)
				{
					if (bangHuiLingDiInfoData.BHID == bhid)
					{
						bangHuiLingDiInfoData.BHName = newName;
					}
				}
			}
		}

		public void ClearBangHuiLingDiByID(int lingDiID)
		{
			lock (this._BangHuiLingDiItemsDict)
			{
				foreach (BangHuiLingDiInfoData bangHuiLingDiInfoData in this._BangHuiLingDiItemsDict.Values)
				{
					if (bangHuiLingDiInfoData.LingDiID == lingDiID)
					{
						bangHuiLingDiInfoData.TotalTax = 0;
						break;
					}
				}
			}
		}

		public BangHuiLingDiInfoData ClearLingDiBangHuiInfo(int lingDiID)
		{
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
				{
					return null;
				}
				bangHuiLingDiInfoData.BHID = 0;
				bangHuiLingDiInfoData.ZoneID = 0;
				bangHuiLingDiInfoData.BHName = "";
				bangHuiLingDiInfoData.LingDiTax = 0;
				bangHuiLingDiInfoData.TakeDayID = 0;
				bangHuiLingDiInfoData.TakeDayNum = 0;
				bangHuiLingDiInfoData.YestodayTax = 0;
				bangHuiLingDiInfoData.TaxDayID = 0;
				bangHuiLingDiInfoData.TodayTax = 0;
				bangHuiLingDiInfoData.TotalTax = 0;
			}
			return bangHuiLingDiInfoData;
		}

		public BangHuiLingDiInfoData AddBangHuiLingDi(int bhid, int zoneID, string bhName, int lingDiID)
		{
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.ContainsKey(lingDiID))
				{
					bangHuiLingDiInfoData = new BangHuiLingDiInfoData
					{
						LingDiID = lingDiID,
						BHID = bhid,
						ZoneID = zoneID,
						BHName = bhName,
						LingDiTax = 0,
						TakeDayID = 0,
						TakeDayNum = 0,
						YestodayTax = 0,
						TaxDayID = 0,
						TodayTax = 0,
						TotalTax = 0
					};
					this._BangHuiLingDiItemsDict[bangHuiLingDiInfoData.LingDiID] = bangHuiLingDiInfoData;
				}
				else
				{
					bangHuiLingDiInfoData = this._BangHuiLingDiItemsDict[lingDiID];
					if (bangHuiLingDiInfoData.BHID != bhid)
					{
						bangHuiLingDiInfoData.LingDiTax = 0;
						bangHuiLingDiInfoData.TakeDayID = 0;
						bangHuiLingDiInfoData.TakeDayNum = 0;
						bangHuiLingDiInfoData.YestodayTax = 0;
						bangHuiLingDiInfoData.TaxDayID = 0;
						bangHuiLingDiInfoData.TodayTax = 0;
						bangHuiLingDiInfoData.TotalTax = 0;
					}
					bangHuiLingDiInfoData.BHID = bhid;
					bangHuiLingDiInfoData.ZoneID = zoneID;
					bangHuiLingDiInfoData.BHName = bhName;
				}
			}
			return bangHuiLingDiInfoData;
		}

		public BangHuiLingDiInfoData UpdateBangHuiLingDiTax(int bhid, int lingDiID, int tax)
		{
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
				{
					return null;
				}
				if (bangHuiLingDiInfoData.BHID != bhid)
				{
					return null;
				}
				bangHuiLingDiInfoData.LingDiTax = tax;
			}
			return bangHuiLingDiInfoData;
		}

		public BangHuiLingDiInfoData UpdateBangHuiLingDiWarRequest(int lingDiID, string warRequest)
		{
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
				{
					return null;
				}
				bangHuiLingDiInfoData.WarRequest = warRequest;
			}
			return bangHuiLingDiInfoData;
		}

		public BangHuiLingDiInfoData AddLingDiTaxMoney(int bhid, int lingDiID, int addMoney)
		{
			int dayOfYear = DateTime.Now.DayOfYear;
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
				{
					return null;
				}
				if (bangHuiLingDiInfoData.BHID != bhid)
				{
					return null;
				}
				bangHuiLingDiInfoData.TotalTax += addMoney;
				if (bangHuiLingDiInfoData.TaxDayID == dayOfYear)
				{
					bangHuiLingDiInfoData.TodayTax += addMoney;
				}
				else
				{
					bangHuiLingDiInfoData.YestodayTax = bangHuiLingDiInfoData.TodayTax;
					bangHuiLingDiInfoData.TaxDayID = dayOfYear;
					bangHuiLingDiInfoData.TodayTax = addMoney;
				}
			}
			return bangHuiLingDiInfoData;
		}

		public BangHuiLingDiInfoData TakeLingDiTaxMoney(int bhid, int lingDiID, int takeMoney)
		{
			int dayOfYear = DateTime.Now.DayOfYear;
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
				{
					return null;
				}
				if (bangHuiLingDiInfoData.BHID != bhid)
				{
					return null;
				}
				if (dayOfYear == bangHuiLingDiInfoData.TakeDayID)
				{
					if (bangHuiLingDiInfoData.TakeDayNum >= 1)
					{
						return null;
					}
				}
				if ((double)takeMoney > (double)bangHuiLingDiInfoData.TotalTax * 0.25)
				{
					return null;
				}
				bangHuiLingDiInfoData.TakeDayID = dayOfYear;
				bangHuiLingDiInfoData.TakeDayNum = 1;
				bangHuiLingDiInfoData.TotalTax = Math.Max(bangHuiLingDiInfoData.TotalTax - takeMoney, 0);
			}
			return bangHuiLingDiInfoData;
		}

		public BangHuiLingDiInfoData TakeLingDiDailyAward(int bhid, int lingDiID)
		{
			int dayOfYear = DateTime.Now.DayOfYear;
			BangHuiLingDiInfoData bangHuiLingDiInfoData = null;
			lock (this._BangHuiLingDiItemsDict)
			{
				if (!this._BangHuiLingDiItemsDict.TryGetValue(lingDiID, out bangHuiLingDiInfoData))
				{
					return null;
				}
				if (bangHuiLingDiInfoData.BHID != bhid)
				{
					return null;
				}
				if (dayOfYear == bangHuiLingDiInfoData.AwardFetchDay)
				{
					return null;
				}
				bangHuiLingDiInfoData.AwardFetchDay = dayOfYear;
			}
			return bangHuiLingDiInfoData;
		}

		public TCPOutPacket GetBangHuiLingDiItemsDictTCPOutPacket(TCPOutPacketPool pool, int cmdID)
		{
			Dictionary<int, BangHuiLingDiItemData> dictionary = new Dictionary<int, BangHuiLingDiItemData>();
			lock (this._BangHuiLingDiItemsDict)
			{
				foreach (int key in this._BangHuiLingDiItemsDict.Keys)
				{
					BangHuiLingDiInfoData bangHuiLingDiInfoData = this._BangHuiLingDiItemsDict[key];
					dictionary[key] = new BangHuiLingDiItemData
					{
						LingDiID = bangHuiLingDiInfoData.LingDiID,
						BHID = bangHuiLingDiInfoData.BHID,
						ZoneID = bangHuiLingDiInfoData.ZoneID,
						BHName = bangHuiLingDiInfoData.BHName,
						LingDiTax = bangHuiLingDiInfoData.LingDiTax,
						WarRequest = bangHuiLingDiInfoData.WarRequest,
						AwardFetchDay = bangHuiLingDiInfoData.AwardFetchDay
					};
				}
			}
			return DataHelper.ObjectToTCPOutPacket<Dictionary<int, BangHuiLingDiItemData>>(dictionary, pool, cmdID);
		}

		public TCPOutPacket GetBangHuiLingDiInfosDictTCPOutPacket(TCPOutPacketPool pool, int bhid, int cmdID)
		{
			Dictionary<int, BangHuiLingDiInfoData> dictionary = new Dictionary<int, BangHuiLingDiInfoData>();
			lock (this._BangHuiLingDiItemsDict)
			{
				foreach (int key in this._BangHuiLingDiItemsDict.Keys)
				{
					BangHuiLingDiInfoData bangHuiLingDiInfoData = this._BangHuiLingDiItemsDict[key];
					if (bhid == bangHuiLingDiInfoData.BHID)
					{
						dictionary[key] = new BangHuiLingDiInfoData
						{
							LingDiID = bangHuiLingDiInfoData.LingDiID,
							BHID = bangHuiLingDiInfoData.BHID,
							ZoneID = bangHuiLingDiInfoData.ZoneID,
							BHName = bangHuiLingDiInfoData.BHName,
							LingDiTax = bangHuiLingDiInfoData.LingDiTax,
							TakeDayID = bangHuiLingDiInfoData.TakeDayID,
							TakeDayNum = bangHuiLingDiInfoData.TakeDayNum,
							YestodayTax = bangHuiLingDiInfoData.YestodayTax,
							TaxDayID = bangHuiLingDiInfoData.TaxDayID,
							TodayTax = bangHuiLingDiInfoData.TodayTax,
							TotalTax = bangHuiLingDiInfoData.TotalTax
						};
					}
				}
			}
			return DataHelper.ObjectToTCPOutPacket<Dictionary<int, BangHuiLingDiInfoData>>(dictionary, pool, cmdID);
		}

		private static int WeekOfYear()
		{
			GregorianCalendar gregorianCalendar = new GregorianCalendar();
			return gregorianCalendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
		}

		public void ProcessClearYangZhouTotalTax(DBManager dbMgr)
		{
			long ticks = DateTime.Now.Ticks;
			if (ticks - this.LastClearYangZhouTotalTaxTicks >= 600000000L)
			{
				this.LastClearYangZhouTotalTaxTicks = ticks;
				int num = BangHuiLingDiManager.WeekOfYear();
				if (num != this.ThisWeekID)
				{
					this.ThisWeekID = num;
					this.ClearBangHuiLingDiByID(3);
					DBWriter.ClearBHLingDiTotalTaxByID(dbMgr, 3);
				}
			}
		}

		private Dictionary<int, BangHuiLingDiInfoData> _BangHuiLingDiItemsDict = new Dictionary<int, BangHuiLingDiInfoData>();

		private int ThisWeekID = BangHuiLingDiManager.WeekOfYear();

		private long LastClearYangZhouTotalTaxTicks = DateTime.Now.Ticks;
	}
}
