using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;

public class UpLevelAwardPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (null != this.m_ListGiftItem)
		{
			this.m_ListGiftItemObC = this.m_ListGiftItem.ItemsSource;
		}
		this.LoadConfigFromXML();
	}

	private void LoadConfigFromXML()
	{
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/UpLevelGift.xml");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Item");
		this.LoadJiangLiInfo(xelementList);
	}

	public void QueryServerInfo()
	{
		GameInstance.Game.SpriteQueryUpLevelGiftFlagList();
	}

	public void InitDataFromServerInfo(List<int> activeData)
	{
		int count = this.CurrentItems.Count;
		int num = Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level;
		for (int i = 0; i < count; i++)
		{
			UpLevelAwardPartGiftItem upLevelAwardPartGiftItem = this.CurrentItems[i];
			if (num >= upLevelAwardPartGiftItem.TiaoJianValue)
			{
				int bitValue = Global.GetBitValue(activeData, upLevelAwardPartGiftItem.ID * 2 + 1);
				if (bitValue == 0)
				{
					upLevelAwardPartGiftItem.GiftGainState = UpLevelAwardGiftGainState.CanGain;
				}
				else if (bitValue == 1)
				{
					upLevelAwardPartGiftItem.GiftGainState = UpLevelAwardGiftGainState.Gained;
					upLevelAwardPartGiftItem.m_TiaoJian.gameObject.SetActive(false);
				}
			}
			else
			{
				upLevelAwardPartGiftItem.GiftGainState = UpLevelAwardGiftGainState.CanNotGain;
			}
		}
	}

	public void RefreshDataFromGainedInfo(int ID)
	{
		int count = this.CurrentItems.Count;
		for (int i = 0; i < count; i++)
		{
			UpLevelAwardPartGiftItem upLevelAwardPartGiftItem = this.CurrentItems[i];
			if (upLevelAwardPartGiftItem.ID == ID)
			{
				upLevelAwardPartGiftItem.GiftGainState = UpLevelAwardGiftGainState.Gained;
				upLevelAwardPartGiftItem.m_TiaoJian.gameObject.SetActive(false);
				return;
			}
		}
	}

	private void InitDataFromConfig(UpLevelAwardPartGiftItem item, XElement args)
	{
		int xelementAttributeInt = Global.GetXElementAttributeInt(args, "ToZhuanSheng");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(args, "ToLevel");
		item.TiaoJian = string.Format(Global.GetLang("需要{0}转{1}级"), xelementAttributeInt, xelementAttributeInt2);
		item.TiaoJianValue = xelementAttributeInt * 100 + xelementAttributeInt2;
	}

	private void LoadJiangLiInfo(List<XElement> xml)
	{
		this.m_ListGiftItemObC.Clear();
		this.CurrentItems.Clear();
		if (xml == null)
		{
			return;
		}
		for (int i = 0; i < xml.Count; i++)
		{
			XElement xelement = xml[i];
			if (Global.GetXElementAttributeInt(xelement, "Occupation") == Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation))
			{
				UpLevelAwardPartGiftItem upLevelAwardPartGiftItem = U3DUtils.NEW<UpLevelAwardPartGiftItem>();
				upLevelAwardPartGiftItem.ID = Global.GetXElementAttributeInt(xelement, "ID");
				this.InitDataFromConfig(upLevelAwardPartGiftItem, xelement);
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
				upLevelAwardPartGiftItem.GoodsList = xelementAttributeStr;
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MoJing");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "BindMoney");
				upLevelAwardPartGiftItem.LoadGoodsList(xelementAttributeStr, xelementAttributeInt, xelementAttributeInt2);
				this.m_ListGiftItemObC.Add(upLevelAwardPartGiftItem);
				this.CurrentItems.Add(upLevelAwardPartGiftItem);
			}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox m_ListGiftItem = new ListBox();

	private ObservableCollection m_ListGiftItemObC;

	private List<UpLevelAwardPartGiftItem> CurrentItems = new List<UpLevelAwardPartGiftItem>();
}
