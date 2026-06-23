using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class StallItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitElement();
	}

	public bool isEmpty
	{
		set
		{
			if (value)
			{
				this.panel.gameObject.SetActive(false);
				this.Bak.spriteName = "npcMallItem_bak2";
			}
		}
	}

	public int SaleNum
	{
		get
		{
			return this.m_nSaleNum;
		}
		set
		{
			this.m_nSaleNum = value;
		}
	}

	public int SalePrice
	{
		get
		{
			return this.m_nSalePrice;
		}
		set
		{
			this.m_nSalePrice = value;
			this.m_LblPrice.text = Convert.ToString(value);
		}
	}

	private void InitElement()
	{
	}

	public GoodsData goodsdata
	{
		get
		{
			return null;
		}
		set
		{
			GGoodIcon goodsItemIcon = this.GetGoodsItemIcon(value, false);
			if (null != goodsItemIcon && null != this.m_GameObjIcon)
			{
				U3DUtils.AddChild(this.m_GameObjIcon.gameObject, goodsItemIcon.gameObject, true);
				this.m_GoodsData = value;
				this.m_GoodsIcon = goodsItemIcon;
				if (this.m_GoodsData.GoodsID == 50200)
				{
					this.m_LblName.text = string.Format(Global.GetLang("{0}金币"), this.m_GoodsData.Quality);
					this.m_LblName.color = NGUIMath.HexToColorEx(16644061U);
				}
				else
				{
					this.m_LblName.text = this.GetGoodsColor(value);
				}
				if (0 < this.m_GoodsData.SaleMoney1)
				{
					this.m_SprJinBi.gameObject.SetActive(true);
					this.m_SprZuanShi.gameObject.SetActive(false);
					this.SalePrice = this.m_GoodsData.SaleMoney1;
				}
				else if (0 < this.m_GoodsData.SaleYuanBao)
				{
					this.m_SprZuanShi.gameObject.SetActive(true);
					this.m_SprJinBi.gameObject.SetActive(false);
					this.SalePrice = this.m_GoodsData.SaleYuanBao;
				}
			}
		}
	}

	public GGoodIcon icon
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	private GGoodIcon GetGoodsItemIcon(GoodsData goodsData, bool isDrag = false)
	{
		if (goodsData == null)
		{
			return null;
		}
		GGoodIcon ggoodIcon;
		if (goodsData != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.GoodsID = goodsData.GoodsID;
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.ItemCategory = categoriy;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			ggoodIcon.Tip = Global.GetGoodsNameByID(goodsData.GoodsID, false);
			bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
			if (isDrag)
			{
				ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			}
			if (ggoodIcon.GetComponent<UIPanel>() != null)
			{
				Object.Destroy(ggoodIcon.GetComponent<UIPanel>());
			}
		}
		else
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
		}
		ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
		};
		return ggoodIcon;
	}

	public string GetGoodsColor(GoodsData goodsData)
	{
		int categoriyByGoodsID = Global.GetCategoriyByGoodsID(this.m_GoodsData.GoodsID);
		string result = string.Empty;
		if (categoriyByGoodsID >= 0 && categoriyByGoodsID < 25)
		{
			result = this.GetZhuangBeiNameColor(this.m_GoodsData);
		}
		else
		{
			result = this.GetXiaoHaoWuPinClolor(this.m_GoodsData);
		}
		return result;
	}

	private string GetZhuangBeiNameColor(GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		string text = "FFFFFF";
		string text2 = string.Empty;
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
		return Global.GetColorStringForNGUIText(new object[]
		{
			text,
			text2
		});
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

	private void ShowGoodsTip(object icon)
	{
		GGoodIcon ggoodIcon = icon as GGoodIcon;
		if (null == ggoodIcon)
		{
			return;
		}
		string text = Convert.ToString(ggoodIcon.GoodsID);
		if (string.Empty == text)
		{
			return;
		}
		int num = Convert.ToInt32(text);
		if (1 < num)
		{
			GTipServiceEx.SelfBagOnly = false;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.OtherOnSale, this.m_GoodsData);
		}
	}

	public GGoodIcon m_GoodsIcon;

	public UISprite Bak;

	public GameObject m_GameObjIcon;

	public GoodsData m_GoodsData;

	public UILabel m_LblName;

	public UILabel m_LblPrice;

	public int m_nSaleNum;

	public int m_nSalePrice;

	public GameObject panel;

	public UISprite m_SprJinBi;

	public UISprite m_SprZuanShi;
}
