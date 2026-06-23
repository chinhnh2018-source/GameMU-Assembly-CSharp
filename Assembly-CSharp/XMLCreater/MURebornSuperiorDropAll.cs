using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MURebornSuperiorDropAll
	{
		public MURebornSuperiorDropAll()
		{
		}

		public MURebornSuperiorDropAll(XElement xe)
		{
			this.m_RebornSuperiorDrops = new List<MURebornSuperiorDrop>();
			IEnumerable<XElement> enumerable = xe.Elements("RebornSuperiorDrop");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MURebornSuperiorDrop murebornSuperiorDrop = new MURebornSuperiorDrop(xe2);
					this.m_RebornSuperiorDrops.Add(murebornSuperiorDrop);
				}
			}
		}

		public List<MURebornSuperiorDrop> RebornSuperiorDrops
		{
			get
			{
				return this.m_RebornSuperiorDrops;
			}
			set
			{
				this.m_RebornSuperiorDrops = value;
			}
		}

		public MURebornSuperiorDrop GetRebornSuperiorDropByID(int id)
		{
			if (this.m_RebornSuperiorDrops == null)
			{
				return null;
			}
			return this.m_RebornSuperiorDrops.Find((MURebornSuperiorDrop info) => info.ID == id);
		}

		private List<MURebornSuperiorDrop> m_RebornSuperiorDrops;
	}
}
