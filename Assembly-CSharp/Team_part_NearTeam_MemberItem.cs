using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class Team_part_NearTeam_MemberItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.imgAvatar);
		this.imgAvatar.Width = 48.0;
		this.imgAvatar.Height = 48.0;
		Canvas.SetLeft(this.imgAvatar, 2);
		Canvas.SetTop(this.imgAvatar, 3);
		this.Container.Children.Add(this.gtbPlayerName);
		Canvas.SetLeft(this.gtbPlayerName, 55);
		Canvas.SetTop(this.gtbPlayerName, 4);
		this.gtbPlayerName.TextColor = new SolidColorBrush(13277735U);
		this.Container.Children.Add(this.gtbLevel);
		Canvas.SetLeft(this.gtbLevel, 114);
		Canvas.SetTop(this.gtbLevel, 34);
		this.gtbLevel.TextColor = new SolidColorBrush(46850U);
		this.Container.Children.Add(this.gtbWork);
		Canvas.SetLeft(this.gtbWork, 57);
		Canvas.SetTop(this.gtbWork, 34);
		this.gtbWork.TextColor = new SolidColorBrush(10626862U);
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

	private void UserControl_MouseEnter(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
		}
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		base.Cursor = Cursors.Auto;
	}

	private Canvas Root;

	public Image imgAvatar = new Image();

	public GTextBlockOutLine gtbPlayerName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine gtbLevel = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine gtbWork = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
}
