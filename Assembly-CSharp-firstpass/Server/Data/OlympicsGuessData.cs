using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class OlympicsGuessData
	{
		[ProtoMember(1)]
		public int ID;

		[ProtoMember(2)]
		public int DayID;

		[ProtoMember(3)]
		public string Content = string.Empty;

		[ProtoMember(4)]
		public string A = string.Empty;

		[ProtoMember(5)]
		public string B = string.Empty;

		[ProtoMember(6)]
		public string C = string.Empty;

		[ProtoMember(7)]
		public string D = string.Empty;

		[ProtoMember(8)]
		public int Answer = -1;

		[ProtoMember(9)]
		public int Grade;

		[ProtoMember(10)]
		public int Select = -1;
	}
}
