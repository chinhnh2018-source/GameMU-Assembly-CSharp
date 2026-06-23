using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace Maticsoft.DBUtility
{
	public abstract class DbHelperMySQL
	{
		public DbHelperMySQL()
		{
		}

		public static int GetMaxID(string FieldName, string TableName)
		{
			string sqlstring = "select max(" + FieldName + ")+1 from " + TableName;
			object single = DbHelperMySQL.GetSingle(sqlstring);
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

		public static bool Exists(string strSql)
		{
			object single = DbHelperMySQL.GetSingle(strSql);
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

		public static bool Exists(string strSql, params MySqlParameter[] cmdParms)
		{
			object single = DbHelperMySQL.GetSingle(strSql, cmdParms);
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

		public static int ExecuteSql(string SQLString)
		{
			int result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection))
				{
					try
					{
						mySqlConnection.Open();
						int num = mySqlCommand.ExecuteNonQuery();
						result = num;
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (mySqlCommand != null)
						{
							mySqlCommand.Dispose();
						}
						if (mySqlConnection != null)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
			return result;
		}

		public static int ExecuteSqlByTime(string SQLString, int Times)
		{
			int result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection))
				{
					try
					{
						mySqlConnection.Open();
						mySqlCommand.CommandTimeout = Times;
						int num = mySqlCommand.ExecuteNonQuery();
						result = num;
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (mySqlCommand != null)
						{
							mySqlCommand.Dispose();
						}
						if (mySqlConnection != null)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
			return result;
		}

		public static int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
		{
			int result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				mySqlConnection.Open();
				MySqlCommand mySqlCommand = new MySqlCommand();
				mySqlCommand.Connection = mySqlConnection;
				MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction();
				mySqlCommand.Transaction = mySqlTransaction;
				try
				{
					foreach (CommandInfo commandInfo in list)
					{
						string commandText = commandInfo.CommandText;
						MySqlParameter[] cmdParms = (MySqlParameter[])commandInfo.Parameters;
						DbHelperMySQL.PrepareCommand(mySqlCommand, mySqlConnection, mySqlTransaction, commandText, cmdParms);
						if (commandInfo.EffentNextType == EffentNextType.SolicitationEvent)
						{
							if (commandInfo.CommandText.ToLower().IndexOf("count(") == -1)
							{
								mySqlTransaction.Rollback();
								throw new Exception("违背要求" + commandInfo.CommandText + "必须符合select count(..的格式");
							}
							object obj = mySqlCommand.ExecuteScalar();
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
								mySqlTransaction.Rollback();
								throw new Exception("SQL:违背要求" + commandInfo.CommandText + "必须符合select count(..的格式");
							}
							object obj = mySqlCommand.ExecuteScalar();
							if (obj == null && obj == DBNull.Value)
							{
							}
							bool flag = Convert.ToInt32(obj) > 0;
							if (commandInfo.EffentNextType == EffentNextType.WhenHaveContine && !flag)
							{
								mySqlTransaction.Rollback();
								throw new Exception("SQL:违背要求" + commandInfo.CommandText + "返回值必须大于0");
							}
							if (commandInfo.EffentNextType == EffentNextType.WhenNoHaveContine && flag)
							{
								mySqlTransaction.Rollback();
								throw new Exception("SQL:违背要求" + commandInfo.CommandText + "返回值必须等于0");
							}
						}
						else
						{
							int num = mySqlCommand.ExecuteNonQuery();
							if (commandInfo.EffentNextType == EffentNextType.ExcuteEffectRows && num == 0)
							{
								mySqlTransaction.Rollback();
								throw new Exception("SQL:违背要求" + commandInfo.CommandText + "必须有影响行");
							}
							mySqlCommand.Parameters.Clear();
						}
					}
					string conStr = PubConstant.GetConnectionString("ConnectionStringPPC");
					if (!OracleHelper.ExecuteSqlTran(conStr, oracleCmdSqlList))
					{
						mySqlTransaction.Rollback();
						throw new Exception("执行失败");
					}
					mySqlTransaction.Commit();
					result = 1;
				}
				catch (MySqlException ex)
				{
					mySqlTransaction.Rollback();
					throw ex;
				}
				finally
				{
					if (mySqlCommand != null)
					{
						mySqlCommand.Dispose();
					}
					if (mySqlConnection != null)
					{
						mySqlConnection.Close();
					}
				}
			}
			return result;
		}

		public static int ExecuteSqlTran(List<string> SQLStringList)
		{
			int result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				mySqlConnection.Open();
				MySqlCommand mySqlCommand = new MySqlCommand();
				mySqlCommand.Connection = mySqlConnection;
				MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction();
				mySqlCommand.Transaction = mySqlTransaction;
				try
				{
					int num = 0;
					for (int i = 0; i < SQLStringList.Count; i++)
					{
						string text = SQLStringList[i];
						if (text.Trim().Length > 1)
						{
							mySqlCommand.CommandText = text;
							num += mySqlCommand.ExecuteNonQuery();
						}
					}
					mySqlTransaction.Commit();
					result = num;
				}
				catch
				{
					mySqlTransaction.Rollback();
					result = 0;
				}
				finally
				{
					if (mySqlCommand != null)
					{
						mySqlCommand.Dispose();
					}
					if (mySqlConnection != null)
					{
						mySqlConnection.Close();
					}
				}
			}
			return result;
		}

		public static int ExecuteSql(string SQLString, string content)
		{
			int result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection);
				MySqlParameter mySqlParameter = new MySqlParameter("@content", content);
				mySqlCommand.Parameters.Add(mySqlParameter);
				try
				{
					mySqlConnection.Open();
					int num = mySqlCommand.ExecuteNonQuery();
					result = num;
				}
				catch (MySqlException ex)
				{
					LogManager.WriteException(ex.ToString());
					throw;
				}
				finally
				{
					if (mySqlCommand != null)
					{
						mySqlCommand.Dispose();
					}
					if (mySqlConnection != null)
					{
						mySqlConnection.Close();
					}
				}
			}
			return result;
		}

		public static object ExecuteSqlGet(string SQLString, string content)
		{
			object result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection);
				MySqlParameter mySqlParameter = new MySqlParameter("@content", SqlDbType.NText);
				mySqlParameter.Value = content;
				mySqlCommand.Parameters.Add(mySqlParameter);
				try
				{
					mySqlConnection.Open();
					object obj = mySqlCommand.ExecuteScalar();
					if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
					{
						result = null;
					}
					else
					{
						result = obj;
					}
				}
				catch (MySqlException ex)
				{
					throw ex;
				}
				finally
				{
					if (mySqlCommand != null)
					{
						mySqlCommand.Dispose();
					}
					if (mySqlConnection != null)
					{
						mySqlConnection.Close();
					}
				}
			}
			return result;
		}

		public static int ExecuteSqlInsertImg(string strSQL, List<Tuple<string, byte[]>> imgList)
		{
			int result = 0;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(strSQL, mySqlConnection))
				{
					try
					{
						mySqlConnection.Open();
						foreach (Tuple<string, byte[]> tuple in imgList)
						{
							string item = tuple.Item1;
							byte[] item2 = tuple.Item2;
							MySqlParameter mySqlParameter = new MySqlParameter(item, item2);
							mySqlCommand.Parameters.Add(mySqlParameter);
						}
						result = mySqlCommand.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						throw ex;
					}
					finally
					{
						if (mySqlCommand != null)
						{
							mySqlCommand.Dispose();
						}
						if (mySqlConnection != null)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
			return result;
		}

		public static long ExecuteSqlGetIncrement(string SQLString, List<Tuple<string, byte[]>> imgList = null)
		{
			long result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection))
				{
					try
					{
						mySqlConnection.Open();
						if (null != imgList)
						{
							foreach (Tuple<string, byte[]> tuple in imgList)
							{
								string item = tuple.Item1;
								byte[] item2 = tuple.Item2;
								MySqlParameter mySqlParameter = new MySqlParameter(item, item2);
								mySqlCommand.Parameters.Add(mySqlParameter);
							}
						}
						mySqlCommand.ExecuteNonQuery();
						mySqlCommand.CommandText = "SELECT LAST_INSERT_ID();";
						object obj = mySqlCommand.ExecuteScalar();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							result = 0L;
						}
						else
						{
							long num;
							long.TryParse(obj.ToString(), out num);
							result = num;
						}
					}
					catch (MySqlException ex)
					{
						result = 0L;
					}
					finally
					{
						if (mySqlCommand != null)
						{
							mySqlCommand.Dispose();
						}
						if (mySqlConnection != null)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
			return result;
		}

		public static object GetSingle(string SQLString)
		{
			object result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection))
				{
					try
					{
						mySqlConnection.Open();
						object obj = mySqlCommand.ExecuteScalar();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							result = null;
						}
						else
						{
							result = obj;
						}
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (mySqlCommand != null)
						{
							mySqlCommand.Dispose();
						}
						if (mySqlConnection != null)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
			return result;
		}

		public static long GetSingleLong(string sql)
		{
			long num = 0L;
			object single = DbHelperMySQL.GetSingle(sql);
			long result;
			if (single != null && long.TryParse(single.ToString(), out num))
			{
				result = num;
			}
			else
			{
				result = -1L;
			}
			return result;
		}

		public static int GetSingleValues(string SQLString, out object[] values)
		{
			int result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection))
				{
					try
					{
						int num = 0;
						mySqlConnection.Open();
						MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
						values = new object[mySqlDataReader.FieldCount];
						if (mySqlDataReader.Read())
						{
							num = mySqlDataReader.GetValues(values);
						}
						mySqlDataReader.Close();
						result = num;
					}
					catch (MySqlException ex)
					{
						throw;
					}
					finally
					{
						if (mySqlCommand != null)
						{
							mySqlCommand.Dispose();
						}
						if (mySqlConnection != null)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
			return result;
		}

		public static object GetSingle(string SQLString, int Times)
		{
			object result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(SQLString, mySqlConnection))
				{
					try
					{
						mySqlConnection.Open();
						mySqlCommand.CommandTimeout = Times;
						object obj = mySqlCommand.ExecuteScalar();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							result = null;
						}
						else
						{
							result = obj;
						}
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (mySqlCommand != null)
						{
							mySqlCommand.Dispose();
						}
						if (mySqlConnection != null)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
			return result;
		}

		public static MySqlDataReader ExecuteReader(string strSQL, bool islog = false)
		{
			MySqlConnection mySqlConnection;
			if (islog)
			{
				mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionLogString);
			}
			else
			{
				mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString);
			}
			MySqlCommand mySqlCommand = new MySqlCommand(strSQL, mySqlConnection);
			MySqlDataReader result;
			try
			{
				mySqlConnection.Open();
				MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
				result = mySqlDataReader;
			}
			catch (MySqlException ex)
			{
				throw ex;
			}
			finally
			{
				if (mySqlCommand != null)
				{
					mySqlCommand.Clone();
				}
			}
			return result;
		}

		public static DataSet Query(string SQLString)
		{
			DataSet result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				DataSet dataSet = new DataSet();
				MySqlDataAdapter mySqlDataAdapter = null;
				try
				{
					mySqlConnection.Open();
					mySqlDataAdapter = new MySqlDataAdapter(SQLString, mySqlConnection);
					mySqlDataAdapter.Fill(dataSet, "ds");
				}
				catch (MySqlException ex)
				{
					throw new Exception(ex.Message);
				}
				finally
				{
					if (mySqlDataAdapter != null)
					{
						mySqlDataAdapter.Dispose();
					}
					if (mySqlConnection != null)
					{
						mySqlConnection.Close();
					}
				}
				result = dataSet;
			}
			return result;
		}

		public static DataSet Query(string SQLString, int Times)
		{
			DataSet result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				DataSet dataSet = new DataSet();
				MySqlDataAdapter mySqlDataAdapter = null;
				try
				{
					mySqlConnection.Open();
					mySqlDataAdapter = new MySqlDataAdapter(SQLString, mySqlConnection);
					mySqlDataAdapter.SelectCommand.CommandTimeout = Times;
					mySqlDataAdapter.Fill(dataSet, "ds");
				}
				catch (MySqlException ex)
				{
					throw new Exception(ex.Message);
				}
				finally
				{
					if (mySqlDataAdapter != null)
					{
						mySqlDataAdapter.Dispose();
					}
					if (mySqlConnection != null)
					{
						mySqlConnection.Close();
					}
				}
				result = dataSet;
			}
			return result;
		}

		public static int ExecuteSql(string SQLString, params MySqlParameter[] cmdParms)
		{
			int result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand())
				{
					try
					{
						DbHelperMySQL.PrepareCommand(mySqlCommand, mySqlConnection, null, SQLString, cmdParms);
						int num = mySqlCommand.ExecuteNonQuery();
						mySqlCommand.Parameters.Clear();
						result = num;
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (mySqlCommand != null)
						{
							mySqlCommand.Dispose();
						}
						if (mySqlConnection != null)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
			return result;
		}

		public static void ExecuteSqlTran(Hashtable SQLStringList)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				mySqlConnection.Open();
				using (MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction())
				{
					MySqlCommand mySqlCommand = new MySqlCommand();
					try
					{
						foreach (object obj in SQLStringList)
						{
							DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
							string cmdText = dictionaryEntry.Key.ToString();
							MySqlParameter[] cmdParms = (MySqlParameter[])dictionaryEntry.Value;
							DbHelperMySQL.PrepareCommand(mySqlCommand, mySqlConnection, mySqlTransaction, cmdText, cmdParms);
							int num = mySqlCommand.ExecuteNonQuery();
							mySqlCommand.Parameters.Clear();
						}
						mySqlTransaction.Commit();
					}
					catch
					{
						mySqlTransaction.Rollback();
						throw;
					}
					finally
					{
						if (mySqlCommand != null)
						{
							mySqlCommand.Dispose();
						}
						if (mySqlConnection != null)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
		}

		public static int ExecuteSqlTran(List<CommandInfo> cmdList)
		{
			int result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				mySqlConnection.Open();
				using (MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction())
				{
					MySqlCommand mySqlCommand = new MySqlCommand();
					try
					{
						int num = 0;
						foreach (CommandInfo commandInfo in cmdList)
						{
							string commandText = commandInfo.CommandText;
							MySqlParameter[] cmdParms = (MySqlParameter[])commandInfo.Parameters;
							DbHelperMySQL.PrepareCommand(mySqlCommand, mySqlConnection, mySqlTransaction, commandText, cmdParms);
							if (commandInfo.EffentNextType == EffentNextType.WhenHaveContine || commandInfo.EffentNextType == EffentNextType.WhenNoHaveContine)
							{
								if (commandInfo.CommandText.ToLower().IndexOf("count(") == -1)
								{
									mySqlTransaction.Rollback();
									return 0;
								}
								object obj = mySqlCommand.ExecuteScalar();
								if (obj == null && obj == DBNull.Value)
								{
								}
								bool flag = Convert.ToInt32(obj) > 0;
								if (commandInfo.EffentNextType == EffentNextType.WhenHaveContine && !flag)
								{
									mySqlTransaction.Rollback();
									return 0;
								}
								if (commandInfo.EffentNextType == EffentNextType.WhenNoHaveContine && flag)
								{
									mySqlTransaction.Rollback();
									return 0;
								}
							}
							else
							{
								int num2 = mySqlCommand.ExecuteNonQuery();
								num += num2;
								if (commandInfo.EffentNextType == EffentNextType.ExcuteEffectRows && num2 == 0)
								{
									mySqlTransaction.Rollback();
									return 0;
								}
								mySqlCommand.Parameters.Clear();
							}
						}
						mySqlTransaction.Commit();
						result = num;
					}
					catch
					{
						mySqlTransaction.Rollback();
						throw;
					}
					finally
					{
						if (mySqlCommand != null)
						{
							mySqlCommand.Dispose();
						}
						if (mySqlConnection != null)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
			return result;
		}

		public static void ExecuteSqlTranWithIndentity(List<CommandInfo> SQLStringList)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				mySqlConnection.Open();
				using (MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction())
				{
					MySqlCommand mySqlCommand = new MySqlCommand();
					try
					{
						int num = 0;
						foreach (CommandInfo commandInfo in SQLStringList)
						{
							string commandText = commandInfo.CommandText;
							MySqlParameter[] array = (MySqlParameter[])commandInfo.Parameters;
							foreach (MySqlParameter mySqlParameter in array)
							{
								if (mySqlParameter.Direction == ParameterDirection.InputOutput)
								{
									mySqlParameter.Value = num;
								}
							}
							DbHelperMySQL.PrepareCommand(mySqlCommand, mySqlConnection, mySqlTransaction, commandText, array);
							int num2 = mySqlCommand.ExecuteNonQuery();
							foreach (MySqlParameter mySqlParameter in array)
							{
								if (mySqlParameter.Direction == ParameterDirection.Output)
								{
									num = Convert.ToInt32(mySqlParameter.Value);
								}
							}
							mySqlCommand.Parameters.Clear();
						}
						mySqlTransaction.Commit();
					}
					catch
					{
						mySqlTransaction.Rollback();
						throw;
					}
					finally
					{
						if (mySqlCommand != null)
						{
							mySqlCommand.Dispose();
						}
						if (mySqlConnection != null)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
		}

		public static void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
		{
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				mySqlConnection.Open();
				using (MySqlTransaction mySqlTransaction = mySqlConnection.BeginTransaction())
				{
					MySqlCommand mySqlCommand = new MySqlCommand();
					try
					{
						int num = 0;
						foreach (object obj in SQLStringList)
						{
							DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
							string cmdText = dictionaryEntry.Key.ToString();
							MySqlParameter[] array = (MySqlParameter[])dictionaryEntry.Value;
							foreach (MySqlParameter mySqlParameter in array)
							{
								if (mySqlParameter.Direction == ParameterDirection.InputOutput)
								{
									mySqlParameter.Value = num;
								}
							}
							DbHelperMySQL.PrepareCommand(mySqlCommand, mySqlConnection, mySqlTransaction, cmdText, array);
							int num2 = mySqlCommand.ExecuteNonQuery();
							foreach (MySqlParameter mySqlParameter in array)
							{
								if (mySqlParameter.Direction == ParameterDirection.Output)
								{
									num = Convert.ToInt32(mySqlParameter.Value);
								}
							}
							mySqlCommand.Parameters.Clear();
						}
						mySqlTransaction.Commit();
					}
					catch
					{
						mySqlTransaction.Rollback();
						throw;
					}
					finally
					{
						if (mySqlCommand != null)
						{
							mySqlCommand.Dispose();
						}
						if (mySqlConnection != null)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
		}

		public static object GetSingle(string SQLString, params MySqlParameter[] cmdParms)
		{
			object result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand())
				{
					try
					{
						DbHelperMySQL.PrepareCommand(mySqlCommand, mySqlConnection, null, SQLString, cmdParms);
						object obj = mySqlCommand.ExecuteScalar();
						mySqlCommand.Parameters.Clear();
						if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
						{
							result = null;
						}
						else
						{
							result = obj;
						}
					}
					catch (MySqlException ex)
					{
						throw ex;
					}
					finally
					{
						if (mySqlCommand != null)
						{
							mySqlCommand.Dispose();
						}
						if (mySqlConnection != null)
						{
							mySqlConnection.Close();
						}
					}
				}
			}
			return result;
		}

		public static MySqlDataReader ExecuteReader(string SQLString, params MySqlParameter[] cmdParms)
		{
			MySqlConnection conn = new MySqlConnection(DbHelperMySQL.connectionString);
			MySqlCommand mySqlCommand = new MySqlCommand();
			MySqlDataReader result;
			try
			{
				DbHelperMySQL.PrepareCommand(mySqlCommand, conn, null, SQLString, cmdParms);
				MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
				mySqlCommand.Parameters.Clear();
				result = mySqlDataReader;
			}
			catch (MySqlException ex)
			{
				throw ex;
			}
			return result;
		}

		public static DataSet Query(string SQLString, params MySqlParameter[] cmdParms)
		{
			DataSet result;
			using (MySqlConnection mySqlConnection = new MySqlConnection(DbHelperMySQL.connectionString))
			{
				MySqlCommand mySqlCommand = new MySqlCommand();
				DbHelperMySQL.PrepareCommand(mySqlCommand, mySqlConnection, null, SQLString, cmdParms);
				using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand))
				{
					DataSet dataSet = new DataSet();
					try
					{
						mySqlDataAdapter.Fill(dataSet, "ds");
						mySqlCommand.Parameters.Clear();
					}
					catch (MySqlException ex)
					{
						throw new Exception(ex.Message);
					}
					finally
					{
						if (mySqlCommand != null)
						{
							mySqlCommand.Dispose();
						}
						if (mySqlConnection != null)
						{
							mySqlConnection.Close();
						}
					}
					result = dataSet;
				}
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

		public static string connectionString = PubConstant.ConnectionString;

		public static string connectionLogString = PubConstant.ConnectionLogString;
	}
}
