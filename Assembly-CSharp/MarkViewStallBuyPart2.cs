using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class MarkViewStallBuyPart2 : UserControl
{
	public MarkViewStallBuyPart2()
	{
		this.thisCtrl = this;
	}

	protected override void InitializeComponent()
	{
		this.Container.Children.Add(this.txtGoodName);
		this.txtGoodName.TextColor = new SolidColorBrush(4278236930U);
		Canvas.SetLeft(this.txtGoodName, 25);
		Canvas.SetTop(this.txtGoodName, 80);
	}

	public int SaleMoney1
	{
		get
		{
			return this._SaleMoney1;
		}
	}

	public int SaleYinPiao
	{
		get
		{
			return this._SaleYinPiao;
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
		gicon.Text = Global.GetLang("物品上架");
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this._SaleMoney1 = Global.SafeConvertToInt32(this.txtPriceGold.Text.Text);
			this._SaleYinPiao = Global.SafeConvertToInt32(this.txtPriceYinPiao.Text.Text);
			if (this._SaleMoney1 <= 0 && this._SaleYinPiao <= 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("请至少输入一种定价"), 0, -1, -1, 0);
				return;
			}
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
		this.txtPriceGold = U3DUtils.NEW<GTextBlock>();
		this.txtPriceGold.BodyWidth = 97.0;
		this.txtPriceGold.BodyHeight = 21.0;
		this.txtPriceGold.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 97.0, 21.0, 3.0, 2.0));
		this.txtPriceGold.Onlydouble = true;
		this.txtPriceGold.Text.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtPriceGold, 172);
		Canvas.SetTop(this.txtPriceGold, 32);
		this.Container.Children.Add(this.txtPriceGold);
		this.txtPriceYinPiao = U3DUtils.NEW<GTextBlock>();
		this.txtPriceYinPiao.BodyWidth = 97.0;
		this.txtPriceYinPiao.BodyHeight = 21.0;
		this.txtPriceYinPiao.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/input21.png"), 97.0, 21.0, 3.0, 2.0));
		this.txtPriceYinPiao.Onlydouble = true;
		this.txtPriceYinPiao.Text.Foreground = new SolidColorBrush(uint.MaxValue);
		Canvas.SetLeft(this.txtPriceYinPiao, 172);
		Canvas.SetTop(this.txtPriceYinPiao, 55);
		this.Container.Children.Add(this.txtPriceYinPiao);
	}

	public void InitPartData()
	{
		if (this._GoodDbID == -1)
		{
			return;
		}
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(this._GoodDbID, null);
		this.ShowGoodsIcon(goodsDataByDbID);
		this.txtGoodName.Text = Global.GetGoodsNameByID(goodsDataByDbID.GoodsID, false);
	}

	private void ShowGoodsIcon(GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
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
				goodsData.GoodsID,
				1,
				goodsData.Id,
				0
			});
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = goodsData.GoodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.BoxTypes = -1;
			Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
			Canvas.SetLeft(ggoodIcon, 28);
			Canvas.SetTop(ggoodIcon, 38);
			this.Container.Children.Add(ggoodIcon);
		}
	}

	private UserControl thisCtrl;

	private GTextBlockOutLine txtGoodName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public DPSelectedItemEventHandler DPSelectedItem;

	private GTextBlock txtPriceGold;

	private GTextBlock txtPriceYinPiao;

	private int _GoodDbID = -1;

	private int _SaleMoney1;

	private int _SaleYinPiao;
}
