using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUAwakenRecovery
	{
		public MUAwakenRecovery()
		{
		}

		public MUAwakenRecovery(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_GoodsID = XMLHelper.GetIntArrtibute(xe, "GoodsID");
			this.m_AwakenNum = XMLHelper.GetIntArrtibute(xe, "AwakenNum");
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

		public int GoodsID
		{
			get
			{
				return this.m_GoodsID;
			}
			set
			{
				this.m_GoodsID = value;
			}
		}

		public int AwakenNum
		{
			get
			{
				return this.m_AwakenNum;
			}
			set
			{
				this.m_AwakenNum = value;
			}
		}

		private int m_ID;

		private int m_GoodsID;

		private int m_AwakenNum;
	}
}
