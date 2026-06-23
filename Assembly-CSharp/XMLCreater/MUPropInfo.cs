using System;

namespace XMLCreater
{
	public class MUPropInfo
	{
		public MUPropInfo(string propName, float propNum)
		{
			this.m_propName = propName;
			this.m_propNum = propNum;
		}

		public MUPropInfo(string content)
		{
			string[] array = content.Split(new char[]
			{
				','
			});
			if (array.Length == 2)
			{
				this.m_propName = array[0];
				if (!float.TryParse(array[1], ref this.m_propNum))
				{
				}
			}
		}

		public string PropName
		{
			get
			{
				return this.m_propName;
			}
			set
			{
				this.m_propName = value;
			}
		}

		public float PropNum
		{
			get
			{
				return this.m_propNum;
			}
			set
			{
				this.m_propNum = value;
			}
		}

		public string ChinesePropName
		{
			get
			{
				return ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(this.PropName, false);
			}
		}

		public bool BePercent
		{
			get
			{
				return ConfigExtPropIndexes.GetExtPropIndexesPercentByWord(this.PropName) > 0f;
			}
		}

		private string m_propName = string.Empty;

		private float m_propNum;
	}
}
