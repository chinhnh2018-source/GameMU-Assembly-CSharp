using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class x2d308a71a493dcae : x66ec8c25e4c7547d
	{
		public x2d308a71a493dcae(TypeModel model, Type type, int key, BclHelpers.NetObjectOptions options)
		{
			bool flag = (byte)(options & BclHelpers.NetObjectOptions.DynamicType) != 0;
			this.xba08ce632055a1d9 = (flag ? -1 : key);
			this.x43163d22e8cd5a71 = (flag ? model.MapType(typeof(object)) : type);
			this.xdfde339da46db651 = options;
		}

		public Type x00c54479c3a7c440
		{
			get
			{
				return this.x43163d22e8cd5a71;
			}
		}

		public bool xf33087968b28143c
		{
			get
			{
				return true;
			}
		}

		public bool x95726b5912481139
		{
			get
			{
				return true;
			}
		}

		public object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			return BclHelpers.ReadNetObject(xbcea506a33cf9111, x337e217cb3ba0627, this.xba08ce632055a1d9, (this.x43163d22e8cd5a71 == typeof(object)) ? null : this.x43163d22e8cd5a71, this.xdfde339da46db651);
		}

		public void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			BclHelpers.WriteNetObject(xbcea506a33cf9111, x6b8e154b42d5c1e3, this.xba08ce632055a1d9, this.xdfde339da46db651);
		}

		private readonly int xba08ce632055a1d9;

		private readonly Type x43163d22e8cd5a71;

		private readonly BclHelpers.NetObjectOptions xdfde339da46db651;
	}
}
