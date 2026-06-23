using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class GongChengSlQueryPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.txtTime);
		this.txtTime.TextColor = new SolidColorBrush(963281U);
		Canvas.SetLeft(this.txtTime, 25);
		Canvas.SetTop(this.txtTime, 17);
		this.Container.Children.Add(this.txtG);
		this.txtG.TextColor = new SolidColorBrush(16776960U);
		Canvas.SetLeft(this.txtG, 25);
		Canvas.SetTop(this.txtG, 37);
		this.Container.Children.Add(this.txtS);
		Canvas.SetLeft(this.txtS, 25);
		Canvas.SetTop(this.txtS, 57);
		this.thisCtrl = this;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("取 消");
		gicon.TextColor = new SolidColorBrush(10551295U);
		Canvas.SetLeft(gicon, 148);
		Canvas.SetTop(gicon, 100);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0
				});
			}
		};
	}

	public void InitPartData()
	{
		this.txtTime.Text = Global.GetLang("攻城时间：2012-10-01 20:00至20:59");
		this.txtG.Text = Global.GetLang("攻城战盟：共产党");
		this.txtS.Text = Global.GetLang("守城战盟：国民党");
	}

	private Canvas Root;

	private GTextBlockOutLine txtTime = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtG = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtS = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private SpriteSL thisCtrl = new SpriteSL();

	public DPSelectedItemEventHandler DPSelectedItem;
}
