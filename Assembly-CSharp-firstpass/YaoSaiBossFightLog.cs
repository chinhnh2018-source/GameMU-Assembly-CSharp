using System;
using ProtoBuf;

[ProtoContract]
public class YaoSaiBossFightLog
{
	[ProtoMember(1)]
	public int OtherRid;

	[ProtoMember(2)]
	public string OtherRname;

	[ProtoMember(3)]
	public int InviteType;

	[ProtoMember(4)]
	public int FightLife;
}
