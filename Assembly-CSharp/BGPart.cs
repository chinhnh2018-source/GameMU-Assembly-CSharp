using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class BGPart : UserControl
{
	public BGPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 598.0;
		this.listBox.Height = 160.0;
		Canvas.SetLeft(this.listBox, 16);
		Canvas.SetTop(this.listBox, 192);
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 0.0, 3.0);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.BorderThickness = 0;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.Container.Children.Add(this.IcoImg_1);
		this.IcoImg_1.Width = 32.0;
		this.IcoImg_1.Height = 32.0;
		Canvas.SetLeft(this.IcoImg_1, 46);
		Canvas.SetTop(this.IcoImg_1, 67);
		this.Container.Children.Add(this.IcoImg_2);
		this.IcoImg_2.Width = 32.0;
		this.IcoImg_2.Height = 32.0;
		Canvas.SetLeft(this.IcoImg_2, 172);
		Canvas.SetTop(this.IcoImg_2, 67);
		this.Container.Children.Add(this.IcoImg_3);
		this.IcoImg_3.Width = 32.0;
		this.IcoImg_3.Height = 32.0;
		Canvas.SetLeft(this.IcoImg_3, 214);
		Canvas.SetTop(this.IcoImg_3, 67);
		this.Container.Children.Add(this.IcoImg_4);
		this.IcoImg_4.Width = 32.0;
		this.IcoImg_4.Height = 32.0;
		Canvas.SetLeft(this.IcoImg_4, 256);
		Canvas.SetTop(this.IcoImg_4, 67);
		this.Container.Children.Add(this.IcoImg_5);
		this.IcoImg_5.Width = 32.0;
		this.IcoImg_5.Height = 32.0;
		Canvas.SetLeft(this.IcoImg_5, 298);
		Canvas.SetTop(this.IcoImg_5, 67);
		this.Container.Children.Add(this.txtTQNum);
		this.txtTQNum.TextColor = new SolidColorBrush(4291467815U);
		Canvas.SetLeft(this.txtTQNum, 476);
		Canvas.SetTop(this.txtTQNum, 55);
		this.Container.Children.Add(this.txtPage);
		this.txtPage.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtPage, 340);
		Canvas.SetTop(this.txtPage, 364);
		this.Container.Children.Add(this.txtSYBG);
		this.txtSYBG.TextColor = new SolidColorBrush(4278236930U);
		Canvas.SetLeft(this.txtSYBG, 146);
		Canvas.SetTop(this.txtSYBG, 364);
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
		this.GxtqIcon = U3DUtils.NEW<GIcon>();
		this.GxtqIcon.Width = 81.0;
		this.GxtqIcon.Height = 21.0;
		this.GxtqIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		this.GxtqIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		this.GxtqIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		this.GxtqIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.GxtqIcon.Text = Global.GetLang("贡献金币");
		this.GxtqIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.MyBangHuiDetailData == null)
			{
				return;
			}
			if (this.MyBangHuiDetailData.BHID != Global.Data.roleData.Faction)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("非【{0}】战盟成员, 无法贡献金币!"), new object[]
				{
					Global.FormatBangHuiName(this.MyBangHuiDetailData.ZoneID, this.MyBangHuiDetailData.BHName)
				}), 0, -1, -1, 0);
				return;
			}
			this.DonateMoney();
		};
		Canvas.SetLeft(this.GxtqIcon, 380);
		Canvas.SetTop(this.GxtqIcon, 77);
		this.Container.Children.Add(this.GxtqIcon);
		this.GxdjIcon = U3DUtils.NEW<GIcon>();
		this.GxdjIcon.Width = 81.0;
		this.GxdjIcon.Height = 21.0;
		this.GxdjIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		this.GxdjIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		this.GxdjIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		this.GxdjIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.GxdjIcon.Text = Global.GetLang("贡献道具");
		this.GxdjIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.MyBangHuiDetailData == null)
			{
				return;
			}
			if (this.MyBangHuiDetailData.BHID != Global.Data.roleData.Faction)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("非【{0}】战盟成员, 无法贡献道具!"), new object[]
				{
					Global.FormatBangHuiName(this.MyBangHuiDetailData.ZoneID, this.MyBangHuiDetailData.BHName)
				}), 0, -1, -1, 0);
				return;
			}
			this.DonateGoods();
		};
		Canvas.SetLeft(this.GxdjIcon, 474);
		Canvas.SetTop(this.GxdjIcon, 77);
		this.Container.Children.Add(this.GxdjIcon);
		this.FirstPageIcon = U3DUtils.NEW<GIcon>();
		this.FirstPageIcon.Width = 66.0;
		this.FirstPageIcon.Height = 21.0;
		this.FirstPageIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		this.FirstPageIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		this.FirstPageIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		this.FirstPageIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.FirstPageIcon.Text = Global.GetLang("首页");
		this.FirstPageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.FirstPageIcon.EnableIcon)
			{
				this.FirstPage();
			}
		};
		Canvas.SetLeft(this.FirstPageIcon, 250);
		Canvas.SetTop(this.FirstPageIcon, 360);
		this.Container.Children.Add(this.FirstPageIcon);
		this.PrePageIcon = U3DUtils.NEW<GIcon>();
		this.PrePageIcon.Width = 16.0;
		this.PrePageIcon.Height = 21.0;
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
		Canvas.SetLeft(this.PrePageIcon, 321);
		Canvas.SetTop(this.PrePageIcon, 363);
		this.Container.Children.Add(this.PrePageIcon);
		this.NextPageIcon = U3DUtils.NEW<GIcon>();
		this.NextPageIcon.Width = 16.0;
		this.NextPageIcon.Height = 21.0;
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
		Canvas.SetLeft(this.NextPageIcon, 371);
		Canvas.SetTop(this.NextPageIcon, 363);
		this.Container.Children.Add(this.NextPageIcon);
		this.EndPageIcon = U3DUtils.NEW<GIcon>();
		this.EndPageIcon.Width = 66.0;
		this.EndPageIcon.Height = 21.0;
		this.EndPageIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		this.EndPageIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		this.EndPageIcon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		this.EndPageIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.EndPageIcon.Text = Global.GetLang("尾页");
		this.EndPageIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.EndPageIcon.EnableIcon)
			{
				this.EndPage();
			}
		};
		Canvas.SetLeft(this.EndPageIcon, 392);
		Canvas.SetTop(this.EndPageIcon, 360);
		this.Container.Children.Add(this.EndPageIcon);
	}

	public void InitPartData(BangHuiDetailData bangHuiDetailData)
	{
		this.MyBangHuiDetailData = bangHuiDetailData;
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("BangHuiGoodsIDs", ',');
		if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length == 5)
		{
			this.GoodsIcon_1 = this.ShowGoodsIcon(this.IcoImg_1, systemParamIntArrayByName[0], 0);
			this.GoodsIcon_2 = this.ShowGoodsIcon(this.IcoImg_2, systemParamIntArrayByName[1], 0);
			this.GoodsIcon_3 = this.ShowGoodsIcon(this.IcoImg_3, systemParamIntArrayByName[2], 0);
			this.GoodsIcon_4 = this.ShowGoodsIcon(this.IcoImg_4, systemParamIntArrayByName[3], 0);
			this.GoodsIcon_5 = this.ShowGoodsIcon(this.IcoImg_5, systemParamIntArrayByName[4], 0);
		}
		this.txtTQNum.Text = "0";
		this.txtPage.Text = "0/0";
		this.txtSYBG.Text = "0";
		GameInstance.Game.SpriteGetBangGongHist(this.MyBangHuiDetailData.BHID);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	private void RefreshBGList()
	{
		this.GoodsIcon_1.Text = ((this.MyBangHuiBagData == null) ? "0" : this.MyBangHuiBagData.Goods1Num.ToString());
		this.GoodsIcon_2.Text = ((this.MyBangHuiBagData == null) ? "0" : this.MyBangHuiBagData.Goods2Num.ToString());
		this.GoodsIcon_3.Text = ((this.MyBangHuiBagData == null) ? "0" : this.MyBangHuiBagData.Goods3Num.ToString());
		this.GoodsIcon_4.Text = ((this.MyBangHuiBagData == null) ? "0" : this.MyBangHuiBagData.Goods4Num.ToString());
		this.GoodsIcon_5.Text = ((this.MyBangHuiBagData == null) ? "0" : this.MyBangHuiBagData.Goods5Num.ToString());
		this.txtTQNum.Text = ((this.MyBangHuiBagData == null) ? "0" : this.MyBangHuiBagData.TongQian.ToString());
		this.txtSYBG.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.RongYu).ToString();
		this.SetBtnState(this.ItemsList.Count > 0);
		this.CurrentSelectedPage = 0;
		this.MaxPageCount = (this.ItemsList.Count - 1) / 7 + 1;
		this.ShowPage(this.CurrentSelectedPage);
	}

	private GIcon ShowGoodsIcon(Canvas img, int goodsID, int goodsNum)
	{
		GIcon gicon = null;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 32.0;
			gicon.Height = 32.0;
			gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/32_Hover.png"));
			gicon.TipType = 1;
			gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			gicon.ItemCode = goodsID;
			gicon.ItemObject = null;
			gicon.BoxTypes = -1;
			gicon.BodyBackground = new SolidColorBrush(ColorSL.FromArgb(255, 28, 19, 8));
			gicon.Text = goodsNum.ToString();
			gicon.TextHorizontalAlignment = global::Layout.Center;
			gicon.TextVerticalAlignment = global::Layout.Bottom;
			gicon.TextShadowColor = 4278190080U;
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0));
			img.Children.Add(gicon);
		}
		return gicon;
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Root);
	}

	private void SetBtnState(bool state)
	{
		this.PrePageIcon.EnableIcon = state;
		this.NextPageIcon.EnableIcon = state;
		this.FirstPageIcon.EnableIcon = state;
		this.EndPageIcon.EnableIcon = state;
	}

	private void ShowJgsmWindow()
	{
		if (null != this.JgsmWindow)
		{
			this.CloseChildWindow(this.JgsmWindow);
			this.JgsmWindow = null;
			this.jgsmPart = null;
			return;
		}
		this.ShowModalDialog();
		this.JgsmWindow = U3DUtils.NEW<GChildWindow>();
		this.JgsmWindow.Left = 0.0;
		this.JgsmWindow.Top = -64.0;
		this.JgsmWindow.HeadLeft = 0.0;
		this.JgsmWindow.HeadTop = 0.0;
		this.JgsmWindow.HeadWidth = 648.0;
		this.JgsmWindow.HeadHeight = 64.0;
		this.JgsmWindow.BodyLeft = 0.0;
		this.JgsmWindow.BodyTop = 64.0;
		this.JgsmWindow.BodyWidth = 648.0;
		this.JgsmWindow.BodyHeight = 407.0;
		this.InitChildWindow(this.JgsmWindow, Global.GetLang("军贡说明"));
		this.JgsmWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(s as GChildWindow);
			this.JgsmWindow = null;
			this.jgsmPart = null;
			return true;
		};
		Canvas.SetZIndex(this.JgsmWindow, 9001.0);
		this.Root.Children.Add(this.JgsmWindow);
		this.jgsmPart = U3DUtils.NEW<JgsmPart>();
		this.jgsmPart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/bh_jg_bak.png"), false, 0);
		this.jgsmPart.InitPartSize((int)this.JgsmWindow.BodyWidth - 18, (int)this.JgsmWindow.BodyHeight - 9);
		this.jgsmPart.InitPartData();
		this.JgsmWindow.SetContent(this.JgsmWindow.BodyPresenter, this.jgsmPart, 9.0, 0.0, true);
	}

	private void CloseChildWindow(GChildWindow childWindow)
	{
		int num = this.ChildWindowList.IndexOf(childWindow, 0);
		if (num >= 0)
		{
			this.ChildWindowList.RemoveRange(num, 1);
		}
		Super.CloseChildWindow(this.Root, childWindow);
	}

	private void InitChildWindow(GChildWindow childWindow, string title)
	{
		Super.InitChildWindow(childWindow, title);
		this.ChildWindowList.Add(childWindow);
	}

	private void InitChildWindow1(GChildWindow childWindow, string title)
	{
		Super.InitChildWindow1(childWindow, title);
		this.ChildWindowList.Add(childWindow);
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

	private void CloseNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.CloseNoBorderWindow(this.Root, noBorderWindow);
	}

	private void InitNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.InitNoBorderWindow(noBorderWindow);
	}

	private void DonateMoney()
	{
		if (null != this.DonateMoneyWindow)
		{
			this.CloseChildWindow(this.DonateMoneyWindow);
			this.DonateMoneyWindow = null;
			this.donateMoneyPart.CleanUpChildWindows();
			this.donateMoneyPart = null;
			return;
		}
		this.ShowModalDialog();
		this.DonateMoneyWindow = U3DUtils.NEW<GChildWindow>();
		this.DonateMoneyWindow.Left = (double)Super.GetChildLeft((int)this.Container.Width, 268);
		this.DonateMoneyWindow.Top = (double)Super.GetChildTop((int)this.Container.Height, 285);
		this.DonateMoneyWindow.HeadLeft = 0.0;
		this.DonateMoneyWindow.HeadTop = 0.0;
		this.DonateMoneyWindow.HeadWidth = 268.0;
		this.DonateMoneyWindow.HeadHeight = 46.0;
		this.DonateMoneyWindow.BodyLeft = 0.0;
		this.DonateMoneyWindow.BodyTop = 46.0;
		this.DonateMoneyWindow.BodyWidth = 268.0;
		this.DonateMoneyWindow.BodyHeight = 239.0;
		this.DonateMoneyWindow.ImgTitle = "NetImages/GameRes/Images/WinTitle/GongxianTongqian.png";
		this.InitChildWindow1(this.DonateMoneyWindow, Global.GetLang("贡献金币"));
		this.DonateMoneyWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(s as GChildWindow);
			this.DonateMoneyWindow = null;
			this.donateMoneyPart.CleanUpChildWindows();
			this.donateMoneyPart = null;
			return true;
		};
		Canvas.SetZIndex(this.DonateMoneyWindow, 9001.0);
		this.Root.Children.Add(this.DonateMoneyWindow);
		this.donateMoneyPart = U3DUtils.NEW<DonateMoneyPart>();
		this.donateMoneyPart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/bh_gxtq_bak.png"), false, 0);
		this.donateMoneyPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.DonateMoneyWindow.NotifyClose(0);
		};
		this.donateMoneyPart.InitPartSize((int)this.DonateMoneyWindow.BodyWidth - 18, (int)this.DonateMoneyWindow.BodyHeight - 9);
		this.donateMoneyPart.InitPartData(this.MyBangHuiDetailData);
		this.DonateMoneyWindow.SetContent(this.DonateMoneyWindow.BodyPresenter, this.donateMoneyPart, 9.0, 0.0, true);
	}

	private void DonateGoods()
	{
		if (null != this.DonateGoodsWindow)
		{
			this.CloseChildWindow(this.DonateGoodsWindow);
			this.DonateGoodsWindow = null;
			this.donateGoodsPart.CleanUpChildWindows();
			this.donateGoodsPart = null;
			return;
		}
		this.ShowModalDialog();
		this.DonateGoodsWindow = U3DUtils.NEW<GChildWindow>();
		this.DonateGoodsWindow.Left = (double)(Super.GetChildLeft((int)this.Container.Width, 268) - 100);
		this.DonateGoodsWindow.Top = (double)Super.GetChildTop((int)this.Container.Height, 252);
		this.DonateGoodsWindow.HeadLeft = 0.0;
		this.DonateGoodsWindow.HeadTop = 0.0;
		this.DonateGoodsWindow.HeadWidth = 268.0;
		this.DonateGoodsWindow.HeadHeight = 46.0;
		this.DonateGoodsWindow.BodyLeft = 0.0;
		this.DonateGoodsWindow.BodyTop = 46.0;
		this.DonateGoodsWindow.BodyWidth = 268.0;
		this.DonateGoodsWindow.BodyHeight = 206.0;
		this.DonateGoodsWindow.ImgTitle = "NetImages/GameRes/Images/WinTitle/GongxianDaoju.png";
		this.InitChildWindow1(this.DonateGoodsWindow, Global.GetLang("贡献道具"));
		this.DonateGoodsWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(s as GChildWindow);
			this.DonateGoodsWindow = null;
			this.donateGoodsPart.CleanUpChildWindows();
			this.donateGoodsPart = null;
			return true;
		};
		Canvas.SetZIndex(this.DonateGoodsWindow, 9001.0);
		this.Root.Children.Add(this.DonateGoodsWindow);
		this.donateGoodsPart = U3DUtils.NEW<DonateGoodsPart>();
		this.donateGoodsPart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/bh_gxdj_bak.png"), false, 0);
		this.donateGoodsPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.DonateGoodsWindow.NotifyClose(0);
		};
		this.donateGoodsPart.InitPartSize((int)this.DonateGoodsWindow.BodyWidth - 18, (int)this.DonateGoodsWindow.BodyHeight - 9);
		this.donateGoodsPart.InitPartData(this.MyBangHuiDetailData);
		this.DonateGoodsWindow.SetContent(this.DonateGoodsWindow.BodyPresenter, this.donateGoodsPart, 9.0, 0.0, true);
	}

	private void ShowPage(int pageIndex)
	{
		this.txtPage.Text = StringUtil.substitute("{0}/{1}", new object[]
		{
			pageIndex + 1,
			this.MaxPageCount
		});
		this.ItemCollection.Clear();
		int num = pageIndex * 7;
		int num2 = num;
		while (num2 < this.ItemsList.Count && num2 < num + 7)
		{
			FamilyGXListItem familyGXListItem = U3DUtils.NEW<FamilyGXListItem>();
			familyGXListItem.BodyWidth = 598.0;
			familyGXListItem.BodyHeight = 19.0;
			familyGXListItem.Width = 598.0;
			familyGXListItem.Height = 19.0;
			familyGXListItem.RoleName = this.ItemsList[num2].RoleName;
			familyGXListItem.Level = this.ItemsList[num2].RoleLevel.ToString();
			familyGXListItem.Occ = Global.GetOccupationStr(this.ItemsList[num2].Occupation);
			familyGXListItem.Zw = Global.GetBHZhiWu(this.ItemsList[num2].BHZhiWu);
			familyGXListItem.NickName = this.ItemsList[num2].BHChengHao;
			familyGXListItem.Bg = this.ItemsList[num2].BangGong.ToString();
			familyGXListItem.BzlNum = this.ItemsList[num2].Goods1Num.ToString();
			familyGXListItem.YxlNum = this.ItemsList[num2].Goods2Num.ToString();
			familyGXListItem.ZslNum = this.ItemsList[num2].Goods3Num.ToString();
			familyGXListItem.TzlNum = this.ItemsList[num2].Goods4Num.ToString();
			familyGXListItem.StlNum = this.ItemsList[num2].Goods5Num.ToString();
			familyGXListItem.TqNum = this.ItemsList[num2].TongQian.ToString();
			this.ItemCollection.AddNoUpdate(familyGXListItem);
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

	private void listBox_SelectionChanged(object sender, EventArgs e)
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
		this.SelectedListItem = U3DUtils.AS<FamilyGXListItem>(this.listBox.SelectedItem);
		if (null == this.SelectedListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedListItem.BodyBackground = this.SelectedListItemBakImg;
		this.SelectedListItem.BodyWidth = 594.0;
		this.SelectedListItem.BodyHeight = 19.0;
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

	private void UnSelectItem()
	{
		this.SelectedListItem = null;
	}

	public void NotifyBangGongHist(BangHuiBagData bangHuiBagData)
	{
		this.MyBangHuiBagData = bangHuiBagData;
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.ItemsList.RemoveAt(0);
		if (this.MyBangHuiBagData != null && this.MyBangHuiBagData.BbangGongHistList != null)
		{
			for (int i = 0; i < this.MyBangHuiBagData.BbangGongHistList.Count; i++)
			{
				this.ItemsList.Add(this.MyBangHuiBagData.BbangGongHistList[i]);
			}
		}
		this.RefreshBGList();
	}

	public void RefreshBangGong()
	{
		this.txtSYBG.Text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.RongYu).ToString();
	}

	public void NotifyDonateTongQianResult(int retCode, int roleID, int bhid)
	{
		if (retCode < 0)
		{
			if (retCode == -1010)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("战盟已经不存在"), new object[0]), 0, -1, -1, 0);
			}
			else if (retCode == -10)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			}
			else if (retCode == -101)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("贡献的绑定金币到战盟库存时发生错误: {0}"), new object[]
				{
					retCode
				}), 0, -1, -1, 0);
			}
			return;
		}
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("贡献金币给【{0}】战盟库存成功"), new object[]
		{
			Global.Data.roleData.BHName
		}), 0, -1, -1, 0);
		GameInstance.Game.SpriteGetBangGongHist(this.MyBangHuiDetailData.BHID);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	public void NotifyDonateGoodsResult(int retCode, int roleID, int bhid)
	{
		if (retCode < 0)
		{
			if (retCode == -1010)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("战盟已经不存在"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("贡献的道具到战盟库存时发生错误: {0}"), new object[]
				{
					retCode
				}), 0, -1, -1, 0);
			}
			return;
		}
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("贡献道具【{0}】战盟库存成功"), new object[]
		{
			Global.Data.roleData.BHName
		}), 0, -1, -1, 0);
		GameInstance.Game.SpriteGetBangGongHist(this.MyBangHuiDetailData.BHID);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	private int CurrentSelectedPage;

	private int MaxPageCount;

	private GIcon GxtqIcon;

	private GIcon GxdjIcon;

	private GIcon PrePageIcon;

	private GIcon NextPageIcon;

	private GIcon FirstPageIcon;

	private GIcon EndPageIcon;

	private GIcon GoodsIcon_1;

	private GIcon GoodsIcon_2;

	private GIcon GoodsIcon_3;

	private GIcon GoodsIcon_4;

	private GIcon GoodsIcon_5;

	private LoadingWindow LoadingWin;

	private List<BangGongHistData> ItemsList = new List<BangGongHistData>();

	public GChildWindow JgsmWindow;

	public GChildWindow JxzzWindow;

	private BangHuiDetailData MyBangHuiDetailData;

	private JgsmPart jgsmPart;

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	private Canvas PlaceHolder;

	private GChildWindow DonateMoneyWindow;

	private GChildWindow DonateGoodsWindow;

	private DonateMoneyPart donateMoneyPart;

	private DonateGoodsPart donateGoodsPart;

	private FamilyGXListItem SelectedListItem;

	private Canvas Root;

	private ListBox listBox = new ListBox();

	private Canvas IcoImg_1 = new Canvas();

	private Canvas IcoImg_2 = new Canvas();

	private Canvas IcoImg_3 = new Canvas();

	private Canvas IcoImg_4 = new Canvas();

	private Canvas IcoImg_5 = new Canvas();

	private GTextBlockOutLine txtTQNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtPage = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtSYBG = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ImageBrush SelectedListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 598.0, 19.0, 5.0, 5.0));

	private BangHuiBagData MyBangHuiBagData;

	private ObservableCollection _ItemCollection;
}
