using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	public sealed class ProtoReader : IDisposable
	{
		public int FieldNumber
		{
			get
			{
				return this.xade3b695478596d6;
			}
		}

		public WireType WireType
		{
			get
			{
				return this.xa5694e1c82a939b4;
			}
		}

		public ProtoReader(Stream source, TypeModel model, SerializationContext context) : this(source, model, context, -1)
		{
		}

		public bool InternStrings
		{
			get
			{
				return this.x48c2a32eaf094d8a;
			}
			set
			{
				this.x48c2a32eaf094d8a = value;
			}
		}

		public ProtoReader(Stream source, TypeModel model, SerializationContext context, int length)
		{
			if (!true)
			{
				goto IL_94;
			}
			if (false)
			{
				return;
			}
			IL_16A:
			bool flag;
			while (source != null)
			{
				while (source.CanRead)
				{
					this.x337e217cb3ba0627 = source;
					this.x8b3e45f7763ede93 = xe2887184ca5e62c2.x7145b88c884afa7f();
					this.xad70a5849826ecef = model;
					this.x01bba5daeb6e30a8 = (length >= 0);
					flag = ((uint)length - (uint)length > uint.MaxValue);
					if (!flag)
					{
						goto IL_E8;
					}
					if (!false)
					{
						goto IL_16A;
					}
				}
				throw new ArgumentException("Cannot read from stream", "source");
			}
			throw new ArgumentNullException("source");
			IL_36:
			this.x0f7b23d1c393aed9 = context;
			flag = ((uint)length > uint.MaxValue);
			if (!flag)
			{
				return;
			}
			flag = ((uint)length - (uint)length < 0U);
			if (flag)
			{
				goto IL_E8;
			}
			goto IL_16A;
			IL_82:
			if ((uint)length > 4294967295U)
			{
				goto IL_36;
			}
			IL_94:
			this.x09e8e06d4b0600f4 = (this.x01bba5daeb6e30a8 ? length : 0);
			if (context == null)
			{
				context = SerializationContext.xb9715d2f06b63cf0;
				goto IL_36;
			}
			flag = ((uint)length + (uint)length > uint.MaxValue);
			if (flag)
			{
				goto IL_82;
			}
			if (-2 != 0)
			{
				context.x69322180ae719ea6();
				flag = ((uint)length > uint.MaxValue);
				if (flag)
				{
					goto IL_82;
				}
			}
			goto IL_36;
			IL_E8:
			if ((uint)length < 0U)
			{
				goto IL_36;
			}
			if (2 == 0)
			{
				goto IL_16A;
			}
			if (15 != 0)
			{
				goto IL_82;
			}
		}

		public SerializationContext Context
		{
			get
			{
				return this.x0f7b23d1c393aed9;
			}
		}

		public void Dispose()
		{
			this.x337e217cb3ba0627 = null;
			this.xad70a5849826ecef = null;
			xe2887184ca5e62c2.xb8b95ec3e3f43efa(ref this.x8b3e45f7763ede93);
		}

		internal int xfbd3c47b33674661(bool x0a50069e75b19055, out uint xbcea506a33cf9111)
		{
			if (this.xadf615803d4cf295 < 10)
			{
				this.xae7813dfd13745c1(10, false);
			}
			while (this.xadf615803d4cf295 != 0)
			{
				int num = this.x8d48119452c041bd;
				xbcea506a33cf9111 = (uint)this.x8b3e45f7763ede93[num++];
				uint num2;
				bool flag = (num2 | 255U) == 0U;
				if (!flag)
				{
					goto IL_336;
				}
				flag = ((x0a50069e75b19055 ? 1U : 0U) > uint.MaxValue);
				if (!flag)
				{
					goto IL_336;
				}
				if (2 == 0)
				{
					goto IL_32B;
				}
				IL_2E0:
				num2 = (uint)this.x8b3e45f7763ede93[num++];
				flag = ((num2 & 0U) == 0U);
				if (flag)
				{
					xbcea506a33cf9111 |= (num2 & 127U) << 7;
					goto IL_273;
				}
				goto IL_1F1;
				IL_F6:
				flag = ((x0a50069e75b19055 ? 1U : 0U) < 0U);
				if (flag)
				{
					flag = ((uint)num + num2 < 0U);
					if (flag)
					{
						if (!false)
						{
							goto IL_62;
						}
						if ((num2 | 4294967294U) != 0U)
						{
							goto IL_75;
						}
					}
					goto IL_96;
				}
				if ((x0a50069e75b19055 ? 1U : 0U) < 0U)
				{
					goto IL_96;
				}
				if (num2 - (x0a50069e75b19055 ? 1U : 0U) < 0U)
				{
					continue;
				}
				if (8 == 0)
				{
					goto IL_75;
				}
				if ((x0a50069e75b19055 ? 1U : 0U) + (uint)num > 4294967295U)
				{
					goto IL_19F;
				}
				flag = ((x0a50069e75b19055 ? 1U : 0U) > uint.MaxValue);
				if (!flag)
				{
					goto IL_3AF;
				}
				flag = ((x0a50069e75b19055 ? 1U : 0U) - num2 > uint.MaxValue);
				if (flag)
				{
					goto IL_19F;
				}
				if (false)
				{
					goto IL_273;
				}
				goto IL_27C;
				IL_1B1:
				xbcea506a33cf9111 |= num2 << 28;
				flag = (((x0a50069e75b19055 ? 1U : 0U) | 3U) == 0U);
				if (flag || (num2 & 240U) == 0U)
				{
					return 5;
				}
				if (!x0a50069e75b19055)
				{
					goto IL_3AF;
				}
				if ((num2 & 240U) != 240U)
				{
					goto IL_F6;
				}
				if (this.xadf615803d4cf295 >= 10)
				{
					goto IL_96;
				}
				goto IL_3AF;
				IL_19F:
				if (this.xadf615803d4cf295 != 4)
				{
					num2 = (uint)this.x8b3e45f7763ede93[num];
					goto IL_1B1;
				}
				throw ProtoReader.xa1450a2e84b3d41f(this);
				IL_27C:
				if (this.xadf615803d4cf295 == 2)
				{
					if (num2 + (uint)num >= 0U)
					{
						throw ProtoReader.xa1450a2e84b3d41f(this);
					}
					goto IL_F6;
				}
				else
				{
					num2 = (uint)this.x8b3e45f7763ede93[num++];
					xbcea506a33cf9111 |= (num2 & 127U) << 14;
					if ((uint)num <= 4294967295U)
					{
						while ((num2 & 128U) != 0U)
						{
							if (this.xadf615803d4cf295 == 3)
							{
								throw ProtoReader.xa1450a2e84b3d41f(this);
							}
							if (255 != 0)
							{
								if (false)
								{
									goto IL_1F1;
								}
								goto IL_1F1;
							}
						}
						return 3;
					}
					goto IL_1B1;
				}
				IL_273:
				if ((num2 & 128U) != 0U)
				{
					goto IL_27C;
				}
				return 2;
				IL_62:
				if (this.x8b3e45f7763ede93[++num] == 255)
				{
					goto IL_75;
				}
				goto IL_3AF;
				IL_1F1:
				num2 = (uint)this.x8b3e45f7763ede93[num++];
				xbcea506a33cf9111 |= (num2 & 127U) << 21;
				if ((num2 & 128U) != 0U)
				{
					goto IL_19F;
				}
				return 4;
				IL_75:
				if (this.x8b3e45f7763ede93[num + 1] != 1)
				{
					goto IL_3AF;
				}
				return 10;
				IL_96:
				if (this.x8b3e45f7763ede93[++num] == 255)
				{
					if (this.x8b3e45f7763ede93[++num] == 255)
					{
						if (this.x8b3e45f7763ede93[++num] == 255)
						{
							goto IL_62;
						}
					}
				}
				IL_3AF:
				throw ProtoReader.xe0800720badf45f1(new OverflowException(), this);
				IL_32B:
				if (this.xadf615803d4cf295 != 1)
				{
					goto IL_2E0;
				}
				throw ProtoReader.xa1450a2e84b3d41f(this);
				IL_336:
				if ((xbcea506a33cf9111 & 128U) != 0U)
				{
					xbcea506a33cf9111 &= 127U;
					goto IL_32B;
				}
				return 1;
			}
			xbcea506a33cf9111 = 0U;
			return 0;
		}

		private uint xeb5ebf9f61661623(bool x0a50069e75b19055)
		{
			uint result;
			int num = this.xfbd3c47b33674661(x0a50069e75b19055, out result);
			if (!false)
			{
				if (num <= 0)
				{
					throw ProtoReader.xa1450a2e84b3d41f(this);
				}
				this.x8d48119452c041bd += num;
			}
			this.xadf615803d4cf295 -= num;
			this.x13d4cb8d1bd20347 += num;
			return result;
		}

		private bool xb5c9aba8e230d14a(out uint xbcea506a33cf9111)
		{
			int i = this.xfbd3c47b33674661(false, out xbcea506a33cf9111);
			while (i <= 0)
			{
				bool flag = (uint)i + (uint)i > uint.MaxValue;
				if (!flag)
				{
					return false;
				}
			}
			this.x8d48119452c041bd += i;
			this.xadf615803d4cf295 -= i;
			if (-2147483648 != 0)
			{
			}
			this.x13d4cb8d1bd20347 += i;
			return true;
		}

		public uint ReadUInt32()
		{
			WireType wireType = this.xa5694e1c82a939b4;
			ulong num3;
			switch (wireType)
			{
			case WireType.Variant:
				return this.xeb5ebf9f61661623(false);
			case WireType.Fixed64:
				break;
			default:
				if (wireType == WireType.Fixed32)
				{
					int num;
					if (((uint)num | 255U) == 0U)
					{
						break;
					}
					int num2;
					if ((uint)num2 + (uint)num3 < 0U)
					{
						goto IL_83;
					}
					if (this.xadf615803d4cf295 < 4)
					{
						this.xae7813dfd13745c1(4, true);
					}
					this.x13d4cb8d1bd20347 += 4;
					this.xadf615803d4cf295 -= 4;
					int num4;
					if (((uint)num4 & 0U) == 0U)
					{
						byte[] array = this.x8b3e45f7763ede93;
						num4 = this.x8d48119452c041bd++;
						uint num5 = (uint)(array[num4] | (int)this.x8b3e45f7763ede93[this.x8d48119452c041bd++] << 8);
						byte[] array2 = this.x8b3e45f7763ede93;
						num2 = this.x8d48119452c041bd++;
						uint num6 = num5 | array2[num2] << 16;
						byte[] array3 = this.x8b3e45f7763ede93;
						num = this.x8d48119452c041bd++;
						return num6 | array3[num] << 24;
					}
				}
				throw this.xcc3b2c94d66a5338();
			}
			num3 = this.ReadUInt64();
			IL_83:
			return checked((uint)num3);
		}

		public int Position
		{
			get
			{
				return this.x13d4cb8d1bd20347;
			}
		}

		internal void xae7813dfd13745c1(int x10f4d88af727adbc, bool xec724f9375e8eee5)
		{
			if (x10f4d88af727adbc > this.x8b3e45f7763ede93.Length)
			{
				goto IL_1A0;
			}
			goto IL_1DF;
			IL_39:
			int num;
			int num2;
			int num3;
			if (x10f4d88af727adbc <= 0)
			{
				if ((uint)num > 4294967295U)
				{
					goto IL_1DF;
				}
			}
			else if (num2 > 0)
			{
				if ((num = this.x337e217cb3ba0627.Read(this.x8b3e45f7763ede93, num3, num2)) > 0)
				{
					this.xadf615803d4cf295 += num;
					goto IL_18C;
				}
				if ((uint)num - (uint)num3 > 4294967295U)
				{
					goto IL_89;
				}
			}
			if (xec724f9375e8eee5)
			{
				if (x10f4d88af727adbc > 0)
				{
					throw ProtoReader.xa1450a2e84b3d41f(this);
				}
			}
			return;
			IL_89:
			goto IL_39;
			IL_105:
			x10f4d88af727adbc -= this.xadf615803d4cf295;
			num3 = this.x8d48119452c041bd + this.xadf615803d4cf295;
			num2 = this.x8b3e45f7763ede93.Length - num3;
			IL_128:
			if (!this.x01bba5daeb6e30a8)
			{
				goto IL_89;
			}
			if (this.x09e8e06d4b0600f4 >= num2)
			{
				goto IL_39;
			}
			IL_171:
			bool flag = (uint)num - (uint)x10f4d88af727adbc < 0U;
			if (!flag)
			{
				num2 = this.x09e8e06d4b0600f4;
				if ((uint)num3 + (xec724f9375e8eee5 ? 1U : 0U) >= 0U)
				{
					goto IL_39;
				}
				goto IL_128;
			}
			IL_18C:
			if (((uint)num & 0U) == 0U)
			{
				x10f4d88af727adbc -= num;
				num2 -= num;
				num3 += num;
				if (this.x01bba5daeb6e30a8)
				{
					this.x09e8e06d4b0600f4 -= num;
					goto IL_39;
				}
				goto IL_39;
			}
			IL_1A0:
			xe2887184ca5e62c2.x6e1b6cb1ff4c4538(ref this.x8b3e45f7763ede93, x10f4d88af727adbc, this.x8d48119452c041bd, this.xadf615803d4cf295);
			if ((uint)num2 - (uint)num2 <= 4294967295U)
			{
				this.x8d48119452c041bd = 0;
				goto IL_105;
			}
			goto IL_105;
			IL_1DF:
			if (this.x8d48119452c041bd + x10f4d88af727adbc < this.x8b3e45f7763ede93.Length)
			{
				goto IL_105;
			}
			flag = (((uint)x10f4d88af727adbc & 0U) == 0U);
			if (!flag)
			{
				goto IL_128;
			}
			x479f2661aae93792.x6a87193e5bb23362(this.x8b3e45f7763ede93, this.x8d48119452c041bd, this.x8b3e45f7763ede93, 0, this.xadf615803d4cf295);
			flag = ((uint)num2 + (uint)num < 0U);
			if (flag)
			{
				goto IL_171;
			}
			this.x8d48119452c041bd = 0;
			goto IL_105;
		}

		public short ReadInt16()
		{
			return checked((short)this.ReadInt32());
		}

		public ushort ReadUInt16()
		{
			return checked((ushort)this.ReadUInt32());
		}

		public byte ReadByte()
		{
			return checked((byte)this.ReadUInt32());
		}

		public sbyte ReadSByte()
		{
			return checked((sbyte)this.ReadInt32());
		}

		public int ReadInt32()
		{
			WireType wireType = this.xa5694e1c82a939b4;
			switch (wireType)
			{
			case WireType.Variant:
				return (int)this.xeb5ebf9f61661623(true);
			case WireType.Fixed64:
			{
				long num = this.ReadInt64();
				return checked((int)num);
			}
			default:
				if (wireType == WireType.Fixed32)
				{
					while (this.xadf615803d4cf295 < 4)
					{
						this.xae7813dfd13745c1(4, true);
						while (!false)
						{
							long num;
							if ((uint)num <= 4294967295U)
							{
								if (false)
								{
									goto IL_E0;
								}
								goto IL_A1;
							}
						}
						continue;
						IL_A1:
						this.x13d4cb8d1bd20347 += 4;
						IL_E0:
						this.xadf615803d4cf295 -= 4;
						return (int)this.x8b3e45f7763ede93[this.x8d48119452c041bd++] | (int)this.x8b3e45f7763ede93[this.x8d48119452c041bd++] << 8 | (int)this.x8b3e45f7763ede93[this.x8d48119452c041bd++] << 16 | (int)this.x8b3e45f7763ede93[this.x8d48119452c041bd++] << 24;
					}
					goto IL_A1;
				}
				if (wireType != WireType.SignedVariant)
				{
					throw this.xcc3b2c94d66a5338();
				}
				return ProtoReader.x09b11d8dc59a63fd(this.xeb5ebf9f61661623(true));
			}
		}

		private static int x09b11d8dc59a63fd(uint xd0f6c850ce50e2bf)
		{
			return (int)(-(xd0f6c850ce50e2bf & 1U) ^ (uint)((int)xd0f6c850ce50e2bf >> 1 & int.MaxValue));
		}

		private static long x09b11d8dc59a63fd(ulong xd0f6c850ce50e2bf)
		{
			return (long)(-(long)(xd0f6c850ce50e2bf & 1UL) ^ (xd0f6c850ce50e2bf >> 1 & 9223372036854775807UL));
		}

		public long ReadInt64()
		{
			WireType wireType = this.xa5694e1c82a939b4;
			if (!false)
			{
				switch (wireType)
				{
				case WireType.Variant:
					return (long)this.x118a4e0bd4a688a2();
				case WireType.Fixed64:
				{
					int num;
					int num2;
					if (this.xadf615803d4cf295 < 8)
					{
						this.xae7813dfd13745c1(8, true);
						if ((uint)num > 4294967295U)
						{
							goto IL_17A;
						}
						if (((uint)num2 & 0U) != 0U)
						{
							goto IL_194;
						}
					}
					this.x13d4cb8d1bd20347 += 8;
					this.xadf615803d4cf295 -= 8;
					long num3 = (long)((ulong)this.x8b3e45f7763ede93[this.x8d48119452c041bd++] | (ulong)this.x8b3e45f7763ede93[this.x8d48119452c041bd++] << 8 | (ulong)this.x8b3e45f7763ede93[this.x8d48119452c041bd++] << 16);
					byte[] array = this.x8b3e45f7763ede93;
					num = this.x8d48119452c041bd++;
					long num4 = num3 | (long)((long)array[num] << 24);
					byte[] array2 = this.x8b3e45f7763ede93;
					num2 = this.x8d48119452c041bd++;
					return num4 | (long)((long)array2[num2] << 32) | (long)((long)((ulong)this.x8b3e45f7763ede93[this.x8d48119452c041bd++]) << 40) | (long)((long)((ulong)this.x8b3e45f7763ede93[this.x8d48119452c041bd++]) << 48) | (long)((long)((ulong)this.x8b3e45f7763ede93[this.x8d48119452c041bd++]) << 56);
				}
				}
			}
			if (wireType != WireType.Fixed32)
			{
				if (wireType == WireType.SignedVariant)
				{
					return ProtoReader.x09b11d8dc59a63fd(this.x118a4e0bd4a688a2());
				}
				goto IL_194;
			}
			IL_17A:
			return (long)this.ReadInt32();
			IL_194:
			throw this.xcc3b2c94d66a5338();
		}

		private int x7f061a73a2346417(out ulong xbcea506a33cf9111)
		{
			if (this.xadf615803d4cf295 < 10)
			{
				goto IL_565;
			}
			IL_51F:
			if (this.xadf615803d4cf295 == 0)
			{
				goto IL_570;
			}
			int num = this.x8d48119452c041bd;
			xbcea506a33cf9111 = (ulong)this.x8b3e45f7763ede93[num++];
			if ((xbcea506a33cf9111 & 128UL) == 0UL)
			{
				return 1;
			}
			xbcea506a33cf9111 &= 127UL;
			if (this.xadf615803d4cf295 == 1)
			{
				throw ProtoReader.xa1450a2e84b3d41f(this);
			}
			ulong num2 = (ulong)this.x8b3e45f7763ede93[num++];
			if (false)
			{
				goto IL_196;
			}
			bool flag = ((uint)num & 0U) == 0U;
			if (flag)
			{
				if ((uint)num2 + (uint)num <= 4294967295U)
				{
					xbcea506a33cf9111 |= (num2 & 127UL) << 7;
					if ((num2 & 128UL) == 0UL)
					{
						return 2;
					}
					IL_487:
					while (this.xadf615803d4cf295 != 2)
					{
						while ((uint)num2 <= 4294967295U)
						{
							flag = ((uint)num2 + (uint)num > uint.MaxValue);
							if (!flag)
							{
								goto IL_3B0;
							}
							IL_3FE:
							num2 = (ulong)this.x8b3e45f7763ede93[num++];
							xbcea506a33cf9111 |= (num2 & 127UL) << 21;
							flag = ((uint)num2 + (uint)num < 0U);
							if (flag)
							{
								flag = ((uint)num2 > uint.MaxValue);
								if (flag)
								{
									flag = ((uint)num + (uint)num2 > uint.MaxValue);
									if (!flag)
									{
										continue;
									}
								}
								else
								{
									flag = ((uint)num + (uint)num2 < 0U);
									if (!flag)
									{
										return 5;
									}
									flag = ((uint)num2 - (uint)num < 0U);
									if (!flag)
									{
										goto IL_565;
									}
									flag = ((uint)num > uint.MaxValue);
									if (!flag)
									{
										goto IL_54C;
									}
									if ((uint)num + (uint)num < 0U)
									{
										goto IL_570;
									}
									flag = ((uint)num2 > uint.MaxValue);
									if (flag)
									{
										goto IL_47B;
									}
									goto IL_487;
								}
							}
							else
							{
								if ((num2 & 128UL) == 0UL)
								{
									return 4;
								}
								flag = ((uint)num < 0U);
								if (flag)
								{
									goto IL_2C2;
								}
								goto IL_28A;
							}
							IL_3B0:
							num2 = (ulong)this.x8b3e45f7763ede93[num++];
							xbcea506a33cf9111 |= (num2 & 127UL) << 14;
							if ((num2 & 128UL) == 0UL)
							{
								return 3;
							}
							if (this.xadf615803d4cf295 != 3)
							{
								goto IL_3FE;
							}
							throw ProtoReader.xa1450a2e84b3d41f(this);
						}
						goto IL_66;
					}
					IL_47B:
					throw ProtoReader.xa1450a2e84b3d41f(this);
				}
				else
				{
					if (-1 == 0)
					{
						goto IL_28A;
					}
					goto IL_293;
				}
			}
			for (;;)
			{
				IL_1EF:
				num2 = (ulong)this.x8b3e45f7763ede93[num++];
				xbcea506a33cf9111 |= (num2 & 127UL) << 35;
				if ((num2 & 128UL) != 0UL)
				{
					goto IL_18D;
				}
				if ((uint)num2 - (uint)num >= 0U)
				{
					break;
				}
				if (4 == 0)
				{
					goto IL_235;
				}
			}
			return 6;
			IL_66:
			num2 = (ulong)this.x8b3e45f7763ede93[num];
			IL_124:
			if (false)
			{
				goto IL_565;
			}
			xbcea506a33cf9111 |= num2 << 63;
			if ((num2 & 18446744073709551614UL) != 0UL)
			{
				throw ProtoReader.xe0800720badf45f1(new OverflowException(), this);
			}
			return 10;
			IL_18D:
			if (this.xadf615803d4cf295 == 6)
			{
				throw ProtoReader.xa1450a2e84b3d41f(this);
			}
			IL_196:
			flag = ((uint)num > uint.MaxValue);
			if (!flag && (uint)num2 - (uint)num >= 0U)
			{
				flag = ((uint)num < 0U);
				if (flag)
				{
					return 7;
				}
			}
			else if ((uint)num - (uint)num2 < 0U)
			{
				goto IL_18D;
			}
			num2 = (ulong)this.x8b3e45f7763ede93[num++];
			xbcea506a33cf9111 |= (num2 & 127UL) << 42;
			if ((num2 & 128UL) != 0UL)
			{
				if (this.xadf615803d4cf295 != 7)
				{
					num2 = (ulong)this.x8b3e45f7763ede93[num++];
					if ((uint)num - (uint)num >= 0U)
					{
						xbcea506a33cf9111 |= (num2 & 127UL) << 49;
						if ((num2 & 128UL) == 0UL)
						{
							return 8;
						}
						if (this.xadf615803d4cf295 == 8)
						{
							throw ProtoReader.xa1450a2e84b3d41f(this);
						}
						num2 = (ulong)this.x8b3e45f7763ede93[num++];
						if (false)
						{
							goto IL_124;
						}
						xbcea506a33cf9111 |= (num2 & 127UL) << 56;
						if ((num2 & 128UL) == 0UL)
						{
							return 9;
						}
						if (this.xadf615803d4cf295 != 9)
						{
							if ((uint)num2 - (uint)num2 >= 0U)
							{
								goto IL_66;
							}
							goto IL_2C2;
						}
					}
					throw ProtoReader.xa1450a2e84b3d41f(this);
				}
				throw ProtoReader.xa1450a2e84b3d41f(this);
			}
			return 7;
			IL_235:
			if (this.xadf615803d4cf295 != 5)
			{
				goto IL_1EF;
			}
			throw ProtoReader.xa1450a2e84b3d41f(this);
			IL_28A:
			if (this.xadf615803d4cf295 == 4)
			{
				goto IL_2C2;
			}
			IL_293:
			num2 = (ulong)this.x8b3e45f7763ede93[num++];
			xbcea506a33cf9111 |= (num2 & 127UL) << 28;
			if ((num2 & 128UL) != 0UL)
			{
				goto IL_235;
			}
			return 5;
			IL_2C2:
			throw ProtoReader.xa1450a2e84b3d41f(this);
			IL_54C:
			if ((uint)num >= 0U)
			{
				return 0;
			}
			goto IL_28A;
			IL_570:
			xbcea506a33cf9111 = 0UL;
			goto IL_54C;
			IL_565:
			this.xae7813dfd13745c1(10, false);
			goto IL_51F;
		}

		private ulong x118a4e0bd4a688a2()
		{
			ulong result;
			int num = this.x7f061a73a2346417(out result);
			if (num > 0)
			{
				this.x8d48119452c041bd += num;
				this.xadf615803d4cf295 -= num;
				this.x13d4cb8d1bd20347 += num;
				return result;
			}
			throw ProtoReader.xa1450a2e84b3d41f(this);
		}

		private string x4f7f4f1b637d388d(string xbcea506a33cf9111)
		{
			if (xbcea506a33cf9111 == null)
			{
				return null;
			}
			if (xbcea506a33cf9111.Length != 0)
			{
				if (8 == 0)
				{
					goto IL_80;
				}
				if (false || this.x7c1c2b8bd447c71a == null)
				{
					this.x7c1c2b8bd447c71a = new Dictionary<string, string>();
					this.x7c1c2b8bd447c71a.Add(xbcea506a33cf9111, xbcea506a33cf9111);
					goto IL_80;
				}
				IL_1F:
				string result;
				if (this.x7c1c2b8bd447c71a.TryGetValue(xbcea506a33cf9111, out result))
				{
					return result;
				}
				if (!false)
				{
					this.x7c1c2b8bd447c71a.Add(xbcea506a33cf9111, xbcea506a33cf9111);
				}
				if (!false)
				{
					return xbcea506a33cf9111;
				}
				IL_46:
				goto IL_1F;
				IL_80:
				if (3 == 0)
				{
					goto IL_46;
				}
				return xbcea506a33cf9111;
			}
			return "";
		}

		public string ReadString()
		{
			if (this.xa5694e1c82a939b4 == WireType.String)
			{
				int num;
				string text;
				if (((uint)num & 0U) == 0U)
				{
					if ((uint)num + (uint)num <= 4294967295U)
					{
						num = (int)this.xeb5ebf9f61661623(false);
						if (num == 0)
						{
							return "";
						}
					}
					if (this.xadf615803d4cf295 < num)
					{
						this.xae7813dfd13745c1(num, true);
					}
					text = ProtoReader.xff3edc9aa5f0523b.GetString(this.x8b3e45f7763ede93, this.x8d48119452c041bd, num);
					bool flag = (uint)num > uint.MaxValue;
					if (flag)
					{
						goto IL_EA;
					}
				}
				while (this.x48c2a32eaf094d8a)
				{
					text = this.x4f7f4f1b637d388d(text);
					bool flag = (uint)num + (uint)num > uint.MaxValue;
					if (!flag)
					{
						IL_25:
						this.xadf615803d4cf295 -= num;
						this.x13d4cb8d1bd20347 += num;
						this.x8d48119452c041bd += num;
						return text;
					}
				}
				goto IL_25;
			}
			IL_EA:
			throw this.xcc3b2c94d66a5338();
		}

		public void ThrowEnumException(Type type, int value)
		{
			string text;
			if (type == null)
			{
				text = "<null>";
				goto IL_38;
			}
			IL_2B:
			text = type.FullName;
			IL_38:
			string text2 = text;
			if ((uint)value - (uint)value >= 0U)
			{
				object[] args;
				if (2 != 0)
				{
					args = new object[]
					{
						"No ",
						text2,
						" enum is mapped to the wire-value ",
						value
					};
				}
				throw ProtoReader.xe0800720badf45f1(new ProtoException(string.Concat(args)), this);
			}
			goto IL_2B;
		}

		private Exception xcc3b2c94d66a5338()
		{
			return this.x98bdea680591defc("Invalid wire-type; this usually means you have over-written a file without truncating or setting the length; see http://stackoverflow.com/q/2152978/23354");
		}

		private Exception x98bdea680591defc(string x1f25abf5fb75e795)
		{
			return ProtoReader.xe0800720badf45f1(new ProtoException(x1f25abf5fb75e795), this);
		}

		public unsafe double ReadDouble()
		{
			WireType wireType = this.xa5694e1c82a939b4;
			if (!false)
			{
				if (wireType == WireType.Fixed64)
				{
					long num = this.ReadInt64();
					return *(double*)(&num);
				}
			}
			if (wireType == WireType.Fixed32)
			{
				return (double)this.ReadSingle();
			}
			throw this.xcc3b2c94d66a5338();
		}

		public static object ReadObject(object value, int key, ProtoReader reader)
		{
			return ProtoReader.x38631c01067da2d2(value, key, reader, null);
		}

		internal static object x38631c01067da2d2(object xbcea506a33cf9111, int xba08ce632055a1d9, ProtoReader xe134235b3526fa75, Type x43163d22e8cd5a71)
		{
			if (xe134235b3526fa75.xad70a5849826ecef == null)
			{
				throw ProtoReader.xe0800720badf45f1(new InvalidOperationException("Cannot deserialize sub-objects unless a model is provided"), xe134235b3526fa75);
			}
			SubItemToken token = ProtoReader.StartSubItem(xe134235b3526fa75);
			if (xba08ce632055a1d9 < 0)
			{
				if (2147483647 != 0)
				{
					if (x43163d22e8cd5a71 == null)
					{
						goto IL_32;
					}
				}
				if (xe134235b3526fa75.xad70a5849826ecef.x3d653e8159e64ad2(xe134235b3526fa75, DataFormat.Default, 1, x43163d22e8cd5a71, ref xbcea506a33cf9111, true, false, true, false))
				{
					goto IL_27;
				}
				IL_32:
				TypeModel.ThrowUnexpectedType(x43163d22e8cd5a71);
			}
			else
			{
				xbcea506a33cf9111 = xe134235b3526fa75.xad70a5849826ecef.Deserialize(xba08ce632055a1d9, xbcea506a33cf9111, xe134235b3526fa75);
			}
			IL_27:
			ProtoReader.EndSubItem(token, xe134235b3526fa75);
			return xbcea506a33cf9111;
		}

		public static void EndSubItem(SubItemToken token, ProtoReader reader)
		{
			int xbcea506a33cf = token.xbcea506a33cf9111;
			bool flag;
			for (;;)
			{
				WireType wireType = reader.xa5694e1c82a939b4;
				if (((uint)xbcea506a33cf & 0U) != 0U)
				{
					goto IL_158;
				}
				if (wireType != WireType.EndGroup)
				{
					if (xbcea506a33cf < reader.x13d4cb8d1bd20347)
					{
						goto Block_6;
					}
					if (reader.x4cb534e776ee27b9 != reader.x13d4cb8d1bd20347)
					{
						if ((uint)xbcea506a33cf - (uint)xbcea506a33cf > 4294967295U)
						{
							goto IL_158;
						}
						if (reader.x4cb534e776ee27b9 != 2147483647)
						{
							goto IL_A4;
						}
					}
					reader.x4cb534e776ee27b9 = xbcea506a33cf;
					if (((uint)xbcea506a33cf | 255U) != 0U)
					{
						break;
					}
					continue;
				}
				IL_12F:
				if (xbcea506a33cf >= 0)
				{
					goto IL_15B;
				}
				if (-xbcea506a33cf != reader.xade3b695478596d6)
				{
					goto IL_FB;
				}
				flag = ((uint)xbcea506a33cf - (uint)xbcea506a33cf > uint.MaxValue);
				if (flag)
				{
					continue;
				}
				goto IL_178;
				IL_158:
				if (false)
				{
					goto IL_15B;
				}
				goto IL_12F;
			}
			flag = ((uint)xbcea506a33cf < 0U);
			if (!flag)
			{
				reader.x1af9fc7847b7aa2c--;
			}
			return;
			IL_A4:
			throw reader.x98bdea680591defc("Sub-message not read correctly");
			Block_6:
			throw reader.x98bdea680591defc("Sub-message not read entirely");
			IL_FB:
			throw reader.x98bdea680591defc("Wrong group was ended");
			IL_15B:
			throw ProtoReader.xe0800720badf45f1(new ArgumentException("token"), reader);
			IL_178:
			reader.xa5694e1c82a939b4 = WireType.None;
			reader.x1af9fc7847b7aa2c--;
		}

		public static SubItemToken StartSubItem(ProtoReader reader)
		{
			WireType wireType = reader.xa5694e1c82a939b4;
			int num;
			bool flag = (uint)num + (uint)num > uint.MaxValue;
			if (flag)
			{
				goto IL_BC;
			}
			switch (wireType)
			{
			case WireType.String:
			{
				int num2 = (int)reader.xeb5ebf9f61661623(false);
				if (num2 < 0)
				{
					throw ProtoReader.xe0800720badf45f1(new InvalidOperationException(), reader);
				}
				num = reader.x4cb534e776ee27b9;
				flag = (((uint)num | uint.MaxValue) == 0U);
				if (flag)
				{
					goto IL_71;
				}
				flag = ((uint)num2 + (uint)num > uint.MaxValue);
				if (!flag)
				{
					reader.x4cb534e776ee27b9 = reader.x13d4cb8d1bd20347 + num2;
				}
				if (!false)
				{
					reader.x1af9fc7847b7aa2c++;
					return new SubItemToken(num);
				}
				break;
			}
			case WireType.StartGroup:
				reader.xa5694e1c82a939b4 = WireType.None;
				goto IL_BC;
			}
			throw reader.xcc3b2c94d66a5338();
			IL_71:
			return new SubItemToken(-reader.xade3b695478596d6);
			IL_BC:
			reader.x1af9fc7847b7aa2c++;
			goto IL_71;
		}

		public int ReadFieldHeader()
		{
			if (this.x4cb534e776ee27b9 > this.x13d4cb8d1bd20347)
			{
				if (255 == 0)
				{
					goto IL_8D;
				}
				if (this.xa5694e1c82a939b4 == WireType.EndGroup)
				{
					return 0;
				}
				if (!false)
				{
					goto IL_8D;
				}
				goto IL_3F;
				IL_16:
				if (this.xa5694e1c82a939b4 == WireType.EndGroup)
				{
					return 0;
				}
				return this.xade3b695478596d6;
				IL_3F:
				this.xa5694e1c82a939b4 = WireType.None;
				uint num;
				bool flag = (num | 2U) == 0U;
				if (!flag)
				{
					this.xade3b695478596d6 = 0;
					goto IL_16;
				}
				IL_5D:
				this.xade3b695478596d6 = (int)(num >> 3);
				if (!false && this.xade3b695478596d6 >= 1)
				{
					goto IL_16;
				}
				throw new ProtoException("Invalid field in source data: " + this.xade3b695478596d6);
				IL_8D:
				if (!this.xb5c9aba8e230d14a(out num))
				{
					goto IL_3F;
				}
				this.xa5694e1c82a939b4 = (WireType)(num & 7U);
				goto IL_5D;
			}
			return 0;
		}

		public bool TryReadFieldHeader(int field)
		{
			if (this.x4cb534e776ee27b9 > this.x13d4cb8d1bd20347)
			{
				int num;
				uint num2;
				bool flag = (uint)num + num2 < 0U;
				if (flag)
				{
					goto IL_A2;
				}
				while (this.xa5694e1c82a939b4 != WireType.EndGroup)
				{
					num = this.xfbd3c47b33674661(false, out num2);
					if ((uint)num - (uint)num <= 4294967295U)
					{
						if (num <= 0)
						{
							return false;
						}
						if ((int)num2 >> 3 != field)
						{
							goto IL_4F;
						}
						WireType wireType;
						if ((wireType = (WireType)(num2 & 7U)) != WireType.EndGroup)
						{
							this.xa5694e1c82a939b4 = wireType;
							this.xade3b695478596d6 = field;
							goto IL_A2;
						}
						if (false)
						{
							goto IL_54;
						}
						return false;
					}
				}
				return false;
				IL_4F:
				return false;
				IL_54:
				if (num2 + num2 < 0U)
				{
					goto IL_4F;
				}
				flag = (((uint)num & 0U) == 0U);
				if (flag)
				{
					this.xadf615803d4cf295 -= num;
					return true;
				}
				IL_90:
				this.x8d48119452c041bd += num;
				goto IL_54;
				IL_A2:
				this.x13d4cb8d1bd20347 += num;
				goto IL_90;
			}
			return false;
		}

		public TypeModel Model
		{
			get
			{
				return this.xad70a5849826ecef;
			}
		}

		public void Hint(WireType wireType)
		{
			if (this.xa5694e1c82a939b4 == wireType)
			{
				return;
			}
			if ((wireType & (WireType)7) == this.xa5694e1c82a939b4)
			{
				this.xa5694e1c82a939b4 = wireType;
			}
		}

		public void Assert(WireType wireType)
		{
			if (this.xa5694e1c82a939b4 == wireType)
			{
				return;
			}
			if ((wireType & (WireType)7) != this.xa5694e1c82a939b4)
			{
				throw this.xcc3b2c94d66a5338();
			}
			this.xa5694e1c82a939b4 = wireType;
		}

		public void SkipField()
		{
			WireType wireType = this.xa5694e1c82a939b4;
			int i;
			int num;
			int num2;
			bool flag;
			for (;;)
			{
				switch (wireType)
				{
				case WireType.Variant:
				case WireType.SignedVariant:
					goto IL_BD;
				case WireType.Fixed64:
					goto IL_23C;
				case WireType.String:
					goto IL_1F0;
				case WireType.StartGroup:
					goto IL_97;
				case WireType.Fixed32:
					do
					{
						if (this.xadf615803d4cf295 >= 4)
						{
							if ((uint)i - (uint)num >= 0U)
							{
								break;
							}
						}
						else
						{
							this.xae7813dfd13745c1(4, true);
							if (false)
							{
								goto IL_23C;
							}
							flag = ((uint)num2 + (uint)num2 > uint.MaxValue);
							if (flag)
							{
								continue;
							}
						}
					}
					while ((uint)num > 4294967295U);
					IL_264:
					this.xadf615803d4cf295 -= 4;
					this.x8d48119452c041bd += 4;
					this.x13d4cb8d1bd20347 += 4;
					if (2 == 0)
					{
						continue;
					}
					return;
					goto IL_264;
				}
				goto Block_16;
			}
			IL_97:
			num2 = this.xade3b695478596d6;
			flag = ((uint)num + (uint)i > uint.MaxValue);
			if (!flag)
			{
				flag = ((uint)num - (uint)i > uint.MaxValue);
				if (flag)
				{
					goto IL_1BA;
				}
				IL_65:
				while (this.ReadFieldHeader() > 0)
				{
					this.SkipField();
				}
				if (this.xa5694e1c82a939b4 == WireType.EndGroup)
				{
					flag = ((uint)num - (uint)i < 0U);
					if (flag)
					{
						goto IL_169;
					}
					goto IL_1BA;
				}
				IL_15:
				throw this.xcc3b2c94d66a5338();
				IL_1BA:
				flag = ((uint)num + (uint)num2 > uint.MaxValue);
				if (flag)
				{
					if (false)
					{
						goto IL_65;
					}
				}
				else if (this.xade3b695478596d6 != num2)
				{
					goto IL_15;
				}
				this.xa5694e1c82a939b4 = WireType.None;
				return;
			}
			if (!false)
			{
				if ((uint)num - (uint)i <= 4294967295U)
				{
					goto IL_E0;
				}
				goto IL_20E;
			}
			IL_B9:
			if (!false)
			{
				return;
			}
			goto IL_97;
			IL_BD:
			this.x118a4e0bd4a688a2();
			return;
			IL_E0:
			if (this.x01bba5daeb6e30a8)
			{
				while (i <= this.x09e8e06d4b0600f4)
				{
					if ((uint)num2 + (uint)num2 < 0U)
					{
						if ((uint)num2 < 0U)
						{
							continue;
						}
					}
					this.x09e8e06d4b0600f4 -= i;
					goto IL_E8;
				}
				throw ProtoReader.xa1450a2e84b3d41f(this);
			}
			IL_E8:
			ProtoReader.x0c66d9eb79cc002f(this.x337e217cb3ba0627, i, this.x8b3e45f7763ede93);
			goto IL_B9;
			IL_169:
			this.x8d48119452c041bd += i;
			this.x13d4cb8d1bd20347 += i;
			return;
			IL_1F0:
			i = (int)this.xeb5ebf9f61661623(false);
			if ((uint)num < 0U)
			{
				return;
			}
			if (i <= this.xadf615803d4cf295)
			{
				this.xadf615803d4cf295 -= i;
				goto IL_330;
			}
			this.x13d4cb8d1bd20347 += i;
			i -= this.xadf615803d4cf295;
			num = (this.xadf615803d4cf295 = 0);
			this.x8d48119452c041bd = num;
			goto IL_E0;
			IL_20E:
			this.xadf615803d4cf295 -= 8;
			this.x8d48119452c041bd += 8;
			this.x13d4cb8d1bd20347 += 8;
			return;
			IL_23C:
			if (this.xadf615803d4cf295 < 8)
			{
				this.xae7813dfd13745c1(8, true);
				goto IL_20E;
			}
			flag = ((uint)i > uint.MaxValue);
			if (flag)
			{
				goto IL_330;
			}
			goto IL_20E;
			Block_16:
			goto IL_335;
			IL_330:
			goto IL_169;
			IL_335:
			throw this.xcc3b2c94d66a5338();
		}

		public ulong ReadUInt64()
		{
			WireType wireType = this.xa5694e1c82a939b4;
			int num;
			if ((uint)num >= 0U)
			{
			}
			int num2;
			switch (wireType)
			{
			case WireType.Variant:
				return this.x118a4e0bd4a688a2();
			case WireType.Fixed64:
				if (this.xadf615803d4cf295 < 8)
				{
					this.xae7813dfd13745c1(8, true);
				}
				break;
			default:
				if ((uint)num2 <= 4294967295U)
				{
					if (wireType != WireType.Fixed32)
					{
						throw this.xcc3b2c94d66a5338();
					}
					return (ulong)this.ReadUInt32();
				}
				break;
			}
			this.x13d4cb8d1bd20347 += 8;
			this.xadf615803d4cf295 -= 8;
			ulong num3 = (ulong)this.x8b3e45f7763ede93[this.x8d48119452c041bd++] | (ulong)this.x8b3e45f7763ede93[this.x8d48119452c041bd++] << 8 | (ulong)this.x8b3e45f7763ede93[this.x8d48119452c041bd++] << 16 | (ulong)this.x8b3e45f7763ede93[this.x8d48119452c041bd++] << 24;
			byte[] array = this.x8b3e45f7763ede93;
			num2 = this.x8d48119452c041bd++;
			ulong num4 = num3 | array[num2] << 32 | (ulong)this.x8b3e45f7763ede93[this.x8d48119452c041bd++] << 40 | (ulong)this.x8b3e45f7763ede93[this.x8d48119452c041bd++] << 48;
			byte[] array2 = this.x8b3e45f7763ede93;
			num = this.x8d48119452c041bd++;
			return num4 | array2[num] << 56;
		}

		public unsafe float ReadSingle()
		{
			WireType wireType = this.xa5694e1c82a939b4;
			double num;
			int num2;
			for (;;)
			{
				bool flag = (uint)num - (uint)num < 0U;
				if (!flag)
				{
					goto IL_0C;
				}
				IL_10:
				if (wireType == WireType.Fixed32)
				{
					goto IL_B0;
				}
				if (((uint)num2 & 0U) == 0U)
				{
					goto IL_BC;
				}
				if (((uint)num & 0U) == 0U)
				{
					continue;
				}
				IL_0C:
				if (wireType != WireType.Fixed64)
				{
					goto IL_10;
				}
				break;
			}
			num = this.ReadDouble();
			float num3 = (float)num;
			if (x479f2661aae93792.xa696f2cecd0cd4ae(num3))
			{
				if (!x479f2661aae93792.xa696f2cecd0cd4ae(num))
				{
					throw ProtoReader.xe0800720badf45f1(new OverflowException(), this);
				}
			}
			return num3;
			IL_B0:
			num2 = this.ReadInt32();
			return *(float*)(&num2);
			IL_BC:
			throw this.xcc3b2c94d66a5338();
		}

		public bool ReadBoolean()
		{
			switch (this.ReadUInt32())
			{
			case 0U:
				return false;
			case 1U:
				return true;
			default:
				throw this.x98bdea680591defc("Unexpected boolean value");
			}
		}

		public static byte[] AppendBytes(byte[] value, ProtoReader reader)
		{
			WireType wireType = reader.xa5694e1c82a939b4;
			if (wireType != WireType.String)
			{
				throw reader.xcc3b2c94d66a5338();
			}
			int i = (int)reader.xeb5ebf9f61661623(false);
			reader.xa5694e1c82a939b4 = WireType.None;
			if (i != 0)
			{
				int num;
				if (value == null)
				{
					if ((uint)num <= 4294967295U)
					{
						goto IL_198;
					}
					goto IL_F3;
				}
				else
				{
					if (value.Length == 0)
					{
						goto IL_198;
					}
					goto IL_FF;
				}
				return value;
				IL_A5:
				reader.x13d4cb8d1bd20347 += i;
				int num2;
				while (i > reader.xadf615803d4cf295)
				{
					if (reader.xadf615803d4cf295 > 0)
					{
						x479f2661aae93792.x6a87193e5bb23362(reader.x8b3e45f7763ede93, reader.x8d48119452c041bd, value, num2, reader.xadf615803d4cf295);
						i -= reader.xadf615803d4cf295;
						goto IL_E3;
					}
					IL_88:
					int num3 = (i <= reader.x8b3e45f7763ede93.Length) ? i : reader.x8b3e45f7763ede93.Length;
					if (-2147483648 != 0)
					{
						if (num3 <= 0)
						{
							continue;
						}
						reader.xae7813dfd13745c1(num3, true);
						continue;
					}
					IL_E3:
					num2 += reader.xadf615803d4cf295;
					num = (reader.xadf615803d4cf295 = 0);
					reader.x8d48119452c041bd = num;
					goto IL_88;
				}
				if (i > 0)
				{
					x479f2661aae93792.x6a87193e5bb23362(reader.x8b3e45f7763ede93, reader.x8d48119452c041bd, value, num2, i);
					reader.x8d48119452c041bd += i;
					reader.xadf615803d4cf295 -= i;
					return value;
				}
				return value;
				IL_F3:
				byte[] array;
				value = array;
				goto IL_A5;
				IL_FF:
				num2 = value.Length;
				IL_103:
				array = new byte[value.Length + i];
				x479f2661aae93792.x6a87193e5bb23362(value, 0, array, 0, value.Length);
				goto IL_F3;
				IL_198:
				num2 = 0;
				bool flag = (uint)num - (uint)i > uint.MaxValue;
				if (!flag)
				{
					value = new byte[i];
					if (((uint)i | 4U) == 0U)
					{
						return value;
					}
					if ((uint)i - (uint)num < 0U)
					{
						goto IL_FF;
					}
					if ((uint)i + (uint)num2 >= 0U)
					{
						goto IL_A5;
					}
					goto IL_103;
				}
			}
			if (value == null)
			{
				return ProtoReader.xe2b8a9037298dea1;
			}
			return value;
		}

		private static byte[] x0f6807cca84a8e5b(Stream xcf18e5243f8d5fd3, int x961016a387451f05)
		{
			bool flag;
			int num;
			byte[] array;
			int num2;
			if (xcf18e5243f8d5fd3 == null)
			{
				flag = (((uint)x961016a387451f05 | 2147483647U) == 0U);
				if (!flag)
				{
					goto IL_F7;
				}
				if (((uint)num & 0U) == 0U)
				{
					goto IL_6C;
				}
				flag = ((uint)x961016a387451f05 - (uint)x961016a387451f05 > uint.MaxValue);
				if (flag)
				{
					goto IL_50;
				}
				return array;
			}
			else
			{
				if (x961016a387451f05 < 0)
				{
					throw new ArgumentOutOfRangeException("length");
				}
				array = new byte[x961016a387451f05];
				num = 0;
				flag = ((uint)x961016a387451f05 + (uint)num2 < 0U);
				if (flag)
				{
					goto IL_F7;
				}
				if ((uint)num - (uint)x961016a387451f05 > 4294967295U)
				{
					goto IL_8E;
				}
			}
			IL_6E:
			while (x961016a387451f05 > 0)
			{
				if ((num2 = xcf18e5243f8d5fd3.Read(array, num, x961016a387451f05)) <= 0)
				{
					if (!false)
					{
						goto IL_6C;
					}
					break;
				}
				else
				{
					x961016a387451f05 -= num2;
				}
			}
			IL_50:
			if (x961016a387451f05 > 0)
			{
				goto IL_8E;
			}
			flag = (((uint)num2 | 4U) == 0U);
			if (!flag)
			{
				return array;
			}
			IL_6C:
			if ((uint)num2 - (uint)x961016a387451f05 >= 0U)
			{
				goto IL_50;
			}
			goto IL_6E;
			IL_8E:
			throw ProtoReader.xa1450a2e84b3d41f(null);
			IL_F7:
			throw new ArgumentNullException("stream");
		}

		private static int xc1c4b08d280d4bc4(Stream x337e217cb3ba0627)
		{
			int num = x337e217cb3ba0627.ReadByte();
			if (num >= 0)
			{
				return num;
			}
			throw ProtoReader.xa1450a2e84b3d41f(null);
		}

		public static int ReadLengthPrefix(Stream source, bool expectHeader, PrefixStyle style, out int fieldNumber)
		{
			int num;
			return ProtoReader.ReadLengthPrefix(source, expectHeader, style, out fieldNumber, out num);
		}

		public static int DirectReadLittleEndianInt32(Stream source)
		{
			return ProtoReader.xc1c4b08d280d4bc4(source) | ProtoReader.xc1c4b08d280d4bc4(source) << 8 | ProtoReader.xc1c4b08d280d4bc4(source) << 16 | ProtoReader.xc1c4b08d280d4bc4(source) << 24;
		}

		public static int DirectReadBigEndianInt32(Stream source)
		{
			return ProtoReader.xc1c4b08d280d4bc4(source) << 24 | ProtoReader.xc1c4b08d280d4bc4(source) << 16 | ProtoReader.xc1c4b08d280d4bc4(source) << 8 | ProtoReader.xc1c4b08d280d4bc4(source);
		}

		public static int DirectReadVarintInt32(Stream source)
		{
			uint result;
			int num = ProtoReader.xb5c9aba8e230d14a(source, out result);
			if (num > 0)
			{
				return (int)result;
			}
			throw ProtoReader.xa1450a2e84b3d41f(null);
		}

		public static void DirectReadBytes(Stream source, byte[] buffer, int offset, int count)
		{
			for (;;)
			{
				IL_02:
				if (count <= 0)
				{
					goto IL_11;
				}
				IL_2D:
				int num;
				bool flag;
				while ((num = source.Read(buffer, offset, count)) > 0)
				{
					count -= num;
					offset += num;
					flag = ((uint)num - (uint)count < 0U);
					if (!flag)
					{
						goto IL_02;
					}
					if ((uint)count > 4294967295U)
					{
						break;
					}
				}
				IL_11:
				if (count > 0)
				{
					break;
				}
				flag = ((uint)offset + (uint)count > uint.MaxValue);
				if (flag)
				{
					goto IL_2D;
				}
				return;
			}
			throw ProtoReader.xa1450a2e84b3d41f(null);
		}

		public static byte[] DirectReadBytes(Stream source, int count)
		{
			byte[] array = new byte[count];
			ProtoReader.DirectReadBytes(source, array, 0, count);
			return array;
		}

		public static string DirectReadString(Stream source, int length)
		{
			byte[] array = new byte[length];
			ProtoReader.DirectReadBytes(source, array, 0, length);
			return Encoding.UTF8.GetString(array, 0, length);
		}

		public static int ReadLengthPrefix(Stream source, bool expectHeader, PrefixStyle style, out int fieldNumber, out int bytesRead)
		{
			fieldNumber = 0;
			int num;
			uint num2;
			bool flag = (uint)num - num2 < 0U;
			if (!flag)
			{
				int num3;
				for (;;)
				{
					switch (style)
					{
					case PrefixStyle.None:
						goto IL_213;
					case PrefixStyle.Base128:
						bytesRead = 0;
						while ((uint)num3 >= 0U)
						{
							if (!expectHeader)
							{
								goto IL_E2;
							}
							num3 = ProtoReader.xb5c9aba8e230d14a(source, out num2);
							bytesRead += num3;
							flag = (((uint)num3 & 0U) == 0U);
							if (!flag)
							{
								goto IL_B9;
							}
							if (((expectHeader ? 1U : 0U) | 2147483647U) != 0U)
							{
								if (num3 <= 0)
								{
									goto IL_DC;
								}
								if (false)
								{
									return (int)num2;
								}
								if ((num2 & 7U) != 2U)
								{
									goto IL_170;
								}
							}
							fieldNumber = (int)(num2 >> 3);
							num3 = ProtoReader.xb5c9aba8e230d14a(source, out num2);
							flag = (num2 > uint.MaxValue);
							if (!flag)
							{
								goto IL_20A;
							}
						}
						continue;
					case PrefixStyle.Fixed32:
						goto IL_CA;
					case PrefixStyle.Fixed32BigEndian:
						goto IL_76;
					}
					goto Block_15;
				}
				IL_29:
				int num4;
				if (num4 < 0)
				{
					bytesRead = 0;
					return -1;
				}
				bytesRead = 4;
				IL_31:
				return num4 << 24 | ProtoReader.xc1c4b08d280d4bc4(source) << 16 | ProtoReader.xc1c4b08d280d4bc4(source) << 8 | ProtoReader.xc1c4b08d280d4bc4(source);
				IL_76:
				num4 = source.ReadByte();
				goto IL_29;
				IL_B9:
				bytesRead += num3;
				if (bytesRead < 0)
				{
					return -1;
				}
				return (int)num2;
				IL_CA:
				num = source.ReadByte();
				flag = (num2 - num2 < 0U);
				if (!flag)
				{
					if (num < 0)
					{
						bytesRead = 0;
						if (false)
						{
							flag = ((uint)num3 + (uint)num3 > uint.MaxValue);
							if (flag)
							{
								goto IL_B9;
							}
							goto IL_124;
						}
					}
					else
					{
						bytesRead = 4;
						flag = ((uint)num4 - (uint)num4 < 0U);
						if (flag)
						{
							goto IL_255;
						}
						if (8 != 0)
						{
							return num | ProtoReader.xc1c4b08d280d4bc4(source) << 8 | ProtoReader.xc1c4b08d280d4bc4(source) << 16 | ProtoReader.xc1c4b08d280d4bc4(source) << 24;
						}
						if (255 != 0)
						{
							goto IL_29;
						}
						goto IL_31;
					}
				}
				return -1;
				IL_DC:
				bytesRead = 0;
				return -1;
				IL_E2:
				num3 = ProtoReader.xb5c9aba8e230d14a(source, out num2);
				flag = ((uint)num4 + (uint)num3 < 0U);
				if (!flag)
				{
					flag = ((uint)num3 - (uint)num4 > uint.MaxValue);
					if (flag)
					{
						goto IL_124;
					}
					goto IL_B9;
				}
				IL_103:
				throw ProtoReader.xa1450a2e84b3d41f(null);
				IL_124:
				bytesRead += num3;
				if (bytesRead != 0)
				{
					return (int)num2;
				}
				goto IL_103;
				IL_170:
				throw new InvalidOperationException();
				IL_20A:
				goto IL_124;
				IL_213:
				bytesRead = 0;
				if (num2 >= 0U)
				{
					return int.MaxValue;
				}
				goto IL_124;
				Block_15:;
			}
			IL_255:
			throw new ArgumentOutOfRangeException("style");
		}

		private static int xb5c9aba8e230d14a(Stream x337e217cb3ba0627, out uint xbcea506a33cf9111)
		{
			xbcea506a33cf9111 = 0U;
			int i;
			bool flag = (uint)i + (uint)i > uint.MaxValue;
			if (!flag)
			{
				i = x337e217cb3ba0627.ReadByte();
				if (i >= 0)
				{
					xbcea506a33cf9111 = (uint)i;
					if (!false)
					{
						flag = ((uint)i > uint.MaxValue);
						if (flag || (xbcea506a33cf9111 & 128U) == 0U)
						{
							return 1;
						}
						xbcea506a33cf9111 &= 127U;
						if ((uint)i - (uint)i < 0U)
						{
							goto IL_124;
						}
						if ((uint)i - (uint)i >= 0U)
						{
							i = x337e217cb3ba0627.ReadByte();
						}
						if (i >= 0)
						{
							goto IL_124;
						}
						IL_117:
						throw ProtoReader.xa1450a2e84b3d41f(null);
						IL_124:
						xbcea506a33cf9111 |= (uint)((uint)(i & 127) << 7);
						if ((i & 128) == 0)
						{
							return 2;
						}
						i = x337e217cb3ba0627.ReadByte();
						while (i >= 0)
						{
							for (;;)
							{
								IL_E2:
								xbcea506a33cf9111 |= (uint)((uint)(i & 127) << 14);
								IL_BB:
								while ((i & 128) != 0)
								{
									for (;;)
									{
										i = x337e217cb3ba0627.ReadByte();
										flag = ((uint)i + (uint)i > uint.MaxValue);
										if (flag || i < 0)
										{
											goto IL_7F;
										}
										xbcea506a33cf9111 |= (uint)((uint)(i & 127) << 21);
										if ((i & 128) == 0)
										{
											return 4;
										}
										i = x337e217cb3ba0627.ReadByte();
										if (i < 0)
										{
											goto IL_53;
										}
										xbcea506a33cf9111 |= (uint)((uint)i << 28);
										if (!false)
										{
											break;
										}
										if (!false)
										{
											goto IL_C6;
										}
										if ((uint)i < 0U)
										{
											goto IL_BB;
										}
									}
									if (-2147483648 != 0)
									{
										goto Block_2;
									}
									goto IL_E2;
								}
								return 3;
							}
							IL_C6:
							if ((uint)i + (uint)i >= 0U)
							{
								continue;
							}
							goto IL_117;
							Block_2:
							if ((i & 240) != 0)
							{
								throw new OverflowException();
							}
							return 5;
							IL_7F:
							throw ProtoReader.xa1450a2e84b3d41f(null);
						}
						throw ProtoReader.xa1450a2e84b3d41f(null);
					}
					IL_53:
					throw ProtoReader.xa1450a2e84b3d41f(null);
				}
				return 0;
			}
			return 2;
		}

		internal static void x0c66d9eb79cc002f(Stream x337e217cb3ba0627, int x10f4d88af727adbc, byte[] x5cafa8d49ea71ea1)
		{
			if (x337e217cb3ba0627.CanSeek)
			{
				goto IL_1CA;
			}
			if (x5cafa8d49ea71ea1 == null)
			{
				goto IL_29;
			}
			goto IL_187;
			IL_15:
			if (x10f4d88af727adbc <= 0)
			{
				return;
			}
			goto IL_B4;
			IL_29:
			x5cafa8d49ea71ea1 = xe2887184ca5e62c2.x7145b88c884afa7f();
			int num;
			bool flag;
			try
			{
				for (;;)
				{
					if (x10f4d88af727adbc > x5cafa8d49ea71ea1.Length)
					{
						if ((num = x337e217cb3ba0627.Read(x5cafa8d49ea71ea1, 0, x5cafa8d49ea71ea1.Length)) > 0)
						{
							goto IL_96;
						}
					}
					IL_37:
					while (x10f4d88af727adbc > 0)
					{
						if ((num = x337e217cb3ba0627.Read(x5cafa8d49ea71ea1, 0, x10f4d88af727adbc)) <= 0)
						{
							goto Block_18;
						}
						x10f4d88af727adbc -= num;
					}
					int num2;
					flag = ((uint)num2 > uint.MaxValue);
					if (!flag)
					{
						break;
					}
					if ((uint)num - (uint)num2 < 0U)
					{
						goto IL_65;
					}
					IL_96:
					x10f4d88af727adbc -= num;
					continue;
					IL_65:
					goto IL_37;
				}
				Block_18:
				goto IL_15;
			}
			finally
			{
				xe2887184ca5e62c2.xb8b95ec3e3f43efa(ref x5cafa8d49ea71ea1);
			}
			IL_B4:
			throw ProtoReader.xa1450a2e84b3d41f(null);
			IL_187:
			for (;;)
			{
				if (x10f4d88af727adbc > x5cafa8d49ea71ea1.Length)
				{
					goto IL_19B;
				}
				IL_11F:
				int num2;
				while (x10f4d88af727adbc > 0)
				{
					if ((uint)num2 < 0U)
					{
						goto IL_0D;
					}
					if ((num2 = x337e217cb3ba0627.Read(x5cafa8d49ea71ea1, 0, x10f4d88af727adbc)) <= 0)
					{
						flag = (((uint)num | 4294967294U) == 0U);
						if (flag)
						{
							goto Block_10;
						}
						if (((uint)x10f4d88af727adbc | 15U) == 0U)
						{
							goto IL_29;
						}
						if ((uint)x10f4d88af727adbc + (uint)num2 >= 0U)
						{
							break;
						}
						goto IL_18C;
					}
					else
					{
						x10f4d88af727adbc -= num2;
					}
				}
				goto IL_15;
				IL_18C:
				x10f4d88af727adbc -= num2;
				if (15 != 0)
				{
					continue;
				}
				IL_19B:
				if ((num2 = x337e217cb3ba0627.Read(x5cafa8d49ea71ea1, 0, x5cafa8d49ea71ea1.Length)) <= 0)
				{
					goto IL_11F;
				}
				goto IL_18C;
			}
			IL_0D:
			if (!false)
			{
				return;
			}
			goto IL_15;
			Block_10:
			flag = (((uint)num | 4294967294U) == 0U);
			if (!flag)
			{
				flag = ((uint)num > uint.MaxValue);
				if (flag)
				{
					goto IL_1CA;
				}
				goto IL_29;
			}
			IL_1B0:
			x10f4d88af727adbc = 0;
			goto IL_15;
			IL_1CA:
			x337e217cb3ba0627.Seek((long)x10f4d88af727adbc, SeekOrigin.Current);
			if (!false)
			{
				goto IL_1B0;
			}
			goto IL_187;
		}

		internal static Exception xe0800720badf45f1(Exception xc3c70767499bc99a, ProtoReader x337e217cb3ba0627)
		{
			if (xc3c70767499bc99a != null)
			{
				while (x337e217cb3ba0627 != null)
				{
					if (!xc3c70767499bc99a.Data.Contains("protoSource"))
					{
						xc3c70767499bc99a.Data.Add("protoSource", string.Format("tag={0}; wire-type={1}; offset={2}; depth={3}", new object[]
						{
							x337e217cb3ba0627.xade3b695478596d6,
							x337e217cb3ba0627.xa5694e1c82a939b4,
							x337e217cb3ba0627.x13d4cb8d1bd20347,
							x337e217cb3ba0627.x1af9fc7847b7aa2c
						}));
						break;
					}
					if (!false)
					{
						break;
					}
				}
			}
			return xc3c70767499bc99a;
		}

		private static Exception xa1450a2e84b3d41f(ProtoReader x337e217cb3ba0627)
		{
			return ProtoReader.xe0800720badf45f1(new EndOfStreamException(), x337e217cb3ba0627);
		}

		public void AppendExtensionData(IExtensible instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			IExtension extensionObject = instance.GetExtensionObject(true);
			bool commit = false;
			Stream stream = extensionObject.BeginAppend();
			try
			{
				using (ProtoWriter protoWriter = new ProtoWriter(stream, this.xad70a5849826ecef, null))
				{
					this.x64a41f907e78ab5e(protoWriter);
					protoWriter.Close();
				}
				commit = true;
			}
			finally
			{
				extensionObject.EndAppend(stream, commit);
			}
		}

		private void x64a41f907e78ab5e(ProtoWriter xbdfb620b7167944b)
		{
			ProtoWriter.WriteFieldHeader(this.xade3b695478596d6, this.xa5694e1c82a939b4, xbdfb620b7167944b);
			for (;;)
			{
				switch (this.xa5694e1c82a939b4)
				{
				case WireType.Variant:
				case WireType.Fixed64:
				case WireType.SignedVariant:
					goto IL_4A;
				case WireType.String:
					ProtoWriter.WriteBytes(ProtoReader.AppendBytes(null, this), xbdfb620b7167944b);
					if (false)
					{
						continue;
					}
					return;
				case WireType.StartGroup:
					goto IL_1F;
				case WireType.Fixed32:
					goto IL_98;
				}
				goto Block_2;
			}
			IL_1F:
			SubItemToken token = ProtoReader.StartSubItem(this);
			SubItemToken token2 = ProtoWriter.StartSubItem(null, xbdfb620b7167944b);
			while (this.ReadFieldHeader() > 0)
			{
				this.x64a41f907e78ab5e(xbdfb620b7167944b);
			}
			ProtoReader.EndSubItem(token, this);
			ProtoWriter.EndSubItem(token2, xbdfb620b7167944b);
			return;
			IL_4A:
			ProtoWriter.WriteInt64(this.ReadInt64(), xbdfb620b7167944b);
			return;
			Block_2:
			goto IL_B3;
			IL_98:
			ProtoWriter.WriteInt32(this.ReadInt32(), xbdfb620b7167944b);
			return;
			IL_B3:
			throw this.xcc3b2c94d66a5338();
		}

		public static bool HasSubValue(WireType wireType, ProtoReader source)
		{
			if (source.x4cb534e776ee27b9 > source.x13d4cb8d1bd20347)
			{
				if (!false)
				{
					if (-1 != 0)
					{
						goto IL_2A;
					}
				}
				if (false)
				{
					goto IL_2A;
				}
				IL_14:
				source.xa5694e1c82a939b4 = wireType;
				return true;
				IL_2A:
				if (wireType != WireType.EndGroup)
				{
					goto IL_14;
				}
			}
			return false;
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

		internal Type x34a9527dfd927b35(string xbcea506a33cf9111)
		{
			return TypeModel.x34a9527dfd927b35(this.xad70a5849826ecef, xbcea506a33cf9111);
		}

		internal void xf4032809c9332461(object xbcea506a33cf9111)
		{
			this.x49b766214fe3dda0.x24ab034b13a4cf87(0, xbcea506a33cf9111);
			this.xaecea4487d2e2f1b -= 1U;
		}

		public static void NoteObject(object value, ProtoReader reader)
		{
			if (reader.xaecea4487d2e2f1b != 0U)
			{
				reader.x49b766214fe3dda0.xa335b28bb552e966(value);
				reader.xaecea4487d2e2f1b -= 1U;
			}
		}

		public Type ReadType()
		{
			return TypeModel.x34a9527dfd927b35(this.xad70a5849826ecef, this.ReadString());
		}

		internal void x2605289f9758acf8(int xa37be32b8baa234f)
		{
			this.xaecea4487d2e2f1b += 1U;
			this.x49b766214fe3dda0.x24ab034b13a4cf87(xa37be32b8baa234f, null);
		}

		internal void x24f0af6d03e81cc5()
		{
			if (!this.x01bba5daeb6e30a8 || this.x09e8e06d4b0600f4 == 0)
			{
				return;
			}
			throw new ProtoException("Incorrect number of bytes consumed");
		}

		private const long x7510c2f45e460eb9 = -9223372036854775808L;

		private const int x1aff58625664574c = -2147483648;

		private Stream x337e217cb3ba0627;

		private byte[] x8b3e45f7763ede93;

		private TypeModel xad70a5849826ecef;

		private int xade3b695478596d6;

		private WireType xa5694e1c82a939b4 = WireType.None;

		private int x09e8e06d4b0600f4;

		private readonly bool x01bba5daeb6e30a8;

		private bool x48c2a32eaf094d8a = true;

		private readonly SerializationContext x0f7b23d1c393aed9;

		private int x8d48119452c041bd;

		private int x13d4cb8d1bd20347;

		private int xadf615803d4cf295;

		private Dictionary<string, string> x7c1c2b8bd447c71a;

		private static readonly UTF8Encoding xff3edc9aa5f0523b = new UTF8Encoding();

		private int x1af9fc7847b7aa2c;

		private int x4cb534e776ee27b9 = int.MaxValue;

		private static readonly byte[] xe2b8a9037298dea1 = new byte[0];

		private readonly x15bd90f59150b4e2 x49b766214fe3dda0 = new x15bd90f59150b4e2();

		private uint xaecea4487d2e2f1b = 1U;
	}
}
