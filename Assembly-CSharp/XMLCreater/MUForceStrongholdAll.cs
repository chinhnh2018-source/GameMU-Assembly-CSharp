using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUForceStrongholdAll
	{
		public MUForceStrongholdAll()
		{
		}

		public MUForceStrongholdAll(XElement xe)
		{
			this.m_ForceStrongholds = new List<MUForceStronghold>();
			IEnumerable<XElement> enumerable = xe.Elements("ForceStronghold");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUForceStronghold muforceStronghold = new MUForceStronghold(xe2);
					this.m_ForceStrongholds.Add(muforceStronghold);
				}
			}
		}

		public List<MUForceStronghold> ForceStrongholds
		{
			get
			{
				return this.m_ForceStrongholds;
			}
			set
			{
				this.m_ForceStrongholds = value;
			}
		}

		public MUForceStronghold GetForceStrongholdByID(int id)
		{
			if (this.m_ForceStrongholds == null)
			{
				return null;
			}
			return this.m_ForceStrongholds.Find((MUForceStronghold info) => info.ID == id);
		}

		public MUForceStronghold GetForceStrongholdByQiZuoID(int qiZuoId)
		{
			if (this.m_ForceStrongholds == null)
			{
				return null;
			}
			return this.m_ForceStrongholds.Find((MUForceStronghold info) => info.QiZuoID == qiZuoId);
		}

		public List<MUForceStronghold> GetForceStrongholdsByMapCode(int mapCode)
		{
			List<MUForceStronghold> list = new List<MUForceStronghold>();
			for (int i = 0; i < this.m_ForceStrongholds.Count; i++)
			{
				if (this.m_ForceStrongholds[i].MapCode == mapCode)
				{
					list.Add(this.m_ForceStrongholds[i]);
				}
			}
			return list;
		}

		private List<MUForceStronghold> m_ForceStrongholds;
	}
}
