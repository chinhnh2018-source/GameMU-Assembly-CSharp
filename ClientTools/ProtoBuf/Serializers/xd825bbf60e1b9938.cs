using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class xd825bbf60e1b9938 : x66ec8c25e4c7547d, x9da713b4847e9e6e
	{
		bool x9da713b4847e9e6e.xc3239c74e4ed7078(TypeModel.CallbackType xee5efdfe033701c1)
		{
			return ((x9da713b4847e9e6e)this.x096afe8c24e93a4b.x9e41724c73da1842).xf5cdd81490e1c274(xee5efdfe033701c1);
		}

		void x9da713b4847e9e6e.x96a100ebafadb7f4(object xbcea506a33cf9111, TypeModel.CallbackType xee5efdfe033701c1, SerializationContext x0f7b23d1c393aed9)
		{
			((x9da713b4847e9e6e)this.x096afe8c24e93a4b.x9e41724c73da1842).x4e4178637618473f(xbcea506a33cf9111, xee5efdfe033701c1, x0f7b23d1c393aed9);
		}

		public xd825bbf60e1b9938(Type type, int key, xf359d5c298ca874d proxy, bool recursionCheck)
		{
			if (2 != 0 && type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (proxy != null)
			{
				this.x43163d22e8cd5a71 = type;
				this.x096afe8c24e93a4b = proxy;
				this.xba08ce632055a1d9 = key;
				this.x6a6daa33fa413007 = recursionCheck;
				return;
			}
			throw new ArgumentNullException("proxy");
		}

		Type x66ec8c25e4c7547d.x2161712d594df31a
		{
			get
			{
				return this.x43163d22e8cd5a71;
			}
		}

		bool x66ec8c25e4c7547d.x84790f641eeacdd2
		{
			get
			{
				return true;
			}
		}

		bool x66ec8c25e4c7547d.x60089ae1c8e627b2
		{
			get
			{
				return true;
			}
		}

		void x66ec8c25e4c7547d.x5f78e8b4160336fb(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			if (this.x6a6daa33fa413007)
			{
				ProtoWriter.WriteObject(xbcea506a33cf9111, this.xba08ce632055a1d9, x6b8e154b42d5c1e3);
				return;
			}
			ProtoWriter.WriteRecursionSafeObject(xbcea506a33cf9111, this.xba08ce632055a1d9, x6b8e154b42d5c1e3);
		}

		object x66ec8c25e4c7547d.xa436996888e4319a(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			return ProtoReader.ReadObject(xbcea506a33cf9111, this.xba08ce632055a1d9, x337e217cb3ba0627);
		}

		private readonly int xba08ce632055a1d9;

		private readonly Type x43163d22e8cd5a71;

		private readonly xf359d5c298ca874d x096afe8c24e93a4b;

		private readonly bool x6a6daa33fa413007;
	}
}
