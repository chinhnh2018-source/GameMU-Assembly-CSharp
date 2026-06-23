using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MySQLDriverCS;

namespace Maticsoft.DBUtility
{
	public abstract class DbHelperMySQL2
	{
		public DbHelperMySQL2()
		{
		}

		public static void LoadConnectStr()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] array = DbHelperMySQL2.connectionLogString.Split(new char[]
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
			MySQLConnectionString mySQLConnectionString = new MySQLConnectionString(dictionary["host"], dictionary["database"], dictionary["user id"], dictionary["password"]);
			dictionary.Clear();
			array = DbHelperMySQL2.connectionString.Split(new char[]
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
			MySQLConnectionString mySQLConnectionString2 = new MySQLConnectionString(dictionary["host"], dictionary["database"], dictionary["user id"], dictionary["password"]);
			DbHelperMySQL2.realConnStrGame = mySQLConnectionString2.AsString;
			DbHelperMySQL2.realConnStrLog = mySQLConnectionString.AsString;
			DbHelperMySQL2.loadConnectStr = true;
		}

		public static MySQLDataReader ExecuteReader(string strSQL, bool islog = false)
		{
			if (!DbHelperMySQL2.loadConnectStr)
			{
				DbHelperMySQL2.LoadConnectStr();
			}
			MySQLConnection mySQLConnection = null;
			if (islog)
			{
				mySQLConnection = new MySQLConnection(DbHelperMySQL2.realConnStrLog);
			}
			else
			{
				mySQLConnection = new MySQLConnection(DbHelperMySQL2.realConnStrGame);
			}
			MySQLCommand mySQLCommand = new MySQLCommand(strSQL, mySQLConnection);
			MySQLCommand mySQLCommand2 = new MySQLCommand("SET NAMES 'latin1'", mySQLConnection);
			MySQLDataReader result;
			try
			{
				mySQLConnection.Open();
				int num = mySQLCommand2.ExecuteNonQuery();
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				result = mySQLDataReader;
			}
			catch (MySqlException ex)
			{
				throw ex;
			}
			finally
			{
				if (mySQLCommand != null)
				{
					mySQLCommand.Dispose();
				}
				if (mySQLCommand2 != null)
				{
					mySQLCommand2.Dispose();
				}
				if (mySQLConnection != null)
				{
					mySQLConnection.Dispose();
					mySQLConnection.Close();
				}
			}
			return result;
		}

		public static string connectionString = PubConstant.ConnectionString;

		public static string connectionLogString = PubConstant.ConnectionLogString;

		public static bool loadConnectStr = false;

		public static string realConnStrGame = "";

		public static string realConnStrLog = "";
	}
}
