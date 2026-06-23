using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class NewGoodsPackData : IProtoBuffData
	{
		public int getBytesSize()
		{
			int num = 0;
			num += ProtoUtil.GetIntSize(this.ownerRoleID, true, 1, true, 0);
			num += ProtoUtil.GetStringSize(this.ownerRoleName, true, 2);
			num += ProtoUtil.GetIntSize(this.autoID, true, 3, true, 0);
			num += ProtoUtil.GetIntSize(this.goodsPackID, true, 4, true, 0);
			num += ProtoUtil.GetIntSize(this.mapCode, true, 5, true, 0);
			num += ProtoUtil.GetIntSize(this.toX, true, 6, true, 0);
			num += ProtoUtil.GetIntSize(this.toY, true, 7, true, 0);
			num += ProtoUtil.GetIntSize(this.goodsID, true, 8, true, 0);
			num += ProtoUtil.GetIntSize(this.goodsNum, true, 9, true, 0);
			num += ProtoUtil.GetLongSize(this.productTicks, true, 10, true, 0L);
			num += ProtoUtil.GetLongSize(this.teamID, true, 11, true, 0L);
			num += ProtoUtil.GetStringSize(this.teamRoleIDs, true, 12);
			num += ProtoUtil.GetIntSize(this.lucky, true, 13, true, 0);
			num += ProtoUtil.GetIntSize(this.excellenceInfo, true, 14, true, 0);
			num += ProtoUtil.GetIntSize(this.appendPropLev, true, 15, true, 0);
			return num + ProtoUtil.GetIntSize(this.forge_Level, true, 16, true, 0);
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
					this.ownerRoleID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 2:
					this.ownerRoleName = ProtoUtil.StringMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					this.autoID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 4:
					this.goodsPackID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 5:
					this.mapCode = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 6:
					this.toX = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 7:
					this.toY = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 8:
					this.goodsID = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 9:
					this.goodsNum = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 10:
					this.productTicks = ProtoUtil.LongMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 11:
					this.teamID = (long)ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 12:
					this.teamRoleIDs = ProtoUtil.StringMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 13:
					this.lucky = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 14:
					this.excellenceInfo = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 15:
					this.appendPropLev = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 16:
					this.forge_Level = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return result;
		}

		public byte[] toBytes()
		{
			int bytesSize = this.getBytesSize();
			byte[] array = new byte[bytesSize];
			int num = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num, this.ownerRoleID, true, 0);
			ProtoUtil.StringMemberToBytes(array, 2, ref num, this.ownerRoleName);
			ProtoUtil.IntMemberToBytes(array, 3, ref num, this.autoID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 4, ref num, this.goodsPackID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 5, ref num, this.mapCode, true, 0);
			ProtoUtil.IntMemberToBytes(array, 6, ref num, this.toX, true, 0);
			ProtoUtil.IntMemberToBytes(array, 7, ref num, this.toY, true, 0);
			ProtoUtil.IntMemberToBytes(array, 8, ref num, this.goodsID, true, 0);
			ProtoUtil.IntMemberToBytes(array, 9, ref num, this.goodsNum, true, 0);
			ProtoUtil.LongMemberToBytes(array, 10, ref num, this.productTicks, true, 0L);
			ProtoUtil.LongMemberToBytes(array, 11, ref num, this.teamID, true, 0L);
			ProtoUtil.StringMemberToBytes(array, 12, ref num, this.teamRoleIDs);
			ProtoUtil.IntMemberToBytes(array, 13, ref num, this.lucky, true, 0);
			ProtoUtil.IntMemberToBytes(array, 14, ref num, this.excellenceInfo, true, 0);
			ProtoUtil.IntMemberToBytes(array, 15, ref num, this.appendPropLev, true, 0);
			ProtoUtil.IntMemberToBytes(array, 16, ref num, this.forge_Level, true, 0);
			return array;
		}

		[ProtoMember(1)]
		public int ownerRoleID;

		[ProtoMember(2)]
		public string ownerRoleName = "";

		[ProtoMember(3)]
		public int autoID;

		[ProtoMember(4)]
		public int goodsPackID;

		[ProtoMember(5)]
		public int mapCode;

		[ProtoMember(6)]
		public int toX;

		[ProtoMember(7)]
		public int toY;

		[ProtoMember(8)]
		public int goodsID;

		[ProtoMember(9)]
		public int goodsNum;

		[ProtoMember(10)]
		public long productTicks;

		[ProtoMember(11)]
		public long teamID;

		[ProtoMember(12)]
		public string teamRoleIDs = "";

		[ProtoMember(13)]
		public int lucky;

		[ProtoMember(14)]
		public int excellenceInfo;

		[ProtoMember(15)]
		public int appendPropLev;

		[ProtoMember(16)]
		public int forge_Level;
	}
}
