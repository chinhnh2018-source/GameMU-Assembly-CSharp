using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class JunTuanRankData
	{
		[ProtoMember(1)]
		public int JunTuanId;

		[ProtoMember(2)]
		public string JunTuanName;

		[ProtoMember(3)]
		public int Point;

		[ProtoMember(4)]
		public DateTime LastTime;

		[ProtoMember(5)]
		public int Rank;
	}
}
