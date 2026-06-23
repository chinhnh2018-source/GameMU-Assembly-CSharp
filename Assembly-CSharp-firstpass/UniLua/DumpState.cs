using System;
using System.Collections.Generic;

namespace UniLua
{
	internal class DumpState
	{
		private DumpState()
		{
		}

		public static DumpStatus Dump(LuaProto proto, LuaWriter writer, bool strip)
		{
			DumpState dumpState = new DumpState();
			dumpState.Writer = writer;
			dumpState.Strip = strip;
			dumpState.Status = DumpStatus.OK;
			dumpState.DumpHeader();
			dumpState.DumpFunction(proto);
			return dumpState.Status;
		}

		private byte[] BuildHeader()
		{
			byte[] array = new byte[DumpState.LUAC_HEADERSIZE];
			int num = 0;
			for (int i = 0; i < "\u001bLua".Length; i++)
			{
				array[num++] = (byte)"\u001bLua".get_Chars(i);
			}
			array[num++] = (byte)DumpState.VERSION;
			array[num++] = 0;
			array[num++] = 1;
			array[num++] = 4;
			array[num++] = 4;
			array[num++] = 4;
			array[num++] = 8;
			array[num++] = 0;
			for (int j = 0; j < "\u0019\u0093\r\n\u001a\n".Length; j++)
			{
				array[num++] = (byte)"\u0019\u0093\r\n\u001a\n".get_Chars(j);
			}
			return array;
		}

		private void DumpHeader()
		{
			byte[] bytes = this.BuildHeader();
			this.DumpBlock(bytes);
		}

		private void DumpBool(bool value)
		{
			this.DumpByte((!value) ? 0 : 1);
		}

		private void DumpInt(int value)
		{
			this.DumpBlock(BitConverter.GetBytes(value));
		}

		private void DumpUInt(uint value)
		{
			this.DumpBlock(BitConverter.GetBytes(value));
		}

		private void DumpString(string value)
		{
			if (value == null)
			{
				this.DumpUInt(0U);
			}
			else
			{
				this.DumpUInt((uint)(value.Length + 1));
				for (int i = 0; i < value.Length; i++)
				{
					this.DumpByte((byte)value.get_Chars(i));
				}
				this.DumpByte(0);
			}
		}

		private void DumpByte(byte value)
		{
			byte[] bytes = new byte[]
			{
				value
			};
			this.DumpBlock(bytes);
		}

		private void DumpCode(LuaProto proto)
		{
			this.DumpVector<Instruction>(proto.Code, delegate(Instruction ins)
			{
				this.DumpBlock(BitConverter.GetBytes((uint)ins));
			});
		}

		private void DumpConstants(LuaProto proto)
		{
			this.DumpVector<StkId>(proto.K, delegate(StkId k)
			{
				int tt = k.V.Tt;
				this.DumpByte((byte)tt);
				switch (tt)
				{
				case 0:
					return;
				case 1:
					this.DumpBool(k.V.BValue());
					return;
				case 3:
					this.DumpBlock(BitConverter.GetBytes(k.V.NValue));
					return;
				case 4:
					this.DumpString(k.V.SValue());
					return;
				}
				Utl.Assert(false);
			});
			this.DumpVector<LuaProto>(proto.P, delegate(LuaProto p)
			{
				this.DumpFunction(p);
			});
		}

		private void DumpUpvalues(LuaProto proto)
		{
			this.DumpVector<UpvalDesc>(proto.Upvalues, delegate(UpvalDesc upval)
			{
				this.DumpByte((!upval.InStack) ? 0 : 1);
				this.DumpByte((byte)upval.Index);
			});
		}

		private void DumpDebug(LuaProto proto)
		{
			this.DumpString((!this.Strip) ? proto.Source : null);
			this.DumpVector<int>((!this.Strip) ? proto.LineInfo : null, delegate(int line)
			{
				this.DumpInt(line);
			});
			this.DumpVector<LocVar>((!this.Strip) ? proto.LocVars : null, delegate(LocVar locvar)
			{
				this.DumpString(locvar.VarName);
				this.DumpInt(locvar.StartPc);
				this.DumpInt(locvar.EndPc);
			});
			this.DumpVector<UpvalDesc>((!this.Strip) ? proto.Upvalues : null, delegate(UpvalDesc upval)
			{
				this.DumpString(upval.Name);
			});
		}

		private void DumpFunction(LuaProto proto)
		{
			this.DumpInt(proto.LineDefined);
			this.DumpInt(proto.LastLineDefined);
			this.DumpByte((byte)proto.NumParams);
			this.DumpByte((!proto.IsVarArg) ? 0 : 1);
			this.DumpByte(proto.MaxStackSize);
			this.DumpCode(proto);
			this.DumpConstants(proto);
			this.DumpUpvalues(proto);
			this.DumpDebug(proto);
		}

		private void DumpVector<T>(IList<T> list, DumpState.DumpItemDelegate<T> dumpItem)
		{
			if (list == null)
			{
				this.DumpInt(0);
			}
			else
			{
				this.DumpInt(list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					dumpItem(list[i]);
				}
			}
		}

		private void DumpBlock(byte[] bytes)
		{
			this.DumpBlock(bytes, 0, bytes.Length);
		}

		private void DumpBlock(byte[] bytes, int start, int length)
		{
			if (this.Status == DumpStatus.OK)
			{
				this.Status = this.Writer(bytes, start, length);
			}
		}

		private const string LUAC_TAIL = "\u0019\u0093\r\n\u001a\n";

		private const int FORMAT = 0;

		private const int ENDIAN = 1;

		private LuaWriter Writer;

		private bool Strip;

		private DumpStatus Status;

		private static int VERSION = (int)(("5".get_Chars(0) - '0') * '\u0010' + ("2".get_Chars(0) - '0'));

		private static int LUAC_HEADERSIZE = "\u001bLua".Length + 2 + 6 + "\u0019\u0093\r\n\u001a\n".Length;

		private delegate void DumpItemDelegate<T>(T item);
	}
}
