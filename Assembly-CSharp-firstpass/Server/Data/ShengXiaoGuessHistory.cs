using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ShengXiaoGuessHistory
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string RoleName = string.Empty;

		[ProtoMember(3)]
		public int GuessKey;

		[ProtoMember(4)]
		public int Mortgage;

		[ProtoMember(5)]
		public int ResultKey;

		[ProtoMember(6)]
		public int GainNum;

		[ProtoMember(7)]
		public int LeftMortgage;

		[ProtoMember(8)]
		public string GuessTime = string.Empty;
	}
}
