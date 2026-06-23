using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BangHuiMemberData
	{
		[ProtoMember(1)]
		public int ZoneID;

		[ProtoMember(2)]
		public int RoleID;

		[ProtoMember(3)]
		public string RoleName = string.Empty;

		[ProtoMember(4)]
		public int Occupation;

		[ProtoMember(5)]
		public int BHZhiwu;

		[ProtoMember(6)]
		public string ChengHao = string.Empty;

		[ProtoMember(7)]
		public int BangGong;

		[ProtoMember(8)]
		public int Level;

		[ProtoMember(9)]
		public int XueWeiNum;

		[ProtoMember(10)]
		public int SkillLearnedNum;

		[ProtoMember(11)]
		public int OnlineState;

		[ProtoMember(12)]
		public int BangHuiMemberCombatForce;

		[ProtoMember(13)]
		public int BangHuiMemberChangeLifeLev;

		[ProtoMember(14)]
		public int JunTuanZhiWu;

		[ProtoMember(15)]
		public int YaoSaiBossState;

		[ProtoMember(16)]
		public int YaoSaiJianYuState;

		[ProtoMember(17)]
		public int Speech;

		[ProtoMember(18)]
		public long LogOffTime;
	}
}
