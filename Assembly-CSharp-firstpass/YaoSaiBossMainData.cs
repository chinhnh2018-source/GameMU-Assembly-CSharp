using System;
using ProtoBuf;

[ProtoContract]
public class YaoSaiBossMainData
{
	[ProtoMember(1)]
	public YaoSaiBossData BossMiniInfo;

	[ProtoMember(2)]
	public int TaoFaCount;

	[ProtoMember(3)]
	public int HaveZhaoHuanCount;

	[ProtoMember(4)]
	public int ZhaoHuanBossIDss;

	[ProtoMember(5)]
	public int OtherID;

	[ProtoMember(6)]
	public int FightCount;
}
