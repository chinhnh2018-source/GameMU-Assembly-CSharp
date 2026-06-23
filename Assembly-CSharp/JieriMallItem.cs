using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class JieriMallItem : UserControl
{
	public JieriMallItem()
	{
		this.ItemCollection = this.IconListBox.ItemsSource;
	}

	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.IconListBox);
		this.IconListBox.Width = 48.0;
		this.IconListBox.Height = 48.0;
		Canvas.SetLeft(this.IconListBox, 0);
		Canvas.SetTop(this.IconListBox, 0);
		this.IconListBox.Background = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.Price);
		this.Price.Height = 15.0;
		this.Price.Width = 41.0;
		this.Price.FontSize = HSTextField.defaultFontSize;
		Canvas.SetLeft(this.Price, 85);
		Canvas.SetTop(this.Price, 3);
		this.Price.TextColor = new SolidColorBrush(4293978114U);
		this.Container.Children.Add(this.ImgBtn);
		Canvas.SetLeft(this.ImgBtn, 63);
		Canvas.SetTop(this.ImgBtn, 26);
	}

	public double BodyWidth
	{
		get
		{
			return this.Width;
		}
		set
		{
			this.Width = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this.Height;
		}
		set
		{
			this.Height = value;
		}
	}

	public GIcon IconSource
	{
		get
		{
			if (this.ItemCollection.Count > 0)
			{
				return U3DUtils.AS<GIcon>(this.ItemCollection[0]);
			}
			return null;
		}
		set
		{
			this.ItemCollection.Clear();
			this.ItemCollection.Add(value);
		}
	}

	public int MallGoodsID
	{
		get
		{
			return this._MallGoodsID;
		}
		set
		{
			this._MallGoodsID = value;
		}
	}

	public string GoodsPrice
	{
		get
		{
			return this.Price.Text;
		}
		set
		{
			this.Price.Text = value;
		}
	}

	public GIcon GoodsBuyBtn
	{
		get
		{
			if (this.ImgBtn.numChildren > 0)
			{
				return U3DUtils.AS<GIcon>(this.ImgBtn.getChildAt(this.ImgBtn.numChildren - 1));
			}
			return null;
		}
		set
		{
			this.ImgBtn.Children.Add(value);
		}
	}

	private ListBox IconListBox = new ListBox();

	private GTextBlockOutLine Price = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Canvas ImgBtn = new Canvas();

	private ObservableCollection _ItemCollection;

	private int _MallGoodsID;
}
