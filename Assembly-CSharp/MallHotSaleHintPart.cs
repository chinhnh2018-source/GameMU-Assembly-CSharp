using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class MallHotSaleHintPart : UserControl
{
	public MallHotSaleHintPart()
	{
		this.ConsumeYBText.Text = Global.GetLang("总将花费钻石：");
		this.AllYBText.Text = Global.GetLang("帐户剩余钻石：");
		this.des1.TextWrapping = TextWrapping.Wrap;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.ScrollViewer1);
		this.ScrollViewer1.Width = 246.0;
		this.ScrollViewer1.Height = 80.0;
		Canvas.SetLeft(this.ScrollViewer1, 22);
		Canvas.SetTop(this.ScrollViewer1, 91);
		this.ScrollViewer1.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		this.ScrollViewer1.Viewer = this.Wrapper;
		this.Wrapper.Width = 246.0;
		this.Container.Children.Add(this.ConsumeYBText);
		this.ConsumeYBText.Text = Global.GetLang("总将花费钻石：");
		Canvas.SetLeft(this.ConsumeYBText, 32);
		Canvas.SetTop(this.ConsumeYBText, 216);
		this.ConsumeYBText.FontSize = HSTextField.defaultFontSize;
		this.ConsumeYBText.TextColor = new SolidColorBrush(7448500U);
		this.Container.Children.Add(this.TotalPrice);
		this.TotalPrice.Text = "0";
		Canvas.SetLeft(this.TotalPrice, 119);
		Canvas.SetTop(this.TotalPrice, 216);
		this.TotalPrice.FontSize = HSTextField.defaultFontSize;
		this.TotalPrice.TextColor = new SolidColorBrush(16764416U);
		this.Container.Children.Add(this.AllYBText);
		this.AllYBText.Text = Global.GetLang("帐户剩余钻石：");
		Canvas.SetLeft(this.AllYBText, 32);
		Canvas.SetTop(this.AllYBText, 239);
		this.AllYBText.FontSize = HSTextField.defaultFontSize;
		this.AllYBText.TextColor = new SolidColorBrush(7448500U);
		this.Container.Children.Add(this.LeftMoney);
		this.LeftMoney.Text = "0";
		Canvas.SetLeft(this.LeftMoney, 119);
		Canvas.SetTop(this.LeftMoney, 239);
		this.LeftMoney.FontSize = HSTextField.defaultFontSize;
		this.LeftMoney.TextColor = new SolidColorBrush(4282827325U);
		this.des1.BodyWidth = 211.0;
		this.des1.TextLineHeight = 3.0;
		this.Container.Children.Add(this.HintZhenQiText);
		this.HintZhenQiText.Text = Global.GetLang(string.Empty);
		Canvas.SetLeft(this.HintZhenQiText, 10);
		Canvas.SetTop(this.HintZhenQiText, 273);
		this.HintZhenQiText.FontSize = HSTextField.defaultFontSize;
		this.HintZhenQiText.TextColor = new SolidColorBrush(16754612U);
	}

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public int MallGoodsID
	{
		get
		{
			return this._MallGoodsID;
		}
		set
		{
			this._MallGoodsID = value;
		}
	}

	public int BuyGoodsID
	{
		get
		{
			return this._BuyGoodsID;
		}
		set
		{
			this._BuyGoodsID = value;
		}
	}

	public int SinglePrice
	{
		get
		{
			return this._SinglePrice;
		}
		set
		{
			this._SinglePrice = value;
		}
	}

	public bool YinLiangBuy
	{
		get
		{
			return this._YinLiangBuy;
		}
		set
		{
			this._YinLiangBuy = value;
		}
	}

	public int MallTabID
	{
		get
		{
			return this._MallTabID;
		}
		set
		{
			this._MallTabID = value;
		}
	}

	public int SinglePurchase
	{
		get
		{
			return this._SinglePurchase;
		}
		set
		{
			this._SinglePurchase = value;
		}
	}

	public int FullPurchase
	{
		get
		{
			return this._FullPurchase;
		}
		set
		{
			this._FullPurchase = value;
		}
	}

	public bool ReadonlyNum
	{
		set
		{
			this._ReadonlyNum = value;
		}
	}

	public bool Disableadd
	{
		set
		{
			this._Disableadd = value;
		}
	}

	public int GetBuyNum()
	{
		int num = 0;
		try
		{
			num = Convert.ToInt32(this.GoodsNum.EditText);
			if ((long)this.BuyGoodsID == ConfigSystemParam.GetSystemParamIntByName("YinPiaoGoodsID"))
			{
				if (num > 9999)
				{
					num = 9999;
				}
			}
			else if (num > 999)
			{
				num = 999;
			}
			if (num < 1)
			{
				num = 0;
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
			num = 0;
		}
		return num;
	}

	public void RefreshUserMoney()
	{
		if (this.MallTabID == 600)
		{
			this.LeftMoney.Text = StringUtil.substitute("{0}", new object[]
			{
				Global.Data.roleData.YinLiang - this.GetBuyNum() * this.SinglePrice
			});
		}
		else if (this.MallTabID == 10000)
		{
			int goodsID = (int)ConfigSystemParam.GetSystemParamIntByName("HuanYingZhenQiGoodsID");
			this.LeftMoney.Text = StringUtil.substitute("{0}", new object[]
			{
				Global.GetTotalGoodsCountByID(goodsID) - this.GetBuyNum() * this.SinglePrice
			});
		}
		else
		{
			this.LeftMoney.Text = StringUtil.substitute("{0}", new object[]
			{
				Global.Data.roleData.UserMoney - this.GetBuyNum() * this.SinglePrice
			});
		}
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
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.BuyGoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		if (this.MallTabID == 600)
		{
			this.AllYBText.Text = Global.GetLang("帐户剩余金币：");
			this.ConsumeYBText.Text = Global.GetLang("总将花费金币：");
			this.TotalPrice.Text = "0";
			this.LeftMoney.Text = "0";
		}
		else if (this.MallTabID == 10000)
		{
			this.AllYBText.Text = Global.GetLang("背包剩余阵旗：");
			this.ConsumeYBText.Text = Global.GetLang("需要阵旗数量：");
			this.TotalPrice.Text = "0";
			this.LeftMoney.Text = "0";
			this.HintZhenQiText.Text = StringUtil.substitute(Global.GetLang("单人每日限购{0}个"), new object[]
			{
				this.SinglePurchase
			});
		}
		this.GoodsNum = U3DUtils.NEW<GTextBlock>();
		this.GoodsNum.BodyWidth = 35.0;
		this.GoodsNum.BodyHeight = 21.0;
		this.GoodsNum.EditText = "1";
		this.GoodsNum.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 35.0, 21.0, 3.0, 2.0));
		this.GoodsNum.Onlydouble = true;
		this.GoodsNum.Text.border = false;
		this.GoodsNum.Text.TextBoxChanged = new EventHandler(this.GoodsNum_TextChanged);
		this.GoodsNum.Text.mouseEnabled = this._ReadonlyNum;
		Canvas.SetLeft(this.GoodsNum, 152);
		Canvas.SetTop(this.GoodsNum, 188);
		this.Container.Children.Add(this.GoodsNum);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 48.0;
		gicon.Height = 48.0;
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/64_Hover.png"));
		gicon.TipType = 1;
		gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
		{
			this.BuyGoodsID,
			0,
			-1,
			-1
		});
		gicon.ItemCategory = 0;
		gicon.ItemCode = this.BuyGoodsID;
		gicon.DisableHandCursor = true;
		Super.GetGoods64x64ImageFromFile(goodsXmlNodeByID.IconCode, gicon);
		Canvas.SetLeft(gicon, 42);
		Canvas.SetTop(gicon, 22);
		this.Container.Children.Add(gicon);
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.TextSize = 12.0;
		gtextBlockOutLine.Text = goodsXmlNodeByID.Title;
		gtextBlockOutLine.TextColor = new SolidColorBrush(65280U);
		Canvas.SetLeft(gtextBlockOutLine, 116);
		Canvas.SetTop(gtextBlockOutLine, 27);
		this.Container.Children.Add(gtextBlockOutLine);
		gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.Text = this.SinglePrice.ToString();
		gtextBlockOutLine.FontSize = 12;
		gtextBlockOutLine.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 206, 0));
		Canvas.SetLeft(gtextBlockOutLine, 150);
		Canvas.SetTop(gtextBlockOutLine, 52);
		this.Container.Children.Add(gtextBlockOutLine);
		this.des1.Text = goodsXmlNodeByID.Description;
		this.des1.FontSize = 12;
		this.des1.TextColor = new SolidColorBrush(46850U);
		Canvas.SetLeft(this.des1, 5);
		Canvas.SetTop(this.des1, 5);
		this.Wrapper.Children.Add(this.des1);
		this.Wrapper.Height = this.des1.RealSize.Height + 20.0;
		this.TotalPrice.Text = StringUtil.substitute("{0}", new object[]
		{
			this.SinglePrice
		});
		this.RefreshUserMoney();
		GIcon gicon2 = U3DUtils.NEW<GIcon>();
		gicon2.Width = 17.0;
		gicon2.Height = 16.0;
		gicon2.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/SubNum_Normal.png"));
		gicon2.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/sub.png"));
		gicon2.EnableIcon = this._Disableadd;
		Canvas.SetLeft(gicon2, 129);
		Canvas.SetTop(gicon2, 189);
		this.Container.Children.Add(gicon2);
		gicon2.MouseLeftButtonDown = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			int num = 1;
			try
			{
				num = Convert.ToInt32(this.GoodsNum.Text.Text);
				if (num > 1)
				{
					num--;
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
				num = 1;
			}
			this.GoodsNum.Text.Text = num.ToString();
			this.TotalPrice.Text = StringUtil.substitute("{0}", new object[]
			{
				this.SinglePrice * num
			});
			this.RefreshUserMoney();
		};
		GIcon gicon3 = U3DUtils.NEW<GIcon>();
		gicon3.Width = 17.0;
		gicon3.Height = 16.0;
		gicon3.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/AddNum_Normal.png"));
		gicon3.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/add.png"));
		gicon3.EnableIcon = this._Disableadd;
		Canvas.SetLeft(gicon3, 193);
		Canvas.SetTop(gicon3, 189);
		this.Container.Children.Add(gicon3);
		gicon3.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!(s as GIcon).EnableIcon)
			{
				return;
			}
			int num = 1;
			try
			{
				num = Convert.ToInt32(this.GoodsNum.Text.Text);
				if ((long)this.BuyGoodsID == ConfigSystemParam.GetSystemParamIntByName("YinPiaoGoodsID"))
				{
					if (num < 9999)
					{
						num++;
					}
				}
				else if (num < 999)
				{
					num++;
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
				num = 1;
			}
			this.GoodsNum.Text.Text = num.ToString();
			this.TotalPrice.Text = StringUtil.substitute("{0}", new object[]
			{
				this.SinglePrice * num
			});
			this.RefreshUserMoney();
		};
		this.AutoUseGoldCheckBox = new GCheckBox();
		this.AutoUseGoldCheckBox.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_cancel.png"));
		this.AutoUseGoldCheckBox.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/checkbox_ok.png"));
		this.AutoUseGoldCheckBox.Text = Global.GetLang("使用绑定钻石代替钻石");
		this.AutoUseGoldCheckBox.Check = true;
		this.AutoUseGoldCheckBox.TextColor = new SolidColorBrush(7448500U);
		Canvas.SetLeft(this.AutoUseGoldCheckBox, 10);
		Canvas.SetTop(this.AutoUseGoldCheckBox, 282);
		if (this.MallTabID != 10000)
		{
			this.Container.Children.Add(this.AutoUseGoldCheckBox);
		}
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("购 买");
		Canvas.SetLeft(gicon, 133);
		Canvas.SetTop(gicon, 276);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!Global.CanAddGoods(this.BuyGoodsID, 1, 0, "1900-01-01 12:00:00", true))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再购买物品..."), new object[0]), 1, -1, -1, 0);
				return;
			}
			if (this.MallTabID == 600)
			{
				if (Global.Data.roleData.YinLiang - this.GetBuyNum() * this.SinglePrice < 0)
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
					return;
				}
			}
			else if (this.MallTabID == 10000)
			{
				int goodsID = (int)ConfigSystemParam.GetSystemParamIntByName("HuanYingZhenQiGoodsID");
				int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
				if (totalGoodsCountByID - this.GetBuyNum() * this.SinglePrice < 0)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("您背包总的阵旗数量不足，无法进行购买"), 0, -1, -1, 0);
					return;
				}
			}
			else if (!this.AutoUseGoldCheckBox.Check)
			{
				if (Global.Data.roleData.UserMoney - this.GetBuyNum() * this.SinglePrice < 0)
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
					return;
				}
			}
			else if (Global.Data.roleData.UserMoney + Global.Data.roleData.Gold - this.GetBuyNum() * this.SinglePrice < 0)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
				return;
			}
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 1,
					ID = 0,
					AutoUseGold = this.AutoUseGoldCheckBox.Check
				});
			}
		};
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("取 消");
		Canvas.SetLeft(gicon, 210);
		Canvas.SetTop(gicon, 276);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 0,
					ID = 0
				});
			}
		};
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.Text = Global.GetLang("充 值");
		Canvas.SetLeft(gicon, 210);
		Canvas.SetTop(gicon, 210);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.OpenChongZhiHtmlWindow();
		};
	}

	private void GoodsNum_TextChanged(object sender, object e)
	{
		int num = -1;
		try
		{
			num = Convert.ToInt32(this.GoodsNum.EditText);
			if ((long)this.BuyGoodsID == ConfigSystemParam.GetSystemParamIntByName("YinPiaoGoodsID"))
			{
				if (num >= 9999)
				{
					num = 9999;
				}
			}
			else if (num >= 999)
			{
				num = 999;
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
			num = -1;
		}
		if (num >= 0 && string.Empty != this.GoodsNum.EditText)
		{
			this.GoodsNum.EditText = num.ToString();
		}
		else
		{
			this.GoodsNum.EditText = string.Empty;
			num = 0;
		}
		this.TotalPrice.Text = StringUtil.substitute("{0}", new object[]
		{
			this.SinglePrice * num
		});
		this.RefreshUserMoney();
	}

	public GScrollView ScrollViewer1 = new GScrollView(0, 0, 0);

	public Canvas Wrapper = new Canvas();

	public GTextBlockOutLine ConsumeYBText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine TotalPrice = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine AllYBText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine LeftMoney = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GTextBlockOutLine des1 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public DPSelectedItemEventHandler DPSelectedItem;

	public GTextBlockOutLine HintZhenQiText = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GCheckBox AutoUseGoldCheckBox;

	private GTextBlock GoodsNum;

	private int _MallGoodsID;

	private int _BuyGoodsID;

	private int _SinglePrice;

	private bool _YinLiangBuy;

	private int _MallTabID;

	private int _SinglePurchase;

	private int _FullPurchase;

	private bool _ReadonlyNum = true;

	private bool _Disableadd = true;
}
