using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.ActivityNew.SevenDay
{
	public class SevenDayBuyAct
	{
		public void LoadConfig()
		{
			Dictionary<int, SevenDayBuyAct._BuyGoodsData> dictionary = new Dictionary<int, SevenDayBuyAct._BuyGoodsData>();
			try
			{
				XElement xelement = XElement.Load(Global.GameResPath("Config/SevenDay/SevenDayQiangGou.xml"));
				foreach (XElement xml in xelement.Elements())
				{
					SevenDayBuyAct._BuyGoodsData buyGoodsData = new SevenDayBuyAct._BuyGoodsData();
					buyGoodsData.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
					buyGoodsData.Day = (int)Global.GetSafeAttributeLong(xml, "Day");
					buyGoodsData.OriginPrice = (int)Global.GetSafeAttributeLong(xml, "OrigPrice");
					buyGoodsData.CurrPrice = (int)Global.GetSafeAttributeLong(xml, "Price");
					buyGoodsData.MaxBuyCount = (int)Global.GetSafeAttributeLong(xml, "Purchase");
					buyGoodsData.Goods = Global.ParseGoodsFromStr_7(Global.GetSafeAttributeStr(xml, "GoodsID").Split(new char[]
					{
						','
					}), 0);
					dictionary[buyGoodsData.Id] = buyGoodsData;
				}
				lock (this.ConfigMutex)
				{
					this._BuyGoodsDict = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("七日登录活动加载配置失败{0}", "Config/SevenDay/SevenDayQiangGou.xml"), ex, true);
			}
		}

		public ESevenDayActErrorCode HandleClientBuy(GameClient client, int id, int cnt)
		{
			int num;
			ESevenDayActErrorCode result;
			if (!SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client, out num))
			{
				result = ESevenDayActErrorCode.NotInActivityTime;
			}
			else
			{
				SevenDayBuyAct._BuyGoodsData buyGoodsData = null;
				lock (this.ConfigMutex)
				{
					if (this._BuyGoodsDict == null || !this._BuyGoodsDict.TryGetValue(id, out buyGoodsData))
					{
						return ESevenDayActErrorCode.ServerConfigError;
					}
				}
				if (buyGoodsData == null || buyGoodsData.Goods == null)
				{
					result = ESevenDayActErrorCode.ServerConfigError;
				}
				else if (buyGoodsData.Day > num)
				{
					result = ESevenDayActErrorCode.NotReachCondition;
				}
				else
				{
					Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Buy);
					lock (activityData)
					{
						SevenDayItemData sevenDayItemData = null;
						if (!activityData.TryGetValue(id, out sevenDayItemData))
						{
							sevenDayItemData = new SevenDayItemData();
							activityData[id] = sevenDayItemData;
						}
						if (cnt <= 0 || sevenDayItemData.Params1 + cnt > buyGoodsData.MaxBuyCount)
						{
							result = ESevenDayActErrorCode.NoEnoughGoodsCanBuy;
						}
						else if (client.ClientData.UserMoney < cnt * buyGoodsData.CurrPrice)
						{
							result = ESevenDayActErrorCode.ZuanShiNotEnough;
						}
						else if (!Global.CanAddGoods(client, buyGoodsData.Goods.GoodsID, buyGoodsData.Goods.GCount * cnt, buyGoodsData.Goods.Binding, "1900-01-01 12:00:00", true, false))
						{
							result = ESevenDayActErrorCode.NoBagSpace;
						}
						else
						{
							sevenDayItemData.Params1 += cnt;
							if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(client.ClientData.RoleID, ESevenDayActType.Buy, id, sevenDayItemData, client.ServerId))
							{
								sevenDayItemData.Params1 -= cnt;
								result = ESevenDayActErrorCode.DBFailed;
							}
							else
							{
								if (!GameManager.ClientMgr.SubUserMoney(client, cnt * buyGoodsData.CurrPrice, "七日抢购", true, true, true, true, DaiBiSySType.None))
								{
									LogManager.WriteLog(2, string.Format("玩家七日抢购物品，检查钻石足够，但是扣除失败,roleid={0}, id={1}", client.ClientData.RoleID, id), null, true);
								}
								GoodsData goods = buyGoodsData.Goods;
								Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goods.GoodsID, goods.GCount * cnt, goods.Quality, goods.Props, goods.Forge_level, goods.Binding, 0, goods.Jewellist, true, 1, string.Format("七日抢购", new object[0]), false, goods.Endtime, goods.AddPropIndex, goods.BornIndex, goods.Lucky, goods.Strong, goods.ExcellenceInfo, goods.AppendPropLev, goods.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
								result = ESevenDayActErrorCode.Success;
							}
						}
					}
				}
			}
			return result;
		}

		public bool HasAnyCanBuy(GameClient client)
		{
			int num;
			bool result;
			if (!SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client, out num))
			{
				result = false;
			}
			else
			{
				Dictionary<int, SevenDayBuyAct._BuyGoodsData> dictionary = null;
				lock (this.ConfigMutex)
				{
					if ((dictionary = this._BuyGoodsDict) == null || dictionary.Count <= 0)
					{
						return false;
					}
				}
				Dictionary<int, SevenDayItemData> activityData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Buy);
				lock (activityData)
				{
					foreach (KeyValuePair<int, SevenDayBuyAct._BuyGoodsData> keyValuePair in dictionary)
					{
						SevenDayBuyAct._BuyGoodsData value = keyValuePair.Value;
						if (value != null && value.Day <= num)
						{
							int num2 = 0;
							SevenDayItemData sevenDayItemData = null;
							if (activityData.TryGetValue(keyValuePair.Key, out sevenDayItemData))
							{
								num2 = sevenDayItemData.Params1;
							}
							if (value.MaxBuyCount > num2)
							{
								return true;
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		private Dictionary<int, SevenDayBuyAct._BuyGoodsData> _BuyGoodsDict = null;

		private object ConfigMutex = new object();

		private class _BuyGoodsData
		{
			public int Id;

			public int Day;

			public int OriginPrice;

			public int CurrPrice;

			public int MaxBuyCount;

			public GoodsData Goods;
		}
	}
}
