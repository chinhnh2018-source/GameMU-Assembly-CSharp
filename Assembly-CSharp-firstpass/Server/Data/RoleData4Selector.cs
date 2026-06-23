using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class RoleData4Selector
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string RoleName = string.Empty;

		[ProtoMember(3)]
		public int RoleSex;

		[ProtoMember(4)]
		public int Occupation;

		[ProtoMember(5)]
		public int Level = 1;

		[ProtoMember(6)]
		public int Faction;

		[ProtoMember(7)]
		public string OtherName = string.Empty;

		[ProtoMember(8)]
		public List<GoodsData> GoodsDataList;

		[ProtoMember(9)]
		public WingData MyWingData;

		[ProtoMember(10)]
		public int CombatForce;

		[ProtoMember(11)]
		public int AdmiredCount;

		[ProtoMember(12)]
		public int FashionWingsID;

		[ProtoMember(13)]
		public long SettingBitFlags;

		[ProtoMember(14)]
		public int ZoneId;

		[ProtoMember(15)]
		public List<int> OccupationList;

		[ProtoMember(21)]
		public RoleHuiJiData HuiJiData;

		[ProtoMember(22)]
		public int CompType;

		[ProtoMember(23)]
		public byte CompZhiWu;

		[ProtoMember(24)]
		public int SubOccupation;
	}
}
