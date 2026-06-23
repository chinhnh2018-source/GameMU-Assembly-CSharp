using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class XingyunLableItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public void mSetText(ZhuanPanGongGaoData mZhuanPanGongGaoData)
	{
		if (mZhuanPanGongGaoData == null)
		{
			return;
		}
		this.gd = Global.GetDummyGoodsData(mZhuanPanGongGaoData.GoodsId);
		this.RoleID = mZhuanPanGongGaoData.RoleID;
		this.GoodsIndex = mZhuanPanGongGaoData.GoodsIndex;
		string empty = string.Empty;
		string empty2 = string.Empty;
		this.mGetTextColoeByGoods(this.gd, out empty2, out empty);
		if (this.mText != null)
		{
			this.mText.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("恭喜")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format(Global.GetLang("【{0}】"), mZhuanPanGongGaoData.RoleName) + Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("获得了")
				})
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				empty2,
				string.Format(Global.GetLang("【{0}】"), empty)
			});
		}
	}

	private void mGetTextColoeByGoods(GoodsData goodsData, out string mFontColor, out string mGoodsName)
	{
		mFontColor = string.Empty;
		mGoodsName = string.Empty;
		if (goodsData == null)
		{
			return;
		}
		string text = string.Empty;
		string text2 = "FFFFFF";
		if (Global.GetGoodsCatetoriy(goodsData.GoodsID) == 10 || Global.GetGoodsCatetoriy(goodsData.GoodsID) == 9)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsData.ExcellenceInfo != 0 || 1 < goodsXmlNodeByID.SuitID)
			{
				text2 = "ff08ff";
			}
			else
			{
				text2 = "0099ff";
			}
		}
		else if (Global.GetGoodsCatetoriy(goodsData.GoodsID) >= 0 && Global.GetGoodsCatetoriy(goodsData.GoodsID) <= 22 && goodsData.ExcellenceInfo > 0)
		{
			int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
			if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
			{
				text2 = "00ff00";
				text += UIHelper.ZuoyueTitleNames[0];
			}
			else if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
			{
				text += UIHelper.ZuoyueTitleNames[1];
				text2 = "0099ff";
			}
			else if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
			{
				text += UIHelper.ZuoyueTitleNames[2];
				text2 = "ff08ff";
			}
		}
		else if (goodsData.Lucky > 0)
		{
			text2 = "0099FF";
		}
		else
		{
			text2 = Global.GetNguiStrColor((GoodsQuality)Super.GetGoodsQuality(goodsData.GoodsID));
		}
		mGoodsName = text + Global.GetGoodsNameByID(goodsData.GoodsID, false);
		mFontColor = text2;
	}

	public TextBlock mText;

	public GoodsData gd;

	public int RoleID = -1;

	public int GoodsIndex = -1;
}
