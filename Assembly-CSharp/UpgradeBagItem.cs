using System;
using HSGameEngine.GameEngine.SilverLight;

public class UpgradeBagItem : UserControl
{
	public UpgradeBagItem()
	{
		this.ItemCollection = this.IconListBox.ItemsSource;
		this.ItemBackGround = this.BackGroundlistBox.ItemsSource;
		this.ItemCollection2 = this.listBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.IconListBox);
		Canvas.SetLeft(this.IconListBox, 0);
		Canvas.SetTop(this.IconListBox, 12);
		this.IconListBox.Width = 56.0;
		this.IconListBox.Height = 56.0;
		this.IconListBox.BorderThickness = 0;
		this.IconListBox.ItemMargin = new Thickness(4.0, 5.0, 0.0, 0.0);
		this.Container.Children.Add(this.LevelText);
		Canvas.SetLeft(this.LevelText, 3);
		Canvas.SetTop(this.LevelText, 80);
		this.LevelText.TextColor = new SolidColorBrush(46337U);
		this.Container.Children.Add(this.BackGroundlistBox);
		Canvas.SetLeft(this.BackGroundlistBox, 100);
		Canvas.SetTop(this.BackGroundlistBox, 0);
		this.BackGroundlistBox.Background = new SolidColorBrush(16777215U);
		this.BackGroundlistBox.Width = 365.0;
		this.BackGroundlistBox.Height = 105.0;
		this.BackGroundlistBox.ItemMargin = new Thickness(10.0, 10.0, 0.0, 0.0);
		this.BackGroundlistBox.BorderThickness = 0;
		this.Container.Children.Add(this.listBox);
		Canvas.SetLeft(this.listBox, 100);
		Canvas.SetTop(this.listBox, 0);
		this.listBox.Background = new SolidColorBrush(16777215U);
		this.listBox.Width = 365.0;
		this.listBox.Height = 105.0;
		this.listBox.ItemMargin = new Thickness(14.0, 14.0, 4.0, 4.0);
		this.listBox.BorderThickness = 0;
		this.Container.Children.Add(this.ImgBtn);
		Canvas.SetLeft(this.ImgBtn, 390);
		Canvas.SetTop(this.ImgBtn, 37);
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

	public Brush IconListBoxBackground
	{
		set
		{
			this.IconListBox.Background = (SolidColorBrush)value;
		}
	}

	public GIcon GoodsBoxBackground
	{
		set
		{
			this.ItemBackGround.Add(value);
		}
	}

	public string BagLevel
	{
		set
		{
			this.LevelText.Text = value;
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

	public GIcon TakeGoodsBtn
	{
		set
		{
			this.ImgBtn.Children.Add(value);
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

	private ListBox IconListBox = new ListBox();

	private GTextBlockOutLine LevelText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox BackGroundlistBox = new ListBox();

	private Canvas ImgBtn = new Canvas();

	private ListBox listBox = new ListBox();

	private ObservableCollection _ItemCollection;

	private ObservableCollection _ItemCollection2;

	private ObservableCollection _ItemBackGround;
}
