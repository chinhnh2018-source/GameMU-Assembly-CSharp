using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class MarkItemListPart : UserControl
{
	public MarkItemListPart()
	{
		this.thisCtrl = this;
		this.ItemCollection = this.listBox.ItemsSource;
		this.Container.addEventListener("mouseDown", new MouseEventHandler(this.UserControl_MouseLeftButtonDown));
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 608.0;
		this.listBox.Height = 230.0;
		Canvas.SetLeft(this.listBox, 14);
		Canvas.SetTop(this.listBox, 68);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.BorderThickness = 0;
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 0.0, 3.0);
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.Container.Children.Add(this.Page);
		this.Page.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.Page, 290);
		Canvas.SetTop(this.Page, 345);
		this.Container.Children.Add(this.TotalPage);
		this.TotalPage.Text = "0";
		this.TotalPage.FontSize = HSTextField.defaultFontSize;
		this.TotalPage.Foreground = new SolidColorBrush(4285638580U);
		Canvas.SetLeft(this.TotalPage, 70);
		Canvas.SetTop(this.TotalPage, 347);
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

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.txtSearchSelectItem = U3DUtils.NEW<GTextBlock>();
		this.txtSearchSelectItem.BodyWidth = 75.0;
		this.txtSearchSelectItem.BodyHeight = 21.0;
		this.txtSearchSelectItem.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 75.0, 21.0, 3.0, 2.0));
		this.txtSearchSelectItem.Text.mouseEnabled = false;
		this.ProcessMenuClick(0);
		Canvas.SetLeft(this.txtSearchSelectItem, 86);
		Canvas.SetTop(this.txtSearchSelectItem, 310);
		this.Container.Children.Add(this.txtSearchSelectItem);
		this.txtSearchKeyword = U3DUtils.NEW<GTextBlock>();
		this.txtSearchKeyword.BodyWidth = 97.0;
		this.txtSearchKeyword.BodyHeight = 21.0;
		this.txtSearchKeyword.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 97.0, 21.0, 3.0, 2.0));
		this.txtSearchKeyword.TextForeground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtSearchKeyword, 265);
		Canvas.SetTop(this.txtSearchKeyword, 310);
		this.Container.Children.Add(this.txtSearchKeyword);
		this.SelectSearchTypeIcon = U3DUtils.NEW<GIcon>();
		this.SelectSearchTypeIcon.Width = 19.0;
		this.SelectSearchTypeIcon.Height = 19.0;
		this.SelectSearchTypeIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn6_normal.png"));
		this.SelectSearchTypeIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn6_hover.png"));
		Canvas.SetLeft(this.SelectSearchTypeIcon, 143);
		Canvas.SetTop(this.SelectSearchTypeIcon, 312);
		this.Container.Children.Add(this.SelectSearchTypeIcon);
		this.SelectSearchTypeIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Point position = new global::MousePosition(e).GetPosition(this.SelectSearchTypeIcon);
			this.ShowMenuWindow(position.X + 79, position.Y + 316, this.SearchTypeIDs, this.SearchTypeNames);
		};
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("搜索");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string actionName = StringUtil.substitute("MarkSearch", new object[0]);
			if (!ActionCoolDownMgr.FindAction(actionName, 3000L))
			{
				ActionCoolDownMgr.AddAction(actionName);
				this.SearchData();
			}
		};
		Canvas.SetLeft(gicon, 366);
		Canvas.SetTop(gicon, 310);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("充值");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.OpenChongZhiHtmlWindow();
		};
		Canvas.SetLeft(gicon, 527);
		Canvas.SetTop(gicon, 310);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("刷新");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearAllResult();
			this.SearchResultType = 0;
			GameInstance.Game.SpriteMarketGoodsList(0, string.Empty);
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.Container.Children.Add(this.LoadingWin);
		};
		Canvas.SetLeft(gicon, 90);
		Canvas.SetTop(gicon, 343);
		this.Container.Children.Add(gicon);
		this.FirstPageIcon = U3DUtils.NEW<GIcon>();
		this.FirstPageIcon.Width = 66.0;
		this.FirstPageIcon.Height = 21.0;
		this.FirstPageIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		this.FirstPageIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		this.FirstPageIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		this.FirstPageIcon.TextColor = new SolidColorBrush(10551295U);
		this.FirstPageIcon.Text = Global.GetLang("首页");
		this.FirstPageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.FirstPageIcon.EnableIcon)
			{
				this.FirstPage();
			}
		};
		Canvas.SetLeft(this.FirstPageIcon, 197);
		Canvas.SetTop(this.FirstPageIcon, 343);
		this.Container.Children.Add(this.FirstPageIcon);
		this.PrePageIcon = U3DUtils.NEW<GIcon>();
		this.PrePageIcon.Width = 14.0;
		this.PrePageIcon.Height = 18.0;
		this.PrePageIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn2_normal.png"));
		this.PrePageIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn2_hover.png"));
		this.PrePageIcon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn2_nouse.png"));
		this.PrePageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.PrePageIcon.EnableIcon)
			{
				this.PrevPage();
			}
		};
		Canvas.SetLeft(this.PrePageIcon, 269);
		Canvas.SetTop(this.PrePageIcon, 343);
		this.Container.Children.Add(this.PrePageIcon);
		this.NextPageIcon = U3DUtils.NEW<GIcon>();
		this.NextPageIcon.Width = 14.0;
		this.NextPageIcon.Height = 18.0;
		this.NextPageIcon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn3_normal.png"));
		this.NextPageIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn3_hover.png"));
		this.NextPageIcon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn3_nouse.png"));
		this.NextPageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.NextPageIcon.EnableIcon)
			{
				this.NextPage();
			}
		};
		Canvas.SetLeft(this.NextPageIcon, 329);
		Canvas.SetTop(this.NextPageIcon, 343);
		this.Container.Children.Add(this.NextPageIcon);
		this.EndPageIcon = U3DUtils.NEW<GIcon>();
		this.EndPageIcon.Width = 66.0;
		this.EndPageIcon.Height = 21.0;
		this.EndPageIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		this.EndPageIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		this.EndPageIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		this.EndPageIcon.TextColor = new SolidColorBrush(10551295U);
		this.EndPageIcon.Text = Global.GetLang("尾页");
		this.EndPageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.EndPageIcon.EnableIcon)
			{
				this.EndPage();
				return;
			}
		};
		Canvas.SetLeft(this.EndPageIcon, 351);
		Canvas.SetTop(this.EndPageIcon, 343);
		this.Container.Children.Add(this.EndPageIcon);
		this.ViewStallIcon = U3DUtils.NEW<GIcon>();
		this.ViewStallIcon.Width = 81.0;
		this.ViewStallIcon.Height = 21.0;
		this.ViewStallIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		this.ViewStallIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		this.ViewStallIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		this.ViewStallIcon.TextColor = new SolidColorBrush(10551295U);
		this.ViewStallIcon.Text = Global.GetLang("查看摊位");
		this.ViewStallIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ViewStallIcon.EnableIcon)
			{
				this.ViewStall();
			}
		};
		Canvas.SetLeft(this.ViewStallIcon, 442);
		Canvas.SetTop(this.ViewStallIcon, 343);
		this.Container.Children.Add(this.ViewStallIcon);
		this.ViewInfoIcon = U3DUtils.NEW<GIcon>();
		this.ViewInfoIcon.Width = 81.0;
		this.ViewInfoIcon.Height = 21.0;
		this.ViewInfoIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		this.ViewInfoIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		this.ViewInfoIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		this.ViewInfoIcon.TextColor = new SolidColorBrush(10551295U);
		this.ViewInfoIcon.Text = Global.GetLang("查看信息");
		this.ViewInfoIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (null != this.SelectedListItem && this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = this.SelectedListItem.OwnerRoleID,
					IDType = 1
				});
			}
		};
		Canvas.SetLeft(this.ViewInfoIcon, 527);
		Canvas.SetTop(this.ViewInfoIcon, 343);
		this.Container.Children.Add(this.ViewInfoIcon);
	}

	public void SearchData()
	{
		string text = Global.StringTrim(this.txtSearchKeyword.EditText);
		if (text.Length < 2)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("要搜索的关键字不能少于2个字"), 0, -1, -1, 0);
			return;
		}
		if (text.Length > 10)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("要搜索的关键字不能超过10个字"), 0, -1, -1, 0);
			return;
		}
		if (this.SelectedMenuItemID == 0)
		{
			string text2 = ConfigGoods.FindGoodsFilterByName(text);
			if (string.IsNullOrEmpty(text2))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("没有找到匹配[{0}]关键词的物品"), new object[]
				{
					text
				}), 0, -1, -1, 0);
				return;
			}
			this.ClearAllResult();
			this.SearchResultType = 1;
			GameInstance.Game.SpriteMarketGoodsList(2, text2);
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.Container.Children.Add(this.LoadingWin);
		}
		else
		{
			this.ClearAllResult();
			this.SearchResultType = 1;
			GameInstance.Game.SpriteMarketGoodsList(1, text);
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.Container.Children.Add(this.LoadingWin);
		}
	}

	public void InitPartData()
	{
		if (this.FirstInitPartData)
		{
			this.FirstInitPartData = false;
		}
	}

	public void ResetGetNewData()
	{
		this.FirstGetNewData = true;
	}

	public void GetNewData(int searchGoodsID = -1)
	{
		if (!this.FirstGetNewData)
		{
			if (searchGoodsID != -1)
			{
				this.Search(searchGoodsID);
			}
			return;
		}
		this.FirstGetNewData = false;
		if (searchGoodsID == -1)
		{
			this.SearchResultType = 0;
			GameInstance.Game.SpriteMarketGoodsList(0, string.Empty);
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.Container.Children.Add(this.LoadingWin);
		}
		else
		{
			this.Search(searchGoodsID);
		}
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Container);
	}

	public void RefreshData(List<SaleGoodsData> saleGoodsDataList)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.SaleGoodsDataList = saleGoodsDataList;
		if ((this.SaleGoodsDataList == null || this.SaleGoodsDataList.Count <= 0) && this.SearchResultType == 1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("没有找到要出售的物品"), new object[0]), 0, -1, -1, 0);
		}
		this.SearchResultType = -1;
		this.ShowGoodsList();
		if (this.SaleGoodsDataList == null || this.SaleGoodsDataList.Count <= 0)
		{
			this.SetBtnState(false);
		}
		else
		{
			this.SetBtnState(true);
			this.CurrentSelectedPage = 0;
			this.MaxPageCount = (this.ItemsList.Count - 1) / this.PageSize + 1;
			this.ShowPage(this.CurrentSelectedPage);
			this.TotalPage.Text = this.MaxPageCount.ToString();
		}
	}

	public void DeleteSaleGoodsData(int goodsDbID)
	{
		if (this.SaleGoodsDataList == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < this.SaleGoodsDataList.Count; i++)
		{
			if (this.SaleGoodsDataList[i].GoodsDbID == goodsDbID)
			{
				flag = true;
				this.SaleGoodsDataList.RemoveRange(i, 1);
				if (null != this.ItemsList[i])
				{
					this.ItemsList[i].MouseLeftButtonUp = null;
				}
				this.ItemsList.RemoveRange(i, 1);
				break;
			}
		}
		if (flag)
		{
			for (int j = 0; j < this.ItemCollection.Count; j++)
			{
				if (U3DUtils.AS<Mark3ListItem>(this.ItemCollection[j]).GoodsDbID == goodsDbID)
				{
					this.ItemCollection.RemoveAt(j);
					break;
				}
			}
			this.MaxPageCount = (this.ItemsList.Count - 1) / this.PageSize + 1;
			this.CurrentSelectedPage = Global.GMin(this.CurrentSelectedPage, this.MaxPageCount - 1);
			this.ShowPage(this.CurrentSelectedPage);
			this.TotalPage.Text = this.MaxPageCount.ToString();
		}
	}

	public void RefreshOtherSaleGoodsDataList()
	{
		if (null != this.markViewStallPart)
		{
			this.markViewStallPart.RefreshData();
		}
	}

	private void ShowGoodsList()
	{
		this.ItemsList = new List<Mark3ListItem>();
		if (this.SaleGoodsDataList == null)
		{
			return;
		}
		for (int i = 0; i < this.SaleGoodsDataList.Count; i++)
		{
			this.ItemsList.Add(null);
		}
	}

	private void SetBtnState(bool state)
	{
		this.ViewStallIcon.EnableIcon = state;
		this.ViewInfoIcon.EnableIcon = state;
		this.PrePageIcon.EnableIcon = state;
		this.NextPageIcon.EnableIcon = state;
		this.FirstPageIcon.EnableIcon = state;
		this.EndPageIcon.EnableIcon = state;
	}

	private void ShowPage(int pageIndex)
	{
		this.Page.Text = StringUtil.substitute("{0}/{1}", new object[]
		{
			pageIndex + 1,
			this.MaxPageCount
		});
		this.ItemCollection.Clear();
		int num = pageIndex * this.PageSize;
		int num2 = num;
		while (num2 < this.ItemsList.Count && num2 < num + this.PageSize)
		{
			if (null == this.ItemsList[num2])
			{
				GoodsData salingGoodsData = this.SaleGoodsDataList[num2].SalingGoodsData;
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(salingGoodsData.GoodsID);
				GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 32.0;
				ggoodIcon.Height = 32.0;
				ggoodIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(goodsXmlNodeByID.IconCode, string.Empty), false, 0);
				ggoodIcon.TipType = 1;
				ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
				{
					goodsXmlNodeByID.ID,
					0,
					salingGoodsData.Id,
					10
				});
				ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
				ggoodIcon.ItemCode = salingGoodsData.GoodsID;
				ggoodIcon.ItemObject = salingGoodsData;
				ggoodIcon.BoxTypes = -1;
				Super.InitGoodsGIcon(ggoodIcon, salingGoodsData, true, IconTextTypes.Qianghua);
				GIcon gicon = U3DUtils.NEW<GIcon>();
				gicon.Width = 81.0;
				gicon.Height = 21.0;
				gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
				gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
				gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
				gicon.TextColor = new SolidColorBrush(10551295U);
				gicon.Text = Global.GetLang("购买");
				gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
				gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					int num3 = this.ItemsList.IndexOf((s as GIcon).Tag as Mark3ListItem);
					if (num3 >= 0)
					{
						this.SelectListBox(num3);
					}
					this.ShowBuy();
				};
				gicon.MouseLeftButtonDown = delegate(object s, MouseEvent e)
				{
				};
				Mark3ListItem mark3ListItem = U3DUtils.NEW<Mark3ListItem>();
				gicon.Tag = mark3ListItem;
				mark3ListItem.BodyWidth = 602.0;
				mark3ListItem.BodyHeight = 42.0;
				mark3ListItem.Width = 602.0;
				mark3ListItem.Height = 42.0;
				mark3ListItem.ContentBackground = new ImageBrush(Global.GetGameResImage("Images/Plate/rm_listItem.png"));
				mark3ListItem.ContentWidth = 40.0;
				mark3ListItem.ContentHeight = 40.0;
				mark3ListItem.Goodsdouble = (num2 + 1).ToString();
				mark3ListItem.GoodImg = ggoodIcon;
				mark3ListItem.GoodsName = Global.GetGoodsNameByID(salingGoodsData.GoodsID, false);
				mark3ListItem.GoodsPriceGold = salingGoodsData.SaleMoney1.ToString();
				mark3ListItem.GoodsPriceIngot = salingGoodsData.SaleYuanBao.ToString();
				mark3ListItem.GoodsSellerID = this.SaleGoodsDataList[num2].RoleID;
				mark3ListItem.GoodsSellerName = this.SaleGoodsDataList[num2].RoleName;
				mark3ListItem.GoodsSellerLevel = this.SaleGoodsDataList[num2].RoleLevel.ToString();
				mark3ListItem.GoodsBuyBtn = gicon;
				mark3ListItem.GoodsID = salingGoodsData.GoodsID.ToString();
				mark3ListItem.GoodsDbID = this.SaleGoodsDataList[num2].GoodsDbID;
				mark3ListItem.OwnerRoleID = this.SaleGoodsDataList[num2].RoleID;
				mark3ListItem.GoodsCount = salingGoodsData.GCount;
				mark3ListItem.addEventListener("mouseUp", new MouseEventHandler(this.ItemMouseLeftButtonUp));
				this.ItemsList[num2] = mark3ListItem;
			}
			this.ItemCollection.AddNoUpdate(this.ItemsList[num2]);
			num2++;
		}
		this.ItemCollection.DelayUpdate();
		if (pageIndex <= 0)
		{
			this.PrePageIcon.EnableIcon = false;
			this.FirstPageIcon.EnableIcon = false;
		}
		else
		{
			this.PrePageIcon.EnableIcon = true;
			this.FirstPageIcon.EnableIcon = true;
		}
		if (pageIndex >= this.MaxPageCount - 1)
		{
			this.NextPageIcon.EnableIcon = false;
			this.EndPageIcon.EnableIcon = false;
		}
		else
		{
			this.NextPageIcon.EnableIcon = true;
			this.EndPageIcon.EnableIcon = true;
		}
		int oldSelectedIndex = 0;
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = this.listBox.SelectedIndex;
		}
		this.SelectListBox(oldSelectedIndex);
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

	private void NextPage()
	{
		if (this.CurrentSelectedPage < this.MaxPageCount - 1)
		{
			this.CurrentSelectedPage++;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	private void PrevPage()
	{
		if (this.CurrentSelectedPage > 0)
		{
			this.CurrentSelectedPage--;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	private void FirstPage()
	{
		this.CurrentSelectedPage = 0;
		this.ShowPage(this.CurrentSelectedPage);
	}

	private void EndPage()
	{
		this.CurrentSelectedPage = this.MaxPageCount - 1;
		this.ShowPage(this.CurrentSelectedPage);
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
			this.SelectedListItem.BodyBackground = null;
		}
		this.SelectedListItem = U3DUtils.AS<Mark3ListItem>(this.listBox.SelectedItem);
		if (null == this.SelectedListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedListItem.BodyBackground = this.SelectedListItemBakImg;
		this.SelectedListItem.BodyWidth = 602.0;
		this.SelectedListItem.BodyHeight = 40.0;
	}

	private void UnSelectItem()
	{
		this.SelectedListItem = null;
	}

	private void SelectListBox(int oldSelectedIndex)
	{
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = Global.GMode(oldSelectedIndex, this.PageSize);
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

	private void InitChildWindow(GChildWindow childWindow, string title, bool limitRange = false)
	{
		Super.InitChildWindow(childWindow, title);
		this.ChildWindowList.Add(childWindow);
	}

	private void InitChildWindow1(GChildWindow childWindow, string title)
	{
		Super.InitChildWindow1(childWindow, title);
		this.ChildWindowList.Add(childWindow);
	}

	private void CloseNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.CloseNoBorderWindow(this.Container, noBorderWindow);
	}

	private void InitNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.InitNoBorderWindow(noBorderWindow);
	}

	private void ViewStall()
	{
		if (null == this.SelectedListItem)
		{
			return;
		}
		if (null != this.ViewStallWindow)
		{
			this.CloseChildWindow(this.ViewStallWindow);
			this.ViewStallWindow = null;
			if (null != this.markViewStallPart)
			{
				this.markViewStallPart.Destroy();
				this.markViewStallPart = null;
				Global.Data.OtherSaleGoodsDataList = null;
			}
			return;
		}
		this.ShowModalDialog();
		this.ViewStallWindow = U3DUtils.NEW<GChildWindow>();
		this.ViewStallWindow.Left = (double)Super.GetChildLeft(648, 308);
		this.ViewStallWindow.Top = (double)(Super.GetChildTop(467, 522) - 40);
		this.ViewStallWindow.HeadLeft = 0.0;
		this.ViewStallWindow.HeadTop = 0.0;
		this.ViewStallWindow.HeadWidth = 308.0;
		this.ViewStallWindow.HeadHeight = 46.0;
		this.ViewStallWindow.BodyLeft = 0.0;
		this.ViewStallWindow.BodyTop = 46.0;
		this.ViewStallWindow.BodyWidth = 308.0;
		this.ViewStallWindow.BodyHeight = 422.0;
		this.InitChildWindow1(this.ViewStallWindow, StringUtil.substitute(Global.GetLang("{0}的摊位"), new object[]
		{
			this.SelectedListItem.txtSellerName.Text
		}));
		this.ViewStallWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(s as GChildWindow);
			this.ViewStallWindow = null;
			if (null != this.markViewStallPart)
			{
				this.markViewStallPart.Destroy();
				this.markViewStallPart = null;
				Global.Data.OtherSaleGoodsDataList = null;
			}
			return true;
		};
		Canvas.SetZIndex(this.ViewStallWindow, 9001.0);
		this.Container.Children.Add(this.ViewStallWindow);
		this.markViewStallPart = U3DUtils.NEW<MarkViewStallPart>();
		this.markViewStallPart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/cktw_bak.png"), false, 0);
		this.markViewStallPart.OtherRoleID = this.SelectedListItem.GoodsSellerID;
		this.markViewStallPart.InitPartSize((int)this.ViewStallWindow.BodyWidth - 18, (int)this.ViewStallWindow.BodyHeight - 9);
		this.markViewStallPart.InitPartData();
		this.ViewStallWindow.SetContent(this.ViewStallWindow.BodyPresenter, this.markViewStallPart, 9.0, 0.0, true);
	}

	public void ShowMenuWindow(int x, int y, int[] ids, string[] names)
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		}
		this.MenuWindow = U3DUtils.NEW<NoBorderWindow>();
		this.MenuWindow.BodyLeft = 0.0;
		this.MenuWindow.BodyTop = 0.0;
		this.MenuWindow.BodyWidth = 120.0;
		this.MenuWindow.BodyHeight = (double)((ids.Length + 1) * 21);
		this.MenuWindow.BodyBackBrush = new SolidColorBrush(1185560U);
		this.MenuWindow.BodyBackOpacity = 0.9;
		this.MenuWindow.Left = (double)x;
		this.MenuWindow.Top = (double)y;
		this.InitNoBorderWindow(this.MenuWindow);
		this.Container.Children.Add(this.MenuWindow);
		this.menuPart = U3DUtils.NEW<GMenuPart>();
		this.menuPart.InitPartSize((int)this.MenuWindow.BodyWidth - 4, (int)this.MenuWindow.BodyHeight - 4);
		string imageFileName = "Images/Plate/menu_item_unselected.png";
		for (int i = 0; i < ids.Length; i++)
		{
			this.menuPart.AddMenuItem(ids[i], imageFileName, names[i], null);
		}
		this.menuPart.RenderMenu(21);
		this.menuPart.MenuItemClick = delegate(object s, EventArgs e)
		{
			GMenuItem gmenuItem = s as GMenuItem;
			if (null == gmenuItem)
			{
				return;
			}
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.ProcessMenuClick(gmenuItem.MenuItemID);
		};
		this.MenuWindow.SetContent(this.MenuWindow.BodyPresenter, this.menuPart, 2.0, 2.0);
	}

	private void ProcessMenuClick(int id)
	{
		this.SelectedMenuItemID = id;
		this.txtSearchSelectItem.EditText = this.SearchTypeNames[id];
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
		this.BuyWindow.Left = (double)Super.GetChildLeft(657, 308);
		this.BuyWindow.Top = (double)Super.GetChildTop(406, 286);
		this.BuyWindow.HeadLeft = 0.0;
		this.BuyWindow.HeadTop = 0.0;
		this.BuyWindow.HeadWidth = 308.0;
		this.BuyWindow.HeadHeight = 46.0;
		this.BuyWindow.BodyLeft = 0.0;
		this.BuyWindow.BodyTop = 46.0;
		this.BuyWindow.BodyWidth = 308.0;
		this.BuyWindow.BodyHeight = 134.0;
		this.InitChildWindow1(this.BuyWindow, Global.GetLang("购买"));
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
		markViewStallBuyPart.GoodsID = Convert.ToInt32(this.SelectedListItem.GoodsID);
		markViewStallBuyPart.GoodDbID = this.SelectedListItem.GoodsDbID;
		markViewStallBuyPart.GoodCount = this.SelectedListItem.GoodsCount;
		markViewStallBuyPart.SaleMoney = this.SelectedListItem.GoodsPriceGold;
		markViewStallBuyPart.SaleYuanBao = this.SelectedListItem.GoodsPriceIngot;
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

	private void Search(int id)
	{
		string goodsNameByID = Global.GetGoodsNameByID(id, false);
		this.txtSearchKeyword.Text.Text = goodsNameByID;
		this.ProcessMenuClick(0);
		this.ClearAllResult();
		this.SearchResultType = 1;
		GameInstance.Game.SpriteMarketGoodsList(2, id.ToString());
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	private void ClearAllResult()
	{
		this.SaleGoodsDataList = null;
		if (this.ItemsList != null)
		{
			for (int i = 0; i < this.ItemsList.Count; i++)
			{
				if (!(null == this.ItemsList[i]))
				{
					this.ItemsList[i].MouseLeftButtonUp = null;
				}
			}
			this.ItemsList.Clear();
		}
		if (this.ItemCollection != null)
		{
			this.ItemCollection.Clear();
		}
	}

	private void UserControl_MouseLeftButtonDown(MouseEvent e)
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		}
	}

	private UserControl thisCtrl;

	private ListBox listBox = new ListBox();

	private GTextBlockOutLine Page = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine TotalPage = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public DPSelectedItemEventHandler DPSelectedItem;

	private int PageSize = 5;

	private int SearchResultType = -1;

	private List<Mark3ListItem> ItemsList;

	private LoadingWindow LoadingWin;

	public GChildWindow ViewStallWindow;

	private int CurrentSelectedPage;

	private int MaxPageCount;

	private GTextBlock txtSearchKeyword;

	private GTextBlock txtSearchSelectItem;

	private GIcon PrePageIcon;

	private GIcon NextPageIcon;

	private GIcon FirstPageIcon;

	private GIcon EndPageIcon;

	private GIcon ViewStallIcon;

	private GIcon ViewInfoIcon;

	private GIcon SelectSearchTypeIcon;

	private int SelectedMenuItemID;

	private GMenuPart menuPart;

	private NoBorderWindow MenuWindow;

	private int[] SearchTypeIDs = new int[]
	{
		default(int),
		1
	};

	private string[] SearchTypeNames = new string[]
	{
		Global.GetLang("物品名称"),
		Global.GetLang("人物名称")
	};

	public GChildWindow BuyWindow;

	private bool FirstInitPartData = true;

	private bool FirstGetNewData = true;

	private List<SaleGoodsData> SaleGoodsDataList = new List<SaleGoodsData>();

	private double LastMouseLeftButtonDownTicks;

	private Mark3ListItem SelectedListItem;

	private ImageBrush SelectedListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 602.0, 42.0, 5.0, 5.0));

	private Canvas PlaceHolder;

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	private MarkViewStallPart markViewStallPart;

	private ObservableCollection _ItemCollection;
}
