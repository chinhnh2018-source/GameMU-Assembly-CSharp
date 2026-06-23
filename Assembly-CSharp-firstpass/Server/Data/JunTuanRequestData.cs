using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class JunTuanRequestData
	{
		[ProtoMember(1)]
		public int JunTuanId;

		[ProtoMember(2)]
		public int BhId;

		[ProtoMember(3)]
		public string BhName;

		[ProtoMember(4)]
		public int BhZoneId;

		[ProtoMember(5)]
		public int RoleNum;

		[ProtoMember(6)]
		public long ZhanLi;

		[ProtoMember(7)]
		public int LeaderZoneId;

		[ProtoMember(8)]
		public string LeaderName;

		[ProtoMember(9)]
		public string JunTuanName;

		[ProtoMember(10)]
		public int Occupation;

		[ProtoMember(11)]
		public int LeaderRoleId;
	}
}
