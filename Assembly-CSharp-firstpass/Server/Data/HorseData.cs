using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class HorseData
	{
		[ProtoMember(1)]
		public int DbID;

		[ProtoMember(2)]
		public int HorseID;

		[ProtoMember(3)]
		public int BodyID;

		[ProtoMember(4)]
		public string PropsNum = string.Empty;

		[ProtoMember(5)]
		public string PropsVal = string.Empty;

		[ProtoMember(6)]
		public long AddDateTime;

		[ProtoMember(7)]
		public int JinJieFailedNum;

		[ProtoMember(8)]
		public long JinJieTempTime;

		[ProtoMember(9)]
		public int JinJieTempNum;

		[ProtoMember(10)]
		public int JinJieFailedDayID;
	}
}
