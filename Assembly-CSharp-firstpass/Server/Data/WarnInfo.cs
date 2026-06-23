using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class WarnInfo
	{
		[ProtoMember(1, IsRequired = true)]
		public int ID;

		[ProtoMember(2, IsRequired = true)]
		public string Desc = string.Empty;

		[ProtoMember(3, IsRequired = true)]
		public int TimeSec;

		[ProtoMember(4, IsRequired = true)]
		public int Operate;
	}
}
