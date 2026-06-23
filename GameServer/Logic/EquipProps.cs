using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameServer.Logic
{
	public class EquipProps
	{
		public void ParseEquipProps(SystemXmlItem systemGoods, out EquipPropItem equipPropItem)
		{
			equipPropItem = null;
			string stringValue = systemGoods.GetStringValue("EquipProps");
			string[] array = stringValue.Split(new char[]
			{
				','
			});
			if (array.Length != 177)
			{
				LogManager.WriteLog(1000, string.Format("解析物品属性失败: EquipID={0},EquipProps属性期望个数{1}，实际个数{2}", systemGoods.GetIntValue("ID", -1), 177, array.Length), null, true);
			}
			double[] array2 = null;
			try
			{
				array2 = Global.StringArray2DoubleArray(array);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("转换物品属性数组: EquipID={0}", systemGoods.GetIntValue("ID", -1)), null, true);
				return;
			}
			equipPropItem = new EquipPropItem();
			int num = 0;
			while (num < 177 && num < array2.Length)
			{
				equipPropItem.ExtProps[num] = array2[num];
				num++;
			}
		}

		public void ParseEquipProps(string props, out EquipPropItem equipPropItem)
		{
			equipPropItem = null;
			string[] array = props.Split(new char[]
			{
				','
			});
			if (array.Length != 177)
			{
				LogManager.WriteLog(2, string.Format("解析物品属性失败", new object[0]), null, true);
			}
			else
			{
				double[] array2 = null;
				try
				{
					array2 = Global.StringArray2DoubleArray(array);
				}
				catch (Exception)
				{
					LogManager.WriteLog(2, string.Format("转换物品属性数组", new object[0]), null, true);
					return;
				}
				equipPropItem = new EquipPropItem();
				for (int i = 0; i < 177; i++)
				{
					equipPropItem.ExtProps[i] = array2[i];
				}
			}
		}

		public string EquipPropsToString(double[] ExtProps)
		{
			string text = "";
			string result;
			if (ExtProps == null)
			{
				result = text;
			}
			else
			{
				for (int i = 0; i < ExtProps.Length; i++)
				{
					if (i == 0)
					{
						text += ExtProps[i];
					}
					else
					{
						text += ",";
						text += ExtProps[i];
					}
				}
				result = text;
			}
			return result;
		}

		public EquipPropItem FindEquipPropItem(int equipID)
		{
			EquipPropItem equipPropItem = null;
			lock (this._EquipPropsDict)
			{
				if (this._EquipPropsDict.TryGetValue(equipID, out equipPropItem))
				{
					return equipPropItem;
				}
			}
			SystemXmlItem systemGoods = null;
			EquipPropItem result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(equipID, out systemGoods))
			{
				result = null;
			}
			else
			{
				this.ParseEquipProps(systemGoods, out equipPropItem);
				if (null != equipPropItem)
				{
					lock (this._EquipPropsDict)
					{
						this._EquipPropsDict[equipID] = equipPropItem;
					}
				}
				result = equipPropItem;
			}
			return result;
		}

		public void ClearCachedEquipPropItem()
		{
			lock (this._EquipPropsDict)
			{
				this._EquipPropsDict.Clear();
			}
		}

		private Dictionary<int, EquipPropItem> _EquipPropsDict = new Dictionary<int, EquipPropItem>();
	}
}
