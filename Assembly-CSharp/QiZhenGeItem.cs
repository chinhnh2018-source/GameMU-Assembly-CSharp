using System;
using HSGameEngine.GameEngine.SilverLight;

public class QiZhenGeItem : UserControl
{
	public QiZhenGeItem()
	{
		this.ItemCollection = this.IconListBox.ItemsSource;
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

	public string GoodsName
	{
		set
		{
			this.GoodsNameText.Text = value;
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

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.GoodsNameText);
		Canvas.SetLeft(this.GoodsNameText, 18);
		Canvas.SetTop(this.GoodsNameText, 9);
		this.GoodsNameText.FontSize = HSTextField.defaultFontSize;
		this.GoodsNameText.TextColor = new SolidColorBrush(4278255360U);
		this.Container.Children.Add(this.IconListBox);
		this.IconListBox.Width = 48.0;
		this.IconListBox.Height = 48.0;
		Canvas.SetLeft(this.IconListBox, 28);
		Canvas.SetTop(this.IconListBox, 39);
		this.IconListBox.Background = new SolidColorBrush(16777215U);
		this.IconListBox.BorderThickness = 0;
	}

	private GTextBlockOutLine GoodsNameText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox IconListBox = new ListBox();

	public ObservableCollection ItemCollection;
}
