using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class x268cc84c68b7a350 : x66ec8c25e4c7547d
	{
		public Type x00c54479c3a7c440
		{
			get
			{
				return x268cc84c68b7a350.x1f8ba6f334d9c5af;
			}
		}

		public x268cc84c68b7a350(TypeModel model)
		{
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

		public void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			ProtoWriter.WriteByte((byte)xbcea506a33cf9111, x6b8e154b42d5c1e3);
		}

		public object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			return x337e217cb3ba0627.ReadByte();
		}

		private static readonly Type x1f8ba6f334d9c5af = typeof(byte);
	}
}
