using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	public class QiZhenGeBuManager
	{
		public static List<QizhenGeBuItemData> GetQizhenGeBuItemDataList(DBManager dbMgr)
		{
			long num = DateTime.Now.Ticks / 10000L;
			if (num - QiZhenGeBuManager.LastQueryTicks >= 600000L)
			{
				QiZhenGeBuManager.QizhenGeBuItemDataList = DBQuery.QueryQizhenGeBuItemDataList(dbMgr);
				QiZhenGeBuManager.LastQueryTicks = num;
			}
			return QiZhenGeBuManager.QizhenGeBuItemDataList;
		}

		private static List<QizhenGeBuItemData> QizhenGeBuItemDataList = null;

		private static long LastQueryTicks = 0L;
	}
}
