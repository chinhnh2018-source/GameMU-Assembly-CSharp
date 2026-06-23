using System;

namespace UniLua
{
	public class LHSAssign
	{
		public LHSAssign()
		{
			this.Prev = null;
			this.Exp = new ExpDesc();
		}

		public LHSAssign Prev;

		public ExpDesc Exp;
	}
}
