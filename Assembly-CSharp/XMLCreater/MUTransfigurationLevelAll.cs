using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUTransfigurationLevelAll
	{
		public MUTransfigurationLevelAll()
		{
		}

		public MUTransfigurationLevelAll(XElement xe)
		{
			this.m_TransfigurationLevels = new List<MUTransfigurationLevel>();
			IEnumerable<XElement> enumerable = xe.Elements("TransfigurationLevel");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUTransfigurationLevel mutransfigurationLevel = new MUTransfigurationLevel(xe2);
					this.m_TransfigurationLevels.Add(mutransfigurationLevel);
				}
			}
		}

		public List<MUTransfigurationLevel> TransfigurationLevels
		{
			get
			{
				return this.m_TransfigurationLevels;
			}
			set
			{
				this.m_TransfigurationLevels = value;
			}
		}

		public MUTransfigurationLevel GetTransfigurationLevelByID(int id)
		{
			if (this.m_TransfigurationLevels == null)
			{
				return null;
			}
			return this.m_TransfigurationLevels.Find((MUTransfigurationLevel info) => info.ID == id);
		}

		public MUTransfigurationLevel GetTransfigurationLevelByOccupationLevel(int occupation, int level)
		{
			if (this.m_TransfigurationLevels == null)
			{
				return null;
			}
			return this.m_TransfigurationLevels.Find((MUTransfigurationLevel info) => info.Level == level && info.LstOccupationID.IndexOf(occupation) > -1);
		}

		public List<MUTransfigurationLevel> GetAllTransfigurationLevelByOccupation(int occupation)
		{
			List<MUTransfigurationLevel> list = new List<MUTransfigurationLevel>();
			if (this.m_TransfigurationLevels == null)
			{
				return list;
			}
			for (int i = 0; i < this.m_TransfigurationLevels.Count; i++)
			{
				if (this.m_TransfigurationLevels[i].LstOccupationID.IndexOf(occupation) > -1)
				{
					list.Add(this.m_TransfigurationLevels[i]);
				}
			}
			return list;
		}

		private List<MUTransfigurationLevel> m_TransfigurationLevels;
	}
}
