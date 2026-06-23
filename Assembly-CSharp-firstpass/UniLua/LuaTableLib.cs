using System;
using System.Text;

namespace UniLua
{
	internal class LuaTableLib
	{
		public static int OpenLib(ILuaState lua)
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
				new NameFuncPair("concat", new CSharpFunctionDelegate(LuaTableLib.TBL_Concat)),
				new NameFuncPair("maxn", new CSharpFunctionDelegate(LuaTableLib.TBL_MaxN)),
				new NameFuncPair("insert", new CSharpFunctionDelegate(LuaTableLib.TBL_Insert)),
				new NameFuncPair("pack", new CSharpFunctionDelegate(LuaTableLib.TBL_Pack)),
				new NameFuncPair("unpack", new CSharpFunctionDelegate(LuaTableLib.TBL_Unpack)),
				new NameFuncPair("remove", new CSharpFunctionDelegate(LuaTableLib.TBL_Remove)),
				new NameFuncPair("sort", new CSharpFunctionDelegate(LuaTableLib.TBL_Sort))
			};
			lua.L_NewLib(define);
			lua.GetField(-1, "unpack");
			lua.SetGlobal("unpack");
			return 1;
		}

		private static int TBL_Concat(ILuaState lua)
		{
			string text = lua.L_OptString(2, string.Empty);
			lua.L_CheckType(1, LuaType.LUA_TTABLE);
			int i = lua.L_OptInt(3, 1);
			int num = lua.L_Opt<int>(new Func<int, int>(lua.L_CheckInteger), 4, lua.L_Len(1));
			StringBuilder stringBuilder = new StringBuilder();
			while (i < num)
			{
				lua.RawGetI(1, i);
				if (!lua.IsString(-1))
				{
					lua.L_Error("invalid value ({0}) at index {1} in table for 'concat'", new object[]
					{
						lua.L_TypeName(-1),
						i
					});
				}
				stringBuilder.Append(lua.ToString(-1));
				stringBuilder.Append(text);
				lua.Pop(1);
				i++;
			}
			if (i == num)
			{
				lua.RawGetI(1, i);
				if (!lua.IsString(-1))
				{
					lua.L_Error("invalid value ({0}) at index {1} in table for 'concat'", new object[]
					{
						lua.L_TypeName(-1),
						i
					});
				}
				stringBuilder.Append(lua.ToString(-1));
				lua.Pop(1);
			}
			lua.PushString(stringBuilder.ToString());
			return 1;
		}

		private static int TBL_MaxN(ILuaState lua)
		{
			double num = 0.0;
			lua.L_CheckType(1, LuaType.LUA_TTABLE);
			lua.PushNil();
			while (lua.Next(1))
			{
				lua.Pop(1);
				if (lua.Type(-1) == LuaType.LUA_TNUMBER)
				{
					double num2 = lua.ToNumber(-1);
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			lua.PushNumber(num);
			return 1;
		}

		private static int AuxGetN(ILuaState lua, int n)
		{
			lua.L_CheckType(n, LuaType.LUA_TTABLE);
			return lua.L_Len(n);
		}

		private static int TBL_Insert(ILuaState lua)
		{
			int num = LuaTableLib.AuxGetN(lua, 1) + 1;
			int top = lua.GetTop();
			int num2;
			if (top != 2)
			{
				if (top != 3)
				{
					return lua.L_Error("wrong number of arguments to 'insert'", new object[0]);
				}
				num2 = lua.L_CheckInteger(2);
				if (num2 > num)
				{
					num = num2;
				}
				for (int i = num; i > num2; i--)
				{
					lua.RawGetI(1, i - 1);
					lua.RawSetI(1, i);
				}
			}
			else
			{
				num2 = num;
			}
			lua.RawSetI(1, num2);
			return 0;
		}

		private static int TBL_Remove(ILuaState lua)
		{
			int num = LuaTableLib.AuxGetN(lua, 1);
			int i = lua.L_OptInt(2, num);
			if (1 > i || i > num)
			{
				return 0;
			}
			lua.RawGetI(1, i);
			while (i < num)
			{
				lua.RawGetI(1, i + 1);
				lua.RawSetI(1, i);
				i++;
			}
			lua.PushNil();
			lua.RawSetI(1, num);
			return 1;
		}

		private static int TBL_Pack(ILuaState lua)
		{
			int top = lua.GetTop();
			lua.CreateTable(top, 1);
			lua.PushInteger(top);
			lua.SetField(-2, "n");
			if (top > 0)
			{
				lua.PushValue(1);
				lua.RawSetI(-2, 1);
				lua.Replace(1);
				for (int i = top; i >= 2; i--)
				{
					lua.RawSetI(1, i);
				}
			}
			return 1;
		}

		private static int TBL_Unpack(ILuaState lua)
		{
			lua.L_CheckType(1, LuaType.LUA_TTABLE);
			int num = lua.L_OptInt(2, 1);
			int num2 = lua.L_OptInt(3, lua.L_Len(1));
			if (num > num2)
			{
				return 0;
			}
			int num3 = num2 - num + 1;
			if (num3 <= 0 || !lua.CheckStack(num3))
			{
				return lua.L_Error("too many results to unpack", new object[0]);
			}
			lua.RawGetI(1, num);
			while (num++ < num2)
			{
				lua.RawGetI(1, num);
			}
			return num3;
		}

		private static void Set2(ILuaState lua, int i, int j)
		{
			lua.RawSetI(1, i);
			lua.RawSetI(1, j);
		}

		private static bool SortComp(ILuaState lua, int a, int b)
		{
			if (!lua.IsNil(2))
			{
				lua.PushValue(2);
				lua.PushValue(a - 1);
				lua.PushValue(b - 2);
				lua.Call(2, 1);
				bool result = lua.ToBoolean(-1);
				lua.Pop(1);
				return result;
			}
			return lua.Compare(a, b, LuaEq.LUA_OPLT);
		}

		private static void AuxSort(ILuaState lua, int l, int u)
		{
			while (l < u)
			{
				lua.RawGetI(1, l);
				lua.RawGetI(1, u);
				if (LuaTableLib.SortComp(lua, -1, -2))
				{
					LuaTableLib.Set2(lua, l, u);
				}
				else
				{
					lua.Pop(2);
				}
				if (u - l == 1)
				{
					break;
				}
				int num = (l + u) / 2;
				lua.RawGetI(1, num);
				lua.RawGetI(1, l);
				if (LuaTableLib.SortComp(lua, -2, -1))
				{
					LuaTableLib.Set2(lua, num, l);
				}
				else
				{
					lua.Pop(1);
					lua.RawGetI(1, u);
					if (LuaTableLib.SortComp(lua, -1, -2))
					{
						LuaTableLib.Set2(lua, num, u);
					}
					else
					{
						lua.Pop(2);
					}
				}
				if (u - l == 2)
				{
					break;
				}
				lua.RawGetI(1, num);
				lua.PushValue(-1);
				lua.RawGetI(1, u - 1);
				LuaTableLib.Set2(lua, num, u - 1);
				num = l;
				int num2 = u - 1;
				for (;;)
				{
					lua.RawGetI(1, ++num);
					while (LuaTableLib.SortComp(lua, -1, -2))
					{
						if (num >= u)
						{
							lua.L_Error("invalid order function for sorting", new object[0]);
						}
						lua.Pop(1);
						lua.RawGetI(1, ++num);
					}
					lua.RawGetI(1, --num2);
					while (LuaTableLib.SortComp(lua, -3, -1))
					{
						if (num2 <= l)
						{
							lua.L_Error("invalid order function for sorting", new object[0]);
						}
						lua.Pop(1);
						lua.RawGetI(1, --num2);
					}
					if (num2 < num)
					{
						break;
					}
					LuaTableLib.Set2(lua, num, num2);
				}
				lua.Pop(3);
				lua.RawGetI(1, u - 1);
				lua.RawGetI(1, num);
				LuaTableLib.Set2(lua, u - 1, num);
				if (num - l < u - num)
				{
					num2 = l;
					num--;
					l = num + 2;
				}
				else
				{
					num2 = num + 1;
					num = u;
					u = num2 - 2;
				}
				LuaTableLib.AuxSort(lua, num2, num);
			}
		}

		private static int TBL_Sort(ILuaState lua)
		{
			int u = LuaTableLib.AuxGetN(lua, 1);
			lua.L_CheckStack(40, string.Empty);
			if (!lua.IsNoneOrNil(2))
			{
				lua.L_CheckType(2, LuaType.LUA_TFUNCTION);
			}
			lua.SetTop(2);
			LuaTableLib.AuxSort(lua, 1, u);
			return 0;
		}

		public const string LIB_NAME = "table";
	}
}
