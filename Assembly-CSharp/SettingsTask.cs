using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class SettingsTask
{
	public void CopyFrom(XElement xml)
	{
		this.MaxOverTimeTick = Global.GetXElementAttributeInt(xml, "MaxOverTimeTick");
		this.MaxFocusNum = Global.GetXElementAttributeInt(xml, "MaxFocusNum");
	}

	public int MaxOverTimeTick;

	public int MaxFocusNum;
}
