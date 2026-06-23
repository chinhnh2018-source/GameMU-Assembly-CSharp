using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ChannelNameVO
{
	public void CopyForm(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.Channel = Global.GetXElementAttributeStr(xml, "Channel");
		this.Name = Global.GetXElementAttributeStr(xml, "Name");
		this.PTID = Global.GetXElementAttributeInt(xml, "PTID");
		this.PTName = Global.GetXElementAttributeStr(xml, "PTName");
	}

	public int ID;

	public string Channel;

	public string Name;

	public int PTID;

	public string PTName;
}
