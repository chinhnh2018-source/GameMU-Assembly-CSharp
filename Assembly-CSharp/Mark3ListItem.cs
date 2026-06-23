using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class Mark3ListItem : UserControl
{
	public Mark3ListItem()
	{
		this.txtName.TextFontWrapping = TextWrapping.Wrap;
		this.txtTongQianHint.Text = Global.GetLang("金币");
		this.txtYinPiaoHint.Text = Global.GetLang("钻石");
		base.addEventListener("mouseOver", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("mouseOut", new MouseEventHandler(this.UserControl_MouseLeave));
		this.drawSprite();
	}

	public string Goodsdouble { get; set; }

	private void drawSprite()
	{
		this.sprite.graphics.beginFill(5207664U);
		this.sprite.graphics.drawRect(0, 0, 100, 20);
		this.sprite.graphics.endFill();
		SpriteSL spriteSL = this.sprite;
		double num = 0.0;
		this.sprite.Y = num;
		spriteSL.X = num;
		this.sprite.alpha = 0.3;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.ContentBak);
		this.Container.Children.Add(this.sprite);
		this.ContentBak.Width = 41.0;
		this.ContentBak.Height = 42.0;
		Canvas.SetLeft(this.ContentBak, 47);
		Canvas.SetTop(this.ContentBak, 2);
		this.Container.Children.Add(this.txtdouble);
		this.txtdouble.TextColor = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtdouble, 15);
		Canvas.SetTop(this.txtdouble, 13);
		this.Container.Children.Add(this.GoodsImg);
		this.GoodsImg.Width = 32.0;
		this.GoodsImg.Height = 32.0;
		Canvas.SetLeft(this.GoodsImg, 50);
		Canvas.SetTop(this.GoodsImg, 5);
		this.Container.Children.Add(this.txtName);
		this.txtName.Width = 49.0;
		this.txtName.Height = 30.0;
		this.txtName.TextColor = new SolidColorBrush(4278236930U);
		this.txtName.BodyWidth = 69.0;
		Canvas.SetLeft(this.txtName, 97);
		Canvas.SetTop(this.txtName, 5);
		this.Container.Children.Add(this.txtTongQianHint);
		Canvas.SetLeft(this.txtTongQianHint, 215);
		Canvas.SetTop(this.txtTongQianHint, 6);
		this.txtTongQianHint.TextColor = new SolidColorBrush(uint.MaxValue);
		this.txtTongQianHint.Text = Global.GetLang("金币");
		this.Container.Children.Add(this.txtPriceGold);
		Canvas.SetLeft(this.txtPriceGold, 257);
		Canvas.SetTop(this.txtPriceGold, 6);
		this.txtPriceGold.TextColor = new SolidColorBrush(uint.MaxValue);
		this.Container.Children.Add(this.txtYinPiaoHint);
		Canvas.SetLeft(this.txtYinPiaoHint, 215);
		Canvas.SetTop(this.txtYinPiaoHint, 23);
		this.txtYinPiaoHint.TextColor = new SolidColorBrush(4294967095U);
		this.txtYinPiaoHint.Text = Global.GetLang("钻石");
		this.Container.Children.Add(this.txtPriceIngot);
		Canvas.SetLeft(this.txtPriceIngot, 257);
		Canvas.SetTop(this.txtPriceIngot, 23);
		this.txtPriceIngot.TextColor = new SolidColorBrush(4294967095U);
		this.Container.Children.Add(this.txtSellerName);
		Canvas.SetLeft(this.txtSellerName, 321);
		Canvas.SetTop(this.txtSellerName, 13);
		this.txtSellerName.TextColor = new SolidColorBrush(4294952960U);
		this.Container.Children.Add(this.txtSellerLevel);
		Canvas.SetLeft(this.txtSellerLevel, 462);
		Canvas.SetTop(this.txtSellerLevel, 13);
		this.txtSellerLevel.TextColor = new SolidColorBrush(uint.MaxValue);
		this.Container.Children.Add(this.ImgBtn);
		this.ImgBtn.Width = 81.0;
		this.ImgBtn.Height = 25.0;
		Canvas.SetLeft(this.ImgBtn, 518);
		Canvas.SetTop(this.ImgBtn, 7);
	}

	public string Tip
	{
		get
		{
			return this._Tip;
		}
		set
		{
			this._Tip = value;
		}
	}

	public int TipType
	{
		get
		{
			return this._TipType;
		}
		set
		{
			this._TipType = value;
		}
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

	public ImageBrush ContentBackground
	{
		set
		{
			this.ContentBak.Background = value;
		}
	}

	public double ContentWidth
	{
		get
		{
			return this.ContentBak.Width;
		}
		set
		{
			this.ContentBak.Width = value;
		}
	}

	public double ContentHeight
	{
		get
		{
			return this.ContentBak.Height;
		}
		set
		{
			this.ContentBak.Height = value;
		}
	}

	public GGoodIcon GoodImg
	{
		set
		{
			this.GoodsImg.Children.Add(value);
		}
	}

	public int OwnerRoleID
	{
		get
		{
			return this._OwnerRoleID;
		}
		set
		{
			this._OwnerRoleID = value;
		}
	}

	public int GoodsDbID
	{
		get
		{
			return this._GoodsDbID;
		}
		set
		{
			this._GoodsDbID = value;
		}
	}

	public string GoodsID
	{
		get
		{
			return this._GoodID;
		}
		set
		{
			this._GoodID = value;
		}
	}

	public int GoodsCount
	{
		get
		{
			return this._GoodsCount;
		}
		set
		{
			this._GoodsCount = value;
		}
	}

	public string Goods
	{
		set
		{
			this.txtdouble.Text = value;
		}
	}

	public string GoodsName
	{
		get
		{
			return this.txtName.Text;
		}
		set
		{
			this.txtName.Text = value;
		}
	}

	public string GoodsPriceGold
	{
		get
		{
			return this.txtPriceGold.Text;
		}
		set
		{
			this.txtPriceGold.Text = value;
		}
	}

	public string GoodsPriceIngot
	{
		get
		{
			return this.txtPriceIngot.Text;
		}
		set
		{
			this.txtPriceIngot.Text = value;
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

	public GIcon GoodsBuyBtn
	{
		set
		{
			this.ImgBtn.Children.Add(value);
		}
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
		}
		this.sprite.alpha = 0.3;
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		base.Cursor = Cursors.Auto;
		this.sprite.alpha = 0.01;
	}

	public Canvas ContentBak = new Canvas();

	public GTextBlockOutLine txtdouble = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public Canvas GoodsImg = new Canvas();

	public GTextBlockOutLine txtName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine txtTongQianHint = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine txtPriceGold = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine txtYinPiaoHint = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine txtPriceIngot = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine txtSellerName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine txtSellerLevel = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public Canvas ImgBtn = new Canvas();

	public Sprite sprite = new Sprite();

	public EventHandler MouseLeftButtonUp;

	private string _GoodID;

	private string _Tip;

	private int _TipType;

	private int _OwnerRoleID;

	private int _GoodsDbID;

	private int _GoodsCount;

	private int _GoodsSellerID;
}
