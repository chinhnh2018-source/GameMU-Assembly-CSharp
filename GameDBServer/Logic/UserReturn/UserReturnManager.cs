using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using GameDBServer.Tools;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic.UserReturn
{
	public class UserReturnManager : SingletonTemplate<UserReturnManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13100, SingletonTemplate<UserReturnManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13101, SingletonTemplate<UserReturnManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13102, SingletonTemplate<UserReturnManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13103, SingletonTemplate<UserReturnManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13104, SingletonTemplate<UserReturnManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13105, SingletonTemplate<UserReturnManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13106, SingletonTemplate<UserReturnManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13107, SingletonTemplate<UserReturnManager>.Instance());
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
			case 13100:
				this.UserReturnIsOpen(client, nID, cmdParams, count);
				break;
			case 13101:
				this.UserReturnDataGet(client, nID, cmdParams, count);
				break;
			case 13102:
				this.UserReturnDataUpdate(client, nID, cmdParams, count);
				break;
			case 13103:
				this.UserReturnDataDel(client, nID, cmdParams, count);
				break;
			case 13104:
				this.UserReturnDataList(client, nID, cmdParams, count);
				break;
			case 13105:
				this.UserReturnAwardList(client, nID, cmdParams, count);
				break;
			case 13106:
				this.UserReturnAwardUpdate(client, nID, cmdParams, count);
				break;
			case 13107:
				this.UserReturnCheck(client, nID, cmdParams, count);
				break;
			}
		}

		public void UserReturnIsOpen(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			lock (UserReturnManager.Mutex)
			{
				UserReturnManager._activityInfo = DataHelper.BytesToObject<ReturnActivity>(cmdParams, 0, count);
				UserReturnManager._userReturnIsOpen = UserReturnManager._activityInfo.IsOpen;
			}
			client.sendCmd<int>(nID, 1);
		}

		public void UserReturnDataGet(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			ReturnData cmdData = null;
			string[] array = null;
			try
			{
				int length = 2;
				if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out array, length))
				{
					client.sendCmd<ReturnData>(nID, cmdData);
					return;
				}
				string userID = array[0];
				int zoneID = int.Parse(array[1]);
				cmdData = this.GetUserReturnData(userID, zoneID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			client.sendCmd<ReturnData>(nID, cmdData);
		}

		public void UserReturnDataList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			List<ReturnData> list = new List<ReturnData>();
			string[] array = null;
			try
			{
				int length = 4;
				if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out array, length))
				{
					client.sendCmd<List<ReturnData>>(nID, list);
					return;
				}
				string text = array[0];
				int num = int.Parse(array[1]);
				int num2 = int.Parse(array[2]);
				int num3 = int.Parse(array[3]);
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("SELECT id,activityID,activityDay,pzoneID,proleID,czoneID,croleID,vip,`level`,logTime,checkState,logState \r\n                                                        FROM t_user_return_back \r\n                                                        WHERE activityDay='{0}' AND activityID={1} AND pzoneID={2} AND proleID={3} AND logState=0 ", new object[]
					{
						text,
						num,
						num2,
						num3
					});
					MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
					while (mySQLDataReader.Read())
					{
						list.Add(new ReturnData
						{
							DBID = Convert.ToInt32(mySQLDataReader["id"].ToString()),
							ActivityID = Convert.ToInt32(mySQLDataReader["activityID"].ToString()),
							ActivityDay = mySQLDataReader["activityDay"].ToString(),
							PZoneID = Convert.ToInt32(mySQLDataReader["pzoneID"].ToString()),
							PRoleID = Convert.ToInt32(mySQLDataReader["proleID"].ToString()),
							CZoneID = Convert.ToInt32(mySQLDataReader["czoneID"].ToString()),
							CRoleID = Convert.ToInt32(mySQLDataReader["croleID"].ToString()),
							Vip = Convert.ToInt32(mySQLDataReader["vip"].ToString()),
							Level = Convert.ToInt32(mySQLDataReader["level"].ToString()),
							LogTime = Convert.ToDateTime(mySQLDataReader["logTime"].ToString()),
							StateCheck = Convert.ToInt32(mySQLDataReader["checkState"].ToString()),
							StateLog = Convert.ToInt32(mySQLDataReader["logState"].ToString())
						});
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			client.sendCmd<List<ReturnData>>(nID, list);
		}

		public void UserReturnDataUpdate(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool cmdData = false;
			try
			{
				ReturnData data = DataHelper.BytesToObject<ReturnData>(cmdParams, 0, count);
				this.UpdateUserReturnData(data);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			client.sendCmd<bool>(nID, cmdData);
		}

		public void UserReturnDataDel(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool cmdData = false;
			try
			{
				ReturnData returnData = DataHelper.BytesToObject<ReturnData>(cmdParams, 0, count);
				cmdData = this.DelUserReturnData(returnData.DBID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			client.sendCmd<bool>(nID, cmdData);
		}

		public void UserReturnAwardList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			Dictionary<int, int[]> dictionary = new Dictionary<int, int[]>();
			string[] array = null;
			try
			{
				int length = 3;
				if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out array, length))
				{
					client.sendCmd<Dictionary<int, int[]>>(nID, dictionary);
					return;
				}
				string arg = array[0];
				int num = int.Parse(array[1]);
				string arg2 = array[2];
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("SELECT type,state FROM t_user_return_award WHERE activityDay = '{0}' AND activityID = '{1}' AND userid='{2}'", arg, num, arg2);
					MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
					while (mySQLDataReader.Read())
					{
						int key = Convert.ToInt32(mySQLDataReader["type"].ToString());
						string[] array2 = mySQLDataReader["state"].ToString().Split(new char[]
						{
							'*'
						});
						List<int> list = new List<int>();
						foreach (string value in array2)
						{
							list.Add(Convert.ToInt32(value));
						}
						if (!dictionary.ContainsKey(key))
						{
							dictionary.Add(key, list.ToArray());
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			client.sendCmd<Dictionary<int, int[]>>(nID, dictionary);
		}

		public void UserReturnAwardUpdate(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool cmdData = false;
			string[] array = null;
			try
			{
				int length = 6;
				if (!CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out array, length))
				{
					client.sendCmd<bool>(nID, cmdData);
					return;
				}
				string text = array[0];
				int num = int.Parse(array[1]);
				int num2 = int.Parse(array[2]);
				string text2 = array[3];
				int num3 = int.Parse(array[4]);
				string text3 = array[5];
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("REPLACE INTO t_user_return_award (activityID,activityDay,zoneID,userid,type,state) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", new object[]
					{
						num,
						text,
						num2,
						text2,
						num3,
						text3
					});
					cmdData = myDbConnection.ExecuteNonQueryBool(sql, 0);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			client.sendCmd<bool>(nID, cmdData);
		}

		public void UserReturnCheck(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int num = 0;
			string[] array = null;
			try
			{
				if (CheckHelper.CheckTCPCmdFields(nID, cmdParams, count, out array, 3))
				{
					if (UserReturnManager._activityInfo.IsOpen)
					{
						string text = array[0];
						string text2 = array[1];
						string text3 = array[2];
						int num2 = 0;
						using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
						{
							string sql = string.Format("select * from t_user_return where userid='{0}' and activityDay='{1}' and activityID='{2}' and zoneID='{3}'", new object[]
							{
								text,
								UserReturnManager._activityInfo.ActivityDay,
								UserReturnManager._activityInfo.ActivityID,
								text2
							});
							if (null == myDbConnection.GetSingle(sql, 0, new MySQLParameter[0]))
							{
								int userInputMoney = DBQuery.GetUserInputMoney(TCPManager.getInstance().DBMgr, text, 0, "2000-01-01 00:00:00", "2050-01-01 00:00:00");
								int num3 = Global.TransMoneyToYuanBao(userInputMoney);
								if (UserReturnManager._activityInfo.VIPNeedExp <= num3)
								{
									sql = string.Format("select regtime from t_roles where userid='{0}' and regtime<'{1}'", text, UserReturnManager._activityInfo.NotLoggedInBegin);
									if (null != myDbConnection.GetSingle(sql, 0, new MySQLParameter[0]))
									{
										sql = string.Format("select dayid from t_login where userid='{0}' and dayid>={1} and dayid<={2}", text, Global.GetOffsetDay(UserReturnManager._activityInfo.NotLoggedInBegin), Global.GetOffsetDay(UserReturnManager._activityInfo.NotLoggedInFinish));
										if (null == myDbConnection.GetSingle(sql, 0, new MySQLParameter[0]))
										{
											sql = string.Format("select changelifecount*100+level as l from t_roles where userid='{0}' and zoneid='{1}' and isdel=0 order by l desc limit 1", text, text2);
											num2 = myDbConnection.GetSingleInt(sql, 0, new MySQLParameter[0]);
											if (UserReturnManager._activityInfo.Level <= num2)
											{
												num = 1;
											}
										}
									}
								}
								sql = string.Format("insert into t_user_return values (0,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}' )", new object[]
								{
									UserReturnManager._activityInfo.ActivityID,
									UserReturnManager._activityInfo.ActivityDay,
									text2,
									text,
									UserReturnManager._activityInfo.VIPNeedExp,
									num2,
									text3,
									num
								});
								myDbConnection.ExecuteNonQuery(sql, 0);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			client.sendCmd<int>(nID, 1);
		}

		public ReturnData GetUserReturnData(string userID, int zoneID)
		{
			ReturnData result = null;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("select vip,level,checkState,logTime from t_user_return where userid='{0}' and zoneID='{1}' and activityDay='{2}' and activityID='{3}'", new object[]
				{
					userID,
					zoneID,
					UserReturnManager._activityInfo.ActivityDay,
					UserReturnManager._activityInfo.ActivityID
				});
				MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
				if (mySQLDataReader.Read())
				{
					int num = DBQuery.GetUserInputMoney(TCPManager.getInstance().DBMgr, userID, 0, UserReturnManager._activityInfo.ActivityDay, "2050-01-01 00:00:00");
					if (num < 0)
					{
						num = 0;
					}
					int leiJiChongZhi = Global.TransMoneyToYuanBao(num);
					result = new ReturnData
					{
						Vip = Convert.ToInt32(mySQLDataReader["vip"].ToString()),
						Level = Convert.ToInt32(mySQLDataReader["level"].ToString()),
						StateCheck = Convert.ToInt32(mySQLDataReader["checkState"].ToString()),
						LogTime = DateTime.Parse(mySQLDataReader["logTime"].ToString()),
						LeiJiChongZhi = leiJiChongZhi
					};
				}
			}
			return result;
		}

		public bool UpdateUserReturnData(ReturnData data)
		{
			bool result;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_user_return_back(activityID,activityDay,pzoneID,proleID,czoneID,croleID,vip,`level`,logTime,checkState,logState) \r\n                                                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}','{6}','{7}','{8}','{9}','{10}')", new object[]
				{
					data.ActivityID,
					data.ActivityDay,
					data.PZoneID,
					data.PRoleID,
					data.CZoneID,
					data.CRoleID,
					data.Vip,
					data.Level,
					data.LogTime,
					data.StateCheck,
					data.StateLog
				});
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public bool DelUserReturnData(int id)
		{
			bool result;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE FROM t_user_return_back WHERE id={0}", id);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		public void ScanLastUserReturn(DBManager dbMgr)
		{
		}

		public List<ReturnData> ReturnList()
		{
			List<ReturnData> list = new List<ReturnData>();
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("SELECT id,activityID,activityDay,pzoneID,proleID,czoneID,croleID,vip,`level`,logTime,checkState,logState FROM t_user_return order by logTime", new object[0]);
				MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
				while (mySQLDataReader.Read())
				{
					list.Add(new ReturnData
					{
						DBID = Convert.ToInt32(mySQLDataReader["id"].ToString()),
						ActivityID = Convert.ToInt32(mySQLDataReader["activityID"].ToString()),
						ActivityDay = mySQLDataReader["activityDay"].ToString(),
						PZoneID = Convert.ToInt32(mySQLDataReader["pzoneID"].ToString()),
						PRoleID = Convert.ToInt32(mySQLDataReader["proleID"].ToString()),
						CZoneID = Convert.ToInt32(mySQLDataReader["czoneID"].ToString()),
						CRoleID = Convert.ToInt32(mySQLDataReader["croleID"].ToString()),
						Vip = Convert.ToInt32(mySQLDataReader["vip"].ToString()),
						Level = Convert.ToInt32(mySQLDataReader["level"].ToString()),
						LogTime = Convert.ToDateTime(mySQLDataReader["logTime"].ToString()),
						StateCheck = Convert.ToInt32(mySQLDataReader["checkState"].ToString()),
						StateLog = Convert.ToInt32(mySQLDataReader["logState"].ToString())
					});
				}
			}
			return list;
		}

		public bool ReturnDel(int dbID)
		{
			bool result;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE FROM t_user_return WHERE id={0}", dbID);
				result = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			return result;
		}

		private static object Mutex = new object();

		private static bool _userReturnIsOpen = false;

		private static ReturnActivity _activityInfo = new ReturnActivity
		{
			IsOpen = false
		};

		private long LastScanTicks = DateTime.Now.Ticks / 10000L;
	}
}
