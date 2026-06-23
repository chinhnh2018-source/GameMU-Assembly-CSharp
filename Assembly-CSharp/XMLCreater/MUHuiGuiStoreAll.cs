using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUHuiGuiStoreAll
	{
		public MUHuiGuiStoreAll()
		{
		}

		public MUHuiGuiStoreAll(XElement xe)
		{
			this.m_dicHuiGuiStore = new Dictionary<int, List<MUHuiGuiStore>>();
			IEnumerable<XElement> enumerable = xe.Elements("OldStore");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUHuiGuiStore muhuiGuiStore = new MUHuiGuiStore(xe2);
					List<MUHuiGuiStore> list = null;
					int num = muhuiGuiStore.HuoDongLevel * 100 + muhuiGuiStore.Day;
					if (!this.m_dicHuiGuiStore.TryGetValue(num, ref list))
					{
						list = new List<MUHuiGuiStore>();
						this.m_dicHuiGuiStore[num] = list;
					}
					list.Add(muhuiGuiStore);
				}
			}
		}

		public Dictionary<int, List<MUHuiGuiStore>> DicHuiGuiStore
		{
			get
			{
				return this.m_dicHuiGuiStore;
			}
			set
			{
				this.m_dicHuiGuiStore = value;
			}
		}

		public List<MUHuiGuiStore> GetStoreByLevelAndDay(int level, int day)
		{
			if (this.m_dicHuiGuiStore == null)
			{
				return null;
			}
			int num = level * 100 + day;
			return this.m_dicHuiGuiStore[num];
		}

		private Dictionary<int, List<MUHuiGuiStore>> m_dicHuiGuiStore;
	}
}
