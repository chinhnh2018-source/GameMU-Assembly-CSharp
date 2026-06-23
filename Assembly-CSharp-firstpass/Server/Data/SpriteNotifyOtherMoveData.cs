using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SpriteNotifyOtherMoveData : IProtoBuffData
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
					this.action = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.toX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.toY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 6:
					this.extAction = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 7:
					this.fromX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 8:
					this.fromY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 9:
					this.startMoveTicks = ProtoUtil.LongMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 10:
					this.pathString = ProtoUtil.StringMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 11:
					this.moveCost = ProtoUtil.DoubleMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.mapCode, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.action, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.toX, true, 4, true, 0);
			num += ProtoUtil.GetIntSize(this.toY, true, 5, true, 0);
			num += ProtoUtil.GetIntSize(this.extAction, true, 6, true, 0);
			num += ProtoUtil.GetIntSize(this.fromX, true, 7, true, 0);
			num += ProtoUtil.GetIntSize(this.fromY, true, 8, true, 0);
			num += ProtoUtil.GetLongSize(this.startMoveTicks, true, 9, true, 0L);
			num += ProtoUtil.GetStringSize(this.pathString, true, 10);
			num += ProtoUtil.GetDoubleSize(this.moveCost, true, 11, true, 0.0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.mapCode, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.action, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.toX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 5, ref num2, this.toY, true, 0);
			ProtoUtil.IntMemberToBytes(array, 6, ref num2, this.extAction, true, 0);
			ProtoUtil.IntMemberToBytes(array, 7, ref num2, this.fromX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 8, ref num2, this.fromY, true, 0);
			ProtoUtil.LongMemberToBytes(array, 9, ref num2, this.startMoveTicks, true, 0L);
			ProtoUtil.StringMemberToBytes(array, 10, ref num2, this.pathString);
			ProtoUtil.DoubleMemberToBytes(array, 11, ref num2, this.moveCost, true, 0.0);
			return array;
		}

		[ProtoMember(1)]
		public int roleID;

		[ProtoMember(2)]
		public int mapCode;

		[ProtoMember(3)]
		public int action;

		[ProtoMember(4)]
		public int toX;

		[ProtoMember(5)]
		public int toY;

		[ProtoMember(6)]
		public int extAction;

		[ProtoMember(7)]
		public int fromX;

		[ProtoMember(8)]
		public int fromY;

		[ProtoMember(9)]
		public long startMoveTicks;

		[ProtoMember(10)]
		public string pathString = string.Empty;

		[ProtoMember(11)]
		public double moveCost;
	}
}
