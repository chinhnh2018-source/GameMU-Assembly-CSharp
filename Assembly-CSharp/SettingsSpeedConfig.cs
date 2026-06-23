using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class SettingsSpeedConfig
{
	public void CopyFrom(XElement xml)
	{
		this.WalkUnitCost = Global.GetXElementAttributeInt(xml, "WalkUnitCost");
		this.RunUnitCost = Global.GetXElementAttributeInt(xml, "RunUnitCost");
		this.Tick = Global.GetXElementAttributeStr(xml, "Tick");
		this.MaxAttackSlotTick = Global.GetXElementAttributeInt(xml, "MaxAttackSlotTick");
	}

	public int WalkUnitCost;

	public int RunUnitCost;

	public string Tick;

	public int MaxAttackSlotTick;
}
