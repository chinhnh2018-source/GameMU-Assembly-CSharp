using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class RedPointVO
{
	public RedPointVO(XElement element)
	{
		this.ID = Global.GetXElementAttributeInt(element, "ID");
		this.OrderID = Global.GetXElementAttributeInt(element, "OrderID");
		this.SystemID = Global.GetXElementAttributeInt(element, "SystemID");
		this.Type = Global.GetXElementAttributeInt(element, "Type");
		this.Parameter = Global.GetXElementAttributeStr(element, "Parameter");
	}

	public int ID;

	public int OrderID;

	public int SystemID;

	public int Type;

	public string Parameter;
}
