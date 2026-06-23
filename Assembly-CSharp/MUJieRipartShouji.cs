using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using Tmsk.Xml;
using UnityEngine;

public class MUJieRipartShouji : UserControl
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
			this.isInTime = Global.InLimitTimeRange(this.awardStartStr, this.awardEndStr);
			GameInstance.Game.SpriteQueryZiKa(this.Type);
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemCollection = this.goodlist.ItemsSource;
	}

	public int Type
	{
		get
		{
			return this.type;
		}
		set
		{
			this.type = value;
		}
	}

	private void InitData(string strXML)
	{
		this.ItemCollection.Clear();
		XElement xelement = XElement.Parse(this.ThisXmlName);
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
		string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement3, "Description2");
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
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement4, "Type");
			if (xelementAttributeInt2 == 2)
			{
				MUJieRipartShoujiItem mujieRipartShoujiItem = U3DUtils.NEW<MUJieRipartShoujiItem>();
				this.ItemCollection.Add(mujieRipartShoujiItem);
				this.listItem.Add(mujieRipartShoujiItem);
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement4, "ID");
				mujieRipartShoujiItem.GoodsIDs = Global.GetXElementAttributeStr(xelement4, "DuiHuanGoodsIDs");
				mujieRipartShoujiItem.ItemID = xelementAttributeInt3;
				mujieRipartShoujiItem.GoodsIDNew = Global.GetXElementAttributeStr(xelement4, "NewGoodsID");
				int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement4, "DayMaxTimes");
				mujieRipartShoujiItem.TotalNum = xelementAttributeInt4;
				this.totalNum = xelementAttributeInt4;
				mujieRipartShoujiItem.Name = string.Format("item{0}", i);
				if (!this.dicItem.ContainsKey(xelementAttributeInt3))
				{
					this.dicItem.Add(xelementAttributeInt3, mujieRipartShoujiItem);
				}
				UIPanel component = mujieRipartShoujiItem.GetComponent<UIPanel>();
				if (component)
				{
					Object.Destroy(component);
				}
			}
		}
	}

	public void setExchangeNum(string result)
	{
		if (result == null)
		{
			return;
		}
		string[] array = result.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 2)
			{
				MUJieRipartShoujiItem mujieRipartShoujiItem = this.listItem[int.Parse(array2[0]) - 1];
				mujieRipartShoujiItem.TotalNum = int.Parse(array2[1]);
			}
		}
	}

	public void setCompletedInfo(int result, int index, int leftNum)
	{
		if (result < 0)
		{
			return;
		}
		MUJieRipartShoujiItem mujieRipartShoujiItem = this.dicItem[index];
		mujieRipartShoujiItem.TotalNum = leftNum;
		for (int i = 0; i < this.listItem.Count; i++)
		{
			this.listItem[i].RefreshGoodsNum();
		}
	}

	public TextBlock huodongStartime;

	public TextBlock huodongEndtime;

	public TextBlock lingquStarttime;

	public TextBlock lingquEndtime;

	public ListBox goodlist;

	public TextBlock descText;

	private string startTimeStr;

	private string endTimeStr;

	private string awardStartStr;

	private string awardEndStr;

	private bool isInTime = true;

	private List<MUJieRipartShoujiItem> listItem = new List<MUJieRipartShoujiItem>();

	private Dictionary<int, MUJieRipartShoujiItem> dicItem = new Dictionary<int, MUJieRipartShoujiItem>();

	private int totalNum;

	private ObservableCollection _ItemCollection;

	private string thisXmlName = string.Empty;

	private int type;
}
