using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class ChengZhanGzPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.txtChengZhanXianLu);
		this.txtChengZhanXianLu.TextColor = new SolidColorBrush(46850U);
		Canvas.SetLeft(this.txtChengZhanXianLu, 160);
		Canvas.SetTop(this.txtChengZhanXianLu, 141);
		string xapParamByName = Super.GetXapParamByName("country", string.Empty);
		if ("korea" != xapParamByName)
		{
			this.txtChengZhanXianLu.text = ConfigSystemParam.GetSystemParamByName("BangHuiFightingLineID", true);
		}
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	private GTextBlockOutLine txtChengZhanXianLu = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
}
