using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SC_SprUseGoods : IProtoBuffData
	{
		public SC_SprUseGoods()
		{
		}

		public SC_SprUseGoods(int error, int dbId, int cnt)
		{
			this.Error = error;
			this.DbId = dbId;
			this.Cnt = cnt;
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
					this.Error = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 2:
					this.DbId = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.Cnt = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.Error, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.DbId, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.Cnt, true, 3, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.Error, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.DbId, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.Cnt, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int Error;

		[ProtoMember(2)]
		public int DbId;

		[ProtoMember(3)]
		public int Cnt;
	}
}
