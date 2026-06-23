using System;
using ProtoBuf;

[ProtoContract]
public class OnePieceTreasureData
{
	[ProtoMember(1)]
	public int PosID;

	[ProtoMember(2)]
	public int EventID;

	[ProtoMember(3)]
	public int RollNumNormal;

	[ProtoMember(4)]
	public int RollNumMiracle;

	[ProtoMember(5)]
	public long ResetPosTicks;
}
