using System;
using HSGameEngine.Drawing;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUFuBenMuBiao
	{
		public MUFuBenMuBiao()
		{
		}

		public MUFuBenMuBiao(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_FuBenID = XMLHelper.GetIntArrtibute(xe, "FuBenID");
			this.m_Name = XMLHelper.GetStringArrtibute(xe, "Name");
			this.m_MuBiaoID = XMLHelper.GetIntArrtibute(xe, "MuBiaoID");
			this.m_MuBiaoCanShu = XMLHelper.GetStringArrtibute(xe, "MuBiaoCanShu");
			string[] array = this.m_MuBiaoCanShu.Split(new char[]
			{
				','
			});
			if (array.Length == 2)
			{
				this.m_MonsterId = array[0].SafeToInt32(0);
				this.m_MonsterNum = array[1].SafeToInt32(0);
			}
			string stringArrtibute = XMLHelper.GetStringArrtibute(xe, "MuBiaoPoint");
			this.m_MuBiaoPoint = new Point(-1, -1);
			string[] array2 = stringArrtibute.Split(new char[]
			{
				','
			});
			if (array2.Length == 2)
			{
				this.m_MuBiaoPoint.X = array2[0].SafeToInt32(0);
				this.m_MuBiaoPoint.Y = array2[1].SafeToInt32(0);
			}
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

		public int FuBenID
		{
			get
			{
				return this.m_FuBenID;
			}
			set
			{
				this.m_FuBenID = value;
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

		public int MuBiaoID
		{
			get
			{
				return this.m_MuBiaoID;
			}
			set
			{
				this.m_MuBiaoID = value;
			}
		}

		public string MuBiaoCanShu
		{
			get
			{
				return this.m_MuBiaoCanShu;
			}
			set
			{
				this.m_MuBiaoCanShu = value;
			}
		}

		public int MonsterId
		{
			get
			{
				return this.m_MonsterId;
			}
		}

		public int MonsterNum
		{
			get
			{
				return this.m_MonsterNum;
			}
		}

		public Point MuBiaoPoint
		{
			get
			{
				return this.m_MuBiaoPoint;
			}
		}

		private int m_ID;

		private int m_FuBenID;

		private string m_Name;

		private int m_MuBiaoID;

		private string m_MuBiaoCanShu;

		private int m_MonsterId;

		private int m_MonsterNum;

		private Point m_MuBiaoPoint;
	}
}
