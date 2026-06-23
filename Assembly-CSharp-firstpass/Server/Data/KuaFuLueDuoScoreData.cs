using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KuaFuLueDuoScoreData
	{
		[ProtoMember(1, IsRequired = true)]
		public int LeftZiYuan;

		[ProtoMember(2)]
		public int LueDuoZiYuan;

		[ProtoMember(3)]
		public int SelfScore;

		[ProtoMember(4)]
		public int LeftNum;
	}
}
