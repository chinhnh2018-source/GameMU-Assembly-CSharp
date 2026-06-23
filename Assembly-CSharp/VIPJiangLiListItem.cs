using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class VIPJiangLiListItem : UserControl
{
	public VIPJiangLiListItem()
	{
		this.Container.Children.Add(this.tb);
		Canvas.SetLeft(this.tb, 0);
		Canvas.SetTop(this.tb, 0);
		this.tb.TextColor = new SolidColorBrush(11394222U);
		this.tb.TextWrapping = TextWrapping.Wrap;
		this.Container.Children.Add(this.btnCanvas);
		this.btnCanvas.Width = 39.0;
		this.btnCanvas.Height = 17.0;
		Canvas.SetTop(this.btnCanvas, 0);
		this.Container.BackgroundColor = 4278190080U;
		this.Container.BackgroundAlpha = 0.01;
	}

	public GIcon BtnLingQu
	{
		set
		{
			this.btnCanvas.Children.Add(value);
		}
	}

	public string JiangLiText
	{
		set
		{
			Super.FormatTextBlockEx2(this.tb, value);
			this.SetPos();
		}
	}

	private void SetPos()
	{
		Canvas.SetLeft(this.btnCanvas, this.tb.ActualWidth + 5.0);
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

	public double JiangLiTextBodyWidth
	{
		set
		{
			this.tb.BodyWidth = value;
		}
	}

	private GTextBlockEx tb = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);

	private Canvas btnCanvas = new Canvas();
}
