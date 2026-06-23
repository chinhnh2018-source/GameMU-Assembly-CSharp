using System;
using HSGameEngine.GameEngine.SilverLight;

public class XingYunZhiJiangLiListItem : UserControl
{
	public XingYunZhiJiangLiListItem()
	{
		this.Container.Children.Add(this.txtXingYunZhi);
		Canvas.SetLeft(this.txtXingYunZhi, 20);
		Canvas.SetTop(this.txtXingYunZhi, 9);
		this.txtXingYunZhi.TextColor = new SolidColorBrush(65535U);
		this.Container.Children.Add(this.txtJiangLiDaoJuName);
		Canvas.SetLeft(this.txtJiangLiDaoJuName, 96);
		Canvas.SetTop(this.txtJiangLiDaoJuName, 9);
		this.txtJiangLiDaoJuName.TextColor = new SolidColorBrush(16776960U);
		this.Container.Children.Add(this.txtShuoMing);
		Canvas.SetLeft(this.txtShuoMing, 243);
		Canvas.SetTop(this.txtShuoMing, 9);
		this.txtShuoMing.TextColor = new SolidColorBrush(11448178U);
		this.Container.Children.Add(this.btnCanvas);
		this.btnCanvas.Width = 81.0;
		this.btnCanvas.Height = 25.0;
		Canvas.SetLeft(this.btnCanvas, 490);
		Canvas.SetTop(this.btnCanvas, 5);
	}

	public GIcon BtnLingQu
	{
		set
		{
			this.btnCanvas.Children.Add(value);
		}
	}

	public string XingYunZhiText
	{
		set
		{
			this.txtXingYunZhi.Text = value;
		}
	}

	public string JiangLiDaoJuNameText
	{
		set
		{
			this.txtJiangLiDaoJuName.Text = value;
		}
	}

	public string ShuoMingText
	{
		set
		{
			this.txtShuoMing.Text = value;
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

	private GTextBlockOutLine txtXingYunZhi = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtJiangLiDaoJuName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtShuoMing = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private Canvas btnCanvas = new Canvas();
}
