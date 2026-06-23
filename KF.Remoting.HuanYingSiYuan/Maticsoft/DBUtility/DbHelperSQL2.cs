using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Maticsoft.DBUtility
{
	public abstract class DbHelperSQL2
	{
		public DbHelperSQL2()
		{
		}

		public static int GetMaxID(string FieldName, string TableName)
		{
			string text = "select max(" + FieldName + ")+1 from " + TableName;
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(text);
			object obj = database.ExecuteScalar(sqlStringCommand);
			int result;
			if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
			{
				result = 1;
			}
			else
			{
				result = int.Parse(obj.ToString());
			}
			return result;
		}

		public static bool Exists(string strSql)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(strSql);
			object obj = database.ExecuteScalar(sqlStringCommand);
			int num;
			if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
			{
				num = 0;
			}
			else
			{
				num = int.Parse(obj.ToString());
			}
			return num != 0;
		}

		public static bool Exists(string strSql, params SqlParameter[] cmdParms)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(strSql);
			DbHelperSQL2.BuildDBParameter(database, sqlStringCommand, cmdParms);
			object obj = database.ExecuteScalar(sqlStringCommand);
			int num;
			if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
			{
				num = 0;
			}
			else
			{
				num = int.Parse(obj.ToString());
			}
			return num != 0;
		}

		public static void BuildDBParameter(Database db, DbCommand dbCommand, params SqlParameter[] cmdParms)
		{
			foreach (SqlParameter sqlParameter in cmdParms)
			{
				db.AddInParameter(dbCommand, sqlParameter.ParameterName, sqlParameter.DbType, sqlParameter.Value);
			}
		}

		public static int ExecuteSql(string strSql)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(strSql);
			return database.ExecuteNonQuery(sqlStringCommand);
		}

		public static int ExecuteSqlByTime(string strSql, int Times)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(strSql);
			sqlStringCommand.CommandTimeout = Times;
			return database.ExecuteNonQuery(sqlStringCommand);
		}

		public static void ExecuteSqlTran(ArrayList SQLStringList)
		{
			Database database = DatabaseFactory.CreateDatabase();
			using (DbConnection dbConnection = database.CreateConnection())
			{
				dbConnection.Open();
				DbTransaction dbTransaction = dbConnection.BeginTransaction();
				try
				{
					for (int i = 0; i < SQLStringList.Count; i++)
					{
						string text = SQLStringList[i].ToString();
						if (text.Trim().Length > 1)
						{
							DbCommand sqlStringCommand = database.GetSqlStringCommand(text);
							database.ExecuteNonQuery(sqlStringCommand);
						}
					}
					dbTransaction.Commit();
				}
				catch
				{
					dbTransaction.Rollback();
				}
				finally
				{
					dbConnection.Close();
				}
			}
		}

		public static int ExecuteSql(string strSql, string content)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(strSql);
			database.AddInParameter(sqlStringCommand, "@content", DbType.String, content);
			return database.ExecuteNonQuery(sqlStringCommand);
		}

		public static object ExecuteSqlGet(string strSql, string content)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(strSql);
			database.AddInParameter(sqlStringCommand, "@content", DbType.String, content);
			object obj = database.ExecuteNonQuery(sqlStringCommand);
			object result;
			if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
			{
				result = null;
			}
			else
			{
				result = obj;
			}
			return result;
		}

		public static int ExecuteSqlInsertImg(string strSql, byte[] fs)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(strSql);
			database.AddInParameter(sqlStringCommand, "@fs", DbType.Byte, fs);
			return database.ExecuteNonQuery(sqlStringCommand);
		}

		public static object GetSingle(string strSql)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(strSql);
			object obj = database.ExecuteScalar(sqlStringCommand);
			object result;
			if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
			{
				result = null;
			}
			else
			{
				result = obj;
			}
			return result;
		}

		public static SqlDataReader ExecuteReader(string strSql)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(strSql);
			return (SqlDataReader)database.ExecuteReader(sqlStringCommand);
		}

		public static DataSet Query(string strSql)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(strSql);
			return database.ExecuteDataSet(sqlStringCommand);
		}

		public static DataSet Query(string strSql, int Times)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(strSql);
			sqlStringCommand.CommandTimeout = Times;
			return database.ExecuteDataSet(sqlStringCommand);
		}

		public static int ExecuteSql(string strSql, params SqlParameter[] cmdParms)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(strSql);
			DbHelperSQL2.BuildDBParameter(database, sqlStringCommand, cmdParms);
			return database.ExecuteNonQuery(sqlStringCommand);
		}

		public static void ExecuteSqlTran(Hashtable SQLStringList)
		{
			Database database = DatabaseFactory.CreateDatabase();
			using (DbConnection dbConnection = database.CreateConnection())
			{
				dbConnection.Open();
				DbTransaction dbTransaction = dbConnection.BeginTransaction();
				try
				{
					foreach (object obj in SQLStringList)
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
						string text = dictionaryEntry.Key.ToString();
						SqlParameter[] cmdParms = (SqlParameter[])dictionaryEntry.Value;
						if (text.Trim().Length > 1)
						{
							DbCommand sqlStringCommand = database.GetSqlStringCommand(text);
							DbHelperSQL2.BuildDBParameter(database, sqlStringCommand, cmdParms);
							database.ExecuteNonQuery(sqlStringCommand);
						}
					}
					dbTransaction.Commit();
				}
				catch
				{
					dbTransaction.Rollback();
				}
				finally
				{
					dbConnection.Close();
				}
			}
		}

		public static object GetSingle(string strSql, params SqlParameter[] cmdParms)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(strSql);
			DbHelperSQL2.BuildDBParameter(database, sqlStringCommand, cmdParms);
			object obj = database.ExecuteScalar(sqlStringCommand);
			object result;
			if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
			{
				result = null;
			}
			else
			{
				result = obj;
			}
			return result;
		}

		public static SqlDataReader ExecuteReader(string strSql, params SqlParameter[] cmdParms)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(strSql);
			DbHelperSQL2.BuildDBParameter(database, sqlStringCommand, cmdParms);
			return (SqlDataReader)database.ExecuteReader(sqlStringCommand);
		}

		public static DataSet Query(string strSql, params SqlParameter[] cmdParms)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand sqlStringCommand = database.GetSqlStringCommand(strSql);
			DbHelperSQL2.BuildDBParameter(database, sqlStringCommand, cmdParms);
			return database.ExecuteDataSet(sqlStringCommand);
		}

		private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
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
				foreach (SqlParameter sqlParameter in cmdParms)
				{
					if ((sqlParameter.Direction == ParameterDirection.InputOutput || sqlParameter.Direction == ParameterDirection.Input) && sqlParameter.Value == null)
					{
						sqlParameter.Value = DBNull.Value;
					}
					cmd.Parameters.Add(sqlParameter);
				}
			}
		}

		public static int RunProcedure(string storedProcName)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand storedProcCommand = database.GetStoredProcCommand(storedProcName);
			return database.ExecuteNonQuery(storedProcCommand);
		}

		public static object RunProcedure(string storedProcName, IDataParameter[] InParameters, SqlParameter OutParameter, int rowsAffected)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand storedProcCommand = database.GetStoredProcCommand(storedProcName);
			DbHelperSQL2.BuildDBParameter(database, storedProcCommand, (SqlParameter[])InParameters);
			database.AddOutParameter(storedProcCommand, OutParameter.ParameterName, OutParameter.DbType, OutParameter.Size);
			rowsAffected = database.ExecuteNonQuery(storedProcCommand);
			return database.GetParameterValue(storedProcCommand, "@" + OutParameter.ParameterName);
		}

		public static SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand storedProcCommand = database.GetStoredProcCommand(storedProcName, parameters);
			return (SqlDataReader)database.ExecuteReader(storedProcCommand);
		}

		public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand storedProcCommand = database.GetStoredProcCommand(storedProcName, parameters);
			return database.ExecuteDataSet(storedProcCommand);
		}

		public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
		{
			Database database = DatabaseFactory.CreateDatabase();
			DbCommand storedProcCommand = database.GetStoredProcCommand(storedProcName, parameters);
			storedProcCommand.CommandTimeout = Times;
			return database.ExecuteDataSet(storedProcCommand);
		}

		private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			SqlCommand sqlCommand = new SqlCommand(storedProcName, connection);
			sqlCommand.CommandType = CommandType.StoredProcedure;
			foreach (SqlParameter sqlParameter in parameters)
			{
				if (sqlParameter != null)
				{
					if ((sqlParameter.Direction == ParameterDirection.InputOutput || sqlParameter.Direction == ParameterDirection.Input) && sqlParameter.Value == null)
					{
						sqlParameter.Value = DBNull.Value;
					}
					sqlCommand.Parameters.Add(sqlParameter);
				}
			}
			return sqlCommand;
		}

		private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			SqlCommand sqlCommand = DbHelperSQL2.BuildQueryCommand(connection, storedProcName, parameters);
			sqlCommand.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
			return sqlCommand;
		}
	}
}
