using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class MerlinGrowthSaveDBData
	{
		[ProtoMember(1)]
		public int _RoleID;

		[ProtoMember(2)]
		public int _Occupation;

		[ProtoMember(3)]
		public int _Level;

		[ProtoMember(4)]
		public int _StarNum;

		[ProtoMember(5)]
		public int _StarExp;

		[ProtoMember(6)]
		public int _LuckyPoint;

		[ProtoMember(7)]
		public long _ToTicks;

		[ProtoMember(8)]
		public long _AddTime;

		[ProtoMember(9)]
		public Dictionary<int, double> _ActiveAttr = new Dictionary<int, double>();

		[ProtoMember(10)]
		public Dictionary<int, double> _UnActiveAttr = new Dictionary<int, double>();

		[ProtoMember(11)]
		public int LevelUpFailNum;
	}
}
