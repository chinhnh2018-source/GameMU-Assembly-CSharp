using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUAllCompResources
	{
		public MUAllCompResources()
		{
		}

		public MUAllCompResources(XElement xe)
		{
			this.m_CompResources = new List<MUCompResources>();
			IEnumerable<XElement> enumerable = xe.Elements("CompResources");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUCompResources mucompResources = new MUCompResources(xe2);
					this.m_CompResources.Add(mucompResources);
				}
			}
		}

		public List<MUCompResources> CompResources
		{
			get
			{
				return this.m_CompResources;
			}
			set
			{
				this.m_CompResources = value;
			}
		}

		public MUCompResources GetCompResourcesByID(int id)
		{
			if (this.m_CompResources == null)
			{
				return null;
			}
			return this.m_CompResources.Find((MUCompResources info) => info.ID == id);
		}

		private List<MUCompResources> m_CompResources;
	}
}
