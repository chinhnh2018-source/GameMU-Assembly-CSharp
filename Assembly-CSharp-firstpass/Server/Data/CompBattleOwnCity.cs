using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class CompBattleOwnCity
	{
		[ProtoMember(1)]
		public List<int> OwnCityList = new List<int>();
	}
}
