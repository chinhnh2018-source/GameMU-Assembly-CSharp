using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SCFindMonster : IProtoBuffData
	{
		public SCFindMonster()
		{
		}

		public SCFindMonster(int roleID, int x, int y, int num)
		{
			this.RoleID = roleID;
			this.X = x;
			this.Y = y;
			this.Num = num;
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
					this.RoleID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 2:
					this.X = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.Y = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.Num = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.RoleID, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.X, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.Y, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.Num, true, 4, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.X, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.Y, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.Num, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public int X;

		[ProtoMember(3)]
		public int Y;

		[ProtoMember(4)]
		public int Num;
	}
}
