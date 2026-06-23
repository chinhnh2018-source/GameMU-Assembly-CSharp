using System;

namespace UniLua
{
	public delegate DumpStatus LuaWriter(byte[] bytes, int start, int length);
}
