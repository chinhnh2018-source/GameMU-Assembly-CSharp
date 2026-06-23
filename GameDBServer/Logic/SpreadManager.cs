using System;
using GameDBServer.DB;
using MySQLDriverCS;

namespace GameDBServer.Logic
{
	public class SpreadManager
	{
		public static string GetAward(DBManager dbMgr, int zoneID, int roleID)
		{
			string text = "";
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text2 = string.Format("SELECT type,state FROM t_spread_award WHERE zoneID = '{0}' AND roleID = '{1}'", zoneID, roleID);
				MySQLCommand mySQLCommand = new MySQLCommand(text2, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					if (text != "")
					{
						text += "$";
					}
					text = text + mySQLDataReader["type"].ToString() + "#";
					text += mySQLDataReader["state"].ToString();
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text2), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return text;
		}

		public static string UpdateAward(DBManager dbMgr, int zoneID, int roleID, int awardType, string award)
		{
			string result = "";
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_spread_award(zoneID,roleID,type,state) VALUES({0}, {1}, {2}, '{3}');", new object[]
				{
					zoneID,
					roleID,
					awardType,
					award
				});
				if (myDbConnection.ExecuteNonQuery(sql, 0) >= 0)
				{
					result = "1";
				}
			}
			return result;
		}
	}
}
