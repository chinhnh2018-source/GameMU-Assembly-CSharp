using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SpriteManaChangeData : IProtoBuffData
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
				case 4:
					goto IL_76;
				case 3:
					this.magicV = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.currentMagicV = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				default:
					goto IL_76;
				}
				continue;
				IL_76:
				throw new ArgumentException("error!!!");
			}
			return result;
		}

		public byte[] toBytes()
		{
			int num = 0;
			num += ProtoUtil.GetIntSize(this.roleID, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.magicV, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.currentMagicV, true, 5, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.magicV, true, 0);
			ProtoUtil.IntMemberToBytes(array, 5, ref num2, this.currentMagicV, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int roleID;

		[ProtoMember(3)]
		public int magicV;

		[ProtoMember(5)]
		public int currentMagicV;
	}
}
