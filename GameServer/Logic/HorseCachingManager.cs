using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	public class HorseCachingManager
	{
		public static SystemXmlItem GetHorseEnchanceItem(int level, HorseExtIndexes extIndex)
		{
			string key = string.Format("{0}_{1}", level, HorseCachingManager.XmlItemNames[(int)extIndex]);
			SystemXmlItem systemXmlItem = null;
			SystemXmlItem result;
			if (!HorseCachingManager.HorseItemsDict.TryGetValue(key, out systemXmlItem))
			{
				result = null;
			}
			else
			{
				result = systemXmlItem;
			}
			return result;
		}

		public static void LoadHorseEnchanceItems()
		{
			string text = "";
			XElement xelement = null;
			try
			{
				text = string.Format("Config/Horses/HorseEnchance.xml", new object[0]);
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
			IEnumerable<XElement> enumerable = xelement.Elements("Levels");
			foreach (XElement xelement2 in enumerable)
			{
				IEnumerable<XElement> enumerable2 = xelement2.Elements();
				foreach (XElement xelement3 in enumerable2)
				{
					SystemXmlItem value = new SystemXmlItem
					{
						XMLNode = xelement3
					};
					string key = string.Format("{0}_{1}", Global.GetSafeAttributeStr(xelement2, "level"), xelement3.Name);
					HorseCachingManager.HorseItemsDict[key] = value;
				}
			}
		}

		private static string[] XmlItemNames = new string[]
		{
			"WuGong",
			"WuFang",
			"MoGong",
			"MoFang",
			"BaoJi",
			"MingZong",
			"ShanBi",
			"ShengL",
			"MoFaL",
			"KangBao"
		};

		private static Dictionary<string, SystemXmlItem> HorseItemsDict = new Dictionary<string, SystemXmlItem>();
	}
}
