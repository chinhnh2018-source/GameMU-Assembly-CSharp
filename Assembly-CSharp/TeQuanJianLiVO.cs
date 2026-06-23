using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TeQuanJianLiVO
{
	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.TeQuanID = Global.GetXElementAttributeInt(xml, "TeQuanID");
		this.WuPinID = Global.GetXElementAttributeStr(xml, "WuPinID");
		this.LingQuTiaoJian = Global.GetXElementAttributeInt(xml, "LingQuTiaoJian");
	}

	public int ID;

	public int TeQuanID;

	public string WuPinID;

	public int LingQuTiaoJian;
}
