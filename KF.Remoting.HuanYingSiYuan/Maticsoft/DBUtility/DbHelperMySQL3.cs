using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using MySql.Data.MySqlClient;
using Tmsk.DbHelper;

namespace Maticsoft.DBUtility
{
	public abstract class DbHelperMySQL3
	{
		public static MyDbConnection2 PopDBConnection(string dbKey)
		{
			MyDbConnection2 myDbConnection = null;
			DbHelperMySQL3.SemaphoreClientsNoPool.WaitOne();
			try
			{
				string text;
				lock (DbHelperMySQL3.Mutex)
				{
					if (!DbHelperMySQL3.ConnectionStringDict.TryGetValue(dbKey, out text))
					{
						text = PubConstant.ConnectionString;
						string databaseName = PubConstant.GetDatabaseName(dbKey);
						int num = text.IndexOf("database=") + "database=".Length;
						int num2 = text.IndexOf(';', num);
						string oldValue = text.Substring(num, num2 - num);
						text = text.Replace(oldValue, databaseName);
						DbHelperMySQL3.ConnectionStringDict[dbKey] = text;
					}
				}
				myDbConnection = new MyDbConnection2(text, DbHelperMySQL3.CodePageNames);
				if (!myDbConnection.Open())
				{
					myDbConnection = null;
				}
			}
			catch (Exception ex)
			{
				myDbConnection = null;
			}
			finally
			{
				if (null == myDbConnection)
				{
					DbHelperMySQL3.SemaphoreClientsNoPool.Release();
				}
			}
			return myDbConnection;
		}

		public static void PushDBConnection(MyDbConnection2 conn)
		{
			if (conn != null)
			{
				DbHelperMySQL3.SemaphoreClientsNoPool.Release();
				conn.Close();
			}
		}

		public static int GetMaxID(string dbKey, string FieldName, string TableName)
		{
			string sqlstring = "select max(" + FieldName + ")+1 from " + TableName;
			object single = DbHelperMySQL3.GetSingle(dbKey, sqlstring);
			int result;
			if (single == null)
			{
				result = 1;
			}
			else
			{
				result = int.Parse(single.ToString());
			}
			return result;
		}

		public static bool Exists(string dbKey, string strSql)
		{
			object single = DbHelperMySQL3.GetSingle(dbKey, strSql);
			int num;
			if (object.Equals(single, null) || object.Equals(single, DBNull.Value))
			{
				num = 0;
			}
			else
			{
				num = int.Parse(single.ToString());
			}
			return num != 0;
		}

		public static bool Exists(string dbKey, string strSql, params MySqlParameter[] cmdParms)
		{
			object single = DbHelperMySQL3.GetSingle(dbKey, strSql, cmdParms);
			int num;
			if (object.Equals(single, null) || object.Equals(single, DBNull.Value))
			{
				num = 0;
			}
			else
			{
				num = int.Parse(single.ToString());
			}
			return num != 0;
		}

		public static int ExecuteSql(string dbKey, string SQLString)
		{
			MyDbConnection2 myDbConnection = null;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != myDbConnection)
				{
					return myDbConnection.ExecuteNonQuery(SQLString, 0);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return -1;
		}

		public static int ExecuteSqlByTime(string dbKey, string SQLString, int Times)
		{
			MyDbConnection2 myDbConnection = null;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != myDbConnection)
				{
					return myDbConnection.ExecuteNonQuery(SQLString, Times);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return -1;
		}

		public static int ExecuteSqlTran(string dbKey, List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
		{
			MyDbConnection2 myDbConnection = null;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != myDbConnection)
				{
					return myDbConnection.ExecuteSqlTran(list, oracleCmdSqlList);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return 0;
		}

		public static int ExecuteSqlTran(string dbKey, List<string> SQLStringList)
		{
			MyDbConnection2 myDbConnection = null;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != myDbConnection)
				{
					return myDbConnection.ExecuteSqlTran(SQLStringList);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return 0;
		}

		public static int ExecuteSql(string dbKey, string SQLString, string content)
		{
			MyDbConnection2 myDbConnection = null;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != myDbConnection)
				{
					return myDbConnection.ExecuteWithContent(SQLString, content);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return 0;
		}

		public static object ExecuteSqlGet(string dbKey, string SQLString, string content)
		{
			MyDbConnection2 myDbConnection = null;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == myDbConnection)
				{
					return myDbConnection.ExecuteSqlGet(SQLString, content);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return null;
		}

		public static int ExecuteSqlInsertImg(string dbKey, string strSQL, List<Tuple<string, byte[]>> imgList)
		{
			MyDbConnection2 myDbConnection = null;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != myDbConnection)
				{
					return myDbConnection.ExecuteSqlInsertImg(strSQL, imgList);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return -1;
		}

		public static object GetSingle(string dbKey, string SQLString)
		{
			MyDbConnection2 myDbConnection = null;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != myDbConnection)
				{
					return myDbConnection.GetSingle(SQLString, 0, new MySqlParameter[0]);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return null;
		}

		public static object GetSingle(string dbKey, string SQLString, int Times)
		{
			MyDbConnection2 myDbConnection = null;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != myDbConnection)
				{
					return myDbConnection.GetSingle(SQLString, Times, new MySqlParameter[0]);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return null;
		}

		public static MyDataReader ExecuteReader(string dbKey, string strSQL)
		{
			MyDbConnection2 myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
			MyDataReader result;
			if (null != myDbConnection)
			{
				MySqlDataReader dataReader = myDbConnection.ExecuteReader(strSQL, new MySqlParameter[0]);
				MyDataReader myDataReader = new MyDataReader(myDbConnection, dataReader);
				result = myDataReader;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static DataSet Query(string dbKey, string SQLString)
		{
			MyDbConnection2 myDbConnection = null;
			DataSet result;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == myDbConnection)
				{
					result = null;
				}
				else
				{
					result = myDbConnection.Query(SQLString, 0);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return result;
		}

		public static DataSet Query(string dbKey, string SQLString, int Times)
		{
			MyDbConnection2 myDbConnection = null;
			DataSet result;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == myDbConnection)
				{
					result = null;
				}
				else
				{
					result = myDbConnection.Query(SQLString, Times);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return result;
		}

		public static int ExecuteSql(string dbKey, string SQLString, params MySqlParameter[] cmdParms)
		{
			MyDbConnection2 myDbConnection = null;
			int result;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == myDbConnection)
				{
					result = -1;
				}
				else
				{
					result = myDbConnection.ExecuteSql(SQLString, cmdParms);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return result;
		}

		public static void ExecuteSqlTran(string dbKey, Hashtable SQLStringList)
		{
			MyDbConnection2 myDbConnection = null;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != myDbConnection)
				{
					myDbConnection.ExecuteSqlTran(SQLStringList);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
		}

		public static int ExecuteSqlTran(string dbKey, List<CommandInfo> cmdList)
		{
			MyDbConnection2 myDbConnection = null;
			int result;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == myDbConnection)
				{
					result = -1;
				}
				else
				{
					result = myDbConnection.ExecuteSqlTran(cmdList);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return result;
		}

		public static void ExecuteSqlTranWithIndentity(string dbKey, List<CommandInfo> SQLStringList)
		{
			MyDbConnection2 myDbConnection = null;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != myDbConnection)
				{
					myDbConnection.ExecuteSqlTranWithIndentity(SQLStringList);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
		}

		public static void ExecuteSqlTranWithIndentity(string dbKey, Hashtable SQLStringList)
		{
			MyDbConnection2 myDbConnection = null;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null != myDbConnection)
				{
					myDbConnection.ExecuteSqlTranWithIndentity(SQLStringList);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
		}

		public static object GetSingle(string dbKey, string SQLString, params MySqlParameter[] cmdParms)
		{
			MyDbConnection2 myDbConnection = null;
			object result;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == myDbConnection)
				{
					result = -1;
				}
				else
				{
					result = myDbConnection.GetSingle(SQLString, 0, cmdParms);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return result;
		}

		public static MySqlDataReader ExecuteReader(string dbKey, string SQLString, params MySqlParameter[] cmdParms)
		{
			MyDbConnection2 myDbConnection = null;
			MySqlDataReader result;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == myDbConnection)
				{
					result = null;
				}
				else
				{
					result = myDbConnection.ExecuteReader(SQLString, cmdParms);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return result;
		}

		public static DataSet Query(string dbKey, string SQLString, params MySqlParameter[] cmdParms)
		{
			MyDbConnection2 myDbConnection = null;
			DataSet result;
			try
			{
				myDbConnection = DbHelperMySQL3.PopDBConnection(dbKey);
				if (null == myDbConnection)
				{
					result = null;
				}
				else
				{
					result = myDbConnection.Query(SQLString, cmdParms);
				}
			}
			finally
			{
				DbHelperMySQL3.PushDBConnection(myDbConnection);
			}
			return result;
		}

		private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
		{
			if (conn.State != ConnectionState.Open)
			{
				conn.Open();
			}
			cmd.Connection = conn;
			cmd.CommandText = cmdText;
			if (trans != null)
			{
				cmd.Transaction = trans;
			}
			cmd.CommandType = CommandType.Text;
			if (cmdParms != null)
			{
				foreach (MySqlParameter mySqlParameter in cmdParms)
				{
					if ((mySqlParameter.Direction == ParameterDirection.InputOutput || mySqlParameter.Direction == ParameterDirection.Input) && mySqlParameter.Value == null)
					{
						mySqlParameter.Value = DBNull.Value;
					}
					cmd.Parameters.Add(mySqlParameter);
				}
			}
		}

		public const int LimitCount = 100;

		public const int InitCount = 1;

		public const bool UsePool = false;

		public static object Mutex = new object();

		public static string connectionString = PubConstant.ConnectionString;

		public static int MaxCount = 5;

		public static int ConnCount = 0;

		public static string CodePageNames = "utf8";

		public static int CodePage = 65001;

		private static Dictionary<string, MyDbConnectionPool> DBConnsDict = new Dictionary<string, MyDbConnectionPool>();

		public static Dictionary<string, string> ConnectionStringDict = new Dictionary<string, string>();

		public static Semaphore SemaphoreClientsNoPool = new Semaphore(50, 50);
	}
}
