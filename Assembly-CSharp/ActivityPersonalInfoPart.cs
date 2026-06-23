using System;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class ActivityPersonalInfoPart : UserControl
{
	public ActivityPersonalInfoPart()
	{
		this.textBox1.Foreground = new SolidColorBrush(uint.MaxValue);
	}

	public GTabControl tc
	{
		get
		{
			return this._tc;
		}
		set
		{
			this._tc = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.ImgTitle1);
		this.ImgTitle1.Width = 118.0;
		this.ImgTitle1.Height = 22.0;
		Canvas.SetLeft(this.ImgTitle1, 53);
		Canvas.SetTop(this.ImgTitle1, 6);
		this.ImgTitle1.HorizontalAlignment = global::Layout.Center;
		this.Container.Children.Add(this.ScrollViewer1);
		this.ScrollViewer1.Width = 530.0;
		this.ScrollViewer1.Height = 302.0;
		Canvas.SetLeft(this.ScrollViewer1, 18);
		Canvas.SetTop(this.ScrollViewer1, 50);
		this.ScrollViewer1.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		this.ScrollViewer1.Viewer = this.Panel;
		this.Container.Children.Add(this.txtNum);
		this.txtNum.Text = string.Empty;
		this.txtNum.TextColor = new SolidColorBrush(65280U);
		this.txtNum.FontSize = HSTextField.defaultFontSize;
		Canvas.SetLeft(this.txtNum, 450);
		Canvas.SetTop(this.txtNum, 20);
		this.Panel.Children.Add(this.textBox1);
		Canvas.SetLeft(this.textBox1, 0);
		Canvas.SetTop(this.textBox1, 49);
		this.textBox1.Width = 520.0;
		this.textBox1.TextWrapping = true;
		this.textBox1.AcceptsReturn = true;
		this.textBox1.border = false;
		this.ScrollViewer1.ResetScrollView();
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	public void InitPartData()
	{
		this.ImgTitle1.URL = Global.GetGameResImageURL("Images/Plate/txt_cz04.png");
	}

	public void GetNewData()
	{
		this.ShowItemList();
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	private void ShowItemList()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < Super.GData.PersonalTextItemList.Count; i++)
		{
			stringBuilder.Append(Super.GData.PersonalTextItemList[i]);
			stringBuilder.Append("\n");
		}
		this.txtNum.Text = StringUtil.substitute(Global.GetLang("总共: {0} 条"), new object[]
		{
			Super.GData.PersonalTextItemList.Count
		});
		this.textBox1.Text = stringBuilder.ToString();
		this.ScrollViewer1.ResetScrollView();
	}

	private URLImage ImgTitle1 = new URLImage();

	private GTextBlockOutLine txtNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private TextBox textBox1 = new TextBox();

	private StackPanel Panel = new StackPanel();

	private GScrollView ScrollViewer1 = new GScrollView(0, 0, 0);

	private GTabControl _tc;
}
