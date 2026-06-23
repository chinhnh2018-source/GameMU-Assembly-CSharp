using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class RegressActiveStore : Activity
	{
		public bool Init()
		{
			this.ActivityType = 114;
			this.FromDate = "-1";
			this.ToDate = "-1";
			this.AwardStartDate = "-1";
			this.AwardEndDate = "-1";
			string text = Global.GameResPath("Config\\HuiGuiStore.xml");
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
					RegressActiveStoreXML regressActiveStoreXML = new RegressActiveStoreXML();
					regressActiveStoreXML.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
					regressActiveStoreXML.HuoDongLevel = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "HuoDongLevel"));
					regressActiveStoreXML.Day = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "Day"));
					regressActiveStoreXML.OrigPrice = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "OrigPrice"));
					regressActiveStoreXML.Price = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "Price"));
					regressActiveStoreXML.SinglePurchase = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "SinglePurchase"));
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsID");
					if (!string.IsNullOrEmpty(safeAttributeStr))
					{
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (array.Length > 0)
						{
							regressActiveStoreXML.GoodsID = GoodsHelper.ParseGoodsDataList(array, text);
						}
					}
					this.regressActiveStoreXML.Add(regressActiveStoreXML.ID, regressActiveStoreXML);
				}
				if (this.regressActiveStoreXML == null)
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

		public bool RegressStoreGoodsBuyCheck(GameClient client, int ConfID, int Level, int Day, int GoodsID, int Count, string stage, out int needYuanBao, out int Sum, out GoodsData goodData)
		{
			needYuanBao = 0;
			Sum = 0;
			goodData = null;
			RegressActiveStoreXML regressActiveStoreXML;
			bool result;
			if (!this.regressActiveStoreXML.TryGetValue(ConfID, out regressActiveStoreXML))
			{
				result = false;
			}
			else if (regressActiveStoreXML.HuoDongLevel != Level || regressActiveStoreXML.Day != Day)
			{
				result = false;
			}
			else if (Count <= 0)
			{
				result = false;
			}
			else
			{
				GoodsData goodsData = null;
				foreach (GoodsData goodsData2 in regressActiveStoreXML.GoodsID)
				{
					if (goodsData2.GoodsID == GoodsID)
					{
						goodsData = goodsData2;
					}
				}
				if (goodsData == null)
				{
					result = false;
				}
				else
				{
					string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, ConfID, stage);
					string[] array;
					if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14134, strcmd, out array, 0))
					{
						result = false;
					}
					else if (array == null || array.Length != 4 || Convert.ToInt32(array[1]) != 0)
					{
						result = false;
					}
					else
					{
						int num = Convert.ToInt32(array[3]);
						int num2 = regressActiveStoreXML.SinglePurchase - num;
						if (num2 < Count)
						{
							result = false;
						}
						else
						{
							needYuanBao = Count * regressActiveStoreXML.Price;
							Sum = num + Count;
							goodData = goodsData;
							result = true;
						}
					}
				}
			}
			return result;
		}

		protected const string RegressActiveStoreXml = "Config\\HuiGuiStore.xml";

		private Dictionary<int, RegressActiveStoreXML> regressActiveStoreXML = new Dictionary<int, RegressActiveStoreXML>();
	}
}
