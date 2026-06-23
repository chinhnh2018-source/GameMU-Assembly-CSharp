using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class DuanWeiVO
{
	public DuanWeiVO(XElement element)
	{
		this.ID = Global.GetXElementAttributeInt(element, "ID");
		this.Type = Global.GetXElementAttributeInt(element, "Type");
		this.Level = Global.GetXElementAttributeInt(element, "Level");
		this.TypeName = Global.GetXElementAttributeStr(element, "TypeName");
		this.NeedDuanWeiJiFen = Global.GetXElementAttributeInt(element, "NeedDuanWeiJiFen");
		this.WinJiFen = Global.GetXElementAttributeInt(element, "WinJiFen");
		this.LoseJiFen = Global.GetXElementAttributeInt(element, "LoseJiFen");
		this.RongYaoNum = Global.GetXElementAttributeInt(element, "RongYaoNum");
		this.WinRongYu = Global.GetXElementAttributeInt(element, "WinRongYu");
		this.LoseRongYu = Global.GetXElementAttributeInt(element, "LoseRongYu");
	}

	public int ID { get; set; }

	public int Type { get; set; }

	public int Level { get; set; }

	public string TypeName { get; set; }

	public int NeedDuanWeiJiFen { get; set; }

	public int WinJiFen { get; set; }

	public int LoseJiFen { get; set; }

	public int RongYaoNum { get; set; }

	public int WinRongYu { get; set; }

	public int LoseRongYu { get; set; }
}
