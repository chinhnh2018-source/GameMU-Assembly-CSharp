using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class TaskData
	{
		[ProtoMember(1)]
		public int DbID;

		[ProtoMember(2)]
		public int DoingTaskID;

		[ProtoMember(3)]
		public int DoingTaskVal1;

		[ProtoMember(4)]
		public int DoingTaskVal2;

		[ProtoMember(5)]
		public int DoingTaskFocus;

		[ProtoMember(6)]
		public long AddDateTime;

		[ProtoMember(7)]
		public TaskAwardsData TaskAwards;

		[ProtoMember(8)]
		public int DoneCount;

		[ProtoMember(9)]
		public int StarLevel;

		[ProtoMember(10)]
		public int RefreshCount;

		[ProtoMember(11)]
		public long ChengJiuVal;

		public int TaskClass;

		public bool IsComplete;

		public int RoadOtherTaskId = -1;

		public bool IsLevelLimited;

		public int zhiXianNpcID = -1;

		public int zhiXianMapCode = -1;

		public int zhiXianToPosX = -1;

		public int zhiXianToPosY = -1;

		public int zhiXianLinkID = -1;

		public int zhiXianTargetType = -1;

		public bool specialRiChangTask;

		public int SpecialID = -1;
	}
}
