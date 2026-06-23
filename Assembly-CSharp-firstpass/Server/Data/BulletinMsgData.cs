using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BulletinMsgData
	{
		[ProtoMember(1)]
		public string MsgID = string.Empty;

		[ProtoMember(2)]
		public int PlayMinutes = -1;

		[ProtoMember(3)]
		public int ToPlayNum;

		[ProtoMember(4)]
		public string BulletinText = string.Empty;

		[ProtoMember(5)]
		public long BulletinTicks;

		[ProtoMember(6)]
		public int playingNum;

		[ProtoMember(7)]
		public int MsgType;
	}
}
