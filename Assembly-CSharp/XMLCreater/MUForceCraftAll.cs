using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUForceCraftAll
	{
		public MUForceCraftAll()
		{
		}

		public MUForceCraftAll(XElement xe)
		{
			this.m_ForceCrafts = new List<MUForceCraft>();
			IEnumerable<XElement> enumerable = xe.Elements("ForceCraft");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUForceCraft muforceCraft = new MUForceCraft(xe2);
					this.m_ForceCrafts.Add(muforceCraft);
				}
			}
		}

		public List<MUForceCraft> ForceCrafts
		{
			get
			{
				return this.m_ForceCrafts;
			}
			set
			{
				this.m_ForceCrafts = value;
			}
		}

		public MUForceCraft GetForceCraftByID(int id)
		{
			if (this.m_ForceCrafts == null)
			{
				return null;
			}
			return this.m_ForceCrafts.Find((MUForceCraft info) => info.ID == id);
		}

		private List<MUForceCraft> m_ForceCrafts;
	}
}
