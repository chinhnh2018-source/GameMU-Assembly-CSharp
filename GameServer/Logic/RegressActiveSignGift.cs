using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class RegressActiveSignGift : Activity
	{
		public bool Init()
		{
			this.ActivityType = 111;
			this.FromDate = "-1";
			this.ToDate = "-1";
			this.AwardStartDate = "-1";
			this.AwardEndDate = "-1";
			string text = Global.GameResPath("Config\\HuiGuiLoginNumGift.xml");
			XElement xelement = XElement.Load(text);
			if (null == xelement)
			{
				LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
			}
			try
			{
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					RegressActiveSignGiftXML regressActiveSignGiftXML = new RegressActiveSignGiftXML();
					regressActiveSignGiftXML.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
					regressActiveSignGiftXML.HuoDongLevel = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "HuoDongLevel"));
					regressActiveSignGiftXML.TimeOl = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "TimeOl"));
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsID1");
					if (!string.IsNullOrEmpty(safeAttributeStr))
					{
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (array.Length > 0)
						{
							regressActiveSignGiftXML.GoodsID1 = GoodsHelper.ParseGoodsDataList(array, text);
						}
					}
					safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsID2");
					if (!string.IsNullOrEmpty(safeAttributeStr))
					{
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (array.Length > 0)
						{
							regressActiveSignGiftXML.GoodsID2 = GoodsHelper.ParseGoodsDataList(array, text);
						}
					}
					this.regressActiveSignGiftXML.Add(regressActiveSignGiftXML.ID, regressActiveSignGiftXML);
				}
				if (this.regressActiveSignGiftXML == null)
				{
					return false;
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return true;
		}

		public void OnRoleLogin(GameClient client)
		{
			if (!this.InActivityTime())
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					16,
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
					16,
					RegressActiveOpen.OpenStateVavle,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
		}

		public bool GetAwardGoodsList(GameClient client, int Level, int Day, out List<GoodsData> OutGoodsData, out int DBDay)
		{
			DBDay = 0;
			OutGoodsData = new List<GoodsData>();
			foreach (RegressActiveSignGiftXML regressActiveSignGiftXML in this.regressActiveSignGiftXML.Values)
			{
				if (regressActiveSignGiftXML.HuoDongLevel == Level)
				{
					if (regressActiveSignGiftXML.TimeOl == Day && regressActiveSignGiftXML.GoodsID1 != null)
					{
						OutGoodsData.AddRange(regressActiveSignGiftXML.GoodsID1);
						DBDay = regressActiveSignGiftXML.TimeOl;
						return true;
					}
				}
			}
			return false;
		}

		public bool GiveAward(GameClient client, List<GoodsData> goodsData)
		{
			bool result;
			if (goodsData == null)
			{
				result = false;
			}
			else
			{
				foreach (GoodsData goodsData2 in goodsData)
				{
					if (Global.GetGoodsRebornEquip(goodsData2.GoodsID) == 1)
					{
						Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Quality, goodsData2.Props, goodsData2.Forge_level, goodsData2.Binding, 15000, goodsData2.Jewellist, true, 1, "三周年每日签到奖励", false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
					}
					else
					{
						Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Quality, goodsData2.Props, goodsData2.Forge_level, goodsData2.Binding, 0, goodsData2.Jewellist, true, 1, "三周年每日签到奖励", false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
					}
				}
				result = true;
			}
			return result;
		}

		protected const string RegressActiveSignGiftXml = "Config\\HuiGuiLoginNumGift.xml";

		private Dictionary<int, RegressActiveSignGiftXML> regressActiveSignGiftXML = new Dictionary<int, RegressActiveSignGiftXML>();
	}
}
