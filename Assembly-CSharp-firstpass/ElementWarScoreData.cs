using System;
using ProtoBuf;

[ProtoContract]
public class ElementWarScoreData
{
	[ProtoMember(1)]
	public int Wave;

	[ProtoMember(2)]
	public long EndTime;

	[ProtoMember(3)]
	public long MonsterCount;
}
