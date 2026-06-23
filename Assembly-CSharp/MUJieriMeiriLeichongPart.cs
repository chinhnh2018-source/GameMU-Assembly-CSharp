using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class MUJieriMeiriLeichongPart : UserControl
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
		this.mInitUIPopupList();
		this.mUIPopupList.onSelectionChange = new UIPopupList.OnSelectionChange(this.mSelectChange);
		this.mUIPopupList.selection = string.Format(Global.GetLang("       第  {0}  天"), this.mGetJieriDays());
		this.mUIPopupList.textLabel.color = Color.yellow;
		this.mUIPopupList.hasHighlightLable = true;
		this.mUIPopupList.textColor = Color.white;
		this.mUIPopupList.highLightIndex = this.mGetJieriDays() - 1;
	}

	private void mInitUIPopupList()
	{
		this.DayToDay.Clear();
		this.mUIPopupList.items.Clear();
		this.mUIPopupList.textScale = 0.6f;
		for (int i = 0; i < this.mGetJieriDays(); i++)
		{
			string text = string.Format(Global.GetLang("       第  {0}  天"), i + 1);
			this.mUIPopupList.items.Add(text);
			if (!this.DayToDay.ContainsKey(text))
			{
				this.DayToDay.Add(text, i + 1);
			}
		}
	}

	private void mSelectChange(string item)
	{
		if (this.DayToDay.ContainsKey(item))
		{
			this.mInitGoodsInfo(this.DayToDay[item]);
		}
	}

	public void InitData(string strXML)
	{
		this.DayList.Clear();
		this.DayToList.Clear();
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
			Global.GetLang("领取时间："),
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
			Global.GetLang("活动内容："),
			"ffffff",
			xelementAttributeStr5
		});
		this.goodList = Global.GetXElementList(xelement, "Award");
		for (int i = 0; i < this.goodList.Count; i++)
		{
			XElement xelement4 = this.goodList[i];
			if (xelement4 != null)
			{
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement4, "Day");
				if (!this.DayList.Contains(xelementAttributeInt2))
				{
					this.DayList.Add(xelementAttributeInt2);
				}
			}
		}
		this.mGetAllItem(this.DayList);
	}

	private void mGetAllItem(List<int> dayList)
	{
		for (int i = 0; i < dayList.Count; i++)
		{
			List<MeiriChongzhiItemData> list = new List<MeiriChongzhiItemData>();
			for (int j = 0; j < this.goodList.Count; j++)
			{
				XElement xelement = this.goodList[j];
				if (xelement != null)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Day");
					if (dayList[i] == xelementAttributeInt)
					{
						int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "ID");
						string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsOne");
						string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "GoodsTwo");
						int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MinYuanBao");
						string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "GoodsThr");
						string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement, "EffectiveTime");
						string goodsID = string.Empty;
						if (!string.IsNullOrEmpty(xelementAttributeStr2))
						{
							goodsID = xelementAttributeStr + "@" + xelementAttributeStr2;
						}
						else
						{
							goodsID = xelementAttributeStr;
						}
						list.Add(new MeiriChongzhiItemData
						{
							ID = xelementAttributeInt2,
							Day = xelementAttributeInt,
							MinYuanBao = xelementAttributeInt3,
							GoodsID = goodsID,
							GoodsThr = xelementAttributeStr3,
							EffectiveTime = xelementAttributeStr4
						});
					}
				}
			}
			if (!this.DayToList.ContainsKey(dayList[i]))
			{
				this.DayToList.Add(dayList[i], list);
			}
		}
	}

	private void mInitItemData(List<MeiriChongzhiItemData> dataList, int totalMoney, int flag)
	{
		this.ItemCollection.Clear();
		for (int i = 0; i < dataList.Count; i++)
		{
			MUJieriMeiriLeichongItem mujieriMeiriLeichongItem = U3DUtils.NEW<MUJieriMeiriLeichongItem>();
			this.ItemCollection.Add(mujieriMeiriLeichongItem);
			mujieriMeiriLeichongItem.Id = i + 1;
			mujieriMeiriLeichongItem.Day = dataList[i].Day;
			mujieriMeiriLeichongItem.Need = dataList[i].MinYuanBao;
			if (totalMoney >= mujieriMeiriLeichongItem.Need)
			{
				int intSomeBit = Global.GetIntSomeBit(flag, i);
				if (intSomeBit == 1)
				{
					mujieriMeiriLeichongItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
				}
				else if (intSomeBit == 0)
				{
					mujieriMeiriLeichongItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
				}
			}
			else
			{
				mujieriMeiriLeichongItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
				if (this._thisDay < this.mGetJieriDays())
				{
					mujieriMeiriLeichongItem.AwardGiftGainState = JieriAwardGiftGainState.OverTime;
				}
			}
			Super.LoadGoodsList(dataList[i].GoodsID, mujieriMeiriLeichongItem.ItemCollection);
			Super.LoadOtherGoodsList(dataList[i].GoodsThr, mujieriMeiriLeichongItem.ItemCollection, dataList[i].EffectiveTime);
			UIPanel component = mujieriMeiriLeichongItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	public int mGetJieriDays()
	{
		int jieriDaysNum = Global.GetJieriDaysNum();
		int num = (Global.GetCorrectDateTime() - Global.GetJieriTime()).Days + 1;
		return (num > jieriDaysNum) ? jieriDaysNum : num;
	}

	public void mInitGoodsInfo(int day)
	{
		if (this.DayToList.ContainsKey(day))
		{
			GameInstance.Game.GetMeiriLeichongInfo(day);
			this._thisDay = day;
		}
	}

	public void SetBak(string bakName)
	{
		this.bak.ImageURL = string.Format("NetImages/GameRes/Images/Plate/{0}.jpg", bakName);
		this.bak.gameObject.SetActive(false);
		this.bak.gameObject.SetActive(true);
	}

	public void SetBtnState(int totalMoney, int flag)
	{
		this.mInitItemData(this.DayToList[this._thisDay], totalMoney, flag);
		Super.HideNetWaiting();
	}

	public void SetCompletedInfo(int result, int position)
	{
		if (result < 0)
		{
			return;
		}
		int index = position % 1000 - 1;
		MUJieriMeiriLeichongItem component = this.ItemCollection.GetAt(index).GetComponent<MUJieriMeiriLeichongItem>();
		if (component != null)
		{
			component.AwardGiftGainState = JieriAwardGiftGainState.Gained;
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

	public ShowNetImage bak;

	public UIPopupList mUIPopupList;

	private ObservableCollection _ItemCollection;

	private Dictionary<string, int> DayToDay = new Dictionary<string, int>();

	private List<XElement> goodList;

	private List<int> DayList = new List<int>();

	private Dictionary<int, List<MeiriChongzhiItemData>> DayToList = new Dictionary<int, List<MeiriChongzhiItemData>>();

	private int _thisDay;
}
