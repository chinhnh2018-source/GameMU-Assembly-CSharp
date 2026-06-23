using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MURebornSuperiorType
	{
		public MURebornSuperiorType()
		{
		}

		public MURebornSuperiorType(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_Name = XMLHelper.GetStringArrtibute(xe, "Name");
			this.m_Type = XMLHelper.GetStringArrtibute(xe, "Type");
			this.m_Parameter = XMLHelper.GetStringArrtibute(xe, "Parameter");
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

		public string Type
		{
			get
			{
				return this.m_Type;
			}
			set
			{
				this.m_Type = value;
			}
		}

		public string Parameter
		{
			get
			{
				return this.m_Parameter;
			}
			set
			{
				this.m_Parameter = value;
			}
		}

		private int m_ID;

		private string m_Name;

		private string m_Type;

		private string m_Parameter;
	}
}
