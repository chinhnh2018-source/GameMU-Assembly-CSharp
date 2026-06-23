using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class HFListItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.txtName);
		this.txtName.TextColor = new SolidColorBrush(4278237444U);
		Canvas.SetLeft(this.txtName, 13);
		Canvas.SetTop(this.txtName, 2);
		this.Container.Children.Add(this.txtLevel);
		this.txtLevel.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtLevel, 133);
		Canvas.SetTop(this.txtLevel, 3);
		this.Container.Children.Add(this.txtOcc);
		this.txtOcc.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtOcc, 176);
		Canvas.SetTop(this.txtOcc, 2);
		this.Container.Children.Add(this.txtSex);
		this.txtSex.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtSex, 231);
		Canvas.SetTop(this.txtSex, 2);
		base.addEventListener("mouseOver", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("mouseOut", new MouseEventHandler(this.UserControl_MouseLeave));
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

	public string RoleLevel
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

	public string RoleOcc
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

	public string RoleSex
	{
		get
		{
			return this.txtSex.Text;
		}
		set
		{
			this.txtSex.Text = value;
		}
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
		}
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Auto;
		}
	}

	private int _RoleID = -1;

	private Canvas Root;

	private GTextBlockOutLine txtName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtLevel = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtOcc = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtSex = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
}
