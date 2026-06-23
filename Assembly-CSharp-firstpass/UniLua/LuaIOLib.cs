using System;

namespace UniLua
{
	internal class LuaIOLib
	{
		public static int OpenLib(ILuaState lua)
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
				new NameFuncPair("close", new CSharpFunctionDelegate(LuaIOLib.IO_Close)),
				new NameFuncPair("flush", new CSharpFunctionDelegate(LuaIOLib.IO_Flush)),
				new NameFuncPair("input", new CSharpFunctionDelegate(LuaIOLib.IO_Input)),
				new NameFuncPair("lines", new CSharpFunctionDelegate(LuaIOLib.IO_Lines)),
				new NameFuncPair("open", new CSharpFunctionDelegate(LuaIOLib.IO_Open)),
				new NameFuncPair("output", new CSharpFunctionDelegate(LuaIOLib.IO_Output)),
				new NameFuncPair("popen", new CSharpFunctionDelegate(LuaIOLib.IO_Popen)),
				new NameFuncPair("read", new CSharpFunctionDelegate(LuaIOLib.IO_Read)),
				new NameFuncPair("tmpfile", new CSharpFunctionDelegate(LuaIOLib.IO_Tmpfile)),
				new NameFuncPair("type", new CSharpFunctionDelegate(LuaIOLib.IO_Type)),
				new NameFuncPair("write", new CSharpFunctionDelegate(LuaIOLib.IO_Write))
			};
			lua.L_NewLib(define);
			return 1;
		}

		private static int IO_Close(ILuaState lua)
		{
			return 0;
		}

		private static int IO_Flush(ILuaState lua)
		{
			return 0;
		}

		private static int IO_Input(ILuaState lua)
		{
			return 0;
		}

		private static int IO_Lines(ILuaState lua)
		{
			return 0;
		}

		private static int IO_Open(ILuaState lua)
		{
			return 0;
		}

		private static int IO_Output(ILuaState lua)
		{
			return 0;
		}

		private static int IO_Popen(ILuaState lua)
		{
			return 0;
		}

		private static int IO_Read(ILuaState lua)
		{
			return 0;
		}

		private static int IO_Tmpfile(ILuaState lua)
		{
			return 0;
		}

		private static int IO_Type(ILuaState lua)
		{
			return 0;
		}

		private static int IO_Write(ILuaState lua)
		{
			return 0;
		}

		public const string LIB_NAME = "io";
	}
}
