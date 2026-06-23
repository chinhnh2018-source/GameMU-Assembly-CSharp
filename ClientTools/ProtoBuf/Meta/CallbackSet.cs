using System;
using System.Reflection;

namespace ProtoBuf.Meta
{
	[DefaultMember("Item")]
	public class CallbackSet
	{
		internal CallbackSet(MetaType metaType)
		{
			if (metaType != null && !false)
			{
				this.xb4f410bbdbde11cb = metaType;
				return;
			}
			throw new ArgumentNullException("metaType");
		}

		internal MethodInfo xe6d4b1b411ed94b5
		{
			get
			{
				if (!false)
				{
					switch (xee5efdfe033701c1)
					{
					case TypeModel.CallbackType.BeforeSerialize:
						return this.x993936bb28e2d7cf;
					case TypeModel.CallbackType.AfterSerialize:
						return this.x4be04acdab95cee0;
					case TypeModel.CallbackType.BeforeDeserialize:
						break;
					case TypeModel.CallbackType.AfterDeserialize:
						return this.xad99bdad20150aa8;
					default:
						if (!false)
						{
						}
						throw new ArgumentException();
					}
				}
				return this.xfd62f6a32342e0e6;
			}
		}

		internal static bool x0134e67247d44030(TypeModel xad70a5849826ecef, MethodInfo x1306445c04667cc7)
		{
			ParameterInfo[] parameters = x1306445c04667cc7.GetParameters();
			return parameters.Length == 0 || (parameters.Length == 1 && parameters[0].ParameterType == xad70a5849826ecef.MapType(typeof(SerializationContext)));
		}

		private MethodInfo xf7d7ca7fbaa74c74(TypeModel xad70a5849826ecef, MethodInfo x7d1bf994956f1081)
		{
			this.xb4f410bbdbde11cb.ThrowIfFrozen();
			if (-1 != 0)
			{
				IL_70:
				while (x7d1bf994956f1081 != null)
				{
					while (!x7d1bf994956f1081.IsStatic)
					{
						if (-1 == 0)
						{
							if (3 == 0)
							{
								continue;
							}
						}
						else if (x7d1bf994956f1081.ReturnType != xad70a5849826ecef.MapType(typeof(void)))
						{
							goto IL_10;
						}
						if (CallbackSet.x0134e67247d44030(xad70a5849826ecef, x7d1bf994956f1081))
						{
							if (-2 == 0)
							{
								goto IL_70;
							}
							return x7d1bf994956f1081;
						}
						IL_10:
						throw CallbackSet.x1922373fb3058bb0(x7d1bf994956f1081);
					}
					throw new ArgumentException("Callbacks cannot be static", "callback");
				}
			}
			return x7d1bf994956f1081;
		}

		internal static Exception x1922373fb3058bb0(MethodInfo x1306445c04667cc7)
		{
			return new NotSupportedException("Invalid callback signature in " + x1306445c04667cc7.DeclaringType.FullName + "." + x1306445c04667cc7.Name);
		}

		public MethodInfo BeforeSerialize
		{
			get
			{
				return this.x993936bb28e2d7cf;
			}
			set
			{
				this.x993936bb28e2d7cf = this.xf7d7ca7fbaa74c74(this.xb4f410bbdbde11cb.xfd195ba400a3473c, value);
			}
		}

		public MethodInfo BeforeDeserialize
		{
			get
			{
				return this.xfd62f6a32342e0e6;
			}
			set
			{
				this.xfd62f6a32342e0e6 = this.xf7d7ca7fbaa74c74(this.xb4f410bbdbde11cb.xfd195ba400a3473c, value);
			}
		}

		public MethodInfo AfterSerialize
		{
			get
			{
				return this.x4be04acdab95cee0;
			}
			set
			{
				this.x4be04acdab95cee0 = this.xf7d7ca7fbaa74c74(this.xb4f410bbdbde11cb.xfd195ba400a3473c, value);
			}
		}

		public MethodInfo AfterDeserialize
		{
			get
			{
				return this.xad99bdad20150aa8;
			}
			set
			{
				this.xad99bdad20150aa8 = this.xf7d7ca7fbaa74c74(this.xb4f410bbdbde11cb.xfd195ba400a3473c, value);
			}
		}

		public bool NonTrivial
		{
			get
			{
				if (this.x993936bb28e2d7cf == null)
				{
					if (false)
					{
						if (8 != 0)
						{
						}
					}
					else if (this.xfd62f6a32342e0e6 != null)
					{
						return true;
					}
					if (this.x4be04acdab95cee0 == null)
					{
						return this.xad99bdad20150aa8 != null;
					}
				}
				return true;
			}
		}

		private readonly MetaType xb4f410bbdbde11cb;

		private MethodInfo x993936bb28e2d7cf;

		private MethodInfo x4be04acdab95cee0;

		private MethodInfo xfd62f6a32342e0e6;

		private MethodInfo xad99bdad20150aa8;
	}
}
