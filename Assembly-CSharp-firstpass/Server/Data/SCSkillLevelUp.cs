using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SCSkillLevelUp : IProtoBuffData
	{
		public SCSkillLevelUp()
		{
		}

		public SCSkillLevelUp(int state, int roleID, int skillID, int skillLevel, int SkillUsedNum)
		{
			this.State = state;
			this.RoleID = roleID;
			this.SkillID = skillID;
			this.SkillLevel = skillLevel;
			this.SkillUsedNum = SkillUsedNum;
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
					this.State = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 2:
					this.RoleID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.SkillID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.SkillLevel = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.SkillUsedNum = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.State, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.RoleID, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.SkillID, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.SkillLevel, true, 4, true, 0);
			num += ProtoUtil.GetIntSize(this.SkillUsedNum, true, 5, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.State, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.SkillID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.SkillLevel, true, 0);
			ProtoUtil.IntMemberToBytes(array, 5, ref num2, this.SkillUsedNum, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int State;

		[ProtoMember(2)]
		public int RoleID;

		[ProtoMember(3)]
		public int SkillID;

		[ProtoMember(4)]
		public int SkillLevel;

		[ProtoMember(5)]
		public int SkillUsedNum;
	}
}
