using System;
using System.Collections.Generic;
using GameDBServer.DB;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic.Name
{
	public class NameUsedMgr : SingletonTemplate<NameUsedMgr>
	{
		private NameUsedMgr()
		{
		}

		public void LoadFromDatabase(DBManager dbMgr)
		{
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT oldname FROM t_change_name", new object[0]);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				try
				{
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					lock (this.cannotUse)
					{
						this.cannotUse.Clear();
						while (mySQLDataReader.Read())
						{
							string text2 = mySQLDataReader["oldname"].ToString();
							if (!string.IsNullOrEmpty(text2) && !this.cannotUse.Contains(text2))
							{
								this.cannotUse.Add(text2);
							}
						}
					}
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查询数据库失败: {0}", text), null, true);
				}
				mySQLCommand.Dispose();
				mySQLCommand = null;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT old_name FROM t_change_name_banghui", new object[0]);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				try
				{
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					lock (this.cannotUse_BangHui)
					{
						this.cannotUse_BangHui.Clear();
						while (mySQLDataReader.Read())
						{
							string text2 = mySQLDataReader["old_name"].ToString();
							if (!string.IsNullOrEmpty(text2) && !this.cannotUse_BangHui.Contains(text2))
							{
								this.cannotUse_BangHui.Add(text2);
							}
						}
					}
				}
				catch (Exception)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查询数据库失败: {0}", text), null, true);
				}
				mySQLCommand.Dispose();
				mySQLCommand = null;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
		}

		public bool AddCannotUse_Ex(string name)
		{
			bool result;
			if (string.IsNullOrEmpty(name))
			{
				result = false;
			}
			else
			{
				lock (this.cannotUse)
				{
					if (this.cannotUse.Contains(name))
					{
						return false;
					}
					this.cannotUse.Add(name);
				}
				result = true;
			}
			return result;
		}

		public bool DelCannotUse_Ex(string name)
		{
			bool result;
			if (string.IsNullOrEmpty(name))
			{
				result = false;
			}
			else
			{
				lock (this.cannotUse)
				{
					if (this.cannotUse.Contains(name))
					{
						this.cannotUse.Remove(name);
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public bool AddCannotUse_BangHui_Ex(string name)
		{
			bool result;
			if (string.IsNullOrEmpty(name))
			{
				result = false;
			}
			else
			{
				lock (this.cannotUse_BangHui)
				{
					if (this.cannotUse_BangHui.Contains(name))
					{
						return false;
					}
					this.cannotUse_BangHui.Add(name);
				}
				result = true;
			}
			return result;
		}

		public bool DelCannotUse_BangHui_Ex(string name)
		{
			bool result;
			if (string.IsNullOrEmpty(name))
			{
				result = false;
			}
			else
			{
				lock (this.cannotUse_BangHui)
				{
					if (this.cannotUse_BangHui.Contains(name))
					{
						this.cannotUse_BangHui.Remove(name);
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		private HashSet<string> cannotUse = new HashSet<string>();

		private HashSet<string> cannotUse_BangHui = new HashSet<string>();
	}
}
