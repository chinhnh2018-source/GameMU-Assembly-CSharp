using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class MountsListItem : UserControl
{
	public MountsListItem()
	{
		base.addEventListener("mouseOver", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("mouseOut", new MouseEventHandler(this.UserControl_MouseLeave));
		base.Background = new SolidColorBrush(5207664U);
		base.BackgroundAlpha = 0.01;
	}

	protected override void InitializeComponent()
	{
		Canvas.SetLeft(this.Container, 0);
		Canvas.SetTop(this.Container, 0);
		this.Container.Children.Add(this.MountName);
		Canvas.SetLeft(this.MountName, 4);
		Canvas.SetTop(this.MountName, 2);
		this.MountName.mouseEnabled = false;
	}

	public int DbID
	{
		get
		{
			return this._DbID;
		}
		set
		{
			this._DbID = value;
		}
	}

	public int HorseBodyID
	{
		get
		{
			return this._HorseBodyID;
		}
		set
		{
			this._HorseBodyID = value;
		}
	}

	public int HorseID
	{
		get
		{
			return this._HorseID;
		}
		set
		{
			this._HorseID = value;
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
			return this.Width;
		}
		set
		{
			this.Width = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this.Height;
		}
		set
		{
			this.Height = value;
		}
	}

	public Brush TextColor
	{
		set
		{
			this.MountName.TextColor = (value as SolidColorBrush);
		}
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
			base.BackgroundAlpha = 0.2;
		}
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		base.Cursor = Cursors.Auto;
		base.BackgroundAlpha = 0.01;
	}

	public GTextBlockOutLine MountName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private int _DbID;

	private int _HorseBodyID;

	private int _HorseID;
}
