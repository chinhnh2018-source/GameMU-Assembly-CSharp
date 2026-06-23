using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class OnlineGiftPart : UserControl
{
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

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.YinLiangText);
		Canvas.SetLeft(this.YinLiangText, 176);
		Canvas.SetTop(this.YinLiangText, 76);
		this.YinLiangText.TextColor = new SolidColorBrush(10626862U);
		this.YinLiangText.FontSize = HSTextField.defaultFontSize;
		this.YinLiangText.HorizontalAlignment = global::Layout.Center;
		this.YinLiangText.VerticalAlignment = global::Layout.Center;
		this.Container.Children.Add(this.ultimoText);
		Canvas.SetLeft(this.ultimoText, 180);
		Canvas.SetTop(this.ultimoText, 148);
		this.ultimoText.TextColor = new SolidColorBrush(10626862U);
		this.ultimoText.FontSize = HSTextField.defaultFontSize;
		this.ultimoText.HorizontalAlignment = global::Layout.Center;
		this.ultimoText.VerticalAlignment = global::Layout.Center;
		this.Container.Children.Add(this.thisTimeText);
		Canvas.SetLeft(this.thisTimeText, 180);
		Canvas.SetTop(this.thisTimeText, 168);
		this.thisTimeText.TextColor = new SolidColorBrush(10626862U);
		this.thisTimeText.FontSize = HSTextField.defaultFontSize;
		this.thisTimeText.HorizontalAlignment = global::Layout.Center;
		this.thisTimeText.VerticalAlignment = global::Layout.Center;
		this.Container.Children.Add(this.Container2);
		this.Container2.Children.Add(this.LiftPanel);
		this.LiftPanel.Children.Add(this.Time1Text);
		Canvas.SetLeft(this.Time1Text, 65);
		Canvas.SetTop(this.Time1Text, 228);
		this.Time1Text.TextColor = new SolidColorBrush(uint.MaxValue);
		this.Time1Text.FontSize = HSTextField.defaultFontSize;
		this.Time1Text.HorizontalAlignment = global::Layout.Center;
		this.Time1Text.VerticalAlignment = global::Layout.Center;
		this.LiftPanel.Children.Add(this.Time2Text);
		Canvas.SetLeft(this.Time2Text, 65);
		Canvas.SetTop(this.Time2Text, 250);
		this.Time2Text.TextColor = new SolidColorBrush(uint.MaxValue);
		this.Time2Text.FontSize = HSTextField.defaultFontSize;
		this.Time2Text.HorizontalAlignment = global::Layout.Center;
		this.Time2Text.VerticalAlignment = global::Layout.Center;
		this.LiftPanel.Children.Add(this.Time3Text);
		Canvas.SetLeft(this.Time3Text, 65);
		Canvas.SetTop(this.Time3Text, 272);
		this.Time3Text.TextColor = new SolidColorBrush(uint.MaxValue);
		this.Time3Text.FontSize = HSTextField.defaultFontSize;
		this.Time3Text.HorizontalAlignment = global::Layout.Center;
		this.Time3Text.VerticalAlignment = global::Layout.Center;
		this.Container2.Children.Add(this.RightPanel);
		this.RightPanel.Children.Add(this.Award1Text);
		Canvas.SetLeft(this.Award1Text, 202);
		Canvas.SetTop(this.Award1Text, 225);
		this.Award1Text.TextColor = new SolidColorBrush(10626862U);
		this.Award1Text.FontSize = HSTextField.defaultFontSize;
		this.Award1Text.HorizontalAlignment = global::Layout.Center;
		this.Award1Text.VerticalAlignment = global::Layout.Center;
		this.RightPanel.Children.Add(this.Award2Text);
		Canvas.SetLeft(this.Award2Text, 202);
		Canvas.SetTop(this.Award2Text, 247);
		this.Award2Text.TextColor = new SolidColorBrush(10626862U);
		this.Award2Text.FontSize = HSTextField.defaultFontSize;
		this.Award2Text.HorizontalAlignment = global::Layout.Center;
		this.Award2Text.VerticalAlignment = global::Layout.Center;
		this.RightPanel.Children.Add(this.Award3Text);
		Canvas.SetLeft(this.Award3Text, 202);
		Canvas.SetTop(this.Award3Text, 269);
		this.Award3Text.TextColor = new SolidColorBrush(10626862U);
		this.Award3Text.FontSize = HSTextField.defaultFontSize;
		this.Award3Text.HorizontalAlignment = global::Layout.Center;
		this.Award3Text.VerticalAlignment = global::Layout.Center;
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = this.Width;
		this.Container.Height = this.Height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Name = "LingQu1";
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("领取");
		Canvas.SetLeft(gicon, 432);
		Canvas.SetTop(gicon, 222);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			this.GetAward(1);
		};
		GIcon gicon2 = U3DUtils.NEW<GIcon>();
		gicon2.Name = "LingQu2";
		gicon2.Width = 66.0;
		gicon2.Height = 21.0;
		gicon2.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon2.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon2.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		gicon2.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon2.Text = Global.GetLang("领取");
		Canvas.SetLeft(gicon2, 432);
		Canvas.SetTop(gicon2, 248);
		this.Container.Children.Add(gicon2);
		gicon2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			this.GetAward(2);
		};
		GIcon gicon3 = U3DUtils.NEW<GIcon>();
		gicon3.Name = "LingQu3";
		gicon3.Width = 66.0;
		gicon3.Height = 21.0;
		gicon3.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon3.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon3.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_nouse.png"), 66.0, 21.0, 3.0, 2.0));
		gicon3.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon3.Text = Global.GetLang("领取");
		Canvas.SetLeft(gicon3, 432);
		Canvas.SetTop(gicon3, 274);
		this.Container.Children.Add(gicon3);
		gicon3.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			this.GetAward(3);
		};
	}

	public void InitPartData()
	{
		if (!this.FirstInitPartData)
		{
			return;
		}
		this.FirstInitPartData = false;
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/OnlieTimeGift.Xml");
		if (isolateResXml == null)
		{
			return;
		}
		int num = 0;
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Item");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (i == 0)
			{
				this.TimeOl1 = Global.GetXElementAttributeInt(xelement, "TimeOl");
				this.Time1Text.Text = this.TimeOl1 + Global.GetLang(" 小时");
				this.Award1Text.Text = StringUtil.substitute(Global.GetLang("免费领取 {0} 金币"), new object[]
				{
					Global.GetXElementAttributeStr(xelement, "YinLiang")
				});
				num += Global.GMax(Global.GetXElementAttributeInt(xelement, "YinLiang"), 0);
			}
			else if (i == 1)
			{
				this.TimeOl2 = Global.GetXElementAttributeInt(xelement, "TimeOl");
				this.Time2Text.Text = this.TimeOl2 + Global.GetLang(" 小时");
				this.Award2Text.Text = StringUtil.substitute(Global.GetLang("免费领取 {0} 金币"), new object[]
				{
					Global.GetXElementAttributeStr(xelement, "YinLiang")
				});
				num += Global.GMax(Global.GetXElementAttributeInt(xelement, "YinLiang"), 0);
			}
			else if (i == 2)
			{
				this.TimeOl3 = Global.GetXElementAttributeInt(xelement, "TimeOl");
				this.Time3Text.Text = this.TimeOl3 + Global.GetLang(" 小时");
				this.Award3Text.Text = StringUtil.substitute(Global.GetLang("免费领取 {0} 金币"), new object[]
				{
					Global.GetXElementAttributeStr(xelement, "YinLiang")
				});
				num += Global.GMax(Global.GetXElementAttributeInt(xelement, "YinLiang"), 0);
			}
		}
		string xapParamByName = Super.GetXapParamByName("country", string.Empty);
		if ("korea" == xapParamByName)
		{
			this.YinLiangText.Text = StringUtil.substitute("{0}", new object[]
			{
				num
			});
		}
		else if ("taiwan" == xapParamByName)
		{
			this.YinLiangText.Text = StringUtil.substitute("{0}NT", new object[]
			{
				num / 1000 * 5
			});
		}
		else if ("vietnam" == xapParamByName)
		{
			this.YinLiangText.Text = StringUtil.substitute("{0}VND", new object[]
			{
				num / 1000 * 4000
			});
		}
		else
		{
			this.YinLiangText.Text = StringUtil.substitute("{0}RMB", new object[]
			{
				num / 1000
			});
		}
		this.RefreshHuodongData();
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.tc.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Container);
	}

	public void ResetGetNewData()
	{
		this.FirstGetNewData = true;
	}

	public void GetNewData()
	{
		if (!this.FirstGetNewData)
		{
			return;
		}
		this.FirstGetNewData = false;
	}

	private void GetAward(int whichOne)
	{
		GameInstance.Game.SpriteGetMTimeGift(whichOne);
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.tc.Container.Children.Add(this.LoadingWin);
	}

	private void SetLingQuIconState(int num, bool havelingQu, bool canLingQu)
	{
		GIcon gicon = U3DUtils.AS<GIcon>(this.Container.FindName(StringUtil.substitute("LingQu{0}", new object[]
		{
			num
		})));
		if (havelingQu)
		{
			gicon.Text = Global.GetLang("已领");
			gicon.EnableIcon = false;
		}
		else
		{
			gicon.Text = Global.GetLang("领取");
			gicon.EnableIcon = canLingQu;
		}
	}

	public void RefreshHuodongData()
	{
		this.ultimoText.Text = StringUtil.substitute(Global.GetLang("{0}小时{1}分钟"), new object[]
		{
			Global.Data.MyHuoDongData.LastMTime / 3600,
			Global.Data.MyHuoDongData.LastMTime % 3600 / 60
		});
		this.thisTimeText.Text = StringUtil.substitute(Global.GetLang("{0}小时{1}分钟"), new object[]
		{
			Global.Data.MyHuoDongData.CurMTime / 3600,
			Global.Data.MyHuoDongData.CurMTime % 3600 / 60
		});
		int onlineGiftState = Global.Data.MyHuoDongData.OnlineGiftState;
		this.SetLingQuIconState(1, (onlineGiftState & 1) == 1, Global.Data.MyHuoDongData.CurMTime / 3600 >= this.TimeOl1);
		this.SetLingQuIconState(2, (onlineGiftState & 2) == 2, Global.Data.MyHuoDongData.CurMTime / 3600 >= this.TimeOl2);
		this.SetLingQuIconState(3, (onlineGiftState & 4) == 4, Global.Data.MyHuoDongData.CurMTime / 3600 >= this.TimeOl3);
	}

	public void GetAwardResult(int result, int roleID)
	{
		if (this.LoadingWin != null)
		{
			this.tc.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result < 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取礼物时出错: {0}"), new object[]
			{
				result
			}), 0, -1, -1, 0);
		}
		this.RefreshHuodongData();
	}

	private LoadingWindow LoadingWin;

	private int TimeOl1;

	private int TimeOl2;

	private int TimeOl3;

	private bool FirstInitPartData = true;

	private bool FirstGetNewData = true;

	private GTextBlockOutLine YinLiangText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine ultimoText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine thisTimeText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Canvas Container2 = new Canvas();

	private Canvas LiftPanel = new Canvas();

	private GTextBlockOutLine Time1Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine Time2Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine Time3Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Canvas RightPanel = new Canvas();

	private GTextBlockOutLine Award1Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine Award2Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine Award3Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTabControl _tc;
}
