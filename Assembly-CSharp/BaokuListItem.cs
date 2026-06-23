using System;
using HSGameEngine.GameEngine.SilverLight;

public class BaokuListItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.GoodsImg);
		this.GoodsImg.Width = 32.0;
		this.GoodsImg.Height = 32.0;
		Canvas.SetLeft(this.GoodsImg, 18);
		Canvas.SetTop(this.GoodsImg, 35);
		this.Container.Children.Add(this.ImgBtn);
		this.ImgBtn.Width = 32.0;
		this.ImgBtn.Height = 32.0;
		Canvas.SetLeft(this.ImgBtn, 0);
		Canvas.SetTop(this.ImgBtn, 0);
		this.ImgBtn.Width = 68.0;
		this.ImgBtn.Height = 111.0;
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

	public GGoodIcon GoodImg
	{
		set
		{
			this.GoodsImg.Children.Add(value);
		}
	}

	public GIcon BaokuBtn
	{
		set
		{
			this.ImgBtn.Children.Add(value);
		}
	}

	public bool Visible
	{
		get
		{
			return this.ImgBtn.Visibility;
		}
		set
		{
			this.ImgBtn.Visibility = value;
		}
	}

	private Canvas Root;

	private Canvas GoodsImg = new Canvas();

	private Canvas ImgBtn = new Canvas();
}
