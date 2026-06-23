using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using GameDBServer.Logic;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.DB.DBController
{
	public abstract class DBController<T>
	{
		protected DBController()
		{
			this.mapper = new DBMapper(typeof(T));
		}

		protected T queryForObject(string sql)
		{
			MySQLConnection mySQLConnection = null;
			T t = default(T);
			try
			{
				mySQLConnection = this.dbMgr.DBConns.PopDBConnection();
				MySQLCommand mySQLCommand = new MySQLCommand(sql, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int fieldCount = mySQLDataReader.FieldCount;
				if (mySQLDataReader.Read())
				{
					int num = 0;
					for (int i = 0; i < fieldCount; i++)
					{
						int ordinal = num++;
						string name = mySQLDataReader.GetName(ordinal);
						object value = mySQLDataReader.GetValue(ordinal);
						if (null == t)
						{
							t = Activator.CreateInstance<T>();
						}
						this.setValue(t, name, value);
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("查询数据库失败: {0}", sql), null, true);
				return default(T);
			}
			finally
			{
				if (null != mySQLConnection)
				{
					this.dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return t;
		}

		protected List<T> queryForList(string sql)
		{
			MySQLConnection mySQLConnection = null;
			List<T> list = null;
			try
			{
				mySQLConnection = this.dbMgr.DBConns.PopDBConnection();
				MySQLCommand mySQLCommand = new MySQLCommand(sql, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int fieldCount = mySQLDataReader.FieldCount;
				string[] array = new string[fieldCount];
				while (mySQLDataReader.Read())
				{
					int num = 0;
					T t = Activator.CreateInstance<T>();
					for (int i = 0; i < fieldCount; i++)
					{
						int num2 = num++;
						if (null == array[num2])
						{
							array[num2] = mySQLDataReader.GetName(num2);
						}
						string columnName = array[num2];
						object value = mySQLDataReader.GetValue(num2);
						if (null == list)
						{
							list = new List<T>();
						}
						this.setValue(t, columnName, value);
					}
					list.Add(t);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			catch (Exception arg)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("查询数据库失败: {0},exception:{1}", sql, arg), null, true);
				return null;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					this.dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		protected Dictionary<object, T> queryForDictionary(string sql, string keyName)
		{
			MySQLConnection mySQLConnection = null;
			Dictionary<object, T> dictionary = null;
			try
			{
				mySQLConnection = this.dbMgr.DBConns.PopDBConnection();
				MySQLCommand mySQLCommand = new MySQLCommand(sql, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int fieldCount = mySQLDataReader.FieldCount;
				dictionary = new Dictionary<object, T>();
				string[] array = new string[fieldCount];
				while (mySQLDataReader.Read())
				{
					int num = 0;
					T t = Activator.CreateInstance<T>();
					object obj = null;
					for (int i = 0; i < fieldCount; i++)
					{
						int num2 = num++;
						if (null == array[num2])
						{
							array[num2] = mySQLDataReader.GetName(num2);
						}
						string columnName = array[num2];
						object value = mySQLDataReader.GetValue(num2);
						this.setValue(t, columnName, value);
						if (null != obj)
						{
							if (keyName.Equals(mySQLDataReader.GetName(num2)) || keyName == mySQLDataReader.GetName(num2))
							{
								obj = mySQLDataReader.GetValue(num2);
							}
						}
					}
					if (null != obj)
					{
						dictionary.Add(obj, t);
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("查询数据库失败: {0}", sql), null, true);
				return null;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					this.dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return dictionary;
		}

		private void setValue(object obj, string columnName, object columnValue)
		{
			MemberInfo memberInfo = this.mapper.getMemberInfo(columnName);
			if (!(null == memberInfo))
			{
				MemberTypes memberType = memberInfo.MemberType;
				if (memberType != MemberTypes.Field)
				{
					if (memberType == MemberTypes.Property)
					{
						PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
						if (columnValue.GetType().Equals(typeof(DBNull)))
						{
							if (propertyInfo.PropertyType.Equals(typeof(string)))
							{
								propertyInfo.SetValue(obj, "", null);
							}
						}
						else
						{
							if (propertyInfo.PropertyType.Equals(typeof(long)) && columnValue.GetType().Equals(typeof(string)))
							{
								columnValue = Convert.ToInt64(columnValue);
							}
							if (propertyInfo.PropertyType.Equals(typeof(byte)) && (columnValue.GetType().Equals(typeof(string)) || columnValue.GetType().Equals(typeof(short)) || columnValue.GetType().Equals(typeof(int)) || columnValue.GetType().Equals(typeof(long))))
							{
								columnValue = Convert.ToByte(columnValue);
							}
							if (propertyInfo.PropertyType.Equals(typeof(string)) && columnValue.GetType().Equals(typeof(byte[])))
							{
								columnValue = Convert.ToString(columnValue);
							}
							propertyInfo.SetValue(obj, columnValue, null);
						}
					}
				}
				else
				{
					FieldInfo fieldInfo = (FieldInfo)memberInfo;
					if (columnValue.GetType().Equals(typeof(DBNull)))
					{
						if (fieldInfo.FieldType.Equals(typeof(string)))
						{
							fieldInfo.SetValue(obj, "");
						}
					}
					else
					{
						if (fieldInfo.FieldType.Equals(typeof(long)) && columnValue.GetType().Equals(typeof(string)))
						{
							columnValue = Convert.ToInt64(columnValue);
						}
						if (fieldInfo.FieldType.Equals(typeof(byte)) && (columnValue.GetType().Equals(typeof(string)) || columnValue.GetType().Equals(typeof(short)) || columnValue.GetType().Equals(typeof(int)) || columnValue.GetType().Equals(typeof(long))))
						{
							columnValue = Convert.ToByte(columnValue);
						}
						if (fieldInfo.FieldType.Equals(typeof(string)) && (columnValue.GetType().Equals(typeof(byte[])) || columnValue.GetType().Equals(typeof(byte[]))))
						{
							byte[] array = (byte[])columnValue;
							columnValue = Convert.ToString(new UTF8Encoding().GetString(array, 0, array.Length));
						}
						fieldInfo.SetValue(obj, columnValue);
					}
				}
			}
		}

		protected int insert(string sql)
		{
			MySQLConnection mySQLConnection = null;
			int result = -1;
			try
			{
				mySQLConnection = this.dbMgr.DBConns.PopDBConnection();
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(sql, mySQLConnection);
				try
				{
					result = mySQLCommand.ExecuteNonQuery();
				}
				catch (Exception arg)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("向数据库写入数据失败: {0},{1}", sql, arg), null, true);
				}
				mySQLCommand.Dispose();
				mySQLCommand = null;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					this.dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		protected int update(string sql)
		{
			MySQLConnection mySQLConnection = null;
			int result = -1;
			try
			{
				mySQLConnection = this.dbMgr.DBConns.PopDBConnection();
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(sql, mySQLConnection);
				try
				{
					result = mySQLCommand.ExecuteNonQuery();
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("向数据库更新数据失败: {0}", sql), null, true);
				}
				mySQLCommand.Dispose();
				mySQLCommand = null;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					this.dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		protected int delete(string sql)
		{
			MySQLConnection mySQLConnection = null;
			int result = -1;
			try
			{
				mySQLConnection = this.dbMgr.DBConns.PopDBConnection();
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(sql, mySQLConnection);
				try
				{
					result = mySQLCommand.ExecuteNonQuery();
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("向数据库删除数据失败: {0}", sql), null, true);
				}
				mySQLCommand.Dispose();
				mySQLCommand = null;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					this.dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		protected int delete(string sql, object[] param)
		{
			MySQLConnection mySQLConnection = null;
			int result = -1;
			try
			{
				mySQLConnection = this.dbMgr.DBConns.PopDBConnection();
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand();
				mySQLCommand.Connection = mySQLConnection;
				mySQLCommand.CommandType = CommandType.Text;
				mySQLCommand.CommandText = sql;
				try
				{
					result = mySQLCommand.ExecuteNonQuery();
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("向数据库删除数据失败: {0}", sql), null, true);
				}
				mySQLCommand.Dispose();
				mySQLCommand = null;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					this.dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		protected DBManager dbMgr = DBManager.getInstance();

		private DBMapper mapper = null;
	}
}
