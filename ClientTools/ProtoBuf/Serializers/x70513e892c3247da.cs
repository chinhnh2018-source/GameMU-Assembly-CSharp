using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class x70513e892c3247da : x5c17ccb70f35d083
	{
		public x70513e892c3247da(TypeModel model) : base(model)
		{
		}

		public override Type x00c54479c3a7c440
		{
			get
			{
				return x70513e892c3247da.x1f8ba6f334d9c5af;
			}
		}

		public override void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			ProtoWriter.WriteUInt16((ushort)((char)xbcea506a33cf9111), x6b8e154b42d5c1e3);
		}

		public override object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			return (char)x337e217cb3ba0627.ReadUInt16();
		}

		private static readonly Type x1f8ba6f334d9c5af = typeof(char);
	}
}
