using System;

namespace UniLua
{
	public class LuaRuntimeException : Exception
	{
		public LuaRuntimeException(ThreadStatus errCode)
		{
			this.ErrCode = errCode;
		}

		public ThreadStatus ErrCode { get; private set; }
	}
}
