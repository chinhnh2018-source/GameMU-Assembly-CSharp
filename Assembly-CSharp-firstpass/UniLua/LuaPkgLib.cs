using System;
using System.Text;

namespace UniLua
{
	internal class LuaPkgLib
	{
		public static int OpenLib(ILuaState lua)
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
				new NameFuncPair("loadlib", new CSharpFunctionDelegate(LuaPkgLib.PKG_LoadLib)),
				new NameFuncPair("searchpath", new CSharpFunctionDelegate(LuaPkgLib.PKG_SearchPath)),
				new NameFuncPair("seeall", new CSharpFunctionDelegate(LuaPkgLib.PKG_SeeAll))
			};
			lua.L_NewLib(define);
			LuaPkgLib.CreateSearchersTable(lua);
			lua.PushValue(-1);
			lua.SetField(-3, "loaders");
			lua.SetField(-2, "searchers");
			LuaPkgLib.SetPath(lua, "path", "LUA_PATH_5_2", "LUA_PATH", "?.lua;");
			LuaPkgLib.SetPath(lua, "cpath", "LUA_CPATH_5_2", "LUA_CPATH", "?.dll;loadall.dll;");
			lua.PushString(string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n", new object[]
			{
				LuaConf.LUA_DIRSEP,
				";",
				"?",
				"!",
				"-"
			}));
			lua.SetField(-2, "config");
			lua.L_GetSubTable(-1001000, "_LOADED");
			lua.SetField(-2, "loaded");
			lua.L_GetSubTable(-1001000, "_PRELOAD");
			lua.SetField(-2, "preload");
			lua.PushGlobalTable();
			lua.PushValue(-2);
			NameFuncPair[] define2 = new NameFuncPair[]
			{
				new NameFuncPair("module", new CSharpFunctionDelegate(LuaPkgLib.LL_Module)),
				new NameFuncPair("require", new CSharpFunctionDelegate(LuaPkgLib.LL_Require))
			};
			lua.L_SetFuncs(define2, 1);
			lua.Pop(1);
			return 1;
		}

		private static void CreateSearchersTable(ILuaState lua)
		{
			CSharpFunctionDelegate[] array = new CSharpFunctionDelegate[]
			{
				new CSharpFunctionDelegate(LuaPkgLib.SearcherPreload),
				new CSharpFunctionDelegate(LuaPkgLib.SearcherLua)
			};
			lua.CreateTable(array.Length, 0);
			for (int i = 0; i < array.Length; i++)
			{
				lua.PushValue(-2);
				lua.PushCSharpClosure(array[i], 1);
				lua.RawSetI(-2, i + 1);
			}
		}

		private static void SetPath(ILuaState lua, string fieldName, string envName1, string envName2, string def)
		{
			lua.PushString(def);
			LuaPkgLib.SetProgDir(lua);
			lua.SetField(-2, fieldName);
		}

		private static bool noEnv(ILuaState lua)
		{
			lua.GetField(-1001000, "LUA_NOENV");
			bool result = lua.ToBoolean(-1);
			lua.Pop(1);
			return result;
		}

		private static void SetProgDir(ILuaState lua)
		{
		}

		private static int SearcherPreload(ILuaState lua)
		{
			string text = lua.L_CheckString(1);
			lua.GetField(-1001000, "_PRELOAD");
			lua.GetField(-1, text);
			if (lua.IsNil(-1))
			{
				lua.PushString(string.Format("\n\tno field package.preload['{0}']", text));
			}
			return 1;
		}

		private static bool Readable(string filename)
		{
			return LuaFile.Readable(filename);
		}

		private static bool PushNextTemplate(ILuaState lua, string path, ref int pos)
		{
			while (pos < path.Length && path.get_Chars(pos) == ";".get_Chars(0))
			{
				pos++;
			}
			if (pos >= path.Length)
			{
				return false;
			}
			int num = pos + 1;
			while (num < path.Length && path.get_Chars(num) != ";".get_Chars(0))
			{
				num++;
			}
			string s = path.Substring(pos, num - pos);
			lua.PushString(s);
			pos = num;
			return true;
		}

		private static string SearchPath(ILuaState lua, string name, string path, string sep, string dirsep)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(sep))
			{
				name = name.Replace(sep, dirsep);
			}
			int num = 0;
			while (LuaPkgLib.PushNextTemplate(lua, path, ref num))
			{
				string text = lua.ToString(-1);
				string text2 = text.Replace("?", name);
				lua.Remove(-1);
				if (LuaPkgLib.Readable(text2))
				{
					return text2;
				}
				lua.PushString(string.Format("\n\tno file '{0}'", text2));
				lua.Remove(-2);
				stringBuilder.Append(lua.ToString(-1));
			}
			lua.PushString(stringBuilder.ToString());
			return null;
		}

		private static string FindFile(ILuaState lua, string name, string pname, string dirsep)
		{
			lua.GetField(lua.UpvalueIndex(1), pname);
			string text = lua.ToString(-1);
			if (text == null)
			{
				lua.L_Error("'package.{0}' must be a string", new object[]
				{
					pname
				});
			}
			return LuaPkgLib.SearchPath(lua, name, text, ".", dirsep);
		}

		private static int CheckLoad(ILuaState lua, bool stat, string filename)
		{
			if (stat)
			{
				lua.PushString(filename);
				return 2;
			}
			return lua.L_Error("error loading module '{0}' from file '{1}':\n\t{2}", new object[]
			{
				lua.ToString(1),
				filename,
				lua.ToString(-1)
			});
		}

		private static int SearcherLua(ILuaState lua)
		{
			string name = lua.L_CheckString(1);
			string text = LuaPkgLib.FindFile(lua, name, "path", LuaPkgLib.LUA_LSUBSEP);
			if (text == null)
			{
				return 1;
			}
			return LuaPkgLib.CheckLoad(lua, lua.L_LoadFile(text) == ThreadStatus.LUA_OK, text);
		}

		private static int LL_Module(ILuaState lua)
		{
			return 0;
		}

		private static void FindLoader(ILuaState lua, string name)
		{
			lua.GetField(lua.UpvalueIndex(1), "searchers");
			if (!lua.IsTable(3))
			{
				lua.L_Error("'package.searchers' must be a table", new object[0]);
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 1;
			for (;;)
			{
				lua.RawGetI(3, num);
				if (lua.IsNil(-1))
				{
					break;
				}
				lua.PushString(name);
				lua.Call(1, 2);
				if (lua.IsFunction(-2))
				{
					return;
				}
				if (lua.IsString(-2))
				{
					lua.Pop(1);
					stringBuilder.Append(lua.ToString(-1));
				}
				else
				{
					lua.Pop(2);
				}
				num++;
			}
			lua.Pop(1);
			lua.PushString(stringBuilder.ToString());
			lua.L_Error("module '{0}' not found:{1}", new object[]
			{
				name,
				lua.ToString(-1)
			});
		}

		private static int LL_Require(ILuaState lua)
		{
			string text = lua.L_CheckString(1);
			lua.SetTop(1);
			lua.GetField(-1001000, "_LOADED");
			lua.GetField(2, text);
			if (lua.ToBoolean(-1))
			{
				return 1;
			}
			lua.Pop(1);
			LuaPkgLib.FindLoader(lua, text);
			lua.PushString(text);
			lua.Insert(-2);
			lua.Call(2, 1);
			if (!lua.IsNil(-1))
			{
				lua.SetField(2, text);
			}
			lua.GetField(2, text);
			if (lua.IsNil(-1))
			{
				lua.PushBoolean(true);
				lua.PushValue(-1);
				lua.SetField(2, text);
			}
			return 1;
		}

		private static int PKG_LoadLib(ILuaState lua)
		{
			return 0;
		}

		private static int PKG_SearchPath(ILuaState lua)
		{
			return 0;
		}

		private static int PKG_SeeAll(ILuaState lua)
		{
			return 0;
		}

		public const string LIB_NAME = "package";

		private const string CLIBS = "_CLIBS";

		private const string LUA_PATH = "LUA_PATH";

		private const string LUA_CPATH = "LUA_CPATH";

		private const string LUA_PATHSUFFIX = "_5_2";

		private const string LUA_PATHVERSION = "LUA_PATH_5_2";

		private const string LUA_CPATHVERSION = "LUA_CPATH_5_2";

		private const string LUA_PATH_DEFAULT = "?.lua;";

		private const string LUA_CPATH_DEFAULT = "?.dll;loadall.dll;";

		private const string LUA_PATH_SEP = ";";

		private const string LUA_PATH_MARK = "?";

		private const string LUA_EXEC_DIR = "!";

		private const string LUA_IGMARK = "-";

		private const string AUXMARK = "\u0001";

		private static readonly string LUA_LSUBSEP = LuaConf.LUA_DIRSEP;
	}
}
