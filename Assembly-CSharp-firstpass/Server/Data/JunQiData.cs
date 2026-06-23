using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class JunQiData
	{
		[ProtoMember(1)]
		public int JunQiID;

		[ProtoMember(2)]
		public string QiName = string.Empty;

		[ProtoMember(3)]
		public int JunQiLevel;

		[ProtoMember(4)]
		public int ZoneID;

		[ProtoMember(5)]
		public int BHID;

		[ProtoMember(6)]
		public string BHName = string.Empty;

		[ProtoMember(7)]
		public int QiZuoNPC;

		[ProtoMember(8)]
		public int MapCode;

		[ProtoMember(9)]
		public int PosX;

		[ProtoMember(10)]
		public int PosY;

		[ProtoMember(11)]
		public int Direction;

		[ProtoMember(12)]
		public int LifeV;

		[ProtoMember(13)]
		public int CutLifeV;

		[ProtoMember(14)]
		public long StartTime;

		[ProtoMember(15)]
		public int BodyCode;

		[ProtoMember(16)]
		public int PicCode;

		[ProtoMember(17)]
		public int CurrentLifeV;
	}
}
