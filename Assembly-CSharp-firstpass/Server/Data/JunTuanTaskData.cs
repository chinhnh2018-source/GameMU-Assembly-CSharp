using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class JunTuanTaskData
	{
		public JunTuanTaskData Clone()
		{
			return base.MemberwiseClone() as JunTuanTaskData;
		}

		[ProtoMember(1)]
		public int TaskId;

		[ProtoMember(2)]
		public int TaskValue;

		[ProtoMember(3)]
		public long TaskState;

		[ProtoMember(5)]
		public int HasGet;

		[NonSerialized]
		public int WeekDay;
	}
}
