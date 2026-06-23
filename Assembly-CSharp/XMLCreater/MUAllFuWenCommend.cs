using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUAllFuWenCommend
	{
		public MUAllFuWenCommend()
		{
		}

		public MUAllFuWenCommend(XElement xe)
		{
			this.m_FuWenCommends = new List<MUFuWenCommend>();
			IEnumerable<XElement> enumerable = xe.Elements("FuWenCommend");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUFuWenCommend mufuWenCommend = new MUFuWenCommend(xe2);
					this.m_FuWenCommends.Add(mufuWenCommend);
				}
			}
		}

		public List<MUFuWenCommend> FuWenCommends
		{
			get
			{
				return this.m_FuWenCommends;
			}
			set
			{
				this.m_FuWenCommends = value;
			}
		}

		public MUFuWenCommend GetFuWenCommendByOccupationID(int occupationid)
		{
			if (this.m_FuWenCommends == null)
			{
				return null;
			}
			return this.m_FuWenCommends.Find((MUFuWenCommend info) => info.OccupationID == occupationid);
		}

		private List<MUFuWenCommend> m_FuWenCommends;
	}
}
