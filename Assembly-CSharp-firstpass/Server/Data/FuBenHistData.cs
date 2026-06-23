using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class FuBenHistData
	{
		[ProtoMember(1)]
		public int FuBenID;

		[ProtoMember(2)]
		public int RoleID;

		[ProtoMember(3)]
		public string RoleName = string.Empty;

		[ProtoMember(4)]
		public int UsedSecs;
	}
}
