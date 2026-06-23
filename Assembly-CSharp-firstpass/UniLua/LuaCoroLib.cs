using System;

namespace UniLua
{
	internal class LuaCoroLib
	{
		public static int OpenLib(ILuaState lua)
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
				new NameFuncPair("create", new CSharpFunctionDelegate(LuaCoroLib.CO_Create)),
				new NameFuncPair("resume", new CSharpFunctionDelegate(LuaCoroLib.CO_Resume)),
				new NameFuncPair("running", new CSharpFunctionDelegate(LuaCoroLib.CO_Running)),
				new NameFuncPair("status", new CSharpFunctionDelegate(LuaCoroLib.CO_Status)),
				new NameFuncPair("wrap", new CSharpFunctionDelegate(LuaCoroLib.CO_Wrap)),
				new NameFuncPair("yield", new CSharpFunctionDelegate(LuaCoroLib.CO_Yield))
			};
			lua.L_NewLib(define);
			return 1;
		}

		private static int CO_Create(ILuaState lua)
		{
			lua.L_CheckType(1, LuaType.LUA_TFUNCTION);
			ILuaState to = lua.NewThread();
			lua.PushValue(1);
			lua.XMove(to, 1);
			return 1;
		}

		private static int AuxResume(ILuaState lua, ILuaState co, int narg)
		{
			if (!co.CheckStack(narg))
			{
				lua.PushString("too many arguments to resume");
				return -1;
			}
			if (co.Status == ThreadStatus.LUA_OK && co.GetTop() == 0)
			{
				lua.PushString("cannot resume dead coroutine");
				return -1;
			}
			lua.XMove(co, narg);
			ThreadStatus threadStatus = co.Resume(lua, narg);
			if (threadStatus != ThreadStatus.LUA_OK && threadStatus != ThreadStatus.LUA_YIELD)
			{
				co.XMove(lua, 1);
				return -1;
			}
			int top = co.GetTop();
			if (!lua.CheckStack(top + 1))
			{
				co.Pop(top);
				lua.PushString("too many results to resume");
				return -1;
			}
			co.XMove(lua, top);
			return top;
		}

		private static int CO_Resume(ILuaState lua)
		{
			ILuaState luaState = lua.ToThread(1);
			lua.L_ArgCheck(luaState != null, 1, "coroutine expected");
			int num = LuaCoroLib.AuxResume(lua, luaState, lua.GetTop() - 1);
			if (num < 0)
			{
				lua.PushBoolean(false);
				lua.Insert(-2);
				return 2;
			}
			lua.PushBoolean(true);
			lua.Insert(-(num + 1));
			return num + 1;
		}

		private static int CO_Running(ILuaState lua)
		{
			bool b = lua.PushThread();
			lua.PushBoolean(b);
			return 2;
		}

		private static int CO_Status(ILuaState lua)
		{
			ILuaState luaState = lua.ToThread(1);
			lua.L_ArgCheck(luaState != null, 1, "coroutine expected");
			if ((LuaState)lua == (LuaState)luaState)
			{
				lua.PushString("running");
			}
			else
			{
				ThreadStatus status = luaState.Status;
				if (status != ThreadStatus.LUA_OK)
				{
					if (status != ThreadStatus.LUA_YIELD)
					{
						lua.PushString("dead");
					}
					else
					{
						lua.PushString("suspended");
					}
				}
				else
				{
					LuaDebug ar = new LuaDebug();
					if (luaState.GetStack(0, ar))
					{
						lua.PushString("normal");
					}
					else if (luaState.GetTop() == 0)
					{
						lua.PushString("dead");
					}
					else
					{
						lua.PushString("suspended");
					}
				}
			}
			return 1;
		}

		private static int CO_AuxWrap(ILuaState lua)
		{
			ILuaState co = lua.ToThread(lua.UpvalueIndex(1));
			int num = LuaCoroLib.AuxResume(lua, co, lua.GetTop());
			if (num < 0)
			{
				if (lua.IsString(-1))
				{
					lua.L_Where(1);
					lua.Insert(-2);
					lua.Concat(2);
				}
				lua.Error();
			}
			return num;
		}

		private static int CO_Wrap(ILuaState lua)
		{
			LuaCoroLib.CO_Create(lua);
			lua.PushCSharpClosure(new CSharpFunctionDelegate(LuaCoroLib.CO_AuxWrap), 1);
			return 1;
		}

		private static int CO_Yield(ILuaState lua)
		{
			return lua.Yield(lua.GetTop());
		}

		public const string LIB_NAME = "coroutine";
	}
}
