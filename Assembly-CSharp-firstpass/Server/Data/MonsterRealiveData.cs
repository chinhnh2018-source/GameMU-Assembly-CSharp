using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class MonsterRealiveData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public int PosX;

		[ProtoMember(3)]
		public int PosY;

		[ProtoMember(4)]
		public int Direction;
	}
}
