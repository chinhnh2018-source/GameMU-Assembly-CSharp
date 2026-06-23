using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class QuestionVerifyReq
	{
		[ProtoMember(1)]
		public long Id;

		[ProtoMember(2)]
		public string Question;

		[ProtoMember(3)]
		public List<int> ArgumentTypes;
	}
}
