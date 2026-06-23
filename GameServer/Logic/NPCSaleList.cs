using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	public class NPCSaleList
	{
		public Dictionary<int, NPCSaleItem> SaleIDSDict
		{
			get
			{
				return this._SaleIDSDict;
			}
		}

		public bool LoadSaleList()
		{
			string text = string.Format("Config/NPCSaleList.xml", new object[0]);
			XElement xelement = XElement.Load(Global.GameResPath(text));
			if (null == xelement)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
			}
			IEnumerable<XElement> enumerable = xelement.Elements("SaleList").Elements<XElement>();
			bool result;
			if (null == enumerable)
			{
				result = false;
			}
			else
			{
				Dictionary<int, NPCSaleItem> dictionary = new Dictionary<int, NPCSaleItem>();
				foreach (XElement xml in enumerable)
				{
					int key = (int)Global.GetSafeAttributeLong(xml, "SaleType");
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "Items");
					string[] array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						if (array2.Length != 5)
						{
							LogManager.WriteLog(2, string.Format("加载NPC出售列表时, 物品配置项个数错误，忽略。{0}", array[i]), null, true);
						}
						else
						{
							XElement xml2 = null;
							try
							{
								xml2 = Global.GetSafeXElement(Global.XmlInfo["Configgoods"], "Item", "ID", array2[0]);
							}
							catch (Exception)
							{
								LogManager.WriteLog(2, string.Format("加载NPC出售列表时, 物品不存在，忽略。GoodsID={0}", array[0]), null, true);
								goto IL_274;
							}
							int key2 = (int)Global.GetSafeAttributeLong(xml2, "ID");
							NPCSaleItem npcsaleItem = null;
							if (!dictionary.TryGetValue(key2, out npcsaleItem))
							{
								npcsaleItem = new NPCSaleItem
								{
									Money1Price = (int)Global.GetSafeAttributeLong(xml2, "PriceOne"),
									YinLiangPrice = (int)Global.GetSafeAttributeLong(xml2, "PriceTwo"),
									TianDiJingYuanPrice = (int)Global.GetSafeAttributeLong(xml2, "JinYuanPrice"),
									LieShaZhiPrice = (int)Global.GetSafeAttributeLong(xml2, "LieShaPrice"),
									JiFenPrice = (int)Global.GetSafeAttributeLong(xml2, "JiFenPrice"),
									ZhanHunPrice = (int)Global.GetSafeAttributeLong(xml2, "ZhanHunPrice"),
									Forge_level = Math.Max(0, Global.SafeConvertToInt32(array2[1])),
									AppendPropLev = Math.Max(0, Global.SafeConvertToInt32(array2[2])),
									Lucky = Math.Max(0, Global.SafeConvertToInt32(array2[3])),
									ExcellenceInfo = Math.Max(0, Global.SafeConvertToInt32(array2[4]))
								};
							}
							npcsaleItem.SaleTypesDict[key] = true;
							dictionary[key2] = npcsaleItem;
						}
						IL_274:;
					}
				}
				this._SaleIDSDict = dictionary;
				result = true;
			}
			return result;
		}

		public bool ReloadSaleList()
		{
			try
			{
				return this.LoadSaleList();
			}
			catch (Exception)
			{
			}
			return false;
		}

		private Dictionary<int, NPCSaleItem> _SaleIDSDict = null;
	}
}
