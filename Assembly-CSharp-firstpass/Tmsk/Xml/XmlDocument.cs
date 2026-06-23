using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tmsk.Xml
{
	public class XmlDocument
	{
		public void LoadXml(string text)
		{
			this._docNode = XElement.Parse(text);
		}

		public XElement DocumentElement
		{
			get
			{
				return this._docNode;
			}
		}

		public XmlDeclaration CreateXmlDeclaration(string ver, string enc, string standalone)
		{
			return new XmlDeclaration(ver, enc, standalone);
		}

		public void AppendChild(XmlNode node)
		{
			this.children.Add(node);
		}

		public XElement CreateElement(string name)
		{
			return new XElement(name);
		}

		public bool Save(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.children.Count; i++)
				{
					stringBuilder.Append(this.children[i].ToString());
				}
				byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
				File.WriteAllBytes(path, bytes);
			}
			catch
			{
				return false;
			}
			return true;
		}

		private List<XmlNode> children = new List<XmlNode>();

		private XElement _docNode;
	}
}
