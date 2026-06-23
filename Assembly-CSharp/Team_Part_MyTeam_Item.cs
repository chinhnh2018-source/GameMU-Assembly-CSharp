using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class Team_Part_MyTeam_Item : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.imgAvatar);
		this.imgAvatar.Width = 48.0;
		this.imgAvatar.Height = 48.0;
		Canvas.SetLeft(this.imgAvatar, 2);
		Canvas.SetTop(this.imgAvatar, 2);
		this.Container.Children.Add(this.gtbPlayerName);
		Canvas.SetLeft(this.gtbPlayerName, 60);
		Canvas.SetTop(this.gtbPlayerName, 8);
		this.gtbPlayerName.TextColor = new SolidColorBrush(13277735U);
		this.Container.Children.Add(this.gtbLevel);
		Canvas.SetLeft(this.gtbLevel, 160);
		Canvas.SetTop(this.gtbLevel, 9);
		this.gtbLevel.TextColor = new SolidColorBrush(7448500U);
		this.Container.Children.Add(this.gtbWork);
		Canvas.SetLeft(this.gtbWork, 60);
		Canvas.SetTop(this.gtbWork, 33);
		this.gtbWork.TextColor = new SolidColorBrush(10626862U);
		this.Container.Children.Add(this.gtbPoint);
		Canvas.SetLeft(this.gtbPoint, 133);
		Canvas.SetTop(this.gtbPoint, 33);
		this.gtbPoint.TextColor = new SolidColorBrush(10530227U);
		this.Container.Children.Add(this.gtbCity);
		Canvas.SetLeft(this.gtbCity, 191);
		Canvas.SetTop(this.gtbCity, 33);
		this.gtbCity.TextColor = new SolidColorBrush(46850U);
		this.Container.Children.Add(this.imgLeader);
		this.imgLeader.Width = 45.0;
		this.imgLeader.Height = 19.0;
		Canvas.SetLeft(this.imgLeader, 223);
		Canvas.SetTop(this.imgLeader, 3);
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

	public Image imgAvatar = new Image();

	public GTextBlockOutLine gtbPlayerName;

	public GTextBlockOutLine gtbLevel;

	public GTextBlockOutLine gtbWork;

	public GTextBlockOutLine gtbPoint;

	public GTextBlockOutLine gtbCity;

	public Image imgLeader = new Image();

	private int _RoleID;
}
