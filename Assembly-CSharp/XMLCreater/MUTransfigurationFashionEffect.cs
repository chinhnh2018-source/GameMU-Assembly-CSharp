using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUTransfigurationFashionEffect
	{
		public MUTransfigurationFashionEffect()
		{
		}

		public MUTransfigurationFashionEffect(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_Name = XMLHelper.GetStringArrtibute(xe, "Name");
			this.m_Level = XMLHelper.GetIntArrtibute(xe, "Level");
			this.m_MagicIcon = XMLHelper.GetIntArrtibute(xe, "MagicIcon");
			this.m_Description = XMLHelper.GetStringArrtibute(xe, "Description");
			this.m_ProPerty = XMLHelper.GetStringArrtibute(xe, "ProPerty");
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

		public int Level
		{
			get
			{
				return this.m_Level;
			}
			set
			{
				this.m_Level = value;
			}
		}

		public int MagicIcon
		{
			get
			{
				return this.m_MagicIcon;
			}
			set
			{
				this.m_MagicIcon = value;
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

		public string ProPerty
		{
			get
			{
				return this.m_ProPerty;
			}
			set
			{
				this.m_ProPerty = value;
			}
		}

		private int m_ID;

		private string m_Name;

		private int m_Level;

		private int m_MagicIcon;

		private string m_Description;

		private string m_ProPerty;
	}
}
