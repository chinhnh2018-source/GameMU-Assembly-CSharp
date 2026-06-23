using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class EveryDayActInfo
	{
		[ProtoMember(1)]
		public int ActID;

		[ProtoMember(2)]
		public int LeftPurNum = -1;

		[ProtoMember(3)]
		public int State;

		[ProtoMember(4)]
		public int ShowNum;
	}
}
