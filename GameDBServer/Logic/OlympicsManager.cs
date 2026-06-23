using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Logic.Olympics;
using GameDBServer.Server;
using GameDBServer.Tools;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class OlympicsManager : SingletonTemplate<OlympicsManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13124, SingletonTemplate<OlympicsManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13125, SingletonTemplate<OlympicsManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13126, SingletonTemplate<OlympicsManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13127, SingletonTemplate<OlympicsManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13128, SingletonTemplate<OlympicsManager>.Instance());
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			switch (nID)
			{
			case 13124:
				this.OlympicsShopList(client, nID, cmdParams, count);
				break;
			case 13125:
				this.OlympicsShopUpdate(client, nID, cmdParams, count);
				break;
			case 13126:
				this.OlympicsGuess(client, nID, cmdParams, count);
				break;
			case 13127:
				this.OlympicsGuessList(client, nID, cmdParams, count);
				break;
			case 13128:
				this.OlympicsGuessUpdate(client, nID, cmdParams, count);
				break;
			}
		}

		private void OlympicsGuess(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string[] array = null;
			int length = 2;
			if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out array, length))
			{
				client.sendCmd<bool>(nID, false);
			}
			else
			{
				int num = int.Parse(array[0]);
				int num2 = int.Parse(array[1]);
				OlympicsGuessDataDB olympicsGuessDataDB = new OlympicsGuessDataDB();
				olympicsGuessDataDB.RoleID = num;
				olympicsGuessDataDB.DayID = num2;
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("SELECT a1,a2,a3,award1,award2,award3 FROM t_olympics_guess WHERE roleID='{0}' and dayID='{1}' ", num, num2);
					MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
					while (mySQLDataReader.Read())
					{
						olympicsGuessDataDB.A1 = int.Parse(mySQLDataReader["a1"].ToString());
						olympicsGuessDataDB.A2 = int.Parse(mySQLDataReader["a2"].ToString());
						olympicsGuessDataDB.A3 = int.Parse(mySQLDataReader["a3"].ToString());
						olympicsGuessDataDB.Award1 = int.Parse(mySQLDataReader["award1"].ToString());
						olympicsGuessDataDB.Award2 = int.Parse(mySQLDataReader["award2"].ToString());
						olympicsGuessDataDB.Award3 = int.Parse(mySQLDataReader["award3"].ToString());
					}
				}
				client.sendCmd<OlympicsGuessDataDB>(nID, olympicsGuessDataDB);
			}
		}

		private void OlympicsGuessList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int num = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			List<OlympicsGuessDataDB> list = new List<OlympicsGuessDataDB>();
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("SELECT roleID,dayID,a1,a2,a3,award1,award2,award3 FROM t_olympics_guess WHERE roleID={0} ORDER BY dayID ", num);
				MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
				while (mySQLDataReader.Read())
				{
					list.Add(new OlympicsGuessDataDB
					{
						RoleID = int.Parse(mySQLDataReader["roleID"].ToString()),
						DayID = int.Parse(mySQLDataReader["dayID"].ToString()),
						A1 = int.Parse(mySQLDataReader["a1"].ToString()),
						A2 = int.Parse(mySQLDataReader["a2"].ToString()),
						A3 = int.Parse(mySQLDataReader["a3"].ToString()),
						Award1 = int.Parse(mySQLDataReader["award1"].ToString()),
						Award2 = int.Parse(mySQLDataReader["award2"].ToString()),
						Award3 = int.Parse(mySQLDataReader["award3"].ToString())
					});
				}
			}
			client.sendCmd<List<OlympicsGuessDataDB>>(nID, list);
		}

		private void OlympicsGuessUpdate(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			OlympicsGuessDataDB olympicsGuessDataDB = DataHelper.BytesToObject<OlympicsGuessDataDB>(cmdParams, 0, count);
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_olympics_guess(roleID,dayID,a1,a2,a3,award1,award2,award3) \r\n                                            VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')", new object[]
				{
					olympicsGuessDataDB.RoleID,
					olympicsGuessDataDB.DayID,
					olympicsGuessDataDB.A1,
					olympicsGuessDataDB.A2,
					olympicsGuessDataDB.A3,
					olympicsGuessDataDB.Award1,
					olympicsGuessDataDB.Award2,
					olympicsGuessDataDB.Award3
				});
				bool flag = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			client.sendCmd<bool>(nID, true);
		}

		private void OlympicsShopList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int num = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("SELECT shopID,count FROM t_olympics_shop WHERE dayID={0} ORDER BY shopID ", num);
				MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
				while (mySQLDataReader.Read())
				{
					int key = int.Parse(mySQLDataReader["shopID"].ToString());
					int value = int.Parse(mySQLDataReader["count"].ToString());
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, value);
					}
				}
			}
			client.sendCmd<Dictionary<int, int>>(nID, dictionary);
		}

		private void OlympicsShopUpdate(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string[] array = null;
			int length = 3;
			if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out array, length))
			{
				client.sendCmd<bool>(nID, false);
			}
			else
			{
				int num = int.Parse(array[0]);
				int num2 = int.Parse(array[1]);
				int num3 = int.Parse(array[2]);
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("REPLACE INTO t_olympics_shop(dayID,shopID,count) VALUES('{0}', '{1}', '{2}')", num, num2, num3);
					bool flag = myDbConnection.ExecuteNonQueryBool(sql, 0);
				}
				client.sendCmd<bool>(nID, true);
			}
		}
	}
}
