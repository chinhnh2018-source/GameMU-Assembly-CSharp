using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ProtoBuf.Serializers;

namespace ProtoBuf.Meta
{
	public class MetaType : xf359d5c298ca874d
	{
		public override string ToString()
		{
			return this.x43163d22e8cd5a71.ToString();
		}

		x66ec8c25e4c7547d xf359d5c298ca874d.xb14619514d709ba1
		{
			get
			{
				return this.x9e41724c73da1842;
			}
		}

		public MetaType BaseType
		{
			get
			{
				return this.x6227b26cefabe265;
			}
		}

		internal TypeModel xfd195ba400a3473c
		{
			get
			{
				return this.xad70a5849826ecef;
			}
		}

		public bool IncludeSerializerMethod
		{
			get
			{
				return !this.x9b280141de83da2b(8);
			}
			set
			{
				this.x38efbc9b1b17e078(8, !value, true);
			}
		}

		public bool AsReferenceDefault
		{
			get
			{
				return this.x9b280141de83da2b(32);
			}
			set
			{
				this.x38efbc9b1b17e078(32, value, true);
			}
		}

		private bool xbcd125d8e5fafe58(Type x69fcae6ed2142af0)
		{
			return this.x43163d22e8cd5a71.IsAssignableFrom(x69fcae6ed2142af0);
		}

		public MetaType AddSubType(int fieldNumber, Type derivedType)
		{
			return this.AddSubType(fieldNumber, derivedType, DataFormat.Default);
		}

		public MetaType AddSubType(int fieldNumber, Type derivedType, DataFormat dataFormat)
		{
			if (derivedType == null)
			{
				throw new ArgumentNullException("derivedType");
			}
			if (fieldNumber >= 1)
			{
				bool flag = (uint)fieldNumber + (uint)fieldNumber < 0U;
				if (flag)
				{
					goto IL_11B;
				}
				if (!this.x43163d22e8cd5a71.IsClass)
				{
					goto IL_F0;
				}
				IL_7E:
				while (!this.x43163d22e8cd5a71.IsSealed)
				{
					if (!this.xbcd125d8e5fafe58(derivedType))
					{
						throw new ArgumentException(derivedType.Name + " is not a valid sub-type of " + this.x43163d22e8cd5a71.Name, "derivedType");
					}
					MetaType metaType = this.xad70a5849826ecef[derivedType];
					this.ThrowIfFrozen();
					metaType.ThrowIfFrozen();
					SubType xbcea506a33cf = new SubType(fieldNumber, metaType, dataFormat);
					this.ThrowIfFrozen();
					metaType.xcc9c70c8e136481f(this);
					if (this.x54cc87219996ce22 == null)
					{
						if ((uint)fieldNumber + (uint)fieldNumber >= 0U)
						{
							this.x54cc87219996ce22 = new x826e0336b5da6af5();
						}
						else
						{
							if (!false)
							{
								continue;
							}
							goto IL_F0;
						}
					}
					this.x54cc87219996ce22.xd6b6ed77479ef68c(xbcea506a33cf);
					return this;
				}
				IL_D9:
				throw new InvalidOperationException("Sub-types can only be added to non-sealed classes");
				IL_F0:
				if (this.x43163d22e8cd5a71.IsInterface)
				{
					goto IL_7E;
				}
				if ((uint)fieldNumber + (uint)fieldNumber < 0U)
				{
					return this;
				}
				if (!false)
				{
					goto IL_D9;
				}
				IL_11B:
				goto IL_F0;
			}
			throw new ArgumentOutOfRangeException("fieldNumber");
		}

		private void xcc9c70c8e136481f(MetaType x6227b26cefabe265)
		{
			if (x6227b26cefabe265 == null)
			{
				throw new ArgumentNullException("baseType");
			}
			while (this.x6227b26cefabe265 != x6227b26cefabe265)
			{
				if (8 != 0)
				{
					while (this.x6227b26cefabe265 == null)
					{
						for (MetaType metaType = x6227b26cefabe265; metaType != null; metaType = metaType.x6227b26cefabe265)
						{
							if (object.ReferenceEquals(metaType, this))
							{
								throw new InvalidOperationException("Cyclic inheritance is not allowed");
							}
						}
						if (2 != 0)
						{
							this.x6227b26cefabe265 = x6227b26cefabe265;
							return;
						}
					}
					throw new InvalidOperationException("A type can only participate in one inheritance hierarchy");
				}
			}
		}

		public bool HasCallbacks
		{
			get
			{
				return this.xbc2aaa430ff675df != null && this.xbc2aaa430ff675df.NonTrivial;
			}
		}

		public bool HasSubtypes
		{
			get
			{
				return this.x54cc87219996ce22 != null && this.x54cc87219996ce22.xd44988f225497f3a != 0;
			}
		}

		public CallbackSet Callbacks
		{
			get
			{
				if (this.xbc2aaa430ff675df == null)
				{
					this.xbc2aaa430ff675df = new CallbackSet(this);
				}
				return this.xbc2aaa430ff675df;
			}
		}

		private bool x25f40ad8c018c1ab
		{
			get
			{
				return this.x43163d22e8cd5a71.IsValueType;
			}
		}

		public MetaType SetCallbacks(MethodInfo beforeSerialize, MethodInfo afterSerialize, MethodInfo beforeDeserialize, MethodInfo afterDeserialize)
		{
			CallbackSet callbacks = this.Callbacks;
			callbacks.BeforeSerialize = beforeSerialize;
			callbacks.AfterSerialize = afterSerialize;
			callbacks.BeforeDeserialize = beforeDeserialize;
			callbacks.AfterDeserialize = afterDeserialize;
			return this;
		}

		public MetaType SetCallbacks(string beforeSerialize, string afterSerialize, string beforeDeserialize, string afterDeserialize)
		{
			CallbackSet callbacks;
			if (this.x25f40ad8c018c1ab)
			{
				if (!false && 4 != 0)
				{
					throw new InvalidOperationException();
				}
			}
			else
			{
				callbacks = this.Callbacks;
				callbacks.BeforeSerialize = this.x2eb4012bc0c599e8(beforeSerialize, true);
				callbacks.AfterSerialize = this.x2eb4012bc0c599e8(afterSerialize, true);
			}
			callbacks.BeforeDeserialize = this.x2eb4012bc0c599e8(beforeDeserialize, true);
			callbacks.AfterDeserialize = this.x2eb4012bc0c599e8(afterDeserialize, true);
			return this;
		}

		internal string xe2423e5eb2c7ed45()
		{
			if (this.xd467cf11a28aa761 == null)
			{
				goto IL_281;
			}
			if (false)
			{
				goto IL_1AE;
			}
			goto IL_26A;
			IL_B9:
			StringBuilder stringBuilder;
			Type type;
			stringBuilder.Append(type.Name);
			IL_C6:
			int num;
			num++;
			IL_CC:
			Type[] genericArguments;
			int key;
			if (num < genericArguments.Length)
			{
				Type type2 = genericArguments[num];
				stringBuilder.Append('_');
				type = type2;
				key = this.xad70a5849826ecef.GetKey(ref type);
				goto IL_162;
			}
			IL_D7:
			return stringBuilder.ToString();
			IL_118:
			bool flag = (uint)num < 0U;
			if (flag)
			{
				goto IL_1AE;
			}
			if (((uint)key & 0U) != 0U)
			{
				goto IL_26A;
			}
			goto IL_2AD;
			IL_162:
			int num2;
			if (key < 0)
			{
				flag = ((uint)num2 + (uint)key < 0U);
				if (flag)
				{
					flag = ((uint)key - (uint)num < 0U);
					if (flag)
					{
						goto IL_118;
					}
					if ((uint)num2 - (uint)num2 <= 4294967295U)
					{
						goto IL_2AD;
					}
					goto IL_CC;
				}
				else
				{
					flag = ((uint)key - (uint)num2 > uint.MaxValue);
					if (!flag)
					{
						goto IL_B9;
					}
				}
			}
			MetaType metaType;
			if ((metaType = this.xad70a5849826ecef[type]) != null && metaType.xd467cf11a28aa761 == null)
			{
				stringBuilder.Append(metaType.xe2423e5eb2c7ed45());
				goto IL_C6;
			}
			goto IL_B9;
			IL_180:
			genericArguments = this.x43163d22e8cd5a71.GetGenericArguments();
			if (2 == 0)
			{
				goto IL_281;
			}
			num = 0;
			if (((uint)key & 0U) == 0U)
			{
				goto IL_1E9;
			}
			IL_1AE:
			if ((uint)num2 + (uint)num2 >= 0U)
			{
				flag = ((uint)num2 + (uint)num2 > uint.MaxValue);
				if (flag)
				{
					goto IL_1E9;
				}
				goto IL_180;
			}
			IL_1B0:
			if (num2 < 0)
			{
				goto IL_180;
			}
			goto IL_24B;
			IL_1E9:
			if (!false)
			{
				goto IL_CC;
			}
			goto IL_162;
			IL_24B:
			stringBuilder.Length = num2;
			flag = ((uint)key - (uint)num > uint.MaxValue);
			if (!flag)
			{
				flag = ((uint)num > uint.MaxValue);
				if (flag)
				{
					goto IL_2AD;
				}
				goto IL_1AE;
			}
			IL_26A:
			return this.xad70a5849826ecef[this.xd467cf11a28aa761].xe2423e5eb2c7ed45();
			IL_281:
			if (!x479f2661aae93792.x1c140bd1078ddda1(this.xc15bd84e01929885))
			{
				return this.xc15bd84e01929885;
			}
			while (!this.x43163d22e8cd5a71.IsGenericType)
			{
				if (((uint)num2 | 15U) != 0U)
				{
					if ((uint)num < 0U)
					{
						goto IL_D7;
					}
					flag = ((uint)num + (uint)key > uint.MaxValue);
					if (flag)
					{
						goto IL_B9;
					}
					goto IL_118;
				}
			}
			stringBuilder = new StringBuilder(this.x43163d22e8cd5a71.Name);
			num2 = this.x43163d22e8cd5a71.Name.IndexOf('`');
			flag = ((uint)key - (uint)num2 > uint.MaxValue);
			if (flag)
			{
				goto IL_24B;
			}
			goto IL_1B0;
			IL_2AD:
			return this.x43163d22e8cd5a71.Name;
		}

		public string Name
		{
			get
			{
				return this.xc15bd84e01929885;
			}
			set
			{
				this.ThrowIfFrozen();
				this.xc15bd84e01929885 = value;
			}
		}

		public MetaType SetFactory(MethodInfo factory)
		{
			if (factory != null)
			{
				if (this.x25f40ad8c018c1ab)
				{
					throw new InvalidOperationException();
				}
				if (!factory.IsStatic)
				{
					throw new ArgumentException("A factory-method must be static", "factory");
				}
				if (factory.ReturnType != this.x43163d22e8cd5a71)
				{
					throw new ArgumentException("The factory-method must return " + this.x43163d22e8cd5a71.FullName, "factory");
				}
				if (!CallbackSet.x0134e67247d44030(this.xad70a5849826ecef, factory))
				{
					throw new ArgumentException("Invalid factory signature in " + factory.DeclaringType.FullName + "." + factory.Name, "factory");
				}
			}
			this.ThrowIfFrozen();
			this.x64b16fcabdb0518e = factory;
			return this;
		}

		public MetaType SetFactory(string factory)
		{
			return this.SetFactory(this.x2eb4012bc0c599e8(factory, false));
		}

		private MethodInfo x2eb4012bc0c599e8(string xc15bd84e01929885, bool x6ed4ed9ed59eb694)
		{
			if (x479f2661aae93792.x1c140bd1078ddda1(xc15bd84e01929885))
			{
				return null;
			}
			if (!x6ed4ed9ed59eb694)
			{
				return x479f2661aae93792.x75d730c39ffa52f2(this.x43163d22e8cd5a71, xc15bd84e01929885);
			}
			return x479f2661aae93792.x40c628a821a074f8(this.x43163d22e8cd5a71, xc15bd84e01929885);
		}

		internal MetaType(RuntimeTypeModel model, Type type)
		{
			for (;;)
			{
				if (!false)
				{
					if (!false)
					{
						goto IL_C8;
					}
					goto IL_C8;
				}
				IL_6E:
				x66ec8c25e4c7547d x66ec8c25e4c7547d;
				if (x66ec8c25e4c7547d != null)
				{
					goto Block_5;
				}
				this.x43163d22e8cd5a71 = type;
				for (;;)
				{
					this.xad70a5849826ecef = model;
					if (false)
					{
						goto IL_C6;
					}
					if (false)
					{
						goto Block_4;
					}
					if (x479f2661aae93792.xb636fcab7a16c388(type))
					{
						goto IL_16;
					}
					if (3 != 0)
					{
						break;
					}
					if (!false)
					{
						goto IL_5C;
					}
				}
				IL_96:
				if (false)
				{
					continue;
				}
				return;
				Block_4:
				if (false)
				{
					goto IL_96;
				}
				break;
				IL_C6:
				goto IL_A0;
				IL_5F:
				WireType wireType;
				x66ec8c25e4c7547d = ValueMember.xac123cbb8d7a8e65(model, DataFormat.Default, type, out wireType, false, false, false, false);
				goto IL_6E;
				IL_5C:
				if (type != null)
				{
					goto IL_5F;
				}
				goto IL_A9;
				IL_C8:
				if (model == null)
				{
					goto IL_BB;
				}
				if (false)
				{
					goto IL_5F;
				}
				if (!false)
				{
					goto IL_5C;
				}
				IL_A0:
				if (3 != 0)
				{
					goto IL_5C;
				}
				goto IL_C8;
			}
			IL_16:
			this.EnumPassthru = type.IsDefined(model.MapType(typeof(FlagsAttribute)), false);
			return;
			Block_5:
			throw new ArgumentException("Data of this type has inbuilt behaviour, and cannot be added to a model in this way: " + type.FullName);
			IL_A9:
			throw new ArgumentNullException("type");
			IL_BB:
			throw new ArgumentNullException("model");
		}

		protected internal void ThrowIfFrozen()
		{
			if ((this.xebf45bdcaa1fd1e1 & 4) != 0)
			{
				throw new InvalidOperationException("The type cannot be changed once a serializer has been generated for " + this.x43163d22e8cd5a71.FullName);
			}
		}

		internal void x69322180ae719ea6()
		{
			this.xebf45bdcaa1fd1e1 |= 4;
		}

		public Type Type
		{
			get
			{
				return this.x43163d22e8cd5a71;
			}
		}

		internal x9da713b4847e9e6e x9e41724c73da1842
		{
			get
			{
				if (this.x38c907674383c2da == null)
				{
					int x53a1af2280f1a = 0;
					try
					{
						this.xad70a5849826ecef.x2c0c646a23b0da22(ref x53a1af2280f1a);
						if (this.x38c907674383c2da == null)
						{
							this.x38efbc9b1b17e078(4, true, false);
							this.x38c907674383c2da = this.x0864aaa2c6d0fbe0();
						}
					}
					finally
					{
						this.xad70a5849826ecef.x7109eb31e17d8f9e(x53a1af2280f1a);
					}
				}
				return this.x38c907674383c2da;
			}
		}

		internal bool x2e7a2ea5da15ce85
		{
			get
			{
				Type type = this.IgnoreListHandling ? null : TypeModel.x4431fb61faccece0(this.xad70a5849826ecef, this.x43163d22e8cd5a71);
				return type != null;
			}
		}

		private x9da713b4847e9e6e x0864aaa2c6d0fbe0()
		{
			int num;
			bool flag;
			Type type;
			if (x479f2661aae93792.xb636fcab7a16c388(this.x43163d22e8cd5a71))
			{
				flag = (((uint)num | 2U) == 0U);
				if (flag)
				{
					goto IL_5CB;
				}
				return new x8f2d2e2582fd9f3b(1, WireType.Variant, false, new x9caf06b8b9c3a409(this.x43163d22e8cd5a71, this.x0b51e8612d340c57()));
			}
			else
			{
				if (this.IgnoreListHandling)
				{
					type = null;
					goto IL_5F4;
				}
				flag = ((uint)num > uint.MaxValue);
				if (flag)
				{
					goto IL_672;
				}
				goto IL_5CB;
			}
			IL_89:
			MetaType baseType;
			MethodInfo[] array;
			int num2;
			if (baseType == null)
			{
				array = null;
				x826e0336b5da6af5 x826e0336b5da6af;
				if (x826e0336b5da6af == null)
				{
					goto IL_672;
				}
				array = new MethodInfo[x826e0336b5da6af.xd44988f225497f3a];
				x826e0336b5da6af.x0fe4f26e70030075(array, 0);
				Array.Reverse(array);
				goto IL_672;
			}
			else
			{
				MethodInfo methodInfo = (!baseType.HasCallbacks) ? null : baseType.Callbacks.BeforeDeserialize;
				flag = ((uint)num2 > uint.MaxValue);
				if (flag)
				{
					goto IL_46A;
				}
				goto IL_62F;
			}
			IL_2D4:
			baseType = this.BaseType;
			goto IL_89;
			IL_2EC:
			int num3;
			num2 = num3;
			int xd44988f225497f3a;
			int[] array2 = new int[xd44988f225497f3a + num2];
			flag = (((uint)num2 & 0U) == 0U);
			if (!flag)
			{
				goto IL_4FF;
			}
			if ((uint)num2 + (uint)xd44988f225497f3a > 4294967295U)
			{
				goto IL_44B;
			}
			IL_32C:
			x66ec8c25e4c7547d[] array3 = new x66ec8c25e4c7547d[xd44988f225497f3a + num2];
			if ((uint)num + (uint)xd44988f225497f3a < 0U)
			{
				goto IL_388;
			}
			if (3 != 0)
			{
				num = 0;
				if (num2 != 0)
				{
					IEnumerator enumerator = this.x54cc87219996ce22.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							SubType subType = (SubType)obj;
							if (!subType.DerivedType.IgnoreListHandling)
							{
								if (this.xad70a5849826ecef.MapType(MetaType.xe6867eee5ccb3249).IsAssignableFrom(subType.DerivedType.Type))
								{
									throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot be used as a subclass");
								}
								if (((uint)num & 0U) != 0U)
								{
									break;
								}
							}
							array2[num] = subType.FieldNumber;
							array3[num++] = subType.x9e41724c73da1842;
						}
					}
					finally
					{
						IDisposable disposable = enumerator as IDisposable;
						if (((uint)num2 & 0U) == 0U)
						{
							flag = ((uint)num2 > uint.MaxValue);
							if (flag)
							{
								goto IL_24B;
							}
							goto IL_26C;
						}
						IL_21F:
						if ((uint)num - (uint)num2 <= 4294967295U)
						{
							goto IL_272;
						}
						goto IL_26C;
						IL_24B:
						disposable.Dispose();
						if ((uint)num2 - (uint)xd44988f225497f3a >= 0U)
						{
							goto IL_21F;
						}
						IL_26C:
						if (disposable != null)
						{
							goto IL_24B;
						}
						IL_272:;
					}
				}
				if (xd44988f225497f3a != 0)
				{
					foreach (object obj2 in this.xa942970cc8a85fd4)
					{
						ValueMember valueMember = (ValueMember)obj2;
						array2[num] = valueMember.FieldNumber;
						array3[num++] = valueMember.x9e41724c73da1842;
					}
				}
				x826e0336b5da6af5 x826e0336b5da6af = null;
				goto IL_2D4;
			}
			IL_357:
			if (this.x54cc87219996ce22 == null)
			{
				num3 = 0;
				goto IL_2EC;
			}
			IL_388:
			num3 = this.x54cc87219996ce22.xd44988f225497f3a;
			goto IL_2EC;
			IL_44B:
			ConstructorInfo constructorInfo;
			if (constructorInfo == null)
			{
				throw new InvalidOperationException();
			}
			if (((uint)num2 & 0U) != 0U)
			{
				goto IL_357;
			}
			MemberInfo[] members;
			return new x046924351e370a53(this.xad70a5849826ecef, constructorInfo, members);
			IL_46A:
			MetaType metaType;
			return new xd66bb433130e02ef(this.x43163d22e8cd5a71, this.xd467cf11a28aa761, metaType.x9e41724c73da1842);
			IL_482:
			if (this.xd467cf11a28aa761 != null)
			{
				do
				{
					metaType = this.xad70a5849826ecef[this.xd467cf11a28aa761];
					if (false)
					{
						goto IL_3A0;
					}
					MetaType metaType2;
					while ((metaType2 = metaType.x6227b26cefabe265) != null)
					{
						metaType = metaType2;
						if (!true)
						{
							goto IL_5A5;
						}
					}
					flag = (((uint)xd44988f225497f3a & 0U) == 0U);
					if (!flag)
					{
						goto IL_5B6;
					}
					flag = ((uint)num - (uint)num < 0U);
				}
				while (flag);
				flag = ((uint)num2 > uint.MaxValue);
				if (flag)
				{
					goto IL_62F;
				}
				goto IL_46A;
				IL_5B6:
				goto IL_4FF;
			}
			if (this.x455b3702109fb5db)
			{
				constructorInfo = MetaType.x3fb5a247ee976f3f(this.x43163d22e8cd5a71, out members);
				goto IL_44B;
			}
			IL_3A0:
			this.xa942970cc8a85fd4.x5749d92c9865c1ba();
			xd44988f225497f3a = this.xa942970cc8a85fd4.xd44988f225497f3a;
			goto IL_357;
			IL_4FF:
			if (false)
			{
				flag = ((uint)xd44988f225497f3a + (uint)num < 0U);
				if (flag)
				{
					goto IL_570;
				}
				goto IL_5A5;
			}
			IL_505:
			Type type2;
			ValueMember valueMember2 = new ValueMember(this.xad70a5849826ecef, 1, this.x43163d22e8cd5a71, type2, this.x43163d22e8cd5a71, DataFormat.Default);
			return new xe558091e7a042d4f(this.xad70a5849826ecef, this.x43163d22e8cd5a71, new int[]
			{
				1
			}, new x66ec8c25e4c7547d[]
			{
				valueMember2.x9e41724c73da1842
			}, null, true, true, null, this.x718bc396727ef5e4, this.x64b16fcabdb0518e);
			IL_570:
			if (this.x54cc87219996ce22 == null)
			{
				goto IL_4FF;
			}
			IL_5A5:
			if (this.x54cc87219996ce22.xd44988f225497f3a == 0)
			{
				goto IL_505;
			}
			throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot be subclassed");
			IL_5CB:
			if ((uint)num < 0U)
			{
				goto IL_2D4;
			}
			type = TypeModel.x4431fb61faccece0(this.xad70a5849826ecef, this.x43163d22e8cd5a71);
			IL_5F4:
			type2 = type;
			if (type2 == null)
			{
				goto IL_482;
			}
			if (this.xd467cf11a28aa761 != null)
			{
				throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot use a surrogate");
			}
			goto IL_570;
			IL_62F:
			for (;;)
			{
				MethodInfo methodInfo;
				if (methodInfo == null)
				{
					flag = ((uint)num + (uint)xd44988f225497f3a < 0U);
					if (!flag)
					{
						if ((uint)num2 <= 4294967295U)
						{
							break;
						}
					}
				}
				else
				{
					x826e0336b5da6af5 x826e0336b5da6af;
					if (x826e0336b5da6af == null)
					{
						x826e0336b5da6af = new x826e0336b5da6af5();
						if ((uint)num + (uint)num > 4294967295U)
						{
							goto IL_482;
						}
					}
					x826e0336b5da6af.xd6b6ed77479ef68c(methodInfo);
					flag = (((uint)xd44988f225497f3a | 255U) == 0U);
					if (flag)
					{
						continue;
					}
				}
				if ((uint)num2 - (uint)num <= 4294967295U)
				{
					goto Block_6;
				}
			}
			if (false)
			{
				goto IL_32C;
			}
			IL_80:
			baseType = baseType.BaseType;
			goto IL_89;
			Block_6:
			goto IL_80;
			IL_672:
			return new xe558091e7a042d4f(this.xad70a5849826ecef, this.x43163d22e8cd5a71, array2, array3, array, this.x6227b26cefabe265 == null, this.UseConstructor, this.xbc2aaa430ff675df, this.x718bc396727ef5e4, this.x64b16fcabdb0518e);
		}

		private static Type xb2541fd3863e380b(MetaType x43163d22e8cd5a71)
		{
			return x43163d22e8cd5a71.x43163d22e8cd5a71.BaseType;
		}

		internal void x26ad2611a3d84e83()
		{
			Type type = MetaType.xb2541fd3863e380b(this);
			bool flag;
			int num;
			if ((flag ? 1U : 0U) + (uint)num < 0U)
			{
				goto IL_582;
			}
			if (type != null)
			{
				goto IL_10B2;
			}
			goto IL_1031;
			IL_1E:
			MethodInfo[] array;
			this.SetCallbacks(MetaType.x19119c355688d829(array, 0, 4), MetaType.x19119c355688d829(array, 1, 5), MetaType.x19119c355688d829(array, 2, 6), MetaType.x19119c355688d829(array, 3, 7));
			bool flag3;
			bool flag2 = ((flag3 ? 1U : 0U) | uint.MaxValue) == 0U;
			if (!flag2)
			{
				return;
			}
			IL_64:
			if (array == null)
			{
				goto IL_126;
			}
			goto IL_1E;
			IL_CE:
			ProtoMemberAttribute[] array3;
			ProtoMemberAttribute[] array2 = array3;
			int i;
			bool flag4;
			int num2;
			for (i = 0; i < array2.Length; i++)
			{
				ProtoMemberAttribute x6c48d6ce93880c = array2[i];
				ValueMember valueMember = this.x26ad2611a3d84e83(flag, x6c48d6ce93880c);
				if (valueMember != null)
				{
					this.xd6b6ed77479ef68c(valueMember);
					if ((flag4 ? 1U : 0U) + (uint)num2 < 0U)
					{
						goto IL_8F4;
					}
				}
			}
			int num3;
			flag2 = (((uint)num3 | 8U) == 0U);
			bool flag5;
			if (!flag2)
			{
				flag2 = ((flag5 ? 1U : 0U) < 0U);
				if (flag2)
				{
					goto IL_1E;
				}
				goto IL_64;
			}
			IL_126:
			flag2 = ((uint)num2 - (flag5 ? 1U : 0U) > uint.MaxValue);
			bool flag6;
			if (!flag2)
			{
				if (((uint)num2 & 0U) != 0U)
				{
					goto IL_577;
				}
				flag2 = ((flag6 ? 1U : 0U) > uint.MaxValue);
				if (flag2)
				{
					goto IL_5B7;
				}
				return;
			}
			IL_19D:
			int num4;
			num4++;
			IL_1A3:
			MemberInfo[] array4;
			x826e0336b5da6af5 x826e0336b5da6af;
			int num5;
			int num6;
			ImplicitFields implicitFields;
			ProtoMemberAttribute[] array5;
			int j;
			if (num4 >= array4.Length)
			{
				array3 = new ProtoMemberAttribute[x826e0336b5da6af.xd44988f225497f3a];
				x826e0336b5da6af.x0fe4f26e70030075(array3, 0);
				flag2 = ((uint)num4 < 0U);
				if (flag2)
				{
					flag2 = ((uint)num5 - (flag ? 1U : 0U) > uint.MaxValue);
					if (flag2)
					{
						goto IL_462;
					}
					flag2 = ((flag6 ? 1U : 0U) - (uint)num3 > uint.MaxValue);
					if (flag2)
					{
						if ((uint)num4 - (uint)num6 >= 0U)
						{
							goto IL_177;
						}
						goto IL_3C6;
					}
				}
				else if (flag3)
				{
					goto IL_14B;
				}
				if (implicitFields == ImplicitFields.None)
				{
					goto IL_CE;
				}
				IL_14B:
				Array.Sort<ProtoMemberAttribute>(array3);
				num = num3;
				array5 = array3;
				j = 0;
				goto IL_55E;
				IL_177:
				goto IL_14B;
			}
			goto IL_676;
			IL_1DD:
			if ((uint)num6 <= 4294967295U)
			{
				goto IL_19D;
			}
			goto IL_2B0;
			IL_211:
			if ((uint)j >= 0U)
			{
				goto IL_19D;
			}
			goto IL_349;
			IL_2B0:
			if ((uint)num3 < 0U)
			{
				goto IL_39F;
			}
			IL_2F3:
			flag2 = ((flag4 ? 1U : 0U) - (uint)num > uint.MaxValue);
			if (!flag2)
			{
				flag2 = (((uint)num | 1U) == 0U);
				if (flag2)
				{
					goto IL_211;
				}
				goto IL_19D;
			}
			IL_30E:
			goto IL_1DD;
			IL_315:
			MemberInfo memberInfo;
			MethodInfo methodInfo;
			if ((methodInfo = (memberInfo as MethodInfo)) != null)
			{
				goto IL_336;
			}
			IL_321:
			flag2 = ((uint)num2 < 0U);
			if (!flag2)
			{
				goto IL_19D;
			}
			IL_336:
			x9829c225286ead14[] array6;
			if (flag)
			{
				flag2 = (((uint)num3 & 0U) == 0U);
				if (!flag2)
				{
					goto IL_57D;
				}
				flag2 = ((flag3 ? 1U : 0U) < 0U);
				if (flag2)
				{
					goto IL_2F3;
				}
				flag2 = ((uint)num3 + (uint)num > uint.MaxValue);
				if (!flag2)
				{
					goto IL_19D;
				}
			}
			else
			{
				array6 = x9829c225286ead14.xebcf83b00134300b(this.xad70a5849826ecef, methodInfo, false);
				if (array6 == null)
				{
					goto IL_19D;
				}
			}
			if (array6.Length <= 0)
			{
				goto IL_19D;
			}
			MetaType.x707f974185362554(methodInfo, array6, "ProtoBuf.ProtoBeforeSerializationAttribute", ref array, 0);
			MetaType.x707f974185362554(methodInfo, array6, "ProtoBuf.ProtoAfterSerializationAttribute", ref array, 1);
			flag2 = ((uint)num + (uint)j < 0U);
			if (flag2)
			{
				goto IL_52A;
			}
			if (((flag3 ? 1U : 0U) & 0U) != 0U)
			{
				goto IL_55E;
			}
			goto IL_462;
			IL_344:
			goto IL_211;
			IL_349:
			FieldInfo fieldInfo;
			if (fieldInfo.IsStatic)
			{
				goto IL_582;
			}
			IL_355:
			if ((flag4 ? 1U : 0U) - (flag4 ? 1U : 0U) > 4294967295U)
			{
				goto IL_315;
			}
			flag2 = ((uint)num + (flag4 ? 1U : 0U) < 0U);
			if (flag2)
			{
				goto IL_30E;
			}
			if ((uint)num3 - (flag3 ? 1U : 0U) > 4294967295U)
			{
				goto IL_1DD;
			}
			IL_39A:
			goto IL_19D;
			IL_39F:
			x826e0336b5da6af5 x826e0336b5da6af2;
			if (!x826e0336b5da6af2.x263d579af1d0d43f(memberInfo.Name))
			{
				goto IL_62B;
			}
			if (((uint)j | 2147483648U) != 0U)
			{
				goto IL_39A;
			}
			goto IL_F03;
			IL_3B7:
			if (memberInfo.DeclaringType != this.x43163d22e8cd5a71)
			{
				goto IL_19D;
			}
			IL_3C6:
			if (!memberInfo.IsDefined(this.xad70a5849826ecef.MapType(typeof(ProtoIgnoreAttribute)), true))
			{
				if (x826e0336b5da6af2 != null)
				{
					goto IL_39F;
				}
				goto IL_62B;
			}
			IL_3E8:
			flag2 = ((uint)j > uint.MaxValue);
			if (flag2)
			{
				goto IL_458;
			}
			goto IL_2B0;
			IL_41E:
			MetaType.x707f974185362554(methodInfo, array6, "System.Runtime.Serialization.OnDeserializingAttribute", ref array, 6);
			MetaType.x707f974185362554(methodInfo, array6, "System.Runtime.Serialization.OnDeserializedAttribute", ref array, 7);
			flag2 = ((uint)num5 + (flag3 ? 1U : 0U) > uint.MaxValue);
			if (!flag2)
			{
				if ((flag5 ? 1U : 0U) + (uint)num4 <= 4294967295U)
				{
					goto IL_1DD;
				}
				goto IL_3B7;
			}
			IL_458:
			goto IL_39F;
			IL_462:
			if ((uint)num6 >= 0U)
			{
				MetaType.x707f974185362554(methodInfo, array6, "ProtoBuf.ProtoBeforeDeserializationAttribute", ref array, 2);
				MetaType.x707f974185362554(methodInfo, array6, "ProtoBuf.ProtoAfterDeserializationAttribute", ref array, 3);
				MetaType.x707f974185362554(methodInfo, array6, "System.Runtime.Serialization.OnSerializingAttribute", ref array, 4);
				MetaType.x707f974185362554(methodInfo, array6, "System.Runtime.Serialization.OnSerializedAttribute", ref array, 5);
				goto IL_41E;
			}
			goto IL_10B2;
			IL_52A:
			goto IL_315;
			IL_531:
			goto IL_19D;
			IL_55E:
			while (j < array5.Length)
			{
				ProtoMemberAttribute protoMemberAttribute = array5[j];
				if (!protoMemberAttribute.TagIsPinned)
				{
					protoMemberAttribute.Rebase(num++);
				}
				j++;
			}
			goto IL_CE;
			IL_577:
			flag6 = true;
			if (!flag)
			{
				goto IL_582;
			}
			IL_57D:
			goto IL_349;
			IL_582:
			MetaType.x41998983fed2df58 x41998983fed2df;
			x826e0336b5da6af5 x826e0336b5da6af3;
			Type type2;
			MetaType.x1cfc463ab345a733(this.xad70a5849826ecef, x41998983fed2df, flag, x826e0336b5da6af3, num5, flag3, implicitFields, x826e0336b5da6af, memberInfo, ref flag4, flag5, flag6, ref type2);
			goto IL_531;
			IL_5B7:
			flag5 = fieldInfo.IsPublic;
			goto IL_577;
			IL_62B:
			flag4 = false;
			IL_62E:
			PropertyInfo propertyInfo;
			if ((propertyInfo = (memberInfo as PropertyInfo)) == null)
			{
				if ((fieldInfo = (memberInfo as FieldInfo)) == null)
				{
					goto IL_52A;
				}
				type2 = fieldInfo.FieldType;
				if (((uint)num3 & 0U) == 0U)
				{
					goto IL_5B7;
				}
				flag2 = ((flag3 ? 1U : 0U) + (uint)num3 < 0U);
				if (flag2)
				{
					goto IL_62B;
				}
				goto IL_875;
			}
			IL_63D:
			flag2 = ((flag6 ? 1U : 0U) + (uint)num2 > uint.MaxValue);
			if (!flag2 && flag)
			{
				goto IL_344;
			}
			IL_658:
			type2 = propertyInfo.PropertyType;
			flag5 = (x479f2661aae93792.xbfdf1fd45a6330fd(propertyInfo, false) != null);
			flag6 = false;
			MetaType.x1cfc463ab345a733(this.xad70a5849826ecef, x41998983fed2df, flag, x826e0336b5da6af3, num5, flag3, implicitFields, x826e0336b5da6af, memberInfo, ref flag4, flag5, flag6, ref type2);
			goto IL_19D;
			IL_676:
			memberInfo = array4[num4];
			if ((flag3 ? 1U : 0U) + (uint)num2 >= 0U)
			{
				goto IL_3B7;
			}
			goto IL_41E;
			IL_6C9:
			array = null;
			x826e0336b5da6af = new x826e0336b5da6af5();
			MemberInfo[] members = this.x43163d22e8cd5a71.GetMembers(flag ? (BindingFlags.Static | BindingFlags.Public) : (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			flag2 = ((uint)num6 > uint.MaxValue);
			if (flag2)
			{
				goto IL_676;
			}
			flag2 = (((uint)num4 & 0U) == 0U);
			if (flag2)
			{
				array4 = members;
				num4 = 0;
				goto IL_1A3;
			}
			goto IL_E06;
			IL_700:
			x41998983fed2df &= MetaType.x41998983fed2df58.x430652ca16c43c9b;
			goto IL_6C9;
			IL_706:
			if (implicitFields != ImplicitFields.None)
			{
				goto IL_700;
			}
			flag2 = ((uint)i - (uint)num < 0U);
			if (flag2)
			{
				if ((flag ? 1U : 0U) > 4294967295U)
				{
					goto IL_64;
				}
			}
			else
			{
				flag2 = ((uint)num2 > uint.MaxValue);
				if (flag2)
				{
					goto IL_62B;
				}
				goto IL_6C9;
			}
			IL_737:
			string text;
			if (!x479f2661aae93792.x1c140bd1078ddda1(text))
			{
				this.Name = text;
				goto IL_7C0;
			}
			if ((uint)num6 - (uint)i > 4294967295U)
			{
				goto IL_321;
			}
			flag2 = ((uint)num6 - (flag3 ? 1U : 0U) < 0U);
			if (!flag2)
			{
				goto IL_706;
			}
			flag2 = (((uint)num & 0U) == 0U);
			if (flag2)
			{
				goto IL_700;
			}
			goto IL_62E;
			IL_7A9:
			x9829c225286ead14[] array7;
			if (num6 >= array7.Length)
			{
				goto IL_737;
			}
			goto IL_F11;
			IL_7C0:
			goto IL_706;
			IL_7C5:
			if ((flag4 ? 1U : 0U) - (flag5 ? 1U : 0U) < 0U)
			{
				goto IL_837;
			}
			if ((uint)num4 - (uint)num < 0U)
			{
				goto IL_344;
			}
			IL_815:
			num6++;
			goto IL_7A9;
			IL_81D:
			if (((flag ? 1U : 0U) | 2147483647U) != 0U)
			{
				goto IL_815;
			}
			goto IL_856;
			IL_837:
			if (text != null)
			{
				goto IL_815;
			}
			x9829c225286ead14 x9829c225286ead;
			object obj;
			if (!x9829c225286ead.x034113016eff150a("TypeName", out obj))
			{
				if ((uint)num6 + (uint)i >= 0U)
				{
					goto IL_815;
				}
				goto IL_81D;
			}
			else
			{
				text = (string)obj;
				flag2 = ((flag5 ? 1U : 0U) + (uint)i < 0U);
				if (flag2)
				{
					goto IL_93D;
				}
				goto IL_875;
			}
			IL_856:
			if (x9829c225286ead.AttributeType.FullName == "System.Xml.Serialization.XmlTypeAttribute")
			{
				goto IL_837;
			}
			goto IL_7C5;
			IL_875:
			flag2 = ((uint)num + (flag4 ? 1U : 0U) > uint.MaxValue);
			if (!flag2)
			{
				goto IL_81D;
			}
			if ((uint)num3 - (uint)j < 0U)
			{
				goto IL_5B7;
			}
			IL_8AB:
			flag2 = ((uint)num2 > uint.MaxValue);
			if (flag2)
			{
				goto IL_7C5;
			}
			IL_8F4:
			goto IL_856;
			IL_8F9:
			if (!(x9829c225286ead.AttributeType.FullName == "System.Runtime.Serialization.DataContractAttribute"))
			{
				goto IL_856;
			}
			if (text != null)
			{
				goto IL_8F4;
			}
			if (x9829c225286ead.x034113016eff150a("Name", out obj))
			{
				text = (string)obj;
			}
			goto IL_856;
			IL_93D:
			if (flag)
			{
				goto IL_8F9;
			}
			if (!x9829c225286ead.x034113016eff150a("DataMemberOffset", out obj))
			{
				goto IL_BC3;
			}
			flag2 = ((flag ? 1U : 0U) - (uint)i < 0U);
			if (flag2)
			{
				goto IL_C30;
			}
			num5 = (int)obj;
			if ((flag4 ? 1U : 0U) <= 4294967295U)
			{
				goto IL_BC3;
			}
			goto IL_D32;
			IL_B4A:
			if (x9829c225286ead.x034113016eff150a("ImplicitFields", out obj))
			{
				if (((uint)num6 & 0U) == 0U && !(obj is ImplicitFields))
				{
					if (!(obj is int))
					{
						throw new NotSupportedException(obj.GetType().FullName);
					}
					flag2 = (((flag ? 1U : 0U) & 0U) == 0U);
					if (!flag2)
					{
						goto IL_8AB;
					}
					implicitFields = (ImplicitFields)((int)obj);
				}
				else
				{
					implicitFields = (ImplicitFields)obj;
				}
			}
			if (!x9829c225286ead.x034113016eff150a("SkipConstructor", out obj))
			{
				flag2 = (((uint)num6 | 4U) == 0U);
				if (flag2)
				{
					goto IL_AC0;
				}
			}
			else
			{
				this.UseConstructor = !(bool)obj;
			}
			for (;;)
			{
				IL_A54:
				if (x9829c225286ead.x034113016eff150a("IgnoreListHandling", out obj))
				{
					this.IgnoreListHandling = (bool)obj;
				}
				while (x9829c225286ead.x034113016eff150a("ImplicitFirstTag", out obj))
				{
					if ((flag6 ? 1U : 0U) - (uint)num5 < 0U)
					{
						goto IL_6C9;
					}
					if ((int)obj <= 0)
					{
						break;
					}
					num3 = (int)obj;
					if ((flag4 ? 1U : 0U) + (flag4 ? 1U : 0U) < 0U)
					{
						return;
					}
					flag2 = ((uint)num < 0U);
					if (flag2)
					{
						goto IL_A54;
					}
					flag2 = ((uint)num5 + (uint)num3 < 0U);
					if (flag2)
					{
						break;
					}
					if ((uint)num + (uint)num4 < 0U)
					{
						goto IL_C9C;
					}
					flag2 = ((flag6 ? 1U : 0U) - (uint)num3 < 0U);
					if (!flag2)
					{
						break;
					}
					flag2 = (((uint)j | 2147483648U) == 0U);
					if (!flag2)
					{
						goto IL_A54;
					}
				}
				break;
			}
			IL_AC0:
			goto IL_8F9;
			IL_BC3:
			if (!x9829c225286ead.TryGet("InferTagFromNameHasValue", false, out obj) || !(bool)obj)
			{
				goto IL_B4A;
			}
			if ((uint)num5 - (uint)num2 > 4294967295U)
			{
				goto IL_658;
			}
			IL_C30:
			if (!x9829c225286ead.x034113016eff150a("InferTagFromName", out obj))
			{
				goto IL_B4A;
			}
			flag3 = (bool)obj;
			flag2 = (((flag3 ? 1U : 0U) & 0U) == 0U);
			if (flag2)
			{
				goto IL_B4A;
			}
			goto IL_D39;
			IL_96D:
			if (!(x9829c225286ead.AttributeType.FullName == "ProtoBuf.ProtoContractAttribute"))
			{
				if (!false)
				{
					goto IL_8F9;
				}
				if (((flag5 ? 1U : 0U) | 8U) != 0U)
				{
					goto IL_93D;
				}
				goto IL_355;
			}
			else
			{
				if (!x9829c225286ead.x034113016eff150a("Name", out obj))
				{
					goto IL_93D;
				}
				text = (string)obj;
				goto IL_93D;
			}
			IL_C5B:
			IL_C61:
			x826e0336b5da6af3.xd6b6ed77479ef68c(x9829c225286ead);
			goto IL_96D;
			IL_C9C:
			if (flag || !(x9829c225286ead.AttributeType.FullName == "ProtoBuf.ProtoPartialMemberAttribute"))
			{
				goto IL_96D;
			}
			if (x826e0336b5da6af3 != null)
			{
				goto IL_C61;
			}
			x826e0336b5da6af3 = new x826e0336b5da6af5();
			goto IL_C5B;
			IL_CD0:
			if (!(x9829c225286ead.AttributeType.FullName == "ProtoBuf.ProtoPartialIgnoreAttribute"))
			{
				goto IL_C9C;
			}
			if (!x9829c225286ead.x034113016eff150a("MemberName", out obj))
			{
				flag2 = (((uint)num3 & 0U) == 0U);
				if (flag2)
				{
					goto IL_C9C;
				}
				goto IL_63D;
			}
			else
			{
				if (obj == null)
				{
					goto IL_C9C;
				}
				flag2 = ((uint)j - (uint)num4 > uint.MaxValue);
				if (flag2)
				{
					if ((uint)num + (uint)num >= 0U)
					{
						goto IL_D32;
					}
					goto IL_211;
				}
				else
				{
					if (x826e0336b5da6af2 == null)
					{
						goto IL_D32;
					}
					flag2 = ((flag ? 1U : 0U) + (uint)num5 > uint.MaxValue);
					if (flag2)
					{
						goto IL_D57;
					}
				}
			}
			IL_CEA:
			x826e0336b5da6af2.xd6b6ed77479ef68c((string)obj);
			goto IL_D57;
			IL_D32:
			x826e0336b5da6af2 = new x826e0336b5da6af5();
			IL_D39:
			goto IL_CEA;
			IL_D57:
			flag2 = ((flag4 ? 1U : 0U) - (uint)num5 > uint.MaxValue);
			if (!flag2)
			{
				flag2 = ((flag4 ? 1U : 0U) > uint.MaxValue);
				if (flag2)
				{
					goto IL_F11;
				}
				goto IL_1100;
			}
			IL_D6F:
			if (!(x9829c225286ead.AttributeType.FullName == "ProtoBuf.ProtoIncludeAttribute"))
			{
				goto IL_CD0;
			}
			num2 = 0;
			flag2 = ((flag5 ? 1U : 0U) < 0U);
			if (flag2)
			{
				goto IL_F03;
			}
			goto IL_EA5;
			IL_DF6:
			DataFormat dataFormat = DataFormat.Default;
			IL_E06:
			if (x9829c225286ead.x034113016eff150a("DataFormat", out obj))
			{
				dataFormat = (DataFormat)((int)obj);
			}
			IL_E16:
			Type type3 = null;
			try
			{
				if (x9829c225286ead.x034113016eff150a("knownTypeName", out obj))
				{
					type3 = this.xad70a5849826ecef.xd6886399673b411b((string)obj, this.x43163d22e8cd5a71.Assembly);
				}
				else if (x9829c225286ead.x034113016eff150a("knownType", out obj))
				{
					type3 = (Type)obj;
				}
				goto IL_DB7;
			}
			catch (Exception innerException)
			{
				throw new InvalidOperationException("Unable to resolve sub-type of: " + this.x43163d22e8cd5a71.FullName, innerException);
			}
			goto IL_EA5;
			IL_DB7:
			if (type3 == null)
			{
				if ((flag3 ? 1U : 0U) - (uint)num6 >= 0U)
				{
					throw new InvalidOperationException("Unable to resolve sub-type of: " + this.x43163d22e8cd5a71.FullName);
				}
				goto IL_C5B;
			}
			else
			{
				if (!this.xbcd125d8e5fafe58(type3))
				{
					goto IL_CD0;
				}
				this.AddSubType(num2, type3, dataFormat);
				goto IL_CD0;
			}
			IL_EA5:
			if (!x9829c225286ead.x034113016eff150a("tag", out obj))
			{
				goto IL_DF6;
			}
			IL_F03:
			num2 = (int)obj;
			goto IL_DF6;
			IL_F11:
			x9829c225286ead = array7[num6];
			if (!flag)
			{
				goto IL_D6F;
			}
			goto IL_CD0;
			IL_1022:
			if (MetaType.x20a65713c9d99978(this.xad70a5849826ecef, type, null) != MetaType.x41998983fed2df58.x4d0b9d4447ba7566)
			{
				this.xad70a5849826ecef.x8a1227c17a49f070(type, true, false, false);
				if (((flag4 ? 1U : 0U) & 0U) != 0U)
				{
					if ((flag3 ? 1U : 0U) >= 0U)
					{
						goto IL_10B2;
					}
					goto IL_39A;
				}
			}
			IL_1031:
			array7 = x9829c225286ead14.xebcf83b00134300b(this.xad70a5849826ecef, this.x43163d22e8cd5a71, false);
			x41998983fed2df = MetaType.x20a65713c9d99978(this.xad70a5849826ecef, this.x43163d22e8cd5a71, array7);
			if (x41998983fed2df != MetaType.x41998983fed2df58.xc0b8234e2c894eae)
			{
				if ((uint)num5 - (flag ? 1U : 0U) > 4294967295U)
				{
					goto IL_3E8;
				}
				flag2 = ((uint)j > uint.MaxValue);
				if (flag2)
				{
					goto IL_1022;
				}
			}
			else
			{
				this.x38efbc9b1b17e078(64, true, true);
			}
			flag = (!this.EnumPassthru && x479f2661aae93792.xb636fcab7a16c388(this.x43163d22e8cd5a71));
			if (x41998983fed2df == MetaType.x41998983fed2df58.x4d0b9d4447ba7566)
			{
				flag2 = ((flag6 ? 1U : 0U) + (uint)num5 > uint.MaxValue);
				if (flag2 || !flag)
				{
					return;
				}
				flag2 = ((uint)num3 > uint.MaxValue);
				if (flag2)
				{
					goto IL_1100;
				}
				flag2 = (((flag5 ? 1U : 0U) & 0U) == 0U);
				if (!flag2)
				{
					goto IL_E16;
				}
			}
			x826e0336b5da6af2 = null;
			if (((uint)i | 255U) == 0U)
			{
				goto IL_7C0;
			}
			x826e0336b5da6af3 = null;
			num5 = 0;
			num3 = 1;
			flag3 = this.xad70a5849826ecef.InferTagFromNameDefault;
			implicitFields = ImplicitFields.None;
			text = null;
			num6 = 0;
			flag2 = ((uint)num6 + (flag6 ? 1U : 0U) > uint.MaxValue);
			if (flag2)
			{
				goto IL_F11;
			}
			goto IL_7A9;
			IL_10B2:
			if (this.xad70a5849826ecef.x36fdee630175c0fa(type) != null)
			{
				goto IL_1031;
			}
			goto IL_1022;
			IL_1100:
			goto IL_C9C;
		}

		private static void x1cfc463ab345a733(TypeModel xad70a5849826ecef, MetaType.x41998983fed2df58 xc0752627b0d47dbb, bool x928589c3132a5f4f, x826e0336b5da6af5 xf1f230b9e66a016a, int x66e0c6e69bac0ae4, bool xa3c3c02513d8dce9, ImplicitFields xee8c578dc2424448, x826e0336b5da6af5 x9b0de312ad5acf8b, MemberInfo xf0b74f36659f8180, ref bool x0d773e8ee668a258, bool x336ea658d1bb3379, bool xce1c790ea40d4c8a, ref Type x7bc89d21aec0483a)
		{
			bool flag = (xce1c790ea40d4c8a ? 1U : 0U) - (x928589c3132a5f4f ? 1U : 0U) < 0U;
			if (!flag)
			{
				ProtoMemberAttribute protoMemberAttribute;
				for (;;)
				{
					switch (xee8c578dc2424448)
					{
					case ImplicitFields.AllPublic:
						if (x336ea658d1bb3379)
						{
							x0d773e8ee668a258 = true;
						}
						break;
					case ImplicitFields.AllFields:
						IL_CD:
						if (!xce1c790ea40d4c8a)
						{
							if (((x336ea658d1bb3379 ? 1U : 0U) | 2147483647U) == 0U)
							{
								goto IL_7C;
							}
						}
						else
						{
							x0d773e8ee668a258 = true;
						}
						break;
					}
					if (x7bc89d21aec0483a.IsSubclassOf(xad70a5849826ecef.MapType(typeof(Delegate))))
					{
						flag = ((x928589c3132a5f4f ? 1U : 0U) + (uint)x66e0c6e69bac0ae4 < 0U);
						if (flag)
						{
							continue;
						}
						x7bc89d21aec0483a = null;
					}
					if (x7bc89d21aec0483a == null)
					{
						goto Block_4;
					}
					protoMemberAttribute = MetaType.x4a2e69a7829004c3(xad70a5849826ecef, xf0b74f36659f8180, xc0752627b0d47dbb, x0d773e8ee668a258, x928589c3132a5f4f, xf1f230b9e66a016a, x66e0c6e69bac0ae4, xa3c3c02513d8dce9);
					if ((xce1c790ea40d4c8a ? 1U : 0U) - (x928589c3132a5f4f ? 1U : 0U) < 0U)
					{
						goto IL_CD;
					}
					if (((uint)x66e0c6e69bac0ae4 & 0U) != 0U || protoMemberAttribute != null)
					{
						break;
					}
					if (!false)
					{
						return;
					}
				}
				x9b0de312ad5acf8b.xd6b6ed77479ef68c(protoMemberAttribute);
				IL_7C:
				Block_4:;
			}
		}

		private static MethodInfo x19119c355688d829(MethodInfo[] x33aa8f538b7e1d2a, int x08db3aeabb253cb1, int x1e218ceaee1bb583)
		{
			MethodInfo methodInfo = x33aa8f538b7e1d2a[x08db3aeabb253cb1];
			while (methodInfo == null)
			{
				methodInfo = x33aa8f538b7e1d2a[x1e218ceaee1bb583];
				while (3 != 0)
				{
					if ((uint)x1e218ceaee1bb583 >= 0U)
					{
						return methodInfo;
					}
				}
			}
			return methodInfo;
		}

		internal static MetaType.x41998983fed2df58 x20a65713c9d99978(RuntimeTypeModel xad70a5849826ecef, Type x43163d22e8cd5a71, x9829c225286ead14[] x233f092c536593eb)
		{
			MetaType.x41998983fed2df58 x41998983fed2df = MetaType.x41998983fed2df58.x4d0b9d4447ba7566;
			for (;;)
			{
				if (x233f092c536593eb != null)
				{
					goto IL_24F;
				}
				bool flag2;
				bool flag = (flag2 ? 1U : 0U) - (flag2 ? 1U : 0U) > uint.MaxValue;
				if (flag)
				{
					goto IL_2B2;
				}
				goto IL_289;
				IL_5D:
				int num;
				string fullName;
				if (num >= x233f092c536593eb.Length)
				{
					if (x41998983fed2df != MetaType.x41998983fed2df58.x4d0b9d4447ba7566)
					{
						return x41998983fed2df;
					}
					flag = (((flag2 ? 1U : 0U) & 0U) == 0U);
					if (!flag)
					{
						goto IL_1C1;
					}
					MemberInfo[] array;
					if (MetaType.x3fb5a247ee976f3f(x43163d22e8cd5a71, out array) != null)
					{
						break;
					}
					if (2 == 0)
					{
						flag = ((uint)num < 0U);
						if (!flag)
						{
							goto IL_1F0;
						}
						flag = ((uint)num - (uint)num < 0U);
						if (flag)
						{
							goto IL_FF;
						}
						goto IL_134;
					}
					else
					{
						flag = ((flag2 ? 1U : 0U) + (flag2 ? 1U : 0U) > uint.MaxValue);
						if (!flag)
						{
							return x41998983fed2df;
						}
						if (((uint)num & 0U) == 0U)
						{
							continue;
						}
						if (false)
						{
							goto IL_24F;
						}
						goto IL_289;
					}
				}
				else
				{
					if ((fullName = x233f092c536593eb[num].AttributeType.FullName) == null)
					{
						goto IL_59;
					}
					if (!(fullName == "ProtoBuf.ProtoContractAttribute"))
					{
						goto IL_9F;
					}
					flag2 = false;
					if (((uint)num | 2U) == 0U)
					{
						goto IL_AD;
					}
					MetaType.x5578649838859fff(ref flag2, x233f092c536593eb[num], "UseProtoMembersOnly");
					if (!flag2)
					{
						x41998983fed2df |= MetaType.x41998983fed2df58.x430652ca16c43c9b;
						goto IL_59;
					}
					return MetaType.x41998983fed2df58.x430652ca16c43c9b;
				}
				IL_16A:
				while (fullName == "System.Runtime.Serialization.DataContractAttribute")
				{
					if (xad70a5849826ecef.AutoAddProtoContractTypesOnly)
					{
						break;
					}
					if ((flag2 ? 1U : 0U) >= 0U)
					{
						if (-2147483648 != 0)
						{
							if (false)
							{
								break;
							}
							x41998983fed2df |= MetaType.x41998983fed2df58.x6037fb37e2f942e7;
							if (((flag2 ? 1U : 0U) | 3U) == 0U)
							{
								goto IL_07;
							}
						}
						flag = (((flag2 ? 1U : 0U) | 15U) == 0U);
						if (flag)
						{
							goto IL_134;
						}
						break;
					}
					else if (false)
					{
						goto IL_2B2;
					}
				}
				goto IL_1F0;
				IL_AD:
				if (xad70a5849826ecef.AutoAddProtoContractTypesOnly)
				{
					goto IL_59;
				}
				goto IL_1C1;
				IL_9F:
				if (fullName == "System.Xml.Serialization.XmlTypeAttribute")
				{
					goto IL_AD;
				}
				flag = (((uint)num | uint.MaxValue) == 0U);
				if (!flag)
				{
					goto IL_16A;
				}
				IL_D7:
				flag = ((uint)num - (uint)num > uint.MaxValue);
				if (!flag)
				{
					goto IL_16A;
				}
				if (!false)
				{
					goto IL_9F;
				}
				if (-1 == 0)
				{
					goto IL_FF;
				}
				flag = ((flag2 ? 1U : 0U) + (flag2 ? 1U : 0U) < 0U);
				if (flag)
				{
					goto IL_134;
				}
				break;
				IL_1C1:
				x41998983fed2df |= MetaType.x41998983fed2df58.xd2e84228f4f88898;
				goto IL_59;
				IL_134:
				goto IL_AD;
				IL_FF:
				if ((uint)num + (flag2 ? 1U : 0U) < 0U)
				{
					goto IL_134;
				}
				if (((flag2 ? 1U : 0U) | 2U) != 0U)
				{
					goto IL_9F;
				}
				goto IL_D7;
				IL_59:
				num++;
				goto IL_5D;
				IL_24F:
				num = 0;
				goto IL_5D;
				IL_289:
				x233f092c536593eb = x9829c225286ead14.xebcf83b00134300b(xad70a5849826ecef, x43163d22e8cd5a71, false);
				goto IL_24F;
				IL_1F0:
				goto IL_59;
				IL_2B2:
				flag = ((uint)num + (uint)num < 0U);
				if (flag)
				{
					return x41998983fed2df;
				}
				goto IL_1F0;
			}
			IL_07:
			return x41998983fed2df | MetaType.x41998983fed2df58.xc0b8234e2c894eae;
			goto IL_07;
		}

		internal static ConstructorInfo x3fb5a247ee976f3f(Type x43163d22e8cd5a71, out MemberInfo[] x5ab0d40e1e1457a3)
		{
			x5ab0d40e1e1457a3 = null;
			IL_476:
			while (x43163d22e8cd5a71 != null)
			{
				while (!x43163d22e8cd5a71.IsAbstract)
				{
					bool flag;
					int num5;
					ConstructorInfo result;
					for (;;)
					{
						IL_44C:
						ConstructorInfo[] array = x479f2661aae93792.x690558c2b7705ce8(x43163d22e8cd5a71, false);
						while (array.Length != 0)
						{
							if (array.Length != 1)
							{
								goto IL_3F0;
							}
							IL_424:
							int num;
							int num2;
							if (array[0].GetParameters().Length == 0)
							{
								flag = ((uint)num + (uint)num2 < 0U);
								if (flag)
								{
									continue;
								}
								break;
							}
							IL_3F0:
							MemberInfo[] array2 = x479f2661aae93792.x37d5499f81df29d6(x43163d22e8cd5a71, true);
							int num3;
							bool flag2;
							if ((uint)num3 + (flag2 ? 1U : 0U) < 0U)
							{
								goto IL_183;
							}
							x826e0336b5da6af5 x826e0336b5da6af = new x826e0336b5da6af5();
							if (false)
							{
								goto IL_BF;
							}
							num2 = 0;
							IL_274:
							MemberInfo[] array3;
							int i;
							int j;
							if (num2 >= array2.Length)
							{
								if (false)
								{
									goto IL_BF;
								}
								if (x826e0336b5da6af.xd44988f225497f3a == 0)
								{
									goto Block_16;
								}
								array3 = new MemberInfo[x826e0336b5da6af.xd44988f225497f3a];
								if ((flag2 ? 1U : 0U) + (uint)i >= 0U)
								{
									x826e0336b5da6af.x0fe4f26e70030075(array3, 0);
									goto IL_2C7;
								}
								goto IL_31E;
							}
							else
							{
								PropertyInfo propertyInfo = array2[num2] as PropertyInfo;
								if (propertyInfo == null)
								{
									if ((uint)i + (uint)i > 4294967295U)
									{
										goto IL_424;
									}
									flag = ((uint)j - (flag2 ? 1U : 0U) < 0U);
									if (!flag)
									{
										goto IL_31E;
									}
								}
								else
								{
									if (!propertyInfo.CanRead)
									{
										goto Block_26;
									}
									if (!propertyInfo.CanWrite || x479f2661aae93792.x960b3b86642a1b2d(propertyInfo, false) == null)
									{
										x826e0336b5da6af.xd6b6ed77479ef68c(propertyInfo);
										goto IL_270;
									}
									goto IL_3A8;
								}
							}
							IL_390:
							goto IL_2A9;
							IL_31E:
							if ((uint)num3 - (flag2 ? 1U : 0U) < 0U)
							{
								break;
							}
							if (!false)
							{
								goto IL_390;
							}
							goto IL_62;
							IL_270:
							num2++;
							goto IL_274;
							IL_2A9:
							FieldInfo fieldInfo = array2[num2] as FieldInfo;
							flag = ((uint)j > uint.MaxValue);
							if (flag)
							{
								goto IL_2C7;
							}
							if (fieldInfo != null)
							{
								goto IL_29A;
							}
							goto IL_270;
							IL_1A3:
							flag = ((uint)num + (uint)num2 < 0U);
							if (flag)
							{
								goto IL_2A9;
							}
							int num4;
							flag = ((uint)num4 + (uint)num < 0U);
							if (!flag)
							{
								i = 0;
							}
							if (((flag2 ? 1U : 0U) & 0U) != 0U)
							{
								goto Block_21;
							}
							ParameterInfo[] parameters;
							int[] array4;
							while (i < array3.Length)
							{
								string b;
								if (!(array3[i].Name.ToLower() != b))
								{
									Type type = x479f2661aae93792.xc880d46f290519e2(array3[i]);
									while (type == parameters[num4].ParameterType)
									{
										array4[num4] = i;
										if (-1 != 0)
										{
											break;
										}
									}
								}
								i++;
								if ((uint)num3 < 0U)
								{
									goto IL_26;
								}
							}
							num4++;
							IL_7B:
							if (num4 < parameters.Length)
							{
								string b = parameters[num4].Name.ToLower();
								goto IL_1A3;
							}
							flag2 = false;
							if ((uint)num3 - (uint)num > 4294967295U)
							{
								goto IL_44C;
							}
							if ((uint)num3 - (uint)num2 >= 0U)
							{
								goto IL_BF;
							}
							goto IL_29A;
							IL_3EB:
							goto IL_274;
							IL_2C7:
							flag = ((uint)j - (uint)i < 0U);
							if (!flag)
							{
								array4 = new int[array3.Length];
								if ((uint)num5 > 4294967295U)
								{
									goto IL_2A3;
								}
								if ((uint)num + (uint)num5 < 0U)
								{
									goto IL_46D;
								}
								num5 = 0;
								if (((uint)j | 4294967294U) == 0U)
								{
									goto IL_3EB;
								}
								result = null;
								x5ab0d40e1e1457a3 = new MemberInfo[array4.Length];
							}
							num3 = 0;
							goto IL_12;
							IL_2A3:
							x826e0336b5da6af.xd6b6ed77479ef68c(fieldInfo);
							goto IL_270;
							IL_29A:
							if (fieldInfo.IsInitOnly)
							{
								goto IL_2A3;
							}
							goto IL_298;
							IL_12:
							if (num3 >= array.Length)
							{
								goto Block_1;
							}
							parameters = array[num3].GetParameters();
							if (false)
							{
								goto IL_1A3;
							}
							flag = ((uint)num3 < 0U);
							if (flag)
							{
								goto IL_178;
							}
							goto IL_26;
							IL_0C:
							num3++;
							goto IL_12;
							IL_26:
							if (parameters.Length != array3.Length)
							{
								goto IL_0C;
							}
							goto IL_178;
							IL_62:
							while (j < array4.Length)
							{
								if (array4[j] >= 0)
								{
									x5ab0d40e1e1457a3[j] = array3[array4[j]];
									j++;
								}
								else
								{
									flag2 = true;
									IL_08:
									if (flag2)
									{
										goto IL_0C;
									}
									num5++;
									result = array[num3];
									goto IL_0C;
								}
							}
							goto IL_08;
							IL_BF:
							j = 0;
							goto IL_62;
							IL_140:
							if (num >= array4.Length)
							{
								num4 = 0;
								goto IL_7B;
							}
							array4[num] = -1;
							IL_183:
							num++;
							goto IL_140;
							IL_178:
							num = 0;
							goto IL_140;
						}
						goto IL_44A;
					}
					IL_46D:
					continue;
					Block_21:
					flag = (((uint)num5 | 4U) == 0U);
					if (flag)
					{
						goto IL_476;
					}
					return result;
					Block_1:
					if (num5 != 1)
					{
						return null;
					}
					return result;
					Block_16:
					return null;
					IL_298:
					return null;
					Block_26:
					return null;
					IL_3A8:
					return null;
					IL_44A:
					return null;
				}
				return null;
			}
			throw new ArgumentNullException("type");
		}

		private static void x707f974185362554(MethodInfo x1306445c04667cc7, x9829c225286ead14[] x233f092c536593eb, string x4a67f04d49b2d893, ref MethodInfo[] xbc2aaa430ff675df, int xc0c4c459c6ccbd00)
		{
			for (int i = 0; i < x233f092c536593eb.Length; i++)
			{
				if (x233f092c536593eb[i].AttributeType.FullName == x4a67f04d49b2d893)
				{
					if (xbc2aaa430ff675df != null)
					{
						if (xbc2aaa430ff675df[xc0c4c459c6ccbd00] != null)
						{
							Type reflectedType = x1306445c04667cc7.ReflectedType;
							throw new ProtoException("Duplicate " + x4a67f04d49b2d893 + " callbacks on " + reflectedType.FullName);
						}
					}
					else
					{
						xbc2aaa430ff675df = new MethodInfo[8];
					}
					xbc2aaa430ff675df[xc0c4c459c6ccbd00] = x1306445c04667cc7;
				}
			}
		}

		private static bool x6c1bc3e71a5ef2b5(MetaType.x41998983fed2df58 xbcea506a33cf9111, MetaType.x41998983fed2df58 x3362caa77b1f70b4)
		{
			return (xbcea506a33cf9111 & x3362caa77b1f70b4) == x3362caa77b1f70b4;
		}

		private static ProtoMemberAttribute x4a2e69a7829004c3(TypeModel xad70a5849826ecef, MemberInfo xf0b74f36659f8180, MetaType.x41998983fed2df58 xc0752627b0d47dbb, bool x0d773e8ee668a258, bool x928589c3132a5f4f, x826e0336b5da6af5 xf1f230b9e66a016a, int x66e0c6e69bac0ae4, bool xba755397c46c6a0b)
		{
			if (xf0b74f36659f8180 != null)
			{
				for (;;)
				{
					bool flag;
					bool flag2;
					if (xc0752627b0d47dbb == MetaType.x41998983fed2df58.x4d0b9d4447ba7566)
					{
						if (((flag ? 1U : 0U) | 4U) != 0U)
						{
							goto IL_AFA;
						}
						goto IL_A38;
					}
					else
					{
						if ((flag2 ? 1U : 0U) + (flag ? 1U : 0U) >= 0U)
						{
							goto IL_A6E;
						}
						goto IL_AFA;
					}
					bool flag3;
					x9829c225286ead14[] xb8c35680a4fe970e;
					x9829c225286ead14 x9829c225286ead;
					bool flag4;
					int num;
					for (;;)
					{
						IL_2A2:
						if (!MetaType.x6c1bc3e71a5ef2b5(xc0752627b0d47dbb, MetaType.x41998983fed2df58.x6037fb37e2f942e7))
						{
							flag3 = (((x928589c3132a5f4f ? 1U : 0U) | 2147483647U) == 0U);
							if (flag3)
							{
								goto IL_2C6;
							}
							goto IL_270;
						}
						else
						{
							x9829c225286ead = MetaType.xe9642e4064adcc39(xb8c35680a4fe970e, "System.Runtime.Serialization.DataMemberAttribute");
							flag3 = ((flag4 ? 1U : 0U) - (uint)x66e0c6e69bac0ae4 < 0U);
							if (!flag3)
							{
								goto IL_2C6;
							}
						}
						IL_382:
						MetaType.xf660326bc61fc4ad(ref num, x9829c225286ead, "Order");
						flag3 = ((uint)num > uint.MaxValue);
						if (!flag3)
						{
							break;
						}
						flag3 = (((uint)x66e0c6e69bac0ae4 | uint.MaxValue) == 0U);
						if (flag3)
						{
							goto IL_673;
						}
						continue;
						IL_2C6:
						if (x9829c225286ead == null)
						{
							goto Block_24;
						}
						goto IL_382;
					}
					bool flag5;
					if ((flag5 ? 1U : 0U) + (flag4 ? 1U : 0U) <= 4294967295U)
					{
						goto IL_32F;
					}
					goto IL_A5C;
					Block_24:
					goto IL_270;
					IL_A38:
					flag = false;
					bool flag6;
					if (((flag6 ? 1U : 0U) | 1U) == 0U)
					{
						goto IL_156;
					}
					bool flag7 = false;
					flag3 = ((flag4 ? 1U : 0U) + (xba755397c46c6a0b ? 1U : 0U) > uint.MaxValue);
					if (flag3)
					{
						continue;
					}
					DataFormat dataFormat = DataFormat.Default;
					flag3 = ((flag ? 1U : 0U) + (flag ? 1U : 0U) < 0U);
					ProtoMemberAttribute protoMemberAttribute;
					if (flag3 || x928589c3132a5f4f)
					{
						x0d773e8ee668a258 = true;
						if (((uint)num & 0U) != 0U)
						{
							continue;
						}
						if (((x928589c3132a5f4f ? 1U : 0U) & 0U) != 0U)
						{
							return protoMemberAttribute;
						}
					}
					xb8c35680a4fe970e = x9829c225286ead14.xebcf83b00134300b(xad70a5849826ecef, xf0b74f36659f8180, true);
					int num2;
					string text;
					bool flag8;
					for (;;)
					{
						if (x928589c3132a5f4f)
						{
							x9829c225286ead = MetaType.xe9642e4064adcc39(xb8c35680a4fe970e, "ProtoBuf.ProtoIgnoreAttribute");
							if (x9829c225286ead == null)
							{
								x9829c225286ead = MetaType.xe9642e4064adcc39(xb8c35680a4fe970e, "ProtoBuf.ProtoEnumAttribute");
								if ((uint)num2 + (flag ? 1U : 0U) < 0U)
								{
									goto IL_32F;
								}
								num = Convert.ToInt32(((FieldInfo)xf0b74f36659f8180).GetRawConstantValue());
								if (((flag ? 1U : 0U) | 3U) == 0U)
								{
									goto IL_A60;
								}
								while (x9829c225286ead != null)
								{
									MetaType.xa0277ce3ce5870ea(ref text, x9829c225286ead, "Name");
									if (!(bool)x479f2661aae93792.x40c628a821a074f8(x9829c225286ead.AttributeType, "HasValue").Invoke(x9829c225286ead.Target, null))
									{
										break;
									}
									flag3 = (((uint)num2 & 0U) == 0U);
									if (!flag3)
									{
										goto IL_43A;
									}
									object obj;
									if (!x9829c225286ead.x034113016eff150a("Value", out obj))
									{
										break;
									}
									num = (int)obj;
									flag3 = ((flag6 ? 1U : 0U) - (flag4 ? 1U : 0U) > uint.MaxValue);
									if (!flag3)
									{
										flag3 = (((flag8 ? 1U : 0U) | 1U) == 0U);
										if (!flag3)
										{
											break;
										}
									}
									flag3 = ((flag ? 1U : 0U) + (flag5 ? 1U : 0U) < 0U);
									if (flag3)
									{
										break;
									}
								}
							}
							else
							{
								flag2 = true;
							}
							IL_852:
							flag4 = true;
							if ((flag6 ? 1U : 0U) - (flag6 ? 1U : 0U) <= 4294967295U)
							{
								goto IL_834;
							}
							goto IL_127;
							goto IL_852;
						}
						flag3 = ((x0d773e8ee668a258 ? 1U : 0U) - (flag6 ? 1U : 0U) < 0U);
						if (!flag3)
						{
							goto IL_7AD;
						}
						IL_834:
						if (((flag5 ? 1U : 0U) | 4294967294U) == 0U)
						{
							continue;
						}
						if (((uint)num2 | 15U) != 0U)
						{
							break;
						}
						IL_7AD:
						if (!false)
						{
							break;
						}
						goto IL_834;
					}
					goto IL_400;
					IL_A60:
					flag5 = false;
					flag2 = false;
					flag4 = false;
					bool flag9;
					if (((flag4 ? 1U : 0U) | 3U) != 0U)
					{
						flag8 = false;
						flag9 = false;
						flag6 = false;
						goto IL_A38;
					}
					flag3 = ((flag8 ? 1U : 0U) - (x0d773e8ee668a258 ? 1U : 0U) > uint.MaxValue);
					if (flag3)
					{
						goto IL_74F;
					}
					goto IL_404;
					IL_AFA:
					if (!x928589c3132a5f4f)
					{
						goto IL_B14;
					}
					if (((xba755397c46c6a0b ? 1U : 0U) & 0U) != 0U)
					{
						continue;
					}
					goto IL_A6E;
					IL_A5E:
					text = null;
					goto IL_A60;
					IL_A5D:
					int num3;
					num2 = num3;
					goto IL_A5E;
					IL_A6E:
					num = int.MinValue;
					if ((flag ? 1U : 0U) > 4294967295U)
					{
						goto IL_40D;
					}
					flag3 = ((x928589c3132a5f4f ? 1U : 0U) + (uint)x66e0c6e69bac0ae4 < 0U);
					if (flag3 || !xba755397c46c6a0b)
					{
						num3 = 1;
						goto IL_A5D;
					}
					IL_A5C:
					num3 = -1;
					goto IL_A5D;
					IL_175:
					if (flag4)
					{
						goto IL_173;
					}
					if (!MetaType.x6c1bc3e71a5ef2b5(xc0752627b0d47dbb, MetaType.x41998983fed2df58.xd2e84228f4f88898))
					{
						goto IL_14E;
					}
					x9829c225286ead = MetaType.xe9642e4064adcc39(xb8c35680a4fe970e, "System.Xml.Serialization.XmlElementAttribute");
					flag3 = (((uint)num | 15U) == 0U);
					if (flag3)
					{
						goto IL_2A2;
					}
					goto IL_2EE;
					IL_3D8:
					if (flag4)
					{
						goto IL_3C2;
					}
					if ((uint)num2 <= 4294967295U)
					{
						goto IL_3F1;
					}
					goto IL_175;
					IL_43A:
					if (x9829c225286ead != null)
					{
						goto IL_727;
					}
					if (((flag6 ? 1U : 0U) & 0U) != 0U)
					{
						goto IL_458;
					}
					goto IL_3D8;
					IL_622:
					if ((x928589c3132a5f4f ? 1U : 0U) >= 0U)
					{
						goto IL_43A;
					}
					goto IL_404;
					IL_3F1:
					if (xf1f230b9e66a016a == null)
					{
						goto IL_3F8;
					}
					using (IEnumerator enumerator = xf1f230b9e66a016a.GetEnumerator())
					{
						for (;;)
						{
							IL_4AF:
							if (enumerator.MoveNext())
							{
								goto IL_5D3;
							}
							if ((flag ? 1U : 0U) - (flag9 ? 1U : 0U) > 4294967295U)
							{
								goto IL_46A;
							}
							flag3 = ((x928589c3132a5f4f ? 1U : 0U) - (flag7 ? 1U : 0U) > uint.MaxValue);
							if (flag3)
							{
								goto IL_5D3;
							}
							break;
							continue;
							IL_46A:
							if (flag4 = (flag = (num > 0)))
							{
								break;
							}
							continue;
							IL_5D3:
							x9829c225286ead14 x9829c225286ead2 = (x9829c225286ead14)enumerator.Current;
							object obj2;
							if (x9829c225286ead2.x034113016eff150a("MemberName", out obj2))
							{
								flag3 = ((flag8 ? 1U : 0U) > uint.MaxValue);
								if (flag3)
								{
									break;
								}
								for (;;)
								{
									flag3 = ((flag ? 1U : 0U) < 0U);
									if (!flag3 && !((string)obj2 == xf0b74f36659f8180.Name))
									{
										goto IL_4AF;
									}
									for (;;)
									{
										MetaType.xf660326bc61fc4ad(ref num, x9829c225286ead2, "Tag");
										MetaType.xa0277ce3ce5870ea(ref text, x9829c225286ead2, "Name");
										MetaType.x5578649838859fff(ref flag8, x9829c225286ead2, "IsRequired");
										MetaType.x5578649838859fff(ref flag5, x9829c225286ead2, "IsPacked");
										MetaType.x5578649838859fff(ref flag7, x9829c225286ead, "OverwriteList");
										MetaType.x2d36184dd86bcc0e(ref dataFormat, x9829c225286ead2, "DataFormat");
										flag3 = (((flag7 ? 1U : 0U) | 2147483648U) == 0U);
										if (flag3)
										{
											break;
										}
										MetaType.x5578649838859fff(ref flag9, x9829c225286ead2, "AsReference");
										MetaType.x5578649838859fff(ref flag6, x9829c225286ead2, "DynamicType");
										if (((flag6 ? 1U : 0U) | 15U) != 0U)
										{
											goto Block_63;
										}
									}
								}
								Block_63:
								flag3 = ((xba755397c46c6a0b ? 1U : 0U) + (xba755397c46c6a0b ? 1U : 0U) > uint.MaxValue);
								if (!flag3 || (flag4 ? 1U : 0U) < 0U)
								{
									goto IL_46A;
								}
							}
						}
						goto IL_3C2;
					}
					goto IL_622;
					IL_2EE:
					if (((x0d773e8ee668a258 ? 1U : 0U) & 0U) == 0U)
					{
						flag3 = ((uint)x66e0c6e69bac0ae4 - (uint)x66e0c6e69bac0ae4 < 0U);
						if (flag3 || x9829c225286ead == null)
						{
							x9829c225286ead = MetaType.xe9642e4064adcc39(xb8c35680a4fe970e, "System.Xml.Serialization.XmlArrayAttribute");
							if ((flag ? 1U : 0U) + (x928589c3132a5f4f ? 1U : 0U) > 4294967295U)
							{
								goto IL_404;
							}
							if ((x928589c3132a5f4f ? 1U : 0U) < 0U)
							{
								goto IL_6C;
							}
						}
						MetaType.xb76bf0f7c3a7dbab(ref flag2, x9829c225286ead, xb8c35680a4fe970e, "System.Xml.Serialization.XmlIgnoreAttribute");
						if (((xba755397c46c6a0b ? 1U : 0U) & 0U) != 0U)
						{
							flag3 = ((flag9 ? 1U : 0U) - (uint)x66e0c6e69bac0ae4 > uint.MaxValue);
							if (flag3)
							{
								goto IL_2A2;
							}
						}
						else
						{
							if (x9829c225286ead == null || flag2)
							{
								goto IL_14E;
							}
							MetaType.xf660326bc61fc4ad(ref num, x9829c225286ead, "Order");
							MetaType.xa0277ce3ce5870ea(ref text, x9829c225286ead, "ElementName");
							flag4 = (num >= num2);
							goto IL_14E;
						}
					}
					goto IL_673;
					IL_32F:
					MetaType.xa0277ce3ce5870ea(ref text, x9829c225286ead, "Name");
					MetaType.x5578649838859fff(ref flag8, x9829c225286ead, "IsRequired");
					flag4 = (num >= num2);
					if (flag4)
					{
						num += x66e0c6e69bac0ae4;
						goto IL_270;
					}
					flag3 = ((x0d773e8ee668a258 ? 1U : 0U) + (x0d773e8ee668a258 ? 1U : 0U) < 0U);
					if (flag3)
					{
						goto IL_2EE;
					}
					goto IL_270;
					IL_727:
					MetaType.xf660326bc61fc4ad(ref num, x9829c225286ead, "Tag");
					MetaType.xa0277ce3ce5870ea(ref text, x9829c225286ead, "Name");
					MetaType.x5578649838859fff(ref flag8, x9829c225286ead, "IsRequired");
					MetaType.x5578649838859fff(ref flag5, x9829c225286ead, "IsPacked");
					flag3 = ((flag ? 1U : 0U) - (uint)num2 < 0U);
					if (flag3)
					{
						goto IL_722;
					}
					flag3 = ((flag7 ? 1U : 0U) - (flag9 ? 1U : 0U) > uint.MaxValue);
					if (flag3)
					{
						goto IL_74F;
					}
					flag3 = ((flag7 ? 1U : 0U) - (flag4 ? 1U : 0U) < 0U);
					if (flag3)
					{
						flag3 = ((uint)num + (xba755397c46c6a0b ? 1U : 0U) < 0U);
						if (flag3)
						{
							goto IL_7F0;
						}
						goto IL_622;
					}
					else
					{
						if ((uint)num + (flag8 ? 1U : 0U) <= 4294967295U)
						{
							MetaType.x5578649838859fff(ref flag7, x9829c225286ead, "OverwriteList");
							MetaType.x2d36184dd86bcc0e(ref dataFormat, x9829c225286ead, "DataFormat");
							MetaType.x5578649838859fff(ref flag9, x9829c225286ead, "AsReference");
							MetaType.x5578649838859fff(ref flag6, x9829c225286ead, "DynamicType");
							flag = (flag4 = (num > 0));
							goto IL_3D8;
						}
						goto IL_A5E;
					}
					IL_156:
					if (((uint)num | 1U) == 0U)
					{
						goto IL_727;
					}
					if (MetaType.xe9642e4064adcc39(xb8c35680a4fe970e, "System.NonSerializedAttribute") == null)
					{
						goto IL_116;
					}
					flag2 = true;
					goto IL_127;
					IL_14E:
					if (!flag2 && !flag4)
					{
						goto IL_156;
					}
					goto IL_116;
					IL_270:
					if (!flag2)
					{
						goto IL_175;
					}
					IL_173:
					goto IL_14E;
					IL_7F0:
					flag3 = ((flag6 ? 1U : 0U) - (uint)num2 > uint.MaxValue);
					if (flag3)
					{
						return protoMemberAttribute;
					}
					goto IL_173;
					IL_673:
					goto IL_7F0;
					IL_111:
					protoMemberAttribute.IsRequired = flag8;
					protoMemberAttribute.Name = (x479f2661aae93792.x1c140bd1078ddda1(text) ? xf0b74f36659f8180.Name : text);
					protoMemberAttribute.Member = xf0b74f36659f8180;
					if (false)
					{
						goto IL_87;
					}
					protoMemberAttribute.TagIsPinned = flag;
					flag3 = ((uint)num > uint.MaxValue);
					if (!flag3)
					{
						return protoMemberAttribute;
					}
					flag3 = ((uint)x66e0c6e69bac0ae4 > uint.MaxValue);
					if (!flag3)
					{
						goto IL_173;
					}
					IL_C6:
					protoMemberAttribute = new ProtoMemberAttribute(num, x0d773e8ee668a258 || xba755397c46c6a0b);
					if ((flag6 ? 1U : 0U) + (flag5 ? 1U : 0U) < 0U)
					{
						goto IL_458;
					}
					protoMemberAttribute.AsReference = flag9;
					if (((flag8 ? 1U : 0U) & 0U) != 0U)
					{
						goto IL_111;
					}
					goto IL_6C;
					IL_116:
					if (flag2)
					{
						break;
					}
					if (num >= num2)
					{
						goto IL_C6;
					}
					if (!x0d773e8ee668a258)
					{
						break;
					}
					goto IL_C6;
					IL_127:
					goto IL_116;
					IL_87:
					if (3 != 0)
					{
						protoMemberAttribute.DataFormat = dataFormat;
						protoMemberAttribute.DynamicType = flag6;
						protoMemberAttribute.IsPacked = flag5;
						protoMemberAttribute.OverwriteList = flag7;
						flag3 = ((flag ? 1U : 0U) > uint.MaxValue);
						if (flag3)
						{
							goto IL_C6;
						}
						goto IL_111;
					}
					else
					{
						flag3 = ((uint)num2 - (xba755397c46c6a0b ? 1U : 0U) < 0U);
						if (flag3)
						{
							goto IL_42A;
						}
						goto IL_400;
					}
					IL_6C:
					if (((flag9 ? 1U : 0U) | 4U) != 0U)
					{
						goto IL_87;
					}
					goto IL_40D;
					IL_722:
					if (!flag2)
					{
						goto IL_43A;
					}
					goto IL_3D8;
					IL_74F:
					x9829c225286ead = MetaType.xe9642e4064adcc39(xb8c35680a4fe970e, "ProtoBuf.ProtoMemberAttribute");
					MetaType.xb76bf0f7c3a7dbab(ref flag2, x9829c225286ead, xb8c35680a4fe970e, "ProtoBuf.ProtoIgnoreAttribute");
					goto IL_722;
					IL_404:
					if (flag4)
					{
						goto IL_3C2;
					}
					goto IL_74F;
					IL_400:
					if (!flag2)
					{
						goto IL_404;
					}
					goto IL_42A;
					IL_40D:
					if (((uint)num | 2U) != 0U)
					{
						goto IL_3C2;
					}
					goto IL_400;
					IL_458:
					goto IL_3F1;
					IL_3C2:
					if (flag2 || flag4)
					{
						goto IL_270;
					}
					if (!false)
					{
						goto IL_2A2;
					}
					IL_42A:
					IL_3F8:
					goto IL_3C2;
				}
				IL_125:
				return null;
				goto IL_125;
			}
			IL_B14:
			return null;
		}

		private ValueMember x26ad2611a3d84e83(bool x928589c3132a5f4f, ProtoMemberAttribute x6c48d6ce93880c76)
		{
			if (x6c48d6ce93880c76 != null)
			{
				MemberInfo member;
				while ((member = x6c48d6ce93880c76.Member) != null)
				{
					Type type = x479f2661aae93792.xc880d46f290519e2(member);
					Type type2 = null;
					Type defaultType = null;
					if ((x928589c3132a5f4f ? 1U : 0U) - (x928589c3132a5f4f ? 1U : 0U) > 4294967295U)
					{
						goto IL_6F;
					}
					int num;
					if ((x928589c3132a5f4f ? 1U : 0U) + (uint)num < 0U)
					{
						goto IL_88;
					}
					MetaType.xaadd3c2a45ddcdb5(this.xad70a5849826ecef, type, ref type2, ref defaultType);
					bool flag;
					x9829c225286ead14[] xb8c35680a4fe970e;
					object defaultValue;
					xd669244d58bc09c0 xd669244d58bc09c;
					for (;;)
					{
						if (type2 == null)
						{
							goto IL_4E6;
						}
						num = this.xad70a5849826ecef.x8a1227c17a49f070(type, false, true, false);
						IL_4E8:
						if (num < 0)
						{
							flag = (((uint)num & 0U) == 0U);
							if (flag)
							{
								goto IL_4CD;
							}
						}
						else
						{
							if (!this.xad70a5849826ecef[type].IgnoreListHandling)
							{
								goto IL_4CD;
							}
							type2 = null;
							defaultType = null;
						}
						flag = ((uint)num - (x928589c3132a5f4f ? 1U : 0U) < 0U);
						if (flag)
						{
							continue;
						}
						IL_4CD:
						xb8c35680a4fe970e = x9829c225286ead14.xebcf83b00134300b(this.xad70a5849826ecef, member, true);
						defaultValue = null;
						if (15 != 0)
						{
							while (this.xad70a5849826ecef.UseImplicitZeroDefaults)
							{
								xd669244d58bc09c = x479f2661aae93792.xf70eec89828a813c(type);
								if (false)
								{
									goto IL_4E8;
								}
								if (false)
								{
									goto IL_183;
								}
								switch (xd669244d58bc09c)
								{
								case xd669244d58bc09c0.x795dc524dba3fd4b:
									goto IL_41B;
								case xd669244d58bc09c0.xfb1fc02db7c42694:
									goto IL_3AE;
								case xd669244d58bc09c0.x2025ae83be2038f0:
									goto IL_2FD;
								case xd669244d58bc09c0.xc0f9b651d77da240:
									goto IL_3E0;
								case xd669244d58bc09c0.x697a219ddc6427a9:
									goto IL_3BB;
								case xd669244d58bc09c0.xf12cc4804eec7b89:
									goto IL_2D8;
								case xd669244d58bc09c0.x85254000935bfc25:
									defaultValue = 0;
									if ((x928589c3132a5f4f ? 1U : 0U) - (uint)num > 4294967295U)
									{
										goto IL_2B9;
									}
									flag = ((uint)num + (uint)num > uint.MaxValue);
									if (flag)
									{
										goto IL_382;
									}
									goto IL_30A;
								case xd669244d58bc09c0.x6cedabde5251b1e5:
									defaultValue = 0U;
									goto IL_382;
								case xd669244d58bc09c0.x0b2292ab52b25d76:
									goto IL_2EF;
								case xd669244d58bc09c0.x394150f1be471c3c:
									goto IL_2B9;
								case xd669244d58bc09c0.x63374d6ffed4adeb:
									goto IL_40A;
								case xd669244d58bc09c0.x94c083f2813272f4:
									goto IL_3F3;
								case xd669244d58bc09c0.x18d3f7d37d3464ca:
									goto IL_428;
								}
								goto Block_25;
								IL_382:
								if (false)
								{
									goto Block_20;
								}
								flag = ((uint)num < 0U);
								if (!flag)
								{
									goto IL_4C8;
								}
							}
							goto Block_15;
						}
						IL_4E6:
						goto IL_4CD;
					}
					IL_286:
					ValueMember valueMember;
					if (false)
					{
						if (-2147483648 == 0)
						{
							goto IL_288;
						}
						if (((uint)num & 0U) != 0U)
						{
							if (false)
							{
								continue;
							}
							goto IL_11C;
						}
					}
					else if (valueMember == null)
					{
						goto IL_7F;
					}
					Type xe1617f817cdad6f = this.x43163d22e8cd5a71;
					PropertyInfo propertyInfo = x479f2661aae93792.x4ff37084a5d7d57f(xe1617f817cdad6f, member.Name + "Specified");
					MethodInfo methodInfo = x479f2661aae93792.xbfdf1fd45a6330fd(propertyInfo, true);
					goto IL_183;
					IL_1E7:
					ValueMember valueMember2;
					valueMember = valueMember2;
					flag = ((x928589c3132a5f4f ? 1U : 0U) - (x928589c3132a5f4f ? 1U : 0U) < 0U);
					if (flag)
					{
						goto IL_201;
					}
					goto IL_286;
					IL_1BB:
					if (x6c48d6ce93880c76.Tag > 0)
					{
						goto IL_1C4;
					}
					valueMember2 = null;
					goto IL_1E7;
					IL_1B3:
					if (!x928589c3132a5f4f)
					{
						goto IL_1BB;
					}
					goto IL_1C4;
					IL_19F:
					x9829c225286ead14 x9829c225286ead;
					object obj;
					if ((x9829c225286ead = MetaType.xe9642e4064adcc39(xb8c35680a4fe970e, "System.ComponentModel.DefaultValueAttribute")) == null || !x9829c225286ead.x034113016eff150a("Value", out obj))
					{
						goto IL_1B3;
					}
					flag = ((uint)num - (uint)num < 0U);
					if (flag)
					{
						goto IL_25D;
					}
					if (false)
					{
						goto IL_286;
					}
					goto IL_201;
					Block_15:
					if ((uint)num - (x928589c3132a5f4f ? 1U : 0U) > 4294967295U)
					{
						goto IL_88;
					}
					if (!false)
					{
						goto IL_19F;
					}
					goto IL_340;
					IL_1C4:
					valueMember2 = new ValueMember(this.xad70a5849826ecef, this.x43163d22e8cd5a71, x6c48d6ce93880c76.Tag, member, type, type2, defaultType, x6c48d6ce93880c76.DataFormat, defaultValue);
					goto IL_1E7;
					IL_201:
					defaultValue = obj;
					goto IL_1B3;
					IL_25D:
					defaultValue = Guid.Empty;
					IL_288:
					goto IL_19F;
					IL_5E1:
					goto IL_288;
					IL_4C8:
					flag = ((uint)num + (x928589c3132a5f4f ? 1U : 0U) > uint.MaxValue);
					if (flag)
					{
						goto IL_5E1;
					}
					goto IL_19F;
					IL_2B9:
					defaultValue = 0UL;
					goto IL_19F;
					IL_2D8:
					defaultValue = 0;
					goto IL_19F;
					IL_2EF:
					defaultValue = 0L;
					goto IL_19F;
					IL_2FD:
					defaultValue = 0;
					IL_340:
					goto IL_19F;
					IL_39D:
					if (-2147483648 != 0)
					{
						goto IL_19F;
					}
					goto IL_40A;
					IL_3BB:
					defaultValue = 0;
					flag = ((x928589c3132a5f4f ? 1U : 0U) + (uint)num > uint.MaxValue);
					if (flag)
					{
						goto IL_3DB;
					}
					if (-2 == 0)
					{
						goto IL_41B;
					}
					goto IL_39D;
					Block_20:
					flag = ((uint)num > uint.MaxValue);
					if (flag)
					{
						goto IL_39D;
					}
					goto IL_41B;
					IL_3AE:
					defaultValue = '\0';
					IL_3DB:
					goto IL_19F;
					IL_3F3:
					defaultValue = 0.0;
					goto IL_3DB;
					IL_3E0:
					defaultValue = 0;
					goto IL_19F;
					IL_40A:
					defaultValue = 0f;
					goto IL_19F;
					IL_41B:
					defaultValue = false;
					goto IL_19F;
					IL_428:
					defaultValue = 0m;
					goto IL_19F;
					Block_25:
					switch (xd669244d58bc09c)
					{
					case xd669244d58bc09c0.x69ec9d2404a6b229:
						defaultValue = TimeSpan.Zero;
						goto IL_5E1;
					case xd669244d58bc09c0.x62a0c09ce3a5e8fb:
						goto IL_19F;
					case xd669244d58bc09c0.x0217cda8370c1f17:
						goto IL_25D;
					default:
						goto IL_19F;
					}
					IL_183:
					goto IL_F4;
					IL_30A:
					if (((x928589c3132a5f4f ? 1U : 0U) | 255U) == 0U)
					{
						goto IL_F4;
					}
					if ((x928589c3132a5f4f ? 1U : 0U) + (x928589c3132a5f4f ? 1U : 0U) <= 4294967295U)
					{
						goto IL_340;
					}
					goto IL_1BB;
					IL_7F:
					flag = ((uint)num - (uint)num > uint.MaxValue);
					if (flag)
					{
						goto IL_11C;
					}
					flag = ((x928589c3132a5f4f ? 1U : 0U) + (x928589c3132a5f4f ? 1U : 0U) > uint.MaxValue);
					if (flag)
					{
						continue;
					}
					return valueMember;
					IL_6F:
					valueMember.IsPacked = x6c48d6ce93880c76.IsPacked;
					if (false)
					{
						goto IL_7F;
					}
					valueMember.IsRequired = x6c48d6ce93880c76.IsRequired;
					valueMember.OverwriteList = x6c48d6ce93880c76.OverwriteList;
					valueMember.AsReference = x6c48d6ce93880c76.AsReference;
					valueMember.DynamicType = x6c48d6ce93880c76.DynamicType;
					return valueMember;
					IL_55:
					if (!x479f2661aae93792.x1c140bd1078ddda1(x6c48d6ce93880c76.Name))
					{
						valueMember.x54f99ef1e934e59c(x6c48d6ce93880c76.Name);
						goto IL_6F;
					}
					goto IL_6F;
					IL_81:
					if (propertyInfo == null)
					{
						goto IL_88;
					}
					valueMember.SetSpecified(methodInfo, x479f2661aae93792.x960b3b86642a1b2d(propertyInfo, true));
					goto IL_55;
					IL_F4:
					if (methodInfo == null)
					{
						goto IL_12E;
					}
					if (!methodInfo.IsStatic)
					{
						goto IL_81;
					}
					goto IL_12E;
					IL_11C:
					flag = ((x928589c3132a5f4f ? 1U : 0U) > uint.MaxValue);
					if (flag)
					{
						goto IL_12E;
					}
					goto IL_F4;
					IL_88:
					MethodInfo methodInfo2 = x479f2661aae93792.x40c628a821a074f8(xe1617f817cdad6f, "ShouldSerialize" + member.Name, x479f2661aae93792.xf6f6ea67665595b8);
					if (methodInfo2 != null && methodInfo2.ReturnType == this.xad70a5849826ecef.MapType(typeof(bool)))
					{
						valueMember.SetSpecified(methodInfo2, null);
						goto IL_55;
					}
					goto IL_55;
					IL_12E:
					propertyInfo = null;
					goto IL_81;
				}
			}
			return null;
		}

		private static void x2d36184dd86bcc0e(ref DataFormat xbcea506a33cf9111, x9829c225286ead14 x4e29d4d73b272868, string xc42b4197632b312c)
		{
			if (x4e29d4d73b272868 == null || xbcea506a33cf9111 != DataFormat.Default)
			{
				return;
			}
			object obj;
			if (x4e29d4d73b272868.x034113016eff150a(xc42b4197632b312c, out obj))
			{
				if (obj != null)
				{
					xbcea506a33cf9111 = (DataFormat)obj;
				}
			}
		}

		private static void xb76bf0f7c3a7dbab(ref bool x33bb7d1cf2dc2466, x9829c225286ead14 x4e29d4d73b272868, x9829c225286ead14[] xb8c35680a4fe970e, string x01366b5d2e943495)
		{
			if (!x33bb7d1cf2dc2466 && (false || x4e29d4d73b272868 != null))
			{
				x33bb7d1cf2dc2466 = (MetaType.xe9642e4064adcc39(xb8c35680a4fe970e, x01366b5d2e943495) != null);
				return;
			}
		}

		private static void x5578649838859fff(ref bool xbcea506a33cf9111, x9829c225286ead14 x4e29d4d73b272868, string xc42b4197632b312c)
		{
			if (x4e29d4d73b272868 != null && -2 != 0 && !xbcea506a33cf9111)
			{
				if (2147483647 != 0)
				{
				}
				object obj;
				if (x4e29d4d73b272868.x034113016eff150a(xc42b4197632b312c, out obj))
				{
					if (obj != null)
					{
						xbcea506a33cf9111 = (bool)obj;
					}
				}
				return;
			}
		}

		private static void xf660326bc61fc4ad(ref int xbcea506a33cf9111, x9829c225286ead14 x4e29d4d73b272868, string xc42b4197632b312c)
		{
			if (x4e29d4d73b272868 != null)
			{
				IL_26:
				while (xbcea506a33cf9111 <= 0)
				{
					object obj;
					while (x4e29d4d73b272868.x034113016eff150a(xc42b4197632b312c, out obj) || -1 == 0)
					{
						if (obj != null)
						{
							xbcea506a33cf9111 = (int)obj;
							if (!false)
							{
								if (4 == 0 || false)
								{
									continue;
								}
								if (false)
								{
									goto IL_26;
								}
							}
						}
						return;
					}
					return;
				}
			}
		}

		private static void xa0277ce3ce5870ea(ref string xc15bd84e01929885, x9829c225286ead14 x4e29d4d73b272868, string xc42b4197632b312c)
		{
			if (x4e29d4d73b272868 != null)
			{
				if (15 == 0)
				{
				}
				IL_3B:
				while (x479f2661aae93792.x1c140bd1078ddda1(xc15bd84e01929885))
				{
					object obj;
					while (!x4e29d4d73b272868.x034113016eff150a(xc42b4197632b312c, out obj))
					{
						if (!false)
						{
							if (false)
							{
								if (false)
								{
									continue;
								}
								goto IL_3B;
							}
						}
						else
						{
							IL_25:
							if (obj != null)
							{
								xc15bd84e01929885 = (string)obj;
								if (false)
								{
									goto IL_48;
								}
							}
						}
						return;
					}
					if (!false)
					{
						goto IL_25;
					}
				}
				return;
				IL_48:
				goto IL_3B;
			}
		}

		private static x9829c225286ead14 xe9642e4064adcc39(x9829c225286ead14[] xb8c35680a4fe970e, string x01366b5d2e943495)
		{
			int num = 0;
			x9829c225286ead14 x9829c225286ead;
			for (;;)
			{
				if (num < xb8c35680a4fe970e.Length)
				{
					x9829c225286ead = xb8c35680a4fe970e[num];
					goto IL_61;
				}
				if (3 == 0)
				{
					if ((uint)num > 4294967295U)
					{
						goto IL_20;
					}
					if (!false)
					{
						if (false)
						{
							goto IL_61;
						}
					}
				}
				else
				{
					bool flag = (uint)num - (uint)num > uint.MaxValue;
					if (flag)
					{
						goto IL_61;
					}
					goto IL_68;
				}
				IL_0D:
				num++;
				continue;
				IL_33:
				goto IL_0D;
				IL_20:
				if (!(x9829c225286ead.AttributeType.FullName == x01366b5d2e943495))
				{
					goto IL_33;
				}
				break;
				IL_61:
				if (x9829c225286ead != null)
				{
					goto IL_20;
				}
				goto IL_0D;
			}
			return x9829c225286ead;
			IL_68:
			return null;
		}

		public MetaType Add(int fieldNumber, string memberName)
		{
			this.x69dc2014b9eea9e3(fieldNumber, memberName, null, null, null);
			return this;
		}

		public ValueMember AddField(int fieldNumber, string memberName)
		{
			return this.x69dc2014b9eea9e3(fieldNumber, memberName, null, null, null);
		}

		public bool UseConstructor
		{
			get
			{
				return !this.x9b280141de83da2b(16);
			}
			set
			{
				this.x38efbc9b1b17e078(16, !value, true);
			}
		}

		public Type ConstructType
		{
			get
			{
				return this.x718bc396727ef5e4;
			}
			set
			{
				this.ThrowIfFrozen();
				this.x718bc396727ef5e4 = value;
			}
		}

		public MetaType Add(string memberName)
		{
			this.Add(this.x64f0230a892ee8f4(), memberName);
			return this;
		}

		public void SetSurrogate(Type surrogateType)
		{
			if (surrogateType != this.x43163d22e8cd5a71)
			{
				goto IL_67;
			}
			surrogateType = null;
			if (255 != 0)
			{
				goto IL_67;
			}
			if (!false)
			{
				goto IL_A0;
			}
			goto IL_A0;
			IL_2B:
			this.ThrowIfFrozen();
			if (!false)
			{
				if (15 == 0)
				{
					goto IL_67;
				}
				if (255 != 0)
				{
					this.xd467cf11a28aa761 = surrogateType;
					if (255 != 0)
					{
						return;
					}
				}
				if (255 == 0)
				{
					goto IL_7C;
				}
				goto IL_47;
			}
			return;
			IL_42:
			if (surrogateType == null)
			{
				goto IL_2B;
			}
			IL_47:
			if (x479f2661aae93792.xd3703a5e339a1b56(this.xad70a5849826ecef.MapType(typeof(IEnumerable)), surrogateType))
			{
				goto IL_7C;
			}
			if (!false)
			{
				if (true)
				{
					goto IL_2B;
				}
				goto IL_A0;
			}
			IL_67:
			if (surrogateType == null)
			{
				goto IL_2B;
			}
			goto IL_42;
			IL_7C:
			throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot be used as a surrogate");
			IL_A0:
			goto IL_42;
		}

		internal MetaType x819a5515328d902f()
		{
			if (this.xd467cf11a28aa761 != null)
			{
				return this.xad70a5849826ecef[this.xd467cf11a28aa761];
			}
			return this;
		}

		internal MetaType xda6e605e9a5fa5d2()
		{
			if (this.xd467cf11a28aa761 != null)
			{
				if (!false)
				{
				}
				return this.xad70a5849826ecef[this.xd467cf11a28aa761];
			}
			if (this.x6227b26cefabe265 == null)
			{
				return this;
			}
			return this.x6227b26cefabe265;
		}

		private int x64f0230a892ee8f4()
		{
			int num = 0;
			foreach (object obj in this.xa942970cc8a85fd4)
			{
				ValueMember valueMember = (ValueMember)obj;
				if (valueMember.FieldNumber > num)
				{
					num = valueMember.FieldNumber;
				}
			}
			if (this.x54cc87219996ce22 != null)
			{
				foreach (object obj2 in this.x54cc87219996ce22)
				{
					SubType subType = (SubType)obj2;
					if ((uint)num > 4294967295U || subType.FieldNumber > num)
					{
						num = subType.FieldNumber;
					}
				}
			}
			return num + 1;
		}

		public MetaType Add(params string[] memberNames)
		{
			int num = this.x64f0230a892ee8f4();
			int i;
			if ((uint)i >= 0U)
			{
				i = 0;
			}
			while (i < memberNames.Length)
			{
				this.Add(num++, memberNames[i]);
				i++;
			}
			return this;
		}

		public MetaType Add(int fieldNumber, string memberName, object defaultValue)
		{
			this.x69dc2014b9eea9e3(fieldNumber, memberName, null, null, defaultValue);
			return this;
		}

		public MetaType Add(int fieldNumber, string memberName, Type itemType, Type defaultType)
		{
			this.x69dc2014b9eea9e3(fieldNumber, memberName, itemType, defaultType, null);
			return this;
		}

		public ValueMember AddField(int fieldNumber, string memberName, Type itemType, Type defaultType)
		{
			return this.x69dc2014b9eea9e3(fieldNumber, memberName, itemType, defaultType, null);
		}

		private ValueMember x69dc2014b9eea9e3(int xade3b695478596d6, string xc42b4197632b312c, Type xd99217279677497c, Type x284cae105ee79236, object xc6e85c82d0d89508)
		{
			MemberInfo memberInfo = null;
			MemberInfo[] member = this.x43163d22e8cd5a71.GetMember(xc42b4197632b312c, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (member == null)
			{
				goto IL_AE;
			}
			if (member.Length == 1)
			{
				memberInfo = member[0];
				goto IL_AE;
			}
			if (!false)
			{
				goto IL_AE;
			}
			IL_93:
			throw new ArgumentException("Unable to determine member: " + xc42b4197632b312c, "memberName");
			IL_AE:
			if (memberInfo != null)
			{
				MemberTypes memberType = memberInfo.MemberType;
				Type memberType2;
				if (memberType != MemberTypes.Field)
				{
					if (memberType != MemberTypes.Property)
					{
						throw new NotSupportedException(memberInfo.MemberType.ToString());
					}
					memberType2 = ((PropertyInfo)memberInfo).PropertyType;
				}
				else
				{
					memberType2 = ((FieldInfo)memberInfo).FieldType;
				}
				MetaType.xaadd3c2a45ddcdb5(this.xad70a5849826ecef, memberType2, ref xd99217279677497c, ref x284cae105ee79236);
				ValueMember valueMember = new ValueMember(this.xad70a5849826ecef, this.x43163d22e8cd5a71, xade3b695478596d6, memberInfo, memberType2, xd99217279677497c, x284cae105ee79236, DataFormat.Default, xc6e85c82d0d89508);
				this.xd6b6ed77479ef68c(valueMember);
				return valueMember;
			}
			goto IL_93;
		}

		internal static void xaadd3c2a45ddcdb5(TypeModel xad70a5849826ecef, Type x43163d22e8cd5a71, ref Type xd99217279677497c, ref Type x284cae105ee79236)
		{
			if (x43163d22e8cd5a71 == null)
			{
				if (false)
				{
					goto IL_1AA;
				}
				return;
			}
			else
			{
				if (!x43163d22e8cd5a71.IsArray)
				{
					goto IL_1AD;
				}
				if (x43163d22e8cd5a71.GetArrayRank() != 1)
				{
					throw new NotSupportedException("Multi-dimension arrays are supported");
				}
				goto IL_20F;
			}
			IL_1D:
			if (-1 != 0)
			{
				return;
			}
			IL_2E:
			if (!x479f2661aae93792.xd3703a5e339a1b56(x43163d22e8cd5a71, x284cae105ee79236))
			{
				x284cae105ee79236 = null;
				goto IL_70;
			}
			if (!false && !false)
			{
				goto IL_4D;
			}
			if (2 != 0)
			{
				goto IL_1D;
			}
			IL_42:
			if (x284cae105ee79236 != null)
			{
				goto IL_2E;
			}
			if (4 != 0)
			{
				goto IL_1D;
			}
			IL_4D:
			if (2 == 0)
			{
				goto IL_20F;
			}
			if (-2 != 0)
			{
				return;
			}
			if (!false)
			{
				goto IL_81;
			}
			if (!false)
			{
				goto IL_75;
			}
			IL_63:
			if (!x43163d22e8cd5a71.IsInterface)
			{
				goto IL_42;
			}
			if (!x43163d22e8cd5a71.IsGenericType)
			{
				if (false)
				{
					goto IL_91;
				}
				goto IL_B2;
			}
			else
			{
				if (x43163d22e8cd5a71.GetGenericTypeDefinition() == xad70a5849826ecef.MapType(typeof(IDictionary<, >)))
				{
					goto IL_147;
				}
				goto IL_B2;
			}
			IL_70:
			return;
			IL_75:
			if (2147483647 == 0)
			{
				goto IL_17C;
			}
			IL_81:
			goto IL_42;
			IL_91:
			Type[] genericArguments;
			if (xd99217279677497c == xad70a5849826ecef.MapType(typeof(KeyValuePair<, >)).MakeGenericType(genericArguments = x43163d22e8cd5a71.GetGenericArguments()))
			{
				x284cae105ee79236 = xad70a5849826ecef.MapType(typeof(Dictionary<, >)).MakeGenericType(genericArguments);
				goto IL_81;
			}
			IL_B2:
			x284cae105ee79236 = xad70a5849826ecef.MapType(typeof(List<>)).MakeGenericType(new Type[]
			{
				xd99217279677497c
			});
			goto IL_75;
			IL_FC:
			if (x284cae105ee79236 == null)
			{
				goto IL_63;
			}
			goto IL_42;
			IL_147:
			goto IL_91;
			IL_178:
			Type type;
			if (xd99217279677497c != null)
			{
				type = null;
				goto IL_1A6;
			}
			IL_17C:
			IL_181:
			if (xd99217279677497c == null)
			{
				return;
			}
			if (2147483647 == 0)
			{
				goto IL_17C;
			}
			if (false)
			{
				goto IL_70;
			}
			if (x284cae105ee79236 != null)
			{
				return;
			}
			if (-2 == 0)
			{
				goto IL_178;
			}
			goto IL_1C3;
			IL_1A6:
			Type type2 = null;
			if (8 == 0)
			{
				goto IL_1C3;
			}
			MetaType.xaadd3c2a45ddcdb5(xad70a5849826ecef, xd99217279677497c, ref type, ref type2);
			if (type == null)
			{
				goto IL_181;
			}
			throw TypeModel.x65d41140f79f7ce3();
			IL_1AA:
			if (!false && 4 != 0)
			{
				goto IL_178;
			}
			IL_1AD:
			if (xd99217279677497c != null)
			{
				goto IL_178;
			}
			goto IL_1D2;
			IL_1C3:
			if (!x43163d22e8cd5a71.IsClass)
			{
				goto IL_FC;
			}
			if (-2 != 0 && x43163d22e8cd5a71.IsAbstract)
			{
				if (false)
				{
					goto IL_147;
				}
				goto IL_FC;
			}
			else
			{
				if (x479f2661aae93792.xdaa1a96eb962bff0(x43163d22e8cd5a71, x479f2661aae93792.xf6f6ea67665595b8, true) != null)
				{
					x284cae105ee79236 = x43163d22e8cd5a71;
					goto IL_FC;
				}
				goto IL_FC;
			}
			IL_1D2:
			xd99217279677497c = TypeModel.x4431fb61faccece0(xad70a5849826ecef, x43163d22e8cd5a71);
			goto IL_26A;
			IL_20F:
			xd99217279677497c = x43163d22e8cd5a71.GetElementType();
			if (xd99217279677497c == xad70a5849826ecef.MapType(typeof(byte)))
			{
				if (2147483647 == 0)
				{
					goto IL_FC;
				}
				if (false)
				{
					goto IL_2E;
				}
			}
			else
			{
				x284cae105ee79236 = x43163d22e8cd5a71;
				if (255 != 0)
				{
					if (false)
					{
						goto IL_FC;
					}
					if (!false)
					{
						goto IL_1AD;
					}
					if (false)
					{
						goto IL_1EC;
					}
				}
				if (false)
				{
					goto IL_26A;
				}
				goto IL_1D2;
			}
			IL_1EC:
			Type type3;
			xd99217279677497c = (type3 = null);
			x284cae105ee79236 = type3;
			if (false)
			{
				goto IL_63;
			}
			if (255 != 0)
			{
				goto IL_1AD;
			}
			goto IL_1A6;
			IL_26A:
			if (15 != 0)
			{
				goto IL_1AA;
			}
		}

		private void xd6b6ed77479ef68c(ValueMember xf0b74f36659f8180)
		{
			int x53a1af2280f1a = 0;
			try
			{
				this.xad70a5849826ecef.x2c0c646a23b0da22(ref x53a1af2280f1a);
				this.ThrowIfFrozen();
				this.xa942970cc8a85fd4.xd6b6ed77479ef68c(xf0b74f36659f8180);
			}
			finally
			{
				this.xad70a5849826ecef.x7109eb31e17d8f9e(x53a1af2280f1a);
			}
		}

		public ValueMember this[int fieldNumber]
		{
			get
			{
				foreach (object obj in this.xa942970cc8a85fd4)
				{
					ValueMember valueMember = (ValueMember)obj;
					ValueMember result;
					if (2147483647 != 0)
					{
						if (valueMember.FieldNumber != fieldNumber)
						{
							continue;
						}
						result = valueMember;
					}
					return result;
				}
				return null;
			}
		}

		public ValueMember this[MemberInfo member]
		{
			get
			{
				if (member == null)
				{
					return null;
				}
				using (IEnumerator enumerator = this.xa942970cc8a85fd4.GetEnumerator())
				{
					while (enumerator.MoveNext() || !true)
					{
						ValueMember valueMember = (ValueMember)enumerator.Current;
						if (false || valueMember.Member == member)
						{
							return valueMember;
						}
					}
				}
				return null;
			}
		}

		public ValueMember[] GetFields()
		{
			ValueMember[] array = new ValueMember[this.xa942970cc8a85fd4.xd44988f225497f3a];
			this.xa942970cc8a85fd4.x0fe4f26e70030075(array, 0);
			Array.Sort<ValueMember>(array, ValueMember.xf7b8e510ae9ad738.xb9715d2f06b63cf0);
			return array;
		}

		public SubType[] GetSubtypes()
		{
			if (this.x54cc87219996ce22 == null || this.x54cc87219996ce22.xd44988f225497f3a == 0)
			{
				return new SubType[0];
			}
			SubType[] array;
			do
			{
				array = new SubType[this.x54cc87219996ce22.xd44988f225497f3a];
			}
			while (3 == 0);
			this.x54cc87219996ce22.x0fe4f26e70030075(array, 0);
			Array.Sort<SubType>(array, SubType.xf7b8e510ae9ad738.xb9715d2f06b63cf0);
			return array;
		}

		internal bool xf491e4040205bc52(int xade3b695478596d6)
		{
			using (IEnumerator enumerator = this.xa942970cc8a85fd4.GetEnumerator())
			{
				while (enumerator.MoveNext() || false)
				{
					ValueMember valueMember = (ValueMember)enumerator.Current;
					if (valueMember.FieldNumber == xade3b695478596d6)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal int xf15263674eb297bb(bool xcfbb09eff68775f8, bool xdfdd2812486e44d2)
		{
			return this.xad70a5849826ecef.xf15263674eb297bb(this.x43163d22e8cd5a71, xcfbb09eff68775f8, xdfdd2812486e44d2);
		}

		internal x9caf06b8b9c3a409.EnumPair[] x0b51e8612d340c57()
		{
			int fieldNumber;
			x9caf06b8b9c3a409.EnumPair[] array;
			int num;
			if (this.x9b280141de83da2b(2))
			{
				bool flag = (uint)fieldNumber > uint.MaxValue;
				if (!flag)
				{
					goto IL_52;
				}
			}
			else
			{
				array = new x9caf06b8b9c3a409.EnumPair[this.xa942970cc8a85fd4.xd44988f225497f3a];
				if (3 != 0)
				{
					num = 0;
					goto IL_39;
				}
				goto IL_52;
			}
			IL_1B:
			object raw;
			ValueMember valueMember;
			array[num] = new x9caf06b8b9c3a409.EnumPair(fieldNumber, raw, valueMember.MemberType);
			num++;
			IL_39:
			if (num >= array.Length)
			{
				return array;
			}
			valueMember = (ValueMember)this.xa942970cc8a85fd4.get_xe6d4b1b411ed94b5(num);
			fieldNumber = valueMember.FieldNumber;
			raw = valueMember.x4f62632ce341aab7();
			goto IL_1B;
			IL_52:
			return null;
		}

		public bool EnumPassthru
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

		public bool IgnoreListHandling
		{
			get
			{
				return this.x9b280141de83da2b(128);
			}
			set
			{
				this.x38efbc9b1b17e078(128, value, true);
			}
		}

		internal bool xfad393511d4f7236
		{
			get
			{
				return this.x9b280141de83da2b(1);
			}
			set
			{
				this.x38efbc9b1b17e078(1, value, false);
			}
		}

		private bool x9b280141de83da2b(byte x8fc2d66566293701)
		{
			return (this.xebf45bdcaa1fd1e1 & x8fc2d66566293701) == x8fc2d66566293701;
		}

		private void x38efbc9b1b17e078(byte x8fc2d66566293701, bool xbcea506a33cf9111, bool x826878e0588df0aa)
		{
			if (x826878e0588df0aa)
			{
				goto IL_6B;
			}
			IL_50:
			if (xbcea506a33cf9111)
			{
				this.xebf45bdcaa1fd1e1 |= x8fc2d66566293701;
				return;
			}
			bool flag = ((uint)x8fc2d66566293701 | 4U) == 0U;
			if (!flag)
			{
				flag = ((uint)x8fc2d66566293701 + (uint)x8fc2d66566293701 < 0U);
				if (!flag)
				{
					flag = (((xbcea506a33cf9111 ? 1U : 0U) & 0U) == 0U);
					if (flag)
					{
						this.xebf45bdcaa1fd1e1 &= ~x8fc2d66566293701;
					}
				}
				return;
			}
			IL_6B:
			if (this.x9b280141de83da2b(x8fc2d66566293701) != xbcea506a33cf9111)
			{
				this.ThrowIfFrozen();
				goto IL_50;
			}
			goto IL_50;
		}

		internal static MetaType xe52a8a367f0efe10(MetaType x337e217cb3ba0627)
		{
			while (x337e217cb3ba0627.x38c907674383c2da != null)
			{
				MetaType metaType = x337e217cb3ba0627.x6227b26cefabe265;
				if (metaType == null)
				{
					return x337e217cb3ba0627;
				}
				x337e217cb3ba0627 = metaType;
			}
			RuntimeTypeModel runtimeTypeModel;
			do
			{
				runtimeTypeModel = x337e217cb3ba0627.xad70a5849826ecef;
			}
			while (255 == 0);
			int x53a1af2280f1a = 0;
			try
			{
				runtimeTypeModel.x2c0c646a23b0da22(ref x53a1af2280f1a);
				MetaType metaType2;
				while ((metaType2 = x337e217cb3ba0627.x6227b26cefabe265) != null)
				{
					x337e217cb3ba0627 = metaType2;
				}
				return x337e217cb3ba0627;
			}
			finally
			{
				runtimeTypeModel.x7109eb31e17d8f9e(x53a1af2280f1a);
			}
			return x337e217cb3ba0627;
		}

		internal bool x0242a1cd267232b1()
		{
			return false;
		}

		internal IEnumerable x84aa3570d857bec4
		{
			get
			{
				return this.xa942970cc8a85fd4;
			}
		}

		internal static StringBuilder xd64bebcffdef70ee(StringBuilder xd07ce4b74c5774a7, int x94e516b5d3c734eb)
		{
			return x479f2661aae93792.xfa6a23a9b7368053(xd07ce4b74c5774a7).Append(' ', x94e516b5d3c734eb * 3);
		}

		internal bool x455b3702109fb5db
		{
			get
			{
				return this.x9b280141de83da2b(64);
			}
		}

		internal void x31407de8873120a6(StringBuilder xd07ce4b74c5774a7, int x94e516b5d3c734eb, ref bool x55b32f58d70e1b0f)
		{
			if (this.xd467cf11a28aa761 != null)
			{
				return;
			}
			ValueMember[] array = new ValueMember[this.xa942970cc8a85fd4.xd44988f225497f3a];
			this.xa942970cc8a85fd4.x0fe4f26e70030075(array, 0);
			Array.Sort<ValueMember>(array, ValueMember.xf7b8e510ae9ad738.xb9715d2f06b63cf0);
			bool flag;
			int num3;
			if (!this.x2e7a2ea5da15ce85)
			{
				int num;
				flag = ((uint)num < 0U);
				if (flag)
				{
					goto IL_4D;
				}
				IL_3D:
				if (!this.x455b3702109fb5db)
				{
					goto IL_4DC;
				}
				IL_4D:
				MemberInfo[] array2;
				int num2;
				if (MetaType.x3fb5a247ee976f3f(this.x43163d22e8cd5a71, out array2) != null)
				{
					MetaType.xd64bebcffdef70ee(xd07ce4b74c5774a7, x94e516b5d3c734eb).Append("message ").Append(this.xe2423e5eb2c7ed45()).Append(" {");
					num2 = 0;
					goto IL_719;
				}
				flag = ((uint)num2 > uint.MaxValue);
				if (flag)
				{
					goto IL_1D9;
				}
				goto IL_23E;
				IL_8B:
				MetaType.xd64bebcffdef70ee(xd07ce4b74c5774a7, x94e516b5d3c734eb).Append('}');
				flag = (((uint)num | 1U) == 0U);
				if (flag)
				{
					goto IL_23E;
				}
				flag = ((uint)num2 - (uint)num > uint.MaxValue);
				if (!flag)
				{
					return;
				}
				flag = ((uint)x94e516b5d3c734eb < 0U);
				if (!flag && !false)
				{
					goto IL_4D;
				}
				goto IL_3D;
				IL_9F:
				SubType[] array3;
				if (num >= array3.Length)
				{
					goto IL_8B;
				}
				SubType subType = array3[num];
				string value = subType.DerivedType.xe2423e5eb2c7ed45();
				MetaType.xd64bebcffdef70ee(xd07ce4b74c5774a7, x94e516b5d3c734eb + 1).Append("optional ").Append(value).Append(" ").Append(value).Append(" = ").Append(subType.FieldNumber).Append(';');
				IL_10C:
				if (((uint)num | 4294967295U) != 0U)
				{
					num++;
					goto IL_9F;
				}
				goto IL_416;
				IL_190:
				num3++;
				IL_196:
				ValueMember[] array4;
				SubType[] array5;
				ValueMember valueMember;
				string text;
				if (num3 >= array4.Length)
				{
					if (this.x54cc87219996ce22 == null || this.x54cc87219996ce22.xd44988f225497f3a == 0)
					{
						goto IL_8B;
					}
					MetaType.xd64bebcffdef70ee(xd07ce4b74c5774a7, x94e516b5d3c734eb + 1).Append("// the following represent sub-types; at most 1 should have a value");
					array5 = new SubType[this.x54cc87219996ce22.xd44988f225497f3a];
					goto IL_214;
				}
				else
				{
					valueMember = array4[num3];
					if (-1 == 0)
					{
						goto IL_416;
					}
					if (valueMember.ItemType != null)
					{
						text = "repeated";
						goto IL_3D4;
					}
					goto IL_41F;
				}
				IL_1D9:
				if ((uint)num3 - (uint)num3 < 0U)
				{
					goto IL_4A3;
				}
				if (!valueMember.AsReference)
				{
					flag = ((uint)num3 < 0U);
					if (!flag)
					{
						goto IL_190;
					}
				}
				else
				{
					if (!valueMember.DynamicType)
					{
						xd07ce4b74c5774a7.Append(" // reference-tracked ").Append(valueMember.xe2423e5eb2c7ed45(false, ref x55b32f58d70e1b0f));
					}
					goto IL_190;
				}
				IL_214:
				int i;
				flag = (((uint)i & 0U) == 0U);
				if (!flag)
				{
					goto IL_5F5;
				}
				this.x54cc87219996ce22.x0fe4f26e70030075(array5, 0);
				if ((uint)num2 >= 0U)
				{
					Array.Sort<SubType>(array5, SubType.xf7b8e510ae9ad738.xb9715d2f06b63cf0);
					array3 = array5;
					num = 0;
					goto IL_9F;
				}
				IL_23E:
				return;
				IL_3D4:
				string value2 = text;
				MetaType.xd64bebcffdef70ee(xd07ce4b74c5774a7, x94e516b5d3c734eb + 1).Append(value2).Append(' ');
				if (valueMember.DataFormat != DataFormat.Group)
				{
					goto IL_371;
				}
				flag = ((uint)x94e516b5d3c734eb > uint.MaxValue);
				if (flag)
				{
					goto IL_410;
				}
				xd07ce4b74c5774a7.Append("group ");
				goto IL_371;
				IL_285:
				xd07ce4b74c5774a7.Append(';');
				string text2;
				if (text2 == "bcl.NetObjectProxy")
				{
					goto IL_1D9;
				}
				goto IL_190;
				IL_290:
				if (valueMember.IsPacked)
				{
					xd07ce4b74c5774a7.Append(" [packed=true]");
				}
				goto IL_285;
				IL_2A7:
				if (valueMember.ItemType == null)
				{
					goto IL_285;
				}
				goto IL_290;
				IL_371:
				text2 = valueMember.xe2423e5eb2c7ed45(true, ref x55b32f58d70e1b0f);
				xd07ce4b74c5774a7.Append(text2).Append(" ").Append(valueMember.Name).Append(" = ").Append(valueMember.FieldNumber);
				if (valueMember.DefaultValue == null)
				{
					goto IL_2A7;
				}
				IL_410:
				flag = ((uint)i + (uint)i < 0U);
				if (flag)
				{
					flag = ((uint)num2 > uint.MaxValue);
					if (flag)
					{
						goto IL_45A;
					}
					return;
				}
				else if (!(valueMember.DefaultValue is string))
				{
					if (valueMember.DefaultValue is bool)
					{
						xd07ce4b74c5774a7.Append(((bool)valueMember.DefaultValue) ? " [default = true]" : " [default = false]");
						goto IL_2A7;
					}
					xd07ce4b74c5774a7.Append(" [default = ").Append(valueMember.DefaultValue).Append(']');
					goto IL_2A7;
				}
				else
				{
					xd07ce4b74c5774a7.Append(" [default = \"").Append(valueMember.DefaultValue).Append("\"]");
					if (((uint)num2 & 0U) != 0U)
					{
						goto IL_290;
					}
					goto IL_2A7;
				}
				IL_416:
				IL_41F:
				text = ((!valueMember.IsRequired) ? "optional" : "required");
				goto IL_3D4;
				IL_45A:
				goto IL_196;
				IL_47C:
				MetaType.xd64bebcffdef70ee(xd07ce4b74c5774a7, x94e516b5d3c734eb).Append("message ").Append(this.xe2423e5eb2c7ed45()).Append(" {");
				IL_4A3:
				array4 = array;
				num3 = 0;
				goto IL_45A;
				IL_4DC:
				if (!x479f2661aae93792.xb636fcab7a16c388(this.x43163d22e8cd5a71))
				{
					if (!false)
					{
						goto IL_47C;
					}
					goto IL_10C;
				}
				else
				{
					MetaType.xd64bebcffdef70ee(xd07ce4b74c5774a7, x94e516b5d3c734eb).Append("enum ").Append(this.xe2423e5eb2c7ed45()).Append(" {");
					ValueMember[] array6 = array;
					i = 0;
					while (i < array6.Length)
					{
						ValueMember valueMember2 = array6[i];
						MetaType.xd64bebcffdef70ee(xd07ce4b74c5774a7, x94e516b5d3c734eb + 1).Append(valueMember2.Name).Append(" = ").Append(valueMember2.FieldNumber).Append(';');
						i++;
						if ((uint)i < 0U)
						{
							goto IL_719;
						}
					}
					MetaType.xd64bebcffdef70ee(xd07ce4b74c5774a7, x94e516b5d3c734eb).Append('}');
					if ((uint)num >= 0U)
					{
						return;
					}
					goto IL_196;
				}
				IL_5F5:
				throw new NotSupportedException("Unknown member type: " + array2[num2].GetType().Name);
				IL_719:
				for (;;)
				{
					Type x7bc89d21aec0483a;
					if (num2 >= array2.Length)
					{
						MetaType.xd64bebcffdef70ee(xd07ce4b74c5774a7, x94e516b5d3c734eb).Append('}');
						flag = ((uint)num3 - (uint)i > uint.MaxValue);
						if (flag)
						{
							goto IL_5E8;
						}
						return;
					}
					else
					{
						if (!(array2[num2] is PropertyInfo))
						{
							goto IL_5E8;
						}
						x7bc89d21aec0483a = ((PropertyInfo)array2[num2]).PropertyType;
					}
					IL_612:
					MetaType.xd64bebcffdef70ee(xd07ce4b74c5774a7, x94e516b5d3c734eb + 1).Append("optional ").Append(this.xad70a5849826ecef.xe2423e5eb2c7ed45(x7bc89d21aec0483a, DataFormat.Default, false, false, ref x55b32f58d70e1b0f).Replace('.', '_')).Append(' ').Append(array2[num2].Name).Append(" = ").Append(num2 + 1).Append(';');
					num2++;
					flag = ((uint)num - (uint)x94e516b5d3c734eb > uint.MaxValue);
					if (flag)
					{
						return;
					}
					if ((uint)i - (uint)num >= 0U)
					{
						continue;
					}
					break;
					IL_5E8:
					if (!(array2[num2] is FieldInfo))
					{
						goto IL_5F5;
					}
					x7bc89d21aec0483a = ((FieldInfo)array2[num2]).FieldType;
					goto IL_612;
				}
				if (255 != 0)
				{
					goto IL_47C;
				}
				goto IL_4DC;
			}
			string value3 = this.xad70a5849826ecef.xe2423e5eb2c7ed45(TypeModel.x4431fb61faccece0(this.xad70a5849826ecef, this.x43163d22e8cd5a71), DataFormat.Default, false, false, ref x55b32f58d70e1b0f);
			MetaType.xd64bebcffdef70ee(xd07ce4b74c5774a7, x94e516b5d3c734eb).Append("message ").Append(this.xe2423e5eb2c7ed45()).Append(" {");
			MetaType.xd64bebcffdef70ee(xd07ce4b74c5774a7, x94e516b5d3c734eb + 1).Append("repeated ").Append(value3).Append(" items = 1;");
			MetaType.xd64bebcffdef70ee(xd07ce4b74c5774a7, x94e516b5d3c734eb).Append('}');
			flag = ((uint)num3 > uint.MaxValue);
			if (flag)
			{
				return;
			}
		}

		private const byte x19de1cdcec1fbf45 = 1;

		private const byte x2d0c3fa60242cd8d = 2;

		private const byte xf2b2ffe9aafadd32 = 4;

		private const byte xd3928ae3d111f9a8 = 8;

		private const byte x222e0934e60e0712 = 16;

		private const byte x3ffced14b38b8e4c = 32;

		private const byte x071a2f72c6532a22 = 64;

		private const byte x189461051823a43b = 128;

		private MetaType x6227b26cefabe265;

		private x826e0336b5da6af5 x54cc87219996ce22;

		internal static readonly Type xe6867eee5ccb3249 = typeof(IEnumerable);

		private CallbackSet xbc2aaa430ff675df;

		private string xc15bd84e01929885;

		private MethodInfo x64b16fcabdb0518e;

		private readonly RuntimeTypeModel xad70a5849826ecef;

		private readonly Type x43163d22e8cd5a71;

		private x9da713b4847e9e6e x38c907674383c2da;

		private Type x718bc396727ef5e4;

		private Type xd467cf11a28aa761;

		private readonly x826e0336b5da6af5 xa942970cc8a85fd4 = new x826e0336b5da6af5();

		private volatile byte xebf45bdcaa1fd1e1;

		[Flags]
		internal enum x41998983fed2df58
		{
			x4d0b9d4447ba7566 = 0,
			x430652ca16c43c9b = 1,
			x6037fb37e2f942e7 = 2,
			xd2e84228f4f88898 = 4,
			xc0b8234e2c894eae = 8
		}

		internal class xf7b8e510ae9ad738 : IComparer<MetaType>, IComparer
		{
			public int Compare(object x, object y)
			{
				return this.Compare(x as MetaType, y as MetaType);
			}

			public int Compare(MetaType x, MetaType y)
			{
				if (object.ReferenceEquals(x, y))
				{
					if (!false)
					{
						return 0;
					}
				}
				if (x == null)
				{
					return -1;
				}
				if (y == null)
				{
					return 1;
				}
				if (!false)
				{
					return string.Compare(x.xe2423e5eb2c7ed45(), y.xe2423e5eb2c7ed45(), StringComparison.Ordinal);
				}
				return 0;
			}

			public static readonly MetaType.xf7b8e510ae9ad738 xb9715d2f06b63cf0 = new MetaType.xf7b8e510ae9ad738();
		}
	}
}
