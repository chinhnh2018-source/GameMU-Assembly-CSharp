using System;
using System.Collections.Generic;
using System.Text;
using UniLua.Tools;

namespace UniLua
{
	internal static class LuaBaseLib
	{
		internal static int OpenLib(ILuaState lua)
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
				new NameFuncPair("assert", new CSharpFunctionDelegate(LuaBaseLib.B_Assert)),
				new NameFuncPair("collectgarbage", new CSharpFunctionDelegate(LuaBaseLib.B_CollectGarbage)),
				new NameFuncPair("dofile", new CSharpFunctionDelegate(LuaBaseLib.B_DoFile)),
				new NameFuncPair("error", new CSharpFunctionDelegate(LuaBaseLib.B_Error)),
				new NameFuncPair("ipairs", new CSharpFunctionDelegate(LuaBaseLib.B_Ipairs)),
				new NameFuncPair("loadfile", new CSharpFunctionDelegate(LuaBaseLib.B_LoadFile)),
				new NameFuncPair("load", new CSharpFunctionDelegate(LuaBaseLib.B_Load)),
				new NameFuncPair("loadstring", new CSharpFunctionDelegate(LuaBaseLib.B_Load)),
				new NameFuncPair("next", new CSharpFunctionDelegate(LuaBaseLib.B_Next)),
				new NameFuncPair("pairs", new CSharpFunctionDelegate(LuaBaseLib.B_Pairs)),
				new NameFuncPair("pcall", new CSharpFunctionDelegate(LuaBaseLib.B_PCall)),
				new NameFuncPair("print", new CSharpFunctionDelegate(LuaBaseLib.B_Print)),
				new NameFuncPair("rawequal", new CSharpFunctionDelegate(LuaBaseLib.B_RawEqual)),
				new NameFuncPair("rawlen", new CSharpFunctionDelegate(LuaBaseLib.B_RawLen)),
				new NameFuncPair("rawget", new CSharpFunctionDelegate(LuaBaseLib.B_RawGet)),
				new NameFuncPair("rawset", new CSharpFunctionDelegate(LuaBaseLib.B_RawSet)),
				new NameFuncPair("select", new CSharpFunctionDelegate(LuaBaseLib.B_Select)),
				new NameFuncPair("getmetatable", new CSharpFunctionDelegate(LuaBaseLib.B_GetMetaTable)),
				new NameFuncPair("setmetatable", new CSharpFunctionDelegate(LuaBaseLib.B_SetMetaTable)),
				new NameFuncPair("tonumber", new CSharpFunctionDelegate(LuaBaseLib.B_ToNumber)),
				new NameFuncPair("tostring", new CSharpFunctionDelegate(LuaBaseLib.B_ToString)),
				new NameFuncPair("type", new CSharpFunctionDelegate(LuaBaseLib.B_Type)),
				new NameFuncPair("xpcall", new CSharpFunctionDelegate(LuaBaseLib.B_XPCall))
			};
			lua.PushGlobalTable();
			lua.PushGlobalTable();
			lua.SetField(-2, "_G");
			lua.L_SetFuncs(define, 0);
			lua.PushString("Lua 5.2");
			lua.SetField(-2, "_VERSION");
			return 1;
		}

		public static int B_Assert(ILuaState lua)
		{
			if (!lua.ToBoolean(1))
			{
				return lua.L_Error("{0}", new object[]
				{
					lua.L_OptString(2, "assertion failed!")
				});
			}
			return lua.GetTop();
		}

		public static int B_CollectGarbage(ILuaState lua)
		{
			string text = lua.L_OptString(1, "collect");
			string text2 = text;
			if (text2 != null)
			{
				if (LuaBaseLib.<>f__switch$map9 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
					dictionary.Add("count", 0);
					dictionary.Add("step", 1);
					dictionary.Add("isrunning", 1);
					LuaBaseLib.<>f__switch$map9 = dictionary;
				}
				int num;
				if (LuaBaseLib.<>f__switch$map9.TryGetValue(text2, ref num))
				{
					if (num == 0)
					{
						lua.PushNumber(0.0);
						lua.PushNumber(0.0);
						return 2;
					}
					if (num == 1)
					{
						lua.PushBoolean(true);
						return 1;
					}
				}
			}
			lua.PushInteger(0);
			return 1;
		}

		private static int DoFileContinuation(ILuaState lua)
		{
			return lua.GetTop() - 1;
		}

		public static int B_DoFile(ILuaState lua)
		{
			string filename = lua.L_OptString(1, null);
			lua.SetTop(1);
			if (lua.L_LoadFile(filename) != ThreadStatus.LUA_OK)
			{
				lua.Error();
			}
			lua.CallK(0, -1, 0, new CSharpFunctionDelegate(LuaBaseLib.DoFileContinuation));
			return LuaBaseLib.DoFileContinuation(lua);
		}

		public static int B_Error(ILuaState lua)
		{
			int num = lua.L_OptInt(2, 1);
			lua.SetTop(1);
			if (lua.IsString(1) && num > 0)
			{
				lua.L_Where(num);
				lua.PushValue(1);
				lua.Concat(2);
			}
			return lua.Error();
		}

		private static int LoadAux(ILuaState lua, ThreadStatus status, int envidx)
		{
			if (status == ThreadStatus.LUA_OK)
			{
				if (envidx != 0)
				{
					lua.PushValue(envidx);
					if (lua.SetUpvalue(-2, 1) == null)
					{
						lua.Pop(1);
					}
				}
				return 1;
			}
			lua.PushNil();
			lua.Insert(-2);
			return 2;
		}

		public static int B_LoadFile(ILuaState lua)
		{
			string filename = lua.L_OptString(1, null);
			string mode = lua.L_OptString(2, null);
			int envidx = lua.IsNone(3) ? 0 : 3;
			ThreadStatus status = lua.L_LoadFileX(filename, mode);
			return LuaBaseLib.LoadAux(lua, status, envidx);
		}

		public static int B_Load(ILuaState lua)
		{
			string text = lua.ToString(1);
			string mode = lua.L_OptString(3, "bt");
			int envidx = lua.IsNone(4) ? 0 : 4;
			if (text != null)
			{
				string name = lua.L_OptString(2, text);
				ThreadStatus status = lua.L_LoadBufferX(text, name, mode);
				return LuaBaseLib.LoadAux(lua, status, envidx);
			}
			throw new NotImplementedException();
		}

		private static int FinishPCall(ILuaState lua, bool status)
		{
			if (!lua.CheckStack(1))
			{
				lua.SetTop(0);
				lua.PushBoolean(false);
				lua.PushString("stack overflow");
				return 2;
			}
			lua.PushBoolean(status);
			lua.Replace(1);
			return lua.GetTop();
		}

		private static int PCallContinuation(ILuaState lua)
		{
			int num;
			ThreadStatus context = lua.GetContext(out num);
			return LuaBaseLib.FinishPCall(lua, context == ThreadStatus.LUA_YIELD);
		}

		public static int B_PCall(ILuaState lua)
		{
			lua.L_CheckAny(1);
			lua.PushNil();
			lua.Insert(1);
			ThreadStatus threadStatus = lua.PCallK(lua.GetTop() - 2, -1, 0, 0, LuaBaseLib.DG_PCallContinuation);
			return LuaBaseLib.FinishPCall(lua, threadStatus == ThreadStatus.LUA_OK);
		}

		public static int B_XPCall(ILuaState lua)
		{
			int top = lua.GetTop();
			lua.L_ArgCheck(top >= 2, 2, "value expected");
			lua.PushValue(1);
			lua.Copy(2, 1);
			lua.Replace(2);
			ThreadStatus threadStatus = lua.PCallK(top - 2, -1, 1, 0, LuaBaseLib.DG_PCallContinuation);
			return LuaBaseLib.FinishPCall(lua, threadStatus == ThreadStatus.LUA_OK);
		}

		public static int B_RawEqual(ILuaState lua)
		{
			lua.L_CheckAny(1);
			lua.L_CheckAny(2);
			lua.PushBoolean(lua.RawEqual(1, 2));
			return 1;
		}

		public static int B_RawLen(ILuaState lua)
		{
			LuaType luaType = lua.Type(1);
			lua.L_ArgCheck(luaType == LuaType.LUA_TTABLE || luaType == LuaType.LUA_TSTRING, 1, "table or string expected");
			lua.PushInteger(lua.RawLen(1));
			return 1;
		}

		public static int B_RawGet(ILuaState lua)
		{
			lua.L_CheckType(1, LuaType.LUA_TTABLE);
			lua.L_CheckAny(2);
			lua.SetTop(2);
			lua.RawGet(1);
			return 1;
		}

		public static int B_RawSet(ILuaState lua)
		{
			lua.L_CheckType(1, LuaType.LUA_TTABLE);
			lua.L_CheckAny(2);
			lua.L_CheckAny(3);
			lua.SetTop(3);
			lua.RawSet(1);
			return 1;
		}

		public static int B_Select(ILuaState lua)
		{
			int top = lua.GetTop();
			if (lua.Type(1) == LuaType.LUA_TSTRING && lua.ToString(1).get_Chars(0) == '#')
			{
				lua.PushInteger(top - 1);
				return 1;
			}
			int num = lua.L_CheckInteger(1);
			if (num < 0)
			{
				num = top + num;
			}
			else if (num > top)
			{
				num = top;
			}
			lua.L_ArgCheck(1 <= num, 1, "index out of range");
			return top - num;
		}

		public static int B_GetMetaTable(ILuaState lua)
		{
			lua.L_CheckAny(1);
			if (!lua.GetMetaTable(1))
			{
				lua.PushNil();
				return 1;
			}
			lua.L_GetMetaField(1, "__metatable");
			return 1;
		}

		public static int B_SetMetaTable(ILuaState lua)
		{
			LuaType luaType = lua.Type(2);
			lua.L_CheckType(1, LuaType.LUA_TTABLE);
			lua.L_ArgCheck(luaType == LuaType.LUA_TNIL || luaType == LuaType.LUA_TTABLE, 2, "nil or table expected");
			if (lua.L_GetMetaField(1, "__metatable"))
			{
				return lua.L_Error("cannot change a protected metatable", new object[0]);
			}
			lua.SetTop(2);
			lua.SetMetaTable(1);
			return 1;
		}

		public static int B_ToNumber(ILuaState lua)
		{
			LuaType luaType = lua.Type(2);
			if (luaType == LuaType.LUA_TNONE || luaType == LuaType.LUA_TNIL)
			{
				bool flag;
				double n = lua.ToNumberX(1, out flag);
				if (flag)
				{
					lua.PushNumber(n);
					return 1;
				}
				lua.L_CheckAny(1);
			}
			else
			{
				string text = lua.L_CheckString(1);
				int num = lua.L_CheckInteger(2);
				bool flag2 = false;
				lua.L_ArgCheck(2 <= num && num <= 36, 2, "base out of range");
				text = text.Trim(new char[]
				{
					' ',
					'\f',
					'\n',
					'\r',
					'\t',
					'\v'
				});
				text += '\0';
				int num2 = 0;
				if (text.get_Chars(num2) == '-')
				{
					num2++;
					flag2 = true;
				}
				else if (text.get_Chars(num2) == '+')
				{
					num2++;
				}
				if (char.IsLetterOrDigit(text, num2))
				{
					double num3 = 0.0;
					do
					{
						int num4;
						if (char.IsDigit(text, num2))
						{
							num4 = int.Parse(text.get_Chars(num2).ToString());
						}
						else
						{
							num4 = (int)(char.ToUpper(text.get_Chars(num2)) - 'A' + '\n');
						}
						if (num4 >= num)
						{
							break;
						}
						num3 = num3 * (double)num + (double)num4;
						num2++;
					}
					while (char.IsLetterOrDigit(text, num2));
					if (num2 == text.Length - 1)
					{
						lua.PushNumber((!flag2) ? num3 : (-num3));
						return 1;
					}
				}
			}
			lua.PushNil();
			return 1;
		}

		public static int B_Type(ILuaState lua)
		{
			LuaType t = lua.Type(1);
			string s = lua.TypeName(t);
			lua.PushString(s);
			return 1;
		}

		private static int PairsMeta(ILuaState lua, string method, bool isZero, CSharpFunctionDelegate iter)
		{
			if (!lua.L_GetMetaField(1, method))
			{
				lua.L_CheckType(1, LuaType.LUA_TTABLE);
				lua.PushCSharpFunction(iter);
				lua.PushValue(1);
				if (isZero)
				{
					lua.PushInteger(0);
				}
				else
				{
					lua.PushNil();
				}
			}
			else
			{
				lua.PushValue(1);
				lua.Call(1, 3);
			}
			return 3;
		}

		public static int B_Next(ILuaState lua)
		{
			lua.SetTop(2);
			if (lua.Next(1))
			{
				return 2;
			}
			lua.PushNil();
			return 1;
		}

		public static int B_Pairs(ILuaState lua)
		{
			return LuaBaseLib.PairsMeta(lua, "__pairs", false, LuaBaseLib.DG_B_Next);
		}

		private static int IpairsAux(ILuaState lua)
		{
			int num = lua.ToInteger(2);
			num++;
			lua.PushInteger(num);
			lua.RawGetI(1, num);
			return (!lua.IsNil(-1)) ? 2 : 1;
		}

		public static int B_Ipairs(ILuaState lua)
		{
			return LuaBaseLib.PairsMeta(lua, "__ipairs", true, LuaBaseLib.DG_IpairsAux);
		}

		public static int B_Print(ILuaState lua)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int top = lua.GetTop();
			lua.GetGlobal("tostring");
			for (int i = 1; i <= top; i++)
			{
				lua.PushValue(-1);
				lua.PushValue(i);
				lua.Call(1, 1);
				string text = lua.ToString(-1);
				if (text == null)
				{
					return lua.L_Error("'tostring' must return a string to 'print'", new object[0]);
				}
				if (i > 1)
				{
					stringBuilder.Append("\t");
				}
				stringBuilder.Append(text);
				lua.Pop(1);
			}
			ULDebug.Log.Invoke(stringBuilder.ToString());
			return 0;
		}

		public static int B_ToString(ILuaState lua)
		{
			lua.L_CheckAny(1);
			lua.L_ToString(1);
			return 1;
		}

		private static CSharpFunctionDelegate DG_PCallContinuation = new CSharpFunctionDelegate(LuaBaseLib.PCallContinuation);

		private static CSharpFunctionDelegate DG_B_Next = new CSharpFunctionDelegate(LuaBaseLib.B_Next);

		private static CSharpFunctionDelegate DG_IpairsAux = new CSharpFunctionDelegate(LuaBaseLib.IpairsAux);
	}
}
