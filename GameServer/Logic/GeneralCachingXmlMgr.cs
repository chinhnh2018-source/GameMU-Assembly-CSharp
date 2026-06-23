using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	public class GeneralCachingXmlMgr
	{
		private static XElement CachingXml(string xmlFileName)
		{
			XElement xelement = null;
			try
			{
				string jieRiConfigFileName = WorldLevelManager.getInstance().GetJieRiConfigFileName(xmlFileName);
				xelement = XElement.Load(jieRiConfigFileName);
				lock (GeneralCachingXmlMgr.CachingXmlDict)
				{
					GeneralCachingXmlMgr.CachingXmlDict[xmlFileName] = xelement;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString() + "xmlFileName=" + xmlFileName);
				return null;
			}
			return xelement;
		}

		public static XElement GetXElement(string xmlFileName)
		{
			XElement result = null;
			lock (GeneralCachingXmlMgr.CachingXmlDict)
			{
				if (GeneralCachingXmlMgr.CachingXmlDict.TryGetValue(xmlFileName, out result))
				{
					return result;
				}
			}
			return GeneralCachingXmlMgr.CachingXml(xmlFileName);
		}

		public static XElement Reload(string xmlFileName)
		{
			return GeneralCachingXmlMgr.CachingXml(xmlFileName);
		}

		public static void RemoveCachingXml(string xmlFileName)
		{
			lock (GeneralCachingXmlMgr.CachingXmlDict)
			{
				GeneralCachingXmlMgr.CachingXmlDict.Remove(xmlFileName);
			}
		}

		private static Dictionary<string, XElement> CachingXmlDict = new Dictionary<string, XElement>();
	}
}
