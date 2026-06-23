using System;
using HSGameEngine.GameEngine.SilverLight;

public class CampaignGiftListItem : UserControl
{
	public double BodyWidth
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

	public double BodyHeight
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

	public Brush FrameBackground
	{
		set
		{
			this.ImgFrame.Background = value;
		}
	}

	public double FrameContentWidth
	{
		get
		{
			return this.ImgFrame.Width;
		}
		set
		{
			this.ImgFrame.Width = value;
		}
	}

	public double FrameContentHeight
	{
		get
		{
			return this.ImgFrame.Height;
		}
		set
		{
			this.ImgFrame.Height = value;
		}
	}

	public string GoodImgURL
	{
		set
		{
			this.ImgBak.URL = value;
		}
	}

	public GIcon LingQuBtn
	{
		set
		{
			this.ImgBtn.Children.Add(value);
		}
	}

	public string HuoDongJieShao
	{
		set
		{
			this.txtHuoDongJieShao.Text = value;
		}
	}

	public string GoodName_01
	{
		set
		{
			this.txtGoodName_01.Text = value;
		}
	}

	public string GoodIntro_01
	{
		set
		{
			this.txtGoodIntro_01.Text = value;
			Canvas.SetLeft(this.txtGoodIntro_01, Canvas.GetLeft(this.txtGoodName_01) + this.txtGoodName_01.RealSize.Width);
		}
	}

	public string GoodName_02
	{
		set
		{
			this.txtGoodName_02.Text = value;
		}
	}

	public string GoodIntro_02
	{
		set
		{
			this.txtGoodIntro_02.Text = value;
			Canvas.SetLeft(this.txtGoodIntro_02, Canvas.GetLeft(this.txtGoodName_02) + this.txtGoodName_02.RealSize.Width);
		}
	}

	public string txtItem_2
	{
		set
		{
			this.ItemText.Text = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.ImgFrame);
		this.ImgFrame.Width = 40.0;
		this.ImgFrame.Height = 40.0;
		Canvas.SetLeft(this.ImgFrame, 421);
		Canvas.SetTop(this.ImgFrame, 13);
		this.ImgFrame.Children.Add(this.ImgBak);
		this.ImgBak.Width = 32.0;
		this.ImgBak.Height = 32.0;
		Canvas.SetLeft(this.ImgBak, 4);
		Canvas.SetTop(this.ImgBak, 4);
		this.Container.Children.Add(this.ImgBtn);
		this.ImgBtn.Width = 81.0;
		this.ImgBtn.Height = 25.0;
		Canvas.SetLeft(this.ImgBtn, 399);
		Canvas.SetTop(this.ImgBtn, 63);
		this.Container.Children.Add(this.txtHuoDongJieShao);
		Canvas.SetLeft(this.txtHuoDongJieShao, 8);
		Canvas.SetTop(this.txtHuoDongJieShao, 4);
		this.txtHuoDongJieShao.Width = 373.0;
		this.txtHuoDongJieShao.TextColor = new SolidColorBrush(6597308U);
		this.Container.Children.Add(this.txtGoodName_01);
		this.txtGoodName_01.TextColor = new SolidColorBrush(10289405U);
		Canvas.SetLeft(this.txtGoodName_01, 10);
		Canvas.SetTop(this.txtGoodName_01, 57);
		this.txtGoodName_01.Width = 100.0;
		this.Container.Children.Add(this.txtGoodIntro_01);
		this.txtGoodIntro_01.TextColor = new SolidColorBrush(10793599U);
		Canvas.SetLeft(this.txtGoodIntro_01, 106);
		Canvas.SetTop(this.txtGoodIntro_01, 57);
		this.txtGoodIntro_01.Width = 295.0;
		this.Container.Children.Add(this.txtGoodName_02);
		this.txtGoodName_02.TextColor = new SolidColorBrush(10289405U);
		Canvas.SetLeft(this.txtGoodName_02, 10);
		Canvas.SetTop(this.txtGoodName_02, 73);
		this.txtGoodName_02.Width = 100.0;
		this.Container.Children.Add(this.txtGoodIntro_02);
		this.txtGoodIntro_02.TextColor = new SolidColorBrush(10793599U);
		Canvas.SetLeft(this.txtGoodIntro_02, 106);
		Canvas.SetTop(this.txtGoodIntro_02, 73);
		this.txtGoodIntro_02.Width = 295.0;
		this.ItemText.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 145, 0));
		this.ItemText.Text = string.Empty;
		this.ItemText.BodyWidth = 295.0;
		this.ItemText.BodyHeight = 16.0;
		Canvas.SetLeft(this.ItemText, 18);
		Canvas.SetTop(this.ItemText, 22);
		this.ItemText.TextWrapping = TextWrapping.Wrap;
		this.Container.Children.Add(this.ItemText);
	}

	private Canvas ImgFrame = new Canvas();

	private URLImage ImgBak = new URLImage();

	private Canvas ImgBtn = new Canvas();

	private GTextBlockOutLine txtHuoDongJieShao = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtGoodName_01 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtGoodIntro_01 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtGoodName_02 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtGoodIntro_02 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine ItemText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
}
