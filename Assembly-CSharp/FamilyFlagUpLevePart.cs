using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class FamilyFlagUpLevePart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.TongQianText);
		this.TongQianText.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.TongQianText, 198);
		Canvas.SetTop(this.TongQianText, 50);
		this.Container.Children.Add(this.Goods1Text);
		this.Goods1Text.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.Goods1Text, 198);
		Canvas.SetTop(this.Goods1Text, 69);
		this.Container.Children.Add(this.Goods2Text);
		this.Goods2Text.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.Goods2Text, 198);
		Canvas.SetTop(this.Goods2Text, 89);
		this.Container.Children.Add(this.Goods3Text);
		this.Goods3Text.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.Goods3Text, 198);
		Canvas.SetTop(this.Goods3Text, 109);
		this.Container.Children.Add(this.Goods4Text);
		this.Goods4Text.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.Goods4Text, 198);
		Canvas.SetTop(this.Goods4Text, 129);
		this.Container.Children.Add(this.Goods5Text);
		this.Goods5Text.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.Goods5Text, 198);
		Canvas.SetTop(this.Goods5Text, 149);
		this.Container.Children.Add(this.BangLingImg);
		this.BangLingImg.Height = 116.0;
		this.BangLingImg.Width = 107.0;
		Canvas.SetLeft(this.BangLingImg, 22);
		Canvas.SetTop(this.BangLingImg, 32);
		this.thisCtrl = this;
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
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("战旗升级");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.FlagUpLeve();
		};
		Canvas.SetLeft(gicon, 44);
		Canvas.SetTop(gicon, 201);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("取 消");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					IDType = 0,
					ID = 2
				});
			}
		};
		Canvas.SetLeft(gicon, 172);
		Canvas.SetTop(gicon, 201);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData(BangQiInfoData bangQiInfoData, int bhid)
	{
		this.BHID = bhid;
		int bangQiLevel = bangQiInfoData.BangQiLevel;
		this._FlagArray = ConfigSystemParam.GetSystemParamIntArrayByName("JunQiDecoIDs", ',');
		if (this._FlagArray != null && this._FlagArray.Length >= 4 && bangQiLevel > 0 && bangQiLevel < 5)
		{
			this.Deco = Global.GetDecoration(this._FlagArray[bangQiLevel - 1], GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
			this.Deco.Coordinate = new Point(200, 295);
		}
		this.MyBangQiInfoData = bangQiInfoData;
		XElement gameResXml = Global.GetGameResXml("Config/FlagUpLevel.Xml");
		if (gameResXml != null)
		{
			XElement xelement = Global.GetXElement(gameResXml, "Level", "ID", (bangQiInfoData.BangQiLevel + 1).ToString());
			if (xelement != null)
			{
				this.TongQianText.Text = Global.GetXElementAttributeStr(xelement, "UseMoney");
				this.Goods1Text.Text = Global.GetXElementAttributeStr(xelement, "GoodsOnedouble");
				this.Goods2Text.Text = Global.GetXElementAttributeStr(xelement, "GoodsTwodouble");
				this.Goods3Text.Text = Global.GetXElementAttributeStr(xelement, "GoodsThreedouble");
				this.Goods4Text.Text = Global.GetXElementAttributeStr(xelement, "GoodsFourdouble");
				this.Goods5Text.Text = Global.GetXElementAttributeStr(xelement, "GoodsFivedouble");
			}
		}
		GameInstance.Game.SpriteGetBangGongHist(this.BHID);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	public void NotifyBangGongHist(BangHuiBagData bangHuiBagData)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (bangHuiBagData == null)
		{
			return;
		}
		this.TongQianText.Text = StringUtil.substitute(Global.GetLang("{0}万/{1}万"), new object[]
		{
			(double)bangHuiBagData.TongQian / 10000.0,
			Convert.ToDouble(this.TongQianText.Text) / 10000.0
		});
		this.Goods1Text.Text = StringUtil.substitute("{0}/{1}", new object[]
		{
			bangHuiBagData.Goods1Num.ToString(),
			this.Goods1Text.Text
		});
		this.Goods2Text.Text = StringUtil.substitute("{0}/{1}", new object[]
		{
			bangHuiBagData.Goods2Num.ToString(),
			this.Goods2Text.Text
		});
		this.Goods3Text.Text = StringUtil.substitute("{0}/{1}", new object[]
		{
			bangHuiBagData.Goods3Num.ToString(),
			this.Goods3Text.Text
		});
		this.Goods4Text.Text = StringUtil.substitute("{0}/{1}", new object[]
		{
			bangHuiBagData.Goods4Num.ToString(),
			this.Goods4Text.Text
		});
		this.Goods5Text.Text = StringUtil.substitute("{0}/{1}", new object[]
		{
			bangHuiBagData.Goods5Num.ToString(),
			this.Goods5Text.Text
		});
	}

	public void FlagUpLeve()
	{
		if (!Global.IsBangHuiLeader(Global.Data.roleData, this.BHID))
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("只有首领才能执行战旗升级操作!"), 0, -1, -1, 0);
			return;
		}
		if (this.MyBangQiInfoData.BangQiLevel >= Global.MaxBangHuiFlagLevel)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("您的战盟战旗等级已经达到了最高等级，无法再升级!"), 0, -1, -1, 0);
			return;
		}
		GameInstance.Game.SpriteUpLevelBangQi(this.MyBangQiInfoData.BangQiLevel + 1);
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
		Super.CleanUpAllChildWindows(this.Container);
	}

	public override void Destroy()
	{
		this.CleanUpChildWindows();
		if (this.Deco != null)
		{
			Global.RemoveObject(this.Deco, true);
			this.Deco = null;
		}
	}

	private GDecoration Deco;

	private int[] _FlagArray;

	private LoadingWindow LoadingWin;

	private int BHID;

	private BangQiInfoData MyBangQiInfoData;

	private GTextBlockOutLine TongQianText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine Goods1Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine Goods2Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine Goods3Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine Goods4Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine Goods5Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Image BangLingImg = new Image();

	private SpriteSL thisCtrl = new SpriteSL();

	public DPSelectedItemEventHandler DPSelectedItem;
}
