using System;

namespace UniLua
{
	internal class LuaBitLib
	{
		public static int OpenLib(ILuaState lua)
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
				new NameFuncPair("arshift", new CSharpFunctionDelegate(LuaBitLib.B_ArithShift)),
				new NameFuncPair("band", new CSharpFunctionDelegate(LuaBitLib.B_And)),
				new NameFuncPair("bnot", new CSharpFunctionDelegate(LuaBitLib.B_Not)),
				new NameFuncPair("bor", new CSharpFunctionDelegate(LuaBitLib.B_Or)),
				new NameFuncPair("bxor", new CSharpFunctionDelegate(LuaBitLib.B_Xor)),
				new NameFuncPair("btest", new CSharpFunctionDelegate(LuaBitLib.B_Test)),
				new NameFuncPair("extract", new CSharpFunctionDelegate(LuaBitLib.B_Extract)),
				new NameFuncPair("lrotate", new CSharpFunctionDelegate(LuaBitLib.B_LeftRotate)),
				new NameFuncPair("lshift", new CSharpFunctionDelegate(LuaBitLib.B_LeftShift)),
				new NameFuncPair("replace", new CSharpFunctionDelegate(LuaBitLib.B_Replace)),
				new NameFuncPair("rrotate", new CSharpFunctionDelegate(LuaBitLib.B_RightRotate)),
				new NameFuncPair("rshift", new CSharpFunctionDelegate(LuaBitLib.B_RightShift))
			};
			lua.L_NewLib(define);
			return 1;
		}

		private static uint Trim(uint x)
		{
			return x & uint.MaxValue;
		}

		private static uint Mask(int n)
		{
			return ~(4294967294U << n - 1);
		}

		private static int B_Shift(ILuaState lua, uint r, int i)
		{
			if (i < 0)
			{
				i = -i;
				r = LuaBitLib.Trim(r);
				if (i >= 32)
				{
					r = 0U;
				}
				else
				{
					r >>= i;
				}
			}
			else
			{
				if (i >= 32)
				{
					r = 0U;
				}
				else
				{
					r <<= i;
				}
				r = LuaBitLib.Trim(r);
			}
			lua.PushUnsigned(r);
			return 1;
		}

		private static int B_LeftShift(ILuaState lua)
		{
			return LuaBitLib.B_Shift(lua, lua.L_CheckUnsigned(1), lua.L_CheckInteger(2));
		}

		private static int B_RightShift(ILuaState lua)
		{
			return LuaBitLib.B_Shift(lua, lua.L_CheckUnsigned(1), -lua.L_CheckInteger(2));
		}

		private static int B_ArithShift(ILuaState lua)
		{
			uint num = lua.L_CheckUnsigned(1);
			int num2 = lua.L_CheckInteger(2);
			if (num2 < 0 || (num & 2147483648U) == 0U)
			{
				return LuaBitLib.B_Shift(lua, num, -num2);
			}
			if (num2 >= 32)
			{
				num = uint.MaxValue;
			}
			else
			{
				num = LuaBitLib.Trim(num >> num2 | ~(uint.MaxValue >> num2));
			}
			lua.PushUnsigned(num);
			return 1;
		}

		private static uint AndAux(ILuaState lua)
		{
			int top = lua.GetTop();
			uint num = uint.MaxValue;
			for (int i = 1; i <= top; i++)
			{
				num &= lua.L_CheckUnsigned(i);
			}
			return LuaBitLib.Trim(num);
		}

		private static int B_And(ILuaState lua)
		{
			uint n = LuaBitLib.AndAux(lua);
			lua.PushUnsigned(n);
			return 1;
		}

		private static int B_Not(ILuaState lua)
		{
			uint x = ~lua.L_CheckUnsigned(1);
			lua.PushUnsigned(LuaBitLib.Trim(x));
			return 1;
		}

		private static int B_Or(ILuaState lua)
		{
			int top = lua.GetTop();
			uint num = 0U;
			for (int i = 1; i <= top; i++)
			{
				num |= lua.L_CheckUnsigned(i);
			}
			lua.PushUnsigned(LuaBitLib.Trim(num));
			return 1;
		}

		private static int B_Xor(ILuaState lua)
		{
			int top = lua.GetTop();
			uint num = 0U;
			for (int i = 1; i <= top; i++)
			{
				num ^= lua.L_CheckUnsigned(i);
			}
			lua.PushUnsigned(LuaBitLib.Trim(num));
			return 1;
		}

		private static int B_Test(ILuaState lua)
		{
			uint num = LuaBitLib.AndAux(lua);
			lua.PushBoolean(num != 0U);
			return 1;
		}

		private static int FieldArgs(ILuaState lua, int farg, out int width)
		{
			int num = lua.L_CheckInteger(farg);
			int num2 = lua.L_OptInt(farg + 1, 1);
			lua.L_ArgCheck(0 <= num, farg, "field cannot be nagetive");
			lua.L_ArgCheck(0 < num2, farg + 1, "width must be positive");
			if (num + num2 > 32)
			{
				lua.L_Error("trying to access non-existent bits", new object[0]);
			}
			width = num2;
			return num;
		}

		private static int B_Extract(ILuaState lua)
		{
			uint num = lua.L_CheckUnsigned(1);
			int n;
			int num2 = LuaBitLib.FieldArgs(lua, 2, out n);
			num = (num >> num2 & LuaBitLib.Mask(n));
			lua.PushUnsigned(num);
			return 1;
		}

		private static int B_Rotate(ILuaState lua, int i)
		{
			uint num = lua.L_CheckUnsigned(1);
			i &= 31;
			num = LuaBitLib.Trim(num);
			num = (num << i | num >> 32 - i);
			lua.PushUnsigned(LuaBitLib.Trim(num));
			return 1;
		}

		private static int B_LeftRotate(ILuaState lua)
		{
			return LuaBitLib.B_Rotate(lua, lua.L_CheckInteger(2));
		}

		private static int B_RightRotate(ILuaState lua)
		{
			return LuaBitLib.B_Rotate(lua, -lua.L_CheckInteger(2));
		}

		private static int B_Replace(ILuaState lua)
		{
			uint num = lua.L_CheckUnsigned(1);
			uint num2 = lua.L_CheckUnsigned(2);
			int n;
			int num3 = LuaBitLib.FieldArgs(lua, 3, out n);
			uint num4 = LuaBitLib.Mask(n);
			num2 &= num4;
			num = ((num & ~(num4 << num3)) | num2 << num3);
			lua.PushUnsigned(num);
			return 1;
		}

		public const string LIB_NAME = "bit32";

		private const int LUA_NBITS = 32;

		private const uint ALLONES = 4294967295U;
	}
}
