using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUCompLevel
	{
		public MUCompLevel()
		{
		}

		public MUCompLevel(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_CompID = XMLHelper.GetIntArrtibute(xe, "CompID");
			this.m_Level = XMLHelper.GetIntArrtibute(xe, "Level");
			this.m_Name = XMLHelper.GetStringArrtibute(xe, "Name");
			this.m_ChatName = XMLHelper.GetStringArrtibute(xe, "ChatName");
			this.m_Enemy = XMLHelper.GetIntArrtibute(xe, "Enemy");
			this.m_Buff = XMLHelper.GetIntArrtibute(xe, "Buff");
			this.m_CraftSelfBuffID = XMLHelper.GetIntArrtibute(xe, "CraftSelfBuffID");
			this.m_CraftBuffID = XMLHelper.GetIntArrtibute(xe, "CraftBuffID");
			this.m_ModEnlarge = XMLHelper.GetFloatArrtibute(xe, "ModEnlarge");
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

		public string ChatName
		{
			get
			{
				return this.m_ChatName;
			}
			set
			{
				this.m_ChatName = value;
			}
		}

		public int Enemy
		{
			get
			{
				return this.m_Enemy;
			}
			set
			{
				this.m_Enemy = value;
			}
		}

		public int Buff
		{
			get
			{
				return this.m_Buff;
			}
			set
			{
				this.m_Buff = value;
			}
		}

		public int CraftSelfBuffID
		{
			get
			{
				return this.m_CraftSelfBuffID;
			}
			set
			{
				this.m_CraftSelfBuffID = value;
			}
		}

		public int CraftBuffID
		{
			get
			{
				return this.m_CraftBuffID;
			}
			set
			{
				this.m_CraftBuffID = value;
			}
		}

		public float ModEnlarge
		{
			get
			{
				return this.m_ModEnlarge;
			}
			set
			{
				this.m_ModEnlarge = value;
			}
		}

		private int m_ID;

		private int m_CompID;

		private int m_Level;

		private string m_Name;

		private string m_ChatName;

		private int m_Enemy;

		private int m_Buff;

		private int m_CraftSelfBuffID;

		private int m_CraftBuffID;

		private float m_ModEnlarge;
	}
}
