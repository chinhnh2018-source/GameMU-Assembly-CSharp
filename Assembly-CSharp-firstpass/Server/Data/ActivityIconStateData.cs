using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class ActivityIconStateData : IProtoBuffData
	{
		public int fromBytes(byte[] data, int offset, int count)
		{
			int result = offset;
			int i = 0;
			int num = 0;
			while (i < count)
			{
				int num2 = -1;
				int wt = -1;
				ProtoUtil.GetTag(data, ref result, ref num2, ref wt, ref i);
				int num3 = num2;
				if (num3 != 1)
				{
					throw new ArgumentException("error!!!");
				}
				num++;
				ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
			}
			if (num > 0)
			{
				this.arrIconState = new ushort[num];
				result = offset;
				i = 0;
				num = 0;
				while (i < count)
				{
					int num4 = -1;
					int wt2 = -1;
					ProtoUtil.GetTag(data, ref result, ref num4, ref wt2, ref i);
					int num3 = num4;
					if (num3 != 1)
					{
						throw new ArgumentException("error!!!");
					}
					this.arrIconState[num++] = (ushort)ProtoUtil.IntMemberFromBytes(data, wt2, ref result, ref i);
				}
			}
			return result;
		}

		public byte[] toBytes()
		{
			int num = 0;
			if (this.arrIconState != null && this.arrIconState.Length > 0)
			{
				for (int i = 0; i < this.arrIconState.Length; i++)
				{
					num += ProtoUtil.GetIntSize((int)this.arrIconState[i], true, 1, true, 0);
				}
			}
			byte[] array = new byte[num];
			int num2 = 0;
			if (this.arrIconState != null && this.arrIconState.Length > 0)
			{
				for (int j = 0; j < this.arrIconState.Length; j++)
				{
					ProtoUtil.IntMemberToBytes(array, 1, ref num2, (int)this.arrIconState[j], true, 0);
				}
			}
			return array;
		}

		[ProtoMember(1)]
		public ushort[] arrIconState;
	}
}
