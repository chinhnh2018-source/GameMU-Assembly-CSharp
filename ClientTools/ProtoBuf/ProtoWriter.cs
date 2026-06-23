using System;
using System.IO;
using System.Text;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	public sealed class ProtoWriter : IDisposable
	{
		public static void WriteObject(object value, int key, ProtoWriter writer)
		{
			SubItemToken token;
			if (writer.xad70a5849826ecef != null)
			{
				token = ProtoWriter.StartSubItem(value, writer);
				while (key < 0)
				{
					if (writer.xad70a5849826ecef == null)
					{
						goto IL_23;
					}
					if (writer.xad70a5849826ecef.x07feef0c759efbcc(writer, value.GetType(), DataFormat.Default, 1, value, false))
					{
						goto IL_2E;
					}
					if ((uint)key + (uint)key >= 0U)
					{
						if (!false)
						{
							goto IL_23;
						}
						if (2 != 0)
						{
							goto IL_85;
						}
					}
				}
				writer.xad70a5849826ecef.Serialize(key, value, writer);
				goto IL_2E;
			}
			bool flag = (uint)key - (uint)key > uint.MaxValue;
			if (!flag)
			{
				goto IL_85;
			}
			IL_23:
			TypeModel.ThrowUnexpectedType(value.GetType());
			IL_2E:
			ProtoWriter.EndSubItem(token, writer);
			if (!false)
			{
				return;
			}
			IL_85:
			throw new InvalidOperationException("Cannot serialize sub-objects unless a model is provided");
		}

		public static void WriteRecursionSafeObject(object value, int key, ProtoWriter writer)
		{
			if (writer.xad70a5849826ecef == null)
			{
				throw new InvalidOperationException("Cannot serialize sub-objects unless a model is provided");
			}
			SubItemToken token = ProtoWriter.StartSubItem(null, writer);
			writer.xad70a5849826ecef.Serialize(key, value, writer);
			ProtoWriter.EndSubItem(token, writer);
		}

		internal static void x9f4d284c7b43f8f0(object xbcea506a33cf9111, int xba08ce632055a1d9, ProtoWriter xbdfb620b7167944b, PrefixStyle x44ecfea61c937b8e, int xade3b695478596d6)
		{
			if (xbdfb620b7167944b.xad70a5849826ecef == null)
			{
				bool flag = (uint)xba08ce632055a1d9 - (uint)xade3b695478596d6 > uint.MaxValue;
				if (flag)
				{
					goto IL_80;
				}
				goto IL_161;
			}
			else
			{
				if (xbdfb620b7167944b.xa5694e1c82a939b4 != WireType.None)
				{
					throw ProtoWriter.x98bdea680591defc(xbdfb620b7167944b);
				}
				if (4 != 0)
				{
					bool flag = ((uint)xade3b695478596d6 & 0U) == 0U;
					if (!flag)
					{
						goto IL_6F;
					}
				}
				if (3 == 0)
				{
					goto IL_18E;
				}
				goto IL_130;
			}
			IL_28:
			SubItemToken x153c99a;
			ProtoWriter.x9f5fbae64ba8f333(x153c99a, xbdfb620b7167944b, x44ecfea61c937b8e);
			return;
			IL_4B:
			x153c99a = ProtoWriter.xbb0d19cdb45a19dd(xbcea506a33cf9111, xbdfb620b7167944b, true);
			if (!false && xba08ce632055a1d9 >= 0)
			{
				xbdfb620b7167944b.xad70a5849826ecef.Serialize(xba08ce632055a1d9, xbcea506a33cf9111, xbdfb620b7167944b);
				goto IL_A2;
			}
			if (xbdfb620b7167944b.xad70a5849826ecef.x07feef0c759efbcc(xbdfb620b7167944b, xbcea506a33cf9111.GetType(), DataFormat.Default, 1, xbcea506a33cf9111, false))
			{
				goto IL_28;
			}
			IL_6F:
			TypeModel.ThrowUnexpectedType(xbcea506a33cf9111.GetType());
			goto IL_28;
			IL_80:
			if ((uint)xba08ce632055a1d9 - (uint)xade3b695478596d6 < 0U)
			{
				goto IL_130;
			}
			if (3 != 0)
			{
				goto IL_4B;
			}
			IL_A2:
			goto IL_28;
			IL_130:
			if (((uint)xba08ce632055a1d9 & 0U) == 0U)
			{
				goto IL_18E;
			}
			IL_161:
			throw new InvalidOperationException("Cannot serialize sub-objects unless a model is provided");
			IL_18E:
			switch (x44ecfea61c937b8e)
			{
			case PrefixStyle.Base128:
				xbdfb620b7167944b.xa5694e1c82a939b4 = WireType.String;
				xbdfb620b7167944b.xade3b695478596d6 = xade3b695478596d6;
				if (((uint)xba08ce632055a1d9 | 3U) != 0U)
				{
					if (8 != 0 && xade3b695478596d6 <= 0)
					{
						goto IL_80;
					}
					ProtoWriter.xd21f404d2945236a(xade3b695478596d6, WireType.String, xbdfb620b7167944b);
				}
				goto IL_4B;
			case PrefixStyle.Fixed32:
			case PrefixStyle.Fixed32BigEndian:
				xbdfb620b7167944b.xade3b695478596d6 = 0;
				xbdfb620b7167944b.xa5694e1c82a939b4 = WireType.Fixed32;
				goto IL_4B;
			default:
				throw new ArgumentOutOfRangeException("style");
			}
		}

		internal int x987c197a20bb9886(ref Type x43163d22e8cd5a71)
		{
			return this.xad70a5849826ecef.GetKey(ref x43163d22e8cd5a71);
		}

		internal x15bd90f59150b4e2 x4406e28dba6f6c8e
		{
			get
			{
				return this.x49b766214fe3dda0;
			}
		}

		internal WireType x58ca9db3d85c761f
		{
			get
			{
				return this.xa5694e1c82a939b4;
			}
		}

		public static void WriteFieldHeader(int fieldNumber, WireType wireType, ProtoWriter writer)
		{
			if (writer.xa5694e1c82a939b4 == WireType.None)
			{
				if (fieldNumber >= 0)
				{
					bool flag = (uint)fieldNumber + (uint)fieldNumber < 0U;
					object[] array;
					if (!flag)
					{
						flag = ((uint)fieldNumber - (uint)fieldNumber < 0U);
						if (!flag && writer.x2f0158cd84d79a16 != 0)
						{
							if (false)
							{
								goto IL_117;
							}
							if (writer.x2f0158cd84d79a16 == fieldNumber)
							{
								goto IL_F4;
							}
							array = new object[4];
							array[0] = "Field mismatch during packed encoding; expected ";
							array[1] = writer.x2f0158cd84d79a16;
							if (((uint)fieldNumber & 0U) != 0U)
							{
								goto IL_D5;
							}
							if (false)
							{
								goto IL_E4;
							}
							array[2] = " but received ";
							IL_19:
							array[3] = fieldNumber;
							if ((uint)fieldNumber + (uint)fieldNumber >= 0U)
							{
								goto IL_E4;
							}
							goto IL_1D4;
							IL_75:
							writer.xade3b695478596d6 = fieldNumber;
							if (!false)
							{
								writer.xa5694e1c82a939b4 = wireType;
								return;
							}
							goto IL_19;
							IL_D5:
							if (false)
							{
								goto IL_F4;
							}
							if (wireType != WireType.SignedVariant)
							{
								throw new InvalidOperationException("Wire-type cannot be encoded as packed: " + wireType);
							}
							goto IL_75;
							IL_E4:
							goto IL_193;
							IL_F4:
							switch (wireType)
							{
							case WireType.Variant:
							case WireType.Fixed64:
								goto IL_75;
							default:
								if (wireType == WireType.Fixed32)
								{
									goto IL_75;
								}
								break;
							}
							IL_117:
							goto IL_D5;
						}
						writer.xade3b695478596d6 = fieldNumber;
						writer.xa5694e1c82a939b4 = wireType;
						ProtoWriter.xd21f404d2945236a(fieldNumber, wireType, writer);
						flag = ((uint)fieldNumber + (uint)fieldNumber > uint.MaxValue);
						if (flag)
						{
							goto IL_164;
						}
					}
					if (!false)
					{
						return;
					}
					IL_193:
					throw new InvalidOperationException(string.Concat(array));
				}
				IL_164:
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			IL_1D4:
			throw new InvalidOperationException(string.Concat(new object[]
			{
				"Cannot write a ",
				wireType,
				" header until the ",
				writer.xa5694e1c82a939b4,
				" data has been written"
			}));
		}

		internal static void xd21f404d2945236a(int xade3b695478596d6, WireType xa5694e1c82a939b4, ProtoWriter xbdfb620b7167944b)
		{
			uint xbcea506a33cf = (uint)(xade3b695478596d6 << 3 | (int)(xa5694e1c82a939b4 & (WireType)7));
			ProtoWriter.x608d56e5eb60d6f7(xbcea506a33cf, xbdfb620b7167944b);
		}

		public static void WriteBytes(byte[] data, ProtoWriter writer)
		{
			ProtoWriter.WriteBytes(data, 0, data.Length, writer);
		}

		public static void WriteBytes(byte[] data, int offset, int length, ProtoWriter writer)
		{
			if (data != null)
			{
				WireType wireType = writer.xa5694e1c82a939b4;
				if (-1 != 0)
				{
					goto IL_1AE;
				}
				goto IL_43;
				IL_0B:
				IL_17:
				ProtoWriter.xb0749858b4a5e375(length, writer);
				x479f2661aae93792.x6a87193e5bb23362(data, offset, writer.x8b3e45f7763ede93, writer.x8d48119452c041bd, length);
				ProtoWriter.x64cb5f5a88b3094c(length, writer);
				bool flag = (uint)length > uint.MaxValue;
				if (!flag)
				{
					return;
				}
				flag = ((uint)offset < 0U);
				if (!flag)
				{
					goto IL_192;
				}
				if ((uint)offset - (uint)offset > 4294967295U)
				{
					goto IL_0B;
				}
				if ((uint)length + (uint)length >= 0U)
				{
					goto IL_95;
				}
				goto IL_1AE;
				IL_43:
				if (length <= writer.x8b3e45f7763ede93.Length)
				{
					goto IL_0B;
				}
				ProtoWriter.xbb7550bbb62a218c(writer);
				writer.x6b8e154b42d5c1e3.Write(data, offset, length);
				writer.x13d4cb8d1bd20347 += length;
				if ((uint)offset >= 0U)
				{
					return;
				}
				return;
				IL_6D:
				if (length == 8)
				{
					goto IL_17;
				}
				throw new ArgumentException("length");
				IL_95:
				if (length != 4)
				{
					throw new ArgumentException("length");
				}
				flag = ((uint)length + (uint)offset < 0U);
				if (flag)
				{
					goto IL_6D;
				}
				goto IL_17;
				IL_1AE:
				switch (wireType)
				{
				case WireType.Fixed64:
					goto IL_6D;
				case WireType.String:
					ProtoWriter.x608d56e5eb60d6f7((uint)length, writer);
					writer.xa5694e1c82a939b4 = WireType.None;
					if (length == 0)
					{
						return;
					}
					if (writer.x730ca8c251524854 == 0)
					{
						goto IL_43;
					}
					goto IL_17;
				case WireType.Fixed32:
					goto IL_95;
				}
				throw ProtoWriter.x98bdea680591defc(writer);
			}
			IL_192:
			throw new ArgumentNullException("blob");
		}

		private static void xbc317e2a1e6559c0(Stream x337e217cb3ba0627, ProtoWriter xbdfb620b7167944b)
		{
			byte[] array = xbdfb620b7167944b.x8b3e45f7763ede93;
			int num2;
			for (;;)
			{
				int num = array.Length - xbdfb620b7167944b.x8d48119452c041bd;
				num2 = 1;
				for (;;)
				{
					IL_C6:
					bool flag;
					if (num > 0)
					{
						while ((num2 = x337e217cb3ba0627.Read(array, xbdfb620b7167944b.x8d48119452c041bd, num)) > 0)
						{
							do
							{
								xbdfb620b7167944b.x8d48119452c041bd += num2;
							}
							while (((uint)num | 255U) == 0U);
							xbdfb620b7167944b.x13d4cb8d1bd20347 += num2;
							num -= num2;
							flag = (((uint)num2 | 255U) == 0U);
							if (flag)
							{
								goto IL_C6;
							}
							if ((uint)num2 - (uint)num < 0U)
							{
								return;
							}
							if (255 != 0)
							{
								goto IL_C6;
							}
						}
					}
					if (num2 <= 0)
					{
						return;
					}
					flag = ((uint)num - (uint)num2 > uint.MaxValue);
					if (flag)
					{
						goto Block_8;
					}
					while (xbdfb620b7167944b.x730ca8c251524854 != 0)
					{
						for (;;)
						{
							ProtoWriter.xb0749858b4a5e375(128, xbdfb620b7167944b);
							if ((num2 = x337e217cb3ba0627.Read(xbdfb620b7167944b.x8b3e45f7763ede93, xbdfb620b7167944b.x8d48119452c041bd, xbdfb620b7167944b.x8b3e45f7763ede93.Length - xbdfb620b7167944b.x8d48119452c041bd)) <= 0)
							{
								return;
							}
							xbdfb620b7167944b.x13d4cb8d1bd20347 += num2;
							xbdfb620b7167944b.x8d48119452c041bd += num2;
							flag = (((uint)num | 3U) == 0U);
							if (flag)
							{
								break;
							}
							if (false)
							{
								goto Block_9;
							}
						}
						if (!false)
						{
							goto IL_C6;
						}
					}
					goto Block_4;
				}
				Block_9:;
			}
			return;
			Block_4:
			ProtoWriter.xbb7550bbb62a218c(xbdfb620b7167944b);
			while ((num2 = x337e217cb3ba0627.Read(array, 0, array.Length)) > 0)
			{
				xbdfb620b7167944b.x6b8e154b42d5c1e3.Write(array, 0, num2);
				xbdfb620b7167944b.x13d4cb8d1bd20347 += num2;
			}
			return;
			Block_8:;
		}

		private static void x64cb5f5a88b3094c(int x961016a387451f05, ProtoWriter xbdfb620b7167944b)
		{
			xbdfb620b7167944b.x8d48119452c041bd += x961016a387451f05;
			xbdfb620b7167944b.x13d4cb8d1bd20347 += x961016a387451f05;
			xbdfb620b7167944b.xa5694e1c82a939b4 = WireType.None;
		}

		public static SubItemToken StartSubItem(object instance, ProtoWriter writer)
		{
			return ProtoWriter.xbb0d19cdb45a19dd(instance, writer, false);
		}

		private void x95b8e8dc281b1d02(object x6ed4ed9ed59eb694)
		{
			int num;
			if (this.x4fc90296feee4ebf == null)
			{
				this.x4fc90296feee4ebf = new xd4dab911626dd004();
			}
			else if (x6ed4ed9ed59eb694 != null && !false && (num = this.x4fc90296feee4ebf.x13c79e2766d7a14b(x6ed4ed9ed59eb694)) >= 0)
			{
				throw new ProtoException("Possible recursion detected (offset: " + (this.x4fc90296feee4ebf.xd44988f225497f3a - num).ToString() + " level(s)): " + x6ed4ed9ed59eb694.ToString());
			}
			this.x4fc90296feee4ebf.xd6b6ed77479ef68c(x6ed4ed9ed59eb694);
		}

		private void xc89e26ca79c0c01c()
		{
			this.x4fc90296feee4ebf.xe91e34fc69f2f721();
		}

		private static SubItemToken xbb0d19cdb45a19dd(object x6ed4ed9ed59eb694, ProtoWriter xbdfb620b7167944b, bool xc9ca98a1dc92bb17)
		{
			int num = ++xbdfb620b7167944b.x1af9fc7847b7aa2c;
			int num2;
			bool flag;
			if (!false)
			{
				if (num > 25)
				{
					xbdfb620b7167944b.x95b8e8dc281b1d02(x6ed4ed9ed59eb694);
					if ((uint)num2 + (uint)num2 < 0U)
					{
						goto IL_7F;
					}
					if ((uint)num + (xc9ca98a1dc92bb17 ? 1U : 0U) < 0U)
					{
						goto IL_B1;
					}
					flag = ((xc9ca98a1dc92bb17 ? 1U : 0U) + (uint)num > uint.MaxValue);
					if (flag)
					{
						goto IL_180;
					}
				}
				if (xbdfb620b7167944b.x2f0158cd84d79a16 == 0)
				{
					goto IL_DC;
				}
				IL_180:
				throw new InvalidOperationException("Cannot begin a sub-item while performing packed encoding");
			}
			if ((uint)num - (uint)num2 >= 0U)
			{
				goto IL_DC;
			}
			goto IL_B1;
			IL_7F:
			xbdfb620b7167944b.x13d4cb8d1bd20347++;
			flag = ((xc9ca98a1dc92bb17 ? 1U : 0U) - (xc9ca98a1dc92bb17 ? 1U : 0U) < 0U);
			if (!flag)
			{
				num2 = xbdfb620b7167944b.x8d48119452c041bd++;
				return new SubItemToken(num2);
			}
			IL_A5:
			goto IL_190;
			IL_B1:
			return new SubItemToken(-xbdfb620b7167944b.xade3b695478596d6);
			IL_DC:
			switch (xbdfb620b7167944b.xa5694e1c82a939b4)
			{
			case WireType.String:
				xbdfb620b7167944b.xa5694e1c82a939b4 = WireType.None;
				ProtoWriter.xb0749858b4a5e375(32, xbdfb620b7167944b);
				xbdfb620b7167944b.x730ca8c251524854++;
				goto IL_7F;
			case WireType.StartGroup:
				xbdfb620b7167944b.xa5694e1c82a939b4 = WireType.None;
				goto IL_B1;
			case WireType.EndGroup:
				IL_190:
				throw ProtoWriter.x98bdea680591defc(xbdfb620b7167944b);
			case WireType.Fixed32:
				if (xc9ca98a1dc92bb17)
				{
					ProtoWriter.xb0749858b4a5e375(32, xbdfb620b7167944b);
					xbdfb620b7167944b.x730ca8c251524854++;
					SubItemToken result = new SubItemToken(xbdfb620b7167944b.x8d48119452c041bd);
					ProtoWriter.x64cb5f5a88b3094c(4, xbdfb620b7167944b);
					return result;
				}
				throw ProtoWriter.x98bdea680591defc(xbdfb620b7167944b);
			default:
				goto IL_A5;
			}
		}

		public static void EndSubItem(SubItemToken token, ProtoWriter writer)
		{
			ProtoWriter.x9f5fbae64ba8f333(token, writer, PrefixStyle.Base128);
		}

		private static void x9f5fbae64ba8f333(SubItemToken x153c99a852375422, ProtoWriter xbdfb620b7167944b, PrefixStyle x44ecfea61c937b8e)
		{
			if (xbdfb620b7167944b.xa5694e1c82a939b4 == WireType.None)
			{
				goto IL_429;
			}
			int num;
			bool flag = (uint)num > uint.MaxValue;
			if (!flag)
			{
				throw ProtoWriter.x98bdea680591defc(xbdfb620b7167944b);
			}
			IL_3AE:
			xbdfb620b7167944b.xc89e26ca79c0c01c();
			IL_3BC:
			xbdfb620b7167944b.x2f0158cd84d79a16 = 0;
			int num2;
			flag = ((uint)num2 > uint.MaxValue);
			if (flag)
			{
				goto IL_3D8;
			}
			goto IL_35C;
			byte[] array;
			int xbcea506a33cf;
			uint num3;
			byte b;
			do
			{
				IL_A1:
				array[xbcea506a33cf++] = (byte)((num3 & 127U) | 128U);
				if (((uint)b | 2147483647U) == 0U)
				{
					goto IL_429;
				}
			}
			while ((num3 >>= 7) != 0U);
			array[xbcea506a33cf - 1] = (byte)((int)array[xbcea506a33cf - 1] & -129);
			xbdfb620b7167944b.x13d4cb8d1bd20347 += num;
			xbdfb620b7167944b.x8d48119452c041bd += num;
			flag = ((uint)num - (uint)num2 < 0U);
			if (flag)
			{
				goto IL_3AE;
			}
			goto IL_26;
			int num4;
			for (;;)
			{
				IL_2BF:
				if (!false)
				{
					switch (x44ecfea61c937b8e)
					{
					case PrefixStyle.Base128:
						num4 = xbdfb620b7167944b.x8d48119452c041bd - xbcea506a33cf - 1;
						num = 0;
						num3 = (uint)num4;
						goto IL_16A;
					case PrefixStyle.Fixed32:
						goto IL_282;
					case PrefixStyle.Fixed32BigEndian:
						goto IL_29F;
					}
					goto Block_16;
				}
				IL_16A:
				while ((num3 >>= 7) != 0U)
				{
					num++;
					if (num3 + (uint)b > 4294967295U)
					{
						goto IL_122;
					}
				}
				if (((uint)xbcea506a33cf | 4294967295U) == 0U)
				{
					goto IL_26;
				}
				if (num != 0)
				{
					goto IL_155;
				}
				if ((uint)b <= 4294967295U)
				{
					break;
				}
				if ((uint)num2 - num3 > 4294967295U)
				{
					goto IL_35C;
				}
				flag = ((uint)b > uint.MaxValue);
				if (flag)
				{
					goto Block_19;
				}
			}
			xbdfb620b7167944b.x8b3e45f7763ede93[xbcea506a33cf] = (byte)(num4 & 127);
			goto IL_4A;
			IL_155:
			ProtoWriter.xb0749858b4a5e375(num, xbdfb620b7167944b);
			array = xbdfb620b7167944b.x8b3e45f7763ede93;
			goto IL_10F;
			IL_29F:
			num4 = xbdfb620b7167944b.x8d48119452c041bd - xbcea506a33cf - 4;
			goto IL_2AA;
			Block_16:
			throw new ArgumentOutOfRangeException("style");
			Block_19:
			flag = ((uint)num2 + (uint)num2 < 0U);
			if (flag)
			{
				goto IL_352;
			}
			goto IL_3AE;
			IL_26:
			IL_4A:
			xbdfb620b7167944b.x730ca8c251524854--;
			flag = (((uint)num4 | 2147483648U) == 0U);
			if (!flag)
			{
				return;
			}
			if (((uint)num & 0U) == 0U)
			{
				goto IL_3AE;
			}
			if (((uint)b & 0U) != 0U)
			{
				goto IL_A1;
			}
			goto IL_2BF;
			IL_10F:
			x479f2661aae93792.x6a87193e5bb23362(array, xbcea506a33cf + 1, array, xbcea506a33cf + 1 + num, num4);
			IL_122:
			num3 = (uint)num4;
			flag = (((uint)b | 8U) == 0U);
			if (flag)
			{
				goto IL_2BF;
			}
			goto IL_A1;
			IL_282:
			num4 = xbdfb620b7167944b.x8d48119452c041bd - xbcea506a33cf - 4;
			ProtoWriter.xac096aee2e17a712(num4, xbdfb620b7167944b.x8b3e45f7763ede93, xbcea506a33cf);
			goto IL_4A;
			IL_2AA:
			byte[] array2 = xbdfb620b7167944b.x8b3e45f7763ede93;
			do
			{
				ProtoWriter.xac096aee2e17a712(num4, array2, xbcea506a33cf);
				b = array2[xbcea506a33cf];
				array2[xbcea506a33cf] = array2[xbcea506a33cf + 3];
				array2[xbcea506a33cf + 3] = b;
				b = array2[xbcea506a33cf + 1];
				if (((uint)xbcea506a33cf | 1U) == 0U)
				{
					goto IL_122;
				}
				if ((uint)xbcea506a33cf < 0U)
				{
					goto IL_10F;
				}
				array2[xbcea506a33cf + 1] = array2[xbcea506a33cf + 2];
				flag = ((uint)num4 + (uint)num < 0U);
			}
			while (flag);
			array2[xbcea506a33cf + 2] = b;
			flag = ((uint)num4 < 0U);
			if (flag)
			{
				goto IL_282;
			}
			goto IL_4A;
			IL_352:
			xbdfb620b7167944b.xa5694e1c82a939b4 = WireType.None;
			return;
			IL_35C:
			if (xbcea506a33cf < 0)
			{
				goto IL_3D8;
			}
			if (((uint)num4 & 0U) == 0U)
			{
				goto IL_3FE;
			}
			flag = ((uint)num2 - num3 < 0U);
			if (!flag)
			{
				goto IL_3AE;
			}
			IL_391:
			if ((uint)num2 - (uint)num4 < 0U)
			{
				goto IL_2AA;
			}
			flag = ((uint)xbcea506a33cf + (uint)b > uint.MaxValue);
			if (flag)
			{
				goto IL_3FE;
			}
			goto IL_352;
			IL_3D8:
			ProtoWriter.xd21f404d2945236a(-xbcea506a33cf, WireType.EndGroup, xbdfb620b7167944b);
			goto IL_391;
			IL_3FE:
			goto IL_2BF;
			IL_429:
			xbcea506a33cf = x153c99a852375422.xbcea506a33cf9111;
			if (xbdfb620b7167944b.x1af9fc7847b7aa2c <= 0)
			{
				throw ProtoWriter.x98bdea680591defc(xbdfb620b7167944b);
			}
			num2 = xbdfb620b7167944b.x1af9fc7847b7aa2c--;
			if (num2 <= 25)
			{
				goto IL_3BC;
			}
			goto IL_3AE;
		}

		public ProtoWriter(Stream dest, TypeModel model, SerializationContext context)
		{
			if (!false)
			{
				if (dest == null)
				{
					throw new ArgumentNullException("dest");
				}
				for (;;)
				{
					while (dest.CanWrite)
					{
						this.x6b8e154b42d5c1e3 = dest;
						this.x8b3e45f7763ede93 = xe2887184ca5e62c2.x7145b88c884afa7f();
						if (2 != 0)
						{
							this.xad70a5849826ecef = model;
							if (-1 != 0)
							{
								goto IL_89;
							}
						}
					}
					break;
				}
				throw new ArgumentException("Cannot write to stream", "dest");
				IL_89:
				this.xa5694e1c82a939b4 = WireType.None;
				if (!false)
				{
				}
			}
			if (!false)
			{
				if (context == null)
				{
					context = SerializationContext.xb9715d2f06b63cf0;
				}
				else
				{
					context.x69322180ae719ea6();
				}
			}
			this.x0f7b23d1c393aed9 = context;
		}

		public SerializationContext Context
		{
			get
			{
				return this.x0f7b23d1c393aed9;
			}
		}

		void IDisposable.x4852c3ca92ab200a()
		{
			this.Dispose();
		}

		private void Dispose()
		{
			if (this.x6b8e154b42d5c1e3 != null)
			{
				ProtoWriter.xbb7550bbb62a218c(this);
				this.x6b8e154b42d5c1e3 = null;
			}
			this.xad70a5849826ecef = null;
			xe2887184ca5e62c2.xb8b95ec3e3f43efa(ref this.x8b3e45f7763ede93);
		}

		internal static int xed8a0d4499d6f292(ProtoWriter xbdfb620b7167944b)
		{
			return xbdfb620b7167944b.x13d4cb8d1bd20347;
		}

		private static void xb0749858b4a5e375(int x3362caa77b1f70b4, ProtoWriter xbdfb620b7167944b)
		{
			if (xbdfb620b7167944b.x8b3e45f7763ede93.Length - xbdfb620b7167944b.x8d48119452c041bd < x3362caa77b1f70b4)
			{
				if (xbdfb620b7167944b.x730ca8c251524854 == 0)
				{
					ProtoWriter.xbb7550bbb62a218c(xbdfb620b7167944b);
					if (xbdfb620b7167944b.x8b3e45f7763ede93.Length - xbdfb620b7167944b.x8d48119452c041bd >= x3362caa77b1f70b4)
					{
						return;
					}
				}
				xe2887184ca5e62c2.x6e1b6cb1ff4c4538(ref xbdfb620b7167944b.x8b3e45f7763ede93, x3362caa77b1f70b4 + xbdfb620b7167944b.x8d48119452c041bd, 0, xbdfb620b7167944b.x8d48119452c041bd);
			}
		}

		public void Close()
		{
			if (this.x1af9fc7847b7aa2c != 0 || this.x730ca8c251524854 != 0)
			{
				throw new InvalidOperationException("Unable to close stream in an incomplete state");
			}
			this.Dispose();
		}

		internal void xf5a7c936201d3c81()
		{
			if (this.x1af9fc7847b7aa2c == 0 && this.x730ca8c251524854 == 0)
			{
				return;
			}
			throw new InvalidOperationException("The writer is in an incomplete state");
		}

		public TypeModel Model
		{
			get
			{
				return this.xad70a5849826ecef;
			}
		}

		internal static void xbb7550bbb62a218c(ProtoWriter xbdfb620b7167944b)
		{
			if (xbdfb620b7167944b.x730ca8c251524854 == 0 && xbdfb620b7167944b.x8d48119452c041bd != 0)
			{
				xbdfb620b7167944b.x6b8e154b42d5c1e3.Write(xbdfb620b7167944b.x8b3e45f7763ede93, 0, xbdfb620b7167944b.x8d48119452c041bd);
				xbdfb620b7167944b.x8d48119452c041bd = 0;
			}
		}

		private static void x608d56e5eb60d6f7(uint xbcea506a33cf9111, ProtoWriter xbdfb620b7167944b)
		{
			ProtoWriter.xb0749858b4a5e375(5, xbdfb620b7167944b);
			int num;
			int num2;
			if ((uint)num + (uint)num2 >= 0U)
			{
				num = 0;
				do
				{
					byte[] array = xbdfb620b7167944b.x8b3e45f7763ede93;
					num2 = xbdfb620b7167944b.x8d48119452c041bd++;
					array[num2] = (byte)((xbcea506a33cf9111 & 127U) | 128U);
					num++;
				}
				while ((xbcea506a33cf9111 >>= 7) != 0U);
				byte[] array2 = xbdfb620b7167944b.x8b3e45f7763ede93;
				int num3 = xbdfb620b7167944b.x8d48119452c041bd - 1;
				array2[num3] &= 127;
				xbdfb620b7167944b.x13d4cb8d1bd20347 += num;
			}
		}

		internal static uint x3953193986db7ab6(int xbcea506a33cf9111)
		{
			return (uint)(xbcea506a33cf9111 << 1 ^ xbcea506a33cf9111 >> 31);
		}

		internal static ulong x3953193986db7ab6(long xbcea506a33cf9111)
		{
			return (ulong)(xbcea506a33cf9111 << 1 ^ xbcea506a33cf9111 >> 63);
		}

		private static void x682618ae849e1564(ulong xbcea506a33cf9111, ProtoWriter xbdfb620b7167944b)
		{
			ProtoWriter.xb0749858b4a5e375(10, xbdfb620b7167944b);
			int num = 0;
			do
			{
				xbdfb620b7167944b.x8b3e45f7763ede93[xbdfb620b7167944b.x8d48119452c041bd++] = (byte)((xbcea506a33cf9111 & 127UL) | 128UL);
				num++;
			}
			while ((xbcea506a33cf9111 >>= 7) != 0UL || 2147483647 == 0);
			byte[] array = xbdfb620b7167944b.x8b3e45f7763ede93;
			int num2 = xbdfb620b7167944b.x8d48119452c041bd - 1;
			array[num2] &= 127;
			xbdfb620b7167944b.x13d4cb8d1bd20347 += num;
		}

		public static void WriteString(string value, ProtoWriter writer)
		{
			if (writer.xa5694e1c82a939b4 != WireType.String)
			{
				throw ProtoWriter.x98bdea680591defc(writer);
			}
			while (value != null)
			{
				if (8 == 0)
				{
					goto IL_BF;
				}
				int length;
				int byteCount;
				for (;;)
				{
					length = value.Length;
					if (length == 0)
					{
						break;
					}
					byteCount = ProtoWriter.xff3edc9aa5f0523b.GetByteCount(value);
					ProtoWriter.x608d56e5eb60d6f7((uint)byteCount, writer);
					if (!false)
					{
						goto Block_2;
					}
				}
				if ((uint)length + (uint)byteCount < 0U)
				{
					return;
				}
				bool flag = ((uint)byteCount | 1U) == 0U;
				if (flag)
				{
					continue;
				}
				goto IL_BF;
				Block_2:
				ProtoWriter.xb0749858b4a5e375(byteCount, writer);
				int bytes = ProtoWriter.xff3edc9aa5f0523b.GetBytes(value, 0, value.Length, writer.x8b3e45f7763ede93, writer.x8d48119452c041bd);
				ProtoWriter.x64cb5f5a88b3094c(bytes, writer);
				return;
				IL_BF:
				ProtoWriter.x608d56e5eb60d6f7(0U, writer);
				writer.xa5694e1c82a939b4 = WireType.None;
				return;
			}
			throw new ArgumentNullException("value");
		}

		public static void WriteUInt64(ulong value, ProtoWriter writer)
		{
			WireType wireType = writer.xa5694e1c82a939b4;
			switch (wireType)
			{
			case WireType.Variant:
			{
				ProtoWriter.x682618ae849e1564(value, writer);
				if ((uint)value - (uint)value < 0U)
				{
					goto IL_18;
				}
				if (((uint)value | 2U) != 0U)
				{
					writer.xa5694e1c82a939b4 = WireType.None;
					return;
				}
				if ((uint)value + (uint)value < 0U)
				{
					goto IL_18;
				}
				bool flag = (uint)value - (uint)value < 0U;
				if (flag)
				{
					if (false)
					{
						return;
					}
					goto IL_BB;
				}
				break;
			}
			case WireType.Fixed64:
				ProtoWriter.WriteInt64((long)value, writer);
				return;
			default:
				goto IL_18;
			}
			IL_0C:
			ProtoWriter.WriteUInt32(checked((uint)value), writer);
			return;
			IL_18:
			if (wireType == WireType.Fixed32)
			{
				goto IL_0C;
			}
			IL_BB:
			throw ProtoWriter.x98bdea680591defc(writer);
		}

		public static void WriteInt64(long value, ProtoWriter writer)
		{
			WireType wireType = writer.xa5694e1c82a939b4;
			byte[] array;
			int num;
			bool flag;
			switch (wireType)
			{
			case WireType.Variant:
				if (value >= 0L)
				{
					ProtoWriter.x682618ae849e1564((ulong)value, writer);
					writer.xa5694e1c82a939b4 = WireType.None;
					return;
				}
				ProtoWriter.xb0749858b4a5e375(10, writer);
				if (false)
				{
					goto IL_0E;
				}
				array = writer.x8b3e45f7763ede93;
				num = writer.x8d48119452c041bd;
				if (false)
				{
					goto IL_18A;
				}
				flag = ((uint)value < 0U);
				if (flag)
				{
					goto IL_293;
				}
				goto IL_EA;
			case WireType.Fixed64:
				ProtoWriter.xb0749858b4a5e375(8, writer);
				array = writer.x8b3e45f7763ede93;
				if ((uint)num + (uint)num < 0U)
				{
					goto IL_175;
				}
				num = writer.x8d48119452c041bd;
				array[num] = (byte)value;
				array[num + 1] = (byte)(value >> 8);
				array[num + 2] = (byte)(value >> 16);
				for (;;)
				{
					array[num + 3] = (byte)(value >> 24);
					array[num + 4] = (byte)(value >> 32);
					array[num + 5] = (byte)(value >> 40);
					array[num + 6] = (byte)(value >> 48);
					if (-2147483648 != 0)
					{
						break;
					}
					if (15 != 0)
					{
						goto IL_238;
					}
				}
				flag = ((uint)num < 0U);
				if (flag)
				{
					goto IL_18A;
				}
				array[num + 7] = (byte)(value >> 56);
				ProtoWriter.x64cb5f5a88b3094c(8, writer);
				return;
			default:
				goto IL_238;
			}
			return;
			IL_0E:
			ProtoWriter.WriteInt32(checked((int)value), writer);
			flag = ((uint)num - (uint)value > uint.MaxValue);
			if (!flag)
			{
				goto IL_12E;
			}
			IL_EA:
			array[num] = (byte)(value | 128L);
			array[num + 1] = (byte)((int)(value >> 7) | 128);
			array[num + 2] = (byte)((int)(value >> 14) | 128);
			if (((uint)num & 0U) == 0U)
			{
				do
				{
					array[num + 3] = (byte)((int)(value >> 21) | 128);
					array[num + 4] = (byte)((int)(value >> 28) | 128);
					if ((uint)value < 0U)
					{
						goto IL_18A;
					}
					array[num + 5] = (byte)((int)(value >> 35) | 128);
					if ((uint)value >= 0U)
					{
						array[num + 6] = (byte)((int)(value >> 42) | 128);
						array[num + 7] = (byte)((int)(value >> 49) | 128);
						if (false)
						{
							return;
						}
					}
					array[num + 8] = (byte)((int)(value >> 56) | 128);
					array[num + 9] = 1;
					ProtoWriter.x64cb5f5a88b3094c(10, writer);
				}
				while (255 == 0);
				return;
			}
			IL_12E:
			if (8 == 0)
			{
				goto IL_293;
			}
			return;
			IL_175:
			writer.xa5694e1c82a939b4 = WireType.None;
			if (true)
			{
				return;
			}
			return;
			IL_18A:
			ProtoWriter.x682618ae849e1564(ProtoWriter.x3953193986db7ab6(value), writer);
			goto IL_175;
			IL_238:
			if (wireType == WireType.Fixed32)
			{
				goto IL_0E;
			}
			if (wireType == WireType.SignedVariant)
			{
				goto IL_18A;
			}
			IL_293:
			throw ProtoWriter.x98bdea680591defc(writer);
		}

		public static void WriteUInt32(uint value, ProtoWriter writer)
		{
			WireType wireType = writer.xa5694e1c82a939b4;
			if (8 != 0)
			{
				for (;;)
				{
					switch (wireType)
					{
					case WireType.Variant:
						goto IL_36;
					case WireType.Fixed64:
						goto IL_2D;
					default:
						if (wireType == WireType.Fixed32)
						{
							goto IL_25;
						}
						if (15 != 0)
						{
							goto Block_3;
						}
						break;
					}
				}
				Block_3:
				throw ProtoWriter.x98bdea680591defc(writer);
				IL_2D:
				ProtoWriter.WriteInt64((long)value, writer);
				return;
				IL_36:
				ProtoWriter.x608d56e5eb60d6f7(value, writer);
				writer.xa5694e1c82a939b4 = WireType.None;
				return;
			}
			IL_25:
			ProtoWriter.WriteInt32((int)value, writer);
		}

		public static void WriteInt16(short value, ProtoWriter writer)
		{
			ProtoWriter.WriteInt32((int)value, writer);
		}

		public static void WriteUInt16(ushort value, ProtoWriter writer)
		{
			ProtoWriter.WriteUInt32((uint)value, writer);
		}

		public static void WriteByte(byte value, ProtoWriter writer)
		{
			ProtoWriter.WriteUInt32((uint)value, writer);
		}

		public static void WriteSByte(sbyte value, ProtoWriter writer)
		{
			ProtoWriter.WriteInt32((int)value, writer);
		}

		private static void xac096aee2e17a712(int xbcea506a33cf9111, byte[] x5cafa8d49ea71ea1, int xc0c4c459c6ccbd00)
		{
			x5cafa8d49ea71ea1[xc0c4c459c6ccbd00] = (byte)xbcea506a33cf9111;
			x5cafa8d49ea71ea1[xc0c4c459c6ccbd00 + 1] = (byte)(xbcea506a33cf9111 >> 8);
			x5cafa8d49ea71ea1[xc0c4c459c6ccbd00 + 2] = (byte)(xbcea506a33cf9111 >> 16);
			x5cafa8d49ea71ea1[xc0c4c459c6ccbd00 + 3] = (byte)(xbcea506a33cf9111 >> 24);
		}

		public static void WriteInt32(int value, ProtoWriter writer)
		{
			WireType wireType = writer.xa5694e1c82a939b4;
			byte b;
			byte b2;
			bool flag = (uint)b - (uint)b2 < 0U;
			if (flag)
			{
				goto IL_154;
			}
			goto IL_2C2;
			IL_154:
			byte[] array2;
			byte[] array = array2;
			int num2;
			int num = num2 + 4;
			byte[] array3 = array2;
			int num3 = num2 + 5;
			byte[] array4 = array2;
			int num4 = num2 + 6;
			byte b3 = array2[num2 + 7] = 0;
			b2 = (array4[num4] = b3);
			b = (array3[num3] = b2);
			array[num] = b;
			byte b4;
			if ((uint)b4 < 0U)
			{
				return;
			}
			ProtoWriter.x64cb5f5a88b3094c(8, writer);
			byte b5;
			byte b6;
			if (((uint)b5 | 8U) != 0U)
			{
				if ((uint)b6 + (uint)num2 > 4294967295U)
				{
					goto IL_252;
				}
				if ((uint)num2 <= 4294967295U)
				{
					return;
				}
				goto IL_2F0;
			}
			IL_21C:
			array2[num2] = (byte)value;
			flag = ((uint)value < 0U);
			if (flag)
			{
				goto IL_29C;
			}
			array2[num2 + 1] = (byte)(value >> 8);
			array2[num2 + 2] = (byte)(value >> 16);
			array2[num2 + 3] = (byte)(value >> 24);
			flag = ((uint)b3 + (uint)b < 0U);
			if (flag)
			{
				flag = ((uint)b3 + (uint)b2 < 0U);
				if (flag)
				{
					goto IL_26D;
				}
				goto IL_29C;
			}
			else
			{
				flag = ((uint)b4 + (uint)b3 < 0U);
				if (flag)
				{
					goto IL_2C2;
				}
				flag = ((uint)b2 + (uint)b > uint.MaxValue);
				if (flag)
				{
					goto IL_2F0;
				}
				goto IL_154;
			}
			IL_252:
			ProtoWriter.xb0749858b4a5e375(4, writer);
			ProtoWriter.xac096aee2e17a712(value, writer.x8b3e45f7763ede93, writer.x8d48119452c041bd);
			ProtoWriter.x64cb5f5a88b3094c(4, writer);
			return;
			IL_26D:
			flag = (((uint)value & 0U) == 0U);
			if (flag)
			{
				goto IL_2F0;
			}
			IL_29C:
			if (wireType != WireType.SignedVariant)
			{
				goto IL_26D;
			}
			ProtoWriter.x608d56e5eb60d6f7(ProtoWriter.x3953193986db7ab6(value), writer);
			writer.xa5694e1c82a939b4 = WireType.None;
			return;
			IL_2C2:
			switch (wireType)
			{
			case WireType.Variant:
				if (value >= 0)
				{
					ProtoWriter.x608d56e5eb60d6f7((uint)value, writer);
					writer.xa5694e1c82a939b4 = WireType.None;
					return;
				}
				ProtoWriter.xb0749858b4a5e375(10, writer);
				array2 = writer.x8b3e45f7763ede93;
				num2 = writer.x8d48119452c041bd;
				do
				{
					array2[num2] = (byte)(value | 128);
					array2[num2 + 1] = (byte)(value >> 7 | 128);
					array2[num2 + 2] = (byte)(value >> 14 | 128);
					array2[num2 + 3] = (byte)(value >> 21 | 128);
					flag = ((uint)b6 + (uint)b > uint.MaxValue);
					if (flag)
					{
						goto Block_3;
					}
					array2[num2 + 4] = (byte)(value >> 28 | 128);
					byte[] array5 = array2;
					int num5 = num2 + 5;
					byte[] array6 = array2;
					int num6 = num2 + 6;
					byte[] array7 = array2;
					int num7 = num2 + 7;
					b5 = (array2[num2 + 8] = byte.MaxValue);
					b6 = (array7[num7] = b5);
					b4 = (array6[num6] = b6);
					array5[num5] = b4;
					array2[num2 + 9] = 1;
				}
				while ((uint)b6 > 4294967295U);
				ProtoWriter.x64cb5f5a88b3094c(10, writer);
				return;
				Block_3:
				break;
			case WireType.Fixed64:
				ProtoWriter.xb0749858b4a5e375(8, writer);
				if ((uint)b4 - (uint)b2 <= 4294967295U)
				{
					array2 = writer.x8b3e45f7763ede93;
					num2 = writer.x8d48119452c041bd;
					goto IL_21C;
				}
				goto IL_26D;
			default:
				if (wireType == WireType.Fixed32)
				{
					goto IL_252;
				}
				goto IL_29C;
			}
			IL_2F0:
			throw ProtoWriter.x98bdea680591defc(writer);
		}

		public unsafe static void WriteDouble(double value, ProtoWriter writer)
		{
			WireType wireType = writer.xa5694e1c82a939b4;
			float num;
			if ((uint)num <= 4294967295U)
			{
				while (wireType != WireType.Fixed64)
				{
					if (wireType != WireType.Fixed32)
					{
						throw ProtoWriter.x98bdea680591defc(writer);
					}
					num = (float)value;
					if (false)
					{
						goto IL_29;
					}
					if (x479f2661aae93792.xa696f2cecd0cd4ae(num))
					{
						goto IL_29;
					}
					if ((uint)num + (uint)value > 4294967295U)
					{
						continue;
					}
					if (((uint)num & 0U) != 0U)
					{
						break;
					}
					IL_21:
					ProtoWriter.WriteSingle(num, writer);
					return;
					IL_29:
					if (!x479f2661aae93792.xa696f2cecd0cd4ae(value))
					{
						throw new OverflowException();
					}
					goto IL_21;
				}
				ProtoWriter.WriteInt64(*(long*)(&value), writer);
				return;
			}
		}

		public unsafe static void WriteSingle(float value, ProtoWriter writer)
		{
			WireType wireType = writer.xa5694e1c82a939b4;
			if (wireType == WireType.Fixed64)
			{
				ProtoWriter.WriteDouble((double)value, writer);
				return;
			}
			if (wireType == WireType.Fixed32)
			{
				if ((uint)value <= 4294967295U)
				{
					ProtoWriter.WriteInt32(*(int*)(&value), writer);
				}
				return;
			}
			throw ProtoWriter.x98bdea680591defc(writer);
		}

		public static void ThrowEnumException(ProtoWriter writer, object enumValue)
		{
			string str = (enumValue == null) ? "<null>" : (enumValue.GetType().FullName + "." + enumValue.ToString());
			throw new ProtoException("No wire-value is mapped to the enum " + str);
		}

		internal static Exception x98bdea680591defc(ProtoWriter xbdfb620b7167944b)
		{
			return new ProtoException(string.Concat(new object[]
			{
				"Invalid serialization operation with wire-type ",
				xbdfb620b7167944b.xa5694e1c82a939b4,
				" at position ",
				xbdfb620b7167944b.x13d4cb8d1bd20347
			}));
		}

		public static void WriteBoolean(bool value, ProtoWriter writer)
		{
			ProtoWriter.WriteUInt32(value ? 1U : 0U, writer);
		}

		public static void AppendExtensionData(IExtensible instance, ProtoWriter writer)
		{
			if (instance == null)
			{
				if (-2147483648 != 0)
				{
					throw new ArgumentNullException("instance");
				}
			}
			else
			{
				if (writer.xa5694e1c82a939b4 != WireType.None)
				{
					throw ProtoWriter.x98bdea680591defc(writer);
				}
				IExtension extensionObject = instance.GetExtensionObject(false);
				while (extensionObject != null)
				{
					Stream stream = extensionObject.BeginQuery();
					try
					{
						ProtoWriter.xbc317e2a1e6559c0(stream, writer);
						break;
					}
					finally
					{
						extensionObject.EndQuery(stream);
					}
				}
			}
		}

		public static void SetPackedField(int fieldNumber, ProtoWriter writer)
		{
			if (fieldNumber <= 0)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			writer.x2f0158cd84d79a16 = fieldNumber;
		}

		internal string x0885c39348d7aa9b(Type x43163d22e8cd5a71)
		{
			return TypeModel.x0885c39348d7aa9b(this.xad70a5849826ecef, x43163d22e8cd5a71);
		}

		public void SetRootObject(object value)
		{
			this.x4406e28dba6f6c8e.x24ab034b13a4cf87(0, value);
		}

		public static void WriteType(Type value, ProtoWriter writer)
		{
			ProtoWriter.WriteString(writer.x0885c39348d7aa9b(value), writer);
		}

		private const int x93a13b11462c8343 = 25;

		private Stream x6b8e154b42d5c1e3;

		private TypeModel xad70a5849826ecef;

		private readonly x15bd90f59150b4e2 x49b766214fe3dda0 = new x15bd90f59150b4e2();

		private int xade3b695478596d6;

		private int x730ca8c251524854;

		private WireType xa5694e1c82a939b4;

		private int x1af9fc7847b7aa2c;

		private xd4dab911626dd004 x4fc90296feee4ebf;

		private readonly SerializationContext x0f7b23d1c393aed9;

		private byte[] x8b3e45f7763ede93;

		private int x8d48119452c041bd;

		private int x13d4cb8d1bd20347;

		private static readonly UTF8Encoding xff3edc9aa5f0523b = new UTF8Encoding();

		private int x2f0158cd84d79a16;
	}
}
