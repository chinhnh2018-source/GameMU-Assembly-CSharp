using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class StallOtherPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitControl();
		this.InitBagPart();
	}

	private void InitControl()
	{
		this.m_ListStallItemObC = this.m_ListStallItem.ItemsSource;
		this.m_ListStallItem.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			StallItem component = this.m_ListStallItem.SelectedItem.gameObject.GetComponent<StallItem>();
			this.ShowGoodsTip(component);
		};
		if (null != this.m_BtnClose)
		{
			this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			};
		}
	}

	public void AddStallItem(GoodsData goodsdata, int nPrice, int nNum)
	{
		if (goodsdata == null)
		{
			return;
		}
		StallItem stallItem = U3DUtils.NEW<StallItem>();
		stallItem.goodsdata = goodsdata;
		stallItem.m_LblPrice.text = Convert.ToString(nPrice);
		this.m_ListStallItemObC.AddNoUpdate(stallItem);
		this.m_ListStallItemObC.DelayUpdate();
	}

	public void DeleteStallItem(int nDbID)
	{
		for (int i = 0; i < this.m_ListStallItemObC.Count; i++)
		{
			StallItem component = this.m_ListStallItemObC[i].gameObject.GetComponent<StallItem>();
			if (component.m_GoodsData.Id == nDbID)
			{
				this.m_ListStallItemObC.RemoveAt(i);
				break;
			}
		}
		this.m_ListStallItemObC.DelayUpdate();
	}

	public void InitStallItem()
	{
		if (Global.Data.OtherSaleGoodsDataList != null)
		{
			for (int i = 0; i < Global.Data.OtherSaleGoodsDataList.Count; i++)
			{
				GoodsData goodsData = Global.Data.OtherSaleGoodsDataList[i];
				StallItem item = U3DUtils.NEW<StallItem>();
				item.goodsdata = Global.Data.OtherSaleGoodsDataList[i];
				if (null != item.m_GoodsIcon)
				{
					item.m_GoodsIcon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
					{
						GameInstance.Game.SpriteMarketBuyGoods(item.m_GoodsData.Id, item.m_GoodsData.GoodsID);
					};
				}
				this.m_ListStallItemObC.AddNoUpdate(item);
				this.m_ListStallItemObC.DelayUpdate();
			}
		}
	}

	private void InitBagPart()
	{
		if (null != this.m_GameObjBag)
		{
			if (Super._ParcelPart != null)
			{
				Super._ParcelPart.iBaoGuoMode = 7;
				U3DUtils.AddChild(this.m_GameObjBag.gameObject, Super._ParcelPart.gameObject, true);
			}
			else
			{
				ParcelPart parcelPart = U3DUtils.NEW<ParcelPart>();
				Super._ParcelPart = parcelPart;
				Super._ParcelPart.iBaoGuoMode = 4;
				Super._ParcelPart.InitPartData();
				U3DUtils.AddChild(this.m_GameObjBag.gameObject, Super._ParcelPart.gameObject, true);
			}
		}
	}

	private List<GoodsData> GetBeiBaoWuPin()
	{
		List<GoodsData> list = new List<GoodsData>();
		if (list == null || Global.Data.roleData.GoodsDataList == null)
		{
			return list;
		}
		for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
		{
			if (Global.Data.roleData.GoodsDataList[i].Using == 0)
			{
				list.Add(Global.Data.roleData.GoodsDataList[i]);
			}
		}
		return list;
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
		ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
		};
		return ggoodIcon;
	}

	private void ShowGoodsTip(StallItem item)
	{
		if (null == item)
		{
			return;
		}
		string text = Convert.ToString(item.m_GoodsData.GoodsID);
		if (string.Empty == text)
		{
			return;
		}
		int num = Convert.ToInt32(text);
		if (1 < num)
		{
			GTipServiceEx.SelfBagOnly = false;
			if (0 < item.m_GoodsData.SaleMoney1)
			{
				GTipServiceEx.ShowTip(item.m_GoodsIcon, TipTypes.GoodsText, GoodsOwnerTypes.OtherOnSale, GoodsPriceUnitTypes.Jinbi, item.m_nSalePrice, item.m_GoodsData.GoodsID, item.m_GoodsData.Id, -1, null);
			}
			else
			{
				GTipServiceEx.ShowTip(item.m_GoodsIcon, TipTypes.GoodsText, GoodsOwnerTypes.OtherOnSale, GoodsPriceUnitTypes.Zhuanshi, item.m_nSalePrice, item.m_GoodsData.GoodsID, item.m_GoodsData.Id, -1, null);
			}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton m_BtnClose;

	public GameObject m_GameObjBag;

	public ParcelPart m_ParcelPart;

	public ListBox m_ListStallItem = new ListBox();

	private ObservableCollection m_ListStallItemObC;
}
