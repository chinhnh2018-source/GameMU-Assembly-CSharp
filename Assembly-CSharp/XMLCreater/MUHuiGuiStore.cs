using System;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUHuiGuiStore
	{
		public MUHuiGuiStore()
		{
		}

		public MUHuiGuiStore(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_HuoDongLevel = XMLHelper.GetIntArrtibute(xe, "HuoDongLevel");
			this.m_Day = XMLHelper.GetIntArrtibute(xe, "Day");
			this.m_GoodsID = XMLHelper.GetStringArrtibute(xe, "GoodsID");
			this.m_OrigPrice = XMLHelper.GetIntArrtibute(xe, "OrigPrice");
			this.m_Price = XMLHelper.GetIntArrtibute(xe, "Price");
			this.m_SinglePurchase = XMLHelper.GetIntArrtibute(xe, "SinglePurchase");
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

		public int Day
		{
			get
			{
				return this.m_Day;
			}
			set
			{
				this.m_Day = value;
			}
		}

		public string GoodsID
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

		public int OrigPrice
		{
			get
			{
				return this.m_OrigPrice;
			}
			set
			{
				this.m_OrigPrice = value;
			}
		}

		public int Price
		{
			get
			{
				return this.m_Price;
			}
			set
			{
				this.m_Price = value;
			}
		}

		public int SinglePurchase
		{
			get
			{
				return this.m_SinglePurchase;
			}
			set
			{
				this.m_SinglePurchase = value;
			}
		}

		private int m_ID;

		private int m_HuoDongLevel;

		private int m_Day;

		private string m_GoodsID;

		private int m_OrigPrice;

		private int m_Price;

		private int m_SinglePurchase;
	}
}
