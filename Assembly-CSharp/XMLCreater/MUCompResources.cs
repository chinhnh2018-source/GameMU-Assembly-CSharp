using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUCompResources
	{
		public MUCompResources()
		{
		}

		public MUCompResources(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_MapID = XMLHelper.GetIntArrtibute(xe, "MapID");
			this.m_normalMonstersID = XMLHelper.GetIntArrtibute(xe, "MonstersID");
			this.m_Site = XMLHelper.GetStringArrtibute(xe, "Site");
			this.m_GrowTime = XMLHelper.GetIntArrtibute(xe, "GrowTime");
			this.m_CollectTime = XMLHelper.GetIntArrtibute(xe, "CollectTime");
			this.m_AutoCollectTime = XMLHelper.GetIntArrtibute(xe, "AutoCollectTime");
			this.m_RefreshTime = XMLHelper.GetStringArrtibute(xe, "RefreshTime");
			this.m_CompNum = XMLHelper.GetIntArrtibute(xe, "CompNum");
			this.m_CompHonor = XMLHelper.GetIntArrtibute(xe, "CompHonor");
			this.m_CompFeast = XMLHelper.GetIntArrtibute(xe, "CompFeast");
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

		public int MapID
		{
			get
			{
				return this.m_MapID;
			}
			set
			{
				this.m_MapID = value;
			}
		}

		public string MonstersIDStr
		{
			get
			{
				return this.m_MonstersIDStr;
			}
			set
			{
				this.m_MonstersIDStr = value;
			}
		}

		public int NormalMonstersID
		{
			get
			{
				return this.m_normalMonstersID;
			}
			set
			{
				this.m_normalMonstersID = value;
			}
		}

		public int FinalMonstersID
		{
			get
			{
				return this.m_finalMonstersID;
			}
			set
			{
				this.m_finalMonstersID = value;
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

		public int GrowTime
		{
			get
			{
				return this.m_GrowTime;
			}
			set
			{
				this.m_GrowTime = value;
			}
		}

		public int CollectTime
		{
			get
			{
				return this.m_CollectTime;
			}
			set
			{
				this.m_CollectTime = value;
			}
		}

		public int AutoCollectTime
		{
			get
			{
				return this.m_AutoCollectTime;
			}
			set
			{
				this.m_AutoCollectTime = value;
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

		public int CompNum
		{
			get
			{
				return this.m_CompNum;
			}
			set
			{
				this.m_CompNum = value;
			}
		}

		public int CompHonor
		{
			get
			{
				return this.m_CompHonor;
			}
			set
			{
				this.m_CompHonor = value;
			}
		}

		public int CompFeast
		{
			get
			{
				return this.m_CompFeast;
			}
			set
			{
				this.m_CompFeast = value;
			}
		}

		private int m_ID;

		private int m_MapID;

		private string m_MonstersIDStr;

		private int m_normalMonstersID;

		private int m_finalMonstersID;

		private string m_Site;

		private int m_GrowTime;

		private int m_CollectTime;

		private int m_AutoCollectTime;

		private string m_RefreshTime;

		private int m_CompNum;

		private int m_CompHonor;

		private int m_CompFeast;
	}
}
