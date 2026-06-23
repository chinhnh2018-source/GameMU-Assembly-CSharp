using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUComp
	{
		public MUComp()
		{
		}

		public MUComp(XElement xe)
		{
			this.m_CompID = XMLHelper.GetIntArrtibute(xe, "CompID");
			this.m_CompName = XMLHelper.GetStringArrtibute(xe, "CompName");
			this.m_MapCode = XMLHelper.GetIntArrtibute(xe, "MapCode");
			this.m_MonstersID = XMLHelper.GetIntArrtibute(xe, "MonstersID");
			this.m_MoBai = XMLHelper.GetIntArrtibute(xe, "MoBai");
		}

		public int CompID
		{
			get
			{
				return this.m_CompID;
			}
			set
			{
				this.m_CompID = value;
			}
		}

		public string CompName
		{
			get
			{
				return this.m_CompName;
			}
			set
			{
				this.m_CompName = value;
			}
		}

		public int MapCode
		{
			get
			{
				return this.m_MapCode;
			}
			set
			{
				this.m_MapCode = value;
			}
		}

		public int MonstersID
		{
			get
			{
				return this.m_MonstersID;
			}
			set
			{
				this.m_MonstersID = value;
			}
		}

		public int MoBai
		{
			get
			{
				return this.m_MoBai;
			}
			set
			{
				this.m_MoBai = value;
			}
		}

		private int m_CompID;

		private string m_CompName;

		private int m_MapCode;

		private int m_MonstersID;

		private int m_MoBai;
	}
}
