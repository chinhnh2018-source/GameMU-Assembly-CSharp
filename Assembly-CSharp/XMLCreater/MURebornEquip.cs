using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MURebornEquip
	{
		public MURebornEquip()
		{
		}

		public MURebornEquip(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_GoodsID = XMLHelper.GetIntArrtibute(xe, "GoodsID");
			this.m_Name = XMLHelper.GetStringArrtibute(xe, "Name");
			this.m_ZSMod = XMLHelper.GetStringArrtibute(xe, "ZSMod");
			this.m_FSMod = XMLHelper.GetStringArrtibute(xe, "FSMod");
			this.m_GSMOd = XMLHelper.GetStringArrtibute(xe, "GSMOd");
			this.m_MJSMod = XMLHelper.GetStringArrtibute(xe, "MJSMod");
			this.m_ZHSMod = XMLHelper.GetStringArrtibute(xe, "ZHSMod");
			this.m_LMJSMod = XMLHelper.GetStringArrtibute(xe, "LMJSMod");
			this.m_FMJSMod = XMLHelper.GetStringArrtibute(xe, "FMJSMod");
		}

		public string LMJSMod
		{
			get
			{
				return this.m_LMJSMod;
			}
			set
			{
				this.m_LMJSMod = value;
			}
		}

		public string FMJSMod
		{
			get
			{
				return this.m_FMJSMod;
			}
			set
			{
				this.m_FMJSMod = value;
			}
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

		public int GoodsID
		{
			get
			{
				return this.m_GoodsID;
			}
			set
			{
				this.m_GoodsID = value;
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

		public string ZSMod
		{
			get
			{
				return this.m_ZSMod;
			}
			set
			{
				this.m_ZSMod = value;
			}
		}

		public string FSMod
		{
			get
			{
				return this.m_FSMod;
			}
			set
			{
				this.m_FSMod = value;
			}
		}

		public string GSMOd
		{
			get
			{
				return this.m_GSMOd;
			}
			set
			{
				this.m_GSMOd = value;
			}
		}

		public string MJSMod
		{
			get
			{
				return this.m_MJSMod;
			}
			set
			{
				this.m_MJSMod = value;
			}
		}

		public string ZHSMod
		{
			get
			{
				return this.m_ZHSMod;
			}
			set
			{
				this.m_ZHSMod = value;
			}
		}

		private string m_LMJSMod;

		private string m_FMJSMod;

		private int m_ID;

		private int m_GoodsID;

		private string m_Name;

		private string m_ZSMod;

		private string m_FSMod;

		private string m_GSMOd;

		private string m_MJSMod;

		private string m_ZHSMod;
	}
}
