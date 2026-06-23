using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUCompSolderSiteAll
	{
		public MUCompSolderSiteAll()
		{
		}

		public MUCompSolderSiteAll(XElement xe)
		{
			this.m_CompSolderSites = new List<MUCompSolderSite>();
			IEnumerable<XElement> enumerable = xe.Elements("CompSolderSite");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUCompSolderSite mucompSolderSite = new MUCompSolderSite(xe2);
					this.m_CompSolderSites.Add(mucompSolderSite);
				}
			}
		}

		public List<MUCompSolderSite> CompSolderSites
		{
			get
			{
				return this.m_CompSolderSites;
			}
			set
			{
				this.m_CompSolderSites = value;
			}
		}

		public MUCompSolderSite GetCompSolderSiteByID(int id)
		{
			if (this.m_CompSolderSites == null)
			{
				return null;
			}
			return this.m_CompSolderSites.Find((MUCompSolderSite info) => info.ID == id);
		}

		private List<MUCompSolderSite> m_CompSolderSites;
	}
}
