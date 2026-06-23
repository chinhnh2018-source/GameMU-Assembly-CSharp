using System;
using System.Collections.Generic;
using GameDBServer.DB;

namespace GameDBServer.Logic
{
	public class UserMailManager
	{
		public static void ScanLastMails(DBManager dbMgr)
		{
			long num = DateTime.Now.Ticks / 10000L;
			if (num - UserMailManager.LastScanMailTicks >= 30000L)
			{
				UserMailManager.LastScanMailTicks = num;
				Dictionary<int, int> dictionary = DBQuery.ScanLastMailIDListFromTable(dbMgr);
				if (dictionary != null && dictionary.Count > 0)
				{
					string text = "";
					string text2 = "";
					foreach (KeyValuePair<int, int> keyValuePair in dictionary)
					{
						int key = keyValuePair.Key;
						DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref key);
						if (null != dbroleInfo)
						{
							if (text.Length > 0)
							{
								text += "_";
							}
							dbroleInfo.LastMailID = keyValuePair.Value;
							text += string.Format("{0}|{1}", dbroleInfo.RoleID, keyValuePair.Value);
						}
						else
						{
							DBWriter.UpdateRoleLastMail(dbMgr, keyValuePair.Key, keyValuePair.Value);
						}
						if (text2.Length > 0)
						{
							text2 += ",";
						}
						text2 += keyValuePair.Value;
					}
					if (text.Length > 0)
					{
						string gmCmd = string.Format("-notifymail {0}", text);
						ChatMsgManager.AddGMCmdChatMsg(-1, gmCmd);
					}
					if (text2.Length >= 0)
					{
						DBWriter.DeleteLastScanMailIDs(dbMgr, dictionary);
					}
				}
			}
		}

		public static void ClearOverdueMails(DBManager dbMgr)
		{
			long num = DateTime.Now.Ticks / 10000L;
			if (num - UserMailManager.LastClearMailTicks >= 123428447L)
			{
				UserMailManager.LastClearMailTicks = num;
				DBWriter.ClearOverdueMails(dbMgr, DateTime.Now.AddDays(-15.0));
			}
		}

		private const int OverdueDays = 15;

		private const long ClearOverdueMailInterval = 123428447L;

		private static long LastScanMailTicks = DateTime.Now.Ticks / 10000L;

		private static long LastClearMailTicks = DateTime.Now.Ticks / 10000L;
	}
}
