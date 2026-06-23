using System;

namespace UniLua
{
	internal static class Utl
	{
		private static void Throw(params string[] msgs)
		{
			throw new Exception(string.Join(string.Empty, msgs));
		}

		public static void Assert(bool condition)
		{
			if (!condition)
			{
				Utl.Throw(new string[]
				{
					"assert failed!"
				});
			}
		}

		public static void Assert(bool condition, string message)
		{
			if (!condition)
			{
				Utl.Throw(new string[]
				{
					"assert failed! ",
					message
				});
			}
		}

		public static void Assert(bool condition, string message, string detailMessage)
		{
			if (!condition)
			{
				Utl.Throw(new string[]
				{
					"assert failed! ",
					message,
					"\n",
					detailMessage
				});
			}
		}

		public static void ApiCheck(bool condition, string message)
		{
			Utl.Assert(condition, message);
		}

		public static void ApiCheckNumElems(LuaState lua, int n)
		{
			Utl.Assert(n < lua.Top.Index - lua.CI.FuncIndex, "not enough elements in the stack");
		}

		public static void InvalidIndex()
		{
			Utl.Assert(false, "invalid index");
		}

		private static bool IsNegative(string s, ref int pos)
		{
			if (pos >= s.Length)
			{
				return false;
			}
			char c = s.get_Chars(pos);
			if (c == '-')
			{
				pos++;
				return true;
			}
			if (c == '+')
			{
				pos++;
			}
			return false;
		}

		private static bool IsXDigit(char c)
		{
			return char.IsDigit(c) || ('a' <= c && c <= 'f') || ('A' <= c && c <= 'F');
		}

		private static double ReadHexa(string s, ref int pos, double r, out int count)
		{
			count = 0;
			while (pos < s.Length && Utl.IsXDigit(s.get_Chars(pos)))
			{
				r = r * 16.0 + (double)int.Parse(s.get_Chars(pos).ToString(), 515);
				pos++;
				count++;
			}
			return r;
		}

		private static double ReadDecimal(string s, ref int pos, double r, out int count)
		{
			count = 0;
			while (pos < s.Length && char.IsDigit(s.get_Chars(pos)))
			{
				r = r * 10.0 + (double)int.Parse(s.get_Chars(pos).ToString());
				pos++;
				count++;
			}
			return r;
		}

		public static double StrX2Number(string s, ref int curpos)
		{
			int num = curpos;
			while (num < s.Length && char.IsWhiteSpace(s.get_Chars(num)))
			{
				num++;
			}
			bool flag = Utl.IsNegative(s, ref num);
			if (num >= s.Length || s.get_Chars(num) != '0' || (s.get_Chars(num + 1) != 'x' && s.get_Chars(num + 1) != 'X'))
			{
				return 0.0;
			}
			num += 2;
			double num2 = 0.0;
			int num3 = 0;
			int num4 = 0;
			num2 = Utl.ReadHexa(s, ref num, num2, out num3);
			if (num < s.Length && s.get_Chars(num) == '.')
			{
				num++;
				num2 = Utl.ReadHexa(s, ref num, num2, out num4);
			}
			if (num3 == 0 && num4 == 0)
			{
				return 0.0;
			}
			num4 *= -4;
			curpos = num;
			if (num < s.Length && (s.get_Chars(num) == 'p' || s.get_Chars(num) == 'P'))
			{
				num++;
				bool flag2 = Utl.IsNegative(s, ref num);
				if (num >= s.Length || !char.IsDigit(s.get_Chars(num)))
				{
					goto IL_194;
				}
				int num5 = 0;
				while (num < s.Length && char.IsDigit(s.get_Chars(num)))
				{
					num5 = num5 * 10 + int.Parse(s.get_Chars(num).ToString());
					num++;
				}
				if (flag2)
				{
					num5 = -num5;
				}
				num4 += num5;
			}
			curpos = num;
			IL_194:
			if (flag)
			{
				num2 = -num2;
			}
			return num2 * Math.Pow(2.0, (double)num4);
		}

		public static double Str2Number(string s, ref int curpos)
		{
			int num = curpos;
			while (num < s.Length && char.IsWhiteSpace(s.get_Chars(num)))
			{
				num++;
			}
			bool flag = Utl.IsNegative(s, ref num);
			double num2 = 0.0;
			int num3 = 0;
			int num4 = 0;
			num2 = Utl.ReadDecimal(s, ref num, num2, out num3);
			if (num < s.Length && s.get_Chars(num) == '.')
			{
				num++;
				num2 = Utl.ReadDecimal(s, ref num, num2, out num4);
			}
			if (num3 == 0 && num4 == 0)
			{
				return 0.0;
			}
			num4 = -num4;
			curpos = num;
			double num5 = 0.0;
			if (num < s.Length && (s.get_Chars(num) == 'e' || s.get_Chars(num) == 'E'))
			{
				num++;
				bool flag2 = Utl.IsNegative(s, ref num);
				if (num >= s.Length || !char.IsDigit(s.get_Chars(num)))
				{
					goto IL_11E;
				}
				int num6;
				num5 = Utl.ReadDecimal(s, ref num, num5, out num6);
				if (flag2)
				{
					num5 = -num5;
				}
				num4 += (int)num5;
			}
			curpos = num;
			IL_11E:
			if (flag)
			{
				num2 = -num2;
			}
			return num2 * Math.Pow(10.0, (double)num4);
		}

		public static string TrimWhiteSpace(string str)
		{
			int num = 0;
			int num2 = str.Length;
			while (num < str.Length && char.IsWhiteSpace(str.get_Chars(num)))
			{
				num++;
			}
			if (num >= num2)
			{
				return string.Empty;
			}
			while (num2 >= 0 && char.IsWhiteSpace(str.get_Chars(num2 - 1)))
			{
				num2--;
			}
			return str.Substring(num, num2 - num);
		}
	}
}
