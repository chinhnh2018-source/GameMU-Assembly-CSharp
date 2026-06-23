using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SCCompTask : IProtoBuffData
	{
		public SCCompTask()
		{
		}

		public SCCompTask(int roleID, int npcID, int taskID, int state)
		{
			this.roleID = roleID;
			this.npcID = npcID;
			this.taskID = taskID;
			this.state = state;
		}

		public int fromBytes(byte[] data, int offset, int count)
		{
			int result = offset;
			int i = 0;
			while (i < count)
			{
				int num = -1;
				int wt = -1;
				ProtoUtil.GetTag(data, ref result, ref num, ref wt, ref i);
				switch (num)
				{
				case 1:
					this.roleID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 2:
					this.npcID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.taskID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.state = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return result;
		}

		public byte[] toBytes()
		{
			int num = 0;
			num += ProtoUtil.GetIntSize(this.roleID, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.npcID, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.taskID, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.state, true, 4, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.npcID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.taskID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.state, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int roleID = 0;

		[ProtoMember(2)]
		public int npcID = 0;

		[ProtoMember(3)]
		public int taskID = 0;

		[ProtoMember(4)]
		public int state = 0;
	}
}
