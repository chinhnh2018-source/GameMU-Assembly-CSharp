using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class SettingsSpriteConfig
{
	public void CopyFrom(XElement xml)
	{
		this.LifeTotalWidth = Global.GetXElementAttributeInt(xml, "LifeTotalWidth");
		this.HoldWidth = Global.GetXElementAttributeInt(xml, "HoldWidth");
		this.HoldHeight = Global.GetXElementAttributeInt(xml, "HoldHeight");
	}

	public int LifeTotalWidth;

	public int HoldWidth;

	public int HoldHeight;
}
