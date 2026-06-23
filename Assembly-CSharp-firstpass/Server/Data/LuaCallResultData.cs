using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class LuaCallResultData
	{
		[ProtoMember(1)]
		public int MapCode;

		[ProtoMember(2)]
		public int RoleID;

		[ProtoMember(3)]
		public int NPCID;

		[ProtoMember(4)]
		public int IsSuccess;

		[ProtoMember(5)]
		public string Result;

		[ProtoMember(6)]
		public int Tag;

		[ProtoMember(7)]
		public int ExtensionID;

		[ProtoMember(8)]
		public string LuaFunction;

		[ProtoMember(9)]
		public int ForceRefresh;
	}
}
