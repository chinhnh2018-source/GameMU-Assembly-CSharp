using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using Tmsk.Xml;
using UnityEngine;

public class MUJieRiPartLeijiDenglu : UserControl
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
			GameInstance.Game.SpriteQueryJieriDengLu();
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
			MUJieriLeijiDengluListItem mujieriLeijiDengluListItem = U3DUtils.NEW<MUJieriLeijiDengluListItem>();
			this.ItemCollection.Add(mujieriLeijiDengluListItem);
			this.CurrentItems.Add(mujieriLeijiDengluListItem);
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement4, "TimeOl");
			string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement4, "GoodsOne");
			string xelementAttributeStr7 = Global.GetXElementAttributeStr(xelement4, "GoodsTwo");
			string goodsIDs = string.Empty;
			if (!string.IsNullOrEmpty(xelementAttributeStr7))
			{
				goodsIDs = xelementAttributeStr6 + "@" + xelementAttributeStr7;
			}
			else
			{
				goodsIDs = xelementAttributeStr6;
			}
			mujieriLeijiDengluListItem.daysLabel.text = "{CC7432}" + string.Format(Global.GetLang("第{0}天"), xelementAttributeInt2);
			mujieriLeijiDengluListItem.daysLabelOver.text = "{808081}" + string.Format(Global.GetLang("第{0}天"), xelementAttributeInt2);
			mujieriLeijiDengluListItem.GoodsIDs = goodsIDs;
			mujieriLeijiDengluListItem.DayMark = (DayMark)xelementAttributeInt2;
			UIPanel component = mujieriLeijiDengluListItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	public void InitLeijiDengluData(int num, int flag)
	{
		int count = this.CurrentItems.Count;
		if (num != -1)
		{
			for (int i = 0; i < num; i++)
			{
				if (i < count)
				{
					MUJieriLeijiDengluListItem mujieriLeijiDengluListItem = this.CurrentItems[i];
					mujieriLeijiDengluListItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
				}
			}
		}
		for (int j = 0; j < count; j++)
		{
			MUJieriLeijiDengluListItem mujieriLeijiDengluListItem = this.CurrentItems[j];
			int intSomeBit = Global.GetIntSomeBit(flag, j);
			if (intSomeBit == 1)
			{
				mujieriLeijiDengluListItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
			}
			else if (intSomeBit == 0 && mujieriLeijiDengluListItem.AwardGiftGainState == JieriAwardGiftGainState.CanGain)
			{
				mujieriLeijiDengluListItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
			}
			else
			{
				mujieriLeijiDengluListItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
			}
		}
	}

	public void setCompletedInfo(int result, int position)
	{
		if (result < 0)
		{
			return;
		}
		MUJieriLeijiDengluListItem mujieriLeijiDengluListItem = this.CurrentItems[position - 1];
		mujieriLeijiDengluListItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
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

	private List<MUJieriLeijiDengluListItem> CurrentItems = new List<MUJieriLeijiDengluListItem>();

	private ObservableCollection _ItemCollection;

	private string thisXmlName = string.Empty;
}
