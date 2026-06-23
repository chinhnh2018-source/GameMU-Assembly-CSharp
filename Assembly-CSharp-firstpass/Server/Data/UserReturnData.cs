using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class UserReturnData
	{
		[ProtoMember(1)]
		public bool ActivityIsOpen;

		[ProtoMember(2)]
		public int ActivityID;

		[ProtoMember(3)]
		public string ActivityDay = string.Empty;

		[ProtoMember(4)]
		public DateTime TimeBegin = DateTime.MinValue;

		[ProtoMember(5)]
		public DateTime TimeEnd = DateTime.MinValue;

		[ProtoMember(6)]
		public DateTime TimeAward = DateTime.MinValue;

		[ProtoMember(7)]
		public string RecallCode = "0";

		[ProtoMember(8)]
		public int RecallZoneID;

		[ProtoMember(9)]
		public int RecallRoleID;

		[ProtoMember(10)]
		public int Level;

		[ProtoMember(11)]
		public int Vip;

		[ProtoMember(12)]
		public DateTime TimeReturn = DateTime.MinValue;

		[ProtoMember(13)]
		public int ReturnState;

		[ProtoMember(14)]
		public Dictionary<int, int[]> AwardDic = new Dictionary<int, int[]>();

		[ProtoMember(15)]
		public DateTime TimeWait = DateTime.MinValue;

		[ProtoMember(16)]
		public int ZhuanSheng;

		[ProtoMember(17)]
		public int DengJi;

		[ProtoMember(18)]
		public string MyCode = string.Empty;

		[ProtoMember(19)]
		public int LeiJiChongZhi;
	}
}
