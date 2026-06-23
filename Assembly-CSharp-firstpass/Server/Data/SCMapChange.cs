using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SCMapChange : IProtoBuffData
	{
		public SCMapChange()
		{
		}

		public SCMapChange(int roleID, int teleportID, int newMapCode, int toNewMapX, int toNewMapY, int toNewDiection, int state)
		{
			this.RoleID = roleID;
			this.TeleportID = teleportID;
			this.NewMapCode = newMapCode;
			this.ToNewMapX = toNewMapX;
			this.ToNewMapY = toNewMapY;
			this.ToNewDiection = toNewDiection;
			this.State = state;
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
					this.TeleportID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.NewMapCode = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.ToNewMapX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.ToNewMapY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 6:
					this.ToNewDiection = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 7:
					this.State = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
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
			num += ProtoUtil.GetIntSize(this.TeleportID, true, 2, true, 0);
			num += ProtoUtil.GetIntSize(this.NewMapCode, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.ToNewMapX, true, 4, true, 0);
			num += ProtoUtil.GetIntSize(this.ToNewMapY, true, 5, true, 0);
			num += ProtoUtil.GetIntSize(this.ToNewDiection, true, 6, true, 0);
			num += ProtoUtil.GetIntSize(this.State, true, 7, true, 0);
			byte[] array = new byte[num];
			int num2 = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.TeleportID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.NewMapCode, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.ToNewMapX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 5, ref num2, this.ToNewMapY, true, 0);
			ProtoUtil.IntMemberToBytes(array, 6, ref num2, this.ToNewDiection, true, 0);
			ProtoUtil.IntMemberToBytes(array, 7, ref num2, this.State, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public int TeleportID;

		[ProtoMember(3)]
		public int NewMapCode;

		[ProtoMember(4)]
		public int ToNewMapX;

		[ProtoMember(5)]
		public int ToNewMapY;

		[ProtoMember(6)]
		public int ToNewDiection;

		[ProtoMember(7)]
		public int State;
	}
}
