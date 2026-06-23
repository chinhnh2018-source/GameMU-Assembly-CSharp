using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;

namespace GameServer.Logic
{
	public class ChuanQiQianHua
	{
		public static List<ChuanQiQianHuaItem> GetListChuanQiQianHuaItem(int qianHuaID)
		{
			List<ChuanQiQianHuaItem> result = null;
			Dictionary<int, List<ChuanQiQianHuaItem>> qianHuaItemDict = ChuanQiQianHua.QianHuaItemDict;
			lock (qianHuaItemDict)
			{
				qianHuaItemDict.TryGetValue(qianHuaID, out result);
			}
			return result;
		}

		public static void LoadEquipQianHuaProps()
		{
			XElement xelement = null;
			string text = "";
			try
			{
				text = string.Format("Config/QiangHua.xml", new object[0]);
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
			Dictionary<int, List<ChuanQiQianHuaItem>> dictionary = new Dictionary<int, List<ChuanQiQianHuaItem>>();
			IEnumerable<XElement> enumerable = xelement.Elements("QiangHua");
			foreach (XElement xmlnode in enumerable)
			{
				SystemXmlItem systemXmlItem = new SystemXmlItem
				{
					XMLNode = xmlnode
				};
				int intValue = systemXmlItem.GetIntValue("ID", -1);
				dictionary[intValue] = ChuanQiQianHua.ParseSystemXmlItem(systemXmlItem);
			}
			ChuanQiQianHua.QianHuaItemDict = dictionary;
		}

		private static List<ChuanQiQianHuaItem> ParseSystemXmlItem(SystemXmlItem systemXmlItem)
		{
			List<ChuanQiQianHuaItem> list = new List<ChuanQiQianHuaItem>();
			string stringValue = systemXmlItem.GetStringValue("QiangHua");
			List<ChuanQiQianHuaItem> result;
			if (string.IsNullOrEmpty(stringValue))
			{
				result = list;
			}
			else
			{
				string[] array = stringValue.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					list.AddRange(ChuanQiQianHua.ParseChuanQiQianHuaItem(systemXmlItem.GetIntValue("ID", -1), array[i]));
				}
				result = list;
			}
			return result;
		}

		public static int GetExtPropIndexeFromStr(string str)
		{
			int num = -1;
			lock (ChuanQiQianHua.StrToExtPropIndexDict)
			{
				if (ChuanQiQianHua.StrToExtPropIndexDict.TryGetValue(str, out num))
				{
					return num;
				}
			}
			for (int i = 0; i < 177; i++)
			{
				string a = string.Format("{0}", (ExtPropIndexes)i);
				if (a == str)
				{
					num = i;
					break;
				}
			}
			lock (ChuanQiQianHua.StrToExtPropIndexDict)
			{
				ChuanQiQianHua.StrToExtPropIndexDict[str] = num;
			}
			return num;
		}

		private static List<ChuanQiQianHuaItem> ParseChuanQiQianHuaItem(int qianHuaID, string strValue)
		{
			return new List<ChuanQiQianHuaItem>();
		}

		public static void ApplayEquipQianHuaProps(double[] equipProps, int occupation, GoodsData goodsData, SystemXmlItem systemGoods, bool toAdd)
		{
			List<MagicActionItem> list = null;
			if (GameManager.SystemMagicActionMgr.GoodsActionsDict.TryGetValue(goodsData.GoodsID, out list) && null != list)
			{
				if (list.Count > 0)
				{
					if (list[0].MagicActionID == MagicActionIDs.DB_ADD_YINYONG)
					{
						if (list[0].MagicActionParams.Length == 2)
						{
							int qianHuaID = (int)list[0].MagicActionParams[0];
							List<ChuanQiQianHuaItem> listChuanQiQianHuaItem = ChuanQiQianHua.GetListChuanQiQianHuaItem(qianHuaID);
							if (listChuanQiQianHuaItem != null && listChuanQiQianHuaItem.Count > 0)
							{
								for (int i = 0; i < listChuanQiQianHuaItem.Count; i++)
								{
									if (listChuanQiQianHuaItem[i].QianHuaLevel <= goodsData.Forge_level)
									{
										if (toAdd)
										{
											equipProps[listChuanQiQianHuaItem[i].PropIndex] += listChuanQiQianHuaItem[i].ItemValue;
										}
										else
										{
											equipProps[listChuanQiQianHuaItem[i].PropIndex] -= listChuanQiQianHuaItem[i].ItemValue;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public static Dictionary<int, List<ChuanQiQianHuaItem>> QianHuaItemDict = null;

		public static Dictionary<string, int> StrToExtPropIndexDict = new Dictionary<string, int>();
	}
}
