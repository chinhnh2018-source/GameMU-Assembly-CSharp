using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUTransfigurationLevel
	{
		public MUTransfigurationLevel()
		{
		}

		public MUTransfigurationLevel(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_lstOccupationID = XMLHelper.GetIntListArrtibute(xe, "OccupationID", ',');
			this.m_Level = XMLHelper.GetIntArrtibute(xe, "Level");
			this.m_GodMod = XMLHelper.GetIntArrtibute(xe, "GodMod");
			this.m_WeaponMOd = XMLHelper.GetIntArrtibute(xe, "WeaponMOd");
			string stringArrtibute = XMLHelper.GetStringArrtibute(xe, "ProPerty");
			this.m_ProPerty = MUPropInfoHelper.DeserializeToPropList(stringArrtibute);
			this.m_UpExp = XMLHelper.GetIntArrtibute(xe, "UpExp");
			this.m_GoodsExp = XMLHelper.GetIntArrtibute(xe, "GoodsExp");
			this.m_ExpCritRate = XMLHelper.GetFloatArrtibute(xe, "ExpCritRate");
			this.m_ExpCritTimes = XMLHelper.GetIntArrtibute(xe, "ExpCritTimes");
			string stringArrtibute2 = XMLHelper.GetStringArrtibute(xe, "NeedGoods");
			this.m_NeedGoods = new List<MUMaterialInfo>();
			if (stringArrtibute2 != string.Empty)
			{
				string[] array = stringArrtibute2.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					MUMaterialInfo mumaterialInfo = new MUMaterialInfo(array[i]);
					this.m_NeedGoods.Add(mumaterialInfo);
				}
			}
			this.m_AttackSkill = XMLHelper.GetIntListArrtibute(xe, "AttackSkill", ',');
			this.m_MagicSkill = XMLHelper.GetIntListArrtibute(xe, "MagicSkill", ',');
			this.m_Duration = XMLHelper.GetIntArrtibute(xe, "Duration");
		}

		public int ID
		{
			get
			{
				return this.m_ID;
			}
			set
			{
				this.m_ID = value;
			}
		}

		public List<int> LstOccupationID
		{
			get
			{
				return this.m_lstOccupationID;
			}
			set
			{
				this.m_lstOccupationID = value;
			}
		}

		public int Level
		{
			get
			{
				return this.m_Level;
			}
			set
			{
				this.m_Level = value;
			}
		}

		public int GodMod
		{
			get
			{
				return this.m_GodMod;
			}
			set
			{
				this.m_GodMod = value;
			}
		}

		public int WeaponMOd
		{
			get
			{
				return this.m_WeaponMOd;
			}
			set
			{
				this.m_WeaponMOd = value;
			}
		}

		public List<MUPropInfo> ProPerty
		{
			get
			{
				return this.m_ProPerty;
			}
			set
			{
				this.m_ProPerty = value;
			}
		}

		public int UpExp
		{
			get
			{
				return this.m_UpExp;
			}
			set
			{
				this.m_UpExp = value;
			}
		}

		public int GoodsExp
		{
			get
			{
				return this.m_GoodsExp;
			}
			set
			{
				this.m_GoodsExp = value;
			}
		}

		public float ExpCritRate
		{
			get
			{
				return this.m_ExpCritRate;
			}
			set
			{
				this.m_ExpCritRate = value;
			}
		}

		public int ExpCritTimes
		{
			get
			{
				return this.m_ExpCritTimes;
			}
			set
			{
				this.m_ExpCritTimes = value;
			}
		}

		public List<MUMaterialInfo> NeedGoods
		{
			get
			{
				return this.m_NeedGoods;
			}
			set
			{
				this.m_NeedGoods = value;
			}
		}

		public List<int> AttackSkill
		{
			get
			{
				return this.m_AttackSkill;
			}
			set
			{
				this.m_AttackSkill = value;
			}
		}

		public List<int> MagicSkill
		{
			get
			{
				return this.m_MagicSkill;
			}
			set
			{
				this.m_MagicSkill = value;
			}
		}

		public int Duration
		{
			get
			{
				return this.m_Duration;
			}
			set
			{
				this.m_Duration = value;
			}
		}

		private int m_ID;

		private string m_OccupationID;

		private List<int> m_lstOccupationID = new List<int>();

		private int m_Level;

		private int m_GodMod;

		private int m_WeaponMOd;

		private List<MUPropInfo> m_ProPerty;

		private int m_UpExp;

		private int m_GoodsExp;

		private float m_ExpCritRate;

		private int m_ExpCritTimes;

		private List<MUMaterialInfo> m_NeedGoods;

		private List<int> m_AttackSkill;

		private List<int> m_MagicSkill;

		private int m_Duration;
	}
}
