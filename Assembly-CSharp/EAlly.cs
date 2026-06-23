using System;

public enum EAlly
{
	EAddUnion = -18,
	EFail,
	EServer,
	ENoTargetUnion = -13,
	EAllyRequestMax,
	EAllyMax,
	EMore,
	EIsAlly,
	EMoney,
	ENotLeader,
	EIsSelf,
	EUnionLevel,
	EUnionJoin,
	EName,
	EZoneID,
	EAllyRequest,
	AllyRequestSucc = 1,
	AllyFailure = 10,
	AllyRefuse,
	AllyAgree,
	AllyRefuseOther = 20,
	AllyAgreeOther,
	EAllyCancel = 30,
	AllyCancelSucc,
	EAllyRemove = 40,
	AllyRemoveSucc,
	AllyRemoveSuccOther,
	Succ = 50,
	Default = 0
}
