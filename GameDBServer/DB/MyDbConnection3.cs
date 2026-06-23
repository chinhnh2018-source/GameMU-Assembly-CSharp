using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using GameDBServer.Logic;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.DB
{
	public class MyDbConnection3 : IDisposable
	{
		public MyDbConnection3(bool logSelectSqlText = false)
		{
			this._MySQLDataReader = null;
			this.m_logSelectSqlText = logSelectSqlText;
			this.DbConn = DBManager.getInstance().DBConns.PopDBConnection();
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (!this.m_disposed)
			{
				if (isDisposing)
				{
					if (this._MySQLDataReader != null && !this._MySQLDataReader.IsClosed)
					{
						this._MySQLDataReader.Close();
						this._MySQLDataReader = null;
					}
					DBManager.getInstance().DBConns.PushDBConnection(this.DbConn);
				}
				this.m_disposed = true;
			}
		}

		public void Close()
		{
			((IDisposable)this).Dispose();
		}

		private void LogSql(string sqlText)
		{
			if (MyDbConnection3.LogSQLString)
			{
				if (!sqlText.StartsWith("select", StringComparison.OrdinalIgnoreCase) || this.m_logSelectSqlText)
				{
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sqlText), EventLevels.Important);
				}
			}
		}

		public bool ExecuteNonQueryBool(string sql, int commandTimeout = 0)
		{
			return this.ExecuteNonQuery(sql, commandTimeout) >= 0;
		}

		public int ExecuteNonQuery(string sql, int commandTimeout = 0)
		{
			int result = -1;
			try
			{
				using (MySQLCommand mySQLCommand = new MySQLCommand(sql, this.DbConn))
				{
					if (commandTimeout > 0)
					{
						mySQLCommand.CommandTimeout = commandTimeout;
					}
					result = mySQLCommand.ExecuteNonQuery();
					this.LogSql(sql);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
				result = -1;
			}
			return result;
		}

		public int ExecuteWithContent(string sql, string content)
		{
			int result = 0;
			try
			{
				using (MySQLCommand mySQLCommand = new MySQLCommand(sql, this.DbConn))
				{
					MySQLParameter mySQLParameter = new MySQLParameter("@content", content);
					mySQLCommand.Parameters.Add(mySQLParameter);
					result = mySQLCommand.ExecuteNonQuery();
					this.LogSql(sql);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
				result = -1;
			}
			return result;
		}

		public int GetSingleInt(string sql, int commandTimeout = 0, params MySQLParameter[] cmdParms)
		{
			object single = this.GetSingle(sql, commandTimeout, cmdParms);
			int result;
			if (single == null)
			{
				result = 0;
			}
			else
			{
				result = Convert.ToInt32(single.ToString());
			}
			return result;
		}

		public long GetSingleLong(string sql, int commandTimeout = 0, params MySQLParameter[] cmdParms)
		{
			object single = this.GetSingle(sql, commandTimeout, cmdParms);
			long result;
			if (single == null)
			{
				result = 0L;
			}
			else
			{
				result = Convert.ToInt64(single.ToString());
			}
			return result;
		}

		public object GetSingle(string sql, int commandTimeout = 0, params MySQLParameter[] cmdParms)
		{
			try
			{
				using (MySQLCommand mySQLCommand = new MySQLCommand(sql, this.DbConn))
				{
					if (commandTimeout > 0)
					{
						mySQLCommand.CommandTimeout = commandTimeout;
					}
					if (cmdParms.Length > 0)
					{
						MyDbConnection3.PrepareCommand(mySQLCommand, this.DbConn, null, sql, cmdParms);
					}
					object obj = mySQLCommand.ExecuteScalar();
					if (cmdParms.Length > 0)
					{
						mySQLCommand.Parameters.Clear();
					}
					this.LogSql(sql);
					if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
					{
						return null;
					}
					return obj;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
			}
			return null;
		}

		public object ExecuteSqlGet(string sql, string content)
		{
			try
			{
				using (MySQLCommand mySQLCommand = new MySQLCommand(sql, this.DbConn))
				{
					MySQLParameter mySQLParameter = new MySQLParameter("@content", content);
					mySQLCommand.Parameters.Add(mySQLParameter);
					object obj = mySQLCommand.ExecuteScalar();
					this.LogSql(sql);
					if (object.Equals(obj, null) || object.Equals(obj, DBNull.Value))
					{
						return null;
					}
					return obj;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
			}
			return null;
		}

		public MySQLDataReader ExecuteReader(string sql, params MySQLParameter[] cmdParms)
		{
			try
			{
				if (this._MySQLDataReader != null && !this._MySQLDataReader.IsClosed)
				{
					this._MySQLDataReader.Close();
					this._MySQLDataReader = null;
				}
				using (MySQLCommand mySQLCommand = new MySQLCommand(sql, this.DbConn))
				{
					if (cmdParms.Length > 0)
					{
						MyDbConnection3.PrepareCommand(mySQLCommand, this.DbConn, null, sql, cmdParms);
					}
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					if (cmdParms.Length > 0)
					{
						mySQLCommand.Parameters.Clear();
					}
					this._MySQLDataReader = mySQLDataReader;
					this.LogSql(sql);
					return mySQLDataReader;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
			}
			return null;
		}

		public DataSet Query(string sql, int Times = 0)
		{
			DataSet dataSet = new DataSet();
			MySQLDataAdapter mySQLDataAdapter2;
			MySQLDataAdapter mySQLDataAdapter = mySQLDataAdapter2 = new MySQLDataAdapter(sql, this.DbConn);
			try
			{
				if (Times > 0)
				{
					mySQLDataAdapter.SelectCommand.CommandTimeout = Times;
				}
				mySQLDataAdapter.Fill(dataSet, "ds");
				this.LogSql(sql);
			}
			catch (MySQLException ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
			}
			finally
			{
				if (mySQLDataAdapter2 != null)
				{
					mySQLDataAdapter2.Dispose();
				}
			}
			return dataSet;
		}

		public DataSet Query(string sql, params MySQLParameter[] cmdParms)
		{
			DataSet dataSet = new DataSet();
			MySQLCommand mySQLCommand = new MySQLCommand();
			MyDbConnection3.PrepareCommand(mySQLCommand, this.DbConn, null, sql, cmdParms);
			DataSet result;
			using (MySQLDataAdapter mySQLDataAdapter = new MySQLDataAdapter(mySQLCommand))
			{
				try
				{
					mySQLDataAdapter.Fill(dataSet, "ds");
					mySQLCommand.Parameters.Clear();
					this.LogSql(sql);
				}
				catch (MySQLException ex)
				{
					throw new Exception(ex.Message);
				}
				finally
				{
					if (mySQLCommand != null)
					{
						mySQLCommand.Dispose();
					}
				}
				result = dataSet;
			}
			return result;
		}

		public int ExecuteSqlTran(List<string> SQLStringList)
		{
			MySQLConnection dbConn = this.DbConn;
			int result;
			using (MySQLCommand mySQLCommand = new MySQLCommand())
			{
				mySQLCommand.Connection = dbConn;
				DbTransaction dbTransaction = dbConn.BeginTransaction();
				mySQLCommand.Transaction = dbTransaction;
				try
				{
					int num = 0;
					for (int i = 0; i < SQLStringList.Count; i++)
					{
						string text = SQLStringList[i];
						if (text.Trim().Length > 1)
						{
							mySQLCommand.CommandText = text;
							num += mySQLCommand.ExecuteNonQuery();
							this.LogSql(text);
						}
					}
					dbTransaction.Commit();
					result = num;
				}
				catch
				{
					dbTransaction.Rollback();
					result = -1;
				}
			}
			return result;
		}

		public int ExecuteSqlInsertImg(string sql, byte[] content)
		{
			int result = 0;
			try
			{
				using (MySQLCommand mySQLCommand = new MySQLCommand(sql, this.DbConn))
				{
					MySQLParameter mySQLParameter = new MySQLParameter("@content", content);
					mySQLCommand.Parameters.Add(mySQLParameter);
					result = mySQLCommand.ExecuteNonQuery();
					this.LogSql(sql);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
				result = -1;
			}
			return result;
		}

		public int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
		{
			MySQLConnection dbConn = this.DbConn;
			int result;
			using (MySQLCommand mySQLCommand = new MySQLCommand())
			{
				mySQLCommand.Connection = dbConn;
				DbTransaction dbTransaction = dbConn.BeginTransaction();
				mySQLCommand.Transaction = dbTransaction;
				try
				{
					foreach (CommandInfo commandInfo in list)
					{
						string commandText = commandInfo.CommandText;
						MySQLParameter[] parameters = commandInfo.Parameters;
						MyDbConnection3.PrepareCommand(mySQLCommand, dbConn, dbTransaction, commandText, parameters);
						if (commandInfo.EffentNextType == EffentNextType.SolicitationEvent)
						{
							if (commandInfo.CommandText.ToLower().IndexOf("count(") == -1)
							{
								dbTransaction.Rollback();
								throw new Exception("违背要求" + commandInfo.CommandText + "必须符合select count(..的格式");
							}
							object obj = mySQLCommand.ExecuteScalar();
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
								dbTransaction.Rollback();
								throw new Exception("SQL:违背要求" + commandInfo.CommandText + "必须符合select count(..的格式");
							}
							object obj = mySQLCommand.ExecuteScalar();
							if (obj == null && obj == DBNull.Value)
							{
							}
							bool flag = Convert.ToInt32(obj) > 0;
							if (commandInfo.EffentNextType == EffentNextType.WhenHaveContine && !flag)
							{
								dbTransaction.Rollback();
								throw new Exception("SQL:违背要求" + commandInfo.CommandText + "返回值必须大于0");
							}
							if (commandInfo.EffentNextType == EffentNextType.WhenNoHaveContine && flag)
							{
								dbTransaction.Rollback();
								throw new Exception("SQL:违背要求" + commandInfo.CommandText + "返回值必须等于0");
							}
						}
						else
						{
							int num = mySQLCommand.ExecuteNonQuery();
							if (commandInfo.EffentNextType == EffentNextType.ExcuteEffectRows && num == 0)
							{
								dbTransaction.Rollback();
								throw new Exception("SQL:违背要求" + commandInfo.CommandText + "必须有影响行");
							}
							mySQLCommand.Parameters.Clear();
						}
					}
					dbTransaction.Commit();
					result = 1;
				}
				catch (MySQLException ex)
				{
					dbTransaction.Rollback();
					throw ex;
				}
			}
			return result;
		}

		public int ExecuteSql(string sql, params MySQLParameter[] cmdParms)
		{
			int result = 0;
			try
			{
				using (MySQLCommand mySQLCommand = new MySQLCommand(sql, this.DbConn))
				{
					MyDbConnection3.PrepareCommand(mySQLCommand, this.DbConn, null, sql, cmdParms);
					result = mySQLCommand.ExecuteNonQuery();
					mySQLCommand.Parameters.Clear();
					this.LogSql(sql);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("执行SQL异常: {0}\r\n{1}", sql, ex.ToString()), null, true);
				LogManager.WriteLog(LogTypes.Error, string.Format("写入数据库失败: {0}", sql), null, true);
				result = -1;
			}
			return result;
		}

		public void ExecuteSqlTran(Hashtable SQLStringList)
		{
			using (DbTransaction dbTransaction = this.DbConn.BeginTransaction())
			{
				using (MySQLCommand mySQLCommand = new MySQLCommand())
				{
					try
					{
						foreach (object obj in SQLStringList)
						{
							DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
							string text = dictionaryEntry.Key.ToString();
							MySQLParameter[] cmdParms = (MySQLParameter[])dictionaryEntry.Value;
							MyDbConnection3.PrepareCommand(mySQLCommand, this.DbConn, dbTransaction, text, cmdParms);
							int num = mySQLCommand.ExecuteNonQuery();
							mySQLCommand.Parameters.Clear();
							this.LogSql(text);
						}
						dbTransaction.Commit();
					}
					catch
					{
						dbTransaction.Rollback();
						throw;
					}
				}
			}
		}

		public void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
		{
			using (DbTransaction dbTransaction = this.DbConn.BeginTransaction())
			{
				using (MySQLCommand mySQLCommand = new MySQLCommand())
				{
					try
					{
						int num = 0;
						foreach (object obj in SQLStringList)
						{
							DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
							string text = dictionaryEntry.Key.ToString();
							MySQLParameter[] array = (MySQLParameter[])dictionaryEntry.Value;
							foreach (MySQLParameter mySQLParameter in array)
							{
								if (mySQLParameter.Direction == ParameterDirection.InputOutput)
								{
									mySQLParameter.Value = num;
								}
							}
							MyDbConnection3.PrepareCommand(mySQLCommand, this.DbConn, dbTransaction, text, array);
							int num2 = mySQLCommand.ExecuteNonQuery();
							foreach (MySQLParameter mySQLParameter in array)
							{
								if (mySQLParameter.Direction == ParameterDirection.Output)
								{
									num = Convert.ToInt32(mySQLParameter.Value);
								}
							}
							mySQLCommand.Parameters.Clear();
							this.LogSql(text);
						}
						dbTransaction.Commit();
					}
					catch
					{
						dbTransaction.Rollback();
						throw;
					}
				}
			}
		}

		public int ExecuteSqlTran(List<CommandInfo> cmdList)
		{
			int result;
			using (DbTransaction dbTransaction = this.DbConn.BeginTransaction())
			{
				using (MySQLCommand mySQLCommand = new MySQLCommand())
				{
					try
					{
						int num = 0;
						foreach (CommandInfo commandInfo in cmdList)
						{
							string commandText = commandInfo.CommandText;
							MySQLParameter[] parameters = commandInfo.Parameters;
							MyDbConnection3.PrepareCommand(mySQLCommand, this.DbConn, dbTransaction, commandText, parameters);
							if (commandInfo.EffentNextType == EffentNextType.WhenHaveContine || commandInfo.EffentNextType == EffentNextType.WhenNoHaveContine)
							{
								if (commandInfo.CommandText.ToLower().IndexOf("count(") == -1)
								{
									dbTransaction.Rollback();
									return 0;
								}
								object obj = mySQLCommand.ExecuteScalar();
								if (obj == null && obj == DBNull.Value)
								{
								}
								bool flag = Convert.ToInt32(obj) > 0;
								if (commandInfo.EffentNextType == EffentNextType.WhenHaveContine && !flag)
								{
									dbTransaction.Rollback();
									return 0;
								}
								if (commandInfo.EffentNextType == EffentNextType.WhenNoHaveContine && flag)
								{
									dbTransaction.Rollback();
									return 0;
								}
							}
							else
							{
								int num2 = mySQLCommand.ExecuteNonQuery();
								num += num2;
								if (commandInfo.EffentNextType == EffentNextType.ExcuteEffectRows && num2 == 0)
								{
									dbTransaction.Rollback();
									return 0;
								}
								mySQLCommand.Parameters.Clear();
								this.LogSql(commandText);
							}
						}
						dbTransaction.Commit();
						result = num;
					}
					catch
					{
						dbTransaction.Rollback();
						throw;
					}
				}
			}
			return result;
		}

		public void ExecuteSqlTranWithIndentity(List<CommandInfo> SQLStringList)
		{
			using (DbTransaction dbTransaction = this.DbConn.BeginTransaction())
			{
				using (MySQLCommand mySQLCommand = new MySQLCommand())
				{
					try
					{
						int num = 0;
						foreach (CommandInfo commandInfo in SQLStringList)
						{
							string commandText = commandInfo.CommandText;
							MySQLParameter[] parameters = commandInfo.Parameters;
							foreach (MySQLParameter mySQLParameter in parameters)
							{
								if (mySQLParameter.Direction == ParameterDirection.InputOutput)
								{
									mySQLParameter.Value = num;
								}
							}
							MyDbConnection3.PrepareCommand(mySQLCommand, this.DbConn, dbTransaction, commandText, parameters);
							int num2 = mySQLCommand.ExecuteNonQuery();
							foreach (MySQLParameter mySQLParameter in parameters)
							{
								if (mySQLParameter.Direction == ParameterDirection.Output)
								{
									num = Convert.ToInt32(mySQLParameter.Value);
								}
							}
							mySQLCommand.Parameters.Clear();
							this.LogSql(commandText);
						}
						dbTransaction.Commit();
					}
					catch
					{
						dbTransaction.Rollback();
						throw;
					}
				}
			}
		}

		private static void PrepareCommand(MySQLCommand cmd, MySQLConnection conn, DbTransaction trans, string cmdText, MySQLParameter[] cmdParms)
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
				foreach (MySQLParameter mySQLParameter in cmdParms)
				{
					if ((mySQLParameter.Direction == ParameterDirection.InputOutput || mySQLParameter.Direction == ParameterDirection.Input) && mySQLParameter.Value == null)
					{
						mySQLParameter.Value = DBNull.Value;
					}
					cmd.Parameters.Add(mySQLParameter);
				}
			}
		}

		public MySQLConnection DbConn = null;

		private MySQLDataReader _MySQLDataReader;

		public static bool LogSQLString = true;

		private bool m_disposed = false;

		private bool m_logSelectSqlText = false;
	}
}
