using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class xb532c08b9899b08a : x66ec8c25e4c7547d
	{
		public Type x00c54479c3a7c440
		{
			get
			{
				return xb532c08b9899b08a.x1f8ba6f334d9c5af;
			}
		}

		bool x66ec8c25e4c7547d.x84790f641eeacdd2
		{
			get
			{
				return false;
			}
		}

		bool x66ec8c25e4c7547d.x60089ae1c8e627b2
		{
			get
			{
				return true;
			}
		}

		public xb532c08b9899b08a(TypeModel model)
		{
		}

		public object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			return BclHelpers.ReadDateTime(x337e217cb3ba0627);
		}

		public void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			BclHelpers.WriteDateTime((DateTime)xbcea506a33cf9111, x6b8e154b42d5c1e3);
		}

		private static readonly Type x1f8ba6f334d9c5af = typeof(DateTime);
	}
}
