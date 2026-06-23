using System;

namespace UniLua
{
	public class LuaLClosureValue
	{
		public LuaLClosureValue(LuaProto p)
		{
			this.Proto = p;
			this.Upvals = new LuaUpvalue[p.Upvalues.Count];
			for (int i = 0; i < p.Upvalues.Count; i++)
			{
				this.Upvals[i] = new LuaUpvalue();
			}
		}

		public LuaProto Proto;

		public LuaUpvalue[] Upvals;
	}
}
