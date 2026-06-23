using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SCClientHeart : IProtoBuffData
	{
		public SCClientHeart()
		{
		}

		public SCClientHeart(int roleID, int token, int allowTicks)
		{
			this.RoleID = roleID;
			this.RandToken = token;
			this.Ticks = allowTicks;
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
					this.RandToken = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.Ticks = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.ReportCliRealTick = ProtoUtil.LongMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.RandToken, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.Ticks, true, 3, true, 0);
			num += ProtoUtil.GetLongSize(this.ReportCliRealTick, true, 4, true, 0L);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.RandToken, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.Ticks, true, 0);
			ProtoUtil.LongMemberToBytes(array, 4, ref num2, this.ReportCliRealTick, true, 0L);
			return array;
		}

		[ProtoMember(1)]
		public int RoleID = 0;

		[ProtoMember(2)]
		public int RandToken = 0;

		[ProtoMember(3)]
		public int Ticks = 0;

		[ProtoMember(4)]
		public long ReportCliRealTick;
	}
}
