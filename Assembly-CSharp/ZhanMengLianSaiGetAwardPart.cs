using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZhanMengLianSaiGetAwardPart : UserControl
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
			this.mBtnGetAward.Text = Global.GetLang("领取奖励");
		}
		catch (Exception ex)
		{
		}
	}

	private void InitTexture()
	{
	}

	public void RefreshInf(string title, List<GoodsData> GoodsList)
	{
		if (GoodsList != null)
		{
			for (int i = 0; i < GoodsList.Count; i++)
			{
				GGoodIcon ggoodIcon = this.initGood(GoodsList[i]);
				ggoodIcon.BackgroundSprite1Visible = true;
				ggoodIcon.BackgroundSprite1.transform.localScale = Vector3.one * 72f;
				ggoodIcon.BackgroundSprite1.spriteName = "bagGrid3_bak";
				if (null != ggoodIcon)
				{
					this.mObservableCollection.AddNoUpdate(ggoodIcon);
				}
				UIPanel component = ggoodIcon.GetComponent<UIPanel>();
				if (null != component)
				{
					Object.Destroy(component);
				}
			}
			if (GoodsList.Count == 4)
			{
				this.mGoodsListBox.transform.localPosition = new Vector3(-125f, 16f, -1f);
			}
			else
			{
				this.mGoodsListBox.transform.localPosition = new Vector3(-168f, 16f, -1f);
			}
			this.mGoodsListBox.repositionNow = true;
		}
		this.mTitleLabel.text = title;
	}

	private GGoodIcon initGood(GoodsData data)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
		GGoodIcon icon = null;
		if (goodsXmlNodeByID != null)
		{
			icon = Global.GetNewGoodIcon();
			icon.Width = 50.0;
			icon.Height = 50.0;
			icon.ItemObject = data;
			icon.ItemCode = goodsXmlNodeByID.ID;
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			icon.BodyURL = new ImageURL(string.Format("NetImages/GameRes/{0}", goodsImageURLFromIconCode), false, 0);
			icon.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				GGoodIcon icon = icon;
				GoodsData goodsData = icon.ItemObject as GoodsData;
			};
			Super.InitGoodsGIcon(icon, data, Global.CanUseGoods(goodsXmlNodeByID.ID, false, true), IconTextTypes.Qianghua);
		}
		return icon;
	}

	private void InitHandler()
	{
		try
		{
			this.mObservableCollection = this.mGoodsListBox.ItemsSource;
			this.mBtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						Type = 0
					});
				}
			};
			this.mBtnGetAward.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				GameInstance.Game.SendZhanMengLianSaiGetAwardRank();
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						Type = 0
					});
				}
			};
		}
		catch (Exception ex)
		{
		}
	}

	[SerializeField]
	private ListBox mGoodsListBox;

	[SerializeField]
	private GButton mBtnClose;

	[SerializeField]
	private GButton mBtnGetAward;

	[SerializeField]
	private UILabel mTitleLabel;

	private ObservableCollection mObservableCollection;

	public DPSelectedItemEventHandler Hander;
}
