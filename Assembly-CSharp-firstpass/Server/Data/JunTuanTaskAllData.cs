using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class JunTuanTaskAllData
	{
		[ProtoMember(1)]
		public int JunTuanId;

		[ProtoMember(8)]
		public DateTime TaskLastTime;

		[ProtoMember(9)]
		public int TaskPoint;

		[ProtoMember(10)]
		public List<JunTuanTaskData> TaskList;
	}
}
