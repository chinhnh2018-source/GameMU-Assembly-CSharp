using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class Mark2ListItem : UserControl
{
	public Mark2ListItem()
	{
		base.addEventListener("mouseOver", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("mouseOut", new MouseEventHandler(this.UserControl_MouseLeave));
		this.drawSprite();
		base.Background = new SolidColorBrush(5207664U);
		base.BackgroundAlpha = 0.01;
	}

	public string Goodsdouble { get; set; }

	private void drawSprite()
	{
		this.sprite.graphics.beginFill(0U);
		this.sprite.graphics.drawRect(0, 0, 100, 20);
		this.sprite.graphics.endFill();
		SpriteSL spriteSL = this.sprite;
		double num = 0.0;
		this.sprite.Y = num;
		spriteSL.X = num;
		this.sprite.alpha = 0.01;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.sprite);
		this.sprite.visible = false;
		this.Container.Children.Add(this.txtStallNum);
		this.txtStallNum.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtStallNum, 20);
		Canvas.SetTop(this.txtStallNum, 2);
		this.Container.Children.Add(this.txtSellerName);
		this.txtSellerName.TextColor = new SolidColorBrush(4294952960U);
		Canvas.SetLeft(this.txtSellerName, 176);
		Canvas.SetTop(this.txtSellerName, 2);
		this.Container.Children.Add(this.txtSellerLevel);
		this.txtSellerLevel.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtSellerLevel, 439);
		Canvas.SetTop(this.txtSellerLevel, 2);
		this.Container.Children.Add(this.txtGoodsNum);
		this.txtGoodsNum.TextColor = new SolidColorBrush(4278236930U);
		Canvas.SetLeft(this.txtGoodsNum, 529);
		Canvas.SetTop(this.txtGoodsNum, 2);
	}

	public ImageBrush BodyBackground
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
			this.sprite.width = value;
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
			this.sprite.height = value;
		}
	}

	public string Goods
	{
		set
		{
			this.txtStallNum.Text = value;
		}
	}

	public int GoodsSellerID
	{
		get
		{
			return this._GoodsSellerID;
		}
		set
		{
			this._GoodsSellerID = value;
		}
	}

	public string GoodsSellerName
	{
		set
		{
			this.txtSellerName.Text = value;
		}
	}

	public string GoodsSellerLevel
	{
		set
		{
			this.txtSellerLevel.Text = value;
		}
	}

	public string GoodsNum
	{
		set
		{
			this.txtGoodsNum.Text = value;
		}
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		base.buttonMode = true;
		base.BackgroundAlpha = 0.2;
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		base.buttonMode = false;
		base.BackgroundAlpha = 0.01;
	}

	public GTextBlockOutLine txtStallNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine txtSellerName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine txtSellerLevel = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine txtGoodsNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public Sprite sprite = new Sprite();

	private int _GoodsSellerID;
}
