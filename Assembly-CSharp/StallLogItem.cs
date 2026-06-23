using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class StallLogItem : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public int nJinQian
	{
		get
		{
			return 0;
		}
		set
		{
			if (null != this.m_LblHuoDeJinQian)
			{
				if (this.IsBuyedState)
				{
					this.m_LblHuoDeJinQian.text = "- " + Convert.ToString(value);
					this.m_LblHuoDeJinQian.color = NGUIMath.HexToColorEx(16711680U);
				}
				else
				{
					this.m_LblHuoDeJinQian.text = "+ " + Convert.ToString(value);
					this.m_LblHuoDeJinQian.color = NGUIMath.HexToColorEx(16777214U);
				}
			}
		}
	}

	public bool IsBuyedState
	{
		get
		{
			return this.m_IsBuyedState;
		}
		set
		{
			this.m_IsBuyedState = value;
		}
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
				this.m_LblWuPinNam.text = this.GetGoodsColor(value);
				if (0 < this.m_GoodsData.SaleMoney1)
				{
					this.m_SprJinBiIcon.gameObject.SetActive(true);
					this.m_SprZuanShiIcon.gameObject.SetActive(false);
					this.nJinQian = this.m_GoodsData.SaleMoney1;
				}
				else if (0 < this.m_GoodsData.SaleYuanBao)
				{
					this.m_SprJinBiIcon.gameObject.SetActive(false);
					this.m_SprZuanShiIcon.gameObject.SetActive(true);
					this.nJinQian = this.m_GoodsData.SaleYuanBao;
				}
			}
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
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
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
		}
		else
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.BackSpriteName0 = "bagGrid_bak";
		}
		if (ggoodIcon.GetComponent<UIPanel>() != null)
		{
			Object.Destroy(ggoodIcon.GetComponent<UIPanel>());
		}
		ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
		};
		return ggoodIcon;
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
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(num, 5, 6, 10000, 1, 0, 1, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			GTipServiceEx.SelfBagOnly = false;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, dummyGoodsDataMu);
		}
	}

	public string GetGoodsColor(GoodsData goodsData)
	{
		int categoriyByGoodsID = Global.GetCategoriyByGoodsID(this.m_GoodsData.GoodsID);
		string result = string.Empty;
		if ((categoriyByGoodsID >= 0 && categoriyByGoodsID < 25) || Global.IsRebornEquip(categoriyByGoodsID))
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

	public UILabel m_LblWuPinNam;

	public UILabel m_LblHuoDeJinQian;

	public UILabel m_LblGouMaiZheName;

	public UILabel m_LblGouMaiTime;

	public GameObject m_GameObjIcon;

	public GoodsData m_GoodsData;

	private bool m_IsBuyedState;

	public UISprite m_SprJinBiIcon;

	public UISprite m_SprZuanShiIcon;
}
