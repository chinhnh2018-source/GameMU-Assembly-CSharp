using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ZhuYinJiCfgData
{
	public ZhuYinJiCfgData(XElement element)
	{
		this.ID = Global.GetXElementAttributeInt(element, "ID");
		this.TypeZhu = Global.GetXElementAttributeInt(element, "TypeZhu");
		this.Name = Global.GetXElementAttributeStr(element, "Name");
		this.TypeFu = Global.GetXElementAttributeStr(element, "TypeFu");
		this.NeedLevel = Global.GetXElementAttributeInt(element, "NeedLevel");
		this.Level = Global.GetXElementAttributeInt(element, "Level");
		this.ShuXing = Global.GetXElementAttributeStr(element, "ShuXing");
	}

	public int ID { get; set; }

	public int TypeZhu { get; set; }

	public string Name { get; set; }

	public string TypeFu { get; set; }

	public int NeedLevel { get; set; }

	public int Level { get; set; }

	public string ShuXing { get; set; }
}
