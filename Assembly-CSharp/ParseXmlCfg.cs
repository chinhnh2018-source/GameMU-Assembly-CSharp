using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ParseXmlCfg
{
	public List<XElement> GetXElements(string cfgName, string rootName)
	{
		XElement gameResXml = Global.GetGameResXml(cfgName);
		if (gameResXml == null)
		{
			return null;
		}
		return Global.GetXElementList(gameResXml, rootName);
	}
}
