using System;
using System.Collections.Generic;
using GameDBServer.Core;
using GameDBServer.Core.Executor;
using GameDBServer.Data;
using GameDBServer.DB.DBController;
using GameDBServer.Server;
using GameDBServer.Server.CmdProcessor;

namespace GameDBServer.Logic
{
	public class ZhanMengShiJianManager : ZhanMengShiJianConstants, ScheduleTask, IManager
	{
		private ZhanMengShiJianManager()
		{
		}

		public static ZhanMengShiJianManager getInstance()
		{
			return ZhanMengShiJianManager.instance;
		}

		public bool initialize()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(10138, ZhanMengShiJianCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(10139, ZhanMengShiJianDetailCmdProcessor.getInstance());
			List<ZhanMengShiJianData> zhanMengShiJianDataList = ZhanMengShiJianDBController.getInstance().getZhanMengShiJianDataList();
			bool result;
			if (null == zhanMengShiJianDataList)
			{
				result = true;
			}
			else
			{
				foreach (ZhanMengShiJianData zhanMengShiJianData in zhanMengShiJianDataList)
				{
					List<ZhanMengShiJianData> list = null;
					if (!this.dataCache.TryGetValue(zhanMengShiJianData.BHID, out list))
					{
						list = new List<ZhanMengShiJianData>();
						this.dataCache.Add(zhanMengShiJianData.BHID, list);
					}
					if (list.Count < ZhanMengShiJianConstants.MaxCacheNum)
					{
						list.Add(zhanMengShiJianData);
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
				foreach (List<ZhanMengShiJianData> list in this.dataCache.Values)
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
				this.executeSaveTime += ZhanMengShiJianConstants.deleteTime;
			}
		}

		public List<ZhanMengShiJianData> getDetailByPageIndex(int bhId, int pageIndex)
		{
			int num = ZhanMengShiJianConstants.MaxCacheNum / ZhanMengShiJianConstants.PageShowNum;
			List<ZhanMengShiJianData> result;
			if (pageIndex >= num)
			{
				result = null;
			}
			else
			{
				List<ZhanMengShiJianData> list = null;
				if (!this.dataCache.TryGetValue(bhId, out list))
				{
					result = null;
				}
				else
				{
					int num2 = pageIndex * ZhanMengShiJianConstants.PageShowNum;
					int num3 = ZhanMengShiJianConstants.PageShowNum;
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
				List<ZhanMengShiJianData> list = null;
				this.dataCache.TryGetValue(num, out list);
				if (list != null && list.Count != 0)
				{
					string createTime = list[list.Count - 1].CreateTime;
					int num2 = ZhanMengShiJianDBController.getInstance().delete(num, createTime);
				}
			}
		}

		public void onZhanMengJieSan(int bhId)
		{
			if (this.dataCache.Remove(bhId))
			{
				ZhanMengShiJianDBController.getInstance().delete(bhId);
			}
		}

		public void onAddZhanMengShiJian(ZhanMengShiJianData data)
		{
			List<ZhanMengShiJianData> list = null;
			lock (this.dataCache)
			{
				if (!this.dataCache.TryGetValue(data.BHID, out list))
				{
					list = new List<ZhanMengShiJianData>();
					this.dataCache.Add(data.BHID, list);
				}
			}
			lock (list)
			{
				list.Insert(0, data);
				if (list.Count > ZhanMengShiJianConstants.MaxCacheNum)
				{
					list.RemoveAt(list.Count - 1);
				}
			}
			ZhanMengShiJianDBController.getInstance().insert(data);
		}

		private static ZhanMengShiJianManager instance = new ZhanMengShiJianManager();

		private long executeSaveTime = TimeUtil.NOW() + ZhanMengShiJianConstants.deleteTime;

		private ScheduleExecutor executor = new ScheduleExecutor(1);

		private Dictionary<int, List<ZhanMengShiJianData>> dataCache = new Dictionary<int, List<ZhanMengShiJianData>>();
	}
}
