using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class RepetitionTopPart : UserControl
{
	public RepetitionTopPart()
	{
		this.thisCtrl = this;
		this.ItemCollection1 = this.lbTopPlayer.ItemsSource;
		this.Container.addEventListener("mouseDown", new MouseEventHandler(this.UserControl_MouseLeftButtonDown));
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.lbTopPlayer);
		this.lbTopPlayer.Width = 323.0;
		this.lbTopPlayer.Height = 205.0;
		this.lbTopPlayer.SelectionChanged = new MouseLeftButtonUpEventHandler(this.lbTopPlayer_SelectionChanged);
		this.lbTopPlayer.MouseLeftButtonDown = new MouseLeftButtonUpEventHandler(this.lbTopPlayer_MouseLeftButtonDown);
		this.lbTopPlayer.Background = new SolidColorBrush(16777215U);
		this.lbTopPlayer.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		Canvas.SetLeft(this.lbTopPlayer, 12);
		Canvas.SetTop(this.lbTopPlayer, 46);
	}

	public ObservableCollection ItemCollection1
	{
		get
		{
			return this._ItemCollection1;
		}
		set
		{
			this._ItemCollection1 = value;
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
	}

	public void InitPartData()
	{
		this.LoadTopTypeList();
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
		GameInstance.Game.SpriteGetFuBenHistListData();
	}

	private void LoadTopTypeList()
	{
		this.ItemCollection1.Clear();
		XElement gameResXml = Global.GetGameResXml("Config/RepetitionTop.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			RepetitionTopListItem repetitionTopListItem = U3DUtils.NEW<RepetitionTopListItem>();
			repetitionTopListItem.BodyWidth = 305.0;
			repetitionTopListItem.BodyHeight = 20.0;
			repetitionTopListItem.RepetitionID = Global.GetXElementAttributeInt(xelement, "ID");
			repetitionTopListItem.RepetitionName = Global.GetXElementAttributeStr(xelement, "Name");
			repetitionTopListItem.RoleName = string.Empty;
			repetitionTopListItem.CompleteTime = string.Empty;
			repetitionTopListItem.RoleID = -1;
			this.ItemCollection1.AddNoUpdate(repetitionTopListItem);
		}
		this.ItemCollection1.DelayUpdate();
	}

	private void lbTopPlayer_SelectionChanged(object sender, MouseEvent e)
	{
		if (null != this.SelectedPlayerListItem)
		{
			this.SelectedPlayerListItem.BodyBackground = null;
		}
		if (this.lbTopPlayer.SelectedIndex < 0)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedPlayerListItem = this.lbTopPlayer.SelectedItem.SafeGetComponent<RepetitionTopListItem>();
		if (null == this.SelectedPlayerListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedPlayerListItem.BodyBackground = this.SelectedPlayerListItemBakImg;
	}

	private void UnSelectItem()
	{
		this.SelectedPlayerListItem = null;
	}

	private string FormatSecs(int secs)
	{
		return StringUtil.substitute(Global.GetLang("{0}分钟{1}秒"), new object[]
		{
			Global.FormatStr("00", secs / 60),
			Global.FormatStr("00", secs % 60)
		});
	}

	public void NotifyFuBenHistDataDict(Dictionary<int, FuBenHistData> fuBenHistDataDict)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (fuBenHistDataDict == null)
		{
			return;
		}
		for (int i = 0; i < this.ItemCollection1.Count; i++)
		{
			RepetitionTopListItem repetitionTopListItem = this.ItemCollection1.GetAt(i).SafeGetComponent<RepetitionTopListItem>();
			if (fuBenHistDataDict.ContainsKey(repetitionTopListItem.RepetitionID))
			{
				FuBenHistData value = fuBenHistDataDict.GetValue(repetitionTopListItem.RepetitionID);
				repetitionTopListItem.RoleID = value.RoleID;
				repetitionTopListItem.RoleName = value.RoleName;
				repetitionTopListItem.CompleteTime = this.FormatSecs(value.UsedSecs);
			}
		}
	}

	private void InitNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.InitNoBorderWindow(noBorderWindow);
	}

	private void CloseNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.CloseNoBorderWindow(this.Container, noBorderWindow);
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

	public void ShowMenuWindow(int cx, int cy, int[] ids, string[] names)
	{
		this.HideWindow();
		int num = ids.Length;
		this.MenuWindow = U3DUtils.NEW<NoBorderWindow>();
		this.MenuWindow.Left = (double)cx;
		this.MenuWindow.Top = (double)cy;
		this.MenuWindow.BodyLeft = 0.0;
		this.MenuWindow.BodyTop = 0.0;
		this.MenuWindow.BodyWidth = 120.0;
		this.MenuWindow.BodyHeight = (double)((num + 1) * 21);
		this.MenuWindow.BodyBackBrush = new SolidColorBrush(1185560U);
		this.MenuWindow.BodyBackOpacity = 0.9;
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
		if (id == 0)
		{
			GameInstance.Game.SpriteQueryIDByName(this.SelectedPlayerListItem.RoleName, 0);
		}
		else if (id == 1)
		{
			if (Global.Data.roleData.RoleName != this.SelectedPlayerListItem.RoleName && this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = id,
					Tag = this.SelectedPlayerListItem.RoleName
				});
			}
		}
		else if (id == 2)
		{
			GameInstance.Game.SpriteAddFriend(this.SelectedPlayerListItem.RoleID, this.SelectedPlayerListItem.RoleName, 0);
		}
		else if (id == 3)
		{
			GameInstance.Game.SpriteQueryIDByName(this.SelectedPlayerListItem.RoleName, 4);
		}
	}

	private void lbTopPlayer_MouseLeftButtonDown(object sender, MouseEvent e)
	{
		Point position = new global::MousePosition(e).GetPosition(this.lbTopPlayer);
		if (position.X < 0 || position.Y < 0 || (double)position.X >= this.lbTopPlayer.Width || (double)position.Y >= this.lbTopPlayer.Height)
		{
			this.HideWindow();
			return;
		}
		if (position.Y > (this.lbTopPlayer.SelectedIndex + 1) * 20 || position.Y < this.lbTopPlayer.SelectedIndex * 20)
		{
			this.HideWindow();
			return;
		}
		if (this.SelectedPlayerListItem == null)
		{
			this.HideWindow();
			return;
		}
		position = new global::MousePosition(e).GetPosition(this.Container);
		this.ShowMenuWindow(position.X, position.Y, this.PlayerListMenuItemIDs, this.PlayerListMenuItemNames);
	}

	private UserControl thisCtrl;

	private ListBox lbTopPlayer = new ListBox();

	public DPSelectedItemEventHandler DPSelectedItem;

	private LoadingWindow LoadingWin;

	private int[] PlayerListMenuItemIDs = new int[]
	{
		default(int),
		1,
		2,
		3
	};

	private string[] PlayerListMenuItemNames = new string[]
	{
		Global.GetLang("查看装备"),
		Global.GetLang("私聊"),
		Global.GetLang("加为好友"),
		Global.GetLang("查看摊位")
	};

	private GMenuPart menuPart;

	private NoBorderWindow MenuWindow;

	private RepetitionTopListItem SelectedPlayerListItem;

	private ImageBrush SelectedPlayerListItemBakImg = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 305.0, 20.0, 5.0, 5.0));

	private ObservableCollection _ItemCollection1;
}
