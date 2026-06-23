using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Maticsoft.DBUtility
{
	public class DbHelperSQLP
	{
		public DbHelperSQLP()
		{
		}

		public DbHelperSQLP(string ConnectionString)
		{
			this.connectionString = ConnectionString;
		}

		public bool ColumnExists(string tableName, string columnName)
		{
			string sqlstring = string.Concat(new string[]
			{
				"select count(1) from syscolumns where [id]=object_id('",
				tableName,
				"') and [name]='",
				columnName,
				"'"
			});
			object single = this.GetSingle(sqlstring);
			return single != null && Convert.ToInt32(single) > 0;
		}

		public int GetMaxID(string FieldName, string TableName)
		{
			string sqlstring = "select max(" + FieldName + ")+1 from " + TableName;
			object single = this.GetSingle(sqlstring);
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

		public bool Exists(string strSql)
		{
			object single = this.GetSingle(strSql);
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

		public bool TabExists(string TableName)
		{
			string sqlstring = "select count(*) from sysobjects where id = object_id(N'[" + TableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
			object single = this.GetSingle(sqlstring);
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

		public bool Exists(string strSql, params SqlParameter[] cmdParms)
		{
			object single = this.GetSingle(strSql, cmdParms);
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

		public int ExecuteSql(string SQLString)
		{
			int result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				using (SqlCommand sqlCommand = new SqlCommand(SQLString, sqlConnection))
				{
					try
					{
						sqlConnection.Open();
						int num = sqlCommand.ExecuteNonQuery();
						result = num;
					}
					catch (SqlException ex)
					{
						sqlConnection.Close();
						throw ex;
					}
				}
			}
			return result;
		}

		public int ExecuteSqlByTime(string SQLString, int Times)
		{
			int result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				using (SqlCommand sqlCommand = new SqlCommand(SQLString, sqlConnection))
				{
					try
					{
						sqlConnection.Open();
						sqlCommand.CommandTimeout = Times;
						int num = sqlCommand.ExecuteNonQuery();
						result = num;
					}
					catch (SqlException ex)
					{
						sqlConnection.Close();
						throw ex;
					}
				}
			}
			return result;
		}

		public int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
		{
			int result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				sqlConnection.Open();
				SqlCommand sqlCommand = new SqlCommand();
				sqlCommand.Connection = sqlConnection;
				SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
				sqlCommand.Transaction = sqlTransaction;
				try
				{
					foreach (CommandInfo commandInfo in list)
					{
						string commandText = commandInfo.CommandText;
						SqlParameter[] cmdParms = (SqlParameter[])commandInfo.Parameters;
						DbHelperSQLP.PrepareCommand(sqlCommand, sqlConnection, sqlTransaction, commandText, cmdParms);
						if (commandInfo.EffentNextType == EffentNextType.SolicitationEvent)
						{
							if (commandInfo.CommandText.ToLower().IndexOf("count(") == -1)
							{
								sqlTransaction.Rollback();
								throw new Exception("违背要求" + commandInfo.CommandText + "必须符合select count(..的格式");
							}
							object obj = sqlCommand.ExecuteScalar();
							if (obj == null && obj == DBNull.Value)
							{
							}
							bool flag = Convert.ToInt32(obj) > 0;
							if (flag)
							{
								commandInfo.OnSolicitationEvent();
							}
						}
						if (commandInfo.EffentNextType == EffentNextType.WhenHaveContine || commandInfo.EffentNextType == EffentNextType.WhenNoHaveContine)
						{
							if (commandInfo.CommandText.ToLower().IndexOf("count(") == -1)
							{
								sqlTransaction.Rollback();
								throw new Exception("SQL:违背要求" + commandInfo.CommandText + "必须符合select count(..的格式");
							}
							object obj = sqlCommand.ExecuteScalar();
							if (obj == null && obj == DBNull.Value)
							{
							}
							bool flag = Convert.ToInt32(obj) > 0;
							if (commandInfo.EffentNextType == EffentNextType.WhenHaveContine && !flag)
							{
								sqlTransaction.Rollback();
								throw new Exception("SQL:违背要求" + commandInfo.CommandText + "返回值必须大于0");
							}
							if (commandInfo.EffentNextType == EffentNextType.WhenNoHaveContine && flag)
							{
								sqlTransaction.Rollback();
								throw new Exception("SQL:违背要求" + commandInfo.CommandText + "返回值必须等于0");
							}
						}
						else
						{
							int num = sqlCommand.ExecuteNonQuery();
							if (commandInfo.EffentNextType == EffentNextType.ExcuteEffectRows && num == 0)
							{
								sqlTransaction.Rollback();
								throw new Exception("SQL:违背要求" + commandInfo.CommandText + "必须有影响行");
							}
							sqlCommand.Parameters.Clear();
						}
					}
					string conStr = PubConstant.GetConnectionString("ConnectionStringPPC");
					if (!OracleHelper.ExecuteSqlTran(conStr, oracleCmdSqlList))
					{
						sqlTransaction.Rollback();
						throw new Exception("Oracle执行失败");
					}
					sqlTransaction.Commit();
					result = 1;
				}
				catch (SqlException ex)
				{
					sqlTransaction.Rollback();
					throw ex;
				}
				catch (Exception ex2)
				{
					sqlTransaction.Rollback();
					throw ex2;
				}
			}
			return result;
		}

		public int ExecuteSqlTran(List<string> SQLStringList)
		{
			int result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				sqlConnection.Open();
				SqlCommand sqlCommand = new SqlCommand();
				sqlCommand.Connection = sqlConnection;
				SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
				sqlCommand.Transaction = sqlTransaction;
				try
				{
					int num = 0;
					for (int i = 0; i < SQLStringList.Count; i++)
					{
						string text = SQLStringList[i];
						if (text.Trim().Length > 1)
						{
							sqlCommand.CommandText = text;
							num += sqlCommand.ExecuteNonQuery();
						}
					}
					sqlTransaction.Commit();
					result = num;
				}
				catch
				{
					sqlTransaction.Rollback();
					result = 0;
				}
			}
			return result;
		}

		public int ExecuteSql(string SQLString, string content)
		{
			int result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				SqlCommand sqlCommand = new SqlCommand(SQLString, sqlConnection);
				SqlParameter sqlParameter = new SqlParameter("@content", SqlDbType.NText);
				sqlParameter.Value = content;
				sqlCommand.Parameters.Add(sqlParameter);
				try
				{
					sqlConnection.Open();
					int num = sqlCommand.ExecuteNonQuery();
					result = num;
				}
				catch (SqlException ex)
				{
					throw ex;
				}
				finally
				{
					sqlCommand.Dispose();
					sqlConnection.Close();
				}
			}
			return result;
		}

		public object ExecuteSqlGet(string SQLString, string content)
		{
			object result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				SqlCommand sqlCommand = new SqlCommand(SQLString, sqlConnection);
				SqlParameter sqlParameter = new SqlParameter("@content", SqlDbType.NText);
				sqlParameter.Value = content;
				sqlCommand.Parameters.Add(sqlParameter);
				try
				{
					sqlConnection.Open();
					object obj = sqlCommand.ExecuteScalar();
					if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
					{
						result = null;
					}
					else
					{
						result = obj;
					}
				}
				catch (SqlException ex)
				{
					throw ex;
				}
				finally
				{
					sqlCommand.Dispose();
					sqlConnection.Close();
				}
			}
			return result;
		}

		public int ExecuteSqlInsertImg(string strSQL, byte[] fs)
		{
			int result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection);
				SqlParameter sqlParameter = new SqlParameter("@fs", SqlDbType.Image);
				sqlParameter.Value = fs;
				sqlCommand.Parameters.Add(sqlParameter);
				try
				{
					sqlConnection.Open();
					int num = sqlCommand.ExecuteNonQuery();
					result = num;
				}
				catch (SqlException ex)
				{
					throw ex;
				}
				finally
				{
					sqlCommand.Dispose();
					sqlConnection.Close();
				}
			}
			return result;
		}

		public object GetSingle(string SQLString)
		{
			object result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				using (SqlCommand sqlCommand = new SqlCommand(SQLString, sqlConnection))
				{
					try
					{
						sqlConnection.Open();
						object obj = sqlCommand.ExecuteScalar();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							result = null;
						}
						else
						{
							result = obj;
						}
					}
					catch (SqlException ex)
					{
						sqlConnection.Close();
						throw ex;
					}
				}
			}
			return result;
		}

		public object GetSingle(string SQLString, int Times)
		{
			object result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				using (SqlCommand sqlCommand = new SqlCommand(SQLString, sqlConnection))
				{
					try
					{
						sqlConnection.Open();
						sqlCommand.CommandTimeout = Times;
						object obj = sqlCommand.ExecuteScalar();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							result = null;
						}
						else
						{
							result = obj;
						}
					}
					catch (SqlException ex)
					{
						sqlConnection.Close();
						throw ex;
					}
				}
			}
			return result;
		}

		public SqlDataReader ExecuteReader(string strSQL)
		{
			SqlConnection sqlConnection = new SqlConnection(this.connectionString);
			SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection);
			SqlDataReader result;
			try
			{
				sqlConnection.Open();
				SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
				result = sqlDataReader;
			}
			catch (SqlException ex)
			{
				throw ex;
			}
			return result;
		}

		public DataSet Query(string SQLString)
		{
			DataSet result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				DataSet dataSet = new DataSet();
				try
				{
					sqlConnection.Open();
					SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(SQLString, sqlConnection);
					sqlDataAdapter.Fill(dataSet, "ds");
				}
				catch (SqlException ex)
				{
					throw new Exception(ex.Message);
				}
				result = dataSet;
			}
			return result;
		}

		public DataSet Query(string SQLString, int Times)
		{
			DataSet result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				DataSet dataSet = new DataSet();
				try
				{
					sqlConnection.Open();
					new SqlDataAdapter(SQLString, sqlConnection)
					{
						SelectCommand = 
						{
							CommandTimeout = Times
						}
					}.Fill(dataSet, "ds");
				}
				catch (SqlException ex)
				{
					throw new Exception(ex.Message);
				}
				result = dataSet;
			}
			return result;
		}

		public int ExecuteSql(string SQLString, params SqlParameter[] cmdParms)
		{
			int result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				using (SqlCommand sqlCommand = new SqlCommand())
				{
					try
					{
						DbHelperSQLP.PrepareCommand(sqlCommand, sqlConnection, null, SQLString, cmdParms);
						int num = sqlCommand.ExecuteNonQuery();
						sqlCommand.Parameters.Clear();
						result = num;
					}
					catch (SqlException ex)
					{
						throw ex;
					}
				}
			}
			return result;
		}

		public void ExecuteSqlTran(Hashtable SQLStringList)
		{
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				sqlConnection.Open();
				using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
				{
					SqlCommand sqlCommand = new SqlCommand();
					try
					{
						foreach (object obj in SQLStringList)
						{
							DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
							string cmdText = dictionaryEntry.Key.ToString();
							SqlParameter[] cmdParms = (SqlParameter[])dictionaryEntry.Value;
							DbHelperSQLP.PrepareCommand(sqlCommand, sqlConnection, sqlTransaction, cmdText, cmdParms);
							int num = sqlCommand.ExecuteNonQuery();
							sqlCommand.Parameters.Clear();
						}
						sqlTransaction.Commit();
					}
					catch
					{
						sqlTransaction.Rollback();
						throw;
					}
				}
			}
		}

		public int ExecuteSqlTran(List<CommandInfo> cmdList)
		{
			int result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				sqlConnection.Open();
				using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
				{
					SqlCommand sqlCommand = new SqlCommand();
					try
					{
						int num = 0;
						foreach (CommandInfo commandInfo in cmdList)
						{
							string commandText = commandInfo.CommandText;
							SqlParameter[] cmdParms = (SqlParameter[])commandInfo.Parameters;
							DbHelperSQLP.PrepareCommand(sqlCommand, sqlConnection, sqlTransaction, commandText, cmdParms);
							if (commandInfo.EffentNextType == EffentNextType.WhenHaveContine || commandInfo.EffentNextType == EffentNextType.WhenNoHaveContine)
							{
								if (commandInfo.CommandText.ToLower().IndexOf("count(") == -1)
								{
									sqlTransaction.Rollback();
									return 0;
								}
								object obj = sqlCommand.ExecuteScalar();
								if (obj == null && obj == DBNull.Value)
								{
								}
								bool flag = Convert.ToInt32(obj) > 0;
								if (commandInfo.EffentNextType == EffentNextType.WhenHaveContine && !flag)
								{
									sqlTransaction.Rollback();
									return 0;
								}
								if (commandInfo.EffentNextType == EffentNextType.WhenNoHaveContine && flag)
								{
									sqlTransaction.Rollback();
									return 0;
								}
							}
							else
							{
								int num2 = sqlCommand.ExecuteNonQuery();
								num += num2;
								if (commandInfo.EffentNextType == EffentNextType.ExcuteEffectRows && num2 == 0)
								{
									sqlTransaction.Rollback();
									return 0;
								}
								sqlCommand.Parameters.Clear();
							}
						}
						sqlTransaction.Commit();
						result = num;
					}
					catch
					{
						sqlTransaction.Rollback();
						throw;
					}
				}
			}
			return result;
		}

		public void ExecuteSqlTranWithIndentity(List<CommandInfo> SQLStringList)
		{
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				sqlConnection.Open();
				using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
				{
					SqlCommand sqlCommand = new SqlCommand();
					try
					{
						int num = 0;
						foreach (CommandInfo commandInfo in SQLStringList)
						{
							string commandText = commandInfo.CommandText;
							SqlParameter[] array = (SqlParameter[])commandInfo.Parameters;
							foreach (SqlParameter sqlParameter in array)
							{
								if (sqlParameter.Direction == ParameterDirection.InputOutput)
								{
									sqlParameter.Value = num;
								}
							}
							DbHelperSQLP.PrepareCommand(sqlCommand, sqlConnection, sqlTransaction, commandText, array);
							int num2 = sqlCommand.ExecuteNonQuery();
							foreach (SqlParameter sqlParameter in array)
							{
								if (sqlParameter.Direction == ParameterDirection.Output)
								{
									num = Convert.ToInt32(sqlParameter.Value);
								}
							}
							sqlCommand.Parameters.Clear();
						}
						sqlTransaction.Commit();
					}
					catch
					{
						sqlTransaction.Rollback();
						throw;
					}
				}
			}
		}

		public void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
		{
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				sqlConnection.Open();
				using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
				{
					SqlCommand sqlCommand = new SqlCommand();
					try
					{
						int num = 0;
						foreach (object obj in SQLStringList)
						{
							DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
							string cmdText = dictionaryEntry.Key.ToString();
							SqlParameter[] array = (SqlParameter[])dictionaryEntry.Value;
							foreach (SqlParameter sqlParameter in array)
							{
								if (sqlParameter.Direction == ParameterDirection.InputOutput)
								{
									sqlParameter.Value = num;
								}
							}
							DbHelperSQLP.PrepareCommand(sqlCommand, sqlConnection, sqlTransaction, cmdText, array);
							int num2 = sqlCommand.ExecuteNonQuery();
							foreach (SqlParameter sqlParameter in array)
							{
								if (sqlParameter.Direction == ParameterDirection.Output)
								{
									num = Convert.ToInt32(sqlParameter.Value);
								}
							}
							sqlCommand.Parameters.Clear();
						}
						sqlTransaction.Commit();
					}
					catch
					{
						sqlTransaction.Rollback();
						throw;
					}
				}
			}
		}

		public object GetSingle(string SQLString, params SqlParameter[] cmdParms)
		{
			object result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				using (SqlCommand sqlCommand = new SqlCommand())
				{
					try
					{
						DbHelperSQLP.PrepareCommand(sqlCommand, sqlConnection, null, SQLString, cmdParms);
						object obj = sqlCommand.ExecuteScalar();
						sqlCommand.Parameters.Clear();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							result = null;
						}
						else
						{
							result = obj;
						}
					}
					catch (SqlException ex)
					{
						throw ex;
					}
				}
			}
			return result;
		}

		public SqlDataReader ExecuteReader(string SQLString, params SqlParameter[] cmdParms)
		{
			SqlConnection conn = new SqlConnection(this.connectionString);
			SqlCommand sqlCommand = new SqlCommand();
			SqlDataReader result;
			try
			{
				DbHelperSQLP.PrepareCommand(sqlCommand, conn, null, SQLString, cmdParms);
				SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
				sqlCommand.Parameters.Clear();
				result = sqlDataReader;
			}
			catch (SqlException ex)
			{
				throw ex;
			}
			return result;
		}

		public DataSet Query(string SQLString, params SqlParameter[] cmdParms)
		{
			DataSet result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				SqlCommand sqlCommand = new SqlCommand();
				DbHelperSQLP.PrepareCommand(sqlCommand, sqlConnection, null, SQLString, cmdParms);
				using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
				{
					DataSet dataSet = new DataSet();
					try
					{
						sqlDataAdapter.Fill(dataSet, "ds");
						sqlCommand.Parameters.Clear();
					}
					catch (SqlException ex)
					{
						throw new Exception(ex.Message);
					}
					result = dataSet;
				}
			}
			return result;
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

		public SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
		{
			SqlDataReader result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				sqlConnection.Open();
				SqlCommand sqlCommand = DbHelperSQLP.BuildQueryCommand(sqlConnection, storedProcName, parameters);
				sqlCommand.CommandType = CommandType.StoredProcedure;
				SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
				result = sqlDataReader;
			}
			return result;
		}

		public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
		{
			DataSet result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				DataSet dataSet = new DataSet();
				sqlConnection.Open();
				new SqlDataAdapter
				{
					SelectCommand = DbHelperSQLP.BuildQueryCommand(sqlConnection, storedProcName, parameters)
				}.Fill(dataSet, tableName);
				sqlConnection.Close();
				result = dataSet;
			}
			return result;
		}

		public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
		{
			DataSet result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				DataSet dataSet = new DataSet();
				sqlConnection.Open();
				new SqlDataAdapter
				{
					SelectCommand = DbHelperSQLP.BuildQueryCommand(sqlConnection, storedProcName, parameters),
					SelectCommand = 
					{
						CommandTimeout = Times
					}
				}.Fill(dataSet, tableName);
				sqlConnection.Close();
				result = dataSet;
			}
			return result;
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

		public int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
		{
			int result;
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				sqlConnection.Open();
				SqlCommand sqlCommand = DbHelperSQLP.BuildIntCommand(sqlConnection, storedProcName, parameters);
				rowsAffected = sqlCommand.ExecuteNonQuery();
				int num = (int)sqlCommand.Parameters["ReturnValue"].Value;
				result = num;
			}
			return result;
		}

		private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
		{
			SqlCommand sqlCommand = DbHelperSQLP.BuildQueryCommand(connection, storedProcName, parameters);
			sqlCommand.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
			return sqlCommand;
		}

		public string connectionString = PubConstant.ConnectionString;
	}
}
