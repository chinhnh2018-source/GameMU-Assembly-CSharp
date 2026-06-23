using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class SettingsGoodsPack
{
	public void CopyFrom(XElement xml)
	{
		this.MaxOvertimeTick = Global.GetXElementAttributeInt(xml, "MaxOvertimeTick");
		this.PackDestroyTimeTick = Global.GetXElementAttributeInt(xml, "PackDestroyTimeTick");
	}

	public int MaxOvertimeTick;

	public int PackDestroyTimeTick;
}
