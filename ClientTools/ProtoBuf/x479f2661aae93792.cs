using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace ProtoBuf
{
	internal class x479f2661aae93792
	{
		private x479f2661aae93792()
		{
		}

		public static StringBuilder xfa6a23a9b7368053(StringBuilder xd07ce4b74c5774a7)
		{
			return xd07ce4b74c5774a7.AppendLine();
		}

		public static bool x1c140bd1078ddda1(string xbcea506a33cf9111)
		{
			return xbcea506a33cf9111 == null || xbcea506a33cf9111.Length == 0;
		}

		[Conditional("DEBUG")]
		public static void x0e85e73526d53aa5(string x1f25abf5fb75e795, object xa59bff7708de3a18)
		{
			try
			{
				if (xa59bff7708de3a18 != null)
				{
					xa59bff7708de3a18.ToString();
				}
			}
			catch
			{
			}
		}

		[Conditional("DEBUG")]
		public static void x0e85e73526d53aa5(string x1f25abf5fb75e795)
		{
		}

		[Conditional("TRACE")]
		public static void x9ab9a9ca11ba90e9(string x1f25abf5fb75e795)
		{
		}

		[Conditional("DEBUG")]
		public static void xa2126e5407196f8d(bool x29ca7772d281a736, string x1f25abf5fb75e795)
		{
		}

		[Conditional("DEBUG")]
		public static void xa2126e5407196f8d(bool x29ca7772d281a736, string x1f25abf5fb75e795, params object[] xce8d8c7e3c2c2426)
		{
		}

		[Conditional("DEBUG")]
		public static void xa2126e5407196f8d(bool x29ca7772d281a736)
		{
			if (!x29ca7772d281a736 && Debugger.IsAttached)
			{
				Debugger.Break();
			}
		}

		public static void xee9aac96ed24c7f9(int[] x83f3ea1d0a03c7e1, object[] x0788cd5a9865fc16)
		{
			bool flag;
			do
			{
				flag = false;
				int i = 1;
				for (;;)
				{
					IL_84:
					while (i < x83f3ea1d0a03c7e1.Length)
					{
						while (x83f3ea1d0a03c7e1[i - 1] > x83f3ea1d0a03c7e1[i])
						{
							int num = x83f3ea1d0a03c7e1[i];
							x83f3ea1d0a03c7e1[i] = x83f3ea1d0a03c7e1[i - 1];
							x83f3ea1d0a03c7e1[i - 1] = num;
							object obj = x0788cd5a9865fc16[i];
							x0788cd5a9865fc16[i] = x0788cd5a9865fc16[i - 1];
							x0788cd5a9865fc16[i - 1] = obj;
							if (false)
							{
								return;
							}
							if ((uint)i + (uint)i < 0U)
							{
								goto IL_84;
							}
							bool flag2 = (flag ? 1U : 0U) + (uint)i > uint.MaxValue;
							if (flag2)
							{
								goto IL_84;
							}
							if (((uint)num & 0U) == 0U)
							{
								flag = true;
								break;
							}
							flag2 = ((uint)i < 0U);
							if (flag2)
							{
								return;
							}
						}
						i++;
					}
					break;
				}
			}
			while (flag);
		}

		public static void x6a87193e5bb23362(byte[] x7f8a886f51b477eb, int x290794402e16c12c, byte[] x3ed4f4f0195b98d7, int xf8203a3f0b65ab06, int x10f4d88af727adbc)
		{
			Buffer.BlockCopy(x7f8a886f51b477eb, x290794402e16c12c, x3ed4f4f0195b98d7, xf8203a3f0b65ab06, x10f4d88af727adbc);
		}

		public static bool xa696f2cecd0cd4ae(float xbcea506a33cf9111)
		{
			return float.IsInfinity(xbcea506a33cf9111);
		}

		internal static MethodInfo x40c628a821a074f8(Type xe1617f817cdad6f8, string xc15bd84e01929885)
		{
			return xe1617f817cdad6f8.GetMethod(xc15bd84e01929885, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		internal static MethodInfo x75d730c39ffa52f2(Type xe1617f817cdad6f8, string xc15bd84e01929885)
		{
			return xe1617f817cdad6f8.GetMethod(xc15bd84e01929885, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		internal static MethodInfo x40c628a821a074f8(Type xe1617f817cdad6f8, string xc15bd84e01929885, Type[] x8f76a1a9d286e18b)
		{
			if (x8f76a1a9d286e18b == null)
			{
				x8f76a1a9d286e18b = x479f2661aae93792.xf6f6ea67665595b8;
			}
			return xe1617f817cdad6f8.GetMethod(xc15bd84e01929885, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, x8f76a1a9d286e18b, null);
		}

		internal static bool x0ec48511dfd2262d(Type x43163d22e8cd5a71, Type x9bb4bfb109b5e594)
		{
			return x43163d22e8cd5a71.IsSubclassOf(x9bb4bfb109b5e594);
		}

		public static bool xa696f2cecd0cd4ae(double xbcea506a33cf9111)
		{
			return double.IsInfinity(xbcea506a33cf9111);
		}

		public static xd669244d58bc09c0 xf70eec89828a813c(Type x43163d22e8cd5a71)
		{
			TypeCode typeCode = Type.GetTypeCode(x43163d22e8cd5a71);
			if (false)
			{
				if (-1 != 0)
				{
					goto IL_63;
				}
			}
			else
			{
				switch (typeCode)
				{
				case TypeCode.Empty:
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.DateTime:
				case TypeCode.String:
					return (xd669244d58bc09c0)typeCode;
				}
				if (x43163d22e8cd5a71 != typeof(TimeSpan))
				{
					goto IL_63;
				}
				return xd669244d58bc09c0.x69ec9d2404a6b229;
			}
			IL_14:
			return xd669244d58bc09c0.xf6c17f648b65c793;
			IL_63:
			if (x43163d22e8cd5a71 == typeof(Guid))
			{
				return xd669244d58bc09c0.x0217cda8370c1f17;
			}
			if (x43163d22e8cd5a71 == typeof(Uri))
			{
				return xd669244d58bc09c0.xb405a444ca77e2d4;
			}
			if (x43163d22e8cd5a71 != typeof(byte[]))
			{
				while (x43163d22e8cd5a71 != typeof(Type))
				{
					if (-2 != 0)
					{
						while (!false)
						{
							if (2 != 0)
							{
								return xd669244d58bc09c0.xf6c17f648b65c793;
							}
						}
					}
					while (!false)
					{
						if (!false)
						{
							if (4 == 0)
							{
								break;
							}
							goto IL_14;
						}
					}
				}
				return xd669244d58bc09c0.x3146d638ec378671;
			}
			return xd669244d58bc09c0.x62a0c09ce3a5e8fb;
		}

		internal static Type xe5e08d1dc9f521de(Type x43163d22e8cd5a71)
		{
			return Nullable.GetUnderlyingType(x43163d22e8cd5a71);
		}

		internal static bool x25f40ad8c018c1ab(Type x43163d22e8cd5a71)
		{
			return x43163d22e8cd5a71.IsValueType;
		}

		internal static bool xb636fcab7a16c388(Type x43163d22e8cd5a71)
		{
			return x43163d22e8cd5a71.IsEnum;
		}

		internal static MethodInfo xbfdf1fd45a6330fd(PropertyInfo x46710263f0fedd95, bool xcbca40eaeace3b79)
		{
			if (x46710263f0fedd95 == null)
			{
				return null;
			}
			return x46710263f0fedd95.GetGetMethod(xcbca40eaeace3b79);
		}

		internal static MethodInfo x960b3b86642a1b2d(PropertyInfo x46710263f0fedd95, bool xcbca40eaeace3b79)
		{
			if (x46710263f0fedd95 == null)
			{
				return null;
			}
			return x46710263f0fedd95.GetSetMethod(xcbca40eaeace3b79);
		}

		internal static ConstructorInfo xdaa1a96eb962bff0(Type x43163d22e8cd5a71, Type[] x2eb67d2eee91fcba, bool xcbca40eaeace3b79)
		{
			return x43163d22e8cd5a71.GetConstructor(xcbca40eaeace3b79 ? (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : (BindingFlags.Instance | BindingFlags.Public), null, x2eb67d2eee91fcba, null);
		}

		internal static ConstructorInfo[] x690558c2b7705ce8(Type x43163d22e8cd5a71, bool xcbca40eaeace3b79)
		{
			return x43163d22e8cd5a71.GetConstructors(xcbca40eaeace3b79 ? (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : (BindingFlags.Instance | BindingFlags.Public));
		}

		internal static PropertyInfo x4ff37084a5d7d57f(Type x43163d22e8cd5a71, string xc15bd84e01929885)
		{
			return x43163d22e8cd5a71.GetProperty(xc15bd84e01929885);
		}

		internal static object x475bc6fe627d885f(Type x43163d22e8cd5a71, string xbcea506a33cf9111)
		{
			return Enum.Parse(x43163d22e8cd5a71, xbcea506a33cf9111, true);
		}

		internal static MemberInfo[] x37d5499f81df29d6(Type x43163d22e8cd5a71, bool xe91a754aa3e42bba)
		{
			BindingFlags bindingFlags;
			if (xe91a754aa3e42bba)
			{
				bindingFlags = (BindingFlags.Instance | BindingFlags.Public);
				goto IL_41;
			}
			IL_3B:
			bindingFlags = (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			IL_41:
			BindingFlags bindingAttr = bindingFlags;
			if (-1 == 0)
			{
				goto IL_3B;
			}
			PropertyInfo[] properties = x43163d22e8cd5a71.GetProperties(bindingAttr);
			FieldInfo[] fields = x43163d22e8cd5a71.GetFields(bindingAttr);
			MemberInfo[] array = new MemberInfo[fields.Length + properties.Length];
			properties.CopyTo(array, 0);
			fields.CopyTo(array, properties.Length);
			if (-2 == 0)
			{
				goto IL_3B;
			}
			return array;
		}

		internal static Type xc880d46f290519e2(MemberInfo xf0b74f36659f8180)
		{
			MemberTypes memberType = xf0b74f36659f8180.MemberType;
			for (;;)
			{
				while (memberType != MemberTypes.Field)
				{
					if (memberType != MemberTypes.Property)
					{
						goto IL_25;
					}
					if (!false)
					{
						if (!false)
						{
							goto IL_3A;
						}
					}
				}
				goto IL_27;
			}
			IL_25:
			goto IL_41;
			IL_27:
			return ((FieldInfo)xf0b74f36659f8180).FieldType;
			IL_3A:
			if (2 != 0)
			{
				return ((PropertyInfo)xf0b74f36659f8180).PropertyType;
			}
			IL_41:
			return null;
		}

		internal static bool xd3703a5e339a1b56(Type x11d58b056c032b03, Type x43163d22e8cd5a71)
		{
			return x11d58b056c032b03.IsAssignableFrom(x43163d22e8cd5a71);
		}

		public static readonly Type[] xf6f6ea67665595b8 = new Type[0];
	}
}
