using System;

namespace ProtoBuf
{
	public static class BclHelpers
	{
		public static object GetUninitializedObject(Type type)
		{
			throw new NotSupportedException("Constructor-skipping is not supported on this platform");
		}

		public static void WriteTimeSpan(TimeSpan timeSpan, ProtoWriter dest)
		{
			WireType x58ca9db3d85c761f = dest.x58ca9db3d85c761f;
			long num;
			if (((uint)num | 3U) == 0U)
			{
				goto IL_1AD;
			}
			if (false)
			{
				goto IL_225;
			}
			goto IL_1C8;
			IL_22:
			x87ada6a502ee9bc0 x87ada6a502ee9bc;
			bool flag;
			if (x87ada6a502ee9bc == x87ada6a502ee9bc0.xafdb6194aad2cca0)
			{
				flag = (((uint)num | 3U) == 0U);
				if (flag)
				{
					goto IL_C5;
				}
			}
			else
			{
				ProtoWriter.WriteFieldHeader(2, WireType.Variant, dest);
				if (false)
				{
					goto IL_10E;
				}
				ProtoWriter.WriteInt32((int)x87ada6a502ee9bc, dest);
			}
			SubItemToken token;
			ProtoWriter.EndSubItem(token, dest);
			return;
			IL_9E:
			token = ProtoWriter.StartSubItem(null, dest);
			if (num != 0L)
			{
				goto IL_E5;
			}
			goto IL_22;
			IL_B6:
			num /= 10000L;
			goto IL_9E;
			IL_C5:
			if (num % 10000000L == 0L)
			{
				goto IL_FC;
			}
			if (((uint)num & 0U) == 0U)
			{
				if (num % 10000L == 0L)
				{
					x87ada6a502ee9bc = x87ada6a502ee9bc0.xe227b5941b24fbbe;
					goto IL_B6;
				}
				x87ada6a502ee9bc = x87ada6a502ee9bc0.xcbcf7fcb8d5bcf77;
				goto IL_9E;
			}
			IL_E5:
			if (4 != 0)
			{
				ProtoWriter.WriteFieldHeader(1, WireType.SignedVariant, dest);
				if ((uint)num + (uint)num <= 4294967295U)
				{
					ProtoWriter.WriteInt64(num, dest);
					goto IL_22;
				}
				goto IL_1AD;
			}
			IL_FC:
			x87ada6a502ee9bc = x87ada6a502ee9bc0.x9187c98651fff1b6;
			num /= 10000000L;
			goto IL_9E;
			IL_10E:
			num /= 600000000L;
			goto IL_9E;
			IL_171:
			num /= 864000000000L;
			goto IL_9E;
			IL_195:
			num = -1L;
			x87ada6a502ee9bc = x87ada6a502ee9bc0.x353abc6637d88c69;
			goto IL_9E;
			IL_1AD:
			if (false)
			{
				goto IL_171;
			}
			flag = (((uint)num | 2U) == 0U);
			if (!flag)
			{
				goto IL_225;
			}
			IL_1C8:
			switch (x58ca9db3d85c761f)
			{
			case WireType.Fixed64:
				ProtoWriter.WriteInt64(timeSpan.Ticks, dest);
				return;
			case WireType.String:
			case WireType.StartGroup:
				num = timeSpan.Ticks;
				if (4 != 0)
				{
					if (timeSpan == TimeSpan.MaxValue)
					{
						num = 1L;
						x87ada6a502ee9bc = x87ada6a502ee9bc0.x353abc6637d88c69;
						goto IL_9E;
					}
					if (timeSpan == TimeSpan.MinValue)
					{
						break;
					}
					if (num % 864000000000L == 0L)
					{
						x87ada6a502ee9bc = x87ada6a502ee9bc0.xafdb6194aad2cca0;
						goto IL_171;
					}
				}
				if (num % 36000000000L == 0L)
				{
					x87ada6a502ee9bc = x87ada6a502ee9bc0.x6439df4c0f2066d0;
					num /= 36000000000L;
					flag = (((uint)num & 0U) == 0U);
					if (flag)
					{
						goto IL_9E;
					}
					goto IL_B6;
				}
				else
				{
					if (num % 600000000L != 0L)
					{
						goto IL_C5;
					}
					x87ada6a502ee9bc = x87ada6a502ee9bc0.x5272bbb3c67d0f74;
					goto IL_10E;
				}
				break;
			default:
				throw new ProtoException("Unexpected wire-type: " + dest.x58ca9db3d85c761f.ToString());
			}
			IL_225:
			goto IL_195;
		}

		public static TimeSpan ReadTimeSpan(ProtoReader source)
		{
			long num = BclHelpers.xd67aec02fd17e584(source);
			while (num != -9223372036854775808L)
			{
				if (((uint)num | 2147483647U) != 0U)
				{
					if (num != 9223372036854775807L)
					{
						return TimeSpan.FromTicks(num);
					}
					return TimeSpan.MaxValue;
				}
			}
			return TimeSpan.MinValue;
		}

		public static DateTime ReadDateTime(ProtoReader source)
		{
			long num = BclHelpers.xd67aec02fd17e584(source);
			if (num == -9223372036854775808L)
			{
				return DateTime.MinValue;
			}
			bool flag = (uint)num < 0U;
			if (!flag && num != 9223372036854775807L)
			{
				return BclHelpers.xfc58af0fe10a0e2e.AddTicks(num);
			}
			return DateTime.MaxValue;
		}

		public static void WriteDateTime(DateTime value, ProtoWriter dest)
		{
			WireType x58ca9db3d85c761f = dest.x58ca9db3d85c761f;
			for (;;)
			{
				TimeSpan timeSpan;
				switch (x58ca9db3d85c761f)
				{
				case WireType.String:
				case WireType.StartGroup:
					if (value == DateTime.MaxValue)
					{
						timeSpan = TimeSpan.MaxValue;
					}
					else
					{
						if (value == DateTime.MinValue)
						{
							goto IL_53;
						}
						if (255 == 0)
						{
							goto IL_5B;
						}
						timeSpan = value - BclHelpers.xfc58af0fe10a0e2e;
					}
					break;
				default:
					goto IL_5B;
				}
				IL_2D:
				BclHelpers.WriteTimeSpan(timeSpan, dest);
				if (2 == 0)
				{
					continue;
				}
				break;
				IL_53:
				timeSpan = TimeSpan.MinValue;
				goto IL_2D;
				IL_5B:
				timeSpan = value - BclHelpers.xfc58af0fe10a0e2e;
				if (3 != 0)
				{
					goto IL_2D;
				}
				goto IL_53;
			}
		}

		private static long xd67aec02fd17e584(ProtoReader x337e217cb3ba0627)
		{
			switch (x337e217cb3ba0627.WireType)
			{
			case WireType.Fixed64:
				return x337e217cb3ba0627.ReadInt64();
			case WireType.String:
			case WireType.StartGroup:
			{
				SubItemToken token = ProtoReader.StartSubItem(x337e217cb3ba0627);
				x87ada6a502ee9bc0 x87ada6a502ee9bc = x87ada6a502ee9bc0.xafdb6194aad2cca0;
				long num = 0L;
				x87ada6a502ee9bc0 x87ada6a502ee9bc2;
				for (;;)
				{
					int num2;
					while ((num2 = x337e217cb3ba0627.ReadFieldHeader()) > 0)
					{
						int num3 = num2;
						switch (num3)
						{
						case 1:
						{
							x337e217cb3ba0627.Assert(WireType.SignedVariant);
							num = x337e217cb3ba0627.ReadInt64();
							long num4;
							bool flag = (uint)num4 - (uint)num3 > uint.MaxValue;
							if (flag)
							{
								goto Block_7;
							}
							break;
						}
						case 2:
							x87ada6a502ee9bc = (x87ada6a502ee9bc0)x337e217cb3ba0627.ReadInt32();
							break;
						default:
							if ((uint)num < 0U)
							{
								goto IL_B6;
							}
							x337e217cb3ba0627.SkipField();
							break;
						}
					}
					ProtoReader.EndSubItem(token, x337e217cb3ba0627);
					x87ada6a502ee9bc2 = x87ada6a502ee9bc;
					switch (x87ada6a502ee9bc2)
					{
					case x87ada6a502ee9bc0.xafdb6194aad2cca0:
						goto IL_B6;
					case x87ada6a502ee9bc0.x6439df4c0f2066d0:
						goto IL_C2;
					case x87ada6a502ee9bc0.x5272bbb3c67d0f74:
						goto IL_CE;
					case x87ada6a502ee9bc0.x9187c98651fff1b6:
						goto IL_D7;
					case x87ada6a502ee9bc0.xe227b5941b24fbbe:
						goto IL_AD;
					case x87ada6a502ee9bc0.xcbcf7fcb8d5bcf77:
						return num;
					default:
						if (((uint)num | 3U) != 0U)
						{
							goto Block_6;
						}
						break;
					}
				}
				return num;
				IL_AD:
				return num * 10000L;
				IL_B6:
				return num * 864000000000L;
				IL_C2:
				return num * 36000000000L;
				IL_CE:
				return num * 600000000L;
				IL_D7:
				return num * 10000000L;
				IL_E0:
				throw new ProtoException("Unknown timescale: " + x87ada6a502ee9bc.ToString());
				Block_6:
				if (x87ada6a502ee9bc2 == x87ada6a502ee9bc0.x353abc6637d88c69)
				{
					long num4 = num;
					int num2;
					bool flag = (uint)num4 + (uint)num2 > uint.MaxValue;
					if ((flag || num4 <= 1L) && num4 >= -1L)
					{
						switch ((int)(num4 - -1L))
						{
						case 0:
							return long.MinValue;
						case 2:
							return long.MaxValue;
						}
					}
					throw new ProtoException("Unknown min/max value: " + num.ToString());
				}
				Block_7:
				goto IL_E0;
			}
			default:
				throw new ProtoException("Unexpected wire-type: " + x337e217cb3ba0627.WireType.ToString());
			}
		}

		public static decimal ReadDecimal(ProtoReader reader)
		{
			ulong num = 0UL;
			uint num2 = 0U;
			uint num3 = 0U;
			SubItemToken token = ProtoReader.StartSubItem(reader);
			int num5;
			byte b;
			int num6;
			int num7;
			bool isNegative;
			for (;;)
			{
				bool flag;
				for (;;)
				{
					int num4;
					if ((num4 = reader.ReadFieldHeader()) <= 0)
					{
						ProtoReader.EndSubItem(token, reader);
						while (num2 + (uint)num5 <= 4294967295U)
						{
							if (!false)
							{
								goto IL_C4;
							}
						}
						goto IL_1D;
					}
					switch (num4)
					{
					case 1:
						goto IL_111;
					case 2:
						num2 = reader.ReadUInt32();
						break;
					case 3:
						num3 = reader.ReadUInt32();
						flag = ((uint)b < 0U);
						if (flag)
						{
							goto IL_F0;
						}
						break;
					default:
						goto IL_10C;
					}
					continue;
					IL_C4:
					if (num == 0UL)
					{
						if (2147483647 == 0)
						{
							continue;
						}
						goto IL_F0;
					}
					IL_37:
					num6 = (int)(num & (ulong)-1);
					if ((uint)b < 0U)
					{
						goto IL_C4;
					}
					if (false)
					{
						goto IL_56;
					}
					num5 = (int)(num >> 32 & (ulong)-1);
					num7 = (int)num2;
					isNegative = ((num3 & 1U) == 1U);
					goto IL_1D;
					IL_F0:
					if (num3 - (uint)num7 < 0U)
					{
						goto IL_111;
					}
					flag = ((uint)num6 - num3 > uint.MaxValue);
					if (flag)
					{
						goto IL_89;
					}
					if (num2 != 0U)
					{
						goto IL_37;
					}
					goto IL_2D;
					IL_10C:
					reader.SkipField();
					continue;
					IL_89:
					flag = ((uint)num7 - num2 > uint.MaxValue);
					if (flag)
					{
						goto IL_10C;
					}
					break;
					IL_56:
					goto IL_89;
					IL_1D:
					b = (byte)((num3 & 510U) >> 1);
					goto IL_56;
					IL_111:
					num = reader.ReadUInt64();
				}
				flag = ((uint)b + (uint)num < 0U);
				if (!flag)
				{
					goto IL_160;
				}
			}
			IL_2D:
			return 0m;
			IL_160:
			return new decimal(num6, num5, num7, isNegative, b);
		}

		public static void WriteDecimal(decimal value, ProtoWriter writer)
		{
			int[] bits = decimal.GetBits(value);
			ulong num = (ulong)((ulong)((long)bits[1]) << 32);
			ulong num2 = (ulong)((long)bits[0] & (long)((ulong)-1));
			ulong num3 = num | num2;
			uint num4 = (uint)bits[2];
			uint num5 = (uint)((bits[3] >> 15 & 510) | (bits[3] >> 31 & 1));
			SubItemToken token = ProtoWriter.StartSubItem(null, writer);
			if (num3 != 0UL)
			{
				ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
				ProtoWriter.WriteUInt64(num3, writer);
			}
			for (;;)
			{
				if (num4 == 0U)
				{
					goto IL_57;
				}
				ProtoWriter.WriteFieldHeader(2, WireType.Variant, writer);
				ProtoWriter.WriteUInt32(num4, writer);
				IL_4E:
				if (4 == 0)
				{
					continue;
				}
				if ((uint)num3 - (uint)num2 <= 4294967295U)
				{
					break;
				}
				IL_57:
				if ((uint)num + num4 >= 0U)
				{
					break;
				}
				goto IL_4E;
			}
			IL_1E:
			if (num5 != 0U)
			{
				ProtoWriter.WriteFieldHeader(3, WireType.Variant, writer);
				ProtoWriter.WriteUInt32(num5, writer);
			}
			ProtoWriter.EndSubItem(token, writer);
			return;
			goto IL_1E;
		}

		public static void WriteGuid(Guid value, ProtoWriter dest)
		{
			byte[] data = value.ToByteArray();
			for (;;)
			{
				SubItemToken token = ProtoWriter.StartSubItem(null, dest);
				for (;;)
				{
					if (value != Guid.Empty)
					{
						ProtoWriter.WriteFieldHeader(1, WireType.Fixed64, dest);
						ProtoWriter.WriteBytes(data, 0, 8, dest);
						ProtoWriter.WriteFieldHeader(2, WireType.Fixed64, dest);
						ProtoWriter.WriteBytes(data, 8, 8, dest);
						if (false)
						{
							break;
						}
					}
					ProtoWriter.EndSubItem(token, dest);
					if (!false)
					{
						return;
					}
				}
			}
		}

		public static Guid ReadGuid(ProtoReader source)
		{
			ulong num = 0UL;
			uint num4;
			uint num5;
			uint num6;
			uint num7;
			for (;;)
			{
				IL_142:
				ulong num2 = 0UL;
				SubItemToken token = ProtoReader.StartSubItem(source);
				int num3;
				bool flag;
				for (;;)
				{
					if ((num3 = source.ReadFieldHeader()) > 0)
					{
						switch (num3)
						{
						case 1:
							num = source.ReadUInt64();
							continue;
						case 2:
							break;
						default:
							if ((uint)num3 + (uint)num3 >= 0U)
							{
								if ((uint)num2 >= 0U)
								{
									goto IL_DA;
								}
								goto IL_0F;
							}
							break;
						}
						num2 = source.ReadUInt64();
						continue;
					}
					flag = (num4 - num5 < 0U);
					if (flag)
					{
						goto IL_142;
					}
					ProtoReader.EndSubItem(token, source);
					if (num5 + (uint)num >= 0U)
					{
						goto Block_5;
					}
					IL_DA:
					source.SkipField();
				}
				IL_0F:
				num6 = (uint)num2;
				flag = ((uint)num > uint.MaxValue);
				if (flag)
				{
					continue;
				}
				goto IL_160;
				Block_5:
				if (num != 0UL)
				{
					goto IL_2D;
				}
				if (num2 == 0UL)
				{
					break;
				}
				flag = (num7 + (uint)num2 < 0U);
				if (!flag)
				{
					goto IL_2D;
				}
				IL_79:
				flag = ((uint)num + (uint)num3 < 0U);
				if (flag)
				{
					continue;
				}
				num5 = (uint)(num2 >> 32);
				goto IL_0F;
				IL_2D:
				num4 = (uint)(num >> 32);
				num7 = (uint)num;
				flag = (((uint)num2 | 4294967294U) == 0U);
				if (flag)
				{
					break;
				}
				goto IL_79;
			}
			return Guid.Empty;
			IL_160:
			return new Guid((int)num7, (short)num4, (short)(num4 >> 16), (byte)num6, (byte)(num6 >> 8), (byte)(num6 >> 16), (byte)(num6 >> 24), (byte)num5, (byte)(num5 >> 8), (byte)(num5 >> 16), (byte)(num5 >> 24));
		}

		public static object ReadNetObject(object value, ProtoReader source, int key, Type type, BclHelpers.NetObjectOptions options)
		{
			SubItemToken token = ProtoReader.StartSubItem(source);
			int num;
			int num2;
			if ((uint)num + (uint)num <= 4294967295U)
			{
				num2 = -1;
				goto IL_596;
			}
			goto IL_1BC;
			IL_69:
			int num3;
			int num4;
			int num5;
			bool flag;
			bool flag2;
			bool flag3;
			bool flag4;
			while ((num3 = source.ReadFieldHeader()) > 0)
			{
				num4 = num3;
				switch (num4)
				{
				case 1:
					num5 = source.ReadInt32();
					value = source.x4406e28dba6f6c8e.x7c483588b4d2e948(num5);
					if (!false)
					{
						continue;
					}
					goto IL_333;
				case 2:
					num2 = source.ReadInt32();
					continue;
				case 3:
					num5 = source.ReadInt32();
					if ((flag ? 1U : 0U) > 4294967295U)
					{
						goto IL_54F;
					}
					if ((uint)key - (flag2 ? 1U : 0U) >= 0U)
					{
						goto IL_4AA;
					}
					goto IL_596;
				case 4:
					num = source.ReadInt32();
					if ((flag2 ? 1U : 0U) <= 4294967295U)
					{
						continue;
					}
					goto IL_4AA;
				case 8:
				{
					string text = source.ReadString();
					type = source.x34a9527dfd927b35(text);
					goto IL_411;
				}
				case 10:
					flag = (type == typeof(string));
					flag3 = (value == null);
					flag2 = (flag3 && flag);
					if (num2 >= 0)
					{
						goto IL_2D7;
					}
					goto IL_287;
				}
				source.SkipField();
				continue;
				IL_4AA:
				if ((uint)num2 - (uint)num2 < 0U)
				{
					goto IL_1B6;
				}
				type = (Type)source.x4406e28dba6f6c8e.x7c483588b4d2e948(num5);
				if ((uint)num2 > 4294967295U)
				{
					goto IL_18D;
				}
				key = source.x987c197a20bb9886(ref type);
				flag4 = ((uint)num - (uint)key < 0U);
				if (flag4)
				{
					return value;
				}
			}
			IL_1E:
			if (num2 >= 0)
			{
				if ((byte)(options & BclHelpers.NetObjectOptions.AsReference) == 0)
				{
					throw new ProtoException("Object key in input stream, but reference-tracking was not expected");
				}
			}
			IL_2B:
			ProtoReader.EndSubItem(token, source);
			flag4 = ((flag3 ? 1U : 0U) + (uint)num5 < 0U);
			if (!flag4)
			{
				flag4 = ((uint)num > uint.MaxValue);
				if (!flag4)
				{
					return value;
				}
				if (((flag3 ? 1U : 0U) & 0U) != 0U)
				{
					goto IL_1F5;
				}
				goto IL_316;
			}
			IL_5B:
			goto IL_69;
			IL_126:
			if (num2 < 0)
			{
				if ((uint)num4 + (uint)num <= 4294967295U)
				{
					goto IL_EB;
				}
				goto IL_176;
			}
			else if (!flag2)
			{
				goto IL_B7;
			}
			IL_65:
			if (num2 < 0 && num >= 0)
			{
				source.x4406e28dba6f6c8e.x24ab034b13a4cf87(num, type);
				goto IL_5B;
			}
			goto IL_69;
			IL_B7:
			object obj;
			if (!object.ReferenceEquals(obj, value))
			{
				throw new ProtoException("A reference-tracked object changed reference during deserialization");
			}
			if (2 == 0)
			{
				goto IL_333;
			}
			if (((uint)num3 | 2147483648U) != 0U)
			{
				if ((uint)num4 - (flag3 ? 1U : 0U) >= 0U)
				{
					goto IL_65;
				}
			}
			else
			{
				if ((uint)key >= 0U)
				{
					goto IL_1E;
				}
				goto IL_2B;
			}
			IL_EB:
			if ((flag ? 1U : 0U) - (uint)key < 0U)
			{
				goto IL_B7;
			}
			if (-2 != 0)
			{
				goto IL_65;
			}
			flag4 = (((uint)num5 & 0U) == 0U);
			if (flag4)
			{
				goto IL_65;
			}
			goto IL_18D;
			IL_176:
			goto IL_126;
			IL_180:
			source.x4406e28dba6f6c8e.x24ab034b13a4cf87(num2, value);
			IL_18D:
			if (num < 0)
			{
				goto IL_126;
			}
			source.x4406e28dba6f6c8e.x24ab034b13a4cf87(num, type);
			goto IL_176;
			IL_1B6:
			goto IL_1C0;
			IL_1BC:
			if (!flag2)
			{
				obj = source.x4406e28dba6f6c8e.x7c483588b4d2e948(num2);
				flag4 = (((uint)num5 | 4U) == 0U);
				if (flag4)
				{
					goto IL_26F;
				}
				flag4 = ((uint)num3 - (uint)num2 > uint.MaxValue);
				if (flag4)
				{
					goto IL_316;
				}
			}
			IL_1C0:
			if (flag2)
			{
				goto IL_180;
			}
			goto IL_126;
			IL_1F5:
			goto IL_1C0;
			IL_26F:
			if ((uint)num3 - (uint)num4 < 0U)
			{
				goto IL_29F;
			}
			IL_287:
			obj = value;
			if (flag)
			{
				value = source.ReadString();
			}
			else
			{
				value = ProtoReader.x38631c01067da2d2(obj, key, source, type);
				if ((flag3 ? 1U : 0U) + (flag ? 1U : 0U) > 4294967295U)
				{
					goto IL_180;
				}
			}
			if (num2 < 0)
			{
				goto IL_126;
			}
			if (((flag2 ? 1U : 0U) & 0U) != 0U)
			{
				goto IL_1BC;
			}
			if (flag3)
			{
				goto IL_1BC;
			}
			goto IL_1B6;
			IL_299:
			if (num < 0)
			{
				goto IL_26F;
			}
			source.x4406e28dba6f6c8e.x24ab034b13a4cf87(num, type);
			goto IL_287;
			IL_29F:
			flag4 = (((uint)num5 & 0U) == 0U);
			if (flag4)
			{
				goto IL_287;
			}
			goto IL_299;
			IL_2D7:
			if (flag2)
			{
				flag4 = ((uint)num5 - (flag2 ? 1U : 0U) > uint.MaxValue);
				if (!flag4)
				{
					goto IL_29F;
				}
			}
			IL_2DD:
			if (value != null)
			{
				source.x4406e28dba6f6c8e.x24ab034b13a4cf87(num2, value);
				goto IL_299;
			}
			source.x2605289f9758acf8(num2);
			goto IL_299;
			IL_316:
			goto IL_2DD;
			IL_333:
			goto IL_2D7;
			IL_411:
			if (type == null)
			{
				string text;
				throw new ProtoException("Unable to resolve type: " + text + " (you can use the TypeModel.DynamicTypeFormatting event to provide a custom mapping)");
			}
			if (type != typeof(string))
			{
				flag4 = ((uint)key - (uint)num3 > uint.MaxValue);
				if (!flag4)
				{
					key = source.x987c197a20bb9886(ref type);
					if (key < 0)
					{
						throw new InvalidOperationException("Dynamic type is not a contract-type: " + type.Name);
					}
				}
			}
			else
			{
				key = -1;
			}
			goto IL_69;
			IL_54F:
			if ((flag ? 1U : 0U) + (flag2 ? 1U : 0U) <= 4294967295U)
			{
				goto IL_69;
			}
			goto IL_411;
			IL_596:
			num = -1;
			goto IL_54F;
		}

		public static void WriteNetObject(object value, ProtoWriter dest, int key, BclHelpers.NetObjectOptions options)
		{
			bool flag = (byte)(options & BclHelpers.NetObjectOptions.DynamicType) != 0;
			bool flag2;
			if (((flag2 ? 1U : 0U) & 0U) == 0U)
			{
				goto IL_332;
			}
			bool flag3 = (flag2 ? 1U : 0U) < 0U;
			if (flag3)
			{
				goto IL_A6;
			}
			goto IL_124;
			int num;
			int num2;
			bool flag4;
			for (;;)
			{
				IL_2E9:
				num = dest.x4406e28dba6f6c8e.xa711886cf1a666de(value, out flag2);
				if ((uint)num2 + (flag4 ? 1U : 0U) > 4294967295U)
				{
					break;
				}
				ProtoWriter.WriteFieldHeader((!flag2) ? 2 : 1, WireType.Variant, dest);
				flag3 = ((uint)num2 < 0U);
				if (!flag3)
				{
					goto IL_316;
				}
			}
			goto IL_17C;
			IL_316:
			flag3 = ((flag2 ? 1U : 0U) > uint.MaxValue);
			if (flag3)
			{
				goto IL_332;
			}
			flag3 = ((uint)num2 < 0U);
			if (flag3)
			{
				goto IL_37E;
			}
			goto IL_29C;
			IL_39:
			SubItemToken token;
			ProtoWriter.EndSubItem(token, dest);
			flag3 = ((uint)key + (flag ? 1U : 0U) < 0U);
			if (!flag3)
			{
				return;
			}
			flag3 = ((uint)num2 - (uint)num2 < 0U);
			if (flag3)
			{
				goto IL_29C;
			}
			goto IL_332;
			IL_5E:
			bool flag5;
			if ((flag2 ? 1U : 0U) + (flag5 ? 1U : 0U) >= 0U)
			{
				goto IL_39;
			}
			goto IL_2E9;
			IL_85:
			if (!flag4)
			{
				goto IL_39;
			}
			if (!flag)
			{
				goto IL_D2;
			}
			Type type = value.GetType();
			flag3 = (((flag2 ? 1U : 0U) | 8U) == 0U);
			bool flag6;
			if (!flag3)
			{
				while (value is string)
				{
					if ((flag4 ? 1U : 0U) - (flag6 ? 1U : 0U) <= 4294967295U)
					{
						goto IL_1AD;
					}
				}
			}
			key = dest.x987c197a20bb9886(ref type);
			if (key < 0)
			{
				throw new InvalidOperationException("Dynamic type is not a contract-type: " + type.Name);
			}
			if ((uint)num2 + (uint)num <= 4294967295U)
			{
				goto IL_17C;
			}
			goto IL_2E9;
			IL_A6:
			ProtoWriter.WriteString((string)value, dest);
			goto IL_5E;
			IL_D2:
			WireType x58ca9db3d85c761f;
			ProtoWriter.WriteFieldHeader(10, x58ca9db3d85c761f, dest);
			if (value is string)
			{
				flag3 = ((flag4 ? 1U : 0U) - (flag6 ? 1U : 0U) < 0U);
				if (flag3)
				{
					goto IL_124;
				}
				goto IL_13C;
			}
			else
			{
				ProtoWriter.WriteObject(value, key, dest);
				if ((flag2 ? 1U : 0U) < 0U)
				{
					goto IL_85;
				}
				if (-2 == 0)
				{
					goto IL_5E;
				}
				flag3 = ((uint)num + (uint)key < 0U);
				if (flag3)
				{
					goto IL_A6;
				}
				goto IL_39;
			}
			IL_102:
			int fieldNumber;
			ProtoWriter.WriteFieldHeader(fieldNumber, WireType.Variant, dest);
			if (true)
			{
				ProtoWriter.WriteInt32(num2, dest);
				if (flag5)
				{
					goto IL_D2;
				}
				ProtoWriter.WriteFieldHeader(8, WireType.String, dest);
				flag3 = ((uint)num > uint.MaxValue);
				if (flag3)
				{
					goto IL_13C;
				}
				ProtoWriter.WriteString(dest.x0885c39348d7aa9b(type), dest);
				goto IL_D2;
			}
			IL_124:
			fieldNumber = 4;
			goto IL_102;
			IL_13C:
			goto IL_A6;
			IL_17C:
			num2 = dest.x4406e28dba6f6c8e.xa711886cf1a666de(type, out flag5);
			flag3 = ((uint)key - (flag4 ? 1U : 0U) > uint.MaxValue);
			if (flag3)
			{
				flag3 = ((flag5 ? 1U : 0U) - (flag5 ? 1U : 0U) > uint.MaxValue);
				if (flag3)
				{
					if ((flag6 ? 1U : 0U) - (flag2 ? 1U : 0U) <= 4294967295U)
					{
						goto IL_27F;
					}
					goto IL_282;
				}
				else
				{
					flag3 = ((flag6 ? 1U : 0U) - (flag6 ? 1U : 0U) < 0U);
					if (flag3)
					{
						goto IL_332;
					}
					goto IL_37E;
				}
			}
			else
			{
				if (flag5)
				{
					fieldNumber = 3;
					goto IL_102;
				}
				goto IL_124;
			}
			IL_1AD:
			goto IL_17C;
			IL_27F:
			if (flag6)
			{
				goto IL_2E9;
			}
			IL_282:
			goto IL_85;
			IL_29C:
			ProtoWriter.WriteInt32(num, dest);
			if (!flag2)
			{
				goto IL_85;
			}
			flag4 = false;
			goto IL_85;
			IL_332:
			if ((flag5 ? 1U : 0U) - (flag4 ? 1U : 0U) >= 0U)
			{
				flag6 = ((byte)(options & BclHelpers.NetObjectOptions.AsReference) != 0);
				x58ca9db3d85c761f = dest.x58ca9db3d85c761f;
				token = ProtoWriter.StartSubItem(null, dest);
				flag4 = true;
				goto IL_27F;
			}
			goto IL_1AD;
			IL_37E:
			goto IL_124;
		}

		private const int x165df624c42231f9 = 1;

		private const int x937a8a717134ae7f = 2;

		private const int x5f6dce7833575afb = 1;

		private const int xf766fcc94ee2ff3a = 2;

		private const int x6b6ec8d9600d1d29 = 3;

		private const int x1d7ad433329146f2 = 1;

		private const int x4c767440df98b35f = 2;

		private const int xe9129b4f5ebb65cc = 1;

		private const int xeea29fb04d132bc9 = 2;

		private const int x8bbd6a68b719ec67 = 3;

		private const int x24788ff62c2f1cd2 = 4;

		private const int xf073a03653018eb9 = 8;

		private const int xd7128e8cac666997 = 10;

		internal static readonly DateTime xfc58af0fe10a0e2e = new DateTime(1970, 1, 1, 0, 0, 0, 0);

		[Flags]
		public enum NetObjectOptions : byte
		{
			None = 0,
			AsReference = 1,
			DynamicType = 2,
			UseConstructor = 4
		}
	}
}
