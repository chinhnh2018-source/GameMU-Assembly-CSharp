using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class BuyGoodsJinTuanPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnSearch.Text = Global.GetLang("搜索");
		this.m_BtnWorldSale.Text = Global.GetLang("世界拍卖");
		this.m_BtnZhanMengSale.Text = Global.GetLang("战盟拍卖");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		base.CancelInvoke("TickProc");
		base.InvokeRepeating("TickProc", 1f, 1f);
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
		if (this.m_BtnHelp != null)
		{
			this.m_BtnHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.OpenHelpWindow();
			};
		}
		if (this.m_BtnWorldSale != null)
		{
			this.m_BtnWorldSale.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.SetPart(2);
			};
		}
		if (this.m_BtnZhanMengSale != null)
		{
			this.m_BtnZhanMengSale.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (!Global.IsHavingBangHui())
				{
					Super.HintMainText(Global.GetLang("当前无战盟"), 10, 3);
					return;
				}
				this.SetPart(1);
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
				int startPage = this.CurrentPage - 1;
				GameInstance.Game.SendGetJinTuanInfo(this.m_AuctionType, (int)this.m_SearchOrderType, this.m_OrderType, startPage, 10, this.SearchAllInputText, this.SearchAllQualityValue);
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
				int startPage = this.CurrentPage + 1;
				GameInstance.Game.SendGetJinTuanInfo(this.m_AuctionType, (int)this.m_SearchOrderType, this.m_OrderType, startPage, 10, this.SearchAllInputText, this.SearchAllQualityValue);
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
				this.SearchOrderTypeValue = SearchOrderTypesJinTuan.OrderByMaxPrice;
				GameInstance.Game.SendGetJinTuanInfo(this.m_AuctionType, (int)this.m_SearchOrderType, this.m_OrderType, this.CurrentPage, 10, this.SearchAllInputText, this.SearchAllQualityValue);
			};
		}
		if (this.m_SpriteOrderByPrice != null)
		{
			UIEventListener.Get(this.m_SpriteOrderByPrice).onClick = delegate(GameObject s)
			{
				if (this.OrderType == 1)
				{
					this.OrderType = 0;
				}
				else
				{
					this.OrderType = 1;
				}
				this.SearchOrderTypeValue = SearchOrderTypesJinTuan.OrderByUnitPrice;
				GameInstance.Game.SendGetJinTuanInfo(this.m_AuctionType, (int)this.m_SearchOrderType, this.m_OrderType, this.CurrentPage, 10, this.SearchAllInputText, this.SearchAllQualityValue);
			};
		}
		if (this.m_SpriteOrderByTime != null)
		{
			UIEventListener.Get(this.m_SpriteOrderByTime).onClick = delegate(GameObject s)
			{
				if (this.OrderType == 1)
				{
					this.OrderType = 0;
				}
				else
				{
					this.OrderType = 1;
				}
				this.SearchOrderTypeValue = SearchOrderTypesJinTuan.OrderByTime;
				GameInstance.Game.SendGetJinTuanInfo(this.m_AuctionType, (int)this.m_SearchOrderType, this.m_OrderType, this.CurrentPage, 10, this.SearchAllInputText, this.SearchAllQualityValue);
			};
		}
		int part = 2;
		if (ConfigSystemParam.GetSystemParamIntByName("AuctionZhanMengOpen") == 0L)
		{
			NGUITools.SetActive(this.m_BtnZhanMengSale, false);
		}
		else if (Global.IsHavingBangHui())
		{
			part = 1;
		}
		this.SetPart(part);
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
		}
	}

	public SearchOrderTypesJinTuan SearchOrderTypeValue
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

	public void RefreshGoodsList(GoldAuctionS2C saleGoodsDataResult)
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
		if (saleGoodsDataResult.ItemList != null)
		{
			int count = saleGoodsDataResult.ItemList.Count;
			for (int i = 0; i < count; i++)
			{
				AuctionItemS2C auctionItemS2C = saleGoodsDataResult.ItemList[i];
				GoodsData goods = auctionItemS2C.Goods;
				if (goods != null && ConfigGoods.GetGoodsXmlNodeByID(goods.GoodsID) == null)
				{
					MUDebug.LogError<string>(new string[]
					{
						"数据对象错误"
					});
				}
				else
				{
					BuyGoodsJinTuanPartGoodItem buyGoodsJinTuanPartGoodItem = U3DUtils.NEW<BuyGoodsJinTuanPartGoodItem>();
					this.m_ListGoodItemObC.Add(buyGoodsJinTuanPartGoodItem);
					buyGoodsJinTuanPartGoodItem.RefreshByData(auctionItemS2C);
					buyGoodsJinTuanPartGoodItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
					{
						if (e.ID == 1)
						{
							this.OpenStallGoodJinTuanPart(e.Data as AuctionItemS2C);
						}
						else if (e.ID == 2)
						{
							GameInstance.Game.SendGetJinTuanInfo(this.m_AuctionType, (int)this.m_SearchOrderType, this.m_OrderType, this.CurrentPage, 10, this.SearchAllInputText, this.SearchAllQualityValue);
						}
					};
				}
			}
			int num = saleGoodsDataResult.TotalCount / 10;
			int num2 = (saleGoodsDataResult.TotalCount % 10 <= 0) ? 0 : 1;
			this.m_totalPage = num + num2;
			this.CurrentPage = saleGoodsDataResult.CurrentPage;
			this.m_TextPage.text = this.CurrentPage + " / " + this.m_totalPage;
		}
		else
		{
			this.m_totalPage = 0;
			this.m_TextPage.text = "0 / 0";
		}
	}

	public void GetJinTuanInfo()
	{
		GameInstance.Game.SendGetJinTuanInfo(this.m_AuctionType, (int)this.m_SearchOrderType, this.m_OrderType, this.CurrentPage, 10, this.SearchAllInputText, this.SearchAllQualityValue);
	}

	private void SetPart(int type)
	{
		this.SearchAllInputText = string.Empty;
		this.SearchAllQualityValue = 15;
		if (type != 1)
		{
			if (type == 2)
			{
				this.SetBtnStat(this.m_BtnWorldSale, true);
				this.SetBtnStat(this.m_BtnZhanMengSale, false);
				this.m_AuctionType = 2;
			}
		}
		else
		{
			this.SetBtnStat(this.m_BtnWorldSale, false);
			this.SetBtnStat(this.m_BtnZhanMengSale, true);
			this.m_AuctionType = 1;
		}
		GameInstance.Game.SendGetJinTuanInfo(this.m_AuctionType, (int)this.m_SearchOrderType, this.m_OrderType, this.CurrentPage, 10, this.SearchAllInputText, this.SearchAllQualityValue);
	}

	private void SetBtnStat(GButton btn, bool selected)
	{
		if (null != btn)
		{
			if (selected)
			{
				btn.Label.color = NGUIMath.HexToColorEx(15790320U);
				btn.Pressed = true;
				btn.Refresh();
			}
			else
			{
				btn.Label.color = NGUIMath.HexToColorEx(10323559U);
				btn.Pressed = false;
				btn.Refresh();
			}
		}
	}

	protected void TickProc()
	{
		if (this.m_ListGoodItem != null && this.m_ListGoodItemObC.Length > 0)
		{
			for (int i = this.m_ListGoodItemObC.Length - 1; i >= 0; i--)
			{
				BuyGoodsJinTuanPartGoodItem buyGoodsJinTuanPartGoodItem = U3DUtils.AS<BuyGoodsJinTuanPartGoodItem>(this.m_ListGoodItem.GetItemByIndex(i));
				if (buyGoodsJinTuanPartGoodItem != null)
				{
					buyGoodsJinTuanPartGoodItem.SubRemainTime();
					if (buyGoodsJinTuanPartGoodItem.RemainTime <= 0L)
					{
						break;
					}
				}
			}
		}
	}

	public override void Destroy()
	{
		base.CancelInvoke("TickProc");
		base.Destroy();
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
					GameInstance.Game.SendGetJinTuanInfo(this.m_AuctionType, (int)this.m_SearchOrderType, this.m_OrderType, this.CurrentPage, 10, this.SearchAllInputText, this.SearchAllQualityValue);
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

	private void OpenStallGoodJinTuanPart(AuctionItemS2C auctionItem)
	{
		this.StallGoodJinTuanWindow = U3DUtils.NEW<GChildWindow>();
		this.StallGoodJinTuanWindow.ModalType = ChildWindowModalType.Translucent;
		Super.InitChildWindow(this.StallGoodJinTuanWindow, Global.GetLang(string.Empty));
		this.StallGoodJinTuanWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
		{
			this.CloseStallGoodJinTuanPart();
			return true;
		};
		if (null == this.stallGoodJinTuanPart)
		{
			this.stallGoodJinTuanPart = U3DUtils.NEW<StallGoodJinTuanPart>();
			this.stallGoodJinTuanPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					this.CloseStallGoodJinTuanPart();
				}
				else if (e.ID == 1)
				{
					if (Global.GetZuanShi(ZuanShiPartClass.JiaoYiSuoJingTuanJingJia))
					{
						GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							Global.GetLang(string.Format(Global.GetLang("需要消耗{0}钻石，确定吗？"), e.NeedYuanBao))
						}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
						if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
						{
							messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
						}
						messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
							if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
							{
								Global.SetZuanShi(ZuanShiPartClass.JiaoYiSuoJingTuanJingJia, !messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
							}
							Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								GameInstance.Game.SendJinTuanJingJia(this.m_AuctionType, e.Title, e.NeedYuanBao);
								this.CloseStallGoodJinTuanPart();
							}
							return true;
						};
					}
					else
					{
						GameInstance.Game.SendJinTuanJingJia(this.m_AuctionType, e.Title, e.NeedYuanBao);
						this.CloseStallGoodJinTuanPart();
					}
				}
			};
			this.stallGoodJinTuanPart.RefreshByAuctionItemS2C(auctionItem);
			this.StallGoodJinTuanWindow.SetContent(this.StallGoodJinTuanWindow.BodyPresenter, this.stallGoodJinTuanPart, 0.0, 0.0, true);
			Super.GData.PlayZoneRoot.Children.Add(this.StallGoodJinTuanWindow);
		}
	}

	private void CloseStallGoodJinTuanPart()
	{
		if (null != this.StallGoodJinTuanWindow)
		{
			Super.CloseChildWindow(Super.GData.PlayZoneRoot, this.StallGoodJinTuanWindow);
			Object.Destroy(this.stallGoodJinTuanPart.gameObject);
			this.stallGoodJinTuanPart = null;
			this.StallGoodJinTuanWindow = null;
		}
	}

	public static ChangeableRulePart.RuleXml GetJinTuanHelpData()
	{
		if (BuyGoodsJinTuanPart.m_compJinTuanHelpData == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/AuctionHelp.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/AuctionHelp.xml 出现错误"
				});
				return null;
			}
			BuyGoodsJinTuanPart.m_compJinTuanHelpData = new ChangeableRulePart.RuleXml(gameResXml);
		}
		return BuyGoodsJinTuanPart.m_compJinTuanHelpData;
	}

	public void OpenHelpWindow()
	{
		ChangeableRulePart.RuleXml jinTuanHelpData = BuyGoodsJinTuanPart.GetJinTuanHelpData();
		if (jinTuanHelpData == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"未找到相关配置"
			});
			return;
		}
		if (this.m_helpWindow == null)
		{
			this.m_helpWindow = U3DUtils.NEW<GChildWindow>();
			this.m_helpWindow.IsShowModal = true;
			this.m_helpWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_helpWindow, Global.GetLang("帮助界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_helpWindow);
		}
		if (this.m_helpPart == null)
		{
			this.m_helpPart = U3DUtils.NEW<CommonHelpWindow>();
			this.m_helpPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseHelpWindow();
			};
		}
		this.m_helpWindow.SetContent(this.m_helpWindow.BodyPresenter, this.m_helpPart, 0.0, 0.0, true);
		this.m_helpPart.SetHelpInfo(jinTuanHelpData.list);
	}

	private void CloseHelpWindow()
	{
		if (null != this.m_helpPart)
		{
			this.m_helpPart.transform.parent = null;
			Object.Destroy(this.m_helpPart.gameObject);
			this.m_helpPart = null;
		}
		if (null != this.m_helpWindow)
		{
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.m_helpWindow);
			this.m_helpWindow = null;
		}
	}

	private const int EachPageCount = 10;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton m_BtnSearch;

	public GButton m_BtnHelp;

	public GButton m_BtnWorldSale;

	public GButton m_BtnZhanMengSale;

	public UIButton m_LeftArrow;

	public UIButton m_RightArrow;

	public UILabel m_TextPage;

	public GameObject PnlGoodItem;

	public GameObject m_SpriteOrderByTotalPrice;

	public GameObject m_SpriteOrderByPrice;

	public GameObject m_SpriteOrderByTime;

	private int m_currentPage = 1;

	private int m_totalPage;

	public ListBox m_ListGoodItem = new ListBox();

	private ObservableCollection m_ListGoodItemObC;

	private int SearchAllQualityValue = 15;

	private string SearchAllInputText = string.Empty;

	private int m_OrderType = 1;

	private int m_AuctionType = 2;

	private SearchOrderTypesJinTuan m_SearchOrderType;

	private GChildWindow SearchWindow;

	private BuyGoodsSearchPart SearchPart;

	private GChildWindow StallGoodJinTuanWindow;

	private StallGoodJinTuanPart stallGoodJinTuanPart;

	private static ChangeableRulePart.RuleXml m_compJinTuanHelpData;

	protected GChildWindow m_helpWindow;

	protected CommonHelpWindow m_helpPart;
}
