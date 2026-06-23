using System;

namespace UniLua
{
	public class Undump
	{
		private Undump(BinaryBytesReader reader)
		{
			this.Reader = reader;
		}

		public static LuaProto LoadBinary(ILuaState lua, ILoadInfo loadinfo, string name)
		{
			LuaProto result;
			try
			{
				BinaryBytesReader reader = new BinaryBytesReader(loadinfo);
				Undump undump = new Undump(reader);
				undump.LoadHeader();
				result = undump.LoadFunction();
			}
			catch (UndumpException ex)
			{
				LuaState luaState = (LuaState)lua;
				luaState.O_PushString(string.Format("{0}: {1} precompiled chunk", name, ex.Why));
				luaState.D_Throw(ThreadStatus.LUA_ERRSYNTAX);
				result = null;
			}
			return result;
		}

		private int LoadInt()
		{
			return this.Reader.ReadInt();
		}

		private byte LoadByte()
		{
			return this.Reader.ReadByte();
		}

		private byte[] LoadBytes(int count)
		{
			return this.Reader.ReadBytes(count);
		}

		private string LoadString()
		{
			return this.Reader.ReadString();
		}

		private bool LoadBoolean()
		{
			return this.LoadByte() != 0;
		}

		private double LoadNumber()
		{
			return this.Reader.ReadDouble();
		}

		private void LoadHeader()
		{
			byte[] array = this.LoadBytes(18);
			byte sizeOfSizeT = array[8];
			this.Reader.SizeOfSizeT = (int)sizeOfSizeT;
		}

		private Instruction LoadInstruction()
		{
			return (Instruction)this.Reader.ReadUInt();
		}

		private LuaProto LoadFunction()
		{
			LuaProto luaProto = new LuaProto();
			luaProto.LineDefined = this.LoadInt();
			luaProto.LastLineDefined = this.LoadInt();
			luaProto.NumParams = (int)this.LoadByte();
			luaProto.IsVarArg = this.LoadBoolean();
			luaProto.MaxStackSize = this.LoadByte();
			this.LoadCode(luaProto);
			this.LoadConstants(luaProto);
			this.LoadUpvalues(luaProto);
			this.LoadDebug(luaProto);
			return luaProto;
		}

		private void LoadCode(LuaProto proto)
		{
			int num = this.LoadInt();
			proto.Code.Clear();
			for (int i = 0; i < num; i++)
			{
				proto.Code.Add(this.LoadInstruction());
			}
		}

		private void LoadConstants(LuaProto proto)
		{
			int num = this.LoadInt();
			proto.K.Clear();
			int i = 0;
			while (i < num)
			{
				int num2 = (int)this.LoadByte();
				StkId stkId = new StkId();
				switch (num2)
				{
				case 0:
					stkId.V.SetNilValue();
					proto.K.Add(stkId);
					break;
				case 1:
					stkId.V.SetBValue(this.LoadBoolean());
					proto.K.Add(stkId);
					break;
				case 2:
					goto IL_CB;
				case 3:
					stkId.V.SetNValue(this.LoadNumber());
					proto.K.Add(stkId);
					break;
				case 4:
					stkId.V.SetSValue(this.LoadString());
					proto.K.Add(stkId);
					break;
				default:
					goto IL_CB;
				}
				i++;
				continue;
				IL_CB:
				throw new UndumpException("LoadConstants unknown type: " + num2);
			}
			num = this.LoadInt();
			proto.P.Clear();
			for (int j = 0; j < num; j++)
			{
				proto.P.Add(this.LoadFunction());
			}
		}

		private void LoadUpvalues(LuaProto proto)
		{
			int num = this.LoadInt();
			proto.Upvalues.Clear();
			for (int i = 0; i < num; i++)
			{
				proto.Upvalues.Add(new UpvalDesc
				{
					Name = null,
					InStack = this.LoadBoolean(),
					Index = (int)this.LoadByte()
				});
			}
		}

		private void LoadDebug(LuaProto proto)
		{
			proto.Source = this.LoadString();
			int num = this.LoadInt();
			proto.LineInfo.Clear();
			for (int i = 0; i < num; i++)
			{
				proto.LineInfo.Add(this.LoadInt());
			}
			num = this.LoadInt();
			proto.LocVars.Clear();
			for (int j = 0; j < num; j++)
			{
				proto.LocVars.Add(new LocVar
				{
					VarName = this.LoadString(),
					StartPc = this.LoadInt(),
					EndPc = this.LoadInt()
				});
			}
			num = this.LoadInt();
			for (int k = 0; k < num; k++)
			{
				proto.Upvalues[k].Name = this.LoadString();
			}
		}

		private BinaryBytesReader Reader;
	}
}
