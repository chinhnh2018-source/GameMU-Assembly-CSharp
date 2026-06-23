using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using Tmsk.Xml;
using UnityEngine;

public class MUJieriPartDuihuan : UserControl
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
			GameInstance.Game.SpriteGetZaJinDanJiFen();
			GameInstance.Game.SpriteQueryZiKa(this.Type);
		}
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

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemCollection = this.goodlist.ItemsSource;
		this.labMojing.text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan).ToString();
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
		List<XElement> xelementList2 = Global.GetXElementList(xelement, "GiftList");
		XElement xelement3 = xelementList2[0];
		if (xelement3 == null)
		{
			return;
		}
		string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement3, "Description");
		List<XElement> xelementList3 = Global.GetXElementList(xelement, "Award");
		for (int i = 0; i < xelementList3.Count; i++)
		{
			XElement xelement4 = xelementList3[i];
			if (xelement4 == null)
			{
				return;
			}
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement4, "Type");
			if (xelementAttributeInt2 == 12)
			{
				MUJieriPartDuihuanItem mujieriPartDuihuanItem = U3DUtils.NEW<MUJieriPartDuihuanItem>();
				this.ItemCollection.Add(mujieriPartDuihuanItem);
				this.listItem.Add(mujieriPartDuihuanItem);
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement4, "ID");
				mujieriPartDuihuanItem.GoodsIDNew = Global.GetXElementAttributeStr(xelement4, "NewGoodsID");
				mujieriPartDuihuanItem.ItemID = xelementAttributeInt3;
				mujieriPartDuihuanItem.Times = Global.GetXElementAttributeInt(xelement4, "DayMaxTimes");
				int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement4, "MoJing");
				int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement4, "JiFen");
				int jingling = 0;
				mujieriPartDuihuanItem.setPicfromInt(xelementAttributeInt4, xelementAttributeInt5, jingling);
				if (!this.dicItem.ContainsKey(xelementAttributeInt3))
				{
					this.dicItem.Add(xelementAttributeInt3, mujieriPartDuihuanItem);
				}
				UIPanel component = mujieriPartDuihuanItem.GetComponent<UIPanel>();
				if (component)
				{
					Object.Destroy(component);
				}
			}
		}
	}

	public void UpdateJifenZhi(int jifen)
	{
		this.labQifu.Text = jifen.ToString();
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
				MUJieriPartDuihuanItem mujieriPartDuihuanItem = this.dicItem[int.Parse(array2[0])];
				mujieriPartDuihuanItem.Times = int.Parse(array2[1]);
			}
		}
	}

	public void setCompletedInfo(int result, int index, int leftNum)
	{
		if (result < 0)
		{
			return;
		}
		MUJieriPartDuihuanItem mujieriPartDuihuanItem = this.dicItem[index];
		mujieriPartDuihuanItem.Times = leftNum;
		GameInstance.Game.SpriteGetZaJinDanJiFen();
		this.labMojing.text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan).ToString();
	}

	public TextBlock labMojing;

	public TextBlock labQifu;

	public TextBlock labJingling;

	public ListBox goodlist;

	private string startTimeStr;

	private string endTimeStr;

	private string awardStartStr;

	private string awardEndStr;

	private List<MUJieriPartDuihuanItem> listItem = new List<MUJieriPartDuihuanItem>();

	private Dictionary<int, MUJieriPartDuihuanItem> dicItem = new Dictionary<int, MUJieriPartDuihuanItem>();

	private ObservableCollection _ItemCollection;

	private string thisXmlName = string.Empty;

	private int type;
}
