using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ShiLianTaAwardsInfoData
	{
		[ProtoMember(1)]
		public int CurrentFloorTotalMonsterNum;

		[ProtoMember(2)]
		public int CurrentFloorExperienceAward;

		[ProtoMember(3)]
		public int NextFloorNeedGoodsID;

		[ProtoMember(4)]
		public int NextFloorNeedGoodsNum;

		[ProtoMember(5)]
		public int NextFloorExperienceAward;
	}
}
