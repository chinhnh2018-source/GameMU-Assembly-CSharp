using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using GameServer.Logic.UserMoneyCharge;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class OneDollarChongZhi : Activity
	{
		public bool Init()
		{
			try
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("YiYuanChongZhiOpen");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array = paramValueByName.Split(new char[]
					{
						'|'
					});
					foreach (string text in array)
					{
						string[] array3 = text.Split(new char[]
						{
							','
						});
						if (array3.Length == 2)
						{
							dictionary[Global.SafeConvertToInt32(array3[0])] = Global.SafeConvertToInt32(array3[1]);
						}
					}
				}
				dictionary.TryGetValue(UserMoneyMgr.getInstance().GetActivityPlatformType(), out this.PlatformOpenStateVavle);
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/YiYuanChongZhi.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/YiYuanChongZhi.xml"));
				if (null == xelement)
				{
					return false;
				}
				XElement xelement2 = xelement.Element("YiYuanChongZhi");
				if (null != xelement2)
				{
					this.FromDate = Global.GetSafeAttributeStr(xelement2, "BeginTime");
					this.ToDate = Global.GetSafeAttributeStr(xelement2, "FinishTime");
					this.ActivityType = 46;
					this.AwardStartDate = this.FromDate;
					this.AwardEndDate = this.ToDate;
					this.OneDollarChongZhiData.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
					DateTime.TryParse(this.FromDate, out this.OneDollarChongZhiData.FromDate);
					DateTime.TryParse(this.ToDate, out this.OneDollarChongZhiData.ToDate);
					this.OneDollarChongZhiData.MinZuanShi = (int)Global.GetSafeAttributeLong(xelement2, "MinZhuanShi");
					this.OneDollarChongZhiData.UserListFile = Global.GetSafeAttributeStr(xelement2, "UserList");
					if (!string.IsNullOrEmpty(this.OneDollarChongZhiData.UserListFile))
					{
						string text2 = string.Format("Config/{0}", this.OneDollarChongZhiData.UserListFile);
						text2 = Global.GameResPath(text2);
						if (File.Exists(text2))
						{
							string[] array4 = File.ReadAllLines(text2);
							foreach (string text3 in array4)
							{
								if (!string.IsNullOrEmpty(text3))
								{
									this.OneDollarChongZhiData.UserListSet.Add(text3.ToLower());
								}
							}
						}
					}
					string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsID1");
					string[] array5 = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					if (array5.Length <= 0)
					{
						LogManager.WriteLog(1, string.Format("解析1元充值活动配置文件中的物品配置项1失败", new object[0]), null, true);
					}
					else
					{
						this.OneDollarChongZhiData.GoodsDataListOne = HuodongCachingMgr.ParseGoodsDataList(array5, "1元充值活动配置1");
					}
					string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "GoodsID2");
					if (!string.IsNullOrEmpty(safeAttributeStr2))
					{
						array5 = safeAttributeStr2.Split(new char[]
						{
							'|'
						});
						this.OneDollarChongZhiData.GoodsDataListTwo = HuodongCachingMgr.ParseGoodsDataList(array5, "1元充值活动配置2");
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/YiYuanChongZhi.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public void OnRoleLogin(GameClient client)
		{
			if (null != this.OneDollarChongZhiData)
			{
				string item = client.strUserID.ToLower();
				if (!this.InActivityTime() || !this.OneDollarChongZhiData.UserListSet.Contains(item))
				{
					string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						9,
						0,
						"",
						0,
						0
					});
					client.sendCmd(770, cmdData, false);
				}
				else
				{
					string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						9,
						this.PlatformOpenStateVavle,
						"",
						0,
						0
					});
					client.sendCmd(770, cmdData, false);
				}
			}
		}

		public override bool CanGiveAward(GameClient client, int index, int totalMoney)
		{
			bool result;
			if (!this.InAwardTime())
			{
				result = false;
			}
			else if (0 == this.PlatformOpenStateVavle)
			{
				result = false;
			}
			else
			{
				string item = client.strUserID.ToLower();
				result = this.OneDollarChongZhiData.UserListSet.Contains(item);
			}
			return result;
		}

		public override bool GiveAward(GameClient client)
		{
			AwardItem awardItem = new AwardItem();
			awardItem.GoodsDataList = this.OneDollarChongZhiData.GoodsDataListOne;
			base.GiveAward(client, awardItem);
			awardItem.GoodsDataList = this.OneDollarChongZhiData.GoodsDataListTwo;
			base.GiveAward(client, awardItem);
			return true;
		}

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			bool result;
			if (null == this.OneDollarChongZhiData)
			{
				result = false;
			}
			else
			{
				int num = Global.CalcOriginalOccupationID(client);
				List<GoodsData> list = new List<GoodsData>();
				foreach (GoodsData item in this.OneDollarChongZhiData.GoodsDataListOne)
				{
					list.Add(item);
				}
				int count = this.OneDollarChongZhiData.GoodsDataListTwo.Count;
				for (int i = 0; i < count; i++)
				{
					GoodsData goodsData = this.OneDollarChongZhiData.GoodsDataListTwo[i];
					if (Global.IsCanGiveRewardByOccupation(client, goodsData.GoodsID))
					{
						list.Add(goodsData);
					}
				}
				result = Global.CanAddGoodsDataList(client, list);
			}
			return result;
		}

		public override List<int> GetAwardMinConditionlist()
		{
			List<int> list = new List<int>();
			List<int> result;
			if (null == this.OneDollarChongZhiData)
			{
				result = list;
			}
			else
			{
				list.Add(this.OneDollarChongZhiData.MinZuanShi);
				result = list;
			}
			return result;
		}

		protected const string OneDollarChongZhiData_fileName = "Config/YiYuanChongZhi.xml";

		protected OneDollarChongZhiConfig OneDollarChongZhiData = new OneDollarChongZhiConfig();

		protected int PlatformOpenStateVavle = 0;
	}
}
