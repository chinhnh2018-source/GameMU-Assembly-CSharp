using System;

public class GFyingImage : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.GoodsImage);
		this.GoodsImage.Stretch = StretchSL.Uniform;
		this.GoodsImage.Width = 32.0;
		this.GoodsImage.Height = 32.0;
	}

	public string GoodsImageSource
	{
		set
		{
			if (value == null)
			{
				this.GoodsImage.Source = null;
				return;
			}
			this.GoodsImage.URL = value;
		}
	}

	private URLImage GoodsImage = new URLImage();
}
