using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class BuyGoodsPartGoodItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		UIEventListener.Get(this.m_BakSprite.gameObject).onClick = delegate(GameObject s)
		{
			if (this.IsDiamond)
			{
				if (this.SaleData.SalingGoodsData.SaleYuanBao > Global.Data.roleData.UserMoney)
				{
					Super.HintMainText(Global.GetLang("钻石不足！"), 10, 3);
				}
			}
			else if (this.SaleData.SalingGoodsData.SaleMoney1 > Global.GetRoleOwnNumByMoneyType(8))
			{
				Super.HintMainText(Global.GetLang("金币不足！"), 10, 3);
			}
			GGoodIcon componentInChildren = this.m_GoodIconContainer.GetComponentInChildren<GGoodIcon>();
			GoodsData goodsData = componentInChildren.ItemObject as GoodsData;
			if (goodsData.SaleYuanBao > 0)
			{
				GTipServiceEx.ShowTip(componentInChildren, TipTypes.GoodsText, GoodsOwnerTypes.JiaoYiShuo, GoodsPriceUnitTypes.Zhuanshi, goodsData.SaleYuanBao, goodsData);
			}
			else
			{
				GTipServiceEx.ShowTip(componentInChildren, TipTypes.GoodsText, GoodsOwnerTypes.JiaoYiShuo, GoodsPriceUnitTypes.Jinbi, goodsData.SaleMoney1, goodsData);
			}
		};
	}

	public void RefreshByData(SaleGoodsData saleData)
	{
		this.SaleData = saleData;
		GoodsData salingGoodsData = saleData.SalingGoodsData;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(salingGoodsData.GoodsID);
		if (salingGoodsData.GoodsID == 50200)
		{
			string text = string.Format(Global.GetLang("{0}金币"), salingGoodsData.Quality);
			this.m_TextName.text = text;
			this.m_TextName.Label.color = NGUIMath.HexToColorEx(16644061U);
		}
		else
		{
			this.m_TextName.text = this.GetGoodsColor(salingGoodsData);
		}
		this.m_TextEachPrice.text = saleData.RoleName;
		if (goodsXmlNodeByID.SuitID <= 0)
		{
			this.m_TextLevel.text = Global.GetLang("无");
		}
		else
		{
			this.m_TextLevel.text = goodsXmlNodeByID.SuitID.ToString();
		}
		if (salingGoodsData.SaleYuanBao > 0)
		{
			this.IsDiamond = true;
			this.m_UnitGoldSprite.gameObject.SetActive(false);
			this.m_UnitDiamondSprite.gameObject.SetActive(true);
			this.m_UnitGoldEachSprite.gameObject.SetActive(false);
			this.m_UnitDiamondEachSprite.gameObject.SetActive(true);
			if (salingGoodsData.SaleYuanBao > Global.Data.roleData.UserMoney)
			{
				this.m_TextPrice.Label.color = NGUIMath.HexToColorEx(16711680U);
				this.m_TextPrice.text = salingGoodsData.SaleYuanBao.ToString();
			}
			else
			{
				this.m_TextPrice.Label.color = NGUIMath.HexToColorEx(16777214U);
				this.m_TextPrice.text = this.GetPriceColorStr(salingGoodsData.SaleYuanBao.ToString());
			}
			if (salingGoodsData.GCount > 0)
			{
				long num = (long)(salingGoodsData.SaleYuanBao / salingGoodsData.GCount);
				this.m_TextEachPrice.text = num.ToString();
			}
			else
			{
				this.m_TextEachPrice.text = salingGoodsData.SaleYuanBao.ToString();
			}
		}
		else
		{
			this.IsDiamond = false;
			this.m_UnitGoldSprite.gameObject.SetActive(true);
			this.m_UnitDiamondSprite.gameObject.SetActive(false);
			this.m_UnitGoldEachSprite.gameObject.SetActive(true);
			this.m_UnitDiamondEachSprite.gameObject.SetActive(false);
			if (salingGoodsData.SaleMoney1 > Global.GetRoleOwnNumByMoneyType(8))
			{
				this.m_TextPrice.Label.color = NGUIMath.HexToColorEx(16711680U);
				this.m_TextPrice.text = salingGoodsData.SaleMoney1.ToString();
			}
			else
			{
				this.m_TextPrice.Label.color = NGUIMath.HexToColorEx(16777214U);
				this.m_TextPrice.text = this.GetPriceColorStr(salingGoodsData.SaleMoney1.ToString());
			}
			if (salingGoodsData.GCount > 0)
			{
				long num2 = (long)(salingGoodsData.SaleMoney1 / salingGoodsData.GCount);
				this.m_TextEachPrice.text = num2.ToString();
			}
			else
			{
				this.m_TextEachPrice.text = salingGoodsData.SaleMoney1.ToString();
			}
		}
		this.AddGoodIcon(salingGoodsData);
	}

	private string GetPriceColorStr(string priceValue)
	{
		int length = priceValue.Length;
		string result = string.Empty;
		if (length <= 4)
		{
			result = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				priceValue
			});
		}
		else if (length <= 8)
		{
			result = Global.GetColorStringForNGUIText(new object[]
			{
				"00ff00",
				priceValue.Substring(0, length - 4)
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				priceValue.Substring(length - 4, 4)
			});
		}
		else if (length <= 12)
		{
			result = Global.GetColorStringForNGUIText(new object[]
			{
				"3eb9ff",
				priceValue.Substring(0, length - 8)
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"00ff00",
				priceValue.Substring(length - 8, 4)
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				priceValue.Substring(length - 4, 4)
			});
		}
		else
		{
			result = Global.GetColorStringForNGUIText(new object[]
			{
				"b266ff",
				priceValue.Substring(0, length - 12)
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"3eb9ff",
				priceValue.Substring(length - 12, 4)
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"00ff00",
				priceValue.Substring(length - 8, 4)
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				priceValue.Substring(length - 4, 4)
			});
		}
		return result;
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
		bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
		U3DUtils.AddChild(this.m_GoodIconContainer, icon.gameObject, true);
		Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
		Global.SetEquipGoodsZhanLiStat(icon, gd);
		icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.None, gd);
		};
		icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.IDType == 14)
			{
				Global.SendEvent("1600", Global.GetLang("交易所购买次数"));
				int moneyType = 0;
				int salePrice = 0;
				if (this.SaleData.SalingGoodsData.SaleMoney1 > 0)
				{
					moneyType = 8;
					salePrice = this.SaleData.SalingGoodsData.SaleMoney1;
				}
				else if (this.SaleData.SalingGoodsData.SaleYuanBao > 0)
				{
					moneyType = 40;
					salePrice = this.SaleData.SalingGoodsData.SaleYuanBao;
				}
				GameInstance.Game.SpriteMarketBuyGoods2(this.SaleData.GoodsDbID, this.SaleData.SalingGoodsData.GoodsID, moneyType, salePrice);
			}
		};
	}

	public string GetGoodsColor(GoodsData goodsData)
	{
		int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
		string result = string.Empty;
		if ((categoriyByGoodsID >= 0 && categoriyByGoodsID < 25) || Global.IsRebornEquip(categoriyByGoodsID))
		{
			result = this.GetZhuangBeiNameColor(goodsData);
		}
		else
		{
			result = this.GetXiaoHaoWuPinClolor(goodsData);
		}
		return result;
	}

	private string GetZhuangBeiNameColor(GoodsData goodsData)
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

	private string GetXiaoHaoWuPinClolor(GoodsData goodsData)
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

	public GTextBlockOutLine m_TextLevel;

	public GTextBlockOutLine m_TextPrice;

	public GTextBlockOutLine m_TextEachPrice;

	public GameObject m_GoodIconContainer;

	public GButton m_BtnBuy;

	public UISprite m_UnitGoldSprite;

	public UISprite m_UnitDiamondSprite;

	public UISprite m_UnitGoldEachSprite;

	public UISprite m_UnitDiamondEachSprite;

	public UISprite m_BakSprite;

	public DPSelectedItemEventHandler DPSelectedItem;

	public int m_nEventID;

	public int ID = -1;

	private bool IsDiamond;

	private SaleGoodsData SaleData;
}
