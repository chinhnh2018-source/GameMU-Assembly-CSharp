using System;
using System.Reflection;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class x7024e111a877c7fe : x66ec8c25e4c7547d
	{
		public static x7024e111a877c7fe xf299a6067f9cb3b3(Type x43163d22e8cd5a71, TypeModel xad70a5849826ecef)
		{
			MethodInfo method;
			if (x43163d22e8cd5a71 != null)
			{
				method = x43163d22e8cd5a71.GetMethod("Parse", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public, null, new Type[]
				{
					xad70a5849826ecef.MapType(typeof(string))
				}, null);
				goto IL_1D;
			}
			if (false)
			{
				goto IL_0C;
			}
			goto IL_84;
			IL_33:
			while (false)
			{
				if (false)
				{
					goto IL_3F;
				}
				if (!false)
				{
					goto IL_0C;
				}
			}
			if (3 != 0)
			{
				goto IL_13;
			}
			if (!false && !false)
			{
				goto IL_E1;
			}
			goto IL_1D;
			IL_0C:
			if (2 == 0)
			{
				goto IL_33;
			}
			IL_13:
			return new x7024e111a877c7fe(method);
			IL_1D:
			if (method == null)
			{
				goto IL_E1;
			}
			if (method.ReturnType != x43163d22e8cd5a71)
			{
				goto IL_E1;
			}
			if (-2147483648 == 0)
			{
				goto IL_DC;
			}
			if (2 == 0)
			{
				goto IL_73;
			}
			IL_3F:
			if (!x479f2661aae93792.x25f40ad8c018c1ab(x43163d22e8cd5a71))
			{
				goto IL_13;
			}
			goto IL_73;
			IL_6A:
			return null;
			IL_73:
			MethodInfo methodInfo = x7024e111a877c7fe.x46eb97b30737d8a2(x43163d22e8cd5a71);
			if (methodInfo == null)
			{
				if (!false)
				{
					goto IL_DC;
				}
			}
			else
			{
				if (methodInfo.ReturnType == xad70a5849826ecef.MapType(typeof(string)))
				{
					goto IL_33;
				}
				goto IL_6A;
			}
			IL_84:
			throw new ArgumentNullException("type");
			IL_DC:
			goto IL_6A;
			IL_E1:
			return null;
		}

		private static MethodInfo x46eb97b30737d8a2(Type x43163d22e8cd5a71)
		{
			return x43163d22e8cd5a71.GetMethod("ToString", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public, null, x479f2661aae93792.xf6f6ea67665595b8, null);
		}

		private x7024e111a877c7fe(MethodInfo parse)
		{
			this.xaeb32f36f12443d3 = parse;
		}

		public Type x00c54479c3a7c440
		{
			get
			{
				return this.xaeb32f36f12443d3.DeclaringType;
			}
		}

		bool x66ec8c25e4c7547d.x84790f641eeacdd2
		{
			get
			{
				return false;
			}
		}

		bool x66ec8c25e4c7547d.x60089ae1c8e627b2
		{
			get
			{
				return true;
			}
		}

		public object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			return this.xaeb32f36f12443d3.Invoke(null, new object[]
			{
				x337e217cb3ba0627.ReadString()
			});
		}

		public void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			ProtoWriter.WriteString(xbcea506a33cf9111.ToString(), x6b8e154b42d5c1e3);
		}

		private readonly MethodInfo xaeb32f36f12443d3;
	}
}
