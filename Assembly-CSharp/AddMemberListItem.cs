using System;
using HSGameEngine.GameEngine.SilverLight;

public class AddMemberListItem : UserControl
{
	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
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

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.txtRoleName);
		this.txtRoleName.TextColor = new SolidColorBrush(4278237444U);
		Canvas.SetLeft(this.txtRoleName, 28);
		Canvas.SetTop(this.txtRoleName, 2);
		this.Container.Children.Add(this.txtLevel);
		this.txtLevel.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtLevel, 165);
		Canvas.SetTop(this.txtLevel, 3);
		this.Container.Children.Add(this.txtOcc);
		this.txtOcc.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtOcc, 218);
		Canvas.SetTop(this.txtOcc, 2);
	}

	private GTextBlockOutLine txtRoleName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtLevel = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtOcc = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private int _RoleID;
}
