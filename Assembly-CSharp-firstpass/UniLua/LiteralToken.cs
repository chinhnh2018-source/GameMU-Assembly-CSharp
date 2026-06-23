using System;

namespace UniLua
{
	public class LiteralToken : Token
	{
		public LiteralToken(int literal)
		{
			this._Literal = literal;
		}

		public override int TokenType
		{
			get
			{
				return this._Literal;
			}
		}

		public override string ToString()
		{
			return string.Format("LiteralToken: {0}", this._Literal);
		}

		private int _Literal;
	}
}
