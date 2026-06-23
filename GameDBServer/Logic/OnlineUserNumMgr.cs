using System;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
	public class OnlineUserNumMgr
	{
		public static void WriteTotalOnlineNumToDB(DBManager dbMgr)
		{
			DateTime now = DateTime.Now;
			long num = now.Ticks / 10000L;
			if (num - OnlineUserNumMgr.LastWriteDBTicks >= 120000L)
			{
				OnlineUserNumMgr.LastWriteDBTicks = num;
				int totalOnlineNum = LineManager.GetTotalOnlineNum();
				string mapOnlineNum = LineManager.GetMapOnlineNum();
				DBWriter.AddNewOnlineNumItem(dbMgr, totalOnlineNum, now, mapOnlineNum);
			}
		}

		public static void NotifyTotalOnlineNumToServer(DBManager dbMgr)
		{
			long num = DateTime.Now.Ticks / 10000L;
			if (num - OnlineUserNumMgr.LastNotifyDBTicks >= 30000L)
			{
				OnlineUserNumMgr.LastNotifyDBTicks = num;
				int totalOnlineNum = LineManager.GetTotalOnlineNum();
			}
		}

		private static long LastWriteDBTicks = DateTime.Now.Ticks / 10000L;

		private static long LastNotifyDBTicks = DateTime.Now.Ticks / 10000L;
	}
}
