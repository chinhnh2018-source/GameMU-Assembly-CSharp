using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic;
using GameServer.Logic.CheatGuard;
using GameServer.Logic.LiXianBaiTan;
using GameServer.Logic.Reborn;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Server.CmdProcesser
{
	public class SaleCmdsProcessor : ICmdProcessor
	{
		private TCPManager tcpMgr
		{
			get
			{
				return TCPManager.getInstance();
			}
		}

		private TCPOutPacketPool pool
		{
			get
			{
				return TCPManager.getInstance().TcpOutPacketPool;
			}
		}

		private TCPClientPool tcpClientPool
		{
			get
			{
				return TCPManager.getInstance().tcpClientPool;
			}
		}

		public SaleCmdsProcessor(TCPGameServerCmds cmdID)
		{
			this.CmdID = cmdID;
		}

		public static SaleCmdsProcessor getInstance(TCPGameServerCmds cmdID)
		{
			return new SaleCmdsProcessor(cmdID);
		}

		private bool CanUseMarket(GameClient client)
		{
			bool result;
			try
			{
				if (Global.TradeLevelLimit(client))
				{
					result = false;
				}
				else
				{
					result = true;
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int cmdID = (int)this.CmdID;
			bool result;
			switch (this.CmdID)
			{
			case TCPGameServerCmds.CMD_SPR_OPENMARKET2:
				result = this.OpenMarket(client, cmdParams);
				break;
			case TCPGameServerCmds.CMD_SPR_MARKETSALEMONEY2:
				result = this.MarketSaleMoney(client, cmdParams);
				break;
			case TCPGameServerCmds.CMD_SPR_SALEGOODS2:
				result = this.SaleGoods(client, cmdParams);
				break;
			case TCPGameServerCmds.CMD_SPR_SELFSALEGOODSLIST2:
				result = this.SelfSaleGoodsList(client, cmdParams);
				break;
			case TCPGameServerCmds.CMD_SPR_OTHERSALEGOODSLIST2:
				result = this.OtherSaleGoodsList(client, cmdParams);
				break;
			case TCPGameServerCmds.CMD_SPR_MARKETROLELIST2:
				result = this.MarketRoleList(client, cmdParams);
				break;
			case TCPGameServerCmds.CMD_SPR_MARKETGOODSLIST2:
				result = this.MarketGoodsList(client, cmdParams);
				break;
			case TCPGameServerCmds.CMD_SPR_MARKETBUYGOODS2:
				result = this.MarketBuyGoods(client, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		private bool OpenMarket(GameClient client, string[] fields)
		{
			int num = Convert.ToInt32(fields[0]);
			int num2 = Convert.ToInt32(fields[1]);
			string text = fields[2];
			bool result;
			if (string.IsNullOrEmpty(text))
			{
				client.ClientData.AllowMarketBuy = false;
				client.ClientData.MarketName = "";
				string cmdData = string.Format("{0}:{1}:{2}", num, text, num2);
				client.sendCmd((int)this.CmdID, cmdData, false);
				result = true;
			}
			else
			{
				text = text.Substring(0, Math.Min(10, text.Length));
				if (client.ClientData.SaleGoodsDataList.Count <= 0)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(578, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = true;
				}
				else if (!Global.AllowOpenMarket(client))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(579, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = true;
				}
				else
				{
					client.ClientData.AllowMarketBuy = true;
					client.ClientData.OfflineMarketState = 1;
					client.ClientData.MarketName = text;
					result = true;
				}
			}
			return result;
		}

		private bool MarketSaleMoney(GameClient client, string[] fields)
		{
			int num = Convert.ToInt32(fields[0]);
			int num2 = Math.Max(0, Convert.ToInt32(fields[1]));
			int num3 = Math.Max(0, Convert.ToInt32(fields[2]));
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("disable-market", 0);
				if (gameConfigItemInt > 0)
				{
					result = true;
				}
				else if (!this.CanUseMarket(client))
				{
					result = true;
				}
				else if (SingletonTemplate<TradeBlackManager>.Instance().IsBanTrade(client.ClientData.RoleID))
				{
					string lang = GLang.GetLang(580, new object[0]);
					GameManager.ClientMgr.NotifyImportantMsg(this.tcpMgr.MySocketListener, this.pool, client, lang, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = true;
				}
				else if (!SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, MoneyTypes.YuanBao, true))
				{
					result = true;
				}
				else if (num2 > client.ClientData.YinLiang)
				{
					string text = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1,
						num,
						num2,
						num3,
						0
					});
					client.sendCmd((int)this.CmdID, text, false);
					result = true;
				}
				else if (!GameManager.ClientMgr.SubUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, num2, "交易市场一", false))
				{
					string text = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-2,
						num,
						num2,
						num3,
						0
					});
					client.sendCmd((int)this.CmdID, text, false);
					result = true;
				}
				else
				{
					GoodsData newGoodsData = Global.GetNewGoodsData(50200, 0);
					newGoodsData.Site = -1;
					newGoodsData.SaleMoney1 = 0;
					newGoodsData.SaleYuanBao = num3;
					newGoodsData.SaleYinPiao = 0;
					newGoodsData.Quality = num2;
					Global.AddSaleGoodsData(client, newGoodsData);
					int num4 = Global.AddGoodsDBCommand_Hook(this.pool, client, newGoodsData.GoodsID, newGoodsData.GCount, newGoodsData.Quality, newGoodsData.Props, newGoodsData.Forge_level, newGoodsData.Forge_level, newGoodsData.Site, newGoodsData.Jewellist, false, 0, "临时摆摊需要", false, "1900-01-01 12:00:00", newGoodsData.AddPropIndex, newGoodsData.BornIndex, newGoodsData.Lucky, newGoodsData.Strong, newGoodsData.ExcellenceInfo, newGoodsData.AppendPropLev, newGoodsData.ChangeLifeLevForEquip, false, null, null, "1900-01-01 12:00:00", 0, true);
					if (num4 < 0)
					{
						string text = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-3,
							num,
							num2,
							num3,
							newGoodsData.Id
						});
						client.sendCmd((int)this.CmdID, text, false);
						result = true;
					}
					else
					{
						newGoodsData.Id = num4;
						string[] array = null;
						string text = Global.FormatUpdateDBGoodsStr(new object[]
						{
							num,
							num4,
							"*",
							"*",
							"*",
							"*",
							newGoodsData.Site,
							"*",
							"*",
							"*",
							"*",
							"*",
							newGoodsData.SaleMoney1,
							newGoodsData.SaleYuanBao,
							newGoodsData.SaleYinPiao,
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*"
						});
						TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(this.tcpClientPool, this.pool, 10006, text, out array, client.ServerId);
						if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
						{
							text = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								-4,
								num,
								num2,
								num3,
								newGoodsData.Id
							});
							client.sendCmd((int)this.CmdID, text, false);
							result = true;
						}
						else if (array.Length <= 0 || Convert.ToInt32(array[1]) < 0)
						{
							text = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								-5,
								num,
								num2,
								num3,
								newGoodsData.Id
							});
							client.sendCmd((int)this.CmdID, text, false);
							result = true;
						}
						else
						{
							Global.ModRoleGoodsEvent(client, newGoodsData, 0, "铜钱交易上架", false);
							EventLogManager.AddGoodsEvent(client, OpTypes.Move, OpTags.None, newGoodsData.GoodsID, (long)newGoodsData.Id, 0, newGoodsData.GCount, "铜钱交易上架");
							SaleGoodsItem saleGoodsItem = new SaleGoodsItem
							{
								GoodsDbID = newGoodsData.Id,
								SalingGoodsData = newGoodsData,
								Client = client
							};
							SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
							if (1 == client.ClientData.SaleGoodsDataList.Count)
							{
								SaleRoleManager.AddSaleRoleItem(client);
							}
							client.ClientData.AllowMarketBuy = true;
							client.ClientData.OfflineMarketState = 1;
							text = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								0,
								num,
								num2,
								num3,
								newGoodsData.Id
							});
							client.sendCmd((int)this.CmdID, text, false);
							result = true;
						}
					}
				}
			}
			return result;
		}

		private bool SaleGoods(GameClient client, string[] fields)
		{
			TCPGameServerCmds cmdID = this.CmdID;
			int num = Convert.ToInt32(fields[0]);
			int num2 = Convert.ToInt32(fields[1]);
			int num3 = Convert.ToInt32(fields[2]);
			int num4 = Convert.ToInt32(fields[3]);
			int num5 = Convert.ToInt32(fields[4]);
			int num6 = Convert.ToInt32(fields[5]);
			int num7 = Convert.ToInt32(fields[6]);
			List<int> list = new List<int>
			{
				num4,
				num5,
				num6
			};
			int num8 = 0;
			foreach (int num9 in list)
			{
				if (num9 != 0 && num8++ > 1)
				{
					return true;
				}
			}
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("disable-market", 0);
				if (gameConfigItemInt > 0)
				{
					result = true;
				}
				else if (!this.CanUseMarket(client))
				{
					result = true;
				}
				else if (SingletonTemplate<TradeBlackManager>.Instance().IsBanTrade(client.ClientData.RoleID))
				{
					string lang = GLang.GetLang(580, new object[0]);
					GameManager.ClientMgr.NotifyImportantMsg(this.tcpMgr.MySocketListener, this.pool, client, lang, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = true;
				}
				else if (num5 > 0 && !SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, MoneyTypes.YuanBao, true))
				{
					result = true;
				}
				else if (num6 > 0 && !SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, MoneyTypes.MoBi, true))
				{
					result = true;
				}
				else if (num4 > 0 && !SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, MoneyTypes.YinLiang, true))
				{
					result = true;
				}
				else
				{
					int num10 = 0;
					GoodsData goodsData = RebornEquip.GetRebornGoodsByDbID(client, num2);
					string text;
					if (goodsData == null)
					{
						goodsData = Global.GetGoodsByDbID(client, num2);
						if (null == goodsData)
						{
							goodsData = Global.GetSaleGoodsDataByDbID(client, num2);
							if (null == goodsData)
							{
								LogManager.WriteLog(2, string.Format("从交易市场定位物品对象失败, CMD={0}, Client={1}, RoleID={2}, GoodsDbID={3}", new object[]
								{
									cmdID,
									Global.GetSocketRemoteEndPoint(client.ClientSocket, false),
									num,
									num2
								}), null, true);
								return true;
							}
							if (RebornEquip.IsRebornType(goodsData.GoodsID) && num3 != 15000)
							{
								return true;
							}
							if (SaleGoodsManager.RemoveSaleGoodsItem(num2) == null && null == LiXianBaiTanManager.RemoveLiXianSaleGoodsItem(num2))
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(2623, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								return true;
							}
							if (!RebornEquip.IsRebornType(goodsData.GoodsID))
							{
								if (!Global.CanAddGoods(client, goodsData.GoodsID, goodsData.GCount, goodsData.Binding, goodsData.Endtime, true, false))
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(581, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
									return true;
								}
								num10 = Global.GetIdleSlotOfBagGoods(client);
							}
							else
							{
								if (!RebornEquip.CanAddGoodsToReborn(client, goodsData.GoodsID, goodsData.GCount, goodsData.Binding, goodsData.Endtime, true, false))
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(7000, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
									return true;
								}
								num10 = RebornEquip.GetIdleSlotOfRebornGoods(client);
							}
						}
						else
						{
							if (goodsData.Using > 0)
							{
								text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
								{
									-7,
									num,
									num2,
									num3,
									num4,
									num5
								});
								client.sendCmd(654, text, false);
								return true;
							}
							if (goodsData.Binding > 0)
							{
								text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
								{
									-100,
									num,
									num2,
									num3,
									num4,
									num5
								});
								client.sendCmd(654, text, false);
								return true;
							}
							if (!Global.CanExchangeCategoriy(goodsData))
							{
								text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
								{
									-100,
									num,
									num2,
									num3,
									num4,
									num5
								});
								client.sendCmd(654, text, false);
								return true;
							}
							if (Global.IsTimeLimitGoods(goodsData))
							{
								text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
								{
									-101,
									num,
									num2,
									num3,
									num4,
									num5
								});
								client.sendCmd(654, text, false);
								return true;
							}
							if (Global.GetSaleGoodsDataCount(client) >= SaleManager.MaxSaleNum)
							{
								text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
								{
									-110,
									num,
									num2,
									num3,
									num4,
									num5
								});
								client.sendCmd(654, text, false);
								return true;
							}
							int goodsGridNumByID = Global.GetGoodsGridNumByID(goodsData.GoodsID);
							if (goodsGridNumByID > 1 && num7 > 0 && num7 < goodsData.GCount)
							{
								if (TCPProcessCmdResults.RESULT_OK != Global.SplitGoodsByCmdParams(client, client.ClientSocket, 133, num, goodsData.Id, goodsData.Site, goodsData.GoodsID, goodsData.GCount - num7, false))
								{
									text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
									{
										-201,
										num,
										num2,
										num3,
										num4,
										num5
									});
									client.sendCmd(654, text, false);
									return true;
								}
							}
						}
					}
					else
					{
						if (goodsData.Using > 0)
						{
							text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
							{
								-7,
								num,
								num2,
								num3,
								num4,
								num5
							});
							client.sendCmd(654, text, false);
							return true;
						}
						if (goodsData.Binding > 0)
						{
							text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
							{
								-100,
								num,
								num2,
								num3,
								num4,
								num5
							});
							client.sendCmd(654, text, false);
							return true;
						}
						if (Global.IsTimeLimitGoods(goodsData))
						{
							text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
							{
								-101,
								num,
								num2,
								num3,
								num4,
								num5
							});
							client.sendCmd(654, text, false);
							return true;
						}
						if (Global.GetSaleGoodsDataCount(client) >= SaleManager.MaxSaleNum)
						{
							text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
							{
								-110,
								num,
								num2,
								num3,
								num4,
								num5
							});
							client.sendCmd(654, text, false);
							return true;
						}
						int goodsGridNumByID = Global.GetGoodsGridNumByID(goodsData.GoodsID);
						if (goodsGridNumByID > 1 && num7 > 0 && num7 < goodsData.GCount)
						{
							if (TCPProcessCmdResults.RESULT_OK != Global.SplitGoodsByCmdParams(client, client.ClientSocket, 133, num, goodsData.Id, goodsData.Site, goodsData.GoodsID, goodsData.GCount - num7, false))
							{
								text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
								{
									-201,
									num,
									num2,
									num3,
									num4,
									num5
								});
								client.sendCmd(654, text, false);
								return true;
							}
						}
					}
					string[] array = null;
					text = Global.FormatUpdateDBGoodsStr(new object[]
					{
						num,
						num2,
						"*",
						"*",
						"*",
						"*",
						num3,
						"*",
						"*",
						"*",
						"*",
						num10,
						num4,
						num5,
						num6,
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*"
					});
					TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(this.tcpClientPool, this.pool, 10006, text, out array, client.ServerId);
					if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
					{
						text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
						{
							-1,
							num,
							num2,
							num3,
							num4,
							num5,
							num6
						});
						client.sendCmd(654, text, false);
						result = true;
					}
					else if (array.Length <= 0 || Convert.ToInt32(array[1]) < 0)
					{
						text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
						{
							-10,
							num,
							num2,
							num3,
							num4,
							num5,
							num6
						});
						client.sendCmd(654, text, false);
						result = true;
					}
					else
					{
						goodsData.BagIndex = num10;
						if (goodsData.Site != num3)
						{
							if (goodsData.Site == 0 && num3 == -1)
							{
								Global.RemoveGoodsData(client, goodsData);
								goodsData.Site = num3;
								goodsData.SaleMoney1 = num4;
								goodsData.SaleYuanBao = num5;
								goodsData.SaleYinPiao = num6;
								Global.AddSaleGoodsData(client, goodsData);
								Global.ModRoleGoodsEvent(client, goodsData, 0, "交易上架", false);
								EventLogManager.AddGoodsEvent(client, OpTypes.Move, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "交易上架");
								SaleGoodsItem saleGoodsItem = new SaleGoodsItem
								{
									GoodsDbID = goodsData.Id,
									SalingGoodsData = goodsData,
									Client = client
								};
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								if (1 == client.ClientData.SaleGoodsDataList.Count)
								{
									SaleRoleManager.AddSaleRoleItem(client);
								}
								client.ClientData.AllowMarketBuy = true;
								client.ClientData.OfflineMarketState = 1;
							}
							else if (goodsData.Site == -1 && num3 == 0)
							{
								SaleGoodsManager.RemoveSaleGoodsItem(goodsData.Id);
								Global.RemoveSaleGoodsData(client, goodsData);
								if (50200 != goodsData.GoodsID)
								{
									goodsData.Site = num3;
									goodsData.SaleMoney1 = 0;
									goodsData.SaleYuanBao = 0;
									goodsData.SaleYinPiao = 0;
									Global.AddGoodsData(client, goodsData);
									Global.ModRoleGoodsEvent(client, goodsData, 0, "交易下架", false);
									EventLogManager.AddGoodsEvent(client, OpTypes.Move, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "交易下架");
								}
								else if (GameManager.ClientMgr.NotifyUseGoods(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, goodsData, goodsData.GCount, false, true))
								{
									int num11 = Math.Max(0, goodsData.Quality);
									if (num11 > 0)
									{
										GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, num11, "金币下架", false);
									}
									Global.ModRoleGoodsEvent(client, goodsData, 0, "铜钱交易下架", false);
									EventLogManager.AddGoodsEvent(client, OpTypes.Move, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "铜钱交易下架");
								}
								if (0 == client.ClientData.SaleGoodsDataList.Count)
								{
									SaleRoleManager.RemoveSaleRoleItem(client.ClientData.RoleID);
								}
							}
							else if (goodsData.Site == 15000 && num3 == -1)
							{
								RebornEquip.RemoveGoodsData(client, goodsData);
								goodsData.Site = num3;
								goodsData.SaleMoney1 = num4;
								goodsData.SaleYuanBao = num5;
								goodsData.SaleYinPiao = num6;
								Global.AddSaleGoodsData(client, goodsData);
								Global.ModRoleGoodsEvent(client, goodsData, 0, "交易上架", false);
								EventLogManager.AddGoodsEvent(client, OpTypes.Move, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "交易上架");
								SaleGoodsItem saleGoodsItem = new SaleGoodsItem
								{
									GoodsDbID = goodsData.Id,
									SalingGoodsData = goodsData,
									Client = client
								};
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								if (1 == client.ClientData.SaleGoodsDataList.Count)
								{
									SaleRoleManager.AddSaleRoleItem(client);
								}
								client.ClientData.AllowMarketBuy = true;
								client.ClientData.OfflineMarketState = 1;
							}
							else if (goodsData.Site == -1 && num3 == 15000)
							{
								SaleGoodsManager.RemoveSaleGoodsItem(goodsData.Id);
								Global.RemoveSaleGoodsData(client, goodsData);
								if (50200 != goodsData.GoodsID)
								{
									goodsData.Site = num3;
									goodsData.SaleMoney1 = 0;
									goodsData.SaleYuanBao = 0;
									goodsData.SaleYinPiao = 0;
									RebornEquip.AddGoodsData(client, goodsData);
									Global.ModRoleGoodsEvent(client, goodsData, 0, "交易下架", false);
									EventLogManager.AddGoodsEvent(client, OpTypes.Move, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "交易下架");
								}
								else if (GameManager.ClientMgr.NotifyUseGoods(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, goodsData, goodsData.GCount, false, true))
								{
									int num11 = Math.Max(0, goodsData.Quality);
									if (num11 > 0)
									{
										GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, num11, "金币下架", false);
									}
									Global.ModRoleGoodsEvent(client, goodsData, 0, "铜钱交易下架", false);
									EventLogManager.AddGoodsEvent(client, OpTypes.Move, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "铜钱交易下架");
								}
								if (0 == client.ClientData.SaleGoodsDataList.Count)
								{
									SaleRoleManager.RemoveSaleRoleItem(client.ClientData.RoleID);
								}
							}
						}
						text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
						{
							0,
							num,
							num2,
							num3,
							num4,
							num5,
							num6
						});
						client.sendCmd(654, text, false);
						result = true;
					}
				}
			}
			return result;
		}

		private bool SelfSaleGoodsList(GameClient client, string[] fields)
		{
			int num = Convert.ToInt32(fields[0]);
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				client.sendCmd<List<GoodsData>>(655, null, false);
				result = true;
			}
			else
			{
				List<GoodsData> saleGoodsDataList = client.ClientData.SaleGoodsDataList;
				client.sendCmd<List<GoodsData>>(655, saleGoodsDataList, false);
				result = true;
			}
			return result;
		}

		private bool OtherSaleGoodsList(GameClient client, string[] fields)
		{
			int num = Convert.ToInt32(fields[0]);
			int roleID = Convert.ToInt32(fields[1]);
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				client.sendCmd<List<GoodsData>>(656, null, false);
				result = true;
			}
			else
			{
				List<GoodsData> cmdData = new List<GoodsData>();
				GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
				if (null != gameClient)
				{
					cmdData = gameClient.ClientData.SaleGoodsDataList;
				}
				else
				{
					cmdData = LiXianBaiTanManager.GetLiXianSaleGoodsList(roleID);
				}
				client.sendCmd<List<GoodsData>>(656, cmdData, false);
				result = true;
			}
			return result;
		}

		private bool MarketRoleList(GameClient client, string[] fields)
		{
			List<SaleRoleData> saleRoleDataList = SaleRoleManager.GetSaleRoleDataList();
			client.sendCmd<List<SaleRoleData>>(657, saleRoleDataList, false);
			return true;
		}

		private bool MarketGoodsList(GameClient client, string[] fields)
		{
			int num = Convert.ToInt32(fields[0]);
			int num2 = Convert.ToInt32(fields[1]);
			int num3 = Convert.ToInt32(fields[2]);
			int num4 = Convert.ToInt32(fields[3]);
			string text = fields[4];
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				client.sendCmd<SaleGoodsSearchResultData>(658, null, false);
				result = true;
			}
			else if (SingletonTemplate<CreateRoleLimitManager>.Instance().RefreshMarketSlotTicks > 0 && TimeUtil.NOW() - client.ClientData._RefreshMarketTicks < (long)SingletonTemplate<CreateRoleLimitManager>.Instance().RefreshMarketSlotTicks)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(129, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				client.ClientData._RefreshMarketTicks = TimeUtil.NOW();
				SaleGoodsSearchResultData saleGoodsSearchResultData = new SaleGoodsSearchResultData();
				if (0 == num2)
				{
					saleGoodsSearchResultData.saleGoodsDataList = SaleGoodsManager.GetSaleGoodsDataList();
				}
				else if (2 == num2)
				{
					Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
					string[] array = text.Split(new char[]
					{
						','
					});
					if (array != null && array.Length > 0)
					{
						for (int i = 0; i < array.Length; i++)
						{
							int key = Global.SafeConvertToInt32(array[i]);
							dictionary[key] = true;
						}
						saleGoodsSearchResultData.saleGoodsDataList = SaleGoodsManager.FindSaleGoodsDataList(dictionary);
					}
				}
				else if (1 == num2)
				{
					saleGoodsSearchResultData.saleGoodsDataList = SaleGoodsManager.FindSaleGoodsDataListByRoleName(text);
				}
				else if (3 == num2)
				{
					string[] array2 = text.Split(new char[]
					{
						'$'
					});
					if (array2.Length >= 6)
					{
						int type = Global.SafeConvertToInt32(array2[0]);
						int id = Global.SafeConvertToInt32(array2[1]);
						int num5 = Global.SafeConvertToInt32(array2[2]);
						int num6 = Global.SafeConvertToInt32(array2[3]);
						int orderBy = Global.SafeConvertToInt32(array2[4]);
						int orderTypeFlags = 1;
						List<int> list;
						if (array2.Length >= 7)
						{
							orderTypeFlags = Global.SafeConvertToInt32(array2[5]);
							list = Global.StringToIntList(array2[6], '#');
						}
						else
						{
							list = Global.StringToIntList(array2[5], '#');
						}
						if (!SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, MoneyTypes.YuanBao, false))
						{
							num5 &= 5;
						}
						if (!SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, MoneyTypes.MoBi, false))
						{
							num5 &= 3;
						}
						if (!SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, MoneyTypes.YinLiang, false))
						{
							num5 &= 6;
						}
						saleGoodsSearchResultData.Type = type;
						saleGoodsSearchResultData.ID = id;
						saleGoodsSearchResultData.MoneyFlags = num5;
						saleGoodsSearchResultData.ColorFlags = num6;
						saleGoodsSearchResultData.OrderBy = orderBy;
						if (num5 <= 0)
						{
							num5 = 7;
						}
						if (num6 <= 0)
						{
							num6 = 63;
						}
						SearchArgs searchArgs = new SearchArgs(id, type, num5, num6, orderBy, orderTypeFlags);
						if (ListExt.IsNullOrEmpty<int>(list))
						{
							saleGoodsSearchResultData.saleGoodsDataList = SaleManager.GetSaleGoodsDataList(searchArgs, null);
							if (null != saleGoodsSearchResultData.saleGoodsDataList)
							{
								saleGoodsSearchResultData.TotalCount = saleGoodsSearchResultData.saleGoodsDataList.Count;
							}
						}
						else
						{
							saleGoodsSearchResultData.saleGoodsDataList = SaleManager.GetSaleGoodsDataList(searchArgs, list);
							if (saleGoodsSearchResultData.saleGoodsDataList == null || saleGoodsSearchResultData.saleGoodsDataList.Count == 0)
							{
								saleGoodsSearchResultData.TotalCount = -1;
							}
							else
							{
								saleGoodsSearchResultData.TotalCount = saleGoodsSearchResultData.saleGoodsDataList.Count;
							}
						}
						if (saleGoodsSearchResultData.saleGoodsDataList != null && saleGoodsSearchResultData.saleGoodsDataList.Count > 0)
						{
							saleGoodsSearchResultData.StartIndex = num3;
							if (num3 >= saleGoodsSearchResultData.TotalCount)
							{
								saleGoodsSearchResultData.saleGoodsDataList = null;
							}
							else
							{
								num3 = Global.GMin(num3, saleGoodsSearchResultData.saleGoodsDataList.Count - 1);
								num4 = Global.GMin(num4, saleGoodsSearchResultData.saleGoodsDataList.Count - num3);
								saleGoodsSearchResultData.saleGoodsDataList = saleGoodsSearchResultData.saleGoodsDataList.GetRange(num3, num4);
							}
						}
					}
				}
				client.sendCmd<SaleGoodsSearchResultData>(658, saleGoodsSearchResultData, false);
				result = true;
			}
			return result;
		}

		private int CalcRealMoneyAfterTax(int money, MoneyTypes moneyType, out int tax)
		{
			tax = 0;
			if (moneyType == MoneyTypes.YinLiang)
			{
				tax = (int)Math.Ceiling((double)money * SaleManager.JiaoYiShuiJinBi);
				tax = Global.GMax(tax, 0);
			}
			else if (moneyType == MoneyTypes.YuanBao)
			{
				tax = (int)Math.Ceiling((double)money * SaleManager.JiaoYiShuiZuanShi);
				tax = Global.GMax(tax, 0);
			}
			else if (moneyType == MoneyTypes.MoBi)
			{
				tax = (int)Math.Ceiling((double)money * SaleManager.JiaoYiShuiMoBi);
				tax = Global.GMax(tax, 0);
			}
			return money - tax;
		}

		private bool CheckBuyParams(GoodsData SalingGoodsData, int clientMoneyType, int clientMoneyValue)
		{
			return clientMoneyValue > 0 && ((clientMoneyType == 8 && SalingGoodsData.SaleMoney1 == clientMoneyValue) || (clientMoneyType == 40 && SalingGoodsData.SaleYuanBao == clientMoneyValue) || (clientMoneyType == 141 && SalingGoodsData.SaleYinPiao == clientMoneyValue));
		}

		private bool MarketBuyGoods(GameClient client, string[] fields)
		{
			int num = Convert.ToInt32(fields[0]);
			int num2 = Convert.ToInt32(fields[1]);
			int num3 = Convert.ToInt32(fields[2]);
			int num4 = Convert.ToInt32(fields[3]);
			int clientMoneyValue = Convert.ToInt32(fields[4]);
			int num5 = 0;
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("disable-market", 0);
				if (gameConfigItemInt > 0)
				{
					result = true;
				}
				else if (!this.CanUseMarket(client))
				{
					result = true;
				}
				else if (SingletonTemplate<TradeBlackManager>.Instance().IsBanTrade(client.ClientData.RoleID))
				{
					string lang = GLang.GetLang(580, new object[0]);
					GameManager.ClientMgr.NotifyImportantMsg(this.tcpMgr.MySocketListener, this.pool, client, lang, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = true;
				}
				else if (!SingletonTemplate<TradeBlackManager>.Instance().CheckTrade(client, (MoneyTypes)num4, true))
				{
					result = true;
				}
				else
				{
					int roleid = 0;
					GameClient gameClient = null;
					SaleGoodsItem saleGoodsItem = SaleGoodsManager.RemoveSaleGoodsItem(num2);
					if (null != saleGoodsItem)
					{
						gameClient = GameManager.ClientMgr.FindClient(saleGoodsItem.Client.ClientData.RoleID);
						if (null != gameClient)
						{
							if (gameClient.ClientData.RoleID == client.ClientData.RoleID)
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								if (GameManager.PlatConfigMgr.GetGameConfigItemStr("CanBuySelfGoods", "1") == "1")
								{
									int num6 = RebornEquip.IsRebornType(saleGoodsItem.SalingGoodsData.GoodsID) ? 15000 : 0;
									int[] array = new int[7];
									array[0] = num;
									array[1] = num2;
									array[2] = num6;
									int[] array2 = array;
									this.SaleGoods(client, Array.ConvertAll<int, string>(array2, (int x) => x.ToString()));
								}
								else
								{
									GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -30, 0, num2, num3, (int)this.CmdID);
								}
								return true;
							}
							if (!gameClient.ClientData.AllowMarketBuy)
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -3, 0, num2, num3, (int)this.CmdID);
								return true;
							}
						}
						roleid = saleGoodsItem.Client.ClientData.RoleID;
					}
					int num8;
					if (saleGoodsItem != null && null != gameClient)
					{
						if (SingletonTemplate<TradeBlackManager>.Instance().IsBanTrade(gameClient.ClientData.RoleID))
						{
							SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
							string lang = GLang.GetLang(582, new object[0]);
							GameManager.ClientMgr.NotifyImportantMsg(this.tcpMgr.MySocketListener, this.pool, client, lang, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							return true;
						}
						if (!this.CheckBuyParams(saleGoodsItem.SalingGoodsData, num4, clientMoneyValue))
						{
							SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -40, 0, num2, num3, (int)this.CmdID);
							return true;
						}
						GoodsData goodsData = Global.GetSaleGoodsDataByDbID(gameClient, num2);
						if (null == goodsData)
						{
							if (null == goodsData)
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -3, 0, num2, num3, (int)this.CmdID);
								return true;
							}
						}
						if (goodsData.GoodsID != num3 || goodsData.Binding != 0)
						{
							SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -1003, 0, num2, num3, (int)this.CmdID);
							return true;
						}
						if (50200 != goodsData.GoodsID)
						{
							if (!RebornEquip.IsRebornType(goodsData.GoodsID) && !Global.CanAddGoods(client, goodsData.GoodsID, goodsData.GCount, 0, goodsData.Endtime, true, false))
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -5, 0, num2, num3, (int)this.CmdID);
								return true;
							}
							if (RebornEquip.IsRebornType(goodsData.GoodsID) && !RebornEquip.CanAddGoodsToReborn(client, goodsData.GoodsID, goodsData.GCount, 0, goodsData.Endtime, true, false))
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -5, 0, num2, num3, (int)this.CmdID);
								return true;
							}
						}
						if (saleGoodsItem.SalingGoodsData.SaleMoney1 > 0 && client.ClientData.YinLiang < saleGoodsItem.SalingGoodsData.SaleMoney1)
						{
							SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -10, 0, num2, num3, (int)this.CmdID);
							return true;
						}
						if (saleGoodsItem.SalingGoodsData.SaleYuanBao > 0 && client.ClientData.UserMoney < saleGoodsItem.SalingGoodsData.SaleYuanBao)
						{
							SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -20, 0, num2, num3, (int)this.CmdID);
							return true;
						}
						int num7 = -1;
						if (141 == num4)
						{
							if (saleGoodsItem.SalingGoodsData.SaleYinPiao > 0 && client.ClientData.MoBi < saleGoodsItem.SalingGoodsData.SaleYinPiao)
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -11, 0, num2, num3, (int)this.CmdID);
								return true;
							}
						}
						else if (142 == num4)
						{
							num7 = (int)GameManager.systemParamsList.GetParamValueIntByName("YinPiaoGoodsID", -1);
							if (saleGoodsItem.SalingGoodsData.SaleYinPiao > 0 && Global.GetTotalGoodsCountByID(client, num7) < saleGoodsItem.SalingGoodsData.SaleYinPiao)
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -21, 0, num2, num3, (int)this.CmdID);
								return true;
							}
						}
						if (saleGoodsItem.SalingGoodsData.SaleYinPiao > 0 && 142 == num4)
						{
							if (!RebornEquip.IsRebornType(goodsData.GoodsID) && !Global.CanAddGoods2(gameClient, num7, saleGoodsItem.SalingGoodsData.SaleYinPiao, 0, "1900-01-01 12:00:00", true))
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -22, 0, num2, num3, (int)this.CmdID);
								return true;
							}
							if (RebornEquip.IsRebornType(goodsData.GoodsID) && !RebornEquip.CanAddGoodsDataList2(gameClient, num7, saleGoodsItem.SalingGoodsData.SaleYinPiao, 0, "1900-01-01 12:00:00", true))
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -22, 0, num2, num3, (int)this.CmdID);
								return true;
							}
						}
						GameManager.logDBCmdMgr.AddMessageLog(-1, "交易日志", "交易市场", gameClient.ClientData.RoleName, client.ClientData.RoleName, "交易", client.ClientData.RoleID, client.ClientData.ZoneID, client.strUserID, gameClient.ClientData.RoleID, GameManager.ServerId, "");
						if (saleGoodsItem.SalingGoodsData.SaleMoney1 > 0)
						{
							if (!GameManager.ClientMgr.SubUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, saleGoodsItem.SalingGoodsData.SaleMoney1, "交易市场二", false))
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -10, 0, num2, num3, (int)this.CmdID);
								return true;
							}
							GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, gameClient, this.CalcRealMoneyAfterTax(saleGoodsItem.SalingGoodsData.SaleMoney1, MoneyTypes.YinLiang, out num5), "交易市场二", false);
						}
						if (saleGoodsItem.SalingGoodsData.SaleYuanBao > 0)
						{
							if (!Global.CanTrade(client))
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -20, 0, num2, num3, (int)this.CmdID);
								return true;
							}
							if (!GameManager.ClientMgr.SubUserMoney(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, saleGoodsItem.SalingGoodsData.SaleYuanBao, "新交易市场购买", false, true, false, DaiBiSySType.None))
							{
								SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -20, 0, num2, num3, (int)this.CmdID);
								return true;
							}
							GameManager.ClientMgr.AddUserMoney(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, gameClient, this.CalcRealMoneyAfterTax(saleGoodsItem.SalingGoodsData.SaleYuanBao, MoneyTypes.YuanBao, out num5), "新交易市场出售", ActivityTypes.None, "");
						}
						if (141 == num4)
						{
							if (saleGoodsItem.SalingGoodsData.SaleYinPiao > 0)
							{
								if (!GameManager.ClientMgr.ModifyMoBiValue(client, -saleGoodsItem.SalingGoodsData.SaleYinPiao, "新交易市场购买", false))
								{
									SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
									GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -11, 0, num2, num3, (int)this.CmdID);
									return true;
								}
								GameManager.ClientMgr.ModifyMoBiValue(gameClient, this.CalcRealMoneyAfterTax(saleGoodsItem.SalingGoodsData.SaleYinPiao, MoneyTypes.MoBi, out num5), "新交易市场出售", false);
							}
						}
						else if (142 == num4)
						{
							if (saleGoodsItem.SalingGoodsData.SaleYinPiao > 0)
							{
								bool flag = false;
								bool flag2 = false;
								if (!GameManager.ClientMgr.NotifyUseGoods(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, num7, saleGoodsItem.SalingGoodsData.SaleYinPiao, false, out flag, out flag2, false))
								{
									SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
									GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -21, 0, num2, num3, (int)this.CmdID);
									return true;
								}
								Global.BatchAddGoods(gameClient, num7, this.CalcRealMoneyAfterTax(saleGoodsItem.SalingGoodsData.SaleYinPiao, MoneyTypes.None, out num5), 0, "交易市场购买后批量添加");
							}
						}
						num8 = saleGoodsItem.SalingGoodsData.SaleYuanBao + saleGoodsItem.SalingGoodsData.SaleYinPiao;
						int saleMoney = goodsData.SaleMoney1;
						int saleYuanBao = goodsData.SaleYuanBao;
						int saleYinPiao = goodsData.SaleYinPiao;
						int site = goodsData.Site;
						GoodsData saleGoods = new GoodsData(goodsData);
						int num9 = Math.Max(0, goodsData.Quality);
						goodsData.SaleMoney1 = 0;
						goodsData.SaleYuanBao = 0;
						goodsData.SaleYinPiao = 0;
						if (RebornEquip.IsRebornType(goodsData.GoodsID))
						{
							goodsData.Site = 15000;
						}
						else
						{
							goodsData.Site = 0;
						}
						Global.RemoveSaleGoodsData(gameClient, goodsData);
						bool flag3 = 50200 != goodsData.GoodsID;
						if (!GameManager.ClientMgr.MoveGoodsDataToOtherRole(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, goodsData, gameClient, client, flag3))
						{
							this.GiveBackSaleGoodsMoney(client, gameClient, goodsData, saleMoney, saleYuanBao, site);
							GameManager.SystemServerEvents.AddEvent(string.Format("转移物品时失败, 交易市场购买, FromRole={0}({1}), ToRole={2}({3}), GoodsDbID={4}, GoodsID={5}, GoodsNum={6}", new object[]
							{
								gameClient.ClientData.RoleID,
								gameClient.ClientData.RoleName,
								client.ClientData.RoleID,
								client.ClientData.RoleName,
								goodsData.Id,
								goodsData.GoodsID,
								goodsData.GCount
							}), EventLevels.Important);
							GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -100, 0, num2, num3, (int)this.CmdID);
							return true;
						}
						if (!flag3)
						{
							if (!GameManager.ClientMgr.NotifyUseGoods(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, goodsData, goodsData.GCount, false, true))
							{
								LogManager.WriteLog(3, string.Format("新交易市场在线购买金币失败, {0}=>{1}", Global.FormatRoleName4(gameClient), Global.FormatRoleName4(client)), null, true);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -1004, 0, num2, num3, (int)this.CmdID);
								return true;
							}
							GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, num9, "摆摊出售金币", false);
						}
						Global.AddRoleSaleEvent(client, goodsData.GoodsID, goodsData.GCount, -saleMoney, -saleYinPiao, -saleYuanBao, num7, -num9);
						Global.AddRoleSaleEvent(gameClient, goodsData.GoodsID, -goodsData.GCount, Math.Max(0, saleMoney - num5), Math.Max(0, saleYinPiao - num5), Math.Max(0, saleYuanBao - num5), num7, num9);
						GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, gameClient, client, 0, 1, num2, num3, (int)this.CmdID);
						GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, 0, 0, num2, num3, (int)this.CmdID);
						string warhprops = Convert.ToBase64String(DataHelper.ObjectToBytes<List<int>>(goodsData.WashProps));
						Global.AddMarketBuyLog(gameClient.ClientData.RoleID, client.ClientData.RoleID, client.ClientData.RoleName, goodsData.GoodsID, goodsData.GCount, goodsData.Forge_level, saleYuanBao, client.ClientData.UserMoney, saleMoney, saleYinPiao, num5, goodsData.ExcellenceInfo, warhprops);
						SingletonTemplate<TradeBlackManager>.Instance().OnMarketBuy(client.ClientData.RoleID, gameClient.ClientData.RoleID, saleGoods);
					}
					else
					{
						LiXianSaleGoodsItem liXianSaleGoodsItem = LiXianBaiTanManager.RemoveLiXianSaleGoodsItem(num2);
						if (null == liXianSaleGoodsItem)
						{
							GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, null, -1, 0, num2, num3, (int)this.CmdID);
							return true;
						}
						if (SingletonTemplate<TradeBlackManager>.Instance().IsBanTrade(liXianSaleGoodsItem.RoleID))
						{
							LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
							string lang = GLang.GetLang(582, new object[0]);
							GameManager.ClientMgr.NotifyImportantMsg(this.tcpMgr.MySocketListener, this.pool, client, lang, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							return true;
						}
						GoodsData goodsData = liXianSaleGoodsItem.SalingGoodsData;
						if (goodsData.GoodsID != num3 || goodsData.Binding != 0)
						{
							LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -1003, 0, num2, num3, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
							return true;
						}
						if (!this.CheckBuyParams(liXianSaleGoodsItem.SalingGoodsData, num4, clientMoneyValue))
						{
							LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -40, 0, num2, num3, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
							return true;
						}
						if (50200 != goodsData.GoodsID)
						{
							if (!RebornEquip.IsRebornType(goodsData.GoodsID) && !Global.CanAddGoods(client, goodsData.GoodsID, goodsData.GCount, 0, goodsData.Endtime, true, false))
							{
								LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -5, 0, num2, num3, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
								return true;
							}
							if (RebornEquip.IsRebornType(goodsData.GoodsID) && !RebornEquip.CanAddGoodsToReborn(client, goodsData.GoodsID, goodsData.GCount, 0, goodsData.Endtime, true, false))
							{
								LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -5, 0, num2, num3, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
								return true;
							}
						}
						if (liXianSaleGoodsItem.SalingGoodsData.SaleMoney1 > 0 && client.ClientData.YinLiang < liXianSaleGoodsItem.SalingGoodsData.SaleMoney1)
						{
							LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -10, 0, num2, num3, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
							return true;
						}
						if (liXianSaleGoodsItem.SalingGoodsData.SaleYuanBao > 0 && client.ClientData.UserMoney < liXianSaleGoodsItem.SalingGoodsData.SaleYuanBao)
						{
							LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
							GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -20, 0, num2, num3, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
							return true;
						}
						if (141 == num4)
						{
							if (liXianSaleGoodsItem.SalingGoodsData.SaleYinPiao > 0 && client.ClientData.MoBi < liXianSaleGoodsItem.SalingGoodsData.SaleYinPiao)
							{
								LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -11, 0, num2, num3, (int)this.CmdID);
								return true;
							}
						}
						GameManager.logDBCmdMgr.AddMessageLog(-1, "交易日志", "交易市场", liXianSaleGoodsItem.RoleName, client.ClientData.RoleName, "交易", liXianSaleGoodsItem.RoleID, client.ClientData.ZoneID, client.strUserID, client.ClientData.RoleID, GameManager.ServerId, "");
						if (50200 != goodsData.GoodsID)
						{
							if (liXianSaleGoodsItem.SalingGoodsData.SaleMoney1 > 0)
							{
								if (!GameManager.ClientMgr.SubUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, liXianSaleGoodsItem.SalingGoodsData.SaleMoney1, "交易市场三", false))
								{
									LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
									GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -10, 0, num2, num3, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
									return true;
								}
								GameManager.ClientMgr.AddOfflineUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, liXianSaleGoodsItem.UserID, liXianSaleGoodsItem.RoleID, liXianSaleGoodsItem.RoleName, this.CalcRealMoneyAfterTax(liXianSaleGoodsItem.SalingGoodsData.SaleMoney1, MoneyTypes.YinLiang, out num5), "交易市场三", client.ClientData.ZoneID);
							}
						}
						if (liXianSaleGoodsItem.SalingGoodsData.SaleYuanBao > 0)
						{
							if (!Global.CanTrade(client))
							{
								LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -20, 0, num2, num3, (int)this.CmdID);
								return true;
							}
							if (!GameManager.ClientMgr.SubUserMoney(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, liXianSaleGoodsItem.SalingGoodsData.SaleYuanBao, "新交易市场购买", false, true, false, DaiBiSySType.None))
							{
								LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
								GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -20, 0, num2, num3, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
								return true;
							}
							GameManager.ClientMgr.AddOfflineUserMoney(this.tcpClientPool, this.pool, liXianSaleGoodsItem.RoleID, liXianSaleGoodsItem.RoleName, this.CalcRealMoneyAfterTax(liXianSaleGoodsItem.SalingGoodsData.SaleYuanBao, MoneyTypes.YuanBao, out num5), "新交易市场出售(离线)", liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.UserID);
						}
						if (141 == num4)
						{
							if (liXianSaleGoodsItem.SalingGoodsData.SaleYinPiao > 0)
							{
								if (!GameManager.ClientMgr.ModifyMoBiValue(client, -liXianSaleGoodsItem.SalingGoodsData.SaleYinPiao, "新交易市场购买", false))
								{
									LiXianBaiTanManager.AddLiXianSaleGoodsItem(liXianSaleGoodsItem);
									GameManager.ClientMgr.NotifySpriteMarketBuy(this.tcpMgr.MySocketListener, this.pool, client, gameClient, -11, 0, num2, num3, (int)this.CmdID);
									return true;
								}
								GameManager.ClientMgr.ModifyMoBiValueOffline(liXianSaleGoodsItem.RoleID, liXianSaleGoodsItem.RoleName, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.UserID, this.CalcRealMoneyAfterTax(liXianSaleGoodsItem.SalingGoodsData.SaleYinPiao, MoneyTypes.MoBi, out num5), "新交易市场出售", false);
							}
						}
						num8 = liXianSaleGoodsItem.SalingGoodsData.SaleYuanBao;
						roleid = liXianSaleGoodsItem.RoleID;
						int saleMoney = goodsData.SaleMoney1;
						int saleYuanBao = goodsData.SaleYuanBao;
						int saleYinPiao = goodsData.SaleYinPiao;
						int site = goodsData.Site;
						GoodsData saleGoods = new GoodsData(goodsData);
						int num9 = Math.Max(0, goodsData.Quality);
						goodsData.SaleMoney1 = 0;
						goodsData.SaleYuanBao = 0;
						goodsData.SaleYinPiao = 0;
						if (RebornEquip.IsRebornType(goodsData.GoodsID))
						{
							goodsData.Site = 15000;
						}
						else
						{
							goodsData.Site = 0;
						}
						string text = GameManager.OnlineUserSession.FindUserID(client.ClientSocket);
						bool flag3 = 50200 != goodsData.GoodsID;
						if (!GameManager.ClientMgr.MoveGoodsDataToOfflineRole(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, goodsData, liXianSaleGoodsItem.UserID, liXianSaleGoodsItem.RoleID, liXianSaleGoodsItem.RoleName, liXianSaleGoodsItem.RoleLevel, text, client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.Level, flag3, client.ClientData.ZoneID))
						{
							this.GiveBackSaleGoodsMoneyOffline(client, liXianSaleGoodsItem.UserID, liXianSaleGoodsItem.RoleID, liXianSaleGoodsItem.RoleName, goodsData, saleMoney, saleYuanBao, site);
							GameManager.SystemServerEvents.AddEvent(string.Format("转移物品时失败, 交易市场购买, FromRole={0}({1}), ToRole={2}({3}), GoodsDbID={4}, GoodsID={5}, GoodsNum={6}", new object[]
							{
								liXianSaleGoodsItem.RoleID,
								liXianSaleGoodsItem.RoleName,
								client.ClientData.RoleID,
								client.ClientData.RoleName,
								goodsData.Id,
								goodsData.GoodsID,
								goodsData.GCount
							}), EventLevels.Important);
							GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -100, 0, num2, num3, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
							return true;
						}
						if (!flag3)
						{
							if (!GameManager.ClientMgr.NotifyUseGoods(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, goodsData, goodsData.GCount, false, true))
							{
								LogManager.WriteLog(3, string.Format("新交易市场购买金币失败, {0}=>{1}", liXianSaleGoodsItem.RoleName, Global.FormatRoleName4(client)), null, true);
								GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, -1004, 0, num2, num3, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
								return true;
							}
							GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, num9, "摆摊出售物品获取金币", false);
						}
						Global.AddRoleSaleEvent2(text, client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.Level, goodsData.GoodsID, goodsData.GCount, -saleMoney, -saleYinPiao, -saleYuanBao, -num9);
						Global.AddRoleSaleEvent2(liXianSaleGoodsItem.UserID, liXianSaleGoodsItem.RoleID, liXianSaleGoodsItem.RoleName, liXianSaleGoodsItem.RoleLevel, goodsData.GoodsID, -goodsData.GCount, Math.Max(0, saleMoney - num5), Math.Max(0, saleYinPiao - num5), Math.Max(0, saleYuanBao - num5), num9);
						GameManager.ClientMgr.NotifySpriteMarketBuy2(this.tcpMgr.MySocketListener, this.pool, client, liXianSaleGoodsItem.RoleID, 0, 0, num2, num3, liXianSaleGoodsItem.ZoneID, liXianSaleGoodsItem.RoleName, (int)this.CmdID);
						string warhprops = Convert.ToBase64String(DataHelper.ObjectToBytes<List<int>>(goodsData.WashProps));
						Global.AddMarketBuyLog(liXianSaleGoodsItem.RoleID, client.ClientData.RoleID, client.ClientData.RoleName, goodsData.GoodsID, goodsData.GCount, goodsData.Forge_level, saleYuanBao, client.ClientData.UserMoney, saleMoney, saleYinPiao, num5, goodsData.ExcellenceInfo, warhprops);
						SingletonTemplate<TradeBlackManager>.Instance().OnMarketBuy(client.ClientData.RoleID, liXianSaleGoodsItem.RoleID, saleGoods);
					}
					int gameConfigItemInt2 = GameManager.GameConfigMgr.GetGameConfigItemInt("tradelog_num_minamount", 5000);
					if (num8 >= gameConfigItemInt2)
					{
						GameManager.logDBCmdMgr.AddTradeNumberInfo(2, num8, roleid, client.ClientData.RoleID, client.ServerId);
					}
					int num10 = Global.IncreaseTradeCount(client, "SaleTradeDayID", "SaleTradeCount", 1);
					int gameConfigItemInt3 = GameManager.GameConfigMgr.GetGameConfigItemInt("tradelog_freq_sale", 10);
					if (num10 >= gameConfigItemInt3)
					{
						GameManager.logDBCmdMgr.AddTradeFreqInfo(2, num10, client.ClientData.RoleID, 0);
					}
					result = true;
				}
			}
			return result;
		}

		private void GiveBackSaleGoodsMoney(GameClient client, GameClient saller, GoodsData SalingGoodsData, int saleMoney, int saleYuanBao, int site)
		{
			SalingGoodsData.SaleMoney1 = saleMoney;
			SalingGoodsData.SaleYuanBao = saleYuanBao;
			SalingGoodsData.Site = site;
			int num = 0;
			int num2 = this.CalcRealMoneyAfterTax(saleMoney, MoneyTypes.YinLiang, out num);
			int num3 = this.CalcRealMoneyAfterTax(saleYuanBao, MoneyTypes.YuanBao, out num);
			if (SalingGoodsData.SaleMoney1 > 0)
			{
				if (!GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, SalingGoodsData.SaleMoney1, "新交易市场购买失败退回", false))
				{
					LogManager.WriteLog(3, string.Format("新交易市场购买失败退回金币失败:", Global.FormatRoleName4(client), SalingGoodsData.SaleMoney1), null, true);
				}
				if (!GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, saller, -num2, "新交易市场购买失败退回", false))
				{
					LogManager.WriteLog(3, string.Format("新交易市场购买失败退回金币失败:", Global.FormatRoleName4(client), -num2), null, true);
				}
			}
			if (SalingGoodsData.SaleYuanBao > 0)
			{
				if (!GameManager.ClientMgr.AddUserMoney(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, SalingGoodsData.SaleYuanBao, "新交易市场购买失败退回", ActivityTypes.None, ""))
				{
					LogManager.WriteLog(3, string.Format("新交易市场购买失败退回钻石失败:", Global.FormatRoleName4(client), SalingGoodsData.SaleYuanBao), null, true);
				}
				if (!GameManager.ClientMgr.AddUserMoney(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, saller, -num3, "新交易市场购买失败退回", ActivityTypes.None, ""))
				{
					LogManager.WriteLog(3, string.Format("新交易市场购买失败退回钻石失败:", Global.FormatRoleName4(client), -num3), null, true);
				}
			}
		}

		private void GiveBackSaleGoodsMoneyOffline(GameClient client, string userID, int sallerRoleID, string sallerName, GoodsData SalingGoodsData, int saleMoney, int saleYuanBao, int site)
		{
			SalingGoodsData.SaleMoney1 = saleMoney;
			SalingGoodsData.SaleYuanBao = saleYuanBao;
			SalingGoodsData.Site = site;
			int num = 0;
			int num2 = this.CalcRealMoneyAfterTax(saleMoney, MoneyTypes.YinLiang, out num);
			int num3 = this.CalcRealMoneyAfterTax(saleYuanBao, MoneyTypes.YuanBao, out num);
			if (SalingGoodsData.SaleMoney1 > 0)
			{
				if (!GameManager.ClientMgr.AddUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, SalingGoodsData.SaleMoney1, "新交易市场购买失败退回", false))
				{
					LogManager.WriteLog(3, string.Format("新交易市场购买失败退回金币失败:", Global.FormatRoleName4(client), SalingGoodsData.SaleMoney1), null, true);
				}
				if (!GameManager.ClientMgr.AddOfflineUserYinLiang(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, userID, sallerRoleID, sallerName, -num2, "新交易市场购买失败退回", client.ClientData.ZoneID))
				{
					LogManager.WriteLog(3, string.Format("新交易市场购买失败退回金币失败:", Global.FormatRoleName4(client), -num2), null, true);
				}
			}
			if (SalingGoodsData.SaleYuanBao > 0)
			{
				if (!GameManager.ClientMgr.AddUserMoney(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, client, SalingGoodsData.SaleYuanBao, "新交易市场购买失败退回", ActivityTypes.None, ""))
				{
					LogManager.WriteLog(3, string.Format("新交易市场购买失败退回钻石失败:", Global.FormatRoleName4(client), SalingGoodsData.SaleYuanBao), null, true);
				}
				if (!GameManager.ClientMgr.AddUserMoneyOffLine(this.tcpMgr.MySocketListener, this.tcpClientPool, this.pool, sallerRoleID, -num3, "新交易市场购买失败退回", client.ClientData.ZoneID, client.strUserID))
				{
					LogManager.WriteLog(3, string.Format("新交易市场购买失败退回钻石失败:", Global.FormatRoleName4(client), -num3), null, true);
				}
			}
		}

		private TCPGameServerCmds CmdID = TCPGameServerCmds.CMD_SPR_OPENMARKET2;
	}
}
