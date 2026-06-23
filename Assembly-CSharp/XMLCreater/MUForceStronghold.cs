using System;
using Tmsk.Xml;
using UnityEngine;

namespace XMLCreater
{
	public class MUForceStronghold
	{
		public MUForceStronghold()
		{
		}

		public MUForceStronghold(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_MapCode = XMLHelper.GetIntArrtibute(xe, "MapCode");
			this.m_QiZhiID = XMLHelper.GetStringArrtibute(xe, "QiZhiID");
			this.m_Name = XMLHelper.GetStringArrtibute(xe, "Name");
			this.m_QiZuoID = XMLHelper.GetIntArrtibute(xe, "QiZuoID");
			this.m_QiZuoSite = XMLHelper.GetStringArrtibute(xe, "QiZuoSite");
			this.m_QiZuoSitePosition = Vector2.zero;
			string[] array = this.m_QiZuoSite.Split(new char[]
			{
				'|'
			});
			if (array.Length == 2)
			{
				int num = array[0].SafeToInt32(0);
				int num2 = array[1].SafeToInt32(0);
				this.m_QiZuoSitePosition.x = (float)num;
				this.m_QiZuoSitePosition.y = (float)num2;
			}
			this.m_Point = XMLHelper.GetIntArrtibute(xe, "Point");
			this.m_Rate = XMLHelper.GetFloatArrtibute(xe, "Rate");
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

		public string QiZhiID
		{
			get
			{
				return this.m_QiZhiID;
			}
			set
			{
				this.m_QiZhiID = value;
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

		public int QiZuoID
		{
			get
			{
				return this.m_QiZuoID;
			}
			set
			{
				this.m_QiZuoID = value;
			}
		}

		public string QiZuoSite
		{
			get
			{
				return this.m_QiZuoSite;
			}
			set
			{
				this.m_QiZuoSite = value;
			}
		}

		public Vector2 QiZuoSitePosition
		{
			get
			{
				return this.m_QiZuoSitePosition;
			}
		}

		public int Point
		{
			get
			{
				return this.m_Point;
			}
			set
			{
				this.m_Point = value;
			}
		}

		public float Rate
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

		private int m_ID;

		private int m_MapCode;

		private string m_QiZhiID;

		private string m_Name;

		private int m_QiZuoID;

		private string m_QiZuoSite;

		private Vector2 m_QiZuoSitePosition;

		private int m_Point;

		private float m_Rate;
	}
}
