using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class CoupleArenaPkResultData
	{
		[ProtoMember(1)]
		public int PKResult;

		[ProtoMember(2)]
		public int GetRongYao;

		[ProtoMember(3)]
		public int GetJiFen;

		[ProtoMember(4)]
		public int DuanWeiType;

		[ProtoMember(5)]
		public int DuanWeiLevel;
	}
}
