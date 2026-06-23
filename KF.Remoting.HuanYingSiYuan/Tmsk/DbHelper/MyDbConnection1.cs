using System;
using System.Collections.Generic;
using System.Data;
using MySQLDriverCS;
using Server.Tools;

namespace Tmsk.DbHelper
{
	public class MyDbConnection1
	{
		public MyDbConnection1(string connStr, string dbNames)
		{
			bool flag = false;
			MySQLConnection mySQLConnection = null;
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				string[] array = connStr.Split(new char[]
				{
					';'
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						'='
					});
					if (array3.Length == 2)
					{
						array3[0] = array3[0].Trim();
						array3[1] = array3[1].Trim();
						dictionary[array3[0]] = array3[1];
					}
				}
				this.ConnStr = new MySQLConnectionString(dictionary["host"], dictionary["database"], dictionary["user id"], dictionary["password"]);
				mySQLConnection = new MySQLConnection(this.ConnStr.AsString);
				mySQLConnection.Open();
				if (!string.IsNullOrEmpty(dbNames))
				{
					MySQLCommand mySQLCommand = new MySQLCommand(string.Format("SET names '{0}'", dbNames), mySQLConnection);
					mySQLCommand.ExecuteNonQuery();
				}
				this.DatabaseName = this.DbConn.Database;
				flag = true;
				this.DbConn = mySQLConnection;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			if (!flag && null != mySQLConnection)
			{
				try
				{
					mySQLConnection.Close();
				}
				catch
				{
				}
			}
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
				}
				catch
				{
				}
			}
		}

		public void UseDatabase(string databaseKey, string databaseName)
		{
			if (databaseKey != this.DatabaseKey)
			{
				try
				{
					MySQLCommand mySQLCommand = new MySQLCommand(string.Format("use '{0}'", databaseName), this.DbConn);
					mySQLCommand.ExecuteNonQuery();
					mySQLCommand.Dispose();
					this.DatabaseKey = databaseKey;
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
			}
		}

		private MySQLConnection DbConn = null;

		private MySQLConnectionString ConnStr = null;

		public string DatabaseKey = null;

		private string DatabaseName = null;
	}
}
