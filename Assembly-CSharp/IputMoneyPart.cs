using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class IputMoneyPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.thisCtrl = this;
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("确  定");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				if (Global.StringTrim(this.MoneyTextBlock.Text.Text) == string.Empty)
				{
					this.MoneyTextBlock.Text.Text = "0";
				}
				if (this.typeMoney == 1)
				{
					if (Convert.ToInt64(Global.StringTrim(this.MoneyTextBlock.Text.Text)) > (long)Global.Data.roleData.YinLiang)
					{
						this.iMyInputMoney = (double)Global.Data.roleData.YinLiang;
					}
					else
					{
						this.iMyInputMoney = (double)Convert.ToInt64(Global.StringTrim(this.MoneyTextBlock.Text.Text));
					}
					this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
					{
						ID = 2,
						IDType = 0
					});
				}
				if (this.typeMoney == 2)
				{
					if (Convert.ToInt64(Global.StringTrim(this.MoneyTextBlock.Text.Text)) > (long)Global.Data.roleData.UserMoney)
					{
						this.iMyInputMoney = (double)Global.Data.roleData.UserMoney;
					}
					else
					{
						this.iMyInputMoney = (double)Convert.ToInt64(Global.StringTrim(this.MoneyTextBlock.Text.Text));
					}
					this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
					{
						ID = 3,
						IDType = 0
					});
				}
			}
		};
		Canvas.SetLeft(gicon, 63);
		Canvas.SetTop(gicon, 134);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("全  部");
		Canvas.SetLeft(gicon, 159);
		Canvas.SetTop(gicon, 95);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.typeMoney == 1)
			{
				this.MoneyTextBlock.EditText = Global.Data.roleData.YinLiang.ToString();
			}
			else
			{
				this.MoneyTextBlock.EditText = Global.Data.roleData.UserMoney.ToString();
			}
		};
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 81.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon.DisableBodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
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
		Canvas.SetLeft(gicon, 150);
		Canvas.SetTop(gicon, 134);
		this.Container.Children.Add(gicon);
		this.MoneyTextBlock = U3DUtils.NEW<GTextBlock>();
		this.MoneyTextBlock.BodyWidth = 120.0;
		this.MoneyTextBlock.BodyHeight = 21.0;
		this.MoneyTextBlock.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 120.0, 21.0, 3.0, 2.0));
		this.MoneyTextBlock.Onlydouble = true;
		this.MoneyTextBlock.Text.Y = 2;
		this.MoneyTextBlock.Text.X = 25;
		this.MoneyTextBlock.Text.border = false;
		this.MoneyTextBlock.Text.Text = "0";
		TextFormat textFormat = new TextFormat();
		this.MoneyTextBlock.Text.setTextFormat(textFormat);
		this.MoneyTextBlock.Text.FontSize = FontSizeMgr.NormalInputFontSize;
		this.MoneyTextBlock.Text.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.MoneyTextBlock, 36);
		Canvas.SetTop(this.MoneyTextBlock, 94);
		this.Container.Children.Add(this.MoneyTextBlock);
		this.MoneyTextBlock.Text.TextBoxChanged = new EventHandler(this.MoneyNum_TextChanged);
		string uri = string.Empty;
		if (this.typeMoney == 1)
		{
			uri = "Images/Plate/ico_jb.png";
		}
		else if (this.typeMoney == 2)
		{
			uri = "Images/Plate/ico_yb.png";
		}
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 14.0;
		gicon.Height = 14.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage(uri));
		Canvas.SetLeft(gicon, 39);
		Canvas.SetTop(gicon, 97);
		this.Container.Children.Add(gicon);
	}

	private void MoneyNum_TextChanged(object sender, object e)
	{
		int num = 0;
		try
		{
			num = Convert.ToInt32(this.MoneyTextBlock.EditText);
			if (this.typeMoney == 1)
			{
				if (num >= Global.Data.roleData.YinLiang)
				{
					num = Global.Data.roleData.YinLiang;
				}
			}
			else if (num >= Global.Data.roleData.UserMoney)
			{
				num = Global.Data.roleData.UserMoney;
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
			num = 0;
		}
		this.MoneyTextBlock.EditText = num.ToString();
	}

	public void InitPartData()
	{
		if (this.typeMoney == 1)
		{
			this.RefurbishAllM(Global.Data.roleData.YinLiang);
		}
		else
		{
			this.RefurbishAllM(Global.Data.roleData.UserMoney);
		}
	}

	private void RefurbishAllM(int money1)
	{
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 225, 206, 0));
		gtextBlockOutLine.Text = money1.ToString();
		Canvas.SetLeft(gtextBlockOutLine, 116);
		Canvas.SetTop(gtextBlockOutLine, 45);
		this.Container.Children.Add(gtextBlockOutLine);
	}

	private GTextBlock MoneyTextBlock;

	public double iMyInputMoney;

	private Canvas Root;

	private SpriteSL thisCtrl = new SpriteSL();

	public int typeMoney;

	public DPSelectedItemEventHandler DPSelectedItem;
}
