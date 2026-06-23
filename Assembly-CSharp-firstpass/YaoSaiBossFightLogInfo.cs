using System;
using System.Collections.Generic;
using ProtoBuf;

[ProtoContract]
public class YaoSaiBossFightLogInfo
{
	[ProtoMember(1)]
	public int OtherRid;

	[ProtoMember(2)]
	public List<YaoSaiBossFightLog> BossFightLogList;
}
