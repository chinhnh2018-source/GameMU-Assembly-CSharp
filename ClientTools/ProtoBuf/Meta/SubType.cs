using System;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf.Serializers;

namespace ProtoBuf.Meta
{
	public sealed class SubType
	{
		public int FieldNumber
		{
			get
			{
				return this.xade3b695478596d6;
			}
		}

		public MetaType DerivedType
		{
			get
			{
				return this.x2736acb4b2bf54b0;
			}
		}

		public SubType(int fieldNumber, MetaType derivedType, DataFormat format)
		{
			IL_55:
			while (derivedType != null)
			{
				bool flag = (uint)fieldNumber < 0U;
				if (!flag)
				{
					while (fieldNumber > 0)
					{
						do
						{
							this.xade3b695478596d6 = fieldNumber;
							if (4 != 0)
							{
								goto Block_1;
							}
						}
						while (3 != 0);
						continue;
						Block_1:
						this.x2736acb4b2bf54b0 = derivedType;
						this.xb0fbb9918378a9ab = format;
						if (false)
						{
							goto IL_55;
						}
						return;
					}
				}
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			throw new ArgumentNullException("derivedType");
		}

		internal x66ec8c25e4c7547d x9e41724c73da1842
		{
			get
			{
				if (this.x38c907674383c2da == null)
				{
					this.x38c907674383c2da = this.x0864aaa2c6d0fbe0();
				}
				return this.x38c907674383c2da;
			}
		}

		private x66ec8c25e4c7547d x0864aaa2c6d0fbe0()
		{
			WireType wireType = WireType.String;
			if (this.xb0fbb9918378a9ab == DataFormat.Group)
			{
				wireType = WireType.StartGroup;
			}
			x66ec8c25e4c7547d tail = new xd825bbf60e1b9938(this.x2736acb4b2bf54b0.Type, this.x2736acb4b2bf54b0.xf15263674eb297bb(false, false), this.x2736acb4b2bf54b0, false);
			return new x8f2d2e2582fd9f3b(this.xade3b695478596d6, wireType, false, tail);
		}

		private readonly int xade3b695478596d6;

		private readonly MetaType x2736acb4b2bf54b0;

		private readonly DataFormat xb0fbb9918378a9ab;

		private x66ec8c25e4c7547d x38c907674383c2da;

		internal class xf7b8e510ae9ad738 : IComparer<SubType>, IComparer
		{
			public int Compare(object x, object y)
			{
				return this.Compare(x as SubType, y as SubType);
			}

			public int Compare(SubType x, SubType y)
			{
				if (!object.ReferenceEquals(x, y))
				{
					if (x != null)
					{
						if (-2 != 0)
						{
							if (y != null)
							{
								if (!false)
								{
								}
								int fieldNumber = x.FieldNumber;
								bool flag = (uint)fieldNumber + (uint)fieldNumber > uint.MaxValue;
								if (flag)
								{
									return 0;
								}
								return fieldNumber.CompareTo(y.FieldNumber);
							}
						}
						return 1;
					}
					return -1;
				}
				return 0;
			}

			public static readonly SubType.xf7b8e510ae9ad738 xb9715d2f06b63cf0 = new SubType.xf7b8e510ae9ad738();
		}
	}
}
