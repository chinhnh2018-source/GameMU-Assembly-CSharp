using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class BattleHintPart : UserControl
{
	public BattleHintPart()
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

	public override void Destroy()
	{
		this.StopTimer();
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.Text = string.Empty;
		gtextBlockOutLine.Height = 23.0;
		gtextBlockOutLine.Width = 84.0;
		gtextBlockOutLine.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 206, 0));
		Canvas.SetLeft(gtextBlockOutLine, 95);
		Canvas.SetTop(gtextBlockOutLine, 205);
		this.Container.Children.Add(gtextBlockOutLine);
		this.textBlockTimer = gtextBlockOutLine;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("立刻进入");
		Canvas.SetLeft(gicon, 216);
		Canvas.SetTop(gicon, 209);
		this.Container.Children.Add(gicon);
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
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("下次再说");
		Canvas.SetLeft(gicon, 307);
		Canvas.SetTop(gicon, 209);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 2,
					IDType = 0
				});
			}
		};
	}

	public void InitPartData()
	{
		this.StartTimer();
	}

	private void Timer_Tick(object sender, object e)
	{
		if (this.TotalElapsedSecs <= 0)
		{
			this.StopTimer();
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 2,
					IDType = 0
				});
			}
			return;
		}
		this.textBlockTimer.Text = StringUtil.substitute(Global.GetLang("倒计时: {0}秒"), new object[]
		{
			Global.FormatStr("00", this.TotalElapsedSecs--)
		});
	}

	private void StopTimer()
	{
		if (this.ElapsedTimer == null)
		{
			return;
		}
		this.ElapsedTimer.Tick = null;
		this.ElapsedTimer.Stop();
		this.ElapsedTimer = null;
	}

	private void StartTimer()
	{
	}

	private GTextBlockOutLine textBlockTimer;

	private int TotalElapsedSecs = 58;

	private DispatcherTimer ElapsedTimer;

	public DPSelectedItemEventHandler DPSelectedItem;

	private UserControl thisCtrl;
}
