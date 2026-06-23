using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class JieriXmlData
	{
		[ProtoMember(1)]
		public List<string> XmlList;

		[ProtoMember(2)]
		public int Version;
	}
}
