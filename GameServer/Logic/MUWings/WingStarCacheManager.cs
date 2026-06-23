using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic.MUWings
{
	public class WingStarCacheManager
	{
		public static SystemXmlItem GetWingStarCacheItem(int occupation, int level, int starNum)
		{
			string key = string.Format("{0}_{1}_{2}", occupation, level, starNum);
			SystemXmlItem systemXmlItem = null;
			SystemXmlItem result;
			if (!WingStarCacheManager.WingStarItemsDict.TryGetValue(key, out systemXmlItem))
			{
				result = null;
			}
			else
			{
				result = systemXmlItem;
			}
			return result;
		}

		public static void LoadWingStarItemsByOccupation(int occupation)
		{
			if (occupation != 4)
			{
				string text = "";
				XElement xelement = null;
				try
				{
					text = string.Format("Config/Wing/WingStar_{0}.xml", occupation);
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
				IEnumerable<XElement> enumerable = xelement.Elements("Wing");
				foreach (XElement xelement2 in enumerable)
				{
					IEnumerable<XElement> enumerable2 = xelement2.Elements("Item");
					foreach (XElement xelement3 in enumerable2)
					{
						SystemXmlItem value = new SystemXmlItem
						{
							XMLNode = xelement3
						};
						string key = string.Format("{0}_{1}_{2}", occupation, (int)Global.GetSafeAttributeLong(xelement2, "ID"), (int)Global.GetSafeAttributeLong(xelement3, "Star"));
						WingStarCacheManager.WingStarItemsDict[key] = value;
					}
				}
			}
		}

		public static void LoadWingStarItems()
		{
			for (int i = 0; i < 6; i++)
			{
				WingStarCacheManager.LoadWingStarItemsByOccupation(i);
			}
		}

		private static Dictionary<string, SystemXmlItem> WingStarItemsDict = new Dictionary<string, SystemXmlItem>();
	}
}
