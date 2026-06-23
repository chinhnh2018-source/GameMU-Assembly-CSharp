using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class VersionSystemOpenVOAll
	{
		public VersionSystemOpenVOAll()
		{
		}

		public VersionSystemOpenVOAll(XElement xe)
		{
			this.m_Versions = new List<VersionSystemOpenVO>();
			IEnumerable<XElement> enumerable = xe.Elements("Version");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					VersionSystemOpenVO versionSystemOpenVO = new VersionSystemOpenVO(xe2);
					this.m_Versions.Add(versionSystemOpenVO);
				}
			}
		}

		public List<VersionSystemOpenVO> Versions
		{
			get
			{
				return this.m_Versions;
			}
			set
			{
				this.m_Versions = value;
			}
		}

		public VersionSystemOpenVO GetVersionByID(int id)
		{
			if (this.m_Versions == null)
			{
				return null;
			}
			return this.m_Versions.Find((VersionSystemOpenVO info) => info.ID == id);
		}

		private List<VersionSystemOpenVO> m_Versions;
	}
}
