using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class PetData
	{
		[ProtoMember(1)]
		public int DbID;

		[ProtoMember(2)]
		public int PetID;

		[ProtoMember(3)]
		public string PetName = string.Empty;

		[ProtoMember(4)]
		public int PetType;

		[ProtoMember(5)]
		public int FeedNum;

		[ProtoMember(6)]
		public int ReAliveNum;

		[ProtoMember(7)]
		public long AddDateTime;

		[ProtoMember(8)]
		public string PetProps = string.Empty;

		[ProtoMember(9)]
		public int Level = 1;
	}
}
