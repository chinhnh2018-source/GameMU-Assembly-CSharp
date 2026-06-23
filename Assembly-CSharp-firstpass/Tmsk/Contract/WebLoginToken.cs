using System;
using ProtoBuf;

namespace Tmsk.Contract
{
	[ProtoContract]
	public class WebLoginToken
	{
		[ProtoMember(1)]
		public int VerSign;

		[ProtoMember(2)]
		public string UserID;

		[ProtoMember(3)]
		public string UserName;

		[ProtoMember(4)]
		public string LastTime;

		[ProtoMember(5)]
		public string Isadult;

		[ProtoMember(6)]
		public string SignCode;
	}
}
