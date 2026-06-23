using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ExtPropIndexesVO
{
	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.Word = Global.GetXElementAttributeStr(xml, "Word");
		this.Description = Global.GetXElementAttributeStr(xml, "Description");
		this.ShowList = Global.GetXElementAttributeInt(xml, "ShowList");
		this.Percent = Global.GetXElementAttributeInt(xml, "Percent");
		this.ShowList2 = Global.GetXElementAttributeInt(xml, "ShowList2");
	}

	public int ID;

	public string Word;

	public string Description;

	public int ShowList;

	public int Percent;

	public int ShowList2;
}
