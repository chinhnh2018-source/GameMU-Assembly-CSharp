using System;
using System.Collections.Generic;
using GameDBServer.Core;
using GameDBServer.DB;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic.BoCai
{
	public class BoCaiDBOperator
	{
		public static bool ReplaceOpenLottery(OpenLottery data)
		{
			int num = -1;
			try
			{
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("REPLACE INTO t_bocai_open_lottery(DataPeriods, AllBalance, SurplusBalance, XiaoHaoDaiBi, BocaiType, strWinNum, WinInfo, IsAward) VALUES({0},{1},{2},{3},{4},'{5}','{6}', {7});", new object[]
					{
						data.DataPeriods,
						data.AllBalance,
						data.SurplusBalance,
						data.XiaoHaoDaiBi,
						data.BocaiType,
						data.strWinNum,
						data.WinInfo,
						data.IsAward
					});
					num = myDbConnection.ExecuteNonQuery(sql, 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return num > -1;
		}

		public static long GetMaxData(int BoCaiType)
		{
			long num = -1L;
			MySQLConnection mySQLConnection = null;
			DBManager instance = DBManager.getInstance();
			long result;
			try
			{
				mySQLConnection = instance.DBConns.PopDBConnection();
				string text = string.Format("SELECT MAX(DataPeriods) as DataPeriods from t_bocai_open_lottery WHERE `BocaiType`={0}", BoCaiType);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				if (mySQLDataReader.Read())
				{
					string value = mySQLDataReader["DataPeriods"].ToString();
					if (string.IsNullOrEmpty(value))
					{
						num = (result = 0L);
						return result;
					}
					num = Convert.ToInt64(value);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			finally
			{
				if (null != mySQLConnection)
				{
					instance.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			result = num;
			return result;
		}

		public static void SelectOpenLottery(int bocaiType, out List<OpenLottery> dList)
		{
			MySQLConnection mySQLConnection = null;
			DBManager instance = DBManager.getInstance();
			dList = null;
			try
			{
				mySQLConnection = instance.DBConns.PopDBConnection();
				string text = string.Format("SELECT `DataPeriods`,`XiaoHaoDaiBi`,`strWinNum`,`WinInfo` ,`AllBalance`,`SurplusBalance` FROM t_bocai_open_lottery WHERE `IsAward` < 1 AND `BocaiType`={0};", bocaiType);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				dList = new List<OpenLottery>();
				while (mySQLDataReader.Read())
				{
					OpenLottery item = new OpenLottery
					{
						strWinNum = mySQLDataReader["strWinNum"].ToString(),
						XiaoHaoDaiBi = Convert.ToInt32(mySQLDataReader["XiaoHaoDaiBi"].ToString()),
						WinInfo = mySQLDataReader["WinInfo"].ToString(),
						DataPeriods = Convert.ToInt64(mySQLDataReader["DataPeriods"].ToString()),
						AllBalance = Convert.ToInt64(mySQLDataReader["AllBalance"].ToString()),
						SurplusBalance = Convert.ToInt64(mySQLDataReader["SurplusBalance"].ToString()),
						BocaiType = bocaiType
					};
					dList.Add(item);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			finally
			{
				if (null != mySQLConnection)
				{
					instance.DBConns.PushDBConnection(mySQLConnection);
				}
			}
		}

		public static bool ReplaceBuyBoCai(BuyBoCai2SDB data)
		{
			int num = -1;
			try
			{
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("REPLACE INTO t_bocai_buy_history(rid, RoleName, ZoneID, UserID, ServerID, BuyNum, BuyValue, IsSend, IsWin, BocaiType, DataPeriods, UpdateTime)VALUES({0},'{1}',{2},'{3}',{4}, {5},'{6}', {7}, {8}, {9}, {10}, '{11}');", new object[]
					{
						data.m_RoleID,
						data.m_RoleName,
						data.ZoneID,
						data.strUserID,
						data.ServerId,
						data.BuyNum,
						data.strBuyValue,
						data.IsSend,
						data.IsWin,
						data.BocaiType,
						data.DataPeriods,
						DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
					});
					num = myDbConnection.ExecuteNonQuery(sql, 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return num > -1;
		}

		public static void SelectBuyBoCai(int bocaiType, long DataPeriods, out List<BuyBoCai2SDB> dList, bool isNoSend)
		{
			MySQLConnection mySQLConnection = null;
			DBManager instance = DBManager.getInstance();
			dList = null;
			try
			{
				mySQLConnection = instance.DBConns.PopDBConnection();
				string text;
				if (isNoSend)
				{
					text = string.Format("SELECT `rid`,`RoleName`,`ZoneID`,`UserID`,`ServerID`,`BuyNum`,`BuyValue`,`IsSend`,`IsWin`  FROM t_bocai_buy_history WHERE `BocaiType`={0} AND `DataPeriods`={1} AND `IsSend`={2};", bocaiType, DataPeriods, 0);
				}
				else
				{
					text = string.Format("SELECT `rid`,`RoleName`,`ZoneID`,`UserID`,`ServerID`,`BuyNum`,`BuyValue`,`IsSend`,`IsWin`  FROM t_bocai_buy_history WHERE `BocaiType`={0} AND `DataPeriods`={1};", bocaiType, DataPeriods);
				}
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				dList = new List<BuyBoCai2SDB>();
				while (mySQLDataReader.Read())
				{
					BuyBoCai2SDB item = new BuyBoCai2SDB
					{
						m_RoleName = mySQLDataReader["RoleName"].ToString(),
						strUserID = mySQLDataReader["UserID"].ToString(),
						strBuyValue = mySQLDataReader["BuyValue"].ToString(),
						m_RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString()),
						ZoneID = Convert.ToInt32(mySQLDataReader["ZoneID"].ToString()),
						ServerId = Convert.ToInt32(mySQLDataReader["ServerID"].ToString()),
						BuyNum = Convert.ToInt32(mySQLDataReader["BuyNum"].ToString()),
						IsSend = (Convert.ToInt32(mySQLDataReader["IsSend"].ToString()) > 0),
						IsWin = (Convert.ToInt32(mySQLDataReader["IsWin"].ToString()) > 0),
						BocaiType = bocaiType,
						DataPeriods = DataPeriods
					};
					dList.Add(item);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			finally
			{
				if (null != mySQLConnection)
				{
					instance.DBConns.PushDBConnection(mySQLConnection);
				}
			}
		}

		public static bool ReplaceBoCaiShop(BoCaiShopDB data)
		{
			int num = -1;
			try
			{
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("REPLACE INTO t_bocai_shop(rid, ID, BuyNum, Periods, WuPinID) VALUES({0},{1},{2},'{3}','{4}');", new object[]
					{
						data.RoleID,
						data.ID,
						data.BuyNum,
						TimeUtil.NowDataTimeString("yyMMdd"),
						data.WuPinID
					});
					num = myDbConnection.ExecuteNonQuery(sql, 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return num > -1;
		}

		public static void SelectBoCaiShop(string Periods, out List<BoCaiShopDB> dList)
		{
			dList = null;
			try
			{
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string sql = string.Format("SELECT rid, ID, BuyNum, WuPinID from t_bocai_shop where Periods={0}", Periods);
					MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
					dList = new List<BoCaiShopDB>();
					while (mySQLDataReader.Read())
					{
						BoCaiShopDB boCaiShopDB = new BoCaiShopDB();
						boCaiShopDB.RoleID = Convert.ToInt32(mySQLDataReader["rid"].ToString());
						boCaiShopDB.ID = Convert.ToInt32(mySQLDataReader["ID"].ToString());
						boCaiShopDB.BuyNum = Convert.ToInt32(mySQLDataReader["BuyNum"].ToString());
						boCaiShopDB.WuPinID = mySQLDataReader["WuPinID"].ToString();
						boCaiShopDB.Periods = Convert.ToInt32(Periods);
						dList.Add(boCaiShopDB);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		public static bool DelData(string table, string Sql)
		{
			int num = -1;
			try
			{
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					num = myDbConnection.ExecuteNonQuery(string.Format("delete from {1} where {0}", Sql, table), 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return num > -1;
		}
	}
}
