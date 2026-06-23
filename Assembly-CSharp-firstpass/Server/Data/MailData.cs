using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class MailData
	{
		[ProtoMember(1)]
		public int MailID;

		[ProtoMember(2)]
		public int SenderRID;

		[ProtoMember(3)]
		public string SenderRName = string.Empty;

		[ProtoMember(4)]
		public string SendTime = string.Empty;

		[ProtoMember(5)]
		public int ReceiverRID;

		[ProtoMember(6)]
		public string ReveiverRName = string.Empty;

		[ProtoMember(7)]
		public string ReadTime = "1900-01-01 12:00:00";

		[ProtoMember(8)]
		public int IsRead;

		[ProtoMember(9)]
		public int MailType;

		[ProtoMember(10)]
		public int Hasfetchattachment;

		[ProtoMember(11)]
		public string Subject = string.Empty;

		[ProtoMember(12)]
		public string Content = string.Empty;

		[ProtoMember(13)]
		public int Yinliang;

		[ProtoMember(14)]
		public int Tongqian;

		[ProtoMember(15)]
		public int YuanBao;

		[ProtoMember(16)]
		public List<MailGoodsData> GoodsList;
	}
}
