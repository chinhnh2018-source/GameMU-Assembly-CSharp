using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class SettingsAlive
{
	public void CopyFrom(XElement xml)
	{
		this.GoodsID = Global.GetXElementAttributeInt(xml, "GoodsID");
		this.MaxLevel = Global.GetXElementAttributeInt(xml, "MaxLevel");
	}

	public int GoodsID;

	public int MaxLevel;
}
