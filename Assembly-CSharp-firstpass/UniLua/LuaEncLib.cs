using System;
using System.Text;

namespace UniLua
{
	internal class LuaEncLib
	{
		public static int OpenLib(ILuaState lua)
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
				new NameFuncPair("encode", new CSharpFunctionDelegate(LuaEncLib.ENC_Encode)),
				new NameFuncPair("decode", new CSharpFunctionDelegate(LuaEncLib.ENC_Decode))
			};
			lua.L_NewLib(define);
			lua.PushString("utf8");
			lua.SetField(-2, "utf8");
			return 1;
		}

		private static int ENC_Encode(ILuaState lua)
		{
			string text = lua.ToString(1);
			string text2 = lua.ToString(2);
			if (text2 != "utf8")
			{
				throw new Exception("unsupported encoding:" + text2);
			}
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
			{
				stringBuilder.Append((char)bytes[i]);
			}
			lua.PushString(stringBuilder.ToString());
			return 1;
		}

		private static int ENC_Decode(ILuaState lua)
		{
			string text = lua.ToString(1);
			string text2 = lua.ToString(2);
			if (text2 != "utf8")
			{
				throw new Exception("unsupported encoding:" + text2);
			}
			byte[] array = new byte[text.Length];
			for (int i = 0; i < text.Length; i++)
			{
				array[i] = (byte)text.get_Chars(i);
			}
			lua.PushString(Encoding.UTF8.GetString(array));
			return 1;
		}

		public const string LIB_NAME = "enc";

		private const string ENC_UTF8 = "utf8";
	}
}
