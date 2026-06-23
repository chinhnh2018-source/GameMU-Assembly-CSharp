using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class JiangLi : UserControl
{
	public void ShowBg(int count)
	{
		if (count > 0 && count <= this.mBgs.Length)
		{
			for (int i = 0; i < count; i++)
			{
				NGUITools.SetActive(this.mBgs[i].gameObject, true);
			}
		}
	}

	public void initGood(string[] goods, int idx)
	{
		int num = Convert.ToInt32(goods[0]);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 50.0;
			ggoodIcon.Height = 50.0;
			GoodsData goodsData = new GoodsData();
			goodsData.GoodsID = num;
			goodsData.GCount = Convert.ToInt32(goods[1]);
			goodsData.Binding = Convert.ToInt32(goods[2]);
			goodsData.Forge_level = Convert.ToInt32(goods[3]);
			goodsData.AppendPropLev = Convert.ToInt32(goods[4]);
			goodsData.Lucky = Convert.ToInt32(goods[5]);
			goodsData.ExcellenceInfo = Convert.ToInt32(goods[6]);
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.ItemCode = num;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			ggoodIcon.transform.localPosition = new Vector3(-130f + 90f * (float)idx, 0f, 0f);
			U3DUtils.AddChild(base.gameObject, ggoodIcon.gameObject, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, Global.CanUseGoods(num, false, true), IconTextTypes.Qianghua);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.ShowGoodsTip(s);
			};
			BoxCollider component = ggoodIcon.transform.GetComponent<BoxCollider>();
			component.center = new Vector3(0f, 0f, -1f);
		}
	}

	private void ShowGoodsTip(object icon)
	{
		GGoodIcon ggoodIcon = icon as GGoodIcon;
		GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
		GTipServiceEx.SelfBagOnly = false;
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
	}

	public bool IsMyAward
	{
		set
		{
			NGUITools.SetActive(this.mSprtAwardTag.gameObject, value);
		}
	}

	public TextBlock Context;

	public UISprite mSprtAwardTag;

	public UISprite[] mBgs;
}
