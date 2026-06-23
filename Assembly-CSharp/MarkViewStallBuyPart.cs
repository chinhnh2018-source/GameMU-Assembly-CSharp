using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class MarkViewStallBuyPart : UserControl
{
	public MarkViewStallBuyPart()
	{
		this.thisCtrl = this;
		this.txtGoodName.TextFontWrapping = TextWrapping.Wrap;
		this.txtGoodName.BodyWidth = 64.0;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.ScrollImg);
		this.ScrollImg.Width = 32.0;
		this.ScrollImg.Height = 32.0;
		Canvas.SetLeft(this.ScrollImg, 28);
		Canvas.SetTop(this.ScrollImg, 38);
		this.Container.Children.Add(this.txtGoodName);
		this.txtGoodName.Width = 70.0;
		this.txtGoodName.Height = 30.0;
		this.txtGoodName.TextColor = new SolidColorBrush(46850U);
		Canvas.SetLeft(this.txtGoodName, 23);
		Canvas.SetTop(this.txtGoodName, 80);
		this.Container.Children.Add(this.txtPriceGold);
		this.txtPriceGold.Text = "0";
		this.txtPriceGold.TextColor = new SolidColorBrush(16764416U);
		Canvas.SetLeft(this.txtPriceGold, 197);
		Canvas.SetTop(this.txtPriceGold, 36);
		this.Container.Children.Add(this.txtPriceIngot);
		this.txtPriceIngot.Text = "0";
		this.txtPriceIngot.TextColor = new SolidColorBrush(16764416U);
		Canvas.SetLeft(this.txtPriceIngot, 197);
		Canvas.SetTop(this.txtPriceIngot, 59);
	}

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public int OwnerRoleID
	{
		get
		{
			return this._OwnerRoleID;
		}
		set
		{
			this._OwnerRoleID = value;
		}
	}

	public int GoodsID
	{
		get
		{
			return this._GoodsID;
		}
		set
		{
			this._GoodsID = value;
		}
	}

	public int GoodDbID
	{
		get
		{
			return this._GoodDbID;
		}
		set
		{
			this._GoodDbID = value;
		}
	}

	public int GoodCount
	{
		get
		{
			return this._GoodCount;
		}
		set
		{
			this._GoodCount = value;
		}
	}

	public string SaleMoney
	{
		set
		{
			this.txtPriceGold.Text = value;
		}
	}

	public string SaleYuanBao
	{
		set
		{
			this.txtPriceIngot.Text = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 80.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 80.0, 21.0, 3.0, 2.0));
		gicon.TextColor = new SolidColorBrush(10551295U);
		gicon.Text = Global.GetLang("购买");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 0
				});
			}
		};
		Canvas.SetLeft(gicon, 173);
		Canvas.SetTop(gicon, 91);
		this.Container.Children.Add(gicon);
	}

	public void InitPartData()
	{
		if (this._GoodsID == -1)
		{
			return;
		}
		this.ShowGoodsIcon(this._GoodsID);
		this.txtGoodName.Text = Global.GetGoodsNameByID(this._GoodsID, false);
	}

	private void ShowGoodsIcon(int goodsID)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 32.0;
			ggoodIcon.Height = 32.0;
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				this._GoodDbID,
				10
			});
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = goodsID;
			ggoodIcon.ItemObject = null;
			ggoodIcon.BoxTypes = -1;
			SaleGoodsData saleGoodsDataByDbID = Super.GetSaleGoodsDataByDbID(this._GoodDbID);
			if (saleGoodsDataByDbID != null)
			{
				SaleGoodsData saleGoodsData = saleGoodsDataByDbID;
				if (saleGoodsData != null)
				{
					Super.InitGoodsGIcon(ggoodIcon, saleGoodsData.SalingGoodsData, true, IconTextTypes.Qianghua);
				}
			}
			this.ScrollImg.Children.Add(ggoodIcon);
		}
	}

	private SpriteSL thisCtrl;

	private Canvas ScrollImg = new Canvas();

	private GTextBlockOutLine txtGoodName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtPriceGold = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtPriceIngot = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public DPSelectedItemEventHandler DPSelectedItem;

	private int _GoodsID = -1;

	private int _GoodDbID = -1;

	private int _GoodCount = -1;

	private int _OwnerRoleID;
}
