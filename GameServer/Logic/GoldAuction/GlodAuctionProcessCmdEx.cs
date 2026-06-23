using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic.GoldAuction
{
	internal class GlodAuctionProcessCmdEx : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static GlodAuctionProcessCmdEx getInstance()
		{
			return GlodAuctionProcessCmdEx.instance;
		}

		public bool initialize()
		{
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

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool startup()
		{
			lock (this.AuctionMsgMutex)
			{
				this.GoldAuctionMgr.InitData();
				TCPCmdDispatcher.getInstance().registerProcessorEx(2080, 8, 8, GlodAuctionProcessCmdEx.getInstance(), TCPCmdFlags.IsStringArrayParams);
				TCPCmdDispatcher.getInstance().registerProcessorEx(2081, 4, 4, GlodAuctionProcessCmdEx.getInstance(), TCPCmdFlags.IsStringArrayParams);
			}
			return true;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				lock (this.AuctionMsgMutex)
				{
					if (nID == 2080)
					{
						GoldAuctionS2C cmdData = new GoldAuctionS2C();
						this.GetGlodAuction(client, nID, bytes, cmdParams, ref cmdData);
						client.sendCmd<GoldAuctionS2C>(nID, cmdData, false);
					}
					else if (nID == 2081)
					{
						int num = 0;
						this.SetGlodAuction(client, nID, bytes, cmdParams, ref num);
						client.sendCmd(nID, num.ToString(), false);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return true;
		}

		private void GetGlodAuction(GameClient client, int nID, byte[] bytes, string[] cmdParams, ref GoldAuctionS2C msgData)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[1]);
				int ordeType = Convert.ToInt32(cmdParams[2]);
				int sortNum = Convert.ToInt32(cmdParams[3]);
				int startNum = Convert.ToInt32(cmdParams[4]);
				int maxNum = Convert.ToInt32(cmdParams[5]);
				string seach = cmdParams[6];
				int color = Convert.ToInt32(cmdParams[7]);
				if (!GoldAuctionManager.IsOpenAuction((AuctionOrderEnum)num))
				{
					msgData.Info = 2;
				}
				else if (num == 1 && client.ClientData.Faction < 1)
				{
					msgData.Info = 1;
				}
				else
				{
					this.GoldAuctionMgr.GetGoldAuctionS2C(num, ordeType, sortNum, startNum, maxNum, seach, color, ref msgData);
					msgData.Info = 0;
				}
			}
			catch (Exception ex)
			{
				msgData.Info = 100;
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		private void SetGlodAuction(GameClient client, int nID, byte[] bytes, string[] cmdParams, ref int Info)
		{
			GoldAuctionItem goldAuctionItem = null;
			Info = 0;
			try
			{
				int num = Convert.ToInt32(cmdParams[1]);
				string[] array = cmdParams[2].Split(new char[]
				{
					'|'
				});
				int num2 = Convert.ToInt32(cmdParams[3]);
				if (array == null || array.Length != 2)
				{
					Info = 3;
				}
				else
				{
					string productionTime = array[0].Replace(',', ':');
					int num3 = Convert.ToInt32(array[1]);
					if (num == 1 && client.ClientData.Faction < 1)
					{
						Info = 1;
					}
					AuctionConfig auctionConfig = GoldAuctionConfigModel.GetAuctionConfig(num3);
					if (null == auctionConfig)
					{
						LogManager.WriteLog(2, string.Format("[ljl]SetGlodAuction ({0}) null == config", num3), null, true);
						Info = 4;
					}
					else
					{
						goldAuctionItem = this.GoldAuctionMgr.GetGoldAuctionItem(num, productionTime, num3, true);
						if (goldAuctionItem == null || goldAuctionItem.AuctionType != num)
						{
							Info = 9;
						}
						else if (client.ClientData.RoleID == goldAuctionItem.BuyerData.m_RoleID)
						{
							Info = 11;
						}
						else if (goldAuctionItem.BuyerData.m_RoleID == 0 && num2 < auctionConfig.OriginPrice)
						{
							Info = 5;
						}
						else if (num2 < auctionConfig.UnitPrice)
						{
							Info = 6;
						}
						else if ((long)num2 <= goldAuctionItem.BuyerData.Value)
						{
							Info = 10;
						}
						else if (!GameManager.ClientMgr.SubUserMoney(client, num2, "金团拍卖购买", false, false, false, false, DaiBiSySType.None))
						{
							Info = 7;
						}
						if (Info != 0)
						{
							this.GoldAuctionMgr.UnLock(goldAuctionItem.ProductionTime, goldAuctionItem.AuctionSource);
						}
						else
						{
							AuctionRoleData buyerData = new AuctionRoleData();
							CopyData.Copy<AuctionRoleData>(goldAuctionItem.BuyerData, ref buyerData);
							bool flag = false;
							int roleID = client.ClientData.RoleID;
							goldAuctionItem.BuyerData.Value = (long)num2;
							goldAuctionItem.BuyerData.m_RoleID = client.ClientData.RoleID;
							goldAuctionItem.BuyerData.m_RoleName = client.ClientData.RoleName;
							goldAuctionItem.BuyerData.ZoneID = client.ClientData.ZoneID;
							goldAuctionItem.BuyerData.strUserID = client.strUserID;
							goldAuctionItem.BuyerData.ServerId = client.ServerId;
							if (num2 >= auctionConfig.MaxPrice && auctionConfig.MaxPrice > 0)
							{
								if (!this.GoldAuctionMgr.DisposeAward(goldAuctionItem))
								{
									Info = 8;
									LogManager.WriteLog(2, string.Format("[ljl]一口价 RoleId={0}, Price={1},itemKey={2} fail", roleID, num2, cmdParams[1]), null, true);
								}
								else
								{
									this.GoldAuctionMgr.DelGoldAuction(goldAuctionItem, "金团拍卖购买su");
									flag = true;
								}
							}
							else if (!this.GoldAuctionMgr.UpdatePrice(goldAuctionItem))
							{
								Info = 8;
								LogManager.WriteLog(2, string.Format("[ljl]更新价格 RoleId={0}, Price={1},itemKey={2} fail", roleID, num2, cmdParams[1]), null, true);
							}
							else
							{
								flag = true;
							}
							if (flag)
							{
								this.GoldAuctionMgr.ReturnOldAuctionMoney(buyerData, goldAuctionItem.StrGoods);
							}
							else
							{
								this.GoldAuctionMgr.UnLock(goldAuctionItem.ProductionTime, goldAuctionItem.AuctionSource);
								GameManager.logDBCmdMgr.AddMessageLog(-1, "钻石", "金团购买失败扣除钻石", client.ClientData.RoleName, client.ClientData.RoleName, "减少", num2, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, "");
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (null != goldAuctionItem)
				{
					this.GoldAuctionMgr.UnLock(goldAuctionItem.ProductionTime, goldAuctionItem.AuctionSource);
				}
				Info = 100;
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		public void KillBossAddAuction(int KillBossRole, long BossLife, List<AuctionRoleData> PointInfoList, AuctionEnum AuctionSource)
		{
			try
			{
				lock (this.AuctionMsgMutex)
				{
					AuctionAwardConfig auctionAwardConfig = GoldAuctionConfigModel.RandAuctionAwardConfig();
					if (null == auctionAwardConfig)
					{
						LogManager.WriteLog(2, "[ljl]KillBossAddAuction RandAuctionAwardConfig null == config", null, true);
					}
					else
					{
						AuctionConfig auctionConfig = GoldAuctionConfigModel.GetAuctionConfig((int)AuctionSource);
						if (null == auctionConfig)
						{
							LogManager.WriteLog(2, string.Format("[ljl]GoldAuctionConfigModel.GetAuctionConfig({0}) null == config", AuctionSource), null, true);
						}
						else
						{
							GoldAuctionItem goldAuctionItem = new GoldAuctionItem();
							goldAuctionItem.AuctionSource = (int)AuctionSource;
							goldAuctionItem.KillBossRoleID = KillBossRole;
							goldAuctionItem.UpDBWay = 2;
							goldAuctionItem.ProductionTime = TimeUtil.NowDataTimeString("yyyy-MM-dd HH:mm:ss.fff");
							goldAuctionItem.BuyerData = new AuctionRoleData();
							goldAuctionItem.BuyerData.m_RoleID = 0;
							goldAuctionItem.BuyerData.Value = (long)auctionConfig.OriginPrice;
							int num = Global.GetRandomNumber(auctionAwardConfig.StartValues, auctionAwardConfig.EndValues) - 1;
							num = Math.Min(num, auctionAwardConfig.strGoodsList.Count - 1);
							goldAuctionItem.StrGoods = auctionAwardConfig.strGoodsList[num];
							if (null == GlobalNew.ParseGoodsData(auctionAwardConfig.strGoodsList[num]))
							{
								LogManager.WriteLog(2, string.Format("[ljl]null == item.Goods index={0}  ,  GoodsList.Count = {1}", num, auctionAwardConfig.strGoodsList.Count), null, true);
							}
							else
							{
								goldAuctionItem.BossLife = 0L;
								foreach (AuctionRoleData auctionRoleData in PointInfoList)
								{
									goldAuctionItem.BossLife += auctionRoleData.Value;
									if (auctionRoleData.Value >= GameManager.systemParamsList.GetParamValueIntByName("AngelTempleAuction", -1))
									{
										AuctionRoleData item = new AuctionRoleData();
										CopyData.Copy<AuctionRoleData>(auctionRoleData, ref item);
										goldAuctionItem.RoleList.Add(item);
									}
								}
								if (goldAuctionItem.BossLife < 1L)
								{
									goldAuctionItem.BossLife = BossLife;
								}
								bool flag2 = false;
								for (int i = 0; i < auctionConfig.OrderList.Count; i++)
								{
									if (GoldAuctionManager.IsOpenAuction((AuctionOrderEnum)auctionConfig.OrderList[i]))
									{
										goldAuctionItem.AuctionType = auctionConfig.OrderList[i];
										goldAuctionItem.LifeTime = auctionConfig.TimeList[i];
										if (goldAuctionItem.AuctionType > 0 && goldAuctionItem.AuctionType < 3)
										{
											if (this.GoldAuctionMgr.SendUpdate2DB(goldAuctionItem))
											{
												this.GoldAuctionMgr.AddNewAuctionItem(goldAuctionItem);
											}
											else
											{
												LogManager.WriteLog(2, string.Format("[ljl]新拍卖物品存库失败 未加入 time={0}，AuctionSource={1}", goldAuctionItem.ProductionTime, AuctionSource), null, true);
											}
											flag2 = true;
											break;
										}
										LogManager.WriteLog(2, string.Format("[ljl]AuctionType ={1} err，AuctionSource={0}", AuctionSource, goldAuctionItem.AuctionType), null, true);
									}
								}
								if (!flag2)
								{
									int roleID = goldAuctionItem.GetMaxmDamageID();
									this.GoldAuctionMgr.SendItem(roleID, goldAuctionItem.RoleList.Find((AuctionRoleData x) => x.m_RoleID == roleID), goldAuctionItem, auctionConfig);
									LogManager.WriteLog(0, string.Format("[ljl]新拍卖物品未加入 直接邮件发送给玩家第一名 time={0}，AuctionSource={1}", goldAuctionItem.ProductionTime, AuctionSource), null, true);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		private static GlodAuctionProcessCmdEx instance = new GlodAuctionProcessCmdEx();

		private object AuctionMsgMutex = new object();

		private GoldAuctionManager GoldAuctionMgr = new GoldAuctionManager();
	}
}
