using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SpritePositionData : IProtoBuffData
	{
		public SpritePositionData()
		{
		}

		public SpritePositionData(int roleId, int mapCode, int toX, int toY, long currentPosTicks)
		{
			this.roleID = roleId;
			this.mapCode = mapCode;
			this.toX = toX;
			this.toY = toY;
			this.currentPosTicks = currentPosTicks;
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
					this.mapCode = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.toX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.toY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.currentPosTicks = ProtoUtil.LongMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.toX, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.toY, true, 4, true, 0);
			num += ProtoUtil.GetLongSize(this.currentPosTicks, true, 5, true, 0L);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.mapCode, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.toX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.toY, true, 0);
			ProtoUtil.LongMemberToBytes(array, 5, ref num2, this.currentPosTicks, true, 0L);
			return array;
		}

		[ProtoMember(1)]
		public int roleID;

		[ProtoMember(2)]
		public int mapCode;

		[ProtoMember(3)]
		public int toX;

		[ProtoMember(4)]
		public int toY;

		[ProtoMember(5)]
		public long currentPosTicks;
	}
}
