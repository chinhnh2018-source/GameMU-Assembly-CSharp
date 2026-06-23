using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class MUJieriPartFanbei : UserControl
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
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemCollection = this.activityList.ItemsSource;
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
		List<XElement> xelementList3 = Global.GetXElementList(xelement, "JieRiDuoBei");
		for (int i = 0; i < xelementList3.Count; i++)
		{
			XElement xelement4 = xelementList3[i];
			if (xelement4 == null)
			{
				return;
			}
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement4, "ID");
			string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement4, "Name");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement4, "TypeID");
			int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement4, "Effective");
			string xelementAttributeStr7 = Global.GetXElementAttributeStr(xelement4, "AwardStartDate");
			string xelementAttributeStr8 = Global.GetXElementAttributeStr(xelement4, "AwardEndDate");
			int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement4, "Link");
			MUJieriPartFanbeiItem mujieriPartFanbeiItem = U3DUtils.NEW<MUJieriPartFanbeiItem>();
			this.ItemCollection.Add(mujieriPartFanbeiItem);
			mujieriPartFanbeiItem.LabName = xelementAttributeStr6;
			mujieriPartFanbeiItem.LabTime = string.Format(Global.GetLang("{0}至{1}"), xelementAttributeStr7, xelementAttributeStr8);
			mujieriPartFanbeiItem.Link = xelementAttributeInt5;
			mujieriPartFanbeiItem.Effective = xelementAttributeInt4;
			mujieriPartFanbeiItem.TypeID = xelementAttributeInt3;
		}
	}

	public TextBlock huodongStartime;

	public TextBlock huodongEndtime;

	public TextBlock lingquStarttime;

	public TextBlock lingquEndtime;

	public ListBox activityList;

	public TextBlock descText;

	private string startTimeStr;

	private string endTimeStr;

	private string awardStartStr;

	private string awardEndStr;

	private ObservableCollection _ItemCollection;

	private string thisXmlName = string.Empty;
}
