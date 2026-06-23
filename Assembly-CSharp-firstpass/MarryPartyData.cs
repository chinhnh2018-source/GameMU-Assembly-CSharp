using System;
using ProtoBuf;

[ProtoContract]
public class MarryPartyData
{
	[ProtoMember(1)]
	public int RoleID = -1;

	[ProtoMember(2)]
	public int PartyType = -1;

	[ProtoMember(3)]
	public int JoinCount;

	[ProtoMember(4)]
	public long StartTime;

	[ProtoMember(5)]
	public string HuasbandName;

	[ProtoMember(6)]
	public string WifeName;

	[ProtoMember(7)]
	public int HuasbandId;

	[ProtoMember(8)]
	public int WifeId;
}
