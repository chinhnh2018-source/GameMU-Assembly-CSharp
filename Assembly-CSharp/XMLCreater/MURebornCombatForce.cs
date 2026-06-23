using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MURebornCombatForce
	{
		public MURebornCombatForce()
		{
		}

		public MURebornCombatForce(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_HolyAttack = XMLHelper.GetFloatArrtibute(xe, "HolyAttack");
			this.m_HolyDefense = XMLHelper.GetIntArrtibute(xe, "HolyDefense");
			this.m_ShadowAttack = XMLHelper.GetIntArrtibute(xe, "ShadowAttack");
			this.m_ShadowDefense = XMLHelper.GetIntArrtibute(xe, "ShadowDefense");
			this.m_NatureAttack = XMLHelper.GetIntArrtibute(xe, "NatureAttack");
			this.m_NatureDefense = XMLHelper.GetIntArrtibute(xe, "NatureDefense");
			this.m_ChaosAttack = XMLHelper.GetFloatArrtibute(xe, "ChaosAttack");
			this.m_ChaosDefense = XMLHelper.GetFloatArrtibute(xe, "ChaosDefense");
			this.m_IncubusAttack = XMLHelper.GetFloatArrtibute(xe, "IncubusAttack");
			this.m_IncubusDefense = XMLHelper.GetFloatArrtibute(xe, "IncubusDefense");
			this.m_RebronAttack = XMLHelper.GetIntArrtibute(xe, "RebronAttack");
			this.m_RebronDefense = XMLHelper.GetIntArrtibute(xe, "RebronDefense");
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

		public float HolyAttack
		{
			get
			{
				return this.m_HolyAttack;
			}
			set
			{
				this.m_HolyAttack = value;
			}
		}

		public int HolyDefense
		{
			get
			{
				return this.m_HolyDefense;
			}
			set
			{
				this.m_HolyDefense = value;
			}
		}

		public int ShadowAttack
		{
			get
			{
				return this.m_ShadowAttack;
			}
			set
			{
				this.m_ShadowAttack = value;
			}
		}

		public int ShadowDefense
		{
			get
			{
				return this.m_ShadowDefense;
			}
			set
			{
				this.m_ShadowDefense = value;
			}
		}

		public int NatureAttack
		{
			get
			{
				return this.m_NatureAttack;
			}
			set
			{
				this.m_NatureAttack = value;
			}
		}

		public int NatureDefense
		{
			get
			{
				return this.m_NatureDefense;
			}
			set
			{
				this.m_NatureDefense = value;
			}
		}

		public float ChaosAttack
		{
			get
			{
				return this.m_ChaosAttack;
			}
			set
			{
				this.m_ChaosAttack = value;
			}
		}

		public float ChaosDefense
		{
			get
			{
				return this.m_ChaosDefense;
			}
			set
			{
				this.m_ChaosDefense = value;
			}
		}

		public float IncubusAttack
		{
			get
			{
				return this.m_IncubusAttack;
			}
			set
			{
				this.m_IncubusAttack = value;
			}
		}

		public float IncubusDefense
		{
			get
			{
				return this.m_IncubusDefense;
			}
			set
			{
				this.m_IncubusDefense = value;
			}
		}

		public int RebronAttack
		{
			get
			{
				return this.m_RebronAttack;
			}
			set
			{
				this.m_RebronAttack = value;
			}
		}

		public int RebronDefense
		{
			get
			{
				return this.m_RebronDefense;
			}
			set
			{
				this.m_RebronDefense = value;
			}
		}

		private int m_ID;

		private float m_HolyAttack;

		private int m_HolyDefense;

		private int m_ShadowAttack;

		private int m_ShadowDefense;

		private int m_NatureAttack;

		private int m_NatureDefense;

		private float m_ChaosAttack;

		private float m_ChaosDefense;

		private float m_IncubusAttack;

		private float m_IncubusDefense;

		private int m_RebronAttack;

		private int m_RebronDefense;
	}
}
