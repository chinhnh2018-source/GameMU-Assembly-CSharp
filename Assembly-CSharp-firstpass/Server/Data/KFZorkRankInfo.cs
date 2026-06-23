using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class KFZorkRankInfo
	{
		[ProtoMember(1)]
		public int Key;

		[ProtoMember(2)]
		public int Value;

		[ProtoMember(3)]
		public int Param1;

		[ProtoMember(4)]
		public string StrParam1 = string.Empty;
	}
}
