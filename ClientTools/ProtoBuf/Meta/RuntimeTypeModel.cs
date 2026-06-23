using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using ProtoBuf.Serializers;

namespace ProtoBuf.Meta
{
	public sealed class RuntimeTypeModel : TypeModel
	{
		private bool xe7529aed1cafd669(byte x804d9de34bfbea97)
		{
			return (this.xdfde339da46db651 & x804d9de34bfbea97) == x804d9de34bfbea97;
		}

		private void xbac772fd6410a5cc(byte x804d9de34bfbea97, bool xbcea506a33cf9111)
		{
			if (xbcea506a33cf9111)
			{
				this.xdfde339da46db651 |= x804d9de34bfbea97;
				return;
			}
			this.xdfde339da46db651 &= ~x804d9de34bfbea97;
		}

		public bool InferTagFromNameDefault
		{
			get
			{
				return this.xe7529aed1cafd669(1);
			}
			set
			{
				this.xbac772fd6410a5cc(1, value);
			}
		}

		public bool AutoAddProtoContractTypesOnly
		{
			get
			{
				return this.xe7529aed1cafd669(128);
			}
			set
			{
				this.xbac772fd6410a5cc(128, value);
			}
		}

		public bool UseImplicitZeroDefaults
		{
			get
			{
				return this.xe7529aed1cafd669(32);
			}
			set
			{
				if (!value)
				{
					if (2147483647 != 0)
					{
					}
					if (this.xe7529aed1cafd669(2))
					{
						throw new InvalidOperationException("UseImplicitZeroDefaults cannot be disabled on the default model");
					}
				}
				this.xbac772fd6410a5cc(32, value);
			}
		}

		public bool AllowParseableTypes
		{
			get
			{
				return this.xe7529aed1cafd669(64);
			}
			set
			{
				if (value && this.xe7529aed1cafd669(2))
				{
					throw new InvalidOperationException("AllowParseableTypes cannot be enabled on the default model");
				}
				this.xbac772fd6410a5cc(64, value);
			}
		}

		public static RuntimeTypeModel Default
		{
			get
			{
				return RuntimeTypeModel.xf4d30283df9a7825.xd2f68ee6f47e9dfb;
			}
		}

		public IEnumerable GetTypes()
		{
			return this.x8f76a1a9d286e18b;
		}

		public override string GetSchema(Type type)
		{
			x826e0336b5da6af5 x826e0336b5da6af = new x826e0336b5da6af5();
			bool flag2;
			bool flag3;
			bool flag = (flag2 ? 1U : 0U) - (flag3 ? 1U : 0U) > uint.MaxValue;
			if (flag)
			{
				goto IL_421;
			}
			MetaType metaType;
			int num;
			int num2;
			do
			{
				IL_625:
				metaType = null;
				flag2 = false;
				while (type == null)
				{
					using (IEnumerator enumerator = this.x8f76a1a9d286e18b.GetEnumerator())
					{
						for (;;)
						{
							if (enumerator.MoveNext())
							{
								goto IL_5C6;
							}
							if (((uint)num & 0U) == 0U)
							{
								break;
							}
							if ((uint)num2 <= 4294967295U)
							{
								goto IL_5C6;
							}
							IL_5D3:
							MetaType metaType3;
							MetaType metaType2 = metaType3.xda6e605e9a5fa5d2();
							if (((uint)num2 & 0U) == 0U)
							{
								goto IL_58D;
							}
							IL_578:
							x826e0336b5da6af.xd6b6ed77479ef68c(metaType2);
							this.xeb40dc8f3537e706(x826e0336b5da6af, metaType2);
							if (!false)
							{
								continue;
							}
							IL_58D:
							if (x826e0336b5da6af.x263d579af1d0d43f(metaType2))
							{
								continue;
							}
							goto IL_578;
							IL_5C6:
							metaType3 = (MetaType)enumerator.Current;
							goto IL_5D3;
						}
						goto IL_43B;
					}
				}
				Type type2 = x479f2661aae93792.xe5e08d1dc9f521de(type);
				if (type2 != null)
				{
					type = type2;
				}
				WireType wireType;
				flag2 = (ValueMember.xac123cbb8d7a8e65(this, DataFormat.Default, type, out wireType, false, false, false, false) != null);
			}
			while (8 == 0);
			flag = ((uint)num - (uint)num2 < 0U);
			if (flag)
			{
				goto IL_649;
			}
			goto IL_509;
			IL_33:
			if (!flag3)
			{
				if ((flag2 ? 1U : 0U) + (uint)num > 4294967295U)
				{
					goto IL_435;
				}
				flag = ((flag3 ? 1U : 0U) + (flag2 ? 1U : 0U) > uint.MaxValue);
				if (!flag)
				{
					goto IL_64E;
				}
				if (((flag2 ? 1U : 0U) & 0U) != 0U)
				{
					goto IL_509;
				}
				goto IL_625;
			}
			IL_57:
			StringBuilder stringBuilder;
			stringBuilder.Append("import \"bcl.proto\" // schema for protobuf-net's handling of core .NET types");
			flag = (((uint)num2 & 0U) == 0U);
			if (!flag)
			{
				goto IL_BC;
			}
			if (!false)
			{
				x479f2661aae93792.xfa6a23a9b7368053(stringBuilder);
				goto IL_64E;
			}
			flag = ((uint)num2 - (flag3 ? 1U : 0U) > uint.MaxValue);
			if (flag)
			{
				goto IL_10B;
			}
			goto IL_146;
			IL_8C:
			num++;
			IL_92:
			MetaType[] array;
			if (num >= array.Length)
			{
				goto IL_33;
			}
			goto IL_146;
			IL_9F:
			if ((flag3 ? 1U : 0U) + (flag2 ? 1U : 0U) < 0U)
			{
				goto IL_F3;
			}
			IL_B7:
			MetaType metaType4;
			if (metaType4 == metaType)
			{
				goto IL_D9;
			}
			IL_BC:
			if ((uint)num <= 4294967295U)
			{
				goto IL_8C;
			}
			IL_BE:
			if (!false)
			{
				goto IL_8C;
			}
			goto IL_9F;
			IL_D9:
			flag = ((uint)num - (flag3 ? 1U : 0U) < 0U);
			if (flag)
			{
				goto IL_10B;
			}
			goto IL_135;
			IL_F3:
			if ((uint)num + (flag3 ? 1U : 0U) > 4294967295U)
			{
				goto IL_D9;
			}
			IL_10B:
			if (metaType4.x2e7a2ea5da15ce85)
			{
				if ((uint)num <= 4294967295U)
				{
					goto IL_B7;
				}
				goto IL_F3;
			}
			IL_135:
			StringBuilder stringBuilder2;
			metaType4.x31407de8873120a6(stringBuilder2, 0, ref flag3);
			IL_141:
			goto IL_BE;
			IL_146:
			metaType4 = array[num];
			if (2 == 0)
			{
				goto IL_B7;
			}
			goto IL_10B;
			IL_421:
			if (((uint)num & 0U) == 0U)
			{
				goto IL_43B;
			}
			IL_435:
			if (!flag2)
			{
				goto IL_4E7;
			}
			IL_43B:
			stringBuilder = new StringBuilder();
			string text = null;
			flag = ((uint)num < 0U);
			if (flag || !flag2)
			{
				IEnumerable enumerable;
				if (metaType == null)
				{
					enumerable = this.x8f76a1a9d286e18b;
					goto IL_402;
				}
				flag = ((uint)num2 + (flag3 ? 1U : 0U) > uint.MaxValue);
				if (flag)
				{
					goto IL_478;
				}
				IL_3F9:
				enumerable = x826e0336b5da6af;
				IL_402:
				IEnumerable enumerable2 = enumerable;
				if ((flag3 ? 1U : 0U) + (flag2 ? 1U : 0U) > 4294967295U)
				{
					goto IL_15E;
				}
				IL_478:
				IEnumerator enumerator2 = enumerable2.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						object obj = enumerator2.Current;
						MetaType metaType5 = (MetaType)obj;
						if (!metaType5.x2e7a2ea5da15ce85)
						{
							string @namespace = metaType5.Type.Namespace;
							if (x479f2661aae93792.x1c140bd1078ddda1(@namespace))
							{
								if ((flag2 ? 1U : 0U) - (uint)num2 >= 0U)
								{
									if (((uint)num | 4294967295U) != 0U)
									{
										continue;
									}
									goto IL_375;
								}
							}
							if (@namespace.StartsWith("System."))
							{
								continue;
							}
							if (text != null)
							{
								if (!(text == @namespace))
								{
									text = null;
									IL_39B:
									goto IL_239;
								}
								continue;
							}
							IL_375:
							text = @namespace;
						}
					}
					goto IL_39B;
				}
				finally
				{
					IDisposable disposable2 = enumerator2 as IDisposable;
					if ((uint)num2 - (flag3 ? 1U : 0U) <= 4294967295U)
					{
					}
					while (disposable2 != null)
					{
						disposable2.Dispose();
						flag = ((uint)num - (flag2 ? 1U : 0U) > uint.MaxValue);
						if (!flag && 4 != 0)
						{
							break;
						}
					}
				}
				goto IL_3F9;
			}
			if ((uint)num + (uint)num2 > 4294967295U)
			{
				goto IL_57;
			}
			IL_239:
			while (!x479f2661aae93792.x1c140bd1078ddda1(text))
			{
				stringBuilder.Append("package ").Append(text).Append(';');
				x479f2661aae93792.xfa6a23a9b7368053(stringBuilder);
				flag = ((uint)num2 < 0U);
				if (!flag)
				{
					IL_216:
					flag3 = false;
					if ((flag3 ? 1U : 0U) + (flag3 ? 1U : 0U) > 4294967295U)
					{
						goto IL_BC;
					}
					stringBuilder2 = new StringBuilder();
					array = new MetaType[x826e0336b5da6af.xd44988f225497f3a];
					x826e0336b5da6af.x0fe4f26e70030075(array, 0);
					Array.Sort<MetaType>(array, MetaType.xf7b8e510ae9ad738.xb9715d2f06b63cf0);
					if (!flag2)
					{
						num = 0;
						goto IL_15E;
					}
					x479f2661aae93792.xfa6a23a9b7368053(stringBuilder2).Append("message ").Append(type.Name).Append(" {");
					MetaType.xd64bebcffdef70ee(stringBuilder2, 1).Append("optional ").Append(this.xe2423e5eb2c7ed45(type, DataFormat.Default, false, false, ref flag3)).Append(" value = 1;");
					x479f2661aae93792.xfa6a23a9b7368053(stringBuilder2).Append('}');
					goto IL_33;
				}
			}
			if ((uint)num2 - (uint)num2 >= 0U)
			{
				goto IL_216;
			}
			goto IL_141;
			IL_15E:
			goto IL_92;
			IL_4E7:
			num2 = this.x8a1227c17a49f070(type, false, false, false);
			if (num2 >= 0)
			{
				metaType = ((MetaType)this.x8f76a1a9d286e18b.get_xe6d4b1b411ed94b5(num2)).xda6e605e9a5fa5d2();
				x826e0336b5da6af.xd6b6ed77479ef68c(metaType);
				this.xeb40dc8f3537e706(x826e0336b5da6af, metaType);
				goto IL_531;
			}
			throw new ArgumentException("The type specified is not a contract-type", "type");
			IL_509:
			if (8 == 0)
			{
				goto IL_9F;
			}
			flag = ((flag2 ? 1U : 0U) - (uint)num > uint.MaxValue);
			if (!flag)
			{
				goto IL_435;
			}
			if (!false)
			{
				goto IL_4E7;
			}
			IL_531:
			IL_649:
			goto IL_421;
			IL_64E:
			return x479f2661aae93792.xfa6a23a9b7368053(stringBuilder.Append(stringBuilder2)).ToString();
		}

		private void xeb40dc8f3537e706(x826e0336b5da6af5 x8a0b266419f09a55, MetaType xb4f410bbdbde11cb)
		{
			if (xb4f410bbdbde11cb.x2e7a2ea5da15ce85)
			{
				goto IL_48D;
			}
			int num;
			bool flag;
			int num2;
			int num3;
			MetaType metaType;
			if (!xb4f410bbdbde11cb.x455b3702109fb5db)
			{
				foreach (object obj in xb4f410bbdbde11cb.x84aa3570d857bec4)
				{
					ValueMember valueMember = (ValueMember)obj;
					flag = (((uint)num & 0U) == 0U);
					if (flag)
					{
						Type type = valueMember.ItemType;
						if (type == null)
						{
							type = valueMember.MemberType;
						}
						WireType wireType;
						x66ec8c25e4c7547d x66ec8c25e4c7547d = ValueMember.xac123cbb8d7a8e65(this, DataFormat.Default, type, out wireType, false, false, false, false);
						if (x66ec8c25e4c7547d != null)
						{
							if (((uint)num2 & 0U) != 0U)
							{
								break;
							}
							continue;
						}
						else
						{
							num3 = this.x8a1227c17a49f070(type, false, false, false);
							if (num3 < 0)
							{
								continue;
							}
							metaType = ((MetaType)this.x8f76a1a9d286e18b.get_xe6d4b1b411ed94b5(num3)).xda6e605e9a5fa5d2();
						}
					}
					if (!x8a0b266419f09a55.x263d579af1d0d43f(metaType) && -2147483648 != 0)
					{
						x8a0b266419f09a55.xd6b6ed77479ef68c(metaType);
						this.xeb40dc8f3537e706(x8a0b266419f09a55, metaType);
					}
				}
				goto IL_273;
			}
			MemberInfo[] array;
			if (MetaType.x3fb5a247ee976f3f(xb4f410bbdbde11cb.Type, out array) != null)
			{
				num2 = 0;
				goto IL_156;
			}
			goto IL_273;
			IL_17:
			if (metaType == null)
			{
				return;
			}
			IL_1A:
			if (x8a0b266419f09a55.x263d579af1d0d43f(metaType))
			{
				return;
			}
			x8a0b266419f09a55.xd6b6ed77479ef68c(metaType);
			IL_52:
			this.xeb40dc8f3537e706(x8a0b266419f09a55, metaType);
			IL_5A:
			return;
			IL_8C:
			metaType = metaType.x819a5515328d902f();
			flag = ((uint)num < 0U);
			if (flag)
			{
				goto IL_1A;
			}
			goto IL_17;
			IL_B0:
			metaType = xb4f410bbdbde11cb.BaseType;
			if (metaType == null)
			{
				goto IL_17;
			}
			int i;
			if ((uint)i + (uint)num2 >= 0U)
			{
				goto IL_8C;
			}
			goto IL_5A;
			IL_156:
			Type x43163d22e8cd5a;
			if (num2 < array.Length)
			{
				x43163d22e8cd5a = null;
				goto IL_3E6;
			}
			IL_273:
			if (!xb4f410bbdbde11cb.HasSubtypes)
			{
				goto IL_B0;
			}
			IL_2A3:
			IL_370:
			int num4;
			flag = ((uint)num4 - (uint)num4 > uint.MaxValue);
			if (flag)
			{
				goto IL_48D;
			}
			goto IL_4EC;
			IL_3E6:
			flag = ((uint)num4 - (uint)num < 0U);
			if (flag || array[num2] is PropertyInfo)
			{
				x43163d22e8cd5a = ((PropertyInfo)array[num2]).PropertyType;
				goto IL_330;
			}
			IL_306:
			while (array[num2] is FieldInfo)
			{
				x43163d22e8cd5a = ((FieldInfo)array[num2]).FieldType;
				flag = ((uint)num3 - (uint)num3 > uint.MaxValue);
				if (!flag)
				{
					goto IL_330;
				}
				if ((uint)num2 + (uint)num > 4294967295U)
				{
					return;
				}
				if (((uint)num3 & 0U) != 0U)
				{
					goto IL_433;
				}
			}
			if ((uint)i - (uint)num > 4294967295U)
			{
				goto IL_52;
			}
			IL_330:
			WireType wireType2;
			x66ec8c25e4c7547d x66ec8c25e4c7547d2 = ValueMember.xac123cbb8d7a8e65(this, DataFormat.Default, x43163d22e8cd5a, out wireType2, false, false, false, false);
			flag = ((uint)num < 0U);
			if (flag)
			{
				goto IL_356;
			}
			if (x66ec8c25e4c7547d2 == null)
			{
				num4 = this.x8a1227c17a49f070(x43163d22e8cd5a, false, false, false);
				if (num4 >= 0)
				{
					metaType = ((MetaType)this.x8f76a1a9d286e18b.get_xe6d4b1b411ed94b5(num4)).xda6e605e9a5fa5d2();
					goto IL_356;
				}
			}
			IL_288:
			num2++;
			flag = ((uint)num < 0U);
			if (flag)
			{
				goto IL_2A3;
			}
			goto IL_156;
			IL_356:
			if (x8a0b266419f09a55.x263d579af1d0d43f(metaType))
			{
				goto IL_288;
			}
			x8a0b266419f09a55.xd6b6ed77479ef68c(metaType);
			flag = ((uint)num - (uint)num3 < 0U);
			if (flag)
			{
				goto IL_306;
			}
			flag = ((uint)num4 > uint.MaxValue);
			if (flag)
			{
				goto IL_370;
			}
			this.xeb40dc8f3537e706(x8a0b266419f09a55, metaType);
			goto IL_288;
			IL_433:
			this.xeb40dc8f3537e706(x8a0b266419f09a55, metaType);
			return;
			IL_48D:
			Type x43163d22e8cd5a2 = TypeModel.x4431fb61faccece0(this, xb4f410bbdbde11cb.Type);
			WireType wireType3;
			if (ValueMember.xac123cbb8d7a8e65(this, DataFormat.Default, x43163d22e8cd5a2, out wireType3, false, false, false, false) != null)
			{
				return;
			}
			num = this.x8a1227c17a49f070(x43163d22e8cd5a2, false, false, false);
			if (((uint)num2 & 0U) == 0U)
			{
				if (num < 0)
				{
					return;
				}
			}
			metaType = ((MetaType)this.x8f76a1a9d286e18b.get_xe6d4b1b411ed94b5(num)).xda6e605e9a5fa5d2();
			if (x8a0b266419f09a55.x263d579af1d0d43f(metaType))
			{
				flag = ((uint)num2 < 0U);
				if (flag)
				{
					goto IL_17;
				}
				return;
			}
			else
			{
				x8a0b266419f09a55.xd6b6ed77479ef68c(metaType);
				if (false)
				{
					goto IL_8C;
				}
				flag = ((uint)num + (uint)i > uint.MaxValue);
				if (!flag)
				{
					goto IL_433;
				}
			}
			IL_4EC:
			flag = ((uint)i > uint.MaxValue);
			if (!flag)
			{
				SubType[] subtypes = xb4f410bbdbde11cb.GetSubtypes();
				i = 0;
				IL_A5:
				while (i < subtypes.Length)
				{
					SubType subType = subtypes[i];
					metaType = subType.DerivedType.x819a5515328d902f();
					if ((uint)num2 + (uint)i >= 0U)
					{
						while (!x8a0b266419f09a55.x263d579af1d0d43f(metaType))
						{
							x8a0b266419f09a55.xd6b6ed77479ef68c(metaType);
							this.xeb40dc8f3537e706(x8a0b266419f09a55, metaType);
							if (!false)
							{
								IL_9F:
								i++;
								goto IL_A5;
							}
						}
						goto IL_9F;
					}
					goto IL_3E6;
				}
				goto IL_B0;
			}
		}

		internal RuntimeTypeModel(bool isDefault)
		{
			this.AutoAddMissingTypes = true;
			this.UseImplicitZeroDefaults = true;
			this.xbac772fd6410a5cc(2, isDefault);
		}

		public MetaType this[Type type]
		{
			get
			{
				return (MetaType)this.x8f76a1a9d286e18b.get_xe6d4b1b411ed94b5(this.x8a1227c17a49f070(type, true, false, false));
			}
		}

		internal MetaType x36fdee630175c0fa(Type x43163d22e8cd5a71)
		{
			foreach (object obj in this.x8f76a1a9d286e18b)
			{
				MetaType metaType = (MetaType)obj;
				if (metaType.Type == x43163d22e8cd5a71)
				{
					if (metaType.xfad393511d4f7236)
					{
						this.x1386456832107deb(metaType);
						if (false)
						{
							continue;
						}
					}
					return metaType;
				}
			}
			Type type = TypeModel.ResolveProxies(x43163d22e8cd5a71);
			if (type != null)
			{
				return this.x36fdee630175c0fa(type);
			}
			return null;
		}

		private void x1386456832107deb(MetaType x43163d22e8cd5a71)
		{
			int x53a1af2280f1a = 0;
			try
			{
				this.x2c0c646a23b0da22(ref x53a1af2280f1a);
			}
			finally
			{
				this.x7109eb31e17d8f9e(x53a1af2280f1a);
			}
		}

		internal int x8a1227c17a49f070(Type x43163d22e8cd5a71, bool xcfbb09eff68775f8, bool xaba817394f04589d, bool xd64f6138fdbf6ce3)
		{
			RuntimeTypeModel.xc0462604e8686057 x0a7c6450dfaa2f9a = new RuntimeTypeModel.xc0462604e8686057(x43163d22e8cd5a71);
			int i = this.x8f76a1a9d286e18b.x2ee5ad3d826ed0fe(x0a7c6450dfaa2f9a);
			if (i < 0)
			{
				goto IL_2F1;
			}
			int num;
			bool flag2;
			bool flag = (uint)num - (flag2 ? 1U : 0U) < 0U;
			if (flag)
			{
				goto IL_3AD;
			}
			goto IL_322;
			IL_268:
			MetaType metaType;
			while (i < 0)
			{
				num = 0;
				try
				{
					this.x2c0c646a23b0da22(ref num);
					flag = (((flag2 ? 1U : 0U) & 0U) == 0U);
					if (!flag)
					{
						goto IL_217;
					}
					if (false)
					{
						goto IL_118;
					}
					if ((metaType = this.x427e8fcb8c4c64c7(x43163d22e8cd5a71)) == null)
					{
						goto IL_217;
					}
					goto IL_120;
					IL_28:
					if (!flag2)
					{
						break;
					}
					metaType.x26ad2611a3d84e83();
					flag = ((xcfbb09eff68775f8 ? 1U : 0U) + (flag2 ? 1U : 0U) > uint.MaxValue);
					if (flag)
					{
						goto IL_6D;
					}
					goto IL_85;
					IL_50:
					int num2;
					i = num2;
					goto IL_28;
					IL_6D:
					this.x2eda32a9e793a6b9();
					flag = ((flag2 ? 1U : 0U) > uint.MaxValue);
					if (!flag)
					{
						i = this.x8f76a1a9d286e18b.xd6b6ed77479ef68c(metaType);
						flag2 = true;
						goto IL_28;
					}
					IL_85:
					metaType.xfad393511d4f7236 = false;
					break;
					IL_118:
					metaType = this.xebcf83b00134300b(x43163d22e8cd5a71);
					IL_120:
					metaType.xfad393511d4f7236 = true;
					flag2 = false;
					if ((xcfbb09eff68775f8 ? 1U : 0U) > 4294967295U)
					{
						goto IL_1AB;
					}
					num2 = this.x8f76a1a9d286e18b.x2ee5ad3d826ed0fe(x0a7c6450dfaa2f9a);
					int num3;
					flag = ((uint)num3 > uint.MaxValue);
					if (flag)
					{
						goto IL_6D;
					}
					if (num2 >= 0)
					{
						goto IL_50;
					}
					goto IL_6D;
					IL_150:
					MetaType.x41998983fed2df58 x41998983fed2df;
					if (!xaba817394f04589d)
					{
						if ((xaba817394f04589d ? 1U : 0U) >= 0U)
						{
							goto IL_118;
						}
						goto IL_1FF;
					}
					else
					{
						if (x41998983fed2df != MetaType.x41998983fed2df58.x4d0b9d4447ba7566)
						{
							goto IL_118;
						}
						goto IL_1AB;
					}
					IL_162:
					bool flag3;
					if (!flag3)
					{
						goto IL_186;
					}
					IL_16A:
					if (!x479f2661aae93792.xb636fcab7a16c388(x43163d22e8cd5a71))
					{
						goto IL_150;
					}
					goto IL_118;
					IL_186:
					if (!xcfbb09eff68775f8)
					{
						if ((flag3 ? 1U : 0U) - (xd64f6138fdbf6ce3 ? 1U : 0U) > 4294967295U)
						{
							goto IL_162;
						}
					}
					else
					{
						TypeModel.ThrowUnexpectedType(x43163d22e8cd5a71);
					}
					return i;
					IL_1AB:
					goto IL_186;
					IL_1B8:
					bool flag4;
					flag3 = flag4;
					flag = ((uint)num2 + (flag3 ? 1U : 0U) > uint.MaxValue);
					if (!flag)
					{
						goto IL_162;
					}
					flag = ((uint)num2 - (flag3 ? 1U : 0U) > uint.MaxValue);
					if (flag)
					{
						goto IL_150;
					}
					goto IL_16A;
					IL_1FF:
					flag4 = xd64f6138fdbf6ce3;
					goto IL_1B8;
					IL_217:
					x41998983fed2df = MetaType.x20a65713c9d99978(this, x43163d22e8cd5a71, null);
					if ((xcfbb09eff68775f8 ? 1U : 0U) - (xcfbb09eff68775f8 ? 1U : 0U) <= 4294967295U)
					{
						if (x41998983fed2df == MetaType.x41998983fed2df58.xc0b8234e2c894eae)
						{
							xd64f6138fdbf6ce3 = true;
						}
						if (this.AutoAddMissingTypes)
						{
							flag4 = true;
							goto IL_1B8;
						}
						goto IL_1FF;
					}
					else
					{
						if ((xaba817394f04589d ? 1U : 0U) - (flag2 ? 1U : 0U) > 4294967295U)
						{
							goto IL_50;
						}
						if ((xd64f6138fdbf6ce3 ? 1U : 0U) + (xcfbb09eff68775f8 ? 1U : 0U) < 0U)
						{
							goto IL_150;
						}
						if ((flag2 ? 1U : 0U) + (uint)i <= 4294967295U)
						{
							goto IL_118;
						}
						if (false)
						{
							goto IL_162;
						}
						goto IL_16A;
					}
					break;
				}
				finally
				{
					this.x7109eb31e17d8f9e(num);
				}
			}
			return i;
			do
			{
				IL_2B9:
				int num2;
				flag = (((uint)num2 & 0U) == 0U);
				if (!flag)
				{
					break;
				}
				bool flag3;
				if ((flag3 ? 1U : 0U) - (xcfbb09eff68775f8 ? 1U : 0U) > 4294967295U)
				{
					goto IL_2CF;
				}
				flag = ((uint)num - (uint)i < 0U);
			}
			while (flag);
			IL_271:
			goto IL_268;
			goto IL_271;
			IL_2CF:
			Type type;
			if (type == null)
			{
				int num3;
				if ((uint)i - (uint)num3 < 0U)
				{
					goto IL_2B9;
				}
				bool flag3;
				if ((flag3 ? 1U : 0U) <= 4294967295U)
				{
					goto IL_268;
				}
			}
			else
			{
				x0a7c6450dfaa2f9a = new RuntimeTypeModel.xc0462604e8686057(type);
			}
			i = this.x8f76a1a9d286e18b.x2ee5ad3d826ed0fe(x0a7c6450dfaa2f9a);
			x43163d22e8cd5a71 = type;
			goto IL_268;
			IL_2F1:
			if (i >= 0)
			{
				if (false)
				{
					goto IL_322;
				}
				goto IL_3AD;
			}
			IL_2FD:
			type = TypeModel.ResolveProxies(x43163d22e8cd5a71);
			goto IL_2CF;
			IL_322:
			if ((metaType = (MetaType)this.x8f76a1a9d286e18b.get_xe6d4b1b411ed94b5(i)).xfad393511d4f7236)
			{
				this.x1386456832107deb(metaType);
				goto IL_2F1;
			}
			flag = ((flag2 ? 1U : 0U) - (flag2 ? 1U : 0U) < 0U);
			if (!flag)
			{
				goto IL_2F1;
			}
			flag = ((xcfbb09eff68775f8 ? 1U : 0U) > uint.MaxValue);
			if (flag)
			{
				goto IL_2F1;
			}
			goto IL_2FD;
			IL_3AD:
			goto IL_2B9;
		}

		private MetaType x427e8fcb8c4c64c7(Type x43163d22e8cd5a71)
		{
			return null;
		}

		private MetaType xebcf83b00134300b(Type x43163d22e8cd5a71)
		{
			this.x2eda32a9e793a6b9();
			return new MetaType(this, x43163d22e8cd5a71);
		}

		public MetaType Add(Type type, bool applyDefaultBehaviour)
		{
			MetaType metaType;
			if (type == null)
			{
				if (!false)
				{
					throw new ArgumentNullException("type");
				}
			}
			else
			{
				metaType = this.x36fdee630175c0fa(type);
			}
			if (metaType != null)
			{
				return metaType;
			}
			int num = 0;
			if (type.IsInterface)
			{
				if (!false)
				{
					goto IL_10A;
				}
				goto IL_10A;
			}
			Block_3:
			try
			{
				IL_1F:
				metaType = this.x427e8fcb8c4c64c7(type);
				for (;;)
				{
					if (metaType != null)
					{
						if (!applyDefaultBehaviour)
						{
							break;
						}
						applyDefaultBehaviour = false;
					}
					if (metaType == null)
					{
						metaType = this.xebcf83b00134300b(type);
					}
					metaType.xfad393511d4f7236 = true;
					bool flag = (uint)num + (uint)num > uint.MaxValue;
					if (!flag)
					{
						goto IL_F6;
					}
				}
				throw new ArgumentException("Default behaviour must be observed for certain types with special handling; " + type.FullName, "applyDefaultBehaviour");
				IL_F6:
				if (((uint)num | 3U) != 0U)
				{
					this.x2c0c646a23b0da22(ref num);
					if (this.x36fdee630175c0fa(type) != null)
					{
						throw new ArgumentException("Duplicate type", "type");
					}
					this.x2eda32a9e793a6b9();
					this.x8f76a1a9d286e18b.xd6b6ed77479ef68c(metaType);
				}
				if (applyDefaultBehaviour)
				{
					metaType.x26ad2611a3d84e83();
				}
				metaType.xfad393511d4f7236 = false;
				return metaType;
			}
			finally
			{
				this.x7109eb31e17d8f9e(num);
			}
			IL_10A:
			if (!base.MapType(MetaType.xe6867eee5ccb3249).IsAssignableFrom(type))
			{
				goto IL_1F;
			}
			if (TypeModel.x4431fb61faccece0(this, type) != null)
			{
				goto Block_3;
			}
			throw new ArgumentException("IEnumerable[<T>] data cannot be used as a meta-type unless an Add method can be resolved");
		}

		public bool AutoAddMissingTypes
		{
			get
			{
				return this.xe7529aed1cafd669(8);
			}
			set
			{
				if (!value)
				{
					while (!this.xe7529aed1cafd669(2))
					{
						if (!false)
						{
							goto IL_05;
						}
					}
					throw new InvalidOperationException("The default model must allow missing types");
				}
				IL_05:
				this.x2eda32a9e793a6b9();
				this.xbac772fd6410a5cc(8, value);
			}
		}

		private void x2eda32a9e793a6b9()
		{
			if (this.xe7529aed1cafd669(4))
			{
				throw new InvalidOperationException("The model cannot be changed once frozen");
			}
		}

		public void Freeze()
		{
			if (this.xe7529aed1cafd669(2))
			{
				throw new InvalidOperationException("The default model cannot be frozen");
			}
			this.xbac772fd6410a5cc(4, true);
		}

		protected override int GetKeyImpl(Type type)
		{
			return this.xf15263674eb297bb(type, false, true);
		}

		internal int xf15263674eb297bb(Type x43163d22e8cd5a71, bool xcfbb09eff68775f8, bool xdfdd2812486e44d2)
		{
			int result;
			try
			{
				int num = this.x8a1227c17a49f070(x43163d22e8cd5a71, xcfbb09eff68775f8, true, false);
				if (false)
				{
					goto IL_44;
				}
				if (num >= 0)
				{
					goto IL_44;
				}
				IL_2D:
				result = num;
				if ((xcfbb09eff68775f8 ? 1U : 0U) < 0U)
				{
					goto IL_56;
				}
				if (!false)
				{
					return result;
				}
				IL_44:
				MetaType metaType = (MetaType)this.x8f76a1a9d286e18b.get_xe6d4b1b411ed94b5(num);
				IL_56:
				if (xdfdd2812486e44d2)
				{
					metaType = MetaType.xe52a8a367f0efe10(metaType);
					num = this.x8a1227c17a49f070(metaType.Type, true, true, false);
					goto IL_2D;
				}
				goto IL_2D;
			}
			catch (NotSupportedException)
			{
				throw;
			}
			catch (Exception ex)
			{
				if (ex.Message.IndexOf(x43163d22e8cd5a71.FullName) < 0)
				{
					throw new ProtoException(ex.Message + " (" + x43163d22e8cd5a71.FullName + ")", ex);
				}
				throw;
			}
			return result;
		}

		protected internal override void Serialize(int key, object value, ProtoWriter dest)
		{
			((MetaType)this.x8f76a1a9d286e18b.get_xe6d4b1b411ed94b5(key)).x9e41724c73da1842.x6210059f049f0d48(value, dest);
		}

		protected internal override object Deserialize(int key, object value, ProtoReader source)
		{
			x66ec8c25e4c7547d x9e41724c73da = ((MetaType)this.x8f76a1a9d286e18b.get_xe6d4b1b411ed94b5(key)).x9e41724c73da1842;
			while (value == null)
			{
				if (!x479f2661aae93792.x25f40ad8c018c1ab(x9e41724c73da.x00c54479c3a7c440))
				{
					IL_50:
					return x9e41724c73da.x06b0e25aa6ad68a9(value, source);
				}
				if (!false)
				{
					if (x9e41724c73da.x95726b5912481139)
					{
						value = Activator.CreateInstance(x9e41724c73da.x00c54479c3a7c440);
					}
					return x9e41724c73da.x06b0e25aa6ad68a9(value, source);
				}
			}
			goto IL_50;
		}

		internal bool xf491e4040205bc52(Type x43163d22e8cd5a71, int xade3b695478596d6)
		{
			return this.x36fdee630175c0fa(x43163d22e8cd5a71).xf491e4040205bc52(xade3b695478596d6);
		}

		internal bool x0242a1cd267232b1(Type x43163d22e8cd5a71)
		{
			MetaType metaType = this.x36fdee630175c0fa(x43163d22e8cd5a71);
			return metaType != null && metaType.x0242a1cd267232b1();
		}

		internal x9caf06b8b9c3a409.EnumPair[] x0b51e8612d340c57(Type x43163d22e8cd5a71)
		{
			int num = this.x8a1227c17a49f070(x43163d22e8cd5a71, false, false, false);
			if (num < 0)
			{
				return null;
			}
			return ((MetaType)this.x8f76a1a9d286e18b.get_xe6d4b1b411ed94b5(num)).x0b51e8612d340c57();
		}

		public int MetadataTimeoutMilliseconds
		{
			get
			{
				return this.xda83f0037c055876;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("MetadataTimeoutMilliseconds");
				}
				this.xda83f0037c055876 = value;
			}
		}

		internal void x2c0c646a23b0da22(ref int x53a1af2280f1a844)
		{
			x53a1af2280f1a844 = 0;
			if (4 == 0 || (!false && !Monitor.TryEnter(this.x8f76a1a9d286e18b, this.xda83f0037c055876)))
			{
				this.x6b26c89aeacb4db3();
				throw new TimeoutException("Timeout while inspecting metadata; this may indicate a deadlock. This can often be avoided by preparing necessary serializers during application initialization, rather than allowing multiple threads to perform the initial metadata inspection; please also see the LockContended event");
			}
			x53a1af2280f1a844 = this.x3d56d5a22c8ee343();
		}

		private int x3d56d5a22c8ee343()
		{
			return Interlocked.CompareExchange(ref this.x4b4d2d8e6068adeb, 0, 0);
		}

		private void x6b26c89aeacb4db3()
		{
			Interlocked.Increment(ref this.x4b4d2d8e6068adeb);
		}

		internal void x7109eb31e17d8f9e(int x53a1af2280f1a844)
		{
			if (x53a1af2280f1a844 != 0)
			{
				Monitor.Exit(this.x8f76a1a9d286e18b);
				if (x53a1af2280f1a844 != this.x3d56d5a22c8ee343())
				{
					LockContentedEventHandler lockContentedEventHandler;
					for (;;)
					{
						lockContentedEventHandler = this.x6a504e095459aa3a;
						if (false)
						{
							goto IL_4B;
						}
						while (!false)
						{
							if (lockContentedEventHandler != null)
							{
								goto IL_1B;
							}
							if (!false)
							{
								if (-1 == 0)
								{
									break;
								}
								goto IL_4B;
							}
						}
					}
					string stackTrace;
					try
					{
						IL_1B:
						throw new Exception();
					}
					catch (Exception ex)
					{
						stackTrace = ex.StackTrace;
					}
					lockContentedEventHandler(this, new LockContentedEventArgs(stackTrace));
					IL_4B:;
				}
			}
		}

		public event LockContentedEventHandler LockContended
		{
			add
			{
				LockContentedEventHandler lockContentedEventHandler = this.x6a504e095459aa3a;
				LockContentedEventHandler lockContentedEventHandler2;
				do
				{
					lockContentedEventHandler2 = lockContentedEventHandler;
					LockContentedEventHandler value2 = (LockContentedEventHandler)Delegate.Combine(lockContentedEventHandler2, value);
					lockContentedEventHandler = Interlocked.CompareExchange<LockContentedEventHandler>(ref this.x6a504e095459aa3a, value2, lockContentedEventHandler2);
				}
				while (lockContentedEventHandler != lockContentedEventHandler2);
			}
			remove
			{
				LockContentedEventHandler lockContentedEventHandler = this.x6a504e095459aa3a;
				LockContentedEventHandler lockContentedEventHandler2;
				do
				{
					lockContentedEventHandler2 = lockContentedEventHandler;
					LockContentedEventHandler value2 = (LockContentedEventHandler)Delegate.Remove(lockContentedEventHandler2, value);
					lockContentedEventHandler = Interlocked.CompareExchange<LockContentedEventHandler>(ref this.x6a504e095459aa3a, value2, lockContentedEventHandler2);
				}
				while (lockContentedEventHandler != lockContentedEventHandler2);
			}
		}

		internal void xaadd3c2a45ddcdb5(Type x43163d22e8cd5a71, ref Type xd99217279677497c, ref Type x284cae105ee79236)
		{
			if (x43163d22e8cd5a71 != null)
			{
				IL_239:
				while (x479f2661aae93792.xf70eec89828a813c(x43163d22e8cd5a71) == xd669244d58bc09c0.xf6c17f648b65c793)
				{
					if (8 != 0)
					{
						if (false)
						{
							if (-2147483648 == 0)
							{
								goto IL_77;
							}
						}
						else if (!this[x43163d22e8cd5a71].IgnoreListHandling)
						{
							if (2147483647 == 0)
							{
								goto IL_264;
							}
							if (x43163d22e8cd5a71.IsArray)
							{
								if (x43163d22e8cd5a71.GetArrayRank() != 1)
								{
									if (15 != 0)
									{
										throw new NotSupportedException("Multi-dimension arrays are supported");
									}
									goto IL_14B;
								}
								else
								{
									xd99217279677497c = x43163d22e8cd5a71.GetElementType();
									if (xd99217279677497c != base.MapType(typeof(byte)))
									{
										x284cae105ee79236 = x43163d22e8cd5a71;
									}
									else
									{
										Type type;
										xd99217279677497c = (type = null);
										x284cae105ee79236 = type;
									}
								}
							}
							while (xd99217279677497c == null)
							{
								xd99217279677497c = TypeModel.x4431fb61faccece0(this, x43163d22e8cd5a71);
								if (!false)
								{
									IL_18A:
									if (xd99217279677497c == null)
									{
										if (3 == 0)
										{
											goto IL_1D;
										}
									}
									else
									{
										Type type2 = null;
										Type type3 = null;
										this.xaadd3c2a45ddcdb5(xd99217279677497c, ref type2, ref type3);
										if (type2 != null)
										{
											throw TypeModel.x65d41140f79f7ce3();
										}
									}
									while (xd99217279677497c == null)
									{
										if (false)
										{
											break;
										}
										if (2 != 0)
										{
											return;
										}
									}
									IL_1D:
									if (x284cae105ee79236 != null)
									{
										return;
									}
									goto IL_130;
								}
							}
							goto IL_18A;
						}
						return;
					}
					IL_5F:
					Type[] genericArguments;
					while (x43163d22e8cd5a71.GetGenericTypeDefinition() == base.MapType(typeof(IDictionary<, >)) && xd99217279677497c == base.MapType(typeof(KeyValuePair<, >)).MakeGenericType(genericArguments = x43163d22e8cd5a71.GetGenericArguments()))
					{
						if (-1 == 0)
						{
							goto IL_115;
						}
						if (15 != 0)
						{
							x284cae105ee79236 = base.MapType(typeof(Dictionary<, >)).MakeGenericType(genericArguments);
							if (!false)
							{
								goto IL_49;
							}
							goto IL_239;
						}
					}
					goto IL_77;
					IL_125:
					if (x284cae105ee79236 != null)
					{
						goto IL_49;
					}
					if (2 != 0 && !x43163d22e8cd5a71.IsInterface)
					{
						if (false)
						{
							goto IL_130;
						}
						goto IL_264;
					}
					else
					{
						if (x43163d22e8cd5a71.IsGenericType)
						{
							goto IL_5F;
						}
						goto IL_77;
					}
					IL_115:
					goto IL_125;
					IL_117:
					if (x479f2661aae93792.xdaa1a96eb962bff0(x43163d22e8cd5a71, x479f2661aae93792.xf6f6ea67665595b8, true) == null)
					{
						goto IL_125;
					}
					IL_14B:
					x284cae105ee79236 = x43163d22e8cd5a71;
					if (!false)
					{
						goto IL_125;
					}
					goto IL_117;
					IL_130:
					if (!x43163d22e8cd5a71.IsClass)
					{
						goto IL_115;
					}
					if (!x43163d22e8cd5a71.IsAbstract)
					{
						goto IL_117;
					}
					goto IL_125;
					IL_77:
					x284cae105ee79236 = base.MapType(typeof(List<>)).MakeGenericType(new Type[]
					{
						xd99217279677497c
					});
					IL_264:
					goto IL_49;
				}
				return;
			}
			if (255 != 0)
			{
				return;
			}
			IL_49:
			if (x284cae105ee79236 != null)
			{
				if (!x479f2661aae93792.xd3703a5e339a1b56(x43163d22e8cd5a71, x284cae105ee79236))
				{
					x284cae105ee79236 = null;
				}
			}
		}

		internal string xe2423e5eb2c7ed45(Type x7bc89d21aec0483a, DataFormat xb0fbb9918378a9ab, bool xa9226d1a5a7dd054, bool x580143c5f0eb0511, ref bool x55b32f58d70e1b0f)
		{
			Type type = x479f2661aae93792.xe5e08d1dc9f521de(x7bc89d21aec0483a);
			if ((x580143c5f0eb0511 ? 1U : 0U) + (x580143c5f0eb0511 ? 1U : 0U) <= 4294967295U)
			{
				xd669244d58bc09c0 xd669244d58bc09c;
				bool flag;
				for (;;)
				{
					if (type != null)
					{
						x7bc89d21aec0483a = type;
					}
					if (x7bc89d21aec0483a == base.MapType(typeof(byte[])))
					{
						goto Block_20;
					}
					WireType wireType;
					x66ec8c25e4c7547d x66ec8c25e4c7547d = ValueMember.xac123cbb8d7a8e65(this, xb0fbb9918378a9ab, x7bc89d21aec0483a, out wireType, false, false, false, false);
					if (x66ec8c25e4c7547d == null)
					{
						goto Block_17;
					}
					if (x66ec8c25e4c7547d is x7024e111a877c7fe)
					{
						goto Block_14;
					}
					xd669244d58bc09c = x479f2661aae93792.xf70eec89828a813c(x7bc89d21aec0483a);
					switch (xd669244d58bc09c)
					{
					case xd669244d58bc09c0.x795dc524dba3fd4b:
						goto IL_157;
					case xd669244d58bc09c0.xfb1fc02db7c42694:
					case xd669244d58bc09c0.xc0f9b651d77da240:
					case xd669244d58bc09c0.xf12cc4804eec7b89:
					case xd669244d58bc09c0.x6cedabde5251b1e5:
						goto IL_C7;
					case xd669244d58bc09c0.x2025ae83be2038f0:
					case xd669244d58bc09c0.x697a219ddc6427a9:
					case xd669244d58bc09c0.x85254000935bfc25:
						goto IL_AD;
					case xd669244d58bc09c0.x0b2292ab52b25d76:
						switch (xb0fbb9918378a9ab)
						{
						case DataFormat.ZigZag:
							goto IL_48;
						case DataFormat.TwosComplement:
							goto IL_36;
						case DataFormat.FixedSize:
							goto IL_30;
						default:
							flag = ((xa9226d1a5a7dd054 ? 1U : 0U) - (x580143c5f0eb0511 ? 1U : 0U) > uint.MaxValue);
							if (flag)
							{
								continue;
							}
							goto IL_297;
						}
						break;
					case xd669244d58bc09c0.x394150f1be471c3c:
						goto IL_90;
					case xd669244d58bc09c0.x63374d6ffed4adeb:
						goto IL_15D;
					case xd669244d58bc09c0.x94c083f2813272f4:
						goto IL_130;
					case xd669244d58bc09c0.x18d3f7d37d3464ca:
						goto IL_18;
					case xd669244d58bc09c0.x242851e6278ed355:
						goto IL_3C;
					case (xd669244d58bc09c0)17:
						goto IL_2BD;
					case xd669244d58bc09c0.x4a498a651d07aefe:
						goto IL_152;
					}
					goto Block_12;
				}
				IL_152:
				while (xa9226d1a5a7dd054)
				{
					x55b32f58d70e1b0f = true;
					flag = ((x580143c5f0eb0511 ? 1U : 0U) - (x580143c5f0eb0511 ? 1U : 0U) < 0U);
					if (!flag)
					{
						IL_123:
						if (xa9226d1a5a7dd054)
						{
							goto IL_DB;
						}
						return "string";
					}
				}
				goto IL_123;
				IL_18:
				x55b32f58d70e1b0f = true;
				return "bcl.Decimal";
				IL_30:
				return "sfixed64";
				IL_36:
				return "int64";
				IL_3C:
				x55b32f58d70e1b0f = true;
				return "bcl.DateTime";
				IL_48:
				return "sint64";
				IL_90:
				flag = (((xa9226d1a5a7dd054 ? 1U : 0U) | 15U) == 0U);
				if (flag)
				{
					goto IL_DB;
				}
				flag = ((xa9226d1a5a7dd054 ? 1U : 0U) + (x580143c5f0eb0511 ? 1U : 0U) < 0U);
				if (flag)
				{
				}
				if (xb0fbb9918378a9ab != DataFormat.FixedSize)
				{
					return "uint64";
				}
				return "fixed64";
				IL_AD:
				switch (xb0fbb9918378a9ab)
				{
				case DataFormat.ZigZag:
					return "sint32";
				case DataFormat.FixedSize:
					return "sfixed32";
				}
				return "int32";
				IL_C7:
				if (xb0fbb9918378a9ab == DataFormat.FixedSize)
				{
					return "fixed32";
				}
				return "uint32";
				IL_DB:
				return "bcl.NetObjectProxy";
				IL_130:
				return "double";
				IL_157:
				return "bool";
				IL_15D:
				return "float";
				Block_12:
				switch (xd669244d58bc09c)
				{
				case xd669244d58bc09c0.x69ec9d2404a6b229:
					x55b32f58d70e1b0f = true;
					return "bcl.TimeSpan";
				case xd669244d58bc09c0.x62a0c09ce3a5e8fb:
					goto IL_2BD;
				case xd669244d58bc09c0.x0217cda8370c1f17:
					x55b32f58d70e1b0f = true;
					return "bcl.Guid";
				default:
					goto IL_2BD;
				}
				Block_14:
				if (xa9226d1a5a7dd054)
				{
					x55b32f58d70e1b0f = true;
				}
				if (!xa9226d1a5a7dd054)
				{
					return "string";
				}
				if ((xa9226d1a5a7dd054 ? 1U : 0U) >= 0U)
				{
					return "bcl.NetObjectProxy";
				}
				goto IL_30;
				Block_17:
				if (!xa9226d1a5a7dd054)
				{
					flag = ((x580143c5f0eb0511 ? 1U : 0U) - (x580143c5f0eb0511 ? 1U : 0U) < 0U);
					if (!flag)
					{
						if (!x580143c5f0eb0511)
						{
							return this[x7bc89d21aec0483a].xda6e605e9a5fa5d2().xe2423e5eb2c7ed45();
						}
					}
				}
				x55b32f58d70e1b0f = true;
				return "bcl.NetObjectProxy";
				Block_20:
				return "bytes";
				IL_297:
				goto IL_36;
			}
			IL_2BD:
			throw new NotSupportedException("No .proto map found for: " + x7bc89d21aec0483a.FullName);
		}

		private const byte x91bf0b37b6daf8d4 = 1;

		private const byte x5d98d6a14f3e0da2 = 2;

		private const byte xf2b2ffe9aafadd32 = 4;

		private const byte xbae9a83ae15e0af4 = 8;

		private const byte x46a2a7ddcd5141b8 = 32;

		private const byte x1dbdd79e5b4a5c38 = 64;

		private const byte x25ee1581cb613420 = 128;

		private byte xdfde339da46db651;

		private readonly x826e0336b5da6af5 x8f76a1a9d286e18b = new x826e0336b5da6af5();

		private int xda83f0037c055876 = 5000;

		private int x4b4d2d8e6068adeb = 1;

		private LockContentedEventHandler x6a504e095459aa3a;

		private sealed class xc0462604e8686057 : x826e0336b5da6af5.xd9fd150330ba5bb7
		{
			public xc0462604e8686057(Type type)
			{
				this.x43163d22e8cd5a71 = type;
			}

			public bool xc313ef0c9ca8870d(object xa59bff7708de3a18)
			{
				return ((MetaType)xa59bff7708de3a18).Type == this.x43163d22e8cd5a71;
			}

			private readonly Type x43163d22e8cd5a71;
		}

		private class xf4d30283df9a7825
		{
			private xf4d30283df9a7825()
			{
			}

			internal static readonly RuntimeTypeModel xd2f68ee6f47e9dfb = new RuntimeTypeModel(true);
		}
	}
}
