using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	public class BangHuiListMgr
	{
		public void RefreshBangHuiListData(DBManager DBMgr)
		{
			lock (this.BangHuiListMutex)
			{
				this.BangHuiListCacheData = DBQuery.GetBangHuiItemDataList(DBMgr, -1, 0, 10000);
			}
		}

		public BangHuiListData GetBangHuiListData(DBManager DBMgr, int isVerify, int startIndex, int endIndex)
		{
			BangHuiListData bangHuiListData = null;
			lock (this.BangHuiListMutex)
			{
				if (isVerify >= 0)
				{
					bangHuiListData = new BangHuiListData();
					bangHuiListData.BangHuiItemDataList = new List<BangHuiItemData>();
					foreach (BangHuiItemData bangHuiItemData in this.BangHuiListCacheData.BangHuiItemDataList)
					{
						if (isVerify == bangHuiItemData.IsVerfiy)
						{
							bangHuiListData.BangHuiItemDataList.Add(bangHuiItemData);
							bangHuiListData.TotalBangHuiItemNum++;
						}
					}
				}
				else
				{
					bangHuiListData = this.BangHuiListCacheData;
				}
			}
			return bangHuiListData;
		}

		public BangHuiCacheData GetBangHuiCacheData(int bhid)
		{
			BangHuiCacheData result;
			lock (this.BangHuiListMutex)
			{
				BangHuiCacheData bangHuiCacheData;
				if (!this.BangHuiCacheDataDict.TryGetValue(bhid, out bangHuiCacheData))
				{
					bangHuiCacheData = new BangHuiCacheData();
					if (!bangHuiCacheData.Query(bhid))
					{
						return null;
					}
					this.BangHuiCacheDataDict[bhid] = bangHuiCacheData;
				}
				result = bangHuiCacheData;
			}
			return result;
		}

		private object BangHuiListMutex = new object();

		private BangHuiListData BangHuiListCacheData = null;

		private Dictionary<int, BangHuiCacheData> BangHuiCacheDataDict = new Dictionary<int, BangHuiCacheData>();
	}
}
