using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class MUJieriPartQianggou : UserControl
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
			this.loadXML();
		}
	}

	private void InitTextInPrefabs()
	{
		this.btnChongzhi.Label.text = Global.GetLang("充值");
		this.labTime.text = Global.GetLang("刷新剩余时间:");
		this.labTime.X = -265.0;
		this.daojishiLabelTime.Pivot = 3;
		this.daojishiLabelTime.X = -208.0;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ZuanshiLabel.text = StringUtil.substitute("{0}", new object[]
		{
			Global.Data.roleData.UserMoney
		});
		GameInstance.Game.SpriteFetchMallData(3);
		this.ItemCollection = this.goodlist.ItemsSource;
		this.currentTicks = Global.GetCorrectLocalTime();
		this.btnChongzhi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.ShowChongZhiWindow();
		};
	}

	private void loadXML()
	{
		XElement xelement = XElement.Parse(this.thisXmlName);
		if (xelement == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "Goods");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement2 = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "Group");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement2, "ID");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement2, "DaysTime");
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
		if (xelement == null)
		{
			return;
		}
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
			if (xelementAttributeInt == 2)
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
				MUJieriPartQianggouItem mujieriPartQianggouItem = U3DUtils.NEW<MUJieriPartQianggouItem>();
				mujieriPartQianggouItem.origPriceLabel.text = num2.ToString();
				mujieriPartQianggouItem.priceLabel.text = num3.ToString();
				mujieriPartQianggouItem.shengyuLabel.text = text2.ToString();
				mujieriPartQianggouItem.TotalNum = xelementAttributeInt4.ToString();
				mujieriPartQianggouItem.xiangouLabel.text = text.ToString();
				mujieriPartQianggouItem.FreshenTime = this.freshenTime;
				mujieriPartQianggouItem.ItemID = num;
				mujieriPartQianggouItem.MallGoodsID = xelementAttributeInt2;
				dummyGoodsData.Binding = 1;
				mujieriPartQianggouItem.GoodsDataInfo = dummyGoodsData;
				mujieriPartQianggouItem.InitGoodsIcon();
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
				mujieriPartQianggouItem.goodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
				mujieriPartQianggouItem.goodIcon.Width = 78.0;
				mujieriPartQianggouItem.goodIcon.Height = 78.0;
				mujieriPartQianggouItem.goodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
				mujieriPartQianggouItem.goodIcon.GoodImg.ForceShow();
				this.ItemCollection.AddNoUpdate(mujieriPartQianggouItem);
				this.listItem.Add(mujieriPartQianggouItem);
				UIPanel component = mujieriPartQianggouItem.transform.GetComponent<UIPanel>();
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
		this.nextQiangGouTime = Global.GetJieriTimeDateTime(this.nextDay, 0, 0, 0);
		base.InvokeRepeating("TickProc", 0f, 1f);
	}

	protected void TickProc()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		long num = this.nextQiangGouTime.Ticks / 10000L;
		int num2 = (int)((num - correctLocalTime) / 1000L);
		if (num2 > 0)
		{
			this.daojishiLabelTime.text = Global.GetTimeStrBySecEx((double)num2, true, -1);
		}
		else if (num2 == 0)
		{
			this.daojishiLabelTime.text = Global.GetLang("等待刷新");
			base.CancelInvoke("TickProc");
			this.thisGroup = 2001;
			this.nextDay = 0;
			this.loadXML();
			base.StartCoroutine(this.GetMalldata());
		}
		else
		{
			this.daojishiLabelTime.text = Global.GetLang("等待刷新");
			base.CancelInvoke("TickProc");
		}
	}

	private IEnumerator GetMalldata()
	{
		yield return new WaitForSeconds(5f);
		GameInstance.Game.SpriteFetchMallData(3);
		yield break;
	}

	public void RefreshOnsaleInfo(int qiangGouId, int num, int saleNum)
	{
		for (int i = 0; i < this.goodlist.Count(); i++)
		{
			MUJieriPartQianggouItem mujieriPartQianggouItem = U3DUtils.AS<MUJieriPartQianggouItem>(this.goodlist[i]);
			if (null != mujieriPartQianggouItem && mujieriPartQianggouItem.ItemID == qiangGouId)
			{
				mujieriPartQianggouItem.shengyuLabel.text = string.Empty + (int.Parse(mujieriPartQianggouItem.TotalNum) - saleNum);
				mujieriPartQianggouItem.xiangouLabel.text = string.Empty + (int.Parse(mujieriPartQianggouItem.xiangouLabel.text) - num);
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

	private List<MUJieriPartQianggouItem> listItem = new List<MUJieriPartQianggouItem>();

	private ObservableCollection _ItemCollection;

	private string thisXmlName = string.Empty;
}
