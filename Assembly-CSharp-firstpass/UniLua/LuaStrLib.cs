using System;
using System.Text;

namespace UniLua
{
	internal static class LuaStrLib
	{
		public static int OpenLib(ILuaState lua)
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
				new NameFuncPair("byte", new CSharpFunctionDelegate(LuaStrLib.Str_Byte)),
				new NameFuncPair("char", new CSharpFunctionDelegate(LuaStrLib.Str_Char)),
				new NameFuncPair("dump", new CSharpFunctionDelegate(LuaStrLib.Str_Dump)),
				new NameFuncPair("find", new CSharpFunctionDelegate(LuaStrLib.Str_Find)),
				new NameFuncPair("format", new CSharpFunctionDelegate(LuaStrLib.Str_Format)),
				new NameFuncPair("gmatch", new CSharpFunctionDelegate(LuaStrLib.Str_Gmatch)),
				new NameFuncPair("gsub", new CSharpFunctionDelegate(LuaStrLib.Str_Gsub)),
				new NameFuncPair("len", new CSharpFunctionDelegate(LuaStrLib.Str_Len)),
				new NameFuncPair("lower", new CSharpFunctionDelegate(LuaStrLib.Str_Lower)),
				new NameFuncPair("match", new CSharpFunctionDelegate(LuaStrLib.Str_Match)),
				new NameFuncPair("rep", new CSharpFunctionDelegate(LuaStrLib.Str_Rep)),
				new NameFuncPair("reverse", new CSharpFunctionDelegate(LuaStrLib.Str_Reverse)),
				new NameFuncPair("sub", new CSharpFunctionDelegate(LuaStrLib.Str_Sub)),
				new NameFuncPair("upper", new CSharpFunctionDelegate(LuaStrLib.Str_Upper))
			};
			lua.L_NewLib(define);
			LuaStrLib.CreateMetaTable(lua);
			return 1;
		}

		private static void CreateMetaTable(ILuaState lua)
		{
			lua.CreateTable(0, 1);
			lua.PushString(string.Empty);
			lua.PushValue(-2);
			lua.SetMetaTable(-2);
			lua.Pop(1);
			lua.PushValue(-2);
			lua.SetField(-2, "__index");
			lua.Pop(1);
		}

		private static int PosRelative(int pos, int len)
		{
			if (pos >= 0)
			{
				return pos;
			}
			if (0 - pos > len)
			{
				return 0;
			}
			return len - -pos + 1;
		}

		private static int Str_Byte(ILuaState lua)
		{
			string text = lua.L_CheckString(1);
			int num = LuaStrLib.PosRelative(lua.L_OptInt(2, 1), text.Length);
			int num2 = LuaStrLib.PosRelative(lua.L_OptInt(3, num), text.Length);
			if (num < 1)
			{
				num = 1;
			}
			if (num2 > text.Length)
			{
				num2 = text.Length;
			}
			if (num > num2)
			{
				return 0;
			}
			int num3 = num2 - num + 1;
			if (num + num3 <= num2)
			{
				return lua.L_Error("string slice too long", new object[0]);
			}
			lua.L_CheckStack(num3, "string slice too long");
			for (int i = 0; i < num3; i++)
			{
				lua.PushInteger((int)((byte)text.get_Chars(num + i - 1)));
			}
			return num3;
		}

		private static int Str_Char(ILuaState lua)
		{
			int top = lua.GetTop();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 1; i <= top; i++)
			{
				int num = lua.L_CheckInteger(i);
				lua.L_ArgCheck((int)((ushort)num) == num, i, "value out of range");
				stringBuilder.Append((char)num);
			}
			lua.PushString(stringBuilder.ToString());
			return 1;
		}

		private static int Str_Dump(ILuaState lua)
		{
			lua.L_CheckType(1, LuaType.LUA_TFUNCTION);
			lua.SetTop(1);
			ByteStringBuilder bsb = new ByteStringBuilder();
			LuaWriter writeFunc = delegate(byte[] bytes, int start, int length)
			{
				bsb.Append(bytes, start, length);
				return DumpStatus.OK;
			};
			if (lua.Dump(writeFunc) != DumpStatus.OK)
			{
				return lua.L_Error("unable to dump given function", new object[0]);
			}
			lua.PushString(bsb.ToString());
			return 1;
		}

		private static int ClassEnd(LuaStrLib.MatchState ms, int p)
		{
			ILuaState lua = ms.Lua;
			char c = ms.Pattern.get_Chars(p++);
			if (c == '%')
			{
				if (p == ms.PatternEnd)
				{
					lua.L_Error("malformed pattern (ends with '%')", new object[0]);
				}
				return p + 1;
			}
			if (c != '[')
			{
				return p;
			}
			if (ms.Pattern.get_Chars(p) == '^')
			{
				p++;
			}
			do
			{
				if (p == ms.PatternEnd)
				{
					lua.L_Error("malformed pattern (missing ']')", new object[0]);
				}
				if (ms.Pattern.get_Chars(p++) == '%' && p < ms.PatternEnd)
				{
					p++;
				}
			}
			while (ms.Pattern.get_Chars(p) != ']');
			return p + 1;
		}

		private static bool IsXDigit(char c)
		{
			switch (c)
			{
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			case 'A':
			case 'B':
			case 'C':
			case 'D':
			case 'E':
			case 'F':
				break;
			default:
				switch (c)
				{
				case 'a':
				case 'b':
				case 'c':
				case 'd':
				case 'e':
				case 'f':
					break;
				default:
					return false;
				}
				break;
			}
			return true;
		}

		private static bool MatchClass(char c, char cl)
		{
			bool result;
			switch (cl)
			{
			case 'p':
				result = char.IsPunctuation(c);
				break;
			default:
				switch (cl)
				{
				case 'a':
					result = char.IsLetter(c);
					break;
				default:
					if (cl != 'l')
					{
						return cl == c;
					}
					result = char.IsLower(c);
					break;
				case 'c':
					result = char.IsControl(c);
					break;
				case 'd':
					result = char.IsDigit(c);
					break;
				case 'g':
					throw new NotImplementedException();
				}
				break;
			case 's':
				result = char.IsWhiteSpace(c);
				break;
			case 'u':
				result = char.IsUpper(c);
				break;
			case 'w':
				result = char.IsLetterOrDigit(c);
				break;
			case 'x':
				result = LuaStrLib.IsXDigit(c);
				break;
			case 'z':
				result = (c == '\0');
				break;
			}
			return result;
		}

		private static bool MatchBreaketClass(LuaStrLib.MatchState ms, char c, int p, int ec)
		{
			bool flag = true;
			if (ms.Pattern.get_Chars(p + 1) == '^')
			{
				flag = false;
				p++;
			}
			while (++p < ec)
			{
				if (ms.Pattern.get_Chars(p) == '%')
				{
					p++;
					if (LuaStrLib.MatchClass(c, ms.Pattern.get_Chars(p)))
					{
						return flag;
					}
				}
				else if (ms.Pattern.get_Chars(p + 1) == '-' && p + 2 < ec)
				{
					p += 2;
					if (ms.Pattern.get_Chars(p - 2) <= c && c <= ms.Pattern.get_Chars(p))
					{
						return flag;
					}
				}
				else if (ms.Pattern.get_Chars(p) == c)
				{
					return flag;
				}
			}
			return !flag;
		}

		private static bool SingleMatch(LuaStrLib.MatchState ms, char c, int p, int ep)
		{
			char c2 = ms.Pattern.get_Chars(p);
			if (c2 == '%')
			{
				return LuaStrLib.MatchClass(c, ms.Pattern.get_Chars(p + 1));
			}
			if (c2 == '.')
			{
				return true;
			}
			if (c2 != '[')
			{
				return ms.Pattern.get_Chars(p) == c;
			}
			return LuaStrLib.MatchBreaketClass(ms, c, p, ep - 1);
		}

		private static int MatchBalance(LuaStrLib.MatchState ms, int s, int p)
		{
			ILuaState lua = ms.Lua;
			if (p >= ms.PatternEnd - 1)
			{
				lua.L_Error("malformed pattern (missing arguments to '%b')", new object[0]);
			}
			if (ms.Src.get_Chars(s) != ms.Pattern.get_Chars(p))
			{
				return -1;
			}
			char c = ms.Pattern.get_Chars(p);
			char c2 = ms.Pattern.get_Chars(p + 1);
			int num = 1;
			while (++s < ms.SrcEnd)
			{
				if (ms.Src.get_Chars(s) == c2)
				{
					if (--num == 0)
					{
						return s + 1;
					}
				}
				else if (ms.Src.get_Chars(s) == c)
				{
					num++;
				}
			}
			return -1;
		}

		private static int MaxExpand(LuaStrLib.MatchState ms, int s, int p, int ep)
		{
			int i = 0;
			while (s + i < ms.SrcEnd && LuaStrLib.SingleMatch(ms, ms.Src.get_Chars(s + i), p, ep))
			{
				i++;
			}
			while (i >= 0)
			{
				int num = LuaStrLib.Match(ms, s + i, ep + 1);
				if (num >= 0)
				{
					return num;
				}
				i--;
			}
			return -1;
		}

		private static int MinExpand(LuaStrLib.MatchState ms, int s, int p, int ep)
		{
			int num;
			for (;;)
			{
				num = LuaStrLib.Match(ms, s, ep + 1);
				if (num >= 0)
				{
					break;
				}
				if (s >= ms.SrcEnd || !LuaStrLib.SingleMatch(ms, ms.Src.get_Chars(s), p, ep))
				{
					return -1;
				}
				s++;
			}
			return num;
		}

		private static int CaptureToClose(LuaStrLib.MatchState ms)
		{
			ILuaState lua = ms.Lua;
			int i = ms.Level;
			for (i--; i >= 0; i--)
			{
				if (ms.Capture[i].Len == -1)
				{
					return i;
				}
			}
			return lua.L_Error("invalid pattern capture", new object[0]);
		}

		private static int StartCapture(LuaStrLib.MatchState ms, int s, int p, int what)
		{
			ILuaState lua = ms.Lua;
			int level = ms.Level;
			if (level >= 32)
			{
				lua.L_Error("too many captures", new object[0]);
			}
			ms.Capture[level].Init = s;
			ms.Capture[level].Len = what;
			ms.Level = level + 1;
			int num = LuaStrLib.Match(ms, s, p);
			if (num == -1)
			{
				ms.Level--;
			}
			return num;
		}

		private static int EndCapture(LuaStrLib.MatchState ms, int s, int p)
		{
			int num = LuaStrLib.CaptureToClose(ms);
			ms.Capture[num].Len = s - ms.Capture[num].Init;
			int num2 = LuaStrLib.Match(ms, s, p);
			if (num2 == -1)
			{
				ms.Capture[num].Len = -1;
			}
			return num2;
		}

		private static int CheckCapture(LuaStrLib.MatchState ms, char l)
		{
			ILuaState lua = ms.Lua;
			int num = (int)(l - '1');
			if (num < 0 || num >= ms.Level || ms.Capture[num].Len == -1)
			{
				return lua.L_Error("invalid capture index %d", new object[]
				{
					num + 1
				});
			}
			return num;
		}

		private static int MatchCapture(LuaStrLib.MatchState ms, int s, char l)
		{
			int num = LuaStrLib.CheckCapture(ms, l);
			int len = ms.Capture[num].Len;
			if (ms.SrcEnd - s >= len && string.Compare(ms.Src, ms.Capture[num].Init, ms.Src, s, len) == 0)
			{
				return s + len;
			}
			return -1;
		}

		private static int Match(LuaStrLib.MatchState ms, int s, int p)
		{
			ILuaState lua = ms.Lua;
			while (p != ms.PatternEnd)
			{
				switch (ms.Pattern.get_Chars(p))
				{
				case '$':
					if (p + 1 == ms.PatternEnd)
					{
						return (s != ms.SrcEnd) ? -1 : s;
					}
					break;
				case '%':
				{
					char c = ms.Pattern.get_Chars(p + 1);
					switch (c)
					{
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						s = LuaStrLib.MatchCapture(ms, s, ms.Pattern.get_Chars(p + 1));
						if (s == -1)
						{
							return -1;
						}
						p += 2;
						continue;
					default:
						if (c != 'b')
						{
							if (c == 'f')
							{
								p += 2;
								if (ms.Pattern.get_Chars(p) != '[')
								{
									lua.L_Error("missing '[' after '%f' in pattern", new object[0]);
								}
								int num = LuaStrLib.ClassEnd(ms, p);
								char c2 = (s != ms.SrcInit) ? ms.Src.get_Chars(s - 1) : '\0';
								if (LuaStrLib.MatchBreaketClass(ms, c2, p, num - 1) || !LuaStrLib.MatchBreaketClass(ms, ms.Src.get_Chars(s), p, num - 1))
								{
									return -1;
								}
								p = num;
								continue;
							}
						}
						else
						{
							s = LuaStrLib.MatchBalance(ms, s, p + 2);
							if (s == -1)
							{
								return -1;
							}
							p += 4;
							continue;
						}
						break;
					}
					break;
				}
				case '(':
					if (ms.Pattern.get_Chars(p + 1) == ')')
					{
						return LuaStrLib.StartCapture(ms, s, p + 2, -2);
					}
					return LuaStrLib.StartCapture(ms, s, p + 1, -1);
				case ')':
					return LuaStrLib.EndCapture(ms, s, p + 1);
				}
				int num2 = LuaStrLib.ClassEnd(ms, p);
				bool flag = s < ms.SrcEnd && LuaStrLib.SingleMatch(ms, ms.Src.get_Chars(s), p, num2);
				if (num2 < ms.PatternEnd)
				{
					char c = ms.Pattern.get_Chars(num2);
					switch (c)
					{
					case '*':
						return LuaStrLib.MaxExpand(ms, s, p, num2);
					case '+':
						return (!flag) ? -1 : LuaStrLib.MaxExpand(ms, s + 1, p, num2);
					default:
						if (c == '?')
						{
							if (flag)
							{
								int num3 = LuaStrLib.Match(ms, s + 1, num2 + 1);
								if (num3 != -1)
								{
									return num3;
								}
							}
							p = num2 + 1;
							continue;
						}
						break;
					case '-':
						return LuaStrLib.MinExpand(ms, s, p, num2);
					}
				}
				if (!flag)
				{
					return -1;
				}
				s++;
				p = num2;
			}
			return s;
		}

		private static void PushOneCapture(LuaStrLib.MatchState ms, int i, int start, int end)
		{
			ILuaState lua = ms.Lua;
			if (i >= ms.Level)
			{
				if (i == 0)
				{
					lua.PushString(ms.Src.Substring(start, end - start));
				}
				else
				{
					lua.L_Error("invalid capture index", new object[0]);
				}
			}
			else
			{
				int len = ms.Capture[i].Len;
				if (len == -1)
				{
					lua.L_Error("unfinished capture", new object[0]);
				}
				if (len == -2)
				{
					lua.PushInteger(ms.Capture[i].Init - ms.SrcInit + 1);
				}
				else
				{
					lua.PushString(ms.Src.Substring(ms.Capture[i].Init, len));
				}
			}
		}

		private static int PushCaptures(ILuaState lua, LuaStrLib.MatchState ms, int spos, int epos)
		{
			int num = (ms.Level != 0 || spos < 0) ? ms.Level : 1;
			lua.L_CheckStack(num, "too many captures");
			for (int i = 0; i < num; i++)
			{
				LuaStrLib.PushOneCapture(ms, i, spos, epos);
			}
			return num;
		}

		private static bool NoSpecials(string pattern)
		{
			return pattern.IndexOfAny(LuaStrLib.SPECIALS) == -1;
		}

		private static int StrFindAux(ILuaState lua, bool find)
		{
			string text = lua.L_CheckString(1);
			string text2 = lua.L_CheckString(2);
			int num = LuaStrLib.PosRelative(lua.L_OptInt(3, 1), text.Length);
			if (num < 1)
			{
				num = 1;
			}
			else if (num > text.Length + 1)
			{
				lua.PushNil();
				return 1;
			}
			if (find && (lua.ToBoolean(4) || LuaStrLib.NoSpecials(text2)))
			{
				int num2 = text.IndexOf(text2, num - 1);
				if (num2 >= 0)
				{
					lua.PushInteger(num2 + 1);
					lua.PushInteger(num2 + text2.Length);
					return 2;
				}
			}
			else
			{
				int num3 = num - 1;
				int num4 = 0;
				bool flag = text2.get_Chars(num4) == '^';
				if (flag)
				{
					num4++;
				}
				LuaStrLib.MatchState matchState = new LuaStrLib.MatchState();
				matchState.Lua = lua;
				matchState.Src = text;
				matchState.SrcInit = num3;
				matchState.SrcEnd = text.Length;
				matchState.Pattern = text2;
				matchState.PatternEnd = text2.Length;
				int num5;
				for (;;)
				{
					matchState.Level = 0;
					num5 = LuaStrLib.Match(matchState, num3, num4);
					if (num5 != -1)
					{
						break;
					}
					if (num3++ >= matchState.SrcEnd || flag)
					{
						goto IL_163;
					}
				}
				if (find)
				{
					lua.PushInteger(num3 + 1);
					lua.PushInteger(num5);
					return LuaStrLib.PushCaptures(lua, matchState, -1, 0) + 2;
				}
				return LuaStrLib.PushCaptures(lua, matchState, num3, num5);
			}
			IL_163:
			lua.PushNil();
			return 1;
		}

		private static int Str_Find(ILuaState lua)
		{
			return LuaStrLib.StrFindAux(lua, true);
		}

		private static int ScanFormat(ILuaState lua, string format, int s, out string form)
		{
			int num = s;
			while (num < format.Length && format.get_Chars(num) != '\0' && "-+ #0".IndexOf(format.get_Chars(num)) != -1)
			{
				num++;
			}
			if (num - s > "-+ #0".Length)
			{
				lua.L_Error("invalid format (repeat flags)", new object[0]);
			}
			if (char.IsDigit(format.get_Chars(num)))
			{
				num++;
			}
			if (char.IsDigit(format.get_Chars(num)))
			{
				num++;
			}
			if (format.get_Chars(num) == '.')
			{
				num++;
				if (char.IsDigit(format.get_Chars(num)))
				{
					num++;
				}
				if (char.IsDigit(format.get_Chars(num)))
				{
					num++;
				}
			}
			if (char.IsDigit(format.get_Chars(num)))
			{
				lua.L_Error("invalid format (width of precision too long)", new object[0]);
			}
			form = "%" + format.Substring(s, num - s + 1);
			return num;
		}

		private static int Str_Format(ILuaState lua)
		{
			int top = lua.GetTop();
			StringBuilder stringBuilder = new StringBuilder();
			int num = 1;
			string text = lua.L_CheckString(num);
			int i = 0;
			int length = text.Length;
			while (i < length)
			{
				if (text.get_Chars(i) != '%')
				{
					stringBuilder.Append(text.get_Chars(i++));
				}
				else if (text.get_Chars(++i) == '%')
				{
					stringBuilder.Append(text.get_Chars(i++));
				}
				else
				{
					if (++num > top)
					{
						lua.L_ArgError(num, "no value");
					}
					string text2;
					i = LuaStrLib.ScanFormat(lua, text, i, out text2);
					char c = text.get_Chars(i++);
					switch (c)
					{
					case 'c':
						stringBuilder.Append((char)lua.L_CheckInteger(num));
						continue;
					case 'd':
					case 'i':
						stringBuilder.Append(lua.L_CheckInteger(num).ToString());
						continue;
					case 'e':
						break;
					case 'f':
						stringBuilder.AppendFormat("{0:F}", lua.L_CheckNumber(num));
						continue;
					case 'g':
						goto IL_26B;
					default:
						switch (c)
						{
						case 'E':
							break;
						default:
						{
							if (c != 'X')
							{
								return lua.L_Error("invalid option '{0}' to 'format'", new object[]
								{
									text.get_Chars(i - 1)
								});
							}
							int num2 = lua.L_CheckInteger(num);
							lua.L_ArgCheck(num2 >= 0, num, "not a non-negative number is proper range");
							stringBuilder.AppendFormat("{0:X}", num2);
							continue;
						}
						case 'G':
							goto IL_26B;
						}
						break;
					case 'o':
					{
						int num3 = lua.L_CheckInteger(num);
						lua.L_ArgCheck(num3 >= 0, num, "not a non-negative number is proper range");
						stringBuilder.Append(Convert.ToString(num3, 8));
						continue;
					}
					case 'q':
						LuaStrLib.AddQuoted(lua, stringBuilder, num);
						continue;
					case 's':
						stringBuilder.Append(lua.L_CheckString(num));
						continue;
					case 'u':
					{
						int num4 = lua.L_CheckInteger(num);
						lua.L_ArgCheck(num4 >= 0, num, "not a non-negative number is proper range");
						stringBuilder.Append(num4.ToString());
						continue;
					}
					case 'x':
					{
						int num5 = lua.L_CheckInteger(num);
						lua.L_ArgCheck(num5 >= 0, num, "not a non-negative number is proper range");
						stringBuilder.AppendFormat("{0:x}", num5);
						continue;
					}
					}
					stringBuilder.AppendFormat("{0:E}", lua.L_CheckNumber(num));
					continue;
					IL_26B:
					stringBuilder.AppendFormat("{0:G}", lua.L_CheckNumber(num));
				}
			}
			lua.PushString(stringBuilder.ToString());
			return 1;
		}

		private static void AddQuoted(ILuaState lua, StringBuilder sb, int arg)
		{
			string text = lua.L_CheckString(arg);
			sb.Append('"');
			for (int i = 0; i < text.Length; i++)
			{
				char c = text.get_Chars(i);
				if (c == '"' || c == '\\' || c == '\n')
				{
					sb.Append('\\').Append(c);
				}
				else if (c == '\0' || char.IsControl(c))
				{
					if (i + 1 >= text.Length || !char.IsDigit(text.get_Chars(i + 1)))
					{
						sb.AppendFormat("\\{0:D}", (int)c);
					}
					else
					{
						sb.AppendFormat("\\{0:D3}", (int)c);
					}
				}
				else
				{
					sb.Append(c);
				}
			}
			sb.Append('"');
		}

		private static int GmatchAux(ILuaState lua)
		{
			LuaStrLib.MatchState matchState = new LuaStrLib.MatchState();
			string text = lua.ToString(lua.UpvalueIndex(1));
			string text2 = lua.ToString(lua.UpvalueIndex(2));
			matchState.Lua = lua;
			matchState.Src = text;
			matchState.SrcInit = 0;
			matchState.SrcEnd = text.Length;
			matchState.Pattern = text2;
			matchState.PatternEnd = text2.Length;
			for (int i = lua.ToInteger(lua.UpvalueIndex(3)); i <= matchState.SrcEnd; i++)
			{
				matchState.Level = 0;
				int num = LuaStrLib.Match(matchState, i, 0);
				if (num != -1)
				{
					int n = (num != 0) ? num : (num + 1);
					lua.PushInteger(n);
					lua.Replace(lua.UpvalueIndex(3));
					return LuaStrLib.PushCaptures(lua, matchState, i, num);
				}
			}
			return 0;
		}

		private static int Str_Gmatch(ILuaState lua)
		{
			lua.L_CheckString(1);
			lua.L_CheckString(2);
			lua.SetTop(2);
			lua.PushInteger(0);
			lua.PushCSharpClosure(new CSharpFunctionDelegate(LuaStrLib.GmatchAux), 3);
			return 1;
		}

		private static void Add_S(LuaStrLib.MatchState ms, StringBuilder b, int s, int e)
		{
			string text = ms.Lua.ToString(3);
			for (int i = 0; i < text.Length; i++)
			{
				if (text.get_Chars(i) != '%')
				{
					b.Append(text.get_Chars(i));
				}
				else
				{
					i++;
					if (!char.IsDigit(text.get_Chars(i)))
					{
						b.Append(text.get_Chars(i));
					}
					else if (text.get_Chars(i) == '0')
					{
						b.Append(ms.Src.Substring(s, e - s));
					}
					else
					{
						LuaStrLib.PushOneCapture(ms, (int)(text.get_Chars(i) - '1'), s, e);
						b.Append(ms.Lua.ToString(-1));
					}
				}
			}
		}

		private static void Add_Value(LuaStrLib.MatchState ms, StringBuilder b, int s, int e)
		{
			ILuaState lua = ms.Lua;
			switch (lua.Type(3))
			{
			case LuaType.LUA_TNUMBER:
			case LuaType.LUA_TSTRING:
				LuaStrLib.Add_S(ms, b, s, e);
				return;
			case LuaType.LUA_TTABLE:
				LuaStrLib.PushOneCapture(ms, 0, s, e);
				lua.GetTable(3);
				break;
			case LuaType.LUA_TFUNCTION:
			{
				lua.PushValue(3);
				int numArgs = LuaStrLib.PushCaptures(lua, ms, s, e);
				lua.Call(numArgs, 1);
				break;
			}
			}
			if (!lua.ToBoolean(-1))
			{
				lua.Pop(1);
				b.Append(ms.Src.Substring(s, e - s));
			}
			else if (!lua.IsString(-1))
			{
				lua.L_Error("invalid replacement value (a %s)", new object[]
				{
					lua.L_TypeName(-1)
				});
			}
			else
			{
				b.Append(lua.ToString(-1));
			}
		}

		private static int Str_Gsub(ILuaState lua)
		{
			string text = lua.L_CheckString(1);
			int length = text.Length;
			string text2 = lua.L_CheckString(2);
			LuaType luaType = lua.Type(3);
			int num = lua.L_OptInt(4, length + 1);
			int num2 = 0;
			if (text2.get_Chars(0) == '^')
			{
				text2 = text2.Substring(1);
				num2 = 1;
			}
			int i = 0;
			LuaStrLib.MatchState matchState = new LuaStrLib.MatchState();
			StringBuilder stringBuilder = new StringBuilder(length);
			lua.L_ArgCheck(luaType == LuaType.LUA_TNUMBER || luaType == LuaType.LUA_TSTRING || luaType == LuaType.LUA_TFUNCTION || luaType == LuaType.LUA_TTABLE, 3, "string/function/table expected");
			matchState.Lua = lua;
			matchState.Src = text;
			matchState.SrcInit = 0;
			matchState.SrcEnd = length;
			matchState.Pattern = text2;
			matchState.PatternEnd = text2.Length;
			int num3 = 0;
			while (i < num)
			{
				matchState.Level = 0;
				int num4 = LuaStrLib.Match(matchState, num3, 0);
				if (num4 != -1)
				{
					i++;
					LuaStrLib.Add_Value(matchState, stringBuilder, num3, num4);
				}
				if (num4 != -1 && num4 > num3)
				{
					num3 = num4;
				}
				else
				{
					if (num3 >= matchState.SrcEnd)
					{
						break;
					}
					char c = text.get_Chars(num3);
					num3++;
					stringBuilder.Append(c);
				}
				if (num2 != 0)
				{
					break;
				}
			}
			stringBuilder.Append(text.Substring(num3, matchState.SrcEnd - num3));
			lua.PushString(stringBuilder.ToString());
			lua.PushInteger(i);
			return 2;
		}

		private static int Str_Len(ILuaState lua)
		{
			string text = lua.L_CheckString(1);
			lua.PushInteger(text.Length);
			return 1;
		}

		private static int Str_Lower(ILuaState lua)
		{
			string text = lua.L_CheckString(1);
			lua.PushString(text.ToLower());
			return 1;
		}

		private static int Str_Match(ILuaState lua)
		{
			return LuaStrLib.StrFindAux(lua, false);
		}

		private static int Str_Rep(ILuaState lua)
		{
			throw new NotImplementedException();
		}

		private static int Str_Reverse(ILuaState lua)
		{
			string text = lua.L_CheckString(1);
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			for (int i = text.Length - 1; i >= 0; i--)
			{
				stringBuilder.Append(text.get_Chars(i));
			}
			lua.PushString(stringBuilder.ToString());
			return 1;
		}

		private static int Str_Sub(ILuaState lua)
		{
			string text = lua.L_CheckString(1);
			int num = LuaStrLib.PosRelative(lua.L_CheckInteger(2), text.Length);
			int num2 = LuaStrLib.PosRelative(lua.L_OptInt(3, -1), text.Length);
			if (num < 1)
			{
				num = 1;
			}
			if (num2 > text.Length)
			{
				num2 = text.Length;
			}
			if (num <= num2)
			{
				lua.PushString(text.Substring(num - 1, num2 - num + 1));
			}
			else
			{
				lua.PushString(string.Empty);
			}
			return 1;
		}

		private static int Str_Upper(ILuaState lua)
		{
			string text = lua.L_CheckString(1);
			lua.PushString(text.ToUpper());
			return 1;
		}

		public const string LIB_NAME = "string";

		private const int CAP_UNFINISHED = -1;

		private const int CAP_POSITION = -2;

		private const int LUA_MAXCAPTURES = 32;

		private const char L_ESC = '%';

		private const string FLAGS = "-+ #0";

		private static readonly char[] SPECIALS = "^$*+?.([%-".ToCharArray();

		private class CaptureInfo
		{
			public int Len;

			public int Init;
		}

		private class MatchState
		{
			public MatchState()
			{
				this.Capture = new LuaStrLib.CaptureInfo[32];
				for (int i = 0; i < 32; i++)
				{
					this.Capture[i] = new LuaStrLib.CaptureInfo();
				}
			}

			public ILuaState Lua;

			public int Level;

			public string Src;

			public int SrcInit;

			public int SrcEnd;

			public string Pattern;

			public int PatternEnd;

			public LuaStrLib.CaptureInfo[] Capture;
		}
	}
}
