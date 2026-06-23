using System;
using System.Collections.Generic;
using AutoCSer.Net.TcpServer;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Server;
using KF.Remoting;
using KF.TcpCall;
using Server.Data;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	internal class BoCaiManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static BoCaiManager getInstance()
		{
			return BoCaiManager.instance;
		}

		public bool initialize()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2082, 4, 4, BoCaiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2083, 2, 2, BoCaiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2085, 1, 1, BoCaiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2086, 4, 4, BoCaiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			this.NotifyEnterHandler1 = new EventSourceEx<KFCallMsg>.HandlerData
			{
				ID = 0,
				EventType = 10039,
				Handler = new Func<KFCallMsg, bool>(this.KFCallMsgFunc)
			};
			this.NotifyEnterHandler2 = new EventSourceEx<KFCallMsg>.HandlerData
			{
				ID = 0,
				EventType = 10040,
				Handler = new Func<KFCallMsg, bool>(this.KFCallMsgFunc)
			};
			KFCallManager.MsgSource.registerListener(10039, this.NotifyEnterHandler1);
			KFCallManager.MsgSource.registerListener(10040, this.NotifyEnterHandler2);
			BoCaiCaiShuZi.GetInstance().Init();
			BoCaiCaiDaXiao.GetInstance().Init();
			BoCaiShopManager.GetInstance().Init();
			return true;
		}

		public bool KFCallMsgFunc(KFCallMsg msg)
		{
			try
			{
				switch (msg.KuaFuEventType)
				{
				case 10039:
				{
					KFStageData kfstageData = msg.Get<KFStageData>();
					if (null != kfstageData)
					{
						if (kfstageData.BoCaiType == 2)
						{
							BoCaiCaiShuZi.GetInstance().SetStageData(kfstageData, true);
						}
						else if (kfstageData.BoCaiType == 1)
						{
							BoCaiCaiDaXiao.GetInstance().SetStageData(kfstageData, true);
						}
					}
					else
					{
						BoCaiCaiDaXiao.GetInstance().SetStageData(kfstageData, true);
						BoCaiCaiShuZi.GetInstance().SetStageData(kfstageData, true);
					}
					break;
				}
				case 10040:
				{
					OpenLottery openLottery = msg.Get<OpenLottery>();
					if (null != openLottery)
					{
						if (openLottery.BocaiType == 2)
						{
							BoCaiCaiShuZi.GetInstance().SetOpenLotteryData(openLottery, false, false);
						}
						else if (openLottery.BocaiType == 1)
						{
							BoCaiCaiDaXiao.GetInstance().SetOpenLotteryData(openLottery, false, true);
						}
					}
					else
					{
						BoCaiCaiDaXiao.GetInstance().SetOpenLotteryData(openLottery, false, true);
						BoCaiCaiShuZi.GetInstance().SetOpenLotteryData(openLottery, false, false);
					}
					break;
				}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩]跨服消息{0}", ex.ToString()), null, true);
			}
			return true;
		}

		public bool showdown()
		{
			KFCallManager.MsgSource.removeListener(10039, this.NotifyEnterHandler1);
			KFCallManager.MsgSource.removeListener(10040, this.NotifyEnterHandler2);
			return true;
		}

		public List<OpenLottery> GetNewOpenLottery10(int type)
		{
			try
			{
				ReturnValue<List<OpenLottery>> openLottery = TcpCall.KFBoCaiManager.GetOpenLottery(0, (long)type, false);
				if (openLottery.IsReturn)
				{
					return openLottery.Value;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return null;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (nID == 2082)
				{
					BuyBoCaiResult buyBoCaiResult = new BuyBoCaiResult();
					int num = Convert.ToInt32(cmdParams[1]);
					int buyNum = Convert.ToInt32(cmdParams[2]);
					string strBuyVal = cmdParams[3];
					buyBoCaiResult.BocaiType = num;
					if (2 == num)
					{
						this.BuyCaiShuzi(client, buyNum, strBuyVal, ref buyBoCaiResult);
					}
					else if (1 == num)
					{
						this.BuyCaiDaXiao(client, buyNum, strBuyVal, ref buyBoCaiResult);
					}
					else
					{
						buyBoCaiResult.Info = 1;
					}
					client.sendCmd<BuyBoCaiResult>(nID, buyBoCaiResult, false);
				}
				else if (nID == 2083)
				{
					GetBoCaiResult cmdData = new GetBoCaiResult();
					this.GetBoCai(client, nID, cmdParams, ref cmdData);
					client.sendCmd<GetBoCaiResult>(nID, cmdData, false);
				}
				else if (nID == 2086)
				{
					client.sendCmd(nID, this.BuyItem(client, nID, cmdParams), false);
				}
				else if (nID == 2085)
				{
					this.GetShopInfo(client, nID, client.ClientData.RoleID);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return true;
		}

		private void BuyCaiShuzi(GameClient client, int BuyNum, string strBuyVal, ref BuyBoCaiResult mgsData)
		{
			try
			{
				List<int> list;
				BoCaiHelper.String2ListInt(strBuyVal, out list);
				if (list.Count != 5 || BuyNum < 1)
				{
					mgsData.Info = 2;
					LogManager.WriteLog(1, string.Format("[ljl_博彩] BuyCaiShuzi 购买内容 {0}, BuyNum={1} ", strBuyVal, BuyNum), null, true);
				}
				else
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] > 9 || list[i] < 0)
						{
							mgsData.Info = 2;
							return;
						}
					}
					if (!BoCaiCaiShuZi.GetInstance().IsCanBuy())
					{
						mgsData.Info = 7;
					}
					else
					{
						ReturnValue<bool> returnValue = TcpCall.KFBoCaiManager.IsCanBuy(2, strBuyVal, BuyNum, BoCaiCaiShuZi.GetInstance().GetDataPeriods());
						if (!returnValue.IsReturn)
						{
							mgsData.Info = 8;
						}
						else if (!returnValue.Value)
						{
							mgsData.Info = 5;
						}
						else
						{
							int xiaoHaoDaiBi = BoCaiCaiShuZi.GetInstance().GetXiaoHaoDaiBi();
							if (xiaoHaoDaiBi < 1)
							{
								mgsData.Info = 3;
								LogManager.WriteLog(2, "[ljl_博彩]XiaoHaoDaiBi /GuDingLeiXing<1", null, true);
							}
							else
							{
								int num = BuyNum * xiaoHaoDaiBi;
								if (!HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, num))
								{
									mgsData.Info = 4;
								}
								else
								{
									int buyNum = BuyNum;
									BuyBoCai2SDB buyBoCai2SDB = BoCaiCaiShuZi.GetInstance().BuyBocai(client, BuyNum, strBuyVal, ref buyNum);
									if (null == buyBoCai2SDB)
									{
										mgsData.Info = 8;
										BoCaiCaiShuZi.GetInstance().BuyBocai(client, -BuyNum, strBuyVal, ref buyNum);
										LogManager.WriteLog(2, "[ljl_博彩]BoCaiCaiShuZi.GetInstance().BuyBocai err", null, true);
									}
									else if (!HuanLeDaiBiManager.GetInstance().UseHuanledaibi(client, num))
									{
										mgsData.Info = 4;
									}
									else
									{
										ReturnValue<bool> returnValue2 = TcpCall.KFBoCaiManager.BuyBoCai(new KFBuyBocaiData
										{
											BocaiType = 2,
											RoleID = buyBoCai2SDB.m_RoleID,
											RoleName = buyBoCai2SDB.m_RoleName,
											ZoneID = buyBoCai2SDB.ZoneID,
											ServerID = buyBoCai2SDB.ServerId,
											BuyNum = buyBoCai2SDB.BuyNum,
											BuyValue = buyBoCai2SDB.strBuyValue
										});
										if (!returnValue2.IsReturn)
										{
											mgsData.Info = 8;
										}
										else if (!returnValue2.Value)
										{
											GameManager.logDBCmdMgr.AddMessageLog(-1, "欢乐代币", "购买失败扣物品成功中心2次通信", client.ClientData.RoleName, client.ClientData.RoleName, "减少", num, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, "");
											mgsData.Info = 5;
										}
										else
										{
											buyBoCai2SDB.BuyNum = buyNum;
											BoCaiBuy2DBList.getInstance().AddData(buyBoCai2SDB, num, true);
											BoCaiCaiShuZi.GetInstance().CopyBuyList(out mgsData.ItemList, buyBoCai2SDB.m_RoleID);
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				mgsData.Info = 100;
				LogManager.WriteLog(9, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
		}

		private void BuyCaiDaXiao(GameClient client, int BuyNum, string strBuyVal, ref BuyBoCaiResult mgsData)
		{
			try
			{
				int num = Convert.ToInt32(strBuyVal);
				if (1 > num || num > 3 || BuyNum < 1)
				{
					mgsData.Info = 2;
					LogManager.WriteLog(1, string.Format("[ljl_博彩]BuyCaiDaXiao 购买内容 {0}, BuyNum={1} ", strBuyVal, BuyNum), null, true);
				}
				else if (!BoCaiCaiDaXiao.GetInstance().IsCanBuy())
				{
					mgsData.Info = 7;
				}
				else
				{
					ReturnValue<bool> returnValue = TcpCall.KFBoCaiManager.IsCanBuy(1, strBuyVal, BuyNum + BoCaiCaiDaXiao.GetInstance().GetBuyNum(client.ClientData.RoleID), BoCaiCaiDaXiao.GetInstance().GetDataPeriods());
					if (!returnValue.IsReturn)
					{
						mgsData.Info = 8;
					}
					else if (!returnValue.Value)
					{
						mgsData.Info = 7;
					}
					else
					{
						int num2 = BoCaiCaiDaXiao.GetInstance().GetXiaoHaoDaiBi() * BuyNum;
						if (num2 < 1)
						{
							mgsData.Info = 3;
							LogManager.WriteLog(2, "[ljl_博彩]XiaoHaoDaiBi /GuDingLeiXing<1", null, true);
						}
						else if (!HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, num2))
						{
							mgsData.Info = 4;
						}
						else
						{
							int buyNum = BuyNum;
							BuyBoCai2SDB buyBoCai2SDB = BoCaiCaiDaXiao.GetInstance().BuyBocai(client, BuyNum, strBuyVal, ref buyNum);
							if (null == buyBoCai2SDB)
							{
								mgsData.Info = 8;
								BoCaiCaiDaXiao.GetInstance().BuyBocai(client, -BuyNum, strBuyVal, ref buyNum);
								LogManager.WriteLog(2, "[ljl_博彩]BoCaiCaiDaXiao.GetInstance().BuyBocai err", null, true);
							}
							else if (!HuanLeDaiBiManager.GetInstance().UseHuanledaibi(client, num2))
							{
								mgsData.Info = 4;
							}
							else
							{
								ReturnValue<bool> returnValue2 = TcpCall.KFBoCaiManager.BuyBoCai(new KFBuyBocaiData
								{
									BocaiType = 1,
									RoleID = buyBoCai2SDB.m_RoleID,
									RoleName = buyBoCai2SDB.m_RoleName,
									ZoneID = buyBoCai2SDB.ZoneID,
									ServerID = buyBoCai2SDB.ServerId,
									BuyNum = buyBoCai2SDB.BuyNum,
									BuyValue = buyBoCai2SDB.strBuyValue
								});
								if (!returnValue2.IsReturn)
								{
									mgsData.Info = 8;
								}
								else if (!returnValue2.Value)
								{
									GameManager.logDBCmdMgr.AddMessageLog(-1, "欢乐代币", "购买失败扣物品成功中心2次通信", client.ClientData.RoleName, client.ClientData.RoleName, "减少", num2, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, "");
									mgsData.Info = 5;
								}
								else
								{
									buyBoCai2SDB.BuyNum = buyNum;
									BoCaiBuy2DBList.getInstance().AddData(buyBoCai2SDB, num2, true);
									BoCaiCaiDaXiao.GetInstance().CopyBuyList(out mgsData.ItemList, buyBoCai2SDB.m_RoleID);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				mgsData.Info = 100;
				LogManager.WriteLog(9, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
		}

		private void GetBoCai(GameClient client, int nID, string[] cmdParams, ref GetBoCaiResult mgsData)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[1]);
				mgsData.Info = 0;
				mgsData.BocaiType = num;
				if (2 == num)
				{
					FunctionSendManager.GetInstance().AddFunction(FunctionType.CaiShuZi, client.ClientData.RoleID);
					BoCaiCaiShuZi.GetInstance().OpenGetBoCai(client.ClientData.RoleID, ref mgsData);
				}
				else if (1 == num)
				{
					FunctionSendManager.GetInstance().AddFunction(FunctionType.CaiDaXiao, client.ClientData.RoleID);
					BoCaiCaiDaXiao.GetInstance().OpenGetBoCai(client.ClientData.RoleID, ref mgsData);
				}
				else
				{
					mgsData.Info = 1;
				}
			}
			catch (Exception ex)
			{
				mgsData.Info = 100;
				LogManager.WriteLog(9, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
		}

		private string BuyItem(GameClient client, int nID, string[] cmdParams)
		{
			string arg = "";
			try
			{
				if (GameManager.systemParamsList.GetParamValueIntByName("HuanLeDuiHuan", -1) < 1L)
				{
					return string.Format("{0}{1}", arg, 7);
				}
				int id = Convert.ToInt32(cmdParams[1]);
				int num = Convert.ToInt32(cmdParams[2]);
				string text = cmdParams[3];
				arg = string.Format("{0}:{1}:{2}:", cmdParams[1], cmdParams[2], cmdParams[3]);
				DuiHuanShangChengConfig boCaiShopConfig = BoCaiConfigMgr.GetBoCaiShopConfig(id, text);
				if (null == boCaiShopConfig)
				{
					return string.Format("{0}{1}", arg, 14);
				}
				int useNum = boCaiShopConfig.DaiBiJiaGe * num;
				GoodsData goodsData = GlobalNew.ParseGoodsData(text);
				if (null == goodsData)
				{
					return string.Format("{0}{1}", arg, 14);
				}
				if (!HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, useNum))
				{
					return string.Format("{0}{1}", arg, 4);
				}
				if (!Global.CanAddGoods3(client, goodsData.GoodsID, num, goodsData.Binding, "1900-01-01 12:00:00", true))
				{
					return string.Format("{0}{1}", arg, 13);
				}
				KFBoCaiShopDB kfboCaiShopDB = new KFBoCaiShopDB();
				kfboCaiShopDB.BuyNum = num;
				kfboCaiShopDB.ID = id;
				kfboCaiShopDB.WuPinID = text;
				kfboCaiShopDB.RoleID = client.ClientData.RoleID;
				kfboCaiShopDB.Periods = Convert.ToInt32(TimeUtil.NowDataTimeString("yyMMdd"));
				if (boCaiShopConfig.MeiRiShangXianDan > -1)
				{
					if (num > boCaiShopConfig.MeiRiShangXianDan)
					{
						return string.Format("{0}{1}", arg, 17);
					}
					if (!BoCaiShopManager.GetInstance().CanBuyItem(kfboCaiShopDB, boCaiShopConfig.MeiRiShangXianDan))
					{
						return string.Format("{0}{1}", arg, 18);
					}
				}
				if (!HuanLeDaiBiManager.GetInstance().UseHuanledaibi(client, useNum))
				{
					return string.Format("{0}{1}", arg, 4);
				}
				int num2 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, num, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, goodsData.Site, goodsData.Jewellist, true, 1, "博彩商店购买", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
				LogManager.WriteLog(0, string.Format("[ljl_博彩] 博彩商店购买 放在背包ret={1}，RoleID={0},WuPinID={2},name={3}", new object[]
				{
					client.ClientData.RoleID,
					num2,
					text,
					client.ClientData.RoleName
				}), null, true);
				return string.Format("{0}{1}", arg, 0);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return string.Format("{0}{1}", arg, 100);
		}

		private void GetShopInfo(GameClient client, int nID, int roleID)
		{
			BoCaiShopInfo boCaiShopInfo = new BoCaiShopInfo();
			try
			{
				boCaiShopInfo.Info = 0;
				BoCaiShopManager.GetInstance().GetSelfBuyData(roleID, ref boCaiShopInfo);
			}
			catch (Exception ex)
			{
				boCaiShopInfo.Info = 100;
				LogManager.WriteLog(9, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			client.sendCmd<BoCaiShopInfo>(nID, boCaiShopInfo, false);
		}

		public bool GetBuyList2DB(int type, long DataPeriods, out List<BuyBoCai2SDB> ItemList, int msgType = 1)
		{
			ItemList = new List<BuyBoCai2SDB>();
			try
			{
				GetBuyBoCaiList getBuyBoCaiList = Global.sendToDB<GetBuyBoCaiList, string>(2083, string.Format("{2},{1},{0}", type, DataPeriods, msgType), 0);
				if (getBuyBoCaiList == null || !getBuyBoCaiList.Flag)
				{
					LogManager.WriteLog(2, string.Format("[ljl_博彩]GetBuyList2DB DBData={0}, type={1}, DataPeriods={2}", null == getBuyBoCaiList, type, DataPeriods), null, true);
					return false;
				}
				if (null == getBuyBoCaiList.ItemList)
				{
					getBuyBoCaiList.ItemList = new List<BuyBoCai2SDB>();
				}
				ItemList.AddRange(getBuyBoCaiList.ItemList);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		public bool GetOpenList2DB(int type, out GetOpenList DBData)
		{
			DBData = new GetOpenList();
			try
			{
				DBData = Global.sendToDB<GetOpenList, string>(2083, string.Format("{1},{0}", type, 3), 0);
				if (DBData == null || !DBData.Flag)
				{
					LogManager.WriteLog(2, string.Format("[ljl_博彩]GetOpenList2DB  DBData={0}, type={1}", null == DBData, type), null, true);
					return false;
				}
				if (null == DBData.ItemList)
				{
					DBData.ItemList = new List<OpenLottery>();
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		private bool ReturnItem(OpenLottery data, BuyBoCai2SDB buyItem)
		{
			try
			{
				buyItem.IsSend = true;
				BoCaiBuy2DBList.getInstance().AddData(buyItem, 0, false);
				if (!"True".Equals(Global.Send2DB<BuyBoCai2SDB>(2082, buyItem, 0)))
				{
					LogManager.WriteLog(1, string.Format("[ljl_博彩]更新数据库标志失败，返回道具，没关系 线程会自动处理旧数据,{0},{1}", buyItem.m_RoleName, buyItem.DataPeriods), null, true);
					return false;
				}
				int num = buyItem.BuyNum * data.XiaoHaoDaiBi;
				string text = "猜大小";
				if (buyItem.BocaiType == 2)
				{
					text = "猜数字";
				}
				string strIntro = string.Format("因系统维护原因导致{0}期{1}玩法没有正常开奖，系统将返还您当期下注的欢乐代币。", buyItem.DataPeriods, text);
				List<GoodsData> goodsData = new List<GoodsData>
				{
					HuanLeDaiBiManager.GetInstance().GetHuanLeDaiBi(num)
				};
				return this.SendMail(buyItem, goodsData, text, strIntro, num, false);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		public bool SendWinItem(OpenLottery data, BuyBoCai2SDB buyItem, double Rate, bool isSendMail, string winType)
		{
			try
			{
				buyItem.IsSend = true;
				if (!winType.Equals(buyItem.strBuyValue))
				{
					buyItem.IsWin = false;
					BoCaiBuy2DBList.getInstance().AddData(buyItem, -1, true);
					return true;
				}
				int num = (int)(Rate * (double)buyItem.BuyNum * (double)data.XiaoHaoDaiBi);
				string strTitle = "猜大小";
				string strIntro = string.Format("恭喜您在{0}期猜大小玩法中，获得欢乐代币{1}，系统将邮件的形式将您获取的欢乐代币返还与你。", buyItem.DataPeriods, num);
				buyItem.IsWin = true;
				string arg = string.Format("TYPE= {8},id={6},name={7}, {0}期开奖{1}赢得{2},自己购买{3},{4}注,info={5}", new object[]
				{
					data.DataPeriods,
					data.strWinNum,
					num,
					buyItem.strBuyValue,
					buyItem.BuyNum,
					data.WinInfo,
					buyItem.m_RoleID,
					buyItem.m_RoleName,
					buyItem.BocaiType
				});
				BoCaiBuy2DBList.getInstance().AddData(buyItem, -1, false);
				if (!"True".Equals(Global.Send2DB<BuyBoCai2SDB>(2082, buyItem, 0)))
				{
					LogManager.WriteLog(1, string.Format("[ljl_博彩]更新数据库标志失败，不发奖励，没关系 会自动处理旧数据{0}", arg), null, true);
					return num < 1;
				}
				if (num < 1)
				{
					return true;
				}
				GoodsData huanLeDaiBi = HuanLeDaiBiManager.GetInstance().GetHuanLeDaiBi(num);
				List<GoodsData> goodsData = new List<GoodsData>
				{
					huanLeDaiBi
				};
				if (isSendMail)
				{
					return this.SendMail(buyItem, goodsData, strTitle, strIntro, num, true);
				}
				GameClient gameClient = GameManager.ClientMgr.FindClient(buyItem.m_RoleID);
				if (gameClient != null && Global.CanAddGoods3(gameClient, huanLeDaiBi.GoodsID, num, huanLeDaiBi.Binding, "1900-01-01 12:00:00", true))
				{
					int num2 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, huanLeDaiBi.GoodsID, huanLeDaiBi.GCount, huanLeDaiBi.Quality, huanLeDaiBi.Props, huanLeDaiBi.Forge_level, huanLeDaiBi.Binding, huanLeDaiBi.Site, huanLeDaiBi.Jewellist, true, 1, "猜大小中奖", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
					LogManager.WriteLog(0, string.Format("[ljl_博彩]放在背包ret={1}，{0}", arg, num2), null, true);
					return true;
				}
				return this.SendMail(buyItem, goodsData, strTitle, strIntro, num, true);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		public bool SendWinItem(OpenLottery data, BuyBoCai2SDB buyItem)
		{
			try
			{
				buyItem.IsSend = true;
				string[] array = data.WinInfo.Split(new char[]
				{
					','
				});
				List<int> list;
				BoCaiHelper.String2ListInt(data.strWinNum, out list);
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				List<int> list2;
				BoCaiHelper.String2ListInt(buyItem.strBuyValue, out list2);
				if (list2.Count != list.Count)
				{
					LogManager.WriteLog(2, string.Format("[ljl_博彩]selfBuy.Count != winList.Count DataPeriods={0}, type={1}, roleid={2}", data.DataPeriods, data.BocaiType, buyItem.m_RoleID), null, true);
					return false;
				}
				int num4 = 0;
				for (int i = 0; i < list2.Count; i++)
				{
					if (list2[i] == list[i])
					{
						num4++;
					}
				}
				int num5 = 0;
				int num6 = 0;
				if (num4 == 5)
				{
					num5 = 1;
					num6 = num * buyItem.BuyNum;
				}
				else if (num4 == 4)
				{
					num5 = 2;
					num6 = num2 * buyItem.BuyNum;
				}
				else if (num4 == 3)
				{
					num5 = 3;
					num6 = num3 * buyItem.BuyNum;
				}
				string arg = string.Format("猜数字id={6},name={7}, {0}期开奖{1}赢得{2},自己购买{3},{4}注,info={5}", new object[]
				{
					data.DataPeriods,
					data.strWinNum,
					num6,
					buyItem.strBuyValue,
					buyItem.BuyNum,
					data.WinInfo,
					buyItem.m_RoleID,
					buyItem.m_RoleName
				});
				if (num6 < 1)
				{
					buyItem.IsWin = false;
					BoCaiBuy2DBList.getInstance().AddData(buyItem, -1, true);
					return true;
				}
				buyItem.IsWin = true;
				BoCaiBuy2DBList.getInstance().AddData(buyItem, -1, false);
				if (!"True".Equals(Global.Send2DB<BuyBoCai2SDB>(2082, buyItem, 0)))
				{
					LogManager.WriteLog(1, string.Format("[ljl_博彩]更新数据库标志失败，不发奖励，没关系 会自动处理旧数据{0}", arg), null, true);
					return false;
				}
				string strTitle = "猜数字";
				string strIntro = string.Format("恭喜您在{0}期猜数字玩法中,中{2}等奖，获得欢乐代币{1}，系统将邮件的形式将您获取的欢乐代币返还与你。", buyItem.DataPeriods, num6, num5);
				List<GoodsData> goodsData = new List<GoodsData>
				{
					HuanLeDaiBiManager.GetInstance().GetHuanLeDaiBi(num6)
				};
				return this.SendMail(buyItem, goodsData, strTitle, strIntro, num6, true);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		private bool SendMail(BuyBoCai2SDB buyItem, List<GoodsData> goodsData, string strTitle, string strIntro, int ItemNum, bool send = true)
		{
			try
			{
				string text;
				if (buyItem.BocaiType == 2)
				{
					text = "猜数字邮件";
				}
				else
				{
					text = "猜大小邮件";
				}
				text += (send ? "发奖" : "退回");
				if (Global.UseMailGivePlayerAward3(buyItem.m_RoleID, goodsData, strTitle, strIntro, 0, 0, 0))
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "欢乐代币", string.Format("{0}成功", text), "系统", buyItem.m_RoleName, "增加", ItemNum, buyItem.ZoneID, buyItem.strUserID, -1, buyItem.ServerId, null);
					return true;
				}
				GameManager.logDBCmdMgr.AddMessageLog(-1, "欢乐代币", string.Format("{0}失败", text), buyItem.m_RoleName, buyItem.m_RoleName, "增加", ItemNum, buyItem.ZoneID, buyItem.strUserID, -1, buyItem.ServerId, "");
				LogManager.WriteLog(2, string.Format("[ljl_博彩]{2}失败 send email roleid={0}, num={1}, name={3}", new object[]
				{
					buyItem.m_RoleID,
					ItemNum,
					text,
					buyItem.m_RoleName
				}), null, true);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		public void OldtterySet(int BoCaiType, long DataPeriods)
		{
			try
			{
				LogManager.WriteLog(0, string.Format("[ljl_博彩]处理历史开奖 BoCaiType={0}, 处理界限 DataPeriods={1}", BoCaiType, DataPeriods), null, true);
				GetOpenList getOpenList;
				if (!BoCaiManager.getInstance().GetOpenList2DB(BoCaiType, out getOpenList))
				{
					LogManager.WriteLog(2, string.Format("[ljl_博彩]处理历史开奖 读库失败 GetOpenList2DB BoCaiType={0}, DataPeriods={1}", BoCaiType, DataPeriods), null, true);
				}
				else
				{
					foreach (OpenLottery openLottery in getOpenList.ItemList)
					{
						OpenLottery openLottery2 = openLottery;
						if (BoCaiType != 1)
						{
							if (BoCaiType == 2)
							{
								if (openLottery.DataPeriods >= Convert.ToInt64(string.Format("{0}1", TimeUtil.NowDataTimeString("yyMMdd"))))
								{
									continue;
								}
							}
						}
						if (string.IsNullOrEmpty(openLottery.strWinNum) || string.IsNullOrEmpty(openLottery.WinInfo))
						{
							ReturnValue<List<OpenLottery>> openLottery3 = TcpCall.KFBoCaiManager.GetOpenLottery(BoCaiType, openLottery.DataPeriods, true);
							if (!openLottery3.IsReturn)
							{
								return;
							}
							List<OpenLottery> value = openLottery3.Value;
							if (value == null || value.Count < 1)
							{
								continue;
							}
							openLottery2 = value[0];
						}
						if (openLottery2 != null && openLottery2.DataPeriods >= 1L && openLottery2.XiaoHaoDaiBi >= 0)
						{
							Global.Send2DB<OpenLottery>(2084, openLottery2, 0);
							List<BuyBoCai2SDB> list;
							if (!BoCaiManager.getInstance().GetBuyList2DB(BoCaiType, openLottery2.DataPeriods, out list, 2))
							{
								LogManager.WriteLog(2, string.Format("[ljl_博彩]获取购买记录失败 BoCaiType={0},DataPeriods={1}", BoCaiType, openLottery2.DataPeriods), null, true);
							}
							else if (list.Count < 1)
							{
								openLottery2.IsAward = true;
								Global.Send2DB<OpenLottery>(2084, openLottery2, 0);
								LogManager.WriteLog(0, string.Format("[ljl_博彩]还有0人没处理 完成处理历史开奖 BoCaiType={0}, DataPeriods={1}", BoCaiType, openLottery2.DataPeriods), null, true);
								continue;
							}
							if (string.IsNullOrEmpty(openLottery2.strWinNum) || string.IsNullOrEmpty(openLottery2.WinInfo))
							{
								LogManager.WriteLog(0, string.Format("[ljl_博彩] 没有开过奖处理历史退回{0}", openLottery2.DataPeriods), null, true);
								foreach (BuyBoCai2SDB buyBoCai2SDB in list)
								{
									if (!this.ReturnItem(openLottery2, buyBoCai2SDB))
									{
										openLottery2.IsAward = false;
									}
								}
								if (openLottery2.IsAward)
								{
									Global.Send2DB<OpenLottery>(2084, openLottery2, 0);
								}
							}
							else
							{
								LogManager.WriteLog(0, string.Format("[ljl_博彩]处理历史开奖{0}", openLottery2.DataPeriods), null, true);
								foreach (BuyBoCai2SDB buyBoCai2SDB in list)
								{
									if (buyBoCai2SDB.BocaiType == 1)
									{
										string winType;
										double num = BoCaiCaiDaXiao.GetInstance().CompensateRate(openLottery2.strWinNum, openLottery2.WinInfo, out winType);
										if (num < 1.0)
										{
											LogManager.WriteLog(0, "[ljl_博彩] 开奖 赔率 < 1 ", null, true);
										}
										else if (!this.SendWinItem(openLottery2, buyBoCai2SDB, num, true, winType))
										{
											openLottery2.IsAward = false;
										}
									}
									else if (buyBoCai2SDB.BocaiType == 2)
									{
										if (!this.SendWinItem(openLottery2, buyBoCai2SDB))
										{
											openLottery2.IsAward = false;
										}
									}
									else
									{
										openLottery2.IsAward = false;
										LogManager.WriteLog(2, string.Format("[ljl_博彩]购买记录 类型不对 BoCaiType={0},DataPeriods={1}", BoCaiType, openLottery2.DataPeriods), null, true);
									}
								}
								if (openLottery2.IsAward)
								{
									Global.Send2DB<OpenLottery>(2084, openLottery2, 0);
								}
							}
						}
					}
					ReturnValue<List<OpenLottery>> openLottery4 = TcpCall.KFBoCaiManager.GetOpenLottery(BoCaiType, getOpenList.MaxDataPeriods, false);
					if (openLottery4.IsReturn)
					{
						List<OpenLottery> value2 = openLottery4.Value;
						if (null != value2)
						{
							foreach (OpenLottery openLottery in value2)
							{
								Global.Send2DB<OpenLottery>(2084, openLottery, 0);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_博彩]{0}", ex.ToString()), null, true);
			}
		}

		public void BoCaiPriorityActivity(GameClient client)
		{
			BoCaiCaiShuZi.GetInstance().PriorityActivity(client);
			BoCaiCaiDaXiao.GetInstance().PriorityActivity(client);
		}

		private const int MaxCaiNum = 9;

		private const int MinCaiNum = 0;

		public const string msgFlag = "True";

		private static BoCaiManager instance = new BoCaiManager();

		private EventSourceEx<KFCallMsg>.HandlerData NotifyEnterHandler1 = null;

		private EventSourceEx<KFCallMsg>.HandlerData NotifyEnterHandler2 = null;
	}
}
