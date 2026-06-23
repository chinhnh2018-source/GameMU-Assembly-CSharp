using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class WingData
	{
		[ProtoMember(1)]
		public int DbID;

		[ProtoMember(2)]
		public int WingID;

		[ProtoMember(3)]
		public int ForgeLevel;

		[ProtoMember(4)]
		public long AddDateTime;

		[ProtoMember(5)]
		public int JinJieFailedNum;

		[ProtoMember(6)]
		public int Using;

		[ProtoMember(7)]
		public int StarExp;

		[ProtoMember(8)]
		public int ZhuLingNum;

		[ProtoMember(9)]
		public int ZhuHunNum;
	}
}
