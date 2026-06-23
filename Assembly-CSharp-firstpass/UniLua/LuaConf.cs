using System;
using System.IO;

namespace UniLua
{
	public static class LuaConf
	{
		public static string LUA_DIRSEP
		{
			get
			{
				char directorySeparatorChar = Path.DirectorySeparatorChar;
				return directorySeparatorChar.ToString();
			}
		}

		public const int LUAI_BITSINT = 32;

		public const int LUAI_MAXSTACK = 1000000;

		public const int LUAI_FIRSTPSEUDOIDX = -1001000;

		public const string LUA_SIGNATURE = "\u001bLua";
	}
}
