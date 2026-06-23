using System;
using System.Collections.Generic;
using ProtoBuf;

[ProtoContract]
public class MyHongBaoData
{
	[ProtoMember(1)]
	public int type;

	[ProtoMember(2)]
	public List<HongBaoItemData> items;

	[ProtoMember(3)]
	public long flag;
}
