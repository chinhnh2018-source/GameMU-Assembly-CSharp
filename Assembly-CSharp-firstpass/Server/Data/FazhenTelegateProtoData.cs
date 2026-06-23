using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class FazhenTelegateProtoData
	{
		[ProtoMember(1)]
		public int gateId;

		[ProtoMember(2)]
		public int DestMapCode;
	}
}
