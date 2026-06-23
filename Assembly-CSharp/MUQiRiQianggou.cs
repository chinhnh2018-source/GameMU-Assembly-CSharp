using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using Server.Data;

public class MUQiRiQianggou : UserControl
{
	public string ThisXmlName
	{
		get
		{
			return this.thisXmlName;
		}
		set
		{
			this.thisXmlName = value;
			this.day = Global.GetQiRiKuanghuanDaysNum();
			GameInstance.Game.DemandQiRiKuangHuanInfo(4);
			this.InitItem(this.day);
		}
	}

	private void InitTextInPrefabs()
	{
		this.Bak.URL = "NetImages/GameRes/Images/QirihuodongPicture/qiriqianggou.jpg";
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		Global.LoadXml();
	}

	private void InitItem(int day)
	{
		if (day <= 0)
		{
			return;
		}
		if (day > 7)
		{
			day = 7;
		}
		this.SetDay(day);
	}

	private void SetDay(int day)
	{
		ObservableCollection itemsSource = this.goodlist.ItemsSource;
		for (int i = day; i > 0; i--)
		{
			MUQiRiQianggouitem muqiRiQianggouitem = U3DUtils.NEW<MUQiRiQianggouitem>();
			muqiRiQianggouitem.whichDay = i.ToString();
			muqiRiQianggouitem.itemcount = Global.GetItemCount(i);
			this.CurrentDayItems.Add(muqiRiQianggouitem);
			itemsSource.AddNoUpdate(muqiRiQianggouitem);
			U3DUtils.AddChild(this.goodlist.gameObject, muqiRiQianggouitem.gameObject, false);
		}
	}

	public void InitBuyData(SevenDayActQueryData sevendayactquerydata)
	{
		SevenDayItemData sevenDayItemData = null;
		int count = this.CurrentDayItems.Count;
		for (int i = 0; i < count; i++)
		{
			MUQiRiQianggouitem muqiRiQianggouitem = this.CurrentDayItems[i];
			foreach (MUQiRiQiangGouPartItem muqiRiQiangGouPartItem in muqiRiQianggouitem.CurrentItems)
			{
				if (sevendayactquerydata.ItemDict == null)
				{
					muqiRiQiangGouPartItem.leftNum = 0;
				}
				else if (!sevendayactquerydata.ItemDict.TryGetValue(muqiRiQiangGouPartItem.ItemID, ref sevenDayItemData))
				{
					muqiRiQiangGouPartItem.leftNum = 0;
				}
				else
				{
					muqiRiQiangGouPartItem.leftNum = sevenDayItemData.Params1;
				}
			}
		}
	}

	public void SetOldNum(int whichone, int leftNum)
	{
		int count = this.CurrentDayItems.Count;
		for (int i = 0; i < count; i++)
		{
			MUQiRiQianggouitem muqiRiQianggouitem = this.CurrentDayItems[i];
			foreach (MUQiRiQiangGouPartItem muqiRiQiangGouPartItem in muqiRiQianggouitem.CurrentItems)
			{
				if (whichone == muqiRiQiangGouPartItem.ItemID)
				{
					muqiRiQiangGouPartItem.leftNum = leftNum;
					return;
				}
			}
		}
	}

	public ShowNetImage Bak;

	public ListBox goodlist;

	public List<MUQiRiQianggouitem> CurrentDayItems = new List<MUQiRiQianggouitem>();

	private string thisXmlName = string.Empty;

	private int day;
}
