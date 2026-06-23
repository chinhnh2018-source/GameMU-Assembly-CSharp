using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class HefuPartQianggou : UserControl
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

	private void InitTextInPrefabs()
	{
		this.labTime.text = Global.GetLang("刷新剩余时间:");
		this.btnChongzhi.Text = Global.GetLang("充值");
		this.daojishiLabelTime.Pivot = 3;
		this.daojishiLabelTime.X = -180.0;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ZuanshiLabel.text = StringUtil.substitute("{0}", new object[]
		{
			Global.Data.roleData.UserMoney
		});
		this.loadXML();
		GameInstance.Game.SpriteFetchMallData(3);
		this.ItemCollection = this.goodlist.ItemsSource;
		this.currentTicks = Global.GetCorrectLocalTime();
		this.btnChongzhi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.ShowChongZhiWindow();
			MUDebug.Log<string>(new string[]
			{
				"充值界面"
			});
		};
	}

	private void loadXML()
	{
		XElement gameResXml = Global.GetGameResXml("Config/HeFuQiangGou.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Goods");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Group");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "ID");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "DaysTime");
			if (!this.dictionaryDay.ContainsKey(xelementAttributeInt))
			{
				this.dictionaryDay.Add(xelementAttributeInt, xelementAttributeInt3);
			}
			if (!this.dictionaryGroup.ContainsKey(xelementAttributeInt2))
			{
				this.dictionaryGroup.Add(xelementAttributeInt2, xelementAttributeInt);
			}
		}
	}

	public void InitItem()
	{
		if (string.IsNullOrEmpty(Global.Data.MallData.QiangGouXmlString))
		{
			MUDebug.Log<string>(new string[]
			{
				"无抢购数据！"
			});
			return;
		}
		XElement xelement = XElement.Parse(Global.Data.MallData.QiangGouXmlString);
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(xelement, "Mall"), "*");
		if (xelementList == null)
		{
			return;
		}
		this.ItemCollection.Clear();
		int num = 0;
		foreach (XElement xelement2 in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "Type");
			if (xelementAttributeInt == 1)
			{
				num = Global.GetXElementAttributeInt(xelement2, "QiangGouID");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement2, "GoodsID");
				double num2 = (double)Global.GetXElementAttributeInt(xelement2, "OrigPrice");
				double num3 = (double)Global.GetXElementAttributeInt(xelement2, "Price");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement2, "SinglePurchase");
				int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement2, "FullPurchase");
				int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement2, "SingleHasPurchase");
				int xelementAttributeInt6 = Global.GetXElementAttributeInt(xelement2, "FullHasPurchase");
				string text = StringUtil.substitute("{0}", new object[]
				{
					xelementAttributeInt3 - xelementAttributeInt5
				});
				string text2 = StringUtil.substitute("{0}", new object[]
				{
					xelementAttributeInt4 - xelementAttributeInt6
				});
				int xelementAttributeInt7 = Global.GetXElementAttributeInt(xelement2, "DaysTime");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "StartTime");
				DateTime dataTime = default(DateTime);
				DateTime.TryParse(xelementAttributeStr, ref dataTime);
				string text3 = dataTime.toString("yyyy-MM-dd") + " 00:00:01";
				DateTime dateTime = default(DateTime);
				DateTime.TryParse(text3, ref dateTime);
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(xelementAttributeInt2);
				GoodsData dummyGoodsData = Global.GetDummyGoodsData(xelementAttributeInt2);
				HefuQianggouItem hefuQianggouItem = U3DUtils.NEW<HefuQianggouItem>();
				hefuQianggouItem.origPriceLabel.text = num2.ToString();
				hefuQianggouItem.priceLabel.text = num3.ToString();
				hefuQianggouItem.shengyuLabel.text = text2.ToString();
				hefuQianggouItem.TotalNum = xelementAttributeInt4.ToString();
				hefuQianggouItem.xiangouLabel.text = text.ToString();
				hefuQianggouItem.FreshenTime = this.freshenTime;
				hefuQianggouItem.ItemID = num;
				hefuQianggouItem.MallGoodsID = xelementAttributeInt2;
				dummyGoodsData.Binding = 1;
				hefuQianggouItem.GoodsDataInfo = dummyGoodsData;
				hefuQianggouItem.InitGoodsIcon();
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
				hefuQianggouItem.goodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
				hefuQianggouItem.goodIcon.Width = 78.0;
				hefuQianggouItem.goodIcon.Height = 78.0;
				hefuQianggouItem.goodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
				hefuQianggouItem.goodIcon.GoodImg.ForceShow();
				this.ItemCollection.AddNoUpdate(hefuQianggouItem);
				this.listItem.Add(hefuQianggouItem);
				UIPanel component = hefuQianggouItem.transform.GetComponent<UIPanel>();
				if (component != null)
				{
					Object.Destroy(component);
				}
			}
		}
		for (int i = 0; i < this.dictionaryGroup.Count; i++)
		{
			if (this.dictionaryGroup.ContainsKey(num))
			{
				this.thisGroup = this.dictionaryGroup[num];
			}
		}
		foreach (int num4 in this.dictionaryDay.Keys)
		{
			if (num4 <= this.thisGroup)
			{
				this.nextDay += this.dictionaryDay[num4];
			}
		}
		this.nextQiangGouTime = Global.GetServerMergeHuodongTimeDateTime(this.nextDay, 0, 0, 0);
		base.InvokeRepeating("TickProc", 0f, 1f);
	}

	protected void TickProc()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		long num = this.nextQiangGouTime.Ticks / 10000L;
		if (num > correctLocalTime)
		{
			int num2 = (int)((num - correctLocalTime) / 1000L);
			if (num2 <= 0)
			{
				this.daojishiLabelTime.text = Global.GetLang(string.Empty);
				base.CancelInvoke("TickProc");
				this.thisGroup = 1;
				this.nextDay = 0;
				this.loadXML();
				GameInstance.Game.SpriteFetchMallData(3);
				this.InitItem();
			}
			else
			{
				this.daojishiLabelTime.text = Global.GetTimeStrBySecEx((double)num2, true, -1);
			}
		}
		else
		{
			this.daojishiLabelTime.text = Global.GetLang(string.Empty);
			base.CancelInvoke("TickProc");
		}
	}

	public void RefreshOnsaleInfo(int qiangGouId, int num, int saleNum)
	{
		for (int i = 0; i < this.goodlist.Count(); i++)
		{
			HefuQianggouItem hefuQianggouItem = U3DUtils.AS<HefuQianggouItem>(this.goodlist[i]);
			if (null != hefuQianggouItem && hefuQianggouItem.ItemID == qiangGouId)
			{
				hefuQianggouItem.shengyuLabel.text = string.Empty + (int.Parse(hefuQianggouItem.TotalNum) - saleNum);
				hefuQianggouItem.xiangouLabel.text = string.Empty + (int.Parse(hefuQianggouItem.xiangouLabel.text) - num);
				this.ZuanshiLabel.text = StringUtil.substitute("{0}", new object[]
				{
					Global.Data.roleData.UserMoney
				});
			}
		}
	}

	public GButton btnChongzhi;

	public TextBlock daojishiLabelTime;

	public ListBox goodlist;

	public TextBlock ZuanshiLabel;

	private DateTime nextQiangGouTime;

	private int thisGroup = 1;

	private int nextDay;

	private int freshenTime;

	private long currentTicks;

	public TextBlock labTime;

	private Dictionary<int, int> dictionaryDay = new Dictionary<int, int>();

	private Dictionary<int, int> dictionaryGroup = new Dictionary<int, int>();

	private List<HefuQianggouItem> listItem = new List<HefuQianggouItem>();

	private ObservableCollection _ItemCollection;
}
