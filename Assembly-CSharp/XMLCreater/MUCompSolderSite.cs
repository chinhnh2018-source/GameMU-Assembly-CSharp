using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUCompSolderSite
	{
		public MUCompSolderSite()
		{
		}

		public MUCompSolderSite(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_Site = XMLHelper.GetStringArrtibute(xe, "Site");
			this.m_RefreshTime = XMLHelper.GetStringArrtibute(xe, "RefreshTime");
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

		public string Site
		{
			get
			{
				return this.m_Site;
			}
			set
			{
				this.m_Site = value;
			}
		}

		public string RefreshTime
		{
			get
			{
				return this.m_RefreshTime;
			}
			set
			{
				this.m_RefreshTime = value;
			}
		}

		private int m_ID;

		private string m_Site;

		private string m_RefreshTime;
	}
}
