using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class SaledoublePart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Container.Children.Add(this.taxRateMoney);
		Canvas.SetLeft(this.taxRateMoney, 155);
		Canvas.SetTop(this.taxRateMoney, 35);
		this.taxRateMoney.TextColor = new SolidColorBrush(4294901760U);
		this.taxRateMoney.Text = "+100";
		this.Container.Children.Add(this.taxRateTotalMoney);
		Canvas.SetLeft(this.taxRateTotalMoney, 155);
		Canvas.SetTop(this.taxRateTotalMoney, 61);
		this.taxRateTotalMoney.TextColor = new SolidColorBrush(4294901760U);
		this.taxRateTotalMoney.Text = "+1000";
		this.taxRateMoney.Visibility = false;
		this.taxRateTotalMoney.Visibility = false;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public int MaxInputCount
	{
		get
		{
			return this._MaxInputCount;
		}
		set
		{
			this._MaxInputCount = value;
		}
	}

	public string ThingText
	{
		get
		{
			return this.NameBlockText.Text;
		}
		set
		{
			this.NameBlockText.Text = value;
		}
	}

	public string Price
	{
		get
		{
			return this.PriceBlockText.Text;
		}
		set
		{
			this.PriceBlockText.Text = value;
			this.AllPrice.Text = value;
			this.GoodsPrice = this.Price;
		}
	}

	public double LingDiTax
	{
		get
		{
			return this._LingDiTax;
		}
		set
		{
			this._LingDiTax = value;
		}
	}

	public void HideGoodsNum()
	{
		this.GoodsNum.ReadOnly = true;
	}

	private void NumTimer_Tick(object sender, object e)
	{
		if (!this.ToAddNum)
		{
			this.SaleCount--;
			if (this.SaleCount <= 1)
			{
				this.SaleCount = 1;
			}
		}
		else
		{
			this.SaleCount++;
			if (this.SaleCount >= this.MaxInputCount)
			{
				this.SaleCount = this.MaxInputCount;
			}
		}
		this.GoodsNum.EditText = StringUtil.substitute("{0}", new object[]
		{
			this.SaleCount
		});
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.GoodsNum = U3DUtils.NEW<GTextBlock>();
		this.GoodsNum.EditText = "1";
		this.GoodsNum.Onlydouble = true;
		this.GoodsNum.BodyWidth = 97.0;
		this.GoodsNum.BodyHeight = 21.0;
		this.GoodsNum.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 97.0, 21.0, 3.0, 2.0));
		this.GoodsNum.Text.TextBoxChanged = new EventHandler(this.GoodsNum_TextChanged);
		Canvas.SetLeft(this.GoodsNum, 52);
		Canvas.SetTop(this.GoodsNum, 82);
		this.Container.Children.Add(this.GoodsNum);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("购买");
		Canvas.SetLeft(gicon, 52);
		Canvas.SetTop(gicon, 120);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OkClick);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 25.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("取消");
		Canvas.SetLeft(gicon, 131);
		Canvas.SetTop(gicon, 120);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.CancelClick);
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.FontSize = 12;
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Foreground = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 206, 0));
		Canvas.SetLeft(gtextBlockOutLine, 81);
		Canvas.SetTop(gtextBlockOutLine, 35);
		this.Container.Children.Add(gtextBlockOutLine);
		this.PriceBlockText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.FontSize = 12;
		gtextBlockOutLine.Text = "0";
		gtextBlockOutLine.Foreground = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 183, 2));
		Canvas.SetLeft(gtextBlockOutLine, 55);
		Canvas.SetTop(gtextBlockOutLine, 11);
		this.Container.Children.Add(gtextBlockOutLine);
		this.NameBlockText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.FontSize = 12;
		gtextBlockOutLine.Text = string.Empty;
		gtextBlockOutLine.Foreground = new SolidColorBrush(uint.MaxValue);
		gtextBlockOutLine.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 206, 0));
		Canvas.SetLeft(gtextBlockOutLine, 81);
		Canvas.SetTop(gtextBlockOutLine, 60);
		this.Container.Children.Add(gtextBlockOutLine);
		this.AllPrice = gtextBlockOutLine;
	}

	private bool ConvertSaleCount()
	{
		try
		{
			this.SaleCount = Convert.ToInt32(this.GoodsNum.EditText);
			if (this.SaleCount <= 0)
			{
				this.SaleCount = 1;
			}
			if (this.SaleCount > this.MaxInputCount)
			{
				this.SaleCount = this.MaxInputCount;
			}
			return true;
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
		Super.ShowMessageBox(this.Container, 0, Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("输入的数字格式错误!"), new object[0]), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
		return false;
	}

	private void OkClick(object sender, MouseEvent e)
	{
		if (!this.ConvertSaleCount())
		{
			return;
		}
		this.ButtonState = "OK";
		if (this.ButtonClick != null)
		{
			this.ButtonClick.Invoke(sender, e);
		}
	}

	private void CancelClick(object sender, MouseEvent e)
	{
		this.ButtonState = "Cancel";
		if (this.ButtonClick != null)
		{
			this.ButtonClick.Invoke(sender, e);
		}
	}

	public void InitPartData()
	{
		this.taxRateMoney.Text = StringUtil.substitute("+{0}", new object[]
		{
			(int)((double)Convert.ToInt32(this.GoodsPrice) * this.LingDiTax)
		});
		this.taxRateTotalMoney.Text = StringUtil.substitute("+{0}", new object[]
		{
			(int)((double)(Convert.ToInt32(this.GoodsPrice) * this.SaleCount) * this.LingDiTax)
		});
	}

	private void GoodsNum_TextChanged(object sender, object e)
	{
		int num = 0;
		try
		{
			if (this.GoodsNum.EditText == string.Empty)
			{
				return;
			}
			num = Convert.ToInt32(this.GoodsNum.EditText);
			if (num >= this.MaxInputCount)
			{
				num = this.MaxInputCount;
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
			num = 1;
		}
		this.GoodsNum.EditText = num.ToString();
		this.AllPrice.Text = (num * Convert.ToInt32(this.GoodsPrice)).ToString();
		this.taxRateTotalMoney.Text = StringUtil.substitute("+{0}", new object[]
		{
			(int)((double)(num * Convert.ToInt32(this.GoodsPrice)) * this.LingDiTax)
		});
		this.SaleCount = num;
	}

	public string GoodsPrice = string.Empty;

	private GTextBlockOutLine NameBlockText;

	private GTextBlockOutLine PriceBlockText;

	private GTextBlockOutLine AllPrice;

	private bool ToAddNum = true;

	private GTextBlock GoodsNum;

	public int SaleCount = 1;

	public string ButtonState = "Cancel";

	private Canvas Root;

	private GTextBlockOutLine taxRateMoney = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine taxRateTotalMoney = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private int _MaxInputCount;

	private double _LingDiTax;

	public EventHandler ButtonClick;
}
