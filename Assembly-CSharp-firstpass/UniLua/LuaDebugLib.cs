using System;

namespace UniLua
{
	internal class LuaDebugLib
	{
		public static int OpenLib(ILuaState lua)
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
				new NameFuncPair("traceback", new CSharpFunctionDelegate(LuaDebugLib.DBG_Traceback))
			};
			lua.L_NewLib(define);
			return 1;
		}

		private static int DBG_Traceback(ILuaState lua)
		{
			return 0;
		}

		public const string LIB_NAME = "debug";
	}
}
