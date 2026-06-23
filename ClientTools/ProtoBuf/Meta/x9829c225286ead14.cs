using System;
using System.Reflection;

namespace ProtoBuf.Meta
{
	internal abstract class x9829c225286ead14
	{
		[Obsolete("Please use AttributeType instead")]
		public Type xd6886399673b411b()
		{
			return this.AttributeType;
		}

		public abstract bool TryGet(string key, bool publicOnly, out object value);

		public bool x034113016eff150a(string xba08ce632055a1d9, out object xbcea506a33cf9111)
		{
			return this.TryGet(xba08ce632055a1d9, true, out xbcea506a33cf9111);
		}

		public abstract Type AttributeType { get; }

		public static x9829c225286ead14[] xebcf83b00134300b(TypeModel xad70a5849826ecef, Type x43163d22e8cd5a71, bool x27b1fb1b0dfd2efd)
		{
			object[] customAttributes = x43163d22e8cd5a71.GetCustomAttributes(x27b1fb1b0dfd2efd);
			int i;
			x9829c225286ead14[] array;
			if ((x27b1fb1b0dfd2efd ? 1U : 0U) + (uint)i <= 4294967295U)
			{
				array = new x9829c225286ead14[customAttributes.Length];
				if (((uint)i & 0U) == 0U)
				{
					for (i = 0; i < customAttributes.Length; i++)
					{
						array[i] = new x9829c225286ead14.x514647735ce5c4b0((Attribute)customAttributes[i]);
					}
				}
			}
			return array;
		}

		public static x9829c225286ead14[] xebcf83b00134300b(TypeModel xad70a5849826ecef, MemberInfo xf0b74f36659f8180, bool x27b1fb1b0dfd2efd)
		{
			object[] customAttributes = xf0b74f36659f8180.GetCustomAttributes(x27b1fb1b0dfd2efd);
			x9829c225286ead14[] array;
			int num;
			if (!false)
			{
				array = new x9829c225286ead14[customAttributes.Length];
				num = 0;
				goto IL_1F;
			}
			IL_0B:
			array[num] = new x9829c225286ead14.x514647735ce5c4b0((Attribute)customAttributes[num]);
			num++;
			IL_1F:
			if (num >= customAttributes.Length)
			{
				return array;
			}
			goto IL_0B;
		}

		public static x9829c225286ead14[] xebcf83b00134300b(TypeModel xad70a5849826ecef, Assembly xd3764619ec304ff0)
		{
			object[] customAttributes = xd3764619ec304ff0.GetCustomAttributes(false);
			x9829c225286ead14[] array;
			while (!false)
			{
				array = new x9829c225286ead14[customAttributes.Length];
				int i = 0;
				bool flag = (uint)i > uint.MaxValue;
				if (flag)
				{
					break;
				}
				while (i < customAttributes.Length)
				{
					array[i] = new x9829c225286ead14.x514647735ce5c4b0((Attribute)customAttributes[i]);
					i++;
				}
				flag = ((uint)i - (uint)i > uint.MaxValue);
				if (!flag)
				{
					break;
				}
			}
			return array;
		}

		public abstract object Target { get; }

		private class x514647735ce5c4b0 : x9829c225286ead14
		{
			public override object Target
			{
				get
				{
					return this.x7918156d406a715d;
				}
			}

			public override Type AttributeType
			{
				get
				{
					return this.x7918156d406a715d.GetType();
				}
			}

			public override bool TryGet(string key, bool publicOnly, out object value)
			{
				MemberInfo[] array = x479f2661aae93792.x37d5499f81df29d6(this.x7918156d406a715d.GetType(), publicOnly);
				int i;
				if (((uint)i | 4294967295U) == 0U)
				{
					goto IL_88;
				}
				MemberInfo[] array2 = array;
				i = 0;
				bool flag;
				while (i < array2.Length)
				{
					MemberInfo memberInfo = array2[i];
					if (!string.Equals(memberInfo.Name, key, StringComparison.OrdinalIgnoreCase))
					{
						i++;
						if (((flag ? 1U : 0U) & 0U) == 0U)
						{
							continue;
						}
					}
					else
					{
						PropertyInfo propertyInfo = memberInfo as PropertyInfo;
						if (propertyInfo == null)
						{
							bool flag2 = ((publicOnly ? 1U : 0U) | 4294967294U) == 0U;
							if (!flag2)
							{
								FieldInfo fieldInfo = memberInfo as FieldInfo;
								if (fieldInfo != null)
								{
									value = fieldInfo.GetValue(this.x7918156d406a715d);
									goto IL_88;
								}
							}
							throw new NotSupportedException(memberInfo.GetType().Name);
						}
						value = propertyInfo.GetValue(this.x7918156d406a715d, null);
					}
					return true;
				}
				IL_22:
				value = null;
				return false;
				IL_88:
				if (false)
				{
					goto IL_22;
				}
				flag = true;
				return flag;
			}

			public x514647735ce5c4b0(Attribute attribute)
			{
				this.x7918156d406a715d = attribute;
			}

			private readonly Attribute x7918156d406a715d;
		}
	}
}
