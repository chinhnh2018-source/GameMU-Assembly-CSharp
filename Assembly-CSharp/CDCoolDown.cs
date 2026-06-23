using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class CDCoolDown : UserControl
{
	public SolidColorBrush BodyColor
	{
		get
		{
			if (this._MaskColor == null)
			{
				this._MaskColor = new SolidColorBrush(1U);
			}
			return this._MaskColor;
		}
		set
		{
			this._MaskColor = value;
		}
	}

	public double MaskAlpha
	{
		get
		{
			return this._MaskAlpha;
		}
		set
		{
			this._MaskAlpha = value;
			this._BodyElem.alpha = value;
		}
	}

	public ImageBrush BackImage
	{
		get
		{
			return this._BackImage;
		}
		set
		{
			this._BackImage = value;
			this.Container.Children.Background = this.BackImage;
		}
	}

	private void onTimer(EventArgs e)
	{
		this._leftTicks -= this._currentInterval;
		if (this._leftTicks <= 0L)
		{
			this.Reset();
			if (this.CoodDownComplete != null)
			{
				this.CoodDownComplete.Invoke(this, EventArgs.Empty);
			}
			return;
		}
		int num = (int)(this._leftTicks / 1000L);
		if (this._isDrawTicks)
		{
			this._TextBlock.Text = (num + 1).ToString();
		}
		this._TextBlock.X = (this.Width - this._TextBlock.Width) / 2.0;
		this._TextBlock.Y = (this.Height - this._TextBlock.Height) / 2.0;
		double r = Math.Max(this.Width, this.Height);
		double num2 = (double)(1L - this._leftTicks / this._totalTicks);
		if (this._isDrawTicks)
		{
			if (this._IsFromLightToDark)
			{
				this.DrawSector(this._BodyElem.graphics, 0.0, 0.0, r, 360.0 * num2, 270.0);
			}
			else
			{
				this.DrawSector(this._BodyElem.graphics, 0.0, 0.0, r, 360.0 - 360.0 * num2, 360.0 * num2 - 90.0);
			}
		}
		else if (this._IsFromLightToDark)
		{
			this.DrawSector(this._BodyElem.graphics, 0.0, 0.0, r, 0.0, 270.0);
		}
		else
		{
			this.DrawSector(this._BodyElem.graphics, 0.0, 0.0, r, 360.0, -90.0);
		}
		this._BodyElem.graphics.endFill();
		this._BodyElem.X = this.Width / 2.0;
		this._BodyElem.Y = this.Height / 2.0;
	}

	public void ManualSet(double manualStartValue, double startValue, bool showText = true)
	{
		this._TextBlock.Visibility = showText;
		double num = 0.0;
		if (manualStartValue > 0.0)
		{
			if (startValue < manualStartValue)
			{
				num = 1.0 - startValue / (manualStartValue * 1.0);
			}
			else
			{
				manualStartValue = startValue;
			}
		}
		this._TextBlock.Text = ((int)(num * 100.0)).ToString() + "%";
		this._TextBlock.X = (this.Width - this._TextBlock.Width) / 2.0;
		this._TextBlock.Y = (this.Height - this._TextBlock.Height) / 2.0;
		double r = Math.Max(this.Width, this.Height);
		if (this._IsFromLightToDark)
		{
			this.DrawSector(this._BodyElem.graphics, 0.0, 0.0, r, 360.0 * num, 270.0);
		}
		else
		{
			this.DrawSector(this._BodyElem.graphics, 0.0, 0.0, r, 360.0 - 360.0 * num, 360.0 * num - 90.0);
		}
		this._BodyElem.graphics.endFill();
		this.Percent = num;
		this._BodyElem.X = this.Width / 2.0;
		this._BodyElem.Y = this.Height / 2.0;
		if (num <= 1E-05)
		{
			this.Reset();
			if (this.CoodDownComplete != null)
			{
				this.CoodDownComplete.Invoke(this, EventArgs.Empty);
			}
			return;
		}
	}

	public void Init()
	{
		this.Container.Children.Add(this._BodyElem);
		this.Container.Children.Add(this._TextBlock);
		this._TextBlock.mouseEnabled = false;
		this.setMaskSize(this.Width, this.Height);
	}

	private void setMaskSize(double ww, double hh)
	{
		if (!this._BodyMask || double.IsNaN(ww) || !double.IsNaN(hh))
		{
		}
	}

	public new double Width
	{
		get
		{
			return base.Width;
		}
		set
		{
			base.Width = value;
			this.setMaskSize(this.Width, this.Height);
		}
	}

	public new double Height
	{
		get
		{
			return base.Height;
		}
		set
		{
			base.Height = value;
			this.setMaskSize(this.Width, this.Height);
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.Init();
	}

	public SolidColorBrush TextColor
	{
		get
		{
			return this._TextBlock.TextColor;
		}
		set
		{
			this._TextBlock.TextColor = value;
		}
	}

	public int TextFontSize
	{
		get
		{
			return this._TextBlock.FontSize;
		}
		set
		{
			this._TextBlock.FontSize = value;
		}
	}

	private void DrawSector(XGraphics graphics, double x = 0.0, double y = 0.0, double r = 100.0, double angle = 360.0, double startFrom = 270.0)
	{
	}

	public void MyStart(long ticks, bool showText = true, int timerTicks = 100, long startTicks = 0L, bool isFromLightToDark = true, bool isDrawTicks = true)
	{
		this.Reset();
		this._IsFromLightToDark = isFromLightToDark;
		this._isDrawTicks = isDrawTicks;
		this.MaskAlpha = 0.7;
		this._timerTicks = timerTicks;
		this._totalTicks = ticks;
		this._leftTicks = this._totalTicks - (Global.GetCorrectLocalTime() - startTicks);
		this._currentInterval = Math.Max(200L, this._totalTicks / 1000L);
		this._startTicks = startTicks;
		this._TextBlock.Visibility = showText;
		this._timer = new Timer((int)this._currentInterval);
		this._timer.addEventListener(TimerEvent.TIMER, new Timer.TimerEventHnadler(this.onTimer));
		if (this._currentInterval > 1000L)
		{
		}
		this._timer.start();
	}

	public void Reset()
	{
	}

	public bool Working()
	{
		return this._timer.running;
	}

	public double Percent
	{
		get
		{
			return this._Percent;
		}
		set
		{
			if (this._Percent != value)
			{
				this._Percent = value;
				if (this._Percent >= 1.0)
				{
					this._Percent = 1.0;
				}
			}
		}
	}

	private ImageBrush _BackImage;

	private ImageBrush _BodyImage = new ImageBrush(null);

	private Sprite _BodyElem = new Sprite();

	private Sprite _BodyMask = new Sprite();

	private GTextBlockOutLine _TextBlock = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private double _Percent;

	private Timer _timer;

	private SolidColorBrush _MaskColor;

	private bool _IsFromLightToDark = true;

	public EventHandler CoodDownComplete;

	private double _MaskAlpha = 1.0;

	private bool _isDrawTicks = true;

	private int _timerTicks;

	private long _startTicks;

	private long _totalTicks;

	private long _leftTicks;

	private long _currentInterval = 200L;
}
