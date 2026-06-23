using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class XingyunChoujiangItem : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public Zhuanpan MZhuanpan
	{
		get
		{
			return this.mZhuanpan;
		}
		set
		{
			this.mZhuanpan = value;
			this.mInitGoodsIcon(value.GoodsID);
			if (Context.IsHaiwai)
			{
				switch (value.AwardLabel)
				{
				case 0:
					this.mMark.spriteName = "none";
					break;
				case 1:
					this.mMark.spriteName = "Nice";
					break;
				case 2:
					this.mMark.spriteName = "Prefect";
					break;
				}
			}
		}
	}

	public XingYunSelectState MXingYunSelectState
	{
		get
		{
			return this.mXingYunSelectState;
		}
		set
		{
			this.mXingYunSelectState = value;
			switch (value)
			{
			case XingYunSelectState.None:
				NGUITools.SetActive(this.mWalking.gameObject, false);
				NGUITools.SetActive(this.mSelected.gameObject, false);
				break;
			case XingYunSelectState.Walking:
				NGUITools.SetActive(this.mWalking.gameObject, true);
				NGUITools.SetActive(this.mSelected.gameObject, false);
				break;
			case XingYunSelectState.Selected:
				NGUITools.SetActive(this.mWalking.gameObject, false);
				NGUITools.SetActive(this.mSelected.gameObject, true);
				break;
			}
		}
	}

	private void mInitGoodsIcon(string value)
	{
		GoodsData dummyGoodsData = Global.GetDummyGoodsData(value);
		if (dummyGoodsData == null)
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(dummyGoodsData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
		ggoodIcon.TipType = 1;
		ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
		ggoodIcon.ItemCode = dummyGoodsData.GoodsID;
		ggoodIcon.ItemObject = dummyGoodsData;
		ggoodIcon.BoxTypes = -1;
		ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
		Super.InitGoodsGIcon(ggoodIcon, dummyGoodsData, Global.CanUseGoods(dummyGoodsData.GoodsID, false, true), IconTextTypes.Qianghua);
		ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		this.mGoodIcon.Add(ggoodIcon);
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

	public SpriteSL mGoodIcon;

	public GameObject mWalking;

	public GameObject mSelected;

	public UISprite mMark;

	private Zhuanpan mZhuanpan = new Zhuanpan();

	private XingYunSelectState mXingYunSelectState;
}
