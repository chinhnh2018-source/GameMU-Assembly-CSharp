using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class MUQiRiQianggouitem : UserControl
{
	public string whichDay
	{
		get
		{
			return this._whichDay;
		}
		set
		{
			this._whichDay = value;
			this.day.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				string.Format(Global.GetLang("第{0}天"), Global.GetLang(this._whichDay))
			});
		}
	}

	public int itemcount
	{
		get
		{
			return this._itemcount;
		}
		set
		{
			this._itemcount = value;
			this.InitItem();
		}
	}

	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	private void InitItem()
	{
		if (this._itemcount == 0)
		{
			return;
		}
		if (this._itemcount >= 1)
		{
			int itemID = 0;
			int num = int.Parse(this.whichDay);
			int oldprice = 0;
			int price = 0;
			int totalNum = 0;
			string goods = string.Empty;
			string name = string.Empty;
			foreach (SevenDayQiangGou sevenDayQiangGou in Global.dic_SevenDayQiangGou.Values)
			{
				if (num == sevenDayQiangGou.Day)
				{
					itemID = sevenDayQiangGou.ID;
					oldprice = sevenDayQiangGou.OrigPrice;
					price = sevenDayQiangGou.Price;
					totalNum = sevenDayQiangGou.Purchase;
					goods = sevenDayQiangGou.GoodsID;
					name = sevenDayQiangGou.Name;
					break;
				}
			}
			this.item_1 = U3DUtils.NEW<MUQiRiQiangGouPartItem>();
			this.item_1.ItemID = itemID;
			this.item_1.Goods = goods;
			this.item_1.oldprice = oldprice;
			this.item_1.price = price;
			this.item_1.TotalNum = totalNum;
			this.item_1.Name = name;
			this.CurrentItems.Add(this.item_1);
			UIPanel component = this.item_1.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			U3DUtils.AddChild(this.leftItem.gameObject, this.item_1.gameObject, false);
		}
		if (this._itemcount >= 2)
		{
			int itemID2 = 0;
			int num2 = int.Parse(this.whichDay);
			int oldprice2 = 0;
			int price2 = 0;
			int totalNum2 = 0;
			string goods2 = string.Empty;
			string name2 = string.Empty;
			bool flag = false;
			foreach (SevenDayQiangGou sevenDayQiangGou2 in Global.dic_SevenDayQiangGou.Values)
			{
				if (num2 == sevenDayQiangGou2.Day)
				{
					if (flag)
					{
						itemID2 = sevenDayQiangGou2.ID;
						oldprice2 = sevenDayQiangGou2.OrigPrice;
						price2 = sevenDayQiangGou2.Price;
						totalNum2 = sevenDayQiangGou2.Purchase;
						goods2 = sevenDayQiangGou2.GoodsID;
						name2 = sevenDayQiangGou2.Name;
						break;
					}
					flag = true;
				}
			}
			this.item_2 = U3DUtils.NEW<MUQiRiQiangGouPartItem>();
			this.item_2.ItemID = itemID2;
			this.item_2.Goods = goods2;
			this.item_2.oldprice = oldprice2;
			this.item_2.price = price2;
			this.item_2.TotalNum = totalNum2;
			this.item_2.Name = name2;
			this.CurrentItems.Add(this.item_2);
			UIPanel component2 = this.item_2.GetComponent<UIPanel>();
			if (component2)
			{
				Object.Destroy(component2);
			}
			U3DUtils.AddChild(this.rightItem.gameObject, this.item_2.gameObject, false);
		}
	}

	public List<MUQiRiQiangGouPartItem> CurrentItems = new List<MUQiRiQiangGouPartItem>();

	public UILabel day;

	public GameObject leftItem;

	public GameObject rightItem;

	private MUQiRiQiangGouPartItem item_1;

	private MUQiRiQiangGouPartItem item_2;

	private string _whichDay;

	private int _itemcount;
}
