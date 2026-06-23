using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class SaleGoodsPart : UserControl
{
	public SaleGoodsPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 270.0;
		this.listBox.Height = 295.0;
		Canvas.SetLeft(this.listBox, 10);
		Canvas.SetTop(this.listBox, 68);
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

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
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
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("物品上架");
		gicon.Tip = "SaleGoodsOutBtn";
		gicon.TipType = 4;
		Canvas.SetLeft(gicon, 93);
		Canvas.SetTop(gicon, 381);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			(s as GIcon).Cursor = Cursors.Auto;
			ObjectClickGetingMgr.StartClickGetThing(2, e);
		};
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("物品下架");
		gicon.Tip = "SaleGoodsBackBtn";
		gicon.TipType = 4;
		Canvas.SetLeft(gicon, 179);
		Canvas.SetTop(gicon, 381);
		this.Container.Children.Add(gicon);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.IconMouseLeftButtonUp);
	}

	private void IconMouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (null == this.SelectedListItem)
		{
			return;
		}
		GoodsData gd = Global.GetSaleGoodsDataByDbID(this.SelectedListItem.GoodsDbID);
		if (gd.Site != -1)
		{
			return;
		}
		if (!Global.CanAddGoods(gd.GoodsID, gd.GCount, gd.Binding, gd.Endtime, false))
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再下架物品..."), new object[0]), 1, -1, -1, 0);
		}
		GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("确认要下架:【{0}】吗？"), new object[]
		{
			this.SelectedListItem.GoodsName
		}), ((int)this.Container.Width - 253) / 2, ((int)this.Container.Height - 171) / 2, (int)this.Container.Width, (int)this.Container.Height, 0.01, default(Vector3), null, null);
		messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
			Super.CloseMessageBox(this.Container, messageBoxWindow);
			if (messageBoxReturn == 0)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
				if (goodsXmlNodeByID != null && Global.IsRebornGood(goodsXmlNodeByID))
				{
					GameInstance.Game.SpriteSaleGoods(gd.Id, 15000, 0, 0, 0, -1);
					return true;
				}
				GameInstance.Game.SpriteSaleGoods(gd.Id, 0, 0, 0, 0, -1);
			}
			return true;
		};
	}

	public void InitPartData()
	{
		GameInstance.Game.SpriteSelfSaleGoodsList();
	}

	public override void Destroy()
	{
		ObjectClickGetingMgr.CancelClickGetThing(2);
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		if (this.listBox.SelectedIndex < 0)
		{
			this.UnSelectItem();
			return;
		}
		if (null != this.SelectedListItem)
		{
			this.SelectedListItem.ContainerBackground = null;
		}
		this.SelectedListItem = U3DUtils.AS<Mark3StallListItem>(this.listBox.SelectedItem);
		if (null == this.SelectedListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedListItem.ContainerBackground = this.SelectedListItemBakImg;
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

	private bool JugeCanAddGoods()
	{
		return Global.Data.SaleGoodsDataList == null || Global.Data.SaleGoodsDataList.Count < 16;
	}

	private void ClickGetThingNotify(ObjectClickEvent evt)
	{
		ClickGetThingEventArgs clickGetThingEventArgs = evt.Tag as ClickGetThingEventArgs;
		if (clickGetThingEventArgs.ClickGetThingType != 2)
		{
			return;
		}
		object sender = clickGetThingEventArgs.sender;
		if (sender is GIcon)
		{
			GIcon gicon = sender as GIcon;
			if (gicon.BoxTypes == 1)
			{
				GoodsData goodsData = gicon.ItemObject as GoodsData;
				if (goodsData.Binding <= 0 && !Global.IsTimeLimitGoods(goodsData))
				{
					if (goodsData.Site == 0 && this.JugeCanAddGoods())
					{
						this.ShowSaleOut(goodsData);
					}
				}
				else if (goodsData.Binding > 0)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("已经绑定的物品，不能出售"), 0, -1, -1, 0);
				}
				else if (Global.IsTimeLimitGoods(goodsData))
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("限时的物品，不能出售"), 0, -1, -1, 0);
				}
			}
		}
		clickGetThingEventArgs.NextClick = true;
	}

	public void ShowModalDialog()
	{
		this.PlaceHolder = new Canvas();
		this.PlaceHolder.Background = new SolidColorBrush(4286611584U);
		this.PlaceHolder.Opacity = 0.01;
		this.PlaceHolder.Width = this.Width;
		this.PlaceHolder.Height = this.Height;
		Canvas.SetZIndex(this.PlaceHolder, 9000.0);
		this.Root.Children.Add(this.PlaceHolder);
	}

	public void CloseModalDialog()
	{
		if (null != this.PlaceHolder)
		{
			this.PlaceHolder.Visibility = false;
			this.Root.Children.Remove(this.PlaceHolder, true);
			this.PlaceHolder = null;
		}
	}

	public void ShowSaleOut(GoodsData goodsData)
	{
		if (null != this.OnSaleWindow)
		{
			this.CloseChildWindow(this.OnSaleWindow);
			this.OnSaleWindow = null;
			return;
		}
		this.ShowModalDialog();
		this.OnSaleWindow = U3DUtils.NEW<GChildWindow>();
		this.OnSaleWindow.Left = (double)(Super.GetChildLeft(657, 308) - 183);
		this.OnSaleWindow.Top = (double)Super.GetChildTop(406, 286);
		this.OnSaleWindow.HeadLeft = 0.0;
		this.OnSaleWindow.HeadTop = 0.0;
		this.OnSaleWindow.HeadWidth = 308.0;
		this.OnSaleWindow.HeadHeight = 46.0;
		this.OnSaleWindow.BodyLeft = 0.0;
		this.OnSaleWindow.BodyTop = 46.0;
		this.OnSaleWindow.BodyWidth = 308.0;
		this.OnSaleWindow.BodyHeight = 134.0;
		Super.InitChildWindow1(this.OnSaleWindow, Global.GetLang("物品上架"));
		this.OnSaleWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(s as GChildWindow);
			this.OnSaleWindow = null;
			return true;
		};
		Canvas.SetZIndex(this.OnSaleWindow, 9001.0);
		this.Root.Children.Add(this.OnSaleWindow);
		MarkViewStallBuyPart2 markViewStallBuyPart2 = U3DUtils.NEW<MarkViewStallBuyPart2>();
		markViewStallBuyPart2.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/cktwgm_bak2.png"), false, 0);
		markViewStallBuyPart2.GoodDbID = goodsData.Id;
		markViewStallBuyPart2.InitPartSize((int)this.OnSaleWindow.BodyWidth - 18, (int)this.OnSaleWindow.BodyHeight - 9);
		markViewStallBuyPart2.InitPartData();
		markViewStallBuyPart2.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(markViewStallBuyPart2.GoodDbID, null);
			if (goodsDataByDbID != null)
			{
				if (goodsDataByDbID.Binding <= 0 && !Global.IsTimeLimitGoods(goodsDataByDbID))
				{
					if (goodsDataByDbID.Site == 0 && this.JugeCanAddGoods())
					{
						GameInstance.Game.SpriteSaleGoods(goodsDataByDbID.Id, -1, markViewStallBuyPart2.SaleMoney1, markViewStallBuyPart2.SaleYinPiao, 0, -1);
					}
				}
				else if (goodsDataByDbID.Binding > 0)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("已经绑定的物品，不能出售"), 0, -1, -1, 0);
				}
				else if (Global.IsTimeLimitGoods(goodsDataByDbID))
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("限时的物品，不能出售"), 0, -1, -1, 0);
				}
			}
			this.CloseModalDialog();
			this.CloseChildWindow(this.OnSaleWindow);
			this.OnSaleWindow = null;
		};
		this.OnSaleWindow.SetContent(this.OnSaleWindow.BodyPresenter, markViewStallBuyPart2, 9.0, 0.0, true);
	}

	private void CloseChildWindow(GChildWindow childWindow)
	{
		this.ChildWindowList.Remove(childWindow);
		Super.CloseChildWindow(this.Root, childWindow);
	}

	private void InitChildWindow(GChildWindow childWindow, string title, bool limitRange = false)
	{
		Super.InitChildWindow(childWindow, title);
		this.ChildWindowList.Add(childWindow);
	}

	private void AddIcon(GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 32.0;
		ggoodIcon.Height = 32.0;
		ggoodIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(goodsXmlNodeByID.IconCode, string.Empty), false, 0);
		ggoodIcon.TipType = 1;
		ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
		{
			goodsXmlNodeByID.ID,
			1,
			goodsData.Id,
			8
		});
		ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
		ggoodIcon.ItemCode = goodsData.GoodsID;
		ggoodIcon.ItemObject = goodsData;
		ggoodIcon.Text = ((goodsData.GCount > 1) ? goodsData.GCount.ToString() : string.Empty);
		ggoodIcon.TextHorizontalAlignment = global::Layout.Center;
		ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
		ggoodIcon.TextShadowColor = 4278190080U;
		ggoodIcon.TextColor = ColorSL.FromArgb(255, 58, 206, 0);
		Super.InitEquipGIcon(ggoodIcon, goodsData, false, IconTextTypes.Qianghua);
		Mark3StallListItem mark3StallListItem = U3DUtils.NEW<Mark3StallListItem>();
		mark3StallListItem.RootWidth = 250.0;
		mark3StallListItem.RootHeight = 52.0;
		mark3StallListItem.ContainerWidth = 250.0;
		mark3StallListItem.ContainerHeight = 52.0;
		mark3StallListItem.ContentWidth = 250.0;
		mark3StallListItem.ContentHeight = 52.0;
		mark3StallListItem.ContentBackground = new ImageURL(Global.GetGameResImageURL("Images/Plate/cktw_listItem.png"), false, 0);
		mark3StallListItem.GoodsName = goodsXmlNodeByID.Title;
		mark3StallListItem.GoodsPriceGold = goodsData.SaleMoney1.ToString();
		mark3StallListItem.GoodsPriceIngot = goodsData.SaleYuanBao.ToString();
		mark3StallListItem.GoodsID = goodsData.GoodsID;
		mark3StallListItem.GoodsDbID = goodsData.Id;
		this.ItemCollection.AddNoUpdate(mark3StallListItem);
	}

	public void RefreshData(GoodsData goodsData, bool toAdd = true)
	{
		if (goodsData == null)
		{
			this.ItemCollection.Clear();
			if (Global.Data.SaleGoodsDataList == null)
			{
				return;
			}
			for (int i = 0; i < Global.Data.SaleGoodsDataList.Count; i++)
			{
				this.AddIcon(Global.Data.SaleGoodsDataList[i]);
			}
			if (this.ItemCollection.Count > 0)
			{
				this.SelectListBox(this.listBox.SelectedIndex);
			}
		}
		else if (toAdd)
		{
			this.AddIcon(goodsData);
		}
		else
		{
			for (int j = 0; j < this.ItemCollection.Count; j++)
			{
				if (U3DUtils.AS<Mark3StallListItem>(this.ItemCollection[j]).GoodsDbID == goodsData.Id)
				{
					this.ItemCollection.RemoveAt(j);
					break;
				}
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	public void NotifyError(int result)
	{
	}

	public GChildWindow OnSaleWindow;

	private Mark3StallListItem SelectedListItem;

	private Canvas Root;

	private ListBox listBox = new ListBox();

	private ImageBrush SelectedListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 250.0, 52.0, 5.0, 5.0));

	private Canvas PlaceHolder;

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	private ObservableCollection _ItemCollection;
}
