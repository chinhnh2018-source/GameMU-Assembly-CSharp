using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUHuiGuiDayZhiGouAll
	{
		public MUHuiGuiDayZhiGouAll()
		{
		}

		public MUHuiGuiDayZhiGouAll(XElement xe)
		{
			this.m_dicHuiGuiDayZhiGou = new Dictionary<int, List<MUHuiGuiDayZhiGou>>();
			IEnumerable<XElement> enumerable = xe.Elements("HuiGuiDayZhiGou");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUHuiGuiDayZhiGou muhuiGuiDayZhiGou = new MUHuiGuiDayZhiGou(xe2);
					List<MUHuiGuiDayZhiGou> list = null;
					int num = muhuiGuiDayZhiGou.HuoDongLevel * 100 + muhuiGuiDayZhiGou.Day;
					if (!this.m_dicHuiGuiDayZhiGou.TryGetValue(num, ref list))
					{
						list = new List<MUHuiGuiDayZhiGou>();
						this.m_dicHuiGuiDayZhiGou[num] = list;
					}
					list.Add(muhuiGuiDayZhiGou);
					if (this.m_dayNum < muhuiGuiDayZhiGou.Day)
					{
						this.m_dayNum = muhuiGuiDayZhiGou.Day;
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

		public Dictionary<int, List<MUHuiGuiDayZhiGou>> DicHuiGuiDayZhiGou
		{
			get
			{
				return this.m_dicHuiGuiDayZhiGou;
			}
			set
			{
				this.m_dicHuiGuiDayZhiGou = value;
			}
		}

		public List<MUHuiGuiDayZhiGou> GetZhiGouByLevelAndDay(int level, int day)
		{
			if (this.DicHuiGuiDayZhiGou == null)
			{
				return null;
			}
			int num = level * 100 + day;
			return this.m_dicHuiGuiDayZhiGou[num];
		}

		public List<MUHuiGuiDayZhiGou> GetZhiGouByLevelAndDay(int level, int day, int chongZhiNum)
		{
			List<MUHuiGuiDayZhiGou> zhiGouByLevelAndDay = this.GetZhiGouByLevelAndDay(level, day);
			if (zhiGouByLevelAndDay == null)
			{
				return null;
			}
			List<MUHuiGuiDayZhiGou> list = new List<MUHuiGuiDayZhiGou>();
			for (int i = 0; i < zhiGouByLevelAndDay.Count; i++)
			{
				MUHuiGuiDayZhiGou muhuiGuiDayZhiGou = zhiGouByLevelAndDay[i];
				if (muhuiGuiDayZhiGou.TotalYuanBao.Count >= 2)
				{
					if (muhuiGuiDayZhiGou.TotalYuanBao[1] == -1)
					{
						muhuiGuiDayZhiGou.TotalYuanBao[1] = int.MaxValue;
					}
					if (chongZhiNum >= muhuiGuiDayZhiGou.TotalYuanBao[0] && chongZhiNum < muhuiGuiDayZhiGou.TotalYuanBao[1])
					{
						list.Add(muhuiGuiDayZhiGou);
					}
				}
			}
			return list;
		}

		private int m_dayNum;

		private Dictionary<int, List<MUHuiGuiDayZhiGou>> m_dicHuiGuiDayZhiGou;
	}
}
