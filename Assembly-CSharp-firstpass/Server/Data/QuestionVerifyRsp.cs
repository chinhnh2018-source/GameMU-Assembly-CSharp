using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class QuestionVerifyRsp
	{
		[ProtoMember(1)]
		public long Id;

		[ProtoMember(2)]
		public uint Answer;
	}
}
