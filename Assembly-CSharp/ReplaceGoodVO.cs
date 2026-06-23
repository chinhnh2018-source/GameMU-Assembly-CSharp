using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ReplaceGoodVO
{
	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.ToType = Global.GetXElementAttributeStr(xml, "ToType");
		this.ToTypeProperty = Global.GetXElementAttributeStr(xml, "ToTypeProperty");
		this.OldGoods = Global.GetXElementAttributeInt(xml, "OldGoods");
		this.NewGoods = Global.GetXElementAttributeInt(xml, "NewGoods");
	}

	public int ID;

	public string ToType;

	public string ToTypeProperty;

	public int OldGoods;

	public int NewGoods;
}
