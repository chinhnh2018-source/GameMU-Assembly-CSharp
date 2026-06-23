using System;
using System.Collections.Generic;
using ProtoBuf;

[ProtoContract]
public class OnePieceTreasureEvent
{
	[ProtoMember(1)]
	public int EventID;

	[ProtoMember(2)]
	public int EventValue;

	[ProtoMember(3)]
	public List<int> BoxList;

	[ProtoMember(4)]
	public int ErrCode;
}
