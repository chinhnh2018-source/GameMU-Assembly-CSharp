using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class AllWorldMapCarryPart : UserControl
{
	public AllWorldMapCarryPart()
	{
		this.thisCtrl = this;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.MapName);
		this.MapName.TextColor = new SolidColorBrush(2925502U);
		Canvas.SetLeft(this.MapName, 86);
		Canvas.SetTop(this.MapName, 18);
		this.Container.Children.Add(this.LimitLev);
		this.LimitLev.TextColor = new SolidColorBrush(2925502U);
		Canvas.SetLeft(this.LimitLev, 86);
		Canvas.SetTop(this.LimitLev, 38);
		this.Container.Children.Add(this.FitLev);
		this.FitLev.TextColor = new SolidColorBrush(2925502U);
		Canvas.SetLeft(this.FitLev, 86);
		Canvas.SetTop(this.FitLev, 58);
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
		gicon.Text = Global.GetLang("快速传送");
		Canvas.SetLeft(gicon, 15);
		Canvas.SetTop(gicon, 176);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = this.iMapdouble,
					IDType = -1
				});
			}
		};
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("徒步前往");
		Canvas.SetLeft(gicon, 99);
		Canvas.SetTop(gicon, 176);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = this.iMapdouble,
					IDType = 0
				});
			}
		};
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 52.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 52.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 52.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("取 消");
		Canvas.SetLeft(gicon, 217);
		Canvas.SetTop(gicon, 176);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = this.iMapdouble,
					IDType = 1
				});
			}
		};
	}

	public void InitPartData(int iMap)
	{
		this.iMapdouble = iMap;
		this.MapName.Text = StringUtil.substitute("{0}[{1}]", new object[]
		{
			ConfigSettings.GetMapNameByCode(iMap, false),
			Global.GetMapPKModeName(iMap)
		});
		this.LimitLev.Text = Global.GetLang("无限制");
		this.FitLev.Text = Global.GetLang("稍后开放");
		XElement gameResXml = Global.GetGameResXml("Config/Settings.Xml");
		if (gameResXml != null)
		{
			XElement xelement = Global.GetXElement(gameResXml, "Map", "Code", iMap.ToString());
			this.FitLev.Text = Global.GetXElementAttributeStr(xelement, "FitLevel");
		}
		int num = (int)ConfigSystemParam.GetSystemParamIntByName("MapChuanSong");
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.TipType = 1;
		gicon.Width = 32.0;
		gicon.Height = 32.0;
		gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
		{
			num,
			0,
			-1,
			-1
		});
		Canvas.SetLeft(gicon, 201);
		Canvas.SetTop(gicon, 48);
		this.Container.Children.Add(gicon);
		if (Global.GetTotalGoodsCountByID(num) <= 0)
		{
			gicon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(goodsXmlNodeByID.IconCode, string.Empty), true, 0);
		}
		else
		{
			gicon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(goodsXmlNodeByID.IconCode, string.Empty), false, 0);
		}
	}

	public override void Destroy()
	{
		this.Container.Children.Clear();
	}

	private SpriteSL thisCtrl;

	private int iMapdouble;

	private GTextBlockOutLine MapName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine LimitLev = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine FitLev = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public DPSelectedItemEventHandler DPSelectedItem;
}
