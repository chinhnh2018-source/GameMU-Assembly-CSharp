using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MURebornEquipEvolutionAll
	{
		public MURebornEquipEvolutionAll()
		{
		}

		public MURebornEquipEvolutionAll(XElement xe)
		{
			this.m_RebornEquipEvolutions = new List<MURebornEquipEvolution>();
			this.m_DicRebornEquipEvolutions = new Dictionary<int, MURebornEquipEvolution>();
			IEnumerable<XElement> enumerable = xe.Elements("RebornEquipEvolution");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MURebornEquipEvolution murebornEquipEvolution = new MURebornEquipEvolution(xe2);
					this.m_RebornEquipEvolutions.Add(murebornEquipEvolution);
					this.m_DicRebornEquipEvolutions[murebornEquipEvolution.NeedEquitID] = murebornEquipEvolution;
				}
			}
		}

		public List<MURebornEquipEvolution> RebornEquipEvolutions
		{
			get
			{
				return this.m_RebornEquipEvolutions;
			}
			set
			{
				this.m_RebornEquipEvolutions = value;
			}
		}

		public MURebornEquipEvolution GetRebornEquipEvolutionByGoodsID(int id)
		{
			if (this.m_DicRebornEquipEvolutions == null)
			{
				return null;
			}
			MURebornEquipEvolution result = null;
			this.m_DicRebornEquipEvolutions.TryGetValue(id, ref result);
			return result;
		}

		private List<MURebornEquipEvolution> m_RebornEquipEvolutions;

		private Dictionary<int, MURebornEquipEvolution> m_DicRebornEquipEvolutions;
	}
}
