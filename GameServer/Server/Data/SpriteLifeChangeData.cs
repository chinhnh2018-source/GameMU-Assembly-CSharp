using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SpriteLifeChangeData : IProtoBuffData
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
					this.lifeV = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.magicV = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.currentLifeV = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.currentMagicV = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 6:
					this.ArmorV = ProtoUtil.LongMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 7:
					this.currentArmorV = ProtoUtil.LongMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.lifeV, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.magicV, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.currentLifeV, true, 4, true, 0);
			num += ProtoUtil.GetIntSize(this.currentMagicV, true, 5, true, 0);
			num += ProtoUtil.GetLongSize(this.ArmorV, true, 6, true, 0L);
			num += ProtoUtil.GetLongSize(this.currentArmorV, true, 7, true, 0L);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.lifeV, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.magicV, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.currentLifeV, true, 0);
			ProtoUtil.IntMemberToBytes(array, 5, ref num2, this.currentMagicV, true, 0);
			ProtoUtil.LongMemberToBytes(array, 6, ref num2, this.ArmorV, true, 0L);
			ProtoUtil.LongMemberToBytes(array, 7, ref num2, this.currentArmorV, true, 0L);
			return array;
		}

		[ProtoMember(1)]
		public int roleID;

		[ProtoMember(2)]
		public int lifeV;

		[ProtoMember(3)]
		public int magicV;

		[ProtoMember(4)]
		public int currentLifeV;

		[ProtoMember(5)]
		public int currentMagicV;

		[ProtoMember(6)]
		public long ArmorV;

		[ProtoMember(7)]
		public long currentArmorV;
	}
}
