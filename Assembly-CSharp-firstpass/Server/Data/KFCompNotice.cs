using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KFCompNotice
	{
		[ProtoMember(1)]
		public int NoticeID;

		[ProtoMember(2)]
		public int CompType;

		[ProtoMember(3)]
		public string Param1 = string.Empty;

		[ProtoMember(4)]
		public string Param2 = string.Empty;

		[ProtoMember(5)]
		public int toMapCode;

		[ProtoMember(6)]
		public int toPosX;

		[ProtoMember(7)]
		public int toPosY;
	}
}
