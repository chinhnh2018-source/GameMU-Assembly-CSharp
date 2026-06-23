using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUAwakenLevelInfos
	{
		public MUAwakenLevelInfos(XElement xe)
		{
			this.m_AwakenLevels = new List<MUAwakenLevelDetail>();
			IEnumerable<XElement> enumerable = xe.Elements("AwakenLevel");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUAwakenLevelDetail muawakenLevelDetail = new MUAwakenLevelDetail(xe2);
					this.m_AwakenLevels.Add(muawakenLevelDetail);
				}
			}
		}

		public List<MUAwakenLevelDetail> AwakenLevels
		{
			get
			{
				return this.m_AwakenLevels;
			}
			set
			{
				this.m_AwakenLevels = value;
			}
		}

		public MUAwakenLevelDetail GetAwakenLevelByID(int id)
		{
			if (this.m_AwakenLevels == null)
			{
				return null;
			}
			return this.m_AwakenLevels.Find((MUAwakenLevelDetail info) => info.ID == id);
		}

		public List<MUAwakenLevelDetail> GetAwakenLevelByOrder(int order)
		{
			List<MUAwakenLevelDetail> list = new List<MUAwakenLevelDetail>();
			for (int i = 0; i < this.m_AwakenLevels.Count; i++)
			{
				if (this.m_AwakenLevels[i].Order == order)
				{
					list.Add(this.m_AwakenLevels[i]);
				}
			}
			return list;
		}

		public MUAwakenLevelDetail GetAwakenLevelByOrderStar(int order, int star)
		{
			if (this.m_AwakenLevels == null)
			{
				return null;
			}
			return this.m_AwakenLevels.Find((MUAwakenLevelDetail info) => info.Order == order && info.Star == star);
		}

		private List<MUAwakenLevelDetail> m_AwakenLevels;
	}
}
