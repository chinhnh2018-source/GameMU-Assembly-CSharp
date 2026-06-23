using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class MUJieriLianxuChongzhiPart : UserControl
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

	public void InitData(string strXML)
	{
		this.ItemCollection.Clear();
		this.xml = XElement.Parse(strXML);
		if (this.xml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(this.xml, "Activities");
		XElement xelement = xelementList[0];
		if (xelement == null)
		{
			return;
		}
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "FromDate");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "ToDate");
		string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "AwardStartDate");
		string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement, "AwardEndDate");
		this.startTimeStr = xelementAttributeStr;
		this.endTimeStr = xelementAttributeStr2;
		this.awardStartStr = xelementAttributeStr3;
		this.awardEndStr = xelementAttributeStr4;
		this.huodongStartime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动时间: "),
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
		this.descList = Global.GetXElementList(this.xml, "GiftList");
		XElement xelement2 = this.descList[0];
		if (xelement2 == null)
		{
			return;
		}
		string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement2, "Description");
		this.descText.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动内容: "),
			"ffffff",
			xelementAttributeStr5
		});
		List<XElement> xelementList2 = Global.GetXElementList(this.xml, "LianXu");
		for (int i = 0; i < xelementList2.Count; i++)
		{
			XElement xelement3 = xelementList2[i];
			if (xelement3 == null)
			{
				return;
			}
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement3, "ID");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement3, "Group");
			if (!this.DicGrouptoDay.ContainsKey(xelementAttributeInt2))
			{
				this.DicGrouptoDay.Add(xelementAttributeInt2, xelementAttributeInt);
			}
		}
		this.InitItem();
	}

	private void InitItem()
	{
		if (this.xml == null)
		{
			return;
		}
		if (this.descList[0] == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(this.xml, "LianXu");
		foreach (int num in this.DicGrouptoDay.Keys)
		{
			MUJieriLianxuChongzhiItem mujieriLianxuChongzhiItem = U3DUtils.NEW<MUJieriLianxuChongzhiItem>();
			this.ItemCollection.Add(mujieriLianxuChongzhiItem);
			string zuanshiNum = string.Empty;
			this.DicDaytoGood.Clear();
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				if (xelement == null)
				{
					return;
				}
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Group");
				if (num == xelementAttributeInt)
				{
					zuanshiNum = Global.GetXElementAttributeStr(xelement, "NeedZuanShi");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Day");
					string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsOne");
					string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "GoodsTwo");
					string text = string.Empty;
					if (string.IsNullOrEmpty(xelementAttributeStr2))
					{
						text = xelementAttributeStr;
					}
					else if (!string.IsNullOrEmpty(xelementAttributeStr) && !string.IsNullOrEmpty(xelementAttributeStr2))
					{
						text = xelementAttributeStr + "@" + xelementAttributeStr2;
					}
					else if (string.IsNullOrEmpty(xelementAttributeStr))
					{
						text = xelementAttributeStr2;
					}
					if (!this.DicDaytoGood.ContainsKey(xelementAttributeInt2))
					{
						this.DicDaytoGood.Add(xelementAttributeInt2, text);
					}
				}
			}
			mujieriLianxuChongzhiItem.ZuanshiNum = zuanshiNum;
			mujieriLianxuChongzhiItem.InitItem1(this.DicDaytoGood, num);
			if (!this.DicIDtoItem.ContainsKey(num))
			{
				this.DicIDtoItem.Add(num, mujieriLianxuChongzhiItem);
			}
			UIPanel component = mujieriLianxuChongzhiItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	public void GetDataInfo()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.GetJieriChongzhiInfoCmd();
	}

	public void SetLianChongState(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		switch (e.fields[0].SafeToInt32(0))
		{
		case 0:
		{
			MUJieriLianxuChongzhiItem mujieriLianxuChongzhiItem = null;
			for (int i = 1; i < e.fields.Length; i++)
			{
				string[] array = e.fields[i].Split(new char[]
				{
					','
				});
				if (array.Length == 3)
				{
					if (this.DicIDtoItem.TryGetValue(array[0].SafeToInt32(0), ref mujieriLianxuChongzhiItem))
					{
						mujieriLianxuChongzhiItem.SetInitState(array[1].SafeToInt32(0), array[2].SafeToInt32(0));
					}
				}
			}
			break;
		}
		case 1:
			Super.HintMainText(Global.GetLang("数据异常！"), 10, 3);
			break;
		case 3:
			Super.HintMainText(Global.GetLang("不是领奖时间！"), 10, 3);
			break;
		case 4:
			Super.HintMainText(Global.GetLang("数据库出错！"), 10, 3);
			break;
		case 5:
			Super.HintMainText(Global.GetLang("远端配置出错！"), 10, 3);
			break;
		case 7:
			Super.HintMainText(Global.GetLang("不满足领奖条件！"), 10, 3);
			break;
		}
	}

	public void SetLianChongResult(int result, int id, int day)
	{
		switch (result)
		{
		case 0:
		{
			MUJieriLianxuChongzhiItem mujieriLianxuChongzhiItem = null;
			if (this.DicIDtoItem.TryGetValue(id, ref mujieriLianxuChongzhiItem))
			{
				mujieriLianxuChongzhiItem.SetLianxuChongzhiState(day);
			}
			break;
		}
		case 1:
			Super.HintMainText(Global.GetLang("领取数据异常！"), 10, 3);
			break;
		case 2:
			Super.HintMainText(Global.GetLang("活动未开启！"), 10, 3);
			break;
		case 3:
			Super.HintMainText(Global.GetLang("不是领奖时间！"), 10, 3);
			break;
		case 4:
			Super.HintMainText(Global.GetLang("数据库出错！"), 10, 3);
			break;
		case 5:
			Super.HintMainText(Global.GetLang("远端配置出错！"), 10, 3);
			break;
		case 6:
			Super.HintMainText(Global.GetLang("背包不足！"), 10, 3);
			break;
		case 7:
			Super.HintMainText(Global.GetLang("不满足领奖条件！"), 10, 3);
			break;
		}
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

	private Dictionary<int, MUJieriLianxuChongzhiItem> DicIDtoItem = new Dictionary<int, MUJieriLianxuChongzhiItem>();

	private Dictionary<int, Dictionary<int, string>> DicIDtoInfo = new Dictionary<int, Dictionary<int, string>>();

	private Dictionary<int, int> DicGrouptoDay = new Dictionary<int, int>();

	private List<XElement> descList = new List<XElement>();

	private XElement xml;

	private ObservableCollection _ItemCollection;

	private Dictionary<int, string> DicDaytoGood = new Dictionary<int, string>();
}
