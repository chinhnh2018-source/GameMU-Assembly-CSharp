using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class CfgGoodsItemList : ListBox
{
	public CfgGoodsItemList(int baoguoID, XElement node)
	{
		this.InitUI(node);
		this.InitGoodsItemList(baoguoID);
	}

	protected void InitUI(XElement node)
	{
		base.Width = (double)Global.GetXElementAttributeInt(node, "Width");
		base.Height = (double)Global.GetXElementAttributeInt(node, "Height");
		base.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this, Global.GetXElementAttributeInt(node, "Left"));
		Canvas.SetTop(this, Global.GetXElementAttributeInt(node, "Top"));
		base.ItemMargin = new Thickness(0.0, 0.0, 5.0, 5.0);
	}

	protected void InitGoodsItemList(int baoguoID)
	{
		Global.Data.ViewGoodsPackDataList = Global.UnPackGoodsID(baoguoID);
		if (Global.Data.ViewGoodsPackDataList == null)
		{
			return;
		}
		for (int i = 0; i < Global.Data.ViewGoodsPackDataList.Count; i++)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Global.Data.ViewGoodsPackDataList[i].GoodsID);
			if (goodsXmlNodeByID != null)
			{
				GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 32.0;
				ggoodIcon.Height = 32.0;
				ggoodIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty), false, 0);
				ggoodIcon.TipType = 1;
				ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
				{
					goodsXmlNodeByID.ID,
					0,
					Global.Data.ViewGoodsPackDataList[i].Id,
					7
				});
				ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
				ggoodIcon.ItemCode = Global.Data.ViewGoodsPackDataList[i].GoodsID;
				ggoodIcon.ItemObject = Global.Data.ViewGoodsPackDataList[i];
				ggoodIcon.BoxTypes = -1;
				ggoodIcon.Text = ((Global.Data.ViewGoodsPackDataList[i].GCount <= 1) ? string.Empty : Global.Data.ViewGoodsPackDataList[i].GCount.ToString());
				ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
				ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
				ggoodIcon.TextShadowColor = 4278190080U;
				ggoodIcon.TextColor = ColorSL.FromArgb(255, 58, 206, 0);
				Super.InitGoodsGIcon(ggoodIcon, Global.Data.ViewGoodsPackDataList[i], true, IconTextTypes.Qianghua);
				GoodsPackItem goodsPackItem = U3DUtils.NEW<GoodsPackItem>();
				goodsPackItem.GoodsImgs = ggoodIcon;
				goodsPackItem.GoodsImgBacks = Global.GetGameResImage("Images/Plate/xtsl_rec1.png");
				base.ItemsSource.AddNoUpdate(goodsPackItem);
			}
		}
		base.ItemsSource.DelayUpdate();
	}
}
