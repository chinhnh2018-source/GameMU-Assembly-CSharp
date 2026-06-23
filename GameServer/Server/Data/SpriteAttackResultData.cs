using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SpriteAttackResultData : IProtoBuffData
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
					this.enemy = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 2:
					this.burst = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.injure = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.enemyLife = ProtoUtil.DoubleMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.newExperience = ProtoUtil.LongMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 6:
					this.currentExperience = ProtoUtil.LongMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 7:
					this.newLevel = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 8:
					this.MerlinInjuer = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 9:
					this.MerlinType = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 10:
					this.armorV_p1 = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.burst, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.injure, true, 3, true, 0);
			num += ProtoUtil.GetDoubleSize(this.enemyLife, true, 4, true, 0.0);
			num += ProtoUtil.GetLongSize(this.newExperience, true, 5, true, 0L);
			num += ProtoUtil.GetLongSize(this.currentExperience, true, 6, true, 0L);
			num += ProtoUtil.GetIntSize(this.newLevel, true, 7, true, 0);
			num += ProtoUtil.GetIntSize(this.MerlinInjuer, true, 8, true, 0);
			num += ProtoUtil.GetIntSize(this.MerlinType, true, 9, true, 0);
			num += ProtoUtil.GetIntSize(this.armorV_p1, true, 10, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.enemy, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.burst, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.injure, true, 0);
			ProtoUtil.DoubleMemberToBytes(array, 4, ref num2, this.enemyLife, true, 0.0);
			ProtoUtil.LongMemberToBytes(array, 5, ref num2, this.newExperience, true, 0L);
			ProtoUtil.LongMemberToBytes(array, 6, ref num2, this.currentExperience, true, 0L);
			ProtoUtil.IntMemberToBytes(array, 7, ref num2, this.newLevel, true, 0);
			ProtoUtil.IntMemberToBytes(array, 8, ref num2, this.MerlinInjuer, true, 0);
			ProtoUtil.IntMemberToBytes(array, 9, ref num2, this.MerlinType, true, 0);
			ProtoUtil.IntMemberToBytes(array, 10, ref num2, this.armorV_p1, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int enemy = 0;

		[ProtoMember(2)]
		public int burst = 0;

		[ProtoMember(3)]
		public int injure = 0;

		[ProtoMember(4)]
		public double enemyLife = 0.0;

		[ProtoMember(5)]
		public long newExperience = 0L;

		[ProtoMember(6)]
		public long currentExperience = 0L;

		[ProtoMember(7)]
		public int newLevel = 0;

		[ProtoMember(8)]
		public int MerlinInjuer = 0;

		[ProtoMember(9)]
		public int MerlinType = 0;

		[ProtoMember(10)]
		public int armorV_p1;
	}
}
