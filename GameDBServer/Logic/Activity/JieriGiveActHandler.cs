using System;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Activity
{
	public class JieriGiveActHandler : SingletonTemplate<JieriGiveActHandler>
	{
		private JieriGiveActHandler()
		{
		}

		public static bool GetTotalGiveAndRecv(DBManager dbMgr, int roleid, string fromDate, string toDate, out int totalGive, out int totalRecv)
		{
			totalGive = 0;
			totalRecv = 0;
			bool result = false;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string str = string.Format("SELECT IFNULL(SUM(goodscnt), 0) AS totalcnt FROM t_jierizengsong WHERE sender={0} AND sendtime>='{1}' AND sendtime <= '{2}'", roleid, fromDate, toDate);
				string str2 = string.Format("SELECT IFNULL(SUM(goodscnt), 0) AS totalcnt FROM t_jierizengsong WHERE receiver={0} AND sendtime>='{1}' AND sendtime <= '{2}'", roleid, fromDate, toDate);
				string text = str + " UNION ALL " + str2;
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int num = 0;
				while (mySQLDataReader.Read() && num < 2)
				{
					if (num == 0)
					{
						totalGive = Convert.ToInt32(mySQLDataReader["totalcnt"].ToString());
					}
					else if (num == 1)
					{
						totalRecv = Convert.ToInt32(mySQLDataReader["totalcnt"].ToString());
					}
					num++;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
				result = true;
			}
			finally
			{
				if (mySQLConnection != null)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public bool AddJieriGiveRecord(DBManager dbMgr, int sender, int receiver, int goods, int cnt)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("INSERT INTO t_jierizengsong (sender, receiver, goodsid, goodscnt, sendtime) VALUES({0}, {1}, {2}, {3}, '{4}')", new object[]
				{
					sender,
					receiver,
					goods,
					cnt,
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
				});
				if (myDbConnection.ExecuteNonQuery(sql, 0) > 0)
				{
					result = true;
				}
			}
			return result;
		}

		public TCPProcessCmdResults ProcQueryJieriGiveInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int activitytype = Convert.ToInt32(array[1]);
				string fromDate = array[2].Replace('$', ':');
				string toDate = array[3].Replace('$', ':');
				int todayIdxInActPeriod = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				string empty = string.Empty;
				JieriGiveActHandler.GetTotalGiveAndRecv(dbMgr, num, fromDate, toDate, out num3, out num4);
				DBQuery.GetAwardHistoryForRole(dbMgr, num, dbroleInfo.ZoneID, activitytype, this._GetAwardKey_Ext_DayIdxInPeriod(fromDate, toDate, todayIdxInActPeriod), out num2, out empty);
				string data2 = string.Format("{0}:{1}:{2}", num3, num4, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public TCPProcessCmdResults ProcRoleJieriGiveToOther(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int sender = Convert.ToInt32(array[0]);
				string rolename = array[1];
				int goods = Convert.ToInt32(array[2]);
				int cnt = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(rolename);
				if (dbroleInfo == null)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!this.AddJieriGiveRecord(dbMgr, sender, dbroleInfo.RoleID, goods, cnt))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-2", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", dbroleInfo.RoleID), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public TCPProcessCmdResults ProcessGetJieriGiveAward(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int activitytype = Convert.ToInt32(array[3]);
				int todayIdxInActPeriod = Convert.ToInt32(array[4]);
				int hasgettimes = Convert.ToInt32(array[5]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1001), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string empty = string.Empty;
				int num2 = 0;
				lock (dbroleInfo)
				{
					string keystr = this._GetAwardKey_Ext_DayIdxInPeriod(fromDate, toDate, todayIdxInActPeriod);
					if (DBQuery.GetAwardHistoryForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, activitytype, keystr, out num2, out empty) < 0)
					{
						int num3 = DBWriter.AddHongDongAwardRecordForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, activitytype, keystr, hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num3 < 0)
						{
							string data2 = string.Format("{0}", -1008);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int num3 = DBWriter.UpdateHongDongAwardRecordForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, activitytype, keystr, hasgettimes, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num3 < 0)
						{
							string data2 = string.Format("{0}", -1008);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
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

		private string _GetAwardKey_Ext_DayIdxInPeriod(string fromDate, string toDate, int todayIdxInActPeriod)
		{
			return Global.GetHuoDongKeyString(fromDate, toDate) + "_" + todayIdxInActPeriod.ToString();
		}
	}
}
