using System;
using HSGameEngine.GameEngine.SilverLight;

public class ChongZhiDaliItem : UserControl
{
	public ChongZhiDaliItem()
	{
		this.ItemBackGround = this.BackGroundlistBox.ItemsSource;
		this.ItemCollection2 = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.GtextTitle);
		Canvas.SetLeft(this.GtextTitle, 23);
		Canvas.SetTop(this.GtextTitle, 13);
		this.GtextTitle.TextColor = new SolidColorBrush(46337U);
		this.Container.Children.Add(this.BackGroundlistBox);
		Canvas.SetLeft(this.BackGroundlistBox, 21);
		Canvas.SetTop(this.BackGroundlistBox, 35);
		this.BackGroundlistBox.Background = new SolidColorBrush(16777215U);
		this.BackGroundlistBox.Width = 507.0;
		this.BackGroundlistBox.Height = 105.0;
		this.BackGroundlistBox.ItemMargin = new Thickness(0.0, 0.0, 0.0, 0.0);
		this.BackGroundlistBox.BorderThickness = 0;
		this.Container.Children.Add(this.listBox);
		Canvas.SetLeft(this.listBox, 21);
		Canvas.SetTop(this.listBox, 39);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.Width = 507.0;
		this.listBox.Height = 105.0;
		this.listBox.ItemMargin = new Thickness(4.0, 0.0, 4.0, 8.0);
		this.Container.Children.Add(this.ImgBtn);
		this.ImgBtn.Width = 81.0;
		this.ImgBtn.Height = 25.0;
		Canvas.SetLeft(this.ImgBtn, 450);
		Canvas.SetTop(this.ImgBtn, 38);
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

	public GIcon LingQuBtn
	{
		set
		{
			this.ImgBtn.Children.Add(value);
		}
	}

	public GIcon GoodsBoxBackground
	{
		set
		{
			this.ItemBackGround.Add(value);
		}
	}

	public string GtextTitles
	{
		set
		{
			this.GtextTitle.Text = value;
		}
	}

	public GIcon BagBoxIcons
	{
		set
		{
			this.ItemCollection.Add(value);
		}
	}

	public GGoodIcon ListBoxIcons
	{
		set
		{
			this.ItemCollection2.Add(value);
		}
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

	public ObservableCollection ItemCollection2
	{
		get
		{
			return this._ItemCollection2;
		}
		set
		{
			this._ItemCollection2 = value;
		}
	}

	public ObservableCollection ItemBackGround
	{
		get
		{
			return this._ItemBackGround;
		}
		set
		{
			this._ItemBackGround = value;
		}
	}

	private GTextBlockOutLine GtextTitle = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox listBox = new ListBox();

	private ListBox BackGroundlistBox = new ListBox();

	private Canvas ImgBtn = new Canvas();

	private ObservableCollection _ItemCollection;

	private ObservableCollection _ItemCollection2;

	private ObservableCollection _ItemBackGround;
}
