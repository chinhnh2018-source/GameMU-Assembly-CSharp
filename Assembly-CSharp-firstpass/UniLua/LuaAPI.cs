using System;

namespace UniLua
{
	public static class LuaAPI
	{
		public static ILuaState NewState()
		{
			return new LuaState(null);
		}
	}
}
