using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class OrnamentXmlData
{
	public OrnamentXmlData(XElement ele)
	{
		if (ele != null)
		{
			this.GoodsID = Global.GetXElementAttributeInt(ele, "GoodsID");
			this.Type = Global.GetXElementAttributeInt(ele, "Type");
			this.Name = Global.GetXElementAttributeStr(ele, "Name");
			this.Recover = Global.GetXElementAttributeInt(ele, "Recover");
			this.Description = Global.GetXElementAttributeStr(ele, "Description");
			this.GoalType = Global.GetXElementAttributeInt(ele, "GoalType");
			this.GoalNum = Global.GetXElementAttributeInt(ele, "GoalNum");
			this.GoalAward = Global.GetXElementAttributeInt(ele, "GoalAward");
			this.List = Global.GetXElementAttributeInt(ele, "List");
			this.Show = Global.GetXElementAttributeInt(ele, "Show");
		}
		this.HaveActive = false;
		this.CanActive = false;
	}

	public int GoodsID;

	public int Type;

	public string Name;

	public int Recover;

	public string Description;

	public int GoalType;

	public int GoalNum;

	public int List;

	public int Show;

	public bool HaveActive;

	public bool CanActive;

	public int GoalAward;
}
