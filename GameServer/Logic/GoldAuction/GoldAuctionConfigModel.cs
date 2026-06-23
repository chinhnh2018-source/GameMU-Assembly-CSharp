using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Tools;
using Server.Tools;

namespace GameServer.Logic.GoldAuction
{
	public class GoldAuctionConfigModel
	{
		public static int LoadConfig()
		{
			try
			{
				Dictionary<int, AuctionConfig> auctionDict;
				GoldAuctionConfigModel.LoadAuctionData(out auctionDict);
				List<AuctionAwardConfig> auctionAwardList;
				GoldAuctionConfigModel.LoadAngelTempleAuctionAwardData(out auctionAwardList);
				lock (GoldAuctionConfigModel.AuctionDict)
				{
					GoldAuctionConfigModel.AuctionDict = auctionDict;
				}
				lock (GoldAuctionConfigModel.AuctionAwardList)
				{
					GoldAuctionConfigModel.AuctionAwardList = auctionAwardList;
				}
				return 1;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return 0;
		}

		public static AuctionConfig GetAuctionConfig(int id)
		{
			AuctionConfig result = null;
			try
			{
				lock (GoldAuctionConfigModel.AuctionDict)
				{
					GoldAuctionConfigModel.AuctionDict.TryGetValue(id, out result);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return result;
		}

		public static AuctionAwardConfig RandAuctionAwardConfig()
		{
			try
			{
				lock (GoldAuctionConfigModel.AuctionAwardList)
				{
					if (GoldAuctionConfigModel.AuctionAwardList.Count < 1)
					{
						LogManager.WriteLog(2, "[ljl]AuctionAwardList.Count < 1", null, true);
						return null;
					}
					List<int> list = new List<int>();
					foreach (AuctionAwardConfig auctionAwardConfig in GoldAuctionConfigModel.AuctionAwardList)
					{
						int num = auctionAwardConfig.EndValues - auctionAwardConfig.StartValues + 1;
						if (1 == num && 0 == auctionAwardConfig.EndValues)
						{
							num = 0;
						}
						else if (num < 0)
						{
							num = 0;
						}
						list.Add(num);
					}
					return GoldAuctionConfigModel.AuctionAwardList[RandomWeight.GetWeightIndex(list, "金团随机")];
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return null;
		}

		private static bool LoadAngelTempleAuctionAwardData(out List<AuctionAwardConfig> _AuctionAwardList)
		{
			_AuctionAwardList = new List<AuctionAwardConfig>();
			try
			{
				XElement xelement = CheckHelper.LoadXml(Global.GameResPath("Config/AngelTempleAuctionAward.xml"), true);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("读取 {0} null == xml", "Config/AngelTempleAuctionAward.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						AuctionAwardConfig auctionAwardConfig = new AuctionAwardConfig();
						auctionAwardConfig.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						auctionAwardConfig.Name = Global.GetSafeAttributeStr(xelement2, "Name");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsID");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							auctionAwardConfig.strGoodsList = array.ToList<string>();
							HuodongCachingMgr.ParseGoodsDataList(array, "Config/AngelTempleAuctionAward.xml");
						}
						auctionAwardConfig.StartValues = (int)Global.GetSafeAttributeLong(xelement2, "StartValues");
						auctionAwardConfig.EndValues = (int)Global.GetSafeAttributeLong(xelement2, "EndValues");
						if (auctionAwardConfig.EndValues - auctionAwardConfig.StartValues < 0 || auctionAwardConfig.StartValues < 0)
						{
							LogManager.WriteLog(1000, string.Format("{0}解析出现问题, 第{1} 条数据 StartValues EndValues 值不对 ", "Config/AngelTempleAuctionAward.xml", _AuctionAwardList.Count + 1), null, true);
						}
						_AuctionAwardList.Add(auctionAwardConfig);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/AngelTempleAuctionAward.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		private static bool LoadAuctionData(out Dictionary<int, AuctionConfig> _AuctionAwardDict)
		{
			_AuctionAwardDict = new Dictionary<int, AuctionConfig>();
			try
			{
				XElement xelement = CheckHelper.LoadXml(Global.GameResPath("Config/Auction.xml"), true);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("读取 {0} null == xml", "Config/Auction.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						AuctionConfig auctionConfig = new AuctionConfig();
						auctionConfig.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						auctionConfig.Name = Global.GetSafeAttributeStr(xelement2, "Name");
						auctionConfig.OriginPrice = (int)Global.GetSafeAttributeLong(xelement2, "OriginPrice");
						auctionConfig.UnitPrice = (int)Global.GetSafeAttributeLong(xelement2, "UnitPrice");
						auctionConfig.MaxPrice = (int)Global.GetSafeAttributeLong(xelement2, "MaxPrice");
						auctionConfig.SuccessTitle = Global.GetSafeAttributeStr(xelement2, "SuccessTitle");
						auctionConfig.SuccessIntro = Global.GetSafeAttributeStr(xelement2, "SuccessIntro");
						auctionConfig.FailTitle = Global.GetSafeAttributeStr(xelement2, "FailTitle");
						auctionConfig.FailIntro = Global.GetSafeAttributeStr(xelement2, "FailIntro");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "List");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							foreach (int item in Global.StringArray2IntArray(safeAttributeStr.Split(new char[]
							{
								'|'
							})))
							{
								auctionConfig.OrderList.Add(item);
							}
						}
						safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "Time");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							foreach (int item in Global.StringArray2IntArray(safeAttributeStr.Split(new char[]
							{
								'|'
							})))
							{
								auctionConfig.TimeList.Add(item);
							}
						}
						if (auctionConfig.TimeList.Count != auctionConfig.OrderList.Count)
						{
							LogManager.WriteLog(1000, string.Format("{0}解析出现问题, 第{1} 条数据 进入拍卖行顺序 竞拍时间长度不同 ", "Config/Auction.xml", _AuctionAwardDict.Count + 1), null, true);
						}
						_AuctionAwardDict.Add(auctionConfig.ID, auctionConfig);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/Auction.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		private const string Auction = "Config/Auction.xml";

		private const string AngelTempleAuctionAward = "Config/AngelTempleAuctionAward.xml";

		public const string AngelTempleAuction = "AngelTempleAuction";

		public const string AngelTempleAuctionMin = "AngelTempleAuctionMin";

		public const string AuctionZhanMengOpen = "AuctionZhanMengOpen";

		public const string BuyTitle = "购买成功";

		public const string BuyIntro = "在金团拍卖购买成功";

		public const string BuyFailTitle = "竞拍失败";

		public const string BuyFailIntro = "您参与的活动奖励{0}，在竞价中被超过,返还您{1}钻石";

		private static Dictionary<int, AuctionConfig> AuctionDict = new Dictionary<int, AuctionConfig>();

		private static List<AuctionAwardConfig> AuctionAwardList = new List<AuctionAwardConfig>();
	}
}
