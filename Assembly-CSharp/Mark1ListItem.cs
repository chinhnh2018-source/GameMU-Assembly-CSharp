using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class Mark1ListItem : UserControl
{
	public Mark1ListItem()
	{
		this.txtGoodName.BodyWidth = 36.0;
		this.txtGoodName.BodyHeight = 30.0;
		this.txtGoodName.TextFontWrapping = TextWrapping.Wrap;
		this.txtGoodName.TextColor = new SolidColorBrush(47364U);
		base.addEventListener("mouseOver", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("mouseOut", new MouseEventHandler(this.UserControl_MouseLeave));
		this.drawSprite();
	}

	private void drawSprite()
	{
		this.sprite.graphics.beginFill(0U);
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
		this.ContentBak.Width = 40.0;
		this.ContentBak.Height = 40.0;
		Canvas.SetLeft(this.ContentBak, 16);
		Canvas.SetTop(this.ContentBak, 20);
		this.Container.Children.Add(this.ImgBtn);
		this.ImgBtn.Width = 32.0;
		this.ImgBtn.Height = 32.0;
		Canvas.SetLeft(this.ImgBtn, 20);
		Canvas.SetTop(this.ImgBtn, 24);
		this.Container.Children.Add(this.txtGoodName);
		this.txtGoodName.TextColor = new SolidColorBrush(4278237444U);
		Canvas.SetLeft(this.txtGoodName, 64);
		Canvas.SetTop(this.txtGoodName, 28);
		this.sprite.visible = false;
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

	public string GoodsName
	{
		get
		{
			return this.txtGoodName.Text;
		}
		set
		{
			this.txtGoodName.Text = value;
		}
	}

	public int GoodsID
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

	public GIcon GoodsImg
	{
		set
		{
			this.ImgBtn.Children.Add(value);
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

	private void UserControl_MouseEnter(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
		}
		this.sprite.visible = true;
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		base.Cursor = Cursors.Auto;
		this.sprite.visible = false;
	}

	private Canvas ContentBak = new Canvas();

	private Canvas ImgBtn = new Canvas();

	private GTextBlockOutLine txtGoodName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public Sprite sprite = new Sprite();

	private int _GoodID = -1;
}
