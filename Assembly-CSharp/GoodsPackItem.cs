using System;
using HSGameEngine.GameEngine.SilverLight;

public class GoodsPackItem : UserControl
{
	public GGoodIcon GoodsImgs
	{
		set
		{
			this.GoodsImg.Children.Add(value);
		}
	}

	public BitmapData GoodsImgBacks
	{
		set
		{
			this.GoodsImgBack.Source = new ImageBrush(value);
		}
	}

	protected override void InitializeComponent()
	{
		Canvas.SetZIndex(this.GoodsImg, 1.0);
		Canvas.SetLeft(this.GoodsImg, 3);
		Canvas.SetTop(this.GoodsImg, 3);
		this.GoodsImg.Width = 32.0;
		this.GoodsImg.Height = 32.0;
		this.Container.Children.Add(this.GoodsImg);
		Canvas.SetLeft(this.GoodsImgBack, 0);
		Canvas.SetTop(this.GoodsImgBack, 0);
		this.GoodsImgBack.Width = 41.0;
		this.GoodsImgBack.Height = 42.0;
		this.Container.Children.Add(this.GoodsImgBack);
	}

	private Canvas GoodsImg = new Canvas();

	private Image GoodsImgBack = new Image();
}
