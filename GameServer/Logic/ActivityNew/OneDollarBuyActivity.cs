using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.UserMoneyCharge;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class OneDollarBuyActivity : Activity, IEventListener
	{
		public void Dispose()
		{
			GlobalEventSource.getInstance().removeListener(36, this);
		}

		public bool Init()
		{
			try
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("OneDollarBuyOpen");
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
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/OneDollarBuy.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/OneDollarBuy.xml"));
				if (null == xelement)
				{
					return false;
				}
				XElement xelement2 = xelement.Element("OneDollarBuy");
				if (null != xelement2)
				{
					this.FromDate = Global.GetSafeAttributeStr(xelement2, "BeginTime");
					this.ToDate = Global.GetSafeAttributeStr(xelement2, "FinishTime");
					this.ActivityType = 45;
					this.AwardStartDate = this.FromDate;
					this.AwardEndDate = this.ToDate;
					this.OneDollarBuyConfigData.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
					DateTime.TryParse(this.FromDate, out this.OneDollarBuyConfigData.FromDate);
					DateTime.TryParse(this.ToDate, out this.OneDollarBuyConfigData.ToDate);
					this.OneDollarBuyConfigData.ZhiGouID = (int)Global.GetSafeAttributeLong(xelement2, "ZhiGouID");
					this.OneDollarBuyConfigData.SinglePurchase = (int)Global.GetSafeAttributeLong(xelement2, "SinglePurchase");
					string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsID1");
					string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "GoodsID2");
					UserMoneyMgr.getInstance().CheckChargeItemConfigLogic(this.OneDollarBuyConfigData.ZhiGouID, this.OneDollarBuyConfigData.SinglePurchase, safeAttributeStr, safeAttributeStr2, string.Format("1元直购 ID={0}", this.OneDollarBuyConfigData.ID));
				}
				base.PredealDateTime();
				if (!this.InActivityTime())
				{
					GameManager.ClientMgr.NotifyAllActivityState(8, 0, "", "", 0);
				}
				else
				{
					GameManager.ClientMgr.NotifyAllActivityState(8, this.PlatformOpenStateVavle, "", "", 0);
				}
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
				if (this.OneDollarBuyConfigData.ZhiGouID == chargeItemBaseEventObject.ChargeItemConfig.ChargeItemID)
				{
					string cmdData = this.BuildOneDollarBuyActInfoForClient(chargeItemBaseEventObject.Player);
					chargeItemBaseEventObject.Player.sendCmd(1621, cmdData, false);
				}
			}
		}

		public void OnRoleLogin(GameClient client)
		{
			if (!this.InActivityTime())
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					8,
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
					8,
					this.PlatformOpenStateVavle,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
		}

		public string BuildOneDollarBuyActInfoForClient(GameClient client)
		{
			int chargeItemPurchaseNum = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, this.OneDollarBuyConfigData.ZhiGouID);
			return string.Format("{0}:{1}", this.OneDollarBuyConfigData.ID, chargeItemPurchaseNum);
		}

		public bool CheckClientCanBuy(GameClient client)
		{
			bool result;
			if (0 == this.PlatformOpenStateVavle)
			{
				result = false;
			}
			else
			{
				int chargeItemPurchaseNum = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, this.OneDollarBuyConfigData.ZhiGouID);
				result = (chargeItemPurchaseNum < this.OneDollarBuyConfigData.SinglePurchase);
			}
			return result;
		}

		protected const string OneDollarBuyActivityData_fileName = "Config/OneDollarBuy.xml";

		protected OneDollarBuyConfig OneDollarBuyConfigData = new OneDollarBuyConfig();

		protected int PlatformOpenStateVavle = 0;
	}
}
