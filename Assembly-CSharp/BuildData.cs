using System;
using ProtoBuf;

[ProtoContract]
public class BuildData
{
	[ProtoMember(1)]
	public int BuildID;

	[ProtoMember(2)]
	public int Lev;

	[ProtoMember(3)]
	public int Exp;

	[ProtoMember(4)]
	public string DevelopTime;

	[ProtoMember(5)]
	public int Task1;

	[ProtoMember(6)]
	public int Task2;

	[ProtoMember(7)]
	public int Task3;

	[ProtoMember(8)]
	public int Task4;
}
