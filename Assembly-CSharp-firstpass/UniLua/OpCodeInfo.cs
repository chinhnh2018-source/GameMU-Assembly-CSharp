using System;
using System.Collections.Generic;

namespace UniLua
{
	internal static class OpCodeInfo
	{
		static OpCodeInfo()
		{
			OpCodeInfo.Info.Add(OpCode.OP_MOVE, OpCodeInfo.M(false, true, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_LOADK, OpCodeInfo.M(false, true, OpArgMask.OpArgK, OpArgMask.OpArgN, OpMode.iABx));
			OpCodeInfo.Info.Add(OpCode.OP_LOADKX, OpCodeInfo.M(false, true, OpArgMask.OpArgN, OpArgMask.OpArgN, OpMode.iABx));
			OpCodeInfo.Info.Add(OpCode.OP_LOADBOOL, OpCodeInfo.M(false, true, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_LOADNIL, OpCodeInfo.M(false, true, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_GETUPVAL, OpCodeInfo.M(false, true, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_GETTABUP, OpCodeInfo.M(false, true, OpArgMask.OpArgU, OpArgMask.OpArgK, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_GETTABLE, OpCodeInfo.M(false, true, OpArgMask.OpArgR, OpArgMask.OpArgK, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_SETTABUP, OpCodeInfo.M(false, false, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_SETUPVAL, OpCodeInfo.M(false, false, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_SETTABLE, OpCodeInfo.M(false, false, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_NEWTABLE, OpCodeInfo.M(false, true, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_SELF, OpCodeInfo.M(false, true, OpArgMask.OpArgR, OpArgMask.OpArgK, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_ADD, OpCodeInfo.M(false, true, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_SUB, OpCodeInfo.M(false, true, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_MUL, OpCodeInfo.M(false, true, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_DIV, OpCodeInfo.M(false, true, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_MOD, OpCodeInfo.M(false, true, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_POW, OpCodeInfo.M(false, true, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_UNM, OpCodeInfo.M(false, true, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_NOT, OpCodeInfo.M(false, true, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_LEN, OpCodeInfo.M(false, true, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_CONCAT, OpCodeInfo.M(false, true, OpArgMask.OpArgR, OpArgMask.OpArgR, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_JMP, OpCodeInfo.M(false, false, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx));
			OpCodeInfo.Info.Add(OpCode.OP_EQ, OpCodeInfo.M(true, false, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_LT, OpCodeInfo.M(true, false, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_LE, OpCodeInfo.M(true, false, OpArgMask.OpArgK, OpArgMask.OpArgK, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_TEST, OpCodeInfo.M(true, false, OpArgMask.OpArgN, OpArgMask.OpArgU, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_TESTSET, OpCodeInfo.M(true, true, OpArgMask.OpArgR, OpArgMask.OpArgU, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_CALL, OpCodeInfo.M(false, true, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_TAILCALL, OpCodeInfo.M(false, true, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_RETURN, OpCodeInfo.M(false, false, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_FORLOOP, OpCodeInfo.M(false, true, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx));
			OpCodeInfo.Info.Add(OpCode.OP_FORPREP, OpCodeInfo.M(false, true, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx));
			OpCodeInfo.Info.Add(OpCode.OP_TFORCALL, OpCodeInfo.M(false, false, OpArgMask.OpArgN, OpArgMask.OpArgU, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_TFORLOOP, OpCodeInfo.M(false, true, OpArgMask.OpArgR, OpArgMask.OpArgN, OpMode.iAsBx));
			OpCodeInfo.Info.Add(OpCode.OP_SETLIST, OpCodeInfo.M(false, false, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_CLOSURE, OpCodeInfo.M(false, true, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABx));
			OpCodeInfo.Info.Add(OpCode.OP_VARARG, OpCodeInfo.M(false, true, OpArgMask.OpArgU, OpArgMask.OpArgN, OpMode.iABC));
			OpCodeInfo.Info.Add(OpCode.OP_EXTRAARG, OpCodeInfo.M(false, false, OpArgMask.OpArgU, OpArgMask.OpArgU, OpMode.iAx));
		}

		public static OpCodeMode GetMode(OpCode op)
		{
			return OpCodeInfo.Info[op];
		}

		private static OpCodeMode M(bool t, bool a, OpArgMask b, OpArgMask c, OpMode op)
		{
			return new OpCodeMode
			{
				TMode = t,
				AMode = a,
				BMode = b,
				CMode = c,
				OpMode = op
			};
		}

		private static Dictionary<OpCode, OpCodeMode> Info = new Dictionary<OpCode, OpCodeMode>();
	}
}
