using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUCompSolder
	{
		public MUCompSolder()
		{
		}

		public MUCompSolder(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_CompID = XMLHelper.GetIntArrtibute(xe, "CompID");
			this.m_Level = XMLHelper.GetIntArrtibute(xe, "Level");
			this.m_MonstersID = XMLHelper.GetIntArrtibute(xe, "MonstersID");
			this.m_AlarmTime = XMLHelper.GetIntArrtibute(xe, "AlarmTime");
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

		public int AlarmTime
		{
			get
			{
				return this.m_AlarmTime;
			}
			set
			{
				this.m_AlarmTime = value;
			}
		}

		private int m_ID;

		private int m_CompID;

		private int m_Level;

		private int m_MonstersID;

		private int m_AlarmTime;
	}
}
