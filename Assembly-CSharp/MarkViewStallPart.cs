using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class MarkViewStallPart : UserControl
{
	public MarkViewStallPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 270.0;
		this.listBox.Height = 295.0;
		Canvas.SetLeft(this.listBox, 10);
		Canvas.SetTop(this.listBox, 68);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.BorderThickness = 0;
		this.listBox.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
	}

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

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public int OtherRoleID
	{
		get
		{
			return this._OtherRoleID;
		}
		set
		{
			this._OtherRoleID = value;
		}
	}

	public override void Destroy()
	{
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("购买");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowBuy();
		};
		Canvas.SetLeft(gicon, 179);
		Canvas.SetTop(gicon, 381);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData()
	{
		GameInstance.Game.SpriteOtherSaleGoodsList(this.OtherRoleID);
	}

	public void ShowBuy()
	{
		if (null == this.SelectedListItem)
		{
			return;
		}
		if (null != this.BuyWindow)
		{
			this.CloseChildWindow(this.BuyWindow);
			this.BuyWindow = null;
			return;
		}
		this.ShowModalDialog();
		this.BuyWindow = U3DUtils.NEW<GChildWindow>();
		this.BuyWindow.Left = (double)Super.GetChildLeft((int)this.Container.Width, 308);
		this.BuyWindow.Top = (double)(Super.GetChildTop((int)this.Container.Height, 180) - 100);
		this.BuyWindow.HeadLeft = 0.0;
		this.BuyWindow.HeadTop = 0.0;
		this.BuyWindow.HeadWidth = 308.0;
		this.BuyWindow.HeadHeight = 46.0;
		this.BuyWindow.BodyLeft = 0.0;
		this.BuyWindow.BodyTop = 46.0;
		this.BuyWindow.BodyWidth = 308.0;
		this.BuyWindow.BodyHeight = 134.0;
		this.InitChildWindow1(this.BuyWindow, Global.GetLang("购买"), false);
		this.BuyWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(s as GChildWindow);
			this.BuyWindow = null;
			return true;
		};
		Canvas.SetZIndex(this.BuyWindow, 9001.0);
		this.Container.Children.Add(this.BuyWindow);
		MarkViewStallBuyPart markViewStallBuyPart = U3DUtils.NEW<MarkViewStallBuyPart>();
		markViewStallBuyPart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/cktwgm_bak.png"), false, 0);
		markViewStallBuyPart.OwnerRoleID = this.SelectedListItem.OwnerRoleID;
		markViewStallBuyPart.GoodsID = this.SelectedListItem.GoodsID;
		markViewStallBuyPart.GoodDbID = this.SelectedListItem.GoodsDbID;
		markViewStallBuyPart.SaleMoney = this.SelectedListItem.GoodsPriceGold;
		markViewStallBuyPart.SaleYuanBao = this.SelectedListItem.GoodsPriceIngot;
		markViewStallBuyPart.GoodCount = this.SelectedListItem.GoodsCount;
		markViewStallBuyPart.InitPartSize((int)this.BuyWindow.BodyWidth - 18, (int)this.BuyWindow.BodyHeight - 9);
		markViewStallBuyPart.InitPartData();
		markViewStallBuyPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (markViewStallBuyPart.OwnerRoleID != Global.Data.roleData.RoleID)
			{
				GameInstance.Game.SpriteMarketBuyGoods(markViewStallBuyPart.GoodDbID, markViewStallBuyPart.GoodsID);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("不能购买自己的物品"), 0, -1, -1, 0);
			}
			this.CloseModalDialog();
			this.CloseChildWindow(this.BuyWindow);
			this.BuyWindow = null;
		};
		this.BuyWindow.SetContent(this.BuyWindow.BodyPresenter, markViewStallBuyPart, 9.0, 0.0, true);
	}

	private void listBox_SelectionChanged(object sender, object e)
	{
		if (this.listBox.SelectedIndex < 0)
		{
			this.UnSelectItem();
			return;
		}
		if (null != this.SelectedListItem)
		{
			this.SelectedListItem.Container.Background = null;
		}
		this.SelectedListItem = U3DUtils.AS<Mark3StallListItem>(this.listBox.SelectedItem);
		if (null == this.SelectedListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedListItem.Container.Background = this.SelectedListItemBakImg;
	}

	private void UnSelectItem()
	{
		this.SelectedListItem = null;
	}

	private void SelectListBox(int oldSelectedIndex)
	{
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = Global.GMin(oldSelectedIndex, this.ItemCollection.Count);
			int num = oldSelectedIndex;
			if (num < 0)
			{
				num = 0;
			}
			this.listBox.SelectedIndex = num;
		}
		else
		{
			this.UnSelectItem();
		}
	}

	public void ShowModalDialog()
	{
		this.PlaceHolder = new Canvas();
		this.PlaceHolder.Background = new SolidColorBrush(4286611584U);
		this.PlaceHolder.Opacity = 0.01;
		this.PlaceHolder.Width = this.Width;
		this.PlaceHolder.Height = this.Height;
		Canvas.SetZIndex(this.PlaceHolder, 9000.0);
		this.Container.Children.Add(this.PlaceHolder);
	}

	public void CloseModalDialog()
	{
		if (null != this.PlaceHolder)
		{
			this.PlaceHolder.Visibility = false;
			this.Container.Children.Remove(this.PlaceHolder, true);
			this.PlaceHolder = null;
		}
	}

	private void CloseChildWindow(GChildWindow childWindow)
	{
		this.ChildWindowList.Remove(childWindow);
		Super.CloseChildWindow(this.Container, childWindow);
	}

	private void InitChildWindow1(GChildWindow childWindow, string title, bool limitRange = false)
	{
		Super.InitChildWindow1(childWindow, title);
		this.ChildWindowList.Add(childWindow);
	}

	public void RefreshData()
	{
		this.ItemCollection.Clear();
		if (Global.Data.OtherSaleGoodsDataList == null)
		{
			return;
		}
		for (int i = 0; i < Global.Data.OtherSaleGoodsDataList.Count; i++)
		{
			GoodsData goodsData = Global.Data.OtherSaleGoodsDataList[i];
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 32.0;
			ggoodIcon.Height = 32.0;
			ggoodIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(goodsXmlNodeByID.IconCode, string.Empty), false, 0);
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsXmlNodeByID.ID,
				0,
				goodsData.Id,
				9
			});
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = goodsData.GoodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.BoxTypes = -1;
			Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
			Mark3StallListItem mark3StallListItem = U3DUtils.NEW<Mark3StallListItem>();
			mark3StallListItem.RootWidth = 250.0;
			mark3StallListItem.RootHeight = 52.0;
			mark3StallListItem.ContainerWidth = 250.0;
			mark3StallListItem.ContainerHeight = 52.0;
			mark3StallListItem.ContentWidth = 250.0;
			mark3StallListItem.ContentHeight = 52.0;
			mark3StallListItem.ContentBackground = new ImageURL(Global.GetGameResImageURL("Images/Plate/cktw_listItem.png"), false, 0);
			mark3StallListItem.GoodImg = ggoodIcon;
			mark3StallListItem.GoodsName = goodsXmlNodeByID.Title;
			mark3StallListItem.GoodsPriceGold = goodsData.SaleMoney1.ToString();
			mark3StallListItem.GoodsPriceIngot = goodsData.SaleYuanBao.ToString();
			mark3StallListItem.GoodsID = goodsData.GoodsID;
			mark3StallListItem.GoodsDbID = goodsData.Id;
			mark3StallListItem.OwnerRoleID = this.OtherRoleID;
			mark3StallListItem.GoodsCount = goodsData.GCount;
			mark3StallListItem.addEventListener("mouseUp", new MouseEventHandler(this.ItemMouseLeftButtonUp));
			this.ItemCollection.AddNoUpdate(mark3StallListItem);
		}
		this.ItemCollection.DelayUpdate();
		if (this.ItemCollection.Count > 0)
		{
			this.SelectListBox(this.listBox.SelectedIndex);
		}
	}

	private void ItemMouseLeftButtonUp(MouseEvent e)
	{
		double num = (double)Global.GetCorrectLocalTime();
		if (num - this.LastMouseLeftButtonDownTicks < 300.0)
		{
			this.MouseLeftButtonDoubleClick(this, e);
			return;
		}
		this.LastMouseLeftButtonDownTicks = num;
	}

	private void MouseLeftButtonDoubleClick(object sender, MouseEvent e)
	{
		this.ShowBuy();
	}

	private ListBox listBox = new ListBox();

	public GChildWindow BuyWindow;

	private Mark3StallListItem SelectedListItem;

	private ImageBrush SelectedListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 250.0, 52.0, 5.0, 5.0));

	private Canvas PlaceHolder;

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	private double LastMouseLeftButtonDownTicks;

	private ObservableCollection _ItemCollection;

	private int _OtherRoleID;
}
