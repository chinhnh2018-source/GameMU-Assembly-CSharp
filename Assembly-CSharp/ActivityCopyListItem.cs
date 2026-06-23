using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class ActivityCopyListItem : UserControl
{
	public ActivityCopyListItem()
	{
		base.addEventListener("mouseOver", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("mouseOut", new MouseEventHandler(this.UserControl_MouseLeave));
		base.Background = new SolidColorBrush(5207664U);
		base.BackgroundAlpha = 0.01;
	}

	public GTextBlockEx ObjectPublishName
	{
		set
		{
			Canvas.SetLeft(value, 350);
			Canvas.SetTop(value, 4);
			this.Container.Children.Add(value);
		}
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

	public string ActivityTime
	{
		get
		{
			return this.txtTime.Text;
		}
		set
		{
			this.txtTime.Text = value;
		}
	}

	public string ActivityName
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

	public string ActivityJoinLevel
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

	public string ActivityJoinNum
	{
		get
		{
			return this.txtNum.Text;
		}
		set
		{
			this.txtNum.Text = value;
		}
	}

	public string ActivityReward
	{
		get
		{
			return this.txtReward.Text;
		}
		set
		{
			this.txtReward.Text = value;
			if (this.txtReward.Text != string.Empty)
			{
				Canvas.SetLeft(this.txtReward, Canvas.GetLeft(this.txtName) + this.txtName.RealSize.Width);
			}
		}
	}

	public string Des01
	{
		get
		{
			return this._des01;
		}
		set
		{
			this._des01 = value;
		}
	}

	public string Des02
	{
		get
		{
			return this._des02;
		}
		set
		{
			this._des02 = value;
		}
	}

	public int ToLevel
	{
		get
		{
			return this._ToLevel;
		}
		set
		{
			this._ToLevel = value;
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

	public SolidColorBrush TxtJoinNumColor
	{
		get
		{
			return this.txtNum.TextColor;
		}
		set
		{
			this.txtNum.TextColor = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.txtTime);
		this.txtTime.mouseEnabled = false;
		this.txtTime.TextColor = new SolidColorBrush(4294967160U);
		this.txtTime.Width = 86.0;
		Canvas.SetLeft(this.txtTime, 7);
		Canvas.SetTop(this.txtTime, 1);
		this.Container.Children.Add(this.txtName);
		this.txtName.TextColor = new SolidColorBrush(4278255615U);
		this.txtName.mouseEnabled = false;
		Canvas.SetLeft(this.txtName, 109);
		Canvas.SetTop(this.txtName, 1);
		this.Container.Children.Add(this.txtReward);
		this.txtReward.TextColor = new SolidColorBrush(4294967095U);
		Canvas.SetTop(this.txtReward, 1);
		this.Container.Children.Add(this.txtLevel);
		this.txtLevel.TextColor = new SolidColorBrush(4281859895U);
		this.txtLevel.mouseEnabled = false;
		Canvas.SetLeft(this.txtLevel, 280);
		Canvas.SetTop(this.txtLevel, 1);
		this.Container.Children.Add(this.txtNum);
		this.txtNum.TextColor = new SolidColorBrush(4281859895U);
		this.txtNum.mouseEnabled = false;
		Canvas.SetLeft(this.txtNum, 465);
		Canvas.SetTop(this.txtNum, 1);
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		base.buttonMode = true;
		base.BackgroundAlpha = 0.2;
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		base.buttonMode = false;
		base.BackgroundAlpha = 0.01;
	}

	private string _des01;

	private string _des02;

	private GTextBlockOutLine txtTime = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtReward = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtLevel = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private int _ToLevel;

	private int _MaxLevel;
}
