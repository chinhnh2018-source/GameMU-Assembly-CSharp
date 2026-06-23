using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KFRankData
	{
		public int CompareTo(KFRankData other)
		{
			if (this.Grade == other.Grade)
			{
				return (!(this.RankTime > other.RankTime)) ? 1 : -1;
			}
			return (this.Grade <= other.Grade) ? 1 : -1;
		}

		public KFRankData Clone(KFRankData target)
		{
			this.Rank = target.Rank;
			this.ZoneID = target.ZoneID;
			this.RoleID = target.RoleID;
			this.RoleName = target.RoleName;
			this.Grade = target.Grade;
			this.RoleData = target.RoleData;
			this.RankType = target.RankType;
			this.RankTime = target.RankTime;
			this.RankOld = target.RankOld;
			this.ServerID = target.ServerID;
			this.GradeOld = target.GradeOld;
			return this;
		}

		[ProtoMember(1)]
		public int Rank;

		[ProtoMember(2)]
		public int ZoneID;

		[ProtoMember(3)]
		public int RoleID;

		[ProtoMember(4)]
		public string RoleName = string.Empty;

		[ProtoMember(5)]
		public int Grade;

		[ProtoMember(6)]
		public byte[] RoleData;

		[ProtoMember(7)]
		public int RankType;

		[ProtoMember(8)]
		public DateTime RankTime = DateTime.MinValue;

		[ProtoMember(9)]
		public int RankOld;

		[ProtoMember(10)]
		public int ServerID;

		[ProtoMember(11)]
		public int GradeOld;
	}
}
