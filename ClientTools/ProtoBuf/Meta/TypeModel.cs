using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ProtoBuf.Meta
{
	public abstract class TypeModel
	{
		protected internal Type MapType(Type type)
		{
			return this.MapType(type, true);
		}

		protected internal virtual Type MapType(Type type, bool demand)
		{
			return type;
		}

		private WireType x24777f7b4955c787(xd669244d58bc09c0 x9035cf16181332fc, DataFormat x5786461d089b10a0, ref Type x43163d22e8cd5a71, out int xbb1542eae9170570)
		{
			xbb1542eae9170570 = -1;
			if (true)
			{
				if (x479f2661aae93792.xb636fcab7a16c388(x43163d22e8cd5a71))
				{
					xbb1542eae9170570 = this.GetKey(ref x43163d22e8cd5a71);
					return WireType.Variant;
				}
				switch (x9035cf16181332fc)
				{
				case xd669244d58bc09c0.x795dc524dba3fd4b:
				case xd669244d58bc09c0.xfb1fc02db7c42694:
				case xd669244d58bc09c0.x2025ae83be2038f0:
				case xd669244d58bc09c0.xc0f9b651d77da240:
				case xd669244d58bc09c0.x697a219ddc6427a9:
				case xd669244d58bc09c0.xf12cc4804eec7b89:
				case xd669244d58bc09c0.x85254000935bfc25:
				case xd669244d58bc09c0.x6cedabde5251b1e5:
					goto IL_19;
				case xd669244d58bc09c0.x0b2292ab52b25d76:
				case xd669244d58bc09c0.x394150f1be471c3c:
					while (x5786461d089b10a0 == DataFormat.FixedSize)
					{
						if (-2147483648 == 0)
						{
							if (!true)
							{
								continue;
							}
						}
						return WireType.Fixed64;
					}
					return WireType.Variant;
				case xd669244d58bc09c0.x63374d6ffed4adeb:
					return WireType.Fixed32;
				case xd669244d58bc09c0.x94c083f2813272f4:
					return WireType.Fixed64;
				case xd669244d58bc09c0.x18d3f7d37d3464ca:
				case xd669244d58bc09c0.x242851e6278ed355:
				case xd669244d58bc09c0.x4a498a651d07aefe:
					break;
				case (xd669244d58bc09c0)17:
					goto IL_25;
				default:
					if (false)
					{
						return WireType.Variant;
					}
					if (15 == 0)
					{
						goto IL_37;
					}
					switch (x9035cf16181332fc)
					{
					case xd669244d58bc09c0.x69ec9d2404a6b229:
					case xd669244d58bc09c0.x62a0c09ce3a5e8fb:
					case xd669244d58bc09c0.x0217cda8370c1f17:
					case xd669244d58bc09c0.xb405a444ca77e2d4:
						break;
					default:
						goto IL_25;
					}
					break;
				}
				return WireType.String;
				IL_25:
				if ((xbb1542eae9170570 = this.GetKey(ref x43163d22e8cd5a71)) >= 0)
				{
					return WireType.String;
				}
				return WireType.None;
			}
			IL_19:
			if (x5786461d089b10a0 == DataFormat.FixedSize)
			{
				return WireType.Fixed32;
			}
			return WireType.Variant;
			IL_37:
			goto IL_19;
		}

		internal bool x07feef0c759efbcc(ProtoWriter xbdfb620b7167944b, Type x43163d22e8cd5a71, DataFormat x5786461d089b10a0, int xffe521cc76054baf, object xbcea506a33cf9111, bool x39fdda937821d784)
		{
			if (x43163d22e8cd5a71 == null)
			{
				goto IL_51C;
			}
			xd669244d58bc09c0 xd669244d58bc09c;
			int num;
			WireType wireType;
			bool flag;
			for (;;)
			{
				IL_4AF:
				xd669244d58bc09c = x479f2661aae93792.xf70eec89828a813c(x43163d22e8cd5a71);
				wireType = this.x24777f7b4955c787(xd669244d58bc09c, x5786461d089b10a0, ref x43163d22e8cd5a71, out num);
				do
				{
					if (num < 0)
					{
						flag = ((x39fdda937821d784 ? 1U : 0U) - (uint)xffe521cc76054baf > uint.MaxValue);
						if (flag)
						{
							goto IL_40E;
						}
						if ((uint)xffe521cc76054baf >= 0U)
						{
							goto IL_3C7;
						}
					}
					else
					{
						while (x479f2661aae93792.xb636fcab7a16c388(x43163d22e8cd5a71))
						{
							this.Serialize(num, xbcea506a33cf9111, xbdfb620b7167944b);
							flag = ((uint)num > uint.MaxValue);
							if (!flag)
							{
								return true;
							}
						}
					}
					ProtoWriter.WriteFieldHeader(xffe521cc76054baf, wireType, xbdfb620b7167944b);
				}
				while ((uint)xffe521cc76054baf - (uint)num < 0U);
				switch (wireType)
				{
				case WireType.None:
					goto IL_412;
				case WireType.Variant:
				case WireType.Fixed64:
					goto IL_3E6;
				case WireType.String:
				case WireType.StartGroup:
					goto IL_419;
				default:
					flag = ((uint)xffe521cc76054baf - (uint)xffe521cc76054baf < 0U);
					if (!flag)
					{
						goto IL_4E9;
					}
					break;
				}
			}
			IL_235:
			ProtoWriter.WriteSByte((sbyte)xbcea506a33cf9111, xbdfb620b7167944b);
			return true;
			IL_2E0:
			xd669244d58bc09c0 xd669244d58bc09c2 = xd669244d58bc09c;
			switch (xd669244d58bc09c2)
			{
			case xd669244d58bc09c0.x795dc524dba3fd4b:
				ProtoWriter.WriteBoolean((bool)xbcea506a33cf9111, xbdfb620b7167944b);
				return true;
			case xd669244d58bc09c0.xfb1fc02db7c42694:
				ProtoWriter.WriteUInt16((ushort)((char)xbcea506a33cf9111), xbdfb620b7167944b);
				return true;
			case xd669244d58bc09c0.x2025ae83be2038f0:
				goto IL_235;
			case xd669244d58bc09c0.xc0f9b651d77da240:
				ProtoWriter.WriteByte((byte)xbcea506a33cf9111, xbdfb620b7167944b);
				return true;
			case xd669244d58bc09c0.x697a219ddc6427a9:
				ProtoWriter.WriteInt16((short)xbcea506a33cf9111, xbdfb620b7167944b);
				return true;
			case xd669244d58bc09c0.xf12cc4804eec7b89:
				ProtoWriter.WriteUInt16((ushort)xbcea506a33cf9111, xbdfb620b7167944b);
				return true;
			case xd669244d58bc09c0.x85254000935bfc25:
				ProtoWriter.WriteInt32((int)xbcea506a33cf9111, xbdfb620b7167944b);
				flag = (((uint)num | 8U) == 0U);
				if (!flag)
				{
					return true;
				}
				flag = ((uint)xffe521cc76054baf < 0U);
				if (flag)
				{
					goto IL_19C;
				}
				goto IL_244;
			case xd669244d58bc09c0.x6cedabde5251b1e5:
				ProtoWriter.WriteUInt32((uint)xbcea506a33cf9111, xbdfb620b7167944b);
				return true;
			case xd669244d58bc09c0.x0b2292ab52b25d76:
				ProtoWriter.WriteInt64((long)xbcea506a33cf9111, xbdfb620b7167944b);
				return true;
			case xd669244d58bc09c0.x394150f1be471c3c:
				goto IL_244;
			case xd669244d58bc09c0.x63374d6ffed4adeb:
				goto IL_19C;
			case xd669244d58bc09c0.x94c083f2813272f4:
				ProtoWriter.WriteDouble((double)xbcea506a33cf9111, xbdfb620b7167944b);
				return true;
			case xd669244d58bc09c0.x18d3f7d37d3464ca:
				goto IL_144;
			case xd669244d58bc09c0.x242851e6278ed355:
				BclHelpers.WriteDateTime((DateTime)xbcea506a33cf9111, xbdfb620b7167944b);
				flag = (((uint)xffe521cc76054baf & 0U) == 0U);
				if (!flag)
				{
					goto IL_19C;
				}
				if (false)
				{
					goto IL_144;
				}
				if ((uint)num - (uint)num >= 0U)
				{
					return true;
				}
				return false;
			case (xd669244d58bc09c0)17:
				break;
			case xd669244d58bc09c0.x4a498a651d07aefe:
				ProtoWriter.WriteString((string)xbcea506a33cf9111, xbdfb620b7167944b);
				flag = ((uint)num + (uint)xffe521cc76054baf < 0U);
				if (flag)
				{
					goto IL_FA;
				}
				if ((uint)num + (uint)xffe521cc76054baf <= 4294967295U)
				{
					return true;
				}
				goto IL_369;
			default:
				switch (xd669244d58bc09c2)
				{
				case xd669244d58bc09c0.x69ec9d2404a6b229:
					goto IL_FA;
				case xd669244d58bc09c0.x62a0c09ce3a5e8fb:
					ProtoWriter.WriteBytes((byte[])xbcea506a33cf9111, xbdfb620b7167944b);
					return true;
				case xd669244d58bc09c0.x0217cda8370c1f17:
					BclHelpers.WriteGuid((Guid)xbcea506a33cf9111, xbdfb620b7167944b);
					return true;
				case xd669244d58bc09c0.xb405a444ca77e2d4:
					ProtoWriter.WriteString(((Uri)xbcea506a33cf9111).AbsoluteUri, xbdfb620b7167944b);
					return true;
				}
				break;
			}
			IEnumerable enumerable = xbcea506a33cf9111 as IEnumerable;
			if (enumerable == null)
			{
				if (15 == 0)
				{
					goto IL_19C;
				}
				flag = (((uint)xffe521cc76054baf | 1U) == 0U);
				if (flag)
				{
					return true;
				}
				if (false)
				{
					goto IL_35A;
				}
				return false;
			}
			else
			{
				if (!x39fdda937821d784)
				{
					foreach (object obj in enumerable)
					{
						flag = ((x39fdda937821d784 ? 1U : 0U) > uint.MaxValue);
						if (!flag)
						{
							if (obj == null)
							{
								throw new NullReferenceException();
							}
							if (!this.x07feef0c759efbcc(xbdfb620b7167944b, null, x5786461d089b10a0, xffe521cc76054baf, obj, true))
							{
								TypeModel.ThrowUnexpectedType(obj.GetType());
								flag = ((x39fdda937821d784 ? 1U : 0U) - (x39fdda937821d784 ? 1U : 0U) > uint.MaxValue);
								if (!flag)
								{
									continue;
								}
							}
						}
						if (2147483647 == 0)
						{
							break;
						}
					}
					return true;
				}
				throw TypeModel.x65d41140f79f7ce3();
			}
			IL_FA:
			BclHelpers.WriteTimeSpan((TimeSpan)xbcea506a33cf9111, xbdfb620b7167944b);
			return true;
			IL_144:
			BclHelpers.WriteDecimal((decimal)xbcea506a33cf9111, xbdfb620b7167944b);
			return true;
			IL_19C:
			ProtoWriter.WriteSingle((float)xbcea506a33cf9111, xbdfb620b7167944b);
			return true;
			IL_244:
			ProtoWriter.WriteUInt64((ulong)xbcea506a33cf9111, xbdfb620b7167944b);
			return true;
			IL_35A:
			goto IL_2E0;
			IL_369:
			if (8 != 0)
			{
				goto IL_35A;
			}
			goto IL_3C7;
			IL_3A4:
			if (wireType == WireType.None)
			{
				goto IL_369;
			}
			goto IL_3D9;
			IL_3C7:
			flag = ((x39fdda937821d784 ? 1U : 0U) > uint.MaxValue);
			if (!flag)
			{
				goto IL_3A4;
			}
			IL_3D9:
			ProtoWriter.WriteFieldHeader(xffe521cc76054baf, wireType, xbdfb620b7167944b);
			goto IL_40E;
			IL_3E6:
			this.Serialize(num, xbcea506a33cf9111, xbdfb620b7167944b);
			return true;
			IL_40E:
			if ((x39fdda937821d784 ? 1U : 0U) >= 0U)
			{
				goto IL_2E0;
			}
			goto IL_3A4;
			IL_412:
			throw ProtoWriter.x98bdea680591defc(xbdfb620b7167944b);
			IL_419:
			SubItemToken token = ProtoWriter.StartSubItem(xbcea506a33cf9111, xbdfb620b7167944b);
			this.Serialize(num, xbcea506a33cf9111, xbdfb620b7167944b);
			ProtoWriter.EndSubItem(token, xbdfb620b7167944b);
			flag = ((uint)num > uint.MaxValue);
			if (flag)
			{
				goto IL_504;
			}
			return true;
			IL_4E9:
			flag = ((x39fdda937821d784 ? 1U : 0U) - (uint)num > uint.MaxValue);
			if (!flag)
			{
				goto IL_3E6;
			}
			IL_504:
			flag = (((uint)xffe521cc76054baf | 4294967294U) == 0U);
			if (!flag)
			{
				if ((uint)xffe521cc76054baf + (uint)xffe521cc76054baf < 0U)
				{
					goto IL_235;
				}
				flag = (((uint)xffe521cc76054baf & 0U) == 0U);
				if (flag)
				{
					goto IL_3A4;
				}
				goto IL_3D9;
			}
			IL_51C:
			x43163d22e8cd5a71 = xbcea506a33cf9111.GetType();
			goto IL_4AF;
		}

		private void xe936c2468442e070(ProtoWriter xbdfb620b7167944b, object xbcea506a33cf9111)
		{
			if (xbcea506a33cf9111 != null)
			{
				Type type = xbcea506a33cf9111.GetType();
				int key;
				for (;;)
				{
					key = this.GetKey(ref type);
					if (key >= 0)
					{
						break;
					}
					if (!false)
					{
						goto IL_5D;
					}
				}
				this.Serialize(key, xbcea506a33cf9111, xbdfb620b7167944b);
				return;
				IL_5D:
				bool flag = (uint)key + (uint)key > uint.MaxValue;
				if (!flag)
				{
					while (!this.x07feef0c759efbcc(xbdfb620b7167944b, type, DataFormat.Default, 1, xbcea506a33cf9111, false))
					{
						TypeModel.ThrowUnexpectedType(type);
						flag = ((uint)key < 0U);
						if (!flag && 15 != 0)
						{
							return;
						}
					}
					return;
				}
			}
			throw new ArgumentNullException("value");
		}

		public void Serialize(Stream dest, object value)
		{
			this.Serialize(dest, value, null);
		}

		public void Serialize(Stream dest, object value, SerializationContext context)
		{
			using (ProtoWriter protoWriter = new ProtoWriter(dest, this, context))
			{
				protoWriter.SetRootObject(value);
				this.xe936c2468442e070(protoWriter, value);
				protoWriter.Close();
			}
		}

		public void Serialize(ProtoWriter dest, object value)
		{
			dest.xf5a7c936201d3c81();
			dest.SetRootObject(value);
			this.xe936c2468442e070(dest, value);
			dest.xf5a7c936201d3c81();
			ProtoWriter.xbb7550bbb62a218c(dest);
		}

		public object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int fieldNumber)
		{
			int num;
			return this.DeserializeWithLengthPrefix(source, value, type, style, fieldNumber, null, out num);
		}

		public object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver)
		{
			int num;
			return this.DeserializeWithLengthPrefix(source, value, type, style, expectedField, resolver, out num);
		}

		public object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, out int bytesRead)
		{
			bool flag;
			return this.x5c075dfed546c92d(source, value, type, style, expectedField, resolver, out bytesRead, out flag, null);
		}

		private object x5c075dfed546c92d(Stream x337e217cb3ba0627, object xbcea506a33cf9111, Type x43163d22e8cd5a71, PrefixStyle x44ecfea61c937b8e, int x259d864bb556a52f, Serializer.TypeResolver x59b00779808aa981, out int x73ff96c61b2f324f, out bool xd1ddd4337de43a81, SerializationContext x0f7b23d1c393aed9)
		{
			xd1ddd4337de43a81 = false;
			x73ff96c61b2f324f = 0;
			if (4 != 0)
			{
				if (x43163d22e8cd5a71 == null)
				{
					if (x44ecfea61c937b8e != PrefixStyle.Base128)
					{
						goto IL_29C;
					}
					if (x59b00779808aa981 == null)
					{
						goto IL_29C;
					}
				}
				for (;;)
				{
					IL_24D:
					bool flag = x259d864bb556a52f > 0 || x59b00779808aa981 != null;
					int num2;
					int num3;
					int num = ProtoReader.ReadLengthPrefix(x337e217cb3ba0627, flag, x44ecfea61c937b8e, out num2, out num3);
					int i;
					if (((uint)i & 0U) != 0U)
					{
						goto IL_28E;
					}
					goto IL_22A;
					bool flag2;
					bool flag3;
					object result;
					for (;;)
					{
						IL_FC:
						if (flag2)
						{
							if (num == 2147483647)
							{
								goto Block_3;
							}
							ProtoReader.x0c66d9eb79cc002f(x337e217cb3ba0627, num, null);
							x73ff96c61b2f324f += num;
						}
						if (flag2)
						{
							goto IL_24D;
						}
						flag3 = ((flag ? 1U : 0U) - (uint)num2 < 0U);
						if (flag3)
						{
							break;
						}
						using (ProtoReader protoReader = new ProtoReader(x337e217cb3ba0627, this, x0f7b23d1c393aed9, num))
						{
							i = this.GetKey(ref x43163d22e8cd5a71);
							while (i < 0)
							{
								flag3 = (((uint)x259d864bb556a52f | 2147483647U) == 0U);
								if (!flag3)
								{
									goto IL_62;
								}
								if ((flag ? 1U : 0U) >= 0U)
								{
								}
								if ((flag ? 1U : 0U) + (flag2 ? 1U : 0U) >= 0U)
								{
									goto IL_50;
								}
								continue;
								IL_35:
								result = xbcea506a33cf9111;
								flag3 = ((uint)i - (flag ? 1U : 0U) < 0U);
								if (!flag3)
								{
									return result;
								}
								IL_50:
								if ((uint)num3 > 4294967295U)
								{
									goto IL_35;
								}
								IL_62:
								if (!this.x3d653e8159e64ad2(protoReader, DataFormat.Default, 1, x43163d22e8cd5a71, ref xbcea506a33cf9111, true, false, true, false) && num != 0)
								{
									TypeModel.ThrowUnexpectedType(x43163d22e8cd5a71);
								}
								IL_24:
								x73ff96c61b2f324f += protoReader.Position;
								xd1ddd4337de43a81 = true;
								goto IL_35;
							}
							xbcea506a33cf9111 = this.Deserialize(i, xbcea506a33cf9111, protoReader);
							goto IL_24;
						}
					}
					if (-1 != 0)
					{
						return result;
					}
					flag3 = ((uint)num - (uint)i > uint.MaxValue);
					if (flag3)
					{
						goto IL_22A;
					}
					continue;
					IL_199:
					while (flag)
					{
						if (x259d864bb556a52f != 0)
						{
							break;
						}
						if (x43163d22e8cd5a71 != null)
						{
							flag3 = (((uint)i | 255U) == 0U);
							if (!flag3)
							{
								break;
							}
						}
						else
						{
							if (x59b00779808aa981 != null)
							{
								x43163d22e8cd5a71 = x59b00779808aa981(num2);
								flag2 = (x43163d22e8cd5a71 == null);
								goto IL_FC;
							}
							break;
						}
					}
					IL_13A:
					flag2 = (x259d864bb556a52f != num2);
					if (((uint)num & 0U) != 0U)
					{
						break;
					}
					goto IL_FC;
					IL_22A:
					if (num3 == 0)
					{
						if ((flag ? 1U : 0U) + (uint)num >= 0U)
						{
							return xbcea506a33cf9111;
						}
						if (-2147483648 != 0)
						{
							goto IL_13A;
						}
					}
					else
					{
						x73ff96c61b2f324f += num3;
						flag3 = ((uint)num - (uint)i > uint.MaxValue);
						if (flag3 || num < 0)
						{
							return xbcea506a33cf9111;
						}
						if (x44ecfea61c937b8e != PrefixStyle.Base128)
						{
							flag2 = false;
							goto IL_FC;
						}
					}
					IL_28E:
					goto IL_199;
				}
				Block_3:
				throw new InvalidOperationException();
			}
			IL_29C:
			throw new InvalidOperationException("A type must be provided unless base-128 prefixing is being used in combination with a resolver");
		}

		public IEnumerable DeserializeItems(Stream source, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver)
		{
			return this.DeserializeItems(source, type, style, expectedField, resolver, null);
		}

		public IEnumerable DeserializeItems(Stream source, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, SerializationContext context)
		{
			return new TypeModel.xd7bfba715d076364(this, source, type, style, expectedField, resolver, context);
		}

		public IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style, int expectedField)
		{
			return this.DeserializeItems<T>(source, style, expectedField, null);
		}

		public IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style, int expectedField, SerializationContext context)
		{
			return new TypeModel<T>.x4f14c9867bda75fa(this, source, style, expectedField, context);
		}

		public void SerializeWithLengthPrefix(Stream dest, object value, Type type, PrefixStyle style, int fieldNumber)
		{
			this.SerializeWithLengthPrefix(dest, value, type, style, fieldNumber, null);
		}

		public void SerializeWithLengthPrefix(Stream dest, object value, Type type, PrefixStyle style, int fieldNumber, SerializationContext context)
		{
			int key;
			if (type == null)
			{
				while (value != null)
				{
					type = this.MapType(value.GetType());
					if ((uint)key - (uint)fieldNumber <= 4294967295U)
					{
						goto IL_37;
					}
				}
				throw new ArgumentNullException("value");
			}
			IL_37:
			key = this.GetKey(ref type);
			using (ProtoWriter protoWriter = new ProtoWriter(dest, this, context))
			{
				bool flag = (uint)fieldNumber + (uint)fieldNumber < 0U;
				if (!flag)
				{
					if (-1 == 0)
					{
						goto IL_7C;
					}
					switch (style)
					{
					case PrefixStyle.None:
						goto IL_7C;
					case PrefixStyle.Base128:
					case PrefixStyle.Fixed32:
					case PrefixStyle.Fixed32BigEndian:
						ProtoWriter.x9f4d284c7b43f8f0(value, key, protoWriter, style, fieldNumber);
						break;
					default:
						flag = (((uint)fieldNumber | 255U) == 0U);
						if (flag)
						{
							goto IL_CC;
						}
						goto IL_7A;
					}
					IL_72:
					protoWriter.Close();
					goto IL_CC;
					IL_7C:
					this.Serialize(key, value, protoWriter);
					goto IL_72;
					IL_CC:
					return;
				}
				IL_7A:
				throw new ArgumentOutOfRangeException("style");
			}
		}

		public object Deserialize(Stream source, object value, Type type)
		{
			return this.Deserialize(source, value, type, null);
		}

		public object Deserialize(Stream source, object value, Type type, SerializationContext context)
		{
			bool x1037baa379fedbd = this.x5e9afee94f23af4c(value, ref type);
			object result;
			using (ProtoReader protoReader = new ProtoReader(source, this, context))
			{
				if (value != null)
				{
					protoReader.xf4032809c9332461(value);
				}
				result = this.xb76bd409a31bc8da(protoReader, type, value, x1037baa379fedbd);
			}
			return result;
		}

		private bool x5e9afee94f23af4c(object xbcea506a33cf9111, ref Type x43163d22e8cd5a71)
		{
			if (x43163d22e8cd5a71 != null)
			{
				goto IL_49;
			}
			bool flag2;
			bool flag = (flag2 ? 1U : 0U) - (flag2 ? 1U : 0U) > uint.MaxValue;
			if (flag)
			{
				goto IL_5A;
			}
			goto IL_5A;
			IL_34:
			Type type;
			x43163d22e8cd5a71 = type;
			flag2 = false;
			goto IL_7C;
			IL_49:
			flag2 = true;
			type = x479f2661aae93792.xe5e08d1dc9f521de(x43163d22e8cd5a71);
			if (false)
			{
				goto IL_85;
			}
			if (type != null)
			{
				goto IL_34;
			}
			if (true)
			{
				return flag2;
			}
			IL_5A:
			if (xbcea506a33cf9111 == null)
			{
				goto IL_85;
			}
			if (!true)
			{
				goto IL_34;
			}
			flag = ((flag2 ? 1U : 0U) - (flag2 ? 1U : 0U) > uint.MaxValue);
			if (!flag)
			{
				x43163d22e8cd5a71 = this.MapType(xbcea506a33cf9111.GetType());
				goto IL_49;
			}
			IL_7C:
			return flag2;
			IL_85:
			throw new ArgumentNullException("type");
		}

		public object Deserialize(Stream source, object value, Type type, int length)
		{
			return this.Deserialize(source, value, type, length, null);
		}

		public object Deserialize(Stream source, object value, Type type, int length, SerializationContext context)
		{
			bool x1037baa379fedbd = this.x5e9afee94f23af4c(value, ref type);
			object result;
			using (ProtoReader protoReader = new ProtoReader(source, this, context, length))
			{
				if (value != null)
				{
					protoReader.xf4032809c9332461(value);
				}
				object obj = this.xb76bd409a31bc8da(protoReader, type, value, x1037baa379fedbd);
				protoReader.x24f0af6d03e81cc5();
				result = obj;
			}
			return result;
		}

		public object Deserialize(ProtoReader source, object value, Type type)
		{
			bool x1037baa379fedbd = this.x5e9afee94f23af4c(value, ref type);
			if (value != null)
			{
				source.xf4032809c9332461(value);
			}
			object result;
			do
			{
				result = this.xb76bd409a31bc8da(source, type, value, x1037baa379fedbd);
				source.x24f0af6d03e81cc5();
			}
			while (false);
			return result;
		}

		private object xb76bd409a31bc8da(ProtoReader xe134235b3526fa75, Type x43163d22e8cd5a71, object xbcea506a33cf9111, bool x1037baa379fedbd3)
		{
			int i = this.GetKey(ref x43163d22e8cd5a71);
			while (i >= 0)
			{
				bool flag = ((uint)i & 0U) == 0U;
				if (flag)
				{
					flag = ((x1037baa379fedbd3 ? 1U : 0U) - (x1037baa379fedbd3 ? 1U : 0U) > uint.MaxValue);
					if (!flag)
					{
						IL_0E:
						if (!x479f2661aae93792.xb636fcab7a16c388(x43163d22e8cd5a71))
						{
							return this.Deserialize(i, xbcea506a33cf9111, xe134235b3526fa75);
						}
						IL_16:
						this.x3d653e8159e64ad2(xe134235b3526fa75, DataFormat.Default, 1, x43163d22e8cd5a71, ref xbcea506a33cf9111, true, false, x1037baa379fedbd3, false);
					}
					return xbcea506a33cf9111;
				}
			}
			if (2147483647 == 0)
			{
				goto IL_0E;
			}
			goto IL_16;
		}

		internal static MethodInfo xf6961b25e800f372(TypeModel xad70a5849826ecef, Type xd275fdd8cec9b85e, Type xd99217279677497c, out bool x02cadcecef04989f)
		{
			Type[] array;
			MethodInfo methodInfo;
			for (;;)
			{
				IL_B4:
				if (!true)
				{
					goto IL_3F;
				}
				x02cadcecef04989f = xad70a5849826ecef.MapType(TypeModel.xdc694e00cd3ff400).IsAssignableFrom(xd275fdd8cec9b85e);
				Type type;
				if (!false)
				{
					array = new Type[]
					{
						xd99217279677497c
					};
					methodInfo = x479f2661aae93792.x40c628a821a074f8(xd275fdd8cec9b85e, "Add", array);
					if (methodInfo != null)
					{
						goto IL_4C;
					}
					type = xad70a5849826ecef.MapType(typeof(ICollection<>)).MakeGenericType(array);
					if (-2147483648 == 0)
					{
						continue;
					}
				}
				if (false)
				{
					return methodInfo;
				}
				if (type.IsAssignableFrom(xd275fdd8cec9b85e))
				{
					goto IL_3F;
				}
				IL_4C:
				if (methodInfo == null)
				{
					array[0] = xad70a5849826ecef.MapType(typeof(object));
					methodInfo = x479f2661aae93792.x40c628a821a074f8(xd275fdd8cec9b85e, "Add", array);
				}
				if (methodInfo == null)
				{
					while (!x02cadcecef04989f)
					{
						if (!false)
						{
							if (false)
							{
								goto IL_B4;
							}
							return methodInfo;
						}
					}
					break;
				}
				return methodInfo;
				IL_3F:
				methodInfo = x479f2661aae93792.x40c628a821a074f8(type, "Add", array);
				goto IL_4C;
			}
			methodInfo = x479f2661aae93792.x40c628a821a074f8(xad70a5849826ecef.MapType(TypeModel.xdc694e00cd3ff400), "Add", array);
			return methodInfo;
		}

		internal static Type x4431fb61faccece0(TypeModel xad70a5849826ecef, Type xd275fdd8cec9b85e)
		{
			if (xd275fdd8cec9b85e == xad70a5849826ecef.MapType(typeof(string)) || xd275fdd8cec9b85e.IsArray || !xad70a5849826ecef.MapType(typeof(IEnumerable)).IsAssignableFrom(xd275fdd8cec9b85e))
			{
				return null;
			}
			x826e0336b5da6af5 x826e0336b5da6af = new x826e0336b5da6af5();
			MethodInfo[] methods = xd275fdd8cec9b85e.GetMethods();
			int num = 0;
			int num2;
			int num3;
			if ((uint)num2 + (uint)num3 >= 0U)
			{
				goto IL_2CC;
			}
			goto IL_3A0;
			IL_2C6:
			num++;
			IL_2CC:
			int xd44988f225497f3a;
			bool flag;
			if (num >= methods.Length)
			{
				Type[] interfaces = xd275fdd8cec9b85e.GetInterfaces();
				num2 = 0;
				for (;;)
				{
					for (;;)
					{
						PropertyInfo[] properties;
						if (num2 >= interfaces.Length)
						{
							flag = ((uint)xd44988f225497f3a < 0U);
							if (!flag)
							{
								properties = xd275fdd8cec9b85e.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
								num3 = 0;
								flag = ((uint)num - (uint)xd44988f225497f3a > uint.MaxValue);
								if (flag)
								{
									goto IL_1CE;
								}
							}
							goto IL_CA;
						}
						Type type = interfaces[num2];
						Type[] genericArguments;
						if (type.IsGenericType)
						{
							if (type.GetGenericTypeDefinition() == xad70a5849826ecef.MapType(typeof(ICollection<>)))
							{
								genericArguments = type.GetGenericArguments();
								goto IL_1E9;
							}
							if ((uint)num2 < 0U)
							{
								goto IL_19D;
							}
							flag = ((uint)num - (uint)num2 > uint.MaxValue);
							if (flag)
							{
								flag = ((uint)num < 0U);
								if (flag)
								{
									goto Block_17;
								}
								if (15 == 0)
								{
									goto IL_2F6;
								}
								goto IL_1CE;
							}
						}
						IL_1F5:
						num2++;
						if ((uint)xd44988f225497f3a + (uint)num3 <= 4294967295U)
						{
							break;
						}
						goto IL_3A0;
						IL_1E9:
						if (x826e0336b5da6af.x263d579af1d0d43f(genericArguments[0]))
						{
							goto IL_1F5;
						}
						x826e0336b5da6af.xd6b6ed77479ef68c(genericArguments[0]);
						goto IL_1F5;
						IL_1CE:
						if ((uint)num - (uint)xd44988f225497f3a <= 4294967295U)
						{
							goto IL_1E9;
						}
						break;
						IL_CA:
						if (num3 >= properties.Length)
						{
							xd44988f225497f3a = x826e0336b5da6af.xd44988f225497f3a;
							flag = ((uint)num2 + (uint)num2 < 0U);
							if (!flag)
							{
								switch (xd44988f225497f3a)
								{
								case 0:
									goto IL_97;
								case 1:
									goto IL_99;
								case 2:
									goto IL_4B;
								}
								goto Block_4;
							}
							IL_4B:
							if (TypeModel.x41e210ff66329ae7(xad70a5849826ecef, (Type)x826e0336b5da6af.get_xe6d4b1b411ed94b5(0), (Type)x826e0336b5da6af.get_xe6d4b1b411ed94b5(1)))
							{
								goto IL_72;
							}
							if (false)
							{
								goto IL_1CE;
							}
							goto IL_23B;
						}
						IL_19D:
						PropertyInfo propertyInfo = properties[num3];
						if (!(propertyInfo.Name != "Item"))
						{
							if ((uint)num2 - (uint)xd44988f225497f3a < 0U)
							{
								goto IL_385;
							}
							if (!x826e0336b5da6af.x263d579af1d0d43f(propertyInfo.PropertyType))
							{
								ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
								if (indexParameters.Length == 1)
								{
									flag = ((uint)num2 > uint.MaxValue);
									if (flag)
									{
										if (8 == 0)
										{
											goto IL_3A9;
										}
									}
									else if (indexParameters[0].ParameterType != xad70a5849826ecef.MapType(typeof(int)))
									{
										goto IL_C4;
									}
									x826e0336b5da6af.xd6b6ed77479ef68c(propertyInfo.PropertyType);
								}
							}
						}
						IL_C4:
						num3++;
						goto IL_CA;
					}
				}
				IL_72:
				return (Type)x826e0336b5da6af.get_xe6d4b1b411ed94b5(0);
				Block_4:
				goto IL_400;
				IL_97:
				return null;
				IL_99:
				return (Type)x826e0336b5da6af.get_xe6d4b1b411ed94b5(0);
				IL_23B:
				if (TypeModel.x41e210ff66329ae7(xad70a5849826ecef, (Type)x826e0336b5da6af.get_xe6d4b1b411ed94b5(1), (Type)x826e0336b5da6af.get_xe6d4b1b411ed94b5(0)))
				{
					return (Type)x826e0336b5da6af.get_xe6d4b1b411ed94b5(1);
				}
				goto IL_400;
				Block_17:
				goto IL_2C6;
				IL_400:
				return null;
			}
			MethodInfo methodInfo = methods[num];
			goto IL_385;
			IL_2F6:
			if ((uint)num2 + (uint)num >= 0U)
			{
				goto IL_2C6;
			}
			goto IL_354;
			IL_385:
			flag = (((uint)xd44988f225497f3a | 1U) == 0U);
			if (!flag)
			{
				if (methodInfo.IsStatic)
				{
					goto IL_2F6;
				}
			}
			IL_354:
			if (methodInfo.Name != "Add")
			{
				goto IL_2C6;
			}
			ParameterInfo[] parameters = methodInfo.GetParameters();
			goto IL_3A9;
			IL_3A0:
			goto IL_354;
			IL_3A9:
			while (parameters.Length == 1)
			{
				if (!x826e0336b5da6af.x263d579af1d0d43f(parameters[0].ParameterType))
				{
					x826e0336b5da6af.xd6b6ed77479ef68c(parameters[0].ParameterType);
					break;
				}
				flag = (((uint)num2 & 0U) == 0U);
				if (flag)
				{
					break;
				}
			}
			goto IL_2C6;
		}

		private static bool x41e210ff66329ae7(TypeModel xad70a5849826ecef, Type x61830ac74d65acc3, Type xbcea506a33cf9111)
		{
			return x61830ac74d65acc3.IsGenericType && x61830ac74d65acc3.GetGenericTypeDefinition() == xad70a5849826ecef.MapType(typeof(KeyValuePair<, >)) && x61830ac74d65acc3.GetGenericArguments()[1] == xbcea506a33cf9111;
		}

		private bool xcf163c468d9e8a34(TypeModel xad70a5849826ecef, ProtoReader xe134235b3526fa75, DataFormat x5786461d089b10a0, int xffe521cc76054baf, Type xd275fdd8cec9b85e, Type xd99217279677497c, ref object xbcea506a33cf9111)
		{
			bool flag;
			MethodInfo methodInfo = TypeModel.xf6961b25e800f372(xad70a5849826ecef, xd275fdd8cec9b85e, xd99217279677497c, out flag);
			if (methodInfo == null)
			{
				throw new NotSupportedException("Unknown list variant: " + xd275fdd8cec9b85e.FullName);
			}
			bool flag2 = false;
			object obj = null;
			bool flag3 = (flag2 ? 1U : 0U) - (flag2 ? 1U : 0U) > uint.MaxValue;
			if (flag3)
			{
				goto IL_2A0;
			}
			if ((flag2 ? 1U : 0U) < 0U)
			{
				goto IL_144;
			}
			IList list;
			object[] array2;
			if ((uint)xffe521cc76054baf <= 4294967295U)
			{
				list = (xbcea506a33cf9111 as IList);
				object[] array;
				if ((flag ? 1U : 0U) - (flag2 ? 1U : 0U) <= 4294967295U)
				{
					if (15 == 0 || !flag)
					{
						array = new object[1];
						goto IL_202;
					}
				}
				array = null;
				IL_202:
				array2 = array;
				goto IL_2A0;
			}
			goto IL_193;
			IL_65:
			x826e0336b5da6af5 x826e0336b5da6af;
			Array array3;
			Array array4;
			x826e0336b5da6af.x0fe4f26e70030075(array3, array4.Length);
			xbcea506a33cf9111 = array3;
			return flag2;
			IL_7C:
			array4 = (Array)xbcea506a33cf9111;
			array3 = Array.CreateInstance(xd99217279677497c, array4.Length + x826e0336b5da6af.xd44988f225497f3a);
			if (3 != 0)
			{
				Array.Copy(array4, array3, array4.Length);
				goto IL_65;
			}
			goto IL_10D;
			IL_AA:
			obj = null;
			IL_AC:
			if (!this.x3d653e8159e64ad2(xe134235b3526fa75, x5786461d089b10a0, xffe521cc76054baf, xd99217279677497c, ref obj, true, true, true, true))
			{
				if (((flag2 ? 1U : 0U) | 8U) == 0U)
				{
					goto IL_14D;
				}
				if (x826e0336b5da6af == null)
				{
					return flag2;
				}
				if (xbcea506a33cf9111 == null)
				{
					array3 = Array.CreateInstance(xd99217279677497c, x826e0336b5da6af.xd44988f225497f3a);
					x826e0336b5da6af.x0fe4f26e70030075(array3, 0);
					xbcea506a33cf9111 = array3;
					return flag2;
				}
				if (x826e0336b5da6af.xd44988f225497f3a != 0)
				{
					goto IL_7C;
				}
				flag3 = ((uint)xffe521cc76054baf - (flag2 ? 1U : 0U) > uint.MaxValue);
				if (flag3)
				{
					goto IL_1A6;
				}
				return flag2;
			}
			else
			{
				flag2 = true;
				if (false)
				{
					goto IL_65;
				}
				if (((flag ? 1U : 0U) & 0U) != 0U)
				{
					goto IL_19D;
				}
				goto IL_193;
			}
			IL_10D:
			array2[0] = obj;
			goto IL_16A;
			IL_11A:
			if (list == null)
			{
				flag3 = ((uint)xffe521cc76054baf + (flag2 ? 1U : 0U) < 0U);
				if (flag3 || x826e0336b5da6af != null)
				{
					x826e0336b5da6af.xd6b6ed77479ef68c(obj);
					goto IL_AA;
				}
				flag3 = ((flag ? 1U : 0U) + (flag ? 1U : 0U) > uint.MaxValue);
				if (flag3)
				{
					goto IL_16A;
				}
				goto IL_10D;
			}
			IL_144:
			list.Add(obj);
			IL_14D:
			goto IL_AA;
			IL_16A:
			methodInfo.Invoke(xbcea506a33cf9111, array2);
			if ((flag2 ? 1U : 0U) >= 0U)
			{
				goto IL_AA;
			}
			goto IL_7C;
			IL_193:
			if (xbcea506a33cf9111 != null)
			{
				goto IL_11A;
			}
			IL_19D:
			if (x826e0336b5da6af != null)
			{
				goto IL_11A;
			}
			IL_1A6:
			xbcea506a33cf9111 = TypeModel.x8b0978848b8680a2(xd275fdd8cec9b85e, xd99217279677497c);
			list = (xbcea506a33cf9111 as IList);
			goto IL_11A;
			IL_2A0:
			flag3 = ((uint)xffe521cc76054baf + (flag2 ? 1U : 0U) > uint.MaxValue);
			if (flag3)
			{
				goto IL_1A6;
			}
			x826e0336b5da6af = ((!xd275fdd8cec9b85e.IsArray) ? null : new x826e0336b5da6af5());
			goto IL_AC;
		}

		private static object x8b0978848b8680a2(Type xd275fdd8cec9b85e, Type xd99217279677497c)
		{
			Type type = xd275fdd8cec9b85e;
			for (;;)
			{
				if (3 == 0)
				{
					goto IL_1B4;
				}
				if (-2 == 0)
				{
					goto IL_27E;
				}
				goto IL_24E;
				IL_24:
				bool flag;
				bool flag2;
				while (!flag)
				{
					type = typeof(ArrayList);
					flag2 = ((flag ? 1U : 0U) + (flag ? 1U : 0U) < 0U);
					if (flag2)
					{
						goto IL_AE;
					}
					flag = true;
					flag2 = ((flag ? 1U : 0U) + (flag ? 1U : 0U) > uint.MaxValue);
					if (!flag2)
					{
						goto IL_296;
					}
				}
				flag2 = ((flag ? 1U : 0U) + (flag ? 1U : 0U) < 0U);
				if (flag2)
				{
					goto IL_AE;
				}
				break;
				IL_1B4:
				if (!xd275fdd8cec9b85e.IsGenericType)
				{
					if (-2 == 0)
					{
						continue;
					}
					if (false)
					{
						goto IL_28C;
					}
					goto IL_118;
				}
				else
				{
					if (xd275fdd8cec9b85e.GetGenericTypeDefinition() != typeof(IDictionary<, >))
					{
						goto IL_118;
					}
					Type[] genericArguments = xd275fdd8cec9b85e.GetGenericArguments();
					type = typeof(Dictionary<, >).MakeGenericType(genericArguments);
					flag = true;
					if ((flag ? 1U : 0U) - (flag ? 1U : 0U) <= 4294967295U)
					{
						goto IL_1F9;
					}
					goto IL_88;
				}
				IL_133:
				if (xd275fdd8cec9b85e.FullName.IndexOf("Dictionary") >= 0)
				{
					goto IL_1B4;
				}
				if (2147483647 == 0)
				{
					goto IL_1F9;
				}
				if (2 == 0)
				{
					goto IL_1B4;
				}
				flag2 = (((flag ? 1U : 0U) | 1U) == 0U);
				if (flag2 || (flag ? 1U : 0U) - (flag ? 1U : 0U) < 0U)
				{
					goto IL_118;
				}
				if (false)
				{
					goto IL_90;
				}
				goto IL_88;
				IL_24E:
				if (xd275fdd8cec9b85e.IsArray)
				{
					goto IL_27E;
				}
				if (xd275fdd8cec9b85e.IsClass && !xd275fdd8cec9b85e.IsAbstract)
				{
					if (x479f2661aae93792.xdaa1a96eb962bff0(xd275fdd8cec9b85e, x479f2661aae93792.xf6f6ea67665595b8, true) != null)
					{
						break;
					}
				}
				flag = false;
				flag2 = ((flag ? 1U : 0U) > uint.MaxValue);
				if (flag2)
				{
					goto IL_133;
				}
				if (!xd275fdd8cec9b85e.IsInterface)
				{
					goto IL_88;
				}
				flag2 = ((flag ? 1U : 0U) + (flag ? 1U : 0U) > uint.MaxValue);
				if (flag2)
				{
					goto IL_178;
				}
				goto IL_133;
				IL_AE:
				if ((flag ? 1U : 0U) - (flag ? 1U : 0U) <= 4294967295U)
				{
					goto IL_88;
				}
				goto IL_24E;
				IL_86:
				goto IL_24;
				IL_90:
				type = typeof(List<>).MakeGenericType(new Type[]
				{
					xd99217279677497c
				});
				flag = true;
				if ((flag ? 1U : 0U) - (flag ? 1U : 0U) <= 4294967295U)
				{
					goto IL_86;
				}
				IL_88:
				if (flag)
				{
					goto IL_24;
				}
				goto IL_90;
				IL_118:
				if (flag)
				{
					goto IL_88;
				}
				if (xd275fdd8cec9b85e != typeof(IDictionary))
				{
					if (false)
					{
						goto IL_133;
					}
					if (false)
					{
						break;
					}
					goto IL_AE;
				}
				else
				{
					type = typeof(Hashtable);
				}
				IL_178:
				if ((flag ? 1U : 0U) - (flag ? 1U : 0U) <= 4294967295U)
				{
					flag = true;
					goto IL_88;
				}
				goto IL_86;
				IL_28C:
				goto IL_178;
				IL_1F9:
				goto IL_118;
			}
			goto IL_296;
			IL_27E:
			return Array.CreateInstance(xd99217279677497c, 0);
			IL_296:
			return Activator.CreateInstance(type);
		}

		internal bool x3d653e8159e64ad2(ProtoReader xe134235b3526fa75, DataFormat x5786461d089b10a0, int xffe521cc76054baf, Type x43163d22e8cd5a71, ref object xbcea506a33cf9111, bool xeb87b08c9033babc, bool x0d64564fe23d43d4, bool x75ef743805c2e831, bool x309954897d422190)
		{
			if (x43163d22e8cd5a71 != null)
			{
				Type type = null;
				int num;
				int num2;
				bool flag2;
				for (;;)
				{
					xd669244d58bc09c0 xd669244d58bc09c = x479f2661aae93792.xf70eec89828a813c(x43163d22e8cd5a71);
					bool flag = ((uint)num | 2147483647U) == 0U;
					if (flag)
					{
						goto IL_6B2;
					}
					WireType wireType = this.x24777f7b4955c787(xd669244d58bc09c, x5786461d089b10a0, ref x43163d22e8cd5a71, out num2);
					flag2 = false;
					if (wireType != WireType.None)
					{
						goto IL_4A0;
					}
					type = TypeModel.x4431fb61faccece0(this, x43163d22e8cd5a71);
					if ((uint)num2 + (uint)num < 0U)
					{
						goto IL_576;
					}
					if (type != null)
					{
						goto IL_64A;
					}
					goto IL_6B2;
					IL_5D0:
					if (x43163d22e8cd5a71 != typeof(byte[]))
					{
						type = x43163d22e8cd5a71.GetElementType();
						goto IL_4C3;
					}
					if ((x0d64564fe23d43d4 ? 1U : 0U) - (x75ef743805c2e831 ? 1U : 0U) < 0U)
					{
						goto IL_459;
					}
					if (((x309954897d422190 ? 1U : 0U) | 2147483647U) == 0U)
					{
						goto IL_4F8;
					}
					if ((x0d64564fe23d43d4 ? 1U : 0U) - (uint)num2 < 0U)
					{
						continue;
					}
					flag = ((uint)num2 + (x309954897d422190 ? 1U : 0U) > uint.MaxValue);
					if (flag)
					{
						goto IL_64A;
					}
					goto IL_5C6;
					IL_64F:
					if (x43163d22e8cd5a71.GetArrayRank() == 1)
					{
						goto IL_5D0;
					}
					flag = ((x0d64564fe23d43d4 ? 1U : 0U) - (x0d64564fe23d43d4 ? 1U : 0U) > uint.MaxValue);
					if (flag)
					{
						goto IL_5D0;
					}
					goto IL_4C3;
					IL_6B2:
					if (x43163d22e8cd5a71.IsArray)
					{
						goto IL_64F;
					}
					goto IL_4C3;
					IL_491:
					if (num <= 0)
					{
						goto Block_8;
					}
					if (num == xffe521cc76054baf)
					{
						xd669244d58bc09c0 xd669244d58bc09c2;
						for (;;)
						{
							flag2 = true;
							xe134235b3526fa75.Hint(wireType);
							if (num2 < 0)
							{
								xd669244d58bc09c2 = xd669244d58bc09c;
								switch (xd669244d58bc09c2)
								{
								case xd669244d58bc09c0.x795dc524dba3fd4b:
									goto IL_1DA;
								case xd669244d58bc09c0.xfb1fc02db7c42694:
									goto IL_1B2;
								case xd669244d58bc09c0.x2025ae83be2038f0:
									goto IL_1ED;
								case xd669244d58bc09c0.xc0f9b651d77da240:
									goto IL_19F;
								case xd669244d58bc09c0.x697a219ddc6427a9:
									goto IL_2F5;
								case xd669244d58bc09c0.xf12cc4804eec7b89:
									goto IL_24C;
								case xd669244d58bc09c0.x85254000935bfc25:
									goto IL_277;
								case xd669244d58bc09c0.x6cedabde5251b1e5:
									goto IL_25F;
								case xd669244d58bc09c0.x0b2292ab52b25d76:
									goto IL_239;
								case xd669244d58bc09c0.x394150f1be471c3c:
									goto IL_226;
								case xd669244d58bc09c0.x63374d6ffed4adeb:
									goto IL_172;
								case xd669244d58bc09c0.x94c083f2813272f4:
									xbcea506a33cf9111 = xe134235b3526fa75.ReadDouble();
									if ((uint)xffe521cc76054baf - (uint)num2 <= 4294967295U)
									{
										goto Block_10;
									}
									continue;
								case xd669244d58bc09c0.x18d3f7d37d3464ca:
									goto IL_12E;
								case xd669244d58bc09c0.x242851e6278ed355:
									goto IL_100;
								case (xd669244d58bc09c0)17:
									goto IL_4A0;
								case xd669244d58bc09c0.x4a498a651d07aefe:
									goto IL_CC;
								}
								goto Block_14;
							}
							goto IL_3C3;
						}
						Block_10:
						if (!false)
						{
							goto IL_4A0;
						}
						goto IL_5D0;
						Block_14:
						switch (xd669244d58bc09c2)
						{
						case xd669244d58bc09c0.x69ec9d2404a6b229:
							xbcea506a33cf9111 = BclHelpers.ReadTimeSpan(xe134235b3526fa75);
							goto IL_4A0;
						case xd669244d58bc09c0.x62a0c09ce3a5e8fb:
							xbcea506a33cf9111 = ProtoReader.AppendBytes((byte[])xbcea506a33cf9111, xe134235b3526fa75);
							goto IL_4A0;
						case xd669244d58bc09c0.x0217cda8370c1f17:
							xbcea506a33cf9111 = BclHelpers.ReadGuid(xe134235b3526fa75);
							goto IL_4A0;
						case xd669244d58bc09c0.xb405a444ca77e2d4:
							xbcea506a33cf9111 = new Uri(xe134235b3526fa75.ReadString());
							goto IL_4A0;
						default:
							goto IL_4A0;
						}
						IL_37F:
						flag = (((x75ef743805c2e831 ? 1U : 0U) & 0U) == 0U);
						if (flag)
						{
							goto IL_4A0;
						}
						goto IL_576;
						IL_2F5:
						xbcea506a33cf9111 = xe134235b3526fa75.ReadInt16();
						if (!false)
						{
							goto IL_4A0;
						}
						flag = ((x75ef743805c2e831 ? 1U : 0U) > uint.MaxValue);
						if (flag)
						{
							goto IL_347;
						}
						flag = ((uint)xffe521cc76054baf + (xeb87b08c9033babc ? 1U : 0U) < 0U);
						if (flag)
						{
							goto IL_37F;
						}
						goto IL_239;
						IL_25F:
						xbcea506a33cf9111 = xe134235b3526fa75.ReadUInt32();
						goto IL_37F;
						IL_CC:
						xbcea506a33cf9111 = xe134235b3526fa75.ReadString();
						IL_129:
						goto IL_4A0;
						IL_3C3:
						WireType wireType2 = wireType;
						flag = ((x75ef743805c2e831 ? 1U : 0U) < 0U);
						if (flag)
						{
							if ((x0d64564fe23d43d4 ? 1U : 0U) + (flag2 ? 1U : 0U) <= 4294967295U)
							{
								goto Block_22;
							}
							goto IL_129;
						}
						else
						{
							switch (wireType2)
							{
							case WireType.String:
							case WireType.StartGroup:
								goto IL_39C;
							default:
								xbcea506a33cf9111 = this.Deserialize(num2, xbcea506a33cf9111, xe134235b3526fa75);
								if ((x75ef743805c2e831 ? 1U : 0U) <= 4294967295U)
								{
									goto IL_4A0;
								}
								goto IL_28;
							}
						}
						IL_100:
						xbcea506a33cf9111 = BclHelpers.ReadDateTime(xe134235b3526fa75);
						if ((flag2 ? 1U : 0U) + (flag2 ? 1U : 0U) <= 4294967295U)
						{
							goto IL_129;
						}
						return flag2;
						IL_12E:
						xbcea506a33cf9111 = BclHelpers.ReadDecimal(xe134235b3526fa75);
						goto IL_4A0;
						IL_172:
						xbcea506a33cf9111 = xe134235b3526fa75.ReadSingle();
						if ((xeb87b08c9033babc ? 1U : 0U) - (x75ef743805c2e831 ? 1U : 0U) >= 0U)
						{
							goto IL_4A0;
						}
						goto IL_25A;
						IL_19F:
						xbcea506a33cf9111 = xe134235b3526fa75.ReadByte();
						goto IL_4A0;
						IL_1B2:
						xbcea506a33cf9111 = (char)xe134235b3526fa75.ReadUInt16();
						goto IL_4A0;
						IL_226:
						xbcea506a33cf9111 = xe134235b3526fa75.ReadUInt64();
						goto IL_4C1;
						IL_1DA:
						xbcea506a33cf9111 = xe134235b3526fa75.ReadBoolean();
						goto IL_4A0;
						IL_1ED:
						xbcea506a33cf9111 = xe134235b3526fa75.ReadSByte();
						goto IL_4A0;
						IL_239:
						xbcea506a33cf9111 = xe134235b3526fa75.ReadInt64();
						IL_25A:
						goto IL_4A0;
						IL_24C:
						xbcea506a33cf9111 = xe134235b3526fa75.ReadUInt16();
						goto IL_25A;
						IL_277:
						xbcea506a33cf9111 = xe134235b3526fa75.ReadInt32();
						goto IL_4A0;
					}
					if (xeb87b08c9033babc)
					{
						goto IL_47A;
					}
					if (!false)
					{
						goto IL_459;
					}
					goto IL_39C;
					IL_4C1:
					goto IL_515;
					IL_39C:
					SubItemToken token = ProtoReader.StartSubItem(xe134235b3526fa75);
					xbcea506a33cf9111 = this.Deserialize(num2, xbcea506a33cf9111, xe134235b3526fa75);
					flag = ((uint)num < 0U);
					if (flag)
					{
						goto IL_47A;
					}
					flag = ((uint)num2 + (uint)num < 0U);
					if (flag)
					{
						goto IL_4C1;
					}
					IL_347:
					ProtoReader.EndSubItem(token, xe134235b3526fa75);
					goto IL_4A0;
					IL_47A:
					xe134235b3526fa75.SkipField();
					IL_4A0:
					if (flag2 && x0d64564fe23d43d4)
					{
						goto Block_9;
					}
					num = xe134235b3526fa75.ReadFieldHeader();
					goto IL_491;
					IL_576:
					if (!x309954897d422190)
					{
						flag2 = this.xcf163c468d9e8a34(this, xe134235b3526fa75, x5786461d089b10a0, xffe521cc76054baf, x43163d22e8cd5a71, type, ref xbcea506a33cf9111);
						while (flag2)
						{
							if (true)
							{
								return flag2;
							}
							flag = ((x0d64564fe23d43d4 ? 1U : 0U) > uint.MaxValue);
							if (!flag)
							{
								IL_4F4:
								if (!x75ef743805c2e831)
								{
									goto IL_4F8;
								}
								xbcea506a33cf9111 = TypeModel.x8b0978848b8680a2(x43163d22e8cd5a71, type);
								if (((uint)num2 & 0U) != 0U)
								{
									goto IL_5C6;
								}
								if (((xeb87b08c9033babc ? 1U : 0U) & 0U) != 0U)
								{
									goto Block_38;
								}
								if ((x0d64564fe23d43d4 ? 1U : 0U) + (x0d64564fe23d43d4 ? 1U : 0U) > 4294967295U)
								{
									goto IL_64F;
								}
								if ((x75ef743805c2e831 ? 1U : 0U) - (xeb87b08c9033babc ? 1U : 0U) >= 0U)
								{
									goto Block_29;
								}
								goto IL_491;
							}
						}
						goto IL_4F4;
					}
					goto IL_592;
					IL_4C3:
					if (type != null)
					{
						goto IL_576;
					}
					TypeModel.ThrowUnexpectedType(x43163d22e8cd5a71);
					flag = ((x309954897d422190 ? 1U : 0U) - (flag2 ? 1U : 0U) > uint.MaxValue);
					if (flag)
					{
						goto IL_515;
					}
					goto IL_4A0;
					IL_64A:
					IL_5C6:
					goto IL_4C3;
					IL_515:
					flag = ((flag2 ? 1U : 0U) < 0U);
					if (flag)
					{
						goto Block_28;
					}
					flag = (((flag2 ? 1U : 0U) | 2147483648U) == 0U);
					if (flag)
					{
						goto Block_44;
					}
					goto IL_4A0;
				}
				IL_11:
				if (!x75ef743805c2e831)
				{
					return flag2;
				}
				if (x43163d22e8cd5a71 == typeof(string))
				{
					return flag2;
				}
				xbcea506a33cf9111 = Activator.CreateInstance(x43163d22e8cd5a71);
				IL_28:
				if ((uint)num2 <= 4294967295U)
				{
					return flag2;
				}
				goto IL_70;
				IL_3C:
				if (x0d64564fe23d43d4)
				{
					return flag2;
				}
				if ((flag2 ? 1U : 0U) - (uint)xffe521cc76054baf >= 0U)
				{
					goto IL_11;
				}
				goto IL_412;
				IL_70:
				if (!flag2)
				{
					goto IL_3C;
				}
				return flag2;
				Block_8:
				Block_9:
				goto IL_70;
				IL_412:
				object[] array;
				array[1] = xffe521cc76054baf;
				if ((uint)num2 <= 4294967295U)
				{
					array[2] = ", but found ";
					array[3] = num;
					throw ProtoReader.xe0800720badf45f1(new InvalidOperationException(string.Concat(array)), xe134235b3526fa75);
				}
				goto IL_11;
				Block_22:
				IL_459:
				array = new object[4];
				array[0] = "Expected field ";
				goto IL_412;
				IL_4F8:
				return flag2;
				Block_28:
				goto IL_726;
				Block_29:
				return flag2;
				IL_592:
				throw TypeModel.x65d41140f79f7ce3();
				Block_38:
				goto IL_726;
				Block_44:
				goto IL_3C;
			}
			IL_726:
			throw new ArgumentNullException("type");
		}

		public static RuntimeTypeModel Create()
		{
			return new RuntimeTypeModel(false);
		}

		protected internal static Type ResolveProxies(Type type)
		{
			Type[] interfaces;
			int num;
			if (type == null)
			{
				if (false)
				{
					goto IL_34;
				}
				goto IL_D2;
			}
			else
			{
				if (type.IsGenericParameter)
				{
					return null;
				}
				Type type2 = x479f2661aae93792.xe5e08d1dc9f521de(type);
				if (type2 != null)
				{
					return type2;
				}
				if (type.FullName.StartsWith("System.Data.Entity.DynamicProxies."))
				{
					return type.BaseType;
				}
				interfaces = type.GetInterfaces();
				num = 0;
			}
			IL_0E:
			if (num < interfaces.Length)
			{
				goto IL_34;
			}
			bool flag = (uint)num > uint.MaxValue;
			if (flag)
			{
				goto IL_D2;
			}
			return null;
			IL_2E:
			num++;
			goto IL_0E;
			IL_34:
			string fullName;
			if ((fullName = interfaces[num].FullName) == null)
			{
				if ((uint)num + (uint)num >= 0U)
				{
					goto IL_2E;
				}
			}
			else
			{
				if (!(fullName == "NHibernate.Proxy.INHibernateProxy") && !(fullName == "NHibernate.Proxy.DynamicProxy.IProxy"))
				{
					goto IL_71;
				}
				goto IL_74;
			}
			IL_42:
			if (!(fullName == "NHibernate.Intercept.IFieldInterceptorAccessor"))
			{
				goto IL_2E;
			}
			goto IL_74;
			IL_71:
			if (!false)
			{
				goto IL_42;
			}
			IL_74:
			return type.BaseType;
			IL_D2:
			if ((uint)num - (uint)num <= 4294967295U)
			{
				return null;
			}
			goto IL_71;
		}

		public bool IsDefined(Type type)
		{
			return this.GetKey(ref type) >= 0;
		}

		protected internal int GetKey(ref Type type)
		{
			int keyImpl = this.GetKeyImpl(type);
			if (keyImpl < 0)
			{
				Type type2 = TypeModel.ResolveProxies(type);
				if (type2 != null)
				{
					type = type2;
					keyImpl = this.GetKeyImpl(type);
				}
			}
			return keyImpl;
		}

		protected abstract int GetKeyImpl(Type type);

		protected internal abstract void Serialize(int key, object value, ProtoWriter dest);

		protected internal abstract object Deserialize(int key, object value, ProtoReader source);

		public object DeepClone(object value)
		{
			if (value != null)
			{
				Type type = value.GetType();
				int key = this.GetKey(ref type);
				if (key >= 0)
				{
					if (!x479f2661aae93792.xb636fcab7a16c388(type))
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							using (ProtoWriter protoWriter = new ProtoWriter(memoryStream, this, null))
							{
								protoWriter.SetRootObject(value);
								this.Serialize(key, value, protoWriter);
								protoWriter.Close();
							}
							memoryStream.Position = 0L;
							using (ProtoReader protoReader = new ProtoReader(memoryStream, this, null))
							{
								return this.Deserialize(key, null, protoReader);
							}
						}
						goto IL_20A;
					}
				}
				int num;
				if (type != typeof(byte[]))
				{
					if (this.x24777f7b4955c787(x479f2661aae93792.xf70eec89828a813c(type), DataFormat.Default, ref type, out num) != WireType.None)
					{
						if ((uint)key >= 0U)
						{
							goto IL_D8;
						}
						object result;
						return result;
					}
				}
				else
				{
					byte[] array = (byte[])value;
					byte[] array2 = new byte[array.Length];
					if (-2147483648 != 0)
					{
						x479f2661aae93792.x6a87193e5bb23362(array, 0, array2, 0, array.Length);
						return array2;
					}
					goto IL_D8;
				}
				IL_39:
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					using (ProtoWriter protoWriter2 = new ProtoWriter(memoryStream2, this, null))
					{
						if (!this.x07feef0c759efbcc(protoWriter2, type, DataFormat.Default, 1, value, false))
						{
							TypeModel.ThrowUnexpectedType(type);
						}
						protoWriter2.Close();
					}
					memoryStream2.Position = 0L;
					using (ProtoReader protoReader2 = new ProtoReader(memoryStream2, this, null))
					{
						value = null;
						this.x3d653e8159e64ad2(protoReader2, DataFormat.Default, 1, type, ref value, true, false, true, false);
						return value;
					}
				}
				IL_D8:
				bool flag = (uint)key < 0U;
				if (!flag && num >= 0)
				{
					goto IL_39;
				}
				return value;
			}
			IL_20A:
			return null;
		}

		protected internal static void ThrowUnexpectedSubtype(Type expected, Type actual)
		{
			if (expected != TypeModel.ResolveProxies(actual))
			{
				throw new InvalidOperationException("Unexpected sub-type: " + actual.FullName);
			}
		}

		protected internal static void ThrowUnexpectedType(Type type)
		{
			string text;
			if (type == null)
			{
				text = "(unknown)";
				goto IL_70;
			}
			IL_63:
			text = type.FullName;
			IL_70:
			string str = text;
			if (type != null)
			{
				Type baseType = type.BaseType;
				if (baseType != null)
				{
					goto IL_21;
				}
				IL_14:
				if (!false)
				{
					if (false)
					{
						goto IL_2B;
					}
					if (false)
					{
						goto IL_63;
					}
					goto IL_7C;
				}
				IL_21:
				if (!baseType.IsGenericType)
				{
					if (!false)
					{
						goto IL_7C;
					}
					if (!false)
					{
						goto IL_14;
					}
					goto IL_42;
				}
				IL_2B:
				if (!(baseType.GetGenericTypeDefinition().Name == "GeneratedMessage`2"))
				{
					goto IL_7C;
				}
				IL_42:
				throw new InvalidOperationException("Are you mixing protobuf-net and protobuf-csharp-port? See http://stackoverflow.com/q/11564914; type: " + str);
			}
			IL_7C:
			throw new InvalidOperationException("Type is not expected, and no contract can be inferred: " + str);
		}

		internal static Exception x65d41140f79f7ce3()
		{
			return new NotSupportedException("Nested or jagged lists and arrays are not supported");
		}

		public static void ThrowCannotCreateInstance(Type type)
		{
			throw new ProtoException("No parameterless constructor found for " + type.Name);
		}

		internal static string x0885c39348d7aa9b(TypeModel xad70a5849826ecef, Type x43163d22e8cd5a71)
		{
			if (xad70a5849826ecef != null)
			{
				TypeFormatEventHandler typeFormatEventHandler;
				if (!false && (typeFormatEventHandler = xad70a5849826ecef.xb02444069fec74f5) == null)
				{
					if (!false)
					{
					}
				}
				else
				{
					TypeFormatEventArgs typeFormatEventArgs = new TypeFormatEventArgs(x43163d22e8cd5a71);
					typeFormatEventHandler(xad70a5849826ecef, typeFormatEventArgs);
					if (!x479f2661aae93792.x1c140bd1078ddda1(typeFormatEventArgs.FormattedName))
					{
						return typeFormatEventArgs.FormattedName;
					}
				}
			}
			return x43163d22e8cd5a71.AssemblyQualifiedName;
		}

		internal static Type x34a9527dfd927b35(TypeModel xad70a5849826ecef, string xbcea506a33cf9111)
		{
			if (xad70a5849826ecef != null)
			{
				if (2 != 0)
				{
					if (false)
					{
						goto IL_4B;
					}
					goto IL_2C;
				}
				IL_0D:
				if (3 != 0)
				{
					goto IL_68;
				}
				IL_0F:
				TypeFormatEventArgs typeFormatEventArgs;
				if (typeFormatEventArgs.Type != null)
				{
					return typeFormatEventArgs.Type;
				}
				if (!false)
				{
					goto IL_68;
				}
				if (15 != 0)
				{
					goto IL_68;
				}
				IL_2C:
				TypeFormatEventHandler typeFormatEventHandler;
				if ((typeFormatEventHandler = xad70a5849826ecef.xb02444069fec74f5) == null)
				{
					if (15 != 0)
					{
						goto IL_0D;
					}
					goto IL_0D;
				}
				IL_4B:
				typeFormatEventArgs = new TypeFormatEventArgs(xbcea506a33cf9111);
				typeFormatEventHandler(xad70a5849826ecef, typeFormatEventArgs);
				if (-2 == 0)
				{
					goto IL_2C;
				}
				goto IL_0F;
			}
			IL_68:
			return Type.GetType(xbcea506a33cf9111);
		}

		public bool CanSerializeContractType(Type type)
		{
			return this.xa18f5e57f5af8cf0(type, false, true, true);
		}

		public bool CanSerialize(Type type)
		{
			return this.xa18f5e57f5af8cf0(type, true, true, true);
		}

		public bool CanSerializeBasicType(Type type)
		{
			return this.xa18f5e57f5af8cf0(type, true, false, true);
		}

		private bool xa18f5e57f5af8cf0(Type x43163d22e8cd5a71, bool xda84049568340c0e, bool x47ab437ca7d231da, bool x2199598070498065)
		{
			if (x43163d22e8cd5a71 == null)
			{
				throw new ArgumentNullException("type");
			}
			Type type = x479f2661aae93792.xe5e08d1dc9f521de(x43163d22e8cd5a71);
			if (type == null)
			{
				goto IL_C7;
			}
			int key;
			if ((uint)key - (xda84049568340c0e ? 1U : 0U) > 4294967295U)
			{
				goto IL_74;
			}
			bool flag = (xda84049568340c0e ? 1U : 0U) - (x2199598070498065 ? 1U : 0U) > uint.MaxValue;
			if (!flag)
			{
				goto IL_C4;
			}
			IL_0B:
			Type type2 = TypeModel.x4431fb61faccece0(this, x43163d22e8cd5a71);
			flag = (((xda84049568340c0e ? 1U : 0U) & 0U) == 0U);
			if (!flag)
			{
				goto IL_51;
			}
			IL_32:
			if (type2 == null)
			{
				return false;
			}
			IL_38:
			return this.xa18f5e57f5af8cf0(type2, xda84049568340c0e, x47ab437ca7d231da, false);
			IL_51:
			if ((x2199598070498065 ? 1U : 0U) + (xda84049568340c0e ? 1U : 0U) > 4294967295U)
			{
				goto IL_38;
			}
			if (!x43163d22e8cd5a71.IsArray)
			{
				goto IL_0B;
			}
			if (x43163d22e8cd5a71.GetArrayRank() != 1)
			{
				goto IL_32;
			}
			type2 = x43163d22e8cd5a71.GetElementType();
			goto IL_A5;
			IL_74:
			key = this.GetKey(ref x43163d22e8cd5a71);
			if (key >= 0)
			{
				if (2 != 0)
				{
					return x47ab437ca7d231da;
				}
				goto IL_C4;
			}
			else
			{
				if (!x2199598070498065)
				{
					return false;
				}
				type2 = null;
				flag = ((x2199598070498065 ? 1U : 0U) < 0U);
				if (!flag)
				{
					goto IL_51;
				}
			}
			IL_A5:
			goto IL_32;
			IL_C4:
			x43163d22e8cd5a71 = type;
			IL_C7:
			switch (x479f2661aae93792.xf70eec89828a813c(x43163d22e8cd5a71))
			{
			case xd669244d58bc09c0.x45260ad4b94166f2:
			case xd669244d58bc09c0.xf6c17f648b65c793:
				goto IL_74;
			default:
				return xda84049568340c0e;
			}
			return false;
		}

		public virtual string GetSchema(Type type)
		{
			throw new NotSupportedException();
		}

		public event TypeFormatEventHandler DynamicTypeFormatting
		{
			add
			{
				TypeFormatEventHandler typeFormatEventHandler = this.xb02444069fec74f5;
				TypeFormatEventHandler typeFormatEventHandler2;
				do
				{
					typeFormatEventHandler2 = typeFormatEventHandler;
					TypeFormatEventHandler value2 = (TypeFormatEventHandler)Delegate.Combine(typeFormatEventHandler2, value);
					typeFormatEventHandler = Interlocked.CompareExchange<TypeFormatEventHandler>(ref this.xb02444069fec74f5, value2, typeFormatEventHandler2);
				}
				while (typeFormatEventHandler != typeFormatEventHandler2);
			}
			remove
			{
				TypeFormatEventHandler typeFormatEventHandler = this.xb02444069fec74f5;
				TypeFormatEventHandler typeFormatEventHandler2;
				do
				{
					typeFormatEventHandler2 = typeFormatEventHandler;
					TypeFormatEventHandler value2 = (TypeFormatEventHandler)Delegate.Remove(typeFormatEventHandler2, value);
					typeFormatEventHandler = Interlocked.CompareExchange<TypeFormatEventHandler>(ref this.xb02444069fec74f5, value2, typeFormatEventHandler2);
				}
				while (typeFormatEventHandler != typeFormatEventHandler2);
			}
		}

		internal virtual Type xd6886399673b411b(string x01366b5d2e943495, Assembly x0f7b23d1c393aed9)
		{
			return TypeModel.xad1a6254e789134f(x01366b5d2e943495, this, x0f7b23d1c393aed9);
		}

		internal static Type xad1a6254e789134f(string xc15bd84e01929885, TypeModel xad70a5849826ecef, Assembly xd3764619ec304ff0)
		{
			if (x479f2661aae93792.x1c140bd1078ddda1(xc15bd84e01929885))
			{
				return null;
			}
			try
			{
				Type type = Type.GetType(xc15bd84e01929885);
				if (type != null)
				{
					return type;
				}
			}
			catch
			{
			}
			try
			{
				int num = xc15bd84e01929885.IndexOf(',');
				string name = ((num > 0) ? xc15bd84e01929885.Substring(0, num) : xc15bd84e01929885).Trim();
				while (xd3764619ec304ff0 != null)
				{
					if ((uint)num >= 0U)
					{
						if (!true)
						{
							goto IL_10E;
						}
						IL_D6:
						if (xd3764619ec304ff0 != null)
						{
							goto IL_10E;
						}
						IL_D9:
						Type type2 = null;
						IL_DA:
						Type type3 = type2;
						for (;;)
						{
							IL_E7:
							while (type3 == null)
							{
								if ((uint)num - (uint)num <= 4294967295U)
								{
									if (((uint)num | 3U) != 0U)
									{
										while ((uint)num + (uint)num < 0U)
										{
											if ((uint)num - (uint)num < 0U)
											{
												goto IL_E7;
											}
											if (false)
											{
												goto Block_9;
											}
											if (!false)
											{
												break;
											}
										}
										goto Block_6;
									}
									goto IL_EE;
								}
							}
							goto IL_DD;
						}
						Block_6:
						goto IL_140;
						Block_9:
						goto IL_D6;
						IL_DD:
						return type3;
						IL_EE:
						goto IL_D9;
						IL_140:
						goto IL_145;
						IL_10E:
						type2 = xd3764619ec304ff0.GetType(name);
						goto IL_DA;
					}
				}
				xd3764619ec304ff0 = Assembly.GetCallingAssembly();
				goto IL_D6;
			}
			catch
			{
			}
			IL_145:
			return null;
		}

		private static readonly Type xdc694e00cd3ff400 = typeof(IList);

		private TypeFormatEventHandler xb02444069fec74f5;

		private class xd7bfba715d076364 : IEnumerable, IEnumerator
		{
			IEnumerator IEnumerable.x05b0b83b5e6c5de6()
			{
				return this;
			}

			public bool MoveNext()
			{
				if (this.xd1ddd4337de43a81)
				{
					int num;
					this.x3bd62873fafa6252 = this.xad70a5849826ecef.x5c075dfed546c92d(this.x337e217cb3ba0627, null, this.x43163d22e8cd5a71, this.x44ecfea61c937b8e, this.x259d864bb556a52f, this.x59b00779808aa981, out num, out this.xd1ddd4337de43a81, this.x0f7b23d1c393aed9);
				}
				return this.xd1ddd4337de43a81;
			}

			void IEnumerator.xf80999337bada71a()
			{
				throw new NotSupportedException();
			}

			public object Current
			{
				get
				{
					return this.x3bd62873fafa6252;
				}
			}

			public xd7bfba715d076364(TypeModel model, Stream source, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, SerializationContext context)
			{
				if (!false)
				{
					this.xd1ddd4337de43a81 = true;
					this.x337e217cb3ba0627 = source;
					this.x43163d22e8cd5a71 = type;
					this.x44ecfea61c937b8e = style;
					this.x259d864bb556a52f = expectedField;
				}
				this.x59b00779808aa981 = resolver;
				this.xad70a5849826ecef = model;
				this.x0f7b23d1c393aed9 = context;
			}

			private bool xd1ddd4337de43a81;

			private object x3bd62873fafa6252;

			private readonly Stream x337e217cb3ba0627;

			private readonly Type x43163d22e8cd5a71;

			private readonly PrefixStyle x44ecfea61c937b8e;

			private readonly int x259d864bb556a52f;

			private readonly Serializer.TypeResolver x59b00779808aa981;

			private readonly TypeModel xad70a5849826ecef;

			private readonly SerializationContext x0f7b23d1c393aed9;
		}

		private class x4f14c9867bda75fa<T> : TypeModel.xd7bfba715d076364, IEnumerator<T>, IEnumerable<T>, IDisposable, IEnumerable, IEnumerator
		{
			IEnumerator<T> IEnumerable<!0>.xce347ded92c0b1ea()
			{
				return this;
			}

			public new T Current
			{
				get
				{
					return (T)((object)base.Current);
				}
			}

			void IDisposable.x4852c3ca92ab200a()
			{
			}

			public x4f14c9867bda75fa(TypeModel model, Stream source, PrefixStyle style, int expectedField, SerializationContext context) : base(model, source, model.MapType(typeof(T)), style, expectedField, null, context)
			{
			}
		}

		protected internal enum CallbackType
		{
			BeforeSerialize,
			AfterSerialize,
			BeforeDeserialize,
			AfterDeserialize
		}
	}
}
