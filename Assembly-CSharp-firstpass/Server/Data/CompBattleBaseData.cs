using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class CompBattleBaseData
	{
		[ProtoMember(1)]
		public List<CompBattleOwnCity> CompBattleOwnCityList = new List<CompBattleOwnCity>();
	}
}
