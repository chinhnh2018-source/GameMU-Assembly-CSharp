using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

namespace HSGameEngine.GameEngine.Data
{
	public static class ConfigMagager
	{
		public static int GetFuBenMapDict(int mapCode)
		{
			XElement gameResXml = Global.GetGameResXml("Config/FuBenMap.Xml");
			if (gameResXml == null)
			{
				return 0;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Copy", "MapCode", mapCode.ToString());
			if (xelement == null)
			{
				return 0;
			}
			return Global.GetXElementAttributeInt(xelement, "MinSaoDangTime");
		}
	}
}
