using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.Fund
{
	public class FundManager : SingletonTemplate<FundManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13116, SingletonTemplate<FundManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13117, SingletonTemplate<FundManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13118, SingletonTemplate<FundManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13119, SingletonTemplate<FundManager>.Instance());
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
			if (nID == 13116)
			{
				this.FundInfo(client, nID, cmdParams, count);
			}
			else if (nID == 13117)
			{
				this.FundBuy(client, nID, cmdParams, count);
			}
			else if (nID == 13118)
			{
				this.FundAward(client, nID, cmdParams, count);
			}
			else if (nID == 13119)
			{
				this.FundMoney(client, nID, cmdParams, count);
			}
		}

		private void FundInfo(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			List<FundDBItem> list = new List<FundDBItem>();
			MySQLConnection mySQLConnection = null;
			try
			{
				int num = DataHelper.BytesToObject<int>(cmdParams, 0, count);
				string text = string.Format("SELECT zoneID,userID,roleID,fundType,fundID,buyTime,awardID,value1,value2,state from t_fund where state>0 and roleID={0}", num);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					list.Add(new FundDBItem
					{
						ZoneID = int.Parse(mySQLDataReader["zoneID"].ToString()),
						UserID = mySQLDataReader["userID"].ToString(),
						RoleID = int.Parse(mySQLDataReader["roleID"].ToString()),
						FundType = int.Parse(mySQLDataReader["fundType"].ToString()),
						FundID = int.Parse(mySQLDataReader["fundID"].ToString()),
						BuyTime = DateTime.Parse(mySQLDataReader["buyTime"].ToString()),
						AwardID = int.Parse(mySQLDataReader["awardID"].ToString()),
						Value1 = int.Parse(mySQLDataReader["value1"].ToString()),
						Value2 = int.Parse(mySQLDataReader["value2"].ToString()),
						State = int.Parse(mySQLDataReader["state"].ToString())
					});
				}
				mySQLCommand.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			finally
			{
				if (null != mySQLConnection)
				{
					DBManager.getInstance().DBConns.PushDBConnection(mySQLConnection);
				}
			}
			client.sendCmd<List<FundDBItem>>(nID, list);
		}

		private void FundBuy(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool cmdData = false;
			MySQLConnection mySQLConnection = null;
			try
			{
				FundDBItem fundDBItem = DataHelper.BytesToObject<FundDBItem>(cmdParams, 0, count);
				string text = string.Format("UPDATE t_fund SET state='0' where zoneID='{0}' and userID='{1}' and roleID='{2}' and fundType='{3}' and state='1'", new object[]
				{
					fundDBItem.ZoneID,
					fundDBItem.UserID,
					fundDBItem.RoleID,
					fundDBItem.FundType
				});
				string text2 = string.Format("INSERT INTO t_fund(zoneID,userID,roleID,fundType,fundID,buyTime) VALUE('{0}','{1}','{2}','{3}','{4}','{5}')", new object[]
				{
					fundDBItem.ZoneID,
					fundDBItem.UserID,
					fundDBItem.RoleID,
					fundDBItem.FundType,
					fundDBItem.FundID,
					fundDBItem.BuyTime
				});
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text2), EventLevels.Important);
				mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				cmdData = (mySQLCommand.ExecuteNonQuery() > 0);
				mySQLCommand = new MySQLCommand(text2, mySQLConnection);
				mySQLCommand.ExecuteNonQuery();
				mySQLCommand.Dispose();
				cmdData = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				cmdData = false;
			}
			finally
			{
				if (null != mySQLConnection)
				{
					DBManager.getInstance().DBConns.PushDBConnection(mySQLConnection);
				}
			}
			client.sendCmd<bool>(nID, cmdData);
		}

		private void FundAward(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool cmdData = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				FundDBItem fundDBItem = DataHelper.BytesToObject<FundDBItem>(cmdParams, 0, count);
				string sql = string.Format("UPDATE t_fund SET awardID='{0}',state='{1}' where zoneID='{2}' and userID='{3}' and roleID='{4}' and fundType='{5}' and fundID='{6}'", new object[]
				{
					fundDBItem.AwardID,
					fundDBItem.State,
					fundDBItem.ZoneID,
					fundDBItem.UserID,
					fundDBItem.RoleID,
					fundDBItem.FundType,
					fundDBItem.FundID
				});
				cmdData = myDbConnection.ExecuteNonQueryBool(sql, 0);
			}
			client.sendCmd<bool>(nID, cmdData);
		}

		private void FundMoney(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool cmdData = false;
			try
			{
				FundDBItem fundDBItem = DataHelper.BytesToObject<FundDBItem>(cmdParams, 0, count);
				cmdData = this.FundAddMoney(fundDBItem.UserID, fundDBItem.Value1, fundDBItem.RoleID, fundDBItem.Value2);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
			client.sendCmd<bool>(nID, cmdData);
		}

		public bool FundAddMoney(string userID, int moneyAdd, int roleID, int moneyCost = 0)
		{
			bool result = false;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string text = string.Format("UPDATE t_fund SET value1=value1+{0},value2=value2+{1} where userID='{2}' and fundType=3 and state=1", moneyAdd, moneyCost, userID);
				DBRoleInfo dbroleInfo = DBManager.getInstance().GetDBRoleInfo(ref roleID);
				if (dbroleInfo == null)
				{
					return false;
				}
				text = string.Format("{0} and zoneID={1}", text, dbroleInfo.ZoneID);
				if (moneyCost > 0 && roleID > 0)
				{
					text = string.Format("{0} and roleID={1}", text, roleID);
				}
				result = myDbConnection.ExecuteNonQueryBool(text, 0);
			}
			return result;
		}
	}
}
