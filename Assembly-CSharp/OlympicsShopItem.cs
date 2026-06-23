using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class OlympicsShopItem : UserControl
{
	[HideInInspector]
	private void InitTextInPrefabs()
	{
		this.limitBuyLabel.Text = Global.GetLang("限购：");
		this.residual.Text = Global.GetLang("剩余");
		this.exchangeBtn.Text = Global.GetLang("兑换");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.exchangeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.IsInOlympicsAwardActivity())
			{
				Super.HintMainText(Global.GetLang("活动已结束"), 10, 3);
				return;
			}
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this.currentGoodsId
				});
			}
		};
	}

	public void SetValue(OlympicsShopData data)
	{
		this.currentShopData = data;
		if (OlympicsDataManage.ownedScore - data.Price < 0)
		{
			this.exchangeBtn.isEnabled = false;
		}
		this.limitCount.Text = (data.NumSingl - data.NumSingleBuy).ToString();
		if (data.NumFullBuy <= 0)
		{
			data.NumFullBuy = 0;
		}
		int num = data.NumFull - data.NumFullBuy;
		if (num <= 0)
		{
			this.shopResidualCount.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				num.ToString()
			});
			this.exchangeBtn.isEnabled = false;
		}
		else
		{
			this.shopResidualCount.Text = num.ToString();
		}
		this.price.Text = data.Price.ToString();
		this.currentGoodsId = data.ID;
		if (this.leftObj.transform.FindChild("GGoodIcon(Clone)"))
		{
			return;
		}
		this.addGoodsIcon(data.Goods, false);
	}

	private void addGoodsIcon(GoodsData gd, bool grayShow = false)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BackSpriteName0 = backSpriteName;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = gd.GoodsID;
			ggoodIcon.ItemObject = gd;
			ggoodIcon.BoxTypes = -1;
			this.shopName.Text = goodsXmlNodeByID.Title;
			U3DUtils.AddChild(this.leftObj, ggoodIcon.gameObject, true);
			if (!grayShow)
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, gd, canUse, IconTextTypes.Qianghua);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			UIPanel component = ggoodIcon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
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

	public GameObject leftObj;

	public TextBlock limitBuyLabel;

	public TextBlock limitCount;

	public TextBlock shopName;

	public TextBlock residual;

	public TextBlock shopResidualCount;

	public TextBlock price;

	public GButton exchangeBtn;

	public int currentGoodsId;

	public OlympicsShopData currentShopData;

	public DPSelectedItemEventHandler DPSelectedItem;
}
