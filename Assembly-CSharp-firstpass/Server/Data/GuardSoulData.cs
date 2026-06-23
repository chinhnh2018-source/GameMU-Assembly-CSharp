using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class GuardSoulData
	{
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"type, ",
				this.type,
				", equipSlot, ",
				this.equipSlot
			});
		}

		[ProtoMember(1, IsRequired = true)]
		public int type;

		[ProtoMember(2, IsRequired = true)]
		public int equipSlot = -1;
	}
}
