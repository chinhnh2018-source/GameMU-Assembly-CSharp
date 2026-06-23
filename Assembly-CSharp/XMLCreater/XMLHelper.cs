using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class XMLHelper
	{
		public static T InstanceConfig<T>(string path) where T : class
		{
			XElement gameResXml = Global.GetGameResXml(path);
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 " + path + Global.GetLang(" 出现错误")
				});
				return (T)((object)null);
			}
			return (T)((object)Activator.CreateInstance(typeof(T), new object[]
			{
				gameResXml
			}));
		}

		public static XElement LoadXML(string content)
		{
			XElement result = null;
			try
			{
				result = XElement.Parse(content);
			}
			catch
			{
			}
			return result;
		}

		public static string GetStringArrtibute(XElement xe, string attribute)
		{
			XAttribute xattribute = xe.Attribute(attribute);
			if (xattribute == null)
			{
				return string.Empty;
			}
			return xattribute.Value;
		}

		public static int GetIntArrtibute(XElement xe, string attribute)
		{
			int result = 0;
			string stringArrtibute = XMLHelper.GetStringArrtibute(xe, attribute);
			if (int.TryParse(stringArrtibute, ref result))
			{
			}
			return result;
		}

		public static long GetLongArrtibute(XElement xe, string attribute)
		{
			long result = 0L;
			string stringArrtibute = XMLHelper.GetStringArrtibute(xe, attribute);
			if (long.TryParse(stringArrtibute, ref result))
			{
			}
			return result;
		}

		public static float GetFloatArrtibute(XElement xe, string attribute)
		{
			float result = 0f;
			string stringArrtibute = XMLHelper.GetStringArrtibute(xe, attribute);
			if (float.TryParse(stringArrtibute, ref result))
			{
			}
			return result;
		}

		public static double GetDoubleArrtibute(XElement xe, string attribute)
		{
			double result = 0.0;
			string stringArrtibute = XMLHelper.GetStringArrtibute(xe, attribute);
			if (double.TryParse(stringArrtibute, ref result))
			{
			}
			return result;
		}

		public static List<int> GetIntListArrtibute(XElement xe, string attribute, char split = ',')
		{
			List<int> list = new List<int>();
			string stringArrtibute = XMLHelper.GetStringArrtibute(xe, attribute);
			if (stringArrtibute != string.Empty)
			{
				string[] array = stringArrtibute.Split(new char[]
				{
					split
				});
				for (int i = 0; i < array.Length; i++)
				{
					list.Add(array[i].SafeToInt32(0));
				}
			}
			return list;
		}

		public static List<string> GetStringListArrtibute(XElement xe, string attribute, char split = ',')
		{
			List<string> list = new List<string>();
			string stringArrtibute = XMLHelper.GetStringArrtibute(xe, attribute);
			if (stringArrtibute != string.Empty)
			{
				string[] array = stringArrtibute.Split(new char[]
				{
					split
				});
				for (int i = 0; i < array.Length; i++)
				{
					list.Add(array[i]);
				}
			}
			return list;
		}
	}
}
