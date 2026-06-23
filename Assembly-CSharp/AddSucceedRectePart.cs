using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class AddSucceedRectePart : UserControl
{
	public AddSucceedRectePart()
	{
		this.thisCtrl = this;
		this.Container.addEventListener("mouseDown", new MouseEventHandler(this.UserControl_MouseLeftButtonDown));
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.SucceedRText);
		this.SucceedRText.TextColor = new SolidColorBrush(16711680U);
		Canvas.SetLeft(this.SucceedRText, 186);
		Canvas.SetTop(this.SucceedRText, 9);
		this.Container.Children.Add(this.LingZhiCountText);
		this.LingZhiCountText.TextColor = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.LingZhiCountText, 136);
		Canvas.SetTop(this.LingZhiCountText, 55);
		this.Container.addEventListener("mouseDown", new MouseEventHandler(this.onClosMenuWindow));
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public int SucceeRect
	{
		set
		{
			this.iSucceeRect = value;
		}
	}

	private void onClosMenuWindow(MouseEvent evt)
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		}
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
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("确 定");
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.selecLZCount > 0 && Global.GetTotalGoodsCountByID(this.iLZid) < this.selecLZCount)
			{
				string goodsNameByID = Global.GetGoodsNameByID(this.iLZid, false);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("抱歉，您背包中【{0}】的数量不够，请到商城中购买！"), new object[]
				{
					goodsNameByID
				}), 19, -1, -1, this.iLZid);
				return;
			}
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = this.selecLZCount
				});
			}
		};
		Canvas.SetLeft(gicon, 44);
		Canvas.SetTop(gicon, 128);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("取 消");
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0
				});
			}
		};
		Canvas.SetLeft(gicon, 173);
		Canvas.SetTop(gicon, 128);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 36.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 36.0, 21.0, 3.0, 2.0));
		Canvas.SetLeft(gicon, 127);
		Canvas.SetTop(gicon, 55);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 19.0;
		gicon.Height = 19.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn6_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/btn6_hover.png"));
		Canvas.SetLeft(gicon, 164);
		Canvas.SetTop(gicon, 54);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.LSMouseLeftButtonUp);
	}

	public void InitPartData()
	{
		this.LingZhiCountText.Text = "0";
		Canvas.SetZIndex(this.LingZhiCountText, 10.0);
		this.SucceedRText.Text = StringUtil.substitute("{0}%", new object[]
		{
			this.iSucceeRect
		});
		this.iLZid = (int)ConfigSystemParam.GetSystemParamIntByName("LingZhiGoodsID");
		this.ShowGoodsIcon(this.iLZid);
	}

	private void ShowGoodsIcon(int goodsID)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 32.0;
			gicon.Height = 32.0;
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
			gicon.BoxTypes = 5;
			gicon.BodyBackground = new SolidColorBrush(ColorSL.FromArgb(255, 28, 19, 8));
			gicon.Text = Global.GetTotalGoodsCountByID(goodsID).ToString();
			gicon.TextHorizontalAlignment = global::Layout.Center;
			gicon.TextVerticalAlignment = global::Layout.Bottom;
			gicon.TextShadowColor = 4278190080U;
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0));
			if (Global.GetTotalGoodsCountByID(goodsID) <= 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			else
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			Canvas.SetLeft(gicon, 34);
			Canvas.SetTop(gicon, 74);
			this.Container.Children.Add(gicon);
			this.LingZhiGoodsIcon = gicon;
		}
	}

	public void RefreshGoodsCount()
	{
		GIcon lingZhiGoodsIcon = this.LingZhiGoodsIcon;
		if (null == lingZhiGoodsIcon)
		{
			return;
		}
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(lingZhiGoodsIcon.ItemCode), string.Empty);
		if (Global.GetTotalGoodsCountByID(lingZhiGoodsIcon.ItemCode) <= 0)
		{
			lingZhiGoodsIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
		}
		else
		{
			lingZhiGoodsIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
		}
		lingZhiGoodsIcon.Text = Global.GetTotalGoodsCountByID(lingZhiGoodsIcon.ItemCode).ToString();
	}

	private void LSMouseLeftButtonUp(object sender, MouseEvent e)
	{
		this.ShowMenuWindow(130, 81, this.tpMenuItemIDs, this.tiMenuItemNames);
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
		this.MenuWindow.Width = 120.0;
		this.MenuWindow.Height = (double)((ids.Length + 1) * 21);
		this.MenuWindow.BodyBackBrush = new SolidColorBrush(1185560U);
		this.MenuWindow.BodyBackOpacity = 0.9;
		Canvas.SetLeft(this.MenuWindow, x);
		Canvas.SetTop(this.MenuWindow, y);
		this.InitNoBorderWindow(this.MenuWindow);
		this.Root.Children.Add(this.MenuWindow);
		this.menuPart = U3DUtils.NEW<GMenuPart>();
		this.menuPart.InitPartSize((int)this.MenuWindow.Width - 4, (int)this.MenuWindow.Height - 4);
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

	private void InitNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.InitNoBorderWindow(noBorderWindow);
	}

	private void CloseNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.CloseNoBorderWindow(this.Root, noBorderWindow);
	}

	private void ProcessMenuClick(int id)
	{
		int num = this.iSucceeRect + Global.JingMaiLingZhiLuckyNum * id;
		if (num > 100)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您所选择的成功率超过了100%"), new object[0]), 0, -1, -1, 0);
			return;
		}
		this.LingZhiCountText.Text = id.ToString();
		this.SucceedRText.Text = StringUtil.substitute("{0}% + {1}%", new object[]
		{
			this.iSucceeRect,
			Global.JingMaiLingZhiLuckyNum * id
		});
		this.selecLZCount = id;
	}

	private void UserControl_MouseLeftButtonDown(MouseEvent e)
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private SpriteSL thisCtrl;

	private int selecLZCount;

	private int iSucceeRect;

	private int iLZid;

	private GIcon LingZhiGoodsIcon;

	private int[] tpMenuItemIDs = new int[]
	{
		1,
		2,
		3,
		4,
		5,
		6,
		7,
		8,
		9
	};

	private string[] tiMenuItemNames = new string[]
	{
		Global.GetLang("1个"),
		Global.GetLang("2个"),
		Global.GetLang("3个"),
		Global.GetLang("4个"),
		Global.GetLang("5个"),
		Global.GetLang("6个"),
		Global.GetLang("7个"),
		Global.GetLang("8个"),
		Global.GetLang("9个")
	};

	private GMenuPart menuPart;

	private Canvas Root;

	private GTextBlockOutLine SucceedRText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine LingZhiCountText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private NoBorderWindow MenuWindow;
}
