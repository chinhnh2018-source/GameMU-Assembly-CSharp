using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ZiYinJiCfgData
{
	public ZiYinJiCfgData(XElement element)
	{
		this.ID = Global.GetXElementAttributeInt(element, "ID");
		this.Type = Global.GetXElementAttributeInt(element, "Type");
		this.Name = Global.GetXElementAttributeStr(element, "Name");
		this.Level = Global.GetXElementAttributeInt(element, "Level");
		this.ShuXing = Global.GetXElementAttributeStr(element, "ShuXing");
	}

	public int ID { get; set; }

	public int Type { get; set; }

	public string Name { get; set; }

	public int Level { get; set; }

	public string ShuXing { get; set; }
}
