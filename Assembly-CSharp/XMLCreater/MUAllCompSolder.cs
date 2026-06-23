using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUAllCompSolder
	{
		public MUAllCompSolder()
		{
		}

		public MUAllCompSolder(XElement xe)
		{
			this.m_CompSolders = new List<MUCompSolder>();
			IEnumerable<XElement> enumerable = xe.Elements("CompSolder");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUCompSolder mucompSolder = new MUCompSolder(xe2);
					this.m_CompSolders.Add(mucompSolder);
				}
			}
		}

		public List<MUCompSolder> CompSolders
		{
			get
			{
				return this.m_CompSolders;
			}
			set
			{
				this.m_CompSolders = value;
			}
		}

		public MUCompSolder GetCompSolderByID(int id)
		{
			if (this.m_CompSolders == null)
			{
				return null;
			}
			return this.m_CompSolders.Find((MUCompSolder info) => info.ID == id);
		}

		private List<MUCompSolder> m_CompSolders;
	}
}
