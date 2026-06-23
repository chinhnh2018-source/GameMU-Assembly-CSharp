using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class MazingerStoreData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public int Type;

		[ProtoMember(3)]
		public int Stage;

		[ProtoMember(4)]
		public int StarLevel;

		[ProtoMember(5)]
		public int Exp;
	}
}
