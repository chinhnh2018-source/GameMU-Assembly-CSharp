using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SpriteRelifeData : IProtoBuffData
	{
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
					this.x = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.y = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.direction = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.lifeV = ProtoUtil.DoubleMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 6:
					this.magicV = ProtoUtil.DoubleMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 7:
					this.force = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.x, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.y, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.direction, true, 4, true, 0);
			num += ProtoUtil.GetDoubleSize(this.lifeV, true, 5, true, 0.0);
			num += ProtoUtil.GetDoubleSize(this.magicV, true, 6, true, 0.0);
			num += ProtoUtil.GetIntSize(this.force, true, 7, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.x, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.y, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.direction, true, 0);
			ProtoUtil.DoubleMemberToBytes(array, 5, ref num2, this.lifeV, true, 0.0);
			ProtoUtil.DoubleMemberToBytes(array, 6, ref num2, this.magicV, true, 0.0);
			ProtoUtil.IntMemberToBytes(array, 7, ref num2, this.force, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int roleID;

		[ProtoMember(2)]
		public int x;

		[ProtoMember(3)]
		public int y;

		[ProtoMember(4)]
		public int direction;

		[ProtoMember(5)]
		public double lifeV;

		[ProtoMember(6)]
		public double magicV;

		[ProtoMember(7)]
		public int force;
	}
}
