using System;

namespace UniLua
{
	public class LuaCsClosureValue
	{
		public LuaCsClosureValue(CSharpFunctionDelegate f)
		{
			this.F = f;
		}

		public LuaCsClosureValue(CSharpFunctionDelegate f, int numUpvalues)
		{
			this.F = f;
			this.Upvals = new StkId[numUpvalues];
			for (int i = 0; i < numUpvalues; i++)
			{
				StkId stkId = new StkId();
				this.Upvals[i] = stkId;
				stkId.SetList(this.Upvals);
				stkId.SetIndex(i);
			}
		}

		public CSharpFunctionDelegate F;

		public StkId[] Upvals;
	}
}
