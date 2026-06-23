using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MURebornSuperiorDrop
	{
		public MURebornSuperiorDrop()
		{
		}

		public MURebornSuperiorDrop(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_Name = XMLHelper.GetStringArrtibute(xe, "Name");
			this.m_RebornSuperiorRate = XMLHelper.GetStringArrtibute(xe, "RebornSuperiorRate");
			this.m_RebornSuperiorBank = XMLHelper.GetStringArrtibute(xe, "RebornSuperiorBank");
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

		public string RebornSuperiorRate
		{
			get
			{
				return this.m_RebornSuperiorRate;
			}
			set
			{
				this.m_RebornSuperiorRate = value;
			}
		}

		public string RebornSuperiorBank
		{
			get
			{
				return this.m_RebornSuperiorBank;
			}
			set
			{
				this.m_RebornSuperiorBank = value;
			}
		}

		private int m_ID;

		private string m_Name;

		private string m_RebornSuperiorRate;

		private string m_RebornSuperiorBank;
	}
}
