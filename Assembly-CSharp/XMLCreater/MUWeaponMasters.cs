using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUWeaponMasters
	{
		public MUWeaponMasters()
		{
		}

		public MUWeaponMasters(XElement xe)
		{
			this.m_WeaponMasters = new List<MUWeaponMasterInfo>();
			IEnumerable<XElement> enumerable = xe.Elements("WeaponMaster");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUWeaponMasterInfo muweaponMasterInfo = new MUWeaponMasterInfo(xe2);
					this.m_WeaponMasters.Add(muweaponMasterInfo);
				}
			}
		}

		public List<MUWeaponMasterInfo> WeaponMasters
		{
			get
			{
				return this.m_WeaponMasters;
			}
			set
			{
				this.m_WeaponMasters = value;
			}
		}

		public MUWeaponMasterInfo GetWeaponMasterByID(int id)
		{
			if (this.m_WeaponMasters == null)
			{
				return null;
			}
			return this.m_WeaponMasters.Find((MUWeaponMasterInfo info) => info.ID == id);
		}

		public List<MUWeaponMasterInfo> GetWeaponMasterByType(int type)
		{
			List<MUWeaponMasterInfo> list = new List<MUWeaponMasterInfo>();
			if (this.m_WeaponMasters == null)
			{
				return list;
			}
			for (int i = 0; i < this.m_WeaponMasters.Count; i++)
			{
				if (this.m_WeaponMasters[i].Type == type)
				{
					list.Add(this.m_WeaponMasters[i]);
				}
			}
			return list;
		}

		private List<MUWeaponMasterInfo> m_WeaponMasters;
	}
}
