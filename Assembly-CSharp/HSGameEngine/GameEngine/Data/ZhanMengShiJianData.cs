using System;
using ProtoBuf;

namespace HSGameEngine.GameEngine.Data
{
	[ProtoContract]
	public class ZhanMengShiJianData
	{
		[ProtoMember(1)]
		public int BHID;

		[ProtoMember(2)]
		public int ShiJianType;

		[ProtoMember(3)]
		public string RoleName = string.Empty;

		[ProtoMember(4)]
		public string CreateTime = string.Empty;

		[ProtoMember(5)]
		public int SubValue1 = -1;

		[ProtoMember(6)]
		public int SubValue2 = -1;

		[ProtoMember(7)]
		public int SubValue3 = -1;

		[ProtoMember(8)]
		public string SubSzValue1 = string.Empty;
	}
}
