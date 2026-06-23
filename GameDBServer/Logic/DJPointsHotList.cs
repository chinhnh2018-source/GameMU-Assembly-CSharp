using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	public class DJPointsHotList
	{
		public List<DJPointData> GetDJPointsHostList(DBManager dbMgr)
		{
			List<DJPointData> result = null;
			lock (this)
			{
				long num = DateTime.Now.Ticks / 10000L;
				if (num - this.LastQueryTicks <= 300000L)
				{
					result = this.DJPointsHostList;
				}
				else
				{
					this.LastQueryTicks = num;
					this.DJPointsHostList = DBQuery.QueryDJPointData(dbMgr);
					result = this.DJPointsHostList;
				}
			}
			return result;
		}

		private List<DJPointData> DJPointsHostList = new List<DJPointData>();

		private long LastQueryTicks = 0L;
	}
}
