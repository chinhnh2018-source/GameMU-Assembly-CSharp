using System;
using HSGameEngine.GameEngine.SilverLight;

public class MeetGiftItem : UserControl
{
	public MeetGiftItem()
	{
		this.ItemCollection = this.IconListBox.ItemsSource;
		this.SmTextInfo.TextFontWrapping = TextWrapping.Wrap;
		this.SmTextInfo.TextHeight = 16.0;
	}

	public Brush IconListBoxBackground
	{
		set
		{
			this.IconListBox.Background = (SolidColorBrush)value;
		}
	}

	public string TextInfo
	{
		set
		{
			this.SmTextInfo.Text = value;
		}
	}

	public string GoodsNmae
	{
		set
		{
			this.SmText.Text = value;
		}
	}

	public string BindYuanbao
	{
		set
		{
			this.txtBindYuanbao.Text = value;
		}
	}

	public string BindTongqian
	{
		set
		{
			this.txtBindTongqian.Text = value;
		}
	}

	public uint GoodsNmaeColor
	{
		set
		{
			this.SmText.TextColor = new SolidColorBrush(value);
		}
	}

	public string TimeTexts
	{
		set
		{
			this.TimeText.Text = value;
		}
	}

	public int TimeTextsLeft
	{
		set
		{
			Canvas.SetLeft(this.TimeText, value);
		}
	}

	public int TimeTextsTop
	{
		set
		{
			Canvas.SetTop(this.TimeText, value);
		}
	}

	public GIcon BagBoxIcons
	{
		set
		{
			this.ItemCollection.Add(value);
		}
	}

	public GIcon TakeGoodsBtn
	{
		set
		{
			this.ImgBtn.Children.Add(value);
		}
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.IconListBox);
		Canvas.SetLeft(this.IconListBox, 0);
		Canvas.SetTop(this.IconListBox, 12);
		this.IconListBox.Width = 56.0;
		this.IconListBox.Height = 56.0;
		this.IconListBox.ItemMargin = new Thickness(4.0, 5.0, 0.0, 0.0);
		this.Container.Children.Add(this.TimeText);
		Canvas.SetLeft(this.TimeText, 3);
		Canvas.SetTop(this.TimeText, 80);
		this.TimeText.FontSize = HSTextField.defaultFontSize;
		this.TimeText.TextColor = new SolidColorBrush(46337U);
		this.Container.Children.Add(this.SmText);
		this.SmText.Text = string.Empty;
		Canvas.SetLeft(this.SmText, 80);
		Canvas.SetTop(this.SmText, 16);
		this.SmText.FontSize = HSTextField.defaultFontSize;
		this.SmText.TextColor = new SolidColorBrush(1999025U);
		this.Container.Children.Add(this.SmTextInfo);
		this.txtBindYuanbao.Text = string.Empty;
		Canvas.SetLeft(this.txtBindYuanbao, 80);
		Canvas.SetTop(this.txtBindYuanbao, 33);
		this.txtBindYuanbao.FontSize = HSTextField.defaultFontSize;
		this.txtBindYuanbao.TextColor = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.txtBindYuanbao);
		this.txtBindTongqian.Text = string.Empty;
		Canvas.SetLeft(this.txtBindTongqian, 80);
		Canvas.SetTop(this.txtBindTongqian, 50);
		this.txtBindTongqian.FontSize = HSTextField.defaultFontSize;
		this.txtBindTongqian.TextColor = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.txtBindTongqian);
		this.SmTextInfo.Text = string.Empty;
		this.SmTextInfo.Width = 216.0;
		Canvas.SetLeft(this.SmTextInfo, 168);
		Canvas.SetTop(this.SmTextInfo, 25);
		this.SmTextInfo.Width = 300.0;
		this.SmTextInfo.TextColor = new SolidColorBrush(14407004U);
		this.Container.Children.Add(this.ImgBtn);
		Canvas.SetLeft(this.ImgBtn, 390);
		Canvas.SetTop(this.ImgBtn, 37);
	}

	private ListBox IconListBox = new ListBox();

	private GTextBlockOutLine TimeText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine SmText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine SmTextInfo = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtBindYuanbao = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtBindTongqian = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Canvas ImgBtn = new Canvas();

	private ObservableCollection ItemCollection;
}
