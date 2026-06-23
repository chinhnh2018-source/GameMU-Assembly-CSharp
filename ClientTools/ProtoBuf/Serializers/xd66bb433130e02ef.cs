using System;
using System.Reflection;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class xd66bb433130e02ef : x66ec8c25e4c7547d, x9da713b4847e9e6e
	{
		bool x9da713b4847e9e6e.xc3239c74e4ed7078(TypeModel.CallbackType xee5efdfe033701c1)
		{
			return false;
		}

		void x9da713b4847e9e6e.x96a100ebafadb7f4(object xbcea506a33cf9111, TypeModel.CallbackType xee5efdfe033701c1, SerializationContext x0f7b23d1c393aed9)
		{
		}

		public bool xf33087968b28143c
		{
			get
			{
				return false;
			}
		}

		public bool x95726b5912481139
		{
			get
			{
				return true;
			}
		}

		public Type x00c54479c3a7c440
		{
			get
			{
				return this.x34eba590a5ec9a74;
			}
		}

		public xd66bb433130e02ef(Type forType, Type declaredType, x9da713b4847e9e6e rootTail)
		{
			do
			{
				this.x34eba590a5ec9a74 = forType;
				this.xf9d569c75ba50e9e = declaredType;
				this.x92da5e7206d44c3e = rootTail;
				this.x3bdf83ccaeb79259 = this.xbea708d0f50f7aff(true);
				this.x79da86e52178cff2 = this.xbea708d0f50f7aff(false);
			}
			while (false);
		}

		private static bool xb2baca885211e661(Type x43163d22e8cd5a71, Type x7f8a886f51b477eb, Type x3ed4f4f0195b98d7, out MethodInfo x9c9eac3a36336680)
		{
			MethodInfo[] methods = x43163d22e8cd5a71.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			int num = 0;
			MethodInfo methodInfo;
			for (;;)
			{
				if (num >= methods.Length)
				{
					x9c9eac3a36336680 = null;
					goto IL_4A;
				}
				methodInfo = methods[num];
				if (!(methodInfo.Name != "op_Implicit"))
				{
					goto IL_76;
				}
				if (!(methodInfo.Name != "op_Explicit"))
				{
					goto IL_5E;
				}
				IL_0E:
				num++;
				continue;
				IL_76:
				if (methodInfo.ReturnType != x3ed4f4f0195b98d7)
				{
					goto IL_0E;
				}
				if ((uint)num + (uint)num >= 0U)
				{
					goto IL_9D;
				}
				goto IL_0E;
				IL_5E:
				if ((uint)num - (uint)num >= 0U)
				{
					goto IL_76;
				}
				goto IL_9D;
				IL_4A:
				if (((uint)num & 0U) != 0U)
				{
					goto IL_5E;
				}
				return false;
				IL_9D:
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (parameters.Length != 1)
				{
					goto IL_0E;
				}
				if (parameters[0].ParameterType == x7f8a886f51b477eb)
				{
					break;
				}
				bool flag = (uint)num + (uint)num > uint.MaxValue;
				if (flag)
				{
					goto IL_4A;
				}
				goto IL_0E;
			}
			x9c9eac3a36336680 = methodInfo;
			return true;
		}

		public MethodInfo xbea708d0f50f7aff(bool x3bdf83ccaeb79259)
		{
			Type type;
			if (x3bdf83ccaeb79259)
			{
				type = this.xf9d569c75ba50e9e;
				goto IL_54;
			}
			if (255 != 0)
			{
				type = this.x34eba590a5ec9a74;
				goto IL_54;
			}
			IL_0A:
			Type x7f8a886f51b477eb;
			Type x3ed4f4f0195b98d;
			MethodInfo result;
			if (!xd66bb433130e02ef.xb2baca885211e661(this.xf9d569c75ba50e9e, x7f8a886f51b477eb, x3ed4f4f0195b98d, out result))
			{
				if (!xd66bb433130e02ef.xb2baca885211e661(this.x34eba590a5ec9a74, x7f8a886f51b477eb, x3ed4f4f0195b98d, out result))
				{
					throw new InvalidOperationException("No suitable conversion operator found for surrogate: " + this.x34eba590a5ec9a74.FullName + " / " + this.xf9d569c75ba50e9e.FullName);
				}
			}
			return result;
			IL_54:
			x3ed4f4f0195b98d = type;
			x7f8a886f51b477eb = ((!x3bdf83ccaeb79259) ? this.xf9d569c75ba50e9e : this.x34eba590a5ec9a74);
			goto IL_0A;
		}

		public void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter xbdfb620b7167944b)
		{
			this.x92da5e7206d44c3e.x6210059f049f0d48(this.x3bdf83ccaeb79259.Invoke(null, new object[]
			{
				xbcea506a33cf9111
			}), xbdfb620b7167944b);
		}

		public object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			object[] array = new object[]
			{
				xbcea506a33cf9111
			};
			xbcea506a33cf9111 = this.x3bdf83ccaeb79259.Invoke(null, array);
			array[0] = this.x92da5e7206d44c3e.x06b0e25aa6ad68a9(xbcea506a33cf9111, x337e217cb3ba0627);
			return this.x79da86e52178cff2.Invoke(null, array);
		}

		private readonly Type x34eba590a5ec9a74;

		private readonly Type xf9d569c75ba50e9e;

		private readonly MethodInfo x3bdf83ccaeb79259;

		private readonly MethodInfo x79da86e52178cff2;

		private x9da713b4847e9e6e x92da5e7206d44c3e;
	}
}
