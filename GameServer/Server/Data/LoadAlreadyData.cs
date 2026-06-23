using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class LoadAlreadyData : IProtoBuffData
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
					this.RoleID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 2:
					this.MapCode = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.StartMoveTicks = ProtoUtil.LongMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.CurrentX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.CurrentY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 6:
					this.CurrentDirection = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 7:
					this.Action = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 8:
					this.ToX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 9:
					this.ToY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 10:
					this.MoveCost = ProtoUtil.DoubleMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 11:
					this.ExtAction = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 12:
					this.PathString = ProtoUtil.StringMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 13:
					this.CurrentPathIndex = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.MapCode, true, 2, true, 0);
			num += ProtoUtil.GetLongSize(this.StartMoveTicks, true, 3, true, 0L);
			num += ProtoUtil.GetIntSize(this.CurrentX, true, 4, true, 0);
			num += ProtoUtil.GetIntSize(this.CurrentY, true, 5, true, 0);
			num += ProtoUtil.GetIntSize(this.CurrentDirection, true, 6, true, 0);
			num += ProtoUtil.GetIntSize(this.Action, true, 7, true, 0);
			num += ProtoUtil.GetIntSize(this.ToX, true, 8, true, 0);
			num += ProtoUtil.GetIntSize(this.ToY, true, 9, true, 0);
			num += ProtoUtil.GetDoubleSize(this.MoveCost, true, 10, true, 0.0);
			num += ProtoUtil.GetIntSize(this.ExtAction, true, 11, true, 0);
			num += ProtoUtil.GetStringSize(this.PathString, true, 12);
			num += ProtoUtil.GetIntSize(this.CurrentPathIndex, true, 13, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.MapCode, true, 0);
			ProtoUtil.LongMemberToBytes(array, 3, ref num2, this.StartMoveTicks, true, 0L);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.CurrentX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 5, ref num2, this.CurrentY, true, 0);
			ProtoUtil.IntMemberToBytes(array, 6, ref num2, this.CurrentDirection, true, 0);
			ProtoUtil.IntMemberToBytes(array, 7, ref num2, this.Action, true, 0);
			ProtoUtil.IntMemberToBytes(array, 8, ref num2, this.ToX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 9, ref num2, this.ToY, true, 0);
			ProtoUtil.DoubleMemberToBytes(array, 10, ref num2, this.MoveCost, true, 0.0);
			ProtoUtil.IntMemberToBytes(array, 11, ref num2, this.ExtAction, true, 0);
			ProtoUtil.StringMemberToBytes(array, 12, ref num2, this.PathString);
			ProtoUtil.IntMemberToBytes(array, 13, ref num2, this.CurrentPathIndex, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int RoleID = 0;

		[ProtoMember(2)]
		public int MapCode = 0;

		[ProtoMember(3)]
		public long StartMoveTicks = 0L;

		[ProtoMember(4)]
		public int CurrentX = 0;

		[ProtoMember(5)]
		public int CurrentY = 0;

		[ProtoMember(6)]
		public int CurrentDirection = 0;

		[ProtoMember(7)]
		public int Action = 0;

		[ProtoMember(8)]
		public int ToX = 0;

		[ProtoMember(9)]
		public int ToY = 0;

		[ProtoMember(10)]
		public double MoveCost = 1.0;

		[ProtoMember(11)]
		public int ExtAction = 0;

		[ProtoMember(12)]
		public string PathString = "";

		[ProtoMember(13)]
		public int CurrentPathIndex = 0;
	}
}
