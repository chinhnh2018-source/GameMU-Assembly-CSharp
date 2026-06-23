using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.SilverLight;

public class GImgLevel : UserControl
{
	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.LifeBarImg_0);
		this.LifeBarImg_0.Width = 150.0;
		this.LifeBarImg_0.Height = 15.0;
		this.Container.Children.Add(this.LifeBarImg_1);
		this.LifeBarImg_1.Width = 150.0;
		this.LifeBarImg_1.Height = 15.0;
	}

	public int Orient
	{
		set
		{
			this._orient = value;
		}
	}

	public BitmapData Img0_Source
	{
		set
		{
			this.LifeBarImg_0.Source = new ImageBrush(value);
			this.source0 = value;
		}
	}

	public SizeSL Img0_Size
	{
		get
		{
			return new SizeSL(this.LifeBarImg_0.Width, this.LifeBarImg_0.Height);
		}
		set
		{
			this.LifeBarImg_0.Width = value.Width;
			this.LifeBarImg_0.Height = value.Height;
		}
	}

	public BitmapData Img1_Source
	{
		get
		{
			return this.source1;
		}
		set
		{
			this.LifeBarImg_1.Source = new ImageBrush(value);
			this.source1 = value;
		}
	}

	public SizeSL Img1_Size
	{
		get
		{
			return new SizeSL(this.LifeBarImg_1.Width, this.LifeBarImg_1.Height);
		}
		set
		{
			this.LifeBarImg_1.Width = value.Width;
			this.LifeBarImg_1.Height = value.Height;
		}
	}

	public double SingleStarWidth
	{
		get
		{
			return this._SingleStarWidth;
		}
		set
		{
			this._SingleStarWidth = value;
		}
	}

	public double SingleStarHeight
	{
		get
		{
			return this._SingleStarHeight;
		}
		set
		{
			this._SingleStarHeight = value;
		}
	}

	public int Level
	{
		set
		{
			this.DrawLevelImg(value);
		}
	}

	private void DrawLevelImg(int level)
	{
		Rectangle scrollRect = default(Rectangle);
		if (this.LifeBarImg_0 == null || this.LifeBarImg_1 == null)
		{
			return;
		}
		if (this._orient == 0)
		{
			int width = level * (int)this.SingleStarWidth;
			scrollRect = new Rectangle(0, 0, width, (int)this.LifeBarImg_0.Height);
		}
		else
		{
			int height = level * (int)this.SingleStarHeight;
			scrollRect = new Rectangle(0, 0, (int)this.LifeBarImg_0.Width, height);
		}
		this.LifeBarImg_1.scrollRect = scrollRect;
	}

	protected SizeSL MeasureOverride(SizeSL availableSize)
	{
		return new SizeSL(this.LifeBarImg_0.Width, this.LifeBarImg_0.Height);
	}

	protected SizeSL ArrangeOverride(SizeSL finalSize)
	{
		return finalSize;
	}

	private Image LifeBarImg_0 = new Image();

	private Image LifeBarImg_1 = new Image();

	private double _SingleStarWidth = 15.0;

	private double _SingleStarHeight = 15.0;

	private BitmapData source0;

	private BitmapData source1;

	private int _orient;
}
