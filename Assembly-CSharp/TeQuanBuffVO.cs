using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TeQuanBuffVO
{
	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.TeQuanID = Global.GetXElementAttributeInt(xml, "TeQuanID");
		this.HuoDongLeiXing = Global.GetXElementAttributeInt(xml, "HuoDongLeiXing");
		this.KaiQiBeiShu = Global.GetXElementAttributeDouble(xml, "KaiQiBeiShu");
	}

	public int ID;

	public int TeQuanID;

	public int HuoDongLeiXing;

	public double KaiQiBeiShu;
}
