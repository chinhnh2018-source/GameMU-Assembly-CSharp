using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class CompBattleZhuJiangInfo
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string Name;

		[ProtoMember(3)]
		public int Level;

		[ProtoMember(4)]
		public int ZoneID;

		[ProtoMember(5)]
		public int Occupation;

		[ProtoMember(6)]
		public int RoleSex;

		[ProtoMember(7)]
		public int CompZhiWu;
	}
}
