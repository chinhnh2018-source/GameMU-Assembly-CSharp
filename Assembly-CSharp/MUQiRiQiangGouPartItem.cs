using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class MUQiRiQiangGouPartItem : UserControl
{
	public new string Name
	{
		get
		{
			return this.name;
		}
		set
		{
			this.name = value;
			this.xiangouLabel.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"FCA000",
				this.name
			});
		}
	}

	public int leftNum
	{
		get
		{
			return this.leftnum;
		}
		set
		{
			this.leftnum = value;
			this.totalNum -= this.leftnum;
			if (this.totalNum <= 0)
			{
				this.totalNum = 0;
			}
			this.shengyuLabel.Text = this.totalNum.ToString();
			if (this.totalNum <= 0)
			{
				this.IsCanBuy = false;
			}
			else
			{
				this.IsCanBuy = true;
			}
		}
	}

	public int TotalNum
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

	public int oldprice
	{
		get
		{
			return this._oldprice;
		}
		set
		{
			this._oldprice = value;
		}
	}

	public int price
	{
		get
		{
			return this._price;
		}
		set
		{
			this._price = value;
			this.priceLabel.Text = this._price.ToString();
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnBuy.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.BuyNowClick);
	}

	private void InitTextInPrefabs()
	{
		this.bak.URL = "NetImages/GameRes/Images/QirihuodongPicture/ditu.png";
		this.btnBuy.Text = Global.GetLang("购买");
		try
		{
			this.originalCostLab.text = Global.GetLang("原价:");
			this.nowlCostLab.text = Global.GetLang("现价:");
			this.remainNumLab.text = Global.GetLang("剩余数量:");
			this.xiangouLabel.Pivot = 3;
			this.xiangouLabel.MaxWidth = 170.0;
			this.xiangouLabel.X = -95.0;
		}
		catch
		{
			MUDebug.Log<string>(new string[]
			{
				"越南东南亚英文，预制报空"
			});
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

	public string Goods
	{
		get
		{
			return this.goods;
		}
		set
		{
			this.goods = value;
			string[] array = this.goods.Split(new char[]
			{
				','
			});
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(this.goods);
			this.MallGoodsID = int.Parse(array[0]);
			this.GoodsDataInfo = dummyGoodsData;
			this.InitGoodsIcon();
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
		this.goodIcon.BackSpriteName0 = "bagGrid4_bak";
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

	protected override void OnDestroy()
	{
		if (this.messageBoxWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
			base.OnDestroy();
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
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("您的钻石数量不足，无法进行购买!"), 0, -1, -1, 0);
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			return;
		}
		if (this.messageBoxWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
		}
		this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("需要消耗{0}钻石，确定吗？"), this.priceLabel.text.SafeToInt32(0))
		}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
		this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
			Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
			if (messageBoxReturn == 0)
			{
				GameInstance.Game.SendBuyInfo(this._itemID, 1);
			}
			return true;
		};
	}

	public bool IsCanBuy
	{
		get
		{
			return this.isCanBuy;
		}
		set
		{
			this.isCanBuy = value;
			if (this.isCanBuy)
			{
				this.btnBuy.isEnabled = true;
			}
			else
			{
				this.btnBuy.isEnabled = false;
			}
		}
	}

	public ShowNetImage bak;

	public TextBlock priceLabel;

	public TextBlock shengyuLabel;

	public TextBlock xiangouLabel;

	public GButton btnBuy;

	public UISprite noBuy;

	public GGoodIcon goodIcon;

	public UILabel originalCostLab;

	public UILabel nowlCostLab;

	public UILabel remainNumLab;

	private string name;

	private int leftnum;

	private int totalNum;

	private int _oldprice;

	private int _price;

	private int _itemID;

	private string goods = string.Empty;

	private int _MallGoodsID;

	private GoodsData _goodsDataInfo;

	private GChildWindow messageBoxWindow;

	private bool isCanBuy = true;
}
