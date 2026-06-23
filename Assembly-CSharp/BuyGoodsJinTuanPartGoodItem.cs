using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class BuyGoodsJinTuanPartGoodItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		UIEventListener.Get(this.m_BakSprite.gameObject).onClick = delegate(GameObject s)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					Data = this.SaleData
				});
			}
		};
	}

	public void RefreshByData(AuctionItemS2C saleData)
	{
		this.SaleData = saleData;
		GoodsData goods = saleData.Goods;
		this.m_TextName.text = BuyGoodsJinTuanPartGoodItem.GetGoodsColor(goods);
		this.RemainTime = saleData.LastTime;
		this.m_TextTime.text = UIHelper.FormatSecs2(this.RemainTime, "00:00:00");
		this.m_TextPrice.text = saleData.MaxPrice.ToString();
		this.m_TextEachPrice.text = saleData.Price.ToString();
		if (saleData.Goods.Binding == 1)
		{
			this.m_TextBind.Label.color = NGUIMath.HexToColorEx(16711680U);
			this.m_TextBind.Text = Global.GetLang("绑定");
		}
		else
		{
			this.m_TextBind.Label.color = NGUIMath.HexToColorEx(65280U);
			this.m_TextBind.Text = Global.GetLang("不绑定");
		}
		if (saleData.BuyRoleId == Global.Data.RoleID)
		{
			this.m_LabelMyOfferPrice.text = Global.GetLang("我的出价");
		}
		else
		{
			this.m_LabelMyOfferPrice.text = string.Empty;
		}
		this.AddGoodIcon(goods);
	}

	public void SubRemainTime()
	{
		if (this.RemainTime > 0L)
		{
			this.RemainTime -= 1L;
			this.m_TextTime.text = UIHelper.FormatSecs2(this.RemainTime, "00:00:00");
		}
		else if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 2
			});
		}
	}

	public void AddGoodIcon(GoodsData gd)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
		icon.Width = 78.0;
		icon.Height = 78.0;
		icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		icon.TipType = 1;
		icon.ItemCode = gd.GoodsID;
		icon.ItemObject = gd;
		icon.BoxTypes = 0;
		icon.TextSize = 16;
		icon.TextShadowColor = 4278190080U;
		icon.Tag = gd.ExcellenceInfo;
		icon.BackSpriteName0 = "bagGrid4_bak";
		icon.Width = 78.0;
		icon.Height = 78.0;
		icon.GoodImg.URL = StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
		{
			Super.GetIconCode(gd.GoodsID)
		});
		icon.ItemCode = gd.GoodsID;
		icon.ItemObject = gd;
		icon.ItemCategory = Global.GetCategoriyByGoodsID(gd.GoodsID);
		bool canUseGoods = Global.CanUseGoods(gd.GoodsID, false, true);
		U3DUtils.AddChild(this.m_GoodIconContainer, icon.gameObject, true);
		Super.InitGoodsGIcon(icon, gd, canUseGoods, IconTextTypes.Qianghua);
		icon.NoUseSprite.gameObject.SetActive(false);
		Global.SetEquipGoodsZhanLiStat(icon, gd);
		icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.None, gd);
			GTipServiceEx.RefreshEquipNoUse(canUseGoods);
		};
	}

	public static string GetGoodsColor(GoodsData goodsData)
	{
		int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
		string result = string.Empty;
		if ((categoriyByGoodsID >= 0 && categoriyByGoodsID < 25) || Global.IsRebornEquip(categoriyByGoodsID))
		{
			result = BuyGoodsJinTuanPartGoodItem.GetZhuangBeiNameColor(goodsData);
		}
		else
		{
			result = BuyGoodsJinTuanPartGoodItem.GetXiaoHaoWuPinClolor(goodsData);
		}
		return result;
	}

	private static string GetZhuangBeiNameColor(GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		string text = "FFFFFF";
		string text2 = string.Empty;
		string text3 = string.Empty;
		int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
		if (goodsXmlNodeByID.Categoriy == 10 || goodsXmlNodeByID.Categoriy == 9)
		{
			if (zhuoyueAttributeCount != 0)
			{
				text = "FF08FF";
			}
			else if (goodsXmlNodeByID.SuitID == 1)
			{
				text = "0099FF";
			}
			else
			{
				text = "FF08FF";
			}
		}
		else if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
		{
			text = "00FF00";
		}
		else if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
		{
			text = "0099FF";
		}
		else if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
		{
			text = "FF08FF";
		}
		text2 += goodsXmlNodeByID.Title;
		if (goodsXmlNodeByID.Categoriy != 9 && goodsXmlNodeByID.Categoriy != 10 && goodsXmlNodeByID.Categoriy != 23 && goodsXmlNodeByID.Categoriy != 24)
		{
			if (goodsData.Forge_level > 0)
			{
				text3 += string.Format("+{0}", goodsData.Forge_level);
			}
			if (goodsData.AppendPropLev > 0)
			{
				text3 += string.Format(Global.GetLang("追{0}"), goodsData.AppendPropLev);
			}
		}
		text2 = Global.GetColorStringForNGUIText(new object[]
		{
			text,
			text2
		});
		return text2 + "\r\n" + text3;
	}

	private static string GetXiaoHaoWuPinClolor(GoodsData goodsData)
	{
		string empty = string.Empty;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		return Global.GetColorStringForNGUIText(new object[]
		{
			goodsXmlNodeByID.GoodsColor,
			goodsXmlNodeByID.Title
		});
	}

	public GTextBlockOutLine m_TextName;

	public GTextBlockOutLine m_TextBind;

	public GTextBlockOutLine m_TextTime;

	public GTextBlockOutLine m_TextPrice;

	public GTextBlockOutLine m_TextEachPrice;

	public UILabel m_LabelMyOfferPrice;

	public GameObject m_GoodIconContainer;

	public UISprite m_UnitDiamondSprite;

	public UISprite m_UnitDiamondEachSprite;

	public UISprite m_BakSprite;

	public DPSelectedItemEventHandler DPSelectedItem;

	public int m_nEventID;

	public int ID = -1;

	public AuctionItemS2C SaleData;

	public long RemainTime;
}
