using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class ZhengBaMiniStateData
	{
		[ProtoMember(1)]
		public long PkStartWaitSec;

		[ProtoMember(2)]
		public long NextLoopWaitSec;

		[ProtoMember(3)]
		public long LoopEndWaitSec;

		[ProtoMember(4, IsRequired = true)]
		public bool IsZhengBaOpened;

		[ProtoMember(5, IsRequired = true)]
		public bool IsThisMonthInActivity;
	}
}
