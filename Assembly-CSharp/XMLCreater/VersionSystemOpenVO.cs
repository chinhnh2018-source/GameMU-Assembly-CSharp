using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class VersionSystemOpenVO
	{
		public VersionSystemOpenVO()
		{
		}

		public VersionSystemOpenVO(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_SystemName = XMLHelper.GetStringArrtibute(xe, "SystemName");
			this.m_IsOpen = (XMLHelper.GetIntArrtibute(xe, "IsOpen") == 1);
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

		public string SystemName
		{
			get
			{
				return this.m_SystemName;
			}
			set
			{
				this.m_SystemName = value;
			}
		}

		public bool IsOpen
		{
			get
			{
				return this.m_IsOpen;
			}
			set
			{
				this.m_IsOpen = value;
			}
		}

		private int m_ID;

		private string m_SystemName;

		private bool m_IsOpen;
	}
}
