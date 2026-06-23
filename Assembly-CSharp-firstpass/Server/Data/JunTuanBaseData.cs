using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class JunTuanBaseData
	{
		[ProtoMember(1)]
		public int JunTuanId;

		[ProtoMember(2)]
		public string JunTuanName;

		[ProtoMember(3)]
		public int LingDi;

		[ProtoMember(8)]
		public List<int> BhList;
	}
}
