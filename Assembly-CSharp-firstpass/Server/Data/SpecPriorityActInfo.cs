using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class SpecPriorityActInfo
	{
		[ProtoMember(1)]
		public int TeQuanID;

		[ProtoMember(2)]
		public int ActID;

		[ProtoMember(3)]
		public int LeftPurNum;

		[ProtoMember(4)]
		public int State;

		[ProtoMember(5)]
		public int ShowNum;

		[ProtoMember(6)]
		public int ShowNum2;
	}
}
