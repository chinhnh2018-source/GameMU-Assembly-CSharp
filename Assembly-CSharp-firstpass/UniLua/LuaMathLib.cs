using System;

namespace UniLua
{
	internal class LuaMathLib
	{
		public static int OpenLib(ILuaState lua)
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
				new NameFuncPair("abs", new CSharpFunctionDelegate(LuaMathLib.Math_Abs)),
				new NameFuncPair("acos", new CSharpFunctionDelegate(LuaMathLib.Math_Acos)),
				new NameFuncPair("asin", new CSharpFunctionDelegate(LuaMathLib.Math_Asin)),
				new NameFuncPair("atan2", new CSharpFunctionDelegate(LuaMathLib.Math_Atan2)),
				new NameFuncPair("atan", new CSharpFunctionDelegate(LuaMathLib.Math_Atan)),
				new NameFuncPair("ceil", new CSharpFunctionDelegate(LuaMathLib.Math_Ceil)),
				new NameFuncPair("cosh", new CSharpFunctionDelegate(LuaMathLib.Math_Cosh)),
				new NameFuncPair("cos", new CSharpFunctionDelegate(LuaMathLib.Math_Cos)),
				new NameFuncPair("deg", new CSharpFunctionDelegate(LuaMathLib.Math_Deg)),
				new NameFuncPair("exp", new CSharpFunctionDelegate(LuaMathLib.Math_Exp)),
				new NameFuncPair("floor", new CSharpFunctionDelegate(LuaMathLib.Math_Floor)),
				new NameFuncPair("fmod", new CSharpFunctionDelegate(LuaMathLib.Math_Fmod)),
				new NameFuncPair("frexp", new CSharpFunctionDelegate(LuaMathLib.Math_Frexp)),
				new NameFuncPair("ldexp", new CSharpFunctionDelegate(LuaMathLib.Math_Ldexp)),
				new NameFuncPair("log10", new CSharpFunctionDelegate(LuaMathLib.Math_Log10)),
				new NameFuncPair("log", new CSharpFunctionDelegate(LuaMathLib.Math_Log)),
				new NameFuncPair("max", new CSharpFunctionDelegate(LuaMathLib.Math_Max)),
				new NameFuncPair("min", new CSharpFunctionDelegate(LuaMathLib.Math_Min)),
				new NameFuncPair("modf", new CSharpFunctionDelegate(LuaMathLib.Math_Modf)),
				new NameFuncPair("pow", new CSharpFunctionDelegate(LuaMathLib.Math_Pow)),
				new NameFuncPair("rad", new CSharpFunctionDelegate(LuaMathLib.Math_Rad)),
				new NameFuncPair("random", new CSharpFunctionDelegate(LuaMathLib.Math_Random)),
				new NameFuncPair("randomseed", new CSharpFunctionDelegate(LuaMathLib.Math_RandomSeed)),
				new NameFuncPair("sinh", new CSharpFunctionDelegate(LuaMathLib.Math_Sinh)),
				new NameFuncPair("sin", new CSharpFunctionDelegate(LuaMathLib.Math_Sin)),
				new NameFuncPair("sqrt", new CSharpFunctionDelegate(LuaMathLib.Math_Sqrt)),
				new NameFuncPair("tanh", new CSharpFunctionDelegate(LuaMathLib.Math_Tanh)),
				new NameFuncPair("tan", new CSharpFunctionDelegate(LuaMathLib.Math_Tan))
			};
			lua.L_NewLib(define);
			lua.PushNumber(3.1415926535897931);
			lua.SetField(-2, "pi");
			lua.PushNumber(double.MaxValue);
			lua.SetField(-2, "huge");
			LuaMathLib.RandObj = new Random();
			return 1;
		}

		private static int Math_Abs(ILuaState lua)
		{
			lua.PushNumber(Math.Abs(lua.L_CheckNumber(1)));
			return 1;
		}

		private static int Math_Acos(ILuaState lua)
		{
			lua.PushNumber(Math.Acos(lua.L_CheckNumber(1)));
			return 1;
		}

		private static int Math_Asin(ILuaState lua)
		{
			lua.PushNumber(Math.Asin(lua.L_CheckNumber(1)));
			return 1;
		}

		private static int Math_Atan2(ILuaState lua)
		{
			lua.PushNumber(Math.Atan2(lua.L_CheckNumber(1), lua.L_CheckNumber(2)));
			return 1;
		}

		private static int Math_Atan(ILuaState lua)
		{
			lua.PushNumber(Math.Atan(lua.L_CheckNumber(1)));
			return 1;
		}

		private static int Math_Ceil(ILuaState lua)
		{
			lua.PushNumber(Math.Ceiling(lua.L_CheckNumber(1)));
			return 1;
		}

		private static int Math_Cosh(ILuaState lua)
		{
			lua.PushNumber(Math.Cosh(lua.L_CheckNumber(1)));
			return 1;
		}

		private static int Math_Cos(ILuaState lua)
		{
			lua.PushNumber(Math.Cos(lua.L_CheckNumber(1)));
			return 1;
		}

		private static int Math_Deg(ILuaState lua)
		{
			lua.PushNumber(lua.L_CheckNumber(1) / 0.017453292519943295);
			return 1;
		}

		private static int Math_Exp(ILuaState lua)
		{
			lua.PushNumber(Math.Exp(lua.L_CheckNumber(1)));
			return 1;
		}

		private static int Math_Floor(ILuaState lua)
		{
			lua.PushNumber(Math.Floor(lua.L_CheckNumber(1)));
			return 1;
		}

		private static int Math_Fmod(ILuaState lua)
		{
			lua.PushNumber(Math.IEEERemainder(lua.L_CheckNumber(1), lua.L_CheckNumber(2)));
			return 1;
		}

		private static int Math_Frexp(ILuaState lua)
		{
			double num = lua.L_CheckNumber(1);
			long num2 = BitConverter.DoubleToInt64Bits(num);
			bool flag = num2 < 0L;
			int num3 = (int)(num2 >> 52 & 2047L);
			long num4 = num2 & 4503599627370495L;
			if (num3 == 0)
			{
				num3++;
			}
			else
			{
				num4 |= 4503599627370496L;
			}
			num3 -= 1075;
			if (num4 == 0L)
			{
				lua.PushNumber(0.0);
				lua.PushNumber(0.0);
				return 2;
			}
			while ((num4 & 1L) == 0L)
			{
				num4 >>= 1;
				num3++;
			}
			double num5 = (double)num4;
			double num6 = (double)num3;
			while (num5 >= 1.0)
			{
				num5 /= 2.0;
				num6 += 1.0;
			}
			if (flag)
			{
				num5 = -num5;
			}
			lua.PushNumber(num5);
			lua.PushNumber(num6);
			return 2;
		}

		private static int Math_Ldexp(ILuaState lua)
		{
			lua.PushNumber(lua.L_CheckNumber(1) * Math.Pow(2.0, lua.L_CheckNumber(2)));
			return 1;
		}

		private static int Math_Log10(ILuaState lua)
		{
			lua.PushNumber(Math.Log10(lua.L_CheckNumber(1)));
			return 1;
		}

		private static int Math_Log(ILuaState lua)
		{
			double num = lua.L_CheckNumber(1);
			double n;
			if (lua.IsNoneOrNil(2))
			{
				n = Math.Log(num);
			}
			else
			{
				double num2 = lua.L_CheckNumber(2);
				if (num2 == 10.0)
				{
					n = Math.Log10(num);
				}
				else
				{
					n = Math.Log(num, num2);
				}
			}
			lua.PushNumber(n);
			return 1;
		}

		private static int Math_Max(ILuaState lua)
		{
			int top = lua.GetTop();
			double num = lua.L_CheckNumber(1);
			for (int i = 2; i <= top; i++)
			{
				double num2 = lua.L_CheckNumber(i);
				if (num2 > num)
				{
					num = num2;
				}
			}
			lua.PushNumber(num);
			return 1;
		}

		private static int Math_Min(ILuaState lua)
		{
			int top = lua.GetTop();
			double num = lua.L_CheckNumber(1);
			for (int i = 2; i <= top; i++)
			{
				double num2 = lua.L_CheckNumber(i);
				if (num2 < num)
				{
					num = num2;
				}
			}
			lua.PushNumber(num);
			return 1;
		}

		private static int Math_Modf(ILuaState lua)
		{
			double num = lua.L_CheckNumber(1);
			double num2 = Math.Ceiling(num);
			lua.PushNumber(num2);
			lua.PushNumber(num - num2);
			return 2;
		}

		private static int Math_Pow(ILuaState lua)
		{
			lua.PushNumber(Math.Pow(lua.L_CheckNumber(1), lua.L_CheckNumber(2)));
			return 1;
		}

		private static int Math_Rad(ILuaState lua)
		{
			lua.PushNumber(lua.L_CheckNumber(1) * 0.017453292519943295);
			return 1;
		}

		private static int Math_Random(ILuaState lua)
		{
			double num = LuaMathLib.RandObj.NextDouble();
			switch (lua.GetTop())
			{
			case 0:
				lua.PushNumber(num);
				break;
			case 1:
			{
				double num2 = lua.L_CheckNumber(1);
				lua.L_ArgCheck(1.0 <= num2, 1, "interval is empty");
				lua.PushNumber(Math.Floor(num * num2) + 1.0);
				break;
			}
			case 2:
			{
				double num3 = lua.L_CheckNumber(1);
				double num4 = lua.L_CheckNumber(2);
				lua.L_ArgCheck(num3 <= num4, 2, "interval is empty");
				lua.PushNumber(Math.Floor(num * (num4 - num3 + 1.0)) + num3);
				break;
			}
			default:
				return lua.L_Error("wrong number of arguments", new object[0]);
			}
			return 1;
		}

		private static int Math_RandomSeed(ILuaState lua)
		{
			LuaMathLib.RandObj = new Random((int)lua.L_CheckUnsigned(1));
			LuaMathLib.RandObj.Next();
			return 0;
		}

		private static int Math_Sinh(ILuaState lua)
		{
			lua.PushNumber(Math.Sinh(lua.L_CheckNumber(1)));
			return 1;
		}

		private static int Math_Sin(ILuaState lua)
		{
			lua.PushNumber(Math.Sin(lua.L_CheckNumber(1)));
			return 1;
		}

		private static int Math_Sqrt(ILuaState lua)
		{
			lua.PushNumber(Math.Sqrt(lua.L_CheckNumber(1)));
			return 1;
		}

		private static int Math_Tanh(ILuaState lua)
		{
			lua.PushNumber(Math.Tanh(lua.L_CheckNumber(1)));
			return 1;
		}

		private static int Math_Tan(ILuaState lua)
		{
			lua.PushNumber(Math.Tan(lua.L_CheckNumber(1)));
			return 1;
		}

		public const string LIB_NAME = "math";

		private const double RADIANS_PER_DEGREE = 0.017453292519943295;

		private static Random RandObj;
	}
}
