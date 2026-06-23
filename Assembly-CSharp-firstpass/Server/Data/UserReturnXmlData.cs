using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class UserReturnXmlData
	{
		[ProtoMember(1)]
		public List<string> XmlNameList;

		[ProtoMember(2)]
		public List<string> XmlList;
	}
}
