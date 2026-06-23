using System;
using HSGameEngine.GameEngine.SilverLight;

public class CompoundItem : UserControl
{
	public string GoodsName
	{
		get
		{
			return this.NameText.Text;
		}
		set
		{
			this.NameText.Text = value;
		}
	}

	public GGoodIcon GoodsImgs
	{
		get
		{
			return this.GoodsIcon;
		}
		set
		{
			this.GoodsIcon = value;
			this.GoodsImg.Children.Clear();
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

	public int NeedGoodsNum
	{
		get
		{
			return this._NeedGoodsNum;
		}
		set
		{
			this._NeedGoodsNum = value;
		}
	}

	public void SetGoodsImgXY(int Lift, int Top)
	{
		Canvas.SetLeft(this.GoodsImg, Lift);
		Canvas.SetTop(this.GoodsImg, Top);
	}

	public void SetGoodsImgBackXY(int Lift, int Top)
	{
		Canvas.SetLeft(this.GoodsImgBack, Lift);
		Canvas.SetTop(this.GoodsImgBack, Top);
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.StackPanel1);
		this.StackPanel1.HorizontalAlignment = global::Layout.Center;
		this.StackPanel1.Width = 80.0;
		this.StackPanel1.Height = 60.0;
		this.StackPanel1.Background = new SolidColorBrush(16777215U);
		this.StackPanel1.Children.Add(this.NameText);
		this.NameText.FontSize = HSTextField.defaultFontSize;
		this.NameText.TextColor = new SolidColorBrush(65535U);
		this.NameText.HorizontalAlignment = global::Layout.Center;
		this.Container.Children.Add(this.GoodsImg);
		Canvas.SetLeft(this.GoodsImg, 20);
		Canvas.SetTop(this.GoodsImg, 18);
		this.GoodsImg.Width = 32.0;
		this.GoodsImg.Height = 32.0;
		this.Container.Children.Add(this.GoodsImgBack);
		Canvas.SetLeft(this.GoodsImgBack, 17);
		Canvas.SetTop(this.GoodsImgBack, 15);
		this.GoodsImgBack.Width = 41.0;
		this.GoodsImgBack.Height = 42.0;
	}

	private StackPanel StackPanel1 = new StackPanel();

	private GTextBlockOutLine NameText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public Canvas GoodsImg = new Canvas();

	private Image GoodsImgBack = new Image();

	private GGoodIcon GoodsIcon;

	private int _NeedGoodsNum;
}
