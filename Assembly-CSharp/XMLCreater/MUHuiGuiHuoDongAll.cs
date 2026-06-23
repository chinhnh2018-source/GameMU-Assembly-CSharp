using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUHuiGuiHuoDongAll
	{
		public MUHuiGuiHuoDongAll()
		{
		}

		public MUHuiGuiHuoDongAll(XElement xe)
		{
			this.m_HuiGuiHuoDongs = new List<MUHuiGuiHuoDong>();
			IEnumerable<XElement> enumerable = xe.Elements("HuiGuiHuoDong");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUHuiGuiHuoDong muhuiGuiHuoDong = new MUHuiGuiHuoDong(xe2);
					this.m_HuiGuiHuoDongs.Add(muhuiGuiHuoDong);
				}
			}
		}

		public List<MUHuiGuiHuoDong> HuiGuiHuoDongs
		{
			get
			{
				return this.m_HuiGuiHuoDongs;
			}
			set
			{
				this.m_HuiGuiHuoDongs = value;
			}
		}

		public MUHuiGuiHuoDong GetHuiGuiHuoDongByID(int id)
		{
			if (this.m_HuiGuiHuoDongs == null)
			{
				return null;
			}
			return this.m_HuiGuiHuoDongs.Find((MUHuiGuiHuoDong info) => info.HuoDongLevel == id);
		}

		private List<MUHuiGuiHuoDong> m_HuiGuiHuoDongs;
	}
}
