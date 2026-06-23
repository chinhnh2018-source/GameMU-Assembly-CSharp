using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUForceCraftRewardAll
	{
		public MUForceCraftRewardAll()
		{
		}

		public MUForceCraftRewardAll(XElement xe, string attribute = "ForceCraftReward")
		{
			this.m_ForceCraftRewards = new List<MUForceCraftReward>();
			IEnumerable<XElement> enumerable = xe.Elements(attribute);
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUForceCraftReward muforceCraftReward = new MUForceCraftReward(xe2);
					this.m_ForceCraftRewards.Add(muforceCraftReward);
				}
			}
		}

		public List<MUForceCraftReward> ForceCraftRewards
		{
			get
			{
				return this.m_ForceCraftRewards;
			}
			set
			{
				this.m_ForceCraftRewards = value;
			}
		}

		public MUForceCraftReward GetForceCraftRewardByID(int id)
		{
			if (this.m_ForceCraftRewards == null)
			{
				return null;
			}
			return this.m_ForceCraftRewards.Find((MUForceCraftReward info) => info.ID == id);
		}

		private List<MUForceCraftReward> m_ForceCraftRewards;
	}
}
