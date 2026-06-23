using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	public class LangHunLingYuBangHuiData
	{
		[ProtoMember(1)]
		public int GetDayAwardsState;

		[ProtoMember(2)]
		public List<LangHunLingYuCityData> SelfCityList;

		[ProtoMember(3)]
		public List<LangHunLingYuCityData> OtherCityList;
	}
}
