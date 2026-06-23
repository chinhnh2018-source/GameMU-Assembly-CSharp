using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TeQuanJiHuoVO
{
	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.HuoDongName = Global.GetXElementAttributeStr(xml, "HuoDongName");
		this.HuoDongTuBiao = Global.GetXElementAttributeStr(xml, "HuoDongTuBiao");
		this.tips = Global.GetXElementAttributeStr(xml, "tips");
		this.CanShu = Global.GetXElementAttributeInt(xml, "CanShu");
		this.TiaoJian = Global.GetXElementAttributeInt(xml, "TiaoJian");
		this.Tips = Global.GetXElementAttributeStr(xml, "Tips");
		this.UIAnNiu = Global.GetXElementAttributeInt(xml, "UIAnNiu");
	}

	public int ID;

	public string HuoDongName;

	public string HuoDongTuBiao;

	public string tips;

	public int CanShu;

	public int TiaoJian;

	public string Tips;

	public int UIAnNiu;
}
