using System;
using System.Collections.Generic;

namespace UniLua
{
	public class LuaProto
	{
		public LuaProto()
		{
			this.Code = new List<Instruction>();
			this.K = new List<StkId>();
			this.P = new List<LuaProto>();
			this.Upvalues = new List<UpvalDesc>();
			this.LineInfo = new List<int>();
			this.LocVars = new List<LocVar>();
		}

		public int GetFuncLine(int pc)
		{
			return (0 > pc || pc >= this.LineInfo.Count) ? 0 : this.LineInfo[pc];
		}

		public List<Instruction> Code;

		public List<StkId> K;

		public List<LuaProto> P;

		public List<UpvalDesc> Upvalues;

		public int LineDefined;

		public int LastLineDefined;

		public int NumParams;

		public bool IsVarArg;

		public byte MaxStackSize;

		public string Source;

		public List<int> LineInfo;

		public List<LocVar> LocVars;
	}
}
