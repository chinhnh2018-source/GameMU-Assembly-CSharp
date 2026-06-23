using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class TaskAwardsData
	{
		[ProtoMember(1)]
		public List<AwardsItemData> TaskawardList;

		[ProtoMember(2)]
		public List<AwardsItemData> OtherTaskawardList;

		[ProtoMember(3)]
		public int Moneyaward;

		[ProtoMember(4)]
		public long Experienceaward;

		[ProtoMember(5)]
		public int YinLiangaward;

		[ProtoMember(6)]
		public int LingLiaward;

		[ProtoMember(7)]
		public int BindYuanBaoaward;

		[ProtoMember(8)]
		public int ZhenQiaward;

		[ProtoMember(9)]
		public int LieShaaward;

		[ProtoMember(10)]
		public int WuXingaward;

		[ProtoMember(11)]
		public int NeedYuanBao;

		[ProtoMember(12)]
		public int JunGongaward;

		[ProtoMember(13)]
		public int RongYuaward;

		[ProtoMember(14)]
		public int AddExperienceForDailyCircleTask;

		[ProtoMember(15)]
		public int AddMoJingForDailyCircleTask;

		[ProtoMember(16)]
		public string AddGoodsForDailyCircleTask = string.Empty;

		[ProtoMember(17)]
		public int MoJingaward;

		[ProtoMember(18)]
		public int XingHunaward;

		[ProtoMember(19)]
		public int FenMoAward;

		[ProtoMember(20)]
		public int ShengwangAward;
	}
}
