using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SpriteActionData : IProtoBuffData
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
					this.mapCode = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.direction = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.action = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.toX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 6:
					this.toY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 7:
					this.targetX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 8:
					this.targetY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 9:
					this.yAngle = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 10:
					this.moveToX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 11:
					this.moveToY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 12:
					this.clientTicks = ProtoUtil.LongMemberFromBytes(data, wt, ref result, ref i);
					break;
				}
			}
			return result;
		}

		public byte[] toBytes()
		{
			int num = 0;
			num += ProtoUtil.GetIntSize(this.roleID, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.mapCode, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.direction, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.action, true, 4, true, 0);
			num += ProtoUtil.GetIntSize(this.toX, true, 5, true, 0);
			num += ProtoUtil.GetIntSize(this.toY, true, 6, true, 0);
			num += ProtoUtil.GetIntSize(this.targetX, true, 7, true, 0);
			num += ProtoUtil.GetIntSize(this.targetY, true, 8, true, 0);
			num += ProtoUtil.GetIntSize(this.yAngle, true, 9, true, 0);
			num += ProtoUtil.GetIntSize(this.moveToX, true, 10, true, 0);
			num += ProtoUtil.GetIntSize(this.moveToY, true, 11, true, 0);
			num += ProtoUtil.GetLongSize(this.clientTicks, true, 12, true, 0L);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.mapCode, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.direction, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.action, true, 0);
			ProtoUtil.IntMemberToBytes(array, 5, ref num2, this.toX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 6, ref num2, this.toY, true, 0);
			ProtoUtil.IntMemberToBytes(array, 7, ref num2, this.targetX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 8, ref num2, this.targetY, true, 0);
			ProtoUtil.IntMemberToBytes(array, 9, ref num2, this.yAngle, true, 0);
			ProtoUtil.IntMemberToBytes(array, 10, ref num2, this.moveToX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 11, ref num2, this.moveToY, true, 0);
			ProtoUtil.LongMemberToBytes(array, 12, ref num2, this.clientTicks, true, 0L);
			return array;
		}

		[ProtoMember(1)]
		public int roleID = 0;

		[ProtoMember(2)]
		public int mapCode = 0;

		[ProtoMember(3)]
		public int direction = 0;

		[ProtoMember(4)]
		public int action = 0;

		[ProtoMember(5)]
		public int toX = 0;

		[ProtoMember(6)]
		public int toY = 0;

		[ProtoMember(7)]
		public int targetX = 0;

		[ProtoMember(8)]
		public int targetY = 0;

		[ProtoMember(9)]
		public int yAngle = 0;

		[ProtoMember(10)]
		public int moveToX = 0;

		[ProtoMember(11)]
		public int moveToY = 0;

		[ProtoMember(12)]
		public long clientTicks;
	}
}
