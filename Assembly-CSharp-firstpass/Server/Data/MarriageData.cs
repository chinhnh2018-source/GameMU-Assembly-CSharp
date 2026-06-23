using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class MarriageData
	{
		[ProtoMember(1)]
		public int nSpouseID = -1;

		[ProtoMember(2)]
		public sbyte byMarrytype = -1;

		[ProtoMember(3)]
		public int nRingID = -1;

		[ProtoMember(4)]
		public int nGoodwillexp;

		[ProtoMember(5)]
		public sbyte byGoodwillstar;

		[ProtoMember(6)]
		public sbyte byGoodwilllevel;

		[ProtoMember(7)]
		public int nGivenrose;

		[ProtoMember(8)]
		public string strLovemessage = string.Empty;

		[ProtoMember(9)]
		public sbyte byAutoReject;
	}
}
