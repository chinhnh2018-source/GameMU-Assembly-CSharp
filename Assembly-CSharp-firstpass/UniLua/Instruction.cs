using System;

namespace UniLua
{
	public struct Instruction
	{
		public Instruction(uint val)
		{
			this.Value = val;
		}

		public override string ToString()
		{
			OpCode opcode = this.GET_OPCODE();
			int arg_A = this.GETARG_A();
			int arg_B = this.GETARG_B();
			int arg_C = this.GETARG_C();
			int arg_Ax = this.GETARG_Ax();
			int arg_Bx = this.GETARG_Bx();
			int arg_sBx = this.GETARG_sBx();
			OpCodeMode mode = OpCodeInfo.GetMode(opcode);
			switch (mode.OpMode)
			{
			case OpMode.iABC:
			{
				string text = string.Format("{0,-9} {1}", opcode, arg_A);
				if (mode.BMode != OpArgMask.OpArgN)
				{
					text = text + " " + ((!Instruction.ISK(arg_B)) ? arg_B : Instruction.MYK(Instruction.INDEXK(arg_B)));
				}
				if (mode.CMode != OpArgMask.OpArgN)
				{
					text = text + " " + ((!Instruction.ISK(arg_C)) ? arg_C : Instruction.MYK(Instruction.INDEXK(arg_C)));
				}
				return text;
			}
			case OpMode.iABx:
			{
				string text2 = string.Format("{0,-9} {1}", opcode, arg_A);
				if (mode.BMode == OpArgMask.OpArgK)
				{
					text2 = text2 + " " + Instruction.MYK(arg_Bx);
				}
				else if (mode.BMode == OpArgMask.OpArgU)
				{
					text2 = text2 + " " + arg_Bx;
				}
				return text2;
			}
			case OpMode.iAsBx:
				return string.Format("{0,-9} {1} {2}", opcode, arg_A, arg_sBx);
			case OpMode.iAx:
				return string.Format("{0,-9} {1}", opcode, Instruction.MYK(arg_Ax));
			default:
				throw new NotImplementedException();
			}
		}

		public static int RKASK(int x)
		{
			return x | 256;
		}

		public static bool ISK(int x)
		{
			return (x & 256) != 0;
		}

		public static int INDEXK(int r)
		{
			return r & -257;
		}

		public static int MYK(int x)
		{
			return -1 - x;
		}

		public static uint MASK1(int size, int pos)
		{
			return ~(uint.MaxValue << size) << pos;
		}

		public static uint MASK0(int size, int pos)
		{
			return ~Instruction.MASK1(size, pos);
		}

		public OpCode GET_OPCODE()
		{
			return (OpCode)(this.Value & Instruction.MASK1(6, 0));
		}

		public Instruction SET_OPCODE(OpCode op)
		{
			this.Value = ((this.Value & Instruction.MASK0(6, 0)) | (uint)(op & (OpCode)Instruction.MASK1(6, 0)));
			return this;
		}

		public int GETARG(int pos, int size)
		{
			return (int)(this.Value >> pos & Instruction.MASK1(size, 0));
		}

		public Instruction SETARG(int value, int pos, int size)
		{
			this.Value = ((this.Value & Instruction.MASK0(size, pos)) | (uint)(value << pos & (int)Instruction.MASK1(size, pos)));
			return this;
		}

		public int GETARG_A()
		{
			return this.GETARG(6, 8);
		}

		public Instruction SETARG_A(int value)
		{
			return this.SETARG(value, 6, 8);
		}

		public int GETARG_B()
		{
			return this.GETARG(23, 9);
		}

		public Instruction SETARG_B(int value)
		{
			return this.SETARG(value, 23, 9);
		}

		public int GETARG_C()
		{
			return this.GETARG(14, 9);
		}

		public Instruction SETARG_C(int value)
		{
			return this.SETARG(value, 14, 9);
		}

		public int GETARG_Bx()
		{
			return this.GETARG(14, 18);
		}

		public Instruction SETARG_Bx(int value)
		{
			return this.SETARG(value, 14, 18);
		}

		public int GETARG_Ax()
		{
			return this.GETARG(6, 26);
		}

		public Instruction SETARG_Ax(int value)
		{
			return this.SETARG(value, 6, 26);
		}

		public int GETARG_sBx()
		{
			return this.GETARG_Bx() - 131071;
		}

		public Instruction SETARG_sBx(int value)
		{
			return this.SETARG_Bx(value + 131071);
		}

		public static Instruction CreateABC(OpCode op, int a, int b, int c)
		{
			return (Instruction)((uint)(op | (OpCode)(a << 6) | (OpCode)(b << 23) | (OpCode)((uint)c << 14)));
		}

		public static Instruction CreateABx(OpCode op, int a, uint bc)
		{
			return (Instruction)((uint)(op | (OpCode)((uint)a << 6) | (OpCode)(bc << 14)));
		}

		public static Instruction CreateAx(OpCode op, int a)
		{
			return (Instruction)((uint)(op | (OpCode)((uint)a << 6)));
		}

		public static explicit operator Instruction(uint val)
		{
			return new Instruction(val);
		}

		public static explicit operator uint(Instruction i)
		{
			return i.Value;
		}

		public const int SIZE_C = 9;

		public const int SIZE_B = 9;

		public const int SIZE_Bx = 18;

		public const int SIZE_A = 8;

		public const int SIZE_Ax = 26;

		public const int SIZE_OP = 6;

		public const int POS_OP = 0;

		public const int POS_A = 6;

		public const int POS_C = 14;

		public const int POS_B = 23;

		public const int POS_Bx = 14;

		public const int POS_Ax = 6;

		public const int MAXARG_Bx = 262143;

		public const int MAXARG_sBx = 131071;

		public const int MAXARG_Ax = 67108863;

		public const int MAXARG_A = 255;

		public const int MAXARG_B = 511;

		public const int MAXARG_C = 511;

		public const int BITRK = 256;

		public const int MAXINDEXRK = 255;

		public uint Value;
	}
}
