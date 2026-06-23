using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using GameDBServer.Core;
using GameDBServer.Data;
using GameDBServer.Logic;
using GameDBServer.Logic.Ten;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB
{
	public class DBQuery
	{
		public static bool GetFriendData(DBManager dbMgr, FriendData friendData)
		{
			DBRoleInfo dbroleInfo = new DBRoleInfo();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT rname, level, occupation, position, changelifecount, combatforce, zoneid,zhanduiid \r\n                                                FROM t_roles WHERE isdel=0 AND rid={0}", friendData.OtherRoleID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					dbroleInfo.RoleID = friendData.OtherRoleID;
					dbroleInfo.RoleName = mySQLDataReader["rname"].ToString();
					dbroleInfo.Level = Convert.ToInt32(mySQLDataReader["level"].ToString());
					dbroleInfo.Occupation = Convert.ToInt32(mySQLDataReader["occupation"].ToString());
					dbroleInfo.Position = mySQLDataReader["position"].ToString();
					dbroleInfo.ChangeLifeCount = Convert.ToInt32(mySQLDataReader["changelifecount"].ToString());
					dbroleInfo.CombatForce = Convert.ToInt32(mySQLDataReader["combatforce"].ToString());
					dbroleInfo.ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString());
					dbroleInfo.ZhanDuiID = Convert.ToInt32(mySQLDataReader["zhanduiid"].ToString());
					MySQLConnection mySQLConnection2 = mySQLConnection;
					string[] array = new string[]
					{
						"spouseid",
						"marrytype",
						"ringid",
						"goodwillexp",
						"goodwillstar",
						"goodwilllevel",
						"givenrose",
						"lovemessage",
						"autoreject",
						"changtime"
					};
					string[] array2 = new string[]
					{
						"t_marry"
					};
					object[,] array3 = new object[1, 3];
					array3[0, 0] = "roleid";
					array3[0, 1] = "=";
					array3[0, 2] = friendData.OtherRoleID;
					MySQLSelectCommand cmd = new MySQLSelectCommand(mySQLConnection2, array, array2, array3, null, null);
					DBRoleInfo.DBTableRow2RoleInfo_MarriageData(dbroleInfo, cmd);
					friendData.OtherRoleName = Global.FormatRoleName(dbroleInfo);
					friendData.OtherLevel = dbroleInfo.Level;
					friendData.Occupation = dbroleInfo.Occupation;
					friendData.OnlineState = Global.GetRoleOnlineState(dbroleInfo);
					friendData.Position = dbroleInfo.Position;
					friendData.FriendChangeLifeLev = dbroleInfo.ChangeLifeCount;
					friendData.FriendCombatForce = dbroleInfo.CombatForce;
					friendData.SpouseId = ((dbroleInfo.MyMarriageData != null) ? dbroleInfo.MyMarriageData.nSpouseID : 0);
					friendData.ZhanDuiID = dbroleInfo.ZhanDuiID;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return dbroleInfo.RoleID > 0;
		}

		public static void QueryDJPointData(DBManager dbMgr, DBRoleInfo dbRoleInfo)
		{
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string arg = string.Format("SELECT Id, rid, djpoint, total, wincnt, yestoday, lastweek, lastmonth, dayupdown, weekupdown, monthupdown FROM t_djpoints WHERE rid={0}", dbRoleInfo.RoleID);
				MySQLConnection mySQLConnection2 = mySQLConnection;
				string[] array = new string[]
				{
					"Id",
					"djpoint",
					"total",
					"wincnt",
					"yestoday",
					"lastweek",
					"lastmonth",
					"dayupdown",
					"weekupdown",
					"monthupdown"
				};
				string[] array2 = new string[]
				{
					"t_djpoints"
				};
				object[,] array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = dbRoleInfo.RoleID;
				MySQLSelectCommand mySQLSelectCommand = new MySQLSelectCommand(mySQLConnection2, array, array2, array3, null, null);
				if (mySQLSelectCommand.Table.Rows.Count > 0)
				{
					bool flag = false;
					try
					{
						DBRoleInfo obj = dbRoleInfo;
						Monitor.Enter(dbRoleInfo, ref flag);
						dbRoleInfo.LastDJPointDataTikcs = DateTime.Now.Ticks / 10000L;
						dbRoleInfo.RoleDJPointData = new DJPointData
						{
							DbID = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["Id"].ToString()),
							RoleID = dbRoleInfo.RoleID,
							DJPoint = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["djpoint"].ToString()),
							Total = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["total"].ToString()),
							Wincnt = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["wincnt"].ToString()),
							Yestoday = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["yestoday"].ToString()),
							Lastweek = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["lastweek"].ToString()),
							Lastmonth = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["lastmonth"].ToString()),
							Dayupdown = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["dayupdown"].ToString()),
							Weekupdown = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["weekupdown"].ToString()),
							Monthupdown = Convert.ToInt32(mySQLSelectCommand.Table.Rows[0]["monthupdown"].ToString())
						};
					}
					finally
					{
						if (flag)
						{
							DBRoleInfo obj;
							Monitor.Exit(obj);
						}
					}
				}
				else
				{
					bool flag2 = false;
					try
					{
						DBRoleInfo obj = dbRoleInfo;
						Monitor.Enter(dbRoleInfo, ref flag2);
						dbRoleInfo.LastDJPointDataTikcs = DateTime.Now.Ticks / 10000L;
						dbRoleInfo.RoleDJPointData = new DJPointData
						{
							DbID = -1,
							RoleID = dbRoleInfo.RoleID,
							DJPoint = 0,
							Total = 0,
							Wincnt = 0,
							Yestoday = 0,
							Lastweek = 0,
							Lastmonth = 0,
							Dayupdown = 0,
							Weekupdown = 0,
							Monthupdown = 0
						};
					}
					finally
					{
						if (flag2)
						{
							DBRoleInfo obj;
							Monitor.Exit(obj);
						}
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", arg), EventLevels.Important);
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
		}

		public static List<DJPointData> QueryDJPointData(DBManager dbMgr)
		{
			List<DJPointData> list = new List<DJPointData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = "SELECT Id, rid, djpoint, total, wincnt, yestoday, lastweek, lastmonth, dayupdown, weekupdown, monthupdown FROM t_djpoints ORDER BY djpoint DESC LIMIT 100";
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int num = 0;
				while (mySQLDataReader.Read() && num < 100)
				{
					list.Add(new DJPointData
					{
						DbID = Convert.ToInt32(mySQLDataReader["Id"].ToString()),
						RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
						DJPoint = Convert.ToInt32(mySQLDataReader["djpoint"].ToString()),
						Total = Convert.ToInt32(mySQLDataReader["total"].ToString()),
						Wincnt = Convert.ToInt32(mySQLDataReader["wincnt"].ToString()),
						Yestoday = Convert.ToInt32(mySQLDataReader["yestoday"].ToString()),
						Lastweek = Convert.ToInt32(mySQLDataReader["lastweek"].ToString()),
						Lastmonth = Convert.ToInt32(mySQLDataReader["lastmonth"].ToString()),
						Dayupdown = Convert.ToInt32(mySQLDataReader["dayupdown"].ToString()),
						Weekupdown = Convert.ToInt32(mySQLDataReader["weekupdown"].ToString()),
						Monthupdown = Convert.ToInt32(mySQLDataReader["monthupdown"].ToString())
					});
					num++;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static Dictionary<string, BulletinMsgData> QueryBulletinMsgDict(DBManager dbMgr)
		{
			Dictionary<string, BulletinMsgData> dictionary = new Dictionary<string, BulletinMsgData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = "SELECT msgid, intervals, bulletintext, fromdate, todate FROM t_bulletin";
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					BulletinMsgData bulletinMsgData = new BulletinMsgData
					{
						MsgID = mySQLDataReader["msgid"].ToString(),
						Interval = Convert.ToInt32(mySQLDataReader["intervals"].ToString()),
						BulletinText = mySQLDataReader["bulletintext"].ToString(),
						BulletinTicks = DataHelper.ConvertToTicks(mySQLDataReader["fromdate"].ToString())
					};
					long num = DataHelper.ConvertToTicks(mySQLDataReader["todate"].ToString()) - bulletinMsgData.BulletinTicks;
					bulletinMsgData.PlayMinutes = (int)(num / 60000L);
					dictionary[bulletinMsgData.MsgID] = bulletinMsgData;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return dictionary;
		}

		public static Dictionary<string, string> QueryGameConfigDict(DBManager dbMgr)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = "SELECT paramname, paramvalue FROM t_config";
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					string key = mySQLDataReader["paramname"].ToString();
					string value = mySQLDataReader["paramvalue"].ToString();
					dictionary[key] = value;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return dictionary;
		}

		public static List<TempItemChargeInfo> QueryTempItemChargeInfo(DBManager dbMgr, int rid, int serialID = 0, byte HandleDel = 0)
		{
			MySQLConnection mySQLConnection = null;
			List<TempItemChargeInfo> result;
			try
			{
				List<TempItemChargeInfo> list = new List<TempItemChargeInfo>();
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text;
				if (serialID == 0)
				{
					text = string.Format("SELECT id, uid, rid, addmoney, itemid, chargetime, isdel FROM t_tempitem WHERE rid = {0}", rid);
				}
				else
				{
					text = string.Format("SELECT id, uid, rid, addmoney, itemid, chargetime, isdel FROM t_tempitem WHERE id = {0}", serialID);
				}
				if (HandleDel == 0)
				{
					text += string.Format(" AND isdel = 0", new object[0]);
				}
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					TempItemChargeInfo tempItemChargeInfo = new TempItemChargeInfo();
					tempItemChargeInfo.ID = Convert.ToInt32(mySQLDataReader["id"].ToString());
					tempItemChargeInfo.userID = mySQLDataReader["uid"].ToString();
					tempItemChargeInfo.chargeRoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString());
					tempItemChargeInfo.addUserMoney = Convert.ToInt32(mySQLDataReader["addmoney"].ToString());
					tempItemChargeInfo.zhigouID = Convert.ToInt32(mySQLDataReader["itemid"].ToString());
					tempItemChargeInfo.chargeTime = mySQLDataReader["chargetime"].ToString();
					tempItemChargeInfo.isDel = Convert.ToByte(mySQLDataReader["isdel"].ToString());
					list.Add(tempItemChargeInfo);
					if (serialID == 0 && tempItemChargeInfo.isDel == 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("从t_tempitem 找到 UID={0}，RID={1}，money={2}", mySQLDataReader["uid"].ToString(), mySQLDataReader["rid"].ToString(), Convert.ToInt32(mySQLDataReader["addmoney"].ToString())), null, true);
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
				result = list;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static void QueryTempMoney(DBManager dbMgr, List<TempMoneyInfo> tempMoneyInfoList)
		{
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = "SELECT id, uid, rid, addmoney, itemid, DATE_FORMAT(chargetime,'%Y-%m-%d %H:%i:%s') AS chargetime,cc FROM t_tempmoney";
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int val = int.MaxValue;
				int num = 0;
				while (mySQLDataReader.Read())
				{
					TempMoneyInfo tempMoneyInfo = new TempMoneyInfo();
					val = Math.Min(val, Convert.ToInt32(mySQLDataReader["id"].ToString()));
					num = Math.Max(num, Convert.ToInt32(mySQLDataReader["id"].ToString()));
					tempMoneyInfo.userID = mySQLDataReader["uid"].ToString();
					tempMoneyInfo.chargeRoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString());
					tempMoneyInfo.addUserMoney = Convert.ToInt32(mySQLDataReader["addmoney"].ToString());
					tempMoneyInfo.addUserItem = Convert.ToInt32(mySQLDataReader["itemid"].ToString());
					tempMoneyInfo.chargeTm = mySQLDataReader["chargetime"].ToString();
					bool flag = false;
					if (mySQLDataReader.FieldCount < 7)
					{
						LogManager.WriteLog(LogTypes.DataCheck, string.Format("DataCheckFaild#t_tempmoney#userid={0},money={1},cc={2}", tempMoneyInfo.userID, tempMoneyInfo.addUserMoney, ""), null, true);
						LogManager.WriteLog(LogTypes.Error, string.Format("从t_tempmoney 找到 UID={0}，money={1},数据库需升级", tempMoneyInfo.userID, tempMoneyInfo.addUserMoney), null, true);
						flag = true;
					}
					else
					{
						tempMoneyInfo.cc = mySQLDataReader["cc"].ToString();
						if (string.IsNullOrWhiteSpace(tempMoneyInfo.cc) || tempMoneyInfo.cc.Length < 32)
						{
							LogManager.WriteLog(LogTypes.DataCheck, string.Format("DataCheckFaild#t_tempmoney#userid={0},money={1},cc={2}", tempMoneyInfo.userID, tempMoneyInfo.addUserMoney, ""), null, true);
							LogManager.WriteLog(LogTypes.Error, string.Format("从t_tempmoney 找到 UID={0}，money={1},后台接口需升级", tempMoneyInfo.userID, tempMoneyInfo.addUserMoney), null, true);
							flag = true;
						}
						else
						{
							string text2 = tempMoneyInfo.cc.Substring(24, 8);
							string text3 = Global.GCC(4, new object[]
							{
								text2,
								tempMoneyInfo.userID,
								tempMoneyInfo.addUserMoney,
								tempMoneyInfo.chargeTm
							});
							if (text3.Substring(0, 24) != tempMoneyInfo.cc.Substring(0, 24))
							{
								LogManager.WriteLog(LogTypes.DataCheck, string.Format("DataCheckFaild#t_tempmoney#userid={0},money={1},cc={2}", tempMoneyInfo.userID, tempMoneyInfo.addUserMoney, tempMoneyInfo.cc), null, true);
								LogManager.WriteLog(LogTypes.Error, string.Format("从t_tempmoney 找到 UID={0}，money={1},校验失败", tempMoneyInfo.userID, tempMoneyInfo.addUserMoney), null, true);
								flag = true;
							}
							else
							{
								string cc = tempMoneyInfo.userID + tempMoneyInfo.chargeTm + tempMoneyInfo.cc;
								if (!Global.AddCC(cc))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("从t_tempmoney 找到 UID={0}，money={1},校验重复", tempMoneyInfo.userID, tempMoneyInfo.addUserMoney), null, true);
									flag = true;
								}
							}
						}
					}
					if (!flag)
					{
						tempMoneyInfoList.Add(tempMoneyInfo);
						LogManager.WriteLog(LogTypes.Error, string.Format("从t_tempmoney 找到 UID={0}，money={1}", tempMoneyInfo.userID, tempMoneyInfo.addUserMoney), null, true);
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
				if (num > 0)
				{
					text = string.Format("DELETE FROM t_tempmoney WHERE id<={0}", num);
					mySQLCommand = new MySQLCommand(text, mySQLConnection);
					mySQLCommand.ExecuteNonQuery();
					mySQLCommand.Dispose();
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				}
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
		}

		public static Dictionary<string, LiPinMaItem> QueryLiPinMaDict(DBManager dbMgr)
		{
			Dictionary<string, LiPinMaItem> dictionary = new Dictionary<string, LiPinMaItem>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = "SELECT lipinma, huodongid, maxnum, usednum, ptid, ptrepeat FROM t_linpinma";
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					LiPinMaItem liPinMaItem = new LiPinMaItem
					{
						LiPinMa = mySQLDataReader["lipinma"].ToString(),
						HuodongID = Convert.ToInt32(mySQLDataReader["huodongid"].ToString()),
						MaxNum = Convert.ToInt32(mySQLDataReader["maxnum"].ToString()),
						UsedNum = Convert.ToInt32(mySQLDataReader["usednum"].ToString()),
						PingTaiID = Convert.ToInt32(mySQLDataReader["ptid"].ToString()),
						PingTaiRepeat = Convert.ToInt32(mySQLDataReader["ptrepeat"].ToString())
					};
					dictionary[liPinMaItem.LiPinMa] = liPinMaItem;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return dictionary;
		}

		public static void QueryPreNames(DBManager dbMgr, Dictionary<string, PreNameItem> preNamesDict, List<PreNameItem> malePreNamesList, List<PreNameItem> femalePreNamesList)
		{
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = "SELECT name, sex FROM t_prenames WHERE used=0";
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					PreNameItem preNameItem = new PreNameItem
					{
						Name = mySQLDataReader["name"].ToString(),
						Sex = Convert.ToInt32(mySQLDataReader["sex"].ToString()),
						Used = 0
					};
					preNamesDict[preNameItem.Name] = preNameItem;
					if (0 == preNameItem.Sex)
					{
						malePreNamesList.Add(preNameItem);
					}
					else
					{
						femalePreNamesList.Add(preNameItem);
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
		}

		public static Dictionary<int, FuBenHistData> QueryFuBenHistDict(DBManager dbMgr)
		{
			Dictionary<int, FuBenHistData> dictionary = new Dictionary<int, FuBenHistData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = "SELECT fubenid, t_fubenhist.rid, t_roles.rname, usedsecs FROM t_fubenhist, t_roles WHERE t_roles.rid=t_fubenhist.rid";
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					FuBenHistData fuBenHistData = new FuBenHistData
					{
						FuBenID = Convert.ToInt32(mySQLDataReader["fubenid"].ToString()),
						RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
						RoleName = mySQLDataReader["rname"].ToString(),
						UsedSecs = Convert.ToInt32(mySQLDataReader["usedsecs"].ToString())
					};
					dictionary[fuBenHistData.FuBenID] = fuBenHistData;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return dictionary;
		}

		public static string QueryUserIDByRoleName(DBManager dbMgr, string otherRoleName, int zoneID)
		{
			string result = "";
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT userid FROM t_roles WHERE rname='{0}' AND zoneid={1}", otherRoleName, zoneID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = mySQLDataReader["userid"].ToString();
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static void QueryUserMoneyByUserID(DBManager dbMgr, string userID, out int userMoney, out int realMoney)
		{
			userMoney = 0;
			realMoney = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT money, realmoney FROM t_money WHERE userid='{0}'", userID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					userMoney = Convert.ToInt32(mySQLDataReader["money"].ToString());
					realMoney = Convert.ToInt32(mySQLDataReader["realmoney"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
		}

		public static void QueryUserUserIdValue(DBManager dbMgr, string userID, out int realMoney, out int unionLevel)
		{
			unionLevel = 0;
			realMoney = 0;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("SELECT realmoney FROM t_money WHERE userid='{0}'", userID);
				realMoney = myDbConnection.GetSingleInt(sql, 0, new MySQLParameter[0]);
				sql = string.Format("SELECT MAX(changelifecount*0xffff+`level`) FROM t_roles WHERE userid='{0}'", userID);
				unionLevel = myDbConnection.GetSingleInt(sql, 0, new MySQLParameter[0]);
			}
		}

		public static void QueryTodayUserMoneyByUserID(DBManager dbMgr, string userID, int zoneID, out int userMoney, out int realMoney)
		{
			userMoney = 0;
			realMoney = 0;
			DateTime now = DateTime.Now;
			string text = string.Format("{0:0000}-{1:00}-{2:00} 00:00:00", now.Year, now.Month, now.Day);
			string text2 = string.Format("{0:0000}-{1:00}-{2:00} 23:59:59", now.Year, now.Month, now.Day);
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text3 = string.Format("SELECT SUM(amount) AS totalmoney FROM t_inputlog WHERE u='{0}' AND inputtime>='{2}' AND inputtime<='{3}'", new object[]
				{
					userID,
					zoneID,
					text,
					text2
				});
				MySQLCommand mySQLCommand = new MySQLCommand(text3, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					try
					{
						userMoney = Convert.ToInt32(mySQLDataReader["totalmoney"].ToString());
						realMoney = userMoney;
					}
					catch (Exception)
					{
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text3), EventLevels.Important);
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

		public static void QueryTodayUserMoneyByUserID2(DBManager dbMgr, string userID, int zoneID, out int userMoney, out int realMoney)
		{
			userMoney = 0;
			realMoney = 0;
			DateTime now = DateTime.Now;
			string text = string.Format("{0:0000}-{1:00}-{2:00} 00:01:01", now.Year, now.Month, now.Day);
			string text2 = string.Format("{0:0000}-{1:00}-{2:00} 23:59:59", now.Year, now.Month, now.Day);
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text3 = string.Format("SELECT SUM(amount) AS totalmoney FROM t_inputlog2 WHERE u='{0}' AND inputtime>='{2}' AND inputtime<='{3}'", new object[]
				{
					userID,
					zoneID,
					text,
					text2
				});
				MySQLCommand mySQLCommand = new MySQLCommand(text3, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					try
					{
						userMoney = Convert.ToInt32(mySQLDataReader["totalmoney"].ToString());
						realMoney = userMoney;
					}
					catch (Exception)
					{
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text3), EventLevels.Important);
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

		public static List<SearchRoleData> SearchOnlineRoleByName(DBManager dbMgr, string searchText, int startIndex, int limitNum)
		{
			List<SearchRoleData> list = new List<SearchRoleData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT rid, rname, sex, occupation, level, zoneid, faction, bhname, changelifecount FROM t_roles WHERE rname LIKE '%{0}%' AND rid>{1} AND lasttime>logofftime AND isdel=0 LIMIT {2}", searchText, startIndex, limitNum);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					int num = Convert.ToInt32(mySQLDataReader["changelifecount"].ToString());
					int num2 = Convert.ToInt32(mySQLDataReader["level"].ToString());
					if (100 * num + num2 >= 50)
					{
						SearchRoleData item = new SearchRoleData
						{
							RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
							RoleName = Global.FormatRoleName(Convert.ToInt32(mySQLDataReader["zoneid"].ToString()), mySQLDataReader["rname"].ToString()),
							RoleSex = Convert.ToInt32(mySQLDataReader["sex"].ToString()),
							Level = num2,
							Occupation = Convert.ToInt32(mySQLDataReader["occupation"].ToString()),
							Faction = Convert.ToInt32(mySQLDataReader["faction"].ToString()),
							BHName = mySQLDataReader["bhname"].ToString()
						};
						list.Add(item);
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static string GetRoleParamByName(DBManager dbMgr, int roleID, string paramName)
		{
			List<PaiHangItemData> list = new List<PaiHangItemData>();
			MySQLConnection mySQLConnection = null;
			string result = "";
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType(paramName, null);
				string text = string.Format("SELECT p.rid, p.{2} FROM {3} as p  where p.{4}={0} and p.rid={1}", new object[]
				{
					roleParamType.KeyString,
					roleID,
					roleParamType.ColumnName,
					roleParamType.TableName,
					roleParamType.IdxName
				});
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					result = mySQLDataReader[1].ToString();
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static List<PaiHangItemData> GetRoleParamsTablePaiHang(DBManager dbMgr, string paramName)
		{
			List<PaiHangItemData> list = new List<PaiHangItemData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType(paramName, null);
				string text = string.Format("SELECT p.rid, {0}, rname, zoneid FROM {1} as p, t_roles as r  where p.{2}={3} and p.rid=r.rid ORDER BY p.{0} DESC LIMIT 100", new object[]
				{
					roleParamType.ColumnName,
					roleParamType.TableName,
					roleParamType.IdxName,
					roleParamType.KeyString
				});
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int num = 0;
				while (mySQLDataReader.Read() && num < 100)
				{
					PaiHangItemData item = new PaiHangItemData
					{
						RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
						RoleName = Global.FormatRoleName(mySQLDataReader["zoneid"].ToString(), mySQLDataReader["rname"].ToString()),
						Val1 = Convert.ToInt32(mySQLDataReader[1].ToString())
					};
					list.Add(item);
					num++;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		private static List<PaiHangItemData> GetUserMoneyTablePaiHang(DBManager dbMgr, string fieldVal1, string otherCondition)
		{
			List<PaiHangItemData> list = new List<PaiHangItemData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT userid, {0} FROM t_money{1} ORDER BY {0} DESC LIMIT 100", fieldVal1, otherCondition);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int num = 0;
				while (mySQLDataReader.Read() && num < 100)
				{
					PaiHangItemData item = new PaiHangItemData
					{
						uid = mySQLDataReader["userid"].ToString(),
						Val1 = Convert.ToInt32(mySQLDataReader[fieldVal1].ToString())
					};
					list.Add(item);
					num++;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		private static List<PaiHangItemData> GetRoleTablePaiHang(DBManager dbMgr, string fieldVal1, string otherCondition)
		{
			List<PaiHangItemData> list = new List<PaiHangItemData>();
			List<PaiHangItemData> result;
			if (!GameDBManager.GameConfigMgr.IsPaiHangKey(fieldVal1))
			{
				result = list;
			}
			else
			{
				MySQLConnection mySQLConnection = null;
				try
				{
					mySQLConnection = dbMgr.DBConns.PopDBConnection();
					string text = string.Format("SELECT rid, rname, zoneid, admiredcount, {0} FROM t_roles{1} ORDER BY {0} DESC LIMIT 100", fieldVal1, otherCondition);
					MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					int num = 0;
					while (mySQLDataReader.Read() && num < 100)
					{
						PaiHangItemData item = new PaiHangItemData
						{
							RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
							RoleName = Global.FormatRoleName(mySQLDataReader["zoneid"].ToString(), mySQLDataReader["rname"].ToString()),
							Val1 = Convert.ToInt32(mySQLDataReader[fieldVal1].ToString()),
							Val2 = Convert.ToInt32(mySQLDataReader["admiredcount"].ToString())
						};
						list.Add(item);
						num++;
					}
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
					mySQLCommand.Dispose();
				}
				finally
				{
					if (null != mySQLConnection)
					{
						dbMgr.DBConns.PushDBConnection(mySQLConnection);
					}
				}
				result = list;
			}
			return result;
		}

		public static List<PaiHangItemData> GetRoleEquipPaiHang(DBManager dbMgr)
		{
			return DBQuery.GetRoleTablePaiHang(dbMgr, "equipjifen", " WHERE equipjifen>0 AND isdel=0 AND isflashplayer=0");
		}

		public static List<PaiHangItemData> GetRoleXueWeiNumPaiHang(DBManager dbMgr)
		{
			return DBQuery.GetRoleTablePaiHang(dbMgr, "xueweinum", " WHERE xueweinum>=20 AND isdel=0");
		}

		public static List<PaiHangItemData> GetRoleSkillLevelPaiHang(DBManager dbMgr)
		{
			return DBQuery.GetRoleTablePaiHang(dbMgr, "skilllearnednum", " WHERE skilllearnednum>=60 AND isdel=0");
		}

		public static List<PaiHangItemData> GetRoleHorseJiFenPaiHang(DBManager dbMgr)
		{
			return DBQuery.GetRoleTablePaiHang(dbMgr, "horsejifen", " WHERE horsejifen>=54 AND isdel=0 AND isflashplayer=0");
		}

		public static List<PaiHangItemData> GetRoleLevelPaiHang(DBManager dbMgr)
		{
			List<PaiHangItemData> list = new List<PaiHangItemData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT rid, rname, zoneid, level, changelifecount, admiredcount FROM t_roles WHERE level>0 AND isdel=0 AND isflashplayer=0 ORDER BY changelifecount DESC, level DESC, experience DESC LIMIT 100", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int num = 0;
				while (mySQLDataReader.Read() && num < 100)
				{
					PaiHangItemData item = new PaiHangItemData
					{
						RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
						RoleName = Global.FormatRoleName(mySQLDataReader["zoneid"].ToString(), mySQLDataReader["rname"].ToString()),
						Val1 = Convert.ToInt32(mySQLDataReader["level"].ToString()),
						Val2 = Convert.ToInt32(mySQLDataReader["changelifecount"].ToString()),
						Val3 = Convert.ToInt32(mySQLDataReader["admiredcount"].ToString())
					};
					list.Add(item);
					num++;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static List<PaiHangItemData> GetRoleYinLiangPaiHang(DBManager dbMgr)
		{
			return DBQuery.GetRoleTablePaiHang(dbMgr, "yinliang", " WHERE yinliang>0 AND isdel=0 AND isflashplayer=0");
		}

		public static List<PaiHangItemData> GetRoleGoldPaiHang(DBManager dbMgr)
		{
			return DBQuery.GetRoleTablePaiHang(dbMgr, "money2", " WHERE money2>0 AND isdel=0 AND isflashplayer=0");
		}

		public static List<PaiHangItemData> GetRoleLianZhanPaiHang(DBManager dbMgr)
		{
			return DBQuery.GetRoleTablePaiHang(dbMgr, "lianzhan", " WHERE lianzhan>=100 AND isdel=0 AND isflashplayer=0");
		}

		public static List<PaiHangItemData> GetRoleKillBossPaiHang(DBManager dbMgr)
		{
			return DBQuery.GetRoleTablePaiHang(dbMgr, "killboss", " WHERE killboss>=1 AND isdel=0 AND isflashplayer=0");
		}

		public static List<PaiHangItemData> GetRoleBattleNumPaiHang(DBManager dbMgr)
		{
			return DBQuery.GetRoleTablePaiHang(dbMgr, "battlenum", " WHERE battlenum>=1 AND isdel=0 AND isflashplayer=0");
		}

		public static List<PaiHangItemData> GetRoleHeroIndexPaiHang(DBManager dbMgr)
		{
			return DBQuery.GetRoleTablePaiHang(dbMgr, "heroindex", " WHERE heroindex>=1 AND isdel=0 AND isflashplayer=0");
		}

		public static List<PaiHangItemData> GetRoleCombatForcePaiHang(DBManager dbMgr)
		{
			return DBQuery.GetRoleTablePaiHang(dbMgr, "combatforce", " WHERE combatforce>=1 AND isdel=0 AND isflashplayer=0");
		}

		public static List<PaiHangItemData> GetUserMoneyPaiHang(DBManager dbMgr)
		{
			return DBQuery.GetUserMoneyTablePaiHang(dbMgr, "money", " WHERE money>0");
		}

		public static List<PaiHangItemData> GetRoleGuardStatuePaiHang(DBManager dbMgr)
		{
			List<PaiHangItemData> list = new List<PaiHangItemData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT roleid, suit, level FROM t_guard_statue ORDER BY suit DESC, level DESC, roleid ASC LIMIT 100", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int num = 0;
				while (mySQLDataReader.Read() && num < 100)
				{
					PaiHangItemData item = new PaiHangItemData
					{
						RoleID = Convert.ToInt32(mySQLDataReader["roleid"].ToString()),
						Val1 = Convert.ToInt32(mySQLDataReader["suit"].ToString()),
						Val2 = Convert.ToInt32(mySQLDataReader["level"].ToString())
					};
					list.Add(item);
					num++;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static List<PaiHangItemData> GetRoleHolyItemPaiHang(DBManager dbMgr)
		{
			List<PaiHangItemData> list = new List<PaiHangItemData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT roleid, SUM(part_suit) AS lev FROM t_holyitem GROUP BY roleid ORDER BY lev DESC, roleid ASC LIMIT 100", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int num = 0;
				while (mySQLDataReader.Read() && num < 100)
				{
					PaiHangItemData item = new PaiHangItemData
					{
						RoleID = Convert.ToInt32(mySQLDataReader["roleid"].ToString()),
						Val1 = Convert.ToInt32(mySQLDataReader["lev"].ToString())
					};
					list.Add(item);
					num++;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static BangHuiListData GetBangHuiItemDataList(DBManager dbMgr, int isVerify, int startIndex, int endIndex)
		{
			BangHuiListData bangHuiListData = new BangHuiListData();
			bangHuiListData.TotalBangHuiItemNum = 0;
			bangHuiListData.BangHuiItemDataList = new List<BangHuiItemData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT b.bhid, b.bhname, b.zoneid, b.rid, r.rname, r.occupation, b.totalnum, b.totallevel, b.qilevel, b.isverfiy, b.totalcombatforce FROM t_banghui AS b, t_roles AS r WHERE b.isdel=0 AND b.rid=r.rid ORDER BY b.totalcombatforce DESC", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					if (isVerify < 0 || Convert.ToInt32(mySQLDataReader["isverfiy"].ToString()) == isVerify)
					{
						BangHuiItemData item = new BangHuiItemData
						{
							BHID = Convert.ToInt32(mySQLDataReader["bhid"].ToString()),
							BHName = mySQLDataReader["bhname"].ToString(),
							ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString()),
							BZRoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
							BZRoleName = mySQLDataReader["rname"].ToString(),
							BZOccupation = Convert.ToInt32(mySQLDataReader["occupation"].ToString()),
							TotalNum = Convert.ToInt32(mySQLDataReader["totalnum"].ToString()),
							TotalLevel = Convert.ToInt32(mySQLDataReader["totallevel"].ToString()),
							QiLevel = Convert.ToInt32(mySQLDataReader["qilevel"].ToString()),
							IsVerfiy = Convert.ToInt32(mySQLDataReader["isverfiy"].ToString()),
							TotalCombatForce = Convert.ToInt32(mySQLDataReader["totalcombatforce"].ToString())
						};
						bangHuiListData.BangHuiItemDataList.Add(item);
						bangHuiListData.TotalBangHuiItemNum++;
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return bangHuiListData;
		}

		public static int FindBangHuiByRoleID(DBManager dbMgr, int roleID)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT bhid FROM t_banghui WHERE isdel=0 AND rid={0}", roleID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Convert.ToInt32(mySQLDataReader["bhid"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int FindJoinBangHuiByRoleID(DBManager dbMgr, int roleID)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT faction FROM t_roles WHERE rid={0}", roleID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Convert.ToInt32(mySQLDataReader["faction"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static bool QueryBHMemberSumData(int bhid, out int totalNum, out int totalLevel, out long totalCombatforce)
		{
			totalCombatforce = (long)(totalNum = (totalLevel = 0));
			try
			{
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("SELECT SUM(combatforce) AS totalcombatforce,SUM(LEVEL) AS totallevel,COUNT(rid) AS totalnum FROM t_roles WHERE isdel=0 and faction={0}", bhid);
					MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
					if (mySQLDataReader.Read())
					{
						totalCombatforce = Convert.ToInt64(mySQLDataReader["totalcombatforce"].ToString());
						totalLevel = Convert.ToInt32(mySQLDataReader["totallevel"].ToString());
						totalNum = Convert.ToInt32(mySQLDataReader["totalnum"].ToString());
					}
					return true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return false;
		}

		public static int QueryBHMemberNum(DBManager dbMgr, int bhid)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT COUNT(rid) AS totalnum FROM t_roles WHERE isdel=0 AND faction={0}", bhid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Convert.ToInt32(mySQLDataReader["totalnum"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int QueryBHMemberLevel(DBManager dbMgr, int bhid)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT SUM(level) AS totallevel FROM t_roles WHERE isdel=0 AND faction={0}", bhid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					try
					{
						result = Convert.ToInt32(mySQLDataReader["totallevel"].ToString());
					}
					catch (Exception)
					{
						result = 0;
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
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
			return result;
		}

		public static int QueryBHMemberTotalCombatForce(DBManager dbMgr, int bhid)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT SUM(combatforce) AS totalcombatforce FROM t_roles WHERE isdel=0 AND faction={0}", bhid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					try
					{
						result = Convert.ToInt32(mySQLDataReader["totalcombatforce"].ToString());
					}
					catch (Exception)
					{
						result = 0;
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
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
			return result;
		}

		public static int[] QueryZhengDuoUsedTime(DBManager dbMgr, int bhid)
		{
			int[] array = new int[2];
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("select zhengduoweek,zhengduousedtime from t_banghui WHERE bhid={0}", bhid);
				MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
				if (mySQLDataReader.Read())
				{
					array[0] = Convert.ToInt32(mySQLDataReader["zhengduoweek"].ToString());
					array[1] = Convert.ToInt32(mySQLDataReader["zhengduousedtime"].ToString());
				}
			}
			return array;
		}

		public static BangHuiDetailData QueryBangHuiInfoByID(DBManager dbMgr, int bhid)
		{
			BangHuiDetailData bangHuiDetailData = null;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT b.bhid, b.bhname, b.zoneid, b.rid, r.rname, r.occupation, b.totalnum, b.totallevel, b.bhbulletin, b.buildtime, b.qiname, b.qilevel, b.isverfiy, b.tongqian, b.jitan, b.junxie, b.guanghuan, b.can_mod_name_times,b.totalcombatforce,b.zhengduousedtime FROM t_banghui AS b, t_roles AS r WHERE b.isdel=0 AND b.rid=r.rid AND b.bhid={0}", bhid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					bangHuiDetailData = new BangHuiDetailData();
					bangHuiDetailData.BHID = Convert.ToInt32(mySQLDataReader["bhid"].ToString());
					bangHuiDetailData.BHName = mySQLDataReader["bhname"].ToString();
					bangHuiDetailData.ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString());
					bangHuiDetailData.BZRoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString());
					bangHuiDetailData.BZRoleName = mySQLDataReader["rname"].ToString();
					bangHuiDetailData.BZOccupation = Convert.ToInt32(mySQLDataReader["occupation"].ToString());
					bangHuiDetailData.TotalNum = Convert.ToInt32(mySQLDataReader["totalnum"].ToString());
					bangHuiDetailData.TotalLevel = Convert.ToInt32(mySQLDataReader["totallevel"].ToString());
					bangHuiDetailData.BHBulletin = mySQLDataReader["bhbulletin"].ToString();
					bangHuiDetailData.BuildTime = mySQLDataReader["buildtime"].ToString();
					bangHuiDetailData.QiName = mySQLDataReader["qiname"].ToString();
					bangHuiDetailData.QiLevel = Convert.ToInt32(mySQLDataReader["qilevel"].ToString());
					bangHuiDetailData.IsVerify = Convert.ToInt32(mySQLDataReader["isverfiy"].ToString());
					bangHuiDetailData.TotalMoney = Convert.ToInt32(mySQLDataReader["tongqian"].ToString());
					bangHuiDetailData.JiTan = Convert.ToInt32(mySQLDataReader["jitan"].ToString());
					bangHuiDetailData.JunXie = Convert.ToInt32(mySQLDataReader["junxie"].ToString());
					bangHuiDetailData.GuangHuan = Convert.ToInt32(mySQLDataReader["guanghuan"].ToString());
					bangHuiDetailData.CanModNameTimes = Convert.ToInt32(mySQLDataReader["can_mod_name_times"].ToString());
					bangHuiDetailData.TotalCombatForce = (long)Convert.ToInt32(mySQLDataReader["totalcombatforce"].ToString());
					bangHuiDetailData.ZhengDuoUsedTime = (long)Convert.ToInt32(mySQLDataReader["zhengduousedtime"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return bangHuiDetailData;
		}

		public static List<BHMatchSupportData> LoadBHMatchSupportFlagData(DBManager dbMgr, int rid, int minSeasonID, int minRound)
		{
			List<BHMatchSupportData> list = new List<BHMatchSupportData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT * FROM t_banghui_match_support_flag WHERE rid={0} AND (season>{1} OR (season={1} AND `round`>={2}))", rid, minSeasonID, minRound);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					list.Add(new BHMatchSupportData
					{
						season = Convert.ToInt32(mySQLDataReader["season"].ToString()),
						round = Convert.ToInt32(mySQLDataReader["round"].ToString()),
						bhid1 = Convert.ToInt32(mySQLDataReader["bhid1"].ToString()),
						bhid2 = Convert.ToInt32(mySQLDataReader["bhid2"].ToString()),
						guess = Convert.ToInt32(mySQLDataReader["guess"].ToString()),
						rid = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
						isaward = Convert.ToByte(mySQLDataReader["is_award"].ToString())
					});
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static List<BangHuiMgrItemData> GetBangHuiMgrItemItemDataList(DBManager dbMgr, int bhid)
		{
			List<BangHuiMgrItemData> list = new List<BangHuiMgrItemData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT r.zoneid, r.rid, r.rname, r.occupation, r.bhzhiwu, r.chenghao, r.banggong, r.level FROM t_roles AS r WHERE r.bhzhiwu>0 AND r.faction={0} and r.isdel=0", bhid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					BangHuiMgrItemData item = new BangHuiMgrItemData
					{
						ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString()),
						RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
						RoleName = mySQLDataReader["rname"].ToString(),
						Occupation = Convert.ToInt32(mySQLDataReader["occupation"].ToString()),
						BHZhiwu = Convert.ToInt32(mySQLDataReader["bhzhiwu"].ToString()),
						ChengHao = mySQLDataReader["chenghao"].ToString(),
						BangGong = Convert.ToInt32(mySQLDataReader["banggong"].ToString()),
						Level = Convert.ToInt32(mySQLDataReader["level"].ToString())
					};
					list.Add(item);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static BangHuiMgrItemData GetBangHuiMgrItemItemDataByID(DBManager dbMgr, int bhid, int roleID)
		{
			BangHuiMgrItemData result = null;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT r.zoneid, r.rid, r.rname, r.occupation, r.bhzhiwu, r.chenghao, r.banggong, r.level FROM t_roles AS r WHERE r.bhzhiwu>0 AND r.faction={0} AND r.rid={1}", bhid, roleID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = new BangHuiMgrItemData
					{
						ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString()),
						RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
						RoleName = mySQLDataReader["rname"].ToString(),
						Occupation = Convert.ToInt32(mySQLDataReader["occupation"].ToString()),
						BHZhiwu = Convert.ToInt32(mySQLDataReader["bhzhiwu"].ToString()),
						ChengHao = mySQLDataReader["chenghao"].ToString(),
						BangGong = Convert.ToInt32(mySQLDataReader["banggong"].ToString()),
						Level = Convert.ToInt32(mySQLDataReader["level"].ToString())
					};
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static List<BangHuiMemberData> GetBangHuiMemberDataList(DBManager dbMgr, int bhid)
		{
			List<BangHuiMemberData> list = new List<BangHuiMemberData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT r.zoneid, r.rid, r.rname, r.occupation, r.bhzhiwu, r.chenghao, r.banggong, r.level, r.xueweinum, r.skilllearnednum, r.combatforce, r.changelifecount,r.juntuanzhiwu FROM t_roles AS r WHERE r.faction={0} AND r.isdel=0", bhid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					BangHuiMemberData item = new BangHuiMemberData
					{
						ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString()),
						RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
						RoleName = mySQLDataReader["rname"].ToString(),
						Occupation = Convert.ToInt32(mySQLDataReader["occupation"].ToString()),
						BHZhiwu = Convert.ToInt32(mySQLDataReader["bhzhiwu"].ToString()),
						ChengHao = mySQLDataReader["chenghao"].ToString(),
						BangGong = Convert.ToInt32(mySQLDataReader["banggong"].ToString()),
						Level = Convert.ToInt32(mySQLDataReader["level"].ToString()),
						XueWeiNum = Convert.ToInt32(mySQLDataReader["xueweinum"].ToString()),
						SkillLearnedNum = Convert.ToInt32(mySQLDataReader["skilllearnednum"].ToString()),
						BangHuiMemberCombatForce = Convert.ToInt32(mySQLDataReader["combatforce"].ToString()),
						BangHuiMemberChangeLifeLev = Convert.ToInt32(mySQLDataReader["changelifecount"].ToString()),
						JunTuanZhiWu = Convert.ToInt32(mySQLDataReader["juntuanzhiwu"].ToString())
					};
					list.Add(item);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static string GetBangHuiMgrItemItemStringList(DBManager dbMgr, int bhid)
		{
			StringBuilder stringBuilder = new StringBuilder();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT r.zoneid, r.rid, r.rname, r.occupation, r.bhzhiwu, r.chenghao, r.banggong FROM t_roles AS r WHERE r.bhzhiwu>0 AND r.faction={0}", bhid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					stringBuilder.AppendFormat("{0},", mySQLDataReader["rid"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return stringBuilder.ToString();
		}

		public static BangHuiBagData QueryBangHuiBagDataByID(DBManager dbMgr, int bhid)
		{
			BangHuiBagData bangHuiBagData = new BangHuiBagData();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT goods1num, goods2num, goods3num, goods4num, goods5num, tongqian FROM t_banghui WHERE isdel=0 AND bhid={0}", bhid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					bangHuiBagData.Goods1Num = Convert.ToInt32(mySQLDataReader["goods1num"].ToString());
					bangHuiBagData.Goods2Num = Convert.ToInt32(mySQLDataReader["goods2num"].ToString());
					bangHuiBagData.Goods3Num = Convert.ToInt32(mySQLDataReader["goods3num"].ToString());
					bangHuiBagData.Goods4Num = Convert.ToInt32(mySQLDataReader["goods4num"].ToString());
					bangHuiBagData.Goods5Num = Convert.ToInt32(mySQLDataReader["goods5num"].ToString());
					bangHuiBagData.TongQian = Convert.ToInt32(mySQLDataReader["tongqian"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return bangHuiBagData;
		}

		public static List<BangGongHistData> GetBangHuiBagHistList(DBManager dbMgr, int bhid)
		{
			List<BangGongHistData> list = new List<BangGongHistData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT r.zoneid, r.rid, r.rname, r.occupation, r.level, r.bhzhiwu, r.chenghao, b.goods1num, b.goods2num, b.goods3num, b.goods4num, b.goods5num, b.tongqian, b.banggong FROM t_banggonghist AS b, t_roles AS r WHERE b.bhid={0} AND b.rid=r.rid ORDER BY b.banggong DESC", bhid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					BangGongHistData item = new BangGongHistData
					{
						ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString()),
						RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
						RoleName = mySQLDataReader["rname"].ToString(),
						Occupation = Convert.ToInt32(mySQLDataReader["occupation"].ToString()),
						RoleLevel = Convert.ToInt32(mySQLDataReader["level"].ToString()),
						BHZhiWu = Convert.ToInt32(mySQLDataReader["bhzhiwu"].ToString()),
						BHChengHao = mySQLDataReader["chenghao"].ToString(),
						Goods1Num = Convert.ToInt32(mySQLDataReader["goods1num"].ToString()),
						Goods2Num = Convert.ToInt32(mySQLDataReader["goods2num"].ToString()),
						Goods3Num = Convert.ToInt32(mySQLDataReader["goods3num"].ToString()),
						Goods4Num = Convert.ToInt32(mySQLDataReader["goods4num"].ToString()),
						Goods5Num = Convert.ToInt32(mySQLDataReader["goods5num"].ToString()),
						TongQian = Convert.ToInt32(mySQLDataReader["tongqian"].ToString()),
						BangGong = Convert.ToInt32(mySQLDataReader["banggong"].ToString())
					};
					list.Add(item);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static BangQiInfoData QueryBangQiInfoByID(DBManager dbMgr, int bhid)
		{
			BangQiInfoData bangQiInfoData = new BangQiInfoData();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT b.qiname, b.qilevel FROM t_banghui AS b WHERE b.isdel=0 AND b.bhid={0}", bhid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					bangQiInfoData.BangQiName = mySQLDataReader["qiname"].ToString();
					bangQiInfoData.BangQiLevel = Convert.ToInt32(mySQLDataReader["qilevel"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return bangQiInfoData;
		}

		public static Dictionary<int, BHLingDiOwnData> GetBHLingDiOwnDataDict(DBManager dbMgr)
		{
			Dictionary<int, BHLingDiOwnData> dictionary = new Dictionary<int, BHLingDiOwnData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT l.lingdi, b.zoneid, b.bhid, b.bhname, b.qiname, b.qilevel FROM t_banghui AS b, t_lingdi AS l WHERE b.bhid=l.bhid AND b.isdel=0", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					BHLingDiOwnData bhlingDiOwnData = new BHLingDiOwnData
					{
						LingDiID = Convert.ToInt32(mySQLDataReader["lingdi"].ToString()),
						ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString()),
						BHID = Convert.ToInt32(mySQLDataReader["bhid"].ToString()),
						BHName = mySQLDataReader["bhname"].ToString(),
						BangQiName = mySQLDataReader["qiname"].ToString(),
						BangQiLevel = Convert.ToInt32(mySQLDataReader["qilevel"].ToString())
					};
					dictionary[bhlingDiOwnData.LingDiID] = bhlingDiOwnData;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return dictionary;
		}

		public static void QueryPreDeleteRoleDict(DBManager dbMgr, Dictionary<int, DateTime> preDeleteRoleDict)
		{
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT rid, predeltime FROM t_roles WHERE isdel=0 and predeltime IS NOT NULL", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					int key = Convert.ToInt32(mySQLDataReader["rid"].ToString());
					DateTime value;
					if (DateTime.TryParse(mySQLDataReader["predeltime"].ToString(), out value))
					{
						preDeleteRoleDict[key] = value;
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
		}

		public static void QueryBangQiDict(DBManager dbMgr, Dictionary<int, BangHuiJunQiItemData> bangHuiJunQiItemDcit)
		{
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT b.bhid, b.qiname, b.qilevel FROM t_banghui AS b WHERE b.isdel=0", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					int num = Convert.ToInt32(mySQLDataReader["bhid"].ToString());
					bangHuiJunQiItemDcit[num] = new BangHuiJunQiItemData
					{
						BHID = num,
						QiName = mySQLDataReader["qiname"].ToString(),
						QiLevel = Convert.ToInt32(mySQLDataReader["qilevel"].ToString())
					};
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
		}

		public static void QueryBHLingDiInfoDict(DBManager dbMgr, Dictionary<int, BangHuiLingDiInfoData> bangHuiLingDiItemsDict)
		{
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT l.lingdi, l.bhid, 1 as zoneid, \"\" as bhname, l.tax, l.takedayid, l.takedaynum, l.yestodaytax, l.taxdayid, l.todaytax, l.totaltax, l.warrequest, l.awardfetchday FROM t_lingdi AS l", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					BangHuiLingDiInfoData bangHuiLingDiInfoData = new BangHuiLingDiInfoData
					{
						LingDiID = Convert.ToInt32(mySQLDataReader["lingdi"].ToString()),
						BHID = Convert.ToInt32(mySQLDataReader["bhid"].ToString()),
						ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString()),
						BHName = mySQLDataReader["bhname"].ToString(),
						LingDiTax = Convert.ToInt32(mySQLDataReader["tax"].ToString()),
						TakeDayID = Convert.ToInt32(mySQLDataReader["takedayid"].ToString()),
						TakeDayNum = Convert.ToInt32(mySQLDataReader["takedaynum"].ToString()),
						YestodayTax = Convert.ToInt32(mySQLDataReader["yestodaytax"].ToString()),
						TaxDayID = Convert.ToInt32(mySQLDataReader["taxdayid"].ToString()),
						TodayTax = Convert.ToInt32(mySQLDataReader["todaytax"].ToString()),
						TotalTax = Convert.ToInt32(mySQLDataReader["TotalTax"].ToString()),
						AwardFetchDay = Convert.ToInt32(mySQLDataReader["awardfetchday"].ToString())
					};
					byte[] array = mySQLDataReader["warrequest"] as byte[];
					if (null == array)
					{
						bangHuiLingDiInfoData.WarRequest = "";
					}
					else
					{
						bangHuiLingDiInfoData.WarRequest = Encoding.Default.GetString(array);
					}
					bangHuiLingDiItemsDict[bangHuiLingDiInfoData.LingDiID] = bangHuiLingDiInfoData;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
		}

		public static string QueryBangFuBenByID(DBManager dbMgr, int bhid)
		{
			string result = "";
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT b.fubenid, b.fubenstate, b.openday, b.killers FROM t_banghui AS b WHERE b.isdel=0 AND b.bhid={0}", bhid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						bhid,
						Convert.ToInt32(mySQLDataReader["fubenid"].ToString()),
						Convert.ToInt32(mySQLDataReader["fubenstate"].ToString()),
						Convert.ToInt32(mySQLDataReader["openday"].ToString()),
						mySQLDataReader["killers"].ToString()
					});
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int QueryHuangFeiCount(DBManager dbMgr)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT COUNT(rid) AS huanghounum FROM t_roles WHERE isdel=0 AND huanghou=1", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Convert.ToInt32(mySQLDataReader["huanghounum"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static List<SearchRoleData> QueryHuangFeiDataList(DBManager dbMgr)
		{
			List<SearchRoleData> list = new List<SearchRoleData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT rid, rname, sex, occupation, level, zoneid, faction, bhname FROM t_roles WHERE isdel=0 AND huanghou=1", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					SearchRoleData item = new SearchRoleData
					{
						RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
						RoleName = Global.FormatRoleName(Convert.ToInt32(mySQLDataReader["zoneid"].ToString()), mySQLDataReader["rname"].ToString()),
						RoleSex = Convert.ToInt32(mySQLDataReader["sex"].ToString()),
						Level = Convert.ToInt32(mySQLDataReader["level"].ToString()),
						Occupation = Convert.ToInt32(mySQLDataReader["occupation"].ToString()),
						Faction = Convert.ToInt32(mySQLDataReader["faction"].ToString()),
						BHName = mySQLDataReader["bhname"].ToString()
					};
					list.Add(item);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static HuangDiTeQuanItem LoadHuangDiTeQuan(DBManager dbMgr)
		{
			HuangDiTeQuanItem result = null;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT Id, tolaofangdayid, tolaofangnum, offlaofangdayid, offlaofangnum, bancatdayid, bancatnum FROM t_hdtequan WHERE Id=1", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = new HuangDiTeQuanItem
					{
						ID = Convert.ToInt32(mySQLDataReader["Id"].ToString()),
						ToLaoFangDayID = Convert.ToInt32(mySQLDataReader["tolaofangdayid"].ToString()),
						ToLaoFangNum = Convert.ToInt32(mySQLDataReader["tolaofangnum"].ToString()),
						OffLaoFangDayID = Convert.ToInt32(mySQLDataReader["offlaofangdayid"].ToString()),
						OffLaoFangNum = Convert.ToInt32(mySQLDataReader["offlaofangnum"].ToString()),
						BanCatDayID = Convert.ToInt32(mySQLDataReader["bancatdayid"].ToString()),
						BanCatNum = Convert.ToInt32(mySQLDataReader["bancatnum"].ToString())
					};
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static List<int> GetNoMoneyBangHuiList(DBManager dbMgr, int maxQiLevel)
		{
			List<int> list = new List<int>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT bhid FROM t_banghui WHERE tongqian<0 and isdel=0 and qilevel<{0}", maxQiLevel);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					list.Add(Convert.ToInt32(mySQLDataReader["bhid"].ToString()));
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static List<QizhenGeBuItemData> QueryQizhenGeBuItemDataList(DBManager dbMgr)
		{
			List<QizhenGeBuItemData> list = new List<QizhenGeBuItemData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT r.rid, r.rname, r.zoneid, q.goodsid, q.goodsnum FROM t_roles AS r, t_qizhengebuy AS q WHERE q.rid=r.rid ORDER BY buytime DESC LIMIT 10", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int num = 0;
				while (mySQLDataReader.Read() && num < 100)
				{
					QizhenGeBuItemData item = new QizhenGeBuItemData
					{
						RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
						RoleName = Global.FormatRoleName(mySQLDataReader["zoneid"].ToString(), mySQLDataReader["rname"].ToString()),
						GoodsID = Convert.ToInt32(mySQLDataReader["goodsid"].ToString()),
						GoodsNum = Convert.ToInt32(mySQLDataReader["goodsnum"].ToString())
					};
					list.Add(item);
					num++;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static int QueryQiangGouBuyItemNumByRoleID(DBManager dbMgr, int roleID, int goodsID, int qiangGouID, int random, int actStartDay)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = "";
				if (random <= 0)
				{
					text = string.Format("SELECT SUM(goodsnum) AS totalgoodsnum FROM t_qianggoubuy WHERE rid={0} and goodsid={1} and qianggouid={2} and actstartday={3}", new object[]
					{
						roleID,
						goodsID,
						qiangGouID,
						actStartDay
					});
				}
				else
				{
					DateTime now = DateTime.Now;
					string text2 = string.Format("{0:0000}-{1:00}-{2:00} 00:01:01", now.Year, now.Month, now.Day);
					string text3 = string.Format("{0:0000}-{1:00}-{2:00} 23:59:59", now.Year, now.Month, now.Day);
					text = string.Format("SELECT SUM(goodsnum) AS totalgoodsnum FROM t_qianggoubuy WHERE rid={0} and goodsid={1} and qianggouid={2} and buytime>='{3}' and buytime<='{4}'", new object[]
					{
						roleID,
						goodsID,
						qiangGouID,
						text2,
						text3
					});
				}
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					try
					{
						result = Convert.ToInt32(mySQLDataReader["totalgoodsnum"].ToString());
					}
					catch (Exception)
					{
						result = 0;
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
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
			return result;
		}

		public static int QueryQiangGouBuyItemNum(DBManager dbMgr, int goodsID, int qiangGouID, int random, int actStartDay)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = "";
				if (random <= 0)
				{
					text = string.Format("SELECT SUM(goodsnum) AS totalgoodsnum FROM t_qianggoubuy WHERE goodsid={0} and qianggouid={1} and actstartday={2}", goodsID, qiangGouID, actStartDay);
				}
				else
				{
					DateTime now = DateTime.Now;
					string text2 = string.Format("{0:0000}-{1:00}-{2:00} 00:01:01", now.Year, now.Month, now.Day);
					string text3 = string.Format("{0:0000}-{1:00}-{2:00} 23:59:59", now.Year, now.Month, now.Day);
					text = string.Format("SELECT SUM(goodsnum) AS totalgoodsnum FROM t_qianggoubuy WHERE goodsid={0} and qianggouid={1} and buytime>='{2}' and buytime<='{3}'", new object[]
					{
						goodsID,
						qiangGouID,
						text2,
						text3
					});
				}
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					try
					{
						result = Convert.ToInt32(mySQLDataReader["totalgoodsnum"].ToString());
					}
					catch (Exception)
					{
						result = 0;
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
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
			return result;
		}

		public static List<ShengXiaoGuessHistory> QueryShengXiaoGuessHistoryDataList(DBManager dbMgr, int roleID = -1)
		{
			List<ShengXiaoGuessHistory> list = new List<ShengXiaoGuessHistory>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = " Where gainnum>0 ";
				if (roleID > 0)
				{
					text += string.Format(" and rid={0} ", roleID);
				}
				string text2 = string.Format("SELECT * FROM t_shengxiaoguesshist {0} ORDER BY guesstime DESC LIMIT 15", text);
				MySQLCommand mySQLCommand = new MySQLCommand(text2, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int num = 0;
				while (mySQLDataReader.Read() && num < 100)
				{
					ShengXiaoGuessHistory item = new ShengXiaoGuessHistory
					{
						RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
						RoleName = Global.FormatRoleName(mySQLDataReader["zoneid"].ToString(), mySQLDataReader["rname"].ToString()),
						GuessKey = Convert.ToInt32(mySQLDataReader["guesskey"].ToString()),
						Mortgage = Convert.ToInt32(mySQLDataReader["mortgage"].ToString()),
						ResultKey = Convert.ToInt32(mySQLDataReader["resultkey"].ToString()),
						GainNum = Convert.ToInt32(mySQLDataReader["gainnum"].ToString()),
						LeftMortgage = Convert.ToInt32(mySQLDataReader["leftmortgage"].ToString()),
						GuessTime = mySQLDataReader["guesstime"].ToString()
					};
					list.Add(item);
					num++;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text2), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static int QueryPingTaiIDByHuoDongID(DBManager dbMgr, int huodongID, int rid, int pingTaiID)
		{
			int result = -1;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT ptid FROM t_usedlipinma WHERE huodongid={0} AND rid={1} AND ptid={2}", huodongID, rid, pingTaiID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Convert.ToInt32(mySQLDataReader["ptid"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int QueryUseNumByHuoDongID(DBManager dbMgr, int huodongID, int rid, int pingTaiID)
		{
			int result = -1;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT count(ptid) as ptidCount FROM t_usedlipinma WHERE huodongid={0} AND rid={1} AND ptid={2}", huodongID, rid, pingTaiID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Convert.ToInt32(mySQLDataReader["ptidCount"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int QueryTotalChongZhiMoney(DBManager dbMgr, string userID, int zoneID)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT SUM(amount) AS totalmoney FROM t_inputlog WHERE u='{0}'", userID, zoneID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					try
					{
						result = Convert.ToInt32(mySQLDataReader["totalmoney"].ToString());
					}
					catch (Exception)
					{
						result = 0;
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
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
			return result;
		}

		public static int QueryChargeMoney(DBManager dbMgr, string userID, int zoneID, int addmoney)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT count(amount) as num FROM t_inputlog WHERE u='{0}' AND amount={2}", userID, zoneID, addmoney);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					try
					{
						result = Convert.ToInt32(mySQLDataReader["num"].ToString());
					}
					catch (Exception)
					{
						result = 0;
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
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
			return result;
		}

		public static int QueryLastScanInputLogID(DBManager dbMgr)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT lastid FROM t_inputhist WHERE Id=1", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					try
					{
						result = Convert.ToInt32(mySQLDataReader["lastid"].ToString());
					}
					catch (Exception)
					{
						result = 0;
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
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
			return result;
		}

		public static int ScanInputLogFromTable(DBManager dbMgr, int lastScanID)
		{
			int num = lastScanID;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT Id, amount, u, inputtime, zoneid FROM t_inputlog WHERE Id>{0} and result='success'", lastScanID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					num = Math.Max(num, Convert.ToInt32(mySQLDataReader["Id"].ToString()));
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return num;
		}

		public static long QueryServerTotalUserMoney()
		{
			long singleLong;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				singleLong = myDbConnection.GetSingleLong("SELECT IFNULL(SUM(money),0) as money FROM t_money", 0, new MySQLParameter[0]);
			}
			return singleLong;
		}

		public static List<InputKingPaiHangData> GetUserInputPaiHang(DBManager dbMgr, string startTime, string endTime, int maxPaiHang = 3)
		{
			List<InputKingPaiHangData> list = new List<InputKingPaiHangData>();
			List<InputKingPaiHangData> result;
			if (maxPaiHang < 1)
			{
				result = list;
			}
			else
			{
				MySQLConnection mySQLConnection = null;
				try
				{
					mySQLConnection = dbMgr.DBConns.PopDBConnection();
					string text = string.Format("SELECT u, sum(totalmoney) as totalmoney, max(time) from\r\n                    (\r\n                    SELECT u, sum(amount) as totalmoney, max(time) as time from t_inputlog where t_inputlog.u IN (select DISTINCT  userid from t_roles where t_roles.isdel=0) and inputtime>='{0}' and inputtime<='{1}' and result='success' \r\n                    GROUP by u   \r\n                    union ALL\r\n                    SELECT u, sum(amount) as totalmoney, max(time) as time from t_inputlog2 where t_inputlog2.u IN (select DISTINCT  userid from t_roles where t_roles.isdel=0) and inputtime>='{0}' and inputtime<='{1}' and result='success' \r\n                    GROUP by u  \r\n                    ) a group by u order by  sum(totalmoney) desc,max(time) ASC limit {2};", startTime, endTime, maxPaiHang);
					MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					int num = 0;
					string paiHangTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					while (mySQLDataReader.Read())
					{
						num++;
						string userID = mySQLDataReader["u"].ToString();
						int paiHangValue = Convert.ToInt32(mySQLDataReader["totalmoney"].ToString());
						InputKingPaiHangData item = new InputKingPaiHangData
						{
							UserID = userID,
							PaiHang = num,
							PaiHangTime = paiHangTime,
							PaiHangValue = paiHangValue
						};
						list.Add(item);
					}
					Comparison<InputKingPaiHangData> comparison = new Comparison<InputKingPaiHangData>(DBQuery.InputKingPaiHangDataCompare);
					list.Sort(comparison);
					for (int i = 0; i < list.Count; i++)
					{
						list[i].PaiHang = i + 1;
						list[i].PaiHangValue = Global.TransMoneyToYuanBao(list[i].PaiHangValue);
					}
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
					mySQLCommand.Dispose();
				}
				finally
				{
					if (null != mySQLConnection)
					{
						dbMgr.DBConns.PushDBConnection(mySQLConnection);
					}
				}
				result = list;
			}
			return result;
		}

		private static int InputKingPaiHangDataCompare(InputKingPaiHangData left, InputKingPaiHangData right)
		{
			return right.PaiHangValue - left.PaiHangValue;
		}

		public static int GetUserInputMoney(DBManager dbMgr, string userid, int zoneid, string startTime, string endTime)
		{
			int num = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT u, sum(amount) as totalmoney, max(time) as time from t_inputlog where inputtime>='{0}' and inputtime<='{1}' and u='{2}' and result='success' GROUP by u  union  SELECT u, sum(amount) as totalmoney, max(time) as time from t_inputlog2 where inputtime>='{0}' and inputtime<='{1}' and u='{2}' and result='success' GROUP by u ", new object[]
				{
					startTime,
					endTime,
					userid,
					zoneid
				});
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					num += Convert.ToInt32(mySQLDataReader["totalmoney"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return num;
		}

		public static Dictionary<int, int> GetUserDanBiInputMoneyCount(DBManager dbMgr, string userid, int zoneid, string startTime, string endTime)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(" SELECT amount,SUM(moneyCount) AS moneyTotalCount  from (SELECT amount, COUNT(amount)  as moneyCount from t_inputlog where inputtime>='{0}' and inputtime<='{1}' and u='{2}' and result='success' GROUP by amount UNION SELECT amount, COUNT(amount) as moneyCount from t_inputlog2 where inputtime>='{0}' and inputtime<='{1}' and u='{2}' and result='success' GROUP by amount) moneyChargeInfo GROUP BY amount", new object[]
				{
					startTime,
					endTime,
					userid,
					zoneid
				});
				MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
				while (mySQLDataReader.Read())
				{
					int key = Global.TransMoneyToYuanBao(Convert.ToInt32(mySQLDataReader["amount"].ToString()));
					int value = Convert.ToInt32(mySQLDataReader["moneyTotalCount"].ToString());
					dictionary[key] = value;
				}
			}
			return dictionary;
		}

		public static int GetAwardHistoryForRole(DBManager dbMgr, int rid, int zoneid, int activitytype, string keystr, out int hasgettimes, out string lastgettime)
		{
			hasgettimes = 0;
			lastgettime = "";
			int result = -1;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT hasgettimes, lastgettime from t_huodongawardrolehist where rid={0} and zoneid={1} and activitytype={2} and keystr='{3}' ", new object[]
				{
					rid,
					zoneid,
					activitytype,
					keystr
				});
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					hasgettimes = Convert.ToInt32(mySQLDataReader["hasgettimes"].ToString());
					lastgettime = mySQLDataReader["lastgettime"].ToString();
					result = 0;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int GetAwardHistoryForUser(DBManager dbMgr, string userid, int activitytype, string keystr, out long hasgettimes, out string lastgettime)
		{
			hasgettimes = 0L;
			lastgettime = "";
			int result = -1;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT hasgettimes, lastgettime from t_huodongawarduserhist where userid='{0}' and activitytype={1} and keystr='{2}' ", userid, activitytype, keystr);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					hasgettimes = Convert.ToInt64(mySQLDataReader["hasgettimes"].ToString());
					lastgettime = mySQLDataReader["lastgettime"].ToString();
					result = 0;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int GetAwardHistoryForUser(DBManager dbMgr, string userid, int activitytype, string keystr, out int hasgettimes, out string lastgettime)
		{
			hasgettimes = 0;
			lastgettime = "";
			int result = -1;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT hasgettimes, lastgettime from t_huodongawarduserhist where userid='{0}' and activitytype={1} and keystr='{2}' ", userid, activitytype, keystr);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					hasgettimes = Convert.ToInt32(mySQLDataReader["hasgettimes"].ToString());
					lastgettime = mySQLDataReader["lastgettime"].ToString();
					result = 0;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static List<HuoDongPaiHangData> GetActivityPaiHangListNearMidTime(DBManager dbMgr, int huoDongType, string midTime, int maxPaiHang = 10)
		{
			List<HuoDongPaiHangData> list = new List<HuoDongPaiHangData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				string text = DateTime.Parse(midTime).AddHours(-36.0).ToString();
				string text2 = DateTime.Parse(midTime).AddHours(36.0).ToString();
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text3 = string.Format("SELECT rid, rname, zoneid, type, paihang, phvalue, paihangtime, ABS(datediff(paihangtime, '{0}')) as diff  from t_huodongpaihang where type={1} and paihangtime<='{2}' and paihangtime>='{3}' ORDER by diff ASC, paihangtime desc, paihang ASC LIMIT 0, {4}", new object[]
				{
					midTime,
					huoDongType,
					text2,
					text,
					maxPaiHang
				});
				MySQLCommand mySQLCommand = new MySQLCommand(text3, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				string text4 = "";
				while (mySQLDataReader.Read())
				{
					HuoDongPaiHangData huoDongPaiHangData = new HuoDongPaiHangData();
					huoDongPaiHangData.RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString());
					huoDongPaiHangData.RoleName = mySQLDataReader["rname"].ToString();
					huoDongPaiHangData.ZoneID = Convert.ToInt32(mySQLDataReader["zoneid"].ToString());
					huoDongPaiHangData.Type = Convert.ToInt32(mySQLDataReader["type"].ToString());
					huoDongPaiHangData.PaiHang = Convert.ToInt32(mySQLDataReader["paihang"].ToString());
					huoDongPaiHangData.PaiHangValue = Convert.ToInt32(mySQLDataReader["phvalue"].ToString());
					huoDongPaiHangData.PaiHangTime = mySQLDataReader["paihangtime"].ToString();
					if (string.IsNullOrEmpty(text4))
					{
						text4 = huoDongPaiHangData.PaiHangTime;
					}
					else if (string.Compare(text4, huoDongPaiHangData.PaiHangTime) != 0)
					{
						break;
					}
					list.Add(huoDongPaiHangData);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text3), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static int QueryLimitGoodsUsedNumByRoleID(DBManager dbMgr, int roleID, int goodsID, out int dayID, out int usedNum)
		{
			dayID = 0;
			usedNum = 0;
			int result = -1;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT dayid, usednum FROM t_limitgoodsbuy WHERE rid={0} AND goodsid={1}", roleID, goodsID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					dayID = Convert.ToInt32(mySQLDataReader["dayid"].ToString());
					usedNum = Convert.ToInt32(mySQLDataReader["usednum"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
				result = 0;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static List<MailData> GetMailItemDataList(DBManager dbMgr, int rid)
		{
			List<MailData> list = new List<MailData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT mailid,senderrid,senderrname,sendtime,receiverrid,reveiverrname,readtime,isread, mailtype,hasfetchattachment,subject,content,yinliang,tongqian,yuanbao from t_mail where receiverrid={0} ORDER by sendtime desc limit 100", rid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					MailData item = new MailData
					{
						MailID = Convert.ToInt32(mySQLDataReader["mailid"].ToString()),
						SenderRID = Convert.ToInt32(mySQLDataReader["senderrid"].ToString()),
						SenderRName = mySQLDataReader["senderrname"].ToString(),
						SendTime = mySQLDataReader["sendtime"].ToString(),
						ReceiverRID = Convert.ToInt32(mySQLDataReader["receiverrid"].ToString()),
						ReveiverRName = mySQLDataReader["reveiverrname"].ToString(),
						ReadTime = mySQLDataReader["readtime"].ToString(),
						IsRead = Convert.ToInt32(mySQLDataReader["isread"].ToString()),
						MailType = Convert.ToInt32(mySQLDataReader["mailtype"].ToString()),
						Hasfetchattachment = Convert.ToInt32(mySQLDataReader["hasfetchattachment"].ToString()),
						Subject = mySQLDataReader["subject"].ToString(),
						Content = "",
						Yinliang = Convert.ToInt32(mySQLDataReader["yinliang"].ToString()),
						Tongqian = Convert.ToInt32(mySQLDataReader["tongqian"].ToString()),
						YuanBao = Convert.ToInt32(mySQLDataReader["yuanbao"].ToString())
					};
					list.Add(item);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static int GetMailItemDataCount(DBManager dbMgr, int rid, int excludeReadState = 0, int limitCount = 1)
		{
			MySQLConnection mySQLConnection = null;
			int num = 0;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT mailid from t_mail where receiverrid={0} and isread<>{1} LIMIT 0,{2}", rid, excludeReadState, limitCount);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					num++;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return num;
		}

		public static MailData GetMailItemData(DBManager dbMgr, int rid, int mailID)
		{
			MailData mailData = null;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT mailid,senderrid,senderrname,sendtime,receiverrid,reveiverrname,readtime,isread, mailtype,hasfetchattachment,subject,content,yinliang,tongqian,yuanbao from t_mail where receiverrid={0} and mailid={1} ORDER by sendtime desc", rid, mailID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					mailData = new MailData
					{
						MailID = Convert.ToInt32(mySQLDataReader["mailid"].ToString()),
						SenderRID = Convert.ToInt32(mySQLDataReader["senderrid"].ToString()),
						SenderRName = mySQLDataReader["senderrname"].ToString(),
						SendTime = mySQLDataReader["sendtime"].ToString(),
						ReceiverRID = Convert.ToInt32(mySQLDataReader["receiverrid"].ToString()),
						ReveiverRName = mySQLDataReader["reveiverrname"].ToString(),
						ReadTime = mySQLDataReader["readtime"].ToString(),
						IsRead = Convert.ToInt32(mySQLDataReader["isread"].ToString()),
						MailType = Convert.ToInt32(mySQLDataReader["mailtype"].ToString()),
						Hasfetchattachment = Convert.ToInt32(mySQLDataReader["hasfetchattachment"].ToString()),
						Subject = mySQLDataReader["subject"].ToString(),
						Content = Global.GetSysEncoding().GetString((byte[])mySQLDataReader["content"]),
						Yinliang = Convert.ToInt32(mySQLDataReader["yinliang"].ToString()),
						Tongqian = Convert.ToInt32(mySQLDataReader["tongqian"].ToString()),
						YuanBao = Convert.ToInt32(mySQLDataReader["yuanbao"].ToString())
					};
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			if (null != mailData)
			{
				mailData.GoodsList = DBQuery.GetMailGoodsDataList(dbMgr, mailID);
			}
			return mailData;
		}

		public static List<MailGoodsData> GetMailGoodsDataList(DBManager dbMgr, int mailID)
		{
			List<MailGoodsData> list = new List<MailGoodsData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT id,mailid,goodsid,forge_level,quality,Props,gcount,binding,origholenum,rmbholenum,jewellist,addpropindex,bornindex,lucky,\r\n                                                        strong,excellenceinfo,appendproplev,equipchangelife from t_mailgoods where mailid={0}", mailID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					MailGoodsData item = new MailGoodsData
					{
						Id = Convert.ToInt32(mySQLDataReader["id"].ToString()),
						MailID = Convert.ToInt32(mySQLDataReader["mailid"].ToString()),
						GoodsID = Convert.ToInt32(mySQLDataReader["goodsid"].ToString()),
						Forge_level = Convert.ToInt32(mySQLDataReader["forge_level"].ToString()),
						Quality = Convert.ToInt32(mySQLDataReader["quality"].ToString()),
						Props = mySQLDataReader["Props"].ToString(),
						GCount = Convert.ToInt32(mySQLDataReader["gcount"].ToString()),
						Binding = Convert.ToInt32(mySQLDataReader["binding"].ToString()),
						OrigHoleNum = Convert.ToInt32(mySQLDataReader["origholenum"].ToString()),
						RMBHoleNum = Convert.ToInt32(mySQLDataReader["rmbholenum"].ToString()),
						Jewellist = mySQLDataReader["jewellist"].ToString(),
						AddPropIndex = Convert.ToInt32(mySQLDataReader["addpropindex"].ToString()),
						BornIndex = Convert.ToInt32(mySQLDataReader["bornindex"].ToString()),
						Lucky = Convert.ToInt32(mySQLDataReader["lucky"].ToString()),
						Strong = Convert.ToInt32(mySQLDataReader["strong"].ToString()),
						ExcellenceInfo = Convert.ToInt32(mySQLDataReader["excellenceinfo"].ToString()),
						AppendPropLev = Convert.ToInt32(mySQLDataReader["appendproplev"].ToString()),
						EquipChangeLifeLev = Convert.ToInt32(mySQLDataReader["equipchangelife"].ToString())
					};
					list.Add(item);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static Dictionary<int, int> ScanLastMailIDListFromTable(DBManager dbMgr)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT MAX(mailid) as mailid, receiverrid from t_mailtemp  GROUP by mailid,receiverrid ORDER by receiverrid asc limit 0, 20", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					int key = Convert.ToInt32(mySQLDataReader["receiverrid"].ToString());
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, Convert.ToInt32(mySQLDataReader["mailid"].ToString()));
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return dictionary;
		}

		public static Dictionary<int, FuMoMailData> GetFuMoMailCached(DBManager dbMgr)
		{
			MySQLConnection mySQLConnection = null;
			Dictionary<int, FuMoMailData> dictionary = new Dictionary<int, FuMoMailData>();
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				MySQLCommand mySQLCommand = new MySQLCommand(SqlDefineManager.SelectAllMailList, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					FuMoMailData fuMoMailData = new FuMoMailData
					{
						MaillID = Convert.ToInt32(mySQLDataReader["maillid"].ToString()),
						SenderRID = Convert.ToInt32(mySQLDataReader["senderrid"].ToString()),
						SenderRName = mySQLDataReader["senderrname"].ToString(),
						SenderJob = Convert.ToInt32(mySQLDataReader["senderjob"].ToString()),
						SendTime = mySQLDataReader["sendtime"].ToString(),
						ReceiverRID = Convert.ToInt32(mySQLDataReader["receiverrid"].ToString()),
						IsRead = Convert.ToInt32(mySQLDataReader["isread"].ToString()),
						ReadTime = mySQLDataReader["readtime"].ToString(),
						FuMoMoney = Convert.ToInt32(mySQLDataReader["fumomoney"].ToString()),
						Content = Global.GetSysEncoding().GetString((byte[])mySQLDataReader["content"])
					};
					dictionary[fuMoMailData.MaillID] = fuMoMailData;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", SqlDefineManager.SelectAllMailList), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return dictionary;
		}

		public static Dictionary<int, Dictionary<int, FuMoMailTemp>> GetAllMap(MySQLDataReader reader)
		{
			Dictionary<int, Dictionary<int, FuMoMailTemp>> dictionary = new Dictionary<int, Dictionary<int, FuMoMailTemp>>();
			Dictionary<int, FuMoMailTemp> dictionary2 = new Dictionary<int, FuMoMailTemp>();
			int num = 0;
			while (reader.Read())
			{
				FuMoMailTemp fuMoMailTemp = new FuMoMailTemp
				{
					TodayID = Convert.ToInt32(reader["tid"].ToString()),
					SenderRID = Convert.ToInt32(reader["senderid"].ToString()),
					ReceiverRID = reader["recid_list"].ToString(),
					Accept = Convert.ToInt32(reader["accept"].ToString()),
					Give = Convert.ToInt32(reader["give"].ToString())
				};
				num = fuMoMailTemp.TodayID;
				dictionary2.Add(fuMoMailTemp.SenderRID, fuMoMailTemp);
			}
			if (num != 0)
			{
				dictionary.Add(num, dictionary2);
			}
			return dictionary;
		}

		public static Dictionary<int, Dictionary<int, FuMoMailTemp>> GetFuMoMailTempCached(DBManager dbMgr)
		{
			string allSql = string.Format(SqlDefineManager.SelectMapStartServer, TimeUtil.GetOffsetDayNow());
			return SqlDefineManager.SqlHandler<Dictionary<int, Dictionary<int, FuMoMailTemp>>>(allSql, new Global.SQLDelegate<Dictionary<int, Dictionary<int, FuMoMailTemp>>>(DBQuery.GetAllMap));
		}

		public static Dictionary<int, FuMoMailTemp> GetFuMoMailMapDataList(DBManager dbMgr, int rid, int nDate)
		{
			Dictionary<int, FuMoMailTemp> dictionary = new Dictionary<int, FuMoMailTemp>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format(SqlDefineManager.SelectMapList, nDate, rid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					FuMoMailTemp fuMoMailTemp = new FuMoMailTemp
					{
						TodayID = Convert.ToInt32(mySQLDataReader["tid"].ToString()),
						SenderRID = Convert.ToInt32(mySQLDataReader["senderid"].ToString()),
						ReceiverRID = mySQLDataReader["recid_list"].ToString(),
						Accept = Convert.ToInt32(mySQLDataReader["accept"].ToString()),
						Give = Convert.ToInt32(mySQLDataReader["give"].ToString())
					};
					dictionary.Add(fuMoMailTemp.SenderRID, fuMoMailTemp);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return dictionary;
		}

		public static int GetMailMaxIDFromTable(DBManager dbMgr)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				MySQLCommand mySQLCommand = new MySQLCommand(SqlDefineManager.SelectMaxMailIndex, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Global.SafeConvertToInt32(mySQLDataReader["mymaxvalue"].ToString(), 10);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", SqlDefineManager.SelectMaxMailIndex), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int GetMailMaxConutFromTable(DBManager dbMgr, int rid)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format(SqlDefineManager.SelectMailCount, rid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Global.SafeConvertToInt32(mySQLDataReader["mymaxvalue"].ToString(), 10);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static Dictionary<int, RebornStampData> GetAllRebornCached(MySQLDataReader reader)
		{
			Dictionary<int, RebornStampData> dictionary = new Dictionary<int, RebornStampData>();
			while (reader.Read())
			{
				RebornStampData rebornStampData = new RebornStampData
				{
					RoleID = Convert.ToInt32(reader["rid"].ToString()),
					ResetNum = Convert.ToInt32(reader["reset"].ToString()),
					UsePoint = Convert.ToInt32(reader["use_point"].ToString()),
					StampInfo = RebornStampManager.UnMakeYinJiUpdateInfo(reader["stamp"].ToString())
				};
				dictionary.Add(rebornStampData.RoleID, rebornStampData);
			}
			return dictionary;
		}

		public static Dictionary<int, RebornStampData> GetRebornYinJiCached(DBManager dbMgr)
		{
			return SqlDefineManager.SqlHandler<Dictionary<int, RebornStampData>>(SqlDefineManager.SelectRebornYinJiAll, new Global.SQLDelegate<Dictionary<int, RebornStampData>>(DBQuery.GetAllRebornCached));
		}

		public static int GetRoleIDByRoleName(DBManager dbMgr, string roleName, int zoneid)
		{
			int num = -1;
			int result;
			if (string.IsNullOrWhiteSpace(roleName))
			{
				result = num;
			}
			else
			{
				MySQLConnection mySQLConnection = null;
				try
				{
					mySQLConnection = dbMgr.DBConns.PopDBConnection();
					string text = string.Format("SELECT rid from t_roles WHERE rname='{0}' and zoneid={1}", roleName, zoneid);
					MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					if (mySQLDataReader.Read())
					{
						num = Convert.ToInt32(mySQLDataReader["rid"].ToString());
					}
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
					mySQLCommand.Dispose();
				}
				finally
				{
					if (null != mySQLConnection)
					{
						dbMgr.DBConns.PushDBConnection(mySQLConnection);
					}
				}
				result = num;
			}
			return result;
		}

		public static int GetMaxMailID(DBManager dbMgr)
		{
			int result = -1;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT MAX(mailid) as mymaxvalue from t_mail", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Global.SafeConvertToInt32(mySQLDataReader["mymaxvalue"].ToString(), 10);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int GetMaxRoleID(DBManager dbMgr)
		{
			int result = -1;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT MAX(rid) as mymaxvalue from t_roles", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Global.SafeConvertToInt32(mySQLDataReader["mymaxvalue"].ToString(), 10);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int GetMaxBangHuiID(DBManager dbMgr)
		{
			int result = -1;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT MAX(bhid) as mymaxvalue from t_banghui", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Global.SafeConvertToInt32(mySQLDataReader["mymaxvalue"].ToString(), 10);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int GetMaxQiangGouItemID(DBManager dbMgr)
		{
			int result = -1;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT MAX(Id) as mymaxvalue from t_qianggouitem", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Global.SafeConvertToInt32(mySQLDataReader["mymaxvalue"].ToString(), 10);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static List<ZaJinDanHistory> QueryZaJinDanHistoryDataList(DBManager dbMgr, int roleID = -1)
		{
			List<ZaJinDanHistory> list = new List<ZaJinDanHistory>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = "";
				if (roleID > 0)
				{
					text += string.Format(" Where rid={0} ", roleID);
				}
				else
				{
					text += string.Format(" Where gaingoodsnum>0 ", new object[0]);
				}
				string text2 = string.Format("SELECT * FROM t_zajindanhist {0} ORDER BY operationtime DESC LIMIT 50", text);
				MySQLCommand mySQLCommand = new MySQLCommand(text2, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int num = 0;
				while (mySQLDataReader.Read() && num < 100)
				{
					ZaJinDanHistory item = new ZaJinDanHistory
					{
						RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
						RoleName = Global.FormatRoleName(mySQLDataReader["zoneid"].ToString(), mySQLDataReader["rname"].ToString()),
						TimesSelected = Convert.ToInt32(mySQLDataReader["timesselected"].ToString()),
						UsedYuanBao = Convert.ToInt32(mySQLDataReader["usedyuanbao"].ToString()),
						UsedJinDan = Convert.ToInt32(mySQLDataReader["usedjindan"].ToString()),
						GainGoodsId = Convert.ToInt32(mySQLDataReader["gaingoodsid"].ToString()),
						GainGoodsNum = Convert.ToInt32(mySQLDataReader["gaingoodsnum"].ToString()),
						GainGold = Convert.ToInt32(mySQLDataReader["gaingold"].ToString()),
						GainYinLiang = Convert.ToInt32(mySQLDataReader["gainyinliang"].ToString()),
						GainExp = Convert.ToInt32(mySQLDataReader["gainexp"].ToString()),
						GoodPorp = mySQLDataReader["strprop"].ToString(),
						OperationTime = mySQLDataReader["operationtime"].ToString()
					};
					list.Add(item);
					num++;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text2), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static int GetFirstChongZhiDaLiNum(DBManager dbMgr, string userID)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT COUNT(rid) AS totalnum from t_roles WHERE userid='{0}' and cztaskid>0", userID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				try
				{
					if (mySQLDataReader.Read())
					{
						result = Convert.ToInt32(mySQLDataReader["totalnum"].ToString());
					}
				}
				catch (Exception)
				{
					result = 0;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
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
			return result;
		}

		public static int GetKaiFuOnlineAwardRoleID(DBManager dbMgr, int dayID, out int totalRoleNum)
		{
			totalRoleNum = 0;
			int result = -1;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType("KaiFuOnlineDayID", null);
				string text = string.Format("SELECT rid,{0} FROM {1} WHERE {2}={3}", new object[]
				{
					roleParamType.ColumnName,
					roleParamType.TableName,
					roleParamType.IdxName,
					roleParamType.KeyString
				});
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				List<int> list = new List<int>();
				try
				{
					while (mySQLDataReader.Read())
					{
						int num = Global.SafeConvertToInt32(mySQLDataReader[1].ToString(), 10);
						if (num >= dayID)
						{
							list.Add(Global.SafeConvertToInt32(mySQLDataReader["rid"].ToString(), 10));
						}
					}
				}
				catch (Exception)
				{
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
				mySQLCommand = null;
				if (list.Count > 0)
				{
					Random random = new Random();
					result = list[random.Next(0, list.Count)];
					totalRoleNum = list.Count;
				}
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static List<KaiFuOnlineAwardData> GetKaiFuOnlineAwardDataList(DBManager dbMgr, int zoneID)
		{
			List<KaiFuOnlineAwardData> list = new List<KaiFuOnlineAwardData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT K.rid, R.zoneid, R.rname, K.dayid, K.totalrolenum FROM t_kfonlineawards AS K, t_roles AS R WHERE K.rid=R.rid AND K.zoneid={0}", zoneID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				try
				{
					while (mySQLDataReader.Read())
					{
						list.Add(new KaiFuOnlineAwardData
						{
							RoleID = Global.SafeConvertToInt32(mySQLDataReader["rid"].ToString(), 10),
							ZoneID = Global.SafeConvertToInt32(mySQLDataReader["zoneid"].ToString(), 10),
							RoleName = mySQLDataReader["rname"].ToString(),
							DayID = Global.SafeConvertToInt32(mySQLDataReader["dayid"].ToString(), 10),
							TotalRoleNum = Global.SafeConvertToInt32(mySQLDataReader["totalrolenum"].ToString(), 10)
						});
					}
				}
				catch (Exception)
				{
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
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
			return list;
		}

		public static void ScanGMMsgFromTable(DBManager dbMgr, List<string> msgList)
		{
			MySQLConnection mySQLConnection = null;
			try
			{
				int num = 0;
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = "SELECT id, msg FROM t_gmmsg";
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					num = Math.Max(Convert.ToInt32(mySQLDataReader["id"].ToString()), num);
					string @string = Global.GetSysEncoding().GetString((byte[])mySQLDataReader["msg"]);
					msgList.Add(@string);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
				if (num > 0)
				{
					text = string.Format("DELETE FROM t_gmmsg WHERE id<={0}", num);
					mySQLCommand = new MySQLCommand(text, mySQLConnection);
					mySQLCommand.ExecuteNonQuery();
					mySQLCommand.Dispose();
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				}
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
		}

		public static List<InputKingPaiHangData> GetUserUsedMoneyPaiHang1(DBManager dbMgr, string startTime, string endTime, int maxPaiHang = 3)
		{
			List<InputKingPaiHangData> list = new List<InputKingPaiHangData>();
			List<InputKingPaiHangData> result;
			if (maxPaiHang < 1)
			{
				result = list;
			}
			else
			{
				MySQLConnection mySQLConnection = null;
				try
				{
					mySQLConnection = dbMgr.DBConns.PopDBConnection();
					string text = string.Format("SELECT t_mallbuy.rid, sum(t_mallbuy.totalprice) as totalmoney, max(t_mallbuy.buytime) as time from t_mallbuy,t_roles  where t_mallbuy.rid=t_roles.rid and buytime>='{0}' and buytime<='{1}' and t_roles.isdel=0 GROUP by rid  union  SELECT t_zajindanhist.rid, sum(usedyuanbao/timesselected) as totalmoney, max(operationtime) as time from t_zajindanhist,t_roles where t_zajindanhist.rid=t_roles.rid and t_roles.isdel=0 and operationtime>='{0}' and operationtime<='{1}' GROUP by rid  union  SELECT t_qizhengebuy.rid, sum(totalprice) as totalmoney, max(buytime) as time from t_qizhengebuy,t_roles where buytime>='{0}' and buytime<='{1}' and t_qizhengebuy.rid=t_roles.rid and t_roles.isdel=0  GROUP by rid order by totalmoney desc,time asc  limit 0, {2} ", startTime, endTime, maxPaiHang * 3);
					MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					List<int> list2 = new List<int>();
					int num = 0;
					string paiHangTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					while (mySQLDataReader.Read())
					{
						num++;
						int item = Convert.ToInt32(mySQLDataReader["rid"].ToString());
						int num2 = (int)Convert.ToDouble(mySQLDataReader["totalmoney"].ToString());
						if (num2 > 0)
						{
							if (!list2.Contains(item))
							{
								list2.Add(item);
								InputKingPaiHangData inputKingPaiHangData = new InputKingPaiHangData
								{
									UserID = item.ToString(),
									PaiHang = num,
									PaiHangTime = paiHangTime,
									PaiHangValue = num2
								};
								list.Add(inputKingPaiHangData);
							}
							else
							{
								InputKingPaiHangData inputKingPaiHangData = list[list2.IndexOf(item)];
								inputKingPaiHangData.PaiHangValue += num2;
							}
						}
						if (list.Count >= maxPaiHang)
						{
							break;
						}
					}
					Comparison<InputKingPaiHangData> comparison = new Comparison<InputKingPaiHangData>(DBQuery.InputKingPaiHangDataCompare);
					list.Sort(comparison);
					for (int i = 0; i < list.Count; i++)
					{
						list[i].PaiHang = i + 1;
					}
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
					mySQLCommand.Dispose();
				}
				finally
				{
					if (null != mySQLConnection)
					{
						dbMgr.DBConns.PushDBConnection(mySQLConnection);
					}
				}
				result = list;
			}
			return result;
		}

		public static int GetUserUsedMoney1(DBManager dbMgr, int rid, string startTime, string endTime)
		{
			int num = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT rid, sum(totalprice) as totalmoney, max(buytime) as time from t_mallbuy where buytime>='{0}' and buytime<='{1}' and rid={2} GROUP by rid  union  SELECT rid, sum(usedyuanbao/timesselected) as totalmoney, max(operationtime) as time from t_zajindanhist where operationtime>='{0}' and operationtime<='{1}' and rid={2} GROUP by rid  union  SELECT rid, sum(totalprice) as totalmoney, max(buytime) as time from t_qizhengebuy where buytime>='{0}' and buytime<='{1}' and rid={2} GROUP by rid", startTime, endTime, rid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					num += (int)Convert.ToDouble(mySQLDataReader["totalmoney"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return num;
		}

		public static List<YueDuChouJiangData> QueryYueDuChouJiangHistoryDataList(DBManager dbMgr, int roleID = -1)
		{
			List<YueDuChouJiangData> list = new List<YueDuChouJiangData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = "";
				if (roleID > 0)
				{
					text += string.Format(" Where rid={0}", roleID);
				}
				else
				{
					text += string.Format(" Where gaingoodsnum>0 ", new object[0]);
				}
				string text2 = string.Format("SELECT * FROM t_yueduchoujianghist {0} ORDER BY operationtime DESC LIMIT 50", text);
				MySQLCommand mySQLCommand = new MySQLCommand(text2, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int num = 0;
				while (mySQLDataReader.Read() && num < 100)
				{
					YueDuChouJiangData item = new YueDuChouJiangData
					{
						RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
						RoleName = Global.FormatRoleName(mySQLDataReader["zoneid"].ToString(), mySQLDataReader["rname"].ToString()),
						GainGoodsId = Convert.ToInt32(mySQLDataReader["gaingoodsid"].ToString()),
						GainGoodsNum = Convert.ToInt32(mySQLDataReader["gaingoodsnum"].ToString()),
						GainGold = Convert.ToInt32(mySQLDataReader["gaingold"].ToString()),
						GainYinLiang = Convert.ToInt32(mySQLDataReader["gainyinliang"].ToString()),
						GainExp = Convert.ToInt32(mySQLDataReader["gainexp"].ToString()),
						OperationTime = mySQLDataReader["operationtime"].ToString()
					};
					list.Add(item);
					num++;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text2), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static int GetBloodCastleEnterCount(DBManager dbMgr, int roleid, int nDate, int activityid)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT triggercount from t_dayactivityinfo where roleid={0} and activityid={1} and timeinfo={2} ", roleid, activityid, nDate);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Convert.ToInt32(mySQLDataReader["triggercount"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static List<int> GetDayActivityTotlePoint(DBManager dbMgr, int activityid)
		{
			List<int> list = new List<int>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT roleid, totalpoint FROM t_dayactivityinfo WHERE totalpoint>0 AND activityid = {0} ORDER BY totalpoint DESC LIMIT 1", activityid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				int num = -1;
				int num2 = -1;
				if (mySQLDataReader.Read())
				{
					num = Convert.ToInt32(mySQLDataReader["roleid"].ToString());
					num2 = (int)Math.Min(2147483647L, Convert.ToInt64(mySQLDataReader["totalpoint"].ToString()));
				}
				if (num != -1 && num2 != -1)
				{
					list.Add(num);
					list.Add(num2);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static int GetRoleDayActivityPoint(DBManager dbMgr, int nRole, int activityid)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT totalpoint FROM t_dayactivityinfo WHERE roleid = {0} AND activityid = {1}", nRole, activityid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = (int)Math.Min(2147483647L, Convert.ToInt64(mySQLDataReader["totalpoint"].ToString()));
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int QueryPlayerAdmiredAnother(DBManager dbMgr, int roleAID, int roleBID, int nDate)
		{
			int result = -1;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT adorationroleid from t_adorationinfo where roleid={0} and adorationroleid={1} and dayid={2}", roleAID, roleBID, nDate);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Convert.ToInt32(mySQLDataReader["adorationroleid"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static List<int> QueryPlayerEveryDayOnLineAwardGiftInfo(DBManager dbMgr, int roleID)
		{
			List<int> list = new List<int>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT everydayonlineawardstep, geteverydayonlineawarddayid from t_huodong where roleid={0}", roleID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					int item = Convert.ToInt32(mySQLDataReader["everydayonlineawardstep"].ToString());
					list.Add(item);
					item = Convert.ToInt32(mySQLDataReader["geteverydayonlineawarddayid"].ToString());
					list.Add(item);
				}
				else
				{
					list = null;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static List<PushMessageData> QueryPushMsgUerList(DBManager dbMgr, int nCondition)
		{
			List<PushMessageData> list = new List<PushMessageData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				DateTime now = DateTime.Now;
				string text = string.Format("SELECT userid, pushid, lastlogintime from t_pushmessageinfo where NOW() <= ADDDATE(lastlogintime, {0})", nCondition + 1);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					PushMessageData item = new PushMessageData
					{
						UserID = mySQLDataReader["userid"].ToString(),
						PushID = mySQLDataReader["pushid"].ToString(),
						LastLoginTime = mySQLDataReader["lastlogintime"].ToString()
					};
					list.Add(item);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static Dictionary<int, int> QueryMoJingExchangeDict(DBManager dbMgr, int nRoleid, int nDayID)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT exchangeid, exchangenum FROM t_mojingexchangeinfo WHERE roleid = {0} AND dayid = {1}", nRoleid, nDayID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					int key = Convert.ToInt32(mySQLDataReader["exchangeid"].ToString());
					int value = Convert.ToInt32(mySQLDataReader["exchangenum"].ToString());
					dictionary.Add(key, value);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return dictionary;
		}

		public static Dictionary<int, OldResourceInfo> QueryResourceGetInfo(DBManager dbMgr, int nRoleid)
		{
			Dictionary<int, OldResourceInfo> dictionary = new Dictionary<int, OldResourceInfo>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				DateTime now = DateTime.Now;
				string text = string.Format("SELECT type, exp, leftCount,mojing,bandmoney,zhangong,chengjiu,shengwang,bangzuan,xinghun,yuansufenmo from t_resourcegetinfo where roleid = {0} AND hasget = {1}", nRoleid, 0);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					OldResourceInfo oldResourceInfo = new OldResourceInfo
					{
						type = Global.SafeConvertToInt32(mySQLDataReader["type"].ToString(), 10),
						exp = ((Global.SafeConvertToInt32(mySQLDataReader["exp"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(mySQLDataReader["exp"].ToString(), 10) : 0),
						leftCount = ((Global.SafeConvertToInt32(mySQLDataReader["leftCount"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(mySQLDataReader["leftCount"].ToString(), 10) : 0),
						mojing = ((Global.SafeConvertToInt32(mySQLDataReader["mojing"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(mySQLDataReader["mojing"].ToString(), 10) : 0),
						bandmoney = ((Global.SafeConvertToInt32(mySQLDataReader["bandmoney"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(mySQLDataReader["bandmoney"].ToString(), 10) : 0),
						zhangong = ((Global.SafeConvertToInt32(mySQLDataReader["zhangong"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(mySQLDataReader["zhangong"].ToString(), 10) : 0),
						chengjiu = ((Global.SafeConvertToInt32(mySQLDataReader["chengjiu"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(mySQLDataReader["chengjiu"].ToString(), 10) : 0),
						shengwang = ((Global.SafeConvertToInt32(mySQLDataReader["shengwang"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(mySQLDataReader["shengwang"].ToString(), 10) : 0),
						bandDiamond = ((Global.SafeConvertToInt32(mySQLDataReader["bangzuan"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(mySQLDataReader["bangzuan"].ToString(), 10) : 0),
						xinghun = ((Global.SafeConvertToInt32(mySQLDataReader["xinghun"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(mySQLDataReader["xinghun"].ToString(), 10) : 0),
						yuanSuFenMo = ((Global.SafeConvertToInt32(mySQLDataReader["yuansufenmo"].ToString(), 10) > 0) ? Global.SafeConvertToInt32(mySQLDataReader["yuansufenmo"].ToString(), 10) : 0),
						roleId = nRoleid
					};
					dictionary[oldResourceInfo.type] = oldResourceInfo;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return dictionary;
		}

		public static int GetUserUsedMoney(DBManager dbMgr, int rid, string startTime, string endTime)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT rid, sum(amount) as totalmoney  from t_consumelog where cdate>='{0}' and cdate<='{1}' and rid={2} GROUP by rid ", startTime, endTime, rid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					result = (int)Convert.ToDouble(mySQLDataReader["totalmoney"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static List<InputKingPaiHangData> GetUserUsedMoneyPaiHang(DBManager dbMgr, string startTime, string endTime, int maxPaiHang = 3)
		{
			List<InputKingPaiHangData> list = new List<InputKingPaiHangData>();
			List<InputKingPaiHangData> result;
			if (maxPaiHang < 1)
			{
				result = list;
			}
			else
			{
				MySQLConnection mySQLConnection = null;
				try
				{
					mySQLConnection = dbMgr.DBConns.PopDBConnection();
					string text = string.Format("SELECT t_consumelog.rid, sum(t_consumelog.amount) as totalmoney, max(cdate) as time from t_consumelog,t_roles  where t_consumelog.rid=t_roles.rid and cdate>='{0}' and cdate<='{1}' and t_roles.isdel=0 GROUP by rid  order by totalmoney desc,time asc  limit 0, {2} ", startTime, endTime, maxPaiHang * 2);
					MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					List<int> list2 = new List<int>();
					int num = 0;
					string paiHangTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
					while (mySQLDataReader.Read())
					{
						num++;
						int item = Convert.ToInt32(mySQLDataReader["rid"].ToString());
						int num2 = (int)Convert.ToDouble(mySQLDataReader["totalmoney"].ToString());
						if (num2 > 0)
						{
							if (!list2.Contains(item))
							{
								list2.Add(item);
								InputKingPaiHangData inputKingPaiHangData = new InputKingPaiHangData
								{
									UserID = item.ToString(),
									PaiHang = num,
									PaiHangTime = paiHangTime,
									PaiHangValue = num2
								};
								list.Add(inputKingPaiHangData);
							}
							else
							{
								InputKingPaiHangData inputKingPaiHangData = list[list2.IndexOf(item)];
								inputKingPaiHangData.PaiHangValue += num2;
							}
						}
						if (list.Count >= maxPaiHang)
						{
							break;
						}
					}
					Comparison<InputKingPaiHangData> comparison = new Comparison<InputKingPaiHangData>(DBQuery.InputKingPaiHangDataCompare);
					list.Sort(comparison);
					for (int i = 0; i < list.Count; i++)
					{
						list[i].PaiHang = i + 1;
					}
					GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
					mySQLCommand.Dispose();
				}
				finally
				{
					if (null != mySQLConnection)
					{
						dbMgr.DBConns.PushDBConnection(mySQLConnection);
					}
				}
				result = list;
			}
			return result;
		}

		public static int QueryVipLevelAwardFlagInfo(DBManager dbMgr, int nRoldID, int nZoneID)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT vipawardflag from t_roles WHERE rid = '{0}' and zoneid={1}", nRoldID, nZoneID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				try
				{
					if (mySQLDataReader.Read())
					{
						result = Convert.ToInt32(mySQLDataReader["vipawardflag"].ToString());
					}
				}
				catch (Exception)
				{
					result = 0;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
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
			return result;
		}

		public static int QueryVipLevelAwardFlagInfoByUserID(DBManager dbMgr, string struseid, int nRoleID, int nZoneID)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT vipawardflag FROM t_roles WHERE userid = '{0}' and zoneid = {1} and rid != {2}", struseid, nZoneID, nRoleID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				try
				{
					if (mySQLDataReader.Read())
					{
						if (Convert.ToInt32(mySQLDataReader["vipawardflag"].ToString()) > 0)
						{
							result = Convert.ToInt32(mySQLDataReader["vipawardflag"].ToString());
						}
					}
				}
				catch (Exception)
				{
					result = 0;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
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
			return result;
		}

		public static int LastLoginRole(DBManager dbMgr, string uid)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT rid FROM t_roles WHERE userid = '{0}' AND isdel=0   ORDER BY lasttime DESC LIMIT 1 ", uid);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				try
				{
					if (mySQLDataReader.Read())
					{
						if (Convert.ToInt32(mySQLDataReader["rid"].ToString()) > 0)
						{
							result = Convert.ToInt32(mySQLDataReader["rid"].ToString());
						}
					}
				}
				catch (Exception)
				{
					result = 0;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
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
			return result;
		}

		public static bool GetUserRole(DBManager dbMgr, string userID, int roleID)
		{
			MySQLConnection mySQLConnection = null;
			bool result = false;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT rid FROM t_roles WHERE userid = '{0}' AND rid={1} AND isdel=0", userID, roleID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				try
				{
					if (mySQLDataReader.Read())
					{
						if (Convert.ToInt32(mySQLDataReader["rid"].ToString()) == roleID)
						{
							result = true;
						}
					}
				}
				catch (Exception)
				{
					result = false;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
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
			return result;
		}

		public static MarriageData GetMarriageData(DBManager dbMgr, int nRoleID)
		{
			MySQLConnection mySQLConnection = null;
			MarriageData result = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT spouseid, marrytype, ringid, goodwillexp, goodwillstar, goodwilllevel, givenrose, lovemessage, autoreject,changtime FROM t_marry WHERE roleid = {0}", nRoleID);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				try
				{
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					if (mySQLDataReader.Read())
					{
						result = new MarriageData
						{
							nSpouseID = Global.SafeConvertToInt32(mySQLDataReader["spouseid"].ToString(), 10),
							byMarrytype = Convert.ToSByte(mySQLDataReader["marrytype"].ToString()),
							nRingID = Global.SafeConvertToInt32(mySQLDataReader["ringid"].ToString(), 10),
							nGoodwillexp = Global.SafeConvertToInt32(mySQLDataReader["goodwillexp"].ToString(), 10),
							byGoodwillstar = Convert.ToSByte(mySQLDataReader["goodwillstar"].ToString()),
							byGoodwilllevel = Convert.ToSByte(mySQLDataReader["goodwilllevel"].ToString()),
							nGivenrose = Global.SafeConvertToInt32(mySQLDataReader["givenrose"].ToString(), 10),
							strLovemessage = mySQLDataReader["lovemessage"].ToString(),
							byAutoReject = Convert.ToSByte(mySQLDataReader["autoreject"].ToString()),
							ChangTime = mySQLDataReader["changtime"].ToString()
						};
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
			return result;
		}

		public static void QueryMarryPartyList(DBManager dbMgr, Dictionary<int, MarryPartyData> partyList)
		{
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT roleid, partytype, joincount, starttime, husbandid, wifeid, (SELECT rname FROM t_roles WHERE rid = husbandid) AS husbandname, (SELECT rname FROM t_roles WHERE rid = wifeid) AS wifename FROM t_marryparty", new object[0]);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				try
				{
					MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
					while (mySQLDataReader.Read())
					{
						MarryPartyData marryPartyData = new MarryPartyData
						{
							RoleID = Convert.ToInt32(mySQLDataReader["roleid"].ToString()),
							PartyType = Convert.ToInt32(mySQLDataReader["partytype"].ToString()),
							JoinCount = Convert.ToInt32(mySQLDataReader["joincount"].ToString()),
							StartTime = DataHelper.ConvertToTicks(mySQLDataReader["starttime"].ToString()),
							HusbandRoleID = Convert.ToInt32(mySQLDataReader["husbandid"].ToString()),
							WifeRoleID = Convert.ToInt32(mySQLDataReader["wifeid"].ToString()),
							HusbandName = mySQLDataReader["husbandname"].ToString(),
							WifeName = mySQLDataReader["wifename"].ToString()
						};
						partyList.Add(marryPartyData.RoleID, marryPartyData);
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

		public static bool CheckOrderNo(DBManager dbMgr, string order_no)
		{
			MySQLConnection mySQLConnection = null;
			bool result = true;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("select * from `t_order` where order_no='{0}';", order_no);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				result = mySQLDataReader.Read();
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static bool CheckInputLogOrderNo(DBManager dbMgr, string order_no)
		{
			MySQLConnection mySQLConnection = null;
			bool result = true;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("select * from `t_inputlog` where order_no='{0}';", order_no);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				result = mySQLDataReader.Read();
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static bool CheckInputLog2OrderNo(DBManager dbMgr, string order_no)
		{
			MySQLConnection mySQLConnection = null;
			bool result = true;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("select * from `t_inputlog2` where order_no='{0}';", order_no);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				result = mySQLDataReader.Read();
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static List<TianTiLogItemData> GetTianTiLogItemDataList(DBManager dbMgr, int roleId, int maxCount)
		{
			List<TianTiLogItemData> list = new List<TianTiLogItemData>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT zoneid1,rolename1,zoneid2,rolename2,success,duanweijifenaward,rongyaoaward,endtime from t_kf_tianti_game_log where rid={0} order by endtime desc limit {1}", roleId, maxCount);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					TianTiLogItemData tianTiLogItemData = new TianTiLogItemData();
					tianTiLogItemData.ZoneId1 = Convert.ToInt32(mySQLDataReader["zoneid1"].ToString());
					tianTiLogItemData.RoleName1 = mySQLDataReader["rolename1"].ToString();
					tianTiLogItemData.ZoneId2 = Convert.ToInt32(mySQLDataReader["zoneid2"].ToString());
					tianTiLogItemData.RoleName2 = mySQLDataReader["rolename2"].ToString();
					tianTiLogItemData.Success = Convert.ToInt32(mySQLDataReader["success"].ToString());
					tianTiLogItemData.DuanWeiJiFenAward = Convert.ToInt32(mySQLDataReader["duanweijifenaward"].ToString());
					tianTiLogItemData.RongYaoAward = Convert.ToInt32(mySQLDataReader["rongyaoaward"].ToString());
					DateTime.TryParse(mySQLDataReader["endtime"].ToString(), out tianTiLogItemData.EndTime);
					list.Add(tianTiLogItemData);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static List<TianTiLogItemData> GetT5v5ItemDataList(DBManager dbMgr, int roleId, int maxCount)
		{
			List<TianTiLogItemData> list = new List<TianTiLogItemData>();
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("SELECT zoneid1,rolename1,zoneid2,rolename2,success,duanweijifenaward,rongyaoaward,endtime from t_5v5_game_log where rid={0} order by endtime desc limit {1}", roleId, maxCount);
				MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
				while (mySQLDataReader.Read())
				{
					TianTiLogItemData tianTiLogItemData = new TianTiLogItemData();
					tianTiLogItemData.ZoneId1 = Convert.ToInt32(mySQLDataReader["zoneid1"].ToString());
					tianTiLogItemData.RoleName1 = mySQLDataReader["rolename1"].ToString();
					tianTiLogItemData.ZoneId2 = Convert.ToInt32(mySQLDataReader["zoneid2"].ToString());
					tianTiLogItemData.RoleName2 = mySQLDataReader["rolename2"].ToString();
					tianTiLogItemData.Success = Convert.ToInt32(mySQLDataReader["success"].ToString());
					tianTiLogItemData.DuanWeiJiFenAward = Convert.ToInt32(mySQLDataReader["duanweijifenaward"].ToString());
					tianTiLogItemData.RongYaoAward = Convert.ToInt32(mySQLDataReader["rongyaoaward"].ToString());
					DateTime.TryParse(mySQLDataReader["endtime"].ToString(), out tianTiLogItemData.EndTime);
					list.Add(tianTiLogItemData);
				}
			}
			return list;
		}

		public static List<GroupMailData> ScanNewGroupMailFromTable(DBManager dbMgr, int beginID)
		{
			List<GroupMailData> list = null;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string arg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				string text = string.Format("SELECT * from t_groupmail where gmailid > {0} and endtime > '{1}'", beginID, arg);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					GroupMailData groupMailData = new GroupMailData();
					groupMailData.GMailID = Convert.ToInt32(mySQLDataReader["gmailid"].ToString());
					groupMailData.Subject = mySQLDataReader["subject"].ToString();
					groupMailData.Content = mySQLDataReader["content"].ToString();
					groupMailData.Conditions = mySQLDataReader["conditions"].ToString();
					groupMailData.InputTime = DateTime.Parse(mySQLDataReader["inputtime"].ToString()).Ticks;
					groupMailData.EndTime = DateTime.Parse(mySQLDataReader["endtime"].ToString()).Ticks;
					groupMailData.Yinliang = Convert.ToInt32(mySQLDataReader["yinliang"].ToString());
					groupMailData.Tongqian = Convert.ToInt32(mySQLDataReader["tongqian"].ToString());
					groupMailData.YuanBao = Convert.ToInt32(mySQLDataReader["yuanbao"].ToString());
					groupMailData.GoodsList = Global.ParseGoodsDataList(mySQLDataReader["goodlist"].ToString());
					if (null == list)
					{
						list = new List<GroupMailData>();
					}
					list.Add(groupMailData);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static bool IsBlackUserID(DBManager dbMgr, string userid)
		{
			bool result;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("SELECT count(*) from t_blackuserid where userid='{0}' limit 1;", userid);
				result = (myDbConnection.GetSingleInt(sql, 0, new MySQLParameter[0]) > 0);
			}
			return result;
		}

		public static RoleMiniInfo QueryRoleMiniInfo(long rid)
		{
			RoleMiniInfo roleMiniInfo = null;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("SELECT zoneid,userid from t_roles where rid={0};", rid);
				MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
				if (mySQLDataReader.Read())
				{
					roleMiniInfo = new RoleMiniInfo();
					roleMiniInfo.roleId = rid;
					roleMiniInfo.zoneId = Convert.ToInt32(mySQLDataReader["zoneid"].ToString());
					roleMiniInfo.userId = mySQLDataReader["userid"].ToString();
				}
			}
			return roleMiniInfo;
		}

		public static List<TenAwardData> ScanNewGroupTenFromTable(DBManager dbMgr)
		{
			List<TenAwardData> list = null;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT * FROM t_ten WHERE state=0 ORDER BY id LIMIT 100", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					TenAwardData tenAwardData = new TenAwardData();
					tenAwardData.DbID = Convert.ToInt32(mySQLDataReader["id"].ToString());
					tenAwardData.RoleID = Convert.ToInt32(mySQLDataReader["roleID"].ToString());
					tenAwardData.AwardID = Convert.ToInt32(mySQLDataReader["giftID"].ToString());
					tenAwardData.UserID = mySQLDataReader["uID"].ToString();
					if (null == list)
					{
						list = new List<TenAwardData>();
					}
					list.Add(tenAwardData);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static int TenOnlyNum(DBManager dbMgr, string userID, int awardID)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT COUNT(*) AS totalnum FROM t_ten WHERE giftID='{0}' and uID='{1}' and state>1", awardID, userID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Convert.ToInt32(mySQLDataReader["totalnum"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int TenDayNum(DBManager dbMgr, string userID, int awardID)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				string text = string.Format("{0:yyyy-MM-dd 00:00:00}", DateTime.Now);
				string text2 = string.Format("{0:yyyy-MM-dd 23:59:59}", DateTime.Now);
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text3 = string.Format("SELECT COUNT(*) AS totalnum FROM t_ten WHERE giftID='{0}' and uID='{1}' and state>1 and updatetime>='{2}' and updatetime<='{3}';", new object[]
				{
					awardID,
					userID,
					text,
					text2
				});
				MySQLCommand mySQLCommand = new MySQLCommand(text3, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Convert.ToInt32(mySQLDataReader["totalnum"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text3), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static bool ActivateStateGet(DBManager dbMgr, string userID)
		{
			bool result = false;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT * FROM t_activate WHERE userID='{0}' LIMIT 1;", userID);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = true;
				}
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static List<GiftCodeAwardData> ScanNewGiftCodeFromTable(DBManager dbMgr)
		{
			List<GiftCodeAwardData> list = null;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT * FROM t_giftcode WHERE mailid=0 ORDER BY id asc LIMIT 100", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					GiftCodeAwardData giftCodeAwardData = new GiftCodeAwardData();
					giftCodeAwardData.Dbid = Convert.ToInt32(mySQLDataReader["id"].ToString());
					giftCodeAwardData.UserId = mySQLDataReader["userid"].ToString();
					giftCodeAwardData.RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString());
					giftCodeAwardData.GiftId = mySQLDataReader["giftid"].ToString();
					giftCodeAwardData.CodeNo = mySQLDataReader["codeno"].ToString();
					if (null == list)
					{
						list = new List<GiftCodeAwardData>();
					}
					list.Add(giftCodeAwardData);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static List<FacebookAwardData> ScanNewGroupFacebookFromTable(DBManager dbMgr)
		{
			List<FacebookAwardData> list = null;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT * FROM t_facebook WHERE state=0 ORDER BY id LIMIT 100", new object[0]);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					FacebookAwardData facebookAwardData = new FacebookAwardData();
					facebookAwardData.DbID = Convert.ToInt32(mySQLDataReader["id"].ToString());
					facebookAwardData.RoleID = Convert.ToInt32(mySQLDataReader["roleID"].ToString());
					facebookAwardData.AwardID = Convert.ToInt32(mySQLDataReader["giftID"].ToString());
					if (null == list)
					{
						list = new List<FacebookAwardData>();
					}
					list.Add(facebookAwardData);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return list;
		}

		public static int FacebookOnlyNum(DBManager dbMgr, int roleID, int awardID)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format("SELECT COUNT(*) AS totalnum FROM t_facebook WHERE giftID={0} and roleID={1} and state>1", awardID, roleID);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Convert.ToInt32(mySQLDataReader["totalnum"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int FacebookDayNum(DBManager dbMgr, int roleID, int awardID)
		{
			int result = 0;
			MySQLConnection mySQLConnection = null;
			try
			{
				string text = string.Format("{0:yyyy-MM-dd 00:00:00}", DateTime.Now);
				string text2 = string.Format("{0:yyyy-MM-dd 23:59:59}", DateTime.Now);
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text3 = string.Format("SELECT COUNT(*) AS totalnum FROM t_facebook WHERE giftID={0} and roleID={1} and state>1 and updatetime>='{2}' and updatetime<='{3}';", new object[]
				{
					awardID,
					roleID,
					text,
					text2
				});
				MySQLCommand mySQLCommand = new MySQLCommand(text3, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					result = Convert.ToInt32(mySQLDataReader["totalnum"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text3), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static bool GetMinRegtime(DBManager dbMgr, string userid, out string OutUserid, out string Regtime)
		{
			OutUserid = "0";
			Regtime = "0";
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.RegressGetMinRegtime, userid);
				MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
				if (mySQLDataReader.Read())
				{
					OutUserid = mySQLDataReader["userid"].ToString();
					Regtime = mySQLDataReader["regtime"].ToString();
					return true;
				}
			}
			return false;
		}

		public static bool GetRegressAwardHistoryForUser(DBManager dbMgr, string userid, int activitytype, string stage, out Dictionary<string, string> SignData)
		{
			SignData = new Dictionary<string, string>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format(SqlDefineManager.RegressGetSignData, userid, activitytype, stage);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					string key = mySQLDataReader["lastgettime"].ToString();
					string value = mySQLDataReader["activitydata"].ToString();
					if (SignData.ContainsKey(key))
					{
						SignData[key] = value;
					}
					else
					{
						SignData.Add(key, value);
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return true;
		}

		public static int GetRegressAwardDayHistoryForUser(DBManager dbMgr, string userid, int activitytype, string keystr, string stage, out string lastgettime, out int hasgettimes, out string activitydata)
		{
			int result = -1;
			lastgettime = "";
			hasgettimes = 0;
			activitydata = "";
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format(SqlDefineManager.RegressGetDaySignData, new object[]
				{
					userid,
					activitytype,
					keystr,
					stage
				});
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					lastgettime = mySQLDataReader["lastgettime"].ToString();
					hasgettimes = Convert.ToInt32(mySQLDataReader["hasgettimes"].ToString());
					activitydata = mySQLDataReader["activitydata"].ToString();
					result = 1;
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static int QueryUserLimitGoodsUsedNumByRoleID(DBManager dbMgr, string UserID, int goodsID, string stage, out int dayID, out int usedNum)
		{
			dayID = 0;
			usedNum = 0;
			int result = -1;
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format(SqlDefineManager.RegressSelectStore, UserID, goodsID, stage);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					dayID = Convert.ToInt32(mySQLDataReader["dayid"].ToString());
					usedNum = Convert.ToInt32(mySQLDataReader["usednum"].ToString());
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
				result = 0;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return result;
		}

		public static bool QueryUserLimitGoodsUsedNumInfoByRoleID(DBManager dbMgr, string UserID, int dayID, string stage, out Dictionary<int, int> GoodsInfo)
		{
			GoodsInfo = new Dictionary<int, int>();
			MySQLConnection mySQLConnection = null;
			try
			{
				mySQLConnection = dbMgr.DBConns.PopDBConnection();
				string text = string.Format(SqlDefineManager.RegressSelectStoreInfo, UserID, dayID, stage);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					int num = Convert.ToInt32(mySQLDataReader["goodsid"].ToString());
					int num2 = Convert.ToInt32(mySQLDataReader["usednum"].ToString());
					if (GoodsInfo.ContainsKey(num))
					{
						Dictionary<int, int> dictionary;
						int key;
						(dictionary = GoodsInfo)[key = num] = dictionary[key] + num2;
					}
					else
					{
						GoodsInfo.Add(num, num2);
					}
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			finally
			{
				if (null != mySQLConnection)
				{
					dbMgr.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return true;
		}

		public static bool GetAllRebornEquipHole(DBManager dbMgr, int rid, out Dictionary<int, RebornEquipData> data)
		{
			data = new Dictionary<int, RebornEquipData>();
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.RebornEquipHoleSelectInfo, rid);
				MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
				while (mySQLDataReader.Read())
				{
					RebornEquipData rebornEquipData = new RebornEquipData();
					rebornEquipData.RoleID = rid;
					rebornEquipData.HoleID = Convert.ToInt32(mySQLDataReader["holeid"].ToString());
					rebornEquipData.Level = Convert.ToInt32(mySQLDataReader["level"].ToString());
					rebornEquipData.Able = Convert.ToInt32(mySQLDataReader["able"].ToString());
					if (data.ContainsKey(rebornEquipData.HoleID))
					{
						return false;
					}
					data.Add(rebornEquipData.HoleID, rebornEquipData);
				}
			}
			return true;
		}

		public static bool GetMazingerStoreInfo(DBManager dbMgr, int rid, out Dictionary<int, MazingerStoreData> data)
		{
			data = new Dictionary<int, MazingerStoreData>();
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format(SqlDefineManager.MazingerStoreSelectInfo, rid);
				MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
				while (mySQLDataReader.Read())
				{
					MazingerStoreData mazingerStoreData = new MazingerStoreData();
					mazingerStoreData.RoleID = rid;
					mazingerStoreData.Type = Convert.ToInt32(mySQLDataReader["type"].ToString());
					mazingerStoreData.Stage = Convert.ToInt32(mySQLDataReader["stage"].ToString());
					mazingerStoreData.StarLevel = Convert.ToInt32(mySQLDataReader["level"].ToString());
					mazingerStoreData.Exp = Convert.ToInt32(mySQLDataReader["exp"].ToString());
					if (data.ContainsKey(mazingerStoreData.Type))
					{
						return false;
					}
					data.Add(mazingerStoreData.Type, mazingerStoreData);
				}
			}
			return true;
		}
	}
}
