using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZhengBaNtfPkResultData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public int RandGroup;

		[ProtoMember(3)]
		public int StillNeedWin;

		[ProtoMember(4)]
		public int LeftUpGradeNum;

		[ProtoMember(5)]
		public bool IsWin;

		[ProtoMember(6)]
		public bool IsUpGrade;

		[ProtoMember(7)]
		public int NewGrade;
	}
}
