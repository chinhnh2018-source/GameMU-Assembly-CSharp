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
	public class JieriGiveKingActHandler : SingletonTemplate<JieriGiveKingActHandler>
	{
		private JieriGiveKingActHandler()
		{
		}

		public TCPProcessCmdResults ProcLoadJieriGiveKingRank(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				List<JieriGiveKingItemData> list = JieriGiveKingActHandler.QueryJieriGiveKingRank(dbMgr, fromDate, toDate, rankCnt);
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				foreach (JieriGiveKingItemData jieriGiveKingItemData in list)
				{
					int getAwardTimes = 0;
					string text2 = "";
					DBQuery.GetAwardHistoryForRole(dbMgr, jieriGiveKingItemData.RoleID, jieriGiveKingItemData.ZoneID, 51, huoDongKeyString, out getAwardTimes, out text2);
					jieriGiveKingItemData.GetAwardTimes = getAwardTimes;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<JieriGiveKingItemData>>(list, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public TCPProcessCmdResults ProcLoadRoleJieriGiveKing(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				JieriGiveKingItemData jieriGiveKingItemData = this.QueryRoleJieriGiveKing(dbMgr, fromDate, toDate, roleID);
				if (jieriGiveKingItemData == null)
				{
					jieriGiveKingItemData = new JieriGiveKingItemData();
					jieriGiveKingItemData.RoleID = roleID;
					jieriGiveKingItemData.Rolename = dbroleInfo.RoleName;
					jieriGiveKingItemData.TotalGive = 0;
					jieriGiveKingItemData.ZoneID = dbroleInfo.ZoneID;
					jieriGiveKingItemData.Rank = -1;
					jieriGiveKingItemData.GetAwardTimes = 0;
				}
				else
				{
					int getAwardTimes = 0;
					string text2 = "";
					DBQuery.GetAwardHistoryForRole(dbMgr, jieriGiveKingItemData.RoleID, jieriGiveKingItemData.ZoneID, 51, Global.GetHuoDongKeyString(fromDate, toDate), out getAwardTimes, out text2);
					jieriGiveKingItemData.GetAwardTimes = getAwardTimes;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<JieriGiveKingItemData>(jieriGiveKingItemData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public TCPProcessCmdResults ProcGetJieriGiveKingAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				lock (dbroleInfo)
				{
					int num2 = DBWriter.AddHongDongAwardRecordForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 51, huoDongKeyString, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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

		public static List<JieriGiveKingItemData> QueryJieriGiveKingRank(DBManager dbMgr, string fromDate, string toDate, int rankCnt)
		{
			List<JieriGiveKingItemData> list = new List<JieriGiveKingItemData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT sender, rname, zoneid, SUM(goodscnt) AS totalsend FROM t_jierizengsong, t_roles WHERE sender=rid AND isdel=0 AND sendtime>= '{0}' AND sendtime<='{1}' GROUP BY sender ORDER BY totalsend DESC, sender ASC LIMIT {2}", fromDate, toDate, rankCnt);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int num = 0;
				while (mySQLDataReader.Read() && num < rankCnt)
				{
					JieriGiveKingItemData item = new JieriGiveKingItemData
					{
						RoleID = Convert.ToInt32(mySQLDataReader["sender"].ToString()),
						Rolename = mySQLDataReader["rname"].ToString(),
						ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString()),
						TotalGive = Convert.ToInt32(mySQLDataReader["totalsend"].ToString()),
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

		private JieriGiveKingItemData QueryRoleJieriGiveKing(DBManager dbMgr, string fromDate, string toDate, int roleID)
		{
			JieriGiveKingItemData jieriGiveKingItemData = null;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = "SELECT t_roles.rid, t_roles.rname, t_roles.zoneid, x.totalsend from t_roles,  (SELECT t_jierizengsong.sender, SUM(t_jierizengsong.goodscnt) AS totalsend " + string.Format(" FROM t_jierizengsong WHERE t_jierizengsong.sender={0} AND sendtime>= '{1}' AND sendtime<='{2}') x ", roleID, fromDate, toDate) + " where t_roles.isdel=0 and t_roles.rid = x.sender ";
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					jieriGiveKingItemData = new JieriGiveKingItemData();
					jieriGiveKingItemData.RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString());
					jieriGiveKingItemData.Rolename = mySQLDataReader["rname"].ToString();
					jieriGiveKingItemData.ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString());
					jieriGiveKingItemData.TotalGive = Convert.ToInt32(mySQLDataReader["totalsend"].ToString());
					jieriGiveKingItemData.Rank = -1;
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
			return jieriGiveKingItemData;
		}
	}
}
