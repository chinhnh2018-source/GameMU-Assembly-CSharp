using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class IputYbPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
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
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("确  定");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				if (Global.StringTrim(this.YuanBaoTextBlock.Text.Text) == string.Empty)
				{
					this.YuanBaoTextBlock.Text.Text = "0";
				}
				if (Convert.ToInt64(Global.StringTrim(this.YuanBaoTextBlock.Text.Text)) > (long)Global.Data.roleData.UserMoney)
				{
					this.iMyInputYb = (double)Global.Data.roleData.UserMoney;
				}
				else
				{
					this.iMyInputYb = (double)Convert.ToInt64(Global.StringTrim(this.YuanBaoTextBlock.Text.Text));
				}
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 2,
					IDType = 0
				});
			}
		};
		Canvas.SetLeft(gicon, 28);
		Canvas.SetTop(gicon, 126);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("全  部");
		Canvas.SetLeft(gicon, 184);
		Canvas.SetTop(gicon, 84);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.YuanBaoTextBlock.Text.Text = string.Empty;
			this.YuanBaoTextBlock.Text.Text = Global.Data.roleData.UserMoney.ToString();
		};
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("取  消");
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
		Canvas.SetLeft(gicon, 184);
		Canvas.SetTop(gicon, 126);
		this.Container.Children.Add(gicon);
		this.YuanBaoTextBlock = U3DUtils.NEW<GTextBlock>();
		this.YuanBaoTextBlock.BodyWidth = 136.0;
		this.YuanBaoTextBlock.BodyHeight = 21.0;
		this.YuanBaoTextBlock.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 136.0, 21.0, 3.0, 2.0));
		this.YuanBaoTextBlock.Onlydouble = true;
		this.YuanBaoTextBlock.Text.Text = string.Empty;
		this.YuanBaoTextBlock.Text.FontSize = FontSizeMgr.NormalInputFontSize;
		this.YuanBaoTextBlock.Text.Padding = new Thickness(30.0, 6.0, 4.0, 0.0);
		this.YuanBaoTextBlock.Text.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.YuanBaoTextBlock, 14);
		Canvas.SetTop(this.YuanBaoTextBlock, 83);
		this.Container.Children.Add(this.YuanBaoTextBlock);
		this.YuanBaoTextBlock.Text.TextChanged = new EventHandler(this.YuanBaoNum_TextChanged);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 16.0;
		gicon.Height = 11.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/ico_yb.png"));
		Canvas.SetLeft(gicon, 18);
		Canvas.SetTop(gicon, 89);
		this.Container.Children.Add(gicon);
	}

	private void YuanBaoNum_TextChanged(object sender, object e)
	{
		int num = 0;
		try
		{
			num = Convert.ToInt32(this.YuanBaoTextBlock.EditText);
			if (num >= Global.Data.roleData.UserMoney)
			{
				num = Global.Data.roleData.UserMoney;
			}
		}
		catch (Exception)
		{
			num = 0;
		}
		this.YuanBaoTextBlock.EditText = num.ToString();
	}

	public void InitPartData()
	{
		this.RefurbishAllM(Global.Data.roleData.UserMoney);
	}

	private void RefurbishAllM(int yanBao)
	{
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 225, 206, 0));
		gtextBlockOutLine.Text = yanBao.ToString();
		Canvas.SetLeft(gtextBlockOutLine, 116);
		Canvas.SetTop(gtextBlockOutLine, 27);
		this.Container.Children.Add(gtextBlockOutLine);
	}

	private GTextBlock YuanBaoTextBlock;

	public double iMyInputYb;

	private Canvas Root;

	private SpriteSL thisCtrl = new SpriteSL();

	public DPSelectedItemEventHandler DPSelectedItem;
}
