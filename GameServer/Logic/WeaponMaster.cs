using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class WeaponMaster
	{
		public static void UpdateRoleAttr(GameClient client, int weaponType, bool needBrocast = false)
		{
			try
			{
				List<WeaponMaster.WeaponMasterItem> list;
				if (WeaponMaster.WeaponMasterXml.TryGetValue(weaponType, out list))
				{
					int num = 11;
					int num2 = 21;
					List<int> list2 = new List<int>();
					foreach (GoodsData goodsData in client.ClientData.GoodsDataList)
					{
						if (goodsData.Using == 1)
						{
							int num3 = -1;
							SystemXmlItem systemXmlItem = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
							{
								num3 = systemXmlItem.GetIntValue("Categoriy", -1);
							}
							if (num3 >= num && num3 <= num2)
							{
								list2.Add(num3);
							}
						}
					}
					if (list2.Count >= 1 && list2.Count <= 2)
					{
						WeaponMaster.WeaponMasterItem weaponMasterItem = null;
						foreach (WeaponMaster.WeaponMasterItem weaponMasterItem2 in list)
						{
							if (WeaponMaster.WeaponIsMatch(weaponMasterItem2.WeaponType1, weaponMasterItem2.WeaponType2, list2) || WeaponMaster.WeaponIsMatch(weaponMasterItem2.WeaponType2, weaponMasterItem2.WeaponType1, list2))
							{
								weaponMasterItem = weaponMasterItem2;
								break;
							}
						}
						double[] array = (weaponMasterItem == null) ? new double[177] : weaponMasterItem.ExtProps;
						client.ClientData.PropsCacheManager.SetExtProps(new object[]
						{
							PropsSystemTypes.WeaponMaster,
							array
						});
						if (needBrocast)
						{
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("WeaponMaster :: 更新角色武器大师属性加成:{0}, 失败。", new object[0]), ex, true);
			}
		}

		public static bool WeaponIsMatch(List<int> leftList, List<int> rightList, List<int> equipList)
		{
			int num = 1;
			if (leftList == null || leftList.Count < 1 || leftList[0] < 0)
			{
				if (equipList.Count > 1)
				{
					return false;
				}
				num = 0;
			}
			else if (!leftList.Contains(equipList[0]))
			{
				return false;
			}
			bool result;
			if (rightList == null || rightList.Count < 1 || rightList[0] < 0)
			{
				result = (equipList.Count < num + 1);
			}
			else
			{
				result = (equipList.Count >= num + 1 && rightList.Contains(equipList[num]));
			}
			return result;
		}

		public static void LoadWeaponMaster()
		{
			string text = "Config\\WeaponMaster.xml";
			try
			{
				text = Global.GameResPath(text);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							int key = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Type", "0"));
							List<WeaponMaster.WeaponMasterItem> list;
							if (!WeaponMaster.WeaponMasterXml.TryGetValue(key, out list))
							{
								list = new List<WeaponMaster.WeaponMasterItem>();
								WeaponMaster.WeaponMasterXml[key] = list;
							}
							string defAttributeStr = Global.GetDefAttributeStr(xelement2, "WeaponType1", "");
							string defAttributeStr2 = Global.GetDefAttributeStr(xelement2, "WeaponType2", "");
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "WeaponMasterProps");
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							double[] array2 = new double[177];
							foreach (string text2 in array)
							{
								string[] array4 = text2.Split(new char[]
								{
									','
								});
								if (array4.Length == 2)
								{
									ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array4[0]);
									if (propIndexByPropName < ExtPropIndexes.Max)
									{
										array2[(int)propIndexByPropName] = Global.SafeConvertToDouble(array4[1]);
									}
								}
							}
							List<WeaponMaster.WeaponMasterItem> list2 = list;
							WeaponMaster.WeaponMasterItem weaponMasterItem = new WeaponMaster.WeaponMasterItem();
							WeaponMaster.WeaponMasterItem weaponMasterItem2 = weaponMasterItem;
							List<int> weaponType;
							if (!("" == defAttributeStr))
							{
								weaponType = Array.ConvertAll<string, int>(defAttributeStr.Split(new char[]
								{
									','
								}), (string x) => Convert.ToInt32(x)).ToList<int>();
							}
							else
							{
								weaponType = new List<int>();
							}
							weaponMasterItem2.WeaponType1 = weaponType;
							WeaponMaster.WeaponMasterItem weaponMasterItem3 = weaponMasterItem;
							List<int> weaponType2;
							if (!("" == defAttributeStr2))
							{
								weaponType2 = Array.ConvertAll<string, int>(defAttributeStr2.Split(new char[]
								{
									','
								}), (string x) => Convert.ToInt32(x)).ToList<int>();
							}
							else
							{
								weaponType2 = new List<int>();
							}
							weaponMasterItem3.WeaponType2 = weaponType2;
							weaponMasterItem.ExtProps = array2;
							list2.Add(weaponMasterItem);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		public static Dictionary<int, List<WeaponMaster.WeaponMasterItem>> WeaponMasterXml = new Dictionary<int, List<WeaponMaster.WeaponMasterItem>>();

		public class WeaponMasterItem
		{
			public int Type;

			public List<int> WeaponType1;

			public List<int> WeaponType2;

			public double[] ExtProps = new double[177];
		}
	}
}
