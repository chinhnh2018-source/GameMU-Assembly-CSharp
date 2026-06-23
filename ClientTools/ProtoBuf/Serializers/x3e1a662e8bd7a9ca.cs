using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
	internal sealed class x3e1a662e8bd7a9ca : x2b98a1e927f9de72
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

		public x3e1a662e8bd7a9ca(Type forType, FieldInfo field, x66ec8c25e4c7547d tail) : base(tail)
		{
			this.x34eba590a5ec9a74 = forType;
			this.xe01ae93d9fe5a880 = field;
		}

		public override void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			xbcea506a33cf9111 = this.xe01ae93d9fe5a880.GetValue(xbcea506a33cf9111);
			if (xbcea506a33cf9111 != null)
			{
				this.x97cebb6cd312a01b.x6210059f049f0d48(xbcea506a33cf9111, x6b8e154b42d5c1e3);
			}
		}

		public override object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			object value = this.x97cebb6cd312a01b.x06b0e25aa6ad68a9(this.x97cebb6cd312a01b.x95726b5912481139 ? this.xe01ae93d9fe5a880.GetValue(xbcea506a33cf9111) : null, x337e217cb3ba0627);
			while (value != null)
			{
				this.xe01ae93d9fe5a880.SetValue(xbcea506a33cf9111, value);
				if (!false && 4 != 0)
				{
					IL_4D:
					return null;
				}
			}
			goto IL_4D;
		}

		private readonly FieldInfo xe01ae93d9fe5a880;

		private readonly Type x34eba590a5ec9a74;
	}
}
