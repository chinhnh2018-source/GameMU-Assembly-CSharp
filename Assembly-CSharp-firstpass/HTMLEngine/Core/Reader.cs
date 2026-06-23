using System;
using System.Text;

namespace HTMLEngine.Core
{
	public class Reader
	{
		public Reader()
		{
			this.sb = new StringBuilder(100);
		}

		public string CurrentText
		{
			get
			{
				return (!this.IsEof) ? this.text.Substring(this.curr) : "(eof)";
			}
		}

		public bool IsEof
		{
			get
			{
				return this.curr >= this.end;
			}
		}

		public long Length
		{
			get
			{
				return (long)(this.end - this.begin);
			}
		}

		public long Position
		{
			get
			{
				return (long)(this.curr - this.begin);
			}
		}

		public long Rest
		{
			get
			{
				return (long)(this.end - this.curr);
			}
		}

		public void SetSource(string text)
		{
			this.text = text;
			this.begin = 0;
			this.end = this.begin + text.Length;
			this.curr = this.begin;
		}

		public char CurrChar
		{
			get
			{
				return (this.curr < this.begin || this.curr >= this.end) ? '\0' : this.text.get_Chars(this.curr);
			}
		}

		public char NextChar
		{
			get
			{
				return (this.curr >= this.end - 1) ? '\0' : this.text.get_Chars(this.curr + 1);
			}
		}

		public char PrevChar
		{
			get
			{
				return (this.curr <= this.begin) ? '\0' : this.text.get_Chars(this.curr - 1);
			}
		}

		private void DoAutoSkip()
		{
			if (this.AutoSkipWhitespace)
			{
				this.SkipWhitespace();
			}
			while (this.curr < this.end)
			{
				int num = 0;
				if (this.AutoSkipSLComments)
				{
					while (this.IsOnSLComments())
					{
						this.SkipToChar('\n', true);
						if (this.AutoSkipWhitespace)
						{
							this.SkipWhitespace();
						}
						num++;
					}
				}
				if (this.AutoSkipMLComments)
				{
					while (this.IsOnMLComments())
					{
						while (this.SkipToChar('*', true) && this.CurrChar == '/')
						{
							this.curr++;
						}
						if (this.AutoSkipWhitespace)
						{
							this.SkipWhitespace();
						}
						num++;
					}
				}
				if (num == 0)
				{
					break;
				}
			}
		}

		public bool SkipToChar(char c, bool thenSkipThisChar = true)
		{
			this.DoAutoSkip();
			while (this.curr < this.end)
			{
				if (this.CurrChar == c)
				{
					if (thenSkipThisChar)
					{
						this.curr++;
					}
					break;
				}
			}
			this.DoAutoSkip();
			return this.curr < this.end;
		}

		public void Skip(int count)
		{
			this.curr += count;
			this.DoAutoSkip();
		}

		public bool SkipWhitespace()
		{
			int num = this.curr;
			while (this.IsOnWhitespace())
			{
				this.curr++;
			}
			return this.curr > num;
		}

		public string ReadToStopChar(char stopChar, bool ignoreCase = false)
		{
			this.DoAutoSkip();
			this.sb.Length = 0;
			while (this.curr < this.end)
			{
				char currChar = this.CurrChar;
				if (Reader.CompareChars(currChar, stopChar, ignoreCase))
				{
					this.DoAutoSkip();
					return this.sb.ToString();
				}
				this.sb.Append(currChar);
				this.curr++;
			}
			this.DoAutoSkip();
			return this.sb.ToString();
		}

		public string ReadToStopChar(char[] chars, bool ignoreCase = false)
		{
			this.DoAutoSkip();
			this.sb.Length = 0;
			while (this.curr < this.end)
			{
				char currChar = this.CurrChar;
				for (int i = 0; i < chars.Length; i++)
				{
					if (Reader.CompareChars(currChar, chars[i], ignoreCase))
					{
						this.DoAutoSkip();
						return this.sb.ToString();
					}
				}
				this.sb.Append(this.CurrChar);
				this.curr++;
			}
			this.DoAutoSkip();
			return this.sb.ToString();
		}

		public string ReadToStopText(string stopText, bool ignoreCase = false)
		{
			this.DoAutoSkip();
			char c = stopText.get_Chars(0);
			this.sb.Length = 0;
			while (this.curr < this.end)
			{
				char currChar = this.CurrChar;
				if (Reader.CompareChars(currChar, c, ignoreCase) && this.IsOnText(stopText, false))
				{
					this.DoAutoSkip();
					return this.sb.ToString();
				}
				this.sb.Append(currChar);
				this.curr++;
			}
			this.DoAutoSkip();
			return this.sb.ToString();
		}

		public string ReadToWhitespace()
		{
			this.DoAutoSkip();
			this.sb.Length = 0;
			while (this.curr < this.end)
			{
				if (this.IsOnWhitespace())
				{
					return this.sb.ToString();
				}
				this.sb.Append(this.CurrChar);
				this.curr++;
			}
			this.DoAutoSkip();
			return this.sb.ToString();
		}

		public string ReadToWhitespaceOrChar(char c)
		{
			this.DoAutoSkip();
			this.sb.Length = 0;
			while (this.curr < this.end)
			{
				if (this.IsOnWhitespace() || this.CurrChar == c)
				{
					return this.sb.ToString();
				}
				this.sb.Append(this.CurrChar);
				this.curr++;
			}
			this.DoAutoSkip();
			return this.sb.ToString();
		}

		public string ReadQuotedString()
		{
			this.DoAutoSkip();
			this.sb.Length = 0;
			if (this.curr < this.end)
			{
				char currChar = this.CurrChar;
				this.curr++;
				while (this.curr < this.end)
				{
					char currChar2 = this.CurrChar;
					this.curr++;
					if (currChar2 == '\\' && this.CurrChar == currChar)
					{
						currChar2 = this.CurrChar;
						this.curr++;
					}
					else if (currChar2 == currChar)
					{
						return this.sb.ToString();
					}
					this.sb.Append(currChar2);
				}
			}
			this.DoAutoSkip();
			return this.sb.ToString();
		}

		public bool IsOnSLComments()
		{
			return this.curr >= this.begin && this.curr < this.end - 1 && this.CurrChar == '/' && this.NextChar == '/';
		}

		public bool IsOnMLComments()
		{
			return this.curr >= this.begin && this.curr < this.end - 1 && this.CurrChar == '/' && this.NextChar == '*';
		}

		public bool IsOnWhitespace()
		{
			return this.curr < this.end && this.CurrChar <= ' ';
		}

		public bool IsOnQuote()
		{
			return (this.curr < this.end && this.CurrChar == '\'') || this.CurrChar == '"';
		}

		public bool IsOnDigit()
		{
			return this.curr < this.end && char.IsDigit(this.CurrChar);
		}

		public bool IsOnLetter()
		{
			return this.curr < this.end && char.IsLetter(this.CurrChar);
		}

		public bool IsOnLetterOrDigit()
		{
			return this.curr < this.end && char.IsLetterOrDigit(this.CurrChar);
		}

		public bool IsOnChar(char c, bool ignoreCase = false)
		{
			return this.curr < this.end && Reader.CompareChars(this.CurrChar, c, ignoreCase);
		}

		public bool IsOnChar(char[] chars, bool ignoreCase = false)
		{
			if (this.curr < this.end)
			{
				char currChar = this.CurrChar;
				for (int i = 0; i < chars.Length; i++)
				{
					if (Reader.CompareChars(currChar, chars[i], ignoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IsOnText(string text, bool ignoreCase = false)
		{
			int length = text.Length;
			return this.Rest >= (long)length && text.IndexOf(text, this.curr, (!ignoreCase) ? 2 : 3) == this.curr;
		}

		private static bool CompareChars(char c1, char c2, bool ignoreCase)
		{
			return (!ignoreCase) ? (c1 == c2) : (char.ToUpperInvariant(c1) == char.ToUpperInvariant(c2));
		}

		public static readonly Reader Instance = new Reader();

		public bool AutoSkipMLComments;

		public bool AutoSkipSLComments;

		public bool AutoSkipWhitespace;

		private string text;

		private int begin;

		private int end;

		private int curr;

		private readonly StringBuilder sb;
	}
}
