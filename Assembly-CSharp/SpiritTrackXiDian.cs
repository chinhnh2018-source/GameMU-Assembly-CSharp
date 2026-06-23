using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class SpiritTrackXiDian : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.DuiGou.gameObject.SetActive(false);
		UIEventListener.Get(this.DuiGouBack.gameObject).onClick = delegate(GameObject s)
		{
			if (!this.DuiGouIs)
			{
				this.DuiGouIs = true;
			}
			else
			{
				this.DuiGouIs = false;
			}
			this.DuiGou.gameObject.SetActive(this.DuiGouIs);
		};
		this.XiDian_Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClickXiDian();
		};
		this.XiDian_Btn.Text = Global.GetLang("洗点");
	}

	public void InitStr()
	{
		this.TitleStr.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("洗点消耗")
		});
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ResettingShenJiFuWen", ',');
		int num = 0;
		if (Global.Data.roleData.ShenJiDict != null)
		{
			Dictionary<int, ShenJiFuWenData>.Enumerator enumerator = Global.Data.roleData.ShenJiDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Dictionary<int, ShenJiFuWen> dicShenJiFuWen = SpiritTrackPart.GetDicShenJiFuWen();
				KeyValuePair<int, ShenJiFuWenData> keyValuePair = enumerator.Current;
				if (dicShenJiFuWen.ContainsKey(keyValuePair.Value.ID))
				{
					int num2 = num;
					Dictionary<int, ShenJiFuWen> dicShenJiFuWen2 = SpiritTrackPart.GetDicShenJiFuWen();
					KeyValuePair<int, ShenJiFuWenData> keyValuePair2 = enumerator.Current;
					int upNeed = dicShenJiFuWen2[keyValuePair2.Value.ID].UpNeed;
					KeyValuePair<int, ShenJiFuWenData> keyValuePair3 = enumerator.Current;
					num = num2 + upNeed * keyValuePair3.Value.Level;
				}
			}
		}
		this.Cost_Stone = systemParamIntArrayByName[1] * num;
		if (this.Cost_Stone > systemParamIntArrayByName[2])
		{
			this.Cost_Stone = systemParamIntArrayByName[2];
		}
		this.XiDian_Juan.text = Global.GetLang("{e3b36c}没有{b266ff}【洗点卷】{-}，自动消耗     {-}") + Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("{0}"), this.Cost_Stone)
		});
		this.XiDian_Num.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("可洗点数:{0}"), num)
		});
	}

	public void initIcon()
	{
		if (this.icon == null)
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ResettingShenJiFuWen", ',');
			this.ADDGoodsICON(systemParamIntArrayByName[0]);
		}
		else
		{
			this.ReNum();
		}
	}

	public void ReNum()
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ResettingShenJiFuWen", ',');
		this.Cost_Juan = 1;
		this.Juan_Num = Global.GetTotalGoodsCountByID(systemParamIntArrayByName[0]);
		if (this.icon != null)
		{
			this.icon.Text = string.Format(Global.GetLang("{0}/{1}"), this.Juan_Num, this.Cost_Juan);
			if (this.Juan_Num >= this.Cost_Juan)
			{
				this.icon.TextColor = 65280U;
			}
			else
			{
				this.icon.TextColor = 16777215U;
			}
		}
	}

	public void ClickXiDian()
	{
		if (!this.DuiGouIs)
		{
			if (this.Juan_Num >= this.Cost_Juan)
			{
				GameInstance.Game.ShenJiDianChongZhi(0);
			}
			else
			{
				Super.HintMainText(Global.GetLang("道具不足"), 10, 3);
			}
			return;
		}
		if (this.Juan_Num >= this.Cost_Juan)
		{
			GameInstance.Game.ShenJiDianChongZhi(0);
			return;
		}
		if (Global.Data.roleData.UserMoney < this.Cost_Stone)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			return;
		}
		GameInstance.Game.ShenJiDianChongZhi(1);
	}

	private void ADDGoodsICON(int goodsID)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(goodsID);
			this.icon = U3DUtils.NEW<GGoodIcon>();
			this.icon.Width = 64.0;
			this.icon.Height = 64.0;
			this.icon.isAutoSize = true;
			this.icon.BackSpriteName0 = backSpriteName;
			this.icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			this.icon.TipType = 1;
			this.icon.Tip = string.Format("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			this.icon.ItemCode = goodsID;
			this.icon.ItemObject = dummyGoodsData;
			this.icon.BoxTypes = 5;
			this.icon.TextShadowColor = 4278190080U;
			this.icon.TextColor = 16777215U;
			this.icon.DisableTextColor = 8421504U;
			this.icon.TextHorizontalAlignment = global::Layout.Right;
			this.icon.TextVerticalAlignment = global::Layout.Bottom;
			this.icon.STextVisibility = false;
			bool canUse = Global.CanUseGoods(dummyGoodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(this.icon, dummyGoodsData, canUse, IconTextTypes.Qianghua);
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ResettingShenJiFuWen", ',');
			this.Cost_Juan = 1;
			this.Juan_Num = Global.GetTotalGoodsCountByID(systemParamIntArrayByName[0]);
			this.icon.Text = string.Format(Global.GetLang("{0}/{1}"), this.Juan_Num, this.Cost_Juan);
			if (this.Juan_Num >= this.Cost_Juan)
			{
				this.icon.TextColor = 65280U;
			}
			else
			{
				this.icon.TextColor = 16777215U;
			}
			this.icon.TextShadowColor = 4278190080U;
			this.icon.TextHorizontalAlignment = global::Layout.Right;
			this.icon.TextVerticalAlignment = global::Layout.Bottom;
			this.GiftGood.Add(this.icon);
			UIPanel component = this.icon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			this.icon.addEventListener("click", delegate(MouseEvent e)
			{
				GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
				if (null == ggoodIcon)
				{
					return;
				}
				GoodsData goodsData = this.icon.ItemObject as GoodsData;
				if (goodsData == null)
				{
					return;
				}
				GTipServiceEx.ShowTip(this.icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
			});
		}
	}

	public UISprite ItemIcon_Sprite;

	public UILabel ItemLable;

	public UILabel XiDian_Num;

	public UILabel XiDian_Juan;

	public UISprite DuiGou;

	public UISprite DuiGouBack;

	public bool DuiGouIs;

	public GButton XiDian_Btn;

	public UILabel TitleStr;

	public ShowNetImage Image;

	public GButton Close;

	public DPSelectedItemEventHandler Closehandler;

	public SpriteSL GiftGood;

	private int Juan_Num;

	private int Cost_Juan;

	private int Cost_Stone;

	private GGoodIcon icon;
}
