using System;
using System.IO;
using System.Text;

namespace UniLua.Tools
{
	public static class Compiler
	{
		private static void Fatal(string msg)
		{
			throw new Exception(msg);
		}

		public static LuaProto CompileFile(string filename)
		{
			ILuaState luaState = LuaAPI.NewState();
			ThreadStatus threadStatus = luaState.L_LoadFileX(filename, null);
			if (threadStatus != ThreadStatus.LUA_OK)
			{
				Compiler.Fatal(luaState.ToString(-1));
			}
			LuaLClosureValue luaLClosureValue = ((LuaState)luaState).Top.V.ClLValue();
			return luaLClosureValue.Proto;
		}

		public static void ListingToFile(LuaProto proto, string filename)
		{
			using (StreamWriter writer = new StreamWriter(filename))
			{
				Compiler._ListFunc(proto, delegate(string output)
				{
					writer.Write(output);
				});
			}
		}

		public static void ListingToFile(string inFilename, string outFilename)
		{
			Compiler.ListingToFile(Compiler.CompileFile(inFilename), outFilename);
		}

		public static void DumpingToFile(LuaProto proto, string filename, bool strip)
		{
			using (BinaryWriter writer = new BinaryWriter(File.Open(filename, 2)))
			{
				LuaWriter writer2 = delegate(byte[] bytes, int start, int length)
				{
					DumpStatus result;
					try
					{
						writer.Write(bytes, start, length);
						result = DumpStatus.OK;
					}
					catch (Exception)
					{
						result = DumpStatus.ERROR;
					}
					return result;
				};
				DumpState.Dump(proto, writer2, strip);
			}
		}

		public static void DumpingToFile(string inFilename, string outFilename, bool strip)
		{
			Compiler.DumpingToFile(Compiler.CompileFile(inFilename), outFilename, strip);
		}

		private static void _ListFunc(LuaProto p, Compiler.ListFuncDelegate outputEvent)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = (p.Source == null) ? "=?" : p.Source;
			if (text.get_Chars(0) == '@' || text.get_Chars(0) == '=')
			{
				text = text.Substring(1);
			}
			else if (text.get_Chars(0) == '\u001b')
			{
				text = "(bstring)";
			}
			else
			{
				text = "(string)";
			}
			stringBuilder.Append(string.Format("{0} <{1}:{2},{3}> ({4} instructions)", new object[]
			{
				(p.LineDefined != 0) ? "function" : "main",
				text,
				p.LineDefined,
				p.LastLineDefined,
				p.Code.Count
			})).Append("\n");
			stringBuilder.Append(string.Format("{0}{1} params, {2} slots, {3} upvalue, {4} locals, {5} constants, {6} functions", new object[]
			{
				p.NumParams,
				(!p.IsVarArg) ? string.Empty : "+",
				p.MaxStackSize,
				p.Upvalues.Count,
				p.LocVars.Count,
				p.K.Count,
				p.P.Count
			})).Append("\n");
			for (int i = 0; i < p.Code.Count; i++)
			{
				Instruction instruction = p.Code[i];
				int num = p.LineInfo[i];
				stringBuilder.Append((i + 1).ToString()).Append("\t").Append("[" + num + "]").Append("\t").Append(instruction.ToString()).Append("\t").Append("; ").Append(num).Append("\n");
			}
			if (outputEvent != null)
			{
				outputEvent(stringBuilder.ToString());
			}
			foreach (LuaProto p2 in p.P)
			{
				Compiler._ListFunc(p2, outputEvent);
			}
		}

		private delegate void ListFuncDelegate(string output);
	}
}
