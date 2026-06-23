using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.UserMoneyCharge;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class JieRiCZQGActivity : Activity, IEventListener
	{
		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 36)
			{
				ChargeItemBaseEventObject chargeItemBaseEventObject = eventObject as ChargeItemBaseEventObject;
				if (this.CZQGZhiGouIDSet.Contains(chargeItemBaseEventObject.ChargeItemConfig.ChargeItemID))
				{
					List<JieriCZQGData> cmdData = this.BuildChongZhiQiangGouInfoForClient(chargeItemBaseEventObject.Player);
					chargeItemBaseEventObject.Player.sendCmd<List<JieriCZQGData>>(1620, cmdData, false);
				}
			}
		}

		public void Dispose()
		{
			GlobalEventSource.getInstance().removeListener(36, this);
		}

		public bool Init()
		{
			try
			{
				string uri = "Config/JieRiGifts/JieRiChongZhiQiangGou.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
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
					foreach (XElement xml in enumerable)
					{
						JieriCZQGConfigData jieriCZQGConfigData = new JieriCZQGConfigData();
						jieriCZQGConfigData.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						jieriCZQGConfigData.ZhiGouID = (int)Global.GetSafeAttributeLong(xml, "ZhiGouID");
						jieriCZQGConfigData.SinglePurchase = (int)Global.GetSafeAttributeLong(xml, "SinglePurchase");
						string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsOne");
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xml, "GoodsTwo");
						UserMoneyMgr.getInstance().CheckChargeItemConfigLogic(jieriCZQGConfigData.ZhiGouID, jieriCZQGConfigData.SinglePurchase, safeAttributeStr, safeAttributeStr2, string.Format("充值抢购 ID={0}", jieriCZQGConfigData.ID));
						string safeAttributeStr3 = Global.GetSafeAttributeStr(xml, "Day");
						string[] array = safeAttributeStr3.Split(new char[]
						{
							','
						});
						if (array.Length == 2)
						{
							int addDays = Global.SafeConvertToInt32(array[0]) - 1;
							int addDays2 = Global.SafeConvertToInt32(array[1]);
							DateTime dateTime = DateTime.Parse(this.FromDate);
							jieriCZQGConfigData.FromDate = Global.GetAddDaysDataTime(dateTime, addDays, true);
							jieriCZQGConfigData.ToDate = Global.GetAddDaysDataTime(dateTime, addDays2, true);
						}
						this.CZQGConfigDict[jieriCZQGConfigData.ID] = jieriCZQGConfigData;
						this.CZQGZhiGouIDSet.Add(jieriCZQGConfigData.ZhiGouID);
					}
				}
				base.PredealDateTime();
				GlobalEventSource.getInstance().registerListener(36, this);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/JieRiGifts/JieRiChongZhiQiangGou.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public List<JieriCZQGData> BuildChongZhiQiangGouInfoForClient(GameClient client)
		{
			List<JieriCZQGData> list = new List<JieriCZQGData>();
			List<JieriCZQGData> result;
			if (!this.InActivityTime())
			{
				result = list;
			}
			else
			{
				foreach (KeyValuePair<int, JieriCZQGConfigData> keyValuePair in this.CZQGConfigDict)
				{
					if (!(TimeUtil.NowDateTime() < keyValuePair.Value.FromDate) && !(TimeUtil.NowDateTime() > keyValuePair.Value.ToDate))
					{
						JieriCZQGData jieriCZQGData = new JieriCZQGData();
						jieriCZQGData.ID = keyValuePair.Value.ID;
						lock (client.ClientData.ChargeItemPurchaseDict)
						{
							Dictionary<int, int> chargeItemPurchaseDict2 = client.ClientData.ChargeItemPurchaseDict;
							chargeItemPurchaseDict2.TryGetValue(keyValuePair.Value.ZhiGouID, out jieriCZQGData.PurchaseNum);
						}
						list.Add(jieriCZQGData);
					}
				}
				result = list;
			}
			return result;
		}

		protected Dictionary<int, JieriCZQGConfigData> CZQGConfigDict = new Dictionary<int, JieriCZQGConfigData>();

		protected HashSet<int> CZQGZhiGouIDSet = new HashSet<int>();
	}
}
