using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUForceCraft
	{
		public MUForceCraft()
		{
		}

		public MUForceCraft(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_Name = XMLHelper.GetStringArrtibute(xe, "Name");
			this.m_MapCode = XMLHelper.GetIntArrtibute(xe, "MapCode");
			this.m_TimePoints = XMLHelper.GetStringArrtibute(xe, "TimePoints");
			this.m_ForceMax = XMLHelper.GetIntArrtibute(xe, "ForceMax");
			this.m_MaxEnterNum = XMLHelper.GetIntArrtibute(xe, "MaxEnterNum");
			this.m_EnterCD = XMLHelper.GetIntArrtibute(xe, "EnterCD");
			this.m_PrepareSecs = XMLHelper.GetIntArrtibute(xe, "PrepareSecs");
			this.m_FightingSecs = XMLHelper.GetIntArrtibute(xe, "FightingSecs");
			this.m_ClearRolesSecs = XMLHelper.GetIntArrtibute(xe, "ClearRolesSecs");
			this.m_Reward = XMLHelper.GetStringArrtibute(xe, "Reward");
			this.m_DuiHuanType = XMLHelper.GetIntArrtibute(xe, "DuiHuanType");
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

		public string TimePoints
		{
			get
			{
				return this.m_TimePoints;
			}
			set
			{
				this.m_TimePoints = value;
			}
		}

		public int ForceMax
		{
			get
			{
				return this.m_ForceMax;
			}
			set
			{
				this.m_ForceMax = value;
			}
		}

		public int MaxEnterNum
		{
			get
			{
				return this.m_MaxEnterNum;
			}
			set
			{
				this.m_MaxEnterNum = value;
			}
		}

		public int EnterCD
		{
			get
			{
				return this.m_EnterCD;
			}
			set
			{
				this.m_EnterCD = value;
			}
		}

		public int PrepareSecs
		{
			get
			{
				return this.m_PrepareSecs;
			}
			set
			{
				this.m_PrepareSecs = value;
			}
		}

		public int FightingSecs
		{
			get
			{
				return this.m_FightingSecs;
			}
			set
			{
				this.m_FightingSecs = value;
			}
		}

		public int ClearRolesSecs
		{
			get
			{
				return this.m_ClearRolesSecs;
			}
			set
			{
				this.m_ClearRolesSecs = value;
			}
		}

		public string Reward
		{
			get
			{
				return this.m_Reward;
			}
			set
			{
				this.m_Reward = value;
			}
		}

		public int DuiHuanType
		{
			get
			{
				return this.m_DuiHuanType;
			}
			set
			{
				this.m_DuiHuanType = value;
			}
		}

		private int m_ID;

		private string m_Name;

		private int m_MapCode;

		private string m_TimePoints;

		private int m_ForceMax;

		private int m_MaxEnterNum;

		private int m_EnterCD;

		private int m_PrepareSecs;

		private int m_FightingSecs;

		private int m_ClearRolesSecs;

		private string m_Reward;

		private int m_DuiHuanType;
	}
}
