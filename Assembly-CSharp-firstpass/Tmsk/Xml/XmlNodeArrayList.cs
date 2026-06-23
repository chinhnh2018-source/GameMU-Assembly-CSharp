using System;
using System.Collections;

namespace Tmsk.Xml
{
	public class XmlNodeArrayList : XmlNodeList
	{
		public XmlNodeArrayList(ArrayList list)
		{
			this._rgNodes = list;
		}

		public override int Count
		{
			get
			{
				return this._rgNodes.Count;
			}
		}

		public override XmlNode this[int i]
		{
			get
			{
				if (i < 0 || i >= this._rgNodes.Count)
				{
					return null;
				}
				return (XmlNode)this._rgNodes[i];
			}
		}

		public override IEnumerator GetEnumerator()
		{
			return this._rgNodes.GetEnumerator();
		}

		private ArrayList _rgNodes;
	}
}
