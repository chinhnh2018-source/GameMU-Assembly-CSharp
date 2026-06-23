using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class FamilyGXListItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.mouseChildren = false;
		this.Container.Children.Add(this.txtRoleName);
		this.txtRoleName.TextColor = new SolidColorBrush(4278237444U);
		Canvas.SetLeft(this.txtRoleName, 2);
		Canvas.SetTop(this.txtRoleName, 2);
		this.Container.Children.Add(this.txtLevel);
		this.txtLevel.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtLevel, 133);
		Canvas.SetTop(this.txtLevel, 3);
		this.Container.Children.Add(this.txtOcc);
		this.txtOcc.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtOcc, 167);
		Canvas.SetTop(this.txtOcc, 2);
		this.Container.Children.Add(this.txtZw);
		this.txtZw.TextColor = new SolidColorBrush(4294955520U);
		Canvas.SetLeft(this.txtZw, 198);
		Canvas.SetTop(this.txtZw, 2);
		this.txtNickName.TextColor = new SolidColorBrush(4279153361U);
		Canvas.SetLeft(this.txtNickName, 203);
		Canvas.SetTop(this.txtNickName, 2);
		this.Container.Children.Add(this.txtBzlNum);
		this.txtBzlNum.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtBzlNum, 247);
		Canvas.SetTop(this.txtBzlNum, 3);
		this.Container.Children.Add(this.txtYxlNum);
		this.txtYxlNum.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtYxlNum, 301);
		Canvas.SetTop(this.txtYxlNum, 3);
		this.Container.Children.Add(this.txtZslNum);
		this.txtZslNum.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtZslNum, 346);
		Canvas.SetTop(this.txtZslNum, 3);
		this.Container.Children.Add(this.txtTzlNum);
		this.txtTzlNum.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtTzlNum, 395);
		Canvas.SetTop(this.txtTzlNum, 3);
		this.Container.Children.Add(this.txtStlNum);
		this.txtStlNum.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtStlNum, 439);
		Canvas.SetTop(this.txtStlNum, 3);
		this.Container.Children.Add(this.txtTqNum);
		this.txtTqNum.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtTqNum, 486);
		Canvas.SetTop(this.txtTqNum, 3);
		this.Container.Children.Add(this.txtBgNum);
		this.txtBgNum.TextColor = new SolidColorBrush(4289798954U);
		Canvas.SetLeft(this.txtBgNum, 560);
		Canvas.SetTop(this.txtBgNum, 3);
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

	public string RoleName
	{
		get
		{
			return this.txtRoleName.Text;
		}
		set
		{
			this.txtRoleName.Text = value;
		}
	}

	public string Occ
	{
		get
		{
			return this.txtOcc.Text;
		}
		set
		{
			this.txtOcc.Text = value;
		}
	}

	public string Zw
	{
		get
		{
			return this.txtZw.Text;
		}
		set
		{
			this.txtZw.Text = value;
		}
	}

	public string NickName
	{
		get
		{
			return this.txtNickName.Text;
		}
		set
		{
			this.txtNickName.Text = value;
		}
	}

	public string BzlNum
	{
		get
		{
			return this.txtBzlNum.Text;
		}
		set
		{
			this.txtBzlNum.Text = value;
		}
	}

	public string YxlNum
	{
		get
		{
			return this.txtYxlNum.Text;
		}
		set
		{
			this.txtYxlNum.Text = value;
		}
	}

	public string ZslNum
	{
		get
		{
			return this.txtZslNum.Text;
		}
		set
		{
			this.txtZslNum.Text = value;
		}
	}

	public string TzlNum
	{
		get
		{
			return this.txtTzlNum.Text;
		}
		set
		{
			this.txtTzlNum.Text = value;
		}
	}

	public string StlNum
	{
		get
		{
			return this.txtStlNum.Text;
		}
		set
		{
			this.txtStlNum.Text = value;
		}
	}

	public string TqNum
	{
		get
		{
			return this.txtTqNum.Text;
		}
		set
		{
			this.txtTqNum.Text = value;
		}
	}

	public string Bg
	{
		get
		{
			return this.txtBgNum.Text;
		}
		set
		{
			this.txtBgNum.Text = value;
		}
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

	private GTextBlockOutLine txtRoleName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtLevel = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtOcc = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtZw = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtNickName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtBzlNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtYxlNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtZslNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtTzlNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtStlNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtTqNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtBgNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
}
