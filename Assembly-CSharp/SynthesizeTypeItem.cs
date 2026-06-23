using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class SynthesizeTypeItem : UserControl
{
	public SynthesizeTypeItem()
	{
		this.NameText.Text = Global.GetLang("名称");
		base.addEventListener("mouseOver", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("mouseOut", new MouseEventHandler(this.UserControl_MouseLeave));
		base.Background = new SolidColorBrush(5207664U);
		base.BackgroundAlpha = 0.01;
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
			this.Container.Width = value;
		}
	}

	public SolidColorBrush TextColor
	{
		set
		{
			this.NameText.TextFontColor = value;
		}
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public string TypeID
	{
		get
		{
			return this.TypeIDText.Text;
		}
		set
		{
			this.TypeIDText.Text = value;
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
			this.BakPanel.Height = value;
		}
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
			base.BackgroundAlpha = 0.4;
		}
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Auto;
			base.BackgroundAlpha = 0.01;
		}
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.NameText);
		this.NameText.Text = Global.GetLang("名称");
		this.NameText.Height = 20.0;
		Canvas.SetLeft(this.NameText, 5);
		Canvas.SetTop(this.NameText, 1);
		this.Container.Children.Add(this.TypeIDText);
		this.TypeIDText.Text = string.Empty;
		this.TypeIDText.Visibility = false;
		this.NameText.mouseEnabled = false;
	}

	private Canvas BakPanel = new Canvas();

	public GTextBlockOutLine NameText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private TextBlock TypeIDText = new TextBlock();
}
