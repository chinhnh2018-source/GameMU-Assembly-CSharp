using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class ThemeDaLiBaoActivity : Activity
	{
		public override bool GiveAward(GameClient client, int _params)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else
			{
				bool flag = true;
				if (null != this.MyAwardItem)
				{
					flag = base.GiveAward(client, this.MyAwardItem);
				}
				if (flag)
				{
					int occupation = client.ClientData.Occupation;
					AwardItem occAward = this.GetOccAward(occupation);
					if (null != occAward)
					{
						flag = base.GiveAward(client, occAward);
					}
				}
				if (client._IconStateMgr.CheckThemeDaLiBao(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				result = flag;
			}
			return result;
		}

		public AwardItem GetOccAward(int _params)
		{
			AwardItem result = null;
			if (this.OccAwardItemDict.ContainsKey(_params))
			{
				result = this.OccAwardItemDict[_params];
			}
			return result;
		}

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else
			{
				int occupation = client.ClientData.Occupation;
				AwardItem occAward = this.GetOccAward(occupation);
				result = ((this.MyAwardItem.GoodsDataList.Count <= 0 && (occAward == null || occAward.GoodsDataList.Count <= 0)) || Global.CanAddGoodsDataList(client, this.MyAwardItem.GoodsDataList));
			}
			return result;
		}

		public bool Init()
		{
			try
			{
				string uri = "Config/ThemeActivityLiBao.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return false;
				}
				this.ActivityType = 151;
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				this.MyAwardItem = new AwardItem();
				this.MyAwardItem.MinAwardCondionValue = 0;
				this.MyAwardItem.AwardYuanBao = 0;
				XElement xelement2 = xelement.Element("ThemeActivityLiBao");
				if (null == xelement2)
				{
					return false;
				}
				string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsOne");
				if (string.IsNullOrEmpty(safeAttributeStr))
				{
					LogManager.WriteLog(1, string.Format("读取大型主题服礼包活动配置文件中的物品配置项1失败", new object[0]), null, true);
				}
				else
				{
					string[] array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					if (array.Length <= 0)
					{
						LogManager.WriteLog(1, string.Format("解析大型主题服礼包活动配置文件中的物品配置项1失败", new object[0]), null, true);
					}
					else
					{
						this.MyAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型主题服礼包配置1");
					}
				}
				safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsTwo");
				if (string.IsNullOrEmpty(safeAttributeStr))
				{
					LogManager.WriteLog(1, string.Format("读取大型主题服礼包活动配置文件中的物品配置项2失败", new object[0]), null, true);
				}
				else
				{
					string[] array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					if (array.Length <= 0)
					{
						LogManager.WriteLog(1, string.Format("解析大型主题服礼包活动配置文件中的物品配置项2失败", new object[0]), null, true);
					}
					else
					{
						List<GoodsData> list = HuodongCachingMgr.ParseGoodsDataList(array, "大型主题服礼包配置2");
						foreach (GoodsData goodsData in list)
						{
							SystemXmlItem systemXmlItem = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
							{
								int mainOccupationByGoodsID = Global.GetMainOccupationByGoodsID(goodsData.GoodsID);
								AwardItem awardItem = this.GetOccAward(mainOccupationByGoodsID);
								if (null == awardItem)
								{
									awardItem = new AwardItem();
									awardItem.GoodsDataList.Add(goodsData);
									this.OccAwardItemDict[mainOccupationByGoodsID] = awardItem;
								}
								else
								{
									awardItem.GoodsDataList.Add(goodsData);
								}
							}
						}
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/ThemeActivityLiBao.xml解析出现异常", ex, true);
				return false;
			}
			return true;
		}

		public AwardItem MyAwardItem = new AwardItem();

		public Dictionary<int, AwardItem> OccAwardItemDict = new Dictionary<int, AwardItem>();
	}
}
