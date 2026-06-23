using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class TianTi5v5PiPeiState
	{
		[ProtoMember(1)]
		public List<TianTi5v5PiPeiRoleState> RoleList = new List<TianTi5v5PiPeiRoleState>();

		[ProtoMember(2)]
		public long EndTicks;

		public int State;
	}
}
