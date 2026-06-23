using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class OneKeyFindFriendData
	{
		[ProtoMember(1)]
		public int m_nRoleID;

		[ProtoMember(2)]
		public string m_nRoleName = string.Empty;

		[ProtoMember(3)]
		public int m_nLevel;

		[ProtoMember(4)]
		public int m_nChangeLifeLev;

		[ProtoMember(5)]
		public int m_nOccupation;
	}
}
