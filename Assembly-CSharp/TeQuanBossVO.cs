using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TeQuanBossVO
{
	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.TeQuanID = Global.GetXElementAttributeInt(xml, "TeQuanID");
		this.DiTuID = Global.GetXElementAttributeInt(xml, "DiTuID");
		this.GuaiWuID = Global.GetXElementAttributeInt(xml, "GuaiWuID");
	}

	public int ID;

	public int TeQuanID;

	public int DiTuID;

	public int GuaiWuID;
}
