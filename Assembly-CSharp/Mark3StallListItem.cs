using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class Mark3StallListItem : UserControl
{
	public Mark3StallListItem()
	{
		this.txtGoodName.BodyWidth = 60.0;
		this.txtGoodName.BodyHeight = 30.0;
		this.txtGoodName.TextFontWrapping = TextWrapping.Wrap;
		base.addEventListener("mouseOver", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("mouseOut", new MouseEventHandler(this.UserControl_MouseLeave));
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.ContentBak);
		this.ContentBak.Children.Add(this.ImgBtn);
		this.ImgBtn.Width = 32.0;
		this.ImgBtn.Height = 32.0;
		Canvas.SetLeft(this.ImgBtn, 3);
		Canvas.SetTop(this.ImgBtn, 6);
		this.ContentBak.Children.Add(this.txtGoodName);
		this.txtGoodName.Width = 49.0;
		this.txtGoodName.Height = 30.0;
		this.txtGoodName.Width = 60.0;
		this.txtGoodName.Height = 28.0;
		this.txtGoodName.TextColor = new SolidColorBrush(4278236930U);
		Canvas.SetLeft(this.txtGoodName, 47);
		Canvas.SetTop(this.txtGoodName, 5);
		this.ContentBak.Children.Add(this.txtWork);
		this.txtWork.TextColor = new SolidColorBrush(10626862U);
		Canvas.SetLeft(this.txtWork, 47);
		Canvas.SetTop(this.txtWork, 34);
		this.ContentBak.Children.Add(this.txtPriceGold);
		this.txtPriceGold.TextColor = new SolidColorBrush(16764416U);
		Canvas.SetLeft(this.txtPriceGold, 181);
		Canvas.SetTop(this.txtPriceGold, 7);
		this.ContentBak.Children.Add(this.txtPriceIngot);
		this.txtPriceIngot.TextColor = new SolidColorBrush(16764416U);
		Canvas.SetLeft(this.txtPriceIngot, 181);
		Canvas.SetTop(this.txtPriceIngot, 30);
	}

	public Brush RootBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public double RootWidth
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

	public double RootHeight
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

	public ImageBrush ContainerBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public double ContainerWidth
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

	public double ContainerHeight
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

	public ImageURL ContentBackground
	{
		set
		{
			this.ContentBak.BackgroundURL = value;
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
			this.ImgBtn.Children.Add(value);
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

	public int GoodsID
	{
		get
		{
			return this._GoodId;
		}
		set
		{
			this._GoodId = value;
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

	public string Work
	{
		set
		{
			this.txtWork.Text = value;
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

	private void UserControl_MouseEnter(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
		}
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		base.Cursor = Cursors.Auto;
	}

	private Canvas ContentBak = new Canvas();

	private Canvas ImgBtn = new Canvas();

	private GTextBlockOutLine txtGoodName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtWork = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtPriceGold = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtPriceIngot = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private int _GoodId = -1;

	private int _OwnerRoleID;

	private int _GoodsDbID;

	private int _GoodsCount;
}
