using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class JgssListItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.txtName);
		this.txtName.TextColor = new SolidColorBrush(11394224U);
		Canvas.SetLeft(this.txtName, 10);
		Canvas.SetTop(this.txtName, 4);
		this.txtName.FontSize = HSTextField.defaultFontSize;
		base.addEventListener("mouseOver", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("mouseOut", new MouseEventHandler(this.UserControl_MouseLeave));
	}

	public Brush BodyBackground
	{
		set
		{
			this.Root.Background = value;
		}
	}

	public double BodyWidth
	{
		get
		{
			return this.Root.Width;
		}
		set
		{
			this.Root.Width = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this.Root.Height;
		}
		set
		{
			this.Root.Height = value;
		}
	}

	public string MapName
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

	public Brush MapNameColor
	{
		get
		{
			return this.txtName.TextColor;
		}
		set
		{
			this.txtName.TextColor = (value as SolidColorBrush);
		}
	}

	public int MapID
	{
		get
		{
			return this._MapID;
		}
		set
		{
			this._MapID = value;
		}
	}

	public int LingDiID
	{
		get
		{
			return this._LingDiID;
		}
		set
		{
			this._LingDiID = value;
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

	private int _MapID = -1;

	private int _LingDiID;

	private Canvas Root;

	private GTextBlockOutLine txtName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
}
