using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class LianSaiLeagueSuperHelpInfVO
{
	public LianSaiLeagueSuperHelpInfVO(XElement xml)
	{
		if (xml != null)
		{
			this.ID = Global.GetXElementAttributeInt(xml, "ID");
			this.Intro = Global.GetXElementAttributeStr(xml, "Intro");
			this.Bold = Global.GetXElementAttributeInt(xml, "Bold");
		}
	}

	public int ID;

	public string Intro;

	public int Bold;
}
