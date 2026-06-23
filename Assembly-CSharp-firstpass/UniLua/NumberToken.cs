using System;

namespace UniLua
{
	public class NumberToken : TypedToken
	{
		public NumberToken(double seminfo) : base(TK.NUMBER)
		{
			this.SemInfo = seminfo;
		}

		public override string ToString()
		{
			return string.Format("NumberToken: {0}", this.SemInfo);
		}

		public double SemInfo;
	}
}
