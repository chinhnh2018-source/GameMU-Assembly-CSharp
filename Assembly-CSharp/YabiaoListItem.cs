using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class YabiaoListItem : UserControl
{
	public YabiaoListItem()
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

	public int YaBiaoID
	{
		get
		{
			return this._YaBiaoID;
		}
		set
		{
			this._YaBiaoID = value;
		}
	}

	public string YaBiaoName
	{
		get
		{
			return this.txtName.Text;
		}
		set
		{
			this.txtName.Text = value;
		}
	}

	public string Level
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

	public int MinLevel
	{
		get
		{
			return this._MinLevel;
		}
		set
		{
			this._MinLevel = value;
		}
	}

	public int MaxLevel
	{
		get
		{
			return this._MaxLevel;
		}
		set
		{
			this._MaxLevel = value;
		}
	}

	public int YaJin
	{
		get
		{
			return this._YaJin;
		}
		set
		{
			this._YaJin = value;
		}
	}

	public string RewardYL
	{
		get
		{
			return this.txtRewardYL.Text;
		}
		set
		{
			this.txtRewardYL.Text = value;
		}
	}

	public string RewardExp
	{
		get
		{
			return this.txtRewardExp.Text;
		}
		set
		{
			this.txtRewardExp.Text = value;
		}
	}

	public string Description
	{
		get
		{
			return this._des;
		}
		set
		{
			this._des = value;
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

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.txtName);
		this.txtName.TextColor = new SolidColorBrush(4291467815U);
		Canvas.SetLeft(this.txtName, 2);
		Canvas.SetTop(this.txtName, 4);
		this.Container.Children.Add(this.txtLevel);
		this.txtLevel.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtLevel, 60);
		Canvas.SetTop(this.txtLevel, 4);
		this.Container.Children.Add(this.txtRewardYL);
		this.txtRewardYL.TextColor = new SolidColorBrush(4279153361U);
		Canvas.SetLeft(this.txtRewardYL, 135);
		Canvas.SetTop(this.txtRewardYL, 5);
		this.Container.Children.Add(this.txtRewardExp);
		this.txtRewardExp.TextColor = new SolidColorBrush(4279153361U);
		Canvas.SetLeft(this.txtRewardExp, 187);
		Canvas.SetTop(this.txtRewardExp, 5);
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

	private string _des;

	private GTextBlockOutLine txtName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtLevel = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtRewardYL = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtRewardExp = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private int _YaBiaoID;

	private int _MinLevel;

	private int _MaxLevel;

	private int _YaJin;
}
