using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class MallHotSalePagePart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblNoQianggouData.text = Global.GetLang("暂无抢购数据");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollection = this.listBox.ItemsSource;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.mallItemList = new List<MallItem>();
	}

	public int QianggouId
	{
		get
		{
			return 100000;
		}
	}

	public int tabID
	{
		get
		{
			return this._tabID;
		}
		set
		{
			this._tabID = value;
			if (value == 100000)
			{
				this.listBox.cellHeight = 193f;
				Vector3 localPosition = this.listBox.transform.localPosition;
				this.listBox.transform.localPosition = new Vector3(localPosition.x, localPosition.y - 48f, localPosition.z);
			}
			else
			{
				this.listBox.cellHeight = 100f;
			}
		}
	}

	public int mallType
	{
		get
		{
			return this._mallType;
		}
		set
		{
			this._mallType = value;
		}
	}

	public List<XElement> mallItemXmlList
	{
		set
		{
			this._mallItemXmlList = value;
		}
	}

	public double QiangGouEndTicks
	{
		get
		{
			return this._QiangGouEndTicks;
		}
		set
		{
			this._QiangGouEndTicks = value;
		}
	}

	private void OnEnable()
	{
		if (Enumerable.Count<MallItem>(this.mallItemList) <= 0)
		{
			if (this.tabID == 100000)
			{
				this.RefreshXianGouData();
			}
			else
			{
				this.RefreshGoodsByType(-1);
			}
		}
	}

	public void RefreshXianGouData()
	{
		if (string.IsNullOrEmpty(Global.Data.MallData.QiangGouXmlString))
		{
			this.lblNoQianggouData.gameObject.SetActive(true);
			return;
		}
		this.lblNoQianggouData.gameObject.SetActive(false);
		XElement xelement = XElement.Parse(Global.Data.MallData.QiangGouXmlString);
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(xelement, "Mall"), "*");
		if (xelementList == null)
		{
			return;
		}
		this.ItemCollection.Clear();
		this.mallItemList.Clear();
		string bodyBackground = "shangchengXiangouItem_bak";
		int num = 0;
		foreach (XElement xelement2 in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "Type");
			if (xelementAttributeInt == 0)
			{
				num++;
				if (num > 4)
				{
					break;
				}
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement2, "QiangGouID");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement2, "GoodsID");
				double num2 = (double)Global.GetXElementAttributeInt(xelement2, "OrigPrice");
				double num3 = (double)Global.GetXElementAttributeInt(xelement2, "Price");
				int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement2, "SinglePurchase");
				int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement2, "FullPurchase");
				int xelementAttributeInt6 = Global.GetXElementAttributeInt(xelement2, "SingleHasPurchase");
				int xelementAttributeInt7 = Global.GetXElementAttributeInt(xelement2, "FullHasPurchase");
				string xianGouShu = StringUtil.substitute("{0}", new object[]
				{
					xelementAttributeInt4 - xelementAttributeInt6
				});
				string shengYuShu = StringUtil.substitute("{0}", new object[]
				{
					xelementAttributeInt5 - xelementAttributeInt7
				});
				long num4 = (long)Global.GetXElementAttributeInt(xelement2, "DaysTime");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "StartTime");
				DateTime dataTime = default(DateTime);
				DateTime.TryParse(xelementAttributeStr, ref dataTime);
				string text = dataTime.toString("yyyy-MM-dd") + " 00:00:01";
				DateTime dateTime = default(DateTime);
				DateTime.TryParse(text, ref dateTime);
				this.QiangGouEndTicks = (double)(dateTime.Ticks + num4 * 24L * 60L * 60L * 1000L * 10000L);
				this.QiangGouEndTicks /= 10000.0;
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(xelementAttributeInt3);
				MallItem mallItem = U3DUtils.NEW<MallItem>();
				mallItem.BodyBackground = bodyBackground;
				mallItem.TabPageID = 100000;
				mallItem.ItemID = xelementAttributeInt2;
				mallItem.MallGoodsID = xelementAttributeInt3;
				mallItem.GoodsName = Global.GetColorStringForNGUIText(new object[]
				{
					goodsXmlNodeByID.GoodsColor,
					goodsXmlNodeByID.Title
				});
				mallItem.GoodsPrice = num3.ToString();
				mallItem.GoodsOrgPrice = num2.ToString();
				mallItem.ShengYuShu = shengYuShu;
				mallItem.XianGouShu = xianGouShu;
				mallItem.ItemType = 1;
				GoodsData dummyGoodsData = Global.GetDummyGoodsData(xelementAttributeInt3);
				dummyGoodsData.Binding = 1;
				mallItem.GoodsDataInfo = dummyGoodsData;
				mallItem.InitGoodsIcon();
				this.mallItemList.Add(mallItem);
				this.ItemCollection.AddNoUpdate(mallItem);
				UIPanel component = mallItem.transform.GetComponent<UIPanel>();
				if (component != null)
				{
					Object.Destroy(component);
				}
			}
		}
	}

	public void RefreshGoodsByType(int hintGoodsID = -1)
	{
		this.ClearHintDecoration();
		if (this._mallItemXmlList == null)
		{
			return;
		}
		if (this.RunningCoroutine == null && base.gameObject.activeInHierarchy)
		{
			this.RunningCoroutine = base.StartCoroutine<bool>(this.LoadMallItem(this.iLoadStartIdx));
		}
	}

	public void RefreshOnsaleInfo(int qiangGouId, int num)
	{
		for (int i = 0; i < this.listBox.Count(); i++)
		{
			MallItem mallItem = U3DUtils.AS<MallItem>(this.listBox.GetItemByIndex(i));
			if (null != mallItem && mallItem.ItemID == qiangGouId)
			{
				mallItem.ItemLimitCount.text = string.Empty + (int.Parse(mallItem.ItemLimitCount.text) - num);
				mallItem.ItemLeftCount.text = string.Empty + (int.Parse(mallItem.ItemLeftCount.text) - num);
			}
		}
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		MallItem mallItem = U3DUtils.AS<MallItem>(this.listBox.SelectedItem);
		if (null == mallItem)
		{
			return;
		}
		GoodsPriceUnitTypes goodsPriceUnit = GoodsPriceUnitTypes.Zhuanshi;
		if (mallItem.IsBangZuan)
		{
			goodsPriceUnit = GoodsPriceUnitTypes.BindZhuanshi;
		}
		if (mallItem.TabPageID == 100000)
		{
			if (mallItem.GoodsDataInfo != null)
			{
				GTipServiceEx.ShowTip(mallItem.ItemIcon, TipTypes.GoodsText, GoodsOwnerTypes.QiangGou, goodsPriceUnit, int.Parse(mallItem.ItemPrice.text), mallItem.GoodsDataInfo);
			}
			else
			{
				GTipServiceEx.ShowTip(mallItem.ItemIcon, TipTypes.GoodsText, GoodsOwnerTypes.QiangGou, goodsPriceUnit, int.Parse(mallItem.ItemPrice.text), mallItem.MallGoodsID, -1, -1, null);
			}
		}
		else if (mallItem.GoodsDataInfo != null)
		{
			GTipServiceEx.ShowTip(mallItem.ItemIcon, TipTypes.GoodsText, GoodsOwnerTypes.mallSale, goodsPriceUnit, int.Parse(mallItem.ItemPrice.text), mallItem.GoodsDataInfo);
		}
		else
		{
			GTipServiceEx.ShowTip(mallItem.ItemIcon, TipTypes.GoodsText, GoodsOwnerTypes.mallSale, goodsPriceUnit, int.Parse(mallItem.ItemPrice.text), mallItem.MallGoodsID, -1, -1, null);
		}
	}

	private void StartGouMai(int buyGoodsID, int itemID, int num, bool isXianGou = false)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(buyGoodsID);
		if (Global.IsRebornGood(goodsXmlNodeByID))
		{
			if (Global.IsRebornBagFull())
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("重生背包已满，请先清理出空闲位置后，再购买！"), new object[0]), 10, 3);
				return;
			}
		}
		else if (Global.IsBagFull())
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再购买！"), new object[0]), 10, 3);
			return;
		}
		if (this.tabID == 10000)
		{
			GameInstance.Game.SpriteMallZhenQiBuy(itemID, num);
		}
		else if (isXianGou)
		{
			GameInstance.Game.SpriteMallQiangGouBuy(itemID, num, false, buyGoodsID);
		}
		else
		{
			GameInstance.Game.SpriteMallBuy(itemID, num, false);
		}
	}

	private IEnumerator LoadMallItem(int StartIdx)
	{
		yield return null;
		int limitPerFrame = (StartIdx + this.LoadCountPerFrame > this._mallItemXmlList.Count) ? this._mallItemXmlList.Count : (StartIdx + this.LoadCountPerFrame);
		for (int i = StartIdx; i < limitPerFrame; i++)
		{
			int goodsID = Global.GetXElementAttributeInt(this._mallItemXmlList[i], "GoodsID");
			GoodsData goodsData = null;
			this.LoadGoodsData(Global.GetXElementAttributeStr(this._mallItemXmlList[i], "Property"), goodsID, out goodsData);
			int mallID = Global.GetXElementAttributeInt(this._mallItemXmlList[i], "ID");
			int tempTabID = Global.GetXElementAttributeInt(this._mallItemXmlList[i], "TabID");
			double origPrice = (double)Global.GetXElementAttributeInt(this._mallItemXmlList[i], "OrigPrice");
			double price = (double)Global.GetXElementAttributeInt(this._mallItemXmlList[i], "Price");
			string pubStartTime = Global.GetXElementAttributeStr(this._mallItemXmlList[i], "PubStartTime");
			string pubEndTime = Global.GetXElementAttributeStr(this._mallItemXmlList[i], "PubEndTime");
			string zhenQi = Global.GetXElementAttributeStr(this._mallItemXmlList[i], "ZhenQi");
			if (Global.InLimitTimeRange(pubStartTime, pubEndTime))
			{
				GoodVO goodVO = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
				MallItem si = U3DUtils.NEW<MallItem>();
				this.ItemCollection.AddNoUpdate(si);
				si.ItemID = mallID;
				si.GoodsDataInfo = goodsData;
				si.MallGoodsID = goodsID;
				int categoriy = Global.GetCategoriyByGoodsID(goodsID);
				if (categoriy >= 0 && categoriy < 25)
				{
					si.GoodsName = string.Concat(new string[]
					{
						"{",
						Global.GetColorByGoodsData(goodsData).ToString("X"),
						"}",
						goodVO.Title,
						"{-}"
					});
				}
				else
				{
					si.GoodsName = Global.GetColorStringForNGUIText(new object[]
					{
						goodVO.GoodsColor,
						goodVO.Title
					});
				}
				si.GoodsPrice = price.ToString();
				si.ItemDiamondIcon.spriteName = "moneyZhuanshi";
				si.TabPageID = this.tabID;
				si.ItemType = 0;
				si.IsBangZuan = false;
				si.InitGoodsIcon();
				if (this.tabID == 10000)
				{
					si.IsBangZuan = true;
					si.GoodsPrice = zhenQi.ToString();
					si.ItemDiamondIcon.spriteName = "moneyBindZhuanshi";
				}
				si.ItemIcon.DPSelectedItem = delegate(object s1, DPSelectedItemEventArgs e1)
				{
					MallItem mallItem = U3DUtils.AS<MallItem>(this.listBox.SelectedItem);
					if (e1.IDType == 8)
					{
						if (mallItem.TabPageID == 10000)
						{
							if (Global.Data.roleData.Gold < int.Parse(mallItem.GoodsPrice) * e1.ID)
							{
								Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
								return;
							}
						}
						else if (Global.Data.roleData.UserMoney < int.Parse(mallItem.GoodsPrice) * e1.ID)
						{
							Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
							return;
						}
						this.StartGouMai(mallItem.MallGoodsID, mallItem.ItemID, e1.ID, mallItem.ItemType != 0);
					}
				};
				this.mallItemList.Add(si);
			}
		}
		if (limitPerFrame < this._mallItemXmlList.Count)
		{
			this.iLoadStartIdx = limitPerFrame;
			base.StartCoroutine<bool>(this.LoadMallItem(this.iLoadStartIdx));
		}
		else
		{
			this.iLoadStartIdx = 0;
			this.RunningCoroutine = null;
		}
		yield break;
	}

	private void LoadGoodsData(string goodsDataInfo, int goodsId, out GoodsData goodsData)
	{
		goodsData = null;
		string[] array = goodsDataInfo.Split(new char[]
		{
			','
		});
		if (array.Length == 4)
		{
			int binding = (this.tabID != 10000) ? 0 : 1;
			goodsData = Global.GetDummyGoodsDataMu(goodsId, int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[3]), int.Parse(array[2]), binding, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		}
	}

	private const int iQiangGouid = 100000;

	public UILabel lblNoQianggouData;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox listBox;

	private ObservableCollection ItemCollection;

	private List<MallItem> mallItemList;

	public UIScrollBar scrollBar;

	public UIDraggablePanel dragPanel;

	private int _tabID;

	private int _mallType;

	private List<XElement> _mallItemXmlList;

	private double _QiangGouEndTicks = -1.0;

	private TTMonoBehaviour.Coroutine<bool> RunningCoroutine;

	private int iLoadStartIdx;

	private int LoadCountPerFrame = 8;

	public enum MallType
	{
		TypeNormal,
		TypeOnSale
	}
}
