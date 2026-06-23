using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	public class MagicsCacheManager
	{
		public static SystemXmlItem GetMagicCacheItem(int occupation, int skillID, int skillLevel)
		{
			string key = string.Format("{0}_{1}_{2}", occupation, skillID, skillLevel);
			SystemXmlItem systemXmlItem = null;
			SystemXmlItem result;
			if (!MagicsCacheManager.MagicItemsDict.TryGetValue(key, out systemXmlItem))
			{
				result = null;
			}
			else
			{
				result = systemXmlItem;
			}
			return result;
		}

		public static void LoadMagicItemsByOccupation(int occupation)
		{
			if (occupation != 4)
			{
				string text = "";
				XElement xelement = null;
				try
				{
					text = string.Format("Config/Magics/Magics_{0}.xml", occupation);
					xelement = XElement.Load(Global.GameResPath(text));
					if (null == xelement)
					{
						throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
					}
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
				}
				IEnumerable<XElement> enumerable = xelement.Elements("Magic");
				foreach (XElement xelement2 in enumerable)
				{
					IEnumerable<XElement> enumerable2 = xelement2.Elements("JiNeng");
					foreach (XElement xelement3 in enumerable2)
					{
						SystemXmlItem value = new SystemXmlItem
						{
							XMLNode = xelement3
						};
						string key = string.Format("{0}_{1}_{2}", occupation, (int)Global.GetSafeAttributeLong(xelement2, "ID"), (int)Global.GetSafeAttributeLong(xelement3, "Level"));
						MagicsCacheManager.MagicItemsDict[key] = value;
					}
				}
			}
		}

		public static void LoadMagicItems()
		{
			for (int i = 0; i < 6; i++)
			{
				MagicsCacheManager.LoadMagicItemsByOccupation(i);
			}
		}

		private static Dictionary<string, SystemXmlItem> MagicItemsDict = new Dictionary<string, SystemXmlItem>();
	}
}
