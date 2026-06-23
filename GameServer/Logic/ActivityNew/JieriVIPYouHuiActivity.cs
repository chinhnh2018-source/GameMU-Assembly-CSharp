using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class JieriVIPYouHuiActivity : Activity
	{
		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/JieRiGifts/VIPYouHuiLiBao.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/JieRiGifts/VIPYouHuiLiBao.xml"));
				if (null == xelement)
				{
					return false;
				}
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					this.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					this.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					this.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					this.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					this.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
				}
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xelement3 in enumerable)
					{
						if (null != xelement3)
						{
							JieriVIPYouHuiActivityConfig jieriVIPYouHuiActivityConfig = new JieriVIPYouHuiActivityConfig();
							jieriVIPYouHuiActivityConfig.ID = (int)Global.GetSafeAttributeLong(xelement3, "ID");
							jieriVIPYouHuiActivityConfig.MinVIPLev = (int)Global.GetSafeAttributeLong(xelement3, "VIPLevel");
							jieriVIPYouHuiActivityConfig.Price = (int)Global.GetSafeAttributeLong(xelement3, "Price");
							jieriVIPYouHuiActivityConfig.SinglePurchase = (int)Global.GetSafeAttributeLong(xelement3, "SinglePurchase");
							jieriVIPYouHuiActivityConfig.FullPurchase = (int)Global.GetSafeAttributeLong(xelement3, "FullPurchase");
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("解析节日活动VIP优惠配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								jieriVIPYouHuiActivityConfig.GoodsDataListOne = HuodongCachingMgr.ParseGoodsDataList(array, "节日活动VIP优惠配置1");
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (!string.IsNullOrEmpty(safeAttributeStr2))
							{
								array = safeAttributeStr2.Split(new char[]
								{
									'|'
								});
								jieriVIPYouHuiActivityConfig.GoodsDataListTwo = HuodongCachingMgr.ParseGoodsDataList(array, "节日活动VIP优惠配置2");
							}
							string safeAttributeStr3 = Global.GetSafeAttributeStr(xelement3, "GoodsThr");
							jieriVIPYouHuiActivityConfig.GoodsDataListThr.Init(safeAttributeStr3, Global.GetSafeAttributeStr(xelement3, "EffectiveTime"), "节日活动VIP优惠配置3");
							this.VIPYouHuiCofigDict[jieriVIPYouHuiActivityConfig.ID] = jieriVIPYouHuiActivityConfig;
						}
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/JieRiGifts/ChongZhiDuiHuan.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public int GetSinglePurchase(GameClient client, int extTag)
		{
			string[] array = null;
			string roleParamByName = Global.GetRoleParamByName(client, "35");
			if (!string.IsNullOrEmpty(roleParamByName))
			{
				array = roleParamByName.Split(new char[]
				{
					','
				});
			}
			int offsetDay = Global.GetOffsetDay(DateTime.Parse(this.FromDate));
			int result;
			if (array == null || offsetDay != Global.SafeConvertToInt32(array[0]))
			{
				Global.SaveRoleParamsStringToDB(client, "35", string.Format("{0}", offsetDay), true);
				result = 0;
			}
			else
			{
				for (int i = 1; i < array.Length - 1; i += 2)
				{
					if (extTag == Global.SafeConvertToInt32(array[i]))
					{
						return Global.SafeConvertToInt32(array[i + 1]);
					}
				}
				result = 0;
			}
			return result;
		}

		public int GetFullPurchase(GameClient client, int extTag)
		{
			string[] array = null;
			string gameConfigItemStr = GameManager.GameConfigMgr.GetGameConfigItemStr("vip_fullpurchase", "");
			if (!string.IsNullOrEmpty(gameConfigItemStr))
			{
				array = gameConfigItemStr.Split(new char[]
				{
					','
				});
			}
			int offsetDay = Global.GetOffsetDay(DateTime.Parse(this.FromDate));
			int result;
			if (array == null || offsetDay != Global.SafeConvertToInt32(array[0]))
			{
				GameManager.GameConfigMgr.SetGameConfigItem("vip_fullpurchase", string.Format("{0}", offsetDay));
				Global.UpdateDBGameConfigg("vip_fullpurchase", string.Format("{0}", offsetDay));
				result = 0;
			}
			else
			{
				for (int i = 1; i < array.Length - 1; i += 2)
				{
					if (extTag == Global.SafeConvertToInt32(array[i]))
					{
						return Global.SafeConvertToInt32(array[i + 1]);
					}
				}
				result = 0;
			}
			return result;
		}

		protected void UpdateSinglePurchase(GameClient client, int extTag)
		{
			string roleParamByName = Global.GetRoleParamByName(client, "35");
			if (!string.IsNullOrEmpty(roleParamByName))
			{
				string[] array = roleParamByName.Split(new char[]
				{
					','
				});
				if (array.Length != 0)
				{
					string text = array[0];
					bool flag = true;
					for (int i = 1; i < array.Length - 1; i += 2)
					{
						object obj = text;
						text = string.Concat(new object[]
						{
							obj,
							',',
							array[i],
							','
						});
						if (extTag == Global.SafeConvertToInt32(array[i]))
						{
							text += string.Format("{0}", Global.SafeConvertToInt32(array[i + 1]) + 1);
							flag = false;
						}
						else
						{
							text += array[i + 1];
						}
					}
					if (flag)
					{
						object obj = text;
						text = string.Concat(new object[]
						{
							obj,
							",",
							extTag,
							",1"
						});
					}
					Global.SaveRoleParamsStringToDB(client, "35", text, true);
				}
			}
		}

		protected void UpdateFullPurchase(GameClient client, int extTag)
		{
			string gameConfigItemStr = GameManager.GameConfigMgr.GetGameConfigItemStr("vip_fullpurchase", "");
			if (!string.IsNullOrEmpty(gameConfigItemStr))
			{
				string[] array = gameConfigItemStr.Split(new char[]
				{
					','
				});
				if (array.Length != 0)
				{
					string text = array[0];
					bool flag = true;
					for (int i = 1; i < array.Length - 1; i += 2)
					{
						object obj = text;
						text = string.Concat(new object[]
						{
							obj,
							',',
							array[i],
							','
						});
						if (extTag == Global.SafeConvertToInt32(array[i]))
						{
							text += string.Format("{0}", Global.SafeConvertToInt32(array[i + 1]) + 1);
							flag = false;
						}
						else
						{
							text += array[i + 1];
						}
					}
					if (flag)
					{
						object obj = text;
						text = string.Concat(new object[]
						{
							obj,
							",",
							extTag,
							",1"
						});
					}
					GameManager.GameConfigMgr.SetGameConfigItem("vip_fullpurchase", text);
					Global.UpdateDBGameConfigg("vip_fullpurchase", text);
				}
			}
		}

		public string BuildQueryVIPYouHuiActivityCmd(GameClient client)
		{
			this.GetFullPurchase(client, 0);
			this.GetSinglePurchase(client, 0);
			string text = "";
			string gameConfigItemStr = GameManager.GameConfigMgr.GetGameConfigItemStr("vip_fullpurchase", "");
			string roleParamByName = Global.GetRoleParamByName(client, "35");
			if (!string.IsNullOrEmpty(gameConfigItemStr) && gameConfigItemStr.Split(new char[]
			{
				','
			}).Length > 1)
			{
				text += gameConfigItemStr.Substring(gameConfigItemStr.IndexOf(',') + 1);
			}
			text += '|';
			if (!string.IsNullOrEmpty(roleParamByName) && roleParamByName.Split(new char[]
			{
				','
			}).Length > 1)
			{
				text += roleParamByName.Substring(roleParamByName.IndexOf(',') + 1);
			}
			return text;
		}

		public override bool CheckCondition(GameClient client, int extTag)
		{
			bool result;
			if (!this.InActivityTime())
			{
				result = false;
			}
			else
			{
				JieriVIPYouHuiActivityConfig jieriVIPYouHuiActivityConfig = null;
				result = (this.VIPYouHuiCofigDict.TryGetValue(extTag, out jieriVIPYouHuiActivityConfig) && (jieriVIPYouHuiActivityConfig.MinVIPLev < 0 || client.ClientData.VipLevel >= jieriVIPYouHuiActivityConfig.MinVIPLev) && client.ClientData.UserMoney >= jieriVIPYouHuiActivityConfig.Price && this.GetSinglePurchase(client, extTag) < jieriVIPYouHuiActivityConfig.SinglePurchase && this.GetFullPurchase(client, extTag) < jieriVIPYouHuiActivityConfig.FullPurchase);
			}
			return result;
		}

		public override bool GiveAward(GameClient client, int _params)
		{
			bool result;
			if (!this.InAwardTime())
			{
				result = false;
			}
			else
			{
				JieriVIPYouHuiActivityConfig jieriVIPYouHuiActivityConfig = null;
				if (!this.VIPYouHuiCofigDict.TryGetValue(_params, out jieriVIPYouHuiActivityConfig))
				{
					result = false;
				}
				else
				{
					if (jieriVIPYouHuiActivityConfig.Price > 0)
					{
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, jieriVIPYouHuiActivityConfig.Price, "节日活动VIP优惠", true, true, false, DaiBiSySType.None))
						{
							return false;
						}
					}
					AwardItem awardItem = new AwardItem();
					awardItem.GoodsDataList = jieriVIPYouHuiActivityConfig.GoodsDataListOne;
					base.GiveAward(client, awardItem);
					awardItem.GoodsDataList = jieriVIPYouHuiActivityConfig.GoodsDataListTwo;
					base.GiveAward(client, awardItem);
					awardItem = jieriVIPYouHuiActivityConfig.GoodsDataListThr.ToAwardItem();
					base.GiveEffectiveTimeAward(client, awardItem);
					this.UpdateSinglePurchase(client, _params);
					this.UpdateFullPurchase(client, _params);
					result = true;
				}
			}
			return result;
		}

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int id)
		{
			JieriVIPYouHuiActivityConfig jieriVIPYouHuiActivityConfig = null;
			bool result;
			if (!this.VIPYouHuiCofigDict.TryGetValue(id, out jieriVIPYouHuiActivityConfig))
			{
				result = false;
			}
			else
			{
				int num = Global.CalcOriginalOccupationID(client);
				List<GoodsData> list = new List<GoodsData>();
				foreach (GoodsData item in jieriVIPYouHuiActivityConfig.GoodsDataListOne)
				{
					list.Add(item);
				}
				int count = jieriVIPYouHuiActivityConfig.GoodsDataListTwo.Count;
				for (int i = 0; i < count; i++)
				{
					GoodsData goodsData = jieriVIPYouHuiActivityConfig.GoodsDataListTwo[i];
					if (Global.IsCanGiveRewardByOccupation(client, goodsData.GoodsID))
					{
						list.Add(goodsData);
					}
				}
				AwardItem awardItem = jieriVIPYouHuiActivityConfig.GoodsDataListThr.ToAwardItem();
				foreach (GoodsData item in awardItem.GoodsDataList)
				{
					list.Add(item);
				}
				result = Global.CanAddGoodsDataList(client, list);
			}
			return result;
		}

		public bool CanGetAnyAward(GameClient client)
		{
			foreach (KeyValuePair<int, JieriVIPYouHuiActivityConfig> keyValuePair in this.VIPYouHuiCofigDict)
			{
				if (this.CheckCondition(client, keyValuePair.Value.ID))
				{
					return true;
				}
			}
			return false;
		}

		public const string VIPYouHuiActivityData_fileName = "Config/JieRiGifts/VIPYouHuiLiBao.xml";

		protected Dictionary<int, JieriVIPYouHuiActivityConfig> VIPYouHuiCofigDict = new Dictionary<int, JieriVIPYouHuiActivityConfig>();
	}
}
