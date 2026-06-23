using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace Tmsk.DbHelper
{
	public class MyDbConnection2
	{
		public MyDbConnection2(string connStr, string pageCodeNames)
		{
			this.ConnStr = connStr;
			this.PageCodeNames = pageCodeNames;
		}

		public bool Open()
		{
			bool flag = false;
			MySqlConnection mySqlConnection = null;
			try
			{
				mySqlConnection = new MySqlConnection(this.ConnStr);
				mySqlConnection.Open();
				this.DatabaseName = mySqlConnection.Database;
				flag = true;
				this.DbConn = mySqlConnection;
				if (!string.IsNullOrEmpty(this.PageCodeNames))
				{
					this.ExecuteNonQuery(string.Format("SET names '{0}'", this.PageCodeNames), 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			if (!flag && null != mySqlConnection)
			{
				try
				{
					mySqlConnection.Close();
				}
				catch
				{
				}
			}
			return flag;
		}

		public bool IsConnected()
		{
			if (null != this.DbConn)
			{
				if (this.DbConn.State != ConnectionState.Closed && ConnectionState.Closed == (this.DbConn.State & ConnectionState.Broken))
				{
					return true;
				}
			}
			return false;
		}

		public void Close()
		{
			if (null != this.DbConn)
			{
				try
				{
					this.DbConn.Close();
					this.DbConn.Dispose();
				}
				catch
				{
				}
			}
		}

		public void UseDatabase(string databaseKey, string databaseName)
		{
			if (databaseKey != this.DatabaseKey && !string.IsNullOrEmpty(databaseName))
			{
				this.DbConn.ChangeDatabase(databaseName);
				this.DatabaseKey = databaseKey;
			}
		}

		public int ExecuteNonQuery(string sql, int commandTimeout = 0)
		{
			int result = -1;
			try
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(sql, this.DbConn))
				{
					if (commandTimeout > 0)
					{
						mySqlCommand.CommandTimeout = commandTimeout;
					}
					result = mySqlCommand.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(sql + "\r\n" + ex.ToString());
			}
			return result;
		}

		public int ExecuteWithContent(string sql, string content)
		{
			int result = 0;
			try
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(sql, this.DbConn))
				{
					MySqlParameter mySqlParameter = new MySqlParameter("@content", content);
					mySqlCommand.Parameters.Add(mySqlParameter);
					result = mySqlCommand.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(sql + "\r\n" + ex.ToString());
			}
			return result;
		}

		public object GetSingle(string sql, int commandTimeout = 0, params MySqlParameter[] cmdParms)
		{
			try
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(sql, this.DbConn))
				{
					if (commandTimeout > 0)
					{
						mySqlCommand.CommandTimeout = commandTimeout;
					}
					if (cmdParms.Length > 0)
					{
						MyDbConnection2.PrepareCommand(mySqlCommand, this.DbConn, null, sql, cmdParms);
					}
					object obj = mySqlCommand.ExecuteScalar();
					if (cmdParms.Length > 0)
					{
						mySqlCommand.Parameters.Clear();
					}
					if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
					{
						return null;
					}
					return obj;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(sql + "\r\n" + ex.ToString());
			}
			return null;
		}

		public object ExecuteSqlGet(string sql, string content)
		{
			try
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(sql, this.DbConn))
				{
					MySqlParameter mySqlParameter = new MySqlParameter("@content", content);
					mySqlCommand.Parameters.Add(mySqlParameter);
					object obj = mySqlCommand.ExecuteScalar();
					if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
					{
						return null;
					}
					return obj;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(sql + "\r\n" + ex.ToString());
			}
			return null;
		}

		public MySqlDataReader ExecuteReader(string sql, params MySqlParameter[] cmdParms)
		{
			try
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(sql, this.DbConn))
				{
					if (cmdParms.Length > 0)
					{
						MyDbConnection2.PrepareCommand(mySqlCommand, this.DbConn, null, sql, cmdParms);
					}
					MySqlDataReader result = mySqlCommand.ExecuteReader();
					if (cmdParms.Length > 0)
					{
						mySqlCommand.Parameters.Clear();
					}
					return result;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(sql + "\r\n" + ex.ToString());
			}
			return null;
		}

		public DataSet Query(string SQLString, int Times = 0)
		{
			DataSet dataSet = new DataSet();
			MySqlDataAdapter mySqlDataAdapter2;
			MySqlDataAdapter mySqlDataAdapter = mySqlDataAdapter2 = new MySqlDataAdapter(SQLString, this.DbConn);
			try
			{
				if (Times > 0)
				{
					mySqlDataAdapter.SelectCommand.CommandTimeout = Times;
				}
				mySqlDataAdapter.Fill(dataSet, "ds");
			}
			catch (MySqlException ex)
			{
				throw new Exception(SQLString + "\r\n" + ex.Message);
			}
			finally
			{
				if (mySqlDataAdapter2 != null)
				{
					mySqlDataAdapter2.Dispose();
				}
			}
			return dataSet;
		}

		public DataSet Query(string SQLString, params MySqlParameter[] cmdParms)
		{
			DataSet dataSet = new DataSet();
			MySqlCommand mySqlCommand = new MySqlCommand();
			MyDbConnection2.PrepareCommand(mySqlCommand, this.DbConn, null, SQLString, cmdParms);
			DataSet result;
			using (MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand))
			{
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
				}
				result = dataSet;
			}
			return result;
		}

		public int ExecuteSqlTran(List<string> SQLStringList)
		{
			MySqlConnection dbConn = this.DbConn;
			int result;
			using (MySqlCommand mySqlCommand = new MySqlCommand())
			{
				mySqlCommand.Connection = dbConn;
				MySqlTransaction mySqlTransaction = dbConn.BeginTransaction();
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
			}
			return result;
		}

		public int ExecuteSqlInsertImg(string strSQL, List<Tuple<string, byte[]>> imgList)
		{
			int result = 0;
			try
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(strSQL, this.DbConn))
				{
					foreach (Tuple<string, byte[]> tuple in imgList)
					{
						string item = tuple.Item1;
						byte[] item2 = tuple.Item2;
						MySqlParameter mySqlParameter = new MySqlParameter(item, item2);
						mySqlCommand.Parameters.Add(mySqlParameter);
					}
					result = mySqlCommand.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(strSQL + "\r\n" + ex.ToString());
			}
			return result;
		}

		public int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
		{
			MySqlConnection dbConn = this.DbConn;
			int result;
			using (MySqlCommand mySqlCommand = new MySqlCommand())
			{
				mySqlCommand.Connection = dbConn;
				MySqlTransaction mySqlTransaction = dbConn.BeginTransaction();
				mySqlCommand.Transaction = mySqlTransaction;
				try
				{
					foreach (CommandInfo commandInfo in list)
					{
						string commandText = commandInfo.CommandText;
						MySqlParameter[] cmdParms = (MySqlParameter[])commandInfo.Parameters;
						MyDbConnection2.PrepareCommand(mySqlCommand, dbConn, mySqlTransaction, commandText, cmdParms);
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
					string connectionString = PubConstant.GetConnectionString("ConnectionStringPPC");
					if (!OracleHelper.ExecuteSqlTran(connectionString, oracleCmdSqlList))
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
			}
			return result;
		}

		public int ExecuteSql(string sql, params MySqlParameter[] cmdParms)
		{
			int result = 0;
			try
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand(sql, this.DbConn))
				{
					MyDbConnection2.PrepareCommand(mySqlCommand, this.DbConn, null, sql, cmdParms);
					result = mySqlCommand.ExecuteNonQuery();
					mySqlCommand.Parameters.Clear();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(sql + "\r\n" + ex.ToString());
			}
			return result;
		}

		public void ExecuteSqlTran(Hashtable SQLStringList)
		{
			using (MySqlTransaction mySqlTransaction = this.DbConn.BeginTransaction())
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand())
				{
					try
					{
						foreach (object obj in SQLStringList)
						{
							DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
							string cmdText = dictionaryEntry.Key.ToString();
							MySqlParameter[] cmdParms = (MySqlParameter[])dictionaryEntry.Value;
							MyDbConnection2.PrepareCommand(mySqlCommand, this.DbConn, mySqlTransaction, cmdText, cmdParms);
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
				}
			}
		}

		public void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
		{
			using (MySqlTransaction mySqlTransaction = this.DbConn.BeginTransaction())
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand())
				{
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
							MyDbConnection2.PrepareCommand(mySqlCommand, this.DbConn, mySqlTransaction, cmdText, array);
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
				}
			}
		}

		public int ExecuteSqlTran(List<CommandInfo> cmdList)
		{
			int result;
			using (MySqlTransaction mySqlTransaction = this.DbConn.BeginTransaction())
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand())
				{
					try
					{
						int num = 0;
						foreach (CommandInfo commandInfo in cmdList)
						{
							string commandText = commandInfo.CommandText;
							MySqlParameter[] cmdParms = (MySqlParameter[])commandInfo.Parameters;
							MyDbConnection2.PrepareCommand(mySqlCommand, this.DbConn, mySqlTransaction, commandText, cmdParms);
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
				}
			}
			return result;
		}

		public void ExecuteSqlTranWithIndentity(List<CommandInfo> SQLStringList)
		{
			using (MySqlTransaction mySqlTransaction = this.DbConn.BeginTransaction())
			{
				using (MySqlCommand mySqlCommand = new MySqlCommand())
				{
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
							MyDbConnection2.PrepareCommand(mySqlCommand, this.DbConn, mySqlTransaction, commandText, array);
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
				}
			}
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

		public MySqlConnection DbConn = null;

		public string DatabaseKey = null;

		private string DatabaseName = null;

		private string ConnStr;

		private string PageCodeNames;
	}
}
