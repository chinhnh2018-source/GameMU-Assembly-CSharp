using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class HefuHuikuiSuperBossGongchengPart : UserControl
{
	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	public void InitPartData(string goodsList)
	{
		string text = StringUtil.trim(goodsList);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 6)
			{
				this.AddGoodsIcon(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2]), int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), -1);
			}
		}
	}

	private void AddGoodsIcon(int goodsID, int gcount, int quality, int forgeLevel, int binding, int born, int Id = -1)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			GoodsData goodsData = Global.AddGiftGoodsData(goodsID, forgeLevel, quality, binding, gcount, born);
			string goodsImageURLFromIconCodeEx = Super.GetGoodsImageURLFromIconCodeEx(Super.GetIconCode(goodsXmlNodeByID));
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 48.0;
			ggoodIcon.Height = 48.0;
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCodeEx, false, 2);
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				goodsData.Id,
				12
			});
			ggoodIcon.ItemCode = goodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.Text = gcount.ToString();
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = ColorSL.FromArgb(255, 58, 206, 0);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
			this.Container.Children.Add(ggoodIcon);
			Canvas.SetLeft(ggoodIcon, 256);
			Canvas.SetTop(ggoodIcon, 62);
			return;
		}
	}
}
