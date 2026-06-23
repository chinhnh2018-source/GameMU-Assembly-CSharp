using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class OrnamentData
	{
		[ProtoMember(1)]
		public int ID;

		[ProtoMember(2)]
		public int Param1;

		[ProtoMember(3)]
		public int Param2;
	}
}
