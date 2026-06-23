using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class PeriodsHistoryData
	{
		[ProtoMember(1)]
		public long DataPeriods;

		[ProtoMember(2)]
		public List<WinLotteryRoleData> WinLotteryRoleList;
	}
}
