using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class HuiKuiHuodongPart : UserControl
{
	public override void Destroy()
	{
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	protected override void InitializeComponent()
	{
		this.timeText.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.timeText, 306);
		Canvas.SetTop(this.timeText, 197);
		this.Container.Children.Add(this.timeText);
		this.yb2Text.TextColor = new SolidColorBrush(3669815U);
		Canvas.SetLeft(this.yb2Text, 351);
		Canvas.SetTop(this.yb2Text, 73);
		this.Container.Children.Add(this.yb2Text);
		this.yb5Text.TextColor = new SolidColorBrush(3669815U);
		Canvas.SetLeft(this.yb5Text, 351);
		Canvas.SetTop(this.yb5Text, 92);
		this.Container.Children.Add(this.yb5Text);
		this.yb7Text.TextColor = new SolidColorBrush(3669815U);
		Canvas.SetLeft(this.yb7Text, 351);
		Canvas.SetTop(this.yb7Text, 110);
		this.Container.Children.Add(this.yb7Text);
		this.yb15Text.TextColor = new SolidColorBrush(3669815U);
		Canvas.SetLeft(this.yb15Text, 351);
		Canvas.SetTop(this.yb15Text, 129);
		this.Container.Children.Add(this.yb15Text);
		this.iphoneText.HorizontalAlignment = global::Layout.Center;
		this.iphoneText.VerticalAlignment = global::Layout.Center;
		this.iphoneText.TextClick = new UIEventEventHandler(this.TextClick);
		Canvas.SetLeft(this.iphoneText, 351);
		Canvas.SetTop(this.iphoneText, 173);
		this.Container.Children.Add(this.iphoneText);
		this.iphoneText.Text = Global.GetLang("领取iPhone 5");
		this.iphoneText.SetSpecialText(Global.GetLang("领取iPhone 5"), new SolidColorBrush(3669815U), true, Super.GetJSurl(4), true);
		this.LingQuIcon = U3DUtils.NEW<GIcon>();
		this.LingQuIcon.Width = 35.0;
		this.LingQuIcon.Height = 21.0;
		this.LingQuIcon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 35.0, 21.0, 3.0, 2.0));
		this.LingQuIcon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 35.0, 21.0, 3.0, 2.0));
		this.LingQuIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		this.LingQuIcon.Text = Global.GetLang("领取");
		this.Container.Children.Add(this.LingQuIcon);
		this.LingQuIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			GameInstance.Game.SpriteGetTo60Award((int)(s as GIcon).Tag);
		};
		this.leve2ProgressBar = new GImgProgressBar();
		this.leve2ProgressBar.BodyWidth = 132.0;
		this.leve2ProgressBar.BodyHeight = 5.0;
		this.leve2ProgressBar.RadiusX = 0;
		this.leve2ProgressBar.RadiusY = 0;
		this.leve2ProgressBar.ProgressBar_Size = new SizeSL(132.0, 5.0);
		this.leve2ProgressBar.ProgressBar_Source = Global.GetGameResImage("Images/Plate/levelProgressbar.png");
		this.leve2ProgressBar.ProgessTextColor = new SolidColorBrush(uint.MaxValue);
		this.leve2ProgressBar.ProgTextMargin = new Thickness(0.0, -5.0, 0.0, 0.0);
		this.Container.Children.Add(this.leve2ProgressBar);
		Canvas.SetLeft(this.leve2ProgressBar, 213);
		Canvas.SetTop(this.leve2ProgressBar, 80);
		this.leve5ProgressBar = new GImgProgressBar();
		this.leve5ProgressBar.BodyWidth = 132.0;
		this.leve5ProgressBar.BodyHeight = 5.0;
		this.leve5ProgressBar.ProgressBar_Source = Global.GetGameResImage("Images/Plate/levelProgressbar.png");
		this.leve5ProgressBar.ProgessTextColor = new SolidColorBrush(uint.MaxValue);
		this.leve5ProgressBar.ProgressBar_Size = new SizeSL(132.0, 5.0);
		this.leve5ProgressBar.ProgTextMargin = new Thickness(0.0, -5.0, 0.0, 0.0);
		this.Container.Children.Add(this.leve5ProgressBar);
		Canvas.SetLeft(this.leve5ProgressBar, 213);
		Canvas.SetTop(this.leve5ProgressBar, 98);
		this.leve7ProgressBar = new GImgProgressBar();
		this.leve7ProgressBar.BodyWidth = 132.0;
		this.leve7ProgressBar.BodyHeight = 5.0;
		this.leve7ProgressBar.ProgressBar_Source = Global.GetGameResImage("Images/Plate/levelProgressbar.png");
		this.leve7ProgressBar.ProgressBar_Size = new SizeSL(132.0, 5.0);
		this.leve7ProgressBar.ProgTextMargin = new Thickness(0.0, -5.0, 0.0, 0.0);
		this.leve7ProgressBar.ProgessTextColor = new SolidColorBrush(uint.MaxValue);
		this.Container.Children.Add(this.leve7ProgressBar);
		Canvas.SetLeft(this.leve7ProgressBar, 213);
		Canvas.SetTop(this.leve7ProgressBar, 116);
		this.leve15ProgressBar = new GImgProgressBar();
		this.leve15ProgressBar.BodyWidth = 132.0;
		this.leve15ProgressBar.BodyHeight = 5.0;
		this.leve15ProgressBar.ProgressBar_Source = Global.GetGameResImage("Images/Plate/levelProgressbar.png");
		this.leve15ProgressBar.ProgressBar_Size = new SizeSL(132.0, 5.0);
		this.leve15ProgressBar.ProgTextMargin = new Thickness(0.0, -5.0, 0.0, 0.0);
		this.leve15ProgressBar.ProgessTextColor = new SolidColorBrush(uint.MaxValue);
		this.Container.Children.Add(this.leve15ProgressBar);
		Canvas.SetLeft(this.leve15ProgressBar, 213);
		Canvas.SetTop(this.leve15ProgressBar, 134);
		this.leve100ProgressBar = new GImgProgressBar();
		this.leve100ProgressBar.BodyWidth = 132.0;
		this.leve100ProgressBar.BodyHeight = 5.0;
		this.leve100ProgressBar.ProgressBar_Source = Global.GetGameResImage("Images/Plate/levelProgressbar.png");
		this.leve100ProgressBar.ProgressBar_Size = new SizeSL(132.0, 5.0);
		this.leve100ProgressBar.ProgTextMargin = new Thickness(0.0, -5.0, 0.0, 0.0);
		this.leve100ProgressBar.ProgessTextColor = new SolidColorBrush(uint.MaxValue);
		this.Container.Children.Add(this.leve100ProgressBar);
		Canvas.SetLeft(this.leve100ProgressBar, 213);
		Canvas.SetTop(this.leve100ProgressBar, 180);
	}

	public void InitPartData()
	{
		if (!this.FirstInitPartData)
		{
			return;
		}
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/UpLevelAward.Xml");
		if (isolateResXml == null)
		{
			return;
		}
		this.yb2Text.Text = StringUtil.substitute(Global.GetLang("{0}绑定钻石"), new object[]
		{
			Global.GetXElementAttributeStr(Global.GetXElement(isolateResXml, "Level", "MaxDay", "2"), "AwardYuanBao")
		});
		this.yb5Text.Text = StringUtil.substitute(Global.GetLang("{0}绑定钻石"), new object[]
		{
			Global.GetXElementAttributeStr(Global.GetXElement(isolateResXml, "Level", "MaxDay", "5"), "AwardYuanBao")
		});
		this.yb7Text.Text = StringUtil.substitute(Global.GetLang("{0}绑定钻石"), new object[]
		{
			Global.GetXElementAttributeStr(Global.GetXElement(isolateResXml, "Level", "MaxDay", "7"), "AwardYuanBao")
		});
		this.yb15Text.Text = StringUtil.substitute(Global.GetLang("{0}绑定钻石"), new object[]
		{
			Global.GetXElementAttributeStr(Global.GetXElement(isolateResXml, "Level", "MaxDay", "15"), "AwardYuanBao")
		});
		this.RefreshData();
	}

	public void RefreshData()
	{
		string regTime = Global.Data.roleData.RegTime;
		DateTime dateTime = default(DateTime);
		DateTime.TryParse(regTime, ref dateTime);
		double num = (double)(Global.GetCorrectLocalTime() - dateTime.Ticks / 10000L);
		num /= 1000.0;
		double num2 = num % 86400.0;
		this.timeText.Text = StringUtil.substitute(Global.GetLang("{0}天{1}时{2}分"), new object[]
		{
			(int)(num / 86400.0),
			(int)(num2 / 3600.0),
			(int)(num2 % 3600.0 / 60.0)
		});
		int num3 = Math.Min(100, Global.Data.roleData.Level);
		int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.To60or100);
		int num4 = -1;
		this.LingQuIcon.Visibility = false;
		if ((roleCommonUseParamsValue & 1) == 1)
		{
			num4 = 0;
			this.leve2ProgressBar.Percent = 1.0;
			this.leve5ProgressBar.Percent = 1.0;
			this.leve7ProgressBar.Percent = 1.0;
			this.leve15ProgressBar.Percent = 1.0;
			this.LingQuIcon.Visibility = true;
		}
		else if ((roleCommonUseParamsValue & 2) == 2)
		{
			num4 = 1;
			this.leve2ProgressBar.Percent = 0.0;
			this.leve5ProgressBar.Percent = 1.0;
			this.leve7ProgressBar.Percent = 1.0;
			this.leve15ProgressBar.Percent = 1.0;
			this.LingQuIcon.Visibility = true;
		}
		else if ((roleCommonUseParamsValue & 4) == 4)
		{
			num4 = 2;
			this.leve2ProgressBar.Percent = 0.0;
			this.leve5ProgressBar.Percent = 0.0;
			this.leve7ProgressBar.Percent = 1.0;
			this.leve15ProgressBar.Percent = 1.0;
			this.LingQuIcon.Visibility = true;
		}
		else if ((roleCommonUseParamsValue & 8) == 8)
		{
			num4 = 3;
			this.leve2ProgressBar.Percent = 0.0;
			this.leve5ProgressBar.Percent = 0.0;
			this.leve7ProgressBar.Percent = 0.0;
			this.leve15ProgressBar.Percent = 1.0;
			this.LingQuIcon.Visibility = true;
		}
		if ((roleCommonUseParamsValue & 32) == 32)
		{
			this.LingQuIcon.EnableIcon = false;
			this.LingQuIcon.Text = Global.GetLang("已领取");
		}
		if (num < 8640000.0 || (roleCommonUseParamsValue & 16) == 16)
		{
			this.leve100ProgressBar.Percent = (double)(num3 / 100);
			this.leve100ProgressBar.ProgessText = StringUtil.substitute("{0}/100", new object[]
			{
				num3
			});
		}
		else
		{
			this.leve100ProgressBar.Percent = 0.0;
		}
		this.iphoneText.Visibility = ((roleCommonUseParamsValue & 16) == 16);
		Canvas.SetLeft(this.LingQuIcon, 410);
		Canvas.SetTop(this.LingQuIcon, 72 + 19 * num4);
		this.LingQuIcon.Tag = num4 + 1;
	}

	public bool TextClick(object sender, BaseEventArgs e)
	{
		Super.ExternalNavigateURL(Super.GetJSurl(4));
		return true;
	}

	private GTextBlockOutLine timeText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GImgProgressBar leve2ProgressBar = new GImgProgressBar();

	private GImgProgressBar leve5ProgressBar = new GImgProgressBar();

	private GImgProgressBar leve7ProgressBar = new GImgProgressBar();

	private GImgProgressBar leve15ProgressBar = new GImgProgressBar();

	private GImgProgressBar leve100ProgressBar = new GImgProgressBar();

	private GIcon LingQuIcon;

	private GTextBlockOutLine yb2Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine yb5Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine yb7Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine yb15Text = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockEx iphoneText = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);

	private bool FirstInitPartData = true;
}
