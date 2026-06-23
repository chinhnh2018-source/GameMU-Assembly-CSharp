using System;
using System.Reflection;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class xe9b44656581cbaf8 : x2b98a1e927f9de72
	{
		public override Type x00c54479c3a7c440
		{
			get
			{
				return this.x34eba590a5ec9a74;
			}
		}

		public override bool x95726b5912481139
		{
			get
			{
				return true;
			}
		}

		public override bool xf33087968b28143c
		{
			get
			{
				return false;
			}
		}

		public xe9b44656581cbaf8(TypeModel model, Type forType, PropertyInfo property, x66ec8c25e4c7547d tail) : base(tail)
		{
			this.x34eba590a5ec9a74 = forType;
			this.x46710263f0fedd95 = property;
			xe9b44656581cbaf8.xe77f10e627ba1599(model, property, tail, out this.x12ec0b76352457fc, true);
			this.xd084d7f8ca800f23 = xe9b44656581cbaf8.xdce8ae8fdc7b3743(model, property);
		}

		private static void xe77f10e627ba1599(TypeModel xad70a5849826ecef, PropertyInfo x46710263f0fedd95, x66ec8c25e4c7547d xf550933a0b8c12b4, out bool x2ef2654185301b3a, bool xcbca40eaeace3b79)
		{
			if (x46710263f0fedd95 == null)
			{
				throw new ArgumentNullException("property");
			}
			for (;;)
			{
				IL_AE:
				x2ef2654185301b3a = (xf550933a0b8c12b4.xf33087968b28143c && (xe9b44656581cbaf8.xdce8ae8fdc7b3743(xad70a5849826ecef, x46710263f0fedd95) != null || (x46710263f0fedd95.CanWrite && x479f2661aae93792.x960b3b86642a1b2d(x46710263f0fedd95, xcbca40eaeace3b79) != null)));
				if (!x46710263f0fedd95.CanRead || x479f2661aae93792.xbfdf1fd45a6330fd(x46710263f0fedd95, xcbca40eaeace3b79) == null)
				{
					goto IL_ED;
				}
				bool flag = ((xcbca40eaeace3b79 ? 1U : 0U) | 15U) == 0U;
				if (!flag && x2ef2654185301b3a)
				{
					break;
				}
				while (xf550933a0b8c12b4.x95726b5912481139)
				{
					if (255 == 0)
					{
						goto IL_AE;
					}
					if (x479f2661aae93792.x25f40ad8c018c1ab(xf550933a0b8c12b4.x00c54479c3a7c440))
					{
						break;
					}
					if ((xcbca40eaeace3b79 ? 1U : 0U) + (xcbca40eaeace3b79 ? 1U : 0U) <= 4294967295U)
					{
						goto Block_4;
					}
				}
				goto IL_76;
			}
			Block_4:
			return;
			IL_76:
			throw new InvalidOperationException("Cannot apply changes to property " + x46710263f0fedd95.DeclaringType.FullName + "." + x46710263f0fedd95.Name);
			IL_ED:
			throw new InvalidOperationException("Cannot serialize property without a get accessor");
		}

		private static MethodInfo xdce8ae8fdc7b3743(TypeModel xad70a5849826ecef, PropertyInfo x46710263f0fedd95)
		{
			Type reflectedType = x46710263f0fedd95.ReflectedType;
			if (2147483647 == 0)
			{
				goto IL_30;
			}
			if (false)
			{
				goto IL_4D;
			}
			MethodInfo methodInfo = x479f2661aae93792.x40c628a821a074f8(reflectedType, "Set" + x46710263f0fedd95.Name, new Type[]
			{
				x46710263f0fedd95.PropertyType
			});
			if (false)
			{
				goto IL_30;
			}
			goto IL_3E;
			IL_13:
			if (false)
			{
				if (false)
				{
					goto IL_3E;
				}
				if (false)
				{
					return methodInfo;
				}
				if (!false)
				{
					goto IL_24;
				}
				goto IL_41;
			}
			IL_1F:
			return null;
			IL_24:
			if (false)
			{
				goto IL_4D;
			}
			if (2147483647 != 0)
			{
				goto IL_1F;
			}
			goto IL_13;
			IL_2B:
			if (!false)
			{
				goto IL_13;
			}
			goto IL_3E;
			IL_30:
			if (2147483647 == 0)
			{
				goto IL_24;
			}
			if (-2 != 0)
			{
				goto IL_2B;
			}
			IL_3E:
			if (methodInfo != null)
			{
				goto IL_4D;
			}
			IL_41:
			goto IL_24;
			IL_4D:
			if (!methodInfo.IsPublic)
			{
				goto IL_1F;
			}
			if (methodInfo.ReturnType != xad70a5849826ecef.MapType(typeof(void)))
			{
				goto IL_2B;
			}
			return methodInfo;
		}

		public override void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			xbcea506a33cf9111 = this.x46710263f0fedd95.GetValue(xbcea506a33cf9111, null);
			if (xbcea506a33cf9111 != null)
			{
				this.x97cebb6cd312a01b.x6210059f049f0d48(xbcea506a33cf9111, x6b8e154b42d5c1e3);
			}
		}

		public override object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			object obj;
			if (this.x97cebb6cd312a01b.x95726b5912481139)
			{
				obj = this.x46710263f0fedd95.GetValue(xbcea506a33cf9111, null);
				goto IL_73;
			}
			if (!false)
			{
				obj = null;
				goto IL_73;
			}
			IL_10:
			object obj2;
			while (obj2 != null)
			{
				if (this.xd084d7f8ca800f23 == null)
				{
					this.x46710263f0fedd95.SetValue(xbcea506a33cf9111, obj2, null);
					if (!false)
					{
						break;
					}
				}
				this.xd084d7f8ca800f23.Invoke(xbcea506a33cf9111, new object[]
				{
					obj2
				});
				if (-1 != 0)
				{
					break;
				}
			}
			goto IL_93;
			IL_73:
			object xbcea506a33cf9112 = obj;
			obj2 = this.x97cebb6cd312a01b.x06b0e25aa6ad68a9(xbcea506a33cf9112, x337e217cb3ba0627);
			if (this.x12ec0b76352457fc)
			{
				if (!false)
				{
					goto IL_10;
				}
				if (!false)
				{
					goto IL_10;
				}
			}
			IL_93:
			return null;
		}

		internal static bool x2eec4c68253a13df(TypeModel xad70a5849826ecef, MemberInfo xf0b74f36659f8180)
		{
			if (xf0b74f36659f8180 == null)
			{
				throw new ArgumentNullException("member");
			}
			PropertyInfo propertyInfo = xf0b74f36659f8180 as PropertyInfo;
			if (propertyInfo == null)
			{
				return xf0b74f36659f8180 is FieldInfo;
			}
			if (!propertyInfo.CanWrite)
			{
				if (!false)
				{
				}
				return xe9b44656581cbaf8.xdce8ae8fdc7b3743(xad70a5849826ecef, propertyInfo) != null;
			}
			return true;
		}

		private readonly PropertyInfo x46710263f0fedd95;

		private readonly Type x34eba590a5ec9a74;

		private readonly bool x12ec0b76352457fc;

		private readonly MethodInfo xd084d7f8ca800f23;
	}
}
