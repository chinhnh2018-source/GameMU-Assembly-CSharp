using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Maticsoft.DBUtility
{
	public abstract class SqlHelper
	{
		public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
		{
			SqlCommand sqlCommand = new SqlCommand();
			int result;
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				SqlHelper.PrepareCommand(sqlCommand, sqlConnection, null, cmdType, cmdText, commandParameters);
				int num = sqlCommand.ExecuteNonQuery();
				sqlCommand.Parameters.Clear();
				result = num;
			}
			return result;
		}

		public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
		{
			SqlCommand sqlCommand = new SqlCommand();
			SqlHelper.PrepareCommand(sqlCommand, connection, null, cmdType, cmdText, commandParameters);
			int result = sqlCommand.ExecuteNonQuery();
			sqlCommand.Parameters.Clear();
			return result;
		}

		public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
		{
			SqlCommand sqlCommand = new SqlCommand();
			SqlHelper.PrepareCommand(sqlCommand, trans.Connection, trans, cmdType, cmdText, commandParameters);
			int result = sqlCommand.ExecuteNonQuery();
			sqlCommand.Parameters.Clear();
			return result;
		}

		public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
		{
			SqlCommand sqlCommand = new SqlCommand();
			SqlConnection sqlConnection = new SqlConnection(connectionString);
			SqlDataReader result;
			try
			{
				SqlHelper.PrepareCommand(sqlCommand, sqlConnection, null, cmdType, cmdText, commandParameters);
				SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
				sqlCommand.Parameters.Clear();
				result = sqlDataReader;
			}
			catch
			{
				sqlConnection.Close();
				throw;
			}
			return result;
		}

		public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
		{
			SqlCommand sqlCommand = new SqlCommand();
			object result;
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				SqlHelper.PrepareCommand(sqlCommand, sqlConnection, null, cmdType, cmdText, commandParameters);
				object obj = sqlCommand.ExecuteScalar();
				sqlCommand.Parameters.Clear();
				result = obj;
			}
			return result;
		}

		public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
		{
			SqlCommand sqlCommand = new SqlCommand();
			SqlHelper.PrepareCommand(sqlCommand, connection, null, cmdType, cmdText, commandParameters);
			object result = sqlCommand.ExecuteScalar();
			sqlCommand.Parameters.Clear();
			return result;
		}

		public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
		{
			SqlHelper.parmCache[cacheKey] = commandParameters;
		}

		public static SqlParameter[] GetCachedParameters(string cacheKey)
		{
			SqlParameter[] array = (SqlParameter[])SqlHelper.parmCache[cacheKey];
			SqlParameter[] result;
			if (array == null)
			{
				result = null;
			}
			else
			{
				SqlParameter[] array2 = new SqlParameter[array.Length];
				int i = 0;
				int num = array.Length;
				while (i < num)
				{
					array2[i] = (SqlParameter)((ICloneable)array[i]).Clone();
					i++;
				}
				result = array2;
			}
			return result;
		}

		private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
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
			cmd.CommandType = cmdType;
			if (cmdParms != null)
			{
				foreach (SqlParameter value in cmdParms)
				{
					cmd.Parameters.Add(value);
				}
			}
		}

		public static readonly string ConnectionStringLocalTransaction = ConfigurationManager.AppSettings["SQLConnString1"];

		public static readonly string ConnectionStringInventoryDistributedTransaction = ConfigurationManager.AppSettings["SQLConnString2"];

		public static readonly string ConnectionStringOrderDistributedTransaction = ConfigurationManager.AppSettings["SQLConnString3"];

		public static readonly string ConnectionStringProfile = ConfigurationManager.AppSettings["SQLProfileConnString"];

		private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
	}
}
