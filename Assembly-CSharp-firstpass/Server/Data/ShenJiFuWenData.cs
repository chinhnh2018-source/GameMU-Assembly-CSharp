using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ShenJiFuWenData
	{
		[ProtoMember(1, IsRequired = true)]
		public int ID;

		[ProtoMember(2, IsRequired = true)]
		public int Level;
	}
}
