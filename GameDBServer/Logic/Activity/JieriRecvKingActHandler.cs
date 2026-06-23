using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Activity
{
	public class JieriRecvKingActHandler : SingletonTemplate<JieriRecvKingActHandler>
	{
		private JieriRecvKingActHandler()
		{
		}

		public TCPProcessCmdResults ProcLoadJieriRecvKingRank(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				string fromDate = array[0].Replace('$', ':');
				string toDate = array[1].Replace('$', ':');
				int rankCnt = Convert.ToInt32(array[2]);
				List<JieriRecvKingItemData> list = JieriRecvKingActHandler.QueryJieriRecvKingRank(dbMgr, fromDate, toDate, rankCnt);
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				foreach (JieriRecvKingItemData jieriRecvKingItemData in list)
				{
					int getAwardTimes = 0;
					string text2 = "";
					DBQuery.GetAwardHistoryForRole(dbMgr, jieriRecvKingItemData.RoleID, jieriRecvKingItemData.ZoneID, 52, huoDongKeyString, out getAwardTimes, out text2);
					jieriRecvKingItemData.GetAwardTimes = getAwardTimes;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<JieriRecvKingItemData>>(list, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public TCPProcessCmdResults ProcLoadRoleJieriRecvKing(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				int roleID = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				JieriRecvKingItemData jieriRecvKingItemData = this.QueryRoleJieriRecvKing(dbMgr, fromDate, toDate, roleID);
				if (jieriRecvKingItemData == null)
				{
					jieriRecvKingItemData = new JieriRecvKingItemData();
					jieriRecvKingItemData.RoleID = roleID;
					jieriRecvKingItemData.Rolename = dbroleInfo.RoleName;
					jieriRecvKingItemData.TotalRecv = 0;
					jieriRecvKingItemData.ZoneID = dbroleInfo.ZoneID;
					jieriRecvKingItemData.Rank = -1;
					jieriRecvKingItemData.GetAwardTimes = 0;
				}
				else
				{
					int getAwardTimes = 0;
					string text2 = "";
					DBQuery.GetAwardHistoryForRole(dbMgr, jieriRecvKingItemData.RoleID, jieriRecvKingItemData.ZoneID, 52, Global.GetHuoDongKeyString(fromDate, toDate), out getAwardTimes, out text2);
					jieriRecvKingItemData.GetAwardTimes = getAwardTimes;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<JieriRecvKingItemData>(jieriRecvKingItemData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public TCPProcessCmdResults ProcGetJieriRecvKingAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1001), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					int num2 = DBWriter.AddHongDongAwardRecordForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 52, Global.GetHuoDongKeyString(fromDate, toDate), 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num2 < 0)
					{
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1008), nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", 1), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static List<JieriRecvKingItemData> QueryJieriRecvKingRank(DBManager dbMgr, string fromDate, string toDate, int rankCnt)
		{
			List<JieriRecvKingItemData> list = new List<JieriRecvKingItemData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT receiver, rname, zoneid, SUM(goodscnt) AS totalrecv FROM t_jierizengsong, t_roles WHERE receiver=rid AND isdel=0 AND sendtime>= '{0}' AND sendtime<='{1}' GROUP BY receiver ORDER BY totalrecv DESC, receiver ASC LIMIT {2}", fromDate, toDate, rankCnt);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int num = 0;
				while (mySQLDataReader.Read() && num < rankCnt)
				{
					JieriRecvKingItemData item = new JieriRecvKingItemData
					{
						RoleID = Convert.ToInt32(mySQLDataReader["receiver"].ToString()),
						Rolename = mySQLDataReader["rname"].ToString(),
						ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString()),
						TotalRecv = Convert.ToInt32(mySQLDataReader["totalrecv"].ToString()),
						Rank = num + 1
					};
					list.Add(item);
					num++;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		private JieriRecvKingItemData QueryRoleJieriRecvKing(DBManager dbMgr, string fromDate, string toDate, int roleID)
		{
			JieriRecvKingItemData jieriRecvKingItemData = null;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = "SELECT t_roles.rid, t_roles.rname, t_roles.zoneid, x.totalrecv from t_roles,  (SELECT t_jierizengsong.receiver, SUM(t_jierizengsong.goodscnt) AS totalrecv " + string.Format(" FROM t_jierizengsong WHERE t_jierizengsong.receiver={0} AND sendtime>= '{1}' AND sendtime<='{2}') x ", roleID, fromDate, toDate) + " where t_roles.isdel=0 and t_roles.rid = x.receiver ";
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					jieriRecvKingItemData = new JieriRecvKingItemData();
					jieriRecvKingItemData.RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString());
					jieriRecvKingItemData.Rolename = mySQLDataReader["rname"].ToString();
					jieriRecvKingItemData.ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString());
					jieriRecvKingItemData.TotalRecv = Convert.ToInt32(mySQLDataReader["totalrecv"].ToString());
					jieriRecvKingItemData.Rank = -1;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return jieriRecvKingItemData;
		}
	}
}
