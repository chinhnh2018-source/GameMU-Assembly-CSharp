using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BangHuiMatchGuessInfo
	{
		[ProtoMember(1)]
		public int round;

		[ProtoMember(2)]
		public int right;

		[ProtoMember(3)]
		public int jifen;
	}
}
