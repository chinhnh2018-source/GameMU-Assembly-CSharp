using System;
using System.Diagnostics;

namespace UniLua
{
	internal class LuaOSLib
	{
		public static int OpenLib(ILuaState lua)
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
				new NameFuncPair("clock", new CSharpFunctionDelegate(LuaOSLib.OS_Clock))
			};
			lua.L_NewLib(define);
			return 1;
		}

		private static int OS_Clock(ILuaState lua)
		{
			lua.PushNumber(Process.GetCurrentProcess().TotalProcessorTime.TotalSeconds);
			return 1;
		}

		public const string LIB_NAME = "os";
	}
}
