using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class MonthCardAwardItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public void SetItVisble(bool show)
	{
		if (base.IsActive != show)
		{
			base.gameObject.SetActive(show);
		}
	}

	public void SetGoodsData(GoodsData data)
	{
		this.GoodsItemData = data;
		this.SetItVisble(true);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
		int categoriy = goodsXmlNodeByID.Categoriy;
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		bool canUse = Global.CanUseGoods(data.GoodsID, false, true);
		this.ItemIcon.Width = 64.0;
		this.ItemIcon.Height = 64.0;
		this.ItemIcon.BackSpriteName0 = "bagGrid4_bak";
		NGUITools.SetActive(this.ItemIcon.BackgroundSprite0, true);
		this.ItemIcon.BackgroundSprite0.MakePixelPerfect();
		this.ItemIcon.ItemCategory = categoriy;
		this.ItemIcon.ItemCode = data.GoodsID;
		this.ItemIcon.ItemObject = data;
		this.ItemIcon.isAutoSize = true;
		this.ItemIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		Super.InitGoodsGIcon(this.ItemIcon, data, canUse, IconTextTypes.Qianghua);
	}

	public void SetDiamondNum(string num)
	{
		this.SetItVisble(true);
		this.ItemIcon.Width = 78.0;
		this.ItemIcon.Height = 78.0;
		this.ItemIcon.BackSpriteName0 = "bagGrid4_bak";
		NGUITools.SetActive(this.ItemIcon.BackgroundSprite0, true);
		this.ItemIcon.BackgroundSprite0.MakePixelPerfect();
		this.ItemIcon.isAutoSize = true;
		this.ItemIcon.SecondText.text = string.Empty;
		this.ItemIcon.ContentText.textColor = 13480843U;
		this.ItemIcon.ContentText.FontSize = 14;
		this.ItemIcon.Text = num;
	}

	public GGoodIcon ItemIcon;

	private GoodsData GoodsItemData;
}
