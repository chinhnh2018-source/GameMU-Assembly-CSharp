using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic.CheatGuard
{
	public class TradeBlackManager : SingletonTemplate<TradeBlackManager>
	{
		private TradeBlackManager()
		{
		}

		public bool LoadConfig()
		{
			bool result = true;
			try
			{
				if (!File.Exists(Global.GameResPath("Config\\Blacklist.xml")))
				{
					return false;
				}
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				XElement xelement = XElement.Load(Global.GameResPath("Config\\Blacklist.xml"));
				foreach (XElement xml in xelement.Elements())
				{
					int key = (int)Global.GetSafeAttributeLong(xml, "GoodsID");
					int value = (int)Global.GetSafeAttributeLong(xml, "Price");
					dictionary.Add(key, value);
				}
				this.GoodsPriceDict = dictionary;
			}
			catch (Exception ex)
			{
				result = false;
				LogManager.WriteLog(2, "load Config\\Blacklist.xml exception!", ex, true);
			}
			try
			{
				List<TradeConfigItem> list = new List<TradeConfigItem>();
				XElement xelement = XElement.Load(Global.GameResPath("Config\\TradeConfig.xml"));
				foreach (XElement xml in xelement.Elements())
				{
					TradeConfigItem tradeConfigItem = new TradeConfigItem();
					tradeConfigItem.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
					tradeConfigItem.MinVip = (int)Global.GetSafeAttributeLong(xml, "MinVip");
					tradeConfigItem.MaxVip = (int)Global.GetSafeAttributeLong(xml, "MaxVip");
					tradeConfigItem.MaxPrice = (int)Global.GetSafeAttributeLong(xml, "MaxPrice");
					tradeConfigItem.MaxTimes = (int)Global.GetSafeAttributeLong(xml, "MaxNum");
					int zhuanSheng = (int)Global.GetSafeAttributeLong(xml, "MinZhuanSheng");
					int level = (int)Global.GetSafeAttributeLong(xml, "MinLevel");
					tradeConfigItem.UnionMinLevel = Global.GetUnionLevel(zhuanSheng, level, false);
					zhuanSheng = (int)Global.GetSafeAttributeLong(xml, "MaxZhuanSheng");
					level = (int)Global.GetSafeAttributeLong(xml, "MaxLevel");
					tradeConfigItem.UnionMaxLevel = Global.GetUnionLevel(zhuanSheng, level, false);
					list.Add(tradeConfigItem);
				}
				this.TradeCfgItems = list;
			}
			catch (Exception ex)
			{
				result = false;
				LogManager.WriteLog(2, "load Config\\TradeConfig.xml exception!", ex, true);
			}
			string str = string.Empty;
			PlatformTypes platformType = GameCoreInterface.getinstance().GetPlatformType();
			if (platformType == 3)
			{
				str = "Android";
			}
			else if (platformType == 2)
			{
				str = "YueYu";
			}
			else if (platformType == 1)
			{
				str = "APP";
			}
			else if (platformType == 4)
			{
				str = "YYB";
			}
			this.BanTradeSec = (int)GameManager.systemParamsList.GetParamValueIntByName("NoTrade_" + str, -1);
			this.BanTradeLog = (int)GameManager.systemParamsList.GetParamValueIntByName("TradeLog_" + str, -1);
			this.BanTradeLogin = (int)GameManager.systemParamsList.GetParamValueIntByName("TradeKill_" + str, -1);
			return result;
		}

		public bool IsBanTrade(int roleId)
		{
			bool result = false;
			TradeBlackObject tradeBlackObject = this.LoadTradeBlackObject(roleId, true);
			if (tradeBlackObject != null)
			{
				result = (tradeBlackObject.BanTradeToTicks > 0L && tradeBlackObject.BanTradeToTicks > TimeUtil.NowDateTime().Ticks);
			}
			return result;
		}

		public void UpdateObjectExtData(GameClient client)
		{
			if (client != null)
			{
				TradeBlackObject tradeBlackObject = this.LoadTradeBlackObject(client.ClientData.RoleID, false);
				if (tradeBlackObject != null)
				{
					lock (tradeBlackObject)
					{
						tradeBlackObject.ChangeLife = client.ClientData.ChangeLifeCount;
						tradeBlackObject.Level = client.ClientData.Level;
						tradeBlackObject.VipLevel = client.ClientData.VipLevel;
						tradeBlackObject.ZoneId = client.ClientData.ZoneID;
						tradeBlackObject.RoleName = client.ClientData.RoleName;
					}
				}
			}
		}

		public void SetBanTradeToTicks(int roleid, long toTicks)
		{
			toTicks = Math.Max(0L, toTicks);
			GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", roleid, 3, toTicks), null, 0);
			TradeBlackObject tradeBlackObject = this.LoadTradeBlackObject(roleid, true);
			if (tradeBlackObject != null)
			{
				lock (tradeBlackObject)
				{
					tradeBlackObject.BanTradeToTicks = toTicks;
				}
			}
			long num = 0L;
			if (toTicks > TimeUtil.NowDateTime().Ticks)
			{
				num = (long)(new DateTime(toTicks) - TimeUtil.NowDateTime()).TotalSeconds;
				num = Math.Max(0L, num);
			}
			if (num > 0L)
			{
				LogManager.WriteLog(2, string.Format("roleid={0} 被封禁交易，秒数={1}", roleid, num), null, true);
			}
			GameClient gameClient = GameManager.ClientMgr.FindClient(roleid);
			if (gameClient != null)
			{
				gameClient.ClientData.BanTradeToTicks = toTicks;
				if (num > 0L)
				{
					string msgText = string.Format(GLang.GetLang(35, new object[0]), num);
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, msgText, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				}
			}
		}

		private void CheckBanTrade(int roleId)
		{
			TradeBlackObject obj = this.LoadTradeBlackObject(roleId, true);
			if (obj != null)
			{
				int num = -1;
				lock (obj)
				{
					if (obj.BanTradeToTicks <= 0L)
					{
						List<TradeConfigItem> tradeCfgItems = this.TradeCfgItems;
						TradeConfigItem tradeConfigItem;
						if (tradeCfgItems == null)
						{
							tradeConfigItem = null;
						}
						else
						{
							tradeConfigItem = tradeCfgItems.Find((TradeConfigItem _i) => _i.MinVip <= obj.VipLevel && _i.MaxVip >= obj.VipLevel && _i.UnionMinLevel <= Global.GetUnionLevel(obj.ChangeLife, obj.Level, false) && _i.UnionMaxLevel >= Global.GetUnionLevel(obj.ChangeLife, obj.Level, false));
						}
						TradeConfigItem tradeConfigItem2 = tradeConfigItem;
						if (tradeConfigItem2 != null)
						{
							long num2 = 0L;
							long num3 = 0L;
							long num4 = 0L;
							foreach (TradeBlackHourItem tradeBlackHourItem in obj.HourItems)
							{
								if (tradeBlackHourItem != null)
								{
									num2 += tradeBlackHourItem.MarketInPrice + tradeBlackHourItem.TradeInPrice;
									num3 += tradeBlackHourItem.MarketOutPrice + tradeBlackHourItem.TradeOutPrice;
									num4 += (long)(tradeBlackHourItem.MarketTimes + tradeBlackHourItem.TradeTimes);
								}
							}
							if (num2 >= (long)tradeConfigItem2.MaxPrice || num3 >= (long)tradeConfigItem2.MaxPrice || num4 >= (long)tradeConfigItem2.MaxTimes)
							{
								int num5 = Math.Max(this.BanTradeSec, 0);
								if (num5 > 0)
								{
									long ticks = TimeUtil.NowDateTime().AddSeconds((double)num5).Ticks;
									this.SetBanTradeToTicks(roleId, ticks);
								}
								if (this.BanTradeLog == 1)
								{
									LogManager.WriteLog(5, string.Format("tradeblack player={0} inprice={1} outprice={2} times={3} bansec={4}", new object[]
									{
										roleId,
										num2,
										num3,
										num4,
										num5
									}), null, true);
								}
								num = Math.Max(this.BanTradeLogin, 0) / 60;
								if (num > 0)
								{
									BanManager.BanRoleName(Global.FormatRoleName3(obj.ZoneId, obj.RoleName), num, 3);
								}
							}
						}
					}
				}
				if (num > 0)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(roleId);
					if (gameClient != null)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, StringUtil.substitute(GLang.GetLang(36, new object[0]), new object[]
						{
							num
						}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
						Global.ForceCloseClient(gameClient, "交易封禁", true);
					}
				}
			}
		}

		private void CheckUnBanTrade(int roleId)
		{
			TradeBlackObject tradeBlackObject = this.LoadTradeBlackObject(roleId, true);
			if (tradeBlackObject != null)
			{
				lock (tradeBlackObject)
				{
					if (tradeBlackObject.BanTradeToTicks > 0L && tradeBlackObject.BanTradeToTicks < TimeUtil.NowDateTime().Ticks)
					{
						this.SetBanTradeToTicks(roleId, 0L);
					}
				}
			}
		}

		public void Update()
		{
			if ((TimeUtil.NowDateTime() - this.lastCheckUnBanTime).TotalSeconds > 60.0)
			{
				this.lastCheckUnBanTime = TimeUtil.NowDateTime();
				List<int> list = null;
				lock (this.TradeBlackObjs)
				{
					list = this.TradeBlackObjs.Keys.ToList<int>();
				}
				if (list != null)
				{
					list.ForEach(delegate(int _r)
					{
						this.CheckUnBanTrade(_r);
					});
				}
			}
			if ((TimeUtil.NowDateTime() - this.lastFreeUnusedTime).TotalHours > 1.0)
			{
				this.lastFreeUnusedTime = TimeUtil.NowDateTime();
				List<int> list = null;
				lock (this.TradeBlackObjs)
				{
					list = this.TradeBlackObjs.Keys.ToList<int>();
				}
				List<int> list2;
				if (list == null)
				{
					list2 = null;
				}
				else
				{
					list2 = list.FindAll(delegate(int _r)
					{
						TradeBlackObject tradeBlackObject = this.LoadTradeBlackObject(_r, true);
						return tradeBlackObject != null && tradeBlackObject.BanTradeToTicks <= 0L && (TimeUtil.NowDateTime() - tradeBlackObject.LastFlushTime).TotalHours > 12.0;
					});
				}
				List<int> list3 = list2;
				if (list3 != null)
				{
					lock (this.TradeBlackObjs)
					{
						foreach (int key in list3)
						{
							this.TradeBlackObjs.Remove(key);
						}
					}
				}
			}
		}

		public void OnExchange(int roleid1, int roleid2, List<GoodsData> gdList1, List<GoodsData> gdList2, int zuanshi1, int zuanshi2)
		{
			long num = (long)((zuanshi1 > 0) ? zuanshi1 : 0);
			long num2 = (long)((zuanshi2 > 0) ? zuanshi2 : 0);
			Func<List<GoodsData>, Dictionary<int, int>, long> func = delegate(List<GoodsData> gdList, Dictionary<int, int> priceDict)
			{
				long totalPrice = 0L;
				if (gdList != null && priceDict != null)
				{
					gdList.ForEach(delegate(GoodsData _g)
					{
						totalPrice += (long)(priceDict.ContainsKey(_g.GoodsID) ? (priceDict[_g.GoodsID] * _g.GCount) : 0);
					});
				}
				return totalPrice;
			};
			num += func(gdList1, this.GoodsPriceDict);
			num2 += func(gdList2, this.GoodsPriceDict);
			DateTime date = TimeUtil.NowDateTime();
			TradeBlackObject tradeBlackObject = this.LoadTradeBlackObject(roleid1, true);
			if (tradeBlackObject != null)
			{
				lock (tradeBlackObject)
				{
					TradeBlackHourItem blackHourItem = this.GetBlackHourItem(tradeBlackObject, date);
					blackHourItem.TradeTimes++;
					blackHourItem.TradeOutPrice += num;
					blackHourItem.TradeInPrice += num2;
					if (!blackHourItem.TradeRoles.Contains(roleid2))
					{
						blackHourItem.TradeRoles.Add(roleid2);
						blackHourItem.TradeDistinctRoleCount++;
					}
					TradeBlackHourItem obj2 = blackHourItem.SimpleClone();
					this.SaveTradeBlackObject(obj2);
				}
			}
			TradeBlackObject tradeBlackObject2 = this.LoadTradeBlackObject(roleid2, true);
			if (tradeBlackObject2 != null)
			{
				lock (tradeBlackObject2)
				{
					TradeBlackHourItem blackHourItem = this.GetBlackHourItem(tradeBlackObject2, date);
					blackHourItem.TradeTimes++;
					blackHourItem.TradeInPrice += num;
					blackHourItem.TradeOutPrice += num2;
					if (!blackHourItem.TradeRoles.Contains(roleid1))
					{
						blackHourItem.TradeRoles.Add(roleid1);
						blackHourItem.TradeDistinctRoleCount++;
					}
					TradeBlackHourItem obj2 = blackHourItem.SimpleClone();
					this.SaveTradeBlackObject(obj2);
				}
			}
			this.CheckBanTrade(roleid1);
			this.CheckBanTrade(roleid2);
		}

		public void OnMarketBuy(int whoBuy, int whoSale, GoodsData saleGoods)
		{
			if (saleGoods != null)
			{
				int num = Math.Max(saleGoods.SaleYuanBao + saleGoods.SaleYinPiao, 0);
				Dictionary<int, int> goodsPriceDict = this.GoodsPriceDict;
				int num2 = (goodsPriceDict != null && goodsPriceDict.ContainsKey(saleGoods.GoodsID)) ? (goodsPriceDict[saleGoods.GoodsID] * saleGoods.GCount) : 0;
				DateTime date = TimeUtil.NowDateTime();
				TradeBlackObject tradeBlackObject = this.LoadTradeBlackObject(whoBuy, true);
				if (tradeBlackObject != null)
				{
					lock (tradeBlackObject)
					{
						TradeBlackHourItem blackHourItem = this.GetBlackHourItem(tradeBlackObject, date);
						blackHourItem.MarketTimes++;
						blackHourItem.MarketInPrice += (long)num2;
						blackHourItem.MarketOutPrice += (long)num;
						if (!blackHourItem.TradeRoles.Contains(whoSale))
						{
							blackHourItem.TradeRoles.Add(whoSale);
							blackHourItem.TradeDistinctRoleCount++;
						}
						TradeBlackHourItem obj2 = blackHourItem.SimpleClone();
						this.SaveTradeBlackObject(obj2);
					}
				}
				TradeBlackObject tradeBlackObject2 = this.LoadTradeBlackObject(whoSale, true);
				if (tradeBlackObject2 != null)
				{
					lock (tradeBlackObject2)
					{
						TradeBlackHourItem blackHourItem = this.GetBlackHourItem(tradeBlackObject2, date);
						blackHourItem.MarketTimes++;
						blackHourItem.MarketOutPrice += (long)num2;
						blackHourItem.MarketInPrice += (long)num;
						if (!blackHourItem.TradeRoles.Contains(whoBuy))
						{
							blackHourItem.TradeRoles.Add(whoBuy);
							blackHourItem.TradeDistinctRoleCount++;
						}
						TradeBlackHourItem obj2 = blackHourItem.SimpleClone();
						this.SaveTradeBlackObject(obj2);
					}
				}
				this.CheckBanTrade(whoBuy);
				this.CheckBanTrade(whoSale);
			}
		}

		private TradeBlackObject LoadTradeBlackObject(int roleid, bool loadDbIfNotExist = true)
		{
			DateTime lastFlushTime = TimeUtil.NowDateTime();
			TradeBlackObject tradeBlackObject = null;
			int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
			lock (this.TradeBlackObjs)
			{
				if (this.TradeBlackObjs.TryGetValue(roleid, out tradeBlackObject))
				{
					tradeBlackObject.LastFlushTime = lastFlushTime;
				}
			}
			if (tradeBlackObject == null && loadDbIfNotExist)
			{
				string cmd = string.Format("{0}:{1}:{2}", roleid, lastFlushTime.ToString("yyyy-MM-dd"), lastFlushTime.Hour);
				List<TradeBlackHourItem> list = Global.sendToDB<List<TradeBlackHourItem>, string>(14007, cmd, 0);
				tradeBlackObject = new TradeBlackObject();
				tradeBlackObject.RoleId = roleid;
				tradeBlackObject.LastFlushTime = lastFlushTime;
				tradeBlackObject.HourItems = new TradeBlackHourItem[24];
				GameClient gameClient = GameManager.ClientMgr.FindClient(roleid);
				if (gameClient != null)
				{
					tradeBlackObject.VipLevel = gameClient.ClientData.VipLevel;
					tradeBlackObject.ChangeLife = gameClient.ClientData.ChangeLifeCount;
					tradeBlackObject.Level = gameClient.ClientData.Level;
					tradeBlackObject.BanTradeToTicks = gameClient.ClientData.BanTradeToTicks;
					tradeBlackObject.ZoneId = gameClient.ClientData.ZoneID;
					tradeBlackObject.RoleName = gameClient.ClientData.RoleName;
				}
				else
				{
					SafeClientData safeClientDataFromLocalOrDB = Global.GetSafeClientDataFromLocalOrDB(roleid);
					if (safeClientDataFromLocalOrDB != null)
					{
						tradeBlackObject.VipLevel = Global.CalcVipLevelByZuanShi(Global.GetUserInputAllYuanBao(safeClientDataFromLocalOrDB.RoleID, safeClientDataFromLocalOrDB.RoleName, 0));
						tradeBlackObject.ChangeLife = safeClientDataFromLocalOrDB.ChangeLifeCount;
						tradeBlackObject.Level = safeClientDataFromLocalOrDB.Level;
						tradeBlackObject.BanTradeToTicks = safeClientDataFromLocalOrDB.BanTradeToTicks;
						tradeBlackObject.ZoneId = safeClientDataFromLocalOrDB.ZoneID;
						tradeBlackObject.RoleName = safeClientDataFromLocalOrDB.RoleName;
					}
				}
				if (list != null)
				{
					foreach (TradeBlackHourItem tradeBlackHourItem in list)
					{
						int num = tradeBlackHourItem.Hour % 24;
						tradeBlackObject.HourItems[num] = tradeBlackHourItem;
						tradeBlackHourItem.TradeRoles = (tradeBlackHourItem.TradeRoles ?? new HashSet<int>());
					}
				}
				lock (this.TradeBlackObjs)
				{
					if (!this.TradeBlackObjs.ContainsKey(roleid))
					{
						this.TradeBlackObjs[roleid] = tradeBlackObject;
					}
					else
					{
						tradeBlackObject = this.TradeBlackObjs[roleid];
					}
				}
			}
			return tradeBlackObject;
		}

		private TradeBlackHourItem GetBlackHourItem(TradeBlackObject obj, DateTime date)
		{
			TradeBlackHourItem tradeBlackHourItem = obj.HourItems[date.Hour];
			if (tradeBlackHourItem == null || tradeBlackHourItem.Day != date.ToString("yyyy-MM-dd"))
			{
				tradeBlackHourItem = new TradeBlackHourItem();
				tradeBlackHourItem.RoleId = obj.RoleId;
				tradeBlackHourItem.Day = date.ToString("yyyy-MM-dd");
				tradeBlackHourItem.Hour = date.Hour;
				tradeBlackHourItem.TradeRoles = new HashSet<int>();
				obj.HourItems[date.Hour] = tradeBlackHourItem;
			}
			return tradeBlackHourItem;
		}

		private void SaveTradeBlackObject(TradeBlackHourItem obj)
		{
			if (obj != null)
			{
				if (!Global.sendToDB<bool, TradeBlackHourItem>(14008, obj, 0))
				{
					LogManager.WriteLog(2, string.Format("TradeBlackManager.SaveTradeBlackObject failed!, roleid={0}", obj.RoleId), null, true);
				}
			}
		}

		public bool CheckTrade(GameClient client, MoneyTypes moneyType, bool notify = true)
		{
			if (moneyType == MoneyTypes.YuanBao)
			{
				if ((Data.OpenData.paimaihangzuanshi == 2 && client.ClientSocket.session.IsAdult == 0) || Data.OpenData.paimaihangzuanshi <= 0)
				{
					if (notify)
					{
						string lang = GLang.GetLang(37, new object[0]);
						GameManager.ClientMgr.NotifyHintMsg(client, lang);
					}
					return false;
				}
			}
			else if (moneyType == MoneyTypes.MoBi)
			{
				if ((Data.OpenData.paimaihangmobi == 2 && client.ClientSocket.session.IsAdult != 0) || Data.OpenData.paimaihangmobi <= 0)
				{
					if (notify)
					{
						string lang = GLang.GetLang(38, new object[0]);
						GameManager.ClientMgr.NotifyHintMsg(client, lang);
					}
					return false;
				}
			}
			else if (moneyType == MoneyTypes.YinLiang)
			{
				if ((Data.OpenData.paimaihangjinbi == 2 && client.ClientSocket.session.IsAdult != 0) || Data.OpenData.paimaihangjinbi <= 0)
				{
					if (notify)
					{
						string lang = GLang.GetLang(39, new object[0]);
						GameManager.ClientMgr.NotifyHintMsg(client, lang);
					}
					return false;
				}
			}
			return true;
		}

		private const string GoodsPriceCfgFile = "Config\\Blacklist.xml";

		private const string TradeBlackCfgFile = "Config\\TradeConfig.xml";

		private Dictionary<int, TradeBlackObject> TradeBlackObjs = new Dictionary<int, TradeBlackObject>();

		private DateTime lastCheckUnBanTime = TimeUtil.NowDateTime();

		private DateTime lastFreeUnusedTime = TimeUtil.NowDateTime();

		private Dictionary<int, int> GoodsPriceDict = null;

		private List<TradeConfigItem> TradeCfgItems = null;

		private int BanTradeSec = -1;

		private int BanTradeLog = 0;

		private int BanTradeLogin = -1;
	}
}
