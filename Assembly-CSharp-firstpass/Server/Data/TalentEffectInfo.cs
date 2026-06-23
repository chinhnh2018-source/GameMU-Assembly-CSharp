using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class TalentEffectInfo
	{
		[ProtoMember(1, IsRequired = true)]
		public int EffectType;

		[ProtoMember(2, IsRequired = true)]
		public int EffectID;

		[ProtoMember(3, IsRequired = true)]
		public double EffectValue;
	}
}
