using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class RepetitionTopListItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.txtRepetitionName);
		this.txtRepetitionName.TextColor = new SolidColorBrush(4279153361U);
		Canvas.SetLeft(this.txtRepetitionName, 48);
		Canvas.SetTop(this.txtRepetitionName, 2);
		this.txtRepetitionName.center = true;
		this.Container.Children.Add(this.txtName);
		this.txtName.TextColor = new SolidColorBrush(4291467815U);
		Canvas.SetLeft(this.txtName, 165);
		Canvas.SetTop(this.txtName, 2);
		this.txtName.center = true;
		this.Container.Children.Add(this.txtTime);
		this.txtTime.TextColor = new SolidColorBrush(4278236930U);
		Canvas.SetLeft(this.txtTime, 238);
		Canvas.SetTop(this.txtTime, 2);
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

	public string RepetitionName
	{
		get
		{
			return this.txtRepetitionName.Text;
		}
		set
		{
			this.txtRepetitionName.Text = value;
		}
	}

	public string CompleteTime
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

	public int RepetitionID
	{
		get
		{
			return this._RepetitionID;
		}
		set
		{
			this._RepetitionID = value;
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

	private void UserControl_MouseEnter(object sender, MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
		}
	}

	private void UserControl_MouseLeave(object sender, MouseEvent e)
	{
		base.Cursor = Cursors.Auto;
	}

	private GTextBlockOutLine txtRepetitionName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtTime = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private int _RepetitionID;

	private int _RoleID;
}
