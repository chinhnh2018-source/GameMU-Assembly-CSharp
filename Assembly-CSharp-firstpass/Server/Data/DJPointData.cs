using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class DJPointData
	{
		[ProtoMember(1)]
		public int DbID;

		[ProtoMember(2)]
		public int RoleID;

		[ProtoMember(3)]
		public int DJPoint;

		[ProtoMember(4)]
		public int Total;

		[ProtoMember(5)]
		public int Wincnt;

		[ProtoMember(6)]
		public int Yestoday;

		[ProtoMember(7)]
		public int Lastweek;

		[ProtoMember(8)]
		public int Lastmonth;

		[ProtoMember(9)]
		public int Dayupdown;

		[ProtoMember(10)]
		public int Weekupdown;

		[ProtoMember(11)]
		public int Monthupdown;

		[ProtoMember(12)]
		public DJRoleInfoData djRoleInfoData;
	}
}
