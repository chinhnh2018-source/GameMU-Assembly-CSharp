using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UniLua.Tools;

namespace UniLua
{
	internal class LuaFFILib
	{
		public static int OpenLib(ILuaState lua)
		{
			NameFuncPair[] define = new NameFuncPair[]
			{
				new NameFuncPair("clear_assembly_list", new CSharpFunctionDelegate(LuaFFILib.FFI_ClearAssemblyList)),
				new NameFuncPair("add_assembly", new CSharpFunctionDelegate(LuaFFILib.FFI_AddAssembly)),
				new NameFuncPair("clear_using_list", new CSharpFunctionDelegate(LuaFFILib.FFI_ClearUsingList)),
				new NameFuncPair("using", new CSharpFunctionDelegate(LuaFFILib.FFI_Using)),
				new NameFuncPair("parse_signature", new CSharpFunctionDelegate(LuaFFILib.FFI_ParseSignature)),
				new NameFuncPair("get_type", new CSharpFunctionDelegate(LuaFFILib.FFI_GetType)),
				new NameFuncPair("get_constructor", new CSharpFunctionDelegate(LuaFFILib.FFI_GetConstructor)),
				new NameFuncPair("get_static_method", new CSharpFunctionDelegate(LuaFFILib.FFI_GetStaticMethod)),
				new NameFuncPair("get_method", new CSharpFunctionDelegate(LuaFFILib.FFI_GetMethod)),
				new NameFuncPair("call_method", new CSharpFunctionDelegate(LuaFFILib.FFI_CallMethod)),
				new NameFuncPair("get_field", new CSharpFunctionDelegate(LuaFFILib.FFI_GetField)),
				new NameFuncPair("get_field_value", new CSharpFunctionDelegate(LuaFFILib.FFI_GetFieldValue)),
				new NameFuncPair("set_field_value", new CSharpFunctionDelegate(LuaFFILib.FFI_SetFieldValue)),
				new NameFuncPair("get_prop", new CSharpFunctionDelegate(LuaFFILib.FFI_GetProp)),
				new NameFuncPair("get_static_prop", new CSharpFunctionDelegate(LuaFFILib.FFI_GetStaticProp)),
				new NameFuncPair("get_prop_value", new CSharpFunctionDelegate(LuaFFILib.FFI_GetPropValue)),
				new NameFuncPair("set_prop_value", new CSharpFunctionDelegate(LuaFFILib.FFI_SetPropValue))
			};
			lua.L_NewLib(define);
			return 1;
		}

		private static int FFI_ClearAssemblyList(ILuaState lua)
		{
			LuaFFILib.AssemblyList.Clear();
			return 0;
		}

		private static int FFI_AddAssembly(ILuaState lua)
		{
			string text = lua.ToString(1);
			Assembly assembly = Assembly.Load(text);
			if (assembly != null)
			{
				LuaFFILib.AssemblyList.Add(assembly);
			}
			else
			{
				ULDebug.LogError.Invoke("assembly not found:" + text);
			}
			return 0;
		}

		private static int FFI_ClearUsingList(ILuaState lua)
		{
			LuaFFILib.UsingList.Clear();
			return 0;
		}

		private static int FFI_Using(ILuaState lua)
		{
			string text = lua.ToString(1);
			LuaFFILib.UsingList.Add(text);
			return 0;
		}

		private static int FFI_ParseSignature(ILuaState lua)
		{
			string signature = lua.ToString(1);
			LuaFFILib.FuncSignature funcSignature = LuaFFILib.FuncSignatureParser.Parse(lua, signature);
			if (funcSignature.ReturnType != null)
			{
				lua.PushString(funcSignature.ReturnType);
			}
			else
			{
				lua.PushNil();
			}
			lua.PushString(funcSignature.FuncName);
			if (funcSignature.ParameterTypes != null)
			{
				lua.NewTable();
				for (int i = 0; i < funcSignature.ParameterTypes.Length; i++)
				{
					lua.PushString(funcSignature.ParameterTypes[i]);
					lua.RawSetI(-2, i + 1);
				}
			}
			else
			{
				lua.PushNil();
			}
			return 3;
		}

		private static int FFI_GetType(ILuaState lua)
		{
			string typename = lua.ToString(1);
			Type type = LuaFFILib.GetType(typename);
			if (type != null)
			{
				lua.PushLightUserData(type);
			}
			else
			{
				lua.PushNil();
			}
			return 1;
		}

		private static int FFI_GetConstructor(ILuaState lua)
		{
			Type type = (Type)lua.ToUserData(1);
			int num = lua.RawLen(2);
			Type[] array = new Type[num];
			for (int i = 0; i < num; i++)
			{
				lua.RawGetI(2, i + 1);
				array[i] = (Type)lua.ToUserData(-1);
				lua.Pop(1);
			}
			ConstructorInfo constructor = type.GetConstructor(array);
			LuaFFILib.FFIConstructorInfo o = new LuaFFILib.FFIConstructorInfo(constructor);
			lua.PushLightUserData(o);
			return 1;
		}

		private static int GetMethodAux(ILuaState lua, BindingFlags flags)
		{
			Type type = (Type)lua.ToUserData(1);
			string text = lua.ToString(2);
			int num = lua.RawLen(3);
			Type[] array = new Type[num];
			for (int i = 0; i < num; i++)
			{
				lua.RawGetI(3, i + 1);
				array[i] = (Type)lua.ToUserData(-1);
				lua.Pop(1);
			}
			MethodInfo method = type.GetMethod(text, flags, null, 3, array, null);
			if (method == null)
			{
				return 0;
			}
			LuaFFILib.FFIMethodInfo o = new LuaFFILib.FFIMethodInfo(method);
			lua.PushLightUserData(o);
			return 1;
		}

		private static int FFI_GetMethod(ILuaState lua)
		{
			return LuaFFILib.GetMethodAux(lua, 276);
		}

		private static int FFI_GetStaticMethod(ILuaState lua)
		{
			return LuaFFILib.GetMethodAux(lua, 280);
		}

		private static int FFI_CallMethod(ILuaState lua)
		{
			LuaFFILib.FFIMethodBase ffimethodBase = (LuaFFILib.FFIMethodBase)lua.ToUserData(1);
			if (ffimethodBase != null)
			{
				try
				{
					return ffimethodBase.Call(lua);
				}
				catch (Exception ex)
				{
					lua.PushString(string.Concat(new string[]
					{
						"call_method Exception: ",
						ex.Message,
						"\nSource:\n",
						ex.Source,
						"\nStaceTrace:\n",
						ex.StackTrace
					}));
					lua.Error();
					return 0;
				}
			}
			lua.PushString("call_method cannot find MethodInfo");
			lua.Error();
			return 0;
		}

		private static int FFI_GetField(ILuaState lua)
		{
			Type type = (Type)lua.ToUserData(1);
			string text = lua.ToString(2);
			FieldInfo field = type.GetField(text, 20);
			if (field == null)
			{
				throw new Exception("GetField failed:" + text);
			}
			lua.PushLightUserData(field);
			return 1;
		}

		private static int FFI_GetFieldValue(ILuaState lua)
		{
			FieldInfo fieldInfo = (FieldInfo)lua.ToUserData(1);
			object obj = lua.ToUserData(2);
			Type t = (Type)lua.ToUserData(3);
			object value = fieldInfo.GetValue(obj);
			LuaFFILib.LuaStackUtil.PushRawValue(lua, value, t);
			return 1;
		}

		private static int FFI_SetFieldValue(ILuaState lua)
		{
			FieldInfo fieldInfo = (FieldInfo)lua.ToUserData(1);
			object obj = lua.ToUserData(2);
			Type t = (Type)lua.ToUserData(4);
			object obj2 = LuaFFILib.LuaStackUtil.ToRawValue(lua, 3, t);
			fieldInfo.SetValue(obj, obj2);
			return 0;
		}

		private static int FFI_GetProp(ILuaState lua)
		{
			Type type = (Type)lua.ToUserData(1);
			string text = lua.ToString(2);
			PropertyInfo property = type.GetProperty(text, 20);
			if (property == null)
			{
				throw new Exception("GetProperty failed:" + text);
			}
			lua.PushLightUserData(property);
			return 1;
		}

		private static int FFI_GetStaticProp(ILuaState lua)
		{
			Type type = (Type)lua.ToUserData(1);
			string text = lua.ToString(2);
			PropertyInfo property = type.GetProperty(text, 24);
			if (property == null)
			{
				throw new Exception("GetProperty failed:" + text);
			}
			lua.PushLightUserData(property);
			return 1;
		}

		private static int FFI_GetPropValue(ILuaState lua)
		{
			PropertyInfo propertyInfo = (PropertyInfo)lua.ToUserData(1);
			object obj = lua.ToUserData(2);
			Type t = (Type)lua.ToUserData(3);
			object value = propertyInfo.GetValue(obj, null);
			LuaFFILib.LuaStackUtil.PushRawValue(lua, value, t);
			return 1;
		}

		private static int FFI_SetPropValue(ILuaState lua)
		{
			PropertyInfo propertyInfo = (PropertyInfo)lua.ToUserData(1);
			object obj = lua.ToUserData(2);
			Type t = (Type)lua.ToUserData(4);
			object obj2 = LuaFFILib.LuaStackUtil.ToRawValue(lua, 3, t);
			propertyInfo.SetValue(obj, obj2, null);
			return 0;
		}

		private static Type FindTypeInAllAssemblies(string typename)
		{
			Type type = null;
			for (int i = 0; i < LuaFFILib.AssemblyList.Count; i++)
			{
				Type type2 = LuaFFILib.AssemblyList[i].GetType(typename);
				if (type2 != null && type == null)
				{
					type = type2;
				}
			}
			return type;
		}

		private static Type GetType(string typename)
		{
			Type type = LuaFFILib.FindTypeInAllAssemblies(typename);
			if (type != null)
			{
				return type;
			}
			for (int i = 0; i < LuaFFILib.UsingList.Count; i++)
			{
				string typename2 = LuaFFILib.UsingList[i] + "." + typename;
				type = LuaFFILib.FindTypeInAllAssemblies(typename2);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		public const string LIB_NAME = "ffi.cs";

		private static List<Assembly> AssemblyList = new List<Assembly>();

		private static List<string> UsingList = new List<string>();

		private static class LuaStackUtil
		{
			public static int PushRawValue(ILuaState lua, object o, Type t)
			{
				string fullName = t.FullName;
				if (fullName != null)
				{
					if (LuaFFILib.LuaStackUtil.<>f__switch$mapA == null)
					{
						Dictionary<string, int> dictionary = new Dictionary<string, int>(15);
						dictionary.Add("System.Boolean", 0);
						dictionary.Add("System.Char", 1);
						dictionary.Add("System.Byte", 2);
						dictionary.Add("System.SByte", 3);
						dictionary.Add("System.Int16", 4);
						dictionary.Add("System.UInt16", 5);
						dictionary.Add("System.Int32", 6);
						dictionary.Add("System.UInt32", 7);
						dictionary.Add("System.Int64", 8);
						dictionary.Add("System.UInt64", 9);
						dictionary.Add("System.Single", 10);
						dictionary.Add("System.Double", 11);
						dictionary.Add("System.Decimal", 12);
						dictionary.Add("System.String", 13);
						dictionary.Add("System.Object", 14);
						LuaFFILib.LuaStackUtil.<>f__switch$mapA = dictionary;
					}
					int num;
					if (LuaFFILib.LuaStackUtil.<>f__switch$mapA.TryGetValue(fullName, ref num))
					{
						switch (num)
						{
						case 0:
							lua.PushBoolean((bool)o);
							return 1;
						case 1:
							lua.PushString(((char)o).ToString());
							return 1;
						case 2:
							lua.PushNumber((double)((byte)o));
							return 1;
						case 3:
							lua.PushNumber((double)((sbyte)o));
							return 1;
						case 4:
							lua.PushNumber((double)((short)o));
							return 1;
						case 5:
							lua.PushNumber((double)((ushort)o));
							return 1;
						case 6:
							lua.PushNumber((double)((int)o));
							return 1;
						case 7:
							lua.PushNumber((uint)o);
							return 1;
						case 8:
							throw new NotImplementedException();
						case 9:
							lua.PushUInt64((ulong)o);
							return 1;
						case 10:
							lua.PushNumber((double)((float)o));
							return 1;
						case 11:
							lua.PushNumber((double)o);
							return 1;
						case 12:
							lua.PushLightUserData((decimal)o);
							return 1;
						case 13:
							lua.PushString(o as string);
							return 1;
						case 14:
							lua.PushLightUserData(o);
							return 1;
						}
					}
				}
				lua.PushLightUserData(o);
				return 1;
			}

			public static object ToRawValue(ILuaState lua, int index, Type t)
			{
				string fullName = t.FullName;
				if (fullName != null)
				{
					if (LuaFFILib.LuaStackUtil.<>f__switch$mapB == null)
					{
						Dictionary<string, int> dictionary = new Dictionary<string, int>(15);
						dictionary.Add("System.Boolean", 0);
						dictionary.Add("System.Char", 1);
						dictionary.Add("System.Byte", 2);
						dictionary.Add("System.SByte", 3);
						dictionary.Add("System.Int16", 4);
						dictionary.Add("System.UInt16", 5);
						dictionary.Add("System.Int32", 6);
						dictionary.Add("System.UInt32", 7);
						dictionary.Add("System.Int64", 8);
						dictionary.Add("System.UInt64", 9);
						dictionary.Add("System.Single", 10);
						dictionary.Add("System.Double", 11);
						dictionary.Add("System.Decimal", 12);
						dictionary.Add("System.String", 13);
						dictionary.Add("System.Object", 14);
						LuaFFILib.LuaStackUtil.<>f__switch$mapB = dictionary;
					}
					int num;
					if (LuaFFILib.LuaStackUtil.<>f__switch$mapB.TryGetValue(fullName, ref num))
					{
						switch (num)
						{
						case 0:
							return lua.ToBoolean(index);
						case 1:
						{
							string text = lua.ToString(index);
							if (string.IsNullOrEmpty(text))
							{
								return null;
							}
							return text.get_Chars(0);
						}
						case 2:
							return (byte)lua.ToNumber(index);
						case 3:
							return (sbyte)lua.ToNumber(index);
						case 4:
							return (short)lua.ToNumber(index);
						case 5:
							return (ushort)lua.ToNumber(index);
						case 6:
							return (int)lua.ToNumber(index);
						case 7:
							return (uint)lua.ToNumber(index);
						case 8:
							return (long)lua.ToUserData(index);
						case 9:
							return (ulong)lua.ToUserData(index);
						case 10:
							return (float)lua.ToNumber(index);
						case 11:
							return lua.ToNumber(index);
						case 12:
							return (decimal)lua.ToUserData(index);
						case 13:
							return lua.ToString(index);
						case 14:
							return lua.ToUserData(index);
						}
					}
				}
				object obj = lua.ToUserData(index);
				if (obj == null)
				{
					return null;
				}
				return obj;
			}
		}

		private abstract class FFIMethodBase
		{
			public FFIMethodBase(MethodBase minfo)
			{
				this.Method = minfo;
				ParameterInfo[] parameters = minfo.GetParameters();
				this.ParameterTypes = new Type[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					this.ParameterTypes[i] = parameters[i].ParameterType;
				}
			}

			public int Call(ILuaState lua)
			{
				int top = lua.GetTop();
				object obj = lua.ToUserData(2);
				int num = top - 3 + 1;
				object[] array = new object[num];
				for (int i = 0; i < num; i++)
				{
					int index = 3 + i;
					Type t = this.ParameterTypes[i];
					array[i] = LuaFFILib.LuaStackUtil.ToRawValue(lua, index, t);
				}
				object o = this.Method.Invoke(obj, array);
				return this.PushReturnValue(lua, o);
			}

			protected virtual int PushReturnValue(ILuaState lua, object o)
			{
				return 0;
			}

			private MethodBase Method;

			private Type[] ParameterTypes;
		}

		private class FFIMethodInfo : LuaFFILib.FFIMethodBase
		{
			public FFIMethodInfo(MethodInfo minfo) : base(minfo)
			{
				this.ReturnType = minfo.ReturnParameter.ParameterType;
			}

			protected override int PushReturnValue(ILuaState lua, object o)
			{
				return LuaFFILib.LuaStackUtil.PushRawValue(lua, o, this.ReturnType);
			}

			private Type ReturnType;
		}

		private class FFIConstructorInfo : LuaFFILib.FFIMethodBase
		{
			public FFIConstructorInfo(ConstructorInfo cinfo) : base(cinfo)
			{
			}

			protected override int PushReturnValue(ILuaState lua, object o)
			{
				lua.PushLightUserData(o);
				return 1;
			}
		}

		private class FuncSignature
		{
			public string FuncName;

			public string ReturnType;

			public string[] ParameterTypes;
		}

		private class FuncSignatureParser
		{
			public static LuaFFILib.FuncSignature Parse(ILuaState lua, string signature)
			{
				StringLoadInfo loadinfo = new StringLoadInfo(signature);
				return new LuaFFILib.FuncSignatureParser
				{
					Lexer = new LLex(lua, loadinfo, signature),
					Result = new LuaFFILib.FuncSignature()
				}.parse(signature);
			}

			private LuaFFILib.FuncSignature parse(string signature)
			{
				this.Lexer.Next();
				this.FuncSignature();
				return this.Result;
			}

			private void FuncSignature()
			{
				string text = this.TypeName();
				string text2 = this.TypeName();
				if (string.IsNullOrEmpty(text2))
				{
					if (string.IsNullOrEmpty(text))
					{
						this.Lexer.SyntaxError("function name expected");
					}
					else
					{
						this.Result.ReturnType = null;
						this.Result.FuncName = text;
					}
				}
				else
				{
					this.Result.ReturnType = text;
					this.Result.FuncName = text2;
				}
				this.FuncArgs();
				if (this.Lexer.Token.TokenType != 289)
				{
					this.Lexer.SyntaxError("redundant tail characters:" + this.Lexer.Token);
				}
			}

			private string TypeName()
			{
				StringBuilder stringBuilder = new StringBuilder();
				while (this.Lexer.Token.TokenType == 288)
				{
					stringBuilder.Append(this.CheckName());
					if (!this.TestNext(46))
					{
						break;
					}
					stringBuilder.Append('.');
				}
				return stringBuilder.ToString();
			}

			private void ReturnType()
			{
				if (this.Lexer.Token.TokenType == 288)
				{
					NameToken nameToken = this.Lexer.Token as NameToken;
					if (nameToken != null)
					{
						this.Result.ReturnType = nameToken.SemInfo;
						this.Lexer.Next();
					}
				}
				this.Lexer.SyntaxError("return type expected");
			}

			private void FuncName()
			{
				if (this.Lexer.Token.TokenType == 288)
				{
					NameToken nameToken = this.Lexer.Token as NameToken;
					if (nameToken != null)
					{
						this.Result.FuncName = nameToken.SemInfo;
						this.Lexer.Next();
					}
				}
				this.Lexer.SyntaxError("function name expected");
			}

			private string CheckName()
			{
				NameToken nameToken = this.Lexer.Token as NameToken;
				string semInfo = nameToken.SemInfo;
				this.Lexer.Next();
				return semInfo;
			}

			private void TypeList()
			{
				List<string> list = new List<string>();
				while (this.Lexer.Token.TokenType == 288)
				{
					list.Add(this.CheckName());
					if (!this.TestNext(44))
					{
						break;
					}
				}
				this.Result.ParameterTypes = list.ToArray();
			}

			private void FuncArgs()
			{
				if (this.Lexer.Token.TokenType == 40)
				{
					int lineNumber = this.Lexer.LineNumber;
					this.Lexer.Next();
					if (this.TestNext(41))
					{
						this.Result.ParameterTypes = new string[0];
						return;
					}
					this.TypeList();
					this.CheckMatch(41, 40, lineNumber);
				}
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

			private LLex Lexer;

			private LuaFFILib.FuncSignature Result;
		}
	}
}
