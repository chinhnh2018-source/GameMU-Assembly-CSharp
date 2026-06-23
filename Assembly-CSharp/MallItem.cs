using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class MallItem : UserControl
{
	public string BodyBackground
	{
		get
		{
			return this.bak.spriteName;
		}
		set
		{
			this.bak.spriteName = value;
		}
	}

	public int nXianGouLeft
	{
		get
		{
			return this.nXianGouMax - this.nXianGouHaveBuy;
		}
	}

	public GoodsOwnerTypes goodsOwnerTypes
	{
		get
		{
			return this._goodsOwnerType;
		}
		set
		{
			this._goodsOwnerType = value;
			if (this._goodsOwnerType == GoodsOwnerTypes.WangZheShangCheng)
			{
				this.texBangzuanIcon.spriteName = "wangzhejf";
				this.ItemDiamondIcon.spriteName = "wangzhejf";
			}
			else if (this._goodsOwnerType == GoodsOwnerTypes.KuaFuPlunderJueXingShop)
			{
				this.texBangzuanIcon.spriteName = "LueDuoPoints";
				this.ItemDiamondIcon.spriteName = "LueDuoPoints";
				this.GoodsOwnerTypesEX = GoodsOwnerTypes.KuaFuPlunderJueXingShop;
			}
			else if (this._goodsOwnerType == GoodsOwnerTypes.KuaFuPlunderJueXingShop_Diamond)
			{
				this.texBangzuanIcon.spriteName = "diamond";
				this.ItemDiamondIcon.spriteName = "diamond";
				this.GoodsOwnerTypesEX = GoodsOwnerTypes.KuaFuPlunderJueXingShop_Diamond;
			}
		}
	}

	public int ItemType
	{
		get
		{
			return this._itemType;
		}
		set
		{
			this._itemType = value;
			this.RefreshLayout();
		}
	}

	private void InitTextInPrefabs()
	{
		this.btnBuyNow.Text = Global.GetLang("立即购买");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.ItemClick);
		this.btnBuyNow.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.BuyNowClick);
	}

	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
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
			this.ItemIcon.GoodImg.ImageURL = "NetImages/GameRes/Images/Goods/" + goodsIconCodeByID + ".png";
			this.ItemIcon.GoodImg.ForceShow();
		}
	}

	public int TabPageID
	{
		get
		{
			return this._tabPageID;
		}
		set
		{
			this._tabPageID = value;
		}
	}

	public string GoodsName
	{
		set
		{
			this.ItemTitle.text = value;
		}
	}

	public string GoodsInfo
	{
		get
		{
			return this.infoText;
		}
		set
		{
			this.infoText = value;
		}
	}

	public string GoodsOrgPrice
	{
		get
		{
			return this.ItemOrgPrice.text;
		}
		set
		{
			this.ItemOrgPrice.text = value;
		}
	}

	public string GoodsPrice
	{
		get
		{
			return this.ItemPrice.text;
		}
		set
		{
			this.ItemPrice.text = value;
		}
	}

	public string IconImgUrl
	{
		set
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(string.Empty + value, "NetImages/GameRes/");
		}
	}

	public string ImgBackgroundURL
	{
		set
		{
			if (value != null)
			{
				this.ItemIcon.BackURL = new ImageURL(value, false, 0);
			}
			else
			{
				this.ItemIcon.BackURL = null;
			}
		}
	}

	public int ZhenQi
	{
		get
		{
			return this._ZhenQi;
		}
		set
		{
			this._ZhenQi = value;
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
		}
	}

	public string ShengYuShu
	{
		get
		{
			return this.ItemLeftCount.text;
		}
		set
		{
			this.ItemLeftCount.text = value;
		}
	}

	public string XianGouShu
	{
		get
		{
			return this.ItemLimitCount.text;
		}
		set
		{
			this.ItemLimitCount.text = value;
		}
	}

	public bool IsBangZuan
	{
		get
		{
			return this.isBangZuan;
		}
		set
		{
			this.isBangZuan = value;
			if (this.isBangZuan)
			{
				this.texBangzuanIcon.spriteName = "moneyBindZhuanshi";
			}
			else
			{
				this.texBangzuanIcon.spriteName = "diamond";
			}
		}
	}

	public GoodsOwnerTypes GoodsOwnerTypesEX
	{
		get
		{
			return this.mGoodsOwnerTypes;
		}
		set
		{
			this.mGoodsOwnerTypes = value;
		}
	}

	private void RefreshLayout()
	{
		int itemType = this._itemType;
		if (itemType != 0)
		{
			if (itemType == 1)
			{
				BoxCollider component = base.transform.GetComponent<BoxCollider>();
				if (component != null)
				{
					component.size = new Vector3(311f, 193f, 0f);
				}
				this.ItemOrgPrice.gameObject.SetActive(true);
				this.ItemLeftCount.gameObject.SetActive(true);
				this.ItemLimitCount.gameObject.SetActive(true);
				this.btnBuyNow.gameObject.SetActive(true);
				this.ItemDiamondIcon.gameObject.SetActive(false);
				this.ItemPriceBak.gameObject.SetActive(false);
				this.texBangzuanIcon.gameObject.SetActive(false);
				this.bak.gameObject.transform.localScale = new Vector3(311f, 193f, -0.01f);
				this.ItemTitle.gameObject.transform.localPosition = new Vector3(-48f, 78f, -0.01f);
				this.ItemPrice.gameObject.transform.localPosition = new Vector3(33f, 13f, -0.01f);
				this.ItemOrgPrice.gameObject.transform.localPosition = new Vector3(33f, 49f, -0.01f);
				this.ItemLeftCount.gameObject.transform.localPosition = new Vector3(-70f, -38f, -0.01f);
				this.ItemLimitCount.gameObject.transform.localPosition = new Vector3(-70f, -69f, -0.01f);
				this.btnBuyNow.gameObject.transform.localPosition = new Vector3(85f, -56f, -0.01f);
				this.ItemIcon.gameObject.transform.localPosition = new Vector3(-101f, 39f, -0.21f);
				if (this.goodsOwnerTypes == GoodsOwnerTypes.LaoWanJiaShangCheng)
				{
					if (this.laoWanJiaObject)
					{
						this.laoWanJiaObject.SetActive(true);
						this.ItemLeftCount.gameObject.SetActive(true);
						this.ItemLimitCount.gameObject.SetActive(false);
						this.ItemLeftCount.transform.localPosition = this.laoWanJiaXianGou.transform.localPosition;
						this.laoWanJiaXianGou.gameObject.SetActive(false);
						this.ItemIcon.BackgroundSprite0.gameObject.SetActive(true);
						this.bak.gameObject.SetActive(false);
					}
				}
				else if (this.laoWanJiaObject)
				{
					this.laoWanJiaObject.SetActive(false);
				}
			}
		}
		else
		{
			this.ItemOrgPrice.gameObject.SetActive(false);
			this.ItemLeftCount.gameObject.SetActive(false);
			this.ItemLimitCount.gameObject.SetActive(false);
			this.btnBuyNow.gameObject.SetActive(false);
			this.texBangzuanIcon.gameObject.SetActive(true);
			this.bak.gameObject.transform.localScale = new Vector3(320f, 98f, -0.01f);
			this.ItemTitle.gameObject.transform.localPosition = new Vector3(-60f, 23f, -0.01f);
			this.ItemPrice.gameObject.transform.localPosition = new Vector3(-32f, -24f, -0.01f);
			this.ItemIcon.gameObject.transform.localPosition = new Vector3(-110f, -1f, -0.01f);
		}
	}

	private void ItemClick(object sender, MouseEvent e)
	{
		if (this.MouseLeftButtonUp != null)
		{
			this.MouseLeftButtonUp(this, e);
			return;
		}
		if (this.MouseLeftButtonUpEX != null)
		{
			e.IDType = this._itemID;
			this.MouseLeftButtonUpEX(this, e);
		}
		if (int.Parse(this.ItemLimitCount.text) <= 0 && this._tabPageID == 10000)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("物品限购数量为0，无法进行购买!"), 0, -1, -1, 0);
			return;
		}
		GoodsPriceUnitTypes goodsPriceUnit = GoodsPriceUnitTypes.Zhuanshi;
		if (this.IsBangZuan)
		{
			goodsPriceUnit = GoodsPriceUnitTypes.BindZhuanshi;
		}
		if (this.mGoodsOwnerTypes == GoodsOwnerTypes.KuaFuPlunderJueXingShop || this.mGoodsOwnerTypes == GoodsOwnerTypes.KuaFuPlunderJueXingShop_Diamond)
		{
			goodsPriceUnit = ((this.mGoodsOwnerTypes != GoodsOwnerTypes.KuaFuPlunderJueXingShop) ? GoodsPriceUnitTypes.Zhuanshi : GoodsPriceUnitTypes.KuaFuPlunderJueXingSorce);
			if (this.GoodsDataInfo != null)
			{
				GTipServiceEx.ShowTip(this.ItemIcon, TipTypes.GoodsText, this.mGoodsOwnerTypes, goodsPriceUnit, int.Parse(this.ItemPrice.text), this.GoodsDataInfo);
			}
			else
			{
				GTipServiceEx.ShowTip(this.ItemIcon, TipTypes.GoodsText, this.mGoodsOwnerTypes, goodsPriceUnit, int.Parse(this.ItemPrice.text), this.MallGoodsID, -1, -1, null);
			}
		}
		else if (this.TabPageID == 100000)
		{
			if (this.GoodsDataInfo != null)
			{
				GTipServiceEx.ShowTip(this.ItemIcon, TipTypes.GoodsText, GoodsOwnerTypes.QiangGou, goodsPriceUnit, int.Parse(this.ItemPrice.text), this.GoodsDataInfo);
			}
			else
			{
				GTipServiceEx.ShowTip(this.ItemIcon, TipTypes.GoodsText, GoodsOwnerTypes.QiangGou, goodsPriceUnit, int.Parse(this.ItemPrice.text), this.MallGoodsID, -1, -1, null);
			}
		}
		else if (this.GoodsDataInfo != null)
		{
			GTipServiceEx.ShowTip(this.ItemIcon, TipTypes.GoodsText, GoodsOwnerTypes.mallSale, goodsPriceUnit, int.Parse(this.ItemPrice.text), this.GoodsDataInfo);
		}
		else
		{
			GTipServiceEx.ShowTip(this.ItemIcon, TipTypes.GoodsText, GoodsOwnerTypes.mallSale, goodsPriceUnit, int.Parse(this.ItemPrice.text), this.MallGoodsID, -1, -1, null);
		}
	}

	private void BuyNowClick(object sender, MouseEvent e)
	{
		if (!Global.CanAddGoods(this.MallGoodsID, 1, 0, "1900-01-01 12:00:00", true))
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再购买物品..."), new object[0]), 1, -1, -1, 0);
			return;
		}
		if (Global.Data.roleData.UserMoney - this.ItemPrice.text.SafeToInt32(0) < 0)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			return;
		}
		if (int.Parse(this.ItemLimitCount.text) <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("物品限购数量为0，无法进行购买!"), 0, -1, -1, 0);
			return;
		}
		this.StartGouMai(this.MallGoodsID, this.ItemID, 1, this.ItemType != 0);
	}

	private void StartGouMai(int buyGoodsID, int itemID, int num, bool isXianGou = false)
	{
		if (this.OnStartGouHander == null)
		{
			if (this._tabPageID == 10000)
			{
				GameInstance.Game.SpriteMallZhenQiBuy(this._itemID, num);
			}
			else if (isXianGou)
			{
				GameInstance.Game.SpriteMallQiangGouBuy(this._itemID, num, false, buyGoodsID);
			}
			else
			{
				GameInstance.Game.SpriteMallBuy(this._itemID, num, false);
			}
		}
		else
		{
			this.OnStartGouHander(buyGoodsID, itemID, num, isXianGou);
		}
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
		this.ItemIcon.Width = 64.0;
		this.ItemIcon.Height = 64.0;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.MallGoodsID);
		this.ItemIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
		this.ItemIcon.ItemObject = this.GoodsDataInfo;
		bool canUse = Global.CanUseGoods(this.MallGoodsID, false, true);
		Super.InitGoodsGIcon(this.ItemIcon, this.GoodsDataInfo, canUse, IconTextTypes.Qianghua);
	}

	public void CreateGGoodsIcon(string[] goods)
	{
		int num = Convert.ToInt32(goods[0]);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
		GGoodIcon itemIcon = this.ItemIcon;
		if (goodsXmlNodeByID != null)
		{
			itemIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			itemIcon.Width = 64.0;
			itemIcon.Height = 64.0;
			GoodsData goodsData = new GoodsData();
			goodsData.GoodsID = num;
			goodsData.GCount = Convert.ToInt32(goods[1]);
			goodsData.Binding = Convert.ToInt32(goods[2]);
			goodsData.Forge_level = Convert.ToInt32(goods[3]);
			goodsData.AppendPropLev = Convert.ToInt32(goods[4]);
			goodsData.Lucky = Convert.ToInt32(goods[5]);
			goodsData.ExcellenceInfo = Convert.ToInt32(goods[6]);
			itemIcon.ItemObject = goodsData;
			itemIcon.ItemCode = num;
			itemIcon.TipType = 1;
			itemIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			itemIcon.SecondText.text = goodsData.GCount.ToString();
			itemIcon.SecondText.gameObject.SetActive(true);
			itemIcon.TextShadowColor = 4278190080U;
			itemIcon.TextHorizontalAlignment = global::Layout.Right;
			itemIcon.TextVerticalAlignment = global::Layout.Bottom;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			itemIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			U3DUtils.AddChild(base.gameObject, itemIcon.gameObject, true);
			Super.InitGoodsGIcon(itemIcon, goodsData, Global.CanUseGoods(num, false, true), IconTextTypes.Qianghua);
		}
	}

	private const float TYPE_NORMAL_BACKGROUND_WIDTH = 320f;

	private const float TYPE_NORMAL_BACKGROUND_HEIGHT = 98f;

	private const float TYPE_NORMAL_TITLE_POS_X = -60f;

	private const float TYPE_NORMAL_TITLE_POS_y = 23f;

	private const float TYPE_NORMAL_PRICE_POS_X = -32f;

	private const float TYPE_NORMAL_PRICE_POS_y = -24f;

	private const float TYPE_NORMAL_GOODSICON_POS_X = -110f;

	private const float TYPE_NORMAL_GOODSICON_POS_Y = -1f;

	private const float TYPE_ONSALE_BACKGROUND_WIDTH = 311f;

	private const float TYPE_ONSALE_BACKGROUND_HEIGHT = 193f;

	private const float TYPE_ONSALE_TITLE_POS_X = -48f;

	private const float TYPE_ONSALE_TITLE_POS_y = 78f;

	private const float TYPE_ONSALE_PRICE_POS_X = 33f;

	private const float TYPE_ONSALE_PRICE_POS_y = 13f;

	private const float TYPE_ONSALE_ORG_PRICE_POS_X = 33f;

	private const float TYPE_ONSALE_ORG_PRICE_POS_y = 49f;

	private const float TYPE_ONSALE_LEFT_COUNT_POS_X = -70f;

	private const float TYPE_ONSALE_LEFT_COUNT_POS_y = -38f;

	private const float TYPE_ONSALE_LIMIT_COUNT_POS_X = -70f;

	private const float TYPE_ONSALE_LIMIT_COUNT_POS_y = -69f;

	private const float TYPE_ONSALE_GOODSICON_POS_X = -101f;

	private const float TYPE_ONSALE_GOODSICON_POS_Y = 39f;

	private const float TYPE_ONSALE_BUYNOW_POS_X = 85f;

	private const float TYPE_ONSALE_BUYNOW_POS_Y = -56f;

	public MouseLeftButtonUpEventHandler MouseLeftButtonUp;

	public MouseLeftButtonUpEventHandler MouseLeftButtonUpEX;

	public MallItem.StartGouMaiDelegate OnStartGouHander;

	public GGoodIcon ItemIcon;

	public UILabel ItemTitle;

	public UILabel ItemPrice;

	public UILabel ItemOrgPrice;

	public UILabel ItemLeftCount;

	public UILabel ItemLimitCount;

	public UISprite ItemDiamondIcon;

	public UISprite ItemPriceBak;

	public UISprite bak;

	public GButton btnBuyNow;

	public UISprite texBangzuanIcon;

	private bool isBangZuan;

	public GameObject wangzheXianGou;

	public UILabel wangzheXianGouLabel;

	public int nXianGouMax;

	public int nXianGouHaveBuy;

	public GameObject laoWanJiaObject;

	public UILabel laoWanJiaXianGou;

	public GoodsOwnerTypes _goodsOwnerType = GoodsOwnerTypes.mallSale;

	private int _itemType;

	private string infoText;

	private ObservableCollection _ItemCollection;

	private int _itemID;

	private int _MallGoodsID;

	private int _tabPageID;

	private int _ZhenQi;

	private int _SinglePurchase;

	private int _FullPurchase;

	private GoodsOwnerTypes mGoodsOwnerTypes = GoodsOwnerTypes.SysGifts;

	private GoodsData _goodsDataInfo;

	public delegate void StartGouMaiDelegate(int buyGoodsID, int itemID, int num, bool isXianGou);
}
