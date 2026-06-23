using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUHuiGuiDayZhiGou
	{
		public MUHuiGuiDayZhiGou()
		{
		}

		public MUHuiGuiDayZhiGou(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_HuoDongLevel = XMLHelper.GetIntArrtibute(xe, "HuoDongLevel");
			this.m_TotalYuanBao = XMLHelper.GetIntListArrtibute(xe, "TotalYuanBao", ',');
			this.m_Day = XMLHelper.GetIntArrtibute(xe, "Day");
			List<int> intListArrtibute = XMLHelper.GetIntListArrtibute(xe, "Price", '|');
			this.m_price = intListArrtibute[0];
			this.m_chongZhiId = intListArrtibute[1];
			this.m_zhiGouId = intListArrtibute[2];
			this.m_GoodsID1 = XMLHelper.GetStringListArrtibute(xe, "GoodsID1", '|');
			this.m_GoodsID2 = XMLHelper.GetStringListArrtibute(xe, "GoodsID2", '|');
			this.m_Max = XMLHelper.GetIntArrtibute(xe, "Max");
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

		public List<int> TotalYuanBao
		{
			get
			{
				return this.m_TotalYuanBao;
			}
			set
			{
				this.m_TotalYuanBao = value;
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

		public int Price
		{
			get
			{
				return this.m_price;
			}
			set
			{
				this.m_price = value;
			}
		}

		public int ChongZhiId
		{
			get
			{
				return this.m_chongZhiId;
			}
			set
			{
				this.m_chongZhiId = value;
			}
		}

		public int ZhiGouId
		{
			get
			{
				return this.m_zhiGouId;
			}
			set
			{
				this.m_zhiGouId = value;
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

		public int Max
		{
			get
			{
				return this.m_Max;
			}
			set
			{
				this.m_Max = value;
			}
		}

		private int m_ID;

		private int m_HuoDongLevel;

		private List<int> m_TotalYuanBao;

		private int m_Day;

		private int m_price;

		private int m_chongZhiId;

		private int m_zhiGouId;

		private List<string> m_GoodsID1;

		private List<string> m_GoodsID2;

		private int m_Max;
	}
}
