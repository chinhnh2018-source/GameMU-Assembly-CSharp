using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class JunTuanMiniData
	{
		[ProtoMember(1)]
		public int JunTuanId;

		[ProtoMember(2)]
		public string JunTuanName;

		[ProtoMember(3)]
		public int LeaderZoneId;

		[ProtoMember(4)]
		public string LeaderName;

		[ProtoMember(5)]
		public int BangHuiNum;

		[ProtoMember(6)]
		public int LingDi;
	}
}
