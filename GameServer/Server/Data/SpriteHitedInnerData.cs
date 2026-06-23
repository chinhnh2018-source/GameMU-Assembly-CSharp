using System;
using Tmsk.Contract;

namespace Server.Data
{
	public class SpriteHitedInnerData : IProtoBuffDataEx, IProtoBuffData
	{
		public SpriteHitedInnerData()
		{
		}

		public SpriteHitedInnerData(int enemy, int enemyX, int enemyY)
		{
			this.enemy = enemy;
			this.enemyX = enemyX;
			this.enemyY = enemyY;
		}

		public int getBytesSize()
		{
			int num = 0;
			num += ProtoUtil.GetIntSize(this.enemy, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.enemyX, true, 2, true, 0);
			return num + ProtoUtil.GetIntSize(this.enemyY, true, 3, true, 0);
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
					this.enemy = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 2:
					this.enemyX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.enemyY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.enemy, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.enemyX, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.enemyY, true, 3, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.enemy, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.enemyX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.enemyY, true, 0);
			return array;
		}

		public int enemy;

		public int enemyX;

		public int enemyY;
	}
}
