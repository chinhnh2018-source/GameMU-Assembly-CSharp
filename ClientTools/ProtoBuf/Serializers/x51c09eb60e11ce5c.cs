using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class x51c09eb60e11ce5c : x2b98a1e927f9de72
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

		public x51c09eb60e11ce5c(TypeModel model, object defaultValue, x66ec8c25e4c7547d tail) : base(tail)
		{
			while (defaultValue != null)
			{
				Type type = model.MapType(defaultValue.GetType());
				if (!false)
				{
					if (type == tail.x00c54479c3a7c440)
					{
						this.xc6e85c82d0d89508 = defaultValue;
						if (255 == 0)
						{
							continue;
						}
						return;
					}
				}
				throw new ArgumentException("Default value is of incorrect type", "defaultValue");
			}
			throw new ArgumentNullException("defaultValue");
		}

		public override void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			if (!object.Equals(xbcea506a33cf9111, this.xc6e85c82d0d89508))
			{
				this.x97cebb6cd312a01b.x6210059f049f0d48(xbcea506a33cf9111, x6b8e154b42d5c1e3);
			}
		}

		public override object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			return this.x97cebb6cd312a01b.x06b0e25aa6ad68a9(xbcea506a33cf9111, x337e217cb3ba0627);
		}

		private readonly object xc6e85c82d0d89508;
	}
}
