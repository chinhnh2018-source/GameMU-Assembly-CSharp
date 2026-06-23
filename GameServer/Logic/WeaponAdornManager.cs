using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;

namespace GameServer.Logic
{
	public class WeaponAdornManager
	{
		public static int GetWeaponAdornOrder(int nOccupation, int nHandType, int nActionType)
		{
			int result;
			if (nOccupation < 0 || nHandType < 0 || nActionType < 0)
			{
				result = -1;
			}
			else
			{
				result = 1000 * nOccupation + 100 * nHandType + nActionType;
			}
			return result;
		}

		public static WeaponAdornInfo GetWeaponAdornInfo(int nOccupation, int nHandType, int nActionType)
		{
			int weaponAdornOrder = WeaponAdornManager.GetWeaponAdornOrder(nOccupation, nHandType, nActionType);
			WeaponAdornInfo result;
			if (weaponAdornOrder < 0)
			{
				result = null;
			}
			else
			{
				WeaponAdornInfo weaponAdornInfo = null;
				if (!WeaponAdornManager.dictWeaponAdornInfo.TryGetValue(weaponAdornOrder, out weaponAdornInfo))
				{
					result = null;
				}
				else
				{
					result = weaponAdornInfo;
				}
			}
			return result;
		}

		public static int VerifyWeaponCanEquip(int nOccupation, int nHandType, int nActionType, Dictionary<int, List<GoodsData>> EquipDict)
		{
			WeaponAdornInfo weaponAdornInfo = WeaponAdornManager.GetWeaponAdornInfo(nOccupation, nHandType, nActionType);
			int result;
			if (null == weaponAdornInfo)
			{
				result = -1;
			}
			else
			{
				int num = 0;
				List<GoodsData> list = null;
				int i = 11;
				while (i < 22)
				{
					list = null;
					lock (EquipDict)
					{
						if (EquipDict.TryGetValue(i, out list))
						{
							if (list != null && list.Count > 0)
							{
								if (weaponAdornInfo.listCoexistType.Count < 1)
								{
									return -5;
								}
								num += list.Count;
								bool flag2 = false;
								for (int j = 0; j < list.Count; j++)
								{
									flag2 = false;
									SystemXmlItem systemXmlItem = null;
									if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(list[j].GoodsID, out systemXmlItem))
									{
										return -1;
									}
									int intValue = systemXmlItem.GetIntValue("HandType", -1);
									int intValue2 = systemXmlItem.GetIntValue("ActionType", -1);
									for (int k = 0; k < weaponAdornInfo.listCoexistType.Count; k++)
									{
										if (weaponAdornInfo.listCoexistType[k].nHandType == intValue && weaponAdornInfo.listCoexistType[k].nActionType == intValue2)
										{
											flag2 = true;
											break;
										}
									}
								}
								if (!flag2)
								{
									return -5;
								}
							}
						}
					}
					IL_19C:
					i++;
					continue;
					goto IL_19C;
				}
				if (num > 1)
				{
					result = -3;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		public static void LoadWeaponAdornInfo()
		{
			try
			{
				XElement gameResXml = Global.GetGameResXml(string.Format("Config/WeaponAdorn.xml", new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements("Weapons").Elements<XElement>();
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							WeaponAdornInfo weaponAdornInfo = new WeaponAdornInfo();
							int num = (int)Global.GetSafeAttributeLong(xelement, "Occupation");
							weaponAdornInfo.nOccupationLimit = num;
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement, "Type");
							if (!string.IsNullOrEmpty(safeAttributeStr.Trim()))
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									','
								});
								if (array != null && array.Length == 2)
								{
									weaponAdornInfo.tagWeaponTypeInfo.nHandType = Convert.ToInt32(array[0]);
									weaponAdornInfo.tagWeaponTypeInfo.nActionType = Convert.ToInt32(array[1]);
								}
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement, "CoexistType");
							if (!string.IsNullOrEmpty(safeAttributeStr2.Trim()))
							{
								string[] array = safeAttributeStr2.Split(new char[]
								{
									'|'
								});
								if (array != null && array.Length > 0)
								{
									for (int i = 0; i < array.Length; i++)
									{
										string[] array2 = array[i].Split(new char[]
										{
											','
										});
										if (array2 != null && array2.Length == 2)
										{
											WeaponTypeAndACTInfo weaponTypeAndACTInfo = new WeaponTypeAndACTInfo();
											weaponTypeAndACTInfo.nHandType = Convert.ToInt32(array2[0]);
											weaponTypeAndACTInfo.nActionType = Convert.ToInt32(array2[1]);
											weaponAdornInfo.listCoexistType.Add(weaponTypeAndACTInfo);
										}
									}
								}
							}
							int weaponAdornOrder = WeaponAdornManager.GetWeaponAdornOrder(num, weaponAdornInfo.tagWeaponTypeInfo.nHandType, weaponAdornInfo.tagWeaponTypeInfo.nActionType);
							if (weaponAdornOrder > 0)
							{
								WeaponAdornManager.dictWeaponAdornInfo.Add(weaponAdornOrder, weaponAdornInfo);
							}
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/WeaponAdorn.xml", new object[0])));
			}
		}

		public static Dictionary<int, WeaponAdornInfo> dictWeaponAdornInfo = new Dictionary<int, WeaponAdornInfo>();
	}
}
