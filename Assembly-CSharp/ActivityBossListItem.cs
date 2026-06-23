using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class ActivityBossListItem : UserControl
{
	public ActivityBossListItem()
	{
		base.addEventListener("mouseOver", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("mouseOut", new MouseEventHandler(this.UserControl_MouseLeave));
		base.Background = new SolidColorBrush(5207664U);
		base.BackgroundAlpha = 0.01;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public double BodyWidth
	{
		get
		{
			return this.Container.Width;
		}
		set
		{
			this.Container.Width = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this.Container.Height;
		}
		set
		{
			this.Container.Height = value;
		}
	}

	public int BossID
	{
		get
		{
			return this._BossID;
		}
		set
		{
			this._BossID = value;
		}
	}

	public GTextBlockEx BossName
	{
		set
		{
			Canvas.SetLeft(value, 1);
			Canvas.SetTop(value, 1);
			this.Container.Children.Add(value);
		}
	}

	public string BossLevel
	{
		get
		{
			return this.txtLevel.Text;
		}
		set
		{
			this.txtLevel.Text = value;
		}
	}

	public string BossMap
	{
		get
		{
			return this.txtMap.Text;
		}
		set
		{
			this.txtMap.Text = value;
		}
	}

	public string BossState
	{
		get
		{
			return this.txtState.Text;
		}
		set
		{
			this.txtState.Text = value;
		}
	}

	public string BossLastKiller
	{
		get
		{
			return this.txtLastKiller.Text;
		}
		set
		{
			this.txtLastKiller.Text = value;
		}
	}

	public SolidColorBrush TxtBossStateColor
	{
		get
		{
			return this.txtState.TextColor;
		}
		set
		{
			this.txtState.TextColor = value;
		}
	}

	public SolidColorBrush TxtKillerColor
	{
		get
		{
			return this.txtLastKiller.TextColor;
		}
		set
		{
			this.txtLastKiller.TextColor = value;
		}
	}

	public SolidColorBrush TxtLevelColor
	{
		get
		{
			return this.txtLevel.TextColor;
		}
		set
		{
			this.txtLevel.TextColor = value;
		}
	}

	public string BossDescription
	{
		get
		{
			return this._BossDescription;
		}
		set
		{
			this._BossDescription = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.txtLevel);
		this.txtLevel.TextColor = new SolidColorBrush(4281859895U);
		this.txtLevel.mouseEnabled = false;
		Canvas.SetLeft(this.txtLevel, 95);
		Canvas.SetTop(this.txtLevel, 1);
		this.Container.Children.Add(this.txtMap);
		this.txtMap.TextColor = new SolidColorBrush(4294967160U);
		this.txtMap.mouseEnabled = false;
		Canvas.SetLeft(this.txtMap, 123);
		Canvas.SetTop(this.txtMap, 1);
		this.Container.Children.Add(this.txtState);
		this.txtState.TextColor = new SolidColorBrush(4281859895U);
		this.txtState.mouseEnabled = false;
		Canvas.SetLeft(this.txtState, 210);
		Canvas.SetTop(this.txtState, 1);
		this.Container.Children.Add(this.txtLastKiller);
		this.txtLastKiller.TextColor = new SolidColorBrush(4294952960U);
		this.txtLastKiller.mouseEnabled = false;
		Canvas.SetLeft(this.txtLastKiller, 268);
		Canvas.SetTop(this.txtLastKiller, 1);
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		base.BackgroundAlpha = 0.2;
		base.buttonMode = true;
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		base.BackgroundAlpha = 0.01;
		base.buttonMode = false;
	}

	private string _BossDescription;

	private int _BossID;

	private GTextBlockOutLine txtLevel = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtMap = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtState = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtLastKiller = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
}
