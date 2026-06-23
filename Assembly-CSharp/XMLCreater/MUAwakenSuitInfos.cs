using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUAwakenSuitInfos
	{
		public MUAwakenSuitInfos()
		{
		}

		public MUAwakenSuitInfos(XElement xe)
		{
			this.m_AwakenSuits = new List<MUAwakenSuitDetail>();
			IEnumerable<XElement> enumerable = xe.Elements("AwakenSuit");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUAwakenSuitDetail muawakenSuitDetail = new MUAwakenSuitDetail(xe2);
					this.m_AwakenSuits.Add(muawakenSuitDetail);
				}
			}
		}

		public List<MUAwakenSuitDetail> AwakenSuits
		{
			get
			{
				return this.m_AwakenSuits;
			}
			set
			{
				this.m_AwakenSuits = value;
			}
		}

		public MUAwakenSuitDetail GetAwakenSuitByID(int id)
		{
			if (this.m_AwakenSuits == null)
			{
				return null;
			}
			return this.m_AwakenSuits.Find((MUAwakenSuitDetail info) => info.ID == id);
		}

		private List<MUAwakenSuitDetail> m_AwakenSuits;
	}
}
