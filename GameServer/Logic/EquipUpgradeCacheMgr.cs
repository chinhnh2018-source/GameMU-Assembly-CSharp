using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	public class EquipUpgradeCacheMgr
	{
		public static SystemXmlItem GetEquipUpgradeCacheItem(int categoriy, int suitID)
		{
			SystemXmlItem result;
			if (null == EquipUpgradeCacheMgr._EquipUpgradeItemsDict)
			{
				result = null;
			}
			else
			{
				string key = string.Format("{0}_{1}", categoriy, suitID);
				SystemXmlItem systemXmlItem = null;
				if (!EquipUpgradeCacheMgr._EquipUpgradeItemsDict.TryGetValue(key, out systemXmlItem))
				{
					result = null;
				}
				else
				{
					result = systemXmlItem;
				}
			}
			return result;
		}

		public static SystemXmlItem GetEquipUpgradeItemByGoodsID(int goodsID, int maxSuiItID)
		{
			SystemXmlItem systemXmlItem = null;
			SystemXmlItem result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemXmlItem))
			{
				result = null;
			}
			else
			{
				int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
				if (intValue < 0 || intValue >= 49)
				{
					result = null;
				}
				else
				{
					int num = systemXmlItem.GetIntValue("SuitID", -1);
					if (num < 1 || num > maxSuiItID)
					{
						num = 1;
					}
					result = EquipUpgradeCacheMgr.GetEquipUpgradeCacheItem(intValue, num);
				}
			}
			return result;
		}

		public static void LoadEquipUpgradeItems()
		{
			string text = "";
			XElement xelement = null;
			try
			{
				text = string.Format("Config/EquipUpgrade.xml", new object[0]);
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
			Dictionary<string, SystemXmlItem> dictionary = new Dictionary<string, SystemXmlItem>();
			IEnumerable<XElement> enumerable = xelement.Elements("Equip");
			foreach (XElement xelement2 in enumerable)
			{
				IEnumerable<XElement> enumerable2 = xelement2.Elements("Item");
				foreach (XElement xelement3 in enumerable2)
				{
					SystemXmlItem value = new SystemXmlItem
					{
						XMLNode = xelement3
					};
					string key = string.Format("{0}_{1}", (int)Global.GetSafeAttributeLong(xelement2, "Categoriy"), (int)Global.GetSafeAttributeLong(xelement3, "SuitID"));
					dictionary[key] = value;
				}
			}
			EquipUpgradeCacheMgr._EquipUpgradeItemsDict = dictionary;
		}

		private static Dictionary<string, SystemXmlItem> _EquipUpgradeItemsDict = new Dictionary<string, SystemXmlItem>();
	}
}
