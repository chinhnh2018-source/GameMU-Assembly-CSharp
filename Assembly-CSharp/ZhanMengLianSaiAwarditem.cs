using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZhanMengLianSaiAwarditem : UserControl
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
			this.mLabelRankIndexlabel.text = string.Empty;
		}
		catch (Exception ex)
		{
		}
	}

	private GGoodIcon initGood(GoodsData data, bool BHaveTips = true)
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
			Vector3 localPosition = ggoodIcon.ContentText.transform.localPosition;
			localPosition.z = -0.003f;
			ggoodIcon.ContentText.transform.localPosition = localPosition;
			localPosition = ggoodIcon.SecondText.transform.localPosition;
			localPosition.z = -0.003f;
			ggoodIcon.SecondText.transform.localPosition = localPosition;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(string.Format("NetImages/GameRes/{0}", goodsImageURLFromIconCode), false, 0);
			Super.InitGoodsGIcon(ggoodIcon, data, Global.CanUseGoods(goodsXmlNodeByID.ID, false, true), IconTextTypes.Qianghua);
			UIDragPanelContents uidragPanelContents = ggoodIcon.GetComponent<UIDragPanelContents>();
			if (null == uidragPanelContents)
			{
				uidragPanelContents = ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			}
		}
		return ggoodIcon;
	}

	private void MouseLeftButtonUp(object sender, MouseEvent e)
	{
		GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
		GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
		GTipServiceEx.SelfBagOnly = false;
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		try
		{
			this._ItemCollection = this.mListBoxLstWuPin.ItemsSource;
			this.mListBoxLstWuPin.MouseLeftButtonDownEx = new MouseLeftButtonUpEventHandler(this.MouseLeftButtonUp);
		}
		catch (Exception ex)
		{
		}
	}

	public void RefreahInf(List<GoodsData> Lst, string name)
	{
		this.mLabelRankIndexlabel.text = name;
		int i = 0;
		while (i < Lst.Count)
		{
			GGoodIcon ggoodIcon = this.initGood(Lst[i], true);
			ggoodIcon.BackgroundSprite1Visible = true;
			ggoodIcon.BackgroundSprite1.transform.localScale = Vector3.one * 72f;
			ggoodIcon.BackgroundSprite1.spriteName = "bagGrid3_bak";
			if (null != ggoodIcon)
			{
				this._ItemCollection.AddNoUpdate(ggoodIcon);
			}
			UIPanel component = ggoodIcon.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
			i++;
			if (3 < i)
			{
				break;
			}
		}
		this.mListBoxLstWuPin.repositionNow = true;
	}

	public UIDraggablePanel DraggablePanel
	{
		set
		{
			if (null == this.mUIDragPanelContents)
			{
				this.mUIDragPanelContents = base.GetComponent<UIDragPanelContents>();
			}
			if (null == this.mUIDragPanelContents)
			{
				this.mUIDragPanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
			}
			this.mUIDragPanelContents.draggablePanel = value;
		}
	}

	[SerializeField]
	private ListBox mListBoxLstWuPin;

	[SerializeField]
	private UILabel mLabelRankIndexlabel;

	private ObservableCollection _ItemCollection;

	private UIDragPanelContents mUIDragPanelContents;
}
