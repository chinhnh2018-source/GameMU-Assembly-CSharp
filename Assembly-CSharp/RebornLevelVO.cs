using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class RebornLevelVO
{
	public RebornLevelVO(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.NeedRebornExp = Global.GetXElementAttributeInt(xml, "NeedRebornExp");
		this.MaxOfMonsters = Global.GetXElementAttributeInt(xml, "MaxOfMonsters");
		this.MaxOfGoods = Global.GetXElementAttributeInt(xml, "MaxOfGoods");
	}

	public int ID;

	public int NeedRebornExp;

	public int MaxOfMonsters;

	public int MaxOfGoods;
}
