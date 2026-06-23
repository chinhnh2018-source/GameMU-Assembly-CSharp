using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class EnhancePart : UserControl
{
	public EnhancePart()
	{
		this.Container.addEventListener("mouseDown", new MouseEventHandler(this.UserControl_MouseLeftButtonDown));
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.Equip);
		this.Equip.Width = 35.0;
		this.Equip.Height = 35.0;
		Canvas.SetLeft(this.Equip, 40);
		Canvas.SetTop(this.Equip, 55);
		this.Equip.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.Qianhua);
		this.Qianhua.Width = 35.0;
		this.Qianhua.Height = 35.0;
		Canvas.SetLeft(this.Qianhua, 21);
		Canvas.SetTop(this.Qianhua, 282);
		this.Qianhua.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.Fu);
		this.Fu.Width = 35.0;
		this.Fu.Height = 35.0;
		this.Fu.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.Fu, 68);
		Canvas.SetTop(this.Fu, 282);
	}

	public ObservableCollection equipIcon
	{
		get
		{
			return this._equipIcon;
		}
		set
		{
			this._equipIcon = value;
		}
	}

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public bool Enchancing
	{
		get
		{
			return this._Enchancing;
		}
		set
		{
			this._Enchancing = value;
		}
	}

	public override void Destroy()
	{
		Super.RemoveSystemNaviBox(this.Container, Global.GetLang("装备精炼UI"), null);
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
		gicon.Text = Global.GetLang("精炼");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		Canvas.SetLeft(gicon, 217);
		Canvas.SetTop(gicon, 375);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartEnchanceEquip();
		};
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("放入装备");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Tip = "PutEquipBtn";
		gicon.TipType = 4;
		Canvas.SetLeft(gicon, 23);
		Canvas.SetTop(gicon, 17);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.InputEquipMouseLeftButtonUp);
		UpDown upDown = U3DUtils.NEW<UpDown>();
		upDown.ValueChange = delegate(object s, EventArgs e)
		{
			int num = Convert.ToInt32(this.XinyunNubText.Text);
			if ((e as ChangeEventArgs).ChangeType == 1)
			{
				if (num >= 20)
				{
					return;
				}
				if (!this.CanValClick())
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("成功率已经达到100%，无需再增加幸运符！"), new object[0]), 0, -1, -1, 0);
					return;
				}
				int goodsID = (int)ConfigSystemParam.GetSystemParamIntByName("EnchanceLuckyGoodsID");
				int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
				if (totalGoodsCountByID < num + 1)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您的背包中没有足够的幸运符！"), new object[0]), 0, -1, -1, 0);
					return;
				}
				this.XinyunNubText.Text = (num + 1).ToString();
			}
			else if ((e as ChangeEventArgs).ChangeType == -1)
			{
				if (num <= 1)
				{
					return;
				}
				this.XinyunNubText.Text = (num - 1).ToString();
			}
			this.ProcessValClick();
		};
		Canvas.SetLeft(upDown, 173);
		Canvas.SetTop(upDown, 376);
		this.Container.Children.Add(upDown);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 36.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 36.0, 21.0, 3.0, 2.0));
		Canvas.SetLeft(gicon, 133);
		Canvas.SetTop(gicon, 379);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 51.0;
		gicon.Height = 29.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/zbtp_qualityPointer.png"));
		gicon.DisableHandCursor = true;
		Canvas.SetLeft(gicon, 80);
		Canvas.SetTop(gicon, 56);
		this.Container.Children.Add(gicon);
		GCheckBox gcheckBox = new GCheckBox();
		gcheckBox.Name = "UsingLuckyRocks";
		gcheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		gcheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		gcheckBox.Text = Global.GetLang("使用幸运符");
		gcheckBox.Check = false;
		gcheckBox.TextColor = new SolidColorBrush(7448500U);
		Canvas.SetLeft(gcheckBox, 48);
		Canvas.SetTop(gcheckBox, 381);
		this.Container.Children.Add(gcheckBox);
		this.luckyCheckBox = gcheckBox;
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.Text = Global.GetLang("  精炼石:");
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.TextSize = 12.0;
		Canvas.SetLeft(gtextBlockOutLine, 174);
		Canvas.SetTop(gtextBlockOutLine, 277);
		this.Container.Children.Add(gtextBlockOutLine);
		this.TipinText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.TextSize = 12.0;
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.Width = 65.0;
		gtextBlockOutLine.Text = "0%";
		Canvas.SetLeft(gtextBlockOutLine, 125);
		Canvas.SetTop(gtextBlockOutLine, 326);
		this.Container.Children.Add(gtextBlockOutLine);
		this.OddsText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.TextSize = 12.0;
		gtextBlockOutLine.Height = 15.0;
		gtextBlockOutLine.Width = 65.0;
		Canvas.SetLeft(gtextBlockOutLine, 256);
		Canvas.SetTop(gtextBlockOutLine, 279);
		this.Container.Children.Add(gtextBlockOutLine);
		this.TipinsNubText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.TextSize = 12.0;
		gtextBlockOutLine.Height = 15.0;
		gtextBlockOutLine.Width = 65.0;
		Canvas.SetLeft(gtextBlockOutLine, 256);
		Canvas.SetTop(gtextBlockOutLine, 302);
		this.Container.Children.Add(gtextBlockOutLine);
		this.YinLiang1Text = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.TextSize = 12.0;
		gtextBlockOutLine.Height = 14.0;
		gtextBlockOutLine.Width = 30.0;
		gtextBlockOutLine.Text = "1";
		Canvas.SetLeft(gtextBlockOutLine, 139);
		Canvas.SetTop(gtextBlockOutLine, 378);
		this.Container.Children.Add(gtextBlockOutLine);
		this.XinyunNubText = gtextBlockOutLine;
		this.progressBar = U3DUtils.NEW<GProgressBar>();
		this.progressBar.BodyWidth = 117.0;
		this.progressBar.BodyHeight = 15.0;
		this.progressBar.BackColor = 4286611584U;
		this.progressBar.StrokeThickness = new Thickness(1.0, 1.0, 1.0, 1.0);
		this.progressBar.Stroke = 4289309097U;
		this.progressBar.RadiusX = 8.0;
		this.progressBar.RadiusY = 8.0;
		this.progressBar.Visibility = false;
		Canvas.SetLeft(this.progressBar, 175);
		Canvas.SetTop(this.progressBar, 329);
		this.Container.Children.Add(this.progressBar);
	}

	private void ClickGetThingNotify(ObjectClickEvent evt)
	{
	}

	public void ClearAllValues()
	{
	}

	private void StartEnchanceEquip()
	{
	}

	private void ExecuteEnchanceEquip()
	{
		this.Enchancing = true;
		Global.Data.GameScene.AddForgeDecoration();
		this.progressBar.Visibility = true;
		this.StartHeart();
	}

	public void NotifyEnchanceResult(GoodsData goodsData, int result, int dbID, int quality, string props, int binding)
	{
	}

	public void StartHeart()
	{
		this.StopHeart();
		this._Timer = new DispatcherTimer("EnchancePart_Timer");
		this._Timer.Interval = TimeSpan.FromMilliseconds(100.0);
		this._Timer.Tick = new DispatcherTimerEventHandler(this.ForgeTimer_Tick);
		this._TimerCount = 0;
		this._Timer.Start();
	}

	public void StopHeart()
	{
		if (this._Timer == null)
		{
			return;
		}
		this._Timer.Stop();
		this._Timer.Tick = null;
		this._Timer = null;
		this._TimerCount = 0;
	}

	private void ForgeTimer_Tick(object sender, object e)
	{
		this._TimerCount++;
		this.progressBar.Percent = (double)this._TimerCount / 12.0;
		if ((double)this._TimerCount >= 12.0)
		{
			this.StopHeart();
			GameInstance.Game.SpriteEnchanceGoods(this.EnchanceDbID, this.RockGoodsID, this.LuckyNum);
		}
	}

	public void InitPartData()
	{
		this.InitXingYunFu();
		Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("装备精炼UI"), 170403, Super.GetTaskStateByID(170403), 1);
	}

	private void AddGoodsIcon(int goodsID, int index, int needNum)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 32.0;
			gicon.Height = 32.0;
			gicon.TipType = 1;
			gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsXmlNodeByID.ID,
				0,
				-1,
				-1
			});
			gicon.ItemCode = goodsID;
			gicon.ItemObject = null;
			gicon.BoxTypes = 5;
			gicon.BodyBackground = new SolidColorBrush(ColorSL.FromArgb(255, 28, 19, 8));
			gicon.Text = needNum.ToString();
			gicon.TextHorizontalAlignment = global::Layout.Right;
			gicon.TextVerticalAlignment = global::Layout.Bottom;
			gicon.TextShadowColor = 4278190080U;
			gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0));
			gicon.DisableHandCursor = true;
			if (Global.GetTotalGoodsCountByID(goodsID) > 0)
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				gicon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
		}
	}

	private void InputEquipMouseLeftButtonUp(object sender, MouseEvent e)
	{
	}

	private void IconMouseLeftButtonUp(object sender, MouseEvent e)
	{
		Point position = new global::MousePosition(e).GetPosition(this.Fu);
		this.ShowMenuWindow(position.X, position.Y + 210, this.tpMenuItemIDs, this.tiMenuItemNames);
	}

	public void ShowMenuWindow(int cx, int cy, int[] ids, string[] names)
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
		this.MenuWindow.Top = this.MenuWindow.Top - this.MenuWindow.BodyHeight;
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

	private void InitNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.InitNoBorderWindow(noBorderWindow);
	}

	private void CloseNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.CloseNoBorderWindow(this.Container, noBorderWindow);
	}

	private void ProcessMenuClick(int id)
	{
	}

	private bool CanValClick()
	{
		return true;
	}

	private void ProcessValClick()
	{
	}

	private void InitRocks(int goodsID, int needNum)
	{
		this.AddGoodsIcon(goodsID, 1, needNum);
	}

	private void InitXingYunFu()
	{
		int num = (int)ConfigSystemParam.GetSystemParamIntByName("EnchanceLuckyGoodsID");
		if (num < 0)
		{
			return;
		}
		this.AddGoodsIcon(num, 2, 1);
	}

	private int GetLuckyNum()
	{
		if (!this.luckyCheckBox.Check)
		{
			return 0;
		}
		return Convert.ToInt32(this.XinyunNubText.Text);
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

	public void RefreshGoodsCount()
	{
	}

	private ListBox Equip = new ListBox();

	private ListBox Qianhua = new ListBox();

	private ListBox Fu = new ListBox();

	public DPSelectedItemEventHandler DPSelectedItem;

	private GCheckBox luckyCheckBox;

	private GProgressBar progressBar;

	private GTextBlockOutLine TipinText;

	private GTextBlockOutLine OddsText;

	private GTextBlockOutLine XinyunNubText;

	private GTextBlockOutLine TipinsNubText;

	private GTextBlockOutLine YinLiang1Text;

	private int EnchanceDbID = -1;

	private int RockGoodsID = -1;

	private int LuckyNum;

	private int _TimerCount;

	private DispatcherTimer _Timer;

	private int[] tpMenuItemIDs = new int[]
	{
		1,
		2
	};

	private string[] tiMenuItemNames = new string[]
	{
		Global.GetLang("1个"),
		Global.GetLang("2个")
	};

	private GMenuPart menuPart;

	private NoBorderWindow MenuWindow;

	private ObservableCollection _equipIcon;

	private bool _Enchancing;
}
