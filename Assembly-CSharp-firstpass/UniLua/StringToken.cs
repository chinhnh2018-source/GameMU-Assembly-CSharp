using System;

namespace UniLua
{
	public class StringToken : TypedToken
	{
		public StringToken(string seminfo) : base(TK.STRING)
		{
			this.SemInfo = seminfo;
		}

		public override string ToString()
		{
			return string.Format("StringToken: {0}", this.SemInfo);
		}

		public string SemInfo;
	}
}
