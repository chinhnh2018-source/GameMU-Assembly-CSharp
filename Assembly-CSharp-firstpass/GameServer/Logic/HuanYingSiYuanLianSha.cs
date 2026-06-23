using System;
using ProtoBuf;

namespace GameServer.Logic
{
	[ProtoContract]
	public class HuanYingSiYuanLianSha
	{
		[ProtoMember(1)]
		public int ZoneID;

		[ProtoMember(2)]
		public string Name = string.Empty;

		[ProtoMember(3)]
		public int LianShaType;

		[ProtoMember(4)]
		public int Occupation;

		[ProtoMember(5)]
		public int ExtScore;

		[ProtoMember(6)]
		public int Side;
	}
}
