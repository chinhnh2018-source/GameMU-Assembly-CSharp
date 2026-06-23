using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SpriteAttackData : IProtoBuffData
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
					this.roleX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.roleY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.enemy = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.enemyX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 6:
					this.enemyY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 7:
					this.realEnemyX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 8:
					this.realEnemyY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 9:
					this.magicCode = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 10:
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
			num += ProtoUtil.GetIntSize(this.roleX, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.roleY, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.enemy, true, 4, true, 0);
			num += ProtoUtil.GetIntSize(this.enemyX, true, 5, true, 0);
			num += ProtoUtil.GetIntSize(this.enemyY, true, 6, true, 0);
			num += ProtoUtil.GetIntSize(this.realEnemyX, true, 7, true, 0);
			num += ProtoUtil.GetIntSize(this.realEnemyY, true, 8, true, 0);
			num += ProtoUtil.GetIntSize(this.magicCode, true, 9, true, 0);
			num += ProtoUtil.GetLongSize(this.clientTicks, true, 10, true, 0L);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.roleX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.roleY, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.enemy, true, 0);
			ProtoUtil.IntMemberToBytes(array, 5, ref num2, this.enemyX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 6, ref num2, this.enemyY, true, 0);
			ProtoUtil.IntMemberToBytes(array, 7, ref num2, this.realEnemyX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 8, ref num2, this.realEnemyY, true, 0);
			ProtoUtil.IntMemberToBytes(array, 9, ref num2, this.magicCode, true, 0);
			ProtoUtil.LongMemberToBytes(array, 10, ref num2, this.clientTicks, true, 0L);
			return array;
		}

		[ProtoMember(1)]
		public int roleID = 0;

		[ProtoMember(2)]
		public int roleX = 0;

		[ProtoMember(3)]
		public int roleY = 0;

		[ProtoMember(4)]
		public int enemy = 0;

		[ProtoMember(5)]
		public int enemyX = 0;

		[ProtoMember(6)]
		public int enemyY = 0;

		[ProtoMember(7)]
		public int realEnemyX = 0;

		[ProtoMember(8)]
		public int realEnemyY = 0;

		[ProtoMember(9)]
		public int magicCode = 0;

		[ProtoMember(10)]
		public long clientTicks;
	}
}
