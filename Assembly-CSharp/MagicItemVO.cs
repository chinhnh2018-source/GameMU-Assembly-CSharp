using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class MagicItemVO
{
	public void CopyFrom(XElement xml)
	{
		this.Level = Global.GetXElementAttributeInt(xml, "Level");
		this.NeedZhuanSheng = Global.GetXElementAttributeInt(xml, "NeedZhuanSheng");
		this.NeedRoleLevel = Global.GetXElementAttributeInt(xml, "NeedRoleLevel");
		this.ShuLianDu = Global.GetXElementAttributeInt(xml, "ShuLianDu");
		this.NeedJinBi = Global.GetXElementAttributeInt(xml, "NeedJinBi");
	}

	public int Level;

	public int NeedZhuanSheng;

	public int NeedRoleLevel;

	public int ShuLianDu;

	public int NeedJinBi;
}
