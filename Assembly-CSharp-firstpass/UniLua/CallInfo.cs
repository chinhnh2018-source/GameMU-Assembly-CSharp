using System;

namespace UniLua
{
	public class CallInfo
	{
		public bool IsLua
		{
			get
			{
				return (this.CallStatus & CallStatus.CIST_LUA) != CallStatus.CIST_NONE;
			}
		}

		public int CurrentPc
		{
			get
			{
				Utl.Assert(this.IsLua);
				return this.SavedPc.Index - 1;
			}
		}

		public CallInfo[] List;

		public int Index;

		public int FuncIndex;

		public int TopIndex;

		public int NumResults;

		public CallStatus CallStatus;

		public CSharpFunctionDelegate ContinueFunc;

		public int Context;

		public int ExtraIndex;

		public bool OldAllowHook;

		public int OldErrFunc;

		public ThreadStatus Status;

		public int BaseIndex;

		public Pointer<Instruction> SavedPc;
	}
}
