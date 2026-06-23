using System;
using System.Collections.Generic;
using ProtoBuf;
using Server.Data;

namespace Tmsk.Contract.KuaFuData
{
	[ProtoContract]
	public class KFCompData
	{
		[ProtoMember(1)]
		public int CompType;

		[ProtoMember(2)]
		public int BoomValue;

		[ProtoMember(3)]
		public int EnemyCompType;

		[ProtoMember(4)]
		public int EnemyCompTypeSet;

		[ProtoMember(5)]
		public string Bulletin;

		[ProtoMember(6)]
		public int Crystal;

		[ProtoMember(7)]
		public int Boss;

		[ProtoMember(8)]
		public int YestdCrystal;

		[ProtoMember(9)]
		public int YestdBoss;

		[ProtoMember(10)]
		public List<int> PlunderResList = new List<int>();

		[ProtoMember(11)]
		public int YestdBossKillCompType;

		[ProtoMember(12)]
		public int SelfJunXian;

		[ProtoMember(13)]
		public CompBattleBaseData compBattleBaseData;

		[ProtoMember(14)]
		public int CompTypeBattle;

		[ProtoMember(15)]
		public int CompBattleStates;
	}
}
