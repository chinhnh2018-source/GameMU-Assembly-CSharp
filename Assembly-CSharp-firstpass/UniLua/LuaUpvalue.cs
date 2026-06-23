using System;

namespace UniLua
{
	public class LuaUpvalue
	{
		public LuaUpvalue()
		{
			this.Value = new StkId();
			this.Value.V.SetNilValue();
			this.V = this.Value;
		}

		public StkId V;

		public StkId Value;
	}
}
