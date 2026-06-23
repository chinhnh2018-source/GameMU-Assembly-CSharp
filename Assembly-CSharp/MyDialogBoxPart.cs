using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class MyDialogBoxPart : UserControl
{
	public string OkText
	{
		set
		{
			this.OkBtn.Text = Global.GetLang(value);
		}
	}

	public string CancelText
	{
		set
		{
			this.CancelBtn.Text = Global.GetLang(value);
		}
	}

	public string HintTitle
	{
		set
		{
			this.HintTitle_Label.text = value;
		}
	}

	public int BoxType
	{
		set
		{
			if (value == 0)
			{
				this.OkBtn.gameObject.SetActive(true);
				this.OkBtn.gameObject.transform.localPosition = this.CancelBtn.transform.localPosition;
			}
			else if (value == 1)
			{
				this.OkBtn.gameObject.SetActive(true);
				this.CancelBtn.gameObject.SetActive(true);
			}
		}
	}

	public string HintText
	{
		set
		{
			if (null != this.HintText_Label)
			{
				this.HintText_Label.text = value;
			}
		}
	}

	public int MyDialogBoxPartReturn
	{
		get
		{
			return this._MyDialogBoxPartReturn;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.OkBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Timer != null)
			{
				this.Timer.Stop();
				this.Timer.Tick = null;
				this.Timer = null;
			}
			this._MyDialogBoxPartReturn = 0;
			this.ButtonClick.Invoke(this, EventArgs.Empty);
		};
		this.CancelBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Timer != null)
			{
				this.Timer.Stop();
				this.Timer.Tick = null;
				this.Timer = null;
			}
			this._MyDialogBoxPartReturn = 1;
			this.ButtonClick.Invoke(this, EventArgs.Empty);
		};
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private void Timer_Tick(object sender, object e)
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (this.ToCloseTicks <= 0.0 || (double)correctLocalTime >= this.ToCloseTicks)
		{
			this.Timer.Tick = null;
			this.Timer.Stop();
			this.Timer = null;
			if (this.ButtonClick != null)
			{
				this._MyDialogBoxPartReturn = 1;
				this.ButtonClick.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public void TimerClose(int milliseconds)
	{
		if (milliseconds <= 0)
		{
			return;
		}
		if (this.Timer != null)
		{
			this.Timer.Stop();
			this.Timer.Tick = null;
			this.Timer = null;
		}
		this.Timer = new DispatcherTimer("MyMyDialogBoxPartPart_Timer")
		{
			Interval = TimeSpan.FromMilliseconds(500.0)
		};
		this.ToCloseTicks = (double)(Global.GetCorrectLocalTime() + (long)milliseconds);
		this.Timer.Tick = new DispatcherTimerEventHandler(this.Timer_Tick);
		this.Timer.Start();
	}

	public TextBlock HintTitle_Label;

	public TextBlock HintText_Label;

	public GButton OkBtn;

	public GButton CancelBtn;

	public EventHandler ButtonClick;

	private int _MyDialogBoxPartReturn = -1;

	private DispatcherTimer Timer;

	private double ToCloseTicks;
}
