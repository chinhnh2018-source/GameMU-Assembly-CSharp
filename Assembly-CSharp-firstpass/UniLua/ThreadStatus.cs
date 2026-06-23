using System;

namespace UniLua
{
	public enum ThreadStatus
	{
		LUA_RESUME_ERROR = -1,
		LUA_OK,
		LUA_YIELD,
		LUA_ERRRUN,
		LUA_ERRSYNTAX,
		LUA_ERRMEM,
		LUA_ERRGCMM,
		LUA_ERRERR,
		LUA_ERRFILE
	}
}
