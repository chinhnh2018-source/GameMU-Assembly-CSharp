using System;
using System.ComponentModel;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
	public sealed class ProtoIncludeAttribute : Attribute
	{
		public ProtoIncludeAttribute(int tag, Type knownType) : this(tag, (knownType == null) ? "" : knownType.AssemblyQualifiedName)
		{
		}

		public ProtoIncludeAttribute(int tag, string knownTypeName)
		{
			IL_7D:
			while (tag > 0)
			{
				IL_0B:
				while (!x479f2661aae93792.x1c140bd1078ddda1(knownTypeName))
				{
					if (!false)
					{
					}
					for (;;)
					{
						this.tag = tag;
						this.knownTypeName = knownTypeName;
						bool flag = ((uint)tag | uint.MaxValue) == 0U;
						if (!flag)
						{
							break;
						}
						if (3 != 0)
						{
							goto IL_7D;
						}
						if ((uint)tag + (uint)tag < 0U)
						{
							break;
						}
						if ((uint)tag + (uint)tag > 4294967295U)
						{
							goto IL_0B;
						}
					}
					return;
				}
				throw new ArgumentNullException("knownTypeName", "Known type cannot be blank");
			}
			throw new ArgumentOutOfRangeException("tag", "Tags must be positive integers");
		}

		public int Tag
		{
			get
			{
				return this.tag;
			}
		}

		public string KnownTypeName
		{
			get
			{
				return this.knownTypeName;
			}
		}

		public Type KnownType
		{
			get
			{
				return TypeModel.xad1a6254e789134f(this.KnownTypeName, null, null);
			}
		}

		[DefaultValue(DataFormat.Default)]
		public DataFormat DataFormat
		{
			get
			{
				return this.dataFormat;
			}
			set
			{
				this.dataFormat = value;
			}
		}

		private readonly int tag;

		private readonly string knownTypeName;

		private DataFormat dataFormat;
	}
}
