using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class RecallLeiChong : RecallGoodsEx
{
	protected override void InitTextInPrefabs()
	{
		base.InitTextInPrefabs();
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (null != this.activityInfo)
		{
			this.activityInfo.text = Global.GetLang(Global.GetLang("成为回归用户即可领取(每个帐号只能领取一次)"));
		}
	}

	public override bool InitRewards()
	{
		if (!base.InitRewards())
		{
			return false;
		}
		this.ShowChongZhiListNew();
		return true;
	}

	private void ShowChongZhiListNew()
	{
		if (base.xml == null)
		{
			return;
		}
		if (base.xmlList == null)
		{
			return;
		}
		this._itemCollection.Clear();
		for (int i = 0; i < base.xmlList.Count; i++)
		{
			XElement xelement = base.xmlList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MinYuanBao");
			RecallLeiChongItem leiChongItem = null;
			leiChongItem = U3DUtils.NEW<RecallLeiChongItem>();
			leiChongItem.observer.Clear();
			leiChongItem.xmlID = xelementAttributeInt;
			leiChongItem.ZuanShiNumber = xelementAttributeInt2;
			leiChongItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (Global.GetBaoGuoSpaceCount() > leiChongItem.observer.Count)
				{
					this.SendOpenAward(leiChongItem.xmlID);
				}
				else
				{
					Super.HintMainText(Global.GetLang("领取失败，背包空间不足！"), 10, 3);
				}
			};
			if (null != leiChongItem.signBtn)
			{
				leiChongItem.signBtn.gameObject.SetActive(false);
			}
			if (null != leiChongItem)
			{
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsID1");
				this.AddIconToList(ref leiChongItem.observer, xelementAttributeStr, true, false);
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "GoodsID2");
				this.AddIconToList(ref leiChongItem.observer, xelementAttributeStr2, true, true);
				leiChongItem.observer.DelayUpdate();
				this._itemCollection.AddNoUpdate(leiChongItem);
				this._itemCollection.DelayUpdate();
				GGoodIcon[] componentsInChildren = leiChongItem.GetComponentsInChildren<GGoodIcon>();
				if (componentsInChildren != null)
				{
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						if (j > 3)
						{
							Object.Destroy(componentsInChildren[j].gameObject);
						}
					}
				}
			}
		}
		base._UpdateRewardsListStatus();
	}

	private void AddIconToList(ref ObservableCollection observer, string goods, bool iconEnable, bool isOccupation = false)
	{
		if (observer == null || string.Empty == goods)
		{
			return;
		}
		int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		if (goods.IndexOf("|") != -1)
		{
			string[] array = goods.Split(new char[]
			{
				'|'
			});
			foreach (string text in array)
			{
				string text2 = text;
				if (string.Empty != text2)
				{
					string[] array3 = text2.Split(new char[]
					{
						','
					});
					string text3 = array3[0];
					if (isOccupation)
					{
						if (MUJieripartChongzhiKingItem.IsTongGuo(array3[0], roleOcc))
						{
							goto IL_DA;
						}
					}
					GGoodIcon goodsItemIconEx = this.GetGoodsItemIconEx(array3, false, iconEnable);
					observer.AddNoUpdate(goodsItemIconEx);
					goodsItemIconEx.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
				}
				IL_DA:;
			}
		}
		else if (string.Empty != goods)
		{
			string[] array4 = goods.Split(new char[]
			{
				','
			});
			string text4 = array4[0];
			if (isOccupation)
			{
				if (!MUJieripartChongzhiKingItem.IsTongGuo(array4[0], roleOcc))
				{
					GGoodIcon goodsItemIconEx2 = this.GetGoodsItemIconEx(array4, false, iconEnable);
					observer.AddNoUpdate(goodsItemIconEx2);
					goodsItemIconEx2.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
				}
			}
			else
			{
				GGoodIcon goodsItemIconEx3 = this.GetGoodsItemIconEx(array4, false, iconEnable);
				observer.AddNoUpdate(goodsItemIconEx3);
				goodsItemIconEx3.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			}
		}
	}

	private void SetGoodsIconImageGray(GGoodIcon icon)
	{
		if (null != icon)
		{
			UITexture component = icon.GoodImg.gameObject.gameObject.GetComponent<UITexture>();
			if (null != component)
			{
				component.shader = Shader.Find("Unlit/Gray");
			}
		}
	}

	private GGoodIcon GetGoodsItemIcon(string strID, bool isDrag = false, int nLock = 0, bool bEnable = true, int nQiangHua = -1, int nZhuoYue = -1)
	{
		GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(strID), 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		dummyGoodsDataMu.Binding = nLock;
		dummyGoodsDataMu.ExcellenceInfo = nZhuoYue;
		dummyGoodsDataMu.Forge_level = nQiangHua;
		GGoodIcon ggoodIcon;
		if (dummyGoodsDataMu != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(dummyGoodsDataMu.GoodsID);
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.GoodsID = dummyGoodsDataMu.GoodsID;
			ggoodIcon.Width = 72.0;
			ggoodIcon.Height = 72.0;
			ggoodIcon.ItemCategory = categoriy;
			ggoodIcon.ItemObject = dummyGoodsDataMu;
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
			ggoodIcon.BackgroundSprite0.MakePixelPerfect();
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			ggoodIcon.Tip = Global.GetGoodsNameByID(dummyGoodsDataMu.GoodsID, false);
			bool canUse = Global.CanUseGoods(dummyGoodsDataMu.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, dummyGoodsDataMu, canUse, IconTextTypes.Qianghua);
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
		UIButtonOffset componentInChildren = ggoodIcon.GetComponentInChildren<UIButtonOffset>();
		if (null != componentInChildren)
		{
			componentInChildren.enabled = false;
		}
		return ggoodIcon;
	}

	private GGoodIcon GetGoodsItemIconEx(string[] goods, bool isDrag = false, bool bEnable = true)
	{
		if (goods.Length != 7)
		{
			return null;
		}
		GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(goods[0]), Convert.ToInt32(goods[3]), Convert.ToInt32(goods[4]), Convert.ToInt32(goods[6]), Convert.ToInt32(goods[5]), Convert.ToInt32(goods[2]), Convert.ToInt32(goods[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		GGoodIcon ggoodIcon;
		if (dummyGoodsDataMu != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(dummyGoodsDataMu.GoodsID);
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
			ggoodIcon.ItemCategory = categoriy;
			ggoodIcon.ItemCode = dummyGoodsDataMu.GoodsID;
			ggoodIcon.ItemObject = dummyGoodsDataMu;
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			bool canUse = Global.CanUseGoods(dummyGoodsDataMu.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, dummyGoodsDataMu, canUse, IconTextTypes.Qianghua);
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
		UIButtonOffset componentInChildren = ggoodIcon.GetComponentInChildren<UIButtonOffset>();
		if (null != componentInChildren)
		{
			componentInChildren.enabled = false;
		}
		ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
		return ggoodIcon;
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		string text = Convert.ToString(ggoodIcon.ItemCode);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		int num = Convert.ToInt32(text);
		if (1 < num)
		{
			GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
			GTipServiceEx.SelfBagOnly = false;
			if (goodsData != null)
			{
				GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
			}
		}
	}

	private void ShowGoodsTip(object icon)
	{
		GGoodIcon ggoodIcon = icon as GGoodIcon;
		if (null == ggoodIcon)
		{
			return;
		}
		string text = Convert.ToString(ggoodIcon.ItemCode);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		int num = Convert.ToInt32(text);
		if (1 < num)
		{
			GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
		}
	}
}
