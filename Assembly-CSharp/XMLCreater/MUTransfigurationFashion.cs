using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUTransfigurationFashion
	{
		public MUTransfigurationFashion()
		{
		}

		public MUTransfigurationFashion(XElement xe)
		{
			this.m_ID = XMLHelper.GetIntArrtibute(xe, "ID");
			this.m_GoodsID = XMLHelper.GetIntArrtibute(xe, "GoodsID");
			this.m_Name = XMLHelper.GetStringArrtibute(xe, "Name");
			this.m_MOD = XMLHelper.GetIntArrtibute(xe, "MOD");
			this.m_level = XMLHelper.GetIntArrtibute(xe, "level");
			string stringArrtibute = XMLHelper.GetStringArrtibute(xe, "NeedGoods");
			this.m_NeedGoods = new List<MUMaterialInfo>();
			if (stringArrtibute != string.Empty)
			{
				string[] array = stringArrtibute.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					MUMaterialInfo mumaterialInfo = new MUMaterialInfo(array[i]);
					this.m_NeedGoods.Add(mumaterialInfo);
				}
			}
			string stringArrtibute2 = XMLHelper.GetStringArrtibute(xe, "ProPerty");
			this.m_ProPerty = MUPropInfoHelper.DeserializeToPropList(stringArrtibute2);
			this.m_Time = XMLHelper.GetIntArrtibute(xe, "Time");
			this.m_AttackSkill = XMLHelper.GetIntListArrtibute(xe, "AttackSkill", ',');
			this.m_MagicSkill = XMLHelper.GetIntListArrtibute(xe, "MagicSkill", ',');
			this.m_Effect = XMLHelper.GetIntArrtibute(xe, "Effect");
			this.m_Duration = XMLHelper.GetIntArrtibute(xe, "Duration");
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

		public int MOD
		{
			get
			{
				return this.m_MOD;
			}
			set
			{
				this.m_MOD = value;
			}
		}

		public int level
		{
			get
			{
				return this.m_level;
			}
			set
			{
				this.m_level = value;
			}
		}

		public List<MUMaterialInfo> NeedGoods
		{
			get
			{
				return this.m_NeedGoods;
			}
			set
			{
				this.m_NeedGoods = value;
			}
		}

		public List<MUPropInfo> ProPerty
		{
			get
			{
				return this.m_ProPerty;
			}
			set
			{
				this.m_ProPerty = value;
			}
		}

		public int Time
		{
			get
			{
				return this.m_Time;
			}
			set
			{
				this.m_Time = value;
			}
		}

		public List<int> AttackSkill
		{
			get
			{
				return this.m_AttackSkill;
			}
			set
			{
				this.m_AttackSkill = value;
			}
		}

		public List<int> MagicSkill
		{
			get
			{
				return this.m_MagicSkill;
			}
			set
			{
				this.m_MagicSkill = value;
			}
		}

		public int Effect
		{
			get
			{
				return this.m_Effect;
			}
			set
			{
				this.m_Effect = value;
			}
		}

		public int Duration
		{
			get
			{
				return this.m_Duration;
			}
			set
			{
				this.m_Duration = value;
			}
		}

		private int m_ID;

		private int m_GoodsID;

		private string m_Name;

		private int m_MOD;

		private int m_level;

		private List<MUMaterialInfo> m_NeedGoods;

		private List<MUPropInfo> m_ProPerty;

		private int m_Time;

		private List<int> m_AttackSkill;

		private List<int> m_MagicSkill;

		private int m_Effect;

		private int m_Duration;
	}
}
