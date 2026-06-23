using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic.MUWings
{
	public class WingPropsCacheManager
	{
		public static SystemXmlItem GetWingPropsCacheItem(int occupation, int level)
		{
			string key = string.Format("{0}_{1}", occupation, level);
			SystemXmlItem systemXmlItem = null;
			SystemXmlItem result;
			if (!WingPropsCacheManager.WingPropsItemsDict.TryGetValue(key, out systemXmlItem))
			{
				result = null;
			}
			else
			{
				result = systemXmlItem;
			}
			return result;
		}

		public static void LoadWingPropsItemsByOccupation(int occupation)
		{
			if (occupation != 4)
			{
				string text = "";
				XElement xelement = null;
				try
				{
					text = string.Format("Config/Wing/Wing_{0}.xml", occupation);
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
				IEnumerable<XElement> enumerable = xelement.Elements("Level");
				foreach (XElement xelement2 in enumerable)
				{
					SystemXmlItem value = new SystemXmlItem
					{
						XMLNode = xelement2
					};
					string key = string.Format("{0}_{1}", occupation, (int)Global.GetSafeAttributeLong(xelement2, "ID"));
					WingPropsCacheManager.WingPropsItemsDict[key] = value;
				}
			}
		}

		public static void LoadWingPropsItems()
		{
			for (int i = 0; i < 6; i++)
			{
				WingPropsCacheManager.LoadWingPropsItemsByOccupation(i);
			}
		}

		private static Dictionary<string, SystemXmlItem> WingPropsItemsDict = new Dictionary<string, SystemXmlItem>();
	}
}
