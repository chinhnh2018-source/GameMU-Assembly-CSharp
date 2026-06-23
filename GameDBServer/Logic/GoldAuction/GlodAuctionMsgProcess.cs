using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Tools;

namespace GameDBServer.Logic.GoldAuction
{
	internal class GlodAuctionMsgProcess : IManager, ICmdProcessor
	{
		public static GlodAuctionMsgProcess getInstance()
		{
			return GlodAuctionMsgProcess.instance;
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
			lock (this.AuctionMsgMutex)
			{
				TCPCmdDispatcher.getInstance().registerProcessor(2080, GlodAuctionMsgProcess.getInstance());
				TCPCmdDispatcher.getInstance().registerProcessor(2081, GlodAuctionMsgProcess.getInstance());
			}
			return true;
		}

		public void LoadDataFromDB(DBManager DBMgr)
		{
			try
			{
				lock (this.AuctionMsgMutex)
				{
					GoldAuctionDB.DelData(string.Format("UpDBWay='Del' AND UpdateTime < '{0}'", DateTime.Now.AddDays(-365.0).ToString("yyyy-MM-dd HH:mm:ss")));
					this.AuctionDict.Clear();
					for (int i = 1; i < 3; i++)
					{
						List<GoldAuctionDBItem> list;
						if (GoldAuctionDB.Select(out list, i))
						{
							this.AuctionDict.Add(i, list);
							LjlLog.WriteLog(LogTypes.Info, string.Format("初始化金团数据type={0},len={1}", ((AuctionOrderEnum)i).ToString(), list.Count), "");
						}
						else
						{
							LjlLog.WriteLog(LogTypes.Info, string.Format("初始化金团数据type={0},false", ((AuctionOrderEnum)i).ToString()), "");
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			try
			{
				lock (this.AuctionMsgMutex)
				{
					if (nID == 2080)
					{
						this.GetGlodAuction(client, nID, cmdParams, count);
					}
					else if (nID == 2081)
					{
						this.SetGlodAuction(client, nID, cmdParams, count);
					}
					else
					{
						LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]id err id={0}", nID), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		private void GetGlodAuction(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			GetAuctionDBData getAuctionDBData = new GetAuctionDBData();
			try
			{
				string @string = new UTF8Encoding().GetString(cmdParams, 0, count);
				int num = Convert.ToInt32(@string);
				List<GoldAuctionDBItem> list;
				if (3 <= num || 0 >= num)
				{
					getAuctionDBData.Flag = false;
					LjlLog.WriteLog(LogTypes.Error, string.Format("GetGlodAuction 金团类型错误{0} type=", num), "");
				}
				else if (this.AuctionDict.TryGetValue(num, out list))
				{
					getAuctionDBData.Flag = CopyData.CopyAuctionDBItem(list, ref getAuctionDBData.ItemList);
				}
				else
				{
					getAuctionDBData.Flag = GoldAuctionDB.Select(out getAuctionDBData.ItemList, num);
					if (getAuctionDBData.Flag)
					{
						list = new List<GoldAuctionDBItem>();
						if (CopyData.CopyAuctionDBItem(getAuctionDBData.ItemList, ref list))
						{
							this.AuctionDict.Add(num, list);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LjlLog.WriteLog(LogTypes.Exception, ex.ToString(), "");
			}
			client.sendCmd<GetAuctionDBData>(nID, getAuctionDBData);
		}

		private void SetGlodAuction(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string text = "false";
			try
			{
				GoldAuctionDBItem tempData = DataHelper.BytesToObject<GoldAuctionDBItem>(cmdParams, 0, count);
				if (3 <= tempData.AuctionType || 0 >= tempData.AuctionType)
				{
					LjlLog.WriteLog(LogTypes.Error, string.Format("SetGlodAuction 金团类型错误 type={0}", tempData.AuctionType), "");
				}
				else if (tempData.UpDBWay == 2)
				{
					text = GoldAuctionDB.Insert(tempData).ToString();
					LjlLog.WriteLog(LogTypes.Info, string.Format("Add 金团新物品 type={0}, time={1}, AuctionSource={2}, upway={3}, msgDbStr={4}", new object[]
					{
						tempData.AuctionType,
						tempData.ProductionTime,
						tempData.AuctionSource,
						((DBAuctionUpEnum)tempData.UpDBWay).ToString(),
						text
					}), "");
				}
				else
				{
					text = GoldAuctionDB.Update(tempData).ToString();
				}
				client.sendCmd(nID, text);
				if (text.Equals("True"))
				{
					List<GoldAuctionDBItem> list;
					if (this.AuctionDict.TryGetValue(tempData.OldAuctionType, out list))
					{
						list.RemoveAll((GoldAuctionDBItem x) => x.ProductionTime == tempData.ProductionTime && x.AuctionSource == tempData.AuctionSource);
					}
					if (this.AuctionDict.TryGetValue(tempData.AuctionType, out list) && tempData.UpDBWay != 1)
					{
						list.Add(tempData);
					}
				}
			}
			catch (Exception ex)
			{
				LjlLog.WriteLog(LogTypes.Exception, ex.ToString(), "");
			}
		}

		private static GlodAuctionMsgProcess instance = new GlodAuctionMsgProcess();

		private object AuctionMsgMutex = new object();

		private Dictionary<int, List<GoldAuctionDBItem>> AuctionDict = new Dictionary<int, List<GoldAuctionDBItem>>();
	}
}
