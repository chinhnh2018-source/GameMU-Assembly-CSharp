using System;
using System.Text;
using GameDBServer.Core;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Tools;

namespace GameDBServer.Logic.BoCai
{
	internal class BoCaiManager : IManager, ICmdProcessor
	{
		public static BoCaiManager getInstance()
		{
			return BoCaiManager.instance;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			bool result;
			lock (this.mutex)
			{
				TCPCmdDispatcher.getInstance().registerProcessor(2082, BoCaiManager.getInstance());
				TCPCmdDispatcher.getInstance().registerProcessor(2083, BoCaiManager.getInstance());
				TCPCmdDispatcher.getInstance().registerProcessor(2084, BoCaiManager.getInstance());
				TCPCmdDispatcher.getInstance().registerProcessor(2085, BoCaiManager.getInstance());
				TCPCmdDispatcher.getInstance().registerProcessor(2086, BoCaiManager.getInstance());
				result = true;
			}
			return result;
		}

		public void LoadDataFromDB(DBManager DBMgr)
		{
			try
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				string arg = TimeUtil.DataTimeToString(dateTime, "yyMMdd");
				BoCaiDBOperator.DelData("t_bocai_shop", string.Format("Periods!='{0}'", arg));
				for (int i = 1; i < 3; i++)
				{
					long nowPeriods = this.GetNowPeriods(dateTime, i);
					BoCaiDBOperator.DelData("t_bocai_open_lottery", string.Format("IsAward=1 AND BocaiType={1} AND DataPeriods < {0}", nowPeriods, i));
					BoCaiDBOperator.DelData("t_bocai_buy_history", string.Format("IsSend=1 AND BocaiType={1} AND DataPeriods < {0}", nowPeriods, i));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		private long GetNowPeriods(DateTime time, int type)
		{
			long result;
			if (type == 2)
			{
				result = Convert.ToInt64(string.Format("{0}1", TimeUtil.DataTimeToString(time.AddYears(-1), "yyMMdd")));
			}
			else if (type == 1)
			{
				result = Convert.ToInt64(string.Format("{0}001", TimeUtil.DataTimeToString(time.AddMonths(-6), "yyMMdd")));
			}
			else
			{
				result = 0L;
			}
			return result;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				lock (this.mutex)
				{
					if (nID == 2082)
					{
						this.updateBuy(client, nID, cmdParams, count);
					}
					else if (nID == 2083)
					{
						string @string = new UTF8Encoding().GetString(cmdParams, 0, count);
						int num = Convert.ToInt32(@string.Split(new char[]
						{
							','
						})[0]);
						if (num == 1)
						{
							this.getBoCaiBuyList(client, nID, @string, false);
						}
						else if (num == 3)
						{
							this.getBoCaiOpenList(client, nID, @string);
						}
						else if (num == 2)
						{
							this.getBoCaiBuyList(client, nID, @string, true);
						}
					}
					else if (nID == 2084)
					{
						this.updateOpen(client, nID, cmdParams, count);
					}
					else if (nID == 2085)
					{
						this.getShopList(client, nID, cmdParams, count);
					}
					else if (nID == 2086)
					{
						this.setShopData(client, nID, cmdParams, count);
					}
					else
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("[ljl]id err id={0}", nID), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		private void updateBuy(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string cmdData = "false";
			try
			{
				BuyBoCai2SDB data = DataHelper.BytesToObject<BuyBoCai2SDB>(cmdParams, 0, count);
				cmdData = BoCaiDBOperator.ReplaceBuyBoCai(data).ToString();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			client.sendCmd(nID, cmdData);
		}

		private void updateOpen(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string cmdData = "false";
			try
			{
				OpenLottery data = DataHelper.BytesToObject<OpenLottery>(cmdParams, 0, count);
				cmdData = BoCaiDBOperator.ReplaceOpenLottery(data).ToString();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
			client.sendCmd(nID, cmdData);
		}

		private void getBoCaiBuyList(GameServerClient client, int nID, string cmdData, bool isNoSend)
		{
			GetBuyBoCaiList getBuyBoCaiList = new GetBuyBoCaiList();
			getBuyBoCaiList.Flag = false;
			try
			{
				string[] array = cmdData.Split(new char[]
				{
					','
				});
				int bocaiType = Convert.ToInt32(array[2]);
				long dataPeriods = Convert.ToInt64(array[1]);
				BoCaiDBOperator.SelectBuyBoCai(bocaiType, dataPeriods, out getBuyBoCaiList.ItemList, isNoSend);
				if (null != getBuyBoCaiList.ItemList)
				{
					getBuyBoCaiList.Flag = true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			client.sendCmd<GetBuyBoCaiList>(nID, getBuyBoCaiList);
		}

		private void getBoCaiOpenList(GameServerClient client, int nID, string cmdData)
		{
			GetOpenList getOpenList = new GetOpenList();
			getOpenList.Flag = false;
			try
			{
				string[] array = cmdData.Split(new char[]
				{
					','
				});
				int num = Convert.ToInt32(array[1]);
				long num2 = Convert.ToInt64(array[1]);
				getOpenList.MaxDataPeriods = BoCaiDBOperator.GetMaxData(num);
				BoCaiDBOperator.SelectOpenLottery(num, out getOpenList.ItemList);
				if (getOpenList.MaxDataPeriods >= 0L && null != getOpenList.ItemList)
				{
					getOpenList.Flag = true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			client.sendCmd<GetOpenList>(nID, getOpenList);
		}

		private void getShopList(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			BoCaiShopDBData boCaiShopDBData = new BoCaiShopDBData();
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				string periods = TimeUtil.DataTimeToString(now, "yyMMdd");
				BoCaiDBOperator.SelectBoCaiShop(periods, out boCaiShopDBData.ItemList);
				boCaiShopDBData.Flag = (null != boCaiShopDBData.ItemList);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
			client.sendCmd<BoCaiShopDBData>(nID, boCaiShopDBData);
		}

		private void setShopData(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string cmdData = "false";
			try
			{
				BoCaiShopDB data = DataHelper.BytesToObject<BoCaiShopDB>(cmdParams, 0, count);
				cmdData = BoCaiDBOperator.ReplaceBoCaiShop(data).ToString();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, ex.ToString(), null, true);
			}
			client.sendCmd(nID, cmdData);
		}

		private static BoCaiManager instance = new BoCaiManager();

		private object mutex = new object();
	}
}
