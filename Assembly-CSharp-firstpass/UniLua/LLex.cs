using System;
using System.Collections.Generic;
using System.Text;

namespace UniLua
{
	public class LLex
	{
		public LLex(ILuaState lua, ILoadInfo loadinfo, string name)
		{
			this.Lua = (LuaState)lua;
			this.LoadInfo = loadinfo;
			this.LineNumber = 1;
			this.LastLine = 1;
			this.Token = null;
			this.LookAhead = null;
			this._Saved = null;
			this.Source = name;
			this._Next();
		}

		static LLex()
		{
			LLex.ReservedWordDict.Add("and", TK.AND);
			LLex.ReservedWordDict.Add("break", TK.BREAK);
			LLex.ReservedWordDict.Add("do", TK.DO);
			LLex.ReservedWordDict.Add("else", TK.ELSE);
			LLex.ReservedWordDict.Add("elseif", TK.ELSEIF);
			LLex.ReservedWordDict.Add("end", TK.END);
			LLex.ReservedWordDict.Add("false", TK.FALSE);
			LLex.ReservedWordDict.Add("for", TK.FOR);
			LLex.ReservedWordDict.Add("function", TK.FUNCTION);
			LLex.ReservedWordDict.Add("goto", TK.GOTO);
			LLex.ReservedWordDict.Add("if", TK.IF);
			LLex.ReservedWordDict.Add("in", TK.IN);
			LLex.ReservedWordDict.Add("local", TK.LOCAL);
			LLex.ReservedWordDict.Add("nil", TK.NIL);
			LLex.ReservedWordDict.Add("not", TK.NOT);
			LLex.ReservedWordDict.Add("or", TK.OR);
			LLex.ReservedWordDict.Add("repeat", TK.REPEAT);
			LLex.ReservedWordDict.Add("return", TK.RETURN);
			LLex.ReservedWordDict.Add("then", TK.THEN);
			LLex.ReservedWordDict.Add("true", TK.TRUE);
			LLex.ReservedWordDict.Add("until", TK.UNTIL);
			LLex.ReservedWordDict.Add("while", TK.WHILE);
		}

		private StringBuilder Saved
		{
			get
			{
				if (this._Saved == null)
				{
					this._Saved = new StringBuilder();
				}
				return this._Saved;
			}
		}

		public void Next()
		{
			this.LastLine = this.LineNumber;
			if (this.LookAhead != null)
			{
				this.Token = this.LookAhead;
				this.LookAhead = null;
			}
			else
			{
				this.Token = this._Lex();
			}
		}

		public Token GetLookAhead()
		{
			Utl.Assert(this.LookAhead == null);
			this.LookAhead = this._Lex();
			return this.LookAhead;
		}

		private void _Next()
		{
			int num = this.LoadInfo.ReadByte();
			this.Current = ((num != -1) ? num : 65535);
		}

		private void _SaveAndNext()
		{
			this.Saved.Append((char)this.Current);
			this._Next();
		}

		private void _Save(char c)
		{
			this.Saved.Append(c);
		}

		private string _GetSavedString()
		{
			return this.Saved.ToString();
		}

		private void _ClearSaved()
		{
			this._Saved = null;
		}

		private bool _CurrentIsNewLine()
		{
			return this.Current == 10 || this.Current == 13;
		}

		private bool _CurrentIsDigit()
		{
			return char.IsDigit((char)this.Current);
		}

		private bool _CurrentIsXDigit()
		{
			return this._CurrentIsDigit() || (65 <= this.Current && this.Current <= 70) || (97 <= this.Current && this.Current <= 102);
		}

		private bool _CurrentIsSpace()
		{
			return char.IsWhiteSpace((char)this.Current);
		}

		private bool _CurrentIsAlpha()
		{
			return char.IsLetter((char)this.Current);
		}

		private bool _IsReserved(string identifier, out TK type)
		{
			return LLex.ReservedWordDict.TryGetValue(identifier, ref type);
		}

		public bool IsReservedWord(string name)
		{
			return LLex.ReservedWordDict.ContainsKey(name);
		}

		private void _IncLineNumber()
		{
			int current = this.Current;
			this._Next();
			if (this._CurrentIsNewLine() && this.Current != current)
			{
				this._Next();
			}
			if (++this.LineNumber >= 2147483647)
			{
				this._Error("chunk has too many lines");
			}
		}

		private string _ReadLongString(int sep)
		{
			this._SaveAndNext();
			if (this._CurrentIsNewLine())
			{
				this._IncLineNumber();
			}
			for (;;)
			{
				int current = this.Current;
				switch (current)
				{
				case 10:
				case 13:
					this._Save('\n');
					this._IncLineNumber();
					break;
				default:
					switch (current)
					{
					case 91:
						if (this._SkipSep() == sep)
						{
							this._SaveAndNext();
							if (sep == 0)
							{
								this._LexError(this._GetSavedString(), "nesting of [[...]] is deprecated", 289);
							}
						}
						break;
					default:
						if (current != 65535)
						{
							this._SaveAndNext();
						}
						else
						{
							this._LexError(this._GetSavedString(), "unfinished long string/comment", 289);
						}
						break;
					case 93:
						if (this._SkipSep() == sep)
						{
							goto Block_5;
						}
						break;
					}
					break;
				}
			}
			Block_5:
			this._SaveAndNext();
			string text = this._GetSavedString();
			return text.Substring(2 + sep, text.Length - 2 * (2 + sep));
		}

		private void _EscapeError(string info, string msg)
		{
			this._LexError("\\" + info, msg, 287);
		}

		private byte _ReadHexEscape()
		{
			int num = 0;
			char[] array = new char[3];
			array[0] = 'x';
			char[] array2 = array;
			for (int i = 1; i < 3; i++)
			{
				this._Next();
				array2[i] = (char)this.Current;
				if (!this._CurrentIsXDigit())
				{
					this._EscapeError(new string(array2, 0, i + 1), "hexadecimal digit expected");
				}
				num = (num << 4) + int.Parse(this.Current.ToString(), 515);
			}
			return (byte)num;
		}

		private byte _ReadDecEscape()
		{
			int num = 0;
			char[] array = new char[3];
			int num2 = 0;
			while (num2 < 3 && this._CurrentIsDigit())
			{
				array[num2] = (char)this.Current;
				num = num * 10 + this.Current - 48;
				this._Next();
				num2++;
			}
			if (num > 255)
			{
				this._EscapeError(new string(array, 0, num2), "decimal escape too large");
			}
			return (byte)num;
		}

		private string _ReadString()
		{
			int current = this.Current;
			this._Next();
			while (this.Current != current)
			{
				int current2 = this.Current;
				switch (current2)
				{
				case 10:
				case 13:
					this._Error("unfinished string");
					break;
				default:
					if (current2 != 92)
					{
						if (current2 != 65535)
						{
							this._SaveAndNext();
						}
						else
						{
							this._Error("unfinished string");
						}
					}
					else
					{
						this._Next();
						int current3 = this.Current;
						byte b;
						switch (current3)
						{
						case 114:
							b = 13;
							break;
						default:
							switch (current3)
							{
							case 97:
								b = 7;
								break;
							case 98:
								b = 8;
								break;
							default:
								switch (current3)
								{
								case 10:
								case 13:
									this._Save('\n');
									this._IncLineNumber();
									continue;
								default:
									if (current3 != 34 && current3 != 39 && current3 != 92)
									{
										if (current3 != 110)
										{
											if (current3 != 65535)
											{
												if (!this._CurrentIsDigit())
												{
													this._EscapeError(this.Current.ToString(), "invalid escape sequence");
												}
												b = this._ReadDecEscape();
												this._Save((char)b);
												continue;
											}
											continue;
										}
										else
										{
											b = 10;
										}
									}
									else
									{
										b = (byte)this.Current;
									}
									break;
								}
								break;
							case 102:
								b = 12;
								break;
							}
							break;
						case 116:
							b = 9;
							break;
						case 118:
							b = 11;
							break;
						case 120:
							b = this._ReadHexEscape();
							break;
						case 122:
							this._Next();
							while (this._CurrentIsSpace())
							{
								if (this._CurrentIsNewLine())
								{
									this._IncLineNumber();
								}
								else
								{
									this._Next();
								}
							}
							continue;
						}
						this._Save((char)b);
						this._Next();
					}
					break;
				}
			}
			this._Next();
			return this._GetSavedString();
		}

		private double _ReadNumber()
		{
			char[] array = new char[]
			{
				'E',
				'e'
			};
			Utl.Assert(this._CurrentIsDigit());
			int current = this.Current;
			this._SaveAndNext();
			if (current == 48 && (this.Current == 88 || this.Current == 120))
			{
				array = new char[]
				{
					'P',
					'p'
				};
				this._SaveAndNext();
			}
			for (;;)
			{
				if (this.Current == (int)array[0] || this.Current == (int)array[1])
				{
					this._SaveAndNext();
					if (this.Current == 43 || this.Current == 45)
					{
						this._SaveAndNext();
					}
				}
				if (!this._CurrentIsXDigit() && this.Current != 46)
				{
					break;
				}
				this._SaveAndNext();
			}
			string text = this._GetSavedString();
			double result;
			if (LuaState.O_Str2Decimal(text, out result))
			{
				return result;
			}
			this._Error("malformed number: " + text);
			return 0.0;
		}

		private void _Error(string error)
		{
			this.Lua.O_PushString(string.Format("{0}:{1}: {2}", this.Source, this.LineNumber, error));
			this.Lua.D_Throw(ThreadStatus.LUA_ERRSYNTAX);
		}

		private void _LexError(string info, string msg, int tokenType)
		{
			this._Error(msg + ":" + info);
		}

		public void SyntaxError(string msg)
		{
			this._Error(msg);
		}

		private int _SkipSep()
		{
			int num = 0;
			int current = this.Current;
			this._SaveAndNext();
			while (this.Current == 61)
			{
				this._SaveAndNext();
				num++;
			}
			return (this.Current != current) ? (-num - 1) : num;
		}

		private Token _Lex()
		{
			this._ClearSaved();
			int num;
			for (;;)
			{
				int current = this.Current;
				switch (current)
				{
				case 58:
					goto IL_204;
				default:
					switch (current)
					{
					case 10:
					case 13:
						this._IncLineNumber();
						break;
					default:
						if (current != 45)
						{
							if (current == 46)
							{
								goto IL_23C;
							}
							if (current == 34 || current == 39)
							{
								goto IL_230;
							}
							if (current != 91)
							{
								if (current == 126)
								{
									goto IL_1D8;
								}
								if (current == 65535)
								{
									goto IL_29D;
								}
								if (!this._CurrentIsSpace())
								{
									goto IL_2BE;
								}
								this._Next();
							}
							else
							{
								num = this._SkipSep();
								if (num >= 0)
								{
									goto Block_12;
								}
								if (num == -1)
								{
									goto Block_13;
								}
								this._Error("invalid long string delimiter");
							}
						}
						else
						{
							this._Next();
							if (this.Current != 45)
							{
								goto Block_8;
							}
							this._Next();
							if (this.Current == 91)
							{
								int num2 = this._SkipSep();
								this._ClearSaved();
								if (num2 >= 0)
								{
									this._ReadLongString(num2);
									this._ClearSaved();
									break;
								}
							}
							while (!this._CurrentIsNewLine() && this.Current != 65535)
							{
								this._Next();
							}
						}
						break;
					}
					break;
				case 60:
					goto IL_180;
				case 61:
					goto IL_154;
				case 62:
					goto IL_1AC;
				}
			}
			Block_8:
			return new LiteralToken(45);
			Block_12:
			string seminfo = this._ReadLongString(num);
			return new StringToken(seminfo);
			Block_13:
			return new LiteralToken(91);
			IL_154:
			this._Next();
			if (this.Current != 61)
			{
				return new LiteralToken(61);
			}
			this._Next();
			return new TypedToken(TK.EQ);
			IL_180:
			this._Next();
			if (this.Current != 61)
			{
				return new LiteralToken(60);
			}
			this._Next();
			return new TypedToken(TK.LE);
			IL_1AC:
			this._Next();
			if (this.Current != 61)
			{
				return new LiteralToken(62);
			}
			this._Next();
			return new TypedToken(TK.GE);
			IL_1D8:
			this._Next();
			if (this.Current != 61)
			{
				return new LiteralToken(126);
			}
			this._Next();
			return new TypedToken(TK.NE);
			IL_204:
			this._Next();
			if (this.Current != 58)
			{
				return new LiteralToken(58);
			}
			this._Next();
			return new TypedToken(TK.DBCOLON);
			IL_230:
			return new StringToken(this._ReadString());
			IL_23C:
			this._SaveAndNext();
			if (this.Current == 46)
			{
				this._SaveAndNext();
				if (this.Current == 46)
				{
					this._SaveAndNext();
					return new TypedToken(TK.DOTS);
				}
				return new TypedToken(TK.CONCAT);
			}
			else
			{
				if (!this._CurrentIsDigit())
				{
					return new LiteralToken(46);
				}
				return new NumberToken(this._ReadNumber());
			}
			IL_29D:
			return new TypedToken(TK.EOS);
			IL_2BE:
			if (this._CurrentIsDigit())
			{
				return new NumberToken(this._ReadNumber());
			}
			if (!this._CurrentIsAlpha() && this.Current != 95)
			{
				int current2 = this.Current;
				this._Next();
				return new LiteralToken(current2);
			}
			do
			{
				this._SaveAndNext();
			}
			while (this._CurrentIsAlpha() || this._CurrentIsDigit() || this.Current == 95);
			string text = this._GetSavedString();
			TK type;
			if (this._IsReserved(text, out type))
			{
				return new TypedToken(type);
			}
			return new NameToken(text);
		}

		public const char EOZ = '￿';

		private LuaState Lua;

		private int Current;

		public int LineNumber;

		public int LastLine;

		private ILoadInfo LoadInfo;

		public string Source;

		public Token Token;

		private Token LookAhead;

		private StringBuilder _Saved;

		private static Dictionary<string, TK> ReservedWordDict = new Dictionary<string, TK>();
	}
}
