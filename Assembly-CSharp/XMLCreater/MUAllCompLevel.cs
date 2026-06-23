using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUAllCompLevel
	{
		public MUAllCompLevel()
		{
		}

		public MUAllCompLevel(XElement xe)
		{
			this.m_CompLevels = new List<MUCompLevel>();
			IEnumerable<XElement> enumerable = xe.Elements("CompLevel");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUCompLevel mucompLevel = new MUCompLevel(xe2);
					this.m_CompLevels.Add(mucompLevel);
				}
			}
		}

		public List<MUCompLevel> CompLevels
		{
			get
			{
				return this.m_CompLevels;
			}
			set
			{
				this.m_CompLevels = value;
			}
		}

		public MUCompLevel GetCompLevelByID(int id)
		{
			if (this.m_CompLevels == null)
			{
				return null;
			}
			return this.m_CompLevels.Find((MUCompLevel info) => info.ID == id);
		}

		public MUCompLevel GetCompLevelByCompIDAndLevel(int compID, int level)
		{
			if (this.m_CompLevels == null)
			{
				return null;
			}
			return this.m_CompLevels.Find((MUCompLevel info) => info.CompID == compID && info.Level == level);
		}

		private List<MUCompLevel> m_CompLevels;
	}
}
