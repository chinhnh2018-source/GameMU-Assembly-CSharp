using System;
using System.Collections.Generic;

namespace Tmsk.Xml
{
	public class XmlNode
	{
		public XmlNode Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		public List<XmlNode> Children
		{
			get
			{
				return this.children;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		public NodeType NodeType { get; set; }

		public void AppendChild(XmlNode node)
		{
			if (this.children == null)
			{
				this.children = new List<XmlNode>();
			}
			this.children.Add(node);
		}

		public void RemoveChild(XmlNode child)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				if (child == this.children[i])
				{
					this.children[i] = null;
				}
			}
		}

		protected XmlNode parent;

		protected List<XmlNode> children;

		private string _name = string.Empty;

		private string _value = string.Empty;
	}
}
