using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	public class SystemParamsList
	{
		public string GetParamValueByName(string name)
		{
			string result;
			if (null == this._ParamsDict)
			{
				result = "";
			}
			else
			{
				string text = null;
				Dictionary<string, string> paramsDict = this._ParamsDict;
				paramsDict.TryGetValue(name, out text);
				result = text;
			}
			return result;
		}

		public long GetParamValueIntByName(string name, int defvalue = -1)
		{
			string paramValueByName = this.GetParamValueByName(name);
			long result;
			if (string.IsNullOrEmpty(paramValueByName))
			{
				result = (long)defvalue;
			}
			else
			{
				try
				{
					return Convert.ToInt64(paramValueByName);
				}
				catch (Exception)
				{
				}
				result = (long)defvalue;
			}
			return result;
		}

		public long GetParamValueIntByName(string name, long defvalue)
		{
			string paramValueByName = this.GetParamValueByName(name);
			long result;
			if (string.IsNullOrEmpty(paramValueByName))
			{
				result = defvalue;
			}
			else
			{
				try
				{
					return Convert.ToInt64(paramValueByName);
				}
				catch (Exception)
				{
				}
				result = defvalue;
			}
			return result;
		}

		public int[] GetParamValueIntArrayByName(string name, char separator = ',')
		{
			string paramValueByName = this.GetParamValueByName(name);
			int[] result;
			if (string.IsNullOrEmpty(paramValueByName))
			{
				result = null;
			}
			else
			{
				try
				{
					return Global.String2IntArray(paramValueByName, separator);
				}
				catch (Exception)
				{
				}
				result = null;
			}
			return result;
		}

		public List<string> GetParamValueStringListByName(string name, char spliteChar = ',')
		{
			string paramValueByName = this.GetParamValueByName(name);
			List<string> result;
			if (string.IsNullOrEmpty(paramValueByName))
			{
				result = null;
			}
			else
			{
				try
				{
					return Global.StringToList(paramValueByName, spliteChar);
				}
				catch (Exception)
				{
				}
				result = null;
			}
			return result;
		}

		public double GetParamValueDoubleByName(string name, double defvalue = 0.0)
		{
			string paramValueByName = this.GetParamValueByName(name);
			double result;
			if (string.IsNullOrEmpty(paramValueByName))
			{
				result = defvalue;
			}
			else
			{
				try
				{
					return Convert.ToDouble(paramValueByName);
				}
				catch (Exception)
				{
				}
				result = defvalue;
			}
			return result;
		}

		public double[] GetParamValueDoubleArrayByName(string name, char separator = ',')
		{
			string paramValueByName = this.GetParamValueByName(name);
			double[] result;
			if (string.IsNullOrEmpty(paramValueByName))
			{
				result = null;
			}
			else
			{
				try
				{
					return Global.String2DoubleArray(paramValueByName, separator);
				}
				catch (Exception)
				{
				}
				result = null;
			}
			return result;
		}

		public void LoadParamsList()
		{
			string text = string.Format("Config/SystemParams.xml", new object[0]);
			XElement xelement = XElement.Load(Global.GameResPath(text));
			if (null == xelement)
			{
				throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
			}
			IEnumerable<XElement> enumerable = xelement.Elements("Params").Elements<XElement>();
			if (null != enumerable)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (XElement xml in enumerable)
				{
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "Name");
					string safeAttributeStr2 = Global.GetSafeAttributeStr(xml, "Value");
					dictionary[safeAttributeStr] = safeAttributeStr2;
				}
				this._ParamsDict = dictionary;
				double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhuanShengExpXiShu", ',');
				if (null != paramValueDoubleArrayByName)
				{
					for (int i = 0; i < paramValueDoubleArrayByName.Length; i++)
					{
						Data.ChangeLifeEverydayExpRate.Add(i, paramValueDoubleArrayByName[i]);
					}
				}
			}
		}

		public int ReloadLoadParamsList()
		{
			try
			{
				this.LoadParamsList();
			}
			catch (Exception)
			{
				return -1;
			}
			return 0;
		}

		private Dictionary<string, string> _ParamsDict = null;
	}
}
