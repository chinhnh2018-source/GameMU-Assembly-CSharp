using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SpriteInjuredData : IProtoBuffData
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
					this.attackerRoleID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 2:
					this.injuredRoleID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.burst = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.injure = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.injuredRoleLife = ProtoUtil.LongMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 6:
					this.attackerLevel = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 7:
					this.injuredRoleMaxLifeV = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 8:
					this.injuredRoleMagic = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 9:
					this.injuredRoleMaxMagicV = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 10:
					this.hitToGridX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 11:
					this.hitToGridY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 12:
					this.MerlinInjuer = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 13:
					this.MerlinType = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 14:
					this.stopCaiJi = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 15:
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
			num += ProtoUtil.GetIntSize(this.attackerRoleID, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.injuredRoleID, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.burst, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.injure, true, 4, true, 0);
			num += ProtoUtil.GetLongSize(this.injuredRoleLife, true, 5, true, 0L);
			num += ProtoUtil.GetIntSize(this.attackerLevel, true, 6, true, 0);
			num += ProtoUtil.GetIntSize(this.injuredRoleMaxLifeV, true, 7, true, 0);
			num += ProtoUtil.GetIntSize(this.injuredRoleMagic, true, 8, true, 0);
			num += ProtoUtil.GetIntSize(this.injuredRoleMaxMagicV, true, 9, true, 0);
			num += ProtoUtil.GetIntSize(this.hitToGridX, true, 10, true, 0);
			num += ProtoUtil.GetIntSize(this.hitToGridY, true, 11, true, 0);
			num += ProtoUtil.GetIntSize(this.MerlinInjuer, true, 12, true, 0);
			num += ProtoUtil.GetIntSize(this.MerlinType, true, 13, true, 0);
			num += ProtoUtil.GetIntSize(this.stopCaiJi, true, 14, true, 0);
			num += ProtoUtil.GetIntSize(this.armorV_p1, true, 15, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.attackerRoleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.injuredRoleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.burst, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.injure, true, 0);
			ProtoUtil.LongMemberToBytes(array, 5, ref num2, this.injuredRoleLife, true, 0L);
			ProtoUtil.IntMemberToBytes(array, 6, ref num2, this.attackerLevel, true, 0);
			ProtoUtil.IntMemberToBytes(array, 7, ref num2, this.injuredRoleMaxLifeV, true, 0);
			ProtoUtil.IntMemberToBytes(array, 8, ref num2, this.injuredRoleMagic, true, 0);
			ProtoUtil.IntMemberToBytes(array, 9, ref num2, this.injuredRoleMaxMagicV, true, 0);
			ProtoUtil.IntMemberToBytes(array, 10, ref num2, this.hitToGridX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 11, ref num2, this.hitToGridY, true, 0);
			ProtoUtil.IntMemberToBytes(array, 12, ref num2, this.MerlinInjuer, true, 0);
			ProtoUtil.IntMemberToBytes(array, 13, ref num2, this.MerlinType, true, 0);
			ProtoUtil.IntMemberToBytes(array, 14, ref num2, this.stopCaiJi, true, 0);
			ProtoUtil.IntMemberToBytes(array, 15, ref num2, this.armorV_p1, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int attackerRoleID;

		[ProtoMember(2)]
		public int injuredRoleID;

		[ProtoMember(3)]
		public int burst;

		[ProtoMember(4)]
		public int injure;

		[ProtoMember(5)]
		public long injuredRoleLife;

		[ProtoMember(6)]
		public int attackerLevel;

		[ProtoMember(7)]
		public int injuredRoleMaxLifeV;

		[ProtoMember(8)]
		public int injuredRoleMagic;

		[ProtoMember(9)]
		public int injuredRoleMaxMagicV;

		[ProtoMember(10)]
		public int hitToGridX;

		[ProtoMember(11)]
		public int hitToGridY;

		[ProtoMember(12)]
		public int MerlinInjuer;

		[ProtoMember(13)]
		public int MerlinType;

		[ProtoMember(14)]
		public int stopCaiJi;

		[ProtoMember(15)]
		public int armorV_p1;
	}
}
