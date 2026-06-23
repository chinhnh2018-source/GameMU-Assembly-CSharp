using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class xdae1073def4c17e2 : x2b98a1e927f9de72
	{
		public xdae1073def4c17e2(TypeModel model, x66ec8c25e4c7547d tail) : base(tail)
		{
			if (!tail.xf33087968b28143c)
			{
				throw new NotSupportedException("NullDecorator only supports implementations that return values");
			}
			if (!x479f2661aae93792.x25f40ad8c018c1ab(tail.x00c54479c3a7c440))
			{
				this.x1f8ba6f334d9c5af = tail.x00c54479c3a7c440;
				return;
			}
			this.x1f8ba6f334d9c5af = model.MapType(typeof(Nullable<>)).MakeGenericType(new Type[]
			{
				tail.x00c54479c3a7c440
			});
		}

		public override Type x00c54479c3a7c440
		{
			get
			{
				return this.x1f8ba6f334d9c5af;
			}
		}

		public override bool xf33087968b28143c
		{
			get
			{
				return true;
			}
		}

		public override bool x95726b5912481139
		{
			get
			{
				return true;
			}
		}

		public override object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			SubItemToken token = ProtoReader.StartSubItem(x337e217cb3ba0627);
			int num;
			if ((uint)num > 4294967295U)
			{
				goto IL_11;
			}
			IL_17:
			while ((num = x337e217cb3ba0627.ReadFieldHeader()) > 0)
			{
				if (num != 1)
				{
					goto IL_11;
				}
				xbcea506a33cf9111 = this.x97cebb6cd312a01b.x06b0e25aa6ad68a9(xbcea506a33cf9111, x337e217cb3ba0627);
			}
			ProtoReader.EndSubItem(token, x337e217cb3ba0627);
			return xbcea506a33cf9111;
			IL_11:
			x337e217cb3ba0627.SkipField();
			goto IL_17;
		}

		public override void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			SubItemToken token = ProtoWriter.StartSubItem(null, x6b8e154b42d5c1e3);
			if (xbcea506a33cf9111 != null)
			{
				this.x97cebb6cd312a01b.x6210059f049f0d48(xbcea506a33cf9111, x6b8e154b42d5c1e3);
			}
			ProtoWriter.EndSubItem(token, x6b8e154b42d5c1e3);
		}

		public const int xd229d86af0f16fb0 = 1;

		private readonly Type x1f8ba6f334d9c5af;
	}
}
