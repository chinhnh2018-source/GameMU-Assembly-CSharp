using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class ChangeLineBar : UserControl
{
	public ChangeLineBar()
	{
		this.Width = 94.0;
		this.Height = 19.0;
		this.Container.Width = 94.0;
		this.Container.Height = 19.0;
		this.LineHint.TextColor = new SolidColorBrush(uint.MaxValue);
		this.LineHint.TextSize = (double)HSTextField.defaultFontSize;
		this.StateHint.TextColor = new SolidColorBrush(4278222848U);
		this.StateHint.TextSize = (double)HSTextField.defaultFontSize;
		this.ChangeMenuBarImg(false);
		this.ChangeBackground(false);
	}

	protected override void InitializeComponent()
	{
		base.addEventListener("ROLL_OVER", new MouseEventHandler(this.Container_MouseEnter));
		base.addEventListener("ROLL_OUT", new MouseEventHandler(this.Container_MouseLeave));
		this.Container.Children.Add(this.LineHint);
		this.LineHint.IsHitTestVisible = false;
		this.LineHint.Height = 23.0;
		Canvas.SetLeft(this.LineHint, 7);
		Canvas.SetTop(this.LineHint, 3);
		this.Container.Children.Add(this.StateHint);
		this.StateHint.IsHitTestVisible = false;
		this.StateHint.Height = 23.0;
		Canvas.SetLeft(this.StateHint, 35);
		Canvas.SetTop(this.StateHint, 3);
		this.Container.Children.Add(this.MenuArraow);
		this.MenuArraow.Stretch = global::StretchSL.None;
		this.MenuArraow.IsHitTestVisible = false;
		Canvas.SetLeft(this.MenuArraow, 75);
		Canvas.SetTop(this.MenuArraow, 6);
		string lang = Global.GetLang("点击切换线路\n线路依照在线人数排序\n排序越靠后的线路会越拥挤\n推荐您进入较为顺畅的线路");
	}

	public double PingTicks
	{
		get
		{
			return this._PingTicks;
		}
		set
		{
			this._PingTicks = value;
		}
	}

	private void ChangeBackground(bool hover)
	{
		if (hover)
		{
			this.Container.Background = new ImageBrush(Global.GetGameResImage("Images/Plate/lineBak_Hover.png"));
		}
		else
		{
			this.Container.Background = new ImageBrush(Global.GetGameResImage("Images/Plate/lineBak_Normal.png"));
		}
	}

	private void ChangeMenuBarImg(bool hover)
	{
		if (hover)
		{
			this.MenuArraow.Source = new ImageBrush(Global.GetGameResImage("Images/Plate/lineBakArrow_Active.png"));
		}
		else
		{
			this.MenuArraow.Source = new ImageBrush(Global.GetGameResImage("Images/Plate/lineBakArrow_Normal.png"));
		}
	}

	private string GetLineName()
	{
		if (Global.CurrentListData == null)
		{
			return string.Empty;
		}
		if (Global.CurrentListData.LineID <= 0 || Global.CurrentListData.LineID >= 11)
		{
			return string.Empty;
		}
		return StringUtil.substitute(Global.GetLang("{0}线"), new object[]
		{
			this.LineNames[Global.CurrentListData.LineID - 1]
		});
	}

	public void GetLineInfo()
	{
		double num = (double)Global.GetCorrectLocalTime();
		if (num - this.LastGetLineInfoTicks < 300000.0)
		{
			return;
		}
		this.LastGetLineInfoTicks = num;
		GameInstance.Game.SpriteGetLineInfo();
	}

	public void UpdateShowInfo()
	{
		this.LineHint.Text = this.GetLineName();
		this.StateHint.TextColor = Super.GetLineDataBrush(Global.CurrentListData);
		this.StateHint.Text = StringUtil.substitute("({0})", new object[]
		{
			Super.GetLineDataText(Global.CurrentListData)
		});
	}

	private void Container_MouseEnter(MouseEvent e)
	{
		this.ChangeMenuBarImg(true);
		this.ChangeBackground(true);
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
		}
	}

	private void Container_MouseLeave(MouseEvent e)
	{
		this.ChangeMenuBarImg(false);
		this.ChangeBackground(false);
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Auto;
		}
	}

	private string[] LineNames = new string[]
	{
		Global.GetLang("一"),
		Global.GetLang("二"),
		Global.GetLang("三"),
		Global.GetLang("四"),
		Global.GetLang("五"),
		Global.GetLang("六"),
		Global.GetLang("七"),
		Global.GetLang("八"),
		Global.GetLang("九"),
		Global.GetLang("十")
	};

	private double _PingTicks;

	private double LastGetLineInfoTicks;

	private GTextBlockOutLine LineHint = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine StateHint = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Image MenuArraow = new Image();
}
