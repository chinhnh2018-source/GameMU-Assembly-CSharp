using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class SpecialActivityData
	{
		[ProtoMember(1)]
		public int GroupID;

		[ProtoMember(2)]
		public List<SpecActInfo> SpecActInfoList;
	}
}
