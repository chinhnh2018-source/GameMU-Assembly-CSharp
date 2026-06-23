using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TeQuanHuoBaoVO
{
	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.TeQuanID = Global.GetXElementAttributeInt(xml, "TeQuanID");
		this.BangZuanShuLiang = Global.GetXElementAttributeInt(xml, "BangZuanShuLiang");
		this.HongBaoQuYu = Global.GetXElementAttributeInt(xml, "HongBaoQuYu");
	}

	public int ID;

	public int TeQuanID;

	public int BangZuanShuLiang;

	public int HongBaoQuYu;
}
