using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using Tmsk.Xml;

public class HefuPartLeijiDenglu : UserControl
{
	public ObservableCollection ItemCollectionDenglu
	{
		get
		{
			return this._ItemCollectionDenglu;
		}
		set
		{
			this._ItemCollectionDenglu = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.huodongStartime.Z = -0.10000000149011612;
		this.huodongEndtime.Z = -0.10000000149011612;
		this.lingquStarttime.Z = -0.10000000149011612;
		this.lingquEndtime.Z = -0.10000000149011612;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollectionDenglu = this.rewardList.ItemsSource;
		this.InitTime();
		this.InitRewardItem();
		GameInstance.Game.QueryPayActiveInfo(Global.Data.roleData.RoleID, 21);
	}

	private void InitTime()
	{
		this.startTime = Global.GetServerMergeHuodongTimeDateTime(0, 0, 0, 0);
		this.endTime = Global.GetServerMergeHuodongTimeDateTime(6, 23, 59, 59);
		this.lingquEndTime = Global.GetServerMergeHuodongTimeDateTime(6, 23, 59, 59);
		DateTime dateTime;
		dateTime..ctor(this.endTime.Year, this.endTime.Month, this.endTime.Day, 0, 0, 0);
		this.startTimeStr = this.startTime.ToString("yyyy-MM-dd HH:mm:ss");
		this.endTimeStr = this.endTime.ToString("yyyy-MM-dd HH:mm:ss");
		this.lingquEndTimeStr = this.lingquEndTime.ToString("yyyy-MM-dd HH:mm:ss");
		this.huodongStartime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.startTimeStr
		});
		this.huodongEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.endTimeStr
		});
		this.lingquStarttime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.startTimeStr
		});
		this.lingquEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.lingquEndTimeStr
		});
	}

	private void InitRewardItem()
	{
		XElement gameResXml = Global.GetGameResXml("Config/HeFuDengLu.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			HefuLeijiDengluRewardListItem hefuLeijiDengluRewardListItem = U3DUtils.NEW<HefuLeijiDengluRewardListItem>();
			this.ItemCollectionDenglu.Add(hefuLeijiDengluRewardListItem);
			this.CurrentItems.Add(hefuLeijiDengluRewardListItem);
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "TimeOl");
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
			hefuLeijiDengluRewardListItem.daysLabel.text = string.Format(Global.GetLang("第{0}天"), xelementAttributeInt);
			hefuLeijiDengluRewardListItem.daysLabelOver.text = string.Format(Global.GetLang("第{0}天"), xelementAttributeInt);
			hefuLeijiDengluRewardListItem.GoodsIDs = xelementAttributeStr;
			hefuLeijiDengluRewardListItem.DayMark = (DayMark)xelementAttributeInt;
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
					HefuLeijiDengluRewardListItem hefuLeijiDengluRewardListItem = this.CurrentItems[i];
					hefuLeijiDengluRewardListItem.AwardGiftGainState = AwardGiftGainState.CanGain;
				}
			}
		}
		for (int j = 1; j <= count; j++)
		{
			HefuLeijiDengluRewardListItem hefuLeijiDengluRewardListItem = this.CurrentItems[j - 1];
			int intSomeBit = Global.GetIntSomeBit(flag, j);
			if (intSomeBit == 1)
			{
				hefuLeijiDengluRewardListItem.AwardGiftGainState = AwardGiftGainState.Gained;
			}
			else if (intSomeBit == 0 && hefuLeijiDengluRewardListItem.AwardGiftGainState == AwardGiftGainState.CanGain)
			{
				hefuLeijiDengluRewardListItem.AwardGiftGainState = AwardGiftGainState.CanGain;
			}
			else
			{
				hefuLeijiDengluRewardListItem.AwardGiftGainState = AwardGiftGainState.CanNotGain;
			}
		}
	}

	public TextBlock huodongStartime;

	public TextBlock huodongEndtime;

	public TextBlock lingquStarttime;

	public TextBlock lingquEndtime;

	public ListBox rewardList;

	private DateTime startTime;

	private DateTime endTime;

	private DateTime lingquEndTime;

	private string startTimeStr;

	private string endTimeStr;

	private string lingquEndTimeStr;

	private List<HefuLeijiDengluRewardListItem> CurrentItems = new List<HefuLeijiDengluRewardListItem>();

	private ObservableCollection _ItemCollectionDenglu;
}
