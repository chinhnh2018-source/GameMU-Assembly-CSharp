using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	internal class GroupMailManager
	{
		public static void ResetData()
		{
			lock (GroupMailManager.GroupMailDataDict_Mutex)
			{
				GroupMailManager.LastMaxGroupMailID = 0;
				GroupMailManager.GroupMailDataDict.Clear();
			}
		}

		public static void ScanLastGroupMails(DBManager dbMgr)
		{
			long num = DateTime.Now.Ticks / 10000L;
			if (num - GroupMailManager.LastScanGroupMailTicks >= 30000L)
			{
				GroupMailManager.LastScanGroupMailTicks = num;
				List<GroupMailData> list = DBQuery.ScanNewGroupMailFromTable(dbMgr, GroupMailManager.LastMaxGroupMailID);
				if (list != null && list.Count > 0)
				{
					foreach (GroupMailData groupMailData in list)
					{
						GroupMailManager.AddGroupMailData(groupMailData);
						if (groupMailData.GMailID > GroupMailManager.LastMaxGroupMailID)
						{
							GroupMailManager.LastMaxGroupMailID = groupMailData.GMailID;
						}
					}
					string gmCmd = string.Format("-notifygmail ", new object[0]);
					ChatMsgManager.AddGMCmdChatMsg(-1, gmCmd);
				}
			}
		}

		private static void AddGroupMailData(GroupMailData gmailData)
		{
			lock (GroupMailManager.GroupMailDataDict_Mutex)
			{
				GroupMailManager.GroupMailDataDict[gmailData.GMailID] = gmailData;
			}
		}

		private static List<GroupMailData> GetGroupMailList(int beginID)
		{
			List<GroupMailData> list = null;
			foreach (KeyValuePair<int, GroupMailData> keyValuePair in GroupMailManager.GroupMailDataDict)
			{
				if (keyValuePair.Value.GMailID > beginID)
				{
					if (null == list)
					{
						list = new List<GroupMailData>();
					}
					list.Add(keyValuePair.Value);
				}
			}
			return list;
		}

		public static TCPProcessCmdResults RequestNewGroupMailList(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int beginID = Convert.ToInt32(array[0]);
				List<GroupMailData> groupMailList = GroupMailManager.GetGroupMailList(beginID);
				byte[] array2 = DataHelper.ObjectToBytes<List<GroupMailData>>(groupMailList);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array2, 0, array2.Length, nID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ModifyGMailRecord(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int mailID = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = DBWriter.ModifyGMailRecord(dbMgr, num, num2, mailID);
				if (num3 > 0)
				{
					lock (dbroleInfo)
					{
						if (null == dbroleInfo.GroupMailRecordList)
						{
							dbroleInfo.GroupMailRecordList = new List<int>();
						}
						if (dbroleInfo.GroupMailRecordList.IndexOf(num2) < 0)
						{
							dbroleInfo.GroupMailRecordList.Add(num2);
						}
					}
				}
				string data2 = string.Format("{0}", num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static long LastScanGroupMailTicks = DateTime.Now.Ticks / 10000L;

		private static int LastMaxGroupMailID = 0;

		private static object GroupMailDataDict_Mutex = new object();

		private static Dictionary<int, GroupMailData> GroupMailDataDict = new Dictionary<int, GroupMailData>();
	}
}
