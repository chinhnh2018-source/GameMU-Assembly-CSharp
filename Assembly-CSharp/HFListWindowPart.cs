using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class HFListWindowPart : UserControl
{
	public HFListWindowPart()
	{
		this.ItemCollection = this.listBox.ItemsSource;
		this.Container.addEventListener("mouseDown", new MouseEventHandler(this.UserControl_MouseLeftButtonDown));
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.listBox);
		this.listBox.Width = 270.0;
		this.listBox.Height = 70.0;
		Canvas.SetLeft(this.listBox, 10);
		Canvas.SetTop(this.listBox, 44);
		this.listBox.ItemMargin = new Thickness(0.0, 0.0, 0.0, 3.0);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.BorderThickness = 0;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.listBox.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.listBox_MouseLeftButtonUp);
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

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	public void InitPartData()
	{
		this.GetData();
	}

	private void LoadList()
	{
		this.ItemCollection.Clear();
		if (this.MyHuangFeiDataList != null)
		{
			for (int i = 0; i < this.MyHuangFeiDataList.Count; i++)
			{
				string roleName = this.MyHuangFeiDataList[i].RoleName;
				HFListItem hflistItem = U3DUtils.NEW<HFListItem>();
				hflistItem.BodyWidth = 270.0;
				hflistItem.BodyHeight = 19.0;
				hflistItem.Width = 270.0;
				hflistItem.Height = 19.0;
				hflistItem.RoleID = this.MyHuangFeiDataList[i].RoleID;
				hflistItem.RoleName = roleName;
				hflistItem.RoleLevel = this.MyHuangFeiDataList[i].Level.ToString();
				hflistItem.RoleOcc = Global.GetOccupationStr(this.MyHuangFeiDataList[i].Occupation);
				hflistItem.RoleSex = ((this.MyHuangFeiDataList[i].RoleSex != 0) ? Global.GetLang("女") : Global.GetLang("男"));
				this.ItemCollection.AddNoUpdate(hflistItem);
			}
			this.ItemCollection.DelayUpdate();
		}
	}

	private void GetData()
	{
		GameInstance.Game.SpriteGetHuangFeiDataList();
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
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
		this.SelectedListItem = U3DUtils.AS<HFListItem>(this.listBox.SelectedItem);
		if (null == this.SelectedListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedListItem.BodyBackground = this.SelectedListItemBakImg;
	}

	private void UnSelectItem()
	{
		this.SelectedListItem = null;
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

	private void HideWindow()
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		}
	}

	private void UserControl_MouseLeftButtonDown(MouseEvent e)
	{
		this.HideWindow();
	}

	public void ShowMenuWindow(int cx, int cy, int[] ids, string[] names, int menus_id)
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		}
		this.MenuWindow = U3DUtils.NEW<NoBorderWindow>();
		this.MenuWindow.Left = (double)cx;
		this.MenuWindow.Top = (double)cy;
		this.MenuWindow.BodyLeft = 0.0;
		this.MenuWindow.BodyTop = 0.0;
		this.MenuWindow.BodyWidth = 120.0;
		this.MenuWindow.BodyHeight = (double)((ids.Length + 1) * 21);
		this.MenuWindow.BodyBackBrush = new SolidColorBrush(1185560U);
		this.MenuWindow.BodyBackOpacity = 0.9;
		this.InitNoBorderWindow(this.MenuWindow);
		this.Root.Children.Add(this.MenuWindow);
		this.menuPart = U3DUtils.NEW<GMenuPart>();
		this.menuPart.InitPartSize((int)this.MenuWindow.BodyWidth - 4, (int)this.MenuWindow.BodyHeight - 4);
		string imageFileName = "Images/Plate/menu_item_unselected.png";
		for (int i = 0; i < ids.Length; i++)
		{
			if (i != 3 || Global.IsHuangDi(Global.Data.roleData))
			{
				this.menuPart.AddMenuItem(ids[i], imageFileName, names[i], null);
			}
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
			this.ProcessPropMenuClick(gmenuItem.MenuItemID);
		};
		this.MenuWindow.SetContent(this.MenuWindow.BodyPresenter, this.menuPart, 2.0, 2.0);
	}

	private void ProcessPropMenuClick(int id)
	{
		if (null == this.SelectedListItem)
		{
			return;
		}
		if (id == 0)
		{
			string roleName = this.SelectedListItem.RoleName;
			GameInstance.Game.SpriteQueryIDByName(roleName, 0);
		}
		else if (id != 1)
		{
			if (id == 2)
			{
				if (Global.Data.roleData.RoleID != this.SelectedListItem.RoleID)
				{
					string roleName2 = this.SelectedListItem.RoleName;
					GameInstance.Game.SpriteAddFriend(this.SelectedListItem.RoleID, roleName2, 0);
				}
			}
			else if (id == 3)
			{
				if (Global.IsHuangDi(Global.Data.roleData))
				{
					string roleName3 = this.SelectedListItem.RoleName;
					GameInstance.Game.SpriteRemoveHuangFei(this.SelectedListItem.RoleID, roleName3);
					this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
					this.Container.Children.Add(this.LoadingWin);
				}
				else
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("只有本服城主才能废黜皇妃"), 0, -1, -1, 0);
				}
			}
		}
	}

	private void listBox_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		Point position = new global::MousePosition(e).GetPosition(this.listBox);
		if (position.X < 0 || position.Y < 0 || (double)position.X >= this.listBox.Width || (double)position.Y >= this.listBox.Height)
		{
			this.HideWindow();
			return;
		}
		if (position.Y > (this.listBox.SelectedIndex + 1) * 19 || position.Y < this.listBox.SelectedIndex * 19)
		{
			this.HideWindow();
			return;
		}
		if (this.SelectedListItem == null)
		{
			this.HideWindow();
			return;
		}
		position = new global::MousePosition(e).GetPosition(this.Root);
		this.ShowMenuWindow(position.X, position.Y, this.HFListMenuItemIDs, this.HFListMenuItemNames, 1);
	}

	public void NotifyHuangFeiDataList(List<SearchRoleData> huangFeiDataList)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.MyHuangFeiDataList = huangFeiDataList;
		this.LoadList();
	}

	public void NotifyRemoveHuangFeiResult(int retCode, int otherRoleID)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (retCode < 0)
		{
			return;
		}
		if (this.MyHuangFeiDataList != null)
		{
			for (int i = 0; i < this.MyHuangFeiDataList.Count; i++)
			{
				if (this.MyHuangFeiDataList[i].RoleID == otherRoleID)
				{
					this.MyHuangFeiDataList.RemoveRange(i, 1);
					this.ItemCollection.RemoveAt(i);
					break;
				}
			}
		}
	}

	private LoadingWindow LoadingWin;

	private HFListItem SelectedListItem;

	private ImageBrush SelectedListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 270.0, 19.0, 5.0, 5.0));

	private Canvas PlaceHolder;

	private GMenuPart menuPart;

	private NoBorderWindow MenuWindow;

	private int[] HFListMenuItemIDs = new int[]
	{
		default(int),
		1,
		2,
		3
	};

	private string[] HFListMenuItemNames = new string[]
	{
		Global.GetLang("查看装备"),
		Global.GetLang("私聊"),
		Global.GetLang("加为好友"),
		Global.GetLang("废除皇妃")
	};

	private List<SearchRoleData> MyHuangFeiDataList;

	private Canvas Root;

	private ListBox listBox = new ListBox();

	private ObservableCollection _ItemCollection;
}
