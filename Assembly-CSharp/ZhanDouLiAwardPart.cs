using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;

public class ZhanDouLiAwardPart : UserControl
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
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/ComatEffectivenessGift.xml");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Item");
		this.LoadJiangLiInfo(xelementList);
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
			ZhanDouLiAwardPartGiftItem zhanDouLiAwardPartGiftItem = U3DUtils.NEW<ZhanDouLiAwardPartGiftItem>();
			zhanDouLiAwardPartGiftItem.ID = Global.GetXElementAttributeInt(xelement, "ID");
			zhanDouLiAwardPartGiftItem.ZhanDouLi = Global.GetXElementAttributeInt(xelement, "ComatEffectiveness");
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsOne");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "GoodsTwo");
			string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "GoodsThr");
			zhanDouLiAwardPartGiftItem.ParseGoodsList(xelementAttributeStr, xelementAttributeStr2, xelementAttributeStr3);
			this.m_ListGiftItemObC.Add(zhanDouLiAwardPartGiftItem);
			this.CurrentItems.Add(zhanDouLiAwardPartGiftItem);
		}
	}

	public void SendQueryAwardRequest()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.SpriteQueryZhanDouLiGiftFlagList();
	}

	public void RefreshAllAwardItemsState(string activeData)
	{
		int count = this.CurrentItems.Count;
		string[] array = activeData.Split(new char[]
		{
			'_'
		});
		if (array.Length != count)
		{
			Super.HintMainText(Global.GetLang("服务器参数有误！"), 10, 3);
			MUDebug.Log<string>(new string[]
			{
				"服务器返回的领奖状态数量和本地加载XML中的数量不相等！"
			});
			return;
		}
		for (int i = 0; i < count; i++)
		{
			ZhanDouLiAwardPartGiftItem zhanDouLiAwardPartGiftItem = this.CurrentItems[i];
			int num = Convert.ToInt32(array[i]);
			switch (num)
			{
			case 0:
				zhanDouLiAwardPartGiftItem.GiftGainState = ZhanDouLiAwardGiftGainState.CanNotGain;
				break;
			case 1:
				zhanDouLiAwardPartGiftItem.GiftGainState = ZhanDouLiAwardGiftGainState.CanGain;
				break;
			case 2:
				zhanDouLiAwardPartGiftItem.GiftGainState = ZhanDouLiAwardGiftGainState.Gained;
				zhanDouLiAwardPartGiftItem.m_TiaoJian.gameObject.SetActive(false);
				break;
			default:
				Super.HintMainText(Global.GetLang("未处理的消息码result：") + num, 10, 3);
				break;
			}
		}
	}

	public void RefreshSingleAwardItemStateAfterGetAward(int ID)
	{
		int count = this.CurrentItems.Count;
		for (int i = 0; i < count; i++)
		{
			ZhanDouLiAwardPartGiftItem zhanDouLiAwardPartGiftItem = this.CurrentItems[i];
			if (zhanDouLiAwardPartGiftItem.ID == ID)
			{
				zhanDouLiAwardPartGiftItem.GiftGainState = ZhanDouLiAwardGiftGainState.Gained;
				zhanDouLiAwardPartGiftItem.m_TiaoJian.gameObject.SetActive(false);
				return;
			}
		}
	}

	public ListBox m_ListGiftItem = new ListBox();

	private ObservableCollection m_ListGiftItemObC;

	private List<ZhanDouLiAwardPartGiftItem> CurrentItems = new List<ZhanDouLiAwardPartGiftItem>();
}
