using System;
using System.Collections.Generic;
using GameDBServer.Core;
using GameDBServer.Core.Executor;
using GameDBServer.DB.DBController;
using Server.Data;

namespace GameDBServer.Logic.BaiTan
{
	public class BaiTanManager : ScheduleTask, IManager
	{
		private BaiTanManager()
		{
		}

		public static BaiTanManager getInstance()
		{
			return BaiTanManager.instance;
		}

		public bool initialize()
		{
			List<BaiTanLogItemData> baiTanLogItemDataList = BaiTanLogDBController.getInstance().getBaiTanLogItemDataList();
			bool result;
			if (null == baiTanLogItemDataList)
			{
				result = true;
			}
			else
			{
				foreach (BaiTanLogItemData baiTanLogItemData in baiTanLogItemDataList)
				{
					List<BaiTanLogItemData> list = null;
					if (!this.dataCache.TryGetValue(baiTanLogItemData.rid, out list))
					{
						list = new List<BaiTanLogItemData>();
						this.dataCache.Add(baiTanLogItemData.rid, list);
					}
					if (list.Count < BaiTanManager.MaxCacheNum)
					{
						list.Add(baiTanLogItemData);
						if (!this.dataCache.TryGetValue(baiTanLogItemData.OtherRoleID, out list))
						{
							list = new List<BaiTanLogItemData>();
							this.dataCache.Add(baiTanLogItemData.OtherRoleID, list);
						}
						if (list.Count < BaiTanManager.MaxCacheNum)
						{
							list.Add(baiTanLogItemData);
						}
					}
				}
				result = true;
			}
			return result;
		}

		public bool startup()
		{
			this.executor.start();
			this.executor.scheduleExecute(this, 2000L, 5000L);
			return true;
		}

		public bool showdown()
		{
			this.executor.stop();
			this.deleteData();
			return true;
		}

		public bool destroy()
		{
			this.executor = null;
			bool result;
			if (this.dataCache == null)
			{
				result = true;
			}
			else
			{
				foreach (List<BaiTanLogItemData> list in this.dataCache.Values)
				{
					list.Clear();
				}
				this.dataCache.Clear();
				this.dataCache = null;
				result = true;
			}
			return result;
		}

		public void run()
		{
			if (TimeUtil.NOW() > this.executeSaveTime)
			{
				this.deleteData();
				this.executeSaveTime += BaiTanManager.deleteTime;
			}
		}

		public List<BaiTanLogItemData> getDetailByPageIndex(int bhId, int pageIndex)
		{
			int num = BaiTanManager.MaxCacheNum / BaiTanManager.PageShowNum;
			List<BaiTanLogItemData> result;
			if (pageIndex >= num)
			{
				result = null;
			}
			else
			{
				List<BaiTanLogItemData> list = null;
				if (!this.dataCache.TryGetValue(bhId, out list))
				{
					result = null;
				}
				else
				{
					int num2 = pageIndex * BaiTanManager.PageShowNum;
					int num3 = BaiTanManager.PageShowNum;
					if (num2 >= list.Count)
					{
						result = null;
					}
					else
					{
						if (num2 + num3 >= list.Count)
						{
							num3 = list.Count - num2;
						}
						if (num3 == 0)
						{
							result = null;
						}
						else
						{
							result = list.GetRange(num2, num3);
						}
					}
				}
			}
			return result;
		}

		private void deleteData()
		{
			foreach (int num in this.dataCache.Keys)
			{
				List<BaiTanLogItemData> list = null;
				this.dataCache.TryGetValue(num, out list);
				if (list != null && list.Count != 0)
				{
					string buyTime = list[list.Count - 1].BuyTime;
					int num2 = BaiTanLogDBController.getInstance().delete(num, buyTime);
				}
			}
		}

		public void onAddBaiTanLog(BaiTanLogItemData data)
		{
			List<BaiTanLogItemData> list = null;
			List<BaiTanLogItemData> list2 = null;
			lock (this.dataCache)
			{
				if (!this.dataCache.TryGetValue(data.rid, out list))
				{
					list = new List<BaiTanLogItemData>();
					this.dataCache.Add(data.rid, list);
				}
				if (!this.dataCache.TryGetValue(data.OtherRoleID, out list2))
				{
					list2 = new List<BaiTanLogItemData>();
					this.dataCache.Add(data.OtherRoleID, list2);
				}
			}
			lock (list)
			{
				list.Insert(0, data);
				if (list.Count > BaiTanManager.MaxCacheNum)
				{
					list.RemoveAt(list.Count - 1);
				}
			}
			lock (list2)
			{
				list2.Insert(0, data);
				if (list2.Count > BaiTanManager.MaxCacheNum)
				{
					list2.RemoveAt(list2.Count - 1);
				}
			}
			BaiTanLogDBController.getInstance().insert(data);
		}

		public static readonly int PageShowNum = 10;

		public static readonly int MaxCacheNum = 100;

		public static readonly long deleteTime = 3600000L;

		private static BaiTanManager instance = new BaiTanManager();

		private long executeSaveTime = TimeUtil.NOW() + BaiTanManager.deleteTime;

		private ScheduleExecutor executor = new ScheduleExecutor(1);

		private Dictionary<int, List<BaiTanLogItemData>> dataCache = new Dictionary<int, List<BaiTanLogItemData>>();
	}
}
