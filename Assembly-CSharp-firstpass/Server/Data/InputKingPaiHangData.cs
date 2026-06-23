using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class InputKingPaiHangData
	{
		[ProtoMember(1)]
		public string UserID;

		[ProtoMember(2)]
		public int PaiHang;

		[ProtoMember(3)]
		public string PaiHangTime = string.Empty;

		[ProtoMember(4)]
		public int PaiHangValue;

		[ProtoMember(5)]
		public string MaxLevelRoleName = string.Empty;

		[ProtoMember(6)]
		public int MaxLevelRoleZoneID = 1;
	}
}
