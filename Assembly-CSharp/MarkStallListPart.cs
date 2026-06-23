using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class MarkStallListPart : UserControl
{
	public MarkStallListPart()
	{
		this.thisCtrl = this;
		this.ItemCollection = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 608.0;
		this.listBox.Height = 222.0;
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 0.0, 3.0);
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		Canvas.SetLeft(this.listBox, 14);
		Canvas.SetTop(this.listBox, 68);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.Page);
		this.Page.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.Page, 290);
		Canvas.SetTop(this.Page, 345);
		this.Container.Children.Add(this.TotalPage);
		this.TotalPage.Text = "0";
		this.TotalPage.FontSize = HSTextField.defaultFontSize;
		this.TotalPage.TextColor = new SolidColorBrush(4285638580U);
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

	public GTabControl tc
	{
		get
		{
			return this._tc;
		}
		set
		{
			this._tc = value;
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
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("充值");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.OpenChongZhiHtmlWindow();
		};
		Canvas.SetLeft(gicon, 527);
		Canvas.SetTop(gicon, 302);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("刷新");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearAllResult();
			GameInstance.Game.SpriteMarketRoleList();
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
				return;
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
				return;
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
				return;
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
				return;
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
					ID = this.SelectedListItem.GoodsSellerID,
					IDType = 1
				});
			}
		};
		Canvas.SetLeft(this.ViewInfoIcon, 527);
		Canvas.SetTop(this.ViewInfoIcon, 343);
		this.Container.Children.Add(this.ViewInfoIcon);
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

	public void GetNewData(int toViewOtherRoleID = 0, string toViewOtherRoleName = "")
	{
		if (!this.FirstGetNewData)
		{
			return;
		}
		this.FirstGetNewData = false;
		GameInstance.Game.SpriteMarketRoleList();
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
		if (toViewOtherRoleID > 0 && !string.IsNullOrEmpty(toViewOtherRoleName))
		{
			this.ViewStall2(toViewOtherRoleID, toViewOtherRoleName);
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

	public void RefreshData(List<SaleRoleData> saleRoleDataList)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.SaleRoleDataList = saleRoleDataList;
		if (this.SaleRoleDataList == null || this.SaleRoleDataList.Count <= 0)
		{
		}
		this.ShowGoodsList();
		if (this.SaleRoleDataList == null || this.SaleRoleDataList.Count <= 0)
		{
			this.SetBtnState(false);
		}
		else
		{
			this.SetBtnState(true);
			this.CurrentSelectedPage = 0;
			this.MaxPageCount = (this.ItemsList.Count - 1) / 10 + 1;
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
		this.ItemsList = new List<Mark2ListItem>();
		if (this.SaleRoleDataList == null)
		{
			return;
		}
		for (int i = 0; i < this.SaleRoleDataList.Count; i++)
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
		int num = pageIndex * 10;
		int num2 = num;
		while (num2 < this.ItemsList.Count && num2 < num + 10)
		{
			if (null == this.ItemsList[num2])
			{
				Mark2ListItem mark2ListItem = U3DUtils.NEW<Mark2ListItem>();
				mark2ListItem.BodyWidth = 602.0;
				mark2ListItem.BodyHeight = 22.0;
				mark2ListItem.Width = 602.0;
				mark2ListItem.Height = 22.0;
				mark2ListItem.Goodsdouble = (num2 + 1).ToString();
				mark2ListItem.GoodsSellerID = this.SaleRoleDataList[num2].RoleID;
				mark2ListItem.GoodsSellerName = this.SaleRoleDataList[num2].RoleName;
				mark2ListItem.GoodsSellerLevel = this.SaleRoleDataList[num2].RoleLevel.ToString();
				mark2ListItem.GoodsNum = this.SaleRoleDataList[num2].SaleGoodsNum.ToString();
				mark2ListItem.addEventListener("mouseUp", new MouseEventHandler(this.ItemMouseLeftButtonUp));
				this.ItemsList[num2] = mark2ListItem;
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
		this.ViewStall();
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
		this.SelectedListItem = U3DUtils.AS<Mark2ListItem>(this.listBox.SelectedItem);
		if (null == this.SelectedListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedListItem.BodyBackground = this.SelectedListItemBakImg;
		this.SelectedListItem.BodyWidth = 602.0;
		this.SelectedListItem.BodyHeight = 18.0;
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

	private void ViewStall()
	{
		if (null == this.SelectedListItem)
		{
			return;
		}
		this.ViewStall2(this.SelectedListItem.GoodsSellerID, this.SelectedListItem.txtSellerName.Text);
	}

	private void ViewStall2(int otherRoleID, string otherRoleName)
	{
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
		Canvas.SetLeft(this.ViewStallWindow, Super.GetChildLeft(648, 308));
		Canvas.SetTop(this.ViewStallWindow, Super.GetChildTop(467, 522) - 40);
		this.InitChildWindow1(this.ViewStallWindow, StringUtil.substitute(Global.GetLang("{0}的摊位"), new object[]
		{
			otherRoleName
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
		this.markViewStallPart.OtherRoleID = otherRoleID;
		this.markViewStallPart.InitPartSize((int)this.ViewStallWindow.BodyWidth - 18, (int)this.ViewStallWindow.BodyHeight - 9);
		this.markViewStallPart.InitPartData();
		this.ViewStallWindow.SetContent(this.ViewStallWindow.BodyPresenter, this.markViewStallPart, 9.0, 0.0, true);
	}

	private void ClearAllResult()
	{
		this.SaleRoleDataList = null;
		if (this.ItemsList != null)
		{
			this.ItemsList.Clear();
		}
		if (this.ItemCollection != null)
		{
			this.ItemCollection.Clear();
		}
	}

	private UserControl thisCtrl;

	private ListBox listBox = new ListBox();

	private GTextBlockOutLine Page = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine TotalPage = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public DPSelectedItemEventHandler DPSelectedItem;

	private List<Mark2ListItem> ItemsList;

	private LoadingWindow LoadingWin;

	public GChildWindow ViewStallWindow;

	private int CurrentSelectedPage;

	private int MaxPageCount;

	private GIcon PrePageIcon;

	private GIcon NextPageIcon;

	private GIcon FirstPageIcon;

	private GIcon EndPageIcon;

	private GIcon ViewStallIcon;

	private GIcon ViewInfoIcon;

	private bool FirstInitPartData = true;

	private bool FirstGetNewData = true;

	private List<SaleRoleData> SaleRoleDataList;

	private double LastMouseLeftButtonDownTicks;

	private Mark2ListItem SelectedListItem;

	private ImageBrush SelectedListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 602.0, 22.0, 5.0, 5.0));

	private Canvas PlaceHolder;

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	private MarkViewStallPart markViewStallPart;

	private ObservableCollection _ItemCollection;

	private GTabControl _tc;
}
