using System;
using System.Collections.Generic;

namespace UniLua
{
	public class FuncState
	{
		public FuncState()
		{
			this.Proto = new LuaProto();
			this.H = new Dictionary<TValue, int>();
			this.NumActVar = 0;
			this.FreeReg = 0;
		}

		public Pointer<Instruction> GetCode(ExpDesc e)
		{
			return new Pointer<Instruction>(this.Proto.Code, e.Info);
		}

		public FuncState Prev;

		public BlockCnt Block;

		public LuaProto Proto;

		public LuaState State;

		public LLex Lexer;

		public Dictionary<TValue, int> H;

		public int NumActVar;

		public int FreeReg;

		public int Pc;

		public int LastTarget;

		public int Jpc;

		public int FirstLocal;
	}
}
