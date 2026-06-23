using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HSGameEngine.GameEngine.Common;
using Tmsk.Xml;
using UnityEngine;

namespace HSGameEngine.GameEngine.Logic
{
	public class XmlManager
	{
		public static string GetOnlyFileName(string xmlName)
		{
			if (string.IsNullOrEmpty(xmlName))
			{
				return null;
			}
			int num = xmlName.LastIndexOf("/");
			num++;
			int num2 = xmlName.LastIndexOf(".");
			if (num2 < 0)
			{
				num2 = xmlName.Length;
			}
			return xmlName.Substring(num, num2 - num);
		}

		private static void Init()
		{
			if (XmlManager.HuanCunMuLuDic.Count == 0)
			{
			}
			if (XmlManager.NotCacheXMLDict.Count == 0)
			{
				XmlManager.NotCacheXMLDict.Add(XmlManager.GetOnlyFileName("Config/Goods.Xml"), 0);
				XmlManager.NotCacheXMLDict.Add(XmlManager.GetOnlyFileName("Config/Monsters.Xml"), 0);
				XmlManager.NotCacheXMLDict.Add(XmlManager.GetOnlyFileName("Config/npcs.Xml"), 0);
				XmlManager.NotCacheXMLDict.Add(XmlManager.GetOnlyFileName("Config/SystemTasks.Xml"), 0);
				XmlManager.NotCacheXMLDict.Add(XmlManager.GetOnlyFileName("Config/Magics/Magics_0.xml"), 0);
				XmlManager.NotCacheXMLDict.Add(XmlManager.GetOnlyFileName("Config/Magics/Magics_1.xml"), 0);
				XmlManager.NotCacheXMLDict.Add(XmlManager.GetOnlyFileName("Config/Magics/Magics_2.xml"), 0);
				XmlManager.NotCacheXMLDict.Add(XmlManager.GetOnlyFileName("Config/Magics/Magics_3.xml"), 0);
				XmlManager.NotCacheXMLDict.Add(XmlManager.GetOnlyFileName("Config/Magics/Magics_5.xml"), 0);
			}
		}

		public static XElement GetResXml(string resName, string xmlName)
		{
			XmlManager.Init();
			xmlName = XmlManager.GetOnlyFileName(xmlName);
			bool flag = true;
			if (XmlManager.NotCacheXMLDict.ContainsKey(xmlName))
			{
				flag = false;
			}
			try
			{
				XElement xelement = XmlManager.GetXElement(xmlName);
				if (xelement != null)
				{
					return xelement;
				}
				AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(resName);
				if (null == assetBundle)
				{
					GError.AddErrMsg(string.Format("GetResXml异常, 缓存中没找到 {0}", resName));
					return null;
				}
				TextAsset textAsset = assetBundle.LoadAsset(xmlName) as TextAsset;
				if (null == textAsset)
				{
					GError.AddErrMsg(string.Format("GetResXml异常, 从缓存获取 {0}后，解析: {1} 失败", resName, xmlName));
					return null;
				}
				xelement = XElement.Parse(textAsset.text);
				if (flag)
				{
					XmlManager.AddXElement(xmlName, xelement);
				}
				return xelement;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			finally
			{
			}
			return null;
		}

		public static XElement GetGameResXml(string xmlName)
		{
			XElement resXml = XmlManager.GetResXml("GameRes", xmlName);
			if (resXml == null)
			{
				resXml = XmlManager.GetResXml("GameRes_VO", xmlName);
			}
			return resXml;
		}

		public static XElement GetLoginResXml(string xmlName)
		{
			return XmlManager.GetResXml("LoginRes", xmlName);
		}

		public static XElement GetIsolateResXml(string xmlName)
		{
			XElement resXml = XmlManager.GetResXml("IsolateRes", xmlName);
			if (resXml == null)
			{
				resXml = XmlManager.GetResXml("IsolateRes_VO", xmlName);
			}
			return resXml;
		}

		public static XElement GetLangResXml(string xmlName)
		{
			return XmlManager.GetResXml("IsolateRes", xmlName);
		}

		public static XElement GetGameMapXml(int mapPicCode, string xmlName)
		{
			xmlName = XmlManager.GetOnlyFileName(xmlName);
			try
			{
				string text = string.Format("MapConfig{0}", mapPicCode);
				XElement xelement = XmlManager.GetXElement(text);
				if (xelement != null)
				{
					return xelement;
				}
				AssetBundle currentMapLoader = AssetBundleManager.CurrentMapLoader;
				if (null == currentMapLoader)
				{
					GError.AddErrMsg(string.Format("GetGameMapXml异常, 缓存中没找到 {0}", text));
					return null;
				}
				TextAsset textAsset = currentMapLoader.LoadAsset(xmlName) as TextAsset;
				if (null == textAsset)
				{
					GError.AddErrMsg(string.Format("GetGameMapXml异常, 从缓存获取 {0}后，解析: {1} 失败", text, xmlName));
					return null;
				}
				xelement = XElement.Parse(textAsset.text);
				XmlManager.AddXElement(xmlName, xelement);
				return xelement;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			finally
			{
			}
			return null;
		}

		public static XElement GetGameMapSettingsXml(int mapCode, string xmlName)
		{
			xmlName = XmlManager.GetOnlyFileName(xmlName);
			try
			{
				string text = string.Format("Map{0}{1}", mapCode, xmlName);
				xmlName = string.Format("{0}{1}", mapCode, xmlName);
				XElement xelement = XmlManager.GetXElement(text);
				if (xelement != null)
				{
					return xelement;
				}
				AssetBundle currentMapSettingsLoader = AssetBundleManager.CurrentMapSettingsLoader;
				if (null == currentMapSettingsLoader)
				{
					GError.AddErrMsg(string.Format("GetGameMapSettingsXml异常, 缓存中没找到 {0}", text));
					return null;
				}
				TextAsset textAsset = currentMapSettingsLoader.LoadAsset(xmlName) as TextAsset;
				if (null == textAsset)
				{
					GError.AddErrMsg(new Exception(string.Format("GetGameMapSettingsXml异常, 从缓存获取 {0}后，解析: {1} 失败", text, xmlName)));
					return null;
				}
				xelement = XElement.Parse(textAsset.text);
				XmlManager.AddXElement(text, xelement);
				return xelement;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			finally
			{
			}
			return null;
		}

		public static void AddXElement(string key, XElement element)
		{
			if (!XmlManager.XmlInfo.ContainsKey(key))
			{
				XmlManager.XmlInfo.Add(key, element);
			}
		}

		public static void RemoveXElement(string key)
		{
			if (XmlManager.XmlInfo.ContainsKey(key))
			{
				XmlManager.XmlInfo.Remove(key);
			}
		}

		public static void SaveXElementToFile(string path, XElement root)
		{
			using (StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8))
			{
				streamWriter.Write(root.ToString());
			}
		}

		public static XElement GetXElement(string key)
		{
			XElement result = null;
			if (!XmlManager.XmlInfo.TryGetValue(key, ref result))
			{
				return null;
			}
			return result;
		}

		public static void ClearPartXml()
		{
			string[] array = new string[XmlManager.XmlInfo.Keys.Count];
			XmlManager.XmlInfo.Keys.CopyTo(array, 0);
			foreach (string text in array)
			{
				if (!XmlManager.HuanCunMuLuDic.ContainsKey(text))
				{
					XmlManager.XmlInfo.Remove(text);
				}
			}
		}

		public static string GetXElementNodePath(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			try
			{
				string text = element.Name.ToString();
				for (element = (XElement)element.Parent; element != null; element = (XElement)element.Parent)
				{
					text = element.Name.ToString() + "/" + text;
				}
				return text;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			return null;
		}

		public static XElement GetXElement(XElement XElement, string newroot)
		{
			if (XElement == null)
			{
				return null;
			}
			IEnumerable<XElement> enumerable = XElement.DescendantsAndSelf(newroot);
			if (enumerable == null)
			{
				return null;
			}
			IEnumerator<XElement> enumerator = enumerable.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				return null;
			}
			return enumerator.Current;
		}

		public static List<int> GetXElementAttributeIntList(XElement xElement, string attributeName, string newroot = "*")
		{
			List<int> list = new List<int>();
			if (xElement != null)
			{
				IEnumerable<XElement> enumerable;
				if ("*" == newroot)
				{
					enumerable = xElement.Elements();
				}
				else
				{
					enumerable = xElement.DescendantsAndSelf(newroot);
				}
				if (enumerable != null)
				{
					foreach (XElement xelement in enumerable)
					{
						list.Add(XmlManager.GetXElementAttributeInt(xelement, attributeName));
					}
				}
			}
			return list;
		}

		public static int[] GetXElementAttributeIntArray(XElement xElement, string attributeName, string newroot = "*")
		{
			return XmlManager.GetXElementAttributeIntList(xElement, attributeName, newroot).ToArray();
		}

		public static List<XElement> GetXElementList(XElement xElement, string newroot)
		{
			List<XElement> list = new List<XElement>();
			if (xElement != null)
			{
				IEnumerable<XElement> enumerable;
				if ("*" == newroot)
				{
					enumerable = xElement.Elements();
				}
				else
				{
					enumerable = xElement.DescendantsAndSelf(newroot);
				}
				if (enumerable != null)
				{
					foreach (XElement xelement in enumerable)
					{
						list.Insert(list.Count, xelement);
					}
				}
			}
			return list;
		}

		public static XElement GetXElement(XElement XElement, string newroot, string attribute, string value)
		{
			if (XElement != null)
			{
				IEnumerable<XElement> enumerable = XElement.DescendantsAndSelf(newroot);
				if (enumerable != null)
				{
					foreach (XElement xelement in enumerable)
					{
						XAttribute xattribute = null;
						if (xelement != null)
						{
							xattribute = xelement.Attribute(attribute);
						}
						if (xattribute != null && xattribute.Value == value)
						{
							return xelement;
						}
					}
				}
				return null;
			}
			return null;
		}

		public static XElement GetXElement(XElement XElement, string newroot, string attribute1, string value1, string attribute2, string value2)
		{
			if (XElement != null)
			{
				IEnumerable<XElement> enumerable = XElement.DescendantsAndSelf(newroot);
				foreach (XElement xelement in enumerable)
				{
					XAttribute xattribute = xelement.Attribute(attribute1);
					XAttribute xattribute2 = xelement.Attribute(attribute2);
					if (xattribute != null && xattribute.Value == value1 && xattribute2 != null && xattribute2.Value == value2)
					{
						return xelement;
					}
				}
				return null;
			}
			return null;
		}

		public static XAttribute GetAttribute(XElement XElement, string attribute)
		{
			if (XElement == null)
			{
				return null;
			}
			XAttribute result;
			try
			{
				XAttribute xattribute = XElement.Attribute(attribute);
				if (xattribute == null)
				{
					GError.AddErrMsg(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, XmlManager.GetXElementNodePath(XElement)));
					result = null;
				}
				else
				{
					result = xattribute;
				}
			}
			catch (Exception ex)
			{
				GError.AddErrMsg(string.Format("读取属性: {0} 失败, xml节点名: {1}", attribute, XmlManager.GetXElementNodePath(XElement)));
				MUDebug.LogException(ex);
				result = null;
			}
			finally
			{
			}
			return result;
		}

		public static string GetXElementAttributeStr(XElement XElement, string attributeName)
		{
			XAttribute attribute = XmlManager.GetAttribute(XElement, attributeName);
			if (attribute == null)
			{
				return string.Empty;
			}
			return (string)attribute;
		}

		public static int GetXElementAttributeInt(XElement XElement, string attributeName)
		{
			XAttribute attribute = XmlManager.GetAttribute(XElement, attributeName);
			if (attribute == null)
			{
				return -1;
			}
			string text = (string)attribute;
			if (text == null || text == string.Empty)
			{
				return -1;
			}
			int result = 0;
			if (int.TryParse(text, ref result))
			{
				return result;
			}
			return -1;
		}

		public static long GetXElementAttributeLong(XElement XElement, string attributeName)
		{
			XAttribute attribute = XmlManager.GetAttribute(XElement, attributeName);
			if (attribute == null)
			{
				return -1L;
			}
			string text = (string)attribute;
			if (text == null || text == string.Empty)
			{
				return -1L;
			}
			long result = 0L;
			if (long.TryParse(text, ref result))
			{
				return result;
			}
			return -1L;
		}

		public static double GetXElementAttributeDouble(XElement XElement, string attributeName)
		{
			XAttribute attribute = XmlManager.GetAttribute(XElement, attributeName);
			if (attribute == null)
			{
				return -1.0;
			}
			string text = (string)attribute;
			if (text == null || text == string.Empty)
			{
				return -1.0;
			}
			double result = 0.0;
			if (double.TryParse(text, ref result))
			{
				return result;
			}
			return -1.0;
		}

		public static float GetXElementAttributeFloat(XElement XElement, string attributeName)
		{
			XAttribute attribute = XmlManager.GetAttribute(XElement, attributeName);
			if (attribute == null)
			{
				return -1f;
			}
			string text = (string)attribute;
			if (text == null || text == string.Empty)
			{
				return -1f;
			}
			float result = 0f;
			if (float.TryParse(text, ref result))
			{
				return result;
			}
			return -1f;
		}

		public static Dictionary<string, XElement> XmlInfo = new Dictionary<string, XElement>();

		private static Dictionary<string, byte> HuanCunMuLuDic = new Dictionary<string, byte>();

		private static Dictionary<string, byte> NotCacheXMLDict = new Dictionary<string, byte>();
	}
}
