using System;
using System.Collections.Generic;
using GameDBServer.DB;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic.GoldAuction
{
	public class GoldAuctionDB
	{
		public static bool Insert(GoldAuctionDBItem Item)
		{
			int num = -1;
			try
			{
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string text = DataHelper.ObjectToHexString<List<AuctionRoleData>>(Item.RoleList);
					string text2 = "INSERT INTO t_gold_auction(BuyerData, AuctionTime, AuctionType, AuctionSource, ProductionTime,";
					text2 += "StrGoods, BossLife, KillBossRoleID, UpDBWay, UpdateTime, AttackerList)";
					text2 += " VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', {10})";
					string sql = string.Format(text2, new object[]
					{
						Item.BuyerData.getAttackerValue(),
						Item.AuctionTime,
						Item.AuctionType,
						Item.AuctionSource,
						Item.ProductionTime,
						Item.StrGoods,
						Item.BossLife,
						Item.KillBossRoleID,
						((DBAuctionUpEnum)Item.UpDBWay).ToString(),
						DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
						text
					});
					num = myDbConnection.ExecuteNonQuery(sql, 0);
					LjlLog.WriteLogFormat(LogTypes.Info, new string[]
					{
						"SQL  Insert ",
						(num > -1).ToString()
					});
				}
			}
			catch (Exception ex)
			{
				LjlLog.WriteLog(LogTypes.Exception, ex.ToString(), "");
			}
			return num > -1;
		}

		public static bool Update(GoldAuctionDBItem Item)
		{
			int num = -1;
			try
			{
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					string text = string.Format("UPDATE t_gold_auction SET BuyerData='{0}', AuctionTime='{1}', AuctionType='{2}', UpDBWay='{3}', UpdateTime='{4}' WHERE AuctionSource='{5}' AND ProductionTime='{6}'", new object[]
					{
						Item.BuyerData.getAttackerValue(),
						Item.AuctionTime,
						Item.AuctionType,
						((DBAuctionUpEnum)Item.UpDBWay).ToString(),
						DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
						Item.AuctionSource,
						Item.ProductionTime
					});
					num = myDbConnection.ExecuteNonQuery(text, 0);
					LjlLog.WriteLogFormat(LogTypes.Info, new string[]
					{
						"金团拍卖行更新",
						(num > -1).ToString(),
						"  ",
						text
					});
				}
			}
			catch (Exception ex)
			{
				LjlLog.WriteLog(LogTypes.Exception, ex.ToString(), "");
			}
			return num > -1;
		}

		public static bool DelData(string Sql)
		{
			int num = -1;
			try
			{
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					num = myDbConnection.ExecuteNonQuery(string.Format("delete from t_gold_auction where {0}", Sql), 0);
					LjlLog.WriteLogFormat(LogTypes.Info, new string[]
					{
						"DelData ",
						(num > -1).ToString(),
						" ,delete from t_gold_auction where ",
						Sql
					});
				}
			}
			catch (Exception ex)
			{
				LjlLog.WriteLog(LogTypes.Exception, ex.ToString(), "");
			}
			return num > -1;
		}

		public static bool Select(out List<GoldAuctionDBItem> dList, int type)
		{
			MySQLConnection mySQLConnection = null;
			DBManager instance = DBManager.getInstance();
			dList = new List<GoldAuctionDBItem>();
			try
			{
				mySQLConnection = instance.DBConns.PopDBConnection();
				string format = "SELECT BuyerData, AuctionTime, AuctionType, AuctionSource, ProductionTime, StrGoods, BossLife, KillBossRoleID, AttackerList FROM t_gold_auction WHERE UpDBWay!='Del' AND AuctionType='{0}';";
				MySQLCommand mySQLCommand = new MySQLCommand(string.Format(format, type), mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					GoldAuctionDBItem goldAuctionDBItem = new GoldAuctionDBItem
					{
						AuctionTime = mySQLDataReader["AuctionTime"].ToString(),
						AuctionType = Convert.ToInt32(mySQLDataReader["AuctionType"].ToString()),
						AuctionSource = Convert.ToInt32(mySQLDataReader["AuctionSource"].ToString()),
						ProductionTime = mySQLDataReader["ProductionTime"].ToString(),
						StrGoods = mySQLDataReader["StrGoods"].ToString(),
						BossLife = Convert.ToInt64(mySQLDataReader["BossLife"].ToString()),
						KillBossRoleID = Convert.ToInt32(mySQLDataReader["KillBossRoleID"].ToString())
					};
					string text = mySQLDataReader["BuyerData"].ToString();
					string[] array = text.Split(new char[]
					{
						','
					});
					goldAuctionDBItem.BuyerData = new AuctionRoleData();
					if (6 == array.Length)
					{
						goldAuctionDBItem.BuyerData.m_RoleID = Convert.ToInt32(array[0]);
						goldAuctionDBItem.BuyerData.Value = Convert.ToInt64(array[1]);
						goldAuctionDBItem.BuyerData.m_RoleName = array[2];
						goldAuctionDBItem.BuyerData.ZoneID = Convert.ToInt32(array[3]);
						goldAuctionDBItem.BuyerData.strUserID = array[4];
						goldAuctionDBItem.BuyerData.ServerId = Convert.ToInt32(array[5]);
					}
					goldAuctionDBItem.OldAuctionType = goldAuctionDBItem.AuctionType;
					byte[] array2 = (mySQLDataReader["AttackerList"] as byte[]) ?? new byte[0];
					goldAuctionDBItem.RoleList = DataHelper.BytesToObject<List<AuctionRoleData>>(array2, 0, array2.Length);
					if (null == goldAuctionDBItem.RoleList)
					{
						goldAuctionDBItem.RoleList = new List<AuctionRoleData>();
					}
					dList.Add(goldAuctionDBItem);
				}
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", string.Format(format, type)), EventLevels.Important);
				mySQLCommand.Dispose();
			}
			catch (Exception ex)
			{
				LjlLog.WriteLog(LogTypes.Exception, ex.ToString(), "");
				return false;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					instance.DBConns.PushDBConnection(mySQLConnection);
				}
			}
			return true;
		}

		private const string strTime = "yyyy-MM-dd HH:mm:ss";
	}
}
