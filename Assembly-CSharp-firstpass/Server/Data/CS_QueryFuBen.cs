using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class CS_QueryFuBen : IProtoBuffData
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
					this.RoleId = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 2:
					this.MapId = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.FuBenId = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.RoleId, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.MapId, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.FuBenId, true, 3, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.RoleId, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.MapId, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.FuBenId, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int RoleId;

		[ProtoMember(2)]
		public int MapId;

		[ProtoMember(3)]
		public int FuBenId;
	}
}
