using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class SettingsDistanceConfig
{
	public void CopyFrom(XElement xml)
	{
		this.WalkStepWidth = Global.GetXElementAttributeInt(xml, "WalkStepWidth");
		this.RunStepWidth = Global.GetXElementAttributeInt(xml, "RunStepWidth");
		this.MaxAttackDistance = Global.GetXElementAttributeInt(xml, "MaxAttackDistance");
		this.MinAttackDistance = Global.GetXElementAttributeInt(xml, "MinAttackDistance");
		this.MaxMagicDistance = Global.GetXElementAttributeInt(xml, "MaxMagicDistance");
		this.MaxUnWatchDistance = Global.GetXElementAttributeInt(xml, "MaxUnWatchDistance");
	}

	public int WalkStepWidth;

	public int RunStepWidth;

	public int MaxAttackDistance;

	public int MinAttackDistance;

	public int MaxMagicDistance;

	public int MaxUnWatchDistance;
}
