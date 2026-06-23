using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using Tmsk.Xml;
using UnityEngine;

public class MUJieriPartLeijiXiaofei : UserControl
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

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemCollection = this.rewardList.ItemsSource;
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
			GameInstance.Game.SpriteQueryLeijiXF();
		}
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
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "FromDate");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "ToDate");
		string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "AwardStartDate");
		string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement2, "AwardEndDate");
		this.startTimeStr = xelementAttributeStr;
		this.endTimeStr = xelementAttributeStr2;
		this.awardStartStr = xelementAttributeStr3;
		this.awardEndStr = xelementAttributeStr4;
		if (Global.InLimitTimeRange(this.awardStartStr, this.awardEndStr))
		{
			this.isInLingqu = true;
		}
		else
		{
			this.isInLingqu = false;
		}
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
			Global.GetLang("领取时间: "),
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
			MUJieriPartLeijiXiaofeiItem mujieriPartLeijiXiaofeiItem = U3DUtils.NEW<MUJieriPartLeijiXiaofeiItem>();
			this.ItemCollection.Add(mujieriPartLeijiXiaofeiItem);
			this.listItem.Add(mujieriPartLeijiXiaofeiItem);
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement4, "ID");
			string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement4, "GoodsOne");
			string xelementAttributeStr7 = Global.GetXElementAttributeStr(xelement4, "GoodsTwo");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement4, "MinYuanBao");
			string goodsIDs = string.Empty;
			if (!string.IsNullOrEmpty(xelementAttributeStr7))
			{
				goodsIDs = xelementAttributeStr6 + "@" + xelementAttributeStr7;
			}
			else
			{
				goodsIDs = xelementAttributeStr6;
			}
			mujieriPartLeijiXiaofeiItem.GoodsIDs = goodsIDs;
			mujieriPartLeijiXiaofeiItem.Id = xelementAttributeInt;
			mujieriPartLeijiXiaofeiItem.Need = xelementAttributeInt2;
			mujieriPartLeijiXiaofeiItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
			mujieriPartLeijiXiaofeiItem.IsInLingqu = this.isInLingqu;
			UIPanel component = mujieriPartLeijiXiaofeiItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	public void setBtnState(int totalCost, int flag)
	{
		int count = this.listItem.Count;
		if (totalCost != this.thisNum)
		{
			if (totalCost >= this.listItem[count - 1].Need)
			{
				for (int i = 0; i < count; i++)
				{
					MUJieriPartLeijiXiaofeiItem mujieriPartLeijiXiaofeiItem = this.listItem[i];
					mujieriPartLeijiXiaofeiItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
				}
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					MUJieriPartLeijiXiaofeiItem mujieriPartLeijiXiaofeiItem = this.listItem[j];
					if (totalCost <= mujieriPartLeijiXiaofeiItem.Need)
					{
						for (int k = 0; k < count; k++)
						{
							mujieriPartLeijiXiaofeiItem = this.listItem[k];
							if (k < j || totalCost == mujieriPartLeijiXiaofeiItem.Need)
							{
								mujieriPartLeijiXiaofeiItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
							}
							else
							{
								mujieriPartLeijiXiaofeiItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
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
				MUJieriPartLeijiXiaofeiItem mujieriPartLeijiXiaofeiItem = this.listItem[l];
				int intSomeBit = Global.GetIntSomeBit(flag, l);
				if (intSomeBit == 1)
				{
					mujieriPartLeijiXiaofeiItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
				}
				else if (intSomeBit == 0 && mujieriPartLeijiXiaofeiItem.AwardGiftGainState == JieriAwardGiftGainState.CanGain)
				{
					mujieriPartLeijiXiaofeiItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
				}
				else
				{
					mujieriPartLeijiXiaofeiItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
				}
			}
		}
		this.thisNum = totalCost;
		this.thisFlag = flag;
	}

	public void setCompletedInfo(int result, int position)
	{
		if (result < 0)
		{
			return;
		}
		MUJieriPartLeijiXiaofeiItem mujieriPartLeijiXiaofeiItem = this.listItem[position - 1];
		mujieriPartLeijiXiaofeiItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
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

	private bool isInLingqu;

	private List<MUJieriPartLeijiXiaofeiItem> listItem = new List<MUJieriPartLeijiXiaofeiItem>();

	private ObservableCollection _ItemCollection;

	private string thisXmlName = string.Empty;

	private int thisNum = -1;

	private int thisFlag = -1;
}
