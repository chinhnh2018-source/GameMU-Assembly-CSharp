using System;
using ProtoBuf;

namespace GameServer.Logic
{
	[ProtoContract]
	public class TaskCheckList
	{
		[ProtoMember(1)]
		public byte[] TaskCheckListData;
	}
}
