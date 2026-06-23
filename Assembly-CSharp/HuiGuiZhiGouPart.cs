using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;
using XMLCreater;

public class HuiGuiZhiGouPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitle.text = Global.GetLang("每日直购");
		this.lblTime.text = Global.GetLang("活动时间：");
		this.lblContent.text = Global.GetLang("活动描述：活动期间，每天会有丰富多彩的直购等着你");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(this, null);
			}
		};
		this.LoadChongZhiItemConf();
		MUHuiGuiHuoDong selfHuoDongLevel = HuiGuiData.GetSelfHuoDongLevel();
		this.lblTime.text = Global.GetLang("活动时间：") + selfHuoDongLevel.BeginTime + " - " + selfHuoDongLevel.FinishTime;
		this.LoadZhiGouItems();
		this.SendHuiGuiZhiGouInfo();
	}

	private void LoadZhiGouItems()
	{
		this.m_lstItems.Clear();
		MUHuiGuiHuoDong selfHuoDongLevel = HuiGuiData.GetSelfHuoDongLevel();
		List<MUHuiGuiDayZhiGou> zhiGouByLevelAndDay = IConfigbase<ConfigHuiGuiHuoDong>.Instance.GetZhiGouByLevelAndDay(selfHuoDongLevel.HuoDongLevel, HuiGuiData.GetCurrentHuiGuiDay(), HuiGuiData.GetSelfChongZhiNum());
		if (zhiGouByLevelAndDay == null)
		{
			return;
		}
		for (int i = 0; i < zhiGouByLevelAndDay.Count; i++)
		{
			HuiGuiZhiGouPartItem huiGuiZhiGouPartItem = U3DUtils.NEW<HuiGuiZhiGouPartItem>();
			huiGuiZhiGouPartItem.transform.SetParent(this.itemContainer);
			huiGuiZhiGouPartItem.transform.localScale = Vector3.one;
			huiGuiZhiGouPartItem.transform.localPosition = new Vector3(0f, 0f - 73f * (float)i, 0f);
			huiGuiZhiGouPartItem.ZhiGouInfo = zhiGouByLevelAndDay[i];
			huiGuiZhiGouPartItem.OnSelectItem = new Action<HuiGuiZhiGouPartItem>(this.OnClickItem);
			if (this.chongzhiInfoDict.ContainsKey(zhiGouByLevelAndDay[i].ChongZhiId.ToString()))
			{
				huiGuiZhiGouPartItem.SetPrice(this.chongzhiInfoDict[zhiGouByLevelAndDay[i].ChongZhiId.ToString()].money);
			}
			this.m_lstItems.Add(huiGuiZhiGouPartItem);
		}
	}

	private void ResetItemBuyNum(HuiGuiZhiGouPartItem item, Dictionary<int, int> data)
	{
		int buyedNum = 0;
		if (data != null)
		{
			data.TryGetValue(item.ZhiGouInfo.ID, ref buyedNum);
		}
		item.SetBuyedNum(buyedNum);
	}

	public void OnClickItem(HuiGuiZhiGouPartItem item)
	{
		if (item.State == HuiGuiZhiGouState.CanBuy)
		{
			if (this.chongzhiInfoDict.ContainsKey(item.ZhiGouInfo.ChongZhiId.ToString()))
			{
				string productId = this.chongzhiInfoDict[item.ZhiGouInfo.ChongZhiId.ToString()].productId;
				int money = this.chongzhiInfoDict[item.ZhiGouInfo.ChongZhiId.ToString()].money.SafeToInt32(0);
				this.ChongZhi(money, productId, item.ZhiGouInfo.ZhiGouId);
			}
		}
		else if (item.State == HuiGuiZhiGouState.LimitBuy)
		{
		}
	}

	private void ChongZhi(int money, string productId = "", int zhiZhouId = 0)
	{
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"越南东南亚英文测试用，回归直购相关数据：Money=",
				money,
				";productId=",
				productId,
				";ZhiGouId=",
				zhiZhouId
			})
		});
		MUDebug.Log<string>(new string[]
		{
			"YN_Android回归直购里的充值"
		});
		PlatSDKMgr.Pay(8, "1", zhiZhouId);
	}

	public void LoadChongZhiItemConf()
	{
		string text = string.Empty;
		string rechargeItemCfgTypeByPlatform = Global.GetRechargeItemCfgTypeByPlatform();
		XElement gameResXml = Global.GetGameResXml("Config/MU_ChongZhi.xml");
		List<XElement> list = new List<XElement>();
		foreach (XElement xelement in gameResXml.Elements())
		{
			if (xelement.Attribute("TypeID").Value.ToString() == rechargeItemCfgTypeByPlatform)
			{
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					list.Add(xelement2);
				}
				break;
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			text = Global.GetXElementAttributeStr(list[i], "ID");
			ChongzhiInfo chongzhiInfo;
			if (this.chongzhiInfoDict.ContainsKey(text))
			{
				chongzhiInfo = this.chongzhiInfoDict[text];
			}
			else
			{
				chongzhiInfo = new ChongzhiInfo();
			}
			chongzhiInfo.Icon = Global.GetXElementAttributeStr(list[i], "Icon");
			chongzhiInfo.money = Global.GetXElementAttributeStr(list[i], "RMB");
			chongzhiInfo.zuanshiCount = Global.GetXElementAttributeStr(list[i], "ZuanShi");
			chongzhiInfo.freeDiamond = Global.GetXElementAttributeStr(list[i], "FirstBindZuanShi");
			chongzhiInfo.productId = string.Empty + text;
			if (text == "10000")
			{
				chongzhiInfo.Type = ChongzhiInfo.ChongZhiType.YueKa;
			}
			else
			{
				chongzhiInfo.Type = ChongzhiInfo.ChongZhiType.Normal;
			}
			chongzhiInfo.productId = Global.GetXElementAttributeStr(list[i], "productIdAn");
			this.chongzhiInfoDict.Add(text, chongzhiInfo);
		}
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<Dictionary<int, int>>("CMD_SPR_REGRESSACTIVE_ZHIGOU_QUERY", new Action<Dictionary<int, int>>(this.ServeGetHuiGuiZhiGouInfo));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<Dictionary<int, int>>("CMD_SPR_REGRESSACTIVE_ZHIGOU_QUERY", new Action<Dictionary<int, int>>(this.ServeGetHuiGuiZhiGouInfo));
	}

	private void SendHuiGuiZhiGouInfo()
	{
		GameInstance.Game.SendGetHuiGuiZhiGouInfo(HuiGuiData.GetSelfHuoDongLevel().HuoDongLevel);
	}

	private void ServeGetHuiGuiZhiGouInfo(Dictionary<int, int> data)
	{
		for (int i = 0; i < this.m_lstItems.Count; i++)
		{
			this.ResetItemBuyNum(this.m_lstItems[i], data);
		}
	}

	private const float CellHeight = 73f;

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel lblTitle;

	public UILabel lblTime;

	public UILabel lblContent;

	public GButton btnClose;

	public Transform itemContainer;

	private List<HuiGuiZhiGouPartItem> m_lstItems = new List<HuiGuiZhiGouPartItem>();

	private Dictionary<string, ChongzhiInfo> chongzhiInfoDict = new Dictionary<string, ChongzhiInfo>();
}
