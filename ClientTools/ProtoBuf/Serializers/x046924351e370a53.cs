using System;
using System.Reflection;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class x046924351e370a53 : x66ec8c25e4c7547d, x9da713b4847e9e6e
	{
		public x046924351e370a53(RuntimeTypeModel model, ConstructorInfo ctor, MemberInfo[] members)
		{
			if (false)
			{
				goto IL_CA;
			}
			IL_22A:
			int num;
			while (ctor != null)
			{
				if ((uint)num - (uint)num <= 4294967295U)
				{
					goto IL_1E6;
				}
			}
			goto IL_24B;
			IL_1E:
			Type parameterType;
			Type concreteType;
			x66ec8c25e4c7547d x66ec8c25e4c7547d2;
			WireType wireType;
			x66ec8c25e4c7547d x66ec8c25e4c7547d = new xdeb0ec0887c9560b(model, parameterType, concreteType, x66ec8c25e4c7547d2, num + 1, false, wireType, true, false, false);
			IL_33:
			this.xcd7938e15d53f21e[num] = x66ec8c25e4c7547d;
			num++;
			IL_41:
			ParameterInfo[] parameters;
			Type type;
			Type type2;
			if (num >= members.Length)
			{
				if (!false)
				{
					return;
				}
				if (!false)
				{
					goto IL_CA;
				}
			}
			else
			{
				parameterType = parameters[num].ParameterType;
				type = null;
				concreteType = null;
				MetaType.xaadd3c2a45ddcdb5(model, parameterType, ref type, ref concreteType);
				if (type != null)
				{
					goto IL_127;
				}
				type2 = parameterType;
				goto IL_12C;
			}
			IL_56:
			IL_70:
			if (parameterType.IsArray)
			{
				x66ec8c25e4c7547d = new x72fd17eaff71827a(model, x66ec8c25e4c7547d2, num + 1, false, wireType, parameterType, false, false);
				goto IL_33;
			}
			goto IL_1E;
			IL_CA:
			Type type3;
			x66ec8c25e4c7547d2 = ValueMember.xac123cbb8d7a8e65(model, DataFormat.Default, type3, out wireType, false, false, false, true);
			if (2 != 0 && x66ec8c25e4c7547d2 != null)
			{
				x66ec8c25e4c7547d2 = new x8f2d2e2582fd9f3b(num + 1, wireType, false, x66ec8c25e4c7547d2);
				if (((uint)num | 255U) != 0U)
				{
					if (false)
					{
						goto IL_C1;
					}
				}
				else if (((uint)num & 0U) == 0U && (uint)num + (uint)num >= 0U)
				{
					goto IL_70;
				}
				if (type != null)
				{
					if (255 != 0)
					{
						goto IL_56;
					}
					if (-1 == 0)
					{
						goto IL_33;
					}
					if (!false)
					{
						goto IL_1E;
					}
					goto IL_24B;
				}
				IL_C1:
				x66ec8c25e4c7547d = x66ec8c25e4c7547d2;
				goto IL_33;
			}
			throw new InvalidOperationException("No serializer defined for type: " + type3.FullName);
			IL_127:
			type2 = type;
			IL_12C:
			type3 = type2;
			bool flag;
			if (255 != 0)
			{
				flag = ((uint)num - (uint)num < 0U);
				if (flag)
				{
					goto IL_1C4;
				}
				goto IL_1D4;
			}
			IL_178:
			this.x738cd76b048d3324 = ctor;
			this.x9b0de312ad5acf8b = members;
			this.xcd7938e15d53f21e = new x66ec8c25e4c7547d[members.Length];
			parameters = ctor.GetParameters();
			num = 0;
			goto IL_41;
			IL_1C4:
			if (members == null)
			{
				throw new ArgumentNullException("members");
			}
			if (false)
			{
				goto IL_127;
			}
			if (255 != 0)
			{
				goto IL_178;
			}
			IL_1D4:
			flag = ((uint)num < 0U);
			if (!flag)
			{
				flag = ((uint)num > uint.MaxValue);
				if (flag)
				{
					if (!false)
					{
						goto IL_22A;
					}
					goto IL_24B;
				}
				else
				{
					flag = ((uint)num + (uint)num > uint.MaxValue);
					if (flag)
					{
						return;
					}
					goto IL_CA;
				}
			}
			IL_1E6:
			if (!false)
			{
				goto IL_1C4;
			}
			goto IL_56;
			IL_24B:
			throw new ArgumentNullException("ctor");
		}

		public bool xf5cdd81490e1c274(TypeModel.CallbackType xee5efdfe033701c1)
		{
			return false;
		}

		public void x4e4178637618473f(object xbcea506a33cf9111, TypeModel.CallbackType xee5efdfe033701c1, SerializationContext x0f7b23d1c393aed9)
		{
		}

		public Type x00c54479c3a7c440
		{
			get
			{
				return this.x738cd76b048d3324.DeclaringType;
			}
		}

		private object x3f88a25febd23896(object xa59bff7708de3a18, int xc0c4c459c6ccbd00)
		{
			PropertyInfo propertyInfo;
			if ((propertyInfo = (this.x9b0de312ad5acf8b[xc0c4c459c6ccbd00] as PropertyInfo)) != null)
			{
				if (xa59bff7708de3a18 != null)
				{
					return propertyInfo.GetValue(xa59bff7708de3a18, null);
				}
				if (!x479f2661aae93792.x25f40ad8c018c1ab(propertyInfo.PropertyType))
				{
					return null;
				}
				if (2147483647 == 0)
				{
					goto IL_86;
				}
			}
			else
			{
				FieldInfo fieldInfo;
				if ((fieldInfo = (this.x9b0de312ad5acf8b[xc0c4c459c6ccbd00] as FieldInfo)) == null)
				{
					goto IL_86;
				}
				if (!false)
				{
					if (xa59bff7708de3a18 != null)
					{
						return fieldInfo.GetValue(xa59bff7708de3a18);
					}
					if (x479f2661aae93792.x25f40ad8c018c1ab(fieldInfo.FieldType))
					{
						return Activator.CreateInstance(fieldInfo.FieldType);
					}
					return null;
				}
			}
			return Activator.CreateInstance(propertyInfo.PropertyType);
			IL_86:
			throw new InvalidOperationException();
		}

		public object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			object[] array = new object[this.x9b0de312ad5acf8b.Length];
			bool flag;
			int i;
			int num;
			if ((flag ? 1U : 0U) >= 0U)
			{
				do
				{
					flag = false;
					if (xbcea506a33cf9111 == null)
					{
						flag = true;
					}
					i = 0;
					bool flag2 = (uint)num > uint.MaxValue;
					if (flag2)
					{
						goto IL_EE;
					}
				}
				while ((flag ? 1U : 0U) - (uint)i < 0U);
			}
			while (i < array.Length)
			{
				array[i] = this.x3f88a25febd23896(xbcea506a33cf9111, i);
				i++;
			}
			while ((num = x337e217cb3ba0627.ReadFieldHeader()) > 0)
			{
				flag = true;
				bool flag2 = ((uint)i | uint.MaxValue) == 0U;
				if (!flag2 && num > this.xcd7938e15d53f21e.Length)
				{
					x337e217cb3ba0627.SkipField();
				}
				else
				{
					x66ec8c25e4c7547d x66ec8c25e4c7547d = this.xcd7938e15d53f21e[num - 1];
					array[num - 1] = this.xcd7938e15d53f21e[num - 1].x06b0e25aa6ad68a9(x66ec8c25e4c7547d.x95726b5912481139 ? array[num - 1] : null, x337e217cb3ba0627);
				}
			}
			if (!flag)
			{
				return xbcea506a33cf9111;
			}
			IL_EE:
			return this.x738cd76b048d3324.Invoke(array);
		}

		public void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			int i = 0;
			while (i < this.xcd7938e15d53f21e.Length)
			{
				object obj = this.x3f88a25febd23896(xbcea506a33cf9111, i);
				if (obj != null)
				{
					this.xcd7938e15d53f21e[i].x6210059f049f0d48(obj, x6b8e154b42d5c1e3);
				}
				i++;
				if ((uint)i + (uint)i > 4294967295U)
				{
					return;
				}
			}
		}

		public bool x95726b5912481139
		{
			get
			{
				return true;
			}
		}

		public bool xf33087968b28143c
		{
			get
			{
				return false;
			}
		}

		private Type xc880d46f290519e2(int xc0c4c459c6ccbd00)
		{
			Type type = x479f2661aae93792.xc880d46f290519e2(this.x9b0de312ad5acf8b[xc0c4c459c6ccbd00]);
			if (type != null)
			{
				return type;
			}
			throw new InvalidOperationException();
		}

		private readonly MemberInfo[] x9b0de312ad5acf8b;

		private readonly ConstructorInfo x738cd76b048d3324;

		private x66ec8c25e4c7547d[] xcd7938e15d53f21e;
	}
}
