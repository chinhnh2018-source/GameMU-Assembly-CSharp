using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KFChat
	{
		public KFChat()
		{
		}

		public KFChat(int zoneId, string roleName, string text, int junTuanId)
		{
			this.ZoneId = zoneId;
			this.RoleName = roleName;
			this.Text = text;
			this.JunTuanId = junTuanId;
		}

		[ProtoMember(1)]
		public int ZoneId;

		[ProtoMember(2)]
		public string RoleName;

		[ProtoMember(3)]
		public string Text;

		[ProtoMember(4)]
		public int JunTuanId;
	}
}
