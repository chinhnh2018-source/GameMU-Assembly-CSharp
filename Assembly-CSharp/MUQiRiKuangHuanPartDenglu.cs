using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class MUQiRiKuangHuanPartDenglu : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	public string ThisXmlName
	{
		get
		{
			return this.thisXmlName;
		}
		set
		{
			this.thisXmlName = value;
			this.InitData(this.ThisXmlName);
			GameInstance.Game.DemandQiRiKuangHuanInfo(1);
		}
	}

	private void InitTextInPrefabs()
	{
		this.Bak.URL = "NetImages/GameRes/Images/QirihuodongPicture/qiridenglu.jpg";
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.rewardList.ItemsSource;
	}

	private void InitData(string strXML)
	{
		string xmlName = "Config/" + strXML;
		XElement gameResXml = Global.GetGameResXml(xmlName);
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "GiftList");
		XElement xelement = xelementList[0];
		if (xelement == null)
		{
			return;
		}
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Description");
		this.descText.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动内容: "),
			"ffffff",
			xelementAttributeStr
		});
		List<XElement> xelementList2 = Global.GetXElementList(gameResXml, "Award");
		for (int i = 0; i < xelementList2.Count; i++)
		{
			XElement xelement2 = xelementList2[i];
			if (xelement2 == null)
			{
				return;
			}
			MUQiRiKuangHuanDengluListItem muqiRiKuangHuanDengluListItem = U3DUtils.NEW<MUQiRiKuangHuanDengluListItem>();
			this.ItemCollection.AddNoUpdate(muqiRiKuangHuanDengluListItem);
			this.CurrentItems.Add(muqiRiKuangHuanDengluListItem);
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "ID");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "GoodsOne");
			string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "GoodsTwo");
			string goodsIDs = string.Empty;
			if (!string.IsNullOrEmpty(xelementAttributeStr3))
			{
				goodsIDs = xelementAttributeStr2 + "@" + xelementAttributeStr3;
			}
			else
			{
				goodsIDs = xelementAttributeStr2;
			}
			muqiRiKuangHuanDengluListItem.daysLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"CC7432",
				string.Format(Global.GetLang("第{0}天"), xelementAttributeInt)
			});
			muqiRiKuangHuanDengluListItem.daysLabelOver.text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				string.Format(Global.GetLang("第{0}天"), xelementAttributeInt)
			});
			muqiRiKuangHuanDengluListItem.GoodsIDs = goodsIDs;
			muqiRiKuangHuanDengluListItem.DayMark = (DayMark)xelementAttributeInt;
			UIPanel component = muqiRiKuangHuanDengluListItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	public void InitDengluData(SevenDayActQueryData sevendayactquerydata)
	{
		SevenDayItemData sevenDayItemData = null;
		int count = this.CurrentItems.Count;
		int qiRiKuanghuanDaysNum = Global.GetQiRiKuanghuanDaysNum();
		for (int i = 0; i < count; i++)
		{
			MUQiRiKuangHuanDengluListItem muqiRiKuangHuanDengluListItem = this.CurrentItems[i];
			if (sevendayactquerydata.ItemDict.TryGetValue(i + 1, ref sevenDayItemData))
			{
				muqiRiKuangHuanDengluListItem.AwardGiftGainState = ((sevenDayItemData.AwardFlag != 1) ? JieriAwardGiftGainState.CanGain : JieriAwardGiftGainState.Gained);
			}
			else
			{
				muqiRiKuangHuanDengluListItem.AwardGiftGainState = ((i >= qiRiKuanghuanDaysNum) ? JieriAwardGiftGainState.NotNeedGain : JieriAwardGiftGainState.CanNotGain);
			}
		}
	}

	public void setCompletedInfo(int position)
	{
		MUQiRiKuangHuanDengluListItem muqiRiKuangHuanDengluListItem = this.CurrentItems[position - 1];
		muqiRiKuangHuanDengluListItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
	}

	public ShowNetImage Bak;

	public TextBlock descText;

	public ListBox rewardList;

	private List<MUQiRiKuangHuanDengluListItem> CurrentItems = new List<MUQiRiKuangHuanDengluListItem>();

	private ObservableCollection _ItemCollection;

	private string thisXmlName = string.Empty;
}
