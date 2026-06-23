using System;

namespace UniLua
{
	public static class ExpKindUtl
	{
		public static bool VKIsVar(ExpKind k)
		{
			return ExpKind.VLOCAL <= k && k <= ExpKind.VINDEXED;
		}

		public static bool VKIsInReg(ExpKind k)
		{
			return k == ExpKind.VNONRELOC || k == ExpKind.VLOCAL;
		}
	}
}
