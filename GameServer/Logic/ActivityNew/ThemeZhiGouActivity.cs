using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.UserMoneyCharge;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class ThemeZhiGouActivity : Activity, IEventListener
	{
		public void Dispose()
		{
			GlobalEventSource.getInstance().removeListener(36, this);
		}

		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/ThemeActivityZhiGou.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/ThemeActivityZhiGou.xml"));
				if (null == xelement)
				{
					return false;
				}
				this.ActivityType = 150;
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						ThemeZhiGouConfig themeZhiGouConfig = new ThemeZhiGouConfig();
						themeZhiGouConfig.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						themeZhiGouConfig.ZhiGouID = (int)Global.GetSafeAttributeLong(xelement2, "ZhiGouID");
						themeZhiGouConfig.SinglePurchase = (int)Global.GetSafeAttributeLong(xelement2, "SinglePurchase");
						int[] safeAttributeIntArray = Global.GetSafeAttributeIntArray(xelement2, "Day", -1, ',');
						if (safeAttributeIntArray.Length == 2)
						{
							themeZhiGouConfig.FromDate = Global.GetKaiFuTime().AddDays((double)(safeAttributeIntArray[0] - 1));
							themeZhiGouConfig.ToDate = Global.GetKaiFuTime().AddDays((double)safeAttributeIntArray[1]);
						}
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsOne");
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "GoodsTwo");
						this.ThemeZhiGouConfigData[themeZhiGouConfig.ID] = themeZhiGouConfig;
						UserMoneyMgr.getInstance().CheckChargeItemConfigLogic(themeZhiGouConfig.ZhiGouID, themeZhiGouConfig.SinglePurchase, safeAttributeStr, safeAttributeStr2, string.Format("主题服直购 ID={0}", themeZhiGouConfig.ID));
					}
				}
				base.PredealDateTime();
				GlobalEventSource.getInstance().registerListener(36, this);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/OneDollarBuy.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 36)
			{
				ChargeItemBaseEventObject chargeItemBaseEventObject = eventObject as ChargeItemBaseEventObject;
				if (this.CheckValidChargeItem(chargeItemBaseEventObject.ChargeItemConfig.ChargeItemID))
				{
					Dictionary<int, int> cmdData = this.BuildThemeZhiGouInfoForClient(chargeItemBaseEventObject.Player);
					chargeItemBaseEventObject.Player.sendCmd<Dictionary<int, int>>(906, cmdData, false);
					if (chargeItemBaseEventObject.Player._IconStateMgr.CheckThemeZhiGou(chargeItemBaseEventObject.Player))
					{
						chargeItemBaseEventObject.Player._IconStateMgr.SendIconStateToClient(chargeItemBaseEventObject.Player);
					}
				}
			}
		}

		public Dictionary<int, int> BuildThemeZhiGouInfoForClient(GameClient client)
		{
			DateTime t = TimeUtil.NowDateTime();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (ThemeZhiGouConfig themeZhiGouConfig in this.ThemeZhiGouConfigData.Values)
			{
				if (!(t < themeZhiGouConfig.FromDate) && !(t > themeZhiGouConfig.ToDate))
				{
					dictionary[themeZhiGouConfig.ID] = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, themeZhiGouConfig.ZhiGouID);
				}
			}
			return dictionary;
		}

		public bool CheckValidChargeItem(int zhigouID)
		{
			DateTime t = TimeUtil.NowDateTime();
			foreach (ThemeZhiGouConfig themeZhiGouConfig in this.ThemeZhiGouConfigData.Values)
			{
				if (!(t < themeZhiGouConfig.FromDate.AddDays(-1.0)) && !(t > themeZhiGouConfig.ToDate))
				{
					if (themeZhiGouConfig.ZhiGouID == zhigouID)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool CheckClientCanBuy(GameClient client)
		{
			DateTime t = TimeUtil.NowDateTime();
			foreach (ThemeZhiGouConfig themeZhiGouConfig in this.ThemeZhiGouConfigData.Values)
			{
				if (!(t < themeZhiGouConfig.FromDate) && !(t > themeZhiGouConfig.ToDate))
				{
					int chargeItemPurchaseNum = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, themeZhiGouConfig.ZhiGouID);
					if (themeZhiGouConfig.SinglePurchase <= 0 || chargeItemPurchaseNum < themeZhiGouConfig.SinglePurchase)
					{
						return true;
					}
				}
			}
			return false;
		}

		protected const string ThemeActivityZhiGouData_fileName = "Config/ThemeActivityZhiGou.xml";

		protected Dictionary<int, ThemeZhiGouConfig> ThemeZhiGouConfigData = new Dictionary<int, ThemeZhiGouConfig>();
	}
}
