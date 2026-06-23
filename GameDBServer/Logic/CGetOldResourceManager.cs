using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	internal class CGetOldResourceManager
	{
		public static TCPProcessCmdResults ProcessQueryGetResourceInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				Dictionary<int, OldResourceInfo> instance = DBQuery.QueryResourceGetInfo(dbMgr, num);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, OldResourceInfo>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public static TCPProcessCmdResults ProcessUpdateGetResourceInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				byte[] array = DataHelper.ObjectToBytes<int>(0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, nID);
				Dictionary<int, Dictionary<int, OldResourceInfo>> dictionary = DataHelper.BytesToObject<Dictionary<int, Dictionary<int, OldResourceInfo>>>(data, 0, count);
				Dictionary<int, OldResourceInfo> dictionary2 = dictionary.Values.ToArray<Dictionary<int, OldResourceInfo>>()[0];
				if (dictionary == null)
				{
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = dictionary.Keys.ToArray<int>()[0];
				for (int i = 1; i < 20; i++)
				{
					OldResourceInfo info = null;
					if (dictionary2 != null)
					{
						dictionary2.TryGetValue(i, out info);
					}
					int num2 = DBWriter.UpdateResourceGetInfo(dbMgr, num, i, info);
					if (num2 <= 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新资源找回失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					}
					array = DataHelper.ObjectToBytes<int>(num2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, nID);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}
	}
}
