using System;
using System.Collections.Generic;
using UniLua.Tools;

namespace UniLua
{
	public class Parser
	{
		private Parser()
		{
			this.ActVars = new List<VarDesc>();
			this.PendingGotos = new List<LabelDesc>();
			this.ActiveLabels = new List<LabelDesc>();
			this.CurFunc = null;
		}

		public static LuaProto Parse(ILuaState lua, ILoadInfo loadinfo, string name)
		{
			Parser parser = new Parser();
			parser.Lua = (LuaState)lua;
			parser.Lexer = new LLex(lua, loadinfo, name);
			FuncState funcState = new FuncState();
			parser.MainFunc(funcState);
			return funcState.Proto;
		}

		private LuaProto AddPrototype()
		{
			LuaProto luaProto = new LuaProto();
			this.CurFunc.Proto.P.Add(luaProto);
			return luaProto;
		}

		private void CodeClosure(ExpDesc v)
		{
			FuncState prev = this.CurFunc.Prev;
			this.InitExp(v, ExpKind.VRELOCABLE, Coder.CodeABx(prev, OpCode.OP_CLOSURE, 0, (uint)(prev.Proto.P.Count - 1)));
			Coder.Exp2NextReg(prev, v);
		}

		private void OpenFunc(FuncState fs, BlockCnt block)
		{
			fs.Lexer = this.Lexer;
			fs.Prev = this.CurFunc;
			this.CurFunc = fs;
			fs.Pc = 0;
			fs.LastTarget = 0;
			fs.Jpc = -1;
			fs.FreeReg = 0;
			fs.NumActVar = 0;
			fs.FirstLocal = this.ActVars.Count;
			fs.Proto.MaxStackSize = 2;
			fs.Proto.Source = this.Lexer.Source;
			this.EnterBlock(fs, block, false);
		}

		private void CloseFunc()
		{
			Coder.Ret(this.CurFunc, 0, 0);
			this.LeaveBlock(this.CurFunc);
			this.CurFunc = this.CurFunc.Prev;
		}

		private void MainFunc(FuncState fs)
		{
			ExpDesc e = new ExpDesc();
			BlockCnt block = new BlockCnt();
			this.OpenFunc(fs, block);
			fs.Proto.IsVarArg = true;
			this.InitExp(e, ExpKind.VLOCAL, 0);
			this.NewUpvalue(fs, "_ENV", e);
			this.Lexer.Next();
			this.StatList();
			this.CloseFunc();
		}

		private bool BlockFollow(bool withUntil)
		{
			int tokenType = this.Lexer.Token.TokenType;
			switch (tokenType)
			{
			case 260:
			case 261:
			case 262:
				break;
			default:
				if (tokenType == 277)
				{
					return withUntil;
				}
				if (tokenType != 289)
				{
					return false;
				}
				break;
			}
			return true;
		}

		private bool TestNext(int tokenType)
		{
			if (this.Lexer.Token.TokenType == tokenType)
			{
				this.Lexer.Next();
				return true;
			}
			return false;
		}

		private void Check(int tokenType)
		{
			if (this.Lexer.Token.TokenType != tokenType)
			{
				this.ErrorExpected(tokenType);
			}
		}

		private void CheckNext(int tokenType)
		{
			this.Check(tokenType);
			this.Lexer.Next();
		}

		private void CheckCondition(bool cond, string msg)
		{
			if (!cond)
			{
				this.Lexer.SyntaxError(msg);
			}
		}

		private void EnterLevel()
		{
			this.Lua.NumCSharpCalls++;
			this.CheckLimit(this.CurFunc, this.Lua.NumCSharpCalls, 200, "C# levels");
		}

		private void LeaveLevel()
		{
			this.Lua.NumCSharpCalls--;
		}

		private void SemanticError(string msg)
		{
			this.Lexer.SyntaxError(msg);
		}

		private void ErrorLimit(FuncState fs, int limit, string what)
		{
			int lineDefined = fs.Proto.LineDefined;
			string text = (lineDefined != 0) ? string.Format("function at line {0}", lineDefined) : "main function";
			string msg = string.Format("too many {0} (limit is {1}) in {2}", what, limit, text);
			this.Lexer.SyntaxError(msg);
		}

		private void CheckLimit(FuncState fs, int v, int l, string what)
		{
			if (v > l)
			{
				this.ErrorLimit(fs, l, what);
			}
		}

		private int RegisterLocalVar(string varname)
		{
			LocVar locVar = new LocVar();
			locVar.VarName = varname;
			locVar.StartPc = 0;
			locVar.EndPc = 0;
			this.CurFunc.Proto.LocVars.Add(locVar);
			return this.CurFunc.Proto.LocVars.Count - 1;
		}

		private VarDesc NewLocalVar(string name)
		{
			FuncState curFunc = this.CurFunc;
			int index = this.RegisterLocalVar(name);
			this.CheckLimit(curFunc, this.ActVars.Count + 1 - curFunc.FirstLocal, 200, "local variables");
			return new VarDesc
			{
				Index = index
			};
		}

		private LocVar GetLocalVar(FuncState fs, int i)
		{
			int index = this.ActVars[fs.FirstLocal + i].Index;
			Utl.Assert(index < fs.Proto.LocVars.Count);
			return fs.Proto.LocVars[index];
		}

		private void AdjustLocalVars(int nvars)
		{
			FuncState curFunc = this.CurFunc;
			curFunc.NumActVar += nvars;
			while (nvars > 0)
			{
				LocVar localVar = this.GetLocalVar(curFunc, curFunc.NumActVar - nvars);
				localVar.StartPc = curFunc.Pc;
				nvars--;
			}
		}

		private void RemoveVars(FuncState fs, int toLevel)
		{
			int num = fs.NumActVar - toLevel;
			while (fs.NumActVar > toLevel)
			{
				LocVar localVar = this.GetLocalVar(fs, --fs.NumActVar);
				localVar.EndPc = fs.Pc;
			}
			this.ActVars.RemoveRange(this.ActVars.Count - num, num);
		}

		private void CloseGoto(int g, LabelDesc label)
		{
			LabelDesc labelDesc = this.PendingGotos[g];
			Utl.Assert(labelDesc.Name == label.Name);
			if (labelDesc.NumActVar < label.NumActVar)
			{
				LocVar localVar = this.GetLocalVar(this.CurFunc, labelDesc.NumActVar);
				string msg = string.Format("<goto {0}> at line {1} jumps into the scope of local '{2}'", labelDesc.Name, labelDesc.Line, localVar.VarName);
				this.SemanticError(msg);
			}
			Coder.PatchList(this.CurFunc, labelDesc.Pc, label.Pc);
			this.PendingGotos.RemoveAt(g);
		}

		private bool FindLabel(int g)
		{
			LabelDesc labelDesc = this.PendingGotos[g];
			BlockCnt block = this.CurFunc.Block;
			for (int i = block.FirstLabel; i < this.ActiveLabels.Count; i++)
			{
				LabelDesc labelDesc2 = this.ActiveLabels[i];
				if (labelDesc2.Name == labelDesc.Name)
				{
					if (labelDesc.NumActVar > labelDesc2.NumActVar && (block.HasUpValue || this.ActiveLabels.Count > block.FirstLabel))
					{
						Coder.PatchClose(this.CurFunc, labelDesc.Pc, labelDesc2.NumActVar);
					}
					this.CloseGoto(g, labelDesc2);
					return true;
				}
			}
			return false;
		}

		private LabelDesc NewLebelEntry(string name, int line, int pc)
		{
			return new LabelDesc
			{
				Name = name,
				Pc = pc,
				Line = line,
				NumActVar = this.CurFunc.NumActVar
			};
		}

		private void FindGotos(LabelDesc label)
		{
			int i = this.CurFunc.Block.FirstGoto;
			while (i < this.PendingGotos.Count)
			{
				if (this.PendingGotos[i].Name == label.Name)
				{
					this.CloseGoto(i, label);
				}
				else
				{
					i++;
				}
			}
		}

		private void MoveGotosOut(FuncState fs, BlockCnt block)
		{
			int i = block.FirstGoto;
			while (i < this.PendingGotos.Count)
			{
				LabelDesc labelDesc = this.PendingGotos[i];
				if (labelDesc.NumActVar > block.NumActVar)
				{
					if (block.HasUpValue)
					{
						Coder.PatchClose(fs, labelDesc.Pc, block.NumActVar);
					}
					labelDesc.NumActVar = block.NumActVar;
				}
				if (!this.FindLabel(i))
				{
					i++;
				}
			}
		}

		private void BreakLabel()
		{
			LabelDesc labelDesc = this.NewLebelEntry("break", 0, this.CurFunc.Pc);
			this.ActiveLabels.Add(labelDesc);
			this.FindGotos(this.ActiveLabels[this.ActiveLabels.Count - 1]);
		}

		private void UndefGoto(LabelDesc gt)
		{
			string text = (!this.Lexer.IsReservedWord(gt.Name)) ? "no visible label '{0}' for <goto> at line {1}" : "<{0}> at line {1} not inside a loop";
			string msg = string.Format(text, gt.Name, gt.Line);
			this.SemanticError(msg);
		}

		private void EnterBlock(FuncState fs, BlockCnt block, bool isLoop)
		{
			block.IsLoop = isLoop;
			block.NumActVar = fs.NumActVar;
			block.FirstLabel = this.ActiveLabels.Count;
			block.FirstGoto = this.PendingGotos.Count;
			block.HasUpValue = false;
			block.Previous = fs.Block;
			fs.Block = block;
			Utl.Assert(fs.FreeReg == fs.NumActVar);
		}

		private void LeaveBlock(FuncState fs)
		{
			BlockCnt block = fs.Block;
			if (block.Previous != null && block.HasUpValue)
			{
				int list = Coder.Jump(fs);
				Coder.PatchClose(fs, list, block.NumActVar);
				Coder.PatchToHere(fs, list);
			}
			if (block.IsLoop)
			{
				this.BreakLabel();
			}
			fs.Block = block.Previous;
			this.RemoveVars(fs, block.NumActVar);
			Utl.Assert(block.NumActVar == fs.NumActVar);
			fs.FreeReg = fs.NumActVar;
			this.ActiveLabels.RemoveRange(block.FirstLabel, this.ActiveLabels.Count - block.FirstLabel);
			if (block.Previous != null)
			{
				this.MoveGotosOut(fs, block);
			}
			else if (block.FirstGoto < this.PendingGotos.Count)
			{
				this.UndefGoto(this.PendingGotos[block.FirstGoto]);
			}
		}

		private UnOpr GetUnOpr(int op)
		{
			if (op == 35)
			{
				return UnOpr.LEN;
			}
			if (op == 45)
			{
				return UnOpr.MINUS;
			}
			if (op != 271)
			{
				return UnOpr.NOUNOPR;
			}
			return UnOpr.NOT;
		}

		private BinOpr GetBinOpr(int op)
		{
			switch (op)
			{
			case 42:
				return BinOpr.MUL;
			case 43:
				return BinOpr.ADD;
			default:
				switch (op)
				{
				case 279:
					return BinOpr.CONCAT;
				default:
					switch (op)
					{
					case 60:
						return BinOpr.LT;
					default:
						if (op == 37)
						{
							return BinOpr.MOD;
						}
						if (op == 94)
						{
							return BinOpr.POW;
						}
						if (op == 257)
						{
							return BinOpr.AND;
						}
						if (op != 272)
						{
							return BinOpr.NOBINOPR;
						}
						return BinOpr.OR;
					case 62:
						return BinOpr.GT;
					}
					break;
				case 281:
					return BinOpr.EQ;
				case 282:
					return BinOpr.GE;
				case 283:
					return BinOpr.LE;
				case 284:
					return BinOpr.NE;
				}
				break;
			case 45:
				return BinOpr.SUB;
			case 47:
				return BinOpr.DIV;
			}
		}

		private int GetBinOprLeftPrior(BinOpr opr)
		{
			switch (opr)
			{
			case BinOpr.ADD:
				return 6;
			case BinOpr.SUB:
				return 6;
			case BinOpr.MUL:
				return 7;
			case BinOpr.DIV:
				return 7;
			case BinOpr.MOD:
				return 7;
			case BinOpr.POW:
				return 10;
			case BinOpr.CONCAT:
				return 5;
			case BinOpr.EQ:
				return 3;
			case BinOpr.LT:
				return 3;
			case BinOpr.LE:
				return 3;
			case BinOpr.NE:
				return 3;
			case BinOpr.GT:
				return 3;
			case BinOpr.GE:
				return 3;
			case BinOpr.AND:
				return 2;
			case BinOpr.OR:
				return 1;
			case BinOpr.NOBINOPR:
				throw new Exception("GetBinOprLeftPrior(NOBINOPR)");
			default:
				throw new Exception("Unknown BinOpr");
			}
		}

		private int GetBinOprRightPrior(BinOpr opr)
		{
			switch (opr)
			{
			case BinOpr.ADD:
				return 6;
			case BinOpr.SUB:
				return 6;
			case BinOpr.MUL:
				return 7;
			case BinOpr.DIV:
				return 7;
			case BinOpr.MOD:
				return 7;
			case BinOpr.POW:
				return 9;
			case BinOpr.CONCAT:
				return 4;
			case BinOpr.EQ:
				return 3;
			case BinOpr.LT:
				return 3;
			case BinOpr.LE:
				return 3;
			case BinOpr.NE:
				return 3;
			case BinOpr.GT:
				return 3;
			case BinOpr.GE:
				return 3;
			case BinOpr.AND:
				return 2;
			case BinOpr.OR:
				return 1;
			case BinOpr.NOBINOPR:
				throw new Exception("GetBinOprRightPrior(NOBINOPR)");
			default:
				throw new Exception("Unknown BinOpr");
			}
		}

		private void StatList()
		{
			while (!this.BlockFollow(true))
			{
				if (this.Lexer.Token.TokenType == 274)
				{
					this.Statement();
					return;
				}
				this.Statement();
			}
		}

		private void FieldSel(ExpDesc v)
		{
			FuncState curFunc = this.CurFunc;
			ExpDesc expDesc = new ExpDesc();
			Coder.Exp2AnyRegUp(curFunc, v);
			this.Lexer.Next();
			this.CodeString(expDesc, this.CheckName());
			Coder.Indexed(curFunc, v, expDesc);
		}

		private int Cond()
		{
			ExpDesc expDesc = new ExpDesc();
			this.Expr(expDesc);
			if (expDesc.Kind == ExpKind.VNIL)
			{
				expDesc.Kind = ExpKind.VFALSE;
			}
			Coder.GoIfTrue(this.CurFunc, expDesc);
			return expDesc.ExitFalse;
		}

		private void GotoStat(int pc)
		{
			string name;
			if (this.TestNext(266))
			{
				name = this.CheckName();
			}
			else
			{
				this.Lexer.Next();
				name = "break";
			}
			this.PendingGotos.Add(this.NewLebelEntry(name, this.Lexer.LineNumber, pc));
			this.FindLabel(this.PendingGotos.Count - 1);
		}

		private void CheckRepeated(FuncState fs, List<LabelDesc> list, string label)
		{
			for (int i = fs.Block.FirstLabel; i < list.Count; i++)
			{
				if (label == list[i].Name)
				{
					this.SemanticError(string.Format("label '{0}' already defined on line {1}", label, list[i].Line));
				}
			}
		}

		private void SkipNoOpStat()
		{
			while (this.Lexer.Token.TokenType == 59 || this.Lexer.Token.TokenType == 285)
			{
				this.Statement();
			}
		}

		private void LabelStat(string label, int line)
		{
			FuncState curFunc = this.CurFunc;
			this.CheckRepeated(curFunc, this.ActiveLabels, label);
			this.CheckNext(285);
			LabelDesc labelDesc = this.NewLebelEntry(label, line, curFunc.Pc);
			this.ActiveLabels.Add(labelDesc);
			this.SkipNoOpStat();
			if (this.BlockFollow(false))
			{
				labelDesc.NumActVar = curFunc.Block.NumActVar;
			}
			this.FindGotos(labelDesc);
		}

		private void WhileStat(int line)
		{
			FuncState curFunc = this.CurFunc;
			BlockCnt block = new BlockCnt();
			this.Lexer.Next();
			int label = Coder.GetLabel(curFunc);
			int list = this.Cond();
			this.EnterBlock(curFunc, block, true);
			this.CheckNext(259);
			this.Block();
			Coder.JumpTo(curFunc, label);
			this.CheckMatch(262, 278, line);
			this.LeaveBlock(curFunc);
			Coder.PatchToHere(curFunc, list);
		}

		private void RepeatStat(int line)
		{
			FuncState curFunc = this.CurFunc;
			int label = Coder.GetLabel(curFunc);
			BlockCnt block = new BlockCnt();
			BlockCnt blockCnt = new BlockCnt();
			this.EnterBlock(curFunc, block, true);
			this.EnterBlock(curFunc, blockCnt, false);
			this.Lexer.Next();
			this.StatList();
			this.CheckMatch(277, 273, line);
			int list = this.Cond();
			if (blockCnt.HasUpValue)
			{
				Coder.PatchClose(curFunc, list, blockCnt.NumActVar);
			}
			this.LeaveBlock(curFunc);
			Coder.PatchList(curFunc, list, label);
			this.LeaveBlock(curFunc);
		}

		private int Exp1()
		{
			ExpDesc expDesc = new ExpDesc();
			this.Expr(expDesc);
			Coder.Exp2NextReg(this.CurFunc, expDesc);
			Utl.Assert(expDesc.Kind == ExpKind.VNONRELOC);
			return expDesc.Info;
		}

		private void ForBody(int t, int line, int nvars, bool isnum)
		{
			FuncState curFunc = this.CurFunc;
			BlockCnt block = new BlockCnt();
			this.AdjustLocalVars(3);
			this.CheckNext(259);
			int num = (!isnum) ? Coder.Jump(curFunc) : Coder.CodeAsBx(curFunc, OpCode.OP_FORPREP, t, -1);
			this.EnterBlock(curFunc, block, false);
			this.AdjustLocalVars(nvars);
			Coder.ReserveRegs(curFunc, nvars);
			this.Block();
			this.LeaveBlock(curFunc);
			Coder.PatchToHere(curFunc, num);
			int list;
			if (isnum)
			{
				list = Coder.CodeAsBx(curFunc, OpCode.OP_FORLOOP, t, -1);
			}
			else
			{
				Coder.CodeABC(curFunc, OpCode.OP_TFORCALL, t, 0, nvars);
				Coder.FixLine(curFunc, line);
				list = Coder.CodeAsBx(curFunc, OpCode.OP_TFORLOOP, t + 2, -1);
			}
			Coder.PatchList(curFunc, list, num + 1);
			Coder.FixLine(curFunc, line);
		}

		private void ForNum(string varname, int line)
		{
			FuncState curFunc = this.CurFunc;
			int freeReg = curFunc.FreeReg;
			this.ActVars.Add(this.NewLocalVar("(for index)"));
			this.ActVars.Add(this.NewLocalVar("(for limit)"));
			this.ActVars.Add(this.NewLocalVar("(for step)"));
			this.ActVars.Add(this.NewLocalVar(varname));
			this.CheckNext(61);
			this.Exp1();
			this.CheckNext(44);
			this.Exp1();
			if (this.TestNext(44))
			{
				this.Exp1();
			}
			else
			{
				Coder.CodeK(curFunc, curFunc.FreeReg, Coder.NumberK(curFunc, 1.0));
				Coder.ReserveRegs(curFunc, 1);
			}
			this.ForBody(freeReg, line, 1, true);
		}

		private void ForList(string indexName)
		{
			FuncState curFunc = this.CurFunc;
			ExpDesc e = new ExpDesc();
			int num = 4;
			int freeReg = curFunc.FreeReg;
			this.ActVars.Add(this.NewLocalVar("(for generator)"));
			this.ActVars.Add(this.NewLocalVar("(for state)"));
			this.ActVars.Add(this.NewLocalVar("(for control)"));
			this.ActVars.Add(this.NewLocalVar(indexName));
			while (this.TestNext(44))
			{
				this.ActVars.Add(this.NewLocalVar(this.CheckName()));
				num++;
			}
			this.CheckNext(268);
			int lineNumber = this.Lexer.LineNumber;
			this.AdjustAssign(3, this.ExpList(e), e);
			Coder.CheckStack(curFunc, 3);
			this.ForBody(freeReg, lineNumber, num - 3, false);
		}

		private void ForStat(int line)
		{
			FuncState curFunc = this.CurFunc;
			BlockCnt block = new BlockCnt();
			this.EnterBlock(curFunc, block, true);
			this.Lexer.Next();
			string text = this.CheckName();
			int tokenType = this.Lexer.Token.TokenType;
			if (tokenType != 44)
			{
				if (tokenType == 61)
				{
					this.ForNum(text, line);
					goto IL_87;
				}
				if (tokenType != 268)
				{
					this.Lexer.SyntaxError("'=' or 'in' expected");
					goto IL_87;
				}
			}
			this.ForList(text);
			IL_87:
			this.CheckMatch(262, 264, line);
			this.LeaveBlock(curFunc);
		}

		private int TestThenBlock(int escapeList)
		{
			FuncState curFunc = this.CurFunc;
			BlockCnt block = new BlockCnt();
			this.Lexer.Next();
			ExpDesc expDesc = new ExpDesc();
			this.Expr(expDesc);
			this.CheckNext(275);
			int list;
			if (this.Lexer.Token.TokenType == 266 || this.Lexer.Token.TokenType == 258)
			{
				Coder.GoIfFalse(this.CurFunc, expDesc);
				this.EnterBlock(curFunc, block, false);
				this.GotoStat(expDesc.ExitTrue);
				this.SkipNoOpStat();
				if (this.BlockFollow(false))
				{
					this.LeaveBlock(curFunc);
					return escapeList;
				}
				list = Coder.Jump(curFunc);
			}
			else
			{
				Coder.GoIfTrue(this.CurFunc, expDesc);
				this.EnterBlock(curFunc, block, false);
				list = expDesc.ExitFalse;
			}
			this.StatList();
			this.LeaveBlock(curFunc);
			if (this.Lexer.Token.TokenType == 260 || this.Lexer.Token.TokenType == 261)
			{
				escapeList = Coder.Concat(curFunc, escapeList, Coder.Jump(curFunc));
			}
			Coder.PatchToHere(curFunc, list);
			return escapeList;
		}

		private void IfStat(int line)
		{
			FuncState curFunc = this.CurFunc;
			int num = -1;
			num = this.TestThenBlock(num);
			while (this.Lexer.Token.TokenType == 261)
			{
				num = this.TestThenBlock(num);
			}
			if (this.TestNext(260))
			{
				this.Block();
			}
			this.CheckMatch(262, 267, line);
			Coder.PatchToHere(curFunc, num);
		}

		private void LocalFunc()
		{
			ExpDesc expDesc = new ExpDesc();
			FuncState curFunc = this.CurFunc;
			VarDesc varDesc = this.NewLocalVar(this.CheckName());
			this.ActVars.Add(varDesc);
			this.AdjustLocalVars(1);
			this.Body(expDesc, false, this.Lexer.LineNumber);
			this.GetLocalVar(curFunc, expDesc.Info).StartPc = curFunc.Pc;
		}

		private void LocalStat()
		{
			int num = 0;
			ExpDesc expDesc = new ExpDesc();
			do
			{
				VarDesc varDesc = this.NewLocalVar(this.CheckName());
				this.ActVars.Add(varDesc);
				num++;
			}
			while (this.TestNext(44));
			int nexps;
			if (this.TestNext(61))
			{
				nexps = this.ExpList(expDesc);
			}
			else
			{
				expDesc.Kind = ExpKind.VVOID;
				nexps = 0;
			}
			this.AdjustAssign(num, nexps, expDesc);
			this.AdjustLocalVars(num);
		}

		private bool FuncName(ExpDesc v)
		{
			this.SingleVar(v);
			while (this.Lexer.Token.TokenType == 46)
			{
				this.FieldSel(v);
			}
			if (this.Lexer.Token.TokenType == 58)
			{
				this.FieldSel(v);
				return true;
			}
			return false;
		}

		private void FuncStat(int line)
		{
			ExpDesc v = new ExpDesc();
			ExpDesc e = new ExpDesc();
			this.Lexer.Next();
			bool isMethod = this.FuncName(v);
			this.Body(e, isMethod, line);
			Coder.StoreVar(this.CurFunc, v, e);
			Coder.FixLine(this.CurFunc, line);
		}

		private void ExprStat()
		{
			LHSAssign lhsassign = new LHSAssign();
			this.SuffixedExp(lhsassign.Exp);
			if (this.Lexer.Token.TokenType == 61 || this.Lexer.Token.TokenType == 44)
			{
				lhsassign.Prev = null;
				this.Assignment(lhsassign, 1);
			}
			else
			{
				if (lhsassign.Exp.Kind != ExpKind.VCALL)
				{
					this.Lexer.SyntaxError("syntax error");
				}
				Pointer<Instruction> code = this.CurFunc.GetCode(lhsassign.Exp);
				code.Value = code.Value.SETARG_C(1);
			}
		}

		private void RetStat()
		{
			FuncState curFunc = this.CurFunc;
			int num2;
			int num;
			if (this.BlockFollow(true) || this.Lexer.Token.TokenType == 59)
			{
				num = (num2 = 0);
			}
			else
			{
				ExpDesc expDesc = new ExpDesc();
				num = this.ExpList(expDesc);
				if (this.HasMultiRet(expDesc.Kind))
				{
					Coder.SetMultiRet(curFunc, expDesc);
					if (expDesc.Kind == ExpKind.VCALL && num == 1)
					{
						Pointer<Instruction> code = curFunc.GetCode(expDesc);
						code.Value = code.Value.SET_OPCODE(OpCode.OP_TAILCALL);
						Utl.Assert(code.Value.GETARG_A() == curFunc.NumActVar);
					}
					num2 = curFunc.NumActVar;
					num = -1;
				}
				else if (num == 1)
				{
					num2 = Coder.Exp2AnyReg(curFunc, expDesc);
				}
				else
				{
					Coder.Exp2NextReg(curFunc, expDesc);
					num2 = curFunc.NumActVar;
					Utl.Assert(num == curFunc.FreeReg - num2);
				}
			}
			Coder.Ret(curFunc, num2, num);
			this.TestNext(59);
		}

		private void Statement()
		{
			int lineNumber = this.Lexer.LineNumber;
			this.EnterLevel();
			int tokenType = this.Lexer.Token.TokenType;
			switch (tokenType)
			{
			case 264:
				this.ForStat(lineNumber);
				goto IL_191;
			case 265:
				this.FuncStat(lineNumber);
				goto IL_191;
			case 266:
				break;
			case 267:
				this.IfStat(lineNumber);
				goto IL_191;
			default:
				if (tokenType != 258)
				{
					if (tokenType == 259)
					{
						this.Lexer.Next();
						this.Block();
						this.CheckMatch(262, 259, lineNumber);
						goto IL_191;
					}
					if (tokenType == 59)
					{
						this.Lexer.Next();
						goto IL_191;
					}
					if (tokenType != 285)
					{
						this.ExprStat();
						goto IL_191;
					}
					this.Lexer.Next();
					this.LabelStat(this.CheckName(), lineNumber);
					goto IL_191;
				}
				break;
			case 269:
				this.Lexer.Next();
				if (this.TestNext(265))
				{
					this.LocalFunc();
				}
				else
				{
					this.LocalStat();
				}
				goto IL_191;
			case 273:
				this.RepeatStat(lineNumber);
				goto IL_191;
			case 274:
				this.Lexer.Next();
				this.RetStat();
				goto IL_191;
			case 278:
				this.WhileStat(lineNumber);
				goto IL_191;
			}
			this.GotoStat(Coder.Jump(this.CurFunc));
			IL_191:
			Utl.Assert((int)this.CurFunc.Proto.MaxStackSize >= this.CurFunc.FreeReg && this.CurFunc.FreeReg >= this.CurFunc.NumActVar);
			this.CurFunc.FreeReg = this.CurFunc.NumActVar;
			this.LeaveLevel();
		}

		private string CheckName()
		{
			NameToken nameToken = this.Lexer.Token as NameToken;
			if (nameToken == null)
			{
				ULDebug.LogError.Invoke(this.Lexer.LineNumber + ":" + this.Lexer.Token);
			}
			string semInfo = nameToken.SemInfo;
			this.Lexer.Next();
			return semInfo;
		}

		private int SearchVar(FuncState fs, string name)
		{
			for (int i = fs.NumActVar - 1; i >= 0; i--)
			{
				if (name == this.GetLocalVar(fs, i).VarName)
				{
					return i;
				}
			}
			return -1;
		}

		private void MarkUpvalue(FuncState fs, int level)
		{
			BlockCnt blockCnt = fs.Block;
			while (blockCnt.NumActVar > level)
			{
				blockCnt = blockCnt.Previous;
			}
			blockCnt.HasUpValue = true;
		}

		private ExpKind SingleVarAux(FuncState fs, string name, ExpDesc e, bool flag)
		{
			if (fs == null)
			{
				return ExpKind.VVOID;
			}
			int num = this.SearchVar(fs, name);
			if (num >= 0)
			{
				this.InitExp(e, ExpKind.VLOCAL, num);
				if (!flag)
				{
					this.MarkUpvalue(fs, num);
				}
				return ExpKind.VLOCAL;
			}
			int num2 = this.SearchUpvalues(fs, name);
			if (num2 < 0)
			{
				if (this.SingleVarAux(fs.Prev, name, e, false) == ExpKind.VVOID)
				{
					return ExpKind.VVOID;
				}
				num2 = this.NewUpvalue(fs, name, e);
			}
			this.InitExp(e, ExpKind.VUPVAL, num2);
			return ExpKind.VUPVAL;
		}

		private void SingleVar(ExpDesc e)
		{
			string text = this.CheckName();
			if (this.SingleVarAux(this.CurFunc, text, e, true) == ExpKind.VVOID)
			{
				ExpDesc expDesc = new ExpDesc();
				this.SingleVarAux(this.CurFunc, "_ENV", e, true);
				Utl.Assert(e.Kind == ExpKind.VLOCAL || e.Kind == ExpKind.VUPVAL);
				this.CodeString(expDesc, text);
				Coder.Indexed(this.CurFunc, e, expDesc);
			}
		}

		private void AdjustAssign(int nvars, int nexps, ExpDesc e)
		{
			FuncState curFunc = this.CurFunc;
			int num = nvars - nexps;
			if (this.HasMultiRet(e.Kind))
			{
				num++;
				if (num < 0)
				{
					num = 0;
				}
				Coder.SetReturns(curFunc, e, num);
				if (num > 1)
				{
					Coder.ReserveRegs(curFunc, num - 1);
				}
			}
			else
			{
				if (e.Kind != ExpKind.VVOID)
				{
					Coder.Exp2NextReg(curFunc, e);
				}
				if (num > 0)
				{
					int freeReg = curFunc.FreeReg;
					Coder.ReserveRegs(curFunc, num);
					Coder.CodeNil(curFunc, freeReg, num);
				}
			}
		}

		private void CheckConflict(LHSAssign lh, ExpDesc v)
		{
			FuncState curFunc = this.CurFunc;
			int freeReg = curFunc.FreeReg;
			bool flag = false;
			while (lh != null)
			{
				ExpDesc exp = lh.Exp;
				if (exp.Kind == ExpKind.VINDEXED)
				{
					if (exp.Ind.Vt == v.Kind && exp.Ind.T == v.Info)
					{
						flag = true;
						exp.Ind.Vt = ExpKind.VLOCAL;
						exp.Ind.T = freeReg;
					}
					if (v.Kind == ExpKind.VLOCAL && exp.Ind.Idx == v.Info)
					{
						flag = true;
						exp.Ind.Idx = freeReg;
					}
				}
				lh = lh.Prev;
			}
			if (flag)
			{
				OpCode op = (v.Kind != ExpKind.VLOCAL) ? OpCode.OP_GETUPVAL : OpCode.OP_MOVE;
				Coder.CodeABC(curFunc, op, freeReg, v.Info, 0);
				Coder.ReserveRegs(curFunc, 1);
			}
		}

		private void Assignment(LHSAssign lh, int nvars)
		{
			this.CheckCondition(ExpKindUtl.VKIsVar(lh.Exp.Kind), "syntax error");
			ExpDesc e = new ExpDesc();
			if (this.TestNext(44))
			{
				LHSAssign lhsassign = new LHSAssign();
				lhsassign.Prev = lh;
				this.SuffixedExp(lhsassign.Exp);
				if (lhsassign.Exp.Kind != ExpKind.VINDEXED)
				{
					this.CheckConflict(lh, lhsassign.Exp);
				}
				this.CheckLimit(this.CurFunc, nvars + this.Lua.NumCSharpCalls, 200, "C# levels");
				this.Assignment(lhsassign, nvars + 1);
			}
			else
			{
				this.CheckNext(61);
				int num = this.ExpList(e);
				if (num == nvars)
				{
					Coder.SetOneRet(this.CurFunc, e);
					Coder.StoreVar(this.CurFunc, lh.Exp, e);
					return;
				}
				this.AdjustAssign(nvars, num, e);
				if (num > nvars)
				{
					this.CurFunc.FreeReg -= num - nvars;
				}
			}
			this.InitExp(e, ExpKind.VNONRELOC, this.CurFunc.FreeReg - 1);
			Coder.StoreVar(this.CurFunc, lh.Exp, e);
		}

		private int ExpList(ExpDesc e)
		{
			int num = 1;
			this.Expr(e);
			while (this.TestNext(44))
			{
				Coder.Exp2NextReg(this.CurFunc, e);
				this.Expr(e);
				num++;
			}
			return num;
		}

		private void Expr(ExpDesc e)
		{
			this.SubExpr(e, 0);
		}

		private BinOpr SubExpr(ExpDesc e, int limit)
		{
			this.EnterLevel();
			UnOpr unOpr = this.GetUnOpr(this.Lexer.Token.TokenType);
			if (unOpr != UnOpr.NOUNOPR)
			{
				int lineNumber = this.Lexer.LineNumber;
				this.Lexer.Next();
				this.SubExpr(e, 8);
				Coder.Prefix(this.CurFunc, unOpr, e, lineNumber);
			}
			else
			{
				this.SimpleExp(e);
			}
			BinOpr binOpr = this.GetBinOpr(this.Lexer.Token.TokenType);
			while (binOpr != BinOpr.NOBINOPR && this.GetBinOprLeftPrior(binOpr) > limit)
			{
				int lineNumber2 = this.Lexer.LineNumber;
				this.Lexer.Next();
				Coder.Infix(this.CurFunc, binOpr, e);
				ExpDesc expDesc = new ExpDesc();
				BinOpr binOpr2 = this.SubExpr(expDesc, this.GetBinOprRightPrior(binOpr));
				Coder.Posfix(this.CurFunc, binOpr, e, expDesc, lineNumber2);
				binOpr = binOpr2;
			}
			this.LeaveLevel();
			return binOpr;
		}

		private bool HasMultiRet(ExpKind k)
		{
			return k == ExpKind.VCALL || k == ExpKind.VVARARG;
		}

		private void ErrorExpected(int token)
		{
			this.Lexer.SyntaxError(string.Format("{0} expected", ((char)token).ToString()));
		}

		private void CheckMatch(int what, int who, int where)
		{
			if (!this.TestNext(what))
			{
				if (where == this.Lexer.LineNumber)
				{
					this.ErrorExpected(what);
				}
				else
				{
					this.Lexer.SyntaxError(string.Format("{0} expected (to close {1} at line {2})", ((char)what).ToString(), ((char)who).ToString(), where));
				}
			}
		}

		private void Block()
		{
			FuncState curFunc = this.CurFunc;
			BlockCnt block = new BlockCnt();
			this.EnterBlock(curFunc, block, false);
			this.StatList();
			this.LeaveBlock(curFunc);
		}

		private void YIndex(ExpDesc v)
		{
			this.Lexer.Next();
			this.Expr(v);
			Coder.Exp2Val(this.CurFunc, v);
			this.CheckNext(93);
		}

		private void RecField(ConstructorControl cc)
		{
			FuncState curFunc = this.CurFunc;
			int freeReg = curFunc.FreeReg;
			ExpDesc expDesc = new ExpDesc();
			ExpDesc e = new ExpDesc();
			if (this.Lexer.Token.TokenType == 288)
			{
				this.CheckLimit(curFunc, cc.NumRecord, 2147483645, "items in a constructor");
				this.CodeString(expDesc, this.CheckName());
			}
			else
			{
				this.YIndex(expDesc);
			}
			cc.NumRecord++;
			this.CheckNext(61);
			int b = Coder.Exp2RK(curFunc, expDesc);
			this.Expr(e);
			Coder.CodeABC(curFunc, OpCode.OP_SETTABLE, cc.ExpTable.Info, b, Coder.Exp2RK(curFunc, e));
			curFunc.FreeReg = freeReg;
		}

		private void CloseListField(FuncState fs, ConstructorControl cc)
		{
			if (cc.ExpLastItem.Kind == ExpKind.VVOID)
			{
				return;
			}
			Coder.Exp2NextReg(fs, cc.ExpLastItem);
			cc.ExpLastItem.Kind = ExpKind.VVOID;
			if (cc.NumToStore == 50)
			{
				Coder.SetList(fs, cc.ExpTable.Info, cc.NumArray, cc.NumToStore);
				cc.NumToStore = 0;
			}
		}

		private void LastListField(FuncState fs, ConstructorControl cc)
		{
			if (cc.NumToStore == 0)
			{
				return;
			}
			if (this.HasMultiRet(cc.ExpLastItem.Kind))
			{
				Coder.SetMultiRet(fs, cc.ExpLastItem);
				Coder.SetList(fs, cc.ExpTable.Info, cc.NumArray, -1);
				cc.NumArray--;
			}
			else
			{
				if (cc.ExpLastItem.Kind != ExpKind.VVOID)
				{
					Coder.Exp2NextReg(fs, cc.ExpLastItem);
				}
				Coder.SetList(fs, cc.ExpTable.Info, cc.NumArray, cc.NumToStore);
			}
		}

		private void ListField(ConstructorControl cc)
		{
			this.Expr(cc.ExpLastItem);
			this.CheckLimit(this.CurFunc, cc.NumArray, 2147483645, "items in a constructor");
			cc.NumArray++;
			cc.NumToStore++;
		}

		private void Field(ConstructorControl cc)
		{
			int tokenType = this.Lexer.Token.TokenType;
			if (tokenType != 91)
			{
				if (tokenType != 288)
				{
					this.ListField(cc);
				}
				else if (this.Lexer.GetLookAhead().TokenType != 61)
				{
					this.ListField(cc);
				}
				else
				{
					this.RecField(cc);
				}
			}
			else
			{
				this.RecField(cc);
			}
		}

		private int Integer2FloatingPointByte(uint x)
		{
			int num = 0;
			if (x < 8U)
			{
				return (int)x;
			}
			while (x >= 16U)
			{
				x = x + 1U >> 1;
				num++;
			}
			return num + 1 << 3 | (int)(x - 8U);
		}

		private void Constructor(ExpDesc t)
		{
			FuncState curFunc = this.CurFunc;
			int lineNumber = this.Lexer.LineNumber;
			int num = Coder.CodeABC(curFunc, OpCode.OP_NEWTABLE, 0, 0, 0);
			ConstructorControl constructorControl = new ConstructorControl();
			constructorControl.ExpTable = t;
			this.InitExp(t, ExpKind.VRELOCABLE, num);
			this.InitExp(constructorControl.ExpLastItem, ExpKind.VVOID, 0);
			Coder.Exp2NextReg(curFunc, t);
			this.CheckNext(123);
			do
			{
				Utl.Assert(constructorControl.ExpLastItem.Kind == ExpKind.VVOID || constructorControl.NumToStore > 0);
				if (this.Lexer.Token.TokenType == 125)
				{
					break;
				}
				this.CloseListField(curFunc, constructorControl);
				this.Field(constructorControl);
			}
			while (this.TestNext(44) || this.TestNext(59));
			this.CheckMatch(125, 123, lineNumber);
			this.LastListField(curFunc, constructorControl);
			Instruction instruction = curFunc.Proto.Code[num];
			instruction.SETARG_B(this.Integer2FloatingPointByte((uint)constructorControl.NumArray));
			instruction.SETARG_C(this.Integer2FloatingPointByte((uint)constructorControl.NumRecord));
			curFunc.Proto.Code[num] = instruction;
		}

		private void ParList()
		{
			int num = 0;
			this.CurFunc.Proto.IsVarArg = false;
			if (this.Lexer.Token.TokenType != 41)
			{
				do
				{
					int tokenType = this.Lexer.Token.TokenType;
					if (tokenType != 280)
					{
						if (tokenType != 288)
						{
							this.Lexer.SyntaxError("<name> or '...' expected");
						}
						else
						{
							VarDesc varDesc = this.NewLocalVar(this.CheckName());
							this.ActVars.Add(varDesc);
							num++;
						}
					}
					else
					{
						this.Lexer.Next();
						this.CurFunc.Proto.IsVarArg = true;
					}
				}
				while (!this.CurFunc.Proto.IsVarArg && this.TestNext(44));
			}
			this.AdjustLocalVars(num);
			this.CurFunc.Proto.NumParams = this.CurFunc.NumActVar;
			Coder.ReserveRegs(this.CurFunc, this.CurFunc.NumActVar);
		}

		private void Body(ExpDesc e, bool isMethod, int line)
		{
			FuncState funcState = new FuncState();
			BlockCnt block = new BlockCnt();
			funcState.Proto = this.AddPrototype();
			funcState.Proto.LineDefined = line;
			this.OpenFunc(funcState, block);
			this.CheckNext(40);
			if (isMethod)
			{
				VarDesc varDesc = this.NewLocalVar("self");
				this.ActVars.Add(varDesc);
				this.AdjustLocalVars(1);
			}
			this.ParList();
			this.CheckNext(41);
			this.StatList();
			funcState.Proto.LastLineDefined = this.Lexer.LineNumber;
			this.CheckMatch(262, 265, line);
			this.CodeClosure(e);
			this.CloseFunc();
		}

		private void FuncArgs(ExpDesc e, int line)
		{
			ExpDesc expDesc = new ExpDesc();
			int tokenType = this.Lexer.Token.TokenType;
			if (tokenType != 40)
			{
				if (tokenType != 123)
				{
					if (tokenType != 287)
					{
						this.Lexer.SyntaxError("function arguments expected");
					}
					else
					{
						StringToken stringToken = this.Lexer.Token as StringToken;
						this.CodeString(expDesc, stringToken.SemInfo);
						this.Lexer.Next();
					}
				}
				else
				{
					this.Constructor(expDesc);
				}
			}
			else
			{
				this.Lexer.Next();
				if (this.Lexer.Token.TokenType == 41)
				{
					expDesc.Kind = ExpKind.VVOID;
				}
				else
				{
					this.ExpList(expDesc);
					Coder.SetMultiRet(this.CurFunc, expDesc);
				}
				this.CheckMatch(41, 40, line);
			}
			Utl.Assert(e.Kind == ExpKind.VNONRELOC);
			int info = e.Info;
			int num;
			if (this.HasMultiRet(expDesc.Kind))
			{
				num = -1;
			}
			else
			{
				if (expDesc.Kind != ExpKind.VVOID)
				{
					Coder.Exp2NextReg(this.CurFunc, expDesc);
				}
				num = this.CurFunc.FreeReg - (info + 1);
			}
			this.InitExp(e, ExpKind.VCALL, Coder.CodeABC(this.CurFunc, OpCode.OP_CALL, info, num + 1, 2));
			Coder.FixLine(this.CurFunc, line);
			this.CurFunc.FreeReg = info + 1;
		}

		private void PrimaryExp(ExpDesc e)
		{
			int tokenType = this.Lexer.Token.TokenType;
			if (tokenType == 40)
			{
				int lineNumber = this.Lexer.LineNumber;
				this.Lexer.Next();
				this.Expr(e);
				this.CheckMatch(41, 40, lineNumber);
				Coder.DischargeVars(this.CurFunc, e);
				return;
			}
			if (tokenType != 288)
			{
				this.Lexer.SyntaxError("unexpected symbol");
				return;
			}
			this.SingleVar(e);
		}

		private void SuffixedExp(ExpDesc e)
		{
			FuncState curFunc = this.CurFunc;
			int lineNumber = this.Lexer.LineNumber;
			this.PrimaryExp(e);
			for (;;)
			{
				int tokenType = this.Lexer.Token.TokenType;
				if (tokenType != 40)
				{
					if (tokenType == 46)
					{
						this.FieldSel(e);
						continue;
					}
					if (tokenType == 58)
					{
						ExpDesc expDesc = new ExpDesc();
						this.Lexer.Next();
						this.CodeString(expDesc, this.CheckName());
						Coder.Self(curFunc, e, expDesc);
						this.FuncArgs(e, lineNumber);
						continue;
					}
					if (tokenType == 91)
					{
						ExpDesc expDesc2 = new ExpDesc();
						Coder.Exp2AnyRegUp(curFunc, e);
						this.YIndex(expDesc2);
						Coder.Indexed(curFunc, e, expDesc2);
						continue;
					}
					if (tokenType != 123 && tokenType != 287)
					{
						break;
					}
				}
				Coder.Exp2NextReg(this.CurFunc, e);
				this.FuncArgs(e, lineNumber);
			}
		}

		private void SimpleExp(ExpDesc e)
		{
			Token token = this.Lexer.Token;
			int tokenType = token.TokenType;
			switch (tokenType)
			{
			case 263:
				this.InitExp(e, ExpKind.VFALSE, 0);
				break;
			default:
				if (tokenType != 286)
				{
					if (tokenType != 287)
					{
						if (tokenType == 123)
						{
							this.Constructor(e);
							return;
						}
						if (tokenType != 270)
						{
							if (tokenType != 276)
							{
								if (tokenType != 280)
								{
									this.SuffixedExp(e);
									return;
								}
								this.CheckCondition(this.CurFunc.Proto.IsVarArg, "cannot use '...' outside a vararg function");
								this.InitExp(e, ExpKind.VVARARG, Coder.CodeABC(this.CurFunc, OpCode.OP_VARARG, 0, 1, 0));
							}
							else
							{
								this.InitExp(e, ExpKind.VTRUE, 0);
							}
						}
						else
						{
							this.InitExp(e, ExpKind.VNIL, 0);
						}
					}
					else
					{
						StringToken stringToken = token as StringToken;
						this.CodeString(e, stringToken.SemInfo);
					}
				}
				else
				{
					NumberToken numberToken = token as NumberToken;
					this.InitExp(e, ExpKind.VKNUM, 0);
					e.NumberValue = numberToken.SemInfo;
				}
				break;
			case 265:
				this.Lexer.Next();
				this.Body(e, false, this.Lexer.LineNumber);
				return;
			}
			this.Lexer.Next();
		}

		private int SearchUpvalues(FuncState fs, string name)
		{
			List<UpvalDesc> upvalues = fs.Proto.Upvalues;
			for (int i = 0; i < upvalues.Count; i++)
			{
				if (upvalues[i].Name == name)
				{
					return i;
				}
			}
			return -1;
		}

		private int NewUpvalue(FuncState fs, string name, ExpDesc e)
		{
			LuaProto proto = fs.Proto;
			int count = proto.Upvalues.Count;
			UpvalDesc upvalDesc = new UpvalDesc();
			upvalDesc.InStack = (e.Kind == ExpKind.VLOCAL);
			upvalDesc.Index = e.Info;
			upvalDesc.Name = name;
			proto.Upvalues.Add(upvalDesc);
			return count;
		}

		private void CodeString(ExpDesc e, string s)
		{
			this.InitExp(e, ExpKind.VK, Coder.StringK(this.CurFunc, s));
		}

		private void InitExp(ExpDesc e, ExpKind k, int i)
		{
			e.Kind = k;
			e.Info = i;
			e.ExitTrue = -1;
			e.ExitFalse = -1;
		}

		private const int MAXVARS = 200;

		private const int UnaryPrior = 8;

		private LLex Lexer;

		private FuncState CurFunc;

		private List<VarDesc> ActVars;

		private List<LabelDesc> PendingGotos;

		private List<LabelDesc> ActiveLabels;

		private LuaState Lua;
	}
}
