using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUALLCompNotice
	{
		public MUALLCompNotice()
		{
		}

		public MUALLCompNotice(XElement xe)
		{
			this.m_CompNotices = new List<MUCompNotice>();
			IEnumerable<XElement> enumerable = xe.Elements("CompNotice");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUCompNotice mucompNotice = new MUCompNotice(xe2);
					this.m_CompNotices.Add(mucompNotice);
				}
			}
		}

		public List<MUCompNotice> CompNotices
		{
			get
			{
				return this.m_CompNotices;
			}
			set
			{
				this.m_CompNotices = value;
			}
		}

		public MUCompNotice GetCompNoticeByID(int id)
		{
			if (this.m_CompNotices == null)
			{
				return null;
			}
			return this.m_CompNotices.Find((MUCompNotice info) => info.ID == id);
		}

		private List<MUCompNotice> m_CompNotices;
	}
}
