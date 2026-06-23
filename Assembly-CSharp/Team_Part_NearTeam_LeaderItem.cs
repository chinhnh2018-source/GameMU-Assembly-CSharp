using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;

public class Team_Part_NearTeam_LeaderItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.gtbPlayerName);
		Canvas.SetLeft(this.gtbPlayerName, 0);
		Canvas.SetTop(this.gtbPlayerName, 4);
		this.gtbPlayerName.TextColor = new SolidColorBrush(13277735U);
		this.Container.Children.Add(this.gtbLevel);
		Canvas.SetLeft(this.gtbLevel, 95);
		Canvas.SetTop(this.gtbLevel, 4);
		this.gtbLevel.TextColor = new SolidColorBrush(46850U);
		this.Container.Children.Add(this.gtbNum);
		Canvas.SetLeft(this.gtbNum, 50);
		Canvas.SetTop(this.gtbNum, 20);
		this.gtbNum.TextColor = new SolidColorBrush(16777215U);
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

	public int TeamID
	{
		get
		{
			return this._TeamID;
		}
		set
		{
			this._TeamID = value;
		}
	}

	public TeamData CurrentTeamData
	{
		get
		{
			return this._CurrentTeamData;
		}
		set
		{
			this._CurrentTeamData = value;
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

	private Canvas Root;

	public GTextBlockOutLine gtbPlayerName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine gtbLevel = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine gtbNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private int _TeamID;

	private TeamData _CurrentTeamData;
}
