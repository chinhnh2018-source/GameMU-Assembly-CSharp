using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KuaFuLueDuoServerInfoList
	{
		[ProtoMember(1)]
		public long Age;

		[ProtoMember(2)]
		public List<KuaFuLueDuoServerInfo> List = new List<KuaFuLueDuoServerInfo>();
	}
}
