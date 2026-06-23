using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BiaoCheData
	{
		[ProtoMember(1)]
		public int OwnerRoleID;

		[ProtoMember(2)]
		public int BiaoCheID;

		[ProtoMember(3)]
		public string BiaoCheName = string.Empty;

		[ProtoMember(4)]
		public int YaBiaoID;

		[ProtoMember(5)]
		public int MapCode;

		[ProtoMember(6)]
		public int PosX;

		[ProtoMember(7)]
		public int PosY;

		[ProtoMember(8)]
		public int Direction;

		[ProtoMember(9)]
		public int LifeV;

		[ProtoMember(10)]
		public int CutLifeV;

		[ProtoMember(11)]
		public long StartTime;

		[ProtoMember(12)]
		public int BodyCode;

		[ProtoMember(13)]
		public int PicCode;

		[ProtoMember(14)]
		public int CurrentLifeV;

		[ProtoMember(15)]
		public string OwnerRoleName = string.Empty;
	}
}
