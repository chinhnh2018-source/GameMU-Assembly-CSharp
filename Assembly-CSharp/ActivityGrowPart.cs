using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;

public class ActivityGrowPart : UserControl
{
	public ActivityGrowPart()
	{
		this.txtProposal.TextFontWrapping = TextWrapping.Wrap;
		this.txtDescription.TextFontWrapping = TextWrapping.Wrap;
		this.txtTips.TextFontWrapping = TextWrapping.Wrap;
		this.txtDescription.mouseEnabled = false;
		this.txtProposal.mouseEnabled = false;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public Brush TodaySYBackground
	{
		set
		{
			this.TodaySY.Background = value;
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

	protected override void InitializeComponent()
	{
		this.Container.Width = 514.0;
		this.Container.Height = 325.0;
		Canvas.SetLeft(this.Container, 26);
		Canvas.SetTop(this.Container, 14);
		this.Container.Children.Add(this.ImgTitle1);
		this.ImgTitle1.Width = 98.0;
		this.ImgTitle1.Height = 28.0;
		Canvas.SetLeft(this.ImgTitle1, 5);
		Canvas.SetTop(this.ImgTitle1, 2);
		this.ImgTitle1.HorizontalAlignment = global::Layout.Left;
		this.Container.Children.Add(this.txtProposal);
		this.txtProposal.TextColor = new SolidColorBrush(16725815U);
		this.txtProposal.FontSize = HSTextField.defaultFontSize;
		this.txtProposal.Width = 460.0;
		this.txtProposal.TextWidth = 460.0;
		Canvas.SetLeft(this.txtProposal, 16);
		Canvas.SetTop(this.txtProposal, 35);
		this.txtProposal.HorizontalAlignment = global::Layout.Left;
		this.Container.Children.Add(this.txtDescription);
		this.txtDescription.TextColor = new SolidColorBrush(11394222U);
		this.txtDescription.FontSize = HSTextField.defaultFontSize;
		this.txtDescription.Width = 460.0;
		this.txtDescription.TextWidth = 460.0;
		Canvas.SetLeft(this.txtDescription, 16);
		Canvas.SetTop(this.txtDescription, 57);
		this.txtDescription.HorizontalAlignment = global::Layout.Left;
		this.Container.Children.Add(this.txtTips);
		this.txtTips.TextColor = new SolidColorBrush(16777080U);
		this.txtTips.FontSize = HSTextField.defaultFontSize;
		this.txtTips.Width = 460.0;
		this.txtTips.TextWidth = 460.0;
		Canvas.SetLeft(this.txtTips, 37);
		Canvas.SetTop(this.txtTips, 93);
		this.txtTips.HorizontalAlignment = global::Layout.Left;
		this.Container.Children.Add(this.ImgTitle2);
		this.ImgTitle2.Width = 98.0;
		this.ImgTitle2.Height = 28.0;
		Canvas.SetLeft(this.ImgTitle2, 5);
		Canvas.SetTop(this.ImgTitle2, 196);
		this.ImgTitle2.HorizontalAlignment = global::Layout.Left;
		this.Container.Children.Add(this.TodaySY);
		this.TodaySY.Width = 490.0;
		this.TodaySY.Height = 50.0;
		Canvas.SetLeft(this.TodaySY, 0);
		Canvas.SetTop(this.TodaySY, 180);
		this.TodaySY.Children.Add(this.txtTodayEx);
		this.txtTodayEx.TextColor = new SolidColorBrush(16777215U);
		this.txtTodayEx.FontSize = HSTextField.defaultFontSize;
		Canvas.SetLeft(this.txtTodayEx, 82);
		Canvas.SetTop(this.txtTodayEx, 4);
		this.TodaySY.Children.Add(this.txtTodayPower);
		this.txtTodayPower.TextColor = new SolidColorBrush(16777215U);
		this.txtTodayPower.FontSize = HSTextField.defaultFontSize;
		Canvas.SetLeft(this.txtTodayPower, 255);
		Canvas.SetTop(this.txtTodayPower, 4);
		this.TodaySY.Children.Add(this.txtTodayKillBossNum);
		this.txtTodayKillBossNum.TextColor = new SolidColorBrush(16777215U);
		this.txtTodayKillBossNum.FontSize = HSTextField.defaultFontSize;
		Canvas.SetLeft(this.txtTodayKillBossNum, 95);
		Canvas.SetTop(this.txtTodayKillBossNum, 33);
		this.TodaySY.Children.Add(this.txtTodayClearanceNum);
		this.txtTodayClearanceNum.TextColor = new SolidColorBrush(16777215U);
		this.txtTodayClearanceNum.FontSize = HSTextField.defaultFontSize;
		Canvas.SetLeft(this.txtTodayClearanceNum, 267);
		Canvas.SetTop(this.txtTodayClearanceNum, 33);
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.txtProposal.Text = "    " + Global.GetLang("您的经脉尚未全通，建议尽快冲通！");
		this.txtDescription.Text = "    " + Global.GetLang("冲穴后将带来大幅的属性提升，这些属性是人物变强的基础，再被各种技能百分比增幅后将会变得令人吃惊的强大。");
		this.txtTips.Text = Global.GetLang("* 冲穴消耗的灵力可通可打坐获得");
	}

	public void InitPartData()
	{
		this.ShowGlowProposal();
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
		GameInstance.Game.SpriteGetRoleDailyData();
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.tc.Container.Children.Add(this.LoadingWin);
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

	private void ShowGlowProposal()
	{
		this.txtProposal.Text = string.Empty;
		this.txtDescription.Text = string.Empty;
		this.txtTips.Text = string.Empty;
		XElement gameResXml = Global.GetGameResXml("Config/Activity/Glow.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
		XElement xelement = xelementList[ActivityGrowPart.MERIDIAN_ID];
		this.txtProposal.Text = "    " + Global.GetXElementAttributeStr(xelement, "Proposal");
		this.txtDescription.Text = "    " + Global.GetXElementAttributeStr(xelement, "Description");
		this.txtTips.Text = "* " + Global.GetXElementAttributeStr(xelement, "Tips");
	}

	public void NotifyResult(RoleDailyData roleDailyData)
	{
		this.MyRoleDailyData = roleDailyData;
		if (this.MyRoleDailyData != null)
		{
			int dayOfYear = Global.GetCorrectDateTime().DayOfYear;
			if (dayOfYear == this.MyRoleDailyData.ExpDayID)
			{
				this.txtTodayEx.Text = this.MyRoleDailyData.TodayExp.ToString();
			}
			else
			{
				this.txtTodayEx.Text = "0";
			}
			if (dayOfYear == this.MyRoleDailyData.LingLiDayID)
			{
				this.txtTodayPower.Text = this.MyRoleDailyData.TodayLingLi.ToString();
			}
			else
			{
				this.txtTodayPower.Text = "0";
			}
			if (dayOfYear == this.MyRoleDailyData.KillBossDayID)
			{
				this.txtTodayKillBossNum.Text = this.MyRoleDailyData.TodayKillBoss.ToString();
			}
			else
			{
				this.txtTodayKillBossNum.Text = "0";
			}
			if (dayOfYear == this.MyRoleDailyData.FuBenDayID)
			{
				this.txtTodayClearanceNum.Text = this.MyRoleDailyData.TodayFuBenNum.ToString();
			}
			else
			{
				this.txtTodayClearanceNum.Text = "0";
			}
		}
		else
		{
			this.txtTodayEx.Text = "0";
			this.txtTodayPower.Text = "0";
			this.txtTodayKillBossNum.Text = "0";
			this.txtTodayClearanceNum.Text = "0";
		}
		if (this.LoadingWin != null)
		{
			this.tc.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
	}

	private static int MERIDIAN_ID;

	private LoadingWindow LoadingWin;

	private bool FirstGetNewData = true;

	private RoleDailyData MyRoleDailyData;

	private URLImage ImgTitle1 = new URLImage();

	private GTextBlockOutLine txtProposal = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtDescription = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtTips = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private URLImage ImgTitle2 = new URLImage();

	private Canvas TodaySY = new Canvas();

	private GTextBlockOutLine txtTodayEx = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtTodayPower = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtTodayKillBossNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtTodayClearanceNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTabControl _tc;
}
