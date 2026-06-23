using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MURebornEquipEvolution
	{
		public MURebornEquipEvolution()
		{
		}

		public MURebornEquipEvolution(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_Name = XMLHelper.GetStringArrtibute(xe, "Name");
			this.m_NewEquitID = XMLHelper.GetIntArrtibute(xe, "NewEquitID");
			this.m_NeedEquitID = XMLHelper.GetIntArrtibute(xe, "NeedEquitID");
			this.m_NeedCuiLian = XMLHelper.GetIntArrtibute(xe, "NeedCuiLian");
			this.m_NeedDuanZao = XMLHelper.GetIntArrtibute(xe, "NeedDuanZao");
			this.m_NeedNiePan = XMLHelper.GetIntArrtibute(xe, "NeedNiePan");
			this.m_SuccessRate = XMLHelper.GetFloatArrtibute(xe, "SuccessRate");
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

		public string Name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		public int NewEquitID
		{
			get
			{
				return this.m_NewEquitID;
			}
			set
			{
				this.m_NewEquitID = value;
			}
		}

		public int NeedEquitID
		{
			get
			{
				return this.m_NeedEquitID;
			}
			set
			{
				this.m_NeedEquitID = value;
			}
		}

		public int NeedCuiLian
		{
			get
			{
				return this.m_NeedCuiLian;
			}
			set
			{
				this.m_NeedCuiLian = value;
			}
		}

		public int NeedDuanZao
		{
			get
			{
				return this.m_NeedDuanZao;
			}
			set
			{
				this.m_NeedDuanZao = value;
			}
		}

		public int NeedNiePan
		{
			get
			{
				return this.m_NeedNiePan;
			}
			set
			{
				this.m_NeedNiePan = value;
			}
		}

		public float SuccessRate
		{
			get
			{
				return this.m_SuccessRate;
			}
			set
			{
				this.m_SuccessRate = value;
			}
		}

		private int m_ID;

		private string m_Name;

		private int m_NewEquitID;

		private int m_NeedEquitID;

		private int m_NeedCuiLian;

		private int m_NeedDuanZao;

		private int m_NeedNiePan;

		private float m_SuccessRate;
	}
}
