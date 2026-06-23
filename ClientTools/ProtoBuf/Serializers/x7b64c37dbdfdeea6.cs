using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class x7b64c37dbdfdeea6 : x2b98a1e927f9de72
	{
		public x7b64c37dbdfdeea6(TypeModel model, x66ec8c25e4c7547d tail) : base(tail)
		{
		}

		public override Type x00c54479c3a7c440
		{
			get
			{
				return x7b64c37dbdfdeea6.x1f8ba6f334d9c5af;
			}
		}

		public override bool x95726b5912481139
		{
			get
			{
				return false;
			}
		}

		public override bool xf33087968b28143c
		{
			get
			{
				return true;
			}
		}

		public override void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			this.x97cebb6cd312a01b.x6210059f049f0d48(((Uri)xbcea506a33cf9111).AbsoluteUri, x6b8e154b42d5c1e3);
		}

		public override object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			string text = (string)this.x97cebb6cd312a01b.x06b0e25aa6ad68a9(null, x337e217cb3ba0627);
			if (text.Length == 0)
			{
				return null;
			}
			return new Uri(text);
		}

		private static readonly Type x1f8ba6f334d9c5af = typeof(Uri);
	}
}
