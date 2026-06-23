using System;
using System.Collections.Generic;
using GameDBServer.Data;
using GameDBServer.DB;
using Server.Tools;

namespace GameDBServer.Logic
{
	internal class GiftCodeNewManager
	{
		public static void ScanLastGroup(DBManager dbMgr)
		{
			long num = DateTime.Now.Ticks / 10000L;
			if (num - GiftCodeNewManager.LastScanTicks >= 10000L)
			{
				GiftCodeNewManager.LastScanTicks = num;
				List<LineItem> lineItemList = LineManager.GetLineItemList();
				if (lineItemList != null && lineItemList.Count != 0)
				{
					bool flag = false;
					for (int i = 0; i < lineItemList.Count; i++)
					{
						if (lineItemList[i].LineID > 0 && (lineItemList[i].LineID < 9000 || lineItemList[i].LineID == GameDBManager.ZoneID))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						List<GiftCodeAwardData> list = DBQuery.ScanNewGiftCodeFromTable(dbMgr);
						if (list != null && list.Count != 0)
						{
							string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
							List<string> list2 = new List<string>();
							foreach (GiftCodeAwardData giftCodeAwardData in list)
							{
								if (giftCodeAwardData.RoleID > 0 && !string.IsNullOrEmpty(giftCodeAwardData.UserId) && !string.IsNullOrEmpty(giftCodeAwardData.GiftId) && !string.IsNullOrEmpty(giftCodeAwardData.CodeNo))
								{
									bool flag2 = DBWriter.UpdateGiftCodeState(dbMgr, giftCodeAwardData.Dbid, 1, time);
									if (flag2)
									{
										string item = string.Format("{0},{1},{2},{3}", new object[]
										{
											giftCodeAwardData.UserId,
											giftCodeAwardData.RoleID,
											giftCodeAwardData.GiftId,
											giftCodeAwardData.CodeNo
										});
										list2.Add(item);
									}
								}
								else
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("[GiftCodeNew]数据表t_giftcode相关配置DBID:{0},RoleId:{1},UserId:{2}错误!", giftCodeAwardData.Dbid, giftCodeAwardData.RoleID, giftCodeAwardData.UserId), null, true);
								}
							}
							if (list2.Count > 0)
							{
								string arg = string.Join("#", list2);
								string gmCmd = string.Format("-giftcodecmd {0}", arg);
								ChatMsgManager.AddGMCmdChatMsgToOneClient(gmCmd);
							}
							list.Clear();
							list2.Clear();
						}
					}
				}
			}
		}

		private static long LastScanTicks = DateTime.Now.Ticks / 10000L;
	}
}
