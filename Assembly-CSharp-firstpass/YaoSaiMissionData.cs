using System;
using ProtoBuf;

[ProtoContract]
public class YaoSaiMissionData
{
	public int ClientTaskIndex
	{
		get
		{
			return this.SiteID - 1;
		}
	}

	[ProtoMember(1)]
	public int SiteID;

	[ProtoMember(2)]
	public int MissionID;

	[ProtoMember(3)]
	public int State;

	[ProtoMember(4)]
	public string ZhiPaiJingLing;

	[ProtoMember(5)]
	public DateTime StartTime;
}
