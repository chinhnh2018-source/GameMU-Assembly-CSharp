using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class EraTaskData
	{
		[ProtoMember(1)]
		public int TaskID;

		[ProtoMember(2)]
		public int TaskVal1;

		[ProtoMember(3)]
		public int TaskVal2;

		[ProtoMember(4)]
		public int TaskVal3;
	}
}
