using System;

namespace UniLua
{
	public abstract class Token
	{
		public abstract int TokenType { get; }

		public bool EqualsToToken(Token other)
		{
			return this.TokenType == other.TokenType;
		}

		public bool EqualsToToken(int other)
		{
			return this.TokenType == other;
		}

		public bool EqualsToToken(TK other)
		{
			return this.TokenType == (int)other;
		}
	}
}
