using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Tmsk.Tools.Tools;

namespace KF.Remoting.Data
{
	public class SystemParamsListKF
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

		public int[] GetParamValueIntArrayByName(string name)
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
					return ConfigHelper.String2IntArray(paramValueByName, ',');
				}
				catch (Exception)
				{
				}
				result = null;
			}
			return result;
		}

		public double GetParamValueDoubleByName(string name)
		{
			string paramValueByName = this.GetParamValueByName(name);
			double result;
			if (string.IsNullOrEmpty(paramValueByName))
			{
				result = 0.0;
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
				result = 0.0;
			}
			return result;
		}

		public void LoadParamsList()
		{
			string text = string.Format("Config/SystemParams.xml", new object[0]);
			XElement xelement = XElement.Load(KuaFuServerManager.GetResourcePath(text, KuaFuServerManager.ResourcePathTypes.GameRes));
			if (null == xelement)
			{
				throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
			}
			IEnumerable<XElement> enumerable = xelement.Elements("Params").Elements<XElement>();
			if (null != enumerable)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (XElement xelement2 in enumerable)
				{
					string elementAttributeValue = ConfigHelper.GetElementAttributeValue(xelement2, "Name", "");
					string elementAttributeValue2 = ConfigHelper.GetElementAttributeValue(xelement2, "Value", "");
					dictionary[elementAttributeValue] = elementAttributeValue2;
				}
				this._ParamsDict = dictionary;
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
