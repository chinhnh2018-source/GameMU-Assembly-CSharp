using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class GongnengYugaoItem : UserControl
{
	public GongnengYugaoItem()
	{
		base.buttonMode = true;
		this.Container.Width = 180.0;
		this.Container.Height = 60.0;
		this.Container.Children.Add(this.txtLevel);
		this.txtLevel.TextColor = new SolidColorBrush(16777080U);
		Canvas.SetLeft(this.txtLevel, 91);
		Canvas.SetTop(this.txtLevel, 12);
		this.txtLevel.IsHitTestVisible = false;
		this.Container.Children.Add(this.txtName);
		this.txtName.TextColor = new SolidColorBrush(16777080U);
		Canvas.SetLeft(this.txtName, 80);
		Canvas.SetTop(this.txtName, 30);
		this.txtName.IsHitTestVisible = false;
		this.Container.Children.Add(this.Img);
		this.Img.Width = 39.0;
		this.Img.Height = 53.0;
		Canvas.SetLeft(this.Img, 11);
		Canvas.SetTop(this.Img, 1);
		this.Img.IsHitTestVisible = false;
		base.addEventListener("mouseUp", new MouseEventHandler(this.mouseEventLeftButtonUp));
		base.addEventListener("mouseDown", new MouseEventHandler(this.mouseEventLeftButtonDown));
	}

	public string Level
	{
		set
		{
			this.txtLevel.Text = value;
		}
	}

	public string NameEx
	{
		set
		{
			this.txtName.Text = value;
		}
	}

	public BitmapData Img_Source
	{
		set
		{
			this.Img.Source = new ImageBrush(value);
		}
	}

	private void mouseEventLeftButtonUp(MouseEvent evt)
	{
		this.MouseLeftButtonUp.Invoke(this, evt);
	}

	private void mouseEventLeftButtonDown(MouseEvent evt)
	{
	}

	public SizeSL Img_Size
	{
		set
		{
			this.Img.Width = value.Width;
			this.Img.Height = value.Height;
		}
	}

	public void AddEffect()
	{
		this.effect = Global.GetDecoration(516, GDecorationTypes.AutoRemove, new Point(0, 0), false, null, -1, -1, true, false);
		this.effect.Coordinate = new Point(105, 25);
	}

	private GTextBlockOutLine txtLevel = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Image Img = new Image();

	public GDecoration effect;

	public EventHandler MouseLeftButtonUp;
}
