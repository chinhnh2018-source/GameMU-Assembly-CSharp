using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class HefuQianggouItem : UserControl
{
	public string TotalNum
	{
		get
		{
			return this.totalNum;
		}
		set
		{
			this.totalNum = value;
		}
	}

	public int FreshenTime
	{
		get
		{
			return this.freshenTime;
		}
		set
		{
			this.freshenTime = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.btnBuy.Text = Global.GetLang("购买");
		this.xiangouLabel.Pivot = 3;
		this.xiangouLabel.X = -200.0;
		this.xiangouLabel.Y = 58.0;
		this.shengyuLabel.Pivot = 3;
		this.shengyuLabel.X = -200.0;
		this.shengyuLabel.Y = 26.0;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnBuy.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.BuyNowClick);
	}

	public int ItemID
	{
		get
		{
			return this._itemID;
		}
		set
		{
			this._itemID = value;
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
			int goodsIconCodeByID = Global.GetGoodsIconCodeByID(value);
			this.goodIcon.GoodImg.ImageURL = "NetImages/GameRes/Images/Goods/" + goodsIconCodeByID + ".png";
			this.goodIcon.GoodImg.ForceShow();
		}
	}

	private void BuyNowClick(object sender, MouseEvent e)
	{
		if (!Global.CanAddGoods(this.MallGoodsID, 1, 0, "1900-01-01 12:00:00", true))
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再购买物品..."), new object[0]), 1, -1, -1, 0);
			return;
		}
		if (Global.Data.roleData.UserMoney - this.priceLabel.text.SafeToInt32(0) < 0)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			return;
		}
		if (int.Parse(this.xiangouLabel.text) <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("物品限购数量为0，无法进行购买!"), 0, -1, -1, 0);
			return;
		}
		GameInstance.Game.SpriteMallQiangGouBuy(this._itemID, 1, false, this.MallGoodsID);
	}

	public GoodsData GoodsDataInfo
	{
		get
		{
			return this._goodsDataInfo;
		}
		set
		{
			this._goodsDataInfo = value;
		}
	}

	public void InitGoodsIcon()
	{
		this.goodIcon.Width = 64.0;
		this.goodIcon.Height = 64.0;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.MallGoodsID);
		this.goodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
		this.goodIcon.ItemObject = this.GoodsDataInfo;
		bool canUse = Global.CanUseGoods(this.MallGoodsID, false, true);
		Super.InitGoodsGIcon(this.goodIcon, this.GoodsDataInfo, canUse, IconTextTypes.Qianghua);
		this.goodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	public TextBlock origPriceLabel;

	public TextBlock priceLabel;

	public TextBlock shengyuLabel;

	public TextBlock xiangouLabel;

	public GButton btnBuy;

	public GGoodIcon goodIcon;

	private int freshenTime;

	private string totalNum;

	private int _itemID;

	private int _MallGoodsID;

	private GoodsData _goodsDataInfo;
}
