using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class CoupleWishWishRecordData
	{
		[ProtoMember(1)]
		public KuaFuRoleMiniData FromRole;

		[ProtoMember(2)]
		public List<KuaFuRoleMiniData> TargetRoles;

		[ProtoMember(3)]
		public int WishType;

		[ProtoMember(4)]
		public string WishTxt;
	}
}
