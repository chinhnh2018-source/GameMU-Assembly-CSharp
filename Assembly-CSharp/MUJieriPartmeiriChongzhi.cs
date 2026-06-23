using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using Tmsk.Xml;
using UnityEngine;

public class MUJieriPartmeiriChongzhi : UserControl
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
			this.InitData(this.thisXmlName);
			GameInstance.Game.SpriteQueryJieriCZSong();
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemCollection = this.rewardList.ItemsSource;
	}

	private void InitData(string strXML)
	{
		XElement xelement = XElement.Parse(strXML);
		if (xelement == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "Activities");
		XElement xelement2 = xelementList[0];
		if (xelement2 == null)
		{
			return;
		}
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "ActivityType");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "FromDate");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "ToDate");
		string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "AwardStartDate");
		string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement2, "AwardEndDate");
		this.startTimeStr = xelementAttributeStr;
		this.endTimeStr = xelementAttributeStr2;
		this.awardStartStr = xelementAttributeStr3;
		this.awardEndStr = xelementAttributeStr4;
		this.huodongStartime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动时间："),
			"ffffff",
			this.startTimeStr
		});
		this.huodongEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("    至    "),
			"ffffff",
			this.endTimeStr
		});
		this.lingquStarttime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("领取时间:"),
			"ffffff",
			this.awardStartStr
		});
		this.lingquEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("    至    "),
			"ffffff",
			this.awardEndStr
		});
		List<XElement> xelementList2 = Global.GetXElementList(xelement, "GiftList");
		XElement xelement3 = xelementList2[0];
		if (xelement3 == null)
		{
			return;
		}
		string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement3, "Description");
		this.descText.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动内容: "),
			"ffffff",
			xelementAttributeStr5
		});
		List<XElement> xelementList3 = Global.GetXElementList(xelement, "Award");
		for (int i = 0; i < xelementList3.Count; i++)
		{
			XElement xelement4 = xelementList3[i];
			if (xelement4 == null)
			{
				return;
			}
			MUJieriPartmeiriChongzhiItem mujieriPartmeiriChongzhiItem = U3DUtils.NEW<MUJieriPartmeiriChongzhiItem>();
			this.ItemCollection.Add(mujieriPartmeiriChongzhiItem);
			this.listItem.Add(mujieriPartmeiriChongzhiItem);
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement4, "ID");
			string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement4, "GoodsOne");
			string xelementAttributeStr7 = Global.GetXElementAttributeStr(xelement4, "GoodsTwo");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement4, "MinYuanBao");
			string goodsIDs = string.Empty;
			if (!string.IsNullOrEmpty(xelementAttributeStr7))
			{
				goodsIDs = xelementAttributeStr6 + "@" + xelementAttributeStr7;
			}
			else
			{
				goodsIDs = xelementAttributeStr6;
			}
			mujieriPartmeiriChongzhiItem.GoodsIDs = goodsIDs;
			mujieriPartmeiriChongzhiItem.Id = xelementAttributeInt2;
			mujieriPartmeiriChongzhiItem.Need = xelementAttributeInt3;
			mujieriPartmeiriChongzhiItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
			UIPanel component = mujieriPartmeiriChongzhiItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	public void setBtnState(int todayMoney, int flag)
	{
		int count = this.listItem.Count;
		if (todayMoney != this.thisNum)
		{
			if (todayMoney >= this.listItem[count - 1].Need)
			{
				for (int i = 0; i < count; i++)
				{
					MUJieriPartmeiriChongzhiItem mujieriPartmeiriChongzhiItem = this.listItem[i];
					mujieriPartmeiriChongzhiItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
				}
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					MUJieriPartmeiriChongzhiItem mujieriPartmeiriChongzhiItem = this.listItem[j];
					if (todayMoney <= mujieriPartmeiriChongzhiItem.Need)
					{
						for (int k = 0; k < count; k++)
						{
							mujieriPartmeiriChongzhiItem = this.listItem[k];
							if (k < j || todayMoney == mujieriPartmeiriChongzhiItem.Need)
							{
								mujieriPartmeiriChongzhiItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
							}
							else
							{
								mujieriPartmeiriChongzhiItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
							}
						}
						break;
					}
				}
			}
		}
		if (flag != this.thisFlag)
		{
			for (int l = 0; l < count; l++)
			{
				MUJieriPartmeiriChongzhiItem mujieriPartmeiriChongzhiItem = this.listItem[l];
				int intSomeBit = Global.GetIntSomeBit(flag, l);
				if (intSomeBit == 1)
				{
					mujieriPartmeiriChongzhiItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
				}
				else if (intSomeBit == 0 && mujieriPartmeiriChongzhiItem.AwardGiftGainState == JieriAwardGiftGainState.CanGain)
				{
					mujieriPartmeiriChongzhiItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
				}
				else
				{
					mujieriPartmeiriChongzhiItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
				}
			}
		}
		this.thisNum = todayMoney;
		this.thisFlag = flag;
	}

	public void setCompletedInfo(int result, int position)
	{
		if (result < 0)
		{
			return;
		}
		MUJieriPartmeiriChongzhiItem mujieriPartmeiriChongzhiItem = this.listItem[position - 1];
		mujieriPartmeiriChongzhiItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
	}

	public TextBlock huodongStartime;

	public TextBlock huodongEndtime;

	public TextBlock lingquStarttime;

	public TextBlock lingquEndtime;

	public TextBlock descText;

	public ListBox rewardList;

	private string startTimeStr;

	private string endTimeStr;

	private string awardStartStr;

	private string awardEndStr;

	private List<MUJieriPartmeiriChongzhiItem> listItem = new List<MUJieriPartmeiriChongzhiItem>();

	private ObservableCollection _ItemCollection;

	private string thisXmlName = string.Empty;

	private int thisNum = -1;

	private int thisFlag = -1;
}
