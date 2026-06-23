using System;
using GameDBServer.Data;

namespace GameDBServer.DB.DBController
{
	internal class DBUserActiveInfo : DBController<AccountActiveData>
	{
		private DBUserActiveInfo()
		{
		}

		public static DBUserActiveInfo getInstance()
		{
			return DBUserActiveInfo.instance;
		}

		public AccountActiveData GetAccountActiveInfo(DBManager dbMgr, string strAccountID)
		{
			string sql = string.Format("select * from t_user_active_info where Account = '{0}';", strAccountID);
			return base.queryForObject(sql);
		}

		public bool UpdateAccountActiveInfo(DBManager dbMgr, string strAccountID)
		{
			bool result = false;
			string text = DateTime.Now.ToString("yyyy-MM-dd");
			AccountActiveData accountActiveInfo = this.GetAccountActiveInfo(dbMgr, strAccountID);
			if (null == accountActiveInfo)
			{
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("INSERT INTO t_user_active_info(Account, createTime, seriesLoginCount, lastSeriesLoginTime) VALUES('{0}', '{1}', {2}, '{3}')", new object[]
					{
						strAccountID,
						text,
						1,
						text
					});
					result = myDbConnection.ExecuteNonQueryBool(sql, 0);
				}
			}
			else
			{
				DateTime dateTime = DateTime.Now.AddDays(-1.0);
				DateTime dateTime2 = DateTime.Parse(accountActiveInfo.strLastSeriesLoginTime + " 00:00:00");
				if (dateTime.DayOfYear == dateTime2.DayOfYear)
				{
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("UPDATE t_user_active_info SET seriesLoginCount={0}, lastSeriesLoginTime='{1}' WHERE Account='{2}'", accountActiveInfo.nSeriesLoginCount + 1, text, accountActiveInfo.strAccount);
						result = myDbConnection.ExecuteNonQueryBool(sql, 0);
					}
				}
			}
			return result;
		}

		private static DBUserActiveInfo instance = new DBUserActiveInfo();
	}
}
