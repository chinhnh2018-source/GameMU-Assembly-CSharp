using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BangHuiMgrItemData
	{
		[ProtoMember(1)]
		public int ZoneID;

		[ProtoMember(2)]
		public int RoleID;

		[ProtoMember(3)]
		public string RoleName = string.Empty;

		[ProtoMember(4)]
		public int Occupation;

		[ProtoMember(5)]
		public int BHZhiwu;

		[ProtoMember(6)]
		public string ChengHao = string.Empty;

		[ProtoMember(7)]
		public int BangGong;

		[ProtoMember(8)]
		public int Level;
	}
}
