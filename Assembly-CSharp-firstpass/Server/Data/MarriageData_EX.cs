using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class MarriageData_EX
	{
		[ProtoMember(1)]
		public MarriageData myMarriageData;

		[ProtoMember(2)]
		public string roleName;

		[ProtoMember(3)]
		public int occupationId;
	}
}
