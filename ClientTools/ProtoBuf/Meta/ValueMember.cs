using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using ProtoBuf.Serializers;

namespace ProtoBuf.Meta
{
	public class ValueMember
	{
		public int FieldNumber
		{
			get
			{
				return this.xade3b695478596d6;
			}
		}

		public MemberInfo Member
		{
			get
			{
				return this.xf0b74f36659f8180;
			}
		}

		public Type ItemType
		{
			get
			{
				return this.xd99217279677497c;
			}
		}

		public Type MemberType
		{
			get
			{
				return this.x4d289d7c3d9f4b5d;
			}
		}

		public Type DefaultType
		{
			get
			{
				return this.x284cae105ee79236;
			}
		}

		public Type ParentType
		{
			get
			{
				return this.xb9962183f21940a7;
			}
		}

		public object DefaultValue
		{
			get
			{
				return this.xc6e85c82d0d89508;
			}
			set
			{
				this.x2eda32a9e793a6b9();
				this.xc6e85c82d0d89508 = value;
			}
		}

		public ValueMember(RuntimeTypeModel model, Type parentType, int fieldNumber, MemberInfo member, Type memberType, Type itemType, Type defaultType, DataFormat dataFormat, object defaultValue) : this(model, fieldNumber, memberType, itemType, defaultType, dataFormat)
		{
			bool flag = (uint)fieldNumber > uint.MaxValue;
			if (!flag)
			{
				if (((uint)fieldNumber & 0U) != 0U || member == null)
				{
					goto IL_1A4;
				}
				if (parentType == null)
				{
					throw new ArgumentNullException("parentType");
				}
				if (fieldNumber >= 1)
				{
					goto IL_125;
				}
				goto IL_183;
			}
			IL_25:
			if (false)
			{
				goto IL_65;
			}
			IL_28:
			MetaType metaType;
			if (metaType == null)
			{
				return;
			}
			IL_35:
			this.xa9226d1a5a7dd054 = metaType.AsReferenceDefault;
			flag = ((uint)fieldNumber > uint.MaxValue);
			if (!flag)
			{
				return;
			}
			if (false)
			{
				goto IL_125;
			}
			flag = ((uint)fieldNumber + (uint)fieldNumber > uint.MaxValue);
			if (flag)
			{
				return;
			}
			goto IL_25;
			IL_65:
			flag = (((uint)fieldNumber & 0U) == 0U);
			if (!flag)
			{
				goto IL_1A4;
			}
			flag = ((uint)fieldNumber < 0U);
			if (flag)
			{
				goto IL_35;
			}
			goto IL_28;
			IL_125:
			this.xf0b74f36659f8180 = member;
			if (-2147483648 == 0)
			{
				goto IL_FC;
			}
			this.xb9962183f21940a7 = parentType;
			if (!false)
			{
				goto IL_BF;
			}
			goto IL_FC;
			IL_47:
			if (defaultValue != null)
			{
				goto IL_E1;
			}
			IL_4E:
			this.xc6e85c82d0d89508 = defaultValue;
			metaType = model.x36fdee630175c0fa(memberType);
			if (!false)
			{
				goto IL_65;
			}
			goto IL_183;
			IL_BF:
			if (fieldNumber >= 1)
			{
				if (!false)
				{
					flag = ((uint)fieldNumber + (uint)fieldNumber > uint.MaxValue);
					if (flag)
					{
						goto IL_E1;
					}
					goto IL_47;
				}
			}
			if (x479f2661aae93792.xb636fcab7a16c388(parentType))
			{
				goto IL_47;
			}
			throw new ArgumentOutOfRangeException("fieldNumber");
			IL_E1:
			if (model.MapType(defaultValue.GetType()) != memberType)
			{
				defaultValue = ValueMember.x4f4b3ff4cf23fdc4(memberType, defaultValue);
				goto IL_4E;
			}
			goto IL_4E;
			IL_FC:
			if (-2147483648 != 0)
			{
				goto IL_47;
			}
			goto IL_BF;
			IL_183:
			if (x479f2661aae93792.xb636fcab7a16c388(parentType))
			{
				goto IL_125;
			}
			if (true)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			goto IL_35;
			IL_1A4:
			throw new ArgumentNullException("member");
		}

		internal ValueMember(RuntimeTypeModel model, int fieldNumber, Type memberType, Type itemType, Type defaultType, DataFormat dataFormat)
		{
			if (-2 == 0)
			{
				if (-2 != 0)
				{
					goto IL_62;
				}
				if (false)
				{
					return;
				}
			}
			else
			{
				if ((uint)fieldNumber - (uint)fieldNumber > 4294967295U)
				{
					goto IL_62;
				}
				bool flag = (uint)fieldNumber + (uint)fieldNumber < 0U;
				if (flag)
				{
					goto IL_9F;
				}
			}
			if (memberType == null)
			{
				goto IL_9F;
			}
			IL_62:
			if (model != null)
			{
				this.xade3b695478596d6 = fieldNumber;
				this.x4d289d7c3d9f4b5d = memberType;
				this.xd99217279677497c = itemType;
				this.x284cae105ee79236 = defaultType;
				this.xad70a5849826ecef = model;
				this.xb0fbb9918378a9ab = dataFormat;
				return;
			}
			throw new ArgumentNullException("model");
			IL_9F:
			throw new ArgumentNullException("memberType");
		}

		internal object x4f62632ce341aab7()
		{
			return ((FieldInfo)this.xf0b74f36659f8180).GetRawConstantValue();
		}

		private static object x4f4b3ff4cf23fdc4(Type x43163d22e8cd5a71, object xbcea506a33cf9111)
		{
			Type type = x479f2661aae93792.xe5e08d1dc9f521de(x43163d22e8cd5a71);
			if (type != null)
			{
				x43163d22e8cd5a71 = type;
			}
			string text;
			if (xbcea506a33cf9111 is string)
			{
				text = (string)xbcea506a33cf9111;
				if (x479f2661aae93792.xb636fcab7a16c388(x43163d22e8cd5a71))
				{
					return x479f2661aae93792.x475bc6fe627d885f(x43163d22e8cd5a71, text);
				}
				xd669244d58bc09c0 xd669244d58bc09c = x479f2661aae93792.xf70eec89828a813c(x43163d22e8cd5a71);
				if (false)
				{
					return text;
				}
				switch (xd669244d58bc09c)
				{
				case xd669244d58bc09c0.x795dc524dba3fd4b:
					goto IL_14D;
				case xd669244d58bc09c0.xfb1fc02db7c42694:
					if (text.Length == 1)
					{
						return text[0];
					}
					throw new FormatException("Single character expected: \"" + text + "\"");
				case xd669244d58bc09c0.x2025ae83be2038f0:
					return sbyte.Parse(text, NumberStyles.Integer, CultureInfo.InvariantCulture);
				case xd669244d58bc09c0.xc0f9b651d77da240:
					return byte.Parse(text, NumberStyles.Integer, CultureInfo.InvariantCulture);
				case xd669244d58bc09c0.x697a219ddc6427a9:
					return short.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case xd669244d58bc09c0.xf12cc4804eec7b89:
					return ushort.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case xd669244d58bc09c0.x85254000935bfc25:
					return int.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case xd669244d58bc09c0.x6cedabde5251b1e5:
					return uint.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case xd669244d58bc09c0.x0b2292ab52b25d76:
					return long.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case xd669244d58bc09c0.x394150f1be471c3c:
					return ulong.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case xd669244d58bc09c0.x63374d6ffed4adeb:
					return float.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case xd669244d58bc09c0.x94c083f2813272f4:
					return double.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case xd669244d58bc09c0.x18d3f7d37d3464ca:
					return decimal.Parse(text, NumberStyles.Any, CultureInfo.InvariantCulture);
				case xd669244d58bc09c0.x242851e6278ed355:
					return DateTime.Parse(text, CultureInfo.InvariantCulture);
				case (xd669244d58bc09c0)17:
					break;
				case xd669244d58bc09c0.x4a498a651d07aefe:
					return text;
				default:
					switch (xd669244d58bc09c)
					{
					case xd669244d58bc09c0.x69ec9d2404a6b229:
						return TimeSpan.Parse(text);
					case xd669244d58bc09c0.x0217cda8370c1f17:
						return new Guid(text);
					case xd669244d58bc09c0.xb405a444ca77e2d4:
						return text;
					}
					break;
				}
			}
			if (x479f2661aae93792.xb636fcab7a16c388(x43163d22e8cd5a71))
			{
				return Enum.ToObject(x43163d22e8cd5a71, xbcea506a33cf9111);
			}
			if (2 != 0)
			{
				return Convert.ChangeType(xbcea506a33cf9111, x43163d22e8cd5a71, CultureInfo.InvariantCulture);
			}
			if (!false)
			{
				goto IL_14D;
			}
			return text;
			IL_14D:
			return bool.Parse(text);
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

		public DataFormat DataFormat
		{
			get
			{
				return this.xb0fbb9918378a9ab;
			}
			set
			{
				this.x2eda32a9e793a6b9();
				this.xb0fbb9918378a9ab = value;
			}
		}

		public bool IsStrict
		{
			get
			{
				return this.x9b280141de83da2b(1);
			}
			set
			{
				this.x38efbc9b1b17e078(1, value, true);
			}
		}

		public bool IsPacked
		{
			get
			{
				return this.x9b280141de83da2b(2);
			}
			set
			{
				this.x38efbc9b1b17e078(2, value, true);
			}
		}

		public bool OverwriteList
		{
			get
			{
				return this.x9b280141de83da2b(8);
			}
			set
			{
				this.x38efbc9b1b17e078(8, value, true);
			}
		}

		public bool IsRequired
		{
			get
			{
				return this.x9b280141de83da2b(4);
			}
			set
			{
				this.x38efbc9b1b17e078(4, value, true);
			}
		}

		public bool AsReference
		{
			get
			{
				return this.xa9226d1a5a7dd054;
			}
			set
			{
				this.x2eda32a9e793a6b9();
				this.xa9226d1a5a7dd054 = value;
			}
		}

		public bool DynamicType
		{
			get
			{
				return this.x580143c5f0eb0511;
			}
			set
			{
				this.x2eda32a9e793a6b9();
				this.x580143c5f0eb0511 = value;
			}
		}

		public void SetSpecified(MethodInfo getSpecified, MethodInfo setSpecified)
		{
			if (getSpecified != null)
			{
				if (getSpecified.ReturnType == this.xad70a5849826ecef.MapType(typeof(bool)))
				{
					goto IL_FA;
				}
				goto IL_10B;
			}
			IL_25:
			if (setSpecified != null)
			{
				goto IL_8D;
			}
			IL_2B:
			this.x2eda32a9e793a6b9();
			this.x50ad0b40c9580760 = getSpecified;
			if (!false)
			{
				this.xede49acc9159386f = setSpecified;
				return;
			}
			IL_35:
			if (-2147483648 == 0)
			{
				goto IL_5A;
			}
			IL_3C:
			goto IL_41;
			IL_3E:
			if (false)
			{
				goto IL_35;
			}
			IL_41:
			throw new ArgumentException("Invalid pattern for setting member-specified", "setSpecified");
			IL_58:
			goto IL_35;
			IL_5A:
			ParameterInfo[] parameters;
			if ((parameters = setSpecified.GetParameters()).Length != 1)
			{
				if (3 == 0)
				{
					goto IL_123;
				}
				goto IL_3E;
			}
			IL_69:
			if (parameters[0].ParameterType == this.xad70a5849826ecef.MapType(typeof(bool)))
			{
				goto IL_2B;
			}
			if (!false)
			{
				goto IL_41;
			}
			goto IL_3E;
			IL_8D:
			if (setSpecified.ReturnType != this.xad70a5849826ecef.MapType(typeof(void)) && !false)
			{
				goto IL_41;
			}
			if (setSpecified.IsStatic)
			{
				goto IL_58;
			}
			goto IL_5A;
			IL_FA:
			if (!getSpecified.IsStatic)
			{
				while (getSpecified.GetParameters().Length == 0)
				{
					if (!false)
					{
						goto IL_25;
					}
					if (3 != 0)
					{
						if (2 == 0)
						{
							goto IL_123;
						}
						if (!false)
						{
							goto IL_8D;
						}
						if (!false)
						{
							goto IL_69;
						}
						goto IL_3C;
					}
				}
				goto IL_10E;
			}
			if (2147483647 != 0)
			{
				goto IL_10E;
			}
			IL_10B:
			if (false)
			{
				goto IL_FA;
			}
			IL_10E:
			throw new ArgumentException("Invalid pattern for checking member-specified", "getSpecified");
			IL_123:
			goto IL_10B;
		}

		private void x2eda32a9e793a6b9()
		{
			if (this.x38c907674383c2da != null)
			{
				throw new InvalidOperationException("The type cannot be changed once a serializer has been generated");
			}
		}

		private x66ec8c25e4c7547d x0864aaa2c6d0fbe0()
		{
			int num = 0;
			x66ec8c25e4c7547d result;
			try
			{
				this.xad70a5849826ecef.x2c0c646a23b0da22(ref num);
				Type type;
				x66ec8c25e4c7547d x66ec8c25e4c7547d;
				for (;;)
				{
					type = ((this.xd99217279677497c != null) ? this.xd99217279677497c : this.x4d289d7c3d9f4b5d);
					WireType wireType;
					x66ec8c25e4c7547d = ValueMember.xac123cbb8d7a8e65(this.xad70a5849826ecef, this.xb0fbb9918378a9ab, type, out wireType, this.xa9226d1a5a7dd054, this.x580143c5f0eb0511, this.OverwriteList, true);
					if (x66ec8c25e4c7547d == null)
					{
						goto Block_30;
					}
					for (;;)
					{
						IL_316:
						bool flag;
						if (this.xd99217279677497c != null)
						{
							if (this.SupportNull)
							{
								while (!this.IsPacked)
								{
									flag = (((uint)num | uint.MaxValue) == 0U);
									if (flag)
									{
										if (((uint)num & 0U) != 0U)
										{
											goto Block_28;
										}
									}
									else if ((uint)num <= 4294967295U)
									{
										if (((uint)num & 0U) != 0U)
										{
											goto IL_488;
										}
										goto IL_345;
									}
									if (15 != 0)
									{
										goto IL_345;
									}
								}
								goto IL_3E0;
							}
							if (((uint)num & 0U) == 0U)
							{
								goto IL_2DA;
							}
							IL_345:
							x66ec8c25e4c7547d = new x8f2d2e2582fd9f3b(1, wireType, this.IsStrict, x66ec8c25e4c7547d);
							x66ec8c25e4c7547d = new xdae1073def4c17e2(this.xad70a5849826ecef, x66ec8c25e4c7547d);
							x66ec8c25e4c7547d = new x8f2d2e2582fd9f3b(this.xade3b695478596d6, WireType.StartGroup, false, x66ec8c25e4c7547d);
							goto IL_2EE;
						}
						goto IL_2DA;
						IL_146:
						if (this.x4d289d7c3d9f4b5d == this.xad70a5849826ecef.MapType(typeof(Uri)))
						{
							x66ec8c25e4c7547d = new x7b64c37dbdfdeea6(this.xad70a5849826ecef, x66ec8c25e4c7547d);
						}
						IL_75:
						while (this.xf0b74f36659f8180 != null)
						{
							IL_12D:
							while (!(this.xf0b74f36659f8180 is PropertyInfo))
							{
								for (;;)
								{
									FieldInfo fieldInfo = this.xf0b74f36659f8180 as FieldInfo;
									flag = ((uint)num + (uint)num > uint.MaxValue);
									if (flag)
									{
										flag = ((uint)num > uint.MaxValue);
										if (flag)
										{
											goto IL_12D;
										}
									}
									else if (fieldInfo == null)
									{
										goto Block_7;
									}
									x66ec8c25e4c7547d = new x3e1a662e8bd7a9ca(this.xb9962183f21940a7, (FieldInfo)this.xf0b74f36659f8180, x66ec8c25e4c7547d);
									if ((uint)num <= 4294967295U)
									{
										break;
									}
									flag = ((uint)num - (uint)num < 0U);
									if (!flag)
									{
										goto IL_174;
									}
									flag = ((uint)num + (uint)num > uint.MaxValue);
									if (!flag)
									{
										goto IL_220;
									}
									if (-2 != 0)
									{
										goto IL_208;
									}
								}
								IL_2F:
								if (this.x50ad0b40c9580760 == null)
								{
									if (this.xede49acc9159386f == null)
									{
										goto Block_4;
									}
								}
								x66ec8c25e4c7547d = new x78d7a2d462b6e713(this.x50ad0b40c9580760, this.xede49acc9159386f, x66ec8c25e4c7547d);
								flag = (((uint)num & 0U) == 0U);
								if (flag)
								{
									if (4 == 0)
									{
										goto IL_75;
									}
									flag = (((uint)num & 0U) == 0U);
									if (flag)
									{
										goto IL_2A;
									}
									goto IL_75;
								}
								else
								{
									if ((uint)num + (uint)num <= 4294967295U)
									{
										goto IL_1D9;
									}
									goto IL_316;
								}
								IL_220:
								goto IL_1A0;
							}
							x66ec8c25e4c7547d = new xe9b44656581cbaf8(this.xad70a5849826ecef, this.xb9962183f21940a7, (PropertyInfo)this.xf0b74f36659f8180, x66ec8c25e4c7547d);
							goto IL_2F;
						}
						goto Block_6;
						IL_208:
						x66ec8c25e4c7547d = new x51c09eb60e11ce5c(this.xad70a5849826ecef, this.xc6e85c82d0d89508, x66ec8c25e4c7547d);
						goto IL_146;
						IL_18C:
						if (this.x50ad0b40c9580760 != null)
						{
							goto IL_146;
						}
						goto IL_208;
						IL_16C:
						if (this.IsRequired)
						{
							goto IL_174;
						}
						goto IL_18C;
						IL_1D9:
						if (this.xc6e85c82d0d89508 != null)
						{
							goto IL_16C;
						}
						goto IL_146;
						IL_2EE:
						if (this.xd99217279677497c == null)
						{
							goto IL_1D9;
						}
						if (this.SupportNull)
						{
							goto IL_488;
						}
						x479f2661aae93792.xe5e08d1dc9f521de(this.xd99217279677497c);
						goto IL_27A;
						IL_2DA:
						x66ec8c25e4c7547d = new x8f2d2e2582fd9f3b(this.xade3b695478596d6, wireType, this.IsStrict, x66ec8c25e4c7547d);
						goto IL_2EE;
						IL_174:
						if (-2147483648 != 0)
						{
							goto IL_146;
						}
						goto IL_1A0;
						IL_27A:
						if (!this.x4d289d7c3d9f4b5d.IsArray)
						{
							x66ec8c25e4c7547d = new xdeb0ec0887c9560b(this.xad70a5849826ecef, this.x4d289d7c3d9f4b5d, this.x284cae105ee79236, x66ec8c25e4c7547d, this.xade3b695478596d6, this.IsPacked, wireType, this.xf0b74f36659f8180 != null && xe9b44656581cbaf8.x2eec4c68253a13df(this.xad70a5849826ecef, this.xf0b74f36659f8180), this.OverwriteList, this.SupportNull);
							goto IL_146;
						}
						x66ec8c25e4c7547d = new x72fd17eaff71827a(this.xad70a5849826ecef, x66ec8c25e4c7547d, this.xade3b695478596d6, this.IsPacked, wireType, this.x4d289d7c3d9f4b5d, this.OverwriteList, this.SupportNull);
						goto IL_146;
						IL_488:
						goto IL_27A;
						IL_1A0:
						flag = (((uint)num | 2U) == 0U);
						if (flag)
						{
							goto IL_16C;
						}
						goto IL_18C;
					}
					Block_28:
					if (3 == 0)
					{
						goto IL_3E0;
					}
				}
				IL_2A:
				result = x66ec8c25e4c7547d;
				goto IL_42;
				Block_4:
				if (!false)
				{
					goto IL_2A;
				}
				IL_42:
				return result;
				Block_6:
				goto IL_2A;
				Block_7:
				throw new InvalidOperationException();
				IL_3E0:
				throw new NotSupportedException("Packed encodings cannot support null values");
				Block_30:
				throw new InvalidOperationException("No serializer defined for type: " + type.FullName);
			}
			finally
			{
				this.xad70a5849826ecef.x7109eb31e17d8f9e(num);
			}
			return result;
		}

		private static WireType xe89a23f6e0b3c6c6(DataFormat x5786461d089b10a0, int x9b0739496f8b5475)
		{
			bool flag = ((uint)x9b0739496f8b5475 | 3U) == 0U;
			if (!flag)
			{
				switch (x5786461d089b10a0)
				{
				case DataFormat.Default:
				case DataFormat.TwosComplement:
					return WireType.Variant;
				case DataFormat.ZigZag:
					return WireType.SignedVariant;
				case DataFormat.FixedSize:
					if (x9b0739496f8b5475 != 32)
					{
						return WireType.Fixed64;
					}
					break;
				default:
					throw new InvalidOperationException();
				}
			}
			return WireType.Fixed32;
		}

		private static WireType xd718eecfa9bd246e(DataFormat x5786461d089b10a0)
		{
			switch (x5786461d089b10a0)
			{
			case DataFormat.Default:
				return WireType.String;
			case DataFormat.FixedSize:
				return WireType.Fixed64;
			case DataFormat.Group:
				return WireType.StartGroup;
			}
			throw new InvalidOperationException();
		}

		internal static x66ec8c25e4c7547d xac123cbb8d7a8e65(RuntimeTypeModel xad70a5849826ecef, DataFormat xb0fbb9918378a9ab, Type x43163d22e8cd5a71, out WireType x6c32308c01dbd660, bool xa9226d1a5a7dd054, bool x580143c5f0eb0511, bool xd6d603913157afbc, bool xa90ce13b8523a76e)
		{
			Type type;
			if ((type = x479f2661aae93792.xe5e08d1dc9f521de(x43163d22e8cd5a71)) != null)
			{
				goto IL_5B8;
			}
			IL_5B7:
			type = x43163d22e8cd5a71;
			IL_5B8:
			x43163d22e8cd5a71 = type;
			if (!x479f2661aae93792.xb636fcab7a16c388(x43163d22e8cd5a71))
			{
				bool flag = (xa9226d1a5a7dd054 ? 1U : 0U) + (xd6d603913157afbc ? 1U : 0U) < 0U;
				if (flag)
				{
					goto IL_58F;
				}
				xd669244d58bc09c0 xd669244d58bc09c = x479f2661aae93792.xf70eec89828a813c(x43163d22e8cd5a71);
				xd669244d58bc09c0 xd669244d58bc09c2 = xd669244d58bc09c;
				switch (xd669244d58bc09c2)
				{
				case xd669244d58bc09c0.x795dc524dba3fd4b:
					x6c32308c01dbd660 = WireType.Variant;
					return new x3ceb7f9b50cd16be(xad70a5849826ecef);
				case xd669244d58bc09c0.xfb1fc02db7c42694:
					goto IL_382;
				case xd669244d58bc09c0.x2025ae83be2038f0:
					x6c32308c01dbd660 = ValueMember.xe89a23f6e0b3c6c6(xb0fbb9918378a9ab, 32);
					return new x2c658bc58dc2f0e6(xad70a5849826ecef);
				case xd669244d58bc09c0.xc0f9b651d77da240:
					x6c32308c01dbd660 = ValueMember.xe89a23f6e0b3c6c6(xb0fbb9918378a9ab, 32);
					return new x268cc84c68b7a350(xad70a5849826ecef);
				case xd669244d58bc09c0.x697a219ddc6427a9:
					goto IL_2E6;
				case xd669244d58bc09c0.xf12cc4804eec7b89:
					x6c32308c01dbd660 = ValueMember.xe89a23f6e0b3c6c6(xb0fbb9918378a9ab, 32);
					goto IL_301;
				case xd669244d58bc09c0.x85254000935bfc25:
					x6c32308c01dbd660 = ValueMember.xe89a23f6e0b3c6c6(xb0fbb9918378a9ab, 32);
					return new x25a67bca27f4f84f(xad70a5849826ecef);
				case xd669244d58bc09c0.x6cedabde5251b1e5:
					x6c32308c01dbd660 = ValueMember.xe89a23f6e0b3c6c6(xb0fbb9918378a9ab, 32);
					return new x342bdf67bfdfab1b(xad70a5849826ecef);
				case xd669244d58bc09c0.x0b2292ab52b25d76:
					x6c32308c01dbd660 = ValueMember.xe89a23f6e0b3c6c6(xb0fbb9918378a9ab, 64);
					if ((x580143c5f0eb0511 ? 1U : 0U) + (xd6d603913157afbc ? 1U : 0U) <= 4294967295U)
					{
						goto IL_436;
					}
					goto IL_109;
				case xd669244d58bc09c0.x394150f1be471c3c:
					goto IL_454;
				case xd669244d58bc09c0.x63374d6ffed4adeb:
					x6c32308c01dbd660 = WireType.Fixed32;
					return new x18b706378f1b116b(xad70a5849826ecef);
				case xd669244d58bc09c0.x94c083f2813272f4:
					x6c32308c01dbd660 = WireType.Fixed64;
					return new xf81db36ccd457507(xad70a5849826ecef);
				case xd669244d58bc09c0.x18d3f7d37d3464ca:
					x6c32308c01dbd660 = WireType.String;
					return new xfe7f376894428b2b(xad70a5849826ecef);
				case xd669244d58bc09c0.x242851e6278ed355:
					x6c32308c01dbd660 = ValueMember.xd718eecfa9bd246e(xb0fbb9918378a9ab);
					return new xb532c08b9899b08a(xad70a5849826ecef);
				case (xd669244d58bc09c0)17:
					goto IL_277;
				case xd669244d58bc09c0.x4a498a651d07aefe:
					goto IL_3FE;
				default:
					goto IL_58F;
				}
				IL_1A5:
				BclHelpers.NetObjectOptions netObjectOptions;
				while (x580143c5f0eb0511)
				{
					netObjectOptions |= BclHelpers.NetObjectOptions.DynamicType;
					flag = (((xa90ce13b8523a76e ? 1U : 0U) & 0U) == 0U);
					if (flag)
					{
						break;
					}
				}
				goto IL_105;
				IL_12:
				int num;
				if (num >= 0)
				{
					x6c32308c01dbd660 = ((xb0fbb9918378a9ab == DataFormat.Group) ? WireType.StartGroup : WireType.String);
					goto IL_BE;
				}
				IL_33:
				x6c32308c01dbd660 = WireType.None;
				goto IL_157;
				IL_55:
				if (xad70a5849826ecef == null)
				{
					flag = ((xa90ce13b8523a76e ? 1U : 0U) - (x580143c5f0eb0511 ? 1U : 0U) < 0U);
					if (flag)
					{
						flag = ((xd6d603913157afbc ? 1U : 0U) - (xa9226d1a5a7dd054 ? 1U : 0U) < 0U);
						if (flag)
						{
							goto IL_157;
						}
						goto IL_12;
					}
					else if (((x580143c5f0eb0511 ? 1U : 0U) | 2147483648U) != 0U)
					{
						if ((xd6d603913157afbc ? 1U : 0U) + (x580143c5f0eb0511 ? 1U : 0U) <= 4294967295U)
						{
							goto IL_33;
						}
						goto IL_12;
					}
				}
				else
				{
					num = xad70a5849826ecef.xf15263674eb297bb(x43163d22e8cd5a71, false, true);
					if (xa9226d1a5a7dd054)
					{
						goto IL_1EB;
					}
					flag = ((uint)num < 0U);
					if (flag)
					{
						goto IL_2D1;
					}
					goto IL_32D;
				}
				IL_BE:
				return new xd825bbf60e1b9938(x43163d22e8cd5a71, num, xad70a5849826ecef[x43163d22e8cd5a71], true);
				IL_103:
				goto IL_117;
				IL_105:
				if (num < 0)
				{
					goto IL_103;
				}
				IL_109:
				if (xad70a5849826ecef[x43163d22e8cd5a71].UseConstructor)
				{
					netObjectOptions |= BclHelpers.NetObjectOptions.UseConstructor;
					flag = ((x580143c5f0eb0511 ? 1U : 0U) < 0U);
					if (flag)
					{
						flag = (((xa90ce13b8523a76e ? 1U : 0U) & 0U) == 0U);
						if (!flag)
						{
							goto IL_33;
						}
						flag = ((uint)num < 0U);
						if (flag)
						{
							goto IL_1C7;
						}
						flag = ((uint)num - (xd6d603913157afbc ? 1U : 0U) < 0U);
						if (flag)
						{
							goto IL_20D;
						}
						flag = ((xa9226d1a5a7dd054 ? 1U : 0U) - (xa9226d1a5a7dd054 ? 1U : 0U) < 0U);
						if (!flag)
						{
							goto IL_105;
						}
					}
				}
				IL_117:
				return new x2d308a71a493dcae(xad70a5849826ecef, x43163d22e8cd5a71, num, netObjectOptions);
				IL_157:
				goto IL_20D;
				IL_1C7:
				netObjectOptions |= BclHelpers.NetObjectOptions.AsReference;
				goto IL_1A5;
				IL_1EB:
				x6c32308c01dbd660 = WireType.String;
				IL_1EE:
				netObjectOptions = BclHelpers.NetObjectOptions.None;
				if (!xa9226d1a5a7dd054)
				{
					goto IL_1A5;
				}
				goto IL_1C7;
				IL_20D:
				if (false)
				{
					goto IL_32F;
				}
				goto IL_351;
				IL_277:
				x66ec8c25e4c7547d x66ec8c25e4c7547d = xad70a5849826ecef.AllowParseableTypes ? x7024e111a877c7fe.xf299a6067f9cb3b3(x43163d22e8cd5a71, xad70a5849826ecef) : null;
				if (x66ec8c25e4c7547d != null)
				{
					flag = (((xa9226d1a5a7dd054 ? 1U : 0U) & 0U) == 0U);
					if (flag)
					{
						x6c32308c01dbd660 = WireType.String;
						return x66ec8c25e4c7547d;
					}
					goto IL_12;
				}
				else
				{
					if (xa90ce13b8523a76e)
					{
						goto IL_55;
					}
					if ((xa90ce13b8523a76e ? 1U : 0U) + (x580143c5f0eb0511 ? 1U : 0U) > 4294967295U)
					{
						goto IL_3C6;
					}
					flag = ((xd6d603913157afbc ? 1U : 0U) + (uint)num > uint.MaxValue);
					if (!flag)
					{
						goto IL_33;
					}
					if ((uint)num - (xa90ce13b8523a76e ? 1U : 0U) <= 4294967295U)
					{
						goto IL_55;
					}
					goto IL_1EE;
				}
				IL_2D1:
				if ((uint)num < 0U)
				{
					goto IL_5BC;
				}
				IL_2E6:
				x6c32308c01dbd660 = ValueMember.xe89a23f6e0b3c6c6(xb0fbb9918378a9ab, 32);
				return new xc23284a0955df688(xad70a5849826ecef);
				IL_301:
				return new x5c17ccb70f35d083(xad70a5849826ecef);
				IL_32D:
				flag = ((xd6d603913157afbc ? 1U : 0U) + (uint)num > uint.MaxValue);
				if (flag)
				{
					goto IL_351;
				}
				if (false || -1 == 0)
				{
					goto IL_382;
				}
				if (!x580143c5f0eb0511)
				{
					goto IL_12;
				}
				goto IL_1EB;
				IL_32F:
				return new x70513e892c3247da(xad70a5849826ecef);
				IL_351:
				flag = ((xd6d603913157afbc ? 1U : 0U) + (xa9226d1a5a7dd054 ? 1U : 0U) > uint.MaxValue);
				if (flag)
				{
					goto IL_5B7;
				}
				goto IL_5F3;
				IL_382:
				x6c32308c01dbd660 = WireType.Variant;
				if (-2 == 0)
				{
					flag = ((x580143c5f0eb0511 ? 1U : 0U) - (xa9226d1a5a7dd054 ? 1U : 0U) > uint.MaxValue);
					if (flag)
					{
						goto IL_3FE;
					}
					goto IL_436;
				}
				else
				{
					flag = ((x580143c5f0eb0511 ? 1U : 0U) + (xa9226d1a5a7dd054 ? 1U : 0U) > uint.MaxValue);
					if (!flag)
					{
						goto IL_5D8;
					}
					flag = ((xa9226d1a5a7dd054 ? 1U : 0U) < 0U);
					if (flag)
					{
						goto IL_566;
					}
					goto IL_5B7;
				}
				IL_3C6:
				return new x6105c3343d9ac486(xad70a5849826ecef);
				IL_3FE:
				x6c32308c01dbd660 = WireType.String;
				if (!xa9226d1a5a7dd054)
				{
					goto IL_3C6;
				}
				if ((uint)num + (xd6d603913157afbc ? 1U : 0U) <= 4294967295U)
				{
					return new x2d308a71a493dcae(xad70a5849826ecef, xad70a5849826ecef.MapType(typeof(string)), 0, BclHelpers.NetObjectOptions.AsReference);
				}
				flag = (((xa90ce13b8523a76e ? 1U : 0U) & 0U) == 0U);
				if (flag)
				{
					goto IL_454;
				}
				goto IL_301;
				IL_436:
				return new x6f437e5b6820cb83(xad70a5849826ecef);
				IL_454:
				x6c32308c01dbd660 = ValueMember.xe89a23f6e0b3c6c6(xb0fbb9918378a9ab, 64);
				return new x0b9b8c9f0f6eb52b(xad70a5849826ecef);
				IL_58F:
				flag = ((xa9226d1a5a7dd054 ? 1U : 0U) + (xa90ce13b8523a76e ? 1U : 0U) < 0U);
				if (flag)
				{
					goto IL_5B7;
				}
				if (((xa90ce13b8523a76e ? 1U : 0U) & 0U) == 0U)
				{
					switch (xd669244d58bc09c2)
					{
					case xd669244d58bc09c0.x69ec9d2404a6b229:
						x6c32308c01dbd660 = ValueMember.xd718eecfa9bd246e(xb0fbb9918378a9ab);
						break;
					case xd669244d58bc09c0.x62a0c09ce3a5e8fb:
						x6c32308c01dbd660 = WireType.String;
						return new x2da9809a4c934e33(xad70a5849826ecef, xd6d603913157afbc);
					case xd669244d58bc09c0.x0217cda8370c1f17:
						x6c32308c01dbd660 = WireType.String;
						if (2147483647 != 0)
						{
							return new xbd5864a3ec0285e9(xad70a5849826ecef);
						}
						goto IL_55;
					case xd669244d58bc09c0.xb405a444ca77e2d4:
						x6c32308c01dbd660 = WireType.String;
						if (false)
						{
							flag = ((uint)num + (x580143c5f0eb0511 ? 1U : 0U) < 0U);
							if (!flag)
							{
								goto IL_2D1;
							}
						}
						else
						{
							flag = ((xa90ce13b8523a76e ? 1U : 0U) - (xd6d603913157afbc ? 1U : 0U) > uint.MaxValue);
							if (flag)
							{
								goto IL_32D;
							}
							return new x6105c3343d9ac486(xad70a5849826ecef);
						}
						break;
					case xd669244d58bc09c0.x3146d638ec378671:
						x6c32308c01dbd660 = WireType.String;
						return new xaaa25b0535f71777(xad70a5849826ecef);
					default:
						goto IL_277;
					}
					return new x2263624548b99182(xad70a5849826ecef);
				}
				IL_5D8:
				flag = ((xd6d603913157afbc ? 1U : 0U) - (uint)num > uint.MaxValue);
				if (!flag)
				{
					goto IL_32F;
				}
				IL_5F3:
				return null;
			}
			IL_566:
			if (xa90ce13b8523a76e)
			{
				if (xad70a5849826ecef != null)
				{
					goto IL_5BC;
				}
			}
			x6c32308c01dbd660 = WireType.None;
			return null;
			IL_5BC:
			x6c32308c01dbd660 = WireType.Variant;
			return new x9caf06b8b9c3a409(x43163d22e8cd5a71, xad70a5849826ecef.x0b51e8612d340c57(x43163d22e8cd5a71));
		}

		internal void x54f99ef1e934e59c(string xc15bd84e01929885)
		{
			this.x2eda32a9e793a6b9();
			this.xc15bd84e01929885 = xc15bd84e01929885;
		}

		public string Name
		{
			get
			{
				if (!x479f2661aae93792.x1c140bd1078ddda1(this.xc15bd84e01929885))
				{
					return this.xc15bd84e01929885;
				}
				return this.xf0b74f36659f8180.Name;
			}
		}

		private bool x9b280141de83da2b(byte x8fc2d66566293701)
		{
			return (this.xebf45bdcaa1fd1e1 & x8fc2d66566293701) == x8fc2d66566293701;
		}

		private void x38efbc9b1b17e078(byte x8fc2d66566293701, bool xbcea506a33cf9111, bool x826878e0588df0aa)
		{
			if (!x826878e0588df0aa)
			{
				goto IL_34;
			}
			IL_1F:
			if (false || this.x9b280141de83da2b(x8fc2d66566293701) != xbcea506a33cf9111)
			{
				this.x2eda32a9e793a6b9();
			}
			IL_34:
			if (xbcea506a33cf9111)
			{
				this.xebf45bdcaa1fd1e1 |= x8fc2d66566293701;
				return;
			}
			this.xebf45bdcaa1fd1e1 &= ~x8fc2d66566293701;
			if (15 == 0)
			{
				goto IL_1F;
			}
		}

		public bool SupportNull
		{
			get
			{
				return this.x9b280141de83da2b(16);
			}
			set
			{
				this.x38efbc9b1b17e078(16, value, true);
			}
		}

		internal string xe2423e5eb2c7ed45(bool xf8a8a7f7be37d265, ref bool x55b32f58d70e1b0f)
		{
			Type type = this.ItemType;
			if (type == null)
			{
				type = this.MemberType;
			}
			return this.xad70a5849826ecef.xe2423e5eb2c7ed45(type, this.DataFormat, xf8a8a7f7be37d265 && this.xa9226d1a5a7dd054, xf8a8a7f7be37d265 && this.x580143c5f0eb0511, ref x55b32f58d70e1b0f);
		}

		private const byte xb8a3a33236947e83 = 1;

		private const byte xccdf24f51a771cc1 = 2;

		private const byte x70bc3714cae36a24 = 4;

		private const byte x007cff453cb0ea17 = 8;

		private const byte x75e63ef75c60cbc1 = 16;

		private readonly int xade3b695478596d6;

		private readonly MemberInfo xf0b74f36659f8180;

		private readonly Type xb9962183f21940a7;

		private readonly Type xd99217279677497c;

		private readonly Type x284cae105ee79236;

		private readonly Type x4d289d7c3d9f4b5d;

		private object xc6e85c82d0d89508;

		private readonly RuntimeTypeModel xad70a5849826ecef;

		private x66ec8c25e4c7547d x38c907674383c2da;

		private DataFormat xb0fbb9918378a9ab;

		private bool xa9226d1a5a7dd054;

		private bool x580143c5f0eb0511;

		private MethodInfo x50ad0b40c9580760;

		private MethodInfo xede49acc9159386f;

		private string xc15bd84e01929885;

		private byte xebf45bdcaa1fd1e1;

		internal class xf7b8e510ae9ad738 : IComparer<ValueMember>, IComparer
		{
			public int Compare(object x, object y)
			{
				return this.Compare(x as ValueMember, y as ValueMember);
			}

			public int Compare(ValueMember x, ValueMember y)
			{
				if (object.ReferenceEquals(x, y))
				{
					return 0;
				}
				if (x != null)
				{
					while (y != null)
					{
						int fieldNumber = x.FieldNumber;
						if ((uint)fieldNumber - (uint)fieldNumber <= 4294967295U)
						{
							return fieldNumber.CompareTo(y.FieldNumber);
						}
					}
					return 1;
				}
				return -1;
			}

			public static readonly ValueMember.xf7b8e510ae9ad738 xb9715d2f06b63cf0 = new ValueMember.xf7b8e510ae9ad738();
		}
	}
}
