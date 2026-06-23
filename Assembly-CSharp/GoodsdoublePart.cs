using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class GoodsdoublePart : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.Root.Children.Add(this.bak);
	}

	public Brush BodyBackground
	{
		set
		{
			this.bak.Background = value;
		}
	}

	public double BodyBackOpacity
	{
		set
		{
			this.bak.Opacity = value;
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

	public int goodsID
	{
		get
		{
			return this._goodsID;
		}
		set
		{
			this._goodsID = value;
		}
	}

	public int OrigGoodsCount
	{
		get
		{
			return this._OrigGoodsCount;
		}
		set
		{
			this._OrigGoodsCount = value;
		}
	}

	private void NumTimer_Tick(object sender, object e)
	{
		if (!this.ToAddNum)
		{
			this.GoodsCount--;
			if (this.GoodsCount < 1)
			{
				this.GoodsCount = 0;
			}
		}
		else
		{
			this.GoodsCount++;
			if (this.GoodsCount >= this.MaxInputCount)
			{
				this.GoodsCount = this.MaxInputCount;
			}
		}
		this.GoodsNum.EditText = StringUtil.substitute("{0}", new object[]
		{
			this.GoodsCount
		});
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.bak.Width = (double)width;
		this.bak.Height = (double)height;
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
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("确定");
		Canvas.SetLeft(gicon, 171);
		Canvas.SetTop(gicon, 37);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OkClick);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("取消");
		Canvas.SetLeft(gicon, 171);
		Canvas.SetTop(gicon, 82);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.CancelClick);
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextSize = 12.0;
		gtextBlockOutLine.Text = string.Empty;
		gtextBlockOutLine.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 0, 183, 2));
		Canvas.SetLeft(gtextBlockOutLine, 10);
		Canvas.SetTop(gtextBlockOutLine, 11);
		this.Container.Children.Add(gtextBlockOutLine);
		this.NameBlockText = gtextBlockOutLine;
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextSize = 12.0;
		gtextBlockOutLine.Text = Global.GetLang("数量:");
		gtextBlockOutLine.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 44, 163, 190));
		Canvas.SetLeft(gtextBlockOutLine, 10);
		Canvas.SetTop(gtextBlockOutLine, 87);
		this.Container.Children.Add(gtextBlockOutLine);
		this.NumBlockText = gtextBlockOutLine;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.goodsID);
		if (goodsXmlNodeByID != null)
		{
			Image image = new Image();
			image.Stretch = global::StretchSL.None;
			Canvas.SetLeft(image, 10);
			Canvas.SetTop(image, 32);
			this.Container.Children.Add(image);
			this.GoodsIcon = U3DUtils.NEW<GIcon>();
			this.GoodsIcon.Width = 32.0;
			this.GoodsIcon.Height = 32.0;
			this.GoodsIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty), false, 0);
			this.GoodsIcon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/32_Hover.png"));
			this.GoodsIcon.TipType = 1;
			this.GoodsIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsXmlNodeByID.ID,
				0,
				-1,
				-1
			});
			this.GoodsIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			this.GoodsIcon.ItemCode = this.goodsID;
			this.GoodsIcon.ItemObject = null;
			this.GoodsIcon.BoxTypes = -1;
			this.GoodsIcon.Text = this.OrigGoodsCount.ToString();
			this.GoodsIcon.TextHorizontalAlignment = global::Layout.Right;
			this.GoodsIcon.TextVerticalAlignment = global::Layout.Bottom;
			this.GoodsIcon.TextShadowColor = 4278190080U;
			this.GoodsIcon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0));
			Canvas.SetLeft(this.GoodsIcon, 14);
			Canvas.SetTop(this.GoodsIcon, 36);
			this.Container.Children.Add(this.GoodsIcon);
			this.MaxInputCount = this.OrigGoodsCount;
		}
	}

	private bool ConvertSaleCount()
	{
		try
		{
			this.GoodsCount = Convert.ToInt32(this.GoodsNum.EditText);
			if (this.GoodsCount <= 0)
			{
				this.GoodsCount = 0;
			}
			if (this.GoodsCount > this.MaxInputCount)
			{
				this.GoodsCount = this.MaxInputCount;
			}
			if (this.GoodsCount > 0)
			{
				return true;
			}
			return false;
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
	}

	private void GoodsNum_TextChanged(object sender, object e)
	{
		int num = 0;
		try
		{
			num = Convert.ToInt32(this.GoodsNum.EditText);
			if (num >= this.MaxInputCount)
			{
				num = this.MaxInputCount;
			}
		}
		catch (Exception ex)
		{
			num = 0;
			MUDebug.LogException(ex);
		}
		if (num >= 0 && string.Empty != this.GoodsNum.EditText)
		{
			this.GoodsNum.EditText = num.ToString();
		}
		else
		{
			num = 0;
			this.GoodsNum.EditText = string.Empty;
		}
		this.GoodsCount = num;
	}

	public string GoodsPrice = string.Empty;

	private GTextBlockOutLine NameBlockText;

	private GTextBlockOutLine NumBlockText;

	private GIcon GoodsIcon;

	private bool ToAddNum = true;

	private GTextBlock GoodsNum;

	public int GoodsCount = 1;

	public string ButtonState = "Cancel";

	private Canvas Root;

	private Canvas bak = new Canvas();

	private int _MaxInputCount;

	private int _goodsID;

	private int _OrigGoodsCount;

	public EventHandler ButtonClick;
}
