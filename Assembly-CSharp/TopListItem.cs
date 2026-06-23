using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class TopListItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.txtName);
		this.txtName.TextColor = new SolidColorBrush(4289584302U);
		this.txtName.mouseEnabled = false;
		base.Background = new SolidColorBrush(865070U);
		base.BackgroundAlpha = 0.01;
	}

	public ImageBrush BodyBackground
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

	public string PaiHangName
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

	public string Title0
	{
		get
		{
			return this._title0;
		}
		set
		{
			this._title0 = value;
		}
	}

	public string Title
	{
		get
		{
			return this._title;
		}
		set
		{
			this._title = value;
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

	public int PaiHangID
	{
		get
		{
			return this._PaiHangID;
		}
		set
		{
			this._PaiHangID = value;
		}
	}

	public SolidColorBrush TxtNameColor
	{
		get
		{
			return this.txtName.TextColor;
		}
		set
		{
			this.txtName.TextColor = value;
		}
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
			base.BackgroundAlpha = 0.16;
		}
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		base.Cursor = Cursors.Auto;
		base.BackgroundAlpha = 0.01;
	}

	private GTextBlockOutLine txtName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private string _title0;

	private string _title;

	private int _PaiHangID;

	private string _des;
}
