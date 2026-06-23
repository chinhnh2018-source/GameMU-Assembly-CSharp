using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class BaoXiangTipsVO
{
	public void CopyFrom(XElement xml)
	{
		this.GoodsID = Global.GetXElementAttributeInt(xml, "GoodsID");
		this.Sex = Global.GetXElementAttributeInt(xml, "Sex");
		this.Ocuupation = Global.GetXElementAttributeInt(xml, "Ocuupation");
		this.Type = Global.GetXElementAttributeInt(xml, "Type");
		this.Name = Global.GetXElementAttributeStr(xml, "Name");
		this.Description = Global.GetXElementAttributeStr(xml, "Description");
		this.Award = Global.GetXElementAttributeStr(xml, "Award");
	}

	public int GoodsID;

	public int Sex;

	public int Ocuupation;

	public int Type;

	public string Name;

	public string Description;

	public string Award;
}
