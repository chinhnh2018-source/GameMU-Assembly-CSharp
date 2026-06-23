using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TeQuanShangChengVO
{
	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.TeQuanID = Global.GetXElementAttributeInt(xml, "TeQuanID");
		this.WuPinID = Global.GetXElementAttributeStr(xml, "WuPinID");
		this.JiaGe = Global.GetXElementAttributeInt(xml, "JiaGe");
		this.GouMaiCiShu = Global.GetXElementAttributeInt(xml, "GouMaiCiShu");
	}

	public int ID;

	public int TeQuanID;

	public string WuPinID;

	public int JiaGe;

	public int GouMaiCiShu;
}
