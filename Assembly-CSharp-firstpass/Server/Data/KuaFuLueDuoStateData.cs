using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KuaFuLueDuoStateData
	{
		[ProtoMember(1)]
		public long Age;

		[ProtoMember(2)]
		public int GameState;

		[ProtoMember(3)]
		public int ZiYuan;

		[ProtoMember(4)]
		public List<BangHuiMiniData> AttackerList;

		[ProtoMember(5)]
		public int EnemyServerID;

		[ProtoMember(6)]
		public int EnemyZiYuan;

		[ProtoMember(7)]
		public int LueDuoZiYuan;

		[ProtoMember(8)]
		public int ServerID;

		[ProtoMember(10)]
		public List<KuaFuLueDuoAwardsData> AwardsDataList;
	}
}
