using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SCModGoods : IProtoBuffData
	{
		public SCModGoods()
		{
		}

		public SCModGoods(int state, int modType, int id, int isusing, int site, int gcount, int bagIndex, int newHint)
		{
			this.State = state;
			this.ModType = modType;
			this.ID = id;
			this.IsUsing = isusing;
			this.Site = site;
			this.Count = gcount;
			this.BagIndex = bagIndex;
			this.NewHint = newHint;
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
					this.State = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 2:
					this.ModType = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.ID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.IsUsing = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.Site = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 6:
					this.Count = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 7:
					this.BagIndex = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 8:
					this.NewHint = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.State, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.ModType, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.ID, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.IsUsing, true, 4, true, 0);
			num += ProtoUtil.GetIntSize(this.Site, true, 5, true, 0);
			num += ProtoUtil.GetIntSize(this.Count, true, 6, true, 0);
			num += ProtoUtil.GetIntSize(this.BagIndex, true, 7, true, 0);
			num += ProtoUtil.GetIntSize(this.NewHint, true, 8, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.State, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.ModType, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.ID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.IsUsing, true, 0);
			ProtoUtil.IntMemberToBytes(array, 5, ref num2, this.Site, true, 0);
			ProtoUtil.IntMemberToBytes(array, 6, ref num2, this.Count, true, 0);
			ProtoUtil.IntMemberToBytes(array, 7, ref num2, this.BagIndex, true, 0);
			ProtoUtil.IntMemberToBytes(array, 8, ref num2, this.NewHint, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int State;

		[ProtoMember(2)]
		public int ModType;

		[ProtoMember(3)]
		public int ID;

		[ProtoMember(4)]
		public int IsUsing;

		[ProtoMember(5)]
		public int Site;

		[ProtoMember(6)]
		public int Count;

		[ProtoMember(7)]
		public int BagIndex;

		[ProtoMember(8)]
		public int NewHint;
	}
}
