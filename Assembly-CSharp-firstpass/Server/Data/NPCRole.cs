using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class NPCRole
	{
		[ProtoMember(1)]
		public int NpcID;

		[ProtoMember(2)]
		public int PosX;

		[ProtoMember(3)]
		public int PosY;

		[ProtoMember(4)]
		public int MapCode = -1;

		[ProtoMember(5)]
		public string RoleString = string.Empty;

		[ProtoMember(6)]
		public int Dir;
	}
}
