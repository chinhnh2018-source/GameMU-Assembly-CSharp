using System;

namespace UniLua
{
	public class ConstructorControl
	{
		public ConstructorControl()
		{
			this.ExpLastItem = new ExpDesc();
		}

		public ExpDesc ExpLastItem;

		public ExpDesc ExpTable;

		public int NumRecord;

		public int NumArray;

		public int NumToStore;
	}
}
