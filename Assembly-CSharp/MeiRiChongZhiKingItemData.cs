using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class MeiRiChongZhiKingItemData
{
	public MeiRiChongZhiKingItemData(XElement e, int day)
	{
		this.ID = Global.GetXElementAttributeInt(e, "ID");
		this.Name = string.Empty;
		this.Day = day;
		string xelementAttributeStr = Global.GetXElementAttributeStr(e, "GoodsOne");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(e, "GoodsTwo");
		this.MinYuanBao = Global.GetXElementAttributeInt(e, "MinYuanBao");
		this.GoodsThr = Global.GetXElementAttributeStr(e, "GoodsThr");
		this.EffectiveTime = Global.GetXElementAttributeStr(e, "EffectiveTime");
		string goodsID = string.Empty;
		if (!string.IsNullOrEmpty(xelementAttributeStr2))
		{
			goodsID = xelementAttributeStr + "@" + xelementAttributeStr2;
		}
		else
		{
			goodsID = xelementAttributeStr;
		}
		this.GoodsID = goodsID;
	}

	public string RoleName
	{
		set
		{
			this.Name = value;
		}
	}

	public int ID;

	public string Name;

	public int Day;

	public int MinYuanBao;

	public string GoodsID;

	public string GoodsThr;

	public string EffectiveTime;
}
