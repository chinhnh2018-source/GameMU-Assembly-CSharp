using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class VerifyPlayerRecallData
	{
		[ProtoMember(1)]
		public string userID = string.Empty;

		[ProtoMember(2)]
		public int roleID;

		[ProtoMember(3)]
		public int serverID;

		[ProtoMember(4)]
		public string userInputID;

		[ProtoMember(5)]
		public int roleLevel;

		[ProtoMember(6)]
		public int vipLevel;

		[ProtoMember(7)]
		public string strMD5 = string.Empty;

		[ProtoMember(8)]
		public int requestType;
	}
}
