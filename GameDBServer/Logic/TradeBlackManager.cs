using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class TradeBlackManager : SingletonTemplate<TradeBlackManager>, IManager, ICmdProcessor
	{
		private TradeBlackManager()
		{
		}

		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(14007, SingletonTemplate<TradeBlackManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(14008, SingletonTemplate<TradeBlackManager>.Instance());
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
			if (nID == 14007)
			{
				this.HandleLoad(client, nID, cmdParams, count);
			}
			else if (nID == 14008)
			{
				this.handleSave(client, nID, cmdParams, count);
			}
		}

		private void HandleLoad(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			List<TradeBlackHourItem> list = null;
			MySQLConnection mySQLConnection = null;
			try
			{
				string @string = new UTF8Encoding().GetString(cmdParams, 0, count);
				string[] array = @string.Split(new char[]
				{
					':'
				});
				int num = Convert.ToInt32(array[0]);
				string arg = array[1].Trim();
				int num2 = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = DBManager.getInstance().GetDBRoleInfo(ref num);
				if (dbroleInfo == null)
				{
					throw new Exception("TradeBlackManager.HandleLoad not Find DBRoleInfo, roleid=" + num);
				}
				mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
				string text = string.Format("SELECT day,hour,distinct_roles,market_times,market_in_price,market_out_price,trade_times,trade_in_price,trade_out_price FROM t_ban_trade WHERE rid={0} AND ((TO_DAYS('{1}') - TO_DAYS(day) = 0 AND hour <= {2}) or (TO_DAYS('{1}') - TO_DAYS(day) = 1 AND hour > {2}))", num, arg, num2);
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				list = new List<TradeBlackHourItem>();
				while (mySQLDataReader.Read())
				{
					list.Add(new TradeBlackHourItem
					{
						RoleId = num,
						Day = mySQLDataReader["day"].ToString().Trim(),
						Hour = Convert.ToInt32(mySQLDataReader["hour"].ToString()),
						TradeDistinctRoleCount = Convert.ToInt32(mySQLDataReader["distinct_roles"].ToString()),
						MarketInPrice = Convert.ToInt64(mySQLDataReader["market_in_price"]),
						MarketTimes = Convert.ToInt32(mySQLDataReader["market_times"]),
						MarketOutPrice = Convert.ToInt64(mySQLDataReader["market_out_price"]),
						TradeInPrice = Convert.ToInt64(mySQLDataReader["Trade_in_price"]),
						TradeTimes = Convert.ToInt32(mySQLDataReader["Trade_times"]),
						TradeOutPrice = Convert.ToInt64(mySQLDataReader["Trade_out_price"])
					});
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException("TradeBlackManager.HandleLoad " + ex.Message);
			}
			finally
			{
				if (null != mySQLConnection)
				{
					DBManager.getInstance().DBConns.PushDBConnection(mySQLConnection);
				}
			}
			client.sendCmd<List<TradeBlackHourItem>>(nID, list);
		}

		private void handleSave(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			bool cmdData = false;
			MySQLConnection mySQLConnection = null;
			try
			{
				TradeBlackHourItem tradeBlackHourItem = DataHelper.BytesToObject<TradeBlackHourItem>(cmdParams, 0, count);
				DBRoleInfo dbroleInfo = DBManager.getInstance().GetDBRoleInfo(ref tradeBlackHourItem.RoleId);
				if (dbroleInfo == null)
				{
					throw new Exception("TradeBlackManager.handleSave not Find DBRoleInfo, roleid=" + tradeBlackHourItem.RoleId);
				}
				string text = string.Format("REPLACE INTO t_ban_trade(rid,day,hour,market_in_price,market_times,market_out_price,Trade_in_price,Trade_times,Trade_out_price,distinct_roles)  VALUES({0},'{1}',{2},{3},{4},{5},{6},{7},{8},{9})", new object[]
				{
					tradeBlackHourItem.RoleId,
					tradeBlackHourItem.Day,
					tradeBlackHourItem.Hour,
					tradeBlackHourItem.MarketInPrice,
					tradeBlackHourItem.MarketTimes,
					tradeBlackHourItem.MarketOutPrice,
					tradeBlackHourItem.TradeInPrice,
					tradeBlackHourItem.TradeTimes,
					tradeBlackHourItem.TradeOutPrice,
					tradeBlackHourItem.TradeDistinctRoleCount
				});
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
				mySQLConnection = DBManager.getInstance().DBConns.PopDBConnection();
				MySQLCommand mySQLCommand = new MySQLCommand(text, mySQLConnection);
				mySQLCommand.ExecuteNonQuery();
				mySQLCommand.Dispose();
				cmdData = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteException("TradeBlackManager.handleSave " + ex.Message);
				cmdData = false;
			}
			finally
			{
				if (mySQLConnection != null)
				{
					DBManager.getInstance().DBConns.PushDBConnection(mySQLConnection);
				}
			}
			client.sendCmd<bool>(nID, cmdData);
		}
	}
}
