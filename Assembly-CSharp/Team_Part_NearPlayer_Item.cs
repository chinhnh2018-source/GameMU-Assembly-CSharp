using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class Team_Part_NearPlayer_Item : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.mouseChildren = false;
		this.Container.Children.Add(this.gtbPlayerName);
		Canvas.SetLeft(this.gtbPlayerName, 55);
		Canvas.SetTop(this.gtbPlayerName, 3);
		this.gtbPlayerName.TextColor = new SolidColorBrush(13277735U);
		this.gtbPlayerName.center = true;
		this.Container.Children.Add(this.gtbLevel);
		Canvas.SetLeft(this.gtbLevel, 140);
		Canvas.SetTop(this.gtbLevel, 3);
		this.gtbLevel.TextColor = new SolidColorBrush(46850U);
		this.gtbLevel.center = true;
		this.Container.Children.Add(this.gtbWork);
		Canvas.SetLeft(this.gtbWork, 186);
		Canvas.SetTop(this.gtbWork, 3);
		this.gtbWork.TextColor = new SolidColorBrush(10626862U);
		this.gtbWork.center = true;
		this.Container.Children.Add(this.gtbMenPai);
		Canvas.SetLeft(this.gtbMenPai, 244);
		Canvas.SetTop(this.gtbMenPai, 3);
		this.gtbMenPai.center = true;
		this.gtbMenPai.TextColor = new SolidColorBrush(16777215U);
		base.addEventListener("mouseOver", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("mouseOut", new MouseEventHandler(this.UserControl_MouseLeave));
		base.Background = new SolidColorBrush(1U);
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

	public GTextBlockOutLine gtbWork = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine gtbMenPai = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private int _RoleID;
}
