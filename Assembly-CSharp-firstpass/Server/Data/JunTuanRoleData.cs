using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class JunTuanRoleData
	{
		[ProtoMember(1)]
		public int RoleId;

		[ProtoMember(2)]
		public string RoleName;

		[ProtoMember(3)]
		public int ZoneId;

		[ProtoMember(4)]
		public string BhName;

		[ProtoMember(5)]
		public int BhZoneId;

		[ProtoMember(6)]
		public int ZhanLi;

		[ProtoMember(7)]
		public int JuTuanZhiWu;

		[ProtoMember(8)]
		public int ChangeLifeCount;

		[ProtoMember(9)]
		public int Level;

		[ProtoMember(10)]
		public int OnlineState;

		[ProtoMember(11)]
		public int BhId;

		[ProtoMember(12)]
		public int Occu;
	}
}
