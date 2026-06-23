using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUHuiGuiLoginNumGiftAll
	{
		public MUHuiGuiLoginNumGiftAll()
		{
		}

		public MUHuiGuiLoginNumGiftAll(XElement xe)
		{
			this.m_dicHuiGuiLoginNumGift = new Dictionary<int, MUHuiGuiLoginNumGift>();
			IEnumerable<XElement> enumerable = xe.Elements("HuiGuiLoginNumGift");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUHuiGuiLoginNumGift muhuiGuiLoginNumGift = new MUHuiGuiLoginNumGift(xe2);
					int num = muhuiGuiLoginNumGift.HuoDongLevel * 100 + muhuiGuiLoginNumGift.TimeOl;
					this.m_dicHuiGuiLoginNumGift[num] = muhuiGuiLoginNumGift;
					if (this.m_dayNum < muhuiGuiLoginNumGift.TimeOl)
					{
						this.m_dayNum = muhuiGuiLoginNumGift.TimeOl;
					}
				}
			}
		}

		public int DayNum
		{
			get
			{
				return this.m_dayNum;
			}
		}

		public Dictionary<int, MUHuiGuiLoginNumGift> DicHuiGuiLoginNumGift
		{
			get
			{
				return this.m_dicHuiGuiLoginNumGift;
			}
			set
			{
				this.m_dicHuiGuiLoginNumGift = value;
			}
		}

		public MUHuiGuiLoginNumGift GetLoginNumGiftByLevelAndDay(int level, int day)
		{
			if (this.m_dicHuiGuiLoginNumGift == null)
			{
				return null;
			}
			int num = level * 100 + day;
			return this.m_dicHuiGuiLoginNumGift[num];
		}

		private int m_dayNum;

		private Dictionary<int, MUHuiGuiLoginNumGift> m_dicHuiGuiLoginNumGift;
	}
}
