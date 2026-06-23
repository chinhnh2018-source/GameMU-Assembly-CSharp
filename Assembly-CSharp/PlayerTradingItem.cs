using System;
using HSGameEngine.GameEngine.SilverLight;

public class PlayerTradingItem : UserControl
{
	public PlayerTradingItem()
	{
		this.ItemCollection = this.IconListBox.ItemsSource;
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.LeftPanel);
		Canvas.SetLeft(this.LeftPanel, -2);
		Canvas.SetTop(this.LeftPanel, 1);
		this.LeftPanel.Orientation = global::Layout.Horizontal;
		this.LeftPanel.Children.Add(this.IconListBox);
		this.IconListBox.HorizontalAlignment = global::Layout.Left;
		this.IconListBox.VerticalAlignment = global::Layout.Top;
		this.IconListBox.Width = 42.0;
		this.IconListBox.Height = 42.0;
		this.IconListBox.Background = new SolidColorBrush(16777215U);
		this.IconListBox.BorderThickness = 0;
		this.Container.Children.Add(this.RightPanel);
		this.RightPanel.Children.Add(this.NameText);
		this.NameText.Height = 23.0;
		this.NameText.FontSize = HSTextField.defaultFontSize;
		Canvas.SetLeft(this.NameText, 0);
		Canvas.SetTop(this.NameText, 13);
		this.NameText.Foreground = new SolidColorBrush(4278382594U);
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

	public double BodyWidth
	{
		get
		{
			return this.Width;
		}
		set
		{
			this.Width = value;
			this.Container.Width = value;
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
			this.Container.Height = value;
			this.LeftPanel.Height = value;
			this.RightPanel.Height = value;
		}
	}

	public double LeftPanelWidth
	{
		get
		{
			return this.LeftPanel.Width;
		}
		set
		{
			this.LeftPanel.Width = value;
			Canvas.SetLeft(this.RightPanel, value + 10.0);
		}
	}

	public double RightPanelWidth
	{
		get
		{
			return this.RightPanel.Width;
		}
		set
		{
			this.RightPanel.Width = value;
		}
	}

	public GIcon IconSource
	{
		set
		{
			this.ItemCollection.Clear();
			this.ItemCollection.Add(value);
		}
	}

	public string GoodsName
	{
		set
		{
			this.NameText.Text = value;
		}
	}

	private Canvas Root;

	private StackPanel LeftPanel = new StackPanel();

	private ListBox IconListBox = new ListBox();

	private Canvas RightPanel = new Canvas();

	private TextBlock NameText = new TextBlock();

	private ObservableCollection _ItemCollection;
}
