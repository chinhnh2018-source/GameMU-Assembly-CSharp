using System;
using System.Reflection;

namespace ProtoBuf.Serializers
{
	internal sealed class x78d7a2d462b6e713 : x2b98a1e927f9de72
	{
		public override Type x00c54479c3a7c440
		{
			get
			{
				return this.x97cebb6cd312a01b.x00c54479c3a7c440;
			}
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

		public x78d7a2d462b6e713(MethodInfo getSpecified, MethodInfo setSpecified, x66ec8c25e4c7547d tail) : base(tail)
		{
			for (;;)
			{
				if (getSpecified == null)
				{
					if (255 == 0)
					{
						return;
					}
					if (setSpecified == null)
					{
						break;
					}
				}
				this.x50ad0b40c9580760 = getSpecified;
				this.xede49acc9159386f = setSpecified;
				if (!false)
				{
					return;
				}
			}
			throw new InvalidOperationException();
		}

		public override void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			if (this.x50ad0b40c9580760 == null || (bool)this.x50ad0b40c9580760.Invoke(xbcea506a33cf9111, null))
			{
				this.x97cebb6cd312a01b.x6210059f049f0d48(xbcea506a33cf9111, x6b8e154b42d5c1e3);
			}
		}

		public override object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			object result = this.x97cebb6cd312a01b.x06b0e25aa6ad68a9(xbcea506a33cf9111, x337e217cb3ba0627);
			if (this.xede49acc9159386f != null)
			{
				this.xede49acc9159386f.Invoke(xbcea506a33cf9111, new object[]
				{
					true
				});
			}
			return result;
		}

		private readonly MethodInfo x50ad0b40c9580760;

		private readonly MethodInfo xede49acc9159386f;
	}
}
