using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class x8f2d2e2582fd9f3b : x2b98a1e927f9de72, x66ec8c25e4c7547d, x9da713b4847e9e6e
	{
		public bool xf5cdd81490e1c274(TypeModel.CallbackType xee5efdfe033701c1)
		{
			x9da713b4847e9e6e x9da713b4847e9e6e = this.x97cebb6cd312a01b as x9da713b4847e9e6e;
			return (false || x9da713b4847e9e6e != null) && x9da713b4847e9e6e.xf5cdd81490e1c274(xee5efdfe033701c1);
		}

		public void x4e4178637618473f(object xbcea506a33cf9111, TypeModel.CallbackType xee5efdfe033701c1, SerializationContext x0f7b23d1c393aed9)
		{
			x9da713b4847e9e6e x9da713b4847e9e6e = this.x97cebb6cd312a01b as x9da713b4847e9e6e;
			while (x9da713b4847e9e6e != null)
			{
				x9da713b4847e9e6e.x4e4178637618473f(xbcea506a33cf9111, xee5efdfe033701c1, x0f7b23d1c393aed9);
				if (!false)
				{
					if (8 != 0)
					{
						return;
					}
					break;
				}
			}
		}

		public override Type x00c54479c3a7c440
		{
			get
			{
				return this.x97cebb6cd312a01b.x00c54479c3a7c440;
			}
		}

		public x8f2d2e2582fd9f3b(int fieldNumber, WireType wireType, bool strict, x66ec8c25e4c7547d tail) : base(tail)
		{
			this.xade3b695478596d6 = fieldNumber;
			this.xa5694e1c82a939b4 = wireType;
			this.xec724f9375e8eee5 = strict;
		}

		public override bool x95726b5912481139
		{
			get
			{
				return this.x97cebb6cd312a01b.x95726b5912481139;
			}
		}

		public override bool xf33087968b28143c
		{
			get
			{
				return this.x97cebb6cd312a01b.xf33087968b28143c;
			}
		}

		private bool xb6d5743d9739d40c
		{
			get
			{
				return (this.xa5694e1c82a939b4 & (WireType)(-8)) != WireType.Variant;
			}
		}

		public override object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			if (this.xec724f9375e8eee5)
			{
				x337e217cb3ba0627.Assert(this.xa5694e1c82a939b4);
			}
			else if (this.xb6d5743d9739d40c)
			{
				x337e217cb3ba0627.Hint(this.xa5694e1c82a939b4);
			}
			return this.x97cebb6cd312a01b.x06b0e25aa6ad68a9(xbcea506a33cf9111, x337e217cb3ba0627);
		}

		public override void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			ProtoWriter.WriteFieldHeader(this.xade3b695478596d6, this.xa5694e1c82a939b4, x6b8e154b42d5c1e3);
			this.x97cebb6cd312a01b.x6210059f049f0d48(xbcea506a33cf9111, x6b8e154b42d5c1e3);
		}

		private readonly bool xec724f9375e8eee5;

		private readonly int xade3b695478596d6;

		private readonly WireType xa5694e1c82a939b4;
	}
}
