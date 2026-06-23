using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class CangBaoEventXml
{
	public static XElement GetCangBaoEventNode(int eventId)
	{
		XElement gameResXml = Global.GetGameResXml("GameRes/Config/Treasure/TreasureEvent.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "TreasureEvent");
			for (int i = 0; i < xelementList.Count; i++)
			{
				if (eventId == Global.GetXElementAttributeInt(xelementList[i], "ID"))
				{
					return xelementList[i];
				}
			}
		}
		return null;
	}
}
