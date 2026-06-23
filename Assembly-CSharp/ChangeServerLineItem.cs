using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class ChangeServerLineItem : UserControl
{
	public ChangeServerLineItem()
	{
		this._textBlock = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this._textBlock.Name = "LineName";
		this._textBlock.TextSize = 12.0;
		Canvas.SetLeft(this._textBlock, 71);
		Canvas.SetTop(this._textBlock, 0);
		this.Container.Children.Add(this._textBlock);
		this._textBlockCurrent = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this._textBlockCurrent.Name = "CurrentLineName";
		this._textBlockCurrent.TextSize = 12.0;
		this._textBlockCurrent.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 44, 163, 190));
		Canvas.SetLeft(this._textBlockCurrent, 35);
		Canvas.SetTop(this._textBlockCurrent, 0);
		this.Container.Children.Add(this._textBlockCurrent);
	}

	protected override void InitializeComponent()
	{
		base.addEventListener("mouseDown", new MouseEventHandler(this.mouseEventLeftButtonDown));
		base.addEventListener("ROLL_OVER", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("ROLL_OUT", new MouseEventHandler(this.UserControl_MouseLeave));
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

	public GTextBlockOutLine LineTextBlock
	{
		get
		{
			return this._textBlock;
		}
	}

	public GTextBlockOutLine CurrentLineTextBlock
	{
		get
		{
			return this._textBlockCurrent;
		}
	}

	public string Tip
	{
		get
		{
			return string.Empty;
		}
		set
		{
		}
	}

	private void mouseEventLeftButtonDown(MouseEvent evt)
	{
		if (this.MouseLeftButtonDown != null)
		{
			this.MouseLeftButtonDown.Invoke(this, evt);
		}
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		base.BackgroundAlpha = 0.4;
		base.Cursor = Cursors.Hand;
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		base.BackgroundAlpha = 0.01;
		base.Cursor = Cursors.Auto;
	}

	private GTextBlockOutLine _textBlock;

	private GTextBlockOutLine _textBlockCurrent;

	public EventHandler MouseLeftButtonDown;
}
