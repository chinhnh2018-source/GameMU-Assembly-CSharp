using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class MUQiRiKuangHuanPartChongZhi : UserControl
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
			GameInstance.Game.DemandQiRiKuangHuanInfo(2);
		}
	}

	private void InitTextInPrefabs()
	{
		this.Bak.URL = "NetImages/GameRes/Images/QirihuodongPicture/qirichongzhi.jpg";
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.rewardList.ItemsSource;
	}

	private void InitData(string strXML)
	{
		this.ItemCollection.Clear();
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
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Title");
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
			MUQiRiKuangHuanPartChongZhiItem muqiRiKuangHuanPartChongZhiItem = U3DUtils.NEW<MUQiRiKuangHuanPartChongZhiItem>();
			this.ItemCollection.Add(muqiRiKuangHuanPartChongZhiItem);
			this.listItem.Add(muqiRiKuangHuanPartChongZhiItem);
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "ID");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement2, "ID");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "GoodsOne");
			string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "GoodsTwo");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement2, "MinZhuanShi");
			string goodsIDs = string.Empty;
			if (!string.IsNullOrEmpty(xelementAttributeStr3))
			{
				goodsIDs = xelementAttributeStr2 + "@" + xelementAttributeStr3;
			}
			else
			{
				goodsIDs = xelementAttributeStr2;
			}
			muqiRiKuangHuanPartChongZhiItem.daysLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"CC7432",
				string.Format(Global.GetLang("第{0}天"), xelementAttributeInt2)
			});
			muqiRiKuangHuanPartChongZhiItem.daysLabelOver.text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				string.Format(Global.GetLang("第{0}天"), xelementAttributeInt2)
			});
			muqiRiKuangHuanPartChongZhiItem.GoodsIDs = goodsIDs;
			muqiRiKuangHuanPartChongZhiItem.DayMark = (DayMark)xelementAttributeInt;
			muqiRiKuangHuanPartChongZhiItem.Need = xelementAttributeInt3;
			if (this.isInActivity)
			{
				muqiRiKuangHuanPartChongZhiItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
			}
			if (!this.isInLingqu)
			{
				muqiRiKuangHuanPartChongZhiItem.AwardGiftGainState = JieriAwardGiftGainState.OverTime;
			}
			UIPanel component = muqiRiKuangHuanPartChongZhiItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	public void InitChongZhiData(SevenDayActQueryData sevendayactquerydata)
	{
		SevenDayItemData sevenDayItemData = null;
		int count = this.listItem.Count;
		int qiRiKuanghuanDaysNum = Global.GetQiRiKuanghuanDaysNum();
		for (int i = 0; i < count; i++)
		{
			MUQiRiKuangHuanPartChongZhiItem muqiRiKuangHuanPartChongZhiItem = this.listItem[i];
			if (sevendayactquerydata.ItemDict == null)
			{
				muqiRiKuangHuanPartChongZhiItem.yichongzhi = 0;
				muqiRiKuangHuanPartChongZhiItem.AwardGiftGainState = ((i + 1 != qiRiKuanghuanDaysNum) ? ((i + 1 >= qiRiKuanghuanDaysNum) ? JieriAwardGiftGainState.NotNeedGain : JieriAwardGiftGainState.OverTime) : JieriAwardGiftGainState.CanNotGain);
			}
			else if (sevendayactquerydata.ItemDict.TryGetValue(i + 1, ref sevenDayItemData))
			{
				muqiRiKuangHuanPartChongZhiItem.yichongzhi = sevenDayItemData.Params1;
				if (sevenDayItemData.AwardFlag == 1)
				{
					muqiRiKuangHuanPartChongZhiItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
				}
				else
				{
					muqiRiKuangHuanPartChongZhiItem.AwardGiftGainState = ((sevenDayItemData.Params1 < muqiRiKuangHuanPartChongZhiItem.Need) ? JieriAwardGiftGainState.CanNotGain : JieriAwardGiftGainState.CanGain);
				}
			}
			else
			{
				muqiRiKuangHuanPartChongZhiItem.yichongzhi = 0;
				muqiRiKuangHuanPartChongZhiItem.AwardGiftGainState = ((i + 1 >= qiRiKuanghuanDaysNum) ? ((i + 1 != qiRiKuanghuanDaysNum) ? JieriAwardGiftGainState.NotNeedGain : JieriAwardGiftGainState.CanNotGain) : JieriAwardGiftGainState.OverTime);
			}
		}
	}

	public void setCompletedInfo(int position)
	{
		MUQiRiKuangHuanPartChongZhiItem muqiRiKuangHuanPartChongZhiItem = this.listItem[position - 1];
		muqiRiKuangHuanPartChongZhiItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
	}

	public ShowNetImage Bak;

	public TextBlock descText;

	public ListBox rewardList;

	private bool isInActivity;

	private bool isInLingqu;

	private List<MUQiRiKuangHuanPartChongZhiItem> listItem = new List<MUQiRiKuangHuanPartChongZhiItem>();

	private List<InputKingPaiHangData> listKingPaiHang = new List<InputKingPaiHangData>();

	private ObservableCollection _ItemCollection;

	private string thisXmlName = string.Empty;
}
