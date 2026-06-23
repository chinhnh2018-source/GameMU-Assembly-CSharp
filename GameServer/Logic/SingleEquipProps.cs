using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	public class SingleEquipProps
	{
		public List<double[]> GetSingleEquipPropsList(int occupation, int categoriy, int suitID)
		{
			List<double[]> result;
			if (null == this._SingleEquipItemsDict)
			{
				result = null;
			}
			else
			{
				string key = string.Format("{0}_{1}_{2}", occupation, categoriy, suitID);
				List<double[]> list = null;
				if (!this._SingleEquipItemsDict.TryGetValue(key, out list))
				{
					result = null;
				}
				else
				{
					result = list;
				}
			}
			return result;
		}

		private List<double[]> ParseSystemXmlItem(SystemXmlItem xmlItem)
		{
			string stringValue = xmlItem.GetStringValue("EquipProps");
			List<double[]> result;
			if (string.IsNullOrEmpty(stringValue))
			{
				result = null;
			}
			else
			{
				string[] array = stringValue.Split(new char[]
				{
					'|'
				});
				if (array == null || array.Length <= 0)
				{
					result = null;
				}
				else
				{
					List<double[]> list = new List<double[]>();
					for (int i = 0; i < array.Length; i++)
					{
						list.Add(this.ParseStringProps(array[i]));
					}
					result = list;
				}
			}
			return result;
		}

		private double[] ParseStringProps(string props)
		{
			double[] result;
			if (string.IsNullOrEmpty(props))
			{
				result = null;
			}
			else
			{
				double[] array = Global.String2DoubleArray(props, ',');
				if (array == null || array.Length != 10)
				{
					result = null;
				}
				else
				{
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = Global.GMax(0.0, array[i]);
					}
					result = array;
				}
			}
			return result;
		}

		private void LoadEquipPropItemsByOccupation(string pathName, int occupation)
		{
			XElement xelement = null;
			string text = "";
			try
			{
				text = string.Format("{0}{1}.xml", pathName, occupation);
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
			IEnumerable<XElement> enumerable = xelement.Elements("Equip");
			foreach (XElement xelement2 in enumerable)
			{
				IEnumerable<XElement> enumerable2 = xelement2.Elements("Item");
				foreach (XElement xelement3 in enumerable2)
				{
					SystemXmlItem xmlItem = new SystemXmlItem
					{
						XMLNode = xelement3
					};
					string key = string.Format("{0}_{1}_{2}", occupation, (int)Global.GetSafeAttributeLong(xelement2, "Categoriy"), (int)Global.GetSafeAttributeLong(xelement3, "SuitID"));
					this._SingleEquipItemsDict[key] = this.ParseSystemXmlItem(xmlItem);
				}
			}
		}

		public void LoadEquipPropItems(string pathName)
		{
			this.LoadEquipPropItemsByOccupation(pathName, 0);
			this.LoadEquipPropItemsByOccupation(pathName, 1);
			this.LoadEquipPropItemsByOccupation(pathName, 2);
		}

		private Dictionary<string, List<double[]>> _SingleEquipItemsDict = new Dictionary<string, List<double[]>>();
	}
}
