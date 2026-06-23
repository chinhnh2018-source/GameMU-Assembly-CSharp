using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class EveryDayActivityData
	{
		[ProtoMember(1)]
		public List<EveryDayActInfo> EveryDayActInfoList;
	}
}
