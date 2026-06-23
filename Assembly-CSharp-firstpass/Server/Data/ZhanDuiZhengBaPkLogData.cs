using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZhanDuiZhengBaPkLogData
	{
		[ProtoMember(1)]
		public int Day;

		[ProtoMember(2)]
		public int ZhanDuiID1;

		[ProtoMember(3)]
		public int ZoneID1;

		[ProtoMember(4)]
		public string ZhanDuiName1;

		[ProtoMember(5)]
		public int ZhanDuiID2;

		[ProtoMember(6)]
		public int ZoneID2;

		[ProtoMember(7)]
		public string ZhanDuiName2;

		[ProtoMember(8)]
		public int PkResult;

		[ProtoMember(9)]
		public bool UpGrade;

		[ProtoMember(10)]
		public int Month;

		[ProtoMember(13)]
		public DateTime StartTime;

		[ProtoMember(14)]
		public DateTime EndTime;
	}
}
