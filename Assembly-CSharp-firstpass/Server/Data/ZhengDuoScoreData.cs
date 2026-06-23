using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZhengDuoScoreData
	{
		[ProtoMember(1)]
		public string Name = string.Empty;

		[ProtoMember(2)]
		public long Hurt;

		[ProtoMember(3)]
		public int Id;
	}
}
