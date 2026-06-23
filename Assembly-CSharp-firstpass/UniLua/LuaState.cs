using System;
using System.Collections.Generic;
using System.Text;
using UniLua.Tools;

namespace UniLua
{
	public class LuaState : ILuaAPI, ILuaState, ILuaAuxLib
	{
		public LuaState(GlobalState g = null)
		{
			this.API = this;
			this.NumNonYieldable = 1;
			this.NumCSharpCalls = 0;
			this.Hook = null;
			this.HookMask = 0;
			this.BaseHookCount = 0;
			this.AllowHook = true;
			this.ResetHookCount();
			this.Status = ThreadStatus.LUA_OK;
			if (g == null)
			{
				this.G = new GlobalState(this);
				this.InitRegistry();
			}
			else
			{
				this.G = g;
			}
			this.OpenUpval = new LinkedList<LuaUpvalue>();
			this.ErrFunc = 0;
			this.InitStack();
		}

		static LuaState()
		{
			LuaState.TheNilValue.V.SetNilValue();
		}

		LuaState ILuaAPI.NewThread()
		{
			LuaState luaState = new LuaState(this.G);
			this.Top.V.SetThValue(luaState);
			this.ApiIncrTop();
			luaState.HookMask = this.HookMask;
			luaState.BaseHookCount = this.BaseHookCount;
			luaState.Hook = this.Hook;
			luaState.ResetHookCount();
			return luaState;
		}

		ThreadStatus ILuaAPI.Load(ILoadInfo loadinfo, string name, string mode)
		{
			LuaState.LoadParameter loadParameter = new LuaState.LoadParameter(this, loadinfo, name, mode);
			ThreadStatus threadStatus = this.D_PCall<LuaState.LoadParameter>(LuaState.DG_F_Load, ref loadParameter, this.Top.Index, this.ErrFunc);
			if (threadStatus == ThreadStatus.LUA_OK)
			{
				StkId stkId = this.Stack[this.Top.Index - 1];
				Utl.Assert(stkId.V.TtIsFunction() && stkId.V.ClIsLuaClosure());
				LuaLClosureValue luaLClosureValue = stkId.V.ClLValue();
				if (luaLClosureValue.Upvals.Length == 1)
				{
					StkId @int = this.G.Registry.V.HValue().GetInt(2);
					luaLClosureValue.Upvals[0].V.V.SetObj(ref @int.V);
				}
			}
			return threadStatus;
		}

		DumpStatus ILuaAPI.Dump(LuaWriter writeFunc)
		{
			Utl.ApiCheckNumElems(this, 1);
			StkId stkId = this.Stack[this.Top.Index - 1];
			if (!stkId.V.TtIsFunction() || !stkId.V.ClIsLuaClosure())
			{
				return DumpStatus.ERROR;
			}
			LuaLClosureValue luaLClosureValue = stkId.V.ClLValue();
			if (luaLClosureValue == null)
			{
				return DumpStatus.ERROR;
			}
			return DumpState.Dump(luaLClosureValue.Proto, writeFunc, false);
		}

		ThreadStatus ILuaAPI.GetContext(out int context)
		{
			if ((this.CI.CallStatus & CallStatus.CIST_YIELDED) != CallStatus.CIST_NONE)
			{
				context = this.CI.Context;
				return this.CI.Status;
			}
			context = 0;
			return ThreadStatus.LUA_OK;
		}

		void ILuaAPI.Call(int numArgs, int numResults)
		{
			this.API.CallK(numArgs, numResults, 0, null);
		}

		void ILuaAPI.CallK(int numArgs, int numResults, int context, CSharpFunctionDelegate continueFunc)
		{
			Utl.ApiCheck(continueFunc == null || !this.CI.IsLua, "cannot use continuations inside hooks");
			Utl.ApiCheckNumElems(this, numArgs + 1);
			Utl.ApiCheck(this.Status == ThreadStatus.LUA_OK, "cannot do calls on non-normal thread");
			this.CheckResults(numArgs, numResults);
			StkId func = this.Stack[this.Top.Index - (numArgs + 1)];
			if (continueFunc != null && this.NumNonYieldable == 0)
			{
				this.CI.ContinueFunc = continueFunc;
				this.CI.Context = context;
				this.D_Call(func, numResults, true);
			}
			else
			{
				this.D_Call(func, numResults, false);
			}
			this.AdjustResults(numResults);
		}

		ThreadStatus ILuaAPI.PCall(int numArgs, int numResults, int errFunc)
		{
			return this.API.PCallK(numArgs, numResults, errFunc, 0, null);
		}

		ThreadStatus ILuaAPI.PCallK(int numArgs, int numResults, int errFunc, int context, CSharpFunctionDelegate continueFunc)
		{
			Utl.ApiCheck(continueFunc == null || !this.CI.IsLua, "cannot use continuations inside hooks");
			Utl.ApiCheckNumElems(this, numArgs + 1);
			Utl.ApiCheck(this.Status == ThreadStatus.LUA_OK, "cannot do calls on non-normal thread");
			this.CheckResults(numArgs, numResults);
			int errFunc2;
			if (errFunc == 0)
			{
				errFunc2 = 0;
			}
			else
			{
				StkId stkId;
				if (!this.Index2Addr(errFunc, out stkId))
				{
					Utl.InvalidIndex();
				}
				errFunc2 = stkId.Index;
			}
			LuaState.CallS callS = default(LuaState.CallS);
			callS.L = this;
			callS.FuncIndex = this.Top.Index - (numArgs + 1);
			ThreadStatus result;
			if (continueFunc == null || this.NumNonYieldable > 0)
			{
				callS.NumResults = numResults;
				result = this.D_PCall<LuaState.CallS>(LuaState.DG_F_Call, ref callS, callS.FuncIndex, errFunc2);
			}
			else
			{
				int index = this.CI.Index;
				this.CI.ContinueFunc = continueFunc;
				this.CI.Context = context;
				this.CI.ExtraIndex = callS.FuncIndex;
				this.CI.OldAllowHook = this.AllowHook;
				this.CI.OldErrFunc = this.ErrFunc;
				this.ErrFunc = errFunc2;
				this.CI.CallStatus |= CallStatus.CIST_YPCALL;
				this.D_Call(this.Stack[callS.FuncIndex], numResults, true);
				CallInfo callInfo = this.BaseCI[index];
				callInfo.CallStatus &= (CallStatus)(-17);
				this.ErrFunc = callInfo.OldErrFunc;
				result = ThreadStatus.LUA_OK;
			}
			this.AdjustResults(numResults);
			return result;
		}

		ThreadStatus ILuaAPI.Resume(ILuaState from, int numArgs)
		{
			LuaState luaState = from as LuaState;
			this.NumCSharpCalls = ((luaState == null) ? 1 : (luaState.NumCSharpCalls + 1));
			this.NumNonYieldable = 0;
			Utl.ApiCheckNumElems(this, (this.Status != ThreadStatus.LUA_OK) ? numArgs : (numArgs + 1));
			LuaState.ResumeParam resumeParam = default(LuaState.ResumeParam);
			resumeParam.L = this;
			resumeParam.firstArg = this.Top.Index - numArgs;
			ThreadStatus threadStatus = this.D_RawRunProtected<LuaState.ResumeParam>(LuaState.DG_Resume, ref resumeParam);
			if (threadStatus == ThreadStatus.LUA_RESUME_ERROR)
			{
				threadStatus = ThreadStatus.LUA_ERRRUN;
			}
			else
			{
				while (threadStatus != ThreadStatus.LUA_OK && threadStatus != ThreadStatus.LUA_YIELD)
				{
					if (!this.Recover(threadStatus))
					{
						this.Status = threadStatus;
						this.SetErrorObj(threadStatus, this.Top);
						this.CI.TopIndex = this.Top.Index;
						break;
					}
					LuaState.UnrollParam unrollParam = default(LuaState.UnrollParam);
					unrollParam.L = this;
					threadStatus = this.D_RawRunProtected<LuaState.UnrollParam>(LuaState.DG_Unroll, ref unrollParam);
				}
				Utl.Assert(threadStatus == this.Status);
			}
			this.NumNonYieldable = 1;
			this.NumCSharpCalls--;
			Utl.Assert(this.NumCSharpCalls == ((luaState == null) ? 0 : luaState.NumCSharpCalls));
			return threadStatus;
		}

		int ILuaAPI.Yield(int numResults)
		{
			return this.API.YieldK(numResults, 0, null);
		}

		int ILuaAPI.YieldK(int numResults, int context, CSharpFunctionDelegate continueFunc)
		{
			CallInfo ci = this.CI;
			Utl.ApiCheckNumElems(this, numResults);
			if (this.NumNonYieldable > 0)
			{
				if (this != this.G.MainThread)
				{
					this.G_RunError("attempt to yield across metamethod/C-call boundary", new object[0]);
				}
				else
				{
					this.G_RunError("attempt to yield from outside a coroutine", new object[0]);
				}
			}
			this.Status = ThreadStatus.LUA_YIELD;
			ci.ExtraIndex = ci.FuncIndex;
			if (ci.IsLua)
			{
				Utl.ApiCheck(continueFunc == null, "hooks cannot continue after yielding");
			}
			else
			{
				ci.ContinueFunc = continueFunc;
				if (ci.ContinueFunc != null)
				{
					ci.Context = context;
				}
				ci.FuncIndex = this.Top.Index - (numResults + 1);
				this.D_Throw(ThreadStatus.LUA_YIELD);
			}
			Utl.Assert((ci.CallStatus & CallStatus.CIST_HOOKED) != CallStatus.CIST_NONE);
			return 0;
		}

		int ILuaAPI.AbsIndex(int index)
		{
			return (index <= 0 && index > -1001000) ? (this.Top.Index - this.CI.FuncIndex + index) : index;
		}

		int ILuaAPI.GetTop()
		{
			return this.Top.Index - (this.CI.FuncIndex + 1);
		}

		void ILuaAPI.SetTop(int index)
		{
			if (index >= 0)
			{
				Utl.ApiCheck(index <= this.StackLast - (this.CI.FuncIndex + 1), "new top too large");
				int num = this.CI.FuncIndex + 1 + index;
				for (int i = this.Top.Index; i < num; i++)
				{
					this.Stack[i].V.SetNilValue();
				}
				this.Top = this.Stack[num];
			}
			else
			{
				Utl.ApiCheck(-(index + 1) <= this.Top.Index - (this.CI.FuncIndex + 1), "invalid new top");
				this.Top = this.Stack[this.Top.Index + index + 1];
			}
		}

		void ILuaAPI.Remove(int index)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				Utl.InvalidIndex();
			}
			for (int i = stkId.Index + 1; i < this.Top.Index; i++)
			{
				this.Stack[i - 1].V.SetObj(ref this.Stack[i].V);
			}
			this.Top = this.Stack[this.Top.Index - 1];
		}

		void ILuaAPI.Insert(int index)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				Utl.InvalidIndex();
			}
			for (int i = this.Top.Index; i > stkId.Index; i--)
			{
				this.Stack[i].V.SetObj(ref this.Stack[i - 1].V);
			}
			stkId.V.SetObj(ref this.Top.V);
		}

		void ILuaAPI.Replace(int index)
		{
			Utl.ApiCheckNumElems(this, 1);
			this.MoveTo(this.Stack[this.Top.Index - 1], index);
			this.Top = this.Stack[this.Top.Index - 1];
		}

		void ILuaAPI.Copy(int fromIndex, int toIndex)
		{
			StkId fr;
			if (!this.Index2Addr(fromIndex, out fr))
			{
				Utl.InvalidIndex();
			}
			this.MoveTo(fr, toIndex);
		}

		void ILuaAPI.XMove(ILuaState to, int n)
		{
			LuaState luaState = to as LuaState;
			if (this == luaState)
			{
				return;
			}
			Utl.ApiCheckNumElems(this, n);
			Utl.ApiCheck(this.G == luaState.G, "moving among independent states");
			Utl.ApiCheck(luaState.CI.TopIndex - luaState.Top.Index >= n, "not enough elements to move");
			int num = this.Top.Index - n;
			this.Top = this.Stack[num];
			for (int i = 0; i < n; i++)
			{
				StkId.inc(ref luaState.Top).V.SetObj(ref this.Stack[num + i].V);
			}
		}

		bool ILuaAPI.CheckStack(int size)
		{
			bool flag;
			if (this.StackLast - this.Top.Index > size)
			{
				flag = true;
			}
			else
			{
				int num = this.Top.Index + 5;
				if (num > 1000000 - size)
				{
					flag = false;
				}
				else
				{
					LuaState.GrowStackParam growStackParam = default(LuaState.GrowStackParam);
					growStackParam.L = this;
					growStackParam.size = size;
					flag = (this.D_RawRunProtected<LuaState.GrowStackParam>(LuaState.DG_GrowStack, ref growStackParam) == ThreadStatus.LUA_OK);
				}
			}
			if (flag && this.CI.TopIndex < this.Top.Index + size)
			{
				this.CI.TopIndex = this.Top.Index + size;
			}
			return flag;
		}

		int ILuaAPI.Error()
		{
			Utl.ApiCheckNumElems(this, 1);
			this.G_ErrorMsg();
			return 0;
		}

		int ILuaAPI.UpvalueIndex(int i)
		{
			return -1001000 - i;
		}

		string ILuaAPI.GetUpvalue(int funcIndex, int n)
		{
			StkId addr;
			if (!this.Index2Addr(funcIndex, out addr))
			{
				return null;
			}
			StkId stkId;
			string text = this.AuxUpvalue(addr, n, out stkId);
			if (text == null)
			{
				return null;
			}
			this.Top.V.SetObj(ref stkId.V);
			this.ApiIncrTop();
			return text;
		}

		string ILuaAPI.SetUpvalue(int funcIndex, int n)
		{
			StkId addr;
			if (!this.Index2Addr(funcIndex, out addr))
			{
				return null;
			}
			Utl.ApiCheckNumElems(this, 1);
			StkId stkId;
			string text = this.AuxUpvalue(addr, n, out stkId);
			if (text == null)
			{
				return null;
			}
			this.Top = this.Stack[this.Top.Index - 1];
			stkId.V.SetObj(ref this.Top.V);
			return text;
		}

		void ILuaAPI.CreateTable(int narray, int nrec)
		{
			LuaTable luaTable = new LuaTable(this);
			this.Top.V.SetHValue(luaTable);
			this.ApiIncrTop();
			if (narray > 0 || nrec > 0)
			{
				luaTable.Resize(narray, nrec);
			}
		}

		void ILuaAPI.NewTable()
		{
			this.API.CreateTable(0, 0);
		}

		bool ILuaAPI.Next(int index)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				throw new Exception("table expected");
			}
			LuaTable luaTable = stkId.V.HValue();
			if (luaTable == null)
			{
				throw new Exception("table expected");
			}
			StkId key = this.Stack[this.Top.Index - 1];
			if (luaTable.Next(key, this.Top))
			{
				this.ApiIncrTop();
				return true;
			}
			this.Top = this.Stack[this.Top.Index - 1];
			return false;
		}

		void ILuaAPI.RawGetI(int index, int n)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				Utl.ApiCheck(false, "table expected");
			}
			LuaTable luaTable = stkId.V.HValue();
			Utl.ApiCheck(luaTable != null, "table expected");
			this.Top.V.SetObj(ref luaTable.GetInt(n).V);
			this.ApiIncrTop();
		}

		string ILuaAPI.DebugGetInstructionHistory()
		{
			return "DEBUG_RECORD_INS not defined";
		}

		void ILuaAPI.RawGet(int index)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				throw new Exception("table expected");
			}
			if (!stkId.V.TtIsTable())
			{
				throw new Exception("table expected");
			}
			LuaTable luaTable = stkId.V.HValue();
			StkId stkId2 = this.Stack[this.Top.Index - 1];
			stkId2.V.SetObj(ref luaTable.Get(ref stkId2.V).V);
		}

		void ILuaAPI.RawSetI(int index, int n)
		{
			Utl.ApiCheckNumElems(this, 1);
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				Utl.InvalidIndex();
			}
			Utl.ApiCheck(stkId.V.TtIsTable(), "table expected");
			LuaTable luaTable = stkId.V.HValue();
			luaTable.SetInt(n, ref this.Stack[this.Top.Index - 1].V);
			this.Top = this.Stack[this.Top.Index - 1];
		}

		void ILuaAPI.RawSet(int index)
		{
			Utl.ApiCheckNumElems(this, 2);
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				Utl.InvalidIndex();
			}
			Utl.ApiCheck(stkId.V.TtIsTable(), "table expected");
			LuaTable luaTable = stkId.V.HValue();
			luaTable.Set(ref this.Stack[this.Top.Index - 2].V, ref this.Stack[this.Top.Index - 1].V);
			this.Top = this.Stack[this.Top.Index - 2];
		}

		void ILuaAPI.GetField(int index, string key)
		{
			StkId t;
			if (!this.Index2Addr(index, out t))
			{
				Utl.InvalidIndex();
			}
			this.Top.V.SetSValue(key);
			StkId top = this.Top;
			this.ApiIncrTop();
			this.V_GetTable(t, top, top);
		}

		void ILuaAPI.SetField(int index, string key)
		{
			StkId t;
			if (!this.Index2Addr(index, out t))
			{
				Utl.InvalidIndex();
			}
			StkId.inc(ref this.Top).V.SetSValue(key);
			this.V_SetTable(t, this.Stack[this.Top.Index - 1], this.Stack[this.Top.Index - 2]);
			this.Top = this.Stack[this.Top.Index - 2];
		}

		void ILuaAPI.GetTable(int index)
		{
			StkId t;
			if (!this.Index2Addr(index, out t))
			{
				Utl.InvalidIndex();
			}
			StkId stkId = this.Stack[this.Top.Index - 1];
			this.V_GetTable(t, stkId, stkId);
		}

		void ILuaAPI.SetTable(int index)
		{
			Utl.ApiCheckNumElems(this, 2);
			StkId t;
			if (!this.Index2Addr(index, out t))
			{
				Utl.InvalidIndex();
			}
			StkId key = this.Stack[this.Top.Index - 2];
			StkId val = this.Stack[this.Top.Index - 1];
			this.V_SetTable(t, key, val);
			this.Top = this.Stack[this.Top.Index - 2];
		}

		void ILuaAPI.Concat(int n)
		{
			Utl.ApiCheckNumElems(this, n);
			if (n >= 2)
			{
				this.V_Concat(n);
			}
			else if (n == 0)
			{
				this.Top.V.SetSValue(string.Empty);
				this.ApiIncrTop();
			}
		}

		LuaType ILuaAPI.Type(int index)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				return LuaType.LUA_TNONE;
			}
			return (LuaType)stkId.V.Tt;
		}

		string ILuaAPI.TypeName(LuaType t)
		{
			return LuaState.TypeName(t);
		}

		bool ILuaAPI.IsNil(int index)
		{
			return this.API.Type(index) == LuaType.LUA_TNIL;
		}

		bool ILuaAPI.IsNone(int index)
		{
			return this.API.Type(index) == LuaType.LUA_TNONE;
		}

		bool ILuaAPI.IsNoneOrNil(int index)
		{
			LuaType luaType = this.API.Type(index);
			return luaType == LuaType.LUA_TNONE || luaType == LuaType.LUA_TNIL;
		}

		bool ILuaAPI.IsString(int index)
		{
			LuaType luaType = this.API.Type(index);
			return luaType == LuaType.LUA_TSTRING || luaType == LuaType.LUA_TNUMBER;
		}

		bool ILuaAPI.IsTable(int index)
		{
			return this.API.Type(index) == LuaType.LUA_TTABLE;
		}

		bool ILuaAPI.IsFunction(int index)
		{
			return this.API.Type(index) == LuaType.LUA_TFUNCTION;
		}

		bool ILuaAPI.Compare(int index1, int index2, LuaEq op)
		{
			StkId stkId;
			if (!this.Index2Addr(index1, out stkId))
			{
				Utl.InvalidIndex();
			}
			StkId stkId2;
			if (!this.Index2Addr(index2, out stkId2))
			{
				Utl.InvalidIndex();
			}
			switch (op)
			{
			case LuaEq.LUA_OPEQ:
				return this.EqualObj(ref stkId.V, ref stkId2.V, false);
			case LuaEq.LUA_OPLT:
				return this.V_LessThan(stkId, stkId2);
			case LuaEq.LUA_OPLE:
				return this.V_LessEqual(stkId, stkId2);
			default:
				Utl.ApiCheck(false, "invalid option");
				return false;
			}
		}

		bool ILuaAPI.RawEqual(int index1, int index2)
		{
			StkId stkId;
			StkId stkId2;
			return this.Index2Addr(index1, out stkId) && this.Index2Addr(index2, out stkId2) && this.V_RawEqualObj(ref stkId.V, ref stkId2.V);
		}

		int ILuaAPI.RawLen(int index)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				Utl.InvalidIndex();
			}
			switch (stkId.V.Tt)
			{
			case 4:
			{
				string text = stkId.V.SValue();
				return (text != null) ? text.Length : 0;
			}
			case 5:
				return stkId.V.HValue().Length;
			case 7:
				return stkId.V.RawUValue().Length;
			}
			return 0;
		}

		void ILuaAPI.Len(int index)
		{
			StkId rb;
			if (!this.Index2Addr(index, out rb))
			{
				Utl.InvalidIndex();
			}
			this.V_ObjLen(this.Top, rb);
			this.ApiIncrTop();
		}

		void ILuaAPI.PushNil()
		{
			this.Top.V.SetNilValue();
			this.ApiIncrTop();
		}

		void ILuaAPI.PushBoolean(bool b)
		{
			this.Top.V.SetBValue(b);
			this.ApiIncrTop();
		}

		void ILuaAPI.PushNumber(double n)
		{
			this.Top.V.SetNValue(n);
			this.ApiIncrTop();
		}

		void ILuaAPI.PushInteger(int n)
		{
			this.Top.V.SetNValue((double)n);
			this.ApiIncrTop();
		}

		void ILuaAPI.PushUnsigned(uint n)
		{
			this.Top.V.SetNValue(n);
			this.ApiIncrTop();
		}

		string ILuaAPI.PushString(string s)
		{
			if (s == null)
			{
				this.API.PushNil();
				return null;
			}
			this.Top.V.SetSValue(s);
			this.ApiIncrTop();
			return s;
		}

		void ILuaAPI.PushCSharpFunction(CSharpFunctionDelegate f)
		{
			this.API.PushCSharpClosure(f, 0);
		}

		void ILuaAPI.PushCSharpClosure(CSharpFunctionDelegate f, int n)
		{
			if (n == 0)
			{
				this.Top.V.SetClCsValue(new LuaCsClosureValue(f));
			}
			else
			{
				Utl.ApiCheckNumElems(this, n);
				Utl.ApiCheck(n <= 255, "upvalue index too large");
				LuaCsClosureValue luaCsClosureValue = new LuaCsClosureValue(f, n);
				int num = this.Top.Index - n;
				this.Top = this.Stack[num];
				for (int i = 0; i < n; i++)
				{
					luaCsClosureValue.Upvals[i].V.SetObj(ref this.Stack[num + i].V);
				}
				this.Top.V.SetClCsValue(luaCsClosureValue);
			}
			this.ApiIncrTop();
		}

		void ILuaAPI.PushValue(int index)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				Utl.InvalidIndex();
			}
			this.Top.V.SetObj(ref stkId.V);
			this.ApiIncrTop();
		}

		void ILuaAPI.PushGlobalTable()
		{
			this.API.RawGetI(-1001000, 2);
		}

		void ILuaAPI.PushLightUserData(object o)
		{
			this.Top.V.SetPValue(o);
			this.ApiIncrTop();
		}

		void ILuaAPI.PushUInt64(ulong o)
		{
			this.Top.V.SetUInt64Value(o);
			this.ApiIncrTop();
		}

		bool ILuaAPI.PushThread()
		{
			this.Top.V.SetThValue(this);
			this.ApiIncrTop();
			return this.G.MainThread == this;
		}

		void ILuaAPI.Pop(int n)
		{
			this.API.SetTop(-n - 1);
		}

		bool ILuaAPI.GetMetaTable(int index)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				Utl.InvalidIndex();
			}
			LuaTable luaTable2;
			switch (stkId.V.Tt)
			{
			case 5:
			{
				LuaTable luaTable = stkId.V.HValue();
				luaTable2 = luaTable.MetaTable;
				goto IL_87;
			}
			case 7:
			{
				LuaUserDataValue luaUserDataValue = stkId.V.RawUValue();
				luaTable2 = luaUserDataValue.MetaTable;
				goto IL_87;
			}
			}
			luaTable2 = this.G.MetaTables[stkId.V.Tt];
			IL_87:
			if (luaTable2 == null)
			{
				return false;
			}
			this.Top.V.SetHValue(luaTable2);
			this.ApiIncrTop();
			return true;
		}

		bool ILuaAPI.SetMetaTable(int index)
		{
			Utl.ApiCheckNumElems(this, 1);
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				Utl.InvalidIndex();
			}
			StkId stkId2 = this.Stack[this.Top.Index - 1];
			LuaTable luaTable;
			if (stkId2.V.TtIsNil())
			{
				luaTable = null;
			}
			else
			{
				Utl.ApiCheck(stkId2.V.TtIsTable(), "table expected");
				luaTable = stkId2.V.HValue();
			}
			switch (stkId.V.Tt)
			{
			case 5:
			{
				LuaTable luaTable2 = stkId.V.HValue();
				luaTable2.MetaTable = luaTable;
				goto IL_DD;
			}
			case 7:
			{
				LuaUserDataValue luaUserDataValue = stkId.V.RawUValue();
				luaUserDataValue.MetaTable = luaTable;
				goto IL_DD;
			}
			}
			this.G.MetaTables[stkId.V.Tt] = luaTable;
			IL_DD:
			this.Top = this.Stack[this.Top.Index - 1];
			return true;
		}

		void ILuaAPI.GetGlobal(string name)
		{
			StkId @int = this.G.Registry.V.HValue().GetInt(2);
			StkId.inc(ref this.Top).V.SetSValue(name);
			this.V_GetTable(@int, this.Stack[this.Top.Index - 1], this.Stack[this.Top.Index - 1]);
		}

		void ILuaAPI.SetGlobal(string name)
		{
			StkId @int = this.G.Registry.V.HValue().GetInt(2);
			StkId.inc(ref this.Top).V.SetSValue(name);
			this.V_SetTable(@int, this.Stack[this.Top.Index - 1], this.Stack[this.Top.Index - 2]);
			this.Top = this.Stack[this.Top.Index - 2];
		}

		string ILuaAPI.ToString(int index)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				return null;
			}
			if (stkId.V.TtIsString())
			{
				return stkId.V.OValue as string;
			}
			if (!this.V_ToString(ref stkId.V))
			{
				return null;
			}
			if (!this.Index2Addr(index, out stkId))
			{
				return null;
			}
			Utl.Assert(stkId.V.TtIsString());
			return stkId.V.OValue as string;
		}

		double ILuaAPI.ToNumberX(int index, out bool isnum)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				isnum = false;
				return 0.0;
			}
			if (stkId.V.TtIsNumber())
			{
				isnum = true;
				return stkId.V.NValue;
			}
			if (stkId.V.TtIsString())
			{
				TValue tvalue = default(TValue);
				if (this.V_ToNumber(stkId, ref tvalue))
				{
					isnum = true;
					return tvalue.NValue;
				}
			}
			isnum = false;
			return 0.0;
		}

		double ILuaAPI.ToNumber(int index)
		{
			bool flag;
			return this.API.ToNumberX(index, out flag);
		}

		int ILuaAPI.ToIntegerX(int index, out bool isnum)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				isnum = false;
				return 0;
			}
			if (stkId.V.TtIsNumber())
			{
				isnum = true;
				return (int)stkId.V.NValue;
			}
			if (stkId.V.TtIsString())
			{
				TValue tvalue = default(TValue);
				if (this.V_ToNumber(stkId, ref tvalue))
				{
					isnum = true;
					return (int)tvalue.NValue;
				}
			}
			isnum = false;
			return 0;
		}

		int ILuaAPI.ToInteger(int index)
		{
			bool flag;
			return this.API.ToIntegerX(index, out flag);
		}

		uint ILuaAPI.ToUnsignedX(int index, out bool isnum)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				isnum = false;
				return 0U;
			}
			if (stkId.V.TtIsNumber())
			{
				isnum = true;
				return (uint)stkId.V.NValue;
			}
			if (stkId.V.TtIsString())
			{
				TValue tvalue = default(TValue);
				if (this.V_ToNumber(stkId, ref tvalue))
				{
					isnum = true;
					return (uint)tvalue.NValue;
				}
			}
			isnum = false;
			return 0U;
		}

		uint ILuaAPI.ToUnsigned(int index)
		{
			bool flag;
			return this.API.ToUnsignedX(index, out flag);
		}

		bool ILuaAPI.ToBoolean(int index)
		{
			StkId stkId;
			return this.Index2Addr(index, out stkId) && !this.IsFalse(ref stkId.V);
		}

		ulong ILuaAPI.ToUInt64X(int index, out bool isnum)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				isnum = false;
				return 0UL;
			}
			if (!stkId.V.TtIsUInt64())
			{
				isnum = false;
				return 0UL;
			}
			isnum = true;
			return stkId.V.UInt64Value;
		}

		ulong ILuaAPI.ToUInt64(int index)
		{
			bool flag;
			return this.API.ToUInt64X(index, out flag);
		}

		object ILuaAPI.ToObject(int index)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				return null;
			}
			return stkId.V.OValue;
		}

		object ILuaAPI.ToUserData(int index)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				return null;
			}
			int tt = stkId.V.Tt;
			switch (tt)
			{
			case 7:
				throw new NotImplementedException();
			default:
				if (tt != 2)
				{
					return null;
				}
				return stkId.V.OValue;
			case 9:
				return stkId.V.UInt64Value;
			}
		}

		ILuaState ILuaAPI.ToThread(int index)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				return null;
			}
			ILuaState result;
			if (stkId.V.TtIsThread())
			{
				ILuaState luaState = stkId.V.OValue as ILuaState;
				result = luaState;
			}
			else
			{
				result = null;
			}
			return result;
		}

		bool ILuaAPI.GetStack(int level, LuaDebug ar)
		{
			if (level < 0)
			{
				return false;
			}
			int num = this.CI.Index;
			while (level > 0 && num > 0)
			{
				level--;
				num--;
			}
			bool result = false;
			if (level == 0 && num > 0)
			{
				result = true;
				ar.ActiveCIIndex = num;
			}
			return result;
		}

		internal void D_Throw(ThreadStatus errCode)
		{
			throw new LuaRuntimeException(errCode);
		}

		private ThreadStatus D_RawRunProtected<T>(PFuncDelegate<T> func, ref T ud)
		{
			int numCSharpCalls = this.NumCSharpCalls;
			ThreadStatus result = ThreadStatus.LUA_OK;
			try
			{
				func(ref ud);
			}
			catch (LuaRuntimeException ex)
			{
				this.NumCSharpCalls = numCSharpCalls;
				result = ex.ErrCode;
			}
			this.NumCSharpCalls = numCSharpCalls;
			return result;
		}

		private void SetErrorObj(ThreadStatus errCode, StkId oldTop)
		{
			switch (errCode)
			{
			case ThreadStatus.LUA_ERRMEM:
				oldTop.V.SetSValue("not enough memory");
				goto IL_6E;
			case ThreadStatus.LUA_ERRERR:
				oldTop.V.SetSValue("error in error handling");
				goto IL_6E;
			}
			oldTop.V.SetObj(ref this.Stack[this.Top.Index - 1].V);
			IL_6E:
			this.Top = this.Stack[oldTop.Index + 1];
		}

		private ThreadStatus D_PCall<T>(PFuncDelegate<T> func, ref T ud, int oldTopIndex, int errFunc)
		{
			int index = this.CI.Index;
			bool allowHook = this.AllowHook;
			int numNonYieldable = this.NumNonYieldable;
			int errFunc2 = this.ErrFunc;
			this.ErrFunc = errFunc;
			ThreadStatus threadStatus = this.D_RawRunProtected<T>(func, ref ud);
			if (threadStatus != ThreadStatus.LUA_OK)
			{
				this.F_Close(this.Stack[oldTopIndex]);
				this.SetErrorObj(threadStatus, this.Stack[oldTopIndex]);
				this.CI = this.BaseCI[index];
				this.AllowHook = allowHook;
				this.NumNonYieldable = numNonYieldable;
			}
			this.ErrFunc = errFunc2;
			return threadStatus;
		}

		private void D_Call(StkId func, int nResults, bool allowYield)
		{
			if (++this.NumCSharpCalls >= 200)
			{
				if (this.NumCSharpCalls == 200)
				{
					this.G_RunError("CSharp Stack Overflow", new object[0]);
				}
				else if (this.NumCSharpCalls >= 225)
				{
					this.D_Throw(ThreadStatus.LUA_ERRERR);
				}
			}
			if (!allowYield)
			{
				this.NumNonYieldable++;
			}
			if (!this.D_PreCall(func, nResults))
			{
				this.V_Execute();
			}
			if (!allowYield)
			{
				this.NumNonYieldable--;
			}
			this.NumCSharpCalls--;
		}

		private bool D_PreCall(StkId func, int nResults)
		{
			int index = func.Index;
			if (!func.V.TtIsFunction())
			{
				func = this.tryFuncTM(func);
				return this.D_PreCall(func, nResults);
			}
			if (func.V.ClIsLuaClosure())
			{
				LuaLClosureValue luaLClosureValue = func.V.ClLValue();
				Utl.Assert(luaLClosureValue != null);
				LuaProto proto = luaLClosureValue.Proto;
				this.D_CheckStack((int)proto.MaxStackSize + proto.NumParams);
				func = this.Stack[index];
				int i;
				for (i = this.Top.Index - func.Index - 1; i < proto.NumParams; i++)
				{
					StkId.inc(ref this.Top).V.SetNilValue();
				}
				int num = proto.IsVarArg ? this.AdjustVarargs(proto, i) : (func.Index + 1);
				this.CI = this.ExtendCI();
				this.CI.NumResults = nResults;
				this.CI.FuncIndex = func.Index;
				this.CI.BaseIndex = num;
				this.CI.TopIndex = num + (int)proto.MaxStackSize;
				Utl.Assert(this.CI.TopIndex <= this.StackLast);
				this.CI.SavedPc = new Pointer<Instruction>(proto.Code, 0);
				this.CI.CallStatus = CallStatus.CIST_LUA;
				this.Top = this.Stack[this.CI.TopIndex];
				return false;
			}
			if (func.V.ClIsCsClosure())
			{
				LuaCsClosureValue luaCsClosureValue = func.V.ClCsValue();
				Utl.Assert(luaCsClosureValue != null);
				this.D_CheckStack(20);
				func = this.Stack[index];
				this.CI = this.ExtendCI();
				this.CI.NumResults = nResults;
				this.CI.FuncIndex = func.Index;
				this.CI.TopIndex = this.Top.Index + 20;
				this.CI.CallStatus = CallStatus.CIST_NONE;
				int num2 = luaCsClosureValue.F(this);
				this.D_PosCall(this.Top.Index - num2);
				return true;
			}
			throw new NotImplementedException();
		}

		private int D_PosCall(int firstResultIndex)
		{
			int funcIndex = this.CI.FuncIndex;
			int numResults = this.CI.NumResults;
			this.CI = this.BaseCI[this.CI.Index - 1];
			int num = numResults;
			while (num != 0 && firstResultIndex < this.Top.Index)
			{
				this.Stack[funcIndex++].V.SetObj(ref this.Stack[firstResultIndex++].V);
				num--;
			}
			while (num-- > 0)
			{
				this.Stack[funcIndex++].V.SetNilValue();
			}
			this.Top = this.Stack[funcIndex];
			return numResults - -1;
		}

		private CallInfo ExtendCI()
		{
			int num = this.CI.Index + 1;
			if (num >= this.BaseCI.Length)
			{
				int num2 = this.BaseCI.Length * 2;
				CallInfo[] array = new CallInfo[num2];
				int i;
				for (i = 0; i < this.BaseCI.Length; i++)
				{
					array[i] = this.BaseCI[i];
					array[i].List = array;
				}
				while (i < num2)
				{
					CallInfo callInfo = new CallInfo();
					array[i] = callInfo;
					callInfo.List = array;
					callInfo.Index = i;
					i++;
				}
				this.BaseCI = array;
				this.CI = array[this.CI.Index];
				return array[num];
			}
			return this.BaseCI[num];
		}

		private int AdjustVarargs(LuaProto p, int actual)
		{
			int numParams = p.NumParams;
			Utl.Assert(actual >= numParams, "AdjustVarargs (actual >= NumFixArgs) is false");
			int num = this.Top.Index - actual;
			int index = this.Top.Index;
			for (int i = index; i < index + numParams; i++)
			{
				this.Stack[i].V.SetObj(ref this.Stack[num].V);
				this.Stack[num++].V.SetNilValue();
			}
			this.Top = this.Stack[index + numParams];
			return index;
		}

		private StkId tryFuncTM(StkId func)
		{
			StkId stkId = this.T_GetTMByObj(ref func.V, TMS.TM_CALL);
			if (!stkId.V.TtIsFunction())
			{
				this.G_TypeError(func, "call");
			}
			for (int i = this.Top.Index; i > func.Index; i--)
			{
				this.Stack[i].V.SetObj(ref this.Stack[i - 1].V);
			}
			this.IncrTop();
			func.V.SetObj(ref stkId.V);
			return func;
		}

		private void D_CheckStack(int n)
		{
			if (this.StackLast - this.Top.Index <= n)
			{
				this.D_GrowStack(n);
			}
		}

		private void D_GrowStack(int n)
		{
			int num = this.Stack.Length;
			if (num > 1000000)
			{
				this.D_Throw(ThreadStatus.LUA_ERRERR);
			}
			int num2 = this.Top.Index + n + 5;
			int num3 = 2 * num;
			if (num3 > 1000000)
			{
				num3 = 1000000;
			}
			if (num3 < num2)
			{
				num3 = num2;
			}
			if (num3 > 1000000)
			{
				this.D_ReallocStack(1000200);
				this.G_RunError("stack overflow", new object[0]);
			}
			else
			{
				this.D_ReallocStack(num3);
			}
		}

		private void D_ReallocStack(int size)
		{
			Utl.Assert(size <= 1000000 || size == 1000200);
			StkId[] array = new StkId[size];
			int i;
			for (i = 0; i < this.Stack.Length; i++)
			{
				array[i] = this.Stack[i];
				array[i].SetList(array);
			}
			while (i < size)
			{
				array[i] = new StkId();
				array[i].SetList(array);
				array[i].SetIndex(i);
				array[i].V.SetNilValue();
				i++;
			}
			this.Top = array[this.Top.Index];
			this.Stack = array;
			this.StackLast = size - 5;
		}

		private void CheckMode(string given, string expected)
		{
			if (given != null && given.IndexOf(expected.get_Chars(0)) == -1)
			{
				this.O_PushString(string.Format("attempt to load a {0} chunk (mode is '{1}')", expected, given));
				this.D_Throw(ThreadStatus.LUA_ERRSYNTAX);
			}
		}

		private static void F_Load(ref LuaState.LoadParameter param)
		{
			LuaState l = param.L;
			int num = param.LoadInfo.PeekByte();
			LuaProto p;
			if (num == (int)"\u001bLua".get_Chars(0))
			{
				l.CheckMode(param.Mode, "binary");
				p = Undump.LoadBinary(l, param.LoadInfo, param.Name);
			}
			else
			{
				l.CheckMode(param.Mode, "text");
				p = Parser.Parse(l, param.LoadInfo, param.Name);
			}
			LuaLClosureValue luaLClosureValue = new LuaLClosureValue(p);
			Utl.Assert(luaLClosureValue.Upvals.Length == luaLClosureValue.Proto.Upvalues.Count);
			l.Top.V.SetClLValue(luaLClosureValue);
			l.IncrTop();
		}

		private static void F_Call(ref LuaState.CallS ud)
		{
			LuaState.CallS callS = ud;
			callS.L.D_Call(callS.L.Stack[callS.FuncIndex], callS.NumResults, false);
		}

		private void CheckResults(int numArgs, int numResults)
		{
			Utl.ApiCheck(numResults == -1 || this.CI.TopIndex - this.Top.Index >= numResults - numArgs, "results from function overflow current stack size");
		}

		private void AdjustResults(int numResults)
		{
			if (numResults == -1 && this.CI.TopIndex < this.Top.Index)
			{
				this.CI.TopIndex = this.Top.Index;
			}
		}

		private void FinishCSharpCall()
		{
			CallInfo ci = this.CI;
			Utl.Assert(ci.ContinueFunc != null);
			Utl.Assert(this.NumNonYieldable == 0);
			this.AdjustResults(ci.NumResults);
			if ((ci.CallStatus & CallStatus.CIST_STAT) == CallStatus.CIST_NONE)
			{
				ci.Status = ThreadStatus.LUA_YIELD;
			}
			Utl.Assert(ci.Status != ThreadStatus.LUA_OK);
			ci.CallStatus = ((ci.CallStatus & (CallStatus)(-49)) | CallStatus.CIST_YIELDED);
			int num = ci.ContinueFunc(this);
			Utl.ApiCheckNumElems(this, num);
			this.D_PosCall(this.Top.Index - num);
		}

		private void Unroll()
		{
			while (this.CI.Index != 0)
			{
				if (!this.CI.IsLua)
				{
					this.FinishCSharpCall();
				}
				else
				{
					this.V_FinishOp();
					this.V_Execute();
				}
			}
		}

		private static void UnrollWrap(ref LuaState.UnrollParam param)
		{
			param.L.Unroll();
		}

		private void ResumeError(string msg, int firstArg)
		{
			this.Top = this.Stack[firstArg];
			this.Top.V.SetSValue(msg);
			this.IncrTop();
			this.D_Throw(ThreadStatus.LUA_RESUME_ERROR);
		}

		private CallInfo FindPCall()
		{
			for (int i = this.CI.Index; i >= 0; i--)
			{
				CallInfo callInfo = this.BaseCI[i];
				if ((callInfo.CallStatus & CallStatus.CIST_YPCALL) != CallStatus.CIST_NONE)
				{
					return callInfo;
				}
			}
			return null;
		}

		private bool Recover(ThreadStatus status)
		{
			CallInfo callInfo = this.FindPCall();
			if (callInfo == null)
			{
				return false;
			}
			StkId stkId = this.Stack[callInfo.ExtraIndex];
			this.F_Close(stkId);
			this.SetErrorObj(status, stkId);
			this.CI = callInfo;
			this.AllowHook = callInfo.OldAllowHook;
			this.NumNonYieldable = 0;
			this.ErrFunc = callInfo.OldErrFunc;
			callInfo.CallStatus |= CallStatus.CIST_STAT;
			callInfo.Status = status;
			return true;
		}

		private void Resume(int firstArg)
		{
			int numCSharpCalls = this.NumCSharpCalls;
			CallInfo ci = this.CI;
			if (numCSharpCalls >= 200)
			{
				this.ResumeError("C stack overflow", firstArg);
			}
			if (this.Status == ThreadStatus.LUA_OK)
			{
				if (ci.Index > 0)
				{
					this.ResumeError("cannot resume non-suspended coroutine", firstArg);
				}
				if (!this.D_PreCall(this.Stack[firstArg - 1], -1))
				{
					this.V_Execute();
				}
			}
			else if (this.Status != ThreadStatus.LUA_YIELD)
			{
				this.ResumeError("cannot resume dead coroutine", firstArg);
			}
			else
			{
				this.Status = ThreadStatus.LUA_OK;
				ci.FuncIndex = ci.ExtraIndex;
				if (ci.IsLua)
				{
					this.V_Execute();
				}
				else
				{
					if (ci.ContinueFunc != null)
					{
						ci.Status = ThreadStatus.LUA_YIELD;
						ci.CallStatus |= CallStatus.CIST_YIELDED;
						int num = ci.ContinueFunc(this);
						Utl.ApiCheckNumElems(this, num);
						firstArg = this.Top.Index - num;
					}
					this.D_PosCall(firstArg);
				}
				this.Unroll();
			}
			Utl.Assert(numCSharpCalls == this.NumCSharpCalls);
		}

		private static void ResumeWrap(ref LuaState.ResumeParam param)
		{
			param.L.Resume(param.firstArg);
		}

		private void MoveTo(StkId fr, int index)
		{
			StkId stkId;
			if (!this.Index2Addr(index, out stkId))
			{
				Utl.InvalidIndex();
			}
			stkId.V.SetObj(ref fr.V);
		}

		private void GrowStack(int size)
		{
			this.D_GrowStack(size);
		}

		private static void GrowStackWrap(ref LuaState.GrowStackParam param)
		{
			param.L.GrowStack(param.size);
		}

		private string AuxUpvalue(StkId addr, int n, out StkId val)
		{
			val = null;
			if (!addr.V.TtIsFunction())
			{
				return null;
			}
			if (addr.V.ClIsLuaClosure())
			{
				LuaLClosureValue luaLClosureValue = addr.V.ClLValue();
				LuaProto proto = luaLClosureValue.Proto;
				if (1 > n || n > proto.Upvalues.Count)
				{
					return null;
				}
				val = luaLClosureValue.Upvals[n - 1].V;
				string name = proto.Upvalues[n - 1].Name;
				return (name != null) ? name : string.Empty;
			}
			else
			{
				if (!addr.V.ClIsCsClosure())
				{
					return null;
				}
				LuaCsClosureValue luaCsClosureValue = addr.V.ClCsValue();
				if (1 > n || n > luaCsClosureValue.Upvals.Length)
				{
					return null;
				}
				val = luaCsClosureValue.Upvals[n - 1];
				return string.Empty;
			}
		}

		internal static string TypeName(LuaType t)
		{
			switch (t)
			{
			case LuaType.LUA_TNIL:
				return "nil";
			case LuaType.LUA_TBOOLEAN:
				return "boolean";
			case LuaType.LUA_TLIGHTUSERDATA:
				return "userdata";
			case LuaType.LUA_TNUMBER:
				return "number";
			case LuaType.LUA_TSTRING:
				return "string";
			case LuaType.LUA_TTABLE:
				return "table";
			case LuaType.LUA_TFUNCTION:
				return "function";
			case LuaType.LUA_TUSERDATA:
				return "userdata";
			case LuaType.LUA_TTHREAD:
				return "thread";
			case LuaType.LUA_TUINT64:
				return "UInt64";
			case LuaType.LUA_TPROTO:
				return "proto";
			case LuaType.LUA_TUPVAL:
				return "upval";
			}
			return "no value";
		}

		internal string ObjTypeName(ref TValue v)
		{
			return LuaState.TypeName((LuaType)v.Tt);
		}

		internal void O_PushString(string s)
		{
			this.Top.V.SetSValue(s);
			this.IncrTop();
		}

		private bool Index2Addr(int index, out StkId addr)
		{
			CallInfo ci = this.CI;
			if (index > 0)
			{
				int num = ci.FuncIndex + index;
				Utl.ApiCheck(index <= ci.TopIndex - (ci.FuncIndex + 1), "unacceptable index");
				if (num >= this.Top.Index)
				{
					addr = null;
					return false;
				}
				addr = this.Stack[num];
				return true;
			}
			else
			{
				if (index > -1001000)
				{
					Utl.ApiCheck(index != 0 && -index <= this.Top.Index - (ci.FuncIndex + 1), "invalid index");
					addr = this.Stack[this.Top.Index + index];
					return true;
				}
				if (index == -1001000)
				{
					addr = this.G.Registry;
					return true;
				}
				index = -1001000 - index;
				Utl.ApiCheck(index <= 256, "upvalue index too large");
				StkId stkId = this.Stack[ci.FuncIndex];
				Utl.Assert(stkId.V.TtIsFunction());
				if (stkId.V.ClIsLcsClosure())
				{
					addr = null;
					return false;
				}
				Utl.Assert(stkId.V.ClIsCsClosure());
				LuaCsClosureValue luaCsClosureValue = stkId.V.ClCsValue();
				if (index > luaCsClosureValue.Upvals.Length)
				{
					addr = null;
					return false;
				}
				addr = luaCsClosureValue.Upvals[index - 1];
				return true;
			}
		}

		public void L_Where(int level)
		{
			LuaDebug luaDebug = new LuaDebug();
			if (this.API.GetStack(level, luaDebug))
			{
				this.GetInfo("Sl", luaDebug);
				if (luaDebug.CurrentLine > 0)
				{
					this.API.PushString(string.Format("{0}:{1}: ", luaDebug.ShortSrc, luaDebug.CurrentLine));
					return;
				}
			}
			this.API.PushString(string.Empty);
		}

		public int L_Error(string fmt, params object[] args)
		{
			this.L_Where(1);
			this.API.PushString(string.Format(fmt, args));
			this.API.Concat(2);
			return this.API.Error();
		}

		public void L_CheckStack(int size, string msg)
		{
			if (!this.API.CheckStack(size + 20))
			{
				if (msg != null)
				{
					this.L_Error(string.Format("stack overflow ({0})", msg), new object[0]);
				}
				else
				{
					this.L_Error("stack overflow", new object[0]);
				}
			}
		}

		public void L_CheckAny(int narg)
		{
			if (this.API.Type(narg) == LuaType.LUA_TNONE)
			{
				this.L_ArgError(narg, "value expected");
			}
		}

		public double L_CheckNumber(int narg)
		{
			bool flag;
			double result = this.API.ToNumberX(narg, out flag);
			if (!flag)
			{
				this.TagError(narg, LuaType.LUA_TNUMBER);
			}
			return result;
		}

		public ulong L_CheckUInt64(int narg)
		{
			bool flag;
			ulong result = this.API.ToUInt64X(narg, out flag);
			if (!flag)
			{
				this.TagError(narg, LuaType.LUA_TUINT64);
			}
			return result;
		}

		public int L_CheckInteger(int narg)
		{
			bool flag;
			int result = this.API.ToIntegerX(narg, out flag);
			if (!flag)
			{
				this.TagError(narg, LuaType.LUA_TNUMBER);
			}
			return result;
		}

		public string L_CheckString(int narg)
		{
			string text = this.API.ToString(narg);
			if (text == null)
			{
				this.TagError(narg, LuaType.LUA_TSTRING);
			}
			return text;
		}

		public uint L_CheckUnsigned(int narg)
		{
			bool flag;
			uint result = this.API.ToUnsignedX(narg, out flag);
			if (!flag)
			{
				this.TagError(narg, LuaType.LUA_TNUMBER);
			}
			return result;
		}

		public T L_Opt<T>(Func<int, T> f, int n, T def)
		{
			LuaType luaType = this.API.Type(n);
			if (luaType == LuaType.LUA_TNONE || luaType == LuaType.LUA_TNIL)
			{
				return def;
			}
			return f.Invoke(n);
		}

		public int L_OptInt(int narg, int def)
		{
			LuaType luaType = this.API.Type(narg);
			if (luaType == LuaType.LUA_TNONE || luaType == LuaType.LUA_TNIL)
			{
				return def;
			}
			return this.L_CheckInteger(narg);
		}

		public string L_OptString(int narg, string def)
		{
			LuaType luaType = this.API.Type(narg);
			if (luaType == LuaType.LUA_TNONE || luaType == LuaType.LUA_TNIL)
			{
				return def;
			}
			return this.L_CheckString(narg);
		}

		private int TypeError(int index, string typeName)
		{
			string text = string.Format("{0} expected, got {1}", typeName, this.L_TypeName(index));
			this.API.PushString(text);
			return this.L_ArgError(index, text);
		}

		private void TagError(int index, LuaType t)
		{
			this.TypeError(index, this.API.TypeName(t));
		}

		public void L_CheckType(int index, LuaType t)
		{
			if (this.API.Type(index) != t)
			{
				this.TagError(index, t);
			}
		}

		public void L_ArgCheck(bool cond, int narg, string extraMsg)
		{
			if (!cond)
			{
				this.L_ArgError(narg, extraMsg);
			}
		}

		public int L_ArgError(int narg, string extraMsg)
		{
			LuaDebug luaDebug = new LuaDebug();
			if (!this.API.GetStack(0, luaDebug))
			{
				return this.L_Error("bad argument {0} ({1})", new object[]
				{
					narg,
					extraMsg
				});
			}
			this.GetInfo("n", luaDebug);
			if (luaDebug.NameWhat == "method")
			{
				narg--;
				if (narg == 0)
				{
					return this.L_Error("calling '{0}' on bad self", new object[]
					{
						luaDebug.Name
					});
				}
			}
			if (luaDebug.Name == null)
			{
				luaDebug.Name = ((!this.PushGlobalFuncName(luaDebug)) ? "?" : this.API.ToString(-1));
			}
			return this.L_Error("bad argument {0} to '{1}' ({2})", new object[]
			{
				narg,
				luaDebug.Name,
				extraMsg
			});
		}

		public string L_TypeName(int index)
		{
			return this.API.TypeName(this.API.Type(index));
		}

		public bool L_GetMetaField(int obj, string name)
		{
			if (!this.API.GetMetaTable(obj))
			{
				return false;
			}
			this.API.PushString(name);
			this.API.RawGet(-2);
			if (this.API.IsNil(-1))
			{
				this.API.Pop(2);
				return false;
			}
			this.API.Remove(-2);
			return true;
		}

		public bool L_CallMeta(int obj, string name)
		{
			obj = this.API.AbsIndex(obj);
			if (!this.L_GetMetaField(obj, name))
			{
				return false;
			}
			this.API.PushValue(obj);
			this.API.Call(1, 1);
			return true;
		}

		private void PushFuncName(LuaDebug ar)
		{
			if (ar.NameWhat.Length > 0 && ar.NameWhat.get_Chars(0) != '\0')
			{
				this.API.PushString(string.Format("function '{0}'", ar.Name));
			}
			else if (ar.What.Length > 0 && ar.What.get_Chars(0) == 'm')
			{
				this.API.PushString("main chunk");
			}
			else if (ar.What.Length > 0 && ar.What.get_Chars(0) == 'C')
			{
				if (this.PushGlobalFuncName(ar))
				{
					this.API.PushString(string.Format("function '{0}'", this.API.ToString(-1)));
					this.API.Remove(-2);
				}
				else
				{
					this.API.PushString("?");
				}
			}
			else
			{
				this.API.PushString(string.Format("function <{0}:{1}>", ar.ShortSrc, ar.LineDefined));
			}
		}

		private int CountLevels()
		{
			LuaDebug ar = new LuaDebug();
			int i = 1;
			int num = 1;
			while (this.API.GetStack(num, ar))
			{
				i = num;
				num *= 2;
			}
			while (i < num)
			{
				int num2 = (i + num) / 2;
				if (this.API.GetStack(num2, ar))
				{
					i = num2 + 1;
				}
				else
				{
					num = num2;
				}
			}
			return num - 1;
		}

		public void L_Traceback(ILuaState otherLua, string msg, int level)
		{
			LuaState luaState = otherLua as LuaState;
			LuaDebug luaDebug = new LuaDebug();
			int top = this.API.GetTop();
			int num = luaState.CountLevels();
			int num2 = (num <= 22) ? 0 : 12;
			if (msg != null)
			{
				this.API.PushString(string.Format("{0}\n", msg));
			}
			this.API.PushString("stack traceback:");
			while (otherLua.GetStack(level++, luaDebug))
			{
				if (level == num2)
				{
					this.API.PushString("\n\t...");
					level = num - 10;
				}
				else
				{
					luaState.GetInfo("Slnt", luaDebug);
					this.API.PushString(string.Format("\n\t{0}:", luaDebug.ShortSrc));
					if (luaDebug.CurrentLine > 0)
					{
						this.API.PushString(string.Format("{0}:", luaDebug.CurrentLine));
					}
					this.API.PushString(" in ");
					this.PushFuncName(luaDebug);
					if (luaDebug.IsTailCall)
					{
						this.API.PushString("\n\t(...tail calls...)");
					}
					this.API.Concat(this.API.GetTop() - top);
				}
			}
			this.API.Concat(this.API.GetTop() - top);
		}

		public int L_Len(int index)
		{
			this.API.Len(index);
			bool flag;
			int result = this.API.ToIntegerX(-1, out flag);
			if (!flag)
			{
				this.L_Error("object length is not a number", new object[0]);
			}
			this.API.Pop(1);
			return result;
		}

		public ThreadStatus L_LoadBuffer(string s, string name)
		{
			return this.L_LoadBufferX(s, name, null);
		}

		public ThreadStatus L_LoadBufferX(string s, string name, string mode)
		{
			StringLoadInfo loadinfo = new StringLoadInfo(s);
			return this.API.Load(loadinfo, name, mode);
		}

		public ThreadStatus L_LoadBytes(byte[] bytes, string name)
		{
			BytesLoadInfo loadinfo = new BytesLoadInfo(bytes);
			return this.API.Load(loadinfo, name, null);
		}

		private ThreadStatus ErrFile(string what, int fnameindex)
		{
			return ThreadStatus.LUA_ERRFILE;
		}

		public ThreadStatus L_LoadFile(string filename)
		{
			return this.L_LoadFileX(filename, null);
		}

		public ThreadStatus L_LoadFileX(string filename, string mode)
		{
			ThreadStatus result = ThreadStatus.LUA_OK;
			if (filename == null)
			{
				throw new NotImplementedException();
			}
			int index = this.API.GetTop() + 1;
			this.API.PushString("@" + filename);
			try
			{
				using (FileLoadInfo fileLoadInfo = LuaFile.OpenFile(filename))
				{
					fileLoadInfo.SkipComment();
					result = this.API.Load(fileLoadInfo, this.API.ToString(-1), mode);
				}
			}
			catch (LuaRuntimeException ex)
			{
				this.API.PushString(string.Format("cannot open {0}: {1}", filename, ex.Message));
				return ThreadStatus.LUA_ERRFILE;
			}
			this.API.Remove(index);
			return result;
		}

		public ThreadStatus L_LoadString(string s)
		{
			return this.L_LoadBuffer(s, s);
		}

		public ThreadStatus L_DoString(string s)
		{
			ThreadStatus threadStatus = this.L_LoadString(s);
			if (threadStatus != ThreadStatus.LUA_OK)
			{
				return threadStatus;
			}
			return this.API.PCall(0, -1, 0);
		}

		public ThreadStatus L_DoFile(string filename)
		{
			ThreadStatus threadStatus = this.L_LoadFile(filename);
			if (threadStatus != ThreadStatus.LUA_OK)
			{
				return threadStatus;
			}
			return this.API.PCall(0, -1, 0);
		}

		public string L_Gsub(string src, string pattern, string rep)
		{
			string text = src.Replace(pattern, rep);
			this.API.PushString(text);
			return text;
		}

		public string L_ToString(int index)
		{
			if (!this.L_CallMeta(index, "__tostring"))
			{
				switch (this.API.Type(index))
				{
				case LuaType.LUA_TNIL:
					this.API.PushString("nil");
					goto IL_CD;
				case LuaType.LUA_TBOOLEAN:
					this.API.PushString((!this.API.ToBoolean(index)) ? "false" : "true");
					goto IL_CD;
				case LuaType.LUA_TNUMBER:
				case LuaType.LUA_TSTRING:
					this.API.PushValue(index);
					goto IL_CD;
				}
				this.API.PushString(string.Format("{0}: {1:X}", this.L_TypeName(index), this.API.ToObject(index).GetHashCode()));
			}
			IL_CD:
			return this.API.ToString(-1);
		}

		public void L_OpenLibs()
		{
			NameFuncPair[] array = new NameFuncPair[]
			{
				new NameFuncPair("_G", new CSharpFunctionDelegate(LuaBaseLib.OpenLib)),
				new NameFuncPair("package", new CSharpFunctionDelegate(LuaPkgLib.OpenLib)),
				new NameFuncPair("coroutine", new CSharpFunctionDelegate(LuaCoroLib.OpenLib)),
				new NameFuncPair("table", new CSharpFunctionDelegate(LuaTableLib.OpenLib)),
				new NameFuncPair("io", new CSharpFunctionDelegate(LuaIOLib.OpenLib)),
				new NameFuncPair("os", new CSharpFunctionDelegate(LuaOSLib.OpenLib)),
				new NameFuncPair("string", new CSharpFunctionDelegate(LuaStrLib.OpenLib)),
				new NameFuncPair("bit32", new CSharpFunctionDelegate(LuaBitLib.OpenLib)),
				new NameFuncPair("math", new CSharpFunctionDelegate(LuaMathLib.OpenLib)),
				new NameFuncPair("debug", new CSharpFunctionDelegate(LuaDebugLib.OpenLib)),
				new NameFuncPair("ffi.cs", new CSharpFunctionDelegate(LuaFFILib.OpenLib)),
				new NameFuncPair("enc", new CSharpFunctionDelegate(LuaEncLib.OpenLib))
			};
			for (int i = 0; i < array.Length; i++)
			{
				this.L_RequireF(array[i].Name, array[i].Func, true);
				this.API.Pop(1);
			}
		}

		public void L_RequireF(string moduleName, CSharpFunctionDelegate openFunc, bool global)
		{
			this.API.PushCSharpFunction(openFunc);
			this.API.PushString(moduleName);
			this.API.Call(1, 1);
			this.L_GetSubTable(-1001000, "_LOADED");
			this.API.PushValue(-2);
			this.API.SetField(-2, moduleName);
			this.API.Pop(1);
			if (global)
			{
				this.API.PushValue(-1);
				this.API.SetGlobal(moduleName);
			}
		}

		public int L_GetSubTable(int index, string fname)
		{
			this.API.GetField(index, fname);
			if (this.API.IsTable(-1))
			{
				return 1;
			}
			this.API.Pop(1);
			index = this.API.AbsIndex(index);
			this.API.NewTable();
			this.API.PushValue(-1);
			this.API.SetField(index, fname);
			return 0;
		}

		public void L_NewLibTable(NameFuncPair[] define)
		{
			this.API.CreateTable(0, define.Length);
		}

		public void L_NewLib(NameFuncPair[] define)
		{
			this.L_NewLibTable(define);
			this.L_SetFuncs(define, 0);
		}

		public void L_SetFuncs(NameFuncPair[] define, int nup)
		{
			this.L_CheckStack(nup, "too many upvalues");
			for (int i = 0; i < define.Length; i++)
			{
				for (int j = 0; j < nup; j++)
				{
					this.API.PushValue(-nup);
				}
				this.API.PushCSharpClosure(define[i].Func, nup);
				this.API.SetField(-(nup + 2), define[i].Name);
			}
			this.API.Pop(nup);
		}

		private bool FindField(int objIndex, int level)
		{
			if (level == 0 || !this.API.IsTable(-1))
			{
				return false;
			}
			this.API.PushNil();
			while (this.API.Next(-2))
			{
				if (this.API.Type(-2) == LuaType.LUA_TSTRING)
				{
					if (this.API.RawEqual(objIndex, -1))
					{
						this.API.Pop(1);
						return true;
					}
					if (this.FindField(objIndex, level - 1))
					{
						this.API.Remove(-2);
						this.API.PushString(".");
						this.API.Insert(-2);
						this.API.Concat(3);
						return true;
					}
				}
				this.API.Pop(1);
			}
			return false;
		}

		private bool PushGlobalFuncName(LuaDebug ar)
		{
			int top = this.API.GetTop();
			this.GetInfo("f", ar);
			this.API.PushGlobalTable();
			if (this.FindField(top + 1, 2))
			{
				this.API.Copy(-1, top + 1);
				this.API.Pop(2);
				return true;
			}
			this.API.SetTop(top);
			return false;
		}

		public int L_Ref(int t)
		{
			if (this.API.IsNil(-1))
			{
				this.API.Pop(1);
				return -1;
			}
			t = this.API.AbsIndex(t);
			this.API.RawGetI(t, 0);
			int num = this.API.ToInteger(-1);
			this.API.Pop(1);
			if (num != 0)
			{
				this.API.RawGetI(t, num);
				this.API.RawSetI(t, 0);
			}
			else
			{
				num = this.API.RawLen(t) + 1;
			}
			this.API.RawSetI(t, num);
			return num;
		}

		public void L_Unref(int t, int reference)
		{
			if (reference >= 0)
			{
				t = this.API.AbsIndex(t);
				this.API.RawGetI(t, 0);
				this.API.RawSetI(t, reference);
				this.API.PushInteger(reference);
				this.API.RawSetI(t, 0);
			}
		}

		public int GetInfo(string what, LuaDebug ar)
		{
			int num = 0;
			CallInfo callInfo;
			StkId stkId;
			if (what.get_Chars(num) == '>')
			{
				callInfo = null;
				stkId = this.Stack[this.Top.Index - 1];
				Utl.ApiCheck(stkId.V.TtIsFunction(), "function expected");
				num++;
				this.Top = this.Stack[this.Top.Index - 1];
			}
			else
			{
				callInfo = this.BaseCI[ar.ActiveCIIndex];
				stkId = this.Stack[callInfo.FuncIndex];
				Utl.Assert(this.Stack[callInfo.FuncIndex].V.TtIsFunction());
			}
			int result = this.AuxGetInfo(what, ar, stkId, callInfo);
			if (what.Contains("f"))
			{
				this.Top.V.SetObj(ref stkId.V);
				this.IncrTop();
			}
			if (what.Contains("L"))
			{
				this.CollectValidLines(stkId);
			}
			return result;
		}

		private int AuxGetInfo(string what, LuaDebug ar, StkId func, CallInfo ci)
		{
			int result = 1;
			for (int i = 0; i < what.Length; i++)
			{
				char c = what.get_Chars(i);
				char c2 = c;
				switch (c2)
				{
				case 'l':
					ar.CurrentLine = ((ci == null || !ci.IsLua) ? -1 : this.GetCurrentLine(ci));
					break;
				default:
					if (c2 != 't')
					{
						if (c2 != 'u')
						{
							if (c2 != 'L')
							{
								if (c2 != 'S')
								{
									if (c2 != 'f')
									{
										result = 0;
									}
								}
								else
								{
									this.FuncInfo(ar, func);
								}
							}
						}
						else
						{
							Utl.Assert(func.V.TtIsFunction());
							if (func.V.ClIsLuaClosure())
							{
								LuaLClosureValue luaLClosureValue = func.V.ClLValue();
								ar.NumUps = luaLClosureValue.Upvals.Length;
								ar.IsVarArg = luaLClosureValue.Proto.IsVarArg;
								ar.NumParams = luaLClosureValue.Proto.NumParams;
							}
							else
							{
								if (!func.V.ClIsCsClosure())
								{
									throw new NotImplementedException();
								}
								LuaCsClosureValue luaCsClosureValue = func.V.ClCsValue();
								ar.NumUps = luaCsClosureValue.Upvals.Length;
								ar.IsVarArg = true;
								ar.NumParams = 0;
							}
						}
					}
					else
					{
						ar.IsTailCall = (ci != null && (ci.CallStatus & CallStatus.CIST_TAIL) != CallStatus.CIST_NONE);
					}
					break;
				case 'n':
				{
					CallInfo callInfo = this.BaseCI[ci.Index - 1];
					if (ci != null && (ci.CallStatus & CallStatus.CIST_TAIL) == CallStatus.CIST_NONE && callInfo.IsLua)
					{
						ar.NameWhat = this.GetFuncName(callInfo, out ar.Name);
					}
					else
					{
						ar.NameWhat = null;
					}
					if (ar.NameWhat == null)
					{
						ar.NameWhat = string.Empty;
						ar.Name = null;
					}
					break;
				}
				}
			}
			return result;
		}

		private void CollectValidLines(StkId func)
		{
			Utl.Assert(func.V.TtIsFunction());
			if (func.V.ClIsLuaClosure())
			{
				LuaLClosureValue luaLClosureValue = func.V.ClLValue();
				LuaProto proto = luaLClosureValue.Proto;
				List<int> lineInfo = proto.LineInfo;
				LuaTable luaTable = new LuaTable(this);
				this.Top.V.SetHValue(luaTable);
				this.IncrTop();
				TValue tvalue = default(TValue);
				tvalue.SetBValue(true);
				for (int i = 0; i < lineInfo.Count; i++)
				{
					luaTable.SetInt(lineInfo[i], ref tvalue);
				}
			}
			else
			{
				if (!func.V.ClIsCsClosure())
				{
					throw new NotImplementedException();
				}
				this.Top.V.SetNilValue();
				this.IncrTop();
			}
		}

		private string GetFuncName(CallInfo ci, out string name)
		{
			LuaProto proto = this.GetCurrentLuaFunc(ci).Proto;
			int currentPc = ci.CurrentPc;
			Instruction instruction = proto.Code[currentPc];
			TMS tm;
			switch (instruction.GET_OPCODE())
			{
			case OpCode.OP_GETTABUP:
			case OpCode.OP_GETTABLE:
			case OpCode.OP_SELF:
				tm = TMS.TM_INDEX;
				goto IL_138;
			case OpCode.OP_SETTABUP:
			case OpCode.OP_SETTABLE:
				tm = TMS.TM_NEWINDEX;
				goto IL_138;
			case OpCode.OP_ADD:
				tm = TMS.TM_ADD;
				goto IL_138;
			case OpCode.OP_SUB:
				tm = TMS.TM_SUB;
				goto IL_138;
			case OpCode.OP_MUL:
				tm = TMS.TM_MUL;
				goto IL_138;
			case OpCode.OP_DIV:
				tm = TMS.TM_DIV;
				goto IL_138;
			case OpCode.OP_MOD:
				tm = TMS.TM_MOD;
				goto IL_138;
			case OpCode.OP_POW:
				tm = TMS.TM_POW;
				goto IL_138;
			case OpCode.OP_UNM:
				tm = TMS.TM_UNM;
				goto IL_138;
			case OpCode.OP_LEN:
				tm = TMS.TM_LEN;
				goto IL_138;
			case OpCode.OP_CONCAT:
				tm = TMS.TM_CONCAT;
				goto IL_138;
			case OpCode.OP_EQ:
				tm = TMS.TM_EQ;
				goto IL_138;
			case OpCode.OP_LT:
				tm = TMS.TM_LT;
				goto IL_138;
			case OpCode.OP_LE:
				tm = TMS.TM_LE;
				goto IL_138;
			case OpCode.OP_CALL:
			case OpCode.OP_TAILCALL:
				return this.GetObjName(proto, currentPc, instruction.GETARG_A(), out name);
			case OpCode.OP_TFORCALL:
				name = "for iterator";
				return "for iterator";
			}
			name = null;
			return null;
			IL_138:
			name = this.GetTagMethodName(tm);
			return "metamethod";
		}

		private void FuncInfo(LuaDebug ar, StkId func)
		{
			Utl.Assert(func.V.TtIsFunction());
			if (func.V.ClIsLuaClosure())
			{
				LuaLClosureValue luaLClosureValue = func.V.ClLValue();
				LuaProto proto = luaLClosureValue.Proto;
				ar.Source = ((!string.IsNullOrEmpty(proto.Source)) ? proto.Source : "=?");
				ar.LineDefined = proto.LineDefined;
				ar.LastLineDefined = proto.LastLineDefined;
				ar.What = ((ar.LineDefined != 0) ? "Lua" : "main");
			}
			else
			{
				if (!func.V.ClIsCsClosure())
				{
					throw new NotImplementedException();
				}
				ar.Source = "=[C#]";
				ar.LineDefined = -1;
				ar.LastLineDefined = -1;
				ar.What = "C#";
			}
			if (ar.Source.Length > 60)
			{
				ar.ShortSrc = ar.Source.Substring(0, 60);
			}
			else
			{
				ar.ShortSrc = ar.Source;
			}
		}

		private void AddInfo(string msg)
		{
			if (this.CI.IsLua)
			{
				int currentLine = this.GetCurrentLine(this.CI);
				string text = this.GetCurrentLuaFunc(this.CI).Proto.Source;
				if (text == null)
				{
					text = "?";
				}
				this.O_PushString(string.Format("{0}:{1}: {2}", text, currentLine, msg));
			}
		}

		internal void G_RunError(string fmt, params object[] args)
		{
			this.AddInfo(string.Format(fmt, args));
			this.G_ErrorMsg();
		}

		private void G_ErrorMsg()
		{
			if (this.ErrFunc != 0)
			{
				StkId stkId = this.RestoreStack(this.ErrFunc);
				if (!stkId.V.TtIsFunction())
				{
					this.D_Throw(ThreadStatus.LUA_ERRERR);
				}
				StkId stkId2 = this.Stack[this.Top.Index - 1];
				this.Top.V.SetObj(ref stkId2.V);
				stkId2.V.SetObj(ref stkId.V);
				this.IncrTop();
				this.D_Call(stkId2, 1, false);
			}
			this.D_Throw(ThreadStatus.LUA_ERRRUN);
		}

		private string UpvalName(LuaProto p, int uv)
		{
			return "(UpvalName:NotImplemented)";
		}

		private string GetUpvalueName(CallInfo ci, StkId o, out string name)
		{
			StkId stkId = this.Stack[ci.FuncIndex];
			Utl.Assert(stkId.V.TtIsFunction() && stkId.V.ClIsLuaClosure());
			LuaLClosureValue luaLClosureValue = stkId.V.ClLValue();
			for (int i = 0; i < luaLClosureValue.Upvals.Length; i++)
			{
				if (luaLClosureValue.Upvals[i].V == o)
				{
					name = this.UpvalName(luaLClosureValue.Proto, i);
					return "upvalue";
				}
			}
			name = null;
			return null;
		}

		private void KName(LuaProto proto, int pc, int c, out string name)
		{
			if (Instruction.ISK(c))
			{
				StkId stkId = proto.K[Instruction.INDEXK(c)];
				if (stkId.V.TtIsString())
				{
					name = stkId.V.SValue();
					return;
				}
			}
			else
			{
				string objName = this.GetObjName(proto, pc, c, out name);
				if (objName == "constant")
				{
					return;
				}
			}
			name = "?";
		}

		private int FindSetReg(LuaProto proto, int lastpc, int reg)
		{
			int result = -1;
			for (int i = 0; i < lastpc; i++)
			{
				Instruction instruction = proto.Code[i];
				OpCode opcode = instruction.GET_OPCODE();
				int arg_A = instruction.GETARG_A();
				OpCode opCode = opcode;
				switch (opCode)
				{
				case OpCode.OP_TEST:
					if (reg == arg_A)
					{
						result = i;
					}
					break;
				default:
					if (opCode != OpCode.OP_LOADNIL)
					{
						if (opCode != OpCode.OP_JMP)
						{
							if (Coder.TestAMode(opcode) && reg == arg_A)
							{
								result = i;
							}
						}
						else
						{
							int arg_sBx = instruction.GETARG_sBx();
							int num = i + 1 + arg_sBx;
							if (i < num && num <= lastpc)
							{
								i += arg_sBx;
							}
						}
					}
					else
					{
						int arg_B = instruction.GETARG_B();
						if (arg_A <= reg && reg <= arg_A + arg_B)
						{
							result = i;
						}
					}
					break;
				case OpCode.OP_CALL:
				case OpCode.OP_TAILCALL:
					if (reg >= arg_A)
					{
						result = i;
					}
					break;
				case OpCode.OP_TFORCALL:
					if (reg >= arg_A + 2)
					{
						result = i;
					}
					break;
				}
			}
			return result;
		}

		private string GetObjName(LuaProto proto, int lastpc, int reg, out string name)
		{
			name = this.F_GetLocalName(proto, reg + 1, lastpc);
			if (name != null)
			{
				return "local";
			}
			int num = this.FindSetReg(proto, lastpc, reg);
			if (num != -1)
			{
				Instruction instruction = proto.Code[num];
				OpCode opcode = instruction.GET_OPCODE();
				switch (opcode)
				{
				case OpCode.OP_MOVE:
				{
					int arg_B = instruction.GETARG_B();
					if (arg_B < instruction.GETARG_A())
					{
						return this.GetObjName(proto, num, arg_B, out name);
					}
					break;
				}
				case OpCode.OP_LOADK:
				case OpCode.OP_LOADKX:
				{
					int num2 = (opcode != OpCode.OP_LOADK) ? proto.Code[num + 1].GETARG_Ax() : instruction.GETARG_Bx();
					StkId stkId = proto.K[num2];
					if (stkId.V.TtIsString())
					{
						name = stkId.V.SValue();
						return "constant";
					}
					break;
				}
				case OpCode.OP_GETUPVAL:
					name = this.UpvalName(proto, instruction.GETARG_B());
					return "upvalue";
				case OpCode.OP_GETTABUP:
				case OpCode.OP_GETTABLE:
				{
					int arg_C = instruction.GETARG_C();
					int arg_B2 = instruction.GETARG_B();
					string text = (opcode != OpCode.OP_GETTABLE) ? this.UpvalName(proto, arg_B2) : this.F_GetLocalName(proto, arg_B2 + 1, num);
					this.KName(proto, num, arg_C, out name);
					return (!(text == "_ENV")) ? "field" : "global";
				}
				case OpCode.OP_SELF:
				{
					int arg_C2 = instruction.GETARG_C();
					this.KName(proto, num, arg_C2, out name);
					return "method";
				}
				}
			}
			return null;
		}

		private bool IsInStack(CallInfo ci, StkId o)
		{
			return false;
		}

		private void G_SimpleTypeError(ref TValue o, string op)
		{
			string text = this.ObjTypeName(ref o);
			this.G_RunError("attempt to {0} a {1} value", new object[]
			{
				op,
				text
			});
		}

		private void G_TypeError(StkId o, string op)
		{
			CallInfo ci = this.CI;
			string text = null;
			string text2 = null;
			string text3 = this.ObjTypeName(ref o.V);
			if (ci.IsLua)
			{
				text2 = this.GetUpvalueName(ci, o, out text);
				if (text2 != null && this.IsInStack(ci, o))
				{
					LuaLClosureValue luaLClosureValue = this.Stack[ci.FuncIndex].V.ClLValue();
					text2 = this.GetObjName(luaLClosureValue.Proto, ci.CurrentPc, o.Index - ci.BaseIndex, out text);
				}
			}
			if (text2 != null)
			{
				this.G_RunError("attempt to {0} {1} '{2}' (a {3} value)", new object[]
				{
					op,
					text2,
					text,
					text3
				});
			}
			else
			{
				this.G_RunError("attempt to {0} a {1} value", new object[]
				{
					op,
					text3
				});
			}
		}

		private void G_ArithError(StkId p1, StkId p2)
		{
			TValue tvalue = default(TValue);
			if (!this.V_ToNumber(p1, ref tvalue))
			{
				p2 = p1;
			}
			this.G_TypeError(p2, "perform arithmetic on");
		}

		private void G_OrderError(StkId p1, StkId p2)
		{
			string text = this.ObjTypeName(ref p1.V);
			string text2 = this.ObjTypeName(ref p2.V);
			if (text == text2)
			{
				this.G_RunError("attempt to compare two {0} values", new object[]
				{
					text
				});
			}
			else
			{
				this.G_RunError("attempt to compare {0} with {1}", new object[]
				{
					text,
					text2
				});
			}
		}

		private void G_ConcatError(StkId p1, StkId p2)
		{
		}

		private LuaUpvalue F_FindUpval(StkId level)
		{
			LinkedListNode<LuaUpvalue> linkedListNode = this.OpenUpval.First;
			LinkedListNode<LuaUpvalue> linkedListNode2 = null;
			while (linkedListNode != null)
			{
				LuaUpvalue value = linkedListNode.Value;
				if (value.V.Index < level.Index)
				{
					break;
				}
				LinkedListNode<LuaUpvalue> next = linkedListNode.Next;
				if (value.V == level)
				{
					return value;
				}
				linkedListNode2 = linkedListNode;
				linkedListNode = next;
			}
			LuaUpvalue luaUpvalue = new LuaUpvalue();
			luaUpvalue.V = level;
			if (linkedListNode2 == null)
			{
				this.OpenUpval.AddFirst(luaUpvalue);
			}
			else
			{
				this.OpenUpval.AddAfter(linkedListNode2, luaUpvalue);
			}
			return luaUpvalue;
		}

		private void F_Close(StkId level)
		{
			LinkedListNode<LuaUpvalue> linkedListNode = this.OpenUpval.First;
			while (linkedListNode != null)
			{
				LuaUpvalue value = linkedListNode.Value;
				if (value.V.Index < level.Index)
				{
					break;
				}
				LinkedListNode<LuaUpvalue> next = linkedListNode.Next;
				this.OpenUpval.Remove(linkedListNode);
				linkedListNode = next;
				value.Value.V.SetObj(ref value.V.V);
				value.V = value.Value;
			}
		}

		private string F_GetLocalName(LuaProto proto, int localNumber, int pc)
		{
			int num = 0;
			while (num < proto.LocVars.Count && proto.LocVars[num].StartPc <= pc)
			{
				if (pc < proto.LocVars[num].EndPc)
				{
					localNumber--;
					if (localNumber == 0)
					{
						return proto.LocVars[num].VarName;
					}
				}
				num++;
			}
			return null;
		}

		public static bool O_Str2Decimal(string s, out double result)
		{
			result = 0.0;
			if (s.Contains("n") || s.Contains("N"))
			{
				return false;
			}
			int num = 0;
			if (s.Contains("x") || s.Contains("X"))
			{
				result = Utl.StrX2Number(s, ref num);
			}
			else
			{
				result = Utl.Str2Number(s, ref num);
			}
			if (num == 0)
			{
				return false;
			}
			while (num < s.Length && char.IsWhiteSpace(s.get_Chars(num)))
			{
				num++;
			}
			return num == s.Length;
		}

		private static double O_Arith(LuaOp op, double v1, double v2)
		{
			switch (op)
			{
			case LuaOp.LUA_OPADD:
				return v1 + v2;
			case LuaOp.LUA_OPSUB:
				return v1 - v2;
			case LuaOp.LUA_OPMUL:
				return v1 * v2;
			case LuaOp.LUA_OPDIV:
				return v1 / v2;
			case LuaOp.LUA_OPMOD:
				return v1 % v2;
			case LuaOp.LUA_OPPOW:
				return Math.Pow(v1, v2);
			case LuaOp.LUA_OPUNM:
				return -v1;
			default:
				throw new NotImplementedException();
			}
		}

		private bool IsFalse(ref TValue v)
		{
			return v.TtIsNil() || (v.TtIsBoolean() && !v.BValue());
		}

		private bool ToString(ref TValue o)
		{
			return o.TtIsString() || this.V_ToString(ref o);
		}

		internal LuaLClosureValue GetCurrentLuaFunc(CallInfo ci)
		{
			if (ci.IsLua)
			{
				return this.Stack[ci.FuncIndex].V.ClLValue();
			}
			return null;
		}

		internal int GetCurrentLine(CallInfo ci)
		{
			Utl.Assert(ci.IsLua);
			LuaLClosureValue luaLClosureValue = this.Stack[ci.FuncIndex].V.ClLValue();
			return luaLClosureValue.Proto.GetFuncLine(ci.CurrentPc);
		}

		public ThreadStatus Status { get; set; }

		private void IncrTop()
		{
			StkId.inc(ref this.Top);
			this.D_CheckStack(0);
		}

		private StkId RestoreStack(int index)
		{
			return this.Stack[index];
		}

		private void ApiIncrTop()
		{
			StkId.inc(ref this.Top);
			Utl.ApiCheck(this.Top.Index <= this.CI.TopIndex, "stack overflow");
		}

		private void InitStack()
		{
			this.Stack = new StkId[40];
			this.StackSize = 40;
			this.StackLast = 35;
			for (int i = 0; i < 40; i++)
			{
				StkId stkId = new StkId();
				this.Stack[i] = stkId;
				stkId.SetList(this.Stack);
				stkId.SetIndex(i);
				stkId.V.SetNilValue();
			}
			this.Top = this.Stack[0];
			this.BaseCI = new CallInfo[8];
			for (int j = 0; j < 8; j++)
			{
				CallInfo callInfo = new CallInfo();
				this.BaseCI[j] = callInfo;
				callInfo.List = this.BaseCI;
				callInfo.Index = j;
			}
			this.CI = this.BaseCI[0];
			this.CI.FuncIndex = this.Top.Index;
			StkId.inc(ref this.Top).V.SetNilValue();
			this.CI.TopIndex = this.Top.Index + 20;
		}

		private void InitRegistry()
		{
			TValue tvalue = default(TValue);
			this.G.Registry.V.SetHValue(new LuaTable(this));
			tvalue.SetThValue(this);
			this.G.Registry.V.HValue().SetInt(1, ref tvalue);
			tvalue.SetHValue(new LuaTable(this));
			this.G.Registry.V.HValue().SetInt(2, ref tvalue);
		}

		private string DumpStackToString(int baseIndex, string tag = "")
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format("===================================================================== DumpStack: {0}", tag)).Append("\n");
			stringBuilder.Append(string.Format("== BaseIndex: {0}", baseIndex)).Append("\n");
			stringBuilder.Append(string.Format("== Top.Index: {0}", this.Top.Index)).Append("\n");
			stringBuilder.Append(string.Format("== CI.Index: {0}", this.CI.Index)).Append("\n");
			stringBuilder.Append(string.Format("== CI.TopIndex: {0}", this.CI.TopIndex)).Append("\n");
			stringBuilder.Append(string.Format("== CI.Func.Index: {0}", this.CI.FuncIndex)).Append("\n");
			int num = 0;
			while (num < this.Stack.Length || num <= this.Top.Index)
			{
				bool flag = this.Top.Index == num;
				bool flag2 = baseIndex == num;
				bool flag3 = num < this.Stack.Length;
				string text = (!flag && !flag2) ? string.Empty : string.Format("<--------------------- {0}{1}", (!flag2) ? string.Empty : "[BASE]", (!flag) ? string.Empty : "[TOP]");
				string text2 = string.Format("======== {0}/{1} > {2} {3}", new object[]
				{
					num - baseIndex,
					num,
					(!flag3) ? string.Empty : this.Stack[num].ToString(),
					text
				});
				stringBuilder.Append(text2).Append("\n");
				num++;
			}
			return stringBuilder.ToString();
		}

		public void DumpStack(int baseIndex, string tag = "")
		{
		}

		private void ResetHookCount()
		{
			this.HookCount = this.BaseHookCount;
		}

		private string GetTagMethodName(TMS tm)
		{
			switch (tm)
			{
			case TMS.TM_INDEX:
				return "__index";
			case TMS.TM_NEWINDEX:
				return "__newindex";
			case TMS.TM_GC:
				return "__gc";
			case TMS.TM_MODE:
				return "__mode";
			case TMS.TM_LEN:
				return "__len";
			case TMS.TM_EQ:
				return "__eq";
			case TMS.TM_ADD:
				return "__add";
			case TMS.TM_SUB:
				return "__sub";
			case TMS.TM_MUL:
				return "__mul";
			case TMS.TM_DIV:
				return "__div";
			case TMS.TM_MOD:
				return "__mod";
			case TMS.TM_POW:
				return "__pow";
			case TMS.TM_UNM:
				return "__unm";
			case TMS.TM_LT:
				return "__lt";
			case TMS.TM_LE:
				return "__le";
			case TMS.TM_CONCAT:
				return "__concat";
			case TMS.TM_CALL:
				return "__call";
			default:
				throw new NotImplementedException();
			}
		}

		private StkId T_GetTM(LuaTable mt, TMS tm)
		{
			if (mt == null)
			{
				return null;
			}
			StkId str = mt.GetStr(this.GetTagMethodName(tm));
			if (str.V.TtIsNil())
			{
				mt.NoTagMethodFlags |= 1U << (int)tm;
				return null;
			}
			return str;
		}

		private StkId T_GetTMByObj(ref TValue o, TMS tm)
		{
			LuaTable luaTable2;
			switch (o.Tt)
			{
			case 5:
			{
				LuaTable luaTable = o.HValue();
				luaTable2 = luaTable.MetaTable;
				goto IL_60;
			}
			case 7:
			{
				LuaUserDataValue luaUserDataValue = o.RawUValue();
				luaTable2 = luaUserDataValue.MetaTable;
				goto IL_60;
			}
			}
			luaTable2 = this.G.MetaTables[o.Tt];
			IL_60:
			return (luaTable2 == null) ? LuaState.TheNilValue : luaTable2.GetStr(this.GetTagMethodName(tm));
		}

		private void V_Execute()
		{
			CallInfo callInfo = this.CI;
			for (;;)
			{
				Utl.Assert(callInfo == this.CI);
				LuaLClosureValue luaLClosureValue = this.Stack[callInfo.FuncIndex].V.ClLValue();
				LuaState.ExecuteEnvironment executeEnvironment;
				executeEnvironment.Stack = this.Stack;
				executeEnvironment.K = luaLClosureValue.Proto.K;
				executeEnvironment.Base = callInfo.BaseIndex;
				Instruction valueInc;
				StkId ra;
				for (;;)
				{
					valueInc = callInfo.SavedPc.ValueInc;
					executeEnvironment.I = valueInc;
					ra = executeEnvironment.RA;
					switch (valueInc.GET_OPCODE())
					{
					case OpCode.OP_MOVE:
					{
						StkId rb = executeEnvironment.RB;
						ra.V.SetObj(ref rb.V);
						break;
					}
					case OpCode.OP_LOADK:
					{
						StkId stkId = executeEnvironment.K[valueInc.GETARG_Bx()];
						ra.V.SetObj(ref stkId.V);
						break;
					}
					case OpCode.OP_LOADKX:
					{
						Utl.Assert(callInfo.SavedPc.Value.GET_OPCODE() == OpCode.OP_EXTRAARG);
						StkId stkId2 = executeEnvironment.K[callInfo.SavedPc.ValueInc.GETARG_Ax()];
						ra.V.SetObj(ref stkId2.V);
						break;
					}
					case OpCode.OP_LOADBOOL:
						ra.V.SetBValue(valueInc.GETARG_B() != 0);
						if (valueInc.GETARG_C() != 0)
						{
							CallInfo callInfo2 = callInfo;
							callInfo2.SavedPc.Index = callInfo2.SavedPc.Index + 1;
						}
						break;
					case OpCode.OP_LOADNIL:
					{
						int arg_B = valueInc.GETARG_B();
						int index = ra.Index;
						do
						{
							this.Stack[index++].V.SetNilValue();
						}
						while (arg_B-- > 0);
						break;
					}
					case OpCode.OP_GETUPVAL:
					{
						int arg_B2 = valueInc.GETARG_B();
						ra.V.SetObj(ref luaLClosureValue.Upvals[arg_B2].V.V);
						break;
					}
					case OpCode.OP_GETTABUP:
					{
						int arg_B3 = valueInc.GETARG_B();
						StkId rkc = executeEnvironment.RKC;
						this.V_GetTable(luaLClosureValue.Upvals[arg_B3].V, rkc, ra);
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					}
					case OpCode.OP_GETTABLE:
					{
						StkId rb2 = executeEnvironment.RB;
						StkId rkc2 = executeEnvironment.RKC;
						StkId val = ra;
						this.V_GetTable(rb2, rkc2, val);
						break;
					}
					case OpCode.OP_SETTABUP:
					{
						int arg_A = valueInc.GETARG_A();
						StkId rkb = executeEnvironment.RKB;
						StkId rkc3 = executeEnvironment.RKC;
						this.V_SetTable(luaLClosureValue.Upvals[arg_A].V, rkb, rkc3);
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					}
					case OpCode.OP_SETUPVAL:
					{
						int arg_B4 = valueInc.GETARG_B();
						LuaUpvalue luaUpvalue = luaLClosureValue.Upvals[arg_B4];
						luaUpvalue.V.V.SetObj(ref ra.V);
						break;
					}
					case OpCode.OP_SETTABLE:
					{
						StkId rkb2 = executeEnvironment.RKB;
						StkId rkc4 = executeEnvironment.RKC;
						this.V_SetTable(ra, rkb2, rkc4);
						break;
					}
					case OpCode.OP_NEWTABLE:
					{
						int arg_B5 = valueInc.GETARG_B();
						int arg_C = valueInc.GETARG_C();
						LuaTable luaTable = new LuaTable(this);
						ra.V.SetHValue(luaTable);
						if (arg_B5 > 0 || arg_C > 0)
						{
							luaTable.Resize(arg_B5, arg_C);
						}
						break;
					}
					case OpCode.OP_SELF:
					{
						StkId stkId3 = this.Stack[ra.Index + 1];
						StkId rb3 = executeEnvironment.RB;
						stkId3.V.SetObj(ref rb3.V);
						this.V_GetTable(rb3, executeEnvironment.RKC, ra);
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					}
					case OpCode.OP_ADD:
					{
						StkId rkb3 = executeEnvironment.RKB;
						StkId rkc5 = executeEnvironment.RKC;
						if (rkb3.V.TtIsNumber() && rkc5.V.TtIsNumber())
						{
							ra.V.SetNValue(rkb3.V.NValue + rkc5.V.NValue);
						}
						else
						{
							this.V_Arith(ra, rkb3, rkc5, TMS.TM_ADD);
						}
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					}
					case OpCode.OP_SUB:
					{
						StkId rkb4 = executeEnvironment.RKB;
						StkId rkc6 = executeEnvironment.RKC;
						if (rkb4.V.TtIsNumber() && rkc6.V.TtIsNumber())
						{
							ra.V.SetNValue(rkb4.V.NValue - rkc6.V.NValue);
						}
						else
						{
							this.V_Arith(ra, rkb4, rkc6, TMS.TM_SUB);
						}
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					}
					case OpCode.OP_MUL:
					{
						StkId rkb5 = executeEnvironment.RKB;
						StkId rkc7 = executeEnvironment.RKC;
						if (rkb5.V.TtIsNumber() && rkc7.V.TtIsNumber())
						{
							ra.V.SetNValue(rkb5.V.NValue * rkc7.V.NValue);
						}
						else
						{
							this.V_Arith(ra, rkb5, rkc7, TMS.TM_MUL);
						}
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					}
					case OpCode.OP_DIV:
					{
						StkId rkb6 = executeEnvironment.RKB;
						StkId rkc8 = executeEnvironment.RKC;
						if (rkb6.V.TtIsNumber() && rkc8.V.TtIsNumber())
						{
							ra.V.SetNValue(rkb6.V.NValue / rkc8.V.NValue);
						}
						else
						{
							this.V_Arith(ra, rkb6, rkc8, TMS.TM_DIV);
						}
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					}
					case OpCode.OP_MOD:
					{
						StkId rkb7 = executeEnvironment.RKB;
						StkId rkc9 = executeEnvironment.RKC;
						if (rkb7.V.TtIsNumber() && rkc9.V.TtIsNumber())
						{
							ra.V.SetNValue(rkb7.V.NValue % rkc9.V.NValue);
						}
						else
						{
							this.V_Arith(ra, rkb7, rkc9, TMS.TM_MOD);
						}
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					}
					case OpCode.OP_POW:
					{
						StkId rkb8 = executeEnvironment.RKB;
						StkId rkc10 = executeEnvironment.RKC;
						if (rkb8.V.TtIsNumber() && rkc10.V.TtIsNumber())
						{
							ra.V.SetNValue(Math.Pow(rkb8.V.NValue, rkc10.V.NValue));
						}
						else
						{
							this.V_Arith(ra, rkb8, rkc10, TMS.TM_POW);
						}
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					}
					case OpCode.OP_UNM:
					{
						StkId rb4 = executeEnvironment.RB;
						if (rb4.V.TtIsNumber())
						{
							ra.V.SetNValue(-rb4.V.NValue);
						}
						else
						{
							this.V_Arith(ra, rb4, rb4, TMS.TM_UNM);
							executeEnvironment.Base = callInfo.BaseIndex;
						}
						break;
					}
					case OpCode.OP_NOT:
					{
						StkId rb5 = executeEnvironment.RB;
						ra.V.SetBValue(this.IsFalse(ref rb5.V));
						break;
					}
					case OpCode.OP_LEN:
						this.V_ObjLen(ra, executeEnvironment.RB);
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					case OpCode.OP_CONCAT:
					{
						int arg_B6 = valueInc.GETARG_B();
						int arg_C2 = valueInc.GETARG_C();
						this.Top = this.Stack[executeEnvironment.Base + arg_C2 + 1];
						this.V_Concat(arg_C2 - arg_B6 + 1);
						executeEnvironment.Base = callInfo.BaseIndex;
						ra = executeEnvironment.RA;
						StkId rb6 = executeEnvironment.RB;
						ra.V.SetObj(ref rb6.V);
						this.Top = this.Stack[callInfo.TopIndex];
						break;
					}
					case OpCode.OP_JMP:
						this.V_DoJump(callInfo, valueInc, 0);
						break;
					case OpCode.OP_EQ:
					{
						StkId rkb9 = executeEnvironment.RKB;
						StkId rkc11 = executeEnvironment.RKC;
						bool flag = valueInc.GETARG_A() != 0;
						if (rkb9.V == rkc11.V != flag)
						{
							CallInfo callInfo3 = callInfo;
							callInfo3.SavedPc.Index = callInfo3.SavedPc.Index + 1;
						}
						else
						{
							this.V_DoNextJump(callInfo);
						}
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					}
					case OpCode.OP_LT:
					{
						bool flag2 = valueInc.GETARG_A() != 0;
						if (this.V_LessThan(executeEnvironment.RKB, executeEnvironment.RKC) != flag2)
						{
							CallInfo callInfo4 = callInfo;
							callInfo4.SavedPc.Index = callInfo4.SavedPc.Index + 1;
						}
						else
						{
							this.V_DoNextJump(callInfo);
						}
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					}
					case OpCode.OP_LE:
					{
						bool flag3 = valueInc.GETARG_A() != 0;
						if (this.V_LessEqual(executeEnvironment.RKB, executeEnvironment.RKC) != flag3)
						{
							CallInfo callInfo5 = callInfo;
							callInfo5.SavedPc.Index = callInfo5.SavedPc.Index + 1;
						}
						else
						{
							this.V_DoNextJump(callInfo);
						}
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					}
					case OpCode.OP_TEST:
						if ((valueInc.GETARG_C() == 0) ? (!this.IsFalse(ref ra.V)) : this.IsFalse(ref ra.V))
						{
							CallInfo callInfo6 = callInfo;
							callInfo6.SavedPc.Index = callInfo6.SavedPc.Index + 1;
						}
						else
						{
							this.V_DoNextJump(callInfo);
						}
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					case OpCode.OP_TESTSET:
					{
						StkId rb7 = executeEnvironment.RB;
						if ((valueInc.GETARG_C() == 0) ? (!this.IsFalse(ref rb7.V)) : this.IsFalse(ref rb7.V))
						{
							CallInfo callInfo7 = callInfo;
							callInfo7.SavedPc.Index = callInfo7.SavedPc.Index + 1;
						}
						else
						{
							ra.V.SetObj(ref rb7.V);
							this.V_DoNextJump(callInfo);
						}
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					}
					case OpCode.OP_CALL:
					{
						int arg_B7 = valueInc.GETARG_B();
						int num = valueInc.GETARG_C() - 1;
						if (arg_B7 != 0)
						{
							this.Top = this.Stack[ra.Index + arg_B7];
						}
						if (!this.D_PreCall(ra, num))
						{
							goto IL_AA4;
						}
						if (num >= 0)
						{
							this.Top = this.Stack[callInfo.TopIndex];
						}
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					}
					case OpCode.OP_TAILCALL:
					{
						int arg_B8 = valueInc.GETARG_B();
						if (arg_B8 != 0)
						{
							this.Top = this.Stack[ra.Index + arg_B8];
						}
						Utl.Assert(valueInc.GETARG_C() - 1 == -1);
						bool flag4 = this.D_PreCall(ra, -1);
						if (!flag4)
						{
							goto IL_B1F;
						}
						executeEnvironment.Base = callInfo.BaseIndex;
						break;
					}
					case OpCode.OP_RETURN:
						goto IL_CC2;
					case OpCode.OP_FORLOOP:
					{
						StkId stkId4 = this.Stack[ra.Index + 1];
						StkId stkId5 = this.Stack[ra.Index + 2];
						StkId stkId6 = this.Stack[ra.Index + 3];
						double nvalue = stkId5.V.NValue;
						double num2 = ra.V.NValue + nvalue;
						double nvalue2 = stkId4.V.NValue;
						if ((0.0 >= nvalue) ? (nvalue2 <= num2) : (num2 <= nvalue2))
						{
							CallInfo callInfo8 = callInfo;
							callInfo8.SavedPc.Index = callInfo8.SavedPc.Index + valueInc.GETARG_sBx();
							ra.V.SetNValue(num2);
							stkId6.V.SetNValue(num2);
						}
						break;
					}
					case OpCode.OP_FORPREP:
					{
						TValue tvalue = default(TValue);
						TValue tvalue2 = default(TValue);
						TValue tvalue3 = default(TValue);
						StkId obj = this.Stack[ra.Index + 1];
						StkId obj2 = this.Stack[ra.Index + 2];
						if (!this.V_ToNumber(ra, ref tvalue))
						{
							this.G_RunError("'for' initial value must be a number", new object[0]);
						}
						if (!this.V_ToNumber(obj, ref tvalue2))
						{
							this.G_RunError("'for' limit must be a number", new object[0]);
						}
						if (!this.V_ToNumber(obj2, ref tvalue3))
						{
							this.G_RunError("'for' step must be a number", new object[0]);
						}
						ra.V.SetNValue(tvalue.NValue - tvalue3.NValue);
						CallInfo callInfo9 = callInfo;
						callInfo9.SavedPc.Index = callInfo9.SavedPc.Index + valueInc.GETARG_sBx();
						break;
					}
					case OpCode.OP_TFORCALL:
					{
						int index2 = ra.Index;
						int num3 = ra.Index + 3;
						this.Stack[num3 + 2].V.SetObj(ref this.Stack[index2 + 2].V);
						this.Stack[num3 + 1].V.SetObj(ref this.Stack[index2 + 1].V);
						this.Stack[num3].V.SetObj(ref this.Stack[index2].V);
						StkId func = this.Stack[num3];
						this.Top = this.Stack[num3 + 3];
						this.D_Call(func, valueInc.GETARG_C(), true);
						executeEnvironment.Base = callInfo.BaseIndex;
						this.Top = this.Stack[callInfo.TopIndex];
						valueInc = callInfo.SavedPc.ValueInc;
						executeEnvironment.I = valueInc;
						ra = executeEnvironment.RA;
						this.DumpStack(executeEnvironment.Base, string.Empty);
						Utl.Assert(valueInc.GET_OPCODE() == OpCode.OP_TFORLOOP);
						goto IL_1005;
					}
					case OpCode.OP_TFORLOOP:
						goto IL_1005;
					case OpCode.OP_SETLIST:
					{
						int i = valueInc.GETARG_B();
						int num4 = valueInc.GETARG_C();
						if (i == 0)
						{
							i = this.Top.Index - ra.Index - 1;
						}
						if (num4 == 0)
						{
							Utl.Assert(callInfo.SavedPc.Value.GET_OPCODE() == OpCode.OP_EXTRAARG);
							num4 = callInfo.SavedPc.ValueInc.GETARG_Ax();
						}
						LuaTable luaTable2 = ra.V.HValue();
						Utl.Assert(luaTable2 != null);
						int num5 = (num4 - 1) * 50 + i;
						int index3 = ra.Index;
						while (i > 0)
						{
							luaTable2.SetInt(num5--, ref this.Stack[index3 + i].V);
							i--;
						}
						this.Top = this.Stack[callInfo.TopIndex];
						break;
					}
					case OpCode.OP_CLOSURE:
					{
						LuaProto p = luaLClosureValue.Proto.P[valueInc.GETARG_Bx()];
						this.V_PushClosure(p, luaLClosureValue.Upvals, executeEnvironment.Base, ra);
						break;
					}
					case OpCode.OP_VARARG:
					{
						int num6 = valueInc.GETARG_B() - 1;
						int num7 = executeEnvironment.Base - callInfo.FuncIndex - luaLClosureValue.Proto.NumParams - 1;
						if (num6 < 0)
						{
							num6 = num7;
							this.D_CheckStack(num7);
							ra = executeEnvironment.RA;
							this.Top = this.Stack[ra.Index + num7];
						}
						int index4 = ra.Index;
						int num8 = executeEnvironment.Base - num7;
						for (int j = 0; j < num6; j++)
						{
							if (j < num7)
							{
								this.Stack[index4++].V.SetObj(ref this.Stack[num8++].V);
							}
							else
							{
								this.Stack[index4++].V.SetNilValue();
							}
						}
						break;
					}
					case OpCode.OP_EXTRAARG:
						Utl.Assert(false);
						this.V_NotImplemented(valueInc);
						break;
					default:
						this.V_NotImplemented(valueInc);
						break;
					}
					continue;
					IL_1005:
					StkId stkId7 = this.Stack[ra.Index + 1];
					if (!stkId7.V.TtIsNil())
					{
						ra.V.SetObj(ref stkId7.V);
						callInfo.SavedPc += valueInc.GETARG_sBx();
					}
				}
				IL_AA4:
				callInfo = this.CI;
				callInfo.CallStatus |= CallStatus.CIST_REENTRY;
				continue;
				IL_B1F:
				CallInfo ci = this.CI;
				CallInfo callInfo10 = this.BaseCI[this.CI.Index - 1];
				StkId stkId8 = this.Stack[ci.FuncIndex];
				StkId stkId9 = this.Stack[callInfo10.FuncIndex];
				LuaLClosureValue luaLClosureValue2 = stkId8.V.ClLValue();
				LuaLClosureValue luaLClosureValue3 = stkId9.V.ClLValue();
				int num9 = ci.BaseIndex + luaLClosureValue2.Proto.NumParams;
				if (luaLClosureValue.Proto.P.Count > 0)
				{
					this.F_Close(this.Stack[executeEnvironment.Base]);
				}
				int k = stkId8.Index;
				int index5 = stkId9.Index;
				while (k < num9)
				{
					this.Stack[index5++].V.SetObj(ref this.Stack[k++].V);
				}
				callInfo10.BaseIndex = stkId9.Index + (ci.BaseIndex - stkId8.Index);
				callInfo10.TopIndex = stkId9.Index + (this.Top.Index - stkId8.Index);
				this.Top = this.Stack[callInfo10.TopIndex];
				callInfo10.SavedPc = ci.SavedPc;
				callInfo10.CallStatus |= CallStatus.CIST_TAIL;
				callInfo = (this.CI = callInfo10);
				luaLClosureValue3 = stkId9.V.ClLValue();
				Utl.Assert(this.Top.Index == callInfo10.BaseIndex + (int)luaLClosureValue3.Proto.MaxStackSize);
				continue;
				IL_CC2:
				int num10 = valueInc.GETARG_B();
				if (num10 != 0)
				{
					this.Top = this.Stack[ra.Index + num10 - 1];
				}
				if (luaLClosureValue.Proto.P.Count > 0)
				{
					this.F_Close(this.Stack[executeEnvironment.Base]);
				}
				num10 = this.D_PosCall(ra.Index);
				if ((callInfo.CallStatus & CallStatus.CIST_REENTRY) == CallStatus.CIST_NONE)
				{
					break;
				}
				callInfo = this.CI;
				if (num10 != 0)
				{
					this.Top = this.Stack[callInfo.TopIndex];
				}
			}
		}

		private void V_NotImplemented(Instruction i)
		{
			ULDebug.LogError.Invoke("[VM] ==================================== Not Implemented Instruction: " + i);
		}

		private StkId FastTM(LuaTable et, TMS tm)
		{
			if (et == null)
			{
				return null;
			}
			if ((et.NoTagMethodFlags & 1U << (int)tm) != 0U)
			{
				return null;
			}
			return this.T_GetTM(et, tm);
		}

		private void V_GetTable(StkId t, StkId key, StkId val)
		{
			for (int i = 0; i < 100; i++)
			{
				StkId stkId2;
				if (t.V.TtIsTable())
				{
					LuaTable luaTable = t.V.HValue();
					StkId stkId = luaTable.Get(ref key.V);
					if (!stkId.V.TtIsNil())
					{
						val.V.SetObj(ref stkId.V);
						return;
					}
					stkId2 = this.FastTM(luaTable.MetaTable, TMS.TM_INDEX);
					if (stkId2 == null)
					{
						val.V.SetObj(ref stkId.V);
						return;
					}
				}
				else
				{
					stkId2 = this.T_GetTMByObj(ref t.V, TMS.TM_INDEX);
					if (stkId2.V.TtIsNil())
					{
						this.G_SimpleTypeError(ref t.V, "index");
					}
				}
				if (stkId2.V.TtIsFunction())
				{
					this.CallTM(ref stkId2.V, ref t.V, ref key.V, val, true);
					return;
				}
				t = stkId2;
			}
			this.G_RunError("loop in gettable", new object[0]);
		}

		private void V_SetTable(StkId t, StkId key, StkId val)
		{
			for (int i = 0; i < 100; i++)
			{
				StkId stkId2;
				if (t.V.TtIsTable())
				{
					LuaTable luaTable = t.V.HValue();
					StkId stkId = luaTable.Get(ref key.V);
					if (!stkId.V.TtIsNil())
					{
						luaTable.Set(ref key.V, ref val.V);
						return;
					}
					stkId2 = this.FastTM(luaTable.MetaTable, TMS.TM_NEWINDEX);
					if (stkId2 == null)
					{
						luaTable.Set(ref key.V, ref val.V);
						return;
					}
				}
				else
				{
					stkId2 = this.T_GetTMByObj(ref t.V, TMS.TM_NEWINDEX);
					if (stkId2.V.TtIsNil())
					{
						this.G_SimpleTypeError(ref t.V, "index");
					}
				}
				if (stkId2.V.TtIsFunction())
				{
					this.CallTM(ref stkId2.V, ref t.V, ref key.V, val, false);
					return;
				}
				t = stkId2;
			}
			this.G_RunError("loop in settable", new object[0]);
		}

		private void V_PushClosure(LuaProto p, LuaUpvalue[] encup, int stackBase, StkId ra)
		{
			LuaLClosureValue luaLClosureValue = new LuaLClosureValue(p);
			ra.V.SetClLValue(luaLClosureValue);
			for (int i = 0; i < p.Upvalues.Count; i++)
			{
				if (p.Upvalues[i].InStack)
				{
					luaLClosureValue.Upvals[i] = this.F_FindUpval(this.Stack[stackBase + p.Upvalues[i].Index]);
				}
				else
				{
					luaLClosureValue.Upvals[i] = encup[p.Upvalues[i].Index];
				}
			}
		}

		private void V_ObjLen(StkId ra, StkId rb)
		{
			LuaTable luaTable = rb.V.HValue();
			StkId stkId;
			if (luaTable != null)
			{
				stkId = this.FastTM(luaTable.MetaTable, TMS.TM_LEN);
				if (stkId == null)
				{
					ra.V.SetNValue((double)luaTable.Length);
					return;
				}
			}
			else
			{
				string text = rb.V.SValue();
				if (text != null)
				{
					ra.V.SetNValue((double)text.Length);
					return;
				}
				stkId = this.T_GetTMByObj(ref rb.V, TMS.TM_LEN);
				if (stkId.V.TtIsNil())
				{
					this.G_TypeError(rb, "get length of");
				}
			}
			this.CallTM(ref stkId.V, ref rb.V, ref rb.V, ra, true);
		}

		private void V_Concat(int total)
		{
			Utl.Assert(total >= 2);
			do
			{
				StkId top = this.Top;
				int i = 2;
				StkId stkId = this.Stack[top.Index - 2];
				StkId stkId2 = this.Stack[top.Index - 1];
				if ((!stkId.V.TtIsString() && !stkId.V.TtIsNumber()) || !this.ToString(ref stkId2.V))
				{
					if (!this.CallBinTM(stkId, stkId2, stkId, TMS.TM_CONCAT))
					{
						this.G_ConcatError(stkId, stkId2);
					}
				}
				else if (stkId2.V.SValue().Length == 0)
				{
					this.ToString(ref stkId.V);
				}
				else if (stkId.V.TtIsString() && stkId.V.SValue().Length == 0)
				{
					stkId.V.SetObj(ref stkId2.V);
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (i = 0; i < total; i++)
					{
						StkId stkId3 = this.Stack[top.Index - (i + 1)];
						if (stkId3.V.TtIsString())
						{
							stringBuilder.Insert(0, stkId3.V.SValue());
						}
						else
						{
							if (!stkId3.V.TtIsNumber())
							{
								break;
							}
							stringBuilder.Insert(0, stkId3.V.NValue.ToString());
						}
					}
					StkId stkId4 = this.Stack[top.Index - i];
					stkId4.V.SetSValue(stringBuilder.ToString());
				}
				total -= i - 1;
				this.Top = this.Stack[this.Top.Index - (i - 1)];
			}
			while (total > 1);
		}

		private void V_DoJump(CallInfo ci, Instruction i, int e)
		{
			int arg_A = i.GETARG_A();
			if (arg_A > 0)
			{
				this.F_Close(this.Stack[ci.BaseIndex + (arg_A - 1)]);
			}
			ci.SavedPc += i.GETARG_sBx() + e;
		}

		private void V_DoNextJump(CallInfo ci)
		{
			Instruction value = ci.SavedPc.Value;
			this.V_DoJump(ci, value, 1);
		}

		private bool V_ToNumber(StkId obj, ref TValue n)
		{
			if (obj.V.TtIsNumber())
			{
				n.SetNValue(obj.V.NValue);
				return true;
			}
			double nvalue;
			if (obj.V.TtIsString() && LuaState.O_Str2Decimal(obj.V.SValue(), out nvalue))
			{
				n.SetNValue(nvalue);
				return true;
			}
			return false;
		}

		private bool V_ToString(ref TValue v)
		{
			if (!v.TtIsNumber())
			{
				return false;
			}
			v.SetSValue(v.NValue.ToString());
			return true;
		}

		private LuaOp TMS2OP(TMS op)
		{
			switch (op)
			{
			case TMS.TM_ADD:
				return LuaOp.LUA_OPADD;
			case TMS.TM_SUB:
				return LuaOp.LUA_OPSUB;
			case TMS.TM_MUL:
				return LuaOp.LUA_OPMUL;
			case TMS.TM_DIV:
				return LuaOp.LUA_OPDIV;
			case TMS.TM_POW:
				return LuaOp.LUA_OPPOW;
			case TMS.TM_UNM:
				return LuaOp.LUA_OPUNM;
			}
			throw new NotImplementedException();
		}

		private void CallTM(ref TValue f, ref TValue p1, ref TValue p2, StkId p3, bool hasres)
		{
			int index = p3.Index;
			StkId top = this.Top;
			StkId.inc(ref this.Top).V.SetObj(ref f);
			StkId.inc(ref this.Top).V.SetObj(ref p1);
			StkId.inc(ref this.Top).V.SetObj(ref p2);
			if (!hasres)
			{
				StkId.inc(ref this.Top).V.SetObj(ref p3.V);
			}
			this.D_CheckStack(0);
			this.D_Call(top, (!hasres) ? 0 : 1, this.CI.IsLua);
			if (hasres)
			{
				this.Top = this.Stack[this.Top.Index - 1];
				this.Stack[index].V.SetObj(ref this.Top.V);
			}
		}

		private bool CallBinTM(StkId p1, StkId p2, StkId res, TMS tm)
		{
			StkId stkId = this.T_GetTMByObj(ref p1.V, tm);
			if (stkId.V.TtIsNil())
			{
				stkId = this.T_GetTMByObj(ref p2.V, tm);
			}
			if (stkId.V.TtIsNil())
			{
				return false;
			}
			this.CallTM(ref stkId.V, ref p1.V, ref p2.V, res, true);
			return true;
		}

		private void V_Arith(StkId ra, StkId rb, StkId rc, TMS op)
		{
			TValue tvalue = default(TValue);
			TValue tvalue2 = default(TValue);
			if (this.V_ToNumber(rb, ref tvalue) && this.V_ToNumber(rc, ref tvalue2))
			{
				double nvalue = LuaState.O_Arith(this.TMS2OP(op), tvalue.NValue, tvalue2.NValue);
				ra.V.SetNValue(nvalue);
			}
			else if (!this.CallBinTM(rb, rc, ra, op))
			{
				this.G_ArithError(rb, rc);
			}
		}

		private bool CallOrderTM(StkId p1, StkId p2, TMS tm, out bool error)
		{
			if (!this.CallBinTM(p1, p2, this.Top, tm))
			{
				error = true;
				return false;
			}
			error = false;
			return !this.IsFalse(ref this.Top.V);
		}

		private bool V_LessThan(StkId lhs, StkId rhs)
		{
			if (lhs.V.TtIsNumber() && rhs.V.TtIsNumber())
			{
				return lhs.V.NValue < rhs.V.NValue;
			}
			if (lhs.V.TtIsString() && rhs.V.TtIsString())
			{
				return string.Compare(lhs.V.SValue(), rhs.V.SValue()) < 0;
			}
			bool flag;
			bool result = this.CallOrderTM(lhs, rhs, TMS.TM_LT, out flag);
			if (flag)
			{
				this.G_OrderError(lhs, rhs);
				return false;
			}
			return result;
		}

		private bool V_LessEqual(StkId lhs, StkId rhs)
		{
			if (lhs.V.TtIsNumber() && rhs.V.TtIsNumber())
			{
				return lhs.V.NValue <= rhs.V.NValue;
			}
			if (lhs.V.TtIsString() && rhs.V.TtIsString())
			{
				return string.Compare(lhs.V.SValue(), rhs.V.SValue()) <= 0;
			}
			bool flag;
			bool result = this.CallOrderTM(lhs, rhs, TMS.TM_LE, out flag);
			if (!flag)
			{
				return result;
			}
			result = this.CallOrderTM(rhs, lhs, TMS.TM_LT, out flag);
			if (!flag)
			{
				return result;
			}
			this.G_OrderError(lhs, rhs);
			return false;
		}

		private void V_FinishOp()
		{
			int index = this.CI.Index;
			int baseIndex = this.CI.BaseIndex;
			Instruction value = (this.CI.SavedPc - 1).Value;
			OpCode opcode = value.GET_OPCODE();
			switch (opcode)
			{
			case OpCode.OP_GETTABUP:
			case OpCode.OP_GETTABLE:
			case OpCode.OP_SELF:
			case OpCode.OP_ADD:
			case OpCode.OP_SUB:
			case OpCode.OP_MUL:
			case OpCode.OP_DIV:
			case OpCode.OP_MOD:
			case OpCode.OP_POW:
			case OpCode.OP_UNM:
			case OpCode.OP_LEN:
			{
				StkId stkId = this.Stack[baseIndex + value.GETARG_A()];
				this.Top = this.Stack[this.Top.Index - 1];
				stkId.V.SetObj(ref this.Stack[this.Top.Index].V);
				return;
			}
			case OpCode.OP_SETTABUP:
			case OpCode.OP_SETTABLE:
			case OpCode.OP_TAILCALL:
				return;
			case OpCode.OP_CONCAT:
			{
				StkId stkId2 = this.Stack[this.Top.Index - 1];
				int arg_B = value.GETARG_B();
				int num = stkId2.Index - 1 - (baseIndex + arg_B);
				StkId stkId3 = this.Stack[stkId2.Index - 2];
				stkId3.V.SetObj(ref stkId2.V);
				if (num > 1)
				{
					this.Top = this.Stack[this.Top.Index - 1];
					this.V_Concat(num);
				}
				CallInfo callInfo = this.BaseCI[index];
				StkId stkId4 = this.Stack[callInfo.BaseIndex + value.GETARG_A()];
				stkId4.V.SetObj(ref this.Stack[this.Top.Index - 1].V);
				this.Top = this.Stack[callInfo.TopIndex];
				return;
			}
			case OpCode.OP_EQ:
			case OpCode.OP_LT:
			case OpCode.OP_LE:
			{
				bool flag = !this.IsFalse(ref this.Stack[this.Top.Index - 1].V);
				this.Top = this.Stack[this.Top.Index - 1];
				Utl.Assert(!Instruction.ISK(value.GETARG_B()));
				if (opcode == OpCode.OP_LE && this.T_GetTMByObj(ref this.Stack[baseIndex + value.GETARG_B()].V, TMS.TM_LE).V.TtIsNil())
				{
					flag = !flag;
				}
				CallInfo callInfo2 = this.BaseCI[index];
				Utl.Assert(callInfo2.SavedPc.Value.GET_OPCODE() == OpCode.OP_JMP);
				if (((!flag) ? 0 : 1) != value.GETARG_A() && value.GETARG_A() == 0 == flag)
				{
					CallInfo callInfo3 = callInfo2;
					callInfo3.SavedPc.Index = callInfo3.SavedPc.Index + 1;
				}
				return;
			}
			case OpCode.OP_CALL:
				if (value.GETARG_C() - 1 >= 0)
				{
					CallInfo callInfo4 = this.BaseCI[index];
					this.Top = this.Stack[callInfo4.TopIndex];
				}
				return;
			case OpCode.OP_TFORCALL:
			{
				CallInfo callInfo5 = this.BaseCI[index];
				Utl.Assert(callInfo5.SavedPc.Value.GET_OPCODE() == OpCode.OP_TFORLOOP);
				this.Top = this.Stack[callInfo5.TopIndex];
				return;
			}
			}
			Utl.Assert(false);
		}

		internal bool V_RawEqualObj(ref TValue t1, ref TValue t2)
		{
			return t1.Tt == t2.Tt && this.V_EqualObject(ref t1, ref t2, true);
		}

		private bool EqualObj(ref TValue t1, ref TValue t2, bool rawEq)
		{
			return t1.Tt == t2.Tt && this.V_EqualObject(ref t1, ref t2, rawEq);
		}

		private StkId GetEqualTM(LuaTable mt1, LuaTable mt2, TMS tm)
		{
			StkId stkId = this.FastTM(mt1, tm);
			if (stkId == null)
			{
				return null;
			}
			if (mt1 == mt2)
			{
				return stkId;
			}
			StkId stkId2 = this.FastTM(mt2, tm);
			if (stkId2 == null)
			{
				return null;
			}
			if (this.V_RawEqualObj(ref stkId.V, ref stkId2.V))
			{
				return stkId;
			}
			return null;
		}

		private bool V_EqualObject(ref TValue t1, ref TValue t2, bool rawEq)
		{
			Utl.Assert(t1.Tt == t2.Tt);
			StkId equalTM;
			switch (t1.Tt)
			{
			case 0:
				return true;
			case 1:
				return t1.BValue() == t2.BValue();
			case 3:
				return t1.NValue == t2.NValue;
			case 4:
				return t1.SValue() == t2.SValue();
			case 5:
			{
				LuaTable luaTable = t1.HValue();
				LuaTable luaTable2 = t2.HValue();
				if (object.ReferenceEquals(luaTable, luaTable2))
				{
					return true;
				}
				if (rawEq)
				{
					return false;
				}
				equalTM = this.GetEqualTM(luaTable.MetaTable, luaTable2.MetaTable, TMS.TM_EQ);
				goto IL_123;
			}
			case 7:
			{
				LuaUserDataValue luaUserDataValue = t1.RawUValue();
				LuaUserDataValue luaUserDataValue2 = t2.RawUValue();
				if (luaUserDataValue.Value == luaUserDataValue2.Value)
				{
					return true;
				}
				if (rawEq)
				{
					return false;
				}
				equalTM = this.GetEqualTM(luaUserDataValue.MetaTable, luaUserDataValue2.MetaTable, TMS.TM_EQ);
				goto IL_123;
			}
			case 9:
				return t1.UInt64Value == t2.UInt64Value;
			}
			return t1.OValue == t2.OValue;
			IL_123:
			if (equalTM == null)
			{
				return false;
			}
			this.CallTM(ref equalTM.V, ref t1, ref t2, this.Top, true);
			return !this.IsFalse(ref this.Top.V);
		}

		private const int ERRORSTACKSIZE = 1000200;

		private const int LEVELS1 = 12;

		private const int LEVELS2 = 10;

		private const int FreeList = 0;

		private const int MAXTAGLOOP = 100;

		private static PFuncDelegate<LuaState.LoadParameter> DG_F_Load = new PFuncDelegate<LuaState.LoadParameter>(LuaState.F_Load);

		private static PFuncDelegate<LuaState.CallS> DG_F_Call = new PFuncDelegate<LuaState.CallS>(LuaState.F_Call);

		private static PFuncDelegate<LuaState.UnrollParam> DG_Unroll = new PFuncDelegate<LuaState.UnrollParam>(LuaState.UnrollWrap);

		private static PFuncDelegate<LuaState.ResumeParam> DG_Resume = new PFuncDelegate<LuaState.ResumeParam>(LuaState.ResumeWrap);

		private static PFuncDelegate<LuaState.GrowStackParam> DG_GrowStack = new PFuncDelegate<LuaState.GrowStackParam>(LuaState.GrowStackWrap);

		internal static StkId TheNilValue = new StkId();

		public StkId[] Stack;

		public StkId Top;

		public int StackSize;

		public int StackLast;

		public CallInfo CI;

		public CallInfo[] BaseCI;

		public GlobalState G;

		public int NumNonYieldable;

		public int NumCSharpCalls;

		public int ErrFunc;

		public bool AllowHook;

		public byte HookMask;

		public int BaseHookCount;

		public int HookCount;

		public LuaHookDelegate Hook;

		public LinkedList<LuaUpvalue> OpenUpval;

		private ILuaAPI API;

		private struct LoadParameter
		{
			public LoadParameter(LuaState l, ILoadInfo loadinfo, string name, string mode)
			{
				this.L = l;
				this.LoadInfo = loadinfo;
				this.Name = name;
				this.Mode = mode;
			}

			public LuaState L;

			public ILoadInfo LoadInfo;

			public string Name;

			public string Mode;
		}

		private struct CallS
		{
			public LuaState L;

			public int FuncIndex;

			public int NumResults;
		}

		private struct UnrollParam
		{
			public LuaState L;
		}

		private struct ResumeParam
		{
			public LuaState L;

			public int firstArg;
		}

		private struct GrowStackParam
		{
			public LuaState L;

			public int size;
		}

		private struct ExecuteEnvironment
		{
			public StkId RA
			{
				get
				{
					return this.Stack[this.Base + this.I.GETARG_A()];
				}
			}

			public StkId RB
			{
				get
				{
					return this.Stack[this.Base + this.I.GETARG_B()];
				}
			}

			public StkId RK(int x)
			{
				return (!Instruction.ISK(x)) ? this.Stack[this.Base + x] : this.K[Instruction.INDEXK(x)];
			}

			public StkId RKB
			{
				get
				{
					return this.RK(this.I.GETARG_B());
				}
			}

			public StkId RKC
			{
				get
				{
					return this.RK(this.I.GETARG_C());
				}
			}

			public StkId[] Stack;

			public List<StkId> K;

			public int Base;

			public Instruction I;
		}
	}
}
