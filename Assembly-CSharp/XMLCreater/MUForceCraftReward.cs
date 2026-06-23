using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUForceCraftReward
	{
		public MUForceCraftReward()
		{
		}

		public MUForceCraftReward(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_Rank = XMLHelper.GetIntArrtibute(xe, "Rank");
			this.m_RankRate = XMLHelper.GetFloatArrtibute(xe, "RankRate");
			if (xe.HasAttribute("Grade"))
			{
				this.m_Grade = XMLHelper.GetIntArrtibute(xe, "Grade");
			}
			else if (xe.HasAttribute("CompHonor"))
			{
				this.m_Contribution = XMLHelper.GetIntArrtibute(xe, "CompHonor");
			}
			if (xe.HasAttribute("Contribution"))
			{
				this.m_Contribution = XMLHelper.GetIntArrtibute(xe, "Contribution");
			}
			else if (xe.HasAttribute("CompFeast"))
			{
				this.m_Grade = XMLHelper.GetIntArrtibute(xe, "CompFeast");
			}
			this.m_GoodsOne = XMLHelper.GetStringArrtibute(xe, "GoodsOne");
			this.m_GoodsTwo = XMLHelper.GetStringArrtibute(xe, "GoodsTwo");
			this.m_lstGoodsOne = new List<string>();
			this.m_lstGoodsTwo = new List<string>();
			if (this.m_GoodsOne != string.Empty)
			{
				string[] array = this.m_GoodsOne.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					this.m_lstGoodsOne.Add(array[i]);
				}
			}
			if (this.m_GoodsTwo != string.Empty)
			{
				string[] array2 = this.m_GoodsTwo.Split(new char[]
				{
					'|'
				});
				for (int j = 0; j < array2.Length; j++)
				{
					this.m_lstGoodsTwo.Add(array2[j]);
				}
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

		public int Rank
		{
			get
			{
				return this.m_Rank;
			}
			set
			{
				this.m_Rank = value;
			}
		}

		public float RankRate
		{
			get
			{
				return this.m_RankRate;
			}
			set
			{
				this.m_RankRate = value;
			}
		}

		public int Grade
		{
			get
			{
				return this.m_Grade;
			}
			set
			{
				this.m_Grade = value;
			}
		}

		public int Contribution
		{
			get
			{
				return this.m_Contribution;
			}
			set
			{
				this.m_Contribution = value;
			}
		}

		public string GoodsOne
		{
			get
			{
				return this.m_GoodsOne;
			}
			set
			{
				this.m_GoodsOne = value;
			}
		}

		public List<string> LstGoodsOne
		{
			get
			{
				return this.m_lstGoodsOne;
			}
		}

		public string GoodsTwo
		{
			get
			{
				return this.m_GoodsTwo;
			}
			set
			{
				this.m_GoodsTwo = value;
			}
		}

		public List<string> LstGoodsTwo
		{
			get
			{
				return this.m_lstGoodsTwo;
			}
		}

		private int m_ID;

		private int m_Rank;

		private float m_RankRate;

		private int m_Grade;

		private int m_Contribution;

		private string m_GoodsOne;

		private List<string> m_lstGoodsOne;

		private string m_GoodsTwo;

		private List<string> m_lstGoodsTwo;
	}
}
