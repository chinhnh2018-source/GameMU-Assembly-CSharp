using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SpriteHitedData : IProtoBuffData
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
					this.roleId = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 2:
					this.enemy = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.enemyX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.enemyY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.magicCode = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.roleId, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.enemy, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.enemyX, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.enemyY, true, 4, true, 0);
			num += ProtoUtil.GetIntSize(this.magicCode, true, 5, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.roleId, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.enemy, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.enemyX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.enemyY, true, 0);
			ProtoUtil.IntMemberToBytes(array, 5, ref num2, this.magicCode, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int roleId;

		[ProtoMember(2)]
		public int enemy;

		[ProtoMember(3)]
		public int enemyX;

		[ProtoMember(4)]
		public int enemyY;

		[ProtoMember(5)]
		public int magicCode;
	}
}
