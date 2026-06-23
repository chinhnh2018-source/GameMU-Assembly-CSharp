using System;
using ProtoBuf;

[ProtoContract]
public class GuanZhanRoleMiniData
{
	[ProtoMember(1)]
	public int RoleID;

	[ProtoMember(2)]
	public string Name;

	[ProtoMember(3)]
	public int Level;

	[ProtoMember(4)]
	public int ChangeLevel;

	[ProtoMember(5)]
	public int Occupation;

	[ProtoMember(6)]
	public int RoleSex;

	[ProtoMember(7)]
	public int BHZhiWu;

	[ProtoMember(8)]
	public int Param1;

	[ProtoMember(9)]
	public int Param2;
}
