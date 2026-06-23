using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class SpecActInfo
	{
		[ProtoMember(1)]
		public int ActID;

		[ProtoMember(2)]
		public int LeftPurNum;

		[ProtoMember(3)]
		public int State;

		[ProtoMember(4)]
		public int ShowNum1;

		[ProtoMember(5)]
		public int ShowNum2;
	}
}
