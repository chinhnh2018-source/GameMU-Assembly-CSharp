using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class RebornEquipData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public int HoleID;

		[ProtoMember(3)]
		public int Level;

		[ProtoMember(4)]
		public int Able;
	}
}
