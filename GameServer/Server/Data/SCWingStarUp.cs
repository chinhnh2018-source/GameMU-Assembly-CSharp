using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SCWingStarUp : IProtoBuffData
	{
		public SCWingStarUp()
		{
		}

		public SCWingStarUp(int state, int roleID, int nNextStarLevel, int nNextStarExp)
		{
			this.RoleID = roleID;
			this.NextStarLevel = nNextStarLevel;
			this.NextStarExp = nNextStarExp;
			this.State = state;
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
					this.RoleID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 2:
					this.NextStarLevel = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.NextStarExp = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.State = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.RoleID, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.NextStarLevel, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.NextStarExp, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.State, true, 4, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.NextStarLevel, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.NextStarExp, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.State, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int RoleID = 0;

		[ProtoMember(2)]
		public int NextStarLevel = 0;

		[ProtoMember(3)]
		public int NextStarExp = 0;

		[ProtoMember(4)]
		public int State = 0;
	}
}
