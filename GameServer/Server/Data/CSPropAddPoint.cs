using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class CSPropAddPoint : IProtoBuffData
	{
		public CSPropAddPoint()
		{
			this.RoleID = 0;
			this.Strength = 0;
			this.Intelligence = 0;
			this.Dexterity = 0;
			this.Constitution = 0;
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
					this.Strength = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.Intelligence = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.Dexterity = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.Constitution = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.Strength, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.Intelligence, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.Dexterity, true, 4, true, 0);
			num += ProtoUtil.GetIntSize(this.Constitution, true, 5, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.Strength, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.Intelligence, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.Dexterity, true, 0);
			ProtoUtil.IntMemberToBytes(array, 5, ref num2, this.Constitution, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int RoleID = 0;

		[ProtoMember(2)]
		public int Strength = 0;

		[ProtoMember(3)]
		public int Intelligence = 0;

		[ProtoMember(4)]
		public int Dexterity = 0;

		[ProtoMember(5)]
		public int Constitution = 0;
	}
}
