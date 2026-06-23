using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MURebornSuperiorTypeAll
	{
		public MURebornSuperiorTypeAll()
		{
		}

		public MURebornSuperiorTypeAll(XElement xe)
		{
			this.m_RebornSuperiorTypes = new List<MURebornSuperiorType>();
			IEnumerable<XElement> enumerable = xe.Elements("RebornSuperiorType");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MURebornSuperiorType murebornSuperiorType = new MURebornSuperiorType(xe2);
					this.m_RebornSuperiorTypes.Add(murebornSuperiorType);
				}
			}
		}

		public List<MURebornSuperiorType> RebornSuperiorTypes
		{
			get
			{
				return this.m_RebornSuperiorTypes;
			}
			set
			{
				this.m_RebornSuperiorTypes = value;
			}
		}

		public MURebornSuperiorType GetRebornSuperiorTypeByID(int id)
		{
			if (this.m_RebornSuperiorTypes == null)
			{
				return null;
			}
			return this.m_RebornSuperiorTypes.Find((MURebornSuperiorType info) => info.ID == id);
		}

		private List<MURebornSuperiorType> m_RebornSuperiorTypes;
	}
}
