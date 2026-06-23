using System;
using ProtoBuf;

[ProtoContract]
public class TarotCardData
{
	[ProtoMember(1)]
	public int GoodId;

	[ProtoMember(2)]
	public int Level;

	[ProtoMember(3)]
	public byte Postion;

	[ProtoMember(4)]
	public int TarotMoney;
}
