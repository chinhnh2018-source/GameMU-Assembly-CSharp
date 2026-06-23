using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class FuMoMailData
	{
		[ProtoMember(1)]
		public int MaillID;

		[ProtoMember(2)]
		public int SenderRID;

		[ProtoMember(3)]
		public string SenderRName = string.Empty;

		[ProtoMember(4)]
		public int SenderJob;

		[ProtoMember(5)]
		public string SendTime = string.Empty;

		[ProtoMember(6)]
		public int ReceiverRID;

		[ProtoMember(7)]
		public int IsRead;

		[ProtoMember(8)]
		public string ReadTime = "1900-01-01 12:00:00";

		[ProtoMember(9)]
		public int FuMoMoney;

		[ProtoMember(10)]
		public string Content = string.Empty;
	}
}
