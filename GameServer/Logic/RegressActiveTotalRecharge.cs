using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class RegressActiveTotalRecharge : Activity
	{
		public bool Init()
		{
			this.ActivityType = 112;
			this.FromDate = "-1";
			this.ToDate = "-1";
			this.AwardStartDate = "-1";
			this.AwardEndDate = "-1";
			string text = Global.GameResPath("Config\\HuiGuiChongZhiGift.xml");
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
					RegressActiveTotalRechargeXML regressActiveTotalRechargeXML = new RegressActiveTotalRechargeXML();
					regressActiveTotalRechargeXML.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
					regressActiveTotalRechargeXML.HuoDongLevel = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "HuoDongLevel"));
					regressActiveTotalRechargeXML.MinYuanBao = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "MinYuanBao"));
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsID1");
					if (!string.IsNullOrEmpty(safeAttributeStr))
					{
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (array.Length > 0)
						{
							regressActiveTotalRechargeXML.GoodsID1 = GoodsHelper.ParseGoodsDataList(array, text);
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
							regressActiveTotalRechargeXML.GoodsID2 = GoodsHelper.ParseGoodsDataList(array, text);
						}
					}
					this.regressActiveTotalRechargeXML.Add(regressActiveTotalRechargeXML.ID, regressActiveTotalRechargeXML);
				}
				if (this.regressActiveTotalRechargeXML == null)
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

		public bool GiveAwardCheck(GameClient client, int Level, int Money, int RechargeConfID, out List<GoodsData> goodsList)
		{
			goodsList = null;
			RegressActiveTotalRechargeXML regressActiveTotalRechargeXML;
			bool result;
			if (!this.regressActiveTotalRechargeXML.TryGetValue(RechargeConfID, out regressActiveTotalRechargeXML))
			{
				result = false;
			}
			else if (regressActiveTotalRechargeXML.HuoDongLevel != Level)
			{
				result = false;
			}
			else if (Money < regressActiveTotalRechargeXML.MinYuanBao)
			{
				result = false;
			}
			else
			{
				goodsList = regressActiveTotalRechargeXML.GoodsID1;
				result = false;
			}
			return result;
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
						Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Quality, goodsData2.Props, goodsData2.Forge_level, goodsData2.Binding, 15000, goodsData2.Jewellist, true, 1, "三周年回归活动累计充值奖励", false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
					}
					else
					{
						Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Quality, goodsData2.Props, goodsData2.Forge_level, goodsData2.Binding, 0, goodsData2.Jewellist, true, 1, "三周年回归活动累计充值奖励", false, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, false, null, null, "1900-01-01 12:00:00", 0, true);
					}
				}
				result = true;
			}
			return result;
		}

		protected const string RegressActiveTotalRechargeXml = "Config\\HuiGuiChongZhiGift.xml";

		private Dictionary<int, RegressActiveTotalRechargeXML> regressActiveTotalRechargeXML = new Dictionary<int, RegressActiveTotalRechargeXML>();
	}
}
