using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ArmyGroupLegionsVO
{
	public void CopyFrom(XElement xml)
	{
		if (xml != null)
		{
			this.ID = Global.GetXElementAttributeInt(xml, "ID");
			this.Name = Global.GetXElementAttributeStr(xml, "Name");
			this.Manager = Global.GetXElementAttributeInt(xml, "Manager");
			this.AppointLeader = Global.GetXElementAttributeInt(xml, "AppointLeader");
			this.AppointElite = Global.GetXElementAttributeInt(xml, "AppointElite");
			this.Quit = Global.GetXElementAttributeInt(xml, "Quit");
			this.Dissolution = Global.GetXElementAttributeInt(xml, "Dissolution");
			this.BulletinCD = Global.GetXElementAttributeInt(xml, "BulletinCD");
			this.TalkLevel = Global.GetXElementAttributeStr(xml, "TalkLevel");
			this.TalkCD = Global.GetXElementAttributeFloat(xml, "TalkCD");
		}
	}

	public int ID;

	public string Name;

	public int Manager;

	public int AppointLeader;

	public int AppointElite;

	public int Quit;

	public int Dissolution;

	public int BulletinCD;

	public string TalkLevel;

	public float TalkCD;
}
