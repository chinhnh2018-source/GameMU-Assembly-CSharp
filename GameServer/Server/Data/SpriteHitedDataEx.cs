using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	[ProtoContract]
	public class SpriteHitedDataEx : IProtoBuffData
	{
		public void AddEnemy(int enemy, int enemyX = 0, int enemyY = 0)
		{
			this.enemys.Add(new SpriteHitedInnerData(enemy, enemyX, enemyY));
		}

		public int getMySize()
		{
			int num = 0;
			num += ProtoUtil.GetIntSize(this.roleId, true, 1, true, 0);
			num += ProtoUtil.GetIntSize(this.magicCode, true, 2, true, 0);
			return num + ProtoUtil.GetListBytesSize<SpriteHitedInnerData>(this.enemys, true, 3);
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
					this.roleId = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 2:
					this.magicCode = ProtoUtil.IntMemberFromBytes(data, wt, ref result, ref i);
					break;
				case 3:
					ProtoUtil.ListMemberFromBytes<SpriteHitedInnerData>(this.enemys, data, wt, ref result, ref i);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return result;
		}

		public byte[] toBytes()
		{
			int mySize = this.getMySize();
			byte[] array = new byte[mySize];
			int num = 0;
			ProtoUtil.IntMemberToBytes(array, 1, ref num, this.roleId, true, 0);
			ProtoUtil.IntMemberToBytes(array, 2, ref num, this.magicCode, true, 0);
			ProtoUtil.ListToBytes<SpriteHitedInnerData>(this.enemys, 3, ref num, array);
			return array;
		}

		[ProtoMember(1)]
		public int roleId;

		[ProtoMember(2)]
		public int magicCode;

		[ProtoMember(3)]
		public List<SpriteHitedInnerData> enemys = new List<SpriteHitedInnerData>();
	}
}
