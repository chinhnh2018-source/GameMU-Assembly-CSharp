using System;

namespace UniLua
{
	public enum CallStatus
	{
		CIST_NONE,
		CIST_LUA,
		CIST_HOOKED,
		CIST_REENTRY = 4,
		CIST_YIELDED = 8,
		CIST_YPCALL = 16,
		CIST_STAT = 32,
		CIST_TAIL = 64
	}
}
