using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUHuiGuiChongZhiGiftAll
	{
		public MUHuiGuiChongZhiGiftAll()
		{
		}

		public MUHuiGuiChongZhiGiftAll(XElement xe)
		{
			this.m_dicHuiGuiChongZhiGift = new Dictionary<int, List<MUHuiGuiChongZhiGift>>();
			IEnumerable<XElement> enumerable = xe.Elements("HuiGuiChongZhiGift");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUHuiGuiChongZhiGift muhuiGuiChongZhiGift = new MUHuiGuiChongZhiGift(xe2);
					List<MUHuiGuiChongZhiGift> list = null;
					if (!this.m_dicHuiGuiChongZhiGift.TryGetValue(muhuiGuiChongZhiGift.HuoDongLevel, ref list))
					{
						list = new List<MUHuiGuiChongZhiGift>();
						this.m_dicHuiGuiChongZhiGift[muhuiGuiChongZhiGift.HuoDongLevel] = list;
					}
					list.Add(muhuiGuiChongZhiGift);
				}
			}
		}

		public Dictionary<int, List<MUHuiGuiChongZhiGift>> DicHuiGuiChongZhiGift
		{
			get
			{
				return this.m_dicHuiGuiChongZhiGift;
			}
			set
			{
				this.m_dicHuiGuiChongZhiGift = value;
			}
		}

		public List<MUHuiGuiChongZhiGift> GetChongZhiGiftByLevel(int huoDongLevel)
		{
			if (this.m_dicHuiGuiChongZhiGift == null)
			{
				return null;
			}
			return this.m_dicHuiGuiChongZhiGift[huoDongLevel];
		}

		private Dictionary<int, List<MUHuiGuiChongZhiGift>> m_dicHuiGuiChongZhiGift;
	}
}
