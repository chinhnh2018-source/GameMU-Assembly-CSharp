using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class BuyGoodsPart : UserControl
{
	protected override void OnDestroy()
	{
		BuyGoodsPart.Instance = null;
	}

	private void InitTextInPrefabs()
	{
		this.m_CBDiamond.Text = Global.GetLang("显示钻石");
		this.m_CBGold.Text = Global.GetLang("显示金币");
		this.m_BtnSearch.Text = Global.GetLang("搜索");
	}

	protected override void InitializeComponent()
	{
		BuyGoodsPart.Instance = this;
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (null != this.m_ListGoodItem)
		{
			this.m_ListGoodItemObC = this.m_ListGoodItem.ItemsSource;
		}
		if (this.m_BtnSearch != null)
		{
			this.m_BtnSearch.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.OpenSearchPart();
			};
		}
		if (this.m_LeftArrow != null)
		{
			UIEventListener.Get(this.m_LeftArrow.gameObject).onClick = delegate(GameObject s)
			{
				if (this.CurrentPage <= 1)
				{
					Super.HintMainText(Global.GetLang("已经达到最小页数"), 10, 3);
					return;
				}
				int num = this.CurrentPage - 1;
				if (this.IsSearchAll)
				{
					Super.ShowNetWaiting(null);
					int unitValue = this.GetUnitValue();
					GameInstance.Game.SpriteMarketGoodsList2(3, string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
					{
						this.JiaoYiTabID,
						this.JiaoYiTypeID,
						unitValue,
						this.SearchAllQualityValue,
						this.OrderType,
						(int)this.SearchOrderTypeValue,
						this.SearchAllInputText
					}), (num - 1) * 10, 10);
				}
				else
				{
					this.GetMarketGoodsList(num);
				}
			};
		}
		if (this.m_RightArrow != null)
		{
			UIEventListener.Get(this.m_RightArrow.gameObject).onClick = delegate(GameObject s)
			{
				if (this.CurrentPage >= this.m_totalPage)
				{
					Super.HintMainText(Global.GetLang("已经达到最大页数"), 10, 3);
					return;
				}
				int num = this.CurrentPage + 1;
				if (this.IsSearchAll)
				{
					Super.ShowNetWaiting(null);
					int unitValue = this.GetUnitValue();
					GameInstance.Game.SpriteMarketGoodsList2(3, string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
					{
						this.JiaoYiTabID,
						this.JiaoYiTypeID,
						unitValue,
						this.SearchAllQualityValue,
						this.OrderType,
						(int)this.SearchOrderTypeValue,
						this.SearchAllInputText
					}), (num - 1) * 10, 10);
				}
				else
				{
					this.GetMarketGoodsList(num);
				}
			};
		}
		if (this.m_CBGold != null)
		{
			this.m_CBGold.CheckChanged = delegate(object s, BaseEventArgs e)
			{
				if (!this.m_CBGold.Check && !this.m_CBDiamond.Check)
				{
					this.m_CBDiamond.Check = true;
				}
				this.GetMarketGoodsList(1);
			};
		}
		if (this.m_CBDiamond != null)
		{
			this.m_CBDiamond.CheckChanged = delegate(object s, BaseEventArgs e)
			{
				if (!this.m_CBDiamond.Check && !this.m_CBGold.Check)
				{
					this.m_CBGold.Check = true;
				}
				this.GetMarketGoodsList(1);
			};
		}
		if (this.m_SpriteOrderByTotalPrice != null)
		{
			UIEventListener.Get(this.m_SpriteOrderByTotalPrice).onClick = delegate(GameObject s)
			{
				if (this.OrderType == 1)
				{
					this.OrderType = 0;
				}
				else
				{
					this.OrderType = 1;
				}
				this.SearchOrderTypeValue = SearchOrderTypes.OrderByMoney;
				if (this.IsSearchAll)
				{
					Super.ShowNetWaiting(null);
					int unitValue = this.GetUnitValue();
					GameInstance.Game.SpriteMarketGoodsList2(3, string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
					{
						this.JiaoYiTabID,
						this.JiaoYiTypeID,
						unitValue,
						this.SearchAllQualityValue,
						this.OrderType,
						(int)this.SearchOrderTypeValue,
						this.SearchAllInputText
					}), (this.CurrentPage - 1) * 10, 10);
				}
				else
				{
					this.GetMarketGoodsList(1);
				}
			};
		}
		if (this.m_SpriteOrderByEachPrice != null)
		{
			UIEventListener.Get(this.m_SpriteOrderByEachPrice).onClick = delegate(GameObject s)
			{
				if (this.OrderType == 1)
				{
					this.OrderType = 0;
				}
				else
				{
					this.OrderType = 1;
				}
				this.SearchOrderTypeValue = SearchOrderTypes.OrderByMoneyPerItem;
				if (this.IsSearchAll)
				{
					Super.ShowNetWaiting(null);
					int unitValue = this.GetUnitValue();
					GameInstance.Game.SpriteMarketGoodsList2(3, string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
					{
						this.JiaoYiTabID,
						this.JiaoYiTypeID,
						unitValue,
						this.SearchAllQualityValue,
						this.OrderType,
						(int)this.SearchOrderTypeValue,
						this.SearchAllInputText
					}), (this.CurrentPage - 1) * 10, 10);
				}
				else
				{
					this.GetMarketGoodsList(1);
				}
			};
		}
		if (this.m_SpriteOrderBySuit != null)
		{
			UIEventListener.Get(this.m_SpriteOrderBySuit).onClick = delegate(GameObject s)
			{
				if (this.OrderType == 1)
				{
					this.OrderType = 0;
				}
				else
				{
					this.OrderType = 1;
				}
				this.SearchOrderTypeValue = SearchOrderTypes.OrderBySuit;
				if (this.IsSearchAll)
				{
					Super.ShowNetWaiting(null);
					int unitValue = this.GetUnitValue();
					GameInstance.Game.SpriteMarketGoodsList2(3, string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
					{
						this.JiaoYiTabID,
						this.JiaoYiTypeID,
						unitValue,
						this.SearchAllQualityValue,
						this.OrderType,
						(int)this.SearchOrderTypeValue,
						this.SearchAllInputText
					}), (this.CurrentPage - 1) * 10, 10);
				}
				else
				{
					this.GetMarketGoodsList(1);
				}
			};
		}
		if (this.m_SpriteOrderByName != null)
		{
			UIEventListener.Get(this.m_SpriteOrderByName).onClick = delegate(GameObject s)
			{
				if (this.OrderType == 1)
				{
					this.OrderType = 0;
				}
				else
				{
					this.OrderType = 1;
				}
				this.SearchOrderTypeValue = SearchOrderTypes.OrderByNameAndColor;
				if (this.IsSearchAll)
				{
					Super.ShowNetWaiting(null);
					int unitValue = this.GetUnitValue();
					GameInstance.Game.SpriteMarketGoodsList2(3, string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
					{
						this.JiaoYiTabID,
						this.JiaoYiTypeID,
						unitValue,
						this.SearchAllQualityValue,
						this.OrderType,
						(int)this.SearchOrderTypeValue,
						this.SearchAllInputText
					}), (this.CurrentPage - 1) * 10, 10);
				}
				else
				{
					this.GetMarketGoodsList(1);
				}
			};
		}
		this.OrderType = 1;
		this.SearchOrderTypeValue = SearchOrderTypes.OrderByMoney;
		this.ItemList.IsPosYFixed = true;
		XElement gameResXml = Global.GetGameResXml("JiaoYiTab");
		this.TabFileList = Global.GetXElementList(gameResXml, "Copy");
		XElement gameResXml2 = Global.GetGameResXml("JiaoYiType");
		this.TabFileListForItem = Global.GetXElementList(gameResXml2, "JiaoYiSuo");
		this.initList();
		if (this.TabFileList.Count > 0)
		{
			this.JiaoYiTabID = Global.GetXElementAttributeInt(this.TabFileList[0], "TabID");
		}
		this.initItemPart(0);
		this.ItemList.UpdataNow();
	}

	public bool IsGoldChecked
	{
		get
		{
			return this.m_CBGold.Check;
		}
	}

	public bool IsDiamondChecked
	{
		get
		{
			return this.m_CBDiamond.Check;
		}
	}

	public int OrderType
	{
		get
		{
			return this.m_OrderType;
		}
		set
		{
			this.m_OrderType = value;
			if (this.m_OrderType == 0)
			{
			}
		}
	}

	public SearchOrderTypes SearchOrderTypeValue
	{
		get
		{
			return this.m_SearchOrderType;
		}
		set
		{
			this.m_SearchOrderType = value;
		}
	}

	public int CurrentPage
	{
		get
		{
			if (this.m_currentPage <= 1)
			{
				return 1;
			}
			return this.m_currentPage;
		}
		set
		{
			this.m_currentPage = value;
			if (this.m_currentPage <= 1)
			{
				this.m_currentPage = 1;
			}
		}
	}

	public int GetUnitValue()
	{
		int num = 0;
		if (this.IsGoldChecked)
		{
			num++;
		}
		if (this.IsDiamondChecked)
		{
			num += 2;
		}
		return num;
	}

	public void GetMarketGoodsListAfterBuy()
	{
		if (this.IsSearchAll)
		{
			Super.ShowNetWaiting(null);
			int unitValue = this.GetUnitValue();
			GameInstance.Game.SpriteMarketGoodsList2(3, string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
			{
				this.JiaoYiTabID,
				this.JiaoYiTypeID,
				unitValue,
				this.SearchAllQualityValue,
				this.OrderType,
				(int)this.SearchOrderTypeValue,
				this.SearchAllInputText
			}), (this.CurrentPage - 1) * 10, 10);
		}
		else
		{
			this.GetMarketGoodsList(1);
		}
	}

	public void GetMarketGoodsList(int requestPage = 1)
	{
		if (requestPage <= 1)
		{
			requestPage = 1;
		}
		this.IsSearchAll = false;
		int unitValue = this.GetUnitValue();
		MUDebug.Log<string>(new string[]
		{
			"unityValue" + unitValue
		});
		Super.ShowNetWaiting(null);
		GameInstance.Game.SpriteMarketGoodsList2(3, string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
		{
			this.JiaoYiTabID,
			this.JiaoYiTypeID,
			unitValue,
			63,
			this.OrderType,
			(int)this.SearchOrderTypeValue,
			string.Empty
		}), (requestPage - 1) * 10, 10);
	}

	private void initList()
	{
		this.itemsCount = this.TabFileList.Count;
		if (this.items == null)
		{
			this.items = new BuyGoodsPartItem[this.itemsCount];
		}
		for (int i = 0; i < this.itemsCount; i++)
		{
			if (this.items[i] == null)
			{
				this.items[i] = U3DUtils.NEW<BuyGoodsPartItem>();
				this.items[i].TextName.text = Global.GetXElementAttributeStr(this.TabFileList[i], "Name");
				this.items[i].ItemIndex = Global.GetXElementAttributeInt(this.TabFileList[i], "TabID");
				this.items[i].BShowChild = Global.GetXElementAttributeInt(this.TabFileList[i], "Show");
				this.items[i].TabFileList = this.TabFileListForItem;
				this.items[i].ListIndex = i;
			}
			this.items[i].DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				BuyGoodsPartItem buyGoodsPartItem = s as BuyGoodsPartItem;
				this.JiaoYiTabID = buyGoodsPartItem.ItemIndex;
				this.JiaoYiTypeID = buyGoodsPartItem.ItemIndexChild;
				if (e.ID == 1)
				{
					this.initItemPart(buyGoodsPartItem.ListIndex);
					this.ItemList.UpdataNow();
				}
				if (e.ID == 2)
				{
					this.GetMarketGoodsList(1);
				}
			};
			U3DUtils.AddChild(this.ItemList.gameObject, this.items[i].gameObject, false);
		}
	}

	private void setTab(int index = -1)
	{
		if (index >= 0)
		{
			if (this.selectedIndex >= 0)
			{
				if (index == this.selectedIndex)
				{
					this.items[this.selectedIndex].ToggleState = !this.items[this.selectedIndex].ToggleState;
					return;
				}
				if (this.items[this.selectedIndex].ToggleState)
				{
					this.items[this.selectedIndex].ToggleState = false;
				}
			}
			this.selectedIndex = index;
			this.items[this.selectedIndex].ToggleState = true;
		}
	}

	private void initItemPart(int index)
	{
		this.GetMarketGoodsList(1);
		this.setTab(index);
	}

	public void RefreshGoodsList(SaleGoodsSearchResultData saleGoodsDataResult)
	{
		this.PnlGoodItem.transform.localPosition = new Vector3(70f, 72f, 0f);
		UIPanel component = this.PnlGoodItem.GetComponent<UIPanel>();
		if (component != null)
		{
			component.clipRange = new Vector4(-10f, -110f, 770f, 310f);
		}
		this.m_ListGoodItemObC.Clear();
		this.m_ListGoodItem.Clear();
		if (saleGoodsDataResult.TotalCount == -1)
		{
			Super.HintMainText(Global.GetLang("搜索的物品不存在"), 10, 3);
		}
		MUDebug.Log<string>(new string[]
		{
			"saleGoodsDataResult.saleGoodsDataList ====== null"
		});
		if (saleGoodsDataResult.saleGoodsDataList != null)
		{
			MUDebug.Log<string>(new string[]
			{
				"saleGoodsDataResult.saleGoodsDataList != null"
			});
			int count = saleGoodsDataResult.saleGoodsDataList.Count;
			MUDebug.Log<string>(new string[]
			{
				"saleGoodsDataResult.saleGoodsDataList Count:" + count
			});
			for (int i = 0; i < count; i++)
			{
				SaleGoodsData saleGoodsData = saleGoodsDataResult.saleGoodsDataList[i];
				GoodsData salingGoodsData = saleGoodsData.SalingGoodsData;
				if (salingGoodsData != null && ConfigGoods.GetGoodsXmlNodeByID(salingGoodsData.GoodsID) == null)
				{
					MUDebug.LogError<string>(new string[]
					{
						"数据对象错误"
					});
				}
				else
				{
					BuyGoodsPartGoodItem buyGoodsPartGoodItem = U3DUtils.NEW<BuyGoodsPartGoodItem>();
					this.m_ListGoodItemObC.Add(buyGoodsPartGoodItem);
					buyGoodsPartGoodItem.RefreshByData(saleGoodsData);
				}
			}
			int num = saleGoodsDataResult.TotalCount / 10;
			int num2 = (saleGoodsDataResult.TotalCount % 10 <= 0) ? 0 : 1;
			this.m_totalPage = num + num2;
			int currentPage = saleGoodsDataResult.StartIndex / 10 + 1;
			this.CurrentPage = currentPage;
			this.m_TextPage.text = this.CurrentPage + " / " + this.m_totalPage;
		}
		else
		{
			this.m_totalPage = 0;
			this.m_TextPage.text = "0 / 0";
		}
	}

	private void OpenSearchPart()
	{
		this.SearchWindow = U3DUtils.NEW<GChildWindow>();
		this.SearchWindow.ModalType = ChildWindowModalType.Translucent;
		Super.InitChildWindow(this.SearchWindow, Global.GetLang(string.Empty));
		this.SearchWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
		{
			this.CloseSearchPart();
			return true;
		};
		if (null == this.SearchPart)
		{
			this.SearchPart = U3DUtils.NEW<BuyGoodsSearchPart>();
			this.SearchPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					this.CloseSearchPart();
				}
				else if (e.ID == 1)
				{
					this.SearchAllQualityValue = e.Quality;
					this.SearchAllInputText = e.Title;
					Super.ShowNetWaiting(null);
					this.IsSearchAll = true;
					int unitValue = this.GetUnitValue();
					GameInstance.Game.SpriteMarketGoodsList2(3, string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
					{
						this.JiaoYiTabID,
						this.JiaoYiTypeID,
						unitValue,
						this.SearchAllQualityValue,
						this.OrderType,
						this.SearchOrderTypeValue,
						this.SearchAllInputText
					}), (this.CurrentPage - 1) * 10, 10);
					this.CloseSearchPart();
				}
			};
			this.SearchWindow.SetContent(this.SearchWindow.BodyPresenter, this.SearchPart, 0.0, 0.0, true);
			Super.GData.PlayZoneRoot.Children.Add(this.SearchWindow);
		}
	}

	private void CloseSearchPart()
	{
		if (null != this.SearchWindow)
		{
			Super.CloseChildWindow(Super.GData.PlayZoneRoot, this.SearchWindow);
			Object.Destroy(this.SearchPart.gameObject);
			this.SearchPart = null;
			this.SearchWindow = null;
		}
	}

	private const int EachPageCount = 10;

	public static BuyGoodsPart Instance;

	public DPSelectedItemEventHandler DPSelectedItem;

	public UITable ItemList;

	public GCheckBox m_CBGold;

	public GCheckBox m_CBDiamond;

	public GButton m_BtnSearch;

	public UIButton m_LeftArrow;

	public UIButton m_RightArrow;

	public UILabel m_TextPage;

	public GameObject PnlGoodItem;

	public GameObject m_SpriteOrderByTotalPrice;

	public GameObject m_SpriteOrderByEachPrice;

	public GameObject m_SpriteOrderBySuit;

	public GameObject m_SpriteOrderByName;

	private BuyGoodsPartItem[] items;

	private int itemsCount;

	private int selectedIndex = -1;

	private int m_currentPage = 1;

	private int m_totalPage;

	private int JiaoYiTabID;

	private int JiaoYiTypeID;

	private List<XElement> TabFileList;

	private List<XElement> TabFileListForItem;

	public ListBox m_ListGoodItem = new ListBox();

	private ObservableCollection m_ListGoodItemObC;

	private bool IsSearchAll;

	private int SearchAllQualityValue;

	private string SearchAllInputText = string.Empty;

	private int m_OrderType = 1;

	private SearchOrderTypes m_SearchOrderType = SearchOrderTypes.OrderByMoney;

	private GChildWindow SearchWindow;

	private BuyGoodsSearchPart SearchPart;
}
