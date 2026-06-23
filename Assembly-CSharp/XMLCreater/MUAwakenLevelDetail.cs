using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUAwakenLevelDetail
	{
		public MUAwakenLevelDetail()
		{
		}

		public MUAwakenLevelDetail(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_Order = XMLHelper.GetIntArrtibute(xe, "Order");
			this.m_Star = XMLHelper.GetIntArrtibute(xe, "Star");
			this.m_ModID = XMLHelper.GetIntArrtibute(xe, "ModID");
			this.m_Awakenment = XMLHelper.GetIntArrtibute(xe, "Awakenment");
			string stringArrtibute = XMLHelper.GetStringArrtibute(xe, "AwakenAdvancedment");
			this.m_Advancedment = new MUMaterialInfo(stringArrtibute);
			this.m_EnlargeRate = XMLHelper.GetFloatArrtibute(xe, "EnlargeRate");
			string stringArrtibute2 = XMLHelper.GetStringArrtibute(xe, "AdvancedEffect");
			this.m_AdvancedEffects = MUPropInfoHelper.DeserializeToPropList(stringArrtibute2);
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

		public int Order
		{
			get
			{
				return this.m_Order;
			}
			set
			{
				this.m_Order = value;
			}
		}

		public int Star
		{
			get
			{
				return this.m_Star;
			}
			set
			{
				this.m_Star = value;
			}
		}

		public int ModID
		{
			get
			{
				return this.m_ModID;
			}
			set
			{
				this.m_ModID = value;
			}
		}

		public int Awakenment
		{
			get
			{
				return this.m_Awakenment;
			}
			set
			{
				this.m_Awakenment = value;
			}
		}

		public MUMaterialInfo Advancedment
		{
			get
			{
				return this.m_Advancedment;
			}
			set
			{
				this.m_Advancedment = value;
			}
		}

		public float EnlargeRate
		{
			get
			{
				return this.m_EnlargeRate;
			}
			set
			{
				this.m_EnlargeRate = value;
			}
		}

		public List<MUPropInfo> AdvancedEffects
		{
			get
			{
				return this.m_AdvancedEffects;
			}
			set
			{
				this.m_AdvancedEffects = value;
			}
		}

		private int m_ID;

		private int m_Order;

		private int m_Star;

		private int m_ModID;

		private int m_Awakenment;

		private MUMaterialInfo m_Advancedment;

		private float m_EnlargeRate;

		private List<MUPropInfo> m_AdvancedEffects;
	}
}
