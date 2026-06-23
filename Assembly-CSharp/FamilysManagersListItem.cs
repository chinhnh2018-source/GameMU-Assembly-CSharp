using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class FamilysManagersListItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.mouseChildren = false;
		this.Container.Children.Add(this.txtName);
		this.txtName.TextColor = new SolidColorBrush(4278238475U);
		Canvas.SetLeft(this.txtName, 46);
		Canvas.SetTop(this.txtName, 2);
		this.txtName.center = true;
		this.Container.Children.Add(this.txtLevel);
		this.txtLevel.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtLevel, 115);
		Canvas.SetTop(this.txtLevel, 2);
		this.txtLevel.center = true;
		this.Container.Children.Add(this.txtOcc);
		this.txtOcc.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtOcc, 150);
		Canvas.SetTop(this.txtOcc, 2);
		this.txtOcc.center = true;
		this.Container.Children.Add(this.txtZw);
		this.txtZw.TextColor = new SolidColorBrush(4294956032U);
		Canvas.SetLeft(this.txtZw, 191);
		Canvas.SetTop(this.txtZw, 2);
		this.txtZw.center = true;
		this.Container.Children.Add(this.txtNickName);
		this.txtNickName.TextColor = new SolidColorBrush(4279153361U);
		Canvas.SetLeft(this.txtNickName, 273);
		Canvas.SetTop(this.txtNickName, 2);
		this.txtNickName.center = true;
		this.txtBg.TextColor = new SolidColorBrush(4289798954U);
		Canvas.SetLeft(this.txtBg, 329);
		Canvas.SetTop(this.txtBg, 3);
		this.txtBg.center = true;
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

	public int RoleID
	{
		get
		{
			return this._RoleID;
		}
		set
		{
			this._RoleID = value;
		}
	}

	public string RoleName
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

	public int BHZhiWu
	{
		get
		{
			return this._BHZhiWu;
		}
		set
		{
			this._BHZhiWu = value;
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

	public string Bg
	{
		get
		{
			return this.txtBg.Text;
		}
		set
		{
			this.txtBg.Text = value;
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

	private GTextBlockOutLine txtName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtLevel = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtOcc = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtZw = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtNickName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtBg = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private int _RoleID;

	private int _BHZhiWu;
}
