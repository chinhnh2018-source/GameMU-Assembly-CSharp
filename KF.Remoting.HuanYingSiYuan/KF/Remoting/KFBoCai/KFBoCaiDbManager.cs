using System;
using System.Collections.Generic;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting.KFBoCai
{
	public class KFBoCaiDbManager
	{
		public static bool InserOpenLottery(OpenLottery data)
		{
			try
			{
				string sqlstring = string.Format("REPLACE INTO t_bocai_open_lottery(DataPeriods, AllBalance, SurplusBalance, XiaoHaoDaiBi, BocaiType, strWinNum, WinInfo) VALUES({0},{1},{2},{3},{4},'{5}','{6}');", new object[]
				{
					data.DataPeriods,
					data.AllBalance,
					data.SurplusBalance,
					data.XiaoHaoDaiBi,
					data.BocaiType,
					data.strWinNum,
					data.WinInfo
				});
				return DbHelperMySQL.ExecuteSql(sqlstring) > -1;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return false;
		}

		public static void SelectOpenLottery(long DataPeriods, int BocaiType, out OpenLottery data)
		{
			data = null;
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT `SurplusBalance`,`XiaoHaoDaiBi`,`strWinNum`,`WinInfo`,`AllBalance` FROM t_bocai_open_lottery WHERE `BocaiType`={0} AND `DataPeriods`={1};", BocaiType, DataPeriods);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				data = new OpenLottery();
				if (mySqlDataReader != null && mySqlDataReader.Read())
				{
					data.AllBalance = Convert.ToInt64(mySqlDataReader["AllBalance"]);
					data.SurplusBalance = Convert.ToInt64(mySqlDataReader["SurplusBalance"]);
					data.XiaoHaoDaiBi = Convert.ToInt32(mySqlDataReader["XiaoHaoDaiBi"]);
					data.strWinNum = mySqlDataReader["strWinNum"].ToString();
					data.WinInfo = mySqlDataReader["WinInfo"].ToString();
					data.BocaiType = BocaiType;
					data.DataPeriods = DataPeriods;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
			}
			finally
			{
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
			}
		}

		public static void SelectOpenLottery(int BocaiType, string cmd, out List<OpenLottery> dList)
		{
			dList = null;
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT `SurplusBalance`,`XiaoHaoDaiBi`,`strWinNum`,`WinInfo`,`AllBalance`,`DataPeriods` FROM t_bocai_open_lottery WHERE `BocaiType`={0}{1}", BocaiType, cmd);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				dList = new List<OpenLottery>();
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					OpenLottery openLottery = new OpenLottery();
					openLottery.DataPeriods = Convert.ToInt64(mySqlDataReader["DataPeriods"]);
					openLottery.AllBalance = Convert.ToInt64(mySqlDataReader["AllBalance"]);
					openLottery.SurplusBalance = Convert.ToInt64(mySqlDataReader["SurplusBalance"]);
					openLottery.XiaoHaoDaiBi = Convert.ToInt32(mySqlDataReader["XiaoHaoDaiBi"]);
					openLottery.strWinNum = mySqlDataReader["strWinNum"].ToString();
					openLottery.WinInfo = mySqlDataReader["WinInfo"].ToString();
					openLottery.BocaiType = BocaiType;
					dList.Add(openLottery);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
			}
			finally
			{
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
			}
		}

		public static long GetMaxPeriods(int BocaiType)
		{
			try
			{
				object single = DbHelperMySQL.GetSingle(string.Format("SELECT MAX(DataPeriods) FROM t_bocai_open_lottery WHERE `BocaiType`={0}", BocaiType));
				if (null == single)
				{
					return 0L;
				}
				return (long)single;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return -1L;
		}

		public static bool InsertLotteryHistory(BoCaiTypeEnum BocaiType, KFBoCaoHistoryData History)
		{
			try
			{
				string sqlstring = string.Format("REPLACE INTO t_bocai_lottery_history(`rid`, `BocaiType`, `DataPeriods`, `ServerID`, `RoleName`, `ZoneID`, `BuyNum`, `WinNo`, `WinMoney`)VALUES({0},{1},{2},{3},'{4}',{5},{6},{7},{8});", new object[]
				{
					History.RoleID,
					BocaiType,
					History.DataPeriods,
					History.ServerID,
					History.RoleName,
					History.ZoneID,
					History.BuyNum,
					History.WinNo,
					History.WinMoney
				});
				return DbHelperMySQL.ExecuteSql(sqlstring) > -1;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return false;
		}

		public static void LoadLotteryHistory(BoCaiTypeEnum BocaiType, out List<KFBoCaoHistoryData> HistoryList, string cmd = "")
		{
			MySqlDataReader mySqlDataReader = null;
			HistoryList = new List<KFBoCaoHistoryData>();
			try
			{
				string strSQL = string.Format("SELECT `rid`,`DataPeriods`,`ServerID`,`RoleName`,`ZoneID`,`BuyNum`,`WinNo`,`WinMoney`FROM t_bocai_lottery_history WHERE `BocaiType`={0} ORDER BY `DataPeriods` DESC, `BuyNum` ASC {1};", BocaiType, cmd);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					KFBoCaoHistoryData kfboCaoHistoryData = new KFBoCaoHistoryData();
					kfboCaoHistoryData.RoleID = Convert.ToInt32(mySqlDataReader["rid"]);
					kfboCaoHistoryData.ZoneID = Convert.ToInt32(mySqlDataReader["ZoneID"]);
					kfboCaoHistoryData.ServerID = Convert.ToInt32(mySqlDataReader["ServerID"]);
					kfboCaoHistoryData.RoleName = mySqlDataReader["RoleName"].ToString();
					kfboCaoHistoryData.BuyNum = Convert.ToInt32(mySqlDataReader["BuyNum"]);
					kfboCaoHistoryData.WinNo = Convert.ToInt32(mySqlDataReader["WinNo"]);
					kfboCaoHistoryData.WinMoney = Convert.ToInt64(mySqlDataReader["WinMoney"]);
					kfboCaoHistoryData.DataPeriods = Convert.ToInt64(mySqlDataReader["DataPeriods"]);
					HistoryList.Add(kfboCaoHistoryData);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
			}
			finally
			{
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
			}
		}

		public static bool InserBuyBocai(long DataPeriods, KFBuyBocaiData buyDaya)
		{
			try
			{
				string sqlstring = string.Format("REPLACE INTO t_bocai_buy_history(`rid`, `BocaiType`, `DataPeriods`, `ServerID`, `RoleName`, `ZoneID`, `BuyNum`, `BuyValue`) VALUES({0},{1},{2},{3},'{4}',{5},{6},'{7}');", new object[]
				{
					buyDaya.RoleID,
					buyDaya.BocaiType,
					DataPeriods,
					buyDaya.ServerID,
					buyDaya.RoleName,
					buyDaya.ZoneID,
					buyDaya.BuyNum,
					buyDaya.BuyValue
				});
				return DbHelperMySQL.ExecuteSql(sqlstring) > -1;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return false;
		}

		public static bool UpdateBuyBocai(long DataPeriods, KFBuyBocaiData buyDaya)
		{
			try
			{
				string sqlstring = string.Format("UPDATE t_bocai_buy_history SET BuyNum='{0}' WHERE rid='{1}' AND BocaiType='{2}' AND DataPeriods='{3}' AND ServerID='{4}' AND BuyValue='{5}'", new object[]
				{
					buyDaya.BuyNum,
					buyDaya.RoleID,
					buyDaya.BocaiType,
					DataPeriods,
					buyDaya.ServerID,
					buyDaya.BuyValue
				});
				return DbHelperMySQL.ExecuteSql(sqlstring) > -1;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return false;
		}

		public static bool LoadBuyHistory(int BocaiType, long DataPeriods, out List<KFBuyBocaiData> HistoryList)
		{
			bool flag = false;
			MySqlDataReader mySqlDataReader = null;
			HistoryList = new List<KFBuyBocaiData>();
			try
			{
				string strSQL = string.Format("SELECT `rid`, `BocaiType`,`ServerID`,`RoleName`,`ZoneID`,`BuyNum`,`BuyValue`FROM t_bocai_buy_history WHERE `DataPeriods`={0};", DataPeriods);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					KFBuyBocaiData kfbuyBocaiData = new KFBuyBocaiData();
					kfbuyBocaiData.RoleID = Convert.ToInt32(mySqlDataReader["rid"]);
					kfbuyBocaiData.ZoneID = Convert.ToInt32(mySqlDataReader["ZoneID"]);
					kfbuyBocaiData.ServerID = Convert.ToInt32(mySqlDataReader["ServerID"]);
					kfbuyBocaiData.RoleName = mySqlDataReader["RoleName"].ToString();
					kfbuyBocaiData.BuyNum = Convert.ToInt32(mySqlDataReader["BuyNum"]);
					kfbuyBocaiData.BocaiType = Convert.ToInt32(mySqlDataReader["BocaiType"]);
					kfbuyBocaiData.BuyValue = mySqlDataReader["BuyValue"].ToString();
					HistoryList.Add(kfbuyBocaiData);
				}
				flag = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
			}
			finally
			{
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
			}
			return flag;
		}

		public static bool ReplaceBoCaiShop(KFBoCaiShopDB data)
		{
			try
			{
				string sqlstring = string.Format("REPLACE INTO t_bocai_shop(ID, BuyNum, Periods, WuPinID) VALUES({0},{1},{2},'{3}');", new object[]
				{
					data.ID,
					data.BuyNum,
					data.Periods,
					data.WuPinID
				});
				return DbHelperMySQL.ExecuteSql(sqlstring) > -1;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return false;
		}

		public static void SelectBoCaiShop(string Periods, out List<KFBoCaiShopDB> dList)
		{
			dList = null;
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT `ID`,`BuyNum`,`WuPinID` FROM t_bocai_shop WHERE `Periods`={0}", Periods);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				dList = new List<KFBoCaiShopDB>();
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					KFBoCaiShopDB kfboCaiShopDB = new KFBoCaiShopDB();
					kfboCaiShopDB.ID = Convert.ToInt32(mySqlDataReader["ID"]);
					kfboCaiShopDB.BuyNum = Convert.ToInt32(mySqlDataReader["BuyNum"]);
					kfboCaiShopDB.WuPinID = mySqlDataReader["WuPinID"].ToString();
					kfboCaiShopDB.Periods = Convert.ToInt32(Periods);
					dList.Add(kfboCaiShopDB);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
			}
			finally
			{
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
			}
		}

		public static bool DelTableData(string table, string Sql)
		{
			try
			{
				string sqlstring = string.Format("delete from {0} where {1}", table, Sql);
				return DbHelperMySQL.ExecuteSql(sqlstring) > -1;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return false;
		}

		public static void StopServer(string str)
		{
			LogManager.WriteLog(2, str, null, true);
			LogManager.WriteLog(1000, str, null, true);
			LogManager.WriteLog(1000, "博彩初始化失败了，停止检查一下,测试阶段可以忽略。任意键继续", null, true);
			Console.ReadKey();
		}

		public static string ListInt2String(List<int> iList)
		{
			string text = "";
			try
			{
				foreach (int num in iList)
				{
					if (string.IsNullOrEmpty(text))
					{
						text = string.Format("{0}", num);
					}
					else
					{
						text = string.Format("{0},{1}", text, num);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			return text;
		}

		public static void String2ListInt(string str, out List<int> iList)
		{
			iList = new List<int>();
			try
			{
				string[] array = str.Split(new char[]
				{
					','
				});
				foreach (string value in array)
				{
					iList.Add(Convert.ToInt32(value));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		private const int result = -1;
	}
}
