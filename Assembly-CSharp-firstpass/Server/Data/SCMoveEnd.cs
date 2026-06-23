using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SCMoveEnd : IProtoBuffData
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
					this.Action = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.MapCode = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.ToMapX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.ToMapY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 6:
					this.ToDiection = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 7:
					this.TryRun = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 8:
					this.clientTicks = ProtoUtil.LongMemberFromBytes(data, wt, ref result, ref i);
					break;
				}
			}
			return result;
		}

		public byte[] toBytes()
		{
			int num = 0;
			num += ProtoUtil.GetIntSize(this.RoleID, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.Action, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.MapCode, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.ToMapX, true, 4, true, 0);
			num += ProtoUtil.GetIntSize(this.ToMapY, true, 5, true, 0);
			num += ProtoUtil.GetIntSize(this.ToDiection, true, 6, true, 0);
			num += ProtoUtil.GetIntSize(this.TryRun, true, 7, true, 0);
			num += ProtoUtil.GetLongSize(this.clientTicks, true, 8, true, 0L);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.Action, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.MapCode, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.ToMapX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 5, ref num2, this.ToMapY, true, 0);
			ProtoUtil.IntMemberToBytes(array, 6, ref num2, this.ToDiection, true, 0);
			ProtoUtil.IntMemberToBytes(array, 7, ref num2, this.TryRun, true, 0);
			ProtoUtil.LongMemberToBytes(array, 8, ref num2, this.clientTicks, true, 0L);
			return array;
		}

		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public int Action;

		[ProtoMember(3)]
		public int MapCode;

		[ProtoMember(4)]
		public int ToMapX;

		[ProtoMember(5)]
		public int ToMapY;

		[ProtoMember(6)]
		public int ToDiection;

		[ProtoMember(7)]
		public int TryRun;

		[ProtoMember(8)]
		public long clientTicks;
	}
}
