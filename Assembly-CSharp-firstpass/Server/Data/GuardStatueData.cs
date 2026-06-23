using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class GuardStatueData
	{
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"level, ",
				this.level,
				", grade, ",
				this.grade,
				", hasGuardPoint, ",
				this.hasGuardPoint,
				", activetedSouls, ",
				this.SoulGuardListToString()
			});
		}

		private string SoulGuardListToString()
		{
			string text = string.Empty;
			for (int i = 0; i < this.soulGuardList.Count; i++)
			{
				GuardSoulData guardSoulData = this.soulGuardList[i];
				if (guardSoulData != null)
				{
					text += guardSoulData.ToString();
				}
			}
			return text;
		}

		[ProtoMember(1, IsRequired = true)]
		public int level;

		[ProtoMember(2, IsRequired = true)]
		public int grade;

		[ProtoMember(3, IsRequired = true)]
		public int hasGuardPoint;

		[ProtoMember(4, IsRequired = true)]
		public List<GuardSoulData> soulGuardList = new List<GuardSoulData>();
	}
}
