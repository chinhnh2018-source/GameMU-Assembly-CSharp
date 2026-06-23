using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUFuWenCommend
	{
		public MUFuWenCommend()
		{
		}

		public MUFuWenCommend(XElement xe)
		{
			this.m_OccupationID = XMLHelper.GetIntArrtibute(xe, "OccupationID");
			this.m_FuWenID = new List<int>();
			string stringArrtibute = XMLHelper.GetStringArrtibute(xe, "FuWenID");
			string[] array = stringArrtibute.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				this.m_FuWenID.Add(array[i].SafeToInt32(0));
			}
			this.m_ShenShiID = new List<int>();
			string stringArrtibute2 = XMLHelper.GetStringArrtibute(xe, "ShenShiID");
			string[] array2 = stringArrtibute2.Split(new char[]
			{
				'|'
			});
			for (int j = 0; j < array2.Length; j++)
			{
				this.m_ShenShiID.Add(array2[j].SafeToInt32(0));
			}
		}

		public int OccupationID
		{
			get
			{
				return this.m_OccupationID;
			}
			set
			{
				this.m_OccupationID = value;
			}
		}

		public List<int> FuWenID
		{
			get
			{
				return this.m_FuWenID;
			}
			set
			{
				this.m_FuWenID = value;
			}
		}

		public List<int> ShenShiID
		{
			get
			{
				return this.m_ShenShiID;
			}
			set
			{
				this.m_ShenShiID = value;
			}
		}

		private int m_OccupationID;

		private List<int> m_FuWenID;

		private List<int> m_ShenShiID;
	}
}
