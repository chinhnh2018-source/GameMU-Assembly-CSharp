using System;
using HSGameEngine.GameEngine.SilverLight;
using Server.Tools;

public class GNoHitWindow : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.progressText);
		this.progressText.TextColor = new SolidColorBrush(4278255360U);
		this.progressText.Text = string.Empty;
		this.progressText.TextSize = (double)HSTextField.defaultFontSize;
		this.progressText.HorizontalAlignment = global::Layout.Center;
		this.progressText.VerticalAlignment = global::Layout.Center;
	}

	public double BodyWidth
	{
		set
		{
			this.Container.Width = value;
		}
	}

	public double BodyHeight
	{
		set
		{
			this.Container.Height = value;
		}
	}

	public int TimerCount
	{
		get
		{
			return this._TimerCount;
		}
		set
		{
			this._TimerCount = value;
		}
	}

	public string TimerHint
	{
		get
		{
			return this._TimerHint;
		}
		set
		{
			this._TimerHint = value;
		}
	}

	public override void Destroy()
	{
		this.StopUITimer();
	}

	public void MyStart()
	{
		this.Container.Background = new SolidColorBrush(ColorSL.FromArgb(255, 28, 40, 48));
		this.Container.BackgroundAlpha = 0.5;
		this.StartUITimer();
	}

	private void StartUITimer()
	{
		if (this.UITimer != null)
		{
			return;
		}
		this.UITimer = new DispatcherTimer("GHotWindow_UITimer");
		this.UITimer.Interval = TimeSpan.FromMilliseconds(1000.0);
		this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
		this.UITimer.Start();
		Canvas.SetLeft(this.progressText, (int)(this.Container.Width - this.progressText.RealSize.Width) / 2);
		Canvas.SetTop(this.progressText, (int)(this.Container.Height - this.progressText.RealSize.Height) / 2);
	}

	private void StopUITimer()
	{
		if (this.UITimer != null)
		{
			this.UITimer.Stop();
			this.UITimer.Tick = null;
			this.UITimer = null;
		}
	}

	private void UITimer_Tick(object sender, object e)
	{
		this._TimerCount--;
		this.progressText.Text = StringUtil.substitute("{0}: {1}", new object[]
		{
			this.TimerHint,
			this._TimerCount
		});
		Canvas.SetLeft(this.progressText, (this.Container.Width - this.progressText.RealSize.Width) / 2.0);
		if (this._TimerCount <= 0)
		{
			this.StopUITimer();
			if (this.TimerCompleted != null)
			{
				this.TimerCompleted.Invoke(this, EventArgs.Empty);
			}
		}
	}

	private GTextBlockOutLine progressText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private int _TimerCount = 10;

	public EventHandler TimerCompleted;

	private DispatcherTimer UITimer;

	private string _TimerHint;
}
