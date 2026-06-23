using System;

namespace UniLua
{
	public class ExpDesc
	{
		public void CopyFrom(ExpDesc e)
		{
			this.Kind = e.Kind;
			this.Info = e.Info;
			this.Ind = e.Ind;
			this.NumberValue = e.NumberValue;
			this.ExitTrue = e.ExitTrue;
			this.ExitFalse = e.ExitFalse;
		}

		public ExpKind Kind;

		public int Info;

		public ExpDesc.IndData Ind;

		public double NumberValue;

		public int ExitTrue;

		public int ExitFalse;

		public struct IndData
		{
			public int T;

			public int Idx;

			public ExpKind Vt;
		}
	}
}
