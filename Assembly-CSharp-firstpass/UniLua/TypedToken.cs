using System;

namespace UniLua
{
	public class TypedToken : Token
	{
		public TypedToken(TK type)
		{
			this._Type = type;
		}

		public override int TokenType
		{
			get
			{
				return (int)this._Type;
			}
		}

		public override string ToString()
		{
			return string.Format("TypedToken: {0}", this._Type);
		}

		private TK _Type;
	}
}
