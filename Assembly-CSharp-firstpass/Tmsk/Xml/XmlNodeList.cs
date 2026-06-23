using System;
using System.Collections;

namespace Tmsk.Xml
{
	public abstract class XmlNodeList : IEnumerable
	{
		public XmlNodeList()
		{
		}

		public abstract int Count { get; }

		public abstract IEnumerator GetEnumerator();

		public virtual XmlNode this[int i]
		{
			get
			{
				return null;
			}
		}
	}
}
