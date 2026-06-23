using System;
using HSGameEngine.GameEngine.SilverLight;

public class DailyTasksItem : UserControl
{
	public GTextBlockEx ObjectPublishName
	{
		set
		{
			if (null == value)
			{
				return;
			}
			Canvas.SetLeft(value, 312);
			Canvas.SetTop(value, 1);
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

	public int ItemTag
	{
		get
		{
			return this.itemTag;
		}
		set
		{
			this.itemTag = value;
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

	public double MaxEnterNum
	{
		get
		{
			return this.MaxNum;
		}
		set
		{
			this.MaxNum = value;
		}
	}

	public string Jindu
	{
		set
		{
			this.txtJindu.Text.Text = value;
		}
	}

	public string TimesText
	{
		set
		{
			this.txtTimesText.Text.Text = value;
		}
	}

	public string Interval
	{
		set
		{
			this.IntervalTime.Text.Text = value;
		}
	}

	public string ActivityName
	{
		set
		{
			this.txtName.Text.Text = value;
		}
	}

	public string ActivityJoinLevel
	{
		set
		{
			this.txtLevel.Text.Text = value;
		}
	}

	public SolidColorBrush TxtLevelColor
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public bool txtJinduCenter
	{
		set
		{
		}
	}

	public GIcon BtnChuansong
	{
		set
		{
			if (null == value)
			{
				return;
			}
			this.btnCanvas.Children.Add(value);
		}
	}

	protected override void InitializeComponent()
	{
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		base.Cursor = Cursors.Hand;
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		base.Cursor = Cursors.Auto;
	}

	public GTextBlock StarLevel;

	public GTextBlock txtJindu;

	public GTextBlock txtTimesText;

	public GTextBlock txtName;

	public GTextBlock txtLevel;

	public GTextBlock IntervalTime;

	private int itemTag;

	private Canvas btnCanvas;

	private double MaxNum;
}
