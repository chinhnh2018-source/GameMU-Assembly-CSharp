using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZhanMengLianSaiShipPinItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitPrefabText()
	{
		try
		{
			this.mLabelDescription.lineWidth = 341;
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				ex.Message
			});
		}
	}

	private void InitTexture()
	{
	}

	private GGoodIcon initGood(GoodsData data)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
		GGoodIcon ggoodIcon = null;
		if (goodsXmlNodeByID != null)
		{
			ggoodIcon = Global.GetNewGoodIcon();
			ggoodIcon.Width = 50.0;
			ggoodIcon.Height = 50.0;
			ggoodIcon.ItemObject = data;
			ggoodIcon.ItemCode = goodsXmlNodeByID.ID;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(string.Format("NetImages/GameRes/{0}", goodsImageURLFromIconCode), false, 0);
			Super.InitGoodsGIcon(ggoodIcon, data, Global.CanUseGoods(goodsXmlNodeByID.ID, false, true), IconTextTypes.Qianghua);
		}
		return ggoodIcon;
	}

	public void RefreshInf(int GoodsID, string name, string Description)
	{
		GoodsData emptyGoodsData = Global.GetEmptyGoodsData(GoodsID, 0, 0, 0, 1, 0, 0, 0, 0);
		if (emptyGoodsData != null)
		{
			GGoodIcon ggoodIcon = this.initGood(emptyGoodsData);
			if (null != ggoodIcon)
			{
				ggoodIcon.BackgroundSprite1Visible = true;
				ggoodIcon.BackgroundSprite1.transform.localScale = Vector3.one * 72f;
				ggoodIcon.BackgroundSprite1.spriteName = "bagGrid3_bak";
				ggoodIcon.transform.localPosition = Vector3.zero;
				ggoodIcon.transform.SetParent(this.mObjIcon.transform, false);
			}
			UIPanel component = ggoodIcon.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
		this.mLabelName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			name
		});
		this.mLabelDescription.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Description
		});
	}

	private void InitHandler()
	{
		try
		{
		}
		catch (Exception ex)
		{
		}
	}

	[SerializeField]
	private GameObject mObjIcon;

	[SerializeField]
	private UILabel mLabelName;

	[SerializeField]
	private UILabel mLabelDescription;
}
