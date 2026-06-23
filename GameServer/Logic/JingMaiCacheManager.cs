using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	public class JingMaiCacheManager
	{
		public static SystemXmlItem GetJingMaiItem(int occupation, int jingMaiID, int jingMaiBodyLevel)
		{
			string key = string.Format("{0}_{1}_{2}", occupation, jingMaiID, jingMaiBodyLevel);
			SystemXmlItem systemXmlItem = null;
			SystemXmlItem result;
			if (!JingMaiCacheManager.JingMaiItemsDict.TryGetValue(key, out systemXmlItem))
			{
				result = null;
			}
			else
			{
				result = systemXmlItem;
			}
			return result;
		}

		public static void LoadJingMaiItemsByOccupation(int occupation)
		{
			string text = "";
			XElement xelement = null;
			try
			{
				text = string.Format("Config/JingMais/{0}.xml", occupation);
				xelement = XElement.Load(Global.GameResPath(text));
				if (null == xelement)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
			}
			IEnumerable<XElement> enumerable = xelement.Elements("JingMai");
			foreach (XElement xelement2 in enumerable)
			{
				IEnumerable<XElement> enumerable2 = xelement2.Elements("Chong");
				foreach (XElement xelement3 in enumerable2)
				{
					SystemXmlItem value = new SystemXmlItem
					{
						XMLNode = xelement3
					};
					string key = string.Format("{0}_{1}_{2}", occupation, (int)Global.GetSafeAttributeLong(xelement2, "ID"), (int)Global.GetSafeAttributeLong(xelement3, "ID"));
					JingMaiCacheManager.JingMaiItemsDict[key] = value;
				}
			}
		}

		public static void LoadJingMaiItems()
		{
			for (int i = 0; i < 3; i++)
			{
				JingMaiCacheManager.LoadJingMaiItemsByOccupation(i);
			}
		}

		private static Dictionary<string, SystemXmlItem> JingMaiItemsDict = new Dictionary<string, SystemXmlItem>();
	}
}
