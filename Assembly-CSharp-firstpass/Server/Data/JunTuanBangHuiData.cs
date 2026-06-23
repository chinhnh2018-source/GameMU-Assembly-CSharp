using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class JunTuanBangHuiData
	{
		[ProtoMember(1)]
		public int BhId;

		[ProtoMember(2)]
		public string BhName;

		[ProtoMember(3)]
		public int BhZoneId;

		[ProtoMember(4)]
		public int LeaderZoneId;

		[ProtoMember(5)]
		public string LeaderName;

		[ProtoMember(6)]
		public int RoleNum;

		[ProtoMember(7)]
		public long ZhanLi;

		[ProtoMember(8)]
		public int JuTuanZhiWu;

		[ProtoMember(9)]
		public int LeaderOccupation;

		[ProtoMember(10)]
		public int LeaderRoleId;

		[NonSerialized]
		public long LastRequestJoinTicks;

		[NonSerialized]
		public long LastCreateTicks;
	}
}
