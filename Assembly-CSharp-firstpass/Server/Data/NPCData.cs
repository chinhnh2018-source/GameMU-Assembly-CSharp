using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class NPCData
	{
		[ProtoMember(1)]
		public int MapCode;

		[ProtoMember(2)]
		public int RoleID;

		[ProtoMember(3)]
		public int NPCID;

		[ProtoMember(4)]
		public List<int> NewTaskIDs;

		[ProtoMember(5)]
		public List<int> OperationIDs;

		[ProtoMember(6)]
		public List<int> ScriptIDs;

		[ProtoMember(7)]
		public int ExtensionID;

		[ProtoMember(8)]
		public List<int> NewTaskIDsDoneCount;

		[ProtoMember(9)]
		public int GatherTime;
	}
}
