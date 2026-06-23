using System;

namespace UniLua
{
	public class GlobalState
	{
		public GlobalState(LuaState state)
		{
			this.MainThread = state;
			this.Registry = new StkId();
			this.UpvalHead = new LuaUpvalue();
			this.MetaTables = new LuaTable[10];
		}

		public StkId Registry;

		public LuaUpvalue UpvalHead;

		public LuaTable[] MetaTables;

		public LuaState MainThread;
	}
}
