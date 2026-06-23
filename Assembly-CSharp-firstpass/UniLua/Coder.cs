using System;

namespace UniLua
{
	public static class Coder
	{
		private static void FreeReg(FuncState fs, int reg)
		{
			if (!Instruction.ISK(reg) && reg >= fs.NumActVar)
			{
				fs.FreeReg--;
				Utl.Assert(reg == fs.FreeReg);
			}
		}

		private static void FreeExp(FuncState fs, ExpDesc e)
		{
			if (e.Kind == ExpKind.VNONRELOC)
			{
				Coder.FreeReg(fs, e.Info);
			}
		}

		private static bool IsNumeral(ExpDesc e)
		{
			return e.Kind == ExpKind.VKNUM && e.ExitTrue == -1 && e.ExitFalse == -1;
		}

		private static bool ConstFolding(OpCode op, ExpDesc e1, ExpDesc e2)
		{
			if (!Coder.IsNumeral(e1) || !Coder.IsNumeral(e2))
			{
				return false;
			}
			if ((op == OpCode.OP_DIV || op == OpCode.OP_MOD) && e2.NumberValue == 0.0)
			{
				return false;
			}
			switch (op)
			{
			case OpCode.OP_ADD:
				e1.NumberValue += e2.NumberValue;
				break;
			case OpCode.OP_SUB:
				e1.NumberValue -= e2.NumberValue;
				break;
			case OpCode.OP_MUL:
				e1.NumberValue *= e2.NumberValue;
				break;
			case OpCode.OP_DIV:
				e1.NumberValue /= e2.NumberValue;
				break;
			case OpCode.OP_MOD:
				e1.NumberValue %= e2.NumberValue;
				break;
			case OpCode.OP_POW:
				e1.NumberValue = Math.Pow(e1.NumberValue, e2.NumberValue);
				break;
			case OpCode.OP_UNM:
				e1.NumberValue = -e1.NumberValue;
				break;
			default:
				throw new Exception("ConstFolding unknown op" + op);
			}
			return true;
		}

		public static void FixLine(FuncState fs, int line)
		{
			fs.Proto.LineInfo[fs.Pc - 1] = line;
		}

		private static void CodeArith(FuncState fs, OpCode op, ExpDesc e1, ExpDesc e2, int line)
		{
			if (Coder.ConstFolding(op, e1, e2))
			{
				return;
			}
			int num = (op == OpCode.OP_UNM || op == OpCode.OP_LEN) ? 0 : Coder.Exp2RK(fs, e2);
			int num2 = Coder.Exp2RK(fs, e1);
			if (num2 > num)
			{
				Coder.FreeExp(fs, e1);
				Coder.FreeExp(fs, e2);
			}
			else
			{
				Coder.FreeExp(fs, e2);
				Coder.FreeExp(fs, e1);
			}
			e1.Info = Coder.CodeABC(fs, op, 0, num2, num);
			e1.Kind = ExpKind.VRELOCABLE;
			Coder.FixLine(fs, line);
		}

		public static bool TestTMode(OpCode op)
		{
			return OpCodeInfo.GetMode(op).TMode;
		}

		public static bool TestAMode(OpCode op)
		{
			return OpCodeInfo.GetMode(op).AMode;
		}

		private static void FixJump(FuncState fs, int pc, int dest)
		{
			Instruction instruction = fs.Proto.Code[pc];
			int num = dest - (pc + 1);
			Utl.Assert(dest != -1);
			if (Math.Abs(num) > 131071)
			{
				fs.Lexer.SyntaxError("control structure too long");
			}
			instruction.SETARG_sBx(num);
			fs.Proto.Code[pc] = instruction;
		}

		public static int GetLabel(FuncState fs)
		{
			fs.LastTarget = fs.Pc;
			return fs.Pc;
		}

		private static int GetJump(FuncState fs, int pc)
		{
			int arg_sBx = fs.Proto.Code[pc].GETARG_sBx();
			if (arg_sBx == -1)
			{
				return -1;
			}
			return pc + 1 + arg_sBx;
		}

		private static Pointer<Instruction> GetJumpControl(FuncState fs, int pc)
		{
			Pointer<Instruction> pointer = new Pointer<Instruction>(fs.Proto.Code, pc);
			if (pc >= 1 && Coder.TestTMode((pointer - 1).Value.GET_OPCODE()))
			{
				return pointer - 1;
			}
			return pointer;
		}

		private static bool NeedValue(FuncState fs, int list)
		{
			while (list != -1)
			{
				if (Coder.GetJumpControl(fs, list).Value.GET_OPCODE() != OpCode.OP_TESTSET)
				{
					return true;
				}
				list = Coder.GetJump(fs, list);
			}
			return false;
		}

		private static bool PatchTestReg(FuncState fs, int node, int reg)
		{
			Pointer<Instruction> jumpControl = Coder.GetJumpControl(fs, node);
			if (jumpControl.Value.GET_OPCODE() != OpCode.OP_TESTSET)
			{
				return false;
			}
			if (reg != 255 && reg != jumpControl.Value.GETARG_B())
			{
				jumpControl.Value = jumpControl.Value.SETARG_A(reg);
			}
			else
			{
				jumpControl.Value = Instruction.CreateABC(OpCode.OP_TEST, jumpControl.Value.GETARG_B(), 0, jumpControl.Value.GETARG_C());
			}
			return true;
		}

		private static void RemoveValues(FuncState fs, int list)
		{
			while (list != -1)
			{
				Coder.PatchTestReg(fs, list, 255);
				list = Coder.GetJump(fs, list);
			}
		}

		private static void PatchListAux(FuncState fs, int list, int vtarget, int reg, int dtarget)
		{
			while (list != -1)
			{
				int jump = Coder.GetJump(fs, list);
				if (Coder.PatchTestReg(fs, list, reg))
				{
					Coder.FixJump(fs, list, vtarget);
				}
				else
				{
					Coder.FixJump(fs, list, dtarget);
				}
				list = jump;
			}
		}

		private static void DischargeJpc(FuncState fs)
		{
			Coder.PatchListAux(fs, fs.Jpc, fs.Pc, 255, fs.Pc);
			fs.Jpc = -1;
		}

		private static void InvertJump(FuncState fs, ExpDesc e)
		{
			Pointer<Instruction> jumpControl = Coder.GetJumpControl(fs, e.Info);
			Utl.Assert(Coder.TestTMode(jumpControl.Value.GET_OPCODE()) && jumpControl.Value.GET_OPCODE() != OpCode.OP_TESTSET && jumpControl.Value.GET_OPCODE() != OpCode.OP_TEST);
			jumpControl.Value = jumpControl.Value.SETARG_A((jumpControl.Value.GETARG_A() != 0) ? 0 : 1);
		}

		private static int JumpOnCond(FuncState fs, ExpDesc e, bool cond)
		{
			if (e.Kind == ExpKind.VRELOCABLE)
			{
				Instruction value = fs.GetCode(e).Value;
				if (value.GET_OPCODE() == OpCode.OP_NOT)
				{
					fs.Pc--;
					return Coder.CondJump(fs, OpCode.OP_TEST, value.GETARG_B(), 0, (!cond) ? 1 : 0);
				}
			}
			Coder.Discharge2AnyReg(fs, e);
			Coder.FreeExp(fs, e);
			return Coder.CondJump(fs, OpCode.OP_TESTSET, 255, e.Info, (!cond) ? 0 : 1);
		}

		public static void GoIfTrue(FuncState fs, ExpDesc e)
		{
			Coder.DischargeVars(fs, e);
			ExpKind kind = e.Kind;
			int l;
			switch (kind)
			{
			case ExpKind.VTRUE:
			case ExpKind.VK:
			case ExpKind.VKNUM:
				l = -1;
				break;
			default:
				if (kind != ExpKind.VJMP)
				{
					l = Coder.JumpOnCond(fs, e, false);
				}
				else
				{
					Coder.InvertJump(fs, e);
					l = e.Info;
				}
				break;
			}
			e.ExitFalse = Coder.Concat(fs, e.ExitFalse, l);
			Coder.PatchToHere(fs, e.ExitTrue);
			e.ExitTrue = -1;
		}

		public static void GoIfFalse(FuncState fs, ExpDesc e)
		{
			Coder.DischargeVars(fs, e);
			ExpKind kind = e.Kind;
			int l;
			switch (kind)
			{
			case ExpKind.VNIL:
			case ExpKind.VFALSE:
				l = -1;
				break;
			default:
				if (kind != ExpKind.VJMP)
				{
					l = Coder.JumpOnCond(fs, e, true);
				}
				else
				{
					l = e.Info;
				}
				break;
			}
			e.ExitTrue = Coder.Concat(fs, e.ExitTrue, l);
			Coder.PatchToHere(fs, e.ExitFalse);
			e.ExitFalse = -1;
		}

		private static void CodeNot(FuncState fs, ExpDesc e)
		{
			Coder.DischargeVars(fs, e);
			switch (e.Kind)
			{
			case ExpKind.VNIL:
			case ExpKind.VFALSE:
				e.Kind = ExpKind.VTRUE;
				goto IL_B7;
			case ExpKind.VTRUE:
			case ExpKind.VK:
			case ExpKind.VKNUM:
				e.Kind = ExpKind.VFALSE;
				goto IL_B7;
			case ExpKind.VNONRELOC:
			case ExpKind.VRELOCABLE:
				Coder.Discharge2AnyReg(fs, e);
				Coder.FreeExp(fs, e);
				e.Info = Coder.CodeABC(fs, OpCode.OP_NOT, 0, e.Info, 0);
				e.Kind = ExpKind.VRELOCABLE;
				goto IL_B7;
			case ExpKind.VJMP:
				Coder.InvertJump(fs, e);
				goto IL_B7;
			}
			throw new Exception("CodeNot unknown e.Kind:" + e.Kind);
			IL_B7:
			int exitFalse = e.ExitFalse;
			e.ExitFalse = e.ExitTrue;
			e.ExitTrue = exitFalse;
			Coder.RemoveValues(fs, e.ExitFalse);
			Coder.RemoveValues(fs, e.ExitTrue);
		}

		private static void CodeComp(FuncState fs, OpCode op, int cond, ExpDesc e1, ExpDesc e2)
		{
			int num = Coder.Exp2RK(fs, e1);
			int num2 = Coder.Exp2RK(fs, e2);
			Coder.FreeExp(fs, e2);
			Coder.FreeExp(fs, e1);
			if (cond == 0 && op != OpCode.OP_EQ)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
				cond = 1;
			}
			e1.Info = Coder.CondJump(fs, op, cond, num, num2);
			e1.Kind = ExpKind.VJMP;
		}

		public static void Prefix(FuncState fs, UnOpr op, ExpDesc e, int line)
		{
			ExpDesc expDesc = new ExpDesc();
			expDesc.ExitTrue = -1;
			expDesc.ExitFalse = -1;
			expDesc.Kind = ExpKind.VKNUM;
			expDesc.NumberValue = 0.0;
			switch (op)
			{
			case UnOpr.MINUS:
				if (Coder.IsNumeral(e))
				{
					e.NumberValue = -e.NumberValue;
				}
				else
				{
					Coder.Exp2AnyReg(fs, e);
					Coder.CodeArith(fs, OpCode.OP_UNM, e, expDesc, line);
				}
				break;
			case UnOpr.NOT:
				Coder.CodeNot(fs, e);
				break;
			case UnOpr.LEN:
				Coder.Exp2AnyReg(fs, e);
				Coder.CodeArith(fs, OpCode.OP_LEN, e, expDesc, line);
				break;
			default:
				throw new Exception("[Coder]Prefix Unknown UnOpr:" + op);
			}
		}

		public static void Infix(FuncState fs, BinOpr op, ExpDesc e)
		{
			switch (op)
			{
			case BinOpr.ADD:
			case BinOpr.SUB:
			case BinOpr.MUL:
			case BinOpr.DIV:
			case BinOpr.MOD:
			case BinOpr.POW:
				if (!Coder.IsNumeral(e))
				{
					Coder.Exp2RK(fs, e);
				}
				return;
			case BinOpr.CONCAT:
				Coder.Exp2NextReg(fs, e);
				return;
			case BinOpr.AND:
				Coder.GoIfTrue(fs, e);
				return;
			case BinOpr.OR:
				Coder.GoIfFalse(fs, e);
				return;
			}
			Coder.Exp2RK(fs, e);
		}

		public static void Posfix(FuncState fs, BinOpr op, ExpDesc e1, ExpDesc e2, int line)
		{
			switch (op)
			{
			case BinOpr.ADD:
				Coder.CodeArith(fs, OpCode.OP_ADD, e1, e2, line);
				break;
			case BinOpr.SUB:
				Coder.CodeArith(fs, OpCode.OP_SUB, e1, e2, line);
				break;
			case BinOpr.MUL:
				Coder.CodeArith(fs, OpCode.OP_MUL, e1, e2, line);
				break;
			case BinOpr.DIV:
				Coder.CodeArith(fs, OpCode.OP_DIV, e1, e2, line);
				break;
			case BinOpr.MOD:
				Coder.CodeArith(fs, OpCode.OP_MOD, e1, e2, line);
				break;
			case BinOpr.POW:
				Coder.CodeArith(fs, OpCode.OP_POW, e1, e2, line);
				break;
			case BinOpr.CONCAT:
			{
				Coder.Exp2Val(fs, e2);
				Pointer<Instruction> code = fs.GetCode(e2);
				if (e2.Kind == ExpKind.VRELOCABLE && code.Value.GET_OPCODE() == OpCode.OP_CONCAT)
				{
					Utl.Assert(e1.Info == code.Value.GETARG_B() - 1);
					Coder.FreeExp(fs, e1);
					code.Value = code.Value.SETARG_B(e1.Info);
					e1.Kind = ExpKind.VRELOCABLE;
					e1.Info = e2.Info;
				}
				else
				{
					Coder.Exp2NextReg(fs, e2);
					Coder.CodeArith(fs, OpCode.OP_CONCAT, e1, e2, line);
				}
				break;
			}
			case BinOpr.EQ:
				Coder.CodeComp(fs, OpCode.OP_EQ, 1, e1, e2);
				break;
			case BinOpr.LT:
				Coder.CodeComp(fs, OpCode.OP_LT, 1, e1, e2);
				break;
			case BinOpr.LE:
				Coder.CodeComp(fs, OpCode.OP_LE, 1, e1, e2);
				break;
			case BinOpr.NE:
				Coder.CodeComp(fs, OpCode.OP_EQ, 0, e1, e2);
				break;
			case BinOpr.GT:
				Coder.CodeComp(fs, OpCode.OP_LT, 0, e1, e2);
				break;
			case BinOpr.GE:
				Coder.CodeComp(fs, OpCode.OP_LE, 0, e1, e2);
				break;
			case BinOpr.AND:
				Utl.Assert(e1.ExitTrue == -1);
				Coder.DischargeVars(fs, e2);
				e2.ExitFalse = Coder.Concat(fs, e2.ExitFalse, e1.ExitFalse);
				e1.CopyFrom(e2);
				break;
			case BinOpr.OR:
				Utl.Assert(e1.ExitFalse == -1);
				Coder.DischargeVars(fs, e2);
				e2.ExitTrue = Coder.Concat(fs, e2.ExitTrue, e1.ExitTrue);
				e1.CopyFrom(e2);
				break;
			default:
				Utl.Assert(false);
				break;
			}
		}

		public static int Jump(FuncState fs)
		{
			int jpc = fs.Jpc;
			fs.Jpc = -1;
			int l = Coder.CodeAsBx(fs, OpCode.OP_JMP, 0, -1);
			return Coder.Concat(fs, l, jpc);
		}

		public static void JumpTo(FuncState fs, int target)
		{
			Coder.PatchList(fs, Coder.Jump(fs), target);
		}

		public static void Ret(FuncState fs, int first, int nret)
		{
			Coder.CodeABC(fs, OpCode.OP_RETURN, first, nret + 1, 0);
		}

		private static int CondJump(FuncState fs, OpCode op, int a, int b, int c)
		{
			Coder.CodeABC(fs, op, a, b, c);
			return Coder.Jump(fs);
		}

		public static void PatchList(FuncState fs, int list, int target)
		{
			if (target == fs.Pc)
			{
				Coder.PatchToHere(fs, list);
			}
			else
			{
				Utl.Assert(target < fs.Pc);
				Coder.PatchListAux(fs, list, target, 255, target);
			}
		}

		public static void PatchClose(FuncState fs, int list, int level)
		{
			level++;
			while (list != -1)
			{
				int jump = Coder.GetJump(fs, list);
				Pointer<Instruction> pointer = new Pointer<Instruction>(fs.Proto.Code, list);
				Utl.Assert(pointer.Value.GET_OPCODE() == OpCode.OP_JMP && (pointer.Value.GETARG_A() == 0 || pointer.Value.GETARG_A() >= level));
				pointer.Value = pointer.Value.SETARG_A(level);
				list = jump;
			}
		}

		public static void PatchToHere(FuncState fs, int list)
		{
			Coder.GetLabel(fs);
			fs.Jpc = Coder.Concat(fs, fs.Jpc, list);
		}

		public static int Concat(FuncState fs, int l1, int l2)
		{
			if (l2 == -1)
			{
				return l1;
			}
			if (l1 == -1)
			{
				return l2;
			}
			int pc = l1;
			for (int jump = Coder.GetJump(fs, pc); jump != -1; jump = Coder.GetJump(fs, pc))
			{
				pc = jump;
			}
			Coder.FixJump(fs, pc, l2);
			return l1;
		}

		public static int StringK(FuncState fs, string s)
		{
			TValue tvalue = default(TValue);
			tvalue.SetSValue(s);
			return Coder.AddK(fs, ref tvalue, ref tvalue);
		}

		public static int NumberK(FuncState fs, double r)
		{
			TValue tvalue = default(TValue);
			tvalue.SetNValue(r);
			return Coder.AddK(fs, ref tvalue, ref tvalue);
		}

		private static int BoolK(FuncState fs, bool b)
		{
			TValue tvalue = default(TValue);
			tvalue.SetBValue(b);
			return Coder.AddK(fs, ref tvalue, ref tvalue);
		}

		private static int NilK(FuncState fs)
		{
			TValue tvalue = default(TValue);
			tvalue.SetNilValue();
			return Coder.AddK(fs, ref tvalue, ref tvalue);
		}

		public static int AddK(FuncState fs, ref TValue key, ref TValue v)
		{
			int count;
			if (fs.H.TryGetValue(key, ref count))
			{
				return count;
			}
			count = fs.Proto.K.Count;
			fs.H.Add(key, count);
			StkId stkId = new StkId();
			stkId.V.SetObj(ref v);
			fs.Proto.K.Add(stkId);
			return count;
		}

		public static void Indexed(FuncState fs, ExpDesc t, ExpDesc k)
		{
			t.Ind.T = t.Info;
			t.Ind.Idx = Coder.Exp2RK(fs, k);
			t.Ind.Vt = ((t.Kind != ExpKind.VUPVAL) ? ExpKind.VLOCAL : ExpKind.VUPVAL);
			t.Kind = ExpKind.VINDEXED;
		}

		private static bool HasJumps(ExpDesc e)
		{
			return e.ExitTrue != e.ExitFalse;
		}

		private static int CodeLabel(FuncState fs, int a, int b, int jump)
		{
			Coder.GetLabel(fs);
			return Coder.CodeABC(fs, OpCode.OP_LOADBOOL, a, b, jump);
		}

		private static void Discharge2Reg(FuncState fs, ExpDesc e, int reg)
		{
			Coder.DischargeVars(fs, e);
			switch (e.Kind)
			{
			case ExpKind.VNIL:
				Coder.CodeNil(fs, reg, 1);
				goto IL_105;
			case ExpKind.VTRUE:
			case ExpKind.VFALSE:
				Coder.CodeABC(fs, OpCode.OP_LOADBOOL, reg, (e.Kind != ExpKind.VTRUE) ? 0 : 1, 0);
				goto IL_105;
			case ExpKind.VK:
				Coder.CodeK(fs, reg, e.Info);
				goto IL_105;
			case ExpKind.VKNUM:
				Coder.CodeK(fs, reg, Coder.NumberK(fs, e.NumberValue));
				goto IL_105;
			case ExpKind.VNONRELOC:
				if (reg != e.Info)
				{
					Coder.CodeABC(fs, OpCode.OP_MOVE, reg, e.Info, 0);
				}
				goto IL_105;
			case ExpKind.VRELOCABLE:
			{
				Pointer<Instruction> code = fs.GetCode(e);
				code.Value = code.Value.SETARG_A(reg);
				goto IL_105;
			}
			}
			Utl.Assert(e.Kind == ExpKind.VVOID || e.Kind == ExpKind.VJMP);
			return;
			IL_105:
			e.Info = reg;
			e.Kind = ExpKind.VNONRELOC;
		}

		public static void CheckStack(FuncState fs, int n)
		{
			int num = fs.FreeReg + n;
			if (num > (int)fs.Proto.MaxStackSize)
			{
				if (num >= 250)
				{
					fs.Lexer.SyntaxError("function or expression too complex");
				}
				fs.Proto.MaxStackSize = (byte)num;
			}
		}

		public static void ReserveRegs(FuncState fs, int n)
		{
			Coder.CheckStack(fs, n);
			fs.FreeReg += n;
		}

		private static void Discharge2AnyReg(FuncState fs, ExpDesc e)
		{
			if (e.Kind != ExpKind.VNONRELOC)
			{
				Coder.ReserveRegs(fs, 1);
				Coder.Discharge2Reg(fs, e, fs.FreeReg - 1);
			}
		}

		private static void Exp2Reg(FuncState fs, ExpDesc e, int reg)
		{
			Coder.Discharge2Reg(fs, e, reg);
			if (e.Kind == ExpKind.VJMP)
			{
				e.ExitTrue = Coder.Concat(fs, e.ExitTrue, e.Info);
			}
			if (Coder.HasJumps(e))
			{
				int dtarget = -1;
				int dtarget2 = -1;
				if (Coder.NeedValue(fs, e.ExitTrue) || Coder.NeedValue(fs, e.ExitFalse))
				{
					int list = (e.Kind != ExpKind.VJMP) ? Coder.Jump(fs) : -1;
					dtarget = Coder.CodeLabel(fs, reg, 0, 1);
					dtarget2 = Coder.CodeLabel(fs, reg, 1, 0);
					Coder.PatchToHere(fs, list);
				}
				int label = Coder.GetLabel(fs);
				Coder.PatchListAux(fs, e.ExitFalse, label, reg, dtarget);
				Coder.PatchListAux(fs, e.ExitTrue, label, reg, dtarget2);
			}
			e.ExitFalse = -1;
			e.ExitTrue = -1;
			e.Info = reg;
			e.Kind = ExpKind.VNONRELOC;
		}

		public static void Exp2NextReg(FuncState fs, ExpDesc e)
		{
			Coder.DischargeVars(fs, e);
			Coder.FreeExp(fs, e);
			Coder.ReserveRegs(fs, 1);
			Coder.Exp2Reg(fs, e, fs.FreeReg - 1);
		}

		public static void Exp2Val(FuncState fs, ExpDesc e)
		{
			if (Coder.HasJumps(e))
			{
				Coder.Exp2AnyReg(fs, e);
			}
			else
			{
				Coder.DischargeVars(fs, e);
			}
		}

		public static int Exp2RK(FuncState fs, ExpDesc e)
		{
			Coder.Exp2Val(fs, e);
			switch (e.Kind)
			{
			case ExpKind.VNIL:
			case ExpKind.VTRUE:
			case ExpKind.VFALSE:
				if (fs.Proto.K.Count <= 255)
				{
					e.Info = ((e.Kind != ExpKind.VNIL) ? Coder.BoolK(fs, e.Kind == ExpKind.VTRUE) : Coder.NilK(fs));
					e.Kind = ExpKind.VK;
					return Instruction.RKASK(e.Info);
				}
				break;
			case ExpKind.VK:
			case ExpKind.VKNUM:
				if (e.Kind == ExpKind.VKNUM)
				{
					e.Info = Coder.NumberK(fs, e.NumberValue);
					e.Kind = ExpKind.VK;
				}
				if (e.Info <= 255)
				{
					return Instruction.RKASK(e.Info);
				}
				break;
			}
			return Coder.Exp2AnyReg(fs, e);
		}

		public static int Exp2AnyReg(FuncState fs, ExpDesc e)
		{
			Coder.DischargeVars(fs, e);
			if (e.Kind == ExpKind.VNONRELOC)
			{
				if (!Coder.HasJumps(e))
				{
					return e.Info;
				}
				if (e.Info >= fs.NumActVar)
				{
					Coder.Exp2Reg(fs, e, e.Info);
					return e.Info;
				}
			}
			Coder.Exp2NextReg(fs, e);
			return e.Info;
		}

		public static void Exp2AnyRegUp(FuncState fs, ExpDesc e)
		{
			if (e.Kind != ExpKind.VUPVAL || Coder.HasJumps(e))
			{
				Coder.Exp2AnyReg(fs, e);
			}
		}

		public static void DischargeVars(FuncState fs, ExpDesc e)
		{
			switch (e.Kind)
			{
			case ExpKind.VLOCAL:
				e.Kind = ExpKind.VNONRELOC;
				break;
			case ExpKind.VUPVAL:
				e.Info = Coder.CodeABC(fs, OpCode.OP_GETUPVAL, 0, e.Info, 0);
				e.Kind = ExpKind.VRELOCABLE;
				break;
			case ExpKind.VINDEXED:
			{
				OpCode op = OpCode.OP_GETTABUP;
				Coder.FreeReg(fs, e.Ind.Idx);
				if (e.Ind.Vt == ExpKind.VLOCAL)
				{
					Coder.FreeReg(fs, e.Ind.T);
					op = OpCode.OP_GETTABLE;
				}
				e.Info = Coder.CodeABC(fs, op, 0, e.Ind.T, e.Ind.Idx);
				e.Kind = ExpKind.VRELOCABLE;
				break;
			}
			case ExpKind.VCALL:
			case ExpKind.VVARARG:
				Coder.SetOneRet(fs, e);
				break;
			}
		}

		public static void SetReturns(FuncState fs, ExpDesc e, int nResults)
		{
			if (e.Kind == ExpKind.VCALL)
			{
				Pointer<Instruction> code = fs.GetCode(e);
				code.Value = code.Value.SETARG_C(nResults + 1);
			}
			else if (e.Kind == ExpKind.VVARARG)
			{
				Pointer<Instruction> code2 = fs.GetCode(e);
				code2.Value = code2.Value.SETARG_B(nResults + 1).SETARG_A(fs.FreeReg);
				Coder.ReserveRegs(fs, 1);
			}
		}

		public static void SetMultiRet(FuncState fs, ExpDesc e)
		{
			Coder.SetReturns(fs, e, -1);
		}

		public static void SetOneRet(FuncState fs, ExpDesc e)
		{
			if (e.Kind == ExpKind.VCALL)
			{
				e.Kind = ExpKind.VNONRELOC;
				e.Info = fs.GetCode(e).Value.GETARG_A();
			}
			else if (e.Kind == ExpKind.VVARARG)
			{
				Pointer<Instruction> code = fs.GetCode(e);
				code.Value = code.Value.SETARG_B(2);
				e.Kind = ExpKind.VRELOCABLE;
			}
		}

		public static void StoreVar(FuncState fs, ExpDesc v, ExpDesc e)
		{
			switch (v.Kind)
			{
			case ExpKind.VLOCAL:
				Coder.FreeExp(fs, e);
				Coder.Exp2Reg(fs, e, v.Info);
				break;
			case ExpKind.VUPVAL:
			{
				int a = Coder.Exp2AnyReg(fs, e);
				Coder.CodeABC(fs, OpCode.OP_SETUPVAL, a, v.Info, 0);
				break;
			}
			case ExpKind.VINDEXED:
			{
				OpCode op = (v.Ind.Vt != ExpKind.VLOCAL) ? OpCode.OP_SETTABUP : OpCode.OP_SETTABLE;
				int c = Coder.Exp2RK(fs, e);
				Coder.CodeABC(fs, op, v.Ind.T, v.Ind.Idx, c);
				break;
			}
			default:
				throw new NotImplementedException("invalid var kind to store");
			}
			Coder.FreeExp(fs, e);
		}

		public static void Self(FuncState fs, ExpDesc e, ExpDesc key)
		{
			Coder.Exp2AnyReg(fs, e);
			int info = e.Info;
			Coder.FreeExp(fs, e);
			e.Info = fs.FreeReg;
			e.Kind = ExpKind.VNONRELOC;
			Coder.ReserveRegs(fs, 2);
			Coder.CodeABC(fs, OpCode.OP_SELF, e.Info, info, Coder.Exp2RK(fs, key));
			Coder.FreeExp(fs, key);
		}

		public static void SetList(FuncState fs, int t, int nelems, int tostore)
		{
			int num = (nelems - 1) / 50 + 1;
			int b = (tostore != -1) ? tostore : 0;
			Utl.Assert(tostore != 0);
			if (num <= 511)
			{
				Coder.CodeABC(fs, OpCode.OP_SETLIST, t, b, num);
			}
			else if (num <= 67108863)
			{
				Coder.CodeABC(fs, OpCode.OP_SETLIST, t, b, 0);
				Coder.CodeExtraArg(fs, num);
			}
			else
			{
				fs.Lexer.SyntaxError("constructor too long");
			}
			fs.FreeReg = t + 1;
		}

		public static void CodeNil(FuncState fs, int from, int n)
		{
			int num = from + n - 1;
			if (fs.Pc > fs.LastTarget)
			{
				Pointer<Instruction> pointer = new Pointer<Instruction>(fs.Proto.Code, fs.Pc - 1);
				if (pointer.Value.GET_OPCODE() == OpCode.OP_LOADNIL)
				{
					int arg_A = pointer.Value.GETARG_A();
					int num2 = arg_A + pointer.Value.GETARG_B();
					if ((arg_A <= from && from <= num2 + 1) || (from <= arg_A && arg_A <= num + 1))
					{
						if (arg_A < from)
						{
							from = arg_A;
						}
						if (num2 > num)
						{
							num = num2;
						}
						pointer.Value = pointer.Value.SETARG_A(from);
						pointer.Value = pointer.Value.SETARG_B(num - from);
						return;
					}
				}
			}
			Coder.CodeABC(fs, OpCode.OP_LOADNIL, from, n - 1, 0);
		}

		private static int CodeExtraArg(FuncState fs, int a)
		{
			Utl.Assert(a <= 67108863);
			return Coder.Code(fs, Instruction.CreateAx(OpCode.OP_EXTRAARG, a));
		}

		public static int CodeK(FuncState fs, int reg, int k)
		{
			if (k <= 262143)
			{
				return Coder.CodeABx(fs, OpCode.OP_LOADK, reg, (uint)k);
			}
			int result = Coder.CodeABx(fs, OpCode.OP_LOADKX, reg, 0U);
			Coder.CodeExtraArg(fs, k);
			return result;
		}

		public static int CodeAsBx(FuncState fs, OpCode op, int a, int sBx)
		{
			return Coder.CodeABx(fs, op, a, (uint)(sBx + 131071));
		}

		public static int CodeABx(FuncState fs, OpCode op, int a, uint bc)
		{
			OpCodeMode mode = OpCodeInfo.GetMode(op);
			Utl.Assert(mode.OpMode == OpMode.iABx || mode.OpMode == OpMode.iAsBx);
			Utl.Assert(mode.CMode == OpArgMask.OpArgN);
			Utl.Assert(a < 255 & bc <= 262143U);
			return Coder.Code(fs, Instruction.CreateABx(op, a, bc));
		}

		public static int CodeABC(FuncState fs, OpCode op, int a, int b, int c)
		{
			return Coder.Code(fs, Instruction.CreateABC(op, a, b, c));
		}

		public static int Code(FuncState fs, Instruction i)
		{
			Coder.DischargeJpc(fs);
			while (fs.Proto.Code.Count <= fs.Pc)
			{
				fs.Proto.Code.Add(new Instruction(2147483645U));
			}
			fs.Proto.Code[fs.Pc] = i;
			while (fs.Proto.LineInfo.Count <= fs.Pc)
			{
				fs.Proto.LineInfo.Add(2147483645);
			}
			fs.Proto.LineInfo[fs.Pc] = fs.Lexer.LastLine;
			return fs.Pc++;
		}

		public const int NO_JUMP = -1;

		private const int NO_REG = 255;
	}
}
