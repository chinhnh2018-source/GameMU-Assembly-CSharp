using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class JieRiChongZhiQiangGouData3
{
	public JieRiChongZhiQiangGouData3(XElement ele)
	{
		JieRiChongZhiQiangGouData3 <>f__this = this;
		string rechargeItemConf = Global.GetRechargeItemCfgTypeByPlatform();
		if (ele != null)
		{
			this.ID = Global.GetXElementAttributeInt(ele, "ID");
			this.Day = Global.GetXElementAttributeStr(ele, "Day");
			this.ZhiGouID = Global.GetXElementAttributeInt(ele, "ZhiGouID");
			this.ChongZhiID = Global.GetXElementAttributeInt(ele, "ChongZhiID");
			this.YuanJia = Global.GetXElementAttributeInt(ele, "YuanJia");
			this.Effect = Global.GetXElementAttributeInt(ele, "Effect");
			XElement gameResXml = Global.GetGameResXml("Config/MU_ChongZhi.xml");
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Type");
			XElement xelement = xelementList.Find((XElement e) => Global.GetXElementAttributeStr(e, "TypeID") == rechargeItemConf);
			XElement xelement2 = null;
			if (xelement != null)
			{
				List<XElement> xelementList2 = Global.GetXElementList(xelement, "ChongZhi");
				xelement2 = xelementList2.Find((XElement e) => Global.GetXElementAttributeStr(e, "ID") == <>f__this.ChongZhiID.ToString());
			}
			if (xelement2 != null)
			{
				this.XianJia = Global.GetXElementAttributeInt(xelement2, "RMB");
				this.productId = Global.GetXElementAttributeStr(xelement2, "productId");
				if (Context.IsHaiwai)
				{
					this.productId = Global.GetXElementAttributeStr(xelement2, "productIdAn");
				}
			}
			this.GoodsOne = Global.GetXElementAttributeStr(ele, "GoodsOne");
			this.GoodsTwo = Global.GetXElementAttributeStr(ele, "GoodsTwo");
			this.SinglePurchase = Global.GetXElementAttributeInt(ele, "SinglePurchase");
			this.TitlePic = Global.GetXElementAttributeStr(ele, "TitlePic");
			this.Background = Global.GetXElementAttributeStr(ele, "Background");
		}
	}

	public string goodsid
	{
		get
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(this.GoodsTwo))
			{
				result = this.GoodsOne + "@" + this.GoodsTwo;
			}
			else
			{
				result = this.GoodsOne;
			}
			return result;
		}
	}

	public int ID;

	public string Day;

	public int ZhiGouID;

	public int ChongZhiID;

	public int YuanJia;

	public int XianJia;

	private string GoodsOne;

	private string GoodsTwo;

	public int SinglePurchase;

	public string TitlePic;

	public string Background;

	public int XianGouLeftNum;

	public int Effect;

	public string productId;
}
