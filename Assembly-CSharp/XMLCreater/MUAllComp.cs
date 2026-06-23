using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUAllComp
	{
		public MUAllComp()
		{
		}

		public MUAllComp(XElement xe)
		{
			this.m_Comps = new List<MUComp>();
			IEnumerable<XElement> enumerable = xe.Elements("Comp");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUComp mucomp = new MUComp(xe2);
					this.m_Comps.Add(mucomp);
				}
			}
		}

		public List<MUComp> Comps
		{
			get
			{
				return this.m_Comps;
			}
			set
			{
				this.m_Comps = value;
			}
		}

		public MUComp GetCompByCompID(int compid)
		{
			if (this.m_Comps == null)
			{
				return null;
			}
			return this.m_Comps.Find((MUComp info) => info.CompID == compid);
		}

		public MUComp GetCompByCompMoBaiID(int moBaiId)
		{
			if (this.m_Comps == null)
			{
				return null;
			}
			return this.m_Comps.Find((MUComp info) => info.MoBai == moBaiId);
		}

		public MUComp GetCompByCompMapCode(int mapCode)
		{
			if (this.m_Comps == null)
			{
				return null;
			}
			return this.m_Comps.Find((MUComp info) => info.MapCode == mapCode);
		}

		private List<MUComp> m_Comps;
	}
}
