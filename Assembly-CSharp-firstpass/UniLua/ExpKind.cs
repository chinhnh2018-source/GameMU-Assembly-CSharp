using System;

namespace UniLua
{
	public enum ExpKind
	{
		VVOID,
		VNIL,
		VTRUE,
		VFALSE,
		VK,
		VKNUM,
		VNONRELOC,
		VLOCAL,
		VUPVAL,
		VINDEXED,
		VJMP,
		VRELOCABLE,
		VCALL,
		VVARARG
	}
}
