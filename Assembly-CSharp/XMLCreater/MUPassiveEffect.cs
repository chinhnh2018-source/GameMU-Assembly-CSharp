using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUPassiveEffect
	{
		public MUPassiveEffect()
		{
		}

		public MUPassiveEffect(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_Name = XMLHelper.GetStringArrtibute(xe, "Name");
			this.m_Description = XMLHelper.GetStringArrtibute(xe, "Description");
			this.m_Type = XMLHelper.GetIntArrtibute(xe, "Type");
			this.m_Rate = XMLHelper.GetIntArrtibute(xe, "Rate");
			this.m_MagicScripts = XMLHelper.GetStringArrtibute(xe, "MagicScripts");
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

		public string Description
		{
			get
			{
				return this.m_Description;
			}
			set
			{
				this.m_Description = value;
			}
		}

		public int Type
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

		public int Rate
		{
			get
			{
				return this.m_Rate;
			}
			set
			{
				this.m_Rate = value;
			}
		}

		public string MagicScripts
		{
			get
			{
				return this.m_MagicScripts;
			}
			set
			{
				this.m_MagicScripts = value;
			}
		}

		private int m_ID;

		private string m_Name;

		private string m_Description;

		private int m_Type;

		private int m_Rate;

		private string m_MagicScripts;
	}
}
