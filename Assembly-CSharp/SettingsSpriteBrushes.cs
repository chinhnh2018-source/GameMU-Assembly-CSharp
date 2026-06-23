using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class SettingsSpriteBrushes
{
	public void CopyFrom(XElement xml)
	{
		this.FactionBrushColor = Global.GetXElementAttributeStr(xml, "FactionBrushColor");
		this.ClanBrushColor = Global.GetXElementAttributeStr(xml, "ClanBrushColor");
		this.SnameBrushColor = Global.GetXElementAttributeStr(xml, "SnameBrushColor");
		this.LifeBrushColor = Global.GetXElementAttributeStr(xml, "LifeBrushColor");
	}

	public string FactionBrushColor;

	public string ClanBrushColor;

	public string SnameBrushColor;

	public string LifeBrushColor;
}
