using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class TalentData
	{
		[ProtoMember(1, IsRequired = true)]
		public bool IsOpen;

		[ProtoMember(2, IsRequired = true)]
		public int TotalCount;

		[ProtoMember(3, IsRequired = true)]
		public long Exp;

		[ProtoMember(4, IsRequired = true)]
		public Dictionary<int, int> CountList = new Dictionary<int, int>();

		[ProtoMember(5, IsRequired = true)]
		public List<TalentEffectItem> EffectList = new List<TalentEffectItem>();

		[ProtoMember(6, IsRequired = true)]
		public Dictionary<int, int> SkillOneValue = new Dictionary<int, int>();

		[ProtoMember(7, IsRequired = true)]
		public int SkillAllValue = 2;

		[ProtoMember(8, IsRequired = true)]
		public int State;

		[ProtoMember(9, IsRequired = true)]
		public int Occupation;
	}
}
