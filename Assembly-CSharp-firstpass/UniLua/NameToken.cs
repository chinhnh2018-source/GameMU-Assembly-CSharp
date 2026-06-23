using System;

namespace UniLua
{
	public class NameToken : TypedToken
	{
		public NameToken(string seminfo) : base(TK.NAME)
		{
			this.SemInfo = seminfo;
		}

		public override string ToString()
		{
			return string.Format("NameToken: {0}", this.SemInfo);
		}

		public string SemInfo;
	}
}
