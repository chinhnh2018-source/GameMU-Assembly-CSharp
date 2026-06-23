using System;
using ProtoBuf;

namespace Server.Data
{
	public class HeFuPKKingData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string RoleName = string.Empty;

		[ProtoMember(3)]
		public int ZoneID;

		[ProtoMember(4)]
		public int State;
	}
}
