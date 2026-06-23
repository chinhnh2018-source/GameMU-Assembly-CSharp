using System;
using System.Collections;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class x72fd17eaff71827a : x2b98a1e927f9de72
	{
		public x72fd17eaff71827a(TypeModel model, x66ec8c25e4c7547d tail, int fieldNumber, bool writePacked, WireType packedWireType, Type arrayType, bool overwriteList, bool supportNull) : base(tail)
		{
			bool flag = (writePacked ? 1U : 0U) > uint.MaxValue;
			if (!flag)
			{
				goto IL_1C5;
			}
			if ((overwriteList ? 1U : 0U) - (supportNull ? 1U : 0U) <= 4294967295U)
			{
				goto IL_185;
			}
			for (;;)
			{
				IL_CD:
				if (xdeb0ec0887c9560b.xe0bc7b4a174cc0ae(packedWireType))
				{
					goto IL_D6;
				}
				if (writePacked)
				{
					goto IL_13A;
				}
				packedWireType = WireType.None;
				IL_10E:
				flag = ((writePacked ? 1U : 0U) + (overwriteList ? 1U : 0U) < 0U);
				if (flag)
				{
					if (((overwriteList ? 1U : 0U) & 0U) != 0U)
					{
						goto IL_13A;
					}
					continue;
				}
				IL_D6:
				this.xade3b695478596d6 = fieldNumber;
				this.xb9c62ffb3e96c0c8 = packedWireType;
				while (writePacked)
				{
					this.xdfde339da46db651 |= 1;
					flag = (((writePacked ? 1U : 0U) & 0U) == 0U);
					if (flag)
					{
						IL_57:
						if (!overwriteList)
						{
							if ((supportNull ? 1U : 0U) - (uint)fieldNumber > 4294967295U)
							{
								goto IL_1BD;
							}
						}
						else
						{
							this.xdfde339da46db651 |= 2;
						}
						if (supportNull)
						{
							goto IL_21;
						}
						flag = (((uint)fieldNumber & 0U) == 0U);
						if (flag)
						{
							goto IL_4D;
						}
						goto IL_10E;
					}
				}
				goto IL_57;
			}
			IL_21:
			this.xdfde339da46db651 |= 4;
			IL_4D:
			this.x92f461742516bc23 = arrayType;
			if (-2 == 0)
			{
				goto IL_149;
			}
			if (8 == 0)
			{
				goto IL_1C5;
			}
			return;
			IL_13A:
			throw new InvalidOperationException("Only simple data-types can use packed encoding");
			IL_149:
			throw new ArgumentOutOfRangeException("fieldNumber");
			IL_185:
			goto IL_18B;
			IL_187:
			if (!writePacked)
			{
				if (packedWireType == WireType.None)
				{
					goto IL_CD;
				}
			}
			IL_18B:
			if (fieldNumber <= 0)
			{
				goto IL_149;
			}
			goto IL_CD;
			IL_1BD:
			goto IL_187;
			IL_1C5:
			this.xd99217279677497c = arrayType.GetElementType();
			if (((writePacked ? 1U : 0U) & 0U) == 0U && supportNull)
			{
				goto IL_187;
			}
			x479f2661aae93792.xe5e08d1dc9f521de(this.xd99217279677497c);
			goto IL_1BD;
		}

		public override Type x00c54479c3a7c440
		{
			get
			{
				return this.x92f461742516bc23;
			}
		}

		public override bool x95726b5912481139
		{
			get
			{
				return this.x4a8d88e6d727acb6;
			}
		}

		public override bool xf33087968b28143c
		{
			get
			{
				return true;
			}
		}

		private bool x4a8d88e6d727acb6
		{
			get
			{
				return (this.xdfde339da46db651 & 2) == 0;
			}
		}

		private bool x2984e6683711bc23
		{
			get
			{
				return (this.xdfde339da46db651 & 4) != 0;
			}
		}

		public override void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			IList list = (IList)xbcea506a33cf9111;
			for (;;)
			{
				int count = list.Count;
				bool flag = (this.xdfde339da46db651 & 1) != 0;
				SubItemToken token;
				if (!flag)
				{
					token = default(SubItemToken);
					goto IL_92;
				}
				if ((flag ? 1U : 0U) <= 4294967295U)
				{
					ProtoWriter.WriteFieldHeader(this.xade3b695478596d6, WireType.String, x6b8e154b42d5c1e3);
					token = ProtoWriter.StartSubItem(xbcea506a33cf9111, x6b8e154b42d5c1e3);
					ProtoWriter.SetPackedField(this.xade3b695478596d6, x6b8e154b42d5c1e3);
					goto IL_92;
				}
				IL_2D:
				int num;
				object obj;
				if (num >= count)
				{
					if (!flag)
					{
						return;
					}
					ProtoWriter.EndSubItem(token, x6b8e154b42d5c1e3);
					if (((uint)num & 0U) == 0U)
					{
						return;
					}
					if (false)
					{
						goto IL_6E;
					}
					continue;
				}
				else
				{
					obj = list[num];
				}
				IL_44:
				bool flag2;
				if (flag2)
				{
					if (obj == null)
					{
						break;
					}
				}
				this.x97cebb6cd312a01b.x6210059f049f0d48(obj, x6b8e154b42d5c1e3);
				num++;
				goto IL_2D;
				IL_92:
				flag2 = !this.x2984e6683711bc23;
				num = 0;
				bool flag3 = (flag2 ? 1U : 0U) - (uint)count < 0U;
				if (flag3)
				{
					flag3 = ((flag2 ? 1U : 0U) - (uint)count < 0U);
					if (flag3)
					{
						return;
					}
					goto IL_44;
				}
				IL_6E:
				goto IL_2D;
			}
			throw new NullReferenceException();
		}

		public override object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			int fieldNumber = x337e217cb3ba0627.FieldNumber;
			x826e0336b5da6af5 x826e0336b5da6af;
			int num;
			Array array;
			for (;;)
			{
				x826e0336b5da6af = new x826e0336b5da6af5();
				if (this.xb9c62ffb3e96c0c8 != WireType.None)
				{
					if (x337e217cb3ba0627.WireType == WireType.String)
					{
						SubItemToken token = ProtoReader.StartSubItem(x337e217cb3ba0627);
						while (ProtoReader.HasSubValue(this.xb9c62ffb3e96c0c8, x337e217cb3ba0627))
						{
							x826e0336b5da6af.xd6b6ed77479ef68c(this.x97cebb6cd312a01b.x06b0e25aa6ad68a9(null, x337e217cb3ba0627));
						}
						ProtoReader.EndSubItem(token, x337e217cb3ba0627);
						goto IL_99;
					}
				}
				do
				{
					IL_E2:
					x826e0336b5da6af.xd6b6ed77479ef68c(this.x97cebb6cd312a01b.x06b0e25aa6ad68a9(null, x337e217cb3ba0627));
				}
				while (x337e217cb3ba0627.TryReadFieldHeader(fieldNumber));
				if ((uint)num + (uint)fieldNumber >= 0U)
				{
					goto IL_99;
				}
				goto IL_7C;
				IL_B2:
				int num2;
				num = num2;
				array = Array.CreateInstance(this.xd99217279677497c, num + x826e0336b5da6af.xd44988f225497f3a);
				if ((uint)fieldNumber > 4294967295U)
				{
					continue;
				}
				bool flag = (uint)fieldNumber + (uint)fieldNumber < 0U;
				if (flag)
				{
					goto IL_E2;
				}
				flag = ((uint)fieldNumber + (uint)num < 0U);
				if (flag)
				{
					break;
				}
				flag = ((uint)num + (uint)fieldNumber > uint.MaxValue);
				if (!flag)
				{
					goto IL_1C;
				}
				if (((uint)num | 4U) != 0U)
				{
					break;
				}
				goto IL_7C;
				IL_99:
				if (!this.x4a8d88e6d727acb6)
				{
					num2 = 0;
					goto IL_B2;
				}
				IL_A1:
				num2 = ((xbcea506a33cf9111 == null) ? 0 : ((Array)xbcea506a33cf9111).Length);
				goto IL_B2;
				IL_7C:
				if ((uint)fieldNumber + (uint)fieldNumber >= 0U)
				{
					goto IL_A1;
				}
				goto IL_1A;
			}
			IL_0C:
			((Array)xbcea506a33cf9111).CopyTo(array, 0);
			IL_1A:
			goto IL_1F;
			IL_1C:
			if (num != 0)
			{
				goto IL_0C;
			}
			IL_1F:
			x826e0336b5da6af.x0fe4f26e70030075(array, num);
			return array;
			goto IL_0C;
		}

		private const byte x0cb185874a68ca46 = 1;

		private const byte x007cff453cb0ea17 = 2;

		private const byte x75e63ef75c60cbc1 = 4;

		private readonly int xade3b695478596d6;

		private readonly byte xdfde339da46db651;

		private readonly WireType xb9c62ffb3e96c0c8;

		private readonly Type x92f461742516bc23;

		private readonly Type xd99217279677497c;
	}
}
