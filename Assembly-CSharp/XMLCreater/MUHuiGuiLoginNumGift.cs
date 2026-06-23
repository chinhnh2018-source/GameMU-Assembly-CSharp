using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUHuiGuiLoginNumGift
	{
		public MUHuiGuiLoginNumGift()
		{
		}

		public MUHuiGuiLoginNumGift(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_HuoDongLevel = XMLHelper.GetIntArrtibute(xe, "HuoDongLevel");
			this.m_TimeOl = XMLHelper.GetIntArrtibute(xe, "TimeOl");
			this.m_GoodsID1 = XMLHelper.GetStringListArrtibute(xe, "GoodsID1", '|');
			this.m_GoodsID2 = XMLHelper.GetStringListArrtibute(xe, "GoodsID2", '|');
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

		public int HuoDongLevel
		{
			get
			{
				return this.m_HuoDongLevel;
			}
			set
			{
				this.m_HuoDongLevel = value;
			}
		}

		public int TimeOl
		{
			get
			{
				return this.m_TimeOl;
			}
			set
			{
				this.m_TimeOl = value;
			}
		}

		public List<string> GoodsID1
		{
			get
			{
				return this.m_GoodsID1;
			}
			set
			{
				this.m_GoodsID1 = value;
			}
		}

		public List<string> GoodsID2
		{
			get
			{
				return this.m_GoodsID2;
			}
			set
			{
				this.m_GoodsID2 = value;
			}
		}

		private int m_ID;

		private int m_HuoDongLevel;

		private int m_TimeOl;

		private List<string> m_GoodsID1;

		private List<string> m_GoodsID2;
	}
}
