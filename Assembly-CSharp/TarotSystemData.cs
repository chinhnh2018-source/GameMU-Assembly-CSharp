using System;
using System.Collections.Generic;
using ProtoBuf;

[ProtoContract]
public class TarotSystemData
{
	[ProtoMember(1)]
	public TarotKingData KingData;

	[ProtoMember(2)]
	public List<TarotCardData> TarotCardDatas;
}
