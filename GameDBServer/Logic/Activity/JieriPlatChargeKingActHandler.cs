using System;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic.Activity
{
	internal class JieriPlatChargeKingActHandler : SingletonTemplate<JieriPlatChargeKingActHandler>
	{
		public TCPProcessCmdResults ProcGetJieriPlatChargeKingList(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string instance = "";
			MySQLConnection mySQLConnection = null;
			try
			{
				string @string = new UTF8Encoding().GetString(data, 0, count);
				string[] array = @string.Split(new char[]
				{
					':'
				});
				string arg = array[0].Replace('$', ':');
				string arg2 = array[1].Replace('$', ':');
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT rank_info FROM t_plat_charge_king_log WHERE start_time='{0}' AND end_time='{1}' ORDER BY Id DESC LIMIT 1", arg, arg2);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					instance = Encoding.Default.GetString((byte[])mySQLDataReader["rank_info"]);
				}
				mySQLCommand.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<string>(instance, pool, nID);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}
	}
}
