using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KarenScoreData
	{
		[ProtoMember(1)]
		public int LegionID;

		[ProtoMember(2)]
		public string Name;

		[ProtoMember(3)]
		public int Score;

		[ProtoMember(4)]
		public List<int> ResourceList;

		[ProtoMember(5)]
		public long ticks;
	}
}
