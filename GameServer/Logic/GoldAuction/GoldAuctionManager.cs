using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.GoldAuction
{
	public class GoldAuctionManager
	{
		public void InitData()
		{
			try
			{
				lock (this.AuctionMutex)
				{
					this.AuctionItemList = new List<GoldAuctionItem>();
					this.S2CCache = new Dictionary<string, AuctionS2CCache>();
					this.InitFromDB();
					ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("GoldAuctionManager.TimerProc", new EventHandler(this.TimerProc)), 0, 1000);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		private void InitFromDB()
		{
			try
			{
				for (int i = 1; i < 3; i++)
				{
					LogManager.WriteLog(0, string.Format("[ljl]InitFromDB AuctionID={0}", i), null, true);
					GetAuctionDBData getAuctionDBData = Global.sendToDB<GetAuctionDBData, string>(2080, i.ToString(), 0);
					if (getAuctionDBData == null || !getAuctionDBData.Flag)
					{
						LogManager.WriteLog(2, string.Format("[ljl]GoldAuctionManager.InitFromDB DBData={0}, type={1}", null == getAuctionDBData, i), null, true);
					}
					else
					{
						foreach (GoldAuctionDBItem goldAuctionDBItem in getAuctionDBData.ItemList)
						{
							GoldAuctionItem goldAuctionItem = new GoldAuctionItem();
							CopyData.CopyAuctionDB2Item(goldAuctionDBItem, out goldAuctionItem);
							AuctionConfig auctionConfig = GoldAuctionConfigModel.GetAuctionConfig(goldAuctionDBItem.AuctionSource);
							if (null == auctionConfig)
							{
								LogManager.WriteLog(2, string.Format("[ljl]GetAuctionConfig null == config AuctionSource ={0}", ((AuctionEnum)goldAuctionDBItem.AuctionSource).ToString()), null, true);
							}
							else
							{
								goldAuctionItem.LifeTime = auctionConfig.GetTimeByAuction(i);
								if (goldAuctionItem.LifeTime == -1)
								{
									LogManager.WriteLog(2, string.Format("[ljl]GetTimeByAuction =-1 AuctionOrderEnum={0}", i), null, true);
								}
								else
								{
									this.AddNewAuctionItem(goldAuctionItem);
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

		public bool SendUpdate2DB(GoldAuctionItem AuctionItem)
		{
			try
			{
				lock (this.AuctionMutex)
				{
					if (AuctionItem == null)
					{
						return false;
					}
					GoldAuctionDBItem goldAuctionDBItem;
					CopyData.CopyAuctionItem2DB(AuctionItem, out goldAuctionDBItem);
					string value = Global.Send2DB<GoldAuctionDBItem>(2081, goldAuctionDBItem, 0);
					if ("True".Equals(value))
					{
						LogManager.WriteLog(0, string.Format("[ljl]AuctionItem SendUpdate2DB true up type = {0},ProductionTime={1}, AuctionSource={2}", (DBAuctionUpEnum)goldAuctionDBItem.UpDBWay, goldAuctionDBItem.ProductionTime, goldAuctionDBItem.AuctionSource), null, true);
						return true;
					}
					LogManager.WriteLog(2, string.Format("[ljl]AuctionItem SendUpdate2DB false up type = {0},ProductionTime={1}, AuctionSource={2}", (DBAuctionUpEnum)goldAuctionDBItem.UpDBWay, goldAuctionDBItem.ProductionTime, goldAuctionDBItem.AuctionSource), null, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		public void UnLock(string ProductionTime, int AuctionSource)
		{
			try
			{
				lock (this.AuctionMutex)
				{
					GoldAuctionItem goldAuctionItem = this.getGoldAuctionItem(ProductionTime, AuctionSource);
					if (null != goldAuctionItem)
					{
						goldAuctionItem.Lock = false;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		private GoldAuctionItem getGoldAuctionItem(string ProductionTime, int AuctionSource)
		{
			GoldAuctionItem result = null;
			try
			{
				result = this.AuctionItemList.Find((GoldAuctionItem x) => x.ProductionTime == ProductionTime && x.AuctionSource == AuctionSource);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return result;
		}

		public void AddNewAuctionItem(GoldAuctionItem AuctionItem)
		{
			try
			{
				lock (this.AuctionMutex)
				{
					GoldAuctionItem goldAuctionItem = this.getGoldAuctionItem(AuctionItem.ProductionTime, AuctionItem.AuctionSource);
					if (null != goldAuctionItem)
					{
						LogManager.WriteLog(2, string.Format("[ljl]AddNewGoldAuction same time={0}, AuctionSource={1}", AuctionItem.ProductionTime, (AuctionEnum)AuctionItem.AuctionSource), null, true);
					}
					else
					{
						this.AuctionItemList.Add(AuctionItem);
						this.S2CCache.Clear();
						LogManager.WriteLog(0, string.Format("[ljl]AddNewGoldAuction su time={0}, AuctionSource={1}", AuctionItem.ProductionTime, (AuctionEnum)AuctionItem.AuctionSource), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		public void DelGoldAuction(GoldAuctionItem Item, string info)
		{
			try
			{
				lock (this.AuctionMutex)
				{
					if (this.AuctionItemList.RemoveAll((GoldAuctionItem x) => x.ProductionTime == Item.ProductionTime && x.AuctionSource == Item.AuctionSource) > 0)
					{
						LogManager.WriteLog(0, string.Format("[ljl]{0} DelGoldAuction time={1}, AuctionSource={2}", info, Item.ProductionTime, (AuctionEnum)Item.AuctionSource), null, true);
						this.S2CCache.Clear();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		public bool UpdatePrice(GoldAuctionItem _item)
		{
			bool flag = false;
			try
			{
				lock (this.AuctionMutex)
				{
					GoldAuctionItem goldAuctionItem = this.getGoldAuctionItem(_item.ProductionTime, _item.AuctionSource);
					if (goldAuctionItem == null || goldAuctionItem.AuctionType != _item.AuctionType)
					{
						return flag;
					}
					CopyData.Copy<AuctionRoleData>(_item.BuyerData, ref goldAuctionItem.BuyerData);
					goldAuctionItem.Lock = false;
					goldAuctionItem.UpDBWay = 3;
					goldAuctionItem.OldAuctionType = goldAuctionItem.AuctionType;
					flag = this.SendUpdate2DB(goldAuctionItem);
					if (flag)
					{
						this.S2CCache.Clear();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return flag;
		}

		public GoldAuctionItem GetGoldAuctionItem(int type, string ProductionTime, int AuctionSource, bool isLock)
		{
			GoldAuctionItem result = null;
			try
			{
				lock (this.AuctionMutex)
				{
					GoldAuctionItem goldAuctionItem = this.getGoldAuctionItem(ProductionTime, AuctionSource);
					if (goldAuctionItem == null || goldAuctionItem.AuctionType != type)
					{
						return null;
					}
					goldAuctionItem.Lock = isLock;
					CopyData.CopyGoldAuctionItem(goldAuctionItem, ref result);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return result;
		}

		public void GetGoldAuctionS2C(int Auctiontype, int ordeType, int sortNum, int startNum, int maxNum, string Seach, int Color, ref GoldAuctionS2C clientData)
		{
			try
			{
				lock (this.AuctionMutex)
				{
					string key = string.Format("{0}|{1}|{2}|{3}|{4}", new object[]
					{
						Auctiontype,
						ordeType,
						sortNum,
						Seach,
						Color
					});
					int num = 0;
					List<AuctionItemS2C> list = new List<AuctionItemS2C>();
					AuctionS2CCache auctionS2CCache;
					if (!this.S2CCache.TryGetValue(key, out auctionS2CCache))
					{
						auctionS2CCache = new AuctionS2CCache();
						foreach (GoldAuctionItem goldAuctionItem in this.AuctionItemList)
						{
							if (goldAuctionItem.AuctionType == Auctiontype)
							{
								num++;
								AuctionItemS2C item;
								if (CopyData.Copy2AuctionItemS2C(goldAuctionItem, out item, Seach, Color))
								{
									list.Add(item);
								}
							}
						}
						list.Sort(new GlodAuctionIComparer(ordeType, sortNum > 0));
						auctionS2CCache.MaxNum = num;
						auctionS2CCache.dList.AddRange(list);
						this.S2CCache.Add(key, auctionS2CCache);
					}
					else
					{
						num = auctionS2CCache.MaxNum;
						foreach (AuctionItemS2C auctionItemS2C in auctionS2CCache.dList)
						{
							string[] array = auctionItemS2C.AuctionItemKey.Split(new char[]
							{
								'|'
							});
							GoldAuctionItem goldAuctionItem2 = this.getGoldAuctionItem(array[0].Replace(',', ':'), Convert.ToInt32(array[1]));
							AuctionItemS2C item;
							if (CopyData.Copy2AuctionItemS2C(auctionItemS2C, out item, goldAuctionItem2))
							{
								list.Add(item);
							}
						}
					}
					int count = maxNum;
					int num2 = (startNum - 1) * maxNum;
					clientData.CurrentPage = startNum;
					if (num2 < 1 || num2 >= list.Count)
					{
						clientData.CurrentPage = 1;
						count = Math.Min(maxNum, list.Count);
					}
					else if (num2 + maxNum > list.Count)
					{
						count = list.Count - num2;
					}
					clientData.TotalCount = num;
					clientData.ItemList.AddRange(list.GetRange(num2, count));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		public static bool IsOpenAuction(AuctionOrderEnum type)
		{
			try
			{
				if (type == AuctionOrderEnum.AuctionZhanMeng)
				{
					return GameManager.systemParamsList.GetParamValueIntByName("AuctionZhanMengOpen", -1) > 0L;
				}
				if (type == AuctionOrderEnum.AuctionWorld)
				{
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return true;
		}

		private void TimerProc(object sender, EventArgs e)
		{
			try
			{
				lock (this.AuctionMutex)
				{
					foreach (GoldAuctionItem outTimeItem in this.AuctionItemList)
					{
						this.SetOutTimeItem(outTimeItem);
					}
					if (this.AuctionItemList.RemoveAll((GoldAuctionItem x) => DateTime.Parse(x.AuctionTime).AddHours((double)x.LifeTime) <= TimeUtil.NowDateTime() && !x.Lock) > 0)
					{
						this.S2CCache.Clear();
					}
					if (this.AuctionItemList.RemoveAll((GoldAuctionItem x) => x.UpDBWay == 1) > 0)
					{
						this.S2CCache.Clear();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		private bool SetOutTimeItem(GoldAuctionItem AuctionItem)
		{
			try
			{
				if (AuctionItem.Lock || DateTime.Parse(AuctionItem.AuctionTime).AddHours((double)AuctionItem.LifeTime) > TimeUtil.NowDateTime())
				{
					return true;
				}
				if (AuctionItem.BuyerData.m_RoleID != 0)
				{
					return this.DisposeAward(AuctionItem);
				}
				AuctionConfig auctionConfig = GoldAuctionConfigModel.GetAuctionConfig(AuctionItem.AuctionSource);
				if (null == auctionConfig)
				{
					LogManager.WriteLog(2, string.Format("[ljl]null == config AuctionSource = {0}", AuctionItem.AuctionSource), null, true);
					return false;
				}
				int nextAuction = auctionConfig.GetNextAuction(AuctionItem.AuctionType);
				if (nextAuction <= -1)
				{
					return this.DisposeAward(AuctionItem);
				}
				AuctionItem.OldAuctionType = AuctionItem.AuctionType;
				AuctionItem.AuctionType = nextAuction;
				AuctionItem.AuctionTime = TimeUtil.NowDataTimeString("yyyy-MM-dd HH:mm:ss");
				AuctionItem.UpDBWay = 3;
				AuctionItem.LifeTime = auctionConfig.GetTimeByAuction(nextAuction);
				this.S2CCache.Clear();
				if (this.SendUpdate2DB(AuctionItem))
				{
					LogManager.WriteLog(0, string.Format("[ljl]超时换拍卖行ProductionTime = {0}, AuctionSource={1},AuctionType={2}", AuctionItem.ProductionTime, AuctionItem.AuctionSource, AuctionItem.AuctionType), null, true);
					return true;
				}
				LogManager.WriteLog(2, string.Format("[ljl]超时换拍卖行 db失败 ProductionTime = {0}, AuctionSource={1}", AuctionItem.ProductionTime, AuctionItem.AuctionSource), null, true);
				if (AuctionItem.LifeTime == -1)
				{
					LogManager.WriteLog(2, string.Format("[ljl]GetTimeByAuction =-1 AuctionOrderEnum={0}", nextAuction), null, true);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		public bool DisposeAward(GoldAuctionItem Item)
		{
			try
			{
				bool flag = false;
				try
				{
					object auctionMutex;
					Monitor.Enter(auctionMutex = this.AuctionMutex, ref flag);
					if (null == Item)
					{
						return false;
					}
					AuctionConfig auctionConfig = GoldAuctionConfigModel.GetAuctionConfig(Item.AuctionSource);
					if (null == auctionConfig)
					{
						LogManager.WriteLog(2, string.Format("[ljl]GoldAuctionConfigModel.GetAuctionConfig({0}) null == config", (AuctionEnum)Item.AuctionSource), null, true);
						return false;
					}
					long num = 0L;
					int num2 = (int)GameManager.systemParamsList.GetParamValueIntByName("AngelTempleAuctionMin", 0);
					if (num2 < 0)
					{
						LogManager.WriteLog(2, "[ljl]AngelTempleAuctionMin < 0", null, true);
						num2 = 1;
					}
					long num3 = 0L;
					if (0 == Item.BuyerData.m_RoleID)
					{
						num = 0L;
						num3 = Item.BuyerData.Value;
					}
					else if (Item.RoleList.Count < 1)
					{
						num = 0L;
					}
					else if ((long)(num2 * Item.RoleList.Count) < Item.BuyerData.Value)
					{
						num = (long)num2;
						num3 = (long)Item.RoleList.Count * num;
					}
					else if ((long)(num2 * Item.RoleList.Count) >= Item.BuyerData.Value)
					{
						num = Item.BuyerData.Value / (long)Item.RoleList.Count;
						num3 = Item.BuyerData.Value;
					}
					Item.UpDBWay = 1;
					Item.OldAuctionType = Item.AuctionType;
					this.S2CCache.Clear();
					if (!this.SendUpdate2DB(Item))
					{
						Item.UpDBWay = 3;
						return false;
					}
					Item.BuyerData.Value -= num3;
					int AwardRole = Item.BuyerData.m_RoleID;
					if (AwardRole > 0)
					{
						this.SendMoney(Item, num, auctionConfig);
						this.SendItem(AwardRole, Item.BuyerData, Item, auctionConfig);
					}
					else
					{
						AwardRole = Item.GetMaxmDamageID();
						this.SendItem(AwardRole, Item.RoleList.Find((AuctionRoleData x) => x.m_RoleID == AwardRole), Item, auctionConfig);
					}
				}
				finally
				{
					if (flag)
					{
						object auctionMutex;
						Monitor.Exit(auctionMutex);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
				return false;
			}
			return true;
		}

		private void SendMoney(GoldAuctionItem AuctionItem, long averageNum, AuctionConfig AuctionCfg)
		{
			try
			{
				Dictionary<AuctionRoleData, long> dictionary = new Dictionary<AuctionRoleData, long>();
				if (averageNum >= 1L)
				{
					foreach (AuctionRoleData auctionRoleData in AuctionItem.RoleList)
					{
						long num = averageNum;
						if (AuctionItem.BuyerData.Value > 0L && AuctionItem.BossLife > 0L && auctionRoleData.Value <= AuctionItem.BossLife)
						{
							num += (long)((double)auctionRoleData.Value / (double)AuctionItem.BossLife * (double)AuctionItem.BuyerData.Value);
						}
						dictionary.Add(auctionRoleData, num);
					}
					foreach (KeyValuePair<AuctionRoleData, long> keyValuePair in dictionary)
					{
						AuctionRoleData key = keyValuePair.Key;
						int num2 = (int)keyValuePair.Value;
						try
						{
							if (num2 > 0)
							{
								if (!Global.UseMailGivePlayerAward3(key.m_RoleID, null, AuctionCfg.SuccessTitle, string.Format(AuctionCfg.SuccessIntro, this.GetItemName(AuctionItem.StrGoods), num2), num2, 0, 0))
								{
									GameManager.logDBCmdMgr.AddMessageLog(-1, "钻石", "邮件发放金团结算失败", key.m_RoleName, key.m_RoleName, "增加", num2, key.ZoneID, key.strUserID, -1, key.ServerId, "");
									LogManager.WriteLog(2, string.Format("[ljl]邮件发放金团结算失败 send email roleid={0}, money={1}", key.m_RoleID, num2), null, true);
								}
								else
								{
									GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", "邮件发放金团结算", "系统", key.m_RoleName, "增加", num2, key.ZoneID, key.strUserID, -1, key.ServerId, null);
								}
							}
						}
						catch (Exception ex)
						{
							LogManager.WriteLog(9, string.Format("[ljl]发放奖励 send email roleid={0}, money={1}, {2}", key.m_RoleID, num2, ex.ToString()), null, true);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		public void SendItem(int roleID, AuctionRoleData roleData, GoldAuctionItem AuctionItem, AuctionConfig AuctionCfg)
		{
			lock (this.AuctionMutex)
			{
				try
				{
					if (roleID <= 0)
					{
						LogManager.WriteLog(2, string.Format("[ljl]SendItem roleid=0,ProductionTime={0},AuctionSource={1}", AuctionItem.ProductionTime, AuctionItem.AuctionSource), null, true);
					}
					else
					{
						List<GoodsData> list = new List<GoodsData>();
						GoodsData goodsData = GlobalNew.ParseGoodsData(AuctionItem.StrGoods);
						if (null == goodsData)
						{
							LogManager.WriteLog(2, string.Format("[ljl]SendItem null == goods,ProductionTime={0},AuctionSource={1}", AuctionItem.ProductionTime, AuctionItem.AuctionSource), null, true);
						}
						else
						{
							list.Add(goodsData);
							string itemName = this.GetItemName(AuctionItem.StrGoods);
							if (null == roleData)
							{
								roleData = new AuctionRoleData();
								GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
								if (gameClient != null)
								{
									roleData.m_RoleID = roleID;
									roleData.strUserID = gameClient.strUserID;
									roleData.m_RoleName = gameClient.ClientData.RoleName;
									roleData.ZoneID = gameClient.ClientData.ZoneID;
									roleData.ServerId = gameClient.ServerId;
								}
							}
							string sSubject;
							string sContent;
							if (roleID == AuctionItem.BuyerData.m_RoleID)
							{
								sSubject = "购买成功";
								sContent = "在金团拍卖购买成功";
							}
							else
							{
								sSubject = AuctionCfg.FailTitle;
								sContent = string.Format(AuctionCfg.FailIntro, itemName);
							}
							if (!Global.UseMailGivePlayerAward3(roleID, list, sSubject, sContent, 0, 0, 0))
							{
								GameManager.logDBCmdMgr.AddMessageLog(-1, itemName, "邮件发放金团结算失败", roleData.m_RoleName, roleData.m_RoleName, "增加", goodsData.GCount, roleData.ZoneID, roleData.strUserID, -1, roleData.ServerId, AuctionItem.StrGoods);
								LogManager.WriteLog(2, string.Format("[ljl]SendItem 邮件发放金团结算失败 roleid={0}, item = {1}", roleID, AuctionItem.StrGoods), null, true);
							}
							else
							{
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, itemName, "邮件发放金团结算", "系统", roleData.m_RoleName, "增加", goodsData.GCount, roleData.ZoneID, roleData.strUserID, -1, roleData.ServerId, null);
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(9, string.Format("[ljl]SendItem send email roleid={0}, item = {1}", roleID, AuctionItem.StrGoods, ex.ToString()), null, true);
				}
			}
		}

		public void ReturnOldAuctionMoney(AuctionRoleData BuyerData, string StrGoods)
		{
			try
			{
				lock (this.AuctionMutex)
				{
					int num = (int)BuyerData.Value;
					if (!Global.UseMailGivePlayerAward3(BuyerData.m_RoleID, null, "竞拍失败", string.Format("您参与的活动奖励{0}，在竞价中被超过,返还您{1}钻石", this.GetItemName(StrGoods), num), num, 0, 0))
					{
						GameManager.logDBCmdMgr.AddMessageLog(-1, "钻石", "邮件发放金团结算失败", BuyerData.m_RoleName, BuyerData.m_RoleName, "增加", num, BuyerData.ZoneID, BuyerData.strUserID, -1, BuyerData.ServerId, "");
						LogManager.WriteLog(2, string.Format("[ljl]邮件发放,有新的竞价者,把钱返回给之前竞价者,失败 send email roleid={0}, money={1}", BuyerData.m_RoleID, num), null, true);
					}
					else
					{
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", "邮件发放金团结算", "系统", BuyerData.m_RoleName, "增加", num, BuyerData.ZoneID, BuyerData.strUserID, -1, BuyerData.ServerId, null);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
		}

		private string GetItemName(string strGoods)
		{
			try
			{
				return Global.GetGoodsName(GlobalNew.ParseGoodsData(strGoods));
			}
			catch
			{
			}
			return "道具";
		}

		private const int PageSetNum = 10;

		private const string msgFlag = "True";

		private object AuctionMutex = new object();

		private List<GoldAuctionItem> AuctionItemList;

		private Dictionary<string, AuctionS2CCache> S2CCache;
	}
}
