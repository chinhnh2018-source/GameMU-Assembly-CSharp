using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BangHuiDetailData
	{
		[ProtoMember(1)]
		public int BHID;

		[ProtoMember(2)]
		public string BHName = string.Empty;

		[ProtoMember(3)]
		public int ZoneID;

		[ProtoMember(4)]
		public int BZRoleID;

		[ProtoMember(5)]
		public string BZRoleName = string.Empty;

		[ProtoMember(6)]
		public int BZOccupation;

		[ProtoMember(7)]
		public int TotalNum;

		[ProtoMember(8)]
		public int TotalLevel;

		[ProtoMember(9)]
		public string BHBulletin = string.Empty;

		[ProtoMember(10)]
		public string BuildTime = string.Empty;

		[ProtoMember(11)]
		public string QiName = string.Empty;

		[ProtoMember(12)]
		public int QiLevel;

		[ProtoMember(13)]
		public List<BangHuiMgrItemData> MgrItemList;

		[ProtoMember(14)]
		public int IsVerify;

		[ProtoMember(15)]
		public int TotalMoney;

		[ProtoMember(16)]
		public int TodayZhanGongForGold;

		[ProtoMember(17)]
		public int TodayZhanGongForDiamond;

		[ProtoMember(18)]
		public int JiTan;

		[ProtoMember(19)]
		public int JunXie;

		[ProtoMember(20)]
		public int GuangHuan;

		[ProtoMember(21)]
		public int CanModNameTimes;
	}
}
