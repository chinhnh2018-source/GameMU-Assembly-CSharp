using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class BangHuiMatchPKInfo
	{
		[ProtoMember(1)]
		public int bhid1;

		[ProtoMember(2)]
		public string bhname1 = string.Empty;

		[ProtoMember(3)]
		public int bhid2;

		[ProtoMember(4)]
		public string bhname2 = string.Empty;

		[ProtoMember(5)]
		public byte result;

		[ProtoMember(6)]
		public byte round;

		[ProtoMember(7)]
		public int zoneid1;

		[ProtoMember(8)]
		public int zoneid2;

		[ProtoMember(9), NonSerialized]
		public byte guess;

		[ProtoMember(10), NonSerialized]
		public byte rank1;

		[ProtoMember(11), NonSerialized]
		public byte win1;

		[ProtoMember(12), NonSerialized]
		public byte winpct1;

		[ProtoMember(13), NonSerialized]
		public byte rank2;

		[ProtoMember(14), NonSerialized]
		public byte win2;

		[ProtoMember(15), NonSerialized]
		public byte winpct2;

		[NonSerialized]
		public byte type;

		[NonSerialized]
		public int season;
	}
}
