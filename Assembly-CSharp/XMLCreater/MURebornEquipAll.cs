using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MURebornEquipAll
	{
		public MURebornEquipAll()
		{
		}

		public MURebornEquipAll(XElement xe)
		{
			this.m_RebornEquips = new List<MURebornEquip>();
			this.m_DicEquips = new Dictionary<int, MURebornEquip>();
			IEnumerable<XElement> enumerable = xe.Elements("RebornEquip");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MURebornEquip murebornEquip = new MURebornEquip(xe2);
					this.m_RebornEquips.Add(murebornEquip);
					this.m_DicEquips[murebornEquip.GoodsID] = murebornEquip;
				}
			}
		}

		public List<MURebornEquip> RebornEquips
		{
			get
			{
				return this.m_RebornEquips;
			}
			set
			{
				this.m_RebornEquips = value;
			}
		}

		public MURebornEquip GetRebornEquipByID(int id)
		{
			if (this.m_DicEquips == null)
			{
				return null;
			}
			MURebornEquip result = null;
			this.m_DicEquips.TryGetValue(id, ref result);
			return result;
		}

		private List<MURebornEquip> m_RebornEquips;

		private Dictionary<int, MURebornEquip> m_DicEquips;
	}
}
