using System;
using GameDBServer.DB;
using MySQLDriverCS;

namespace GameDBServer.Logic
{
	public class BangHuiCacheData
	{
		public bool Query(int bhid)
		{
			this.BhId = bhid;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				using (MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(string.Format("select bhid,rid,voice from t_banghui where bhid={0}", bhid), new MySQLParameter[0]))
				{
					if (!mySQLDataReader.Read())
					{
						return false;
					}
					this.BhId = Convert.ToInt32(mySQLDataReader[0].ToString());
					this.LeaderId = Convert.ToInt64(mySQLDataReader[1].ToString());
					this.GVoicePrioritys = mySQLDataReader[2].ToString();
				}
			}
			return true;
		}

		public bool UpdateGVoicePrioritys(string prioritys)
		{
			this.GVoicePrioritys = prioritys;
			bool result;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("update t_banghui set voice='{1}' where bhid={0}", this.BhId, prioritys);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public int BhId;

		public long LeaderId;

		public string GVoicePrioritys;
	}
}
