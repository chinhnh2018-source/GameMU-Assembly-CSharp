using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Ornament
{
	public class OrnamentManager
	{
		public static TCPProcessCmdResults ProcessUpdateOrnamentDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				OrnamentUpdateDbData ornamentUpdateDbData = DataHelper.BytesToObject<OrnamentUpdateDbData>(data, 0, count);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref ornamentUpdateDbData.RoleId);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, ornamentUpdateDbData.RoleId), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					if (null == dbroleInfo.OrnamentDataDict)
					{
						dbroleInfo.OrnamentDataDict = new Dictionary<int, OrnamentData>();
					}
					dbroleInfo.OrnamentDataDict[ornamentUpdateDbData.Data.ID] = ornamentUpdateDbData.Data;
				}
				DBWriter.UpdateOramentData(dbMgr, ornamentUpdateDbData);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<bool>(true, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}
	}
}
