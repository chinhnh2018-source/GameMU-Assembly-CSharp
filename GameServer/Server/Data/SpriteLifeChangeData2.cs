using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SpriteLifeChangeData2 : IProtoBuffData
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
					goto IL_72;
				case 4:
					this.currentLifeV = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				default:
					goto IL_72;
				}
				continue;
				IL_72:
				throw new ArgumentException("error!!!");
			}
			return result;
		}

		public byte[] toBytes()
		{
			int num = 0;
			num += ProtoUtil.GetIntSize(this.roleID, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.lifeV, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.currentLifeV, true, 4, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.lifeV, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.currentLifeV, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int roleID;

		[ProtoMember(2)]
		public int lifeV;

		[ProtoMember(4)]
		public int currentLifeV;
	}
}
