using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Tmsk.Xml
{
	public class XElement : XmlNode
	{
		public XElement()
		{
		}

		public XElement(string eleName)
		{
			base.Name = eleName;
		}

		public static XElement Load(string uri)
		{
			if (File.Exists(uri))
			{
				string text = File.ReadAllText(uri);
				return XElement.Parse(text);
			}
			return null;
		}

		public static XElement Parse(string text)
		{
			return XmlParser.Parse(text);
		}

		public static XElement Parse(byte[] bytes)
		{
			XElement result = null;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(bytes))
				{
					using (StreamReader streamReader = new StreamReader(memoryStream, Encoding.UTF8, true))
					{
						string strContent = streamReader.ReadToEnd();
						result = XmlParser.Parse(strContent);
					}
				}
			}
			catch
			{
			}
			return result;
		}

		public void Add(XElement ele)
		{
			if (ele != null)
			{
				ele.Parent = this;
				base.AppendChild(ele);
			}
		}

		public void Remove()
		{
			if (this.parent != null)
			{
				this.parent.RemoveChild(this);
			}
			this.parent = null;
		}

		public XElement ElementEx(string name)
		{
			foreach (XElement xelement in this.DescendantsAndSelf())
			{
				if (xelement != null && xelement.Name == name)
				{
					return xelement;
				}
			}
			return null;
		}

		public XElement Element(string name)
		{
			foreach (XElement xelement in this.Elements())
			{
				if (xelement.Name == name)
				{
					return xelement;
				}
			}
			return null;
		}

		public IEnumerable<XElement> Elements()
		{
			if (this.children != null)
			{
				foreach (XmlNode item in this.children)
				{
					if (item != null)
					{
						XElement element = item as XElement;
						if (element != null)
						{
							yield return element;
						}
					}
				}
			}
			yield break;
		}

		public IEnumerable<XElement> Elements(string name)
		{
			foreach (XElement item in this.Elements())
			{
				if (item != null && item.Name == name)
				{
					yield return item;
				}
			}
			yield break;
		}

		public List<XElement> ElementsList(string name)
		{
			List<XElement> list = new List<XElement>();
			foreach (XElement xelement in this.DescendantsAndSelf())
			{
				if (xelement != null && xelement.Name == name)
				{
					list.Add(xelement);
				}
			}
			return list;
		}

		public IEnumerable<XElement> DescendantsAndSelf()
		{
			yield return this;
			if (this.children != null)
			{
				foreach (XmlNode item in this.children)
				{
					if (item != null)
					{
						XElement element = item as XElement;
						if (element != null)
						{
							foreach (XElement item2 in element.DescendantsAndSelf())
							{
								yield return item2;
							}
						}
					}
				}
			}
			yield break;
		}

		public IEnumerable<XElement> DescendantsAndSelf(string name)
		{
			foreach (XElement item in this.DescendantsAndSelf())
			{
				if (item != null && item.Name == name)
				{
					yield return item;
				}
			}
			yield break;
		}

		public virtual XmlNodeList GetElementsByTagName(string name)
		{
			ArrayList arrayList = new ArrayList();
			if (name != null && this.children != null)
			{
				for (int i = 0; i < this.children.Count; i++)
				{
					XmlNode xmlNode = this.children[i];
					if (xmlNode != null)
					{
						if (name.Equals(xmlNode.Name))
						{
							arrayList.Add(xmlNode);
						}
					}
				}
			}
			return new XmlNodeArrayList(arrayList);
		}

		public bool HasAttribute()
		{
			return this._firstAttribute != null;
		}

		public bool HasAttribute(string name)
		{
			return null != this.Attribute(name);
		}

		public IEnumerable<XAttribute> Attributes()
		{
			XAttribute next;
			for (XAttribute a = this._firstAttribute; a != null; a = next)
			{
				next = a.NextAttribute;
				yield return a;
			}
			yield break;
		}

		public IEnumerable<XAttribute> Attributes(string name)
		{
			foreach (XAttribute a in this.Attributes())
			{
				if (a.name == name)
				{
					yield return a;
				}
			}
			yield break;
		}

		public XAttribute Attribute(string name)
		{
			foreach (XAttribute xattribute in this.Attributes())
			{
				if (xattribute.name == name)
				{
					return xattribute;
				}
			}
			return null;
		}

		public void AppendAttribute(string name, string value)
		{
			XAttribute xattribute = new XAttribute();
			xattribute.name = name;
			xattribute.value = value;
			if (this._firstAttribute == null)
			{
				this._firstAttribute = xattribute;
				this._lastAttribute = xattribute;
			}
			else
			{
				this._lastAttribute.next = xattribute;
				xattribute.previous = this._lastAttribute;
				this._lastAttribute = xattribute;
			}
		}

		public void SetAttribute(string name, string value)
		{
			this.SetAttributeValue(name, value);
		}

		public void SetAttributeValue(string name, string value)
		{
			if (name == null)
			{
				return;
			}
			XAttribute xattribute = this.Attribute(name);
			if (xattribute != null)
			{
				if (value == null)
				{
					xattribute.Remove();
				}
				else
				{
					xattribute.value = value;
				}
			}
			else
			{
				xattribute = new XAttribute();
				xattribute.name = name;
				xattribute.value = value;
				if (this._firstAttribute == null)
				{
					this._firstAttribute = xattribute;
					this._lastAttribute = xattribute;
				}
				else
				{
					this._lastAttribute.NextAttribute = xattribute;
					xattribute.PreviousAttribute = this._lastAttribute;
					this._lastAttribute = xattribute;
				}
			}
		}

		public void RemoveAttribute(string name)
		{
			XAttribute xattribute = this.Attribute(name);
			if (xattribute == null)
			{
				return;
			}
			if (xattribute == this._firstAttribute || xattribute == this._lastAttribute)
			{
				if (xattribute == this._lastAttribute)
				{
					this._lastAttribute = xattribute.PreviousAttribute;
					if (this._lastAttribute != null)
					{
						this._lastAttribute.NextAttribute = null;
					}
				}
				if (xattribute == this._firstAttribute)
				{
					this._firstAttribute = xattribute.NextAttribute;
					if (this._firstAttribute != null)
					{
						this._firstAttribute.PreviousAttribute = null;
					}
				}
				xattribute.NextAttribute = null;
				xattribute.PreviousAttribute = null;
			}
			else
			{
				xattribute.Remove();
			}
		}

		public void RemoveAllAttributes()
		{
			while (this._firstAttribute != null)
			{
				this._firstAttribute.Remove();
			}
		}

		public void GetAttributeNames(List<string> list)
		{
			foreach (XAttribute xattribute in this.Attributes())
			{
				if (xattribute != null)
				{
					list.Add(xattribute.name);
				}
			}
		}

		private void GetAttributeString(StringBuilder sb)
		{
			foreach (XAttribute xattribute in this.Attributes())
			{
				if (xattribute != null)
				{
					xattribute.GetString(sb);
				}
			}
		}

		private void GetElementString(StringBuilder sb, string pading)
		{
			sb.Append(pading + "<");
			sb.Append(base.Name);
			this.GetAttributeString(sb);
			if (this.children == null || this.children.Count <= 0)
			{
				sb.Append("/>\r\n");
				return;
			}
			sb.Append(">\r\n");
			string pading2 = pading + '\t';
			for (int i = 0; i < this.children.Count; i++)
			{
				XmlNode xmlNode = this.children[i];
				if (xmlNode != null)
				{
					(xmlNode as XElement).GetElementString(sb, pading2);
				}
			}
			sb.Append(pading);
			sb.Append(string.Format("</{0}>\r\n", base.Name));
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.GetElementString(stringBuilder, string.Empty);
			return stringBuilder.ToString();
		}

		public string GetAttributeStr(string attName)
		{
			try
			{
				XAttribute xattribute = this.Attribute(attName);
				if (xattribute == null || xattribute.value == null)
				{
					return string.Empty;
				}
				return xattribute.value;
			}
			catch
			{
			}
			return string.Empty;
		}

		public int GetAttributeInt(string attName, int def = -1)
		{
			try
			{
				XAttribute xattribute = this.Attribute(attName);
				if (xattribute == null)
				{
					return def;
				}
				return int.Parse(xattribute.value, 7, CultureInfo.InvariantCulture);
			}
			catch
			{
			}
			return def;
		}

		public long GetAttributeLong(string attName, long def = -1L)
		{
			try
			{
				XAttribute xattribute = this.Attribute(attName);
				if (xattribute == null || xattribute.value == null || xattribute.value == string.Empty)
				{
					return def;
				}
				long result = def;
				if (long.TryParse(xattribute.value, ref result))
				{
					return result;
				}
			}
			catch
			{
			}
			return def;
		}

		public float GetAttributeFloat(string attName, float def = -1f)
		{
			try
			{
				XAttribute xattribute = this.Attribute(attName);
				if (xattribute == null)
				{
					return def;
				}
				return float.Parse(xattribute.value, 167, CultureInfo.InvariantCulture);
			}
			catch
			{
			}
			return def;
		}

		public double GetAttributeDouble(string attName, double def = -1.0)
		{
			try
			{
				XAttribute xattribute = this.Attribute(attName);
				if (xattribute == null || xattribute.value == null || xattribute.value == string.Empty)
				{
					return def;
				}
				double result = def;
				if (double.TryParse(xattribute.value, ref result))
				{
					return result;
				}
			}
			catch
			{
			}
			return def;
		}

		public int[] GetAttributeIntArray(string attributeName, char ch = ',')
		{
			XAttribute xattribute = this.Attribute(attributeName);
			if (xattribute == null || xattribute.value == null || xattribute.value == string.Empty)
			{
				return new int[0];
			}
			return XElement.String2IntArray(xattribute.value, ch);
		}

		public static int[] String2IntArray(string str, char ch)
		{
			if (str.Trim() == string.Empty)
			{
				return null;
			}
			string[] array = str.Split(new char[]
			{
				ch
			});
			if (array == null)
			{
				return null;
			}
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i].Trim() == string.Empty))
				{
					array2[i] = XElement.SafeConvertToInt32(array[i]);
				}
			}
			return array2;
		}

		public static int SafeConvertToInt32(string str)
		{
			str = str.Trim();
			int result = 0;
			if (int.TryParse(str, ref result))
			{
				return result;
			}
			return 0;
		}

		public bool ParseInt(string attName, ref int ret)
		{
			ret = -1;
			try
			{
				XAttribute xattribute = this.Attribute(attName);
				if (xattribute == null)
				{
					return false;
				}
				ret = int.Parse(xattribute.value, 7, CultureInfo.InvariantCulture);
				return true;
			}
			catch
			{
			}
			return false;
		}

		public bool ParseBool(string attName, ref bool ret)
		{
			ret = false;
			try
			{
				XAttribute xattribute = this.Attribute(attName);
				if (xattribute == null)
				{
					return false;
				}
				ret = (bool)xattribute;
				return true;
			}
			catch
			{
			}
			return false;
		}

		public bool ParseFloat(string attName, ref float ret)
		{
			ret = -1f;
			try
			{
				XAttribute xattribute = this.Attribute(attName);
				if (xattribute == null)
				{
					return false;
				}
				ret = float.Parse(xattribute.value, 167, CultureInfo.InvariantCulture);
				return true;
			}
			catch
			{
			}
			return false;
		}

		public bool ParseStr(string attName, ref string ret)
		{
			ret = string.Empty;
			try
			{
				XAttribute xattribute = this.Attribute(attName);
				if (xattribute == null || xattribute.value == null)
				{
					return false;
				}
				ret = xattribute.value;
				return true;
			}
			catch
			{
			}
			return false;
		}

		public bool Save(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				XmlDeclaration node = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
				xmlDocument.AppendChild(node);
				xmlDocument.AppendChild(this);
				xmlDocument.Save(path);
			}
			catch
			{
				return false;
			}
			return true;
		}

		private const NumberStyles integerStyle = 7;

		private const NumberStyles floatStyle = 167;

		private XAttribute _firstAttribute;

		private XAttribute _lastAttribute;
	}
}
